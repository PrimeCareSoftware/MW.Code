# 📋 Plano de Desenvolvimento Priorizado - PrimeCare Software

> **Documento Consolidado:** Este documento unifica PLANO_DESENVOLVIMENTO_PRIORIZADO.md (Parte 1) e PLANO_DESENVOLVIMENTO_PRIORIZADO_PARTE2.md (Parte 2)

> **Objetivo:** Documento detalhado com ordem de prioridade e passos necessários para cada desenvolvimento pendente.

> **Base:** Análise do PENDING_TASKS.md e APPS_PENDING_TASKS.md  
> **Última Atualização:** Janeiro 2025  
> **Status:** Plano de execução 2025-2026

---

## 🎯 Visão Executiva

Este documento organiza TODAS as pendências do PrimeCare Software em uma ordem de prioridade clara, considerando:

1. **Obrigatoriedade Legal** (CFM, ANVISA, Receita Federal, ANS)
2. **Impacto no Negócio** (Aquisição de clientes, retenção, receita)
3. **Complexidade Técnica** (Esforço e dependências)
4. **Viabilidade de Execução** (Recursos disponíveis)

### Resumo de Prioridades

| Categoria | Total de Tarefas | Esforço Total |
|-----------|------------------|---------------|
| 🔥🔥🔥 **CRÍTICO** (Legal) | 8 tarefas | 22-28 meses/dev |
| 🔥🔥 **ALTA** (Segurança + Compliance) | 12 tarefas | 18-24 meses/dev |
| 🔥 **MÉDIA** (Competitividade) | 15 tarefas | 28-36 meses/dev |
| ⚪ **BAIXA** (Nice to have) | 15 tarefas | 24-30 meses/dev |

**Total Geral:** 50 tarefas principais | 92-118 meses/dev de esforço

---

## 📊 ORDEM DE PRIORIDADE ABSOLUTA

### Legenda de Prioridades

- 🔥🔥🔥 **P0 - CRÍTICO**: Obrigatório por lei ou essencial para operação
- 🔥🔥 **P1 - ALTO**: Segurança crítica ou muito alta demanda de mercado
- 🔥 **P2 - MÉDIO**: Diferencial competitivo importante
- ⚪ **P3 - BAIXO**: Conveniente mas não essencial

---

## 🔥🔥🔥 PRIORIDADE CRÍTICA (P0) - DEVE SER FEITO

### Tarefas Obrigatórias por Lei Brasileira

---

### 1️⃣ CONFORMIDADE CFM - PRONTUÁRIO MÉDICO (Resolução 1.821/2007)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (CFM)  
**Prazo:** Q1/2025 (Janeiro-Março 2025)  
**Esforço:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000  

#### Por que é Crítico?
- **Obrigatório por lei** para todos os sistemas de prontuário eletrônico
- **Risco legal** alto para clínicas que usam o sistema
- **Compliance** essencial para vender para clínicas sérias
- Sem isso, o sistema pode ser **considerado irregular pelo CFM**

#### O que precisa ser feito?

**Etapa 1: Análise e Planejamento (1 semana)**
1. Estudar Resolução CFM 1.821/2007 completa
2. Mapear campos obrigatórios do prontuário atual vs. CFM
3. Identificar gaps na estrutura de dados
4. Criar especificação técnica detalhada
5. Revisar com médico consultor (se disponível)

**Etapa 2: Estruturação do Banco de Dados (1 semana)**
1. Criar/atualizar entidades no domínio:
   - `MedicalRecord` com campos obrigatórios CFM
   - `ClinicalExamination` (exame físico por sistemas)
   - `DiagnosticHypothesis` (hipóteses diagnósticas + CID-10)
   - `TherapeuticPlan` (plano terapêutico detalhado)
   - `InformedConsent` (consentimento informado)
2. Adicionar migrations no banco de dados
3. Criar validações de domínio

**Etapa 3: Implementação Backend (2 semanas)**
1. Atualizar API do prontuário médico
2. Criar endpoint para consentimento informado
3. Implementar validações obrigatórias antes de salvar
4. Adicionar campo de CID-10 com busca autocomplete
5. Estruturar campos SOAP (Subjetivo, Objetivo, Avaliação, Plano)
6. Implementar histórico de evolução

**Etapa 4: Implementação Frontend (3 semanas)**
1. Redesenhar tela de prontuário com campos obrigatórios
2. Criar formulário estruturado SOAP
3. Implementar busca de CID-10 (dropdown com autocomplete)
4. Criar modal de consentimento informado digital
5. Adicionar validações visuais (campos obrigatórios em vermelho)
6. Implementar alertas para campos faltantes
7. Criar visualização de histórico de evolução

**Etapa 5: Testes e Validação (1 semana)**
1. Testes unitários de validações
2. Testes de integração
3. Teste com médico real
4. Ajustes conforme feedback
5. Documentação de compliance

**Etapa 6: Deploy e Treinamento (1 semana)**
1. Deploy em produção gradual
2. Criar guia de uso para médicos
3. Webinar ou vídeo de treinamento
4. Suporte dedicado nas primeiras semanas

#### Dependências
- Nenhuma (pode começar imediatamente)

#### Entregáveis
- [ ] Prontuário com campos obrigatórios CFM 1.821
- [ ] Sistema de consentimento informado digital
- [ ] Validações antes de salvar prontuário
- [ ] Documentação de compliance CFM

#### Critérios de Sucesso
- Todos os campos obrigatórios da CFM 1.821 implementados
- Médicos conseguem preencher prontuário completo em < 10 min
- Zero violações de compliance CFM
- Aprovação por médico consultor

---

### 2️⃣ EMISSÃO DE NF-e / NFS-e (RECEITA FEDERAL)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (Receita Federal)  
**Prazo:** Q2/2025 (Abril-Junho 2025)  
**Esforço:** 3 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 90.000 + R$ 50-200/mês (gateway)

#### Por que é Crítico?
- **Obrigatório por lei** para todas as clínicas (emissão de nota fiscal)
- Sem isso, clínicas estão **irregulares perante Receita Federal**
- **Barreira de entrada** enorme - muitas clínicas não usam o sistema por isso
- Pode ser **cobrado como módulo premium** (alta rentabilidade)

#### O que precisa ser feito?

**Etapa 1: Escolha e Contratação de Gateway (1 semana)**
1. Avaliar gateways disponíveis:
   - **Focus NFe** (recomendado - R$ 50-150/mês)
   - **eNotas** (alternativa - R$ 100-200/mês)
   - **Bling** (simples - R$ 70/mês)
2. Contratar plano empresarial
3. Obter credenciais de API (sandbox e produção)
4. Configurar certificado digital A1 (ou suporte A3)

**Etapa 2: Modelagem de Dados (2 semanas)**
1. Criar entidades de domínio:
   - `InvoiceConfiguration` (configuração por clínica)
   - `ServiceInvoice` (NFS-e)
   - `ProductInvoice` (NF-e, se necessário)
   - `TaxSettings` (impostos e alíquotas)
2. Migrations de banco
3. Relacionar com `Appointment` e `Payment`

