# 📅 Guia de Implementação - Agendamento Online (Booking)

> **Status:** 🔴 **NÃO IMPLEMENTADO**  
> **Prioridade:** 🔥🔥🔥 **CRÍTICA**  
> **Esforço:** 3 semanas | 2 desenvolvedores  
> **Investimento:** R$ 45.000  
> **ROI:** **< 6 meses** - Principal funcionalidade do portal

---

## 📋 Visão Geral

O sistema de **agendamento online** permite que pacientes agendem, reagendem e cancelem consultas diretamente pelo portal, sem necessidade de ligar para a recepção. Esta é a funcionalidade **core** que diferencia um portal de visualização de uma plataforma de autoatendimento completa.

### Impacto no Negócio

| Métrica | Sem Booking | Com Booking | Melhoria |
|---------|-------------|-------------|----------|
| Ligações/dia | 80-100 | 40-50 | **-50%** |
| Agendamentos online | 0% | 70% | **+70%** |
| Tempo de agendamento | 5 min (ligação) | 2 min (online) | **-60%** |
| Disponibilidade | 8h-18h (seg-sex) | **24/7** | ∞ |
| Satisfação paciente | 7.5/10 | 9.0/10 | **+20%** |

---

## 🏗️ Arquitetura

### Fluxo de Dados

```
┌─────────────┐         ┌─────────────┐         ┌──────────────┐
│  Paciente   │         │   Backend   │         │  Banco Dados │
│  (Angular)  │         │   (.NET 8)  │         │ (PostgreSQL) │
└──────┬──────┘         └──────┬──────┘         └──────┬───────┘
       │                       │                        │
       │ 1. Seleciona          │                        │
       │    especialidade      │                        │
       ├──────────────────────>│                        │
       │                       │ 2. Busca médicos       │
       │                       │    da especialidade    │
       │                       ├───────────────────────>│
       │                       │<───────────────────────┤
       │<──────────────────────┤ 3. Lista médicos       │
       │                       │                        │
       │ 4. Seleciona médico   │                        │
       │    e data             │                        │
       ├──────────────────────>│                        │
       │                       │ 5. Busca agenda médico │
       │                       │    + consultas         │
       │                       ├───────────────────────>│
       │                       │<───────────────────────┤
       │                       │ 6. Calcula slots       │
       │<──────────────────────┤    disponíveis         │
       │                       │                        │
       │ 7. Seleciona horário  │                        │
       ├──────────────────────>│                        │
       │                       │ 8. Valida disponib.    │
       │                       ├───────────────────────>│
       │                       │<───────────────────────┤
       │                       │ 9. Cria agendamento    │
       │                       ├───────────────────────>│
       │<──────────────────────┤ 10. Confirmação        │
       │                       │                        │
       │ 11. Email/WhatsApp    │                        │
       │<──────────────────────┤                        │
```

---

## 🔧 Implementação Backend

### 1. Domain Models

#### DoctorSchedule Entity

```csharp
// PatientPortal.Domain/Entities/DoctorSchedule.cs
namespace PatientPortal.Domain.Entities
{
    public class DoctorSchedule
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int AppointmentDuration { get; set; } // em minutos (ex: 30)
        public bool IsActive { get; set; }
        
        // Exceções (feriados, folgas)
        public List<ScheduleException> Exceptions { get; set; }
        
        // Navigation
        public Doctor Doctor { get; set; }
    }
    
    public class ScheduleException
    {
        public Guid Id { get; set; }
        public Guid DoctorScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; } // "Feriado", "Folga", "Conferência"
        public bool IsRecurring { get; set; } // para feriados anuais
    }
}
```

#### TimeSlot DTO

```csharp
// PatientPortal.Application/DTOs/Appointments/TimeSlotDto.cs
namespace PatientPortal.Application.DTOs.Appointments
{
    public class TimeSlotDto
    {
        public DateTime DateTime { get; set; }
        public string TimeFormatted { get; set; } // "14:00"
        public bool IsAvailable { get; set; }
        public int DurationMinutes { get; set; }
        public string DoctorName { get; set; }
        public Guid DoctorId { get; set; }
        public string Specialty { get; set; }
    }
    
    public class AvailabilityRequestDto
    {
        public Guid? DoctorId { get; set; } // null = todos os médicos
        public string Specialty { get; set; } // null = todas especialidades
        public DateTime Date { get; set; }
        public int DaysAhead { get; set; } = 1; // quantos dias buscar (1-30)
    }
    
    public class BookAppointmentDto
    {
        [Required]
        public Guid DoctorId { get; set; }
        
        [Required]
        public DateTime ScheduledDateTime { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } // motivo da consulta
        
        [MaxLength(1000)]
        public string Notes { get; set; } // observações adicionais
        
        public string AppointmentType { get; set; } // "Consulta", "Retorno", "Emergência"
    }
}
```

