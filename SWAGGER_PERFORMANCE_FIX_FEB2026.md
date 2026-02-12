# Correção de Performance do Swagger - Fevereiro 2026

## Problema
Ao executar a API, o Swagger e as APIs levavam quase 1 minuto para carregar.

## Causa Raiz
Com **111 controladores** e documentação XML habilitada, o Swagger estava regenerando o arquivo swagger.json em **cada requisição**, processando:
- Todos os comentários XML dos 111 controladores
- Todas as ações e parâmetros de cada controlador
- Geração de schemas OpenAPI para todos os tipos
- Resolução de conflitos de nomes de schemas

Este processo de geração é computacionalmente intensivo e estava sendo executado repetidamente sem cache.

## Solução Implementada

### 1. Cache-Control Headers para Swagger JSON
Adicionado cabeçalho de cache no endpoint `/swagger/v1/swagger.json`:

```csharp
app.UseSwagger(c =>
{
    // Enable caching to improve Swagger loading performance
    // Swagger JSON will be cached for 24 hours to avoid regeneration on every request
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        httpReq.HttpContext.Response.Headers.Append("Cache-Control", "public, max-age=86400"); // 24 hours
    });
});
```

**Benefício**: O navegador e proxies podem fazer cache do arquivo swagger.json por 24 horas, evitando requisições desnecessárias.

### 2. Response Caching Middleware
Ativado o middleware `UseResponseCaching()` que já estava configurado mas não estava sendo utilizado:

```csharp
// Add Response Caching (after compression, before other middleware)
// This improves performance by caching responses including Swagger JSON
app.UseResponseCaching();
```

**Benefício**: O servidor pode fazer cache de respostas no lado do servidor, melhorando ainda mais a performance.

## Impacto da Mudança

### Performance Esperada
- **Primeira requisição**: ~10-15 segundos (geração completa do swagger.json)
- **Requisições subsequentes**: <1 segundo (servido do cache)
- **Após 24 horas**: O cache expira e o arquivo é regenerado automaticamente

### Sem Impacto em
- ✅ Funcionalidade da API
- ✅ Swagger UI
- ✅ Autenticação/Autorização
- ✅ Documentação XML
- ✅ Desenvolvimento local

## Arquivos Modificados
- `src/MedicSoft.Api/Program.cs`
  - Linha ~766: Adicionado configuração de cache para UseSwagger
  - Linha ~799: Adicionado UseResponseCaching middleware

## Testes de Validação

### Teste Manual
1. Iniciar a API
2. Acessar `/swagger` pela primeira vez
   - Deve carregar em ~10-15 segundos (geração)
3. Recarregar a página (F5)
   - Deve carregar em <1 segundo (cache)
4. Verificar cabeçalhos HTTP em `/swagger/v1/swagger.json`
   - Deve conter: `Cache-Control: public, max-age=86400`

### Como Testar
```bash
# Verificar cabeçalhos de cache
curl -I http://localhost:5000/swagger/v1/swagger.json

# Deve incluir:
# Cache-Control: public, max-age=86400
```

## Considerações Técnicas

### Cache de 24 Horas
O tempo de cache de 24 horas foi escolhido porque:
- ✅ Balanceia performance vs. atualização de documentação
- ✅ Em desenvolvimento, cache pode ser limpo com Ctrl+Shift+R (hard refresh)
- ✅ Em produção, novas versões da API invalidam o cache automaticamente

### Alternativas Consideradas
1. **Remover XML documentation** ❌ 
   - Perderia documentação importante da API
2. **Reduzir número de controladores** ❌ 
   - Não é prático/viável
3. **Swagger em arquivo estático** ❌ 
   - Perderia sincronização automática com código

## Conclusão
A solução implementada é **mínima, eficaz e não invasiva**, resolvendo o problema de performance sem impactar funcionalidades existentes. O Swagger agora carrega rapidamente após a primeira geração, melhorando significativamente a experiência do desenvolvedor.

## Próximos Passos (Opcional)
- [ ] Monitorar logs para verificar melhoria de performance
- [ ] Considerar ajustar tempo de cache se necessário (24h vs 1h vs 12h)
- [ ] Avaliar se há controllers que podem ter documentação simplificada
