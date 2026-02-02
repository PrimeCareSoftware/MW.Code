# Guia de Implementação - Plano Financeiro

## Visão Geral
Este guia descreve como implementar os planos financeiros definidos no documento `PLANO_FINANCEIRO_MENSAL.md` no sistema PrimeCare, garantindo que a infraestrutura de assinaturas já existente suporte adequadamente a estratégia de precificação unificada.

## Data de Criação
02 de Fevereiro de 2026

---

## 1. Status Atual da Implementação

### ✅ Já Implementado

#### Backend (PR #608)
- [x] Entidade `SubscriptionPlan` com suporte completo a:
  - Preços mensais e anuais
  - Limites de usuários e pacientes
  - Sistema de campanhas (early adopter)
  - Features disponíveis e em desenvolvimento
  - Benefícios para early adopters
- [x] Entidade `BusinessConfiguration` com:
  - 17 feature flags por tipo de negócio
  - Terminologia personalizada por especialidade
  - Suporte para 8 especialidades diferentes
- [x] Repositórios e serviços de domínio
- [x] DTOs e configurações de banco de dados

#### Frontend (PR #609)
- [x] Componente de configuração de negócio (`business-configuration`)
- [x] Editor de templates (`template-editor`)
- [x] Wizard de onboarding personalizado
- [x] Serviço de terminologia com cache
- [x] Pipe para tradução de termos
- [x] Painel de administração de planos (`mw-system-admin/plans`)

#### Funcionalidades Existentes
- [x] Criação e gestão de planos via API
- [x] Ativação/desativação de planos
- [x] Sistema de campanhas com limite de vagas
- [x] Preços diferenciados para early adopters
- [x] Controle de features por plano

### ⚠️ Pendências Identificadas

Após análise do código, não há pendências críticas de desenvolvimento. A infraestrutura está pronta para suportar o modelo financeiro proposto.

---

## 2. Ajustes nos Planos Existentes

### 2.1 Atualização dos Planos no Sistema

Os planos definidos no `PLANO_FINANCEIRO_MENSAL.md` devem ser criados/atualizados no banco de dados. Veja os valores corretos:

#### Via API REST

**Endpoint**: `POST /api/SystemAdmin/subscription-plans`

```json
{
  "name": "Starter",
  "description": "MVP Básico - Ideal para profissionais autônomos de qualquer especialidade",
  "monthlyPrice": 49.00,
  "yearlyPrice": 490.00,
  "trialDays": 14,
  "maxUsers": 1,
  "maxPatients": 50,
  "campaignName": "Lançamento Early Adopter",
  "campaignDescription": "Preço fixo vitalício para primeiros usuários",
  "originalPrice": 149.00,
  "campaignPrice": 49.00,
  "campaignStartDate": "2026-02-02T00:00:00Z",
  "campaignEndDate": "2026-08-02T00:00:00Z",
  "maxEarlyAdopters": 500,
  "earlyAdopterBenefits": [
    "Preço fixo vitalício de R$ 49/mês",
    "R$ 100 em créditos de serviço",
    "Acesso beta a novos recursos",
    "Badge de Cliente Fundador"
  ],
  "featuresAvailable": [
    "Até 1 usuário",
    "Até 50 pacientes",
    "Agenda de consultas/sessões básica",
    "Cadastro de pacientes",
    "Prontuário digital simples",
    "Relatórios básicos",
    "Terminologia personalizada",
    "Modelos de documentos padrão",
    "Suporte por email (48h)"
  ],
  "featuresInDevelopment": [
    "Integração WhatsApp Business",
    "Lembretes automáticos",
    "Backup automático diário"
  ]
}
```

