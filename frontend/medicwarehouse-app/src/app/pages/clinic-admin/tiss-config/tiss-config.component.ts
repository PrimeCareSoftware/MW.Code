import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';

interface TissConfigDto {
  tissEnabled: boolean;
  tissProviderCode?: string;
  tissProviderName?: string;
  tussEnabled: boolean;
  autoGenerateBatches: boolean;
  batchCompetenceDay: number;
}

@Component({
  selector: 'app-tiss-config',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './tiss-config.component.html',
  styleUrl: './tiss-config.component.scss'
})
export class TissConfigComponent implements OnInit {
  configForm: FormGroup;
  config = signal<TissConfigDto | null>(null);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(private fb: FormBuilder) {
    this.configForm = this.fb.group({
      tissEnabled: [false],
      tissProviderCode: [''],
      tissProviderName: [''],
      tussEnabled: [false],
      autoGenerateBatches: [false],
      batchCompetenceDay: [1]
    });
  }

  ngOnInit(): void {
    this.loadConfig();
  }

  loadConfig(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    // TODO: Replace with actual service call when backend is ready
    // For now, use mock data from localStorage
    const savedConfig = localStorage.getItem('tissConfig');
    if (savedConfig) {
      const data = JSON.parse(savedConfig);
      this.config.set(data);
      this.configForm.patchValue(data);
    } else {
      // Default configuration
      const defaultConfig: TissConfigDto = {
        tissEnabled: false,
        tissProviderCode: '',
        tissProviderName: '',
        tussEnabled: false,
        autoGenerateBatches: false,
        batchCompetenceDay: 1
      };
      this.config.set(defaultConfig);
    }
    this.isLoading.set(false);
  }

  onSubmit(): void {
    if (this.configForm.valid) {
      this.isSaving.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      // TODO: Replace with actual service call when backend is ready
      // For now, save to localStorage
      const configData = this.configForm.value;
      localStorage.setItem('tissConfig', JSON.stringify(configData));
      this.config.set(configData);
      this.successMessage.set('Configurações TISS/TUSS atualizadas com sucesso!');
      this.isSaving.set(false);
      setTimeout(() => this.successMessage.set(''), 5000);
    }
  }

  onTissEnabledChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    if (!checked) {
      // If TISS is disabled, also disable TUSS and auto-generate batches
      this.configForm.patchValue({
        tussEnabled: false,
        autoGenerateBatches: false
      });
    }
  }
}
