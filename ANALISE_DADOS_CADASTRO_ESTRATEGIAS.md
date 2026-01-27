# Estratégias de Análise de Dados de Cadastro e Captação de Clientes

## 📋 Visão Geral

Este documento apresenta estratégias e ideias para interpretar os dados capturados durante o fluxo de cadastro no sistema MedicWarehouse, com o objetivo de:

1. **Identificar e recuperar clientes desistentes** (reduzir churn no funil)
2. **Otimizar o fluxo de atração** de novos clientes
3. **Aumentar a taxa de conversão** do cadastro

O sistema já possui uma infraestrutura robusta de captura de dados através da entidade `SalesFunnelMetric`, que registra cada etapa do processo de cadastro.

## 🎯 Dados Capturados Atualmente

### Estrutura de Dados
O sistema captura as seguintes informações em cada etapa do cadastro:

- **SessionId**: Identificador único da jornada do usuário
- **Step**: Etapa atual (1-6)
  - Etapa 1: Informações da Clínica
  - Etapa 2: Endereço
  - Etapa 3: Informações do Proprietário
  - Etapa 4: Credenciais de Login
  - Etapa 5: Seleção de Plano
  - Etapa 6: Confirmação
- **Action**: Ação realizada (entered, completed, abandoned)
- **CapturedData**: Dados preenchidos (sanitizados, sem senhas)
- **PlanId**: Plano selecionado
- **IpAddress**: Endereço IP (para análise geográfica)
- **UserAgent**: Navegador/dispositivo utilizado
- **Referrer**: Origem do tráfego
- **DurationMs**: Tempo gasto em cada etapa
- **Metadata**: Parâmetros UTM, testes A/B, etc.
- **IsConverted**: Se completou o cadastro

## 💡 Estratégias de Análise e Ação

### 1. Análise de Abandono por Etapa

#### Objetivo
Identificar em qual etapa os usuários estão desistindo do cadastro.

#### Implementação
```sql
-- Exemplo de query para identificar taxa de abandono por etapa
SELECT 
    Step,
    StepName,
    COUNT(DISTINCT SessionId) as Total,
    SUM(CASE WHEN Action = 'abandoned' THEN 1 ELSE 0 END) as Abandonos,
    ROUND(100.0 * SUM(CASE WHEN Action = 'abandoned' THEN 1 ELSE 0 END) / COUNT(DISTINCT SessionId), 2) as TaxaAbandono
FROM SalesFunnelMetrics
GROUP BY Step, StepName
ORDER BY Step;
```

#### Ações Recomendadas
- **Se alta taxa de abandono na Etapa 1-2**: Simplificar campos obrigatórios
- **Se alta taxa de abandono na Etapa 3**: Validação de CPF pode estar gerando fricção
- **Se alta taxa de abandono na Etapa 5**: Preços podem estar fora do mercado ou pouco claros

### 2. Segmentação de Usuários Desistentes

#### 2.1 Por Tempo Decorrido
Classificar usuários desistentes pelo tempo desde o abandono:

- **Quentes (0-24h)**: Alta prioridade para re-engajamento imediato
- **Mornos (24-72h)**: Prioridade média para campanhas de recuperação
- **Frios (>72h)**: Baixa prioridade, campanhas de remarketing de longo prazo

#### 2.2 Por Etapa de Abandono
Diferentes mensagens para diferentes etapas:

- **Abandonou na Etapa 1-2**: "Complete seu cadastro em apenas 3 minutos"
- **Abandonou na Etapa 3-4**: "Seus dados estão seguros conosco - LGPD compliant"
- **Abandonou na Etapa 5**: "Ofertas especiais de planos para você"

#### 2.3 Por Plano Visualizado
Se o usuário visualizou um plano específico:

- Enviar comparação de planos
- Destacar benefícios do plano visualizado
- Oferecer desconto ou período de trial estendido

### 3. Campanhas de Recuperação Automatizadas

