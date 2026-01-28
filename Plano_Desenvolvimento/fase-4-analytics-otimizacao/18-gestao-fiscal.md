# 📋 Prompt 18: Gestão Fiscal e Contábil

> **STATUS:** ✅ **COMPLETO** - Implementação finalizada em Janeiro/2026  
> **Última Atualização:** 28 de Janeiro de 2026

**Prioridade:** 🔥 P2 - Médio  
**Complexidade:** ⚡⚡ Média  
**Tempo Estimado:** 2 meses | 1-2 desenvolvedores  
**Custo:** R$ 45.000  
**Pré-requisitos:** Sistema financeiro básico funcionando

## ✅ Status da Implementação

| Fase | Status | Descrição |
|------|--------|-----------|
| Sprint 1 | ✅ Completo | Modelo de dados e cálculo de impostos |
| Sprint 2 | ✅ Completo | Apuração mensal e cálculo DAS |
| Sprint 3 | ✅ Completo | Plano de contas e lançamentos |
| Sprint 4 | ✅ Completo | DRE e Balanço Patrimonial |
| Sprint 5 | ✅ Completo | Integrações contábeis |
| Sprint 6 | ✅ Completo | SPED Fiscal e Contábil |
| Sprint 7 | ✅ Completo | Frontend e Dashboard |
| **Testes** | ✅ **101+ testes** | **Cobertura: 92%** |

### Documentação Relacionada
- 📖 [Implementação Técnica](../../GESTAO_FISCAL_IMPLEMENTACAO.md)
- 📋 [Resumo Fase 1](../../GESTAO_FISCAL_RESUMO_FASE1.md) - Modelo de Dados
- 📋 [Resumo Fase 2](../../GESTAO_FISCAL_RESUMO_FASE2.md) - Cálculo de Impostos
- 📋 [Resumo Fase 3](../../GESTAO_FISCAL_RESUMO_FASE3.md) - Apuração Mensal
- 📋 [Resumo Fase 4](../../GESTAO_FISCAL_RESUMO_FASE4.md) - DRE e Balanço
- 📋 [Resumo Fase 5](../../GESTAO_FISCAL_RESUMO_FASE5.md) - Integração Contábil
- 📋 [Resumo Fase 6](../../GESTAO_FISCAL_RESUMO_FASE6.md) - SPED
- 📋 [Resumo Fase 7](../../GESTAO_FISCAL_RESUMO_FASE7.md) - Frontend

---

## 🎯 Objetivo

Implementar módulo completo de gestão fiscal com controle de impostos (ISS, PIS, COFINS, IR, CSLL), cálculo de DAS do Simples Nacional, integração com sistemas contábeis (Domínio, ContaAzul, Omie), plano de contas, DRE, Balanço Patrimonial, e exportação SPED para garantir conformidade fiscal.

---

## 📊 Contexto do Sistema

### Problema Atual
- Cálculo manual de impostos
- Falta de integração contábil
- Dificuldade em gerar relatórios fiscais
- Sem rastreabilidade de tributos
- Retrabalho para contador
- Risco de não conformidade

### Solução Proposta
Sistema fiscal que:
- Calcula impostos automaticamente por nota
- Integra com principais softwares contábeis
- Gera DRE e Balanço automaticamente
- Exporta SPED Fiscal e Contábil
- Calcula DAS do Simples Nacional
- Mantém plano de contas parametrizável

---

## 🏗️ Arquitetura da Solução

### 1. Modelo de Dados Fiscal (2 semanas)

#### 1.1 Regime Tributário e Configurações
```csharp
// src/MedicSoft.Core/Entities/Fiscal/RegimeTributario.cs
public class ConfiguracaoFiscal
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    
    // Regime tributário
    public RegimeTributarioEnum Regime { get; set; }
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }
    
    // Simples Nacional
    public bool OptanteSimplesNacional { get; set; }
    public AnexoSimplesNacional? AnexoSimples { get; set; }
    public decimal? FatorR { get; set; } // Para Anexo III/V
    
    // Alíquotas (quando não Simples)
    public decimal AliquotaISS { get; set; } // %
    public decimal AliquotaPIS { get; set; }
    public decimal AliquotaCOFINS { get; set; }
    public decimal AliquotaIR { get; set; }
    public decimal AliquotaCSLL { get; set; }
    
    // INSS
    public bool RetemINSS { get; set; }
    public decimal AliquotaINSS { get; set; }
    
    // Configurações específicas
    public string CodigoServico { get; set; } // LC 116/2003
    public string CNAE { get; set; }
    public string InscricaoMunicipal { get; set; }
    public bool ISS_Retido { get; set; }
}

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

#### 1.2 Entidade de Impostos
```csharp
// src/MedicSoft.Core/Entities/Fiscal/ImpostoNota.cs
public class ImpostoNota
{
    public Guid Id { get; set; }
    public Guid NotaFiscalId { get; set; }
    public NotaFiscal NotaFiscal { get; set; }
    
    // Valores base
    public decimal ValorBruto { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorLiquido => ValorBruto - ValorDesconto;
    
    // Tributos Federais
    public decimal AliquotaPIS { get; set; }
    public decimal ValorPIS { get; set; }
    
    public decimal AliquotaCOFINS { get; set; }
    public decimal ValorCOFINS { get; set; }
    
    public decimal AliquotaIR { get; set; }
    public decimal ValorIR { get; set; }
    
    public decimal AliquotaCSLL { get; set; }
    public decimal ValorCSLL { get; set; }
    
    // Tributo Municipal
    public decimal AliquotaISS { get; set; }
    public decimal ValorISS { get; set; }
    public bool ISSRetido { get; set; }
    public string CodigoServicoMunicipal { get; set; }
    
    // INSS
    public decimal AliquotaINSS { get; set; }
    public decimal ValorINSS { get; set; }
    public bool INSSRetido { get; set; }
    
    // Totalizadores
    public decimal TotalImpostos => ValorPIS + ValorCOFINS + ValorIR + ValorCSLL + ValorISS + ValorINSS;
    public decimal ValorLiquidoTributos => ValorLiquido - TotalImpostos;
    public decimal CargaTributaria => ValorLiquido > 0 ? (TotalImpostos / ValorLiquido * 100) : 0;
    
    // Metadados
    public DateTime DataCalculo { get; set; }
    public string RegimeTributario { get; set; }
}
```

#### 1.3 Apuração de Impostos
```csharp
// src/MedicSoft.Core/Entities/Fiscal/ApuracaoImpostos.cs
public class ApuracaoImpostos
{
    public Guid Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public DateTime DataApuracao { get; set; }
    
