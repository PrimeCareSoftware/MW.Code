# Guia de Segurança - MedicWarehouse

## 📋 Visão Geral

Este documento descreve todas as medidas de segurança implementadas no sistema MedicWarehouse para proteger contra ataques comuns como SQL Injection, XSS, CSRF, e outras ameaças.

## 🔒 Medidas de Segurança Implementadas

### 1. Autenticação e Autorização

#### JWT (JSON Web Tokens)
- **Algoritmo**: HMAC-SHA256
- **Tamanho Mínimo da Chave**: 32 caracteres (256 bits)
- **Tempo de Expiração**: 60 minutos
- **Validação Completa**:
  - Validação de assinatura
  - Validação de emissor (Issuer)
  - Validação de audiência (Audience)
  - Validação de tempo de vida
  - Zero tolerância de clock skew

```csharp
// Configuração no Program.cs
ValidateIssuerSigningKey = true,
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ClockSkew = TimeSpan.Zero
```

#### Hashing de Senhas
- **Algoritmo**: BCrypt
- **Work Factor**: 12 (iterações)
- **Salt**: Gerado automaticamente por senha
- **Proteção**: Cada hash é único mesmo para senhas idênticas

#### Requisitos de Senha Forte
- Mínimo 8 caracteres em desenvolvimento, 12 em produção
- Pelo menos 1 letra maiúscula
- Pelo menos 1 letra minúscula
- Pelo menos 1 dígito
- Pelo menos 1 caractere especial
- Não pode conter padrões fracos comuns (ex: "Password", "12345678")

### 2. Proteção contra Injeção

#### SQL Injection
**Proteção**: Entity Framework Core com queries parametrizadas

- ✅ **Todas as queries usam parâmetros**
- ✅ **Nenhuma query raw SQL no código**
- ✅ **LINQ para todas as operações de banco**
- ✅ **Sanitização adicional em camada de defesa profunda**

```csharp
// Exemplo de query segura
return await _dbSet
    .Where(p => p.Document == document && p.TenantId == tenantId)
    .FirstOrDefaultAsync();
```

#### XSS (Cross-Site Scripting)
**Proteção**: Múltiplas camadas

1. **Backend**:
   - HTML encoding automático
   - Sanitização de inputs
   - Content-Security-Policy headers

2. **Frontend (Angular)**:
   - Sanitização automática de templates
   - Binding seguro de dados
   - Validação de inputs

```csharp
// Sanitização de HTML
var sanitized = InputSanitizer.SanitizeHtml(userInput);

// Remoção completa de HTML
var stripped = InputSanitizer.StripHtml(userInput);
```

### 3. Headers de Segurança

Implementados via `SecurityHeadersMiddleware`:

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Content-Security-Policy: (configuração restritiva)
Permissions-Policy: (bloqueio de recursos sensíveis)
```

#### Content Security Policy (CSP)
```
default-src 'self';
script-src 'self' 'unsafe-inline' 'unsafe-eval';
style-src 'self' 'unsafe-inline';
img-src 'self' data: https:;
font-src 'self' data:;
connect-src 'self';
frame-ancestors 'none';
```

### 4. CORS (Cross-Origin Resource Sharing)

#### Desenvolvimento
```json
{
  "AllowedOrigins": [
    "http://localhost:4200",
    "http://localhost:4201",
    "http://localhost:3000"
  ]
}
```

#### Produção
```json
{
  "AllowedOrigins": [
    "https://medicwarehouse.com",
    "https://www.medicwarehouse.com",
    "https://app.medicwarehouse.com"
  ]
}
```

### 5. Rate Limiting

Proteção contra ataques de força bruta e DDoS:

#### Desenvolvimento
- 100 requisições por minuto por usuário/IP
- Sem fila de espera

#### Produção
- 10 requisições por minuto por usuário/IP  
- Sem fila de espera
- Status 429 (Too Many Requests) quando excedido

```csharp
// Configuração
options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
    httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
        factory: partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 10,
            QueueLimit = 0,
            Window = TimeSpan.FromSeconds(60)
        }));
```

### 6. HTTPS e Transport Security

#### Desenvolvimento
- HTTPS opcional (para facilitar testes locais)
- RequireHttpsMetadata = false

#### Produção
- **HTTPS obrigatório**
- HSTS (HTTP Strict Transport Security) habilitado
- RequireHttpsMetadata = true
- Certificados SSL/TLS validados

### 7. Isolamento Multi-tenant

**Query Filters Globais** para garantir isolamento de dados:

```csharp
// Aplicado automaticamente a todas as queries
modelBuilder.Entity<Patient>()
    .HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
