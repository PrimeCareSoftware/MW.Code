# 🤖 Machine Learning - Documentação Técnica

> **Status:** ✅ Framework Completo e Integrado ao Frontend  
> **Data:** Janeiro 2026  
> **Versão:** 2.0.0

## 📋 Visão Geral

Sistema de Machine Learning implementado com ML.NET para previsão inteligente de demanda de consultas e risco de no-show (falta) de pacientes. **Agora com integração completa ao frontend Angular**, permitindo visualização das previsões diretamente no Dashboard Clínico.

---

## 🏗️ Arquitetura

### Tecnologias
- **ML.NET 3.0.1** - Framework de Machine Learning da Microsoft
- **FastTree Algorithm** - Gradient Boosting para regressão e classificação
- **.NET 8** - Runtime e APIs
- **PostgreSQL** - Armazenamento de dados históricos

### Estrutura do Projeto

```
src/MedicSoft.ML/
├── Models/
│   ├── PrevisaoDemanda.cs       # Modelos de previsão de demanda
│   └── PrevisaoNoShow.cs        # Modelos de previsão de no-show
├── Services/
│   ├── PrevisaoDemandaService.cs    # Serviço de ML para demanda
│   └── PrevisaoNoShowService.cs     # Serviço de ML para no-show
└── Data/
    └── (modelos treinados salvos aqui)
```

---

## 🎯 Modelo 1: Previsão de Demanda

### Objetivo
Prever o número de consultas para os próximos dias, permitindo melhor planejamento de recursos.

### Features (Entrada)
| Feature | Tipo | Descrição | Valores |
|---------|------|-----------|---------|
| Mes | float | Mês do ano | 1-12 |
| DiaSemana | float | Dia da semana | 0-6 (0=Domingo) |
| Semana | float | Semana do ano | 1-52 |
| IsFeriado | float | É feriado? | 0 ou 1 |
| TemperaturaMedia | float | Temperatura média do dia | 15-35°C |

### Output
- **NumeroConsultas** (float): Número previsto de consultas para o dia

### Algoritmo
- **FastTree Regression**
  - `numberOfTrees: 100` - 100 árvores de decisão
  - `numberOfLeaves: 20` - Máx 20 folhas por árvore
  - `minimumExampleCountPerLeaf: 10` - Mín 10 exemplos por folha

### Métricas de Avaliação
- **R² (R-Squared)**: Percentual de variância explicada (target: > 0.75)
- **MAE (Mean Absolute Error)**: Erro médio absoluto
- **RMSE (Root Mean Squared Error)**: Raiz do erro quadrático médio

### Endpoints API

#### GET /api/MLPrediction/demanda/proxima-semana
Retorna previsão para os próximos 7 dias.

**Resposta:**
```json
{
  "periodo": "Próxima Semana",
  "totalPrevisto": 145,
  "previsoes": [
    {
      "data": "2026-01-28",
      "consultasPrevistas": 20,
      "confiancaPrevisao": 0.8
    },
    {
      "data": "2026-01-29",
      "consultasPrevistas": 22,
      "confiancaPrevisao": 0.8
    }
    // ... mais 5 dias
  ]
}
```

#### GET /api/MLPrediction/demanda/data?data=2026-02-01
Retorna previsão para uma data específica.

**Resposta:**
```json
{
  "data": "2026-02-01",
  "consultasPrevistas": 25
}
```

### Como Treinar

1. **Coletar dados históricos** (mínimo 30 dias, ideal 2+ anos):
```csharp
var dadosTreinamento = await _consolidadoRepository
    .GetUltimosMesesAsync(24) // 24 meses
    .Select(d => new DadosTreinamentoDemanda
    {
        Mes = d.Data.Month,
        DiaSemana = (int)d.Data.DayOfWeek,
        Semana = GetNumeroSemana(d.Data),
        IsFeriado = d.IsFeriado ? 1 : 0,
        TemperaturaMedia = d.TemperaturaMedia,
        NumeroConsultas = d.TotalConsultas
    });
```

2. **Treinar via API** (Admin):
```bash
POST /api/MLPrediction/admin/treinar/demanda
Content-Type: application/json

[
  {
    "mes": 1,
    "diaSemana": 1,
    "semana": 4,
    "isFeriado": 0,
    "temperaturaMedia": 28,
    "numeroConsultas": 25
  },
  // ... mais registros
]
```

