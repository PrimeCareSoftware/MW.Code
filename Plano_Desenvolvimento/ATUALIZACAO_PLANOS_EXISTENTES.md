# 📝 Atualização dos Planos de Desenvolvimento Existentes
## Integração da Estratégia Multi-Negócios

> **Data:** 26 de Janeiro de 2026  
> **Versão:** 1.0  
> **Objetivo:** Atualizar planos existentes para incluir adaptações multi-negócios

---

## 🎯 Sumário Executivo

Este documento analisa como a **estratégia de adaptação multi-negócios** impacta os planos de desenvolvimento já existentes e sugere ajustes necessários para garantir alinhamento estratégico.

---

## 📋 Análise dos Planos Existentes

### Planos Revisados

1. **Fase 1: Conformidade Legal** - 7 tarefas (P0)
2. **Fase 2: Segurança e LGPD** - 5 tarefas (P1)
3. **Fase 4: Analytics e Otimização** - 7 tarefas (P2)
4. **Fase 5: Enterprise** - 4 tarefas (P3)

**Total:** 23 tarefas no plano original

---

## 🔄 Ajustes Necessários por Fase

### Fase 1: Conformidade Legal

#### Tarefas Existentes Impactadas

**05-cfm-2314-telemedicina.md**
- ✅ **Já está alinhado** com a estratégia multi-negócios
- Telemedicina é essencial para profissionais autônomos
- **Ação:** Expandir para incluir outros conselhos (CRP, CRN, CREFITO)

**Novo Item Sugerido:**
```markdown
## 05b-regulamentacao-multiprofissional.md

### Objetivo
Adaptar compliance de telemedicina para outras profissões além de médicos

### Tarefas
1. **Compliance Psicologia (CFP)**
   - Resolução CFP 11/2018 (atendimento online)
   - Termo de consentimento específico
   - Cadastro Nacional de Psicólogos

2. **Compliance Nutrição (CFN)**
   - Resolução CFN 617/2019 (telenutrição)
   - Protocolo de avaliação remota
   - Documentação específica

3. **Compliance Fisioterapia (COFFITO)**
   - Resolução COFFITO 516/2020 (tele-fisioterapia)
   - Orientação remota de exercícios
   - Limitações de atendimento online

4. **Compliance Odontologia (CFO)**
   - Resolução CFO 226/2020 (teleodontologia)
   - Apenas orientação, não procedimentos
   - Integração com atendimento presencial

### Investimento
- **Tempo:** 3 meses
- **Custo:** R$ 60.000 (1 dev backend + 1 jurídico)
- **Prioridade:** P1 (Alta)

### Entregáveis
- ✅ Termos de consentimento por profissão
- ✅ Validações específicas no sistema
- ✅ Documentação de compliance
- ✅ Integração com conselhos regionais
```

---

### Fase 2: Segurança e LGPD

#### Tarefas Existentes Impactadas

**10-portal-paciente.md**
- ✅ **Requer adaptação** para diferentes especialidades
- **Ação:** Incluir temas visuais e conteúdo contextual

**Ajuste Sugerido:**
```markdown
## Adição ao 10-portal-paciente.md

### 10.4 Personalização por Especialidade

#### Temas Visuais
- **Psicologia:** Cores calmas (roxo, lilás), foco em bem-estar emocional
- **Nutrição:** Cores vibrantes (verde, laranja), foco em saúde e alimentação
- **Odontologia:** Cores limpas (azul, branco), foco em higiene e sorriso
- **Fisioterapia:** Cores energéticas (laranja, amarelo), foco em movimento

#### Conteúdo Contextual
- **Psicologia:** "Preparação para sua sessão", "Dicas de bem-estar"
- **Nutrição:** "Receitas saudáveis", "Cardápio da semana"
- **Odontologia:** "Dicas de higiene", "Cuidados pós-procedimento"
- **Fisioterapia:** "Exercícios em casa", "Alongamentos diários"

### Investimento Adicional
- **Tempo:** +2 semanas
- **Custo:** +R$ 10.000 (1 designer)
```

**11-prontuario-soap.md**
- ✅ **Requer adaptação** - SOAP é modelo médico
- **Ação:** Criar modelos alternativos por especialidade

