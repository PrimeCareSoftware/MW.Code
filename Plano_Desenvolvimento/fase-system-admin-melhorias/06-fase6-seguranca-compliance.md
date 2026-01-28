# 📊 Fase 6: Segurança e Compliance - System Admin

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Status:** Planejamento  
**Esforço:** 1 mês | 2 desenvolvedores  
**Custo Estimado:** R$ 39.000  
**Prazo:** Q4 2026

---

## 📋 Contexto

### Situação Atual

O system-admin possui segurança básica mas precisa de recursos enterprise-grade para compliance e auditoria.

**Funcionalidades Existentes:**
- ✅ Autenticação JWT
- ✅ Autorização baseada em roles
- ✅ Audit logs básicos
- ❌ Sem MFA/2FA
- ❌ Sem detecção de anomalias
- ❌ Audit logs não completos
- ❌ Sem backup automático robusto

### Objetivo da Fase 6

Elevar segurança para nível enterprise com:
1. Autenticação robusta (MFA, detecção de login suspeito)
2. Autorização granular por permissões
3. Audit log completo e compliance (LGPD, SOC 2)
4. Testes e qualidade automatizados

**Inspiração:** Auth0, AWS IAM, Stripe Security

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais
1. Implementar MFA/2FA e detecção de login suspeito
2. Sistema de permissões granular
3. Audit log completo com retenção e alertas
4. Compliance LGPD e SOC 2 readiness
5. CI/CD robusto com testes automatizados

### Benefícios Esperados
- 🔐 **Segurança enterprise-grade**
- 📋 **Compliance LGPD/SOC 2**
- 🔍 **Auditoria completa** de todas ações
- ✅ **Qualidade garantida** com testes > 80%

---

## 📝 Tarefas Detalhadas

### 1. Autenticação Robusta (2 semanas)

#### 1.1 Multi-Factor Authentication (MFA)

**Backend - MFA Service:**
```csharp
// Services/Auth/MfaService.cs
public interface IMfaService
{
    Task<MfaSetupDto> SetupTotp(int userId);
    Task<bool> VerifyTotp(int userId, string code);
    Task<string> GenerateBackupCodes(int userId);
    Task SendSmsCode(int userId);
    Task<bool> VerifySmsCode(int userId, string code);
    Task<bool> IsLoginSuspicious(int userId, LoginAttemptDto attempt);
}

public class MfaService : IMfaService
{
    public async Task<MfaSetupDto> SetupTotp(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");
            
        // Gerar secret key
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var secretKeyBase32 = Base32Encoding.ToString(secretKey);
        
        // Salvar secret (criptografado)
        user.MfaSecretKey = _encryption.Encrypt(secretKeyBase32);
        user.MfaEnabled = false; // Será habilitado após primeiro verify
        await _context.SaveChangesAsync();
        
        // Gerar QR Code para Google Authenticator
        var totpUrl = $"otpauth://totp/PrimeCare:{user.Email}?secret={secretKeyBase32}&issuer=PrimeCare";
        var qrCode = QrCodeGenerator.Generate(totpUrl);
        
        return new MfaSetupDto
        {
            SecretKey = secretKeyBase32,
            QrCode = qrCode,
            BackupCodes = await GenerateBackupCodes(userId)
        };
    }
    
    public async Task<bool> VerifyTotp(int userId, string code)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.MfaSecretKey))
            return false;
            
        var secretKey = _encryption.Decrypt(user.MfaSecretKey);
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        
        var isValid = totp.VerifyTotp(code, out long timeStepMatched, 
            new VerificationWindow(previous: 1, future: 1));
            
        if (isValid)
        {
            // Habilitar MFA se for a primeira verificação
            if (!user.MfaEnabled)
            {
                user.MfaEnabled = true;
                await _context.SaveChangesAsync();
            }
            
            // Registrar verificação bem-sucedida
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "MfaVerified",
                EntityType = "User",
                EntityId = userId.ToString(),
                UserId = userId
            });
        }
        
        return isValid;
    }
    
    public async Task<string> GenerateBackupCodes(int userId)
    {
        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            codes.Add(RandomNumberGenerator.GetInt32(100000, 999999).ToString());
        }
        
        var user = await _context.Users.FindAsync(userId);
        user.MfaBackupCodes = _encryption.Encrypt(string.Join(",", codes));
        await _context.SaveChangesAsync();
        
        return string.Join("\n", codes);
    }
    
    public async Task SendSmsCode(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.Phone))
            throw new ValidationException("User has no phone number");
            
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        
        // Salvar código temporariamente (5 min)
        await _cache.SetStringAsync($"sms_code:{userId}", code, 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            
        // Enviar SMS
        await _smsService.SendAsync(user.Phone, 
            $"Seu código de verificação PrimeCare: {code}");
    }
    
    public async Task<bool> VerifySmsCode(int userId, string code)
    {
        var cachedCode = await _cache.GetStringAsync($"sms_code:{userId}");
        
        if (cachedCode == code)
        {
            await _cache.RemoveAsync($"sms_code:{userId}");
            return true;
        }
        
        return false;
    }
}
```

