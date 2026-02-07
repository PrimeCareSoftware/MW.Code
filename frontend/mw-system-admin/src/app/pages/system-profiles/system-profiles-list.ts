import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { SystemProfilesService, DefaultProfileTemplate, ClinicType, PermissionCategory } from '../../services/system-profiles.service';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-system-profiles-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Navbar],
  templateUrl: './system-profiles-list.html',
  styleUrl: './system-profiles-list.scss'
})
export class SystemProfilesList implements OnInit {
  templates = signal<DefaultProfileTemplate[]>([]);
  filteredTemplates = signal<DefaultProfileTemplate[]>([]);
  clinicTypes = signal<ClinicType[]>([]);
  permissions = signal<PermissionCategory[]>([]);
  
  loading = signal(true);
  error = signal<string | null>(null);
  
  showDetailModal = false;
  selectedTemplate: DefaultProfileTemplate | null = null;
  
  // Filter states
  searchTerm = '';
  selectedClinicType = '';

  constructor(private systemProfilesService: SystemProfilesService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    // Load all data in parallel
    Promise.all([
      firstValueFrom(this.systemProfilesService.getDefaultTemplates()),
      firstValueFrom(this.systemProfilesService.getClinicTypes()),
      firstValueFrom(this.systemProfilesService.getAllPermissions())
    ]).then(([templates, clinicTypes, permissions]) => {
      this.templates.set(templates || []);
      this.filteredTemplates.set(templates || []);
      this.clinicTypes.set(clinicTypes || []);
      this.permissions.set(permissions || []);
      this.loading.set(false);
    }).catch((err) => {
      this.error.set(err.error?.message || 'Erro ao carregar perfis padrão');
      this.loading.set(false);
    });
  }

  applyFilters(): void {
    let filtered = this.templates();

    // Filter by search term
    if (this.searchTerm.trim()) {
      const search = this.searchTerm.toLowerCase();
      filtered = filtered.filter(t => 
        t.name.toLowerCase().includes(search) || 
        t.description.toLowerCase().includes(search)
      );
    }

    // Filter by clinic type
    if (this.selectedClinicType) {
      filtered = filtered.filter(t => 
        t.applicableClinicTypes.includes(this.selectedClinicType)
      );
    }

    this.filteredTemplates.set(filtered);
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedClinicType = '';
    this.filteredTemplates.set(this.templates());
  }

  viewDetails(template: DefaultProfileTemplate): void {
    this.selectedTemplate = template;
    this.showDetailModal = true;
  }

  closeDetailModal(): void {
    this.showDetailModal = false;
    this.selectedTemplate = null;
  }

  getClinicTypeNames(types: string[]): string {
    return types.map(type => {
      const clinicType = this.clinicTypes().find(ct => ct.value === type);
      return clinicType ? clinicType.name : type;
    }).join(', ');
  }

  getPermissionDescription(permissionKey: string): string {
    for (const category of this.permissions()) {
      const permission = category.permissions.find(p => p.key === permissionKey);
      if (permission) {
        return permission.description;
      }
    }
    return permissionKey;
  }

  getPermissionsByCategory(): Map<string, string[]> {
    if (!this.selectedTemplate) return new Map();

    const map = new Map<string, string[]>();
    
    for (const permission of this.selectedTemplate.permissions) {
      for (const category of this.permissions()) {
        if (category.permissions.some(p => p.key === permission)) {
          if (!map.has(category.category)) {
            map.set(category.category, []);
          }
          map.get(category.category)!.push(permission);
          break;
        }
      }
    }

    return map;
  }
}
