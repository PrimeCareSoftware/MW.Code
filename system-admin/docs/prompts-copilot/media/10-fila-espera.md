# 🎫 Prompt: Sistema de Fila de Espera com SignalR

## 📊 Status
- **Prioridade**: 🔥 MÉDIA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 2-3 meses | 2 devs
- **Prazo**: Q2/2026

## 🎯 Contexto

Implementar sistema completo de gerenciamento de fila de espera em tempo real para clínicas, com painel de chamada, totem de autoatendimento, notificações e integração com agendamento. Utiliza SignalR para comunicação real-time entre todos os componentes do sistema.

## 📋 Justificativa

### Problemas Atuais
- ❌ Recepção desorganizada e lotada
- ❌ Pacientes não sabem quanto tempo vão esperar
- ❌ Walk-ins misturados com agendados
- ❌ Reclamações sobre tempo de espera
- ❌ Sem priorização (idosos, gestantes, urgências)
- ❌ Falta de transparência no atendimento

### Benefícios
- ✅ Organização da recepção
- ✅ Redução de reclamações
- ✅ Transparência no atendimento
- ✅ Priorização automática
- ✅ Estimativa de tempo de espera
- ✅ Notificações em tempo real
- ✅ Métricas de performance
- ✅ Melhor experiência do paciente

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// src/Domain/Entities/QueueTicket.cs
public class QueueTicket : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public string TicketNumber { get; set; }  // Ex: "A001", "B015"
    public QueueType QueueType { get; set; }
    
    // Paciente
    public Guid? PatientId { get; set; }  // Null para walk-ins
    public string PatientName { get; set; }
    public string PatientCpf { get; set; }
    public string PatientPhone { get; set; }
    
    // Agendamento
    public Guid? AppointmentId { get; set; }  // Se veio de agendamento
    public DateTime? ScheduledTime { get; set; }
    
    // Prioridade
    public PriorityLevel Priority { get; set; }
    public string PriorityReason { get; set; }  // Ex: "Idoso", "Gestante"
    
    // Status
    public QueueStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Atendimento
    public Guid? DoctorId { get; set; }
    public Guid? RoomId { get; set; }
    public string RoomName { get; set; }
    
    // Tempo
    public int EstimatedWaitMinutes { get; set; }
    public int? ActualWaitMinutes { get; set; }
    public int PositionInQueue { get; set; }
    
    // Notificações
    public bool NotificationSent { get; set; }
    public DateTime? LastNotificationAt { get; set; }
    
    // Navigation
    public virtual Patient Patient { get; set; }
    public virtual Appointment Appointment { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual Room Room { get; set; }
}

public enum QueueType
{
    General,        // Fila geral
    Consultation,   // Consulta médica
    Exam,           // Exames
    Vaccination,    // Vacinação
    Pharmacy,       // Farmácia
    Reception       // Recepção/Cadastro
}

public enum QueueStatus
{
    Waiting,        // Aguardando
    Called,         // Chamado
    InProgress,     // Em atendimento
    Completed,      // Concluído
    Cancelled,      // Cancelado
    NoShow          // Não compareceu
}

public enum PriorityLevel
{
    Normal = 0,
    High = 1,       // Prioridade alta (idosos 60+, gestantes)
    Urgent = 2      // Urgência médica
}

// Value Objects
public class QueueStatistics : ValueObject
{
    public int TotalWaiting { get; private set; }
    public int TotalCalled { get; private set; }
    public int TotalInProgress { get; private set; }
    public int TotalCompleted { get; private set; }
    public double AverageWaitMinutes { get; private set; }
    public double AverageServiceMinutes { get; private set; }
    public int LongestWaitMinutes { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalWaiting;
        yield return AverageWaitMinutes;
    }
}

