# Sistema de Gestão de Leads Autônomo - Documentação Técnica

## Visão Geral

Este documento descreve a implementação completa de um sistema de gestão de leads standalone, desenvolvido para substituir a integração com Salesforce implementada no PR 640. O sistema captura automaticamente leads abandonados do fluxo de cadastro e fornece ferramentas completas para gestão interna sem dependência de plataformas externas.

## Problema Resolvido

O PR 640 implementou ferramentas de Salesforce para captura e gestão de leads. No entanto, sem contrato com a plataforma Salesforce, era necessário criar uma solução autônoma que fornecesse funcionalidades similares para ajudar a obter melhores resultados com potenciais clientes.

## Solução Implementada

### Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Angular)                        │
│  - LeadsPage Component                                       │
│  - LeadService                                               │
│  - Lead Models (TypeScript)                                  │
└──────────────────┬──────────────────────────────────────────┘
                   │ HTTP REST API
┌──────────────────▼──────────────────────────────────────────┐
│                Backend (.NET 8 API)                          │
│  - LeadsController                                           │
│  - ILeadManagementService / LeadManagementService           │
│  - LeadCaptureHostedService (Background)                    │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│                    Domínio                                   │
│  - Lead (Entity)                                            │
│  - LeadActivity (Entity)                                    │
│  - SalesFunnelMetric (Entity existente)                    │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│               Banco de Dados PostgreSQL                      │
│  - Tabela Leads                                             │
│  - Tabela LeadActivities                                    │
└──────────────────────────────────────────────────────────────┘
```

## Componentes Implementados

### 1. Backend (.NET 8)

#### 1.1 Entidades de Domínio

**`Lead.cs`** - Entidade principal de leads
- **Campos de Identificação**: SessionId, CompanyName, ContactName, Email, Phone
- **Campos de Localização**: City, State
- **Campos do Funil**: PlanId, PlanName, LastStepReached, LeadSource, Status
- **Campos de Marketing**: Referrer, UtmCampaign, UtmSource, UtmMedium
- **Campos de Gestão Interna**: 
  - AssignedToUserId - Usuário responsável pelo lead
  - AssignedAt - Data de atribuição
  - NextFollowUpDate - Próximo follow-up agendado
  - Score - Pontuação do lead (0-100)
  - Tags - Tags para categorização
  - Notes - Notas sobre o lead
- **Campos de Auditoria**: CapturedAt, LastActivityAt, CreatedAt, UpdatedAt, IsDeleted, DeletedAt
- **Navegação**: Activities - Lista de atividades do lead

**Métodos Principais**:
- `UpdateContactInfo()` - Atualizar informações de contato
- `UpdateStatus()` - Mudar status do lead
- `AssignTo()` / `Unassign()` - Atribuir/desatribuir lead
- `ScheduleFollowUp()` / `ClearFollowUp()` - Agendar follow-up
- `UpdateScore()` - Atualizar pontuação
- `SetTags()` - Definir tags
- `AddNotes()` - Adicionar notas
- `CalculateInitialScore()` - Calcular score inicial baseado na qualidade dos dados

**`LeadActivity.cs`** - Registro de atividades/interações
- **Campos**: Type, Title, Description, PerformedByUserId, PerformedByUserName, ActivityDate, DurationMinutes, Outcome
- **Tipos de Atividade**: PhoneCall, Email, Meeting, Note, StatusChange, Assignment, FollowUpScheduled, Other

**`LeadStatus` Enum**:
- New (0) - Novo lead não contactado
- Contacted (1) - Já foi contactado
- Qualified (2) - Lead qualificado como potencial cliente
- Converted (3) - Convertido em cliente
- Lost (4) - Lead perdido
- Nurturing (5) - Em nutrição/acompanhamento

#### 1.2 Serviço de Gestão de Leads

**`ILeadManagementService` / `LeadManagementService`**

**Funcionalidades de Criação**:
- `CreateLeadFromFunnelAsync()` - Criar lead a partir de sessão de funil abandonada
  - Consolida dados de todas as etapas do funil
  - Calcula score inicial automaticamente
  - Cria atividade inicial

**Funcionalidades de Consulta**:
- `GetUnassignedLeadsAsync()` - Buscar leads não atribuídos
- `GetLeadsAssignedToUserAsync()` - Buscar leads de um usuário específico
- `GetLeadsByStatusAsync()` - Buscar leads por status
- `GetLeadsNeedingFollowUpAsync()` - Buscar leads que precisam follow-up hoje
- `SearchLeadsAsync()` - Buscar por nome, email, telefone ou empresa

**Funcionalidades de Gestão**:
- `AssignLeadAsync()` - Atribuir lead a usuário
- `UpdateLeadStatusAsync()` - Atualizar status com notas
- `ScheduleFollowUpAsync()` - Agendar follow-up
- `UpdateLeadContactInfoAsync()` - Atualizar informações de contato
- `AddLeadNotesAsync()` - Adicionar notas
- `SetLeadTagsAsync()` - Definir tags

**Funcionalidades de Atividades**:
- `AddActivityAsync()` - Registrar nova atividade/interação
- `GetLeadActivitiesAsync()` - Listar todas as atividades de um lead

**Funcionalidades de Análise**:
- `GetLeadStatisticsAsync()` - Estatísticas gerais (total, por status, conversão)
- `GetStatisticsByUserAsync()` - Estatísticas por usuário atribuído

#### 1.3 Serviço de Captura Automática

**`LeadCaptureHostedService`**
- Executa automaticamente a cada 60 minutos
- Identifica sessões abandonadas (>24h sem conversão, mínimo step 2)
- Cria leads automaticamente usando `LeadManagementService`
- Resiliente a falhas (não interrompe serviço principal)
- Log detalhado de operações

#### 1.4 API REST

**`LeadsController`** - Endpoints disponíveis:

```
# Consultas
GET  /api/leads/unassigned                - Leads não atribuídos
GET  /api/leads/assigned/{userId}         - Leads de um usuário
GET  /api/leads/by-status/{status}        - Leads por status
GET  /api/leads/needing-followup          - Leads precisando follow-up
GET  /api/leads/search?searchTerm=...     - Buscar leads
GET  /api/leads/statistics                - Estatísticas gerais
GET  /api/leads/statistics/by-user        - Estatísticas por usuário

