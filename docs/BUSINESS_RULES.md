# Regras de Negócio - PrimeCare Software

## Visão Geral

O PrimeCare Software é um sistema multitenant de gestão para consultórios e clínicas médicas (SaaS) que implementa regras de negócio específicas para garantir a privacidade dos dados médicos e a flexibilidade de vínculos entre pacientes e clínicas.

## 1. Gestão de Pacientes e Vínculos com Clínicas

### 1.1 Cadastro e Vínculo de Pacientes

**Regra Principal**: Na primeira consulta, caso o paciente possua cadastro em outras clínicas, o sistema deve obter os dados pré-existentes do paciente e vincular à clínica atual.

#### Implementação

- **Entidade PatientClinicLink**: Representa o vínculo N:N entre pacientes e clínicas
  - Um paciente pode estar vinculado a múltiplas clínicas (N:N)
  - Cada vínculo possui data de criação e status ativo/inativo
  - O vínculo mantém o `TenantId` para isolamento de dados

### 1.1.1 Regras de Responsáveis para Crianças

**Regra Principal**: Pacientes menores de 18 anos (crianças) devem ter um responsável cadastrado.

#### Implementação

- **Relacionamento Guardian-Child**: Implementado como auto-relacionamento na entidade Patient
  - Uma criança pode ter um responsável (GuardianId)
  - Um responsável pode ter múltiplas crianças
  - O sistema valida automaticamente a idade para determinar se é criança (< 18 anos)

#### Validações

1. **Criação de Paciente Criança**:
   ```
   - Sistema calcula idade com base na data de nascimento
   - Se idade < 18: campo responsável torna-se obrigatório
   - Sistema valida que o responsável existe e não é criança
   - Sistema cria vínculo guardian-child automaticamente
   ```

2. **Atendimento de Crianças**:
   ```
   - Uma mãe pode levar dois filhos para consulta simultânea
   - Sistema permite visualizar todas as crianças de um responsável
   - Endpoint: GET /api/patients/{guardianId}/children
   - Facilita agendamento e atendimento conjunto
   ```

3. **Proteções do Sistema**:
   ```
   - Criança não pode ser responsável por outra criança
   - Paciente não pode ser responsável de si mesmo
   - Apenas adultos (18+) podem ser responsáveis
   - Sistema remove automaticamente vínculo quando criança completa 18 anos
   ```

#### Fluxo de Cadastro

1. **Novo Paciente sem Cadastro Prévio**:
   ```
   - Usuário acessa o formulário de cadastro de paciente
   - Preenche os dados (Nome, CPF, Data de Nascimento, etc.)
   - Sistema valida CPF (formato brasileiro de 11 dígitos)
   - Sistema verifica se CPF já existe no sistema (busca global)
   - Se não existir: cria novo paciente e vincula à clínica atual
   - Se existir: reutiliza dados existentes e cria vínculo com clínica atual
   ```

2. **Paciente com Cadastro em Outra Clínica**:
   ```
   - Sistema busca paciente por CPF usando endpoint GET /api/patients/by-document/{cpf}
   - Se encontrado: retorna dados do paciente
   - Clínica pode revisar e atualizar dados se necessário
   - Sistema cria vínculo usando POST /api/patients/{patientId}/link-clinic/{clinicId}
   - Paciente fica disponível para agendamentos na nova clínica
   ```

3. **Atualização de Dados**:
   ```
   - Qualquer clínica vinculada pode atualizar dados cadastrais do paciente
   - Alterações ficam disponíveis para todas as clínicas vinculadas
   - Histórico de alterações é mantido com timestamps (CreatedAt, UpdatedAt)
   ```

### 1.2 Busca de Pacientes

**Regra**: A consulta de pacientes deve ser feita por CPF, Nome ou Telefone.

#### Endpoints Implementados

1. **Busca Geral** (GET `/api/patients/search?searchTerm={termo}`):
   - Busca por CPF, Nome ou Telefone simultaneamente
   - Retorna pacientes vinculados à clínica atual (tenant)
   - Resultados ordenados por nome

2. **Busca por CPF Global** (GET `/api/patients/by-document/{cpf}`):
   - Busca paciente por CPF em todas as clínicas
   - Usado para verificar cadastro prévio
   - Permite reutilizar dados existentes

3. **Busca por Nome** (GET `/api/patients/search?searchTerm={nome}`):
   - Busca case-insensitive
   - Suporta busca parcial (ex: "Silva" encontra "João Silva")

