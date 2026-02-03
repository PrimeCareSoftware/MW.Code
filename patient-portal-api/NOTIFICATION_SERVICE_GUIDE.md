# 🔔 Guia de Implementação - Sistema de Notificações e Lembretes

> **Status:** 🔴 **NÃO IMPLEMENTADO**  
> **Prioridade:** 🔥🔥 **ALTA**  
> **Esforço:** 1 semana | 1 desenvolvedor  
> **Investimento:** R$ 15.000  
> **ROI:** **Reduz no-show em 30-40%**

---

## 📋 Visão Geral

O **Sistema de Notificações Automáticas** envia lembretes de consultas por WhatsApp e Email, permitindo que pacientes confirmem presença com um clique. Esta funcionalidade é crítica para reduzir o índice de faltas (no-show) e melhorar a eficiência operacional da clínica.

### Impacto no Negócio

| Métrica | Sem Lembretes | Com Lembretes | Melhoria |
|---------|---------------|---------------|----------|
| **No-show rate** | 15-20% | 8-12% | **-40%** |
| **Confirmações** | 0% | 85%+ | +85% |
| **Receita perdida/mês** | R$ 12-15k | R$ 5-8k | **-50%** |
| **Ligações de confirmação** | 100% manual | 10% manual | **-90%** |
| **Satisfação paciente** | 7.5/10 | 8.8/10 | **+17%** |

---

## 🏗️ Arquitetura

### Componentes do Sistema

```
┌─────────────────────────────────────────────────────────────┐
│                    Background Service                        │
│              AppointmentReminderService                      │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Timer (executa a cada hora)                         │  │
│  └────────────────┬────────────────────────────────────┘  │
│                   │                                         │
│                   ▼                                         │
│  ┌────────────────────────────────────────────────────┐   │
│  │  Buscar consultas (D+1, não confirmadas)          │   │
│  └────────────────┬───────────────────────────────────┘   │
│                   │                                         │
│         ┌─────────┴─────────┐                              │
│         ▼                   ▼                              │
│  ┌──────────────┐    ┌──────────────┐                     │
│  │   WhatsApp   │    │    Email     │                     │
│  │   (Twilio)   │    │  (SendGrid)  │                     │
│  └──────┬───────┘    └──────┬───────┘                     │
│         │                   │                              │
│         └───────┬───────────┘                              │
│                 ▼                                           │
│  ┌─────────────────────────────────┐                       │
│  │  Link de Confirmação Único      │                       │
│  │  /confirm/{appointmentId}/{token}│                      │
│  └─────────────────────────────────┘                       │
└─────────────────────────────────────────────────────────────┘
```

### Fluxo de Notificação

```
1. [Scheduler] Executa a cada hora (Hangfire/BackgroundService)
2. [Query] Busca consultas para amanhã (24h antes)
3. [Filter] Filtra apenas não confirmadas
4. [Loop] Para cada consulta:
   a. Gera token único de confirmação
   b. Cria link de confirmação
   c. Envia WhatsApp (via Twilio)
   d. Envia Email (via SendGrid)
   e. Registra log de envio
5. [Monitor] Aguarda confirmação do paciente
6. [Update] Atualiza status quando confirmado
```

---

## 🔧 Implementação Backend

### 1. Domain Models

```csharp
// PatientPortal.Domain/Entities/AppointmentReminder.cs
namespace PatientPortal.Domain.Entities
{
    public class AppointmentReminder
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string Channel { get; set; } // "WhatsApp", "Email", "SMS"
        public string Recipient { get; set; } // Telefone ou email
        public DateTime SentAt { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string ConfirmationToken { get; set; } // Token único para confirmação
        public DateTime? ConfirmedAt { get; set; }
        public string MessageId { get; set; } // ID da mensagem no provedor (Twilio/SendGrid)
        public string Status { get; set; } // "Sent", "Delivered", "Read", "Failed"
        public string ErrorMessage { get; set; }
        
        // Navigation
        public Appointment Appointment { get; set; }
    }
    
    public class NotificationTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // "AppointmentReminder", "AppointmentConfirmation", "AppointmentCancellation"
        public string Channel { get; set; } // "WhatsApp", "Email", "SMS"
        public string Subject { get; set; } // Para email
        public string TemplateBody { get; set; } // Template com placeholders: {{PatientName}}, {{DoctorName}}, etc.
        public bool IsActive { get; set; }
    }
}
```

### 2. Notification Services

#### INotificationService Interface

