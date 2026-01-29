# 📋 Referência de Permissões - PrimeCare System Admin

**Versão:** 1.0  
**Atualizado:** Janeiro 2026  
**Fase:** 6 - Segurança e Compliance

---

## 📑 Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Permissões](#estrutura-de-permissões)
3. [Permissões por Recurso](#permissões-por-recurso)
4. [Roles Pré-Definidos](#roles-pré-definidos)
5. [Matriz de Permissões](#matriz-de-permissões)
6. [Uso em Código](#uso-em-código)

---

## 🎯 Visão Geral

O sistema PrimeCare utiliza um sistema de permissões granulares baseado em recursos e ações. Cada permissão segue o formato:

```
recurso.ação
```

Exemplos:
- `clinic.view` - Visualizar clínica
- `users.create` - Criar usuários
- `patients.manage` - Gerenciar pacientes (todas as ações)

---

## 🏗️ Estrutura de Permissões

### Formato

```
[recurso].[ação]
```

### Recursos Disponíveis

| Recurso | Descrição |
|---------|-----------|
| `clinic` | Clínicas e suas configurações |
| `users` | Usuários do sistema |
| `profiles` | Perfis de acesso |
| `patients` | Pacientes |
| `appointments` | Consultas e agendamentos |
| `medical-records` | Prontuários médicos |
| `procedures` | Procedimentos |
| `payments` | Pagamentos e cobranças |
| `invoices` | Faturas e notas fiscais |
| `expenses` | Despesas |
| `reports` | Relatórios e analytics |
| `medications` | Medicamentos |
| `prescriptions` | Prescrições |
| `exams` | Exames |
| `notifications` | Notificações |
| `waiting-queue` | Fila de espera |
| `attendance` | Atendimento |
| `data` | Operações LGPD |

### Ações Disponíveis

| Ação | Descrição | HTTP Equivalente |
|------|-----------|------------------|
| `view` | Visualizar/Ler | GET |
| `create` | Criar novo | POST |
| `edit` | Editar existente | PUT/PATCH |
| `delete` | Excluir | DELETE |
| `export` | Exportar dados | GET (download) |
| `manage` | Todas as ações | ALL |

---

## 📦 Permissões por Recurso

### Clínica (`clinic`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `clinic.view` | Visualizar informações da clínica | Ver dashboard, dados básicos |
| `clinic.edit` | Editar configurações da clínica | Alterar nome, endereço, etc |
| `clinic.manage` | Gerenciar tudo relacionado à clínica | Todas as operações |

**Endpoints:**
```
GET    /api/clinics/{id}          → clinic.view
PUT    /api/clinics/{id}          → clinic.edit
DELETE /api/clinics/{id}          → clinic.manage
```

---

### Usuários (`users`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `users.view` | Visualizar lista de usuários | Listar colaboradores |
| `users.create` | Criar novos usuários | Adicionar médico, recepcionista |
| `users.edit` | Editar usuários existentes | Alterar cargo, permissões |
| `users.delete` | Desativar/excluir usuários | Remover acesso |
| `users.manage` | Gerenciar usuários | Todas as operações |

**Endpoints:**
```
GET    /api/users                 → users.view
POST   /api/users                 → users.create
PUT    /api/users/{id}            → users.edit
DELETE /api/users/{id}            → users.delete
POST   /api/users/{id}/reset-pwd  → users.manage
```

---

### Perfis de Acesso (`profiles`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `profiles.view` | Visualizar perfis de acesso | Listar perfis criados |
| `profiles.create` | Criar novos perfis | Criar perfil "Enfermeiro" |
| `profiles.edit` | Editar perfis existentes | Adicionar/remover permissões |
| `profiles.delete` | Excluir perfis | Remover perfil não usado |
| `profiles.manage` | Gerenciar perfis | Todas as operações |

---

### Pacientes (`patients`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `patients.view` | Visualizar pacientes | Lista, busca, detalhes |
| `patients.create` | Cadastrar novos pacientes | Novo cadastro |
| `patients.edit` | Editar dados de pacientes | Atualizar telefone, endereço |
| `patients.delete` | Excluir pacientes | Exclusão lógica |
| `patients.manage` | Gerenciar pacientes | Todas as operações |

**Endpoints:**
```
GET    /api/patients              → patients.view
GET    /api/patients/{id}         → patients.view
POST   /api/patients              → patients.create
PUT    /api/patients/{id}         → patients.edit
DELETE /api/patients/{id}         → patients.delete
```

---

### Consultas (`appointments`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `appointments.view` | Visualizar agenda | Ver consultas agendadas |
| `appointments.create` | Criar agendamentos | Marcar consulta |
| `appointments.edit` | Editar agendamentos | Remarcar, alterar status |
| `appointments.delete` | Cancelar agendamentos | Cancelar consulta |
| `appointments.manage` | Gerenciar agenda | Todas as operações |

---

### Prontuários (`medical-records`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `medical-records.view` | Visualizar prontuários | Ler histórico médico |
| `medical-records.create` | Criar novos registros | Nova consulta, evolução |
| `medical-records.edit` | Editar registros | Corrigir informações |
| `medical-records.manage` | Gerenciar prontuários | Todas as operações |

**Nota:** Exclusão de prontuários não é permitida por lei (CFM 1821/2007).

---

### Atendimento (`attendance`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `attendance.view` | Visualizar atendimentos | Ver lista de atendimentos |
| `attendance.perform` | Realizar atendimentos | Atender paciente, criar SOAP |

---

### Procedimentos (`procedures`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `procedures.view` | Visualizar procedimentos | Ver tabela de procedimentos |
| `procedures.create` | Cadastrar procedimentos | Novo procedimento |
| `procedures.edit` | Editar procedimentos | Alterar preço, descrição |
| `procedures.delete` | Excluir procedimentos | Remover não usado |

---

### Pagamentos (`payments`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `payments.view` | Visualizar pagamentos | Ver histórico financeiro |
| `payments.manage` | Gerenciar pagamentos | Criar, editar, estornar |

---

### Faturas (`invoices`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `invoices.view` | Visualizar faturas | Ver lista de NFSe |
| `invoices.manage` | Gerenciar faturas | Emitir, cancelar |

---

### Despesas (`expenses`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `expenses.view` | Visualizar despesas | Ver gastos |
| `expenses.create` | Lançar despesas | Nova despesa |
| `expenses.edit` | Editar despesas | Corrigir valores |
| `expenses.delete` | Excluir despesas | Remover lançamento |

---

### Relatórios (`reports`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `reports.financial` | Relatórios financeiros | Ver faturamento, despesas |
| `reports.operational` | Relatórios operacionais | Produtividade, atendimentos |

---

### Medicamentos e Prescrições

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `medications.view` | Visualizar medicamentos | Ver banco de medicamentos |
| `prescriptions.create` | Criar prescrições | Receitar medicamentos |

---

### Exames

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `exams.view` | Visualizar exames | Ver resultado de exames |
| `exams.request` | Solicitar exames | Pedir novos exames |

---

### Notificações

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `notifications.view` | Visualizar notificações | Ver alertas |
| `notifications.manage` | Gerenciar notificações | Criar, enviar |

---

### Fila de Espera

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `waiting-queue.view` | Visualizar fila | Ver quem está esperando |
| `waiting-queue.manage` | Gerenciar fila | Chamar próximo, priorizar |

---

### Dados / LGPD (`data`)

| Permissão | Descrição | Exemplo de Uso |
|-----------|-----------|----------------|
| `data.export` | Exportar dados | Direito de acesso (Art. 18 LGPD) |
| `data.delete` | Anonimizar dados | Direito de exclusão (Art. 18 LGPD) |

**⚠️ Crítico:** Apenas administradores devem ter essas permissões.

---

## 👥 Roles Pré-Definidos

### SystemAdmin

**Descrição:** Acesso completo ao sistema  
**Permissões:** TODAS

```json
{
  "role": "SystemAdmin",
  "permissions": ["*"]
}
```

---

### ClinicOwner

**Descrição:** Dono/administrador da clínica

**Permissões:**
- `clinic.manage`
- `users.manage`
- `profiles.manage`
- `patients.manage`
- `appointments.manage`
- `medical-records.manage`
- `procedures.manage`
- `payments.manage`
- `invoices.manage`
- `expenses.manage`
- `reports.financial`
- `reports.operational`
- `data.export`

---

### Doctor / Dentist

**Descrição:** Profissional de saúde

**Permissões:**
- `clinic.view`
- `patients.view`
- `patients.create`
- `patients.edit`
- `appointments.view`
- `appointments.create`
- `appointments.edit`
- `medical-records.view`
- `medical-records.create`
- `medical-records.edit`
- `attendance.view`
- `attendance.perform`
- `medications.view`
- `prescriptions.create`
- `exams.view`
- `exams.request`
- `procedures.view`

---

### Nurse

**Descrição:** Enfermeiro(a)

**Permissões:**
- `clinic.view`
- `patients.view`
- `appointments.view`
- `medical-records.view`
- `medical-records.create`
- `medical-records.edit`
- `attendance.view`
- `medications.view`
- `exams.view`
- `waiting-queue.view`
- `waiting-queue.manage`

---

### Receptionist

**Descrição:** Recepcionista

**Permissões:**
- `clinic.view`
- `patients.view`
- `patients.create`
- `patients.edit`
- `appointments.view`
- `appointments.create`
- `appointments.edit`
- `appointments.delete`
- `waiting-queue.view`
- `waiting-queue.manage`
- `notifications.view`

---

### Secretary

**Descrição:** Secretário(a) administrativo

**Permissões:**
- `clinic.view`
- `patients.view`
- `patients.create`
- `patients.edit`
- `appointments.view`
- `appointments.create`
- `appointments.edit`
- `payments.view`
- `payments.manage`
- `invoices.view`
- `invoices.manage`
- `expenses.view`
- `expenses.create`
- `procedures.view`
- `reports.financial`

---

## 📊 Matriz de Permissões

| Recurso | SystemAdmin | ClinicOwner | Doctor | Nurse | Receptionist | Secretary |
|---------|-------------|-------------|--------|-------|--------------|-----------|
| **clinic** | ✅ Manage | ✅ Manage | 👁️ View | 👁️ View | 👁️ View | 👁️ View |
| **users** | ✅ Manage | ✅ Manage | ❌ | ❌ | ❌ | ❌ |
| **profiles** | ✅ Manage | ✅ Manage | ❌ | ❌ | ❌ | ❌ |
| **patients** | ✅ Manage | ✅ Manage | ✏️ Edit | 👁️ View | ✏️ Edit | ✏️ Edit |
| **appointments** | ✅ Manage | ✅ Manage | ✏️ Edit | 👁️ View | ✏️ Edit | ✏️ Edit |
| **medical-records** | ✅ Manage | ✅ Manage | ✏️ Edit | ✏️ Edit | ❌ | ❌ |
| **attendance** | ✅ Manage | ✅ Manage | ✅ Perform | 👁️ View | ❌ | ❌ |
| **procedures** | ✅ Manage | ✅ Manage | 👁️ View | ❌ | ❌ | 👁️ View |
| **payments** | ✅ Manage | ✅ Manage | ❌ | ❌ | ❌ | ✅ Manage |
| **invoices** | ✅ Manage | ✅ Manage | ❌ | ❌ | ❌ | ✅ Manage |
| **expenses** | ✅ Manage | ✅ Manage | ❌ | ❌ | ❌ | ✏️ Edit |
| **reports** | ✅ All | ✅ All | ❌ | ❌ | ❌ | 👁️ Financial |
| **data (LGPD)** | ✅ All | ✅ Export | ❌ | ❌ | ❌ | ❌ |

**Legenda:**
- ✅ Manage = Todas as operações
- ✏️ Edit = Criar e Editar
- 👁️ View = Apenas visualizar
- ❌ = Sem acesso

---

## 💻 Uso em Código

### Backend - C#

#### Verificar Permissão em Controller

```csharp
using MedicSoft.Application.Authorization;

[RequirePermission("patients.create")]
[HttpPost("patients")]
public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto dto)
{
    var patient = await _patientService.CreateAsync(dto);
    return Ok(patient);
}
```

#### Verificar Permissão Programaticamente

```csharp
var hasPermission = await _authorizationService.HasPermission(
    userId, 
    "medical-records.edit"
);

if (!hasPermission)
{
    return Forbid(); // 403 Forbidden
}
```

#### Obter Todas as Permissões do Usuário

```csharp
var permissions = await _authorizationService.GetUserPermissions(userId);

// Retorna lista: ["clinic.view", "patients.manage", ...]
```

---

### Frontend - TypeScript/Angular

#### Verificar Permissão no Template

```html
<button 
  *ngIf="hasPermission('patients.create')"
  (click)="createPatient()"
>
  Novo Paciente
</button>
```

#### Verificar Permissão no Component

```typescript
export class PatientListComponent {
  canCreatePatient: boolean;

  constructor(private authService: AuthService) {
    this.canCreatePatient = this.authService.hasPermission('patients.create');
  }

  createPatient() {
    if (!this.canCreatePatient) {
      this.toastr.error('Você não tem permissão para criar pacientes');
      return;
    }
    
    // Criar paciente...
  }
}
```

#### Route Guard

```typescript
@Injectable()
export class PermissionGuard implements CanActivate {
  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredPermission = route.data['permission'];
    
    if (!this.authService.hasPermission(requiredPermission)) {
      this.router.navigate(['/forbidden']);
      return false;
    }
    
    return true;
  }
}

// Uso nas rotas
{
  path: 'patients/new',
  component: PatientFormComponent,
  canActivate: [PermissionGuard],
  data: { permission: 'patients.create' }
}
```

---

## 🔒 Boas Práticas

### ✅ Faça

1. **Princípio do Menor Privilégio**
   - Dê apenas as permissões necessárias
   - Revise permissões periodicamente

2. **Use Perfis Pré-Definidos**
   - Reutilize roles padrão quando possível
   - Crie perfis customizados apenas quando necessário

3. **Documente Permissões Customizadas**
   - Explique por que foram criadas
   - Quem deve ter acesso

4. **Auditoria Regular**
   - Revise permissões trimestralmente
   - Remova acessos não utilizados

### ❌ Evite

1. **Não use wildcards desnecessariamente**
   ```
   ❌ users.*  (todas as permissões de usuários)
   ✅ users.view, users.create (apenas o necessário)
   ```

2. **Não dê `*.manage` sem necessidade**
   - `.manage` é muito poderoso
   - Use permissões específicas quando possível

3. **Não ignore erros 403 Forbidden**
   - Sempre trate adequadamente
   - Mostre mensagem clara ao usuário

---

## 📚 Referências

- [Documentação de Autorização](./SECURITY_BEST_PRACTICES_GUIDE.md)
- [LGPD Compliance](./LGPD_COMPLIANCE_GUIDE.md)
- [Audit Logs](./AUDIT_LOG_QUERY_GUIDE.md)

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Próxima revisão:** Julho 2026
