# MedicWarehouse - Sistema de Gestão para Consultórios Médicos

[![CI - Test Frontend e Backend](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml/badge.svg)](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml)

> 🚀 **NOVO!** Quer rodar o sistema localmente AGORA? [→ Ver Guia de Início Rápido](docs/GUIA_INICIO_RAPIDO_LOCAL.md) - Setup em menos de 10 minutos!

> 🌍 **NOVO!** Desenvolva em **macOS, Windows ou Linux**! [→ Ver Guia Multiplataforma](docs/GUIA_MULTIPLATAFORMA.md) - Compatibilidade total garantida!

> 🐳 **NOVO!** Migramos para **Podman** (100% gratuito e open-source)! Docker ainda é suportado. [→ Ver Guia de Migração](docs/DOCKER_TO_PODMAN_MIGRATION.md)

> ⚠️ **AÇÃO NECESSÁRIA**: GitHub Pages precisa ser habilitado para deploy da documentação. [Ver instruções →](docs/GITHUB_PAGES_SETUP_REQUIRED.md)

Uma solução **DDD** multitenant completa para gestão de consultórios médicos (SaaS) construída com **Angular 20**, **.NET 8** e **PostgreSQL**.

## 🌍 Compatibilidade Multiplataforma

O MedicWarehouse é **100% cross-platform**:

- ✅ **macOS**: Script automatizado de setup (`setup-macos.sh`)
- ✅ **Windows**: Script PowerShell de setup (`setup-windows.ps1`)
- ✅ **Linux**: Suporte completo (Ubuntu, Fedora, Debian, etc.)
- 📖 **[Guia Completo](docs/GUIA_MULTIPLATAFORMA.md)**: Instruções detalhadas para cada plataforma

> ✅ **NOVO**: Sistema migrado para PostgreSQL! Economia de 90-96% em custos de infraestrutura. [Ver detalhes →](docs/MIGRACAO_POSTGRESQL.md)

> 🎯 **NOVO**: Login por Subdomínio! Acesse sua clínica via `clinic1.mwsistema.com.br` sem precisar digitar Tenant ID. [Ver guia →](docs/SUBDOMAIN_LOGIN_GUIDE.md)

> 🧪 **NOVO**: Dados Mockados! Execute o frontend sem backend para desenvolvimento e testes. [Ver guia →](docs/MOCK_DATA_GUIDE.md)

> 📱 **NOVO!** Aplicativos Móveis Nativos! Apps para iOS (Swift/SwiftUI) e Android (Kotlin/Compose) disponíveis! [→ Ver Documentação Mobile](mobile/README.md)

## 📱 Mobile Applications 🆕

O MedicWarehouse agora possui **aplicativos nativos** para dispositivos móveis:

### iOS App (Swift/SwiftUI)
- 🍎 **iPhone e iPad**: iOS 17.0+
- 🎨 **SwiftUI**: Interface nativa e moderna
- 📂 **Localização**: `mobile/ios/`
- 📖 **[Documentação →](mobile/ios/README.md)**

### Android App (Kotlin/Jetpack Compose)
- 🤖 **Android**: API 24+ (Android 7.0+)
- 🎨 **Jetpack Compose**: UI declarativa moderna
- 📂 **Localização**: `mobile/android/`
- 📖 **[Documentação →](mobile/android/README.md)**

**Funcionalidades Mobile:**
- ✅ Autenticação JWT (usuários e proprietários)
- ✅ Dashboard com estatísticas em tempo real
- ✅ Listagem de pacientes com busca
- ✅ Listagem de agendamentos com filtros
- 🚧 Criação/edição (em desenvolvimento)
- 🚧 Prontuários médicos (planejado)
- 🚧 Notificações push (planejado)

**[📚 Ver documentação completa dos apps mobile →](mobile/README.md)**

## 🖥️ Frontend Applications

O MedicWarehouse possui **dois aplicativos Angular independentes**:

### 1. **MedicWarehouse App** (`frontend/medicwarehouse-app`)
Aplicativo principal para uso diário das clínicas:
- 👨‍⚕️ **Usuários**: Proprietários de clínicas, médicos, secretárias, enfermeiros
- 📊 **Dashboard** da clínica individual
- 👥 **Gestão de pacientes** e prontuários
- 📅 **Agendamentos** e atendimentos
- 💊 **Prescrições** médicas e procedimentos
- 🎥 **Telemedicina** com videochamadas (NOVO!) 🆕
- 🌐 **Login por Subdomínio** - Acesso personalizado por clínica (NOVO!) 🆕
- 🧪 **Dados Mockados** - Desenvolvimento sem backend (NOVO!) 🆕
- **URL desenvolvimento**: `http://localhost:4200`
- **Exemplo com subdomínio**: `http://clinic1.localhost:4200`

### 2. **MW System Admin** (`frontend/mw-system-admin`) 🆕
Painel administrativo para gestão do sistema completo:
- 🔧 **Usuários**: System Owners (administradores do sistema)
- 🏥 **Gestão de todas as clínicas** (criar, ativar, desativar)
- 💰 **Métricas financeiras** (MRR, receitas, churn)
- 📊 **Analytics globais** do sistema
- ⚙️ **Controle de assinaturas** e override manual
- 👤 **Gestão de system owners**
- 🧪 **Dados Mockados** - Desenvolvimento sem backend (NOVO!) 🆕
- **URL desenvolvimento**: `http://localhost:4201`
- 📖 **Documentação**: [SYSTEM_OWNER_ACCESS.md](docs/SYSTEM_OWNER_ACCESS.md)

## 🎥 Microserviço de Telemedicina 🆕

**Novo microserviço independente para teleconsultas médicas!**

- 📂 **Localização**: `telemedicine/`
- 📖 **Documentação**: [`telemedicine/README.md`](telemedicine/README.md)
- 🎯 **Arquitetura**: Clean Architecture + DDD
- 🔐 **Multi-tenant**: Isolamento completo por TenantId
- 🎥 **Integração**: Daily.co (10.000 min/mês grátis)
- ✅ **Testes**: 22 testes unitários passando
- 💰 **Custo**: ~$30/mês para 1.000 consultas
- 🚀 **Status**: Pronto para produção

