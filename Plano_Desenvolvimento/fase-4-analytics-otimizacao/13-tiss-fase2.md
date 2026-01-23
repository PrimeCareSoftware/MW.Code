# 📋 Prompt 13: Integração TISS - Fase 2 (Webservices + Gestão de Glosas)

**Prioridade:** 🔥 P2 - Médio  
**Complexidade:** ⚡⚡⚡ Alta  
**Tempo Estimado:** 3 meses | 2-3 desenvolvedores  
**Custo:** R$ 135.000  
**Pré-requisitos:** TISS Fase 1 completa (Prompt 06)

---

## 🎯 Objetivo

Completar a integração TISS com webservices de operadoras, implementar sistema automático de gestão de glosas, e criar dashboards analíticos para performance das operadoras.

---

## 📊 Contexto do Sistema

### O que já existe (Fase 1)
- ✅ Estrutura de dados TISS (tabelas, guias)
- ✅ Geração de XML TISS conforme padrão ANS
- ✅ Envio manual de lotes
- ✅ Cadastro de convênios e planos

### O que precisa ser desenvolvido (Fase 2)
- 🔨 Webservices de comunicação com operadoras
- 🔨 Conferência automática de glosas
- 🔨 Sistema de recurso de glosa
- 🔨 Dashboards de performance por operadora
- 🔨 Análise histórica e relatórios

---

## 🏗️ Arquitetura da Solução

### 1. Camada de Webservices (6 semanas)

#### 1.1 Interface de Comunicação
```csharp
// src/MedicSoft.Api/Services/TISS/ITissWebServiceClient.cs
public interface ITissWebServiceClient
{
    Task<string> EnviarLoteAsync(Guid loteId);
    Task<TissRetornoLote> ConsultarLoteAsync(string protocoloOperadora);
    Task<TissRetornoGuia> ConsultarGuiaAsync(string numeroGuia);
    Task<bool> CancelarGuiaAsync(string numeroGuia, string motivo);
    Task<TissRetornoRecurso> EnviarRecursoAsync(Guid recursoId);
}

// Implementações por operadora
public class UnimeWebServiceClient : ITissWebServiceClient { }
public class SulamericaWebServiceClient : ITissWebServiceClient { }
public class BradescoSaudeWebServiceClient : ITissWebServiceClient { }
// Factory para escolher implementação baseado na operadora
```

#### 1.2 Configuração de Operadoras
```csharp
// src/MedicSoft.Core/Entities/TISS/TissOperadoraConfig.cs
public class TissOperadoraConfig
{
    public Guid Id { get; set; }
    public Guid ConvenioId { get; set; }
    public Convenio Convenio { get; set; }
    
    // Webservice config
    public string WebServiceUrl { get; set; }
    public string Usuario { get; set; }
    public string SenhaEncriptada { get; set; }
    public string CertificadoDigitalPath { get; set; } // A1/A3
    
    // Configurações específicas
    public int TimeoutSegundos { get; set; } = 120;
    public int TentativasReenvio { get; set; } = 3;
    public bool UsaSoapHeader { get; set; }
    public bool UsaCertificadoDigital { get; set; }
    
    // Mapeamento de códigos específicos
    public Dictionary<string, string> MapeamentoTabelas { get; set; }
}
```

#### 1.3 Retry Policy e Resiliência
```csharp
// src/MedicSoft.Api/Services/TISS/TissWebServiceClient.cs
public class TissWebServiceClient
{
    private readonly ILogger<TissWebServiceClient> _logger;
    
    public async Task<T> ExecutarComRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3)
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retry, ctx) =>
                {
                    _logger.LogWarning(
                        "Tentativa {Retry} após {Delay}s. Erro: {Error}",
                        retry, timeSpan.TotalSeconds, exception.Message);
                });
        
        return await policy.ExecuteAsync(operation);
    }
}
```

---

### 2. Sistema de Gestão de Glosas (4 semanas)

