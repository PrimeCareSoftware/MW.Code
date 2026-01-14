import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ClinicCustomizationService } from '../../../services/clinic-customization.service';
import { ClinicCustomizationDto } from '../../../models/clinic-customization.model';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-customization-editor',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './customization-editor.component.html',
  styleUrl: './customization-editor.component.scss'
})
export class CustomizationEditorComponent implements OnInit {
  customizationForm: FormGroup;
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private customizationService: ClinicCustomizationService
  ) {
    this.customizationForm = this.fb.group({
      primaryColor: ['#2563eb'],
      secondaryColor: ['#7c3aed'],
      fontColor: ['#1f2937'],
      logoUrl: [''],
      backgroundImageUrl: ['']
    });
  }

  ngOnInit(): void {
    this.loadCustomization();
  }

  loadCustomization(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.customizationService.getCurrentClinicCustomization().subscribe({
      next: (data) => {
        this.customizationForm.patchValue({
          primaryColor: data.primaryColor || '#2563eb',
          secondaryColor: data.secondaryColor || '#7c3aed',
          fontColor: data.fontColor || '#1f2937',
          logoUrl: data.logoUrl || '',
          backgroundImageUrl: data.backgroundImageUrl || ''
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading customization', error);
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.customizationForm.valid) {
      this.isSaving.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      // Update colors first
      this.customizationService.updateColors({
        primaryColor: this.customizationForm.value.primaryColor,
        secondaryColor: this.customizationForm.value.secondaryColor,
        fontColor: this.customizationForm.value.fontColor
      }).subscribe({
        next: () => {
          // Update logo if provided
          const logoUrl = this.customizationForm.value.logoUrl;
          if (logoUrl) {
            this.customizationService.updateLogo(logoUrl).subscribe({
              next: () => this.updateBackground(),
              error: (error) => this.handleError(error)
            });
          } else {
            this.updateBackground();
          }
        },
        error: (error) => this.handleError(error)
      });
    }
  }

  private updateBackground(): void {
    const backgroundUrl = this.customizationForm.value.backgroundImageUrl;
    if (backgroundUrl) {
      this.customizationService.updateBackground(backgroundUrl).subscribe({
        next: () => this.handleSuccess(),
        error: (error) => this.handleError(error)
      });
    } else {
      this.handleSuccess();
    }
  }

  private handleSuccess(): void {
    this.isSaving.set(false);
    this.successMessage.set('Personalização atualizada com sucesso!');
    setTimeout(() => this.successMessage.set(''), 5000);
  }

  private handleError(error: any): void {
    this.isSaving.set(false);
    this.errorMessage.set('Erro ao atualizar personalização: ' + (error.error?.message || error.message));
  }

  uploadLogo(): void {
    alert('Funcionalidade de upload será implementada em breve. Por enquanto, insira a URL da imagem.');
  }

  uploadBackground(): void {
    alert('Funcionalidade de upload será implementada em breve. Por enquanto, insira a URL da imagem.');
  }
}
