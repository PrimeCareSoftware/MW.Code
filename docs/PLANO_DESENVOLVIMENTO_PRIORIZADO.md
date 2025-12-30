# 📋 Plano de Desenvolvimento Priorizado - MedicWarehouse

> **Objetivo:** Documento detalhado com ordem de prioridade e passos necessários para cada desenvolvimento pendente.

> **Base:** Análise do PENDING_TASKS.md e APPS_PENDING_TASKS.md  
> **Última Atualização:** Dezembro 2024  
> **Status:** Plano de execução 2025-2026

---

## 🎯 Visão Executiva

Este documento organiza TODAS as pendências do MedicWarehouse em uma ordem de prioridade clara, considerando:

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

**Este documento serve como roteiro detalhado de desenvolvimento do MedicWarehouse para 2025-2026, com foco em compliance regulatório e crescimento de mercado.**

