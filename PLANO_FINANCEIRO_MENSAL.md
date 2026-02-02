# Plano Financeiro Mensal - PrimeCare SaaS Multi-Especialidades

## Data de Criação
02 de Fevereiro de 2026

## Contexto
Baseado nas implementações dos PRs #608 e #609, que estabeleceram a fundação multi-especialidades do sistema PrimeCare (8 especialidades: Medicina, Psicologia, Nutrição, Fisioterapia, Odontologia, Enfermagem, Terapia Ocupacional e Fonoaudiologia), este documento define o planejamento financeiro mensal unificado.

**Premissa Inicial**: Não haverá diferenciação de preço por tipo de clínica/especialidade. Todos os tipos profissionais pagarão o mesmo valor baseado no tamanho do negócio e recursos utilizados.

---

## 1. Estrutura de Preços Mensais

### 1.1 Planos Disponíveis

#### **Plano Starter** - MVP Básico
- **Preço Mensal**: R$ 49,00
- **Público-Alvo**: Profissionais autônomos de qualquer especialidade
- **Período de Trial**: 14 dias
- **Limites**:
  - 1 usuário
  - 50 pacientes
- **Recursos Incluídos**:
  - Agenda de consultas/sessões básica
  - Cadastro de pacientes
  - Prontuário digital simples (adaptado à especialidade)
  - Relatórios básicos
  - Terminologia personalizada por especialidade
  - Modelos de documentos padrão
  - Suporte por email (48h)

#### **Plano Professional** - MVP Intermediário ⭐ RECOMENDADO
- **Preço Mensal**: R$ 89,00
- **Público-Alvo**: Consultórios pequenos de qualquer especialidade
- **Período de Trial**: 14 dias
- **Limites**:
  - 2 usuários
  - 200 pacientes
- **Recursos Incluídos**:
  - Todos os recursos do Starter
  - Agenda avançada (múltiplos profissionais)
  - Prontuário digital completo (adaptado à especialidade)
  - Módulo Financeiro básico
  - Relatórios gerenciais
  - Portal do Paciente (básico)
  - Templates de documentos customizáveis
  - Business Configuration (16 feature toggles)
  - Suporte prioritário (24h)

#### **Plano Enterprise** - MVP Avançado
- **Preço Mensal**: R$ 149,00
- **Público-Alvo**: Clínicas estabelecidas de qualquer especialidade
- **Período de Trial**: 14 dias
- **Limites**:
  - 5 usuários
  - Pacientes ilimitados
- **Recursos Incluídos**:
  - Todos os recursos do Professional
  - Módulo Financeiro completo
  - Gestão de estoque
  - Fila de espera
  - Telemedicina básica
  - Portal do Paciente completo
  - Editor de templates avançado
  - Relatórios avançados
  - Conformidade LGPD
  - Onboarding wizard personalizado
  - Suporte 24/7

#### **Plano Personalizado** - Sob Consulta
- **Preço Mensal**: A partir de R$ 300,00
- **Público-Alvo**: Clínicas grandes ou redes de clínicas
- **Características**:
  - Usuários ilimitados
  - Pacientes ilimitados
  - Múltiplas clínicas
  - Desenvolvimento de funcionalidades específicas
  - Integrações customizadas
  - Gerente de conta dedicado
  - SLA garantido

---

## 2. Projeção de Receitas Mensais

### 2.1 Cenário Conservador (Ano 1)

| Período | Starter | Professional | Enterprise | Total Clientes | MRR (R$) |
|---------|---------|--------------|------------|----------------|----------|
| Mês 1   | 10      | 5            | 2          | 17             | 1.133    |
| Mês 3   | 25      | 15           | 5          | 45             | 3.320    |
| Mês 6   | 50      | 35           | 12         | 97             | 7.903    |
| Mês 9   | 80      | 60           | 20         | 160            | 14.260   |
| Mês 12  | 120     | 90           | 30         | 240            | 22.350   |

**MRR (Monthly Recurring Revenue) = (Starter × 49) + (Professional × 89) + (Enterprise × 149)**

### 2.2 Cenário Moderado (Ano 1)

| Período | Starter | Professional | Enterprise | Total Clientes | MRR (R$) |
|---------|---------|--------------|------------|----------------|----------|
| Mês 1   | 15      | 10           | 3          | 28             | 2.072    |
| Mês 3   | 40      | 30           | 10         | 80             | 6.630    |
| Mês 6   | 80      | 60           | 20         | 160            | 14.260   |
| Mês 9   | 130     | 100          | 35         | 265            | 25.485   |
| Mês 12  | 200     | 150          | 50         | 400            | 41.150   |

