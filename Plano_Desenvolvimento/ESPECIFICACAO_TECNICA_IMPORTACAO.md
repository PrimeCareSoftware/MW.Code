# 🔧 Especificação Técnica - Sistema de Importação de Dados

> **Documento Técnico Complementar ao:** [PLANO_IMPORTACAO_DADOS.md](PLANO_IMPORTACAO_DADOS.md)  
> **Data de Criação:** 29 de Janeiro de 2026  
> **Versão:** 1.0  
> **Público-Alvo:** Desenvolvedores e Arquitetos

## 📐 Arquitetura Detalhada

### Domain Model (DDD)

#### Aggregate Roots

```csharp
// src/MedicSoft.Domain/Entities/Import/ImportJob.cs
public class ImportJob : BaseEntity
{
    public string FileName { get; private set; }
    public FileFormat Format { get; private set; }
    public ImportStatus Status { get; private set; }
    public ImportType Type { get; private set; } // Patient, Appointment, etc.
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    
    // Configuração de mapeamento
    public ImportMapping Mapping { get; private set; }
    
    // Estatísticas
    public ImportStatistics Statistics { get; private set; }
    
    // Resultados
    private readonly List<ImportRecord> _records = new();
    public IReadOnlyCollection<ImportRecord> Records => _records.AsReadOnly();
    
    // Arquivo original
    public string StorageUrl { get; private set; }
    public long FileSizeBytes { get; private set; }
    
    // Audit
    public Guid ImportedByUserId { get; private set; }
    public Guid TargetClinicId { get; private set; }
    
    public void Start() { /* ... */ }
    public void Complete() { /* ... */ }
    public void Fail(string error) { /* ... */ }
    public void AddRecord(ImportRecord record) { /* ... */ }
}

// src/MedicSoft.Domain/Entities/Import/ImportRecord.cs
public class ImportRecord : BaseEntity
{
    public Guid ImportJobId { get; private set; }
    public int LineNumber { get; private set; }
    public ImportRecordStatus Status { get; private set; }
    public string RawData { get; private set; } // JSON dos dados originais
    public Guid? CreatedEntityId { get; private set; } // ID do paciente criado
    
    private readonly List<ValidationError> _errors = new();
    public IReadOnlyCollection<ValidationError> Errors => _errors.AsReadOnly();
    
    public bool IsValid => !_errors.Any();
    
    public void AddError(string field, string message) { /* ... */ }
    public void MarkAsProcessed(Guid entityId) { /* ... */ }
    public void MarkAsFailed() { /* ... */ }
}
```

#### Value Objects

```csharp
// src/MedicSoft.Domain/ValueObjects/Import/ImportMapping.cs
public class ImportMapping : ValueObject
{
    public Dictionary<string, string> FieldMappings { get; private set; }
    public Dictionary<string, string> ValueTransformations { get; private set; }
    public string? TemplateId { get; private set; }
    public string? TemplateName { get; private set; }
    
    public ImportMapping(
        Dictionary<string, string> fieldMappings,
        Dictionary<string, string>? valueTransformations = null,
        string? templateId = null,
        string? templateName = null)
    {
        FieldMappings = fieldMappings ?? throw new ArgumentNullException(nameof(fieldMappings));
        ValueTransformations = valueTransformations ?? new Dictionary<string, string>();
        TemplateId = templateId;
        TemplateName = templateName;
    }
    
    public string? GetMappedField(string sourceField)
    {
        return FieldMappings.GetValueOrDefault(sourceField);
    }
    
    public string TransformValue(string field, string value)
    {
        var key = $"{field}:{value}";
        return ValueTransformations.GetValueOrDefault(key, value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FieldMappings;
        yield return ValueTransformations;
    }
}

// src/MedicSoft.Domain/ValueObjects/Import/ImportStatistics.cs
public class ImportStatistics : ValueObject
{
    public int TotalRecords { get; private set; }
    public int ProcessedRecords { get; private set; }
    public int SuccessfulRecords { get; private set; }
    public int FailedRecords { get; private set; }
    public int SkippedRecords { get; private set; }
    
    public double ProgressPercentage => 
        TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
    
    public double SuccessRate =>
        ProcessedRecords > 0 ? (double)SuccessfulRecords / ProcessedRecords * 100 : 0;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalRecords;
        yield return ProcessedRecords;
        yield return SuccessfulRecords;
        yield return FailedRecords;
        yield return SkippedRecords;
    }
}

// src/MedicSoft.Domain/ValueObjects/Import/FileFormat.cs
public enum FileFormat
{
    CSV,
    Excel,
    JSON,
    XML
}

// src/MedicSoft.Domain/ValueObjects/Import/ImportStatus.cs
public enum ImportStatus
{
    Pending,
    Validating,
    ValidationFailed,
    ReadyToImport,
    Importing,
    Completed,
    Failed,
    Cancelled
}

// src/MedicSoft.Domain/ValueObjects/Import/ImportType.cs
public enum ImportType
{
    Patient,
    Appointment,
    MedicalRecord,
    Procedure,
    Payment,
    HealthInsurance
}

// src/MedicSoft.Domain/ValueObjects/Import/ValidationError.cs
public class ValidationError : ValueObject
{
    public string Field { get; private set; }
    public string Message { get; private set; }
    public ValidationSeverity Severity { get; private set; }
    
    public ValidationError(string field, string message, ValidationSeverity severity = ValidationSeverity.Error)
    {
        Field = field ?? throw new ArgumentNullException(nameof(field));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Severity = severity;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Field;
        yield return Message;
        yield return Severity;
    }
}

public enum ValidationSeverity
{
    Warning,
    Error,
    Critical
}
```