4. **Busca por Telefone** (GET `/api/patients/search?searchTerm={telefone}`):
   - Aceita diferentes formatos: (11) 98765-4321, 11987654321, etc.
   - Busca apenas os dígitos do número

## 2. Isolamento de Prontuários e Histórico Médico

### 2.1 Privacidade e Segurança

**Regra Principal**: O paciente pode estar vinculado a N consultórios/clínicas, porém o prontuário e histórico médico deve estar disponível somente para a clínica respectiva. Outras clínicas não podem ter acesso ao prontuário e histórico do mesmo paciente.

#### Implementação Técnica

1. **Isolamento por Tenant (TenantId)**:
   ```csharp
   // Todos os prontuários possuem TenantId
   public class MedicalRecord : BaseEntity
   {
       public string TenantId { get; private set; }
       // ... outros campos
   }
   ```

2. **Filtros Globais no Entity Framework**:
   ```csharp
   // DbContext aplica filtro automático
   modelBuilder.Entity<MedicalRecord>()
       .HasQueryFilter(mr => EF.Property<string>(mr, "TenantId") == GetTenantId());
   ```

3. **Consultas Isoladas**:
   - GET `/api/medical-records/patient/{patientId}`: Retorna apenas prontuários da clínica atual
   - Não há endpoint cross-tenant para prontuários
   - Cada consulta é filtrada automaticamente pelo TenantId

### 2.2 Dados Compartilhados vs. Isolados

#### Dados Compartilhados (Visíveis por Todas as Clínicas Vinculadas)
- Nome do paciente
- CPF/Documento
- Data de nascimento
- Gênero
- Email
- Telefone
- Endereço
- Alergias (informação crítica de segurança)
- Status ativo/inativo

#### Dados Isolados (Apenas Clínica Específica)
- Prontuários médicos (MedicalRecord)
- Diagnósticos
- Prescrições
- Observações de consulta
- Histórico de atendimentos
- Tempo de consulta
- Datas de consultas

## 3. Adaptabilidade para Diferentes Tipos de Clínicas

**Regra**: O sistema deve ser adaptável para todo tipo de clínica médica, odontológica, psicológica, etc.

### 3.1 Implementação Flexível

1. **Templates de Prontuário** (MedicalRecordTemplate):
   ```csharp
   - Nome do template
   - Descrição
   - Conteúdo do template (campos personalizáveis)
   - Categoria (Médico, Odontológico, Psicológico, Fisioterapia, etc.)
   - Status ativo/inativo
   ```

2. **Templates de Receita** (PrescriptionTemplate):
   ```csharp
   - Nome do template
   - Descrição
   - Conteúdo da receita (formato personalizável)
   - Categoria (por especialidade)
   - Status ativo/inativo
   ```

3. **Categorias Suportadas**:
   - Clínica Médica Geral
   - Odontologia
   - Psicologia
   - Fisioterapia
   - Nutrição
   - Cardiologia
   - Pediatria
   - Outras especialidades médicas

### 3.2 Personalização por Clínica

Cada clínica (tenant) pode:
- Criar seus próprios templates de prontuário
- Definir templates de prescrição específicos
- Customizar campos de acordo com sua especialidade
- Manter biblioteca de templates reutilizáveis

## 4. Sistema de Templates

### 4.1 Templates de Prontuário Médico

**Funcionalidade**: O sistema deve permitir o cadastro de templates de prontuário.

#### Características

- **Categorização por Especialidade**: Templates organizados por categoria médica
- **Reutilização**: Templates podem ser aplicados a múltiplos atendimentos
- **Versionamento**: Histórico de alterações mantido
- **Personalização**: Cada clínica mantém seus próprios templates

#### Endpoints

- POST `/api/medical-record-templates`: Criar novo template
- GET `/api/medical-record-templates`: Listar templates da clínica
- GET `/api/medical-record-templates/category/{category}`: Filtrar por categoria
- PUT `/api/medical-record-templates/{id}`: Atualizar template
- DELETE `/api/medical-record-templates/{id}`: Desativar template

### 4.2 Templates de Receita Médica

**Funcionalidade**: O sistema deve permitir o cadastro de templates de receitas médicas.

#### Características

- **Prescrições Pré-formatadas**: Templates com formato padronizado
- **Campos Dinâmicos**: Suporte a placeholders (ex: {nome_paciente}, {data})
- **Impressão Otimizada**: Layout preparado para impressão
- **Biblioteca de Medicamentos**: Templates com medicamentos comuns