# Gerenciamento
POST /api/leads/create-from-funnel/{sessionId}  - Criar lead manual
POST /api/leads/{leadId}/assign                 - Atribuir lead
PUT  /api/leads/{leadId}/status                 - Atualizar status
POST /api/leads/{leadId}/followup               - Agendar follow-up
PUT  /api/leads/{leadId}/contact-info           - Atualizar contato
POST /api/leads/{leadId}/notes                  - Adicionar notas
PUT  /api/leads/{leadId}/tags                   - Definir tags

# Atividades
POST /api/leads/{leadId}/activities        - Adicionar atividade
GET  /api/leads/{leadId}/activities        - Listar atividades
```

**Autorização**: Requer roles `SystemAdmin` ou `SalesManager`

#### 1.5 Migration

**`20260203201500_RefactorSalesforceLeadsToStandaloneLeadManagement.cs`**
- Remove tabela `SalesforceLeads` antiga
- Cria tabela `Leads` nova com campos adicionais
- Cria tabela `LeadActivities`
- Índices otimizados para consultas frequentes
- Suporte a soft delete

### 2. Frontend (Angular/Ionic)

#### 2.1 Modelos TypeScript

**`lead.model.ts`**
- Interface `Lead` - Modelo completo do lead
- Interface `LeadActivity` - Modelo de atividade
- Interface `LeadStatistics` - Estatísticas agregadas
- Interface `UserLeadStatistics` - Estatísticas por usuário
- DTOs para requests (Assign, UpdateStatus, ScheduleFollowUp, etc.)
- Funções auxiliares:
  - `getLeadStatusLabel()` - Tradução de status
  - `getActivityTypeLabel()` - Tradução de tipo de atividade
  - `getLeadScoreColor()` - Cor baseada em score
  - `getLeadStatusColor()` - Cor baseada em status

#### 2.2 Serviço Angular

**`lead.service.ts`**
- Comunicação com API REST
- Métodos para todas as operações CRUD
- Gestão de atividades
- Consulta de estatísticas

#### 2.3 Componente de Gestão

**`LeadsPage`** (`leads.page.ts/.html/.scss`)

**Funcionalidades da UI**:
- 📊 **Dashboard com 8 KPIs**:
  - Total de Leads
  - Novos
  - Qualificados
  - Convertidos
  - Taxa de Conversão
  - Score Médio
  - Precisam Follow-up
  - Não Atribuídos

- 🔍 **Filtros e Busca**:
  - Busca por nome, email, telefone, empresa
  - Filtro por status
  - Filtro por atribuição (todos/atribuídos/não atribuídos)

- 📋 **Tabela de Leads**:
  - Visualização de todos os campos principais
  - Badges coloridos para status e score
  - Ações rápidas (atribuir, follow-up, atividade)
  - Clique para ver detalhes

- 📝 **Painel de Detalhes**:
  - Informações completas do lead
  - Timeline de atividades
  - Notas
  - Botões de ação

- 🎯 **Modais**:
  - Adicionar Atividade (com tipo, título, descrição, duração, resultado)
  - Adicionar Notas
  - Agendar Follow-up
  - Atribuir a Usuário

- 🎨 **Design Responsivo**:
  - Adaptável para desktop, tablet e mobile
  - Grid flexível para estatísticas
  - Tabela com scroll horizontal em telas pequenas

### 3. Configuração

#### 3.1 DbContext
- Adicionado `DbSet<Lead> Leads`
- Adicionado `DbSet<LeadActivity> LeadActivities`
- Removido `DbSet<SalesforceLead> SalesforceLeads`

#### 3.2 Program.cs
- Removida configuração Salesforce
- Adicionado `ILeadManagementService` / `LeadManagementService`
- Adicionado `LeadCaptureHostedService`

#### 3.3 Rotas Frontend
- Rota `/leads` para LeadsPage
- Removida rota `/salesforce-leads`

#### 3.4 Menu
- Item "Gestão de Leads" no menu lateral
- Removido item "Leads Salesforce"

## Sistema de Scoring Automático

O sistema calcula automaticamente uma pontuação (0-100) para cada lead baseado na qualidade dos dados capturados:

```
Base: 50 pontos

