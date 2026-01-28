import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { OnboardingService, OnboardingProgress, OnboardingStep } from '../../../services/onboarding/onboarding.service';

@Component({
  selector: 'app-onboarding-progress',
  imports: [CommonModule, RouterLink],
  templateUrl: './onboarding-progress.component.html',
  styleUrl: './onboarding-progress.component.scss'
})
export class OnboardingProgressComponent implements OnInit, OnDestroy {
  progress: OnboardingProgress | null = null;
  private subscription?: Subscription;

  constructor(private onboardingService: OnboardingService) {}

  ngOnInit(): void {
    this.subscription = this.onboardingService.progress$.subscribe(progress => {
      this.progress = progress;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  getStepIcon(step: OnboardingStep): string {
    const icons: { [key: string]: string } = {
      'configure_hours': 'schedule',
      'add_patient': 'person_add',
      'schedule_appointment': 'event',
      'perform_attendance': 'medical_services',
      'issue_prescription': 'description'
    };
    return icons[step.id] || 'check_circle';
  }

  onStepClick(step: OnboardingStep): void {
    if (step.route) {
      // Navigation is handled by routerLink in template
    } else if (step.action) {
      step.action();
    }
  }

  skipOnboarding(): void {
    this.onboardingService.skipOnboarding();
  }
}
