# 📊 SNGPC - Integração Completa com ANVISA

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (ANVISA RDC 27/2007)  
**Status Atual:** 30% completo (Dashboard existe, mas falta integração completa)  
**Esforço:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000  
**Prazo:** Q2 2026 (Abril-Junho)

## 📋 Contexto

O **Sistema Nacional de Gerenciamento de Produtos Controlados (SNGPC)** é obrigatório para todas as farmácias e clínicas que prescrevem medicamentos controlados (Portaria 344/98). O sistema deve rastrear todas as prescrições de substâncias controladas e transmitir mensalmente para a ANVISA.

### ✅ O que já foi implementado (30%)

**Dashboard SNGPC - Completo:**
- ✅ `SNGPCDashboardComponent` (~376 linhas)
- ✅ Visualização de estatísticas básicas
- ✅ Contagem de prescrições controladas
- ✅ Entidade `SNGPCReport` criada

**Prescrições Controladas - Completo:**
- ✅ Controle de numeração sequencial
- ✅ Validações ANVISA por tipo
- ✅ Tipos A, B, C1 implementados
- ✅ QR Code para rastreabilidade

### ⏳ O que falta (70%)

1. **Livro de Registro Digital** (30% do trabalho restante)
   - Registro eletrônico de todas as receitas controladas
   - Movimentação de estoque (entrada/saída)
   - Balanço mensal obrigatório
   - Relatórios de auditoria

2. **Transmissão XML Mensal para ANVISA** (40% do trabalho restante)
   - Geração de arquivo XML conforme layout SNGPC
   - Validação contra schemas oficiais
   - Envio automático via webservice ANVISA
   - Protocolo de recebimento e confirmação

3. **Monitoramento e Alertas** (20% do trabalho restante)
   - Alertas de prazo de envio
   - Validações de conformidade
   - Detecção de inconsistências
   - Relatórios de não-conformidade

4. **Integração com Farmácias (opcional)** (10% do trabalho restante)
   - API para farmácias consultarem prescrições
   - Confirmação de dispensação
   - Feedback de controle

## 🎯 Objetivos da Tarefa

Implementar o sistema completo de gerenciamento de produtos controlados (SNGPC) com livro de registro digital, transmissão automática para ANVISA e plena conformidade com RDC 27/2007.

## 📝 Tarefas Detalhadas

### 1. Livro de Registro Digital (3 semanas)

#### 1.1 Modelagem de Dados
```csharp
// Livro de Registro de Controlados
public class ControlledMedicationRegistry
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public DateTime Date { get; set; }
    public string RegistryType { get; set; } // Entrada, Saída, Balanço
    
    // Dados do Medicamento
    public string MedicationName { get; set; }
    public string ActiveIngredient { get; set; }
    public string AnvisaList { get; set; } // A1, A2, A3, B1, B2, C1, etc.
    public string Concentration { get; set; }
    public string PharmaceuticalForm { get; set; }
    
    // Movimentação
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal Balance { get; set; }
    
    // Documento de Origem
    public string DocumentType { get; set; } // Nota Fiscal, Receita, Devolução
    public string DocumentNumber { get; set; }
    public DateTime? DocumentDate { get; set; }
    
    // Prescrição (se saída)
    public int? PrescriptionId { get; set; }
    public string PatientName { get; set; }
    public string PatientCPF { get; set; }
    public string DoctorName { get; set; }
    public string DoctorCRM { get; set; }
    
    // Fornecedor (se entrada)
    public string SupplierName { get; set; }
    public string SupplierCNPJ { get; set; }
    
    // Auditoria
    public int RegisteredByUserId { get; set; }
    public DateTime RegisteredAt { get; set; }
    
    // Navegação
    public Clinic Clinic { get; set; }
    public DigitalPrescription Prescription { get; set; }
    public User RegisteredBy { get; set; }
}

// Balanço Mensal
public class MonthlyControlledBalance
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    
    public string MedicationName { get; set; }
    public string ActiveIngredient { get; set; }
    public string AnvisaList { get; set; }
    
    public decimal InitialBalance { get; set; }
    public decimal TotalIn { get; set; }
    public decimal TotalOut { get; set; }
    public decimal FinalBalance { get; set; }
    
    // Divergências
    public decimal? PhysicalBalance { get; set; } // Contagem física
    public decimal? Discrepancy { get; set; } // Diferença
    public string DiscrepancyReason { get; set; }
    
    public DateTime ClosedAt { get; set; }
    public int ClosedByUserId { get; set; }
    
    // Navegação
    public Clinic Clinic { get; set; }
    public User ClosedBy { get; set; }
}

// Transmissão SNGPC
public class SngpcTransmission
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    
    public DateTime GeneratedAt { get; set; }
    public int GeneratedByUserId { get; set; }
    
    public string XmlFilePath { get; set; }
    public string XmlContent { get; set; } // Store XML
    public string XmlHash { get; set; } // SHA-256 para integridade
    
    // Envio
    public DateTime? SentAt { get; set; }
    public string TransmissionStatus { get; set; } // Pending, Sent, Confirmed, Error
    public string ProtocolNumber { get; set; }
    public string AnvisaResponse { get; set; }
    public string ErrorMessage { get; set; }
    
    public int TotalRecords { get; set; }
    public int TotalPrescriptions { get; set; }
    
    // Navegação
    public Clinic Clinic { get; set; }
    public User GeneratedBy { get; set; }
}
```

