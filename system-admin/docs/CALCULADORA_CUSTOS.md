# 💰 Calculadora de Custos de Infraestrutura

## 📊 Estimativa de Custos por Número de Clínicas

Esta calculadora ajuda você a estimar os custos de infraestrutura do Omni Care Software conforme seu negócio cresce.

---

## 🎯 Opção 1: Railway + Vercel (Recomendado)

### Faixa 1: Início (1-20 clínicas pequenas)

**Configuração:**
- Backend: 512MB RAM, 0.5 vCPU
- PostgreSQL: 1GB storage
- Frontend: Vercel Free

**Uso Estimado:**
- 100-500 requisições/dia por clínica
- 50MB dados/clínica no banco
- ~20 usuários simultâneos no pico

**Custos Mensais:**
```
Railway Backend:
- RAM: 512MB × 730h × $0.000463/GB-h = $0.17
- CPU: 0.5 vCPU × 730h × $0.000231/vCPU-h = $0.08
- Egress: ~10GB × $0.10/GB = $1.00
- Subtotal Railway: $1.25/mês

Vercel Frontend: $0 (Free tier)

TOTAL: ~$1-5/mês (coberto pelos $5 grátis Railway!)
```

💡 **Custo Real: $0-5/mês** (usando créditos grátis)

---

### Faixa 2: Crescimento (20-50 clínicas pequenas/médias)

**Configuração:**
- Backend: 1GB RAM, 1 vCPU
- PostgreSQL: 3GB storage
- Frontend: Vercel Free

**Uso Estimado:**
- 500-1,000 requisições/dia por clínica
- 150MB dados/clínica
- ~50 usuários simultâneos no pico

**Custos Mensais:**
```
Railway Backend:
- RAM: 1GB × 730h × $0.000463/GB-h = $0.34
- CPU: 1 vCPU × 730h × $0.000231/vCPU-h = $0.17
- Egress: ~25GB × $0.10/GB = $2.50
- PostgreSQL storage: 3GB × $0.20/GB = $0.60
- Subtotal Railway: $3.61/mês

Vercel Frontend: $0 (Free tier)

TOTAL: ~$5-10/mês
```

💡 **Custo Real: $5-10/mês**

---

### Faixa 3: Estabelecido (50-100 clínicas médias)

**Configuração:**
- Backend: 2GB RAM, 2 vCPU
- PostgreSQL: 10GB storage
- Frontend: Vercel Free

**Uso Estimado:**
- 1,000-2,000 requisições/dia por clínica
- 300MB dados/clínica
- ~100 usuários simultâneos no pico

**Custos Mensais:**
```
Railway Backend:
- RAM: 2GB × 730h × $0.000463/GB-h = $0.68
- CPU: 2 vCPU × 730h × $0.000231/vCPU-h = $0.34
- Egress: ~60GB × $0.10/GB = $6.00
- PostgreSQL storage: 10GB × $0.20/GB = $2.00
- Subtotal Railway: $9.02/mês

Vercel Frontend: $0 (Free tier)

TOTAL: ~$10-20/mês
```

💡 **Custo Real: $10-20/mês**

---

### Faixa 4: Consolidado (100-200 clínicas médias/grandes)

**Configuração:**
- Backend: 4GB RAM, 4 vCPU
- PostgreSQL: 25GB storage
- Frontend: Vercel Pro ($20/mês)

**Uso Estimado:**
- 2,000-5,000 requisições/dia por clínica
- 500MB dados/clínica
- ~250 usuários simultâneos no pico

**Custos Mensais:**
```
Railway Backend:
- RAM: 4GB × 730h × $0.000463/GB-h = $1.35
- CPU: 4 vCPU × 730h × $0.000231/vCPU-h = $0.67
- Egress: ~150GB × $0.10/GB = $15.00
- PostgreSQL storage: 25GB × $0.20/GB = $5.00
- Subtotal Railway: $22.02/mês

Vercel Pro: $20/mês

TOTAL: ~$40-60/mês
```

💡 **Custo Real: $40-60/mês**

**Nota**: Nesta faixa, considere upgrade para Railway Pro ($20/mês base) para mais recursos e suporte.

