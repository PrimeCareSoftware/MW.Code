import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject, takeUntil } from 'rxjs';
import { environment } from '../../../../environments/environment';

interface IdentityVerificationResponse {
  id: string;
  userId: string;
  userType: 'Provider' | 'Patient';
  documentType: string;
  documentNumber: string;
  status: string;
  verifiedAt?: string;
  validUntil?: string;
}

@Component({
  selector: 'app-identity-verification-upload',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './identity-verification-upload.html',
  styleUrl: './identity-verification-upload.scss'
})
export class IdentityVerificationUpload implements OnInit, OnDestroy {
  uploadForm: FormGroup;
  isUploading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // Preview URLs for uploaded files
  documentPhotoPreview = signal<string | null>(null);
  crmCardPhotoPreview = signal<string | null>(null);
  selfiePreview = signal<string | null>(null);
  
  // Files
  documentPhotoFile: File | null = null;
  crmCardPhotoFile: File | null = null;
  selfieFile: File | null = null;
  
  userId: string | null = null;
  userType: 'Provider' | 'Patient' = 'Patient';
  tenantId: string = 'default-tenant'; // TODO: Get from auth service
  
  private baseUrl = environment.apiUrl || 'http://localhost:5000/api';
  private telemedicineUrl = `${this.baseUrl}/telemedicine`;
  private destroy$ = new Subject<void>();

  // File validation constants
  readonly MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB
  readonly ALLOWED_TYPES = ['image/jpeg', 'image/jpg', 'image/png', 'application/pdf'];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private http: HttpClient
  ) {
    this.uploadForm = this.fb.group({
      documentType: ['RG', Validators.required],
      documentNumber: ['', [Validators.required, Validators.minLength(5)]],
      crmNumber: [''],
      crmState: [''],
      documentPhoto: [null, Validators.required],
      crmCardPhoto: [null],
      selfie: [null]
    });
  }

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.userId = params['userId'];
        this.userType = params['userType'] || 'Patient';
        
        // Update form validators based on user type
        if (this.userType === 'Provider') {
          this.uploadForm.get('crmNumber')?.setValidators([Validators.required]);
          this.uploadForm.get('crmState')?.setValidators([Validators.required]);
          this.uploadForm.get('crmCardPhoto')?.setValidators([Validators.required]);
        }
        this.uploadForm.updateValueAndValidity();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onDocumentPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!this.validateFile(file)) {
        return;
      }
      
      this.documentPhotoFile = file;
      this.uploadForm.patchValue({ documentPhoto: file });
      this.createPreview(file, this.documentPhotoPreview);
    }
  }

  onCrmCardPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!this.validateFile(file)) {
        return;
      }
      
      this.crmCardPhotoFile = file;
      this.uploadForm.patchValue({ crmCardPhoto: file });
      this.createPreview(file, this.crmCardPhotoPreview);
    }
  }

  onSelfieSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!this.validateFile(file)) {
        return;
      }
      
      this.selfieFile = file;
      this.uploadForm.patchValue({ selfie: file });
      this.createPreview(file, this.selfiePreview);
    }
  }

  validateFile(file: File): boolean {
    // Check file size
    if (file.size > this.MAX_FILE_SIZE) {
      this.errorMessage.set(`Arquivo muito grande. Tamanho máximo: ${this.MAX_FILE_SIZE / (1024 * 1024)}MB`);
      return false;
    }

    // Check file type
    if (!this.ALLOWED_TYPES.includes(file.type)) {
      this.errorMessage.set('Tipo de arquivo não permitido. Use JPG, PNG ou PDF.');
      return false;
    }

    this.errorMessage.set('');
    return true;
  }

  createPreview(file: File, previewSignal: { set: (value: string | null) => void }): void {
    // Only create preview for images
    if (file.type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target?.result && typeof e.target.result === 'string') {
          previewSignal.set(e.target.result);
        }
      };
      reader.readAsDataURL(file);
    } else {
      previewSignal.set('PDF');
    }
  }

  clearDocumentPhoto(): void {
    this.documentPhotoFile = null;
    this.documentPhotoPreview.set(null);
    this.uploadForm.patchValue({ documentPhoto: null });
  }

  clearCrmCardPhoto(): void {
    this.crmCardPhotoFile = null;
    this.crmCardPhotoPreview.set(null);
    this.uploadForm.patchValue({ crmCardPhoto: null });
  }

  clearSelfie(): void {
    this.selfieFile = null;
    this.selfiePreview.set(null);
    this.uploadForm.patchValue({ selfie: null });
  }

  onSubmit(): void {
    if (this.uploadForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    if (!this.userId) {
      this.errorMessage.set('ID do usuário não fornecido');
      return;
    }

    if (!this.documentPhotoFile) {
      this.errorMessage.set('Foto do documento é obrigatória');
      return;
    }

    if (this.userType === 'Provider' && !this.crmCardPhotoFile) {
      this.errorMessage.set('Foto da carteira do CRM é obrigatória para profissionais');
      return;
    }

    this.isUploading.set(true);
    this.errorMessage.set('');

    // Create FormData for multipart upload
    const formData = new FormData();
    formData.append('userId', this.userId);
    formData.append('userType', this.userType);
    formData.append('documentType', this.uploadForm.value.documentType);
    formData.append('documentNumber', this.uploadForm.value.documentNumber);
    
    if (this.userType === 'Provider') {
      formData.append('crmNumber', this.uploadForm.value.crmNumber);
      formData.append('crmState', this.uploadForm.value.crmState);
    }

    // Append files
    formData.append('documentPhoto', this.documentPhotoFile);
    if (this.crmCardPhotoFile) {
      formData.append('crmCardPhoto', this.crmCardPhotoFile);
    }
    if (this.selfieFile) {
      formData.append('selfie', this.selfieFile);
    }

    const headers = new HttpHeaders({
      'X-Tenant-Id': this.tenantId
      // Note: Don't set Content-Type for multipart/form-data - browser sets it with boundary
    });

    this.http.post<IdentityVerificationResponse>(
      `${this.telemedicineUrl}/identityverification`,
      formData,
      { headers }
    )
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (response) => {
        this.successMessage.set('Documentos enviados com sucesso! Aguarde a verificação.');
        this.isUploading.set(false);
        
        setTimeout(() => {
          this.router.navigate(['/telemedicine']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error uploading identity verification:', error);
        this.errorMessage.set(
          error.error?.message || 
          'Erro ao enviar documentos. Por favor, tente novamente.'
        );
        this.isUploading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/telemedicine']);
  }

  get isProvider(): boolean {
    return this.userType === 'Provider';
  }
}
