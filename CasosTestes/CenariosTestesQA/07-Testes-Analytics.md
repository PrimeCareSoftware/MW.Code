# 07 - Cenários de Testes de Analytics e BI

> **Módulo:** Business Intelligence e Analytics  
> **Tempo estimado:** 25 minutos

## 🎯 Objetivo

Validar dashboards e relatórios analíticos:
- ✅ Dashboard principal
- ✅ Métricas financeiras
- ✅ Métricas operacionais
- ✅ Relatórios customizados
- ✅ Exportação de dados

## 📝 Casos de Teste

### CT-ANALYTICS-001: Visualizar Dashboard Principal
**Passos:** Login > Dashboard
**Esperado:** Cards com KPIs: consultas hoje, faturamento mês, taxa ocupação

### CT-ANALYTICS-002: Filtrar por Período
**Passos:** Dashboard > Filtro: "Último mês"
**Esperado:** Dados atualizados para período selecionado

### CT-ANALYTICS-003: Ver Relatório Financeiro
**Passos:** Analytics > Financeiro > Faturamento Mensal
**Esperado:** Gráfico de barras, total por mês, comparativo ano anterior

### CT-ANALYTICS-004: Ver Taxa de No-Show
**Passos:** Analytics > Operacional > No-Show
**Esperado:** Percentual de faltas, tendência, motivos

### CT-ANALYTICS-005: Ver Ranking de Médicos
**Passos:** Analytics > Médicos > Produtividade
**Esperado:** Lista ordenada por atendimentos, faturamento

### CT-ANALYTICS-006: Criar Relatório Customizado
**Passos:** Analytics > Novo Relatório > Configure campos
**Esperado:** Relatório salvo, pode ser executado novamente

### CT-ANALYTICS-007: Exportar Dados para Excel
**Passos:** Relatório > Exportar > Excel
**Esperado:** Arquivo .xlsx baixado com dados

### CT-ANALYTICS-008: Dashboard de Convênios (TISS)
**Passos:** Analytics > Convênios > Glosas
**Esperado:** Taxa de glosa por convênio, valor glosado

### CT-ANALYTICS-009: Análise de Forecast
**Passos:** Analytics > Machine Learning > Previsões
**Esperado:** Previsão de demanda próximos 30 dias

### CT-ANALYTICS-010: Ver Tempo Médio de Espera
**Passos:** Analytics > Operacional > Tempo Espera
**Esperado:** Média em minutos, por médico, por horário

## ✅ Critérios de Aceite
- [ ] Dashboards carregam corretamente
- [ ] Filtros funcionam
- [ ] Dados calculados corretamente
- [ ] Gráficos renderizam bem
- [ ] Exportação funciona

## 📚 Documentação
- [BI Analytics Implementation](../../IMPLEMENTATION_SUMMARY_BI_ANALYTICS.md)
- [Testing Guide BI Analytics](../../TESTING_GUIDE_BI_ANALYTICS.md)

## ⏭️ Próximos Passos
➡️ [08-Testes-Acessibilidade.md](08-Testes-Acessibilidade.md)
