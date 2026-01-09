# Sistema de Gerenciamento de Proprietários e Permissões

## Visão Geral

Este documento descreve o sistema implementado para permitir que proprietários de clínicas gerenciem seus usuários, controlem permissões granulares e tenham acesso especial para casos como amigos médicos e ambientes de teste.

## Funcionalidades Implementadas

### 1. Override Manual de Assinatura

Permite ao SystemAdmin manter uma clínica ativa mesmo que:
- O pagamento da mensalidade esteja em atraso
- A clínica não tenha sido cadastrada pelo site
- A clínica esteja em período de teste

**Casos de Uso:**
- Oferecer acesso gratuito para amigos médicos
- Manter acesso para clínicas parceiras
- Facilitar testes e demonstrações

#### API Endpoints

**Ativar Override Manual:**
```http
POST /api/system-admin/clinics/{clinicId}/subscription/manual-override/enable
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Acesso gratuito para amigo médico"
}
```

**Desativar Override Manual:**
```http
POST /api/system-admin/clinics/{clinicId}/subscription/manual-override/disable
Authorization: Bearer {token}
```

**Resposta de Sucesso:**
```json
{
  "message": "Override manual ativado com sucesso",
  "reason": "Acesso gratuito para amigo médico",
  "setBy": "admin@medicwarehouse.com",
  "setAt": "2025-10-12T03:15:00Z"
}
```

### 2. Controle de Ambientes (Dev/Staging/Production)

O sistema agora diferencia entre ambientes de desenvolvimento/homologação e produção:

#### Desenvolvimento e Staging (Homologação)
- **Sem cobrança**: Todas as clínicas têm acesso livre
- **Testes ilimitados**: Crie quantas clínicas teste quiser
- **Sem bloqueio por pagamento**: Pagamentos em atraso não bloqueiam acesso

#### Produção
- **Cobrança ativa**: Regras normais de assinatura aplicadas
- **Bloqueio por inadimplência**: Pagamentos em atraso bloqueiam acesso
- **Override manual disponível**: SystemAdmin pode liberar acesso manualmente

#### Configuração

O ambiente é detectado automaticamente pela variável `ASPNETCORE_ENVIRONMENT`:

```bash
# Development
ASPNETCORE_ENVIRONMENT=Development

# Staging/Homologação
ASPNETCORE_ENVIRONMENT=Staging
# ou
ASPNETCORE_ENVIRONMENT=Homologacao

# Production
ASPNETCORE_ENVIRONMENT=Production
```

### 3. Sistema de Permissões Granulares

Implementado controle de acesso baseado em roles com permissões específicas para cada tipo de operação.

#### Roles e Permissões

##### SystemAdmin (Administrador do Sistema)
- ✅ Acesso completo ao sistema
- ✅ Gerenciar todas as clínicas (cross-tenant)
- ✅ Gerenciar assinaturas e planos
- ✅ Ativar/desativar override manual
- ✅ Criar outros administradores do sistema

##### ClinicOwner (Dono da Clínica)
- ✅ Gerenciar usuários da clínica
- ✅ Configurações da clínica
- ✅ Gerenciar assinatura
- ✅ Acesso a todos os módulos
- ✅ Relatórios financeiros
- ✅ Alterar roles de usuários
- ✅ Ativar/desativar usuários

##### Doctor / Dentist
- ✅ Visualizar e gerenciar pacientes
- ✅ Visualizar e gerenciar agendamentos
- ✅ Visualizar e gerenciar prontuários
- ✅ Criar e editar prescrições
- ✅ Gerenciar registros médicos

##### Nurse (Enfermeiro)
- ✅ Visualizar pacientes
- ✅ Visualizar agendamentos
- ✅ Visualizar prontuários
- ✅ Gerenciar prontuários (limitado)
- ❌ **NÃO pode criar prescrições**

##### Secretary (Secretária)
- ✅ Visualizar e gerenciar pacientes
- ✅ Visualizar e gerenciar agendamentos
- ✅ Gerenciar pagamentos
- ❌ **NÃO pode editar prontuários ou prescrições**
- ❌ **NÃO pode acessar registros médicos**

##### Receptionist (Recepcionista)
- ✅ Visualizar e gerenciar pacientes
- ✅ Visualizar e gerenciar agendamentos
- ❌ **NÃO pode acessar prontuários**
- ❌ **NÃO pode gerenciar pagamentos**

#### Aplicação de Permissões

Use o atributo `RequirePermission` para proteger endpoints:

```csharp
[HttpPost]
[RequirePermission(Permission.ManageMedicalRecords)]
public async Task<ActionResult> CreateMedicalRecord([FromBody] CreateDto dto)
{
    // Apenas Doctor, Dentist, Nurse e ClinicOwner podem acessar
    // Secretary NÃO tem acesso
}

[HttpPost]
[RequirePermission(Permission.ManageUsers)]
public async Task<ActionResult> CreateUser([FromBody] CreateUserDto dto)
{
    // Apenas ClinicOwner e SystemAdmin podem acessar
}
```

### 4. Melhorias no JWT Token

O token JWT agora inclui o `clinic_id`:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "medico@clinica.com",
  "tenantId": "clinica-abc",
  "role": "Doctor",
  "clinicId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "expiresAt": "2025-10-12T04:15:00Z"
}
```

Isso permite:
- Validação automática de acesso à clínica correta
- Melhor controle de multi-tenancy
- Auditoria mais precisa de ações

### 5. Endpoints do SystemAdmin

#### Listar Todas as Clínicas
```http
GET /api/system-admin/clinics?status=active&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Detalhes de uma Clínica
```http
GET /api/system-admin/clinics/{clinicId}
Authorization: Bearer {token}
```

