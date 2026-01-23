# 🩺 CFM 2.314/2022 - Conformidade Telemedicina

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (Conselho Federal de Medicina)  
**Status Atual:** 0% completo (Microserviço existe, mas sem compliance)  
**Esforço:** 1 mês | 1 desenvolvedor  
**Custo Estimado:** R$ 15.000  
**Prazo:** Q2 2026 (Maio-Junho)

## 📋 Contexto

A **Resolução CFM 2.314/2022** define regras específicas para a prática da telemedicina no Brasil. Sem compliance total com esta resolução, a prática de telemedicina é **ilegal** e pode resultar em processos éticos no CFM.

### ✅ O que já existe

**Microserviço de Telemedicina - 80% Completo:**
- ✅ Microserviço ASP.NET Core criado
- ✅ MVP de videochamadas funcionando
- ✅ Integração básica com WebRTC
- ✅ Agendamento de teleconsultas
- ✅ Sala de espera virtual

**O que NÃO está em compliance:**
- ❌ Termo de consentimento específico para telemedicina
- ❌ Verificação de identidade bidirecional
- ❌ Registro de modalidade (presencial/tele) no prontuário
- ❌ Gravação opcional de consultas (com consentimento)
- ❌ Validação de primeiro atendimento
- ❌ Documentação legal completa

### ⏳ O que precisa ser implementado (100%)

1. **Termo de Consentimento Específico** (20% do trabalho)
   - Consentimento informado para teleconsulta
   - Explicação de limitações da telemedicina
   - Armazenamento com timestamp e aceite digital

2. **Identificação Bidirecional** (30% do trabalho)
   - Verificação de identidade do médico (foto, CRM visível)
   - Verificação de identidade do paciente (documento, selfie opcional)
   - Armazenamento seguro de comprovantes

3. **Prontuário de Teleconsulta** (15% do trabalho)
   - Campo "Modalidade" (Presencial/Teleconsulta)
   - Marcação automática de teleconsultas
   - Campos adicionais CFM 2.314

4. **Gravação de Consultas (Opcional)** (25% do trabalho)
   - Opção de gravar teleconsulta (com consentimento)
   - Armazenamento criptografado
   - Retenção por 20 anos

5. **Validação de Primeiro Atendimento** (10% do trabalho)
   - Verificar se já houve atendimento presencial
   - Alertas e exceções (áreas remotas, emergências)

## 🎯 Objetivos da Tarefa

Implementar compliance completo com CFM 2.314/2022 no sistema de telemedicina existente, garantindo que todas as teleconsultas sejam legais e documentadas conforme exigências do Conselho Federal de Medicina.

## 📝 Tarefas Detalhadas

### 1. Termo de Consentimento Específico (1 semana)

#### 1.1 Modelagem de Dados
```csharp
// Consentimento de Telemedicina
public class TelemedicineConsent
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    
    // Termo
    public string ConsentVersion { get; set; } // Versão do termo (para auditoria)
    public string ConsentText { get; set; } // Texto completo apresentado
    
    // Aceite
    public bool Accepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string DigitalSignature { get; set; } // Hash ou assinatura digital
    
    // Limitações Explicadas
    public bool UnderstoodLimitations { get; set; }
    public bool AgreesToEmergencyProtocol { get; set; }
    public bool AgreesToDataPrivacy { get; set; }
    
    // Dados Específicos da Consulta
    public int? AppointmentId { get; set; }
    public int? TelemedicineSessionId { get; set; }
    
    // Revogação (se aplicável)
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string RevocationReason { get; set; }
    
    // Navegação
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public Clinic Clinic { get; set; }
    public Appointment Appointment { get; set; }
}

// Adicionar ao Appointment
public class Appointment
{
    // ... campos existentes ...
    
    public AppointmentModality Modality { get; set; } = AppointmentModality.InPerson;
    public int? TelemedicineConsentId { get; set; }
    public TelemedicineConsent TelemedicineConsent { get; set; }
}

public enum AppointmentModality
{
    InPerson,       // Presencial
    Telemedicine,   // Teleconsulta
    Hybrid          // Híbrido (parte presencial, parte tele)
}
```

