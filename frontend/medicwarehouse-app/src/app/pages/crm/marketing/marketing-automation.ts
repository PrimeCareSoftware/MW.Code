import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../../shared/navbar/navbar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-marketing-automation',
  imports: [CommonModule, Navbar, FormsModule],
  templateUrl: './marketing-automation.html',
  styleUrl: './marketing-automation.scss'
})
export class MarketingAutomation implements OnInit {
  campaigns = signal<any[]>([]);
  isLoading = signal<boolean>(false);

  ngOnInit(): void {
    this.loadCampaigns();
  }

  async loadCampaigns(): Promise<void> {
    this.isLoading.set(true);
    try {
      // TODO: Integrate with MarketingAutomationService when API is connected
      // For now, just set empty array
      this.campaigns.set([]);
    } catch (error) {
      console.error('Error loading marketing campaigns:', error);
    } finally {
      this.isLoading.set(false);
    }
  }

  onNewCampaign(): void {
    // TODO: Open modal or navigate to campaign creation form
    console.log('Nova Campanha clicked');
    alert('Funcionalidade "Nova Campanha" será implementada em breve.');
  }
}
