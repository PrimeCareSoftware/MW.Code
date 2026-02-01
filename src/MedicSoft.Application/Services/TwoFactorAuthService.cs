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
        
        // Email-based 2FA methods
        Task<bool> EnableEmailAsync(string userId, string email, string ipAddress, string tenantId);
        Task<string> GenerateAndSendEmailCodeAsync(string userId, string email, string ipAddress, string purpose, string tenantId);
        Task<bool> VerifyEmailCodeAsync(string userId, string code, string tenantId);
        Task<TwoFactorMethod> GetTwoFactorMethodAsync(string userId, string tenantId);
    }

    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
        private readonly IEmailVerificationTokenRepository _emailTokenRepository;
        private readonly IDataEncryptionService _encryptionService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly MedicSoft.Application.Services.CRM.IEmailService _emailService;
        private const int SecretKeyLength = 20; // 20 bytes = 32 Base32 characters
        private const int TimeStep = 30; // 30 seconds per TOTP code
        private const int BackupCodesCount = 10;

        public TwoFactorAuthService(
            ITwoFactorAuthRepository twoFactorAuthRepository,
            IEmailVerificationTokenRepository emailTokenRepository,
            IDataEncryptionService encryptionService,
            IPasswordHasher passwordHasher,
            MedicSoft.Application.Services.CRM.IEmailService emailService)
        {
            _twoFactorAuthRepository = twoFactorAuthRepository ?? throw new ArgumentNullException(nameof(twoFactorAuthRepository));
            _emailTokenRepository = emailTokenRepository ?? throw new ArgumentNullException(nameof(emailTokenRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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

        // Email-based 2FA implementation
        public async Task<bool> EnableEmailAsync(string userId, string email, string ipAddress, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            
            if (twoFactor != null)
            {
                // Update existing 2FA to email method
                await _twoFactorAuthRepository.DeleteAsync(twoFactor);
            }
            
            // Create new email-based 2FA record
            twoFactor = new TwoFactorAuth(userId, TwoFactorMethod.Email, tenantId);
            twoFactor.Enable(ipAddress);
            await _twoFactorAuthRepository.AddAsync(twoFactor);
            
            return true;
        }

        public async Task<string> GenerateAndSendEmailCodeAsync(string userId, string email, string ipAddress, string purpose, string tenantId)
        {
            // Parse userId to Guid
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            // Check rate limiting (max 3 codes per hour)
            var recentTokensCount = await _emailTokenRepository.CountRecentTokensAsync(userGuid, tenantId, TimeSpan.FromHours(1));
            if (recentTokensCount >= 3)
            {
                throw new InvalidOperationException("Limite de códigos atingido. Tente novamente em 1 hora.");
            }

            // Generate 6-digit code using CSPRNG
            var code = GenerateSecureCode();

            // Create verification token
            var token = new EmailVerificationToken(userGuid, code, purpose, ipAddress, tenantId, expirationMinutes: 5);
            await _emailTokenRepository.AddAsync(token);

            // Send email
            var emailSubject = "Código de Verificação 2FA - PrimeCare";
            var emailBody = GenerateEmailBody(code, purpose);
            await _emailService.SendEmailAsync(email, emailSubject, emailBody);

            // Return temp token (encode userId and tokenId for verification)
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userId}:{token.Id}"));
        }

        public async Task<bool> VerifyEmailCodeAsync(string userId, string code, string tenantId)
        {
            // Parse userId to Guid
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return false;
            }

            var token = await _emailTokenRepository.GetByCodeAndUserIdAsync(code, userGuid, tenantId);
            
            if (token == null || !token.IsValid())
            {
                return false;
            }

            // Check max attempts
            if (token.VerificationAttempts >= 5)
            {
                return false;
            }

            // Verify code
            if (token.Code != code)
            {
                token.IncrementAttempts();
                await _emailTokenRepository.UpdateAsync(token);
                return false;
            }

            // Mark as used
            token.MarkAsUsed();
            await _emailTokenRepository.UpdateAsync(token);

            return true;
        }

        public async Task<TwoFactorMethod> GetTwoFactorMethodAsync(string userId, string tenantId)
        {
            var twoFactor = await _twoFactorAuthRepository.GetByUserIdAsync(userId, tenantId);
            return twoFactor?.Method ?? TwoFactorMethod.None;
        }

        private string GenerateSecureCode()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                var randomNumber = BitConverter.ToUInt32(randomBytes, 0);
                // Generate 6-digit code (100000-999999)
                var code = (randomNumber % 900000) + 100000;
                return code.ToString();
            }
        }

        private string GenerateEmailBody(string code, string purpose)
        {
            var encodedCode = System.Net.WebUtility.HtmlEncode(code);
            var purposeText = purpose == "Login" ? "fazer login" : "continuar";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
        .code {{ font-size: 32px; font-weight: bold; color: #007bff; text-align: center; 
                 padding: 20px; background-color: white; border-radius: 5px; margin: 20px 0; 
                 letter-spacing: 5px; }}
        .warning {{ background-color: #fff3cd; padding: 15px; border-radius: 5px; margin-top: 20px; 
                    border-left: 4px solid #ffc107; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Código de Verificação 2FA</h1>
        </div>
        <div class=""content"">
            <p>Você solicitou autenticação de dois fatores para {purposeText} na sua conta PrimeCare.</p>
            <p>Use o código de verificação abaixo:</p>
            <div class=""code"">{encodedCode}</div>
            <p><strong>Este código é válido por 5 minutos.</strong></p>
            <div class=""warning"">
                <strong>⚠️ Atenção:</strong> Se você não solicitou este código, 
                ignore este e-mail. Sua conta permanece segura.
            </div>
        </div>
        <div class=""footer"">
            <p>© 2026 PrimeCare Software. Todos os direitos reservados.</p>
            <p>Este é um e-mail automático. Por favor, não responda.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
