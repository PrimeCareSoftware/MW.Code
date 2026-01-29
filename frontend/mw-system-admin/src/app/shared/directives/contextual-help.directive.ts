import { Directive, ElementRef, Input, OnInit, OnDestroy, inject } from '@angular/core';
import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { HelpTooltipComponent } from '../components/help-tooltip/help-tooltip.component';

/**
 * Directive that adds contextual help icon next to an element
 * Shows tooltip with help content on click
 * 
 * Usage:
 * <label appContextualHelp="help-article-id" helpPosition="right">
 *   Field Label
 * </label>
 */
@Directive({
  selector: '[appContextualHelp]',
  standalone: true
})
export class ContextualHelpDirective implements OnInit, OnDestroy {
  @Input() appContextualHelp: string = ''; // ID do artigo de ajuda
  @Input() helpPosition: 'top' | 'bottom' | 'left' | 'right' = 'right';
  
  private elementRef = inject(ElementRef);
  private overlay = inject(Overlay);
  private overlayRef: OverlayRef | null = null;
  private helpIcon: HTMLElement | null = null;

  ngOnInit() {
    this.createHelpIcon();
  }

  ngOnDestroy() {
    if (this.overlayRef) {
      this.overlayRef.dispose();
    }
    if (this.helpIcon && this.helpIcon.parentNode) {
      this.helpIcon.parentNode.removeChild(this.helpIcon);
    }
  }

  private createHelpIcon() {
    this.helpIcon = document.createElement('span');
    this.helpIcon.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" height="18" viewBox="0 0 24 24" width="18" style="vertical-align: middle; margin-left: 4px; cursor: pointer; fill: currentColor; opacity: 0.6;">
        <path d="M0 0h24v24H0V0z" fill="none"/>
        <path d="M11 18h2v-2h-2v2zm1-16C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm0-14c-2.21 0-4 1.79-4 4h2c0-1.1.9-2 2-2s2 .9 2 2c0 2-3 1.75-3 5h2c0-2.25 3-2.5 3-5 0-2.21-1.79-4-4-4z"/>
      </svg>
    `;
    
    this.helpIcon.addEventListener('click', (event) => {
      event.preventDefault();
      event.stopPropagation();
      this.showHelp();
    });
    
    this.helpIcon.addEventListener('mouseenter', () => {
      if (this.helpIcon) {
        this.helpIcon.style.opacity = '1';
      }
    });
    
    this.helpIcon.addEventListener('mouseleave', () => {
      if (this.helpIcon) {
        this.helpIcon.style.opacity = '0.6';
      }
    });
    
    // Insert help icon after the element
    const parent = this.elementRef.nativeElement.parentNode;
    if (parent) {
      parent.insertBefore(this.helpIcon, this.elementRef.nativeElement.nextSibling);
    }
  }

  private showHelp() {
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = null;
      return;
    }

    const positionStrategy = this.overlay
      .position()
      .flexibleConnectedTo(this.elementRef)
      .withPositions([
        {
          originX: this.helpPosition === 'left' ? 'start' : this.helpPosition === 'right' ? 'end' : 'center',
          originY: this.helpPosition === 'top' ? 'top' : this.helpPosition === 'bottom' ? 'bottom' : 'center',
          overlayX: this.helpPosition === 'left' ? 'end' : this.helpPosition === 'right' ? 'start' : 'center',
          overlayY: this.helpPosition === 'top' ? 'bottom' : this.helpPosition === 'bottom' ? 'top' : 'center'
        }
      ]);

    this.overlayRef = this.overlay.create({
      positionStrategy,
      hasBackdrop: true,
      backdropClass: 'help-backdrop',
      scrollStrategy: this.overlay.scrollStrategies.reposition()
    });

    const portal = new ComponentPortal(HelpTooltipComponent);
    const componentRef = this.overlayRef.attach(portal);
    componentRef.instance.articleId = this.appContextualHelp;
    componentRef.instance.close.subscribe(() => {
      if (this.overlayRef) {
        this.overlayRef.dispose();
        this.overlayRef = null;
      }
    });

    this.overlayRef.backdropClick().subscribe(() => {
      if (this.overlayRef) {
        this.overlayRef.dispose();
        this.overlayRef = null;
      }
    });
  }
}
