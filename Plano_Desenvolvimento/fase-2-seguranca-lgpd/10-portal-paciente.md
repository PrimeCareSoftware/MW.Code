# 🌐 Portal do Paciente - Self-Service Web

**Prioridade:** 🔥🔥 P1 - ALTA  
**Impacto:** Muito Alto - Redução de custos operacionais  
**Status Atual:** 0% completo  
**Esforço:** 2-3 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 90.000  
**Prazo:** Q2 2026 (Abril-Junho)

## 📋 Contexto

O Portal do Paciente é uma plataforma web self-service que permite aos pacientes acessarem seus dados médicos, agendarem consultas, confirmarem compromissos, e baixarem documentos, reduzindo significativamente a carga sobre a recepção.

### Por que é Prioridade Alta?

1. **ROI Rápido:** Retorno do investimento em < 6 meses
2. **Redução de Custos:** 40-50% de redução em ligações telefônicas
3. **No-Show:** Reduz em 30-40% com confirmações automáticas
4. **Mercado:** 90% dos concorrentes já possuem portal
5. **Experiência:** Pacientes esperam ter acesso digital aos seus dados
6. **Escalabilidade:** Libera equipe para tarefas mais críticas

### Situação Atual

- ❌ Pacientes ligam para recepção para agendamento
- ❌ Sem confirmação automática de consultas
- ❌ Pacientes precisam ir presencialmente para pegar receitas/atestados
- ❌ Alto índice de no-show (falta)
- ❌ Sobrecarga na recepção
- ✅ Sistema de agendamento backend existe (pode ser reutilizado)

### Benefícios Esperados

| Métrica | Antes | Depois | Melhoria |
|---------|-------|--------|----------|
| Ligações/dia | 80-100 | 40-50 | **-50%** |
| No-show rate | 15-20% | 8-12% | **-40%** |
| Tempo recepção/paciente | 5 min | 2 min | **-60%** |
| Satisfação paciente | 7.5/10 | 9.0/10 | **+20%** |
| Custo operacional | R$ 15k/mês | R$ 9k/mês | **-40%** |

## 🎯 Objetivos da Tarefa

Criar portal web responsivo (PWA) onde pacientes possam se cadastrar, agendar consultas online, confirmar compromissos, visualizar histórico médico, baixar documentos (receitas, atestados, laudos), e acessar telemedicina, com autenticação segura e design mobile-first.

## 📝 Tarefas Detalhadas

### 1. Novo Projeto Angular - Patient Portal (1 semana)

#### 1.1 Estrutura do Projeto

```bash
# Criar novo projeto Angular
ng new patient-portal --routing --style=scss --strict

cd patient-portal

# Instalar dependências
npm install @angular/material @angular/cdk
npm install @auth0/angular-jwt
npm install ngx-mask
npm install chart.js ng2-charts
npm install pwabuilder-lib --save-dev

# Configurar PWA
ng add @angular/pwa
```

#### 1.2 Estrutura de Pastas

```
patient-portal/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── guards/
│   │   │   │   └── auth.guard.ts
│   │   │   ├── interceptors/
│   │   │   │   └── auth.interceptor.ts
│   │   │   └── services/
│   │   │       ├── auth.service.ts
│   │   │       ├── api.service.ts
│   │   │       └── notification.service.ts
│   │   ├── shared/
│   │   │   ├── components/
│   │   │   │   ├── header/
│   │   │   │   ├── footer/
│   │   │   │   └── loading/
│   │   │   └── models/
│   │   ├── features/
│   │   │   ├── auth/
│   │   │   │   ├── login/
│   │   │   │   ├── register/
│   │   │   │   └── forgot-password/
│   │   │   ├── dashboard/
│   │   │   ├── appointments/
│   │   │   │   ├── appointment-list/
│   │   │   │   ├── appointment-booking/
│   │   │   │   └── appointment-confirmation/
│   │   │   ├── documents/
│   │   │   │   ├── document-list/
│   │   │   │   └── document-viewer/
│   │   │   ├── medical-history/
│   │   │   ├── profile/
│   │   │   └── telemedicine/
│   │   └── app-routing.module.ts
│   ├── assets/
│   ├── environments/
│   └── styles.scss
└── angular.json
```

### 2. Backend - API para Pacientes (2 semanas)

#### 2.1 Controller Específico para Pacientes

