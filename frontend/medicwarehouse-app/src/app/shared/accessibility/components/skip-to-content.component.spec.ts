/**
 * Testes para SkipToContentComponent
 * Valida navegação por teclado e acessibilidade WCAG 2.1
 */

import { TestBed } from '@angular/core/testing';
import { SkipToContentComponent } from './skip-to-content.component';

describe('SkipToContentComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkipToContentComponent]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should render skip link with correct aria-label', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const skipLink = compiled.querySelector('a.skip-to-content');
    
    expect(skipLink).toBeTruthy();
    expect(skipLink?.getAttribute('aria-label')).toBe('Pular para o conteúdo principal');
    expect(skipLink?.getAttribute('href')).toBe('#main-content');
  });

  it('should have correct styles for focus visibility', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const skipLink = compiled.querySelector('a.skip-to-content') as HTMLElement;
    
    expect(skipLink).toBeTruthy();
    
    // Verify link is positioned offscreen by default
    const styles = window.getComputedStyle(skipLink);
    expect(styles.position).toBe('absolute');
  });

  it('should focus main content when clicked', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    // Create mock main content element
    const mainContent = document.createElement('main');
    mainContent.id = 'main-content';
    document.body.appendChild(mainContent);

    const mockEvent = new Event('click') as any;
    mockEvent.preventDefault = jasmine.createSpy('preventDefault');

    component.skipToContent(mockEvent);

    expect(mockEvent.preventDefault).toHaveBeenCalled();
    expect(document.activeElement).toBe(mainContent);

    // Cleanup
    document.body.removeChild(mainContent);
  });

  it('should add tabindex if not present on main content', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    const component = fixture.componentInstance;

    const mainContent = document.createElement('main');
    mainContent.id = 'main-content';
    document.body.appendChild(mainContent);

    const mockEvent = new Event('click') as any;
    mockEvent.preventDefault = jasmine.createSpy('preventDefault');

    component.skipToContent(mockEvent);

    expect(mainContent.getAttribute('tabindex')).toBe('-1');

    // Cleanup
    document.body.removeChild(mainContent);
  });

  it('should not throw error if main content does not exist', () => {
    const fixture = TestBed.createComponent(SkipToContentComponent);
    const component = fixture.componentInstance;

    const mockEvent = new Event('click') as any;
    mockEvent.preventDefault = jasmine.createSpy('preventDefault');

    expect(() => component.skipToContent(mockEvent)).not.toThrow();
  });
});
