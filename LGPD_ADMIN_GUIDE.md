# 🛡️ Guia do Administrador - Sistema de Auditoria LGPD

**PrimeCare Software - System Admin**  
**Versão:** 1.0  
**Data:** 29 de Janeiro de 2026  
**Público-alvo:** Administradores de Sistema, DPO, Equipe de Compliance

---

## 📋 Sumário

1. [Introdução](#introdução)
2. [Visão Geral do Sistema](#visão-geral-do-sistema)
3. [Logs de Auditoria](#logs-de-auditoria)
4. [Gestão de Consentimentos](#gestão-de-consentimentos)
5. [Requisições de Exclusão de Dados](#requisições-de-exclusão-de-dados)
6. [Dashboard de Compliance LGPD](#dashboard-de-compliance-lgpd)
7. [Relatórios para ANPD](#relatórios-para-anpd)
8. [Gestão de Incidentes](#gestão-de-incidentes)
9. [Melhores Práticas](#melhores-práticas)
10. [Solução de Problemas](#solução-de-problemas)

---

## 🎯 Introdução

Este guia fornece instruções completas para administradores do sistema PrimeCare sobre como gerenciar o sistema de auditoria LGPD, processar solicitações de titulares de dados e garantir compliance contínuo com a Lei 13.709/2018.

### Responsabilidades do Administrador

Como administrador, você é responsável por:

- ✅ Monitorar logs de auditoria
- ✅ Processar requisições de exclusão de dados
- ✅ Gerenciar consentimentos
- ✅ Gerar relatórios de compliance
- ✅ Responder a solicitações LGPD em até 15 dias
- ✅ Identificar e reportar incidentes de segurança
- ✅ Manter documentação atualizada

### Permissões Necessárias

Para acessar as funcionalidades de auditoria LGPD, você precisa ter:

- 🔐 **Role:** System Admin
- 🔐 **Permissões:**
  - `audit.view` - Visualizar logs de auditoria
  - `audit.export` - Exportar logs
  - `consent.manage` - Gerenciar consentimentos
  - `data-deletion.process` - Processar requisições de exclusão
  - `lgpd.reports` - Gerar relatórios LGPD

---

## 🌐 Visão Geral do Sistema

### Arquitetura LGPD

```
┌─────────────────────────────────────────────────┐
│           Frontend (System Admin)                │
│  • Logs de Auditoria                            │
│  • Dashboard LGPD                               │
│  • Gestão de Consentimentos                     │
│  • Requisições de Exclusão                      │
└────────────────┬────────────────────────────────┘
                 │
                 │ HTTPS/TLS 1.3
                 │
┌────────────────▼────────────────────────────────┐
│              Backend API                         │
│  • AuditController                              │
│  • ConsentController                            │
│  • DataDeletionController                       │
│  • DataPortabilityController                    │
└────────────────┬────────────────────────────────┘
                 │
                 │ LgpdAuditMiddleware (Automático)
                 │
┌────────────────▼────────────────────────────────┐
│           Services Layer                         │
│  • AuditService                                 │
│  • ConsentManagementService                     │
│  • DataDeletionService                          │
│  • DataPortabilityService                       │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│           Database (SQL Server)                  │
│  • AuditLog (Criptografado)                     │
│  • DataConsentLog                               │
│  • DataAccessLog                                │
│  • DataDeletionRequest                          │
└─────────────────────────────────────────────────┘
```

### Componentes Principais

1. **LgpdAuditMiddleware** - Registra automaticamente todas as operações
2. **AuditService** - Gerencia logs de auditoria
3. **ConsentManagementService** - Gerencia consentimentos
4. **DataDeletionService** - Processa exclusões
5. **DataPortabilityService** - Gera exportações

---

## 📊 Logs de Auditoria

### Acessando Logs

1. **Login** no System Admin
2. **Navegue:** Menu → **Monitoramento e Segurança** → **Logs de Auditoria**
3. **Visualize** os logs mais recentes (últimos 7 dias por padrão)

### Interface de Logs

#### Filtros Disponíveis

A interface oferece filtros poderosos para encontrar logs específicos:

**Filtros de Data:**
- Data Inicial
- Data Final
- Presets: Hoje, Últimos 7 dias, Últimos 30 dias, Personalizado

**Filtros de Usuário:**
- ID do Usuário
- Nome do Usuário
- Email do Usuário

**Filtros de Entidade:**
- Tipo de Entidade (Patient, User, Clinic, MedicalRecord, etc.)
- ID da Entidade

**Filtros de Ação:**
- CREATE - Criação de registros
- READ - Leitura de dados
- UPDATE - Atualização de registros
- DELETE - Exclusão de registros
- LOGIN - Autenticação
- LOGOUT - Saída do sistema
- LOGIN_FAILED - Tentativas falhas
- EXPORT - Exportação de dados
- E mais...

**Filtros de Resultado:**
- SUCCESS - Operação bem-sucedida
- FAILED - Operação falhou
- UNAUTHORIZED - Acesso negado
- PARTIAL_SUCCESS - Sucesso parcial

**Filtros de Severidade:**
- INFO - Informativo
- WARNING - Aviso
- ERROR - Erro
- CRITICAL - Crítico (requer atenção imediata)

#### Visualizando Detalhes

Para ver detalhes completos de um log:

1. **Clique** no ícone 👁️ (olho) na coluna "Ações"
2. **Modal abre** com informações detalhadas:
   - Informações gerais (data, ação, resultado, severidade)
   - Dados do usuário (nome, email, IP)
   - Entidade afetada (tipo, ID, descrição)
   - Detalhes da requisição (método HTTP, caminho, status)
   - Alterações (valores antigos vs novos)
   - Informações LGPD (categoria de dados, finalidade)
   - User Agent completo

#### Exportando Logs

**Opção 1: CSV**
- Clique em **"Exportar CSV"**
- Formato tabular
- Abre facilmente no Excel
- Inclui campos principais

**Opção 2: JSON**
- Clique em **"Exportar JSON"**
- Formato estruturado completo
- Inclui todos os campos
- Ideal para processamento automatizado

**Nome do arquivo:** `audit-logs-[data-hora].csv` ou `.json`

### Casos de Uso Comuns

#### 1. Investigar Acesso Não Autorizado

**Cenário:** Suspeita de acesso indevido a dados de paciente.

**Passos:**
1. Filtre por **Tipo de Entidade:** `Patient`
2. Filtre por **ID da Entidade:** [ID do paciente]
3. Filtre por **Resultado:** `UNAUTHORIZED`
4. Veja todos os acessos negados
5. Identifique padrões suspeitos
6. **Exporte** logs para documentação

#### 2. Auditoria de Atividades de Usuário

**Cenário:** Revisar ações de um usuário específico.

**Passos:**
1. Filtre por **ID do Usuário** ou **Email do Usuário**
2. Defina **Período** desejado
3. Veja histórico completo de ações
4. Identifique comportamentos anormais

#### 3. Rastrear Alterações em Registro

**Cenário:** Descobrir quem alterou um registro específico.

**Passos:**
1. Filtre por **Tipo de Entidade** (ex: `MedicalRecord`)
2. Filtre por **ID da Entidade** (ex: `123`)
3. Filtre por **Ação:** `UPDATE`
4. Veja quem alterou, quando e o quê
5. Compare valores antigos vs novos

#### 4. Monitorar Eventos de Segurança

**Cenário:** Revisar tentativas de login falhas.

**Passos:**
1. Filtre por **Ação:** `LOGIN_FAILED`
2. Defina **Período:** Últimos 30 dias
3. Filtre por **Severidade:** `WARNING` ou `ERROR`
4. Identifique tentativas de força bruta
5. Tome ações preventivas

#### 5. Preparar Relatório ANPD

**Cenário:** ANPD solicitou relatório de acessos a dados sensíveis.

**Passos:**
1. Filtre por **DataCategory:** `SENSITIVE`
2. Defina período solicitado
3. Filtre por **LgpdPurpose** se necessário
4. **Exporte** em CSV ou JSON
5. Compile relatório formal

### Interpretando Logs

#### Campos Importantes

**Timestamp:**
- Data e hora exata da operação
- Fuso horário: UTC-3 (Brasília)
- Formato: YYYY-MM-DD HH:mm:ss

**Action:**
- Identifica tipo de operação
- Ícones visuais facilitam identificação
- Agrupa operações similares

**EntityType e EntityId:**
- Identifica o recurso afetado
- EntityType: Tipo (Patient, User, etc.)
- EntityId: ID único do recurso

**Result:**
- SUCCESS: Operação concluída com sucesso
- FAILED: Operação falhou (veja FailureReason)
- UNAUTHORIZED: Acesso negado (veja DenialReason)

**Severity:**
- INFO: Operação normal
- WARNING: Atenção necessária
- ERROR: Erro que afetou operação
- CRITICAL: Incidente de segurança

**DataCategory (LGPD):**
- PUBLIC: Dados públicos
- PERSONAL: Dados pessoais comuns
- SENSITIVE: Dados sensíveis (saúde, biometria)
- CONFIDENTIAL: Dados confidenciais

**LgpdPurpose (LGPD):**
- HEALTHCARE: Prestação de serviços de saúde
- BILLING: Faturamento
- CONSENT: Gestão de consentimentos
- LEGAL_OBLIGATION: Cumprimento de obrigação legal
- LEGITIMATE_INTEREST: Legítimo interesse

#### Alertas Automáticos

🚨 **Fique atento a:**

- Múltiplas tentativas de login falhas (força bruta)
- Acessos fora do horário comercial a dados sensíveis
- Volume anormal de acessos READ de um único usuário
- Acessos UNAUTHORIZED repetidos
- Ações DELETE em massa
- Logs com Severity CRITICAL

---

## ✋ Gestão de Consentimentos

### Visão Geral

Consentimentos são autorizações que pacientes dão para uso de seus dados. A LGPD exige que consentimentos sejam:

- ✅ Livres (sem coerção)
- ✅ Informados (claros e específicos)
- ✅ Inequívocos (sem ambiguidade)
- ✅ Revogáveis (a qualquer momento)

### Acessando Gestão de Consentimentos

1. **Login** no System Admin
2. **Navegue:** Menu → **LGPD** → **Consentimentos**
3. **Visualize** lista de todos os consentimentos

### Interface de Consentimentos

#### Lista de Consentimentos

**Colunas principais:**
- Paciente (nome e CPF)
- Tipo de Consentimento
- Finalidade
- Status (Active, Revoked, Expired)
- Data de Consentimento
- Data de Revogação (se aplicável)
- Ações

#### Filtros

- **Por Paciente:** Buscar por nome ou CPF
- **Por Tipo:** Marketing, Newsletter, Pesquisa, etc.
- **Por Status:** Ativos, Revogados, Expirados
- **Por Período:** Data de consentimento

#### Visualizar Detalhes

Clique em um consentimento para ver:
- **Texto do termo** completo
- **Versão do termo**
- **Método de obtenção** (WEB, MOBILE, PAPER)
- **IP Address** de onde foi dado
- **User Agent**
- **Histórico de alterações**

### Tipos de Consentimento

#### 1. Consentimentos Médicos (Obrigatórios)

**Tratamento Médico:**
- Necessário para prestar atendimento
- Não pode ser revogado enquanto for paciente
- Base legal: Tutela da Saúde (Art. 11, II, f)

**Prontuário Médico:**
- Obrigatório por lei CFM
- Mantido por 20 anos
- Base legal: Obrigação Legal (Art. 7, II)

#### 2. Consentimentos Opcionais

**Marketing e Promoções:**
- Receber ofertas comerciais
- Pode ser revogado a qualquer momento
- Base legal: Consentimento (Art. 7, I)

**Newsletter:**
- Receber informações por email
- Pode ser revogado
- Base legal: Consentimento

**Pesquisa e Estudos:**
- Uso anonimizado para pesquisa
- Pode ser revogado
- Base legal: Legítimo Interesse (Art. 7, IX)

**Compartilhamento com Parceiros:**
- Laboratórios, convênios
- Pode ser revogado
- Base legal: Consentimento

### Processando Revogações

#### Quando Paciente Revoga

**Passos:**
1. **Notificação automática** para administrador
2. **Acesse** lista de consentimentos
3. **Filtre** por Status: `Revoked`
4. **Veja** consentimento revogado
5. **Tome ações:**
   - Marketing revogado → Remover de listas de email
   - Newsletter revogada → Desinscrever
   - Pesquisa revogada → Marcar dados como não utilizáveis

#### Revogação Manual (Admin)

**Cenário:** Paciente solicitou por telefone/presencial.

**Passos:**
1. **Acesse** lista de consentimentos
2. **Localize** consentimento do paciente
3. **Clique** em "Revogar"
4. **Preencha motivo** (ex: "Solicitado por telefone")
5. **Confirme** revogação
6. **Sistema registra** automaticamente em AuditLog

### Renovação de Consentimentos

**Quando renovar:**
- Consentimentos expirados
- Mudança nos termos
- Novas finalidades de tratamento

**Passos:**
1. **Identifique** consentimentos expirados
2. **Prepare novo termo** (se houve mudanças)
3. **Solicite ao paciente** novo consentimento
4. **Registre** novo consentimento no sistema

### Relatórios de Consentimento

**Métricas importantes:**
- Taxa de aceitação por tipo
- Taxa de revogação
- Consentimentos expirados
- Consentimentos ativos por finalidade

**Gerar relatório:**
1. Dashboard LGPD → **Métricas de Consentimento**
2. Escolha período
3. Exporte CSV ou PDF

---

## 🗑️ Requisições de Exclusão de Dados

### Visão Geral

Pacientes têm direito ao esquecimento (Art. 18, VI da LGPD), mas existem limitações legais:

- ⚖️ **Prontuários médicos:** 20 anos (CFM 1.821/2007)
- ⚖️ **Notas fiscais:** 5 anos (Código Tributário)
- ⚖️ **Dados em processos judiciais:** Até fim do processo

### Acessando Requisições

1. **Login** no System Admin
2. **Navegue:** Menu → **LGPD** → **Requisições de Exclusão**
3. **Visualize** lista de requisições pendentes

### Interface de Requisições

#### Lista de Requisições

**Colunas principais:**
- Paciente (nome e CPF)
- Tipo (Complete, Anonymization, Partial)
- Motivo
- Status
- Data da Solicitação
- Prazo (15 dias)
- Ações

#### Status de Requisição

```
🟡 Pending       → 🔵 Processing    → 🟢 LegalApproval → ✅ Completed
                                    ↘ ❌ Rejected
```

**Pending:**
- Requisição recebida
- Aguardando análise
- Prazo de 15 dias correndo

**Processing:**
- Em análise pela equipe
- Verificando obrigações legais
- Preparando documentação

**LegalApproval:**
- Aguardando aprovação legal (DPO)
- Requisições complexas
- Dados em processo judicial

**Completed:**
- Dados anonimizados/excluídos
- Paciente notificado
- Log criado

**Rejected:**
- Requisição negada
- Motivo informado ao paciente
- Possibilidade de recurso

### Processando Requisições

#### Passo 1: Análise Inicial

**Quando uma nova requisição chega:**

1. **Notificação automática** para administrador
2. **Acesse** lista de requisições pendentes
3. **Clique** na requisição para ver detalhes:
   - Informações do paciente
   - Tipo de exclusão solicitada
   - Motivo (se fornecido)
   - Data da solicitação

#### Passo 2: Verificação de Obrigações Legais

**Perguntas a fazer:**

✅ **Há prontuários médicos?**
- SIM → Manter por 20 anos (CFM 1.821/2007)
- Ação: Anonimizar, não excluir

✅ **Há notas fiscais?**
- SIM → Manter por 5 anos (Código Tributário)
- Ação: Manter dados de faturamento

✅ **Há processos judiciais em andamento?**
- SIM → Manter até fim do processo
- Ação: Marcar requisição para "LegalApproval"

✅ **Há investigações de segurança?**
- SIM → Consultar jurídico
- Ação: Aguardar conclusão

#### Passo 3: Processar Requisição

**Na interface:**

1. **Clique** em "Processar"
2. **Revise** obrigações legais
3. **Adicione notas internas:**
   ```
   Exemplo:
   - Verificado: 3 prontuários médicos (2020-2023)
   - Ação: Anonimizar dados pessoais
   - Manter: Prontuários por 20 anos (até 2043)
   - Excluir: Dados de marketing, preferências
   ```
4. **Clique** em "Salvar e Avançar"
5. **Status muda** para "Processing"

#### Passo 4: Aprovação (se necessária)

**Se a requisição é simples:**
- Avance direto para "Completar"

**Se a requisição é complexa:**
1. **Clique** em "Solicitar Aprovação Legal"
2. **DPO/Jurídico recebe notificação**
3. **Aguarde aprovação**
4. **Status** → "LegalApproval"

#### Passo 5: Completar Exclusão

**Após aprovação (ou se não necessária):**

1. **Clique** em "Completar Exclusão"
2. **Escolha método:**
   - **Anonimização** (Recomendado)
   - **Exclusão Parcial**
   - **Exclusão Completa** (aguardar prazos legais)
3. **Confirme ação**
4. **Sistema executa:**
   - Anonimiza dados pessoais
   - Mantém dados obrigatórios
   - Gera log de auditoria CRITICAL
   - Notifica paciente por email
5. **Status** → "Completed"

#### Passo 6: Rejeitar (se necessário)

**Quando rejeitar:**
- Dados em processo judicial
- Não há base legal para exclusão
- Impossibilidade técnica temporária

**Passos:**
1. **Clique** em "Rejeitar"
2. **Preencha motivo detalhado:**
   ```
   Exemplo:
   "Requisição rejeitada devido a processo judicial em andamento
   (Processo nº 1234567-89.2024.8.26.0100). Os dados serão mantidos
   conforme determinação judicial até conclusão do processo.
   Prazo estimado: 2025."
   ```
3. **Confirme rejeição**
4. **Sistema notifica paciente** com motivo
5. **Status** → "Rejected"

### Anonimização CFM Compliant

**O que o sistema faz automaticamente:**

**Dados Anonimizados:**
- Nome → `"Patient-ABC123..."`
- CPF → `"***"`
- Email → `"anonymized-xyz@example.com"`
- Telefone → Removido
- Endereço → Removido

**Dados Mantidos (20 anos):**
- Prontuários médicos (sem identificação pessoal)
- Histórico de consultas (anonimizado)
- Prescrições (anonimizadas)
- Diagnósticos (anonimizados)
- Exames (anonimizados)

**Dados Excluídos Imediatamente:**
- Marketing e comunicações
- Consentimentos opcionais
- Preferências de conta
- Histórico de login

### Prazos e SLA

⏰ **15 dias corridos** para resposta (LGPD Art. 18, §1º)

**Contagem:**
- Dia 0: Requisição recebida
- Dias 1-7: Análise inicial
- Dias 8-12: Processamento
- Dias 13-14: Aprovação (se necessária)
- Dia 15: Conclusão

**Se não conseguir cumprir prazo:**
1. Notifique paciente antes do dia 15
2. Explique motivo do atraso
3. Informe novo prazo estimado
4. Mantenha paciente informado

### Relatórios de Exclusão

**Métricas importantes:**
- Total de requisições (por período)
- Requisições pendentes
- Requisições concluídas no prazo
- Requisições atrasadas
- Taxa de rejeição

**Gerar relatório:**
1. Dashboard LGPD → **Métricas de Exclusão**
2. Escolha período
3. Exporte CSV ou PDF

---

## 📈 Dashboard de Compliance LGPD

### Visão Geral

O Dashboard oferece visão consolidada do status de compliance LGPD da organização.

### Acessando

1. **Login** no System Admin
2. **Navegue:** Menu → **LGPD** → **Dashboard**
3. **Visualize** métricas em tempo real

### Métricas Principais

#### 1. Estatísticas Gerais

**Cards no topo:**
- 📊 **Total de Logs** (últimos 30 dias)
- 🔒 **Acessos a Dados Sensíveis** (últimos 30 dias)
- ✅ **Consentimentos Ativos**
- 🗑️ **Requisições Pendentes**

#### 2. Gráficos de Auditoria

**Gráfico 1: Acessos por Tipo de Entidade (Pizza)**
- Patient: X%
- MedicalRecord: Y%
- Prescription: Z%
- Outros: W%

**Gráfico 2: Acessos por Usuário (Barra)**
- Top 10 usuários com mais acessos
- Identifica comportamento anormal

**Gráfico 3: Timeline de Atividades (Linha)**
- Evolução de acessos ao longo do tempo
- Identifica picos e padrões

**Gráfico 4: Distribuição de Severidade (Donut)**
- INFO: X%
- WARNING: Y%
- ERROR: Z%
- CRITICAL: W%

#### 3. Alertas

🚨 **Alertas ativos:**

**Segurança:**
- Acessos não autorizados (últimas 24h)
- Tentativas de login falhas (últimas 24h)
- Volume anormal de acessos por usuário

**Compliance:**
- Requisições pendentes há mais de 10 dias
- Requisições atrasadas (> 15 dias)
- Consentimentos expirados

**Clique em alerta** para ver detalhes e tomar ação.

#### 4. Métricas de Consentimento

**Gráficos:**
- Taxa de aceitação por tipo
- Consentimentos ativos vs revogados
- Evolução de consentimentos ao longo do tempo

**Tabela:**
- Tipo | Total | Ativos | Revogados | Taxa

#### 5. Métricas de Portabilidade e Exclusão

**Exportações:**
- Total de exportações (últimos 30 dias)
- Formato mais usado (JSON, PDF, XML)
- Tempo médio de processamento

**Exclusões:**
- Requisições pendentes
- Requisições concluídas
- Taxa de rejeição
- Tempo médio de processamento

### Filtros de Período

- 📅 Últimos 7 dias
- 📅 Últimos 30 dias
- 📅 Últimos 90 dias
- 📅 Personalizado (escolher datas)

### Exportando Dashboard

**Opções:**
- **PDF** - Relatório visual completo
- **Excel** - Dados brutos para análise
- **PowerPoint** - Slides para apresentação

**Casos de uso:**
- Reuniões de compliance
- Apresentações para diretoria
- Auditorias internas
- Relatórios mensais/trimestrais

---

## 📑 Relatórios para ANPD

### Quando Gerar

**Situações:**
- Auditoria da ANPD
- Incidente de segurança
- Solicitação formal da autoridade
- Relatórios periódicos (se aplicável)

### Tipos de Relatório

#### 1. Relatório de Compliance Geral

**Conteúdo:**
- Visão geral do sistema de auditoria
- Total de logs por período
- Categorias de dados tratados
- Finalidades do tratamento
- Medidas de segurança implementadas
- Processos de resposta a solicitações
- Incidentes reportados (se houver)

**Gerar:**
1. Dashboard LGPD → **Relatórios**
2. Selecione "Relatório de Compliance Geral"
3. Escolha período
4. Clique em "Gerar PDF"

#### 2. Relatório de Acessos a Dados Sensíveis

**Conteúdo:**
- Lista de todos os acessos a dados da categoria SENSITIVE
- Por usuário, data, entidade
- Finalidade de cada acesso
- Justificativa (quando aplicável)

**Gerar:**
1. Logs de Auditoria
2. Filtre por **DataCategory:** `SENSITIVE`
3. Defina período solicitado
4. Exporte em CSV ou JSON
5. Compile relatório formal em documento separado

#### 3. Relatório de Incidente de Segurança

**Obrigatório quando:**
- Vazamento de dados
- Acesso não autorizado em massa
- Ransomware
- Perda de dados

**Conteúdo (LGPD Art. 48):**
1. **Descrição do incidente**
   - O que aconteceu
   - Quando foi detectado
   - Sistemas afetados
   
2. **Dados envolvidos**
   - Categorias de dados
   - Volume estimado
   - Sensibilidade
   
3. **Titulares afetados**
   - Quantidade estimada
   - Perfil (pacientes, usuários, etc.)
   
4. **Medidas tomadas**
   - Contenção imediata
   - Erradicação da ameaça
   - Correções implementadas
   
5. **Riscos identificados**
   - Impacto potencial
   - Probabilidade de dano
   
6. **Medidas de mitigação**
   - Curto prazo
   - Longo prazo
   
7. **Comunicação**
   - Titulares notificados?
   - Quando?
   - Como?

**Prazo:** **72 horas** (recomendado)

#### 4. Relatório de Direitos dos Titulares

**Conteúdo:**
- Total de solicitações recebidas (por tipo)
- Solicitações atendidas no prazo
- Solicitações atrasadas (com justificativa)
- Solicitações rejeitadas (com motivo)
- Tempo médio de resposta

**Período:** Geralmente trimestral ou anual

**Gerar:**
1. Dashboard LGPD → **Relatórios**
2. Selecione "Relatório de Direitos dos Titulares"
3. Escolha período
4. Clique em "Gerar PDF"

### Template de Resposta à ANPD

```
RELATÓRIO DE COMPLIANCE LGPD
PrimeCare Software

Data: [DATA]
Período: [INÍCIO] a [FIM]
Solicitação: [NÚMERO/REFERÊNCIA ANPD]

1. IDENTIFICAÇÃO DO CONTROLADOR
   Nome: [Nome da Clínica/Instituição]
   CNPJ: [CNPJ]
   Endereço: [Endereço Completo]
   DPO: [Nome do Encarregado]
   Email DPO: dpo@primecare.com

2. SISTEMA DE AUDITORIA
   [Descrição do sistema implementado]
   
3. DADOS TRATADOS
   [Categorias e volumes]
   
4. LOGS DE AUDITORIA
   Total de logs: [NÚMERO]
   Período: [INÍCIO] a [FIM]
   [Anexo CSV/JSON com logs]
   
5. DIREITOS DOS TITULARES
   Solicitações recebidas: [NÚMERO]
   Solicitações atendidas: [NÚMERO]
   Prazo médio: [DIAS]
   
6. INCIDENTES
   [Relatar se houver, ou "Nenhum incidente reportado"]
   
7. MEDIDAS DE SEGURANÇA
   [Lista de medidas implementadas]

8. ANEXOS
   - Anexo A: Logs de auditoria (CSV)
   - Anexo B: Política de Privacidade
   - Anexo C: Termos de Consentimento

Atenciosamente,
[Nome do DPO]
Encarregado de Dados - PrimeCare Software
```

---

## 🚨 Gestão de Incidentes

### Definição de Incidente

**Incidente de segurança LGPD:** Evento que pode acarretar risco ou dano relevante aos titulares (Art. 48).

**Exemplos:**
- Vazamento de dados
- Acesso não autorizado
- Perda de dados
- Ransomware
- Roubo de dispositivos
- Erro humano (envio de email errado)

### Classificação de Severidade

| Nível | Descrição | Exemplo | Ação |
|-------|-----------|---------|------|
| **Baixo** | Dados não sensíveis, poucas pessoas | Email de 1 usuário exposto | Notificação interna |
| **Médio** | Dados pessoais, número moderado | Lista de 50 pacientes vazada | Investigação + correção |
| **Alto** | Dados sensíveis, muitas pessoas | 500 prontuários acessados indevidamente | Notificação ANPD |
| **Crítico** | Dados sensíveis em massa, risco iminente | Ransomware, banco exposto | **Notificação urgente** |

### Plano de Resposta (IRP)

#### Fase 1: Detecção e Análise (0-2h)

**Ao detectar incidente:**

1. **Identifique:**
   - O que aconteceu?
   - Quando foi detectado?
   - Sistemas afetados?
   
2. **Classifique severidade** (Baixo/Médio/Alto/Crítico)

3. **Acione equipe:**
   - DPO (Encarregado)
   - TI/Segurança
   - Jurídico
   - Comunicação
   
4. **Isole sistemas afetados** (se aplicável)

#### Fase 2: Contenção (2-8h)

**Ações imediatas:**

1. **Bloqueie acesso não autorizado:**
   - Revogue credenciais comprometidas
   - Desative contas suspeitas
   - Isole sistemas

2. **Preserve evidências:**
   - Não altere logs
   - Faça backup de evidências
   - Documente tudo

3. **No System Admin:**
   ```
   1. Logs de Auditoria
   2. Filtre período do incidente
   3. Identifique ações suspeitas
   4. Exporte logs para análise
   5. Identifique usuários/IPs envolvidos
   ```

4. **Suspenda usuários comprometidos** (se necessário)

#### Fase 3: Erradicação (8-24h)

1. **Identifique causa raiz**
2. **Remova ameaças**
3. **Corrija vulnerabilidades**
4. **Atualize sistemas**

#### Fase 4: Recuperação (24-72h)

1. **Restaure sistemas**
2. **Verifique integridade dos dados**
3. **Restabeleça operações**
4. **Monitoramento intensivo**

#### Fase 5: Notificação

**Prazo ANPD:** **72 horas** (recomendado)

**Template de notificação ANPD:**
```
NOTIFICAÇÃO DE INCIDENTE DE SEGURANÇA

Data do Incidente: [DATA]
Data de Detecção: [DATA]
Data desta Notificação: [DATA]

1. DESCRIÇÃO DO INCIDENTE
   [Descrever o que aconteceu]

2. DADOS ENVOLVIDOS
   Categorias: [Pessoais / Sensíveis]
   Volume estimado: [NÚMERO]
   
3. TITULARES AFETADOS
   Quantidade: [NÚMERO]
   Perfil: [Pacientes / Usuários]
   
4. MEDIDAS TOMADAS
   [Lista de ações de contenção e correção]
   
5. RISCOS IDENTIFICADOS
   [Impactos potenciais]
   
6. MEDIDAS DE MITIGAÇÃO
   [Plano de ação]
   
7. COMUNICAÇÃO AOS TITULARES
   [Se e como foram notificados]
```

**Template de comunicação aos titulares:**
```
Assunto: Notificação de Incidente de Segurança - PrimeCare

Prezado(a) [Nome],

Informamos que em [DATA] identificamos um incidente de segurança
que pode ter afetado seus dados pessoais.

DADOS POTENCIALMENTE AFETADOS:
- [Lista específica]

AÇÕES TOMADAS:
- [Medidas de contenção]
- [Correções implementadas]

RISCOS IDENTIFICADOS:
- [Riscos para o titular]

RECOMENDAÇÕES:
- Alterar senha imediatamente
- Monitorar suas contas
- Ativar autenticação em dois fatores

Para mais informações, entre em contato:
Email: lgpd@primecare.com
Telefone: +55 (11) XXXX-XXXX

Atenciosamente,
Equipe PrimeCare
```

#### Fase 6: Lições Aprendidas

**Após resolução:**

1. **Documente:**
   - Cronologia completa
   - Ações tomadas
   - Efetividade das medidas
   
2. **Analise:**
   - O que funcionou?
   - O que falhou?
   - Como prevenir recorrência?
   
3. **Atualize:**
   - Procedimentos de segurança
   - Treinamentos
   - Políticas internas
   - Controles técnicos

---

## ✨ Melhores Práticas

### Auditoria Regular

✅ **Diariamente:**
- Revise alertas de segurança
- Monitore requisições pendentes
- Verifique acessos CRITICAL

✅ **Semanalmente:**
- Revise logs de auditoria
- Analise padrões de acesso
- Verifique compliance de prazos

✅ **Mensalmente:**
- Gere relatórios de compliance
- Revise métricas do dashboard
- Atualize documentação

✅ **Trimestralmente:**
- Auditoria completa de logs
- Revisão de políticas
- Treinamento da equipe

### Gestão Proativa

1. **Automatize alertas:**
   - Configure notificações para eventos CRITICAL
   - Alertas de requisições atrasadas
   - Monitoramento de acessos não autorizados

2. **Mantenha documentação atualizada:**
   - Políticas de privacidade
   - Termos de consentimento
   - Procedimentos internos

3. **Treine equipe regularmente:**
   - Compliance LGPD
   - Segurança da informação
   - Resposta a incidentes

4. **Revise permissões:**
   - Princípio do menor privilégio
   - Revogue acessos desnecessários
   - Audite permissões trimestralmente

### Comunicação Clara

**Com pacientes:**
- Linguagem simples e clara
- Prazos realistas
- Transparência total

**Com equipe:**
- Procedimentos bem documentados
- Treinamento contínuo
- Cultura de segurança

**Com autoridades:**
- Respostas profissionais
- Documentação completa
- Prazos respeitados

---

## 🔧 Solução de Problemas

### Problema 1: Logs não aparecem

**Sintomas:**
- Nenhum log sendo gerado
- Logs antigos, mas não novos

**Soluções:**
1. Verifique se LgpdAuditMiddleware está ativo
2. Verifique conexão com banco de dados
3. Verifique logs de erro do backend
4. Confirme permissões de escrita no BD

### Problema 2: Exportação de dados falha

**Sintomas:**
- Erro ao exportar dados de paciente
- Arquivo vazio ou incompleto

**Soluções:**
1. Verifique permissões do usuário
2. Verifique espaço em disco
3. Tente formato diferente (JSON, XML, PDF)
4. Verifique logs de erro no backend

### Problema 3: Requisição de exclusão travada

**Sintomas:**
- Status não muda de "Processing"
- Erro ao completar exclusão

**Soluções:**
1. Verifique obrigações legais pendentes
2. Confirme aprovação legal (se necessária)
3. Verifique logs de erro
4. Tente processo manual de anonimização

### Problema 4: Dashboard não carrega

**Sintomas:**
- Dashboard em branco
- Erros de carregamento

**Soluções:**
1. Verifique conexão com API
2. Limpe cache do navegador
3. Verifique console do navegador para erros
4. Tente período menor de dados

### Problema 5: Alertas excessivos

**Sintomas:**
- Muitos alertas falsos positivos
- Alertas repetitivos

**Soluções:**
1. Ajuste thresholds de alertas
2. Configure whitelist de IPs conhecidos
3. Refine regras de detecção
4. Documente exceções legítimas

---

## 📞 Suporte

### Contatos

**Equipe LGPD:**
- Email: lgpd@primecare.com
- Telefone: +55 (11) XXXX-XXXX

**DPO (Encarregado):**
- Email: dpo@primecare.com
- Telefone: +55 (11) XXXX-XXXX

**Suporte Técnico:**
- Email: suporte@primecare.com
- Telefone: +55 (11) XXXX-XXXX (24/7)

### Documentação Adicional

- [LGPD Compliance Guide](./LGPD_COMPLIANCE_GUIDE.md)
- [Guia do Usuário LGPD](./USER_GUIDE_LGPD.md)
- [Audit Log Query Guide](./AUDIT_LOG_QUERY_GUIDE.md)
- [Security Best Practices](./SECURITY_BEST_PRACTICES_GUIDE.md)

---

**Última Atualização:** 29 de Janeiro de 2026  
**Versão:** 1.0  
**Próxima Revisão:** 29 de Julho de 2026

---

**PrimeCare Software** - Compliance LGPD com Excelência
