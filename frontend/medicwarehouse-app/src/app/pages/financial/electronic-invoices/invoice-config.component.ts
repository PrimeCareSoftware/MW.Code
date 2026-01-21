import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { ElectronicInvoiceService } from '../../../services/electronic-invoice.service';
import { InvoiceConfiguration } from '../../../models/electronic-invoice.model';

@Component({
  selector: 'app-invoice-config',
  imports: [CommonModule, RouterLink, Navbar, ReactiveFormsModule, FormsModule],
  templateUrl: './invoice-config.component.html',
  styleUrl: './invoice-config.component.scss'
})
export class InvoiceConfigComponent implements OnInit {
  form: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  isEditMode = signal<boolean>(false);
  configuration = signal<InvoiceConfiguration | null>(null);
  certificateFile = signal<File | null>(null);
  certificatePassword = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private invoiceService: ElectronicInvoiceService,
    private router: Router
  ) {
    this.form = this.fb.group({
      cnpj: ['', [Validators.required]],
      companyName: ['', [Validators.required]],
      municipalRegistration: [''],
      stateRegistration: [''],
      tradeName: [''],
      address: [''],
      addressNumber: [''],
      addressComplement: [''],
      neighborhood: [''],
      city: [''],
      state: [''],
      zipCode: [''],
      cityCode: [''],
      phone: [''],
      email: ['', [Validators.email]],
      serviceCode: [''],
      defaultIssRate: [5.0, [Validators.required, Validators.min(0)]],
      issRetainedByDefault: [false],
      isSimplifiedTaxRegime: [true],
      simplifiedTaxRegimeCode: ['1'],
      gateway: ['FocusNFe', [Validators.required]],
      gatewayApiKey: [''],
      gatewayEnvironment: ['homologacao'],
      autoIssueAfterPayment: [false],
      sendEmailAfterIssuance: [true]
    });
  }

  ngOnInit(): void {
    this.loadConfiguration();
  }

  loadConfiguration(): void {
    this.isLoading.set(true);
    this.invoiceService.getConfiguration().subscribe({
      next: (config) => {
        this.configuration.set(config);
        this.isEditMode.set(true);
        this.form.patchValue(config);
        // CNPJ is read-only in edit mode
        this.form.get('cnpj')?.disable();
        this.form.get('companyName')?.disable();
        this.isLoading.set(false);
      },
      error: (error) => {
        // No configuration exists yet
        this.isEditMode.set(false);
        this.isLoading.set(false);
      }
    });
  }

  onCnpjInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // CNPJ: 00.000.000/0000-00
    value = value.replace(/^(\d{2})(\d)/, '$1.$2');
    value = value.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
    value = value.replace(/\.(\d{3})(\d)/, '.$1/$2');
    value = value.replace(/(\d{4})(\d)/, '$1-$2');
    
    this.form.patchValue({ cnpj: value });
  }

  onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // (00) 00000-0000
    if (value.length > 10) {
      value = value.replace(/^(\d{2})(\d{5})(\d{4}).*/, '($1) $2-$3');
    } else if (value.length > 6) {
      value = value.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, '($1) $2-$3');
    } else if (value.length > 2) {
      value = value.replace(/^(\d{2})(\d{0,5})/, '($1) $2');
    } else {
      value = value.replace(/^(\d*)/, '($1');
    }
    
    this.form.patchValue({ phone: value });
  }

  onZipCodeInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    
    // 00000-000
    value = value.replace(/^(\d{5})(\d)/, '$1-$2');
    
    this.form.patchValue({ zipCode: value });
  }

  onCertificateSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.certificateFile.set(input.files[0]);
    }
  }

  uploadCertificate(): void {
    const file = this.certificateFile();
    const password = this.certificatePassword();

    if (!file || !password) {
      this.errorMessage.set('Selecione o certificado e informe a senha');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string;
      const certificate = base64.split(',')[1]; // Remove data:... prefix

      this.isLoading.set(true);
      this.invoiceService.uploadCertificate({ certificate, password }).subscribe({
        next: () => {
          this.successMessage.set('Certificado enviado com sucesso!');
          this.certificateFile.set(null);
          this.certificatePassword.set('');
          this.loadConfiguration();
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (error) => {
          this.errorMessage.set(error.error?.error || 'Erro ao enviar certificado');
          this.isLoading.set(false);
        }
      });
    };
    reader.readAsDataURL(file);
  }

  save(): void {
    if (!this.form.valid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const formData = this.form.getRawValue();

    const action = this.isEditMode()
      ? this.invoiceService.updateConfiguration(formData)
      : this.invoiceService.createConfiguration(formData);

    action.subscribe({
      next: (config) => {
        this.configuration.set(config);
        this.successMessage.set('Configuração salva com sucesso!');
        this.isEditMode.set(true);
        this.form.get('cnpj')?.disable();
        this.form.get('companyName')?.disable();
        this.isLoading.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao salvar configuração');
        this.isLoading.set(false);
      }
    });
  }

  activate(): void {
    if (!confirm('Deseja ativar a configuração de notas fiscais?')) {
      return;
    }

    this.isLoading.set(true);
    this.invoiceService.activateConfiguration().subscribe({
      next: () => {
        this.successMessage.set('Configuração ativada com sucesso!');
        this.loadConfiguration();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao ativar configuração');
        this.isLoading.set(false);
      }
    });
  }

  deactivate(): void {
    if (!confirm('Deseja desativar a configuração de notas fiscais?')) {
      return;
    }

    this.isLoading.set(true);
    this.invoiceService.deactivateConfiguration().subscribe({
      next: () => {
        this.successMessage.set('Configuração desativada com sucesso!');
        this.loadConfiguration();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        this.errorMessage.set(error.error?.error || 'Erro ao desativar configuração');
        this.isLoading.set(false);
      }
    });
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }
}
