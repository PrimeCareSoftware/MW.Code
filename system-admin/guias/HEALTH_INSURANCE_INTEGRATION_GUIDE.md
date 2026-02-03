# Guia de Integração com Operadoras de Planos de Saúde

## Visão Geral

Este documento descreve como funciona a integração com operadoras de planos de saúde no Brasil, fornecendo um roteiro para implementação gradual no sistema Omni Care Software.

## 1. Contexto e Padrões no Brasil

### 1.1 Padrão TISS (Troca de Informações em Saúde Suplementar)

O **TISS** é o padrão obrigatório estabelecido pela ANS (Agência Nacional de Saúde Suplementar) para troca de informações entre prestadores de serviços de saúde e operadoras de planos de saúde.

**Componentes principais:**
- **Guias**: Documentos eletrônicos que solicitam autorização para procedimentos
- **Lotes**: Agrupamento de guias para envio em lote
- **Demonstrativos**: Relatórios de pagamento das operadoras
- **Tabelas Padronizadas**: Códigos únicos para procedimentos, materiais e medicamentos

### 1.2 Tipos de Guias TISS

1. **Guia de Consulta** - Para consultas médicas simples
2. **Guia de SP/SADT** - Serviços Profissionais e Serviços de Apoio Diagnóstico e Terapêutico
3. **Guia de Internação** - Para internações hospitalares
4. **Guia de Honorários** - Para pagamento de profissionais em procedimentos
5. **Guia de Resumo de Internação** - Resumo após alta hospitalar
6. **Guia de Tratamento Odontológico** - Específica para dentistas

### 1.3 Fluxo Típico de Autorização

```
1. Paciente agenda consulta
   ↓
2. Clínica verifica elegibilidade do beneficiário (online ou telefone)
   ↓
3. Clínica solicita autorização prévia (se necessário)
   ↓
4. Operadora analisa e autoriza/nega
   ↓
5. Clínica realiza o atendimento
   ↓
6. Clínica gera guia TISS
   ↓
7. Clínica envia lote de guias para faturamento
   ↓
8. Operadora processa e retorna demonstrativo
   ↓
9. Operadora efetua pagamento
```

## 2. Modelos de Integração no Mercado

### 2.1 Integração Manual (Mais Simples)

**Características:**
- Preenchimento de formulários em papel ou PDF
- Envio físico ou por email para operadoras
- Acompanhamento manual dos processos
- Maior trabalho administrativo

**Vantagens:**
- Não requer infraestrutura tecnológica
- Baixo custo inicial
- Funciona com todas as operadoras

**Desvantagens:**
- Alto requerimento de tempo
- Sujeito a erros humanos
- Difícil rastreamento
- Atrasos no recebimento

### 2.2 Integração via Portal Web (Comum)

**Características:**
- Acesso aos portais das operadoras
- Preenchimento de formulários online
- Envio e consulta de status via web
- Cada operadora tem seu próprio portal

**Vantagens:**
- Mais rápido que o processo manual
- Feedback imediato em alguns casos
- Histórico disponível online

**Desvantagens:**
- Múltiplos portais para gerenciar
- Diferentes interfaces e processos
- Ainda requer entrada manual de dados
- Não há integração com o sistema da clínica

### 2.3 Integração via XML TISS (Recomendado)

**Características:**
- Geração automática de arquivos XML no padrão TISS
- Envio programático para as operadoras
- Resposta automática com status de autorização
- Reduz drasticamente trabalho manual

**Vantagens:**
- Automação completa
- Redução de erros
- Integração direta com o sistema
- Agilidade no faturamento
- Padrão único para todas as operadoras

**Desvantagens:**
- Requer desenvolvimento técnico
- Necessita certificado digital
- Depende de homologação com cada operadora

### 2.4 Integração via APIs REST (Moderno)

**Características:**
- Algumas operadoras modernas oferecem APIs REST
- Comunicação em tempo real
- Verificação de elegibilidade instantânea
- Autorização online

**Vantagens:**
- Tempo real
- Moderna e eficiente
- Fácil manutenção
- Feedback imediato

**Desvantagens:**
- Poucas operadoras oferecem
- Cada operadora tem sua própria API
- Necessita autenticação e certificados