#### Endpoints

- POST `/api/prescription-templates`: Criar novo template
- GET `/api/prescription-templates`: Listar templates da clínica
- GET `/api/prescription-templates/category/{category}`: Filtrar por categoria
- PUT `/api/prescription-templates/{id}`: Atualizar template
- DELETE `/api/prescription-templates/{id}`: Desativar template

## 4.3 Gestão de Medicamentos

**Funcionalidade**: O sistema deve permitir o cadastro de medicamentos com autocomplete nas receitas.

#### Características

- **Cadastro Completo**: Nome comercial, genérico, princípio ativo, dosagem, forma farmacêutica
- **Classificação ANVISA**: Registro ANVISA, código de barras, medicamento controlado
- **Categorias**: Analgésico, Antibiótico, Anti-inflamatório, Anti-hipertensivo, etc.
- **Autocomplete**: Busca inteligente ao digitar receitas médicas
- **Itens de Prescrição**: Vínculo de medicamentos a prontuários com dosagem, frequência e duração

#### Endpoints

- POST `/api/medications`: Criar novo medicamento
- GET `/api/medications`: Listar medicamentos da clínica
- GET `/api/medications/search?term={termo}`: Buscar medicamentos (autocomplete)
- GET `/api/medications/category/{category}`: Filtrar por categoria
- PUT `/api/medications/{id}`: Atualizar medicamento
- DELETE `/api/medications/{id}`: Desativar medicamento

## 5. Timeline/Feed do Histórico do Paciente

**Regra**: O histórico do paciente deve ser exibido como um feed/timeline dentro do cadastro do mesmo.

### 5.1 Visualização Timeline

#### Estrutura do Feed

```
┌─────────────────────────────────────────┐
│ Histórico do Paciente - João Silva     │
├─────────────────────────────────────────┤
│ 🕐 15/01/2024 14:30                    │
│ Consulta de Rotina (30 min)            │
│ Diagnóstico: Hipertensão controlada     │
│ Prescrição: Losartana 50mg              │
├─────────────────────────────────────────┤
│ 🕐 10/12/2023 10:00                    │
│ Consulta de Emergência (45 min)        │
│ Diagnóstico: Gripe comum                │
│ Prescrição: Paracetamol 750mg           │
├─────────────────────────────────────────┤
│ 🕐 05/11/2023 16:15                    │
│ Exame de Rotina (20 min)                │
│ Observações: Pressão arterial normal    │
└─────────────────────────────────────────┘
```

### 5.2 Informações Exibidas no Timeline

Cada entrada mostra:
- Data e hora da consulta
- Tipo de atendimento
- Duração da consulta
- Diagnóstico resumido
- Prescrição (se houver)
- Observações relevantes
- Status (Concluída/Em andamento)

### 5.3 Ordenação e Filtros

- **Ordenação padrão**: Mais recente primeiro (DESC)
- **Filtros disponíveis**:
  - Por período (últimos 30 dias, 6 meses, 1 ano)
  - Por tipo de atendimento
  - Por diagnóstico
  - Busca por texto livre

### 5.4 Implementação Técnica

```typescript
// Frontend - Componente de Timeline
interface TimelineEntry {
  date: Date;
  type: string;
  duration: number;
  diagnosis: string;
  prescription: string;
  notes: string;
  status: string;
}

// Endpoint
GET /api/medical-records/patient/{patientId}
// Retorna array ordenado de prontuários para exibição em timeline
```

## 6. Fluxos de Trabalho

### 6.1 Fluxo Completo de Primeiro Atendimento

```
1. Recepção registra novo paciente
   ├─ Busca por CPF (GET /api/patients/by-document/{cpf})
   ├─ Se encontrado: vincula à clínica atual
   └─ Se não encontrado: cria novo cadastro

2. Sistema valida dados
   ├─ CPF válido
   ├─ Email único (por tenant)
   ├─ Campos obrigatórios preenchidos
   └─ Se menor de 18: responsável obrigatório

3. Se paciente é criança (< 18 anos)
   ├─ Sistema exibe campo de busca de responsável
   ├─ Recepcionista busca e seleciona responsável adulto
   ├─ Sistema valida que responsável não é criança
   └─ Vínculo guardian-child criado automaticamente

4. Paciente vinculado à clínica
   ├─ POST /api/patients/{patientId}/link-clinic/{clinicId}
   └─ Registro salvo com TenantId

5. Agendamento criado
   └─ Paciente disponível para consultas na clínica

5. Durante atendimento
   ├─ Médico acessa prontuário (vazio se primeira consulta)
   ├─ Visualiza dados cadastrais e alergias
   ├─ Preenche diagnóstico, prescrição e observações
   └─ Salva prontuário (isolado por TenantId)

6. Após atendimento
   ├─ Timeline atualizada com nova consulta
   ├─ Prescrição disponível para impressão
   └─ Histórico acessível apenas na clínica atual
```

