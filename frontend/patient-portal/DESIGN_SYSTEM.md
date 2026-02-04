# Patient Portal - Omni Care Design System

## Overview

This document describes the design system implementation for the Patient Portal, based on the Omni Care healthcare design system. The refactoring replaced hardcoded values with a consistent, maintainable design token system.

## Design Tokens

Design tokens are centralized in `src/styles/_design-tokens.scss` and use CSS custom properties (CSS variables) for easy theming and maintenance.

### Color System

The color system uses HSL format for better color manipulation and theming support.

#### Primary Colors (Healthcare Teal)
- **Primary**: `hsl(174, 72%, 40%)` - Main brand color
- **Primary Foreground**: `hsl(0, 0%, 100%)` - Text on primary backgrounds
- **Accent**: `hsl(174, 85%, 45%)` - Vibrant teal for emphasis

#### Semantic Colors
- **Success**: `hsl(158, 64%, 45%)` - Success states, confirmations
- **Destructive**: `hsl(0, 84%, 60%)` - Errors, dangerous actions
- **Warning**: `hsl(45, 93%, 47%)` - Warnings, cautions
- **Info**: `hsl(200, 98%, 39%)` - Informational messages

#### Neutral Colors
- **Background**: `hsl(180, 20%, 99%)` - Main background
- **Foreground**: `hsl(200, 50%, 10%)` - Main text color
- **Muted**: `hsl(180, 15%, 95%)` - Subtle backgrounds
- **Muted Foreground**: `hsl(200, 15%, 45%)` - Secondary text

### Spacing Scale

Based on a 4px grid system for consistent spacing:

```scss
--spacing-1: 0.25rem;  /* 4px */
--spacing-2: 0.5rem;   /* 8px */
--spacing-3: 0.75rem;  /* 12px */
--spacing-4: 1rem;     /* 16px */
--spacing-5: 1.25rem;  /* 20px */
--spacing-6: 1.5rem;   /* 24px */
--spacing-7: 1.75rem;  /* 28px */
--spacing-8: 2rem;     /* 32px */
--spacing-10: 2.5rem;  /* 40px */
--spacing-12: 3rem;    /* 48px */
--spacing-16: 4rem;    /* 64px */
--spacing-20: 5rem;    /* 80px */
```

### Typography

#### Font Families
- **Sans**: `'Inter'` - Body text
- **Display**: `'Plus Jakarta Sans'` - Headings and display text

#### Font Sizes
```scss
--text-xs: 0.75rem;    /* 12px */
--text-sm: 0.875rem;   /* 14px */
--text-base: 1rem;     /* 16px */
--text-lg: 1.125rem;   /* 18px */
--text-xl: 1.25rem;    /* 20px */
--text-2xl: 1.5rem;    /* 24px */
--text-3xl: 1.875rem;  /* 30px */
--text-4xl: 2.25rem;   /* 36px */
--text-5xl: 3rem;      /* 48px */
```

#### Font Weights
```scss
--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
--font-extrabold: 800;
```

### Border Radius

Consistent rounded corners throughout:

```scss
--radius: 0.75rem;                    /* 12px - base */
--radius-sm: calc(var(--radius) - 4px);  /* 8px */
--radius-md: var(--radius);              /* 12px */
--radius-lg: calc(var(--radius) + 4px);  /* 16px */
```

### Shadows

Elevation system for depth:

```scss
--shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
--shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1);
--shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
--shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
--shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
--shadow-2xl: 0 25px 50px -12px rgb(0 0 0 / 0.25);
```

### Transitions

Consistent animation timing:

```scss
--transition-fast: 150ms cubic-bezier(0.4, 0, 0.2, 1);
--transition-base: 250ms cubic-bezier(0.4, 0, 0.2, 1);
--transition-slow: 350ms cubic-bezier(0.4, 0, 0.2, 1);
```

## Theme Support

### Light Theme (Default)
The default theme uses the healthcare teal palette with light backgrounds.

### Dark Theme
Activated with `.dark` class on `<body>`:
- Inverted color scheme
- Adjusted shadows and borders
- Maintained color contrast ratios

### High Contrast Theme
Activated with `.theme-high-contrast` class for accessibility:
- Maximum contrast colors
- Yellow accents for focus states
- Bold borders and outlines

## Utility Classes

### Card Styles

```scss
.card-elevated {
  background: hsl(var(--card));
  border-radius: var(--radius-lg);
  border: 1px solid hsl(var(--border) / 0.5);
  box-shadow: var(--shadow-md);
}
```

### Gradients

```scss
.hero-gradient {
  background: linear-gradient(135deg, 
    hsl(var(--hero-gradient-start)) 0%, 
    hsl(var(--hero-gradient-end)) 100%);
}
```

### Glow Effects

```scss
.glow-effect {
  box-shadow: 0 0 40px -10px hsl(var(--glow) / 0.4);
}
```