## 3. Entidades para Implementação

### 3.1 HealthInsuranceOperator (Operadora)

```csharp
public class HealthInsuranceOperator : BaseEntity
{
    public string TradeName { get; private set; }           // Nome comercial
    public string CompanyName { get; private set; }         // Razão social
    public string RegisterNumber { get; private set; }      // Registro ANS
    public string Document { get; private set; }            // CNPJ
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string ContactPerson { get; private set; }
    public bool IsActive { get; private set; }
    
    // Configurações de integração
    public OperatorIntegrationType IntegrationType { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? ApiEndpoint { get; private set; }
    public string? ApiKey { get; private set; }
    public bool RequiresPriorAuthorization { get; private set; }
    
    // Configurações TISS
    public string? TissVersion { get; private set; }
    public bool SupportsTissXml { get; private set; }
    public string? BatchSubmissionEmail { get; private set; }
}

public enum OperatorIntegrationType
{
    Manual,      // Processo manual
    WebPortal,   // Portal web
    TissXml,     // XML TISS
    RestApi      // API REST
}
```

### 3.2 HealthInsurancePlan (Plano)

```csharp
public class HealthInsurancePlan : BaseEntity
{
    public Guid OperatorId { get; private set; }
    public string PlanName { get; private set; }
    public string PlanCode { get; private set; }            // Código do plano
    public string RegisterNumber { get; private set; }      // Registro ANS do plano
    public PlanType Type { get; private set; }              // Individual, Empresarial, Coletivo
    public bool IsActive { get; private set; }
    
    // Cobertura
    public bool CoversConsultations { get; private set; }
    public bool CoversExams { get; private set; }
    public bool CoversProcedures { get; private set; }
    public bool RequiresPriorAuthorization { get; private set; }
    
    // Navigation
    public HealthInsuranceOperator? Operator { get; private set; }
}

public enum PlanType
{
    Individual,   // Plano individual/familiar
    Enterprise,   // Plano empresarial
    Collective    // Plano coletivo por adesão
}
```

### 3.3 PatientHealthInsurance (Vínculo Paciente-Plano)

```csharp
public class PatientHealthInsurance : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid HealthInsurancePlanId { get; private set; }
    public string CardNumber { get; private set; }          // Número da carteirinha
    public string? CardValidationCode { get; private set; }  // CVV da carteirinha
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public bool IsActive { get; private set; }
    
    // Informações do titular (se paciente for dependente)
    public bool IsHolder { get; private set; }
    public string? HolderName { get; private set; }
    public string? HolderDocument { get; private set; }     // CPF do titular
    
    // Navigation
    public Patient? Patient { get; private set; }
    public HealthInsurancePlan? HealthInsurancePlan { get; private set; }
}
```

### 3.4 AuthorizationRequest (Solicitação de Autorização)

```csharp
public class AuthorizationRequest : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid HealthInsurancePlanId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public string RequestNumber { get; private set; }       // Número da solicitação
    public DateTime RequestDate { get; private set; }
    public AuthorizationStatus Status { get; private set; }
    
    // Autorização
    public string? AuthorizationNumber { get; private set; }
    public DateTime? AuthorizationDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public string? DenialReason { get; private set; }
    
    // Procedimentos solicitados
    public string ProcedureCode { get; private set; }       // Código TUSS
    public string ProcedureDescription { get; private set; }
    public int Quantity { get; private set; }
    
    // Informações clínicas
    public string? ClinicalIndication { get; private set; }
    public string? Diagnosis { get; private set; }
    
    // Navigation
    public Patient? Patient { get; private set; }
    public HealthInsurancePlan? HealthInsurancePlan { get; private set; }
    public Appointment? Appointment { get; private set; }
}

public enum AuthorizationStatus
{
    Pending,          // Aguardando análise
    Approved,         // Aprovada
    Denied,           // Negada
    Expired,          // Expirada
    Cancelled         // Cancelada
}
```

### 3.5 TissBatch (Lote TISS)