#### 1.2 Serviço de Registro
```csharp
public interface IControlledMedicationRegistryService
{
    Task<ControlledMedicationRegistry> RegisterPrescriptionAsync(int prescriptionId);
    Task<ControlledMedicationRegistry> RegisterEntryAsync(RegisterEntryDto dto);
    Task<List<ControlledMedicationRegistry>> GetRegistryAsync(int clinicId, DateTime startDate, DateTime endDate);
    Task<MonthlyControlledBalance> CalculateMonthlyBalanceAsync(int clinicId, int year, int month);
    Task<MonthlyControlledBalance> CloseMonthlyBalanceAsync(int clinicId, int year, int month, int userId);
}

public class ControlledMedicationRegistryService : IControlledMedicationRegistryService
{
    private readonly IControlledMedicationRegistryRepository _repository;
    private readonly IDigitalPrescriptionRepository _prescriptionRepository;
    private readonly ILogger<ControlledMedicationRegistryService> _logger;
    
    public async Task<ControlledMedicationRegistry> RegisterPrescriptionAsync(int prescriptionId)
    {
        var prescription = await _prescriptionRepository.GetByIdWithDetailsAsync(prescriptionId);
        
        if (prescription == null)
            throw new NotFoundException($"Prescrição {prescriptionId} não encontrada");
        
        if (!IsControlledPrescription(prescription))
            throw new InvalidOperationException("Prescrição não é de medicamento controlado");
        
        // Criar registro de saída
        var registry = new ControlledMedicationRegistry
        {
            ClinicId = prescription.ClinicId,
            Date = prescription.PrescriptionDate,
            RegistryType = "Saída",
            
            // Medicamento (pegar primeiro item - controlados tem apenas 1)
            MedicationName = prescription.Items.First().MedicationName,
            ActiveIngredient = prescription.Items.First().ActiveIngredient,
            AnvisaList = MapControlTypeToAnvisaList(prescription.ControlType),
            Concentration = prescription.Items.First().Concentration,
            PharmaceuticalForm = prescription.Items.First().PharmaceuticalForm,
            
            // Movimentação
            QuantityIn = 0,
            QuantityOut = prescription.Items.First().Quantity,
            Balance = 0, // Será calculado
            
            // Documento
            DocumentType = "Receita Médica",
            DocumentNumber = prescription.DocumentNumber,
            DocumentDate = prescription.PrescriptionDate,
            
            // Prescrição
            PrescriptionId = prescription.Id,
            PatientName = prescription.Patient.FullName,
            PatientCPF = prescription.Patient.CPF,
            DoctorName = prescription.Doctor.FullName,
            DoctorCRM = $"{prescription.Doctor.CRM}/{prescription.Doctor.CRMState}",
            
            RegisteredByUserId = prescription.CreatedByUserId,
            RegisteredAt = DateTime.UtcNow
        };
        
        // Calcular saldo
        var previousBalance = await GetLatestBalanceAsync(
            prescription.ClinicId, 
            registry.MedicationName
        );
        
        registry.Balance = previousBalance - registry.QuantityOut;
        
        await _repository.AddAsync(registry);
        
        _logger.LogInformation($"Registro SNGPC criado: Prescrição {prescriptionId}, Medicamento: {registry.MedicationName}");
        
        return registry;
    }
    
    public async Task<ControlledMedicationRegistry> RegisterEntryAsync(RegisterEntryDto dto)
    {
        // Validar
        if (string.IsNullOrEmpty(dto.DocumentNumber))
            throw new ValidationException("Número de documento obrigatório");
        
        var registry = new ControlledMedicationRegistry
        {
            ClinicId = dto.ClinicId,
            Date = dto.Date,
            RegistryType = "Entrada",
            
            MedicationName = dto.MedicationName,
            ActiveIngredient = dto.ActiveIngredient,
            AnvisaList = dto.AnvisaList,
            Concentration = dto.Concentration,
            PharmaceuticalForm = dto.PharmaceuticalForm,
            
            QuantityIn = dto.Quantity,
            QuantityOut = 0,
            
            DocumentType = dto.DocumentType, // "Nota Fiscal", "Transferência", etc.
            DocumentNumber = dto.DocumentNumber,
            DocumentDate = dto.DocumentDate,
            
            SupplierName = dto.SupplierName,
            SupplierCNPJ = dto.SupplierCNPJ,
            
            RegisteredByUserId = dto.UserId,
            RegisteredAt = DateTime.UtcNow
        };
        
        // Calcular saldo
        var previousBalance = await GetLatestBalanceAsync(dto.ClinicId, dto.MedicationName);
        registry.Balance = previousBalance + registry.QuantityIn;
        
        await _repository.AddAsync(registry);
        
        return registry;
    }
    
    public async Task<MonthlyControlledBalance> CalculateMonthlyBalanceAsync(int clinicId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        var registries = await _repository.GetByClinicAndPeriodAsync(clinicId, startDate, endDate);
        
        // Agrupar por medicamento
        var balancesByMedication = registries
            .GroupBy(r => new { r.MedicationName, r.ActiveIngredient, r.AnvisaList })
            .Select(g => new MonthlyControlledBalance
            {
                ClinicId = clinicId,
                Year = year,
                Month = month,
                
                MedicationName = g.Key.MedicationName,
                ActiveIngredient = g.Key.ActiveIngredient,
                AnvisaList = g.Key.AnvisaList,
                
                InitialBalance = g.First().Balance - g.Sum(r => r.QuantityIn - r.QuantityOut),
                TotalIn = g.Sum(r => r.QuantityIn),
                TotalOut = g.Sum(r => r.QuantityOut),
                FinalBalance = g.Last().Balance
            })
            .ToList();
        
        return balancesByMedication.FirstOrDefault(); // TODO: retornar lista
    }
    
    private async Task<decimal> GetLatestBalanceAsync(int clinicId, string medicationName)
    {
        var latest = await _repository.GetLatestRegistryAsync(clinicId, medicationName);
        return latest?.Balance ?? 0;
    }
    
    private bool IsControlledPrescription(DigitalPrescription prescription)
    {
        return prescription.PrescriptionType == PrescriptionType.Controlled;
    }
    
    private string MapControlTypeToAnvisaList(ControlType controlType)
    {
        return controlType switch
        {
            ControlType.A1 => "A1",
            ControlType.A2 => "A2",
            ControlType.A3 => "A3",
            ControlType.B1 => "B1",
            ControlType.B2 => "B2",
            ControlType.C1 => "C1",
            ControlType.C2 => "C2",
            ControlType.C3 => "C3",
            ControlType.C4 => "C4",
            ControlType.C5 => "C5",
            _ => "OUTROS"
        };
    }
}
```

