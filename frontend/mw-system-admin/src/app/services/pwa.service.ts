import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// Type definition for BeforeInstallPromptEvent
interface BeforeInstallPromptEvent extends Event {
  readonly platforms: string[];
  readonly userChoice: Promise<{
    outcome: 'accepted' | 'dismissed';
    platform: string;
  }>;
  prompt(): Promise<void>;
}

@Injectable({
  providedIn: 'root'
})
export class PwaService {
  private promptEvent: BeforeInstallPromptEvent | null = null;
  private isInstallable$ = new BehaviorSubject<boolean>(false);
  
  constructor() {
    this.setupInstallPrompt();
  }
  
  private setupInstallPrompt(): void {
    // Listen for the beforeinstallprompt event
    window.addEventListener('beforeinstallprompt', (event: Event) => {
      console.log('PWA: beforeinstallprompt event fired');
      // Prevent the default install prompt
      event.preventDefault();
      // Store the event for later use
      this.promptEvent = event as BeforeInstallPromptEvent;
      // Update installable state
      this.isInstallable$.next(true);
    });
    
    // Listen for the appinstalled event
    window.addEventListener('appinstalled', () => {
      console.log('PWA: App installed successfully');
      this.promptEvent = null;
      this.isInstallable$.next(false);
    });
  }
  
  /**
   * Check if the app can be installed
   */
  get canInstall$() {
    return this.isInstallable$.asObservable();
  }
  
  /**
   * Check if the app can be installed (synchronous)
   */
  get canInstall(): boolean {
    return this.isInstallable$.value;
  }
  
  /**
   * Trigger the installation prompt
   */
  async installApp(): Promise<boolean> {
    if (!this.promptEvent) {
      console.warn('PWA: Install prompt not available');
      return false;
    }
    
    try {
      // Show the install prompt
      this.promptEvent.prompt();
      
      // Wait for the user to respond to the prompt
      const { outcome } = await this.promptEvent.userChoice;
      
      console.log(`PWA: User response to install prompt: ${outcome}`);
      
      // Reset the prompt event
      this.promptEvent = null;
      this.isInstallable$.next(false);
      
      return outcome === 'accepted';
    } catch (error) {
      console.error('PWA: Error showing install prompt:', error);
      return false;
    }
  }
  
  /**
   * Check if the app is running in standalone mode (installed)
   */
  isInstalled(): boolean {
    // Check if running in standalone mode (iOS)
    if ((window.navigator as any).standalone) {
      return true;
    }
    
    // Check if running in standalone mode (Android/Desktop)
    if (window.matchMedia('(display-mode: standalone)').matches) {
      return true;
    }
    
    return false;
  }
}