```csharp
public class TissBatch : BaseEntity
{
    public Guid ClinicId { get; private set; }
    public Guid OperatorId { get; private set; }
    public string BatchNumber { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? SubmittedDate { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public BatchStatus Status { get; private set; }
    
    // Arquivo XML
    public string? XmlFileName { get; private set; }
    public string? XmlFilePath { get; private set; }
    
    // Retorno da operadora
    public string? ProtocolNumber { get; private set; }
    public string? ResponseXmlFileName { get; private set; }
    public decimal? ApprovedAmount { get; private set; }
    public decimal? GlosedAmount { get; private set; }      // Valor glosado
    
    // Navigation
    public Clinic? Clinic { get; private set; }
    public HealthInsuranceOperator? Operator { get; private set; }
    public ICollection<TissGuide> Guides { get; private set; }
}

public enum BatchStatus
{
    Draft,            // Em elaboração
    ReadyToSend,      // Pronto para enviar
    Sent,             // Enviado
    Processing,       // Em processamento pela operadora
    Processed,        // Processado
    PartiallyPaid,    // Parcialmente pago
    Paid,             // Pago
    Rejected          // Rejeitado
}
```

### 3.6 TissGuide (Guia TISS)

```csharp
public class TissGuide : BaseEntity
{
    public Guid TissBatchId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid PatientHealthInsuranceId { get; private set; }
    public string GuideNumber { get; private set; }
    public TissGuideType GuideType { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public GuideStatus Status { get; private set; }
    
    // Autorização
    public string? AuthorizationNumber { get; private set; }
    
    // Retorno
    public decimal? ApprovedAmount { get; private set; }
    public decimal? GlosedAmount { get; private set; }
    public string? GlossReason { get; private set; }
    
    // Navigation
    public TissBatch? TissBatch { get; private set; }
    public Appointment? Appointment { get; private set; }
    public PatientHealthInsurance? PatientHealthInsurance { get; private set; }
    public ICollection<TissGuideProcedure> Procedures { get; private set; }
}

public enum TissGuideType
{
    Consultation,     // Consulta
    SPSADT,          // Serviços Profissionais / SADT
    Hospitalization, // Internação
    Fees,            // Honorários
    Dental           // Odontológico
}

public enum GuideStatus
{
    Draft,           // Rascunho
    Sent,            // Enviada
    Approved,        // Aprovada
    PartiallyApproved, // Parcialmente aprovada
    Rejected,        // Rejeitada
    Paid             // Paga
}
```

### 3.7 TissGuideProcedure (Procedimento na Guia)

```csharp
public class TissGuideProcedure : BaseEntity
{
    public Guid TissGuideId { get; private set; }
    public string ProcedureCode { get; private set; }       // Código TUSS
    public string ProcedureDescription { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    
    // Retorno
    public decimal? ApprovedQuantity { get; private set; }
    public decimal? ApprovedAmount { get; private set; }
    public decimal? GlosedAmount { get; private set; }
    public string? GlossReason { get; private set; }
    
    // Navigation
    public TissGuide? TissGuide { get; private set; }
}
```

## 4. Tabela TUSS (Terminologia Unificada da Saúde Suplementar)

A **TUSS** é a tabela de códigos padronizada para procedimentos, materiais e medicamentos no Brasil.

### 4.1 Estrutura dos Códigos TUSS

- **Procedimentos**: Código de 8 dígitos (ex: 10101012 - Consulta médica)
- **Medicamentos**: Código padronizado BRASINDICE
- **Materiais e OPME**: Códigos específicos

### 4.2 Categorias Principais

1. **Consultas Médicas** (10101xxx)
2. **Exames Laboratoriais** (40xxx)
3. **Exames de Imagem** (20xxx)
4. **Procedimentos Cirúrgicos** (30xxx)
5. **Terapias** (50xxx)

### 4.3 Implementação no Sistema

```csharp
public class TussProcedure : BaseEntity
{
    public string Code { get; private set; }               // Código TUSS
    public string Description { get; private set; }         // Descrição
    public string Category { get; private set; }            // Categoria
    public decimal ReferencePrice { get; private set; }     // Preço de referência
    public bool RequiresAuthorization { get; private set; }
    public bool IsActive { get; private set; }
}
```

## 5. Roadmap de Implementação

