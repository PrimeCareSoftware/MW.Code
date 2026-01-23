# 🔒 Melhorias de Segurança - Bundle Completo

**Prioridade:** 🔥🔥 P1 - ALTA  
**Impacto:** Alto - Segurança Crítica  
**Status Atual:** 0% completo  
**Esforço:** 3 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 45.000  
**Prazo:** Q1-Q2 2026 (Janeiro-Junho)

## 📋 Contexto

Bundle de **6 melhorias de segurança essenciais** que elevam significativamente o nível de proteção contra ataques modernos: força bruta, phishing, DDoS e ameaças internas.

### Componentes

1. **Bloqueio de Conta** - Tentativas falhadas (2 semanas - R$ 7.5k)
2. **MFA Obrigatório** - Administradores (2 semanas - R$ 7.5k)
3. **WAF** - Web Application Firewall (1 mês - R$ 12k)
4. **SIEM** - Log Management (1 mês - R$ 12k)
5. **Refresh Tokens** - Padrão moderno (2 semanas - R$ 7.5k)
6. **Pentest Profissional** - Auditoria externa (R$ 15-30k)

### Por que é Prioridade Alta?

- **Proteção Real:** Bloqueia 95%+ dos ataques comuns
- **Compliance:** ISO 27001, OWASP Top 10
- **Detecção Precoce:** SIEM detecta ameaças em minutos
- **Mitigação de Riscos:** Reduz superfície de ataque drasticamente

## 🎯 Objetivos

Implementar defesas em múltiplas camadas que protejam contra força bruta, garantam autenticação forte, detectem ataques automaticamente, centralizem logs de segurança e permitam revogação granular de tokens.

## 1. Bloqueio de Conta (2 semanas)

### Entidades

```csharp
// src/MedicSoft.Core/Entities/Security/LoginAttempt.cs
public class LoginAttempt : BaseEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string IpAddress { get; set; }
    public DateTime AttemptTime { get; set; }
    public bool WasSuccessful { get; set; }
    public string FailureReason { get; set; }
}

public class AccountLockout : BaseEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime LockedAt { get; set; }
    public DateTime UnlocksAt { get; set; }
    public int FailedAttempts { get; set; }
    public bool IsActive { get; set; }
}
```

### Serviço

```csharp
public interface IBruteForceProtectionService
{
    Task<bool> IsAccountLockedAsync(string username);
    Task<bool> CanAttemptLoginAsync(string username, string ipAddress);
    Task RecordFailedAttemptAsync(string username, string ipAddress);
    Task RecordSuccessfulLoginAsync(string username);
}

public class BruteForceProtectionService : IBruteForceProtectionService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan[] LockoutDurations = {
        TimeSpan.FromMinutes(5),   // 1ª vez
        TimeSpan.FromMinutes(15),  // 2ª vez
        TimeSpan.FromHours(1),     // 3ª vez
        TimeSpan.FromHours(24)     // 4ª+ vez
    };
    
    public async Task RecordFailedAttemptAsync(string username, string ipAddress)
    {
        await _loginAttemptRepository.AddAsync(new LoginAttempt {
            Username = username,
            IpAddress = ipAddress,
            AttemptTime = DateTime.UtcNow,
            WasSuccessful = false
        });
        
        var recentFailed = await _loginAttemptRepository.GetAll()
            .Where(a => a.Username == username 
                && !a.WasSuccessful 
                && a.AttemptTime > DateTime.UtcNow.AddMinutes(-30))
            .CountAsync();
        
        if (recentFailed >= MaxFailedAttempts)
            await LockAccountAsync(username, recentFailed);
    }
}
```

## 2. MFA Obrigatório para Admins (2 semanas)

### Entidades

```csharp
public class TwoFactorAuth : BaseEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public bool IsEnabled { get; set; }
    public TwoFactorMethod Method { get; set; }  // TOTP, SMS, Email
    public string SecretKey { get; set; }  // Encrypted
    public List<BackupCode> BackupCodes { get; set; }
}

public class BackupCode
{
    public string Code { get; set; }  // Hashed
    public bool IsUsed { get; set; }
}
```

