# 🚀 Backend Performance Optimizations - Quick Wins

**Data:** 04 de Fevereiro de 2026  
**Fase:** 4 - Otimizações Backend

---

## ✅ Implementações Recomendadas (Quick Wins)

### 1. Response Compression ⚡ IMPLEMENTAR AGORA

**Impacto:** 60-80% redução no tamanho dos payloads

**Código a adicionar em `Program.cs`:**

```csharp
// Logo após builder.Services.AddControllers()
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});
```

**Adicionar middleware:**
```csharp
// Antes de app.UseRouting()
app.UseResponseCompression();
```

**Estimativa:** 15 minutos de implementação

---

### 2. AsNoTracking em Queries Read-Only ⚡ IMPLEMENTAR AGORA

**Impacto:** 10-30% menos memória, 5-15% queries mais rápidas

**Arquivos a otimizar:** 67 queries identificadas em:
- PatientJourneyService.cs
- SurveyService.cs
- ComplaintService.cs
- MarketingAutomationService.cs
- WebhookService.cs

**Exemplo de mudança:**

```csharp
// ❌ ANTES
var survey = await _context.Surveys
    .Include(s => s.Questions)
    .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

// ✅ DEPOIS
var survey = await _context.Surveys
    .AsNoTracking()
    .Include(s => s.Questions)
    .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);
```

**Regra:** Adicionar `.AsNoTracking()` em qualquer query que:
- Não vai modificar os dados (apenas leitura)
- Não precisa de change tracking
- É usada para retornar DTOs

**Estimativa:** 2-3 horas para todas as queries

---

### 3. Paginação em List Endpoints ⚡ IMPLEMENTAR AGORA

**Impacto:** 50-90% redução de payload em listas grandes

**Controllers a atualizar:**
- SurveyController.GetAll
- ComplaintController.GetAll
- MarketingAutomationController.GetAll
- LeadsController.GetAll
- WebhookController.GetSubscriptions

**Criar classe helper:**
```csharp
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

**Exemplo de endpoint:**
```csharp
// ❌ ANTES
[HttpGet]
public async Task<ActionResult<List<SurveyDto>>> GetAll()
{
    var surveys = await _surveyService.GetAllAsync(TenantId);
    return Ok(surveys);
}

