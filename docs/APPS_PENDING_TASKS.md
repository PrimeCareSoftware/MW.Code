# 📱 Pendências de Desenvolvimento - Aplicativos MedicWarehouse

> **Objetivo:** Documento centralizado com todas as pendências específicas de cada aplicativo do ecossistema MedicWarehouse.

> **Última Atualização:** Novembro 2025  
> **Status:** Em desenvolvimento ativo

---

## 📊 Visão Geral do Ecossistema

O MedicWarehouse possui **6 aplicativos** que compõem o ecossistema completo:

| Aplicativo | Tecnologia | Localização | Status |
|------------|-----------|-------------|--------|
| 🏥 **MedicWarehouse App** | Angular 20 | `frontend/medicwarehouse-app` | ✅ Beta |
| 🔧 **MW System Admin** | Angular 20 | `frontend/mw-system-admin` | ✅ Beta |
| 🌐 **MW Site** | Angular 20 | `frontend/mw-site` | ✅ Em desenvolvimento |
| 📚 **MW Docs** | Angular 20 | `frontend/mw-docs` | ✅ Produção |
| 📱 **iOS App** | Swift/SwiftUI | `mobile/ios` | ✅ Beta |
| 🤖 **Android App** | Kotlin/Compose | `mobile/android` | 🚧 Em desenvolvimento |

---

## 🏥 MedicWarehouse App (Frontend Principal)

**Descrição:** Aplicativo principal para gestão de clínicas, pacientes, agendamentos e atendimentos.

**Tecnologias:**
- Angular 20.3.3
- TypeScript
- SCSS
- Material Design / Custom Components

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Login | ✅ | Autenticação JWT, login de usuários e proprietários |
| Registro | ✅ | Cadastro de novas clínicas e proprietários |
| Dashboard | ✅ | Visão geral com estatísticas e ações rápidas |
| Pacientes | ✅ | Listagem, busca, cadastro e edição |
| Agendamentos | ✅ | Agenda, filtros por status, calendário |
| Atendimento | ✅ | Prontuário, prescrições, fechamento |
| Fila de Espera | ✅ | Gerenciamento de fila em tempo real |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Dashboard de Relatórios** - Implementar gráficos e relatórios detalhados
- [ ] **Módulo Financeiro** - Contas a pagar/receber, fluxo de caixa
- [ ] **Notificações em Tempo Real** - Push notifications e alertas
- [ ] **Prontuário SOAP** - Estruturar prontuário no padrão SOAP
- [ ] **Integração com Convênios** - TISS para faturamento

#### Prioridade Média
- [ ] **Telemedicina** - Videochamadas integradas
- [ ] **Portal do Paciente** - Área restrita para pacientes
- [ ] **Multiidioma (i18n)** - Suporte a inglês e espanhol
- [ ] **Modo Offline** - Cache local com sincronização
- [ ] **Exportação de Dados** - PDF, Excel, CSV

#### Prioridade Baixa
- [ ] **Temas Personalizados** - Customização visual por clínica
- [ ] **Widgets Configuráveis** - Dashboard personalizável
- [ ] **Atalhos de Teclado** - Produtividade para usuários avançados

### 📁 Estrutura de Páginas

```
frontend/medicwarehouse-app/src/app/pages/
├── appointments/      # Agendamentos
├── attendance/        # Atendimento/Consulta
├── dashboard/         # Dashboard principal
├── login/            # Autenticação
├── patients/         # Pacientes
├── register/         # Cadastro de clínica
└── waiting-queue/    # Fila de espera
```

---

## 🔧 MW System Admin

**Descrição:** Painel de administração para System Owners gerenciarem todas as clínicas do sistema.

**Tecnologias:**
- Angular 20
- TypeScript
- SCSS
- Material Design

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Login | ✅ | Autenticação de System Owner |
| Dashboard | ✅ | Métricas globais do sistema |
| Clínicas | ✅ | Listagem e gerenciamento de todas as clínicas |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Gestão de System Owners** - Criar/editar outros administradores
- [ ] **Gestão de Planos** - Criar e gerenciar planos de assinatura
- [ ] **Relatórios Financeiros** - MRR, ARR, churn, LTV
- [ ] **Auditoria Global** - Logs de todas as ações do sistema
- [ ] **Override de Assinaturas** - Ajustes manuais em assinaturas

