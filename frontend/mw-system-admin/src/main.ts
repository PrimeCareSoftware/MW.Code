import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { isDevMode } from '@angular/core';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));

// Register service worker for PWA support in production
if ('serviceWorker' in navigator && !isDevMode()) {
  navigator.serviceWorker.register('/ngsw-worker.js').then(
    (registration) => {
      console.log('Service Worker registered successfully:', registration.scope);
    },
    (error) => {
      console.error('Service Worker registration failed:', error);
    }
  );
}