### 2. DoctorAvailabilityService

```csharp
// PatientPortal.Application/Services/DoctorAvailabilityService.cs
namespace PatientPortal.Application.Services
{
    public interface IDoctorAvailabilityService
    {
        Task<List<TimeSlotDto>> GetAvailableSlotsAsync(AvailabilityRequestDto request);
        Task<bool> IsSlotAvailableAsync(Guid doctorId, DateTime dateTime);
        Task<List<DateTime>> GetBlockedDatesAsync(Guid doctorId, DateTime startDate, DateTime endDate);
    }
    
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<DoctorSchedule> _scheduleRepository;
        private readonly ILogger<DoctorAvailabilityService> _logger;
        
        public DoctorAvailabilityService(
            IRepository<Doctor> doctorRepository,
            IRepository<Appointment> appointmentRepository,
            IRepository<DoctorSchedule> scheduleRepository,
            ILogger<DoctorAvailabilityService> logger)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _scheduleRepository = scheduleRepository;
            _logger = logger;
        }
        
        public async Task<List<TimeSlotDto>> GetAvailableSlotsAsync(AvailabilityRequestDto request)
        {
            var availableSlots = new List<TimeSlotDto>();
            
            // 1. Buscar médicos (filtrar por especialidade se especificado)
            var doctorsQuery = _doctorRepository.GetAll().Where(d => d.IsActive);
            
            if (request.DoctorId.HasValue)
            {
                doctorsQuery = doctorsQuery.Where(d => d.Id == request.DoctorId.Value);
            }
            
            if (!string.IsNullOrEmpty(request.Specialty))
            {
                doctorsQuery = doctorsQuery.Where(d => d.Specialty == request.Specialty);
            }
            
            var doctors = await doctorsQuery.ToListAsync();
            
            // 2. Iterar por cada dia solicitado
            for (int dayOffset = 0; dayOffset < request.DaysAhead; dayOffset++)
            {
                var currentDate = request.Date.AddDays(dayOffset);
                
                // Não permitir agendamento em finais de semana (configurável)
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                
                // 3. Para cada médico
                foreach (var doctor in doctors)
                {
                    var slots = await GenerateSlotsForDoctorAsync(doctor.Id, currentDate);
                    availableSlots.AddRange(slots);
                }
            }
            
            return availableSlots.OrderBy(s => s.DateTime).ToList();
        }
        
        private async Task<List<TimeSlotDto>> GenerateSlotsForDoctorAsync(Guid doctorId, DateTime date)
        {
            var slots = new List<TimeSlotDto>();
            
            // 1. Buscar configuração de horário do médico para este dia da semana
            var schedule = await _scheduleRepository.GetAll()
                .Include(s => s.Doctor)
                .Include(s => s.Exceptions)
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId 
                    && s.DayOfWeek == date.DayOfWeek 
                    && s.IsActive);
            
            if (schedule == null)
            {
                _logger.LogInformation($"Doctor {doctorId} has no schedule for {date.DayOfWeek}");
                return slots;
            }
            
            // 2. Verificar se há exceção (folga, feriado) para esta data
            var hasException = schedule.Exceptions.Any(e => 
                e.Date.Date == date.Date || 
                (e.IsRecurring && e.Date.Month == date.Month && e.Date.Day == date.Day)
            );
            
            if (hasException)
            {
                _logger.LogInformation($"Doctor {doctorId} has exception on {date:yyyy-MM-dd}");
                return slots;
            }
            
            // 3. Buscar agendamentos já existentes do médico neste dia
            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.DoctorId == doctorId 
                    && a.ScheduledDate.Date == date.Date
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
            
            // 4. Gerar time slots (horários disponíveis)
            var timeSlots = GenerateTimeSlots(
                schedule.StartTime, 
                schedule.EndTime, 
                schedule.AppointmentDuration
            );
            
            // 5. Para cada slot, verificar se está disponível
            foreach (var timeSlot in timeSlots)
            {
                var slotDateTime = date.Date.Add(timeSlot);
                
                // Não permitir agendamento no passado
                if (slotDateTime <= DateTime.Now)
                    continue;
                
                // Verificar se slot já está ocupado
                var isOccupied = existingAppointments.Any(a => 
                    Math.Abs((a.ScheduledDate - slotDateTime).TotalMinutes) < schedule.AppointmentDuration
                );
                
                slots.Add(new TimeSlotDto
                {
                    DateTime = slotDateTime,
                    TimeFormatted = slotDateTime.ToString("HH:mm"),
                    IsAvailable = !isOccupied,
                    DurationMinutes = schedule.AppointmentDuration,
                    DoctorName = schedule.Doctor.Name,
                    DoctorId = doctorId,
                    Specialty = schedule.Doctor.Specialty
                });
            }
            
            return slots;
        }
        
        private List<TimeSpan> GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime, int durationMinutes)
        {
            var slots = new List<TimeSpan>();
            var current = startTime;
            var duration = TimeSpan.FromMinutes(durationMinutes);
            
            while (current.Add(duration) <= endTime)
            {
                slots.Add(current);
                current = current.Add(duration);
            }
            
            return slots;
        }
        
        public async Task<bool> IsSlotAvailableAsync(Guid doctorId, DateTime dateTime)
        {
            // Verificar se há agendamento conflitante
            var conflictingAppointment = await _appointmentRepository.GetAll()
                .AnyAsync(a => a.DoctorId == doctorId 
                    && a.ScheduledDate == dateTime
                    && a.Status != AppointmentStatus.Cancelled);
            
            return !conflictingAppointment;
        }
        
        public async Task<List<DateTime>> GetBlockedDatesAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            var blockedDates = new List<DateTime>();
            
            var schedules = await _scheduleRepository.GetAll()
                .Include(s => s.Exceptions)
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync();
            
            foreach (var schedule in schedules)
            {
                foreach (var exception in schedule.Exceptions)
                {
                    if (exception.Date >= startDate && exception.Date <= endDate)
                    {
                        blockedDates.Add(exception.Date);
                    }
                }
            }
            
            return blockedDates.OrderBy(d => d).ToList();
        }
    }
}
```

