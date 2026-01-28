using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllTags();
        Task<TagDto?> GetTagById(Guid tagId);
        Task<TagDto> CreateTag(CreateTagDto createDto, string tenantId);
        Task<bool> UpdateTag(Guid tagId, UpdateTagDto updateDto);
        Task<bool> DeleteTag(Guid tagId);
        Task<bool> AssignTagsToClinics(Guid tagId, List<Guid> clinicIds, string? assignedBy = null);
        Task<bool> RemoveTagFromClinics(Guid tagId, List<Guid> clinicIds);
        Task<List<TagDto>> GetTagsByClinic(Guid clinicId);
        Task ApplyAutomaticTags();
    }

    public class TagService : ITagService
    {
        private readonly MedicSoftDbContext _context;

        public TagService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagDto>> GetAllTags()
        {
            return await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Order)
                .ThenBy(t => t.Name)
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Color = t.Color,
                    IsAutomatic = t.IsAutomatic,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TagDto?> GetTagById(Guid tagId)
        {
            return await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .Where(t => t.Id == tagId)
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Color = t.Color,
                    IsAutomatic = t.IsAutomatic,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TagDto> CreateTag(CreateTagDto createDto, string tenantId)
        {
            var tag = new Tag(
                name: createDto.Name,
                category: createDto.Category,
                color: createDto.Color,
                tenantId: tenantId,
                description: createDto.Description,
                isAutomatic: createDto.IsAutomatic,
                automationRules: createDto.AutomationRules,
                order: createDto.Order
            );

            _context.Set<Tag>().Add(tag);
            await _context.SaveChangesAsync();

            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                Category = tag.Category,
                Color = tag.Color,
                IsAutomatic = tag.IsAutomatic,
                Order = tag.Order,
                CreatedAt = tag.CreatedAt
            };
        }

        public async Task<bool> UpdateTag(Guid tagId, UpdateTagDto updateDto)
        {
            var tag = await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                return false;

            tag.Update(
                name: updateDto.Name,
                category: updateDto.Category,
                color: updateDto.Color,
                description: updateDto.Description,
                order: updateDto.Order
            );

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTag(Guid tagId)
        {
            var tag = await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                return false;

            // Remove all clinic-tag associations
            var clinicTags = await _context.Set<ClinicTag>()
                .IgnoreQueryFilters()
                .Where(ct => ct.TagId == tagId)
                .ToListAsync();

            _context.Set<ClinicTag>().RemoveRange(clinicTags);
            _context.Set<Tag>().Remove(tag);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTagsToClinics(Guid tagId, List<Guid> clinicIds, string? assignedBy = null)
        {
            var tag = await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                return false;

            foreach (var clinicId in clinicIds)
            {
                // Check if already assigned
                var existing = await _context.Set<ClinicTag>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(ct => ct.ClinicId == clinicId && ct.TagId == tagId);

                if (existing == null)
                {
                    var clinicTag = new ClinicTag(
                        clinicId: clinicId,
                        tagId: tagId,
                        tenantId: tag.TenantId,
                        assignedBy: assignedBy,
                        isAutoAssigned: false
                    );

                    _context.Set<ClinicTag>().Add(clinicTag);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTagFromClinics(Guid tagId, List<Guid> clinicIds)
        {
            var clinicTags = await _context.Set<ClinicTag>()
                .IgnoreQueryFilters()
                .Where(ct => ct.TagId == tagId && clinicIds.Contains(ct.ClinicId))
                .ToListAsync();

            _context.Set<ClinicTag>().RemoveRange(clinicTags);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TagDto>> GetTagsByClinic(Guid clinicId)
        {
            return await _context.Set<ClinicTag>()
                .IgnoreQueryFilters()
                .Where(ct => ct.ClinicId == clinicId)
                .Select(ct => new TagDto
                {
                    Id = ct.Tag!.Id,
                    Name = ct.Tag.Name,
                    Description = ct.Tag.Description,
                    Category = ct.Tag.Category,
                    Color = ct.Tag.Color,
                    IsAutomatic = ct.Tag.IsAutomatic,
                    Order = ct.Tag.Order,
                    CreatedAt = ct.Tag.CreatedAt
                })
                .ToListAsync();
        }

        public async Task ApplyAutomaticTags()
        {
            var automaticTags = await _context.Set<Tag>()
                .IgnoreQueryFilters()
                .Where(t => t.IsAutomatic)
                .ToListAsync();

            foreach (var tag in automaticTags)
            {
                // Parse automation rules and apply based on tag name
                // Use exact string comparison for tag names
                var tagNameLower = tag.Name.ToLower().Trim();
                
                if (tagNameLower == "at risk" || tagNameLower == "atrisk")
                {
                    await ApplyAtRiskTag(tag);
                }
                else if (tagNameLower == "high value" || tagNameLower == "highvalue")
                {
                    await ApplyHighValueTag(tag);
                }
                else if (tagNameLower == "new")
                {
                    await ApplyNewTag(tag);
                }
                // Add more tag types as needed based on exact matches
            }
        }

        private async Task ApplyAtRiskTag(Tag tag)
        {
            // Find clinics with no activity in last 30 days
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            
            var inactiveClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.TenantId,
                    LastActivity = _context.Set<UserSession>()
                        .IgnoreQueryFilters()
                        .Where(s => s.TenantId == c.Id.ToString())
                        .OrderByDescending(s => s.CreatedAt)
                        .Select(s => (DateTime?)s.CreatedAt)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var atRiskClinicIds = inactiveClinics
                .Where(c => !c.LastActivity.HasValue || c.LastActivity.Value < thirtyDaysAgo)
                .Select(c => c.Id)
                .ToList();

            foreach (var clinicId in atRiskClinicIds)
            {
                // Check if already tagged
                var existing = await _context.Set<ClinicTag>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(ct => ct.ClinicId == clinicId && ct.TagId == tag.Id);

                if (existing == null)
                {
                    var clinicTag = new ClinicTag(
                        clinicId: clinicId,
                        tagId: tag.Id,
                        tenantId: tag.TenantId,
                        assignedBy: "System",
                        isAutoAssigned: true
                    );

                    _context.Set<ClinicTag>().Add(clinicTag);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ApplyHighValueTag(Tag tag)
        {
            // Find clinics with MRR > R$ 1000
            var highValueClinics = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.CurrentPrice >= 1000)
                .Select(s => s.ClinicId)
                .Distinct()
                .ToListAsync();

            foreach (var clinicId in highValueClinics)
            {
                var existing = await _context.Set<ClinicTag>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(ct => ct.ClinicId == clinicId && ct.TagId == tag.Id);

                if (existing == null)
                {
                    var clinicTag = new ClinicTag(
                        clinicId: clinicId,
                        tagId: tag.Id,
                        tenantId: tag.TenantId,
                        assignedBy: "System",
                        isAutoAssigned: true
                    );

                    _context.Set<ClinicTag>().Add(clinicTag);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ApplyNewTag(Tag tag)
        {
            // Find clinics created in last 30 days
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            
            var newClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.CreatedAt >= thirtyDaysAgo)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var clinicId in newClinics)
            {
                var existing = await _context.Set<ClinicTag>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(ct => ct.ClinicId == clinicId && ct.TagId == tag.Id);

                if (existing == null)
                {
                    var clinicTag = new ClinicTag(
                        clinicId: clinicId,
                        tagId: tag.Id,
                        tenantId: tag.TenantId,
                        assignedBy: "System",
                        isAutoAssigned: true
                    );

                    _context.Set<ClinicTag>().Add(clinicTag);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
