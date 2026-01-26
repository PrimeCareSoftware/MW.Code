import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent, merge, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class OfflineService {
  private onlineStatus$ = new BehaviorSubject<boolean>(navigator.onLine);
  
  constructor() {
    this.initializeNetworkStatus();
  }
  
  private initializeNetworkStatus(): void {
    // Listen to online/offline events
    merge(
      fromEvent(window, 'online').pipe(map(() => true)),
      fromEvent(window, 'offline').pipe(map(() => false))
    ).subscribe(status => {
      console.log(`Network status changed: ${status ? 'online' : 'offline'}`);
      this.onlineStatus$.next(status);
    });
  }
  
  /**
   * Observable that emits current online status
   */
  get isOnline$(): Observable<boolean> {
    return this.onlineStatus$.asObservable();
  }
  
  /**
   * Get current online status
   */
  get isOnline(): boolean {
    return this.onlineStatus$.value;
  }
  
  /**
   * Check if the application is running in offline mode
   */
  get isOffline(): boolean {
    return !this.isOnline;
  }
}
