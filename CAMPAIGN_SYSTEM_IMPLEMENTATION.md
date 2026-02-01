# Sistema de Campanhas para Planos de Assinatura

## 📋 Visão Geral

Este documento descreve a implementação do sistema de campanhas promocionais para planos de assinatura, permitindo que o sistema exiba planos com preços promocionais Early Adopter alinhados com o documento `PLANO_LANCAMENTO_MVP_SAAS.md`.

## 🎯 Problema Resolvido

Antes desta implementação, havia uma desconexão entre:
- **Frontend**: Planos hardcoded com preços Early Adopter (R$49/R$89/R$149)
- **Backend**: Planos no banco de dados com preços diferentes (R$99/R$199/R$399/R$999)

## ✨ Solução Implementada

### Backend

#### 1. Extensão da Entidade `SubscriptionPlan`

Adicionados 11 novos campos para suportar campanhas:

```csharp
// Informações da campanha
public string? CampaignName { get; private set; }
public string? CampaignDescription { get; private set; }

// Preços
public decimal? OriginalPrice { get; private set; }      // Preço futuro/normal
public decimal? CampaignPrice { get; private set; }      // Preço promocional

// Período da campanha
public DateTime? CampaignStartDate { get; private set; }
public DateTime? CampaignEndDate { get; private set; }   // null = vitalício

// Controle de vagas Early Adopter
public int? MaxEarlyAdopters { get; private set; }
public int CurrentEarlyAdopters { get; private set; }

// Recursos e benefícios (JSON)
public string? EarlyAdopterBenefits { get; private set; }
public string? FeaturesAvailable { get; private set; }
public string? FeaturesInDevelopment { get; private set; }
```

#### 2. Métodos de Negócio

```csharp
// Configuração de campanha
SetCampaignPricing(name, description, originalPrice, campaignPrice, startDate, endDate, maxEarlyAdopters)
ClearCampaignPricing()

// Verificação de status
bool IsCampaignActive()
bool CanJoinCampaign()
decimal GetEffectivePrice()
int GetSavingsPercentage()

// Gestão de Early Adopters
IncrementEarlyAdopters()

// Gestão de arrays JSON
SetEarlyAdopterBenefits(string[])
GetEarlyAdopterBenefits()
SetFeaturesAvailable(string[])
GetFeaturesAvailable()
SetFeaturesInDevelopment(string[])
GetFeaturesInDevelopment()
```

#### 3. Migration

**Nome**: `20260201180912_AddCampaignFieldsToSubscriptionPlan`

Adiciona todas as colunas necessárias com tipos apropriados:
- Colunas de texto com limites de tamanho
- Decimais com precisão `decimal(18,2)`
- Campos JSON usando `jsonb` (PostgreSQL)
- Índices em `CampaignName` e período de campanha

#### 4. DTOs Atualizados

`SubscriptionPlanDto` agora inclui:
```csharp
// Campos de campanha
public string? CampaignName { get; set; }
public decimal? OriginalPrice { get; set; }
public decimal? CampaignPrice { get; set; }
public bool IsCampaignActive { get; set; }
public bool CanJoinCampaign { get; set; }
public decimal EffectivePrice { get; set; }
public int SavingsPercentage { get; set; }
public List<string> EarlyAdopterBenefits { get; set; }
public List<string> FeaturesAvailable { get; set; }
public List<string> FeaturesInDevelopment { get; set; }
// ... outros campos
```

#### 5. Controllers

**RegistrationController** (`/api/registration/plans`):
- Retorna planos com todos os dados de campanha
- Calcula automaticamente preço efetivo e porcentagem de economia

**SystemAdminController**:
- `POST /api/system-admin/subscription-plans`: Criar plano com campanha
- `PUT /api/system-admin/subscription-plans/{id}`: Atualizar plano com campanha

#### 6. RegistrationService

Atualizado para:
```csharp
// Usa preço da campanha quando disponível
var effectivePrice = plan.GetEffectivePrice();

// Incrementa contador de early adopters
if (plan.CanJoinCampaign())
{
    plan.IncrementEarlyAdopters();
    await _subscriptionPlanRepository.UpdateAsync(plan);
}
```

#### 7. Seeder com Planos MVP

Três planos configurados conforme `PLANO_LANCAMENTO_MVP_SAAS.md`:

**Starter** (Básico):
- Early Adopter: R$ 49/mês
- Futuro: R$ 149/mês
- Economia: 67% (R$ 100/mês)
- Limite: 100 vagas
- Usuários: 1
- Pacientes: 50