#### 3.1 Email de Recuperação Imediata (1-2 horas após abandono)
```json
{
  "trigger": "SessionAbandoned + 2h sem retorno",
  "segmento": "Todos os abandonos",
  "conteudo": {
    "assunto": "Faltou pouco! Complete seu cadastro no MedicWarehouse",
    "corpo": "Olá! Notamos que você começou seu cadastro mas não finalizou. Podemos ajudar?"
  },
  "cta": "Link direto para retomar no passo em que parou"
}
```

#### 3.2 Email de Recuperação com Incentivo (24h)
```json
{
  "trigger": "SessionAbandoned + 24h",
  "segmento": "Não retornaram após primeiro email",
  "conteudo": {
    "assunto": "🎁 Oferta especial: 30 dias grátis para você",
    "corpo": "Complete seu cadastro hoje e ganhe 30 dias de trial gratuito!"
  },
  "incentivo": "Trial estendido ou desconto no primeiro mês"
}
```

#### 3.3 SMS de Recuperação (Para quem forneceu telefone)
```
"Olá! Vimos que você se interessou pelo MedicWarehouse. Complete seu cadastro: [link curto]"
```

#### 3.4 Retargeting via Pixel/Ads
Para usuários que abandonaram:
- Anúncios no Google Ads destacando benefícios
- Anúncios no Facebook/Instagram com casos de sucesso
- Anúncios no LinkedIn para clínicas maiores (planos Premium/Enterprise)

### 4. Análise de Padrões de Sucesso

#### Identificar características de usuários que convertem
```sql
-- Exemplo: Analisar características de sessões convertidas
SELECT 
    CASE 
        WHEN JSON_EXTRACT(Metadata, '$.utm_source') IS NOT NULL 
        THEN JSON_EXTRACT(Metadata, '$.utm_source')
        ELSE 'Direct'
    END as TrafficSource,
    COUNT(*) as Conversions,
    AVG(TotalDurationMs) as AvgTimeToConvert
FROM (
    SELECT 
        SessionId,
        Metadata,
        SUM(DurationMs) as TotalDurationMs
    FROM SalesFunnelMetrics
    WHERE IsConverted = 1
    GROUP BY SessionId, Metadata
) converted_sessions
GROUP BY TrafficSource
ORDER BY Conversions DESC;
```

#### Insights Acionáveis
- **Fontes de tráfego com melhor conversão**: Aumentar investimento
- **Tempo médio até conversão**: Otimizar fluxo se muito longo
- **Dispositivos com melhor conversão**: Otimizar experiência mobile/desktop

### 5. Análise Geográfica e Temporal

#### 5.1 Por Região (via IP)
- Identificar regiões com mais abandonos
- Criar campanhas regionalizadas
- Ajustar preços por região se necessário

#### 5.2 Por Horário
```sql
-- Identificar melhores horários de conversão
SELECT 
    HOUR(CreatedAt) as Hora,
    COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) as Conversions,
    COUNT(DISTINCT SessionId) as Total,
    ROUND(100.0 * COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) / COUNT(DISTINCT SessionId), 2) as TaxaConversao
FROM SalesFunnelMetrics
GROUP BY HOUR(CreatedAt)
ORDER BY TaxaConversao DESC;
```

#### Ações
- Agendar campanhas de email nos horários de melhor conversão
- Disponibilizar chat ao vivo nos horários críticos
- Ajustar anúncios para horários de pico

### 6. Análise de Dispositivos e Navegadores

#### Objetivo
Identificar problemas técnicos que podem estar causando abandonos.

```sql
-- Analisar conversão por tipo de dispositivo
SELECT 
    CASE 
        WHEN UserAgent LIKE '%Mobile%' THEN 'Mobile'
        WHEN UserAgent LIKE '%Tablet%' THEN 'Tablet'
        ELSE 'Desktop'
    END as DeviceType,
    COUNT(DISTINCT SessionId) as TotalSessions,
    COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) as Conversions,
    ROUND(100.0 * COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) / COUNT(DISTINCT SessionId), 2) as ConversionRate
FROM SalesFunnelMetrics
GROUP BY DeviceType;
```

