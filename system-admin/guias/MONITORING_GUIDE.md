# Sistema de Monitoramento e Logging - Omni Care Software API

## 📊 Visão Geral

O sistema de monitoramento implementado oferece logging detalhado, rastreamento de performance e detecção automática de problemas sem necessidade de debugging manual.

## 🎯 Funcionalidades Principais

### 1. **Logging Estruturado com Serilog**
- Logs formatados e estruturados para análise fácil
- Múltiplos destinos (Console, Arquivo, Seq)
- Enriquecimento automático com contexto (Máquina, Thread, Processo)

### 2. **Monitoramento de Performance**
- Detecção automática de requisições lentas
- Alertas de timeout
- Categorização de performance (NORMAL, WARNING, CRITICAL, TIMEOUT)

### 3. **Logging Detalhado de Requisições**
- Captura completa de requisições HTTP
- Informações de usuário, tenant e IP
- Tamanho de resposta e duração

### 4. **Tratamento Avançado de Exceções**
- Contexto completo do erro
- Stack trace detalhado em logs
- Mensagens sanitizadas para o usuário

## 🔧 Configuração

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "Logs/omnicare-.log" } },
      { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
    ]
  },
  "Monitoring": {
    "SlowRequestThresholdMs": 3000,
    "PerformanceWarningThresholdMs": 2000,
    "PerformanceCriticalThresholdMs": 5000,
    "TimeoutThresholdMs": 30000,
    "EnableRequestLogging": true,
    "EnablePerformanceMonitoring": true
  }
}
```

## 📁 Estrutura de Logs

### Logs em Arquivo
Os logs são salvos em `Logs/` com rotação diária:

- **omnicare-YYYYMMDD.log**: Todos os logs (Information e acima)
- **omnicare-errors-YYYYMMDD.log**: Apenas erros e avisos
- Retenção: 30 dias para logs gerais, 60 dias para erros
- Rotação automática ao atingir 10MB

### Formato dos Logs

```
2026-01-18 22:30:00.123 +00:00 [INF] [MedicSoft.Api.Middleware.RequestLoggingMiddleware] Request initiated: GET /api/patients from 192.168.1.1
RequestId=abc-123, UserId=doctor@example.com, TenantId=clinic-001

2026-01-18 22:30:00.456 +00:00 [WRN] [MedicSoft.Api.Middleware.PerformanceMonitoringMiddleware] PERFORMANCE WARNING: GET /api/patients took 2500ms (warning threshold: 2000ms)
Endpoint=GET /api/patients, DurationMs=2500, PerformanceCategory=WARNING
```

## 🚨 Alertas e Thresholds

### Níveis de Performance
1. **NORMAL**: < 2000ms (Debug)
2. **WARNING**: 2000-5000ms (Warning)
3. **CRITICAL**: 5000-30000ms (Error)
4. **TIMEOUT**: > 30000ms (Critical)

### Exemplos de Alertas

**Requisição Lenta:**
```
SLOW REQUEST DETECTED: POST /api/appointments took 3500ms (threshold: 3000ms)
Status: 200, Size: 1234 bytes
```

**Performance Crítica:**
```
PERFORMANCE CRITICAL: GET /api/reports took 6000ms (critical threshold: 5000ms)
```

**Alerta de Timeout:**
```
TIMEOUT ALERT: GET /api/complex-report took 35000ms (timeout threshold: 30000ms)
Possible timeout scenario
```

## 📈 Visualização com Seq (Desenvolvimento)

### Instalação Local do Seq

#### Docker (Recomendado):
```bash
docker run -d \
  --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  datalust/seq:latest
```

#### Acesso:
- URL: http://localhost:5341
- Usuário: admin
- Senha: (definir no primeiro acesso)

### Queries Úteis no Seq

#### 1. Requisições Lentas
```
PerformanceCategory = "WARNING" or PerformanceCategory = "CRITICAL"
```

#### 2. Erros por Endpoint
```
@Level = "Error" 
| group by RequestPath 
| select count() as ErrorCount, RequestPath
```

#### 3. Performance por Usuário
```
@Message like "%PERFORMANCE%" 
| group by UserId 
| select avg(DurationMs), UserId
```

#### 4. Timeouts
```
@Message like "%TIMEOUT ALERT%"
```

#### 5. Taxa de Erro por Tenant
```
@Level = "Error" 
| group by TenantId 
| select count() as Errors, TenantId
```

## 🎯 Casos de Uso

### 1. Identificar Endpoints Lentos

**Problema**: Alguns endpoints estão demorando muito
**Solução**: Buscar logs com `SLOW REQUEST DETECTED` ou `PERFORMANCE WARNING`

```bash
# Em arquivos de log
grep "SLOW REQUEST" Logs/omnicare-*.log

# No Seq
@Message like "%SLOW REQUEST%"
```

### 2. Debugar Erro em Produção

**Problema**: Um erro ocorreu em produção e preciso entender o contexto
**Solução**: Buscar por RequestId nos logs

```bash
# Encontrar o erro
grep "ERROR" Logs/omnicare-errors-*.log | tail -n 20

