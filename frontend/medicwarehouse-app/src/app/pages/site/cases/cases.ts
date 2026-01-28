import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

interface CaseMetric {
  label: string;
  value: string;
  icon: string;
}

interface SuccessCase {
  id: string;
  clinicName: string;
  specialty: string;
  location: string;
  image: string;
  quote: string;
  authorName: string;
  authorRole: string;
  metrics: CaseMetric[];
  challenges: string[];
  solutions: string[];
  results: string[];
}

@Component({
  selector: 'app-cases',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './cases.html',
  styleUrl: './cases.scss'
})
export class CasesComponent {
  cases: SuccessCase[] = [
    {
      id: 'clinica-sorriso',
      clinicName: 'Clínica Sorriso',
      specialty: 'Odontologia',
      location: 'São Paulo, SP',
      image: '/assets/images/cases/clinica-sorriso.jpg',
      quote: 'O PrimeCare reduziu nosso tempo de agendamento em 70% e eliminou completamente os erros de prontuário.',
      authorName: 'Dra. Maria Silva',
      authorRole: 'Diretora Clínica',
      metrics: [
        { label: 'Redução no tempo de agendamento', value: '70%', icon: 'schedule' },
        { label: 'Aumento na satisfação dos pacientes', value: '45%', icon: 'sentiment_very_satisfied' },
        { label: 'Economia mensal', value: 'R$ 3.500', icon: 'attach_money' },
        { label: 'ROI em', value: '2 meses', icon: 'trending_up' }
      ],
      challenges: [
        'Agendamento manual via telefone e WhatsApp',
        'Prontuários em papel desorganizados',
        'Dificuldade em localizar histórico de pacientes',
        'Falta de controle financeiro'
      ],
      solutions: [
        'Agenda online integrada com lembretes automáticos',
        'Prontuário eletrônico com busca rápida',
        'Histórico completo do paciente em um clique',
        'Dashboard financeiro com relatórios detalhados'
      ],
      results: [
        '70% menos tempo gasto em agendamentos',
        '45% aumento na satisfação dos pacientes',
        'Zero erros de prontuário',
        'R$ 3.500 economia mensal em papel e arquivamento'
      ]
    },
    {
      id: 'consultorio-dr-santos',
      clinicName: 'Consultório Dr. Santos',
      specialty: 'Cardiologia',
      location: 'Rio de Janeiro, RJ',
      image: '/assets/images/cases/consultorio-santos.jpg',
      quote: 'Consegui atender 30% mais pacientes por mês sem contratar mais funcionários. O sistema é intuitivo e rápido.',
      authorName: 'Dr. João Santos',
      authorRole: 'Cardiologista',
      metrics: [
        { label: 'Aumento na capacidade', value: '30%', icon: 'people' },
        { label: 'Redução em faltas', value: '60%', icon: 'event_available' },
        { label: 'Tempo economizado por dia', value: '2h', icon: 'timer' },
        { label: 'Pacientes atendidos/mês', value: '+45', icon: 'show_chart' }
      ],
      challenges: [
        'Alta taxa de faltas (40%)',
        'Tempo excessivo em tarefas administrativas',
        'Dificuldade em acompanhar pacientes crônicos',
        'Falta de dados para tomar decisões'
      ],
      solutions: [
        'Lembretes automáticos via WhatsApp e SMS',
        'Automação de tarefas administrativas',
        'Alertas e lembretes para consultas de retorno',
        'Dashboard com métricas em tempo real'
      ],
      results: [
        '60% redução em faltas de consulta',
        '2 horas economizadas por dia',
        '30% aumento na capacidade de atendimento',
        '45 pacientes a mais atendidos por mês'
      ]
    },
    {
      id: 'clinica-vida-saudavel',
      clinicName: 'Clínica Vida Saudável',
      specialty: 'Clínica Médica',
      location: 'Belo Horizonte, MG',
      image: '/assets/images/cases/vida-saudavel.jpg',
      quote: 'A telemedicina integrada foi um diferencial durante a pandemia. Hoje, 40% das nossas consultas são online.',
      authorName: 'Dr. Pedro Costa',
      authorRole: 'Diretor Médico',
      metrics: [
        { label: 'Consultas online', value: '40%', icon: 'videocam' },
        { label: 'Crescimento de receita', value: '55%', icon: 'attach_money' },
        { label: 'Alcance regional', value: '5 cidades', icon: 'location_on' },
        { label: 'NPS', value: '92', icon: 'star' }
      ],
      challenges: [
        'Limitação geográfica (apenas presencial)',
        'Perda de pacientes na pandemia',
        'Sistema antigo sem telemedicina',
        'Falta de integração com laboratórios'
      ],
      solutions: [
        'Telemedicina integrada e segura',
        'Agendamento híbrido (online e presencial)',
        'Prescrição digital com assinatura eletrônica',
        'Integração com laboratórios parceiros'
      ],
      results: [
        '40% das consultas agora são online',
        '55% de crescimento na receita',
        'Atende pacientes em 5 cidades diferentes',
        'NPS de 92 (excelente)'
      ]
    }
  ];

  filteredCases: SuccessCase[] = this.cases;
  selectedSpecialty: string = 'all';

  specialties = [
    { value: 'all', label: 'Todas as Especialidades' },
    { value: 'Odontologia', label: 'Odontologia' },
    { value: 'Cardiologia', label: 'Cardiologia' },
    { value: 'Clínica Médica', label: 'Clínica Médica' },
    { value: 'Dermatologia', label: 'Dermatologia' },
    { value: 'Ortopedia', label: 'Ortopedia' }
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
    // Scroll to contact section or navigate to contact page
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
  }
}