### 3. AppointmentBookingService

```csharp
// PatientPortal.Application/Services/AppointmentBookingService.cs
namespace PatientPortal.Application.Services
{
    public interface IAppointmentBookingService
    {
        Task<AppointmentDto> BookAppointmentAsync(Guid patientId, BookAppointmentDto dto);
        Task<AppointmentDto> RescheduleAppointmentAsync(Guid appointmentId, Guid patientId, DateTime newDateTime);
        Task CancelAppointmentAsync(Guid appointmentId, Guid patientId, string reason);
    }
    
    public class AppointmentBookingService : IAppointmentBookingService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IDoctorAvailabilityService _availabilityService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AppointmentBookingService> _logger;
        
        public async Task<AppointmentDto> BookAppointmentAsync(Guid patientId, BookAppointmentDto dto)
        {
            // 1. Validar se slot ainda está disponível (race condition check)
            var isAvailable = await _availabilityService.IsSlotAvailableAsync(
                dto.DoctorId, 
                dto.ScheduledDateTime
            );
            
            if (!isAvailable)
            {
                throw new BusinessException("Este horário não está mais disponível");
            }
            
            // 2. Validar se paciente já não tem consulta neste horário
            var patientHasConflict = await _appointmentRepository.GetAll()
                .AnyAsync(a => a.PatientId == patientId 
                    && a.ScheduledDate == dto.ScheduledDateTime
                    && a.Status != AppointmentStatus.Cancelled);
            
            if (patientHasConflict)
            {
                throw new BusinessException("Você já tem uma consulta agendada neste horário");
            }
            
            // 3. Criar agendamento
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                ScheduledDate = dto.ScheduledDateTime,
                Reason = dto.Reason,
                Notes = dto.Notes,
                Status = AppointmentStatus.Scheduled,
                AppointmentType = dto.AppointmentType ?? "Consulta",
                CreatedAt = DateTime.UtcNow,
                BookedOnline = true // flag para analytics
            };
            
            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            
            _logger.LogInformation($"Appointment {appointment.Id} booked online by patient {patientId}");
            
            // 4. Enviar confirmação
            await _notificationService.SendAppointmentConfirmationAsync(appointment);
            
            return MapToDto(appointment);
        }
        
        public async Task<AppointmentDto> RescheduleAppointmentAsync(
            Guid appointmentId, 
            Guid patientId, 
            DateTime newDateTime)
        {
            // 1. Buscar agendamento existente
            var appointment = await _appointmentRepository.GetAll()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId);
            
            if (appointment == null)
            {
                throw new NotFoundException("Agendamento não encontrado");
            }
            
            // 2. Validar se pode reagendar (mínimo 2h de antecedência)
            if (appointment.ScheduledDate <= DateTime.Now.AddHours(2))
            {
                throw new BusinessException("Não é possível reagendar com menos de 2 horas de antecedência");
            }
            
            // 3. Validar se novo horário está disponível
            var isAvailable = await _availabilityService.IsSlotAvailableAsync(
                appointment.DoctorId, 
                newDateTime
            );
            
            if (!isAvailable)
            {
                throw new BusinessException("O novo horário selecionado não está disponível");
            }
            
            // 4. Atualizar agendamento
            var oldDateTime = appointment.ScheduledDate;
            appointment.ScheduledDate = newDateTime;
            appointment.UpdatedAt = DateTime.UtcNow;
            
            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            
            _logger.LogInformation($"Appointment {appointmentId} rescheduled from {oldDateTime} to {newDateTime}");
            
            // 5. Notificar mudança
            await _notificationService.SendAppointmentRescheduleNotificationAsync(appointment, oldDateTime);
            
            return MapToDto(appointment);
        }
        
        public async Task CancelAppointmentAsync(Guid appointmentId, Guid patientId, string reason)
        {
            // 1. Buscar agendamento
            var appointment = await _appointmentRepository.GetAll()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == patientId);
            
            if (appointment == null)
            {
                throw new NotFoundException("Agendamento não encontrado");
            }
            
            // 2. Validar se pode cancelar (mínimo 24h de antecedência)
            if (appointment.ScheduledDate <= DateTime.Now.AddHours(24))
            {
                throw new BusinessException("Cancelamentos devem ser feitos com pelo menos 24 horas de antecedência");
            }
            
            // 3. Atualizar status
            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = reason;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancelledBy = "Patient";
            
            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            
            _logger.LogInformation($"Appointment {appointmentId} cancelled by patient {patientId}. Reason: {reason}");
            
            // 4. Notificar cancelamento
            await _notificationService.SendAppointmentCancellationNotificationAsync(appointment);
        }
    }
}
```

