import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
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
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private clinicAdminService: ClinicAdminService
  ) {
    this.clinicForm = this.fb.group({
      phone: [''],
      email: [''],
      address: [''],
      workingHours: ['']
    });
  }

  ngOnInit(): void {
    this.loadClinicInfo();
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
}