# Buscar todo o contexto da requisição pelo RequestId
grep "RequestId=abc-123" Logs/omnicare-*.log
```

### 3. Monitorar Query SQL Lenta

**Problema**: Algumas queries estão demorando
**Solução**: Logs de database estão habilitados automaticamente

```
Database Query: Executed DbCommand (2,345ms) 
[Parameters=[@p0='123'], CommandType='Text', CommandTimeout='60']
SELECT * FROM Patients WHERE ClinicId = @p0
```

### 4. Análise de Uso por Tenant

**Problema**: Preciso saber qual clínica está gerando mais requisições
**Solução**: Filtrar logs por TenantId

```bash
# Contar requisições por tenant
grep "TenantId=clinic-001" Logs/omnicare-*.log | wc -l

# No Seq
TenantId != "None" 
| group by TenantId 
| select count() as RequestCount, TenantId
```

### 5. Rastrear Caminho de Execução

**Problema**: Preciso entender o fluxo completo de uma requisição
**Solução**: Usar o RequestId para correlacionar todos os logs

```
[15:30:00.123] Request initiated: POST /api/appointments
  RequestId=req-789

[15:30:00.234] Executing AppointmentService.CreateAppointment
  RequestId=req-789

[15:30:01.123] Database Query: INSERT INTO Appointments...
  RequestId=req-789

[15:30:01.456] Request completed: POST /api/appointments - Status: 201, Duration: 1333ms
  RequestId=req-789
```

## 🔒 Segurança e Privacidade

### Dados Sensíveis
- ❌ Senhas NUNCA são logadas
- ❌ Tokens de autenticação são omitidos
- ✅ Stack traces completos apenas em logs de erro
- ✅ Mensagens de erro sanitizadas para o usuário

### Configuração de Dados Sensíveis

```json
{
  "Monitoring": {
    "LogRequestBody": false,
    "LogResponseBody": false,
    "MaxBodyLogLength": 4096
  }
}
```

## 📊 Métricas Disponíveis

### Por Requisição
- ✅ Tempo de execução (ms)
- ✅ Status HTTP
- ✅ Tamanho da resposta (bytes)
- ✅ Usuário autenticado
- ✅ Tenant/Clínica
- ✅ IP de origem
- ✅ User Agent

### Por Aplicação
- ✅ Nome da máquina
- ✅ ID do processo
- ✅ ID da thread
- ✅ Ambiente (Dev/Prod)

## 🚀 Próximos Passos (Opcionais)

### 1. Application Insights (Azure - Gratuito até 5GB/mês)
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

### 2. Elastic Stack (ELK - Auto-hospedado)
- Elasticsearch: Armazenamento e busca
- Logstash: Processamento
- Kibana: Visualização

### 3. Grafana + Loki (Open Source)
```bash
docker run -d --name=loki -p 3100:3100 grafana/loki
docker run -d --name=grafana -p 3000:3000 grafana/grafana
```

### 4. Health Checks
Adicione endpoints de health check para monitoramento externo:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddCheck<CustomHealthCheck>("custom");

app.MapHealthChecks("/health");
```

## 📖 Boas Práticas

### 1. Níveis de Log Apropriados
```csharp
// ✅ Correto
_logger.LogInformation("User {UserId} logged in", userId);
_logger.LogWarning("Slow query detected: {Duration}ms", duration);
_logger.LogError(ex, "Failed to create appointment");

// ❌ Evitar
_logger.LogInformation($"User {userId} logged in"); // Interpolação de string
_logger.LogError("Error: " + ex.Message); // Sem exception object
```

### 2. Contexto Rico
```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["ClinicId"] = clinicId,
    ["Operation"] = "CreateAppointment"
}))
{
    // Todos os logs neste escopo terão essas propriedades
    _logger.LogInformation("Starting appointment creation");
}
```

### 3. Correlação de Requisições
- Sempre use o RequestId fornecido automaticamente
- Propague o RequestId para serviços externos

## 🆘 Troubleshooting

### Logs não aparecem no arquivo
```bash
# Verificar permissões da pasta Logs
ls -la Logs/

# Criar pasta se não existir
mkdir -p Logs
chmod 755 Logs
```

### Seq não conecta
```bash
# Verificar se está rodando
docker ps | grep seq

# Ver logs do Seq
docker logs seq

# Testar conectividade
curl http://localhost:5341
```

### Performance degradada após habilitar logs
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"  // Reduzir volume em produção
    }
  },
  "Monitoring": {
    "LogRequestBody": false,
    "LogResponseBody": false
  }
}
```

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs em `Logs/omnicare-errors-*.log`
2. Consulte o Seq em http://localhost:5341
3. Entre em contato com o suporte técnico

---

**Última Atualização**: 18 de Janeiro de 2026
**Versão**: 1.0.0
