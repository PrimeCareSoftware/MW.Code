# 🛡️ Prompt: Melhorias de Segurança Diversas

## 📊 Status
- **Prioridade**: 🔥🔥 ALTA
- **Progresso**: 30% (Parcial - alguns recursos já implementados)
- **Esforço**: 3-4 meses | 1-2 devs
- **Prazo**: Q1-Q2/2025

## 🎯 Contexto

Implementar melhorias abrangentes de segurança para proteger o sistema contra ameaças modernas, incluindo ataques de força bruta, comprometimento de contas, injeções, DDoS, e vazamento de dados. Conjunto de 6 melhorias independentes que podem ser implementadas em paralelo.

## 📋 Melhorias Detalhadas

### 1. Bloqueio de Conta por Tentativas Falhadas ⚠️ PARCIAL

**Status**: ⚠️ 50% - Implementado no Portal do Paciente, falta no sistema principal  
**Esforço**: 2 semanas | 1 dev  
**Prazo**: Q1/2025

#### O que já existe
- ✅ Account lockout no Patient Portal (5 tentativas, 15min bloqueio)
- ✅ Password hashing PBKDF2 (100k iterações)

#### O que falta implementar

```csharp
// Entidade de Auditoria de Login
public class LoginAttempt : Entity
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime AttemptDate { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public bool IsSuccess { get; set; }
    public string FailureReason { get; set; }
}

// Entidade de Bloqueio
public class AccountLockout : Entity
{
    public Guid UserId { get; set; }
    public DateTime LockoutStart { get; set; }
    public DateTime LockoutEnd { get; set; }
    public int FailedAttempts { get; set; }
    public int LockoutLevel { get; set; } // 1, 2, 3, 4
    public bool IsActive { get; set; }
    public string Reason { get; set; }
}

// Service de Bloqueio
public class AccountLockoutService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    public async Task<bool> IsAccountLocked(string email)
    {
        var lockout = await _context.AccountLockouts
            .Where(l => l.User.Email == email && l.IsActive && l.LockoutEnd > DateTime.UtcNow)
            .FirstOrDefaultAsync();
        
        return lockout != null;
    }
    
    public async Task RecordFailedAttempt(string email, string ipAddress, string userAgent)
    {
        var userId = await GetUserIdByEmail(email);
        
        // Registrar tentativa
        var attempt = new LoginAttempt
        {
            UserId = userId,
            Email = email,
            AttemptDate = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = false,
            FailureReason = "Invalid password"
        };
        
        _context.LoginAttempts.Add(attempt);
        
        // Verificar tentativas recentes (últimos 15 minutos)
        var recentAttempts = await _context.LoginAttempts
            .Where(a => a.Email == email 
                     && !a.IsSuccess 
                     && a.AttemptDate > DateTime.UtcNow.AddMinutes(-15))
            .CountAsync();
        
        // Aplicar bloqueio progressivo
        if (recentAttempts >= 5)
        {
            await ApplyLockout(userId, recentAttempts);
        }
        
        await _context.SaveChangesAsync();
    }
    
    private async Task ApplyLockout(Guid userId, int failedAttempts)
    {
        // Bloqueio progressivo: 5min, 15min, 1h, 24h
        var lockoutDuration = failedAttempts switch
        {
            5 => TimeSpan.FromMinutes(5),
            10 => TimeSpan.FromMinutes(15),
            15 => TimeSpan.FromHours(1),
            _ => TimeSpan.FromHours(24)
        };
        
        var lockout = new AccountLockout
        {
            UserId = userId,
            LockoutStart = DateTime.UtcNow,
            LockoutEnd = DateTime.UtcNow.Add(lockoutDuration),
            FailedAttempts = failedAttempts,
            LockoutLevel = (failedAttempts / 5),
            IsActive = true,
            Reason = $"Múltiplas tentativas de login falhadas ({failedAttempts})"
        };
        
        _context.AccountLockouts.Add(lockout);
        
        // Notificar usuário por email
        await _emailService.SendAccountLockedEmail(userId, lockoutDuration);
    }
}
```

