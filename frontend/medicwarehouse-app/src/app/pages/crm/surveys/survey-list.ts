import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';
import { SurveyService } from '../../../services/crm';
import { Survey } from '../../../models/crm';

@Component({
  selector: 'app-survey-list',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './survey-list.html',
  styleUrl: './survey-list.scss'
})
export class SurveyList implements OnInit {
  surveys = signal<Survey[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private surveyService: SurveyService) {}

  ngOnInit(): void {
    this.loadSurveys();
  }

  loadSurveys(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.surveyService.getAll().subscribe({
      next: (surveys) => {
        this.surveys.set(surveys);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading surveys:', error);
        this.errorMessage.set(error.message || 'Erro ao carregar pesquisas');
        this.isLoading.set(false);
      }
    });
  }

  onNewSurvey(): void {
    console.log('Nova Pesquisa clicked');
    alert('Funcionalidade "Nova Pesquisa" será implementada em breve.');
  }
}
