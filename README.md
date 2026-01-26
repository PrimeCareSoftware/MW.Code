# PrimeCare Software - Sistema de Gestão para Consultórios Médicos

[![CI - Test Frontend e Backend](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml/badge.svg)](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml)

> 📚 **NOVO!** Toda a documentação foi reorganizada! [→ Ver Central de Documentação](system-admin/README.md) | [→ Ver Índice Completo](system-admin/INDICE.md)

> 📊 **NOVO!** Quer ver tudo que foi desenvolvido? [→ Ver Resumo Técnico Completo](system-admin/docs/RESUMO_TECNICO_COMPLETO.md) - Visão geral de **92% de completude do sistema**!

> 🚀 **NOVO!** Quer rodar o sistema localmente AGORA? [→ Ver Guia de Início Rápido](system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md) - Setup em menos de 10 minutos!

> 🌍 **NOVO!** Desenvolva em **macOS, Windows ou Linux**! [→ Ver Guia Multiplataforma](system-admin/guias/GUIA_MULTIPLATAFORMA.md) - Compatibilidade total garantida!

> 🐳 **NOVO!** Migramos para **Podman** (100% gratuito e open-source)! Docker ainda é suportado. [→ Ver Guia de Migração](system-admin/infrastructure/DOCKER_TO_PODMAN_MIGRATION.md)

> ⚠️ **AÇÃO NECESSÁRIA**: GitHub Pages precisa ser habilitado para deploy da documentação. [Ver instruções →](system-admin/docs/GITHUB_PAGES_SETUP_REQUIRED.md)

Uma solução **DDD** multitenant completa para gestão de consultórios médicos (SaaS) construída com **Angular 20**, **.NET 8** e **PostgreSQL**.

## 📊 Status do Projeto

| Métrica | Valor |
|---------|-------|
| **Completude Geral** | ✅ **95%** |
| **Controllers Backend** | 50+ |
| **Componentes Frontend** | 171+ |
| **Apps Mobile** | ❌ Descontinuados (migrados para PWA) |
| **Microserviços** | 1 (Telemedicina) - Demais descontinuados |
| **Testes Automatizados** | 792+ |
| **Documentos** | 49+ |

[→ Ver Resumo Técnico Completo](system-admin/docs/RESUMO_TECNICO_COMPLETO.md)

## 🌍 Compatibilidade Multiplataforma

O PrimeCare Software é **100% cross-platform**:

- ✅ **macOS**: Script automatizado de setup (`setup-macos.sh`)
- ✅ **Windows**: Script PowerShell de setup (`setup-windows.ps1`)
- ✅ **Linux**: Suporte completo (Ubuntu, Fedora, Debian, etc.)
- 📖 **[Guia Completo](system-admin/guias/GUIA_MULTIPLATAFORMA.md)**: Instruções detalhadas para cada plataforma

> ✅ **NOVO**: Sistema migrado para PostgreSQL! Economia de 90-96% em custos de infraestrutura. [Ver detalhes →](system-admin/infrastructure/MIGRACAO_POSTGRESQL.md)

> 🎯 **NOVO**: Login por Subdomínio! Acesse sua clínica via `clinic1.mwsistema.com.br` sem precisar digitar Tenant ID. [Ver guia →](system-admin/guias/SUBDOMAIN_LOGIN_GUIDE.md)

> 🧪 **NOVO**: Dados Mockados! Execute o frontend sem backend para desenvolvimento e testes. [Ver guia →](system-admin/guias/MOCK_DATA_GUIDE.md)

> 📱 **IMPORTANTE**: Migração para PWA! Descontinuamos os apps nativos iOS/Android em favor de um PWA multiplataforma. [Ver guia de migração →](system-admin/docs/MOBILE_TO_PWA_MIGRATION.md) | [Como instalar PWA →](system-admin/guias/PWA_INSTALLATION_GUIDE.md)

## 📱 Aplicativo Móvel (PWA) 🆕

O PrimeCare Software agora é um **Progressive Web App (PWA)** que funciona em todos os dispositivos:

### Características do PWA:
- 📱 **Multiplataforma**: Funciona em iOS, Android, Windows, macOS e Linux
- ⚡ **Instalável**: Adicione à tela inicial como um app nativo
- 🔄 **Atualizações Automáticas**: Sempre a versão mais recente
- 💾 **Funciona Offline**: Acesso básico sem internet
- 🚀 **Performance**: Rápido e responsivo
- 🎨 **Interface Nativa**: Visual moderno e intuitivo

### Compatibilidade:
- ✅ **iOS 16.4+** (iPhone e iPad via Safari)
- ✅ **Android 7.0+** (via Chrome)
- ✅ **Windows 10+** (via Chrome/Edge)
- ✅ **macOS 10.15+** (via Safari/Chrome)
- ✅ **Linux** (via Chrome/Firefox)

**📖 [Guia de Instalação do PWA →](system-admin/guias/PWA_INSTALLATION_GUIDE.md)**
**📖 [Documentação da Migração →](system-admin/docs/MOBILE_TO_PWA_MIGRATION.md)**