**Checklist:**
- [ ] Implementar AccountLockoutService no sistema principal
- [ ] Criar entidades LoginAttempt e AccountLockout
- [ ] Integrar no AuthenticationService
- [ ] Adicionar migrations
- [ ] Criar email de notificação de bloqueio
- [ ] Dashboard admin para visualizar tentativas falhadas
- [ ] Endpoint admin para desbloquear conta manualmente
- [ ] Testes unitários (≥ 80% cobertura)

---

### 2. MFA Obrigatório para Administradores ⚠️ PARCIAL

**Status**: ⚠️ 40% - 2FA existe apenas em recuperação de senha  
**Esforço**: 2 semanas | 1 dev  
**Prazo**: Q1/2025

#### O que já existe
- ✅ 2FA por SMS (recuperação de senha)
- ✅ 2FA por Email (recuperação de senha)

#### O que falta implementar

```csharp
// Entidade de MFA
public class MfaDevice : Entity
{
    public Guid UserId { get; set; }
    public MfaType Type { get; set; } // TOTP, SMS, Email, U2F
    public string DeviceName { get; set; }
    public string Secret { get; set; } // Para TOTP
    public string PhoneNumber { get; set; } // Para SMS
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

public enum MfaType
{
    TOTP,       // Google Authenticator, Microsoft Authenticator
    SMS,        // Código por SMS
    Email,      // Código por Email
    U2F,        // YubiKey, FIDO2
    BackupCode  // Códigos de backup descartáveis
}

// Service de MFA
public class MfaService
{
    public async Task<MfaSetupDto> SetupTOTP(Guid userId, string deviceName)
    {
        // Gerar secret
        var secret = GenerateTOTPSecret();
        
        var device = new MfaDevice
        {
            UserId = userId,
            Type = MfaType.TOTP,
            DeviceName = deviceName,
            Secret = secret,
            IsVerified = false,
            IsActive = false
        };
        
        await _context.MfaDevices.AddAsync(device);
        await _context.SaveChangesAsync();
        
        // Gerar QR Code para app authenticator
        var qrCode = GenerateQRCode(secret, deviceName);
        
        return new MfaSetupDto
        {
            Secret = secret,
            QRCodeUrl = qrCode,
            BackupCodes = GenerateBackupCodes(userId)
        };
    }
    
    public async Task<bool> VerifyTOTP(Guid userId, string code)
    {
        var device = await _context.MfaDevices
            .Where(d => d.UserId == userId && d.Type == MfaType.TOTP && d.IsActive)
            .FirstOrDefaultAsync();
        
        if (device == null)
            return false;
        
        var totp = new Totp(Encoding.UTF8.GetBytes(device.Secret));
        var isValid = totp.VerifyTotp(code, out long timeStepMatched, 
                                      new VerificationWindow(2, 2));
        
        if (isValid)
        {
            device.LastUsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        
        return isValid;
    }
    
    private List<string> GenerateBackupCodes(Guid userId, int count = 10)
    {
        var codes = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            var code = GenerateRandomCode(8);
            codes.Add(code);
            
            // Armazenar hash do código
            var backupCode = new MfaDevice
            {
                UserId = userId,
                Type = MfaType.BackupCode,
                Secret = HashCode(code),
                IsVerified = true,
                IsActive = true
            };
            
            _context.MfaDevices.Add(backupCode);
        }
        
        return codes;
    }
}

// Integração no Login
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userService.ValidateCredentials(request.Email, request.Password);
    
    if (user == null)
        return Unauthorized();
    
    // Verificar se MFA é obrigatório
    if (user.IsAdmin || user.RequiresMfa)
    {
        var hasMfa = await _mfaService.UserHasMfaEnabled(user.Id);
        
        if (!hasMfa)
        {
            return BadRequest(new { 
                error = "MFA_REQUIRED", 
                message = "Configure MFA antes de fazer login" 
            });
        }
        
        // Gerar token temporário para MFA challenge
        var tempToken = GenerateTempToken(user.Id);
        
        return Ok(new { 
            requiresMfa = true, 
            tempToken,
            mfaMethods = await _mfaService.GetUserMfaMethods(user.Id)
        });
    }
    
    // Login normal sem MFA
    var tokens = await GenerateTokens(user);
    return Ok(tokens);
}

[HttpPost("verify-mfa")]
public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaRequest request)
{
    var userId = ValidateTempToken(request.TempToken);
    
    if (userId == Guid.Empty)
        return Unauthorized();
    
    var isValid = await _mfaService.VerifyCode(userId, request.Code, request.Method);
    
    if (!isValid)
        return BadRequest(new { error = "Invalid MFA code" });
    
    var user = await _userService.GetById(userId);
    var tokens = await GenerateTokens(user);
    
    return Ok(tokens);
}
```

