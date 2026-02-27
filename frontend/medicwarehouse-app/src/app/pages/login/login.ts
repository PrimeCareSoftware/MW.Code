import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { Auth } from '../../services/auth';
import { TenantResolverService } from '../../services/tenant-resolver.service';
import { ClinicCustomizationService } from '../../services/clinic-customization.service';
import { ClinicCustomizationPublicDto } from '../../models/clinic-customization.model';

type LoginStep = 'credentials' | 'twoFactor' | 'changePassword';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  loginForm: FormGroup;
  twoFactorForm: FormGroup;
  changePasswordForm: FormGroup;
  errorMessage = signal<string>('');
  infoMessage = signal<string>('');
  isLoading = signal<boolean>(false);
  tenantFromUrl = signal<string | null>(null);
  clinicName = signal<string | null>(null);
  customization = signal<ClinicCustomizationPublicDto | null>(null);
  isOwnerLogin = signal<boolean>(false);
  step = signal<LoginStep>('credentials');
  tempToken = signal<string | null>(null);
  twoFactorMethod = signal<string>('Email');

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router,
    private route: ActivatedRoute,
    private tenantResolver: TenantResolverService,
    private clinicCustomizationService: ClinicCustomizationService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      tenantId: [''] // Optional, will be auto-filled from URL
    });

    this.twoFactorForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
    });

    this.changePasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    // Detect tenant from URL on component load
    this.detectTenantFromUrl();
    
    // Check if there's a message from router state (e.g., session invalidation)
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras?.state?.['message']) {
      this.infoMessage.set(navigation.extras.state['message']);
    }

    // Check for query parameters (e.g., from checkout page)
    this.route.queryParams.subscribe(params => {
      // Auto-fill username if provided
      if (params['username']) {
        this.loginForm.patchValue({ username: params['username'] });
      }
      
      // Auto-fill tenantId if provided
      if (params['tenantId']) {
        this.loginForm.patchValue({ tenantId: params['tenantId'] });
      }
      
      // Auto-set owner login toggle if specified
      if (params['isOwner'] === 'true') {
        this.isOwnerLogin.set(true);
        this.infoMessage.set('Você está prestes a fazer login como proprietário da clínica. Use suas credenciais de registro.');
      }
    });
  }

  detectTenantFromUrl(): void {
    const tenant = this.tenantResolver.extractTenantFromUrl();
    if (tenant) {
      this.tenantFromUrl.set(tenant);
      this.loginForm.patchValue({ tenantId: tenant });
      
      // Fetch clinic customization based on subdomain
      this.clinicCustomizationService.getBySubdomain(tenant).subscribe({
        next: (customizationData) => {
          this.customization.set(customizationData);
          this.clinicName.set(customizationData.clinicName || null);
          this.applyCustomization(customizationData);
        },
        error: (error) => {
          console.error('Error loading clinic customization:', error);
          // Try to resolve clinic name for backward compatibility
          this.tenantResolver.resolveTenantBySubdomain(tenant).subscribe({
            next: (tenantInfo) => {
              this.clinicName.set(tenantInfo.clinicName || null);
            },
            error: () => {
              // Ignore errors, just proceed without clinic name
            }
          });
        }
      });
    }
  }

  applyCustomization(customization: ClinicCustomizationPublicDto): void {
    if (customization.primaryColor && this.isValidColor(customization.primaryColor)) {
      document.documentElement.style.setProperty('--primary-color', customization.primaryColor);
    }
    if (customization.secondaryColor && this.isValidColor(customization.secondaryColor)) {
      document.documentElement.style.setProperty('--secondary-color', customization.secondaryColor);
    }
    if (customization.fontColor && this.isValidColor(customization.fontColor)) {
      document.documentElement.style.setProperty('--font-color', customization.fontColor);
    }
  }

  private isValidColor(color: string): boolean {
    // Validate hex color format (#RGB or #RRGGBB)
    const hexColorRegex = /^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/;
    return hexColorRegex.test(color);
  }

  toggleLoginType(): void {
    this.isOwnerLogin.set(!this.isOwnerLogin());
    this.errorMessage.set(''); // Clear any existing errors
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');

      // Use owner login if the toggle is enabled
      const loginMethod = this.isOwnerLogin() 
        ? this.authService.ownerLogin(this.loginForm.value)
        : this.authService.login(this.loginForm.value, false);

      loginMethod.subscribe({
        next: (response) => {
          this.isLoading.set(false);

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

          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading.set(false);
          
          // Try to get error message and code from response
          const errorCode = error.error?.code;
          const errorMsg = error.error?.message || 'Falha no login. Por favor, verifique suas credenciais.';
          
          // Show specific message for unconfirmed email
          if (errorCode === 'EMAIL_NOT_CONFIRMED') {
            this.errorMessage.set(errorMsg);
          } else if (errorCode === 'UNAUTHORIZED' || errorMsg.toLowerCase().includes('incorretos') || errorMsg.toLowerCase().includes('incorrect')) {
            if (this.isOwnerLogin()) {
              this.errorMessage.set(errorMsg + ' Certifique-se de que você está usando as credenciais de proprietário da clínica.');
            } else {
              this.errorMessage.set(errorMsg + ' Se você é o proprietário da clínica, ative a opção "Login como Proprietário".');
            }
          } else {
            this.errorMessage.set(errorMsg);
          }
        }
      });
    }
  }

  onVerify2fa(): void {
    if (this.twoFactorForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const token = this.tempToken();
    if (!token) {
      this.errorMessage.set('Sessão expirada. Por favor, faça login novamente.');
      this.step.set('credentials');
      this.isLoading.set(false);
      return;
    }

    this.authService.verify2faEmail(token, this.twoFactorForm.value.code, this.isOwnerLogin()).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Código inválido ou expirado. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }

  onResend2fa(): void {
    const token = this.tempToken();
    if (!token) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.authService.resend2faEmail(token, this.isOwnerLogin()).subscribe({
      next: (response) => {
        if (response.tempToken) {
          this.tempToken.set(response.tempToken);
        }
        this.infoMessage.set('Novo código enviado para seu e-mail.');
        this.twoFactorForm.reset();
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Erro ao reenviar código. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }

  onChangePassword(): void {
    if (this.changePasswordForm.invalid) {
      return;
    }

    const { newPassword, confirmPassword } = this.changePasswordForm.value;
    if (newPassword !== confirmPassword) {
      this.errorMessage.set('As senhas não conferem.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const token = this.tempToken();
    if (!token) {
      this.errorMessage.set('Sessão expirada. Por favor, faça login novamente.');
      this.step.set('credentials');
      this.isLoading.set(false);
      return;
    }

    this.authService.completePasswordChange(token, newPassword, confirmPassword).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Erro ao alterar senha. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }

  backToLogin(): void {
    this.step.set('credentials');
    this.tempToken.set(null);
    this.errorMessage.set('');
    this.infoMessage.set('');
    this.twoFactorForm.reset();
    this.changePasswordForm.reset();
  }
}