**Features:**
- ✅ Gestão de sessões de videochamada
- ✅ Tokens JWT para segurança
- ✅ Gravação de consultas (opcional)
- ✅ Rastreamento de duração
- ✅ API RESTful com Swagger
- ✅ HIPAA Compliant

**Guias:**
- [Análise de Serviços de Vídeo](docs/TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md)
- [Integração Frontend](telemedicine/FRONTEND_INTEGRATION.md)

## 🎫 Sistema de Chamados (Support Tickets) 🆕

**Sistema de suporte técnico integrado à API principal!**

- 📂 **API Endpoint**: `/api/tickets`
- 📖 **Documentação**: [`docs/TICKET_API_DOCUMENTATION.md`](docs/TICKET_API_DOCUMENTATION.md)
- 🔧 **Migrado de**: Microserviço SystemAdmin → API Principal
- 🎯 **Finalidade**: Bugs, suporte técnico, solicitação de recursos
- ✅ **Migração**: Script SQL e EF Core migration incluídos

**Features:**
- ✅ Criação e gerenciamento de tickets
- ✅ Comentários e atualizações
- ✅ Anexos de imagens (até 5MB)
- ✅ Atribuição para System Owners
- ✅ Rastreamento de status e histórico
- ✅ Estatísticas e métricas
- ✅ Comentários internos (visíveis apenas para admins)
- ✅ Múltiplos tipos: Bug, Feature Request, Suporte Técnico, etc.
- ✅ Prioridades: Low, Medium, High, Critical

**Guias:**
- [Documentação da API de Tickets](docs/TICKET_API_DOCUMENTATION.md)
- [Script de Migração](scripts/run-ticket-migration.sh)

## 📚 Documentação Completa

### 💼 Glossário de Termos Empresariais (NOVO!)
**Não entende termos da área empresarial? Comece por aqui!**
- 📖 **[`GLOSSARIO_TERMOS_EMPRESARIAIS.md`](docs/GLOSSARIO_TERMOS_EMPRESARIAIS.md)** - Glossário completo explicando todos os termos de negócio
- 💡 Aprenda sobre: SaaS, MRR, Churn, CAC, LTV, ROI, e muito mais
- 🎯 Ideal para: Empreendedores, donos de negócio, estudantes
- 📊 Exemplos práticos e aplicações reais

### 📱 Documentação Portátil
**Leia toda a documentação no celular ou gere um PDF completo!**
- 📂 **Localização**: `documentacao-portatil/`
- 📖 **Guia de uso**: [`documentacao-portatil/README.md`](documentacao-portatil/README.md)
- ✨ **O que faz**: Consolida todos os documentos em um único arquivo HTML responsivo ou PDF
- 🚀 **Como usar**: `cd documentacao-portatil && npm run gerar`

### 🌐 Documentação Web
Toda a documentação detalhada do projeto está disponível no **projeto web de documentação**:
- 📂 **Localização**: `frontend/mw-docs/src/assets/docs/`
- 🌐 **Documentação Web**: Execute o projeto mw-docs para visualizar
- 📖 **Principais Documentos**:
  - **[`PENDING_TASKS.md`](docs/PENDING_TASKS.md)** - ⭐ **NOVO!** Documento centralizado com todas as pendências e planejamento futuro (2025-2026)
  - **[`WAITING_QUEUE_GUIDE.md`](docs/WAITING_QUEUE_GUIDE.md)** - 🆕 **NOVO!** Guia completo da Fila de Espera com busca de pacientes avulsos
  - **[`PATIENT_CONSULTATION_IMPROVEMENTS.md`](docs/PATIENT_CONSULTATION_IMPROVEMENTS.md)** - 🆕 **NOVO!** Melhorias na consulta de pacientes e início rápido de atendimento
  - `GUIA_DESENVOLVIMENTO_AUTH.md` - **Guia de desenvolvimento e autenticação para MVP/testes** 🔥
  - `SYSTEM_SETUP_GUIDE.md` - **Guia completo de configuração e setup do sistema**
  - `BUSINESS_RULES.md` - Regras de negócio
  - `SUBSCRIPTION_SYSTEM.md` - Sistema de assinaturas
  - `SYSTEM_ADMIN_DOCUMENTATION.md` - Área do administrador do sistema
  - `PASSWORD_RECOVERY_FLOW.md` - Recuperação de senha com 2FA
  - `API_QUICK_GUIDE.md` - Guia rápido da API
  - `SECURITY_GUIDE.md` - Guia de segurança
  - `GUIA_EXECUCAO.md` - Como executar o projeto

## 🏗️ Arquitetura

O projeto segue os princípios do Domain-Driven Design (DDD) com arquitetura em camadas:

- **MedicSoft.Domain**: Entidades, Value Objects, Domain Services e Events
- **MedicSoft.Application**: CQRS com Commands/Queries, DTOs e Application Services  
- **MedicSoft.Repository**: Implementação do repositório com Entity Framework Core
- **MedicSoft.Api**: API RESTful com Swagger
- **MedicSoft.CrossCutting**: Serviços transversais (logging, segurança, etc.)
- **MedicSoft.Test**: Testes unitários e de integração

## 🚀 Funcionalidades

### 💳 Sistema de Assinaturas SaaS (NOVO!)
- ✅ **Planos de Assinatura**: Trial, Basic, Standard, Premium, Enterprise
- ✅ **Upgrade/Downgrade**: Upgrade cobra diferença imediata, downgrade na próxima cobrança
- ✅ **Congelamento de Plano**: Suspende cobrança e acesso por 1 mês
- ✅ **Validação de Pagamento**: Notificações automáticas via SMS, Email e WhatsApp
- ✅ **Bloqueio por Inadimplência**: Acesso bloqueado até regularização
- ✅ **Restauração Automática**: Acesso liberado após confirmação de pagamento

### 👥 Gestão de Usuários e Permissões
- ✅ **Múltiplos Perfis**: SystemAdmin, ClinicOwner, Doctor, Dentist, Nurse, Receptionist, Secretary
- ✅ **Controle de Acesso**: Permissões granulares por role
- ✅ **Limite de Usuários**: Validação automática baseada no plano
- ✅ **Administrador da Clínica**: Cadastro do dono com poderes completos
- ✅ **Cadastro de Médicos**: Suporte para CRM, especialidade, etc.
- ✅ **Área do System Owner**: Gestão completa de todas as clínicas (NOVO!)
  - Listagem de todas as clínicas com paginação
  - Analytics do sistema (MRR, churn, etc)
  - Gerenciamento de assinaturas
  - Ativação/Desativação de clínicas
  - Criação de administradores do sistema
