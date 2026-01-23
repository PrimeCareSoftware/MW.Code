import { Component, HostListener, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClinicSelectionService } from '../../services/clinic-selection.service';
import { UserClinicDto } from '../../models/clinic.model';

@Component({
  selector: 'app-clinic-selector',
  imports: [CommonModule],
  templateUrl: './clinic-selector.html',
  styleUrl: './clinic-selector.scss'
})
export class ClinicSelectorComponent implements OnInit {
  dropdownOpen = signal(false);
  
  // Use computed signals from the service
  availableClinics = computed(() => this.clinicSelectionService.availableClinics());
  currentClinic = computed(() => this.clinicSelectionService.currentClinic());
  
  // Computed clinic name for display
  currentClinicName = computed(() => {
    const clinic = this.currentClinic();
    return clinic?.clinicName || '';
  });

  constructor(private clinicSelectionService: ClinicSelectionService) {}

  ngOnInit(): void {
    // Load user clinics on initialization
    this.clinicSelectionService.getUserClinics().subscribe({
      error: (error) => {
        console.error('Error loading user clinics:', error);
      }
    });
  }

  toggleDropdown(): void {
    this.dropdownOpen.set(!this.dropdownOpen());
  }

  selectClinic(clinicId: string): void {
    this.clinicSelectionService.selectClinic(clinicId).subscribe({
      next: (response) => {
        if (response.success) {
          this.dropdownOpen.set(false);
          // Optionally reload the page or emit an event to update data
          window.location.reload();
        }
      },
      error: (error) => {
        console.error('Error switching clinic:', error);
        alert('Erro ao trocar de clínica. Por favor, tente novamente.');
      }
    });
  }

  isCurrentClinic(clinicId: string): boolean {
    const current = this.currentClinic();
    return current?.clinicId === clinicId;
  }

  shouldShowSelector(): boolean {
    return this.availableClinics().length > 1;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (typeof document !== 'undefined' && event?.target) {
      const target = event.target as HTMLElement;
      if (!target.closest('.clinic-selector')) {
        this.dropdownOpen.set(false);
      }
    }
  }
}