**Professional** (Intermediário):
- Early Adopter: R$ 89/mês
- Futuro: R$ 239/mês
- Economia: 63% (R$ 150/mês)
- Limite: 100 vagas
- Usuários: 2
- Pacientes: 200

**Enterprise** (Avançado):
- Early Adopter: R$ 149/mês
- Futuro: R$ 389/mês
- Economia: 62% (R$ 240/mês)
- Limite: 100 vagas
- Usuários: 5
- Pacientes: Ilimitados

### Frontend

#### 1. Modelo Atualizado

`SubscriptionPlan` interface estendida com campos de campanha:
```typescript
export interface SubscriptionPlan {
  // ... campos existentes
  
  // Campos de campanha
  campaignName?: string;
  campaignPrice?: number;
  originalPrice?: number;
  isCampaignActive?: boolean;
  canJoinCampaign?: boolean;
  effectivePrice?: number;
  maxEarlyAdopters?: number;
  currentEarlyAdopters?: number;
  // ... outros
}
```

#### 2. PricingComponent

Atualizado para mapear dados da API:
```typescript
this.plans = plans.map(plan => ({
  ...plan,
  // Compatibilidade com template existente
  isMvp: plan.isCampaignActive || false,
  earlyAdopterPrice: plan.campaignPrice,
  futurePrice: plan.originalPrice,
  savingsPercentage: plan.savingsPercentage,
  // Prioriza featuresAvailable da campanha
  features: plan.featuresAvailable?.length 
    ? plan.featuresAvailable 
    : plan.features
}));
```

#### 3. Template Existente

O template `pricing.html` já estava preparado para exibir:
- Badge MVP Launch
- Comparação de preços (Early Adopter vs Futuro)
- Porcentagem de economia
- Lista de recursos disponíveis
- Lista de recursos em desenvolvimento
- Benefícios exclusivos Early Adopter

## 🚀 Como Usar

### 1. Executar Migration

```bash
cd src/MedicSoft.Repository
dotnet ef database update
```

### 2. Executar Seeder (Opcional)

Se quiser criar os planos MVP de exemplo:
```bash
# Chamar o endpoint de seed ou executar o DataSeederService
POST /api/data-seeder/seed
```

### 3. Criar Campanha via API

```bash
POST /api/system-admin/subscription-plans
{
  "name": "Professional",
  "description": "Ideal para consultórios pequenos",
  "monthlyPrice": 89.00,
  "maxUsers": 2,
  "maxPatients": 200,
  "maxClinics": 1,
  "trialDays": 14,
  "type": 2,
  "hasReports": true,
  
  // Campos de campanha
  "campaignName": "MVP Early Adopter",
  "campaignDescription": "Preço especial vitalício",
  "originalPrice": 239.00,
  "campaignPrice": 89.00,
  "campaignStartDate": "2026-01-25T00:00:00Z",
  "campaignEndDate": null,
  "maxEarlyAdopters": 100,
  
  // Benefícios (opcional)
  "earlyAdopterBenefits": [
    "Preço vitalício de R$ 89/mês",
    "R$ 100 em créditos",
    "Badge de Fundador"
  ],
  
  // Recursos (opcional)
  "featuresAvailable": [
    "Até 2 usuários",
    "Até 200 pacientes",
    "Prontuário completo"
  ],
  
  "featuresInDevelopment": [
    "WhatsApp API",
    "Dashboard Analytics"
  ]
}
```

### 4. Visualizar no Site

Os planos aparecerão automaticamente na página `/site/pricing` com:
- Badge "MVP Launch" se campanha ativa
- Comparação de preços
- Economia em %
- Recursos disponíveis e em desenvolvimento
- Benefícios exclusivos

## 📊 Estrutura de Dados

### Tabela: SubscriptionPlans

| Campo | Tipo | Descrição |
|-------|------|-----------|
| CampaignName | varchar(200) | Nome da campanha |
| CampaignDescription | varchar(1000) | Descrição da campanha |
| OriginalPrice | decimal(18,2) | Preço original/futuro |
| CampaignPrice | decimal(18,2) | Preço promocional |
| CampaignStartDate | datetime | Início da campanha |
| CampaignEndDate | datetime | Fim (null = vitalício) |
| MaxEarlyAdopters | int | Limite de vagas |
| CurrentEarlyAdopters | int | Vagas ocupadas |
| EarlyAdopterBenefits | jsonb | Array de benefícios |
| FeaturesAvailable | jsonb | Array de recursos |
| FeaturesInDevelopment | jsonb | Array em dev |

