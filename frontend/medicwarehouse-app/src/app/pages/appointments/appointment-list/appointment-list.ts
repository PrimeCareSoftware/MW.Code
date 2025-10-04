import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { DailyAgenda } from '../../../models/appointment.model';

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

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit(): void {
    this.loadAgenda();
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

  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedDate.set(input.value);
    this.loadAgenda();
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