### ⚠️ Apps Nativos Descontinuados

Os aplicativos nativos iOS (Swift) e Android (Kotlin) foram **descontinuados** em Janeiro de 2026. Todos os recursos foram migrados para o PWA com melhorias significativas:

- 💰 **Economia**: Sem taxas de 30% das lojas de apps
- ⚡ **Mais Rápido**: Atualizações instantâneas sem aprovação
- 🌍 **Mais Alcance**: Funciona em qualquer dispositivo
- 🔧 **Mais Fácil**: Uma base de código ao invés de três

**Código dos apps nativos arquivado em**: `mobile/ios/` e `mobile/android/` (somente referência)

## 🖥️ Frontend Application

O PrimeCare Software agora possui **um único aplicativo Angular unificado** que consolida todas as funcionalidades:

### **PrimeCare Frontend** (`frontend/medicwarehouse-app`)
Aplicativo unificado acessando diferentes seções por rotas:

#### 📱 **Clínica** (rotas principais)
- 👨‍⚕️ **Usuários**: Proprietários de clínicas, médicos, secretárias, enfermeiros
- 📊 **Dashboard** da clínica individual
- 👥 **Gestão de pacientes** e prontuários
- 📅 **Agendamentos** e atendimentos
- 💊 **Prescrições** médicas e procedimentos
- 🎥 **Telemedicina** com videochamadas
- 🌐 **Login por Subdomínio** - Acesso personalizado por clínica
- 🧪 **Dados Mockados** - Desenvolvimento sem backend
- **URL desenvolvimento**: `http://localhost:4200`
- **Exemplo com subdomínio**: `http://clinic1.localhost:4200`

#### ⚙️ **System Admin** (`/system-admin/*`)
- 🔧 **Usuários**: System Owners (administradores do sistema)
- 🏥 **Gestão de todas as clínicas** (criar, ativar, desativar)
- 💰 **Métricas financeiras** (MRR, receitas, churn)
- 📊 **Analytics globais** do sistema
- ⚙️ **Controle de assinaturas** e override manual
- 👤 **Gestão de system owners**
- **URL**: `http://localhost:4200/system-admin`

#### 🌐 **Site Marketing** (`/site/*`)
- 🏠 **Landing page** e apresentação do produto
- 💰 **Página de pricing** com planos
- 📝 **Formulário de registro** de novas clínicas
- 📞 **Página de contato**
- 📜 **Termos de uso** e política de privacidade
- **URL**: `http://localhost:4200/site`

**Benefícios da Consolidação:**
- ♻️ **Redução de 66%**: 3 apps → 1 app unificado
- 🔧 **Manutenção Simplificada**: Uma base de código
- 🚀 **Deploy Único**: Um build, um deploy
- 🎨 **UX Consistente**: Design system unificado
- 📦 **Menor footprint**: Dependências compartilhadas

### 🏥 **Portal do Paciente** (`frontend/patient-portal`) ✅ COMPLETO
Portal dedicado para acesso de pacientes (separado da aplicação principal):

- 👤 **Usuários**: Pacientes (acesso externo)
- ✅ **Login e Cadastro**: Sistema completo de autenticação
- 📋 **Ver Agendamentos**: Consulta de agendamentos confirmados
- 📄 **Documentos Médicos**: Download de receitas, exames, atestados
- 👤 **Perfil**: Gerenciamento de dados pessoais
- 🔐 **Autenticação**: JWT + Refresh Tokens
- 🔒 **Segurança**: Account lockout, password hashing, LGPD compliant
- 🔍 **Sistema de Auditoria LGPD Completo**: Rastreabilidade de todas operações ([ver documentação](./LGPD_AUDIT_SYSTEM.md))
  - ✅ Registro automático de todas ações (AuditLog)
  - ✅ Rastreamento de acesso a dados sensíveis (DataAccessLog)
  - ✅ Gestão de consentimentos (DataConsentLog)
  - ✅ Direito ao esquecimento - anonimização (DataDeletionRequest)
  - ✅ Portabilidade de dados - exportação JSON/XML/PDF (DataPortability)
  - ✅ Conformidade Art. 8, 18 e 37 da LGPD
- ✅ **58 Testes**: Cobertura completa de unit tests
- ✅ **Build**: Produção otimizado (394 KB)
- **URL desenvolvimento**: `http://localhost:4202` (quando executado separadamente)
- **API Backend**: `patient-portal-api/` (API dedicada - 100% completa)

**Por que separado?**
- 🔒 Isolamento de segurança (dados de pacientes)
- 🎯 Interface simplificada para usuários finais
- 📱 Autenticação independente
- ⚖️ Conformidade LGPD/CFM

### 📚 **Portal de Documentação** (`frontend/mw-docs`)
Portal de documentação técnica (GitHub Pages):