- ✅ **Recuperação de Senha com 2FA**: (NOVO!)
  - Autenticação em duas etapas via SMS ou Email
  - Códigos de verificação de 6 dígitos
  - Tokens seguros com expiração de 15 minutos
  - Validação de força de senha

### 🎛️ Configuração de Módulos (NOVO!)
- ✅ **Módulos por Plano**: Recursos habilitados conforme o plano
- ✅ **Habilitar/Desabilitar**: Controle de módulos por clínica
- ✅ **Configuração Personalizada**: Parâmetros específicos por módulo

### 🏥 Gestão Clínica
- ✅ **Multitenant**: Isolamento de dados por consultório
- ✅ **Vínculo Multi-Clínica**: Paciente pode estar vinculado a múltiplas clínicas (N:N)
- ✅ **Busca Inteligente**: Busca de pacientes por CPF, Nome ou Telefone
- ✅ **Reutilização de Cadastro**: Sistema detecta cadastro prévio e vincula à nova clínica
- ✅ **Privacidade de Prontuários**: Cada clínica acessa apenas seus próprios prontuários
- ✅ **Templates**: Templates reutilizáveis para prontuários e prescrições médicas
- ✅ **Gestão Familiar**: Sistema de vínculo Responsável-Criança
  - Cadastro de crianças vinculadas a responsáveis adultos
  - Validações de idade e obrigatoriedade de responsável
  - Contato de emergência e autorização de atendimento
  - Visualização de vínculos familiares

### 💊 Medicamentos e Prescrições
- ✅ **Cadastro de Medicamentos**: Base completa com classificação ANVISA
- ✅ **Autocomplete**: Busca inteligente de medicamentos ao prescrever
- ✅ **Itens de Prescrição**: Vínculo de medicamentos com dosagem, frequência e duração
- ✅ **Medicamentos Controlados**: Identificação de substâncias controladas (Portaria 344/98)
- ✅ **Categorias**: Analgésico, Antibiótico, Anti-inflamatório, etc.

### 📝 Editor de Texto Rico e Autocomplete 🆕
- ✅ **Editor de Texto Rico**: Formatação avançada (negrito, itálico, listas, títulos)
- ✅ **Autocomplete de Medicações**: Digite `@@` para buscar medicações (130+ itens)
- ✅ **Autocomplete de Exames**: Digite `##` para buscar exames (150+ itens)
- ✅ **Navegação por Teclado**: ↑↓ para navegar, Enter para selecionar
- ✅ **Dados em PT-BR**: Base completa de medicações e exames brasileiros
- ✅ **Integração no Atendimento**: Campos de diagnóstico, prescrição e observações
- 📖 **Documentação**: [RICH_TEXT_EDITOR_AUTOCOMPLETE.md](docs/RICH_TEXT_EDITOR_AUTOCOMPLETE.md)

### 📅 Agendamentos e Atendimento
- ✅ **CRUD de Pacientes**: Cadastro completo com validações
- ✅ **Agendamento de Consultas**: Sistema completo de agendamentos
- ✅ **Agenda Diária**: Visualização da agenda com slots disponíveis
- ✅ **Visualização em Calendário**: Navegação mensal com indicadores
- ✅ **Atendimento ao Paciente**: Tela completa de atendimento com prontuário
- ✅ **Timer de Consulta**: Cronômetro automático para controle do tempo
- ✅ **Prontuário Médico**: Registro de diagnóstico, prescrição e observações
- ✅ **Histórico do Paciente**: Timeline de consultas anteriores
- ✅ **Prescrição Médica**: Área de texto com impressão otimizada
- ✅ **Encaixes**: Permite agendamentos de emergência

### 🏥 Conformidade CFM 1.821/2007 - Prontuário Eletrônico 🆕

**Sistema 100% conforme a Resolução CFM 1.821/2007 sobre prontuários eletrônicos!**

#### Anamnese Estruturada (Campos Obrigatórios)
- ✅ **Queixa Principal**: Campo obrigatório com validação de 10+ caracteres
- ✅ **História da Doença Atual (HDA)**: Descrição detalhada com validação de 50+ caracteres
- ✅ **História Patológica Pregressa (HPP)**: Registro de histórico médico do paciente
- ✅ **História Familiar**: Antecedentes familiares relevantes
- ✅ **Hábitos de Vida**: Tabagismo, etilismo, atividade física, etc.
- ✅ **Medicações em Uso**: Lista de medicamentos atuais do paciente

#### Exame Clínico Completo
- ✅ **Sinais Vitais Obrigatórios**:
  - Pressão Arterial (Sistólica/Diastólica): 50-300/30-200 mmHg
  - Frequência Cardíaca: 30-220 bpm
  - Frequência Respiratória: 8-60 irpm
  - Temperatura: 32-45°C
  - Saturação de O2: 0-100%
- ✅ **Exame Físico Sistemático**: Descrição obrigatória (mín. 20 caracteres)
- ✅ **Estado Geral**: Registro do estado geral do paciente

#### Hipóteses Diagnósticas com CID-10
- ✅ **Diagnósticos Estruturados**: Suporte a múltiplos diagnósticos por atendimento
- ✅ **Código CID-10 Validado**: Validação automática de formato (ex: A00, J20.9, Z99.01)
- ✅ **Tipificação**: Principal ou Secundário
- ✅ **Data do Diagnóstico**: Registro temporal de cada hipótese

#### Plano Terapêutico Detalhado
- ✅ **Tratamento/Conduta**: Descrição obrigatória (mín. 20 caracteres)
- ✅ **Prescrição Medicamentosa**: Lista detalhada de medicamentos prescritos
- ✅ **Solicitação de Exames**: Exames complementares solicitados
- ✅ **Encaminhamentos**: Referências para outros especialistas
- ✅ **Orientações ao Paciente**: Instruções e cuidados
- ✅ **Data de Retorno**: Agendamento de retorno automático

