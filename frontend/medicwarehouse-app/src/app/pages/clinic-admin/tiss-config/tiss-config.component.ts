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
  private readonly SUCCESS_MESSAGE_TIMEOUT = 5000;
  private readonly STORAGE_KEY = 'tissConfig';
  readonly MIN_COMPETENCE_DAY = 1;
  readonly MAX_COMPETENCE_DAY = 28;

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
      batchCompetenceDay: [this.MIN_COMPETENCE_DAY]
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
    const savedConfig = this.getStoredConfig();
    if (savedConfig) {
      this.config.set(savedConfig);
      this.configForm.patchValue(savedConfig);
    } else {
      // Default configuration
      const defaultConfig: TissConfigDto = {
        tissEnabled: false,
        tissProviderCode: '',
        tissProviderName: '',
        tussEnabled: false,
        autoGenerateBatches: false,
        batchCompetenceDay: this.MIN_COMPETENCE_DAY
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
      this.saveConfig(configData);
      this.config.set(configData);
      this.successMessage.set('Configurações TISS/TUSS atualizadas com sucesso!');
      this.isSaving.set(false);
      setTimeout(() => this.successMessage.set(''), this.SUCCESS_MESSAGE_TIMEOUT);
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

  private getStoredConfig(): TissConfigDto | null {
    const savedConfig = localStorage.getItem(this.STORAGE_KEY);
    return savedConfig ? JSON.parse(savedConfig) : null;
  }

  private saveConfig(config: TissConfigDto): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(config));
  }
}