- 📖 **36+ Documentos** técnicos organizados
- 🔍 **Busca em tempo real** por título, categoria e descrição
- 📊 **Diagramas Mermaid** interativos
- 📝 **Renderização Markdown** com syntax highlighting
- **URL produção**: `https://primecaresoftware.github.io/MW.Code/`
- **Deploy**: Automático via GitHub Actions

> **Nota**: Os projetos `mw-site` e `mw-system-admin` foram **descontinuados e deletados** em Janeiro 2026, pois suas funcionalidades foram completamente migradas e integradas ao `medicwarehouse-app`.

### 🔌 Port Configuration

All frontend projects are configured with unique ports to allow running them simultaneously during development:

- **medicwarehouse-app**: Port 4200
- **mw-system-admin**: Port 4201
- **patient-portal**: Port 4202
- **mw-docs**: Port 4203

📖 **[Complete Port Configuration Guide →](system-admin/frontend/FRONTEND_PORTS.md)**

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
- [Análise de Serviços de Vídeo](system-admin/regras-negocio/TELEMEDICINE_VIDEO_SERVICES_ANALYSIS.md)
- [Integração Frontend](telemedicine/FRONTEND_INTEGRATION.md)

## 🎫 Sistema de Chamados (Support Tickets) 🆕

**Sistema de suporte técnico integrado à API principal!**

- 📂 **API Endpoint**: `/api/tickets`
- 📖 **Documentação**: [`system-admin/docs/TICKET_API_DOCUMENTATION.md`](system-admin/docs/TICKET_API_DOCUMENTATION.md)
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
- [Documentação da API de Tickets](system-admin/docs/TICKET_API_DOCUMENTATION.md)
- [Script de Migração](scripts/run-ticket-migration.sh)

## 📚 Documentação Completa

### 💼 Glossário de Termos Empresariais (NOVO!)
**Não entende termos da área empresarial? Comece por aqui!**
- 📖 **[`GLOSSARIO_TERMOS_EMPRESARIAIS.md`](system-admin/docs/GLOSSARIO_TERMOS_EMPRESARIAIS.md)** - Glossário completo explicando todos os termos de negócio
- 💡 Aprenda sobre: SaaS, MRR, Churn, CAC, LTV, ROI, e muito mais
- 🎯 Ideal para: Empreendedores, donos de negócio, estudantes
- 📊 Exemplos práticos e aplicações reais

### 📊 Documentação Técnica Consolidada (NOVO!)
- 📖 **[`RESUMO_TECNICO_COMPLETO.md`](system-admin/guias/RESUMO_TECNICO_COMPLETO.md)** - ⭐ Visão geral completa do sistema (92% completude)
- 📖 **[`GUIA_COMPLETO_APIs.md`](system-admin/guias/GUIA_COMPLETO_APIs.md)** - ⭐ Documentação de todos os endpoints da API
- 📖 **[`CHANGELOG.md`](CHANGELOG.md)** - ⭐ Histórico completo de desenvolvimento

### 📱 Documentação Portátil
### 🌐 Documentação Completa

**📍 Toda a documentação foi consolidada na pasta `/docs`!**

- 📂 **Índice Principal**: [`system-admin/docs/DOCUMENTATION_INDEX.md`](system-admin/docs/DOCUMENTATION_INDEX.md) - ⭐ **Comece aqui!** Navegação completa
- 🌐 **Interface Web Interativa**: [`frontend/mw-docs`](frontend/mw-docs/README.md) - Documentação navegável via Angular
- 📋 **Plano de Desenvolvimento**: [`system-admin/docs/PLANO_DESENVOLVIMENTO.md`](system-admin/docs/PLANO_DESENVOLVIMENTO.md) - Roadmap 2025-2026

**📖 Principais Documentos**:
- [`BUSINESS_RULES.md`](system-admin/docs/BUSINESS_RULES.md) - ⭐ **ESSENCIAL** - Regras de negócio do sistema
- [`PENDING_TASKS.md`](system-admin/docs/PENDING_TASKS.md) - Documento centralizado com todas as pendências
- [`GUIA_INICIO_RAPIDO_LOCAL.md`](system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md) - Setup rápido em 10 minutos
- [`AUTHENTICATION_GUIDE.md`](system-admin/infrastructure/AUTHENTICATION_GUIDE.md) - Guia de autenticação JWT
- [`SEEDER_GUIDE.md`](system-admin/guias/SEEDER_GUIDE.md) - Guia completo dos seeders
- [`SYSTEM_MAPPING.md`](system-admin/guias/SYSTEM_MAPPING.md) - Mapeamento completo do sistema