#### 1.3 Endpoints API
```csharp
[ApiController]
[Route("api/sngpc")]
public class SngpcController : ControllerBase
{
    private readonly IControlledMedicationRegistryService _registryService;
    private readonly ISngpcTransmissionService _transmissionService;
    
    [HttpPost("registry/prescription/{prescriptionId}")]
    public async Task<IActionResult> RegisterPrescription(int prescriptionId)
    {
        var registry = await _registryService.RegisterPrescriptionAsync(prescriptionId);
        return Ok(registry);
    }
    
    [HttpPost("registry/entry")]
    public async Task<IActionResult> RegisterEntry([FromBody] RegisterEntryDto dto)
    {
        var registry = await _registryService.RegisterEntryAsync(dto);
        return Created($"/api/sngpc/registry/{registry.Id}", registry);
    }
    
    [HttpGet("registry")]
    public async Task<IActionResult> GetRegistry(
        [FromQuery] int clinicId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var registries = await _registryService.GetRegistryAsync(clinicId, startDate, endDate);
        return Ok(registries);
    }
    
    [HttpGet("balance/{year}/{month}")]
    public async Task<IActionResult> GetMonthlyBalance(int year, int month, [FromQuery] int clinicId)
    {
        var balance = await _registryService.CalculateMonthlyBalanceAsync(clinicId, year, month);
        return Ok(balance);
    }
    
    [HttpPost("balance/{year}/{month}/close")]
    public async Task<IActionResult> CloseMonthlyBalance(int year, int month, [FromQuery] int clinicId)
    {
        var userId = GetCurrentUserId();
        var balance = await _registryService.CloseMonthlyBalanceAsync(clinicId, year, month, userId);
        return Ok(balance);
    }
}
```

### 2. Transmissão XML para ANVISA (4 semanas)

#### 2.1 Modelo XML SNGPC
```csharp
// Modelo conforme layout SNGPC da ANVISA
[XmlRoot("mensagem")]
public class SngpcXmlMessage
{
    [XmlElement("cabecalho")]
    public SngpcHeader Header { get; set; }
    
    [XmlArray("movimentacao")]
    [XmlArrayItem("registro")]
    public List<SngpcRecord> Records { get; set; }
}

public class SngpcHeader
{
    [XmlElement("cnpj")]
    public string CNPJ { get; set; }
    
    [XmlElement("razaoSocial")]
    public string CompanyName { get; set; }
    
    [XmlElement("nomeFantasia")]
    public string TradeName { get; set; }
    
    [XmlElement("uf")]
    public string State { get; set; }
    
    [XmlElement("municipio")]
    public string City { get; set; }
    
    [XmlElement("tipoEstabelecimento")]
    public string EstablishmentType { get; set; } // "CONSULTORIO", "CLINICA", etc.
    
    [XmlElement("dataInicio")]
    public DateTime PeriodStart { get; set; }
    
    [XmlElement("dataFim")]
    public DateTime PeriodEnd { get; set; }
    
    [XmlElement("versaoLayout")]
    public string LayoutVersion { get; set; } = "2.1";
}

public class SngpcRecord
{
    [XmlElement("tipoMovimentacao")]
    public string MovementType { get; set; } // "E" (Entrada) ou "S" (Saída)
    
    [XmlElement("data")]
    public DateTime Date { get; set; }
    
    // Medicamento
    [XmlElement("nomeComercial")]
    public string CommercialName { get; set; }
    
    [XmlElement("principioAtivo")]
    public string ActiveIngredient { get; set; }
    
    [XmlElement("lista")]
    public string AnvisaList { get; set; }
    
    [XmlElement("concentracao")]
    public string Concentration { get; set; }
    
    [XmlElement("formaFarmaceutica")]
    public string PharmaceuticalForm { get; set; }
    
    [XmlElement("quantidade")]
    public decimal Quantity { get; set; }
    
    [XmlElement("unidade")]
    public string Unit { get; set; }
    
    // Documento de Origem
    [XmlElement("tipoDocumento")]
    public string DocumentType { get; set; }
    
    [XmlElement("numeroDocumento")]
    public string DocumentNumber { get; set; }
    
    [XmlElement("dataDocumento")]
    public DateTime DocumentDate { get; set; }
    
    // Se Saída - Dados da Prescrição
    [XmlElement("nomePaciente")]
    public string PatientName { get; set; }
    
    [XmlElement("cpfPaciente")]
    public string PatientCPF { get; set; }
    
    [XmlElement("nomeMedico")]
    public string DoctorName { get; set; }
    
    [XmlElement("crmMedico")]
    public string DoctorCRM { get; set; }
    
    [XmlElement("ufCrm")]
    public string CRMState { get; set; }
    
    // Se Entrada - Dados do Fornecedor
    [XmlElement("nomeFornecedor")]
    public string SupplierName { get; set; }
    
    [XmlElement("cnpjFornecedor")]
    public string SupplierCNPJ { get; set; }
}
```

