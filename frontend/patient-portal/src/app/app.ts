import { Component, OnInit, signal } from '@angular/core';
import { ThemeService } from './services/theme.service';
import { SwUpdate, VersionReadyEvent } from '@angular/service-worker';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('patient-portal');
  
  constructor(
    private themeService: ThemeService,
    private swUpdate: SwUpdate
  ) {}
  
  ngOnInit() {
    // Check for service worker updates
    if (this.swUpdate.isEnabled) {
      // Check for updates every 6 hours
      setInterval(() => {
        this.swUpdate.checkForUpdate();
      }, 6 * 60 * 60 * 1000);
      
      // Listen for version updates
      this.swUpdate.versionUpdates
        .pipe(
          filter((evt): evt is VersionReadyEvent => evt.type === 'VERSION_READY')
        )
        .subscribe(() => {
          if (confirm('Nova versão disponível! Deseja atualizar agora?')) {
            window.location.reload();
          }
        });
      
      // Check for unrecoverable state
      this.swUpdate.unrecoverable.subscribe(event => {
        console.error('Service Worker em estado irrecuperável:', event.reason);
        if (confirm(
          'Ocorreu um erro crítico. A aplicação precisa ser recarregada. Deseja recarregar agora?'
        )) {
          window.location.reload();
        }
      });
    }
  }
}