### 🧪 Guias de Configuração e Testes (NOVO! - Janeiro 2026)
**Documentação completa para testar TODAS as funcionalidades do sistema!**
- 📖 **[Índice Geral de Testes](system-admin/guias/testes-configuracao/README.md)** - ⭐ **Centro de Testes** - Navegação completa
- 🏥 **[Cadastro de Paciente](system-admin/guias/testes-configuracao/01-CADASTRO-PACIENTE.md)** - 25+ cenários de teste
- 📅 **[Atendimento e Consulta](system-admin/guias/testes-configuracao/02-ATENDIMENTO-CONSULTA.md)** - 30+ cenários de teste
- 💰 **[Módulo Financeiro](system-admin/guias/testes-configuracao/03-MODULO-FINANCEIRO.md)** - 25+ cenários de teste
- 🏥 **[TISS - Padrão ANS](system-admin/guias/testes-configuracao/04-TISS-PADRAO.md)** - 20+ cenários de teste
- 📋 **[TUSS - Tabela de Procedimentos](system-admin/guias/testes-configuracao/05-TUSS-TABELA.md)** - 18+ cenários de teste
- 🎥 **[Telemedicina](system-admin/guias/testes-configuracao/06-TELEMEDICINA.md)** - 22+ cenários (CFM 1821/2018)
- ✅ **[Cenários Completos](system-admin/guias/testes-configuracao/07-CENARIOS-COMPLETOS.md)** - 200+ cenários consolidados

**📊 Total**: 200+ cenários de teste com configuração passo a passo, exemplos de API, troubleshooting e checklists de validação.

**🎯 Consolidação Janeiro 2026**: Removidos 137 arquivos duplicados/desnecessários. Toda documentação agora em `/docs`.

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
- 📖 **Documentação**: [RICH_TEXT_EDITOR_AUTOCOMPLETE.md](system-admin/guias/RICH_TEXT_EDITOR_AUTOCOMPLETE.md)

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

### 🏥 Conformidade CFM 1.821/2007 - Prontuário Eletrônico 🆕✨

**Sistema 85% conforme a Resolução CFM 1.821/2007 sobre prontuários eletrônicos! (Janeiro 2026)**

#### Anamnese Estruturada (Campos Obrigatórios)
- ✅ **Queixa Principal**: Campo obrigatório com validação de 10+ caracteres
- ✅ **História da Doença Atual (HDA)**: Descrição detalhada com validação de 50+ caracteres
- ✅ **História Patológica Pregressa (HPP)**: Registro de histórico médico do paciente
- ✅ **História Familiar**: Antecedentes familiares relevantes
- ✅ **Hábitos de Vida**: Tabagismo, etilismo, atividade física, etc.
- ✅ **Medicações em Uso**: Lista de medicamentos atuais do paciente

#### Exame Clínico Completo ✨ (Frontend Janeiro 2026)
- ✅ **Sinais Vitais Obrigatórios** com validação inteligente:
  - Pressão Arterial (Sistólica/Diastólica): 50-300/30-200 mmHg
  - Frequência Cardíaca: 30-220 bpm
  - Frequência Respiratória: 8-60 irpm
  - Temperatura: 32-45°C
  - Saturação de O2: 0-100%
  - 🎯 **Alertas visuais para valores anormais** (fora da faixa normal)
- ✅ **Exame Físico Sistemático**: Descrição obrigatória (mín. 20 caracteres)
- ✅ **Estado Geral**: Registro do estado geral do paciente
- ✅ **Componente Frontend**: `ClinicalExaminationFormComponent` completo

#### Hipóteses Diagnósticas com CID-10 ✨ (Frontend Janeiro 2026)
- ✅ **Diagnósticos Estruturados**: Suporte a múltiplos diagnósticos por atendimento
- ✅ **Código CID-10 Validado**: Validação automática de formato (ex: A00, J20.9, Z99.01)
- ✅ **Tipificação**: Principal ou Secundário
- ✅ **Data do Diagnóstico**: Registro temporal de cada hipótese
- ✅ **Busca Rápida CID-10**: Interface com exemplos comuns para facilitar preenchimento
- ✅ **Validação Regex**: Padrão `[A-Z]{1,3}\d{2}(\.\d{1,2})?`
- ✅ **Componente Frontend**: `DiagnosticHypothesisFormComponent` completo

#### Plano Terapêutico Detalhado ✨ (Frontend Janeiro 2026)
- ✅ **Tratamento/Conduta**: Descrição obrigatória (mín. 20 caracteres)
- ✅ **Prescrição Medicamentosa**: Lista detalhada de medicamentos prescritos
- ✅ **Solicitação de Exames**: Exames complementares solicitados
- ✅ **Encaminhamentos**: Referências para outros especialistas
- ✅ **Orientações ao Paciente**: Instruções e cuidados
- ✅ **Data de Retorno**: Agendamento de retorno automático
- ✅ **Componente Frontend**: `TherapeuticPlanFormComponent` completo

#### Consentimento Informado ✨ (Frontend Janeiro 2026)
- ✅ **Termo de Consentimento**: Registro de consentimento do paciente
- ✅ **Aceite Digital**: Registro de aceite com data/hora
- ✅ **Rastreabilidade**: IP de origem e assinatura digital (opcional)
- ✅ **Histórico Completo**: Todos os consentimentos registrados
- ✅ **Aceite Imediato**: Opção de registrar aceite no momento do atendimento
- ✅ **Componente Frontend**: `InformedConsentFormComponent` completo

