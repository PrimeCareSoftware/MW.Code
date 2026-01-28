# 📊 Fase 2: Gestão de Clientes - System Admin

**Prioridade:** 🔥🔥 P1 - ALTA  
**Status:** Planejamento  
**Esforço:** 2 meses | 2-3 desenvolvedores  
**Custo Estimado:** R$ 78.000  
**Prazo:** Q2 2026

---

## 📋 Contexto

### Situação Atual

O system-admin possui listagem básica de clínicas, mas falta recursos de CRM para gestão profissional de clientes.

**Funcionalidades Existentes:**
- ✅ Lista simples de clínicas
- ✅ Visualização de detalhes básicos
- ✅ Ativação/desativação de clínicas
- ❌ Sem visão de usuários cross-tenant
- ❌ Sem segmentação ou tags
- ❌ Sem métricas de health/engagement

### Objetivo da Fase 2

Transformar a gestão de clínicas em um **CRM básico** com:
1. Gestão avançada de clínicas (múltiplas visualizações, health score)
2. Gestão de usuários cross-tenant
3. Sistema de tags e categorização inteligente

**Inspiração:** HubSpot CRM, Salesforce, Pipedrive

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais
1. Criar visualizações múltiplas de clínicas (lista, cards, mapa, kanban)
2. Implementar perfil rico de clínica com health score e timeline
3. Adicionar gestão completa de usuários cross-tenant
4. Criar sistema de tags para segmentação

### Benefícios Esperados
- 📊 **Visão 360°** de cada cliente
- 🎯 **Segmentação inteligente** para ações direcionadas
- ⚡ **Gestão proativa** de churn
- 👥 **Controle total** de usuários

---

## 📝 Tarefas Detalhadas

### 1. Gestão de Clínicas Avançada (4 semanas)

#### 1.1 Backend - Serviço de Clínicas Avançado

**Criar serviço com métricas e health score:**
```csharp
// Services/Clinics/ClinicManagementService.cs
public interface IClinicManagementService
{
    Task<ClinicDetailDto> GetClinicDetail(int clinicId);
    Task<ClinicHealthScoreDto> CalculateHealthScore(int clinicId);
    Task<List<ClinicTimelineEventDto>> GetTimeline(int clinicId);
    Task<ClinicUsageMetricsDto> GetUsageMetrics(int clinicId);
    Task<List<ClinicDto>> GetClinicsWithFilters(ClinicFilterDto filters);
    Task<List<ClinicDto>> GetClinicsBySegment(string segmentId);
    Task<byte[]> ExportClinics(List<int> clinicIds, ExportFormat format);
}

public class ClinicManagementService : IClinicManagementService
{
    public async Task<ClinicHealthScoreDto> CalculateHealthScore(int clinicId)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Subscriptions)
            .Include(c => c.Users)
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync(c => c.Id == clinicId);
            
        var score = new ClinicHealthScoreDto();
        
        // 1. Frequência de Uso (0-30 pontos)
        var lastActivity = await GetLastActivity(clinicId);
        var daysSinceActivity = (DateTime.UtcNow - lastActivity).Days;
        score.UsageScore = daysSinceActivity switch
        {
            <= 1 => 30,
            <= 7 => 25,
            <= 14 => 20,
            <= 30 => 10,
            _ => 0
        };
        
        // 2. Usuários Ativos (0-25 pontos)
        var activeUsers = await GetActiveUsersCount(clinicId, days: 30);
        var totalUsers = clinic.Users.Count;
        score.UserEngagementScore = totalUsers > 0 
            ? (int)(25 * (activeUsers / (double)totalUsers))
            : 0;
        
        // 3. Tickets Abertos (0-20 pontos)
        var openTickets = await _context.Tickets
            .CountAsync(t => t.ClinicId == clinicId && t.Status == "Open");
        score.SupportScore = openTickets switch
        {
            0 => 20,
            1 => 15,
            2 => 10,
            3 => 5,
            _ => 0
        };
        
        // 4. Pagamentos em Dia (0-25 pontos)
        var hasPaymentIssues = await HasPaymentIssues(clinicId);
        score.PaymentScore = hasPaymentIssues ? 0 : 25;
        
        // Total Score
        score.TotalScore = score.UsageScore + score.UserEngagementScore 
            + score.SupportScore + score.PaymentScore;
        
        score.HealthStatus = score.TotalScore switch
        {
            >= 80 => HealthStatus.Healthy,
            >= 50 => HealthStatus.NeedsAttention,
            _ => HealthStatus.AtRisk
        };
        
        return score;
    }
    
    public async Task<List<ClinicTimelineEventDto>> GetTimeline(int clinicId)
    {
        var events = new List<ClinicTimelineEventDto>();
        
        // Eventos de assinatura
        var subscriptionEvents = await _context.Subscriptions
            .Where(s => s.ClinicId == clinicId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new ClinicTimelineEventDto
            {
                Type = "subscription",
                Title = $"Plano {s.Plan.Name}",
                Description = s.Status,
                Date = s.CreatedAt,
                Icon = "card_membership"
            })
            .ToListAsync();
            
        // Eventos de tickets
        var ticketEvents = await _context.Tickets
            .Where(t => t.ClinicId == clinicId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(20)
            .Select(t => new ClinicTimelineEventDto
            {
                Type = "ticket",
                Title = $"Ticket #{t.Id}",
                Description = t.Subject,
                Date = t.CreatedAt,
                Icon = "support"
            })
            .ToListAsync();
            
        // Eventos de auditoria
        var auditEvents = await _context.AuditLogs
            .Where(a => a.EntityId == clinicId.ToString() && a.EntityType == "Clinic")
            .OrderByDescending(a => a.CreatedAt)
            .Take(20)
            .Select(a => new ClinicTimelineEventDto
            {
                Type = "audit",
                Title = a.Action,
                Description = a.Changes,
                Date = a.CreatedAt,
                Icon = "history"
            })
            .ToListAsync();
            
        events.AddRange(subscriptionEvents);
        events.AddRange(ticketEvents);
        events.AddRange(auditEvents);
        
        return events.OrderByDescending(e => e.Date).ToList();
    }
}
```

