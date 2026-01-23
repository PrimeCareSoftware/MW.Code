# 🏥 TISS Fase 1 - Integração com Convênios (ANS)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal para convênios (ANS)  
**Status Atual:** 0% completo  
**Esforço:** 3 meses | 2-3 desenvolvedores  
**Custo Estimado:** R$ 135.000  
**Prazo:** Q3-Q4 2026 (Julho-Dezembro)

## 📋 Contexto

O **Padrão TISS (Troca de Informações em Saúde Suplementar)** é obrigatório por lei (ANS) para todas as operadoras de planos de saúde e prestadores de serviços médicos no Brasil. Sem TISS, o sistema **não pode ser vendido** para a maioria das clínicas (70% atendem convênios).

### Por que é CRÍTICO?

- **70% das clínicas brasileiras atendem convênios** (mercado de R$ 200M+)
- Sem TISS, o produto **não é vendável** para este segmento
- **Barreira competitiva** extremamente alta
- Sistemas concorrentes já possuem TISS
- Abre mercado enterprise (redes de clínicas)

### O que é TISS?

O TISS define:
1. **Guias médicas** (Consulta, SP/SADT, Internação, Honorários)
2. **Tabelas obrigatórias** (CBHPM, TUSS, Rol ANS)
3. **XML para faturamento** em lotes
4. **Autorizações prévias** de procedimentos
5. **Protocolo de comunicação** com operadoras

### Versão Atual

- **TISS 4.02.00** (ou mais recente disponível)
- Publicado pela ANS (Agência Nacional de Saúde Suplementar)

## 🎯 Objetivos da Tarefa

Implementar integração completa com o padrão TISS 4.02.00+, permitindo que clínicas façam:
1. Cadastro de operadoras e planos de saúde
2. Vinculação de pacientes a convênios
3. Solicitação de autorizações prévias
4. Geração de guias médicas (SP/SADT)
5. Faturamento em lotes (XML)
6. Relatórios por convênio e glosas

## 📝 Tarefas Detalhadas

### 1. Estudo e Importação de Tabelas (4 semanas)

#### 1.1 Baixar Documentação Oficial
```bash
# Fontes oficiais
# 1. ANS - Portal TISS: https://www.gov.br/ans/pt-br/assuntos/prestadores/padrao-para-troca-de-informacao-de-saude-suplementar-2013-tiss
# 2. CBHPM: https://cbhpm.org.br/
# 3. TUSS: disponível via ANS

# Downloads necessários:
# - TISS 4.02.00 (ou mais recente) - Componentes
# - TISS XML Schemas (XSD)
# - Tabela CBHPM atualizada (Excel/CSV)
# - Tabela TUSS (Excel/CSV)
# - Rol ANS de cobertura obrigatória
```

#### 1.2 Modelagem de Tabelas
```csharp
// Tabela CBHPM - Classificação Brasileira Hierarquizada de Procedimentos Médicos
public class CbhpmProcedure
{
    public int Id { get; set; }
    public string Code { get; set; } // Código CBHPM (ex: "10101012")
    public string Description { get; set; }
    public string Category { get; set; }
    public string Specialty { get; set; }
    
    // Valores de referência
    public decimal ReferencePorte { get; set; }
    public decimal ReferenceUco { get; set; }
    public decimal ReferenceFilmes { get; set; }
    public decimal ReferenceCostOperacional { get; set; }
    
    // Observações
    public string Notes { get; set; }
    public bool IsActive { get; set; }
    
    // Versão da tabela
    public string TableVersion { get; set; }
    public DateTime EffectiveDate { get; set; }
}

// Tabela TUSS - Terminologia Unificada da Saúde Suplementar
public class TussTerm
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string TermType { get; set; } // "Procedimento", "Material", "Medicamento", "Taxa", "Diária"
    
    public bool IsActive { get; set; }
    public string TableVersion { get; set; }
}

// Rol ANS - Procedimentos de Cobertura Obrigatória
public class AnsRolProcedure
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Segmentation { get; set; } // "Ambulatorial", "Hospitalar", "Obstetrícia"
    
    public bool RequiresAuthorization { get; set; }
    public string AuthorizationNotes { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime EffectiveDate { get; set; }
}
```

