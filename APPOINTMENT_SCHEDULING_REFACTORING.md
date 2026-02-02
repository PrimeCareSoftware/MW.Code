# Refatoração do Sistema de Agendamento - Documentação

## Visão Geral

Este documento descreve as melhorias implementadas no sistema de agendamento para permitir que o perfil de secretária possa:
- Visualizar todos os agendamentos dos médicos no calendário
- Escolher o médico ao agendar uma consulta
- Filtrar a visualização do calendário por médico para evitar conflitos de horários

## Alterações Implementadas

### Backend (C# / .NET)

#### 1. Atualização do Endpoint de Agenda Diária

**Arquivo**: `src/MedicSoft.Api/Controllers/AppointmentsController.cs`

- Adicionado parâmetro opcional `professionalId` aos endpoints:
  - `GET /api/appointments?date=<date>&clinicId=<clinicId>&professionalId=<professionalId>`
  - `GET /api/appointments/agenda?date=<date>&clinicId=<clinicId>&professionalId=<professionalId>`

**Exemplo de uso**:
```
GET /api/appointments/agenda?date=2026-02-02&clinicId=123&professionalId=456
```

#### 2. Novo Endpoint para Listar Profissionais

**Arquivo**: `src/MedicSoft.Api/Controllers/UsersController.cs`

- Novo endpoint: `GET /api/users/professionals`
- Retorna lista de médicos e dentistas associados à clínica do usuário autenticado
- Requer permissão `appointments.view` (secretárias têm essa permissão)

**Resposta**:
```json
[
  {
    "id": "guid",
    "fullName": "Dr. João Silva",
    "professionalId": "CRM 12345",
    "specialty": "Cardiologia",
    "role": "Doctor"
  }
]
```

#### 3. Atualização dos Serviços e Queries

**Arquivos Modificados**:
- `src/MedicSoft.Application/Services/AppointmentService.cs`
- `src/MedicSoft.Application/Queries/Appointments/GetDailyAgendaQuery.cs`
- `src/MedicSoft.Application/Handlers/Queries/Appointments/GetDailyAgendaQueryHandler.cs`

**Funcionalidade**:
- Adicionado filtro por `professionalId` na query de agenda diária
- Filtro aplicado antes de retornar os agendamentos

### Frontend (Angular)

#### 1. Atualização do Modelo de Appointment

**Arquivo**: `frontend/medicwarehouse-app/src/app/models/appointment.model.ts`

**Alterações**:
- Adicionado campo `professionalId` e `professionalName` ao modelo `Appointment`
- Adicionado `professionalId` aos DTOs `CreateAppointment` e `UpdateAppointment`
- Novo interface `Professional` para representar médicos/profissionais

```typescript
export interface Professional {
  id: string;
  fullName: string;
  professionalId?: string; // CRM, CRO, etc.
  specialty?: string;
  role: string;
}
```

#### 2. Atualização do Serviço de Appointments

**Arquivo**: `frontend/medicwarehouse-app/src/app/services/appointment.ts`

**Alterações**:
- Método `getDailyAgenda` atualizado para aceitar parâmetro opcional `professionalId`
- Novo método `getProfessionals()` para buscar lista de médicos

```typescript
getDailyAgenda(clinicId: string, date: string, professionalId?: string): Observable<DailyAgenda>
getProfessionals(): Observable<Professional[]>
```

#### 3. Calendário de Agendamentos

**Arquivos Modificados**:
- `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-calendar/appointment-calendar.ts`
- `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-calendar/appointment-calendar.html`
- `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-calendar/appointment-calendar.scss`

**Funcionalidades Adicionadas**:

1. **Filtro por Médico**:
   - Dropdown no topo do calendário para selecionar um médico
   - Opção "Todos os Médicos" para visualizar todos os agendamentos
   - Filtro aplicado via backend ao carregar agendamentos

2. **Indicador Visual do Médico**:
   - Nome do médico exibido em cada bloco de agendamento
   - Formato: "Dr(a). [Nome do Médico]"

3. **Carregamento de Profissionais**:
   - Lista de médicos carregada automaticamente ao abrir o calendário
   - Método `loadProfessionals()` busca médicos da clínica atual

**HTML do Filtro**:
```html
<div class="calendar-filters">
  <div class="filter-group">
    <label for="doctorFilter">Filtrar por Médico:</label>
    <select id="doctorFilter" class="form-select" (change)="onDoctorFilterChange($any($event.target).value || null)">
      <option value="">Todos os Médicos</option>
      @for (professional of professionals(); track professional.id) {
        <option [value]="professional.id">
          {{ professional.fullName }}
          @if (professional.specialty) {
            <span> - {{ professional.specialty }}</span>
          }
        </option>
      }
    </select>
  </div>
</div>
```