**Endpoints API:**
```csharp
// Controllers/SystemAdmin/ClinicsController.cs
[ApiController]
[Route("api/system-admin/clinics")]
[Authorize(Roles = "SystemAdmin")]
public class ClinicsController : ControllerBase
{
    [HttpGet("{id}/detail")]
    public async Task<ActionResult<ClinicDetailDto>> GetDetail(int id)
    
    [HttpGet("{id}/health-score")]
    public async Task<ActionResult<ClinicHealthScoreDto>> GetHealthScore(int id)
    
    [HttpGet("{id}/timeline")]
    public async Task<ActionResult<List<ClinicTimelineEventDto>>> GetTimeline(int id)
    
    [HttpGet("{id}/usage-metrics")]
    public async Task<ActionResult<ClinicUsageMetricsDto>> GetUsageMetrics(int id)
    
    [HttpPost("filter")]
    public async Task<ActionResult<List<ClinicDto>>> FilterClinics(
        [FromBody] ClinicFilterDto filters)
    
    [HttpPost("export")]
    public async Task<IActionResult> ExportClinics(
        [FromBody] List<int> clinicIds,
        [FromQuery] ExportFormat format = ExportFormat.Csv)
    
    [HttpPost("bulk-action")]
    public async Task<IActionResult> BulkAction(
        [FromBody] BulkActionDto action)
}
```

#### 1.2 Frontend - Múltiplas Visualizações