### 4. API Controller

```csharp
// PatientPortal.Api/Controllers/AppointmentBookingController.cs
namespace PatientPortal.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentBookingController : ControllerBase
    {
        private readonly IDoctorAvailabilityService _availabilityService;
        private readonly IAppointmentBookingService _bookingService;
        
        [HttpGet("available-slots")]
        [AllowAnonymous] // permite ver horários sem login (para facilitar agendamento)
        public async Task<ActionResult<List<TimeSlotDto>>> GetAvailableSlots(
            [FromQuery] AvailabilityRequestDto request)
        {
            var slots = await _availabilityService.GetAvailableSlotsAsync(request);
            return Ok(slots);
        }
        
        [HttpGet("doctors/{doctorId}/blocked-dates")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DateTime>>> GetBlockedDates(
            Guid doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var blockedDates = await _availabilityService.GetBlockedDatesAsync(doctorId, startDate, endDate);
            return Ok(blockedDates);
        }
        
        [HttpPost("book")]
        public async Task<ActionResult<AppointmentDto>> BookAppointment([FromBody] BookAppointmentDto dto)
        {
            var patientId = GetCurrentPatientId();
            var appointment = await _bookingService.BookAppointmentAsync(patientId, dto);
            
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }
        
        [HttpPut("{id}/reschedule")]
        public async Task<ActionResult<AppointmentDto>> RescheduleAppointment(
            Guid id,
            [FromBody] RescheduleDto dto)
        {
            var patientId = GetCurrentPatientId();
            var appointment = await _bookingService.RescheduleAppointmentAsync(id, patientId, dto.NewDateTime);
            
            return Ok(appointment);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(
            Guid id,
            [FromBody] CancellationDto dto)
        {
            var patientId = GetCurrentPatientId();
            await _bookingService.CancelAppointmentAsync(id, patientId, dto.Reason);
            
            return NoContent();
        }
        
        private Guid GetCurrentPatientId()
        {
            var patientIdClaim = User.FindFirst("patient_id")?.Value;
            return Guid.Parse(patientIdClaim);
        }
    }
}
```

---

## 🎨 Implementação Frontend

### 1. AppointmentBookingComponent