```json
{
  "name": "Professional",
  "description": "MVP Intermediário - Ideal para consultórios pequenos de qualquer especialidade",
  "monthlyPrice": 89.00,
  "yearlyPrice": 890.00,
  "trialDays": 14,
  "maxUsers": 2,
  "maxPatients": 200,
  "campaignName": "Lançamento Early Adopter",
  "campaignDescription": "Preço fixo vitalício para primeiros usuários - PLANO RECOMENDADO",
  "originalPrice": 239.00,
  "campaignPrice": 89.00,
  "campaignStartDate": "2026-02-02T00:00:00Z",
  "campaignEndDate": "2026-08-02T00:00:00Z",
  "maxEarlyAdopters": 300,
  "earlyAdopterBenefits": [
    "Preço fixo vitalício de R$ 89/mês",
    "R$ 100 em créditos de serviço",
    "Acesso beta a novos recursos",
    "Treinamento personalizado (2h)",
    "Badge de Cliente Fundador"
  ],
  "featuresAvailable": [
    "Até 2 usuários",
    "Até 200 pacientes",
    "Todos os recursos do Starter",
    "Agenda avançada (múltiplos profissionais)",
    "Prontuário digital completo",
    "Módulo Financeiro básico",
    "Relatórios gerenciais",
    "Portal do Paciente (básico)",
    "Templates customizáveis",
    "Business Configuration (16 toggles)",
    "Suporte prioritário (24h)"
  ],
  "featuresInDevelopment": [
    "Integração WhatsApp API",
    "Notificações por SMS",
    "Assinatura digital (ICP-Brasil)",
    "Exportação TISS",
    "Dashboard Analytics",
    "API de Integração"
  ]
}
```

```json
{
  "name": "Enterprise",
  "description": "MVP Avançado - Ideal para clínicas estabelecidas de qualquer especialidade",
  "monthlyPrice": 149.00,
  "yearlyPrice": 1490.00,
  "trialDays": 14,
  "maxUsers": 5,
  "maxPatients": 999999,
  "campaignName": "Lançamento Early Adopter",
  "campaignDescription": "Preço fixo vitalício para primeiros usuários",
  "originalPrice": 389.00,
  "campaignPrice": 149.00,
  "campaignStartDate": "2026-02-02T00:00:00Z",
  "campaignEndDate": "2026-08-02T00:00:00Z",
  "maxEarlyAdopters": 200,
  "earlyAdopterBenefits": [
    "Preço fixo vitalício de R$ 149/mês",
    "R$ 100 em créditos de serviço",
    "Acesso beta a novos recursos",
    "Treinamento personalizado (2h)",
    "Gerente de sucesso dedicado (3 meses)",
    "Badge de Cliente Fundador",
    "Voto no roadmap de desenvolvimento"
  ],
  "featuresAvailable": [
    "Até 5 usuários",
    "Pacientes ilimitados",
    "Todos os recursos do Professional",
    "Módulo Financeiro completo",
    "Gestão de estoque",
    "Fila de espera",
    "Telemedicina básica",
    "Portal do Paciente completo",
    "Editor de templates avançado",
    "Relatórios avançados",
    "Conformidade LGPD",
    "Onboarding wizard personalizado",
    "Suporte 24/7"
  ],
  "featuresInDevelopment": [
    "Assinatura digital (ICP-Brasil)",
    "Exportação TISS completa",
    "BI e Analytics avançado",
    "CRM para gestão de leads",
    "Automação de workflows",
    "Integração com laboratórios",
    "Agendamento online",
    "Marketing automation"
  ]
}
```

### 2.2 Script de Seed para Planos

Adicione ao `DataSeederService.cs` a criação automática dos planos:

```csharp
// Localização: src/MedicSoft.Application/Services/DataSeederService.cs
private async Task SeedSubscriptionPlansAsync()
{
    var starterPlan = new SubscriptionPlan(
        "Starter",
        "MVP Básico - Ideal para profissionais autônomos de qualquer especialidade",
        49.00m,
        14,
        1,
        50,
        SubscriptionPlanType.Basic,
        "system"
    );
    
    starterPlan.SetCampaignPricing(
        "Lançamento Early Adopter",
        "Preço fixo vitalício para primeiros usuários",
        149.00m,
        49.00m,
        DateTime.UtcNow,
        DateTime.UtcNow.AddMonths(6),
        500
    );
    
    starterPlan.SetFeaturesAvailable(new[]
    {
        "Até 1 usuário",
        "Até 50 pacientes",
        "Agenda de consultas/sessões básica",
        "Cadastro de pacientes",
        "Prontuário digital simples",
        "Relatórios básicos",
        "Terminologia personalizada",
        "Modelos de documentos padrão",
        "Suporte por email (48h)"
    });
    
    await _subscriptionPlanRepository.AddAsync(starterPlan);
    
    // Professional e Enterprise seguem o mesmo padrão...
}
```

