# Prompt 04: Fase 4 - Recursos Avançados (Mês 8-10)

## 📋 Contexto

A Fase 4 adiciona recursos avançados que posicionam o PrimeCare como uma solução completa e competitiva no mercado. Estes recursos são diferenciadores importantes e elevam significativamente o valor da plataforma.

**Referência**: `MVP_IMPLEMENTATION_GUIDE.md` - Fase 4
**Status**: 📋 Planejado
**Prioridade**: P2 - Média
**Estimativa**: 3 meses (Mês 8-10)
**Equipe**: 3-4 desenvolvedores

## 🎯 Objetivos

1. Implementar Assinatura Digital ICP-Brasil
2. Implementar Exportação TISS completa
3. Implementar CRM Integrado
4. Implementar Marketing Automation
5. Implementar API Pública

## 📚 Tarefas

### 1. Assinatura Digital ICP-Brasil (5 semanas)

**1.1 Integração com Certificado Digital**

- [ ] Pesquisar e selecionar provedor de Assinatura Digital
  - Opções: Serpro, Valid, Certisign, Soluti
  - Avaliar custos, APIs disponíveis, suporte
- [ ] Criar conta e configurar ambiente de testes
- [ ] Implementar fluxo de upload de certificado A1
- [ ] Implementar integração com token A3 (USB)

**1.2 Assinatura de Documentos**

```csharp
// src/Core/Interfaces/IDigitalSignatureService.cs
public interface IDigitalSignatureService
{
    Task<byte[]> SignDocumentAsync(byte[] document, Certificate certificate);
    Task<bool> ValidateSignatureAsync(byte[] signedDocument);
    Task<SignatureInfo> GetSignatureInfoAsync(byte[] signedDocument);
}

public class SignatureInfo
{
    public string SignerName { get; set; }
    public string SignerCpf { get; set; }
    public DateTime SignedAt { get; set; }
    public bool IsValid { get; set; }
    public CertificateInfo Certificate { get; set; }
}
```

**Documentos a Assinar**:
- [ ] Receitas médicas (obrigatório)
- [ ] Atestados médicos (obrigatório)
- [ ] Laudos médicos (obrigatório)
- [ ] Solicitações de exames
- [ ] Relatórios médicos
- [ ] Guias TISS

**1.3 Conformidade CFM**

- [ ] Garantir que assinatura atende CFM 1.821/2007
- [ ] Incluir número do CRM e UF na assinatura
- [ ] Incluir carimbo visual com informações do médico
- [ ] Implementar validação de certificado válido

**1.4 Interface de Usuário**

```typescript
// frontend/medicwarehouse-app/src/app/components/digital-signature/

interface DigitalSignatureComponent {
  // Upload de certificado A1
  uploadCertificate(file: File, password: string): Promise<void>;
  
  // Conectar token A3
  connectToken(): Promise<void>;
  
  // Assinar documento
  signDocument(documentId: string): Promise<void>;
  
  // Verificar assinatura
  verifySignature(documentId: string): Promise<SignatureInfo>;
}
```

- [ ] Modal para upload de certificado
- [ ] Detecção automática de token A3
- [ ] Botão "Assinar Digitalmente" nos documentos
- [ ] Visualização de status de assinatura
- [ ] Validador de assinatura digital

**1.5 Armazenamento Seguro**

- [ ] Criptografar certificados no banco (Azure Key Vault)
- [ ] Nunca armazenar senha do certificado
- [ ] Implementar expiration tracking de certificados
- [ ] Notificar médicos quando certificado está próximo de expirar

### 2. Exportação TISS Completa (4 semanas)

**Nota**: TISS Fase 1 já foi implementada (Prompt 06). Esta fase completa a implementação.

**2.1 Padrão TISS 4.02.00 Completo**

Implementar **todos** os tipos de guia:
- [ ] Guia de Consulta (já implementado na Fase 1)
- [ ] Guia de SP/SADT (Serviços Profissionais / Serviços Auxiliares de Diagnóstico e Terapia)
- [ ] Guia de Internação
- [ ] Guia de Resumo de Internação
- [ ] Guia de Honorários Individuais
- [ ] Guia de Tratamento Odontológico

**2.2 Lote de Guias**