```typescript
// frontend/patient-portal/src/app/pages/appointments/booking/appointment-booking.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-appointment-booking',
  templateUrl: './appointment-booking.component.html',
  styleUrls: ['./appointment-booking.component.scss']
})
export class AppointmentBookingComponent implements OnInit {
  bookingForm: FormGroup;
  currentStep = 1;
  totalSteps = 5;
  
  // Data
  specialties: string[] = [];
  doctors: Doctor[] = [];
  availableSlots: TimeSlot[] = [];
  selectedDate: Date | null = null;
  selectedDoctor: Doctor | null = null;
  
  // Loading states
  loadingDoctors = false;
  loadingSlots = false;
  submitting = false;
  
  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private doctorService: DoctorService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.bookingForm = this.fb.group({
      specialty: ['', Validators.required],
      doctor: ['', Validators.required],
      date: ['', Validators.required],
      time: ['', Validators.required],
      reason: ['', [Validators.required, Validators.maxLength(500)]],
      notes: ['', Validators.maxLength(1000)]
    });
  }
  
  ngOnInit() {
    this.loadSpecialties();
    
    // Watch for specialty changes
    this.bookingForm.get('specialty')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(specialty => {
        if (specialty) {
          this.loadDoctors(specialty);
        }
      });
  }
  
  loadSpecialties() {
    this.doctorService.getSpecialties().subscribe(
      specialties => this.specialties = specialties,
      error => this.showError('Erro ao carregar especialidades')
    );
  }
  
  loadDoctors(specialty: string) {
    this.loadingDoctors = true;
    this.doctors = [];
    this.availableSlots = [];
    
    this.doctorService.getDoctorsBySpecialty(specialty).subscribe(
      doctors => {
        this.doctors = doctors;
        this.loadingDoctors = false;
      },
      error => {
        this.showError('Erro ao carregar médicos');
        this.loadingDoctors = false;
      }
    );
  }
  
  onDoctorChange(doctorId: string) {
    this.selectedDoctor = this.doctors.find(d => d.id === doctorId) || null;
    this.availableSlots = [];
    
    if (this.selectedDoctor && this.selectedDate) {
      this.loadAvailableSlots();
    }
  }
  
  onDateChange(date: Date) {
    this.selectedDate = date;
    this.availableSlots = [];
    
    if (this.selectedDoctor && this.selectedDate) {
      this.loadAvailableSlots();
    }
  }
  
  loadAvailableSlots() {
    if (!this.selectedDoctor || !this.selectedDate) return;
    
    this.loadingSlots = true;
    
    const request: AvailabilityRequest = {
      doctorId: this.selectedDoctor.id,
      date: this.selectedDate,
      daysAhead: 1
    };
    
    this.appointmentService.getAvailableSlots(request).subscribe(
      slots => {
        this.availableSlots = slots.filter(s => s.isAvailable);
        this.loadingSlots = false;
        
        if (this.availableSlots.length === 0) {
          this.showWarning('Não há horários disponíveis para esta data');
        }
      },
      error => {
        this.showError('Erro ao carregar horários disponíveis');
        this.loadingSlots = false;
      }
    );
  }
  
  nextStep() {
    if (this.currentStep < this.totalSteps) {
      this.currentStep++;
    }
  }
  
  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }
  
  canProceedToNext(): boolean {
    switch (this.currentStep) {
      case 1: return !!this.bookingForm.get('specialty')?.value;
      case 2: return !!this.bookingForm.get('doctor')?.value;
      case 3: return !!this.bookingForm.get('date')?.value;
      case 4: return !!this.bookingForm.get('time')?.value;
      case 5: return this.bookingForm.valid;
      default: return false;
    }
  }
  
  onSubmit() {
    if (this.bookingForm.invalid) {
      this.showError('Por favor, preencha todos os campos obrigatórios');
      return;
    }
    
    this.submitting = true;
    
    const selectedTime = this.bookingForm.get('time')?.value;
    const selectedSlot = this.availableSlots.find(s => s.timeFormatted === selectedTime);
    
    if (!selectedSlot) {
      this.showError('Horário selecionado inválido');
      this.submitting = false;
      return;
    }
    
    const bookingData: BookAppointmentRequest = {
      doctorId: this.bookingForm.get('doctor')?.value,
      scheduledDateTime: selectedSlot.dateTime,
      reason: this.bookingForm.get('reason')?.value,
      notes: this.bookingForm.get('notes')?.value || '',
      appointmentType: 'Consulta'
    };
    
    this.appointmentService.bookAppointment(bookingData).subscribe(
      appointment => {
        this.showSuccess('Consulta agendada com sucesso!');
        this.router.navigate(['/appointments', appointment.id]);
      },
      error => {
        if (error.status === 409) {
          this.showError('Este horário não está mais disponível. Por favor, selecione outro horário.');
          this.loadAvailableSlots(); // Recarregar slots
        } else {
          this.showError('Erro ao agendar consulta. Tente novamente.');
        }
        this.submitting = false;
      }
    );
  }
  
  private showSuccess(message: string) {
    this.snackBar.open(message, 'OK', { duration: 5000, panelClass: ['success-snackbar'] });
  }
  
  private showError(message: string) {
    this.snackBar.open(message, 'Fechar', { duration: 5000, panelClass: ['error-snackbar'] });
  }
  
  private showWarning(message: string) {
    this.snackBar.open(message, 'OK', { duration: 5000, panelClass: ['warning-snackbar'] });
  }
}
```