### 2.3 Cenário Otimista (Ano 1)

| Período | Starter | Professional | Enterprise | Total Clientes | MRR (R$) |
|---------|---------|--------------|------------|----------------|----------|
| Mês 1   | 25      | 20           | 8          | 53             | 4.217    |
| Mês 3   | 70      | 50           | 20         | 140            | 11.880   |
| Mês 6   | 150     | 100          | 40         | 290            | 25.250   |
| Mês 9   | 250     | 180          | 70         | 500            | 46.670   |
| Mês 12  | 400     | 300          | 100        | 800            | 76.600   |

---

## 3. Custos Operacionais Mensais

### 3.1 Infraestrutura de TI

| Item | Custo Mensal (R$) | Observações |
|------|-------------------|-------------|
| Servidor de Produção (AWS/Azure) | 800 | Escalável baseado em uso |
| Banco de Dados PostgreSQL | 400 | RDS ou similar |
| CDN e Storage (S3) | 200 | Para arquivos e backups |
| Serviço de Email (SendGrid/AWS SES) | 150 | Até 50k emails/mês |
| Monitoramento (Datadog/New Relic) | 300 | APM e logs |
| SSL/Certificados | 50 | Renovação anual |
| Backup e Disaster Recovery | 250 | Segurança e conformidade |
| **Subtotal Infraestrutura** | **2.150** | |

### 3.2 Desenvolvimento e Manutenção

| Item | Custo Mensal (R$) | Observações |
|------|-------------------|-------------|
| Desenvolvedor Full-Stack | 8.000 | 1 desenvolvedor |
| Suporte Técnico (Part-time) | 3.000 | Atendimento aos clientes |
| DevOps/SysAdmin (Part-time) | 4.000 | Manutenção da infraestrutura |
| Designer UI/UX (Freelance) | 2.000 | Melhorias contínuas |
| **Subtotal Desenvolvimento** | **17.000** | |

### 3.3 Marketing e Vendas

| Item | Custo Mensal (R$) | Observações |
|------|-------------------|-------------|
| Google Ads | 1.500 | Aquisição de clientes |
| Facebook/Instagram Ads | 1.000 | Marketing digital |
| SEO e Content Marketing | 800 | Blog e materiais |
| Email Marketing (Mailchimp) | 200 | Automação |
| CRM (HubSpot/Pipedrive) | 300 | Gestão de leads |
| **Subtotal Marketing** | **3.800** | |

### 3.4 Operacional e Administrativo

| Item | Custo Mensal (R$) | Observações |
|------|-------------------|-------------|
| Contabilidade | 800 | Serviços contábeis |
| Jurídico | 500 | Consultoria legal |
| Seguros | 300 | Responsabilidade civil |
| Ferramentas de Produtividade | 200 | Slack, GitHub, etc. |
| Taxas Bancárias e Gateway de Pagamento | 500 | Stripe/PagSeguro (estimativa) |
| Contingência (10%) | 2.245 | Imprevistos |
| **Subtotal Operacional** | **4.545** | |

### 3.5 Custo Total Mensal

| Categoria | Valor (R$) |
|-----------|------------|
| Infraestrutura | 2.150 |
| Desenvolvimento | 17.000 |
| Marketing | 3.800 |
| Operacional | 4.545 |
| **TOTAL** | **27.495** |

---

## 4. Análise de Break-Even

### 4.1 Ponto de Equilíbrio

Para cobrir os custos operacionais mensais de **R$ 27.495**, precisamos de:

**Cenário 1: Mix Equilibrado (40% Starter, 40% Professional, 20% Enterprise)**
- Receita média por cliente = (0.4 × 49) + (0.4 × 89) + (0.2 × 149) = R$ 85,00
- **Clientes necessários = 27.495 / 85 = 324 clientes**

**Cenário 2: Focus Professional (20% Starter, 60% Professional, 20% Enterprise)**
- Receita média por cliente = (0.2 × 49) + (0.6 × 89) + (0.2 × 149) = R$ 93,20
- **Clientes necessários = 27.495 / 93,20 = 295 clientes**

**Cenário 3: Focus Enterprise (20% Starter, 30% Professional, 50% Enterprise)**
- Receita média por cliente = (0.2 × 49) + (0.3 × 89) + (0.5 × 149) = R$ 111,00
- **Clientes necessários = 27.495 / 111 = 248 clientes**

