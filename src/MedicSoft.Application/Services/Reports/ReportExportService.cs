using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicSoft.Application.Services.Reports
{
    /// <summary>
    /// Service for exporting reports to various formats (PDF, Excel)
    /// Phase 3: Analytics and BI
    /// </summary>
    public class ReportExportService : IReportExportService
    {
        private readonly ILogger<ReportExportService> _logger;

        public ReportExportService(ILogger<ReportExportService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Export report data to PDF with branding
        /// </summary>
        public async Task<byte[]> ExportToPdfAsync(string reportTitle, string description, List<Dictionary<string, object>> data, string brandName = "MedicSoft")
        {
            _logger.LogInformation("Exporting report to PDF: {ReportTitle}", reportTitle);

            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Header with branding
                        page.Header()
                            .Height(70)
                            .Background(Colors.Blue.Medium)
                            .Padding(10)
                            .Column(column =>
                            {
                                column.Item().Text(brandName)
                                    .FontSize(20)
                                    .Bold()
                                    .FontColor(Colors.White);
                                
                                column.Item().Text(reportTitle)
                                    .FontSize(14)
                                    .FontColor(Colors.White);
                            });

                        // Content
                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                // Description
                                if (!string.IsNullOrEmpty(description))
                                {
                                    column.Item().PaddingBottom(10).Text(description)
                                        .FontSize(11)
                                        .Italic();
                                }

                                // Report generation date
                                column.Item().PaddingBottom(20).Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Medium);

                                // Data table
                                if (data != null && data.Any())
                                {
                                    var firstRow = data.FirstOrDefault();
                                    if (firstRow != null)
                                    {
                                        var headers = firstRow.Keys.ToList();

                                    column.Item().Table(table =>
                                    {
                                        // Define columns - call once for all columns
                                        table.ColumnsDefinition(columns =>
                                        {
                                            foreach (var _ in headers)
                                            {
                                                columns.RelativeColumn();
                                            }
                                        });

                                        // Header row
                                        table.Header(header =>
                                        {
                                            foreach (var headerName in headers)
                                            {
                                                header.Cell()
                                                    .Background(Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(headerName)
                                                    .Bold();
                                            }
                                        });

                                        // Data rows
                                        foreach (var row in data)
                                        {
                                            foreach (var header in headers)
                                            {
                                                var value = row.ContainsKey(header) ? row[header]?.ToString() ?? "" : "";
                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(value);
                                            }
                                        }
                                    });
                                    }
                                }
                                else
                                {
                                    column.Item().Text("No data available")
                                        .FontSize(11)
                                        .Italic();
                                }
                            });

                        // Footer
                        page.Footer()
                            .Height(30)
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" of ");
                                text.TotalPages();
                            });
                    });
                });

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                return stream.ToArray();
            });
        }

        /// <summary>
        /// Export report data to Excel with multiple tabs if needed
        /// </summary>
        public async Task<byte[]> ExportToExcelAsync(string reportTitle, List<Dictionary<string, object>> data, Dictionary<string, List<Dictionary<string, object>>>? additionalSheets = null)
        {
            _logger.LogInformation("Exporting report to Excel: {ReportTitle}", reportTitle);

            return await Task.Run(() =>
            {
                using var workbook = new XLWorkbook();

                // Main sheet
                var mainSheet = workbook.Worksheets.Add(SanitizeSheetName(reportTitle));
                AddDataToSheet(mainSheet, data);

                // Additional sheets (for multiple tabs)
                if (additionalSheets != null)
                {
                    foreach (var kvp in additionalSheets)
                    {
                        var sheet = workbook.Worksheets.Add(SanitizeSheetName(kvp.Key));
                        AddDataToSheet(sheet, kvp.Value);
                    }
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            });
        }

        private void AddDataToSheet(IXLWorksheet sheet, List<Dictionary<string, object>> data)
        {
            if (data == null || !data.Any())
            {
                sheet.Cell(1, 1).Value = "No data available";
                return;
            }

            var firstRow = data.FirstOrDefault();
            if (firstRow == null)
            {
                sheet.Cell(1, 1).Value = "No data available";
                return;
            }

            var headers = firstRow.Keys.ToList();
            
            // Add headers
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = sheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Add data rows
            for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                var row = data[rowIndex];
                for (int colIndex = 0; colIndex < headers.Count; colIndex++)
                {
                    var header = headers[colIndex];
                    var value = row.ContainsKey(header) ? row[header] : null;
                    
                    var cell = sheet.Cell(rowIndex + 2, colIndex + 1);
                    
                    if (value != null)
                    {
                        // Handle different data types
                        if (value is DateTime dateTime)
                        {
                            cell.Value = dateTime;
                            cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                        }
                        else if (value is decimal || value is double || value is float)
                        {
                            cell.Value = Convert.ToDouble(value);
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        }
                        else if (value is int || value is long)
                        {
                            cell.Value = Convert.ToInt64(value);
                        }
                        else
                        {
                            cell.Value = value.ToString();
                        }
                    }
                }
            }

            // Auto-fit columns
            sheet.Columns().AdjustToContents();
        }

        private string SanitizeSheetName(string name)
        {
            // Excel sheet name restrictions
            if (string.IsNullOrEmpty(name))
                return "Sheet1";

            // Remove invalid characters
            var sanitized = new string(name
                .Where(c => !new[] { '\\', '/', '*', '?', ':', '[', ']' }.Contains(c))
                .ToArray());

            // Limit length to 31 characters
            if (sanitized.Length > 31)
                sanitized = sanitized.Substring(0, 31);

            return sanitized;
        }
    }

    /// <summary>
    /// Interface for report export service
    /// </summary>
    public interface IReportExportService
    {
        Task<byte[]> ExportToPdfAsync(string reportTitle, string description, List<Dictionary<string, object>> data, string brandName = "MedicSoft");
        Task<byte[]> ExportToExcelAsync(string reportTitle, List<Dictionary<string, object>> data, Dictionary<string, List<Dictionary<string, object>>>? additionalSheets = null);
    }
}