#### Consentimento Informado
- ✅ **Termo de Consentimento**: Registro de consentimento do paciente
- ✅ **Aceite Digital**: Registro de aceite com data/hora
- ✅ **Rastreabilidade**: IP de origem e assinatura digital (opcional)
- ✅ **Histórico Completo**: Todos os consentimentos registrados

#### Auditoria e Controle
- ✅ **Fechamento de Prontuário**: Impede alterações após finalização
- ✅ **Identificação Profissional**: Médico responsável (CRM/UF)
- ✅ **Timestamps Completos**: Data/hora de criação e modificação
- ✅ **Isolamento Multi-tenant**: Segurança e privacidade garantidas
- ✅ **Guarda de 20 anos**: Soft-delete sem exclusão física

📖 **Documentação Completa**:
- [Especificação CFM 1.821](docs/ESPECIFICACAO_CFM_1821.md)
- [Implementação Detalhada](docs/CFM_1821_IMPLEMENTACAO.md)
- [Phase 3 - Backend Complete](docs/PHASE_3_BACKEND_COMPLETE.md)
- [Phase 4 - Frontend Complete](docs/PHASE_4_FRONTEND_COMPLETE.md)

### 💳 Assinaturas e Cobrança
- ✅ **Período de Teste**: 15 dias gratuitos para novas clínicas
- ✅ **Planos Flexíveis**: Trial, Basic, Standard, Premium, Enterprise
- ✅ **Gestão de Assinaturas**: Ativação, suspensão, cancelamento
- ✅ **Controle de Pagamentos**: Registro de pagamentos e renovações
- ✅ **Status de Assinatura**: Trial, Active, Suspended, PaymentOverdue, Cancelled

### 💰 Sistema de Pagamentos e Nota Fiscal
- ✅ **Múltiplos Métodos de Pagamento**: Dinheiro, Cartão de Crédito, Cartão de Débito, PIX, Transferência Bancária, Cheque
- ✅ **Fluxo de Pagamento Completo**: Pendente → Processando → Pago → Reembolsado/Cancelado
- ✅ **Gestão de Pagamentos**: Processar, reembolsar, cancelar pagamentos
- ✅ **Emissão de Notas Fiscais**: Criar, emitir, enviar, cancelar nota fiscal
- ✅ **Ciclo de Vida de NF**: Rascunho → Emitida → Enviada → Paga → Cancelada
- ✅ **Controle de Vencimento**: Identificação de notas vencidas com cálculo de dias
- ✅ **Vínculo Automático**: Pagamento vinculado à consulta ou assinatura
- ✅ **Histórico Completo**: Rastreamento de todas as transações financeiras
- ✅ **API RESTful**: Endpoints completos para integração de pagamentos

### 💼 Gestão Financeira e Contas a Pagar
- ✅ **Controle de Despesas**: CRUD completo de contas a pagar
- ✅ **Categorias de Despesas**: Aluguel, Utilidades, Materiais, Equipamentos, Salários, Impostos, etc.
- ✅ **Status de Despesas**: Pendente, Pago, Vencido, Cancelado
- ✅ **Controle de Vencimento**: Alertas automáticos de despesas vencidas
- ✅ **Fornecedores**: Cadastro de fornecedores com documento
- ✅ **Múltiplos Métodos de Pagamento**: Suporte a todos os métodos de pagamento

### 📊 Relatórios e Dashboards Financeiros
- ✅ **Resumo Financeiro**: Receitas, despesas e lucro líquido por período
- ✅ **Relatórios de Receita**: Breakdown diário de faturamento
- ✅ **Relatórios de Agendamentos**: Estatísticas de consultas por status e tipo
- ✅ **Relatórios de Pacientes**: Crescimento de base de pacientes
- ✅ **Contas a Receber**: Controle de pagamentos pendentes e vencidos
- ✅ **Contas a Pagar**: Controle de despesas pendentes e vencidas
- ✅ **Análise por Método de Pagamento**: Distribuição de receitas por forma de pagamento
- ✅ **Análise por Categoria**: Distribuição de despesas por categoria
- ✅ **API RESTful**: Endpoints completos para geração de relatórios

### 📲 Notificações
- ✅ **SMS**: Integração preparada para envio de SMS
- ✅ **WhatsApp**: Interface para WhatsApp Business API
- ✅ **Email**: Envio de emails transacionais
- ✅ **Lembretes Automáticos**: Confirmação de agendamento 24h antes
- ✅ **Retry Logic**: Até 3 tentativas para notificações falhadas
- ✅ **Múltiplos Canais**: SMS, WhatsApp, Email, Push
- ✅ **Rotinas Configuráveis**: Sistema completo de automação de notificações
  - Agendamento Flexível: Diário, Semanal, Mensal, Custom, Antes/Depois de Eventos
  - Templates Personalizáveis: Mensagens com placeholders dinâmicos
  - Filtros de Destinatários: Segmentação baseada em critérios configuráveis
  - Escopo Multi-nível: Clínica ou Sistema (admin)
  - Até 10 retentativas configuráveis
  - [📚 Documentação Completa](docs/NOTIFICATION_ROUTINES_DOCUMENTATION.md)
  - [💡 Exemplos de Uso](docs/NOTIFICATION_ROUTINES_EXAMPLE.md)

### 🏥 Procedimentos e Serviços
- ✅ **Cadastro de Procedimentos**: Nome, código, categoria, preço, duração
- ✅ **CRUD Completo**: API RESTful para gerenciar procedimentos
- ✅ **Gestão de Materiais**: Controle de estoque com entrada e saída
- ✅ **Vínculo Procedimento-Consulta**: Registro completo por atendimento
- ✅ **Fechamento de Conta**: Resumo de billing com valores discriminados
- ✅ **Controle de Estoque**: Alerta de estoque mínimo
- ✅ **Categorias**: Consulta, Exame, Cirurgia, Terapia, Vacinação, Retorno, etc.
- ✅ **Múltiplos Procedimentos**: Adicionar vários procedimentos ao mesmo atendimento
- ✅ **Cálculo Automático**: Total calculado automaticamente baseado nos procedimentos

