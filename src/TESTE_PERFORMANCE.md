# 🧪 Teste Prático - Validação de Performance

## Antes de Testar

Certifique-se que:
- ✅ Compilou sem erros: `dotnet build MedicSoft.sln`
- ✅ Banco de dados está acessível
- ✅ `appsettings.Development.json` está configurado

---

## Teste 1: Medição de Startup Time

### Passo 1 - Executar com timer
```bash
cd /Users/igorlessarobainadesouza/Documents/MW.Code/src

# Método 1: Usar 'time' command
time dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj

# Método 2: Ver logs no VS Code
# Ao ver "Iniciando Omni Care Software API..." começar o timer
# Parar quando ver "Now listening on:"
```

**Resultado esperado**: Startup < 15 segundos

---

## Teste 2: Swagger Acessibilidade

### Medida 1 - Primeira requisição ao swagger.json

```bash
# Terminal 1 - deixar rodando
cd /Users/igorlessarobainadesouza/Documents/MW.Code/src
dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj

# Terminal 2 - medir tempo de resposta
curl -w "\nTempo Total: %{time_total}s\n" \
  -o /dev/null -s \
  http://localhost:5000/swagger/v1/swagger.json
```

**Resultado esperado**: < 5 segundos (primeira vez)

### Medida 2 - Swagger UI Loading

1. Abrir navegador
2. Ir para `http://localhost:5000/swagger`
3. Abrir Developer Tools (F12)
4. Aba "Network"
5. Observar tempo de carregamento dos assets

**Resultado esperado**: 
- swagger-ui.css: <1s
- swagger-ui.js: <1s  
- swagger.json: <5s
- **Total**: <10s

---

## Teste 3: Logs de Background Task

### Verificar que migrations rodaram em background

1. Deixar API rodando
2. Procurar nos logs por:
   ```
   [Information] Database migrations applied successfully
   [Information] Defensive database repair completed
   ```

3. Confirmar que essas mensagens aparecem após o Swagger estar acessível

**Resultado esperado**: Logs aparecem em ~10-30 segundos após startup

---

## Teste 4: Comparação Antes vs Depois (Opcional)

### Desfazer mudanças temporariamente
```bash
git diff MedicSoft.Api/Program.cs
# Anotar as mudanças
git checkout MedicSoft.Api/Program.cs
git checkout MedicSoft.Api/Filters/AuthorizeCheckOperationFilter.cs
```

### Medir antes
```bash
time dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj
# Anotar tempo até "Now listening on"
```

### Restaurar mudanças
```bash
git checkout .
```

### Medir depois
```bash
time dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj
# Comparar tempo
```

---

## Troubleshooting

### ❌ "API demora muito para iniciar"
**Possível causa**: Database connection lenta ou migrations ainda rodando  
**Solução**: Verificar logs para `Database migrations applied successfully`

### ❌ "Swagger ainda lento"
**Possível causa**: XML comments ainda sendo carregados  
**Solução**: Verificar `appsettings.Development.json`:
```json
"SwaggerSettings": {
  "IncludeXmlComments": false
}
```

### ❌ "Erro de compilação"
**Solução**: Limpar e reconstruir
```bash
dotnet clean MedicSoft.sln
dotnet build MedicSoft.sln -c Debug
```

### ❌ "Erro de database"
**Possível causa**: Migrations rodando em background causaram erro  
**Solução**: Verificar logs para mensagens de erro SQL e verificar conexão com banco

---

## Checklist de Validação

- [ ] API inicia em <15 segundos
- [ ] Swagger acessível em <5 segundos após startup
- [ ] Logs mostram "Database migrations applied successfully"
- [ ] Nenhum erro de compilação (0 erros, apenas warnings)
- [ ] Swagger UI carrega completamente em <10 segundos
- [ ] Endpoints funcionam normalmente
- [ ] Database foi configurado com sucesso (verificar tabelas criadas)

---

## Comandos Úteis

### Ver logs de compilação detalhado
```bash
dotnet build MedicSoft.sln -v detailed 2>&1 | grep -i swagger
```

### Monitorar tamanho do swagger.json
```bash
curl -s http://localhost:5000/swagger/v1/swagger.json | wc -c
# Resultado esperado: 1-2 MB
```

### Verificar que XML não está sendo carregado (em dev)
```bash
dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj 2>&1 | grep -i "XML documentation\|xml comments"
# Não deve aparecer nada em Development
```

### Listar todas as informações de startup
```bash
dotnet run --project MedicSoft.Api/MedicSoft.Api.csproj 2>&1 | grep -E "\[Information\]|Now listening|migrations|Swagger"
```

---

## Resultado Esperado (Logs Completos)

```
[Information] Iniciando Omni Care Software API...
[Information] Configuração de logging Serilog aplicada com sucesso
[Information] Swagger XML comments skipped for faster startup
[Information] Configure the HTTP request pipeline
[Information] Now listening on: http://localhost:5000
[Application started. Press Ctrl+C to shut down.]
...
[10 segundos depois]
[Information] Database migrations applied successfully
[Information] Defensive database repair completed
```

---

**Tempo estimado do teste completo**: 5 minutos ⏱️

**Data da implementação**: 18 de fevereiro de 2026