**Etapa 3: Implementação Backend - Configuração (2 semanas)**
1. Criar API de configuração de notas fiscais
2. Cadastro de CNPJ, CNAE, regime tributário
3. Configuração de impostos (ISS, PIS, COFINS, IR, CSLL)
4. Upload e gerenciamento de certificado digital
5. Integração com gateway (Focus NFe / eNotas)

**Etapa 4: Implementação Backend - Emissão (3 semanas)**
1. Criar serviço de emissão automática pós-pagamento
2. Geração de XML conforme padrão SEFAZ
3. Assinatura digital do XML
4. Envio para gateway e SEFAZ
5. Recebimento de protocolo e XML de retorno
6. Armazenamento de XML + PDF (Azure Blob / AWS S3)
7. Envio automático de nota por email ao paciente

**Etapa 5: Implementação Backend - Gestão (2 semanas)**
1. Consultar status de nota fiscal
2. Cancelamento de nota (dentro do prazo)
3. Substituição de nota (retificação)
4. Relatórios fiscais (livro de serviços)
5. Exportação de dados para contabilidade

**Etapa 6: Implementação Frontend (3 semanas)**
1. Tela de configuração de NF-e/NFS-e
2. Tela de listagem de notas emitidas
3. Visualização de XML e PDF
4. Ação de cancelar/substituir nota
5. Dashboard de faturamento fiscal
6. Relatórios de impostos

**Etapa 7: Testes (2 semanas)**
1. Testes em ambiente sandbox
2. Emissão de notas de teste
3. Cancelamento e substituição
4. Testes com certificado digital
5. Validação de cálculos de impostos

**Etapa 8: Homologação e Deploy (1 semana)**
1. Homologação com Receita Federal (ambiente de teste)
2. Deploy gradual em produção
3. Primeira emissão real monitorada
4. Treinamento de clientes
5. Documentação completa

#### Dependências
- Módulo financeiro parcialmente implementado
- Sistema de pagamentos funcionando
- Certificado digital da clínica (A1 ou A3)

#### Entregáveis
- [ ] Integração com gateway de NF-e/NFS-e
- [ ] Emissão automática pós-pagamento
- [ ] Gestão completa de notas (cancelar, substituir)
- [ ] Relatórios fiscais
- [ ] Armazenamento de XML e PDF

#### Critérios de Sucesso
- Emissão automática de NFS-e em < 30 segundos
- Taxa de erro < 1%
- 100% das notas armazenadas por 5+ anos
- Exportação contábil funcional

---

### 3️⃣ RECEITAS MÉDICAS DIGITAIS (CFM 1.643/2002 + ANVISA)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (CFM + ANVISA)  
**Prazo:** Q2/2025 (Abril-Junho 2025)  
**Esforço:** 3 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 90.000

#### Por que é Crítico?
- Receitas médicas são **documento legal** e devem cumprir normas
- **Medicamentos controlados** (ANVISA) exigem receituário especial
- Sem compliance, receitas podem ser **recusadas em farmácias**
- **Risco legal** para médicos e clínicas

#### O que precisa ser feito?

**Etapa 1: Estudo Regulatório (1 semana)**
1. Estudar Resolução CFM 1.643/2002
2. Estudar Portaria ANVISA 344/1998 (controlados)
3. Identificar tipos de receitas:
   - Receita simples (medicamentos comuns)
   - Receita controle especial B (psicotrópicos)
   - Receita controle especial A (entorpecentes)
   - Receita antimicrobiana
   - Receita C1 (outros controlados)
4. Mapear requisitos técnicos de cada tipo

**Etapa 2: Modelagem de Dados (1 semana)**
1. Criar entidades:
   - `PrescriptionType` (enum: Simple, SpecialB, SpecialA, Antibiotic, C1)
   - `Prescription` (receita completa)
   - `Medication` (medicamento com DCB/DCI)
   - `PrescriptionSequence` (numeração controlada)
2. Integrar com tabela de medicamentos (importar base ANVISA)
3. Migrations

**Etapa 3: Backend - Validações (2 semanas)**
1. Criar validações específicas por tipo de receita
2. Validar numeração sequencial (controladas)
3. Validar validade conforme tipo
4. Verificar campos obrigatórios
5. Implementar regras de retenção de receitas

**Etapa 4: Backend - Geração e Assinatura (2 semanas)**
1. Criar templates de receitas (PDF)
2. Gerar PDF com código de barras / QR Code
3. Preparar para assinatura digital ICP-Brasil (futuro)
4. Armazenar PDF gerado
5. Enviar por email/WhatsApp

**Etapa 5: Frontend - Interface (3 semanas)**
1. Tela de prescrição médica com tipos
2. Autocomplete de medicamentos (com destaque para controlados)
3. Alertas para medicamentos controlados
4. Seleção de tipo de receita
5. Visualização de preview da receita
6. Impressão de receitas especiais (cores diferentes)

**Etapa 6: Integração SNGPC (2 semanas)**
1. Estudar SNGPC (Sistema Nacional de Produtos Controlados)
2. Implementar escrituração digital
3. Geração de XML para ANVISA
4. Transmissão mensal ao SNGPC
5. Relatórios de conformidade

**Etapa 7: Testes e Validação (1 semana)**
1. Teste de todos os tipos de receita
2. Validar com farmácia parceira
3. Verificar aceitação em redes (Drogasil, Pague Menos)
4. Ajustes conforme feedback

**Etapa 8: Deploy e Documentação (1 semana)**
1. Deploy gradual
2. Guia para médicos
3. Guia de medicamentos controlados
4. Treinamento

#### Dependências
- Prontuário médico funcionando
- (Futuro) Assinatura digital ICP-Brasil

#### Entregáveis
- [ ] Sistema de prescrição com tipos de receita CFM
- [ ] Validações específicas por tipo (ANVISA)
- [ ] Integração SNGPC (controlados)
- [ ] PDF de receita profissional
- [ ] Guia de medicamentos controlados

#### Critérios de Sucesso
- Receitas aceitas em 100% das farmácias testadas
- Conformidade com CFM 1.643 e ANVISA 344
- Zero recusas por problemas de formato
- Transmissão SNGPC automática

---

### 4️⃣ INTEGRAÇÃO TISS - FASE 1 (ANS)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal para convênios (ANS)  
**Prazo:** Q4/2025 (Outubro-Dezembro 2025)  
**Esforço:** 3 meses | 2-3 desenvolvedores  
**Custo Estimado:** R$ 135.000

#### Por que é Crítico?
- **70% das clínicas atendem convênios** (mercado gigante)
- Sem TISS, o sistema **não pode ser vendido** para maioria das clínicas
- **Barreira competitiva** muito alta
- Abre mercado de **R$ 200M+** em clínicas com convênios

#### O que precisa ser feito?

**Etapa 1: Estudo do Padrão TISS (2 semanas)**
1. Baixar documentação oficial ANS (TISS 4.02.00+)
2. Estudar estrutura de guias:
   - Guia de Consulta (SP/SADT)
   - Guia de Internação
   - Guia de Honorários
3. Entender tabelas obrigatórias:
   - CBHPM (procedimentos médicos)
   - TUSS (terminologia unificada)
   - Rol ANS (cobertura obrigatória)
4. Estudar XML schemas oficiais