#### 1.3 Script de Importação
```csharp
public class TissTablesImportService
{
    public async Task ImportCbhpmAsync(string filePath)
    {
        _logger.LogInformation("Iniciando importação CBHPM...");
        
        // Ler arquivo Excel/CSV
        var records = await ReadExcelFileAsync<CbhpmImportDto>(filePath);
        
        var procedures = new List<CbhpmProcedure>();
        
        foreach (var record in records)
        {
            procedures.Add(new CbhpmProcedure
            {
                Code = record.Codigo,
                Description = record.Descricao,
                Category = record.Categoria,
                Specialty = record.Especialidade,
                ReferencePorte = record.Porte,
                ReferenceUco = record.UCO,
                ReferenceFilmes = record.Filmes,
                ReferenceCostOperacional = record.CustoOperacional,
                IsActive = true,
                TableVersion = record.Versao,
                EffectiveDate = record.DataVigencia
            });
        }
        
        // Bulk insert
        await _cbhpmRepository.BulkInsertAsync(procedures);
        
        _logger.LogInformation($"CBHPM importado: {procedures.Count} procedimentos");
    }
    
    public async Task ImportTussAsync(string filePath)
    {
        _logger.LogInformation("Iniciando importação TUSS...");
        
        var records = await ReadExcelFileAsync<TussImportDto>(filePath);
        
        var terms = records.Select(r => new TussTerm
        {
            Code = r.Codigo,
            Description = r.Descricao,
            TermType = r.Tipo,
            IsActive = true,
            TableVersion = r.Versao
        }).ToList();
        
        await _tussRepository.BulkInsertAsync(terms);
        
        _logger.LogInformation($"TUSS importado: {terms.Count} termos");
    }
    
    public async Task ImportAnsRolAsync(string filePath)
    {
        _logger.LogInformation("Iniciando importação Rol ANS...");
        
        var records = await ReadExcelFileAsync<AnsRolImportDto>(filePath);
        
        var procedures = records.Select(r => new AnsRolProcedure
        {
            Code = r.Codigo,
            Description = r.Descricao,
            Segmentation = r.Segmentacao,
            RequiresAuthorization = r.RequerAutorizacao,
            AuthorizationNotes = r.ObservacoesAutorizacao,
            IsActive = true,
            EffectiveDate = r.DataVigencia
        }).ToList();
        
        await _ansRolRepository.BulkInsertAsync(procedures);
        
        _logger.LogInformation($"Rol ANS importado: {procedures.Count} procedimentos");
    }
}
```

#### 1.4 Indexação para Busca Rápida
```sql
-- Criar índices para performance
CREATE INDEX IX_CbhpmProcedures_Code ON CbhpmProcedures(Code);
CREATE INDEX IX_CbhpmProcedures_Description ON CbhpmProcedures(Description);
CREATE FULLTEXT INDEX ON CbhpmProcedures(Description);

CREATE INDEX IX_TussTerms_Code ON TussTerms(Code);
CREATE INDEX IX_TussTerms_Description ON TussTerms(Description);
CREATE FULLTEXT INDEX ON TussTerms(Description);

CREATE INDEX IX_AnsRolProcedures_Code ON AnsRolProcedures(Code);
```

### 2. Modelagem de Dados - Operadoras e Planos (2 semanas)