```

**Garantias**:
- ✅ Todos os prontuários possuem TenantId
- ✅ Queries automáticas filtram por TenantId
- ✅ Não há endpoints cross-tenant
- ✅ Isolamento completo entre clínicas

### 8. Sanitização de Inputs

#### Funcionalidades Disponíveis

```csharp
// HTML
var safe = InputSanitizer.SanitizeHtml(input);
var text = InputSanitizer.StripHtml(input);

// Email
var (isValid, email) = InputSanitizer.SanitizeEmail(input);

// Telefone
var phone = InputSanitizer.SanitizePhoneNumber(input);

// URL
var (isValid, url) = InputSanitizer.SanitizeUrl(input);

// Nome de arquivo
var filename = InputSanitizer.SanitizeFileName(input);

// Limite de tamanho
var limited = InputSanitizer.TrimAndLimit(input, maxLength);

// SQL (defesa adicional)
var sql = InputSanitizer.SanitizeSqlInput(input);
```

## 🚀 Configuração para Produção

### 1. Variáveis de Ambiente

Nunca commitar secrets no código. Use variáveis de ambiente:

```bash
export DB_SERVER="seu-servidor.database.windows.net"
export DB_NAME="MedicWarehouse"
export DB_USER="admin_user"
export DB_PASSWORD="SuaSenhaSegura123!"
export JWT_SECRET_KEY="SuaChaveSecretaComPeloMenos32Caracteres!"
```

### 2. appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=False;MultipleActiveResultSets=true;Encrypt=True"
  },
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "ExpiryMinutes": 60,
    "Issuer": "MedicWarehouse",
    "Audience": "MedicWarehouse-API"
  },
  "Security": {
    "RequireHttps": true,
    "MinPasswordLength": 12
  }
}
```

### 3. Azure Key Vault (Recomendado)

Para ambientes de produção, use Azure Key Vault:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

## 🧪 Testes de Segurança

### Cobertura de Testes

- ✅ 39 testes específicos de segurança
- ✅ Testes de password hashing
- ✅ Testes de sanitização de inputs
- ✅ Testes de validação de senha forte
- ✅ Testes de proteção contra XSS
- ✅ 100% dos testes passando

### Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas testes de segurança
dotnet test --filter Category=Security
```

## 📝 Checklist de Segurança

### Antes de Deploy em Produção

- [ ] Atualizar `appsettings.Production.json` com valores corretos
- [ ] Configurar variáveis de ambiente no servidor
- [ ] Gerar nova chave JWT (mínimo 32 caracteres)
- [ ] Configurar certificado SSL/TLS válido
- [ ] Revisar origens CORS permitidas
- [ ] Habilitar HTTPS obrigatório
- [ ] Configurar rate limiting adequado
- [ ] Validar isolamento multi-tenant
- [ ] Testar todos os endpoints com autenticação
- [ ] Revisar logs de segurança
- [ ] Configurar monitoramento de segurança
- [ ] Realizar pen-test básico

### Manutenção Contínua

- [ ] Atualizar dependências mensalmente
- [ ] Revisar logs de tentativas de acesso
- [ ] Monitorar rate limiting triggers
- [ ] Auditar mudanças em dados sensíveis
- [ ] Backup regular do banco de dados
- [ ] Testar restore de backup
- [ ] Revisar acessos de usuários
- [ ] Atualizar certificados SSL antes do vencimento

## 🔍 Monitoramento

### Métricas Importantes

1. **Tentativas de Login Falhadas**
   - Alertar após 5 tentativas falhadas
   - Lockout temporário após 10 tentativas

2. **Rate Limiting Triggers**
   - Monitorar IPs que atingem limites frequentemente
   - Possível ataque DDoS

3. **Queries Anormais**
   - Queries muito lentas
   - Volume anormal de queries
   - Tentativas de SQL injection

4. **Acessos Não Autorizados**
   - Tentativas de acesso a recursos protegidos
   - Tokens JWT inválidos ou expirados

## 🆘 Resposta a Incidentes

### Em Caso de Violação de Segurança

1. **Contenção Imediata**
   - Desabilitar conta comprometida
   - Revogar tokens ativos
   - Bloquear IPs suspeitos

2. **Investigação**
   - Analisar logs de acesso
   - Identificar extensão do comprometimento
   - Documentar evidências

3. **Recuperação**
   - Resetar credenciais comprometidas
   - Gerar novos tokens
   - Atualizar senhas afetadas

4. **Pós-Incidente**
   - Notificar usuários afetados
   - Revisar e melhorar medidas de segurança
   - Atualizar documentação

## 📚 Referências

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/security/)
- [BCrypt Best Practices](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)

## 📞 Suporte

Para questões de segurança críticas:
- Email: security@medicwarehouse.com
- Não divulgar vulnerabilidades publicamente
- Seguir processo de divulgação responsável

---

**Última Atualização**: 2025-10-10  
**Versão**: 1.0  
**Status**: Em Produção
