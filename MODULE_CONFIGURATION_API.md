# API de Configuração de Módulos

## Visão Geral

O sistema de configuração de módulos permite gerenciar quais funcionalidades estão disponíveis e habilitadas para cada clínica, com base no plano de assinatura contratado.

## Arquitetura

### Componentes Principais

1. **ModuleConfiguration**: Entidade que armazena a configuração de módulos por clínica
2. **ModuleConfigurationHistory**: Entidade que registra todas as mudanças de configuração
3. **SystemModules**: Classe estática com informações sobre todos os módulos disponíveis
4. **ModuleInfo**: Metadados detalhados de cada módulo

### Módulos Disponíveis

O sistema possui 13 módulos organizados por categoria:

#### Core (Básico)
- **PatientManagement**: Gestão de Pacientes
- **AppointmentScheduling**: Agendamento de Consultas
- **MedicalRecords**: Prontuário Eletrônico
- **Prescriptions**: Prescrições Médicas
- **FinancialManagement**: Gestão Financeira
- **UserManagement**: Gestão de Usuários

#### Advanced (Avançado)
- **WhatsAppIntegration**: Integração WhatsApp
- **SMSNotifications**: Notificações SMS
- **InventoryManagement**: Gestão de Estoque
- **WaitingQueue**: Fila de Espera

#### Premium
- **TissExport**: Exportação TISS
- **DoctorFieldsConfig**: Configuração de Campos

#### Analytics
- **Reports**: Relatórios Avançados

## Endpoints da API

### 1. Configuração por Clínica

#### GET /api/module-config
Lista todos os módulos e seu status para a clínica autenticada.

**Resposta:**
```json
[
  {
    "moduleName": "PatientManagement",
    "isEnabled": true,
    "isAvailableInPlan": true,
    "configuration": null
  }
]
```

#### GET /api/module-config/info
Retorna informações detalhadas sobre todos os módulos disponíveis.

**Resposta:**
```json
[
  {
    "name": "PatientManagement",
    "displayName": "Gestão de Pacientes",
    "description": "Cadastro, edição e consulta de pacientes",
    "category": "Core",
    "icon": "people",
    "isCore": true,
    "requiredModules": ["UserManagement"],
    "minimumPlan": "Basic"
  }
]
```

#### POST /api/module-config/{moduleName}/enable
Habilita um módulo para a clínica.

**Parâmetros:**
- `moduleName`: Nome do módulo a ser habilitado

**Resposta:**
```json
{
  "message": "Module PatientManagement enabled successfully"
}
```

**Erros:**
- `400`: Módulo não disponível no plano atual
- `400`: Módulos requeridos não estão habilitados

#### POST /api/module-config/{moduleName}/disable
Desabilita um módulo para a clínica.

**Parâmetros:**
- `moduleName`: Nome do módulo a ser desabilitado

**Resposta:**
```json
{
  "message": "Module Reports disabled successfully"
}
```

**Erros:**
- `400`: Módulos core não podem ser desabilitados

#### POST /api/module-config/{moduleName}/enable-with-reason
Habilita um módulo com motivo (para auditoria).

**Body:**
```json
{
  "reason": "Solicitação do gestor para relatórios avançados"
}
```

#### PUT /api/module-config/{moduleName}/config
Atualiza a configuração de um módulo.

**Body:**
```json
{
  "configuration": "{\"maxUsers\": 10, \"autoBackup\": true}"
}
```

#### POST /api/module-config/validate
Valida se um módulo pode ser habilitado.

**Body:**
```json
{
  "moduleName": "Reports"
}
```

**Resposta:**
```json
{
  "isValid": true,
  "errorMessage": ""
}
```

#### GET /api/module-config/{moduleName}/history
Retorna o histórico de mudanças de um módulo.

**Resposta:**
```json
[
  {
    "id": "guid",
    "moduleName": "Reports",
    "action": "Enabled",
    "changedBy": "user-id",
    "changedAt": "2026-01-29T20:00:00Z",
    "reason": "Upgrade para plano Premium",
    "previousConfiguration": null,
    "newConfiguration": null
  }
]
```

### 2. Administração Global (System Admin)

Todos os endpoints requerem role `SystemAdmin`.

#### GET /api/system-admin/modules/usage
Retorna estatísticas de uso global de módulos.

**Resposta:**
```json
[
  {
    "moduleName": "PatientManagement",
    "displayName": "Gestão de Pacientes",
    "totalClinics": 100,
    "clinicsWithModuleEnabled": 95,
    "adoptionRate": 95.0,
    "category": "Core"
  }
]
```

#### GET /api/system-admin/modules/adoption
Retorna taxa de adoção de módulos.

**Resposta:**
```json
[
  {
    "moduleName": "PatientManagement",
    "displayName": "Gestão de Pacientes",
    "adoptionRate": 95.0,
    "enabledCount": 95
  }
]
```

#### GET /api/system-admin/modules/usage-by-plan
Retorna uso de módulos agrupados por plano de assinatura.

**Resposta:**
```json
[
  {
    "planName": "Premium",
    "moduleName": "Reports",
    "clinicsCount": 25,
    "usagePercentage": 80.0
  }
]
```

#### GET /api/system-admin/modules/counts
Retorna contagem de clínicas usando cada módulo.

**Resposta:**
```json
{
  "PatientManagement": 95,
  "Reports": 45,
  "WhatsAppIntegration": 30
}
```

