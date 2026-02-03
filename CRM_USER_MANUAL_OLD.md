# 📖 Manual do Usuário - CRM Avançado Omni Care

## Bem-vindo ao CRM Avançado

O CRM Avançado do Omni Care é uma solução completa para gerenciamento do relacionamento com pacientes, permitindo:
- Acompanhar a jornada completa do paciente
- Automatizar comunicações personalizadas
- Coletar feedback através de pesquisas NPS/CSAT
- Gerenciar reclamações via ouvidoria
- Prever riscos de perda de pacientes (churn)
- Analisar sentimentos em feedbacks

---

## 📊 Patient Journey - Jornada do Paciente

### O que é?
O sistema mapeia automaticamente a jornada de cada paciente através de 7 estágios:

1. **🔍 Descoberta** - Paciente conhece sua clínica (marketing, indicação)
2. **🤔 Consideração** - Paciente avalia opções e compara
3. **👨‍⚕️ Primeira Consulta** - Primeiro atendimento
4. **💊 Tratamento** - Durante o tratamento
5. **🔄 Retorno** - Consultas de acompanhamento
6. **❤️ Fidelização** - Paciente recorrente, satisfeito
7. **🏆 Advocacia** - Paciente promotor, recomenda a clínica

### Como visualizar a jornada?

1. Acesse o menu **CRM** > **Jornada do Paciente**
2. Busque ou selecione um paciente
3. Visualize a linha do tempo com:
   - Estágios percorridos
   - Tempo em cada estágio
   - Pontos de contato (touchpoints)
   - Métricas importantes

### Métricas da Jornada

**Total de Touchpoints**: Número total de interações registradas

**Lifetime Value (LTV)**: Valor total gerado pelo paciente

**NPS Score**: Net Promoter Score (0-10)

**Risco de Churn**: 
- 🟢 Low (Baixo)
- 🟡 Medium (Médio)
- 🟠 High (Alto)
- 🔴 Critical (Crítico)

### Registrar Touchpoints Manualmente

Quando necessário, você pode registrar interações manualmente:

1. Na tela da jornada, clique em **+ Novo Touchpoint**
2. Selecione o tipo:
   - Ligação telefônica
   - Email
   - SMS
   - WhatsApp
   - Visita ao site
   - Interação presencial
3. Adicione descrição
4. Indique a direção (Iniciado por você ou pelo paciente)
5. Salve

---

## 🤖 Automação de Marketing

### O que é?
Sistema que envia automaticamente emails, SMS ou mensagens WhatsApp baseado em gatilhos específicos.

### Criar uma Automação

1. Acesse **CRM** > **Automações de Marketing**
2. Clique em **+ Nova Automação**
3. Preencha:
   - **Nome**: Ex: "Boas-vindas Novo Paciente"
   - **Descrição**: Objetivo da automação
   - **Gatilho**: O que dispara a automação

### Tipos de Gatilhos

**Mudança de Estágio**
- Quando paciente entra em um estágio específico
- Ex: Ao entrar em "Primeira Consulta"

**Evento Específico**
- Consulta agendada
- Consulta realizada
- Paciente não compareceu (no-show)
- Aniversário

**Agendado**
- Data/hora específica
- Ex: Toda segunda-feira às 9h

**Comportamental**
- Baseado em ações do paciente
- Ex: Não retorna há 30 dias

### Adicionar Ações

Após configurar o gatilho, adicione ações:

1. Clique em **+ Adicionar Ação**
2. Escolha o tipo:
   - **Enviar Email**: Use um template
   - **Enviar SMS**: Texto curto
   - **Enviar WhatsApp**: Mensagem personalizada
   - **Adicionar Tag**: Para segmentação
   - **Criar Tarefa**: Para equipe

3. Configure detalhes da ação
4. Defina ordem (se múltiplas ações)

### Templates de Email

Para criar templates:

1. Acesse **CRM** > **Templates de Email**
2. Clique em **+ Novo Template**
3. Preencha:
   - Nome do template
   - Assunto
   - Corpo HTML
   - Corpo texto simples

**Variáveis Disponíveis**:
- `{{nome_paciente}}` - Nome do paciente
- `{{data_consulta}}` - Data da próxima consulta
- `{{nome_clinica}}` - Nome da clínica
- `{{telefone_clinica}}` - Telefone para contato

### Ativar/Desativar Automações

- Use o botão toggle para ativar/desativar
- Automações desativadas não serão executadas
- Histórico de execuções é mantido

### Métricas de Automação

- **Vezes Executada**: Quantas vezes foi disparada
- **Taxa de Sucesso**: Percentual de envios bem-sucedidos
- **Última Execução**: Data/hora da última execução

---

## 📋 Pesquisas NPS/CSAT

### O que são?

**NPS (Net Promoter Score)**
- Mede lealdade do paciente
- Escala de 0 a 10
- Pergunta: "Quanto você recomendaria nossa clínica?"

**CSAT (Customer Satisfaction)**
- Mede satisfação geral
- Escala de 1 a 5 estrelas
- Pergunta: "Quão satisfeito você está?"

### Criar uma Pesquisa

