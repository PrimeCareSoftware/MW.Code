using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing suppliers
    /// </summary>
    [ApiController]
    [Route("api/suppliers")]
    [Authorize]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierRepository _repository;

        public SuppliersController(
            ISupplierRepository repository,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all suppliers
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.SuppliersView)]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            var suppliers = await _repository.GetAllAsync(GetTenantId());
            var dtos = suppliers.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get active suppliers only
        /// </summary>
        [HttpGet("active")]
        [RequirePermissionKey(PermissionKeys.SuppliersView)]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetActive()
        {
            var suppliers = await _repository.GetActiveAsync(GetTenantId());
            var dtos = suppliers.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get supplier by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.SuppliersView)]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id, GetTenantId());
            if (supplier == null)
                return NotFound(new { message = "Fornecedor não encontrado." });

            return Ok(MapToDto(supplier));
        }

        /// <summary>
        /// Search suppliers by name
        /// </summary>
        [HttpGet("search")]
        [RequirePermissionKey(PermissionKeys.SuppliersView)]
        [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Nome é obrigatório para a busca." });

            var allSuppliers = await _repository.GetAllAsync(GetTenantId());
            var suppliers = allSuppliers.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            var dtos = suppliers.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Create a new supplier
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.SuppliersManage)]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var supplier = new Supplier(
                    dto.Name,
                    GetTenantId(),
                    dto.TradeName,
                    dto.DocumentNumber,
                    dto.Email,
                    dto.Phone
                );

                if (!string.IsNullOrEmpty(dto.Address) || !string.IsNullOrEmpty(dto.City) || 
                    !string.IsNullOrEmpty(dto.State) || !string.IsNullOrEmpty(dto.ZipCode))
                {
                    supplier.UpdateAddress(dto.Address, dto.City, dto.State, dto.ZipCode);
                }

                if (!string.IsNullOrEmpty(dto.BankName) || !string.IsNullOrEmpty(dto.BankAccount) || 
                    !string.IsNullOrEmpty(dto.PixKey))
                {
                    supplier.SetBankingInfo(dto.BankName, dto.BankAccount, dto.PixKey);
                }

                if (!string.IsNullOrEmpty(dto.Notes))
                {
                    supplier.UpdateNotes(dto.Notes);
                }

                var created = await _repository.AddAsync(supplier);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update supplier
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.SuppliersManage)]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplier = await _repository.GetByIdAsync(id, GetTenantId());
            if (supplier == null)
                return NotFound(new { message = "Fornecedor não encontrado." });

            try
            {
                supplier.UpdateInfo(dto.Name, dto.TradeName, dto.DocumentNumber, dto.Email, dto.Phone);
                supplier.UpdateAddress(dto.Address, dto.City, dto.State, dto.ZipCode);
                supplier.SetBankingInfo(dto.BankName, dto.BankAccount, dto.PixKey);

                if (!string.IsNullOrEmpty(dto.Notes))
                    supplier.UpdateNotes(dto.Notes);

                await _repository.UpdateAsync(supplier);
                return Ok(MapToDto(supplier));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate supplier
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.SuppliersManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id, GetTenantId());
            if (supplier == null)
                return NotFound(new { message = "Fornecedor não encontrado." });

            supplier.Activate();
            await _repository.UpdateAsync(supplier);
            return NoContent();
        }

        /// <summary>
        /// Deactivate supplier
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.SuppliersManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id, GetTenantId());
            if (supplier == null)
                return NotFound(new { message = "Fornecedor não encontrado." });

            supplier.Deactivate();
            await _repository.UpdateAsync(supplier);
            return NoContent();
        }

        /// <summary>
        /// Delete supplier
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.SuppliersManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id, GetTenantId());
            if (supplier == null)
                return NotFound(new { message = "Fornecedor não encontrado." });

            await _repository.DeleteAsync(id, GetTenantId());
            return NoContent();
        }

        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                TradeName = supplier.TradeName,
                DocumentNumber = supplier.DocumentNumber,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                City = supplier.City,
                State = supplier.State,
                ZipCode = supplier.ZipCode,
                BankName = supplier.BankName,
                BankAccount = supplier.BankAccount,
                PixKey = supplier.PixKey,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }
    }
}
