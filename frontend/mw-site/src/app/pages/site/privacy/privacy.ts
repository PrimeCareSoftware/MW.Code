import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-privacy',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './privacy.html',
  styleUrl: './privacy.scss'
})
export class PrivacyComponent {
}