**Bibliotecas:**
- `Otp.NET` - Para TOTP/HOTP
- `QRCoder` - Para gerar QR codes
- `Yubico.Core` - Para YubiKey/U2F

**Checklist:**
- [ ] Implementar MfaService completo
- [ ] Criar entidade MfaDevice
- [ ] Integração TOTP (Google/Microsoft Authenticator)
- [ ] Suporte a códigos de backup
- [ ] Frontend para configurar MFA
- [ ] Frontend para challenge MFA no login
- [ ] Endpoint para listar dispositivos MFA
- [ ] Endpoint para remover dispositivo MFA
- [ ] Forçar MFA para admins e owners
- [ ] Testes unitários e de integração

---

### 3. WAF (Web Application Firewall) ❌ NÃO INICIADO

**Status**: ❌ 0%  
**Esforço**: 1 mês | 1 dev + DevOps  
**Prazo**: Q2/2025

#### Recomendação: Cloudflare WAF

**Benefícios:**
- ✅ Proteção contra OWASP Top 10
- ✅ DDoS mitigation automático
- ✅ Rate limiting avançado
- ✅ Geo-blocking
- ✅ Bot detection (bons e maus)
- ✅ CDN incluído (performance)
- ✅ SSL/TLS automático
- ✅ Logs e analytics

**Planos:**
- Free: $0/mês - Proteção básica
- Pro: $20/mês - WAF + Analytics
- Business: $200/mês - WAF avançado + suporte
- Enterprise: Custom - DDoS ilimitado + SLA

#### Implementação

**1. Configuração Cloudflare:**
```bash
# DNS apontando para Cloudflare
omnicare.com.br -> Cloudflare Proxy -> Origin Server

# Regras WAF (Cloudflare Dashboard)
- OWASP Core Rule Set (CRS 3.3)
- Rate limiting: 100 req/min por IP
- Challenge em países suspeitos
- Block em User-Agents maliciosos
- SQL Injection patterns
- XSS patterns
```

**2. Rate Limiting Customizado:**
```javascript
// Cloudflare Workers (JavaScript)
addEventListener('fetch', event => {
  event.respondWith(handleRequest(event.request))
})

async function handleRequest(request) {
  const ip = request.headers.get('CF-Connecting-IP')
  const url = new URL(request.url)
  
  // Rate limit específico para login
  if (url.pathname === '/api/auth/login') {
    const rateLimit = await checkRateLimit(ip, 'login', 5, 60) // 5 req/min
    
    if (!rateLimit.allowed) {
      return new Response('Too many requests', { 
        status: 429,
        headers: {
          'Retry-After': rateLimit.retryAfter
        }
      })
    }
  }
  
  return fetch(request)
}
```

**Alternativas:**
- **AWS WAF**: Integrado com AWS (se usar AWS)
- **Azure WAF**: Integrado com Azure (se usar Azure)
- **ModSecurity**: Open source (self-hosted)

