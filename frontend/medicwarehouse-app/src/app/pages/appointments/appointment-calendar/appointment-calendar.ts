import { Component, OnInit, signal, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, NavigationEnd } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, filter } from 'rxjs/operators';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { Appointment, Professional, BlockedTimeSlot, BlockedTimeSlotTypeLabels, RecurringDeleteScope, RecurringDeleteScopeLabels } from '../../../models/appointment.model';
import { Auth } from '../../../services/auth';
import { HelpButtonComponent } from '../../../shared/help-button/help-button';
import { ScheduleBlockingDialogComponent } from '../schedule-blocking-dialog/schedule-blocking-dialog.component';
import { RecurrenceActionDialogComponent, RecurrenceActionDialogResult } from '../recurrence-action-dialog/recurrence-action-dialog.component';
import { ClinicAdminService } from '../../../services/clinic-admin.service';

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
  // Route constants for calendar navigation
  private readonly CALENDAR_ROUTES = ['/appointments', '/appointments/calendar'];
  
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
  
  // Subscription for router events (for cleanup)
  private routerSubscription?: Subscription;
  
  // Track if initial load has completed to avoid duplicate loading
  private initialLoadComplete = false;
  
  // Clinic ID will be retrieved from authenticated user
  clinicId: string | null = null;
  
  // Clinic hours loaded from configuration
  private clinicOpeningHour: number = 8;  // Default fallback
  private clinicClosingHour: number = 18; // Default fallback

  constructor(
    private appointmentService: AppointmentService,
    private router: Router,
    private auth: Auth,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
    private clinicAdminService: ClinicAdminService
  ) {}

  ngOnInit(): void {
    // Get clinicId from authenticated user
    this.clinicId = this.auth.getClinicId();
    
    if (!this.clinicId) {
      this.errorMessage.set('Para visualizar o calendário de consultas, você precisa estar associado a uma clínica. Entre em contato com o administrador do sistema para configurar sua associação com uma clínica.');
      return;
    }
    
    // Load clinic configuration to get opening/closing hours
    this.loadClinicConfiguration();
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
    
    // Listen for navigation events to reload data when returning to calendar
    // This fixes the issue where new appointments don't appear without F5
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      filter((event: NavigationEnd) => this.CALENDAR_ROUTES.includes(event.url))
    ).subscribe(() => {
      // Skip reload on initial navigation (first load)
      if (this.initialLoadComplete) {
        this.loadWeekAppointments();
      }
    });
    
    this.loadWeekAppointments().then(() => {
      this.initialLoadComplete = true;
    });
  }

  ngOnDestroy(): void {
    this.filterChange$.complete();
    // Cleanup router subscription to prevent memory leaks
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  /**
   * Parse ISO date string (YYYY-MM-DD) as local date without timezone conversion.
   * Fixes the d-1 bug where new Date('2024-01-15') interprets as UTC and converts to local timezone.
   */
  private parseLocalDate(dateString: string): Date {
    // Validate date string format
    if (!dateString || typeof dateString !== 'string') {
      console.warn('Invalid date string provided to parseLocalDate:', dateString);
      return new Date(); // Fallback to current date
    }
    
    const parts = dateString.split('-');
    if (parts.length !== 3) {
      console.warn('Date string is not in YYYY-MM-DD format:', dateString);
      return new Date(); // Fallback to current date
    }
    
    const [year, month, day] = parts.map(Number);
    
    // Validate parsed values
    if (isNaN(year) || isNaN(month) || isNaN(day)) {
      console.warn('Date string contains non-numeric values:', dateString);
      return new Date(); // Fallback to current date
    }
    
    return new Date(year, month - 1, day);
  }

  /**
   * Format a Date object to YYYY-MM-DD string using local timezone.
   * Avoids timezone conversion issues with toISOString().
   */
  private formatLocalDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  getWeekStart(date: Date): Date {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day; // Sunday is 0
    return new Date(d.setDate(diff));
  }

  loadClinicConfiguration(): void {
    this.clinicAdminService.getClinicInfo().subscribe({
      next: (clinicInfo) => {
        // Parse opening and closing times from TimeSpan string format (HH:mm:ss)
        const openingResult = this.parseTimeString(clinicInfo.openingTime);
        if (openingResult !== null) {
          this.clinicOpeningHour = openingResult.hour;
        }
        
        const closingResult = this.parseTimeString(clinicInfo.closingTime);
        if (closingResult !== null) {
          // For closing time, we want to show appointment slots up to the closing time
          // The loop in generateTimeSlots() uses "hour < endHour", so we need to add 1
          // to include the closing hour. E.g., if closing time is 22:00, we want to show
          // slots 21:00 and 21:30, but also 22:00 (end of day), so endHour should be 23.
          // For 22:30, we want 21:00, 21:30, 22:00, 22:30, so endHour should also be 23.
          // Cap at 24 to prevent invalid hours.
          this.clinicClosingHour = Math.min(closingResult.hour + 1, 24);
        }
        
        // Generate time slots with loaded configuration
        this.generateTimeSlots();
      },
      error: (error) => {
        console.error('Error loading clinic configuration, using default hours:', error);
        // Use default hours (8-18) on error
        this.generateTimeSlots();
      }
    });
  }

  private parseTimeString(timeString: string | undefined): { hour: number; minute: number } | null {
    if (!timeString) {
      return null;
    }
    
    // Validate HH:mm:ss or HH:mm format
    const timeRegex = /^([0-1]?[0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9])?$/;
    if (!timeRegex.test(timeString)) {
      return null;
    }
    
    const parts = timeString.split(':');
    const parsedHour = parseInt(parts[0], 10);
    const parsedMinute = parts.length >= 2 ? parseInt(parts[1], 10) : 0;
    
    if (isNaN(parsedHour) || parsedHour < 0 || parsedHour >= 24) {
      return null;
    }
    if (isNaN(parsedMinute) || parsedMinute < 0 || parsedMinute >= 60) {
      return null;
    }
    
    return { hour: parsedHour, minute: parsedMinute };
  }

  generateTimeSlots(): void {
    const slots: TimeSlot[] = [];
    const startHour = this.clinicOpeningHour;
    const endHour = this.clinicClosingHour;
    
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
      // Use optimized week agenda endpoint instead of 7 separate daily requests
      const startDateStr = this.formatLocalDate(weekStart);
      const endDateStr = this.formatLocalDate(weekEnd);
      
      const [weekAgenda, blockedSlots] = await Promise.all([
        this.appointmentService.getWeekAgenda(clinicId, startDateStr, endDateStr, doctorId || undefined).toPromise(),
        this.appointmentService.getBlockedTimeSlotsByDateRange(
          startDateStr,
          endDateStr,
          clinicId
        ).toPromise()
      ]);
      
      // Clear existing appointments
      days.forEach(day => {
        day.appointments = [];
        day.blockedSlots = [];
      });
      
      // Distribute appointments to their respective days
      if (weekAgenda && weekAgenda.appointments) {
        weekAgenda.appointments.forEach(appointment => {
          // Parse date string as local date to avoid timezone conversion issues (d-1 bug)
          const appointmentDate = this.parseLocalDate(appointment.scheduledDate);
          appointmentDate.setHours(0, 0, 0, 0); // Ensure time is at midnight for comparison
          const dayIndex = days.findIndex(d => {
            const dayDate = new Date(d.date);
            dayDate.setHours(0, 0, 0, 0);
            return dayDate.getTime() === appointmentDate.getTime();
          });
          
          if (dayIndex >= 0) {
            days[dayIndex].appointments.push(appointment);
          }
        });
      }
      
      // Update day columns with blocked slots
      if (blockedSlots) {
        blockedSlots.forEach(block => {
          // Parse date string as local date to avoid timezone conversion issues (d-1 bug)
          const blockDate = this.parseLocalDate(block.date);
          blockDate.setHours(0, 0, 0, 0); // Ensure time is at midnight for comparison
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
      const dateStr = this.formatLocalDate(slot.dayColumn.date);
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
    
    // If this is a recurring block, show dialog with 3 scope options
    if (blockedSlot.isRecurring && blockedSlot.recurringSeriesId) {
      const dialogRef = this.dialog.open(RecurrenceActionDialogComponent, {
        width: '550px',
        data: {
          action: 'delete',
          blockDate: new Date(blockedSlot.date)
        }
      });

      dialogRef.afterClosed().subscribe((result: RecurrenceActionDialogResult | null) => {
        if (result) {
          this.performDelete(blockedSlot.id, result.scope, result.reason);
        }
      });
    } else {
      // Non-recurring block, use simple confirmation
      const confirmDelete = confirm('Tem certeza que deseja remover este bloqueio?');
      if (confirmDelete) {
        this.performDelete(blockedSlot.id, RecurringDeleteScope.ThisOccurrence);
      }
    }
  }

  private performDelete(blockId: string, scope: RecurringDeleteScope, reason?: string): void {
    this.appointmentService.deleteBlockedTimeSlot(blockId, scope, reason).subscribe({
      next: () => {
        const messages: { [key: number]: string } = {
          [RecurringDeleteScope.ThisOccurrence]: 'Bloqueio removido com sucesso',
          [RecurringDeleteScope.ThisAndFuture]: 'Bloqueio e ocorrências futuras removidos com sucesso',
          [RecurringDeleteScope.AllInSeries]: 'Série completa removida com sucesso'
        };

        const message = messages[scope] || 'Bloqueio removido com sucesso';
        
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
