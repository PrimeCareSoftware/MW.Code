import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-survey-list',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './survey-list.html',
  styleUrl: './survey-list.scss'
})
export class SurveyList implements OnInit {
  surveys = signal<any[]>([]);
  isLoading = signal<boolean>(false);

  ngOnInit(): void {
    this.loadSurveys();
  }

  async loadSurveys(): Promise<void> {
    this.isLoading.set(true);
    try {
      // TODO: Integrate with SurveyService when API is connected
      // For now, just set empty array
      this.surveys.set([]);
    } catch (error) {
      console.error('Error loading surveys:', error);
    } finally {
      this.isLoading.set(false);
    }
  }

  onNewSurvey(): void {
    // TODO: Open modal or navigate to survey creation form
    console.log('Nova Pesquisa clicked');
    alert('Funcionalidade "Nova Pesquisa" será implementada em breve.');
  }
}