#### Ações
- Se mobile tem taxa baixa: otimizar responsividade
- Se navegador específico tem problemas: testar e corrigir bugs
- Implementar testes cross-browser automatizados

### 7. Análise de Planos Selecionados

#### Identificar preferências e otimizar ofertas
```sql
-- Quais planos são mais visualizados mas não convertem?
SELECT 
    PlanId,
    COUNT(DISTINCT SessionId) as TimesViewed,
    COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) as Conversions,
    ROUND(100.0 * COUNT(DISTINCT CASE WHEN IsConverted = 1 THEN SessionId END) / COUNT(DISTINCT SessionId), 2) as ConversionRate
FROM SalesFunnelMetrics
WHERE PlanId IS NOT NULL
GROUP BY PlanId
ORDER BY TimesViewed DESC;
```

#### Ações
- **Plano muito visualizado mas baixa conversão**: 
  - Preço pode estar alto
  - Benefícios não estão claros
  - Falta de trial ou garantia
- **Plano pouco visualizado**:
  - Reposicionar na página
  - Destacar benefícios únicos
  - Adicionar badge "Mais popular" ou "Melhor valor"

### 8. Testes A/B e Otimização Contínua

#### 8.1 Elementos para Testar
- **Headlines e descrições** de planos
- **Ordem dos campos** no formulário
- **Campos obrigatórios vs opcionais**
- **Quantidade de etapas** (6 vs 4 vs 3)
- **Design visual** (cores, botões, layout)
- **Prova social** (depoimentos, número de clientes)
- **Garantias** (satisfação garantida, período de trial)
- **Urgência** (ofertas por tempo limitado)

#### 8.2 Implementação
Usar campo `Metadata` para rastrear variantes:
```json
{
  "ab_test": "checkout_flow_v2",
  "variant": "A",
  "test_start": "2026-01-01"
}
```

### 9. Pontuação de Lead (Lead Scoring)

#### Criar um sistema de pontuação para priorizar leads
```javascript
// Exemplo de algoritmo de pontuação
function calculateLeadScore(session) {
  let score = 0;
  
  // Quanto mais longe chegou, maior a pontuação
  score += session.lastStep * 10; // 10-60 pontos
  
  // Tempo gasto (engajamento)
  if (session.totalDuration > 300000) score += 20; // >5min = +20 pontos
  else if (session.totalDuration > 120000) score += 10; // >2min = +10 pontos
  
  // Plano visualizado
  if (session.planId === 'premium') score += 30; // Interesse em plano premium
  else if (session.planId === 'standard') score += 20;
  
  // Dados preenchidos
  if (session.hasEmail) score += 15;
  if (session.hasPhone) score += 15;
  if (session.hasCompanyData) score += 10;
  
  // Fonte de tráfego qualificado
  if (session.referrer?.includes('google')) score += 10;
  
  return score; // 0-150 pontos
}

// Classificação
// 100-150: Hot Lead - Contato imediato via telefone
// 60-99: Warm Lead - Email personalizado + remarketing
// 30-59: Cold Lead - Email automático + remarketing leve
// 0-29: Very Cold - Apenas remarketing passivo
```

### 10. Dashboard de Métricas em Tempo Real

#### KPIs Essenciais a Monitorar
1. **Taxa de Conversão Geral**: % de sessões que completam cadastro
2. **Taxa de Conversão por Etapa**: % que avançam de cada etapa
3. **Tempo Médio até Conversão**: Duração total da jornada
4. **Custo por Aquisição (CPA)**: Gasto em marketing / conversões
5. **Valor do Tempo de Vida (LTV)**: Receita média por cliente
6. **Taxa de Recuperação**: % de abandonos que retornam
7. **ROI de Campanhas**: Retorno de cada campanha de recuperação

