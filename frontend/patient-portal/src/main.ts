import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';
import { isDevMode } from '@angular/core';

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));

// Register service worker for PWA support in production
if ('serviceWorker' in navigator && !isDevMode()) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/ngsw-worker.js').then(
      (registration) => {
        console.log('Service Worker registered successfully:', registration.scope);
      },
      (error) => {
        console.error('Service Worker registration failed:', error);
      }
    );
  });
}
