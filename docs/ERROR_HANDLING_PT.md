# Tratamento de Erros Amigável ao Usuário

## Visão Geral

Este documento descreve as melhorias implementadas para tornar as mensagens de erro mais amigáveis ao usuário, evitando a exposição de detalhes técnicos ou falhas de segurança.

## Mudanças no Backend (.NET)

### 1. Middleware de Tratamento Global de Exceções

**Arquivo**: `src/MedicSoft.Api/Middleware/GlobalExceptionHandlerMiddleware.cs`

Este middleware intercepta todas as exceções não tratadas na aplicação e:

- **Sanitiza** mensagens de erro para remover detalhes técnicos
- **Traduz** mensagens comuns do inglês para português brasileiro
- **Categoriza** exceções por tipo e retorna códigos HTTP apropriados
- **Padroniza** o formato de resposta de erro

#### Tipos de Exceções Tratadas

| Exceção | Status HTTP | Mensagem ao Usuário |
|---------|-------------|---------------------|
| `UnauthorizedAccessException` | 401 | "Você não tem permissão para realizar esta ação." |
| `InvalidOperationException` | 400 | Mensagem sanitizada ou genérica |
| `ArgumentException` / `ArgumentNullException` | 400 | "Os dados fornecidos são inválidos..." |
| `KeyNotFoundException` | 404 | "O recurso solicitado não foi encontrado." |
| `TimeoutException` | 408 | "A operação demorou muito tempo..." |
| Outras exceções | 500 | "Ocorreu um erro ao processar sua solicitação..." |

#### Formato de Resposta Padronizado

```json
{
  "message": "Mensagem amigável em português",
  "errorCode": "CODIGO_DO_ERRO",
  "timestamp": "2026-01-12T15:30:00Z"
}
```

### 2. Helper de Validação

**Arquivo**: `src/MedicSoft.Api/Helpers/ValidationHelper.cs`

Fornece métodos para converter erros de `ModelState` em mensagens amigáveis:

- **Traduz** mensagens de validação do inglês para português
- **Traduz** nomes de campos comuns
- **Formata** erros em um formato estruturado e legível

#### Uso

```csharp
if (!ModelState.IsValid)
    return BadRequest(ValidationHelper.GetValidationErrors(ModelState));
```

### 3. Registro do Middleware

O middleware é registrado em `Program.cs` como o primeiro middleware da pipeline para garantir que capture todas as exceções:

```csharp
app.UseGlobalExceptionHandler();
```

## Mudanças no Frontend (Angular)

### 1. Interceptor de Erros HTTP

**Arquivo**: `frontend/medicwarehouse-app/src/app/interceptors/error.interceptor.ts`

Este interceptor:

- **Intercepta** todos os erros HTTP
- **Traduz** códigos de status HTTP para mensagens em português
- **Exibe** notificações toast automáticas para erros
- **Redireciona** para login em caso de erro 401
- **Extrai** mensagens da API quando disponíveis

#### Mensagens por Status HTTP

| Status | Mensagem |
|--------|----------|
| 400 | "Os dados fornecidos são inválidos..." |
| 401 | "Sua sessão expirou..." |
| 403 | "Você não tem permissão..." |
| 404 | "O recurso solicitado não foi encontrado." |
| 408 | "A operação demorou muito tempo..." |
| 409 | "Este registro já existe no sistema." |
| 429 | "Muitas requisições..." |
| 500 | "Ocorreu um erro no servidor..." |
| 502/503/504 | "O serviço está temporariamente indisponível..." |
| 0 | "Não foi possível conectar ao servidor..." |

### 2. Registro do Interceptor

O interceptor é registrado em `app.config.ts`:

```typescript
provideHttpClient(withInterceptors([
  mockDataInterceptor, 
  authInterceptor, 
  errorInterceptor  // Novo interceptor
]))
```

### 3. Simplificação de Componentes

Os componentes agora podem simplificar o tratamento de erros, pois o interceptor cuida automaticamente:

**Antes:**
```typescript
this.service.getData().subscribe({
  next: (data) => this.data = data,
  error: (error) => {
    this.error = error.error?.message || 'Erro ao carregar dados';
    this.loading = false;
  }
});
```

**Depois:**
```typescript
this.service.getData().subscribe({
  next: (data) => this.data = data,
  error: () => {
    // Erro já tratado pelo interceptor
    this.loading = false;
  }
});
```

## Segurança

### Proteção Contra Exposição de Informações

1. **Detalhes técnicos nunca são expostos** ao usuário final
2. **Stack traces e nomes de assemblies** são ocultados
3. **Mensagens de banco de dados** são sanitizadas
4. **Caminhos de arquivos** não são revelados
5. **Informações de conexão** permanecem privadas

### Logging

Todos os erros são registrados no servidor com detalhes completos para diagnóstico, mas apenas mensagens sanitizadas são retornadas ao cliente.

## Exemplos de Uso

### Backend - Controller

```csharp
[HttpPost]
public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
{
    // Validação com mensagens em português
    if (!ModelState.IsValid)
        return BadRequest(ValidationHelper.GetValidationErrors(ModelState));

    try
    {
        var patient = await _service.CreatePatientAsync(dto, GetTenantId());
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }
    catch (InvalidOperationException ex)
    {
        // O middleware tratará esta exceção se não for capturada
        return BadRequest(new { message = ex.Message });
    }
}
```

### Frontend - Component

```typescript
onSubmit(): void {
  if (this.form.invalid) return;

  this.service.save(this.form.value).subscribe({
    next: () => this.router.navigate(['/success']),
    error: () => {
      // Interceptor já exibiu a mensagem de erro
      this.saving = false;
    }
  });
}
```

## Mensagens Traduzidas

### Mensagens Comuns em Português

- "Paciente não encontrado"
- "Usuário ou senha incorretos"
- "Você não tem permissão para realizar esta ação"
- "Os dados fornecidos são inválidos"
- "Este registro já existe no sistema"
- "Ocorreu um erro ao processar sua solicitação"
- "Sua sessão expirou. Por favor, faça login novamente"

## Testes

Para testar o tratamento de erros:

1. **Erro de validação**: Envie um POST sem campos obrigatórios
2. **Erro de autorização**: Tente acessar um recurso sem permissão
3. **Erro 404**: Busque um recurso inexistente
4. **Erro de servidor**: Simule uma exceção no serviço
5. **Erro de rede**: Desconecte a internet e tente uma operação

Em todos os casos, o usuário deve ver uma mensagem clara em português sem detalhes técnicos.

## Manutenção

### Adicionando Novas Traduções

**Backend** (`GlobalExceptionHandlerMiddleware.cs`):
```csharp
var commonMessages = new Dictionary<string, string>
{
    { "New English Message", "Nova Mensagem em Português" },
    // ...
};
```

**Frontend** (`error.interceptor.ts`):
```typescript
switch (error.status) {
  case 418: // Novo código
    return 'Nova mensagem em português';
  // ...
}
```

## Conclusão

Com estas mudanças, o sistema agora:

✅ Exibe mensagens de erro em português brasileiro  
✅ Oculta detalhes técnicos e de implementação  
✅ Fornece feedback claro e acionável ao usuário  
✅ Mantém logs detalhados no servidor para diagnóstico  
✅ Trata erros de forma consistente em toda a aplicação  
