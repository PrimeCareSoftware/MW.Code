# Guia do Usuário - Relatórios Financeiros

## Índice

1. [Introdução](#introdução)
2. [Acessando os Relatórios](#acessando-os-relatórios)
3. [DRE - Demonstrativo de Resultados](#dre---demonstrativo-de-resultados)
4. [Previsão de Fluxo de Caixa](#previsão-de-fluxo-de-caixa)
5. [Análise de Rentabilidade](#análise-de-rentabilidade)
6. [Dicas e Boas Práticas](#dicas-e-boas-práticas)
7. [Perguntas Frequentes](#perguntas-frequentes)

## Introdução

Os Relatórios Financeiros do Omni Care Software são ferramentas poderosas para análise estratégica da saúde financeira da sua clínica. Este guia irá ajudá-lo a entender e utilizar cada relatório de forma eficaz.

### Relatórios Disponíveis

1. **DRE (Demonstrativo de Resultados do Exercício)** - Visão contábil completa de receitas, custos e lucros
2. **Previsão de Fluxo de Caixa** - Projeção de entradas e saídas futuras
3. **Análise de Rentabilidade** - Identificação dos procedimentos, profissionais e convênios mais lucrativos

### Permissões Necessárias

Para acessar os relatórios financeiros, você precisa ter a permissão **ReportsFinancial**. Entre em contato com o administrador do sistema caso não tenha acesso.

## Acessando os Relatórios

### Navegação

1. Faça login no sistema Omni Care
2. No menu lateral, clique em **Financeiro**
3. Selecione **Relatórios**
4. Escolha o relatório desejado:
   - DRE
   - Previsão de Fluxo de Caixa
   - Análise de Rentabilidade

### URLs Diretas

- DRE: `/financial/reports/dre`
- Previsão: `/financial/reports/cash-flow-forecast`
- Rentabilidade: `/financial/reports/profitability`

## DRE - Demonstrativo de Resultados

### O que é o DRE?

O DRE (Demonstrativo de Resultados do Exercício) é um relatório contábil que mostra a performance financeira da clínica em um período específico, seguindo a estrutura:

```
Receita Bruta
(-) Deduções (estornos, cancelamentos)
= Receita Líquida
(-) Custos Operacionais
(-) Despesas Administrativas
(-) Despesas de Vendas
(-) Despesas Financeiras
= Lucro Operacional
= Lucro Líquido
= Margem de Lucro (%)
```

### Como Gerar o DRE

1. **Selecione a Clínica**
   - Escolha a clínica para análise no menu dropdown
   - Caso tenha múltiplas clínicas, analise cada uma individualmente

2. **Defina o Período**
   - **Data Início:** Primeiro dia do período de análise
   - **Data Fim:** Último dia do período de análise
   - Dica: Use períodos mensais para análise regular

3. **Clique em "Gerar Relatório"**
   - O sistema processará todos os pagamentos e despesas do período
   - O relatório será exibido em poucos segundos

### Interpretando o DRE

#### Seção de Receitas

- **Receita Bruta:** Total de todos os pagamentos recebidos
- **Deduções:** Valores estornados, cancelamentos, descontos
- **Receita Líquida:** Receita real após deduções (Bruta - Deduções)

#### Seção de Custos e Despesas

- **Custos Operacionais:** Materiais e suprimentos médicos
- **Despesas Administrativas:** Salários, aluguel, manutenção, utilities, serviços profissionais, software
- **Despesas de Vendas:** Marketing e publicidade
- **Despesas Financeiras:** Impostos e seguros

#### Seção de Resultados

- **Lucro Operacional:** Receita Líquida - Total de Despesas
- **Lucro Líquido:** Lucro final (no sistema simplificado = Lucro Operacional)
- **Margem de Lucro:** Percentual de lucro sobre a receita líquida

#### Detalhamentos

- **Receitas por Método:** Distribuição de receitas por forma de pagamento (Dinheiro, Cartão, PIX, etc.)
- **Despesas por Categoria:** Distribuição de despesas por tipo

### Indicadores de Alerta

🟢 **Saudável:** Margem de lucro acima de 20%  
🟡 **Atenção:** Margem de lucro entre 10-20%  
🔴 **Crítico:** Margem de lucro abaixo de 10%

### Ações Recomendadas

**Se a margem estiver baixa:**
- Revise os custos operacionais (materiais, fornecedores)
- Analise despesas administrativas desnecessárias
- Considere reajuste de preços dos procedimentos
- Avalie a rentabilidade individual de cada serviço

## Previsão de Fluxo de Caixa

### O que é a Previsão de Fluxo de Caixa?

A Previsão de Fluxo de Caixa projeta as entradas e saídas futuras de dinheiro com base em:
- Contas a receber pendentes (valores a entrar)
- Contas a pagar pendentes (valores a sair)
- Saldo atual da clínica

### Como Gerar a Previsão

1. **Selecione a Clínica**
   - Escolha a clínica para análise

2. **Defina o Período de Projeção**
   - Escolha entre 1 e 12 meses de projeção
   - Recomendado: 3 meses para gestão tática, 6-12 meses para planejamento estratégico

3. **Clique em "Gerar Previsão"**
   - O sistema analisará todas as contas pendentes
   - A projeção será calculada mês a mês

### Interpretando a Previsão

#### Cards de Resumo

- **Saldo Atual:** Valor disponível hoje (receitas pagas - despesas pagas)
- **Receitas Previstas:** Total de contas a receber pendentes no período
- **Despesas Previstas:** Total de contas a pagar pendentes no período
- **Saldo Projetado:** Saldo esperado ao final do período (Atual + Receitas - Despesas)

#### Tabela de Previsão Mensal

Para cada mês, você verá:
- **Receitas:** Valores a receber com vencimento no mês
- **Despesas:** Valores a pagar com vencimento no mês
- **Saldo do Mês:** Diferença entre receitas e despesas do mês
- **Saldo Acumulado:** Saldo projetado considerando meses anteriores

#### Listas Detalhadas

- **Contas a Receber Pendentes:** Lista completa de receitas esperadas com paciente, vencimento e valor
- **Contas a Pagar Pendentes:** Lista completa de despesas programadas com fornecedor, categoria e vencimento

### Indicadores de Alerta

🟢 **Saudável:** Saldo acumulado sempre positivo  
🟡 **Atenção:** Saldo acumulado próximo de zero em algum mês  
🔴 **Crítico:** Saldo acumulado negativo projetado

### Ações Recomendadas

**Se o fluxo estiver negativo em algum mês:**
- Negocie prazos com fornecedores
- Acelere cobranças de inadimplentes
- Considere antecipação de recebíveis (com cautela devido aos juros)
- Reavalie despesas não essenciais
- Planeje reserve de emergência

**Dica:** Mantenha uma reserva de caixa equivalente a 3-6 meses de despesas fixas.

## Análise de Rentabilidade

### O que é a Análise de Rentabilidade?

A Análise de Rentabilidade identifica quais segmentos da sua clínica são mais lucrativos, analisando:
- Tipos de procedimentos/consultas
- Profissionais
- Convênios e particular

### Como Gerar a Análise

1. **Selecione a Clínica**
   - Escolha a clínica para análise

2. **Defina o Período**
   - **Data Início:** Primeiro dia do período
   - **Data Fim:** Último dia do período
   - Recomendado: Períodos de 1-3 meses para análises regulares

3. **Clique em "Gerar Análise"**
   - O sistema processará todos os atendimentos e pagamentos
   - Os dados serão agrupados por procedimento, profissional e convênio

### Interpretando a Análise

#### Cards de Resumo

- **Receita Total:** Soma de todas as receitas no período
- **Custos Totais:** Soma de todas as despesas no período
- **Lucro Total:** Receita - Custos
- **Margem de Lucro:** Percentual de lucro sobre receita

#### Rentabilidade por Procedimento

Mostra quais tipos de atendimento são mais lucrativos:
- **Procedimento:** Tipo de consulta/procedimento
- **Quantidade:** Número de atendimentos realizados
- **Receita Total:** Faturamento total do procedimento
- **Valor Médio:** Receita média por atendimento
- **% do Total:** Percentual da receita total

**Como usar:**
- Identifique procedimentos com alto volume e boa margem
- Considere expandir os serviços mais rentáveis
- Avalie se procedimentos com baixa margem podem ser otimizados ou descontinuados

#### Rentabilidade por Profissional

Mostra o desempenho financeiro de cada profissional:
- **Profissional:** Nome do médico/profissional
- **Atendimentos:** Número total de consultas
- **Receita Total:** Faturamento gerado
- **Valor Médio:** Receita média por atendimento
- **% do Total:** Contribuição para receita total

**Como usar:**
- Reconheça profissionais de alto desempenho
- Identifique oportunidades de treinamento
- Ajuste escalas e horários com base na demanda

#### Rentabilidade por Convênio

Mostra a lucratividade de cada convênio médico e de pacientes particulares:
- **Convênio:** Nome do convênio ou "Particular"
- **Atendimentos:** Número de consultas
- **Receita Total:** Faturamento do convênio
- **Valor Médio:** Ticket médio
- **% do Total:** Participação na receita

**Como usar:**
- Avalie se vale a pena manter convênios de baixo retorno
- Considere negociar valores com operadoras
- Balance entre atendimentos particulares e convênios

### Exemplo de Análise Estratégica

**Cenário:**
- Consultas particulares: 30% volume, 40% receita → Alta rentabilidade
- Convênio A: 50% volume, 35% receita → Baixa rentabilidade
- Convênio B: 20% volume, 25% receita → Boa rentabilidade

**Ação:**
1. Aumentar divulgação para pacientes particulares
2. Negociar reajuste com Convênio A ou reduzir vagas
3. Manter parceria com Convênio B

## Dicas e Boas Práticas

### Frequência de Análise

- **DRE:** Mensal (fechamento mensal)
- **Previsão:** Semanal ou quinzenal para gestão tática
- **Rentabilidade:** Mensal ou trimestral

### Melhores Períodos para Análise

- Use meses completos (dia 1 ao último dia do mês)
- Compare meses equivalentes (Janeiro 2025 vs Janeiro 2026)
- Considere sazonalidade (férias, feriados, eventos locais)

### Exportação de Relatórios

Atualmente, os botões de exportação para PDF e Excel estão preparados para implementação futura. Por enquanto:
- Tire screenshots dos relatórios
- Use Ctrl+P (imprimir) e salve como PDF
- Copie as tabelas para planilhas manualmente

### Integração com Contabilidade

Os relatórios do sistema podem complementar sua contabilidade formal:
- Envie os relatórios mensalmente para seu contador
- Use o DRE como base para o DRE contábil oficial
- Mantenha documentação de todas as transações

## Perguntas Frequentes

### Por que os valores não batem com meu extrato bancário?

O sistema usa regime de competência (quando ocorre) e não regime de caixa (quando entra/sai dinheiro). O relatório de Previsão de Fluxo de Caixa é mais próximo do extrato bancário.

### Posso comparar períodos diferentes?

Atualmente não há comparação automática. Gere relatórios separados para cada período e compare manualmente. Essa funcionalidade será adicionada em versões futuras.

### Os relatórios consideram impostos?

Atualmente, o sistema não deduz impostos automaticamente. Para análise completa, consulte seu contador para cálculos tributários específicos.

### Como o sistema calcula o saldo atual na Previsão?

O saldo é aproximado com base na diferença entre:
- Contas a receber pagas (créditos realizados)
- Contas a pagar pagas (débitos realizados)

Para precisão máxima, mantenha todas as transações registradas no sistema.

### Posso gerar relatórios para múltiplas clínicas simultaneamente?

Não, atualmente cada relatório analisa uma clínica por vez. Gere relatórios separados e consolide manualmente se necessário.

### O que fazer se encontrar dados inconsistentes?

1. Verifique se todas as transações foram lançadas corretamente
2. Confirme que pagamentos e despesas estão com as datas corretas
3. Revise as categorias de despesas
4. Entre em contato com o suporte técnico se o problema persistir

## Suporte

Para mais informações ou suporte técnico:
- Consulte a documentação técnica: `docs/MODULO_FINANCEIRO.md`
- Entre em contato com o suporte: [informações de contato]
- Assista aos vídeos tutoriais: [link para vídeos]

---

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Última atualização:** 22 de Janeiro de 2026