#### Auditoria e Controle
- ✅ **Fechamento de Prontuário**: Impede alterações após finalização
- ✅ **Identificação Profissional**: Médico responsável (CRM/UF)
- ✅ **Timestamps Completos**: Data/hora de criação e modificação
- ✅ **Isolamento Multi-tenant**: Segurança e privacidade garantidas
- ✅ **Guarda de 20 anos**: Soft-delete sem exclusão física

#### Status de Implementação (Janeiro 2026)
- ✅ **Backend**: 100% completo (entidades, repositórios, controllers, handlers)
- ✅ **Frontend Components**: 100% completo (4 componentes prontos)
- 🔄 **Integração**: Em progresso (integrar componentes no fluxo de atendimento)
- 📊 **Compliance Geral**: 85% (↑15% em Janeiro 2026)

📖 **Documentação Completa**:
- [Especificação CFM 1.821](system-admin/guias/ESPECIFICACAO_CFM_1821.md)
- [Implementação Detalhada](system-admin/guias/CFM_1821_IMPLEMENTACAO.md)
- [Plano de Implementação Pendente](system-admin/guias/IMPLEMENTACAO_PENDENTE_CFM_PRESCRICOES.md) - **NOVO!**
- [Histórico de Implementação](system-admin/guias/archive/README.md) - Phases 3, 4, e 5 completas (arquivado)

### 💊 Receitas Médicas Digitais - CFM 1.643/2002 & ANVISA 344/1998 🆕✨

**Sistema completo de prescrições digitais conforme CFM 1.643/2002 e ANVISA Portaria 344/1998! (Janeiro 2026)**

#### Tipos de Receita Suportados
- ✅ **Receita Simples** - Medicamentos comuns - 30 dias de validade
- ✅ **Receita de Controle Especial A** (Lista A1/A2/A3) - Entorpecentes - 30 dias + SNGPC
- ✅ **Receita de Controle Especial B** (Lista B1/B2) - Psicotrópicos - 30 dias + SNGPC
- ✅ **Receita de Controle Especial C1** (Lista C1) - Outras controladas - 30 dias + SNGPC
- ✅ **Receita Antimicrobiana** - Antibióticos - 10 dias de validade

#### Frontend Components ✨ (Janeiro 2026)
- ✅ **Digital Prescription Form Component** - Formulário completo de prescrição
  - Seletor de tipo de receita com informações de compliance
  - Editor de itens de prescrição com validações ANVISA
  - Campos obrigatórios por tipo de receita
  - Preview antes de finalizar
  - Suporte para múltiplos medicamentos
  - ~950 linhas de código TypeScript
  
- ✅ **Digital Prescription View Component** - Visualização e impressão
  - Layout otimizado para impressão
  - QR Code para verificação de autenticidade
  - Informações completas do médico (CRM/UF)
  - Informações completas do paciente
  - Lista detalhada de medicamentos
  - Assinatura digital (preparado para ICP-Brasil)
  - ~700 linhas de código TypeScript
  
- ✅ **Prescription Type Selector Component** - Seleção visual de tipo
  - Cards informativos para cada tipo de receita
  - Avisos sobre medicamentos controlados
  - Informações de validade e compliance
  - Características específicas de cada tipo
  - ~210 linhas de código TypeScript
  
- ✅ **SNGPC Dashboard Component** - Gestão de medicamentos controlados
  - Estatísticas de prescrições não reportadas
  - Dashboard de relatórios ANVISA
  - Rastreamento de prazo (dia 10 do mês seguinte)
  - Geração de XML para ANVISA
  - Controle de transmissão
  - ~376 linhas de código TypeScript

#### Backend Features
- ✅ **Entidades de Domínio**: DigitalPrescription, DigitalPrescriptionItem, SNGPCReport
- ✅ **Controle Sequencial**: Numeração automática para receitas controladas
- ✅ **Validações ANVISA**: Por tipo de receita e substância controlada
- ✅ **Código de Verificação**: QR Code para autenticidade
- ✅ **Assinatura Digital**: Preparado para ICP-Brasil
- ✅ **Relatórios SNGPC**: Sistema completo de reporting para ANVISA
- ✅ **API RESTful**: 15+ endpoints para gestão completa

#### Compliance Regulatório
- ✅ **CFM 1.643/2002**: Formato digital de receita médica
- ✅ **ANVISA 344/1998**: Classificação de substâncias controladas
- ✅ **CFM 1.821/2007**: Integração com prontuário eletrônico
- ✅ **SNGPC**: Sistema Nacional de Gerenciamento de Produtos Controlados
- ✅ **Retenção**: 20 anos de guarda obrigatória

📖 **Documentação Completa**:
- [Receitas Digitais - Guia Completo](system-admin/guias/DIGITAL_PRESCRIPTIONS.md)
- [Implementação Pendente](system-admin/guias/IMPLEMENTACAO_PENDENTE_CFM_PRESCRICOES.md)

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
  - [📚 Documentação Completa](system-admin/guias/NOTIFICATION_ROUTINES_DOCUMENTATION.md)
  - [💡 Exemplos de Uso](system-admin/guias/NOTIFICATION_ROUTINES_EXAMPLE.md)

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

