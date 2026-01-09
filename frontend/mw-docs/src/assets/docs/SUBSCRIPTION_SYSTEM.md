# Sistema de Assinaturas e Gerenciamento de Usuários

## Visão Geral

O PrimeCare Software implementa um sistema completo de assinaturas SaaS com gerenciamento de planos, permissões por usuário e controle de acesso a módulos.

## 1. Planos de Assinatura

### 1.1 Tipos de Plano

- **Trial (Teste)**: 15 dias gratuitos com recursos limitados
- **Basic (Básico)**: R$ 190/mês - 2 usuários, 100 pacientes
- **Standard (Médio)**: R$ 240/mês - 3 usuários, 300 pacientes
- **Premium**: R$ 320/mês - 5 usuários, pacientes ilimitados
- **Enterprise (Personalizado)**: Sob consulta

### 1.2 Recursos por Plano

| Recurso | Basic | Standard | Premium | Enterprise |
|---------|-------|----------|---------|------------|
| Usuários | 2 | 3 | 5 | Ilimitado |
| Pacientes | 100 | 300 | Ilimitados | Ilimitados |
| Relatórios | ❌ | ✅ | ✅ | ✅ |
| WhatsApp | ❌ | ✅ | ✅ | ✅ |
| SMS | ❌ | ❌ | ✅ | ✅ |
| TISS Export | ❌ | ❌ | ✅ | ✅ |

## 2. Gestão de Assinaturas

### 2.1 Estados da Assinatura

```
Trial → Active → PaymentOverdue → Suspended/Cancelled
              ↓
           Frozen (1 mês)
```

- **Trial**: Período de teste gratuito
- **Active**: Assinatura ativa e paga
- **PaymentOverdue**: Pagamento em atraso
- **Frozen**: Congelada temporariamente (1 mês)
- **Suspended**: Suspensa por falta de pagamento
- **Cancelled**: Cancelada

### 2.2 Upgrade de Plano

Quando um cliente faz upgrade:
1. O sistema calcula a diferença de valor entre os planos
2. **Cobra imediatamente a diferença proporcional**
3. Aplica o novo plano após confirmação do pagamento
4. Ajusta o valor da próxima cobrança

**Exemplo:**
```
Plano atual: Basic (R$ 190)
Novo plano: Premium (R$ 320)
Diferença: R$ 130

→ Cliente paga R$ 130 imediatamente
→ Plano é atualizado
→ Próxima cobrança: R$ 320 no vencimento
```

### 2.3 Downgrade de Plano

Quando um cliente faz downgrade:
1. O sistema agenda a mudança
2. **Mudança só é aplicada no próximo ciclo de cobrança**
3. Cliente continua com plano atual até o vencimento
4. Nenhum reembolso é aplicado

**Exemplo:**
```
Plano atual: Premium (R$ 320)
Novo plano: Basic (R$ 190)

→ Downgrade agendado para próximo vencimento
→ Cliente continua com Premium até lá
→ Próxima cobrança: R$ 190
```

### 2.4 Congelamento de Plano

- Duração: **1 mês fixo**
- **Suspende cobrança e acesso ao sistema**
- Prorroga a data do próximo pagamento em 1 mês
- Pode ser descongelado antes do término

**API Endpoints:**
```bash
# Congelar assinatura
POST /api/subscriptions/freeze

# Descongelar assinatura
POST /api/subscriptions/unfreeze
```

## 3. Validação de Pagamentos

### 3.1 Verificação Automática

O sistema verifica diariamente:
- Pagamentos vencidos
- Trials próximos do término (3 dias antes)
- Downgrades pendentes para aplicar

### 3.2 Notificações de Atraso

Quando o pagamento está atrasado, o sistema **envia automaticamente**:

1. **SMS** para o telefone da clínica
2. **Email** para o email da clínica
3. **WhatsApp** para o número cadastrado

**Mensagem enviada:**
```
Prezado(a) [Nome da Clínica],

Identificamos que o pagamento da sua assinatura está em atraso.

⚠️ ATENÇÃO: Seu acesso ao sistema PrimeCare Software 
ficará indisponível até a regularização do pagamento.

Valor: R$ XXX,XX
Data de vencimento: DD/MM/AAAA

Para regularizar, acesse: [link]

Após o pagamento, seu acesso será restabelecido 
automaticamente.

Dúvidas? Entre em contato conosco.

Atenciosamente,
Equipe PrimeCare Software
```