**Ajuste Sugerido:**
```markdown
## Adição ao 11-prontuario-soap.md

### 11.5 Modelos de Prontuário por Especialidade

#### Modelo SOAP (Médicos)
- **S**ubjetivo: Queixa do paciente
- **O**bjetivo: Exame físico
- **A**valiação: Diagnóstico
- **P**lano: Tratamento

#### Modelo Psicológico (Psicólogos)
- **Motivo da Sessão**
- **Histórico/Contexto**
- **Observações da Sessão**
- **Intervenções Realizadas**
- **Tarefas para Casa**
- **Plano para Próxima Sessão**

#### Modelo Nutricional (Nutricionistas)
- **Anamnese Alimentar**
- **Avaliação Antropométrica**
- **Objetivos do Paciente**
- **Plano Alimentar**
- **Suplementação (se aplicável)**
- **Orientações**

#### Modelo Odontológico (Dentistas)
- **Queixa Principal**
- **Odontograma**
- **Diagnóstico**
- **Plano de Tratamento**
- **Procedimentos Realizados**
- **Próximos Passos**

#### Modelo Fisioterapêutico (Fisioterapeutas)
- **Anamnese**
- **Avaliação Física**
- **Diagnóstico Cinético-Funcional**
- **Plano de Tratamento**
- **Exercícios Prescritos**
- **Evolução**

### Investimento Adicional
- **Tempo:** +3 semanas
- **Custo:** +R$ 20.000 (1 dev frontend + consultorias com profissionais)
```

---

### Fase 4: Analytics e Otimização

#### Tarefas Existentes Impactadas

**15-bi-analytics.md**
- ✅ **Requer adaptação** para métricas por especialidade
- **Ação:** Criar dashboards específicos

**Ajuste Sugerido:**
```markdown
## Adição ao 15-bi-analytics.md

### 15.6 Dashboards por Especialidade

#### Dashboard Psicologia
- **Métricas:**
  - Taxa de adesão ao tratamento (% pacientes que continuam)
  - Número médio de sessões por paciente
  - Motivos mais comuns de busca por terapia
  - Taxa de alta terapêutica

#### Dashboard Nutrição
- **Métricas:**
  - Taxa de perda/ganho de peso médio
  - Aderência ao plano alimentar (%)
  - Consultas iniciais vs. retornos
  - Objetivos mais comuns (emagrecimento, ganho de massa, etc.)

#### Dashboard Odontologia
- **Métricas:**
  - Procedimentos mais realizados
  - Taxa de conversão (orçamento → procedimento)
  - Receita por procedimento
  - Taxa de retorno (prevenção vs. urgência)

#### Dashboard Fisioterapia
- **Métricas:**
  - Número médio de sessões por tratamento
  - Taxa de recuperação (melhora relatada)
  - Tipos de lesões mais atendidas
  - Efetividade de exercícios prescritos

### Investimento Adicional
- **Tempo:** +2 semanas
- **Custo:** +R$ 15.000 (1 analista BI)
```

---

### Fase 5: Enterprise

#### Tarefas Existentes Impactadas

**20-api-publica.md**
- ✅ **Já está alinhado** com estratégia
- API pública permite integrações com ferramentas de nicho
- **Ação:** Documentar casos de uso por especialidade

**21-integracao-laboratorios.md**
- ⚠️ **Parcialmente relevante**
- Médicos e nutricionistas usam, psicólogos não
- **Ação:** Tornar opcional por especialidade

**Novo Item Sugerido:**
```markdown
## 21b-integracoes-especializadas.md

### Objetivo
Criar integrações específicas por especialidade

### Integrações Propostas

#### Para Nutricionistas
1. **Banco de Dados de Alimentos**
   - Integração com TACO (Tabela Brasileira)
   - Integração com USDA (alimentos internacionais)
   - API de informação nutricional

2. **Apps de Dieta**
   - Exportação de plano para MyFitnessPal
   - Sincronização com apps de contagem de calorias

3. **Balança Inteligente**
   - Integração com balanças bluetooth
   - Importação automática de peso

#### Para Psicólogos
1. **Apps de Bem-Estar**
   - Integração com apps de meditação (Calm, Headspace)
   - Integração com diários emocionais

2. **Testes Psicológicos**
   - Integração com plataformas de testes online
   - Importação de resultados (Beck, WHOQOL, etc.)

#### Para Dentistas
1. **Laboratórios de Prótese**
   - Envio digital de pedidos
   - Rastreamento de status

2. **Fabricantes de Implantes**
   - Catálogo de produtos
   - Cálculo de orçamentos

#### Para Fisioterapeutas
1. **Apps de Exercícios**
   - Biblioteca de vídeos de exercícios
   - Prescrição com QR code

2. **Wearables**
   - Integração com smartwatches (monitoramento de atividade)
   - Importação de dados de movimento

### Investimento
- **Tempo:** 4 meses (1 mês por especialidade)
- **Custo:** R$ 80.000 (2 devs backend)
- **Prioridade:** P2 (Média)

### Entregáveis
- ✅ 10+ integrações específicas
- ✅ Documentação de API
- ✅ SDKs para parceiros
```

