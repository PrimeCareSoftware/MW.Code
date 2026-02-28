import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientJourneyService } from '../../../services/crm';
import { PatientJourney as PatientJourneyModel } from '../../../models/crm';

@Component({
  selector: 'app-patient-journey',
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-journey.html',
  styleUrl: './patient-journey.scss'
})
export class PatientJourney implements OnInit {
  journeys = signal<PatientJourneyModel[]>([]);
  isLoading = signal<boolean>(false);
  // This holds informational messages, not errors, since this page requires patient selection
  infoMessage = signal<string>('');

  constructor(private patientJourneyService: PatientJourneyService) {}

  ngOnInit(): void {
    this.loadJourneys();
  }

  loadJourneys(): void {
    this.isLoading.set(true);
    this.infoMessage.set('');
    
    // Patient journey requires a patient ID to function.
    // This informational message guides the user on next steps.
    this.journeys.set([]);
    this.isLoading.set(false);
    this.infoMessage.set('Para visualizar jornadas, selecione um paciente específico.');
  }

  onViewAnalytics(): void {
    console.log('Analytics clicked');
    alert('Funcionalidade "Analytics" será implementada em breve.');
  }
}