// src/Domain/Entities/QueueConfiguration.cs
public class QueueConfiguration : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    
    // Configurações de Senha
    public string TicketPrefix { get; set; }  // Ex: "A", "B", "C"
    public int NextTicketNumber { get; set; }
    public int TicketNumberLength { get; set; }  // Ex: 3 = "001"
    
    // Configurações de Tempo
    public int DefaultServiceTimeMinutes { get; set; }  // Tempo médio
    public int BufferTimeMinutes { get; set; }  // Tempo buffer entre atendimentos
    
    // Prioridades
    public bool EnablePriority { get; set; }
    public bool AutoDetectSeniors { get; set; }  // Detectar 60+ anos
    public bool AutoDetectPregnant { get; set; }
    
    // Notificações
    public bool SendSmsNotification { get; set; }
    public bool SendWhatsAppNotification { get; set; }
    public bool SendAppNotification { get; set; }
    public int NotifyBeforeMinutes { get; set; }  // Notificar X min antes
    
    // Painel
    public int DisplayTicketsCount { get; set; }  // Quantas senhas mostrar no painel
    public bool PlayAudioAlert { get; set; }
    public string AudioAlertMessage { get; set; }  // "Senha {number}, sala {room}"
}

// src/Domain/Entities/Room.cs
public class Room : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public string Name { get; set; }  // Ex: "Sala 1", "Consultório A"
    public string Number { get; set; }
    public RoomType Type { get; set; }
    public bool IsActive { get; set; }
    public Guid? CurrentDoctorId { get; set; }
    public Guid? CurrentTicketId { get; set; }
}

public enum RoomType
{
    Consultation,
    Exam,
    Vaccination,
    Procedure
}
```

### Camada de Aplicação (Application Layer)

```csharp
// src/Application/Services/IQueueService.cs
public interface IQueueService
{
    // Geração de Senha
    Task<QueueTicket> GenerateTicketAsync(GenerateTicketRequest request);
    Task<QueueTicket> GenerateTicketFromAppointmentAsync(Guid appointmentId);
    
    // Chamada
    Task<QueueTicket> CallNextTicketAsync(Guid roomId, Guid? doctorId = null);
    Task<QueueTicket> CallSpecificTicketAsync(Guid ticketId, Guid roomId);
    Task<QueueTicket> RecallTicketAsync(Guid ticketId);
    
    // Status
    Task StartServiceAsync(Guid ticketId);
    Task CompleteServiceAsync(Guid ticketId);
    Task CancelTicketAsync(Guid ticketId, string reason);
    Task MarkAsNoShowAsync(Guid ticketId);
    
    // Consultas
    Task<List<QueueTicket>> GetWaitingTicketsAsync(QueueType? type = null);
    Task<QueueTicket> GetTicketByNumberAsync(string ticketNumber);
    Task<QueueTicket> GetTicketByIdAsync(Guid ticketId);
    Task<int> GetPositionInQueueAsync(Guid ticketId);
    Task<int> GetEstimatedWaitTimeAsync(Guid ticketId);
    
    // Estatísticas
    Task<QueueStatistics> GetStatisticsAsync(DateTime date);
    Task<List<QueueTicket>> GetHistoryAsync(DateTime startDate, DateTime endDate);
    
    // Configuração
    Task<QueueConfiguration> GetConfigurationAsync();
    Task UpdateConfigurationAsync(QueueConfiguration config);
}

// src/Application/Services/QueueService.cs
public class QueueService : IQueueService
{
    private readonly IQueueTicketRepository _ticketRepository;
    private readonly IQueueConfigurationRepository _configRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IQueueHubService _hubService;
    private readonly INotificationService _notificationService;
    private readonly ITenantContext _tenantContext;
    
    public async Task<QueueTicket> GenerateTicketAsync(GenerateTicketRequest request)
    {
        var config = await _configRepository.GetByTenantAsync(_tenantContext.TenantId);
        var nextNumber = await GetNextTicketNumberAsync(config);
        
        // Detectar prioridade automática
        var priority = await DeterminePriorityAsync(request);
        
        var ticket = new QueueTicket
        {
            TenantId = _tenantContext.TenantId,
            TicketNumber = nextNumber,
            QueueType = request.QueueType,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            PatientCpf = request.PatientCpf,
            PatientPhone = request.PatientPhone,
            Priority = priority.Level,
            PriorityReason = priority.Reason,
            Status = QueueStatus.Waiting,
            CreatedAt = DateTime.UtcNow,
            EstimatedWaitMinutes = await CalculateEstimatedWaitAsync()
        };
        
        await _ticketRepository.AddAsync(ticket);
        
        // Atualizar posição na fila
        ticket.PositionInQueue = await GetPositionInQueueAsync(ticket.Id);
        await _ticketRepository.UpdateAsync(ticket);
        
        // Notificar painel em tempo real via SignalR
        await _hubService.NotifyNewTicketAsync(ticket);
        
        return ticket;
    }
    