**Checklist:**
- [ ] Contratar Cloudflare (Pro ou Business)
- [ ] Configurar DNS para usar Cloudflare
- [ ] Ativar WAF com OWASP CRS
- [ ] Configurar rate limiting
- [ ] Configurar geo-blocking (se necessário)
- [ ] Configurar challenge para bots
- [ ] Testar proteção contra SQL Injection
- [ ] Testar proteção contra XSS
- [ ] Configurar alertas de ataques
- [ ] Documentar regras implementadas

---

### 4. SIEM para Centralização de Logs ❌ NÃO INICIADO

**Status**: ❌ 0%  
**Esforço**: 1 mês | 1 dev  
**Prazo**: Q2/2025

#### Recomendação: Serilog + Seq

**Arquitetura:**
```
[Aplicação] -> Serilog -> [Seq] -> Dashboards/Alertas
                       -> [Elasticsearch] -> Kibana (opcional)
```

#### Implementação

**1. Configurar Serilog:**
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "Omni Care")
    .Enrich.WithProperty("Environment", environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.Seq("http://seq-server:5341", apiKey: seqApiKey)
    .WriteTo.File("logs/omnicare-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

**2. Structured Logging:**
```csharp
// Em vez de:
_logger.LogInformation($"User {userId} logged in");

// Fazer:
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ipAddress);

// Security events
_logger.LogWarning("Failed login attempt for {Email} from {IpAddress}", email, ip);
_logger.LogCritical("Potential SQL injection detected: {Query} from {IpAddress}", query, ip);
```

**3. Seq Queries e Alertas:**
```sql
-- Alertas importantes no Seq

-- 1. Múltiplas tentativas de login falhadas
SELECT COUNT(*) FROM Stream
WHERE @EventType = 'FailedLogin'
  AND @Timestamp > Now() - 5m
GROUP BY IpAddress
HAVING COUNT(*) > 10

-- 2. Erros críticos
SELECT * FROM Stream
WHERE @Level = 'Fatal' OR @Level = 'Error'
  AND @Timestamp > Now() - 1h

-- 3. Acessos suspeitos
SELECT * FROM Stream
WHERE @EventType = 'UnauthorizedAccess'
  AND @Timestamp > Now() - 1h
```

**Checklist:**
- [ ] Instalar Seq (Docker ou self-hosted)
- [ ] Configurar Serilog em todos os projetos
- [ ] Implementar structured logging
- [ ] Configurar níveis de log por ambiente
- [ ] Criar queries importantes no Seq
- [ ] Configurar alertas (email/Slack/Teams)
- [ ] Dashboard de segurança no Seq
- [ ] Rotação de logs (retenção 90 dias)
- [ ] Backup de logs críticos
- [ ] Documentar convenções de logging

---

### 5. Refresh Token Pattern ✅ IMPLEMENTADO

**Status**: ✅ 100% - Já implementado no Patient Portal  
**Esforço**: 0 (apenas replicar para sistema principal)  
**Prazo**: Q1/2025

#### O que já existe

- ✅ Access Token curta duração (15 min)
- ✅ Refresh Token longa duração (7 dias)
- ✅ Endpoint /refresh para renovar
- ✅ Rotação de refresh tokens
- ✅ Revogação de tokens

#### Replicar para sistema principal (se necessário)

---

### 6. Pentest Profissional Semestral ❌ NÃO INICIADO

**Status**: ❌ 0% - Contratar em Q2/2025  
**Esforço**: Contratação externa  
**Prazo**: Q2/2025 (primeiro pentest)

#### Escopo Sugerido

**1. OWASP Top 10:**
- Broken Access Control
- Cryptographic Failures
- Injection (SQL, NoSQL, Command)
- Insecure Design
- Security Misconfiguration
- Vulnerable Components
- Authentication Failures
- Data Integrity Failures
- Logging Failures
- SSRF (Server-Side Request Forgery)

**2. API Security:**
- Broken Object Level Authorization
- Broken Authentication
- Excessive Data Exposure
- Lack of Resources & Rate Limiting
- Broken Function Level Authorization
- Mass Assignment
- Security Misconfiguration
- Injection
- Improper Assets Management
- Insufficient Logging

**3. Infraestrutura:**
- Network security
- Server hardening
- Container security (Docker/Kubernetes)
- Database security

#### Empresas Recomendadas (Brasil)

- **Morphus Labs**: R$ 20-40k
- **Clavis**: R$ 15-30k
- **E-VAL**: R$ 18-35k
- **Tempest**: R$ 25-50k
- **Conviso**: R$ 20-40k

**Frequência:** Semestral ou anual

**Checklist:**
- [ ] Definir escopo de pentest
- [ ] Solicitar orçamentos (3 empresas)
- [ ] Aprovar orçamento
- [ ] Agendar pentest
- [ ] Preparar ambiente de testes
- [ ] Executar pentest
- [ ] Analisar relatório
- [ ] Priorizar correções
- [ ] Implementar correções críticas
- [ ] Re-testar vulnerabilidades corrigidas

---

## 📚 Referências

- [PENDING_TASKS.md - Seção Segurança](../../PENDING_TASKS.md#7-melhorias-de-segurança-diversas)
- [SUGESTOES_MELHORIAS_SEGURANCA.md](../../SUGESTOES_MELHORIAS_SEGURANCA.md)
- [OWASP Top 10 2021](https://owasp.org/www-project-top-ten/)
- [OWASP API Security Top 10](https://owasp.org/www-project-api-security/)
- [Cloudflare WAF Documentation](https://developers.cloudflare.com/waf/)
- [Seq Documentation](https://docs.datalust.co/docs)

## 💰 Investimento

- **Account Lockout**: 2 semanas, 1 dev = R$ 15k
- **MFA**: 2 semanas, 1 dev = R$ 15k
- **WAF (Cloudflare Pro)**: R$ 20/mês = R$ 240/ano
- **SIEM (Seq)**: Self-hosted gratuito ou R$ 50/mês
- **Pentest**: R$ 20-40k semestral
- **TOTAL Desenvolvimento**: R$ 30k
- **TOTAL Recorrente**: R$ 50-90k/ano

## ✅ Critérios de Aceitação

### 1. Account Lockout
- [ ] Conta bloqueia após 5 tentativas falhadas
- [ ] Bloqueio progressivo (5min, 15min, 1h, 24h)
- [ ] Usuário recebe email de notificação
- [ ] Admin pode desbloquear manualmente
- [ ] Logs de tentativas falhadas disponíveis

### 2. MFA
- [ ] MFA obrigatório para admins
- [ ] Suporte a TOTP (Google/Microsoft Authenticator)
- [ ] Códigos de backup disponíveis
- [ ] Usuário pode gerenciar dispositivos MFA
- [ ] Teste de MFA funciona corretamente

### 3. WAF
- [ ] Cloudflare WAF configurado
- [ ] Proteção OWASP CRS ativa
- [ ] Rate limiting em endpoints críticos
- [ ] Dashboards de ataques disponíveis
- [ ] Alertas de ataques configurados

### 4. SIEM
- [ ] Serilog configurado em todos os projetos
- [ ] Logs centralizados no Seq
- [ ] Structured logging implementado
- [ ] Alertas críticos configurados
- [ ] Retenção de 90 dias

### 5. Pentest
- [ ] Primeiro pentest executado
- [ ] Relatório recebido e analisado
- [ ] Vulnerabilidades críticas corrigidas
- [ ] Próximo pentest agendado (6-12 meses)

---

**Última Atualização**: Janeiro 2026  
**Status**: Parcial (30%) - Priorizar Account Lockout e MFA  
**Próximo Passo**: Implementar Account Lockout no sistema principal
