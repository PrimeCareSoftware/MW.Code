# Implementação de Gestão de Leads Salesforce

## Visão Geral

Este documento descreve a implementação completa do sistema de captura e gerenciamento de leads abandonados do fluxo de cadastro, com integração ao Salesforce CRM.

## Problema Resolvido

O site OmniCare estava capturando dados de potenciais clientes que avançavam no fluxo de cadastro mas abandonavam antes de completar a contratação. Esses dados valiosos não estavam sendo aproveitados para ações de marketing e vendas.

## Solução Implementada

### Arquitetura

A solução segue o padrão de arquitetura limpa já estabelecido no projeto:

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Angular)                        │
│  - SalesforceLeadsComponent                                  │
│  - SalesforceLeadsService                                    │
└──────────────────┬──────────────────────────────────────────┘
                   │ HTTP REST API
┌──────────────────▼──────────────────────────────────────────┐
│                Backend (.NET 8 API)                          │
│  - SalesforceLeadsController                                 │
│  - ISalesforceLeadService / SalesforceLeadService           │
│  - SalesforceLeadSyncHostedService (Background)             │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│                    Domínio                                   │
│  - SalesforceLead (Entity)                                  │
│  - SalesFunnelMetric (Entity existente)                     │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│               Salesforce CRM (Externo)                       │
│  - OAuth 2.0 Authentication                                  │
│  - Lead Creation API                                         │
└──────────────────────────────────────────────────────────────┘
```

## Componentes Implementados

### 1. Backend

#### 1.1 Entidade de Domínio

**`SalesforceLead.cs`**
- Armazena informações do lead capturado
- Rastreia status de sincronização com Salesforce
- Suporta até 3 tentativas de sincronização
- Campos principais:
  - Informações de contato (nome, email, telefone)
  - Dados da empresa (nome, cidade, estado)
  - Plano selecionado
  - Última etapa alcançada (1-6)
  - Status do lead (New, Contacted, Qualified, Converted, Lost, Nurturing)
  - Dados de tracking (UTM, referrer)

#### 1.2 Serviço de Integração

**`ISalesforceLeadService / SalesforceLeadService`**

Funcionalidades:
- ✅ Criar lead a partir de sessão de funil abandonada
- ✅ Sincronizar lead individual com Salesforce
- ✅ Sincronizar todos os leads pendentes
- ✅ Atualizar status de leads
- ✅ Obter estatísticas de leads
- ✅ Testar conexão com Salesforce

Características técnicas:
- Autenticação OAuth 2.0 Password Flow
- Retry policies com Polly
- Cache de tokens de autenticação
- Logging detalhado de erros

#### 1.3 API Controller

**`SalesforceLeadsController`**

Endpoints disponíveis:
```
GET  /api/salesforceleads/unsynced          - Lista leads não sincronizados
GET  /api/salesforceleads/by-status/{status} - Lista por status
GET  /api/salesforceleads/statistics        - Estatísticas gerais
POST /api/salesforceleads/create-from-funnel/{sessionId} - Cria lead
POST /api/salesforceleads/sync/{leadId}     - Sincroniza lead específico
POST /api/salesforceleads/sync-all          - Sincroniza todos
PUT  /api/salesforceleads/{leadId}/status   - Atualiza status
GET  /api/salesforceleads/test-connection   - Testa conexão
```

#### 1.4 Background Service

**`SalesforceLeadSyncHostedService`**

- Executa a cada 30 minutos automaticamente
- Identifica sessões abandonadas (>24h sem conversão)
- Cria leads automaticamente
- Tenta sincronizar com Salesforce
- Resiliente a falhas (não interrompe o serviço principal)

#### 1.5 Migration

**`20260203183400_AddSalesforceLeadManagement.cs`**

Cria tabela `SalesforceLeads` com:
- Índices otimizados para consultas frequentes
- Suporte a soft delete
- Multi-tenancy preparado
- Campos de auditoria

### 2. Frontend

#### 2.1 Modelos TypeScript

**`salesforce-lead.model.ts`**
- Interfaces tipadas para leads
- Enum de status
- Estatísticas e resultados de sync

#### 2.2 Serviço Angular

**`SalesforceLeadsService`**

Métodos disponíveis:
- `getUnsyncedLeads()` - Lista não sincronizados
- `getLeadsByStatus()` - Filtra por status
- `getStatistics()` - Obter métricas
- `createLeadFromFunnel()` - Criar lead
- `syncLead()` - Sincronizar individual
- `syncAllLeads()` - Sincronizar todos
- `updateLeadStatus()` - Atualizar status
- `testConnection()` - Verificar conexão
- Helpers de formatação e tradução

#### 2.3 Componente de Gerenciamento

**`SalesforceLeadsComponent`**

Funcionalidades da UI:
- 📊 Dashboard com KPIs (total, novos, qualificados, convertidos, taxa de conversão)
- 🔄 Teste de conexão com Salesforce
- 🔎 Filtros por status
- 🔍 Busca por nome, email ou telefone
- 📋 Tabela responsiva com todos os leads
- ⚡ Sincronização individual ou em lote
- ✏️ Atualização de status inline
- 🎨 Badges coloridos para status visual

### 3. Configuração

#### 3.1 appsettings.json

```json
{
  "Salesforce": {
    "Enabled": false,
    "InstanceUrl": "https://your-instance.salesforce.com",
    "ClientId": "",
    "ClientSecret": "",
    "Username": "",
    "Password": "",
    "SecurityToken": "",
    "ApiVersion": "v57.0",
    "AutoSyncEnabled": false,
    "SyncIntervalMinutes": 60,
    "MaxSyncAttempts": 3
  }
}
```

#### 3.2 Variáveis de Ambiente (Produção)

Para produção, configure via variáveis de ambiente:
```bash
Salesforce__Enabled=true
Salesforce__InstanceUrl=https://yourcompany.salesforce.com
Salesforce__ClientId=<seu_client_id>
Salesforce__ClientSecret=<seu_client_secret>
Salesforce__Username=<seu_username>
Salesforce__Password=<sua_senha>
Salesforce__SecurityToken=<seu_token>
```

## Fluxo de Funcionamento

### Captura Automática de Leads

1. **Cliente abandona cadastro** no site OmniCare
   - Sistema já captura dados via `SalesFunnelMetric`
   - Sessão fica marcada como abandonada

2. **Background Service detecta abandono** (após 24h)
   - Verifica sessões não convertidas
   - Cria registro `SalesforceLead` com dados capturados
   - Agrupa informações de todas as etapas do funil

3. **Sincronização automática com Salesforce**
   - Autentica via OAuth 2.0
   - Cria Lead no Salesforce com custom fields
   - Atualiza status local (synced/failed)
   - Retry automático em caso de falha (até 3 vezes)

### Gestão Manual

1. **Admin acessa System Admin → Leads Salesforce**
2. **Visualiza dashboard** com métricas consolidadas
3. **Pode realizar ações**:
   - Testar conexão Salesforce
   - Sincronizar leads individualmente
   - Sincronizar todos pendentes
   - Atualizar status de leads
   - Filtrar e buscar leads

## Campos Personalizados no Salesforce

Para aproveitar ao máximo a integração, crie estes custom fields no Salesforce:

| Campo API Name | Tipo | Descrição |
|----------------|------|-----------|
| `Registration_Step__c` | Number | Última etapa alcançada (1-6) |
| `Selected_Plan__c` | Text(100) | Nome do plano selecionado |
| `UTM_Campaign__c` | Text(200) | Campaign de origem |
| `UTM_Source__c` | Text(200) | Fonte de tráfego |
| `UTM_Medium__c` | Text(200) | Meio de marketing |
| `Session_ID__c` | Text(100) | ID da sessão para rastreamento |

## Segurança e Compliance

### LGPD

- ✅ Dados sensíveis criptografados em trânsito (HTTPS/TLS)
- ✅ Senhas e tokens não são logados
- ✅ Soft delete implementado (não remove dados físicamente)
- ✅ Auditoria de acesso via logs
- ✅ Retenção de dados configurável

### Segurança

- ✅ Autenticação obrigatória (Bearer token)
- ✅ Autorização por role (SystemAdmin apenas)
- ✅ Validação de entrada em todos os endpoints
- ✅ Credentials em variáveis de ambiente (não em código)
- ✅ Rate limiting aplicado
- ✅ SQL injection prevenido (EF Core parametrizado)

## Métricas e Monitoramento

### KPIs Disponíveis

1. **Total de Leads**: Quantidade total capturada
2. **Novos**: Leads não contactados
3. **Qualificados**: Leads validados como potenciais
4. **Convertidos**: Leads que viraram clientes
5. **Taxa de Conversão**: % de leads convertidos
6. **Sincronizados/Pendentes**: Status de integração Salesforce

### Logs

O sistema gera logs detalhados em:
```
Logs/primecare-{date}.log        - Logs gerais
Logs/primecare-errors-{date}.log - Apenas erros
```

Eventos logados:
- Criação de leads
- Tentativas de sincronização
- Erros de autenticação
- Falhas de API

## Testes

### Testes Manuais Recomendados

1. **Teste de Conexão**
   - Acessar página de leads
   - Clicar em "Testar Conexão"
   - Verificar status de sucesso/erro

2. **Criação de Lead**
   - Simular abandono de cadastro
   - Aguardar 24h ou criar manualmente via API
   - Verificar dados capturados

3. **Sincronização**
   - Clicar em "Sincronizar Todos"
   - Verificar leads criados no Salesforce
   - Validar campos personalizados

### Testes Automatizados (Futuro)

- [ ] Unit tests para `SalesforceLeadService`
- [ ] Integration tests para API endpoints
- [ ] E2E tests para fluxo completo

## Troubleshooting

### Lead não sincroniza

**Problema**: Lead permanece com status "Pendente"

**Soluções**:
1. Verificar configuração Salesforce (credenciais corretas?)
2. Testar conexão via botão "Testar Conexão"
3. Verificar logs de erro: `Logs/primecare-errors-*.log`
4. Verificar se atingiu limite de 3 tentativas (`syncAttempts`)
5. Resetar contador manualmente via API se necessário

### Campos não aparecem no Salesforce

**Problema**: Lead criado mas sem campos personalizados

**Solução**:
1. Criar custom fields no Salesforce conforme tabela acima
2. Atribuir permissões aos campos para o usuário da integração
3. Re-sincronizar lead

### Background service não executa

**Problema**: Leads não são criados automaticamente

**Solução**:
1. Verificar se serviço está registrado: `Program.cs`
2. Verificar logs de startup da aplicação
3. Confirmar que `AutoSyncEnabled=true` na configuração

## Próximas Melhorias

### Curto Prazo
- [ ] Página de configuração Salesforce no System Admin UI
- [ ] Webhook reverso: atualizar leads do Salesforce → OmniCare
- [ ] Dashboard de funil de vendas completo
- [ ] Exportação de leads para CSV

### Médio Prazo
- [ ] Integração com outras ferramentas CRM (HubSpot, RD Station)
- [ ] Automação de emails para leads (via SendGrid)
- [ ] Lead scoring automático com ML
- [ ] Segmentação avançada de leads

### Longo Prazo
- [ ] Chatbot para qualificação de leads
- [ ] Integração com WhatsApp Business
- [ ] Analytics preditivos de conversão

## Referências

- [Salesforce REST API Documentation](https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/)
- [OAuth 2.0 in Salesforce](https://help.salesforce.com/s/articleView?id=sf.remoteaccess_oauth_flows.htm)
- [LGPD - Lei Geral de Proteção de Dados](https://www.gov.br/esporte/pt-br/acesso-a-informacao/lgpd)

## Suporte

Para dúvidas ou problemas:
1. Consulte este documento
2. Verifique os logs da aplicação
3. Entre em contato com a equipe de desenvolvimento