#### 2.1 Entidades
```csharp
// Operadora de Saúde
public class HealthInsuranceOperator
{
    public int Id { get; set; }
    public string AnsRegistrationNumber { get; set; } // Número ANS da operadora
    public string TradeName { get; set; }
    public string LegalName { get; set; }
    public string CNPJ { get; set; }
    
    // Contato
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    
    // Endereço
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    
    // Configurações TISS
    public bool SupportsTissWebservice { get; set; }
    public string WebserviceUrl { get; set; }
    public string WebserviceUsername { get; set; }
    public string WebservicePassword { get; set; }
    
    // Tabela de Preços (por procedimento)
    public ICollection<OperatorProcedurePricing> ProcedurePricing { get; set; }
    
    // Planos desta operadora
    public ICollection<HealthPlan> HealthPlans { get; set; }
    
    // Estatísticas
    public decimal AverageGlossaRate { get; set; } // Taxa de glosa média
    public int TotalPatients { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Plano de Saúde
public class HealthPlan
{
    public int Id { get; set; }
    public int OperatorId { get; set; }
    public HealthInsuranceOperator Operator { get; set; }
    
    public string PlanCode { get; set; }
    public string PlanName { get; set; }
    public string AnsRegistrationNumber { get; set; } // Número ANS do plano
    
    // Tipo de plano
    public PlanSegmentation Segmentation { get; set; }
    public PlanType Type { get; set; }
    public PlanCoverage Coverage { get; set; }
    
    // Rede
    public string Network { get; set; } // "Básica", "Premium", "Gold", etc.
    
    // Coparticipação
    public bool HasCoparticipation { get; set; }
    public decimal CoparticipationPercentage { get; set; }
    
    // Carências
    public int ConsultationWaitingPeriodDays { get; set; }
    public int ExamWaitingPeriodDays { get; set; }
    public int SurgeryWaitingPeriodDays { get; set; }
    
    public bool IsActive { get; set; }
}

public enum PlanSegmentation
{
    Ambulatorial,
    Hospitalar,
    AmbulatoralHospitalar,
    Obstetricia,
    Odontologico,
    Referencia  // Cobertura completa
}

public enum PlanType
{
    Individual,
    FamilyOrCollective,
    Corporate
}

public enum PlanCoverage
{
    Municipal,
    GroupOfMunicipalities,
    State,
    GroupOfStates,
    National
}

// Tabela de Preços por Operadora
public class OperatorProcedurePricing
{
    public int Id { get; set; }
    public int OperatorId { get; set; }
    public HealthInsuranceOperator Operator { get; set; }
    
    public string ProcedureCode { get; set; } // CBHPM/TUSS
    public string ProcedureDescription { get; set; }
    
    public decimal OperatorPrice { get; set; }
    public decimal OperatorPorte { get; set; }
    public decimal OperatorUco { get; set; }
    
    // Multiplicadores
    public decimal HonoraryMultiplier { get; set; } = 1.0m;
    public decimal FilmsMultiplier { get; set; } = 1.0m;
    
    public DateTime EffectiveDate { get; set; }
    public bool IsActive { get; set; }
}

// Plano de Saúde do Paciente
public class PatientHealthPlan
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    public int HealthPlanId { get; set; }
    public HealthPlan HealthPlan { get; set; }
    
    // Dados da Carteirinha
    public string CardNumber { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    
    // Titular
    public bool IsDependent { get; set; }
    public string HolderName { get; set; }
    public string HolderCPF { get; set; }
    
    // CNS (Cartão Nacional de Saúde)
    public string CnsNumber { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### 2.2 Migrations
```csharp
public class AddTissEntitiesMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "HealthInsuranceOperators",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                AnsRegistrationNumber = table.Column<string>(maxLength: 6, nullable: false),
                TradeName = table.Column<string>(maxLength: 200, nullable: false),
                LegalName = table.Column<string>(maxLength: 200, nullable: false),
                CNPJ = table.Column<string>(maxLength: 14, nullable: false),
                // ... outros campos
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HealthInsuranceOperators", x => x.Id);
            });
        
        migrationBuilder.CreateIndex(
            name: "IX_HealthInsuranceOperators_AnsRegistrationNumber",
            table: "HealthInsuranceOperators",
            column: "AnsRegistrationNumber",
            unique: true);
        
        // Tabelas relacionadas...
    }
}
```

### 3. Backend - Autorizações (3 semanas)

#### 3.1 Modelagem de Guias
```csharp
// Guia TISS Genérica
public class TissGuide
{
    public int Id { get; set; }
    public string GuideNumber { get; set; } // Número sequencial da guia
    public string GuideType { get; set; } // "Consulta", "SP-SADT", "Internacao", "Honorarios"
    
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    public int PatientHealthPlanId { get; set; }
    public PatientHealthPlan PatientHealthPlan { get; set; }
    
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    
    // Dados da Guia
    public DateTime GuideDate { get; set; }
    public DateTime? ServiceDate { get; set; }
    
    // Procedimentos
    public ICollection<TissGuideProcedure> Procedures { get; set; }
    
    // Autorização
    public string AuthorizationNumber { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public AuthorizationStatus AuthorizationStatus { get; set; }
    public DateTime? AuthorizationRequestDate { get; set; }
    public string AuthorizationDenialReason { get; set; }
    
    // Faturamento
    public decimal TotalAmount { get; set; }
    public decimal? GlossaAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public string GlossaReason { get; set; }
    
    public int? TissBatchId { get; set; }
    public TissBatch Batch { get; set; }
    
    public GuideStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
}

public enum AuthorizationStatus
{
    NotRequired,      // Não requer autorização
    Pending,          // Aguardando autorização
    Authorized,       // Autorizado
    PartiallyAuthorized, // Parcialmente autorizado
    Denied            // Negado
}

public enum GuideStatus
{
    Draft,            // Rascunho
    WaitingAuthorization, // Aguardando autorização
    Authorized,       // Autorizado
    ReadyToBill,      // Pronto para faturar
    Billed,           // Faturado
    Paid,             // Pago
    PartiallyPaid,    // Parcialmente pago
    Glossed           // Glosado
}

// Procedimentos da Guia
public class TissGuideProcedure
{
    public int Id { get; set; }
    public int TissGuideId { get; set; }
    public TissGuide TissGuide { get; set; }
    
    public string ProcedureCode { get; set; } // CBHPM/TUSS
    public string ProcedureDescription { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Glosa
    public decimal? GlossedQuantity { get; set; }
    public decimal? GlossedAmount { get; set; }
    public string GlossaReason { get; set; }
    
    // Autorização
    public bool RequiresAuthorization { get; set; }
    public string AuthorizationNumber { get; set; }
    public bool IsAuthorized { get; set; }
}

// Guia de Consulta (SP-SADT)
public class TissConsultationGuide : TissGuide
{
    public string ConsultationType { get; set; } // "Primeira Consulta", "Retorno", "Urgência"
    public string MainComplaint { get; set; }
    public string IndicationType { get; set; } // "Clínica", "Cirúrgica", etc.
    