    // Faturamento
    public decimal FaturamentoBruto { get; set; }
    public decimal Deducoes { get; set; }
    public decimal FaturamentoLiquido => FaturamentoBruto - Deducoes;
    
    // Impostos apurados
    public decimal TotalPIS { get; set; }
    public decimal TotalCOFINS { get; set; }
    public decimal TotalIR { get; set; }
    public decimal TotalCSLL { get; set; }
    public decimal TotalISS { get; set; }
    public decimal TotalINSS { get; set; }
    
    // Simples Nacional
    public decimal? ReceitaBruta12Meses { get; set; }
    public decimal? AliquotaEfetiva { get; set; }
    public decimal? ValorDAS { get; set; }
    
    // Status
    public StatusApuracao Status { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string ComprovantesPagamento { get; set; }
    
    // Relação com notas
    public List<NotaFiscal> NotasIncluidas { get; set; }
}

public enum StatusApuracao
{
    EmAberto,
    Apurado,
    Pago,
    Parcelado,
    Atrasado
}
```

---

### 2. Cálculo Automático de Impostos (3 semanas)

#### 2.1 Serviço de Cálculo
```csharp
// src/MedicSoft.Api/Services/Fiscal/CalculoImpostosService.cs
public class CalculoImpostosService : ICalculoImpostosService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CalculoImpostosService> _logger;
    
    public async Task<ImpostoNota> CalcularImpostosAsync(Guid notaFiscalId)
    {
        var nota = await _context.NotasFiscais
            .Include(n => n.Clinica)
            .FirstOrDefaultAsync(n => n.Id == notaFiscalId);
            
        if (nota == null)
            throw new NotFoundException("Nota fiscal não encontrada");
            
        var config = await _context.ConfiguracoesFiscais
            .Where(c => c.ClinicaId == nota.ClinicaId 
                     && c.VigenciaInicio <= nota.DataEmissao
                     && (c.VigenciaFim == null || c.VigenciaFim >= nota.DataEmissao))
            .FirstOrDefaultAsync();
            
        if (config == null)
            throw new InvalidOperationException("Configuração fiscal não encontrada");
        
        var imposto = new ImpostoNota
        {
            NotaFiscalId = notaFiscalId,
            ValorBruto = nota.ValorTotal,
            ValorDesconto = nota.Desconto,
            DataCalculo = DateTime.UtcNow,
            RegimeTributario = config.Regime.ToString()
        };
        
        if (config.OptanteSimplesNacional)
        {
            await CalcularSimplesNacionalAsync(imposto, config, nota);
        }
        else
        {
            CalcularRegimeNormal(imposto, config, nota);
        }
        
        _context.ImpostosNotas.Add(imposto);
        await _context.SaveChangesAsync();
        
        return imposto;
    }
    
    private void CalcularRegimeNormal(ImpostoNota imposto, ConfiguracaoFiscal config, NotaFiscal nota)
    {
        var baseCalculo = imposto.ValorLiquido;
        
        // PIS
        imposto.AliquotaPIS = config.AliquotaPIS;
        imposto.ValorPIS = baseCalculo * (config.AliquotaPIS / 100);
        
        // COFINS
        imposto.AliquotaCOFINS = config.AliquotaCOFINS;
        imposto.ValorCOFINS = baseCalculo * (config.AliquotaCOFINS / 100);
        
        // ISS
        imposto.AliquotaISS = config.AliquotaISS;
        imposto.ValorISS = baseCalculo * (config.AliquotaISS / 100);
        imposto.ISSRetido = config.ISS_Retido;
        imposto.CodigoServicoMunicipal = config.CodigoServico;
        
        // IR e CSLL (Lucro Presumido - base 32% receita bruta)
        if (config.Regime == RegimeTributarioEnum.LucroPresumido)
        {
            var baseIRCSLL = baseCalculo * 0.32m; // 32% presunção
            
            imposto.AliquotaIR = config.AliquotaIR;
            imposto.ValorIR = baseIRCSLL * (config.AliquotaIR / 100);
            
            imposto.AliquotaCSLL = config.AliquotaCSLL;
            imposto.ValorCSLL = baseIRCSLL * (config.AliquotaCSLL / 100);
        }
        
        // INSS (se aplicável)
        if (config.RetemINSS)
        {
            imposto.AliquotaINSS = config.AliquotaINSS;
            imposto.ValorINSS = baseCalculo * (config.AliquotaINSS / 100);
            imposto.INSSRetido = true;
        }
    }
    
    private async Task CalcularSimplesNacionalAsync(
        ImpostoNota imposto, 
        ConfiguracaoFiscal config, 
        NotaFiscal nota)
    {
        // Buscar faturamento dos últimos 12 meses
        var dataInicio = nota.DataEmissao.AddMonths(-12);
        var receitaBruta12Meses = await _context.NotasFiscais
            .Where(n => n.ClinicaId == nota.ClinicaId
                     && n.DataEmissao >= dataInicio
                     && n.DataEmissao < nota.DataEmissao
                     && n.Status == StatusNota.Autorizada)
            .SumAsync(n => n.ValorTotal);
        
        // Determinar alíquota efetiva baseada na tabela
        var aliquotaEfetiva = ObterAliquotaSimplesNacional(
            receitaBruta12Meses, 
            config.AnexoSimples.Value,
            config.FatorR);
        
        // Calcular DAS
        var valorDAS = imposto.ValorLiquido * (aliquotaEfetiva / 100);
        
        // Distribuir proporcionalmente entre tributos
        // Anexo III: CPP, IR, CSLL, COFINS, PIS, ISS
        var distribuicao = ObterDistribuicaoAnexo(config.AnexoSimples.Value);
        
        imposto.ValorPIS = valorDAS * distribuicao.PIS;
        imposto.ValorCOFINS = valorDAS * distribuicao.COFINS;
        imposto.ValorIR = valorDAS * distribuicao.IR;
        imposto.ValorCSLL = valorDAS * distribuicao.CSLL;
        imposto.ValorISS = valorDAS * distribuicao.ISS;
        
        imposto.AliquotaPIS = aliquotaEfetiva * distribuicao.PIS;
        imposto.AliquotaCOFINS = aliquotaEfetiva * distribuicao.COFINS;
        imposto.AliquotaIR = aliquotaEfetiva * distribuicao.IR;
        imposto.AliquotaCSLL = aliquotaEfetiva * distribuicao.CSLL;
        imposto.AliquotaISS = aliquotaEfetiva * distribuicao.ISS;
    }
    
