import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MarketingAutomationService } from '../../../services/crm';
import { MarketingAutomation as MarketingAutomationModel } from '../../../models/crm';

@Component({
  selector: 'app-marketing-automation',
  imports: [CommonModule, FormsModule],
  templateUrl: './marketing-automation.html',
  styleUrl: './marketing-automation.scss'
})
export class MarketingAutomation implements OnInit {
  campaigns = signal<MarketingAutomationModel[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private marketingAutomationService: MarketingAutomationService) {}

  ngOnInit(): void {
    this.loadCampaigns();
  }

  loadCampaigns(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.marketingAutomationService.getAll().subscribe({
      next: (campaigns) => {
        this.campaigns.set(campaigns);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading marketing campaigns:', error);
        this.errorMessage.set(error.userMessage || error.message || 'Erro ao carregar campanhas');
        this.campaigns.set([]);
        this.isLoading.set(false);
      }
    });
  }

  onNewCampaign(): void {
    console.log('Nova Campanha clicked');
    alert('Funcionalidade "Nova Campanha" será implementada em breve.');
  }
}