---

## 3. Dashboard de Métricas Financeiras

### 3.1 Novos Endpoints Necessários

Adicione ao `SystemAdminController` para monitorar métricas do plano financeiro:

```csharp
// GET /api/SystemAdmin/financial-metrics
[HttpGet("financial-metrics")]
public async Task<IActionResult> GetFinancialMetrics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
{
    var metrics = new
    {
        MRR = await CalculateMRR(),
        TotalActiveClients = await GetActiveClientsCount(),
        ClientsByPlan = await GetClientDistributionByPlan(),
        ChurnRate = await CalculateChurnRate(startDate, endDate),
        AverageRevenuePerClient = await CalculateARPC(),
        ClientsBySpecialty = await GetClientDistributionBySpecialty(),
        CampaignProgress = await GetCampaignProgress()
    };
    
    return Ok(metrics);
}

// GET /api/SystemAdmin/revenue-projection
[HttpGet("revenue-projection")]
public async Task<IActionResult> GetRevenueProjection([FromQuery] int months = 12)
{
    var projection = await _saasMetricsService.CalculateRevenueProjection(months);
    return Ok(projection);
}
```

### 3.2 Componente Frontend de Dashboard Financeiro

Crie um novo componente em `frontend/mw-system-admin/src/app/pages/financial-dashboard/`:

```typescript
// financial-dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { SystemAdminService } from '../../services/system-admin';

@Component({
  selector: 'app-financial-dashboard',
  templateUrl: './financial-dashboard.component.html'
})
export class FinancialDashboard implements OnInit {
  mrr = 0;
  totalClients = 0;
  churnRate = 0;
  clientsByPlan: any[] = [];
  clientsBySpecialty: any[] = [];
  revenueProjection: any[] = [];
  
  constructor(private systemAdminService: SystemAdminService) {}
  
  ngOnInit() {
    this.loadFinancialMetrics();
    this.loadRevenueProjection();
  }
  
  loadFinancialMetrics() {
    this.systemAdminService.getFinancialMetrics().subscribe(metrics => {
      this.mrr = metrics.mrr;
      this.totalClients = metrics.totalActiveClients;
      this.churnRate = metrics.churnRate;
      this.clientsByPlan = metrics.clientsByPlan;
      this.clientsBySpecialty = metrics.clientsBySpecialty;
    });
  }
  
  loadRevenueProjection() {
    this.systemAdminService.getRevenueProjection(12).subscribe(projection => {
      this.revenueProjection = projection;
    });
  }
}
```

---

## 4. Integração com Gateway de Pagamento

### 4.1 Configuração Inicial

✅ **IMPLEMENTADO** - O sistema foi configurado para integração com Mercado Pago.

**Status:** Aguardando credenciais do Mercado Pago para ativação completa.

#### Componentes Implementados:

1. **PaymentGatewaySettings** - Configurações do gateway em `appsettings.json`
2. **IPaymentGatewayService** - Interface para operações de pagamento
3. **MercadoPagoPaymentGatewayService** - Implementação do serviço
4. **CreditCardPayments Feature Flag** - Flag para habilitar/desabilitar pagamentos por cartão

#### Configuração em appsettings.json:

```json
"PaymentGateway": {
  "Provider": "MercadoPago",
  "Enabled": true,
  "MercadoPago": {
    "AccessToken": "",
    "PublicKey": "",
    "WebhookSecret": "",
    "ApiUrl": "https://api.mercadopago.com",
    "Enabled": false,
    "NotificationUrl": ""
  },
  "EnableCreditCardPayments": true,
  "EnablePixPayments": true,
  "EnableBankSlipPayments": false,
  "TimeoutSeconds": 30
}
```

#### Gateway Recomendado: Mercado Pago