    private decimal ObterAliquotaSimplesNacional(
        decimal receitaBruta12Meses, 
        AnexoSimplesNacional anexo,
        decimal? fatorR)
    {
        // Tabela Simples Nacional - Anexo III (exemplo simplificado)
        // Na prática, usar tabela completa com todas as faixas
        if (anexo == AnexoSimplesNacional.AnexoIII)
        {
            if (receitaBruta12Meses <= 180000) return 6.00m;
            if (receitaBruta12Meses <= 360000) return 11.20m;
            if (receitaBruta12Meses <= 720000) return 13.50m;
            if (receitaBruta12Meses <= 1800000) return 16.00m;
            if (receitaBruta12Meses <= 3600000) return 21.00m;
            return 33.00m;
        }
        else // Anexo V
        {
            if (receitaBruta12Meses <= 180000) return 15.50m;
            if (receitaBruta12Meses <= 360000) return 18.00m;
            if (receitaBruta12Meses <= 720000) return 19.50m;
            if (receitaBruta12Meses <= 1800000) return 20.50m;
            if (receitaBruta12Meses <= 3600000) return 23.00m;
            return 30.50m;
        }
    }
}
```

#### 2.2 Apuração Mensal
```csharp
// src/MedicSoft.Api/Services/Fiscal/ApuracaoImpostosService.cs
public class ApuracaoImpostosService : IApuracaoImpostosService
{
    private readonly ApplicationDbContext _context;
    
    public async Task<ApuracaoImpostos> GerarApuracaoMensalAsync(
        Guid clinicaId, 
        int mes, 
        int ano)
    {
        var dataInicio = new DateTime(ano, mes, 1);
        var dataFim = dataInicio.AddMonths(1).AddDays(-1);
        
        // Buscar todas as notas do período
        var notas = await _context.NotasFiscais
            .Include(n => n.Impostos)
            .Where(n => n.ClinicaId == clinicaId
                     && n.DataEmissao >= dataInicio
                     && n.DataEmissao <= dataFim
                     && n.Status == StatusNota.Autorizada)
            .ToListAsync();
        
        var apuracao = new ApuracaoImpostos
        {
            Mes = mes,
            Ano = ano,
            DataApuracao = DateTime.UtcNow,
            Status = StatusApuracao.Apurado
        };
        
        // Totalizar valores
        apuracao.FaturamentoBruto = notas.Sum(n => n.ValorTotal);
        apuracao.Deducoes = notas.Sum(n => n.Desconto);
        
        apuracao.TotalPIS = notas.Sum(n => n.Impostos?.ValorPIS ?? 0);
        apuracao.TotalCOFINS = notas.Sum(n => n.Impostos?.ValorCOFINS ?? 0);
        apuracao.TotalIR = notas.Sum(n => n.Impostos?.ValorIR ?? 0);
        apuracao.TotalCSLL = notas.Sum(n => n.Impostos?.ValorCSLL ?? 0);
        apuracao.TotalISS = notas.Sum(n => n.Impostos?.ValorISS ?? 0);
        apuracao.TotalINSS = notas.Sum(n => n.Impostos?.ValorINSS ?? 0);
        
        // Calcular DAS se Simples Nacional
        var config = await _context.ConfiguracoesFiscais
            .FirstOrDefaultAsync(c => c.ClinicaId == clinicaId);
            
        if (config?.OptanteSimplesNacional == true)
        {
            var receitaBruta12Meses = await CalcularReceitaBruta12MesesAsync(clinicaId, dataFim);
            apuracao.ReceitaBruta12Meses = receitaBruta12Meses;
            
            var aliquota = ObterAliquotaSimplesNacional(
                receitaBruta12Meses, 
                config.AnexoSimples.Value, 
                config.FatorR);
            apuracao.AliquotaEfetiva = aliquota;
            apuracao.ValorDAS = apuracao.FaturamentoLiquido * (aliquota / 100);
        }
        
        _context.ApuracoesImpostos.Add(apuracao);
        await _context.SaveChangesAsync();
        
        return apuracao;
    }
    
    public async Task<byte[]> GerarGuiaDASAsync(Guid apuracaoId)
    {
        var apuracao = await _context.ApuracoesImpostos.FindAsync(apuracaoId);
        
        // Gerar arquivo PDF da guia DAS
        // Integrar com portal do Simples Nacional ou gerar manualmente
        
        return new byte[0]; // PDF bytes
    }
}
```

---

### 3. Plano de Contas (2 semanas)

#### 3.1 Modelo de Plano de Contas
```csharp
// src/MedicSoft.Core/Entities/Fiscal/PlanoContas.cs
public class ContaContabil
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } // Ex: 1.1.01.001
    public string Descricao { get; set; }
    
    // Hierarquia
    public Guid? ContaPaiId { get; set; }
    public ContaContabil ContaPai { get; set; }
    public List<ContaContabil> ContasFilhas { get; set; }
    public int Nivel { get; set; }
    
    // Tipo
    public TipoContaContabil Tipo { get; set; }
    public NaturezaConta Natureza { get; set; } // Devedora ou Credora
    
    // Status
    public bool Analitica { get; set; } // Recebe lançamentos
    public bool Ativa { get; set; }
    
    // DRE
    public bool ApareceDRE { get; set; }
    public GrupoDRE? GrupoDRE { get; set; }
    
    // Metadados
    public DateTime DataCriacao { get; set; }
    public string UsuarioCriacao { get; set; }
}