#### 1.2 Texto do Termo de Consentimento
```csharp
public class TelemedicineConsentTextProvider
{
    public const string CURRENT_VERSION = "1.0";
    
    public static string GetConsentText()
    {
        return @"
TERMO DE CONSENTIMENTO INFORMADO PARA TELEMEDICINA

Eu, [NOME DO PACIENTE], CPF [CPF], declaro estar ciente e de acordo com as seguintes informações sobre o atendimento por TELEMEDICINA:

1. DEFINIÇÃO
A telemedicina é o exercício da medicina mediado por tecnologias para fins de assistência, educação, pesquisa, prevenção de doenças e promoção de saúde, conforme Resolução CFM nº 2.314/2022.

2. LIMITAÇÕES DA TELEMEDICINA
- O atendimento por telemedicina possui limitações em relação ao atendimento presencial
- Não é possível realizar exame físico completo
- A qualidade da consulta depende da conexão de internet e equipamentos
- Em casos de emergência, devo buscar atendimento presencial imediato

3. IDENTIFICAÇÃO
- Compreendo que o médico e eu devemos nos identificar mutuamente antes da consulta
- Posso ser solicitado a apresentar documento de identificação com foto
- O médico apresentará seu CRM e identificação profissional

4. PRONTUÁRIO E PRIVACIDADE
- Todas as informações da teleconsulta serão registradas em prontuário eletrônico
- A consulta pode ser gravada (com meu consentimento adicional) para fins de documentação
- Meus dados serão protegidos conforme LGPD e sigilo médico

5. PRESCRIÇÕES E DOCUMENTOS
- Prescrições médicas e atestados serão fornecidos digitalmente com assinatura eletrônica
- Esses documentos têm validade legal

6. EMERGÊNCIAS
- Em caso de emergência médica durante ou após a teleconsulta, devo:
  * Ligar imediatamente para 192 (SAMU) ou 193 (Bombeiros)
  * Buscar atendimento presencial no hospital mais próximo
  * Informar o médico assistente assim que possível

7. CUSTOS
- Estou ciente dos custos da teleconsulta
- A cobertura por convênio (se aplicável) segue as regras da operadora

8. CONSENTIMENTO
- Consinto voluntariamente em ser atendido por telemedicina
- Fui informado sobre as alternativas de atendimento presencial
- Posso revogar este consentimento a qualquer momento

Data: [DATA]
IP: [IP_ADDRESS]

Assinatura Digital do Paciente (aceite eletrônico)
";
    }
}
```

#### 1.3 Serviço de Consentimento
```csharp
public interface ITelemedicineConsentService
{
    Task<TelemedicineConsent> CreateConsentAsync(CreateConsentDto dto);
    Task<TelemedicineConsent> GetActiveConsentAsync(int patientId, int doctorId);
    Task<bool> HasValidConsentAsync(int patientId, int doctorId, int clinicId);
    Task<TelemedicineConsent> RevokeConsentAsync(int consentId, string reason);
}

public class TelemedicineConsentService : ITelemedicineConsentService
{
    private readonly ITelemedicineConsentRepository _repository;
    private readonly ILogger<TelemedicineConsentService> _logger;
    
    public async Task<TelemedicineConsent> CreateConsentAsync(CreateConsentDto dto)
    {
        // Validar
        if (!dto.Accepted)
            throw new InvalidOperationException("Consentimento deve ser aceito para continuar");
        
        if (!dto.UnderstoodLimitations || !dto.AgreesToEmergencyProtocol || !dto.AgreesToDataPrivacy)
            throw new InvalidOperationException("Todas as condições devem ser aceitas");
        
        // Verificar se já existe consentimento ativo
        var existing = await _repository.GetActiveConsentAsync(dto.PatientId, dto.DoctorId, dto.ClinicId);
        
        if (existing != null)
        {
            _logger.LogInformation($"Consentimento já existe: Paciente {dto.PatientId}, Médico {dto.DoctorId}");
            return existing;
        }
        
        // Criar novo consentimento
        var consent = new TelemedicineConsent
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ClinicId = dto.ClinicId,
            
            ConsentVersion = TelemedicineConsentTextProvider.CURRENT_VERSION,
            ConsentText = TelemedicineConsentTextProvider.GetConsentText(),
            
            Accepted = dto.Accepted,
            AcceptedAt = DateTime.UtcNow,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
            DigitalSignature = GenerateDigitalSignature(dto),
            
            UnderstoodLimitations = dto.UnderstoodLimitations,
            AgreesToEmergencyProtocol = dto.AgreesToEmergencyProtocol,
            AgreesToDataPrivacy = dto.AgreesToDataPrivacy,
            
            AppointmentId = dto.AppointmentId,
            
            IsRevoked = false
        };
        
        await _repository.AddAsync(consent);
        
        _logger.LogInformation($"Consentimento de telemedicina criado: Paciente {dto.PatientId}, Médico {dto.DoctorId}");
        
        return consent;
    }
    
    public async Task<bool> HasValidConsentAsync(int patientId, int doctorId, int clinicId)
    {
        var consent = await _repository.GetActiveConsentAsync(patientId, doctorId, clinicId);
        
        return consent != null && !consent.IsRevoked;
    }
    
    public async Task<TelemedicineConsent> RevokeConsentAsync(int consentId, string reason)
    {
        var consent = await _repository.GetByIdAsync(consentId);
        
        if (consent == null)
            throw new NotFoundException($"Consentimento {consentId} não encontrado");
        
        if (consent.IsRevoked)
            throw new InvalidOperationException("Consentimento já foi revogado");
        
        consent.IsRevoked = true;
        consent.RevokedAt = DateTime.UtcNow;
        consent.RevocationReason = reason;
        
        await _repository.UpdateAsync(consent);
        
        return consent;
    }
    
    private string GenerateDigitalSignature(CreateConsentDto dto)
    {
        // Gerar hash SHA-256 dos dados do consentimento
        var data = $"{dto.PatientId}|{dto.DoctorId}|{dto.ClinicId}|{DateTime.UtcNow:O}|{dto.IpAddress}";
        
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
```

