# Glossário de Termos Empresariais - PrimeCare Software

## 📚 Introdução

Este documento foi criado para ajudar você a entender os **termos empresariais e de negócio** utilizados no sistema PrimeCare Software. Se você não tem familiaridade com conceitos da área empresarial, este guia irá explicar cada termo de forma clara e prática, para que possa estudar e aplicar esses conhecimentos no seu negócio.

---

## 🎯 Termos Fundamentais de Negócio

### SaaS (Software as a Service / Software como Serviço)
**O que é:** Modelo de negócio onde o software é oferecido pela internet mediante assinatura, sem que o cliente precise instalar nada no computador.

**Exemplo prático:** 
- Netflix é um SaaS para entretenimento
- PrimeCare Software é um SaaS para gestão de clínicas médicas
- Cliente paga mensalidade e acessa o sistema pela internet

**Vantagens:**
- Cliente não precisa comprar servidores
- Atualizações automáticas
- Acesso de qualquer lugar
- Custo inicial baixo

---

### Multitenant (Multilocação)
**O que é:** Arquitetura onde um único sistema atende múltiplos clientes (tenants), mantendo os dados de cada um totalmente separados e seguros.

**Exemplo prático:**
- 100 clínicas usam o mesmo PrimeCare Software
- Cada clínica só vê seus próprios dados
- Clínica A não consegue acessar dados da Clínica B
- É como um prédio com vários apartamentos (tenants)

**Benefícios:**
- Reduz custos de infraestrutura
- Facilita manutenção
- Cada cliente tem sua "área privada"

---

### MRR (Monthly Recurring Revenue / Receita Recorrente Mensal)
**O que é:** Total de dinheiro que entra todo mês através das assinaturas ativas.

**Como calcular:**
```
MRR = Soma de todas as mensalidades dos clientes ativos

Exemplo:
- 10 clientes no plano Basic (R$ 190/mês) = R$ 1.900
- 5 clientes no plano Standard (R$ 240/mês) = R$ 1.200
- 3 clientes no plano Premium (R$ 320/mês) = R$ 960
----------------------------------------
MRR Total = R$ 4.060/mês
```

**Por que é importante:**
- Prevê a receita do próximo mês
- Mostra a saúde financeira do negócio
- Ajuda no planejamento financeiro

---

### ARR (Annual Recurring Revenue / Receita Recorrente Anual)
**O que é:** Projeção da receita recorrente para 12 meses.

**Como calcular:**
```
ARR = MRR × 12

Se MRR = R$ 4.060
ARR = R$ 48.720/ano
```

**Aplicação:**
- Planejamento de longo prazo
- Valuation da empresa
- Projeção de crescimento

---

### Churn Rate (Taxa de Cancelamento)
**O que é:** Percentual de clientes que cancelam a assinatura em um período.

**Como calcular:**
```
Churn Rate = (Clientes que cancelaram / Total de clientes no início) × 100

Exemplo:
- Início do mês: 100 clientes
- Cancelamentos no mês: 5 clientes
- Churn Rate = (5 / 100) × 100 = 5%
```

**Interpretação:**
- Churn baixo (< 5%): Ótimo! Clientes estão satisfeitos
- Churn médio (5-10%): Atenção! Precisa melhorar
- Churn alto (> 10%): Alerta! Clientes insatisfeitos

**Como reduzir:**
- Melhorar suporte ao cliente
- Adicionar mais funcionalidades
- Ouvir feedback dos usuários

---

### CAC (Customer Acquisition Cost / Custo de Aquisição de Cliente)
**O que é:** Quanto você gasta para conquistar um novo cliente.

**Como calcular:**
```
CAC = (Gastos com Marketing + Vendas) / Número de novos clientes

Exemplo:
- Gastos com anúncios: R$ 5.000
- Salário da equipe de vendas: R$ 10.000
- Novos clientes no mês: 15
----------------------------------------
CAC = R$ 15.000 / 15 = R$ 1.000 por cliente
```

**Por que importa:**
- Precisa ser menor que o LTV
- Ajuda a definir orçamento de marketing
- Mostra eficiência de vendas

---

### LTV (Lifetime Value / Valor do Tempo de Vida do Cliente)
**O que é:** Quanto um cliente gera de receita durante todo o tempo que fica com você.

**Como calcular:**
```
LTV = Receita Mensal Média × Tempo Médio de Permanência

Exemplo:
- Cliente paga R$ 240/mês
- Fica em média 24 meses
----------------------------------------
LTV = R$ 240 × 24 = R$ 5.760
```