**Vantagens:**
- Ampla aceitação no Brasil
- Suporte a múltiplos métodos de pagamento (cartão, PIX, boleto)
- API bem documentada
- SDKs oficiais para .NET
- Sistema de webhooks robusto
- Ambiente de sandbox para testes

### 4.2 Feature Flag para Pagamentos por Cartão

✅ **IMPLEMENTADO** - Feature flag `CreditCardPayments` adicionada à `BusinessConfiguration`.

Esta flag permite habilitar/desabilitar pagamentos por cartão de crédito por clínica:

**Habilitar via API:**
```http
PUT /api/businessconfiguration/{clinicId}/feature
Content-Type: application/json

{
  "featureName": "CreditCardPayments",
  "enabled": true
}
```

**Desabilitar via API:**
```http
PUT /api/businessconfiguration/{clinicId}/feature
Content-Type: application/json

{
  "featureName": "CreditCardPayments",
  "enabled": false
}
```

**Verificar via código:**
```csharp
var config = await _businessConfigurationRepository.GetByClinicIdAsync(clinicId);
bool canProcessCard = config.IsFeatureEnabled("CreditCardPayments");
```

### 4.3 Fluxo de Assinatura

```
1. Cliente seleciona plano → 
2. Sistema verifica disponibilidade de campanha → 
3. Sistema verifica se pagamentos por cartão estão habilitados (feature flag) →
4. Apresenta preço (campaign ou regular) → 
5. Cliente confirma → 
6. Sistema cria pagamento no gateway (Mercado Pago) → 
7. Gateway retorna status → 
8. Sistema ativa clínica com plano escolhido
```

### 4.4 Exemplo de Uso do Gateway

```csharp
// POST /api/subscriptions/create
[HttpPost("create")]
public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
{
    // Verificar se pagamentos por cartão estão habilitados
    var config = await _businessConfigurationRepository.GetByClinicIdAsync(request.ClinicId);
    if (!config.IsFeatureEnabled("CreditCardPayments"))
    {
        return BadRequest("Pagamentos por cartão não estão habilitados para esta clínica");
    }
    
    var plan = await _subscriptionPlanRepository.GetByIdAsync(request.PlanId);
    
    if (plan == null)
        return NotFound("Plano não encontrado");
    
    var effectivePrice = plan.GetEffectivePrice();
    var canJoinCampaign = plan.CanJoinCampaign();
    
    // Criar pagamento no gateway de pagamento (Mercado Pago)
    var paymentResult = await _paymentGatewayService.CreateSubscriptionPaymentAsync(
        request.CustomerId,
        request.CustomerEmail,
        effectivePrice,
        plan.Name,
        request.TenantId
    );
    
    if (paymentResult.Success)
    {
        // Incrementar contador de early adopters se aplicável
        if (canJoinCampaign)
            plan.IncrementEarlyAdopters();
        
        // Ativar clínica com o plano
        await _clinicService.ActivateClinicWithPlan(request.ClinicId, plan.Id);
    }
    
    return Ok(paymentResult);
}
```

### 4.5 Próximos Passos

Para completar a integração com Mercado Pago:

1. ✅ Configuração básica implementada
2. ✅ Interface do serviço criada
3. ✅ Feature flag de pagamentos implementada
4. ⏳ Obter credenciais do Mercado Pago (AccessToken, PublicKey, WebhookSecret)
5. ⏳ Adicionar SDK do Mercado Pago: `dotnet add package MercadoPagoCore`
6. ⏳ Implementar criação de preferências de pagamento
7. ⏳ Implementar processamento de webhooks
8. ⏳ Implementar consulta de status de pagamento
9. ⏳ Implementar reembolsos
10. ⏳ Configurar URL de notificação (webhook)

**Documentação Completa:** Consulte `PAYMENT_GATEWAY_README.md` para detalhes de configuração e uso.

---

## 5. Sistema de Notificações e Alertas

### 5.1 Alertas de Negócio

Configure notificações automáticas para:

| Evento | Quando | Ação |
|--------|--------|------|
| Novo cliente pagante | Imediatamente | Email para equipe + Slack |
| Cancelamento | Imediatamente | Email para CS + investigar motivo |
| Trial expirando | 3 dias antes | Email para cliente |
| Campanha atingindo limite | 90% das vagas | Email para marketing |
| MRR abaixo da meta | Mensal | Email para gestão |
| Churn > 5% | Mensal | Alerta crítico |

