# ❓ Portal do Paciente - FAQ e Troubleshooting

> **Guia de Resolução de Problemas**  
> **Última Atualização:** 04 de Fevereiro de 2026  
> **Versão:** 1.1

---

## 📋 Índice

1. [Problemas de Autenticação](#-problemas-de-autenticação)
2. [Problemas de Performance](#-problemas-de-performance)
3. [Problemas de API](#-problemas-de-api)
4. [Problemas de Frontend](#-problemas-de-frontend)
5. [Problemas de Notificações](#-problemas-de-notificações)
6. [Problemas de Banco de Dados](#-problemas-de-banco-de-dados)
7. [Problemas de Deploy](#-problemas-de-deploy)
8. [Perguntas Frequentes](#-perguntas-frequentes)

---

## 🔐 Problemas de Autenticação

### 1. "Token expirado" ou "Unauthorized 401"

**Sintomas:**
- Usuário logado é deslogado automaticamente
- Erro 401 ao fazer requisições

**Causas Comuns:**
- Access token expirou (15 minutos)
- Refresh token expirado (7 dias)
- Token inválido ou corrompido

**Solução:**

```typescript
// Implementar refresh token automático no interceptor
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error.status === 401) {
          // Tentar refresh token
          return this.authService.refreshToken().pipe(
            switchMap(newToken => {
              // Retry request com novo token
              const clonedRequest = req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              });
              return next.handle(clonedRequest);
            }),
            catchError(refreshError => {
              // Refresh falhou, redirecionar para login
              this.authService.logout();
              this.router.navigate(['/login']);
              return throwError(refreshError);
            })
          );
        }
        return throwError(error);
      })
    );
  }
}
```

### 2. "Account locked" após tentativas de login

**Sintomas:**
- Mensagem "Conta bloqueada por 15 minutos"
- Não consegue fazer login mesmo com senha correta

**Causas:**
- 5 tentativas falhadas de login

**Solução:**
- **Aguardar 15 minutos** (lockout automático)
- **Admin pode desbloquear manualmente:**

```sql
-- Desbloquear conta no banco de dados
UPDATE "PatientUsers"
SET "AccessFailedCount" = 0,
    "LockoutEnd" = NULL
WHERE "Id" = 'patient-guid-here';
```

### 3. CPF não é reconhecido no login

**Sintomas:**
- Mensagem "CPF não encontrado"
- Usuário tem certeza que está cadastrado

**Verificações:**
1. Verificar se CPF está cadastrado:

```sql
SELECT * FROM "PatientUsers" WHERE "CPF" = '12345678901';
```

2. Verificar formatação (remover pontos e traços):

```typescript
// Correto
const cpf = '12345678901'; // Apenas números

// Incorreto
const cpf = '123.456.789-01'; // Com formatação
```

3. Verificar se email foi confirmado:

```sql
SELECT "EmailConfirmed" FROM "PatientUsers" WHERE "CPF" = '12345678901';
-- Se false, enviar novo email de confirmação
```

---

## ⚡ Problemas de Performance

### 1. Página de documentos carrega lentamente

**Sintomas:**
- Lista de documentos demora > 5 segundos para carregar
- Muitos documentos (> 100)

**Soluções:**

**Backend: Adicionar paginação eficiente**

```csharp
[HttpGet("documents")]
public async Task<ActionResult<PagedResult<DocumentDto>>> GetDocuments(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    var patientId = GetCurrentPatientId();
    
    var query = _documentRepository.GetAll()
        .Where(d => d.PatientId == patientId)
        .OrderByDescending(d => d.CreatedAt);
    
    var total = await query.CountAsync();
    
    var documents = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(d => new DocumentDto
        {
            // Mapear apenas campos necessários
            Id = d.Id,
            Name = d.Name,
            Type = d.Type,
            CreatedAt = d.CreatedAt,
            Size = d.Size
            // NÃO carregar Content aqui
        })
        .ToListAsync();
    
    return Ok(new PagedResult<DocumentDto>
    {
        Items = documents,
        TotalCount = total,
        Page = page,
        PageSize = pageSize
    });
}
```

**Frontend: Virtual scrolling**

```typescript
// Use CDK Virtual Scrolling para listas grandes
<cdk-virtual-scroll-viewport itemSize="80" class="document-list">
  <mat-list-item *cdkVirtualFor="let document of documents">
    <!-- Item content -->
  </mat-list-item>
</cdk-virtual-scroll-viewport>
```

### 2. Download de documento muito lento

**Sintomas:**
- Download de PDF > 10 segundos
- Timeout em documentos grandes

**Soluções:**

**1. Streaming ao invés de carregar tudo em memória:**

```csharp
[HttpGet("documents/{id}/download")]
public async Task<IActionResult> DownloadDocument(Guid id)
{
    var patientId = GetCurrentPatientId();
    var document = await _documentRepository.GetByIdAsync(id);
    
    if (document.PatientId != patientId)
        return Forbid();
    
    // Stream direto do blob storage
    var stream = await _blobStorageService.GetStreamAsync(document.BlobPath);
    
    return File(stream, document.ContentType, document.FileName);
}
```

**2. CDN para documentos estáticos:**

```csharp
// Gerar URL assinada (expira em 1 hora)
[HttpGet("documents/{id}/url")]
public async Task<ActionResult<string>> GetDocumentUrl(Guid id)
{
    var url = await _blobStorageService.GenerateSignedUrlAsync(
        document.BlobPath,
        expiresIn: TimeSpan.FromHours(1)
    );
    
    return Ok(url);
}
```

### 3. API lenta em horários de pico

**Sintomas:**
- Requisições > 2 segundos
- Timeouts frequentes
- Muitos usuários simultâneos

**Soluções:**

**1. Adicionar cache Redis:**

```csharp
public class CachedAppointmentService : IAppointmentService
{
    private readonly IAppointmentService _innerService;
    private readonly IDistributedCache _cache;
    
    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(Guid patientId)
    {
        var cacheKey = $"appointments:upcoming:{patientId}";
        
        // Tentar buscar do cache
        var cachedValue = await _cache.GetStringAsync(cacheKey);
        if (cachedValue != null)
        {
            return JsonSerializer.Deserialize<List<AppointmentDto>>(cachedValue);
        }
        
        // Cache miss - buscar do banco
        var appointments = await _innerService.GetUpcomingAppointmentsAsync(patientId);
        
        // Salvar no cache (TTL: 5 minutos)
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(appointments),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }
        );
        
        return appointments;
    }
}
```

**2. Indexação de banco de dados:**

```sql
-- Criar índices para queries frequentes
CREATE INDEX idx_appointments_patient_date 
ON "Appointments" ("PatientId", "ScheduledDate");

CREATE INDEX idx_documents_patient_createddate 
ON "Documents" ("PatientId", "CreatedAt" DESC);

CREATE INDEX idx_patientusers_cpf 
ON "PatientUsers" ("CPF");

CREATE INDEX idx_patientusers_email 
ON "PatientUsers" ("Email");
```

---

## 🔌 Problemas de API

### 1. CORS Error: "Access-Control-Allow-Origin"

**Sintomas:**
- Console mostra erro de CORS
- Requisições bloqueadas pelo browser

**Solução:**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PatientPortalCors", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:4200",
                "https://portal.omnicare.com"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("PatientPortalCors");
```

### 2. 500 Internal Server Error sem mensagem

**Sintomas:**
- API retorna 500
- Sem detalhes do erro

**Debug:**

```csharp
// Habilitar detalhes de erro em Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

// Middleware de logging global
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception");
        
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "Internal server error",
            message = ex.Message,
            trace = ex.StackTrace // Apenas em dev
        });
    }
});
```

### 3. Rate Limit: "Too Many Requests (429)"

**Sintomas:**
- Erro 429 após muitas requisições
- "Rate limit exceeded"

**Solução:**

**Aumentar limite (se legítimo):**

```csharp
// appsettings.json
{
  "RateLimiting": {
    "PermitLimit": 200, // Era 100
    "WindowMinutes": 1
  }
}
```

**Implementar retry com backoff:**

```typescript
// Angular retry com exponential backoff
this.http.get('/api/appointments').pipe(
  retry({
    count: 3,
    delay: (error, retryCount) => {
      if (error.status === 429) {
        // Exponential backoff: 1s, 2s, 4s
        return timer(Math.pow(2, retryCount) * 1000);
      }
      return throwError(error);
    }
  })
);
```

---

## 🎨 Problemas de Frontend

### 1. Componente não atualiza após mudança

**Sintomas:**
- Dados mudam no backend mas UI não reflete
- Precisa dar F5 para ver mudanças

**Solução:**

```typescript
// Usar BehaviorSubject para dados reativos
export class AppointmentService {
  private appointmentsSubject = new BehaviorSubject<Appointment[]>([]);
  public appointments$ = this.appointmentsSubject.asObservable();
  
  loadAppointments() {
    this.http.get<Appointment[]>('/api/appointments').subscribe(
      data => this.appointmentsSubject.next(data)
    );
  }
  
  bookAppointment(data: BookingData) {
    return this.http.post('/api/appointments/book', data).pipe(
      tap(() => this.loadAppointments()) // Recarregar após booking
    );
  }
}

// Componente
export class AppointmentsComponent {
  appointments$ = this.appointmentService.appointments$;
  
  ngOnInit() {
    this.appointmentService.loadAppointments();
  }
}
```

### 2. Memory leak em subscriptions

**Sintomas:**
- Aplicação fica lenta após uso prolongado
- Múltiplas requisições duplicadas

**Solução:**

```typescript
export class DocumentsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  ngOnInit() {
    this.documentService.getDocuments()
      .pipe(takeUntil(this.destroy$))
      .subscribe(docs => this.documents = docs);
  }
  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

### 3. Formulário não valida corretamente

**Sintomas:**
- Validações não aparecem
- Pode enviar formulário inválido

**Solução:**

```typescript
export class LoginComponent {
  loginForm = this.fb.group({
    cpf: ['', [
      Validators.required,
      Validators.pattern(/^\d{11}$/),
      this.cpfValidator
    ]],
    password: ['', [
      Validators.required,
      Validators.minLength(8)
    ]]
  });
  
  // Custom validator
  cpfValidator(control: AbstractControl): ValidationErrors | null {
    const cpf = control.value?.replace(/\D/g, '');
    
    if (!cpf || cpf.length !== 11) {
      return { invalidCpf: true };
    }
    
    // Validação de CPF completa
    if (!this.isValidCpf(cpf)) {
      return { invalidCpf: true };
    }
    
    return null;
  }
  
  onSubmit() {
    // Marcar todos os campos como touched
    Object.keys(this.loginForm.controls).forEach(key => {
      this.loginForm.get(key)?.markAsTouched();
    });
    
    if (this.loginForm.invalid) {
      return;
    }
    
    // Proceed with submit
  }
}
```

---

## 📧 Problemas de Notificações

### 1. WhatsApp não envia (Twilio)

**Sintomas:**
- Erro ao enviar WhatsApp
- Mensagens não chegam

**Verificações:**

1. **Credenciais Twilio:**

```bash
# Testar credenciais
curl -X GET "https://api.twilio.com/2010-04-01/Accounts/{AccountSid}.json" \
  -u "{AccountSid}:{AuthToken}"
```

2. **Número WhatsApp aprovado:**
- Twilio Sandbox: número precisa enviar "join [code]" primeiro
- Produção: número precisa estar aprovado pelo Twilio

3. **Formato do número:**

```csharp
// Correto
var toNumber = "whatsapp:+5511999999999"; // Inclui código do país

// Incorreto
var toNumber = "whatsapp:11999999999"; // Falta +55
```

**Logs detalhados:**

```csharp
try 
{
    var message = await MessageResource.CreateAsync(...);
    
    _logger.LogInformation($@"
        WhatsApp sent successfully
        SID: {message.Sid}
        Status: {message.Status}
        To: {message.To}
        From: {message.From}
    ");
}
catch (ApiException ex)
{
    _logger.LogError($@"
        Twilio API Error
        Code: {ex.Code}
        Status: {ex.Status}
        Message: {ex.Message}
        MoreInfo: {ex.MoreInfo}
    ");
}
```

### 2. Emails vão para SPAM

**Sintomas:**
- Emails enviados mas não chegam na caixa de entrada
- Vão direto para spam

**Soluções:**

1. **Configurar SPF, DKIM, DMARC:**

```dns
; SPF Record
@ TXT "v=spf1 include:sendgrid.net ~all"

; DKIM (fornecido pelo SendGrid)
s1._domainkey TXT "k=rsa; p=MIGfMA0GCS..."

; DMARC
_dmarc TXT "v=DMARC1; p=quarantine; rua=mailto:postmaster@omnicare.com"
```

2. **Usar domínio próprio:**

```csharp
// NÃO usar @gmail.com ou @hotmail.com
FromEmail = "noreply@omnicare.com" // ✅

// Evitar
FromEmail = "noreply@gmail.com" // ❌
```

3. **Conteúdo do email:**
- Evitar palavras como "grátis", "urgente", "clique aqui"
- Incluir link de unsubscribe
- Manter ratio texto/imagem saudável (> 60% texto)

### 3. Background Service não executa

**Sintomas:**
- Lembretes não são enviados automaticamente
- Logs não mostram execução do serviço

**Debug:**

```csharp
public class AppointmentReminderService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== SERVICE STARTED ===");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"[{DateTime.Now}] Executing reminder job...");
            
            try
            {
                await SendRemindersAsync();
                _logger.LogInformation("Reminder job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reminder job");
            }
            
            _logger.LogInformation($"Next execution in {_interval.TotalMinutes} minutes");
            await Task.Delay(_interval, stoppingToken);
        }
        
        _logger.LogInformation("=== SERVICE STOPPED ===");
    }
}
```

**Verificar se está registrado:**

```csharp
// Program.cs
services.AddHostedService<AppointmentReminderService>();
```

---

## 💾 Problemas de Banco de Dados

### 1. "Password authentication failed for user postgres"

**Sintomas:**
- Erro `28P01: password authentication failed for user "postgres"`
- Serviço de lembretes de consulta não funciona
- Logs mostram múltiplos erros de autenticação do PostgreSQL

**Causas Comuns:**
- Connection string com credenciais incorretas
- Banco de dados não está rodando
- Senha do PostgreSQL mudou mas configuração não foi atualizada
- Usuário PostgreSQL não existe ou não tem permissões

**Solução:**

**1. Verificar credenciais no appsettings.json:**

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse;Username=postgres;Password=postgres;Include Error Detail=true"
  }
}
```

**2. Verificar se PostgreSQL está rodando:**

```bash
# Docker
docker ps | grep postgres

# Serviço local
sudo systemctl status postgresql  # Linux
brew services list | grep postgres  # Mac
Get-Service postgresql*  # Windows PowerShell
```

**3. Testar conexão manualmente:**

```bash
# psql
psql -h localhost -p 5432 -U postgres -d medicwarehouse

# Se falhar com mesmo erro, resetar senha:
# Docker
docker exec -it patient-portal-postgres psql -U postgres
ALTER USER postgres WITH PASSWORD 'nova_senha';

# Serviço local
sudo -u postgres psql
ALTER USER postgres WITH PASSWORD 'nova_senha';
```

**4. Atualizar configuração:**

```bash
# .env file (se usando docker-compose)
POSTGRES_PASSWORD=nova_senha

# appsettings.json
"DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse;Username=postgres;Password=nova_senha"
```

**Nota:** O serviço de lembretes agora lida graciosamente com falhas de conexão do banco de dados. Ele irá:
- Logar um aviso ao invés de um erro
- Continuar rodando e tentar novamente no próximo intervalo
- Não crashar a aplicação se o banco estiver temporariamente indisponível

**5. Configuração para ambiente de testes (Testing):**

Para evitar erros de conexão durante testes automatizados, use o arquivo `appsettings.Testing.json` que desabilita o serviço de lembretes:

```json
{
  "AppointmentReminder": {
    "Enabled": false
  }
}
```

Isso é especialmente útil quando:
- Rodando testes de integração/performance sem banco de dados completo
- O banco de teste não tem as tabelas do sistema principal (Appointments, Patients, etc.)
- Executando em ambientes CI/CD com recursos limitados

### 2. Migration falha

**Sintomas:**
- `dotnet ef database update` retorna erro
- Mudanças no schema não aplicam

**Soluções:**

**Erro: Coluna já existe**

```bash
# Reverter última migration
dotnet ef migrations remove --project PatientPortal.Infrastructure

# Criar nova migration
dotnet ef migrations add FixColumnName --project PatientPortal.Infrastructure

# Aplicar
dotnet ef database update --project PatientPortal.Infrastructure
```

**Erro: Relação não existe (PostgreSQL)**

```csharp
// Usar nomes case-sensitive com aspas
[Table("PatientUsers")] // ✅
[Table("patientusers")] // ❌ PostgreSQL converte para lowercase
```

### 2. Deadlock em transações

**Sintomas:**
- Timeout ao salvar dados
- Erro "deadlock detected"

**Solução:**

```csharp
// Usar transações com isolation level apropriado
using var transaction = await _context.Database.BeginTransactionAsync(
    IsolationLevel.ReadCommitted // Ao invés de Serializable
);

try
{
    // Fazer updates em ordem consistente (sempre A → B → C)
    await UpdatePatient(patientId);
    await UpdateAppointment(appointmentId);
    await SaveChangesAsync();
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

### 3. Queries lentas

**Sintomas:**
- Queries demoram > 2 segundos
- CPU do banco alto

**Debug:**

```sql
-- PostgreSQL: Ver queries lentas
SELECT pid, now() - pg_stat_activity.query_start AS duration, query
FROM pg_stat_activity
WHERE state = 'active'
AND now() - pg_stat_activity.query_start > interval '2 seconds'
ORDER BY duration DESC;

-- Analisar plano de execução
EXPLAIN ANALYZE 
SELECT * FROM "Appointments" 
WHERE "PatientId" = 'guid-here' 
AND "ScheduledDate" > NOW();
```

**Soluções:**

```csharp
// 1. Usar AsNoTracking para queries read-only
var appointments = await _context.Appointments
    .AsNoTracking()
    .Where(a => a.PatientId == patientId)
    .ToListAsync();

// 2. Projetar apenas campos necessários
var appointments = await _context.Appointments
    .Where(a => a.PatientId == patientId)
    .Select(a => new AppointmentDto
    {
        Id = a.Id,
        DoctorName = a.Doctor.Name, // EF faz join automaticamente
        ScheduledDate = a.ScheduledDate
        // NÃO carregar todo o objeto
    })
    .ToListAsync();

// 3. Usar Include para evitar N+1
var appointments = await _context.Appointments
    .Include(a => a.Doctor)
    .Include(a => a.Patient)
    .Where(a => a.PatientId == patientId)
    .ToListAsync();
```

---

## 🚀 Problemas de Deploy

### 1. Aplicação não inicia após deploy

**Verificar logs:**

```bash
# Docker logs
docker logs patient-portal-api --tail 100

# Kubernetes logs
kubectl logs -f deployment/patient-portal-api

# Azure App Service
az webapp log tail --name patient-portal-api --resource-group omnicare-rg
```

**Checklist:**

- [ ] Connection string correta?
- [ ] Variáveis de ambiente configuradas?
- [ ] Migrations aplicadas?
- [ ] Porta correta exposta?

### 2. "Failed to bind to address" (porta em uso)

**Solução:**

```bash
# Descobrir processo usando a porta
lsof -i :5000  # Linux/Mac
netstat -ano | findstr :5000  # Windows

# Mudar porta na aplicação
export ASPNETCORE_URLS="http://+:5001"
```

### 3. Database connection timeout

**Verificar:**

```csharp
// Connection string com timeout aumentado
"Host=localhost;Database=patient_portal;Username=postgres;Password=pwd;Timeout=60;Command Timeout=60"
```

---

## ❓ Perguntas Frequentes

### Segurança

**Q: Como são armazenadas as senhas?**  
A: Usando PBKDF2 com 100.000 iterações + salt único por usuário.

**Q: Os tokens JWT expiram?**  
A: Sim. Access token: 15 min. Refresh token: 7 dias.

**Q: Dados médicos são criptografados?**  
A: Sim. Em trânsito (HTTPS) e em repouso (AES-256 para campos sensíveis).

### Funcionalidades

**Q: Paciente pode cancelar consulta no mesmo dia?**  
A: Não. Requer mínimo 24h de antecedência (configurável).

**Q: Quantos documentos um paciente pode ter?**  
A: Ilimitado. Usar paginação para listas grandes.

**Q: WhatsApp é obrigatório para lembretes?**  
A: Não. Emails são enviados sempre. WhatsApp é adicional se telefone disponível.

### Performance

**Q: Quantos usuários simultâneos suporta?**  
A: Testado com 1.000+ usuários simultâneos. Escala horizontalmente.

**Q: Qual o tamanho máximo de documento?**  
A: 10 MB por padrão (configurável até 50 MB).

**Q: Cache é usado?**  
A: Sim. Redis para sessões e dados frequentes (TTL: 5 min).

### Custos

**Q: Qual o custo de notificações por mês?**  
A: WhatsApp: ~$5 USD. Email: Grátis até 100/dia (SendGrid).

**Q: Precisa pagar pelo Twilio?**  
A: Sandbox é grátis (teste). Produção: pay-per-message (~$0.005/msg).

---

## 📞 Suporte

### Quando reportar um bug:

1. ✅ Descrição clara do problema
2. ✅ Passos para reproduzir
3. ✅ Mensagens de erro completas
4. ✅ Ambiente (dev/staging/prod)
5. ✅ Browser/versão (se frontend)
6. ✅ Screenshots/videos se possível

### Template de Bug Report:

```markdown
**Descrição:**
[Descreva o problema]

**Passos para Reproduzir:**
1. Acesse /appointments
2. Clique em "Agendar"
3. Selecione médico
4. Erro ocorre

**Comportamento Esperado:**
[O que deveria acontecer]

**Comportamento Atual:**
[O que está acontecendo]

**Erro:**
```
[Cole logs/erros aqui]
```

**Ambiente:**
- Browser: Chrome 120
- OS: Windows 11
- Ambiente: Production
- Data/Hora: 2026-01-26 14:30 BRT

**Screenshots:**
[Adicione screenshots]
```

---

## 📚 Documentação Adicional

- [PATIENT_PORTAL_ARCHITECTURE.md](../system-admin/regras-negocio/PATIENT_PORTAL_ARCHITECTURE.md) - Arquitetura
- [PATIENT_PORTAL_SECURITY_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_SECURITY_GUIDE.md) - Segurança
- [PATIENT_PORTAL_DEPLOYMENT_GUIDE.md](../system-admin/guias/PATIENT_PORTAL_DEPLOYMENT_GUIDE.md) - Deploy
- [BOOKING_IMPLEMENTATION_GUIDE.md](./BOOKING_IMPLEMENTATION_GUIDE.md) - Agendamento Online
- [NOTIFICATION_SERVICE_GUIDE.md](./NOTIFICATION_SERVICE_GUIDE.md) - Notificações

---

**Última Atualização:** 04 de Fevereiro de 2026  
**Mantido por:** Equipe Omni Care  
**Contribua:** Abra uma issue no GitHub com sugestões de melhorias