**Regra de ouro:**
```
LTV deve ser pelo menos 3x maior que o CAC

Se CAC = R$ 1.000
LTV ideal ≥ R$ 3.000
```

---

### ROI (Return on Investment / Retorno sobre Investimento)
**O que é:** Quanto você ganhou em relação ao que investiu.

**Como calcular:**
```
ROI = [(Receita - Custo) / Custo] × 100

Exemplo:
- Investiu R$ 10.000 em marketing
- Gerou R$ 30.000 em vendas
----------------------------------------
ROI = [(30.000 - 10.000) / 10.000] × 100 = 200%
```

**Interpretação:**
- ROI positivo: Lucro
- ROI negativo: Prejuízo
- ROI de 200%: Para cada R$ 1 investido, ganhou R$ 2

---

## 💰 Termos de Assinatura e Pagamento

### Trial (Período de Teste)
**O que é:** Período gratuito onde o cliente pode testar o sistema antes de pagar.

**No PrimeCare Software:**
- 15 dias gratuitos
- Recursos limitados
- Não precisa cartão de crédito
- Após trial, deve escolher um plano pago

**Objetivo:**
- Mostrar valor do produto
- Conquistar confiança do cliente
- Reduzir barreira de entrada

---

### Upgrade (Melhoria de Plano)
**O que é:** Quando o cliente muda para um plano mais caro com mais recursos.

**Como funciona no PrimeCare Software:**
```
Cliente no Basic (R$ 190) → Premium (R$ 320)
- Diferença: R$ 130
- Paga R$ 130 imediatamente (proporcional)
- Recebe recursos do Premium na hora
- Próxima cobrança: R$ 320
```

**Benefícios:**
- Cliente tem mais recursos
- Empresa aumenta receita
- Win-win (ganha-ganha)

---

### Downgrade (Redução de Plano)
**O que é:** Quando o cliente muda para um plano mais barato com menos recursos.

**Como funciona no PrimeCare Software:**
```
Cliente no Premium (R$ 320) → Basic (R$ 190)
- Mudança agendada para próximo vencimento
- Continua com Premium até lá
- Sem reembolso
- Próxima cobrança: R$ 190
```

**Quando acontece:**
- Cliente quer economizar
- Não está usando todos os recursos
- Negócio está em dificuldade

---

### Freeze (Congelamento)
**O que é:** Pausar a assinatura por um período determinado.

**No PrimeCare Software:**
- Duração: 1 mês fixo
- Suspende cobrança
- Bloqueia acesso ao sistema
- Prorroga vencimento em 1 mês

**Quando usar:**
- Cliente está viajando
- Consultório fechado temporariamente
- Problemas financeiros temporários

---

### Payment Overdue (Pagamento Atrasado)
**O que é:** Quando o pagamento não foi realizado na data de vencimento.

**Fluxo no PrimeCare Software:**
```
1. Vencimento passa
2. Status → PaymentOverdue
3. Sistema envia notificações (SMS, Email, WhatsApp)
4. Cliente tem prazo para regularizar
5. Se não pagar → Suspende acesso
```

---

### Inadimplência
**O que é:** Cliente que está com pagamentos atrasados.

**Como gerenciar:**
- Enviar lembretes antes do vencimento
- Facilitar formas de pagamento
- Oferecer parcelamento
- Manter comunicação respeitosa

**Impacto no negócio:**
- Reduz o MRR
- Aumenta custos de cobrança
- Prejudica fluxo de caixa

---

## 👥 Termos de Gestão de Usuários

### RBAC (Role-Based Access Control / Controle de Acesso Baseado em Funções)
**O que é:** Sistema que define o que cada tipo de usuário pode fazer.

**Roles (Funções) no PrimeCare Software:**
```
1. SystemAdmin (Administrador do Sistema)
   - Gerencia TODAS as clínicas
   - Vê métricas globais
   - Controle total

2. ClinicOwner (Dono da Clínica)
   - Gerencia SUA clínica
   - Cadastra funcionários
   - Vê relatórios financeiros

3. Doctor/Dentist (Médico/Dentista)
   - Atende pacientes
   - Prescreve medicamentos
   - Acessa prontuários

4. Nurse (Enfermeiro)
   - Prepara pacientes
   - Registra sinais vitais
   - Auxilia médicos

5. Receptionist/Secretary (Recepcionista/Secretária)
   - Agenda consultas
   - Cadastra pacientes
   - Recebe pagamentos
```