### 🔐 Segurança e Administração
- ✅ **BCrypt Password Hashing**: Senhas hashadas com BCrypt (work factor 12)
- ✅ **Rate Limiting**: Proteção contra força bruta e DDoS
- ✅ **Security Headers**: CSP, X-Frame-Options, HSTS, e mais
- ✅ **Input Sanitization**: Proteção contra XSS e injection attacks
- ✅ **CORS Seguro**: Origens específicas por ambiente
- ✅ **Multi-tenant Isolation**: Isolamento completo de dados por tenant
- ✅ **Painel do Dono da Clínica**: Gestão completa de usuários e configurações
- ✅ **Painel do Sistema**: Administração master para dono do sistema
- ✅ **Gestão de Permissões**: Controle granular de acesso
- ✅ **Auditoria**: Log completo de operações

> 📖 **Para detalhes completos de segurança**, consulte [SECURITY_GUIDE.md](docs/SECURITY_GUIDE.md)

### 📊 Relatórios e Integrações
- ✅ **Swagger**: Documentação interativa da API
- ✅ **Podman**: Containerização completa (livre e open-source)
- ✅ **Relatórios Financeiros**: Dashboards completos de receitas, despesas e lucro
- ✅ **Relatórios Operacionais**: Agendamentos, pacientes e performance
- ✅ **Contas a Receber e Pagar**: Controle completo de fluxo de caixa
- 🚧 **TISS Export**: Integração com padrão TISS (em planejamento)

## 🔧 Tecnologias

- **Backend**: .NET 8, Entity Framework Core, PostgreSQL (Npgsql)
- **Frontend**: Angular 20, TypeScript, SCSS
- **Banco de Dados**: PostgreSQL 16 (via Podman) - Migrado de SQL Server com economia de 90%+
- **Containerização**: Podman e Podman Compose (livre e open-source)
- **Autenticação**: JWT (stateless)
- **Arquitetura**: DDD + Clean Architecture

## 🏃‍♂️ Como Executar

> 🚀 **NOVO!** [**GUIA_INICIO_RAPIDO_LOCAL.md**](docs/GUIA_INICIO_RAPIDO_LOCAL.md) - **Setup em 10 minutos para testar HOJE!**

> 🔧 **IMPORTANTE!** [**LOCALHOST_SETUP_FIX.md**](docs/LOCALHOST_SETUP_FIX.md) - **Fix aplicado para executar em localhost** - Leia se tiver problemas com autenticação ou dados vazios

> 📖 **Para um guia completo e detalhado**, consulte o arquivo [GUIA_EXECUCAO.md](docs/GUIA_EXECUCAO.md)

> ✅ **NOVO!** [**CHECKLIST_TESTES_COMPLETO.md**](docs/CHECKLIST_TESTES_COMPLETO.md) - Teste todos os 80+ endpoints e funcionalidades

> 📊 **NOVO!** [**RESUMO_SISTEMA_COMPLETO.md**](docs/RESUMO_SISTEMA_COMPLETO.md) - Visão geral de tudo que está implementado

### 🔑 Primeiros Passos - Criando Usuários Iniciais (IMPORTANTE para MVP)

**Problema**: Para testar o sistema, você precisa de autenticação, mas não consegue criar o primeiro usuário sem autenticação.

**Solução**: Use os **endpoints de desenvolvimento** para criar usuários iniciais sem autenticação:

```bash
# 1. Criar um System Owner (administrador do sistema)
POST http://localhost:5000/api/data-seeder/seed-system-owner

# Credenciais criadas:
# Username: admin
# Password: Admin@123
# TenantId: system

# 2. Fazer login
POST http://localhost:5000/api/auth/owner-login
{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "system"
}

# 3. Usar o token retornado para acessar endpoints protegidos
```

**Ou criar dados completos de demonstração:**
```bash
# Cria clínica, usuários, pacientes, agendamentos, etc.
POST http://localhost:5000/api/data-seeder/seed-demo

# Credenciais criadas:
# - admin / Admin@123 (SystemAdmin)
# - dr.silva / Doctor@123 (Doctor)  
# - recep.maria / Recep@123 (Receptionist)
```

> 📖 **Para mais detalhes sobre autenticação e desenvolvimento**, consulte:
> - [GUIA_DESENVOLVIMENTO_AUTH.md](docs/GUIA_DESENVOLVIMENTO_AUTH.md) - Guia completo para desenvolvimento e testes
> - [AUTHENTICATION_GUIDE.md](docs/AUTHENTICATION_GUIDE.md) - Documentação completa de autenticação
> - [CARGA_INICIAL_TESTES.md](docs/CARGA_INICIAL_TESTES.md) - Detalhes sobre dados de teste

### Pré-requisitos

- Podman e Podman Compose (ou Docker como alternativa)
- .NET 8 SDK (para desenvolvimento)
- Node.js 18+ (para desenvolvimento)

### Executar com Podman

```bash
# Clone o repositório
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code

# Execute com Podman Compose
podman-compose up -d

# Ou, se preferir usar Docker como alternativa:
# docker-compose up -d

# A API estará disponível em: http://localhost:5000
# O Frontend estará disponível em: http://localhost:4200
# Swagger UI estará disponível em: http://localhost:5000/swagger
```

### Executar para Desenvolvimento

#### Backend (.NET API)

```bash
# Restaurar dependências
dotnet restore

# Executar a API
cd src/MedicSoft.Api
dotnet run

# A API estará disponível em: https://localhost:7107
# Swagger UI estará disponível em: https://localhost:7107/swagger
```

#### Frontend (Angular)

**MedicWarehouse App** (aplicativo principal das clínicas):
```bash
# Navegar para o frontend
cd frontend/medicwarehouse-app

# Instalar dependências
npm install

# Executar em modo de desenvolvimento
npm start

# O frontend estará disponível em: http://localhost:4200
```

**MW System Admin** (painel administrativo do sistema) 🆕:
```bash
# Navegar para o system admin
cd frontend/mw-system-admin

# Instalar dependências
npm install

# Executar em modo de desenvolvimento (porta 4201)
npm start

# O admin estará disponível em: http://localhost:4201
```

> 📖 **Documentação completa do System Admin**: [SYSTEM_OWNER_ACCESS.md](docs/SYSTEM_OWNER_ACCESS.md)

#### Banco de Dados (PostgreSQL)

```bash
# Executar apenas o PostgreSQL via Podman
podman-compose up postgres -d

# Ou executar PostgreSQL standalone com Podman:
podman run -d \
  --name medicwarehouse-postgres \
  -e POSTGRES_DB=medicwarehouse \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine

# Aplicar migrations
dotnet ef database update --context MedicSoftDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api
```

