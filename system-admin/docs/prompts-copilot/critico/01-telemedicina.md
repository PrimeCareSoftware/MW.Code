# 🎥 Prompt: Telemedicina / Teleconsulta

## 📊 Status
- **Prioridade**: 🔥🔥🔥 CRÍTICA
- **Progresso**: 80% (MVP implementado, falta compliance CFM 2.314)
- **Esforço**: 2-3 meses | 2 devs
- **Prazo**: Q2/2026

## 🎯 Contexto

Implementar sistema completo de telemedicina com compliance total à Resolução CFM 2.314/2022, permitindo videochamadas seguras entre médico e paciente com todos os requisitos regulatórios brasileiros.

## ✅ O que já existe

- ✅ Microserviço de telemedicina criado (80%)
- ✅ Estrutura básica implementada
- ✅ Integração inicial com frontend

## 🎯 O que falta implementar

### 1. Compliance CFM 2.314/2022

**Requisitos Obrigatórios:**
- [ ] Termo de consentimento específico para teleconsulta
- [ ] Registro de consentimento no prontuário
- [ ] Identificação inequívoca do médico (CRM + foto)
- [ ] Identificação do paciente (documento com foto)
- [ ] Guarda de gravação por 20 anos (se aplicável)
- [ ] Sigilo e segurança das informações (criptografia E2E)
- [ ] Infraestrutura tecnológica adequada
- [ ] Atestados e receitas com assinatura digital
- [ ] Primeiro atendimento presencial (com exceções)
- [ ] Registro detalhado no prontuário com modalidade

### 2. Sistema de Videochamada

**Opções de Tecnologia:**
- Daily.co (recomendado - R$ 0 até 10k minutos/mês)
- Jitsi Meet (open source, auto-hospedado)
- Twilio Video (pago, muito confiável)

**Funcionalidades Necessárias:**
- [ ] Qualidade HD adaptativa
- [ ] Sala de espera virtual
- [ ] Gravação opcional (com consentimento)
- [ ] Chat paralelo durante videochamada
- [ ] Compartilhamento de tela
- [ ] Controles de áudio/vídeo
- [ ] Indicador de qualidade de conexão
- [ ] Fallback para áudio se vídeo falhar

### 3. Agendamento de Teleconsulta

- [ ] Tipo de consulta "Teleconsulta" no agendamento
- [ ] Validação de equipamento antes da consulta
- [ ] Instruções para paciente (como acessar)
- [ ] Link único e seguro para cada consulta
- [ ] Notificações 24h e 1h antes
- [ ] Teste de câmera/microfone pré-consulta

### 4. Prontuário de Teleconsulta

- [ ] Campo "Modalidade" (Presencial/Teleconsulta)
- [ ] Registro automático de início/fim da chamada
- [ ] Captura de evidências (screenshots, se consentido)
- [ ] Registro de qualidade da conexão
- [ ] Anotações durante a chamada
- [ ] Integração com prontuário SOAP

### 5. Documentos e Prescrições Digitais

- [ ] Emissão de receitas durante teleconsulta
- [ ] Atestados médicos digitais
- [ ] Requisições de exames
- [ ] Assinatura digital ICP-Brasil
- [ ] Envio automático por email/WhatsApp

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// Entidades
public class TelemedicineAppointment : Entity
{
    public Guid AppointmentId { get; set; }
    public string VideoRoomId { get; set; }
    public string SecureAccessToken { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public TelemedicineStatus Status { get; set; }
    public string RecordingUrl { get; set; }  // Se gravação autorizada
    public bool PatientConsented { get; set; }
    public DateTime? ConsentDate { get; set; }
    public string ConsentIpAddress { get; set; }
    public ConnectionQuality ConnectionQuality { get; set; }
    public bool IsFirstAppointment { get; set; }  // Validação CFM
}

public class TelemedicineConsent : Entity
{
    public Guid PatientId { get; set; }
    public Guid AppointmentId { get; set; }
    public DateTime ConsentDate { get; set; }
    public string ConsentText { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public bool AcceptsRecording { get; set; }
    public bool AcceptsDataSharing { get; set; }
}

public enum TelemedicineStatus
{
    Scheduled,
    WaitingRoom,
    InProgress,
    Completed,
    Cancelled,
    TechnicalIssue
}

public enum ConnectionQuality
{
    Excellent,
    Good,
    Fair,
    Poor,
    Failed
}
```

### Camada de Aplicação (Application Layer)

```csharp
// Service Interface
public interface ITelemedicineService
{
    Task<TelemedicineAppointment> CreateTelemedicineAppointment(
        Guid appointmentId, 
        Guid patientId, 
        Guid doctorId);
    
    Task<string> GenerateSecureRoomLink(Guid appointmentId);
    
    Task<bool> ValidatePatientConsent(Guid patientId);
    
    Task RecordConsentAsync(TelemedicineConsent consent);
    
    Task<TelemedicineAppointment> StartSession(Guid appointmentId);
    
    Task<TelemedicineAppointment> EndSession(
        Guid appointmentId, 
        ConnectionQuality quality);
    