    public async Task<QueueTicket> CallNextTicketAsync(Guid roomId, Guid? doctorId = null)
    {
        // Buscar próximo ticket (com prioridade)
        var nextTicket = await _ticketRepository.GetNextWaitingTicketAsync(
            _tenantContext.TenantId,
            considerPriority: true);
        
        if (nextTicket == null)
            throw new InvalidOperationException("Não há tickets aguardando");
        
        nextTicket.Status = QueueStatus.Called;
        nextTicket.CalledAt = DateTime.UtcNow;
        nextTicket.RoomId = roomId;
        nextTicket.DoctorId = doctorId;
        nextTicket.ActualWaitMinutes = (int)(DateTime.UtcNow - nextTicket.CreatedAt).TotalMinutes;
        
        await _ticketRepository.UpdateAsync(nextTicket);
        
        // Notificar painel via SignalR
        await _hubService.NotifyTicketCalledAsync(nextTicket);
        
        // Enviar notificação ao paciente (SMS/WhatsApp)
        if (!string.IsNullOrEmpty(nextTicket.PatientPhone))
        {
            await _notificationService.SendQueueNotificationAsync(
                nextTicket.PatientPhone,
                $"Sua senha {nextTicket.TicketNumber} foi chamada. " +
                $"Dirija-se à {nextTicket.RoomName}.");
        }
        
        // Áudio no painel
        await _hubService.PlayAudioAlertAsync(nextTicket);
        
        return nextTicket;
    }
    
    private async Task<(PriorityLevel Level, string Reason)> DeterminePriorityAsync(
        GenerateTicketRequest request)
    {
        var config = await _configRepository.GetByTenantAsync(_tenantContext.TenantId);
        
        if (!config.EnablePriority)
            return (PriorityLevel.Normal, null);
        
        // Urgência manual
        if (request.IsUrgent)
            return (PriorityLevel.Urgent, "Urgência médica");
        
        // Idosos (60+)
        if (config.AutoDetectSeniors && request.PatientId.HasValue)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId.Value);
            if (patient != null && patient.Age >= 60)
                return (PriorityLevel.High, "Idoso (60+ anos)");
        }
        
        // Gestantes (implementar lógica específica se necessário)
        if (config.AutoDetectPregnant && request.IsPregnant)
            return (PriorityLevel.High, "Gestante");
        
        return (PriorityLevel.Normal, null);
    }
    
    private async Task<int> CalculateEstimatedWaitAsync()
    {
        var config = await _configRepository.GetByTenantAsync(_tenantContext.TenantId);
        var waitingCount = await _ticketRepository.CountWaitingTicketsAsync(_tenantContext.TenantId);
        
        var avgServiceTime = config.DefaultServiceTimeMinutes;
        var estimatedWait = waitingCount * avgServiceTime;
        
        return estimatedWait;
    }
}

// DTOs
public class GenerateTicketRequest
{
    public QueueType QueueType { get; set; }
    public Guid? PatientId { get; set; }
    public string PatientName { get; set; }
    public string PatientCpf { get; set; }
    public string PatientPhone { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsPregnant { get; set; }
}
```

### SignalR Hub

```csharp
// src/Infrastructure/Hubs/QueueHub.cs
public class QueueHub : Hub
{
    private readonly IQueueService _queueService;
    private readonly ITenantContext _tenantContext;
    
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User.FindFirst("TenantId")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");
        }
        
        await base.OnConnectedAsync();
    }
    
    public async Task SubscribeToQueue(string queueType)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"queue_{queueType}");
    }
    
    public async Task<QueueTicket> CallNextTicket(Guid roomId)
    {
        var ticket = await _queueService.CallNextTicketAsync(roomId);
        return ticket;
    }
    
    public async Task<List<QueueTicket>> GetWaitingTickets()
    {
        return await _queueService.GetWaitingTicketsAsync();
    }
}

