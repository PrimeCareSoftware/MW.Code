# 👨‍💼 Guia do Usuário - System Admin

## Bem-vindo ao Sistema de Módulos

Este guia ensina como gerenciar módulos do Omni Care como administrador do sistema.

Como **System Admin**, você tem controle total sobre:
- 📊 Visualização de métricas de uso de módulos
- ⚙️ Configuração de módulos por plano de assinatura
- 🌐 Ações globais (habilitar/desabilitar para todas as clínicas)
- 📈 Analytics e relatórios de adoção

---

## 📊 Dashboard de Módulos

### Acessar o Dashboard

1. Faça login como **System Admin**
2. No menu lateral, clique em **"Módulos"**
3. Você verá o dashboard com métricas de uso

![Dashboard de Módulos](./screenshots/modules-dashboard.png)

### Entendendo as Métricas

#### KPIs Principais

**Total de Módulos**
- Quantidade total de módulos disponíveis no sistema
- Atualmente: 13 módulos

**Taxa Média de Adoção**
- Percentual médio de clínicas usando cada módulo
- Calculado como: (Clínicas usando módulo / Total de clínicas) × 100
- Exemplo: Se 75 de 100 clínicas usam "Gestão de Pacientes", a taxa é 75%

**Mais Usado**
- Módulo com maior taxa de adoção
- Geralmente módulos CORE como "PatientManagement"

**Menos Usado**
- Módulo com menor taxa de adoção
- Pode indicar necessidade de divulgação ou treinamento

#### Tabela de Uso

A tabela principal mostra todos os módulos com:

| Coluna | Descrição |
|--------|-----------|
| **Módulo** | Nome e descrição do módulo |
| **Categoria** | Core, Advanced, Premium ou Analytics |
| **Clínicas Usando** | Número de clínicas com módulo habilitado |
| **Taxa de Adoção** | Percentual de uso |
| **Ações** | Botões para visualizar detalhes ou configurar |

**Filtros Disponíveis:**
- 🔍 Busca por nome do módulo
- 🏷️ Filtro por categoria
- 📊 Ordenação por nome, taxa de adoção ou número de clínicas

### Categorias de Módulos

#### 🌟 Core (Essenciais)
Módulos fundamentais que não podem ser desabilitados.

**Módulos Core:**
- **PatientManagement** - Gestão de Pacientes
- **AppointmentScheduling** - Agendamento de Consultas
- **MedicalRecords** - Prontuários Médicos
- **Prescriptions** - Prescrições
- **UserManagement** - Gestão de Usuários

**Características:**
- Disponíveis em todos os planos
- Não podem ser desabilitados
- Essenciais para operação básica do sistema

#### 🔧 Advanced (Avançados)
Funcionalidades avançadas para operação otimizada.

**Módulos Advanced:**
- **FinancialManagement** - Gestão Financeira
- **WaitingQueue** - Fila de Espera
- **InventoryManagement** - Gestão de Estoque
- **DoctorFieldsConfig** - Configuração de Campos do Médico

**Características:**
- Disponíveis em planos Standard e superiores
- Podem ser desabilitados se não utilizados
- Melhoram eficiência operacional

#### 💎 Premium
Recursos premium para diferenciação competitiva.

**Módulos Premium:**
- **WhatsAppIntegration** - Integração WhatsApp
- **SMSNotifications** - Notificações SMS
- **TissExport** - Exportação TISS

**Características:**
- Disponíveis apenas em planos Premium e Enterprise
- Podem ter custos adicionais (ex: SMS)
- Aumentam satisfação do paciente

#### 📊 Analytics
Análises e relatórios para tomada de decisão.

**Módulos Analytics:**
- **Reports** - Relatórios Avançados

**Características:**
- Disponíveis em planos Standard e superiores
- Fornecem insights de negócio
- Suportam exportação em múltiplos formatos

---

## 📋 Configurar Módulos por Plano

### Acesso

1. No menu, clique em **"Módulos"**
2. Clique na aba **"Módulos por Plano"**
3. Selecione um plano no dropdown

![Configuração de Planos](./screenshots/plan-modules.png)

### Tipos de Planos