    public int? ReferringDoctorId { get; set; }
    public Doctor ReferringDoctor { get; set; }
}
```

#### 3.2 Serviço de Autorizações
```csharp
public interface ITissAuthorizationService
{
    Task<TissGuide> CreateGuideAsync(CreateTissGuideDto dto);
    Task<TissGuide> RequestAuthorizationAsync(int guideId);
    Task<TissGuide> UpdateAuthorizationStatusAsync(int guideId, AuthorizationStatus status, string authorizationNumber = null);
    Task<List<TissGuide>> GetPendingAuthorizationsAsync(int clinicId);
}

public class TissAuthorizationService : ITissAuthorizationService
{
    public async Task<TissGuide> CreateGuideAsync(CreateTissGuideDto dto)
    {
        // Validar plano do paciente
        var patientPlan = await _patientHealthPlanRepository.GetByIdAsync(dto.PatientHealthPlanId);
        
        if (patientPlan == null || !patientPlan.IsActive)
            throw new ValidationException("Plano de saúde do paciente inválido ou inativo");
        
        // Verificar validade da carteirinha
        if (patientPlan.ValidUntil.HasValue && patientPlan.ValidUntil.Value < DateTime.Today)
            throw new ValidationException("Carteirinha do plano de saúde vencida");
        
        // Gerar número da guia
        var guideNumber = await GenerateGuideNumberAsync(dto.ClinicId);
        
        // Criar guia
        var guide = new TissGuide
        {
            GuideNumber = guideNumber,
            GuideType = dto.GuideType,
            ClinicId = dto.ClinicId,
            PatientId = dto.PatientId,
            PatientHealthPlanId = dto.PatientHealthPlanId,
            DoctorId = dto.DoctorId,
            GuideDate = DateTime.Today,
            ServiceDate = dto.ServiceDate,
            AuthorizationStatus = AuthorizationStatus.NotRequired,
            Status = GuideStatus.Draft,
            Procedures = new List<TissGuideProcedure>(),
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = dto.UserId
        };
        
        // Adicionar procedimentos
        decimal totalAmount = 0;
        foreach (var procDto in dto.Procedures)
        {
            // Buscar preço da operadora
            var pricing = await _pricingRepository.GetPricingAsync(
                patientPlan.HealthPlan.OperatorId,
                procDto.ProcedureCode
            );
            
            var unitPrice = pricing?.OperatorPrice ?? procDto.UnitPrice;
            var totalPrice = unitPrice * procDto.Quantity;
            
            // Verificar se requer autorização
            var ansProc = await _ansRolRepository.GetByCodeAsync(procDto.ProcedureCode);
            var requiresAuth = ansProc?.RequiresAuthorization ?? false;
            
            var guideProcedure = new TissGuideProcedure
            {
                ProcedureCode = procDto.ProcedureCode,
                ProcedureDescription = procDto.ProcedureDescription,
                Quantity = procDto.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                RequiresAuthorization = requiresAuth,
                IsAuthorized = !requiresAuth
            };
            
            guide.Procedures.Add(guideProcedure);
            totalAmount += totalPrice;
        }
        
        guide.TotalAmount = totalAmount;
        
        // Se algum procedimento requer autorização, marcar guia
        if (guide.Procedures.Any(p => p.RequiresAuthorization))
        {
            guide.AuthorizationStatus = AuthorizationStatus.Pending;
            guide.Status = GuideStatus.WaitingAuthorization;
        }
        else
        {
            guide.Status = GuideStatus.ReadyToBill;
        }
        
        await _repository.AddAsync(guide);
        
        _logger.LogInformation($"Guia TISS criada: {guideNumber}, Valor: {totalAmount:C}");
        
        return guide;
    }
    
    public async Task<TissGuide> RequestAuthorizationAsync(int guideId)
    {
        var guide = await _repository.GetByIdWithDetailsAsync(guideId);
        
        if (guide == null)
            throw new NotFoundException($"Guia {guideId} não encontrada");
        
        if (guide.AuthorizationStatus != AuthorizationStatus.Pending)
            throw new InvalidOperationException("Guia não está aguardando autorização");
        
        // Verificar se operadora tem webservice
        var operator = guide.PatientHealthPlan.HealthPlan.Operator;
        
        if (operator.SupportsTissWebservice)
        {
            // Enviar via webservice
            var response = await _tissWebserviceClient.RequestAuthorizationAsync(guide);
            
            if (response.Success)
            {
                guide.AuthorizationNumber = response.AuthorizationNumber;
                guide.AuthorizationDate = DateTime.Now;
                guide.AuthorizationStatus = AuthorizationStatus.Authorized;
                guide.Status = GuideStatus.Authorized;
                
                // Marcar procedimentos autorizados
                foreach (var proc in guide.Procedures.Where(p => p.RequiresAuthorization))
                {
                    proc.IsAuthorized = true;
                    proc.AuthorizationNumber = response.AuthorizationNumber;
                }
            }
            else
            {
                guide.AuthorizationStatus = AuthorizationStatus.Denied;
                guide.AuthorizationDenialReason = response.DenialReason;
            }
        }
        else
        {
            // Autorização manual (operadora não tem webservice)
            guide.AuthorizationRequestDate = DateTime.Now;
            _logger.LogInformation($"Autorização manual necessária para guia {guide.GuideNumber}");
        }
        
        await _repository.UpdateAsync(guide);
        
        return guide;
    }
    