#### 2.1 Entidade Glosa
```csharp
// src/MedicSoft.Core/Entities/TISS/TissGlosa.cs
public class TissGlosa
{
    public Guid Id { get; set; }
    public Guid GuiaId { get; set; }
    public TissGuia Guia { get; set; }
    
    public string NumeroGuia { get; set; }
    public DateTime DataGlosa { get; set; }
    public DateTime DataIdentificacao { get; set; }
    
    // Dados da glosa
    public TipoGlosa Tipo { get; set; } // Administrativa, Técnica, Financeira
    public string CodigoGlosa { get; set; } // Código da operadora
    public string DescricaoGlosa { get; set; }
    public decimal ValorGlosado { get; set; }
    public decimal ValorOriginal { get; set; }
    
    // Item específico glosado
    public int? SequenciaItem { get; set; }
    public string CodigoProcedimento { get; set; }
    public string NomeProcedimento { get; set; }
    
    // Status
    public StatusGlosa Status { get; set; }
    public string JustificativaRecurso { get; set; }
    public List<TissRecursoGlosa> Recursos { get; set; }
}

public enum TipoGlosa
{
    Administrativa = 1, // Dados incorretos, ausência de documentos
    Tecnica = 2,        // Procedimento não autorizado, incompatível
    Financeira = 3      // Valores divergentes
}

public enum StatusGlosa
{
    Nova = 1,
    EmAnalise = 2,
    RecursoEnviado = 3,
    RecursoDeferido = 4,
    RecursoIndeferido = 5,
    Acatada = 6
}
```

#### 2.2 Detecção Automática de Glosas
```csharp
// src/MedicSoft.Api/Services/TISS/GlosaDetectionService.cs
public class GlosaDetectionService
{
    public async Task ProcessarRetornoLoteAsync(Guid loteId, XDocument xmlRetorno)
    {
        // Parse do XML de retorno
        var glosas = ExtractGlosasFromXml(xmlRetorno);
        
        foreach (var glosaDto in glosas)
        {
            // Busca guia correspondente
            var guia = await _guiaRepository.GetByNumeroAsync(glosaDto.NumeroGuia);
            
            // Cria registro de glosa
            var glosa = new TissGlosa
            {
                GuiaId = guia.Id,
                NumeroGuia = glosaDto.NumeroGuia,
                DataGlosa = glosaDto.DataGlosa,
                DataIdentificacao = DateTime.Now,
                Tipo = ClassificarTipoGlosa(glosaDto.CodigoGlosa),
                CodigoGlosa = glosaDto.CodigoGlosa,
                DescricaoGlosa = glosaDto.Descricao,
                ValorGlosado = glosaDto.ValorGlosado,
                ValorOriginal = guia.ValorTotal,
                Status = StatusGlosa.Nova
            };
            
            await _glosaRepository.AddAsync(glosa);
            
            // Notifica responsável
            await _notificationService.NotificarGlosaAsync(glosa);
            
            // Analisa se recurso automático é possível
            await AnalisarPossibilidadeRecursoAsync(glosa);
        }
    }
    
    private TipoGlosa ClassificarTipoGlosa(string codigo)
    {
        // Mapeamento de códigos ANS
        return codigo.StartsWith("A") ? TipoGlosa.Administrativa :
               codigo.StartsWith("T") ? TipoGlosa.Tecnica :
               TipoGlosa.Financeira;
    }
}
```

#### 2.3 Sistema de Recursos
```csharp
// src/MedicSoft.Core/Entities/TISS/TissRecursoGlosa.cs
public class TissRecursoGlosa
{
    public Guid Id { get; set; }
    public Guid GlosaId { get; set; }
    public TissGlosa Glosa { get; set; }
    
    public DateTime DataEnvio { get; set; }
    public string Justificativa { get; set; }
    public List<DocumentoAnexo> Anexos { get; set; }
    
    // Resposta da operadora
    public DateTime? DataResposta { get; set; }
    public ResultadoRecurso? Resultado { get; set; }
    public string JustificativaOperadora { get; set; }
    public decimal? ValorDeferido { get; set; }
}

public enum ResultadoRecurso
{
    Deferido = 1,      // Glosa revertida
    Parcial = 2,       // Glosa parcialmente revertida
    Indeferido = 3     // Glosa mantida
}
```

---

### 3. Dashboards de Performance (3 semanas)