### Serviço TOTP

```csharp
public interface ITwoFactorAuthService
{
    Task<TwoFactorSetupInfo> EnableTOTPAsync(string userId);
    Task<bool> VerifyTOTPAsync(string userId, string code);
    Task<List<string>> GenerateBackupCodesAsync(string userId);
}

public class TwoFactorAuthService : ITwoFactorAuthService
{
    public async Task<TwoFactorSetupInfo> EnableTOTPAsync(string userId)
    {
        var secretKey = GenerateSecretKey();  // Base32, 20 bytes
        
        var twoFactor = new TwoFactorAuth {
            UserId = userId,
            SecretKey = _encryptionService.Encrypt(secretKey),
            Method = TwoFactorMethod.TOTP
        };
        
        await _repository.AddAsync(twoFactor);
        
        return new TwoFactorSetupInfo {
            SecretKey = secretKey,
            QRCodeUrl = $"otpauth://totp/PrimeCare:{email}?secret={secretKey}&issuer=PrimeCare"
        };
    }
    
    public async Task<bool> VerifyTOTPAsync(string userId, string code)
    {
        var twoFactor = await GetTwoFactorAsync(userId);
        var secretKey = _encryptionService.Decrypt(twoFactor.SecretKey);
        
        // Verificar código com tolerância ±1 time step (30s)
        return VerifyTOTPCode(secretKey, code);
    }
    
    private bool VerifyTOTPCode(string secretKey, string userCode)
    {
        var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        
        for (int i = -1; i <= 1; i++)
        {
            var expectedCode = GenerateTOTPCode(secretKey, currentStep + i);
            if (expectedCode == userCode) return true;
        }
        return false;
    }
}
```

## 3. WAF - Cloudflare (1 mês)

### Configuração

```yaml
# cloudflare-waf-rules.yaml
rules:
  - name: "Block SQL Injection"
    expression: |
      (http.request.uri.query contains "UNION" or
       http.request.uri.query contains "SELECT" or
       http.request.body contains "' OR '1'='1")
    action: block
    
  - name: "Block XSS"
    expression: |
      (http.request.uri.query contains "<script" or
       http.request.body contains "onerror=")
    action: block
    
  - name: "Rate Limit Login"
    expression: "http.request.uri.path eq '/api/auth/login'"
    action: rate_limit
    ratelimit:
      requests_per_period: 10
      period: 60
      
  - name: "Block Bad Bots"
    expression: |
      (http.user_agent contains "sqlmap" or
       http.user_agent contains "nikto")
    action: block
```

### Custo Cloudflare

- **Free:** $0 - DDoS básico
- **Pro:** $20/mês - WAF básico
- **Business:** $200/mês - WAF completo ⭐ Recomendado
- **Enterprise:** Personalizado - WAF avançado

## 4. SIEM - ELK Stack (1 mês)

### Docker Compose

```yaml
# docker-compose.elk.yml
version: '3.8'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=true
    ports:
      - "9200:9200"
    volumes:
      - elasticsearch-data:/usr/share/elasticsearch/data
  
  logstash:
    image: docker.elastic.co/logstash/logstash:8.11.0
    volumes:
      - ./logstash/pipeline:/usr/share/logstash/pipeline
    ports:
      - "5044:5044"
    depends_on:
      - elasticsearch
  
  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.0
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200
    ports:
      - "5601:5601"
    depends_on:
      - elasticsearch

volumes:
  elasticsearch-data:
```

### Logstash Pipeline