> 📖 **Guia completo de setup do PostgreSQL**: [PODMAN_POSTGRES_SETUP.md](docs/PODMAN_POSTGRES_SETUP.md)  
> 📖 **Detalhes da migração SQL Server → PostgreSQL**: [MIGRACAO_POSTGRESQL.md](docs/MIGRACAO_POSTGRESQL.md)

## 📖 Documentação da API

Após executar a aplicação, acesse a documentação interativa do Swagger:

- **Swagger UI (Docker)**: http://localhost:5000/swagger
- **Swagger UI (Desenvolvimento local)**: https://localhost:7107/swagger

### 📮 Coleção Postman

Para facilitar o teste e integração, todas as APIs foram exportadas para o Postman:

- 📥 **Arquivo**: [`MedicWarehouse-Postman-Collection.json`](MedicWarehouse-Postman-Collection.json)
- 📖 **Guia de Importação**: [`POSTMAN_IMPORT_GUIDE.md`](docs/POSTMAN_IMPORT_GUIDE.md)
- ✨ **Recursos incluídos**:
  - Todos os endpoints organizados por funcionalidade
  - Variáveis pré-configuradas (base_url, tenant_id)
  - Headers pré-configurados
  - Exemplos de requests prontos para uso

**Como usar:**
1. Importe o arquivo no Postman
2. Configure as variáveis da coleção
3. Teste os endpoints!

Para instruções detalhadas, consulte o [Guia de Importação do Postman](docs/POSTMAN_IMPORT_GUIDE.md).

> 📖 **Guia Completo**: Para um passo a passo detalhado de como configurar e cadastrar tudo no sistema, consulte o [Guia de Configuração do Sistema](frontend/mw-docs/src/assets/docs/SYSTEM_SETUP_GUIDE.md).

### Endpoints Principais

- **Autenticação** 🔐:
  - `POST /api/auth/login` - Login de usuários (doctors, secretaries, etc.)
  - `POST /api/auth/owner-login` - Login de proprietários (clinic owners e system owners)
  - `POST /api/auth/validate` - Validar token JWT
  - 📖 **Veja**: [`AUTHENTICATION_GUIDE.md`](docs/AUTHENTICATION_GUIDE.md) para detalhes completos

- **Registro e Configuração**:
  - `POST /api/registration` - Registro de nova clínica
  - `GET /api/registration/check-cnpj/{cnpj}` - Verificar disponibilidade de CNPJ
  - `GET /api/registration/check-username/{username}` - Verificar disponibilidade de username

- **Pacientes**:
  - `GET /api/patients` - Listar pacientes
  - `GET /api/patients/{id}` - Obter paciente por ID
  - `GET /api/patients/search?searchTerm={termo}` - Buscar por CPF, Nome ou Telefone
  - `GET /api/patients/by-document/{cpf}` - Buscar por CPF em todas as clínicas
  - `POST /api/patients` - Criar novo paciente (com suporte a guardianId para crianças)
  - `PUT /api/patients/{id}` - Atualizar paciente
  - `DELETE /api/patients/{id}` - Excluir paciente
  - `POST /api/patients/{patientId}/link-clinic/{clinicId}` - Vincular paciente à clínica
  - `POST /api/patients/{childId}/link-guardian/{guardianId}` - 🆕 Vincular criança a responsável
  - `GET /api/patients/{guardianId}/children` - 🆕 Listar filhos de um responsável

- **Agendamentos**:
  - `POST /api/appointments` - Criar agendamento
  - `GET /api/appointments/{id}` - Obter agendamento por ID
  - `PUT /api/appointments/{id}/cancel` - Cancelar agendamento
  - `GET /api/appointments/agenda` - Agenda diária
  - `GET /api/appointments/available-slots` - Horários disponíveis

- **Prontuários Médicos**:
  - `POST /api/medical-records` - Criar prontuário
  - `PUT /api/medical-records/{id}` - Atualizar prontuário
  - `POST /api/medical-records/{id}/complete` - Finalizar atendimento
  - `GET /api/medical-records/appointment/{appointmentId}` - Buscar por agendamento
  - `GET /api/medical-records/patient/{patientId}` - Histórico do paciente

- **Procedimentos e Serviços** 🆕:
  - `GET /api/procedures` - Listar todos os procedimentos da clínica
  - `GET /api/procedures/{id}` - Obter procedimento por ID
  - `POST /api/procedures` - Criar novo procedimento
  - `PUT /api/procedures/{id}` - Atualizar procedimento
  - `DELETE /api/procedures/{id}` - Desativar procedimento
  - `POST /api/procedures/appointments/{appointmentId}/procedures` - Adicionar procedimento ao atendimento
  - `GET /api/procedures/appointments/{appointmentId}/procedures` - Listar procedimentos do atendimento
  - `GET /api/procedures/appointments/{appointmentId}/billing-summary` - 💰 Resumo de cobrança com total

- **Despesas (Contas a Pagar)**:
  - `GET /api/expenses` - Listar despesas (filtros: clinicId, status, category)
  - `GET /api/expenses/{id}` - Obter despesa por ID
  - `POST /api/expenses` - Criar nova despesa
  - `PUT /api/expenses/{id}` - Atualizar despesa
  - `PUT /api/expenses/{id}/pay` - Marcar despesa como paga
  - `PUT /api/expenses/{id}/cancel` - Cancelar despesa
  - `DELETE /api/expenses/{id}` - Excluir despesa

- **Relatórios e Dashboards**:
  - `GET /api/reports/financial-summary` - Resumo financeiro completo (receitas, despesas, lucro)
  - `GET /api/reports/revenue` - Relatório de receita com breakdown diário
  - `GET /api/reports/appointments` - Relatório de agendamentos (estatísticas, status, tipos)
  - `GET /api/reports/patients` - Relatório de crescimento de pacientes
  - `GET /api/reports/accounts-receivable` - Contas a receber (pendentes e vencidas)
  - `GET /api/reports/accounts-payable` - Contas a pagar (pendentes e vencidas)

