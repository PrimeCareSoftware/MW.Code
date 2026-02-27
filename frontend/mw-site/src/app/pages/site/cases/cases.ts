import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

interface CaseMetric {
  label: string;
  value: string;
  icon: string;
}

export interface SuccessCase {
  id: string;
  clinicName: string;
  specialty: string;
  location: string;
  quote: string;
  authorName: string;
  authorRole: string;
  metrics: CaseMetric[];
}

@Component({
  selector: 'app-cases',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './cases.html',
  styleUrl: './cases.scss'
})
export class CasesComponent {
  constructor(private analytics: WebsiteAnalyticsService) {}

  ngOnInit(): void {
    // Track page view
    this.analytics.trackPageView('/site/cases', 'Cases de Sucesso - Omni Care');
  }

  onCaseView(caseItem: SuccessCase): void {
    this.analytics.trackCaseStudyView(caseItem.clinicName);
  }

  onContactClick(): void {
    this.analytics.trackCTAClick('Fale Conosco', 'Cases Page');
  }

  cases: SuccessCase[] = [
    {
      id: 'clinica-sorriso',
      clinicName: 'Clínica Sorriso',
      specialty: 'Odontologia',
      location: 'São Paulo, SP',
      quote: 'O Omni Care reduziu nosso tempo de agendamento em 70% e eliminou completamente os erros de prontuário.',
      authorName: 'Dra. Maria Silva',
      authorRole: 'Diretora Clínica',
      metrics: [
        { label: 'Redução no tempo de agendamento', value: '70%', icon: 'schedule' },
        { label: 'Aumento na satisfação dos pacientes', value: '45%', icon: 'sentiment_very_satisfied' },
        { label: 'Economia mensal', value: 'R$ 3.500', icon: 'attach_money' },
        { label: 'ROI em', value: '2 meses', icon: 'trending_up' }
      ]
    },
    {
      id: 'consultorio-dr-santos',
      clinicName: 'Consultório Dr. Santos',
      specialty: 'Cardiologia',
      location: 'Rio de Janeiro, RJ',
      quote: 'Consegui atender 30% mais pacientes por mês sem contratar mais funcionários. O sistema é intuitivo e rápido.',
      authorName: 'Dr. João Santos',
      authorRole: 'Cardiologista',
      metrics: [
        { label: 'Aumento na capacidade', value: '30%', icon: 'people' },
        { label: 'Redução em faltas', value: '60%', icon: 'event_available' },
        { label: 'Tempo economizado por dia', value: '2h', icon: 'timer' },
        { label: 'Pacientes atendidos/mês', value: '+45', icon: 'show_chart' }
      ]
    },
    {
      id: 'clinica-vida-saudavel',
      clinicName: 'Clínica Vida Saudável',
      specialty: 'Clínica Médica',
      location: 'Belo Horizonte, MG',
      quote: 'A telemedicina integrada foi um diferencial durante a pandemia. Hoje, 40% das nossas consultas são online.',
      authorName: 'Dr. Pedro Costa',
      authorRole: 'Diretor Médico',
      metrics: [
        { label: 'Consultas online', value: '40%', icon: 'videocam' },
        { label: 'Crescimento de receita', value: '55%', icon: 'attach_money' },
        { label: 'Alcance regional', value: '5 cidades', icon: 'location_on' },
        { label: 'NPS', value: '92', icon: 'star' }
      ]
    }
  ];

  filteredCases: SuccessCase[] = this.cases;
  selectedSpecialty: string = 'all';

  specialties = [
    { value: 'all', label: 'Todas as Especialidades' },
    { value: 'Odontologia', label: 'Odontologia' },
    { value: 'Cardiologia', label: 'Cardiologia' },
    { value: 'Clínica Médica', label: 'Clínica Médica' }
  ];

  filterBySpecialty(specialty: string): void {
    this.selectedSpecialty = specialty;
    if (specialty === 'all') {
      this.filteredCases = this.cases;
    } else {
      this.filteredCases = this.cases.filter(c => c.specialty === specialty);
    }
  }

  scrollToContact(): void {
    // Navigate to contact page or scroll to contact section if available
    const contactSection = document.querySelector('#contact-section');
    if (contactSection) {
      contactSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } else {
      // Fallback: scroll to bottom if no specific contact section exists
      window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    }
  }

  getIconLabel(icon: string): string {
    const labels: { [key: string]: string } = {
      'schedule': 'Ícone de relógio',
      'sentiment_very_satisfied': 'Ícone de satisfação',
      'attach_money': 'Ícone de dinheiro',
      'trending_up': 'Ícone de crescimento',
      'people': 'Ícone de pessoas',
      'event_available': 'Ícone de evento',
      'timer': 'Ícone de temporizador',
      'show_chart': 'Ícone de gráfico',
      'videocam': 'Ícone de videocâmera',
      'location_on': 'Ícone de localização',
      'star': 'Ícone de estrela'
    };
    return labels[icon] || 'Ícone';
  }
}