#### POST /api/system-admin/modules/{moduleName}/enable-globally
Habilita um módulo para todas as clínicas com plano apropriado.

**Resposta:**
```json
{
  "message": "Module Reports enabled globally"
}
```

#### POST /api/system-admin/modules/{moduleName}/disable-globally
Desabilita um módulo para todas as clínicas.

**Resposta:**
```json
{
  "message": "Module Reports disabled globally"
}
```

#### GET /api/system-admin/modules/{moduleName}/clinics
Lista todas as clínicas com um módulo específico habilitado.

**Resposta:**
```json
[
  {
    "clinicId": "guid",
    "clinicName": "Clínica ABC",
    "isEnabled": true,
    "configuration": null,
    "updatedAt": "2026-01-29T20:00:00Z"
  }
]
```

#### GET /api/system-admin/modules/{moduleName}/stats
Retorna estatísticas detalhadas de um módulo específico.

**Resposta:**
```json
{
  "moduleName": "Reports",
  "totalClinics": 100,
  "clinicsWithModuleEnabled": 45,
  "adoptionRate": 45.0
}
```

## Regras de Negócio

### 1. Módulos Core
- Não podem ser desabilitados
- Estão disponíveis em todos os planos
- Exemplos: PatientManagement, UserManagement

### 2. Módulos Requeridos
Alguns módulos dependem de outros:
- `AppointmentScheduling` requer `PatientManagement`
- `Prescriptions` requer `MedicalRecords`
- `WaitingQueue` requer `AppointmentScheduling`
- `DoctorFieldsConfig` requer `MedicalRecords`
- `WhatsAppIntegration` requer `PatientManagement`
- `SMSNotifications` requer `PatientManagement`
- `TissExport` requer `FinancialManagement`

### 3. Planos Mínimos
Cada módulo tem um plano mínimo requerido:
- **Basic**: Módulos core
- **Standard**: WhatsApp, SMS, Inventory, WaitingQueue, Reports
- **Premium**: TISS Export, DoctorFieldsConfig

### 4. Auditoria
Todas as mudanças de configuração são registradas em `ModuleConfigurationHistory`:
- Ação realizada (Enabled/Disabled/ConfigUpdated)
- Usuário que realizou a mudança
- Data e hora
- Motivo (opcional)
- Configurações anterior e nova (quando aplicável)

## Exemplos de Uso

### Habilitar Módulo de Relatórios

```http
POST /api/module-config/Reports/enable-with-reason
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Upgrade para plano Premium aprovado"
}
```

### Verificar Status dos Módulos

```http
GET /api/module-config
Authorization: Bearer {token}
```

### Administrador: Ver Uso Global

```http
GET /api/system-admin/modules/usage
Authorization: Bearer {admin-token}
```

### Administrador: Habilitar Módulo Globalmente

```http
POST /api/system-admin/modules/WhatsAppIntegration/enable-globally
Authorization: Bearer {admin-token}
```

## Códigos de Status HTTP

- `200 OK`: Operação realizada com sucesso
- `400 Bad Request`: Requisição inválida (módulo não existe, não disponível no plano, etc.)
- `401 Unauthorized`: Token de autenticação ausente ou inválido
- `403 Forbidden`: Usuário não tem permissão (ex: não é SystemAdmin)
- `404 Not Found`: Recurso não encontrado
- `500 Internal Server Error`: Erro interno do servidor

## Segurança

### Autenticação
Todos os endpoints requerem autenticação via JWT token.

### Autorização
- Endpoints `/api/module-config/*`: Qualquer usuário autenticado da clínica
- Endpoints `/api/system-admin/modules/*`: Requerem role `SystemAdmin`

### Validações
- Validação de módulo existe no sistema
- Validação de disponibilidade no plano
- Validação de módulos requeridos habilitados
- Validação de plano mínimo atendido
- Proteção contra desabilitação de módulos core

## Migração de Dados

### Tabela: ModuleConfigurationHistories

```sql
CREATE TABLE "ModuleConfigurationHistories" (
    "Id" uuid NOT NULL,
    "ModuleConfigurationId" uuid NOT NULL,
    "ClinicId" uuid NOT NULL,
    "ModuleName" character varying(100) NOT NULL,
    "Action" character varying(50) NOT NULL,
    "PreviousConfiguration" jsonb,
    "NewConfiguration" jsonb,
    "ChangedBy" character varying(100) NOT NULL,
    "ChangedAt" timestamp with time zone NOT NULL,
    "Reason" character varying(500),
    "TenantId" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ModuleConfigurationHistories" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_ModuleConfigurationHistories_ChangedAt" 
    ON "ModuleConfigurationHistories" ("ChangedAt");

CREATE INDEX "IX_ModuleConfigurationHistories_ClinicId_ModuleName" 
    ON "ModuleConfigurationHistories" ("ClinicId", "ModuleName");
```

### Coluna Adicionada: SubscriptionPlans.EnabledModules

```sql
ALTER TABLE "SubscriptionPlans" 
ADD COLUMN "EnabledModules" text;
```

## Próximos Passos

1. Implementar testes unitários e de integração
2. Adicionar cache para otimização de performance
3. Implementar notificações quando módulos são habilitados/desabilitados
4. Criar dashboard visual de uso de módulos
5. Adicionar suporte a configurações avançadas por módulo