---

### Faixa 5: Escala (200-500 clínicas grandes)

**Configuração:**
- Backend: 8GB RAM, 8 vCPU (ou múltiplas instâncias)
- PostgreSQL: 50GB storage + Read Replicas
- Frontend: Vercel Pro + CDN
- Load Balancer

**Uso Estimado:**
- 5,000-10,000 requisições/dia por clínica
- 1GB dados/clínica
- ~500 usuários simultâneos no pico

**Custos Mensais:**
```
Railway Backend:
- RAM: 8GB × 730h × $0.000463/GB-h = $2.71
- CPU: 8 vCPU × 730h × $0.000231/vCPU-h = $1.35
- Egress: ~400GB × $0.10/GB = $40.00
- PostgreSQL Primary: 50GB × $0.20/GB = $10.00
- PostgreSQL Replica: 50GB × $0.20/GB = $10.00
- Subtotal Railway: $64.06/mês

Vercel Pro: $20/mês
Railway Pro Subscription: $20/mês

TOTAL: ~$100-150/mês
```

💡 **Custo Real: $100-150/mês**

**Nesta escala, considere migrar para Cloud tradicional (AWS/Azure/GCP) para melhor pricing.**

---

## 🔧 Opção 2: VPS Tradicional (Hetzner/DigitalOcean)

### Faixa 1: Início (1-30 clínicas)

**Servidor:** Hetzner CX21 ou DigitalOcean Basic

**Specs:**
- 2 vCPU
- 4GB RAM
- 80GB SSD
- 20TB tráfego

**Custo:** €4.51/mês (~$5 USD) ou $6/mês

💡 **Suporta até 30 clínicas pequenas confortavelmente**

---

### Faixa 2: Crescimento (30-80 clínicas)

**Servidor:** Hetzner CX31 ou DigitalOcean CPU-Optimized

**Specs:**
- 2 vCPU (CPU-optimized)
- 8GB RAM
- 160GB SSD
- 20TB tráfego

**Custo:** €10.18/mês (~$11 USD) ou $18/mês

💡 **Suporta até 80 clínicas pequenas/médias**

---

### Faixa 3: Estabelecido (80-150 clínicas)

**Servidor:** Hetzner CX41 ou DigitalOcean General Purpose

**Specs:**
- 4 vCPU
- 16GB RAM
- 240GB SSD
- 20TB tráfego

**Custo:** €20.34/mês (~$22 USD) ou $48/mês

💡 **Suporta até 150 clínicas médias**

---

### Faixa 4: Consolidado (150-300 clínicas)

**Servidor:** Hetzner CX51 ou 2× DigitalOcean + Load Balancer

**Specs:**
- 8 vCPU
- 32GB RAM
- 360GB SSD
- 20TB tráfego

**Custo:** €40.67/mês (~$44 USD) ou $96/mês (2 servers) + $12 (LB)

💡 **Suporta até 300 clínicas médias/grandes**

**Extras:**
- Backup: +20% ($8-20/mês)
- Monitoring: $5-10/mês (opcional)

**Total VPS:** $50-130/mês

---

### Faixa 5: Escala (300+ clínicas)

**Recomendação:** Migre para Cloud com Auto-Scaling

- AWS/Azure/GCP
- Kubernetes
- Auto-scaling
- Multi-region
- CDN global

**Custo estimado:** $200-1,000+/mês (depende muito do uso)

---

## 🆓 Opção 3: Free Tier (Demonstração Apenas)

### Limitações

**Serviços:**
- Render Free: Backend (sleep após 15min)
- Neon Free: PostgreSQL 0.5GB (sleep após 5min)
- Vercel Free: Frontend (sem limitações)

**Custo:** $0/mês

**⚠️ Limitações Críticas:**
- Backend "acorda" em 30-60 segundos
- Banco "acorda" em 10-20 segundos
- **Não use para clientes pagantes!**
- Ideal apenas para demos e testes

---

## 📊 Tabela Comparativa Resumida

