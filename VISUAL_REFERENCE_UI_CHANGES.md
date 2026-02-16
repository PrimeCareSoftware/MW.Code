# Visual Reference - UI Changes

## Overview
This document provides a textual description of the UI changes for the clinic hours configuration and business configuration restrictions.

## 1. Clinic Hours Configuration Screen (Already Implemented)

### Location
**Path**: Configurações > Configuração do Negócio

### Screen Structure

```
┌────────────────────────────────────────────────────────────────┐
│  Navbar                                                         │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Configuração do Negócio                                       │
│  Configure o tipo de negócio, especialidade e recursos        │
│  disponíveis                                                   │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Tipo de Negócio                                               │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐          │
│  │ Profissional │ │ Clínica      │ │ Clínica      │          │
│  │ Autônomo     │ │ Pequena      │ │ Média        │ ...      │
│  └──────────────┘ └──────────────┘ └──────────────┘          │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Especialidade Principal                                       │
│  🩺 Médico  🧠 Psicólogo  🥗 Nutricionista  💪 Fisioterapeuta │
│  🦷 Dentista  💉 Enfermeiro  🎨 Terapeuta  🗣️ Fonoaudiólogo   │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  ⭐ Horário de Atendimento                                     │
│  Configure os horários de funcionamento da clínica e          │
│  duração das consultas.                                        │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Horário de Abertura    Horário de Fechamento             │ │
│  │ [08:00  ▼]             [18:00  ▼]                        │ │
│  │                                                            │ │
│  │ Duração da Consulta                                       │ │
│  │ [30 minutos ▼]                                            │ │
│  │ Options: 15, 30, 45, 60 minutos                          │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                 │
│  ☑ Permitir horários de emergência                            │
│  Permite agendamentos fora do horário normal para emergências │
│                                                                 │
│  ☑ Agendamento online habilitado                              │
│  Permite que pacientes agendem consultas pelo portal          │
│                                                                 │
│  ┌──────────────────────────────────────────────┐            │
│  │  Salvar Configurações de Horário              │            │
│  └──────────────────────────────────────────────┘            │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Recursos Clínicos                                             │
│  ☑ Prescrição Eletrônica    ☐ Integração com Laboratórios    │
│  ☑ Controle de Vacinas      ☐ Gestão de Estoque              │
└────────────────────────────────────────────────────────────────┘

... (more feature sections)
```

### Key Elements

1. **Horário de Abertura** (Opening Time)
   - Type: Time picker input
   - Default: 08:00
   - Format: HH:mm
   - Validation: Must be before closing time

2. **Horário de Fechamento** (Closing Time)
   - Type: Time picker input
   - Default: 18:00
   - Format: HH:mm
   - Validation: Must be after opening time

3. **Duração da Consulta** (Appointment Duration)
   - Type: Dropdown select
   - Options: 15, 30, 45, 60 minutos
   - Default: 30 minutos

4. **Permitir horários de emergência** (Allow Emergency Slots)
   - Type: Checkbox
   - Default: Checked
   - Description: "Permite agendamentos fora do horário normal para emergências"

5. **Agendamento online habilitado** (Enable Online Scheduling)
   - Type: Checkbox
   - Default: Checked
   - Description: "Permite que pacientes agendem consultas pelo portal"

6. **Salvar Configurações de Horário** (Save Schedule Settings)
   - Type: Primary button
   - Action: Saves all schedule settings
   - States:
     - Normal: "Salvar Configurações de Horário"
     - Saving: "Salvando..."
     - Success: Green success message appears

### User Flow

1. User navigates to Configurações > Configuração do Negócio
2. User scrolls to "Horário de Atendimento" section
3. User modifies desired fields:
   - Changes opening time using time picker
   - Changes closing time using time picker
   - Selects appointment duration from dropdown
   - Toggles checkboxes as needed
4. User clicks "Salvar Configurações de Horário" button
5. System validates input (opening < closing)
6. System saves to database
7. Success message appears: "Configurações de horário atualizadas com sucesso!"
8. Message auto-dismisses after 3 seconds

## 2. System Admin Business Configuration Management

### Location
**Path**: System Admin Panel > Clinics > [Select Clinic] > Business Configuration

### Screen Structure - System Owner View

```
┌────────────────────────────────────────────────────────────────┐
│  System Admin Navbar                                           │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  ← Voltar                                                      │
│                                                                 │
│  Configuração de Negócio                                       │
│  Gerencie as configurações e funcionalidades da clínica       │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  ⚠️ Esta clínica ainda não possui uma configuração de negócio │
│                                                                 │
│  ┌──────────────────────────────────────────────┐            │
│  │  Criar Configuração Padrão                    │            │
│  └──────────────────────────────────────────────┘            │
└────────────────────────────────────────────────────────────────┘
```

