import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <div class="logo">
            <svg width="48" height="48" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
              <rect width="48" height="48" rx="8" fill="#667eea"/>
              <path d="M24 14L18 20H22V34H26V20H30L24 14Z" fill="white"/>
            </svg>
          </div>
          <h1>Sistema de Administração</h1>
          <p>MedicWarehouse - System Owner</p>
        </div>

        @if (error()) {
          <div class="alert alert-error">
            <span class="alert-icon">⚠️</span>
            <span>{{ error() }}</span>
          </div>
        }

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="username">Usuário</label>
            <input 
              type="text" 
              id="username" 
              formControlName="username"
              class="form-control"
              placeholder="Digite seu usuário"
              [class.error]="loginForm.get('username')?.invalid && loginForm.get('username')?.touched"
            />
            @if (loginForm.get('username')?.invalid && loginForm.get('username')?.touched) {
              <span class="error-message">Usuário é obrigatório</span>
            }
          </div>

          <div class="form-group">
            <label for="tenantId">Tenant ID</label>
            <input 
              type="text" 
              id="tenantId" 
              formControlName="tenantId"
              class="form-control"
              placeholder="Digite o tenant ID"
              [class.error]="loginForm.get('tenantId')?.invalid && loginForm.get('tenantId')?.touched"
            />
            @if (loginForm.get('tenantId')?.invalid && loginForm.get('tenantId')?.touched) {
              <span class="error-message">Tenant ID é obrigatório</span>
            }
          </div>

          <div class="form-group">
            <label for="password">Senha</label>
            <input 
              type="password" 
              id="password" 
              formControlName="password"
              class="form-control"
              placeholder="Digite sua senha"
              [class.error]="loginForm.get('password')?.invalid && loginForm.get('password')?.touched"
            />
            @if (loginForm.get('password')?.invalid && loginForm.get('password')?.touched) {
              <span class="error-message">Senha é obrigatória</span>
            }
          </div>

          <button 
            type="submit" 
            class="btn btn-primary"
            [disabled]="loginForm.invalid || loading()"
          >
            @if (loading()) {
              <span>Entrando...</span>
            } @else {
              <span>Entrar</span>
            }
          </button>
        </form>

        <div class="login-footer">
          <p>Área restrita para administradores do sistema</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 20px;
    }

    .login-card {
      background: white;
      border-radius: 16px;
      padding: 48px;
      width: 100%;
      max-width: 450px;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    }

    .login-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .logo {
      margin: 0 auto 16px;
      width: 48px;
      height: 48px;
    }

    h1 {
      font-size: 28px;
      font-weight: 700;
      color: #1a202c;
      margin: 0 0 8px 0;
    }

    .login-header p {
      color: #718096;
      font-size: 14px;
      margin: 0;
    }

    .alert {
      padding: 12px 16px;
      border-radius: 8px;
      margin-bottom: 24px;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .alert-error {
      background: #fee;
      color: #c53030;
      border: 1px solid #fc8181;
    }

    .alert-icon {
      font-size: 20px;
    }

    .form-group {
      margin-bottom: 20px;
    }

    label {
      display: block;
      font-weight: 600;
      color: #2d3748;
      margin-bottom: 8px;
      font-size: 14px;
    }

    .form-control {
      width: 100%;
      padding: 12px 16px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 16px;
      transition: all 0.2s;
      box-sizing: border-box;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control.error {
      border-color: #fc8181;
    }

    .error-message {
      display: block;
      color: #c53030;
      font-size: 13px;
      margin-top: 6px;
    }

    .btn {
      width: 100%;
      padding: 14px;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
      margin-top: 8px;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .login-footer {
      text-align: center;
      margin-top: 24px;
      padding-top: 24px;
      border-top: 1px solid #e2e8f0;
    }

    .login-footer p {
      color: #718096;
      font-size: 13px;
      margin: 0;
    }

    @media (max-width: 576px) {
      .login-card {
        padding: 32px 24px;
      }

      h1 {
        font-size: 24px;
      }
    }
  `]
})
export class Login {
  loginForm: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);

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
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        if (response.isSystemOwner) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error.set('Acesso negado. Apenas System Owners podem acessar esta área.');
          this.authService.logout();
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao fazer login. Verifique suas credenciais.');
        this.loading.set(false);
      }
    });
  }
}
