using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class TwoFactorSetupInfo
    {
        public string SecretKey { get; set; } = null!;
        public string QRCodeUrl { get; set; } = null!;
        public List<string> BackupCodes { get; set; } = new();
    }

    public interface ITwoFactorAuthService
    {
        Task<TwoFactorSetupInfo> EnableTOTPAsync(string userId, string email, string ipAddress, string tenantId);
        Task<bool> VerifyTOTPAsync(string userId, string code, string tenantId);
        Task<bool> VerifyBackupCodeAsync(string userId, string code, string tenantId);
        Task<List<string>> RegenerateBackupCodesAsync(string userId, string tenantId);
        Task DisableTwoFactorAsync(string userId, string tenantId);
        Task<bool> IsTwoFactorEnabledAsync(string userId, string tenantId);
    }

    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
        private readonly IDataEncryptionService _encryptionService;
        private readonly IPasswordHasher _passwordHasher;
        private const int SecretKeyLength = 20; // 20 bytes = 32 Base32 characters
        private const int TimeStep = 30; // 30 seconds per TOTP code
        private const int BackupCodesCount = 10;

        public TwoFactorAuthService(
            ITwoFactorAuthRepository twoFactorAuthRepository,
            IDataEncryptionService encryptionService,
            IPasswordHasher passwordHasher)
        {
            _twoFactorAuthRepository = twoFactorAuthRepository ?? throw new ArgumentNullException(nameof(twoFactorAuthRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<TwoFactorSetupInfo> EnableTOTPAsync(string userId, string email, string ipAddress, string tenantId)
        {
            var secretKey = GenerateSecretKey();
            
            var twoFactor = new TwoFactorAuth(
                userId,
                TwoFactorMethod.TOTP,
                tenantId,
                _encryptionService.Encrypt(secretKey));
            
            // Generate backup codes
            var backupCodes = new List<string>();
            for (int i = 0; i < BackupCodesCount; i++)
            {
                var code = GenerateBackupCode();
                backupCodes.Add(code);
                var hashedCode = _passwordHasher.HashPassword(code);
                twoFactor.AddBackupCode(FormatBackupCodeForDisplay(code), hashedCode);
            }
            
            twoFactor.Enable(ipAddress);
            await _twoFactorAuthRepository.AddAsync(twoFactor);
            
            return new TwoFactorSetupInfo
            {
                SecretKey = secretKey,
                QRCodeUrl = $"otpauth://totp/PrimeCare:{email}?secret={secretKey}&issuer=PrimeCare",
                BackupCodes = backupCodes
            };
        }

        public async Task<bool> VerifyTOTPAsync(string userId, string code, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            
            if (twoFactor == null || !twoFactor.IsEnabled || twoFactor.SecretKey == null)
                return false;
            
            var secretKey = _encryptionService.Decrypt(twoFactor.SecretKey);
            return VerifyTOTPCode(secretKey, code);
        }

        public async Task<bool> VerifyBackupCodeAsync(string userId, string code, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            
            if (twoFactor == null || !twoFactor.IsEnabled)
                return false;
            
            foreach (var backupCode in twoFactor.BackupCodes)
            {
                if (!backupCode.IsUsed && _passwordHasher.VerifyPassword(code, backupCode.HashedCode))
                {
                    twoFactor.MarkBackupCodeAsUsed(backupCode.Code);
                    await _twoFactorAuthRepository.UpdateAsync(twoFactor);
                    return true;
                }
            }
            
            return false;
        }

        public async Task<List<string>> RegenerateBackupCodesAsync(string userId, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            
            if (twoFactor == null)
                throw new InvalidOperationException("Two-factor authentication not configured for user");
            
            var backupCodes = new List<string>();
            for (int i = 0; i < BackupCodesCount; i++)
            {
                var code = GenerateBackupCode();
                backupCodes.Add(code);
                var hashedCode = _passwordHasher.HashPassword(code);
                twoFactor.AddBackupCode(FormatBackupCodeForDisplay(code), hashedCode);
            }
            
            await _twoFactorAuthRepository.UpdateAsync(twoFactor);
            return backupCodes;
        }

        public async Task DisableTwoFactorAsync(string userId, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            
            if (twoFactor != null)
            {
                twoFactor.Disable();
                await _twoFactorAuthRepository.UpdateAsync(twoFactor);
            }
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string userId, string tenantId)
        {
            return await _twoFactorAuthRepository.IsEnabledForUserAsync(userId, tenantId);
        }

        private string GenerateSecretKey()
        {
            var bytes = new byte[SecretKeyLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base32Encode(bytes);
        }

        private string GenerateBackupCode()
        {
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var number = BitConverter.ToUInt32(bytes, 0) % 100000000; // 8 digits
            return number.ToString("D8");
        }

        private string FormatBackupCodeForDisplay(string code)
        {
            // Format as XXXX-1234 (mask first 4 digits)
            if (code.Length >= 8)
                return $"XXXX-{code[4..]}";
            return code;
        }

        private bool VerifyTOTPCode(string secretKey, string userCode)
        {
            var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / TimeStep;
            
            // Check current time step and ±1 time step for clock drift tolerance
            for (int i = -1; i <= 1; i++)
            {
                var expectedCode = GenerateTOTPCode(secretKey, currentStep + i);
                if (expectedCode == userCode)
                    return true;
            }
            
            return false;
        }

        private string GenerateTOTPCode(string secretKey, long timeStep)
        {
            var secretBytes = Base32Decode(secretKey);
            var timeBytes = BitConverter.GetBytes(timeStep);
            
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeBytes);
            
            using (var hmac = new HMACSHA1(secretBytes))
            {
                var hash = hmac.ComputeHash(timeBytes);
                var offset = hash[hash.Length - 1] & 0x0F;
                var binaryCode = (hash[offset] & 0x7F) << 24
                    | (hash[offset + 1] & 0xFF) << 16
                    | (hash[offset + 2] & 0xFF) << 8
                    | (hash[offset + 3] & 0xFF);
                
                var code = binaryCode % 1000000;
                return code.ToString("D6");
            }
        }

        private string Base32Encode(byte[] data)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var result = new StringBuilder();
            
            for (int i = 0; i < data.Length; i += 5)
            {
                int byteCount = Math.Min(5, data.Length - i);
                ulong buffer = 0;
                
                for (int j = 0; j < byteCount; j++)
                    buffer = (buffer << 8) | data[i + j];
                
                int bitCount = byteCount * 8;
                while (bitCount > 0)
                {
                    int index = (int)((buffer >> (bitCount - 5)) & 0x1F);
                    result.Append(base32Chars[index]);
                    bitCount -= 5;
                }
            }
            
            return result.ToString();
        }

        private byte[] Base32Decode(string input)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            input = input.ToUpper().Replace(" ", "").Replace("-", "");
            
            var bytes = new List<byte>();
            ulong buffer = 0;
            int bitsInBuffer = 0;
            
            foreach (char c in input)
            {
                int value = base32Chars.IndexOf(c);
                if (value < 0)
                    continue;
                
                buffer = (buffer << 5) | (ulong)value;
                bitsInBuffer += 5;
                
                if (bitsInBuffer >= 8)
                {
                    bytes.Add((byte)(buffer >> (bitsInBuffer - 8)));
                    bitsInBuffer -= 8;
                }
            }
            
            return bytes.ToArray();
        }
    }
}
