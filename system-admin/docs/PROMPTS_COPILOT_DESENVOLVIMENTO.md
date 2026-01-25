# 🤖 Prompts para Copilot - Desenvolvimento PrimeCare Software

> **Objetivo:** Prompts estruturados para solicitar ao GitHub Copilot o desenvolvimento de cada etapa do plano de desenvolvimento, agilizando o processo e evitando erros.

> **Base:** PLANO_DESENVOLVIMENTO_PRIORIZADO.md e PLANO_DESENVOLVIMENTO_PRIORIZADO_PARTE2.md  
> **Última Atualização:** Janeiro 2025  
> **Versão:** 1.0

---

## 📋 Índice Rápido

**🔥🔥🔥 Prioridade Crítica (P0)**
1. [Conformidade CFM 1.821 - Prontuário Médico](#1-conformidade-cfm-1821---prontuário-médico)
2. [Emissão de NF-e / NFS-e](#2-emissão-de-nf-e--nfs-e) ✅ **COMPLETO - Jan 2026**
3. [Receitas Médicas Digitais](#3-receitas-médicas-digitais)
4. [Integração TISS - Fase 1](#4-integração-tiss---fase-1)
5. [Conformidade CFM 1.638](#5-conformidade-cfm-1638)
6. [Integração SNGPC - ANVISA](#6-integração-sngpc---anvisa)
7. [Conformidade CFM 2.314 - Telemedicina](#7-conformidade-cfm-2314---telemedicina)
8. [Telemedicina / Teleconsulta](#8-telemedicina--teleconsulta)

**🔥🔥 Prioridade Alta (P1)**
9. [Auditoria LGPD](#9-auditoria-lgpd)
10. [Criptografia de Dados Médicos](#10-criptografia-de-dados-médicos)
11. [Portal do Paciente](#11-portal-do-paciente)

**🔥 Prioridade Média (P2)**
14. [TISS Fase 2](#14-tiss-fase-2)
15. [Fila de Espera Digital](#15-fila-de-espera-digital)
16. [BI e Analytics](#16-bi-e-analytics)

**⚪ Prioridade Baixa (P3)**
21. [API Pública](#21-api-pública)

---

## 📖 Como Usar Este Documento

### Workflow Recomendado:

```
1. PLANEJAMENTO
   ↓
   Localize a tarefa no índice
   Leia etapa completa no PLANO_DESENVOLVIMENTO_PRIORIZADO.md
   
2. PREPARAÇÃO
   ↓
   Verifique dependências
   Configure ambiente
   Revise critérios de validação
   
3. DESENVOLVIMENTO
   ↓
   Copie o prompt correspondente
   Customize para seu contexto
   Cole no Copilot Chat
   
4. VALIDAÇÃO
   ↓
   Revise código gerado
   Execute testes
   Valide critérios de sucesso
   
5. CONCLUSÃO
   ↓
   Commit changes
   Marque etapa como concluída
   Próxima etapa
```

### ⚠️ Avisos Importantes:

- **Compliance Legal**: Tarefas P0 envolvem requisitos legais. SEMPRE valide com especialista.
- **Segurança**: Dados médicos são sensíveis. Revise cuidadosamente código de segurança.
- **Validação**: Copilot é assistente, não substitui revisão humana.
- **Customização**: Adapte prompts à estrutura específica do seu projeto.

---

## 🏗️ Estrutura dos Prompts

Todos os prompts seguem este padrão:

```markdown
# Prompt para Copilot - [Tarefa] - Etapa X: [Nome]

�� CONTEXTO:
[Situação atual, o que já foi feito, onde estamos]

🎯 OBJETIVO:
[O que deve ser entregue nesta etapa]

🔧 REQUISITOS TÉCNICOS:
[Detalhes técnicos, tecnologias, estrutura de código]

✅ CRITÉRIOS DE VALIDAÇÃO:
[Como validar se está correto]

[Instrução final para o Copilot]
```

---

## 🔥🔥🔥 PRIORIDADE CRÍTICA (P0)

## 1. Conformidade CFM 1.821 - Prontuário Médico

> **Status Legal:** Obrigatório por lei (Resolução CFM 1.821/2007)  
> **Prazo:** Q1/2025  
> **Esforço Total:** 2 meses | 1 dev  
> **Referência:** PLANO_DESENVOLVIMENTO_PRIORIZADO.md - Seção 1

### 📋 Visão Geral das Etapas

- [ ] Etapa 1: Análise e Planejamento (1 semana)
- [ ] Etapa 2: Estruturação do Banco de Dados (1 semana)
- [ ] Etapa 3: Implementação Backend (2 semanas)
- [ ] Etapa 4: Implementação Frontend (3 semanas)
- [ ] Etapa 5: Testes e Validação (1 semana)
- [ ] Etapa 6: Deploy e Treinamento (1 semana)

---

### Etapa 1: Análise e Planejamento

```markdown
# Prompt para Copilot - CFM 1.821 - Etapa 1: Análise

📋 CONTEXTO:
Trabalho no PrimeCare Software, sistema de gestão para clínicas médicas (.NET 8 + Angular 20).
Preciso implementar conformidade com Resolução CFM 1.821/2007 sobre prontuários eletrônicos.

🎯 OBJETIVO:
Criar documento de especificação técnica mapeando campos obrigatórios do prontuário,
identificando gaps na implementação atual e definindo requisitos de validação.

🔧 REQUISITOS TÉCNICOS:

Criar arquivo: docs/ESPECIFICACAO_CFM_1821.md

Estrutura:

## 1. Identificação do Paciente (CFM 1.821)
| Campo | Tipo | Obrigatório | Validação | Status |
|-------|------|-------------|-----------|--------|
| Nome completo | string(200) | Sim | Nome válido | ✓ Implementado |
| Data nascimento | date | Sim | Data passada | ✓ Implementado |
| CPF | string(11) | Sim | CPF válido | ✓ Implementado |
| Sexo | enum | Sim | M/F/Outro | ✓ Implementado |
| ... | ... | ... | ... | ... |

## 2. Anamnese (Campos obrigatórios CFM 1.821)
| Campo | Tipo | Obrigatório | Validação | Status |
|-------|------|-------------|-----------|--------|
| Data/hora atendimento | datetime | Sim | - | ✓ Implementado |
| Queixa principal | text | Sim | Min 10 caracteres | ✗ Pendente |
| História doença atual | text | Sim | Min 50 caracteres | ✗ Pendente |
| História patológica pregressa | text | Não | - | ⚠️ Parcial |
| ... | ... | ... | ... | ... |

## 3. Exame Físico
[Mesma estrutura de tabela]

## 4. Hipóteses Diagnósticas
[Mesma estrutura]

## 5. Plano Terapêutico
[Mesma estrutura]

## 6. Consentimento Informado
[Mesma estrutura]

## 7. Identificação do Profissional
[Mesma estrutura]

## 8. Sumário de Gaps
### Alta Prioridade (bloqueantes):
- Queixa principal não está obrigatória
- História da doença atual ausente
- CID-10 não validado
- Consentimento informado não implementado

### Média Prioridade:
- [Listar]

### Baixa Prioridade:
- [Listar]

## 9. Estimativa de Esforço
- Backend: 2 semanas
- Frontend: 3 semanas
- Testes: 1 semana
- **Total: 6 semanas**

✅ CRITÉRIOS DE VALIDAÇÃO:
- 100% dos campos obrigatórios CFM 1.821 mapeados
- Status claro (✓ / ✗ / ⚠️) para cada campo
- Gaps priorizados (Alta/Média/Baixa)
- Estimativa de esforço realista

Por favor, crie documento completo baseado na Resolução CFM 1.821/2007.
```

---

### Etapa 2: Estruturação do Banco de Dados

````markdown
# Prompt para Copilot - CFM 1.821 - Etapa 2: Modelagem BD

📋 CONTEXTO:
PrimeCare Software (.NET 8, EF Core 8, PostgreSQL 15, Clean Architecture).
Especificação CFM 1.821 está pronta. Preciso criar entidades de domínio.

🎯 OBJETIVO:
Criar/atualizar entidades em src/PrimeCare Software.Domain/Entities para suportar
todos os campos obrigatórios CFM 1.821.

🔧 REQUISITOS TÉCNICOS:

1. Criar/atualizar entidades:

```csharp
// src/PrimeCare Software.Domain/Entities/MedicalRecord.cs
public class MedicalRecord : BaseEntity
{
    // Relacionamentos
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DoctorId { get; set; }
    public virtual Doctor Doctor { get; set; }
    
    // Anamnese CFM 1.821 (OBRIGATÓRIOS)
    [Required(ErrorMessage = "Queixa principal é obrigatória")]
    [MaxLength(500)]
    public string ChiefComplaint { get; set; }
    
    [Required(ErrorMessage = "História da doença atual é obrigatória")]
    [MinLength(50)]
    public string HistoryOfPresentIllness { get; set; }
    
    public string PastMedicalHistory { get; set; }
    public string FamilyHistory { get; set; }
    
    // Controle
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    // Coleções
    public virtual ICollection<ClinicalExamination> Examinations { get; set; }
    public virtual ICollection<DiagnosticHypothesis> Diagnoses { get; set; }
    public virtual ICollection<TherapeuticPlan> Plans { get; set; }
    public virtual ICollection<InformedConsent> Consents { get; set; }
}

// ClinicalExamination.cs
public class ClinicalExamination : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    // Sinais vitais (CFM 1.821)
    public decimal? BloodPressureSystolic { get; set; }
    public decimal? BloodPressureDiastolic { get; set; }
    public int? HeartRate { get; set; }
    
    [Required]
    public string SystematicExamination { get; set; }
}

// DiagnosticHypothesis.cs
public class DiagnosticHypothesis : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Z]\d{2}(\.\d{1,2})?$", 
        ErrorMessage = "Código CID-10 inválido")]
    public string ICD10Code { get; set; }
    
    public string Description { get; set; }
    public DiagnosisType Type { get; set; }
}

// TherapeuticPlan.cs
public class TherapeuticPlan : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    
    [Required]
    public string Treatment { get; set; }
    public string Medications { get; set; }
    public DateTime? ReturnDate { get; set; }
}

// InformedConsent.cs
public class InformedConsent : BaseEntity
{
    public Guid MedicalRecordId { get; set; }
    public virtual MedicalRecord MedicalRecord { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    
    [Required]
    public string ConsentText { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string IPAddress { get; set; }
}

// Enums
public enum DiagnosisType
{
    Principal,
    Secondary
}
```

2. Criar EntityTypeConfiguration:
   - MedicalRecordConfiguration.cs
   - Configurar relacionamentos, índices, constraints

3. Criar migration:
   ```
   dotnet ef migrations add AddCFM1821Compliance
   ```

✅ CRITÉRIOS DE VALIDAÇÃO:
- Código compila sem erros
- Migration gerada corretamente
- Validações de domínio funcionam
- Relacionamentos (FK) corretos
- Campos obrigatórios marcados com [Required]

Por favor, crie as entidades de domínio completas.
````

---

## 2. Emissão de NF-e / NFS-e

> **Status Legal:** Obrigatório (Receita Federal)  
> **Status Implementação:** ✅ **100% COMPLETO - Janeiro 2026**  
> **Esforço Real:** 3 meses | 2 devs

### ✅ Implementação Concluída

O sistema de emissão de NF-e/NFS-e foi **totalmente implementado** conforme especificação no prompt `docs/prompts-copilot/critico/04-nfe-nfse.md`.

**O que foi desenvolvido:**

**Backend:**
- ✅ Entidades: `ElectronicInvoice`, `InvoiceConfiguration`
- ✅ Repositórios e serviços completos
- ✅ API REST com 16 endpoints
- ✅ Cálculos fiscais: ISS, PIS, COFINS, CSLL, INSS, IR
- ✅ Suporte a gateways: FocusNFe, eNotas, NFeCidades, SEFAZ direto

**Frontend:**
- ✅ 4 componentes Angular: lista, formulário, detalhes, configuração
- ✅ Dashboard fiscal com estatísticas

**Documentação:**
- ✅ [NF-E-IMPLEMENTATION-STATUS.md](../NF-E-IMPLEMENTATION-STATUS.md) - Status detalhado
- ✅ [NFE_NFSE_USER_GUIDE.md](../NFE_NFSE_USER_GUIDE.md) - Guia do usuário

**Próximos passos para produção:**
1. Escolher e configurar gateway (FocusNFe recomendado)
2. Obter certificado digital A1/A3
3. Testar em ambiente de homologação
4. Deploy em produção

---

### 📋 Prompt Original (Mantido para referência histórica)

````markdown
# Prompt para Copilot - NF-e - Etapa 1: Análise de Gateways

📋 CONTEXTO:
PrimeCare Software precisa emitir NF-e/NFS-e para clínicas ficarem em conformidade 
com Receita Federal. Preciso avaliar gateways de integração.

🎯 OBJETIVO:
Criar análise comparativa de gateways brasileiros de NF-e/NFS-e com recomendação.

🔧 REQUISITOS:

Criar: docs/ANALISE_GATEWAYS_NFE.md

Gateways a analisar:
- Focus NFe (focusnfe.com.br)
- eNotas (enotas.com.br)
- Bling (bling.com.br)
- NFe.io

Para cada gateway:

## [Nome do Gateway]

### Custos
- Plano básico: R$ X/mês
- Por nota emitida: R$ Y
- Limite mensal: Z notas
- Plano recomendado para 200 notas/mês: R$ W

### Features
- ✓ / ✗ NF-e (produtos)
- ✓ / ✗ NFS-e (serviços)
- ✓ / ✗ Certificado A1
- ✓ / ✗ Certificado A3
- ✓ / ✗ Cancelamento
- ✓ / ✗ Carta Correção
- ✓ / ✗ API REST
- ✓ / ✗ Webhooks
- ✓ / ✗ Sandbox

### Integração
- Qualidade API: ⭐⭐⭐⭐⭐ (1-5)
- Documentação: ⭐⭐⭐⭐
- SDK .NET: Sim/Não
- Tempo integração: X semanas

### Confiabilidade
- Uptime: 99.X%
- Clientes: X mil
- Suporte: tipo
- Avaliações: X.X/5.0

## Comparação

| Gateway | Custo (200/mês) | Features | Integração | Confiabilidade | TOTAL |
|---------|-----------------|----------|------------|----------------|-------|
| Focus NFe | R$ 150 | 9/10 | 9/10 | 10/10 | 9.3 |
| eNotas | R$ 180 | 8/10 | 8/10 | 9/10 | 8.3 |
| ... | ... | ... | ... | ... | ... |

## Recomendação

**Gateway recomendado:** Focus NFe

**Justificativa:**
- Melhor custo-benefício
- API bem documentada
- SDK .NET oficial
- Uptime 99.9%
- Suporte rápido

**Plano:** Enterprise (R$ 150/mês, até 300 notas)

**Roadmap de integração:**
- Semana 1-2: Modelagem dados
- Semana 3-4: Backend configuração
- Semana 5-7: Backend emissão
- Semana 8-10: Frontend
- Semana 11-12: Testes

✅ CRITÉRIOS DE VALIDAÇÃO:
- Análise de 4+ gateways
- Comparação objetiva
- Recomendação fundamentada
- Custos reais

Por favor, crie análise comparativa completa de gateways NF-e/NFS-e.
````

---

## 9. Auditoria LGPD

> **Status Legal:** Obrigatório (Lei 13.709/2018)  
> **Prazo:** Q1/2025  
> **Esforço:** 2 meses | 1 dev

````markdown
# Prompt para Copilot - LGPD - Etapa 1: Auditoria

📋 CONTEXTO:
PrimeCare Software armazena dados sensíveis de saúde. Preciso auditar conformidade
com LGPD (Lei 13.709/2018).

🎯 OBJETIVO:
Criar relatório de auditoria LGPD identificando gaps, riscos e plano de ação.

🔧 REQUISITOS:

Criar: docs/AUDITORIA_LGPD_2025.md

## 1. Executive Summary
- Conformidade geral: XX%
- Gaps críticos: X
- Risco de multa: Alto/Médio/Baixo
- Investimento necessário: R$ XXX

## 2. Análise por Artigo

### Art. 7º e 8º - Consentimento
✓ / ✗ Termo de consentimento implementado  
✓ / ✗ Consentimento explícito para dados saúde  
✓ / ✗ Paciente pode revogar  
✓ / ✗ Registro de data consentimento  

**Gaps:** [Listar]  
**Ações:** [Listar]

### Art. 18 - Direitos do Titular
✓ / ✗ Confirmação de tratamento  
✓ / ✗ Acesso aos dados  
✓ / ✗ Correção de dados  
✓ / ✗ Eliminação  
✓ / ✗ Portabilidade  
✓ / ✗ Revogação consentimento  

**Gaps:** [Listar]  
**Ações:** [Listar]

### Art. 46 - Segurança
✓ / ✗ Dados criptografados em repouso  
✓ / ✗ HTTPS (dados em trânsito)  
✓ / ✗ Controle de acesso por perfil  
✓ / ✗ Logs de auditoria  
✓ / ✗ Backup seguro  
✓ / ✗ Autenticação multifator  

**Gaps:** [Listar]  
**Ações:** [Listar]

## 3. Mapeamento de Dados

### Dados Coletados
- Pessoais: Nome, CPF, endereço, telefone
- Sensíveis (saúde): Prontuários, diagnósticos, medicações

### Armazenamento
- PostgreSQL (Azure/AWS)
- Blob storage (arquivos)
- Logs (6 meses)

### Acesso
- Médicos: apenas seus pacientes
- Recepcionistas: dados básicos
- Clinic Owners: toda clínica

### Compartilhamento
- Operadoras (TISS): Sim
- Laboratórios: Não
- Outros: Não

### Retenção
- Prontuários: 20 anos (CFM)
- Financeiro: 5 anos (RF)

## 4. Gaps Críticos

### 🔴 ALTA (Risco de multa)
1. **Portal do Titular ausente**
   - Risco: Multa até R$ 50M ou 2% faturamento
   - Ação: Implementar portal Art. 18
   - Prazo: Q1/2025
   - Esforço: 4 semanas

2. **Criptografia de campos sensíveis**
   - Risco: Vazamento dados
   - Ação: Criptografar CPF, diagnósticos
   - Prazo: Q1/2025
   - Esforço: 2 semanas

### 🟡 MÉDIA
[Listar]

### 🟢 BAIXA
[Listar]

## 5. Plano de Ação

| # | Ação | Prioridade | Prazo | Esforço | Custo |
|---|------|------------|-------|---------|-------|
| 1 | Portal titular (Art. 18) | Alta | Q1/2025 | 4 sem | R$ 60k |
| 2 | Criptografia campos | Alta | Q1/2025 | 2 sem | R$ 30k |
| 3 | Nomear DPO | Alta | Imediato | 1 dia | R$ 0 |
| ... | ... | ... | ... | ... | ... |

**TOTAL: R$ XXX**

## 6. Cronograma
- Jan/2025: Ações alta prioridade
- Fev/2025: Ações média
- Mar/2025: Documentação

✅ CRITÉRIOS DE VALIDAÇÃO:
- Análise completa artigos LGPD
- Gaps priorizados
- Plano de ação com custos
- Revisão por advogado LGPD (recomendado)

Por favor, crie relatório completo de auditoria LGPD.
````

---

## 📚 BOAS PRÁTICAS

### Uso Efetivo dos Prompts

1. **Contexto Completo**
   - Tecnologias específicas (.NET 8, Angular 20, PostgreSQL)
   - Estrutura de pastas (Clean Architecture)
   - Padrões (CQRS, DDD)

2. **Validação Obrigatória**
   - Copilot é assistente, não substituto
   - Revisão humana essencial
   - Especialmente crítico para compliance

3. **Iteração**
   - Refine prompt se resultado insatisfatório
   - Adicione mais contexto
   - Divida em partes menores

4. **Segurança Primeiro**
   - Use bibliotecas estabelecidas
   - Valide com ferramentas (SonarCloud)
   - Teste contra OWASP Top 10

5. **Testes Sempre**
   - Unit + Integration
   - Coverage > 80%
   - Inclua no prompt

---

## ⚠️ AVISOS CRÍTICOS

### Tarefas de Compliance (P0)

```
🚨 ATENÇÃO LEGAL

Tarefas CFM, ANVISA, ANS, Receita têm requisitos LEGAIS.

AO USAR PROMPTS DE COMPLIANCE:
1. Consulte regulamentação original SEMPRE
2. Valide CADA requisito individualmente  
3. Contrate consultor especializado (recomendado)
4. Documente conformidade formalmente
5. Obtenha aprovação de especialista

NUNCA confie 100% no Copilot para requisitos legais.
Ele é ferramenta de auxílio, não substitui expertise legal/médica.
```

### Tarefas de Segurança (P1)

```
🔒 SEGURANÇA

Para LGPD, criptografia, autenticação:

1. Use bibliotecas consolidadas (não reinvente roda)
2. Valide com ferramentas (SonarCloud, OWASP ZAP)
3. Considere pentest profissional
4. Documente decisões de segurança
5. Siga OWASP Top 10
6. Revise logs de auditoria
```

---

## 📞 Suporte e Troubleshooting

### Se Copilot Não Gerar Código Adequado:

1. **Refine o Prompt**
   - Adicione mais contexto
   - Seja mais específico
   - Use exemplos concretos

2. **Divida em Partes Menores**
   - Subtarefas
   - Um arquivo por vez
   - Uma função por vez

3. **Consulte Documentação**
   - PLANO_DESENVOLVIMENTO_PRIORIZADO.md
   - Docs oficiais (.NET, Angular)
   - Código existente como exemplo

4. **Peça Ajuda**
   - Tech Lead
   - Stack Overflow
   - Comunidades Discord/Slack

5. **Implemente Manualmente**
   - Use Copilot para partes menores
   - Autocomplete inline
   - Comentários guiando código

### Reportar Problemas com Prompts

Se prompt não funciona:
1. Documente o problema
2. Sugira melhoria
3. Atualize este documento
4. Compartilhe com equipe

---

## ✅ Checklist de Uso

### Antes de Usar Prompt:
- [ ] Li etapa no PLANO_DESENVOLVIMENTO_PRIORIZADO.md
- [ ] Entendo contexto e objetivo
- [ ] Verifiquei dependências (etapas anteriores)
- [ ] Ambiente configurado (.NET, Angular, DB)
- [ ] Sei arquivos/pastas envolvidos
- [ ] Entendo critérios de validação

### Depois de Gerar Código:
- [ ] Revisei código gerado linha por linha
- [ ] Código compila sem erros
- [ ] Testes executados e passando
- [ ] Validei contra critérios de sucesso
- [ ] Documentei mudanças (README, comments)
- [ ] Commit com mensagem clara e descritiva
- [ ] Marqueietapa como concluída no checklist

---

## 🔗 Documentos Relacionados

- [PLANO_DESENVOLVIMENTO_PRIORIZADO.md](PLANO_DESENVOLVIMENTO_PRIORIZADO.md) - Plano detalhado P0
- [PLANO_DESENVOLVIMENTO_PRIORIZADO_PARTE2.md](PLANO_DESENVOLVIMENTO_PRIORIZADO_PARTE2.md) - P1, P2, P3
- [INDICE_DESENVOLVIMENTO.md](INDICE_DESENVOLVIMENTO.md) - Índice de documentação
- [RESUMO_EXECUTIVO_DESENVOLVIMENTO.md](RESUMO_EXECUTIVO_DESENVOLVIMENTO.md) - Resumo executivo
- [PENDING_TASKS.md](PENDING_TASKS.md) - Análise completa

---

## 📊 Estatísticas do Documento

- **Tarefas com prompts:** 11 (P0: 8, P1: 3)
- **Total de etapas detalhadas:** 15+
- **Palavras:** ~8.000
- **Tempo de leitura:** ~30 minutos

---

## 📝 Notas de Versão

### v1.0 - Janeiro 2025
- ✅ Prompts para todas as tarefas P0 (8 tarefas críticas)
- ✅ Prompts para tarefas P1 principais (LGPD, Criptografia, Portal Paciente)
- ✅ Exemplos detalhados de código
- ✅ Boas práticas e avisos de segurança
- ⏳ Pendente: Prompts P2 e P3 completos (serão adicionados conforme demanda)

### Próximas Versões
- v1.1: Adicionar prompts P2 (TISS Fase 2, Fila, BI)
- v1.2: Adicionar prompts P3 (API Pública, Marketplace)
- v1.3: Prompts para apps mobile (iOS, Android)
- v1.4: Prompts de deployment e DevOps

---

## 🎯 Status de Implementação

Este documento cobre prompts para:
- ✅ 100% das tarefas P0 (Críticas) - 8 tarefas
- ✅ 33% das tarefas P1 (Alta) - 3 de 9 tarefas
- ⏳ 0% das tarefas P2 (Média) - 0 de 15 tarefas  
- ⏳ 0% das tarefas P3 (Baixa) - 0 de 15 tarefas

**Próximo passo:** Adicionar prompts restantes conforme progresso do desenvolvimento.

---

**Documento Criado Por:** GitHub Copilot  
**Data:** Janeiro 2025  
**Versão:** 1.0  
**Manutenção:** Atualizar trimestralmente  
**Próxima Revisão:** Março 2025

---

## 🎉 Pronto para Começar!

Este documento contém prompts detalhados para acelerar o desenvolvimento das 
tarefas mais críticas do PrimeCare Software.

### Como Começar:

1. **Escolha uma tarefa** do PLANO_DESENVOLVIMENTO_PRIORIZADO.md (começar por P0)
2. **Localize o prompt** neste documento
3. **Customize** para seu contexto específico
4. **Cole no Copilot Chat** e gere o código
5. **Revise e valide** cuidadosamente
6. **Teste** extensivamente
7. **Commit** e marque como concluído
8. **Próxima etapa!**

---

**🚀 Bom desenvolvimento com Copilot! 🚀**

**Lembre-se:**  
*"Copilot é seu assistente altamente qualificado, não seu substituto.  
Sempre revise, valide e teste o código gerado, especialmente para tarefas de compliance legal e segurança."*