```csharp
// PatientPortal.Application/Interfaces/INotificationService.cs
namespace PatientPortal.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResult> SendWhatsAppAsync(string phoneNumber, string message, Dictionary<string, string> metadata = null);
        Task<NotificationResult> SendEmailAsync(string toEmail, string subject, string htmlBody, string plainTextBody = null);
        Task<NotificationResult> SendSmsAsync(string phoneNumber, string message);
    }
    
    public class NotificationResult
    {
        public bool Success { get; set; }
        public string MessageId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
    }
}
```

#### WhatsApp Service (Twilio)

```csharp
// PatientPortal.Infrastructure/Services/TwilioWhatsAppService.cs
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PatientPortal.Infrastructure.Services
{
    public class TwilioWhatsAppService : INotificationService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        private readonly ILogger<TwilioWhatsAppService> _logger;
        
        public TwilioWhatsAppService(
            IConfiguration configuration,
            ILogger<TwilioWhatsAppService> logger)
        {
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromNumber = configuration["Twilio:WhatsAppNumber"]; // Ex: "whatsapp:+14155238886"
            _logger = logger;
            
            TwilioClient.Init(_accountSid, _authToken);
        }
        
        public async Task<NotificationResult> SendWhatsAppAsync(
            string phoneNumber, 
            string message, 
            Dictionary<string, string> metadata = null)
        {
            try
            {
                // Formatar número para WhatsApp (adicionar 'whatsapp:' prefix)
                var toNumber = new PhoneNumber($"whatsapp:+{phoneNumber.Replace("+", "")}");
                var fromNumber = new PhoneNumber(_fromNumber);
                
                var messageResource = await MessageResource.CreateAsync(
                    to: toNumber,
                    from: fromNumber,
                    body: message
                );
                
                _logger.LogInformation($"WhatsApp sent successfully. SID: {messageResource.Sid}");
                
                return new NotificationResult
                {
                    Success = true,
                    MessageId = messageResource.Sid,
                    Status = messageResource.Status.ToString(),
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send WhatsApp to {phoneNumber}");
                
                return new NotificationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SentAt = DateTime.UtcNow
                };
            }
        }
        
        public async Task<NotificationResult> SendEmailAsync(string toEmail, string subject, string htmlBody, string plainTextBody = null)
        {
            throw new NotImplementedException("Use SendGridService for emails");
        }
        
        public async Task<NotificationResult> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var toNumber = new PhoneNumber($"+{phoneNumber.Replace("+", "")}");
                var fromNumber = new PhoneNumber(_fromNumber.Replace("whatsapp:", ""));
                
                var messageResource = await MessageResource.CreateAsync(
                    to: toNumber,
                    from: fromNumber,
                    body: message
                );
                
                return new NotificationResult
                {
                    Success = true,
                    MessageId = messageResource.Sid,
                    Status = messageResource.Status.ToString(),
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to {phoneNumber}");
                
                return new NotificationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SentAt = DateTime.UtcNow
                };
            }
        }
    }
}
```

#### Email Service (SendGrid)

```csharp
// PatientPortal.Infrastructure/Services/SendGridEmailService.cs
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PatientPortal.Infrastructure.Services
{
    public class SendGridEmailService : INotificationService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<SendGridEmailService> _logger;
        
        public SendGridEmailService(
            IConfiguration configuration,
            ILogger<SendGridEmailService> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            _fromEmail = configuration["SendGrid:FromEmail"];
            _fromName = configuration["SendGrid:FromName"];
            _logger = logger;
        }
        
        public async Task<NotificationResult> SendWhatsAppAsync(string phoneNumber, string message, Dictionary<string, string> metadata = null)
        {
            throw new NotImplementedException("Use TwilioService for WhatsApp");
        }
        
        public async Task<NotificationResult> SendEmailAsync(
            string toEmail, 
            string subject, 
            string htmlBody, 
            string plainTextBody = null)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress(toEmail);
                
                var msg = MailHelper.CreateSingleEmail(
                    from, 
                    to, 
                    subject, 
                    plainTextBody ?? StripHtmlTags(htmlBody), 
                    htmlBody
                );
                
                var response = await client.SendEmailAsync(msg);
                
                _logger.LogInformation($"Email sent to {toEmail}. Status: {response.StatusCode}");
                
                return new NotificationResult
                {
                    Success = response.IsSuccessStatusCode,
                    MessageId = response.Headers.GetValues("X-Message-Id").FirstOrDefault(),
                    Status = response.StatusCode.ToString(),
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                
                return new NotificationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    SentAt = DateTime.UtcNow
                };
            }
        }
        
        public Task<NotificationResult> SendSmsAsync(string phoneNumber, string message)
        {
            throw new NotImplementedException("Use TwilioService for SMS");
        }
        
        private string StripHtmlTags(string html)
        {
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
```