```csharp
// src/MedicSoft.Api/Controllers/Portal/PatientPortalController.cs
namespace MedicSoft.Api.Controllers.Portal
{
    [ApiController]
    [Route("api/portal/patients")]
    [Authorize(Roles = "Patient")]
    public class PatientPortalController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IDocumentService _documentService;
        
        [HttpGet("me")]
        public async Task<ActionResult<PatientDto>> GetMyProfile()
        {
            var patientId = GetCurrentPatientId();
            var patient = await _patientService.GetByIdAsync(patientId);
            return Ok(patient);
        }
        
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePatientDto dto)
        {
            var patientId = GetCurrentPatientId();
            await _patientService.UpdateAsync(patientId, dto);
            return NoContent();
        }
        
        [HttpGet("me/appointments")]
        public async Task<ActionResult<List<AppointmentDto>>> GetMyAppointments(
            [FromQuery] AppointmentFilter filter)
        {
            var patientId = GetCurrentPatientId();
            filter.PatientId = patientId;
            
            var appointments = await _appointmentService.GetByFilterAsync(filter);
            return Ok(appointments);
        }
        
        [HttpPost("me/appointments")]
        public async Task<ActionResult<AppointmentDto>> BookAppointment(
            [FromBody] BookAppointmentDto dto)
        {
            var patientId = GetCurrentPatientId();
            dto.PatientId = patientId;
            
            var appointment = await _appointmentService.BookOnlineAsync(dto);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }
        
        [HttpPost("me/appointments/{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(Guid id)
        {
            var patientId = GetCurrentPatientId();
            await _appointmentService.ConfirmAsync(id, patientId);
            return NoContent();
        }
        
        [HttpPost("me/appointments/{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(
            Guid id, 
            [FromBody] CancelAppointmentDto dto)
        {
            var patientId = GetCurrentPatientId();
            await _appointmentService.CancelAsync(id, patientId, dto.Reason);
            return NoContent();
        }
        
        [HttpGet("me/documents")]
        public async Task<ActionResult<List<DocumentDto>>> GetMyDocuments()
        {
            var patientId = GetCurrentPatientId();
            var documents = await _documentService.GetByPatientIdAsync(patientId);
            return Ok(documents);
        }
        
        [HttpGet("me/documents/{id}/download")]
        public async Task<IActionResult> DownloadDocument(Guid id)
        {
            var patientId = GetCurrentPatientId();
            var document = await _documentService.GetDocumentAsync(id, patientId);
            
            return File(document.Content, document.ContentType, document.FileName);
        }
        
        [HttpGet("me/medical-history")]
        public async Task<ActionResult<MedicalHistoryDto>> GetMyMedicalHistory()
        {
            var patientId = GetCurrentPatientId();
            var history = await _patientService.GetMedicalHistoryAsync(patientId);
            return Ok(history);
        }
        
        private Guid GetCurrentPatientId()
        {
            var patientIdClaim = User.FindFirst("patient_id")?.Value;
            return Guid.Parse(patientIdClaim);
        }
    }
}
```

#### 2.2 Serviço de Disponibilidade de Médicos