3. **Carregar modelo treinado**:
```bash
POST /api/MLPrediction/admin/carregar-modelos
```

### Armazenamento
- Modelo salvo em: `/MLModels/modelo_demanda.zip`
- Formato: ML.NET ZIP (contém pipeline completo)
- Persistência: Disco local (considerar Azure Blob Storage em produção)

---

## 🚫 Modelo 2: Previsão de No-Show

### Objetivo
Calcular o risco (probabilidade) de um paciente faltar à consulta agendada, permitindo ações preventivas.

### Features (Entrada)
| Feature | Tipo | Descrição | Valores |
|---------|------|-----------|---------|
| IdadePaciente | float | Idade do paciente | 0-120 |
| DiasAteConsulta | float | Dias até a consulta | 0-365 |
| HoraDia | float | Hora da consulta | 0-23 |
| HistoricoNoShow | float | % de no-shows anteriores do paciente | 0-1 |
| TempoDesdeUltimaConsulta | float | Dias desde última consulta | 0-9999 |
| IsConvenio | float | Usa convênio? | 0 ou 1 |
| TemLembrete | float | Recebeu lembrete? | 0 ou 1 |

### Output
- **VaiComparecer** (bool): Previsão binária
- **Probability** (float): Probabilidade de comparecer (0-1)
- **RiscoNoShow** (float): 1 - Probability = risco de NÃO comparecer

### Algoritmo
- **FastTree Binary Classification**
  - `numberOfTrees: 100`
  - `numberOfLeaves: 20`
  - `minimumExampleCountPerLeaf: 10`

### Métricas de Avaliação
- **Accuracy**: Acurácia geral (target: > 0.70)
- **AUC (Area Under ROC Curve)**: Qualidade geral do classificador (target: > 0.80)
- **F1 Score**: Balanceamento entre precisão e recall

### Níveis de Risco
| Risco | Percentual | Ações Recomendadas |
|-------|------------|-------------------|
| 🟢 MUITO BAIXO | 0-30% | Lembrete padrão suficiente |
| 🟡 BAIXO | 30-50% | Enviar lembrete padrão 24h antes |
| 🟠 MÉDIO | 50-70% | Lembrete adicional por WhatsApp + confirmação 2h antes |
| 🔴 ALTO | 70-100% | Ligar para confirmar + oferecer reagendamento |

### Endpoints API

#### POST /api/MLPrediction/noshow/calcular-risco
Calcula risco de no-show para um agendamento.

**Request:**
```json
{
  "idadePaciente": 35,
  "diasAteConsulta": 3,
  "horaDia": 14,
  "historicoNoShow": 0.1,
  "tempoDesdeUltimaConsulta": 90,
  "isConvenio": 1,
  "temLembrete": 1
}
```

**Resposta:**
```json
{
  "riscoNoShow": 0.25,
  "riscoPercentual": 25.0,
  "nivel": "BAIXO",
  "acoesRecomendadas": [
    "🟢 BAIXO RISCO: Enviar lembrete padrão 24h antes"
  ]
}
```

### Como Treinar

1. **Coletar dados históricos de agendamentos**:
```csharp
var agendamentosHistorico = await _agendamentoRepository
    .GetAgendamentosFinalizadosAsync(DateTime.Now.AddYears(-2), DateTime.Now);

var dadosTreinamento = agendamentosHistorico.Select(a => new DadosNoShow
{
    IdadePaciente = CalcularIdade(a.Paciente.DataNascimento),
    DiasAteConsulta = (a.DataHora - a.DataCriacao).Days,
    HoraDia = a.DataHora.Hour,
    HistoricoNoShow = await CalcularTaxaNoShowPacienteAsync(a.PacienteId),
    TempoDesdeUltimaConsulta = await GetDiasDesdeUltimaConsultaAsync(a.PacienteId, a.DataHora),
    IsConvenio = a.ConvenioId.HasValue ? 1 : 0,
    TemLembrete = a.LembreteEnviado ? 1 : 0,
    VaiComparecer = a.Status == StatusAgendamento.Realizada
});
```

