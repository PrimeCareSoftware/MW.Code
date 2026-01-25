# Dark Mode and High Contrast Theme Implementation

## Overview
This implementation adds dark mode (night mode) and high contrast themes to all Angular applications (medicwarehouse-app, patient-portal, and mw-system-admin) for improved accessibility and user experience.

## Features

### 1. Three Theme Options
- **Light Mode** (default): Standard bright theme
- **Dark Mode**: Eye-friendly dark theme for reduced eye strain
- **High Contrast Mode**: Maximum contrast for users with visual impairments (WCAG 2.1 AA compliant)

### 2. Automatic Theme Detection
- Detects system theme preference (`prefers-color-scheme`)
- Saves user's theme preference in localStorage
- Theme persists across sessions and page reloads

### 3. Accessibility Features
- High contrast mode with:
  - Strong borders (3px minimum)
  - High contrast colors (black/white/yellow)
  - Prominent focus indicators (3px yellow outline)
  - Maximum text readability
- Keyboard accessible theme toggle
- ARIA labels and roles for screen readers

## Implementation Details

### Files Added

#### Theme Service (`theme.service.ts`)
```typescript
frontend/medicwarehouse-app/src/app/services/theme.service.ts
frontend/patient-portal/src/app/services/theme.service.ts
frontend/mw-system-admin/src/app/services/theme.service.ts
```

Manages theme state and applies theme classes to the body element.

#### Theme Toggle Component (`theme-toggle.component.ts`)
```typescript
frontend/medicwarehouse-app/src/app/shared/theme-toggle/theme-toggle.component.ts
frontend/patient-portal/src/app/shared/theme-toggle/theme-toggle.component.ts
frontend/mw-system-admin/src/app/shared/theme-toggle/theme-toggle.component.ts
```

Provides UI controls for switching between themes.

### CSS Variables

Each theme defines CSS custom properties for:
- Background colors (`--bg-primary`, `--bg-secondary`, `--bg-tertiary`)
- Text colors (`--text-primary`, `--text-secondary`, `--text-tertiary`)
- Border colors (`--border-color`, `--border-color-hover`)
- Primary, gray, and semantic colors (success, warning, error, info)
- Shadows and other UI elements

### Theme Classes

The service adds one of these classes to the body element:
- `theme-light` - Light theme (default)
- `theme-dark` - Dark theme
- `theme-high-contrast` - High contrast theme

## Usage

### For Users

1. Locate the theme toggle buttons (usually in the top navigation bar)
2. Click the desired theme:
   - ☀️ **Claro** - Light theme
   - 🌙 **Noturno** - Dark theme
   - ◐ **Alto Contraste** - High contrast theme
3. The theme preference is automatically saved

### For Developers

#### Using the Theme Service

```typescript
import { ThemeService } from './services/theme.service';

constructor(private themeService: ThemeService) {}

// Get current theme
const currentTheme = this.themeService.getTheme();

// Set theme
this.themeService.setTheme('dark');

// Toggle through themes
this.themeService.toggleTheme();

// Check theme type
if (this.themeService.isDark()) {
  // Dark mode specific logic
}
```

#### Using Theme Variables in CSS

```scss
.my-component {
  background-color: var(--bg-primary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  
  &:hover {
    border-color: var(--border-color-hover);
  }
}
```

#### Adding the Theme Toggle Component

```typescript
// In your component imports
import { ThemeToggleComponent } from '../shared/theme-toggle/theme-toggle.component';

@Component({
  selector: 'my-component',
  imports: [ThemeToggleComponent, ...],
  // ...
})
```

```html
<!-- In your template -->
<app-theme-toggle></app-theme-toggle>
```

## Browser Support

- Modern browsers with CSS custom properties support
- localStorage support for theme persistence
- Respects system-level `prefers-color-scheme` media query

## Accessibility Compliance

This implementation follows WCAG 2.1 Level AA guidelines:

- **High Contrast Mode**:
  - Minimum contrast ratio of 7:1 for text
  - Minimum contrast ratio of 3:1 for UI components
  - Clear focus indicators (3px yellow outline)
  - Strong borders for all interactive elements

- **Keyboard Navigation**:
  - All theme toggle buttons are keyboard accessible
  - Clear focus states
  - Proper ARIA labels and roles

- **Screen Reader Support**:
  - Semantic HTML with proper ARIA attributes
  - Descriptive button labels
  - State announcement via `aria-pressed`

## Testing

To test the themes:

1. **Visual Testing**: Switch between themes and verify all UI elements display correctly
2. **Keyboard Testing**: Tab through theme controls and activate with Enter/Space
3. **Screen Reader Testing**: Use a screen reader to verify announcements
4. **Persistence Testing**: Refresh the page and verify theme persists
5. **System Preference Testing**: Change system theme and verify new sessions respect it

## Future Enhancements

Potential improvements for future versions:
- Custom theme colors
- Per-component theme overrides
- Animated theme transitions
- More theme variants (e.g., sepia, blue light filter)
- Scheduled theme switching (automatic day/night switching)