```csharp
// src/MedicSoft.Core/Services/DoctorAvailabilityService.cs
public interface IDoctorAvailabilityService
{
    Task<List<DoctorAvailabilityDto>> GetAvailableSlotsAsync(
        Guid? doctorId, 
        DateTime date, 
        string specialty = null);
    
    Task<bool> IsSlotAvailableAsync(Guid doctorId, DateTime dateTime);
}

public class DoctorAvailabilityService : IDoctorAvailabilityService
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<DoctorSchedule> _scheduleRepository;
    
    public async Task<List<DoctorAvailabilityDto>> GetAvailableSlotsAsync(
        Guid? doctorId,
        DateTime date,
        string specialty = null)
    {
        var availableSlots = new List<DoctorAvailabilityDto>();
        
        // Buscar médicos (filtrado por especialidade se especificado)
        var doctorsQuery = _doctorRepository.GetAll();
        
        if (doctorId.HasValue)
            doctorsQuery = doctorsQuery.Where(d => d.Id == doctorId.Value);
        
        if (!string.IsNullOrEmpty(specialty))
            doctorsQuery = doctorsQuery.Where(d => d.Specialty == specialty);
        
        var doctors = await doctorsQuery.ToListAsync();
        
        foreach (var doctor in doctors)
        {
            // Buscar horários de trabalho do médico
            var schedule = await _scheduleRepository.GetAll()
                .Where(s => s.DoctorId == doctor.Id && s.DayOfWeek == date.DayOfWeek)
                .FirstOrDefaultAsync();
            
            if (schedule == null) continue;
            
            // Buscar agendamentos existentes
            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.DoctorId == doctor.Id 
                    && a.ScheduledDate.Date == date.Date
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
            
            // Gerar slots disponíveis
            var slots = GenerateTimeSlots(
                schedule.StartTime, 
                schedule.EndTime, 
                schedule.AppointmentDuration
            );
            
            foreach (var slot in slots)
            {
                var slotDateTime = date.Date.Add(slot);
                
                // Verificar se slot já está ocupado
                var isOccupied = existingAppointments.Any(a => 
                    a.ScheduledDate == slotDateTime
                );
                
                if (!isOccupied && slotDateTime > DateTime.Now)
                {
                    availableSlots.Add(new DoctorAvailabilityDto
                    {
                        DoctorId = doctor.Id,
                        DoctorName = doctor.Name,
                        Specialty = doctor.Specialty,
                        AvailableDate = slotDateTime,
                        Duration = schedule.AppointmentDuration
                    });
                }
            }
        }
        
        return availableSlots.OrderBy(s => s.AvailableDate).ToList();
    }
    
    private List<TimeSpan> GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime, int durationMinutes)
    {
        var slots = new List<TimeSpan>();
        var current = startTime;
        
        while (current.Add(TimeSpan.FromMinutes(durationMinutes)) <= endTime)
        {
            slots.Add(current);
            current = current.Add(TimeSpan.FromMinutes(durationMinutes));
        }
        
        return slots;
    }
}
```

### 3. Autenticação de Paciente (2 semanas)

#### 3.1 Registro Self-Service

```csharp
// src/MedicSoft.Api/Controllers/Portal/PatientAuthController.cs
[ApiController]
[Route("api/portal/auth")]
public class PatientAuthController : ControllerBase
{
    private readonly IPatientAuthService _authService;
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResultDto>> Register([FromBody] RegisterPatientDto dto)
    {
        // Validar CPF
        if (!CpfValidator.IsValid(dto.CPF))
            return BadRequest("CPF inválido");
        
        // Verificar se CPF já existe
        var existingPatient = await _authService.FindByCpfAsync(dto.CPF);
        if (existingPatient != null)
            return Conflict("Paciente já cadastrado");
        
        // Criar paciente
        var patient = await _authService.RegisterAsync(dto);
        
        // Enviar email de confirmação
        await _authService.SendConfirmationEmailAsync(patient.Id);
        
        return Ok(new RegisterResultDto
        {
            PatientId = patient.Id,
            Message = "Cadastro realizado! Verifique seu email para confirmar."
        });
    }
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
    {
        var result = await _authService.ConfirmEmailAsync(dto.Token);
        
        if (!result)
            return BadRequest("Token inválido ou expirado");
        
        return Ok("Email confirmado com sucesso!");
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] PatientLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto.CPF, dto.Password);
        
        if (!result.Success)
            return Unauthorized(result.Message);
        
        return Ok(new LoginResultDto
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            ExpiresIn = 3600,
            Patient = result.Patient
        });
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await _authService.SendPasswordResetEmailAsync(dto.Email);
        
        return Ok("Se o email existir, você receberá instruções para redefinir sua senha.");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
        
        if (!result)
            return BadRequest("Token inválido ou expirado");
        
        return Ok("Senha redefinida com sucesso!");
    }
}
```

#### 3.2 Frontend - Componente de Login

```typescript
// patient-portal/src/app/features/auth/login/login.component.ts
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      cpf: ['', [Validators.required, this.cpfValidator]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.loading = true;
    this.errorMessage = '';

    const { cpf, password } = this.loginForm.value;

    this.authService.login(cpf, password).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('refreshToken', response.refreshToken);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Erro ao fazer login';
        this.loading = false;
      }
    });
  }

  cpfValidator(control: any) {
    const cpf = control.value?.replace(/\D/g, '');
    if (!cpf || cpf.length !== 11) {
      return { invalidCpf: true };
    }
    // Validação completa de CPF aqui
    return null;
  }
}
```

### 4. Dashboard do Paciente (2 semanas)

