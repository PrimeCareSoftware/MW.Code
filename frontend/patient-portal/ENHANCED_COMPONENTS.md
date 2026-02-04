# Enhanced UI Components Documentation

## Overview

This document provides comprehensive documentation for the newly created enhanced UI components in the Patient Portal. These components follow the Omni Care design system and provide improved user experience with smooth animations, better accessibility, and mobile-first design.

## Components

### 1. SkeletonLoaderComponent

**Location:** `src/app/shared/components/skeleton-loader/skeleton-loader.component.ts`

**Purpose:** Provides animated placeholder loading states to improve perceived performance.

**Usage:**
```html
<app-skeleton-loader variant="card" height="200px"></app-skeleton-loader>
<app-skeleton-loader variant="text" width="80%"></app-skeleton-loader>
<app-skeleton-loader variant="avatar" shape="circle"></app-skeleton-loader>
```

**Props:**
- `variant`: 'card' | 'text' | 'title' | 'avatar' | 'button' (default: 'text')
- `width`: string (optional, e.g., '100px', '50%')
- `height`: string (optional, e.g., '200px')
- `shape`: 'default' | 'circle' | 'rounded' (default: 'default')

**Features:**
- Animated shimmer effect
- Multiple variants for different content types
- Customizable dimensions
- Follows design system colors

---

### 2. AnimatedCounterComponent

**Location:** `src/app/shared/components/animated-counter/animated-counter.component.ts`

**Purpose:** Displays numbers with smooth count-up animation effect.

**Usage:**
```html
<app-animated-counter [value]="150" [duration]="1200"></app-animated-counter>
<app-animated-counter [value]="total" prefix="R$" suffix=",00"></app-animated-counter>
```

**Props:**
- `value`: number (required, the target value to count to)
- `duration`: number (default: 1000, animation duration in ms)
- `prefix`: string (optional, text before the number)
- `suffix`: string (optional, text after the number)

**Features:**
- Smooth easing animation (cubic easing)
- Customizable duration
- Prefix and suffix support
- Automatic cleanup on destroy

---

### 3. AnimatedCardComponent

**Location:** `src/app/shared/components/animated-card/animated-card.component.ts`

**Purpose:** Enhanced card component with smooth hover effects and animations.

**Usage:**
```html
<app-animated-card [hoverable]="true" [elevated]="true">
  <h3>Card Title</h3>
  <p>Card content goes here</p>
</app-animated-card>

<app-animated-card [clickable]="true" (cardClick)="handleClick($event)">
  <p>Clickable card</p>
</app-animated-card>
```

**Props:**
- `clickable`: boolean (default: false, makes card clickable)
- `hoverable`: boolean (default: true, enables hover effects)
- `elevated`: boolean (default: true, adds shadow elevation)

**Events:**
- `cardClick`: EventEmitter<Event> (emitted when card is clicked, only if clickable=true)

**Features:**
- Smooth hover transitions
- Shimmer effect on hover
- Elevation shadows
- Click feedback
- Content projection via ng-content

---

### 4. FabButtonComponent

**Location:** `src/app/shared/components/fab-button/fab-button.component.ts`

**Purpose:** Floating Action Button for primary actions, positioned fixed on screen.

**Usage:**
```html
<app-fab-button 
  icon="add" 
  tooltip="Add New Item" 
  color="primary"
  link="/create">
</app-fab-button>

<app-fab-button 
  icon="chat" 
  tooltip="Messages" 
  (fabClick)="openChat()">
</app-fab-button>
```

**Props:**
- `icon`: string (default: 'add', Material icon name)
- `tooltip`: string (default: '', tooltip text)
- `color`: 'primary' | 'accent' | 'warn' (default: 'primary')
- `link`: string (optional, router link)

**Events:**
- `fabClick`: EventEmitter<void> (emitted when FAB is clicked, if no link provided)

**Features:**
- Fixed positioning (bottom-right)
- Smooth scale animations
- Material Design compliant
- Router integration
- Mobile-optimized positioning

---

### 5. BottomNavComponent

**Location:** `src/app/shared/components/bottom-nav/bottom-nav.component.ts`

**Purpose:** Mobile-first bottom navigation bar for primary app navigation.

**Usage:**
```html
<app-bottom-nav></app-bottom-nav>
```

**Features:**
- Fixed bottom positioning
- Auto-hides on desktop (>768px)
- Active route highlighting
- Icon + label navigation
- Badge support for notifications
- Smooth transitions
- Touch-optimized spacing

**Navigation Items:**
- Home (Dashboard)
- Appointments
- Documents
- Profile

---

### 6. ProgressBarComponent

**Location:** `src/app/shared/components/progress-bar/progress-bar.component.ts`

**Purpose:** Animated progress bar with gradient and shimmer effects.

**Usage:**
```html
<app-progress-bar [value]="75" color="primary" [showLabel]="true"></app-progress-bar>
<app-progress-bar [value]="progress" color="success" size="lg"></app-progress-bar>
```

