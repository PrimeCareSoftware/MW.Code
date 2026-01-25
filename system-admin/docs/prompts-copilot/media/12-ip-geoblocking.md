# 🛡️ Prompt: IP Blocking e Geo-blocking

## 📊 Status
- **Prioridade**: 🔥 MÉDIA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1 mês | 1 dev
- **Prazo**: Q3/2026

## 🎯 Contexto

Implementar sistema robusto de bloqueio de IPs e geo-blocking para proteger a aplicação contra ataques, acessos maliciosos e fraudes. Inclui lista negra/branca de IPs, bloqueio por país, detecção de proxies/VPN/Tor, e integração com serviços de reputação de IPs.

## 📋 Justificativa

### Problemas/Ameaças
- ❌ Ataques de força bruta (brute force)
- ❌ Acessos de bots maliciosos
- ❌ Fraudes de pagamento
- ❌ Scraping não autorizado
- ❌ Acesso de regiões suspeitas
- ❌ Uso de VPNs para burlar bloqueios

### Benefícios
- ✅ Redução de ataques: 70-80%
- ✅ Proteção contra fraudes
- ✅ Compliance geográfico (LGPD, GDPR)
- ✅ Redução de custos de infraestrutura
- ✅ Melhor controle de acesso
- ✅ Logs detalhados de tentativas bloqueadas

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// src/Domain/Entities/IpBlockRule.cs
public class IpBlockRule : Entity
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; }  // Ex: "192.168.1.1" ou "192.168.1.0/24" (CIDR)
    public RuleType Type { get; set; }  // Blacklist ou Whitelist
    public string Reason { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }  // Null = permanente
    public int BlockCount { get; set; }
    public DateTime? LastBlockedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    
    // Auto-bloqueio
    public bool IsAutomatic { get; set; }
    public string TriggerReason { get; set; }  // Ex: "5 tentativas falhas em 5min"
}

public enum RuleType
{
    Blacklist,  // Bloquear
    Whitelist   // Permitir (ignorar outras regras)
}

// src/Domain/Entities/GeoBlockRule.cs
public class GeoBlockRule : Entity
{
    public Guid Id { get; set; }
    public string CountryCode { get; set; }  // ISO 3166-1 alpha-2 (BR, US, CN, etc)
    public string CountryName { get; set; }
    public RuleType Type { get; set; }
    public bool IsActive { get; set; }
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

// src/Domain/Entities/BlockedAccessLog.cs
public class BlockedAccessLog : Entity
{
    public Guid Id { get; set; }
    public DateTime BlockedAt { get; set; }
    public string IpAddress { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string City { get; set; }
    public BlockReason Reason { get; set; }
    public string RuleDescription { get; set; }
    public string RequestPath { get; set; }
    public string UserAgent { get; set; }
    public string Referrer { get; set; }
    
    // Análise de IP
    public bool IsProxy { get; set; }
    public bool IsVpn { get; set; }
    public bool IsTor { get; set; }
    public bool IsHosting { get; set; }  // Data center
    public int ThreatScore { get; set; }  // 0-100
    public string IpReputationSource { get; set; }  // AbuseIPDB, IPQualityScore, etc
}

public enum BlockReason
{
    IpBlacklist,
    CountryBlocked,
    ProxyDetected,
    VpnDetected,
    TorDetected,
    HighThreatScore,
    RateLimitExceeded,
    BruteForceDetected,
    MaliciousActivity
}

// Value Objects
public class IpInfo : ValueObject
{
    public string IpAddress { get; private set; }
    public string CountryCode { get; private set; }
    public string CountryName { get; private set; }
    public string Region { get; private set; }
    public string City { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string Isp { get; private set; }
    public bool IsProxy { get; private set; }
    public bool IsVpn { get; private set; }
    public bool IsTor { get; private set; }
    public bool IsHosting { get; private set; }
    public int ThreatScore { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IpAddress;
    }
}
```

### Camada de Aplicação (Application Layer)

```csharp
// src/Application/Services/IIpBlockingService.cs
public interface IIpBlockingService
{
    // Verificação
    Task<bool> IsIpBlockedAsync(string ipAddress);
    Task<bool> IsCountryBlockedAsync(string countryCode);
    Task<BlockCheckResult> CheckAccessAsync(string ipAddress);
    
    // Regras de IP
    Task<IpBlockRule> AddIpRuleAsync(string ipAddress, RuleType type, string reason, DateTime? expiresAt = null);
    Task RemoveIpRuleAsync(Guid ruleId);
    Task<List<IpBlockRule>> GetIpRulesAsync(RuleType? type = null);
    
