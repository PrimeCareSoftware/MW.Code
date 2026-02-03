# Guia do Usuário - Sistema de Gestão de Leads

## Introdução

O Sistema de Gestão de Leads é uma ferramenta completa para capturar e gerenciar potenciais clientes que iniciam o cadastro no site OmniCare mas não finalizam. Este sistema ajuda sua equipe a converter mais leads em clientes pagantes.

## Acesso ao Sistema

1. Faça login no System Admin
2. No menu lateral, clique em **"Gestão de Leads"**
3. Você verá o dashboard principal com suas estatísticas

## Dashboard Principal

### Estatísticas (KPIs)

O dashboard exibe 8 indicadores principais:

- **Total de Leads**: Quantidade total de leads capturados
- **Novos**: Leads que ainda não foram contactados
- **Qualificados**: Leads validados como potenciais clientes
- **Convertidos**: Leads que se tornaram clientes pagantes
- **Taxa de Conversão**: Percentual de leads que viraram clientes
- **Score Médio**: Pontuação média de qualidade dos leads
- **Precisam Follow-up**: Leads com follow-up agendado para hoje ou atrasado
- **Não Atribuídos**: Leads sem responsável atribuído

## Trabalhando com Leads

### Buscar e Filtrar Leads

**Busca por Texto**:
- Digite nome, email, telefone ou empresa na barra de busca
- A tabela será filtrada automaticamente

**Filtros**:
- **Status**: Filtre por Novo, Contactado, Qualificado, Convertido, Perdido ou Nutrição
- **Atribuição**: Veja todos, apenas atribuídos ou apenas não atribuídos

### Entender a Tabela de Leads

Cada lead mostra:
- **Score**: Pontuação de 0-100 indicando qualidade dos dados
  - 🟢 Verde (80-100): Lead quente, dados completos
  - 🔵 Azul (60-79): Lead morno, bons dados
  - 🟡 Amarelo (40-59): Lead frio, dados limitados
  - 🔴 Vermelho (0-39): Lead muito frio, poucos dados

- **Informações de Contato**: Nome, empresa, email, telefone
- **Plano**: Qual plano o lead demonstrou interesse
- **Status**: Estado atual do lead (com cores específicas)
- **Capturado**: Data que o lead abandonou o cadastro
- **Follow-up**: Próxima data de contato agendada

### Visualizar Detalhes do Lead

Clique em qualquer linha da tabela para ver:

1. **Informações Completas**:
   - Todos os dados de contato
   - Localização (cidade/estado)
   - Plano de interesse
   - Fonte do lead (referrer, UTM parameters)

2. **Timeline de Atividades**:
   - Histórico completo de interações
   - Quem realizou cada atividade
   - Data e resultado de cada contato

3. **Notas**: Observações adicionadas pela equipe

## Ações com Leads

### 1. Atribuir Lead

Para assumir responsabilidade por um lead:

1. Clique no ícone de pessoa (+) na tabela OU
2. No painel de detalhes, clique em "Atribuir Lead"
3. Digite o ID do usuário responsável
4. Confirme

**Quando usar**: Assim que decidir fazer follow-up de um lead

### 2. Agendar Follow-up

Para programar o próximo contato:

1. Clique no ícone de calendário na tabela OU
2. No painel de detalhes, clique em "Agendar Follow-up"
3. Escolha data e hora
4. Confirme

**Quando usar**: Após cada contato, programe o próximo

### 3. Registrar Atividade

Para documentar uma interação:

1. Clique no ícone de (+) na tabela OU
2. No painel de detalhes, clique em "Adicionar Atividade"
3. Preencha:
   - **Tipo**: Ligação, Email, Reunião, Nota ou Outro
   - **Título**: Descrição curta (ex: "Ligação de follow-up")
   - **Descrição**: Detalhes da conversa
   - **Duração**: Tempo gasto (minutos)
   - **Resultado**: O que foi decidido/acordado
4. Salve

**Quando usar**: Sempre após contactar o lead

### 4. Adicionar Notas

Para registrar observações gerais:

1. No painel de detalhes, clique em "Adicionar Notas"
2. Digite suas observações
3. Salve