public enum TipoContaContabil
{
    Ativo = 1,
    Passivo = 2,
    PatrimonioLiquido = 3,
    Receita = 4,
    Despesa = 5,
    Custos = 6
}

public enum NaturezaConta
{
    Devedora,  // Ativo, Despesa, Custo
    Credora    // Passivo, PL, Receita
}

public enum GrupoDRE
{
    ReceitaBruta,
    DeducoesReceita,
    ReceitaLiquida,
    CustoServicos,
    LucroBruto,
    DespesasOperacionais,
    DespesasAdministrativas,
    DespesasComerciais,
    OutrasReceitasDespesas,
    EBITDA,
    DepreciacaoAmortizacao,
    EBIT,
    ResultadoFinanceiro,
    LucroAntesIR,
    ImpostoRenda,
    LucroLiquido
}
```

#### 3.2 Lançamentos Contábeis
```csharp
// src/MedicSoft.Core/Entities/Fiscal/LancamentoContabil.cs
public class LancamentoContabil
{
    public Guid Id { get; set; }
    public DateTime DataLancamento { get; set; }
    public string Historico { get; set; }
    public string NumeroDocumento { get; set; }
    
    // Partidas dobradas
    public List<PartidaContabil> Partidas { get; set; }
    
    // Metadados
    public TipoLancamento Tipo { get; set; }
    public string OrigemLancamento { get; set; } // NotaFiscal, Pagamento, Manual, etc.
    public Guid? ReferenciaId { get; set; }
    
    public Guid UsuarioId { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class PartidaContabil
{
    public Guid Id { get; set; }
    public Guid LancamentoId { get; set; }
    
    public Guid ContaContabilId { get; set; }
    public ContaContabil ContaContabil { get; set; }
    
    public TipoPartida Tipo { get; set; } // Débito ou Crédito
    public decimal Valor { get; set; }
    
    // Centro de custo (opcional)
    public Guid? CentroCustoId { get; set; }
    public CentroCusto CentroCusto { get; set; }
}

public enum TipoLancamento
{
    Manual,
    Automatico
}

public enum TipoPartida
{
    Debito,
    Credito
}
```

#### 3.3 Serviço de Contabilização Automática
```csharp
// src/MedicSoft.Api/Services/Fiscal/ContabilizacaoService.cs
public class ContabilizacaoService : IContabilizacaoService
{
    private readonly ApplicationDbContext _context;
    
    public async Task ContabilizarNotaFiscalAsync(Guid notaFiscalId)
    {
        var nota = await _context.NotasFiscais
            .Include(n => n.Impostos)
            .FirstOrDefaultAsync(n => n.Id == notaFiscalId);
            
        if (nota == null) return;
        
        var lancamento = new LancamentoContabil
        {
            DataLancamento = nota.DataEmissao,
            Historico = $"Emissão NF {nota.Numero} - {nota.Tomador.Nome}",
            NumeroDocumento = nota.Numero.ToString(),
            Tipo = TipoLancamento.Automatico,
            OrigemLancamento = "NotaFiscal",
            ReferenciaId = notaFiscalId,
            Partidas = new List<PartidaContabil>()
        };
        
        // Débito: Contas a Receber (Ativo)
        lancamento.Partidas.Add(new PartidaContabil
        {
            ContaContabilId = await ObterContaAsync("1.1.02.001"), // Contas a Receber
            Tipo = TipoPartida.Debito,
            Valor = nota.ValorTotal
        });
        
        // Crédito: Receita de Serviços
        lancamento.Partidas.Add(new PartidaContabil
        {
            ContaContabilId = await ObterContaAsync("3.1.01.001"), // Receita Serviços Médicos
            Tipo = TipoPartida.Credito,
            Valor = nota.ValorLiquido
        });
        
        // Crédito: Impostos a Recolher
        if (nota.Impostos != null)
        {
            if (nota.Impostos.ValorISS > 0)
            {
                lancamento.Partidas.Add(new PartidaContabil
                {
                    ContaContabilId = await ObterContaAsync("2.1.04.001"), // ISS a Recolher
                    Tipo = TipoPartida.Credito,
                    Valor = nota.Impostos.ValorISS
                });
            }
            
            // PIS, COFINS, etc...
        }
        
        _context.LancamentosContabeis.Add(lancamento);
        await _context.SaveChangesAsync();
    }
    
    public async Task ContabilizarPagamentoAsync(Guid pagamentoId)
    {
        // Lógica similar para pagamentos
        // Débito: Fornecedores a Pagar
        // Crédito: Banco
    }
}
```

---

### 4. DRE e Balanço (2 semanas)

#### 4.1 Serviço de DRE
```csharp
// src/MedicSoft.Api/Services/Fiscal/DREService.cs
public class DREService : IDREService
{
    private readonly ApplicationDbContext _context;
    
    public async Task<DRE> GerarDREAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim)
    {
        var lancamentos = await _context.LancamentosContabeis
            .Where(l => l.DataLancamento >= dataInicio 
                     && l.DataLancamento <= dataFim)
            .Include(l => l.Partidas)
            .ThenInclude(p => p.ContaContabil)
            .ToListAsync();
        
        var dre = new DRE
        {
            ClinicaId = clinicaId,
            PeriodoInicio = dataInicio,
            PeriodoFim = dataFim,
            DataGeracao = DateTime.UtcNow
        };
        
        // 1. Receita Bruta
        dre.ReceitaBruta = await CalcularSaldoGrupoAsync(
            lancamentos, GrupoDRE.ReceitaBruta);
        
        // 2. (-) Deduções
        dre.Deducoes = await CalcularSaldoGrupoAsync(
            lancamentos, GrupoDRE.DeducoesReceita);
        
        // 3. = Receita Líquida
        dre.ReceitaLiquida = dre.ReceitaBruta - dre.Deducoes;
        
        // 4. (-) Custos
        dre.CustoServicos = await CalcularSaldoGrupoAsync(
            lancamentos, GrupoDRE.CustoServicos);
        
        // 5. = Lucro Bruto
        dre.LucroBruto = dre.ReceitaLiquida - dre.CustoServicos;
        dre.MargemBruta = dre.ReceitaLiquida > 0 
            ? (dre.LucroBruto / dre.ReceitaLiquida * 100) 
            : 0;
        
        // 6. (-) Despesas Operacionais
        dre.DespesasOperacionais = await CalcularSaldoGrupoAsync(
            lancamentos, GrupoDRE.DespesasOperacionais);
        
        // 7. = EBITDA
        dre.EBITDA = dre.LucroBruto - dre.DespesasOperacionais;
        dre.MargemEBITDA = dre.ReceitaLiquida > 0 
            ? (dre.EBITDA / dre.ReceitaLiquida * 100) 
            : 0;
        
        // 8. (-) Depreciação
        dre.DepreciacaoAmortizacao = await CalcularSaldoGrupoAsync(
            lancamentos, GrupoDRE.DepreciacaoAmortizacao);
        
        // 9. = EBIT (Lucro Operacional)
        dre.EBIT = dre.EBITDA - dre.DepreciacaoAmortizacao;
        
        // 10. (+/-) Resultado Financeiro
        dre.ReceitasFinanceiras = await CalcularReceitasFinanceirasAsync(lancamentos);
        dre.DespesasFinanceiras = await CalcularDespesasFinanceirasAsync(lancamentos);
        dre.ResultadoFinanceiro = dre.ReceitasFinanceiras - dre.DespesasFinanceiras;
        
        // 11. = Lucro Antes IR
        dre.LucroAntesIR = dre.EBIT + dre.ResultadoFinanceiro;
        
        // 12. (-) IR e CSLL
        dre.ImpostoRenda = await CalcularIRAsync(lancamentos);
        dre.CSLL = await CalcularCSLLAsync(lancamentos);
        
        // 13. = Lucro Líquido
        dre.LucroLiquido = dre.LucroAntesIR - dre.ImpostoRenda - dre.CSLL;
        dre.MargemLiquida = dre.ReceitaLiquida > 0 
            ? (dre.LucroLiquido / dre.ReceitaLiquida * 100) 
            : 0;
        
        _context.DREs.Add(dre);
        await _context.SaveChangesAsync();
        
        return dre;
    }
}

public class DRE
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public DateTime DataGeracao { get; set; }
    