#### 1.4 Endpoints API
```csharp
[ApiController]
[Route("api/telemedicine/consent")]
public class TelemedicineConsentController : ControllerBase
{
    private readonly ITelemedicineConsentService _consentService;
    
    [HttpPost]
    public async Task<IActionResult> CreateConsent([FromBody] CreateConsentDto dto)
    {
        dto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        dto.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        var consent = await _consentService.CreateConsentAsync(dto);
        return Created($"/api/telemedicine/consent/{consent.Id}", consent);
    }
    
    [HttpGet("check")]
    public async Task<IActionResult> CheckConsent([FromQuery] int patientId, [FromQuery] int doctorId, [FromQuery] int clinicId)
    {
        var hasConsent = await _consentService.HasValidConsentAsync(patientId, doctorId, clinicId);
        return Ok(new { hasConsent });
    }
    
    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> RevokeConsent(int id, [FromBody] RevokeConsentDto dto)
    {
        var consent = await _consentService.RevokeConsentAsync(id, dto.Reason);
        return Ok(consent);
    }
    
    [HttpGet("text")]
    public IActionResult GetConsentText()
    {
        var text = TelemedicineConsentTextProvider.GetConsentText();
        return Ok(new { text, version = TelemedicineConsentTextProvider.CURRENT_VERSION });
    }
}
```

### 2. Identificação Bidirecional (2 semanas)

#### 2.1 Modelagem de Verificação de Identidade
```csharp
public class IdentityVerification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserType { get; set; } // "Doctor", "Patient"
    
    // Documento de Identificação
    public string DocumentType { get; set; } // "RG", "CPF", "CNH", "Passaporte"
    public string DocumentNumber { get; set; }
    public string DocumentPhotoPath { get; set; } // Foto do documento
    
    // Selfie de Verificação
    public string SelfiePath { get; set; }
    
    // Verificação para Médicos
    public string CrmCardPhotoPath { get; set; } // Foto da carteira do CRM
    
    // Status da Verificação
    public VerificationStatus Status { get; set; }
    public DateTime VerifiedAt { get; set; }
    public int? VerifiedByUserId { get; set; }
    public string VerificationNotes { get; set; }
    
    // Sessão de Telemedicina
    public int? TelemedicineSessionId { get; set; }
    
    // Validade (verificação pode expirar)
    public DateTime ValidUntil { get; set; }
    
    // Navegação
    public User User { get; set; }
    public User VerifiedBy { get; set; }
}

public enum VerificationStatus
{
    Pending,        // Aguardando verificação
    Verified,       // Verificado
    Rejected,       // Rejeitado
    Expired         // Expirado
}

// Adicionar ao TelemedicineSession
public class TelemedicineSession
{
    // ... campos existentes ...
    
    public int? DoctorVerificationId { get; set; }
    public int? PatientVerificationId { get; set; }
    
    public IdentityVerification DoctorVerification { get; set; }
    public IdentityVerification PatientVerification { get; set; }
    
    public bool IsIdentityVerified => 
        DoctorVerification?.Status == VerificationStatus.Verified &&
        PatientVerification?.Status == VerificationStatus.Verified;
}
```

