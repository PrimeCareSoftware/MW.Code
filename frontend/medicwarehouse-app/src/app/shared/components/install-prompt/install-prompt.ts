import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PwaService } from '../../../services/pwa.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-install-prompt',
  templateUrl: './install-prompt.html',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  styleUrls: ['./install-prompt.scss']
})
export class InstallPromptComponent implements OnInit {
  canInstall$!: Observable<boolean>;
  showPrompt = true;
  
  constructor(private pwaService: PwaService) {}
  
  ngOnInit(): void {
    this.canInstall$ = this.pwaService.canInstall$;
  }
  
  async install(): Promise<void> {
    const installed = await this.pwaService.installApp();
    if (installed) {
      console.log('App installed successfully');
    }
  }
  
  dismiss(): void {
    this.showPrompt = false;
  }
}