**A. Lista Melhorada com Filtros Avançados:**
```typescript
// system-admin/src/app/clinics/clinics-list/clinics-list.component.ts
@Component({
  selector: 'app-clinics-list',
  standalone: true,
  template: `
    <div class="clinics-container">
      <!-- View Switcher -->
      <div class="view-controls">
        <mat-button-toggle-group [(value)]="currentView">
          <mat-button-toggle value="list">
            <mat-icon>list</mat-icon> Lista
          </mat-button-toggle>
          <mat-button-toggle value="cards">
            <mat-icon>grid_view</mat-icon> Cards
          </mat-button-toggle>
          <mat-button-toggle value="map">
            <mat-icon>map</mat-icon> Mapa
          </mat-button-toggle>
          <mat-button-toggle value="kanban">
            <mat-icon>view_kanban</mat-icon> Kanban
          </mat-button-toggle>
        </mat-button-toggle-group>
        
        <button mat-raised-button color="primary" (click)="openFilters()">
          <mat-icon>filter_list</mat-icon>
          Filtros {{ activeFiltersCount ? '(' + activeFiltersCount + ')' : '' }}
        </button>
        
        <button mat-button [matMenuTriggerFor]="exportMenu">
          <mat-icon>download</mat-icon> Exportar
        </button>
      </div>
      
      <!-- Segment Quick Filters -->
      <div class="segment-chips">
        <mat-chip-set>
          <mat-chip (click)="applySegment('new')">
            🆕 Novos ({{ segments.new }})
          </mat-chip>
          <mat-chip (click)="applySegment('power')">
            ⚡ Power Users ({{ segments.power }})
          </mat-chip>
          <mat-chip (click)="applySegment('at-risk')">
            ⚠️ Em Risco ({{ segments.atRisk }})
          </mat-chip>
          <mat-chip (click)="applySegment('vip')">
            💎 VIP ({{ segments.vip }})
          </mat-chip>
          <mat-chip (click)="applySegment('trial')">
            🔄 Trial ({{ segments.trial }})
          </mat-chip>
        </mat-chip-set>
      </div>
      
      <!-- Views -->
      <app-clinics-table 
        *ngIf="currentView === 'list'"
        [clinics]="filteredClinics"
        (bulkAction)="handleBulkAction($event)"
      />
      
      <app-clinics-cards 
        *ngIf="currentView === 'cards'"
        [clinics]="filteredClinics"
      />
      
      <app-clinics-map 
        *ngIf="currentView === 'map'"
        [clinics]="filteredClinics"
      />
      
      <app-clinics-kanban 
        *ngIf="currentView === 'kanban'"
        [clinics]="filteredClinics"
      />
    </div>
  `
})
export class ClinicsListComponent implements OnInit {
  currentView: 'list' | 'cards' | 'map' | 'kanban' = 'list';
  clinics: Clinic[] = [];
  filteredClinics: Clinic[] = [];
  segments = { new: 0, power: 0, atRisk: 0, vip: 0, trial: 0 };
  activeFiltersCount = 0;
  
  ngOnInit() {
    this.loadClinics();
    this.loadSegmentCounts();
  }
  
  async loadClinics() {
    this.clinics = await this.clinicService.getAll();
    this.filteredClinics = this.clinics;
  }
  
  applySegment(segment: string) {
    this.filteredClinics = this.clinics.filter(c => 
      this.matchesSegment(c, segment)
    );
  }
  
  handleBulkAction(action: BulkAction) {
    const selectedIds = action.clinicIds;
    
    switch (action.type) {
      case 'send-email':
        this.openEmailModal(selectedIds);
        break;
      case 'change-plan':
        this.openPlanChangeModal(selectedIds);
        break;
      case 'add-tag':
        this.openTagModal(selectedIds);
        break;
      case 'export':
        this.exportClinics(selectedIds);
        break;
    }
  }
}
```