1. Acesse **CRM** > **Pesquisas**
2. Clique em **+ Nova Pesquisa**
3. Escolha o tipo (NPS, CSAT ou Customizada)
4. Configure:
   - Nome da pesquisa
   - Descrição
   - Quando enviar (gatilho)
   - Delay (atraso após evento)

### Adicionar Questões

1. Clique em **+ Nova Questão**
2. Tipos disponíveis:
   - **Escala Numérica**: 0-10
   - **Avaliação Estrelas**: 1-5
   - **Múltipla Escolha**: Opções predefinidas
   - **Texto Livre**: Resposta aberta
   - **Sim/Não**: Binária

3. Marque se é obrigatória
4. Defina ordem de exibição

### Configurar Envio Automático

1. Selecione o **Gatilho**:
   - Após estágio específico
   - Após evento (consulta realizada)
   
2. Defina **Delay**:
   - Ex: 24 horas após consulta
   - Ex: 1 hora após agendamento

3. Ative a pesquisa

### Visualizar Resultados

1. Acesse a pesquisa
2. Veja métricas:
   - **Score Médio**: Média das respostas
   - **Total de Respostas**: Quantidade recebida
   - **Taxa de Resposta**: Percentual de quem respondeu

3. Gráficos disponíveis:
   - Distribuição de respostas
   - Evolução temporal
   - Análise de texto livre

### Interpretação NPS

- **Promotores (9-10)**: Clientes leais, recomendarão
- **Neutros (7-8)**: Satisfeitos, mas não entusiasmados
- **Detratores (0-6)**: Insatisfeitos, risco de churn

**Cálculo NPS**: % Promotores - % Detratores

- NPS > 50: Excelente
- NPS 30-50: Bom
- NPS 0-30: Razoável
- NPS < 0: Crítico

---

## 🎯 Ouvidoria - Gestão de Reclamações

### O que é?
Sistema para registrar, acompanhar e resolver reclamações e feedbacks de pacientes.

### Registrar uma Reclamação

1. Acesse **CRM** > **Ouvidoria**
2. Clique em **+ Nova Reclamação**
3. Preencha:
   - **Paciente**: Busque o paciente
   - **Assunto**: Título resumido
   - **Descrição**: Detalhes da reclamação
   - **Categoria**: Tipo de reclamação
   - **Prioridade**: Urgência

### Categorias de Reclamação

- **Atendimento**: Qualidade do atendimento
- **Agendamento**: Problemas com marcação
- **Faturamento**: Questões financeiras
- **Instalações**: Estrutura física
- **Profissional**: Relacionado ao médico/equipe
- **Tempo de Espera**: Demora no atendimento
- **Tratamento Médico**: Aspectos clínicos
- **Outro**: Outras categorias

### Status da Reclamação

- **Recebida**: Aguardando triagem
- **Em Análise**: Sendo investigada
- **Em Andamento**: Sendo resolvida
- **Aguardando Resposta**: Esperando paciente
- **Resolvida**: Problema solucionado
- **Fechada**: Concluída
- **Cancelada**: Desconsiderada

### Atribuir Reclamação

1. Abra a reclamação
2. Clique em **Atribuir a**
3. Selecione o responsável
4. A pessoa será notificada

### Adicionar Interações

Para registrar ações tomadas:

1. Na reclamação, clique em **+ Nova Interação**
2. Digite a mensagem
3. Marque se é **Interna** (não visível ao paciente)
4. Salve

### SLA (Service Level Agreement)

O sistema rastreia automaticamente:
- **Tempo de Primeira Resposta**: Quanto tempo até primeira ação
- **Tempo de Resolução**: Quanto tempo até resolver

### Encerrar Reclamação

1. Resolva o problema
2. Atualize status para **Resolvida**
3. Sistema enviará pesquisa de satisfação ao paciente
4. Após feedback, mude para **Fechada**

### Portal do Paciente

Pacientes podem:
- Abrir reclamações online
- Acompanhar pelo número de protocolo
- Receber atualizações por email
- Avaliar a resolução

---

## 📊 Dashboard de Analytics

### Métricas Principais

**Visão Geral**
- Total de pacientes ativos
- Novos pacientes (mês)
- Taxa de retenção
- Taxa de churn
- NPS médio

**Jornada**
- Distribuição por estágio
- Tempo médio por estágio
- Touchpoints por estágio
- Taxa de progressão

**Satisfação**
- NPS Score
- CSAT Score
- Taxa de resposta
- Evolução temporal

**Ouvidoria**
- Reclamações abertas
- Tempo médio de resposta
- Tempo médio de resolução
- Taxa de resolução
- Satisfação com resolução

**Risco de Churn**
- Pacientes por nível de risco
- Principais fatores de risco
- Ações recomendadas

### Filtros Disponíveis

- Período: Última semana, mês, trimestre, ano
- Clínica/Unidade
- Profissional
- Categoria
- Status

### Exportar Relatórios

1. Configure filtros desejados
2. Clique em **Exportar**
3. Escolha formato (PDF, Excel, CSV)

---

## 🤖 Análise de Sentimento com IA

