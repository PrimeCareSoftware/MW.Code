import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { TenantResolverService } from '../../services/tenant-resolver.service';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  loginForm: FormGroup;
  errorMessage = signal<string>('');
  isLoading = signal<boolean>(false);
  tenantFromUrl = signal<string | null>(null);
  clinicName = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private authService: Auth,
    private router: Router,
    private tenantResolver: TenantResolverService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      tenantId: [''] // Optional, will be auto-filled from URL
    });

    // Try to detect tenant from URL on component load
    this.detectTenantFromUrl();
  }

  detectTenantFromUrl(): void {
    const tenant = this.tenantResolver.extractTenantFromUrl();
    if (tenant) {
      this.tenantFromUrl.set(tenant);
      this.loginForm.patchValue({ tenantId: tenant });
      
      // Try to resolve clinic name for better UX
      this.tenantResolver.resolveTenantBySubdomain(tenant).subscribe({
        next: (tenantInfo) => {
          this.clinicName.set(tenantInfo.clinicName || null);
        },
        error: () => {
          // Ignore errors, just proceed without clinic name
        }
      });
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
