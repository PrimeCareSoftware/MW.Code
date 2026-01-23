import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { AnamnesisService } from '../../../services/anamnesis.service';
import { AnamnesisTemplate, MedicalSpecialty, SPECIALTY_NAMES } from '../../../models/anamnesis.model';

@Component({
  selector: 'app-template-selector',
  imports: [CommonModule, RouterLink, FormsModule, Navbar],
  templateUrl: './template-selector.html',
  styleUrl: './template-selector.scss'
})
export class TemplateSelectorComponent implements OnInit {
  templates = signal<AnamnesisTemplate[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedSpecialty = signal<MedicalSpecialty>(MedicalSpecialty.GeneralMedicine);
  appointmentId: string = '';
  
  specialties = [
    MedicalSpecialty.GeneralMedicine,
    MedicalSpecialty.Cardiology,
    MedicalSpecialty.Pediatrics,
    MedicalSpecialty.Gynecology,
    MedicalSpecialty.Dermatology,
    MedicalSpecialty.Orthopedics,
    MedicalSpecialty.Psychiatry,
    MedicalSpecialty.Endocrinology,
    MedicalSpecialty.Neurology,
    MedicalSpecialty.Ophthalmology,
    MedicalSpecialty.Otorhinolaryngology,
    MedicalSpecialty.Other
  ];
  
  specialtyNames = SPECIALTY_NAMES;

  constructor(
    private anamnesisService: AnamnesisService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.appointmentId = params['appointmentId'] || '';
      if (params['specialty']) {
        const specialty = Number(params['specialty']);
        if (this.isValidSpecialty(specialty)) {
          this.selectedSpecialty.set(specialty);
        }
      }
    });
    this.loadTemplates();
  }

  private isValidSpecialty(value: number): boolean {
    return Object.values(MedicalSpecialty).includes(value);
  }

  loadTemplates(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.anamnesisService.getTemplatesBySpecialty(this.selectedSpecialty()).subscribe({
      next: (data) => {
        this.templates.set(data.filter(t => t.isActive));
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar templates');
        this.isLoading.set(false);
        console.error('Error loading templates:', error);
      }
    });
  }

  onSpecialtyChange(): void {
    this.loadTemplates();
  }

  selectTemplate(template: AnamnesisTemplate): void {
    if (this.appointmentId) {
      this.router.navigate(['/anamnesis/questionnaire', this.appointmentId], {
        queryParams: { templateId: template.id }
      });
    } else {
      this.errorMessage.set('ID do atendimento não fornecido');
    }
  }
}
