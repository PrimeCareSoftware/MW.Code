# Guia do Usuário - TISS (Troca de Informações em Saúde Suplementar)

## 📋 Índice
1. [O que é TISS](#o-que-é-tiss)
2. [Para que serve](#para-que-serve)
3. [Como funciona no PrimeCare](#como-funciona-no-primecare)
4. [Passo a Passo](#passo-a-passo)
5. [Tipos de Guias TISS](#tipos-de-guias-tiss)
6. [Solicitação de Autorizações](#solicitação-de-autorizações)
7. [Criação de Guias](#criação-de-guias)
8. [Envio de Lotes](#envio-de-lotes)
9. [Acompanhamento](#acompanhamento)
10. [Perguntas Frequentes](#perguntas-frequentes)

---

## O que é TISS?

**TISS** significa **Troca de Informações em Saúde Suplementar** e é o padrão obrigatório estabelecido pela **ANS (Agência Nacional de Saúde Suplementar)** para troca de informações entre:

- **Prestadores de serviços de saúde** (clínicas, consultórios, hospitais, laboratórios)
- **Operadoras de planos de saúde** (Unimed, Amil, Bradesco Saúde, SulAmérica, etc.)

### Por que existe?

O TISS foi criado para **padronizar e simplificar** a comunicação entre clínicas e operadoras de planos de saúde, evitando que cada operadora tenha seu próprio formato e processo.

### O que o TISS padroniza?

- **Guias eletrônicas** para solicitação de procedimentos
- **Lotes de faturamento** para cobrança de serviços prestados
- **Demonstrativos de pagamento** das operadoras
- **Códigos de procedimentos** (tabela TUSS)
- **Formato dos arquivos XML** para envio eletrônico

---

## Para que serve?

O TISS serve para:

1. ✅ **Solicitar autorizações** de procedimentos antes de realizá-los
2. ✅ **Enviar guias de atendimento** para faturamento
3. ✅ **Agrupar múltiplas guias** em lotes para envio às operadoras
4. ✅ **Acompanhar o status** de autorizações e pagamentos
5. ✅ **Facilitar o recebimento** pelos serviços prestados

### Benefícios para sua clínica:

- 📈 **Agilidade no faturamento** - envio eletrônico é mais rápido que papel
- 💰 **Recebimento mais rápido** - menos erros = menos glosas
- 🎯 **Redução de erros** - sistema valida as informações automaticamente
- 📊 **Rastreabilidade** - histórico completo de cada guia
- ⏰ **Economia de tempo** - menos trabalho manual para equipe administrativa

---

## Como funciona no PrimeCare?

O PrimeCare Software gerencia todo o processo TISS de forma **automatizada**, desde a verificação do convênio do paciente até o recebimento do pagamento.

### Fluxo Completo:

```
1. Paciente chega com carteirinha do convênio
   ↓
2. Sistema verifica elegibilidade (se cadastrado)
   ↓
3. Procedimento é registrado no atendimento
   ↓
4. Sistema verifica se precisa de autorização prévia
   ↓
5. Se necessário, solicita autorização à operadora
   ↓
6. Após autorização, realiza o atendimento
   ↓
7. Sistema gera automaticamente a guia TISS
   ↓
8. Guias são agrupadas em lotes mensais
   ↓
9. Lote é enviado à operadora (XML ou portal)
   ↓
10. Operadora processa e retorna demonstrativo
    ↓
11. Sistema registra valores aprovados/glosados
    ↓
12. Operadora efetua o pagamento
```

---

## Passo a Passo

### 1️⃣ Cadastrar Operadoras de Planos de Saúde

**Menu:** Configurações → Convênios → Operadoras

1. Clique em **"Nova Operadora"**
2. Preencha os dados:
   - **Nome Comercial**: Ex: "Unimed Campinas"
   - **Razão Social**: Nome oficial da empresa
   - **Registro ANS**: Código de 6 dígitos obrigatório
   - **CNPJ**: Documento da operadora
   - **Contato**: Telefone e e-mail
3. Configure o **tipo de integração**:
   - **Manual**: Você preenche formulários em papel/PDF
   - **Portal Web**: Você acessa o portal da operadora
   - **XML TISS**: Sistema gera e envia arquivos XML automaticamente
   - **API REST**: Integração em tempo real (se disponível)
4. Se escolher **XML TISS**, preencha:
   - **Versão TISS**: Ex: "4.03.00"
   - **E-mail para lotes**: Onde enviar os arquivos XML
5. Clique em **"Salvar"**

### 2️⃣ Cadastrar Planos de Saúde

**Menu:** Configurações → Convênios → Planos

1. Clique em **"Novo Plano"**
2. Selecione a **operadora** cadastrada anteriormente
3. Preencha:
   - **Nome do Plano**: Ex: "Unimed Executivo"
   - **Código do Plano**: Código interno da operadora
   - **Registro ANS**: Código do plano na ANS
   - **Tipo**: Individual, Empresarial ou Coletivo
4. Marque as **coberturas**:
   - ☑️ Consultas
   - ☑️ Exames
   - ☑️ Procedimentos
5. Indique se **"Requer autorização prévia"**
6. Clique em **"Salvar"**

### 3️⃣ Cadastrar a Carteirinha do Paciente

**Menu:** Pacientes → Selecionar Paciente → Aba "Convênios"

1. Clique em **"Adicionar Convênio"**
2. Selecione a **operadora** e o **plano**
3. Preencha os dados da carteirinha:
   - **Número da Carteirinha**: Número impresso no cartão
   - **Código de Validação**: Código de barras ou validação
   - **Validade**: Data de início e fim (se houver)
4. Se o paciente for **dependente**, informe:
   - Nome do titular
   - CPF do titular
   - Relação (cônjuge, filho, etc.)
5. Clique em **"Salvar"**

### 4️⃣ Solicitar Autorização Prévia (se necessário)

**Menu:** Atendimento → Autorizações → Nova Solicitação

⚠️ **Importante**: Alguns procedimentos exigem autorização ANTES de serem realizados. Consulte o plano do paciente.

1. Selecione o **paciente**
2. Selecione o **convênio do paciente**
3. Escolha o **procedimento** que será realizado (código TUSS)
4. Preencha:
   - **Indicação clínica**: Motivo do procedimento
   - **Diagnóstico**: CID-10 do paciente
   - **Quantidade**: Número de sessões/procedimentos
   - **Data prevista**: Quando será realizado
5. Clique em **"Solicitar Autorização"**

**O que acontece:**
- Sistema gera uma solicitação de autorização
- Dependendo da integração, pode ser:
  - **Manual**: Imprimir e enviar para operadora
  - **Portal**: Link para acessar portal da operadora
  - **XML/API**: Enviado automaticamente

**Aguarde a resposta:**
- Status: **Pendente** → **Aprovado** ou **Negado**
- Se aprovado, anote o **número da autorização** (você vai precisar!)

### 5️⃣ Realizar o Atendimento

**Menu:** Atendimento → Agenda → Iniciar Atendimento

1. No **prontuário eletrônico**, registre:
   - Anamnese e exame físico
   - Diagnóstico (CID-10)
   - Procedimentos realizados
2. Ao adicionar um **procedimento**:
   - Selecione o **código TUSS** correto
   - Se houver autorização, informe o **número da autorização**
   - Sistema valida se o procedimento está coberto
3. Finalize o atendimento

**O que acontece:**
- Sistema registra que o procedimento foi realizado
- Guia TISS é criada automaticamente em status **"Rascunho"**
- Você pode editar antes de enviar

### 6️⃣ Revisar e Finalizar Guias

**Menu:** Faturamento → Guias TISS

1. Visualize as guias em **"Rascunho"**
2. Clique em cada guia para **revisar**:
   - ✅ Dados do paciente corretos
   - ✅ Número da autorização (se aplicável)
   - ✅ Procedimentos corretos com códigos TUSS
   - ✅ Valores unitários corretos
   - ✅ Data do atendimento correta
3. Se estiver tudo correto, clique em **"Finalizar Guia"**
4. Status muda para **"Pronto para Envio"**

### 7️⃣ Criar e Enviar Lote de Faturamento

**Menu:** Faturamento → Lotes TISS

1. Clique em **"Novo Lote"**
2. Selecione a **operadora**
3. Selecione o **período** (geralmente mensal)
4. Sistema mostra todas as guias **"Prontas para Envio"** daquela operadora
5. Revise o **valor total** do lote
6. Clique em **"Gerar Lote"**

**Dependendo da integração:**

**A) XML TISS (Automático):**
- Sistema gera arquivo XML no padrão TISS
- Clique em **"Baixar XML"** ou **"Enviar por E-mail"**
- Arquivo é enviado para o e-mail da operadora

**B) Portal Web:**
- Sistema gera as informações
- Clique em **"Exportar para Portal"**
- Acesse o portal da operadora
- Faça upload das guias/lote

**C) Manual:**
- Sistema gera PDF com as guias
- Imprima e envie fisicamente

7. Anote o **número do protocolo** de recebimento
8. Status do lote muda para **"Enviado"**

### 8️⃣ Acompanhar o Processamento

**Menu:** Faturamento → Lotes TISS → Ver Detalhes do Lote

**Status possíveis:**
- 📝 **Rascunho**: Lote em criação
- ✉️ **Enviado**: Lote enviado à operadora
- 🔄 **Em Processamento**: Operadora está analisando
- ✅ **Processado**: Operadora finalizou análise
- ⚠️ **Parcialmente Pago**: Algumas guias foram glosadas
- 💰 **Pago**: Lote pago integralmente
- ❌ **Rejeitado**: Lote rejeitado (verificar erros)

**Quando a operadora retornar o demonstrativo:**

1. Acesse o lote
2. Clique em **"Importar Retorno"**
3. Faça upload do arquivo de retorno XML
4. Sistema processa e atualiza:
   - ✅ Valores aprovados
   - ❌ Valores glosados (não pagos)
   - 📄 Motivos de glosa
5. Visualize os **totais**:
   - Valor solicitado
   - Valor aprovado
   - Valor glosado
   - Valor a receber

### 9️⃣ Contestar Glosas (se necessário)

**Menu:** Faturamento → Lotes TISS → Ver Guias com Glosa

Se uma guia foi glosada (não paga):

1. Clique na guia glosada
2. Veja o **motivo da glosa**:
   - Código do procedimento incorreto
   - Falta de autorização
   - Dados incompletos
   - Prazo expirado
   - Procedimento não coberto
3. Se discordar, clique em **"Contestar Glosa"**
4. Preencha a **justificativa** com documentação
5. Envie a contestação
6. Aguarde nova análise da operadora

### 🔟 Registrar o Pagamento

**Menu:** Faturamento → Lotes TISS → Registrar Pagamento

Quando o pagamento cair na conta:

1. Acesse o lote
2. Clique em **"Registrar Pagamento"**
3. Preencha:
   - Data do pagamento
   - Valor pago
   - Forma de pagamento (TED, DOC, etc.)
   - Banco/Agência/Conta
4. Se houver diferenças, registre o motivo
5. Clique em **"Confirmar Pagamento"**
6. Status muda para **"Pago"**

---

## Tipos de Guias TISS

O TISS define diferentes tipos de guias para diferentes situações:

### 1. Guia de Consulta
**Quando usar:** Consultas médicas simples em consultório

**Exemplos:**
- Consulta com clínico geral
- Retorno médico
- Consulta de rotina

**Informações necessárias:**
- Dados do paciente
- Dados do profissional (CRM/CRO)
- Data e hora da consulta
- Procedimento (código TUSS de consulta)

### 2. Guia SP/SADT (Serviços Profissionais e Apoio Diagnóstico)
**Quando usar:** Exames, terapias, pequenos procedimentos

**Exemplos:**
- Exames laboratoriais (hemograma, glicemia)
- Exames de imagem (raio-X, ultrassom, tomografia)
- Fisioterapia
- Psicoterapia
- Pequenas cirurgias ambulatoriais

**Informações necessárias:**
- Dados do paciente
- Indicação clínica (por que o exame/procedimento é necessário)
- Diagnóstico (CID-10)
- Procedimentos solicitados (códigos TUSS)
- Número de autorização (se necessário)
- Profissional solicitante e executante

### 3. Guia de Internação
**Quando usar:** Internações hospitalares

**Exemplos:**
- Cirurgias que requerem internação
- Tratamentos que exigem internação
- Emergências com internação

**Informações necessárias:**
- Dados do paciente
- Motivo da internação
- Tipo de acomodação (enfermaria, quarto, apartamento)
- Previsão de permanência
- Procedimentos que serão realizados
- Diagnóstico inicial

**Observação:** Durante a internação, são geradas **guias complementares**:
- Guia de Honorários (pagamento dos profissionais)
- Guia de Materiais e Medicamentos
- Guia de Taxas e Diárias

### 4. Guia de Honorários
**Quando usar:** Pagamento de profissionais em procedimentos

**Exemplos:**
- Cirurgião em uma cirurgia
- Anestesista
- Auxiliares e assistentes

**Informações necessárias:**
- Dados da cirurgia/procedimento principal
- Profissional a ser pago
- Papel do profissional (cirurgião, anestesista, etc.)
- Valor dos honorários

### 5. Guia de Tratamento Odontológico
**Quando usar:** Procedimentos odontológicos

**Exemplos:**
- Limpeza dental
- Restaurações
- Extrações
- Tratamento de canal

**Informações necessárias:**
- Dados do paciente
- Dentista (CRO)
- Procedimentos odontológicos (códigos TUSS odonto)
- Dentes tratados (identificação numérica)

### 6. Guia de Resumo de Internação
**Quando usar:** Ao final de uma internação

**Resumo da internação contendo:**
- Data de entrada e saída
- Diagnóstico inicial e final
- Procedimentos realizados
- Evolução do paciente
- Motivo da alta

---

## Solicitação de Autorizações

### Quando é necessário autorização prévia?

Não são todos os procedimentos que exigem autorização. Geralmente:

✅ **NÃO precisam de autorização:**
- Consultas simples
- Exames de rotina básicos
- Urgências e emergências

⚠️ **PRECISAM de autorização:**
- Exames mais complexos (ressonância, tomografia)
- Cirurgias
- Internações programadas
- Procedimentos de alto custo
- Fisioterapia (múltiplas sessões)
- Terapias especializadas

**Dica:** No PrimeCare, ao cadastrar o plano, marque quais tipos de procedimento requerem autorização. O sistema alertará automaticamente.

### Como solicitar:

1. **Antes de realizar o procedimento**
2. **Preencha a solicitação** com:
   - Indicação clínica detalhada (por que é necessário?)
   - Diagnóstico (CID-10)
   - Procedimento desejado (código TUSS)
   - Quantidade de sessões (se aplicável)
3. **Aguarde resposta** da operadora (prazo varia: 3 a 15 dias úteis)
4. **Se aprovado**, anote o número da autorização
5. **Se negado**, pode:
   - Contestar apresentando mais documentação
   - Optar por particular
   - Substituir por procedimento coberto

### Prazo de validade da autorização:

Autorizações têm prazo de validade. Geralmente:
- **Consultas/exames:** 30 a 60 dias
- **Cirurgias:** 60 a 90 dias
- **Terapias:** 3 a 6 meses (quantidade definida de sessões)

Se o prazo expirar, precisa **solicitar nova autorização**.

---

## Criação de Guias

### Guias são criadas automaticamente

Quando você registra um atendimento com procedimento de convênio, o sistema **cria automaticamente** uma guia TISS em status "Rascunho".

### O que você precisa fazer:

1. **Revisar a guia** antes de enviar
2. **Confirmar** que os dados estão corretos:
   - Paciente e convênio
   - Procedimentos e códigos TUSS
   - Valores
   - Número de autorização (se aplicável)
   - Data do atendimento
   - Profissional executante
3. **Finalizar** a guia para envio

### Editando uma guia:

Se precisa corrigir algo:

1. Acesse: **Faturamento → Guias TISS**
2. Localize a guia em "Rascunho"
3. Clique em **"Editar"**
4. Faça as correções necessárias
5. Clique em **"Salvar"**
6. Clique em **"Finalizar Guia"**

⚠️ **Atenção:** Guias já enviadas NÃO podem ser editadas. Se errar, precisa criar uma **guia de cancelamento** ou enviar uma **retificação**.

---

## Envio de Lotes

### Por que enviar em lotes?

Em vez de enviar cada guia individualmente, as operadoras preferem receber **lotes** agrupando múltiplas guias. Isso:

- Facilita o processamento
- Reduz custos administrativos
- Padroniza o faturamento

### Quando enviar lotes?

**Recomendação:** Envie lotes **mensalmente**, geralmente:
- No **5º dia útil** do mês seguinte ao atendimento
- Ou na data definida pela operadora

**Exemplo:** Atendimentos de Janeiro → Enviar lote até 5º dia útil de Fevereiro

### Quantas guias por lote?

Não há limite técnico, mas:
- **Mínimo:** 1 guia (pode enviar lote com 1 única guia se necessário)
- **Máximo:** Algumas operadoras limitam em 500-1000 guias por lote
- **Ideal:** Agrupe todos os atendimentos do mês

### Organizando seus lotes:

**Dica:** Crie lotes separados por:
- **Operadora** (cada operadora = um lote)
- **Período** (mensal)
- **Tipo de guia** (se a operadora exigir)

---

## Acompanhamento

### Painel de Controle TISS

**Menu:** Faturamento → Dashboard TISS

Visão geral de:
- 📊 **Total de guias por status** (Rascunho, Enviadas, Pagas)
- 💰 **Valores em aberto** por operadora
- ⏰ **Lotes aguardando envio**
- ⚠️ **Guias com glosa**
- 📈 **Faturamento TISS mensal**

### Relatórios disponíveis:

1. **Relatório de Guias por Período**
   - Todas as guias criadas em um período
   - Filtros: operadora, status, profissional

2. **Relatório de Glosas**
   - Guias que foram glosadas
   - Motivos mais frequentes
   - Valor total glosado

3. **Relatório de Faturamento**
   - Valor total faturado
   - Por operadora, por plano, por profissional
   - Taxa de glosa

4. **Relatório de Autorizações**
   - Autorizações solicitadas
   - Taxa de aprovação/negação
   - Tempo médio de resposta

### Notificações:

Configure **alertas automáticos** para:
- ✉️ Quando uma autorização for aprovada/negada
- ⏰ Quando uma autorização estiver próxima do vencimento
- 📄 Quando um lote for processado pela operadora
- ⚠️ Quando houver glosas

---

## Perguntas Frequentes

### 1. O que fazer se o paciente não trouxe a carteirinha?

**Opções:**
1. Ligar para operadora e **verificar elegibilidade** por CPF
2. Pedir que o paciente envie **foto da carteirinha** por WhatsApp
3. Atender como **particular** e depois:
   - Solicitar reembolso ao paciente na operadora (se o plano permitir)
   - Ou aguardar dados do convênio e faturar posteriormente

### 2. Autorizações são obrigatórias para tudo?

**Não.** Depende do plano e do procedimento.

- **Urgências/Emergências:** Podem ser feitas SEM autorização prévia (lei)
- **Consultas simples:** Geralmente não precisam
- **Exames/procedimentos complexos:** Geralmente precisam

Cadastre no sistema quais procedimentos requerem autorização e o sistema alertará automaticamente.

### 3. O que é "glosa" e por que acontece?

**Glosa** é quando a operadora **não paga** total ou parcialmente uma guia.

**Motivos comuns:**
- ❌ Procedimento sem autorização prévia
- ❌ Código TUSS incorreto ou incompatível
- ❌ Dados incompletos na guia
- ❌ Prazo de autorização expirado
- ❌ Procedimento não coberto pelo plano
- ❌ Erro de digitação em dados do beneficiário

**Como evitar:**
- ✅ Sempre solicitar autorização quando necessário
- ✅ Revisar todas as guias antes de enviar
- ✅ Usar códigos TUSS corretos
- ✅ Verificar validade da carteirinha do paciente
- ✅ Preencher todos os campos obrigatórios

### 4. Quanto tempo demora para receber?

**Prazo típico:**
- Envio do lote: **Dia 5 do mês seguinte**
- Processamento pela operadora: **5 a 15 dias úteis**
- Pagamento: **30 a 45 dias** após o envio

**Total:** Geralmente de **35 a 60 dias** após o atendimento.

**Exemplo:**
- Atendimento: 15 de Janeiro
- Envio do lote: 5 de Fevereiro
- Retorno da operadora: 20 de Fevereiro
- Pagamento: 15 de Março

### 5. Posso cancelar ou corrigir uma guia já enviada?

**Depende do status:**

- ✅ **Rascunho:** Pode editar ou excluir livremente
- ⚠️ **Enviado (aguardando processamento):** Entre em contato com a operadora
- ❌ **Processado/Pago:** Não pode cancelar. Se precisa corrigir:
  - Envie **guia de retificação** com os dados corretos
  - Ou envie **guia de cancelamento** + **nova guia correta**

### 6. Preciso ter certificado digital?

**Depende da integração:**

- ❌ **Manual/Portal Web:** Não precisa
- ✅ **XML TISS eletrônico:** Sim, algumas operadoras exigem
- ✅ **API REST:** Sim, geralmente requer

O certificado digital A1 ou A3 (e-CNPJ) é usado para **assinar digitalmente** os arquivos XML, garantindo autenticidade.

### 7. Todas as operadoras aceitam TISS?

**Sim**, o padrão TISS é **obrigatório por lei** (Resolução Normativa ANS nº 305/2012).

No entanto, o **nível de automação** varia:
- Algumas aceitam apenas papel
- Outras aceitam XML por e-mail
- Algumas têm portal web
- Poucas oferecem API em tempo real

### 8. O que é "Padrão TISS 4.03"?

É a **versão** do padrão TISS.

A ANS atualiza o padrão periodicamente. As versões mais recentes:
- **3.05.00** (2020-2021)
- **4.02.00** (2021-2023)
- **4.03.00** (2023-atual) ← versão mais recente

Ao configurar a operadora no sistema, informe qual versão ela aceita.

### 9. Posso usar o TISS para pacientes particulares?

**Não.** O TISS é exclusivo para **convênios e planos de saúde**.

Para pacientes particulares, você usa o **sistema financeiro normal** do PrimeCare (recibos, notas fiscais, etc.).

### 10. E se a operadora negar a autorização?

**Opções:**

1. **Recurso/Contestação:**
   - Apresente mais documentação médica
   - Justificativa clínica detalhada
   - Exames que comprovem necessidade

2. **Substitui por procedimento coberto:**
   - Converse com o médico
   - Encontre alternativa dentro da cobertura

3. **Paciente paga particular:**
   - Emite recibo particular
   - Paciente pode tentar reembolso (se plano permitir)

4. **Busca por segunda opinião:**
   - Encaminhe para outro especialista
   - Nova solicitação com mais informações

---

## 📞 Suporte

Dúvidas sobre o uso do sistema TISS no PrimeCare?

- 📧 **E-mail:** suporte@primecaresoftware.com
- 💬 **Chat:** Disponível no sistema (canto inferior direito)
- 📚 **Base de conhecimento:** [docs.primecaresoftware.com](https://docs.primecaresoftware.com)
- 🎥 **Vídeos tutoriais:** Canal do YouTube PrimeCare Software

---

## 📚 Documentos Relacionados

- [Guia do Usuário - TUSS](./GUIA_USUARIO_TUSS.md)
- [Guia de Integração com Operadoras](./HEALTH_INSURANCE_INTEGRATION_GUIDE.md)
- [Status de Implementação TISS](./TISS_PHASE1_IMPLEMENTATION_STATUS.md)

---

**Última atualização:** Janeiro 2026  
**Versão:** 1.0  
**Elaborado por:** PrimeCare Software
