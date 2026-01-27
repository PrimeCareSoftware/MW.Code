# Guia Prático: Captação de Clientes Desistentes

## 🎯 Objetivo

Este guia fornece um plano de ação prático e sequencial para implementar estratégias de captação de clientes desistentes e otimização do funil de cadastro no MedicWarehouse.

## 📅 Plano de Implementação em 12 Semanas

### Semana 1-2: Análise e Preparação

#### Dia 1-3: Auditoria de Dados
- [ ] Executar query de análise de abandono por etapa
- [ ] Identificar a etapa com maior taxa de abandono
- [ ] Calcular taxa de conversão atual do funil
- [ ] Documentar padrões de comportamento observados

**Query para executar:**
```sql
SELECT 
    Step,
    StepName,
    COUNT(DISTINCT SessionId) as Total,
    COUNT(DISTINCT CASE WHEN Action = 'abandoned' THEN SessionId END) as Abandonos,
    COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) as Conversoes,
    ROUND(100.0 * COUNT(DISTINCT CASE WHEN Action = 'abandoned' THEN SessionId END) / COUNT(DISTINCT SessionId), 2) as TaxaAbandono
FROM SalesFunnelMetrics
WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)
GROUP BY Step, StepName
ORDER BY Step;
```

#### Dia 4-7: Configuração de Infraestrutura
- [ ] Configurar serviço de email (SendGrid ou similar)
- [ ] Criar templates HTML de emails de recuperação
- [ ] Testar envio de emails
- [ ] Configurar ambiente de desenvolvimento

**Checklist de Configuração:**
```bash
# 1. Criar conta no SendGrid (ou outro provedor)
# 2. Gerar API Key
# 3. Adicionar ao appsettings.json:
{
  "EmailService": {
    "Provider": "SendGrid",
    "ApiKey": "SG.xxxx",
    "FromEmail": "noreply@medicwarehouse.com",
    "FromName": "MedicWarehouse"
  }
}

# 4. Testar envio via API
curl -X POST "https://api.sendgrid.com/v3/mail/send" \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{...}'
```

#### Dia 8-10: Definição de Métricas e KPIs
- [ ] Estabelecer baseline de taxa de conversão atual
- [ ] Definir metas para os próximos 3 meses
- [ ] Criar planilha de acompanhamento semanal
- [ ] Configurar Google Analytics 4

**Metas Sugeridas:**
- Aumentar conversão geral em 20% (ex: de 15% para 18%)
- Reduzir abandono na etapa crítica em 30%
- Recuperar 15% dos leads abandonados
- Alcançar ROI de 500% em campanhas de email

### Semana 3-4: Implementação de Recuperação Básica

#### Tarefa 1: Criar Serviço de Lead Recovery
```csharp
// 1. Criar arquivo: MedicSoft.Application/Services/LeadRecoveryService.cs
// 2. Copiar código do guia técnico
// 3. Registrar no Program.cs:
builder.Services.AddScoped<ILeadRecoveryService, LeadRecoveryService>();
```

#### Tarefa 2: Criar Background Job
```csharp
// 1. Criar arquivo: MedicSoft.Api/BackgroundJobs/LeadRecoveryJob.cs
// 2. Registrar no Program.cs:
builder.Services.AddHostedService<LeadRecoveryJob>();
```

