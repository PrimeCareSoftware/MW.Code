using Microsoft.EntityFrameworkCore;
using MedicSoft.Patients.Api.Data;
using MedicSoft.Patients.Api.Models;

namespace MedicSoft.Patients.Api.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string tenantId);
    Task<PatientDto?> GetPatientByIdAsync(Guid id, string tenantId);
    Task<PatientDto?> GetPatientByDocumentGlobalAsync(string document);
    Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm, string tenantId);
    Task<PatientDto> CreatePatientAsync(CreatePatientDto dto, string tenantId);
    Task<PatientDto?> UpdatePatientAsync(Guid id, UpdatePatientDto dto, string tenantId);
    Task<bool> DeletePatientAsync(Guid id, string tenantId);
    Task<bool> LinkPatientToClinicAsync(Guid patientId, Guid clinicId, string tenantId);
    Task<bool> LinkChildToGuardianAsync(Guid childId, Guid guardianId, string tenantId);
    Task<IEnumerable<PatientDto>> GetChildrenOfGuardianAsync(Guid guardianId, string tenantId);
}

public class PatientService : IPatientService
{
    private readonly PatientsDbContext _context;
    private readonly ILogger<PatientService> _logger;

    public PatientService(PatientsDbContext context, ILogger<PatientService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string tenantId)
    {
        var patients = await _context.Patients
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.FullName)
            .ToListAsync();

        return patients.Select(MapToDto);
    }

    public async Task<PatientDto?> GetPatientByIdAsync(Guid id, string tenantId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        return patient != null ? MapToDto(patient) : null;
    }

    public async Task<PatientDto?> GetPatientByDocumentGlobalAsync(string document)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Document == document && p.IsActive);

        return patient != null ? MapToDto(patient) : null;
    }

    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm, string tenantId)
    {
        var normalizedSearch = searchTerm.ToLower();

        var patients = await _context.Patients
            .Where(p => p.TenantId == tenantId && p.IsActive &&
                (p.FullName.ToLower().Contains(normalizedSearch) ||
                 p.Document.Contains(searchTerm) ||
                 p.Phone.Contains(searchTerm)))
            .OrderBy(p => p.FullName)
            .Take(50)
            .ToListAsync();

        return patients.Select(MapToDto);
    }

    public async Task<PatientDto> CreatePatientAsync(CreatePatientDto dto, string tenantId)
    {
        // Check if patient with same document already exists
        if (!string.IsNullOrEmpty(dto.Document))
        {
            var existing = await _context.Patients
                .FirstOrDefaultAsync(p => p.Document == dto.Document && p.TenantId == tenantId);
            if (existing != null)
            {
                throw new InvalidOperationException("A patient with this document already exists");
            }
        }

        var patient = new PatientEntity
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Document = dto.Document,
            Email = dto.Email ?? string.Empty,
            Phone = dto.Phone ?? string.Empty,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Address = dto.Address ?? string.Empty,
            City = dto.City ?? string.Empty,
            State = dto.State ?? string.Empty,
            ZipCode = dto.ZipCode ?? string.Empty,
            Notes = dto.Notes,
            GuardianId = dto.GuardianId,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created patient: {PatientId} in tenant: {TenantId}", patient.Id, tenantId);
        return MapToDto(patient);
    }

    public async Task<PatientDto?> UpdatePatientAsync(Guid id, UpdatePatientDto dto, string tenantId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (patient == null)
            return null;

        if (!string.IsNullOrEmpty(dto.FullName)) patient.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Document)) patient.Document = dto.Document;
        if (dto.Email != null) patient.Email = dto.Email;
        if (dto.Phone != null) patient.Phone = dto.Phone;
        if (dto.DateOfBirth.HasValue) patient.DateOfBirth = dto.DateOfBirth;
        if (dto.Gender.HasValue) patient.Gender = dto.Gender.Value;
        if (dto.Address != null) patient.Address = dto.Address;
        if (dto.City != null) patient.City = dto.City;
        if (dto.State != null) patient.State = dto.State;
        if (dto.ZipCode != null) patient.ZipCode = dto.ZipCode;
        if (dto.Notes != null) patient.Notes = dto.Notes;
        if (dto.GuardianId.HasValue) patient.GuardianId = dto.GuardianId;

        patient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated patient: {PatientId}", id);
        return MapToDto(patient);
    }

    public async Task<bool> DeletePatientAsync(Guid id, string tenantId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (patient == null)
            return false;

        patient.IsActive = false;
        patient.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted (deactivated) patient: {PatientId}", id);
        return true;
    }

    public async Task<bool> LinkPatientToClinicAsync(Guid patientId, Guid clinicId, string tenantId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId && p.TenantId == tenantId);

        if (patient == null)
            return false;

        var existingLink = await _context.PatientClinicLinks
            .FirstOrDefaultAsync(l => l.PatientId == patientId && l.ClinicId == clinicId);

        if (existingLink != null)
            return true;

        var link = new PatientClinicLinkEntity
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            ClinicId = clinicId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.PatientClinicLinks.Add(link);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Linked patient {PatientId} to clinic {ClinicId}", patientId, clinicId);
        return true;
    }

    public async Task<bool> LinkChildToGuardianAsync(Guid childId, Guid guardianId, string tenantId)
    {
        var child = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == childId && p.TenantId == tenantId);

        if (child == null)
            return false;

        var guardian = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == guardianId && p.TenantId == tenantId);

        if (guardian == null)
            return false;

        child.GuardianId = guardianId;
        child.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Linked child {ChildId} to guardian {GuardianId}", childId, guardianId);
        return true;
    }

    public async Task<IEnumerable<PatientDto>> GetChildrenOfGuardianAsync(Guid guardianId, string tenantId)
    {
        var children = await _context.Patients
            .Where(p => p.GuardianId == guardianId && p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.FullName)
            .ToListAsync();

        return children.Select(MapToDto);
    }

    private static PatientDto MapToDto(PatientEntity entity)
    {
        return new PatientDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Document = entity.Document,
            Email = entity.Email,
            Phone = entity.Phone,
            DateOfBirth = entity.DateOfBirth,
            Gender = GetGenderName(entity.Gender),
            Address = entity.Address,
            City = entity.City,
            State = entity.State,
            ZipCode = entity.ZipCode,
            Notes = entity.Notes,
            GuardianId = entity.GuardianId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static string GetGenderName(int gender)
    {
        return gender switch
        {
            0 => "Male",
            1 => "Female",
            2 => "Other",
            _ => "NotSpecified"
        };
    }
}
