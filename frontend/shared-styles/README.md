# MedicWarehouse Design System

## 📋 Overview

This design system provides a unified visual identity across all MedicWarehouse systems. It normalizes colors, typography, components, and themes to ensure consistency and maintainability.

**Latest Update (February 2026):** Design system updated to align with **Clinic Harmony UI** - an Apple-inspired minimalist healthcare design featuring softer colors, subtle shadows, and smooth animations.

## 🎨 Color Palette

### Primary - Soft Medical Blue (Clinic Harmony inspired)
- **Base Color**: `#4A9FDE` (primary-500) - HSL: 211 84% 55%
- **Previous**: `#1e40af` (darker blue)
- **Change**: Softer, more approachable medical blue
- **Usage**: Primary actions, links, focus states
- **Shades**: 50-900 scale available

### Accent - Subtle Teal (NEW from Clinic Harmony)
- **Base Color**: `#14b8a6` (accent-500) - HSL: 174 62% 47%
- **Usage**: Highlights, interactive elements, secondary actions
- **Previous**: Purple accent (#a855f7)
- **Change**: Teal provides better contrast and modern feel

### Secondary - Subtle Purple (Maintained)
- **Base Color**: `#a855f7` (secondary-500)
- **Usage**: Secondary actions, highlights, premium features

### Semantic Colors
- **Success**: `#22c55e` (success-500)
- **Warning**: `#f59e0b` (warning-500)
- **Error**: `#ef4444` (error-500)
- **Info**: `#3b82f6` (info-500)

### Neutral Colors
- **Grays**: 50-900 scale
- **Apple-inspired**: Clean, modern, professional

## 🏗️ Structure

```
frontend/
├── shared-styles/
│   ├── _design-tokens.scss     # Color palette, spacing, typography
│   ├── _components.scss        # Reusable UI components
│   ├── _utilities.scss         # Utility classes
│   └── index.scss              # Barrel export
│
├── medicwarehouse-app/         # Main application
├── mw-system-admin/            # Admin panel
├── patient-portal/             # Patient portal
└── mw-docs/                    # Documentation site
```

## 🚀 Usage

### In New Apps

Import the design system at the top of your main styles file:

```scss
@use '../shared-styles' as *;
```

### Available Tokens

#### Colors
```scss
var(--primary-500)      // #1e40af
var(--secondary-500)    // #a855f7
var(--success-500)      // #22c55e
var(--warning-500)      // #f59e0b
var(--error-500)        // #ef4444
var(--gray-50) to var(--gray-900)
```

#### Spacing (4px base)
```scss
var(--spacing-1)  // 4px
var(--spacing-2)  // 8px
var(--spacing-3)  // 12px
var(--spacing-4)  // 16px
var(--spacing-6)  // 24px
var(--spacing-8)  // 32px
```

#### Typography
```scss
var(--font-sans)        // Inter, system fonts
var(--font-display)     // Plus Jakarta Sans
var(--font-mono)        // SF Mono, Consolas

var(--font-size-xs)     // 12px
var(--font-size-sm)     // 14px
var(--font-size-base)   // 16px
var(--font-size-lg)     // 18px
var(--font-size-xl)     // 20px
```

#### Border Radius
```scss
var(--radius-sm)   // 6px
var(--radius)      // 8px
var(--radius-md)   // 12px
var(--radius-lg)   // 16px
var(--radius-full) // 9999px (pill)
```

#### Shadows
```scss
var(--shadow-xs)
var(--shadow-sm)
var(--shadow)
var(--shadow-md)
var(--shadow-lg)
var(--shadow-xl)
```

#### Transitions
```scss
var(--transition-fast)  // 150ms
var(--transition-base)  // 250ms
var(--transition-slow)  // 350ms
```

## 🎭 Themes

### Light Theme (Default)
- Background: White (#ffffff)
- Text: Dark gray (#171717)
- High contrast, easy to read

### Dark Theme
- Background: Dark blue (#0f172a)
- Text: Light gray (#f1f5f9)
- Inverted color scales
- Deeper shadows

### High Contrast Theme
- Background: Pure black (#000000)
- Text: Pure white (#ffffff)
- Primary: Yellow (#ffeb3b)
- Maximum accessibility (WCAG AAA)
- Bold borders and outlines

### Applying Themes

Add theme class to body element:

```html
<body class="theme-dark">      <!-- Dark theme -->
<body class="theme-high-contrast">  <!-- High contrast theme -->
```

## 🧩 Components

Pre-built, accessible components included:

### Buttons
```html
<button class="btn btn-primary">Primary Action</button>
<button class="btn btn-secondary">Secondary</button>
<button class="btn btn-danger">Delete</button>
<button class="btn btn-success">Confirm</button>

<!-- Sizes -->
<button class="btn btn-primary btn-sm">Small</button>
<button class="btn btn-primary btn-lg">Large</button>
```

### Cards
```html
<div class="card">
  <div class="card-header">
    <h3>Card Title</h3>
  </div>
  <div class="card-body">
    Content goes here
  </div>
  <div class="card-footer">
    <button class="btn btn-primary">Action</button>
  </div>
</div>
```

### Badges
```html
<span class="badge badge-primary">New</span>
<span class="badge badge-success">Active</span>
<span class="badge badge-warning">Pending</span>
<span class="badge badge-error">Error</span>
```

### Alerts
```html
<div class="alert alert-success">Operation successful!</div>
<div class="alert alert-error">An error occurred</div>
<div class="alert alert-warning">Please be careful</div>
<div class="alert alert-info">Did you know?</div>
```

### Forms
```html
<div class="form-group">
  <label class="form-label required">Email</label>
  <input type="email" class="form-control" placeholder="user@example.com">
  <div class="form-help">We'll never share your email</div>
</div>
```

## 🔧 Utility Classes

### Spacing
```scss
.m-{0-8}     // Margin
.mt-{0-8}    // Margin top
.mb-{0-8}    // Margin bottom
.ml-{0-8}    // Margin left
.mr-{0-8}    // Margin right
.p-{0-8}     // Padding
.pt-{0-8}    // Padding top
// etc.
```

### Display
```scss
.d-none, .d-block, .d-flex, .d-grid
.flex-row, .flex-column
.justify-center, .justify-between
.align-center, .align-start
.gap-{0-8}
```

### Text
```scss
.text-center, .text-left, .text-right
.text-primary, .text-secondary, .text-tertiary
.text-success, .text-error, .text-warning
.font-weight-{light|normal|medium|semibold|bold}
.font-size-{xs|sm|base|lg|xl|2xl|3xl}
```

### Width & Height
```scss
.w-{25|50|75|100|auto}
.h-{25|50|75|100|auto}
```

### Border & Rounded
```scss
.border, .border-top, .border-bottom
.rounded, .rounded-sm, .rounded-lg, .rounded-full
```

### Shadows
```scss
.shadow-xs, .shadow-sm, .shadow, .shadow-md, .shadow-lg
```

## 📦 Integration Status

| System | Status | Notes |
|--------|--------|-------|
| MedicWarehouse App | ✅ Migrated | Uses shared tokens + app-specific extensions |
| System Admin | ✅ Migrated | Uses shared tokens + Shepherd.js theme |
| Patient Portal | ✅ Migrated | Migrated from Teal/HSL to Blue/HEX |
| MW Docs | ✅ Migrated | Migrated from Light Blue to Medical Blue |

## 🎯 Design Principles

1. **Consistency**: Same colors, spacing, and components across all apps
2. **Accessibility**: WCAG 2.1 AA compliance minimum, AAA for high contrast
3. **Maintainability**: Single source of truth for design tokens
4. **Scalability**: Easy to add new apps or update global styles
5. **Apple-Inspired**: Clean, modern, professional aesthetic

## 🔄 Migration Notes

### Breaking Changes
- **Patient Portal**: Changed from Teal (#17A589) to Medical Blue (#1e40af)
- **MW Docs**: Changed from Light Blue (#0ea5e9) to Medical Blue (#1e40af)
- **Patient Portal**: Removed HSL format, now uses standard HEX

### Compatibility Aliases
The following aliases are provided for backward compatibility:

```scss
--primary-color          // Same as --primary-500
--primary-color-hover    // Same as --primary-600
--text-primary           // Contextual text color
--bg-primary            // Contextual background color
--border-color          // Contextual border color
```

## 📝 Contributing

When adding new design tokens or components:

1. Add to appropriate file in `frontend/shared-styles/`
2. Document in this README
3. Test in all themes (Light, Dark, High Contrast)
4. Ensure WCAG AA accessibility compliance
5. Update all affected apps

## 🧪 Testing

Before deploying changes:

1. Build all frontend apps
2. Visual regression testing in all themes
3. Accessibility audit (Lighthouse, axe)
4. Cross-browser testing
5. Responsive design verification

## 📚 References

- [Material Design 3](https://m3.material.io/)
- [Tailwind CSS Colors](https://tailwindcss.com/docs/customizing-colors)
- [Apple Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)
- [WCAG 2.1](https://www.w3.org/WAI/WCAG21/quickref/)

## 🤝 Support

For questions or issues with the design system, contact the frontend team or create an issue in the repository.