    private async Task<string> GenerateGuideNumberAsync(int clinicId)
    {
        var year = DateTime.Now.Year;
        var count = await _repository.CountGuidesByClinicAndYearAsync(clinicId, year);
        
        return $"{clinicId:D4}{year}{(count + 1):D6}";
    }
}
```

### 4. Backend - Faturamento em Lotes (3 semanas)

#### 4.1 Modelagem de Lotes
```csharp
public class TissBatch
{
    public int Id { get; set; }
    public string BatchNumber { get; set; }
    
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    
    public int OperatorId { get; set; }
    public HealthInsuranceOperator Operator { get; set; }
    
    // Período de competência
    public int ReferenceMonth { get; set; }
    public int ReferenceYear { get; set; }
    
    // Guias incluídas
    public ICollection<TissGuide> Guides { get; set; }
    public int TotalGuides { get; set; }
    
    // Valores
    public decimal TotalAmount { get; set; }
    public decimal? GlossedAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    
    // XML
    public string XmlFilePath { get; set; }
    public string XmlContent { get; set; }
    public string XmlHash { get; set; }
    
    // Envio
    public DateTime? SentAt { get; set; }
    public string ProtocolNumber { get; set; }
    public BatchStatus Status { get; set; }
    
    // Retorno
    public DateTime? ProcessedAt { get; set; }
    public string OperatorResponse { get; set; }
    public string PaymentReceipt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
}

public enum BatchStatus
{
    Draft,              // Rascunho
    ReadyToSend,        // Pronto para enviar
    Sent,               // Enviado
    Processing,         // Em processamento
    Processed,          // Processado
    PartiallyPaid,      // Parcialmente pago
    Paid,               // Pago
    Rejected            // Rejeitado
}
```

#### 4.2 Geração de XML TISS
```csharp
// Modelo XML TISS (simplificado - o real é muito mais complexo)
[XmlRoot("ansToiss")]
public class TissXmlBatch
{
    [XmlElement("cabecalho")]
    public TissXmlHeader Header { get; set; }
    
    [XmlArray("loteGuias")]
    [XmlArrayItem("guia")]
    public List<TissXmlGuide> Guides { get; set; }
}

public class TissXmlHeader
{
    [XmlElement("identificacaoTransacao")]
    public TissXmlTransaction Transaction { get; set; }
    
    [XmlElement("origem")]
    public TissXmlOrigin Origin { get; set; }
    
    [XmlElement("destino")]
    public TissXmlDestination Destination { get; set; }
    
    [XmlElement("versaoPadrao")]
    public string StandardVersion { get; set; } = "4.02.00";
}

public class TissXmlTransaction
{
    [XmlElement("tipoTransacao")]
    public string TransactionType { get; set; } = "ENVIO_LOTE_GUIAS";
    
    [XmlElement("sequencialTransacao")]
    public string SequentialNumber { get; set; }
    
    [XmlElement("dataRegistroTransacao")]
    public DateTime TransactionDate { get; set; }
    
    [XmlElement("horaRegistroTransacao")]
    public string TransactionTime { get; set; }
}

public class TissXmlOrigin
{
    [XmlElement("codigoPrestadorNaOperadora")]
    public string ProviderCode { get; set; }
    
    [XmlElement("nomeContratado")]
    public string ContractedName { get; set; }
    
    [XmlElement("codigoCNES")]
    public string CnesCode { get; set; }
}

public class TissXmlGuide
{
    [XmlElement("numeroGuiaPrestador")]
    public string ProviderGuideNumber { get; set; }
    
    [XmlElement("numeroGuiaOperadora")]
    public string OperatorGuideNumber { get; set; }
    
    [XmlElement("numeroCarteira")]
    public string CardNumber { get; set; }
    
    [XmlElement("validadeCarteira")]
    public DateTime CardValidity { get; set; }
    
    [XmlElement("nomeBeneficiario")]
    public string BeneficiaryName { get; set; }
    
    [XmlElement("numeroCNS")]
    public string CnsNumber { get; set; }
    
    [XmlElement("atendimentoRN")]
    public string NewbornCare { get; set; } = "N";
    
    [XmlElement("nomeProfissional")]
    public string ProfessionalName { get; set; }
    
    [XmlElement("conselhoProfissional")]
    public string ProfessionalCouncil { get; set; } = "06"; // CRM
    
