import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../../components/site/header/header';
import { FooterComponent } from '../../../../components/site/footer/footer';

interface Service {
  icon: string;
  title: string;
  description: string;
  features: string[];
}

interface Testimonial {
  name: string;
  role: string;
  clinic: string;
  content: string;
  rating: number;
  initials: string;
}

@Component({
  selector: 'app-omni-care-home',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './omni-care-home.component.html',
  styleUrl: './omni-care-home.component.scss'
})
export class OmniCareHomeComponent {
  benefits = [
    'Gestão completa em uma única plataforma',
    'Telemedicina integrada e segura',
    'Suporte 24/7 para sua equipe'
  ];

  services: Service[] = [
    {
      icon: 'business',
      title: 'Gerenciamento de Clínicas',
      description: 'Controle completo de múltiplas unidades, profissionais, salas e equipamentos em uma única plataforma.',
      features: ['Multi-unidades', 'Controle de estoque', 'Relatórios gerenciais']
    },
    {
      icon: 'calendar_month',
      title: 'Agendamento de Consultas',
      description: 'Sistema inteligente de agendamentos com lembretes automáticos e confirmação por WhatsApp.',
      features: ['Agenda online 24/7', 'Lembretes automáticos', 'Lista de espera']
    },
    {
      icon: 'videocam',
      title: 'Telemedicina',
      description: 'Consultas por vídeo com segurança, integração de prontuário e prescrição digital.',
      features: ['Videochamadas HD', 'Prontuário integrado', 'Prescrição digital']
    },
    {
      icon: 'layers',
      title: 'Integração com Sistemas',
      description: 'Conecte-se com laboratórios, convênios e sistemas de pagamento de forma simplificada.',
      features: ['APIs abertas', 'Integração PIX', 'Convênios']
    }
  ];

  testimonials: Testimonial[] = [
    {
      name: 'Dra. Mariana Silva',
      role: 'Cardiologista',
      clinic: 'Clínica Coração Saudável',
      content: 'O Omni Care revolucionou a forma como gerencio minha clínica. A telemedicina integrada me permite atender pacientes de todo o Brasil.',
      rating: 5,
      initials: 'MS'
    },
    {
      name: 'Dr. Ricardo Mendes',
      role: 'Diretor Clínico',
      clinic: 'Centro Médico Vida',
      content: 'Reduzimos em 40% as faltas de pacientes com os lembretes automáticos. O sistema de agendamento online é simplesmente perfeito.',
      rating: 5,
      initials: 'RM'
    },
    {
      name: 'Dra. Ana Paula Costa',
      role: 'Dermatologista',
      clinic: 'Dermato Estética',
      content: 'A integração com laboratórios e a prescrição digital economizam horas do meu dia. Consigo focar no que realmente importa: meus pacientes.',
      rating: 5,
      initials: 'AC'
    }
  ];

  companies = ['Hospital A', 'Clínica B', 'Centro C', 'Lab D', 'Plano E'];

  createRange(num: number): number[] {
    return Array(num).fill(0).map((_, i) => i);
  }
}
