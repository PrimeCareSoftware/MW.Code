# 📚 Mapa de Documentação - PrimeCare Software

> **Última Atualização:** Janeiro 2026  
> **Propósito:** Índice completo de toda documentação do projeto

Este documento fornece uma visão geral de toda a documentação disponível no repositório PrimeCare Software.

## 📍 Documentação Principal (Raiz)

### Documentos Essenciais
- **[README.md](./README.md)** - Documentação principal do projeto, setup e visão geral
- **[CHANGELOG.md](./CHANGELOG.md)** - Histórico de versões e mudanças
- **[CONTRIBUTING.md](./CONTRIBUTING.md)** - Guia de contribuição para desenvolvedores
- **[TISS_FASE1_IMPLEMENTACAO_COMPLETA.md](./TISS_FASE1_IMPLEMENTACAO_COMPLETA.md)** - Status da implementação TISS Fase 1

### 🗂️ Documentação Arquivada
- **[docs_archive/](./docs_archive/)** - Documentação antiga e arquivada (11 arquivos)
  - Resumos de implementação supersedidos
  - Documentação LGPD antiga (migrada para system-admin)
  - Status temporários de projetos concluídos

---

## 📋 Plano de Desenvolvimento

**Localização:** [Plano_Desenvolvimento/](./Plano_Desenvolvimento/)

Contém o roadmap completo do projeto organizado em fases de prioridade.

### Estrutura
- **[README.md](./Plano_Desenvolvimento/README.md)** - Visão geral do plano (24 prompts, R$ 1.455.000)
- **[DEPENDENCIES.md](./Plano_Desenvolvimento/DEPENDENCIES.md)** - Matriz de dependências entre tarefas
- **[EFFORT_ESTIMATES.md](./Plano_Desenvolvimento/EFFORT_ESTIMATES.md)** - Estimativas detalhadas

### Fases do Projeto

#### 🔴 Fase 1 - Conformidade Legal (P0 - Crítico)
**[fase-1-conformidade-legal/](./Plano_Desenvolvimento/fase-1-conformidade-legal/)**
- CFM 1.821/1.638 - Prontuários e versionamento
- Prescrições digitais
- SNGPC - Controle de medicamentos
- CFM 2.314 - Telemedicina
- TISS Fase 1 - Convênios

#### 🟡 Fase 2 - Segurança e LGPD (P1 - Alta)
**[fase-2-seguranca-lgpd/](./Plano_Desenvolvimento/fase-2-seguranca-lgpd/)**
- Auditoria LGPD completa
- Criptografia de dados
- Portal do paciente
- Prontuário SOAP
- Melhorias de segurança

#### 🟢 Fase 4 - Analytics e Otimização (P2 - Média)
**[fase-4-analytics-otimizacao/](./Plano_Desenvolvimento/fase-4-analytics-otimizacao/)**
- TISS Fase 2 - Análise de glosas
- Fila de espera avançada
- BI e Analytics
- Assinatura digital
- CRM avançado
- Gestão fiscal
- Acessibilidade WCAG

#### 🔵 Fase 5 - Enterprise (P3 - Baixa)
**[fase-5-enterprise/](./Plano_Desenvolvimento/fase-5-enterprise/)**
- API pública
- Integração com laboratórios
- Marketplace de extensões
- Programa de referral

### Documentos Adicionais
- **[PLANO_ADAPTACAO_MULTI_NEGOCIOS.md](./Plano_Desenvolvimento/PLANO_ADAPTACAO_MULTI_NEGOCIOS.md)** - Adaptação multi-negócios
- **[ANALISE_MERCADO_SAAS_SAUDE.md](./Plano_Desenvolvimento/ANALISE_MERCADO_SAAS_SAUDE.md)** - Análise de mercado
- **[GUIA_CONFIGURACAO_TIPOS_NEGOCIO.md](./Plano_Desenvolvimento/GUIA_CONFIGURACAO_TIPOS_NEGOCIO.md)** - Configuração de tipos de negócio
- **[TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md](./Plano_Desenvolvimento/TELEATENDIMENTO_PROFISSIONAIS_AUTONOMOS.md)** - Teleatendimento para autônomos

---

## 🏥 System-Admin - Documentação Técnica

**Localização:** [system-admin/](./system-admin/)

Central de documentação técnica detalhada do sistema.

### 📑 Índices Principais
- **[README.md](./system-admin/README.md)** - Hub central da documentação
- **[INDICE.md](./system-admin/INDICE.md)** - Índice completo de toda documentação system-admin