### 3.3 Bloqueio de Acesso

Quando o pagamento não é realizado:
1. Status muda para `PaymentOverdue`
2. **Acesso ao sistema é bloqueado**
3. Notificações são enviadas
4. Após pagamento, acesso é **restaurado automaticamente**

## 4. Sistema de Usuários e Permissões

### 4.1 Tipos de Usuário (Roles)

1. **SystemAdmin**: Administrador do sistema completo
2. **ClinicOwner**: Dono da clínica
3. **Doctor**: Médico
4. **Dentist**: Dentista
5. **Nurse**: Enfermeiro(a)
6. **Receptionist**: Recepcionista
7. **Secretary**: Secretária

### 4.2 Permissões por Role

#### SystemAdmin (Administrador do Sistema)
- ✅ Acesso completo a todas as clínicas (cross-tenant)
- ✅ Gerenciar assinaturas e planos
- ✅ Analytics e BI global
- ✅ Acesso cross-tenant para auditoria
- ✅ Criar outros System Admins
- ✅ Ativar/Desativar clínicas
- ✅ Modificar preços e planos
- ✅ Acesso a todos os endpoints do sistema

**API Endpoints Exclusivos**:
```bash
GET /api/system-admin/clinics
GET /api/system-admin/clinics/{id}
PUT /api/system-admin/clinics/{id}/subscription
POST /api/system-admin/clinics/{id}/toggle-status
GET /api/system-admin/analytics
POST /api/system-admin/users
GET /api/system-admin/plans
```

**Documentação Completa**: Ver [SYSTEM_ADMIN_DOCUMENTATION.md](SYSTEM_ADMIN_DOCUMENTATION.md)

#### ClinicOwner (Dono da Clínica)
- ✅ Gerenciar usuários da clínica
- ✅ Configurações da clínica
- ✅ Gerenciar assinatura
- ✅ Acesso a todos os módulos
- ✅ Relatórios financeiros

#### Doctor / Dentist
- ✅ Visualizar pacientes
- ✅ Gerenciar pacientes
- ✅ Visualizar agendamentos
- ✅ Gerenciar agendamentos
- ✅ Visualizar prontuários
- ✅ Gerenciar prontuários

#### Nurse (Enfermeiro)
- ✅ Visualizar pacientes
- ✅ Visualizar agendamentos
- ✅ Visualizar prontuários
- ✅ Gerenciar prontuários (limitado)

#### Receptionist (Recepcionista)
- ✅ Visualizar pacientes
- ✅ Gerenciar pacientes
- ✅ Visualizar agendamentos
- ✅ Gerenciar agendamentos

#### Secretary (Secretária)
- ✅ Visualizar pacientes
- ✅ Gerenciar pacientes
- ✅ Visualizar agendamentos
- ✅ Gerenciar agendamentos
- ✅ Gerenciar pagamentos

### 4.3 Limite de Usuários por Plano

O sistema **valida automaticamente** ao criar usuários:

```csharp
// Exemplo de validação
var currentUserCount = await _userRepository
    .GetUserCountByClinicIdAsync(clinicId, tenantId);

if (currentUserCount >= plan.MaxUsers)
{
    return BadRequest(new { 
        message = $"User limit reached. Current plan allows 
                    {plan.MaxUsers} users. Please upgrade your plan." 
    });
}
```

## 5. Sistema de Módulos

### 5.1 Módulos Disponíveis

1. **PatientManagement**: Gestão de pacientes
2. **AppointmentScheduling**: Agendamento de consultas
3. **MedicalRecords**: Prontuários médicos
4. **Prescriptions**: Prescrições
5. **FinancialManagement**: Gestão financeira
6. **Reports**: Relatórios (depende do plano)
7. **WhatsAppIntegration**: Integração WhatsApp (depende do plano)
8. **SMSNotifications**: Notificações SMS (depende do plano)
9. **TissExport**: Exportação TISS (depende do plano)
10. **InventoryManagement**: Gestão de estoque
11. **UserManagement**: Gestão de usuários

### 5.2 Configuração de Módulos

Cada clínica pode:
- Habilitar/desabilitar módulos disponíveis no plano
- Configurar parâmetros específicos de cada módulo
- Visualizar quais módulos estão disponíveis no plano atual