```ruby
# logstash/pipeline/api-logs.conf
input {
  beats { port => 5044 }
}

filter {
  json { source => "message" }
  date { match => ["timestamp", "ISO8601"] }
  
  geoip {
    source => "ip_address"
    target => "geoip"
  }
  
  if [message] =~ /login.*failed/i {
    mutate { add_tag => ["failed_login"] }
  }
}

output {
  elasticsearch {
    hosts => ["elasticsearch:9200"]
    index => "primecare-logs-%{+YYYY.MM.dd}"
  }
  
  if "critical" in [tags] {
    email {
      to => "security@primecare.com"
      subject => "🚨 Security Alert"
    }
  }
}
```

### Kibana Dashboards

```json
{
  "title": "Security Dashboard",
  "panels": [
    {
      "title": "Failed Logins (24h)",
      "type": "metric",
      "query": "tags:failed_login AND @timestamp:[now-24h TO now]"
    },
    {
      "title": "Login Map",
      "type": "map",
      "field": "geoip.location"
    },
    {
      "title": "Top Failed IPs",
      "type": "table",
      "query": "tags:failed_login",
      "aggregation": "terms",
      "field": "ip_address"
    }
  ]
}
```

## 5. Refresh Token Pattern (2 semanas)

### Entidades

```csharp
public class RefreshToken : BaseEntity
{
    public Guid Id { get; set; }
    public string Token { get; set; }  // Hashed
    public string UserId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string IpAddress { get; set; }
    public bool IsRevoked { get; set; }
    public string ReplacedByToken { get; set; }  // Token rotation
}
```

### Token Service

```csharp
public interface ITokenService
{
    TokenPair GenerateTokenPair(User user);
    Task<TokenPair> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task RevokeAllUserTokensAsync(string userId);
}

public class TokenService : ITokenService
{
    private readonly int AccessTokenMinutes = 15;
    private readonly int RefreshTokenDays = 7;
    
    public TokenPair GenerateTokenPair(User user)
    {
        var accessToken = _jwtService.GenerateToken(user, TimeSpan.FromMinutes(AccessTokenMinutes));
        var refreshTokenValue = GenerateRefreshTokenValue();
        
        var refreshToken = new RefreshToken {
            Token = HashToken(refreshTokenValue),
            UserId = user.Id,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays)
        };
        
        await _repository.AddAsync(refreshToken);
        
        return new TokenPair {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresIn = AccessTokenMinutes * 60
        };
    }
    
    public async Task<TokenPair> RefreshTokenAsync(string refreshTokenValue)
    {
        var tokenHash = HashToken(refreshTokenValue);
        var refreshToken = await _repository.GetAll()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == tokenHash);
        
        if (refreshToken == null || refreshToken.IsRevoked)
        {
            if (refreshToken?.IsRevoked == true)
            {
                // Token reuse - revogar todos os tokens do usuário
                await RevokeAllUserTokensAsync(refreshToken.UserId);
            }
            throw new SecurityException("Invalid token");
        }
        
        // Revogar token antigo (rotation)
        refreshToken.IsRevoked = true;
        await _repository.UpdateAsync(refreshToken);
        
        // Gerar novo par
        return GenerateTokenPair(refreshToken.User);
    }
}
```

### Frontend Interceptor

```typescript
// frontend/src/app/core/interceptors/token.interceptor.ts
export class TokenInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<any>(null);
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getAccessToken();
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
    
    return next.handle(req).pipe(
      catchError(error => {
        if (error.status === 401) {
          return this.handle401Error(req, next);
        }
        return throwError(() => error);
      })
    );
  }
  
  private handle401Error(req: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      
      return this.authService.refreshToken().pipe(
        switchMap(tokens => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(tokens.accessToken);
          return next.handle(this.addToken(req, tokens.accessToken));
        }),
        catchError(err => {
          this.authService.logout();
          return throwError(() => err);
        })
      );
    }
    
    return this.refreshTokenSubject.pipe(
      filter(token => token != null),
      take(1),
      switchMap(token => next.handle(this.addToken(req, token)))
    );
  }
}
```

## 6. Pentest Profissional

### Escopo