#### Alertas Automáticos
- ⚠️ Taxa de conversão caiu >10% comparado à semana anterior
- ⚠️ Tempo médio em etapa específica aumentou significativamente
- ⚠️ Spike anormal de abandonos em etapa específica
- ✅ Campanha de recuperação com ROI >500%

## 🚀 Roadmap de Implementação

### Fase 1: Fundação (Semana 1-2)
- [ ] Configurar dashboard básico de métricas
- [ ] Implementar queries de análise fundamentais
- [ ] Criar relatório semanal automatizado

### Fase 2: Recuperação Básica (Semana 3-4)
- [ ] Implementar sistema de email de recuperação (2h após abandono)
- [ ] Criar templates de email personalizados por etapa
- [ ] Configurar tracking de abertura e cliques

### Fase 3: Segmentação Avançada (Semana 5-6)
- [ ] Implementar lead scoring
- [ ] Criar segmentos de campanhas específicas
- [ ] Integrar com ferramenta de CRM

### Fase 4: Otimização (Semana 7-8)
- [ ] Implementar framework de testes A/B
- [ ] Criar variantes de checkout para teste
- [ ] Analisar resultados e implementar vencedores

### Fase 5: Automação Completa (Semana 9-12)
- [ ] Implementar retargeting via pixel
- [ ] Criar jornadas automatizadas multi-canal
- [ ] Implementar SMS de recuperação
- [ ] Machine Learning para previsão de churn

## 📊 Exemplos de Análises Práticas

### Análise 1: Identificar Gargalos do Funil
```sql
WITH step_metrics AS (
  SELECT 
    Step,
    StepName,
    COUNT(DISTINCT SessionId) as Total,
    COUNT(DISTINCT CASE WHEN Action = 'completed' THEN SessionId END) as Completed
  FROM SalesFunnelMetrics
  WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)
  GROUP BY Step, StepName
)
SELECT 
  Step,
  StepName,
  Total,
  Completed,
  ROUND(100.0 * Completed / Total, 2) as CompletionRate,
  LAG(Completed) OVER (ORDER BY Step) as PreviousStepCompleted,
  ROUND(100.0 * (LAG(Completed) OVER (ORDER BY Step) - Completed) / LAG(Completed) OVER (ORDER BY Step), 2) as DropoffRate
FROM step_metrics
ORDER BY Step;
```

### Análise 2: Perfil de Clientes que Convertem Rápido
```sql
SELECT 
  CASE 
    WHEN TotalDuration < 300000 THEN 'Rápido (<5min)'
    WHEN TotalDuration < 600000 THEN 'Médio (5-10min)'
    ELSE 'Lento (>10min)'
  END as ConversionSpeed,
  COUNT(*) as Count,
  AVG(PlanPrice) as AvgPlanPrice,
  GROUP_CONCAT(DISTINCT TrafficSource) as Sources
FROM (
  SELECT 
    sfm.SessionId,
    SUM(sfm.DurationMs) as TotalDuration,
    sp.MonthlyPrice as PlanPrice,
    JSON_EXTRACT(sfm.Metadata, '$.utm_source') as TrafficSource
  FROM SalesFunnelMetrics sfm
  LEFT JOIN ClinicSubscriptions cs ON sfm.ClinicId = cs.ClinicId
  LEFT JOIN SubscriptionPlans sp ON cs.PlanId = sp.Id
  WHERE sfm.IsConverted = 1
  AND sfm.CreatedAt >= DATE_SUB(NOW(), INTERVAL 90 DAY)
  GROUP BY sfm.SessionId, sp.MonthlyPrice
) converted
GROUP BY ConversionSpeed;
```