**Detecção de Login Suspeito:**
```csharp
// Services/Auth/LoginAnomalyDetectionService.cs
public interface ILoginAnomalyDetectionService
{
    Task<bool> IsLoginSuspicious(int userId, LoginAttemptDto attempt);
    Task RecordLoginAttempt(int userId, LoginAttemptDto attempt, bool success);
}

public class LoginAnomalyDetectionService : ILoginAnomalyDetectionService
{
    public async Task<bool> IsLoginSuspicious(int userId, LoginAttemptDto attempt)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;
            
        var recentLogins = await _context.LoginHistory
            .Where(l => l.UserId == userId && l.Success)
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .ToListAsync();
            
        if (!recentLogins.Any())
            return false; // Primeiro login, não é suspeito
            
        var flags = new List<string>();
        
        // 1. Novo IP
        var knownIps = recentLogins.Select(l => l.IpAddress).Distinct().ToList();
        if (!knownIps.Contains(attempt.IpAddress))
        {
            flags.Add("new_ip");
        }
        
        // 2. Nova localização (país)
        var knownCountries = recentLogins.Select(l => l.Country).Distinct().ToList();
        var attemptCountry = await GetCountryFromIp(attempt.IpAddress);
        if (!knownCountries.Contains(attemptCountry))
        {
            flags.Add("new_country");
        }
        
        // 3. Novo dispositivo
        var knownDevices = recentLogins.Select(l => l.UserAgent).Distinct().ToList();
        if (!knownDevices.Any(ua => AreSimilarUserAgents(ua, attempt.UserAgent)))
        {
            flags.Add("new_device");
        }
        
        // 4. Horário incomum
        var lastLogin = recentLogins.First();
        var hoursSinceLastLogin = (DateTime.UtcNow - lastLogin.CreatedAt).TotalHours;
        if (hoursSinceLastLogin < 1 && lastLogin.Country != attemptCountry)
        {
            flags.Add("impossible_travel"); // Viagem impossível
        }
        
        // Considerar suspeito se tiver 2+ flags
        var isSuspicious = flags.Count >= 2;
        
        if (isSuspicious)
        {
            // Enviar notificação para o usuário
            await _notificationService.CreateNotification(new CreateNotificationDto
            {
                UserId = userId,
                Type = "warning",
                Title = "Login Suspeito Detectado",
                Message = $"Detectamos uma tentativa de login de um novo dispositivo/localização. Se não foi você, altere sua senha imediatamente.",
                ActionUrl = "/security/activity"
            });
            
            // Registrar no audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "SuspiciousLoginDetected",
                EntityType = "User",
                EntityId = userId.ToString(),
                Details = $"Flags: {string.Join(", ", flags)}",
                IpAddress = attempt.IpAddress
            });
        }
        
        return isSuspicious;
    }
    
    private async Task<string> GetCountryFromIp(string ipAddress)
    {
        // Usar serviço de geolocalização (MaxMind, IP2Location, etc)
        return "BR"; // placeholder
    }
    
    private bool AreSimilarUserAgents(string ua1, string ua2)
    {
        // Comparar apenas browser e OS, ignorar versão
        var parser = Parser.GetDefault();
        var client1 = parser.Parse(ua1);
        var client2 = parser.Parse(ua2);
        
        return client1.UA.Family == client2.UA.Family &&
               client1.OS.Family == client2.OS.Family;
    }
}
```