| Plano | Preço | Módulos Típicos |
|-------|-------|----------------|
| **Basic** | R$ 99/mês | Core apenas |
| **Standard** | R$ 199/mês | Basic + Reports + TISS |
| **Premium** | R$ 299/mês | Standard + WhatsApp + SMS |
| **Enterprise** | Sob consulta | Todos os módulos + customizações |

### Habilitar/Desabilitar Módulos no Plano

#### Passo a Passo

1. **Selecione o plano** que deseja configurar no dropdown
2. Você verá a lista de todos os módulos
3. **Marque/desmarque** os checkboxes dos módulos
4. Clique em **"Salvar Configurações"**
5. Aguarde a confirmação de sucesso

#### Regras Importantes

⚠️ **Módulos CORE não podem ser desabilitados**
- São essenciais para o funcionamento do sistema
- Aparecem sempre marcados e desabilitados

⚠️ **Validação de Dependências**
- Alguns módulos dependem de outros
- O sistema impedirá configurações inválidas

**Exemplo de Dependências:**
- `WaitingQueue` → requer `AppointmentScheduling`
- `SMSNotifications` → requer `PatientManagement`
- `Reports` → requer `MedicalRecords`

⚠️ **Impacto nas Clínicas**
- Ao desabilitar um módulo de um plano:
  - Clínicas novas não terão acesso ao módulo
  - Clínicas existentes mantêm módulos já habilitados
  - Para desabilitar em clínicas existentes, use ações globais

### Exemplo: Configurar Plano Standard

**Objetivo:** Configurar um plano intermediário com relatórios

**Passos:**
1. Selecione "Standard" no dropdown
2. Habilite os seguintes módulos:
   - ✅ PatientManagement (Core - já habilitado)
   - ✅ AppointmentScheduling (Core - já habilitado)
   - ✅ MedicalRecords (Core - já habilitado)
   - ✅ Prescriptions (Core - já habilitado)
   - ✅ UserManagement (Core - já habilitado)
   - ✅ FinancialManagement (Advanced)
   - ✅ WaitingQueue (Advanced)
   - ✅ Reports (Analytics)
   - ✅ TissExport (Premium)
   - ❌ WhatsAppIntegration (Premium - não incluído)
   - ❌ SMSNotifications (Premium - não incluído)
   - ❌ InventoryManagement (Advanced - opcional)
3. Clique em "Salvar Configurações"
4. Confirme a operação

**Resultado:**
- Clínicas com plano Standard terão acesso aos módulos marcados
- Podem habilitar/desabilitar conforme necessidade
- Não poderão habilitar WhatsApp ou SMS (requer upgrade)

---

## 🔍 Detalhes do Módulo

### Visualizar Detalhes

1. No dashboard, clique no ícone 👁️ **"Visualizar"** de um módulo
2. Você será direcionado para a página de detalhes

### Informações Disponíveis

#### Informações Básicas
- **Nome:** Nome técnico e exibição
- **Categoria:** Core, Advanced, Premium ou Analytics
- **Descrição:** Funcionalidade do módulo
- **Dependências:** Módulos necessários
- **Status:** Ativo/Inativo

#### Estatísticas de Uso

**Adoção:**
- Número total de clínicas usando
- Percentual de adoção global
- Gráfico de evolução no tempo

**Por Plano:**
- Distribuição de uso por tipo de plano
- Gráfico de pizza ou barras
- Identifica planos com maior uso

#### Lista de Clínicas

Tabela mostrando quais clínicas usam o módulo:

| Clínica | Plano | Habilitado em | Status |
|---------|-------|---------------|--------|
| Clínica São Paulo | Premium | 15/01/2026 | ✅ Ativo |
| Clínica Rio | Standard | 10/01/2026 | ✅ Ativo |
| Clínica BH | Enterprise | 05/01/2026 | ✅ Ativo |

**Filtros:**
- Busca por nome da clínica
- Filtro por plano
- Filtro por status

#### Histórico de Mudanças

Timeline de mudanças no módulo:

```
📅 29/01/2026 10:30 - Clínica São Paulo habilitou o módulo
📅 28/01/2026 15:45 - Clínica Rio desabilitou o módulo
📅 25/01/2026 09:00 - System Admin atualizou configurações globais
```

**Informações em cada entrada:**
- Data e hora
- Clínica ou usuário responsável
- Ação realizada (habilitar, desabilitar, configurar)
- Valores antes e depois (se aplicável)