### 3. AppointmentReminderService (Background Service)

```csharp
// PatientPortal.Api/BackgroundServices/AppointmentReminderService.cs
namespace PatientPortal.Api.BackgroundServices
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentReminderService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Executar a cada hora
        
        public AppointmentReminderService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AppointmentReminderService started");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing appointment reminders");
                }
                
                // Aguardar próxima execução
                await Task.Delay(_interval, stoppingToken);
            }
            
            _logger.LogInformation("AppointmentReminderService stopped");
        }
        
        private async Task SendRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var appointmentRepository = scope.ServiceProvider.GetRequiredService<IRepository<Appointment>>();
            var reminderRepository = scope.ServiceProvider.GetRequiredService<IRepository<AppointmentReminder>>();
            var whatsAppService = scope.ServiceProvider.GetRequiredService<TwilioWhatsAppService>();
            var emailService = scope.ServiceProvider.GetRequiredService<SendGridEmailService>();
            
            // Buscar consultas para amanhã (24 horas antes)
            var tomorrow = DateTime.Now.AddDays(1);
            var dayAfterTomorrow = tomorrow.AddDays(1);
            
            var appointmentsToRemind = await appointmentRepository.GetAll()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.ScheduledDate >= tomorrow.Date 
                    && a.ScheduledDate < dayAfterTomorrow.Date
                    && a.Status == AppointmentStatus.Scheduled
                    && !a.PatientConfirmed)
                .ToListAsync();
            
            _logger.LogInformation($"Found {appointmentsToRemind.Count} appointments to remind");
            
            foreach (var appointment in appointmentsToRemind)
            {
                // Verificar se já enviou lembrete
                var alreadySent = await reminderRepository.GetAll()
                    .AnyAsync(r => r.AppointmentId == appointment.Id 
                        && r.SentAt.Date == DateTime.Today);
                
                if (alreadySent)
                {
                    _logger.LogInformation($"Reminder already sent for appointment {appointment.Id}");
                    continue;
                }
                
                // Gerar token de confirmação único
                var confirmationToken = GenerateConfirmationToken();
                var confirmationLink = $"https://portal.omnicare.com/appointments/{appointment.Id}/confirm?token={confirmationToken}";
                
                // Enviar WhatsApp
                if (!string.IsNullOrEmpty(appointment.Patient.Phone))
                {
                    var whatsAppMessage = BuildWhatsAppMessage(appointment, confirmationLink);
                    var whatsAppResult = await whatsAppService.SendWhatsAppAsync(
                        appointment.Patient.Phone, 
                        whatsAppMessage
                    );
                    
                    await SaveReminderLogAsync(
                        reminderRepository, 
                        appointment.Id, 
                        "WhatsApp", 
                        appointment.Patient.Phone, 
                        whatsAppResult,
                        confirmationToken
                    );
                }
                
                // Enviar Email
                if (!string.IsNullOrEmpty(appointment.Patient.Email))
                {
                    var (subject, htmlBody) = BuildEmailMessage(appointment, confirmationLink);
                    var emailResult = await emailService.SendEmailAsync(
                        appointment.Patient.Email, 
                        subject, 
                        htmlBody
                    );
                    
                    await SaveReminderLogAsync(
                        reminderRepository, 
                        appointment.Id, 
                        "Email", 
                        appointment.Patient.Email, 
                        emailResult,
                        confirmationToken
                    );
                }
                
                _logger.LogInformation($"Reminders sent for appointment {appointment.Id}");
            }
            
            await reminderRepository.SaveChangesAsync();
        }
        
        private string BuildWhatsAppMessage(Appointment appointment, string confirmationLink)
        {
            return $@"🏥 *Lembrete de Consulta - Omni Care*

Olá, {appointment.Patient.Name}! 👋

Você tem uma consulta marcada para *amanhã*:

👨‍⚕️ *Médico:* Dr(a). {appointment.Doctor.Name}
📅 *Data:* {appointment.ScheduledDate:dd/MM/yyyy}
🕐 *Horário:* {appointment.ScheduledDate:HH:mm}
📍 *Local:* {appointment.Doctor.ClinicAddress}

✅ *Confirme sua presença:*
{confirmationLink}

❌ *Precisa cancelar?* 
Entre em contato pelo (11) 9999-9999

_Mensagem automática - Não responda_";
        }
        
        private (string subject, string htmlBody) BuildEmailMessage(Appointment appointment, string confirmationLink)
        {
            var subject = $"Lembrete: Consulta com Dr(a). {appointment.Doctor.Name} Amanhã";
            
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #1976d2; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .confirm-button {{ display: inline-block; background-color: #4caf50; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; color: #888; font-size: 12px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>🏥 Lembrete de Consulta</h2>
        </div>
        <div class='content'>
            <p>Olá, <strong>{appointment.Patient.Name}</strong>!</p>
            <p>Este é um lembrete de que você tem uma consulta marcada para <strong>amanhã</strong>:</p>
            
            <div class='details'>
                <p><strong>👨‍⚕️ Médico:</strong> Dr(a). {appointment.Doctor.Name}</p>
                <p><strong>🏥 Especialidade:</strong> {appointment.Doctor.Specialty}</p>
                <p><strong>📅 Data:</strong> {appointment.ScheduledDate:dddd, dd 'de' MMMM 'de' yyyy}</p>
                <p><strong>🕐 Horário:</strong> {appointment.ScheduledDate:HH:mm}</p>
                <p><strong>📍 Local:</strong> {appointment.Doctor.ClinicAddress}</p>
                <p><strong>📝 Motivo:</strong> {appointment.Reason}</p>
            </div>
            
            <p style='text-align: center;'>
                <a href='{confirmationLink}' class='confirm-button'>✅ Confirmar Presença</a>
            </p>
            
            <p><strong>⚠️ Importante:</strong></p>
            <ul>
                <li>Chegue com 15 minutos de antecedência</li>
                <li>Traga documento com foto e carteirinha do convênio (se aplicável)</li>
                <li>Cancelamentos devem ser feitos com 24h de antecedência</li>
            </ul>
            
            <p>Se precisar cancelar ou reagendar, acesse seu <a href='https://portal.omnicare.com'>Portal do Paciente</a> ou ligue para (11) 9999-9999.</p>
        </div>
        <div class='footer'>
            <p>© 2026 Omni Care Software - Sistema de Gestão Médica</p>
            <p>Esta é uma mensagem automática. Por favor, não responda este email.</p>
        </div>
    </div>
</body>
</html>";
            
            return (subject, htmlBody);
        }
        
        private async Task SaveReminderLogAsync(
            IRepository<AppointmentReminder> repository,
            Guid appointmentId,
            string channel,
            string recipient,
            NotificationResult result,
            string confirmationToken)
        {
            var reminder = new AppointmentReminder
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointmentId,
                Channel = channel,
                Recipient = recipient,
                SentAt = DateTime.UtcNow,
                IsDelivered = result.Success,
                DeliveredAt = result.Success ? DateTime.UtcNow : null,
                MessageId = result.MessageId,
                Status = result.Status,
                ErrorMessage = result.ErrorMessage,
                ConfirmationToken = confirmationToken
            };
            
            await repository.AddAsync(reminder);
        }
        
        private string GenerateConfirmationToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "").Replace("/", "").Replace("=", "")
                .Substring(0, 16);
        }
    }
}
```

