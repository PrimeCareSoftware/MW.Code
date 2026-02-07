# Color Migration Guide - MedicWarehouse App

## Purpose
This guide maps hardcoded color values to design tokens from the shared design system.

## Color Token Mapping

### Primary Colors
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `#667eea` | `var(--primary-500)` | Primary brand color (legacy purple) |
| `#1e40af` | `var(--primary-500)` | Primary brand color (Medical Blue - current) |
| `#764ba2` | `var(--secondary-700)` | Secondary/gradient color |
| `#5568d3` | `var(--primary-600)` | Primary hover state |

### Neutral/Gray Colors
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `#f8f9fa` | `var(--gray-50)` | Light background |
| `#f5f5f5` | `var(--gray-100)` | Very light background |
| `#f0f0f0` | `var(--gray-100)` | Light background |
| `#e0e0e0` | `var(--gray-200)` | Border, divider |
| `#c3cfe2` | `var(--gray-300)` | Medium border |
| `#1a202c` | `var(--gray-900)` | Primary text |
| `#2d3748` | `var(--gray-800)` | Secondary text |
| `#718096` | `var(--gray-500)` | Tertiary text |
| `#a0aec0` | `var(--gray-400)` | Light text |

### Semantic Colors

#### Error/Danger
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `#dc3545` | `var(--error-500)` | Error, danger button |
| `#c82333` | `var(--error-600)` | Error hover state |
| `#f44336` | `var(--error-500)` | Error border |
| `#f8d7da` | `var(--error-100)` | Error background |
| `#721c24` | `var(--error-700)` | Error text |
| `#f5c6cb` | `var(--error-200)` | Error border light |
| `#c53030` | `var(--error-600)` | Error text dark |

#### Success
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `#065f46` | `var(--success-700)` | Success text |
| `#d1fae5` | `var(--success-100)` | Success background |
| `#166534` | `var(--success-700)` | Success text dark |
| `#f0fdf4` | `var(--success-50)` | Success background light |

#### Info
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `#1e40af` | `var(--info-700)` | Info text |
| `#dbeafe` | `var(--info-100)` | Info background |

### Shadows
| Hardcoded Value | Design Token | Usage |
|----------------|--------------|-------|
| `0 2px 8px rgba(0, 0, 0, 0.1)` | `var(--shadow-md)` | Card shadow |
| `0 4px 12px rgba(102, 126, 234, 0.4)` | `var(--shadow-primary)` | Primary button hover |
| `0 4px 16px rgba(0, 0, 0, 0.15)` | `var(--shadow-lg)` | Elevated card |
| `0 1px 3px rgba(0, 0, 0, 0.1)` | `var(--shadow-sm)` | Subtle shadow |
| `0 0 0 3px rgba(102, 126, 234, 0.1)` | `var(--focus-ring)` | Focus ring |

### Gradients
| Hardcoded Value | Design Token Alternative |
|----------------|--------------------------|
| `linear-gradient(135deg, #667eea 0%, #764ba2 100%)` | Use button component class `.btn-primary` |
| `linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)` | Use `.bg-gradient-light` utility |

## Migration Steps

1. **Find and Replace**: Use the mapping above to replace hardcoded values
2. **Test Visual Appearance**: Ensure colors look correct after migration
3. **Check Dark Mode**: Verify colors work in dark theme
4. **Validate Accessibility**: Ensure contrast ratios are maintained

## Components to Migrate

### Priority 1 (User-facing)
- [ ] `src/app/pages/admin/profiles/profile-form.component.scss`
- [ ] `src/app/pages/admin/profiles/profile-list.component.scss`

### Priority 2 (Features)
- [ ] `src/app/pages/telemedicine/identity-verification-upload/identity-verification-upload.scss`
- [ ] `src/app/pages/telemedicine/session-form/session-form.scss`
- [ ] `src/app/pages/telemedicine/session-details/session-details.scss`
- [ ] `src/app/pages/telemedicine/session-list/session-list.scss`
- [ ] `src/app/pages/telemedicine/session-compliance-checker/session-compliance-checker.scss`

## Example Migration

### Before:
```scss
.card {
  background-color: #f8f9fa;
  border: 1px solid #e0e0e0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  
  .title {
    color: #1a202c;
  }
  
  .subtitle {
    color: #718096;
  }
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  
  &:hover {
    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
  }
}
```

### After:
```scss
.card {
  background-color: var(--gray-50);
  border: 1px solid var(--gray-200);
  box-shadow: var(--shadow-md);
  
  .title {
    color: var(--gray-900);
  }
  
  .subtitle {
    color: var(--gray-500);
  }
}

.btn-primary {
  background: var(--primary-500);
  color: white;
  
  &:hover {
    box-shadow: var(--shadow-primary);
  }
}
```

## Benefits of Migration

✅ **Consistency**: All colors follow the design system  
✅ **Theme Support**: Automatic dark mode support  
✅ **Maintainability**: Single source of truth for colors  
✅ **Accessibility**: Verified contrast ratios  
✅ **Future-proof**: Easy to update entire system from one place  