**API Endpoints:**

```bash
# Listar módulos e status
GET /api/moduleconfig

# Habilitar módulo
POST /api/moduleconfig/{moduleName}/enable

# Desabilitar módulo
POST /api/moduleconfig/{moduleName}/disable

# Atualizar configuração
PUT /api/moduleconfig/{moduleName}/config
```

**Resposta de GET /api/moduleconfig:**
```json
[
  {
    "moduleName": "Reports",
    "isEnabled": true,
    "isAvailableInPlan": true,
    "configuration": null
  },
  {
    "moduleName": "SMSNotifications",
    "isEnabled": false,
    "isAvailableInPlan": false,
    "configuration": null
  }
]
```

## 6. API Endpoints - Assinaturas

### 6.1 Consultar Assinatura Atual

```bash
GET /api/subscriptions/current
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Resposta:**
```json
{
  "id": "guid",
  "clinicId": "guid",
  "planName": "Premium",
  "status": "Active",
  "currentPrice": 320.00,
  "startDate": "2024-01-01T00:00:00Z",
  "nextPaymentDate": "2024-02-01T00:00:00Z",
  "trialEndDate": null,
  "isFrozen": false,
  "hasPendingChange": false,
  "canAccess": true
}
```

### 6.2 Fazer Upgrade

```bash
POST /api/subscriptions/upgrade
Authorization: Bearer {token}
Content-Type: application/json

{
  "newPlanId": "guid"
}
```

### 6.3 Fazer Downgrade

```bash
POST /api/subscriptions/downgrade
Authorization: Bearer {token}
Content-Type: application/json

{
  "newPlanId": "guid"
}
```

### 6.4 Congelar/Descongelar

```bash
# Congelar
POST /api/subscriptions/freeze

# Descongelar
POST /api/subscriptions/unfreeze
```

## 7. API Endpoints - Usuários

### 7.1 Listar Usuários

```bash
GET /api/users
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

### 7.2 Criar Usuário

```bash
POST /api/users
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "drsmith",
  "email": "smith@clinic.com",
  "password": "SecurePass123!",
  "fullName": "Dr. John Smith",
  "phone": "+5511999999999",
  "role": "Doctor",
  "professionalId": "CRM 12345",
  "specialty": "Cardiologia"
}
```

**Validações:**
- ✅ Username único no tenant
- ✅ Email válido
- ✅ Senha forte (mínimo 8 caracteres)
- ✅ Limite de usuários do plano

### 7.3 Atualizar Usuário

```bash
PUT /api/users/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "newemail@clinic.com",
  "fullName": "Dr. John Smith Jr.",
  "phone": "+5511999999999",
  "professionalId": "CRM 12345",
  "specialty": "Cardiologia"
}
```

### 7.4 Alterar Role

```bash
PUT /api/users/{id}/role
Authorization: Bearer {token}
Content-Type: application/json

{
  "newRole": "ClinicOwner"
}
```

### 7.5 Ativar/Desativar

```bash
# Ativar
POST /api/users/{id}/activate

# Desativar
POST /api/users/{id}/deactivate
```

## 8. Cadastro de Clínica (Onboarding)

### 8.1 Fluxo de Registro

1. Cliente acessa site MW.Site
2. Escolhe plano de assinatura
3. Preenche dados da clínica e do administrador
4. Sistema cria:
   - Registro da clínica
   - Primeiro usuário (ClinicOwner)
   - Assinatura no plano escolhido
   - Trial de 15 dias (se selecionado)

### 8.2 Endpoint de Registro

```bash
POST /api/registration
Content-Type: application/json

{
  "clinicName": "Clínica Sorriso",
  "clinicCNPJ": "12.345.678/0001-90",
  "clinicPhone": "+5511999999999",
  "clinicEmail": "contato@clinica.com",
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Sala 10",
  "neighborhood": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567",
  "username": "admin",
  "password": "SecurePass123!",
  "ownerName": "Dr. João Silva",
  "ownerEmail": "joao@clinica.com",
  "ownerPhone": "+5511988888888",
  "planId": "guid",
  "useTrial": true,
  "acceptTerms": true
}
```

## 9. Banco de Dados

### 9.1 Tabelas Criadas