## Component Patterns

### Cards

Cards use the elevated style with consistent padding and shadows:

```scss
.card {
  background: hsl(var(--card));
  border-radius: var(--radius-lg);
  border: 1px solid hsl(var(--border));
  box-shadow: var(--shadow-md);
  padding: var(--spacing-6);
  
  &:hover {
    box-shadow: var(--shadow-xl);
    transform: translateY(-2px);
  }
}
```

### Buttons

Buttons use the hero gradient for primary actions:

```scss
.button-primary {
  background: linear-gradient(135deg, 
    hsl(var(--hero-gradient-start)) 0%, 
    hsl(var(--hero-gradient-end)) 100%);
  color: hsl(var(--primary-foreground));
  border-radius: var(--radius-lg);
  padding: var(--spacing-3) var(--spacing-5);
  font-weight: var(--font-semibold);
  transition: all var(--transition-base);
  
  &:hover {
    box-shadow: 0 6px 20px hsl(var(--primary) / 0.6);
    transform: translateY(-2px);
  }
}
```

### Form Fields

Form fields use muted backgrounds with smooth focus transitions:

```scss
.input-field {
  background-color: hsl(var(--muted));
  border-radius: var(--radius-lg);
  border: 1px solid hsl(var(--border));
  padding: var(--spacing-3) var(--spacing-4);
  
  &:focus {
    background-color: hsl(var(--card));
    box-shadow: 0 0 0 3px hsl(var(--primary) / 0.1);
    border-color: hsl(var(--primary));
  }
}
```

## Usage Examples

### Using Colors

```scss
// Background colors
background: hsl(var(--background));
background: hsl(var(--card));
background: hsl(var(--muted));

// Text colors
color: hsl(var(--foreground));
color: hsl(var(--muted-foreground));
color: hsl(var(--primary));

// Borders
border-color: hsl(var(--border));
```

### Using Spacing

```scss
// Padding
padding: var(--spacing-4);
padding: var(--spacing-6) var(--spacing-4);

// Margins
margin-bottom: var(--spacing-6);
gap: var(--spacing-3);
```

### Using Typography

```scss
// Font sizes
font-size: var(--text-base);
font-size: var(--text-2xl);

// Font weights
font-weight: var(--font-medium);
font-weight: var(--font-bold);

// Font families
font-family: var(--font-sans);
font-family: var(--font-display);
```

## Accessibility

### Focus States

All interactive elements have clear focus indicators:

```scss
button:focus-visible,
a:focus-visible {
  outline: 3px solid hsl(var(--ring));
  outline-offset: 2px;
  border-radius: var(--radius-sm);
}
```

### Color Contrast

All color combinations meet WCAG 2.1 AA standards:
- Normal text: 4.5:1 minimum
- Large text: 3:1 minimum
- UI components: 3:1 minimum

### High Contrast Mode

High contrast theme provides maximum visibility for users who need it.

## Migration Guide

### Replacing Old Styles

**Before:**
```scss
color: #667eea;
padding: 24px;
border-radius: 12px;
font-size: 16px;
font-weight: 600;
```

**After:**
```scss
color: hsl(var(--primary));
padding: var(--spacing-6);
border-radius: var(--radius-lg);
font-size: var(--text-base);
font-weight: var(--font-semibold);
```

### Gradients

**Before:**
```scss
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
```

**After:**
```scss
background: linear-gradient(135deg, 
  hsl(var(--hero-gradient-start)) 0%, 
  hsl(var(--hero-gradient-end)) 100%);
```

## Best Practices

1. **Always use design tokens** instead of hardcoded values
2. **Use semantic colors** (success, warning, destructive) for their intended purpose
3. **Maintain consistent spacing** using the spacing scale
4. **Apply transitions** to interactive elements for smooth UX
5. **Test in dark mode** and high contrast mode
6. **Ensure proper focus states** for keyboard navigation
7. **Use the card-elevated class** for elevated surfaces
8. **Apply shadows** appropriately to establish hierarchy

## Browser Support

- Chrome/Edge: Latest 2 versions
- Firefox: Latest 2 versions  
- Safari: Latest 2 versions
- Mobile browsers: iOS Safari 14+, Chrome Android

## Related Files

- `src/styles/_design-tokens.scss` - Design token definitions
- `src/styles.scss` - Global styles and imports
- `src/app/pages/dashboard/dashboard.component.scss` - Dashboard component
- `src/app/pages/auth/login.component.scss` - Login component
- `src/app/pages/appointments/appointments.component.scss` - Appointments component

## Future Improvements

- [ ] Create reusable SCSS mixins for common patterns
- [ ] Add more utility classes
- [ ] Implement CSS-in-JS solution for dynamic theming
- [ ] Create Storybook documentation
- [ ] Add animation utilities
- [ ] Create form component library