### Análise 3: Padrões de Abandono
```sql
SELECT 
  Step,
  StepName,
  HOUR(CreatedAt) as Hour,
  COUNT(*) as Abandonments,
  AVG(DurationMs) / 1000 as AvgSecondsBeforeAbandon
FROM SalesFunnelMetrics
WHERE Action = 'abandoned'
AND CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)
GROUP BY Step, StepName, HOUR(CreatedAt)
HAVING Abandonments > 5
ORDER BY Abandonments DESC
LIMIT 20;
```

## 🎓 Melhores Práticas

### 1. Privacidade e LGPD
- ✅ Sempre sanitizar dados sensíveis (senhas, dados bancários)
- ✅ Anonimizar IPs após análise geográfica
- ✅ Implementar política de retenção de dados (ex: 90 dias)
- ✅ Obter consentimento para comunicações de marketing
- ✅ Disponibilizar opt-out fácil em todos os emails

### 2. Qualidade de Dados
- ✅ Validar dados antes de armazenar
- ✅ Usar IDs consistentes (SessionId como UUID)
- ✅ Timestamp preciso com timezone UTC
- ✅ Metadata estruturada em JSON válido

### 3. Performance
- ✅ Indexar campos frequentemente consultados (SessionId, CreatedAt, IsConverted)
- ✅ Particionar tabela por data se volume > 1M registros/mês
- ✅ Usar cache para dashboards em tempo real
- ✅ Processar análises pesadas em background jobs

### 4. Testes
- ✅ Sempre ter grupo de controle em testes A/B
- ✅ Calcular significância estatística antes de concluir
- ✅ Documentar todos os testes e resultados
- ✅ Não otimizar múltiplas variáveis simultaneamente

## 📈 Métricas de Sucesso

### Objetivos Mensuráveis
- **Aumentar taxa de conversão em 20%** nos próximos 3 meses
- **Reduzir abandono na etapa crítica em 30%**
- **Recuperar 15% dos usuários** que abandonaram via campanhas
- **Reduzir tempo médio de conversão em 25%**
- **Alcançar ROI de 500%** em campanhas de recuperação

### Monitoramento Contínuo
- Review semanal de métricas principais
- Análise mensal detalhada de tendências
- Quarterly business review com stakeholders
- Ajuste contínuo de estratégias baseado em dados

## 🔗 Recursos Adicionais

### Ferramentas Recomendadas
- **Analytics**: Google Analytics 4, Mixpanel, Amplitude
- **Email Marketing**: SendGrid, Mailchimp, Customer.io
- **CRM**: HubSpot, Salesforce, Pipedrive
- **A/B Testing**: Optimizely, VWO, Google Optimize
- **Retargeting**: Google Ads, Facebook Pixel, AdRoll
- **SMS**: Twilio, Vonage, AWS SNS
- **BI/Dashboards**: Metabase, Superset, Tableau

### Integrações Sugeridas
1. **Webhook para CRM**: Enviar leads quentes automaticamente
2. **Pixel de remarketing**: Facebook, Google, LinkedIn
3. **Automação de email**: Integrar com SendGrid/Mailchimp
4. **Chat ao vivo**: Intercom, Drift para assistência em tempo real
5. **Analytics avançado**: Enviar eventos para Google Analytics

## 📚 Conclusão

A infraestrutura de captura de dados já implementada no MedicWarehouse através do `SalesFunnelMetric` fornece uma base sólida para:

1. **Compreender profundamente** o comportamento dos usuários no funil de cadastro
2. **Identificar pontos de fricção** que causam abandono
3. **Segmentar e recuperar** usuários que não completaram o cadastro
4. **Otimizar continuamente** o fluxo baseado em dados reais
5. **Maximizar o ROI** de investimentos em marketing e aquisição

A chave para o sucesso está na **implementação gradual** das estratégias, começando pelas de maior impacto e menor complexidade, e na **cultura de otimização baseada em dados** onde todas as decisões são validadas através de métricas e testes.

---

**Próximos Passos**: Consulte o documento `ANALISE_DADOS_CADASTRO_GUIA_TECNICO.md` para detalhes de implementação técnica de cada estratégia.
