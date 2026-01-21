# TISS/TUSS Implementation Guide

This guide explains how to set up and use the TISS XML validation and TUSS import features.

## Table of Contents

1. [TISS XML Validation](#tiss-xml-validation)
2. [TUSS Procedure Import](#tuss-procedure-import)
3. [API Endpoints](#api-endpoints)
4. [Testing](#testing)

---

## TISS XML Validation

### Overview

The TISS XML Validator Service validates XML files against ANS (Agência Nacional de Saúde Suplementar) official schemas for the TISS 4.02.00 standard.

### Downloading TISS Schemas

1. **Official Source**: ANS (Agência Nacional de Saúde Suplementar)
   - Website: https://www.gov.br/ans/pt-br/assuntos/prestadores/padrao-para-troca-de-informacao-de-saude-suplementar-2013-tiss
   - Navigate to "Padrões de Representação de Conceitos" section
   - Download TISS 4.02.00 schemas package

2. **Schema Files Required**:
   - `tissLoteGuias_4_02_00.xsd` - Main batch schema
   - `tissGuiaConsulta_4_02_00.xsd` - Consultation guide schema
   - `tissGuiaSPSADT_4_02_00.xsd` - SP/SADT guide schema
   - Additional supporting schema files (types, common elements, etc.)

3. **Installation Location**:
   ```
   wwwroot/schemas/tiss/4.02.00/
   ```
   
   Create the directory structure if it doesn't exist:
   ```bash
   mkdir -p wwwroot/schemas/tiss/4.02.00
   ```
   
   Place all downloaded `.xsd` files in this directory.

### Validation Features

- **Basic Validation** (always available):
  - XML well-formedness check
  - Required elements validation
  - TISS version validation
  - Structure consistency checks

- **Schema Validation** (when schema files are available):
  - Full XSD schema validation
  - Data type validation
  - Cardinality validation
  - Pattern validation

### Usage Example

```csharp
// Inject the service
private readonly ITissXmlValidatorService _validatorService;

// Validate a batch XML
var result = await _validatorService.ValidateBatchXmlAsync(xmlContent);

if (result.IsValid)
{
    Console.WriteLine("XML is valid!");
}
else
{
    Console.WriteLine($"Found {result.ErrorCount} errors:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}

// Validate a guide XML
var guideResult = await _validatorService.ValidateGuideXmlAsync(xmlContent);
```

---

## TUSS Procedure Import

### Overview

The TUSS Import Service allows importing the official TUSS (Terminologia Unificada da Saúde Suplementar) procedure table from ANS.

### Downloading TUSS Table

1. **Official Source**: ANS (Agência Nacional de Saúde Suplementar)
   - Website: https://www.gov.br/ans/pt-br/assuntos/prestadores/banco-de-dados-de-procedimentos-tuss
   - Look for "Tabela TUSS" section
   - Download the latest version (typically available as Excel or CSV)

2. **File Format**: 
   - The official table is usually in Excel format (.xlsx)
   - Convert to CSV for import (Excel import coming soon)
   - Ensure UTF-8 encoding for proper character handling

### CSV Format

The import service expects the following CSV format:

```csv
Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
10101012,Consulta médica,01,Consulta médica em consultório,100.00,false
10101020,Consulta em pronto-socorro,01,Consulta médica em pronto-socorro,150.00,true
20101015,Raio X de tórax,02,Radiografia de tórax PA e perfil,80.00,false
```

### Column Specifications

| Column | Required | Type | Description | Notes |
|--------|----------|------|-------------|-------|
| Code | Yes | String (8 digits) | TUSS procedure code | Must be exactly 8 digits |
| Name | Yes | String | Short procedure name | Used as description if Description is empty |
| Category | Yes | String | Category code or name | E.g., "01" for consultations |
| Description | No | String | Full procedure description | Defaults to Name if not provided |
| ReferencePrice | Yes | Decimal | Reference price | Can use dot or comma as decimal separator |
| RequiresAuthorization | No | Boolean | Requires prior authorization | Accepts: true/false, yes/no, 1/0, sim/não |

### Converting Excel to CSV

If you download the official table in Excel format:

#### Using Excel/LibreOffice:
1. Open the Excel file
2. Select "File" > "Save As"
3. Choose format: "CSV (Comma delimited) (*.csv)"
4. Ensure UTF-8 encoding is selected

#### Using Python (for automation):
```python
import pandas as pd

# Read Excel file
df = pd.read_excel('TUSS_oficial.xlsx')

# Rename columns to match expected format
df.columns = ['Code', 'Name', 'Category', 'Description', 'ReferencePrice', 'RequiresAuthorization']

# Save as CSV with UTF-8 encoding
df.to_csv('TUSS_import.csv', index=False, encoding='utf-8')
```

### Import Behavior

- **New procedures**: Created with all provided information
- **Existing procedures**: Updated with new information (matched by Code)
- **Invalid records**: Skipped with detailed error messages in the result
- **Batch processing**: Processes in batches of 100 for better performance
- **Transactional**: Each batch is committed as a transaction

### Usage Example

```csharp
// Inject the service
private readonly ITussImportService _importService;

// Import from CSV stream
using var fileStream = File.OpenRead("TUSS_oficial.csv");
var result = await _importService.ImportFromCsvAsync(fileStream, tenantId);

Console.WriteLine(result.GetSummary());
// Output: "Import successful: 1234 procedures imported, 56 updated in 12.34s"

// Check for errors
if (result.FailedImports > 0)
{
    Console.WriteLine($"Failed imports: {result.FailedImports}");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

---

## API Endpoints

### TUSS Import Endpoints

#### Import from CSV
```http
POST /api/tuss-import/csv
Content-Type: multipart/form-data
Authorization: Bearer {token}

file: TUSS_oficial.csv (max 10 MB)
```

Response:
```json
{
  "success": true,
  "totalRecords": 1234,
  "successfulImports": 1200,
  "updatedRecords": 30,
  "failedImports": 4,
  "errors": [
    {
      "lineNumber": 45,
      "code": "123",
      "message": "Invalid TUSS code format",
      "details": "Code must be exactly 8 digits"
    }
  ],
  "duration": "00:00:12.3456789",
  "importedAt": "2024-01-20T10:30:00Z",
  "tenantId": "clinic-123"
}
```

#### Get Import Status
```http
GET /api/tuss-import/status
Authorization: Bearer {token}
```

#### Get Import Information
```http
GET /api/tuss-import/info
```

Returns format specifications and sample CSV.

### TISS Validation (integrated in XML generator)

The validation service is automatically used when generating TISS XML files if schemas are available.

---

## Testing

### Running Unit Tests

```bash
# Run all tests
dotnet test

# Run only TISS/TUSS tests
dotnet test --filter "FullyQualifiedName~Tiss|FullyQualifiedName~Tuss"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage

- **TissXmlValidatorServiceTests**: 15+ test cases covering validation scenarios
- **TussImportServiceTests**: 20+ test cases covering import scenarios

### Manual Testing

#### Test TUSS Import:

1. Create a test CSV file:
```csv
Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
10101012,Consulta teste,01,Consulta de teste,100.00,false
```

2. Use Swagger UI or curl:
```bash
curl -X POST "https://localhost:5001/api/tuss-import/csv" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "file=@test_tuss.csv"
```

#### Test TISS Validation:

1. Generate a TISS XML using the existing generator
2. The validation happens automatically during generation
3. Check logs for validation results

---

## Troubleshooting

### Common Issues

1. **"Schema files not found" warning**
   - Download TISS schemas from ANS website
   - Place in `wwwroot/schemas/tiss/4.02.00/`
   - Restart the application

2. **"Invalid TUSS code format"**
   - Ensure codes are exactly 8 digits
   - Remove any formatting or spaces
   - Check for leading zeros

3. **"Invalid reference price"**
   - Use decimal values (not currency symbols)
   - Both comma and dot are accepted as decimal separators
   - Example: `100.50` or `100,50`

4. **Import fails with database errors**
   - Check database connection
   - Ensure migrations are up to date
   - Verify tenant permissions

### Performance Tips

- Import large files (10,000+ records) during off-peak hours
- The service processes in batches of 100 for optimal performance
- Monitor memory usage for very large imports
- Consider splitting extremely large files (100,000+ records)

---

## Future Enhancements

### Planned Features

1. **Excel Import Support**
   - Direct import from .xlsx files
   - Requires EPPlus or ClosedXML NuGet package

2. **Import History**
   - Database storage of import history
   - Import audit trail
   - Rollback capability

3. **Advanced Validation**
   - Business rule validation
   - Price range validation
   - Category consistency checks

4. **Scheduled Imports**
   - Automatic periodic imports
   - ANS API integration (when available)

---

## Additional Resources

### Official Documentation
- ANS TISS Standards: https://www.gov.br/ans/pt-br/assuntos/prestadores/padrao-para-troca-de-informacao-de-saude-suplementar-2013-tiss
- ANS TUSS Database: https://www.gov.br/ans/pt-br/assuntos/prestadores/banco-de-dados-de-procedimentos-tuss

### Support
- For technical issues, open a GitHub issue
- For TISS/TUSS standard questions, consult ANS documentation
- For import problems, check the detailed error messages in the import result

---

## License and Compliance

This implementation follows ANS standards for TISS 4.02.00 and TUSS procedure terminology. Always ensure you're using the latest official tables from ANS to maintain compliance with Brazilian healthcare regulations.

**Last Updated**: January 2024
**TISS Version**: 4.02.00
**Status**: Production Ready