#### 2.2 Serviço de Geração XML
```csharp
public interface ISngpcTransmissionService
{
    Task<SngpcTransmission> GenerateXmlAsync(int clinicId, int year, int month);
    Task<bool> ValidateXmlAsync(string xmlContent);
    Task<SngpcTransmission> SendToAnvisaAsync(int transmissionId);
    Task<bool> CheckTransmissionStatusAsync(int transmissionId);
}

public class SngpcTransmissionService : ISngpcTransmissionService
{
    private readonly IControlledMedicationRegistryRepository _registryRepository;
    private readonly ISngpcTransmissionRepository _transmissionRepository;
    private readonly IClinicRepository _clinicRepository;
    private readonly IAnvisaSngpcClient _anvisaClient;
    private readonly ILogger<SngpcTransmissionService> _logger;
    
    public async Task<SngpcTransmission> GenerateXmlAsync(int clinicId, int year, int month)
    {
        var clinic = await _clinicRepository.GetByIdAsync(clinicId);
        
        if (clinic == null)
            throw new NotFoundException($"Clínica {clinicId} não encontrada");
        
        // Verificar se já existe transmissão para o período
        var existing = await _transmissionRepository.GetByPeriodAsync(clinicId, year, month);
        if (existing != null && existing.TransmissionStatus == "Confirmed")
        {
            throw new InvalidOperationException("Já existe transmissão confirmada para este período");
        }
        
        // Buscar registros do período
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        var registries = await _registryRepository.GetByClinicAndPeriodAsync(clinicId, startDate, endDate);
        
        if (!registries.Any())
        {
            throw new InvalidOperationException("Nenhum registro encontrado para o período");
        }
        
        // Gerar XML
        var xmlMessage = new SngpcXmlMessage
        {
            Header = new SngpcHeader
            {
                CNPJ = clinic.CNPJ,
                CompanyName = clinic.LegalName ?? clinic.Name,
                TradeName = clinic.Name,
                State = clinic.State,
                City = clinic.City,
                EstablishmentType = "CLINICA", // ou "CONSULTORIO"
                PeriodStart = startDate,
                PeriodEnd = endDate,
                LayoutVersion = "2.1"
            },
            Records = registries.Select(MapToSngpcRecord).ToList()
        };
        
        // Serializar
        var xmlContent = SerializeToXml(xmlMessage);
        
        // Validar
        var isValid = await ValidateXmlAsync(xmlContent);
        if (!isValid)
        {
            throw new InvalidOperationException("XML gerado não passou na validação");
        }
        
        // Calcular hash
        var xmlHash = CalculateSHA256(xmlContent);
        
        // Salvar transmissão
        var transmission = new SngpcTransmission
        {
            ClinicId = clinicId,
            Year = year,
            Month = month,
            GeneratedAt = DateTime.UtcNow,
            GeneratedByUserId = 1, // TODO: pegar do context
            XmlContent = xmlContent,
            XmlHash = xmlHash,
            TransmissionStatus = "Pending",
            TotalRecords = registries.Count,
            TotalPrescriptions = registries.Count(r => r.RegistryType == "Saída")
        };
        
        await _transmissionRepository.AddAsync(transmission);
        
        _logger.LogInformation($"XML SNGPC gerado: Clínica {clinicId}, Período {year}/{month}, {transmission.TotalRecords} registros");
        
        return transmission;
    }
    
    public async Task<bool> ValidateXmlAsync(string xmlContent)
    {
        // Validar contra XSD oficial SNGPC
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, "schemas/sngpc_v2.1.xsd");
        
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet
        };
        
        var isValid = true;
        settings.ValidationEventHandler += (sender, args) =>
        {
            isValid = false;
            _logger.LogError($"Erro de validação XML SNGPC: {args.Message}");
        };
        
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader, settings);
        
        try
        {
            while (xmlReader.Read()) { }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao validar XML SNGPC");
            return false;
        }
        
        return isValid;
    }
    
    public async Task<SngpcTransmission> SendToAnvisaAsync(int transmissionId)
    {
        var transmission = await _transmissionRepository.GetByIdAsync(transmissionId);
        
        if (transmission == null)
            throw new NotFoundException($"Transmissão {transmissionId} não encontrada");
        
        if (transmission.TransmissionStatus == "Confirmed")
            throw new InvalidOperationException("Transmissão já foi confirmada pela ANVISA");
        
        try
        {
            // Enviar para webservice ANVISA
            var response = await _anvisaClient.SendSngpcXmlAsync(transmission.XmlContent);
            
            transmission.SentAt = DateTime.UtcNow;
            transmission.TransmissionStatus = response.Success ? "Sent" : "Error";
            transmission.ProtocolNumber = response.ProtocolNumber;
            transmission.AnvisaResponse = response.Message;
            transmission.ErrorMessage = response.Success ? null : response.ErrorMessage;
            
            await _transmissionRepository.UpdateAsync(transmission);
            
            _logger.LogInformation($"Transmissão SNGPC enviada: ID {transmissionId}, Protocolo: {response.ProtocolNumber}");
            
            return transmission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar transmissão SNGPC {transmissionId}");
            
            transmission.TransmissionStatus = "Error";
            transmission.ErrorMessage = ex.Message;
            await _transmissionRepository.UpdateAsync(transmission);
            
            throw;
        }
    }
    
    public async Task<bool> CheckTransmissionStatusAsync(int transmissionId)
    {
        var transmission = await _transmissionRepository.GetByIdAsync(transmissionId);
        
        if (transmission == null || string.IsNullOrEmpty(transmission.ProtocolNumber))
            return false;
        
        // Consultar status na ANVISA
        var status = await _anvisaClient.CheckProtocolStatusAsync(transmission.ProtocolNumber);
        
        if (status.IsConfirmed)
        {
            transmission.TransmissionStatus = "Confirmed";
            await _transmissionRepository.UpdateAsync(transmission);
            return true;
        }
        
        return false;
    }
    
    private SngpcRecord MapToSngpcRecord(ControlledMedicationRegistry registry)
    {
        return new SngpcRecord
        {
            MovementType = registry.RegistryType == "Entrada" ? "E" : "S",
            Date = registry.Date,
            
            CommercialName = registry.MedicationName,
            ActiveIngredient = registry.ActiveIngredient,
            AnvisaList = registry.AnvisaList,
            Concentration = registry.Concentration,
            PharmaceuticalForm = registry.PharmaceuticalForm,
            Quantity = registry.RegistryType == "Entrada" ? registry.QuantityIn : registry.QuantityOut,
            Unit = "UNIDADE", // TODO: pegar do registro
            
            DocumentType = registry.DocumentType,
            DocumentNumber = registry.DocumentNumber,
            DocumentDate = registry.DocumentDate ?? registry.Date,
            
            // Se saída
            PatientName = registry.PatientName,
            PatientCPF = registry.PatientCPF,
            DoctorName = registry.DoctorName,
            DoctorCRM = registry.DoctorCRM?.Split('/')[0],
            CRMState = registry.DoctorCRM?.Split('/')[1],
            
            // Se entrada
            SupplierName = registry.SupplierName,
            SupplierCNPJ = registry.SupplierCNPJ
        };
    }
    
    private string SerializeToXml<T>(T obj)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        });
        
        serializer.Serialize(xmlWriter, obj);
        return stringWriter.ToString();
    }
    
    private string CalculateSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
```

