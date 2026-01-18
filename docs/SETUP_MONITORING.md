# Setup do Sistema de Monitoramento - PrimeCare Software

## 🚀 Início Rápido (3 passos)

### Passo 1: Iniciar o Seq (Opcional, mas recomendado)

```bash
# Usando Docker Compose (recomendado)
docker-compose -f docker-compose.seq.yml up -d

# OU usando Docker diretamente
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

# Verificar se está rodando
docker ps | grep seq

# Acessar: http://localhost:5341
```

### Passo 2: Executar a API

```bash
cd src/MedicSoft.Api
dotnet restore
dotnet run
```

### Passo 3: Verificar Logs

#### No Console
Os logs aparecem automaticamente na saída do terminal.

#### Em Arquivo
```bash
# Ver todos os logs de hoje
cat Logs/primecare-$(date +%Y%m%d).log

# Ver apenas erros
cat Logs/primecare-errors-$(date +%Y%m%d).log

# Seguir logs em tempo real
tail -f Logs/primecare-$(date +%Y%m%d).log
```

#### No Seq (se configurado)
1. Abra: http://localhost:5341
2. Veja logs em tempo real
3. Use queries para filtrar e analisar

## 📊 Exemplos de Uso

### Ver Requisições Lentas

**No terminal:**
```bash
grep "SLOW REQUEST" Logs/primecare-*.log
```

**No Seq:**
```
@Message like "%SLOW REQUEST%"
```

### Buscar Erros Recentes

**No terminal:**
```bash
tail -20 Logs/primecare-errors-$(date +%Y%m%d).log
```

**No Seq:**
```
@Level = "Error"
| select @Timestamp, @Message, @Exception
| order by @Timestamp desc
```

### Analisar Performance por Endpoint

**No Seq:**
```
@Message like "%PERFORMANCE%"
| group by Endpoint
| select count() as Count, avg(DurationMs) as AvgDuration, Endpoint
| order by AvgDuration desc
```

### Rastrear Requisição Específica

**No terminal:**
```bash
# Encontrar RequestId no erro
grep "ERROR" Logs/primecare-errors-*.log | tail -1

# Buscar todo o contexto
grep "RequestId=abc-123" Logs/primecare-*.log
```

**No Seq:**
```
RequestId = "abc-123"
| select @Timestamp, @Message, @Level
| order by @Timestamp asc
```

## ⚙️ Configuração

### Ajustar Thresholds de Performance

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

### Desabilitar Logs de Requisição

```json
{
  "Monitoring": {
    "EnableRequestLogging": false,
    "EnablePerformanceMonitoring": false
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

## 🔍 Comandos Úteis

### Gerenciar Seq

```bash
# Iniciar
docker start seq

# Parar
docker stop seq

# Remover
docker rm seq

# Ver logs do Seq
docker logs seq
```

### Análise de Logs

```bash
# Contar requisições de hoje
grep "Request initiated" Logs/primecare-$(date +%Y%m%d).log | wc -l

# Top 10 endpoints mais usados
grep "Request initiated" Logs/primecare-*.log | awk '{print $7}' | sort | uniq -c | sort -rn | head -10

# Erros agrupados por tipo
grep "\[ERR\]" Logs/primecare-errors-*.log | awk -F'ExceptionType=' '{print $2}' | awk '{print $1}' | sort | uniq -c
```

### Limpeza de Logs

```bash
# Remover logs antigos (mais de 30 dias)
find Logs/ -name "primecare-*.log" -mtime +30 -delete

# Remover logs de erro antigos (mais de 60 dias)
find Logs/ -name "primecare-errors-*.log" -mtime +60 -delete
```

## 📚 Documentação Completa

- [Guia Completo de Monitoramento](./MONITORING_GUIDE.md)
- [Guia de Início Rápido](./QUICK_START_MONITORING.md)
- [Resumo da Implementação](./IMPLEMENTATION_SUMMARY.md)

## 🆘 Troubleshooting

### Seq não conecta

```bash
# Verificar se está rodando
docker ps | grep seq

# Se não estiver, iniciar
docker start seq

# Se não existir, criar
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

# Testar conectividade
curl http://localhost:5341
```

### Logs não aparecem em arquivo

```bash
# Criar pasta de logs
mkdir -p Logs
chmod 755 Logs

# Verificar permissões
ls -la Logs/

# Reiniciar a API
```

### Performance degradada com muitos logs

```json
// Reduzir nível de log em appsettings.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  }
}
```

## 🎯 Próximos Passos

1. ✅ Familiarize-se com os logs no console
2. ✅ Explore os arquivos de log
3. ✅ Configure e explore o Seq
4. ✅ Crie queries customizadas
5. ✅ Configure alertas (Seq)
6. ✅ Integre com ferramentas de monitoramento

## ✅ Checklist de Verificação

Após o setup, verifique:

- [ ] API iniciando sem erros
- [ ] Logs aparecendo no console
- [ ] Arquivo de log sendo criado em `Logs/`
- [ ] Seq conectado e recebendo logs
- [ ] Requisições sendo logadas
- [ ] Erros capturados com contexto

## 💡 Dicas

1. **Mantenha o Seq aberto** durante desenvolvimento para feedback visual imediato
2. **Use `tail -f`** para acompanhar logs em tempo real no terminal
3. **Filtre por RequestId** para rastrear requisições específicas
4. **Configure alertas** no Seq para erros críticos
5. **Revise logs regularmente** para identificar padrões

---

**Pronto para começar!** 🎉

Se tiver dúvidas, consulte a [documentação completa](./MONITORING_GUIDE.md).