// src/Infrastructure/Services/QueueHubService.cs
public interface IQueueHubService
{
    Task NotifyNewTicketAsync(QueueTicket ticket);
    Task NotifyTicketCalledAsync(QueueTicket ticket);
    Task NotifyTicketCompletedAsync(QueueTicket ticket);
    Task UpdateQueueStatisticsAsync(QueueStatistics stats);
    Task PlayAudioAlertAsync(QueueTicket ticket);
}

public class QueueHubService : IQueueHubService
{
    private readonly IHubContext<QueueHub> _hubContext;
    
    public async Task NotifyNewTicketAsync(QueueTicket ticket)
    {
        await _hubContext.Clients
            .Group($"tenant_{ticket.TenantId}")
            .SendAsync("NewTicket", ticket);
    }
    
    public async Task NotifyTicketCalledAsync(QueueTicket ticket)
    {
        await _hubContext.Clients
            .Group($"tenant_{ticket.TenantId}")
            .SendAsync("TicketCalled", ticket);
    }
    
    public async Task PlayAudioAlertAsync(QueueTicket ticket)
    {
        var audioMessage = new
        {
            TicketNumber = ticket.TicketNumber,
            RoomName = ticket.RoomName,
            Message = $"Senha {ticket.TicketNumber}, sala {ticket.RoomName}"
        };
        
        await _hubContext.Clients
            .Group($"tenant_{ticket.TenantId}")
            .SendAsync("PlayAudio", audioMessage);
    }
}
```

### Camada de API (API Layer)

```csharp
// src/API/Controllers/QueueController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;
    
    [HttpPost("tickets")]
    public async Task<ActionResult<QueueTicket>> GenerateTicket(
        [FromBody] GenerateTicketRequest request)
    {
        var ticket = await _queueService.GenerateTicketAsync(request);
        return Ok(ticket);
    }
    
    [HttpPost("tickets/from-appointment/{appointmentId}")]
    public async Task<ActionResult<QueueTicket>> GenerateFromAppointment(Guid appointmentId)
    {
        var ticket = await _queueService.GenerateTicketFromAppointmentAsync(appointmentId);
        return Ok(ticket);
    }
    
    [HttpPost("call-next")]
    public async Task<ActionResult<QueueTicket>> CallNext(
        [FromBody] CallNextRequest request)
    {
        var ticket = await _queueService.CallNextTicketAsync(
            request.RoomId,
            request.DoctorId);
        return Ok(ticket);
    }
    
    [HttpPost("tickets/{ticketId}/call")]
    public async Task<ActionResult<QueueTicket>> CallSpecific(
        Guid ticketId,
        [FromBody] CallTicketRequest request)
    {
        var ticket = await _queueService.CallSpecificTicketAsync(ticketId, request.RoomId);
        return Ok(ticket);
    }
    
    [HttpPost("tickets/{ticketId}/recall")]
    public async Task<ActionResult<QueueTicket>> Recall(Guid ticketId)
    {
        var ticket = await _queueService.RecallTicketAsync(ticketId);
        return Ok(ticket);
    }
    
    [HttpPost("tickets/{ticketId}/start")]
    public async Task<ActionResult> StartService(Guid ticketId)
    {
        await _queueService.StartServiceAsync(ticketId);
        return Ok();
    }
    
    [HttpPost("tickets/{ticketId}/complete")]
    public async Task<ActionResult> CompleteService(Guid ticketId)
    {
        await _queueService.CompleteServiceAsync(ticketId);
        return Ok();
    }
    
    [HttpGet("tickets/waiting")]
    public async Task<ActionResult<List<QueueTicket>>> GetWaiting(
        [FromQuery] QueueType? type = null)
    {
        var tickets = await _queueService.GetWaitingTicketsAsync(type);
        return Ok(tickets);
    }
    
    [HttpGet("tickets/{ticketNumber}/by-number")]
    public async Task<ActionResult<QueueTicket>> GetByNumber(string ticketNumber)
    {
        var ticket = await _queueService.GetTicketByNumberAsync(ticketNumber);
        return Ok(ticket);
    }
    
    [HttpGet("statistics")]
    public async Task<ActionResult<QueueStatistics>> GetStatistics(
        [FromQuery] DateTime? date = null)
    {
        var stats = await _queueService.GetStatisticsAsync(date ?? DateTime.Today);
        return Ok(stats);
    }
    
    [HttpGet("configuration")]
    public async Task<ActionResult<QueueConfiguration>> GetConfiguration()
    {
        var config = await _queueService.GetConfigurationAsync();
        return Ok(config);
    }
    
    [HttpPut("configuration")]
    public async Task<ActionResult> UpdateConfiguration(
        [FromBody] QueueConfiguration config)
    {
        await _queueService.UpdateConfigurationAsync(config);
        return Ok();
    }
}
```

## 🎨 Frontend (Angular)

### Componentes

```typescript
// src/app/features/queue/totem/totem.component.ts
@Component({
  selector: 'app-queue-totem',
  template: `
    <div class="totem-container">
      <div class="totem-header">
        <h1>Bem-vindo à Clínica</h1>
        <p>Retire sua senha de atendimento</p>
      </div>
      
      <div class="totem-body">
        <div class="queue-type-selection">
          <button mat-raised-button color="primary" class="queue-button"
                  (click)="generateTicket('Consultation')">
            <mat-icon>medical_services</mat-icon>
            <span>Consulta Médica</span>
          </button>
          
          <button mat-raised-button color="primary" class="queue-button"
                  (click)="generateTicket('Exam')">
            <mat-icon>assignment</mat-icon>
            <span>Exames</span>
          </button>
          
          <button mat-raised-button color="primary" class="queue-button"
                  (click)="generateTicket('Vaccination')">
            <mat-icon>vaccines</mat-icon>
            <span>Vacinação</span>
          </button>
          
          <button mat-raised-button color="primary" class="queue-button"
                  (click)="generateTicket('Reception')">
            <mat-icon>person</mat-icon>
            <span>Recepção</span>
          </button>
        </div>
        
        <div *ngIf="generatedTicket" class="ticket-display">
          <h2>Sua Senha</h2>
          <div class="ticket-number">{{ generatedTicket.ticketNumber }}</div>
          <div class="ticket-info">
            <p><strong>Posição:</strong> {{ generatedTicket.positionInQueue }}</p>
            <p><strong>Tempo estimado:</strong> {{ generatedTicket.estimatedWaitMinutes }} min</p>
            <p *ngIf="generatedTicket.priority !== 'Normal'" class="priority-badge">
              {{ generatedTicket.priorityReason }}
            </p>
          </div>
          <button mat-raised-button color="accent" (click)="printTicket()">
            <mat-icon>print</mat-icon>
            Imprimir Senha
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .totem-container {
      height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
    }
    
    .totem-header {
      text-align: center;
      margin-bottom: 3rem;
    }
    
    .totem-header h1 {
      font-size: 3rem;
      margin-bottom: 1rem;
    }
    
    .queue-type-selection {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 2rem;
      max-width: 800px;
      margin: 0 auto;
    }
    
    .queue-button {
      height: 200px;
      font-size: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    
    .queue-button mat-icon {
      font-size: 4rem;
      width: 4rem;
      height: 4rem;
    }
    
    .ticket-display {
      text-align: center;
      margin-top: 3rem;
      background: white;
      color: #333;
      padding: 3rem;
      border-radius: 1rem;
      max-width: 500px;
      margin: 3rem auto 0;
    }
    
    .ticket-number {
      font-size: 6rem;
      font-weight: bold;
      color: #667eea;
      margin: 2rem 0;
    }
    
    .priority-badge {
      display: inline-block;
      background: #ff9800;
      color: white;
      padding: 0.5rem 1rem;
      border-radius: 2rem;
      font-weight: bold;
    }
  `]
})
export class TotemComponent {
  generatedTicket: QueueTicket | null = null;
  
