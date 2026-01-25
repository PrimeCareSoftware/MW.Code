import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { OwnerClinicService, ClinicDto, CreateClinicDto, UpdateClinicDto } from '../../../services/owner-clinic.service';
import { ClinicAdminInfoDto } from '../../../models/clinic-admin.model';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-clinic-info',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './clinic-info.component.html',
  styleUrl: './clinic-info.component.scss'
})
export class ClinicInfoComponent implements OnInit {
  clinicForm: FormGroup;
  clinicInfo = signal<ClinicAdminInfoDto | null>(null);
  ownerClinics = signal<ClinicDto[]>([]);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // For clinic CRUD
  showClinicModal = signal<boolean>(false);
  clinicModalMode = signal<'create' | 'edit'>('create');
  selectedClinic = signal<ClinicDto | null>(null);
  clinicModalForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private clinicAdminService: ClinicAdminService,
    private ownerClinicService: OwnerClinicService
  ) {
    this.clinicForm = this.fb.group({
      phone: [''],
      email: [''],
      address: [''],
      workingHours: ['']
    });

    this.clinicModalForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      tradeName: ['', [Validators.required, Validators.minLength(3)]],
      document: ['', [Validators.required]],
      phone: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      address: ['', [Validators.required]],
      openingTime: ['08:00', [Validators.required]],
      closingTime: ['18:00', [Validators.required]],
      appointmentDurationMinutes: [30, [Validators.required, Validators.min(5)]],
      allowEmergencySlots: [true],
      numberOfRooms: [1, [Validators.min(1)]],
      notifyPrimaryDoctorOnOtherDoctorAppointment: [true]
    });
  }

  ngOnInit(): void {
    this.loadClinicInfo();
    this.loadOwnerClinics();
  }

  loadClinicInfo(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.getClinicInfo().subscribe({
      next: (data) => {
        this.clinicInfo.set(data);
        this.clinicForm.patchValue({
          phone: data.phone || '',
          email: data.email || '',
          address: data.address || '',
          workingHours: data.workingHours || ''
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar informações da clínica: ' + (error.error?.message || error.message));
        this.isLoading.set(false);
      }
    });
  }

  loadOwnerClinics(): void {
    this.ownerClinicService.getMyClinics().subscribe({
      next: (clinics) => {
        this.ownerClinics.set(clinics);
      },
      error: (error) => {
        console.error('Error loading owner clinics:', error);
        // Not critical, just log it
      }
    });
  }

  onSubmit(): void {
    if (this.clinicForm.valid) {
      this.isSaving.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      this.clinicAdminService.updateClinicInfo(this.clinicForm.value).subscribe({
        next: (data) => {
          this.clinicInfo.set(data);
          this.successMessage.set('Informações da clínica atualizadas com sucesso!');
          this.isSaving.set(false);
          setTimeout(() => this.successMessage.set(''), 5000);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar informações: ' + (error.error?.message || error.message));
          this.isSaving.set(false);
        }
      });
    }
  }

  openCreateClinicModal(): void {
    this.clinicModalMode.set('create');
    this.selectedClinic.set(null);
    this.clinicModalForm.reset({
      openingTime: '08:00',
      closingTime: '18:00',
      appointmentDurationMinutes: 30,
      allowEmergencySlots: true,
      numberOfRooms: 1,
      notifyPrimaryDoctorOnOtherDoctorAppointment: true
    });
    this.showClinicModal.set(true);
  }

  openEditClinicModal(clinic: ClinicDto): void {
    this.clinicModalMode.set('edit');
    this.selectedClinic.set(clinic);
    this.clinicModalForm.patchValue({
      name: clinic.name,
      tradeName: clinic.tradeName,
      document: clinic.document,
      phone: clinic.phone,
      email: clinic.email,
      address: clinic.address,
      openingTime: clinic.openingTime,
      closingTime: clinic.closingTime,
      appointmentDurationMinutes: clinic.appointmentDurationMinutes,
      allowEmergencySlots: clinic.allowEmergencySlots,
      numberOfRooms: clinic.numberOfRooms,
      notifyPrimaryDoctorOnOtherDoctorAppointment: clinic.notifyPrimaryDoctorOnOtherDoctorAppointment
    });
    this.showClinicModal.set(true);
  }

  closeClinicModal(): void {
    this.showClinicModal.set(false);
    this.selectedClinic.set(null);
  }

  saveClinic(): void {
    if (this.clinicModalForm.invalid) {
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.clinicModalForm.value;

    if (this.clinicModalMode() === 'create') {
      const createDto: CreateClinicDto = {
        name: formValue.name,
        tradeName: formValue.tradeName,
        document: formValue.document,
        phone: formValue.phone,
        email: formValue.email,
        address: formValue.address,
        openingTime: formValue.openingTime,
        closingTime: formValue.closingTime,
        appointmentDurationMinutes: formValue.appointmentDurationMinutes
      };

      this.ownerClinicService.createClinic(createDto).subscribe({
        next: (clinic) => {
          this.successMessage.set('Clínica criada com sucesso!');
          this.loadOwnerClinics();
          this.closeClinicModal();
          this.isSaving.set(false);
          setTimeout(() => this.successMessage.set(''), 5000);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao criar clínica: ' + (error.error?.message || error.message));
          this.isSaving.set(false);
        }
      });
    } else {
      const updateDto: UpdateClinicDto = {
        name: formValue.name,
        tradeName: formValue.tradeName,
        phone: formValue.phone,
        email: formValue.email,
        address: formValue.address,
        openingTime: formValue.openingTime,
        closingTime: formValue.closingTime,
        appointmentDurationMinutes: formValue.appointmentDurationMinutes,
        allowEmergencySlots: formValue.allowEmergencySlots,
        numberOfRooms: formValue.numberOfRooms,
        notifyPrimaryDoctorOnOtherDoctorAppointment: formValue.notifyPrimaryDoctorOnOtherDoctorAppointment
      };

      this.ownerClinicService.updateClinic(this.selectedClinic()!.id, updateDto).subscribe({
        next: (clinic) => {
          this.successMessage.set('Clínica atualizada com sucesso!');
          this.loadOwnerClinics();
          this.closeClinicModal();
          this.isSaving.set(false);
          setTimeout(() => this.successMessage.set(''), 5000);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar clínica: ' + (error.error?.message || error.message));
          this.isSaving.set(false);
        }
      });
    }
  }
}