### 6.2 Fluxo de Atendimento em Clínica Secundária

```
1. Paciente já cadastrado busca atendimento em nova clínica

2. Nova clínica busca por CPF
   ├─ GET /api/patients/by-document/{cpf}
   └─ Encontra paciente com dados existentes

3. Sistema exibe dados cadastrais
   ├─ Nome, CPF, contato (compartilhados)
   ├─ Alergias (importante para segurança)
   └─ Histórico médico VAZIO (isolado por clínica)

4. Clínica pode atualizar dados se paciente solicitar
   └─ PUT /api/patients/{id}

5. Vínculo criado
   └─ POST /api/patients/{patientId}/link-clinic/{clinicId}

6. Novo histórico independente inicia
   └─ Prontuários desta clínica isolados das outras
```

## 7. Segurança e Privacidade

### 7.1 Princípios de Privacidade

1. **Isolamento Total de Prontuários**:
   - Nenhuma clínica acessa prontuários de outra
   - Filtros automáticos garantem isolamento
   - Auditorias de acesso mantidas

2. **Compartilhamento Controlado**:
   - Apenas dados cadastrais básicos compartilhados
   - Informações de segurança (alergias) visíveis
   - Histórico médico sempre isolado

3. **LGPD Compliance**:
   - Consentimento do paciente para vínculo
   - Direito ao esquecimento implementado
   - Portabilidade de dados cadastrais
   - Histórico médico permanece na clínica origem

### 7.2 Auditoria e Rastreabilidade

Todos os registros mantêm:
- `CreatedAt`: Data/hora de criação
- `UpdatedAt`: Data/hora de última alteração
- `TenantId`: Identificador da clínica
- Logs de acesso e modificações

## 8. Boas Práticas de Uso

### 8.1 Para Recepcionistas

1. **Sempre buscar por CPF primeiro** antes de criar novo cadastro
2. Confirmar dados com paciente antes de vincular
3. Atualizar informações de contato se mudaram
4. Registrar alergias imediatamente (informação crítica)

### 8.2 Para Médicos

1. Revisar alergias antes de prescrever
2. Usar templates para agilizar preenchimento
3. Preencher diagnóstico completo para histórico
4. Utilizar timeline para consultar atendimentos anteriores
5. Lembrar que histórico não inclui outras clínicas

### 8.3 Para Administradores

1. Criar templates padrão para especialidade da clínica
2. Revisar e atualizar templates periodicamente
3. Treinar equipe sobre privacidade de dados
4. Monitorar vínculos de pacientes
5. Realizar backups regulares dos dados

## 9. Benefícios do Sistema

### 9.1 Para Pacientes

- Cadastro único reutilizável em múltiplas clínicas
- Não precisa repetir dados básicos
- Privacidade do histórico médico garantida
- Fácil portabilidade entre clínicas

### 9.2 Para Clínicas

- Redução de tempo no cadastro de pacientes
- Dados sempre atualizados
- Histórico organizado em timeline
- Templates agilizam atendimento
- Sistema adaptável à especialidade

### 9.3 Para o Sistema de Saúde

- Dados mais precisos e consistentes
- Redução de duplicidade
- Privacidade respeitada conforme LGPD
- Interoperabilidade entre clínicas (dados cadastrais)
- Histórico médico protegido por isolamento

## 10. Perguntas Frequentes (FAQ)

### Q1: O que acontece se um paciente quiser que uma clínica acesse seu histórico de outra clínica?

**R**: Por questões de privacidade e LGPD, cada clínica mantém seu próprio prontuário isolado. O paciente pode solicitar uma cópia do prontuário de uma clínica e apresentar à outra clínica, que pode registrar as informações relevantes em seu próprio sistema.

### Q2: Posso desvincular um paciente de uma clínica?

**R**: Sim, o vínculo pode ser desativado, mas o histórico médico da clínica é mantido para fins de auditoria e conformidade legal.

### Q3: Como funcionam as alergias se são compartilhadas?

