import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { CompanyService } from '../../services/company.service';
import { Company, DocumentType, DocumentTypeLabels } from '../../models/company.model';

@Component({
  selector: 'app-company-info',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './company-info.html',
  styleUrl: './company-info.scss'
})
export class CompanyInfo implements OnInit {
  companyForm: FormGroup;
  company = signal<Company | null>(null);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  documentTypeLabels = DocumentTypeLabels;
  DocumentType = DocumentType; // Make enum available in template

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private companyService: CompanyService
  ) {
    this.companyForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      tradeName: ['', [Validators.required, Validators.minLength(3)]],
      document: [{ value: '', disabled: true }],
      documentType: [{ value: DocumentType.CNPJ, disabled: true }],
      phone: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      subdomain: ['']
    });
  }

  ngOnInit(): void {
    this.loadCompany();
  }

  loadCompany(): void {
    this.isLoading.set(true);
    this.companyService.getCompany().subscribe({
      next: (company) => {
        this.company.set(company);
        this.companyForm.patchValue({
          name: company.name,
          tradeName: company.tradeName,
          document: company.document,
          documentType: company.documentType,
          phone: company.phone,
          email: company.email,
          subdomain: company.subdomain
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading company:', error);
        this.errorMessage.set('Erro ao carregar informações da empresa');
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.companyForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios corretamente.');
      return;
    }

    const companyData = this.company();
    if (!companyData) {
      this.errorMessage.set('Nenhuma empresa encontrada para atualizar.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.companyForm.value;
    
    this.companyService.update(companyData.id, {
      name: formValue.name,
      tradeName: formValue.tradeName,
      phone: formValue.phone,
      email: formValue.email,
      subdomain: formValue.subdomain
    }).subscribe({
      next: () => {
        this.successMessage.set('Informações da empresa atualizadas com sucesso!');
        this.isSaving.set(false);
        // Reload company data
        setTimeout(() => {
          this.loadCompany();
          this.successMessage.set('');
        }, 2000);
      },
      error: (error) => {
        console.error('Error updating company:', error);
        this.errorMessage.set('Erro ao atualizar informações da empresa');
        this.isSaving.set(false);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/dashboard']);
  }

  getDocumentTypeLabel(): string {
    const docType = this.companyForm.get('documentType')?.value;
    return this.documentTypeLabels[docType as DocumentType] ?? '';
  }
}