### Application Layer

#### Services

```csharp
// src/MedicSoft.Application/Services/Import/ImportService.cs
public interface IImportService
{
    Task<ImportJob> CreateImportJobAsync(
        Stream fileStream, 
        string fileName, 
        FileFormat format,
        ImportType type,
        string tenantId,
        Guid userId,
        Guid clinicId,
        CancellationToken cancellationToken = default);
    
    Task<ImportJob> ValidateImportAsync(
        Guid importJobId,
        ImportMapping mapping,
        CancellationToken cancellationToken = default);
    
    Task<ImportJob> ExecuteImportAsync(
        Guid importJobId,
        CancellationToken cancellationToken = default);
    
    Task<ImportJob> GetImportJobAsync(
        Guid importJobId,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<ImportJob>> GetImportJobsAsync(
        string tenantId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    Task CancelImportAsync(
        Guid importJobId,
        CancellationToken cancellationToken = default);
}

// src/MedicSoft.Application/Services/Import/FileParserService.cs
public interface IFileParserService
{
    Task<ParsedData> ParseFileAsync(
        Stream fileStream,
        FileFormat format,
        CancellationToken cancellationToken = default);
    
    Task<List<string>> DetectColumnsAsync(
        Stream fileStream,
        FileFormat format,
        CancellationToken cancellationToken = default);
}

public class ParsedData
{
    public List<string> Headers { get; set; } = new();
    public List<Dictionary<string, string>> Rows { get; set; } = new();
    public int TotalRows => Rows.Count;
}

// src/MedicSoft.Application/Services/Import/ValidationService.cs
public interface IValidationService
{
    Task<ValidationResult> ValidatePatientDataAsync(
        Dictionary<string, string> data,
        string tenantId,
        CancellationToken cancellationToken = default);
    
    Task<List<ValidationError>> ValidateFieldAsync(
        string field,
        string value,
        CancellationToken cancellationToken = default);
}

public class ValidationResult
{
    public bool IsValid => !Errors.Any(e => e.Severity == ValidationSeverity.Error);
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationError> Warnings { get; set; } = new();
}

// src/MedicSoft.Application/Services/Import/MappingService.cs
public interface IMappingService
{
    Task<ImportMapping> CreateMappingAsync(
        Dictionary<string, string> fieldMappings,
        CancellationToken cancellationToken = default);
    
    Task<ImportMapping> AutoDetectMappingAsync(
        List<string> sourceColumns,
        CancellationToken cancellationToken = default);
    
    Task<ImportMapping> GetTemplateAsync(
        string templateId,
        CancellationToken cancellationToken = default);
    
    Task<List<MappingTemplate>> ListTemplatesAsync(
        CancellationToken cancellationToken = default);
}

public class MappingTemplate
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string SourceSystem { get; set; } = null!; // Ex: "iClinic", "Ninsaude"
    public ImportMapping Mapping { get; set; } = null!;
}
```