#### Prioridade Média
- [ ] **Dashboard Analytics** - Gráficos de crescimento, métricas KPI
- [ ] **Gestão de Funcionalidades** - Feature flags por plano
- [ ] **Comunicação em Massa** - Envio de emails/notificações para clínicas
- [ ] **Backup/Restore** - Backup de dados de clínicas específicas
- [ ] **Exportação de Dados** - Relatórios consolidados

#### Prioridade Baixa
- [ ] **Tema Escuro** - Dark mode para admin
- [ ] **API de Webhooks** - Integração com sistemas externos
- [ ] **Logs de Performance** - Monitoramento de tempo de resposta

### 📁 Estrutura de Páginas

```
frontend/mw-system-admin/src/app/pages/
├── clinics/          # Gestão de clínicas
├── dashboard/        # Dashboard global
└── login/           # Autenticação System Owner
```

---

## 🌐 MW Site (Marketing/Landing Page)

**Descrição:** Site institucional e landing page para captação de novos clientes.

**Tecnologias:**
- Angular 20.3.5
- TypeScript
- SCSS
- SEO otimizado

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Home | ✅ | Landing page com hero section |
| Funcionalidades | ✅ | Apresentação de recursos |
| Planos | ✅ | Tabela de preços e planos |
| Contato | ✅ | Formulário de contato |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Blog** - Sistema de artigos e conteúdo
- [ ] **Cases de Sucesso** - Depoimentos de clientes
- [ ] **FAQ** - Perguntas frequentes
- [ ] **Chat Online** - Suporte em tempo real
- [ ] **SEO Avançado** - Meta tags dinâmicas, sitemap

#### Prioridade Média
- [ ] **Calculadora de ROI** - Simular economia com o sistema
- [ ] **Tour Virtual** - Demonstração interativa do sistema
- [ ] **Comparativo** - Comparação com concorrentes
- [ ] **Integrações** - Página de integrações disponíveis
- [ ] **Política de Privacidade** - LGPD compliance

#### Prioridade Baixa
- [ ] **Multiidioma** - Versões em inglês e espanhol
- [ ] **PWA** - Progressive Web App para site
- [ ] **Analytics Avançado** - Tracking de conversões

### 📁 Estrutura de Páginas

```
frontend/mw-site/src/app/
├── components/       # Componentes reutilizáveis
├── pages/           # Páginas do site
│   ├── home/        # Landing page
│   ├── features/    # Funcionalidades
│   ├── pricing/     # Planos e preços
│   └── contact/     # Contato
└── services/        # Serviços
```

---

## 📚 MW Docs (Documentação)

**Descrição:** Central de documentação do projeto com visualização de todos os documentos.

**Tecnologias:**
- Angular 20.3.5
- ngx-markdown
- Mermaid (diagramas)
- Syntax highlighting

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Home | ✅ | Listagem de documentos por categoria |
| Doc Viewer | ✅ | Visualização de documentos Markdown |
| Busca | ✅ | Pesquisa em tempo real |
| Diagramas | ✅ | Renderização de Mermaid |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Versionamento** - Histórico de versões de documentos
- [ ] **Edição Online** - Editar documentos direto na interface
- [ ] **PDF Export** - Exportar documentos para PDF
- [ ] **Índice Automático** - TOC gerado automaticamente
- [ ] **Links Internos** - Navegação entre documentos

#### Prioridade Média
- [ ] **Comentários** - Sistema de comentários/feedback
- [ ] **Favoritos** - Marcar documentos frequentes
- [ ] **Dark Mode** - Tema escuro para leitura
- [ ] **Print Friendly** - Layout otimizado para impressão
- [ ] **Compartilhamento** - Links diretos para seções

#### Prioridade Baixa
- [ ] **API Pública** - Acesso programático aos docs
- [ ] **Changelog Automático** - Geração de changelog
- [ ] **Tradução** - Suporte multilíngue

### 📁 Estrutura de Páginas