---

## 📊 Novo Plano Consolidado

### Estrutura Atualizada

```
Plano_Desenvolvimento/
├── fase-1-conformidade-legal/
│   ├── 01-cfm-1821-finalizacao.md
│   ├── 02-cfm-1638-versionamento.md
│   ├── 03-prescricoes-digitais-finalizacao.md
│   ├── 04-sngpc-integracao.md
│   ├── 05-cfm-2314-telemedicina.md
│   ├── 05b-regulamentacao-multiprofissional.md ⭐ NOVO
│   ├── 06-tiss-fase1-convenios.md
│   └── 07-telemedicina-mvp-finalizacao.md
│
├── fase-2-seguranca-lgpd/
│   ├── 08-auditoria-lgpd.md
│   ├── 09-criptografia-dados.md
│   ├── 10-portal-paciente.md (atualizado)
│   ├── 11-prontuario-soap.md (atualizado)
│   └── 12-melhorias-seguranca.md
│
├── fase-3-multi-negocios/ ⭐ NOVA FASE
│   ├── README.md
│   ├── 24-feature-flags.md
│   ├── 25-terminologia-adaptavel.md
│   ├── 26-templates-especializados.md
│   ├── 27-onboarding-diferenciado.md
│   ├── 28-profissionais-sem-cnpj.md
│   └── 29-modelos-precificacao.md
│
├── fase-4-analytics-otimizacao/
│   ├── 13-tiss-fase2.md
│   ├── 14-fila-espera-avancada.md
│   ├── 15-bi-analytics.md (atualizado)
│   ├── 16-assinatura-digital.md
│   ├── 17-crm-avancado.md
│   ├── 18-gestao-fiscal.md
│   └── 19-acessibilidade-wcag.md
│
└── fase-5-enterprise/
    ├── 20-api-publica.md
    ├── 21-integracao-laboratorios.md
    ├── 21b-integracoes-especializadas.md ⭐ NOVO
    ├── 22-marketplace.md
    └── 23-programa-referral.md
```

---

## 📈 Impacto no Cronograma e Investimento

### Antes (Plano Original)

| Fase | Tarefas | Tempo | Investimento |
|------|---------|-------|--------------|
| Fase 1 | 7 | 12-14 meses | R$ 262.500 |
| Fase 2 | 5 | 9-11 meses | R$ 210.000 |
| Fase 4 | 7 | 16-20 meses | R$ 602.500 |
| Fase 5 | 4 | 9-14 meses | R$ 180.000 |
| **TOTAL** | **23** | **46-59 meses** | **R$ 1.255.000** |

### Depois (Com Adaptações Multi-Negócios)

| Fase | Tarefas | Tempo | Investimento |
|------|---------|-------|--------------|
| Fase 1 | 8 (+1) | 15-17 meses | R$ 322.500 (+R$ 60k) |
| Fase 2 | 5 (atualizadas) | 11-13 meses | R$ 240.000 (+R$ 30k) |
| **Fase 3 (NOVA)** | **6** | **8-10 meses** | **R$ 175.000** |
| Fase 4 | 7 (atualizadas) | 18-22 meses | R$ 617.500 (+R$ 15k) |
| Fase 5 | 5 (+1) | 13-18 meses | R$ 260.000 (+R$ 80k) |
| **TOTAL** | **31 (+8)** | **65-80 meses** | **R$ 1.615.000 (+R$ 360k)** |

---