**Por que é importante:**
- Segurança dos dados
- Organização clara
- Responsabilidades definidas

---

### Permissions (Permissões)
**O que é:** Ações específicas que cada role pode executar.

**Exemplos:**
```
Receptionist pode:
✅ Agendar consultas
✅ Cadastrar pacientes
❌ Prescrever medicamentos (só médico)
❌ Ver relatórios financeiros (só dono)
```

---

### Onboarding (Integração)
**O que é:** Processo de apresentar o sistema ao novo cliente.

**Etapas no PrimeCare Software:**
```
1. Cadastro inicial da clínica
2. Tutorial das funcionalidades
3. Importação de dados (se necessário)
4. Treinamento da equipe
5. Primeiro agendamento
6. Suporte inicial
```

**Objetivo:**
- Cliente aprender rápido
- Reduzir abandono inicial
- Aumentar satisfação

---

## 📊 Termos de Métricas e Analytics

### Dashboard (Painel de Controle)
**O que é:** Tela com resumo visual das principais informações do negócio.

**Informações típicas:**
- Consultas do dia/semana/mês
- Receita do período
- Pacientes ativos
- Agendamentos pendentes
- Inadimplência

---

### KPI (Key Performance Indicator / Indicador-Chave de Desempenho)
**O que é:** Métricas que mostram se o negócio está indo bem.

**KPIs do PrimeCare Software:**
```
1. Taxa de ocupação de agenda
   - Quantos horários foram preenchidos

2. Tempo médio de atendimento
   - Quanto tempo dura cada consulta

3. Taxa de comparecimento
   - Quantos pacientes aparecem nas consultas

4. Receita por médico
   - Quanto cada médico gera

5. Taxa de retorno de pacientes
   - Quantos voltam para nova consulta
```

---

### Conversion Rate (Taxa de Conversão)
**O que é:** Percentual de pessoas que completam uma ação desejada.

**Exemplos:**
```
1. Trial → Pagante
   - Quantos que testaram assinaram
   - Meta: > 20%

2. Visitante → Trial
   - Quantos que visitaram o site se cadastraram
   - Meta: > 5%

3. Lead → Cliente
   - Quantos contatos viraram clientes
   - Meta: > 10%
```

---

### Funnel (Funil de Vendas)
**O que é:** Jornada do cliente desde conhecer até comprar.

**Funil do PrimeCare Software:**
```
1. Visitante (topo do funil)
   ↓ (conversão)
2. Lead (interessado)
   ↓ (conversão)
3. Trial (testando)
   ↓ (conversão)
4. Cliente (pagante)
   ↓ (fidelização)
5. Promotor (indica outros)
```

**Meta:** Aumentar conversão em cada etapa.

---

## 🏥 Termos Específicos da Área Médica

### Prontuário Eletrônico
**O que é:** Histórico digital completo do paciente.

**Contém:**
- Dados pessoais
- Histórico de consultas
- Diagnósticos
- Prescrições
- Exames
- Alergias

---

### Anamnese
**O que é:** Entrevista inicial com o paciente para coletar histórico médico.

**Informações coletadas:**
- Queixa principal
- História da doença atual
- Antecedentes pessoais
- Antecedentes familiares
- Hábitos de vida

---

### Triage (Triagem)
**O que é:** Classificação inicial de urgência do paciente.

**Níveis:**
- 🔴 Emergência (imediato)
- 🟡 Urgente (até 1 hora)
- 🟢 Não urgente (ordem de chegada)

---

### TISS (Troca de Informações em Saúde Suplementar)
**O que é:** Padrão brasileiro para troca de informações com planos de saúde.

**Para que serve:**
- Enviar guias para operadoras
- Receber autorizações
- Processar pagamentos
- Padronizar comunicação

---

## 💼 Termos de Gestão Empresarial

### Fluxo de Caixa
**O que é:** Controle de tudo que entra e sai de dinheiro.

**Como fazer:**
```
Receitas:
+ R$ 10.000 (mensalidades)
+ R$ 2.000 (consultas particulares)

Despesas:
- R$ 3.000 (salários)
- R$ 1.000 (aluguel)
- R$ 500 (internet/luz)
----------------------------------------
Saldo = R$ 7.500 (positivo = bom!)
```

---

### Break-even (Ponto de Equilíbrio)
**O que é:** Momento em que a receita cobre todos os custos (nem lucro, nem prejuízo).