> 📖 **Para detalhes completos de segurança**, consulte [SECURITY_GUIDE.md](system-admin/guias/SECURITY_GUIDE.md)

### 🔍 Sistema de Auditoria e Compliance LGPD (NOVO! ✨)

Sistema completo de auditoria e compliance com a LGPD (Lei 13.709/2018):

#### **Rastreabilidade Completa**
- ✅ **AuditLog**: Registro automático de TODAS as operações
  - CRUD (Create, Read, Update, Delete)
  - Autenticação (Login, Logout, falhas)
  - Mudanças de senha, MFA
  - Exportações e compartilhamentos
  - Tentativas não autorizadas
- ✅ **DataAccessLog**: Rastreamento específico de acesso a dados sensíveis
  - Quem acessou, quando e por quê
  - Campos específicos acessados
  - Autorização (aprovado/negado)

#### **Gestão de Consentimentos (Art. 8)**
- ✅ **DataConsentLog**: Registro completo de consentimentos
  - Tipos: Tratamento, Compartilhamento, Marketing, Pesquisa, Telemedicina
  - Texto exato apresentado ao titular
  - Versão do termo + método (WEB/MOBILE/PAPEL)
  - Revogação de consentimento
  - Status: Ativo/Revogado/Expirado

#### **Direito ao Esquecimento (Art. 18, VI)**
- ✅ **DataDeletionRequest**: Gerenciamento de exclusões
  - Solicitação de exclusão/anonimização
  - Workflow: Pendente → Processando → Completo/Rejeitado
  - Aprovação legal quando necessário
  - Anonimização preservando dados estatísticos

#### **Portabilidade de Dados (Art. 18, V)**
- ✅ **DataPortabilityService**: Exportação completa
  - Formatos: JSON, XML, PDF, Pacote ZIP
  - Dados incluídos: Pessoais, médicos, consultas, exames, prescrições
  - Download imediato

#### **APIs LGPD**
```
/api/consent/*           - Gestão de consentimentos
/api/datadeletion/*      - Direito ao esquecimento
/api/dataportability/*   - Exportação de dados
/api/audit/*             - Consulta de logs
```

#### **Conformidade**
- ✅ Art. 8 - Consentimento do titular
- ✅ Art. 18, I - Confirmação de tratamento
- ✅ Art. 18, II - Acesso aos dados
- ✅ Art. 18, V - Portabilidade
- ✅ Art. 18, VI - Eliminação (esquecimento)
- ✅ Art. 18, IX - Revogação de consentimento
- ✅ Art. 37 - Registro de operações

> 📖 **Documentação completa**: [LGPD_AUDIT_SYSTEM.md](./LGPD_AUDIT_SYSTEM.md)

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

> 🚀 **NOVO!** [**GUIA_INICIO_RAPIDO_LOCAL.md**](system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md) - **Setup em 10 minutos para testar HOJE!**

> 🔧 **IMPORTANTE!** [**LOCALHOST_SETUP_FIX.md**](system-admin/guias/LOCALHOST_SETUP_FIX.md) - **Fix aplicado para executar em localhost** - Leia se tiver problemas com autenticação ou dados vazios

> 📖 **Para um guia completo e detalhado**, consulte o arquivo [GUIA_EXECUCAO.md](system-admin/guias/GUIA_EXECUCAO.md)

> ✅ **NOVO!** [**CHECKLIST_TESTES_COMPLETO.md**](system-admin/guias/CHECKLIST_TESTES_COMPLETO.md) - Teste todos os 80+ endpoints e funcionalidades

> 📊 **NOVO!** [**RESUMO_SISTEMA_COMPLETO.md**](system-admin/guias/RESUMO_SISTEMA_COMPLETO.md) - Visão geral de tudo que está implementado

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
> - [GUIA_DESENVOLVIMENTO_AUTH.md](system-admin/guias/GUIA_DESENVOLVIMENTO_AUTH.md) - Guia completo para desenvolvimento e testes
> - [AUTHENTICATION_GUIDE.md](system-admin/infrastructure/AUTHENTICATION_GUIDE.md) - Documentação completa de autenticação
> - [CARGA_INICIAL_TESTES.md](system-admin/guias/CARGA_INICIAL_TESTES.md) - Detalhes sobre dados de teste

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

**PrimeCare Frontend Unificado** (aplicativo único com todas as funcionalidades):
```bash
# Navegar para o frontend
cd frontend/medicwarehouse-app

# Instalar dependências
npm install --legacy-peer-deps

# Executar em modo de desenvolvimento
npm start

# O frontend estará disponível em: http://localhost:4200
# Acessar diferentes seções por rotas:
# - Clínica: http://localhost:4200/dashboard
# - System Admin: http://localhost:4200/system-admin
# - Site Marketing: http://localhost:4200/site
```

