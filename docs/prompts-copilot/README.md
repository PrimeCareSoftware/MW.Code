# 🤖 Prompts para Desenvolvimento com GitHub Copilot

Este diretório contém prompts estruturados para desenvolvimento com GitHub Copilot, organizados por prioridade e categoria conforme o documento [PENDING_TASKS.md](../PENDING_TASKS.md).

## 📋 Índice de Prompts por Prioridade

### 🔥🔥🔥 Prioridade Crítica

1. [Telemedicina / Teleconsulta](./critico/01-telemedicina.md) - **80% completo, falta compliance CFM**
2. [Portal do Paciente](./critico/02-portal-paciente.md) - **✅ 100% COMPLETO**
3. [Integração TISS / Convênios](./critico/03-integracao-tiss.md) - **70% completo, falta Fase 2**
4. [Emissão NF-e/NFS-e](./critico/04-nfe-nfse.md) - **❌ Não iniciado**
5. [Conformidade CFM Completa](./critico/05-conformidade-cfm.md) - **95% completo**

### 🔥🔥 Prioridade Alta

6. [Prontuário SOAP Estruturado](./alta/06-prontuario-soap.md)
7. [Auditoria Completa (LGPD)](./alta/07-auditoria-lgpd.md)
8. [Criptografia de Dados Médicos](./alta/08-criptografia-dados.md)
9. [Receitas Médicas Digitais](./alta/09-receitas-digitais.md) - **90% completo**
10. [SNGPC (ANVISA)](./alta/10-sngpc-anvisa.md) - **85% completo**
11. [Melhorias de Segurança](./alta/11-melhorias-seguranca.md)

### 🔥 Prioridade Média

12. [Assinatura Digital ICP-Brasil](./media/12-assinatura-digital.md)
13. [Sistema de Fila de Espera](./media/13-fila-espera.md)
14. [BI e Analytics Avançados](./media/14-bi-analytics.md)
15. [Anamnese Guiada por Especialidade](./media/15-anamnese-guiada.md)
16. [CRM - Jornada do Paciente](./media/16-crm-jornada.md)
17. [Automação de Marketing](./media/17-automacao-marketing.md)
18. [Pesquisas de Satisfação (NPS)](./media/18-pesquisas-nps.md)

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
| **Crítico (🔥🔥🔥)** | 5 | 1 | 3 | 1 |
| **Alto (🔥🔥)** | 6 | 2 | 0 | 4 |
| **Médio (🔥)** | 6 | 0 | 0 | 6 |
| **Total** | 17 | 3 | 3 | 11 |

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
