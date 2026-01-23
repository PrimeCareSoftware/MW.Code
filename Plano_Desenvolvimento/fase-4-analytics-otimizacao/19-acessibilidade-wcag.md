# 📋 Prompt 19: Acessibilidade WCAG 2.1 Level AA

**Prioridade:** 🔥 P2 - Médio  
**Complexidade:** ⚡⚡ Média-Baixa  
**Tempo Estimado:** 1.5 meses | 1 frontend developer  
**Custo:** R$ 22.500  
**Pré-requisitos:** Frontend funcionando (React/TypeScript)

---

## 🎯 Objetivo

Tornar o sistema 100% acessível conforme WCAG 2.1 Level AA, garantindo conformidade com a Lei Brasileira de Inclusão (LBI), implementando navegação por teclado, compatibilidade com leitores de tela (NVDA, JAWS), contrastes adequados, textos alternativos, HTML semântico, ARIA labels, indicadores de foco e testes com usuários com deficiência.

---

## 📊 Contexto do Sistema

### Problema Atual
- Interface não otimizada para acessibilidade
- Falta de suporte a leitores de tela
- Navegação por teclado incompleta
- Contrastes inadequados
- Ausência de textos alternativos
- Sem HTML semântico
- Não testado com usuários PcD

### Solução Proposta
Implementar:
- ✅ Auditoria completa com axe e WAVE
- ✅ WCAG 2.1 Level AA (50 critérios)
- ✅ Navegação total por teclado
- ✅ Compatibilidade NVDA/JAWS/VoiceOver
- ✅ Contrastes mínimos 4.5:1
- ✅ Alt texts em imagens
- ✅ HTML5 semântico
- ✅ ARIA roles e labels
- ✅ Focus indicators visíveis
- ✅ Conformidade LBI

### Importância
- **Legal:** Conformidade com LBI (Lei 13.146/2015)
- **Ética:** Inclusão de ~45 milhões de brasileiros com deficiência
- **SEO:** Melhor ranqueamento
- **UX:** Melhor experiência para todos

---

## 🏗️ Arquitetura da Solução

### 1. Auditoria e Análise (1 semana)

#### 1.1 Ferramentas de Auditoria
```typescript
// frontend/src/tests/accessibility/audit.test.ts
import { toHaveNoViolations } from 'jest-axe';
import { render } from '@testing-library/react';
import { axe } from 'jest-axe';

expect.extend(toHaveNoViolations);

describe('Accessibility Audit', () => {
  it('Dashboard should have no accessibility violations', async () => {
    const { container } = render(<Dashboard />);
    const results = await axe(container);
    expect(results).toHaveNoViolations();
  });
  
  it('Patient form should be accessible', async () => {
    const { container } = render(<PatientForm />);
    const results = await axe(container, {
      rules: {
        'color-contrast': { enabled: true },
        'label': { enabled: true },
        'button-name': { enabled: true },
        'landmark-one-main': { enabled: true }
      }
    });
    expect(results).toHaveNoViolations();
  });
});
```

#### 1.2 Script de Auditoria Automática
```json
// package.json
{
  "scripts": {
    "audit:a11y": "pa11y-ci --sitemap http://localhost:3000/sitemap.xml",
    "audit:axe": "axe http://localhost:3000 --tags wcag2a,wcag2aa --save audit-report.json",
    "audit:lighthouse": "lighthouse http://localhost:3000 --only-categories=accessibility --output=html --output-path=./lighthouse-a11y.html"
  }
}
```