---

## 🌐 Ações Globais

### Habilitar Globalmente

**O que faz:** Habilita o módulo para todas as clínicas que têm o módulo disponível em seu plano.

**Quando usar:**
- Lançamento de um novo módulo
- Promoção ou campanha especial
- Ativação de recurso crítico

**Como executar:**
1. Acesse os detalhes do módulo
2. Clique em **"Habilitar Globalmente"**
3. Leia o aviso com atenção
4. Confirme a ação

**Exemplo de Aviso:**
```
⚠️ Atenção!

Você está prestes a habilitar o módulo "WhatsAppIntegration" 
para todas as 45 clínicas que têm este módulo em seu plano.

Clínicas afetadas:
- 20 clínicas no plano Premium
- 25 clínicas no plano Enterprise

Deseja continuar?

[Cancelar] [Confirmar]
```

**Impacto:**
- ✅ Clínicas verão o módulo habilitado imediatamente
- ✅ Funcionalidades do módulo ficam disponíveis
- ✅ Registro em histórico de auditoria

**Rollback:**
- Pode ser revertido com "Desabilitar Globalmente"
- Histórico mantém registro da mudança

### Desabilitar Globalmente

**O que faz:** Desabilita o módulo para todas as clínicas, independente do plano.

**Quando usar:**
- Problema crítico no módulo
- Manutenção programada
- Descontinuação de funcionalidade

**Como executar:**
1. Acesse os detalhes do módulo
2. Clique em **"Desabilitar Globalmente"**
3. **⚠️ CUIDADO:** Leia o aviso crítico
4. Digite "CONFIRMAR" para prosseguir
5. Confirme a ação

**Exemplo de Aviso:**
```
🚨 ATENÇÃO - AÇÃO CRÍTICA!

Você está prestes a DESABILITAR o módulo "Reports" 
para TODAS as 150 clínicas do sistema.

IMPACTO:
- Funcionalidades do módulo serão desabilitadas imediatamente
- Clínicas não poderão gerar relatórios
- Possível impacto em operações críticas

Esta ação é reversível, mas pode causar transtornos.

Para confirmar, digite "CONFIRMAR" abaixo:
[____________]

[Cancelar] [Desabilitar]
```

**Impacto:**
- ⚠️ Funcionalidades ficam indisponíveis imediatamente
- ⚠️ Menu items são removidos da interface
- ⚠️ APIs relacionadas retornam erro
- ✅ Dados não são perdidos
- ✅ Pode ser reabilitado sem perda de informações

**Comunicação:**
- Avisar clínicas antes da ação
- Fornecer justificativa e prazo
- Oferecer suporte para dúvidas

---

## 📈 Relatórios e Analytics

### Adoção por Categoria

**O que mostra:** Distribuição de uso de módulos por categoria.

**Visualização:**
- Gráfico de barras comparativo
- Percentual de adoção média por categoria

**Insights:**
- Categorias mais populares
- Oportunidades de crescimento
- Identificação de categorias sub-utilizadas

**Exemplo:**
```
Core:      95% de adoção média
Advanced:  60% de adoção média
Premium:   35% de adoção média
Analytics: 45% de adoção média
```

**Análise:**
- Core: Esperado, são módulos essenciais
- Advanced: Boa adoção, mas pode melhorar
- Premium: Oportunidade de upsell
- Analytics: Educar sobre benefícios

### Uso por Plano

**O que mostra:** Comparação de uso de módulos entre diferentes planos.

**Visualização:**
- Tabela com breakdown por plano
- Gráficos de comparação

**Exemplo:**

| Módulo | Basic | Standard | Premium | Enterprise |
|--------|-------|----------|---------|------------|
| PatientManagement | 100% | 100% | 100% | 100% |
| Reports | - | 75% | 85% | 95% |
| WhatsApp | - | - | 60% | 90% |
| SMS | - | - | 55% | 88% |

**Insights:**
- Módulos com baixa adoção em planos que os incluem
- Oportunidades para educação e treinamento
- Validação de features mais valorizadas

### Tendências

**O que mostra:** Evolução do uso de módulos ao longo do tempo.