#### Commands and Handlers

```csharp
// src/MedicSoft.Application/Commands/Import/CreateImportJobCommand.cs
public record CreateImportJobCommand(
    Stream FileStream,
    string FileName,
    FileFormat Format,
    ImportType Type,
    string TenantId,
    Guid UserId,
    Guid ClinicId) : IRequest<ImportJob>;

public class CreateImportJobCommandHandler : IRequestHandler<CreateImportJobCommand, ImportJob>
{
    private readonly IImportService _importService;
    private readonly IStorageService _storageService;
    
    public async Task<ImportJob> Handle(CreateImportJobCommand request, CancellationToken cancellationToken)
    {
        // 1. Upload file to storage
        var storageUrl = await _storageService.UploadAsync(
            request.FileStream,
            $"imports/{request.TenantId}/{request.FileName}",
            cancellationToken);
        
        // 2. Create import job
        var importJob = await _importService.CreateImportJobAsync(
            request.FileStream,
            request.FileName,
            request.Format,
            request.Type,
            request.TenantId,
            request.UserId,
            request.ClinicId,
            cancellationToken);
        
        return importJob;
    }
}

// src/MedicSoft.Application/Commands/Import/ValidateImportCommand.cs
public record ValidateImportCommand(
    Guid ImportJobId,
    ImportMapping Mapping) : IRequest<ImportJob>;

// src/MedicSoft.Application/Commands/Import/ExecuteImportCommand.cs
public record ExecuteImportCommand(
    Guid ImportJobId) : IRequest<ImportJob>;
```

### Infrastructure Layer

#### File Parsers

```csharp
// src/MedicSoft.Infrastructure/Import/Parsers/CsvFileParser.cs
public class CsvFileParser : IFileParser
{
    public async Task<ParsedData> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = DetectDelimiter(stream),
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        });
        
        var headers = new List<string>();
        var rows = new List<Dictionary<string, string>>();
        
        await csv.ReadAsync();
        csv.ReadHeader();
        headers = csv.HeaderRecord?.ToList() ?? new List<string>();
        
        while (await csv.ReadAsync())
        {
            var row = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                row[header] = csv.GetField(header) ?? string.Empty;
            }
            rows.Add(row);
        }
        
        return new ParsedData
        {
            Headers = headers,
            Rows = rows
        };
    }
    
    private string DetectDelimiter(Stream stream)
    {
        // Logic to detect delimiter (,, ;, \t, |)
        // Read first few lines and count occurrences
        return ","; // default
    }
}

// src/MedicSoft.Infrastructure/Import/Parsers/ExcelFileParser.cs
public class ExcelFileParser : IFileParser
{
    public async Task<ParsedData> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.First();
        
        var headers = new List<string>();
        var rows = new List<Dictionary<string, string>>();
        
        // Read headers from first row
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            headers.Add(worksheet.Cells[1, col].Text);
        }
        
        // Read data rows
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var rowData = new Dictionary<string, string>();
            for (int col = 1; col <= headers.Count; col++)
            {
                rowData[headers[col - 1]] = worksheet.Cells[row, col].Text;
            }
            rows.Add(rowData);
        }
        
        return new ParsedData
        {
            Headers = headers,
            Rows = rows
        };
    }
}
```

#### Background Jobs

