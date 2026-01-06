# Apple-Inspired Design System

## Overview
This document describes the Apple-inspired design system implemented across all MedicWarehouse frontend applications (mw-site, mw-docs, medicwarehouse-app, and mw-system-admin).

## Typography

### Font Family
All applications use Apple's system font stack for optimal rendering on Apple devices and excellent fallbacks on other platforms:

```css
font-family: -apple-system, BlinkMacSystemFont, 'SF Pro Display', 'SF Pro Text', 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
```

**Key benefits:**
- Native Apple device rendering with SF Pro fonts
- No external font loading (better performance)
- Consistent with Apple's design language
- Excellent fallbacks for non-Apple platforms

### Font Sizes & Weights

#### Headings
- **H1**: 4rem (64px) on desktop, 3.25rem on tablet, 2.75rem on mobile
  - Font-weight: 700
  - Letter-spacing: -0.028em
  - Line-height: 1.05

- **H2**: 2.75rem (44px) on desktop, 2.25rem on tablet, 1.875rem on mobile
  - Font-weight: 700
  - Letter-spacing: -0.024em
  - Line-height: 1.1

- **H3**: 1.625rem (26px) on desktop, 1.375rem on mobile
  - Font-weight: 600
  - Letter-spacing: -0.020em
  - Line-height: 1.2

#### Body Text
- **Paragraph**: 1.0625rem (17px)
  - Font-weight: 400
  - Letter-spacing: -0.003em
  - Line-height: 1.6
  - Color: var(--gray-600)

#### Buttons
- **Standard**: 1.0625rem (17px)
  - Font-weight: 400
  - Letter-spacing: -0.008em
  - Padding: 0.75rem 1.75rem

- **Large**: 1.125rem (18px)
  - Font-weight: 400
  - Letter-spacing: -0.008em
  - Padding: 0.9375rem 2rem

## Color Palette

### Primary Colors (Blue - Apple-inspired)
```css
--primary-50: #f0f9ff;
--primary-100: #e0f2fe;
--primary-200: #bae6fd;
--primary-300: #7dd3fc;
--primary-400: #38bdf8;
--primary-500: #0ea5e9;  /* Main brand color */
--primary-600: #0284c7;
--primary-700: #0369a1;
--primary-800: #075985;
--primary-900: #0c4a6e;
```

### Neutral Colors (Apple-like Grays)
```css
--gray-50: #fafafa;   /* Backgrounds */
--gray-100: #f5f5f5;  /* Subtle backgrounds */
--gray-200: #e5e5e5;  /* Borders */
--gray-300: #d4d4d4;  /* Hover borders */
--gray-400: #a3a3a3;  /* Disabled text */
--gray-500: #737373;  /* Muted text */
--gray-600: #525252;  /* Body text */
--gray-700: #404040;  /* Headings */
--gray-800: #262626;  /* Dark text */
--gray-900: #171717;  /* Darkest text */
```

### Semantic Colors
```css
--success-500: #10b981;  /* Success green */
--warning-500: #f59e0b;  /* Warning amber */
--error-500: #ef4444;    /* Error red */
```

### Accent Colors
```css
--accent-500: #a855f7;  /* Purple for premium features */
--accent-600: #9333ea;
```

## Spacing System

Based on a 4px base unit (0.25rem):
```css
--spacing-1: 0.25rem;   /* 4px */
--spacing-2: 0.5rem;    /* 8px */
--spacing-3: 0.75rem;   /* 12px */
--spacing-4: 1rem;      /* 16px */
--spacing-5: 1.25rem;   /* 20px */
--spacing-6: 1.5rem;    /* 24px */
--spacing-8: 2rem;      /* 32px */
--spacing-10: 2.5rem;   /* 40px */
--spacing-12: 3rem;     /* 48px */
--spacing-16: 4rem;     /* 64px */
```

## Border Radius

Apple uses smooth, generous curves:
```css
--radius-sm: 0.5rem;    /* 8px */
--radius: 0.75rem;      /* 12px */
--radius-md: 1rem;      /* 16px */
--radius-lg: 1.25rem;   /* 20px */
--radius-xl: 1.75rem;   /* 28px */
--radius-2xl: 2.25rem;  /* 36px */
--radius-full: 9999px;  /* Circular */
```

