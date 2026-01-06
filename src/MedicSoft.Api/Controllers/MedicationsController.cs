using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationsController : BaseController
    {
        private readonly IMedicationRepository _medicationRepository;

        public MedicationsController(IMedicationRepository medicationRepository, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _medicationRepository = medicationRepository;
        }

        /// <summary>
        /// Get all active medications for autocomplete
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll([FromQuery] bool activeOnly = true)
        {
            var tenantId = GetTenantId();
            var medications = activeOnly 
                ? await _medicationRepository.GetActiveAsync(tenantId)
                : await _medicationRepository.GetAllAsync(tenantId);
            
            var dtos = medications.Select(m => MapToDto(m));
            return Ok(dtos);
        }

        /// <summary>
        /// Search medications by name for autocomplete
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MedicationAutocompleteDto>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(Array.Empty<MedicationAutocompleteDto>());

            var tenantId = GetTenantId();
            var medications = await _medicationRepository.SearchByNameAsync(term, tenantId);
            
            var dtos = medications.Select(m => new MedicationAutocompleteDto
            {
                Id = m.Id,
                Name = m.Name,
                GenericName = m.GenericName,
                Dosage = m.Dosage,
                PharmaceuticalForm = m.PharmaceuticalForm,
                AdministrationRoute = m.AdministrationRoute
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Get medication by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicationDto>> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var medication = await _medicationRepository.GetByIdAsync(id, tenantId);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found");

            return Ok(MapToDto(medication));
        }

        /// <summary>
        /// Get medications by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MedicationDto>>> GetByCategory(MedicationCategory category)
        {
            var tenantId = GetTenantId();
            var medications = await _medicationRepository.GetByCategoryAsync(category, tenantId);
            
            var dtos = medications.Select(m => MapToDto(m));
            return Ok(dtos);
        }

        /// <summary>
        /// Create a new medication
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MedicationDto>> Create([FromBody] CreateMedicationDto dto)
        {
            var tenantId = GetTenantId();
            
            // Check if name is unique
            if (!await _medicationRepository.IsNameUniqueAsync(dto.Name, tenantId))
                return BadRequest($"A medication with the name '{dto.Name}' already exists");

            var medication = new Medication(
                dto.Name,
                dto.Dosage,
                dto.PharmaceuticalForm,
                dto.Category,
                dto.RequiresPrescription,
                tenantId,
                dto.GenericName,
                dto.Manufacturer,
                dto.ActiveIngredient,
                dto.Concentration,
                dto.AdministrationRoute,
                dto.IsControlled,
                dto.ControlledList,
                dto.AnvisaRegistration,
                dto.Barcode,
                dto.Description
            );

            await _medicationRepository.AddAsync(medication);

            return CreatedAtAction(nameof(GetById), new { id = medication.Id }, MapToDto(medication));
        }

        /// <summary>
        /// Update an existing medication
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MedicationDto>> Update(Guid id, [FromBody] UpdateMedicationDto dto)
        {
            var tenantId = GetTenantId();
            var medication = await _medicationRepository.GetByIdAsync(id, tenantId);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found");

            // Check if new name is unique (if changed)
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != medication.Name)
            {
                if (!await _medicationRepository.IsNameUniqueAsync(dto.Name, tenantId, id))
                    return BadRequest($"A medication with the name '{dto.Name}' already exists");
            }

            medication.Update(
                dto.Name ?? medication.Name,
                dto.Dosage ?? medication.Dosage,
                dto.PharmaceuticalForm ?? medication.PharmaceuticalForm,
                dto.Category ?? medication.Category,
                dto.RequiresPrescription ?? medication.RequiresPrescription,
                dto.GenericName ?? medication.GenericName,
                dto.Manufacturer ?? medication.Manufacturer,
                dto.ActiveIngredient ?? medication.ActiveIngredient,
                dto.Concentration ?? medication.Concentration,
                dto.AdministrationRoute ?? medication.AdministrationRoute,
                dto.IsControlled ?? medication.IsControlled,
                dto.ControlledList ?? medication.ControlledList,
                dto.AnvisaRegistration ?? medication.AnvisaRegistration,
                dto.Barcode ?? medication.Barcode,
                dto.Description ?? medication.Description
            );

            await _medicationRepository.UpdateAsync(medication);

            return Ok(MapToDto(medication));
        }

        /// <summary>
        /// Delete (deactivate) a medication
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var medication = await _medicationRepository.GetByIdAsync(id, tenantId);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found");

            medication.Deactivate();
            await _medicationRepository.UpdateAsync(medication);

            return NoContent();
        }

        private static MedicationDto MapToDto(Medication medication)
        {
            return new MedicationDto
            {
                Id = medication.Id,
                Name = medication.Name,
                GenericName = medication.GenericName,
                Manufacturer = medication.Manufacturer,
                ActiveIngredient = medication.ActiveIngredient,
                Dosage = medication.Dosage,
                PharmaceuticalForm = medication.PharmaceuticalForm,
                Concentration = medication.Concentration,
                AdministrationRoute = medication.AdministrationRoute,
                Category = medication.Category,
                RequiresPrescription = medication.RequiresPrescription,
                IsControlled = medication.IsControlled,
                AnvisaRegistration = medication.AnvisaRegistration,
                Barcode = medication.Barcode,
                Description = medication.Description,
                IsActive = medication.IsActive,
                CreatedAt = medication.CreatedAt,
                UpdatedAt = medication.UpdatedAt
            };
        }
    }
}