```csharp
// src/MedicSoft.Infrastructure/Import/Jobs/ImportJobProcessor.cs
public class ImportJobProcessor : IImportJobProcessor
{
    private readonly IImportService _importService;
    private readonly IValidationService _validationService;
    private readonly IPatientRepository _patientRepository;
    private readonly INotificationService _notificationService;
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessImportJobAsync(Guid importJobId, CancellationToken cancellationToken)
    {
        var importJob = await _importService.GetImportJobAsync(importJobId, cancellationToken);
        
        try
        {
            importJob.Start();
            await _importService.UpdateAsync(importJob, cancellationToken);
            
            // Process each record
            foreach (var record in importJob.Records)
            {
                await ProcessRecordAsync(record, importJob, cancellationToken);
            }
            
            importJob.Complete();
            await _importService.UpdateAsync(importJob, cancellationToken);
            
            // Send notification
            await _notificationService.SendImportCompletedNotificationAsync(importJob);
        }
        catch (Exception ex)
        {
            importJob.Fail(ex.Message);
            await _importService.UpdateAsync(importJob, cancellationToken);
            throw;
        }
    }
    
    private async Task ProcessRecordAsync(
        ImportRecord record, 
        ImportJob job, 
        CancellationToken cancellationToken)
    {
        // 1. Validate
        var validationResult = await _validationService.ValidatePatientDataAsync(
            ParseRawData(record.RawData),
            job.TenantId,
            cancellationToken);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                record.AddError(error.Field, error.Message);
            }
            record.MarkAsFailed();
            return;
        }
        
        // 2. Check for duplicates
        var existingPatient = await _patientRepository.GetByDocumentAsync(
            data["Document"],
            job.TenantId,
            cancellationToken);
        
        if (existingPatient != null)
        {
            // Handle duplicate based on configuration
            record.AddError("Document", "Patient already exists");
            record.MarkAsFailed();
            return;
        }
        
        // 3. Create patient
        var patient = CreatePatientFromData(data, job.TenantId);
        await _patientRepository.AddAsync(patient, cancellationToken);
        
        record.MarkAsProcessed(patient.Id);
    }
}
```

### API Layer

#### Controllers

```csharp
// src/MedicSoft.Api/Controllers/ImportController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(ImportJobDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] FileFormat format,
        [FromForm] ImportType type,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var tenantId = User.GetTenantId();
        var clinicId = User.GetClinicId();
        
        using var stream = file.OpenReadStream();
        
        var command = new CreateImportJobCommand(
            stream,
            file.FileName,
            format,
            type,
            tenantId,
            userId,
            clinicId);
        
        var importJob = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetImportJob),
            new { id = importJob.Id },
            importJob.ToDto());
    }
    
    [HttpPost("{id}/validate")]
    [ProducesResponseType(typeof(ImportJobDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate(
        Guid id,
        [FromBody] ImportMappingDto mapping,
        CancellationToken cancellationToken)
    {
        var command = new ValidateImportCommand(id, mapping.ToDomain());
        var importJob = await _mediator.Send(command, cancellationToken);
        
        return Ok(importJob.ToDto());
    }
    
    [HttpPost("{id}/execute")]
    [ProducesResponseType(typeof(ImportJobDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Execute(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Enqueue background job
        var jobId = BackgroundJob.Enqueue<ImportJobProcessor>(
            x => x.ProcessImportJobAsync(id, cancellationToken));
        
        return AcceptedAtAction(
            nameof(GetImportJob),
            new { id },
            new { jobId, message = "Import started" });
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ImportJobDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImportJob(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetImportJobQuery(id);
        var importJob = await _mediator.Send(query, cancellationToken);
        
        return Ok(importJob.ToDto());
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ImportJobDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImportJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var tenantId = User.GetTenantId();
        var query = new GetImportJobsQuery(tenantId, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelImport(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelImportCommand(id);
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
    
    [HttpGet("templates")]
    [ProducesResponseType(typeof(List<MappingTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTemplates(
        CancellationToken cancellationToken)
    {
        var query = new GetMappingTemplatesQuery();
        var templates = await _mediator.Send(query, cancellationToken);
        
        return Ok(templates);
    }
}
```

### Database Schema