#### 2.2 Serviço de Verificação
```csharp
public interface IIdentityVerificationService
{
    Task<IdentityVerification> CreateVerificationAsync(CreateVerificationDto dto);
    Task<IdentityVerification> VerifyIdentityAsync(int verificationId, int verifiedByUserId, bool approved, string notes = null);
    Task<bool> IsIdentityVerifiedAsync(int userId, string userType);
    Task<IdentityVerification> GetLatestVerificationAsync(int userId, string userType);
}

public class IdentityVerificationService : IIdentityVerificationService
{
    private readonly IIdentityVerificationRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<IdentityVerificationService> _logger;
    
    public async Task<IdentityVerification> CreateVerificationAsync(CreateVerificationDto dto)
    {
        // Validar arquivos obrigatórios
        if (dto.DocumentPhoto == null)
            throw new ValidationException("Foto do documento é obrigatória");
        
        if (dto.UserType == "Doctor" && dto.CrmCardPhoto == null)
            throw new ValidationException("Foto da carteira do CRM é obrigatória para médicos");
        
        // Salvar arquivos
        var documentPhotoPath = await _fileStorage.SaveAsync(
            dto.DocumentPhoto, 
            "identity-verifications",
            $"document_{dto.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}"
        );
        
        string selfiePath = null;
        if (dto.Selfie != null)
        {
            selfiePath = await _fileStorage.SaveAsync(
                dto.Selfie,
                "identity-verifications",
                $"selfie_{dto.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}"
            );
        }
        
        string crmCardPhotoPath = null;
        if (dto.CrmCardPhoto != null)
        {
            crmCardPhotoPath = await _fileStorage.SaveAsync(
                dto.CrmCardPhoto,
                "identity-verifications",
                $"crm_{dto.UserId}_{DateTime.UtcNow:yyyyMMddHHmmss}"
            );
        }
        
        // Criar verificação
        var verification = new IdentityVerification
        {
            UserId = dto.UserId,
            UserType = dto.UserType,
            
            DocumentType = dto.DocumentType,
            DocumentNumber = dto.DocumentNumber,
            DocumentPhotoPath = documentPhotoPath,
            
            SelfiePath = selfiePath,
            CrmCardPhotoPath = crmCardPhotoPath,
            
            Status = VerificationStatus.Pending,
            VerifiedAt = DateTime.UtcNow,
            
            TelemedicineSessionId = dto.TelemedicineSessionId,
            
            // Validade de 1 ano
            ValidUntil = DateTime.UtcNow.AddYears(1)
        };
        
        await _repository.AddAsync(verification);
        
        _logger.LogInformation($"Verificação de identidade criada: User {dto.UserId}, Type {dto.UserType}");
        
        return verification;
    }
    
    public async Task<IdentityVerification> VerifyIdentityAsync(int verificationId, int verifiedByUserId, bool approved, string notes = null)
    {
        var verification = await _repository.GetByIdAsync(verificationId);
        
        if (verification == null)
            throw new NotFoundException($"Verificação {verificationId} não encontrada");
        
        if (verification.Status != VerificationStatus.Pending)
            throw new InvalidOperationException("Verificação já foi processada");
        
        verification.Status = approved ? VerificationStatus.Verified : VerificationStatus.Rejected;
        verification.VerifiedByUserId = verifiedByUserId;
        verification.VerificationNotes = notes;
        
        await _repository.UpdateAsync(verification);
        
        return verification;
    }
    
    public async Task<bool> IsIdentityVerifiedAsync(int userId, string userType)
    {
        var verification = await _repository.GetLatestVerificationAsync(userId, userType);
        
        return verification != null &&
               verification.Status == VerificationStatus.Verified &&
               verification.ValidUntil > DateTime.UtcNow;
    }
}
```

#### 2.3 Validação Antes de Iniciar Teleconsulta
```csharp
public class TelemedicineSessionService
{
    public async Task<TelemedicineSession> StartSessionAsync(int appointmentId, int doctorId, int patientId)
    {
        // Validar consentimento
        var hasConsent = await _consentService.HasValidConsentAsync(patientId, doctorId, appointmentId);
        
        if (!hasConsent)
            throw new InvalidOperationException("Consentimento de telemedicina não encontrado ou inválido");
        
        // Validar identificação do médico
        var isDoctorVerified = await _verificationService.IsIdentityVerifiedAsync(doctorId, "Doctor");
        
        if (!isDoctorVerified)
            throw new InvalidOperationException("Identidade do médico não verificada");
        
        // Validar identificação do paciente
        var isPatientVerified = await _verificationService.IsIdentityVerifiedAsync(patientId, "Patient");
        
        if (!isPatientVerified)
            throw new InvalidOperationException("Identidade do paciente não verificada");
        
        // Criar sessão
        var session = new TelemedicineSession
        {
            AppointmentId = appointmentId,
            DoctorId = doctorId,
            PatientId = patientId,
            StartedAt = DateTime.UtcNow,
            Status = SessionStatus.Active,
            // ... outros campos
        };
        
        await _sessionRepository.AddAsync(session);
        
        return session;
    }
}
```

