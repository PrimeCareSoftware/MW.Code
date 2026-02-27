import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

type LoginStep = 'credentials' | 'twoFactor' | 'changePassword';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  loginForm: FormGroup;
  twoFactorForm: FormGroup;
  changePasswordForm: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  infoMessage = signal<string | null>(null);
  step = signal<LoginStep>('credentials');
  tempToken = signal<string | null>(null);
  twoFactorMethod = signal<string>('Email');

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      tenantId: ['system', Validators.required]
    });

    this.twoFactorForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
    });

    this.changePasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    });

    // Check if there's a message from router state (e.g., session invalidation)
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras?.state?.['message']) {
      this.infoMessage.set(navigation.extras.state['message']);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.authService.ownerLogin(this.loginForm.value).subscribe({
      next: (response) => {
        this.loading.set(false);

        if (response.requiresPasswordChange && response.tempToken) {
          this.tempToken.set(response.tempToken);
          this.step.set('changePassword');
          this.infoMessage.set('Por segurança, você precisa alterar sua senha antes de continuar.');
          return;
        }

        if (response.requiresTwoFactor && response.tempToken) {
          this.tempToken.set(response.tempToken);
          this.twoFactorMethod.set(response.method || 'Email');
          this.step.set('twoFactor');
          this.infoMessage.set(response.message || 'Código de verificação enviado para seu e-mail.');
          return;
        }

        if (response.isSystemOwner) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error.set('Acesso negado. Apenas System Owners podem acessar esta área.');
          this.authService.logout();
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao fazer login. Verifique suas credenciais.');
        this.loading.set(false);
      }
    });
  }

  onVerify2fa(): void {
    if (this.twoFactorForm.invalid) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const token = this.tempToken();
    if (!token) {
      this.error.set('Sessão expirada. Por favor, faça login novamente.');
      this.step.set('credentials');
      this.loading.set(false);
      return;
    }

    this.authService.verify2faEmail(token, this.twoFactorForm.value.code, true).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.isSystemOwner) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error.set('Acesso negado. Apenas System Owners podem acessar esta área.');
          this.authService.logout();
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Código inválido ou expirado. Tente novamente.');
        this.loading.set(false);
      }
    });
  }

  onResend2fa(): void {
    const token = this.tempToken();
    if (!token) return;

    this.loading.set(true);
    this.error.set(null);

    this.authService.resend2faEmail(token, true).subscribe({
      next: (response) => {
        if (response.tempToken) {
          this.tempToken.set(response.tempToken);
        }
        this.infoMessage.set('Novo código enviado para seu e-mail.');
        this.twoFactorForm.reset();
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao reenviar código. Tente novamente.');
        this.loading.set(false);
      }
    });
  }

  onChangePassword(): void {
    if (this.changePasswordForm.invalid) {
      return;
    }

    const { newPassword, confirmPassword } = this.changePasswordForm.value;
    if (newPassword !== confirmPassword) {
      this.error.set('As senhas não conferem.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const token = this.tempToken();
    if (!token) {
      this.error.set('Sessão expirada. Por favor, faça login novamente.');
      this.step.set('credentials');
      this.loading.set(false);
      return;
    }

    this.authService.completePasswordChange(token, newPassword, confirmPassword).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.isSystemOwner) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error.set('Acesso negado. Apenas System Owners podem acessar esta área.');
          this.authService.logout();
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao alterar senha. Tente novamente.');
        this.loading.set(false);
      }
    });
  }

  backToLogin(): void {
    this.step.set('credentials');
    this.tempToken.set(null);
    this.error.set(null);
    this.infoMessage.set(null);
    this.twoFactorForm.reset();
    this.changePasswordForm.reset();
  }
}
