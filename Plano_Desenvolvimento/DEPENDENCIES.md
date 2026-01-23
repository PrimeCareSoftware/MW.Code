# 🔗 Matriz de Dependências - Plano de Desenvolvimento

> **Última Atualização:** 23 de Janeiro de 2026  
> **Total de Prompts:** 24

---

## 📊 Legenda

- ✅ **Pré-requisito Obrigatório:** Deve ser completado antes
- 🔄 **Dependência Parcial:** Pode começar em paralelo, mas precisa de integração
- ⚡ **Opcional:** Melhora se tiver, mas não é bloqueante
- 🆓 **Independente:** Pode ser executado sem dependências

---

## 🗺️ Grafo de Dependências

```
FASE 1 (P0 - Crítico)
├─ 01. CFM 1.821 → [02, 03, 16] 🆓
├─ 02. CFM 1.638 (Versionamento) → [11] ← 01 ✅
├─ 03. Prescrições Digitais → [04, 16] ← 01 ✅
├─ 04. SNGPC → [] ← 03 ✅
├─ 05. CFM 2.314 (Telemedicina) → [07] 🆓
├─ 06. TISS Fase 1 → [13] 🆓
└─ 07. Telemedicina MVP → Produção ← 05 ✅

FASE 2 (P1 - Alta)
├─ 08. Auditoria LGPD → [09, 12] 🆓
├─ 09. Criptografia → [] ← 08 ⚡
├─ 10. Portal Paciente → [] 🆓
├─ 11. Prontuário SOAP → [] ← 02 🔄
└─ 12. Melhorias Segurança → [] ← 08 ⚡

FASE 4 (P2 - Médio)
├─ 13. TISS Fase 2 → [] ← 06 ✅
├─ 14. Fila Espera → [] 🆓
├─ 15. BI/Analytics → [17] 🆓
├─ 16. Assinatura Digital → [] ← 01, 03 🔄
├─ 17. CRM Avançado → [] ← 15 ⚡
├─ 18. Gestão Fiscal → [] 🆓
└─ 19. Acessibilidade → [] 🆓

FASE 5 (P3 - Baixa)
├─ 20. API Pública → [21, 22] 🆓
├─ 21. Integração Labs → [] ← 20 ⚡
├─ 22. Marketplace → [] ← 20 ⚡
└─ 23. Programa Referral → [] 🆓
```

---

## 📋 Detalhamento por Prompt

### Fase 1: Conformidade Legal

#### 01. CFM 1.821 - Finalização
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:**
  - 02. CFM 1.638 (precisa do prontuário base)
  - 03. Prescrições Digitais (precisa do prontuário)
  - 16. Assinatura Digital (documentos a assinar)

#### 02. CFM 1.638 - Versionamento
- **Depende de:** 01. CFM 1.821 (✅ Obrigatório)
- **Bloqueante para:**
  - 11. Prontuário SOAP (sistema de versionamento necessário)

#### 03. Prescrições Digitais - Finalização
- **Depende de:** 01. CFM 1.821 (✅ Obrigatório)
- **Bloqueante para:**
  - 04. SNGPC (precisa das prescrições)
  - 16. Assinatura Digital (prescrições a assinar)

#### 04. SNGPC - Integração ANVISA
- **Depende de:** 03. Prescrições Digitais (✅ Obrigatório)
- **Bloqueante para:** Nenhum

#### 05. CFM 2.314 - Telemedicina Compliance
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:**
  - 07. Telemedicina MVP (compliance necessária)

#### 06. TISS Fase 1 - Convênios
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:**
  - 13. TISS Fase 2 (precisa da base Fase 1)

#### 07. Telemedicina MVP → Produção
- **Depende de:** 05. CFM 2.314 (✅ Obrigatório)
- **Bloqueante para:** Nenhum

---

### Fase 2: Segurança e LGPD

#### 08. Auditoria LGPD
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:**
  - 09. Criptografia (⚡ opcional - auditoria identifica necessidades)
  - 12. Melhorias Segurança (⚡ opcional - prioriza itens)

#### 09. Criptografia de Dados
- **Depende de:** 08. Auditoria LGPD (⚡ opcional)
- **Bloqueante para:** Nenhum

#### 10. Portal do Paciente
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:** Nenhum

#### 11. Prontuário SOAP
- **Depende de:** 02. CFM 1.638 (🔄 parcial - versionamento integrado)
- **Bloqueante para:** Nenhum

#### 12. Melhorias de Segurança
- **Depende de:** 08. Auditoria LGPD (⚡ opcional)
- **Bloqueante para:** Nenhum

---

### Fase 4: Analytics e Otimização

#### 13. TISS Fase 2
- **Depende de:** 06. TISS Fase 1 (✅ Obrigatório)
- **Bloqueante para:** Nenhum

#### 14. Fila de Espera Avançada
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:** Nenhum