**B. Perfil Rico de Clínica:**
```typescript
// system-admin/src/app/clinics/clinic-profile/clinic-profile.component.ts
@Component({
  selector: 'app-clinic-profile',
  standalone: true,
  template: `
    <div class="profile-container">
      <!-- Header -->
      <div class="profile-header">
        <div class="clinic-info">
          <h1>{{ clinic.name }}</h1>
          <p>{{ clinic.cnpj }} • {{ clinic.email }}</p>
          <div class="health-score">
            <mat-icon [class]="healthStatusClass">
              {{ healthStatusIcon }}
            </mat-icon>
            <span>Health Score: {{ healthScore.totalScore }}/100</span>
          </div>
        </div>
        
        <!-- Quick Actions -->
        <div class="quick-actions">
          <button mat-raised-button (click)="loginAsClinic()">
            🔓 Login como
          </button>
          <button mat-button (click)="sendEmail()">
            📧 Enviar Email
          </button>
          <button mat-button (click)="createTicket()">
            💬 Criar Ticket
          </button>
          <button mat-button [matMenuTriggerFor]="moreActions">
            Mais Ações
          </button>
        </div>
      </div>
      
      <!-- Tabs -->
      <mat-tab-group>
        <!-- Informações Básicas -->
        <mat-tab label="Informações">
          <app-clinic-basic-info [clinic]="clinic" />
        </mat-tab>
        
        <!-- Timeline -->
        <mat-tab label="Timeline">
          <div class="timeline">
            <div *ngFor="let event of timeline" class="timeline-item">
              <mat-icon class="timeline-icon">{{ event.icon }}</mat-icon>
              <div class="timeline-content">
                <h4>{{ event.title }}</h4>
                <p>{{ event.description }}</p>
                <span class="timeline-date">{{ event.date | date }}</span>
              </div>
            </div>
          </div>
        </mat-tab>
        
        <!-- Métricas de Uso -->
        <mat-tab label="Métricas">
          <div class="usage-metrics">
            <div class="metric-card">
              <h3>Usuários Ativos</h3>
              <div class="metric-value">
                {{ usageMetrics.dau }} DAU / {{ usageMetrics.mau }} MAU
              </div>
            </div>
            <div class="metric-card">
              <h3>Consultas Realizadas</h3>
              <div class="metric-value">
                {{ usageMetrics.consultations }} (30 dias)
              </div>
            </div>
            <div class="metric-card">
              <h3>Pacientes Cadastrados</h3>
              <div class="metric-value">
                {{ usageMetrics.patients }}
              </div>
            </div>
            <div class="metric-card">
              <h3>Features Mais Usadas</h3>
              <ul>
                <li *ngFor="let feature of usageMetrics.topFeatures">
                  {{ feature.name }} ({{ feature.usage }}%)
                </li>
              </ul>
            </div>
          </div>
        </mat-tab>
        
        <!-- Health Score Detalhado -->
        <mat-tab label="Health Score">
          <div class="health-breakdown">
            <app-score-gauge
              label="Frequência de Uso"
              [value]="healthScore.usageScore"
              [max]="30"
            />
            <app-score-gauge
              label="Engajamento de Usuários"
              [value]="healthScore.userEngagementScore"
              [max]="25"
            />
            <app-score-gauge
              label="Suporte"
              [value]="healthScore.supportScore"
              [max]="20"
            />
            <app-score-gauge
              label="Pagamentos"
              [value]="healthScore.paymentScore"
              [max]="25"
            />
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `
})
export class ClinicProfileComponent implements OnInit {
  clinic: Clinic;
  healthScore: ClinicHealthScore;
  timeline: TimelineEvent[];
  usageMetrics: UsageMetrics;
  
  async ngOnInit() {
    const clinicId = this.route.snapshot.params['id'];
    await Promise.all([
      this.loadClinic(clinicId),
      this.loadHealthScore(clinicId),
      this.loadTimeline(clinicId),
      this.loadUsageMetrics(clinicId)
    ]);
  }
  
  async loginAsClinic() {
    const confirmed = await this.confirmDialog.open(
      'Você está prestes a fazer login como esta clínica. Isso será registrado no audit log.'
    );
    
    if (confirmed) {
      const token = await this.adminService.impersonateClinic(this.clinic.id);
      // Abrir em nova aba com banner de admin mode
      window.open(`/admin-impersonate?token=${token}`, '_blank');
    }
  }
}
```

**C. Visualização Kanban:**
```typescript
// system-admin/src/app/clinics/clinics-kanban/clinics-kanban.component.ts
@Component({
  selector: 'app-clinics-kanban',
  standalone: true,
  template: `
    <div class="kanban-board">
      <div class="kanban-column" *ngFor="let status of statuses">
        <div class="column-header">
          <h3>{{ status.label }}</h3>
          <span class="count">{{ getCount(status.value) }}</span>
        </div>
        
        <div 
          cdkDropList
          [cdkDropListData]="getClinicsByStatus(status.value)"
          (cdkDropListDropped)="onDrop($event, status.value)"
          class="clinic-list"
        >
          <div
            *ngFor="let clinic of getClinicsByStatus(status.value)"
            cdkDrag
            class="clinic-card"
          >
            <h4>{{ clinic.name }}</h4>
            <p>{{ clinic.plan }}</p>
            <div class="health-indicator" [class]="clinic.healthStatus">
              {{ clinic.healthScore }}
            </div>
            <div class="tags">
              <mat-chip *ngFor="let tag of clinic.tags">
                {{ tag }}
              </mat-chip>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class ClinicsKanbanComponent {
  @Input() clinics: Clinic[];
  
  statuses = [
    { value: 'trial', label: '🔄 Trial' },
    { value: 'active', label: '✅ Ativo' },
    { value: 'at-risk', label: '⚠️ Em Risco' },
    { value: 'churned', label: '❌ Cancelado' }
  ];
  
  getClinicsByStatus(status: string): Clinic[] {
    return this.clinics.filter(c => c.status === status);
  }
  
  async onDrop(event: CdkDragDrop<Clinic[]>, newStatus: string) {
    const clinic = event.item.data;
    await this.clinicService.updateStatus(clinic.id, newStatus);
    // Atualizar UI
  }
}
```

---

### 2. Gestão de Usuários Cross-Tenant (2 semanas)

#### 2.1 Backend - User Management Service

```csharp
// Services/Users/CrossTenantUserService.cs
public interface ICrossTenantUserService
{
    Task<List<UserDto>> GetAllUsers(UserFilterDto filters);
    Task<UserDetailDto> GetUserDetail(int userId);
    Task<List<ClinicDto>> GetUserClinics(int userId);
    Task ResetPassword(int userId);
    Task ToggleUserStatus(int userId, bool isActive);
    Task TransferOwnership(int userId, int targetUserId);
}