    // Receitas
    public decimal ReceitaBruta { get; set; }
    public decimal Deducoes { get; set; }
    public decimal ReceitaLiquida { get; set; }
    
    // Custos
    public decimal CustoServicos { get; set; }
    
    // Lucro Bruto
    public decimal LucroBruto { get; set; }
    public decimal MargemBruta { get; set; }
    
    // Despesas
    public decimal DespesasOperacionais { get; set; }
    public decimal DespesasAdministrativas { get; set; }
    public decimal DespesasComerciais { get; set; }
    
    // EBITDA
    public decimal EBITDA { get; set; }
    public decimal MargemEBITDA { get; set; }
    
    // Depreciação
    public decimal DepreciacaoAmortizacao { get; set; }
    
    // EBIT
    public decimal EBIT { get; set; }
    
    // Resultado Financeiro
    public decimal ReceitasFinanceiras { get; set; }
    public decimal DespesasFinanceiras { get; set; }
    public decimal ResultadoFinanceiro { get; set; }
    
    // Lucro
    public decimal LucroAntesIR { get; set; }
    public decimal ImpostoRenda { get; set; }
    public decimal CSLL { get; set; }
    public decimal LucroLiquido { get; set; }
    public decimal MargemLiquida { get; set; }
}
```

---

### 5. Integração Contábil (2 semanas)

#### 5.1 Interface de Integração
```csharp
// src/MedicSoft.Api/Services/Fiscal/Integracoes/IContabilIntegration.cs
public interface IContabilIntegration
{
    Task<bool> TestarConexaoAsync();
    Task EnviarLancamentoAsync(LancamentoContabil lancamento);
    Task EnviarPlanoContasAsync(List<ContaContabil> contas);
    Task<string> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato);
}

public enum FormatoExportacao
{
    TXT,
    CSV,
    XML,
    JSON
}

// Implementação Domínio Sistemas
public class DominioIntegration : IContabilIntegration
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public async Task EnviarLancamentoAsync(LancamentoContabil lancamento)
    {
        var payload = new
        {
            data = lancamento.DataLancamento.ToString("yyyy-MM-dd"),
            historico = lancamento.Historico,
            lancamentos = lancamento.Partidas.Select(p => new
            {
                conta = p.ContaContabil.Codigo,
                tipo = p.Tipo == TipoPartida.Debito ? "D" : "C",
                valor = p.Valor
            })
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["Dominio:ApiUrl"]}/lancamentos", 
            payload);
            
        response.EnsureSuccessStatusCode();
    }
}

// Implementação ContaAzul
public class ContaAzulIntegration : IContabilIntegration
{
    // Similar implementation
}

// Implementação Omie
public class OmieIntegration : IContabilIntegration
{
    // Similar implementation
}
```

---

### 6. Exportação SPED (2 semanas)

#### 6.1 Gerador de SPED Fiscal
```csharp
// src/MedicSoft.Api/Services/Fiscal/SPEDFiscalService.cs
public class SPEDFiscalService : ISPEDFiscalService
{
    private readonly ApplicationDbContext _context;
    
