# 📊 Sistema de Monitoramento e Logging - Omni Care Software

## 🎯 Visão Geral

Sistema completo de monitoramento e logging implementado para capturar **erros**, **execuções lentas**, **timeouts** e **métricas detalhadas** sem necessidade de debugging manual.

**Status:** ✅ **Completo e Production-Ready**  
**Custo:** 💰 **R$ 0,00 / mês**  
**Tempo para Setup:** ⏱️ **3 minutos**

---

## 🚀 Início Rápido (3 Passos)

### 1️⃣ Iniciar Seq (Opcional)
```bash
docker-compose -f docker-compose.seq.yml up -d
```

### 2️⃣ Executar API
```bash
cd src/MedicSoft.Api
dotnet run
```

### 3️⃣ Acessar Logs
- **Console:** Saída do terminal
- **Arquivo:** `Logs/omnicare-*.log`
- **Seq:** http://localhost:5341

---

## ✅ O Que Foi Implementado?

### Detecção Automática
- ✅ **Requisições lentas** (>3s): Alerta WARNING
- ✅ **Performance crítica** (>5s): Alerta ERROR
- ✅ **Possíveis timeouts** (>30s): Alerta CRITICAL
- ✅ **Queries SQL lentas**: Log automático
- ✅ **Erros com stack trace**: Contexto completo

### Métricas Capturadas
- ⏱️ Tempo de execução
- 👤 Usuário autenticado
- 🏥 Clínica/Tenant
- 🌐 IP de origem
- 📊 Status HTTP
- 💾 Tamanho da resposta
- 🔍 RequestId para correlação

### Destinos de Log
1. **Console** - Desenvolvimento imediato
2. **Arquivo** - Histórico persistente (30-60 dias)
3. **Seq** - Dashboard visual em tempo real

---

## 📚 Documentação Completa

### Principais Guias

| Documento | Descrição | Link |
|-----------|-----------|------|
| 🎯 **Solução Completa** | Resumo da implementação e problemas resolvidos | [SOLUTION_COMPLETE.md](./SOLUTION_COMPLETE.md) |
| 📖 **Guia Completo** | Manual completo (240+ linhas) com casos de uso | [MONITORING_GUIDE.md](./MONITORING_GUIDE.md) |
| ⚡ **Início Rápido** | Setup em 5 minutos | [QUICK_START_MONITORING.md](./QUICK_START_MONITORING.md) |
| 🔧 **Setup** | Passo a passo detalhado | [SETUP_MONITORING.md](./SETUP_MONITORING.md) |
| 📊 **Resumo** | Benefícios e impacto | [IMPLEMENTATION_SUMMARY.md](./IMPLEMENTATION_SUMMARY.md) |

---

## 🎯 Exemplos Práticos

### Ver Requisições Lentas
```bash
# Terminal
grep "SLOW REQUEST" Logs/omnicare-*.log

# Seq
@Message like "%SLOW REQUEST%"
```

### Rastrear Erro Específico
```bash
# Encontrar erro
grep "ERROR" Logs/omnicare-errors-*.log | tail -1

# Rastrear requisição completa
grep "RequestId=abc-123" Logs/omnicare-*.log
```

### Analisar Performance por Endpoint
```sql
-- Seq Query
@Message like "%PERFORMANCE%"
| group by Endpoint
| select avg(DurationMs) as AvgDuration, count() as Count, Endpoint
| order by AvgDuration desc
```

---

## 🔧 Configuração

### Ajustar Thresholds
Edite `src/MedicSoft.Api/appsettings.json`:

```json
{
  "Monitoring": {
    "SlowRequestThresholdMs": 3000,
    "PerformanceWarningThresholdMs": 2000,
    "PerformanceCriticalThresholdMs": 5000,
    "TimeoutThresholdMs": 30000
  }
}
```

### Reduzir Volume de Logs
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  }
}
```

---

## 📊 Estrutura de Logs

### Console Output
```
[22:30:00 INF] Request initiated: POST /api/appointments
  RequestId=req-789, UserId=doctor@example.com, TenantId=clinic-001

