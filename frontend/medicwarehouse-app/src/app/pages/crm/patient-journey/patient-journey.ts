import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';
import { PatientJourneyService } from '../../../services/crm';
import { PatientJourney as PatientJourneyModel } from '../../../models/crm';

@Component({
  selector: 'app-patient-journey',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './patient-journey.html',
  styleUrl: './patient-journey.scss'
})
export class PatientJourney implements OnInit {
  journeys = signal<PatientJourneyModel[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private patientJourneyService: PatientJourneyService) {}

  ngOnInit(): void {
    this.loadJourneys();
  }

  loadJourneys(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.journeys.set([]);
    this.isLoading.set(false);
    this.errorMessage.set('Para visualizar jornadas, selecione um paciente específico.');
  }

  onViewAnalytics(): void {
    console.log('Analytics clicked');
    alert('Funcionalidade "Analytics" será implementada em breve.');
  }
}