    Task<bool> ValidateFirstAppointmentRule(Guid patientId, Guid doctorId);
}

// DTOs
public record CreateTelemedicineAppointmentCommand(
    Guid AppointmentId,
    Guid PatientId,
    Guid DoctorId,
    DateTime ScheduledTime,
    bool IsFirstAppointment
);

public record TelemedicineConsentCommand(
    Guid PatientId,
    Guid AppointmentId,
    bool AcceptsTerms,
    bool AcceptsRecording,
    string IpAddress
);
```

### Camada de Infraestrutura (Infrastructure Layer)

```csharp
// Video Service Integration (Daily.co example)
public class DailyCoVideoService : IVideoService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public async Task<VideoRoom> CreateRoom(string roomName, VideoRoomOptions options)
    {
        // Integration with Daily.co API
        var request = new
        {
            name = roomName,
            privacy = "private",
            properties = new
            {
                enable_chat = true,
                enable_screenshare = true,
                enable_recording = options.EnableRecording ? "cloud" : "off",
                max_participants = 2,  // Médico + Paciente
                exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()
            }
        };
        
        var response = await _httpClient.PostAsJsonAsync("/rooms", request);
        return await response.Content.ReadFromJsonAsync<VideoRoom>();
    }
    
    public async Task<MeetingToken> CreateMeetingToken(
        string roomName, 
        string userId, 
        string userName,
        bool isModerator)
    {
        // Generate secure token for participant
    }
    
    public async Task<string> GetRecordingUrl(string roomName)
    {
        // Retrieve recording if authorized
    }
}
```

### Camada de API (API Layer)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TelemedicineController : ControllerBase
{
    private readonly ITelemedicineService _telemedicineService;
    
    [HttpPost("appointments")]
    public async Task<IActionResult> CreateTelemedicineAppointment(
        [FromBody] CreateTelemedicineAppointmentCommand command)
    {
        // Validate first appointment rule
        var isValid = await _telemedicineService
            .ValidateFirstAppointmentRule(command.PatientId, command.DoctorId);
        
        if (!isValid && command.IsFirstAppointment)
        {
            return BadRequest("CFM 2.314: Primeiro atendimento deve ser presencial");
        }
        
        var appointment = await _telemedicineService
            .CreateTelemedicineAppointment(
                command.AppointmentId, 
                command.PatientId, 
                command.DoctorId);
        
        return Ok(appointment);
    }
    
    [HttpPost("consent")]
    public async Task<IActionResult> RecordConsent(
        [FromBody] TelemedicineConsentCommand command)
    {
        var consent = new TelemedicineConsent
        {
            PatientId = command.PatientId,
            AppointmentId = command.AppointmentId,
            ConsentDate = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            AcceptsRecording = command.AcceptsRecording
        };
        
        await _telemedicineService.RecordConsentAsync(consent);
        return Ok();
    }
    
    [HttpPost("{appointmentId}/join")]
    public async Task<IActionResult> JoinSession(Guid appointmentId)
    {
        // Validate consent
        // Generate secure link
        // Return room details
    }
    
    [HttpPost("{appointmentId}/end")]
    public async Task<IActionResult> EndSession(
        Guid appointmentId,
        [FromBody] EndSessionCommand command)
    {
        var appointment = await _telemedicineService
            .EndSession(appointmentId, command.Quality);
        
        return Ok(appointment);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes Necessários

```typescript
// 1. Telemedicine Consent Component
@Component({
  selector: 'app-telemedicine-consent',
  template: `
    <h2>Termo de Consentimento para Teleconsulta</h2>
    <div class="consent-text">
      <!-- CFM 2.314 consent text -->
    </div>
    <mat-checkbox [(ngModel)]="acceptsTerms">
      Aceito os termos da teleconsulta
    </mat-checkbox>
    <mat-checkbox [(ngModel)]="acceptsRecording">
      Autorizo gravação da consulta (opcional)
    </mat-checkbox>
    <button (click)="submitConsent()">Confirmar</button>
  `
})
export class TelemedicineConsentComponent { }

// 2. Video Room Component
@Component({
  selector: 'app-video-room',
  template: `
    <div class="video-container">
      <div id="remote-video"></div>
      <div id="local-video"></div>
      <div class="controls">
        <button (click)="toggleAudio()">
          <mat-icon>{{ audioEnabled ? 'mic' : 'mic_off' }}</mat-icon>
        </button>
        <button (click)="toggleVideo()">
          <mat-icon>{{ videoEnabled ? 'videocam' : 'videocam_off' }}</mat-icon>
        </button>
        <button (click)="shareScreen()">
          <mat-icon>screen_share</mat-icon>
        </button>
        <button (click)="endCall()" class="end-call">
          <mat-icon>call_end</mat-icon>
        </button>
      </div>
      <div class="quality-indicator">
        Qualidade: {{ connectionQuality }}
      </div>
    </div>
  `
})
export class VideoRoomComponent implements OnInit, OnDestroy {
  private callFrame: any;
  
