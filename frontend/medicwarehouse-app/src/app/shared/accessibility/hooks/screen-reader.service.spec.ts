/**
 * Testes para ScreenReaderService
 * Valida anúncios para leitores de tela conforme WCAG 2.1
 */

import { TestBed } from '@angular/core/testing';
import { ScreenReaderService } from './screen-reader.service';

describe('ScreenReaderService', () => {
  let service: ScreenReaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScreenReaderService]
    });
    service = TestBed.inject(ScreenReaderService);
  });

  afterEach(() => {
    // Clean up any live regions created during tests
    const liveRegions = document.querySelectorAll('[aria-live]');
    liveRegions.forEach(region => region.remove());
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create live region on first announce', () => {
    service.announce('Test message');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    expect(liveRegion).toBeTruthy();
  });

  it('should announce message with polite priority', () => {
    service.announce('Test message', 'polite');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    expect(liveRegion?.textContent).toContain('Test message');
  });

  it('should announce message with assertive priority', () => {
    service.announce('Urgent message', 'assertive');
    
    const liveRegion = document.querySelector('[aria-live="assertive"]');
    expect(liveRegion?.textContent).toContain('Urgent message');
  });

  it('should announceSuccess with correct styling', () => {
    service.announceSuccess('Operation successful');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    expect(liveRegion?.textContent).toContain('Operation successful');
  });

  it('should announceError with assertive priority', () => {
    service.announceError('Error occurred');
    
    const liveRegion = document.querySelector('[aria-live="assertive"]');
    expect(liveRegion?.textContent).toContain('Error occurred');
  });

  it('should announceWarning with polite priority', () => {
    service.announceWarning('Warning message');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    expect(liveRegion?.textContent).toContain('Warning message');
  });

  it('should announceInfo with polite priority', () => {
    service.announceInfo('Info message');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    expect(liveRegion?.textContent).toContain('Info message');
  });

  it('should have aria-atomic="true" on live region', () => {
    service.announce('Test');
    
    const liveRegion = document.querySelector('[aria-live]');
    expect(liveRegion?.getAttribute('aria-atomic')).toBe('true');
  });

  it('should be off-screen but accessible to screen readers', () => {
    service.announce('Test');
    
    const liveRegion = document.querySelector('[aria-live]') as HTMLElement;
    const styles = window.getComputedStyle(liveRegion);
    
    expect(styles.position).toBe('absolute');
    expect(liveRegion.style.width).toBe('1px');
    expect(liveRegion.style.height).toBe('1px');
  });

  it('should clear message after delay', (done) => {
    service.announce('Temporary message');
    
    const liveRegion = document.querySelector('[aria-live]');
    expect(liveRegion?.textContent).toContain('Temporary message');

    // Wait for clear timeout (default 3000ms)
    setTimeout(() => {
      expect(liveRegion?.textContent).toBe('');
      done();
    }, 3500);
  });

  it('should handle multiple consecutive announcements', () => {
    service.announce('First message');
    service.announce('Second message');
    service.announce('Third message');
    
    const liveRegion = document.querySelector('[aria-live="polite"]');
    // Last message should be visible
    expect(liveRegion?.textContent).toContain('Third message');
  });

  it('should create separate regions for polite and assertive', () => {
    service.announce('Polite message', 'polite');
    service.announce('Assertive message', 'assertive');
    
    const politeRegion = document.querySelector('[aria-live="polite"]');
    const assertiveRegion = document.querySelector('[aria-live="assertive"]');
    
    expect(politeRegion).toBeTruthy();
    expect(assertiveRegion).toBeTruthy();
    expect(politeRegion).not.toBe(assertiveRegion);
  });

  it('should have role="status" on live region', () => {
    service.announce('Test');
    
    const liveRegion = document.querySelector('[aria-live]');
    expect(liveRegion?.getAttribute('role')).toBe('status');
  });
});
