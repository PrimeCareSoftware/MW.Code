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
    if (customization.primaryColor) {
      document.documentElement.style.setProperty('--primary-color', customization.primaryColor);
    }
    if (customization.secondaryColor) {
      document.documentElement.style.setProperty('--secondary-color', customization.secondaryColor);
    }
    if (customization.fontColor) {
      document.documentElement.style.setProperty('--font-color', customization.fontColor);
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');

      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading.set(false);
          this.errorMessage.set(error.error?.message || 'Login failed. Please check your credentials.');
        }
      });
    }
  }
}
