# System Admin Color Migration Plan

## Overview
System Admin has **1,252 hardcoded color instances** that need to be migrated to design tokens. This document outlines a systematic approach to handle this large-scale refactoring.

## Color Mapping Reference

### Most Common Hardcoded Colors (Priority Order)

| Hardcoded | Design Token | Usage | Count (Est.) |
|-----------|--------------|-------|--------------|
| `#667eea` | `var(--primary-500)` | Primary brand color | ~300 |
| `#1a202c` | `var(--gray-900)` | Primary text | ~150 |
| `#2d3748` | `var(--gray-800)` | Secondary text | ~120 |
| `#718096` | `var(--gray-500)` | Tertiary text | ~100 |
| `#e2e8f0` | `var(--gray-200)` | Borders, dividers | ~150 |
| `#f7fafc` | `var(--gray-50)` | Light background | ~100 |
| `#a0aec0` | `var(--gray-400)` | Light text, placeholders | ~80 |
| `#dbeafe` | `var(--info-100)` | Info background | ~40 |
| `#1e40af` | `var(--info-700)` | Info text | ~30 |
| `#d1fae5` | `var(--success-100)` | Success background | ~30 |
| `#065f46` | `var(--success-700)` | Success text | ~25 |
| `#166534` | `var(--success-700)` | Success text dark | ~20 |
| `#f0fdf4` | `var(--success-50)` | Success background light | ~15 |
| `#5568d3` | `var(--primary-600)` | Primary hover | ~20 |
| `#cbd5e0` | `var(--gray-300)` | Medium borders | ~17 |

### Other Common Colors
- `#c53030` → `var(--error-600)` (Error dark)
- `white` → Keep as is (standard)
- `#f7fafc` → `var(--gray-50)` (Light bg)

## Migration Strategy

### Phase 2.1: Automated Search & Replace (70% coverage)
Create a script to automatically replace the most common patterns in all SCSS files.

**Target Files**: All `.scss` files in `src/app/pages/`

**Replacement Rules**:
```
#667eea → var(--primary-500)
#1a202c → var(--gray-900)
#2d3748 → var(--gray-800)
#718096 → var(--gray-500)
#e2e8f0 → var(--gray-200)
#f7fafc → var(--gray-50)
#a0aec0 → var(--gray-400)
#dbeafe → var(--info-100)
#1e40af → var(--info-700)
#d1fae5 → var(--success-100)
#065f46 → var(--success-700)
#166534 → var(--success-700)
#f0fdf4 → var(--success-50)
#5568d3 → var(--primary-600)
#cbd5e0 → var(--gray-300)
#c53030 → var(--error-600)
```

### Phase 2.2: Manual Review of Critical Components
After automated replacement, manually review:
- Navigation components
- Dashboard components
- Form components with complex styling
- Modal dialogs

### Phase 2.3: Build & Test
- Run `npm run build` to check for errors
- Visual regression testing on key pages
- Validate dark mode compatibility

### Phase 2.4: Edge Cases
Handle remaining hardcoded values that don't fit the common patterns.

## Component Priority List

### High Priority (User-Facing)
1. `exam-catalog/exam-catalog-list.scss` (~200 instances)
2. `clinic-owners/clinic-owner-*.scss` (~150 instances)
3. `modules-dashboard/modules-dashboard.component.scss` (~100 instances)
4. `plan-modules/plan-modules.component.scss` (~80 instances)
5. `audit-logs/audit-logs.scss` (~120 instances)

### Medium Priority (Admin Features)
6. Subscription management components
7. User management components
8. Settings and configuration pages

### Low Priority (Less Frequently Used)
9. Historical reports
10. Advanced settings
11. Developer tools

## Automation Script Template

```bash
#!/bin/bash
# Color migration script for System Admin

# Define color mappings
declare -A COLOR_MAP=(
    ["#667eea"]="var(--primary-500)"
    ["#1a202c"]="var(--gray-900)"
    ["#2d3748"]="var(--gray-800)"
    ["#718096"]="var(--gray-500)"
    ["#e2e8f0"]="var(--gray-200)"
    ["#f7fafc"]="var(--gray-50)"
    ["#a0aec0"]="var(--gray-400)"
    ["#dbeafe"]="var(--info-100)"
    ["#1e40af"]="var(--info-700)"
    ["#d1fae5"]="var(--success-100)"
    ["#065f46"]="var(--success-700)"
    ["#166534"]="var(--success-700)"
    ["#f0fdf4"]="var(--success-50)"
    ["#5568d3"]="var(--primary-600)"
    ["#cbd5e0"]="var(--gray-300)"
    ["#c53030"]="var(--error-600)"
)

# Target directory
TARGET_DIR="frontend/mw-system-admin/src/app/pages"

# Create backup
echo "Creating backup..."
cp -r $TARGET_DIR ${TARGET_DIR}_backup_$(date +%Y%m%d_%H%M%S)

# Replace colors
for color in "${!COLOR_MAP[@]}"; do
    token="${COLOR_MAP[$color]}"
    echo "Replacing $color with $token"
    find $TARGET_DIR -name "*.scss" -type f -exec sed -i "s/$color/$token/g" {} +
done

echo "Migration complete. Please review changes and test."
```

## Testing Checklist

After migration:
- [ ] Build completes without errors
- [ ] No visual regressions on:
  - [ ] Exam catalog listing
  - [ ] Clinic owners management
  - [ ] Modules dashboard
  - [ ] Audit logs
  - [ ] Plan modules
- [ ] Dark mode works correctly
- [ ] Hover states are preserved
- [ ] Focus states are preserved
- [ ] Shepherd.js tours still work

## Rollback Plan

If issues are found:
1. Restore from backup: `${TARGET_DIR}_backup_*`
2. Review specific problematic files
3. Apply migrations in smaller batches

## Documentation Updates

After successful migration:
- [ ] Update System Admin README
- [ ] Create visual comparison screenshots
- [ ] Document any custom color usage
- [ ] Update component style guide