**R**: As alergias são informações críticas de segurança e são compartilhadas entre todas as clínicas vinculadas para prevenir prescrições perigosas. Qualquer clínica pode atualizar as alergias do paciente.

### Q4: Posso criar templates específicos para minha especialidade?

**R**: Sim! Cada clínica pode criar quantos templates desejar, organizados por categoria. Os templates são isolados por clínica (tenant).

### Q5: O sistema funciona offline?

**R**: Não, o sistema requer conexão com internet para funcionar, pois é uma aplicação web SaaS baseada em nuvem.

### Q6: Como faço para migrar dados de outro sistema?

**R**: O sistema oferece APIs REST que podem ser usadas para importação de dados. Contate o suporte técnico para assistência na migração.

## 11. Suporte e Contato

Para dúvidas, sugestões ou suporte técnico:

- **Email**: contato@primecaresoftware.com
- **Documentação Técnica**: Consulte README.md e IMPLEMENTATION.md
- **Issues**: https://github.com/PrimeCare Software/MW.Code/issues

---

**Última Atualização**: Janeiro 2025  
**Versão do Documento**: 1.0  
**Autor**: Equipe PrimeCare Software

## 6. Sistema de Assinaturas e Cobrança

**Regra**: O sistema deve oferecer período de teste gratuito de 15 dias e planos pagos com diferentes recursos.

### 6.1 Planos de Assinatura

- **Trial (Teste)**: 15 dias gratuitos com recursos limitados
- **Basic**: Plano básico para pequenas clínicas
- **Standard**: Plano padrão com recursos intermediários
- **Premium**: Plano completo com todos os recursos
- **Enterprise**: Plano customizado para grandes organizações

### 6.2 Gestão de Assinaturas

Estados da Assinatura: **Trial** → **Active** → **Suspended/PaymentOverdue** → **Cancelled**

## 6.5 Sistema de Pagamentos para Consultas

**Regra**: O sistema deve permitir registro de pagamentos de consultas com múltiplos métodos de pagamento.

### 6.5.1 Métodos de Pagamento Suportados

- **Dinheiro (Cash)**: Pagamento em espécie
- **Cartão de Crédito (CreditCard)**: Com armazenamento dos últimos 4 dígitos
- **Cartão de Débito (DebitCard)**: Com armazenamento dos últimos 4 dígitos
- **PIX**: Com chave PIX e ID da transação
- **Transferência Bancária (BankTransfer)**
- **Cheque (Check)**

### 6.5.2 Fluxo de Pagamento

Estados do Pagamento: **Pending** → **Processing** → **Paid** | **Failed** | **Refunded** | **Cancelled**

- Pagamentos começam como **Pending** ao serem criados
- Podem ser marcados como **Processing** durante o processamento
- Mudam para **Paid** quando confirmados com Transaction ID
- Podem ser **Refunded** apenas se estiverem **Paid**
- Podem ser **Cancelled** apenas se estiverem **Pending** ou **Failed**

### 6.5.3 Regras de Negócio para Pagamentos

1. Todo pagamento deve estar vinculado a uma consulta ou assinatura
2. O valor do pagamento deve ser maior que zero
3. Pagamentos pagos não podem ser cancelados (apenas reembolsados)
4. Reembolsos exigem motivo obrigatório
5. Cancelamentos exigem motivo obrigatório
6. Pagamentos com cartão devem armazenar apenas os últimos 4 dígitos
7. Pagamentos PIX devem armazenar a chave PIX utilizada

## 6.6 Sistema de Emissão de Nota Fiscal

**Regra**: O sistema deve emitir notas fiscais para pagamentos de consultas e assinaturas.

### 6.6.1 Tipos de Nota Fiscal

- **Appointment**: Nota fiscal de consulta médica
- **Subscription**: Nota fiscal de assinatura do sistema
- **Service**: Nota fiscal de serviços adicionais

### 6.6.2 Fluxo de Nota Fiscal

Estados da Nota Fiscal: **Draft** → **Issued** → **Sent** → **Paid** | **Cancelled** | **Overdue**

- Notas fiscais começam como **Draft** ao serem criadas
- Devem ser **Issued** (emitidas) para serem válidas
- Podem ser marcadas como **Sent** quando enviadas ao cliente
- Mudam para **Paid** quando o pagamento é confirmado
- Tornam-se **Overdue** automaticamente após a data de vencimento
- Podem ser **Cancelled** se não estiverem pagas

### 6.6.3 Regras de Negócio para Nota Fiscal