**Endpoint de Login com MFA:**
```csharp
// Controllers/Auth/AuthController.cs
[HttpPost("login")]
public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
{
    var user = await _authService.ValidateCredentials(dto.Email, dto.Password);
    if (user == null)
    {
        await RecordFailedLogin(dto.Email, Request);
        return Unauthorized("Invalid credentials");
    }
    
    // Verificar se login é suspeito
    var loginAttempt = new LoginAttemptDto
    {
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = Request.Headers["User-Agent"].ToString()
    };
    
    var isSuspicious = await _anomalyDetection.IsLoginSuspicious(user.Id, loginAttempt);
    
    // Se usuário tem MFA habilitado OU login suspeito, exigir MFA
    if (user.MfaEnabled || isSuspicious)
    {
        // Gerar token temporário de MFA
        var mfaToken = _jwtService.GenerateMfaToken(user.Id);
        
        return Ok(new LoginResponseDto
        {
            RequiresMfa = true,
            MfaToken = mfaToken,
            MfaMethods = GetAvailableMfaMethods(user)
        });
    }
    
    // Login normal
    var token = await _authService.GenerateToken(user);
    await RecordSuccessfulLogin(user.Id, loginAttempt);
    
    return Ok(new LoginResponseDto
    {
        Token = token,
        User = _mapper.Map<UserDto>(user)
    });
}

[HttpPost("mfa/verify")]
public async Task<ActionResult<LoginResponseDto>> VerifyMfa([FromBody] MfaVerifyDto dto)
{
    // Validar MFA token temporário
    var userId = _jwtService.ValidateMfaToken(dto.MfaToken);
    if (userId == null)
        return Unauthorized("Invalid MFA token");
        
    var user = await _context.Users.FindAsync(userId);
    
    // Verificar código
    bool isValid = dto.Method switch
    {
        "totp" => await _mfaService.VerifyTotp(userId.Value, dto.Code),
        "sms" => await _mfaService.VerifySmsCode(userId.Value, dto.Code),
        "backup" => await _mfaService.VerifyBackupCode(userId.Value, dto.Code),
        _ => false
    };
    
    if (!isValid)
        return Unauthorized("Invalid MFA code");
        
    // Gerar token final
    var token = await _authService.GenerateToken(user);
    
    return Ok(new LoginResponseDto
    {
        Token = token,
        User = _mapper.Map<UserDto>(user)
    });
}
```

#### 1.2 Frontend - MFA Setup

