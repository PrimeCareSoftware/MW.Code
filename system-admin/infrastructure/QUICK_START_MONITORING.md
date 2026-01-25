# 🚀 Guia Rápido - Sistema de Monitoramento

## Início Rápido (5 minutos)

### 1. Verificar Instalação

Os pacotes Serilog já estão instalados no projeto. Verifique:

```bash
cd src/MedicSoft.Api
dotnet restore
```

### 2. Iniciar o Seq (opcional, mas recomendado)

```bash
# Usando Docker
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

# Acessar: http://localhost:5341
```

### 3. Executar a API

```bash
dotnet run
```

### 4. Verificar Logs

Os logs já estão sendo gravados em:
- **Console**: Saída padrão do terminal
- **Arquivo**: `Logs/primecare-YYYYMMDD.log`
- **Seq**: http://localhost:5341 (se configurado)

## 🎯 Principais Recursos

### Detecção Automática de Problemas

✅ **Requisições Lentas** (> 3s)
```
SLOW REQUEST DETECTED: GET /api/patients took 3500ms
```

✅ **Performance Crítica** (> 5s)
```
PERFORMANCE CRITICAL: POST /api/appointments took 6000ms
```

✅ **Possíveis Timeouts** (> 30s)
```
TIMEOUT ALERT: GET /api/reports took 35000ms
```

✅ **Erros com Contexto Completo**
```
[ERROR] Failed to create appointment
  UserId=doctor@example.com
  TenantId=clinic-001
  RequestPath=/api/appointments
  Exception: NullReferenceException
```

### Queries SQL Lentas

Automaticamente detectadas e logadas:
```
Database Query: Executed DbCommand (2,345ms)
SELECT * FROM Patients WHERE ClinicId = @p0
```

## 📊 Visualização com Seq

### Dashboards Prontos

1. **Tempo Real**
   - Acesse: http://localhost:5341
   - Veja requisições em tempo real

2. **Filtrar por Severidade**
   ```
   @Level = "Error"
   @Level = "Warning"
   ```

3. **Buscar por Usuário**
   ```
   UserId = "doctor@example.com"
   ```

4. **Performance por Endpoint**
   ```
   @Message like "%PERFORMANCE%"
   | group by Endpoint
   | select avg(DurationMs), Endpoint
   ```

## 🔍 Análise de Logs

### Buscar Erros Recentes

```bash
# Últimos 20 erros
tail -n 20 Logs/primecare-errors-$(date +%Y%m%d).log

# Buscar erro específico
grep "NullReferenceException" Logs/primecare-errors-*.log
```

### Analisar Requisições por Tenant

```bash
# Contar requisições de uma clínica
grep "TenantId=clinic-001" Logs/primecare-$(date +%Y%m%d).log | wc -l
```

### Rastrear Requisição Específica

```bash
# Buscar todos os logs de uma requisição
grep "RequestId=abc-123" Logs/primecare-*.log
```

## ⚙️ Configuração Customizada

### Ajustar Thresholds

Edite `appsettings.json`:

```json
{
  "Monitoring": {
    "SlowRequestThresholdMs": 3000,     // Padrão: 3s
    "PerformanceWarningThresholdMs": 2000,  // Padrão: 2s
    "PerformanceCriticalThresholdMs": 5000,  // Padrão: 5s
    "TimeoutThresholdMs": 30000         // Padrão: 30s
  }
}
```

### Desabilitar Monitoramento (se necessário)

```json
{
  "Monitoring": {
    "EnableRequestLogging": false,
    "EnablePerformanceMonitoring": false
  }
}
```

### Reduzir Volume de Logs em Produção

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"  // Apenas avisos e erros
    }
  }
}
```

## 💡 Dicas Práticas

### 1. Monitorar em Desenvolvimento

Mantenha o Seq aberto em uma aba do navegador durante o desenvolvimento:
- Veja erros em tempo real
- Identifique queries lentas imediatamente
- Analise o fluxo de cada requisição

### 2. Investigar Problemas de Performance

```
1. Abra Seq
2. Filtre por: PerformanceCategory = "WARNING" or PerformanceCategory = "CRITICAL"
3. Identifique os endpoints mais lentos
4. Verifique as queries SQL relacionadas
5. Otimize conforme necessário
```

### 3. Debug de Erros em Produção

```
1. Localize o erro em: Logs/primecare-errors-*.log
2. Copie o RequestId
3. Busque todo o contexto: grep "RequestId=..." Logs/primecare-*.log
4. Analise o fluxo completo da requisição
```

## 🎓 Próximos Passos

1. **Familiarize-se com o Seq**
   - Explore queries
   - Crie dashboards personalizados
   - Configure alertas

2. **Revise os Logs Regularmente**
   - Identifique padrões de erro
   - Otimize endpoints lentos
   - Monitore crescimento de uso

3. **Documente Problemas Comuns**
   - Crie playbook de troubleshooting
   - Documente soluções encontradas

## 📚 Documentação Completa

Para mais detalhes, consulte:
- [MONITORING_GUIDE.md](./MONITORING_GUIDE.md) - Guia completo
- [Serilog Documentation](https://serilog.net/) - Documentação oficial
- [Seq Documentation](https://docs.datalust.co/docs) - Documentação do Seq

## ✅ Checklist de Verificação

Após configurar, verifique:

- [ ] API iniciando sem erros
- [ ] Logs sendo gravados em `Logs/`
- [ ] Seq conectado e recebendo logs (se configurado)
- [ ] Requisições sendo logadas com tempo de execução
- [ ] Erros aparecendo com contexto completo
- [ ] Queries SQL sendo logadas

## 🆘 Problemas Comuns

### "Seq não conecta"
- ✅ Verifique se o Docker está rodando: `docker ps`
- ✅ Tente acessar: http://localhost:5341
- ✅ Pode funcionar sem o Seq (logs em arquivo)

### "Logs não aparecem"
- ✅ Verifique permissões da pasta `Logs/`
- ✅ Confira se `appsettings.json` está correto
- ✅ Reinicie a aplicação

### "Muitos logs, performance degradada"
- ✅ Aumente `MinimumLevel` para "Warning"
- ✅ Desabilite logs de requisição em produção
- ✅ Reduza `SlowRequestThresholdMs`

---

**Pronto para começar!** 🎉

Qualquer dúvida, consulte o guia completo ou os logs de erro em `Logs/primecare-errors-*.log`.