```typescript
// patient-portal/src/app/features/dashboard/dashboard.component.ts
export class DashboardComponent implements OnInit {
  patient: Patient;
  upcomingAppointments: Appointment[] = [];
  recentDocuments: Document[] = [];
  activePrescriptions: Prescription[] = [];
  healthSummary: HealthSummary;
  
  constructor(
    private patientService: PatientService,
    private appointmentService: AppointmentService,
    private documentService: DocumentService
  ) {}
  
  ngOnInit() {
    this.loadDashboardData();
  }
  
  loadDashboardData() {
    // Carregar dados do paciente
    this.patientService.getMyProfile().subscribe(
      patient => this.patient = patient
    );
    
    // Próximas consultas
    this.appointmentService.getUpcoming().subscribe(
      appointments => this.upcomingAppointments = appointments
    );
    
    // Documentos recentes
    this.documentService.getRecent(5).subscribe(
      documents => this.recentDocuments = documents
    );
    
    // Prescrições ativas
    this.patientService.getActivePrescriptions().subscribe(
      prescriptions => this.activePrescriptions = prescriptions
    );
  }
  
  confirmAppointment(appointmentId: string) {
    this.appointmentService.confirm(appointmentId).subscribe(() => {
      // Atualizar lista
      this.loadDashboardData();
    });
  }
}
```

### 5. Agendamento Online (3 semanas)

```typescript
// patient-portal/src/app/features/appointments/appointment-booking/appointment-booking.component.ts
export class AppointmentBookingComponent implements OnInit {
  bookingForm: FormGroup;
  availableSpecialties: string[] = [];
  availableDoctors: Doctor[] = [];
  availableSlots: TimeSlot[] = [];
  selectedDoctor: Doctor;
  selectedDate: Date;
  
  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private doctorService: DoctorService
  ) {
    this.bookingForm = this.fb.group({
      specialty: ['', Validators.required],
      doctor: ['', Validators.required],
      date: ['', Validators.required],
      time: ['', Validators.required],
      reason: ['', Validators.required]
    });
  }
  
  ngOnInit() {
    this.loadSpecialties();
  }
  
  loadSpecialties() {
    this.doctorService.getSpecialties().subscribe(
      specialties => this.availableSpecialties = specialties
    );
  }
  
  onSpecialtyChange(specialty: string) {
    this.doctorService.getDoctorsBySpecialty(specialty).subscribe(
      doctors => this.availableDoctors = doctors
    );
  }
  
  onDoctorChange(doctorId: string) {
    this.selectedDoctor = this.availableDoctors.find(d => d.id === doctorId);
    this.loadAvailableSlots();
  }
  
  onDateChange(date: Date) {
    this.selectedDate = date;
    this.loadAvailableSlots();
  }
  
  loadAvailableSlots() {
    if (!this.selectedDoctor || !this.selectedDate) return;
    
    this.appointmentService.getAvailableSlots(
      this.selectedDoctor.id,
      this.selectedDate
    ).subscribe(slots => {
      this.availableSlots = slots;
    });
  }
  
  onSubmit() {
    if (this.bookingForm.invalid) return;
    
    const appointmentData = {
      doctorId: this.bookingForm.value.doctor,
      scheduledDate: this.combineDateTime(
        this.bookingForm.value.date,
        this.bookingForm.value.time
      ),
      reason: this.bookingForm.value.reason,
      appointmentType: this.selectedDoctor.defaultAppointmentType
    };
    
    this.appointmentService.book(appointmentData).subscribe({
      next: (appointment) => {
        this.showSuccessMessage('Consulta agendada com sucesso!');
        this.router.navigate(['/appointments']);
      },
      error: (error) => {
        this.showErrorMessage('Erro ao agendar consulta');
      }
    });
  }
}
```

### 6. Confirmação Automática de Consultas (1 semana)

#### 6.1 Backend - Serviço de Notificações

