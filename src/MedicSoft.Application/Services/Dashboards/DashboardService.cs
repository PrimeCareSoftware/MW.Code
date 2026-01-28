using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task<CustomDashboardDto> GetDashboardAsync(int id)
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
                CreatedAt = DateTime.UtcNow,
                Layout = "{}"
            };

            _context.Set<CustomDashboard>().Add(dashboard);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard created successfully with ID: {DashboardId}", dashboard.Id);

            return MapToDto(dashboard);
        }

        public async Task<CustomDashboardDto> UpdateDashboardAsync(int id, UpdateDashboardDto dto)
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
            dashboard.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Dashboard updated successfully");

            return MapToDto(dashboard);
        }

        public async Task DeleteDashboardAsync(int id)
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

        public async Task<DashboardWidgetDto> AddWidgetAsync(int dashboardId, CreateWidgetDto dto)
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

        public async Task UpdateWidgetPositionAsync(int widgetId, WidgetPositionDto position)
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

        public async Task DeleteWidgetAsync(int widgetId)
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

        public async Task<WidgetDataDto> ExecuteWidgetQueryAsync(int widgetId)
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

        public async Task<byte[]> ExportDashboardAsync(int id, ExportFormat format)
        {
            _logger.LogInformation("Exporting dashboard: {DashboardId} as {Format}", id, format);

            var dashboard = await GetDashboardAsync(id);

            _logger.LogWarning("Dashboard export feature is not yet implemented. Returning empty result.");
            throw new NotImplementedException($"Dashboard export to {format} format is not yet implemented");
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
                                row[columnName] = value;
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
    }
}