1. Toda nota fiscal deve estar vinculada a um pagamento único
2. Não pode haver mais de uma nota fiscal para o mesmo pagamento
3. Número da nota fiscal deve ser único no sistema
4. Notas pagas não podem ser canceladas
5. Notas em rascunho podem ter valor e descrição alterados
6. Notas emitidas não podem ser editadas
7. Sistema calcula automaticamente dias até vencimento e dias vencidos
8. Campos do cliente (nome, documento, endereço) são desnormalizados para histórico

### 6.6.4 Informações da Nota Fiscal

- Número da nota fiscal (único)
- Data de emissão
- Data de vencimento
- Valor base
- Valor de impostos
- Valor total (base + impostos)
- Descrição do serviço
- Dados do cliente (nome, documento, endereço)

## 7. Sistema de Notificações

**Regra**: O sistema deve enviar notificações automáticas via SMS e WhatsApp para confirmar agendamentos.

### 7.1 Canais: SMS, WhatsApp, Email, Push

### 7.2 Tipos: Lembrete de Consulta (24h antes), Confirmação, Cancelamento, Reagendamento

### 7.3 Máximo de 3 tentativas para notificações falhadas com log completo

### 7.4 Rotinas de Notificação Configuráveis

**Funcionalidade**: Sistema de rotinas automatizadas e personalizáveis para envio de notificações.

#### Características

- **Múltiplos Canais**: SMS, WhatsApp, Email, Push
- **Tipos de Notificação**: Lembretes, confirmações, cancelamentos, avisos de pagamento
- **Agendamento Flexível**: Diário, semanal, mensal, customizado, antes/depois de eventos
- **Templates Personalizáveis**: Mensagens com placeholders dinâmicos
- **Filtros de Destinatários**: Segmentação baseada em critérios configuráveis
- **Escopo Configurável**: Clínica ou Sistema (admin)
- **Retentativas**: Até 10 tentativas configuráveis
- **Multi-tenant**: Isolamento por clínica

#### Tipos de Agendamento

1. **Daily**: Execução diária em horário específico
2. **Weekly**: Execução em dias específicos da semana
3. **Monthly**: Execução em dia específico do mês
4. **Custom**: Expressão customizada (tipo cron)
5. **BeforeAppointment**: X horas/dias antes da consulta
6. **AfterAppointment**: X horas/dias depois da consulta

#### Endpoints

- POST `/api/notificationroutines`: Criar nova rotina
- GET `/api/notificationroutines`: Listar todas as rotinas
- GET `/api/notificationroutines/active`: Listar rotinas ativas
- GET `/api/notificationroutines/{id}`: Obter rotina específica
- PUT `/api/notificationroutines/{id}`: Atualizar rotina
- DELETE `/api/notificationroutines/{id}`: Excluir rotina
- POST `/api/notificationroutines/{id}/activate`: Ativar rotina
- POST `/api/notificationroutines/{id}/deactivate`: Desativar rotina

#### Exemplo de Uso

```json
{
  "name": "Lembrete WhatsApp 24h Antes",
  "description": "Envia lembrete via WhatsApp 24 horas antes da consulta",
  "channel": "WhatsApp",
  "type": "AppointmentReminder",
  "messageTemplate": "Olá {patientName}! Lembrete: você tem consulta amanhã às {appointmentTime} com Dr(a). {doctorName}.",
  "scheduleType": "Daily",
  "scheduleConfiguration": "{\"time\":\"18:00\"}",
  "scope": "Clinic",
  "maxRetries": 3,
  "recipientFilter": "{\"hasAppointmentNextDay\":true}"
}
```

Para documentação completa, consulte: [NOTIFICATION_ROUTINES_DOCUMENTATION.md](NOTIFICATION_ROUTINES_DOCUMENTATION.md)

## 8. Procedimentos e Serviços

**Regra**: Cadastro de procedimentos/serviços, vínculo com materiais e registro na consulta.

### 8.1 Procedimentos: Nome, código, categoria, preço, duração, materiais

**Entidade**: `Procedure`

Representa um procedimento/serviço oferecido pela clínica.

#### Propriedades:
- **Name**: Nome do procedimento (ex: "Consulta Médica Geral")
- **Code**: Código único (ex: "CONS-001")
- **Description**: Descrição detalhada
- **Category**: Categoria do procedimento
  - Consultation (Consulta)
  - Exam (Exame)
  - Surgery (Cirurgia)
  - Therapy (Terapia)
  - Vaccination (Vacinação)
  - Diagnostic (Diagnóstico)
  - Treatment (Tratamento)
  - Emergency (Emergência)
  - Prevention (Prevenção)
  - Aesthetic (Estética)
  - FollowUp (Retorno)
  - Other (Outros)