### 4.2 Timeline para Break-Even

Baseado nos cenários de receita:

| Cenário | Break-Even Esperado |
|---------|---------------------|
| Conservador | Mês 14-16 |
| Moderado | Mês 10-12 |
| Otimista | Mês 8-9 |

---

## 5. Estratégias de Redução de Custos

### 5.1 Fase Inicial (Meses 1-6)

Para reduzir custos iniciais até alcançar tração:

| Ação | Economia Mensal (R$) | Impacto |
|------|----------------------|---------|
| Usar servidor compartilhado | 400 | Migrar para dedicado após 100 clientes |
| Suporte apenas por email | 1.500 | Adicionar chat ao vivo depois |
| Marketing orgânico focado | 1.500 | Reduzir ads pagos inicialmente |
| Desenvolvedor Fundador (sem salário) | 8.000 | Equity ao invés de salário |
| **Total Economia** | **11.400** | |

**Custo Operacional Reduzido Fase 1 = R$ 16.095/mês**

Com custos reduzidos:
- **Break-even = 16.095 / 85 = 190 clientes** (mix equilibrado)
- **Alcançável em 6-8 meses no cenário moderado**

### 5.2 Otimização de Infraestrutura

À medida que a base de clientes cresce:

- **0-100 clientes**: Infraestrutura compartilhada (~R$ 1.200/mês)
- **100-500 clientes**: Infraestrutura escalável (~R$ 2.150/mês)
- **500+ clientes**: Infraestrutura dedicada com otimizações (~R$ 3.500/mês)

---

## 6. Projeção de Lucro

### 6.1 Ano 1 - Cenário Moderado

| Trimestre | MRR Médio | Receita Trim. | Custos Trim. | Lucro/Prejuízo | Margem |
|-----------|-----------|---------------|--------------|----------------|--------|
| Q1 | 4.351 | 13.053 | 48.285 | (35.232) | -270% |
| Q2 | 10.445 | 31.335 | 48.285 | (16.950) | -54% |
| Q3 | 19.873 | 59.619 | 48.285 | 11.334 | 19% |
| Q4 | 33.318 | 99.954 | 48.285 | 51.669 | 52% |
| **Ano** | **16.997** | **203.961** | **193.140** | **10.821** | **5%** |

### 6.2 Ano 2 - Projeção

Assumindo crescimento de 150% na base de clientes:

| Métrica | Valor |
|---------|-------|
| Clientes finais Ano 2 | 1.000 |
| MRR médio Ano 2 | R$ 95.000 |
| Receita Anual | R$ 1.140.000 |
| Custos Operacionais | R$ 450.000 |
| **Lucro Líquido** | **R$ 690.000** |
| **Margem de Lucro** | **61%** |

---

## 7. Estratégias de Crescimento

### 7.1 Aquisição de Clientes

**Custo de Aquisição de Cliente (CAC)**:
- Target inicial: R$ 200 por cliente
- Objetivo Ano 2: R$ 150 por cliente

**Lifetime Value (LTV)**:
- Plano Professional (principal): R$ 89/mês × 24 meses = R$ 2.136
- **Razão LTV/CAC = 2.136 / 200 = 10.7x** (excelente)

### 7.2 Canais de Aquisição

| Canal | Investimento Mensal | Clientes Esperados | CAC |
|-------|---------------------|-------------------|-----|
| Google Ads | R$ 1.500 | 8-10 | R$ 180 |
| Facebook/Instagram | R$ 1.000 | 6-8 | R$ 143 |
| SEO Orgânico | R$ 800 | 5-8 | R$ 123 |
| Indicação (Referral) | R$ 0 | 3-5 | R$ 0 |
| **Total** | **R$ 3.300** | **22-31** | **R$ 135** |

### 7.3 Retenção de Clientes

**Taxa de Churn Target**: < 5% ao mês

Estratégias:
1. Onboarding personalizado por especialidade
2. Suporte proativo (não reativo)
3. Treinamentos mensais gratuitos
4. Comunidade de usuários
5. Programa de fidelidade (desconto anual)
6. Feature roadmap transparente

**Impacto da Retenção**:
- Churn 5%: Lifetime = 20 meses → LTV = R$ 1.780
- Churn 3%: Lifetime = 33 meses → LTV = R$ 2.937
- Churn 2%: Lifetime = 50 meses → LTV = R$ 4.450