- **Data Seeding (Dados de Teste)** 🆕:
  - `GET /api/data-seeder/demo-info` - Informações sobre os dados demo
  - `POST /api/data-seeder/seed-demo` - 🔧 Gerar dados de teste completos com garantia de consistência
    - ✅ **Transacional**: Todas as operações em uma transação (rollback automático em caso de erro)
    - ✅ **Consistente**: Datas e relacionamentos validados
    - ✅ **Completo**: Cria clínica demo com TenantId: `demo-clinic-001`
    - ✅ Cria 3 usuários: Admin, Médico e Recepcionista
    - ✅ Cria 6 pacientes (incluindo 2 crianças com responsável)
    - ✅ Cria 8 procedimentos diversos
    - ✅ Cria 5 agendamentos com histórico (passados, presente e futuros)
    - ✅ Cria prontuários médicos, prescrições, exames e pagamentos de exemplo
  - `DELETE /api/data-seeder/clear-database` - 🧹 Limpar dados demo (transacional)

## 🧪 Testes

O projeto possui ampla cobertura de testes unitários e de integração.

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~ProcedureTests"
```

### Estatísticas de Testes

- ✅ **719 testes** implementados
- ✅ **100% de cobertura** nas entidades de domínio
- ✅ **Testes de Validação**: Regras de negócio e validações
- ✅ **Testes de Comportamento**: Fluxos e estados das entidades
- ✅ **Novos Testes** 🆕:
  - 23 testes para entidade Procedure
  - 15 testes para entidade AppointmentProcedure
  - Validações de preços, durações e vínculos

## 🗃️ Estrutura do Banco de Dados

### Tabelas Principais

- **Patients**: Dados dos pacientes (🆕 incluindo GuardianId para crianças)
- **Clinics**: Informações dos consultórios
- **PatientClinicLinks**: Vínculos N:N entre pacientes e clínicas
- **Appointments**: Agendamentos de consultas
- **MedicalRecords**: Prontuários médicos e histórico de atendimentos (isolados por clínica)
- **MedicalRecordTemplates**: Templates reutilizáveis para prontuários
- **PrescriptionTemplates**: Templates reutilizáveis para prescrições
- **Procedures** 🆕: Procedimentos/serviços oferecidos pela clínica
- **AppointmentProcedures** 🆕: Vínculo de procedimentos realizados em atendimentos
- **Materials**: Materiais e insumos com controle de estoque
- **ProcedureMaterials**: Vínculo de materiais necessários para procedimentos
- **Payments**: Pagamentos de consultas e assinaturas
- **Invoices**: Notas fiscais e comprovantes

### Multitenancy

O sistema utiliza **multitenancy** por coluna `TenantId`, garantindo isolamento de dados entre diferentes consultórios.

**Importante**: 
- Pacientes podem estar vinculados a múltiplas clínicas (N:N)
- Dados cadastrais são compartilhados entre clínicas vinculadas
- Prontuários médicos são **isolados por clínica** - cada clínica vê apenas seus próprios registros
- Sistema detecta cadastro prévio por CPF e reutiliza dados, criando novo vínculo

Para mais detalhes sobre as regras de negócio, consulte [BUSINESS_RULES.md](docs/BUSINESS_RULES.md)

## 📱 Interface e Telas

Para visualizar todas as telas do sistema com descrições detalhadas e fluxos de navegação, consulte:
- **[SCREENS_DOCUMENTATION.md](docs/SCREENS_DOCUMENTATION.md)** - Documentação completa de todas as interfaces com diagramas de fluxo
- **[docs/VISUAL_FLOW_SUMMARY.md](docs/VISUAL_FLOW_SUMMARY.md)** - Resumo visual rápido com diagramas Mermaid interativos

Este documento inclui:
- Mockups ASCII de todas as telas
- Diagramas Mermaid de fluxos de navegação (renderizados automaticamente pelo GitHub)
- Descrição detalhada de funcionalidades
- Estados e transições de agendamentos
- Padrões de interface e componentes

### Principais Fluxos Documentados:
1. **Fluxo de Primeiro Atendimento**: Dashboard → Novo Paciente → Cadastro → Agendamento → Atendimento
2. **Fluxo de Paciente Recorrente**: Dashboard → Agenda → Atendimento (com histórico visível)
3. **Fluxo de Vínculo Multi-Clínica**: Busca por CPF → Detecta cadastro existente → Vincula à clínica atual

## 🔐 Segurança

O MedicWarehouse implementa múltiplas camadas de segurança para proteger dados sensíveis:

### Implementações de Segurança

- **JWT Authentication**: Autenticação baseada em tokens com HMAC-SHA256 encryption
  - Endpoints: `POST /api/auth/login` e `POST /api/auth/owner-login`
  - Token expiration: 60 minutos (configurável)
  - Zero clock skew - tokens expirados são rejeitados imediatamente
  - Claims incluem: username, role, tenant_id, clinic_id, is_system_owner
  - Validação completa: issuer, audience, signature, lifetime
- **BCrypt Password Hashing**: Senhas hashadas com BCrypt (work factor 12)
- **Rate Limiting**: Proteção contra ataques de força bruta (10 req/min em produção)
- **Security Headers**: CSP, X-Frame-Options, HSTS, X-Content-Type-Options, etc.
- **Input Sanitization**: Proteção contra XSS e injection attacks
- **CORS Seguro**: Origens específicas configuradas por ambiente
- **Tenant Isolation**: Isolamento automático de dados por tenant com query filters globais
- **SQL Injection Protection**: Entity Framework Core com queries parametrizadas
- **HTTPS Enforcement**: HTTPS obrigatório em produção com HSTS
- **Environment-based Config**: Secrets via variáveis de ambiente, nunca hardcoded

### Testes de Segurança

- **719 testes** passando e aprovados (incluindo testes de JWT, validações e segurança)
- Cobertura de JWT token generation/validation, password hashing, input sanitization e validações
- 100% de taxa de sucesso

### Documentação Completa

Para detalhes completos sobre segurança, autenticação e melhores práticas:
- 📖 **[AUTHENTICATION_GUIDE.md](docs/AUTHENTICATION_GUIDE.md)** - Guia completo de autenticação JWT
- 📖 **[SECURITY_GUIDE.md](frontend/mw-docs/src/assets/docs/SECURITY_GUIDE.md)** - Guia completo de segurança

## 🚀 Deploy e Infraestrutura de Produção

### 💰 Infraestrutura com Baixo Custo (NOVO!) 🔥

**Documentação completa para produção com custo mínimo ($5-20/mês) enquanto você não tem clientes grandes!**

#### 📚 [INFRA_DOCS_INDEX.md](docs/INFRA_DOCS_INDEX.md) - **COMECE AQUI!**
Índice completo com todos os guias de infraestrutura. Navegação fácil para encontrar o que você precisa.

#### 🚀 Guias Principais:

- **[QUICK_START_PRODUCTION.md](docs/QUICK_START_PRODUCTION.md)** - ⚡ **Do Zero ao Ar em 30 Minutos**
  - Setup rápido com Railway ou VPS
  - Passo a passo simplificado
  - Para quem quer resultados AGORA

- **[CALCULADORA_CUSTOS.md](docs/CALCULADORA_CUSTOS.md)** - 💵 **Planeje Seus Custos**
  - Estimativas por número de clínicas (1-500+)
  - Comparação Railway vs VPS vs Cloud
  - Projeção de crescimento e ROI

- **[INFRA_PRODUCAO_BAIXO_CUSTO.md](docs/INFRA_PRODUCAO_BAIXO_CUSTO.md)** - 📋 **Guia Completo**
  - 💚 **Railway + Vercel** (Recomendado) - $5-20/mês
  - 🔧 **VPS (Hetzner/DigitalOcean)** - $5-10/mês
  - 🆓 **Free Tier** - $0/mês (apenas testes)
  - Comparativos, estratégias de escala, backups

- **[DEPLOY_RAILWAY_GUIDE.md](docs/DEPLOY_RAILWAY_GUIDE.md)** - 🚂 **Deploy no Railway**
  - Passo a passo detalhado
  - PostgreSQL incluído
  - SSL e backups automáticos

- **[MIGRACAO_POSTGRESQL.md](docs/MIGRACAO_POSTGRESQL.md)** - 🔄 **Economize 90%+ em Banco**
  - Migração SQL Server → PostgreSQL
  - Guia técnico completo
  - Scripts e validação

### Usando Podman (Desenvolvimento/VPS)

**Desenvolvimento:**
```bash
# Build e iniciar
podman-compose up -d

