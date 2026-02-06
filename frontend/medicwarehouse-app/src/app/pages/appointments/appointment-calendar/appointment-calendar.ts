import { Component, OnInit, signal, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { Appointment, Professional, BlockedTimeSlot, BlockedTimeSlotTypeLabels } from '../../../models/appointment.model';
import { Auth } from '../../../services/auth';
import { HelpButtonComponent } from '../../../shared/help-button/help-button';
import { ScheduleBlockingDialogComponent } from '../schedule-blocking-dialog/schedule-blocking-dialog.component';
import { RecurrenceActionDialogComponent } from '../recurrence-action-dialog/recurrence-action-dialog.component';

interface TimeSlot {
  time: string;
  hour: number;
  minute: number;
}

interface DayColumn {
  date: Date;
  dayName: string;
  dayNumber: number;
  isToday: boolean;
  appointments: Appointment[];
  blockedSlots: BlockedTimeSlot[];
}

interface CalendarSlot {
  timeSlot: TimeSlot;
  dayColumn: DayColumn;
  appointment: Appointment | null;
  blockedSlot: BlockedTimeSlot | null;
  isAvailable: boolean;
  isBlocked: boolean;
}

@Component({
  selector: 'app-appointment-calendar',
  standalone: true,
  imports: [CommonModule, RouterLink, Navbar, HelpButtonComponent],
  templateUrl: './appointment-calendar.html',
  styleUrl: './appointment-calendar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppointmentCalendar implements OnInit, OnDestroy {
  currentWeekStart = signal<Date>(this.getWeekStart(new Date()));
  weekDays = signal<DayColumn[]>([]);
  timeSlots = signal<TimeSlot[]>([]);
  calendarGrid = signal<CalendarSlot[][]>([]);
  
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedDoctorId = signal<string | null>(null);
  professionals = signal<Professional[]>([]);
  
  // Subject for debounced filter changes
  private filterChange$ = new Subject<{ date?: Date; professionalId?: string | null }>();
  
  // Clinic ID will be retrieved from authenticated user
  clinicId: string | null = null;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router,
    private auth: Auth,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Get clinicId from authenticated user
    this.clinicId = this.auth.getClinicId();
    
    if (!this.clinicId) {
      this.errorMessage.set('Para visualizar o calendário de consultas, você precisa estar associado a uma clínica. Entre em contato com o administrador do sistema para configurar sua associação com uma clínica.');
      return;
    }
    
    this.generateTimeSlots();
    this.generateWeekDays();
    this.loadProfessionals();
    
    // Set up debounced filter changes (300ms debounce)
    this.filterChange$.pipe(
      debounceTime(300),
      distinctUntilChanged((prev, curr) => 
        prev.date?.getTime() === curr.date?.getTime() && 
        prev.professionalId === curr.professionalId
      ),
      switchMap(() => {
        this.isLoading.set(true);
        this.cdr.markForCheck();
        return Promise.resolve(this.loadWeekAppointments());
      })
    ).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
        this.isLoading.set(false);
        this.cdr.markForCheck();
      }
    });
    
    this.loadWeekAppointments();
  }

  ngOnDestroy(): void {
    this.filterChange$.complete();
  }

  getWeekStart(date: Date): Date {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day; // Sunday is 0
    return new Date(d.setDate(diff));
  }

  generateTimeSlots(): void {
    const slots: TimeSlot[] = [];
    const startHour = 8; // 8 AM
    const endHour = 18; // 6 PM
    
    for (let hour = startHour; hour < endHour; hour++) {
      for (let minute = 0; minute < 60; minute += 30) {
        const hourStr = hour.toString().padStart(2, '0');
        const minuteStr = minute.toString().padStart(2, '0');
        slots.push({
          time: `${hourStr}:${minuteStr}`,
          hour,
          minute
        });
      }
    }
    
    this.timeSlots.set(slots);
  }

  generateWeekDays(): void {
    const weekStart = this.currentWeekStart();
    const days: DayColumn[] = [];
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    const dayNames = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];
    
    for (let i = 0; i < 7; i++) {
      const date = new Date(weekStart);
      date.setDate(weekStart.getDate() + i);
      date.setHours(0, 0, 0, 0);
      
      days.push({
        date,
        dayName: dayNames[i],
        dayNumber: date.getDate(),
        isToday: date.getTime() === today.getTime(),
        appointments: [],
        blockedSlots: []
      });
    }
    
    this.weekDays.set(days);
  }

  async loadWeekAppointments(): Promise<void> {
    if (!this.clinicId) {
      this.errorMessage.set('ID da clínica não disponível');
      return;
    }
    
    this.isLoading.set(true);
    const weekStart = this.currentWeekStart();
    const weekEnd = new Date(weekStart);
    weekEnd.setDate(weekStart.getDate() + 6);
    const days = this.weekDays();
    const clinicId = this.clinicId; // Store in local variable for type safety
    const doctorId = this.selectedDoctorId(); // Get selected doctor filter
    
    try {
      // Load appointments for each day
      const appointmentPromises = days.map(day => {
        const dateStr = day.date.toISOString().split('T')[0];
        return this.appointmentService.getDailyAgenda(clinicId, dateStr, doctorId || undefined).toPromise();
      });
      
      // Load blocked slots for the week
      const startDateStr = weekStart.toISOString().split('T')[0];
      const endDateStr = weekEnd.toISOString().split('T')[0];
      const blockedSlotsPromise = this.appointmentService.getBlockedTimeSlotsByDateRange(
        startDateStr,
        endDateStr,
        clinicId
      ).toPromise();
      
      const [appointmentResults, blockedSlots] = await Promise.all([
        Promise.all(appointmentPromises),
        blockedSlotsPromise
      ]);
      
      // Update day columns with appointments
      appointmentResults.forEach((agenda, index) => {
        if (agenda && agenda.appointments) {
          days[index].appointments = agenda.appointments;
        }
      });
      
      // Update day columns with blocked slots
      if (blockedSlots) {
        blockedSlots.forEach(block => {
          const blockDate = new Date(block.date);
          blockDate.setHours(0, 0, 0, 0);
          const dayIndex = days.findIndex(d => {
            const dayDate = new Date(d.date);
            dayDate.setHours(0, 0, 0, 0);
            return dayDate.getTime() === blockDate.getTime();
          });
          
          if (dayIndex >= 0) {
            // Filter by professional if doctor filter is active
            if (!doctorId || !block.professionalId || block.professionalId === doctorId) {
              days[dayIndex].blockedSlots.push(block);
            }
          }
        });
      }
      
      this.weekDays.set([...days]);
      this.generateCalendarGrid();
      this.isLoading.set(false);
      this.cdr.markForCheck(); // Notify change detection
    } catch (error) {
      console.error('Error loading appointments:', error);
      this.errorMessage.set('Erro ao carregar agendamentos');
      this.isLoading.set(false);
      this.cdr.markForCheck(); // Notify change detection
    }
  }

  loadProfessionals(): void {
    this.appointmentService.getProfessionals().subscribe({
      next: (professionals) => {
        this.professionals.set(professionals);
      },
      error: (error) => {
        console.error('Error loading professionals:', error);
        // Don't show error to user, just log it
      }
    });
  }

  onDoctorFilterChange(doctorId: string | null): void {
    this.selectedDoctorId.set(doctorId);
    // Emit filter change (will be debounced)
    this.filterChange$.next({ professionalId: doctorId });
  }

  generateCalendarGrid(): void {
    const timeSlots = this.timeSlots();
    const days = this.weekDays();
    const grid: CalendarSlot[][] = [];
    
    timeSlots.forEach(timeSlot => {
      const row: CalendarSlot[] = [];
      
      days.forEach(day => {
        const appointment = this.findAppointmentForSlot(day.appointments, timeSlot);
        const blockedSlot = this.findBlockedSlotForSlot(day.blockedSlots, timeSlot);
        
        row.push({
          timeSlot,
          dayColumn: day,
          appointment,
          blockedSlot,
          isAvailable: !appointment && !blockedSlot,
          isBlocked: !!blockedSlot
        });
      });
      
      grid.push(row);
    });
    
    this.calendarGrid.set(grid);
  }

  findAppointmentForSlot(appointments: Appointment[], timeSlot: TimeSlot): Appointment | null {
    return appointments.find(apt => {
      const aptTime = apt.scheduledTime;
      const [hours, minutes] = aptTime.split(':').map(Number);
      return hours === timeSlot.hour && minutes === timeSlot.minute;
    }) || null;
  }

  findBlockedSlotForSlot(blockedSlots: BlockedTimeSlot[], timeSlot: TimeSlot): BlockedTimeSlot | null {
    return blockedSlots.find(block => {
      const startParts = block.startTime.split(':');
      const endParts = block.endTime.split(':');
      const startHour = parseInt(startParts[0]);
      const startMinute = parseInt(startParts[1]);
      const endHour = parseInt(endParts[0]);
      const endMinute = parseInt(endParts[1]);
      
      const slotMinutes = timeSlot.hour * 60 + timeSlot.minute;
      const blockStart = startHour * 60 + startMinute;
      const blockEnd = endHour * 60 + endMinute;
      
      return slotMinutes >= blockStart && slotMinutes < blockEnd;
    }) || null;
  }

  onSlotClick(slot: CalendarSlot): void {
    if (slot.blockedSlot) {
      // Open dialog to view/edit blocked slot
      this.openBlockingDialog(slot.dayColumn.date, slot.timeSlot, slot.blockedSlot);
    } else if (slot.appointment) {
      // Navigate to appointment details or attendance
      if (slot.appointment.status === 'Scheduled' || slot.appointment.status === 'Confirmed') {
        this.router.navigate(['/appointments', slot.appointment.id, 'attendance']);
      }
    } else {
      // Show context menu or create new appointment
      // For now, just navigate to new appointment
      const dateStr = slot.dayColumn.date.toISOString().split('T')[0];
      this.router.navigate(['/appointments/new'], {
        queryParams: {
          date: dateStr,
          time: slot.timeSlot.time
        }
      });
    }
  }

  openBlockingDialog(date?: Date, timeSlot?: TimeSlot, existingBlock?: BlockedTimeSlot): void {
    const dialogRef = this.dialog.open(ScheduleBlockingDialogComponent, {
      width: '600px',
      data: {
        clinicId: this.clinicId,
        date: date,
        timeSlot: timeSlot,
        professionals: this.professionals(),
        blockedSlot: existingBlock
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload the week appointments to show the new/updated block
        this.loadWeekAppointments();
      }
    });
  }

  onBlockSchedule(): void {
    this.openBlockingDialog();
  }

  onDeleteBlock(event: Event, blockedSlot: BlockedTimeSlot): void {
    event.stopPropagation();
    
    // If this is a recurring block, ask user if they want to delete single or series
    if (blockedSlot.isRecurring && blockedSlot.recurringPatternId) {
      const dialogRef = this.dialog.open(RecurrenceActionDialogComponent, {
        width: '500px',
        data: {
          action: 'delete',
          blockDate: new Date(blockedSlot.date)
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          const deleteSeries = result === 'series';
          this.performDelete(blockedSlot.id, deleteSeries);
        }
      });
    } else {
      // Non-recurring block, use simple confirmation
      const confirmDelete = confirm(`Tem certeza que deseja remover este bloqueio?`);
      if (confirmDelete) {
        this.performDelete(blockedSlot.id, false);
      }
    }
  }

  private performDelete(blockId: string, deleteSeries: boolean): void {
    this.appointmentService.deleteBlockedTimeSlot(blockId, deleteSeries).subscribe({
      next: () => {
        const message = deleteSeries ? 'Série de bloqueios removida com sucesso' : 'Bloqueio removido com sucesso';
        this.snackBar.open(message, 'Fechar', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.loadWeekAppointments();
      },
      error: (error) => {
        console.error('Error deleting block:', error);
        this.snackBar.open(
          error.error?.message || 'Erro ao deletar bloqueio',
          'Fechar',
          { duration: 3000 }
        );
        this.cdr.markForCheck(); // Notify change detection
      }
    });
  }

  getBlockTypeLabel(type: number): string {
    return BlockedTimeSlotTypeLabels[type] || 'Bloqueado';
  }

  previousWeek(): void {
    const current = this.currentWeekStart();
    const newStart = new Date(current);
    newStart.setDate(current.getDate() - 7);
    this.currentWeekStart.set(newStart);
    this.generateWeekDays();
    // Emit filter change (will be debounced)
    this.filterChange$.next({ date: newStart });
  }

  nextWeek(): void {
    const current = this.currentWeekStart();
    const newStart = new Date(current);
    newStart.setDate(current.getDate() + 7);
    this.currentWeekStart.set(newStart);
    this.generateWeekDays();
    // Emit filter change (will be debounced)
    this.filterChange$.next({ date: newStart });
  }

  goToToday(): void {
    const newStart = this.getWeekStart(new Date());
    this.currentWeekStart.set(newStart);
    this.generateWeekDays();
    // Emit filter change (will be debounced)
    this.filterChange$.next({ date: newStart });
  }

  getWeekRange(): string {
    const weekStart = this.currentWeekStart();
    const weekEnd = new Date(weekStart);
    weekEnd.setDate(weekStart.getDate() + 6);
    
    return `${weekStart.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' })} - ${weekEnd.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' })}`;
  }

  getAppointmentDuration(appointment: Appointment): number {
    // Return the number of 30-minute slots this appointment spans
    return Math.ceil(appointment.durationMinutes / 30);
  }

  getStatusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  getProfessionalColor(professionalId?: string): string {
    if (!professionalId) return '#2196F3'; // Default blue color
    
    const professional = this.professionals().find(p => p.id === professionalId);
    return professional?.calendarColor || this.getDefaultColorForProfessional(professionalId);
  }

  private getDefaultColorForProfessional(professionalId: string): string {
    // Generate a consistent color based on professional ID
    // Using colors with good contrast against white backgrounds (WCAG AA compliant)
    const colors = [
      '#1976D2', // Blue (darker for better contrast)
      '#388E3C', // Green (darker for better contrast)
      '#F57C00', // Orange (darker for better contrast)
      '#7B1FA2', // Purple (darker for better contrast)
      '#D32F2F', // Red (darker for better contrast)
      '#0097A7', // Cyan (darker for better contrast)
      '#F9A825', // Amber (darker yellow for better contrast)
      '#5D4037', // Brown
      '#455A64', // Blue Grey (darker)
      '#C2185B'  // Pink (darker for better contrast)
    ];
    
    // Simple hash function to get consistent color for ID
    let hash = 0;
    for (let i = 0; i < professionalId.length; i++) {
      hash = professionalId.charCodeAt(i) + ((hash << 5) - hash);
    }
    
    return colors[Math.abs(hash) % colors.length];
  }

  // TrackBy functions for performance optimization
  trackByAppointmentId(index: number, appointment: Appointment): string {
    return appointment.id;
  }

  trackByProfessionalId(index: number, professional: Professional): string {
    return professional.id;
  }

  trackByDayDate(index: number, day: DayColumn): number {
    return day.date.getTime();
  }

  trackBySlotKey(index: number, slot: CalendarSlot): string {
    return `${slot.dayColumn.date.getTime()}_${slot.timeSlot.time}`;
  }

  trackByTimeSlot(index: number, row: CalendarSlot[]): string {
    // Always use the time slot as the unique identifier
    // This should never be null for a valid row
    return row[0]?.timeSlot?.time || '';
  }
}
