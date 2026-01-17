import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent {
  whatsappNumber = environment.whatsappNumber;

  features = [
    {
      icon: '🏥',
      title: 'Gestão Completa',
      description: 'Sistema completo para gerenciar pacientes, agendamentos e prontuários médicos de forma simples e eficiente.'
    },
    {
      icon: '📱',
      title: 'Integração WhatsApp',
      description: 'Envie lembretes automáticos via WhatsApp e mantenha seus pacientes sempre informados sobre suas consultas.'
    },
    {
      icon: '📊',
      title: 'Relatórios Inteligentes',
      description: 'Tenha acesso a relatórios detalhados e dashboards para acompanhar o crescimento do seu consultório.'
    },
    {
      icon: '🔒',
      title: 'Segurança Total',
      description: 'Seus dados e os de seus pacientes protegidos com criptografia e backup automático diário.'
    },
    {
      icon: '☁️',
      title: '100% Cloud',
      description: 'Acesse de qualquer lugar, a qualquer momento. Sem instalação, sem preocupação com infraestrutura.'
    },
    {
      icon: '⚡',
      title: 'Rápido e Eficiente',
      description: 'Interface moderna e intuitiva que economiza tempo e aumenta a produtividade da sua equipe.'
    }
  ];

  benefits = [
    {
      title: 'Agende consultas em segundos',
      description: 'Visualização em calendário com blocos de horários e confirmação instantânea.'
    },
    {
      title: 'Prontuários digitais completos',
      description: 'Histórico completo do paciente, prescrições e documentos em um só lugar.'
    },
    {
      title: 'Reduza faltas em até 70%',
      description: 'Com lembretes automáticos por WhatsApp e SMS, seus pacientes nunca mais esquecem.'
    },
    {
      title: 'Suporte dedicado',
      description: 'Nossa equipe está sempre pronta para ajudar você e sua equipe.'
    }
  ];

  openWhatsApp(): void {
    window.open(`https://wa.me/${this.whatsappNumber}`, '_blank');
  }
}
