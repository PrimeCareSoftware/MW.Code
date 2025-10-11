# 🚀 CI/CD com GitHub Actions

Este documento descreve a implementação do pipeline de CI/CD para o MedicWarehouse usando GitHub Actions.

## 📋 Visão Geral

O workflow de CI/CD foi configurado para executar automaticamente testes do frontend (Angular) e backend (.NET) em cada push ou pull request para as branches `main` e `develop`.

## 🔧 Estrutura do Workflow

O arquivo de workflow está localizado em: `.github/workflows/ci.yml`

### Jobs Configurados

#### 1. **backend-tests** - Testes do Backend (.NET)
- **Plataforma**: Ubuntu Latest
- **SDK**: .NET 8.0.x
- **Etapas**:
  1. Checkout do código
  2. Configuração do .NET SDK
  3. Restauração de dependências (`dotnet restore`)
  4. Build do projeto (`dotnet build`)
  5. Execução dos testes (`dotnet test`)
  6. Upload dos resultados dos testes (formato TRX)
  7. Upload dos relatórios de cobertura de código

**Comando de Teste**:
```bash
dotnet test --no-build --configuration Release --verbosity normal --logger "trx" --collect:"XPlat Code Coverage"
```

#### 2. **frontend-tests** - Testes do Frontend (Angular)
- **Plataforma**: Ubuntu Latest
- **Node.js**: 20.x
- **Etapas**:
  1. Checkout do código
  2. Configuração do Node.js
  3. Instalação das dependências (`npm ci`)
  4. Execução dos testes com Karma/Jasmine
  5. Upload dos resultados dos testes e cobertura

**Comando de Teste**:
```bash
npm test -- --watch=false --browsers=ChromeHeadless
```

**Variáveis de Ambiente**:
- `CHROME_BIN`: `/usr/bin/google-chrome`
- `CHROMIUM_FLAGS`: `--no-sandbox --disable-setuid-sandbox --disable-dev-shm-usage`

#### 3. **sonar-backend** - Análise SonarCloud do Backend
- **Plataforma**: Ubuntu Latest
- **SDK**: .NET 8.0.x
- **Dependências**: Aguarda conclusão do job `backend-tests`
- **Etapas**:
  1. Checkout do código com histórico completo (fetch-depth: 0)
  2. Configuração do .NET SDK
  3. Instalação do SonarCloud scanner para .NET
  4. Restauração de dependências
  5. Início da análise SonarCloud
  6. Build do projeto
  7. Execução dos testes com cobertura (formato OpenCover)
  8. Finalização e envio da análise para SonarCloud

**Configuração**:
- **Organization**: medicwarehouse
- **Project Key**: MedicWarehouse_MW.Code
- **Coverage Format**: OpenCover
- **Test Results**: TRX format

#### 4. **sonar-frontend** - Análise SonarCloud do Frontend
- **Plataforma**: Ubuntu Latest
- **Node.js**: 20.x
- **Dependências**: Aguarda conclusão do job `frontend-tests`
- **Etapas**:
  1. Checkout do código com histórico completo (fetch-depth: 0)
  2. Configuração do Node.js
  3. Instalação das dependências
  4. Execução dos testes com cobertura de código
  5. Análise e envio para SonarCloud

**Configuração**:
- **Organization**: medicwarehouse
- **Project Key**: MedicWarehouse_MW.Code_Frontend
- **Coverage Format**: LCOV
- **Source Directory**: src
- **Test Inclusions**: \*\*/\*.spec.ts

#### 5. **build-check** - Verificação de Build
- **Plataforma**: Ubuntu Latest
- **Dependências**: Aguarda conclusão dos jobs `backend-tests` e `frontend-tests`
- **Etapas**:
  1. Checkout do código
  2. Configuração do .NET SDK e Node.js
  3. Build do backend em modo Release
  4. Build do frontend em modo produção
  5. Verificação dos artefatos de build

## 🎯 Triggers

O workflow é acionado nas seguintes situações:

### Push
```yaml
push:
  branches: [ main, develop ]
```

### Pull Request
```yaml
pull_request:
  branches: [ main, develop ]
```

### Manual
```yaml
workflow_dispatch:
```
O workflow também pode ser executado manualmente através da interface do GitHub Actions.

## 📊 Resultados e Artefatos

### Artefatos Gerados

1. **backend-test-results**: Resultados dos testes do backend (arquivos .trx)
2. **backend-coverage-reports**: Relatórios de cobertura de código do backend (Cobertura XML)
3. **frontend-test-results**: Resultados dos testes do frontend e relatórios de cobertura

### Visualização dos Resultados

Os resultados podem ser visualizados em:
- **GitHub Actions**: Na aba "Actions" do repositório
- **Pull Requests**: Status checks aparecem automaticamente em cada PR
- **Artefatos**: Disponíveis para download na página de cada execução do workflow

## 🔍 Executar Testes Localmente

### Backend (.NET)

```bash
# Restaurar dependências
dotnet restore

# Executar todos os testes
dotnet test

# Executar com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~ValueObjects"
```

### Frontend (Angular)

```bash
# Navegar para o diretório do frontend
cd frontend/medicwarehouse-app

# Instalar dependências
npm install

# Executar testes (modo watch)
npm test

# Executar testes uma vez (modo CI)
npm test -- --watch=false --browsers=ChromeHeadless

# Executar testes com cobertura
npm test -- --watch=false --code-coverage
```

## 📈 Estatísticas dos Testes

