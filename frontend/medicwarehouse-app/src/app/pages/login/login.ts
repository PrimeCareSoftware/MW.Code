import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { TenantResolverService } from '../../services/tenant-resolver.service';
import { ClinicCustomizationService } from '../../services/clinic-customization.service';
import { ClinicCustomizationPublicDto } from '../../models/clinic-customization.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  loginForm: FormGroup;
  errorMessage = signal<string>('');
  infoMessage = signal<string>('');
  isLoading = signal<boolean>(false);
  tenantFromUrl = signal<string | null>(null);
  clinicName = signal<string | null>(null);
  customization = signal<ClinicCustomizationPublicDto | null>(null);
  isOwnerLogin = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router,
    private tenantResolver: TenantResolverService,
    private clinicCustomizationService: ClinicCustomizationService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      tenantId: [''] // Optional, will be auto-filled from URL
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
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading.set(false);
          const errorMsg = error.error?.message || 'Falha no login. Por favor, verifique suas credenciais.';
          
          // Provide helpful hints based on login type
          if (this.isOwnerLogin() && errorMsg.includes('incorretos')) {
            this.errorMessage.set(errorMsg + ' Certifique-se de que você está usando as credenciais de proprietário da clínica.');
          } else if (!this.isOwnerLogin() && errorMsg.includes('incorretos')) {
            this.errorMessage.set(errorMsg + ' Se você é o proprietário da clínica, ative a opção "Login como Proprietário".');
          } else {
            this.errorMessage.set(errorMsg);
          }
        }
      });
    }
  }
}