2. **Treinar via API**:
```bash
POST /api/MLPrediction/admin/treinar/noshow
Content-Type: application/json

[
  {
    "idadePaciente": 35,
    "diasAteConsulta": 7,
    "horaDia": 10,
    "historicoNoShow": 0.0,
    "tempoDesdeUltimaConsulta": 60,
    "isConvenio": 1,
    "temLembrete": 1,
    "vaiComparecer": true
  },
  // ... mais registros
]
```

### Armazenamento
- Modelo salvo em: `/MLModels/modelo_noshow.zip`

---

## 🧪 Testes e Validação

### Requisitos Mínimos de Dados
- **Previsão de Demanda**: Mínimo 30 dias, ideal 2+ anos
- **Previsão de No-Show**: Mínimo 100 agendamentos, ideal 1000+

### Pipeline de Teste

1. **Coleta de Dados**
```bash
# Exportar dados consolidados
GET /api/Analytics/dashboard/clinico?inicio=2024-01-01&fim=2026-01-31
```

2. **Treinar Modelos**
```bash
POST /api/MLPrediction/admin/treinar/demanda
POST /api/MLPrediction/admin/treinar/noshow
```

3. **Validar Métricas**
- Verificar logs para métricas de acurácia
- R² > 0.75 para demanda
- Accuracy > 0.70 para no-show

4. **Testar Previsões**
```bash
GET /api/MLPrediction/demanda/proxima-semana
POST /api/MLPrediction/noshow/calcular-risco
```

5. **Comparar com Realidade**
- Aguardar 7 dias
- Comparar previsões com dados reais
- Ajustar hiperparâmetros se necessário

---

## 🔧 Manutenção e Re-Treinamento

### Quando Re-Treinar?
- **Mensal**: Para manter modelos atualizados com tendências recentes
- **Após Mudanças**: Novos processos, pandemia, expansão da clínica
- **Queda de Performance**: Quando previsões divergem muito da realidade

### Automatização de Re-Treinamento
Considerar implementar job Hangfire mensal:

```csharp
RecurringJob.AddOrUpdate<MLTrainingJob>(
    "ml-retraining",
    job => job.RetrainModelsAsync(),
    Cron.Monthly(1, 2), // Dia 1 às 02:00
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
);
```

---

## 🚀 Roadmap Futuro

### Curto Prazo (1-2 meses)
- [ ] Integrar previsões nos dashboards frontend
- [ ] Criar job de re-treinamento automático
- [ ] Adicionar métricas de performance dos modelos
- [ ] Dashboard de acurácia dos modelos

### Médio Prazo (3-6 meses)
- [ ] Modelo de previsão de cancelamentos
- [ ] Modelo de churn de pacientes
- [ ] Modelo de previsão de receita
- [ ] AutoML para otimização automática de hiperparâmetros

### Longo Prazo (6+ meses)
- [ ] Migrar para Azure ML ou AWS SageMaker
- [ ] Implementar A/B testing de modelos
- [ ] Modelo de recomendação de horários
- [ ] Análise de sentimento em avaliações (NLP)

---

## 📊 Exemplos de Uso

### Scenario 1: Planejamento Semanal
```csharp
var previsao = await _demandaService.PreverProximaSemana();
Console.WriteLine($"Próxima semana: {previsao.TotalPrevisto} consultas previstas");

foreach (var dia in previsao.Previsoes)
{
    Console.WriteLine($"{dia.Data:dd/MM}: {dia.ConsultasPrevistas} consultas");
}
```

### Scenario 2: Identificar Agendamentos de Risco
```csharp
var agendamentosHoje = await _agendamentoRepository.GetByDataAsync(DateTime.Today);

foreach (var agendamento in agendamentosHoje)
{
    var dados = MontarDadosNoShow(agendamento);
    var risco = _noShowService.CalcularRiscoNoShow(dados);
    
    if (risco > 0.5) // Risco médio ou alto
    {
        var acoes = _noShowService.SugerirAcoes(risco);
        await EnviarAlertaParaRecepcionista(agendamento, risco, acoes);
    }
}
```

---

## 🔐 Segurança e Privacidade

