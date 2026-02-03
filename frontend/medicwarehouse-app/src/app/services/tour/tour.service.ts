import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import Shepherd from 'shepherd.js';

export type TourType = 'first-login' | 'first-consultation' | 'first-record';

export interface TourStep {
  id: string;
  title: string;
  text: string;
  attachTo?: {
    element: string;
    on: 'top' | 'bottom' | 'left' | 'right';
  };
  buttons?: Array<{
    text: string;
    action: () => void;
    classes?: string;
  }>;
  when?: {
    show?: () => void;
    hide?: () => void;
  };
}

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private currentTour: any | null = null;
  private readonly TOUR_COMPLETED_KEY_PREFIX = 'primecare_tour_completed_';

  constructor(private router: Router) {}

  /**
   * Check if a specific tour has been completed
   */
  isTourCompleted(tourType: TourType): boolean {
    return localStorage.getItem(`${this.TOUR_COMPLETED_KEY_PREFIX}${tourType}`) === 'true';
  }

  /**
   * Mark a tour as completed
   */
  markTourAsCompleted(tourType: TourType): void {
    localStorage.setItem(`${this.TOUR_COMPLETED_KEY_PREFIX}${tourType}`, 'true');
  }

  /**
   * Reset a specific tour
   */
  resetTour(tourType: TourType): void {
    localStorage.removeItem(`${this.TOUR_COMPLETED_KEY_PREFIX}${tourType}`);
  }

  /**
   * Start the First Login tour (Dashboard → Agenda → Patients → Settings)
   */
  startFirstLoginTour(): void {
    if (this.isTourCompleted('first-login')) {
      return;
    }

    this.currentTour = new Shepherd.Tour({
      useModalOverlay: true,
      defaultStepOptions: {
        classes: 'shepherd-theme-primecare',
        scrollTo: { behavior: 'smooth', block: 'center' },
        cancelIcon: {
          enabled: true
        }
      }
    });

    this.currentTour.addSteps([
      {
        id: 'welcome',
        title: '👋 Bem-vindo ao Omni Care!',
        text: 'Vamos fazer um tour rápido pelas principais funcionalidades do sistema. Isso levará apenas 2 minutos.',
        buttons: [
          {
            text: 'Pular',
            action: () => {
              this.currentTour?.cancel();
              this.markTourAsCompleted('first-login');
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Começar',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'dashboard',
        title: '📊 Dashboard',
        text: 'Aqui você vê um resumo da sua clínica: consultas do dia, pacientes atendidos e receita do mês.',
        attachTo: {
          element: '.dashboard-summary',
          on: 'bottom'
        },
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'menu',
        title: '📱 Menu Principal',
        text: 'Use este menu para navegar entre as diferentes áreas do sistema.',
        attachTo: {
          element: '.sidebar-menu',
          on: 'right'
        },
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => {
              this.router.navigate(['/app/agenda']);
              setTimeout(() => this.currentTour?.next(), 500);
            }
          }
        ]
      },
      {
        id: 'agenda',
        title: '📅 Agenda',
        text: 'Na Agenda você gerencia todas as consultas. Clique em um horário vazio para agendar.',
        buttons: [
          {
            text: 'Voltar',
            action: () => {
              this.router.navigate(['/app/dashboard']);
              setTimeout(() => this.currentTour?.back(), 500);
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => {
              this.router.navigate(['/app/patients']);
              setTimeout(() => this.currentTour?.next(), 500);
            }
          }
        ]
      },
      {
        id: 'patients',
        title: '👥 Pacientes',
        text: 'Aqui você cadastra e gerencia todos os seus pacientes. Use a busca para encontrar rapidamente.',
        buttons: [
          {
            text: 'Voltar',
            action: () => {
              this.router.navigate(['/app/agenda']);
              setTimeout(() => this.currentTour?.back(), 500);
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => {
              this.router.navigate(['/app/settings']);
              setTimeout(() => this.currentTour?.next(), 500);
            }
          }
        ]
      },
      {
        id: 'settings',
        title: '⚙️ Configurações',
        text: 'Configure horários de atendimento, dados da clínica e preferências do sistema.',
        buttons: [
          {
            text: 'Voltar',
            action: () => {
              this.router.navigate(['/app/patients']);
              setTimeout(() => this.currentTour?.back(), 500);
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Finalizar',
            action: () => {
              this.currentTour?.complete();
              this.markTourAsCompleted('first-login');
              this.router.navigate(['/app/dashboard']);
            }
          }
        ]
      }
    ]);

    this.currentTour.on('cancel', () => {
      this.markTourAsCompleted('first-login');
    });

    this.currentTour.start();
  }

  /**
   * Start the First Consultation tour (contextual)
   */
  startFirstConsultationTour(): void {
    if (this.isTourCompleted('first-consultation')) {
      return;
    }

    this.currentTour = new Shepherd.Tour({
      useModalOverlay: true,
      defaultStepOptions: {
        classes: 'shepherd-theme-primecare',
        scrollTo: { behavior: 'smooth', block: 'center' },
        cancelIcon: {
          enabled: true
        }
      }
    });

    this.currentTour.addSteps([
      {
        id: 'consultation-welcome',
        title: '🩺 Primeira Consulta',
        text: 'Vamos aprender como agendar e gerenciar consultas no Omni Care.',
        buttons: [
          {
            text: 'Pular',
            action: () => {
              this.currentTour?.cancel();
              this.markTourAsCompleted('first-consultation');
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Começar',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'select-date',
        title: '📅 Selecione a Data',
        text: 'Escolha o dia e horário da consulta clicando em um espaço disponível na agenda.',
        attachTo: {
          element: '.calendar-view',
          on: 'top'
        },
        buttons: [
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'select-patient',
        title: '👤 Selecione o Paciente',
        text: 'Busque e selecione o paciente para esta consulta. Se o paciente ainda não existe, você pode cadastrá-lo rapidamente.',
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'consultation-type',
        title: '🏥 Tipo de Consulta',
        text: 'Escolha o tipo de consulta (primeira consulta, retorno, emergência, etc.) e adicione observações se necessário.',
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'reminders',
        title: '🔔 Lembretes Automáticos',
        text: 'O sistema pode enviar lembretes automáticos por WhatsApp ou SMS para o paciente antes da consulta.',
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Finalizar',
            action: () => {
              this.currentTour?.complete();
              this.markTourAsCompleted('first-consultation');
            }
          }
        ]
      }
    ]);

    this.currentTour.on('cancel', () => {
      this.markTourAsCompleted('first-consultation');
    });

    this.currentTour.start();
  }

  /**
   * Start the First Medical Record tour (prontuário SOAP)
   */
  startFirstRecordTour(): void {
    if (this.isTourCompleted('first-record')) {
      return;
    }

    this.currentTour = new Shepherd.Tour({
      useModalOverlay: true,
      defaultStepOptions: {
        classes: 'shepherd-theme-primecare',
        scrollTo: { behavior: 'smooth', block: 'center' },
        cancelIcon: {
          enabled: true
        }
      }
    });

    this.currentTour.addSteps([
      {
        id: 'record-welcome',
        title: '📋 Prontuário Eletrônico',
        text: 'Vamos aprender como preencher e gerenciar prontuários no formato SOAP.',
        buttons: [
          {
            text: 'Pular',
            action: () => {
              this.currentTour?.cancel();
              this.markTourAsCompleted('first-record');
            },
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Começar',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'soap-s',
        title: '🗣️ S - Subjetivo',
        text: 'Registre a queixa principal e história clínica relatada pelo paciente.',
        attachTo: {
          element: '#soap-subjective',
          on: 'right'
        },
        buttons: [
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'soap-o',
        title: '🔍 O - Objetivo',
        text: 'Registre sinais vitais, exame físico e resultados de exames.',
        attachTo: {
          element: '#soap-objective',
          on: 'right'
        },
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'soap-a',
        title: '💡 A - Avaliação',
        text: 'Registre a hipótese diagnóstica e avaliação clínica.',
        attachTo: {
          element: '#soap-assessment',
          on: 'right'
        },
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'soap-p',
        title: '📝 P - Plano',
        text: 'Defina o plano de tratamento, prescrições e próximos passos.',
        attachTo: {
          element: '#soap-plan',
          on: 'right'
        },
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Próximo',
            action: () => this.currentTour?.next()
          }
        ]
      },
      {
        id: 'attachments',
        title: '📎 Anexos',
        text: 'Você pode anexar exames, imagens e outros documentos ao prontuário.',
        buttons: [
          {
            text: 'Voltar',
            action: () => this.currentTour?.back(),
            classes: 'shepherd-button-secondary'
          },
          {
            text: 'Finalizar',
            action: () => {
              this.currentTour?.complete();
              this.markTourAsCompleted('first-record');
            }
          }
        ]
      }
    ]);

    this.currentTour.on('cancel', () => {
      this.markTourAsCompleted('first-record');
    });

    this.currentTour.start();
  }

  /**
   * Cancel the current tour
   */
  cancelTour(): void {
    if (this.currentTour) {
      this.currentTour.cancel();
      this.currentTour = null;
    }
  }

  /**
   * Complete the current tour
   */
  completeTour(): void {
    if (this.currentTour) {
      this.currentTour.complete();
      this.currentTour = null;
    }
  }
}
