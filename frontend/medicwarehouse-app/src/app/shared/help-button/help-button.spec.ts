import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HelpButtonComponent } from './help-button';
import { HelpService } from '../../services/help.service';

describe('HelpButtonComponent', () => {
  let component: HelpButtonComponent;
  let fixture: ComponentFixture<HelpButtonComponent>;
  let helpService: HelpService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HelpButtonComponent],
      providers: [HelpService]
    }).compileComponents();

    fixture = TestBed.createComponent(HelpButtonComponent);
    component = fixture.componentInstance;
    helpService = TestBed.inject(HelpService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default position as bottom-right', () => {
    expect(component.position).toBe('bottom-right');
  });

  it('should accept position input', () => {
    component.position = 'top-right';
    expect(component.position).toBe('top-right');
  });

  it('should accept module input', () => {
    component.module = 'patients';
    expect(component.module).toBe('patients');
  });

  it('should call helpService.getHelpContent when openHelp is called', () => {
    spyOn(helpService, 'getHelpContent').and.callThrough();
    component.module = 'patients';
    
    // Manually initialize the ViewChild since we're testing
    // In actual usage, Angular will handle this
    if (component.helpDialog) {
      spyOn(component.helpDialog, 'open');
    }
    
    component.openHelp();
    
    expect(helpService.getHelpContent).toHaveBeenCalledWith('patients');
  });

  it('should render help button', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('.help-fab');
    expect(button).toBeTruthy();
  });

  it('should have correct CSS class for bottom-right position', () => {
    component.position = 'bottom-right';
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('.help-fab');
    expect(button?.classList.contains('position-bottom-right')).toBe(true);
  });

  it('should have correct CSS class for top-right position', () => {
    component.position = 'top-right';
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('.help-fab');
    expect(button?.classList.contains('position-top-right')).toBe(true);
  });
});