---

## 8. Investimento Inicial Necessário

### 8.1 Capital Mínimo para 12 Meses

**Runway Necessário (Cenário Conservador)**:

| Item | Valor (R$) |
|------|------------|
| Custos Operacionais 12 meses | 193.140 |
| Buffer de Segurança (20%) | 38.628 |
| Marketing Inicial (3 meses) | 11.400 |
| Desenvolvimento Inicial | 24.000 |
| Legal e Estruturação | 15.000 |
| **Total Necessário** | **282.168** |

### 8.2 Fontes de Financiamento

**Opções**:
1. **Bootstrapping**: Fundadores investem próprio capital
2. **Investidor Anjo**: R$ 150.000 - R$ 300.000 por 10-15% equity
3. **Aceleradora**: R$ 100.000 + mentoria por 5-10% equity
4. **Crowdfunding**: Campanha de pré-venda com early adopters
5. **Empréstimo BNDES/Finep**: Linhas para startups de tecnologia

---

## 9. Modelo de Precificação Diferenciada (Futuro)

### 9.1 Possível Diferenciação por Especialidade (Fase 2)

Embora não seja a estratégia inicial, no futuro pode-se considerar:

| Especialidade | Ajuste de Preço | Justificativa |
|---------------|----------------|---------------|
| Medicina | Base (0%) | Referência |
| Odontologia | +10% | Complexidade de prontuário (odontograma) |
| Psicologia | Base (0%) | Similar à medicina |
| Nutrição | Base (0%) | Similar à medicina |
| Fisioterapia | +5% | Templates de evolução específicos |
| Demais | Base (0%) | Funcionalidades padrão |

**Observação**: Esta diferenciação só deve ser considerada após:
- Validação de mercado com preço único
- Análise de custos reais por especialidade
- Feedback dos clientes sobre percepção de valor

### 9.2 Upsell e Cross-sell

**Módulos Adicionais (Add-ons)**:
- Telemedicina Avançada: +R$ 29/mês
- Assinatura Digital ICP-Brasil: +R$ 39/mês
- Exportação TISS Completa: +R$ 19/mês
- Integração WhatsApp Business: +R$ 24/mês
- SMS em Massa: R$ 0,15 por SMS
- Módulo de Marketing Automation: +R$ 49/mês

**Potencial de Receita Adicional**: +30-50% do MRR base

---

## 10. Métricas de Sucesso

### 10.1 KPIs Principais

| Métrica | Meta Mês 6 | Meta Mês 12 | Meta Mês 24 |
|---------|------------|-------------|-------------|
| Clientes Ativos | 160 | 400 | 1.000 |
| MRR | R$ 14.260 | R$ 41.150 | R$ 95.000 |
| Churn Rate | < 5% | < 3% | < 2% |
| CAC | R$ 200 | R$ 150 | R$ 100 |
| LTV/CAC Ratio | > 8x | > 10x | > 15x |
| Net Promoter Score | > 40 | > 50 | > 60 |
| Tempo Médio Onboarding | < 7 dias | < 5 dias | < 3 dias |

### 10.2 Distribuição por Especialidade (Target)

| Especialidade | % da Base | Clientes (Mês 12) |
|---------------|-----------|-------------------|
| Medicina | 35% | 140 |
| Psicologia | 25% | 100 |
| Odontologia | 15% | 60 |
| Nutrição | 10% | 40 |
| Fisioterapia | 8% | 32 |
| Outras | 7% | 28 |

---

## 11. Riscos e Mitigações

### 11.1 Riscos Financeiros

| Risco | Probabilidade | Impacto | Mitigação |
|-------|--------------|---------|-----------|
| Churn alto (>8%) | Média | Alto | Investir em Customer Success |
| CAC > R$ 300 | Baixa | Médio | Focar em orgânico e referral |
| Crescimento lento | Média | Alto | Ajustar estratégia de marketing |
| Custos de infraestrutura explodem | Baixa | Médio | Otimização contínua e monitoramento |
| Concorrência com preços mais baixos | Alta | Médio | Diferenciação por qualidade e suporte |

### 11.2 Plano de Contingência

**Se MRR < R$ 10.000 no Mês 6**:
1. Reduzir custos de marketing em 50%
2. Focar em orgânico e indicações
3. Considerar pivô no produto
4. Buscar investimento externo

