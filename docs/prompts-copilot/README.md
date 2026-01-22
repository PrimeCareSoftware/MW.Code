# 🤖 Prompts para Desenvolvimento com GitHub Copilot

Este diretório contém prompts estruturados para desenvolvimento com GitHub Copilot, organizados por prioridade e categoria conforme o documento [PENDING_TASKS.md](../PENDING_TASKS.md).

> **✅ STATUS: 100% COMPLETO** - Todos os 18 prompts criados e documentados (Janeiro 2026)

## 📋 Índice de Prompts por Prioridade

### 🔥🔥🔥 Prioridade Crítica (4 prompts)

1. [Telemedicina / Teleconsulta](./critico/01-telemedicina.md) - ✅ **100% COMPLETO** (Backend + Frontend)
2. [Portal do Paciente](./critico/02-portal-paciente.md) - ✅ **100% COMPLETO** (API + Angular 20)
3. [Integração TISS / Convênios](./critico/03-integracao-tiss.md) - ⚠️ **97% COMPLETO** (Analytics implementados)
4. [Emissão NF-e/NFS-e](./critico/04-nfe-nfse.md) - ✅ **100% COMPLETO** (Janeiro 2026)

### 🔥🔥 Prioridade Alta (4 prompts)

5. [Criptografia de Dados Médicos](./alta/05-criptografia-dados.md) - ❌ Não iniciado
6. [Prontuário SOAP Estruturado](./alta/06-prontuario-soap.md) - ❌ Não iniciado
7. [Auditoria Completa (LGPD)](./alta/07-auditoria-lgpd.md) - ✅ **100% COMPLETO** (Janeiro 2026)
8. [Melhorias de Segurança Diversas](./alta/08-melhorias-seguranca.md) - ⚠️ 30% (Parcial)

### 🔥 Prioridade Média (5 prompts)

9. [Assinatura Digital ICP-Brasil](./media/09-assinatura-digital.md) - ❌ Não iniciado
10. [Sistema de Fila de Espera](./media/10-fila-espera.md) - ❌ Não iniciado
11. [Anamnese Guiada por Especialidade](./media/11-anamnese-especialidade.md) - ❌ Não iniciado
12. [IP Blocking e Geo-blocking](./media/12-ip-geoblocking.md) - ❌ Não iniciado
14. [BI e Analytics Avançados](./media/14-bi-analytics.md) - ⚠️ Parcial (Dashboard implementado)

### ⚪ Prioridade Baixa (5 prompts)

13. [API Pública para Integrações](./baixo/13-api-publica.md) - ❌ Não iniciado
15. [Integração com Laboratórios](./baixo/15-integracao-laboratorios.md) - ❌ Não iniciado
16. [Benchmarking Anônimo](./baixo/16-benchmarking.md) - ❌ Não iniciado
17. [Marketplace Público](./baixo/17-marketplace.md) - ❌ Não iniciado
18. [Programa de Indicação e Fidelidade](./baixo/18-programa-fidelidade.md) - ❌ Não iniciado

### 📊 Por Categoria Regulatória

#### CFM (Conselho Federal de Medicina)
- [CFM 1.821/2007 - Prontuário Médico](./regulatorio/cfm-1821-prontuario.md)
- [CFM 2.314/2022 - Telemedicina](./regulatorio/cfm-2314-telemedicina.md)
- [CFM 1.638/2002 - Prontuário Eletrônico](./regulatorio/cfm-1638-eletronico.md)
- [CFM 1.643/2002 - Receita Médica Digital](./regulatorio/cfm-1643-receita.md)

#### ANVISA
- [SNGPC - Sistema Nacional de Produtos Controlados](./regulatorio/anvisa-sngpc.md)
- [RDC 44/2009 - Boas Práticas Farmacêuticas](./regulatorio/anvisa-rdc44.md)
- [Notificação de Eventos Adversos](./regulatorio/anvisa-eventos-adversos.md)

#### ANS (Agência Nacional de Saúde Suplementar)
- [TISS Fase 1 - Base Funcional](./regulatorio/ans-tiss-fase1.md)
- [TISS Fase 2 - Completo](./regulatorio/ans-tiss-fase2.md)
- [Registro de Operadoras (RPS)](./regulatorio/ans-registro-operadoras.md)