### 2. Template HTML (Stepper)

```html
<!-- appointment-booking.component.html -->
<div class="booking-container">
  <mat-card>
    <mat-card-header>
      <mat-card-title>Agendar Consulta</mat-card-title>
      <mat-card-subtitle>Siga os passos para agendar sua consulta online</mat-card-subtitle>
    </mat-card-header>
    
    <mat-card-content>
      <!-- Stepper Progress -->
      <mat-stepper [linear]="true" #stepper>
        
        <!-- Step 1: Especialidade -->
        <mat-step [completed]="!!bookingForm.get('specialty')?.value">
          <ng-template matStepLabel>Especialidade</ng-template>
          <div class="step-content">
            <h3>Selecione a especialidade</h3>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Especialidade</mat-label>
              <mat-select formControlName="specialty">
                <mat-option *ngFor="let specialty of specialties" [value]="specialty">
                  {{ specialty }}
                </mat-option>
              </mat-select>
            </mat-form-field>
            <div class="step-actions">
              <button mat-raised-button color="primary" 
                      [disabled]="!canProceedToNext()" 
                      (click)="nextStep()">
                Próximo
              </button>
            </div>
          </div>
        </mat-step>
        
        <!-- Step 2: Médico -->
        <mat-step [completed]="!!bookingForm.get('doctor')?.value">
          <ng-template matStepLabel>Médico</ng-template>
          <div class="step-content">
            <h3>Escolha o médico</h3>
            <div *ngIf="loadingDoctors" class="loading">
              <mat-spinner diameter="50"></mat-spinner>
              <p>Carregando médicos...</p>
            </div>
            <mat-form-field appearance="outline" class="full-width" *ngIf="!loadingDoctors">
              <mat-label>Médico</mat-label>
              <mat-select formControlName="doctor" (selectionChange)="onDoctorChange($event.value)">
                <mat-option *ngFor="let doctor of doctors" [value]="doctor.id">
                  {{ doctor.name }} - CRM {{ doctor.crm }}
                </mat-option>
              </mat-select>
            </mat-form-field>
            <div class="step-actions">
              <button mat-button (click)="previousStep()">Voltar</button>
              <button mat-raised-button color="primary" 
                      [disabled]="!canProceedToNext()" 
                      (click)="nextStep()">
                Próximo
              </button>
            </div>
          </div>
        </mat-step>
        
        <!-- Step 3: Data -->
        <mat-step [completed]="!!bookingForm.get('date')?.value">
          <ng-template matStepLabel>Data</ng-template>
          <div class="step-content">
            <h3>Selecione a data</h3>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Data da consulta</mat-label>
              <input matInput 
                     [matDatepicker]="picker" 
                     formControlName="date"
                     [min]="minDate"
                     [max]="maxDate"
                     (dateChange)="onDateChange($event.value)">
              <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
            </mat-form-field>
            <div class="step-actions">
              <button mat-button (click)="previousStep()">Voltar</button>
              <button mat-raised-button color="primary" 
                      [disabled]="!canProceedToNext()" 
                      (click)="nextStep()">
                Próximo
              </button>
            </div>
          </div>
        </mat-step>
        
        <!-- Step 4: Horário -->
        <mat-step [completed]="!!bookingForm.get('time')?.value">
          <ng-template matStepLabel>Horário</ng-template>
          <div class="step-content">
            <h3>Escolha o horário</h3>
            <div *ngIf="loadingSlots" class="loading">
              <mat-spinner diameter="50"></mat-spinner>
              <p>Carregando horários disponíveis...</p>
            </div>
            <div *ngIf="!loadingSlots && availableSlots.length > 0" class="time-slots">
              <mat-chip-listbox formControlName="time">
                <mat-chip-option *ngFor="let slot of availableSlots" 
                                 [value]="slot.timeFormatted"
                                 [selected]="bookingForm.get('time')?.value === slot.timeFormatted">
                  {{ slot.timeFormatted }}
                </mat-chip-option>
              </mat-chip-listbox>
            </div>
            <div *ngIf="!loadingSlots && availableSlots.length === 0" class="no-slots">
              <mat-icon>event_busy</mat-icon>
              <p>Não há horários disponíveis para esta data</p>
              <button mat-button color="primary" (click)="previousStep()">
                Escolher outra data
              </button>
            </div>
            <div class="step-actions">
              <button mat-button (click)="previousStep()">Voltar</button>
              <button mat-raised-button color="primary" 
                      [disabled]="!canProceedToNext()" 
                      (click)="nextStep()">
                Próximo
              </button>
            </div>
          </div>
        </mat-step>
        
        <!-- Step 5: Confirmação -->
        <mat-step>
          <ng-template matStepLabel>Confirmação</ng-template>
          <div class="step-content">
            <h3>Motivo da consulta</h3>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Motivo da consulta</mat-label>
              <textarea matInput 
                        formControlName="reason"
                        rows="3"
                        maxlength="500"
                        placeholder="Descreva brevemente o motivo da sua consulta"></textarea>
              <mat-hint align="end">{{ bookingForm.get('reason')?.value?.length || 0 }}/500</mat-hint>
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Observações (opcional)</mat-label>
              <textarea matInput 
                        formControlName="notes"
                        rows="2"
                        maxlength="1000"
                        placeholder="Informações adicionais"></textarea>
              <mat-hint align="end">{{ bookingForm.get('notes')?.value?.length || 0 }}/1000</mat-hint>
            </mat-form-field>
            
            <!-- Resumo -->
            <mat-card class="summary-card">
              <h4>Resumo do Agendamento</h4>
              <div class="summary-item">
                <mat-icon>person</mat-icon>
                <span>{{ selectedDoctor?.name }}</span>
              </div>
              <div class="summary-item">
                <mat-icon>medical_services</mat-icon>
                <span>{{ bookingForm.get('specialty')?.value }}</span>
              </div>
              <div class="summary-item">
                <mat-icon>event</mat-icon>
                <span>{{ bookingForm.get('date')?.value | date:'dd/MM/yyyy' }}</span>
              </div>
              <div class="summary-item">
                <mat-icon>schedule</mat-icon>
                <span>{{ bookingForm.get('time')?.value }}</span>
              </div>
            </mat-card>
            
            <div class="step-actions">
              <button mat-button (click)="previousStep()">Voltar</button>
              <button mat-raised-button color="primary" 
                      [disabled]="!canProceedToNext() || submitting"
                      (click)="onSubmit()">
                <mat-spinner *ngIf="submitting" diameter="20"></mat-spinner>
                <span *ngIf="!submitting">Confirmar Agendamento</span>
              </button>
            </div>
          </div>
        </mat-step>
        
      </mat-stepper>
    </mat-card-content>
  </mat-card>
</div>
```