**Visualização:**
- Gráfico de linha temporal
- Período configurável (7d, 30d, 90d, 1 ano)

**Métricas:**
- Crescimento de adoção
- Módulos em ascensão
- Módulos em declínio

**Exemplo:**
```
WhatsAppIntegration:
- Jan 2026: 30% (45 clínicas)
- Dez 2025: 25% (38 clínicas)
- Nov 2025: 20% (30 clínicas)
Tendência: 📈 Crescimento de 10% em 2 meses
```

**Análise:**
- Identificar padrões sazonais
- Medir impacto de campanhas
- Antecipar necessidades futuras

### Exportar Relatórios

**Formatos Disponíveis:**
- 📊 Excel (.xlsx) - Para análise detalhada
- 📄 PDF - Para apresentações
- 📋 CSV - Para importar em outras ferramentas

**Dados Incluídos:**
- Estatísticas de todos os módulos
- Histórico de mudanças
- Tendências e análises

**Como exportar:**
1. Na página de relatórios, clique em **"Exportar"**
2. Selecione o formato desejado
3. Escolha o período (opcional)
4. Clique em **"Baixar"**
5. Aguarde o download

---

## 💡 Melhores Práticas

### Gestão Proativa

✅ **Revise a adoção mensalmente**
- Agende revisões periódicas
- Identifique tendências cedo
- Tome ações preventivas

✅ **Promova módulos sub-utilizados**
- Crie materiais educativos
- Ofereça webinars de treinamento
- Mostre cases de sucesso

✅ **Configure planos progressivos**
- Basic: Funcionalidades essenciais
- Standard: + Relatórios e integrações básicas
- Premium: + Comunicação (WhatsApp, SMS)
- Enterprise: Tudo + customizações

✅ **Monitore feedback das clínicas**
- Crie canais de comunicação
- Registre sugestões e problemas
- Priorize melhorias baseadas em uso

### O que Evitar

❌ **Desabilitar módulos em uso sem aviso**
- Sempre comunique antes
- Forneça alternativas
- Dê tempo para adaptação

❌ **Remover módulos core dos planos**
- Quebrará funcionalidades essenciais
- Sistema impedirá esta ação

❌ **Ignorar módulos com baixa adoção**
- Baixa adoção pode indicar problemas
- Investigue antes de descontinuar

❌ **Mudar configurações sem documentar**
- Mantenha changelog interno
- Documente razões das mudanças
- Facilita troubleshooting

### Comunicação

**Antes de mudanças críticas:**
1. Notificar clínicas afetadas (48h antes mínimo)
2. Explicar motivo e impacto
3. Fornecer documentação de suporte
4. Disponibilizar canal para dúvidas

**Email Template:**
```
Assunto: Atualização de Módulos - [Nome do Módulo]

Prezada clínica,

Informamos que em [DATA] às [HORA], o módulo [NOME] 
será [AÇÃO].

Motivo: [JUSTIFICATIVA]

Impacto: [DESCRIÇÃO DO IMPACTO]

O que fazer: [INSTRUÇÕES]

Dúvidas? Entre em contato: [CONTATO]

Atenciosamente,
Equipe Omni Care
```

---

## 🆘 Problemas Comuns

### "Módulo não pode ser habilitado"

**Possíveis causas:**
1. Módulo não disponível no plano da clínica
2. Dependências não satisfeitas
3. Limite de recursos do plano atingido

**Soluções:**
1. Verificar plano da clínica: Menu → Clínicas → [Nome] → Assinatura
2. Verificar dependências do módulo: Detalhes do Módulo → Seção "Dependências"
3. Revisar limites: Detalhes do Plano → Limites e Cotas
4. Se necessário, fazer upgrade de plano

**Como diagnosticar:**
```
1. Acesse Módulos → Detalhes → [Módulo]
2. Verifique "Dependências"
3. Confirme que módulos dependentes estão habilitados
4. Verifique logs de erro: Menu → Logs → Filtrar por ClinicId
```

### "Taxa de adoção baixa"

**Análise necessária:**
- Módulo é novo? (adoção leva tempo)
- Módulo é complexo? (requer treinamento)
- Módulo tem custos adicionais? (barreira financeira)
- Módulo resolve problema real? (value proposition)