### 4. Confirmation Controller

```csharp
// PatientPortal.Api/Controllers/AppointmentConfirmationController.cs
namespace PatientPortal.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentConfirmationController : ControllerBase
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<AppointmentReminder> _reminderRepository;
        private readonly ILogger<AppointmentConfirmationController> _logger;
        
        [HttpPost("{id}/confirm")]
        [AllowAnonymous] // Permitir confirmação sem login (via link do email/WhatsApp)
        public async Task<IActionResult> ConfirmAppointment(
            Guid id, 
            [FromQuery] string token)
        {
            // 1. Validar token
            var reminder = await _reminderRepository.GetAll()
                .FirstOrDefaultAsync(r => r.AppointmentId == id && r.ConfirmationToken == token);
            
            if (reminder == null)
            {
                return BadRequest("Token de confirmação inválido");
            }
            
            // 2. Verificar se token ainda é válido (48h)
            if (reminder.SentAt < DateTime.UtcNow.AddHours(-48))
            {
                return BadRequest("Token de confirmação expirado");
            }
            
            // 3. Buscar agendamento
            var appointment = await _appointmentRepository.GetAll()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null)
            {
                return NotFound("Agendamento não encontrado");
            }
            
            // 4. Verificar se já confirmado
            if (appointment.PatientConfirmed)
            {
                return Ok(new { message = "Consulta já confirmada anteriormente" });
            }
            
            // 5. Confirmar agendamento
            appointment.PatientConfirmed = true;
            appointment.ConfirmedAt = DateTime.UtcNow;
            
            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            
            // 6. Atualizar log de lembrete
            reminder.ConfirmedAt = DateTime.UtcNow;
            await _reminderRepository.UpdateAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
            
            _logger.LogInformation($"Appointment {id} confirmed by patient via token");
            
            return Ok(new 
            { 
                message = "Consulta confirmada com sucesso!",
                appointment = new
                {
                    id = appointment.Id,
                    doctor = appointment.Doctor.Name,
                    scheduledDate = appointment.ScheduledDate,
                    patient = appointment.Patient.Name
                }
            });
        }
        
        [HttpGet("{id}/confirmation-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetConfirmationStatus(Guid id, [FromQuery] string token)
        {
            var reminder = await _reminderRepository.GetAll()
                .FirstOrDefaultAsync(r => r.AppointmentId == id && r.ConfirmationToken == token);
            
            if (reminder == null)
            {
                return BadRequest("Token inválido");
            }
            
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            
            return Ok(new 
            { 
                isConfirmed = appointment.PatientConfirmed,
                confirmedAt = appointment.ConfirmedAt
            });
        }
    }
}
```