### O que é?
Sistema que usa Inteligência Artificial para analisar automaticamente o sentimento em textos de:
- Comentários de pesquisas
- Reclamações
- Emails
- Mensagens

### Como Funciona?

O sistema analisa automaticamente e classifica como:
- **😊 Positivo**: Cliente satisfeito
- **😐 Neutro**: Sem emoção forte
- **😞 Negativo**: Cliente insatisfeito
- **🤔 Misto**: Sentimentos mistos

### Scores de Sentimento

- **Score Positivo**: 0-100% de positividade
- **Score Negativo**: 0-100% de negatividade
- **Confiança**: Quão certo o sistema está

### Tópicos Extraídos

O sistema identifica automaticamente:
- Temas principais mencionados
- Aspectos específicos (atendimento, limpeza, etc)
- Palavras-chave relevantes

### Alertas Automáticos

Quando detecta sentimento muito negativo:
- Notifica gestor responsável
- Destaca para ação imediata
- Sugere priorização

---

## 🎯 Predição de Churn

### O que é?
Sistema de Machine Learning que prediz quais pacientes têm risco de abandonar a clínica.

### Níveis de Risco

- **🟢 Baixo**: Paciente engajado, baixa probabilidade de churn
- **🟡 Médio**: Atenção necessária
- **🟠 Alto**: Risco significativo, ação urgente
- **🔴 Crítico**: Altíssimo risco, intervenção imediata

### Fatores de Risco

O sistema analisa:
- Tempo desde última visita
- Frequência de consultas
- Histórico de cancelamentos
- No-shows
- Scores de satisfação
- Reclamações registradas
- Valor gasto (LTV)
- Engajamento com comunicações

### Ações Recomendadas

Para cada paciente em risco, o sistema sugere:
- Entrar em contato via telefone/WhatsApp
- Oferecer desconto na próxima consulta
- Agendar consulta de retorno
- Resolver reclamação pendente
- Enviar pesquisa de satisfação

### Como Usar?

1. Acesse **CRM** > **Risco de Churn**
2. Veja lista de pacientes em risco
3. Filtre por nível de risco
4. Clique em paciente para ver detalhes
5. Execute ações recomendadas
6. Registre ações tomadas

### Monitoramento

- Sistema atualiza predições semanalmente
- Notificações automáticas para novos riscos
- Histórico de predições mantido
- Efetividade das ações rastreada

---

## 💡 Melhores Práticas

### Jornada do Paciente
- ✅ Registre todos os touchpoints importantes
- ✅ Mantenha dados atualizados
- ✅ Revise jornadas de pacientes-chave regularmente
- ✅ Use insights para melhorar processos

### Automações
- ✅ Teste antes de ativar
- ✅ Personalize mensagens
- ✅ Não exagere na frequência
- ✅ Monitore taxas de abertura/resposta
- ✅ Ajuste com base em resultados

### Pesquisas
- ✅ Mantenha pesquisas curtas (2-5 questões)
- ✅ Envie no momento certo (não imediato)
- ✅ Agradeça por respostas
- ✅ Aja baseado em feedback recebido
- ✅ Feche o loop com pacientes

### Ouvidoria
- ✅ Responda rapidamente (< 24h)
- ✅ Seja empático e profissional
- ✅ Mantenha paciente informado
- ✅ Resolva definitivamente
- ✅ Aprenda com reclamações

### Gestão de Churn
- ✅ Aja preventivamente
- ✅ Priorize riscos altos/críticos
- ✅ Personalize abordagem
- ✅ Documente ações tomadas
- ✅ Acompanhe resultados

---

## ❓ Perguntas Frequentes (FAQ)

**Q: Como sei qual estágio atribuir a um paciente?**
A: O sistema avança automaticamente baseado em eventos. Você pode ajustar manualmente se necessário.

**Q: Posso editar uma automação ativa?**
A: Sim, mas desative primeiro, faça alterações e reative.

**Q: Pesquisas são anônimas?**
A: Não, são associadas ao paciente para rastreamento, mas podem ser configuradas como anônimas.

**Q: Como o NPS é calculado?**
A: NPS = % Promotores (9-10) - % Detratores (0-6)

**Q: Reclamações são visíveis para o paciente?**
A: Pacientes veem suas próprias reclamações e respostas não-internas.

**Q: A análise de sentimento funciona em português?**
A: Sim, usando Azure Cognitive Services com suporte a português.

**Q: Com que frequência o churn é recalculado?**
A: Semanalmente, mas você pode solicitar recálculo manual.

**Q: Posso exportar dados?**
A: Sim, todos os relatórios podem ser exportados em PDF, Excel ou CSV.

---

## 📞 Suporte

Para dúvidas ou problemas:
- **Email**: suporte@omnicare.com.br
- **Telefone**: (11) 1234-5678
- **Chat**: Disponível no sistema
- **Documentação**: docs.omnicare.com.br

---

## 🔄 Atualizações

Este manual é atualizado regularmente. Versão: 1.0 (Janeiro 2026)

Para ver novidades e mudanças recentes, consulte o [CHANGELOG](./CHANGELOG.md).