**Quando usar**: Para informações que não são uma atividade específica

### 5. Atualizar Status

Para mover o lead pelo funil de vendas:

Status disponíveis:
- **Novo**: Lead acabou de ser capturado (automático)
- **Contactado**: Já fizemos o primeiro contato
- **Qualificado**: Lead validou interesse real
- **Convertido**: Virou cliente pagante! 🎉
- **Perdido**: Não tem interesse ou não é viável
- **Nutrição**: Tem potencial mas não é o momento certo

Para atualizar:
1. Clique no badge de status atual
2. Selecione novo status
3. Adicione notas explicando a mudança (opcional)
4. Confirme

## Fluxo de Trabalho Recomendado

### Para Leads Novos

1. **Revisão Inicial** (mesmo dia da captura):
   - Verifique o score e dados disponíveis
   - Atribua para si mesmo
   - Marque status como "Contactado" quando fizer primeiro contato

2. **Primeiro Contato** (até 24h):
   - Ligue ou envie email
   - Registre a atividade
   - Agende follow-up
   - Atualize status conforme resposta

3. **Follow-ups**:
   - Siga os agendamentos
   - Registre cada interação
   - Reagende próximo contato
   - Atualize status conforme progresso

### Para Leads Qualificados

1. Apresente proposta comercial
2. Agende demonstração do sistema
3. Responda dúvidas
4. Negocie condições
5. Finalize contrato
6. Marque como "Convertido" ao fechar venda

### Para Leads em Nutrição

1. Mantenha contato periódico
2. Envie conteúdo relevante
3. Monitore mudanças na situação
4. Reavalie periodicamente

## Dicas e Boas Práticas

### Maximizar Conversão

1. **Seja Rápido**: Contate leads novos em até 24h
2. **Seja Persistente**: Faça até 5-7 tentativas de contato
3. **Seja Organizado**: Use follow-ups para não esquecer ninguém
4. **Seja Detalhista**: Registre todas as interações
5. **Priorize**: Foque primeiro em leads com score alto

### Manter Dados Limpos

1. Sempre registre atividades após contatos
2. Mantenha follow-ups atualizados
3. Atualize status conforme progresso real
4. Marque como "Perdido" leads sem viabilidade
5. Use notas para contexto futuro

### Trabalho em Equipe

1. Atribua leads claramente
2. Não trabalhe leads de outros sem coordenar
3. Compartilhe insights nas notas
4. Atualize status para visibilidade da equipe

## Entendendo o Scoring

O sistema atribui automaticamente uma pontuação baseada na qualidade dos dados:

**Fatores que aumentam o score**:
- ✅ Tem email (+20 pontos)
- ✅ Tem telefone (+15 pontos)
- ✅ Tem nome da empresa (+10 pontos)
- ✅ Selecionou um plano (+10 pontos)
- ✅ Avançou mais etapas no cadastro (até +12 pontos)
- ✅ Tem informações de localização (+10 pontos)
- ✅ Veio de campanha rastreada (+5 pontos)

**Como usar o score**:
- Priorize leads com score 80+ (dados completos)
- Leads 60-79 são bons prospects
- Leads abaixo de 40 podem ter dados incompletos

## Perguntas Frequentes

**P: Como os leads são capturados?**
R: Automaticamente, quando alguém inicia o cadastro no site mas não completa. O sistema verifica a cada hora e captura abandonos após 24h.

**P: Posso editar as informações do lead?**
R: Não diretamente nesta versão. Use notas para registrar informações atualizadas.

**P: O que fazer com leads antigos?**
R: Reavalie periodicamente. Marque como "Perdido" se sem viabilidade ou "Nutrição" para contato futuro.

**P: Como vejo meu desempenho?**
R: Use "Estatísticas por Usuário" para ver seus leads, conversões e taxa de sucesso.

**P: Posso exportar os leads?**
R: Não nesta versão. Funcionalidade de exportação está planejada para futuro.

## Suporte

Para dúvidas ou problemas com o sistema, entre em contato com a equipe de TI.

---

**Versão**: 1.0  
**Última Atualização**: Fevereiro 2026