  constructor(
    private queueService: QueueService,
    private snackBar: MatSnackBar
  ) {}
  
  async generateTicket(queueType: string) {
    try {
      this.generatedTicket = await this.queueService.generateTicket({
        queueType,
        patientName: 'Paciente sem cadastro',  // Ou coletar dados
        isUrgent: false
      });
      
      this.snackBar.open('Senha gerada com sucesso!', 'OK', { duration: 3000 });
      
      // Auto-reset após 30 segundos
      setTimeout(() => {
        this.generatedTicket = null;
      }, 30000);
    } catch (error) {
      this.snackBar.open('Erro ao gerar senha: ' + error.message, 'OK');
    }
  }
  
  printTicket() {
    window.print();
  }
}

// src/app/features/queue/display-panel/display-panel.component.ts
@Component({
  selector: 'app-queue-display-panel',
  template: `
    <div class="display-panel">
      <div class="panel-header">
        <h1>Painel de Chamadas</h1>
        <div class="current-time">{{ currentTime | date:'HH:mm:ss' }}</div>
      </div>
      
      <div class="current-call" *ngIf="currentTicket">
        <div class="call-animation">
          <mat-icon>campaign</mat-icon>
        </div>
        <div class="ticket-number">{{ currentTicket.ticketNumber }}</div>
        <div class="room-name">{{ currentTicket.roomName }}</div>
      </div>
      
      <div class="waiting-tickets">
        <h2>Aguardando</h2>
        <div class="ticket-grid">
          <div *ngFor="let ticket of waitingTickets" class="waiting-ticket">
            <span class="ticket-num">{{ ticket.ticketNumber }}</span>
            <span class="ticket-wait">{{ ticket.estimatedWaitMinutes }}min</span>
          </div>
        </div>
      </div>
      
      <div class="statistics">
        <div class="stat-item">
          <mat-icon>people</mat-icon>
          <span>{{ statistics?.totalWaiting }} aguardando</span>
        </div>
        <div class="stat-item">
          <mat-icon>schedule</mat-icon>
          <span>{{ statistics?.averageWaitMinutes | number:'1.0-0' }}min médio</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .display-panel {
      height: 100vh;
      background: #1a1a2e;
      color: white;
      padding: 2rem;
      overflow: hidden;
    }
    
    .current-call {
      text-align: center;
      padding: 4rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 2rem;
      margin: 2rem 0;
      animation: pulse 2s infinite;
    }
    
    @keyframes pulse {
      0%, 100% { transform: scale(1); }
      50% { transform: scale(1.05); }
    }
    
    .ticket-number {
      font-size: 8rem;
      font-weight: bold;
      margin: 2rem 0;
    }
    
    .room-name {
      font-size: 3rem;
    }
    
    .waiting-tickets {
      margin-top: 3rem;
    }
    
    .ticket-grid {
      display: grid;
      grid-template-columns: repeat(5, 1fr);
      gap: 1rem;
      margin-top: 1rem;
    }
    
    .waiting-ticket {
      background: #16213e;
      padding: 1rem;
      border-radius: 0.5rem;
      text-align: center;
    }
  `]
})
export class DisplayPanelComponent implements OnInit, OnDestroy {
  currentTicket: QueueTicket | null = null;
  waitingTickets: QueueTicket[] = [];
  statistics: QueueStatistics | null = null;
  currentTime = new Date();
  