#### 2.3 Cliente ANVISA Webservice
```csharp
public interface IAnvisaSngpcClient
{
    Task<SngpcSendResponse> SendSngpcXmlAsync(string xmlContent);
    Task<SngpcStatusResponse> CheckProtocolStatusAsync(string protocolNumber);
}

public class AnvisaSngpcClient : IAnvisaSngpcClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AnvisaSngpcClient> _logger;
    
    public AnvisaSngpcClient(HttpClient httpClient, IConfiguration configuration, ILogger<AnvisaSngpcClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        // Configurar base URL (homologação ou produção)
        var baseUrl = configuration["Anvisa:Sngpc:BaseUrl"];
        _httpClient.BaseAddress = new Uri(baseUrl);
    }
    
    public async Task<SngpcSendResponse> SendSngpcXmlAsync(string xmlContent)
    {
        try
        {
            // Endpoint ANVISA para envio SNGPC
            var endpoint = "/sngpc/envio";
            
            var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseXml = await response.Content.ReadAsStringAsync();
                var protocolNumber = ExtractProtocolNumber(responseXml);
                
                return new SngpcSendResponse
                {
                    Success = true,
                    ProtocolNumber = protocolNumber,
                    Message = "Enviado com sucesso"
                };
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro ao enviar SNGPC: {errorMessage}");
                
                return new SngpcSendResponse
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao enviar SNGPC");
            return new SngpcSendResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    public async Task<SngpcStatusResponse> CheckProtocolStatusAsync(string protocolNumber)
    {
        try
        {
            var endpoint = $"/sngpc/consulta/{protocolNumber}";
            
            var response = await _httpClient.GetAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                var responseXml = await response.Content.ReadAsStringAsync();
                var status = ExtractStatus(responseXml);
                
                return new SngpcStatusResponse
                {
                    IsConfirmed = status == "PROCESSADO",
                    Status = status
                };
            }
            
            return new SngpcStatusResponse { IsConfirmed = false, Status = "ERRO" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao consultar protocolo {protocolNumber}");
            return new SngpcStatusResponse { IsConfirmed = false, Status = "ERRO" };
        }
    }
    
    private string ExtractProtocolNumber(string xml)
    {
        // Parse XML response e extrair número de protocolo
        var doc = XDocument.Parse(xml);
        return doc.Descendants("protocolo").FirstOrDefault()?.Value;
    }
    
    private string ExtractStatus(string xml)
    {
        var doc = XDocument.Parse(xml);
        return doc.Descendants("status").FirstOrDefault()?.Value;
    }
}

public class SngpcSendResponse
{
    public bool Success { get; set; }
    public string ProtocolNumber { get; set; }
    public string Message { get; set; }
    public string ErrorMessage { get; set; }
}

public class SngpcStatusResponse
{
    public bool IsConfirmed { get; set; }
    public string Status { get; set; }
}
```

