import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark' | 'high-contrast';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'app-theme';
  private readonly theme = signal<Theme>(this.getInitialTheme());

  constructor() {
    // Apply theme whenever it changes
    effect(() => {
      this.applyTheme(this.theme());
    });
  }

  private getInitialTheme(): Theme {
    // Check localStorage first
    const stored = localStorage.getItem(this.THEME_KEY) as Theme;
    if (stored && ['light', 'dark', 'high-contrast'].includes(stored)) {
      return stored;
    }

    // Check system preference
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }

    return 'light';
  }

  private applyTheme(theme: Theme): void {
    // Remove all theme classes
    document.body.classList.remove('theme-light', 'theme-dark', 'theme-high-contrast');
    
    // Add current theme class
    document.body.classList.add(`theme-${theme}`);
    
    // Store preference
    localStorage.setItem(this.THEME_KEY, theme);
    
    // Update meta theme-color for mobile browsers
    const metaThemeColor = document.querySelector('meta[name="theme-color"]');
    if (metaThemeColor) {
      const colors = {
        light: '#ffffff',
        dark: '#0f172a',
        'high-contrast': '#000000'
      };
      metaThemeColor.setAttribute('content', colors[theme]);
    }
  }

  getTheme(): Theme {
    return this.theme();
  }

  setTheme(theme: Theme): void {
    this.theme.set(theme);
  }

  toggleTheme(): void {
    const themes: Theme[] = ['light', 'dark', 'high-contrast'];
    const currentIndex = themes.indexOf(this.theme());
    const nextIndex = (currentIndex + 1) % themes.length;
    this.theme.set(themes[nextIndex]);
  }

  isDark(): boolean {
    return this.theme() === 'dark';
  }

  isHighContrast(): boolean {
    return this.theme() === 'high-contrast';
  }
}
