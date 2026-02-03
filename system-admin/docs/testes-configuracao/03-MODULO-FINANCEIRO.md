# 💰 Módulo Financeiro - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar o módulo Financeiro do Omni Care Software, incluindo contas a receber, contas a pagar, fluxo de caixa, e relatórios financeiros.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend)
- Usuário com perfil Owner ou Secretary logado
- Consultas realizadas para gerar recebíveis
- Fornecedores cadastrados

## 📖 Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Cenários de Teste - Contas a Receber](#cenários-de-teste---contas-a-receber)
3. [Cenários de Teste - Contas a Pagar](#cenários-de-teste---contas-a-pagar)
4. [Cenários de Teste - Fluxo de Caixa](#cenários-de-teste---fluxo-de-caixa)
5. [Cenários de Teste - Fornecedores](#cenários-de-teste---fornecedores)
6. [Cenários de Teste - Fechamento Financeiro](#cenários-de-teste---fechamento-financeiro)
7. [Cenários de Teste - Relatórios](#cenários-de-teste---relatórios)
8. [API Testing](#api-testing)
9. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuração Inicial

### 1. Configurar Formas de Pagamento

**Passos:**
1. Acesse **"Configurações"** → **"Financeiro"** → **"Formas de Pagamento"**
2. Cadastre as formas:
   - **Dinheiro** (taxa 0%)
   - **Cartão de Débito** (taxa 2%)
   - **Cartão de Crédito** (taxa 3.5%)
   - **PIX** (taxa 0%)
   - **Transferência Bancária** (taxa 0%)
   - **Boleto** (taxa R$ 2,50 fixo)
   - **Convênio** (taxa variável)

3. Configure dias para recebimento de cada forma
4. Salve

**Resultado Esperado:**
- ✅ Formas de pagamento disponíveis
- ✅ Taxas calculadas automaticamente
- ✅ Prazo de recebimento definido

---

### 2. Configurar Categorias de Despesas

**Passos:**
1. Acesse **"Configurações"** → **"Financeiro"** → **"Categorias"**
2. Cadastre categorias:
   - **Salários e Encargos**
   - **Aluguel**
   - **Energia Elétrica**
   - **Água**
   - **Internet e Telefone**
   - **Material de Expediente**
   - **Material Médico-Hospitalar**
   - **Manutenção e Limpeza**
   - **Marketing e Propaganda**
   - **Impostos e Taxas**
   - **Outros**

3. Defina cores para cada categoria
4. Salve

**Resultado Esperado:**
- ✅ Categorias criadas
- ✅ Disponíveis para classificação
- ✅ Usadas em relatórios

---

### 3. Configurar Contas Bancárias

**Passos:**
1. Acesse **"Configurações"** → **"Financeiro"** → **"Contas Bancárias"**
2. Cadastre contas:
   - **Banco do Brasil** - CC 12345-6
   - **Caixa Econômica** - Poupança 98765-4
   - **Nubank** - CC 11111-1

3. Defina saldo inicial de cada conta
4. Marque conta principal
5. Salve

**Resultado Esperado:**
- ✅ Contas cadastradas
- ✅ Saldo inicial registrado
- ✅ Disponível para movimentações

---

### 4. Verificar Permissões

**Perfis com acesso ao Financeiro:**
- ✅ Owner (acesso total)
- ✅ Secretary (lançamentos e consultas)
- ⚠️ Medic (apenas visualização de seus recebíveis)
- ❌ Nurse (sem acesso)
- ❌ SystemAdmin (sem acesso)

---

## 🧪 Cenários de Teste - Contas a Receber

### Cenário 1.1: Lançamento Manual de Recebível

**Objetivo:** Criar conta a receber manualmente

**Passos:**
1. Acesse **"Financeiro"** → **"Contas a Receber"**
2. Clique em **"+ Nova Conta"**
3. Preencha:
   - **Descrição:** Consulta - Maria Silva
   - **Paciente:** Maria Silva Santos
   - **Valor:** R$ 200,00
   - **Data de Vencimento:** 30/01/2026
   - **Forma de Pagamento:** Dinheiro
   - **Categoria:** Consultas
   - **Observações:** Pagamento à vista

4. Clique em **"Salvar"**

**Resultado Esperado:**
- ✅ Conta criada com status "Pendente"
- ✅ Aparece na listagem
- ✅ Vencimento configurado
- ✅ Valor correto

---

### Cenário 1.2: Recebível Gerado Automaticamente

**Objetivo:** Validar geração automática após consulta

**Pré-requisito:** Consulta finalizada

**Passos:**
1. Finalize uma consulta com valor R$ 200,00
2. Acesse **"Contas a Receber"**
3. Localize o recebível gerado automaticamente

**Resultado Esperado:**
- ✅ Conta criada automaticamente
- ✅ Vinculada à consulta
- ✅ Valor e paciente corretos
- ✅ Status "Pendente"

---

### Cenário 1.3: Registrar Pagamento

**Objetivo:** Baixar conta a receber

**Passos:**
1. Na listagem, clique em **"Receber"** na conta
2. Confirme:
   - **Data de Pagamento:** 25/01/2026 (hoje)
   - **Valor Recebido:** R$ 200,00
   - **Forma:** Dinheiro
   - **Conta Bancária:** Caixa (se não for dinheiro)

3. Clique em **"Confirmar Pagamento"**

**Resultado Esperado:**
- ✅ Status alterado para "Pago"
- ✅ Data de pagamento registrada
- ✅ Entrada no fluxo de caixa
- ✅ Saldo atualizado

---

### Cenário 1.4: Pagamento Parcial

**Objetivo:** Receber valor parcial da conta

**Passos:**
1. Conta de R$ 200,00 pendente
2. Clique em **"Receber Parcial"**
3. Informe valor: R$ 100,00
4. Confirme

**Resultado Esperado:**
- ✅ Saldo devedor atualizado: R$ 100,00
- ✅ Status "Parcialmente Pago"
- ✅ Histórico de pagamentos registrado

---

### Cenário 1.5: Parcelamento de Recebível

**Objetivo:** Criar conta parcelada

**Passos:**
1. Crie nova conta a receber
2. Valor: R$ 600,00
3. Marque **"Parcelar"**
4. Configure:
   - **Número de Parcelas:** 3x
   - **Primeira Parcela:** 30/01/2026
   - **Intervalo:** 30 dias
   - **Taxa de Juros:** 2% (opcional)

5. Salve

**Resultado Esperado:**
- ✅ 3 parcelas criadas:
  - Parcela 1: R$ 200,00 - 30/01/2026
  - Parcela 2: R$ 204,00 - 01/03/2026 (com juros)
  - Parcela 3: R$ 208,08 - 31/03/2026 (com juros)
- ✅ Cada parcela pode ser paga independentemente

---

### Cenário 1.6: Aplicar Desconto

**Objetivo:** Conceder desconto em pagamento antecipado

**Passos:**
1. Conta de R$ 200,00 vencendo em 10 dias
2. Paciente quer pagar hoje
3. Clique em **"Receber com Desconto"**
4. Aplique desconto:
   - **Tipo:** Percentual
   - **Valor:** 10%
   - **Motivo:** Pagamento antecipado

5. Valor final: R$ 180,00
6. Confirme pagamento

**Resultado Esperado:**
- ✅ Desconto aplicado
- ✅ Valor recebido: R$ 180,00
- ✅ Motivo registrado no histórico

---

### Cenário 1.7: Juros e Multa por Atraso

**Objetivo:** Calcular juros em pagamento atrasado

**Passos:**
1. Conta de R$ 200,00 vencida há 10 dias
2. Configure em **"Configurações"**:
   - **Multa:** 2% (fixa)
   - **Juros:** 1% ao mês (proporcional)

3. Clique em **"Receber com Juros"**
4. Sistema calcula:
   - Valor original: R$ 200,00
   - Multa (2%): R$ 4,00
   - Juros 10 dias (0.33%): R$ 0,66
   - **Total:** R$ 204,66

5. Confirme pagamento

**Resultado Esperado:**
- ✅ Juros e multa calculados automaticamente
- ✅ Valor total correto
- ✅ Detalhamento no histórico

---

### Cenário 1.8: Estorno de Pagamento

**Objetivo:** Reverter pagamento já registrado

**Passos:**
1. Localize conta já paga
2. Clique em **"Menu"** → **"Estornar Pagamento"**
3. Informe motivo: "Erro de lançamento"
4. Confirme

**Resultado Esperado:**
- ✅ Status volta para "Pendente"
- ✅ Lançamento negativo no fluxo de caixa
- ✅ Saldo da conta revertido
- ✅ Histórico mantém registro do estorno

---

## 🧪 Cenários de Teste - Contas a Pagar

### Cenário 2.1: Lançar Conta a Pagar

**Objetivo:** Registrar despesa da clínica

**Passos:**
1. Acesse **"Financeiro"** → **"Contas a Pagar"**
2. Clique em **"+ Nova Conta"**
3. Preencha:
   - **Descrição:** Aluguel Janeiro/2026
   - **Fornecedor:** Imobiliária ABC
   - **Valor:** R$ 3.500,00
   - **Data de Vencimento:** 10/02/2026
   - **Categoria:** Aluguel
   - **Forma de Pagamento:** Transferência Bancária
   - **Conta Bancária:** Banco do Brasil
   - **Observações:** Referente ao mês de janeiro

4. Anexe boleto/documento (opcional)
5. Salve

**Resultado Esperado:**
- ✅ Conta criada com status "Pendente"
- ✅ Aparece na listagem
- ✅ Alerta próximo ao vencimento

---

### Cenário 2.2: Pagar Conta

**Objetivo:** Efetuar pagamento de despesa

**Passos:**
1. Na listagem, clique em **"Pagar"**
2. Confirme:
   - **Data de Pagamento:** 08/02/2026
   - **Valor Pago:** R$ 3.500,00
   - **Forma:** Transferência
   - **Conta:** Banco do Brasil

3. Confirme pagamento

**Resultado Esperado:**
- ✅ Status "Pago"
- ✅ Saída registrada no fluxo de caixa
- ✅ Saldo da conta atualizado

---

### Cenário 2.3: Pagar com Desconto

**Objetivo:** Negociar desconto com fornecedor

**Passos:**
1. Conta de R$ 1.000,00
2. Fornecedor oferece 5% de desconto para pagamento antecipado
3. Clique em **"Pagar com Desconto"**
4. Aplique desconto: 5%
5. Valor final: R$ 950,00
6. Confirme

**Resultado Esperado:**
- ✅ Desconto aplicado
- ✅ Economia registrada
- ✅ Valor pago correto

---

### Cenário 2.4: Conta Parcelada (Fornecedor)

**Objetivo:** Compra parcelada de equipamento

**Passos:**
1. Crie conta a pagar
2. Descrição: Equipamento Médico
3. Valor total: R$ 12.000,00
4. Marque **"Parcelar"**
5. Configure:
   - **Parcelas:** 6x
   - **Primeira Parcela:** 15/02/2026
   - **Intervalo:** 30 dias

6. Salve

**Resultado Esperado:**
- ✅ 6 parcelas de R$ 2.000,00 criadas
- ✅ Vencimentos mensais
- ✅ Cada parcela independente

---

### Cenário 2.5: Conta Recorrente

**Objetivo:** Despesa que se repete mensalmente

**Passos:**
1. Crie conta a pagar
2. Descrição: Plano de Internet
3. Valor: R$ 299,00
4. Marque **"Conta Recorrente"**
5. Configure:
   - **Frequência:** Mensal
   - **Dia do Vencimento:** 5
   - **Repetir por:** 12 meses

6. Salve

**Resultado Esperado:**
- ✅ 12 contas criadas (uma por mês)
- ✅ Todas vinculadas como recorrentes
- ✅ Fácil identificação na listagem

---

## 🧪 Cenários de Teste - Fluxo de Caixa

### Cenário 3.1: Visualizar Fluxo Diário

**Objetivo:** Ver movimentações do dia

**Passos:**
1. Acesse **"Financeiro"** → **"Fluxo de Caixa"**
2. Selecione **"Hoje"**
3. Visualize entradas e saídas

**Resultado Esperado:**
- ✅ Lista de movimentações do dia
- ✅ Total de entradas
- ✅ Total de saídas
- ✅ Saldo do dia

---

### Cenário 3.2: Lançamento Manual no Caixa

**Objetivo:** Registrar movimentação avulsa

**Passos:**
1. Clique em **"+ Novo Lançamento"**
2. Preencha:
   - **Tipo:** Entrada
   - **Descrição:** Venda de material
   - **Valor:** R$ 150,00
   - **Categoria:** Outras Receitas
   - **Data:** 22/01/2026
   - **Forma:** Dinheiro

3. Salve

**Resultado Esperado:**
- ✅ Lançamento registrado
- ✅ Saldo atualizado
- ✅ Categoria correta

---

### Cenário 3.3: Transferência entre Contas

**Objetivo:** Mover dinheiro entre contas bancárias

**Passos:**
1. Clique em **"Transferência"**
2. Configure:
   - **De:** Caixa (Dinheiro)
   - **Para:** Banco do Brasil
   - **Valor:** R$ 5.000,00
   - **Data:** 22/01/2026
   - **Observações:** Depósito em conta

3. Confirme

**Resultado Esperado:**
- ✅ Saída registrada no Caixa
- ✅ Entrada registrada no Banco
- ✅ Saldos atualizados
- ✅ Transferência vinculada (não conta como receita/despesa)

---

### Cenário 3.4: Relatório de Fluxo Mensal

**Objetivo:** Análise do mês

**Passos:**
1. Selecione **"Visualização Mensal"**
2. Escolha mês: Janeiro/2026
3. Visualize gráfico e tabela

**Resultado Esperado:**
- ✅ Gráfico de entradas vs saídas por dia
- ✅ Total de entradas do mês
- ✅ Total de saídas do mês
- ✅ Resultado (lucro/prejuízo)
- ✅ Comparativo com mês anterior

---

### Cenário 3.5: Projeção de Fluxo de Caixa

**Objetivo:** Ver previsão futura

**Passos:**
1. Acesse **"Projeção de Caixa"**
2. Selecione período: Próximos 30 dias
3. Sistema mostra:
   - Saldo atual
   - Entradas previstas (contas a receber)
   - Saídas previstas (contas a pagar)
   - Saldo projetado

**Resultado Esperado:**
- ✅ Projeção por dia
- ✅ Identificação de dias críticos (saldo negativo)
- ✅ Gráfico de tendência

---

## 🧪 Cenários de Teste - Fornecedores

### Cenário 4.1: Cadastrar Fornecedor

**Objetivo:** Adicionar novo fornecedor

**Passos:**
1. Acesse **"Financeiro"** → **"Fornecedores"**
2. Clique em **"+ Novo Fornecedor"**
3. Preencha:
   - **Nome/Razão Social:** Distribuidora Médica XYZ Ltda
   - **Nome Fantasia:** Med XYZ
   - **CNPJ:** 12.345.678/0001-99
   - **Email:** contato@medxyz.com.br
   - **Telefone:** (11) 3456-7890
   - **Endereço:** Rua das Flores, 123
   - **Categoria:** Material Médico-Hospitalar
   
4. Dados Bancários:
   - **Banco:** Itaú
   - **Agência:** 1234
   - **Conta:** 56789-0
   - **PIX:** 12.345.678/0001-99

5. Salve

**Resultado Esperado:**
- ✅ Fornecedor cadastrado
- ✅ Disponível para vinculação em contas
- ✅ Dados bancários salvos

---

### Cenário 4.2: Histórico com Fornecedor

**Objetivo:** Visualizar todas as transações

**Passos:**
1. Na listagem, clique no fornecedor
2. Acesse aba **"Histórico"**

**Resultado Esperado:**
- ✅ Lista de todas as compras
- ✅ Valores pagos e pendentes
- ✅ Total gasto com fornecedor
- ✅ Última transação

---

## 🧪 Cenários de Teste - Fechamento Financeiro

### Cenário 5.1: Fechamento de Consulta

**Objetivo:** Gerar cobrança após atendimento

**Passos:**
1. Finalize uma consulta
2. Sistema abre **"Fechamento Financeiro"**
3. Itens:
   - Consulta: R$ 200,00
   - Procedimento adicional: R$ 50,00
   - **Subtotal:** R$ 250,00
4. Aplicar desconto: 10% = R$ 25,00
5. **Total:** R$ 225,00
6. Forma de pagamento: Cartão de Crédito
7. Confirme

**Resultado Esperado:**
- ✅ Fechamento registrado
- ✅ Conta a receber gerada
- ✅ Recibo emitido
- ✅ Vinculado à consulta

---

### Cenário 5.2: Fechamento com Convênio

**Objetivo:** Faturamento para convênio

**Passos:**
1. Consulta com convênio
2. Fechamento financeiro
3. Selecione **"Convênio: Unimed"**
4. Sistema busca valores da tabela do convênio
5. Gera guia TISS automaticamente
6. Status: "Aguardando Aprovação"
7. Confirme

**Resultado Esperado:**
- ✅ Guia TISS gerada
- ✅ Enviada ao convênio (se integrado)
- ✅ Conta a receber com status especial
- ✅ Prazo de recebimento do convênio aplicado

---

### Cenário 5.3: Divisão de Pagamento

**Objetivo:** Parte particular, parte convênio

**Passos:**
1. Consulta: R$ 500,00
2. Convênio cobre: R$ 300,00
3. Paciente paga: R$ 200,00
4. No fechamento:
   - **Convênio:** R$ 300,00 (guia TISS)
   - **Particular:** R$ 200,00 (pago agora)
5. Confirme

**Resultado Esperado:**
- ✅ 2 contas a receber:
  - 1 para convênio (pendente)
  - 1 para particular (pago)
- ✅ Total correto: R$ 500,00

---

## 🧪 Cenários de Teste - Relatórios

### Cenário 6.1: DRE (Demonstração de Resultado)

**Objetivo:** Ver lucros e despesas do período

**Passos:**
1. Acesse **"Relatórios"** → **"DRE"**
2. Selecione período: Janeiro/2026
3. Visualize:
   - **Receitas:**
     - Consultas: R$ 50.000,00
     - Procedimentos: R$ 15.000,00
     - **Total:** R$ 65.000,00
   
   - **Despesas:**
     - Salários: R$ 20.000,00
     - Aluguel: R$ 3.500,00
     - Materiais: R$ 8.000,00
     - Outros: R$ 5.000,00
     - **Total:** R$ 36.500,00
   
   - **Resultado:** R$ 28.500,00 (lucro)

4. Exporte para PDF/Excel

**Resultado Esperado:**
- ✅ Relatório completo e preciso
- ✅ Gráficos visuais
- ✅ Comparativo com período anterior
- ✅ Exportação funcionando

---

### Cenário 6.2: Relatório de Inadimplência

**Objetivo:** Identificar contas vencidas

**Passos:**
1. Acesse **"Relatórios"** → **"Inadimplência"**
2. Visualize:
   - Contas vencidas
   - Valor total em atraso
   - Dias de atraso
   - Paciente/telefone
3. Ordene por: Valor (maior para menor)
4. Exporte lista para contato

**Resultado Esperado:**
- ✅ Lista de inadimplentes
- ✅ Dados de contato
- ✅ Total a recuperar
- ✅ Ação de cobrança disponível

---

### Cenário 6.3: Relatório por Forma de Pagamento

**Objetivo:** Analisar preferências de pagamento

**Passos:**
1. Acesse **"Relatórios"** → **"Formas de Pagamento"**
2. Período: Último mês
3. Visualize distribuição:
   - Dinheiro: 15%
   - Débito: 20%
   - Crédito: 40%
   - PIX: 20%
   - Convênio: 5%

**Resultado Esperado:**
- ✅ Gráfico de pizza
- ✅ Valores e percentuais
- ✅ Taxas de cada forma
- ✅ Sugestões de otimização

---

## 🔌 API Testing

### Endpoint: Criar Conta a Receber

```bash
curl -X POST "http://localhost:5000/api/accounts-receivable" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "description": "Consulta - Maria Silva",
    "patientId": "patient-uuid",
    "amount": 200.00,
    "dueDate": "2026-01-30",
    "paymentMethod": "Cash",
    "category": "Consultas"
  }'
```

---

### Endpoint: Registrar Pagamento

```bash
curl -X POST "http://localhost:5000/api/accounts-receivable/{id}/pay" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "paymentDate": "2026-01-25",
    "amountPaid": 200.00,
    "paymentMethod": "Cash"
  }'
```

---

### Endpoint: Fluxo de Caixa do Dia

```bash
curl -X GET "http://localhost:5000/api/cash-flow?date=2026-01-22" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

## 🐛 Troubleshooting

### Problema 1: Saldo incorreto

**Solução:**
1. Acesse **"Configurações"** → **"Recalcular Saldos"**
2. Sistema reprocessa todas as transações

### Problema 2: Conta não baixa após pagamento

**Solução:**
1. Verifique se pagamento foi confirmado
2. Verifique permissões do usuário
3. Tente novamente

---

## ✅ Checklist de Validação Final

- [ ] Criar conta a receber manual
- [ ] Conta gerada automaticamente
- [ ] Registrar pagamento
- [ ] Pagamento parcial
- [ ] Parcelamento de recebível
- [ ] Aplicar desconto
- [ ] Juros e multa por atraso
- [ ] Estorno de pagamento
- [ ] Lançar conta a pagar
- [ ] Pagar conta
- [ ] Pagar com desconto
- [ ] Conta parcelada
- [ ] Conta recorrente
- [ ] Fluxo de caixa diário
- [ ] Lançamento manual
- [ ] Transferência entre contas
- [ ] Relatório mensal
- [ ] Projeção de caixa
- [ ] Cadastrar fornecedor
- [ ] Histórico com fornecedor
- [ ] Fechamento de consulta
- [ ] Fechamento com convênio
- [ ] Divisão de pagamento
- [ ] Relatório DRE
- [ ] Relatório de inadimplência
- [ ] Relatório por forma de pagamento

---

## 📚 Documentação Relacionada

- [Módulo Financeiro Completo](../MODULO_FINANCEIRO.md)
- [Nota Fiscal Eletrônica](../NFE_NFSE_USER_GUIDE.md)
- [TISS e TUSS](04-TISS-PADRAO.md)