### 3. Prontuário de Teleconsulta (1 semana)

#### 3.1 Atualizar Entidade MedicalRecord
```csharp
public class MedicalRecord
{
    // ... campos existentes ...
    
    // Telemedicina
    public AppointmentModality Modality { get; set; } = AppointmentModality.InPerson;
    public int? TelemedicineSessionId { get; set; }
    public TelemedicineSession TelemedicineSession { get; set; }
    
    // Campos específicos CFM 2.314
    public string ConnectionQuality { get; set; } // "Excellent", "Good", "Fair", "Poor"
    public string TechnicalIssues { get; set; } // Registrar problemas técnicos se houver
    public bool PatientConsentedToRecording { get; set; }
    public string RecordingPath { get; set; }
}
```

#### 3.2 Serviço de Prontuário
```csharp
public class MedicalRecordService
{
    public async Task<MedicalRecord> CreateTelemedicineRecordAsync(CreateTelemedicineRecordDto dto)
    {
        // Validar sessão de telemedicina
        var session = await _sessionRepository.GetByIdAsync(dto.TelemedicineSessionId);
        
        if (session == null)
            throw new NotFoundException("Sessão de telemedicina não encontrada");
        
        if (!session.IsIdentityVerified)
            throw new InvalidOperationException("Identidades não verificadas para esta sessão");
        
        // Criar prontuário
        var record = new MedicalRecord
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            ClinicId = dto.ClinicId,
            AppointmentId = dto.AppointmentId,
            
            Modality = AppointmentModality.Telemedicine,
            TelemedicineSessionId = dto.TelemedicineSessionId,
            
            ConnectionQuality = dto.ConnectionQuality,
            TechnicalIssues = dto.TechnicalIssues,
            PatientConsentedToRecording = dto.ConsentedToRecording,
            
            // ... outros campos padrão do prontuário
        };
        
        await _repository.AddAsync(record);
        
        return record;
    }
}
```

### 4. Gravação de Consultas (2 semanas)

#### 4.1 Modelagem
```csharp
public class TelemedicineRecording
{
    public int Id { get; set; }
    public int TelemedicineSessionId { get; set; }
    
    public string RecordingPath { get; set; }
    public string EncryptionKey { get; set; } // Chave de criptografia
    public long FileSizeBytes { get; set; }
    public TimeSpan Duration { get; set; }
    
    public DateTime RecordedAt { get; set; }
    public DateTime RecordingStarted { get; set; }
    public DateTime RecordingEnded { get; set; }
    
    // Consentimento
    public bool PatientConsented { get; set; }
    public bool DoctorConsented { get; set; }
    public DateTime ConsentObtainedAt { get; set; }
    
    // Retenção (20 anos conforme CFM)
    public DateTime ExpiresAt { get; set; }
    
    // Acesso
    public int AccessCount { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Navegação
    public TelemedicineSession Session { get; set; }
}
```

#### 4.2 Serviço de Gravação
```csharp
public interface ITelemedicineRecordingService
{
    Task<TelemedicineRecording> StartRecordingAsync(int sessionId);
    Task<TelemedicineRecording> StopRecordingAsync(int recordingId);
    Task<Stream> GetRecordingAsync(int recordingId, int requestingUserId);
    Task<List<RecordingAccessLog>> GetAccessLogsAsync(int recordingId);
}

public class TelemedicineRecordingService : ITelemedicineRecordingService
{
    public async Task<TelemedicineRecording> StartRecordingAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdWithDetailsAsync(sessionId);
        
        if (session == null)
            throw new NotFoundException("Sessão não encontrada");
        
        // Verificar consentimento de AMBOS (médico e paciente)
        if (!session.PatientConsentedToRecording || !session.DoctorConsentedToRecording)
            throw new InvalidOperationException("Consentimento de gravação necessário de ambas as partes");
        
        // Gerar chave de criptografia
        var encryptionKey = GenerateEncryptionKey();
        
        var recording = new TelemedicineRecording
        {
            TelemedicineSessionId = sessionId,
            RecordingStarted = DateTime.UtcNow,
            PatientConsented = true,
            DoctorConsented = true,
            ConsentObtainedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(20), // CFM: 20 anos
            EncryptionKey = encryptionKey,
            AccessCount = 0
        };
        
        await _repository.AddAsync(recording);
        
        // Iniciar gravação via WebRTC/Media Server
        await _mediaServerClient.StartRecordingAsync(sessionId, recording.Id);
        
        return recording;
    }
    
    public async Task<Stream> GetRecordingAsync(int recordingId, int requestingUserId)
    {
        var recording = await _repository.GetByIdWithDetailsAsync(recordingId);
        
        if (recording == null)
            throw new NotFoundException("Gravação não encontrada");
        
        // Verificar permissão de acesso
        if (!await CanAccessRecordingAsync(requestingUserId, recording))
            throw new UnauthorizedAccessException("Sem permissão para acessar esta gravação");
        
        // Registrar acesso
        recording.AccessCount++;
        recording.LastAccessedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(recording);
        
        await LogAccessAsync(recordingId, requestingUserId);
        
        // Descriptografar e retornar
        var encryptedStream = await _fileStorage.GetStreamAsync(recording.RecordingPath);
        var decryptedStream = await _encryptionService.DecryptStreamAsync(encryptedStream, recording.EncryptionKey);
        
        return decryptedStream;
    }
    
    private async Task<bool> CanAccessRecordingAsync(int userId, TelemedicineRecording recording)
    {
        var session = recording.Session;
        
        // Apenas médico e paciente da consulta podem acessar
        return userId == session.DoctorId || userId == session.PatientId;
    }
}
```