```typescript
// components/security/mfa-setup.component.ts
@Component({
  selector: 'app-mfa-setup',
  standalone: true,
  template: `
    <mat-stepper linear #stepper>
      <!-- Step 1: Escolher Método -->
      <mat-step label="Método">
        <h3>Escolha o método de autenticação</h3>
        <mat-radio-group [(ngModel)]="selectedMethod">
          <mat-radio-button value="totp">
            <div class="method-option">
              <mat-icon>smartphone</mat-icon>
              <div>
                <h4>Aplicativo Autenticador</h4>
                <p>Use Google Authenticator ou similar</p>
              </div>
            </div>
          </mat-radio-button>
          
          <mat-radio-button value="sms">
            <div class="method-option">
              <mat-icon>sms</mat-icon>
              <div>
                <h4>SMS</h4>
                <p>Receba códigos por mensagem</p>
              </div>
            </div>
          </mat-radio-button>
        </mat-radio-group>
        
        <div class="stepper-actions">
          <button mat-raised-button color="primary" (click)="stepper.next()">
            Próximo
          </button>
        </div>
      </mat-step>
      
      <!-- Step 2: Configurar -->
      <mat-step label="Configurar">
        <div *ngIf="selectedMethod === 'totp'">
          <h3>Escaneie o QR Code</h3>
          <img [src]="mfaSetup.qrCode" alt="QR Code">
          
          <p>Ou digite manualmente:</p>
          <code>{{ mfaSetup.secretKey }}</code>
          
          <mat-form-field>
            <mat-label>Código de Verificação</mat-label>
            <input matInput [(ngModel)]="verificationCode" maxlength="6">
          </mat-form-field>
        </div>
        
        <div *ngIf="selectedMethod === 'sms'">
          <h3>Confirme seu telefone</h3>
          <mat-form-field>
            <mat-label>Telefone</mat-label>
            <input matInput [(ngModel)]="phoneNumber" placeholder="+55 11 99999-9999">
          </mat-form-field>
          
          <button mat-button (click)="sendSmsCode()">Enviar Código</button>
          
          <mat-form-field *ngIf="codeSent">
            <mat-label>Código Recebido</mat-label>
            <input matInput [(ngModel)]="verificationCode" maxlength="6">
          </mat-form-field>
        </div>
        
        <div class="stepper-actions">
          <button mat-button (click)="stepper.previous()">Voltar</button>
          <button 
            mat-raised-button 
            color="primary" 
            (click)="verifyAndContinue()"
            [disabled]="!verificationCode"
          >
            Verificar
          </button>
        </div>
      </mat-step>
      
      <!-- Step 3: Backup Codes -->
      <mat-step label="Códigos de Backup">
        <h3>Salve seus códigos de backup</h3>
        <p>Use estes códigos se perder acesso ao seu método principal.</p>
        
        <div class="backup-codes">
          <code *ngFor="let code of backupCodes">{{ code }}</code>
        </div>
        
        <button mat-button (click)="downloadBackupCodes()">
          <mat-icon>download</mat-icon>
          Baixar Códigos
        </button>
        
        <mat-checkbox [(ngModel)]="confirmedBackup">
          Eu salvei meus códigos de backup em local seguro
        </mat-checkbox>
        
        <div class="stepper-actions">
          <button 
            mat-raised-button 
            color="primary" 
            (click)="completeMfaSetup()"
            [disabled]="!confirmedBackup"
          >
            Concluir
          </button>
        </div>
      </mat-step>
    </mat-stepper>
  `
})
export class MfaSetupComponent implements OnInit {
  selectedMethod: 'totp' | 'sms' = 'totp';
  mfaSetup: MfaSetup;
  verificationCode = '';
  backupCodes: string[] = [];
  confirmedBackup = false;
  
  async ngOnInit() {
    this.mfaSetup = await this.mfaService.setup();
    this.backupCodes = this.mfaSetup.backupCodes.split('\n');
  }
  
  async verifyAndContinue() {
    const isValid = await this.mfaService.verify(this.verificationCode);
    
    if (isValid) {
      this.snackBar.open('MFA configurado com sucesso!', 'OK', { duration: 3000 });
      this.stepper.next();
    } else {
      this.snackBar.open('Código inválido. Tente novamente.', 'OK', { duration: 3000 });
    }
  }
  
  downloadBackupCodes() {
    const blob = new Blob([this.backupCodes.join('\n')], { type: 'text/plain' });
    saveAs(blob, 'primecare-backup-codes.txt');
  }
  
  async completeMfaSetup() {
    await this.mfaService.enable();
    this.dialogRef.close(true);
  }
}
```

---

### 2. Autorização Granular (1 semana)

**Sistema de Permissões:**
```csharp
// Entities/Permission.cs
public class Permission
{
    public int Id { get; set; }
    public string Resource { get; set; } // clinics, users, tickets, billing
    public string Action { get; set; } // read, write, delete
    public string Name { get; set; } // clinics.read, users.write
    public string Description { get; set; }
}

// Entities/Role.cs
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } // SuperAdmin, Admin, Support, Observer
    public string Description { get; set; }
    public List<RolePermission> Permissions { get; set; }
}

// Services/Auth/AuthorizationService.cs
public class AuthorizationService
{
    public async Task<bool> HasPermission(int userId, string permission)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                    .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        return user?.Role?.Permissions
            .Any(rp => rp.Permission.Name == permission) ?? false;
    }
    
    public async Task<List<string>> GetUserPermissions(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                    .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        return user?.Role?.Permissions
            .Select(rp => rp.Permission.Name)
            .ToList() ?? new List<string>();
    }
}

// Attribute para permissões
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;
    
    public RequirePermissionAttribute(string permission)
    {
        _permission = permission;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var authService = context.HttpContext.RequestServices
            .GetRequiredService<IAuthorizationService>();
            
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var hasPermission = authService.HasPermission(userId, _permission).Result;
        
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}

// Usage
[RequirePermission("clinics.write")]
[HttpPost]
public async Task<ActionResult<ClinicDto>> CreateClinic(CreateClinicDto dto)
{
    // ...
}
```