    // Regras Geográficas
    Task<GeoBlockRule> AddGeoRuleAsync(string countryCode, RuleType type, string reason);
    Task RemoveGeoRuleAsync(Guid ruleId);
    Task<List<GeoBlockRule>> GetGeoRulesAsync(RuleType? type = null);
    
    // Auto-bloqueio
    Task CheckAndAutoBlockAsync(string ipAddress, string reason);
    
    // Logs
    Task LogBlockedAccessAsync(BlockedAccessLog log);
    Task<List<BlockedAccessLog>> GetBlockedAccessLogsAsync(DateTime startDate, DateTime endDate);
    Task<BlockStatistics> GetBlockStatisticsAsync(DateTime startDate, DateTime endDate);
}

// src/Application/Services/IIpInformationService.cs
public interface IIpInformationService
{
    Task<IpInfo> GetIpInfoAsync(string ipAddress);
    Task<bool> IsProxyOrVpnAsync(string ipAddress);
    Task<int> GetThreatScoreAsync(string ipAddress);
}

// DTOs
public class BlockCheckResult
{
    public bool IsBlocked { get; set; }
    public BlockReason? Reason { get; set; }
    public string Message { get; set; }
    public IpInfo IpInfo { get; set; }
}

public class BlockStatistics
{
    public int TotalBlocked { get; set; }
    public Dictionary<BlockReason, int> ByReason { get; set; }
    public Dictionary<string, int> ByCountry { get; set; }
    public List<string> TopBlockedIps { get; set; }
}
```

### Middleware

```csharp
// src/API/Middleware/IpBlockingMiddleware.cs
public class IpBlockingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockingMiddleware> _logger;
    
    public async Task InvokeAsync(
        HttpContext context,
        IIpBlockingService blockingService,
        IIpInformationService ipInfoService)
    {
        var ipAddress = GetClientIpAddress(context);
        
        // Verificar bloqueio
        var checkResult = await blockingService.CheckAccessAsync(ipAddress);
        
        if (checkResult.IsBlocked)
        {
            _logger.LogWarning(
                "Access blocked for IP {IpAddress}: {Reason}",
                ipAddress,
                checkResult.Reason);
            
            // Registrar log
            await blockingService.LogBlockedAccessAsync(new BlockedAccessLog
            {
                BlockedAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                CountryCode = checkResult.IpInfo?.CountryCode,
                CountryName = checkResult.IpInfo?.CountryName,
                City = checkResult.IpInfo?.City,
                Reason = checkResult.Reason.Value,
                RequestPath = context.Request.Path,
                UserAgent = context.Request.Headers["User-Agent"],
                Referrer = context.Request.Headers["Referer"],
                IsProxy = checkResult.IpInfo?.IsProxy ?? false,
                IsVpn = checkResult.IpInfo?.IsVpn ?? false,
                IsTor = checkResult.IpInfo?.IsTor ?? false,
                ThreatScore = checkResult.IpInfo?.ThreatScore ?? 0
            });
            
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Access Forbidden",
                message = checkResult.Message,
                reason = checkResult.Reason.ToString()
            });
            
            return;
        }
        
        await _next(context);
    }
    
    private string GetClientIpAddress(HttpContext context)
    {
        // Verificar headers de proxy/load balancer
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        
        // Se múltiplos IPs (proxies encadeados), pegar o primeiro
        if (ip?.Contains(',') == true)
        {
            ip = ip.Split(',')[0].Trim();
        }
        
        return ip ?? "0.0.0.0";
    }
}
```

### Integração com Serviços Externos

```csharp
// src/Infrastructure/IpInformation/MaxMindGeoIpService.cs
public class MaxMindGeoIpService : IIpInformationService
{
    private readonly DatabaseReader _reader;
    private readonly HttpClient _httpClient;
    private readonly string _ipQualityScoreApiKey;
    
    public async Task<IpInfo> GetIpInfoAsync(string ipAddress)
    {
        // MaxMind GeoIP2 (local database)
        var cityResponse = _reader.City(ipAddress);
        
        var ipInfo = new IpInfo
        {
            IpAddress = ipAddress,
            CountryCode = cityResponse.Country.IsoCode,
            CountryName = cityResponse.Country.Name,
            Region = cityResponse.MostSpecificSubdivision.Name,
            City = cityResponse.City.Name,
            Latitude = cityResponse.Location.Latitude ?? 0,
            Longitude = cityResponse.Location.Longitude ?? 0
        };
        
        // IPQualityScore para análise de proxy/VPN
        var proxyData = await CheckProxyAsync(ipAddress);
        ipInfo.IsProxy = proxyData.IsProxy;
        ipInfo.IsVpn = proxyData.IsVpn;
        ipInfo.IsTor = proxyData.IsTor;
        ipInfo.IsHosting = proxyData.IsHosting;
        ipInfo.ThreatScore = proxyData.FraudScore;
        
        return ipInfo;
    }
    