### Thread-Safety
- ✅ **Serviços ML são Singleton mas thread-safe**
  - Lock mechanism implementado em operações de modelo
  - Proteção contra race conditions durante treinamento/predição
  - Garantia de consistência em ambiente multi-thread
  - Ver `CORREÇOES_PR425.md` para detalhes

### LGPD Compliance
- ✅ Dados anonimizados para treinamento
- ✅ Apenas features agregadas, sem PII
- ✅ Modelos não armazenam dados individuais
- ✅ Previsões logadas para auditoria

### Validação de Entrada
- ✅ **Data Annotations em todos os modelos**
  - Validação de ranges (idade 0-120, horas 0-23)
  - Proteção contra valores maliciosos
  - Mensagens de erro descritivas
  - ModelState validation no controller

### Controle de Acesso
- **Treinamento**: Apenas Admin/Owner
- **Carregamento**: Apenas Admin/Owner
- **Previsões**: Usuários autenticados do tenant
- **Hangfire Dashboard**: Admin/Owner apenas (autenticação implementada)

---

## ⚡ Performance

### Implementação Atual
- ✅ Thread-safe com locking
- ⚠️ PredictionEngine criado por request (overhead aceitável para baixa frequência)

### Otimização Futura (PredictionEnginePool)
Para alta frequência de previsões, considere:
```csharp
// Usar Microsoft.Extensions.ML
builder.Services.AddPredictionEnginePool<DadosNoShow, PrevisaoNoShowResult>()
    .FromFile("MLModels/modelo_noshow.zip");

// No serviço: Pool gerencia instances automaticamente
var previsao = _predictionEnginePool.Predict(dados);
```

**Benefícios:**
- Reuso de PredictionEngine instances
- Thread-safe nativo
- Melhor performance em alta escala

**Referência:** [Serve Model Web API ML.NET](https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/serve-model-web-api-ml-net)

---

## 🎨 Integração com Frontend (NOVO - Janeiro 2026)

### Frontend Service

**MLPredictionService** (`frontend/medicwarehouse-app/src/app/services/ml-prediction.service.ts`)

Serviço Angular para comunicação com APIs de ML:

```typescript
@Injectable({ providedIn: 'root' })
export class MLPredictionService {
  private apiUrl = `${environment.apiUrl}/MLPrediction`;

  // Previsão de demanda
  getPrevisaoProximaSemana(): Observable<PrevisaoConsultas>
  getPrevisaoParaData(data: string): Observable<PrevisaoDataEspecifica>
  
  // Previsão de no-show
  calcularRiscoNoShow(dados: DadosNoShow): Observable<RiscoNoShow>
  
  // Admin endpoints
  carregarModelos(): Observable<any>
  treinarModeloDemanda(): Observable<any>
  treinarModeloNoShow(): Observable<any>
}
```

### TypeScript Models

**ml-prediction.model.ts** - 7 interfaces TypeScript:
- `PrevisaoConsultas` - Container de previsões
- `PrevisaoDia` - Previsão individual por dia
- `PrevisaoDataEspecifica` - Previsão para data específica
- `DadosNoShow` - Input para predição de no-show
- `RiscoNoShow` - Output com risco e ações recomendadas
- `AgendamentoRisco` - Info completa de agendamento com risco

### Integração no Dashboard Clínico

#### Nova Seção: 🤖 Previsões com Machine Learning

**Visualização de Previsão de Demanda:**
- Gráfico de área (ApexCharts) mostrando próximos 7 dias
- Cards com total previsto e média diária
- Atualização automática ao carregar dashboard
- Loading states e error handling elegantes

**Informações de No-Show:**
- Card informativo sobre o sistema de predição
- Instruções para uso na tela de agendamentos
- Lista de ações recomendadas por nível de risco
- Design responsivo e acessível

#### Código de Integração

