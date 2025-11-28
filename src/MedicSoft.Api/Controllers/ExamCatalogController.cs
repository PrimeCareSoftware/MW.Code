using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamCatalogController : BaseController
    {
        private readonly IExamCatalogRepository _examCatalogRepository;

        public ExamCatalogController(IExamCatalogRepository examCatalogRepository, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _examCatalogRepository = examCatalogRepository;
        }

        /// <summary>
        /// Get all active exams from catalog for autocomplete
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamCatalogDto>>> GetAll([FromQuery] bool activeOnly = true)
        {
            var tenantId = GetTenantId();
            var exams = activeOnly 
                ? await _examCatalogRepository.GetActiveAsync(tenantId)
                : await _examCatalogRepository.GetAllAsync(tenantId);
            
            var dtos = exams.Select(e => MapToDto(e));
            return Ok(dtos);
        }

        /// <summary>
        /// Search exams by name for autocomplete
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ExamAutocompleteDto>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(Array.Empty<ExamAutocompleteDto>());

            var tenantId = GetTenantId();
            var exams = await _examCatalogRepository.SearchByNameAsync(term, tenantId);
            
            var dtos = exams.Select(e => new ExamAutocompleteDto
            {
                Id = e.Id,
                Name = e.Name,
                ExamType = e.ExamType,
                Category = e.Category,
                Preparation = e.Preparation
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Get exam by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamCatalogDto>> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var exam = await _examCatalogRepository.GetByIdAsync(id, tenantId);

            if (exam == null)
                return NotFound($"Exam with ID {id} not found");

            return Ok(MapToDto(exam));
        }

        /// <summary>
        /// Get exams by type
        /// </summary>
        [HttpGet("type/{examType}")]
        public async Task<ActionResult<IEnumerable<ExamCatalogDto>>> GetByExamType(ExamType examType)
        {
            var tenantId = GetTenantId();
            var exams = await _examCatalogRepository.GetByExamTypeAsync(examType, tenantId);
            
            var dtos = exams.Select(e => MapToDto(e));
            return Ok(dtos);
        }

        /// <summary>
        /// Get exams by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ExamCatalogDto>>> GetByCategory(string category)
        {
            var tenantId = GetTenantId();
            var exams = await _examCatalogRepository.GetByCategoryAsync(category, tenantId);
            
            var dtos = exams.Select(e => MapToDto(e));
            return Ok(dtos);
        }

        /// <summary>
        /// Create a new exam in catalog
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExamCatalogDto>> Create([FromBody] CreateExamCatalogDto dto)
        {
            var tenantId = GetTenantId();
            
            // Check if name is unique
            if (!await _examCatalogRepository.IsNameUniqueAsync(dto.Name, tenantId))
                return BadRequest($"An exam with the name '{dto.Name}' already exists");

            var exam = new ExamCatalog(
                dto.Name,
                dto.ExamType,
                tenantId,
                dto.Description,
                dto.Category,
                dto.Preparation,
                dto.Synonyms,
                dto.TussCode
            );

            await _examCatalogRepository.AddAsync(exam);

            return CreatedAtAction(nameof(GetById), new { id = exam.Id }, MapToDto(exam));
        }

        /// <summary>
        /// Update an existing exam in catalog
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamCatalogDto>> Update(Guid id, [FromBody] UpdateExamCatalogDto dto)
        {
            var tenantId = GetTenantId();
            var exam = await _examCatalogRepository.GetByIdAsync(id, tenantId);

            if (exam == null)
                return NotFound($"Exam with ID {id} not found");

            // Check if new name is unique (if changed)
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != exam.Name)
            {
                if (!await _examCatalogRepository.IsNameUniqueAsync(dto.Name, tenantId, id))
                    return BadRequest($"An exam with the name '{dto.Name}' already exists");
            }

            exam.Update(
                dto.Name ?? exam.Name,
                dto.ExamType ?? exam.ExamType,
                dto.Description ?? exam.Description,
                dto.Category ?? exam.Category,
                dto.Preparation ?? exam.Preparation,
                dto.Synonyms ?? exam.Synonyms,
                dto.TussCode ?? exam.TussCode
            );

            await _examCatalogRepository.UpdateAsync(exam);

            return Ok(MapToDto(exam));
        }

        /// <summary>
        /// Delete (deactivate) an exam from catalog
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var exam = await _examCatalogRepository.GetByIdAsync(id, tenantId);

            if (exam == null)
                return NotFound($"Exam with ID {id} not found");

            exam.Deactivate();
            await _examCatalogRepository.UpdateAsync(exam);

            return NoContent();
        }

        private static ExamCatalogDto MapToDto(ExamCatalog exam)
        {
            return new ExamCatalogDto
            {
                Id = exam.Id,
                Name = exam.Name,
                Description = exam.Description,
                ExamType = exam.ExamType,
                Category = exam.Category,
                Preparation = exam.Preparation,
                Synonyms = exam.Synonyms,
                TussCode = exam.TussCode,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt
            };
        }
    }
}
