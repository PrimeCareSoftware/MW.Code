using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Services;

/// <summary>
/// Service for seeding demo data in the Patient Portal
/// </summary>
public class PatientPortalSeederService
{
    private readonly PatientPortalDbContext _context;
    private const string DemoClinicId = "demo-clinic-001";

    public PatientPortalSeederService(PatientPortalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds demo patient portal users
    /// </summary>
    public async Task SeedDemoDataAsync()
    {
        // Check if data already exists
        var existingUsers = await _context.PatientUsers.AnyAsync();
        if (existingUsers)
        {
            throw new InvalidOperationException("Demo data already exists. Clear the database first using DELETE /api/data-seeder/clear-database");
        }

        // Fetch patients from main database
        var patients = await FetchPatientsFromMainDatabaseAsync();
        
        if (!patients.Any())
        {
            throw new InvalidOperationException("No patients found in main database. Please seed the main application first.");
        }

        // Create PatientUsers for each patient
        var patientUsers = new List<PatientUser>();
        
        foreach (var patient in patients)
        {
            var patientUser = new PatientUser
            {
                Id = Guid.NewGuid(),
                ClinicId = patient.ClinicId,
                PatientId = patient.PatientId,
                Email = patient.Email,
                PasswordHash = HashPassword("Patient@123"), // Demo password
                CPF = patient.CPF,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                IsActive = true,
                EmailConfirmed = true,
                PhoneConfirmed = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            patientUsers.Add(patientUser);
        }

        await _context.PatientUsers.AddRangeAsync(patientUsers);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets all patient users from the database
    /// </summary>
    public async Task<List<PatientUser>> GetPatientUsersAsync()
    {
        return await _context.PatientUsers
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    /// <summary>
    /// Clears all patient portal data
    /// </summary>
    public async Task ClearDatabaseAsync()
    {
        // Delete in order to respect foreign key constraints
        _context.TwoFactorTokens.RemoveRange(_context.TwoFactorTokens);
        _context.PasswordResetTokens.RemoveRange(_context.PasswordResetTokens);
        _context.EmailVerificationTokens.RemoveRange(_context.EmailVerificationTokens);
        _context.RefreshTokens.RemoveRange(_context.RefreshTokens);
        _context.PatientUsers.RemoveRange(_context.PatientUsers);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Fetches patient data from the main database
    /// </summary>
    private async Task<List<PatientData>> FetchPatientsFromMainDatabaseAsync()
    {
        // Query the main database Patients table using raw SQL
        // Join with PatientClinicLinks to get the ClinicId since Patients don't have direct ClinicId
        var sql = @"
            SELECT 
                p.""Id"" as ""PatientId"",
                pcl.""ClinicId"",
                p.""Document"" as ""CPF"",
                p.""Name"" as ""FullName"",
                p.""Email"",
                p.""PhoneNumber"",
                p.""DateOfBirth""
            FROM ""Patients"" p
            INNER JOIN ""PatientClinicLinks"" pcl ON p.""Id"" = pcl.""PatientId""
            WHERE pcl.""ClinicId""::text = {0}
            AND pcl.""IsActive"" = true
            AND p.""Email"" IS NOT NULL 
            AND p.""Email"" != ''
            AND p.""Document"" IS NOT NULL 
            AND p.""Document"" != ''
            ORDER BY p.""CreatedAt"" DESC
            LIMIT 10";

        var patients = await _context.Database
            .SqlQueryRaw<PatientData>(sql, DemoClinicId)
            .ToListAsync();

        return patients;
    }

    /// <summary>
    /// Hashes a password using PBKDF2
    /// </summary>
    private string HashPassword(string password)
    {
        // Generate a 128-bit salt
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

        // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Combine salt and hash
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }
}

/// <summary>
/// DTO for patient data from main database
/// </summary>
public class PatientData
{
    public Guid PatientId { get; set; }
    public Guid ClinicId { get; set; }
    public string CPF { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