> **Nota**: Usamos `--legacy-peer-deps` devido a conflitos menores de versão entre @angular/material e @angular/cdk que não afetam a funcionalidade.


```

#### Banco de Dados (PostgreSQL)

```bash
# Executar apenas o PostgreSQL via Podman
podman-compose up postgres -d

# Ou executar PostgreSQL standalone com Podman:
podman run -d \
  --name primecare-postgres \
  -e POSTGRES_DB=medicwarehouse \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine

# Aplicar migrations
# Método 1: Aplicar todas as migrations automaticamente (RECOMENDADO)
./run-all-migrations.sh

# Método 2: Aplicar apenas a aplicação principal
dotnet ef database update --context MedicSoftDbContext \
  --project src/MedicSoft.Repository \
  --startup-project src/MedicSoft.Api
```

> 📖 **Guia completo de migrations**: [MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md) - Como aplicar todas as migrations do sistema  
> 📖 **Guia completo de setup do PostgreSQL**: [PODMAN_POSTGRES_SETUP.md](system-admin/guias/PODMAN_POSTGRES_SETUP.md)  
> 📖 **Detalhes da migração SQL Server → PostgreSQL**: [MIGRACAO_POSTGRESQL.md](system-admin/guias/MIGRACAO_POSTGRESQL.md)

#### 🌱 Popular Banco de Dados com Dados de Exemplo

Após aplicar as migrations, popule o banco com dados de teste completos para começar a usar o sistema imediatamente:

**Opção 1: Script Automatizado (Recomendado)**

```bash
# Linux/macOS
./scripts/seed-demo-data.sh

# Windows PowerShell
.\scripts\seed-demo-data.ps1
```

**Opção 2: Usando cURL/API diretamente**

```bash
# Popular dados
curl -X POST http://localhost:5000/api/data-seeder/seed-demo