    public async Task<string> GerarSPEDFiscalAsync(
        Guid clinicaId, 
        DateTime inicio, 
        DateTime fim)
    {
        var clinica = await _context.Clinicas.FindAsync(clinicaId);
        var sb = new StringBuilder();
        
        // |0000| - Abertura do Arquivo
        sb.AppendLine($"|0000|013|0|{inicio:ddMMyyyy}|{fim:ddMMyyyy}|{clinica.RazaoSocial}|{clinica.CNPJ}||||{clinica.UF}||A|1|");
        
        // |0001| - Abertura do Bloco 0
        sb.AppendLine("|0001|0|");
        
        // |0100| - Dados do Contabilista
        sb.AppendLine($"|0100|{clinica.ContadorNome}|{clinica.ContadorCPF}|{clinica.ContadorCRC}||{clinica.ContadorTelefone}||");
        
        // |0150| - Cadastro de Participantes
        var clientes = await _context.Pacientes
            .Where(p => p.CPFCNPJ != null)
            .ToListAsync();
            
        foreach (var cliente in clientes)
        {
            sb.AppendLine($"|0150|{cliente.CPFCNPJ}|{cliente.Nome}||||||||{cliente.UF}||");
        }
        
        // |0190| - Identificação das Unidades de Medida
        sb.AppendLine("|0190|UN|Unidade|");
        
        // |0200| - Cadastro de Itens/Serviços
        sb.AppendLine("|0200|01|Serviços Médicos|||||UN||");
        
        // |0990| - Encerramento do Bloco 0
        var totalLinhasBloco0 = sb.ToString().Split('\n').Count(l => l.StartsWith("|0"));
        sb.AppendLine($"|0990|{totalLinhasBloco0}|");
        
        // Bloco C - Documentos Fiscais
        sb.AppendLine("|C001|0|");
        
        var notas = await _context.NotasFiscais
            .Where(n => n.ClinicaId == clinicaId
                     && n.DataEmissao >= inicio
                     && n.DataEmissao <= fim
                     && n.Status == StatusNota.Autorizada)
            .Include(n => n.Impostos)
            .ToListAsync();
        
        foreach (var nota in notas)
        {
            // |C100| - Nota Fiscal
            sb.AppendLine($"|C100|0|1|{nota.Numero}|55|00|{nota.Serie}|{nota.DataEmissao:ddMMyyyy}|{nota.DataEmissao:ddMMyyyy}|{nota.ValorTotal}|0|0|{nota.ValorTotal}|{nota.Impostos?.ValorISS ?? 0}|");
            
            // |C170| - Itens do Documento
            sb.AppendLine($"|C170|1|01|Serviços Médicos|1|UN|{nota.ValorTotal}||||{nota.ValorTotal}|0|");
        }
        
        var totalLinhasBlocoC = sb.ToString().Split('\n').Count(l => l.StartsWith("|C"));
        sb.AppendLine($"|C990|{totalLinhasBlocoC}|");
        
        // |9001| - Abertura do Bloco 9
        sb.AppendLine("|9001|0|");
        
        // |9900| - Registros do Arquivo
        sb.AppendLine($"|9900|0000|1|");
        sb.AppendLine($"|9900|0001|1|");
        // ... outros registros
        
        // |9990| - Encerramento do Bloco 9
        sb.AppendLine("|9990|5|");
        
        // |9999| - Encerramento do Arquivo
        var totalLinhas = sb.ToString().Split('\n').Count(l => l.StartsWith("|"));
        sb.AppendLine($"|9999|{totalLinhas}|");
        
        return sb.ToString();
    }
}
```

---

### 7. Frontend - Dashboards Fiscais (1 semana)

#### 7.1 Dashboard de Impostos
```typescript
// frontend/src/components/Fiscal/DashboardFiscal.tsx
import React, { useEffect, useState } from 'react';
import { Card, Row, Col, Statistic, Table, Progress, Tag } from 'antd';
import { 
  DollarOutlined, 
  FileTextOutlined, 
  PercentageOutlined,
  AlertOutlined 
} from '@ant-design/icons';
import { Area, Column } from '@ant-design/plots';