```sql
-- Tabela de jobs de importação
CREATE TABLE import_jobs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id VARCHAR(50) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    format VARCHAR(20) NOT NULL,
    import_type VARCHAR(50) NOT NULL,
    status VARCHAR(50) NOT NULL,
    storage_url TEXT NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    
    -- Mapeamento (JSON)
    mapping JSONB NOT NULL,
    
    -- Estatísticas
    total_records INT NOT NULL DEFAULT 0,
    processed_records INT NOT NULL DEFAULT 0,
    successful_records INT NOT NULL DEFAULT 0,
    failed_records INT NOT NULL DEFAULT 0,
    skipped_records INT NOT NULL DEFAULT 0,
    
    -- Timestamps
    started_at TIMESTAMP,
    completed_at TIMESTAMP,
    
    -- Audit
    imported_by_user_id UUID NOT NULL,
    target_clinic_id UUID NOT NULL,
    
    -- Error handling
    error_message TEXT,
    
    -- BaseEntity fields
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_import_jobs_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id),
    CONSTRAINT fk_import_jobs_user FOREIGN KEY (imported_by_user_id) REFERENCES users(id),
    CONSTRAINT fk_import_jobs_clinic FOREIGN KEY (target_clinic_id) REFERENCES clinics(id)
);

CREATE INDEX idx_import_jobs_tenant ON import_jobs(tenant_id);
CREATE INDEX idx_import_jobs_status ON import_jobs(status);
CREATE INDEX idx_import_jobs_created_at ON import_jobs(created_at DESC);

-- Tabela de registros de importação
CREATE TABLE import_records (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    import_job_id UUID NOT NULL,
    line_number INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    raw_data JSONB NOT NULL,
    created_entity_id UUID,
    
    -- Erros de validação (JSON array)
    errors JSONB,
    
    -- BaseEntity fields
    tenant_id VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_import_records_job FOREIGN KEY (import_job_id) REFERENCES import_jobs(id) ON DELETE CASCADE
);

CREATE INDEX idx_import_records_job ON import_records(import_job_id);
CREATE INDEX idx_import_records_status ON import_records(status);

-- Tabela de templates de mapeamento
CREATE TABLE import_mapping_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    source_system VARCHAR(100) NOT NULL,
    import_type VARCHAR(50) NOT NULL,
    mapping JSONB NOT NULL,
    is_public BOOLEAN NOT NULL DEFAULT false,
    tenant_id VARCHAR(50),
    
    -- BaseEntity fields
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT unique_template_name_per_tenant UNIQUE (name, tenant_id)
);

CREATE INDEX idx_import_templates_source ON import_mapping_templates(source_system);
CREATE INDEX idx_import_templates_type ON import_mapping_templates(import_type);
```

## 🔒 Considerações de Segurança

### Validação de Arquivos

```csharp
public class FileUploadValidator
{
    private static readonly string[] AllowedExtensions = { ".csv", ".xlsx", ".xls", ".json", ".xml" };
    private static readonly string[] AllowedMimeTypes = 
    {
        "text/csv",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/json",
        "text/xml",
        "application/xml"
    };
    private const long MaxFileSizeBytes = 104_857_600; // 100 MB
    
    public ValidationResult Validate(IFormFile file)
    {
        var errors = new List<string>();
        
        // Check extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            errors.Add($"File extension '{extension}' not allowed");
        }
        
        // Check MIME type
        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            errors.Add($"MIME type '{file.ContentType}' not allowed");
        }
        
        // Check file size
        if (file.Length > MaxFileSizeBytes)
        {
            errors.Add($"File size ({file.Length} bytes) exceeds maximum ({MaxFileSizeBytes} bytes)");
        }
        
        // Check file content (magic numbers)
        using var stream = file.OpenReadStream();
        if (!IsValidFileContent(stream, extension))
        {
            errors.Add("File content does not match extension");
        }
        
        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
    
    private bool IsValidFileContent(Stream stream, string extension)
    {
        // Read first few bytes to verify file signature (magic numbers)
        var buffer = new byte[8];
        stream.Read(buffer, 0, buffer.Length);
        stream.Position = 0;
        
        return extension switch
        {
            ".xlsx" => buffer[0] == 0x50 && buffer[1] == 0x4B, // PK (ZIP signature)
            ".csv" => true, // No magic number for CSV
            ".json" => buffer[0] == 0x7B || buffer[0] == 0x5B, // { or [
            ".xml" => buffer[0] == 0x3C, // <
            _ => false
        };
    }
}
```

### Sanitização de Dados