### Backend
- **Total de Testes**: 305
- **ValueObjects**: 170 testes
- **Entidades**: 116 testes
- **Services**: 14 testes
- **Status**: ✅ Todos passando

### Frontend
- **Framework**: Karma + Jasmine
- **Navegador**: Chrome Headless
- **Testes**: Componentes Angular

## 🛠️ Manutenção

### Atualizar Versões

Para atualizar as versões do SDK ou Node.js, edite o arquivo `.github/workflows/ci.yml`:

```yaml
# .NET SDK
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '8.0.x'  # Atualizar aqui

# Node.js
- name: Setup Node.js
  uses: actions/setup-node@v4
  with:
    node-version: '20.x'  # Atualizar aqui
```

### Adicionar Novos Jobs

Para adicionar novos jobs ao workflow, adicione uma nova seção no arquivo YAML:

```yaml
jobs:
  novo-job:
    name: Nome do Job
    runs-on: ubuntu-latest
    needs: [backend-tests, frontend-tests]  # Dependências opcionais
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      # Adicione mais steps aqui
```

## 🔒 Segurança

- Os testes são executados em ambientes isolados do GitHub Actions
- Não há exposição de credenciais ou secrets nos logs
- O Chrome Headless é executado com flags de segurança apropriadas

## 🔍 SonarCloud - Análise de Qualidade de Código

### Configuração

O projeto utiliza **SonarCloud** para análise estática de código e qualidade. A análise é executada automaticamente após os testes serem concluídos com sucesso.

#### Backend (.NET)
- **Scanner**: dotnet-sonarscanner
- **Formato de Cobertura**: OpenCover
- **Project Key**: MedicWarehouse_MW.Code
- **Métricas Analisadas**:
  - Code Smells
  - Bugs
  - Vulnerabilidades
  - Cobertura de Código
  - Duplicação de Código
  - Complexidade Ciclomática

#### Frontend (Angular)
- **Scanner**: SonarCloud GitHub Action
- **Formato de Cobertura**: LCOV
- **Project Key**: MedicWarehouse_MW.Code_Frontend
- **Configuração**: sonar-project.properties
- **Métricas Analisadas**:
  - Code Smells
  - Bugs
  - Vulnerabilidades
  - Cobertura de Código (TypeScript/JavaScript)
  - Duplicação de Código

### Secrets Necessários

Para que a análise SonarCloud funcione, é necessário configurar o seguinte secret no GitHub:

1. **SONAR_TOKEN**: Token de autenticação do SonarCloud
   - Obtido em: https://sonarcloud.io/account/security
   - Configurado em: Settings > Secrets and variables > Actions > New repository secret

### Visualizar Resultados

Os resultados da análise podem ser visualizados em:
- **SonarCloud Dashboard**: https://sonarcloud.io/organizations/medicwarehouse/projects
- **Pull Requests**: Comentários automáticos com quality gate status
- **GitHub Actions**: Logs detalhados da execução

### Quality Gates

O projeto está configurado com quality gates padrão do SonarCloud:
- **Coverage**: Mínimo recomendado de 80%
- **Duplicação**: Máximo de 3%
- **Maintainability Rating**: A ou B
- **Reliability Rating**: A
- **Security Rating**: A

## 📚 Referências

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [.NET Testing](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [Angular Testing](https://angular.dev/guide/testing)
- [Karma Configuration](https://karma-runner.github.io/latest/config/configuration-file.html)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [SonarScanner for .NET](https://docs.sonarcloud.io/advanced-setup/ci-based-analysis/sonarscanner-for-net/)

## 🎯 Próximos Passos

- [x] Adicionar análise de qualidade de código (SonarCloud)
- [ ] Configurar deploy automático para ambientes de staging
- [ ] Adicionar testes de integração E2E com Playwright ou Cypress
- [ ] Configurar notificações de falha por email/Slack
- [ ] Adicionar badges de status no README

## ✅ Status Atual

- ✅ Testes de Backend configurados e funcionando
- ✅ Testes de Frontend configurados e funcionando
- ✅ Build verification implementado
- ✅ Upload de artefatos configurado
- ✅ Suporte a execução manual
- ✅ Análise SonarCloud para Backend
- ✅ Análise SonarCloud para Frontend
- ✅ **Correções de qualidade aplicadas (Outubro 2025)**

## 📝 Histórico de Melhorias de Qualidade

### Outubro 2025 - Correções SonarCloud

Foram aplicadas correções para resolver issues identificados pelo SonarCloud, sem alterar regras de negócio:

#### Fase 1 - Core Domain
1. **Blocos Catch Específicos**: Substituição de catches genéricos por `ArgumentException` e `FormatException`
2. **Parâmetros Nullable**: Explicitação de parâmetros nullable em `MedicalRecord`
3. **Testes Limpos**: Remoção de asserts desnecessários em tipos valor
4. **Constantes de Domínio**: Extração de magic numbers (11, 14) para `DocumentConstants`

**Impacto**: Build passou de 4 warnings para 0 warnings

#### Fase 2 - WhatsAppAgent
5. **Propriedades Nullable**: Marcação de propriedades opcionais como nullable em entities e DTOs
6. **Validação de Webhook**: Adição de validação no `ProcessMessageAsync`
7. **Null-Safety**: Uso de null-coalescing operators e inicialização de construtores EF Core

**Impacto**: Eliminação de 40+ warnings CS8618/CS8604, build limpo com 647 testes passando (100%)
