# 08 - Configuração Fiscal e Notas Fiscais

> **Objetivo:** Configurar o sistema fiscal, impostos e emissão de notas fiscais  
> **Tempo estimado:** 40-50 minutos  
> **Pré-requisitos:** 
> - Clínica configurada ([ver Configuração da Clínica](06-Configuracao-Clinica.md))
> - Módulo Financeiro configurado ([ver Configuração Financeiro](07-Configuracao-Financeiro.md))
> - Contador contratado (recomendado)
> - CNPJ regularizado

## 📋 Índice

1. [Visão Geral do Módulo Fiscal](#1-visão-geral-do-módulo-fiscal)
2. [Configuração Fiscal Básica](#2-configuração-fiscal-básica)
3. [Configuração de Impostos](#3-configuração-de-impostos)
4. [Configuração de Notas Fiscais](#4-configuração-de-notas-fiscais)
5. [Integração com Sistemas de NF-e/NFS-e](#5-integração-com-sistemas-de-nf-e-nfs-e)
6. [Plano de Contas Contábil](#6-plano-de-contas-contábil)
7. [Apuração Mensal de Impostos](#7-apuração-mensal-de-impostos)
8. [Verificação Final](#8-verificação-final)

---

## 1. Visão Geral do Módulo Fiscal

### 1.1. Funcionalidades Disponíveis

**Gestão Fiscal:**
- ✅ Configuração de regime tributário
- ✅ Cálculo automático de impostos
- ✅ Apuração mensal de tributos
- ✅ Geração de DAS (Simples Nacional)
- ✅ Controle de impostos retidos
- ✅ Histórico de alterações fiscais

**Notas Fiscais:**
- ✅ Controle interno de invoices
- ✅ Integração com sistemas de NF-e/NFS-e
- ✅ Cálculo automático de tributos
- ✅ Histórico de emissões
- ✅ Cancelamento de notas

**Contabilidade:**
- ✅ Plano de contas contábil
- ✅ Lançamentos automáticos
- ✅ DRE (Demonstrativo de Resultados)
- ✅ Balanço Patrimonial
- ✅ Exportação para sistemas contábeis

### 1.2. Informações Importantes

**⚠️ ATENÇÃO - Nota Fiscal Eletrônica:**

O sistema atual oferece **controle interno de invoices** (faturas), mas **NÃO emite notas fiscais oficiais (NF-e/NFS-e)** automaticamente.

Para emitir notas fiscais com validade jurídica, você precisará:
1. Contratar um serviço especializado (Focus NFe, ENotas, PlugNotas)
2. Configurar integração com o serviço escolhido

**Por que usar serviço externo?**
- ✅ Homologação junto à SEFAZ já realizada
- ✅ Certificado digital gerenciado pelo serviço
- ✅ Atualização automática de layout
- ✅ Suporte especializado
- ✅ Menor custo vs desenvolvimento próprio

### 1.3. Fluxo de Configuração

```
1. Definir Regime Tributário
   ↓
2. Configurar Impostos e Alíquotas
   ↓
3. Preencher Dados Fiscais
   ↓
4. Configurar Serviço de NF-e/NFS-e (se aplicável)
   ↓
5. Criar Plano de Contas
   ↓
6. Testar Emissão
   ↓
7. Pronto para operar!
```

---

## 2. Configuração Fiscal Básica

### 2.1. Acessar Configurações Fiscais

**Passos:**
1. Menu **"Financeiro"** → **"Fiscal"** → **"Configurações"**
2. Ou: Menu **"Configurações"** → **"Fiscal"**

### 2.2. Dados Fiscais da Clínica

```
Informações Básicas:
✅ CNPJ: 12.345.678/0001-90 (já cadastrado)
✅ Razão Social: Clínica Saúde Total Ltda (já cadastrado)
✅ Inscrição Estadual: 123.456.789.012
✅ Inscrição Municipal: 987654321
✅ CNAE Principal: 8630-5/02 (Atividade médica ambulatorial com recursos para realização de exames complementares)
✅ Código de Serviço Municipal: 04.02 (Serviços de análises clínicas, patologia, eletricidade médica, radioterapia, quimioterapia, ultra-sonografia, ressonância magnética, radiologia, tomografia e congêneres)
```

**Lista de CNAEs comuns para clínicas:**
- `8630-5/01` - Atividade médica ambulatorial com recursos para realização de procedimentos cirúrgicos
- `8630-5/02` - Atividade médica ambulatorial com recursos para exames complementares
- `8630-5/03` - Atividade médica ambulatorial restrita a consultas
- `8630-5/04` - Atividade odontológica
- `8650-0/01` - Atividades de psicologia e psicanálise
- `8650-0/03` - Atividades de nutrição

**Códigos de Serviço (LC 116/2003):**
- `04.01` - Medicina e biomedicina
- `04.02` - Análises clínicas, patologia, radiologia, tomografia
- `04.03` - Hospitais, clínicas, laboratórios, sanatórios
- `05.09` - Planos de medicina de grupo ou individual e convênios

### 2.3. Escolher Regime Tributário

**Opções disponíveis:**

#### **Opção 1: Simples Nacional (Recomendado para pequenas e médias)**
```
✅ Regime: Simples Nacional
✅ Anexo: Anexo III ou V (depende do Fator R)
✅ Faturamento Anual: Até R$ 4.800.000
✅ Vantagens: Tributos unificados em uma guia (DAS)
✅ Quando usar: Faturamento até R$ 4,8 milhões/ano
```

**Fator R - Anexo III ou V?**
```
Fator R = (Folha de Pagamento últimos 12 meses) / (Receita Bruta últimos 12 meses)

Se Fator R ≥ 28%: Anexo III (alíquotas menores: 6% a 19,5%)
Se Fator R < 28%: Anexo V (alíquotas maiores: 15,5% a 30,5%)
```

**Exemplo de cálculo:**
```
Receita Bruta 12 meses: R$ 600.000
Folha de Pagamento 12 meses: R$ 180.000
Fator R = 180.000 / 600.000 = 0,30 = 30%
Resultado: Fator R ≥ 28% → Anexo III
```

#### **Opção 2: Lucro Presumido**
```
✅ Regime: Lucro Presumido
✅ Faturamento Anual: Até R$ 78.000.000
✅ Presunção de Lucro: 32% para serviços de saúde
✅ Quando usar: Faturamento de R$ 4,8 a R$ 78 milhões/ano
```

#### **Opção 3: Lucro Real**
```
✅ Regime: Lucro Real
✅ Faturamento Anual: Qualquer valor
✅ Tributação: Sobre lucro efetivo
✅ Quando usar: Faturamento alto ou margem de lucro baixa
```

#### **Opção 4: MEI (Microempreendedor Individual)**
```
✅ Regime: MEI
✅ Faturamento Anual: Até R$ 81.000
✅ Quando usar: Profissional autônomo individual
✅ Observação: Limitações de atividades e contratações
```

### 2.4. Configuração para Simples Nacional

**Exemplo de configuração mais comum:**

```
Regime Tributário:
✅ Regime: Simples Nacional
✅ Data de Opção: 01/01/2026
✅ Anexo: Anexo III
✅ Fator R Atual: 30.5%

Alíquota Efetiva:
✅ Receita Bruta 12 meses: R$ 360.000,00
✅ Faixa de Enquadramento: 1ª Faixa (até R$ 180.000) - 6%
✅ Parcela a Deduzir: R$ 0,00
✅ Alíquota Efetiva: 6,00%
```

**Tabela Simples Nacional - Anexo III (Serviços):**
| Faixa | Receita Bruta 12 meses | Alíquota | Dedução |
|-------|------------------------|----------|---------|
| 1ª | Até 180.000,00 | 6,00% | - |
| 2ª | De 180.000,01 a 360.000,00 | 11,20% | R$ 9.360,00 |
| 3ª | De 360.000,01 a 720.000,00 | 13,50% | R$ 17.640,00 |
| 4ª | De 720.000,01 a 1.800.000,00 | 16,00% | R$ 35.640,00 |
| 5ª | De 1.800.000,01 a 3.600.000,00 | 21,00% | R$ 125.640,00 |
| 6ª | De 3.600.000,01 a 4.800.000,00 | 33,00% | R$ 648.000,00 |

### 2.5. Configuração para Lucro Presumido

```
Regime Tributário:
✅ Regime: Lucro Presumido
✅ Data de Opção: 01/01/2026
✅ Presunção de Lucro: 32%
✅ Período de Apuração: Trimestral

Impostos:
✅ PIS: 0,65% sobre faturamento
✅ COFINS: 3,00% sobre faturamento
✅ IR: 15% sobre lucro presumido (32% do faturamento)
✅ Adicional IR: 10% sobre lucro > R$ 60.000 trimestre
✅ CSLL: 9% sobre lucro presumido (32% do faturamento)
✅ ISS: Conforme município (2% a 5%)
```

---

## 3. Configuração de Impostos

### 3.1. Impostos Federais - Simples Nacional

**Para clínicas no Simples Nacional - Anexo III:**

```
DAS (Documento de Arrecadação do Simples):
✅ Inclui: IRPJ, CSLL, PIS, COFINS, CPP (INSS patronal)
✅ NÃO Inclui: ISS (recolhido separadamente em alguns municípios)
✅ Alíquota: Conforme faixa de faturamento (6% a 19,5%)
✅ Vencimento: Dia 20 do mês seguinte
```

**ISS (Imposto Sobre Serviços):**
```
✅ Base de Cálculo: Valor dos serviços prestados
✅ Alíquota: 2% a 5% (conforme município)
✅ Retenção: Pode ser retido na fonte por alguns convênios
✅ Local de Recolhimento: Município onde serviço foi prestado
✅ Exemplo São Paulo: 5%
✅ Exemplo Município XYZ: 2,5%
```

### 3.2. Impostos Federais - Lucro Presumido

**PIS (Programa de Integração Social):**
```
✅ Regime: Cumulativo
✅ Base de Cálculo: Faturamento bruto
✅ Alíquota: 0,65%
✅ Vencimento: Até o dia 25 do mês seguinte
```

**COFINS (Contribuição para Financiamento da Seguridade Social):**
```
✅ Regime: Cumulativo
✅ Base de Cálculo: Faturamento bruto
✅ Alíquota: 3,00%
✅ Vencimento: Até o dia 25 do mês seguinte
```

**IRPJ (Imposto de Renda Pessoa Jurídica):**
```
✅ Base de Cálculo: 32% do faturamento (presunção)
✅ Alíquota: 15% sobre lucro presumido
✅ Adicional: 10% sobre lucro > R$ 20.000/mês
✅ Recolhimento: Trimestral (últimos dias de março, junho, setembro, dezembro)
```

**CSLL (Contribuição Social sobre Lucro Líquido):**
```
✅ Base de Cálculo: 32% do faturamento
✅ Alíquota: 9% sobre lucro presumido
✅ Recolhimento: Trimestral
```

**INSS Patronal:**
```
✅ Base de Cálculo: Folha de pagamento
✅ Alíquota: 20% sobre salários + RAT/FAP
✅ Vencimento: Dia 20 do mês seguinte
```

### 3.3. Configurar Alíquotas no Sistema

**Passos:**
1. Menu **"Fiscal"** → **"Configurações"** → **"Impostos"**
2. Preencher alíquotas:

```
Simples Nacional:
✅ Anexo: III
✅ Faixa Atual: 1ª (6%)
✅ Atualização Automática: SIM (sistema recalcula mensalmente)

ISS Municipal:
✅ Alíquota: 5,00%
✅ Código de Serviço: 04.02
✅ Retenção na Fonte: SIM (quando aplicável)
✅ Município: São Paulo - SP

OU (se Lucro Presumido):

PIS:
✅ Alíquota: 0,65%
✅ Regime: Cumulativo

COFINS:
✅ Alíquota: 3,00%
✅ Regime: Cumulativo

IR:
✅ Base: Lucro Presumido 32%
✅ Alíquota: 15%
✅ Adicional: 10% (lucro > R$ 20.000/mês)

CSLL:
✅ Base: Lucro Presumido 32%
✅ Alíquota: 9%

ISS:
✅ Alíquota: 5,00%
```

### 3.4. Configurar Retenções

**ISS Retido na Fonte:**
```
✅ Ativar Retenção: SIM
✅ Percentual Retido: 5%
✅ Aplicar em: Convênios e empresas
✅ Não aplicar em: Consultas particulares
✅ Gerar Comprovante: SIM
```

**INSS Retido (Serviços PJ para PJ):**
```
✅ Ativar Retenção INSS: NÃO (para serviços de saúde, geralmente não há)
✅ Se aplicável: 11% sobre valor do serviço
```

---

## 4. Configuração de Notas Fiscais

### 4.1. Configuração Básica de Invoice (Controle Interno)

**O sistema PrimeCare oferece controle interno de invoices:**

```
Numeração:
✅ Série: 1
✅ Número Inicial: 1
✅ Incremento: Automático
✅ Formato: AAAA/NNNNNN (2026/000001)

Dados Padrão:
✅ Descrição do Serviço: "Consulta médica"
✅ CNAE: 8630-5/02
✅ Código de Serviço: 04.02
✅ Natureza da Operação: Prestação de serviços

Impostos:
✅ Calcular Automaticamente: SIM
✅ Exibir Discriminação: SIM
✅ Incluir Carga Tributária: SIM
```

### 4.2. Modelo de Invoice Gerado

**Exemplo de invoice (controle interno):**

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
              INVOICE / FATURA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

EMITENTE
Clínica Saúde Total Ltda
CNPJ: 12.345.678/0001-90
Av. Paulista, 1578 - São Paulo/SP
Tel: (11) 3456-7890

TOMADOR
João da Silva
CPF: 123.456.789-00

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
DISCRIMINAÇÃO DOS SERVIÇOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Consulta médica - Clínica Geral
Data: 15/02/2026
Profissional: Dra. Maria Santos - CRM 123456

Valor dos Serviços: R$ 200,00

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CÁLCULO DOS IMPOSTOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Simples Nacional (Anexo III): R$ 12,00 (6,00%)
ISS: R$ 10,00 (5,00%)

Total de Impostos: R$ 22,00
Carga Tributária: 11,00%

VALOR LÍQUIDO: R$ 178,00

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Invoice Nº: 2026/000001
Data de Emissão: 15/02/2026
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

**⚠️ IMPORTANTE:** Este invoice é apenas para controle interno. Não tem validade fiscal oficial.

---

## 5. Integração com Sistemas de NF-e/NFS-e

### 5.1. Escolher Provedor de NF-e/NFS-e

**Provedores recomendados:**

| Provedor | Tipos | Preço Médio | Recomendado Para |
|----------|-------|-------------|------------------|
| **Focus NFe** | NF-e, NFS-e, NFC-e | R$ 59-199/mês | Pequenas e médias |
| **ENotas** | NFS-e, NF-e | R$ 49-149/mês | Pequenas clínicas |
| **PlugNotas** | NF-e, NFS-e | R$ 69-299/mês | Médias e grandes |
| **NFSe.io** | NFS-e | R$ 39-99/mês | Foco em serviços |

**Fatores de escolha:**
- ✅ Atende seu município (NFS-e)
- ✅ Suporte técnico
- ✅ API REST disponível
- ✅ Documentação clara
- ✅ Preço compatível com volume
- ✅ Trial gratuito

### 5.2. Contratar Provedor (Exemplo: Focus NFe)

**Passos:**
1. Acesse https://focusnfe.com.br
2. Clique em "Começar Teste Grátis"
3. Preencha cadastro da clínica
4. Escolha plano
5. Configure certificado digital A1

**Informações necessárias:**
```
✅ CNPJ da clínica
✅ Certificado Digital A1 (.pfx)
✅ Senha do certificado
✅ Código do município (IBGE)
✅ Série da NFS-e
```

### 5.3. Configurar Integração no PrimeCare

**Passos:**
1. Menu **"Fiscal"** → **"Configurações"** → **"NF-e/NFS-e"**
2. Selecione **"Habilitar NFS-e"**
3. Escolha o provedor:

```
Provedor: Focus NFe
✅ Token API: [seu token da Focus NFe]
✅ Ambiente: Homologação (para testes) ou Produção
✅ Certificado Digital: Upload do arquivo .pfx
✅ Senha do Certificado: ********

Configurações da NFS-e:
✅ Série: 1
✅ Próximo Número: 1
✅ Regime Especial: [se aplicável]
✅ Natureza da Operação: Tributação no município
✅ OptanteSimples Nacional: SIM
✅ Incentivo Fiscal: NÃO
✅ Código de Tributação Municipal: [conforme município]
```

### 5.4. Testar Integração

**Teste em Ambiente de Homologação:**

```
1. Criar consulta de teste
2. Finalizar consulta
3. Clicar em "Emitir NFS-e"
4. Sistema envia para provedor
5. Provedor valida com prefeitura (homologação)
6. Retorna sucesso ou erro
```

**Resultado Esperado:**
```
✅ NFS-e emitida com sucesso
✅ Número da nota: 2026000001
✅ Código de Verificação: ABC123DEF456
✅ XML da nota armazenado
✅ PDF disponível para download
✅ Email enviado ao paciente automaticamente
```

### 5.5. Passar para Produção

**Quando tudo estiver testado:**

```
1. Voltar em Configurações
2. Alterar Ambiente: Homologação → Produção
3. Confirmar mudança
4. Sistema passa a emitir notas oficiais
```

**⚠️ ATENÇÃO:** 
- Certifique-se que todos os dados fiscais estão corretos
- Certificado digital deve estar válido
- Mantenha backup do certificado em local seguro

---

## 6. Plano de Contas Contábil

### 6.1. Criar Plano de Contas Padrão

**Passos:**
1. Menu **"Fiscal"** → **"Contabilidade"** → **"Plano de Contas"**
2. Clicar em **"Importar Plano Padrão"**
3. Selecionar: **"Plano de Contas para Clínicas e Consultórios"**

### 6.2. Estrutura do Plano de Contas

**Contas principais criadas:**

```
1. ATIVO
   1.1 CIRCULANTE
       1.1.01 Disponível
              1.1.01.001 Caixa
              1.1.01.002 Banco Conta Corrente
              1.1.01.003 Aplicações Financeiras
       1.1.02 Clientes
              1.1.02.001 Clientes a Receber
              1.1.02.002 (-) Provisão p/ Devedores Duvidosos
       1.1.03 Estoques
              1.1.03.001 Material Médico
              1.1.03.002 Medicamentos
              1.1.03.003 Material de Limpeza

   1.2 NÃO CIRCULANTE
       1.2.01 Imobilizado
              1.2.01.001 Móveis e Utensílios
              1.2.01.002 Equipamentos Médicos
              1.2.01.003 Computadores e Periféricos
              1.2.01.004 (-) Depreciação Acumulada

2. PASSIVO
   2.1 CIRCULANTE
       2.1.01 Fornecedores
              2.1.01.001 Fornecedores a Pagar
       2.1.02 Obrigações Trabalhistas
              2.1.02.001 Salários a Pagar
              2.1.02.002 INSS a Recolher
              2.1.02.003 FGTS a Recolher
       2.1.03 Obrigações Tributárias
              2.1.03.001 ISS a Recolher
              2.1.03.002 PIS a Recolher
              2.1.03.003 COFINS a Recolher
              2.1.03.004 IRPJ a Recolher
              2.1.03.005 CSLL a Recolher
              2.1.03.006 Simples Nacional (DAS)

3. PATRIMÔNIO LÍQUIDO
   3.1 Capital Social
       3.1.01.001 Capital Subscrito
   3.2 Lucros/Prejuízos
       3.2.01.001 Lucros Acumulados
       3.2.01.002 Resultado do Exercício

4. RECEITAS
   4.1 Receitas de Serviços
       4.1.01.001 Consultas Particulares
       4.1.01.002 Consultas Convênio
       4.1.01.003 Procedimentos
       4.1.01.004 Telemedicina
   4.2 Outras Receitas
       4.2.01.001 Receitas Financeiras
       4.2.01.002 Outras Receitas Operacionais

5. CUSTOS E DESPESAS
   5.1 Despesas com Pessoal
       5.1.01.001 Salários
       5.1.01.002 Encargos Sociais
       5.1.01.003 Benefícios
       5.1.01.004 Treinamentos
   5.2 Despesas Administrativas
       5.2.01.001 Aluguel
       5.2.01.002 Energia Elétrica
       5.2.01.003 Água e Esgoto
       5.2.01.004 Telefone e Internet
       5.2.01.005 Material de Expediente
   5.3 Despesas Operacionais
       5.3.01.001 Material Médico
       5.3.01.002 Medicamentos
       5.3.01.003 Limpeza e Higiene
       5.3.01.004 Manutenção
   5.4 Despesas Tributárias
       5.4.01.001 ISS
       5.4.01.002 PIS
       5.4.01.003 COFINS
       5.4.01.004 IRPJ
       5.4.01.005 CSLL
       5.4.01.006 DAS - Simples Nacional
   5.5 Despesas Financeiras
       5.5.01.001 Juros Pagos
       5.5.01.002 Taxas Bancárias
```

### 6.3. Personalizar Plano de Contas

**Adicionar contas específicas da sua clínica:**

```
Exemplo: Adicionar subconta para Telemedicina
Conta Pai: 4.1.01 Receitas de Serviços
✅ Código: 4.1.01.005
✅ Nome: Consultas por Telemedicina
✅ Tipo: Receita
✅ Natureza: Credora
✅ Aceita Lançamento: SIM
✅ Status: Ativa
```

### 6.4. Vincular Categorias Financeiras

**Vincular categorias de despesas com contas contábeis:**

```
Categoria: Material Médico-Hospitalar
✅ Conta Contábil: 5.3.01.001 (Material Médico)

Categoria: Aluguel
✅ Conta Contábil: 5.2.01.001 (Aluguel)

Categoria: Salários e Encargos
✅ Conta Contábil: 5.1.01.001 (Salários)
```

---

## 7. Apuração Mensal de Impostos

### 7.1. Configurar Apuração Automática

**Passos:**
1. Menu **"Fiscal"** → **"Apuração"** → **"Configurações"**

```
Apuração Mensal:
✅ Executar Automaticamente: SIM
✅ Dia da Apuração: Último dia útil do mês
✅ Notificar Responsável: SIM
✅ Email: financeiro@saudetotal.com.br

Simples Nacional:
✅ Calcular DAS: Automaticamente
✅ Considerar Receita 12 meses: SIM
✅ Aplicar Fator R: SIM (quando aplicável)
✅ Gerar PDF do DAS: SIM

ISS:
✅ Calcular Separadamente: SIM (se município exigir)
✅ Considerar Retenções: SIM
✅ Gerar Guia: SIM
```

### 7.2. Processo de Apuração Manual

**Executar apuração do mês:**

```
1. Acessar: Fiscal → Apuração → Nova Apuração
2. Selecionar período: 02/2026
3. Clicar em "Calcular Impostos"
4. Sistema processa:
   ✅ Soma todas as notas fiscais emitidas
   ✅ Deduz devoluções e cancelamentos
   ✅ Calcula receita bruta do mês
   ✅ Soma receita últimos 12 meses
   ✅ Determina faixa do Simples Nacional
   ✅ Calcula alíquota efetiva
   ✅ Aplica fator R (se necessário)
   ✅ Gera valor do DAS
   ✅ Calcula ISS separado (se aplicável)
5. Exibe resumo da apuração
6. Clicar em "Confirmar e Gerar Guias"
```

### 7.3. Resultado da Apuração

**Exemplo de apuração gerada:**

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
   APURAÇÃO DE IMPOSTOS - FEVEREIRO/2026
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

RECEITAS DO MÊS:
Receita Bruta: R$ 45.000,00
(-) Devoluções: R$ 0,00
(-) Cancelamentos: R$ 0,00
(=) Receita Líquida: R$ 45.000,00

RECEITA ACUMULADA 12 MESES:
Fev/2025 a Jan/2026: R$ 420.000,00
Fev/2026: R$ 45.000,00
Total 12 meses: R$ 465.000,00

SIMPLES NACIONAL:
Receita 12 meses: R$ 465.000,00
Faixa: 3ª (de R$ 360k a R$ 720k)
Alíquota: 13,50%
Parcela a deduzir: R$ 17.640,00
Valor DAS: (465.000 × 13,50%) - 17.640 = R$ 44.767,50 / 12 = R$ 3.730,63
Valor DAS do mês: R$ 3.730,63
Vencimento: 20/03/2026

ISS (recolhimento separado):
Base de cálculo: R$ 45.000,00
Alíquota: 5%
Valor ISS: R$ 2.250,00
(-) ISS já recolhido no DAS: R$ 0,00
(=) ISS a recolher separadamente: R$ 2.250,00
Vencimento: 10/03/2026

TOTAL A RECOLHER: R$ 5.980,63
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

### 7.4. Gerar Guias de Pagamento

**Sistema gera automaticamente:**

```
✅ DAS - Documento de Arrecadação do Simples Nacional
   - Código de barras
   - QR Code PIX
   - PDF para impressão

✅ Guia ISS Municipal (se aplicável)
   - Código de barras
   - Dados para pagamento
   - PDF para impressão
```

### 7.5. Registrar Pagamento

**Após pagar as guias:**

```
1. Acessar apuração do mês
2. Clicar em "Registrar Pagamento"
3. Informar:
   ✅ Data do Pagamento: 18/03/2026
   ✅ Valor Pago: R$ 5.980,63
   ✅ Forma de Pagamento: Transferência Bancária
   ✅ Conta Bancária: Banco do Brasil CC 12345-6
   ✅ Comprovante: Upload do PDF
4. Salvar
```

**Sistema atualiza:**
- ✅ Apuração marcada como "Paga"
- ✅ Lançamento contábil gerado automaticamente
- ✅ Fluxo de caixa atualizado

---

## 8. Verificação Final

### 8.1. Checklist de Configuração Completa

```
Configuração Fiscal:
✅ Regime tributário definido
✅ Dados fiscais completos (CNPJ, IE, IM, CNAE)
✅ Alíquotas de impostos configuradas
✅ Código de serviço municipal definido

Notas Fiscais:
✅ Invoice interno configurado
✅ Numeração sequencial ativa
✅ Cálculo automático de impostos funcionando
✅ (Opcional) Integração NF-e/NFS-e configurada

Plano de Contas:
✅ Plano de contas importado
✅ Contas personalizadas adicionadas
✅ Categorias vinculadas a contas contábeis

Apuração:
✅ Apuração automática configurada
✅ Notificações ativadas
✅ Processo testado com sucesso
```

### 8.2. Teste Prático Completo

**Cenário: Emitir primeira nota e apurar:**

```
1. Criar consulta particular - R$ 200,00
2. Finalizar consulta
3. Emitir invoice/nota
4. Verificar cálculo de impostos:
   - Simples Nacional (6%): R$ 12,00
   - ISS (5%): R$ 10,00
   - Total impostos: R$ 22,00
5. Confirmar emissão
6. Verificar se aparece em "Notas Emitidas"
7. (Fim do mês) Executar apuração
8. Verificar se nota foi incluída
9. Gerar guias de pagamento
10. Registrar pagamento
```

**Resultado Esperado:**
- ✅ Todos os passos executados com sucesso
- ✅ Cálculos corretos
- ✅ Guias geradas
- ✅ Sistema pronto para operação

### 8.3. Próximos Passos

**Após configuração fiscal:**

1. **Iniciar operação da clínica**
   - Cadastrar pacientes
   - Agendar consultas
   - Emitir notas fiscais

2. **Rotinas mensais:**
   - Executar apuração
   - Pagar impostos
   - Enviar relatórios ao contador

3. **Documentação para o contador:**
   - Exportar DRE mensal
   - Exportar Balancete
   - Enviar XMLs das notas fiscais
   - Enviar comprovantes de pagamento de impostos

### 8.4. Integração com Contador

**Exportar dados para contabilidade:**

```
1. Menu Fiscal → Relatórios → Exportações
2. Selecionar período
3. Escolher formato:
   ✅ Domínio Sistemas (.txt)
   ✅ ContaAzul (.csv)
   ✅ Omie (.json)
   ✅ Excel genérico (.xlsx)
4. Incluir:
   ✅ Notas fiscais emitidas
   ✅ Lançamentos contábeis
   ✅ DRE
   ✅ Balancete
5. Clicar em "Gerar Exportação"
6. Enviar arquivo ao contador
```

---

## 🔧 Troubleshooting

### Problema: Erro ao calcular impostos

**Soluções:**
1. ✅ Verifique se regime tributário está configurado
2. ✅ Confirme se alíquotas estão corretas
3. ✅ Verifique se há receita acumulada (para Simples)
4. ✅ Consulte seu contador

### Problema: Integração NFS-e falhando

**Soluções:**
1. ✅ Verifique token da API do provedor
2. ✅ Confirme certificado digital válido
3. ✅ Teste em ambiente de homologação primeiro
4. ✅ Verifique logs de erro no sistema
5. ✅ Entre em contato com suporte do provedor

### Problema: Plano de contas não aparece

**Soluções:**
1. ✅ Verifique se importou o plano padrão
2. ✅ Confirme permissões de acesso
3. ✅ Limpe cache do navegador
4. ✅ Faça logout e login novamente

### Problema: Apuração com valores incorretos

**Soluções:**
1. ✅ Verifique se todas as notas do mês foram emitidas
2. ✅ Confirme se não há notas duplicadas
3. ✅ Verifique cancelamentos
4. ✅ Recalcule a apuração
5. ✅ Consulte seu contador

---

## 📚 Documentação Relacionada

- [Configuração da Clínica](06-Configuracao-Clinica.md)
- [Configuração Financeiro](07-Configuracao-Financeiro.md)
- [Cenário Completo de Setup](../CenariosTestesQA/09-Cenario-Completo-Setup-Clinica.md)
- [Gestão Fiscal - Documentação Técnica](../../GESTAO_FISCAL_IMPLEMENTACAO.md)
- [Módulo Financeiro](../../system-admin/docs/MODULO_FINANCEIRO.md)
- [Guia NF-e/NFS-e](../../system-admin/guias/NFE_NFSE_USER_GUIDE.md)

---

## ⚖️ Disclaimer Legal

**IMPORTANTE:** 

As informações fiscais e tributárias neste documento são para fins educacionais e de configuração do sistema. **NÃO substituem a orientação de um contador ou advogado tributarista**.

Consulte sempre um profissional da área contábil para:
- ✅ Escolher regime tributário mais adequado
- ✅ Definir alíquotas corretas
- ✅ Cumprir obrigações acessórias
- ✅ Interpretar legislação tributária

A responsabilidade pelo correto cumprimento das obrigações fiscais é do proprietário da clínica e de seu contador.

---

**Versão:** 1.0  
**Última Atualização:** Fevereiro 2026  
**Mantido por:** Equipe PrimeCare Software