**Se Churn > 8%**:
1. Entrevistar clientes que cancelaram
2. Implementar melhorias emergenciais
3. Oferecer plano de recuperação com desconto
4. Reforçar equipe de suporte

---

## 12. Roadmap Financeiro

### 12.1 Trimestre 1 (Meses 1-3)
- **Objetivo**: Validação de Produto e Mercado
- **Investimento**: R$ 48.285
- **Meta de Clientes**: 45
- **Meta MRR**: R$ 3.320
- **Foco**: Desenvolvimento, testes beta, primeiros clientes pagantes

### 12.2 Trimestre 2 (Meses 4-6)
- **Objetivo**: Crescimento Inicial
- **Investimento**: R$ 48.285
- **Meta de Clientes**: 160
- **Meta MRR**: R$ 14.260
- **Foco**: Marketing digital, otimização de conversão, retenção

### 12.3 Trimestre 3 (Meses 7-9)
- **Objetivo**: Escalabilidade
- **Investimento**: R$ 48.285
- **Meta de Clientes**: 265
- **Meta MRR**: R$ 25.485
- **Foco**: Automação, processos, expansão da equipe

### 12.4 Trimestre 4 (Meses 10-12)
- **Objetivo**: Consolidação e Lucratividade
- **Investimento**: R$ 48.285
- **Meta de Clientes**: 400
- **Meta MRR**: R$ 41.150
- **Foco**: Rentabilidade, novos módulos, preparação Ano 2

---

## 13. Conclusões e Recomendações

### 13.1 Viabilidade Financeira

✅ **O modelo de negócio é financeiramente viável** com as seguintes premissas:
- Investimento inicial de R$ 150.000 - R$ 300.000
- Alcance de 190+ clientes em 6-8 meses (fase bootstrap)
- CAC controlado abaixo de R$ 200
- Churn mantido abaixo de 5%

### 13.2 Estratégia de Preço Unificado

✅ **A estratégia de preço único (sem diferenciação por especialidade) é recomendada** porque:
- Simplifica a comunicação e marketing
- Reduz complexidade operacional
- Facilita comparação de planos
- Demonstra justiça e transparência
- Permite ajustes futuros baseados em dados reais

### 13.3 Próximos Passos

1. **Imediato (Mês 1)**:
   - Finalizar implementação de Business Configuration (PR #608/#609)
   - Validar preços com primeiros beta testers
   - Configurar gateway de pagamento
   - Implementar dashboard de métricas financeiras

2. **Curto Prazo (Meses 2-3)**:
   - Lançar campanha early adopter
   - Onboarding de primeiros 20-30 clientes pagantes
   - Coletar feedback sobre percepção de valor
   - Ajustar roadmap baseado em uso real

3. **Médio Prazo (Meses 4-6)**:
   - Escalar marketing digital
   - Implementar programa de indicação
   - Adicionar módulos premium (add-ons)
   - Avaliar necessidade de investimento externo

4. **Longo Prazo (Meses 7-12)**:
   - Consolidar base de clientes
   - Alcançar lucratividade
   - Planejar expansão internacional
   - Considerar nova rodada de investimento para crescimento acelerado

---

## 14. Aprovação e Revisões

| Versão | Data | Autor | Mudanças |
|--------|------|-------|----------|
| 1.0 | 02/02/2026 | Copilot Agent | Criação inicial do plano |

**Próxima Revisão Programada**: 01/03/2026

---

## Anexos

### A. Calculadora de Break-Even

Para calcular seu break-even personalizado:

```
Break-Even = Custos Fixos Mensais / (Preço Médio - Custo Variável por Cliente)

Onde:
- Custos Fixos = R$ 27.495 (ou R$ 16.095 em modo bootstrap)
- Preço Médio = Depende do mix de planos
- Custo Variável por Cliente ≈ R$ 5 (hospedagem, email, processamento)
```

### B. Links Úteis

- **Implementação Backend**: PR #608 - Multi-specialty foundation
- **Implementação Frontend**: PR #609 - Frontend integration
- **Documentação de Planos**: `frontend/medicwarehouse-app/src/app/models/subscription-plan.model.ts`
- **Gestão de Planos**: `frontend/mw-system-admin/src/app/pages/plans/`
- **Business Configuration**: `src/MedicSoft.Domain/Entities/BusinessConfiguration.cs`

### C. Contato para Questões Financeiras

Para dúvidas ou ajustes neste plano, contate a equipe de gestão.

---

**Documento Confidencial** - Uso interno apenas