---

## 🎨 Frontend - Página de Confirmação

```typescript
// frontend/patient-portal/src/app/pages/appointments/confirm/confirm-appointment.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../../services/appointment.service';

@Component({
  selector: 'app-confirm-appointment',
  templateUrl: './confirm-appointment.component.html',
  styleUrls: ['./confirm-appointment.component.scss']
})
export class ConfirmAppointmentComponent implements OnInit {
  appointmentId: string;
  token: string;
  loading = true;
  confirmed = false;
  error = false;
  appointment: any;
  
  constructor(
    private route: ActivatedRoute,
    private appointmentService: AppointmentService
  ) {}
  
  ngOnInit() {
    this.appointmentId = this.route.snapshot.params['id'];
    this.token = this.route.snapshot.queryParams['token'];
    
    if (!this.appointmentId || !this.token) {
      this.error = true;
      this.loading = false;
      return;
    }
    
    this.confirmAppointment();
  }
  
  confirmAppointment() {
    this.appointmentService.confirmAppointment(this.appointmentId, this.token)
      .subscribe({
        next: (response) => {
          this.confirmed = true;
          this.appointment = response.appointment;
          this.loading = false;
        },
        error: (error) => {
          this.error = true;
          this.loading = false;
        }
      });
  }
}
```

```html
<!-- confirm-appointment.component.html -->
<div class="confirm-container">
  <mat-card>
    <mat-card-content>
      <!-- Loading -->
      <div *ngIf="loading" class="loading">
        <mat-spinner></mat-spinner>
        <p>Processando confirmação...</p>
      </div>
      
      <!-- Success -->
      <div *ngIf="!loading && confirmed" class="success">
        <mat-icon color="primary" class="success-icon">check_circle</mat-icon>
        <h2>Consulta Confirmada!</h2>
        <p>Sua presença foi confirmada com sucesso.</p>
        
        <div class="appointment-details">
          <p><strong>Médico:</strong> {{ appointment.doctor }}</p>
          <p><strong>Data:</strong> {{ appointment.scheduledDate | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>
        
        <p class="reminder">⏰ Lembre-se de chegar com 15 minutos de antecedência.</p>
        
        <button mat-raised-button color="primary" routerLink="/appointments">
          Ver Minhas Consultas
        </button>
      </div>
      
      <!-- Error -->
      <div *ngIf="!loading && error" class="error">
        <mat-icon color="warn" class="error-icon">error</mat-icon>
        <h2>Erro na Confirmação</h2>
        <p>Não foi possível confirmar sua consulta. O link pode estar expirado ou inválido.</p>
        
        <button mat-raised-button color="primary" routerLink="/">
          Voltar ao Portal
        </button>
      </div>
    </mat-card-content>
  </mat-card>
</div>
```

---

## ⚙️ Configuração

### appsettings.json