**Props:**
- `value`: number (default: 0, progress value 0-100)
- `color`: 'primary' | 'success' | 'warning' | 'error' (default: 'primary')
- `size`: 'sm' | 'md' | 'lg' (default: 'md')
- `showLabel`: boolean (default: false, shows percentage label)
- `animated`: boolean (default: true, enables animations)

**Features:**
- Gradient fill colors
- Shimmer animation
- Multiple sizes
- Optional percentage label
- Smooth transitions

---

### 7. BreadcrumbComponent

**Location:** `src/app/shared/components/breadcrumb/breadcrumb.component.ts`

**Purpose:** Hierarchical navigation breadcrumbs with accessibility support.

**Usage:**
```html
<app-breadcrumb [items]="breadcrumbItems"></app-breadcrumb>
```

**TypeScript:**
```typescript
breadcrumbItems: BreadcrumbItem[] = [
  { label: 'Home', route: '/dashboard', icon: 'home' },
  { label: 'Appointments', route: '/appointments' },
  { label: 'Details' }
];
```

**Props:**
- `items`: BreadcrumbItem[] (required, array of breadcrumb items)

**BreadcrumbItem Interface:**
```typescript
interface BreadcrumbItem {
  label: string;       // Display text
  route?: string;      // Optional router link
  icon?: string;       // Optional Material icon (shown on first item)
}
```

**Features:**
- ARIA accessibility labels
- Router integration
- Icon support for home
- Mobile-responsive (text truncation)
- Active item highlighting
- Keyboard navigation support

---

### 8. BadgeComponent

**Location:** `src/app/shared/components/badge/badge.component.ts`

**Purpose:** Status and label badges with multiple variants and styles.

**Usage:**
```html
<app-badge variant="success">Active</app-badge>
<app-badge variant="warning" icon="warning">Warning</app-badge>
<app-badge variant="error" [dot]="true">3 errors</app-badge>
<app-badge variant="outline" size="lg">Large Badge</app-badge>
```

**Props:**
- `variant`: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'neutral' | 'outline' (default: 'primary')
- `size`: 'sm' | 'md' | 'lg' (default: 'md')
- `icon`: string (optional, Material icon name)
- `dot`: boolean (default: false, shows dot indicator)

**Features:**
- Multiple color variants
- Icon support
- Dot indicator for notifications
- Outline variant
- Size variants
- Content projection

---

## Design Principles

All components follow these design principles:

1. **Mobile-First**: All components are optimized for mobile devices first
2. **Accessibility**: ARIA labels, keyboard navigation, focus states
3. **Performance**: Optimized animations using CSS transforms
4. **Consistency**: Following Omni Care design system tokens
5. **Modularity**: Standalone components that can be used anywhere
6. **Responsive**: Adaptive layouts for different screen sizes

## Animation Guidelines

Components use these animation timings from the design system:
- `--transition-fast`: 150ms (micro-interactions)
- `--transition-base`: 250ms (standard transitions)
- `--transition-slow`: 350ms (complex animations)

## Color System

Components leverage the Omni Care color tokens:
- Primary: Teal healthcare color
- Success: Green for positive actions
- Warning: Yellow/orange for cautions
- Error/Destructive: Red for errors
- Info: Blue for informational content
- Muted: Gray tones for secondary content

## Best Practices

1. **Use skeleton loaders** during data fetching for better perceived performance
2. **Use animated counters** for important metrics to draw attention
3. **Use FAB** for the most important action on a page
4. **Use bottom nav** on mobile for primary navigation
5. **Use badges** for status indicators and notification counts
6. **Use breadcrumbs** for deep navigation hierarchies
7. **Use progress bars** for step indicators and loading progress

## Examples

### Loading State Pattern
```html
<div *ngIf="!loading; else loadingTemplate">
  <!-- Actual content -->
</div>

<ng-template #loadingTemplate>
  <app-skeleton-loader variant="card"></app-skeleton-loader>
  <app-skeleton-loader variant="text"></app-skeleton-loader>
</ng-template>
```

### Statistics Card with Animation
```html
<app-animated-card>
  <h3>Total Users</h3>
  <app-animated-counter [value]="totalUsers" [duration]="1500"></app-animated-counter>
</app-animated-card>
```

### Status Badge
```html
<app-badge 
  [variant]="appointment.status === 'confirmed' ? 'success' : 'warning'"
  [icon]="appointment.isTelehealth ? 'videocam' : 'place'">
  {{ appointment.status }}
</app-badge>
```

## Browser Support

All components support:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile Safari (iOS 14+)
- Chrome Mobile (Android 10+)

## Accessibility Features

- Semantic HTML
- ARIA labels and roles
- Keyboard navigation
- Focus indicators
- Screen reader support
- Color contrast compliance (WCAG 2.1 AA)
