import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-terms',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './terms.html',
  styleUrl: './terms.scss'
})
export class TermsComponent {
}