# Fazer login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "dr.silva", "password": "Doctor@123", "tenantId": "demo-clinic-001"}'
```

**Opção 3: Usando Postman**
1. Importe `PrimeCare-Postman-Collection.json`
2. Execute: `Data Seeder > Seed Demo Data`
3. Execute: `Auth > Login`

**O que é criado:**
- ✅ 5 Planos de assinatura
- ✅ 1 Clínica Demo completa
- ✅ 4 Usuários (Owner, Admin, Médico, Recepcionista)
- ✅ 6 Pacientes (incluindo 2 crianças com responsável)
- ✅ 8 Procedimentos diversos
- ✅ 5 Agendamentos (passados, hoje e futuros)
- ✅ 2 Prontuários médicos completos
- ✅ 10 Despesas categorizadas
- ✅ 5 Solicitações de exames
- ✅ E muito mais...

**Credenciais de acesso:**
- **Médico**: dr.silva / Doctor@123
- **Recepcionista**: recep.maria / Recep@123
- **Owner**: owner.demo / Owner@123
- **Admin**: admin / Admin@123
- **TenantID**: demo-clinic-001

> 📖 **Guia Completo da API de Seed**: [SEED_API_GUIDE.md](system-admin/guias/SEED_API_GUIDE.md) - Documentação detalhada com todos os dados criados, cenários de teste e troubleshooting

## 📖 Documentação da API

Após executar a aplicação, acesse a documentação interativa do Swagger:

- **Swagger UI (Docker)**: http://localhost:5000/swagger
- **Swagger UI (Desenvolvimento local)**: https://localhost:7107/swagger

### 📮 Coleção Postman

Para facilitar o teste e integração, todas as APIs foram exportadas para o Postman:

- 📥 **Arquivo**: [`PrimeCare-Postman-Collection.json`](PrimeCare-Postman-Collection.json)
- 📖 **Guia de Importação**: [`POSTMAN_IMPORT_GUIDE.md`](system-admin/guias/POSTMAN_IMPORT_GUIDE.md)
- ✨ **Recursos incluídos**:
  - Todos os endpoints organizados por funcionalidade
  - Variáveis pré-configuradas (base_url, tenant_id)
  - Headers pré-configurados
  - Exemplos de requests prontos para uso

**Como usar:**
1. Importe o arquivo no Postman
2. Configure as variáveis da coleção
3. Teste os endpoints!

Para instruções detalhadas, consulte o [Guia de Importação do Postman](system-admin/guias/POSTMAN_IMPORT_GUIDE.md).

> 📖 **Guia Completo**: Para um passo a passo detalhado de como configurar e cadastrar tudo no sistema, consulte o [Guia de Configuração do Sistema](system-admin/guias/SYSTEM_SETUP_GUIDE.md).

### Endpoints Principais

- **Autenticação** 🔐:
  - `POST /api/auth/login` - Login de usuários (doctors, secretaries, etc.)
  - `POST /api/auth/owner-login` - Login de proprietários (clinic owners e system owners)
  - `POST /api/auth/validate` - Validar token JWT
  - 📖 **Veja**: [`AUTHENTICATION_GUIDE.md`](system-admin/infrastructure/AUTHENTICATION_GUIDE.md) para detalhes completos

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

Para mais detalhes sobre as regras de negócio, consulte [BUSINESS_RULES.md](system-admin/docs/BUSINESS_RULES.md)

## 📱 Interface e Telas

Para visualizar todas as telas do sistema com descrições detalhadas e fluxos de navegação, consulte:
- **[SCREENS_DOCUMENTATION.md](system-admin/guias/SCREENS_DOCUMENTATION.md)** - Documentação completa de todas as interfaces com diagramas de fluxo
- **[docs/VISUAL_FLOW_SUMMARY.md](system-admin/guias/VISUAL_FLOW_SUMMARY.md)** - Resumo visual rápido com diagramas Mermaid interativos

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

O PrimeCare Software implementa múltiplas camadas de segurança para proteger dados sensíveis:

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
- 📖 **[AUTHENTICATION_GUIDE.md](system-admin/infrastructure/AUTHENTICATION_GUIDE.md)** - Guia completo de autenticação JWT
- 📖 **[SECURITY_GUIDE.md](system-admin/guias/SECURITY_GUIDE.md)** - Guia completo de segurança

## 🚀 Deploy e Infraestrutura de Produção

### 💰 Infraestrutura com Baixo Custo (NOVO!) 🔥

**Documentação completa para produção com custo mínimo ($5-20/mês) enquanto você não tem clientes grandes!**

#### 📚 [INFRA_DOCS_INDEX.md](system-admin/guias/INFRA_DOCS_INDEX.md) - **COMECE AQUI!**
Índice completo com todos os guias de infraestrutura. Navegação fácil para encontrar o que você precisa.

#### 🚀 Guias Principais:

- **[QUICK_START_PRODUCTION.md](system-admin/guias/QUICK_START_PRODUCTION.md)** - ⚡ **Do Zero ao Ar em 30 Minutos**
  - Setup rápido com Railway ou VPS
  - Passo a passo simplificado
  - Para quem quer resultados AGORA

- **[CALCULADORA_CUSTOS.md](system-admin/guias/CALCULADORA_CUSTOS.md)** - 💵 **Planeje Seus Custos**
  - Estimativas por número de clínicas (1-500+)
  - Comparação Railway vs VPS vs Cloud
  - Projeção de crescimento e ROI

- **[INFRA_PRODUCAO_BAIXO_CUSTO.md](system-admin/guias/INFRA_PRODUCAO_BAIXO_CUSTO.md)** - 📋 **Guia Completo**
  - 💚 **Railway + Vercel** (Recomendado) - $5-20/mês
  - 🔧 **VPS (Hetzner/DigitalOcean)** - $5-10/mês
  - 🆓 **Free Tier** - $0/mês (apenas testes)
  - Comparativos, estratégias de escala, backups

- **[DEPLOY_RAILWAY_GUIDE.md](system-admin/guias/DEPLOY_RAILWAY_GUIDE.md)** - 🚂 **Deploy no Railway**
  - Passo a passo detalhado
  - PostgreSQL incluído
  - SSL e backups automáticos

- **[DEPLOY_HOSTINGER_GUIA_COMPLETO.md](system-admin/guias/DEPLOY_HOSTINGER_GUIA_COMPLETO.md)** - 🏢 **Deploy no Hostinger VPS** 🆕
  - Guia completo para iniciantes
  - Configuração passo a passo de VPS
  - Instalação de todos os componentes
  - Domínio e SSL explicados
  - R$ 20-60/mês

- **[DEPLOY_HOSTINGER_INICIO_RAPIDO.md](system-admin/guias/DEPLOY_HOSTINGER_INICIO_RAPIDO.md)** - ⚡ **Hostinger em 30 min** 🆕
  - Comandos prontos para uso
  - Deploy rápido no Hostinger
  - Checklist de verificação

- **[MIGRACAO_POSTGRESQL.md](system-admin/guias/MIGRACAO_POSTGRESQL.md)** - 🔄 **Economize 90%+ em Banco**
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

Para detalhes completos, veja: [INFRA_PRODUCAO_BAIXO_CUSTO.md](system-admin/guias/INFRA_PRODUCAO_BAIXO_CUSTO.md)

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

Para mais detalhes, consulte: [CI_CD_DOCUMENTATION.md](system-admin/guias/CI_CD_DOCUMENTATION.md)

## 🤝 Contribuição

Contribuições são bem-vindas! Veja nosso [Guia de Contribuição](CONTRIBUTING.md) para saber como começar.

### Como Contribuir

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

**Guia completo:** [CONTRIBUTING.md](CONTRIBUTING.md)

### Boas Issues para Começar

Procure por issues marcadas com:
- `good first issue` - Boas para iniciantes
- `help wanted` - Precisamos de ajuda
- `documentation` - Melhorias na documentação

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Contato

- **Projeto**: PrimeCare Software
- **Email**: contato@primecaresoftware.com
- **GitHub**: [https://github.com/MedicWarehouse/MW.Code](https://github.com/MedicWarehouse/MW.Code)

