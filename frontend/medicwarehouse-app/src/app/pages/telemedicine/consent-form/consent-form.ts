import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-consent-form',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './consent-form.html',
  styleUrl: './consent-form.scss'
})
export class ConsentForm implements OnInit {
  consentForm: FormGroup;
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  sessionId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.consentForm = this.fb.group({
      patientName: ['', Validators.required],
      patientCpf: ['', Validators.required],
      acceptTerms: [false, Validators.requiredTrue],
      acceptDataProcessing: [false, Validators.requiredTrue],
      acceptRecording: [false],
      signature: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.sessionId = params['sessionId'];
    });
  }

  onSubmit(): void {
    if (this.consentForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios e aceite os termos');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');

    // TODO: Implement consent submission to backend
    // For now, just simulate success
    setTimeout(() => {
      this.successMessage.set('Consentimento registrado com sucesso!');
      this.isSaving.set(false);
      
      setTimeout(() => {
        if (this.sessionId) {
          this.router.navigate(['/telemedicine/room', this.sessionId]);
        } else {
          this.router.navigate(['/telemedicine']);
        }
      }, 1500);
    }, 1000);
  }

  cancel(): void {
    this.router.navigate(['/telemedicine']);
  }
}
