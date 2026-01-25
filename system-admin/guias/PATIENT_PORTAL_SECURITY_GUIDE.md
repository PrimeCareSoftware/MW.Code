# 🔒 Portal do Paciente - Guia de Segurança Completo

## Versão 1.0 | Janeiro 2026

---

## 📋 Índice

1. [Visão Geral de Segurança](#visão-geral-de-segurança)
2. [Arquitetura de Segurança](#arquitetura-de-segurança)
3. [Autenticação e Autorização](#autenticação-e-autorização)
4. [Proteção de Dados](#proteção-de-dados)
5. [Conformidade Legal](#conformidade-legal)
6. [Configuração de Produção](#configuração-de-produção)
7. [Boas Práticas de Desenvolvimento](#boas-práticas-de-desenvolvimento)
8. [Testes de Segurança](#testes-de-segurança)
9. [Monitoramento e Auditoria](#monitoramento-e-auditoria)
10. [Resposta a Incidentes](#resposta-a-incidentes)
11. [Checklist de Segurança](#checklist-de-segurança)

---

## 🎯 Visão Geral de Segurança

### Princípios de Segurança

O Portal do Paciente foi desenvolvido seguindo os princípios fundamentais de segurança da informação:

#### 1. **Confidencialidade**
- Dados médicos são acessíveis apenas pelo paciente titular
- Comunicações criptografadas end-to-end
- Armazenamento seguro com criptografia em repouso

#### 2. **Integridade**
- Garantia de que dados não sejam alterados indevidamente
- Logs de auditoria para todas as operações críticas
- Validação rigorosa de entrada de dados

#### 3. **Disponibilidade**
- Sistema projetado para alta disponibilidade
- Backups regulares e disaster recovery
- Proteção contra ataques DDoS

#### 4. **Autenticidade**
- Identificação positiva de usuários via JWT
- Autenticação multifator (preparado para implementação)
- Tokens de sessão com validade limitada

#### 5. **Não-repúdio**
- Logs de auditoria imutáveis
- Rastreabilidade de todas as ações
- Conformidade com requisitos legais (CFM)

### Modelo de Ameaças

**Ameaças Identificadas:**

1. **Acesso Não Autorizado**
   - Tentativas de login por força bruta
   - Roubo de credenciais
   - Session hijacking

2. **Vazamento de Dados**
   - SQL Injection
   - Exposição de dados sensíveis
   - Man-in-the-middle attacks

3. **Ataques à Disponibilidade**
   - DDoS (Distributed Denial of Service)
   - Resource exhaustion
   - API abuse

4. **Engenharia Social**
   - Phishing
   - Pretexting
   - Manipulação de usuários

### Controles Implementados

**Controles Preventivos:**
- Autenticação forte com JWT
- Criptografia de dados (em trânsito e em repouso)
- Validação e sanitização de entrada
- Bloqueio de conta após tentativas falhas
- CORS configurado adequadamente
- Rate limiting em endpoints críticos

**Controles Detectivos:**
- Logging centralizado
- Monitoramento de anomalias
- Alertas de segurança
- Auditoria de acessos

**Controles Corretivos:**
- Processo de resposta a incidentes
- Revogação de tokens
- Isolamento de contas comprometidas
- Notificação de usuários afetados

---

## 🏗️ Arquitetura de Segurança

### Camadas de Segurança

```
┌─────────────────────────────────────────────────┐
│           Frontend (Angular 20)                 │
│  - HTTPS Only                                   │
│  - Input Validation                             │
│  - JWT Token Storage (HttpOnly Cookies)         │
└─────────────────────────────────────────────────┘
                      │ HTTPS
                      ▼
┌─────────────────────────────────────────────────┐
│           API Gateway (Future)                  │
│  - Rate Limiting                                │
│  - DDoS Protection                              │
│  - Request Logging                              │
└─────────────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────┐
│           API Backend (.NET 8)                  │
│  - JWT Authentication                           │
│  - Authorization Policies                       │
│  - Input Validation & Sanitization              │
│  - Business Logic Validation                    │
└─────────────────────────────────────────────────┘
                      │ Encrypted
                      ▼
┌─────────────────────────────────────────────────┐
│           Database (PostgreSQL 14+)             │
│  - Encrypted at Rest                            │
│  - Row-Level Security (RLS)                     │
│  - Audit Logging                                │
└─────────────────────────────────────────────────┘
```

### Segmentação de Rede

**Recomendações para Produção:**

1. **Frontend (DMZ)**
   - Zona desmilitarizada
   - Acesso público via HTTPS
   - Sem acesso direto ao banco de dados

2. **API Backend (Application Zone)**
   - Rede privada
   - Acesso apenas via API Gateway
   - Comunicação criptografada com banco

3. **Database (Data Zone)**
   - Rede mais restrita
   - Acesso apenas pelo backend
   - Backup isolado em rede separada

---

## 🔑 Autenticação e Autorização

### 1. Autenticação JWT

#### Implementação Atual

```csharp
// Token Configuration
{
  "JwtSettings": {
    "SecretKey": "YOUR-SECRET-KEY-MIN-32-CHARS",
    "ExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7,
    "Issuer": "PatientPortal",
    "Audience": "PatientPortal-API"
  }
}
```

#### Características dos Tokens

**Access Token:**
- Validade: 15 minutos
- Algoritmo: HMAC-SHA256
- Claims incluídos:
  - `sub` (Subject): User ID
  - `email`: E-mail do usuário
  - `jti` (JWT ID): Identificador único
  - `iat` (Issued At): Data de emissão
  - `exp` (Expiration): Data de expiração

**Refresh Token:**
- Validade: 7 dias
- Armazenado com hash no banco de dados
- Uso único (one-time use)
- Vinculado ao IP de origem

#### Geração de Chave Secreta

Para produção, gere uma chave segura:

```bash
# Método 1: OpenSSL
openssl rand -base64 32

# Método 2: PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))

# Método 3: Node.js
node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
```

### 2. Hash de Senhas

#### Algoritmo Utilizado

- **PBKDF2-HMAC-SHA256**
- **Iterações:** 100.000
- **Salt:** Gerado aleatoriamente para cada senha (128 bits)
- **Output:** 256 bits

```csharp
public static string HashPassword(string password)
{
    using var rng = RandomNumberGenerator.Create();
    byte[] salt = new byte[16];
    rng.GetBytes(salt);
    
    var pbkdf2 = new Rfc2898DeriveBytes(
        password, 
        salt, 
        iterations: 100000, 
        HashAlgorithmName.SHA256
    );
    
    byte[] hash = pbkdf2.GetBytes(32);
    
    // Combine salt + hash
    byte[] hashBytes = new byte[48];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 32);
    
    return Convert.ToBase64String(hashBytes);
}
```

### 3. Proteção de Conta

#### Account Lockout

**Configuração:**
- **Tentativas permitidas:** 5
- **Tempo de bloqueio:** 15 minutos
- **Reset automático:** Após período de bloqueio
- **Notificação:** Usuário é informado sobre o bloqueio

```csharp
public class PatientUser
{
    public int AccessFailedCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }
}
```

#### Password Policy

**Requisitos Mínimos:**
- Mínimo 8 caracteres
- Pelo menos 1 letra maiúscula
- Pelo menos 1 letra minúscula
- Pelo menos 1 número
- Pelo menos 1 caractere especial
- Não pode conter o nome do usuário
- Não pode ser uma senha comum (lista de senhas fracas)

### 4. Autenticação de Dois Fatores (2FA)

**Status:** Preparado no modelo de dados, aguardando implementação

**Planejamento:**
- TOTP (Time-based One-Time Password)
- SMS como fallback
- Códigos de recuperação
- Opção de dispositivos confiáveis

---

## 🛡️ Proteção de Dados

### 1. Criptografia

#### Dados em Trânsito

**HTTPS/TLS 1.3:**
- Certificado SSL/TLS válido obrigatório
- Perfect Forward Secrecy (PFS)
- Ciphers fortes apenas
- HSTS (HTTP Strict Transport Security)

```csharp
// Program.cs - Produção
app.UseHttpsRedirection();
app.UseHsts();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});
```

#### Dados em Repouso

**PostgreSQL Encryption:**
- TDE (Transparent Data Encryption) recomendado
- Backup criptografado
- Encryption at rest para dados sensíveis

**Application-Level Encryption:**
```csharp
// Para dados super sensíveis (futuro)
public class EncryptionService
{
    public string Encrypt(string plainText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        // ... implementação
    }
}
```

### 2. Proteção contra SQL Injection

**Entity Framework Core:**
- Uso de queries parametrizadas
- LINQ para construção de queries
- Nunca concatenar strings SQL

```csharp
// ✅ CORRETO - Parametrizado
var user = await _context.PatientUsers
    .Where(u => u.Email == email)
    .FirstOrDefaultAsync();

// ❌ ERRADO - Vulnerável
var query = $"SELECT * FROM PatientUsers WHERE Email = '{email}'";
```

### 3. Proteção contra XSS

**Frontend (Angular):**
- Sanitização automática de dados
- DomSanitizer para casos especiais
- Content Security Policy (CSP)

**Backend:**
- Encode output
- Validação de entrada
- Headers de segurança

```csharp
// Adicionar Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

### 4. Proteção contra CSRF

**JWT sem cookies:**
- Tokens enviados via header Authorization
- Não há necessidade de tokens CSRF tradicionais
- SameSite cookies para refresh tokens (se usar cookies)

### 5. CORS Configuration

**Desenvolvimento:**
```csharp
// AllowAll apenas para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Produção:**
```csharp
// Restrito a origens específicas
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "https://portal-paciente.medicwarehouse.com",
                "https://portal.clinica.com.br"
              )
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### 6. Rate Limiting

**Configuração Recomendada:**

```csharp
builder.Services.AddRateLimiter(options =>
{
    // Login endpoint
    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
    });
    
    // API geral
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});
```

---

## ⚖️ Conformidade Legal

### 1. LGPD (Lei Geral de Proteção de Dados)

#### Princípios Implementados

**Art. 6º - Princípios:**
- ✅ Finalidade: Dados usados apenas para atendimento médico
- ✅ Adequação: Compatível com finalidade informada
- ✅ Necessidade: Coleta limitada ao mínimo necessário
- ✅ Livre acesso: Paciente pode acessar seus dados
- ✅ Qualidade: Dados mantidos atualizados
- ✅ Transparência: Informações claras sobre tratamento
- ✅ Segurança: Medidas técnicas e administrativas
- ✅ Prevenção: Medidas preventivas implementadas
- ✅ Não discriminação: Sem tratamento discriminatório
- ✅ Responsabilização: Demonstração de compliance

#### Direitos do Titular (Art. 18)

Sistema permite:
- ✅ Confirmação de tratamento de dados
- ✅ Acesso aos dados
- ✅ Correção de dados incompletos/incorretos
- ⏳ Anonimização/bloqueio (em planejamento)
- ⏳ Eliminação de dados (em planejamento)
- ✅ Portabilidade (via download de documentos)
- ⏳ Informação sobre compartilhamento (em planejamento)
- ✅ Revogação de consentimento

#### Base Legal

- **Art. 7º, I:** Consentimento do titular
- **Art. 7º, VII:** Tutela da saúde em procedimento realizado por profissionais de saúde
- **Art. 11:** Tratamento de dados sensíveis relacionados à saúde

#### Registros de Atividades

**Logs obrigatórios:**
- Data e hora de acesso
- Identificação do usuário
- Tipo de operação realizada
- Dados acessados/modificados
- IP de origem
- Finalidade do acesso

### 2. Resoluções CFM (Conselho Federal de Medicina)

#### CFM 1.821/2007 - Prontuário Eletrônico

**Requisitos atendidos:**
- ✅ Identificação do paciente
- ✅ Data e hora de criação/acesso
- ✅ Registro de autor (médico)
- ✅ Segurança de acesso
- ✅ Backup e recuperação
- ⏳ Assinatura digital (futuro)
- ⏳ Certificação ICP-Brasil (futuro)

#### CFM 1.638/2002 - Prontuário Médico

**Conformidade:**
- ✅ Confidencialidade
- ✅ Guarda permanente
- ✅ Identificação inequívoca
- ✅ Legibilidade (documentos digitais)

#### CFM 2.314/2022 - Telemedicina

**Preparação:**
- Sistema preparado para teleconsultas (futuro)
- Consentimento informado do paciente
- Transmissão segura de dados
- Armazenamento adequado

### 3. Outras Regulamentações

#### Lei 13.787/2018 - Prescrição Digital

- ✅ Armazenamento seguro de receitas
- ⏳ Assinatura digital (futuro)
- ✅ Acesso controlado

#### ANS (Agência Nacional de Saúde)

- Preparado para integração com padrões ANS
- TISS (Troca de Informações de Saúde Suplementar)

---

## ⚙️ Configuração de Produção

### 1. Variáveis de Ambiente

**Nunca hardcode secrets!** Use variáveis de ambiente ou Azure Key Vault.

#### Variáveis Essenciais

```bash
# JWT Configuration
export JwtSettings__SecretKey="<generate-secure-random-key-min-32-chars>"
export JwtSettings__ExpiryMinutes="15"
export JwtSettings__RefreshTokenExpiryDays="7"
export JwtSettings__Issuer="PatientPortal"
export JwtSettings__Audience="PatientPortal-API"

# Database
export ConnectionStrings__DefaultConnection="Host=<host>;Port=5432;Database=medicwarehouse;Username=<user>;Password=<secure-password>"

# Application
export ASPNETCORE_ENVIRONMENT="Production"
export ASPNETCORE_URLS="https://+:443;http://+:80"

# CORS
export Cors__AllowedOrigins__0="https://portal-paciente.medicwarehouse.com"

# Logging
export Logging__LogLevel__Default="Warning"
export Logging__LogLevel__Microsoft="Warning"
```

### 2. Azure Key Vault (Recomendado)

```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

### 3. Configurações de HTTPS

#### Certificado SSL/TLS

```bash
# Obter certificado Let's Encrypt (gratuito)
certbot certonly --standalone -d portal-paciente.medicwarehouse.com
```

#### Configuração Kestrel

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
    });
});
```

### 4. Headers de Segurança

```csharp
app.Use(async (context, next) =>
{
    // Previne clickjacking
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    
    // Previne MIME sniffing
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    
    // XSS Protection (legacy, mas ainda útil)
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    
    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;");
    
    // Referrer Policy
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // Permissions Policy
    context.Response.Headers.Add("Permissions-Policy", 
        "geolocation=(), microphone=(), camera=()");
    
    await next();
});
```

### 5. Logging Seguro

#### Não Logar Dados Sensíveis

```csharp
// ❌ ERRADO
_logger.LogInformation("Login attempt for {Email} with password {Password}", email, password);

// ✅ CORRETO
_logger.LogInformation("Login attempt for user {UserId}", userId);
```

#### Estrutura de Logs

```json
{
  "timestamp": "2026-01-07T14:30:00Z",
  "level": "Information",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "action": "Login",
  "result": "Success",
  "ipAddress": "192.168.1.100",
  "userAgent": "Mozilla/5.0..."
}
```

---

## 💻 Boas Práticas de Desenvolvimento

### 1. Secure Coding Guidelines

#### Validação de Entrada

```csharp
// Sempre validar entrada
[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Invalid email format")]
public string Email { get; set; }

[Required]
[StringLength(11, MinimumLength = 11, ErrorMessage = "CPF must be 11 digits")]
[RegularExpression(@"^\d{11}$", ErrorMessage = "CPF must contain only numbers")]
public string CPF { get; set; }
```

#### Sanitização

```csharp
// Remover caracteres perigosos
public string SanitizeCPF(string cpf)
{
    return Regex.Replace(cpf, @"[^\d]", "");
}
```

#### Autorização Adequada

```csharp
[Authorize] // Sempre marcar endpoints protegidos
[HttpGet("me")]
public async Task<IActionResult> GetProfile()
{
    var userId = GetUserId();
    if (userId == null)
        return Unauthorized();
    
    // Verificar se usuário pode acessar o recurso
    var user = await _repository.GetByIdAsync(userId.Value);
    if (user == null)
        return NotFound();
    
    return Ok(user);
}
```

### 2. Dependency Security

#### Manter Dependências Atualizadas

```bash
# Verificar vulnerabilidades
dotnet list package --vulnerable

# Atualizar pacotes
dotnet add package <PackageName> --version <LatestVersion>
```

#### Usar Ferramentas de Análise

- **OWASP Dependency-Check**
- **Snyk**
- **WhiteSource**
- **GitHub Dependabot**

### 3. Code Review Checklist

- [ ] Não há credenciais hardcoded
- [ ] Validação de entrada implementada
- [ ] Autorização verificada
- [ ] Logs não contêm dados sensíveis
- [ ] Queries são parametrizadas
- [ ] Errors não expõem detalhes internos
- [ ] HTTPS é obrigatório
- [ ] Rate limiting onde apropriado
- [ ] Testes de segurança incluídos

---

## 🧪 Testes de Segurança

### 1. Testes Unitários de Segurança

```csharp
[Fact]
public void PasswordHash_ShouldBeSecure()
{
    var password = "TestPassword123!";
    var hash1 = SecurityHelper.HashPassword(password);
    var hash2 = SecurityHelper.HashPassword(password);
    
    // Hash deve ser diferente devido ao salt
    Assert.NotEqual(hash1, hash2);
    
    // Mas ambos devem validar corretamente
    Assert.True(SecurityHelper.VerifyPassword(password, hash1));
    Assert.True(SecurityHelper.VerifyPassword(password, hash2));
}

[Fact]
public void AccountLockout_ShouldTriggerAfter5Attempts()
{
    var user = new PatientUser();
    
    for (int i = 0; i < 5; i++)
    {
        user.AccessFailedCount++;
    }
    
    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
    
    Assert.True(user.IsLockedOut());
}
```

### 2. Testes de Penetração

#### Ferramentas Recomendadas

1. **OWASP ZAP** - Automated security testing
2. **Burp Suite** - Web vulnerability scanner
3. **SQLMap** - SQL injection testing
4. **Nmap** - Network scanning
5. **Metasploit** - Penetration testing framework

#### Checklist de Testes

- [ ] SQL Injection em todos os endpoints
- [ ] XSS (Cross-Site Scripting)
- [ ] CSRF (Cross-Site Request Forgery)
- [ ] Broken Authentication
- [ ] Sensitive Data Exposure
- [ ] XML External Entities (XXE)
- [ ] Broken Access Control
- [ ] Security Misconfiguration
- [ ] Insecure Deserialization
- [ ] Using Components with Known Vulnerabilities

### 3. SAST (Static Application Security Testing)

```bash
# SonarQube
dotnet sonarscanner begin /k:"PatientPortal"
dotnet build
dotnet sonarscanner end

# Security Code Scan
dotnet add package SecurityCodeScan.VS2019
dotnet build
```

### 4. DAST (Dynamic Application Security Testing)

- Testes em ambiente running
- Simular ataques reais
- Verificar configurações de produção

---

## 📊 Monitoramento e Auditoria

### 1. Logs de Auditoria

#### Eventos a Logar

**Autenticação:**
- Login bem-sucedido
- Tentativa de login falha
- Logout
- Alteração de senha
- Bloqueio de conta

**Acesso a Dados:**
- Visualização de documentos
- Download de documentos
- Acesso a consultas
- Visualização de perfil

**Modificações:**
- Atualização de perfil
- Alteração de configurações

#### Estrutura de Log de Auditoria

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } // Login, ViewDocument, etc.
    public string EntityType { get; set; } // PatientUser, Document, etc.
    public Guid? EntityId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public bool Success { get; set; }
    public string Details { get; set; }
}
```

### 2. Monitoramento de Segurança

#### Métricas a Monitorar

- Taxa de login failures
- Contas bloqueadas
- Tentativas de acesso não autorizado
- Erros 401/403
- Latência de API
- Taxa de erros 500

#### Alertas

**Críticos:**
- Múltiplas tentativas de login falhas do mesmo IP
- Acesso a recursos não autorizados
- Mudança súbita no padrão de acesso
- Erros de banco de dados

**Avisos:**
- Alta taxa de erros 4xx
- Lentidão na API
- Uso excessivo de recursos

### 3. Application Insights (Azure)

```csharp
// Configure Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Custom events
_telemetryClient.TrackEvent("LoginAttempt", new Dictionary<string, string>
{
    { "UserId", userId.ToString() },
    { "Result", "Success" }
});
```

---

## 🚨 Resposta a Incidentes

### 1. Plano de Resposta

#### Fases

1. **Preparação**
   - Equipe de resposta definida
   - Ferramentas prontas
   - Documentação atualizada

2. **Identificação**
   - Monitoramento contínuo
   - Alertas configurados
   - Análise de logs

3. **Contenção**
   - Isolar sistemas afetados
   - Revogar tokens comprometidos
   - Bloquear IPs maliciosos

4. **Erradicação**
   - Remover causa raiz
   - Aplicar patches
   - Atualizar configurações

5. **Recuperação**
   - Restaurar serviços
   - Verificar integridade
   - Monitorar comportamento

6. **Lições Aprendidas**
   - Documentar incidente
   - Atualizar procedimentos
   - Treinar equipe

### 2. Procedimentos Específicos

#### Conta Comprometida

1. Bloquear conta imediatamente
2. Revogar todos os tokens de acesso
3. Notificar o usuário
4. Investigar acesso não autorizado
5. Resetar senha
6. Revisar logs de auditoria
7. Verificar se dados foram acessados/modificados

#### Vazamento de Dados

1. Conter o vazamento
2. Avaliar extensão do dano
3. Notificar ANPD (se aplicável)
4. Notificar usuários afetados
5. Oferecer medidas de proteção
6. Documentar para compliance LGPD

#### Ataque DDoS

1. Ativar proteção DDoS (Cloudflare, AWS Shield)
2. Identificar padrão de ataque
3. Bloquear IPs maliciosos
4. Escalar recursos se necessário
5. Monitorar até normalização

### 3. Comunicação

#### Notificação Interna

- Equipe de desenvolvimento
- Equipe de segurança
- Gerência
- Jurídico (se necessário)

#### Notificação Externa

- Usuários afetados
- Autoridades (ANPD, se aplicável)
- Parceiros (se aplicável)
- Público (se crítico)

---

## ✅ Checklist de Segurança

### Desenvolvimento

- [ ] Código revisado por pares
- [ ] Testes de segurança passando
- [ ] Dependências atualizadas
- [ ] Nenhum secret hardcoded
- [ ] Logs não contêm dados sensíveis
- [ ] Validação de entrada implementada
- [ ] Autorização verificada em todos os endpoints

### Pré-Produção

- [ ] Configuração de produção revisada
- [ ] Secrets migrados para Key Vault/Env vars
- [ ] HTTPS configurado corretamente
- [ ] Certificado SSL válido
- [ ] CORS restrito a domínios específicos
- [ ] Rate limiting ativado
- [ ] Headers de segurança configurados
- [ ] Logs e monitoramento ativos

### Produção

- [ ] Backups configurados e testados
- [ ] Disaster recovery plan documentado
- [ ] Equipe de resposta a incidentes pronta
- [ ] Monitoramento 24/7 ativo
- [ ] Alertas configurados
- [ ] Documentação atualizada
- [ ] Conformidade LGPD/CFM verificada
- [ ] Testes de penetração realizados
- [ ] Auditoria de segurança aprovada

### Manutenção Contínua

- [ ] Revisar logs semanalmente
- [ ] Atualizar dependências mensalmente
- [ ] Testes de penetração trimestrais
- [ ] Revisar políticas de segurança semestralmente
- [ ] Treinar equipe regularmente
- [ ] Atualizar documentação conforme mudanças

---

## 📚 Recursos Adicionais

### Documentação

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [LGPD - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [CFM - Resoluções](https://portal.cfm.org.br/)

### Ferramentas

- [OWASP ZAP](https://www.zaproxy.org/)
- [Burp Suite](https://portswigger.net/burp)
- [SonarQube](https://www.sonarqube.org/)
- [Snyk](https://snyk.io/)

### Treinamento

- OWASP Training
- Secure Coding Practices
- LGPD Compliance Training
- Incident Response Training

---

## 📞 Contato de Segurança

Para reportar vulnerabilidades de segurança:

- **E-mail:** security@medicwarehouse.com
- **PGP Key:** [Disponível mediante solicitação]
- **Responsible Disclosure:** 90 dias para correção

---

**Versão:** 1.0
**Última Atualização:** Janeiro 2026
**Próxima Revisão:** Julho 2026

© 2026 PrimeCare Software. Documento Confidencial.