public class CrossTenantUserService : ICrossTenantUserService
{
    public async Task<List<UserDto>> GetAllUsers(UserFilterDto filters)
    {
        // IMPORTANTE: usar IgnoreQueryFilters() para cross-tenant
        var query = _context.Users
            .IgnoreQueryFilters()
            .AsQueryable();
            
        if (filters.ClinicId.HasValue)
            query = query.Where(u => u.ClinicId == filters.ClinicId);
            
        if (!string.IsNullOrEmpty(filters.Role))
            query = query.Where(u => u.Role == filters.Role);
            
        if (filters.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filters.IsActive);
            
        if (filters.LastLoginBefore.HasValue)
            query = query.Where(u => u.LastLoginAt < filters.LastLoginBefore);
            
        return await query
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                ClinicName = u.Clinic.Name,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync();
    }
}
```

#### 2.2 Frontend - User Management

```typescript
// system-admin/src/app/users/users-list.component.ts
@Component({
  selector: 'app-users-list',
  standalone: true,
  template: `
    <div class="users-container">
      <div class="filters">
        <mat-form-field>
          <input matInput placeholder="Buscar..." [(ngModel)]="searchQuery">
        </mat-form-field>
        
        <mat-form-field>
          <mat-select placeholder="Clínica" [(ngModel)]="selectedClinic">
            <mat-option [value]="null">Todas</mat-option>
            <mat-option *ngFor="let clinic of clinics" [value]="clinic.id">
              {{ clinic.name }}
            </mat-option>
          </mat-select>
        </mat-form-field>
        
        <mat-form-field>
          <mat-select placeholder="Role" [(ngModel)]="selectedRole">
            <mat-option [value]="null">Todas</mat-option>
            <mat-option value="Owner">Owner</mat-option>
            <mat-option value="Admin">Admin</mat-option>
            <mat-option value="Doctor">Médico</mat-option>
            <mat-option value="Secretary">Secretária</mat-option>
          </mat-select>
        </mat-form-field>
      </div>
      
      <table mat-table [dataSource]="users">
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Nome</th>
          <td mat-cell *matCellDef="let user">{{ user.name }}</td>
        </ng-container>
        
        <ng-container matColumnDef="email">
          <th mat-header-cell *matHeaderCellDef>Email</th>
          <td mat-cell *matCellDef="let user">{{ user.email }}</td>
        </ng-container>
        
        <ng-container matColumnDef="clinic">
          <th mat-header-cell *matHeaderCellDef>Clínica</th>
          <td mat-cell *matCellDef="let user">{{ user.clinicName }}</td>
        </ng-container>
        
        <ng-container matColumnDef="role">
          <th mat-header-cell *matHeaderCellDef>Role</th>
          <td mat-cell *matCellDef="let user">
            <mat-chip>{{ user.role }}</mat-chip>
          </td>
        </ng-container>
        
        <ng-container matColumnDef="lastLogin">
          <th mat-header-cell *matHeaderCellDef>Último Login</th>
          <td mat-cell *matCellDef="let user">
            {{ user.lastLoginAt | date }}
          </td>
        </ng-container>
        
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Ações</th>
          <td mat-cell *matCellDef="let user">
            <button mat-icon-button [matMenuTriggerFor]="userMenu">
              <mat-icon>more_vert</mat-icon>
            </button>
            <mat-menu #userMenu="matMenu">
              <button mat-menu-item (click)="viewUser(user)">
                Ver Detalhes
              </button>
              <button mat-menu-item (click)="resetPassword(user)">
                Resetar Senha
              </button>
              <button mat-menu-item (click)="toggleStatus(user)">
                {{ user.isActive ? 'Desativar' : 'Ativar' }}
              </button>
            </mat-menu>
          </td>
        </ng-container>
        
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
    </div>
  `
})
export class UsersListComponent implements OnInit {
  users: User[] = [];
  clinics: Clinic[] = [];
  displayedColumns = ['name', 'email', 'clinic', 'role', 'lastLogin', 'actions'];
  
