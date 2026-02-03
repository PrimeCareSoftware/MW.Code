# 07 - Configuração do Módulo Financeiro

> **Objetivo:** Configurar completamente o módulo financeiro da clínica  
> **Tempo estimado:** 30-40 minutos  
> **Pré-requisitos:** 
> - Clínica criada e configurada ([ver Configuração da Clínica](06-Configuracao-Clinica.md))
> - Módulo Financeiro habilitado
> - Usuário com perfil Owner ou Admin

## 📋 Índice

1. [Visão Geral do Módulo Financeiro](#1-visão-geral-do-módulo-financeiro)
2. [Configuração de Formas de Pagamento](#2-configuração-de-formas-de-pagamento)
3. [Configuração de Categorias de Despesas](#3-configuração-de-categorias-de-despesas)
4. [Configuração de Contas Bancárias](#4-configuração-de-contas-bancárias)
5. [Configuração de Fornecedores](#5-configuração-de-fornecedores)
6. [Configuração de Permissões](#6-configuração-de-permissões)
7. [Configuração de Regras de Negócio](#7-configuração-de-regras-de-negócio)
8. [Verificação Final](#8-verificação-final)

---

## 1. Visão Geral do Módulo Financeiro

### 1.1. Funcionalidades Disponíveis

O módulo financeiro do Omni Care oferece:

**Contas a Receber:**
- ✅ Controle de recebimentos de consultas
- ✅ Gestão de inadimplência
- ✅ Parcelamento de valores
- ✅ Cálculo automático de juros e multas
- ✅ Descontos para pagamento antecipado
- ✅ Histórico completo de pagamentos

**Contas a Pagar:**
- ✅ Gestão de despesas e fornecedores
- ✅ Categorização de gastos
- ✅ Controle de vencimentos
- ✅ Suporte a parcelamento
- ✅ Notificações de vencimento

**Fluxo de Caixa:**
- ✅ Registro de entradas e saídas
- ✅ Balanço em tempo real
- ✅ Relatórios por período
- ✅ Categorização detalhada

**Fechamento Financeiro:**
- ✅ Fechamento de consultas
- ✅ Divisão particular/convênio
- ✅ Aplicação de descontos
- ✅ Geração automática de recebíveis

### 1.2. Fluxo de Configuração

```
1. Formas de Pagamento
   ↓
2. Categorias de Despesas
   ↓
3. Contas Bancárias
   ↓
4. Fornecedores
   ↓
5. Permissões de Usuários
   ↓
6. Regras de Negócio
   ↓
7. Pronto para usar!
```

---

## 2. Configuração de Formas de Pagamento

### 2.1. Acessar Configuração

**Passos:**
1. Menu lateral **"Financeiro"**
2. Submenu **"Configurações"**
3. Selecione **"Formas de Pagamento"**

### 2.2. Criar Formas de Pagamento Padrão

#### **Forma 1: Dinheiro**

```
✅ Nome: "Dinheiro"
✅ Descrição: "Pagamento em espécie"
✅ Tipo: Dinheiro
✅ Taxa (%): 0.00
✅ Dias para Recebimento: 0
✅ Permite Parcelamento: NÃO
✅ Status: Ativo
```

**Passos:**
1. Clicar em **"+ Nova Forma de Pagamento"**
2. Preencher os campos acima
3. Clicar em **"Salvar"**

#### **Forma 2: Cartão de Débito**

```
✅ Nome: "Cartão de Débito"
✅ Descrição: "Débito à vista com taxa de operadora"
✅ Tipo: Cartão Débito
✅ Taxa (%): 2.00
✅ Dias para Recebimento: 1
✅ Permite Parcelamento: NÃO
✅ Status: Ativo
```

#### **Forma 3: Cartão de Crédito**

```
✅ Nome: "Cartão de Crédito"
✅ Descrição: "Crédito com taxa de operadora"
✅ Tipo: Cartão Crédito
✅ Taxa (%): 3.50
✅ Taxa Parcelamento (%): 4.00 (opcional)
✅ Dias para Recebimento: 30
✅ Permite Parcelamento: SIM
✅ Parcelas Máximas: 12
✅ Status: Ativo
```

#### **Forma 4: PIX**

```
✅ Nome: "PIX"
✅ Descrição: "Transferência instantânea PIX"
✅ Tipo: PIX
✅ Taxa (%): 0.00
✅ Dias para Recebimento: 0
✅ Permite Parcelamento: NÃO
✅ Chave PIX: [sua chave PIX]
✅ Status: Ativo
```

#### **Forma 5: Transferência Bancária**

```
✅ Nome: "Transferência Bancária"
✅ Descrição: "TED/DOC"
✅ Tipo: Transferência
✅ Taxa (%): 0.00
✅ Dias para Recebimento: 1
✅ Permite Parcelamento: NÃO
✅ Status: Ativo
```

#### **Forma 6: Boleto Bancário**

```
✅ Nome: "Boleto Bancário"
✅ Descrição: "Boleto com taxa fixa"
✅ Tipo: Boleto
✅ Taxa Fixa (R$): 2.50
✅ Dias para Recebimento: 3
✅ Dias para Vencimento: 3
✅ Permite Parcelamento: NÃO
✅ Status: Ativo
```

#### **Forma 7: Convênio Médico**

```
✅ Nome: "Convênio"
✅ Descrição: "Pagamento via convênio médico"
✅ Tipo: Convênio
✅ Taxa (%): Variável por convênio
✅ Dias para Recebimento: 30-60 (conforme convênio)
✅ Permite Parcelamento: NÃO
✅ Requer Aprovação Prévia: SIM
✅ Status: Ativo
```

### 2.3. Configurações Avançadas

**Juros e Multas para Atraso:**
```
✅ Aplicar Juros Automáticos: SIM
✅ Taxa de Juros Mensal (%): 1.00
✅ Aplicar Multa Automática: SIM
✅ Multa por Atraso (%): 2.00
```

**Descontos:**
```
✅ Permitir Desconto à Vista: SIM
✅ Desconto Máximo (%): 10.00
✅ Desconto Requer Autorização: SIM (acima de 5%)
```

### 2.4. Verificar Formas de Pagamento

**Resultado Esperado:**
```
Lista de Formas de Pagamento:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ Dinheiro (0% taxa, 0 dias)
✅ Cartão de Débito (2% taxa, 1 dia)
✅ Cartão de Crédito (3.5% taxa, 30 dias, até 12x)
✅ PIX (0% taxa, 0 dias)
✅ Transferência Bancária (0% taxa, 1 dia)
✅ Boleto (R$ 2,50 taxa, 3 dias)
✅ Convênio (taxa variável, 30-60 dias)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## 3. Configuração de Categorias de Despesas

### 3.1. Acessar Configuração

**Passos:**
1. Menu **"Financeiro"** → **"Configurações"** → **"Categorias de Despesas"**

### 3.2. Criar Categorias Padrão

#### **Categoria 1: Salários e Encargos**

```
✅ Nome: "Salários e Encargos"
✅ Descrição: "Folha de pagamento e encargos trabalhistas"
✅ Código: "1.01"
✅ Tipo: Despesa Fixa
✅ Cor: #FF6B6B (Vermelho)
✅ Ícone: 💰
✅ Conta Contábil: 3.1.01.001 (opcional)
✅ Status: Ativo
```

#### **Categoria 2: Aluguel**

```
✅ Nome: "Aluguel"
✅ Descrição: "Aluguel do imóvel"
✅ Código: "1.02"
✅ Tipo: Despesa Fixa
✅ Cor: #FFA94D (Laranja)
✅ Ícone: 🏢
✅ Status: Ativo
```

#### **Categoria 3: Utilidades (Água, Luz, Internet)**

```
✅ Nome: "Utilidades"
✅ Descrição: "Água, Energia, Internet, Telefone"
✅ Código: "1.03"
✅ Tipo: Despesa Fixa
✅ Cor: #FFD93D (Amarelo)
✅ Ícone: ⚡
✅ Status: Ativo
```

#### **Categoria 4: Material de Expediente**

```
✅ Nome: "Material de Expediente"
✅ Descrição: "Materiais de escritório e administrativos"
✅ Código: "2.01"
✅ Tipo: Despesa Variável
✅ Cor: #6BCB77 (Verde)
✅ Ícone: 📄
✅ Status: Ativo
```

#### **Categoria 5: Material Médico-Hospitalar**

```
✅ Nome: "Material Médico-Hospitalar"
✅ Descrição: "Insumos, medicamentos, equipamentos médicos"
✅ Código: "2.02"
✅ Tipo: Despesa Variável
✅ Cor: #4D96FF (Azul)
✅ Ícone: 💉
✅ Status: Ativo
```

#### **Categoria 6: Manutenção e Limpeza**

```
✅ Nome: "Manutenção e Limpeza"
✅ Descrição: "Serviços de limpeza e manutenção"
✅ Código: "2.03"
✅ Tipo: Despesa Variável
✅ Cor: #C77DFF (Roxo)
✅ Ícone: 🧹
✅ Status: Ativo
```

#### **Categoria 7: Marketing e Propaganda**

```
✅ Nome: "Marketing e Propaganda"
✅ Descrição: "Publicidade, divulgação, mídias sociais"
✅ Código: "2.04"
✅ Tipo: Despesa Variável
✅ Cor: #F72585 (Rosa)
✅ Ícone: 📣
✅ Status: Ativo
```

#### **Categoria 8: Impostos e Taxas**

```
✅ Nome: "Impostos e Taxas"
✅ Descrição: "Tributos, taxas, contribuições"
✅ Código: "3.01"
✅ Tipo: Despesa Tributária
✅ Cor: #94D2BD (Verde Água)
✅ Ícone: 📊
✅ Status: Ativo
```

#### **Categoria 9: Serviços de Terceiros**

```
✅ Nome: "Serviços de Terceiros"
✅ Descrição: "Contabilidade, jurídico, consultoria"
✅ Código: "2.05"
✅ Tipo: Despesa Variável
✅ Cor: #EE9B00 (Dourado)
✅ Ícone: 👔
✅ Status: Ativo
```

#### **Categoria 10: Outras Despesas**

```
✅ Nome: "Outras Despesas"
✅ Descrição: "Despesas diversas não categorizadas"
✅ Código: "9.99"
✅ Tipo: Despesa Variável
✅ Cor: #ADB5BD (Cinza)
✅ Ícone: 📦
✅ Status: Ativo
```

### 3.3. Organização Hierárquica (Opcional)

Para clínicas maiores, organize em subcategorias:

```
1. PESSOAL
   ├── 1.01 Salários
   ├── 1.02 Encargos Sociais
   ├── 1.03 Benefícios
   └── 1.04 Treinamentos

2. INFRAESTRUTURA
   ├── 2.01 Aluguel
   ├── 2.02 Condomínio
   ├── 2.03 IPTU
   └── 2.04 Seguro

3. OPERACIONAL
   ├── 3.01 Material Médico
   ├── 3.02 Material de Limpeza
   ├── 3.03 Material de Expediente
   └── 3.04 Medicamentos
```

### 3.4. Verificar Categorias

**Resultado Esperado:**
- ✅ 10+ categorias criadas
- ✅ Cores distintas para cada categoria
- ✅ Códigos únicos atribuídos
- ✅ Todas ativas e disponíveis

---

## 4. Configuração de Contas Bancárias

### 4.1. Acessar Configuração

**Passos:**
1. Menu **"Financeiro"** → **"Configurações"** → **"Contas Bancárias"**

### 4.2. Adicionar Conta Corrente Principal

```
✅ Banco: Banco do Brasil
✅ Código do Banco: 001
✅ Agência: 1234-5
✅ Conta Corrente: 12345-6
✅ Tipo: Conta Corrente
✅ Titular: Clínica Saúde Total Ltda
✅ CNPJ: 12.345.678/0001-90
✅ Saldo Inicial: R$ 10.000,00
✅ Data do Saldo Inicial: 01/02/2026
✅ É Conta Principal: SIM
✅ Status: Ativa
```

**Informações PIX:**
```
✅ Chave PIX Tipo: CNPJ
✅ Chave PIX: 12.345.678/0001-90
✅ Nome para PIX: Clinica Saude Total
```

### 4.3. Adicionar Conta Poupança (Reserva)

```
✅ Banco: Caixa Econômica Federal
✅ Código do Banco: 104
✅ Agência: 9876
✅ Conta Poupança: 98765-4
✅ Tipo: Poupança
✅ Titular: Clínica Saúde Total Ltda
✅ Saldo Inicial: R$ 50.000,00
✅ Data do Saldo Inicial: 01/02/2026
✅ É Conta Principal: NÃO
✅ Finalidade: Reserva de Emergência
✅ Status: Ativa
```

### 4.4. Adicionar Conta Digital (Operacional)

```
✅ Banco: Nubank
✅ Código do Banco: 260
✅ Conta: 11111-1
✅ Tipo: Conta Digital
✅ Titular: Clínica Saúde Total Ltda
✅ Saldo Inicial: R$ 5.000,00
✅ Data do Saldo Inicial: 01/02/2026
✅ É Conta Principal: NÃO
✅ Finalidade: Despesas operacionais
✅ Status: Ativa
```

**Informações PIX:**
```
✅ Chave PIX Tipo: Email
✅ Chave PIX: financeiro@saudetotal.com.br
```

### 4.5. Configurar Conciliação Bancária

**Ativar Conciliação Automática:**
```
✅ Importar OFX: SIM
✅ Frequência: Diária
✅ Notificar Divergências: SIM
✅ Email para Notificações: financeiro@saudetotal.com.br
```

### 4.6. Verificar Contas Cadastradas

**Resultado Esperado:**
```
Contas Bancárias:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ Banco do Brasil CC 12345-6 (Principal)
   Saldo: R$ 10.000,00
   
✅ Caixa Poupança 98765-4
   Saldo: R$ 50.000,00
   
✅ Nubank 11111-1
   Saldo: R$ 5.000,00
   
📊 Saldo Total: R$ 65.000,00
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## 5. Configuração de Fornecedores

### 5.1. Acessar Cadastro

**Passos:**
1. Menu **"Financeiro"** → **"Fornecedores"** → **"+ Novo Fornecedor"**

### 5.2. Cadastrar Fornecedor de Material Médico

```
Informações Básicas:
✅ Razão Social: "Med Supply Ltda"
✅ Nome Fantasia: "Med Supply"
✅ CNPJ: "11.222.333/0001-44"
✅ Inscrição Estadual: "123456789"
✅ Telefone: "(11) 3333-4444"
✅ Email: "vendas@medsupply.com.br"
✅ Site: "www.medsupply.com.br"

Endereço:
✅ CEP: "04567-890"
✅ Rua: "Rua dos Fornecedores"
✅ Número: "100"
✅ Bairro: "Centro"
✅ Cidade: "São Paulo"
✅ Estado: "SP"

Informações Financeiras:
✅ Banco: Bradesco
✅ Agência: 3456
✅ Conta: 34567-8
✅ Tipo: Conta Corrente
✅ Chave PIX: 11.222.333/0001-44

Condições Comerciais:
✅ Prazo de Pagamento: 30 dias
✅ Forma de Pagamento Preferencial: Boleto
✅ Limite de Crédito: R$ 20.000,00

Categorias:
✅ Material Médico-Hospitalar
✅ Status: Ativo
```

### 5.3. Cadastrar Fornecedor de Limpeza

```
Informações Básicas:
✅ Razão Social: "Limpeza Total Serviços Ltda"
✅ Nome Fantasia: "Limpeza Total"
✅ CNPJ: "22.333.444/0001-55"
✅ Telefone: "(11) 4444-5555"
✅ Email: "contato@limpezatotal.com.br"

Informações Financeiras:
✅ Chave PIX: (11) 94444-5555

Condições:
✅ Prazo: Semanal
✅ Forma: PIX
✅ Valor Médio Mensal: R$ 1.500,00

Categoria:
✅ Manutenção e Limpeza
```

### 5.4. Cadastrar Fornecedor de Software

```
Informações Básicas:
✅ Razão Social: "Omni Care Software Ltda"
✅ Nome Fantasia: "Omni Care"
✅ CNPJ: "33.444.555/0001-66"
✅ Email: "financeiro@omnicare.com.br"

Informações Financeiras:
✅ Forma de Pagamento: Cartão de Crédito (recorrente)

Condições:
✅ Frequência: Mensal
✅ Valor: Conforme plano contratado
✅ Dia de Vencimento: 05

Categoria:
✅ Serviços de Terceiros
```

### 5.5. Cadastrar Contador

```
Informações Básicas:
✅ Razão Social: "João Contador ME"
✅ Nome Fantasia: "Contabilidade Silva"
✅ CPF: "444.555.666-77"
✅ CRC: "SP-123456/O"
✅ Telefone: "(11) 5555-6666"
✅ Email: "joao@contabilidadesilva.com.br"

Informações Financeiras:
✅ Chave PIX: 444.555.666-77

Condições:
✅ Frequência: Mensal
✅ Valor: R$ 800,00
✅ Dia de Vencimento: 10
✅ Forma: PIX

Categoria:
✅ Serviços de Terceiros
```

### 5.6. Verificar Fornecedores

**Resultado Esperado:**
```
Fornecedores Cadastrados:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ Med Supply (Material Médico)
✅ Limpeza Total (Limpeza)
✅ Omni Care Software (Software/TI)
✅ Contabilidade Silva (Serviços)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Total: 4 fornecedores ativos
```

---

## 6. Configuração de Permissões

### 6.1. Acessar Gestão de Permissões

**Passos:**
1. Menu **"Administração"** → **"Permissões"** → **"Módulo Financeiro"**

### 6.2. Definir Permissões por Perfil

#### **Perfil: Owner (Proprietário)**
```
✅ Visualizar Dashboard Financeiro
✅ Visualizar Contas a Receber
✅ Visualizar Contas a Pagar
✅ Criar/Editar Recebíveis
✅ Criar/Editar Despesas
✅ Dar Baixa em Pagamentos
✅ Aprovar Descontos
✅ Visualizar Relatórios
✅ Exportar Dados
✅ Configurar Formas de Pagamento
✅ Configurar Categorias
✅ Gerenciar Contas Bancárias
✅ Gerenciar Fornecedores
✅ Fazer Fechamento Financeiro
✅ Acesso Total
```

#### **Perfil: Admin (Administrador)**
```
✅ Visualizar Dashboard Financeiro
✅ Visualizar Contas a Receber
✅ Visualizar Contas a Pagar
✅ Criar/Editar Recebíveis
✅ Criar/Editar Despesas
✅ Dar Baixa em Pagamentos
✅ Aprovar Descontos (até 10%)
✅ Visualizar Relatórios
✅ Exportar Dados
✅ Gerenciar Fornecedores
✅ Fazer Fechamento Financeiro
```

#### **Perfil: Secretary (Secretária)**
```
✅ Visualizar Dashboard Financeiro (limitado)
✅ Visualizar Contas a Receber
✅ Criar Recebíveis (consultas)
✅ Dar Baixa em Pagamentos
✅ Aplicar Descontos (até 5%)
❌ NÃO pode acessar Contas a Pagar
❌ NÃO pode ver relatórios completos
❌ NÃO pode gerenciar configurações
```

#### **Perfil: Doctor (Médico)**
```
✅ Visualizar Dashboard (seus recebimentos)
✅ Visualizar suas Contas a Receber
❌ NÃO pode acessar financeiro geral
❌ NÃO pode dar baixa em pagamentos
❌ NÃO pode aplicar descontos
```

### 6.3. Configurar Aprovações

**Fluxo de Aprovação de Descontos:**
```
Desconto até 5%: Secretária pode aplicar
Desconto 5-10%: Requer aprovação do Admin
Desconto >10%: Requer aprovação do Owner
```

**Fluxo de Aprovação de Despesas:**
```
Despesa até R$ 500: Secretária pode lançar
Despesa R$ 500-2.000: Requer aprovação do Admin
Despesa >R$ 2.000: Requer aprovação do Owner
```

### 6.4. Verificar Permissões

**Teste:**
1. Faça login com cada perfil
2. Verifique acesso ao menu Financeiro
3. Teste operações permitidas/negadas

---

## 7. Configuração de Regras de Negócio

### 7.1. Regras de Recebimento

**Configurar em:** Financeiro → Configurações → Regras de Negócio

```
✅ Gerar Recebível Automaticamente: SIM (ao finalizar consulta)
✅ Permitir Pagamento Parcelado: SIM
✅ Parcelas Máximas: 12
✅ Valor Mínimo da Parcela: R$ 50,00
✅ Aplicar Juros em Parcelamento: SIM
✅ Taxa de Juros (%): 1.99 ao mês
```

**Inadimplência:**
```
✅ Dias de Tolerância: 5 dias
✅ Aplicar Multa Após: 5 dias
✅ Multa por Atraso (%): 2%
✅ Juros Diários (%): 0.033% (1% ao mês)
✅ Enviar Notificação: SIM
✅ Enviar 1ª Notificação: 3 dias antes do vencimento
✅ Enviar 2ª Notificação: No dia do vencimento
✅ Enviar 3ª Notificação: 3 dias após vencimento
```

### 7.2. Regras de Pagamento

```
✅ Gerar Contas a Pagar: Manual
✅ Notificar Vencimentos: SIM
✅ Dias de Antecedência: 7 dias
✅ Enviar para: financeiro@saudetotal.com.br
✅ Permitir Pagamento Antecipado: SIM
✅ Aplicar Desconto Antecipado: Negociar com fornecedor
```

### 7.3. Regras de Fluxo de Caixa

```
✅ Regime de Caixa: Competência
✅ Registrar Automaticamente: Pagamentos confirmados
✅ Categorização Obrigatória: SIM
✅ Permitir Movimentação Manual: SIM (Owner/Admin apenas)
✅ Exigir Comprovante: Valores acima de R$ 500
```

### 7.4. Regras de Fechamento

```
✅ Permitir Fechamento Retroativo: NÃO
✅ Exigir Conferência: SIM
✅ Quem pode fechar: Owner, Admin
✅ Bloquear Período Fechado: SIM
✅ Gerar Backup Antes: SIM
```

---

## 8. Verificação Final

### 8.1. Checklist de Configuração Completa

```
✅ Formas de pagamento configuradas (mínimo 5)
✅ Categorias de despesas criadas (mínimo 8)
✅ Pelo menos 1 conta bancária cadastrada
✅ Saldo inicial das contas definido
✅ Fornecedores principais cadastrados
✅ Permissões por perfil configuradas
✅ Regras de inadimplência definidas
✅ Regras de parcelamento configuradas
✅ Notificações ativadas
✅ Fluxos de aprovação estabelecidos
```

### 8.2. Teste Prático

**Cenário 1: Receber Pagamento de Consulta**
```
1. Criar uma consulta particular
2. Finalizar consulta com valor R$ 200,00
3. Registrar pagamento em Dinheiro
4. Verificar se aparece em Contas a Receber
5. Dar baixa no pagamento
6. Verificar se atualiza Fluxo de Caixa
```

**Resultado Esperado:**
- ✅ Recebível gerado automaticamente
- ✅ Pagamento registrado corretamente
- ✅ Baixa realizada com sucesso
- ✅ Fluxo de caixa atualizado

**Cenário 2: Lançar Despesa**
```
1. Ir em Contas a Pagar
2. Clicar em "+ Nova Despesa"
3. Selecionar fornecedor
4. Selecionar categoria
5. Informar valor e vencimento
6. Salvar
```

**Resultado Esperado:**
- ✅ Despesa criada
- ✅ Aparece na lista de Contas a Pagar
- ✅ Notificação de vencimento configurada

### 8.3. Verificar Dashboard

**Acessar:** Menu **"Financeiro"** → **"Dashboard"**

**Deve exibir:**
```
📊 Resumo Financeiro
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💰 Total a Receber: R$ XXX
💸 Total a Pagar: R$ XXX
📈 Saldo Projetado: R$ XXX
📊 Recebido no Mês: R$ XXX
💳 Pago no Mês: R$ XXX
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Gráficos:
✅ Evolução Mensal
✅ Receitas vs Despesas
✅ Por Categoria
✅ Por Forma de Pagamento
```

### 8.4. Próximos Passos

Após configurar o módulo financeiro, prossiga para:

1. **[Configuração Fiscal](08-Configuracao-Fiscal.md)**
   - Regime tributário
   - Impostos (ISS, PIS, COFINS, etc.)
   - Notas fiscais eletrônicas

2. **[Cenário Completo](../CenariosTestesQA/09-Cenario-Completo-Setup-Clinica.md)**
   - Teste completo do zero à primeira consulta
   - Fechamento financeiro mensal

---

## 🔧 Troubleshooting

### Problema: Forma de pagamento não aparece no fechamento

**Soluções:**
1. ✅ Verifique se está com status "Ativo"
2. ✅ Verifique se não há erro de configuração
3. ✅ Faça logout e login novamente
4. ✅ Limpe o cache do navegador

### Problema: Não consigo criar conta bancária

**Soluções:**
1. ✅ Verifique suas permissões (Owner ou Admin necessário)
2. ✅ Certifique-se de preencher todos os campos obrigatórios
3. ✅ Verifique se o código do banco está correto

### Problema: Recebível não é gerado automaticamente

**Soluções:**
1. ✅ Verifique se a regra está ativada nas configurações
2. ✅ Certifique-se de que a consulta foi finalizada corretamente
3. ✅ Verifique se há valor definido para a consulta
4. ✅ Confira se o paciente está cadastrado

### Problema: Permissões não estão funcionando

**Soluções:**
1. ✅ Verifique o perfil do usuário
2. ✅ Refaça o login após alterar permissões
3. ✅ Confirme se as permissões foram salvas corretamente

---

## 📚 Documentação Relacionada

- [Configuração da Clínica](06-Configuracao-Clinica.md)
- [Configuração Fiscal](08-Configuracao-Fiscal.md)
- [Módulo Financeiro - Documentação Técnica](../../system-admin/docs/MODULO_FINANCEIRO.md)
- [Testes do Módulo Financeiro](../../system-admin/docs/testes-configuracao/03-MODULO-FINANCEIRO.md)
- [Guia de Relatórios Financeiros](../../system-admin/guias/GUIA_USUARIO_RELATORIOS_FINANCEIROS.md)

---

## 📊 API Endpoints para Referência

**Formas de Pagamento:**
- `GET /api/payment-methods` - Listar formas
- `POST /api/payment-methods` - Criar forma
- `PUT /api/payment-methods/{id}` - Atualizar forma

**Contas a Receber:**
- `GET /api/accounts-receivable` - Listar recebíveis
- `POST /api/accounts-receivable` - Criar recebível
- `POST /api/accounts-receivable/{id}/pay` - Dar baixa

**Contas a Pagar:**
- `GET /api/accounts-payable` - Listar despesas
- `POST /api/accounts-payable` - Criar despesa
- `POST /api/accounts-payable/{id}/pay` - Pagar despesa

**Fornecedores:**
- `GET /api/suppliers` - Listar fornecedores
- `POST /api/suppliers` - Criar fornecedor
- `PUT /api/suppliers/{id}` - Atualizar fornecedor

---

**Versão:** 1.0  
**Última Atualização:** Fevereiro 2026  
**Mantido por:** Equipe Omni Care Software