### Screen Structure - Non-System-Owner View (NEW BEHAVIOR)

```
┌────────────────────────────────────────────────────────────────┐
│  System Admin Navbar                                           │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  ← Voltar                                                      │
│                                                                 │
│  Configuração de Negócio                                       │
│  Gerencie as configurações e funcionalidades da clínica       │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  ⚠️ Esta clínica ainda não possui uma configuração de negócio │
│                                                                 │
│  ⚠️ Apenas proprietários do sistema podem criar               │
│     configurações de negócio.                                  │
└────────────────────────────────────────────────────────────────┘
```

### Key Differences

#### Before (All System Admins)
- ✅ "Criar Configuração Padrão" button visible to all SystemAdmin users
- ⚠️ Could lead to unauthorized configuration changes

#### After (System Owners Only)
- ✅ "Criar Configuração Padrão" button visible ONLY to users with `isSystemOwner: true`
- ✅ Warning message shown to non-system-owner admins
- ✅ Backend enforces restriction with 403 Forbidden if API called directly

### Button States

**For System Owners:**
1. Normal State: "Criar Configuração Padrão"
2. Saving State: "Criando..." (button disabled)
3. Success State: Success message appears, configuration form shown

**For Non-System-Owners:**
- Button is completely hidden
- Warning text is shown instead
- If user attempts API call directly, receives 403 Forbidden

## 3. After Configuration Created

### Screen Structure (Same for All Admins with Config)

```
┌────────────────────────────────────────────────────────────────┐
│  ← Voltar                                                      │
│                                                                 │
│  Configuração de Negócio                                       │
│  Gerencie as configurações e funcionalidades da clínica       │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Informações Básicas                                           │
│                                                                 │
│  Tipo de Negócio                    Especialidade Principal   │
│  [Clínica Pequena ▼]               [Médico ▼]                │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│  Funcionalidades Clínicas                                      │
│  ☑ Prescrição Eletrônica                                      │
│  ☑ Integração com Laboratório                                 │
│  ☑ Controle de Vacinas                                        │
│  ☐ Gestão de Estoque                                          │
└────────────────────────────────────────────────────────────────┘

... (more sections)

┌────────────────────────────────────────────────────────────────┐
│  Criado em: 16/02/2026 13:45                                  │
│  Atualizado em: 16/02/2026 13:45                              │
└────────────────────────────────────────────────────────────────┘
```

## 4. Error Scenarios

### Invalid Time Range (Clinic Hours)

```
┌────────────────────────────────────────────────────────────────┐
│  ❌ Horário de abertura deve ser antes do horário de          │
│     fechamento                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Unauthorized Access (Non-System-Owner)

```
┌────────────────────────────────────────────────────────────────┐
│  ❌ Acesso negado. Permissões insuficientes para acessar      │
│     este recurso.                                              │
└────────────────────────────────────────────────────────────────┘
```

### API Response: 403 Forbidden
```json
{
  "message": "Access denied. Insufficient permissions to access this resource."
}
```

## 5. Success Messages

### Clinic Hours Updated

```
┌────────────────────────────────────────────────────────────────┐
│  ✓ Configurações de horário atualizadas com sucesso!          │
└────────────────────────────────────────────────────────────────┘
```
*Auto-dismisses after 3 seconds*

### Business Configuration Created

```
┌────────────────────────────────────────────────────────────────┐
│  ✓ Configuração criada com sucesso! Você pode personalizá-la  │
│    abaixo.                                                      │
└────────────────────────────────────────────────────────────────┘
```
*Auto-dismisses after 5 seconds*

## 6. Responsive Behavior

All screens are responsive and adapt to different viewport sizes:
- Mobile: Stacked layout, full-width inputs
- Tablet: 2-column grid for form fields
- Desktop: Multi-column grid with optimal spacing

## Color Scheme

- Success messages: Green background (#10b981)
- Error messages: Red background (#ef4444)
- Warning messages: Orange background (#f59e0b)
- Primary buttons: Blue (#3b82f6)
- Secondary buttons: Gray (#6b7280)

## Accessibility

- All form inputs have proper labels
- Color is not the only indicator (icons used)
- Keyboard navigation supported
- Screen reader friendly ARIA labels
- Focus indicators on interactive elements

## Notes for Manual Testing

When testing the UI:

1. **Clinic Hours**: Verify time pickers work correctly across browsers
2. **System Owner Check**: Test with both system owner and regular admin accounts
3. **Success Messages**: Confirm auto-dismiss timing (3s for schedule, 5s for config)
4. **Error Messages**: Verify validation messages appear correctly
5. **Responsive Design**: Test on mobile, tablet, and desktop viewports