### Fase 1: Fundação (2-3 semanas)
- [ ] Criar entidades de domínio
- [ ] Implementar repositories e services
- [ ] Criar migrations
- [ ] Desenvolver CRUD básico de operadoras
- [ ] Desenvolver CRUD de planos de saúde
- [ ] Implementar vínculo paciente-plano
- [ ] Testes unitários

### Fase 2: Cadastro e Verificação (2-3 semanas)
- [ ] Tela de cadastro de operadoras
- [ ] Tela de cadastro de planos
- [ ] Vincular plano ao paciente no cadastro
- [ ] Verificação de elegibilidade (manual inicialmente)
- [ ] Lista de pacientes por plano
- [ ] Relatórios básicos

### Fase 3: Autorização Prévia (3-4 semanas)
- [ ] Solicitação de autorização prévia
- [ ] Acompanhamento de autorizações
- [ ] Alertas de autorização pendente
- [ ] Relatório de autorizações
- [ ] Integração com agendamento

### Fase 4: Guias TISS (4-6 semanas)
- [ ] Geração manual de guias
- [ ] Impressão de guias em PDF
- [ ] Controle de lotes
- [ ] Arquivo XML TISS básico
- [ ] Validação de guias

### Fase 5: Faturamento (4-6 semanas)
- [ ] Lotes de faturamento
- [ ] Geração de XML TISS completo
- [ ] Envio de lotes (manual ou automático)
- [ ] Processamento de retornos
- [ ] Relatórios de faturamento
- [ ] Glosas e ajustes

### Fase 6: Integrações Avançadas (6-8 semanas)
- [ ] Integração com portais de operadoras
- [ ] APIs REST de operadoras
- [ ] Verificação online de elegibilidade
- [ ] Autorização online
- [ ] Certificado digital A1/A3
- [ ] Assinatura digital de XMLs

## 6. APIs e Endpoints Necessários

### 6.1 Operadoras
```
GET    /api/health-insurance-operators          - Listar operadoras
GET    /api/health-insurance-operators/{id}     - Buscar operadora
POST   /api/health-insurance-operators          - Criar operadora
PUT    /api/health-insurance-operators/{id}     - Atualizar operadora
DELETE /api/health-insurance-operators/{id}     - Desativar operadora
```

### 6.2 Planos
```
GET    /api/health-insurance-plans                           - Listar planos
GET    /api/health-insurance-plans/{id}                      - Buscar plano
GET    /api/health-insurance-plans/operator/{operatorId}     - Planos por operadora
POST   /api/health-insurance-plans                           - Criar plano
PUT    /api/health-insurance-plans/{id}                      - Atualizar plano
DELETE /api/health-insurance-plans/{id}                      - Desativar plano
```

### 6.3 Paciente-Plano
```
GET    /api/patients/{patientId}/health-insurance            - Listar planos do paciente
POST   /api/patients/{patientId}/health-insurance            - Vincular plano ao paciente
PUT    /api/patients/{patientId}/health-insurance/{id}       - Atualizar vínculo
DELETE /api/patients/{patientId}/health-insurance/{id}       - Desativar vínculo
GET    /api/patients/{patientId}/health-insurance/validate   - Validar elegibilidade
```

### 6.4 Autorizações
```
GET    /api/authorizations                      - Listar autorizações
GET    /api/authorizations/{id}                 - Buscar autorização
POST   /api/authorizations                      - Criar solicitação
PUT    /api/authorizations/{id}/approve         - Aprovar autorização
PUT    /api/authorizations/{id}/deny            - Negar autorização
DELETE /api/authorizations/{id}                 - Cancelar autorização
```

### 6.5 Guias TISS
```
GET    /api/tiss/guides                         - Listar guias
GET    /api/tiss/guides/{id}                    - Buscar guia
POST   /api/tiss/guides                         - Criar guia
PUT    /api/tiss/guides/{id}                    - Atualizar guia
POST   /api/tiss/guides/{id}/generate-xml       - Gerar XML da guia
```

### 6.6 Lotes TISS
```
GET    /api/tiss/batches                        - Listar lotes
GET    /api/tiss/batches/{id}                   - Buscar lote
POST   /api/tiss/batches                        - Criar lote
POST   /api/tiss/batches/{id}/add-guide         - Adicionar guia ao lote
POST   /api/tiss/batches/{id}/generate-xml      - Gerar XML do lote
POST   /api/tiss/batches/{id}/submit            - Enviar lote
POST   /api/tiss/batches/{id}/process-return    - Processar retorno
```

