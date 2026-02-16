using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.Dashboards;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(MedicSoftDbContext context, ILogger<DashboardService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CustomDashboardDto>> GetAllDashboardsAsync()
        {
            _logger.LogInformation("Retrieving all dashboards");

            var dashboards = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return dashboards.Select(MapToDto).ToList();
        }

        public async Task<CustomDashboardDto> GetDashboardAsync(Guid id)
        {
            _logger.LogInformation("Retrieving dashboard with ID: {DashboardId}", id);

            var dashboard = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {id} not found");
            }

            return MapToDto(dashboard);
        }

        public async Task<CustomDashboardDto> CreateDashboardAsync(CreateDashboardDto dto, string userId)
        {
            _logger.LogInformation("Creating new dashboard: {DashboardName} for user: {UserId}", dto.Name, userId);

            var dashboard = new CustomDashboard
            {
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                IsPublic = dto.IsPublic,
                CreatedBy = userId,
                Layout = "{}"
            };

            _context.Set<CustomDashboard>().Add(dashboard);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard created successfully with ID: {DashboardId}", dashboard.Id);

            return MapToDto(dashboard);
        }

        public async Task<CustomDashboardDto> UpdateDashboardAsync(Guid id, UpdateDashboardDto dto)
        {
            _logger.LogInformation("Updating dashboard with ID: {DashboardId}", id);

            var dashboard = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {id} not found");
            }

            dashboard.Name = dto.Name;
            dashboard.Description = dto.Description;
            dashboard.Layout = dto.Layout ?? dashboard.Layout;
            dashboard.IsDefault = dto.IsDefault;
            dashboard.IsPublic = dto.IsPublic;
            dashboard.UpdateTimestamp();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard updated successfully");

            return MapToDto(dashboard);
        }

        public async Task DeleteDashboardAsync(Guid id)
        {
            _logger.LogInformation("Deleting dashboard with ID: {DashboardId}", id);

            var dashboard = await _context.Set<CustomDashboard>()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {id} not found");
            }

            _context.Set<CustomDashboard>().Remove(dashboard);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard deleted successfully");
        }

        public async Task<DashboardWidgetDto> AddWidgetAsync(Guid dashboardId, CreateWidgetDto dto)
        {
            _logger.LogInformation("Adding widget to dashboard: {DashboardId}", dashboardId);

            var dashboard = await _context.Set<CustomDashboard>()
                .FirstOrDefaultAsync(d => d.Id == dashboardId);

            if (dashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {dashboardId} not found");
            }

            if (!string.IsNullOrWhiteSpace(dto.Query) && !ValidateSqlQuery(dto.Query))
            {
                throw new InvalidOperationException("Widget query contains unsafe SQL statements");
            }

            var widget = new DashboardWidget
            {
                DashboardId = dashboardId,
                Type = dto.Type,
                Title = dto.Title,
                Config = dto.Config,
                Query = dto.Query,
                RefreshInterval = dto.RefreshInterval,
                GridX = dto.GridX,
                GridY = dto.GridY,
                GridWidth = dto.GridWidth,
                GridHeight = dto.GridHeight
            };

            _context.Set<DashboardWidget>().Add(widget);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Widget added successfully with ID: {WidgetId}", widget.Id);

            return MapWidgetToDto(widget);
        }

        public async Task UpdateWidgetPositionAsync(Guid widgetId, WidgetPositionDto position)
        {
            _logger.LogInformation("Updating position for widget: {WidgetId}", widgetId);

            var widget = await _context.Set<DashboardWidget>()
                .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
            {
                throw new InvalidOperationException($"Widget with ID {widgetId} not found");
            }

            widget.GridX = position.GridX;
            widget.GridY = position.GridY;
            widget.GridWidth = position.GridWidth;
            widget.GridHeight = position.GridHeight;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Widget position updated successfully");
        }

        public async Task DeleteWidgetAsync(Guid widgetId)
        {
            _logger.LogInformation("Deleting widget: {WidgetId}", widgetId);

            var widget = await _context.Set<DashboardWidget>()
                .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
            {
                throw new InvalidOperationException($"Widget with ID {widgetId} not found");
            }

            _context.Set<DashboardWidget>().Remove(widget);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Widget deleted successfully");
        }

        public async Task<WidgetDataDto> ExecuteWidgetQueryAsync(Guid widgetId)
        {
            _logger.LogInformation("Executing query for widget: {WidgetId}", widgetId);

            var widget = await _context.Set<DashboardWidget>()
                .FirstOrDefaultAsync(w => w.Id == widgetId);

            if (widget == null)
            {
                return new WidgetDataDto
                {
                    WidgetId = widgetId,
                    Error = $"Widget with ID {widgetId} not found"
                };
            }

            if (string.IsNullOrWhiteSpace(widget.Query))
            {
                return new WidgetDataDto
                {
                    WidgetId = widgetId,
                    Error = "Widget has no query defined"
                };
            }

            if (!ValidateSqlQuery(widget.Query))
            {
                _logger.LogWarning("Invalid or potentially unsafe SQL query for widget: {WidgetId}", widgetId);
                return new WidgetDataDto
                {
                    WidgetId = widgetId,
                    Error = "Query contains unsafe SQL statements"
                };
            }

            try
            {
                var data = await ExecuteSafeSelectQuery(widget.Query);
                
                return new WidgetDataDto
                {
                    WidgetId = widgetId,
                    Data = data,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query for widget: {WidgetId}", widgetId);
                return new WidgetDataDto
                {
                    WidgetId = widgetId,
                    Error = "Query execution failed. Please check the query syntax and try again."
                };
            }
        }

        public async Task<byte[]> ExportDashboardAsync(Guid id, DashboardExportFormat format)
        {
            _logger.LogInformation("Exporting dashboard: {DashboardId} as {Format}", id, format);

            var dashboard = await GetDashboardAsync(id);

            switch (format)
            {
                case DashboardExportFormat.Json:
                    return ExportToJson(dashboard);
                
                case DashboardExportFormat.Pdf:
                case DashboardExportFormat.Excel:
                    _logger.LogWarning("Dashboard export to {Format} format is not yet available. Only JSON export is currently supported.", format);
                    throw new InvalidOperationException($"Dashboard export to {format} format is not yet available. Only JSON export is currently supported. Please use JSON format or contact support for additional export options.");
                
                default:
                    throw new ArgumentException($"Unsupported export format: {format}");
            }
        }

        private byte[] ExportToJson(CustomDashboardDto dashboard)
        {
            var exportData = new
            {
                dashboard.Id,
                dashboard.Name,
                dashboard.Description,
                dashboard.Layout,
                dashboard.IsDefault,
                dashboard.IsPublic,
                dashboard.CreatedBy,
                dashboard.CreatedAt,
                dashboard.UpdatedAt,
                Widgets = dashboard.Widgets.Select(w => new
                {
                    w.Id,
                    w.Type,
                    w.Title,
                    w.Config,
                    w.Query,
                    w.RefreshInterval,
                    w.GridX,
                    w.GridY,
                    w.GridWidth,
                    w.GridHeight
                }).ToList()
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return Encoding.UTF8.GetBytes(json);
        }

        public async Task<List<WidgetTemplateDto>> GetWidgetTemplatesAsync()
        {
            _logger.LogInformation("Retrieving all widget templates");

            var templates = await _context.Set<WidgetTemplate>()
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapTemplateToDto).ToList();
        }

        public async Task<List<WidgetTemplateDto>> GetWidgetTemplatesByCategoryAsync(string category)
        {
            _logger.LogInformation("Retrieving widget templates for category: {Category}", category);

            var templates = await _context.Set<WidgetTemplate>()
                .Where(t => t.Category == category)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapTemplateToDto).ToList();
        }

        private bool ValidateSqlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            var normalizedQuery = query.Trim().ToUpperInvariant();

            if (!normalizedQuery.StartsWith("SELECT"))
            {
                _logger.LogWarning("Query does not start with SELECT");
                return false;
            }

            var dangerousKeywords = new[]
            {
                "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER",
                "EXEC", "EXECUTE", "TRUNCATE", "MERGE", "GRANT", "REVOKE",
                "CALL", "PROCEDURE"
            };

            var pattern = string.Join("|", dangerousKeywords.Select(k => $@"\b{Regex.Escape(k)}\b"));
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            if (regex.IsMatch(query))
            {
                _logger.LogWarning("Query contains dangerous keywords");
                return false;
            }

            if (query.Contains(";") && query.IndexOf(";") < query.Length - 1)
            {
                _logger.LogWarning("Query contains multiple statements");
                return false;
            }

            if (query.Contains("--") || query.Contains("/*"))
            {
                _logger.LogWarning("Query contains SQL comments");
                return false;
            }

            return true;
        }

        private async Task<List<Dictionary<string, object>>> ExecuteSafeSelectQuery(string query)
        {
            const int MaxRows = 10000;
            var result = new List<Dictionary<string, object>>();

            var connection = _context.Database.GetDbConnection();
            var shouldCloseConnection = connection.State != ConnectionState.Open;

            try
            {
                if (shouldCloseConnection)
                {
                    await _context.Database.OpenConnectionAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var rowCount = 0;
                        while (await reader.ReadAsync())
                        {
                            if (++rowCount > MaxRows)
                            {
                                _logger.LogWarning("Query returned more than {MaxRows} rows, truncating results", MaxRows);
                                break;
                            }

                            var row = new Dictionary<string, object>();
                            
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                row[columnName] = value ?? (object)DBNull.Value;
                            }
                            
                            result.Add(row);
                        }
                    }
                }
            }
            finally
            {
                if (shouldCloseConnection && connection.State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return result;
        }

        private CustomDashboardDto MapToDto(CustomDashboard dashboard)
        {
            return new CustomDashboardDto
            {
                Id = dashboard.Id,
                Name = dashboard.Name,
                Description = dashboard.Description,
                Layout = dashboard.Layout,
                IsDefault = dashboard.IsDefault,
                IsPublic = dashboard.IsPublic,
                CreatedBy = dashboard.CreatedBy,
                CreatedAt = dashboard.CreatedAt,
                UpdatedAt = dashboard.UpdatedAt,
                Widgets = dashboard.Widgets?.Select(MapWidgetToDto).ToList() ?? new List<DashboardWidgetDto>()
            };
        }

        private DashboardWidgetDto MapWidgetToDto(DashboardWidget widget)
        {
            return new DashboardWidgetDto
            {
                Id = widget.Id,
                DashboardId = widget.DashboardId,
                Type = widget.Type,
                Title = widget.Title,
                Config = widget.Config,
                Query = widget.Query,
                RefreshInterval = widget.RefreshInterval,
                GridX = widget.GridX,
                GridY = widget.GridY,
                GridWidth = widget.GridWidth,
                GridHeight = widget.GridHeight
            };
        }

        private WidgetTemplateDto MapTemplateToDto(WidgetTemplate template)
        {
            return new WidgetTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                Type = template.Type,
                DefaultConfig = template.DefaultConfig,
                DefaultQuery = template.DefaultQuery,
                IsSystem = template.IsSystem,
                Icon = template.Icon
            };
        }

        // ========== Category 4.1: Dashboard Sharing Implementation ==========

        public async Task<DashboardShareDto> ShareDashboardAsync(Guid dashboardId, CreateDashboardShareDto dto, string sharedByUserId)
        {
            _logger.LogInformation("Sharing dashboard {DashboardId} with user {UserId} or role {Role}", 
                dashboardId, dto.SharedWithUserId, dto.SharedWithRole);

            // Validate dashboard exists and user has permission to share it
            var dashboard = await _context.Set<CustomDashboard>()
                .FirstOrDefaultAsync(d => d.Id == dashboardId);

            if (dashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {dashboardId} not found");
            }

            // Security check: Only dashboard owner or public dashboards can be shared
            if (dashboard.CreatedBy != sharedByUserId && !dashboard.IsPublic)
            {
                throw new UnauthorizedAccessException("You do not have permission to share this dashboard");
            }

            // Validate permission level
            if (dto.PermissionLevel != "View" && dto.PermissionLevel != "Edit")
            {
                throw new InvalidOperationException("Permission level must be 'View' or 'Edit'");
            }

            // Validate either user or role is specified, not both
            if (string.IsNullOrWhiteSpace(dto.SharedWithUserId) && string.IsNullOrWhiteSpace(dto.SharedWithRole))
            {
                throw new InvalidOperationException("Must specify either SharedWithUserId or SharedWithRole");
            }

            if (!string.IsNullOrWhiteSpace(dto.SharedWithUserId) && !string.IsNullOrWhiteSpace(dto.SharedWithRole))
            {
                throw new InvalidOperationException("Cannot specify both SharedWithUserId and SharedWithRole");
            }

            // Validate user exists if sharing with specific user
            if (!string.IsNullOrWhiteSpace(dto.SharedWithUserId))
            {
                if (!Guid.TryParse(dto.SharedWithUserId, out var userGuid))
                {
                    throw new InvalidOperationException($"Invalid user ID format: {dto.SharedWithUserId}");
                }

                var userExists = await _context.Set<User>()
                    .AnyAsync(u => u.Id == userGuid);
                
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {dto.SharedWithUserId} not found");
                }
            }

            // Validate role exists if sharing with role
            if (!string.IsNullOrWhiteSpace(dto.SharedWithRole))
            {
                var validRoles = new[] { "SystemAdmin", "ClinicOwner", "Doctor", "Nurse", "Receptionist", "Accountant" };
                if (!validRoles.Contains(dto.SharedWithRole))
                {
                    throw new InvalidOperationException($"Invalid role: {dto.SharedWithRole}");
                }
            }

            var share = new DashboardShare
            {
                DashboardId = dashboardId,
                SharedWithUserId = dto.SharedWithUserId,
                SharedWithRole = dto.SharedWithRole,
                PermissionLevel = dto.PermissionLevel,
                SharedBy = sharedByUserId,
                ExpiresAt = dto.ExpiresAt
            };

            _context.Set<DashboardShare>().Add(share);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard share created with ID: {ShareId}", share.Id);

            return await MapShareToDto(share);
        }

        public async Task<List<DashboardShareDto>> GetDashboardSharesAsync(Guid dashboardId)
        {
            _logger.LogInformation("Retrieving shares for dashboard: {DashboardId}", dashboardId);

            // Load all shares for the dashboard
            var shares = await _context.Set<DashboardShare>()
                .Where(s => s.DashboardId == dashboardId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            // Collect all user IDs and parse them to Guids
            var userGuids = shares
                .Where(s => !string.IsNullOrWhiteSpace(s.SharedWithUserId))
                .Select(s => new { UserId = s.SharedWithUserId, Parsed = Guid.TryParse(s.SharedWithUserId, out var guid) ? (Guid?)guid : null })
                .Where(x => x.Parsed.HasValue)
                .Select(x => x.Parsed!.Value)
                .Distinct()
                .ToList();

            // Batch load all user information in a single query
            var users = await _context.Set<User>()
                .Where(u => userGuids.Contains(u.Id))
                .Select(u => new { u.Id, u.Username })
                .ToDictionaryAsync(u => u.Id.ToString(), u => u.Username);

            // Map shares to DTOs with user information
            var result = shares.Select(share => new DashboardShareDto
            {
                Id = share.Id,
                DashboardId = share.DashboardId,
                SharedWithUserId = share.SharedWithUserId,
                SharedWithUserName = !string.IsNullOrWhiteSpace(share.SharedWithUserId) && users.ContainsKey(share.SharedWithUserId)
                    ? users[share.SharedWithUserId]
                    : null,
                SharedWithRole = share.SharedWithRole,
                PermissionLevel = share.PermissionLevel,
                SharedBy = share.SharedBy,
                ExpiresAt = share.ExpiresAt,
                CreatedAt = share.CreatedAt
            }).ToList();

            return result;
        }

        public async Task RevokeDashboardShareAsync(Guid shareId)
        {
            _logger.LogInformation("Revoking dashboard share: {ShareId}", shareId);

            var share = await _context.Set<DashboardShare>()
                .Include(s => s.Dashboard)
                .FirstOrDefaultAsync(s => s.Id == shareId);

            if (share == null)
            {
                throw new InvalidOperationException($"Dashboard share with ID {shareId} not found");
            }

            _context.Set<DashboardShare>().Remove(share);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard share revoked successfully");
        }

        public async Task<List<CustomDashboardDto>> GetSharedDashboardsAsync(string userId, string userRole)
        {
            _logger.LogInformation("Retrieving shared dashboards for user: {UserId} with role: {Role}", userId, userRole);

            var currentDate = DateTime.UtcNow;

            // Get dashboards shared with this user or their role (not expired)
            var sharedDashboardIds = await _context.Set<DashboardShare>()
                .Where(s => (s.SharedWithUserId == userId || s.SharedWithRole == userRole) &&
                           (s.ExpiresAt == null || s.ExpiresAt > currentDate))
                .Select(s => s.DashboardId)
                .Distinct()
                .ToListAsync();

            if (!sharedDashboardIds.Any())
            {
                return new List<CustomDashboardDto>();
            }

            var dashboards = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .Where(d => sharedDashboardIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToListAsync();

            return dashboards.Select(MapToDto).ToList();
        }

        public async Task<CustomDashboardDto> DuplicateDashboardAsync(Guid dashboardId, string userId, string newName)
        {
            _logger.LogInformation("Duplicating dashboard {DashboardId} for user {UserId}", dashboardId, userId);

            var originalDashboard = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == dashboardId);

            if (originalDashboard == null)
            {
                throw new InvalidOperationException($"Dashboard with ID {dashboardId} not found");
            }

            // Security check: User can duplicate if they own it, it's public, or it's shared with them
            var canDuplicate = originalDashboard.CreatedBy == userId || 
                              originalDashboard.IsPublic ||
                              await _context.Set<DashboardShare>()
                                  .AnyAsync(s => s.DashboardId == dashboardId && 
                                                (s.SharedWithUserId == userId) &&
                                                (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow));

            if (!canDuplicate)
            {
                throw new UnauthorizedAccessException("You do not have permission to duplicate this dashboard");
            }

            var newDashboard = new CustomDashboard
            {
                Name = newName ?? $"{originalDashboard.Name} (Copy)",
                Description = originalDashboard.Description,
                Layout = originalDashboard.Layout,
                IsDefault = false, // Copies are never default
                IsPublic = false, // Copies are private by default
                CreatedBy = userId
            };

            _context.Set<CustomDashboard>().Add(newDashboard);
            await _context.SaveChangesAsync();

            // Copy widgets
            foreach (var originalWidget in originalDashboard.Widgets)
            {
                var newWidget = new DashboardWidget
                {
                    DashboardId = newDashboard.Id,
                    Type = originalWidget.Type,
                    Title = originalWidget.Title,
                    Config = originalWidget.Config,
                    Query = originalWidget.Query,
                    RefreshInterval = originalWidget.RefreshInterval,
                    GridX = originalWidget.GridX,
                    GridY = originalWidget.GridY,
                    GridWidth = originalWidget.GridWidth,
                    GridHeight = originalWidget.GridHeight
                };

                _context.Set<DashboardWidget>().Add(newWidget);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard duplicated successfully with ID: {DashboardId}", newDashboard.Id);

            // Reload with widgets
            var result = await _context.Set<CustomDashboard>()
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == newDashboard.Id);

            return MapToDto(result);
        }

        private async Task<DashboardShareDto> MapShareToDto(DashboardShare share)
        {
            var dto = new DashboardShareDto
            {
                Id = share.Id,
                DashboardId = share.DashboardId,
                SharedWithUserId = share.SharedWithUserId,
                SharedWithRole = share.SharedWithRole,
                PermissionLevel = share.PermissionLevel,
                SharedBy = share.SharedBy,
                ExpiresAt = share.ExpiresAt,
                CreatedAt = share.CreatedAt
            };

            // Try to get username if shared with a user
            if (!string.IsNullOrWhiteSpace(share.SharedWithUserId) && Guid.TryParse(share.SharedWithUserId, out var userGuid))
            {
                var user = await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Id == userGuid);
                
                if (user != null)
                {
                    dto.SharedWithUserName = user.Username;
                }
            }

            return dto;
        }
    }
}