---

### 3. Audit Log Completo (1 semana)

**Audit Log Avançado:**
```csharp
// Entities/AuditLog.cs
public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } // Create, Update, Delete, View, etc
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }
    public string UserName { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Changes { get; set; } // JSON com before/after
    public string Details { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Severity { get; set; } // Info, Warning, Critical
}

// Services/Audit/AuditService.cs
public class AuditService : IAuditService
{
    public async Task LogAsync(AuditLogDto dto)
    {
        var log = new AuditLog
        {
            Action = dto.Action,
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            UserId = dto.UserId,
            UserName = dto.UserName ?? _currentUser.Name,
            IpAddress = dto.IpAddress ?? _httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = _httpContext.Request.Headers["User-Agent"].ToString(),
            Changes = dto.Changes,
            Details = dto.Details,
            CreatedAt = DateTime.UtcNow,
            Severity = dto.Severity ?? "Info"
        };
        
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
        
        // Alertas em ações críticas
        if (log.Severity == "Critical")
        {
            await _alertingService.SendAlert(new AlertDto
            {
                Title = "Ação Crítica Executada",
                Message = $"{log.UserName} executou: {log.Action} em {log.EntityType}",
                Severity = "high"
            });
        }
        
        // Enviar para sistema de logging externo (opcional)
        if (_configuration.GetValue<bool>("AuditLog:SendToExternal"))
        {
            await _logShippingService.ShipLog(log);
        }
    }
    
    public async Task<object> GetChangesDiff(int auditLogId)
    {
        var log = await _context.AuditLogs.FindAsync(auditLogId);
        if (log == null || string.IsNullOrEmpty(log.Changes))
            return null;
            
        var changes = JsonSerializer.Deserialize<ChangesDto>(log.Changes);
        
        // Gerar diff visual
        return new
        {
            before = changes.Before,
            after = changes.After,
            diff = GenerateDiff(changes.Before, changes.After)
        };
    }
    
    private object GenerateDiff(object before, object after)
    {
        // Implementar diff algorithm (Myers, Hunt-McIlroy, etc)
        // Retornar mudanças de forma estruturada
        return new { /* diff result */ };
    }
}

// Interceptor automático para SaveChanges
public class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added ||
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            {
                await LogEntityChange(entry);
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private async Task LogEntityChange(EntityEntry entry)
    {
        var changes = new ChangesDto
        {
            Before = entry.State == EntityState.Added ? null : entry.OriginalValues.ToObject(),
            After = entry.State == EntityState.Deleted ? null : entry.CurrentValues.ToObject()
        };
        
        await _auditService.LogAsync(new AuditLogDto
        {
            Action = entry.State.ToString(),
            EntityType = entry.Entity.GetType().Name,
            EntityId = entry.Property("Id").CurrentValue?.ToString(),
            Changes = JsonSerializer.Serialize(changes)
        });
    }
}
```

---

### 4. Compliance LGPD (1 semana)

**Right to Access:**
```csharp
[RequirePermission("data.export")]
[HttpGet("clinics/{id}/export-data")]
public async Task<IActionResult> ExportClinicData(int id)
{
    var data = await _smartActionService.ExportClinicData(id);
    
    return File(data, "application/json", $"clinic-{id}-data.json");
}
```

