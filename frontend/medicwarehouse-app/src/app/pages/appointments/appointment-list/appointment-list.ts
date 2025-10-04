import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { DailyAgenda } from '../../../models/appointment.model';

interface CalendarDay {
  date: Date;
  day: number;
  isCurrentMonth: boolean;
  isToday: boolean;
  hasAppointments: boolean;
  appointmentCount: number;
}

@Component({
  selector: 'app-appointment-list',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './appointment-list.html',
  styleUrl: './appointment-list.scss'
})
export class AppointmentList implements OnInit {
  agenda = signal<DailyAgenda | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedDate = signal<string>(new Date().toISOString().split('T')[0]);
  viewMode = signal<'list' | 'calendar'>('list');
  calendarDays = signal<CalendarDay[]>([]);
  currentMonth = signal<Date>(new Date());

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit(): void {
    this.loadAgenda();
    this.generateCalendar();
  }

  loadAgenda(): void {
    this.isLoading.set(true);
    // Using default clinic ID for demo
    const clinicId = '00000000-0000-0000-0000-000000000001';
    
    this.appointmentService.getDailyAgenda(clinicId, this.selectedDate()).subscribe({
      next: (data) => {
        this.agenda.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar agenda');
        this.isLoading.set(false);
        console.error('Error loading agenda:', error);
      }
    });
  }

  generateCalendar(): void {
    const current = this.currentMonth();
    const year = current.getFullYear();
    const month = current.getMonth();
    
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const prevLastDay = new Date(year, month, 0);
    
    const firstDayOfWeek = firstDay.getDay();
    const daysInMonth = lastDay.getDate();
    const daysInPrevMonth = prevLastDay.getDate();
    
    const days: CalendarDay[] = [];
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    // Previous month days
    for (let i = firstDayOfWeek - 1; i >= 0; i--) {
      const date = new Date(year, month - 1, daysInPrevMonth - i);
      days.push({
        date,
        day: daysInPrevMonth - i,
        isCurrentMonth: false,
        isToday: false,
        hasAppointments: false,
        appointmentCount: 0
      });
    }
    
    // Current month days
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      date.setHours(0, 0, 0, 0);
      days.push({
        date,
        day,
        isCurrentMonth: true,
        isToday: date.getTime() === today.getTime(),
        hasAppointments: false, // Will be updated when loading appointments
        appointmentCount: 0
      });
    }
    
    // Next month days to fill the grid
    const remainingDays = 42 - days.length; // 6 rows * 7 days
    for (let day = 1; day <= remainingDays; day++) {
      const date = new Date(year, month + 1, day);
      days.push({
        date,
        day,
        isCurrentMonth: false,
        isToday: false,
        hasAppointments: false,
        appointmentCount: 0
      });
    }
    
    this.calendarDays.set(days);
  }

  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedDate.set(input.value);
    this.loadAgenda();
  }

  selectCalendarDate(day: CalendarDay): void {
    if (day.isCurrentMonth) {
      const dateStr = day.date.toISOString().split('T')[0];
      this.selectedDate.set(dateStr);
      this.viewMode.set('list');
      this.loadAgenda();
    }
  }

  previousMonth(): void {
    const current = this.currentMonth();
    const newMonth = new Date(current.getFullYear(), current.getMonth() - 1, 1);
    this.currentMonth.set(newMonth);
    this.generateCalendar();
  }

  nextMonth(): void {
    const current = this.currentMonth();
    const newMonth = new Date(current.getFullYear(), current.getMonth() + 1, 1);
    this.currentMonth.set(newMonth);
    this.generateCalendar();
  }

  toggleView(): void {
    this.viewMode.set(this.viewMode() === 'list' ? 'calendar' : 'list');
  }

  getMonthName(): string {
    return this.currentMonth().toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });
  }

  cancelAppointment(id: string): void {
    if (confirm('Tem certeza que deseja cancelar este agendamento?')) {
      this.appointmentService.cancel(id, 'Cancelado pelo usuário').subscribe({
        next: () => {
          this.loadAgenda();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cancelar agendamento');
          console.error('Error canceling appointment:', error);
        }
      });
    }
  }
}