**Como calcular:**
```
Break-even = Custos Fixos / (Preço - Custo Variável)

Exemplo:
- Custos fixos: R$ 10.000/mês (servidor, salários)
- Preço do plano: R$ 240
- Custo variável por cliente: R$ 40
----------------------------------------
Break-even = 10.000 / (240 - 40) = 50 clientes

Precisa de 50 clientes para empatar!
```

---

### Runway (Pista de Pouso)
**O que é:** Quanto tempo seu dinheiro dura até acabar.

**Como calcular:**
```
Runway = Dinheiro em Caixa / Queima Mensal

Exemplo:
- Tem R$ 100.000 em caixa
- Gasta R$ 20.000/mês mais do que ganha
----------------------------------------
Runway = 100.000 / 20.000 = 5 meses

Você tem 5 meses para virar o jogo!
```

---

### Burn Rate (Taxa de Queima)
**O que é:** Quanto dinheiro você gasta por mês além do que ganha.

**Exemplo:**
```
Receita mensal: R$ 30.000
Despesas mensais: R$ 50.000
----------------------------------------
Burn Rate = R$ 20.000/mês (negativo)
```

**Atenção:** Burn rate alto consome o caixa rápido!

---

## 🎁 Termos de Marketing e Vendas

### Freemium
**O que é:** Modelo onde oferece versão gratuita e cobra por recursos avançados.

**Exemplo:**
- Grátis: 1 usuário, recursos básicos
- Pago: múltiplos usuários, recursos avançados

---

### Upselling
**O que é:** Oferecer produto/serviço superior ao que o cliente já tem.

**Exemplo:**
- Cliente tem plano Basic
- Você oferece Premium com mais recursos
- Cliente faz upgrade

---

### Cross-selling
**O que é:** Vender produtos complementares.

**Exemplo:**
- Cliente tem o sistema de gestão
- Você oferece módulo de WhatsApp
- Cliente adiciona ao plano

---

### Lead
**O que é:** Potencial cliente que demonstrou interesse.

**Como gerar:**
- Anúncios online
- Indicações
- Eventos
- Conteúdo educativo

---

### Prospect
**O que é:** Lead qualificado que tem potencial real de comprar.

**Diferença:**
```
Lead = Alguém que deu o email
Prospect = Alguém que realmente precisa e pode pagar
```

---

## 📈 Termos de Crescimento

### Growth Hacking
**O que é:** Estratégias criativas e de baixo custo para crescer rápido.

**Exemplos:**
- Programa de indicações
- Conteúdo viral
- Parcerias estratégicas
- Gamificação

---

### Viral Loop
**O que é:** Cada usuário traz novos usuários automaticamente.

**Exemplo:**
```
1. Cliente usa o sistema
2. Indica para 3 colegas
3. Cada colega indica mais 3
4. Crescimento exponencial!
```

---

### Network Effect (Efeito de Rede)
**O que é:** Produto fica mais valioso conforme mais pessoas usam.

**Exemplo:**
- WhatsApp: quanto mais gente usa, mais útil fica
- Rede de clínicas: podem compartilhar pacientes

---

### Scalability (Escalabilidade)
**O que é:** Capacidade de crescer sem aumentar custos proporcionalmente.

**SaaS é escalável:**
```
10 clientes: Custo R$ 5.000
100 clientes: Custo R$ 8.000 (não 10x mais!)
1000 clientes: Custo R$ 15.000

Receita cresce linear, custo cresce devagar!
```

---

## 🔒 Termos de Segurança e Compliance

### LGPD (Lei Geral de Proteção de Dados)
**O que é:** Lei brasileira que protege dados pessoais.

**Principais pontos:**
- Cliente deve autorizar uso de dados
- Dados devem estar seguros
- Cliente pode pedir exclusão
- Multas pesadas para quem descumprir

**No PrimeCare Software:**
- Dados médicos são sensíveis
- Isolamento entre clínicas (multitenant)
- Criptografia de dados
- Logs de acesso

---

### 2FA (Two-Factor Authentication / Autenticação em Dois Fatores)
**O que é:** Segurança adicional que exige dois tipos de prova de identidade.

**Exemplo:**
```
1º Fator: Senha (algo que você sabe)
2º Fator: Código SMS (algo que você tem)
```

---

### Encryption (Criptografia)
**O que é:** Transformar dados em código secreto que só pode ser lido com a chave certa.

