import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Navbar } from '../../shared/navbar/navbar';
import { Auth } from '../../services/auth';

interface UserProfile {
  id: string;
  username: string;
  email: string;
  fullName: string;
  phone: string;
  role: string;
  professionalId?: string;
  specialty?: string;
  showInAppointmentScheduling: boolean;
}

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Navbar],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  loading = false;
  saving = false;
  changingPassword = false;
  error = '';
  success = '';
  passwordError = '';
  passwordSuccess = '';

  // Forms
  profileForm!: FormGroup;
  passwordForm!: FormGroup;

  // Password visibility
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;

  // Active tab
  activeTab: 'profile' | 'password' = 'profile';

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private auth: Auth
  ) {
    this.initializeForms();
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  private initializeForms(): void {
    this.profileForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      fullName: ['', Validators.required],
      phone: ['']
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  private passwordMatchValidator(g: FormGroup) {
    const newPassword = g.get('newPassword')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { mismatch: true };
  }

  loadProfile(): void {
    this.loading = true;
    this.error = '';

    this.http.get<UserProfile>('/api/Users/me/profile').subscribe({
      next: (profile) => {
        this.profile = profile;
        this.profileForm.patchValue({
          email: profile.email,
          fullName: profile.fullName,
          phone: profile.phone || ''
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar perfil: ' + (err.error?.message || err.message);
        this.loading = false;
      }
    });
  }

  updateProfile(): void {
    if (this.profileForm.invalid) {
      this.error = 'Por favor, preencha todos os campos obrigatórios corretamente.';
      return;
    }

    this.saving = true;
    this.error = '';
    this.success = '';

    const data = this.profileForm.value;

    this.http.put('/api/Users/me/profile', data).subscribe({
      next: () => {
        this.success = 'Perfil atualizado com sucesso!';
        this.saving = false;
        // Reload profile to get updated data
        this.loadProfile();
        // Clear success message after 5 seconds
        setTimeout(() => this.success = '', 5000);
      },
      error: (err) => {
        this.error = 'Erro ao atualizar perfil: ' + (err.error?.message || err.message);
        this.saving = false;
      }
    });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordError = 'Por favor, preencha todos os campos corretamente.';
      return;
    }

    if (this.passwordForm.errors?.['mismatch']) {
      this.passwordError = 'As senhas não coincidem.';
      return;
    }

    this.changingPassword = true;
    this.passwordError = '';
    this.passwordSuccess = '';

    const data = {
      currentPassword: this.passwordForm.value.currentPassword,
      newPassword: this.passwordForm.value.newPassword
    };

    this.http.post('/api/Users/me/change-password', data).subscribe({
      next: () => {
        this.passwordSuccess = 'Senha alterada com sucesso!';
        this.changingPassword = false;
        this.passwordForm.reset();
        // Clear success message after 5 seconds
        setTimeout(() => this.passwordSuccess = '', 5000);
      },
      error: (err) => {
        this.passwordError = err.error?.message || 'Erro ao alterar senha. Verifique a senha atual.';
        this.changingPassword = false;
      }
    });
  }

  toggleCurrentPassword(): void {
    this.showCurrentPassword = !this.showCurrentPassword;
  }

  toggleNewPassword(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  setActiveTab(tab: 'profile' | 'password'): void {
    this.activeTab = tab;
    // Clear messages when switching tabs
    this.error = '';
    this.success = '';
    this.passwordError = '';
    this.passwordSuccess = '';
  }

  getPasswordStrength(): { label: string, class: string, width: string } {
    const password = this.passwordForm.get('newPassword')?.value || '';
    if (password.length === 0) {
      return { label: '', class: '', width: '0%' };
    }

    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;

    if (strength <= 2) {
      return { label: 'Fraca', class: 'weak', width: '33%' };
    } else if (strength <= 3) {
      return { label: 'Média', class: 'medium', width: '66%' };
    } else {
      return { label: 'Forte', class: 'strong', width: '100%' };
    }
  }

  hasMinLength(): boolean {
    const password = this.passwordForm.get('newPassword')?.value || '';
    return password.length >= 8;
  }

  hasUpperCase(): boolean {
    const password = this.passwordForm.get('newPassword')?.value || '';
    return /[A-Z]/.test(password);
  }

  hasLowerCase(): boolean {
    const password = this.passwordForm.get('newPassword')?.value || '';
    return /[a-z]/.test(password);
  }

  hasNumber(): boolean {
    const password = this.passwordForm.get('newPassword')?.value || '';
    return /[0-9]/.test(password);
  }

  hasSpecialChar(): boolean {
    const password = this.passwordForm.get('newPassword')?.value || '';
    return /[^a-zA-Z0-9]/.test(password);
  }
}