```csharp
public class DataSanitizer
{
    public string SanitizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        // Remove HTML tags
        input = Regex.Replace(input, @"<[^>]+>", string.Empty);
        
        // Remove SQL injection attempts
        input = Regex.Replace(input, @"('|(--)|;|\/\*|\*\/)", string.Empty);
        
        // Remove XSS attempts
        input = Regex.Replace(input, @"<script|javascript:|onerror=", string.Empty, RegexOptions.IgnoreCase);
        
        // Trim and normalize whitespace
        input = Regex.Replace(input.Trim(), @"\s+", " ");
        
        return input;
    }
}
```

## 📊 Performance e Otimização

### Batch Insert

```csharp
public async Task BatchInsertPatientsAsync(
    List<Patient> patients,
    CancellationToken cancellationToken)
{
    const int batchSize = 1000;
    
    for (int i = 0; i < patients.Count; i += batchSize)
    {
        var batch = patients.Skip(i).Take(batchSize).ToList();
        
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.Patients.AddRangeAsync(batch, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
```

### Streaming Large Files

```csharp
public async Task<ParsedData> StreamParseAsync(
    Stream fileStream,
    CancellationToken cancellationToken)
{
    var headers = new List<string>();
    var chunkSize = 1000;
    var currentChunk = new List<Dictionary<string, string>>();
    
    using var reader = new StreamReader(fileStream);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    
    await csv.ReadAsync();
    csv.ReadHeader();
    headers = csv.HeaderRecord?.ToList() ?? new();
    
    while (await csv.ReadAsync())
    {
        var row = ReadRow(csv, headers);
        currentChunk.Add(row);
        
        if (currentChunk.Count >= chunkSize)
        {
            // Process chunk
            await ProcessChunkAsync(currentChunk, cancellationToken);
            currentChunk.Clear();
        }
    }
    
    // Process remaining records
    if (currentChunk.Any())
    {
        await ProcessChunkAsync(currentChunk, cancellationToken);
    }
    
    return new ParsedData { Headers = headers };
}
```

## 🧪 Estratégias de Teste

### Unit Tests

```csharp
[Fact]
public async Task ImportJob_Should_ValidatePatientData()
{
    // Arrange
    var validationService = new ValidationService();
    var data = new Dictionary<string, string>
    {
        ["Name"] = "João Silva",
        ["Document"] = "123.456.789-00",
        ["DateOfBirth"] = "1990-01-01",
        ["Gender"] = "Masculino"
    };
    
    // Act
    var result = await validationService.ValidatePatientDataAsync(data, "tenant1");
    
    // Assert
    Assert.True(result.IsValid);
    Assert.Empty(result.Errors);
}

[Fact]
public async Task CsvParser_Should_ParseValidFile()
{
    // Arrange
    var csv = "Name,CPF,BirthDate\nJoão Silva,12345678900,1990-01-01";
    var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
    var parser = new CsvFileParser();
    
    // Act
    var result = await parser.ParseAsync(stream, CancellationToken.None);
    
    // Assert
    Assert.Equal(3, result.Headers.Count);
    Assert.Single(result.Rows);
    Assert.Equal("João Silva", result.Rows[0]["Name"]);
}
```

### Integration Tests

```csharp
[Fact]
public async Task ImportController_Should_CreateImportJob()
{
    // Arrange
    var client = _factory.CreateClient();
    var file = CreateTestFile("patients.csv");
    using var content = new MultipartFormDataContent();
    content.Add(new StreamContent(file), "file", "patients.csv");
    content.Add(new StringContent("CSV"), "format");
    content.Add(new StringContent("Patient"), "type");
    
    // Act
    var response = await client.PostAsync("/api/import/upload", content);
    
    // Assert
    response.EnsureSuccessStatusCode();
    var importJob = await response.Content.ReadFromJsonAsync<ImportJobDto>();
    Assert.NotNull(importJob);
    Assert.Equal(ImportStatus.Pending, importJob.Status);
}
```

## 📚 Referências

- [CsvHelper Documentation](https://joshclose.github.io/CsvHelper/)
- [EPPlus Documentation](https://epplussoftware.com/docs)
- [Hangfire Documentation](https://docs.hangfire.io/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [OWASP File Upload Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/File_Upload_Cheat_Sheet.html)

---

> **Versão:** 1.0  
> **Data:** 29 de Janeiro de 2026  
> **Elaborado por:** GitHub Copilot
