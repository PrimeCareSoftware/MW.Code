import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-footer',
  imports: [RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class FooterComponent {
  protected currentYear = new Date().getFullYear();
  protected whatsappNumber = environment.whatsappNumber;
  protected companyEmail = environment.companyEmail;
  protected companyPhone = environment.companyPhone;

  openWhatsApp(): void {
    window.open(`https://wa.me/${this.whatsappNumber}`, '_blank');
  }
}
