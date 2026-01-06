# Input Mask Directives Documentation

## Overview

This document describes the input mask directives implemented to improve user experience when entering formatted data in forms across the MedicWarehouse frontend applications.

## Purpose

Input masks automatically format data as the user types, providing immediate visual feedback and ensuring data is entered in the correct format. This improves:
- **User Experience**: Users see the expected format immediately
- **Data Quality**: Reduces formatting errors and inconsistencies
- **Accessibility**: Clear visual indication of expected input format

## Available Directives

### 1. CPF Mask Directive (`appCpfMask`)

**Format**: `000.000.000-00`

**Usage**:
```html
<input type="text" [(ngModel)]="cpf" appCpfMask placeholder="000.000.000-00" />
```

**Behavior**:
- Accepts only numeric input
- Automatically adds dots and hyphen as user types
- Limits input to 11 digits
- Formats on input: `123` → `123`, `12345678` → `123.456.78`
- Formats on blur: `12345678901` → `123.456.789-01`

**Location**: 
- `frontend/mw-site/src/app/directives/cpf-mask.directive.ts`
- `frontend/medicwarehouse-app/src/app/directives/cpf-mask.directive.ts`

### 2. CNPJ Mask Directive (`appCnpjMask`)

**Format**: `00.000.000/0000-00`

**Usage**:
```html
<input type="text" [(ngModel)]="cnpj" appCnpjMask placeholder="00.000.000/0000-00" />
```

**Behavior**:
- Accepts only numeric input
- Automatically adds dots, slash, and hyphen as user types
- Limits input to 14 digits
- Formats progressively: `12345678` → `12.345.678`
- Full format on blur: `12345678901234` → `12.345.678/9012-34`

**Location**: 
- `frontend/mw-site/src/app/directives/cnpj-mask.directive.ts`
- `frontend/medicwarehouse-app/src/app/directives/cnpj-mask.directive.ts`

### 3. Phone Mask Directive (`appPhoneMask`)

**Formats**: 
- Landline: `(00) 0000-0000` (10 digits)
- Mobile: `(00) 00000-0000` (11 digits)

**Usage**:
```html
<input type="tel" [(ngModel)]="phone" appPhoneMask placeholder="(00) 00000-0000" />
```

**Behavior**:
- Accepts only numeric input
- Automatically adds parentheses, space, and hyphen
- Limits input to 11 digits (with area code)
- Adapts format based on number length
- Handles both landline and mobile formats

**Location**: 
- `frontend/mw-site/src/app/directives/phone-mask.directive.ts`
- `frontend/medicwarehouse-app/src/app/directives/phone-mask.directive.ts`

### 4. CEP Mask Directive (`appCepMask`)

**Format**: `00000-000`

**Usage**:
```html
<input type="text" [(ngModel)]="cep" appCepMask placeholder="00000-000" />
```

**Behavior**:
- Accepts only numeric input
- Automatically adds hyphen as user types
- Limits input to 8 digits
- Formats: `12345678` → `12345-678`

**Location**: 
- `frontend/mw-site/src/app/directives/cep-mask.directive.ts`
- `frontend/medicwarehouse-app/src/app/directives/cep-mask.directive.ts`

### 5. Date Mask Directive (`appDateMask`)

**Format**: `DD/MM/YYYY`

**Usage**:
```html
<input type="text" [(ngModel)]="date" appDateMask placeholder="DD/MM/YYYY" />
```

**Behavior**:
- Only works on `type="text"` inputs (skips HTML5 date inputs)
- Accepts only numeric input
- Automatically adds slashes as user types
- Limits input to 8 digits
- Formats: `01012024` → `01/01/2024`

**Note**: This directive is designed for text inputs. HTML5 `type="date"` inputs already have browser-native formatting and should be used when possible.

**Location**: 
- `frontend/mw-site/src/app/directives/date-mask.directive.ts`
- `frontend/medicwarehouse-app/src/app/directives/date-mask.directive.ts`

## Implementation

### Forms Using Masks

#### mw-site (Registration Form)
- `clinicCNPJ` - CNPJ mask
- `clinicPhone` - Phone mask
- `ownerCPF` - CPF mask
- `ownerPhone` - Phone mask
- `zipCode` - CEP mask

#### medicwarehouse-app (Patient Form)
- `document` - CPF mask
- `phoneNumber` - Phone mask
- `zipCode` - CEP mask

### How to Add a Mask to a New Form

1. Import the directive in your component:
```typescript
import { CpfMaskDirective } from '../../directives/cpf-mask.directive';

@Component({
  selector: 'app-your-component',
  imports: [..., CpfMaskDirective],
  // ...
})
```

2. Add the directive to your input field:
```html
<input type="text" formControlName="cpf" appCpfMask placeholder="000.000.000-00" />
```

## Technical Details

### Event Listeners

Each directive uses two event listeners:
- **`@HostListener('input')`**: Formats the input progressively as the user types
- **`@HostListener('blur')`**: Applies final formatting when the user leaves the field

### Character Filtering

All directives use `replace(/\D/g, '')` to strip non-numeric characters, ensuring only valid data is processed.

### Length Limiting

Each directive enforces maximum length limits to prevent over-input:
- CPF: 11 digits
- CNPJ: 14 digits
- Phone: 11 digits
- CEP: 8 digits
- Date: 8 digits

### Progressive Formatting

The directives apply formatting progressively as the user types, showing the mask structure only when relevant. For example, the CPF mask shows dots only after 3 digits are entered.

## Browser Compatibility

These directives work with all modern browsers that support Angular 20+. They are implemented as standalone directives and don't require any external libraries.

## Maintenance

The directives are self-contained and require minimal maintenance. If formatting rules change (e.g., phone number format), update the regular expressions in the respective directive.

## Testing

To test the masks:
1. Build and run the frontend application
2. Navigate to forms with masked inputs
3. Verify:
   - Only numeric input is accepted
   - Formatting appears as you type
   - Formatting is correct on blur
   - Maximum length is enforced
   - Pasting formatted or unformatted values works correctly

## Future Enhancements

Potential improvements for the directives:
- Add validation logic to verify checksum digits (CPF/CNPJ)
- Add configurable options (e.g., optional mask characters)
- Create a generic mask directive with pattern parameter
- Add unit tests for each directive
- Add locale support for different formats (international phone numbers, etc.)

## Related Files

- Form components using masks:
  - `/frontend/mw-site/src/app/pages/register/register.ts`
  - `/frontend/mw-site/src/app/pages/register/register.html`
  - `/frontend/medicwarehouse-app/src/app/pages/patients/patient-form/patient-form.ts`
  - `/frontend/medicwarehouse-app/src/app/pages/patients/patient-form/patient-form.html`

## Support

For questions or issues related to the input masks, please refer to the Angular documentation on directives or contact the development team.