---

## ✅ Critérios de Sucesso

### Técnicos
- [ ] API endpoints implementados e testados
- [ ] Validação de disponibilidade em tempo real
- [ ] Proteção contra race conditions (double-booking)
- [ ] Validações de negócio (horários, antecedência)
- [ ] Testes unitários (> 80% coverage)
- [ ] Testes de integração (fluxo completo)
- [ ] Performance (< 500ms para buscar slots)

### Funcionais
- [ ] Paciente consegue agendar consulta online
- [ ] Sistema mostra apenas horários realmente disponíveis
- [ ] Confirmação enviada por email/WhatsApp
- [ ] Reagendamento com pelo menos 2h de antecedência
- [ ] Cancelamento com pelo menos 24h de antecedência
- [ ] Interface intuitiva (< 5 cliques para agendar)

### Negócio
- [ ] Redução de 40-50% em ligações telefônicas
- [ ] 70%+ dos agendamentos feitos online em 3 meses
- [ ] Taxa de erro/abandono < 5%
- [ ] Satisfação do paciente > 8.5/10
- [ ] Redução de conflitos de agendamento (double-booking = 0%)

---

## 🧪 Testes

### Testes Unitários Backend

```csharp
[Fact]
public async Task GetAvailableSlots_ShouldReturnOnlyAvailableSlots()
{
    // Arrange
    var request = new AvailabilityRequestDto 
    { 
        DoctorId = Guid.NewGuid(), 
        Date = DateTime.Today.AddDays(1) 
    };
    
    // Act
    var slots = await _availabilityService.GetAvailableSlotsAsync(request);
    
    // Assert
    Assert.All(slots, slot => Assert.True(slot.IsAvailable));
}

[Fact]
public async Task BookAppointment_WithConflictingSlot_ShouldThrowException()
{
    // Arrange
    var dto = new BookAppointmentDto 
    { 
        DoctorId = Guid.NewGuid(),
        ScheduledDateTime = DateTime.Now.AddDays(1)
    };
    
    // Simular slot já ocupado
    _availabilityService.Setup(x => x.IsSlotAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
        .ReturnsAsync(false);
    
    // Act & Assert
    await Assert.ThrowsAsync<BusinessException>(
        () => _bookingService.BookAppointmentAsync(Guid.NewGuid(), dto)
    );
}
```