#### Tarefa 3: Criar Templates de Email
```html
<!-- Template 1: Etapas Iniciais (1-2) -->
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #4CAF50; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; background: #f9f9f9; }
        .button { 
            display: inline-block; 
            padding: 15px 30px; 
            background: #4CAF50; 
            color: white; 
            text-decoration: none; 
            border-radius: 5px; 
            margin: 20px 0;
        }
        .footer { text-align: center; padding: 20px; font-size: 12px; color: #777; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🏥 MedicWarehouse</h1>
        </div>
        <div class="content">
            <h2>Olá! Faltou pouco para começar 🚀</h2>
            <p>Notamos que você iniciou seu cadastro no MedicWarehouse, mas não finalizou.</p>
            <p>Leva apenas <strong>3 minutos</strong> para completar e você pode começar a usar nossa plataforma imediatamente!</p>
            
            <h3>✨ Benefícios de se cadastrar:</h3>
            <ul>
                <li>✅ Agenda de consultas inteligente</li>
                <li>✅ Prontuário médico digital</li>
                <li>✅ Relatórios gerenciais</li>
                <li>✅ Suporte dedicado</li>
            </ul>
            
            <center>
                <a href="{resumeUrl}" class="button">Continuar Cadastro</a>
            </center>
            
            <p>Se tiver alguma dúvida, responda este email. Estamos aqui para ajudar! 💙</p>
        </div>
        <div class="footer">
            <p>MedicWarehouse - Software de Gestão para Clínicas Médicas</p>
            <p>Não quer receber estes emails? <a href="{unsubscribeUrl}">Descadastrar</a></p>
        </div>
    </div>
</body>
</html>
```

#### Tarefa 4: Testar Sistema
- [ ] Criar sessão de teste no banco
- [ ] Aguardar 2 horas
- [ ] Verificar se email foi enviado
- [ ] Validar links de retomada funcionam
- [ ] Testar com diferentes etapas

### Semana 5-6: Dashboard de Métricas

#### Tarefa 1: Criar Endpoint de Analytics
```csharp
// 1. Criar RegistrationAnalyticsController.cs
// 2. Implementar métodos de análise
// 3. Testar endpoints via Swagger
```