### 5.2 Implementação

```csharp
// Service para notificações financeiras
public class FinancialNotificationService
{
    public async Task NotifyNewSubscription(Clinic clinic, SubscriptionPlan plan)
    {
        await _emailService.SendAsync(
            "team@primecare.com",
            "Novo Cliente Pagante!",
            $"Cliente: {clinic.Name} - Plano: {plan.Name} - Valor: R$ {plan.GetEffectivePrice()}"
        );
        
        await _slackService.SendMessageAsync(
            "#sales",
            $"🎉 Novo cliente: {clinic.Name} no plano {plan.Name}!"
        );
    }
    
    public async Task NotifyCampaignNearingLimit(SubscriptionPlan plan)
    {
        var remaining = plan.MaxEarlyAdopters - plan.CurrentEarlyAdopters;
        var percentage = (plan.CurrentEarlyAdopters / plan.MaxEarlyAdopters) * 100;
        
        if (percentage >= 90)
        {
            await _emailService.SendAsync(
                "marketing@primecare.com",
                $"Campanha {plan.CampaignName} perto do limite",
                $"Restam apenas {remaining} vagas!"
            );
        }
    }
}
```

---

## 6. Relatórios Gerenciais

### 6.1 Relatório Mensal de Receita

Template para email mensal automático:

```
Assunto: Relatório Financeiro Mensal - [Mês/Ano]

MRR Atual: R$ [valor]
Crescimento vs Mês Anterior: [%]
Total de Clientes: [número]
Novos Clientes no Mês: [número]
Cancelamentos no Mês: [número]
Churn Rate: [%]

Distribuição por Plano:
- Starter: [número] clientes (R$ [receita])
- Professional: [número] clientes (R$ [receita])
- Enterprise: [número] clientes (R$ [receita])

Distribuição por Especialidade:
- Medicina: [número] clientes
- Psicologia: [número] clientes
- Odontologia: [número] clientes
- Outras: [número] clientes

Status das Metas:
✅ Meta de Clientes: [atual] / [meta]
✅ Meta de MRR: R$ [atual] / R$ [meta]
⚠️ Churn Rate: [atual%] (meta < 5%)

Campanha Early Adopter:
- Vagas Ocupadas: [número] / [total]
- Receita da Campanha: R$ [valor]
```

### 6.2 Dashboard Executivo

Crie uma view resumida em `/admin/executive-dashboard` com:

- **Cartões de Métricas**: MRR, Total de Clientes, Churn, CAC, LTV
- **Gráfico de Crescimento**: MRR dos últimos 12 meses
- **Funil de Conversão**: Trials → Pagantes
- **Top 5 Clientes**: Por receita
- **Alertas**: Problemas que precisam atenção

---

## 7. Testes e Validação

### 7.1 Testes Unitários

Adicione testes para:

```csharp
[Fact]
public void Plan_ShouldCalculateCorrectEffectivePrice_WhenCampaignActive()
{
    var plan = CreateTestPlan();
    plan.SetCampaignPricing("Test", "Test", 100m, 50m);
    
    var effectivePrice = plan.GetEffectivePrice();
    
    Assert.Equal(50m, effectivePrice);
}

[Fact]
public void Plan_ShouldNotAllowJoiningCampaign_WhenSlotsAreFull()
{
    var plan = CreateTestPlan();
    plan.SetCampaignPricing("Test", "Test", 100m, 50m, maxEarlyAdopters: 1);
    plan.IncrementEarlyAdopters();
    
    Assert.False(plan.CanJoinCampaign());
}
```

### 7.2 Testes de Integração

Teste o fluxo completo:

```csharp
[Fact]
public async Task Should_CreateSubscription_AndIncrementEarlyAdopters()
{
    // Arrange
    var plan = await CreateTestPlanInDatabase();
    var clinic = await CreateTestClinic();
    
    // Act
    var result = await _subscriptionService.CreateSubscription(clinic.Id, plan.Id);
    
    // Assert
    Assert.True(result.Success);
    var updatedPlan = await _planRepository.GetByIdAsync(plan.Id);
    Assert.Equal(1, updatedPlan.CurrentEarlyAdopters);
}
```