#### Analytics do Sistema
```http
GET /api/system-admin/analytics
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "totalClinics": 150,
  "activeClinics": 145,
  "inactiveClinics": 5,
  "totalUsers": 450,
  "activeUsers": 425,
  "totalPatients": 12500,
  "monthlyRecurringRevenue": 48000.00,
  "subscriptionsByStatus": [
    { "status": "Active", "count": 140 },
    { "status": "Trial", "count": 5 },
    { "status": "PaymentOverdue", "count": 3 }
  ],
  "subscriptionsByPlan": [
    { "plan": "Básico", "count": 50 },
    { "plan": "Professional", "count": 70 },
    { "plan": "Premium", "count": 30 }
  ]
}
```

## Banco de Dados

### Novos Campos em ClinicSubscriptions

```sql
ALTER TABLE ClinicSubscriptions
ADD ManualOverrideActive bit NOT NULL DEFAULT 0,
    ManualOverrideReason nvarchar(500) NULL,
    ManualOverrideSetAt datetime2 NULL,
    ManualOverrideSetBy nvarchar(100) NULL;
```

### Migration

Execute a migration para adicionar os novos campos:

```bash
# A migration será aplicada automaticamente em:
dotnet ef database update

# Ou manualmente via SQL:
# /src/MedicSoft.Repository/Migrations/20251012195249_AddOwnerEntity.cs
```

## Exemplos de Uso

### Exemplo 1: ClinicOwner Gerenciando Usuários

```typescript
// Frontend - Owner Dashboard
async function createSecretary() {
  const response = await fetch('/api/users', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username: 'secretaria',
      email: 'secretaria@clinica.com',
      password: 'Senha@123',
      fullName: 'Maria Silva',
      phone: '11999999999',
      role: 'Secretary'
    })
  });
  
  // Secretária criada, mas NÃO poderá editar prontuários
}
```

### Exemplo 2: SystemAdmin Liberando Acesso para Amigo

```bash
# Ativar override manual
curl -X POST https://api.medicwarehouse.com/api/system-admin/clinics/abc123/subscription/manual-override/enable \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{"reason": "Acesso gratuito para Dr. João, amigo pessoal"}'
```

### Exemplo 3: Verificar Acesso em Diferentes Ambientes

```csharp
// Development - Sempre permite acesso
var canAccess = subscriptionService.CanAccessSystem(subscription, "Development");
// Retorna: true (mesmo com pagamento atrasado)

// Production - Verifica regras de negócio
var canAccess = subscriptionService.CanAccessSystem(subscription, "Production");
// Retorna: false (se pagamento atrasado e sem override)

// Production com Override - Permite acesso
subscription.EnableManualOverride("Amigo médico", "admin");
var canAccess = subscriptionService.CanAccessSystem(subscription, "Production");
// Retorna: true (override ativo)
```

## Testes

### Testes Implementados

Total: **692 testes** (todos passando ✅)

#### Novos Testes (23 adicionados):

**Manual Override (12 testes):**
- ✅ Ativar override com dados válidos
- ✅ Validar campos obrigatórios
- ✅ Desativar override
- ✅ Verificar acesso com override ativo
- ✅ Testar diferentes estados de assinatura

**Environment-Based Access (11 testes):**
- ✅ Ambiente Development sempre permite acesso
- ✅ Ambiente Staging sempre permite acesso
- ✅ Ambiente Production respeita regras de negócio
- ✅ Override manual funciona em Production
- ✅ Case-insensitive para nomes de ambiente

### Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas testes de override manual
dotnet test --filter "FullyQualifiedName~ManualOverride"

# Apenas testes de ambiente
dotnet test --filter "FullyQualifiedName~SubscriptionServiceEnvironment"
```

## Segurança

### Proteções Implementadas

1. **Autorização por Role**
   - SystemAdmin necessário para override manual
   - ClinicOwner necessário para gerenciar usuários
   - Permissões específicas para operações sensíveis

2. **Auditoria**
   - Registro de quem ativou override manual
   - Data/hora de ativação do override
   - Razão documentada para override

3. **Isolamento de Tenant**
   - Users só acessam sua própria clínica
   - SystemAdmin pode fazer cross-tenant quando necessário

4. **Validações**
   - Campos obrigatórios validados
   - Limites de usuários por plano respeitados
   - Estados de assinatura validados

## Próximos Passos

1. **Frontend (Opcional)**
   - Dashboard do owner para gerenciar usuários
   - Tela de administração system-wide
   - Interface para ativar/desativar overrides

2. **Notificações**
   - Email quando override é ativado
   - Alerta para ClinicOwner sobre mudanças de permissão

3. **Relatórios**
   - Relatório de clínicas com override ativo
   - Analytics de uso por role

## Suporte

Para dúvidas ou problemas:
- **Email**: contato@primecaresoftware.com
- **Documentação**: https://docs.medicwarehouse.com
- **Issues**: https://github.com/PrimeCare Software/MW.Code/issues

## Changelog

### v2.0.0 (2025-10-12)
- ✨ Adicionado sistema de override manual para assinaturas
- ✨ Implementado controle de ambientes (dev/staging sem cobrança)
- ✨ Sistema de permissões granulares com RequirePermissionAttribute
- ✨ clinic_id adicionado ao JWT token
- 🔒 Proteção de endpoints sensíveis (prontuários, prescrições)
- ✅ 23 novos testes adicionados (692 total)
- 📝 Documentação completa do sistema