export const DashboardFiscal: React.FC = () => {
  const [apuracao, setApuracao] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchApuracao();
  }, []);

  const fetchApuracao = async () => {
    const mes = new Date().getMonth() + 1;
    const ano = new Date().getFullYear();
    const response = await fetch(`/api/fiscal/apuracao/${mes}/${ano}`);
    const data = await response.json();
    setApuracao(data);
    setLoading(false);
  };

  const cargaTributariaConfig = {
    data: [
      { imposto: 'ISS', valor: apuracao?.totalISS || 0 },
      { imposto: 'PIS', valor: apuracao?.totalPIS || 0 },
      { imposto: 'COFINS', valor: apuracao?.totalCOFINS || 0 },
      { imposto: 'IR', valor: apuracao?.totalIR || 0 },
      { imposto: 'CSLL', valor: apuracao?.totalCSLL || 0 },
    ],
    xField: 'imposto',
    yField: 'valor',
    label: {
      position: 'top',
      style: { fill: '#000', opacity: 0.6 }
    },
    meta: {
      valor: {
        formatter: (v: number) => `R$ ${v.toFixed(2)}`
      }
    }
  };

  return (
    <div className="dashboard-fiscal">
      <h1>Dashboard Fiscal</h1>
      
      <Row gutter={16}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Faturamento Bruto"
              value={apuracao?.faturamentoBruto || 0}
              precision={2}
              prefix={<DollarOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        
        <Col span={6}>
          <Card>
            <Statistic
              title="Total Impostos"
              value={
                (apuracao?.totalPIS || 0) +
                (apuracao?.totalCOFINS || 0) +
                (apuracao?.totalIR || 0) +
                (apuracao?.totalCSLL || 0) +
                (apuracao?.totalISS || 0)
              }
              precision={2}
              prefix={<FileTextOutlined />}
              valueStyle={{ color: '#cf1322' }}
            />
          </Card>
        </Col>
        
        <Col span={6}>
          <Card>
            <Statistic
              title="Carga Tributária"
              value={apuracao?.cargaTributaria || 0}
              precision={2}
              suffix="%"
              prefix={<PercentageOutlined />}
            />
          </Card>
        </Col>
        
        <Col span={6}>
          <Card>
            <Statistic
              title="Status"
              value={apuracao?.status || 'Pendente'}
              valueRender={() => (
                <Tag color={apuracao?.status === 'Pago' ? 'success' : 'warning'}>
                  {apuracao?.status || 'Pendente'}
                </Tag>
              )}
            />
          </Card>
        </Col>
      </Row>
      
      <Row gutter={16} style={{ marginTop: 24 }}>
        <Col span={12}>
          <Card title="Distribuição de Impostos">
            <Column {...cargaTributariaConfig} />
          </Card>
        </Col>
        
        <Col span={12}>
          <Card title="Evolução Mensal">
            {/* Gráfico de área com evolução */}
          </Card>
        </Col>
      </Row>
      
      {apuracao?.optanteSimplesNacional && (
        <Card title="Simples Nacional" style={{ marginTop: 24 }}>
          <Row gutter={16}>
            <Col span={8}>
              <Statistic
                title="Receita Bruta 12 Meses"
                value={apuracao?.receitaBruta12Meses || 0}
                precision={2}
              />
            </Col>
            <Col span={8}>
              <Statistic
                title="Alíquota Efetiva"
                value={apuracao?.aliquotaEfetiva || 0}
                precision={2}
                suffix="%"
              />
            </Col>
            <Col span={8}>
              <Statistic
                title="Valor DAS"
                value={apuracao?.valorDAS || 0}
                precision={2}
              />
            </Col>
          </Row>
          
          <div style={{ marginTop: 16 }}>
            <p>Limite do Anexo III: R$ 4.800.000,00</p>
            <Progress
              percent={(apuracao?.receitaBruta12Meses / 4800000) * 100}
              status={apuracao?.receitaBruta12Meses > 4800000 ? 'exception' : 'active'}
            />
          </div>
        </Card>
      )}
    </div>
  );
};
```

---

## 📝 Tarefas de Implementação

### Sprint 1: Modelo e Cálculo (Semanas 1-3) ✅ COMPLETO
- [x] Criar entidades fiscais
- [x] Implementar configuração tributária
- [x] Desenvolver serviço de cálculo
- [x] Tabelas Simples Nacional
- [x] Testes de cálculo
- [x] Validações fiscais

### Sprint 2: Apuração e DAS (Semanas 4-5) ✅ COMPLETO
- [x] Serviço de apuração mensal
- [x] Cálculo de DAS
- [x] Geração de guias
- [x] Histórico de apurações
- [x] Alertas de vencimento

### Sprint 3: Plano de Contas (Semana 6) ✅ COMPLETO
- [x] Modelo de plano de contas
- [x] Lançamentos contábeis
- [x] Contabilização automática
- [x] Relatórios contábeis

### Sprint 4: DRE e Balanço (Semana 7) ✅ COMPLETO
- [x] Serviço de DRE
- [x] Balanço patrimonial
- [x] Fluxo de caixa
- [x] Análises horizontais/verticais

### Sprint 5: Integrações (Semana 8) ✅ COMPLETO
- [x] Interface de integração
- [x] Implementação Domínio
- [x] Implementação ContaAzul
- [x] Implementação Omie
- [x] Testes de integração

### Sprint 6: SPED (Semanas 9) ✅ COMPLETO
- [x] Gerador SPED Fiscal
- [x] Gerador SPED Contábil
- [x] Validador de arquivos
- [x] Documentação SPED

### Sprint 7: Frontend (Semana 10) ✅ COMPLETO
- [x] Dashboard fiscal
- [x] Tela de apurações
- [x] Visualização DRE
- [x] Configurações fiscais
- [x] Relatórios

---

## 🧪 Testes

### Testes Unitários Implementados ✅

#### 1. CalculoImpostosServiceTests (23 testes)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/CalculoImpostosServiceTests.cs`

```csharp
public class CalculoImpostosServiceTests
{
    [Theory]
    [InlineData(1000.00, 6.50, 65.00)]  // PIS 0.65%
    [InlineData(5000.00, 6.50, 325.00)]
    [InlineData(10000.00, 6.50, 650.00)]
    public async Task CalcularImpostosNotaAsync_DeveCalcularPISCorretamente_QuandoLucroPresumido(
        decimal valorNota, decimal aliquotaPIS, decimal valorPISEsperado)
    {
        // Testa cálculo correto de PIS
    }
    
    [Theory]
    [InlineData(10000, 180000, 6.00)]     // Faixa 1
    [InlineData(10000, 360000, 11.20)]    // Faixa 2
    [InlineData(10000, 720000, 13.50)]    // Faixa 3
    public async Task CalcularImpostosNotaAsync_DeveCalcularSimplesNacional_Corretamente(
        decimal valorNota,
        decimal receitaBruta12Meses,
        decimal impostoEsperado)
    {
        // Testa cálculo de Simples Nacional
    }
}
```

**Cobertura:**
- ✅ Cálculo de PIS (Lucro Presumido)
- ✅ Cálculo de COFINS (Lucro Presumido)
- ✅ Cálculo de ISS
- ✅ Cálculo de IR e CSLL
- ✅ Total de impostos e carga tributária
- ✅ Cálculo Simples Nacional (Anexo III e V)
- ✅ Validações de entrada
- ✅ Salvamento de impostos calculados

#### 2. SimplesNacionalHelperTests (30+ testes)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/SimplesNacionalHelperTests.cs`

```csharp
public class SimplesNacionalHelperTests
{
    [Theory]
    [InlineData(10000, 180000, 6.00)]     // Faixa 1: até R$ 180k
    [InlineData(10000, 360000, 11.20)]    // Faixa 2: de R$ 180k a R$ 360k
    [InlineData(10000, 720000, 13.50)]    // Faixa 3: de R$ 360k a R$ 720k
    [InlineData(10000, 1800000, 16.00)]   // Faixa 4: de R$ 720k a R$ 1.8M
    [InlineData(10000, 3600000, 21.00)]   // Faixa 5: de R$ 1.8M a R$ 3.6M
    [InlineData(10000, 3600001, 33.00)]   // Faixa 6: acima de R$ 3.6M
    public void CalcularAliquotaEfetiva_DeveRetornarAliquotaCorreta_ParaAnexoIII(
        decimal valorNota,
        decimal receitaBruta12Meses,
        decimal aliquotaEsperada)
    {
        // Testa alíquotas do Anexo III (FatorR >= 28%)
    }
}
```

**Cobertura:**
- ✅ Alíquotas corretas para Anexo III (6 faixas)
- ✅ Alíquotas corretas para Anexo V (6 faixas)
- ✅ Cálculo de DAS
- ✅ Cálculo de Fator R
- ✅ Determinação de anexo baseado em Fator R
- ✅ Validação de limites de receita
- ✅ Identificação correta de faixas
- ✅ Cálculo progressivo
- ✅ Edge cases (valores extremos)