[22:30:01 INF] Request completed: POST /api/appointments
  Status=201, Duration=1234ms, Size=2048 bytes
```

### Arquivo de Log
```
2026-01-18 22:30:00.123 +00:00 [INF] [RequestLoggingMiddleware] 
Request initiated: POST /api/appointments
RequestId=req-789, UserId=doctor@example.com, TenantId=clinic-001, IP=192.168.1.10
```

### Seq Dashboard
- 📊 Gráficos em tempo real
- 🔍 Busca e filtros avançados
- 📈 Análise de tendências
- 🚨 Alertas configuráveis

---

## 🎓 Casos de Uso

### 1. Debugar Erro em Produção
**Problema:** Erro ocorreu mas não sei o contexto  
**Solução:** Buscar RequestId → ver toda a requisição

### 2. Identificar Endpoints Lentos
**Problema:** Sistema está lento mas não sei onde  
**Solução:** Filtrar por SLOW REQUEST → otimizar endpoints

### 3. Monitorar Queries SQL
**Problema:** Database está sobrecarregado  
**Solução:** Ver queries lentas automaticamente logadas

### 4. Analisar Uso por Clínica
**Problema:** Preciso saber qual clínica usa mais  
**Solução:** Agrupar logs por TenantId

---

## 💡 Tecnologias Utilizadas

- **Serilog** - Logging estruturado
- **Seq** - Dashboard visual (opcional)
- **ASP.NET Core** - Middleware pipeline
- **Docker** - Deploy fácil do Seq

Todas são **100% gratuitas** para uso básico!

---

## ✅ Validação

```
✅ Build: SUCCESS (0 errors)
✅ Serilog: Configurado
✅ Middleware: Registrados
✅ Logs: Funcionando
✅ Seq: Pronto (opcional)
✅ Docs: Completas
```

---

## 🆘 Troubleshooting

### Seq não conecta
```bash
docker ps | grep seq
docker start seq
curl http://localhost:5341
```

### Logs não aparecem
```bash
mkdir -p Logs
chmod 755 Logs
dotnet run
```

### Performance degradada
```json
// Reduzir nível de log
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  }
}
```

Mais ajuda: [MONITORING_GUIDE.md](./MONITORING_GUIDE.md) - Seção "Troubleshooting"

---

## 🔜 Próximos Passos

### Imediato
1. ✅ Explorar logs no console
2. ✅ Testar algumas requisições
3. ✅ Ver métricas no Seq

### Curto Prazo
- [ ] Configurar alertas no Seq
- [ ] Criar dashboards customizados
- [ ] Ajustar thresholds conforme uso real

### Futuro (Opcional)
- [ ] Health Checks
- [ ] Application Insights
- [ ] Grafana integration
- [ ] Métricas de negócio

---

## 📞 Suporte

### Documentação
- [🎯 Solução Completa](./SOLUTION_COMPLETE.md)
- [📖 Guia Completo](./MONITORING_GUIDE.md)
- [⚡ Início Rápido](./QUICK_START_MONITORING.md)
- [🔧 Setup](./SETUP_MONITORING.md)

### Recursos Externos
- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://docs.datalust.co/)
- [ASP.NET Core Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

---

## 🎉 Resumo

### Antes
❌ Sem logs estruturados  
❌ Debugging manual  
❌ Sem métricas  
❌ Erros invisíveis

### Depois
✅ Logs automáticos com contexto  
✅ Detecção proativa de problemas  
✅ Métricas completas  
✅ Dashboard visual  
✅ Zero custo  
✅ Production-ready

---

**Implementado por:** GitHub Copilot Agent  
**Data:** 18 de Janeiro de 2026  
**Versão:** 1.0.0  
**Status:** ✅ **COMPLETO**

---

## 🚀 Comece Agora!

```bash
# Clone e execute
git pull
cd src/MedicSoft.Api
dotnet run

# Faça uma requisição
curl http://localhost:5000/api/health

# Veja os logs
tail -f Logs/omnicare-*.log
```

**É isso!** Seu sistema já está sendo monitorado. 🎉