```csharp
// src/Core/Entities/TISS/TISSBatch.cs
public class TISSBatch
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid HealthInsuranceProviderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalGuides { get; set; }
    public decimal TotalValue { get; set; }
    public TISSBatchStatus Status { get; set; }
    public string XmlFilePath { get; set; }
    public List<TISSGuide> Guides { get; set; }
    public string ProtocolNumber { get; set; } // Retorno da operadora
    public DateTime? SubmittedAt { get; set; }
}
```

- [ ] Criar lote de guias
- [ ] Validar XML contra XSD do TISS
- [ ] Gerar arquivo para envio
- [ ] Processar retorno da operadora
- [ ] Gerenciar glosas e devoluções

**2.3 Integração com Operadoras**

- [ ] Implementar envio via webservice (quando disponível)
- [ ] Implementar envio via FTP (legacy)
- [ ] Implementar envio manual (download XML)
- [ ] Processar arquivo de retorno
- [ ] Conciliar pagamentos

**2.4 Relatórios TISS**

- [ ] Relatório de guias enviadas
- [ ] Relatório de glosas
- [ ] Relatório de pagamentos recebidos
- [ ] Demonstrativo de pagamento
- [ ] Análise de performance por convênio

### 3. CRM Integrado (5 semanas)

**Nota**: CRM básico já foi implementado. Esta fase integra profundamente com o sistema.

**3.1 Pipeline de Vendas**

```typescript
// frontend/medicwarehouse-app/src/app/pages/crm/pipeline/

interface SalesPipeline {
  stages: Stage[];
  deals: Deal[];
}

interface Stage {
  id: string;
  name: string;
  order: number;
  probability: number; // % de conversão
  deals: Deal[];
}

interface Deal {
  id: string;
  name: string;
  value: number;
  probability: number;
  expectedCloseDate: Date;
  stageId: string;
  contactId: string;
  ownerId: string; // Quem é responsável
  activities: Activity[];
  notes: Note[];
}
```

**Estágios Padrão**:
1. Lead (10% conversão)
2. Contato Realizado (25%)
3. Reunião Agendada (40%)
4. Proposta Enviada (60%)
5. Negociação (80%)
6. Fechado-Ganho (100%)
7. Fechado-Perdido (0%)

- [ ] Board Kanban para visualizar pipeline
- [ ] Drag-and-drop para mover deals entre estágios
- [ ] Previsão de receita baseada em probabilidade
- [ ] Alertas para deals parados
- [ ] Relatório de conversão por estágio

**3.2 Gestão de Contatos e Leads**

```typescript
interface Contact {
  id: string;
  type: 'lead' | 'patient' | 'prospect';
  name: string;
  email: string;
  phone: string;
  source: string; // Como chegou até nós
  tags: string[];
  customFields: Record<string, any>;
  
  // Scoring
  score: number; // 0-100
  lastInteraction: Date;
  
  // Relacionamento
  deals: Deal[];
  activities: Activity[];
  appointments: Appointment[];
}
```

- [ ] Importação de leads (CSV, Excel)
- [ ] Captura de leads via formulários web
- [ ] Lead scoring automático
- [ ] Segmentação de contatos
- [ ] Enriquecimento de dados (integração com APIs externas)

**3.3 Automação de Follow-up**

- [ ] Sequências de email automáticas
- [ ] Lembretes de follow-up
- [ ] Tasks automáticas baseadas em triggers
- [ ] Workflows de nurturing

**3.4 Integração com Pacientes**

- [ ] Converter lead em paciente com 1 clique
- [ ] Sincronizar dados entre CRM e cadastro de pacientes
- [ ] Histórico unificado de interações
- [ ] View 360° do cliente/paciente

### 4. Marketing Automation (4 semanas)

**4.1 Campanhas de Email**

```typescript
// frontend/medicwarehouse-app/src/app/pages/marketing/campaigns/

interface EmailCampaign {
  id: string;
  name: string;
  subject: string;
  content: string; // HTML
  segmentId: string; // Qual segmento de contatos
  scheduledFor?: Date;
  status: 'draft' | 'scheduled' | 'sending' | 'sent' | 'paused';
  
  stats: {
    sent: number;
    delivered: number;
    opened: number;
    clicked: number;
    bounced: number;
    unsubscribed: number;
  };
}
```

**Funcionalidades**:
- [ ] Editor visual de email (drag-and-drop)
- [ ] Personalização com variáveis ({{nome}}, {{clinica}}, etc)
- [ ] A/B testing (assunto, conteúdo)
- [ ] Agendamento de envio
- [ ] Tracking de opens e clicks
- [ ] Gerenciamento de unsubscribe