### 🔧 Implementações (59 documentos ativos)
**[implementacoes/](./system-admin/implementacoes/)**
- **[INDEX.md](./system-admin/implementacoes/INDEX.md)** - Índice completo de implementações
- Implementações SNGPC (Status 97%)
- Implementações TISS/TUSS (Status 97%)
- Portal do Paciente
- Telemedicina
- LGPD e Auditoria
- Anamnese e SOAP
- Prescrições digitais
- Multi-clínica
- Financeiro e Fiscal
- Frontend e UX

**Arquivados:** [implementacoes/archive_jan2026/](./system-admin/implementacoes/archive_jan2026/) (13 documentos)

### 📖 Documentação Técnica
**[docs/](./system-admin/docs/)**
- Guias técnicos e arquiteturais
- Migrações (PostgreSQL, PWA, etc.)
- Integrações e APIs
- Funcionalidades implementadas
- Regras de negócio

**Subdiretórios:**
- **[archive/](./system-admin/docs/archive/)** - Documentação técnica arquivada

### 📚 Guias de Usuário
**[guias/](./system-admin/guias/)**
- Guias de início rápido
- Guias multiplataforma (macOS, Windows, Linux)
- Configuração e setup
- Login por subdomínio
- PWA - Instalação e uso
- Mock data para desenvolvimento

### 🏗️ Backend
**[backend/](./system-admin/backend/)**
- Arquitetura de serviços
- APIs e controllers
- Configurações de licença
- Guias de API rápidos

### 🎨 Frontend
**[frontend/](./system-admin/frontend/)**
- Componentes Angular
- Guias de desenvolvimento
- Consolidação de aplicações
- PWA

### 🏢 Infraestrutura
**[infrastructure/](./system-admin/infrastructure/)**
- Docker e Podman
- Migrações de infraestrutura
- Configurações de deployment
- PostgreSQL

### 📋 Regras de Negócio
**[regras-negocio/](./system-admin/regras-negocio/)**
- Fluxos médicos e clínicos
- Telemedicina
- Portal do paciente
- TISS e prescrições
- Documentação SOAP

**Subdiretórios:**
- **[telemedicine/](./system-admin/regras-negocio/telemedicine/)** - CFM 2.314 e segurança
- **[patient-portal/](./system-admin/regras-negocio/patient-portal/)** - Portal do paciente

### 🔒 Segurança
**[seguranca/](./system-admin/seguranca/)**
- LGPD Compliance
- Criptografia de dados médicos
- Auditoria e logs
- Gestão de sessões
- Validações de segurança
- Análise de qualidade de código

### ⚕️ Conformidade CFM
**[cfm-compliance/](./system-admin/cfm-compliance/)**
- Resoluções CFM 1.821, 1.638, 2.314
- Prontuários eletrônicos
- Telemedicina
- Versionamento de dados

---

## 🧪 Testes

**Localização:** [tests/](./tests/)

- **[TISS_TUSS_TESTING_GUIDE.md](./tests/TISS_TUSS_TESTING_GUIDE.md)** - Guia de testes TISS/TUSS
- Suítes de testes automatizados
- Testes unitários e E2E

---

## 🚀 Scripts e Ferramentas

### Scripts de Setup
- **[setup-macos.sh](./setup-macos.sh)** - Setup automatizado para macOS
- **[setup-windows.ps1](./setup-windows.ps1)** - Setup automatizado para Windows

### Scripts de Migração
- **[run-all-migrations.sh](./run-all-migrations.sh)** - Executar todas migrações (Unix)
- **[run-all-migrations.ps1](./run-all-migrations.ps1)** - Executar todas migrações (Windows)

### Scripts de Teste
- **[TESTE_API_RAPIDO.sh](./TESTE_API_RAPIDO.sh)** - Teste rápido de API (Unix)
- **[TESTE_API_RAPIDO.bat](./TESTE_API_RAPIDO.bat)** - Teste rápido de API (Windows)
- **[test_jwt.sh](./test_jwt.sh)** - Teste de autenticação JWT

---

## 🐳 Docker e Infraestrutura

### Arquivos de Configuração
- **[docker-compose.yml](./docker-compose.yml)** - Desenvolvimento local
- **[docker-compose.production.yml](./docker-compose.production.yml)** - Produção
- **[docker-compose.microservices.yml](./docker-compose.microservices.yml)** - Microserviços
- **[docker-compose.seq.yml](./docker-compose.seq.yml)** - Logging com Seq

