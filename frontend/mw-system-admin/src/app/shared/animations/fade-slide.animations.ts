import { trigger, transition, style, animate, state } from '@angular/animations';

export const fadeSlideInAnimation = trigger('fadeSlideIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(10px)' }),
    animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ opacity: 0, transform: 'translateY(10px)' }))
  ])
]);

export const fadeInAnimation = trigger('fadeIn', [
  transition(':enter', [
    style({ opacity: 0 }),
    animate('300ms ease-out', style({ opacity: 1 }))
  ]),
  transition(':leave', [
    animate('200ms ease-in', style({ opacity: 0 }))
  ])
]);

export const scaleInAnimation = trigger('scaleIn', [
  transition(':enter', [
    style({ opacity: 0, transform: 'scale(0.95)' }),
    animate('200ms ease-out', style({ opacity: 1, transform: 'scale(1)' }))
  ]),
  transition(':leave', [
    animate('150ms ease-in', style({ opacity: 0, transform: 'scale(0.95)' }))
  ])
]);

export const slideInFromRightAnimation = trigger('slideInFromRight', [
  transition(':enter', [
    style({ transform: 'translateX(100%)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateX(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('250ms ease-in', style({ transform: 'translateX(100%)', opacity: 0 }))
  ])
]);

export const slideInFromLeftAnimation = trigger('slideInFromLeft', [
  transition(':enter', [
    style({ transform: 'translateX(-100%)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateX(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('250ms ease-in', style({ transform: 'translateX(-100%)', opacity: 0 }))
  ])
]);

export const slideInFromTopAnimation = trigger('slideInFromTop', [
  transition(':enter', [
    style({ transform: 'translateY(-100%)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateY(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('250ms ease-in', style({ transform: 'translateY(-100%)', opacity: 0 }))
  ])
]);

export const slideInFromBottomAnimation = trigger('slideInFromBottom', [
  transition(':enter', [
    style({ transform: 'translateY(100%)', opacity: 0 }),
    animate('300ms ease-out', style({ transform: 'translateY(0)', opacity: 1 }))
  ]),
  transition(':leave', [
    animate('250ms ease-in', style({ transform: 'translateY(100%)', opacity: 0 }))
  ])
]);

export const expandCollapseAnimation = trigger('expandCollapse', [
  state('collapsed', style({
    height: '0',
    overflow: 'hidden',
    opacity: 0
  })),
  state('expanded', style({
    height: '*',
    overflow: 'visible',
    opacity: 1
  })),
  transition('collapsed <=> expanded', [
    animate('300ms cubic-bezier(0.4, 0, 0.2, 1)')
  ])
]);

export const rotateAnimation = trigger('rotate', [
  state('default', style({ transform: 'rotate(0deg)' })),
  state('rotated', style({ transform: 'rotate(180deg)' })),
  transition('default <=> rotated', [
    animate('300ms ease-in-out')
  ])
]);

export const pulseAnimation = trigger('pulse', [
  transition('* => *', [
    animate('1s ease-in-out', style({ transform: 'scale(1.05)' })),
    animate('1s ease-in-out', style({ transform: 'scale(1)' }))
  ])
]);