```
frontend/mw-docs/src/app/
├── components/
│   ├── home/              # Página inicial com listagem
│   └── doc-viewer/        # Visualizador de documentos
├── models/                # Interfaces TypeScript
├── services/              # Serviço de documentação
└── assets/docs/           # Arquivos Markdown
```

---

## 📱 iOS App (Swift/SwiftUI)

**Descrição:** Aplicativo nativo iOS para acesso mobile ao sistema MedicWarehouse.

**Tecnologias:**
- Swift 5.9
- SwiftUI
- Combine
- iOS 17.0+

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Login | ✅ | Autenticação JWT (usuários e proprietários) |
| Dashboard | ✅ | Estatísticas em tempo real |
| Pacientes | ✅ | Listagem, busca, detalhes |
| Agendamentos | ✅ | Lista com filtros, detalhes |
| Perfil | ✅ | Informações do usuário e logout |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Criar/Editar Paciente** - Formulário de cadastro
- [ ] **Criar/Editar Agendamento** - Formulário de agendamento
- [ ] **Prontuários** - Visualização de histórico médico
- [ ] **Notificações Push** - APNs integration
- [ ] **Biometria** - Face ID / Touch ID

#### Prioridade Média
- [ ] **Modo Offline** - Cache local com sincronização
- [ ] **Telemedicina** - Videochamadas no app
- [ ] **Upload de Fotos** - Anexar imagens ao prontuário
- [ ] **Prescrições** - Visualizar e criar prescrições
- [ ] **Widget iOS** - Widget na Home Screen

#### Prioridade Baixa
- [ ] **Apple Watch** - Companion app
- [ ] **Siri Shortcuts** - Ações rápidas por voz
- [ ] **Handoff** - Continuidade com macOS
- [ ] **CarPlay** - Interface para carros (lembretes)

### 📁 Estrutura do Projeto

```
mobile/ios/MedicWarehouse/
├── MedicWarehouseApp.swift    # Entry point
├── ContentView.swift          # Root view
├── Views/
│   ├── LoginView.swift        # Tela de login
│   ├── DashboardView.swift    # Dashboard
│   ├── PatientsView.swift     # Lista de pacientes
│   └── AppointmentsView.swift # Lista de agendamentos
├── ViewModels/
│   └── AuthViewModel.swift    # ViewModel de autenticação
├── Services/
│   ├── APIService.swift       # Serviço de API
│   └── NetworkManager.swift   # Gerenciador de rede
└── Models/
    └── Models.swift           # Modelos de dados
```

---

## 🤖 Android App (Kotlin/Jetpack Compose)

**Descrição:** Aplicativo nativo Android para acesso mobile ao sistema MedicWarehouse.

**Tecnologias:**
- Kotlin 1.9.20
- Jetpack Compose
- Material Design 3
- Hilt (DI)
- Retrofit
- Android 7.0+ (API 24)

### ✅ Funcionalidades Implementadas

| Módulo | Status | Descrição |
|--------|--------|-----------|
| Login | ✅ | Autenticação JWT |
| Dashboard | ✅ | Estatísticas básicas |
| Arquitetura | ✅ | Clean Architecture + MVVM |

### 🚧 Pendências de Desenvolvimento

#### Prioridade Alta
- [ ] **Tela de Pacientes** - Completar listagem e detalhes
- [ ] **Tela de Agendamentos** - Completar listagem e filtros
- [ ] **Criar/Editar Paciente** - Formulário de cadastro
- [ ] **Criar/Editar Agendamento** - Formulário de agendamento
- [ ] **Notificações Push** - Firebase Cloud Messaging

#### Prioridade Média
- [ ] **Prontuários** - Visualização de histórico médico
- [ ] **Biometria** - Fingerprint / Face unlock
- [ ] **Modo Offline** - Room database + WorkManager
- [ ] **Telemedicina** - Videochamadas no app
- [ ] **Upload de Fotos** - CameraX integration

#### Prioridade Baixa
- [ ] **Widget Android** - Widget na Home Screen
- [ ] **Wear OS** - App para smartwatches
- [ ] **Android Auto** - Interface para carros
- [ ] **Voice Actions** - Comandos por voz

### 📁 Estrutura do Projeto