#### SubscriptionPlans
```sql
CREATE TABLE SubscriptionPlans (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(500),
    MonthlyPrice decimal(18,2) NOT NULL,
    TrialDays int NOT NULL,
    MaxUsers int NOT NULL,
    MaxPatients int NOT NULL,
    HasReports bit NOT NULL,
    HasWhatsAppIntegration bit NOT NULL,
    HasSMSNotifications bit NOT NULL,
    HasTissExport bit NOT NULL,
    IsActive bit NOT NULL,
    Type int NOT NULL,
    TenantId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL
);
```

#### Users
```sql
CREATE TABLE Users (
    Id uniqueidentifier PRIMARY KEY,
    Username nvarchar(100) NOT NULL UNIQUE,
    Email nvarchar(200) NOT NULL,
    PasswordHash nvarchar(500) NOT NULL,
    FullName nvarchar(200) NOT NULL,
    Phone nvarchar(20) NOT NULL,
    ClinicId uniqueidentifier,
    Role int NOT NULL,
    IsActive bit NOT NULL,
    LastLoginAt datetime2,
    ProfessionalId nvarchar(50),
    Specialty nvarchar(100),
    TenantId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(Id)
);
```

#### ClinicSubscriptions
```sql
CREATE TABLE ClinicSubscriptions (
    Id uniqueidentifier PRIMARY KEY,
    ClinicId uniqueidentifier NOT NULL,
    SubscriptionPlanId uniqueidentifier NOT NULL,
    StartDate datetime2 NOT NULL,
    EndDate datetime2,
    TrialEndDate datetime2,
    Status int NOT NULL,
    LastPaymentDate datetime2,
    NextPaymentDate datetime2,
    CurrentPrice decimal(18,2) NOT NULL,
    CancellationReason nvarchar(500),
    CancellationDate datetime2,
    IsFrozen bit NOT NULL,
    FrozenStartDate datetime2,
    FrozenEndDate datetime2,
    PendingPlanId uniqueidentifier,
    PendingPlanPrice decimal(18,2),
    PlanChangeDate datetime2,
    IsUpgrade bit NOT NULL,
    TenantId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(Id),
    FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id),
    FOREIGN KEY (PendingPlanId) REFERENCES SubscriptionPlans(Id)
);
```

#### ModuleConfigurations
```sql
CREATE TABLE ModuleConfigurations (
    Id uniqueidentifier PRIMARY KEY,
    ClinicId uniqueidentifier NOT NULL,
    ModuleName nvarchar(100) NOT NULL,
    IsEnabled bit NOT NULL,
    Configuration nvarchar(2000),
    TenantId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(Id),
    UNIQUE (ClinicId, ModuleName)
);
```

## 10. Testes

### 10.1 Cenários de Teste

1. **Upgrade de Plano**
   - Verificar cálculo de diferença
   - Validar cobrança imediata
   - Confirmar aplicação do novo plano

2. **Downgrade de Plano**
   - Verificar agendamento para próximo ciclo
   - Validar que não há cobrança imediata
   - Confirmar aplicação na data correta

3. **Congelamento**
   - Verificar bloqueio de acesso
   - Validar suspensão de cobrança
   - Confirmar prorrogação de vencimento

4. **Notificações de Atraso**
   - Simular pagamento vencido
   - Verificar envio de SMS, Email e WhatsApp
   - Confirmar bloqueio de acesso

5. **Limites de Usuários**
   - Criar usuários até o limite do plano
   - Verificar erro ao exceder limite
   - Validar após upgrade de plano

## 11. Segurança

### 11.1 Isolamento Multi-tenant

Todas as queries incluem filtro de `TenantId`:
```csharp
modelBuilder.Entity<User>()
    .HasQueryFilter(u => EF.Property<string>(u, "TenantId") == GetTenantId());
```

### 11.2 Autenticação

- JWT tokens com claims de tenant e role
- Password hashing com BCrypt (work factor 12)
- Validação de força de senha

### 11.3 Autorização

Verificação de permissões em cada endpoint:
```csharp
if (!user.HasPermission(Permission.ManageUsers))
{
    return Forbid();
}
```

## 12. Próximos Passos

1. ✅ Implementar webhook de pagamento
2. ✅ Criar dashboard de analytics de assinaturas
3. ✅ Adicionar testes de integração
4. ✅ Implementar sistema de cupons/descontos
5. ✅ Criar relatório de churn

---

**Documentação atualizada em:** Outubro 2024
**Versão:** 2.0
