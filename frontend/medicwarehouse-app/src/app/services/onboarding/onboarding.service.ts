import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface OnboardingStep {
  id: string;
  title: string;
  description: string;
  completed: boolean;
  route?: string;
  action?: () => void;
}

export interface OnboardingProgress {
  totalSteps: number;
  completedSteps: number;
  percentage: number;
  steps: OnboardingStep[];
}

@Injectable({
  providedIn: 'root'
})
export class OnboardingService {
  private readonly STORAGE_KEY = 'primecare_onboarding_progress';
  private readonly ONBOARDING_COMPLETED_KEY = 'primecare_onboarding_completed';
  
  private progressSubject = new BehaviorSubject<OnboardingProgress>(this.loadProgress());
  public progress$: Observable<OnboardingProgress> = this.progressSubject.asObservable();

  private defaultSteps: OnboardingStep[] = [
    {
      id: 'configure_hours',
      title: 'Configure horários de atendimento',
      description: 'Defina os horários em que sua clínica atende pacientes',
      completed: false,
      route: '/app/settings/schedule'
    },
    {
      id: 'add_patient',
      title: 'Adicione um paciente',
      description: 'Cadastre seu primeiro paciente no sistema',
      completed: false,
      route: '/app/patients/new'
    },
    {
      id: 'schedule_appointment',
      title: 'Agende uma consulta',
      description: 'Crie seu primeiro agendamento na agenda',
      completed: false,
      route: '/app/appointments/new'
    },
    {
      id: 'perform_attendance',
      title: 'Realize um atendimento',
      description: 'Preencha um prontuário durante uma consulta',
      completed: false,
      route: '/app/attendance'
    },
    {
      id: 'issue_prescription',
      title: 'Emita uma prescrição',
      description: 'Crie sua primeira prescrição digital',
      completed: false,
      route: '/app/prescriptions/new'
    }
  ];

  constructor() {}

  /**
   * Load onboarding progress from localStorage
   */
  private loadProgress(): OnboardingProgress {
    const stored = localStorage.getItem(this.STORAGE_KEY);
    if (stored) {
      try {
        const steps = JSON.parse(stored) as OnboardingStep[];
        return this.calculateProgress(steps);
      } catch (error) {
        console.error('Error loading onboarding progress, clearing corrupted data:', error);
        // Clear corrupted data to prevent repeated errors
        localStorage.removeItem(this.STORAGE_KEY);
      }
    }
    return this.calculateProgress(this.defaultSteps);
  }

  /**
   * Calculate progress metrics from steps
   */
  private calculateProgress(steps: OnboardingStep[]): OnboardingProgress {
    const totalSteps = steps.length;
    const completedSteps = steps.filter(s => s.completed).length;
    const percentage = totalSteps > 0 ? Math.round((completedSteps / totalSteps) * 100) : 0;
    
    return {
      totalSteps,
      completedSteps,
      percentage,
      steps
    };
  }

  /**
   * Save progress to localStorage
   */
  private saveProgress(steps: OnboardingStep[]): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(steps));
  }

  /**
   * Mark a step as completed
   */
  completeStep(stepId: string): void {
    const currentProgress = this.progressSubject.value;
    const updatedSteps = currentProgress.steps.map(step =>
      step.id === stepId ? { ...step, completed: true } : step
    );
    
    this.saveProgress(updatedSteps);
    this.progressSubject.next(this.calculateProgress(updatedSteps));

    // Check if all steps are completed
    if (updatedSteps.every(s => s.completed)) {
      this.markOnboardingAsCompleted();
    }
  }

  /**
   * Reset a specific step
   */
  resetStep(stepId: string): void {
    const currentProgress = this.progressSubject.value;
    const updatedSteps = currentProgress.steps.map(step =>
      step.id === stepId ? { ...step, completed: false } : step
    );
    
    this.saveProgress(updatedSteps);
    this.progressSubject.next(this.calculateProgress(updatedSteps));
  }

  /**
   * Reset all onboarding progress
   */
  resetOnboarding(): void {
    const resetSteps = this.defaultSteps.map(step => ({ ...step, completed: false }));
    this.saveProgress(resetSteps);
    localStorage.removeItem(this.ONBOARDING_COMPLETED_KEY);
    this.progressSubject.next(this.calculateProgress(resetSteps));
  }

  /**
   * Check if onboarding is completed
   */
  isOnboardingCompleted(): boolean {
    return localStorage.getItem(this.ONBOARDING_COMPLETED_KEY) === 'true';
  }

  /**
   * Mark onboarding as completed
   */
  markOnboardingAsCompleted(): void {
    localStorage.setItem(this.ONBOARDING_COMPLETED_KEY, 'true');
  }

  /**
   * Skip onboarding
   */
  skipOnboarding(): void {
    this.markOnboardingAsCompleted();
    // Notify subscribers that onboarding is complete
    const currentProgress = this.progressSubject.value;
    this.progressSubject.next({ ...currentProgress, percentage: 100 });
  }

  /**
   * Get current progress
   */
  getCurrentProgress(): OnboardingProgress {
    return this.progressSubject.value;
  }

  /**
   * Check if user should see onboarding
   */
  shouldShowOnboarding(): boolean {
    if (this.isOnboardingCompleted()) {
      return false;
    }
    
    const progress = this.getCurrentProgress();
    return progress.completedSteps < progress.totalSteps;
  }

  /**
   * Get next incomplete step
   */
  getNextStep(): OnboardingStep | null {
    const progress = this.getCurrentProgress();
    const nextStep = progress.steps.find(s => !s.completed);
    return nextStep || null;
  }
}