| Clínicas | Railway | VPS (Hetzner) | AWS/Azure | Free Tier |
|----------|---------|---------------|-----------|-----------|
| **1-20** | $0-5 | $5 | $50-100 | $0 ⚠️ |
| **20-50** | $5-10 | $5-11 | $80-150 | - |
| **50-100** | $10-20 | $11-22 | $150-300 | - |
| **100-200** | $40-60 | $22-44 | $300-600 | - |
| **200-500** | $100-150 | $44-130 | $600-1,500 | - |
| **500+** | $200+ | $130-300 | $1,000-5,000+ | - |

---

## 💡 Recomendações por Estágio

### Estágio 1: MVP/Validação (0-5 clínicas)
**Recomendação:** Free Tier ou Railway ($0-5/mês)
- Custo zero ou mínimo
- Setup rápido
- Perfeito para testar

### Estágio 2: Early Adopters (5-30 clínicas)
**Recomendação:** Railway ($5-10/mês) ou VPS ($5/mês)
- Custo previsível
- Escalável
- Manutenção mínima

### Estágio 3: Product-Market Fit (30-100 clínicas)
**Recomendação:** Railway ($10-20/mês) ou VPS ($11-22/mês)
- Infraestrutura sólida
- Backups automáticos
- Monitoramento

### Estágio 4: Crescimento (100-300 clínicas)
**Recomendação:** Railway Pro ($40-100/mês) ou VPS robusto ($40-60/mês)
- Alta disponibilidade
- Read replicas
- Load balancing

### Estágio 5: Escala (300+ clínicas)
**Recomendação:** Cloud Profissional (AWS/Azure/GCP)
- Auto-scaling
- Multi-region
- SLA garantido
- Equipe DevOps

---

## 🎯 Calculadora Rápida

**Quanto vou gastar com X clínicas?**

```
Se você tem N clínicas pequenas:

Railway:
- 1-20 clínicas: $0-5/mês
- 21-50 clínicas: $5-10/mês
- 51-100 clínicas: $10-20/mês
- 101-200 clínicas: $40-60/mês

VPS (Hetzner):
- 1-30 clínicas: $5/mês
- 31-80 clínicas: $11/mês
- 81-150 clínicas: $22/mês
- 151-300 clínicas: $44-130/mês
```

**Fatores que aumentam custo:**
- ✅ Mais requisições por clínica
- ✅ Mais dados armazenados
- ✅ Mais usuários simultâneos
- ✅ Uploads de arquivos grandes
- ✅ Integrations (APIs externas)

**Fatores que reduzem custo:**
- ✅ Cache agressivo
- ✅ CDN para assets
- ✅ Otimização de queries
- ✅ Compressão de dados
- ✅ Lazy loading

---

## 📈 Projeção de ROI

### Exemplo: 50 clínicas pagando R$200/mês cada

**Receita Mensal:** 50 × R$200 = R$10,000 (~$2,000 USD)*

*Nota: Taxa de câmbio BRL:USD varia. Use a taxa atual para cálculos precisos.*

**Custos Infraestrutura:** 
- Railway: $10-20/mês
- VPS: $11-22/mês

**Margem de Infraestrutura:** 99% 🎉

**Outros custos a considerar:**
- Equipe (desenvolvimento/suporte)
- Marketing
- Impostos
- Gateway de pagamento (2-5%)

---

## 🎁 Dica de Ouro

**Comece com Railway** pelos primeiros 50-100 clientes. É:
- ✅ Mais rápido de configurar
- ✅ Manutenção zero
- ✅ Escalável automaticamente
- ✅ Backups incluídos

**Migre para VPS** quando:
- Você tem conhecimento técnico
- Quer mais controle
- Tem 100+ clínicas
- Tem equipe DevOps

**Vá para Cloud** (AWS/Azure) quando:
- Você tem 300+ clínicas
- Precisa de multi-region
- Tem orçamento para DevOps
- Precisa de SLA 99.99%

---

**Dúvidas?** Consulte:
- [INFRA_PRODUCAO_BAIXO_CUSTO.md](INFRA_PRODUCAO_BAIXO_CUSTO.md)
- [QUICK_START_PRODUCTION.md](QUICK_START_PRODUCTION.md)

**Boa sorte com seu negócio! 💰🚀**