    private async Task<ProxyCheckResult> CheckProxyAsync(string ipAddress)
    {
        var url = $"https://ipqualityscore.com/api/json/ip/{_ipQualityScoreApiKey}/{ipAddress}";
        var response = await _httpClient.GetAsync(url);
        var data = await response.Content.ReadFromJsonAsync<ProxyCheckResult>();
        return data;
    }
}

// src/Infrastructure/IpInformation/AbuseIpDbService.cs
public class AbuseIpDbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public async Task<int> CheckIpReputationAsync(string ipAddress)
    {
        _httpClient.DefaultRequestHeaders.Add("Key", _apiKey);
        
        var url = $"https://api.abuseipdb.com/api/v2/check?ipAddress={ipAddress}";
        var response = await _httpClient.GetAsync(url);
        var data = await response.Content.ReadFromJsonAsync<AbuseIpDbResponse>();
        
        return data.Data.AbuseConfidenceScore;
    }
    
    public async Task ReportMaliciousIpAsync(string ipAddress, string reason)
    {
        _httpClient.DefaultRequestHeaders.Add("Key", _apiKey);
        
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("ip", ipAddress),
            new KeyValuePair<string, string>("categories", "18,21"),  // Brute force, Web app attack
            new KeyValuePair<string, string>("comment", reason)
        });
        
        await _httpClient.PostAsync("https://api.abuseipdb.com/api/v2/report", content);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes

```typescript
// src/app/features/security/ip-blocking/ip-rules-list/ip-rules-list.component.ts
@Component({
  selector: 'app-ip-rules-list',
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Regras de Bloqueio de IP</mat-card-title>
        <button mat-raised-button color="primary" (click)="addRule()">
          <mat-icon>add</mat-icon>
          Nova Regra
        </button>
      </mat-card-header>
      
      <mat-card-content>
        <mat-tab-group>
          <mat-tab label="Lista Negra ({{ blacklistRules.length }})">
            <mat-table [dataSource]="blacklistRules">
              <ng-container matColumnDef="ipAddress">
                <mat-header-cell *matHeaderCellDef>IP/CIDR</mat-header-cell>
                <mat-cell *matCellDef="let rule">{{ rule.ipAddress }}</mat-cell>
              </ng-container>
              
              <ng-container matColumnDef="reason">
                <mat-header-cell *matHeaderCellDef>Motivo</mat-header-cell>
                <mat-cell *matCellDef="let rule">{{ rule.reason }}</mat-cell>
              </ng-container>
              
              <ng-container matColumnDef="createdAt">
                <mat-header-cell *matHeaderCellDef>Criado</mat-header-cell>
                <mat-cell *matCellDef="let rule">{{ rule.createdAt | date:'short' }}</mat-cell>
              </ng-container>
              
              <ng-container matColumnDef="expiresAt">
                <mat-header-cell *matHeaderCellDef>Expira</mat-header-cell>
                <mat-cell *matCellDef="let rule">
                  {{ rule.expiresAt ? (rule.expiresAt | date:'short') : 'Permanente' }}
                </mat-cell>
              </ng-container>
              
              <ng-container matColumnDef="blockCount">
                <mat-header-cell *matHeaderCellDef>Bloqueios</mat-header-cell>
                <mat-cell *matCellDef="let rule">{{ rule.blockCount }}</mat-cell>
              </ng-container>
              
              <ng-container matColumnDef="actions">
                <mat-header-cell *matHeaderCellDef>Ações</mat-header-cell>
                <mat-cell *matCellDef="let rule">
                  <button mat-icon-button (click)="removeRule(rule)" color="warn">
                    <mat-icon>delete</mat-icon>
                  </button>
                </mat-cell>
              </ng-container>
              
              <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
              <mat-row *matRowDef="let row; columns: displayedColumns"></mat-row>
            </mat-table>
          </mat-tab>
          
          <mat-tab label="Lista Branca ({{ whitelistRules.length }})">
            <!-- Similar structure for whitelist -->
          </mat-tab>
        </mat-tab-group>
      </mat-card-content>
    </mat-card>
    
    <mat-card class="mt-4">
      <mat-card-header>
        <mat-card-title>Bloqueio Geográfico</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>Selecione os países que deseja bloquear ou permitir:</p>
        
        <mat-radio-group [(ngModel)]="geoBlockMode">
          <mat-radio-button value="allowlist">Permitir apenas países selecionados</mat-radio-button>
          <mat-radio-button value="blocklist">Bloquear países selecionados</mat-radio-button>
        </mat-radio-group>
        
        <mat-form-field appearance="outline" class="w-full mt-3">
          <mat-label>Buscar país</mat-label>
          <input matInput [(ngModel)]="countrySearch" (input)="filterCountries()">
          <mat-icon matPrefix>search</mat-icon>
        </mat-form-field>
        
        <mat-selection-list [(ngModel)]="selectedCountries">
          <mat-list-option *ngFor="let country of filteredCountries" [value]="country.code">
            <span class="fi fi-{{ country.code.toLowerCase() }}"></span>
            {{ country.name }}
          </mat-list-option>
        </mat-selection-list>
        
        <button mat-raised-button color="primary" (click)="saveGeoRules()">
          Salvar Configurações
        </button>
      </mat-card-content>
    </mat-card>
  `
})
export class IpRulesListComponent implements OnInit {
  blacklistRules: IpBlockRule[] = [];
  whitelistRules: IpBlockRule[] = [];
  geoBlockMode: 'allowlist' | 'blocklist' = 'blocklist';
  selectedCountries: string[] = [];
  filteredCountries: any[] = [];
  countrySearch = '';
  