### 6.7 Tabela TUSS
```
GET    /api/tuss/procedures                     - Listar procedimentos TUSS
GET    /api/tuss/procedures/search              - Buscar por código ou descrição
GET    /api/tuss/procedures/{code}              - Buscar procedimento
```

## 7. Integrações de Mercado

### 7.1 Principais Operadoras no Brasil

1. **Unimed** - Maior cooperativa médica do Brasil
   - Portal próprio para cada unimed
   - Suporta TISS XML
   - Algumas unidades têm API REST

2. **Bradesco Saúde**
   - Portal web robusto
   - TISS XML completo
   - API REST em desenvolvimento

3. **Amil**
   - Portal web
   - TISS XML
   - Boa documentação

4. **SulAmérica**
   - Portal web
   - TISS XML
   - API REST para verificação

5. **NotreDame Intermédica**
   - Portal web moderno
   - TISS XML
   - APIs REST disponíveis

### 7.2 Certificado Digital

**Tipos:**
- **A1**: Arquivo digital, validade 1 ano, mais prático
- **A3**: Token ou cartão, validade 1-3 anos, mais seguro

**Necessário para:**
- Assinatura de XML TISS
- Autenticação em APIs
- Envio seguro de lotes

## 8. Recursos e Links Úteis

### 8.1 Documentação Oficial

- **ANS**: https://www.ans.gov.br/
- **Padrão TISS**: https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar
- **Tabela TUSS**: https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar/padrao-tiss-componente-organizacional

### 8.2 Ferramentas

- **Validador TISS**: Ferramenta para validar XMLs
- **Biblioteca OpenTISS**: Biblioteca open-source para geração de XMLs
- **Schema XSD TISS**: Schemas XML para validação

### 8.3 Fornecedores de Soluções

Existem empresas que fornecem APIs e integrações prontas:
- Pixel TI
- iClinic (integra com várias operadoras)
- MV (soluções hospitalares)

## 9. Considerações de Segurança

### 9.1 Dados Sensíveis

- Números de carteirinha são dados sensíveis (LGPD)
- Criptografar em banco de dados
- Acesso restrito por perfil
- Logs de auditoria

### 9.2 Comunicação

- Sempre usar HTTPS
- Certificado digital válido
- Validação de certificados SSL
- Tokens de API seguros

### 9.3 Armazenamento

- XMLs assinados digitalmente
- Backup regular
- Retenção por 5 anos (exigência legal)
- Controle de acesso aos arquivos

## 10. Estimativa de Esforço Total

**Desenvolvimento completo**: 25-35 semanas (6-9 meses)

**Equipe recomendada:**
- 1 Desenvolvedor Backend Sênior
- 1 Desenvolvedor Frontend Pleno
- 1 QA/Tester
- 1 Analista de Negócios (part-time)

**Custos aproximados:**
- Certificado Digital A1: R$ 200-300/ano
- Certificado Digital A3: R$ 400-600 (1-3 anos)
- Homologação com operadoras: Geralmente gratuito
- Infraestrutura: Custo adicional mínimo

## 11. Conclusão

A integração com operadoras de planos de saúde é um projeto robusto que adiciona valor significativo ao sistema. A implementação deve ser gradual, começando com funcionalidades básicas e evoluindo para integrações mais complexas.

**Prioridades recomendadas:**
1. Cadastro e vínculo de planos (essencial)
2. Autorização prévia (importante para muitos procedimentos)
3. Geração de guias (facilita faturamento)
4. XML TISS (automação e padronização)
5. Integrações online (eficiência máxima)

**Próximos Passos:**
1. Revisar este documento com stakeholders
2. Priorizar fases de implementação
3. Começar pela Fase 1 (Fundação)
4. Avaliar parceria com fornecedores de APIs prontas
5. Considerar MVP com uma única operadora para validação

---

**Última Atualização**: 2024-11-19  
**Versão**: 1.0  
**Autor**: Omni Care Software Team