  async resetPassword(user: User) {
    const confirmed = await this.confirmDialog.open(
      `Resetar senha de ${user.name}? Um email será enviado.`
    );
    
    if (confirmed) {
      await this.userService.resetPassword(user.id);
      this.snackBar.open('Email de reset enviado!', 'OK', { duration: 3000 });
    }
  }
}
```

---

### 3. Sistema de Tags e Categorização (2 semanas)

#### 3.1 Backend - Tagging System

```csharp
// Entities/Tag.cs
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; } // HEX color
    public string Category { get; set; } // tipo, região, valor, status, custom
    public bool IsSystem { get; set; } // tags automáticas
    public DateTime CreatedAt { get; set; }
}

// Entities/ClinicTag.cs
public class ClinicTag
{
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } // user ou 'system' se automático
}

// Services/Tags/TagService.cs
public interface ITagService
{
    Task<List<TagDto>> GetAllTags();
    Task<TagDto> CreateTag(CreateTagDto dto);
    Task AddTagToClinic(int clinicId, int tagId);
    Task RemoveTagFromClinic(int clinicId, int tagId);
    Task ApplyAutomaticTags(); // Background job
}

public class TagService : ITagService
{
    public async Task ApplyAutomaticTags()
    {
        // Tag "At-risk" - Inatividade > 30 dias
        var inactiveClinics = await _context.Clinics
            .Where(c => c.LastActivityAt < DateTime.UtcNow.AddDays(-30))
            .ToListAsync();
            
        var atRiskTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == "At-risk" && t.IsSystem);
            
        foreach (var clinic in inactiveClinics)
        {
            await AddTagToClinic(clinic.Id, atRiskTag.Id);
        }
        
        // Tag "High-value" - MRR > R$ 1000
        var highValueClinics = await _context.Subscriptions
            .Where(s => s.Mrr > 1000)
            .Select(s => s.ClinicId)
            .ToListAsync();
            
        var highValueTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == "High-value" && t.IsSystem);
            
        foreach (var clinicId in highValueClinics)
        {
            await AddTagToClinic(clinicId, highValueTag.Id);
        }
        
        // Mais regras automáticas...
    }
}
```

#### 3.2 Frontend - Tag Management

```typescript
// system-admin/src/app/tags/tag-manager.component.ts
@Component({
  selector: 'app-tag-manager',
  standalone: true,
  template: `
    <div class="tag-manager">
      <button mat-raised-button color="primary" (click)="createTag()">
        Nova Tag
      </button>
      
      <div class="tag-categories">
        <div *ngFor="let category of categories" class="category-section">
          <h3>{{ category.label }}</h3>
          <div class="tag-list">
            <mat-chip 
              *ngFor="let tag of getTagsByCategory(category.value)"
              [style.background-color]="tag.color"
              [removable]="!tag.isSystem"
              (removed)="deleteTag(tag)"
            >
              {{ tag.name }}
              <mat-icon matChipRemove *ngIf="!tag.isSystem">cancel</mat-icon>
            </mat-chip>
          </div>
        </div>
      </div>
    </div>
  `
})
export class TagManagerComponent {
  tags: Tag[] = [];
  categories = [
    { value: 'tipo', label: '🏢 Tipo' },
    { value: 'região', label: '🌎 Região' },
    { value: 'valor', label: '💰 Valor' },
    { value: 'status', label: '🎯 Status' },
    { value: 'custom', label: '🔧 Customizadas' }
  ];
  
  async createTag() {
    const dialogRef = this.dialog.open(CreateTagDialogComponent);
    const result = await dialogRef.afterClosed().toPromise();
    
    if (result) {
      await this.tagService.create(result);
      this.loadTags();
    }
  }
}