## Shadows

Apple-style subtle, layered shadows:
```css
--shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.03);
--shadow: 0 1px 3px 0 rgb(0 0 0 / 0.06), 0 1px 2px -1px rgb(0 0 0 / 0.06);
--shadow-md: 0 4px 8px -2px rgb(0 0 0 / 0.08), 0 2px 4px -2px rgb(0 0 0 / 0.05);
--shadow-lg: 0 10px 20px -5px rgb(0 0 0 / 0.1), 0 4px 8px -4px rgb(0 0 0 / 0.06);
--shadow-xl: 0 20px 30px -8px rgb(0 0 0 / 0.12), 0 8px 12px -6px rgb(0 0 0 / 0.08);
```

## Transitions

Apple-inspired smooth, natural animations:
```css
--transition-fast: 200ms cubic-bezier(0.4, 0, 0.2, 1);
--transition-base: 300ms cubic-bezier(0.4, 0, 0.2, 1);
--transition-slow: 400ms cubic-bezier(0.4, 0, 0.2, 1);
--transition-spring: 400ms cubic-bezier(0.34, 1.56, 0.64, 1);
```

The cubic-bezier values create Apple's characteristic "ease-in-out" effect that feels natural and polished.

## Button Styles

### Primary Button
- Background: `var(--primary-500)` with gradient
- Color: White
- Hover: Slight lift (-1px transform) with enhanced shadow
- Active: Returns to original position
- Focus: Blue ring for accessibility

### Secondary Button
- Background: White
- Color: `var(--primary-600)`
- Border: 1.5px solid `var(--gray-300)`
- Hover: Slight lift with gray background

### Accent Button
- Background: `var(--accent-500)` (Purple)
- Used for premium or special actions

## Form Elements

### Input Fields
- Padding: 0.75rem 1rem
- Border: 2px solid `var(--gray-300)`
- Border radius: `var(--radius-lg)`
- Font-size: 1.0625rem
- Focus: Blue border with subtle shadow ring

### Labels
- Font-weight: 500
- Color: `var(--gray-700)`
- Margin-bottom: 0.5rem

## Accessibility

### Focus States
All interactive elements have visible focus rings for keyboard navigation:
```css
--focus-ring: 0 0 0 3px rgba(14, 165, 233, 0.2);
--focus-ring-error: 0 0 0 3px rgba(239, 68, 68, 0.2);
--focus-ring-success: 0 0 0 3px rgba(16, 185, 129, 0.2);
```

### Color Contrast
All text colors meet WCAG AA standards:
- Body text: `var(--gray-600)` on white background
- Headings: `var(--gray-900)` on white background
- Buttons: High contrast ratios

## Responsive Breakpoints

```css
/* Mobile First Approach */
@media (max-width: 768px)  { /* Mobile */ }
@media (max-width: 1024px) { /* Tablet */ }
@media (min-width: 1025px) { /* Desktop */ }
```

## Design Principles

1. **Clarity**: Large, legible typography with generous spacing
2. **Depth**: Subtle shadows create hierarchy without distraction
3. **Simplicity**: Clean, minimal design with focus on content
4. **Fluidity**: Smooth transitions and natural animations
5. **Consistency**: Unified design language across all applications
6. **Performance**: No external font loading, system fonts only
7. **Accessibility**: Proper focus states, color contrast, and semantic HTML

## Implementation Status

- ✅ **mw-site**: Fully implemented with refined typography
- ✅ **mw-docs**: Updated with comprehensive design system
- ✅ **medicwarehouse-app**: Apple system fonts, comprehensive tokens
- ✅ **mw-system-admin**: Apple system fonts, refined styling

## Resources

- [Apple Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)
- [SF Pro Font](https://developer.apple.com/fonts/)
- [Apple Design Resources](https://developer.apple.com/design/resources/)

## Maintenance

To maintain consistency:
1. Always use CSS variables for colors, spacing, and typography
2. Follow the established font-size scale
3. Use the transition variables for all animations
4. Test on both Apple and non-Apple devices
5. Ensure proper fallbacks for all features

---

**Last Updated**: December 23, 2025
**Maintained by**: MedicWarehouse Development Team
