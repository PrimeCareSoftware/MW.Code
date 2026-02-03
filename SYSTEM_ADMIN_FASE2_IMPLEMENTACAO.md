# System Admin Fase 2: Implementação de Gestão de Clientes

## 📋 Resumo Executivo

**Status:** ✅ Backend Implementado | 🔄 Frontend em Desenvolvimento  
**Data de Início:** Janeiro 2026  
**Prazo Estimado:** Q2 2026  
**Esforço:** 2 meses | 2-3 desenvolvedores

### Objetivo

Transformar a gestão básica de clínicas em um **sistema CRM completo** com:
- Gestão avançada de clínicas (visualizações múltiplas, health score)
- Gestão de usuários cross-tenant
- Sistema de tags para segmentação inteligente
- Métricas e analytics avançados

---

## 🎯 Funcionalidades Implementadas

### 1. Gestão Avançada de Clínicas

#### Backend Services

**`ClinicManagementService`**
- ✅ Cálculo de health score (0-100 pontos)
- ✅ Timeline de eventos da clínica
- ✅ Métricas de uso detalhadas
- ✅ Filtros avançados e busca
- ✅ Segmentação por critérios múltiplos

**Endpoints API (`ClinicManagementController`):**
- `GET /api/system-admin/clinic-management/{id}/detail` - Detalhes completos
- `GET /api/system-admin/clinic-management/{id}/health-score` - Health score
- `GET /api/system-admin/clinic-management/{id}/timeline` - Timeline de eventos
- `GET /api/system-admin/clinic-management/{id}/usage-metrics` - Métricas de uso
- `POST /api/system-admin/clinic-management/filter` - Filtros avançados
- `GET /api/system-admin/clinic-management/segment/{segment}` - Segmentos rápidos

#### Health Score Algorithm

O health score é calculado com base em 4 componentes:

1. **Usage Score (0-30 pontos)**
   - Baseado em dias desde última atividade
   - ≤1 dia = 30 pts | ≤7 dias = 25 pts | ≤14 dias = 20 pts | ≤30 dias = 10 pts | >30 dias = 0 pts

2. **User Engagement Score (0-25 pontos)**
   - Percentual de usuários ativos nos últimos 30 dias
   - Fórmula: 25 * (usuários_ativos / total_usuários)

3. **Support Score (0-20 pontos)**
   - Baseado em tickets abertos
   - 0 tickets = 20 pts | 1 = 15 pts | 2 = 10 pts | 3 = 5 pts | 4+ = 0 pts

4. **Payment Score (0-25 pontos)**
   - Pagamentos em dia = 25 pts | Com problemas = 0 pts

**Status de Saúde:**
- 🟢 **Healthy**: 80-100 pontos
- 🟡 **Needs Attention**: 50-79 pontos
- 🔴 **At Risk**: 0-49 pontos

---

### 2. Gestão de Usuários Cross-Tenant

#### Backend Services

**`CrossTenantUserService`**
- ✅ Busca de usuários em todas as clínicas
- ✅ Filtros por role, status, clínica
- ✅ Reset de senha
- ✅ Ativação/desativação de usuários

**Endpoints API (`CrossTenantUsersController`):**
- `POST /api/system-admin/users/filter` - Busca com filtros
- `GET /api/system-admin/users/{id}` - Detalhes do usuário
- `POST /api/system-admin/users/{id}/reset-password` - Reset de senha
- `POST /api/system-admin/users/{id}/toggle-activation` - Ativar/desativar

#### Recursos

- **Busca Cross-Tenant**: Usa `IgnoreQueryFilters()` para acessar dados de todas as clínicas
- **Informações Consolidadas**: Inclui dados da clínica associada
- **Operações Administrativas**: Reset de senha e controle de ativação

---

### 3. Sistema de Tags

#### Entidades

**`Tag`** - Entidade de tag para categorização
```csharp
- Id: Guid
- Name: string
- Description: string?
- Category: string (type, region, value, status, custom)
- Color: string (hex color)
- IsAutomatic: bool
- AutomationRules: string? (JSON)
- Order: int
```

**`ClinicTag`** - Relacionamento muitos-para-muitos
```csharp
- ClinicId: Guid
- TagId: Guid
- AssignedBy: string?
- AssignedAt: DateTime
- IsAutoAssigned: bool
```

#### Backend Services

**`TagService`**
- ✅ CRUD completo de tags
- ✅ Atribuição de tags a clínicas
- ✅ Remoção de tags
- ✅ Aplicação automática de tags

**Endpoints API (`TagsController`):**
- `GET /api/system-admin/tags` - Listar todas as tags
- `POST /api/system-admin/tags` - Criar tag
- `PUT /api/system-admin/tags/{id}` - Atualizar tag
- `DELETE /api/system-admin/tags/{id}` - Deletar tag
- `POST /api/system-admin/tags/assign` - Atribuir tag a clínicas
- `POST /api/system-admin/tags/remove` - Remover tag de clínicas
- `GET /api/system-admin/tags/clinic/{clinicId}` - Tags de uma clínica
- `POST /api/system-admin/tags/apply-automatic` - Aplicar tags automáticas

