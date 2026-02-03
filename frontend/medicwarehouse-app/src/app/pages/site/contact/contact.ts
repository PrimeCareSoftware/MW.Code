import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SubscriptionService } from '../../../services/subscription';
import { ContactRequest } from '../../../models/contact.model';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-contact',
  imports: [CommonModule, FormsModule, HeaderComponent, FooterComponent],
  templateUrl: './contact.html',
  styleUrl: './contact.scss'
})
export class ContactComponent {
  private subscriptionService = inject(SubscriptionService);
  
  whatsappNumber = environment.whatsappNumber;
  companyEmail = environment.companyEmail;
  companyPhone = environment.companyPhone;
  companyAddress = environment.companyAddress;
  companyAddressDetails = environment.companyAddressDetails;
  
  model: ContactRequest = {
    name: '',
    email: '',
    phone: '',
    subject: '',
    message: ''
  };
  
  isSubmitting = false;
  submitSuccess = false;
  submitError = '';
  showSuccessMessage = false;

  onSubmit(): void {
    if (!this.isValid()) {
      this.submitError = 'Por favor, preencha todos os campos obrigatórios.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = '';
    this.submitSuccess = false;

    this.subscriptionService.sendContactMessage(this.model).subscribe({
      next: (response) => {
        this.submitSuccess = true;
        this.showSuccessMessage = true;
        this.resetForm();
        this.isSubmitting = false;
      },
      error: (error) => {
        this.submitError = 'Erro ao enviar mensagem. Tente novamente.';
        this.isSubmitting = false;
        console.error('Contact error:', error);
      }
    });
  }

  isValid(): boolean {
    return !!(this.model.name && this.model.email && this.model.phone && 
              this.model.subject && this.model.message);
  }

  resetForm(): void {
    this.model = {
      name: '',
      email: '',
      phone: '',
      subject: '',
      message: ''
    };
  }

  openWhatsApp(): void {
    window.open(`https://wa.me/${this.whatsappNumber}`, '_blank');
  }

  resetSuccessState(): void {
    this.showSuccessMessage = false;
    this.submitSuccess = false;
    this.resetForm();
  }
}