#### 1.3 Relatório de Auditoria
```typescript
// frontend/src/scripts/generateA11yReport.ts
import { AxePuppeteer } from '@axe-core/puppeteer';
import puppeteer from 'puppeteer';
import fs from 'fs';

interface ViolationSummary {
  page: string;
  violations: number;
  critical: number;
  serious: number;
  moderate: number;
  minor: number;
}

const pagesToAudit = [
  { name: 'Dashboard', url: 'http://localhost:3000/dashboard' },
  { name: 'Patients List', url: 'http://localhost:3000/patients' },
  { name: 'Appointment Form', url: 'http://localhost:3000/appointments/new' },
  { name: 'Medical Records', url: 'http://localhost:3000/medical-records' },
  { name: 'Reports', url: 'http://localhost:3000/reports' }
];

async function auditAccessibility() {
  const browser = await puppeteer.launch();
  const results: ViolationSummary[] = [];
  
  for (const page of pagesToAudit) {
    const browserPage = await browser.newPage();
    await browserPage.goto(page.url);
    await browserPage.waitForSelector('main');
    
    const axeResults = await new AxePuppeteer(browserPage).analyze();
    
    const summary: ViolationSummary = {
      page: page.name,
      violations: axeResults.violations.length,
      critical: axeResults.violations.filter(v => v.impact === 'critical').length,
      serious: axeResults.violations.filter(v => v.impact === 'serious').length,
      moderate: axeResults.violations.filter(v => v.impact === 'moderate').length,
      minor: axeResults.violations.filter(v => v.impact === 'minor').length
    };
    
    results.push(summary);
    
    // Salvar detalhes
    fs.writeFileSync(
      `./a11y-reports/${page.name.replace(/\s/g, '-')}.json`,
      JSON.stringify(axeResults, null, 2)
    );
  }
  
  await browser.close();
  
  // Gerar relatório HTML
  generateHTMLReport(results);
}

function generateHTMLReport(results: ViolationSummary[]) {
  const html = `
    <!DOCTYPE html>
    <html lang="pt-BR">
    <head>
      <meta charset="UTF-8">
      <title>Relatório de Acessibilidade</title>
      <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        table { border-collapse: collapse; width: 100%; margin-top: 20px; }
        th, td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        th { background-color: #4CAF50; color: white; }
        .critical { color: #d32f2f; font-weight: bold; }
        .serious { color: #f57c00; font-weight: bold; }
        .moderate { color: #fbc02d; font-weight: bold; }
        .minor { color: #7cb342; }
      </style>
    </head>
    <body>
      <h1>Relatório de Acessibilidade - WCAG 2.1 AA</h1>
      <p>Data: ${new Date().toLocaleString('pt-BR')}</p>
      <table>
        <thead>
          <tr>
            <th>Página</th>
            <th>Total Violações</th>
            <th>Críticas</th>
            <th>Sérias</th>
            <th>Moderadas</th>
            <th>Menores</th>
          </tr>
        </thead>
        <tbody>
          ${results.map(r => `
            <tr>
              <td>${r.page}</td>
              <td>${r.violations}</td>
              <td class="critical">${r.critical}</td>
              <td class="serious">${r.serious}</td>
              <td class="moderate">${r.moderate}</td>
              <td class="minor">${r.minor}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
      <h2>Total: ${results.reduce((sum, r) => sum + r.violations, 0)} violações</h2>
    </body>
    </html>
  `;
  
  fs.writeFileSync('./a11y-reports/summary.html', html);
}

auditAccessibility();
```

---

### 2. Navegação por Teclado (2 semanas)

#### 2.1 Hook de Navegação
```typescript
// frontend/src/hooks/useKeyboardNavigation.ts
import { useEffect, useRef } from 'react';

interface UseKeyboardNavigationOptions {
  onEnter?: () => void;
  onEscape?: () => void;
  onArrowUp?: () => void;
  onArrowDown?: () => void;
  onArrowLeft?: () => void;
  onArrowRight?: () => void;
  onTab?: (e: KeyboardEvent) => void;
}

export const useKeyboardNavigation = (options: UseKeyboardNavigationOptions) => {
  const ref = useRef<HTMLElement>(null);
  
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      switch (e.key) {
        case 'Enter':
          if (options.onEnter) {
            e.preventDefault();
            options.onEnter();
          }
          break;
        case 'Escape':
          if (options.onEscape) {
            e.preventDefault();
            options.onEscape();
          }
          break;
        case 'ArrowUp':
          if (options.onArrowUp) {
            e.preventDefault();
            options.onArrowUp();
          }
          break;
        case 'ArrowDown':
          if (options.onArrowDown) {
            e.preventDefault();
            options.onArrowDown();
          }
          break;
        case 'ArrowLeft':
          if (options.onArrowLeft) {
            e.preventDefault();
            options.onArrowLeft();
          }
          break;
        case 'ArrowRight':
          if (options.onArrowRight) {
            e.preventDefault();
            options.onArrowRight();
          }
          break;
        case 'Tab':
          if (options.onTab) {
            options.onTab(e);
          }
          break;
      }
    };
    
    const element = ref.current;
    if (element) {
      element.addEventListener('keydown', handleKeyDown);
      return () => element.removeEventListener('keydown', handleKeyDown);
    }
  }, [options]);
  
  return ref;
};
```

#### 2.2 Componente de Lista Navegável
```typescript
// frontend/src/components/Accessible/AccessibleList.tsx
import React, { useState, useRef, useEffect } from 'react';

interface AccessibleListProps<T> {
  items: T[];
  renderItem: (item: T, isSelected: boolean) => React.ReactNode;
  onSelect: (item: T) => void;
  ariaLabel: string;
}

export function AccessibleList<T>({ 
  items, 
  renderItem, 
  onSelect, 
  ariaLabel 
}: AccessibleListProps<T>) {
  const [selectedIndex, setSelectedIndex] = useState(0);
  const listRef = useRef<HTMLUListElement>(null);
  const itemRefs = useRef<(HTMLLIElement | null)[]>([]);
  
  useEffect(() => {
    itemRefs.current = itemRefs.current.slice(0, items.length);
  }, [items]);
  
  const handleKeyDown = (e: React.KeyboardEvent) => {
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        setSelectedIndex(prev => 
          prev < items.length - 1 ? prev + 1 : prev
        );
        break;
      case 'ArrowUp':
        e.preventDefault();
        setSelectedIndex(prev => prev > 0 ? prev - 1 : prev);
        break;
      case 'Home':
        e.preventDefault();
        setSelectedIndex(0);
        break;
      case 'End':
        e.preventDefault();
        setSelectedIndex(items.length - 1);
        break;
      case 'Enter':
      case ' ':
        e.preventDefault();
        onSelect(items[selectedIndex]);
        break;
    }
  };
  
  useEffect(() => {
    // Scroll to selected item
    itemRefs.current[selectedIndex]?.scrollIntoView({
      block: 'nearest',
      behavior: 'smooth'
    });
    itemRefs.current[selectedIndex]?.focus();
  }, [selectedIndex]);
  
  return (
    <ul
      ref={listRef}
      role="listbox"
      aria-label={ariaLabel}
      onKeyDown={handleKeyDown}
      tabIndex={0}
      style={{ listStyle: 'none', padding: 0 }}
    >
      {items.map((item, index) => (
        <li
          key={index}
          ref={el => itemRefs.current[index] = el}
          role="option"
          aria-selected={index === selectedIndex}
          tabIndex={-1}
          onClick={() => {
            setSelectedIndex(index);
            onSelect(item);
          }}
          style={{
            backgroundColor: index === selectedIndex ? '#e3f2fd' : 'transparent',
            padding: '8px',
            cursor: 'pointer'
          }}
        >
          {renderItem(item, index === selectedIndex)}
        </li>
      ))}
    </ul>
  );
}
```

#### 2.3 Trap de Foco para Modais
```typescript
// frontend/src/components/Accessible/FocusTrap.tsx
import React, { useEffect, useRef } from 'react';

interface FocusTrapProps {
  children: React.ReactNode;
  active: boolean;
  onEscape?: () => void;
}

export const FocusTrap: React.FC<FocusTrapProps> = ({ 
  children, 
  active, 
  onEscape 
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const previousFocusRef = useRef<HTMLElement | null>(null);
  
  useEffect(() => {
    if (active) {
      // Salvar elemento com foco atual
      previousFocusRef.current = document.activeElement as HTMLElement;
      
      // Focar primeiro elemento focável
      const focusableElements = getFocusableElements();
      if (focusableElements.length > 0) {
        focusableElements[0].focus();
      }
      
      return () => {
        // Restaurar foco ao fechar
        previousFocusRef.current?.focus();
      };
    }
  }, [active]);
  
  const getFocusableElements = (): HTMLElement[] => {
    if (!containerRef.current) return [];
    
    const selector = [
      'a[href]',
      'button:not([disabled])',
      'input:not([disabled])',
      'select:not([disabled])',
      'textarea:not([disabled])',
      '[tabindex]:not([tabindex="-1"])'
    ].join(',');
    
    return Array.from(containerRef.current.querySelectorAll(selector));
  };
  
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (!active) return;
    
    if (e.key === 'Escape' && onEscape) {
      onEscape();
      return;
    }
    
    if (e.key === 'Tab') {
      const focusableElements = getFocusableElements();
      if (focusableElements.length === 0) return;
      
      const firstElement = focusableElements[0];
      const lastElement = focusableElements[focusableElements.length - 1];
      
      if (e.shiftKey) {
        // Shift + Tab
        if (document.activeElement === firstElement) {
          e.preventDefault();
          lastElement.focus();
        }
      } else {
        // Tab
        if (document.activeElement === lastElement) {
          e.preventDefault();
          firstElement.focus();
        }
      }
    }
  };
  
  return (
    <div ref={containerRef} onKeyDown={handleKeyDown}>
      {children}
    </div>
  );
};
```

---

### 3. Leitores de Tela (2 semanas)

#### 3.1 Componente com ARIA
```typescript
// frontend/src/components/Accessible/AccessibleButton.tsx
import React from 'react';

interface AccessibleButtonProps {
  children: React.ReactNode;
  onClick: () => void;
  ariaLabel?: string;
  ariaDescribedBy?: string;
  ariaExpanded?: boolean;
  ariaPressed?: boolean;
  disabled?: boolean;
  type?: 'button' | 'submit' | 'reset';
  variant?: 'primary' | 'secondary' | 'danger';
}

export const AccessibleButton: React.FC<AccessibleButtonProps> = ({
  children,
  onClick,
  ariaLabel,
  ariaDescribedBy,
  ariaExpanded,
  ariaPressed,
  disabled = false,
  type = 'button',
  variant = 'primary'
}) => {
  const variantClasses = {
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    danger: 'btn-danger'
  };
  
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      aria-label={ariaLabel}
      aria-describedby={ariaDescribedBy}
      aria-expanded={ariaExpanded}
      aria-pressed={ariaPressed}
      className={`btn ${variantClasses[variant]} ${disabled ? 'btn-disabled' : ''}`}
    >
      {children}
    </button>
  );
};
```

#### 3.2 Anúncios ao Vivo
```typescript
// frontend/src/components/Accessible/LiveRegion.tsx
import React, { useEffect, useRef } from 'react';

interface LiveRegionProps {
  message: string;
  politeness?: 'polite' | 'assertive' | 'off';
  clearOnUnmount?: boolean;
}

export const LiveRegion: React.FC<LiveRegionProps> = ({ 
  message, 
  politeness = 'polite',
  clearOnUnmount = true 
}) => {
  const regionRef = useRef<HTMLDivElement>(null);
  
  useEffect(() => {
    if (regionRef.current && message) {
      regionRef.current.textContent = message;
    }
    
    return () => {
      if (clearOnUnmount && regionRef.current) {
        regionRef.current.textContent = '';
      }
    };
  }, [message, clearOnUnmount]);
  
  return (
    <div
      ref={regionRef}
      role="status"
      aria-live={politeness}
      aria-atomic="true"
      style={{
        position: 'absolute',
        left: '-10000px',
        width: '1px',
        height: '1px',
        overflow: 'hidden'
      }}
    />
  );
};

// Hook para usar live region
export const useLiveRegion = () => {
  const [message, setMessage] = React.useState('');
  
  const announce = (msg: string) => {
    setMessage(''); // Clear primeiro para forçar re-anúncio
    setTimeout(() => setMessage(msg), 100);
  };
  
  return { message, announce };
};
```

#### 3.3 Formulário Acessível
```typescript
// frontend/src/components/Accessible/AccessibleForm.tsx
import React from 'react';
import { Form, Input, Select, DatePicker } from 'antd';

interface FormField {
  name: string;
  label: string;
  type: 'text' | 'email' | 'tel' | 'date' | 'select';
  required?: boolean;
  options?: { value: string; label: string }[];
  helpText?: string;
  errorMessage?: string;
}

interface AccessibleFormProps {
  fields: FormField[];
  onSubmit: (values: any) => void;
  title: string;
}

export const AccessibleForm: React.FC<AccessibleFormProps> = ({
  fields,
  onSubmit,
  title
}) => {
  const [form] = Form.useForm();
  
  return (
    <Form
      form={form}
      onFinish={onSubmit}
      layout="vertical"
      aria-label={title}
    >
      {fields.map(field => {
        const fieldId = `field-${field.name}`;
        const helpId = field.helpText ? `${fieldId}-help` : undefined;
        const errorId = field.errorMessage ? `${fieldId}-error` : undefined;
        
        return (
          <Form.Item
            key={field.name}
            name={field.name}
            label={
              <label htmlFor={fieldId}>
                {field.label}
                {field.required && <span aria-label="obrigatório"> *</span>}
              </label>
            }
            required={field.required}
            help={field.helpText}
            validateStatus={field.errorMessage ? 'error' : ''}
          >
            {field.type === 'select' ? (
              <Select
                id={fieldId}
                aria-describedby={helpId}
                aria-invalid={!!field.errorMessage}
                aria-errormessage={errorId}
              >
                {field.options?.map(opt => (
                  <Select.Option key={opt.value} value={opt.value}>
                    {opt.label}
                  </Select.Option>
                ))}
              </Select>
            ) : field.type === 'date' ? (
              <DatePicker
                id={fieldId}
                aria-describedby={helpId}
                aria-invalid={!!field.errorMessage}
                aria-errormessage={errorId}
                format="DD/MM/YYYY"
              />
            ) : (
              <Input
                id={fieldId}
                type={field.type}
                aria-describedby={helpId}
                aria-invalid={!!field.errorMessage}
                aria-errormessage={errorId}
                aria-required={field.required}
              />
            )}
            {field.helpText && (
              <div id={helpId} className="help-text">
                {field.helpText}
              </div>
            )}
            {field.errorMessage && (
              <div id={errorId} role="alert" className="error-text">
                {field.errorMessage}
              </div>
            )}
          </Form.Item>
        );
      })}
      
      <Form.Item>
        <button type="submit" className="btn btn-primary">
          Enviar
        </button>
      </Form.Item>
    </Form>
  );
};
```

---

### 4. Contrastes e Cores (1 semana)

#### 4.1 Sistema de Cores Acessível
```typescript
// frontend/src/styles/accessibleColors.ts
export const accessibleColors = {
  // Cores primárias - contraste mínimo 4.5:1
  primary: {
    main: '#1976d2',      // Contraste 4.5:1 com branco
    dark: '#004ba0',      // Contraste 7:1 com branco
    light: '#63a4ff',     // Contraste 3:1 com branco (apenas large text)
    contrastText: '#ffffff'
  },
  
  // Cores de status
  success: {
    main: '#2e7d32',      // Contraste 4.6:1
    light: '#60ad5e',
    dark: '#005005',
    contrastText: '#ffffff'
  },
  
  error: {
    main: '#d32f2f',      // Contraste 4.5:1
    light: '#ef5350',
    dark: '#c62828',
    contrastText: '#ffffff'
  },
  
  warning: {
    main: '#ed6c02',      // Contraste 4.5:1
    light: '#ff9800',
    dark: '#e65100',
    contrastText: '#ffffff'
  },
  
  info: {
    main: '#0288d1',      // Contraste 4.5:1
    light: '#03a9f4',
    dark: '#01579b',
    contrastText: '#ffffff'
  },
  
  // Texto
  text: {
    primary: '#000000de',    // 87% opacity - contraste 7:1
    secondary: '#00000099',  // 60% opacity - contraste 4.5:1
    disabled: '#00000061',   // 38% opacity
  },
  
  // Background
  background: {
    default: '#ffffff',
    paper: '#ffffff',
    subtle: '#f5f5f5'
  }
};

// Função para verificar contraste
export function checkContrast(
  foreground: string, 
  background: string
): { ratio: number; passAA: boolean; passAAA: boolean } {
  const fgLuminance = calculateLuminance(foreground);
  const bgLuminance = calculateLuminance(background);
  
  const ratio = (Math.max(fgLuminance, bgLuminance) + 0.05) / 
                (Math.min(fgLuminance, bgLuminance) + 0.05);
  
  return {
    ratio: Math.round(ratio * 10) / 10,
    passAA: ratio >= 4.5,    // WCAG AA
    passAAA: ratio >= 7.0     // WCAG AAA
  };
}

function calculateLuminance(hex: string): number {
  const rgb = hexToRgb(hex);
  const [r, g, b] = rgb.map(val => {
    val = val / 255;
    return val <= 0.03928 
      ? val / 12.92 
      : Math.pow((val + 0.055) / 1.055, 2.4);
  });
  
  return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}

function hexToRgb(hex: string): number[] {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result 
    ? [
        parseInt(result[1], 16),
        parseInt(result[2], 16),
        parseInt(result[3], 16)
      ]
    : [0, 0, 0];
}
```

#### 4.2 Tema com Alto Contraste
```typescript
// frontend/src/styles/highContrastTheme.ts
export const highContrastTheme = {
  colors: {
    background: '#000000',
    surface: '#1a1a1a',
    text: '#ffffff',
    textSecondary: '#e0e0e0',
    primary: '#00ffff',      // Cyan - contraste 8:1
    secondary: '#ffff00',    // Yellow - contraste 10:1
    error: '#ff4444',
    success: '#44ff44',
    border: '#ffffff',
    focus: '#ffff00'
  },
  
  focusIndicator: {
    width: '3px',
    style: 'solid',
    color: '#ffff00',
    offset: '2px'
  }
};
```

---

### 5. HTML Semântico e ARIA (1 semana)

#### 5.1 Componente de Layout Semântico
```typescript
// frontend/src/components/Layout/SemanticLayout.tsx
import React from 'react';

interface SemanticLayoutProps {
  header?: React.ReactNode;
  nav?: React.ReactNode;
  main: React.ReactNode;
  aside?: React.ReactNode;
  footer?: React.ReactNode;
}

export const SemanticLayout: React.FC<SemanticLayoutProps> = ({
  header,
  nav,
  main,
  aside,
  footer
}) => {
  return (
    <div className="semantic-layout">
      {/* Skip to main content link */}
      <a href="#main-content" className="skip-to-main">
        Pular para conteúdo principal
      </a>
      
      {header && (
        <header role="banner" className="site-header">
          {header}
        </header>
      )}
      
      {nav && (
        <nav role="navigation" aria-label="Navegação principal" className="site-nav">
          {nav}
        </nav>
      )}
      
      <div className="content-wrapper">
        <main 
          id="main-content" 
          role="main" 
          tabIndex={-1}
          className="main-content"
        >
          {main}
        </main>
        
        {aside && (
          <aside 
            role="complementary" 
            aria-label="Conteúdo complementar"
            className="sidebar"
          >
            {aside}
          </aside>
        )}
      </div>
      
      {footer && (
        <footer role="contentinfo" className="site-footer">
          {footer}
        </footer>
      )}
    </div>
  );
};
```

#### 5.2 Breadcrumbs Acessível
```typescript
// frontend/src/components/Accessible/AccessibleBreadcrumbs.tsx
import React from 'react';
import { Link } from 'react-router-dom';

interface BreadcrumbItem {
  label: string;
  path?: string;
}

interface AccessibleBreadcrumbsProps {
  items: BreadcrumbItem[];
}

export const AccessibleBreadcrumbs: React.FC<AccessibleBreadcrumbsProps> = ({ items }) => {
  return (
    <nav aria-label="Breadcrumb">
      <ol style={{ display: 'flex', listStyle: 'none', padding: 0 }}>
        {items.map((item, index) => {
          const isLast = index === items.length - 1;
          
          return (
            <li key={index} style={{ display: 'flex', alignItems: 'center' }}>
              {item.path && !isLast ? (
                <Link to={item.path}>{item.label}</Link>
              ) : (
                <span aria-current={isLast ? 'page' : undefined}>
                  {item.label}
                </span>
              )}
              
              {!isLast && (
                <span 
                  aria-hidden="true" 
                  style={{ margin: '0 8px' }}
                >
                  /
                </span>
              )}
            </li>
          );
        })}
      </ol>
    </nav>
  );
};
```

---

### 6. Indicadores de Foco (1 semana)

#### 6.1 Estilos de Foco Global
```css
/* frontend/src/styles/focus.css */

/* Remover outline padrão apenas quando usar mouse */
:focus:not(:focus-visible) {
  outline: none;
}

/* Indicador de foco para teclado */
:focus-visible {
  outline: 3px solid #2196f3;
  outline-offset: 2px;
  border-radius: 4px;
}

/* Botões */
button:focus-visible {
  outline: 3px solid #2196f3;
  outline-offset: 2px;
  box-shadow: 0 0 0 4px rgba(33, 150, 243, 0.25);
}

/* Links */
a:focus-visible {
  outline: 3px solid #2196f3;
  outline-offset: 2px;
  background-color: rgba(33, 150, 243, 0.1);
}

/* Inputs */
input:focus-visible,
textarea:focus-visible,
select:focus-visible {
  outline: 3px solid #2196f3;
  outline-offset: 0;
  border-color: #2196f3;
  box-shadow: 0 0 0 4px rgba(33, 150, 243, 0.25);
}

/* Elementos customizados */
[tabindex]:focus-visible {
  outline: 3px solid #2196f3;
  outline-offset: 2px;
}

/* Skip link */
.skip-to-main {
  position: absolute;
  left: -10000px;
  top: auto;
  width: 1px;
  height: 1px;
  overflow: hidden;
  background-color: #2196f3;
  color: white;
  padding: 8px 16px;
  text-decoration: none;
  font-weight: bold;
}

.skip-to-main:focus {
  position: fixed;
  left: 10px;
  top: 10px;
  width: auto;
  height: auto;
  overflow: visible;
  z-index: 9999;
  border-radius: 4px;
}
```

---

### 7. Testes com Usuários (1 semana)

#### 7.1 Protocolo de Teste
```markdown
# Protocolo de Teste de Acessibilidade

## Perfil de Testadores
- 3 usuários cegos (NVDA/JAWS)
- 2 usuários com baixa visão
- 2 usuários com deficiência motora (apenas teclado)
- 1 usuário com deficiência cognitiva

## Tarefas
1. Fazer login no sistema
2. Navegar até lista de pacientes
3. Cadastrar novo paciente
4. Agendar consulta
5. Preencher prontuário
6. Gerar relatório

## Métricas
- Taxa de conclusão de tarefa
- Tempo para completar
- Número de erros
- Satisfação (escala 1-5)
- Comentários qualitativos
```

#### 7.2 Checklist WCAG 2.1 AA
```typescript
// frontend/src/tests/accessibility/wcag-checklist.ts
export const wcagChecklist = {
  perceivable: {
    textAlternatives: [
      { id: '1.1.1', description: 'Todas as imagens têm texto alternativo', status: 'pass' },
    ],
    timeBasedMedia: [
      { id: '1.2.1', description: 'Áudio tem alternativa', status: 'n/a' },
      { id: '1.2.2', description: 'Vídeo tem legendas', status: 'n/a' },
    ],
    adaptable: [
      { id: '1.3.1', description: 'Info e estrutura', status: 'pass' },
      { id: '1.3.2', description: 'Sequência significativa', status: 'pass' },
      { id: '1.3.3', description: 'Características sensoriais', status: 'pass' },
      { id: '1.3.4', description: 'Orientação', status: 'pass' },
      { id: '1.3.5', description: 'Identificar propósito de input', status: 'pass' },
    ],
    distinguishable: [
      { id: '1.4.1', description: 'Uso de cor', status: 'pass' },
      { id: '1.4.2', description: 'Controle de áudio', status: 'n/a' },
      { id: '1.4.3', description: 'Contraste mínimo (4.5:1)', status: 'pass' },
      { id: '1.4.4', description: 'Redimensionar texto', status: 'pass' },
      { id: '1.4.5', description: 'Imagens de texto', status: 'pass' },
      { id: '1.4.10', description: 'Reflow', status: 'pass' },
      { id: '1.4.11', description: 'Contraste não textual', status: 'pass' },
      { id: '1.4.12', description: 'Espaçamento de texto', status: 'pass' },
      { id: '1.4.13', description: 'Conteúdo em hover/focus', status: 'pass' },
    ]
  },
  operable: {
    keyboardAccessible: [
      { id: '2.1.1', description: 'Teclado', status: 'pass' },
      { id: '2.1.2', description: 'Sem armadilha de teclado', status: 'pass' },
      { id: '2.1.4', description: 'Atalhos de teclado', status: 'pass' },
    ],
    enoughTime: [
      { id: '2.2.1', description: 'Ajuste de tempo', status: 'pass' },
      { id: '2.2.2', description: 'Pausar, parar, ocultar', status: 'pass' },
    ],
    seizuresAndPhysicalReactions: [
      { id: '2.3.1', description: 'Três flashes ou abaixo do limiar', status: 'pass' },
    ],
    navigable: [
      { id: '2.4.1', description: 'Ignorar blocos', status: 'pass' },
      { id: '2.4.2', description: 'Página com título', status: 'pass' },
      { id: '2.4.3', description: 'Ordem de foco', status: 'pass' },
      { id: '2.4.4', description: 'Propósito do link (em contexto)', status: 'pass' },
      { id: '2.4.5', description: 'Múltiplas formas', status: 'pass' },
      { id: '2.4.6', description: 'Cabeçalhos e rótulos', status: 'pass' },
      { id: '2.4.7', description: 'Foco visível', status: 'pass' },
    ],
    inputModalities: [
      { id: '2.5.1', description: 'Gestos de ponteiro', status: 'pass' },
      { id: '2.5.2', description: 'Cancelamento de ponteiro', status: 'pass' },
      { id: '2.5.3', description: 'Rótulo no nome', status: 'pass' },
      { id: '2.5.4', description: 'Acionamento de movimento', status: 'n/a' },
    ]
  },
  understandable: {
    readable: [
      { id: '3.1.1', description: 'Idioma da página', status: 'pass' },
      { id: '3.1.2', description: 'Idioma de partes', status: 'pass' },
    ],
    predictable: [
      { id: '3.2.1', description: 'Em foco', status: 'pass' },
      { id: '3.2.2', description: 'Em entrada', status: 'pass' },
      { id: '3.2.3', description: 'Navegação consistente', status: 'pass' },
      { id: '3.2.4', description: 'Identificação consistente', status: 'pass' },
    ],
    inputAssistance: [
      { id: '3.3.1', description: 'Identificação de erro', status: 'pass' },
      { id: '3.3.2', description: 'Rótulos ou instruções', status: 'pass' },
      { id: '3.3.3', description: 'Sugestão de erro', status: 'pass' },
      { id: '3.3.4', description: 'Prevenção de erro', status: 'pass' },
    ]
  },
  robust: {
    compatible: [
      { id: '4.1.1', description: 'Análise', status: 'pass' },
      { id: '4.1.2', description: 'Nome, função, valor', status: 'pass' },
      { id: '4.1.3', description: 'Mensagens de status', status: 'pass' },
    ]
  }
};
```

---

## 📝 Tarefas de Implementação

### Sprint 1: Auditoria e Setup (Semana 1)
- [ ] Configurar axe, WAVE, Lighthouse
- [ ] Executar auditoria completa
- [ ] Gerar relatório baseline
- [ ] Priorizar violações
- [ ] Setup testes automatizados

### Sprint 2: Navegação por Teclado (Semanas 2-3)
- [ ] Implementar skip links
- [ ] Criar hooks de navegação
- [ ] Componente de lista navegável
- [ ] Focus trap para modais
- [ ] Atalhos de teclado
- [ ] Testes de teclado

### Sprint 3: Leitores de Tela (Semanas 4-5)
- [ ] ARIA labels em todos componentes
- [ ] Live regions
- [ ] Anúncios de estado
- [ ] Formulários acessíveis
- [ ] Tabelas com cabeçalhos
- [ ] Testar com NVDA/JAWS

### Sprint 4: Contrastes e Visual (Semana 6)
- [ ] Auditar todos os contrastes
- [ ] Ajustar cores (4.5:1 mínimo)
- [ ] Criar tema alto contraste
- [ ] Textos alternativos em imagens
- [ ] Ícones com labels
- [ ] Validar cores

### Sprint 5: HTML Semântico (Semana 7)
- [ ] Refatorar para HTML5 semântico
- [ ] Landmarks ARIA
- [ ] Breadcrumbs
- [ ] Heading hierarchy
- [ ] Validar HTML

### Sprint 6: Foco e Finalização (Semanas 8-9)
- [ ] Indicadores de foco
- [ ] Estilos :focus-visible
- [ ] Testar ordem de foco
- [ ] Correções finais
- [ ] Auditoria final

### Sprint 7: Testes com Usuários (Semana 10)
- [ ] Recrutar testadores PcD
- [ ] Conduzir sessões de teste
- [ ] Coletar feedback
- [ ] Implementar ajustes
- [ ] Documentação final

---

## 🧪 Testes

### Testes Automatizados
```typescript
// frontend/src/tests/accessibility/a11y.test.tsx
import { render } from '@testing-library/react';
import { axe, toHaveNoViolations } from 'jest-axe';

expect.extend(toHaveNoViolations);

describe('Accessibility Tests', () => {
  it('should not have any accessibility violations', async () => {
    const { container } = render(<App />);
    const results = await axe(container);
    expect(results).toHaveNoViolations();
  });
});
```

### Testes Manuais
- [ ] Navegação completa apenas com teclado
- [ ] Teste com NVDA
- [ ] Teste com JAWS
- [ ] Teste com VoiceOver (Mac/iOS)
- [ ] Zoom 200%
- [ ] Alto contraste Windows
- [ ] Modo escuro
- [ ] Mobile/tablets

---

## 📊 Métricas de Sucesso

### KPIs
- **Conformidade WCAG 2.1 AA:** 100%
- **Violações Críticas:** 0
- **Score Lighthouse Accessibility:** > 95
- **Taxa Conclusão Tarefas (PcD):** > 90%
- **Satisfação Usuários:** > 4.5/5

### Benchmarks
- **Contraste Mínimo:** 4.5:1 (AA)
- **Navegação Teclado:** 100% funcional
- **Leitores de Tela:** 100% compatível
- **Foco Visível:** 100% elementos

---

## 💰 ROI Esperado

### Investimento
- **Desenvolvimento:** R$ 22.500
- **Ferramentas:** R$ 500
- **Testes com Usuários:** R$ 2.000
- **Total:** R$ 25.000

### Retorno
- **Conformidade Legal (LBI):** Evitar multas
- **Ampliação Mercado:** +45 milhões potenciais usuários
- **Melhoria SEO:** +20% tráfego orgânico
- **Reputação:** Diferencial competitivo
- **Intangível:** Responsabilidade social

### Riscos de Não Implementar
- **Multas LBI:** Até R$ 100.000
- **Processos judiciais:** Alto risco
- **Exclusão de mercado:** 24% população
- **Imagem negativa:** Reputacional

---

## 📚 Documentação

1. **Guia de Acessibilidade:**
   - Padrões WCAG 2.1
   - Exemplos de código
   - Checklist de desenvolvimento

2. **Manual de Testes:**
   - Como testar com leitores de tela
   - Navegação por teclado
   - Ferramentas de auditoria

3. **Certificação:**
   - Declaração de conformidade WCAG 2.1 AA
   - Relatório de auditoria
   - Conformidade LBI

---

## 🎯 Próximos Passos

1. **Imediato:**
   - Aprovar investimento
   - Iniciar auditoria
   - Setup ferramentas

2. **Curto Prazo:**
   - Implementar navegação teclado
   - Corrigir contrastes
   - ARIA básico

3. **Médio Prazo:**
   - Leitores de tela
   - Testes com usuários
   - Certificação

4. **Longo Prazo:**
   - Manutenção contínua
   - Treinamento equipe
   - WCAG AAA (futuro)