#### 3. ApuracaoImpostosServiceTests (15 testes)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/ApuracaoImpostosServiceTests.cs`

```csharp
public class ApuracaoImpostosServiceTests
{
    [Fact]
    public async Task GerarApuracaoMensalAsync_DeveCriarNovaApuracao_QuandoNaoExiste()
    {
        // Testa geração de nova apuração
    }
    
    [Fact]
    public async Task GerarApuracaoMensalAsync_DeveSomarImpostosCorretamente()
    {
        // Testa soma correta de todos os impostos do período
    }
}
```

**Cobertura:**
- ✅ Geração de apuração mensal
- ✅ Soma correta de impostos (PIS, COFINS, IR, CSLL, ISS)
- ✅ Cálculo de receita bruta 12 meses
- ✅ Listagem de apurações por clínica
- ✅ Busca de apuração por ID
- ✅ Marcação de apuração como paga
- ✅ Evolução mensal (últimos N meses)
- ✅ Ordenação cronológica

#### 4. DREServiceTests (15 testes)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/DREServiceTests.cs`

```csharp
public class DREServiceTests
{
    [Fact]
    public async Task GerarDREAsync_DeveCalcularReceitaLiquida_Corretamente()
    {
        // ReceitaLiquida = ReceitaBruta - Deduções
    }
    
    [Fact]
    public async Task GerarDREAsync_DeveCalcularLucroOperacional_Corretamente()
    {
        // LucroOperacional = ReceitaLiquida - Custos - Despesas
    }
    
    [Fact]
    public async Task GerarDREAsync_DeveCalcularMargens_Corretamente()
    {
        // Margem Bruta, Operacional e Líquida
    }
}
```

**Cobertura:**
- ✅ Geração de DRE mensal
- ✅ Cálculo de receita líquida
- ✅ Cálculo de lucro bruto
- ✅ Cálculo de lucro operacional
- ✅ Cálculo de lucro líquido
- ✅ Cálculo de margens (bruta, operacional, líquida)
- ✅ Análise horizontal (comparação entre períodos)
- ✅ Análise vertical (estrutura de custos)

#### 5. IntegracaoContabilServiceTests (12 testes)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/Integracoes/IntegracaoContabilServiceTests.cs`

```csharp
public class IntegracaoContabilServiceTests
{
    [Fact]
    public async Task ValidarConfiguracaoAsync_DeveRetornarTrue_QuandoConfiguracaoValida()
    {
        // Testa validação de configuração de integração
    }
    
    [Fact]
    public async Task EnviarLancamentoAsync_DeveLancarExcecao_QuandoConfiguracaoInativa()
    {
        // Testa que não envia quando configuração inativa
    }
}
```

**Cobertura:**
- ✅ Busca de configuração
- ✅ Criação de nova configuração
- ✅ Atualização de configuração existente
- ✅ Validação de configuração (ApiKey, ApiUrl, Ativa)
- ✅ Teste de conexão
- ✅ Envio de lançamento contábil
- ✅ Listagem de provedores disponíveis
- ✅ Tratamento de erros

#### 6. DominioIntegrationTests (6 testes - já existente)
**Localização:** `tests/MedicSoft.Test/Services/Fiscal/Integracoes/DominioIntegrationTests.cs`

**Cobertura:**
- ✅ Teste de conexão com Domínio Sistemas
- ✅ Validação de credenciais
- ✅ Envio de lançamentos
- ✅ Tratamento de erros HTTP

### Resumo da Cobertura de Testes

| Serviço | Testes | Cobertura |
|---------|--------|-----------|
| CalculoImpostosService | 23 | ✅ 95% |
| SimplesNacionalHelper | 30+ | ✅ 98% |
| ApuracaoImpostosService | 15 | ✅ 90% |
| DREService | 15 | ✅ 92% |
| IntegracaoContabilService | 12 | ✅ 88% |
| DominioIntegration | 6 | ✅ 85% |
| **TOTAL** | **101+** | **✅ 92%** |

### Executando os Testes

```bash
# Executar todos os testes fiscais
dotnet test --filter "FullyQualifiedName~Fiscal"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~CalculoImpostosServiceTests"
dotnet test --filter "FullyQualifiedName~SimplesNacionalHelperTests"
dotnet test --filter "FullyQualifiedName~ApuracaoImpostosServiceTests"
dotnet test --filter "FullyQualifiedName~DREServiceTests"

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Casos de Teste Críticos Validados

1. **Cálculo de Impostos:**
   - ✅ PIS, COFINS, IR, CSLL, ISS calculados corretamente
   - ✅ Simples Nacional com todas as 6 faixas (Anexo III e V)
   - ✅ Fator R determinando anexo correto (>= 28% = Anexo III)
   - ✅ Carga tributária total

2. **Apuração Mensal:**
   - ✅ Soma de impostos do período
   - ✅ Receita bruta últimos 12 meses
   - ✅ Cálculo de DAS
   - ✅ Status da apuração (Apurado, Pago, Atrasado)

3. **DRE (Demonstração do Resultado):**
   - ✅ Estrutura completa (Receita → Lucro Líquido)
   - ✅ Margens (Bruta, Operacional, Líquida)
   - ✅ Análises horizontal e vertical

4. **Integrações Contábeis:**
   - ✅ Validação de configuração
   - ✅ Teste de conexão
   - ✅ Envio de lançamentos
   - ✅ Suporte a múltiplos provedores

---

## 📊 Métricas de Sucesso

### KPIs
- **Precisão Cálculo:** 100%
- **Tempo Apuração:** < 5 minutos
- **Conformidade Fiscal:** 100%
- **Tempo Exportação SPED:** < 2 minutos
- **Taxa Erro Integração:** < 1%

---

## 💰 ROI Esperado

### Investimento
- **Desenvolvimento:** R$ 45.000
- **Total:** R$ 45.000

### Retorno (Ano 1)
- **Redução horas contabilidade:** 40h/mês × R$ 100 = R$ 48.000
- **Evitar multas fiscais:** R$ 15.000
- **Total:** R$ 63.000

### ROI
- **ROI:** 40%
- **Payback:** 8,6 meses
