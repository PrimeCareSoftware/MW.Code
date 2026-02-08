import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TicketFab } from './shared/ticket-fab/ticket-fab';
import { ChatFabComponent } from './shared/chat-fab/chat-fab';
import { Auth } from './services/auth';
import { ThemeService } from './services/theme.service';
import { SkipToContentComponent } from './shared/accessibility/components/skip-to-content.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TicketFab, ChatFabComponent, SkipToContentComponent],
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