**Tipos:**
- Em trânsito: Protege dados durante transmissão (HTTPS)
- Em repouso: Protege dados armazenados (banco de dados)

---

## 💡 Dicas para Aplicar no Seu Negócio

### 1. Comece Medindo
```
✅ Defina seus KPIs principais
✅ Registre todos os números
✅ Compare mês a mês
✅ Tome decisões baseadas em dados
```

### 2. Foque na Retenção
```
✅ Manter cliente é mais barato que conquistar novo
✅ Cliente satisfeito indica outros
✅ Reduza o churn a qualquer custo
✅ Invista em suporte e qualidade
```

### 3. Entenda Seus Números
```
✅ CAC vs LTV
✅ MRR e crescimento
✅ Churn e causas
✅ Break-even e runway
```

### 4. Pense em Escalabilidade
```
✅ Automatize processos
✅ Documente tudo
✅ Padronize atendimento
✅ Use tecnologia
```

### 5. Cuide do Cliente
```
✅ Onboarding bem-feito
✅ Suporte rápido
✅ Escute feedback
✅ Melhore continuamente
```

---

## 📖 Recursos para Aprofundar

### Livros Recomendados
1. **"A Startup Enxuta"** - Eric Ries
   - Conceitos de MVP, pivotar, aprender rápido

2. **"Zero to One"** - Peter Thiel
   - Construir empresas inovadoras

3. **"Tração"** - Gabriel Weinberg
   - 19 canais para crescer seu negócio

4. **"Hooked"** - Nir Eyal
   - Como criar produtos que prendem atenção

### Blogs e Sites
- **Endeavor Brasil**: Conteúdo sobre empreendedorismo
- **Rock Content**: Marketing digital
- **Saia do Lugar**: Gestão e finanças
- **ABStartups**: Ecossistema de startups

### Cursos Online
- **Udemy**: Cursos de gestão e negócios
- **Coursera**: Certificações de universidades
- **Sebrae**: Cursos gratuitos para empreendedores
- **LinkedIn Learning**: Habilidades profissionais

---

## 🎓 Glossário Rápido (Ordem Alfabética)

| Termo | Significado Resumido |
|-------|---------------------|
| **2FA** | Autenticação em dois fatores |
| **Analytics** | Análise de dados |
| **ARR** | Receita recorrente anual |
| **Break-even** | Ponto de equilíbrio |
| **Burn Rate** | Taxa de queima de caixa |
| **CAC** | Custo de aquisição de cliente |
| **Churn** | Taxa de cancelamento |
| **Conversion** | Taxa de conversão |
| **Cross-selling** | Venda de complementares |
| **Dashboard** | Painel de controle |
| **Downgrade** | Redução de plano |
| **Freemium** | Grátis + premium |
| **Freeze** | Congelamento |
| **Funnel** | Funil de vendas |
| **Growth** | Crescimento |
| **KPI** | Indicador-chave de desempenho |
| **Lead** | Potencial cliente |
| **LGPD** | Lei de proteção de dados |
| **LTV** | Valor vitalício do cliente |
| **MRR** | Receita recorrente mensal |
| **Multitenant** | Múltiplos clientes isolados |
| **Onboarding** | Integração de novos usuários |
| **RBAC** | Controle de acesso por função |
| **ROI** | Retorno sobre investimento |
| **Runway** | Tempo até dinheiro acabar |
| **SaaS** | Software como serviço |
| **Scalability** | Escalabilidade |
| **Trial** | Período de teste |
| **Upgrade** | Melhoria de plano |
| **Upselling** | Venda de produto superior |

---

## 🆘 Precisa de Mais Ajuda?

### Documentação Relacionada
- [`BUSINESS_RULES.md`](frontend/mw-docs/src/assets/docs/BUSINESS_RULES.md) - Regras de negócio detalhadas
- [`SUBSCRIPTION_SYSTEM.md`](frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md) - Sistema de assinaturas
- [`README.md`](../README.md) - Visão geral do projeto

### Contato
- 📧 Entre em contato com a equipe PrimeCare Software
- 💬 Participe de comunidades de empreendedorismo
- 📚 Continue estudando e praticando

---

**Última atualização:** Outubro 2025

**Versão:** 1.0

**Mantido por:** Equipe PrimeCare Software

---

> 💡 **Dica Final:** Este glossário é um ponto de partida. A melhor forma de aprender é praticando! Comece aplicando um termo por vez no seu negócio e vá expandindo gradualmente. Boa sorte! 🚀
