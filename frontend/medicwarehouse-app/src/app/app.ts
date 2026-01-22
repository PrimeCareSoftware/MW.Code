import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TicketFab } from './shared/ticket-fab/ticket-fab';
import { Auth } from './services/auth';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TicketFab],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('primecare-frontend');
  
  constructor(
    public authService: Auth,
    private themeService: ThemeService
  ) {}
  
  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }
}
