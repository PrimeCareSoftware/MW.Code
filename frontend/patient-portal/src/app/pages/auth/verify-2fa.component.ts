import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-verify-2fa',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './verify-2fa.component.html',
  styleUrls: ['./verify-2fa.component.scss']
})
export class VerifyTwoFactorComponent implements OnInit {
  verifyForm: FormGroup;
  loading = false;
  resending = false;
  tempToken = '';
  returnUrl = '/dashboard';
  countdown = 0;
  private countdownInterval?: any;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService
  ) {
    this.verifyForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  ngOnInit(): void {
    // Get temp token from route parameters
    this.tempToken = this.route.snapshot.queryParams['tempToken'];
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';

    if (!this.tempToken) {
      this.notificationService.error('Token temporário não encontrado');
      this.router.navigate(['/auth/login']);
      return;
    }

    // Start countdown for resend
    this.startResendCountdown();
  }

  ngOnDestroy(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }

  onSubmit(): void {
    if (this.verifyForm.invalid) {
      this.notificationService.warning('Por favor, digite o código de 6 dígitos');
      return;
    }

    this.loading = true;

    this.authService.verifyTwoFactor({
      tempToken: this.tempToken,
      code: this.verifyForm.value.code
    }).subscribe({
      next: () => {
        this.notificationService.success('Verificação concluída com sucesso!');
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        const message = error.error?.message || 'Código inválido ou expirado';
        this.notificationService.error(message);
        this.loading = false;
      }
    });
  }

  resendCode(): void {
    if (this.resending || this.countdown > 0) {
      return;
    }

    this.resending = true;

    this.authService.resendTwoFactorCode({
      tempToken: this.tempToken
    }).subscribe({
      next: () => {
        this.notificationService.success('Código reenviado com sucesso!');
        this.resending = false;
        this.startResendCountdown();
      },
      error: (error) => {
        const message = error.error?.message || 'Erro ao reenviar código';
        this.notificationService.error(message);
        this.resending = false;
      }
    });
  }

  private startResendCountdown(): void {
    this.countdown = 60; // 60 seconds countdown

    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }

    this.countdownInterval = setInterval(() => {
      this.countdown--;
      if (this.countdown <= 0) {
        clearInterval(this.countdownInterval);
      }
    }, 1000);
  }

  // Helper method to format code input
  onCodeInput(event: any): void {
    const value = event.target.value;
    // Only allow digits
    const digitsOnly = value.replace(/\D/g, '');
    this.verifyForm.patchValue({ code: digitsOnly });
  }
}