#### 3.1 Dashboard Principal
```typescript
// frontend/src/app/features/tiss/pages/dashboard-tiss/dashboard-tiss.component.ts
interface DashboardTissData {
  periodo: { inicio: Date; fim: Date };
  
  // Métricas gerais
  totalGuiasEnviadas: number;
  totalGuiasAprovadas: number;
  totalGuiasGlosadas: number;
  taxaGlosa: number; // %
  
  valorTotalFaturado: number;
  valorTotalGlosado: number;
  valorTotalRecebido: number;
  
  // Por operadora
  performancePorOperadora: OperadoraPerformance[];
  
  // Glosas
  glosasPorTipo: { tipo: string; count: number; valor: number }[];
  glosasPorProcedimento: { procedimento: string; count: number }[];
  glosasMaisFrequentes: { codigo: string; descricao: string; count: number }[];
  
  // Recursos
  recursosEnviados: number;
  recursosDeferidos: number;
  recursosIndeferidos: number;
  taxaSucessoRecursos: number; // %
  
  // Tendências
  tendenciaGlosas: { mes: string; taxa: number }[];
  tendenciaValores: { mes: string; faturado: number; recebido: number }[];
}

interface OperadoraPerformance {
  convenioId: string;
  nomeOperadora: string;
  
  guiasEnviadas: number;
  guiasAprovadas: number;
  taxaAprovacao: number;
  
  valorFaturado: number;
  valorGlosado: number;
  valorRecebido: number;
  taxaGlosa: number;
  
  tempoMedioRetorno: number; // dias
  
  ultimoEnvio: Date;
}
```

#### 3.2 Relatórios Analíticos
```csharp
// src/MedicSoft.Api/Services/TISS/TissAnalyticsService.cs
public class TissAnalyticsService
{
    // Relatório de performance mensal
    public async Task<RelatorioPerformance> GerarRelatorioMensalAsync(
        int mes, int ano, Guid? convenioId = null)
    {
        var guias = await _guiaRepository.GetByPeriodoAsync(
            new DateTime(ano, mes, 1),
            new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)));
        
        if (convenioId.HasValue)
            guias = guias.Where(g => g.ConvenioId == convenioId.Value);
        
        return new RelatorioPerformance
        {
            Periodo = $"{mes:00}/{ano}",
            TotalGuias = guias.Count(),
            ValorFaturado = guias.Sum(g => g.ValorTotal),
            ValorGlosado = CalcularGlosasDoMes(guias),
            TaxaGlosa = CalcularTaxaGlosa(guias),
            ProcedimentosMaisGlosados = GetProcedimentosMaisGlosados(guias)
        };
    }
    
    // Análise de tendências
    public async Task<TendenciaGlosas> AnalisarTendenciasAsync(int meses = 12)
    {
        var dados = new List<DadosMes>();
        
        for (int i = meses - 1; i >= 0; i--)
        {
            var data = DateTime.Now.AddMonths(-i);
            var performance = await GerarRelatorioMensalAsync(data.Month, data.Year);
            
            dados.Add(new DadosMes
            {
                Mes = data.ToString("MMM/yyyy"),
                TaxaGlosa = performance.TaxaGlosa,
                ValorGlosado = performance.ValorGlosado
            });
        }
        
        return new TendenciaGlosas
        {
            Dados = dados,
            MediaTaxaGlosa = dados.Average(d => d.TaxaGlosa),
            Tendencia = CalcularTendencia(dados)
        };
    }
}
```

---

### 4. Notificações e Alertas (1 semana)

```csharp
// src/MedicSoft.Api/Services/TISS/TissNotificationService.cs
public class TissNotificationService
{
    public async Task NotificarGlosaAsync(TissGlosa glosa)
    {
        var guia = await _guiaRepository.GetByIdAsync(glosa.GuiaId);
        var convenio = guia.Convenio;
        
        // Notifica gestor financeiro
        await _emailService.SendEmailAsync(
            to: "financeiro@clinica.com.br",
            subject: $"Nova Glosa - {convenio.Nome} - R$ {glosa.ValorGlosado:N2}",
            body: $@"
                <h3>Nova Glosa Identificada</h3>
                <p><strong>Operadora:</strong> {convenio.Nome}</p>
                <p><strong>Guia:</strong> {glosa.NumeroGuia}</p>
                <p><strong>Tipo:</strong> {glosa.Tipo}</p>
                <p><strong>Valor:</strong> R$ {glosa.ValorGlosado:N2}</p>
                <p><strong>Motivo:</strong> {glosa.DescricaoGlosa}</p>
                <p><a href='https://sistema.clinica.com/tiss/glosas/{glosa.Id}'>
                    Ver Detalhes e Entrar com Recurso
                </a></p>
            ");
        
        // Se taxa de glosa da operadora está alta, alerta direção
        var taxaGlosa = await CalcularTaxaGlosaOperadoraAsync(convenio.Id);
        if (taxaGlosa > 15) // mais de 15%
        {
            await NotificarDirecaoTaxaAltaAsync(convenio, taxaGlosa);
        }
    }
    
    public async Task AlertarPrazoRecursoAsync()
    {
        // Job executado diariamente
        var glosasProximasPrazo = await _glosaRepository
            .GetGlosasSemRecursoAsync()
            .Where(g => (DateTime.Now - g.DataGlosa).TotalDays > 25) // prazo 30 dias
            .ToListAsync();
        
        foreach (var glosa in glosasProximasPrazo)
        {
            await _emailService.SendEmailAsync(
                to: "financeiro@clinica.com.br",
                subject: $"URGENTE: Prazo de Recurso - Glosa {glosa.NumeroGuia}",
                body: "Recurso deve ser enviado em até 5 dias!");
        }
    }
}
```