```csharp
// src/MedicSoft.Core/Services/AppointmentReminderService.cs
public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppointmentReminderService> _logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment reminders");
            }
            
            // Executar a cada hora
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task SendRemindersAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var appointmentRepository = scope.ServiceProvider.GetRequiredService<IRepository<Appointment>>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        
        var tomorrow = DateTime.Now.AddDays(1);
        
        // Consultas para amanhã que ainda não foram confirmadas
        var appointmentsToRemind = await appointmentRepository.GetAll()
            .Where(a => a.ScheduledDate.Date == tomorrow.Date 
                && a.Status == AppointmentStatus.Scheduled
                && !a.PatientConfirmed)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync();
        
        foreach (var appointment in appointmentsToRemind)
        {
            // Enviar WhatsApp
            await notificationService.SendWhatsAppAsync(
                appointment.Patient.Phone,
                $"Olá {appointment.Patient.Name}! Você tem consulta marcada com {appointment.Doctor.Name} " +
                $"amanhã às {appointment.ScheduledDate:HH:mm}. " +
                $"Confirme aqui: https://portal.primecare.com/appointments/{appointment.Id}/confirm"
            );
            
            // Enviar Email
            await notificationService.SendEmailAsync(
                appointment.Patient.Email,
                "Lembrete: Consulta Médica Amanhã",
                $"Sua consulta com Dr(a). {appointment.Doctor.Name} está agendada para " +
                $"{appointment.ScheduledDate:dd/MM/yyyy} às {appointment.ScheduledDate:HH:mm}"
            );
        }
    }
}
```

### 7. Visualização de Documentos (2 semanas)

```typescript
// patient-portal/src/app/features/documents/document-list/document-list.component.ts
export class DocumentListComponent implements OnInit {
  documents: Document[] = [];
  filteredDocuments: Document[] = [];
  documentTypes = ['Todos', 'Receita', 'Atestado', 'Laudo', 'Exame'];
  selectedType = 'Todos';
  
  constructor(
    private documentService: DocumentService,
    private dialog: MatDialog
  ) {}
  
  ngOnInit() {
    this.loadDocuments();
  }
  
  loadDocuments() {
    this.documentService.getAll().subscribe(
      documents => {
        this.documents = documents;
        this.filterDocuments();
      }
    );
  }
  
  filterDocuments() {
    if (this.selectedType === 'Todos') {
      this.filteredDocuments = this.documents;
    } else {
      this.filteredDocuments = this.documents.filter(
        d => d.type === this.selectedType
      );
    }
  }
  
  viewDocument(document: Document) {
    this.dialog.open(DocumentViewerComponent, {
      data: { document },
      width: '80vw',
      height: '90vh'
    });
  }
  
  downloadDocument(document: Document) {
    this.documentService.download(document.id).subscribe(
      blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = document.fileName;
        a.click();
      }
    );
  }
  
  shareDocument(document: Document) {
    const dialogRef = this.dialog.open(ShareDocumentDialog, {
      data: { documentId: document.id }
    });
    
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.documentService.share(document.id, result.email).subscribe();
      }
    });
  }
}
```

### 8. Design Mobile-First e PWA (2 semanas)

#### 8.1 Service Worker para PWA

```typescript
// patient-portal/src/app/app.component.ts
export class AppComponent implements OnInit {
  constructor(
    private swUpdate: SwUpdate,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit() {
    if (this.swUpdate.isEnabled) {
      this.swUpdate.versionUpdates.subscribe(event => {
        if (event.type === 'VERSION_READY') {
          const snackBarRef = this.snackBar.open(
            'Nova versão disponível!',
            'Atualizar',
            { duration: 0 }
          );
          
          snackBarRef.onAction().subscribe(() => {
            window.location.reload();
          });
        }
      });
    }
  }
}
```

#### 8.2 Manifest.json

```json
{
  "name": "PrimeCare - Portal do Paciente",
  "short_name": "PrimeCare",
  "theme_color": "#1976d2",
  "background_color": "#fafafa",
  "display": "standalone",
  "scope": "/",
  "start_url": "/",
  "icons": [
    {
      "src": "assets/icons/icon-72x72.png",
      "sizes": "72x72",
      "type": "image/png"
    },
    {
      "src": "assets/icons/icon-192x192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "assets/icons/icon-512x512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ]
}
```

### 9. Testes (2 semanas)

