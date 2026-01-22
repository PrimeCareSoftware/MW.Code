import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { PublicDisplaySettingsDto } from '../../../models/clinic-admin.model';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-public-display',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './public-display.component.html',
  styleUrl: './public-display.component.scss'
})
export class PublicDisplayComponent implements OnInit {
  private readonly SUCCESS_MESSAGE_TIMEOUT = 5000;

  settingsForm: FormGroup;
  currentSettings = signal<PublicDisplaySettingsDto | null>(null);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  // Clinic types available
  clinicTypes = [
    { value: 'Medical', label: 'Médica' },
    { value: 'Dental', label: 'Odontológica' },
    { value: 'Nutritionist', label: 'Nutricionista' },
    { value: 'Psychology', label: 'Psicologia' },
    { value: 'PhysicalTherapy', label: 'Fisioterapia' },
    { value: 'Veterinary', label: 'Veterinária' },
    { value: 'Other', label: 'Outros' }
  ];

  constructor(
    private fb: FormBuilder,
    private clinicAdminService: ClinicAdminService
  ) {
    this.settingsForm = this.fb.group({
      showOnPublicSite: [false],
      clinicType: ['Medical', Validators.required],
      whatsAppNumber: ['']
    });
  }

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.getPublicDisplaySettings().subscribe({
      next: (data) => {
        this.currentSettings.set(data);
        this.settingsForm.patchValue({
          showOnPublicSite: data.showOnPublicSite,
          clinicType: data.clinicType,
          whatsAppNumber: data.whatsAppNumber || ''
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar configurações: ' + (error.error?.message || error.message));
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.settingsForm.valid) {
      this.isSaving.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      const formValue = this.settingsForm.value;
      const request = {
        showOnPublicSite: formValue.showOnPublicSite,
        clinicType: formValue.clinicType,
        whatsAppNumber: formValue.whatsAppNumber?.trim() || undefined
      };

      this.clinicAdminService.updatePublicDisplaySettings(request).subscribe({
        next: (response) => {
          this.currentSettings.set({
            showOnPublicSite: response.showOnPublicSite,
            clinicType: response.clinicType,
            whatsAppNumber: response.whatsAppNumber
          });
          this.successMessage.set('Configurações atualizadas com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => this.successMessage.set(''), this.SUCCESS_MESSAGE_TIMEOUT);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar configurações: ' + (error.error?.message || error.message));
          this.isSaving.set(false);
        }
      });
    }
  }

  getClinicTypeLabel(type: string): string {
    const clinicType = this.clinicTypes.find(ct => ct.value === type);
    return clinicType ? clinicType.label : type;
  }
}