### Índices

- `IX_SubscriptionPlans_CampaignName`
- `IX_SubscriptionPlans_CampaignStartDate_CampaignEndDate`

## 🔒 Segurança

✅ **Validações implementadas**:
- Preços não podem ser negativos
- Preço de campanha não pode ser maior que original
- Limite de vagas deve ser positivo
- Campos obrigatórios validados

✅ **CodeQL**: Nenhuma vulnerabilidade encontrada

⚠️ **Concorrência**: 
- O incremento de `CurrentEarlyAdopters` não tem controle de concorrência
- Em alta demanda, pode exceder `MaxEarlyAdopters`
- **Recomendação**: Implementar optimistic locking ou constraint de DB

## 🧪 Testes

### Testes Unitários Sugeridos

```csharp
[Fact]
public void SetCampaignPricing_ShouldSetAllFields()
{
    var plan = new SubscriptionPlan(...);
    plan.SetCampaignPricing("MVP", "Early Adopter", 149m, 49m);
    
    Assert.Equal("MVP", plan.CampaignName);
    Assert.Equal(149m, plan.OriginalPrice);
    Assert.Equal(49m, plan.CampaignPrice);
    Assert.True(plan.IsCampaignActive());
}

[Fact]
public void GetEffectivePrice_WithActiveCampaign_ReturnsCampaignPrice()
{
    var plan = CreatePlanWithCampaign();
    Assert.Equal(49m, plan.GetEffectivePrice());
}

[Fact]
public void IncrementEarlyAdopters_WhenFull_ThrowsException()
{
    var plan = CreateFullCampaign();
    Assert.Throws<InvalidOperationException>(() => 
        plan.IncrementEarlyAdopters());
}
```

### Teste de Integração

```csharp
[Fact]
public async Task Register_WithCampaign_UsesPromoPrice()
{
    // Arrange
    var plan = CreatePlanWithCampaign(originalPrice: 149m, campaignPrice: 49m);
    var request = CreateRegistrationRequest(plan.Id);
    
    // Act
    var result = await _registrationService.RegisterClinicWithOwnerAsync(request);
    
    // Assert
    var subscription = await _subscriptionRepo.GetByClinicIdAsync(result.ClinicId);
    Assert.Equal(49m, subscription.CurrentPrice);
    Assert.Equal(1, plan.CurrentEarlyAdopters);
}
```

## 📚 Referências

- **Documento de requisitos**: `PLANO_LANCAMENTO_MVP_SAAS.md`
- **Migration**: `20260201180912_AddCampaignFieldsToSubscriptionPlan.cs`
- **Entidade**: `src/MedicSoft.Domain/Entities/SubscriptionPlan.cs`
- **Seeder**: `src/MedicSoft.Application/Services/DataSeederService.cs`
- **Frontend**: `frontend/medicwarehouse-app/src/app/pages/site/pricing/`

## 🎯 Próximos Passos (Opcionais)

1. **UI Admin**: Interface visual para criar/editar campanhas
2. **Controle de Concorrência**: Implementar locking para early adopters
3. **Notificações**: Alertar quando campanha estiver acabando
4. **Relatórios**: Dashboard de performance de campanhas
5. **Cupons**: Sistema de cupons de desconto
6. **A/B Testing**: Testar diferentes preços de campanha

## ✅ Checklist de Deploy

- [ ] Executar migration no banco de produção
- [ ] Executar seeder ou criar planos manualmente via API
- [ ] Verificar que os planos aparecem corretamente no frontend
- [ ] Testar fluxo completo de registro com plano de campanha
- [ ] Monitorar contador de early adopters
- [ ] Configurar alertas quando atingir 80% do limite de vagas
- [ ] Documentar processo para time de suporte

---

## 🔒 Controle de Concorrência (Atualização 2026-02-01)

### Problema Identificado

A implementação original do PR #586 tinha uma condição de corrida no método `IncrementEarlyAdopters()`:

```csharp
// PROBLEMA: Race condition
if (plan.CanJoinCampaign())  // Usuário A verifica: 99/100 ✓
{                             // Usuário B verifica: 99/100 ✓
    plan.IncrementEarlyAdopters();  // A incrementa: 100
    await _subscriptionPlanRepository.UpdateAsync(plan);
}                             // B incrementa: 101 ❌ (excede o limite!)
```