**Ações:**
1. **Pesquisar com clínicas:** Por que não usam?
2. **Criar material educativo:** Vídeos, guias, tutoriais
3. **Realizar webinars:** Demonstrações práticas
4. **Oferecer período trial:** Premium temporariamente gratuito
5. **Compartilhar cases:** Histórias de sucesso

**Métricas de acompanhamento:**
- Taxa de ativação (clínicas que experimentaram)
- Taxa de retenção (clínicas que continuam usando)
- NPS específico do módulo

### "Clínicas reclamando de limite"

**Situação:** Clínica atingiu limite do plano (usuários, pacientes, etc.)

**Diagnóstico:**
1. Verificar uso atual: Dashboard → Clínicas → [Nome]
2. Ver limite do plano: Planos → [Plano da Clínica]
3. Histórico de crescimento: Analytics → Tendências

**Soluções:**

**Opção 1: Upgrade de Plano**
1. Apresentar benefícios do plano superior
2. Calcular ROI para a clínica
3. Oferecer migração assistida
4. Garantir transição suave

**Opção 2: Ajuste Customizado**
1. Avaliar caso específico
2. Criar exceção se justificado
3. Documentar razão do ajuste
4. Definir prazo de revisão

**Opção 3: Otimização**
1. Analisar uso atual
2. Identificar desperdícios
3. Sugerir otimizações
4. Treinar equipe da clínica

**Template de Resposta:**
```
Prezada [Clínica],

Identificamos que vocês atingiram o limite de [RECURSO] 
do plano [PLANO ATUAL].

Uso atual: [NÚMERO] de [NÚMERO LIMITE]

Sugerimos:
1. [OPÇÃO 1]
2. [OPÇÃO 2]

Podemos agendar uma call para discutir a melhor opção?

Atenciosamente,
[Seu Nome]
System Admin - Omni Care
```

### "Erro ao salvar configurações"

**Possíveis causas:**
1. Sessão expirada (timeout)
2. Conflito de versão (alteração simultânea)
3. Validação de negócio falhou
4. Problema de conectividade

**Soluções:**
1. **Sessão expirada:**
   - Fazer logout e login novamente
   - Tentar salvar novamente

2. **Conflito:**
   - Recarregar a página
   - Verificar se outra pessoa está editando
   - Tentar novamente

3. **Validação:**
   - Ler mensagem de erro
   - Corrigir problema apontado
   - Tentar salvar novamente

4. **Conectividade:**
   - Verificar internet
   - Verificar status do sistema
   - Aguardar e tentar novamente

**Logs:**
- Acessar: Menu → Logs → Filtrar por "ModuleConfig"
- Buscar por timestamp do erro
- Analisar stack trace

---

## 🔧 Configurações Avançadas

### Personalização de Planos

**Criar Plano Customizado:**

Caso os planos padrão não atendam:

1. Menu → Administração → Planos
2. Clique em **"Novo Plano Customizado"**
3. Preencha informações:
   - Nome do plano
   - Descrição
   - Preço mensal
   - Limites (usuários, pacientes, etc.)
4. Selecione módulos incluídos
5. Salve o plano
6. Atribua a clínicas específicas

**Exemplo - Plano para Clínicas Especializadas:**
```
Nome: Premium Oftalmologia
Preço: R$ 349/mês
Limites: 10 usuários, 5000 pacientes

Módulos:
- Todos Core (obrigatório)
- FinancialManagement
- Reports
- WhatsAppIntegration
- InventoryManagement (para controle de lentes)
- Custom: Campos específicos de oftalmologia
```

### Configuração de Features Flags

Para testes A/B ou rollout gradual:

1. Menu → Administração → Feature Flags
2. Criar novo flag:
   - Nome: `new_dashboard_layout`
   - Descrição: `Novo layout do dashboard`
   - Status: Habilitado
   - Percentual: 10% (rollout gradual)
   - Clínicas específicas: Opcional
3. Monitorar métricas
4. Aumentar percentual gradualmente
5. Habilitar 100% quando validado

### Notificações Automáticas

Configurar alertas para eventos importantes:

1. Menu → Administração → Notificações
2. Criar novo alerta:
   - Evento: "Módulo Premium habilitado"
   - Destinatários: "Equipe de Sucesso do Cliente"
   - Meio: Email + Slack
   - Template: Customizar mensagem