  async ngOnInit() {
    // Initialize Daily.co
    this.callFrame = DailyIframe.createFrame({
      showLeaveButton: false,
      iframeStyle: {
        width: '100%',
        height: '100%'
      }
    });
    
    // Join room with token
    await this.callFrame.join({
      url: this.roomUrl,
      token: this.accessToken
    });
    
    // Monitor connection quality
    this.callFrame.on('network-quality-change', this.handleQualityChange);
  }
  
  handleQualityChange(event: any) {
    this.connectionQuality = event.quality;
  }
}

// 3. Equipment Test Component
@Component({
  selector: 'app-equipment-test',
  template: `
    <h2>Teste seus Equipamentos</h2>
    <div class="test-container">
      <div class="camera-test">
        <video #videoPreview autoplay></video>
        <p>{{ cameraStatus }}</p>
      </div>
      <div class="microphone-test">
        <div class="audio-level"></div>
        <p>{{ microphoneStatus }}</p>
      </div>
      <div class="speaker-test">
        <button (click)="playTestSound()">Testar Som</button>
        <p>{{ speakerStatus }}</p>
      </div>
    </div>
    <button [disabled]="!allTestsPassed" (click)="proceedToCall()">
      Iniciar Teleconsulta
    </button>
  `
})
export class EquipmentTestComponent { }
```

## 📋 Checklist de Implementação

### Backend

- [ ] Criar entidades de domínio (TelemedicineAppointment, TelemedicineConsent)
- [ ] Implementar repositórios
- [ ] Criar serviços de aplicação
- [ ] Integrar com Daily.co (ou escolher outra plataforma)
- [ ] Implementar validações CFM 2.314
- [ ] Criar controllers REST
- [ ] Adicionar migrations
- [ ] Implementar testes unitários
- [ ] Implementar testes de integração

### Frontend

- [ ] Criar componente de consentimento
- [ ] Implementar teste de equipamentos
- [ ] Criar sala de espera virtual
- [ ] Integrar com Daily.co SDK
- [ ] Implementar controles de vídeo
- [ ] Criar indicador de qualidade
- [ ] Implementar chat paralelo
- [ ] Adicionar compartilhamento de tela
- [ ] Criar notificações pré-consulta

### Compliance e Documentação

- [ ] Termo de consentimento CFM 2.314
- [ ] Política de privacidade para teleconsulta
- [ ] Manual do usuário (médico e paciente)
- [ ] Processo de verificação de identidade
- [ ] Sistema de armazenamento de gravações (20 anos)
- [ ] Auditoria de acessos

## 🧪 Testes

### Testes Unitários
```csharp
public class TelemedicineServiceTests
{
    [Fact]
    public async Task ShouldCreateTelemedicineAppointment()
    {
        // Arrange
        var service = CreateService();
        var command = new CreateTelemedicineAppointmentCommand(...);
        
        // Act
        var result = await service.CreateTelemedicineAppointment(...);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.VideoRoomId);
    }
    
    [Fact]
    public async Task ShouldEnforceFirstAppointmentRule()
    {
        // Test CFM 2.314 rule
    }
    
    [Fact]
    public async Task ShouldRecordConsent()
    {
        // Test consent recording
    }
}
```

### Testes de Integração
- [ ] Testar criação de sala de vídeo
- [ ] Testar geração de tokens seguros
- [ ] Testar gravação (se habilitada)
- [ ] Testar qualidade de conexão
- [ ] Testar encerramento de sessão

## 📚 Referências

- [PENDING_TASKS.md - Seção Telemedicina](../../PENDING_TASKS.md#1-telemedicina--teleconsulta)
- [Resolução CFM 2.314/2022](https://www.in.gov.br/web/dou/-/resolucao-cfm-n-2.314-de-20-de-abril-de-2022-394965619)
- [Daily.co Documentation](https://docs.daily.co/)
- [TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md](../../TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md)

## 💰 Investimento

- **Desenvolvimento**: 2-3 meses, 2 devs
- **Custo**: R$ 91.5k
- **Infraestrutura**: Daily.co ~R$ 500/mês (após tier gratuito)
- **ROI Esperado**: Expansão geográfica, diferencial competitivo

## ✅ Critérios de Aceitação

1. ✅ Sistema permite agendamento de teleconsultas
2. ✅ Paciente pode dar consentimento digital antes da consulta
3. ✅ Médico e paciente podem se conectar por vídeo HD
4. ✅ Sistema valida regra de primeiro atendimento presencial
5. ✅ Qualidade de conexão é monitorada e registrada
6. ✅ Prontuário registra modalidade "Teleconsulta"
7. ✅ Gravações são armazenadas com segurança (se consentido)
8. ✅ Conformidade total com CFM 2.314/2022
9. ✅ Documentos digitais podem ser emitidos durante consulta
10. ✅ Testes de equipamento antes da consulta funcionam

---

**Última Atualização**: Janeiro 2026
**Status**: Pronto para desenvolvimento
**Próximo Passo**: Escolher plataforma de vídeo e iniciar implementação backend