+ 20 pontos - Se tem email
+ 15 pontos - Se tem telefone
+ 10 pontos - Se tem nome da empresa
+ 5 pontos  - Se tem nome do contato
+ 5 pontos  - Se tem cidade
+ 5 pontos  - Se tem estado
+ 10 pontos - Se selecionou um plano
+ 2 pontos por etapa alcançada (até 12 pontos para step 6)
+ 5 pontos  - Se tem parâmetros UTM (campanha rastreada)

Máximo: 100 pontos
```

**Interpretação**:
- 80-100: Lead quente (alta qualidade de dados)
- 60-79: Lead morno (boa qualidade)
- 40-59: Lead frio (dados limitados)
- 0-39: Lead muito frio (poucos dados)

## Fluxo de Funcionamento

### Captura Automática de Leads

1. **Cliente abandona cadastro** no site OmniCare
   - Sistema captura dados via `SalesFunnelMetric`
   - Sessão fica marcada como não convertida

2. **Background Service detecta abandono** (após 24h)
   - Verifica sessões não convertidas com mínimo 2 etapas
   - Consolida informações de todas as etapas
   - Calcula score inicial
   - Cria registro `Lead`
   - Cria atividade inicial "Lead Captured"

3. **Notificação** (futuro)
   - Email para equipe de vendas
   - Dashboard de novos leads

### Gestão Manual

1. **Admin acessa "Gestão de Leads"**
2. **Visualiza dashboard** com métricas
3. **Filtra/busca leads** conforme necessidade
4. **Seleciona lead** para ver detalhes
5. **Realiza ações**:
   - Atribui para si ou outro usuário
   - Agenda follow-up
   - Registra atividade (ligação, email, reunião)
   - Adiciona notas
   - Atualiza status conforme progresso

## Comparação com Implementação Salesforce

| Funcionalidade | Salesforce (PR 640) | Standalone (Atual) |
|----------------|---------------------|---------------------|
| Captura automática de leads | ✅ | ✅ |
| Background service | ✅ | ✅ |
| Scoring automático | ❌ | ✅ |
| Atribuição de leads | ❌ | ✅ |
| Follow-up scheduling | ❌ | ✅ |
| Timeline de atividades | ❌ | ✅ |
| Dashboard de métricas | ✅ | ✅ |
| Sincronização externa | ✅ (Salesforce) | ❌ (desnecessário) |
| Dependência externa | ✅ (Salesforce API) | ❌ |
| Custo adicional | ✅ (licença Salesforce) | ❌ |

## Próximas Melhorias Recomendadas

### Curto Prazo
1. **Notificações por Email**
   - Email automático para equipe ao capturar novo lead
   - Lembretes de follow-up pendente

2. **Relatórios Avançados**
   - Relatório de conversão por fonte (UTM)
   - Relatório de performance por usuário
   - Análise de funil de vendas

3. **Automações**
   - Auto-atribuição baseada em regras
   - Status automático baseado em atividades
   - Escalação de leads sem follow-up

### Médio Prazo
1. **Integração com Comunicação**
   - Envio de emails direto da interface
   - Integração com WhatsApp Business
   - Templates de mensagens

2. **Lead Nurturing**
   - Sequências automáticas de follow-up
   - Scoring dinâmico baseado em engajamento
   - Segmentação avançada

3. **Analytics**
   - Dashboard executivo
   - Predição de conversão com ML
   - Análise de padrões de abandono

### Longo Prazo
1. **Integrações**
   - API pública para integrações externas
   - Webhook para sistemas third-party
   - Sincronização com outros CRMs (opcional)

2. **Mobile App**
   - App nativo para gestão de leads em movimento
   - Notificações push
   - Acesso offline

## Segurança e Compliance

### LGPD
- ✅ Dados sensíveis criptografados em trânsito (HTTPS/TLS)
- ✅ Soft delete implementado (não remove dados físicamente)
- ✅ Auditoria de acesso via logs
- ✅ Retenção de dados configurável
- ✅ Dados de leads são system-wide (não multi-tenant)

### Segurança
- ✅ Autenticação obrigatória (JWT Bearer token)
- ✅ Autorização por role (SystemAdmin, SalesManager)
- ✅ Validação de entrada em todos os endpoints
- ✅ Rate limiting aplicado
- ✅ SQL injection prevenido (EF Core parametrizado)
- ✅ Logging seguro (sem dados sensíveis em logs)

## Conclusão

O sistema de gestão de leads standalone fornece todas as funcionalidades essenciais da implementação Salesforce do PR 640, com melhorias significativas:

1. **Sem Dependências Externas**: Não requer licença ou API externa
2. **Funcionalidades Adicionais**: Scoring, atribuição, follow-ups, timeline
3. **Custo Zero**: Sem custos recorrentes de plataforma
4. **Controle Total**: Dados e lógica totalmente sob controle
5. **Personalizável**: Fácil adicionar funcionalidades específicas do negócio

O sistema está pronto para uso após correção dos erros de compilação e testes de integração.

---

**Desenvolvido**: Fevereiro 2026  
**Versão**: 1.0  
**Status**: Em desenvolvimento (pendente correções de build)
