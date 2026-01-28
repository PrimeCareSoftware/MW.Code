import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-patient-journey',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './patient-journey.html',
  styleUrl: './patient-journey.scss'
})
export class PatientJourney implements OnInit {
  journeys = signal<any[]>([]);
  isLoading = signal<boolean>(false);

  ngOnInit(): void {
    this.loadJourneys();
  }

  async loadJourneys(): Promise<void> {
    this.isLoading.set(true);
    try {
      // TODO: Integrate with PatientJourneyService when API is connected
      // For now, just set empty array
      this.journeys.set([]);
    } catch (error) {
      console.error('Error loading patient journeys:', error);
    } finally {
      this.isLoading.set(false);
    }
  }
}