**Etapa 2: Importação de Tabelas (2 semanas)**
1. Importar tabela CBHPM atualizada
2. Importar tabela TUSS
3. Importar Rol ANS
4. Criar script de atualização trimestral
5. Indexação para busca rápida

**Etapa 3: Modelagem de Dados (2 semanas)**
1. Criar entidades:
   - `HealthInsuranceOperator` (operadora)
   - `PatientHealthPlan` (plano do paciente)
   - `TISSGuide` (guia TISS genérica)
   - `TISSConsultationGuide` (guia de consulta)
   - `TISSAuthorization` (autorização prévia)
   - `TISSBatch` (lote de faturamento)
2. Relacionamentos com agendamento e atendimento
3. Migrations

**Etapa 4: Backend - Cadastro (2 semanas)**
1. API de cadastro de operadoras
2. API de planos de saúde do paciente
3. Validação de carteirinha (número, validade)
4. Tabela de preços por operadora

**Etapa 5: Backend - Autorização (2 semanas)**
1. Criar fluxo de solicitação de autorização
2. Gerar guia SP/SADT conforme TISS
3. Envio manual ou webservice (se disponível)
4. Registro de número de autorização
5. Controle de status (pendente/autorizado/negado)

**Etapa 6: Backend - Faturamento (3 semanas)**
1. Geração de lotes XML conforme TISS 4.02.00
2. Validação de XML contra schemas XSD
3. Assinatura digital do XML
4. Interface para envio (manual ou automático)
5. Protocolo de recebimento
6. Armazenamento de lotes enviados

**Etapa 7: Frontend - Operadoras (1 semana)**
1. Tela de cadastro de operadoras
2. Configuração de preços por operadora
3. Histórico de glosas por operadora

**Etapa 8: Frontend - Pacientes (1 semana)**
1. Campo de plano de saúde no cadastro de paciente
2. Validação de carteirinha
3. Visualização de autorizações

**Etapa 9: Frontend - Autorização (2 semanas)**
1. Tela de solicitação de autorização
2. Formulário de guia SP/SADT
3. Acompanhamento de autorizações pendentes
4. Dashboard de autorizações

**Etapa 10: Frontend - Faturamento (2 semanas)**
1. Tela de geração de lotes
2. Seleção de atendimentos para faturar
3. Preview do XML
4. Download de XML e protocolo
5. Relatórios de faturamento

**Etapa 11: Testes e Homologação (2 semanas)**
1. Testes de geração de XML
2. Validação contra schemas XSD
3. Teste com operadora parceira (se possível)
4. Simulação de envio
5. Ajustes conforme feedback

**Etapa 12: Deploy e Treinamento (1 semana)**
1. Deploy gradual
2. Piloto com 2-3 clínicas
3. Treinamento específico TISS
4. Documentação completa

#### Dependências
- Agendamentos e atendimentos implementados
- Sistema de pagamentos parcial

#### Entregáveis
- [ ] Cadastro de operadoras e planos
- [ ] Solicitação de autorizações
- [ ] Geração de guias TISS XML
- [ ] Faturamento em lotes
- [ ] Relatórios por convênio

#### Critérios de Sucesso
- XML validado contra XSD oficial ANS
- Aceitação de lotes por pelo menos 1 operadora
- Tempo de geração de lote < 2 minutos
- Interface intuitiva para não-técnicos

---

### 5️⃣ CONFORMIDADE CFM 1.638/2002 - PRONTUÁRIO ELETRÔNICO

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (CFM)  
**Prazo:** Q1/2025 (Janeiro-Março 2025)  
**Esforço:** 1.5 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 22.500

#### Por que é Crítico?
- Define **requisitos técnicos de segurança** do prontuário
- Exige **imutabilidade** e **rastreabilidade** de alterações
- Sem isso, prontuário pode ser **contestado juridicamente**
- **Complementa a Resolução 1.821**

#### O que precisa ser feito?

**Etapa 1: Versionamento de Prontuários (2 semanas)**
1. Implementar padrão Event Sourcing ou versionamento:
   - Cada alteração gera nova versão
   - Versão anterior nunca é deletada
   - Timestamp + usuário em cada versão
2. Criar tabela `MedicalRecordVersion`
3. Migration para versionar prontuários existentes

**Etapa 2: Imutabilidade (1 semana)**
1. Adicionar campo `IsClosed` no prontuário
2. Após "concluir atendimento", prontuário fecha
3. Reabrir apenas com justificativa escrita
4. Alterações pós-fechamento viram adendos (nova versão)

**Etapa 3: Assinatura Digital (preparação) (1 semana)**
1. Preparar estrutura para assinatura ICP-Brasil
2. Hash SHA-256 de cada prontuário fechado
3. Timestamp confiável (NTP sincronizado)
4. Campo para armazenar assinatura (futuro)

**Etapa 4: Auditoria de Acessos (2 semanas)**
1. Logar TODOS os acessos a prontuários
2. Incluir: quem, quando, IP, ação (leitura/escrita)
3. Armazenar logs por 20 anos (conforme CFM)
4. Interface para consultar histórico de acessos

**Etapa 5: Backend (1 semana)**
1. Endpoint para histórico de versões
2. Endpoint para reabrir prontuário (com justificativa)
3. Endpoint para logs de auditoria

**Etapa 6: Frontend (2 semanas)**
1. Botão "Concluir Atendimento" (fecha prontuário)
2. Modal de confirmação com avisos legais
3. Visualização de histórico de versões
4. Visualização de logs de auditoria
5. Modal para reabrir com justificativa

**Etapa 7: Testes (1 semana)**
1. Testar versionamento
2. Testar imutabilidade pós-fechamento
3. Testar logs de auditoria
4. Validar com médico

**Etapa 8: Deploy (1 semana)**
1. Deploy gradual
2. Migração de prontuários antigos
3. Treinamento
4. Documentação de compliance

#### Dependências
- Tarefa #1 (Prontuário CFM 1.821) concluída

#### Entregáveis
- [ ] Versionamento completo de prontuários
- [ ] Imutabilidade após conclusão
- [ ] Auditoria de acessos
- [ ] Preparação para assinatura digital

#### Critérios de Sucesso
- 100% dos prontuários versionados
- Zero possibilidade de alterar sem rastreio
- Logs de auditoria de 100% dos acessos
- Conformidade com CFM 1.638

---

### 6️⃣ INTEGRAÇÃO SNGPC - ANVISA (MEDICAMENTOS CONTROLADOS)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA (para clínicas com farmácia)  
**Obrigatoriedade:** Legal (ANVISA)  
**Prazo:** Q2/2025 (Abril-Junho 2025)  
**Esforço:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000

#### Por que é Crítico?
- **Obrigatório por lei** para dispensação de controlados
- Clínicas com farmácia não podem operar sem
- **Multas pesadas** da ANVISA por não conformidade
- Sistema complementar à receita médica digital

#### O que precisa ser feito?

**Etapa 1: Estudo do SNGPC (1 semana)**
1. Estudar documentação SNGPC ANVISA
2. Entender escrituração digital
3. Formato de XML para transmissão
4. Prazos e regras de envio

**Etapa 2: Modelagem (1 semana)**
1. Criar entidade `ControlledMedicationDispensing`
2. Relacionar com prescrição e paciente
3. Campos: lote, validade, quantidade, CPF paciente
4. Migration