// Dialog component
@Component({
  selector: 'app-create-tag-dialog',
  template: `
    <h2 mat-dialog-title>Nova Tag</h2>
    <mat-dialog-content>
      <mat-form-field>
        <input matInput placeholder="Nome" [(ngModel)]="tag.name">
      </mat-form-field>
      
      <mat-form-field>
        <mat-select placeholder="Categoria" [(ngModel)]="tag.category">
          <mat-option value="tipo">Tipo</mat-option>
          <mat-option value="região">Região</mat-option>
          <mat-option value="valor">Valor</mat-option>
          <mat-option value="status">Status</mat-option>
          <mat-option value="custom">Customizada</mat-option>
        </mat-select>
      </mat-form-field>
      
      <div class="color-picker">
        <label>Cor:</label>
        <input type="color" [(ngModel)]="tag.color">
      </div>
    </mat-dialog-content>
    <mat-dialog-actions>
      <button mat-button mat-dialog-close>Cancelar</button>
      <button mat-raised-button [mat-dialog-close]="tag" color="primary">
        Criar
      </button>
    </mat-dialog-actions>
  `
})
export class CreateTagDialogComponent {
  tag = { name: '', category: 'custom', color: '#3f51b5' };
}
```

---

## ✅ Critérios de Sucesso

### Gestão de Clínicas
- [ ] 4 visualizações funcionando (lista, cards, mapa, kanban)
- [ ] Filtros avançados com múltiplos critérios
- [ ] Ações em lote implementadas (email, plano, tags)
- [ ] Perfil rico com health score e timeline
- [ ] Health score calculado corretamente
- [ ] Exportação em CSV, Excel e PDF

### Gestão de Usuários
- [ ] Lista cross-tenant funcionando
- [ ] Filtros por clínica, role e status
- [ ] Reset de senha funcional
- [ ] Ativação/desativação de contas
- [ ] Transferência de ownership

### Tags
- [ ] Sistema de tags operacional
- [ ] 5+ categorias de tags
- [ ] Tags automáticas funcionando (background job)
- [ ] Filtros por tags
- [ ] Colorização customizável

### Performance
- [ ] Lista de clínicas carrega em < 2s (1000 registros)
- [ ] Busca e filtros responsivos (< 500ms)
- [ ] Exportação não bloqueia UI

---

## 🧪 Testes e Validação

### 1. Testes Unitários
```csharp
public class ClinicManagementServiceTests
{
    [Fact]
    public async Task CalculateHealthScore_ShouldReturnCorrectScore()
    {
        // Test com clinic ativa, usuários engajados, sem tickets
        var score = await _service.CalculateHealthScore(clinicId);
        Assert.InRange(score.TotalScore, 80, 100);
        Assert.Equal(HealthStatus.Healthy, score.HealthStatus);
    }
    
    [Fact]
    public async Task GetTimeline_ShouldIncludeAllEventTypes()
    {
        var timeline = await _service.GetTimeline(clinicId);
        Assert.Contains(timeline, e => e.Type == "subscription");
        Assert.Contains(timeline, e => e.Type == "ticket");
        Assert.Contains(timeline, e => e.Type == "audit");
    }
}
```

### 2. Testes E2E
```typescript
describe('Clinics Kanban', () => {
  it('should drag clinic to different status', () => {
    cy.visit('/system-admin/clinics?view=kanban');
    cy.get('.clinic-card').first().drag('.kanban-column[data-status="active"]');
    cy.contains('Status atualizado com sucesso');
  });
});
```

---

## 📚 Documentação

### Para Desenvolvedores
- API documentation (Swagger)
- Health score algorithm documentation
- Tag automation rules
- Cross-tenant queries guide

### Para Usuários
- Guia de uso das visualizações
- Como usar tags efetivamente
- Interpretação do health score
- Ações em lote - boas práticas

---

## 🔄 Próximos Passos

Após Fase 2:
1. ✅ Validar com admins
2. ✅ Ajustar health score se necessário
3. ➡️ Prosseguir para **Fase 3: Analytics e BI**

---

## 📞 Referências

### Documentos
- [PLANO_MELHORIAS_SYSTEM_ADMIN_2026.md](../PLANO_MELHORIAS_SYSTEM_ADMIN_2026.md)
- [01-fase1-fundacao-ux.md](./01-fase1-fundacao-ux.md)

### Código Existente
- Backend: `/src/MedicWarehouse.Api/Controllers/SystemAdmin/`
- Frontend: `/system-admin/src/app/clinics/`

### Inspiração
- **HubSpot CRM:** https://www.hubspot.com/products/crm
- **Pipedrive:** https://www.pipedrive.com
- **Salesforce:** https://www.salesforce.com

---

**Criado:** Janeiro 2026  
**Versão:** 1.0  
**Status:** Pronto para implementação