```markdown
## Scope - PrimeCare Security Testing

### In Scope
- Web Application: https://api.primecare.com
- Frontend: https://app.primecare.com
- Patient Portal: https://portal.primecare.com
- REST APIs: /api/*
- Authentication & Authorization
- File Upload
- Session Management

### Test Types
1. **OWASP Top 10** - SQL Injection, XSS, CSRF, etc.
2. **API Security** - Authentication, rate limiting
3. **Infrastructure** - SSL/TLS, headers, cookies

### Deliverables
- Executive Summary
- Technical Report with PoCs
- CVSS scores
- Remediation recommendations

### Timeline
- Week 1: Reconnaissance
- Week 2-3: Penetration testing
- Week 4: Documentation

### Cost
- Basic: R$ 15,000 (80 hours)
- Complete: R$ 30,000 (160 hours + retest)
```

### Empresas Recomendadas (Brasil)

| Empresa | Preço | Especialidade |
|---------|-------|---------------|
| **Morphus** | R$ 25-40k | Pentest completo |
| **Clavis** | R$ 15-30k | Web/API security |
| **Tempest** | R$ 30-50k | Enterprise |
| **Conviso** | R$ 25-45k | AppSec + DevSecOps |

## ✅ Critérios de Sucesso

- [ ] Zero ataques de força bruta bem-sucedidos
- [ ] 100% administradores com MFA
- [ ] WAF bloqueando >90% de ataques
- [ ] SIEM operacional com alertas
- [ ] Tokens revogáveis em <1s
- [ ] Pentest sem vulnerabilidades críticas

## 📦 Entregáveis

1. **Bloqueio** - LoginAttempt tracking, lockout management, email notifications
2. **MFA** - TOTP, QR codes, backup codes, admin enforcement
3. **WAF** - Cloudflare configurado com regras OWASP
4. **SIEM** - ELK Stack, dashboards, alertas automatizados
5. **Tokens** - Rotation, revogação, frontend interceptor
6. **Pentest** - Relatório completo e remediação

## 🔗 Dependências

- ✅ Sistema de autenticação
- ❌ Task #08 (Auditoria) - recomendado
- Cloudflare account (WAF)
- Docker (SIEM)

## 🧪 Testes

```bash
# Testar bloqueio
for i in {1..6}; do
  curl -X POST /api/auth/login -d '{"username":"test","password":"wrong"}'
done
# 6ª tentativa deve retornar 429

# Testar MFA
curl /api/auth/enable-2fa
# Escanear QR code e verificar código
```

## 📊 Métricas

- **Bloqueio:** 0 ataques bem-sucedidos
- **MFA:** 100% admins protegidos
- **WAF:** >90% bloqueio
- **SIEM:** <5 min detecção
- **Tokens:** <1s revogação

## 💰 Custos Totais

| Componente | Custo Implementação | Custo Mensal |
|------------|---------------------|--------------|
| Bloqueio | R$ 7.500 | R$ 0 |
| MFA | R$ 7.500 | R$ 0 |
| WAF | R$ 12.000 | R$ 200 (Cloudflare Business) |
| SIEM | R$ 12.000 | R$ 100 (infra) |
| Tokens | R$ 7.500 | R$ 0 |
| Pentest | R$ 15.000-30.000 | R$ 0 (semestral/anual) |
| **TOTAL** | **R$ 61.500-76.500** | **R$ 300/mês** |

## 📚 Referências

- [OWASP Top 10](https://owasp.org/Top10/)
- [Cloudflare WAF](https://www.cloudflare.com/waf/)
- [ELK Stack](https://www.elastic.co/elastic-stack/)
- [RFC 6238 - TOTP](https://tools.ietf.org/html/rfc6238)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)

---

> **IMPORTANTE:** Bundle de 6 melhorias críticas de segurança - proteção em múltiplas camadas  
> **ROI:** Previne milhões em prejuízos de security breaches  
> **Última Atualização:** 23 de Janeiro de 2026