**Right to Delete:**
```csharp
[RequirePermission("data.delete")]
[HttpPost("clinics/{id}/anonymize")]
public async Task<IActionResult> AnonymizeClinicData(int id)
{
    await _gdprService.AnonymizeClinic(id);
    return Ok();
}

// Services/Gdpr/GdprService.cs
public async Task AnonymizeClinic(int clinicId)
{
    var clinic = await _context.Clinics
        .Include(c => c.Users)
        .Include(c => c.Patients)
        .FirstOrDefaultAsync(c => c.Id == clinicId);
        
    if (clinic == null)
        throw new NotFoundException("Clinic not found");
        
    // Anonimizar dados pessoais
    clinic.Name = $"Clinic-{Guid.NewGuid()}";
    clinic.Email = $"anonymized-{Guid.NewGuid()}@example.com";
    clinic.Phone = "***";
    clinic.Address = "***";
    clinic.Cnpj = "***";
    
    foreach (var user in clinic.Users)
    {
        user.Name = $"User-{Guid.NewGuid()}";
        user.Email = $"anonymized-{Guid.NewGuid()}@example.com";
        user.Phone = "***";
    }
    
    foreach (var patient in clinic.Patients)
    {
        patient.Name = $"Patient-{Guid.NewGuid()}";
        patient.Cpf = "***";
        patient.Phone = "***";
        patient.Email = $"anonymized-{Guid.NewGuid()}@example.com";
    }
    
    await _context.SaveChangesAsync();
    
    // Audit log
    await _auditService.LogAsync(new AuditLogDto
    {
        Action = "DataAnonymized",
        EntityType = "Clinic",
        EntityId = clinicId.ToString(),
        Severity = "Critical",
        Details = "All personal data anonymized (GDPR compliance)"
    });
}
```

---

### 5. Testes e Qualidade (1 semana)

**CI/CD Pipeline:**
```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 8.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
    - name: Upload coverage
      uses: codecov/codecov-action@v2
      with:
        files: ./coverage.cobertura.xml
        
  security-scan:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Run Snyk
      uses: snyk/actions/dotnet@master
      env:
        SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        
  deploy-staging:
    needs: [test, security-scan]
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    
    steps:
    - name: Deploy to staging
      run: |
        # Deploy logic
```

---

## ✅ Critérios de Sucesso

### Autenticação
- [ ] MFA/2FA implementado (TOTP, SMS)
- [ ] Detecção de login suspeito
- [ ] Notificações de segurança
- [ ] Backup codes funcionando

### Autorização
- [ ] Sistema de permissões granular
- [ ] 4+ roles pré-definidos
- [ ] Permissões verificadas em todos endpoints
- [ ] Forbidden (403) correto

### Audit Log
- [ ] 100% de ações registradas
- [ ] Before/after diff
- [ ] Retenção de 2 anos
- [ ] Alertas em ações críticas
- [ ] Exportação funcional

### Compliance
- [ ] LGPD - Right to access
- [ ] LGPD - Right to delete
- [ ] Anonimização de dados
- [ ] Backup automático diário
- [ ] SOC 2 readiness

### Testes
- [ ] Coverage > 80%
- [ ] CI/CD pipeline completo
- [ ] Security scanning
- [ ] E2E tests críticos

---

## 🧪 Testes e Validação

```csharp
public class MfaServiceTests
{
    [Fact]
    public async Task SetupTotp_ShouldGenerateValidSecret()
    {
        var setup = await _mfaService.SetupTotp(userId);
        
        Assert.NotNull(setup.SecretKey);
        Assert.NotNull(setup.QrCode);
        Assert.NotEmpty(setup.BackupCodes);
    }
    
    [Fact]
    public async Task VerifyTotp_ShouldAcceptValidCode()
    {
        var isValid = await _mfaService.VerifyTotp(userId, validCode);
        Assert.True(isValid);
    }
}
```

---

## 📚 Documentação

- Security best practices
- MFA setup guide
- Permissions reference
- Audit log queries
- LGPD compliance guide
- Incident response plan

---

## 🔄 Próximos Passos

Após Fase 6:
1. ✅ Security audit externo
2. ✅ Penetration testing
3. ✅ Compliance certification
4. 🎉 **System Admin 2.0 completo!**

---

## 📞 Referências

- **Auth0:** https://auth0.com
- **AWS IAM:** https://aws.amazon.com/iam
- **Stripe Security:** https://stripe.com/docs/security

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Pronto para implementação
