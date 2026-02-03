import { Component, OnInit, HostListener } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';
import { WebsiteAnalyticsService } from '../../../services/analytics/website-analytics.service';

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
  selector: 'app-home',
  imports: [CommonModule, RouterLink, HeaderComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent implements OnInit {
  whatsappNumber = environment.whatsappNumber;
  private pageStartTime = Date.now();
  private scrollDepthTracked = { 25: false, 50: false, 75: false, 100: false };
  private scrollSubject = new Subject<void>();
  
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
  
  constructor(private analytics: WebsiteAnalyticsService) {}

  ngOnInit(): void {
    // Track page view
    this.analytics.trackPageView('/home', 'PrimeCare Software - Home');
    
    // Setup throttled scroll tracking
    this.scrollSubject
      .pipe(throttleTime(500))
      .subscribe(() => this.trackScrollDepth());
  }


  
  @HostListener('window:scroll')
  onScroll(): void {
    // Emit scroll event to throttled subject
    this.scrollSubject.next();
  }
  
  private trackScrollDepth(): void {
    const scrollHeight = document.documentElement.scrollHeight;
    const clientHeight = window.innerHeight;
    const scrollTop = window.scrollY;
    
    const scrollableHeight = scrollHeight - clientHeight;
    if (scrollableHeight <= 0) {
      return;
    }
    
    const scrollPercent = Math.round((scrollTop / scrollableHeight) * 100);
    
    const milestones = [25, 50, 75, 100] as const;
    milestones.forEach(milestone => {
      if (scrollPercent >= milestone && !this.scrollDepthTracked[milestone]) {
        this.scrollDepthTracked[milestone] = true;
        this.analytics.trackScrollDepth(milestone, 'home');
      }
    });
  }
  
  trackCTA(ctaName: string, ctaLocation: string = 'hero'): void {
    this.analytics.trackCTAClick(ctaName, ctaLocation);
    this.analytics.trackConversion('trial_signup');
  }
  
  trackNavigation(destination: string): void {
    this.analytics.trackNavigation(destination, 'home');
  }

  openWhatsApp(): void {
    this.analytics.trackCTAClick('WhatsApp', 'floating-button');
    this.analytics.trackConversion('contact');
    window.open(`https://wa.me/${this.whatsappNumber}`, '_blank');
  }

  createRange(num: number): number[] {
    return Array(num).fill(0).map((_, i) => i);
  }
}
