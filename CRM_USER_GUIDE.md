# 📖 Guia do Usuário - Sistema CRM Avançado

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Sistema:** MedicSoft - Omni Care

---

## 📋 Índice

1. [Introdução](#introdução)
2. [Jornada do Paciente](#jornada-do-paciente)
3. [Automação de Marketing](#automação-de-marketing)
4. [Pesquisas NPS/CSAT](#pesquisas-npscsat)
5. [Ouvidoria](#ouvidoria)
6. [Dashboard e Métricas](#dashboard-e-métricas)
7. [Melhores Práticas](#melhores-práticas)

---

## 🎯 Introdução

O Sistema CRM (Customer Relationship Management) da MedicSoft permite gerenciar todo o ciclo de vida do relacionamento com pacientes, desde a primeira interação até a fidelização.

### Principais Funcionalidades

- **Jornada do Paciente:** Acompanhe cada etapa da experiência do paciente
- **Automação de Marketing:** Crie campanhas automáticas baseadas em comportamento
- **Pesquisas NPS/CSAT:** Meça satisfação e identifique promotores
- **Ouvidoria:** Gerencie reclamações com protocolo e SLA
- **Análise de Sentimento:** IA identifica sentimentos em comentários
- **Predição de Churn:** Identifique pacientes em risco de abandono

---

## 🗺️ Jornada do Paciente

### O Que É?

A Jornada do Paciente mapeia todos os pontos de contato (touchpoints) e estágios pelos quais um paciente passa, desde a descoberta da clínica até se tornar um promotor da marca.

### Estágios da Jornada

#### 1. **Descoberta** 🔍
- **Descrição:** Paciente conhece a clínica/serviço
- **Exemplos de Touchpoints:**
  - Pesquisa no Google
  - Anúncio em redes sociais
  - Indicação de amigo
  - Visita ao site
- **Objetivo:** Capturar interesse inicial

#### 2. **Consideração** 🤔
- **Descrição:** Paciente avalia opções e compara
- **Exemplos de Touchpoints:**
  - Consulta de preços
  - Leitura de avaliações
  - Contato por WhatsApp
  - Download de materiais
- **Objetivo:** Demonstrar valor e diferenciais

#### 3. **Primeira Consulta** 👨‍⚕️
- **Descrição:** Primeiro atendimento presencial
- **Exemplos de Touchpoints:**
  - Agendamento
  - Confirmação
  - Check-in
  - Atendimento
- **Objetivo:** Excelente primeira impressão

#### 4. **Tratamento** 💊
- **Descrição:** Durante o processo terapêutico
- **Exemplos de Touchpoints:**
  - Exames
  - Procedimentos
  - Acompanhamento
  - Prescrições
- **Objetivo:** Garantir adesão e qualidade

#### 5. **Retorno** 🔄
- **Descrição:** Consultas de acompanhamento
- **Exemplos de Touchpoints:**
  - Agendamento de retorno
  - Follow-ups
  - Ajustes de tratamento
- **Objetivo:** Continuidade do cuidado

#### 6. **Fidelização** ⭐
- **Descrição:** Cliente recorrente e satisfeito
- **Exemplos de Touchpoints:**
  - Check-ups regulares
  - Programas de fidelidade
  - Benefícios exclusivos
- **Objetivo:** Manter relacionamento de longo prazo

#### 7. **Advocacia** 📣
- **Descrição:** Promotor ativo da marca
- **Exemplos de Touchpoints:**
  - Avaliações positivas
  - Indicações
  - Testemunhos
  - Compartilhamento em redes
- **Objetivo:** Amplificar reputação

### Como Visualizar a Jornada

1. Acesse **CRM > Jornada do Paciente**
2. Busque pelo nome ou ID do paciente
3. Visualize:
   - Estágio atual
   - Histórico de estágios
   - Todos os touchpoints
   - Métricas (LTV, NPS, Satisfação)
   - Risco de churn

### Adicionando Touchpoints Manualmente

1. Na tela da jornada, clique em **"Adicionar Touchpoint"**
2. Preencha:
   - **Tipo:** Email, Telefone, WhatsApp, SMS, Presencial
   - **Canal:** Nome do canal utilizado
   - **Descrição:** Detalhes da interação
   - **Direção:** Entrada (paciente contatou) ou Saída (clínica contatou)
3. Clique em **"Salvar"**

> **💡 Dica:** Touchpoints são registrados automaticamente pelo sistema em eventos como agendamentos, consultas e envio de mensagens.

---

## 🎯 Automação de Marketing

### O Que É?

Automações de marketing são fluxos que executam ações automaticamente quando certas condições são atendidas (triggers), sem necessidade de intervenção manual.

### Criando uma Automação

1. Acesse **CRM > Automação > Nova Automação**
2. Preencha:
   - **Nome:** Ex: "Boas-vindas Novo Paciente"
   - **Descrição:** Objetivo da automação
   - **Trigger:** Quando executar
   - **Segmentação:** Filtros (opcional)

#### Tipos de Trigger

- **Manual:** Executado sob demanda
- **Agendamento:** Data/hora específica
- **Evento:** Após uma ação (cadastro, consulta, etc.)
- **Comportamento:** Baseado em ação do paciente

### Adicionando Ações

1. Na automação criada, clique em **"Adicionar Ação"**
2. Escolha o tipo de ação:
   - **Enviar Email:** Template personalizado
   - **Enviar SMS:** Mensagem curta
   - **Enviar WhatsApp:** Mensagem via WhatsApp Business
   - **Adicionar Tag:** Marcar o paciente
   - **Remover Tag:** Remover marcação
   - **Alterar Score:** Ajustar pontuação de engajamento
   - **Avançar Estágio:** Mover na jornada
   - **Criar Tarefa:** Gerar task para equipe
   - **Webhook:** Integrar com sistema externo

3. Configure os detalhes da ação
4. Defina o **delay** (espera antes de executar)
5. Salve a ação

### Templates de Email

#### Criando Templates

1. Acesse **CRM > Templates > Novo Template**
2. Preencha:
   - **Nome:** Identificação do template
   - **Assunto:** Assunto do email
   - **Corpo:** Conteúdo HTML

#### Variáveis Dinâmicas

Use variáveis para personalizar mensagens:

- `{{nome}}` - Nome do paciente
- `{{primeiroNome}}` - Primeiro nome
- `{{email}}` - Email do paciente
- `{{telefone}}` - Telefone
- `{{proximaConsulta}}` - Data da próxima consulta
- `{{medico}}` - Nome do médico responsável
- `{{clinica}}` - Nome da clínica

**Exemplo:**
```
Olá {{primeiroNome}},

Sua consulta com Dr(a). {{medico}} está confirmada para {{proximaConsulta}}.

Atenciosamente,
Equipe {{clinica}}
```

### Ativando/Desativando Automações

- **Ativar:** Clique no botão "Ativar" para iniciar a execução
- **Desativar:** Clique em "Desativar" para pausar temporariamente

> ⚠️ **Atenção:** Automações desativadas não executam, mesmo que os triggers sejam acionados.

### Métricas de Automação

Acompanhe a performance:

- **Taxa de Sucesso:** % de execuções bem-sucedidas
- **Total Executado:** Número de vezes que foi executada
- **Taxa de Abertura:** % de emails abertos (requer integração)
- **Taxa de Clique:** % de cliques em links

---

## 📊 Pesquisas NPS/CSAT

### O Que São?

- **NPS (Net Promoter Score):** Mede probabilidade de indicação (0-10)
- **CSAT (Customer Satisfaction):** Mede satisfação com atendimento (1-5 estrelas)

### Criando Pesquisas

1. Acesse **CRM > Pesquisas > Nova Pesquisa**
2. Escolha o tipo:
   - **NPS:** Uma pergunta padrão ("De 0 a 10, qual a probabilidade de você recomendar...")
   - **CSAT:** Questões sobre satisfação específica
   - **Personalizada:** Crie suas próprias perguntas

3. Configure:
   - **Nome:** Identificação interna
   - **Título:** Visível ao paciente
   - **Descrição:** Contexto da pesquisa
   - **Trigger:** Quando enviar (após consulta, 24h depois, etc.)

### Adicionando Questões

1. Clique em **"Adicionar Questão"**
2. Preencha:
   - **Texto:** Pergunta
   - **Tipo:** Escala, Múltipla Escolha, Texto Livre
   - **Obrigatória:** Sim/Não
   - **Ordem:** Sequência de exibição

### Enviando Pesquisas

**Envio Automático:**
- Configure trigger (ex: 24h após consulta)
- Sistema envia automaticamente

**Envio Manual:**
1. Acesse a pesquisa
2. Clique em **"Enviar para Paciente"**
3. Selecione o paciente
4. Confirme o envio

### Interpretando NPS

**Classificação de Respondentes:**
- **Promotores (9-10):** Defensores da marca
- **Neutros (7-8):** Satisfeitos, mas não entusiastas
- **Detratores (0-6):** Insatisfeitos, risco de churn

**Cálculo do NPS:**
```
NPS = % Promotores - % Detratores
```

**Faixas de Avaliação:**
- **NPS 75-100:** Excelente
- **NPS 50-74:** Muito Bom
- **NPS 0-49:** Razoável
- **NPS < 0:** Zona de Perigo

### Analytics de Pesquisas

Visualize:
- **NPS Score:** Média geral
- **Distribuição:** % de cada categoria
- **Evolução:** Tendência ao longo do tempo
- **Por Médico:** Performance individual
- **Por Especialidade:** Comparação entre áreas
- **Comentários:** Feedback qualitativo

---

## 🎧 Ouvidoria

### O Que É?

Sistema de gerenciamento de reclamações, sugestões e elogios com rastreamento por protocolo e controle de SLA.

### Registrando Reclamação

1. Acesse **CRM > Ouvidoria > Nova Reclamação**
2. Preencha:
   - **Paciente:** Selecione ou pesquise
   - **Categoria:** Tipo de reclamação
   - **Prioridade:** Baixa, Média, Alta, Crítica
   - **Título:** Resumo do problema
   - **Descrição:** Detalhes completos

3. Clique em **"Registrar"**
4. Sistema gera **protocolo** automaticamente (formato: CMP-YYYY-NNNNNN)

### Categorias de Reclamação

- **Atendimento:** Qualidade do atendimento
- **Agendamento:** Problemas com marcação
- **Infraestrutura:** Instalações físicas
- **Procedimento:** Questões clínicas
- **Financeiro:** Cobranças e pagamentos
- **Outro:** Outras categorias

### Fluxo de Atendimento

#### 1. **Recebida**
- Reclamação registrada
- Aguardando atribuição
- **SLA:** Atribuir em até 2h

#### 2. **Em Andamento**
- Atribuída a responsável
- Em análise/investigação
- **SLA:** Primeira resposta em até 24h

#### 3. **Resolvida**
- Problema solucionado
- Aguardando confirmação do paciente
- **SLA:** Resolução em até 72h

#### 4. **Fechada**
- Confirmada resolução
- Caso encerrado
- Métricas registradas

### Adicionando Interações

1. Abra a reclamação
2. Clique em **"Nova Interação"**
3. Escreva a resposta/atualização
4. Marque se é:
   - **Interna:** Visível apenas para equipe
   - **Externa:** Visível para paciente
5. Salve

### Mudando Status

1. Na reclamação, clique em **"Alterar Status"**
2. Escolha o novo status
3. Adicione observação (obrigatório)
4. Confirme

### Atribuindo Responsável

1. Clique em **"Atribuir"**
2. Selecione o profissional/departamento
3. Adicione nota (opcional)
4. Confirme

> 💡 **Dica:** Atribuições geram notificação automática para o responsável.

### Dashboard de Ouvidoria

Visualize métricas consolidadas:

- **Total de Reclamações:** Geral e por período
- **Por Status:** Distribuição dos casos
- **Por Categoria:** Principais motivos
- **Por Prioridade:** Urgência dos casos
- **SLA Cumprimento:** % de casos dentro do prazo
- **Tempo Médio de Resposta:** Performance
- **Tempo Médio de Resolução:** Eficiência
- **Taxa de Resolução:** % de casos resolvidos

---

## 📈 Dashboard e Métricas

### KPIs Principais

#### Satisfação

- **NPS Score:** Meta > 50
- **CSAT Score:** Meta > 4.5/5
- **Taxa de Resposta:** % de pacientes que respondem pesquisas
- **Evolução:** Tendência mensal

#### Retenção

- **Taxa de Churn:** % de pacientes que abandonaram
- **Taxa de Retenção:** % de pacientes ativos
- **LTV (Lifetime Value):** Valor médio por paciente
- **Retorno de Investimento:** ROI de campanhas

#### Engajamento

- **Touchpoints/Paciente:** Média de interações
- **Taxa de Abertura:** Emails abertos
- **Taxa de Clique:** Cliques em emails
- **Engajamento Score:** Pontuação geral

#### Ouvidoria

- **Reclamações Totais:** Volume absoluto
- **Reclamações Resolvidas:** % de resolução
- **SLA Cumprido:** % dentro do prazo
- **Tempo de Resolução:** Média em horas

### Filtros e Períodos

- **Período:** Hoje, Semana, Mês, Trimestre, Ano, Customizado
- **Médico:** Filtrar por profissional
- **Especialidade:** Filtrar por área médica
- **Estágio da Jornada:** Filtrar por etapa

### Exportando Relatórios

1. No dashboard, clique em **"Exportar"**
2. Escolha o formato: PDF, Excel, CSV
3. Selecione dados a incluir
4. Clique em **"Gerar Relatório"**

---

## 💡 Melhores Práticas

### Jornada do Paciente

✅ **DO (Faça):**
- Registre todos os touchpoints importantes
- Atualize métricas regularmente
- Revise jornadas de pacientes VIP semanalmente
- Use insights para melhorar processos

❌ **DON'T (Não Faça):**
- Ignorar touchpoints negativos
- Deixar jornadas estagnadas sem avanço
- Esquecer de registrar interações importantes

### Automação de Marketing

✅ **DO (Faça):**
- Teste automações antes de ativar
- Use segmentação para relevância
- Personalize com variáveis dinâmicas
- Monitore métricas e ajuste

❌ **DON'T (Não Faça):**
- Enviar muitas mensagens (spam)
- Usar linguagem genérica
- Ignorar horários adequados
- Ativar sem testar

### Pesquisas NPS/CSAT

✅ **DO (Faça):**
- Envie no momento certo (24-48h após atendimento)
- Mantenha pesquisas curtas (máx 5 questões)
- Agradeça pela participação
- Aja sobre feedback negativo rapidamente

❌ **DON'T (Não Faça):**
- Enviar pesquisas repetidamente
- Ignorar comentários negativos
- Fazer perguntas muito técnicas
- Deixar de responder aos detratores

### Ouvidoria

✅ **DO (Faça):**
- Responda rápido (< 24h)
- Seja empático e profissional
- Registre todas as interações
- Cumpra prazos prometidos
- Feche o loop com o paciente

❌ **DON'T (Não Faça):**
- Demorar para responder
- Ser defensivo ou ignorar
- Prometer o que não pode cumprir
- Fechar casos sem resolver

### Análise de Dados

✅ **DO (Faça):**
- Revise dashboards semanalmente
- Identifique tendências e padrões
- Compartilhe insights com equipe
- Use dados para decisões estratégicas

❌ **DON'T (Não Faça):**
- Tomar decisões sem dados
- Ignorar sinais de alerta
- Focar apenas em métricas positivas
- Não agir sobre insights

---

## 🆘 Suporte

### Recursos Adicionais

- **Documentação Técnica:** `/CRM_IMPLEMENTATION_STATUS.md`
- **API Documentation:** `/CRM_API_DOCUMENTATION.md`
- **Configuração:** `/CRM_CONFIGURATION_GUIDE.md`

### Contato

Para suporte técnico ou dúvidas:
- **Email:** suporte@omnicare.com.br
- **Telefone:** (11) 9999-9999
- **Portal:** https://suporte.omnicare.com.br

---

**Última Atualização:** Janeiro 2026  
**Versão do Documento:** 1.0  
**Sistema:** MedicSoft CRM Advanced