#### 4. Formulário de Agendamento

**Arquivos Modificados**:
- `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-form/appointment-form.ts`
- `frontend/medicwarehouse-app/src/app/pages/appointments/appointment-form/appointment-form.html`

**Funcionalidades Adicionadas**:

1. **Campo de Seleção de Médico**:
   - Dropdown para escolher o médico responsável pelo atendimento
   - Campo opcional (não é obrigatório selecionar um médico)
   - Lista de médicos carregada automaticamente

2. **Suporte em Criação e Edição**:
   - Médico pode ser selecionado ao criar novo agendamento
   - Médico pode ser alterado ao editar agendamento existente

**HTML do Campo**:
```html
<div class="form-group">
  <label for="professionalId">Médico</label>
  <select id="professionalId" formControlName="professionalId" class="form-control">
    <option value="">Selecione um médico (opcional)</option>
    @for (professional of professionals(); track professional.id) {
      <option [value]="professional.id">
        {{ professional.fullName }}
        @if (professional.specialty) {
          <span> - {{ professional.specialty }}</span>
        }
      </option>
    }
  </select>
  <small class="form-text">Selecione o médico responsável pelo atendimento</small>
</div>
```

## Permissões

As funcionalidades implementadas respeitam o sistema de permissões existente:

- **Secretárias** têm permissão `appointments.view` e podem:
  - Visualizar todos os agendamentos no calendário
  - Filtrar agendamentos por médico
  - Ver a lista de médicos disponíveis
  - Criar e editar agendamentos selecionando o médico

- **Endpoint de Profissionais** (`GET /api/users/professionals`):
  - Requer permissão `appointments.view`
  - Retorna apenas médicos e dentistas da clínica do usuário

## Fluxo de Uso

### Para Visualizar Agendamentos:

1. Secretária acessa o calendário de agendamentos
2. Sistema carrega automaticamente todos os agendamentos da clínica
3. Secretária pode filtrar por médico específico usando o dropdown
4. Calendário é atualizado mostrando apenas agendamentos do médico selecionado

### Para Criar Agendamento:

1. Secretária clica em "Novo Agendamento" ou em horário vazio no calendário
2. Preenche dados do paciente, data, horário e duração
3. **Seleciona o médico responsável** (novo campo)
4. Adiciona observações se necessário
5. Salva o agendamento

### Para Editar Agendamento:

1. Secretária clica em um agendamento existente
2. Pode alterar data, horário, duração e **médico responsável**
3. Salva as alterações

## Prevenção de Conflitos

Com a implementação do filtro por médico:

1. Secretária pode visualizar agenda específica de cada médico
2. Ao criar agendamento, vê apenas horários ocupados do médico selecionado
3. Evita agendamento de múltiplos pacientes no mesmo horário para o mesmo médico
4. Permite agendamentos simultâneos para médicos diferentes

## Compatibilidade

### Retrocompatibilidade:

- **Backend**: Parâmetro `professionalId` é opcional, endpoints continuam funcionando sem ele
- **Frontend**: Campos de médico são opcionais, sistema funciona com agendamentos sem médico atribuído
- **Dados Existentes**: Agendamentos antigos sem médico atribuído são exibidos normalmente

### Migração de Dados:

Não é necessária migração de dados. O campo `ProfessionalId` já existia no modelo `Appointment` e nos DTOs.

## Testes

### Backend:

```bash
cd /home/runner/work/MW.Code/MW.Code
dotnet build src/MedicSoft.Api/MedicSoft.Api.csproj
```

Status: ✅ Compilado com sucesso (apenas warnings existentes)

### Frontend:

```bash
cd /home/runner/work/MW.Code/MW.Code/frontend/medicwarehouse-app
npm install
npm run build
```

## Melhorias Futuras

1. **Validação de Conflitos no Backend**:
   - Adicionar validação que impede criar agendamentos conflitantes para o mesmo médico
   - Retornar mensagem de erro clara quando houver conflito

2. **Indicador Visual de Disponibilidade**:
   - Mostrar no calendário quais horários estão disponíveis por médico
   - Cores diferentes para cada médico

3. **Relatório de Agendamentos por Médico**:
   - Estatísticas de agendamentos por médico
   - Tempo médio de atendimento por médico

4. **Notificações**:
   - Notificar médico quando um agendamento for criado/alterado
   - Lembrete automático para médico e paciente

## Suporte

Para questões ou problemas relacionados a essas alterações, entre em contato com a equipe de desenvolvimento.