- **Price**: Preço padrão do procedimento
- **DurationMinutes**: Duração estimada em minutos
- **RequiresMaterials**: Indica se requer materiais
- **IsActive**: Status ativo/inativo

#### Endpoints API:
```
GET    /api/procedures                           # Listar procedimentos
GET    /api/procedures/{id}                      # Obter por ID
POST   /api/procedures                           # Criar novo
PUT    /api/procedures/{id}                      # Atualizar
DELETE /api/procedures/{id}                      # Desativar
```

### 8.2 Materiais: Controle de estoque com entrada/saída e alertas

**Entidade**: `Material`

Representa materiais/insumos utilizados em procedimentos.

#### Propriedades:
- **Name**: Nome do material
- **Code**: Código único
- **Unit**: Unidade de medida (caixa, frasco, unidade, etc.)
- **UnitPrice**: Preço unitário
- **StockQuantity**: Quantidade em estoque
- **MinimumStock**: Estoque mínimo para alertas

### 8.3 Vínculo: Procedimento + Consulta + Paciente com dedução de estoque

**Entidade**: `AppointmentProcedure`

Vincula procedimentos realizados durante um atendimento.

#### Propriedades:
- **AppointmentId**: ID do agendamento
- **ProcedureId**: ID do procedimento realizado
- **PatientId**: ID do paciente
- **PriceCharged**: Preço cobrado (pode ser diferente do padrão)
- **PerformedAt**: Data/hora da realização
- **Notes**: Observações

#### Endpoints API:
```
POST /api/procedures/appointments/{appointmentId}/procedures     # Adicionar procedimento
GET  /api/procedures/appointments/{appointmentId}/procedures     # Listar procedimentos
GET  /api/procedures/appointments/{appointmentId}/billing-summary # Resumo de cobrança
```

### 8.4 Fechamento de Atendimento e Billing

**Regra**: Ao finalizar um atendimento, o sistema deve calcular o total baseado nos procedimentos realizados.

#### Fluxo de Fechamento:

```
1. Durante o Atendimento
   ├─ Médico/Dentista realiza procedimentos
   ├─ POST /api/procedures/appointments/{id}/procedures
   ├─ Sistema registra cada procedimento com preço
   └─ Procedimentos vinculados ao atendimento

2. Fechamento por Médico ou Recepcionista
   ├─ Acessa resumo de cobrança
   ├─ GET /api/procedures/appointments/{id}/billing-summary
   └─ Sistema retorna:
      ├─ Lista de procedimentos realizados
      ├─ Subtotal (soma dos procedimentos)
      ├─ Impostos (se aplicável)
      ├─ Total a pagar
      └─ Status do pagamento

3. Exemplo de Resposta:
{
  "appointmentId": "guid",
  "patientId": "guid",
  "patientName": "João Silva",
  "appointmentDate": "2024-01-15T10:00:00Z",
  "procedures": [
    {
      "procedureName": "Consulta Médica Geral",
      "procedureCode": "CONS-001",
      "priceCharged": 150.00,
      "performedAt": "2024-01-15T10:00:00Z",
      "notes": "Consulta realizada"
    },
    {
      "procedureName": "Eletrocardiograma",
      "procedureCode": "EXAM-002",
      "priceCharged": 120.00,
      "performedAt": "2024-01-15T10:30:00Z",
      "notes": "ECG normal"
    }
  ],
  "subTotal": 270.00,
  "taxAmount": 0.00,
  "total": 270.00,
  "paymentStatus": "Pending"
}

4. Processamento do Pagamento
   ├─ POST /api/payments
   ├─ Vincula ao appointmentId
   ├─ Registra método de pagamento
   └─ Atualiza status para "Paid"
```

#### Permissões:
- **Médico/Dentista**: Pode adicionar procedimentos e fechar atendimento
- **Recepcionista**: Pode visualizar resumo e processar pagamento
- **Secretário**: Pode visualizar resumo e processar pagamento

### 8.2 Materiais: Controle de estoque com entrada/saída e alertas

### 8.3 Vínculo: Procedimento + Consulta + Paciente com dedução de estoque

## 9. Painel de Administração

### 9.1 Painel do Dono da Clínica
- Gestão de usuários e permissões
- Configurações da clínica
- Relatórios gerenciais e financeiros