# A API estará em: http://localhost:5000
# Frontend em: http://localhost:4200
```

**Produção (VPS):**
```bash
# Usar compose otimizado para produção
podman-compose -f podman-compose.production.yml up -d

# Ver logs
podman-compose -f podman-compose.production.yml logs -f
```

> **Nota:** Os arquivos compose também funcionam com Docker (`docker-compose`) como alternativa.

### Configuração de Produção

📋 **Checklist de Setup:**
- [ ] Copiar `.env.example` para `.env` e configurar
- [ ] Gerar `JWT_SECRET_KEY` forte (32+ caracteres)
- [ ] Configurar `POSTGRES_PASSWORD` segura
- [ ] Atualizar `CORS` com domínios corretos
- [ ] Configurar backups automáticos
- [ ] Habilitar HTTPS (SSL/TLS)
- [ ] Configurar monitoramento de logs

Para detalhes completos, veja: [INFRA_PRODUCAO_BAIXO_CUSTO.md](docs/INFRA_PRODUCAO_BAIXO_CUSTO.md)

## 🔄 CI/CD

O projeto utiliza **GitHub Actions** para integração e entrega contínuas. O workflow executa automaticamente:

- ✅ **Testes Backend**: Executa todos os 305 testes unitários do .NET
- ✅ **Testes Frontend**: Executa testes do Angular com Karma/Jasmine
- ✅ **Build Verification**: Verifica se o build está funcional
- ✅ **Code Coverage**: Gera relatórios de cobertura de código
- ✅ **SonarCloud Analysis**: Análise de qualidade de código para backend e frontend

O workflow é executado automaticamente em:
- Push para as branches `main` e `develop`
- Pull Requests para as branches `main` e `develop`
- Execução manual via GitHub Actions

Para mais detalhes, consulte: [CI_CD_DOCUMENTATION.md](docs/CI_CD_DOCUMENTATION.md)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Contato

- **Projeto**: MedicWarehouse
- **Email**: contato@medicwarehouse.com
- **GitHub**: [https://github.com/MedicWarehouse/MW.Code](https://github.com/MedicWarehouse/MW.Code)
## 🌐 MW.Site - Marketing Website

**Novo!** Site de marketing e contratação do MedicWarehouse SaaS.

### Características

- **Angular 20**: Framework moderno e performático
- **Landing Page**: Home page com hero, features e benefícios
- **Pricing Page**: 4 planos disponíveis (Básico, Médio, Premium, Personalizado)
- **Registro Completo**: Wizard de 5 etapas para cadastro de clínicas
- **Período de Teste**: 15 dias gratuitos automaticamente
- **Integração WhatsApp**: Contato direto via WhatsApp
- **Carrinho de Compras**: Visualização antes de finalizar cadastro
- **Responsive Design**: Funciona perfeitamente em mobile, tablet e desktop

### Planos Disponíveis

| Plano | Preço | Usuários | Pacientes | Recursos |
|-------|-------|----------|-----------|----------|
| **Básico** | R$ 190/mês | 2 | 100 | Funcionalidades básicas |
| **Médio** ⭐ | R$ 240/mês | 3 | 300 | WhatsApp + Relatórios |
| **Premium** | R$ 320/mês | 5 | Ilimitados | Todos os recursos + SMS + TISS |
| **Personalizado** | Sob consulta | Customizado | Ilimitados | Recursos exclusivos |

### Como Executar MW.Site

```bash
# Navegar para o diretório
cd frontend/mw-site

# Instalar dependências
npm install

# Executar em modo de desenvolvimento
npm start

# Build para produção
npm run build
```

Acesse: `http://localhost:4200`

### Documentação Completa

Para informações detalhadas sobre MW.Site, consulte: [MW_SITE_DOCUMENTATION.md](docs/MW_SITE_DOCUMENTATION.md)

### API Endpoints para MW.Site

#### Registro de Nova Clínica
```bash
POST /api/registration
```

#### Verificar CNPJ
```bash
GET /api/registration/check-cnpj/{cnpj}
```

#### Verificar Username
```bash
GET /api/registration/check-username/{username}
```

#### Enviar Formulário de Contato
```bash
POST /api/contact
```

