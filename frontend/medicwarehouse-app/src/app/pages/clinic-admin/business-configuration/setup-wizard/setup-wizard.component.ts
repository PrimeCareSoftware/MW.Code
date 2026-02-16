import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FocusTrapDirective } from '../../../../shared/accessibility/directives/focus-trap.directive';
import { 
  BusinessType,
  ProfessionalSpecialty
} from '../../../../services/business-configuration.service';

interface ConfigurationTemplate {
  id: string;
  name: string;
  description: string;
  icon: string;
  businessType: BusinessType;
  specialty: ProfessionalSpecialty;
  features: {
    electronicPrescription: boolean;
    labIntegration: boolean;
    vaccineControl: boolean;
    inventoryManagement: boolean;
    multiRoom: boolean;
    receptionQueue: boolean;
    financialModule: boolean;
    healthInsurance: boolean;
    telemedicine: boolean;
    homeVisit: boolean;
    groupSessions: boolean;
    publicProfile: boolean;
    onlineBooking: boolean;
    patientReviews: boolean;
    biReports: boolean;
    apiAccess: boolean;
    whiteLabel: boolean;
  };
}

export interface WizardConfiguration {
  businessType: BusinessType;
  primarySpecialty: ProfessionalSpecialty;
  openingTime: string;
  closingTime: string;
  appointmentDurationMinutes: number;
  allowEmergencySlots: boolean;
  enableOnlineAppointmentScheduling: boolean;
  features?: {
    electronicPrescription: boolean;
    labIntegration: boolean;
    vaccineControl: boolean;
    inventoryManagement: boolean;
    multiRoom: boolean;
    receptionQueue: boolean;
    financialModule: boolean;
    healthInsurance: boolean;
    telemedicine: boolean;
    homeVisit: boolean;
    groupSessions: boolean;
    publicProfile: boolean;
    onlineBooking: boolean;
    patientReviews: boolean;
    biReports: boolean;
    apiAccess: boolean;
    whiteLabel: boolean;
  };
}

@Component({
  selector: 'app-setup-wizard',
  standalone: true,
  imports: [CommonModule, FormsModule, FocusTrapDirective],
  templateUrl: './setup-wizard.component.html',
  styleUrls: ['./setup-wizard.component.scss']
})
export class SetupWizardComponent {
  @Output() complete = new EventEmitter<WizardConfiguration>();
  @Output() cancel = new EventEmitter<void>();

  currentStep = 0;
  totalSteps = 3;

  BusinessType = BusinessType;
  ProfessionalSpecialty = ProfessionalSpecialty;

  // Configuration data
  selectedTemplate: ConfigurationTemplate | null = null;
  businessType: BusinessType = BusinessType.SmallClinic;
  specialty: ProfessionalSpecialty = ProfessionalSpecialty.Medico;
  openingTime = '08:00';
  closingTime = '18:00';
  appointmentDurationMinutes = 30;
  allowEmergencySlots = true;
  enableOnlineAppointmentScheduling = true;

