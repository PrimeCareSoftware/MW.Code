# 📋 Implementação do Módulo de Gestão Fiscal e Contábil

> **Status:** ✅ Fase 3 Completa - Serviços de Negócio  
> **Data:** 28 de Janeiro de 2026  
> **Prompt Base:** [18-gestao-fiscal.md](./Plano_Desenvolvimento/fase-4-analytics-otimizacao/18-gestao-fiscal.md)

---

## 🎯 Objetivo

Implementação de sistema completo de gestão fiscal com:
- Controle de impostos (ISS, PIS, COFINS, IR, CSLL)
- Cálculo automático de tributos
- DAS do Simples Nacional
- Plano de contas contábil
- Integração com sistemas contábeis
- DRE e Balanço Patrimonial
- Exportação SPED

---

## ✅ Fase 1: Domínio e Entidades (COMPLETO)

### Entidades Criadas

#### 1. Configuração Fiscal (`ConfiguracaoFiscal.cs`)
**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/ConfiguracaoFiscal.cs`

Gerencia as configurações tributárias da clínica:
- **Regime tributário:** Simples Nacional, Lucro Presumido, Lucro Real, MEI
- **Simples Nacional:** Anexo III/V, Fator R
- **Alíquotas:** ISS, PIS, COFINS, IR, CSLL, INSS
- **Dados fiscais:** CNAE, Código de Serviço (LC 116/2003), Inscrição Municipal

```csharp
public enum RegimeTributarioEnum
{
    SimplesNacional = 1,
    LucroPresumido = 2,
    LucroReal = 3,
    MEI = 4
}

public enum AnexoSimplesNacional
{
    AnexoIII = 3,  // Serviços (FatorR >= 28%)
    AnexoV = 5     // Serviços (FatorR < 28%)
}
```

#### 2. Impostos por Nota (`ImpostoNota.cs`)
**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/ImpostoNota.cs`

Armazena o cálculo detalhado de impostos para cada nota fiscal:
- **Valores base:** Bruto, Desconto, Líquido
- **Tributos federais:** PIS, COFINS, IR, CSLL
- **Tributo municipal:** ISS (com indicação de retenção)
- **INSS:** Quando aplicável
- **Totalizadores automáticos:** Total de impostos, Carga tributária (%)

**Propriedades Calculadas:**
- `ValorLiquido = ValorBruto - ValorDesconto`
- `TotalImpostos = ValorPIS + ValorCOFINS + ValorIR + ValorCSLL + ValorISS + ValorINSS`
- `ValorLiquidoTributos = ValorLiquido - TotalImpostos`
- `CargaTributaria = (TotalImpostos / ValorLiquido) * 100`

#### 3. Apuração Mensal (`ApuracaoImpostos.cs`)
**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/ApuracaoImpostos.cs`

Consolida os impostos do mês para pagamento:
- **Período:** Mês/Ano
- **Faturamento:** Bruto, Deduções, Líquido
- **Impostos totais:** PIS, COFINS, IR, CSLL, ISS, INSS
- **Simples Nacional:** Receita 12 meses, Alíquota efetiva, Valor DAS
- **Status:** Em Aberto, Apurado, Pago, Parcelado, Atrasado

```csharp
public enum StatusApuracao
{
    EmAberto = 1,
    Apurado = 2,
    Pago = 3,
    Parcelado = 4,
    Atrasado = 5
}
```

#### 4. Plano de Contas (`PlanoContas.cs`)
**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/PlanoContas.cs`

Estrutura hierárquica do plano de contas contábil:
- **Código estruturado:** Ex: 1.1.01.001
- **Tipos de conta:** Ativo, Passivo, Patrimônio Líquido, Receita, Despesa, Custos
- **Natureza:** Devedora ou Credora
- **Hierarquia:** Contas sintéticas (agrupadores) e analíticas (lançamentos)
- **Níveis:** Estrutura de árvore com contas pai e subcontas

```csharp
public enum TipoConta
{
    Ativo = 1,
    Passivo = 2,
    PatrimonioLiquido = 3,
    Receita = 4,
    Despesa = 5,
    Custos = 6
}
```