#### 2.4 Endpoints de Transmissão
```csharp
[HttpPost("transmission/generate")]
public async Task<IActionResult> GenerateTransmission([FromQuery] int clinicId, [FromQuery] int year, [FromQuery] int month)
{
    var transmission = await _transmissionService.GenerateXmlAsync(clinicId, year, month);
    return Ok(transmission);
}

[HttpPost("transmission/{id}/send")]
public async Task<IActionResult> SendTransmission(int id)
{
    var transmission = await _transmissionService.SendToAnvisaAsync(id);
    return Ok(transmission);
}

[HttpGet("transmission/{id}/status")]
public async Task<IActionResult> CheckStatus(int id)
{
    var isConfirmed = await _transmissionService.CheckTransmissionStatusAsync(id);
    return Ok(new { confirmed = isConfirmed });
}

[HttpGet("transmission")]
public async Task<IActionResult> GetTransmissions([FromQuery] int clinicId, [FromQuery] int? year = null)
{
    var transmissions = await _transmissionRepository.GetByClinicAsync(clinicId, year);
    return Ok(transmissions);
}

[HttpGet("transmission/{id}/xml")]
[Produces("application/xml")]
public async Task<IActionResult> DownloadXml(int id)
{
    var transmission = await _transmissionRepository.GetByIdAsync(id);
    
    if (transmission == null)
        return NotFound();
    
    return Content(transmission.XmlContent, "application/xml");
}
```

### 3. Frontend - Livro de Registro e Transmissão (2 semanas)

#### 3.1 Componente Livro de Registro
```typescript
// sngpc-registry.component.ts
export class SngpcRegistryComponent implements OnInit {
  registries: ControlledMedicationRegistry[] = [];
  selectedPeriod: { year: number, month: number };
  
  displayedColumns = ['date', 'medicationName', 'anvisaList', 'type', 'quantityIn', 'quantityOut', 'balance', 'document', 'actions'];
  
  async ngOnInit() {
    this.selectedPeriod = {
      year: new Date().getFullYear(),
      month: new Date().getMonth() + 1
    };
    await this.loadRegistries();
  }
  
  async loadRegistries() {
    const startDate = new Date(this.selectedPeriod.year, this.selectedPeriod.month - 1, 1);
    const endDate = new Date(this.selectedPeriod.year, this.selectedPeriod.month, 0);
    
    this.registries = await this.sngpcService.getRegistry(
      this.clinicId,
      startDate,
      endDate
    );
  }
  
  async registerEntry() {
    const dialogRef = this.dialog.open(RegisterEntryDialogComponent, {
      width: '600px',
      data: { clinicId: this.clinicId }
    });
    
    const result = await dialogRef.afterClosed().toPromise();
    if (result) {
      await this.loadRegistries();
      this.toastr.success('Entrada registrada com sucesso');
    }
  }
  
  async viewMonthlyBalance() {
    const balance = await this.sngpcService.getMonthlyBalance(
      this.selectedPeriod.year,
      this.selectedPeriod.month,
      this.clinicId
    );
    
    this.dialog.open(MonthlyBalanceDialogComponent, {
      width: '800px',
      data: { balance }
    });
  }
}
```

```html
<!-- sngpc-registry.component.html -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Livro de Registro de Controlados - SNGPC</mat-card-title>
    <mat-card-subtitle>
      Registro obrigatório conforme ANVISA RDC 27/2007
    </mat-card-subtitle>
  </mat-card-header>
  
  <mat-card-content>
    <!-- Seletor de período -->
    <div class="period-selector">
      <mat-form-field appearance="outline">
        <mat-label>Ano</mat-label>
        <mat-select [(ngModel)]="selectedPeriod.year" (selectionChange)="loadRegistries()">
          <mat-option *ngFor="let year of years" [value]="year">{{year}}</mat-option>
        </mat-select>
      </mat-form-field>
      
      <mat-form-field appearance="outline">
        <mat-label>Mês</mat-label>
        <mat-select [(ngModel)]="selectedPeriod.month" (selectionChange)="loadRegistries()">
          <mat-option *ngFor="let month of months" [value]="month.value">{{month.name}}</mat-option>
        </mat-select>
      </mat-form-field>
      
      <button mat-raised-button color="primary" (click)="registerEntry()">
        <mat-icon>add</mat-icon>
        Registrar Entrada
      </button>
      
      <button mat-raised-button (click)="viewMonthlyBalance()">
        <mat-icon>assessment</mat-icon>
        Balanço Mensal
      </button>
    </div>
    
    <!-- Tabela de registros -->
    <table mat-table [dataSource]="registries" class="full-width">
      <ng-container matColumnDef="date">
        <th mat-header-cell *matHeaderCellDef>Data</th>
        <td mat-cell *matCellDef="let registry">{{registry.date | date:'dd/MM/yyyy'}}</td>
      </ng-container>
      
      <ng-container matColumnDef="medicationName">
        <th mat-header-cell *matHeaderCellDef>Medicamento</th>
        <td mat-cell *matCellDef="let registry">
          <div>
            <strong>{{registry.medicationName}}</strong>
            <small class="text-muted">{{registry.activeIngredient}}</small>
          </div>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="anvisaList">
        <th mat-header-cell *matHeaderCellDef>Lista ANVISA</th>
        <td mat-cell *matCellDef="let registry">
          <mat-chip [class]="getListClass(registry.anvisaList)">
            {{registry.anvisaList}}
          </mat-chip>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="type">
        <th mat-header-cell *matHeaderCellDef>Tipo</th>
        <td mat-cell *matCellDef="let registry">
          <mat-icon [color]="registry.registryType === 'Entrada' ? 'primary' : 'warn'">
            {{registry.registryType === 'Entrada' ? 'arrow_downward' : 'arrow_upward'}}
          </mat-icon>
          {{registry.registryType}}
        </td>
      </ng-container>
      
      <ng-container matColumnDef="quantityIn">
        <th mat-header-cell *matHeaderCellDef>Entrada</th>
        <td mat-cell *matCellDef="let registry">{{registry.quantityIn || '-'}}</td>
      </ng-container>
      
      <ng-container matColumnDef="quantityOut">
        <th mat-header-cell *matHeaderCellDef>Saída</th>
        <td mat-cell *matCellDef="let registry">{{registry.quantityOut || '-'}}</td>
      </ng-container>
      
      <ng-container matColumnDef="balance">
        <th mat-header-cell *matHeaderCellDef>Saldo</th>
        <td mat-cell *matCellDef="let registry">
          <strong>{{registry.balance}}</strong>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="document">
        <th mat-header-cell *matHeaderCellDef>Documento</th>
        <td mat-cell *matCellDef="let registry">
          <div>
            <small>{{registry.documentType}}</small>
            <small>{{registry.documentNumber}}</small>
          </div>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef>Ações</th>
        <td mat-cell *matCellDef="let registry">
          <button mat-icon-button (click)="viewDetails(registry)">
            <mat-icon>visibility</mat-icon>
          </button>
        </td>
      </ng-container>
      
      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
  </mat-card-content>
</mat-card>
```