### 5. Frontend - Modal de Consentimento (1 semana)

#### 5.1 Componente de Consentimento
```typescript
// telemedicine-consent-modal.component.ts
export class TelemedicineConsentModalComponent implements OnInit {
  @Input() patientId: number;
  @Input() doctorId: number;
  @Input() clinicId: number;
  @Input() appointmentId: number;
  
  consentText: string;
  consentForm = new FormGroup({
    understoodLimitations: new FormControl(false, Validators.requiredTrue),
    agreesToEmergencyProtocol: new FormControl(false, Validators.requiredTrue),
    agreesToDataPrivacy: new FormControl(false, Validators.requiredTrue),
    accepted: new FormControl(false, Validators.requiredTrue)
  });
  
  async ngOnInit() {
    const response = await this.telemedicineService.getConsentText();
    this.consentText = response.text;
  }
  
  async submitConsent() {
    if (!this.consentForm.valid) {
      this.toastr.warning('Por favor, aceite todos os termos para continuar');
      return;
    }
    
    const dto: CreateConsentDto = {
      patientId: this.patientId,
      doctorId: this.doctorId,
      clinicId: this.clinicId,
      appointmentId: this.appointmentId,
      ...this.consentForm.value
    };
    
    try {
      await this.telemedicineService.createConsent(dto);
      this.toastr.success('Consentimento registrado com sucesso');
      this.dialogRef.close(true);
    } catch (error) {
      this.toastr.error('Erro ao registrar consentimento: ' + error.message);
    }
  }
}
```

```html
<!-- telemedicine-consent-modal.component.html -->
<h2 mat-dialog-title>Consentimento para Telemedicina</h2>

<mat-dialog-content>
  <div class="consent-text-container">
    <pre class="consent-text">{{consentText}}</pre>
  </div>
  
  <form [formGroup]="consentForm">
    <mat-checkbox formControlName="understoodLimitations">
      <strong>Compreendo as limitações da telemedicina</strong> em relação ao atendimento presencial
    </mat-checkbox>
    
    <mat-checkbox formControlName="agreesToEmergencyProtocol">
      <strong>Concordo com o protocolo de emergências</strong> e sei que devo buscar atendimento presencial em casos urgentes
    </mat-checkbox>
    
    <mat-checkbox formControlName="agreesToDataPrivacy">
      <strong>Concordo com a política de privacidade</strong> e proteção de dados (LGPD)
    </mat-checkbox>
    
    <mat-divider></mat-divider>
    
    <mat-checkbox formControlName="accepted">
      <strong>Li e aceito os termos acima</strong> e consinto voluntariamente em ser atendido por telemedicina
    </mat-checkbox>
  </form>
</mat-dialog-content>

<mat-dialog-actions align="end">
  <button mat-button (click)="dialogRef.close(false)">Cancelar</button>
  <button mat-raised-button color="primary" 
          (click)="submitConsent()"
          [disabled]="!consentForm.valid">
    Aceitar e Continuar
  </button>
</mat-dialog-actions>
```