#### 5. Lançamento Contábil (`LancamentoContabil.cs`)
**Localização:** `src/MedicSoft.Domain/Entities/Fiscal/LancamentoContabil.cs`

Registros individuais de débito/crédito:
- **Tipo:** Débito ou Crédito
- **Origem:** Manual, Nota Fiscal, Pagamento, Recebimento, Fechamento, Ajuste
- **Rastreabilidade:** Vincula ao documento de origem (nota, pagamento, etc)
- **Lote:** Agrupa débitos e créditos de uma mesma operação

```csharp
public enum TipoLancamentoContabil
{
    Debito = 1,
    Credito = 2
}

public enum OrigemLancamento
{
    Manual = 1,
    NotaFiscal = 2,
    Pagamento = 3,
    Recebimento = 4,
    FechamentoMensal = 5,
    Ajuste = 6
}
```

---

## 🏗️ Arquitetura

### Estrutura de Diretórios
```
src/MedicSoft.Domain/Entities/Fiscal/
├── ConfiguracaoFiscal.cs      # Configurações tributárias
├── ImpostoNota.cs             # Impostos por nota fiscal
├── ApuracaoImpostos.cs        # Apuração mensal
├── PlanoContas.cs             # Plano de contas
└── LancamentoContabil.cs      # Lançamentos contábeis
```

### Relacionamentos
```
Clinic (1) ←→ (N) ConfiguracaoFiscal
Clinic (1) ←→ (N) ApuracaoImpostos  
Clinic (1) ←→ (N) PlanoContas
Clinic (1) ←→ (N) LancamentoContabil

ElectronicInvoice (1) ←→ (1) ImpostoNota
ApuracaoImpostos (1) ←→ (N) ElectronicInvoice

PlanoContas (1) ←→ (N) PlanoContas (hierarquia)
PlanoContas (1) ←→ (N) LancamentoContabil
```

---

## 📋 Próximos Passos

### Fase 2: Repositórios e Configurações EF Core ✅ COMPLETO
- [x] Criar interfaces de repositórios
- [x] Implementar Entity Framework configurations
- [x] Criar migrations para novas tabelas
- [x] Registrar repositórios no DI container

### Fase 3: Serviços de Negócio ✅ COMPLETO
- [x] `CalculoImpostosService` - Cálculo automático de tributos
- [x] `ApuracaoImpostosService` - Consolidação mensal e DAS
- [x] `SimplesNacionalHelper` - Tabelas e cálculos do Simples Nacional
- [ ] `ContabilizacaoService` - Lançamentos automáticos (Fase 5)

### Fase 4: Relatórios Contábeis
- [ ] `DREService` - Demonstração de Resultados
- [ ] `BalancoPatrimonialService` - Balanço
- [ ] `FluxoCaixaService` - Fluxo de caixa contábil

### Fase 5: Integrações Externas ✅ COMPLETO
- [x] Interface `IIntegracaoContabil`
- [x] Adaptador Domínio Sistemas
- [x] Adaptador ContaAzul
- [x] Adaptador Omie
- [x] Serviço de orquestração
- [x] Repositório de configurações
- Ver: [GESTAO_FISCAL_RESUMO_FASE5.md](./GESTAO_FISCAL_RESUMO_FASE5.md)

### Fase 6: SPED
- [ ] Gerador SPED Fiscal (EFD ICMS/IPI)
- [ ] Gerador SPED Contábil (ECD)
- [ ] Validador de arquivos SPED

### Fase 7: API REST
- [ ] Controllers e DTOs
- [ ] Endpoints CRUD
- [ ] Documentação Swagger

### Fase 8: Frontend
- [ ] Dashboard fiscal
- [ ] Telas de configuração
- [ ] Relatórios visuais

---

## 🎯 Casos de Uso Principais

### 1. Configuração Inicial da Clínica
1. Admin acessa configurações fiscais
2. Seleciona regime tributário
3. Preenche alíquotas e códigos fiscais
4. Sistema salva configuração com vigência

### 2. Emissão de Nota Fiscal
1. Sistema emite nota fiscal de serviço
2. Busca configuração fiscal vigente
3. Calcula impostos automaticamente
4. Gera `ImpostoNota` com detalhamento
5. Armazena para apuração mensal