## 🎯 Priorização Revisada

### Crítico para Multi-Negócios (Fazer Primeiro)

1. **Fase 3: Tarefas 24-29** (Sistema de Adaptabilidade)
   - Sem isso, não conseguimos adaptar para outras especialidades
   - **Prazo:** Q1-Q2 2026
   - **Investimento:** R$ 175.000

2. **Fase 1: Tarefa 05b** (Compliance Multiprofissional)
   - Necessário para legalidade de telemedicina em outras profissões
   - **Prazo:** Q2 2026
   - **Investimento:** R$ 60.000

3. **Fase 2: Atualizações 10 e 11** (Portal e Prontuário)
   - Experiência diferenciada por especialidade
   - **Prazo:** Q3 2026
   - **Investimento:** R$ 30.000

### Médio Prazo (Fazer Depois)

4. **Fase 4: Atualização 15** (BI por Especialidade)
   - **Prazo:** Q4 2026
   - **Investimento:** R$ 15.000

5. **Fase 5: Tarefa 21b** (Integrações Especializadas)
   - **Prazo:** Q1 2027
   - **Investimento:** R$ 80.000

---

## 📋 Checklist de Implementação

### Q1 2026
- [ ] Criar Fase 3 no diretório
- [ ] Escrever prompts detalhados para tarefas 24-29
- [ ] Atualizar README.md principal
- [ ] Atualizar DEPENDENCIES.md

### Q2 2026
- [ ] Iniciar implementação Fase 3
- [ ] Atualizar tarefas da Fase 2
- [ ] Adicionar tarefa 05b na Fase 1

### Q3 2026
- [ ] Completar Fase 3
- [ ] Testar com beta testers de cada especialidade
- [ ] Coletar feedback e iterar

### Q4 2026
- [ ] Atualizar Fase 4 (BI)
- [ ] Começar planejamento Fase 5b

---

## 🔗 Dependências Atualizadas

### Novas Dependências

```
Tarefa 24 (Feature Flags)
└── Depende de: Nenhuma
    └── Bloqueia: 25, 26, 27, 28, 29

Tarefa 25 (Terminologia)
└── Depende de: 24
    └── Bloqueia: 27

Tarefa 26 (Templates)
└── Depende de: 24, 11
    └── Bloqueia: 27

Tarefa 27 (Onboarding)
└── Depende de: 24, 25, 26
    └── Bloqueia: Nenhuma

Tarefa 28 (Sem CNPJ)
└── Depende de: 24
    └── Bloqueia: 29

Tarefa 29 (Precificação)
└── Depende de: 28
    └── Bloqueia: Nenhuma

Tarefa 05b (Compliance Multi)
└── Depende de: 05
    └── Bloqueia: 07

Tarefa 21b (Integrações Esp)
└── Depende de: 20, 26
    └── Bloqueia: Nenhuma
```

---

## 📞 Próximos Passos

1. **Revisão com Stakeholders** (1 semana)
   - Apresentar plano atualizado
   - Validar prioridades
   - Aprovar orçamento adicional (R$ 360k)

2. **Criação de Prompts Detalhados** (2 semanas)
   - Escrever prompts para tarefas 24-29
   - Atualizar prompts existentes (10, 11, 15)
   - Criar prompt para 05b e 21b

3. **Atualização de Documentação** (1 semana)
   - Atualizar README.md
   - Atualizar DEPENDENCIES.md
   - Atualizar EFFORT_ESTIMATES.md

4. **Kickoff da Fase 3** (Início Q1 2026)
   - Alocar equipe
   - Definir sprints
   - Começar implementação

---

## 📚 Documentos Relacionados

- [PLANO_ADAPTACAO_MULTI_NEGOCIOS.md](./PLANO_ADAPTACAO_MULTI_NEGOCIOS.md)
- [ANALISE_MERCADO_SAAS_SAUDE.md](./ANALISE_MERCADO_SAAS_SAUDE.md)
- [TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md](./TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md)
- [GUIA_CONFIGURACAO_TIPOS_NEGOCIO.md](./GUIA_CONFIGURACAO_TIPOS_NEGOCIO.md)

---

> **Versão:** 1.0  
> **Data:** 26 de Janeiro de 2026  
> **Status:** Plano Atualizado - Aguardando Aprovação