---

## 📝 Tarefas de Implementação

### Sprint 1: Webservices (Semanas 1-6)
- [ ] Criar interface `ITissWebServiceClient`
- [ ] Implementar cliente genérico com SOAP
- [ ] Implementar clientes específicos (Unimed, SulAmérica, Bradesco)
- [ ] Configurar retry policy e timeout
- [ ] Testar com ambiente homologação operadoras
- [ ] Implementar envio automático de lotes
- [ ] Criar job de consulta de retornos

### Sprint 2: Gestão de Glosas (Semanas 7-10)
- [ ] Criar entidades `TissGlosa` e `TissRecursoGlosa`
- [ ] Implementar detecção automática de glosas
- [ ] Criar tela de listagem de glosas
- [ ] Criar tela de recurso de glosa
- [ ] Implementar envio automático de recursos
- [ ] Sistema de anexos (documentos comprobatórios)

### Sprint 3: Dashboards (Semanas 11-13)
- [ ] Criar serviço de analytics
- [ ] Implementar dashboard principal
- [ ] Gráficos de tendências
- [ ] Relatórios por operadora
- [ ] Exportar relatórios (Excel, PDF)

### Sprint 4: Notificações e Finalização (Semanas 14-15)
- [ ] Sistema de notificações de glosas
- [ ] Alertas de prazo de recurso
- [ ] Configurações por operadora
- [ ] Testes integrados
- [ ] Documentação
- [ ] Deploy produção

---

## 🧪 Testes

### Testes Unitários
```csharp
public class GlosaDetectionServiceTests
{
    [Fact]
    public async Task DeveDetectarGlosasCorretamente()
    {
        // Arrange
        var xmlRetorno = CarregarXmlRetornoComGlosas();
        
        // Act
        await _service.ProcessarRetornoLoteAsync(loteId, xmlRetorno);
        
        // Assert
        var glosas = await _glosaRepository.GetByLoteIdAsync(loteId);
        Assert.Equal(3, glosas.Count());
        Assert.Equal(TipoGlosa.Administrativa, glosas.First().Tipo);
    }
}
```

### Testes de Integração
- Testar envio de lote para operadora (homologação)
- Testar consulta de retorno
- Testar envio de recurso
- Validar parsing de XML de retorno

---

## 📊 Métricas de Sucesso

- ✅ 95%+ das operadoras integradas via webservice
- ✅ Detecção automática de 100% das glosas
- ✅ Taxa de sucesso em recursos > 40%
- ✅ Tempo de identificação de glosa < 24h
- ✅ Dashboards atualizados em tempo real

---

## 🚀 Deploy e Rollout

### Fase 1: Operadora Piloto (1 semana)
- Integrar com 1 operadora (a mais usada)
- Testar em produção com volume real
- Ajustar configurações

### Fase 2: Expansão (2 semanas)
- Integrar demais operadoras
- Treinar equipe financeira
- Criar documentação

### Fase 3: Otimização (contínuo)
- Monitorar performance
- Ajustar timeouts e retries
- Adicionar novas operadoras conforme necessário

---

## 📚 Documentação Necessária

1. **Manual de Configuração de Operadoras**
   - Como cadastrar credenciais webservice
   - Certificados digitais A1/A3
   - Mapeamento de tabelas específicas

2. **Guia de Gestão de Glosas**
   - Como analisar uma glosa
   - Como entrar com recurso
   - Documentos necessários por tipo

3. **Manual de Dashboards**
   - Como interpretar os gráficos
   - Ações recomendadas por cenário
   - Exportação de relatórios

---

## 💰 ROI Esperado

**Investimento:** R$ 135.000  
**Economia Anual:**
- Redução de 30% em glosas (melhor preparo): R$ 60.000/ano
- Sucesso em 40% dos recursos: R$ 40.000/ano
- Redução de 80% em tempo administrativo: R$ 50.000/ano
**Total:** R$ 150.000/ano

**Payback:** ~11 meses