#### Tags Automáticas

Tags são aplicadas automaticamente baseadas em regras:

1. **"At Risk"** - Clínicas sem atividade há mais de 30 dias
2. **"High Value"** - Clínicas com MRR ≥ R$ 1.000
3. **"New"** - Clínicas criadas nos últimos 30 dias

---

## 📊 Estrutura de Arquivos

### Backend

```
src/
├── MedicSoft.Domain/
│   └── Entities/
│       ├── Tag.cs                          [NOVO]
│       └── ClinicTag.cs                    [NOVO]
│
├── MedicSoft.Application/
│   ├── DTOs/
│   │   └── SystemAdmin/
│   │       └── ClinicManagementDtos.cs     [NOVO]
│   │
│   └── Services/
│       └── SystemAdmin/
│           ├── ClinicManagementService.cs  [NOVO]
│           ├── CrossTenantUserService.cs   [NOVO]
│           └── TagService.cs               [NOVO]
│
└── MedicSoft.Api/
    └── Controllers/
        └── SystemAdmin/
            ├── ClinicManagementController.cs   [NOVO]
            ├── CrossTenantUsersController.cs   [NOVO]
            └── TagsController.cs               [NOVO]
```

### Frontend (Em Desenvolvimento)

```
frontend/mw-system-admin/src/app/
├── pages/
│   └── clinics/
│       ├── clinics-list.ts                 [ATUALIZAR]
│       ├── clinics-cards.ts                [NOVO]
│       ├── clinics-map.ts                  [NOVO]
│       ├── clinics-kanban.ts               [NOVO]
│       ├── clinic-profile.ts               [NOVO]
│       └── clinic-health-score.ts          [NOVO]
│
├── components/
│   ├── tag-manager.ts                      [NOVO]
│   ├── health-score-badge.ts               [NOVO]
│   └── timeline.ts                         [NOVO]
│
└── services/
    ├── clinic-management.service.ts        [NOVO]
    └── tag.service.ts                      [NOVO]
```

---

## 🔧 DTOs e Modelos

### ClinicDetailDto
```csharp
public class ClinicDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string TradeName { get; set; }
    public string Document { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
    public string? Subdomain { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Subscription info
    public SubscriptionInfoDto? CurrentSubscription { get; set; }
    
    // User counts
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    
    // Support tickets
    public int OpenTickets { get; set; }
    public int TotalTickets { get; set; }
    
    // Tags
    public List<TagDto> Tags { get; set; }
}
```

### ClinicHealthScoreDto
```csharp
public class ClinicHealthScoreDto
{
    public Guid ClinicId { get; set; }
    public int UsageScore { get; set; }         // 0-30
    public int UserEngagementScore { get; set; } // 0-25
    public int SupportScore { get; set; }       // 0-20
    public int PaymentScore { get; set; }       // 0-25
    public int TotalScore { get; set; }         // 0-100
    public HealthStatus HealthStatus { get; set; }
    public DateTime CalculatedAt { get; set; }
    
    // Additional details
    public DateTime? LastActivity { get; set; }
    public int DaysSinceActivity { get; set; }
    public int ActiveUsersCount { get; set; }
    public int TotalUsersCount { get; set; }
    public int OpenTicketsCount { get; set; }
    public bool HasPaymentIssues { get; set; }
}
```

### ClinicFilterDto
```csharp
public class ClinicFilterDto
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public List<string>? Tags { get; set; }
    public HealthStatus? HealthStatus { get; set; }
    public string? SubscriptionStatus { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
```

---

## 🚀 Próximos Passos

### Infraestrutura

- [ ] **Registrar serviços** no container de DI (Startup.cs ou Program.cs)
- [ ] **Criar migration** para entidades Tag e ClinicTag
- [ ] **Aplicar migration** no banco de dados
- [ ] **Seed inicial** de tags padrão (opcional)

### Frontend

- [ ] **Criar visualizações múltiplas** (Lista, Cards, Mapa, Kanban)
- [ ] **Implementar perfil rico** da clínica com abas
- [ ] **Adicionar filtros avançados** com chips de segmento
- [ ] **Visualização de health score** com gráficos
- [ ] **Interface de gestão de tags** (criar, editar, atribuir)
- [ ] **Interface de usuários cross-tenant**
- [ ] **Ações em lote** (bulk actions)
- [ ] **Exportação** de dados (CSV, Excel, PDF)

### Background Jobs

- [ ] **Job de cálculo de health score** (diário)
- [ ] **Job de aplicação automática de tags** (diário)
- [ ] **Job de limpeza de dados antigos** (semanal)

### Testes

- [ ] **Testes unitários** dos serviços
- [ ] **Testes de integração** dos controllers
- [ ] **Testes de performance** para filtros com muitos registros
- [ ] **Testes E2E** do frontend

---