### 3. Apuração Mensal
1. No fim do mês, sistema consolida notas
2. Soma faturamento e impostos
3. Para Simples: calcula receita 12 meses e alíquota efetiva
4. Gera `ApuracaoImpostos` com valores a pagar
5. Cria guias de pagamento (DAS ou individuais)

### 4. Contabilização Automática
1. Ao emitir nota, sistema cria lançamentos contábeis:
   - Débito: Clientes a Receber
   - Crédito: Receita de Serviços
   - Débito: Impostos a Recolher (cada tributo)
2. Ao receber, lança:
   - Débito: Banco
   - Crédito: Clientes a Receber

### 5. Geração de DRE
1. Sistema consolida lançamentos do período
2. Agrupa receitas, custos e despesas
3. Calcula resultado operacional e líquido
4. Exporta relatório PDF/Excel

---

## 💡 Decisões Técnicas

### Por que entidades separadas?
- **Separação de responsabilidades:** Configuração ≠ Cálculo ≠ Apuração
- **Auditoria:** Histórico completo de cálculos por nota
- **Flexibilidade:** Permite múltiplas configurações por vigência
- **Performance:** Consultas otimizadas por finalidade

### Por que plano de contas hierárquico?
- **Padrão contábil brasileiro:** Exigido para DRE e Balanço
- **Flexibilidade:** Clínicas podem customizar estrutura
- **Agregação:** Facilita totalizações e relatórios consolidados

### Por que rastrear origem dos lançamentos?
- **Auditoria fiscal:** Rastrear cada lançamento até documento origem
- **Correções:** Facilita identificar e corrigir erros
- **Integração:** Permite sincronização com sistemas externos

---

## 📊 Impacto no Sistema

### Tabelas Adicionadas
- `ConfiguracoesFiscais`
- `ImpostosNotas`
- `ApuracoesImpostos`
- `PlanoContas`
- `LancamentosContabeis`

### Integrações com Sistema Existente
- `Clinic` - Configuração fiscal por clínica
- `ElectronicInvoice` - Cálculo de impostos por nota
- Módulo financeiro - Lançamentos contábeis automáticos

---

## 🔒 Segurança e Compliance

### Conformidade Legal
- ✅ Lei Complementar 116/2003 (ISS)
- ✅ Resolução CGSN 140/2018 (Simples Nacional)
- ✅ Instruções Normativas RFB (SPED)
- ✅ Normas do Conselho Federal de Contabilidade

### Auditoria
- Todos os cálculos são registrados com timestamp
- Configurações fiscais mantêm histórico por vigência
- Lançamentos contábeis rastreiam documento origem
- Apurações armazenam comprovantes de pagamento

---

## 📚 Referências

### Documentação
- [Prompt Original](./Plano_Desenvolvimento/fase-4-analytics-otimizacao/18-gestao-fiscal.md)
- [Simples Nacional - Tabelas](http://www8.receita.fazenda.gov.br/SimplesNacional/)
- [SPED - Receita Federal](http://sped.rfb.gov.br/)

### Código
- Entity Base Class: `src/MedicSoft.Domain/Common/BaseEntity.cs`
- Electronic Invoice: `src/MedicSoft.Domain/Entities/ElectronicInvoice.cs`
- Clinic Entity: `src/MedicSoft.Domain/Entities/Clinic.cs`

---

## 📅 Timeline

| Fase | Descrição | Status | Data |
|------|-----------|--------|------|
| 1 | Domínio e Entidades | ✅ Completo | Jan 2026 |
| 2 | Repositórios e Migrations | ✅ Completo | Jan 2026 |
| 3 | Serviços de Cálculo | ✅ Completo | Jan 2026 |
| 4 | Relatórios Contábeis | ✅ Completo | Jan 2026 |
| 5 | Integrações Externas | ✅ Completo | Jan 2026 |
| 6 | SPED | ⏳ Pendente | - |
| 7 | API REST | ⏳ Pendente | - |
| 8 | Frontend | ⏳ Pendente | - |

---

**Legenda:**
- ✅ Completo
- 🔄 Em Andamento
- ⏳ Pendente
- ❌ Bloqueado