**4.2 Segmentação**

```typescript
interface Segment {
  id: string;
  name: string;
  rules: SegmentRule[];
  contactCount: number;
  autoUpdate: boolean; // Atualizar automaticamente
}

interface SegmentRule {
  field: string; // Ex: 'lastAppointment', 'tags', 'age'
  operator: 'equals' | 'contains' | 'greaterThan' | 'lessThan' | 'between';
  value: any;
}
```

**Exemplos de Segmentos**:
- Pacientes inativos (sem consulta há 6+ meses)
- Aniversariantes do mês
- Pacientes de uma especialidade específica
- Leads não convertidos
- Pacientes com convênio X

**4.3 Automação de Workflows**

```typescript
interface AutomationWorkflow {
  id: string;
  name: string;
  trigger: Trigger;
  actions: Action[];
  active: boolean;
}

interface Trigger {
  type: 'appointment_scheduled' | 'patient_registered' | 'birthday' | 'inactivity' | 'tag_added';
  conditions?: any;
}

interface Action {
  type: 'send_email' | 'send_sms' | 'send_whatsapp' | 'create_task' | 'add_tag' | 'update_score';
  delay?: number; // minutos
  config: any;
}
```

**Exemplos de Workflows**:
1. **Welcome Series**: Novo paciente → Email de boas-vindas → Aguardar 2 dias → Email sobre serviços
2. **Re-engajamento**: 6 meses sem consulta → Email de saudades → Aguardar 1 semana → SMS com promoção
3. **Aniversário**: Dia do aniversário → Email parabenizando
4. **Pós-consulta**: Consulta realizada → Aguardar 1 dia → Email de satisfação

**4.4 Landing Pages**

- [ ] Builder de landing pages
- [ ] Formulários de captura de leads
- [ ] Integração com Facebook Pixel e Google Analytics
- [ ] A/B testing de páginas
- [ ] Templates responsivos

### 5. API Pública (4 semanas)

**5.1 Design da API**

```csharp
// RESTful API with OpenAPI/Swagger documentation
// Base URL: https://api.primecare.com.br/v1/

// Autenticação: OAuth 2.0 + API Keys
// Rate limiting: 1000 requests/hour por API key
// Formato: JSON
// Versionamento: URL-based (/v1/, /v2/)
```

**Endpoints Principais**:

```
// Pacientes
GET    /api/v1/patients
POST   /api/v1/patients
GET    /api/v1/patients/{id}
PUT    /api/v1/patients/{id}
DELETE /api/v1/patients/{id}

// Agendamentos
GET    /api/v1/appointments
POST   /api/v1/appointments
GET    /api/v1/appointments/{id}
PUT    /api/v1/appointments/{id}
DELETE /api/v1/appointments/{id}

// Profissionais
GET    /api/v1/professionals
GET    /api/v1/professionals/{id}/availability

// Convênios
GET    /api/v1/health-insurances
GET    /api/v1/health-insurances/{id}

// Webhooks
POST   /api/v1/webhooks
GET    /api/v1/webhooks
DELETE /api/v1/webhooks/{id}
```

**5.2 Autenticação e Segurança**

- [ ] Implementar OAuth 2.0 (Client Credentials Flow)
- [ ] Gerar API Keys para clientes
- [ ] Implementar rate limiting
- [ ] Implementar API throttling
- [ ] Logs de todas as requisições
- [ ] IP whitelisting (opcional)

**5.3 Documentação**

