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

#### 3. **build-check** - Verificação de Build
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

## 📚 Referências

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [.NET Testing](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [Angular Testing](https://angular.dev/guide/testing)
- [Karma Configuration](https://karma-runner.github.io/latest/config/configuration-file.html)

## 🎯 Próximos Passos

- [ ] Adicionar análise de qualidade de código (SonarQube, CodeQL)
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