3. Salvar configuração

**Eventos Monitoráveis:**
- Módulo habilitado/desabilitado
- Limite de plano atingido
- Upgrade/downgrade de plano
- Erro crítico em módulo
- Baixa adoção de módulo novo

---

## 📞 Suporte

### Contatos

**Suporte Técnico:**
- 📧 Email: suporte@omnicare.com.br
- 📱 WhatsApp: (11) 98765-4321
- 💬 Chat: [Abrir Ticket no Sistema]
- 📚 Base de Conhecimento: [Central de Ajuda]

**Horário de Atendimento:**
- Segunda a Sexta: 8h às 18h
- Sábado: 8h às 12h
- Emergências: 24/7 (apenas casos críticos)

### SLA (Service Level Agreement)

| Prioridade | Tempo de Resposta | Tempo de Resolução |
|------------|-------------------|-------------------|
| Crítico | 30 minutos | 4 horas |
| Alto | 2 horas | 8 horas |
| Médio | 8 horas | 24 horas |
| Baixo | 24 horas | 72 horas |

**Prioridade Crítica:**
- Sistema fora do ar
- Perda de dados
- Falha de segurança

**Prioridade Alta:**
- Módulo crítico não funciona
- Impacto em múltiplas clínicas
- Problema de performance severo

### Documentação Adicional

- 📖 [Documentação Técnica da API](./API_DOCUMENTATION.md)
- 🏗️ [Arquitetura do Sistema](./ARQUITETURA_MODULOS.md)
- 🏥 [Guia do Usuário - Clínica](./GUIA_USUARIO_CLINICA.md)
- 📝 [Release Notes](./RELEASE_NOTES.md)

---

## 📺 Vídeo Tutoriais

🎥 **Tutoriais em Vídeo:**

1. [Introdução ao Sistema de Módulos](https://youtube.com/...) - 3 min
2. [Dashboard e Métricas](https://youtube.com/...) - 6 min
3. [Configurar Módulos por Plano](https://youtube.com/...) - 7 min
4. [Ações Globais e Impactos](https://youtube.com/...) - 5 min
5. [Relatórios e Analytics](https://youtube.com/...) - 8 min

---

## 📋 Checklist do System Admin

### Diário
- [ ] Verificar alertas críticos
- [ ] Revisar logs de erro
- [ ] Monitorar performance do sistema

### Semanal
- [ ] Revisar métricas de adoção
- [ ] Analisar módulos com baixa performance
- [ ] Verificar tickets de suporte relacionados a módulos
- [ ] Atualizar documentação se necessário

### Mensal
- [ ] Relatório executivo de adoção
- [ ] Análise de tendências
- [ ] Identificar oportunidades de upsell
- [ ] Revisar configuração de planos
- [ ] Planejar campanhas educativas

### Trimestral
- [ ] Avaliação completa de módulos
- [ ] Pesquisa de satisfação com clínicas
- [ ] Planejamento de novos módulos
- [ ] Revisão de pricing e planos

---

## 🎯 Objetivos e KPIs

### Métricas de Sucesso

**Adoção:**
- Taxa média de adoção > 60%
- Crescimento mês a mês > 5%
- Módulos Premium: adoção > 30%

**Satisfação:**
- NPS geral > 50
- NPS por módulo > 40
- Tickets de suporte relacionados < 2% do total

**Negócio:**
- Upgrade de planos: > 10% ao trimestre
- Churn < 5% ao mês
- Módulos habilitados por clínica: média > 8

### Dashboard Executivo

Apresente mensalmente ao management:

1. **Slide 1: Overview**
   - Total de módulos ativos
   - Taxa média de adoção
   - Crescimento vs. mês anterior

2. **Slide 2: Destaques**
   - Módulo mais adotado
   - Módulo com maior crescimento
   - Módulo com desafios

3. **Slide 3: Negócio**
   - Upgrades de plano
   - Receita incremental
   - Oportunidades identificadas

4. **Slide 4: Próximos Passos**
   - Ações planejadas
   - Novos módulos em desenvolvimento
   - Campanhas educativas

---

*Última atualização: 29 de Janeiro de 2026*

**Versão:** 1.0  
**Autor:** Omni Care Software - System Admin Team