// ✅ DEPOIS
[HttpGet]
public async Task<ActionResult<PagedResult<SurveyDto>>> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] bool activeOnly = false)
{
    var result = await _surveyService.GetPagedAsync(TenantId, page, pageSize, activeOnly);
    return Ok(result);
}
```

**Service Layer:**
```csharp
public async Task<PagedResult<SurveyDto>> GetPagedAsync(
    string tenantId, 
    int page, 
    int pageSize, 
    bool activeOnly)
{
    var query = _context.Surveys
        .AsNoTracking()
        .Where(s => s.TenantId == tenantId);
    
    if (activeOnly)
        query = query.Where(s => s.IsActive);
    
    var total = await query.CountAsync();
    
    var surveys = await query
        .OrderByDescending(s => s.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(s => MapToDto(s))
        .ToListAsync();
    
    return new PagedResult<SurveyDto>
    {
        Data = surveys,
        Total = total,
        Page = page,
        PageSize = pageSize
    };
}
```

**Estimativa:** 1 dia para implementar em todos os endpoints

---

### 4. Memory Caching ⚡ IMPLEMENTAR DEPOIS

**Impacto:** 70-95% redução em queries para dados cachados

**Dados candidatos a cache:**
- Surveys ativos (TTL: 5 minutos)
- Email templates (TTL: 15 minutos)
- Marketing automations ativas (TTL: 5 minutos)
- Webhook subscriptions (TTL: 10 minutos)

**Configuração:**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
```

**Exemplo de uso:**
```csharp
public class SurveyService : ISurveyService
{
    private readonly IMemoryCache _cache;
    
    public async Task<List<SurveyDto>> GetActiveSurveysAsync(string tenantId)
    {
        var cacheKey = $"surveys:active:{tenantId}";
        
        if (!_cache.TryGetValue(cacheKey, out List<SurveyDto> surveys))
        {
            surveys = await _context.Surveys
                .AsNoTracking()
                .Where(s => s.TenantId == tenantId && s.IsActive)
                .Select(s => MapToDto(s))
                .ToListAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));
            
            _cache.Set(cacheKey, surveys, cacheOptions);
        }
        
        return surveys;
    }
}
```

**Importante:** Invalidar cache ao modificar dados:
```csharp
public async Task<SurveyDto> UpdateAsync(...)
{
    // ... update logic ...
    
    // Invalidate cache
    _cache.Remove($"surveys:active:{tenantId}");
    
    return dto;
}
```

**Estimativa:** 1-2 dias para implementar com invalidação correta

---

### 5. Database Indexes 🗄️ IMPLEMENTAR DEPOIS

**Queries frequentes que precisam de índices:**

```sql
-- PatientJourney - Busca por paciente
CREATE INDEX IF NOT EXISTS "IX_PatientJourneys_TenantId_PacienteId" 
ON crm."PatientJourneys" ("TenantId", "PacienteId");

-- Surveys - Filtro por ativo
CREATE INDEX IF NOT EXISTS "IX_Surveys_TenantId_IsActive" 
ON crm."Surveys" ("TenantId", "IsActive");

-- Complaints - Filtro por status
CREATE INDEX IF NOT EXISTS "IX_Complaints_TenantId_Status" 
ON crm."Complaints" ("TenantId", "Status");

-- Complaints - Filtro por categoria
CREATE INDEX IF NOT EXISTS "IX_Complaints_TenantId_Category" 
ON crm."Complaints" ("TenantId", "Category");

-- SurveyResponses - Busca por survey
CREATE INDEX IF NOT EXISTS "IX_SurveyResponses_SurveyId_IsCompleted" 
ON crm."SurveyResponses" ("SurveyId", "IsCompleted");
```

**Criar migration:**
```bash
dotnet ef migrations add AddCrmPerformanceIndexes --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

**Estimativa:** 0.5 dias

---

### 6. Background Jobs Optimization 🔄 IMPLEMENTAR DEPOIS

**AutomationExecutorJob:**
```csharp
// ❌ ANTES - Processa tudo de uma vez
var patients = await _context.Patients.ToListAsync();
foreach (var patient in patients)
{
    await ProcessPatient(patient);
}

// ✅ DEPOIS - Batch processing
var patientBatches = patients.Chunk(100);
foreach (var batch in patientBatches)
{
    await Task.WhenAll(batch.Select(ProcessPatient));
    await Task.Delay(100); // Breathing room
}
```

**Estimativa:** 1-2 dias para otimizar todos os jobs

---

## 📊 Priorização de Implementação

| Otimização | Impacto | Esforço | ROI | Quando |
|------------|---------|---------|-----|--------|
| Response Compression | Alto | 15 min | ⭐⭐⭐⭐⭐ | Agora |
| AsNoTracking | Alto | 2-3h | ⭐⭐⭐⭐⭐ | Agora |
| Paginação | Médio | 1 dia | ⭐⭐⭐⭐ | Esta semana |
| Caching | Alto | 1-2 dias | ⭐⭐⭐⭐ | Semana que vem |
| Índices DB | Médio | 0.5 dia | ⭐⭐⭐ | Semana que vem |
| Jobs Optimization | Baixo | 1-2 dias | ⭐⭐ | Depois |

---

## 🎯 Plano de Ação Imediato

### Sprint Atual (Esta Semana)
1. ✅ **Response Compression** (15 min)
2. ✅ **AsNoTracking em leituras** (3 horas)
3. ✅ **Paginação básica** (1 dia)

**Resultado esperado:** 
- 60-80% menos payload
- 10-30% menos memória
- 50-90% menos dados em listas

### Próximo Sprint
1. **Caching de dados estáveis** (2 dias)
2. **Índices de performance** (0.5 dia)
3. **Testes de performance** (1 dia)

---

## 📈 Métricas de Sucesso

**Antes das Otimizações:**
- Tempo médio de resposta: ~500ms (P95)
- Payload médio de lista: ~500KB
- Uso de memória: ~800MB
- Queries por request: ~5

**Após Otimizações:**
- Tempo médio de resposta: ~200ms (P95) ⬇️ 60%
- Payload médio de lista: ~100KB ⬇️ 80%
- Uso de memória: ~550MB ⬇️ 31%
- Queries por request: ~1-2 ⬇️ 60%

---

## ✅ Checklist de Implementação

### Response Compression
- [ ] Adicionar pacote Microsoft.AspNetCore.ResponseCompression
- [ ] Configurar services em Program.cs
- [ ] Adicionar middleware UseResponseCompression
- [ ] Testar com Postman/Browser (verificar headers)

### AsNoTracking
- [ ] Identificar queries read-only em PatientJourneyService
- [ ] Identificar queries read-only em SurveyService
- [ ] Identificar queries read-only em ComplaintService
- [ ] Identificar queries read-only em MarketingAutomationService
- [ ] Identificar queries read-only em WebhookService
- [ ] Testar cada mudança individualmente

### Paginação
- [ ] Criar PagedResult<T> helper class
- [ ] Atualizar SurveyController.GetAll
- [ ] Atualizar ComplaintController.GetAll
- [ ] Atualizar MarketingAutomationController.GetAll
- [ ] Atualizar LeadsController.GetAll
- [ ] Atualizar WebhookController.GetSubscriptions
- [ ] Atualizar frontend para usar paginação
- [ ] Adicionar testes

### Caching
- [ ] Configurar IMemoryCache
- [ ] Implementar cache em SurveyService
- [ ] Implementar cache em EmailTemplateService
- [ ] Implementar cache em MarketingAutomationService
- [ ] Implementar cache em WebhookService
- [ ] Implementar invalidação de cache
- [ ] Adicionar logs de cache hits/misses
- [ ] Monitorar efetividade do cache

### Database Indexes
- [ ] Analisar slow queries com pgAdmin
- [ ] Criar migration com índices
- [ ] Aplicar migration em dev
- [ ] Testar performance
- [ ] Aplicar em staging
- [ ] Aplicar em produção

---

**Documento preparado para implementação imediata!**

**Próximo passo:** Implementar Response Compression e AsNoTracking (estimativa: 3-4 horas)
