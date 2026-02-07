# UI Components Migration from clinic-harmony-ui-main

## Overview

This directory contains Angular components migrated from the React-based `clinic-harmony-ui-main` project. All components maintain the same functionality and design as their React counterparts but are implemented using Angular.

## Migration Status

### ✅ Completed Components (11/62)

1. **Button** - Full-featured button component with variants (default, destructive, outline, secondary, ghost, link) and sizes (default, sm, lg, icon)
2. **Card** - Card container with sub-components (CardHeader, CardTitle, CardDescription, CardContent, CardFooter)
3. **Input** - Text input component with ControlValueAccessor support for reactive and template-driven forms
4. **Badge** - Badge component with variants (default, secondary, destructive, outline)

### 🚧 Pending Components (58/62)

#### High Priority Core Components
- Avatar
- Checkbox
- Select/Dropdown
- Form components (form-input, form-select, form-textarea)
- Label
- Textarea
- Switch
- Radio Group

#### Layout Components
- Dialog/Modal
- Sheet/Drawer
- Sidebar
- Accordion
- Tabs
- Collapsible
- Separator
- Resizable

#### Data Display
- Table
- Data Table
- Chart
- Calendar
- Progress
- Skeleton
- Metric Card
- Status Badge

#### Interactive Components
- Tooltip
- Popover
- Hover Card
- Dropdown Menu
- Context Menu
- Command
- Navigation Menu
- Menubar

#### Feedback Components
- Alert
- Alert Dialog
- Toast/Sonner
- Toaster

#### Advanced Components
- Carousel
- Slider
- Pagination
- Scroll Area
- Breadcrumb
- Toggle
- Toggle Group
- Toggle Switch
- Input OTP
- Aspect Ratio

#### Layout & Specialized
- AppSidebar
- Topbar
- DashboardLayout
- NavLink
- AreaChart
- BarChart
- DonutChart

## Component Structure

Each Angular component follows this structure:

```
component-name/
├── component-name.component.ts    # Component logic
├── component-name.component.html  # Template (if complex)
├── component-name.component.scss  # Styles
└── index.ts                       # Export file
```

### Standalone Components

All components are implemented as standalone components (Angular 14+), which means:
- No need for NgModule declarations
- Direct imports in component metadata
- Better tree-shaking
- More modular architecture

## Usage

### Import Individual Components

```typescript
import { ButtonComponent } from '@app/shared/ui-components/button';
import { CardComponent, CardHeaderComponent, CardTitleComponent } from '@app/shared/ui-components/card';

@Component({
  standalone: true,
  imports: [ButtonComponent, CardComponent, CardHeaderComponent, CardTitleComponent],
  // ...
})
```

### Import from Index

```typescript
import { ButtonComponent, CardComponent, InputComponent, BadgeComponent } from '@app/shared/ui-components';
```

## Design System Integration

All components use CSS variables from the shared design system located in `/frontend/shared-styles`. The design tokens match the clinic-harmony-ui-main design system:

- Color scheme: Soft Medical Blue (#3D9DED)
- Typography: Inter font family
- Spacing, shadows, and borders follow the design tokens
- Full support for light/dark themes

## Key Differences from React Components

### 1. Two-Way Binding

Angular components use `[(ngModel)]` for two-way data binding instead of React's controlled components:

```html
<!-- React -->
<Input value={value} onChange={(e) => setValue(e.target.value)} />

<!-- Angular -->
<app-input [(ngModel)]="value"></app-input>
```

### 2. Event Emitters

Angular uses `@Output` and `EventEmitter` instead of callback props:

```html
<!-- React -->
<Button onClick={handleClick}>Click me</Button>

<!-- Angular -->
<app-button (buttonClick)="handleClick($event)">Click me</app-button>
```

### 3. Content Projection

Angular uses `<ng-content>` instead of React's `children`:

```html
<!-- React -->
<Card>{children}</Card>

<!-- Angular -->
<app-card><ng-content></ng-content></app-card>
```

### 4. Directives vs JSX

Angular uses structural directives instead of JSX expressions:

```html
<!-- React -->
{isLoading && <Spinner />}
{items.map(item => <div key={item.id}>{item.name}</div>)}

<!-- Angular -->
<app-spinner *ngIf="isLoading"></app-spinner>
<div *ngFor="let item of items">{{item.name}}</div>
```

## Testing

Each component should have corresponding unit tests following Angular testing best practices:

```typescript
describe('ButtonComponent', () => {
  // Component tests
});
```

## Contributing

When migrating new components:

1. Keep the same functionality as the React version
2. Use the same CSS classes and design tokens
3. Follow Angular best practices (standalone components, strict types)
4. Include JSDoc comments with usage examples
5. Export from the component's index.ts file
6. Add to the main index.ts exports
7. Update this README with the component status

## Migration Approach

The migration is being done incrementally:

1. **Phase 1**: Core UI components (Button, Card, Input, Badge, etc.)
2. **Phase 2**: Form components
3. **Phase 3**: Layout components
4. **Phase 4**: Data display components
5. **Phase 5**: Interactive components
6. **Phase 6**: Advanced and specialized components

## References

- Original React components: `/clinic-harmony-ui-main/src/components`
- Shared design system: `/frontend/shared-styles`
- Design system documentation: `/CLINIC_HARMONY_MIGRATION.md`