  templates: ConfigurationTemplate[] = [
    {
      id: 'solo-doctor',
      name: 'Médico Autônomo',
      description: 'Configuração ideal para médicos que atuam sozinhos',
      icon: '👨‍⚕️',
      businessType: BusinessType.SoloPractitioner,
      specialty: ProfessionalSpecialty.Medico,
      features: {
        electronicPrescription: true,
        labIntegration: true,
        vaccineControl: false,
        inventoryManagement: false,
        multiRoom: false,
        receptionQueue: false,
        financialModule: true,
        healthInsurance: true,
        telemedicine: true,
        homeVisit: false,
        groupSessions: false,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: false,
        apiAccess: false,
        whiteLabel: false
      }
    },
    {
      id: 'psychologist',
      name: 'Psicólogo',
      description: 'Ideal para profissionais de saúde mental',
      icon: '🧠',
      businessType: BusinessType.SoloPractitioner,
      specialty: ProfessionalSpecialty.Psicologo,
      features: {
        electronicPrescription: false,
        labIntegration: false,
        vaccineControl: false,
        inventoryManagement: false,
        multiRoom: false,
        receptionQueue: false,
        financialModule: true,
        healthInsurance: false,
        telemedicine: true,
        homeVisit: false,
        groupSessions: true,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: false,
        apiAccess: false,
        whiteLabel: false
      }
    },
    {
      id: 'small-clinic',
      name: 'Clínica Pequena',
      description: 'Para clínicas com 2-5 profissionais',
      icon: '🏥',
      businessType: BusinessType.SmallClinic,
      specialty: ProfessionalSpecialty.Medico,
      features: {
        electronicPrescription: true,
        labIntegration: true,
        vaccineControl: true,
        inventoryManagement: true,
        multiRoom: true,
        receptionQueue: true,
        financialModule: true,
        healthInsurance: true,
        telemedicine: true,
        homeVisit: true,
        groupSessions: false,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: false,
        apiAccess: false,
        whiteLabel: false
      }
    },
    {
      id: 'dental-clinic',
      name: 'Clínica Odontológica',
      description: 'Configuração para dentistas e ortodontistas',
      icon: '🦷',
      businessType: BusinessType.SmallClinic,
      specialty: ProfessionalSpecialty.Dentista,
      features: {
        electronicPrescription: true,
        labIntegration: true,
        vaccineControl: false,
        inventoryManagement: true,
        multiRoom: true,
        receptionQueue: true,
        financialModule: true,
        healthInsurance: true,
        telemedicine: false,
        homeVisit: false,
        groupSessions: false,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: false,
        apiAccess: false,
        whiteLabel: false
      }
    },
    {
      id: 'medium-clinic',
      name: 'Clínica Média/Grande',
      description: 'Para clínicas com 6+ profissionais',
      icon: '🏢',
      businessType: BusinessType.MediumClinic,
      specialty: ProfessionalSpecialty.Medico,
      features: {
        electronicPrescription: true,
        labIntegration: true,
        vaccineControl: true,
        inventoryManagement: true,
        multiRoom: true,
        receptionQueue: true,
        financialModule: true,
        healthInsurance: true,
        telemedicine: true,
        homeVisit: true,
        groupSessions: false,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: true,
        apiAccess: false,
        whiteLabel: false
      }
    },
    {
      id: 'custom',
      name: 'Configuração Personalizada',
      description: 'Configure manualmente todas as opções',
      icon: '⚙️',
      businessType: BusinessType.SmallClinic,
      specialty: ProfessionalSpecialty.Medico,
      features: {
        electronicPrescription: true,
        labIntegration: true,
        vaccineControl: false,
        inventoryManagement: true,
        multiRoom: true,
        receptionQueue: true,
        financialModule: true,
        healthInsurance: true,
        telemedicine: true,
        homeVisit: false,
        groupSessions: false,
        publicProfile: true,
        onlineBooking: true,
        patientReviews: true,
        biReports: false,
        apiAccess: false,
        whiteLabel: false
      }
    }
  ];

  appointmentDurationOptions = [
    { value: 15, label: '15 minutos' },
    { value: 30, label: '30 minutos' },
    { value: 45, label: '45 minutos' },
    { value: 60, label: '60 minutos' }
  ];

  selectTemplate(template: ConfigurationTemplate): void {
    this.selectedTemplate = template;
    this.businessType = template.businessType;
    this.specialty = template.specialty;
  }

  nextStep(): void {
    if (this.currentStep < this.totalSteps - 1) {
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  canProceed(): boolean {
    switch (this.currentStep) {
      case 0:
        return this.selectedTemplate !== null;
      case 1:
        // Schedule validation - ensure both times are valid and opening < closing
        const opening = this.parseTimeToMinutes(this.openingTime);
        const closing = this.parseTimeToMinutes(this.closingTime);
        return opening >= 0 && closing >= 0 && opening < closing;
      case 2:
        return true;
      default:
        return false;
    }
  }

  private parseTimeToMinutes(time: string): number {
    // Returns total minutes from "HH:mm" format, or -1 for invalid input
    if (!time) return -1;
    const parts = time.split(':');
    if (parts.length >= 2) {
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1], 10);
      if (!isNaN(hours) && !isNaN(minutes) && hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59) {
        return hours * 60 + minutes;
      }
    }
    return -1; // Invalid time format
  }

  finish(): void {
    const config: WizardConfiguration = {
      businessType: this.businessType,
      primarySpecialty: this.specialty,
      openingTime: this.openingTime,
      closingTime: this.closingTime,
      appointmentDurationMinutes: this.appointmentDurationMinutes,
      allowEmergencySlots: this.allowEmergencySlots,
      enableOnlineAppointmentScheduling: this.enableOnlineAppointmentScheduling,
      features: this.selectedTemplate ? this.selectedTemplate.features : undefined
    };
    this.complete.emit(config);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  getProgressPercentage(): number {
    // Convert 0-indexed currentStep to 1-indexed for percentage (e.g., step 0 = 33%, step 1 = 66%, step 2 = 100%)
    return ((this.currentStep + 1) / this.totalSteps) * 100;
  }
}