    [XmlElement("numeroConselhoProfissional")]
    public string CouncilNumber { get; set; }
    
    [XmlElement("UF")]
    public string State { get; set; }
    
    [XmlElement("CBOS")]
    public string CbosCode { get; set; }
    
    [XmlArray("procedimentosExecutados")]
    [XmlArrayItem("procedimento")]
    public List<TissXmlProcedure> Procedures { get; set; }
    
    [XmlElement("valorTotal")]
    public decimal TotalAmount { get; set; }
}

public class TissXmlProcedure
{
    [XmlElement("dataExecucao")]
    public DateTime ExecutionDate { get; set; }
    
    [XmlElement("horaInicial")]
    public string StartTime { get; set; }
    
    [XmlElement("horaFinal")]
    public string EndTime { get; set; }
    
    [XmlElement("codigoTabela")]
    public string TableCode { get; set; } = "22"; // CBHPM
    
    [XmlElement("codigoProcedimento")]
    public string ProcedureCode { get; set; }
    
    [XmlElement("descricaoProcedimento")]
    public string ProcedureDescription { get; set; }
    
    [XmlElement("quantidadeExecutada")]
    public int QuantityExecuted { get; set; }
    
    [XmlElement("viaAcesso")]
    public string AccessRoute { get; set; }
    
    [XmlElement("tecnicaUtilizada")]
    public string TechniqueUsed { get; set; }
    
    [XmlElement("reducaoAcrescimo")]
    public decimal ReductionIncrease { get; set; }
    
    [XmlElement("valorUnitario")]
    public decimal UnitValue { get; set; }
    
    [XmlElement("valorTotal")]
    public decimal TotalValue { get; set; }
}
```

#### 4.3 Serviço de Faturamento
```csharp
public interface ITissBatchService
{
    Task<TissBatch> CreateBatchAsync(CreateBatchDto dto);
    Task<string> GenerateXmlAsync(int batchId);
    Task<bool> ValidateXmlAsync(string xmlContent);
    Task<TissBatch> SendBatchAsync(int batchId);
}

public class TissBatchService : ITissBatchService
{
    public async Task<TissBatch> CreateBatchAsync(CreateBatchDto dto)
    {
        // Buscar guias autorizadas prontas para faturar
        var guides = await _guideRepository.GetGuidesReadyToBillAsync(
            dto.ClinicId,
            dto.OperatorId,
            dto.ReferenceMonth,
            dto.ReferenceYear
        );
        
        if (!guides.Any())
            throw new ValidationException("Nenhuma guia pronta para faturamento encontrada");
        
        // Gerar número do lote
        var batchNumber = await GenerateBatchNumberAsync(dto.ClinicId, dto.OperatorId);
        
        // Calcular totais
        var totalAmount = guides.Sum(g => g.TotalAmount);
        var totalGuides = guides.Count;
        
        // Criar lote
        var batch = new TissBatch
        {
            BatchNumber = batchNumber,
            ClinicId = dto.ClinicId,
            OperatorId = dto.OperatorId,
            ReferenceMonth = dto.ReferenceMonth,
            ReferenceYear = dto.ReferenceYear,
            TotalGuides = totalGuides,
            TotalAmount = totalAmount,
            Status = BatchStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = dto.UserId
        };
        
        await _repository.AddAsync(batch);
        
        // Vincular guias ao lote
        foreach (var guide in guides)
        {
            guide.TissBatchId = batch.Id;
            guide.Status = GuideStatus.Billed;
        }
        
        await _guideRepository.UpdateRangeAsync(guides);
        
        _logger.LogInformation($"Lote TISS criado: {batchNumber}, {totalGuides} guias, Total: {totalAmount:C}");
        
        return batch;
    }
    
    public async Task<string> GenerateXmlAsync(int batchId)
    {
        var batch = await _repository.GetByIdWithDetailsAsync(batchId);
        
        if (batch == null)
            throw new NotFoundException($"Lote {batchId} não encontrado");
        
        var clinic = batch.Clinic;
        var operator = batch.Operator;
        
        // Construir XML
        var xmlBatch = new TissXmlBatch
        {
            Header = new TissXmlHeader
            {
                Transaction = new TissXmlTransaction
                {
                    TransactionType = "ENVIO_LOTE_GUIAS",
                    SequentialNumber = batch.BatchNumber,
                    TransactionDate = DateTime.Now,
                    TransactionTime = DateTime.Now.ToString("HHmmss")
                },
                Origin = new TissXmlOrigin
                {
                    ProviderCode = clinic.ProviderCodeInOperator,
                    ContractedName = clinic.LegalName ?? clinic.Name,
                    CnesCode = clinic.CnesCode
                },
                Destination = new TissXmlDestination
                {
                    OperatorRegistrationNumber = operator.AnsRegistrationNumber,
                    OperatorName = operator.TradeName
                },
                StandardVersion = "4.02.00"
            },
            Guides = batch.Guides.Select(MapGuideToXml).ToList()
        };
        
        // Serializar
        var xmlContent = SerializeToXml(xmlBatch);
        
        // Validar contra XSD
        var isValid = await ValidateXmlAsync(xmlContent);
        
        if (!isValid)
            throw new InvalidOperationException("XML gerado não passou na validação contra XSD");
        
        // Calcular hash
        var xmlHash = CalculateSHA256(xmlContent);
        
        // Salvar
        batch.XmlContent = xmlContent;
        batch.XmlHash = xmlHash;
        batch.Status = BatchStatus.ReadyToSend;
        
        await _repository.UpdateAsync(batch);
        
        return xmlContent;
    }
    