```
mobile/android/app/src/main/kotlin/com/medicwarehouse/app/
├── MainActivity.kt           # Activity principal
├── MedicWarehouseApp.kt     # Application class
├── ui/
│   ├── screens/
│   │   ├── LoginScreen.kt    # Tela de login
│   │   └── DashboardScreen.kt # Dashboard
│   ├── theme/
│   │   ├── Theme.kt          # Material Design theme
│   │   └── Type.kt           # Tipografia
│   └── navigation/
│       └── NavGraph.kt       # Navegação
├── viewmodel/
│   ├── AuthViewModel.kt      # ViewModel de autenticação
│   └── DashboardViewModel.kt # ViewModel do dashboard
├── data/
│   ├── Models.kt             # Modelos de dados
│   └── Repository.kt         # Repositório
└── network/
    ├── ApiService.kt         # Interface Retrofit
    ├── AuthInterceptor.kt    # Interceptor JWT
    ├── TokenManager.kt       # Gerenciador de tokens
    └── NetworkModule.kt      # Módulo Hilt
```

---

## 📅 Roadmap de Desenvolvimento

### Q1 2025 - Consolidação Mobile

| Tarefa | App | Esforço | Status |
|--------|-----|---------|--------|
| Completar Android | 🤖 Android | 4-6 semanas | 🚧 |
| Paridade iOS/Android | 📱 iOS + 🤖 Android | 2-3 semanas | 🚧 |
| Notificações Push | 📱 + 🤖 | 2 semanas | ❌ |
| Biometria | 📱 + 🤖 | 1 semana | ❌ |

### Q2 2025 - Funcionalidades Avançadas Web

| Tarefa | App | Esforço | Status |
|--------|-----|---------|--------|
| Dashboard Relatórios | 🏥 App | 3-4 semanas | ❌ |
| Módulo Financeiro | 🏥 App | 4-6 semanas | ❌ |
| Blog no Site | 🌐 Site | 2-3 semanas | ❌ |
| Gestão System Owners | 🔧 Admin | 2 semanas | ❌ |

### Q3 2025 - Telemedicina

| Tarefa | App | Esforço | Status |
|--------|-----|---------|--------|
| Telemedicina Web | 🏥 App | 6-8 semanas | ❌ |
| Telemedicina iOS | 📱 iOS | 3-4 semanas | ❌ |
| Telemedicina Android | 🤖 Android | 3-4 semanas | ❌ |

### Q4 2025 - Portal do Paciente

| Tarefa | App | Esforço | Status |
|--------|-----|---------|--------|
| Portal Web | 🆕 Patient Portal | 8-10 semanas | ❌ |
| App Paciente iOS | 📱 Patient iOS | 6-8 semanas | ❌ |
| App Paciente Android | 🤖 Patient Android | 6-8 semanas | ❌ |

---

## 📊 Métricas de Progresso

### Progresso por Aplicativo

| Aplicativo | Implementado | Pendente | % Completo |
|------------|--------------|----------|------------|
| 🏥 App | 7 módulos | 12 features | 60% |
| 🔧 Admin | 3 módulos | 11 features | 30% |
| 🌐 Site | 4 módulos | 10 features | 40% |
| 📚 Docs | 4 módulos | 9 features | 45% |
| 📱 iOS | 5 módulos | 12 features | 50% |
| 🤖 Android | 3 módulos | 14 features | 25% |

### Total de Tarefas

- **Total de Funcionalidades Pendentes:** 68
- **Alta Prioridade:** 25
- **Média Prioridade:** 28
- **Baixa Prioridade:** 15

---

## 🔗 Documentação Relacionada

- [PENDING_TASKS.md](PENDING_TASKS.md) - Pendências gerais do sistema
- [MOBILE_APPS_GUIDE.md](MOBILE_APPS_GUIDE.md) - Guia dos aplicativos mobile
- [MOBILE_IMPLEMENTATION_SUMMARY.md](MOBILE_IMPLEMENTATION_SUMMARY.md) - Resumo da implementação mobile
- [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md) - Índice geral de documentação

---

**Documento Elaborado Por:** GitHub Copilot  
**Data:** Novembro 2025  
**Versão:** 1.0

**Este documento deve ser atualizado sempre que houver progresso significativo nos aplicativos.**