Sob alta carga, múltiplos usuários poderiam exceder o `MaxEarlyAdopters`.

### Solução Implementada

#### 1. Controle de Concorrência Otimista (PostgreSQL xmin)

```csharp
// SubscriptionPlan.cs
public uint RowVersion { get; private set; }

// SubscriptionPlanConfiguration.cs
builder.Property(sp => sp.RowVersion)
    .HasColumnName("xmin")
    .HasColumnType("xid")
    .IsRowVersion()
    .ValueGeneratedOnAddOrUpdate()
    .IsConcurrencyToken();
```

**Benefícios**:
- Usa coluna de sistema nativa do PostgreSQL (sem overhead)
- EF Core detecta automaticamente modificações concorrentes
- Lança `DbUpdateConcurrencyException` em caso de conflito

#### 2. Constraint de Banco de Dados

```sql
ALTER TABLE "SubscriptionPlans"
ADD CONSTRAINT "CK_SubscriptionPlans_EarlyAdoptersLimit"
CHECK ("MaxEarlyAdopters" IS NULL OR "CurrentEarlyAdopters" <= "MaxEarlyAdopters");
```

**Benefícios**:
- Defesa em profundidade (defense-in-depth)
- Protege contra bugs na aplicação
- Garante integridade mesmo sob ataque

#### 3. Lógica de Retry com Exponential Backoff

```csharp
// RegistrationService.cs
for (int attempt = 1; attempt <= MaxCampaignJoinRetries; attempt++)
{
    try
    {
        return await RegisterClinicWithCampaignAsync(...);
    }
    catch (DbUpdateConcurrencyException) when (attempt < MaxCampaignJoinRetries)
    {
        // Recarrega o plano e tenta novamente
        plan = await _subscriptionPlanRepository.GetByIdAsync(...);
        if (!plan.CanJoinCampaign())
            return RegistrationResult.CreateFailure("Campaign is no longer available");
        
        await Task.Delay(100 * attempt); // 100ms, 200ms, 300ms
    }
}
```

**Configuração**:
- `MaxCampaignJoinRetries = 3`
- Backoff: 100ms → 200ms → 300ms
- Mensagens de erro amigáveis

### Garantias de Concorrência

| Cenário | Proteção | Resultado |
|---------|----------|-----------|
| 2 usuários simultâneos | xmin + retry | ✅ Um sucesso, um retry |
| 10 usuários simultâneos | xmin + retry | ✅ Ordem serializada |
| Vaga 100 disputada | xmin + constraint | ✅ Apenas um ganha |
| Bug na aplicação | Constraint DB | ✅ Bloqueado no banco |

### Testes Unitários

14 novos testes adicionados em `SubscriptionPlanTests.cs`:

```csharp
[Fact]
public void IsCampaignActive_WhenSlotsAreFull_ReturnsFalse()
[Fact]
public void IncrementEarlyAdopters_WhenCampaignIsFull_ThrowsInvalidOperationException()
[Fact]
public void CanJoinCampaign_WithAvailableSlots_ReturnsTrue()
// ... mais 11 testes
```

### Migration

**Arquivo**: `20260201183349_AddConcurrencyControlToSubscriptionPlan.cs`

**Ações**:
1. Adiciona coluna `xmin` (uint, xid type)
2. Cria constraint `CK_SubscriptionPlans_EarlyAdoptersLimit`
3. Mantém compatibilidade reversa

**Rollback**:
```bash
dotnet ef migrations remove
# ou
dotnet ef database update PreviousMigration
```

### Monitoramento Recomendado

```sql
-- Alertar quando 80% das vagas forem preenchidas
SELECT 
    "CampaignName",
    "CurrentEarlyAdopters",
    "MaxEarlyAdopters",
    ("CurrentEarlyAdopters" * 100.0 / "MaxEarlyAdopters") as "PercentUsed"
FROM "SubscriptionPlans"
WHERE "MaxEarlyAdopters" IS NOT NULL
    AND "CurrentEarlyAdopters" >= ("MaxEarlyAdopters" * 0.8);
```

### Performance

- **xmin lookup**: O(1) - coluna de sistema
- **Constraint check**: O(1) - validação simples
- **Retry overhead**: Desprezível em casos normais
- **Worst case**: 3 tentativas × 600ms = 1.8s (raro)

---

**Versão**: 1.1  
**Data**: 01 de Fevereiro de 2026  
**Autores**: Sistema de Desenvolvimento Automatizado, PR #586, Correções de Concorrência
