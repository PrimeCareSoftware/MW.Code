import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatStepperModule } from '@angular/material/stepper';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { environment } from '../../../environments/environment';

interface DeletionRequestData {
  requestType: string;
  reason: string;
  confirmation: boolean;
  understanding: boolean;
}

@Component({
  selector: 'app-deletion-request',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatStepperModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './DeletionRequest.component.html',
  styleUrls: ['./privacy.scss']
})
export class DeletionRequestComponent {
  loading = false;
  submitted = false;
  error: string | null = null;

  requestData: DeletionRequestData = {
    requestType: '',
    reason: '',
    confirmation: false,
    understanding: false
  };

  requestTypes = [
    { value: 'complete', label: 'Exclusão Completa', description: 'Remove todos os seus dados permanentemente' },
    { value: 'anonymization', label: 'Anonimização', description: 'Mantém dados estatísticos sem identificação pessoal' },
    { value: 'partial', label: 'Exclusão Parcial', description: 'Remove apenas dados específicos' }
  ];

  constructor(private http: HttpClient) {}

  isStep1Valid(): boolean {
    return !!this.requestData.requestType;
  }

  isStep2Valid(): boolean {
    return this.requestData.reason.trim().length >= 10;
  }

  isStep3Valid(): boolean {
    return this.requestData.confirmation && this.requestData.understanding;
  }

  getRequestTypeLabel(): string {
    const type = this.requestTypes.find(t => t.value === this.requestData.requestType);
    return type ? type.label : '';
  }

  submitRequest(): void {
    if (!this.isStep3Valid()) {
      return;
    }

    this.loading = true;
    this.error = null;

    this.http
      .post(`${environment.apiUrl}/patient-portal/data-deletion-request`, this.requestData)
      .subscribe({
        next: () => {
          this.loading = false;
          this.submitted = true;
        },
        error: (error) => {
          this.loading = false;
          this.error = 'Erro ao enviar solicitação. Tente novamente mais tarde.';
          console.error('Deletion request error:', error);
        }
      });
  }

  resetForm(): void {
    this.requestData = {
      requestType: '',
      reason: '',
      confirmation: false,
      understanding: false
    };
    this.submitted = false;
    this.error = null;
  }

  getRequestTypeDescription(): string {
    const type = this.requestTypes.find(t => t.value === this.requestData.requestType);
    return type ? type.description : '';
  }
}