  private hubConnection: signalR.HubConnection;
  private timeInterval: any;
  
  constructor(
    private queueService: QueueService,
    private audioService: AudioService
  ) {}
  
  async ngOnInit() {
    await this.loadInitialData();
    await this.setupSignalR();
    
    this.timeInterval = setInterval(() => {
      this.currentTime = new Date();
    }, 1000);
  }
  
  async setupSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/queue')
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.on('TicketCalled', (ticket: QueueTicket) => {
      this.currentTicket = ticket;
      this.playCallAlert(ticket);
      this.loadWaitingTickets();
    });
    
    this.hubConnection.on('NewTicket', () => {
      this.loadWaitingTickets();
      this.loadStatistics();
    });
    
    this.hubConnection.on('PlayAudio', (audioData: any) => {
      this.audioService.playTicketCall(audioData);
    });
    
    await this.hubConnection.start();
  }
  
  async loadInitialData() {
    this.waitingTickets = await this.queueService.getWaitingTickets();
    this.statistics = await this.queueService.getStatistics();
  }
  
  playCallAlert(ticket: QueueTicket) {
    // Text-to-Speech ou áudio pré-gravado
    const msg = `Senha ${ticket.ticketNumber}, dirija-se à ${ticket.roomName}`;
    this.audioService.speak(msg);
  }
  
  ngOnDestroy() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
    if (this.timeInterval) {
      clearInterval(this.timeInterval);
    }
  }
}