#### 5.2 Componente de Verificação de Identidade
```typescript
// identity-verification.component.ts
export class IdentityVerificationComponent {
  @Input() userId: number;
  @Input() userType: 'Doctor' | 'Patient';
  
  verificationForm = new FormGroup({
    documentType: new FormControl('RG', Validators.required),
    documentNumber: new FormControl('', Validators.required),
    documentPhoto: new FormControl(null, Validators.required),
    selfie: new FormControl(null),
    crmCardPhoto: new FormControl(null) // Apenas para médicos
  });
  
  documentPhotoPreview: string;
  selfiePreview: string;
  crmCardPhotoPreview: string;
  
  onDocumentPhotoSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files[0];
    this.verificationForm.patchValue({ documentPhoto: file });
    
    // Preview
    const reader = new FileReader();
    reader.onload = () => {
      this.documentPhotoPreview = reader.result as string;
    };
    reader.readAsDataURL(file);
  }
  
  async submitVerification() {
    if (!this.verificationForm.valid) {
      this.toastr.warning('Preencha todos os campos obrigatórios');
      return;
    }
    
    // Criar FormData para upload
    const formData = new FormData();
    formData.append('userId', this.userId.toString());
    formData.append('userType', this.userType);
    formData.append('documentType', this.verificationForm.get('documentType').value);
    formData.append('documentNumber', this.verificationForm.get('documentNumber').value);
    formData.append('documentPhoto', this.verificationForm.get('documentPhoto').value);
    
    if (this.verificationForm.get('selfie').value) {
      formData.append('selfie', this.verificationForm.get('selfie').value);
    }
    
    if (this.userType === 'Doctor' && this.verificationForm.get('crmCardPhoto').value) {
      formData.append('crmCardPhoto', this.verificationForm.get('crmCardPhoto').value);
    }
    
    try {
      await this.identityService.createVerification(formData);
      this.toastr.success('Verificação enviada com sucesso! Aguarde aprovação.');
      this.dialogRef.close(true);
    } catch (error) {
      this.toastr.error('Erro ao enviar verificação: ' + error.message);
    }
  }
}
```

### 6. Validação de Primeiro Atendimento (1 semana)

```csharp
public class FirstAppointmentValidationService
{
    public async Task<FirstAppointmentValidationResult> ValidateAsync(int patientId, int doctorId, AppointmentModality modality)
    {
        if (modality != AppointmentModality.Telemedicine)
            return FirstAppointmentValidationResult.Allowed();
        
        // Verificar se já houve atendimento presencial
        var hasInPersonAppointment = await _appointmentRepository
            .HasInPersonAppointmentAsync(patientId, doctorId);
        
        if (hasInPersonAppointment)
            return FirstAppointmentValidationResult.Allowed();
        
        // Verificar exceções
        var patient = await _patientRepository.GetByIdAsync(patientId);
        
        // Exceção 1: Área remota
        if (IsRemoteArea(patient.ZipCode))
            return FirstAppointmentValidationResult.AllowedWithException("Área remota");
        
        // Exceção 2: Emergência (deve ser marcado explicitamente)
        // Isso seria validado no agendamento
        
        // Primeira consulta por telemedicina não recomendada
        return FirstAppointmentValidationResult.NotRecommended(
            "CFM 2.314: Recomenda-se que o primeiro atendimento seja presencial. " +
            "Exceções: áreas remotas, emergências ou impossibilidade de atendimento presencial."
        );
    }
}

public class FirstAppointmentValidationResult
{
    public bool IsAllowed { get; set; }
    public bool IsException { get; set; }
    public string Message { get; set; }
    public string ExceptionReason { get; set; }
    
    public static FirstAppointmentValidationResult Allowed() => new() { IsAllowed = true };
    public static FirstAppointmentValidationResult AllowedWithException(string reason) => 
        new() { IsAllowed = true, IsException = true, ExceptionReason = reason };
    public static FirstAppointmentValidationResult NotRecommended(string message) => 
        new() { IsAllowed = false, Message = message };
}
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] Termo de consentimento específico implementado
- [ ] Verificação de identidade bidirecional funcional
- [ ] Prontuário distingue presencial vs teleconsulta
- [ ] Gravação de consultas (opcional) implementada
- [ ] Validação de primeiro atendimento ativa
- [ ] Todos os dados armazenados com criptografia

### Funcionais
- [ ] 100% das teleconsultas têm consentimento registrado
- [ ] Identidade verificada em 100% das sessões
- [ ] Gravações acessíveis apenas por autorizados
- [ ] Alertas de primeira consulta funcionando
- [ ] Interface intuitiva e profissional

### Conformidade Legal (CFM 2.314/2022)
- [ ] ✅ Art. 3º - Consentimento informado registrado
- [ ] ✅ Art. 4º - Identificação bidirecional implementada
- [ ] ✅ Art. 9º - Prontuário diferenciado para teleconsulta
- [ ] ✅ Art. 12º - Gravação com consentimento (opcional)
- [ ] ✅ Primeiro atendimento validado e alertado
- [ ] ✅ Retenção de dados por 20+ anos

### Jurídico
- [ ] Revisão jurídica aprovada
- [ ] Termo de consentimento validado por advogado
- [ ] Política de privacidade atualizada
- [ ] Documentação legal completa

## 📦 Entregáveis

1. **Código Backend**
   - `TelemedicineConsent` entity
   - `IdentityVerification` entity
   - `TelemedicineRecording` entity
   - Serviços e repositórios
   - Validações CFM 2.314

2. **Código Frontend**
   - `TelemedicineConsentModalComponent`
   - `IdentityVerificationComponent`
   - Indicadores visuais de teleconsulta
   - Dashboard de compliance

3. **Documentação**
   - Guia de compliance CFM 2.314
   - Manual para médicos
   - Política de teleconsulta
   - FAQ legal

4. **Termo de Consentimento**
   - Texto legal revisado
   - Versões em PDF
   - Template personalizável por clínica

## 🔗 Dependências

### Pré-requisitos (✅ Completos)
- ✅ Microserviço de telemedicina criado
- ✅ MVP de videochamadas funcionando
- ✅ Sistema de agendamentos

### Dependências Externas
- Revisão jurídica (advogado especializado)
- Armazenamento seguro para gravações
- Servidor de mídia para gravação

### Tarefas Dependentes
- **Telemedicina MVP** - Base para compliance
- **Prescrições Digitais** - Prescrições em teleconsultas
- **Prontuário Eletrônico** - Registro de teleconsultas

## 🧪 Testes

### Testes Unitários
```csharp
[Fact]
public async Task StartSession_WithoutConsent_ShouldThrowException()
{
    // Arrange
    var sessionDto = CreateSessionDto();
    
    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => _sessionService.StartSessionAsync(sessionDto)
    );
}