    public async Task<bool> ValidateXmlAsync(string xmlContent)
    {
        // Validar contra XSD oficial TISS
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, "schemas/tiss_componente_organizacao_guia_4.02.00.xsd");
        
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet
        };
        
        var isValid = true;
        settings.ValidationEventHandler += (sender, args) =>
        {
            isValid = false;
            _logger.LogError($"Erro de validação XML TISS: {args.Message}");
        };
        
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader, settings);
        
        try
        {
            while (xmlReader.Read()) { }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção ao validar XML TISS");
            return false;
        }
        
        return isValid;
    }
    
    public async Task<TissBatch> SendBatchAsync(int batchId)
    {
        var batch = await _repository.GetByIdAsync(batchId);
        
        if (batch == null)
            throw new NotFoundException($"Lote {batchId} não encontrado");
        
        if (batch.Status != BatchStatus.ReadyToSend)
            throw new InvalidOperationException("Lote não está pronto para envio");
        
        // Enviar via webservice ou gerar arquivo para envio manual
        var operator = batch.Operator;
        
        if (operator.SupportsTissWebservice)
        {
            try
            {
                var response = await _tissWebserviceClient.SendBatchAsync(batch.XmlContent);
                
                batch.SentAt = DateTime.UtcNow;
                batch.ProtocolNumber = response.ProtocolNumber;
                batch.Status = BatchStatus.Sent;
                
                _logger.LogInformation($"Lote {batch.BatchNumber} enviado. Protocolo: {response.ProtocolNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar lote {batch.BatchNumber}");
                throw;
            }
        }
        else
        {
            // Envio manual - apenas marcar como pronto
            batch.Status = BatchStatus.ReadyToSend;
            _logger.LogInformation($"Lote {batch.BatchNumber} pronto para envio manual");
        }
        
        await _repository.UpdateAsync(batch);
        
        return batch;
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
        
        // Add XML namespace
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("ans", "http://www.ans.gov.br/padroes/tiss/schemas");
        
        serializer.Serialize(xmlWriter, obj, namespaces);
        return stringWriter.ToString();
    }
}
```

### 5. Frontend - Gestão de Convênios (4 semanas)

I'll create a comprehensive TISS interface structure but will keep the code examples concise due to length. Let me continue and finish this prompt:

#### 5.1 Cadastro de Operadoras
```typescript
// operator-list.component.ts (simplified)
export class OperatorListComponent {
  operators: HealthInsuranceOperator[] = [];
  
  async loadOperators() {
    this.operators = await this.tissService.getOperators();
  }
  
  async createOperator() {
    const dialogRef = this.dialog.open(OperatorFormDialogComponent, { width: '800px' });
    const result = await dialogRef.afterClosed().toPromise();
    if (result) await this.loadOperators();
  }
}
```

#### 5.2 Gestão de Autorizações
```typescript
// authorization-dashboard.component.ts (simplified)
export class AuthorizationDashboardComponent {
  pendingAuthorizations: TissGuide[] = [];
  
  async requestAuthorization(guide: TissGuide) {
    await this.tissService.requestAuthorization(guide.id);
    this.toastr.success('Autorização solicitada');
    await this.loadPendingAuthorizations();
  }
}
```

#### 5.3 Faturamento em Lotes
```typescript
// batch-create.component.ts (simplified)
export class BatchCreateComponent {
  async generateBatch() {
    const batch = await this.tissService.createBatch({
      clinicId: this.selectedClinic,
      operatorId: this.selectedOperator,
      referenceMonth: this.month,
      referenceYear: this.year
    });
    
    // Generate XML
    await this.tissService.generateBatchXml(batch.id);
    
    this.router.navigate(['/tiss/batches', batch.id]);
  }
}
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] Tabelas CBHPM, TUSS e Rol ANS importadas
- [ ] XML TISS validado contra XSD oficial ANS
- [ ] Performance: geração de lote com 100 guias <30s
- [ ] Indexação eficiente para buscas
- [ ] Cobertura de testes >70%