### 9.2 Painel do Administrador do Sistema
- Gestão de todas as clínicas
- Gestão de assinaturas e planos
- Analytics e BI global
- Acesso cross-tenant para auditoria

---

## 7. Fluxo de Atendimento de Crianças com Responsável

### 7.1 Cenário: Mãe com Dois Filhos

**Situação**: Uma mãe leva seus dois filhos menores para consulta.

#### Fluxo Detalhado

```
1. Cadastro do Responsável (Mãe)
   ├─ Recepção cadastra a mãe como paciente adulto
   ├─ CPF, nome, dados de contato, endereço
   └─ Paciente ID: [GUID-MAE]

2. Cadastro da Primeira Criança
   ├─ Sistema calcula idade: 8 anos (< 18)
   ├─ Campo "Responsável" torna-se obrigatório
   ├─ Recepcionista busca e seleciona a mãe
   ├─ Sistema cria vínculo: GuardianId = [GUID-MAE]
   └─ Criança ID: [GUID-FILHO1]

3. Cadastro da Segunda Criança
   ├─ Sistema calcula idade: 5 anos (< 18)
   ├─ Recepcionista busca e seleciona a mãe
   ├─ Sistema cria vínculo: GuardianId = [GUID-MAE]
   └─ Criança ID: [GUID-FILHO2]

4. Agendamento Conjunto
   ├─ Sistema permite visualizar filhos da mãe
   ├─ GET /api/patients/{GUID-MAE}/children
   ├─ Retorna lista: [FILHO1, FILHO2]
   ├─ Recepcionista agenda consultas próximas
   └─ Facilita atendimento sequencial ou simultâneo

5. Durante o Atendimento
   ├─ Médico pode ver que são irmãos (mesmo GuardianId)
   ├─ Informações do responsável disponíveis
   ├─ Histórico mantido separado por criança
   └─ Prescrições individuais por paciente
```

### 7.2 Endpoints para Responsáveis

1. **Vincular Criança a Responsável**:
   ```
   POST /api/patients/{childId}/link-guardian/{guardianId}
   
   Validações:
   - Criança deve ter menos de 18 anos
   - Responsável deve ter 18 anos ou mais
   - Ambos devem estar no mesmo tenant
   ```

2. **Listar Filhos de um Responsável**:
   ```
   GET /api/patients/{guardianId}/children
   
   Retorna:
   - Array de pacientes menores de 18 anos
   - Dados completos de cada criança
   - Ordenados por idade (mais velho primeiro)
   ```

3. **Criar Paciente com Responsável**:
   ```
   POST /api/patients
   Body: {
     name: "João Silva",
     dateOfBirth: "2015-03-10",
     guardianId: "[GUID-DO-RESPONSAVEL]",
     ...outros campos
   }
   ```

### 7.3 Benefícios do Sistema

1. **Organização Familiar**:
   - Visualização clara de vínculos familiares
   - Facilita agendamento de consultas conjuntas
   - Responsável recebe notificações de todos os filhos

2. **Segurança e Compliance**:
   - Garantia de que crianças têm responsável identificado
   - Rastreabilidade de autorização de atendimento
   - Contato de emergência sempre disponível

3. **Eficiência Operacional**:
   - Atendimento mais rápido de famílias
   - Dados do responsável compartilhados entre filhos
   - Redução de duplicação de informações

---

## 📱 Documentação Visual de Interfaces

Para visualizar os fluxos de trabalho completos com mockups de telas e diagramas interativos, consulte:

### [SCREENS_DOCUMENTATION.md](SCREENS_DOCUMENTATION.md)

Este documento complementar contém:
- **Mockups ASCII** de todas as telas do sistema
- **Diagramas Mermaid** com fluxos de navegação
- **Descrição detalhada** de cada interface
- **Estados e transições** dos agendamentos
- **Validações** e regras de cada formulário

**Principais fluxos visuais documentados:**
1. Fluxo de Primeiro Atendimento (novo paciente)
2. Fluxo de Atendimento Recorrente (paciente existente)
3. Fluxo de Busca e Vínculo (paciente de outra clínica)
4. Estados dos Agendamentos (Agendado → Em Atendimento → Concluído)
5. **NOVO**: Cadastro de Crianças com Responsável

A documentação visual complementa as regras de negócio descritas neste documento, mostrando como elas se manifestam na interface do usuário.

---

**Data**: Janeiro 2025  
**Versão**: 1.1  
**Equipe**: PrimeCare Software
