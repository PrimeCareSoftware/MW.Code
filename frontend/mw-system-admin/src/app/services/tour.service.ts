import { Injectable } from '@angular/core';
import Shepherd from 'shepherd.js';

export interface TourStep {
  id: string;
  text: string;
  attachTo?: {
    element: string;
    on: 'top' | 'bottom' | 'left' | 'right';
  };
  buttons?: Array<{
    text: string;
    action: 'next' | 'back' | 'cancel' | 'complete';
    classes?: string;
  }>;
}

export interface TourConfig {
  id: string;
  steps: TourStep[];
  useModalOverlay?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private currentTour?: Shepherd.Tour;
  private completedTours: Set<string> = new Set();
  
  constructor() {
    this.loadCompletedTours();
  }
  
  private loadCompletedTours(): void {
    const completed = localStorage.getItem('completed-tours');
    if (completed) {
      try {
        const tours = JSON.parse(completed);
        this.completedTours = new Set(tours);
      } catch (e) {
        console.error('Error loading completed tours:', e);
      }
    }
  }
  
  private saveCompletedTour(tourId: string): void {
    this.completedTours.add(tourId);
    localStorage.setItem('completed-tours', JSON.stringify([...this.completedTours]));
  }
  
  startDashboardTour(): void {
    if (this.isTourCompleted('dashboard-tour')) {
      return;
    }
    
    this.currentTour = new Shepherd.Tour({
      useModalOverlay: true,
      defaultStepOptions: {
        cancelIcon: {
          enabled: true
        },
        classes: 'shepherd-theme-custom',
        scrollTo: { behavior: 'smooth', block: 'center' }
      }
    });
    
    this.currentTour.addStep({
      id: 'welcome',
      text: '<h3>Bem-vindo ao System Admin!</h3><p>Vamos fazer um tour rápido pelas principais funcionalidades.</p>',
      buttons: [
        {
          text: 'Pular',
          action: () => this.currentTour?.cancel(),
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Começar',
          action: () => this.currentTour?.next()
        }
      ]
    });
    
    this.currentTour.addStep({
      id: 'metrics',
      text: '<h3>Métricas Principais</h3><p>Aqui você vê as métricas principais do seu negócio: MRR, clientes ativos, churn rate e muito mais.</p>',
      attachTo: {
        element: '.kpi-cards',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: () => this.currentTour?.back(),
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Próximo',
          action: () => this.currentTour?.next()
        }
      ]
    });
    
    this.currentTour.addStep({
      id: 'navigation',
      text: '<h3>Menu de Navegação</h3><p>Use o menu lateral para acessar rapidamente todas as seções do sistema.</p>',
      attachTo: {
        element: '.sidenav-container',
        on: 'right'
      },
      buttons: [
        {
          text: 'Anterior',
          action: () => this.currentTour?.back(),
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Próximo',
          action: () => this.currentTour?.next()
        }
      ]
    });
    
    this.currentTour.addStep({
      id: 'search',
      text: '<h3>Busca Rápida</h3><p>Use <kbd>Ctrl+K</kbd> para buscar rapidamente clínicas, usuários, tickets e muito mais!</p>',
      attachTo: {
        element: '.search-button',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: () => this.currentTour?.back(),
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Próximo',
          action: () => this.currentTour?.next()
        }
      ]
    });
    
    this.currentTour.addStep({
      id: 'notifications',
      text: '<h3>Notificações</h3><p>Fique de olho nas notificações importantes. Você receberá alertas sobre assinaturas vencidas, trials expirando e mais.</p>',
      attachTo: {
        element: '.notification-button',
        on: 'bottom'
      },
      buttons: [
        {
          text: 'Anterior',
          action: () => this.currentTour?.back(),
          classes: 'shepherd-button-secondary'
        },
        {
          text: 'Concluir',
          action: () => this.currentTour?.complete()
        }
      ]
    });
    
    this.currentTour.on('complete', () => {
      this.saveCompletedTour('dashboard-tour');
    });
    
    // Don't save as completed on cancel - allow users to see it again
    this.currentTour.on('cancel', () => {
      console.log('Tour cancelled by user');
    });
    
    this.currentTour.start();
  }
  
  startCustomTour(config: TourConfig): void {
    if (this.isTourCompleted(config.id)) {
      return;
    }
    
    this.currentTour = new Shepherd.Tour({
      useModalOverlay: config.useModalOverlay ?? true,
      defaultStepOptions: {
        cancelIcon: {
          enabled: true
        },
        classes: 'shepherd-theme-custom',
        scrollTo: { behavior: 'smooth', block: 'center' }
      }
    });
    
    config.steps.forEach(step => {
      const buttons = step.buttons?.map(btn => ({
        text: btn.text,
        action: this.getButtonAction(btn.action),
        classes: btn.classes || ''
      })) || [];
      
      this.currentTour!.addStep({
        id: step.id,
        text: step.text,
        attachTo: step.attachTo,
        buttons
      });
    });
    
    this.currentTour.on('complete', () => {
      this.saveCompletedTour(config.id);
    });
    
    // Don't save as completed on cancel - allow users to see it again
    this.currentTour.on('cancel', () => {
      console.log(`Tour ${config.id} cancelled by user`);
    });
    
    this.currentTour.start();
  }
  
  private getButtonAction(action: string): () => void {
    switch (action) {
      case 'next':
        return () => this.currentTour?.next();
      case 'back':
        return () => this.currentTour?.back();
      case 'cancel':
        return () => this.currentTour?.cancel();
      case 'complete':
        return () => this.currentTour?.complete();
      default:
        return () => this.currentTour?.next();
    }
  }
  
  shouldShowTour(tourId: string): boolean {
    return !this.isTourCompleted(tourId);
  }
  
  isTourCompleted(tourId: string): boolean {
    return this.completedTours.has(tourId);
  }
  
  resetTour(tourId: string): void {
    this.completedTours.delete(tourId);
    localStorage.setItem('completed-tours', JSON.stringify([...this.completedTours]));
  }
  
  resetAllTours(): void {
    this.completedTours.clear();
    localStorage.removeItem('completed-tours');
  }
  
  cancelCurrentTour(): void {
    this.currentTour?.cancel();
  }
}