// src/app/features/queue/attendant-dashboard/attendant-dashboard.component.ts
@Component({
  selector: 'app-attendant-dashboard',
  template: `
    <div class="attendant-dashboard">
      <h2>Painel do Atendente</h2>
      
      <mat-card class="current-service">
        <mat-card-header>
          <mat-card-title>Atendimento Atual</mat-card-title>
        </mat-card-header>
        <mat-card-content *ngIf="currentTicket">
          <div class="ticket-info">
            <h3>Senha: {{ currentTicket.ticketNumber }}</h3>
            <p><strong>Paciente:</strong> {{ currentTicket.patientName }}</p>
            <p><strong>Tipo:</strong> {{ currentTicket.queueType }}</p>
            <p><strong>Tempo de espera:</strong> {{ currentTicket.actualWaitMinutes }}min</p>
          </div>
          <div class="actions">
            <button mat-raised-button color="primary" (click)="startService()">
              Iniciar Atendimento
            </button>
            <button mat-raised-button color="accent" (click)="completeService()">
              Concluir
            </button>
          </div>
        </mat-card-content>
        <mat-card-content *ngIf="!currentTicket">
          <p>Nenhum atendimento em andamento</p>
          <button mat-raised-button color="primary" (click)="callNext()">
            <mat-icon>call_end</mat-icon>
            Chamar Próximo
          </button>
        </mat-card-content>
      </mat-card>
      
      <mat-card class="waiting-list">
        <mat-card-header>
          <mat-card-title>Fila de Espera ({{ waitingTickets.length }})</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <mat-list>
            <mat-list-item *ngFor="let ticket of waitingTickets">
              <mat-icon mat-list-icon [color]="ticket.priority !== 'Normal' ? 'warn' : ''">
                {{ ticket.priority === 'Urgent' ? 'priority_high' : 'person' }}
              </mat-icon>
              <div mat-line>{{ ticket.ticketNumber }} - {{ ticket.patientName }}</div>
              <div mat-line>Aguardando {{ ticket.estimatedWaitMinutes }}min</div>
              <button mat-icon-button (click)="callSpecific(ticket)">
                <mat-icon>phone_forwarded</mat-icon>
              </button>
            </mat-list-item>
          </mat-list>
        </mat-card-content>
      </mat-card>
    </div>
  `
})
export class AttendantDashboardComponent implements OnInit {
  currentTicket: QueueTicket | null = null;
  waitingTickets: QueueTicket[] = [];
  selectedRoom: Room | null = null;
  
  constructor(
    private queueService: QueueService,
    private snackBar: MatSnackBar
  ) {}
  
  async ngOnInit() {
    await this.loadWaitingTickets();
    await this.setupSignalR();
  }
  
  async callNext() {
    try {
      this.currentTicket = await this.queueService.callNext(this.selectedRoom!.id);
      this.snackBar.open('Próximo paciente chamado!', 'OK', { duration: 3000 });
      await this.loadWaitingTickets();
    } catch (error) {
      this.snackBar.open('Erro: ' + error.message, 'OK');
    }
  }
  
  async startService() {
    await this.queueService.startService(this.currentTicket!.id);
    this.snackBar.open('Atendimento iniciado', 'OK', { duration: 2000 });
  }
  
  async completeService() {
    await this.queueService.completeService(this.currentTicket!.id);
    this.currentTicket = null;
    this.snackBar.open('Atendimento concluído!', 'OK', { duration: 2000 });
  }
}
```

### Services

```typescript
// src/app/core/services/queue.service.ts
@Injectable({ providedIn: 'root' })
export class QueueService {
  private apiUrl = '/api/queue';
  private hubConnection: signalR.HubConnection;
  
  constructor(private http: HttpClient) {
    this.setupSignalR();
  }
  