```json
{
  "Twilio": {
    "AccountSid": "YOUR_TWILIO_ACCOUNT_SID",
    "AuthToken": "YOUR_TWILIO_AUTH_TOKEN",
    "WhatsAppNumber": "whatsapp:+14155238886",
    "PhoneNumber": "+15551234567"
  },
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "FromEmail": "noreply@omnicare.com",
    "FromName": "Omni Care - Portal do Paciente"
  },
  "AppointmentReminders": {
    "Enabled": true,
    "IntervalHours": 1,
    "HoursBeforeAppointment": 24,
    "MaxRetries": 3
  }
}
```

### Startup Configuration

```csharp
// Program.cs
services.AddScoped<TwilioWhatsAppService>();
services.AddScoped<SendGridEmailService>();
services.AddHostedService<AppointmentReminderService>();

// Adicionar Twilio NuGet package
// Install-Package Twilio

// Adicionar SendGrid NuGet package
// Install-Package SendGrid
```

---

## ✅ Critérios de Sucesso

### Técnicos
- [ ] Background service executando a cada hora
- [ ] Integração Twilio (WhatsApp) funcional
- [ ] Integração SendGrid (Email) funcional
- [ ] Links de confirmação únicos e seguros
- [ ] Logs de envio completos
- [ ] Retry logic para falhas
- [ ] Testes automatizados (> 80% coverage)

### Funcionais
- [ ] Lembretes enviados 24h antes da consulta
- [ ] Paciente consegue confirmar com 1 clique
- [ ] Notificações em 2 canais (WhatsApp + Email)
- [ ] Templates profissionais e personalizados
- [ ] Status de confirmação visível no dashboard

### Negócio
- [ ] Redução de 30-40% no no-show
- [ ] Taxa de confirmação > 80%
- [ ] Redução de ligações de confirmação em 90%
- [ ] Satisfação do paciente > 8.5/10
- [ ] Taxa de entrega > 95%

---

## 🧪 Testes

### Testes Unitários

```csharp
[Fact]
public async Task SendReminders_ShouldSendToAppointmentsTomorrow()
{
    // Arrange
    var tomorrow = DateTime.Now.AddDays(1);
    var appointment = CreateTestAppointment(tomorrow);
    
    // Act
    await _reminderService.SendRemindersAsync();
    
    // Assert
    _whatsAppService.Verify(x => x.SendWhatsAppAsync(
        It.IsAny<string>(), 
        It.IsAny<string>(), 
        It.IsAny<Dictionary<string, string>>()), 
        Times.Once
    );
}

[Fact]
public async Task ConfirmAppointment_WithValidToken_ShouldUpdateStatus()
{
    // Arrange
    var appointmentId = Guid.NewGuid();
    var token = "valid-token";
    
    // Act
    var result = await _confirmationController.ConfirmAppointment(appointmentId, token);
    
    // Assert
    Assert.IsType<OkObjectResult>(result);
    var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
    Assert.True(appointment.PatientConfirmed);
}
```

---

## 📊 Métricas a Monitorar

1. **Taxa de Envio**
   - % de lembretes enviados com sucesso
   - Meta: > 98%

2. **Taxa de Entrega**
   - % de mensagens entregues (WhatsApp/Email)
   - Meta: > 95%

3. **Taxa de Leitura**
   - % de mensagens lidas pelos pacientes
   - Meta: > 80%

4. **Taxa de Confirmação**
   - % de pacientes que confirmam presença
   - Meta: > 75%

5. **Redução de No-Show**
   - Comparativo antes vs depois
   - Meta: -30% a -40%

---

## 🚨 Custos Estimados

### Twilio (WhatsApp)
- **Custo por mensagem:** ~$0.005 USD
- **Volume estimado:** 1.000 msgs/mês
- **Custo mensal:** ~$5 USD (~R$ 25)

### SendGrid (Email)
- **Plano gratuito:** 100 emails/dia
- **Plano pago:** $14.95/mês (40k emails)
- **Custo mensal:** R$ 0 - R$ 75

**Total Estimado:** R$ 25 - R$ 100/mês

---

## 📚 Referências

- [Twilio WhatsApp API](https://www.twilio.com/docs/whatsapp)
- [SendGrid Email API](https://docs.sendgrid.com/)
- [.NET Background Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/hosted-services)
- [PATIENT_PORTAL_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_GUIDE.md)

---

**Última Atualização:** 26 de Janeiro de 2026  
**Versão:** 1.0  
**Status:** 📖 Documentação Completa - Aguardando Implementação
