import { Injectable } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay } from 'rxjs/operators';
import { Observable } from 'rxjs';

export type DeviceType = 'mobile' | 'tablet' | 'desktop';

export interface BreakpointState {
  isMobile: boolean;
  isTablet: boolean;
  isDesktop: boolean;
  deviceType: DeviceType;
}

@Injectable({
  providedIn: 'root'
})
export class BreakpointService {
  
  isMobile$: Observable<boolean>;
  isTablet$: Observable<boolean>;
  isDesktop$: Observable<boolean>;
  isHandset$: Observable<boolean>;
  isTabletOrSmaller$: Observable<boolean>;
  breakpointState$: Observable<BreakpointState>;
    
  constructor(private breakpointObserver: BreakpointObserver) {
    this.isMobile$ = this.breakpointObserver
      .observe(['(max-width: 599px)'])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
      
    this.isTablet$ = this.breakpointObserver
      .observe(['(min-width: 600px) and (max-width: 1279px)'])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
      
    this.isDesktop$ = this.breakpointObserver
      .observe(['(min-width: 1280px)'])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
    
    this.isHandset$ = this.breakpointObserver
      .observe([Breakpoints.Handset])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
    
    this.isTabletOrSmaller$ = this.breakpointObserver
      .observe([Breakpoints.Handset, Breakpoints.Tablet])
      .pipe(
        map(result => result.matches),
        shareReplay(1)
      );
    
    this.breakpointState$ = this.breakpointObserver
      .observe(['(max-width: 599px)', '(min-width: 600px) and (max-width: 1279px)', '(min-width: 1280px)'])
      .pipe(
        map(result => {
          const isMobile = window.matchMedia('(max-width: 599px)').matches;
          const isTablet = window.matchMedia('(min-width: 600px) and (max-width: 1279px)').matches;
          const isDesktop = window.matchMedia('(min-width: 1280px)').matches;
          
          let deviceType: DeviceType = 'desktop';
          if (isMobile) deviceType = 'mobile';
          else if (isTablet) deviceType = 'tablet';
          
          return {
            isMobile,
            isTablet,
            isDesktop,
            deviceType
          };
        }),
        shareReplay(1)
      );
  }
  
  getCurrentBreakpoint(): BreakpointState {
    const isMobile = window.matchMedia('(max-width: 599px)').matches;
    const isTablet = window.matchMedia('(min-width: 600px) and (max-width: 1279px)').matches;
    const isDesktop = window.matchMedia('(min-width: 1280px)').matches;
    
    let deviceType: DeviceType = 'desktop';
    if (isMobile) deviceType = 'mobile';
    else if (isTablet) deviceType = 'tablet';
    
    return {
      isMobile,
      isTablet,
      isDesktop,
      deviceType
    };
  }
  
  isMobile(): boolean {
    return window.matchMedia('(max-width: 599px)').matches;
  }
  
  isTablet(): boolean {
    return window.matchMedia('(min-width: 600px) and (max-width: 1279px)').matches;
  }
  
  isDesktop(): boolean {
    return window.matchMedia('(min-width: 1280px)').matches;
  }
}
