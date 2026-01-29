import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/auth.model';

interface PrivacyOption {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-privacy-center',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './PrivacyCenter.component.html',
  styleUrls: ['./privacy.css']
})
export class PrivacyCenterComponent implements OnInit {
  currentUser: User | null = null;

  privacyOptions: PrivacyOption[] = [
    {
      title: 'Visualizar Meus Dados',
      description: 'Veja todos os dados pessoais que temos armazenados sobre você',
      icon: 'visibility',
      route: '/privacy/data-viewer',
      color: 'primary'
    },
    {
      title: 'Exportar Meus Dados',
      description: 'Baixe uma cópia dos seus dados pessoais em formato estruturado',
      icon: 'download',
      route: '/privacy/data-portability',
      color: 'accent'
    },
    {
      title: 'Gerenciar Consentimentos',
      description: 'Gerencie suas permissões e consentimentos de uso de dados',
      icon: 'check_circle',
      route: '/privacy/consent-manager',
      color: 'primary'
    },
    {
      title: 'Solicitar Exclusão',
      description: 'Solicite a exclusão ou anonimização dos seus dados pessoais',
      icon: 'delete',
      route: '/privacy/deletion-request',
      color: 'warn'
    }
  ];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
  }
}