  displayedColumns = ['ipAddress', 'reason', 'createdAt', 'expiresAt', 'blockCount', 'actions'];
  
  constructor(
    private ipBlockingService: IpBlockingService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}
  
  async ngOnInit() {
    await this.loadRules();
    this.loadCountries();
  }
  
  async loadRules() {
    const rules = await this.ipBlockingService.getIpRules();
    this.blacklistRules = rules.filter(r => r.type === 'Blacklist');
    this.whitelistRules = rules.filter(r => r.type === 'Whitelist');
  }
  
  addRule() {
    const dialogRef = this.dialog.open(AddIpRuleDialogComponent);
    
    dialogRef.afterClosed().subscribe(async (result) => {
      if (result) {
        await this.ipBlockingService.addIpRule(result);
        await this.loadRules();
        this.snackBar.open('Regra adicionada!', 'OK', { duration: 3000 });
      }
    });
  }
  
  async removeRule(rule: IpBlockRule) {
    const confirmed = confirm('Remover esta regra?');
    if (confirmed) {
      await this.ipBlockingService.removeIpRule(rule.id);
      await this.loadRules();
      this.snackBar.open('Regra removida!', 'OK', { duration: 3000 });
    }
  }
}

// src/app/features/security/ip-blocking/blocked-access-logs/blocked-access-logs.component.ts
@Component({
  selector: 'app-blocked-access-logs',
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Logs de Acessos Bloqueados</mat-card-title>
      </mat-card-header>
      
      <mat-card-content>
        <div class="filters">
          <mat-form-field appearance="outline">
            <mat-label>Data Inicial</mat-label>
            <input matInput [matDatepicker]="startPicker" [(ngModel)]="startDate">
            <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
            <mat-datepicker #startPicker></mat-datepicker>
          </mat-form-field>
          
          <mat-form-field appearance="outline">
            <mat-label>Data Final</mat-label>
            <input matInput [matDatepicker]="endPicker" [(ngModel)]="endDate">
            <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
            <mat-datepicker #endPicker></mat-datepicker>
          </mat-form-field>
          
          <button mat-raised-button color="primary" (click)="loadLogs()">
            Buscar
          </button>
        </div>
        
        <div class="statistics" *ngIf="statistics">
          <mat-card class="stat-card">
            <h3>{{ statistics.totalBlocked }}</h3>
            <p>Total Bloqueados</p>
          </mat-card>
          
          <mat-card class="stat-card">
            <h3>{{ statistics.topBlockedIps.length }}</h3>
            <p>IPs Únicos</p>
          </mat-card>
        </div>
        
        <mat-table [dataSource]="logs">
          <ng-container matColumnDef="blockedAt">
            <mat-header-cell *matHeaderCellDef>Data/Hora</mat-header-cell>
            <mat-cell *matCellDef="let log">{{ log.blockedAt | date:'short' }}</mat-cell>
          </ng-container>
          
          <ng-container matColumnDef="ipAddress">
            <mat-header-cell *matHeaderCellDef>IP</mat-header-cell>
            <mat-cell *matCellDef="let log">
              {{ log.ipAddress }}
              <mat-chip-list>
                <mat-chip *ngIf="log.isVpn" color="warn" selected>VPN</mat-chip>
                <mat-chip *ngIf="log.isProxy" color="warn" selected>Proxy</mat-chip>
                <mat-chip *ngIf="log.isTor" color="warn" selected>Tor</mat-chip>
              </mat-chip-list>
            </mat-cell>
          </ng-container>
          
          <ng-container matColumnDef="country">
            <mat-header-cell *matHeaderCellDef>País</mat-header-cell>
            <mat-cell *matCellDef="let log">
              <span class="fi fi-{{ log.countryCode.toLowerCase() }}"></span>
              {{ log.countryName }}
            </mat-cell>
          </ng-container>
          
          <ng-container matColumnDef="reason">
            <mat-header-cell *matHeaderCellDef>Motivo</mat-header-cell>
            <mat-cell *matCellDef="let log">{{ log.reason }}</mat-cell>
          </ng-container>
          
          <ng-container matColumnDef="threatScore">
            <mat-header-cell *matHeaderCellDef>Ameaça</mat-header-cell>
            <mat-cell *matCellDef="let log">
              <mat-progress-bar mode="determinate" [value]="log.threatScore" 
                               [color]="log.threatScore > 75 ? 'warn' : 'primary'">
              </mat-progress-bar>
              {{ log.threatScore }}%
            </mat-cell>
          </ng-container>
          
          <mat-header-row *matHeaderRowDef="logColumns"></mat-header-row>
          <mat-row *matRowDef="let row; columns: logColumns"></mat-row>
        </mat-table>
      </mat-card-content>
    </mat-card>
  `
})
export class BlockedAccessLogsComponent implements OnInit {
  logs: BlockedAccessLog[] = [];
  statistics: BlockStatistics | null = null;
  startDate = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000);  // 7 dias atrás
  endDate = new Date();
  
  logColumns = ['blockedAt', 'ipAddress', 'country', 'reason', 'threatScore'];
  
  constructor(private ipBlockingService: IpBlockingService) {}
  
  async ngOnInit() {
    await this.loadLogs();
  }
  
  async loadLogs() {
    this.logs = await this.ipBlockingService.getBlockedAccessLogs(this.startDate, this.endDate);
    this.statistics = await this.ipBlockingService.getBlockStatistics(this.startDate, this.endDate);
  }
}
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar entidades (IpBlockRule, GeoBlockRule, BlockedAccessLog)
- [ ] Implementar repositórios
- [ ] Criar IpBlockingService
- [ ] Criar middleware IpBlockingMiddleware
- [ ] Integração MaxMind GeoIP2
- [ ] Integração IPQualityScore
- [ ] Integração AbuseIPDB
- [ ] Sistema de auto-bloqueio
- [ ] CIDR support (bloqueio de ranges)
- [ ] Controllers REST
- [ ] Migrations

### Frontend
- [ ] IpRulesListComponent
- [ ] AddIpRuleDialogComponent
- [ ] GeoBlockingConfigComponent
- [ ] BlockedAccessLogsComponent
- [ ] IpBlockingService (Angular)
- [ ] Dashboard de segurança
- [ ] Notificações de bloqueios

### Integrações
- [ ] MaxMind GeoIP2 Database
- [ ] IPQualityScore API
- [ ] AbuseIPDB API
- [ ] Cloudflare (opcional)

### Testes
- [ ] Testes unitários
- [ ] Testes de middleware
- [ ] Testes de integração com APIs
- [ ] Testes de CIDR matching

## 💰 Investimento

- **Esforço**: 1 mês | 1 dev
- **Custo Desenvolvimento**: R$ 45k
- **Custos Mensais**:
  - MaxMind GeoIP2: R$ 0-200/mês
  - IPQualityScore: R$ 0-500/mês (5k requests grátis)
  - AbuseIPDB: Grátis (1k checks/dia)

### ROI Esperado
- Redução de ataques: 70-80%
- Economia em infra: R$ 500-2k/mês
- Proteção contra fraudes: Inestimável

## 🎯 Critérios de Aceitação

- [ ] Bloqueio de IPs individuais funciona
- [ ] Bloqueio de ranges CIDR funciona
- [ ] Geo-blocking por país funciona
- [ ] Detecção de VPN/Proxy/Tor funciona
- [ ] Lista branca tem prioridade sobre blacklist
- [ ] Auto-bloqueio de força bruta funciona
- [ ] Logs detalhados salvos
- [ ] Dashboard de estatísticas funcional
- [ ] Regras temporárias com expiração funcionam
- [ ] Performance aceitável (<50ms overhead)
