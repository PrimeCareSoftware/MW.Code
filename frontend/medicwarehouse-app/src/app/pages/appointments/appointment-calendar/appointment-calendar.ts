import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { Appointment } from '../../../models/appointment.model';

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
}

interface CalendarSlot {
  timeSlot: TimeSlot;
  dayColumn: DayColumn;
  appointment: Appointment | null;
  isAvailable: boolean;
}

@Component({
  selector: 'app-appointment-calendar',
  standalone: true,
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './appointment-calendar.html',
  styleUrl: './appointment-calendar.scss'
})
export class AppointmentCalendar implements OnInit {
  currentWeekStart = signal<Date>(this.getWeekStart(new Date()));
  weekDays = signal<DayColumn[]>([]);
  timeSlots = signal<TimeSlot[]>([]);
  calendarGrid = signal<CalendarSlot[][]>([]);
  
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedDoctorId = signal<string | null>(null);
  
  // Default clinic ID - in real app, this would come from user context
  clinicId = '00000000-0000-0000-0000-000000000001';

  constructor(
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.generateTimeSlots();
    this.generateWeekDays();
    this.loadWeekAppointments();
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
        appointments: []
      });
    }
    
    this.weekDays.set(days);
  }

  async loadWeekAppointments(): Promise<void> {
    this.isLoading.set(true);
    const weekStart = this.currentWeekStart();
    const days = this.weekDays();
    
    try {
      // Load appointments for each day
      const promises = days.map(day => {
        const dateStr = day.date.toISOString().split('T')[0];
        return this.appointmentService.getDailyAgenda(this.clinicId, dateStr).toPromise();
      });
      
      const results = await Promise.all(promises);
      
      // Update day columns with appointments
      results.forEach((agenda, index) => {
        if (agenda && agenda.appointments) {
          days[index].appointments = this.filterAppointmentsByDoctor(agenda.appointments);
        }
      });
      
      this.weekDays.set([...days]);
      this.generateCalendarGrid();
      this.isLoading.set(false);
    } catch (error) {
      console.error('Error loading appointments:', error);
      this.errorMessage.set('Erro ao carregar agendamentos');
      this.isLoading.set(false);
    }
  }

  filterAppointmentsByDoctor(appointments: Appointment[]): Appointment[] {
    const doctorId = this.selectedDoctorId();
    if (!doctorId) {
      return appointments;
    }
    // Filter by doctor if doctorId is set
    return appointments.filter(apt => {
      // Assuming appointment has doctorId field - needs to be added to model
      return (apt as any).doctorId === doctorId;
    });
  }

  generateCalendarGrid(): void {
    const timeSlots = this.timeSlots();
    const days = this.weekDays();
    const grid: CalendarSlot[][] = [];
    
    timeSlots.forEach(timeSlot => {
      const row: CalendarSlot[] = [];
      
      days.forEach(day => {
        const appointment = this.findAppointmentForSlot(day.appointments, timeSlot);
        
        row.push({
          timeSlot,
          dayColumn: day,
          appointment,
          isAvailable: !appointment
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

  onSlotClick(slot: CalendarSlot): void {
    if (slot.appointment) {
      // Navigate to appointment details or attendance
      if (slot.appointment.status === 'Scheduled' || slot.appointment.status === 'Confirmed') {
        this.router.navigate(['/appointments', slot.appointment.id, 'attendance']);
      }
    } else {
      // Create new appointment for this slot
      const dateStr = slot.dayColumn.date.toISOString().split('T')[0];
      this.router.navigate(['/appointments/new'], {
        queryParams: {
          date: dateStr,
          time: slot.timeSlot.time
        }
      });
    }
  }

  previousWeek(): void {
    const current = this.currentWeekStart();
    const newStart = new Date(current);
    newStart.setDate(current.getDate() - 7);
    this.currentWeekStart.set(newStart);
    this.generateWeekDays();
    this.loadWeekAppointments();
  }

  nextWeek(): void {
    const current = this.currentWeekStart();
    const newStart = new Date(current);
    newStart.setDate(current.getDate() + 7);
    this.currentWeekStart.set(newStart);
    this.generateWeekDays();
    this.loadWeekAppointments();
  }

  goToToday(): void {
    this.currentWeekStart.set(this.getWeekStart(new Date()));
    this.generateWeekDays();
    this.loadWeekAppointments();
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
}