  private setupSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/queue')
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.start();
  }
  
  generateTicket(request: GenerateTicketRequest): Promise<QueueTicket> {
    return firstValueFrom(
      this.http.post<QueueTicket>(`${this.apiUrl}/tickets`, request)
    );
  }
  
  callNext(roomId: string, doctorId?: string): Promise<QueueTicket> {
    return firstValueFrom(
      this.http.post<QueueTicket>(`${this.apiUrl}/call-next`, { roomId, doctorId })
    );
  }
  
  getWaitingTickets(): Promise<QueueTicket[]> {
    return firstValueFrom(
      this.http.get<QueueTicket[]>(`${this.apiUrl}/tickets/waiting`)
    );
  }
  
  getStatistics(): Promise<QueueStatistics> {
    return firstValueFrom(
      this.http.get<QueueStatistics>(`${this.apiUrl}/statistics`)
    );
  }
  
  // SignalR subscriptions
  onNewTicket(callback: (ticket: QueueTicket) => void) {
    this.hubConnection.on('NewTicket', callback);
  }
  
  onTicketCalled(callback: (ticket: QueueTicket) => void) {
    this.hubConnection.on('TicketCalled', callback);
  }
}

// src/app/core/services/audio.service.ts
@Injectable({ providedIn: 'root' })
export class AudioService {
  speak(text: string) {
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = 'pt-BR';
    utterance.rate = 0.9;
    utterance.pitch = 1;
    window.speechSynthesis.speak(utterance);
  }
  
  playTicketCall(audioData: { ticketNumber: string; roomName: string }) {
    // Tocar som de alerta
    const audio = new Audio('/assets/sounds/alert.mp3');
    audio.play();
    
    // Após som, falar a mensagem
    setTimeout(() => {
      this.speak(audioData.message);
    }, 1000);
  }
}
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar entidades de domínio (QueueTicket, QueueConfiguration, Room)
- [ ] Implementar repositórios
- [ ] Criar QueueService
- [ ] Implementar SignalR Hub (QueueHub)
- [ ] Criar QueueHubService para broadcasts
- [ ] Implementar lógica de priorização
- [ ] Criar controllers REST API
- [ ] Adicionar migrations
- [ ] Implementar cálculo de tempo estimado
- [ ] Sistema de notificações (SMS/WhatsApp)
- [ ] Logs e auditoria

### Frontend
- [ ] Criar TotemComponent (geração de senhas)
- [ ] Criar DisplayPanelComponent (painel de TV)
- [ ] Criar AttendantDashboardComponent
- [ ] Criar QueueConfigurationComponent
- [ ] Implementar QueueService (Angular)
- [ ] Implementar AudioService (Text-to-Speech)
- [ ] Integração SignalR no frontend
- [ ] Design responsivo
- [ ] Modo fullscreen para painel
- [ ] Impressão de senhas

### Hardware (Opcional)
- [ ] Setup Raspberry Pi para painel
- [ ] Impressora térmica para senhas
- [ ] Alto-falantes para áudio
- [ ] Touchscreen para totem

### Testes
- [ ] Testes unitários (entidades)
- [ ] Testes de serviços
- [ ] Testes de SignalR Hub
- [ ] Testes de priorização
- [ ] Testes de integração
- [ ] Testes de performance (múltiplos clientes)

### Documentação
- [ ] Guia de configuração
- [ ] Manual do atendente
- [ ] Guia de setup de hardware
- [ ] Troubleshooting

## 💰 Investimento

### Desenvolvimento
- **Esforço**: 2-3 meses | 2 devs
- **Custo**: R$ 90-135k

### Hardware (Por Clínica)
- Raspberry Pi 4: R$ 600
- Monitor/TV 32": R$ 800-1.500
- Impressora térmica: R$ 400-800
- Total hardware: R$ 2-3k

### ROI Esperado
- Redução de reclamações: 50-70%
- Melhoria em NPS: +15-20 pontos
- Otimização da recepção: 30-40% mais eficiente
- Diferencial competitivo: Premium pricing

## 🎯 Critérios de Aceitação

- [ ] Totem gera senhas corretamente
- [ ] Priorização funciona (idosos, gestantes, urgências)
- [ ] Painel atualiza em tempo real via SignalR
- [ ] Chamada de senha com áudio funciona
- [ ] Estimativa de tempo é precisa
- [ ] Notificações SMS/WhatsApp funcionam
- [ ] Dashboard do atendente funcional
- [ ] Estatísticas de performance disponíveis
- [ ] Sistema suporta múltiplas filas
- [ ] Integração com agendamento funciona
- [ ] Impressão de senhas funciona
- [ ] Design intuitivo e acessível
