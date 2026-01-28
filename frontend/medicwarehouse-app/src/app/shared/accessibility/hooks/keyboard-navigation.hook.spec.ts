/**
 * Testes para KeyboardNavigationService
 * Valida navegação por teclado conforme WCAG 2.1
 */

import { TestBed } from '@angular/core/testing';
import { KeyboardNavigationService } from './keyboard-navigation.hook';

describe('KeyboardNavigationService', () => {
  let service: KeyboardNavigationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [KeyboardNavigationService]
    });
    service = TestBed.inject(KeyboardNavigationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register Enter key handler', () => {
    const element = document.createElement('button');
    const onEnter = jasmine.createSpy('onEnter');
    
    service.registerHandlers(element, { onEnter });
    
    const event = new KeyboardEvent('keydown', { key: 'Enter' });
    element.dispatchEvent(event);
    
    expect(onEnter).toHaveBeenCalled();
  });

  it('should register Escape key handler', () => {
    const element = document.createElement('div');
    const onEscape = jasmine.createSpy('onEscape');
    
    service.registerHandlers(element, { onEscape });
    
    const event = new KeyboardEvent('keydown', { key: 'Escape' });
    element.dispatchEvent(event);
    
    expect(onEscape).toHaveBeenCalled();
  });

  it('should register Arrow key handlers', () => {
    const element = document.createElement('div');
    const onArrowUp = jasmine.createSpy('onArrowUp');
    const onArrowDown = jasmine.createSpy('onArrowDown');
    const onArrowLeft = jasmine.createSpy('onArrowLeft');
    const onArrowRight = jasmine.createSpy('onArrowRight');
    
    service.registerHandlers(element, {
      onArrowUp,
      onArrowDown,
      onArrowLeft,
      onArrowRight
    });
    
    element.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowUp' }));
    element.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown' }));
    element.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowLeft' }));
    element.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight' }));
    
    expect(onArrowUp).toHaveBeenCalled();
    expect(onArrowDown).toHaveBeenCalled();
    expect(onArrowLeft).toHaveBeenCalled();
    expect(onArrowRight).toHaveBeenCalled();
  });

  it('should register Tab key handler', () => {
    const element = document.createElement('input');
    const onTab = jasmine.createSpy('onTab');
    
    service.registerHandlers(element, { onTab });
    
    const event = new KeyboardEvent('keydown', { key: 'Tab' });
    element.dispatchEvent(event);
    
    expect(onTab).toHaveBeenCalledWith(event);
  });

  it('should register Space key handler', () => {
    const element = document.createElement('div');
    const onSpace = jasmine.createSpy('onSpace');
    
    service.registerHandlers(element, { onSpace });
    
    const event = new KeyboardEvent('keydown', { key: ' ' });
    element.dispatchEvent(event);
    
    expect(onSpace).toHaveBeenCalled();
  });

  it('should unregister handlers', () => {
    const element = document.createElement('button');
    const onEnter = jasmine.createSpy('onEnter');
    
    const unregister = service.registerHandlers(element, { onEnter });
    unregister();
    
    const event = new KeyboardEvent('keydown', { key: 'Enter' });
    element.dispatchEvent(event);
    
    expect(onEnter).not.toHaveBeenCalled();
  });

  it('should focus element', () => {
    const element = document.createElement('button');
    document.body.appendChild(element);
    
    service.focusElement(element);
    
    expect(document.activeElement).toBe(element);
    
    document.body.removeChild(element);
  });

  it('should get focusable elements in container', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button>Button 1</button>
      <input type="text">
      <a href="#">Link</a>
      <button disabled>Disabled</button>
      <div tabindex="-1">Not focusable</div>
      <div tabindex="0">Focusable</div>
    `;
    document.body.appendChild(container);
    
    const focusable = service.getFocusableElements(container);
    
    // Should return: 2 buttons (not disabled), 1 input, 1 link, 1 div with tabindex="0"
    expect(focusable.length).toBe(5);
    
    document.body.removeChild(container);
  });

  it('should focus first focusable element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="second">Second</button>
    `;
    document.body.appendChild(container);
    
    service.focusFirst(container);
    
    expect(document.activeElement?.id).toBe('first');
    
    document.body.removeChild(container);
  });

  it('should focus last focusable element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="last">Last</button>
    `;
    document.body.appendChild(container);
    
    service.focusLast(container);
    
    expect(document.activeElement?.id).toBe('last');
    
    document.body.removeChild(container);
  });

  it('should focus next element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="second">Second</button>
      <button id="third">Third</button>
    `;
    document.body.appendChild(container);
    
    const firstButton = container.querySelector('#first') as HTMLElement;
    firstButton.focus();
    
    service.focusNext(container);
    
    expect(document.activeElement?.id).toBe('second');
    
    document.body.removeChild(container);
  });

  it('should focus previous element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="second">Second</button>
      <button id="third">Third</button>
    `;
    document.body.appendChild(container);
    
    const secondButton = container.querySelector('#second') as HTMLElement;
    secondButton.focus();
    
    service.focusPrevious(container);
    
    expect(document.activeElement?.id).toBe('first');
    
    document.body.removeChild(container);
  });

  it('should wrap around when focusing next from last element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="last">Last</button>
    `;
    document.body.appendChild(container);
    
    const lastButton = container.querySelector('#last') as HTMLElement;
    lastButton.focus();
    
    service.focusNext(container);
    
    expect(document.activeElement?.id).toBe('first');
    
    document.body.removeChild(container);
  });

  it('should wrap around when focusing previous from first element', () => {
    const container = document.createElement('div');
    container.innerHTML = `
      <button id="first">First</button>
      <button id="last">Last</button>
    `;
    document.body.appendChild(container);
    
    const firstButton = container.querySelector('#first') as HTMLElement;
    firstButton.focus();
    
    service.focusPrevious(container);
    
    expect(document.activeElement?.id).toBe('last');
    
    document.body.removeChild(container);
  });
});
