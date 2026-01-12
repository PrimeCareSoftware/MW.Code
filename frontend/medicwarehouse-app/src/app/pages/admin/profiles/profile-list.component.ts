import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AccessProfileService } from '../../../services/access-profile.service';
import { AccessProfile } from '../../../models/access-profile.model';

@Component({
  selector: 'app-profile-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './profile-list.component.html',
  styleUrls: ['./profile-list.component.scss']
})
export class ProfileListComponent implements OnInit {
  profiles: AccessProfile[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private profileService: AccessProfileService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProfiles();
  }

  loadProfiles(): void {
    this.loading = true;
    this.error = null;

    this.profileService.getProfiles().subscribe({
      next: (profiles) => {
        this.profiles = profiles;
        this.loading = false;
      },
      error: () => {
        // Erro já tratado pelo interceptor
        this.loading = false;
      }
    });
  }

  createProfile(): void {
    this.router.navigate(['/admin/profiles/new']);
  }

  editProfile(profileId: string): void {
    this.router.navigate(['/admin/profiles/edit', profileId]);
  }

  deleteProfile(profile: AccessProfile): void {
    if (profile.isDefault) {
      alert('Não é possível excluir perfis padrão');
      return;
    }

    if (profile.userCount > 0) {
      alert('Não é possível excluir perfil que está sendo usado por usuários');
      return;
    }

    if (confirm(`Tem certeza que deseja excluir o perfil "${profile.name}"?`)) {
      this.profileService.deleteProfile(profile.id).subscribe({
        next: () => {
          this.loadProfiles();
        },
        error: () => {
          // Erro já tratado pelo interceptor
        }
      });
    }
  }

  getPermissionCount(profile: AccessProfile): number {
    return profile.permissions.length;
  }
}