#### Receita Federal
- [Emissão NF-e/NFS-e](./regulatorio/receita-nfe.md)
- [Controle de Faturamento e Impostos](./regulatorio/receita-impostos.md)
- [Integração Contábil](./regulatorio/receita-contabil.md)
- [eSocial e Folha de Pagamento](./regulatorio/receita-esocial.md)

## 📝 Como Usar os Prompts

### 1. Selecione o Prompt Adequado
Escolha o arquivo de prompt correspondente à tarefa que deseja desenvolver.

### 2. Copie o Prompt
Abra o arquivo markdown e copie o prompt completo.

### 3. Use com GitHub Copilot
Cole o prompt no GitHub Copilot Chat ou use como comentário no código para gerar a implementação.

### 4. Adapte Conforme Necessário
Os prompts são templates base. Adapte conforme o contexto específico do seu desenvolvimento.

## 🎯 Estrutura dos Prompts

Cada prompt contém:

1. **Contexto**: Descrição da funcionalidade e seu propósito
2. **Requisitos**: Lista completa de requisitos funcionais e não-funcionais
3. **Arquitetura**: Estrutura de camadas (Domain, Application, Infrastructure, API)
4. **Tecnologias**: Stack tecnológico específico
5. **Checklist de Implementação**: Passo a passo detalhado
6. **Testes**: Estratégia de testes e validação
7. **Referências**: Links para documentação relevante

## 📊 Status de Implementação

| Categoria | Total | Completo | Em Progresso | Não Iniciado |
|-----------|-------|----------|--------------|--------------|
| **Crítico (🔥🔥🔥)** | 4 | 3 | 1 | 0 |
| **Alto (🔥🔥)** | 4 | 1 | 1 | 2 |
| **Médio (🔥)** | 5 | 0 | 1 | 4 |
| **Baixo (⚪)** | 5 | 0 | 0 | 5 |
| **TOTAL** | **18** | **4** | **3** | **11** |

### 🎯 Progresso Global: 22% Completo | 17% Em Progresso | 61% Pendente

**Prompts Criados**: ✅ 18/18 (100%)  
**Features Implementadas**: ⚠️ 4/18 (22%)  
**Investimento Total Planejado**: R$ 970.000 (2025-2027)

## 🗺️ Roadmap de Desenvolvimento

### Q1 2025 - Foundation & Compliance
- Conformidade CFM Completa
- Auditoria LGPD
- Criptografia de Dados
- Prontuário SOAP
- Melhorias de Segurança

### Q2 2025 - Fiscal & Patient Experience
- Emissão NF-e/NFS-e
- Receitas Médicas Digitais Completas
- SNGPC ANVISA
- Portal do Paciente Melhorias

### Q3 2025 - Telemedicina & CRM
- Telemedicina Compliance Completo
- CRM - Jornada do Paciente
- Automação de Marketing
- Pesquisas NPS

### Q4 2025 - TISS & Analytics
- TISS Fase 1 Completo
- TISS Fase 2 Início
- BI e Analytics Avançados

### 2026 - Expansão & Integrações
- Assinatura Digital ICP-Brasil
- API Pública
- Integrações com Laboratórios
- Marketplace

## 📚 Documentação de Referência

- [PENDING_TASKS.md](../PENDING_TASKS.md) - Documento consolidado de pendências
- [ANALISE_MELHORIAS_SISTEMA.md](../ANALISE_MELHORIAS_SISTEMA.md) - Análise completa do sistema
- [SUGESTOES_MELHORIAS_SEGURANCA.md](../SUGESTOES_MELHORIAS_SEGURANCA.md) - Melhorias de segurança
- [FUNCIONALIDADES_IMPLEMENTADAS.md](../FUNCIONALIDADES_IMPLEMENTADAS.md) - Status atual

## 💡 Dicas de Uso

1. **Comece pelo Crítico**: Priorize tarefas marcadas como 🔥🔥🔥
2. **Valide Requisitos**: Revise os requisitos antes de iniciar
3. **Siga a Arquitetura**: Mantenha a consistência com o padrão DDD existente
4. **Teste Continuamente**: Implemente testes à medida que desenvolve
5. **Documente**: Atualize a documentação após cada implementação

## 🔄 Atualizações

Este diretório será atualizado conforme:
- Novas tarefas forem adicionadas ao PENDING_TASKS.md
- Tarefas existentes forem concluídas
- Requisitos forem refinados
- Feedback do time for incorporado

**Última Atualização**: Janeiro 2026
**Versão**: 1.0
