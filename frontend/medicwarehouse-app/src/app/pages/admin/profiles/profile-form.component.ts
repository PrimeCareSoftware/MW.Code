import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccessProfileService } from '../../../services/access-profile.service';
import { PermissionCategory } from '../../../models/access-profile.model';

@Component({
  selector: 'app-profile-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-form.component.html',
  styleUrls: ['./profile-form.component.scss']
})
export class ProfileFormComponent implements OnInit {
  form!: FormGroup;
  permissionCategories: PermissionCategory[] = [];
  selectedPermissions: Set<string> = new Set();
  loading = false;
  saving = false;
  error: string | null = null;
  profileId: string | null = null;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private profileService: AccessProfileService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.profileId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.profileId;

    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]]
    });

    this.loadPermissions();

    if (this.isEditMode && this.profileId) {
      this.loadProfile(this.profileId);
    }
  }

  loadPermissions(): void {
    this.profileService.getAllPermissions().subscribe({
      next: (categories) => {
        this.permissionCategories = categories;
      },
      error: () => {
        // Erro já tratado pelo interceptor
      }
    });
  }

  loadProfile(id: string): void {
    this.loading = true;
    this.profileService.getProfile(id).subscribe({
      next: (profile) => {
        this.form.patchValue({
          name: profile.name,
          description: profile.description
        });
        this.selectedPermissions = new Set(profile.permissions);
        this.loading = false;
      },
      error: () => {
        // Erro já tratado pelo interceptor
        this.loading = false;
      }
    });
  }

  togglePermission(key: string): void {
    if (this.selectedPermissions.has(key)) {
      this.selectedPermissions.delete(key);
    } else {
      this.selectedPermissions.add(key);
    }
  }

  isPermissionSelected(key: string): boolean {
    return this.selectedPermissions.has(key);
  }

  selectAllInCategory(category: PermissionCategory): void {
    category.permissions.forEach(p => this.selectedPermissions.add(p.key));
  }

  deselectAllInCategory(category: PermissionCategory): void {
    category.permissions.forEach(p => this.selectedPermissions.delete(p.key));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    if (this.selectedPermissions.size === 0) {
      alert('Selecione pelo menos uma permissão');
      return;
    }

    this.saving = true;
    this.error = null;

    const profileData = {
      ...this.form.value,
      permissions: Array.from(this.selectedPermissions)
    };

    const request$ = this.isEditMode && this.profileId
      ? this.profileService.updateProfile(this.profileId, profileData)
      : this.profileService.createProfile(profileData);

    request$.subscribe({
      next: () => {
        this.router.navigate(['/admin/profiles']);
      },
      error: () => {
        // Erro já tratado pelo interceptor
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/admin/profiles']);
  }
}