---

## 8. Monitoramento e Análise

### 8.1 Logs Importantes

Configure logging para:

```csharp
_logger.LogInformation(
    "Subscription created: ClinicId={ClinicId}, PlanId={PlanId}, Price={Price}, IsCampaign={IsCampaign}",
    clinicId, planId, price, isCampaign
);

_logger.LogWarning(
    "Campaign near limit: Plan={PlanName}, Current={Current}, Max={Max}",
    plan.Name, plan.CurrentEarlyAdopters, plan.MaxEarlyAdopters
);

_logger.LogError(
    "Subscription creation failed: ClinicId={ClinicId}, Error={Error}",
    clinicId, error
);
```

### 8.2 Métricas de Performance

Configure Application Insights ou similar para rastrear:

- Taxa de conversão Trial → Pagante
- Tempo médio de onboarding
- Taxa de sucesso de pagamentos
- Erros no checkout

---

## 9. Checklist de Implementação

### Backend
- [x] SubscriptionPlan entity implementada
- [x] BusinessConfiguration entity implementada
- [x] Repositórios e serviços criados
- [x] PaymentGatewaySettings configuração criada
- [x] IPaymentGatewayService interface implementada
- [x] MercadoPagoPaymentGatewayService implementado
- [x] CreditCardPayments feature flag adicionada
- [x] Migração para feature flag criada
- [ ] Endpoints de métricas financeiras
- [ ] Credenciais Mercado Pago configuradas
- [ ] SDK Mercado Pago instalado
- [ ] Integração completa com Mercado Pago
- [ ] Sistema de notificações
- [ ] Seed dos planos conforme plano financeiro
- [ ] Testes unitários e integração

### Frontend
- [x] Componente de gestão de planos (admin)
- [x] Business configuration UI
- [x] Template editor
- [x] Onboarding wizard
- [ ] Dashboard financeiro executivo
- [ ] Página de seleção de planos (público)
- [ ] Checkout de assinatura
- [ ] Testes e2e

### Infraestrutura
- [ ] Configurar gateway de pagamento (Stripe/PagSeguro)
- [ ] Configurar webhooks de pagamento
- [ ] Configurar notificações (email/Slack)
- [ ] Configurar monitoramento de métricas
- [ ] Configurar backup de dados financeiros
- [ ] Documentação de API para integrações

### Operacional
- [ ] Treinamento da equipe de suporte
- [ ] Documentação de processos de vendas
- [ ] FAQ de planos e preços
- [ ] Materiais de marketing
- [ ] Contrato de serviço (ToS)
- [ ] Política de cancelamento e reembolso

---

## 10. Cronograma de Implementação

### Semana 1
- Atualizar seed dos planos no banco
- Criar endpoints de métricas financeiras
- Implementar dashboard financeiro básico

### Semana 2
- Integrar gateway de pagamento
- Implementar fluxo de checkout
- Testes de pagamento em sandbox

### Semana 3
- Sistema de notificações
- Relatórios gerenciais
- Testes end-to-end

### Semana 4
- Documentação final
- Treinamento de equipe
- Deploy em produção
- Lançamento da campanha Early Adopter

---

## 11. Suporte e Manutenção

### Revisão Mensal
- Analisar métricas vs metas do plano financeiro
- Ajustar estratégia de marketing se necessário
- Revisar preços (após 6 meses de operação)
- Coletar feedback dos clientes

### Manutenção Contínua
- Monitorar health do gateway de pagamento
- Verificar integridade dos dados financeiros
- Atualizar projeções baseado em dados reais
- Otimizar conversão e reduzir churn

---

## 12. Contato e Suporte

Para questões sobre implementação:
- **Técnicas**: Time de desenvolvimento
- **Financeiras**: Gestão/CFO
- **Pagamentos**: Suporte do gateway escolhido

---

**Última Atualização**: 02/02/2026
**Versão**: 1.0
**Status**: Pronto para implementação
