using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for importing TUSS procedures from CSV/Excel files
    /// 
    /// Expected CSV Format:
    /// Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
    /// 10101012,Consulta médica,01,Consulta médica em consultório,100.00,false
    /// 
    /// Notes:
    /// - Code: 8-digit TUSS code
    /// - Name: Short procedure name (used in Description field)
    /// - Category: Category code or name
    /// - Description: Full procedure description (optional, defaults to Name)
    /// - ReferencePrice: Decimal price (use dot or comma as decimal separator)
    /// - RequiresAuthorization: true/false or 1/0 or yes/no
    /// </summary>
    public class TussImportService : ITussImportService
    {
        private readonly ITussProcedureRepository _repository;
        private readonly ILogger<TussImportService> _logger;
        private ImportResult? _lastImportResult;

        public TussImportService(
            ITussProcedureRepository repository,
            ILogger<TussImportService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ImportResult> ImportFromCsvAsync(Stream csvStream, string tenantId)
        {
            _logger.LogInformation("Starting TUSS import from CSV for tenant {TenantId}", tenantId);
            
            var stopwatch = Stopwatch.StartNew();
            var result = new ImportResult
            {
                ImportedAt = DateTime.UtcNow,
                TenantId = tenantId
            };

            try
            {
                using var reader = new StreamReader(csvStream, Encoding.UTF8);
                
                // Read header
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    result.Success = false;
                    result.Errors.Add(new ImportError
                    {
                        LineNumber = 1,
                        Message = "CSV file is empty or has no header"
                    });
                    return result;
                }

                var headers = ParseCsvLine(headerLine);
                ValidateHeaders(headers, result);
                
                if (!result.Success)
                {
                    return result;
                }

                // Process records in batches for better performance
                var procedures = new List<TussProcedure>();
                var lineNumber = 1;

                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();
                    
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    result.TotalRecords++;

                    try
                    {
                        var values = ParseCsvLine(line);
                        var procedure = ParseProcedure(values, tenantId, lineNumber, result);
                        
                        if (procedure != null)
                        {
                            procedures.Add(procedure);
                        }

                        // Batch save every 100 records
                        if (procedures.Count >= 100)
                        {
                            await SaveBatchAsync(procedures, result);
                            procedures.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing line {LineNumber}", lineNumber);
                        result.Errors.Add(new ImportError
                        {
                            LineNumber = lineNumber,
                            Message = "Error processing line",
                            Details = ex.Message
                        });
                        result.FailedImports++;
                    }
                }

                // Save remaining procedures
                if (procedures.Count > 0)
                {
                    await SaveBatchAsync(procedures, result);
                }

                result.Success = result.SuccessfulImports > 0;
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;

                _lastImportResult = result;

                _logger.LogInformation("TUSS import completed: {Summary}", result.GetSummary());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error during TUSS import");
                result.Success = false;
                result.Errors.Add(new ImportError
                {
                    LineNumber = 0,
                    Message = "Fatal import error",
                    Details = ex.Message
                });
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
            }

            return result;
        }

        public async Task<ImportResult> ImportFromExcelAsync(Stream excelStream, string tenantId)
        {
            _logger.LogInformation("Starting TUSS import from Excel for tenant {TenantId}", tenantId);
            
            var result = new ImportResult
            {
                ImportedAt = DateTime.UtcNow,
                TenantId = tenantId,
                Success = false
            };

            // For Excel support, we would need EPPlus or ClosedXML library
            // For now, return a message to convert Excel to CSV
            result.Errors.Add(new ImportError
            {
                LineNumber = 0,
                Message = "Excel import not yet implemented",
                Details = "Please convert your Excel file to CSV format and use the CSV import endpoint. " +
                          "To enable Excel support, install EPPlus or ClosedXML NuGet package and implement this method."
            });

            _logger.LogWarning("Excel import attempted but not implemented");
            
            return await Task.FromResult(result);
        }

        public async Task<ImportResult?> GetLastImportStatusAsync(string tenantId)
        {
            // In a production system, this should be stored in database
            // For now, return the in-memory last result if it matches the tenant
            if (_lastImportResult != null && _lastImportResult.TenantId == tenantId)
            {
                return await Task.FromResult(_lastImportResult);
            }

            return await Task.FromResult<ImportResult?>(null);
        }

        private void ValidateHeaders(List<string> headers, ImportResult result)
        {
            var requiredHeaders = new[] { "Code", "Name", "Category", "ReferencePrice" };
            var missingHeaders = requiredHeaders.Where(h => 
                !headers.Any(header => header.Equals(h, StringComparison.OrdinalIgnoreCase))).ToList();

            if (missingHeaders.Any())
            {
                result.Success = false;
                result.Errors.Add(new ImportError
                {
                    LineNumber = 1,
                    Message = $"Missing required headers: {string.Join(", ", missingHeaders)}"
                });
            }
        }

        private TussProcedure? ParseProcedure(List<string> values, string tenantId, int lineNumber, ImportResult result)
        {
            if (values.Count < 4)
            {
                result.Errors.Add(new ImportError
                {
                    LineNumber = lineNumber,
                    Message = "Insufficient columns",
                    Details = $"Expected at least 4 columns, found {values.Count}"
                });
                result.FailedImports++;
                return null;
            }

            try
            {
                var code = values[0].Trim();
                var name = values[1].Trim();
                var category = values[2].Trim();
                var description = values.Count > 3 && !string.IsNullOrWhiteSpace(values[3]) 
                    ? values[3].Trim() 
                    : name;
                
                // Parse reference price
                if (!TryParseDecimal(values.Count > 4 ? values[4] : "0", out var referencePrice))
                {
                    result.Errors.Add(new ImportError
                    {
                        LineNumber = lineNumber,
                        Code = code,
                        Message = "Invalid reference price",
                        Details = $"Could not parse price: {(values.Count > 4 ? values[4] : "missing")}"
                    });
                    result.FailedImports++;
                    return null;
                }

                // Parse requires authorization
                var requiresAuthorization = false;
                if (values.Count > 5 && !string.IsNullOrWhiteSpace(values[5]))
                {
                    requiresAuthorization = ParseBoolean(values[5]);
                }

                // Validate code format (should be 8 digits)
                if (string.IsNullOrWhiteSpace(code) || code.Length != 8 || !code.All(char.IsDigit))
                {
                    result.Errors.Add(new ImportError
                    {
                        LineNumber = lineNumber,
                        Code = code,
                        Message = "Invalid TUSS code format",
                        Details = "Code must be exactly 8 digits"
                    });
                    result.FailedImports++;
                    return null;
                }

                var procedure = new TussProcedure(
                    code,
                    description,
                    category,
                    referencePrice,
                    tenantId,
                    requiresAuthorization
                );

                return procedure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing procedure at line {LineNumber}", lineNumber);
                result.Errors.Add(new ImportError
                {
                    LineNumber = lineNumber,
                    Message = "Error parsing procedure",
                    Details = ex.Message
                });
                result.FailedImports++;
                return null;
            }
        }

        private async Task SaveBatchAsync(List<TussProcedure> procedures, ImportResult result)
        {
            _logger.LogDebug("Saving batch of {Count} procedures", procedures.Count);

            foreach (var procedure in procedures)
            {
                try
                {
                    // Check if procedure already exists
                    var existing = await _repository.GetByCodeAsync(procedure.Code, procedure.TenantId);
                    
                    if (existing != null)
                    {
                        // Update existing procedure
                        existing.UpdateInfo(
                            procedure.Description,
                            procedure.Category,
                            procedure.ReferencePrice,
                            procedure.RequiresAuthorization
                        );
                        _repository.UpdateAsync(existing);
                        result.UpdatedRecords++;
                    }
                    else
                    {
                        // Add new procedure
                        await _repository.AddAsync(procedure);
                        result.SuccessfulImports++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving procedure {Code}", procedure.Code);
                    result.Errors.Add(new ImportError
                    {
                        Code = procedure.Code,
                        Message = "Error saving procedure to database",
                        Details = ex.Message
                    });
                    result.FailedImports++;
                }
            }

            // Commit transaction
            try
            {
                await _repository.SaveChangesAsync();
                _logger.LogDebug("Batch saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing batch transaction");
                result.Errors.Add(new ImportError
                {
                    Message = "Error committing transaction",
                    Details = ex.Message
                });
                // Roll back the counts since save failed
                var batchCount = procedures.Count;
                result.SuccessfulImports = Math.Max(0, result.SuccessfulImports - batchCount);
                result.FailedImports += batchCount;
            }
        }

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString());
            return values;
        }

        private bool TryParseDecimal(string value, out decimal result)
        {
            // Try to parse with both comma and dot as decimal separator
            value = value.Trim().Replace(",", ".");
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        private bool ParseBoolean(string value)
        {
            var normalized = value.Trim().ToLowerInvariant();
            return normalized == "true" || 
                   normalized == "1" || 
                   normalized == "yes" || 
                   normalized == "sim" ||
                   normalized == "s" ||
                   normalized == "y";
        }
    }
}