[Fact]
public async Task CreateConsent_WithAllAccepted_ShouldSucceed()
{
    // Arrange
    var consentDto = CreateValidConsentDto();
    
    // Act
    var consent = await _consentService.CreateConsentAsync(consentDto);
    
    // Assert
    Assert.NotNull(consent);
    Assert.True(consent.Accepted);
    Assert.NotNull(consent.DigitalSignature);
}
```

### Testes de Integração
- Fluxo completo: consentimento → verificação → teleconsulta → gravação → prontuário
- Validação de primeiro atendimento
- Revogação de consentimento

### Testes E2E
- Paciente aceita termo → verifica identidade → médico verifica identidade → teleconsulta inicia
- Tentativa de iniciar sem consentimento (deve falhar)
- Tentativa de acessar gravação sem permissão (deve falhar)

## 📊 Métricas de Acompanhamento

### Durante Desenvolvimento
- Cobertura de testes: >80%
- Taxa de validação de consentimento: 100%
- Performance: modal de consentimento <2s

### Pós-Deploy
- Taxa de consentimento: meta 100%
- Taxa de verificação de identidade: meta 100%
- Teleconsultas com compliance: meta 100%
- Zero processos CFM por não-conformidade
- Satisfação de médicos: meta >8/10

## 🚨 Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Termo de consentimento inválido juridicamente | Baixa | Crítico | Revisão jurídica especializada |
| Resistência de médicos ao processo | Média | Alto | Treinamento, UX simples, enfatizar proteção legal |
| Problemas técnicos na gravação | Média | Médio | Testes extensivos, backup de gravações |
| Falha na verificação de identidade | Baixa | Alto | Múltiplas formas de verificação, manual se necessário |

## 📚 Referências

### Regulamentações
- [Resolução CFM nº 2.314/2022](https://www.in.gov.br/en/web/dou/-/resolucao-cfm-n-2.314-de-20-de-abril-de-2022-394984568) - Telemedicina
- [Resolução CFM nº 1.643/2002](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1643) - Prescrições Digitais
- [Lei 13.989/2020](http://www.planalto.gov.br/ccivil_03/_ato2019-2022/2020/lei/L13989.htm) - Telemedicina durante COVID-19
- [LGPD](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm) - Proteção de Dados

### Código Existente
- `telemedicine/` - Microserviço de telemedicina
- `src/MedicSoft.Api/Controllers/AppointmentsController.cs`
- `frontend/src/app/telemedicine/` - Componentes existentes

---

> **Próximo Passo:** Após concluir esta tarefa, seguir para **06-tiss-fase1-convenios.md**  
> **Última Atualização:** 23 de Janeiro de 2026