### Podman (Alternativa Open Source)
- **[podman-compose.yml](./podman-compose.yml)** - Desenvolvimento local
- **[podman-compose.production.yml](./podman-compose.production.yml)** - Produção
- **[podman-compose.microservices.yml](./podman-compose.microservices.yml)** - Microserviços

**Guia de Migração:** [system-admin/infrastructure/DOCKER_TO_PODMAN_MIGRATION.md](./system-admin/infrastructure/DOCKER_TO_PODMAN_MIGRATION.md)

---

## 📱 Aplicações

### Frontend Web
**[frontend/medicwarehouse-app/](./frontend/medicwarehouse-app/)**
- Aplicação Angular 20 unificada
- Clínica, Admin e Frontend de clientes

### Patient Portal API
**[patient-portal-api/](./patient-portal-api/)**
- API dedicada para portal do paciente
- Node.js + Express

### Telemedicina
**[telemedicine/](./telemedicine/)**
- Serviço de telemedicina
- WebRTC para videochamadas

### Mobile (Arquivado - Migrado para PWA)
**[mobile/](./mobile/)**
- **⚠️ Descontinuado:** Apps nativos iOS e Android foram substituídos por PWA
- Código mantido apenas para referência histórica

---

## 🔍 Como Navegar Esta Documentação

### Para Iniciar Desenvolvimento
1. **[README.md](./README.md)** - Visão geral e setup inicial
2. **[system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md](./system-admin/guias/GUIA_INICIO_RAPIDO_LOCAL.md)** - Setup em 10 minutos
3. **[system-admin/guias/GUIA_MULTIPLATAFORMA.md](./system-admin/guias/GUIA_MULTIPLATAFORMA.md)** - Setup específico por plataforma

### Para Entender Implementações
1. **[system-admin/implementacoes/INDEX.md](./system-admin/implementacoes/INDEX.md)** - Índice de todas implementações
2. **[system-admin/docs/RESUMO_TECNICO_COMPLETO.md](./system-admin/docs/RESUMO_TECNICO_COMPLETO.md)** - Visão geral técnica (92% completo)

### Para Planejar Novas Funcionalidades
1. **[Plano_Desenvolvimento/README.md](./Plano_Desenvolvimento/README.md)** - Roadmap completo
2. **[Plano_Desenvolvimento/DEPENDENCIES.md](./Plano_Desenvolvimento/DEPENDENCIES.md)** - Dependências entre tarefas

### Para Conformidade e Segurança
1. **[system-admin/cfm-compliance/](./system-admin/cfm-compliance/)** - Conformidade CFM
2. **[system-admin/seguranca/](./system-admin/seguranca/)** - Segurança e LGPD

### Para Contribuir
1. **[CONTRIBUTING.md](./CONTRIBUTING.md)** - Guia de contribuição
2. **[CHANGELOG.md](./CHANGELOG.md)** - Histórico de mudanças

---

## 📊 Estatísticas de Documentação

| Categoria | Quantidade |
|-----------|------------|
| **Documentos Raiz** | 4 essenciais |
| **Plano Desenvolvimento** | 28 documentos (4 fases) |
| **System-Admin Total** | 300+ documentos |
| ├─ Implementações Ativas | 59 documentos |
| ├─ Guias de Usuário | 48 documentos |
| ├─ Docs Técnicos | 124 documentos |
| ├─ Regras de Negócio | 24 documentos |
| ├─ Segurança | 12 documentos |
| └─ CFM Compliance | 16 documentos |
| **Documentação Arquivada** | 24 documentos |
| **TOTAL** | ~350+ documentos ativos |

---

## 🔄 Política de Atualização

- **Documentação Essencial (README, CONTRIBUTING):** Atualizada conforme necessário
- **Plano de Desenvolvimento:** Revisado trimestralmente
- **Implementações:** Atualizadas durante desenvolvimento
- **Guias:** Atualizados a cada release
- **Arquivamento:** Documentos obsoletos movidos para pastas `archive/` com README explicativo

---

## ❓ Precisa de Ajuda?

1. **Dúvidas Gerais:** Veja o [README.md](./README.md) principal
2. **Setup e Configuração:** Consulte [system-admin/guias/](./system-admin/guias/)
3. **Implementações Específicas:** Veja [system-admin/implementacoes/INDEX.md](./system-admin/implementacoes/INDEX.md)
4. **Contribuir:** Leia [CONTRIBUTING.md](./CONTRIBUTING.md)
5. **Roadmap:** Consulte [Plano_Desenvolvimento/README.md](./Plano_Desenvolvimento/README.md)

---

**Última Reorganização:** Janeiro 2026  
**Próxima Revisão:** Abril 2026