#### 3.2 Componente de Transmissão SNGPC
```typescript
// sngpc-transmission.component.ts
export class SngpcTransmissionComponent implements OnInit {
  transmissions: SngpcTransmission[] = [];
  selectedClinic: number;
  
  async ngOnInit() {
    await this.loadTransmissions();
  }
  
  async generateTransmission() {
    const dialogRef = this.dialog.open(GenerateTransmissionDialogComponent, {
      width: '500px',
      data: { clinicId: this.selectedClinic }
    });
    
    const result = await dialogRef.afterClosed().toPromise();
    
    if (result) {
      try {
        const transmission = await this.sngpcService.generateTransmission(
          result.clinicId,
          result.year,
          result.month
        );
        
        this.toastr.success('XML SNGPC gerado com sucesso');
        await this.loadTransmissions();
      } catch (error) {
        this.toastr.error('Erro ao gerar XML: ' + error.message);
      }
    }
  }
  
  async sendTransmission(transmissionId: number) {
    const confirmed = await this.confirmDialog.show({
      title: 'Enviar para ANVISA',
      message: 'Confirma o envio desta transmissão para a ANVISA? Esta ação não pode ser desfeita.',
      confirmText: 'Sim, Enviar',
      cancelText: 'Cancelar'
    });
    
    if (confirmed) {
      try {
        await this.sngpcService.sendTransmission(transmissionId);
        this.toastr.success('Transmissão enviada para ANVISA com sucesso');
        await this.loadTransmissions();
      } catch (error) {
        this.toastr.error('Erro ao enviar: ' + error.message);
      }
    }
  }
  
  async checkStatus(transmissionId: number) {
    try {
      const response = await this.sngpcService.checkStatus(transmissionId);
      
      if (response.confirmed) {
        this.toastr.success('Transmissão confirmada pela ANVISA!');
      } else {
        this.toastr.info('Transmissão ainda não confirmada');
      }
      
      await this.loadTransmissions();
    } catch (error) {
      this.toastr.error('Erro ao verificar status');
    }
  }
  
  async downloadXml(transmissionId: number) {
    const blob = await this.sngpcService.downloadXml(transmissionId);
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `sngpc_${transmissionId}.xml`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
```

### 4. Monitoramento e Alertas (1 semana)