#### Tarefa 2: Criar Frontend do Dashboard
```typescript
// Angular Component: registration-analytics.component.ts
import { Component, OnInit } from '@angular/core';
import { AnalyticsService } from './analytics.service';

@Component({
  selector: 'app-registration-analytics',
  template: `
    <div class="dashboard">
      <h1>Dashboard de Cadastros</h1>
      
      <!-- KPIs Principais -->
      <div class="kpi-grid">
        <div class="kpi-card">
          <h3>Taxa de Conversão</h3>
          <div class="kpi-value">{{ stats.conversionRate | number:'1.1-1' }}%</div>
          <div class="kpi-change" [class.positive]="stats.changePositive">
            {{ stats.change > 0 ? '+' : '' }}{{ stats.change | number:'1.1-1' }}% vs semana passada
          </div>
        </div>
        
        <div class="kpi-card">
          <h3>Leads Ativos</h3>
          <div class="kpi-value">{{ stats.activeLeads }}</div>
        </div>
        
        <div class="kpi-card">
          <h3>Taxa de Recuperação</h3>
          <div class="kpi-value">{{ stats.recoveryRate | number:'1.1-1' }}%</div>
        </div>
        
        <div class="kpi-card">
          <h3>Conversões Hoje</h3>
          <div class="kpi-value">{{ stats.todayConversions }}</div>
        </div>
      </div>
      
      <!-- Gráfico de Funil -->
      <div class="chart-section">
        <h2>Funil de Conversão</h2>
        <app-funnel-chart [data]="funnelData"></app-funnel-chart>
      </div>
      
      <!-- Tabela de Leads de Alto Valor -->
      <div class="leads-section">
        <h2>Leads de Alto Valor 🔥</h2>
        <table class="leads-table">
          <thead>
            <tr>
              <th>Score</th>
              <th>Última Etapa</th>
              <th>Plano</th>
              <th>Tempo</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let lead of highValueLeads">
              <td><span class="badge score">{{ lead.score }}</span></td>
              <td>Etapa {{ lead.lastStep }}</td>
              <td>{{ lead.plan }}</td>
              <td>{{ lead.hoursAgo }}h atrás</td>
              <td>
                <button (click)="contactLead(lead)">Contatar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { padding: 20px; }
    .kpi-grid { 
      display: grid; 
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }
    .kpi-card {
      background: white;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .kpi-value {
      font-size: 36px;
      font-weight: bold;
      color: #4CAF50;
      margin: 10px 0;
    }
    .kpi-change {
      font-size: 14px;
      color: #f44336;
    }
    .kpi-change.positive {
      color: #4CAF50;
    }
    .chart-section, .leads-section {
      background: white;
      padding: 20px;
      border-radius: 8px;
      margin-bottom: 20px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .leads-table {
      width: 100%;
      border-collapse: collapse;
    }
    .leads-table th {
      background: #f5f5f5;
      padding: 12px;
      text-align: left;
      border-bottom: 2px solid #ddd;
    }
    .leads-table td {
      padding: 12px;
      border-bottom: 1px solid #eee;
    }
    .badge.score {
      background: #4CAF50;
      color: white;
      padding: 4px 12px;
      border-radius: 12px;
      font-weight: bold;
    }
  `]
})
export class RegistrationAnalyticsComponent implements OnInit {
  stats: any = {};
  funnelData: any[] = [];
  highValueLeads: any[] = [];

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit() {
    this.loadData();
    // Atualizar a cada 5 minutos
    setInterval(() => this.loadData(), 300000);
  }

  loadData() {
    this.analyticsService.getOverview().subscribe(data => {
      this.stats = data;
    });
    
    this.analyticsService.getFunnelData().subscribe(data => {
      this.funnelData = data;
    });
    
    this.analyticsService.getHighValueLeads().subscribe(data => {
      this.highValueLeads = data;
    });
  }

  contactLead(lead: any) {
    // Implementar ação de contato
    console.log('Contacting lead:', lead);
  }
}
```

### Semana 7-8: Otimização e Testes A/B

#### Experimento 1: Reduzir Campos Obrigatórios
**Hipótese:** Menos campos obrigatórios = maior taxa de conversão

**Variante A (Controle):** Fluxo atual com todos os campos
**Variante B (Teste):** Remover campos opcionais das etapas 1-3

**Implementação:**
```typescript
// Frontend: registration.component.ts
export class RegistrationComponent {
  variant: string;
  
  ngOnInit() {
    // Pegar variante do backend
    this.variant = this.abTestService.getVariant('reduce_fields_test');
    
    if (this.variant === 'B') {
      // Tornar campos opcionais
      this.makeFieldsOptional(['complement', 'ownerCPF']);
    }
  }
  
  trackVariantExposure() {
    this.abTestService.trackExposure('reduce_fields_test', this.variant, this.sessionId);
  }
}
```

**Métricas de Sucesso:**
- Conversão aumentou >5%?
- Tempo médio de conclusão diminuiu?
- Qualidade dos leads mantida?

#### Experimento 2: Adicionar Prova Social
**Hipótese:** Depoimentos de clientes aumentam confiança

**Variante A:** Sem depoimentos
**Variante B:** Com 3 depoimentos na página

**Métricas:**
- Taxa de conversão na etapa 5 (Plano)
- Tempo gasto na página de planos

### Semana 9-10: Campanhas de Recuperação Avançadas

#### Campanha 1: Email Drip Sequence
**Sequência de 3 emails:**

**Email 1: 2 horas após abandono**
- Assunto: "Faltou pouco! Complete seu cadastro"
- CTA: Link direto para retomar

**Email 2: 24 horas após (se não abriu Email 1)**
- Assunto: "🎁 Oferta especial: 30 dias grátis"
- CTA: Trial estendido

**Email 3: 72 horas após (se não converteu)**
- Assunto: "Última chance: Sua oferta expira em 24h"
- CTA: Urgência + desconto

#### Campanha 2: Segmentação por Perfil

**Leads Quentes (Score > 80):**
- [ ] Contato telefônico dentro de 4 horas
- [ ] Email personalizado do gerente comercial
- [ ] WhatsApp com demo ao vivo

**Leads Mornos (Score 50-80):**
- [ ] Email automático com FAQ
- [ ] Remarketing no Google Ads
- [ ] Chat bot proativo

**Leads Frios (Score < 50):**
- [ ] Email semanal com conteúdo educativo
- [ ] Remarketing passivo
- [ ] Newsletter mensal

### Semana 11-12: Machine Learning e Automação

#### Implementar Predição de Churn
```python
# Script Python para treinar modelo
import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split

# 1. Carregar dados históricos
df = pd.read_sql("""
    SELECT 
        Step,
        CAST(DurationMs AS FLOAT) / 1000 as DurationSeconds,
        CASE WHEN PlanId LIKE '%premium%' THEN 1 ELSE 0 END as ViewedPremium,
        HOUR(CreatedAt) as Hour,
        CASE WHEN UserAgent LIKE '%Mobile%' THEN 1 ELSE 0 END as IsMobile,
        IsConverted as Converted
    FROM SalesFunnelMetrics
    WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 90 DAY)
""", connection)

# 2. Preparar features
X = df[['Step', 'DurationSeconds', 'ViewedPremium', 'Hour', 'IsMobile']]
y = df['Converted']

# 3. Treinar modelo
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)
model = RandomForestClassifier(n_estimators=100)
model.fit(X_train, y_train)

# 4. Avaliar
score = model.score(X_test, y_test)
print(f"Acurácia: {score:.2%}")

# 5. Salvar modelo
import joblib
joblib.dump(model, 'churn_prediction_model.pkl')
```

#### Integrar Predição no Backend
```csharp
// MedicSoft.Application/Services/ChurnPredictionService.cs
using Python.Runtime;

public class ChurnPredictionService
{
    public double PredictChurnProbability(SalesFunnelMetric metric)
    {
        // Usar PythonNet para chamar modelo Python
        using (Py.GIL())
        {
            dynamic model = Py.Import("joblib").load("churn_prediction_model.pkl");
            dynamic np = Py.Import("numpy");
            
            var features = np.array(new[] { 
                metric.Step, 
                (metric.DurationMs ?? 0) / 1000.0,
                metric.PlanId?.Contains("premium") == true ? 1 : 0,
                metric.CreatedAt.Hour,
                metric.UserAgent?.Contains("Mobile") == true ? 1 : 0
            });
            
            var probability = (double)model.predict_proba(features.reshape(1, -1))[0][0];
            return probability;
        }
    }
}
```

## 📊 Checklist de Monitoramento Semanal

### Segunda-feira: Review da Semana Anterior
- [ ] Coletar métricas da semana passada
- [ ] Comparar com semana anterior
- [ ] Identificar anomalias
- [ ] Atualizar planilha de KPIs

### Quarta-feira: Análise de Meio de Semana
- [ ] Verificar taxa de conversão atual
- [ ] Revisar leads de alto valor
- [ ] Ajustar campanhas se necessário

### Sexta-feira: Preparação para Próxima Semana
- [ ] Planejar testes A/B da próxima semana
- [ ] Revisar templates de email
- [ ] Preparar relatório para stakeholders

## 🎯 Ações Rápidas para Impacto Imediato

### Esta Semana (Impacto Rápido)

#### 1. Identificar Gargalo Principal (2 horas)
```sql
-- Executar esta query agora
SELECT Step, StepName, 
       COUNT(*) as Total,
       SUM(CASE WHEN Action = 'abandoned' THEN 1 ELSE 0 END) as Abandonos
FROM SalesFunnelMetrics
WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)
GROUP BY Step, StepName
ORDER BY Abandonos DESC
LIMIT 1;
```
**Ação:** Simplificar a etapa com mais abandonos

#### 2. Criar Email de Recuperação Simples (4 horas)
- [ ] Configurar SendGrid
- [ ] Criar 1 template básico
- [ ] Testar envio manual para leads quentes

#### 3. Adicionar Google Analytics (1 hora)
```html
<!-- Adicionar no head -->
<script async src="https://www.googletagmanager.com/gtag/js?id=G-XXXXXXXXXX"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'G-XXXXXXXXXX');
</script>
```

### Este Mês (Médio Prazo)

#### 1. Implementar Sistema de Lead Scoring
- [ ] Definir critérios de pontuação
- [ ] Criar função de cálculo
- [ ] Gerar lista de leads prioritários
- [ ] Configurar alertas para scores altos

#### 2. Configurar Remarketing
- [ ] Instalar Facebook Pixel
- [ ] Criar audiências personalizadas
- [ ] Configurar campanhas no Ads Manager
- [ ] Orçamento inicial: R$ 500/mês

#### 3. Dashboard Básico no Excel/Google Sheets
Enquanto não tem dashboard no sistema:
```
| Semana | Conversões | Taxa | Abandonos Etapa 1 | Abandonos Etapa 2 | ...
| S1     | 45         | 15%  | 120              | 89               | ...
| S2     | 52         | 17%  | 110              | 82               | ...
```

## 💰 Estimativa de ROI

### Investimento Inicial (Primeiro Mês)
- **Tempo de Desenvolvimento**: 80 horas × R$ 150/h = R$ 12.000
- **Ferramentas**: SendGrid ($19/mês) + GA4 (grátis) = R$ 100
- **Marketing**: Facebook Ads (R$ 500) + Google Ads (R$ 500) = R$ 1.000
- **Total**: R$ 13.100

### Retorno Esperado (Mês 1-3)
Assumindo:
- 500 abandonos/mês
- 15% de recuperação = 75 conversões extras
- Ticket médio R$ 300/mês
- **Receita adicional**: 75 × R$ 300 = R$ 22.500/mês

**ROI Trimestral**: (R$ 67.500 - R$ 13.100) / R$ 13.100 = **415%** 🎉

## 🚨 Alertas e Quando Agir

### Alerta Vermelho 🔴 (Ação Imediata)
- Taxa de conversão caiu >15% em 24h
- **Ação**: Investigar problema técnico, reverter última mudança

### Alerta Amarelo 🟡 (Revisar Esta Semana)
- Taxa de conversão caiu 5-15%
- **Ação**: Analisar por etapa, ajustar campanhas

### Alerta Verde 🟢 (Otimização Contínua)
- Tudo normal, buscar melhorias incrementais
- **Ação**: Continuar testes A/B, documentar aprendizados

## 📚 Recursos Complementares

### Templates de Email Prontos
- 📧 [Biblioteca de Templates](./email-templates/)
- 🎨 [Design System](./design-system/)

### Planilhas e Ferramentas
- 📊 [Planilha de KPIs](./kpi-tracker.xlsx)
- 📈 [Calculadora de ROI](./roi-calculator.xlsx)
- 📋 [Checklist Semanal](./weekly-checklist.pdf)

### Integrações Recomendadas
- **SendGrid**: Email marketing (mais barato)
- **Mailchimp**: Alternativa com UI amigável
- **Customer.io**: Automação avançada
- **Segment**: Centralizar todos os eventos

## ✅ Checklist Final de Validação

### Antes de Lançar
- [ ] Emails de recuperação testados e funcionando
- [ ] Background job rodando sem erros
- [ ] Dashboard acessível para admins
- [ ] Métricas sendo coletadas corretamente
- [ ] Leads sendo pontuados automaticamente
- [ ] Alertas configurados
- [ ] LGPD compliance validado
- [ ] Testes A/B configurados

### Após Lançamento (Primeira Semana)
- [ ] Monitorar emails enviados diariamente
- [ ] Verificar taxa de abertura >20%
- [ ] Verificar taxa de clique >5%
- [ ] Acompanhar conversões de recuperação
- [ ] Ajustar copy se necessário
- [ ] Validar que links estão funcionando

### Otimização Contínua (Mensal)
- [ ] Revisar performance de cada template
- [ ] Testar novos horários de envio
- [ ] Experimentar diferentes CTAs
- [ ] Analisar padrões de sucesso
- [ ] Compartilhar aprendizados com time

---

## 🎓 Conclusão

Este guia fornece um caminho claro e prático para implementar estratégias de captação de clientes desistentes. Comece pelas ações de impacto rápido, implemente gradualmente os recursos mais complexos, e sempre monitore as métricas para ajustar o curso.

**Lembre-se:** 
- ✅ Começar pequeno e iterar rápido
- ✅ Medir tudo
- ✅ Testar antes de escalar
- ✅ Focar no ROI
- ✅ Respeitar privacidade (LGPD)

**Próximo Passo:** Escolha uma ação de impacto rápido e comece HOJE! 🚀

---

**Documentos relacionados:**
- `ANALISE_DADOS_CADASTRO_ESTRATEGIAS.md` - Estratégias de negócio
- `ANALISE_DADOS_CADASTRO_GUIA_TECNICO.md` - Implementação técnica detalhada