- [ ] Documentação completa no Swagger/OpenAPI
- [ ] Guia de início rápido
- [ ] Exemplos de código (JavaScript, Python, PHP, C#)
- [ ] Coleção do Postman
- [ ] Changelog de versões

**5.4 Developer Portal**

```typescript
// frontend/medicwarehouse-app/src/app/pages/developer-portal/

interface DeveloperPortal {
  // Gerenciamento de API Keys
  apiKeys: APIKey[];
  
  // Logs de requisições
  requestLogs: RequestLog[];
  
  // Documentação interativa
  documentation: SwaggerUI;
  
  // Webhooks
  webhooks: Webhook[];
  
  // Métricas de uso
  usage: {
    requestsToday: number;
    requestsThisMonth: number;
    quotaUsed: number;
    quotaTotal: number;
  };
}
```

**5.5 Webhooks**

Permitir clientes se inscreverem para receber notificações:

```typescript
interface Webhook {
  id: string;
  url: string;
  events: string[]; // ['appointment.created', 'patient.updated', etc]
  secret: string; // Para validar assinatura
  active: boolean;
}

// Eventos disponíveis
const WEBHOOK_EVENTS = [
  'appointment.created',
  'appointment.updated',
  'appointment.canceled',
  'patient.created',
  'patient.updated',
  'payment.received',
  'document.signed'
];
```

## ✅ Critérios de Sucesso

### Assinatura Digital
- [ ] Integração funcionando com pelo menos 2 provedores
- [ ] Suporte para certificado A1 e A3
- [ ] Todos os tipos de documentos podem ser assinados
- [ ] Validação de assinatura funcionando

### TISS Completa
- [ ] Todos os 6 tipos de guia implementados
- [ ] Exportação XML validando contra XSD
- [ ] Integração com pelo menos 5 operadoras
- [ ] Conciliação de pagamentos funcionando

### CRM
- [ ] Pipeline visual funcionando
- [ ] Lead scoring implementado
- [ ] Conversão de lead para paciente funcionando
- [ ] Pelo menos 50% dos usuários usando CRM ativamente

### Marketing Automation
- [ ] Editor de email funcionando
- [ ] Pelo menos 10 templates de email prontos
- [ ] Workflows automáticos funcionando
- [ ] Taxa de entrega de emails > 95%

### API Pública
- [ ] Documentação completa no Swagger
- [ ] Pelo menos 20 endpoints implementados
- [ ] Rate limiting funcionando
- [ ] Pelo menos 3 clientes usando a API

## 📊 Métricas a Monitorar

### Assinatura Digital
- **Documentos Assinados/Mês**: Baseline
- **Tempo Médio para Assinar**: Meta < 30s
- **Taxa de Sucesso**: Meta > 95%

### TISS
- **Guias Enviadas/Mês**: Baseline
- **Taxa de Glosas**: Meta < 5%
- **Tempo de Conciliação**: Meta < 7 dias

### CRM
- **Taxa de Conversão Lead → Paciente**: Meta > 20%
- **Tempo Médio no Pipeline**: Baseline
- **Adoção do CRM**: Meta > 50%

### Marketing
- **Taxa de Abertura de Emails**: Meta > 20%
- **Taxa de Click**: Meta > 2%
- **Taxa de Unsubscribe**: Meta < 0.5%

### API
- **Requests/Dia**: Baseline
- **Error Rate**: Meta < 1%
- **Latência P95**: Meta < 500ms

## 🔗 Dependências

### Pré-requisitos
- Prompt 03: Fase 3 - Recursos Essenciais completo
- Sistema de pagamentos funcionando
- CRM básico implementado (Prompt 17)

### Bloqueia
- Prompt 05: Fase 5 - Inteligência e Automação

## 📂 Arquivos Principais

```
src/
├── API/Controllers/
│   ├── DigitalSignatureController.cs (criar)
│   ├── TISSController.cs (expandir)
│   ├── CRMController.cs (expandir)
│   ├── MarketingController.cs (criar)
│   └── PublicAPIController.cs (criar)
├── Core/Services/
│   ├── DigitalSignatureService.cs (criar)
│   ├── TISSExportService.cs (expandir)
│   └── MarketingAutomationService.cs (criar)

frontend/medicwarehouse-app/src/app/
├── pages/
│   ├── digital-signature/
│   ├── tiss/
│   ├── crm/ (expandir)
│   ├── marketing/
│   └── developer-portal/
```

## 🔐 Segurança

- [ ] Criptografar certificados digitais
- [ ] Logs de todas as assinaturas digitais
- [ ] API Keys com permissões granulares
- [ ] Validar assinaturas de webhooks
- [ ] Rate limiting por API key
- [ ] Sanitizar dados em campanhas de email

## 📝 Notas

- **Custos**: Assinatura digital tem custo por assinatura, considerar nos planos
- **Compliance**: Assinatura digital deve atender normas CFM e ICP-Brasil
- **TISS**: Testar extensivamente com operadoras reais
- **API**: Documentação é crítica para adoção

## 🚀 Próximos Passos

Após concluir este prompt:
1. Iniciar Prompt 05: Fase 5 - Inteligência e Automação (Mês 11-12)
2. Divulgar API pública para parceiros
3. Criar cases de uso da API
4. Monitorar uso e feedback