#### 4.1 Serviço de Alertas
```csharp
public class SngpcMonitoringService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckPendingTransmissionsAsync();
            await CheckOverdueBalancesAsync();
            
            // Executar diariamente
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
    
    private async Task CheckPendingTransmissionsAsync()
    {
        // Verificar transmissões pendentes há mais de 7 dias
        var pendingTransmissions = await _transmissionRepository
            .GetPendingTransmissionsAsync(DateTime.UtcNow.AddDays(-7));
        
        foreach (var transmission in pendingTransmissions)
        {
            await _notificationService.SendAlertAsync(
                transmission.ClinicId,
                "SNGPC - Transmissão Pendente",
                $"A transmissão {transmission.Id} do período {transmission.Month}/{transmission.Year} está pendente há mais de 7 dias"
            );
        }
    }
    
    private async Task CheckOverdueBalancesAsync()
    {
        // Verificar balanços mensais não fechados
        var today = DateTime.Today;
        
        // Balanço do mês anterior deve ser fechado até dia 10
        if (today.Day > 10)
        {
            var lastMonth = today.AddMonths(-1);
            var unclosedBalances = await _registryRepository
                .GetUnclosedBalancesAsync(lastMonth.Year, lastMonth.Month);
            
            foreach (var clinicId in unclosedBalances)
            {
                await _notificationService.SendAlertAsync(
                    clinicId,
                    "SNGPC - Balanço Mensal Não Fechado",
                    $"O balanço mensal de {lastMonth:MMMM/yyyy} ainda não foi fechado. Prazo: dia 10"
                );
            }
        }
    }
}
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] Livro de registro digital funcional
- [ ] Rastreamento completo de controlados
- [ ] XML SNGPC validado contra XSD oficial
- [ ] Transmissão automática para ANVISA
- [ ] Protocolo de recebimento registrado
- [ ] Performance: geração de XML < 10s para 1000 registros

### Funcionais
- [ ] Registro automático de prescrições controladas
- [ ] Registro manual de entradas de estoque
- [ ] Balanço mensal calculado automaticamente
- [ ] Alertas de prazo de transmissão
- [ ] Interface intuitiva para usuários não-técnicos

### Conformidade Legal (ANVISA RDC 27/2007)
- [ ] ✅ Livro de registro digital conforme especificação
- [ ] ✅ Transmissão mensal obrigatória implementada
- [ ] ✅ Retenção de dados por 2+ anos
- [ ] ✅ Rastreabilidade completa
- [ ] ✅ Auditoria de todas as operações

### Operacional
- [ ] Taxa de envio bem-sucedido: >95%
- [ ] Tempo médio de geração: <30 segundos
- [ ] Zero atrasos em transmissões mensais
- [ ] Satisfação de usuários: >7/10

## 📦 Entregáveis

1. **Código Backend**
   - `ControlledMedicationRegistry` entity
   - `MonthlyControlledBalance` entity
   - `SngpcTransmission` entity
   - Serviços e repositórios
   - Cliente ANVISA webservice
   - Background service de monitoramento

2. **Código Frontend**
   - `SngpcRegistryComponent`
   - `SngpcTransmissionComponent`
   - `MonthlyBalanceDialogComponent`
   - Dashboard atualizado

3. **Schemas e Validação**
   - XSD SNGPC v2.1
   - Scripts de validação
   - Exemplos de XML válidos

4. **Documentação**
   - Guia do usuário SNGPC
   - Manual de transmissão mensal
   - Procedimentos de auditoria
   - FAQ e troubleshooting

## 🔗 Dependências

### Pré-requisitos (✅ Completos)
- ✅ Sistema de prescrições digitais 80% completo
- ✅ Prescrições controladas funcionais
- ✅ Dashboard SNGPC básico

### Dependências Externas
- Credenciais ANVISA webservice
- Certificado digital para assinatura XML
- CNPJ da clínica

### Bibliotecas Necessárias
- System.Xml (validação XSD)
- System.Security.Cryptography (hash)

### Tarefas Dependentes
- **Prescrições Digitais** - Fonte de dados para SNGPC
- **Assinatura Digital** - Para assinatura de XML

## 🧪 Testes

### Testes Unitários
```csharp
[Fact]
public async Task RegisterPrescription_ControlledMedication_ShouldCreateRegistry()
{
    // Arrange
    var prescription = CreateControlledPrescription();
    
    // Act
    var registry = await _registryService.RegisterPrescriptionAsync(prescription.Id);
    
    // Assert
    Assert.NotNull(registry);
    Assert.Equal("Saída", registry.RegistryType);
    Assert.True(registry.Balance >= 0);
}

[Fact]
public async Task GenerateXml_ValidPeriod_ShouldValidateAgainstXsd()
{
    // Arrange
    var clinicId = 1;
    var year = 2026;
    var month = 1;
    
    // Act
    var transmission = await _transmissionService.GenerateXmlAsync(clinicId, year, month);
    
    // Assert
    Assert.NotNull(transmission.XmlContent);
    var isValid = await _transmissionService.ValidateXmlAsync(transmission.XmlContent);
    Assert.True(isValid);
}
```

### Testes de Integração
- Fluxo completo: criar prescrição controlada → registro automático → balanço mensal → gerar XML → enviar
- Validação XML contra XSD oficial
- Simulação de resposta ANVISA

### Testes E2E
- Médico prescreve controlado → sistema registra → geração de XML mensal → transmissão para ANVISA
- Verificação de alertas de prazo

## 📊 Métricas de Acompanhamento

### Durante Desenvolvimento
- Cobertura de testes: >75%
- Taxa de validação XML: 100%
- Performance de geração: <10s

### Pós-Deploy
- Taxa de transmissão bem-sucedida: meta >95%
- Tempo médio de transmissão: meta <2 minutos
- Taxa de conformidade: meta 100%
- Zero multas ANVISA
- Satisfação de usuários: meta >7/10

## 🚨 Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Webservice ANVISA indisponível | Média | Alto | Retry automático, fila de transmissões |
| XML rejeitado pela ANVISA | Baixa | Alto | Validação rigorosa contra XSD, testes extensivos |
| Esquecimento de transmissão mensal | Média | Crítico | Alertas automáticos, dashboard com deadline |
| Performance ruim com muitos registros | Baixa | Médio | Paginação, indexação, otimização de queries |

## 📚 Referências

### Regulamentações
- [RDC ANVISA 27/2007](http://antigo.anvisa.gov.br/documents/10181/2718376/RDC_27_2007_.pdf) - SNGPC
- [Portaria ANVISA 344/98](https://bvsms.saude.gov.br/bvs/saudelegis/svs/1998/prt0344_12_05_1998_rep.html) - Medicamentos Controlados
- [Manual SNGPC](https://www.gov.br/anvisa/pt-br/assuntos/fiscalizacao-e-monitoramento/sngpc)

### Documentação Técnica
- Layout XML SNGPC v2.1 (ANVISA)
- XSD Schema oficial
- Webservice ANVISA - Documentação de API

### Código Existente
- `src/MedicSoft.Api/Controllers/DigitalPrescriptionsController.cs`
- `frontend/src/app/prescriptions/sngpc-dashboard.component.ts`
- `docs/schemas/sngpc_v2.1.xsd` - Schema ANVISA

---

> **Próximo Passo:** Após concluir esta tarefa, seguir para **05-cfm-2314-telemedicina.md**  
> **Última Atualização:** 23 de Janeiro de 2026