### Testes E2E Frontend

```typescript
// appointment-booking.e2e.spec.ts
describe('Appointment Booking Flow', () => {
  it('should complete full booking flow', async () => {
    await page.goto('/appointments/book');
    
    // Step 1: Specialty
    await page.selectOption('[formControlName="specialty"]', 'Cardiologia');
    await page.click('button:has-text("Próximo")');
    
    // Step 2: Doctor
    await page.selectOption('[formControlName="doctor"]', 'Dr. João Silva');
    await page.click('button:has-text("Próximo")');
    
    // Step 3: Date
    await page.fill('[matInput][formControlName="date"]', '2026-02-15');
    await page.click('button:has-text("Próximo")');
    
    // Step 4: Time
    await page.click('mat-chip-option:has-text("14:00")');
    await page.click('button:has-text("Próximo")');
    
    // Step 5: Reason
    await page.fill('[formControlName="reason"]', 'Consulta de rotina');
    await page.click('button:has-text("Confirmar Agendamento")');
    
    // Assert
    await expect(page.locator('.success-snackbar')).toBeVisible();
    await expect(page).toHaveURL(/\/appointments\/[a-f0-9-]+/);
  });
});
```

---

## 📊 Métricas e Monitoramento

### KPIs a Monitorar

1. **Taxa de Conversão**
   - % de pacientes que iniciam e completam agendamento
   - Meta: > 80%

2. **Tempo Médio de Agendamento**
   - Tempo do início ao fim do fluxo
   - Meta: < 3 minutos

3. **Taxa de Erro**
   - % de agendamentos que falham
   - Meta: < 2%

4. **Double-Booking**
   - Agendamentos conflitantes
   - Meta: 0%

5. **Slots Disponíveis vs Ocupados**
   - Taxa de ocupação da agenda
   - Meta: 70-80%

---

## 🚨 Possíveis Problemas e Soluções

### Race Conditions (Double-Booking)

**Problema:** Dois pacientes tentam agendar o mesmo horário simultaneamente.

**Solução:**
```csharp
// Use database locks ou unique constraints
[HttpPost("book")]
[Transaction(IsolationLevel = IsolationLevel.Serializable)]
public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
{
    // Re-check availability dentro da transação
    var isAvailable = await _availabilityService.IsSlotAvailableAsync(
        dto.DoctorId, 
        dto.ScheduledDateTime
    );
    
    if (!isAvailable)
    {
        return Conflict("Horário não disponível");
    }
    
    // Proceed with booking...
}
```

### Performance (Muitos Slots)

**Problema:** Buscar 30 dias de disponibilidade de 10 médicos = muitos dados.

**Solução:**
- Paginar resultados (7 dias por vez)
- Cache de schedules (não mudam frequentemente)
- Lazy loading de slots (carregar conforme navegação)

---

## 📚 Referências

- [PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md)
- [PATIENT_PORTAL_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_GUIDE.md)
- [10-portal-paciente.md](../Plano_Desenvolvimento/fase-2-seguranca-lgpd/10-portal-paciente.md)

---

**Última Atualização:** 26 de Janeiro de 2026  
**Versão:** 1.0  
**Status:** 📖 Documentação Completa - Aguardando Implementação