### Funcionais
- [ ] Cadastro de operadoras e planos completo
- [ ] Vinculação de pacientes a convênios
- [ ] Solicitação de autorizações prévias
- [ ] Geração de guias médicas (SP/SADT)
- [ ] Faturamento em lotes (XML TISS)
- [ ] Relatórios por convênio
- [ ] Dashboard de glosas

### Conformidade Legal (ANS)
- [ ] ✅ TISS 4.02.00+ compliance total
- [ ] ✅ Tabelas oficiais atualizadas
- [ ] ✅ XML conforme padrão ANS
- [ ] ✅ Assinatura digital de lotes
- [ ] ✅ Protocolo de envio registrado

### Operacional
- [ ] Aceite de lotes por pelo menos 3 operadoras
- [ ] Tempo de geração de XML <1 min para 100 guias
- [ ] Interface intuitiva (usuários não-técnicos)
- [ ] Taxa de erro de XML <1%
- [ ] Satisfação de usuários: >7/10

## 📦 Entregáveis

1. **Backend**
   - Entidades TISS completas
   - Serviços e repositórios
   - Geração de XML TISS
   - Validação contra XSD
   - Webservice client (se aplicável)

2. **Frontend**
   - Cadastro de operadoras/planos
   - Gestão de autorizações
   - Criação de guias
   - Faturamento em lotes
   - Relatórios e dashboards

3. **Tabelas**
   - CBHPM importada
   - TUSS importada
   - Rol ANS importado
   - Scripts de atualização trimestral

4. **Documentação**
   - Guia do usuário TISS
   - Manual de faturamento
   - Troubleshooting
   - FAQ operadoras

## 🔗 Dependências

### Pré-requisitos
- ✅ Sistema de agendamentos
- ✅ Cadastro de pacientes
- ✅ Cadastro de médicos
- ✅ Sistema de pagamentos básico

### Dependências Externas
- Tabelas oficiais ANS (CBHPM, TUSS, Rol)
- XSD schemas TISS 4.02.00+
- Credenciais de operadoras (webservice)

### Tarefas Dependentes
- **Relatórios Financeiros** - Usa dados TISS
- **Dashboard Analytics** - Estatísticas de convênios

## 🧪 Testes

### Testes Unitários (>100 testes)
```csharp
[Fact]
public async Task CreateGuide_WithValidData_ShouldGenerateGuideNumber()
{
    // Arrange & Act
    var guide = await _service.CreateGuideAsync(CreateValidGuideDto());
    
    // Assert
    Assert.NotNull(guide.GuideNumber);
    Assert.Matches(@"\d{18}", guide.GuideNumber);
}

[Fact]
public async Task GenerateXml_ValidBatch_ShouldPassXsdValidation()
{
    // Arrange
    var batch = await CreateTestBatch();
    
    // Act
    var xml = await _batchService.GenerateXmlAsync(batch.Id);
    var isValid = await _batchService.ValidateXmlAsync(xml);
    
    // Assert
    Assert.True(isValid);
}
```

### Testes de Integração
- Fluxo completo: criar guia → autorizar → faturar → gerar XML
- Validação XML contra XSD oficial
- Import de tabelas (CBHPM, TUSS, Rol)

### Testes com Operadoras Reais
- Testar com 2-3 operadoras parceiras
- Envio de lotes de teste
- Validação de retorno

## 📊 Métricas

### Durante Desenvolvimento
- Cobertura: >70%
- Performance geração XML: <30s para 100 guias
- Taxa de validação XML: 100%

### Pós-Deploy
- Taxa de aceitação de lotes: >95%
- Tempo médio de faturamento: <5 minutos
- Taxa de glosa: meta <10%
- ROI: Aumento de 40-60% em vendas
- Satisfação: >8/10

## 🚨 Riscos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| XML rejeitado por operadoras | Média | Alto | Validação rigorosa XSD, testes com operadoras |
| Tabelas desatualizadas | Média | Médio | Script trimestral automático, alertas |
| Complexidade alta para usuários | Alta | Médio | UX simplificada, treinamento, wizards |
| Operadoras sem webservice | Alta | Baixo | Suporte a envio manual (arquivo) |

## 📚 Referências

- [Portal TISS ANS](https://www.gov.br/ans/pt-br/assuntos/prestadores/padrao-para-troca-de-informacao-de-saude-suplementar-2013-tiss)
- [CBHPM](https://cbhpm.org.br/)
- [Rol ANS](https://www.gov.br/ans/pt-br/assuntos/consumidor/o-que-o-seu-plano-deve-cobrir/o-que-e-o-rol-de-procedimentos-e-evento-em-saude)
- TISS 4.02.00 Componentes (ANS)
- XSD Schemas oficiais

---

> **Próximo Passo:** Após concluir esta tarefa, seguir para **07-telemedicina-mvp-finalizacao.md**  
> **Última Atualização:** 23 de Janeiro de 2026