**Etapa 3: Backend - Escrituração (2 semanas)**
1. Registrar cada dispensação de controlado
2. Numeração sequencial obrigatória
3. Livro digital de substâncias controladas
4. Validações ANVISA

**Etapa 4: Backend - Transmissão (2 semanas)**
1. Gerar XML mensal para SNGPC
2. Validação contra schema ANVISA
3. Integração com webservice SNGPC
4. Protocolo de recebimento
5. Relatórios de conformidade

**Etapa 5: Frontend (2 semanas)**
1. Tela de dispensação de medicamentos
2. Registro de controlados
3. Livro digital (visualização)
4. Geração de XML mensal
5. Transmissão ao SNGPC

**Etapa 6: Testes (1 semana)**
1. Testar escrituração
2. Validar XML
3. Simular transmissão
4. Homologação com ANVISA (ambiente teste)

**Etapa 7: Deploy (1 semana)**
1. Deploy em produção
2. Treinamento farmacêuticos
3. Primeira transmissão monitorada
4. Documentação

#### Dependências
- Receitas médicas digitais (#3) implementadas

#### Entregáveis
- [ ] Escrituração digital de controlados
- [ ] Livro digital ANVISA
- [ ] Geração de XML SNGPC
- [ ] Transmissão automática mensal
- [ ] Relatórios de conformidade

#### Critérios de Sucesso
- 100% dos controlados registrados
- XML aceito pela ANVISA
- Transmissão automática funcionando
- Conformidade total com Portaria 344

---

### 7️⃣ CONFORMIDADE CFM 2.314/2022 - TELEMEDICINA

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA (quando telemedicina implementada)  
**Obrigatoriedade:** Legal (CFM)  
**Prazo:** Q3/2025 (Julho-Setembro 2025)  
**Esforço:** 2 meses | 1 desenvolvedor (em paralelo com telemedicina)  
**Custo Estimado:** R$ 30.000

#### Por que é Crítico?
- Telemedicina **sem compliance CFM é ilegal**
- Médicos podem sofrer **processo no CFM**
- Exige consentimento específico e identificação rigorosa
- Documentação deve ser perfeita

#### O que precisa ser feito?

**Etapa 1: Termo de Consentimento (1 semana)**
1. Criar termo legal específico para telemedicina
2. Consultar advogado especializado em direito médico
3. Incluir todos os requisitos CFM 2.314
4. Armazenar aceite digital com timestamp

**Etapa 2: Identificação Bidirecional (2 semanas)**
1. Verificação de identidade do médico:
   - Foto do médico
   - CRM visível
   - Confirmação de identidade
2. Verificação de identidade do paciente:
   - Upload de documento com foto
   - Selfie de confirmação (opcional)
3. Armazenar comprovantes

**Etapa 3: Prontuário de Teleconsulta (1 semana)**
1. Adicionar campo "Modalidade" (Presencial / Teleconsulta)
2. Marcar automaticamente teleconsultas
3. Campos adicionais específicos (qualidade conexão, etc.)

**Etapa 4: Gravação de Consultas (2 semanas)**
1. Opção de gravar teleconsulta (com consentimento)
2. Armazenar gravação criptografada
3. Retenção por 20 anos
4. Download apenas por autorizado

**Etapa 5: Assinatura Digital (preparação) (1 semana)**
1. Preparar receitas e atestados digitais
2. Estrutura para assinatura ICP-Brasil
3. Validade jurídica

**Etapa 6: Validação de Primeiro Atendimento (1 semana)**
1. Verificar se já houve atendimento presencial
2. Alerta se primeira consulta for teleconsulta
3. Exceções: áreas remotas, emergências

**Etapa 7: Frontend (2 semanas)**
1. Modal de consentimento antes de entrar na consulta
2. Upload de documentos de identificação
3. Confirmação de identidade bidirecional
4. Opção de gravar consulta
5. Indicador visual de "Teleconsulta" no prontuário

**Etapa 8: Testes e Validação Legal (1 semana)**
1. Testar fluxo completo
2. Revisão jurídica
3. Validar com CFM (se possível)
4. Ajustes

**Etapa 9: Deploy (1 semana)**
1. Deploy gradual
2. Treinamento específico de compliance
3. Guia legal para médicos
4. Documentação

#### Dependências
- Telemedicina básica implementada
- Sistema de armazenamento de arquivos (gravações)

#### Entregáveis
- [ ] Termo de consentimento específico CFM 2.314
- [ ] Verificação de identidade bidirecional
- [ ] Gravação de consultas (opcional, com consentimento)
- [ ] Prontuário marcado como Teleconsulta
- [ ] Validação de primeiro atendimento

#### Critérios de Sucesso
- 100% conformidade com CFM 2.314
- Zero teleconsultas sem consentimento
- Identificação registrada em 100% das consultas
- Aprovação jurídica

---

### 8️⃣ TELEMEDICINA / TELECONSULTA

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Impacto:** Muito Alto - Diferencial competitivo  
**Prazo:** Q3/2025 (Julho-Setembro 2025)  
**Esforço:** 4-6 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 135.000 + R$ 300-500/mês (infraestrutura)

#### Por que é Crítico?
- **80% dos concorrentes** já oferecem
- Pandemia consolidou adoção permanente
- **Expansão geográfica** sem presença física
- Diferencial **muito valorizado** por clínicas

#### O que precisa ser feito?

**Etapa 1: Escolha de Plataforma de Vídeo (1 semana)**
1. Avaliar opções:
   - **Daily.co** (HIPAA compliant, recomendado) - $0.10-0.15/min
   - **Jitsi Meet Self-Hosted** (open source, gratuito)
   - **Twilio Video** (escalável) - $0.0015/min
2. Contratar plano
3. Obter credenciais de API

**Etapa 2: Microserviço de Telemedicina (4 semanas)**
1. Criar microserviço separado (ASP.NET Core)
2. Integração com Daily.co ou Jitsi
3. Gerenciamento de salas virtuais
4. Controle de permissões (quem pode entrar)
5. Gravação opcional
6. API REST para frontend

**Etapa 3: Backend - Agendamento (2 semanas)**
1. Adicionar tipo "Teleconsulta" em agendamento
2. Gerar link único da sala virtual
3. Enviar link por email/SMS/WhatsApp 30min antes
4. Sala de espera virtual

**Etapa 4: Backend - Sala de Espera (1 semana)**
1. Fila virtual de pacientes aguardando
2. Notificação ao médico quando paciente entra
3. Teste de câmera e microfone (frontend)

**Etapa 5: Frontend Web - Paciente (3 semanas)**
1. Interface de teleconsulta (embed Daily.co)
2. Teste de equipamento (câmera, microfone)
3. Sala de espera com status
4. Controles de mudo, câmera, compartilhar tela
5. Chat paralelo
6. Botão de emergência (encerrar)

**Etapa 6: Frontend Web - Médico (3 semanas)**
1. Interface de teleconsulta médico
2. Visualização de fila de espera
3. Chamar próximo paciente
4. Controles profissionais (gravar, compartilhar)
5. Acesso rápido ao prontuário (sidebar)
6. Botão de finalizar consulta

**Etapa 7: Aplicativos Mobile (4 semanas)**
1. Integração nos apps iOS e Android
2. Mesma funcionalidade web
3. Otimização para mobile
4. Notificações push

**Etapa 8: Compliance CFM (integrado)
1. Ver Tarefa #7 - Conformidade CFM 2.314

**Etapa 9: Testes (2 semanas)**
1. Testes de qualidade de vídeo
2. Testes de latência
3. Teste em diferentes conexões (3G, 4G, WiFi)
4. Teste de gravação
5. Teste com múltiplos usuários simultâneos

**Etapa 10: Infraestrutura (1 semana)**
1. Provisionar servidores (se self-hosted)
2. Configurar CDN
3. Monitoramento de qualidade
4. Backup de gravações

**Etapa 11: Deploy e Piloto (2 semanas)**
1. Deploy gradual
2. Piloto com 5 médicos voluntários
3. Coletar feedback
4. Ajustes
5. Launch oficial

**Etapa 12: Treinamento (1 semana)**
1. Criar guia de uso para médicos
2. Criar guia para pacientes
3. Vídeos tutoriais
4. Webinar de lançamento
5. Suporte dedicado

#### Dependências
- Sistema de agendamentos funcionando
- Sistema de notificações (email/SMS) funcionando

#### Entregáveis
- [ ] Videochamadas HD com qualidade profissional
- [ ] Sala de espera virtual
- [ ] Integração com agendamento
- [ ] Apps web e mobile
- [ ] Gravação opcional
- [ ] Compliance CFM 2.314

#### Critérios de Sucesso
- Qualidade de vídeo > 720p em conexão 4G
- Latência < 200ms
- Taxa de sucesso > 95% (consultas sem problemas técnicos)
- NPS de médicos e pacientes > 8.0
- 100% conformidade CFM

---

## 🔥🔥 PRIORIDADE ALTA (P1)

### (Continua com as outras tarefas...)

---

## 💡 Como Usar Este Documento

### Para o Gerente de Projetos
1. Siga a ordem de prioridade rigorosamente
2. Tarefas P0 (CRÍTICAS) devem ser feitas antes de qualquer P1
3. Use as estimativas de esforço para planejar sprints
4. Considere dependências entre tarefas

### Para Desenvolvedores
1. Cada tarefa tem passos claros e detalhados
2. Siga a ordem das etapas dentro de cada tarefa
3. Consulte "Dependências" antes de começar
4. Marque os "Entregáveis" conforme for completando

### Para Stakeholders
1. Use "Por que é Crítico?" para entender impacto
2. "Custo Estimado" ajuda no planejamento financeiro
3. "Prazo" indica quando esperar cada entrega
4. "Critérios de Sucesso" define o que é uma implementação bem-sucedida

---

## 📊 Resumo Financeiro P0 (Tarefas Críticas)

| # | Tarefa | Esforço | Custo | Prazo |
|---|--------|---------|-------|-------|
| 1 | Conformidade CFM 1.821 | 2 meses, 1 dev | R$ 30k | Q1/2025 |
| 2 | NF-e/NFS-e | 3 meses, 2 devs | R$ 90k | Q2/2025 |
| 3 | Receitas Digitais CFM+ANVISA | 3 meses, 2 devs | R$ 90k | Q2/2025 |
| 4 | TISS Fase 1 | 3 meses, 2-3 devs | R$ 135k | Q4/2025 |
| 5 | Conformidade CFM 1.638 | 1.5 meses, 1 dev | R$ 22.5k | Q1/2025 |
| 6 | SNGPC ANVISA | 2 meses, 1 dev | R$ 30k | Q2/2025 |
| 7 | Conformidade CFM 2.314 | 2 meses, 1 dev | R$ 30k | Q3/2025 |
| 8 | Telemedicina | 4-6 meses, 2 devs | R$ 135k | Q3/2025 |
| **TOTAL P0** | **22-28 meses/dev** | **R$ 562.5k** | **2025** |

---

**📌 PRÓXIMO PASSO:** Começar imediatamente pela Tarefa #1 (Conformidade CFM 1.821) em Janeiro/2025.

---

**Documento Criado Por:** GitHub Copilot  
**Data:** Dezembro 2024  
**Versão:** 1.0  
**Status:** Pronto para execução

**Este documento serve como roteiro detalhado de desenvolvimento do PrimeCare Software para 2025-2026, com foco em compliance regulatório e crescimento de mercado.**

# 📋 Plano de Desenvolvimento Priorizado - Parte 2
## Prioridades Médias e Baixas + Apps

> **Complemento do documento principal**  
> **Foco:** Tarefas P1 (Alta), P2 (Média) e P3 (Baixa) + Aplicativos

---

## 🔥🔥 PRIORIDADE ALTA (P1)

### 9️⃣ AUDITORIA COMPLETA (LGPD)

**Prioridade:** 🔥🔥 P1 - ALTA  
**Obrigatoriedade:** Legal (LGPD)  
**Prazo:** Q1/2025 (Janeiro-Março 2025)  
**Esforço:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000

#### Por que é Alto?
- **LGPD é lei** desde 2020 com multas pesadas
- Empresas de saúde são **alvo prioritário** da ANPD
- Sem auditoria, impossível comprovar compliance
- **Rastreabilidade** é requisito fundamental

#### O que precisa ser feito?

**Etapa 1: Modelagem de Auditoria (1 semana)**
```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string Action { get; set; }  // CREATE, READ, UPDATE, DELETE, LOGIN
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string OldValues { get; set; }  // JSON before
    public string NewValues { get; set; }  // JSON after
    public string Result { get; set; }  // SUCCESS, FAILED, UNAUTHORIZED
    public string FailureReason { get; set; }
}
```

**Etapa 2: Implementação Backend (3 semanas)**
1. Criar `AuditService` central
2. Interceptor global para logar ações
3. Eventos de domínio para auditoria
4. Armazenamento otimizado (índices)
5. Retenção de 7-10 anos

**Etapa 3: Eventos a Auditar (2 semanas)**
- **Autenticação:** login, logout, falhas, MFA
- **Autorização:** acesso negado, tentativas
- **Dados Sensíveis:** prontuários, documentos, exports
- **Configurações:** alterações de sistema

**Etapa 4: Frontend - Visualização (2 semanas)**
1. Tela de logs de auditoria
2. Filtros avançados (usuário, ação, período)
3. Exportação para análise
4. Dashboard de atividades suspeitas

**Etapa 5: LGPD Específico (1 semana)**
1. Registro de consentimentos
2. Direito ao esquecimento (soft delete melhorado)
3. Portabilidade de dados (export JSON/XML)
4. Relatório de atividades por paciente

**Etapa 6: Testes (1 semana)**
1. Verificar logging em todas as operações
2. Performance (não pode afetar aplicação)
3. Retenção de logs
4. Compliance LGPD

**Etapa 7: Deploy (1 semana)**
1. Deploy gradual
2. Monitoramento de performance
3. Documentação de compliance LGPD

#### Entregáveis
- [ ] Sistema de auditoria completo
- [ ] Logs de todas as ações sensíveis
- [ ] Interface de visualização
- [ ] Relatórios LGPD
- [ ] Retenção de 7+ anos

#### Critérios de Sucesso
- 100% das operações sensíveis logadas
- Impacto de performance < 5%
- Exportação de dados em < 30s
- Aprovação de consultor LGPD

---

### 🔟 CRIPTOGRAFIA DE DADOS MÉDICOS

**Prioridade:** 🔥🔥 P1 - ALTA  
**Obrigatoriedade:** Best Practice + LGPD  
**Prazo:** Q1/2025 (Janeiro-Março 2025)  
**Esforço:** 1-2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 22.500

#### Por que é Alto?
- **Dados de saúde são ultra-sensíveis**
- LGPD exige proteção adequada
- Vazamento pode custar milhões
- **Compliance e confiança** dos clientes

#### O que precisa ser feito?

**Etapa 1: Escolha de Estratégia (1 semana)**
1. Avaliar opções:
   - **TDE** (Transparent Data Encryption) - DB nível
   - **Criptografia Application-Level** - mais controle
   - **Azure Key Vault / AWS KMS** - gestão de chaves
2. Decisão: Application-Level + Key Vault (recomendado)

**Etapa 2: Setup de Key Management (1 semana)**
1. Configurar Azure Key Vault ou AWS KMS
2. Criar master key
3. Rotação automática de chaves
4. Backup de chaves

**Etapa 3: Serviço de Criptografia (2 semanas)**
```csharp
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    byte[] EncryptBytes(byte[] data);
    byte[] DecryptBytes(byte[] encryptedData);
}

// Implementação com AES-256-GCM
public class AesGcmEncryptionService : IEncryptionService
{
    // Usa Azure Key Vault para chaves
}
```

**Etapa 4: Identificar Dados Sensíveis (1 semana)**
- Prontuários completos
- Prescrições médicas
- CPF, RG, CNS
- Dados de saúde mental
- Resultados de exames
- Números de cartão (se armazenados)

**Etapa 5: Implementação Backend (3 semanas)**
1. Atributo `[Encrypted]` em propriedades
2. Interceptor Entity Framework para criptografar/descriptografar
3. Migration para criptografar dados existentes
4. Índices em campos criptografados (hashed)

**Etapa 6: Performance (1 semana)**
1. Cache de chaves de criptografia
2. Otimização de queries
3. Benchmark antes/depois

**Etapa 7: Testes (1 semana)**
1. Verificar criptografia em repouso
2. Testar descriptografia
3. Performance tests
4. Disaster recovery (perda de chave)

**Etapa 8: Deploy (1 semana)**
1. Migration de dados existentes (pode demorar)
2. Deploy gradual
3. Monitoramento
4. Documentação

#### Entregáveis
- [ ] Dados sensíveis criptografados em repouso
- [ ] Gerenciamento de chaves no Azure/AWS
- [ ] Rotação automática de chaves
- [ ] Performance aceitável

#### Critérios de Sucesso
- 100% dos dados sensíveis criptografados
- Chaves NUNCA no código ou banco
- Rotação de chaves automática
- Impacto performance < 10%

---

### 1️⃣1️⃣ PORTAL DO PACIENTE

**Prioridade:** 🔥🔥 P1 - ALTA  
**Impacto:** Muito Alto - Redução de custos  
**Prazo:** Q2/2025 (Abril-Junho 2025)  
**Esforço:** 2-3 meses | 2 desenvolvedores  
**Custo Estimado:** R$ 90.000

#### Por que é Alto?
- **90% dos concorrentes** já têm
- Reduz **40-50% de ligações** na recepção
- Reduz **no-show em 30-40%**
- **ROI muito rápido** (< 6 meses)

#### O que precisa ser feito?

**Etapa 1: Novo Projeto Angular (1 semana)**
```
frontend/patient-portal/
├── src/
│   ├── app/
│   │   ├── pages/
│   │   │   ├── login/
│   │   │   ├── register/
│   │   │   ├── dashboard/
│   │   │   ├── appointments/
│   │   │   ├── documents/
│   │   │   └── profile/
│   │   ├── services/
│   │   └── guards/
│   └── assets/
```

**Etapa 2: Backend - API Paciente (2 semanas)**
1. Criar endpoints específicos para paciente
2. Autenticação separada (CPF + senha)
3. Permissões restritas (só próprios dados)
4. Rate limiting mais rigoroso

**Etapa 3: Autenticação Paciente (2 semanas)**
1. Cadastro self-service
2. Validação de CPF
3. Confirmação por email/SMS
4. Login seguro
5. Recuperação de senha
6. 2FA opcional

**Etapa 4: Dashboard (2 semanas)**
1. Próximas consultas
2. Histórico de atendimentos
3. Documentos recentes
4. Prescrições ativas
5. Ações rápidas

**Etapa 5: Agendamento Online (3 semanas)**
1. Ver disponibilidade de médicos
2. Filtrar por especialidade
3. Agendar nova consulta
4. Reagendar consulta existente
5. Cancelar (com políticas)
6. Notificações de confirmação

**Etapa 6: Confirmação de Consultas (1 semana)**
1. Notificação 24h antes
2. Botões: Confirmar ou Cancelar
3. Lembrete no dia (2h antes)

**Etapa 7: Documentos (2 semanas)**
1. Listagem de documentos (receitas, atestados, laudos)
2. Download de PDF
3. Compartilhamento via WhatsApp/Email
4. Histórico de prontuário (resumido)

**Etapa 8: Telemedicina (se disponível) (1 semana)**
1. Botão "Entrar na consulta"
2. Teste de equipamento
3. Sala de espera
4. Link direto para videochamada

**Etapa 9: Pagamentos (futuro) (2 semanas)**
1. Ver faturas pendentes
2. Pagar online (cartão, PIX)
3. Histórico de pagamentos
4. Notas fiscais

**Etapa 10: Design e UX (2 semanas)**
1. Design responsivo (mobile-first)
2. Acessibilidade WCAG 2.1
3. Cores e identidade visual amigável
4. PWA (Progressive Web App)

**Etapa 11: Testes (2 semanas)**
1. Testes com pacientes reais
2. Usabilidade
3. Performance
4. Segurança

**Etapa 12: Deploy (1 semana)**
1. Deploy em produção
2. Campanha de divulgação
3. Onboarding de pacientes
4. Suporte dedicado

#### Entregáveis
- [ ] Portal web responsivo
- [ ] Autenticação segura
- [ ] Agendamento online
- [ ] Confirmação de consultas
- [ ] Download de documentos
- [ ] Integração com telemedicina

#### Critérios de Sucesso
- 50%+ dos pacientes se cadastram
- Redução de 40%+ em ligações
- Redução de 30%+ em no-show
- NPS do portal > 8.0
- Tempo de carregamento < 3s

---

### 1️⃣2️⃣ PRONTUÁRIO SOAP ESTRUTURADO

**Prioridade:** 🔥🔥 P1 - ALTA  
**Impacto:** Médio - Qualidade e Padronização  
**Prazo:** Q1/2025 (Janeiro-Março 2025)  
**Esforço:** 1-2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 22.500

#### Por que é Alto?
- **Padrão internacional** de prontuário
- Facilita IA e análise de dados no futuro
- **Compliance** com boas práticas médicas
- Melhora qualidade do atendimento

#### O que precisa ser feito?

**Etapa 1: Estudo SOAP (1 semana)**
```
SOAP:
- S (Subjetivo): Queixa principal, sintomas, história
- O (Objetivo): Sinais vitais, exame físico, resultados
- A (Avaliação): Diagnósticos, CID-10, hipóteses
- P (Plano): Prescrição, exames, retorno, orientações
```

**Etapa 2: Modelagem (1 semana)**
```csharp
public class SOAPMedicalRecord
{
    // Subjetivo
    public string ChiefComplaint { get; set; }
    public string HistoryOfPresentIllness { get; set; }
    public string ReviewOfSystems { get; set; }
    
    // Objetivo
    public VitalSigns VitalSigns { get; set; }
    public string PhysicalExamination { get; set; }
    public string LabResults { get; set; }
    
    // Avaliação
    public List<Diagnosis> Diagnoses { get; set; }  // Com CID-10
    public string DifferentialDiagnosis { get; set; }
    
    // Plano
    public List<Prescription> Prescriptions { get; set; }
    public List<LabOrder> LabOrders { get; set; }
    public string Instructions { get; set; }
    public DateTime? FollowUpDate { get; set; }
}
```

**Etapa 3: Backend (2 semanas)**
1. Criar entidades SOAP
2. APIs para cada seção
3. Validações
4. Migration

**Etapa 4: Frontend - Estrutura (3 semanas)**
1. Dividir prontuário em 4 abas (S-O-A-P)
2. Campos específicos por seção
3. Autocomplete onde possível
4. Validações visuais

**Etapa 5: Templates por Especialidade (2 semanas)**
1. Cardiologia
2. Pediatria
3. Dermatologia
4. Ortopedia
5. Clínica Geral

**Etapa 6: Migração (1 semana)**
1. Manter prontuários antigos como "texto livre"
2. Novos obrigatoriamente SOAP
3. Opção de converter antigos

**Etapa 7: Testes (1 semana)**
1. Testar com médicos
2. Feedback de usabilidade
3. Ajustes

**Etapa 8: Deploy (1 semana)**
1. Deploy gradual
2. Treinamento
3. Documentação

#### Entregáveis
- [ ] Prontuário estruturado SOAP
- [ ] Templates por especialidade
- [ ] Validações e campos obrigatórios
- [ ] Migração de prontuários antigos

#### Critérios de Sucesso
- 100% dos novos prontuários em formato SOAP
- Tempo de preenchimento < 10 min
- Aprovação de médicos
- Dados estruturados para IA futura

---

### 1️⃣3️⃣ MELHORIAS DE SEGURANÇA

**Prioridade:** 🔥🔥 P1 - ALTA  
**Impacto:** Alto - Segurança crítica  
**Prazo:** Q1-Q2/2025  
**Esforço:** 3 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 45.000

#### Conjunto de Melhorias

**13.1 - Bloqueio de Conta por Tentativas Falhadas**
- Esforço: 2 semanas
- Contador de tentativas falhadas
- Bloqueio progressivo (5min → 15min → 1h → 24h)
- Notificação por email
- Log de todas as tentativas

**13.2 - MFA Obrigatório para Administradores**
- Esforço: 2 semanas
- Expandir 2FA atual
- Suporte TOTP (Google Authenticator)
- Códigos de backup
- U2F/FIDO2 (YubiKey) futuro

**13.3 - WAF (Web Application Firewall)**
- Esforço: 1 mês
- Cloudflare WAF (recomendado)
- Regras OWASP CRS
- Rate limiting avançado
- Bot detection

**13.4 - SIEM (Centralização de Logs)**
- Esforço: 1 mês
- ELK Stack (Elasticsearch + Logstash + Kibana)
- Serilog integration
- Dashboards de segurança
- Alertas automáticos

**13.5 - Refresh Token Pattern**
- Esforço: 2 semanas
- Access token curto (15 min)
- Refresh token longo (7-30 dias)
- Rotação de tokens
- Revogação granular

**13.6 - Pentest Profissional**
- Esforço: Contratação externa
- Semestral ou anual
- Custo: R$ 15-30k por teste
- Empresas: Morphus, Clavis, Tempest

#### Entregáveis
- [ ] Bloqueio automático de contas
- [ ] MFA obrigatório para admins
- [ ] WAF configurado
- [ ] SIEM funcionando
- [ ] Refresh tokens implementados
- [ ] Relatório de pentest

#### Critérios de Sucesso
- Zero ataques de força bruta bem-sucedidos
- 100% dos admins com MFA
- WAF bloqueando > 90% dos ataques
- SIEM com alertas funcionando
- Tokens revogáveis em < 1s

---

## 🔥 PRIORIDADE MÉDIA (P2)

### 1️⃣4️⃣ INTEGRAÇÃO TISS - FASE 2

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q1/2026  
**Esforço:** 3 meses | 2-3 devs  
**Custo:** R$ 135.000

#### O que precisa ser feito?
1. Webservices de operadoras
2. Conferência automática de glosas
3. Recurso de glosa
4. Relatórios avançados
5. Dashboard de performance por operadora
6. Análise histórica

---

### 1️⃣5️⃣ SISTEMA DE FILA DE ESPERA

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q2/2026  
**Esforço:** 2-3 meses | 2 devs  
**Custo:** R$ 90.000

#### O que precisa ser feito?
1. Totem de autoatendimento
2. Geração de senha
3. Painel de TV (SignalR real-time)
4. Priorização (idosos, gestantes, urgência)
5. Estimativa de tempo de espera
6. Notificações SMS/App

---

### 1️⃣6️⃣ BI E ANALYTICS AVANÇADOS

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q2/2026  
**Esforço:** 3-4 meses | 2 devs  
**Custo:** R$ 110.000

#### Dashboards
1. **Clínico:** ocupação, tempo de consulta, diagnósticos
2. **Financeiro:** receita, ticket médio, projeções
3. **Operacional:** tempo de espera, eficiência
4. **Qualidade:** NPS, satisfação, reclamações

#### Análise Preditiva (ML.NET)
- Previsão de demanda
- Risco de no-show
- Identificação de padrões
- Recomendações

---

### 1️⃣7️⃣ ASSINATURA DIGITAL (ICP-BRASIL)

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q3/2026  
**Esforço:** 2-3 meses | 2 devs  
**Custo:** R$ 90.000

#### O que precisa ser feito?
1. Integração com ICP-Brasil
2. Suporte A1 (software) e A3 (token)
3. Assinatura de prontuários
4. Assinatura de receitas
5. Assinatura de atestados e laudos
6. Timestamping
7. Validação de assinaturas

---

### 1️⃣8️⃣ CRM AVANÇADO

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q3-Q4/2025  
**Esforço:** 3-4 meses | 2 devs  
**Custo:** R$ 110.000

#### Funcionalidades
1. **Jornada do Paciente:** 7 estágios mapeados
2. **Automação de Marketing:** Campanhas segmentadas
3. **NPS/CSAT:** Pesquisas automáticas
4. **Ouvidoria:** Gestão de reclamações
5. **Análise de Sentimento:** IA em feedbacks

---

### 1️⃣9️⃣ GESTÃO FISCAL E CONTÁBIL

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q3/2025  
**Esforço:** 2 meses | 1-2 devs  
**Custo:** R$ 45.000

#### O que precisa ser feito?
1. Controle tributário (ISS, PIS, COFINS, IR, CSLL)
2. DAS (Simples Nacional)
3. Integração contábil (Domínio, ContaAzul, Omie)
4. Plano de contas
5. DRE e Balancete
6. Exportação SPED

---

### 2️⃣0️⃣ ACESSIBILIDADE DIGITAL (LBI)

**Prioridade:** 🔥 P2 - MÉDIA  
**Prazo:** Q3/2025  
**Esforço:** 1.5 meses | 1 dev frontend  
**Custo:** R$ 22.500

#### O que precisa ser feito?
1. Auditoria com axe, WAVE
2. WCAG 2.1 nível AA
3. Navegação por teclado
4. Compatibilidade com leitores de tela
5. Contraste adequado
6. Textos alternativos
7. Testes com usuários com deficiência

---

## ⚪ PRIORIDADE BAIXA (P3)

### 2️⃣1️⃣ API PÚBLICA

**Esforço:** 1-2 meses | 1 dev  
**Prazo:** Q3/2026

---

### 2️⃣2️⃣ INTEGRAÇÃO COM LABORATÓRIOS

**Esforço:** 4-6 meses | 2 devs  
**Prazo:** Q4/2026

---

### 2️⃣3️⃣ MARKETPLACE PÚBLICO

**Esforço:** 3-4 meses | 2 devs  
**Prazo:** 2027+

---

### 2️⃣4️⃣ PROGRAMA DE INDICAÇÃO

**Esforço:** 1-2 meses | 1 dev  
**Prazo:** 2027+

---

## 📱 APLICATIVOS MOBILE

### iOS App

**Prioridade Alta:**
1. Criar/Editar Paciente (2 semanas)
2. Criar/Editar Agendamento (2 semanas)
3. Prontuários (2 semanas)
4. Notificações Push (1 semana)
5. Biometria (1 semana)

**Prioridade Média:**
6. Modo Offline (3 semanas)
7. Telemedicina (3 semanas)
8. Upload de Fotos (1 semana)
9. Widget iOS (2 semanas)

**Total iOS:** ~4-5 meses | 1 dev iOS

---

### Android App

**Prioridade Alta:**
1. Completar Pacientes (2 semanas)
2. Completar Agendamentos (2 semanas)
3. Criar/Editar Paciente (2 semanas)
4. Criar/Editar Agendamento (2 semanas)
5. Notificações Push (1 semana)
6. Prontuários (2 semanas)

**Prioridade Média:**
7. Biometria (1 semana)
8. Modo Offline (3 semanas)
9. Telemedicina (3 semanas)
10. Widget Android (2 semanas)

**Total Android:** ~5-6 meses | 1 dev Android

---

## 🌐 APLICATIVOS WEB

### PrimeCare Software App (Frontend Principal)

**Prioridade Alta:**
1. Dashboard de Relatórios (4 semanas)
2. Módulo Financeiro (6 semanas)
3. Notificações em Tempo Real (2 semanas)

**Prioridade Média:**
4. Multiidioma (3 semanas)
5. Modo Offline (4 semanas)
6. Exportação de Dados (2 semanas)

---

### MW System Admin

**Prioridade Alta:**
1. Gestão de System Owners (2 semanas)
2. Gestão de Planos (3 semanas)
3. Relatórios Financeiros (3 semanas)
4. Auditoria Global (2 semanas)

**Prioridade Média:**
5. Dashboard Analytics (4 semanas)
6. Feature Flags (2 semanas)
7. Comunicação em Massa (3 semanas)

---

### MW Site (Marketing)

**Prioridade Alta:**
1. Blog (3 semanas)
2. Cases de Sucesso (2 semanas)
3. FAQ (1 semana)
4. Chat Online (2 semanas)
5. SEO Avançado (2 semanas)

**Prioridade Média:**
6. Calculadora ROI (2 semanas)
7. Tour Virtual (3 semanas)
8. Comparativo (2 semanas)

---

### MW Docs (Documentação)

**Prioridade Alta:**
1. Versionamento (2 semanas)
2. Edição Online (3 semanas)
3. PDF Export (1 semana)
4. Índice Automático (1 semana)

**Prioridade Média:**
5. Comentários (2 semanas)
6. Dark Mode (1 semana)
7. Compartilhamento (1 semana)

---

## 📊 CRONOGRAMA CONSOLIDADO 2025-2026

### 2025

**Q1 (Jan-Mar):**
- ✅ P0: CFM 1.821, CFM 1.638
- ✅ P1: Auditoria LGPD, Criptografia, SOAP
- ✅ P1: Segurança (bloqueio, MFA)

**Q2 (Abr-Jun):**
- ✅ P0: NF-e/NFS-e, Receitas Digitais, SNGPC
- ✅ P1: Portal do Paciente
- ✅ P1: Segurança (WAF, SIEM, Refresh Token)

**Q3 (Jul-Set):**
- ✅ P0: Telemedicina, CFM 2.314
- ✅ P2: CRM, Acessibilidade
- ✅ P2: Fiscal e Contábil

**Q4 (Out-Dez):**
- ✅ P0: TISS Fase 1
- ✅ P2: Marketing, NPS, Ouvidoria

### 2026

**Q1 (Jan-Mar):**
- ✅ P2: TISS Fase 2

**Q2 (Abr-Jun):**
- ✅ P2: BI Avançado, Fila de Espera

**Q3 (Jul-Set):**
- ✅ P2: Assinatura Digital, IP Blocking
- ✅ P3: API Pública, Anamnese Guiada

**Q4 (Out-Dez):**
- ✅ P3: Laboratórios

---

## 💰 INVESTIMENTO TOTAL RESUMIDO

| Ano | P0 (Crítico) | P1 (Alto) | P2 (Médio) | P3 (Baixo) | Apps | TOTAL |
|-----|--------------|-----------|------------|-----------|------|-------|
| **2025** | R$ 532.5k | R$ 210k | R$ 110k | - | R$ 120k | **R$ 972.5k** |
| **2026** | R$ 30k | - | R$ 425k | R$ 180k | R$ 150k | **R$ 785k** |
| **TOTAL** | **R$ 562.5k** | **R$ 210k** | **R$ 535k** | **R$ 180k** | **R$ 270k** | **R$ 1.757.5M** |

---

## 📝 NOTAS FINAIS

### Priorização Dinâmica
- Prioridades podem mudar conforme feedback de mercado
- Tarefas P0 são inegociáveis (obrigatórias por lei)
- Tarefas P1-P3 podem ser reorganizadas

### Recursos Humanos
- 2-3 desenvolvedores backend (.NET)
- 1-2 desenvolvedores frontend (Angular)
- 1 desenvolvedor iOS
- 1 desenvolvedor Android
- 1 DevOps/Infra
- 1 QA
- 1 Product Owner
- **Total:** 7-10 pessoas

### Gestão
- Sprints de 2 semanas
- Retrospectivas quinzenais
- Review com stakeholders mensais
- Atualizaçãodeste documento trimestralmente

---

**Documento Criado Por:** GitHub Copilot  
**Data:** Dezembro 2024  
**Versão:** 1.0  

**Use em conjunto com PLANO_DESENVOLVIMENTO_PRIORIZADO.md (Parte 1) para visão completa.**