```typescript
// patient-portal/src/app/features/appointments/appointment-booking/appointment-booking.component.spec.ts
describe('AppointmentBookingComponent', () => {
  let component: AppointmentBookingComponent;
  let fixture: ComponentFixture<AppointmentBookingComponent>;
  let appointmentService: jasmine.SpyObj<AppointmentService>;
  
  beforeEach(() => {
    const appointmentServiceSpy = jasmine.createSpyObj('AppointmentService', 
      ['getAvailableSlots', 'book']);
    
    TestBed.configureTestingModule({
      declarations: [ AppointmentBookingComponent ],
      providers: [
        { provide: AppointmentService, useValue: appointmentServiceSpy }
      ]
    });
    
    fixture = TestBed.createComponent(AppointmentBookingComponent);
    component = fixture.componentInstance;
    appointmentService = TestBed.inject(AppointmentService) as jasmine.SpyObj<AppointmentService>;
  });
  
  it('should load available slots when doctor and date are selected', () => {
    const mockSlots = [
      { time: '08:00', available: true },
      { time: '09:00', available: true }
    ];
    
    appointmentService.getAvailableSlots.and.returnValue(of(mockSlots));
    
    component.selectedDoctor = { id: '123', name: 'Dr. Silva' };
    component.selectedDate = new Date();
    component.loadAvailableSlots();
    
    expect(appointmentService.getAvailableSlots).toHaveBeenCalled();
    expect(component.availableSlots.length).toBe(2);
  });
});
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] PWA instalável em dispositivos móveis
- [ ] Responsivo (mobile, tablet, desktop)
- [ ] Performance (Lighthouse > 90)
- [ ] Acessibilidade (WCAG 2.1 AA)
- [ ] Tempo de carregamento < 3s

### Funcionais
- [ ] Cadastro self-service funcional
- [ ] Agendamento online com disponibilidade real-time
- [ ] Confirmação de consultas por WhatsApp/Email
- [ ] Download de documentos (PDF)
- [ ] Visualização de histórico médico
- [ ] Integração com telemedicina (se disponível)

### Negócio
- [ ] 50%+ dos pacientes se cadastram em 6 meses
- [ ] Redução de 40%+ em ligações telefônicas
- [ ] Redução de 30%+ em no-show
- [ ] NPS do portal > 8.0
- [ ] 70%+ dos agendamentos feitos online

## 📦 Entregáveis

1. **Frontend (Angular PWA)**
   - Portal completo responsivo
   - Autenticação segura
   - Dashboard do paciente
   - Agendamento online
   - Visualização de documentos
   - Perfil e histórico médico

2. **Backend APIs**
   - PatientPortalController
   - PatientAuthController
   - DoctorAvailabilityService
   - AppointmentReminderService

3. **Infraestrutura**
   - PWA configurado
   - Service Worker
   - Push notifications (opcional)
   - Deploy em CDN

4. **Documentação**
   - Guia do usuário (paciente)
   - FAQ
   - Troubleshooting
   - Vídeos tutoriais

## 🔗 Dependências

### Pré-requisitos
- ✅ Sistema de agendamento backend
- ✅ Sistema de documentos
- ✅ Notificações WhatsApp/Email
- ❌ Telemedicina (opcional)

### Dependências Externas
- Angular 17+
- Angular Material
- CDN para hosting (Cloudflare/Vercel)

## 🧪 Testes

### Testes Unitários
```bash
ng test --code-coverage
# Meta: > 80% de cobertura
```

### Testes E2E
```bash
ng e2e
# Testar fluxo completo: registro → login → agendamento → confirmação
```

### Testes de Usabilidade
- Testar com 10+ pacientes reais
- Coletar feedback
- Ajustar UX baseado em feedback

## 📊 Métricas de Sucesso

- **Cadastros:** 50%+ dos pacientes em 6 meses
- **Uso:** 70%+ dos agendamentos online
- **No-Show:** Redução de 30-40%
- **Ligações:** Redução de 40-50%
- **Satisfação:** NPS > 8.0
- **Performance:** Lighthouse > 90

## 🚨 Riscos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Baixa adoção inicial | Média | Alto | Campanha de marketing, incentivos |
| Problemas de UX | Média | Médio | Testes de usabilidade, iteração |
| Sobrecarga de agendamentos | Baixa | Médio | Rate limiting, validações |
| Bugs em produção | Média | Alto | Testes abrangentes, rollout gradual |

## 📚 Referências

### Código
- `patient-portal/` - Projeto Angular PWA
- `src/MedicSoft.Api/Controllers/Portal/` - APIs do portal

### Tecnologias
- [Angular PWA](https://angular.io/guide/service-worker-intro)
- [Angular Material](https://material.angular.io/)
- [Workbox](https://developers.google.com/web/tools/workbox)

---

> **IMPORTANTE:** Portal do Paciente tem **ROI rápido** (< 6 meses) e **alto impacto no negócio**  
> **Próximos Passos:** Após aprovação, iniciar projeto Angular  
> **Última Atualização:** 23 de Janeiro de 2026