```typescript
@Component({
  selector: 'app-dashboard-clinico',
  // ...
})
export class DashboardClinicoComponent implements OnInit {
  previsaoDemanda?: PrevisaoConsultas;
  loadingPrevisao = false;
  previsaoError: string | null = null;
  
  constructor(
    private analyticsBIService: AnalyticsBIService,
    private mlPredictionService: MLPredictionService  // NOVO
  ) {}
  
  ngOnInit() {
    this.loadDashboard();
    this.loadPrevisaoDemanda();  // NOVO
  }
  
  loadPrevisaoDemanda() {
    this.loadingPrevisao = true;
    this.mlPredictionService.getPrevisaoProximaSemana().subscribe({
      next: (data) => {
        this.previsaoDemanda = data;
        this.initPrevisaoDemandaChart();  // NOVO: Renderiza gráfico
        this.loadingPrevisao = false;
      },
      error: (err) => {
        this.previsaoError = 'Erro ao carregar previsão...';
        this.loadingPrevisao = false;
      }
    });
  }
  
  initPrevisaoDemandaChart() {
    // Cria gráfico de área com ApexCharts
    // Mostra previsão dos próximos 7 dias
    // Design em gradiente verde
  }
}
```

#### HTML Template (Resumo)

```html
<!-- ML Predictions Section -->
<div class="ml-predictions-section">
  <h2>🤖 Previsões com Machine Learning</h2>
  
  <!-- Demand Forecast Chart -->
  <div class="chart-card">
    <h3>📈 Previsão de Demanda - Próxima Semana</h3>
    
    @if (loadingPrevisao) {
      <app-loading message="Carregando previsões..."></app-loading>
    }
    
    @if (previsaoDemanda) {
      <div class="prediction-summary">
        <div class="prediction-card">
          Total Previsto: {{ previsaoDemanda.totalPrevisto }} consultas
        </div>
      </div>
      
      <apx-chart
        [series]="previsaoDemandaChartOptions.series!"
        [chart]="previsaoDemandaChartOptions.chart!"
        <!-- ... outros options -->
      ></apx-chart>
    }
  </div>
  
  <!-- No-Show Risk Info -->
  <div class="chart-card">
    <h3>⚠️ Previsão de No-Show</h3>
    <div class="info-message info">
      Sistema disponível na tela de agendamentos...
    </div>
  </div>
</div>
```

#### Styling (SCSS)

```scss
.ml-predictions-section {
  margin: 2rem 0;
  
  .prediction-card {
    background: linear-gradient(135deg, #10b981 0%, #059669 100%);
    color: white;
    padding: 1.25rem;
    border-radius: 8px;
    
    .prediction-value {
      font-size: 1.5rem;
      font-weight: 600;
    }
  }
  
  .info-message {
    padding: 1.25rem;
    border-radius: 8px;
    
    &.info {
      background: rgba(59, 130, 246, 0.1);
      color: #1e40af;
      border: 1px solid rgba(59, 130, 246, 0.3);
    }
  }
}
```

### Experiência do Usuário

1. **Dashboard Clínico** é carregado
2. Simultaneamente, serviço ML busca previsões
3. Enquanto carrega: spinner animado
4. Ao completar: gráfico de área renderizado
5. Se erro: mensagem amigável explicando situação
6. Usuário pode ver previsão de 7 dias de uma vez
7. Cards destacados mostram totais e médias

### Tratamento de Erros

**Cenários Tratados:**
- ✅ Modelo ML não treinado: mensagem explicativa
- ✅ API offline: mensagem de erro temporário
- ✅ Timeout: loading infinito evitado
- ✅ Dados insuficientes: mensagem orientando treinamento

### Performance

- Requisições ML são **paralelas** ao dashboard principal
- Falha em ML não bloqueia dashboard
- Dados ML são **opcionais** e **independentes**
- Cache pode ser implementado para otimização futura

---

## 📚 Referências

- [ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [FastTree Algorithm](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.trainers.fasttree)
- [Binary Classification Guide](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis)
- [Regression Guide](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/predict-prices)
- [PredictionEnginePool Best Practices](https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/serve-model-web-api-ml-net)

---

**Última Atualização:** 27 de Janeiro de 2026  
**Versão:** 2.0.0 (com integração completa ao frontend)  
**Status:** ✅ Framework Completo com Integração Frontend - Pronto para Produção

**Documentos Relacionados:**
- `IMPLEMENTATION_SUMMARY_BI_ANALYTICS.md` - Status geral do projeto BI Analytics
- `CORREÇOES_PR425.md` - Detalhes das correções de segurança implementadas
- `15-bi-analytics.md` - Prompt completo com sprints 4 e 5