#### 15. BI e Analytics
- **Depende de:** Nenhum (🆓 Independente - usa dados históricos)
- **Bloqueante para:**
  - 17. CRM Avançado (⚡ opcional - analytics alimenta CRM)

#### 16. Assinatura Digital
- **Depende de:**
  - 01. CFM 1.821 (🔄 parcial - documentos a assinar)
  - 03. Prescrições Digitais (🔄 parcial - prescrições a assinar)
- **Bloqueante para:** Nenhum

#### 17. CRM Avançado
- **Depende de:** 15. BI/Analytics (⚡ opcional - usa dados)
- **Bloqueante para:** Nenhum

#### 18. Gestão Fiscal
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:** Nenhum

#### 19. Acessibilidade WCAG
- **Depende de:** Nenhum (🆓 Independente - refactoring frontend)
- **Bloqueante para:** Nenhum

---

### Fase 5: Enterprise Features

#### 20. API Pública
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:**
  - 21. Integração Labs (⚡ opcional - usa API)
  - 22. Marketplace (⚡ opcional - plugins usam API)

#### 21. Integração Laboratórios
- **Depende de:** 20. API Pública (⚡ opcional)
- **Bloqueante para:** Nenhum

#### 22. Marketplace
- **Depende de:** 20. API Pública (⚡ opcional)
- **Bloqueante para:** Nenhum

#### 23. Programa Referral
- **Depende de:** Nenhum (🆓 Independente)
- **Bloqueante para:** Nenhum

---

## 🚀 Caminhos Críticos

### Caminho 1: Conformidade Legal (Mais Urgente)
```
01. CFM 1.821 (1 mês)
  → 02. CFM 1.638 (1.5 meses)
    → 11. SOAP (1-2 meses)
  → 03. Prescrições (2 meses)
    → 04. SNGPC (2 meses)
    → 16. Assinatura Digital (2-3 meses)

Total: ~9.5-11.5 meses
```

### Caminho 2: Telemedicina
```
05. CFM 2.314 (1 mês)
  → 07. Telemedicina Produção (1-2 meses)

Total: 2-3 meses
```

### Caminho 3: Convênios
```
06. TISS Fase 1 (3 meses)
  → 13. TISS Fase 2 (3 meses)

Total: 6 meses
```

### Caminho 4: Analytics e CRM
```
15. BI/Analytics (3-4 meses)
  → 17. CRM Avançado (3-4 meses)

Total: 6-8 meses
```

### Caminho 5: API e Integrações
```
20. API Pública (1-2 meses)
  → 21. Labs (4-6 meses)
  → 22. Marketplace (3-4 meses)

Total: 8-12 meses
```

---

## 📊 Grupos Paralelos Recomendados

### Grupo A: Compliance Crítico (Equipe 1)
- 01. CFM 1.821
- 02. CFM 1.638
- 03. Prescrições
- 04. SNGPC
- 05. CFM 2.314
- 07. Telemedicina

**Tempo:** ~6-8 meses (com sobreposição)

### Grupo B: Segurança (Equipe 2)
- 08. LGPD
- 09. Criptografia
- 12. Melhorias Segurança
- 16. Assinatura Digital
- 19. Acessibilidade

**Tempo:** ~6-8 meses (com sobreposição)

### Grupo C: Experiência (Equipe 3)
- 10. Portal Paciente
- 11. SOAP
- 14. Fila Espera

**Tempo:** ~4-6 meses (com sobreposição)

### Grupo D: Analytics e Otimização (Equipe 4)
- 06. TISS Fase 1
- 13. TISS Fase 2
- 15. BI/Analytics
- 17. CRM
- 18. Gestão Fiscal

**Tempo:** ~8-10 meses (com sobreposição)

### Grupo E: Enterprise (Equipe 5)
- 20. API Pública
- 21. Labs
- 22. Marketplace
- 23. Referral

**Tempo:** ~8-12 meses (com sobreposição)

---

## ⚠️ Riscos de Dependência

### Risco Alto
- **TISS Fase 2 sem Fase 1:** Impossível integrar webservices sem estrutura base
- **SNGPC sem Prescrições:** Não há dados para transmitir

### Risco Médio
- **Assinatura Digital prematura:** Pode assinar documentos que ainda vão mudar
- **CRM sem Analytics:** Perde insights de dados históricos

### Risco Baixo
- **Labs sem API:** Pode usar integração direta, mas API facilita
- **Marketplace sem API:** Plugins precisariam de acesso diferente

---

## 💡 Recomendações

1. **Priorize Grupo A (Compliance)** - Risco legal alto
2. **Paralelise Grupos B e C** - Ganho de tempo
3. **Grupo D após dados acumulados** - Analytics precisa de histórico
4. **Grupo E pode ser postergado** - Menor urgência
5. **Mantenha 1 desenvolvedor de integração** - Evitar silos entre grupos
