# 🏥 TISS (Troca de Informações na Saúde Suplementar) - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar a integração com o padrão TISS (Troca de Informações na Saúde Suplementar) da ANS (Agência Nacional de Saúde Suplementar), incluindo geração de guias, envio para operadoras e processamento de retornos.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend)
- Usuário com perfil Owner ou Secretary logado
- Convênios cadastrados
- Pacientes com carteirinhas de convênio
- Procedimentos TUSS configurados
- Tabela TUSS importada

## 📖 Índice

1. [O que é TISS](#o-que-é-tiss)
2. [Configuração Inicial](#configuração-inicial)
3. [Cenários de Teste - Guias](#cenários-de-teste---guias)
4. [Cenários de Teste - Lotes](#cenários-de-teste---lotes)
5. [Cenários de Teste - Processamento](#cenários-de-teste---processamento)
6. [Cenários de Teste - Glosas](#cenários-de-teste---glosas)
7. [Validações e Erros Comuns](#validações-e-erros-comuns)
8. [API Testing](#api-testing)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 O que é TISS

TISS (Troca de Informações na Saúde Suplementar) é o **padrão obrigatório** estabelecido pela ANS para:

- 📋 Troca de informações entre prestadores e operadoras de planos de saúde
- 💰 Faturamento de procedimentos médicos
- 🔄 Solicitação de autorizações
- 📊 Auditoria e controle de qualidade

### Tipos de Guias TISS

1. **Guia de Consulta** - Consultas médicas
2. **Guia de SP/SADT** - Serviços Profissionais e Serviços de Apoio Diagnóstico e Terapia
3. **Guia de Internação** - Internações hospitalares
4. **Guia de Honorários** - Honorários médicos
5. **Guia de Urgência/Emergência** - Atendimentos emergenciais
6. **Guia de Odontologia** - Procedimentos odontológicos

### Versão TISS Implementada

O Omni Care Software suporta **TISS 4.03.00** (versão vigente - Janeiro 2026)

---

## 🔧 Configuração Inicial

### 1. Importar Tabela TUSS

**Passos:**
1. Baixe a tabela TUSS atualizada do site da ANS
2. Acesse **"Configurações"** → **"TISS/TUSS"** → **"Importar Tabela"**
3. Selecione o arquivo XML da tabela TUSS
4. Clique em **"Importar"**
5. Aguarde processamento (pode levar alguns minutos)

**Resultado Esperado:**
- ✅ Tabela importada com sucesso
- ✅ Procedimentos disponíveis para uso
- ✅ Códigos TUSS validados

---

### 2. Cadastrar Operadoras de Plano de Saúde

**Passos:**
1. Acesse **"Configurações"** → **"Convênios"**
2. Clique em **"+ Nova Operadora"**
3. Preencha:
   - **Nome:** Unimed São Paulo
   - **Registro ANS:** 358428
   - **CNPJ:** 43.202.472/0001-50
   - **Tipo:** Cooperativa Médica
   - **Email Faturamento:** faturamento@unimed.com.br
   - **Telefone:** (11) 3003-1150
   
4. **Configurações TISS:**
   - **Aceita TISS XML:** Sim
   - **Versão TISS:** 4.03.00
   - **URL WebService:** https://api.unimed.com.br/tiss
   - **Requer Autorização Prévia:** Sim
   - **Prazo Pagamento:** 30 dias

5. **Tabela de Valores:**
   - Importar tabela própria do convênio
   - Ou usar valores padrão CBHPM

6. Salve

**Resultado Esperado:**
- ✅ Operadora cadastrada
- ✅ Integração TISS configurada
- ✅ Tabela de valores carregada

---

### 3. Configurar Prestador

**Passos:**
1. Acesse **"Configurações"** → **"TISS/TUSS"** → **"Dados do Prestador"**
2. Preencha dados da sua clínica:
   - **CNPJ:** 12.345.678/0001-99
   - **Razão Social:** Clínica Médica ABC Ltda
   - **CNES:** 1234567
   - **Endereço Completo**
   
3. **Profissionais:**
   - Vincule os médicos
   - Adicione CRM de cada um
   - Conselho Regional

4. Salve

**Resultado Esperado:**
- ✅ Dados do prestador configurados
- ✅ Médicos vinculados
- ✅ Pronto para gerar guias

---

### 4. Verificar Permissões

**Perfis com acesso ao TISS:**
- ✅ Owner (acesso total)
- ✅ Secretary (geração de guias)
- ✅ Medic (apenas suas guias)
- ❌ Nurse (sem acesso)

---

## 🧪 Cenários de Teste - Guias

### Cenário 1.1: Gerar Guia de Consulta

**Objetivo:** Criar guia TISS para consulta médica

**Passos:**
1. Finalize uma consulta de paciente com convênio
2. Sistema detecta convênio automaticamente
3. Clique em **"Gerar Guia TISS"**
4. Preencha dados da guia:
   
   **Dados da Guia:**
   - **Número da Guia:** (gerado automaticamente)
   - **Data de Atendimento:** 22/01/2026
   - **Tipo de Consulta:** Consulta no consultório
   - **Caráter do Atendimento:** Eletivo
   
   **Dados do Beneficiário:**
   - **Carteirinha:** 123456789012345 (auto-preenchido)
   - **Nome:** Maria Silva Santos (auto-preenchido)
   - **Data de Nascimento:** 15/05/1985
   
   **Dados do Procedimento:**
   - **Código TUSS:** 10101012 (Consulta médica)
   - **Descrição:** Consulta em consultório
   - **Quantidade:** 1
   - **Valor Unitário:** R$ 200,00
   - **Valor Total:** R$ 200,00
   
   **Dados do Profissional:**
   - **Nome:** Dr. João Santos (auto-preenchido)
   - **CRM:** 12345
   - **UF:** SP

5. Clique em **"Gerar Guia"**

**Resultado Esperado:**
- ✅ Guia TISS gerada (XML)
- ✅ Número único de guia
- ✅ Validação ANS aprovada
- ✅ PDF da guia para impressão
- ✅ Pronta para envio

---

### Cenário 1.2: Guia de SP/SADT (Exames)

**Objetivo:** Solicitar autorização para exames

**Passos:**
1. Durante consulta, médico solicita exames
2. Clique em **"Gerar Guia SP/SADT"**
3. Preencha:
   
   **Tipo de Guia:** Solicitação de autorização
   
   **Procedimentos:**
   - **Hemograma completo** (TUSS: 40304310)
     - Quantidade: 1
     - Valor: R$ 20,00
   
   - **Glicemia de jejum** (TUSS: 40301354)
     - Quantidade: 1
     - Valor: R$ 15,00
   
   - **TSH** (TUSS: 40316203)
     - Quantidade: 1
     - Valor: R$ 35,00
   
   **Indicação Clínica:** Investigação de fadiga e ganho de peso
   
   **CID-10:** R53 (Mal-estar e fadiga)

4. Gere a guia

**Resultado Esperado:**
- ✅ Guia SP/SADT gerada
- ✅ Múltiplos procedimentos incluídos
- ✅ Aguardando autorização da operadora
- ✅ Status: "Pendente"

---

### Cenário 1.3: Guia com Senha de Autorização

**Objetivo:** Registrar autorização prévia recebida

**Pré-requisito:** Operadora enviou senha de autorização

**Passos:**
1. Guia SP/SADT aguardando autorização
2. Operadora aprova e envia senha: **12345678**
3. No sistema, localize a guia
4. Clique em **"Adicionar Autorização"**
5. Informe:
   - **Senha/Número Autorização:** 12345678
   - **Data Autorização:** 22/01/2026
   - **Validade:** 23/02/2026 (30 dias)
6. Salve

**Resultado Esperado:**
- ✅ Status alterado para "Autorizado"
- ✅ Senha vinculada à guia
- ✅ Pode executar os procedimentos
- ✅ Guia pronta para faturamento

---

### Cenário 1.4: Guia de Urgência/Emergência

**Objetivo:** Atendimento emergencial

**Passos:**
1. Paciente chega em emergência
2. Crie novo atendimento
3. Marque **"Urgência/Emergência"**
4. Gere **"Guia de Urgência"**
5. Preencha:
   - **Tipo de Atendimento:** Emergência
   - **Motivo:** Dor torácica aguda
   - **Data/Hora:** 22/01/2026 03:30
   - **CID-10:** R07.2 (Dor precordial)
   
6. **Procedimentos Realizados:**
   - ECG (TUSS: 41101065) - R$ 50,00
   - Consulta emergência (TUSS: 10101039) - R$ 300,00
   - Medicações administradas

7. Gere a guia

**Resultado Esperado:**
- ✅ Guia de emergência gerada
- ✅ Não requer autorização prévia
- ✅ Pode ser faturada imediatamente
- ✅ Prazo de envio: até 48h

---

### Cenário 1.5: Guia com Glosa Parcial (Teste de Correção)

**Objetivo:** Corrigir e reenviar guia glosada

**Contexto:** Operadora glosou item por erro de código

**Passos:**
1. Receba retorno com glosa:
   - **Motivo:** Código TUSS incorreto
   - **Item Glosado:** Procedimento 2
   
2. Abra a guia glosada
3. Clique em **"Corrigir e Reenviar"**
4. Corrija o código TUSS
5. Adicione justificativa
6. Reenvie

**Resultado Esperado:**
- ✅ Guia corrigida
- ✅ Nova versão gerada
- ✅ Histórico de alterações mantido
- ✅ Status: "Reenviado"

---

## 🧪 Cenários de Teste - Lotes

### Cenário 2.1: Criar Lote de Faturamento

**Objetivo:** Agrupar guias para envio

**Passos:**
1. Acesse **"TISS"** → **"Lotes de Faturamento"**
2. Clique em **"+ Novo Lote"**
3. Configure:
   - **Operadora:** Unimed São Paulo
   - **Período:** 01/01/2026 a 31/01/2026
   - **Competência:** Janeiro/2026
   
4. Sistema lista guias elegíveis:
   - 45 Guias de Consulta
   - 12 Guias SP/SADT
   - 3 Guias de Urgência
   - **Total:** 60 guias

5. Selecione todas as guias autorizadas
6. Clique em **"Criar Lote"**

**Resultado Esperado:**
- ✅ Lote criado com número único
- ✅ 60 guias incluídas
- ✅ Valor total: R$ 15.500,00
- ✅ Arquivo XML gerado
- ✅ Status: "Aguardando Envio"

---

### Cenário 2.2: Validar Lote

**Objetivo:** Verificar conformidade antes do envio

**Passos:**
1. Lote criado, clique em **"Validar Lote"**
2. Sistema executa validações:
   - ✅ Formato XML conforme XSD da ANS
   - ✅ Todas as guias têm autorização
   - ✅ Dados de beneficiários completos
   - ✅ Códigos TUSS válidos
   - ✅ Valores dentro da tabela
   - ❌ 2 guias com data futura (erro)

3. Corrija os erros apontados
4. Execute validação novamente

**Resultado Esperado:**
- ✅ Todas as validações aprovadas
- ✅ Lote pronto para envio
- ✅ Relatório de validação gerado

---

### Cenário 2.3: Enviar Lote para Operadora

**Objetivo:** Transmitir lote via WebService

**Pré-requisito:** Integração WebService configurada

**Passos:**
1. Lote validado, clique em **"Enviar Lote"**
2. Sistema conecta ao WebService da operadora
3. Envia arquivo XML
4. Aguarda protocolo de recebimento

**Resultado Esperado:**
- ✅ Lote enviado com sucesso
- ✅ Protocolo recebido: **2026012200123**
- ✅ Data/Hora de envio registrada
- ✅ Status: "Enviado - Aguardando Processamento"

---

### Cenário 2.4: Consultar Status do Lote

**Objetivo:** Verificar processamento pela operadora

**Passos:**
1. Acesse o lote enviado
2. Clique em **"Consultar Status"**
3. Sistema consulta WebService
4. Recebe retorno:
   - **Status:** Em análise
   - **Previsão:** 5 dias úteis
   - **Analista:** Maria Silva

5. Configure notificação automática

**Resultado Esperado:**
- ✅ Status atualizado
- ✅ Previsão de retorno informada
- ✅ Notificação quando processar

---

## 🧪 Cenários de Teste - Processamento

### Cenário 3.1: Importar Retorno da Operadora

**Objetivo:** Processar arquivo de retorno

**Passos:**
1. Operadora envia arquivo de retorno (XML)
2. Acesse **"TISS"** → **"Processar Retorno"**
3. Faça upload do arquivo XML
4. Clique em **"Processar"**
5. Sistema analisa:
   - 50 guias pagas
   - 8 guias glosadas
   - 2 guias negadas
   - Valor pago: R$ 13.200,00
   - Valor glosado: R$ 2.300,00

6. Confirme processamento

**Resultado Esperado:**
- ✅ Retorno processado
- ✅ Status de cada guia atualizado
- ✅ Valor a receber registrado
- ✅ Glosas destacadas para análise

---

### Cenário 3.2: Analisar Glosas

**Objetivo:** Revisar guias glosadas

**Passos:**
1. Acesse **"TISS"** → **"Glosas"**
2. Visualize lista de glosas:
   
   **Glosa 1:**
   - Guia: 123456
   - Motivo: Código incompatível com idade
   - Valor: R$ 200,00
   - Ação: Corrigir e recursar
   
   **Glosa 2:**
   - Guia: 123457
   - Motivo: Falta de autorização prévia
   - Valor: R$ 150,00
   - Ação: Providenciar autorização retroativa
   
   **Glosa 3:**
   - Guia: 123458
   - Motivo: Procedimento não coberto
   - Valor: R$ 300,00
   - Ação: Cobrar do paciente

3. Classifique cada glosa
4. Tome as ações necessárias

**Resultado Esperado:**
- ✅ Glosas analisadas e classificadas
- ✅ Plano de ação definido
- ✅ Recursos preparados
- ✅ Cobranças redirecionadas

---

### Cenário 3.3: Recurso de Glosa

**Objetivo:** Contestar glosa indevida

**Passos:**
1. Glosa: "Código incompatível com idade"
2. Análise: Código está correto
3. Clique em **"Recursar Glosa"**
4. Preencha:
   - **Motivo do Recurso:** Código adequado conforme TUSS vigente
   - **Justificativa Detalhada:** [texto explicativo]
   - **Anexos:** 
     - Documento do paciente
     - Prontuário da consulta
     - Tabela TUSS
   
5. Gere documento de recurso
6. Envie à operadora

**Resultado Esperado:**
- ✅ Recurso registrado
- ✅ Documentação anexada
- ✅ Protocolo gerado
- ✅ Status: "Em recurso"

---

### Cenário 3.4: Registrar Pagamento do Convênio

**Objetivo:** Baixar contas após recebimento

**Passos:**
1. Operadora paga: R$ 13.200,00
2. Acesse o lote processado
3. Clique em **"Registrar Pagamento"**
4. Preencha:
   - **Data Pagamento:** 25/02/2026
   - **Valor:** R$ 13.200,00
   - **Forma:** Transferência Bancária
   - **Conta:** Banco do Brasil
   - **Observações:** Ref. Lote 2026012200123

5. Confirme

**Resultado Esperado:**
- ✅ Pagamento registrado
- ✅ Todas as guias marcadas como pagas
- ✅ Entrada no fluxo de caixa
- ✅ Recibo gerado

---

## 🧪 Cenários de Teste - Glosas

### Cenário 4.1: Glosa Técnica

**Motivos Comuns:**
- Código TUSS incorreto
- CID incompatível
- Quantidade excessiva
- Data inválida

**Ação:** Corrigir e reenviar

---

### Cenário 4.2: Glosa Administrativa

**Motivos Comuns:**
- Falta de autorização
- Carteirinha inválida
- Dados incompletos
- Prazo de envio excedido

**Ação:** Completar documentação e recursar

---

### Cenário 4.3: Glosa por Negativa de Cobertura

**Motivo:** Procedimento não coberto pelo plano

**Ação:** 
1. Verificar contrato do beneficiário
2. Se realmente não coberto: cobrar do paciente
3. Se coberto: recursar com contrato

---

## ⚠️ Validações e Erros Comuns

### Erro 1: "Beneficiário não encontrado"

**Causa:** Número de carteirinha incorreto

**Solução:**
1. Verifique carteirinha com paciente
2. Confirme com operadora
3. Atualize cadastro

---

### Erro 2: "Código TUSS inválido"

**Causa:** Código inexistente ou desatualizado

**Solução:**
1. Consulte tabela TUSS atualizada
2. Use código correto
3. Atualize cadastro de procedimentos

---

### Erro 3: "Falta de autorização prévia"

**Causa:** Procedimento requer autorização não solicitada

**Solução:**
1. Solicite autorização antes
2. Aguarde aprovação
3. Vincule senha à guia

---

### Erro 4: "CID-10 incompatível com procedimento"

**Causa:** Diagnóstico não justifica o procedimento

**Solução:**
1. Revise CID informado
2. Adicione CID secundário se necessário
3. Justifique clinicamente

---

## 🔌 API Testing

### Endpoint: Gerar Guia TISS

```bash
curl -X POST "http://localhost:5000/api/tiss/guides" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "guideType": "Consultation",
    "appointmentId": "appointment-uuid",
    "healthInsuranceId": "insurance-uuid",
    "beneficiaryCard": "123456789012345",
    "procedures": [
      {
        "tussCode": "10101012",
        "description": "Consulta médica",
        "quantity": 1,
        "unitValue": 200.00
      }
    ],
    "professionalCrm": "12345",
    "cid10": "R07.2"
  }'
```

---

### Endpoint: Criar Lote

```bash
curl -X POST "http://localhost:5000/api/tiss/batches" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "healthInsuranceId": "insurance-uuid",
    "referenceMonth": "2026-01",
    "guideIds": ["guide-1", "guide-2", "guide-3"]
  }'
```

---

### Endpoint: Validar Lote

```bash
curl -X POST "http://localhost:5000/api/tiss/batches/{batch_id}/validate" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

### Endpoint: Enviar Lote

```bash
curl -X POST "http://localhost:5000/api/tiss/batches/{batch_id}/send" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

## 🐛 Troubleshooting

### Problema 1: XML não valida

**Causa:** Versão do schema desatualizada

**Solução:**
1. Baixe XSD atualizado da ANS
2. Atualize sistema
3. Revalide

---

### Problema 2: WebService não responde

**Causa:** Certificado digital expirado

**Solução:**
1. Renove certificado
2. Configure no sistema
3. Teste conexão

---

## ✅ Checklist de Validação Final

- [ ] Importar tabela TUSS
- [ ] Cadastrar operadora
- [ ] Configurar prestador
- [ ] Gerar guia de consulta
- [ ] Gerar guia SP/SADT
- [ ] Guia com autorização
- [ ] Guia de urgência
- [ ] Criar lote de faturamento
- [ ] Validar lote
- [ ] Enviar lote
- [ ] Consultar status
- [ ] Importar retorno
- [ ] Analisar glosas
- [ ] Recurso de glosa
- [ ] Registrar pagamento
- [ ] Testes de API

---

## 📚 Documentação Relacionada

- [TUSS (Tabela de Procedimentos)](05-TUSS-TABELA.md)
- [Guia do Usuário TISS](../GUIA_USUARIO_TISS.md)
- [Avaliação TISS/TUSS](../EVALUATION_SUMMARY_TISS_TUSS.md)
- [Status Implementação TISS](../TISS_PHASE1_IMPLEMENTATION_STATUS.md)

## 🔗 Links Úteis

- [Portal ANS](https://www.ans.gov.br)
- [Download Tabela TUSS](https://www.ans.gov.br/prestadores/tiss-troca-de-informacao-de-saude-suplementar)
- [Padrão TISS Completo](https://www.ans.gov.br/images/stories/prestadores/E-FISF-01.pdf)
