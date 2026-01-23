# Prompt 23: Programa de Referral

## 📋 Metadados

- **Prioridade**: P3 - Low
- **Complexidade**: Low
- **Tempo Estimado**: 1-2 meses
- **Equipe**: 1 desenvolvedor
- **Custo Estimado**: R$ 37,500

## 🎯 Objetivo

Implementar um programa de indicação (referral) para incentivar clínicas a trazerem novos clientes através de recompensas automáticas e gamificação.

## 📖 Contexto

### Problema
Crescimento orgânico é lento e CAC (Customer Acquisition Cost) é alto. Clientes satisfeitos podem trazer novos clientes mas não há incentivo estruturado.

### Solução Proposta
1. Referral code generation (unique por clínica)
2. Referral tracking (quem indicou quem)
3. Incentive configuration (discount, cashback, free months)
4. Referral analytics dashboard
5. Automatic reward distribution
6. Referral leaderboard
7. Email/SMS campaigns para referrals
8. Terms and conditions management
9. Multi-tier referral (indicar indicadores)
10. Anti-fraud detection

## 🏗️ Arquitetura

### Referral System

```csharp
public class ReferralService
{
    public async Task<Referral> CreateReferralAsync(Guid referrerId, string refereeEmail)
    {
        var code = GenerateUniqueCode();
        var referral = new Referral
        {
            ReferrerId = referrerId,
            RefereeEmail = refereeEmail,
            Code = code,
            Status = ReferralStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(referral);
        await SendReferralEmailAsync(refereeEmail, code);
        
        return referral;
    }
    
    public async Task ProcessReferralConversionAsync(string code)
    {
        var referral = await GetByCodeAsync(code);
        referral.Status = ReferralStatus.Converted;
        referral.ConvertedAt = DateTime.UtcNow;
        
        // Apply rewards
        await ApplyReferrerRewardAsync(referral.ReferrerId);
        await ApplyRefereeRewardAsync(referral.RefereeId);
        
        await _repository.UpdateAsync(referral);
    }
}
```

## 📅 Implementação

### Sprint 1: Core System (Semana 1-2)
- Referral code generation
- Tracking system
- Reward configuration
- Database models

### Sprint 2: Rewards & Notifications (Semana 3-4)
- Automatic reward distribution
- Email/SMS campaigns
- Dashboard
- Leaderboard

### Sprint 3: Advanced & Launch (Semana 5-6)
- Multi-tier referrals
- Anti-fraud
- Analytics
- Testing & launch

## 📊 Métricas de Sucesso

### KPIs Técnicos
1. Code Generation Time: < 100ms
2. Tracking Accuracy: 100%
3. Reward Distribution: < 24h

### KPIs de Negócio
1. Referrals Created: 500+ em 6 meses
2. Conversion Rate: > 20%
3. CAC Reduction: 40%
4. New MRR from Referrals: R$ 50.000/mês em 12 meses

## 💰 ROI

### Investimento
- Desenvolvimento: R$ 37.500
- Rewards Budget: R$ 60.000/ano
- Marketing: R$ 12.000/ano
- Total Ano 1: R$ 109.500

### Retorno
- New Customer Revenue: R$ 360.000/ano (60 clientes × R$ 500/mês × 12)
- Reduced Marketing Cost: R$ 120.000/ano
- LTV Increase: R$ 80.000/ano
- **ROI Ano 1: 411%**
- **Break-even: 3 meses**

| Ano | Investimento | Receita | ROI  |
|-----|-------------|---------|------|
| 1   | R$ 109.5k   | R$ 560k | 411% |
| 2   | R$ 72k      | R$ 840k | 1067%|
| 3   | R$ 72k      | R$ 1.2M | 1567%|