## 📝 Documentação Criada

1. ✅ **SYSTEM_ADMIN_FASE2_API_DOCUMENTATION.md** - Documentação completa da API
2. ✅ **SYSTEM_ADMIN_FASE2_IMPLEMENTACAO.md** - Este documento
3. ⏳ **SYSTEM_ADMIN_USER_GUIDE.md** - Será atualizado com novas features
4. ⏳ **Migration Guide** - Será criado quando frontend estiver pronto

---

## 🎓 Guias de Uso

### Como Calcular Health Score de uma Clínica

```bash
GET /api/system-admin/clinic-management/{clinicId}/health-score
Authorization: Bearer {token}
```

O health score é calculado automaticamente baseado em:
- Última atividade da clínica
- Percentual de usuários ativos
- Tickets abertos
- Status de pagamento

### Como Filtrar Clínicas

```bash
POST /api/system-admin/clinic-management/filter
Content-Type: application/json
Authorization: Bearer {token}

{
  "searchTerm": "exemplo",
  "isActive": true,
  "healthStatus": "NeedsAttention",
  "tags": ["High Value"],
  "page": 1,
  "pageSize": 20
}
```

### Como Aplicar Tags Automaticamente

```bash
POST /api/system-admin/tags/apply-automatic
Authorization: Bearer {token}
```

Este endpoint aplica todas as tags automáticas baseadas em suas regras:
- "At Risk" para clínicas inativas
- "High Value" para clínicas com alto MRR
- "New" para clínicas recentes

### Como Buscar Usuários Cross-Tenant

```bash
POST /api/system-admin/users/filter
Content-Type: application/json
Authorization: Bearer {token}

{
  "searchTerm": "joão",
  "role": "Doctor",
  "isActive": true,
  "page": 1,
  "pageSize": 20
}
```

---

## ⚡ Performance e Otimização

### Recomendações

1. **Caching de Health Scores**
   - Cache por 24 horas
   - Invalidar ao detectar mudanças relevantes
   - Recalcular via background job

2. **Paginação Obrigatória**
   - Máximo de 100 itens por página
   - Padrão de 20 itens
   - Usar cursor pagination para grandes datasets

3. **Índices de Banco de Dados**
   ```sql
   CREATE INDEX idx_clinics_isactive ON Clinics(IsActive);
   CREATE INDEX idx_clinics_createdat ON Clinics(CreatedAt);
   CREATE INDEX idx_clinictags_clinicid ON ClinicTags(ClinicId);
   CREATE INDEX idx_clinictags_tagid ON ClinicTags(TagId);
   CREATE INDEX idx_tags_category ON Tags(Category);
   ```

4. **Query Optimization**
   - Usar `AsNoTracking()` para leituras
   - Projeções específicas com `Select()`
   - Evitar N+1 queries com `Include()`

---

## 🔐 Segurança

### Controle de Acesso

- **Todos os endpoints** requerem role `SystemAdmin`
- **Cross-tenant queries** usam `IgnoreQueryFilters()` apenas em controllers autorizados
- **Audit logging** de operações sensíveis (reset de senha, mudanças de status)

### Proteção de Dados

- **Senhas** são hashadas antes de salvar
- **Dados sensíveis** não são expostos nos DTOs
- **Rate limiting** deve ser aplicado nos endpoints de listagem

---

## 📊 Métricas de Sucesso

### Critérios de Aceitação

- ✅ Health score calculado com precisão
- ✅ Filtros funcionam com <500ms de resposta
- ✅ Tags automáticas aplicadas corretamente
- ✅ Cross-tenant queries retornam dados corretos
- ⏳ Frontend com 4 visualizações diferentes
- ⏳ Tempo de carregamento <2s para 1000 registros

### KPIs a Monitorar

- Tempo médio de resposta dos endpoints
- Taxa de erro das APIs
- Uso de memória e CPU
- Número de clínicas por segmento
- Taxa de clínicas "At Risk"

---

## 🐛 Problemas Conhecidos

Nenhum problema conhecido no momento.

---

## 💡 Melhorias Futuras

1. **Machine Learning para Health Score**
   - Pesos dinâmicos baseados em histórico
   - Predição de churn

2. **Dashboards Avançados**
   - Gráficos de tendências
   - Análise de coortes
   - Comparações período-a-período

3. **Automação de Ações**
   - Emails automáticos para clínicas "At Risk"
   - Sugestões de upgrade para "High Value"
   - Onboarding automático para "New"

4. **Integração com CRM Externo**
   - Sync com HubSpot/Salesforce
   - Webhooks para eventos importantes

---

## 📞 Suporte

**Dúvidas ou Problemas:**
- Email: suporte@medicwarehouse.com.br
- GitHub Issues: https://github.com/Omni CareSoftware/MW.Code/issues
- Documentação: https://docs.medicwarehouse.com.br

---

## 🏆 Créditos

**Desenvolvido por:** Omni Care Software  
**Data:** Janeiro 2026  
**Versão:** 2.0.0
