# 🔍 SonarCloud Setup Guide

Este guia explica como configurar o SonarCloud para o projeto MedicWarehouse.

## 📋 Pré-requisitos

1. Conta no SonarCloud (https://sonarcloud.io)
2. Acesso de administrador ao repositório GitHub
3. Organização criada no SonarCloud

## 🚀 Configuração Inicial

### 1. Criar Organização no SonarCloud

1. Acesse https://sonarcloud.io
2. Faça login com sua conta GitHub
3. Clique em "+" no canto superior direito
4. Selecione "Create new organization"
5. Escolha "medicwarehouse" como nome da organização
6. Autorize o acesso ao GitHub

### 2. Criar Projetos

#### Projeto Backend
1. Na organização, clique em "Analyze new project"
2. Selecione o repositório "MedicWarehouse/MW.Code"
3. Configure:
   - **Project Key**: `MedicWarehouse_MW.Code`
   - **Project Name**: `MedicWarehouse Backend`
4. Escolha "With GitHub Actions"

#### Projeto Frontend
1. Clique novamente em "Analyze new project"
2. Configure manualmente:
   - **Project Key**: `MedicWarehouse_MW.Code_Frontend`
   - **Project Name**: `MedicWarehouse Frontend`
3. Escolha "With GitHub Actions"

### 3. Obter Token de Autenticação

1. No SonarCloud, vá para "My Account" > "Security"
2. Em "Generate Tokens", crie um novo token:
   - **Name**: `GitHub Actions - MW.Code`
   - **Type**: `Project Analysis Token` ou `Global Analysis Token`
3. Copie o token gerado (você não poderá vê-lo novamente)

### 4. Configurar Secret no GitHub

1. Acesse o repositório no GitHub
2. Vá para "Settings" > "Secrets and variables" > "Actions"
3. Clique em "New repository secret"
4. Configure:
   - **Name**: `SONAR_TOKEN`
   - **Value**: Cole o token copiado do SonarCloud
5. Clique em "Add secret"

## 📊 Estrutura de Análise

### Backend (.NET)

O workflow realiza:
- Instalação do scanner `dotnet-sonarscanner`
- Build do projeto .NET
- Execução de testes com cobertura (formato OpenCover)
- Upload dos resultados para SonarCloud

**Métricas analisadas**:
- Code Smells
- Bugs
- Vulnerabilidades de Segurança
- Cobertura de Código
- Duplicação
- Complexidade Ciclomática

### Frontend (Angular)

O workflow realiza:
- Execução de testes com cobertura (formato LCOV)
- Análise com SonarCloud GitHub Action
- Upload dos resultados

**Métricas analisadas**:
- Code Smells TypeScript/JavaScript
- Bugs
- Vulnerabilidades
- Cobertura de Testes
- Duplicação de Código

## 🔧 Configuração de Quality Gates

### Backend

Os quality gates recomendados:
```
Coverage: >= 80%
Duplications: <= 3%
Maintainability Rating: A ou B
Reliability Rating: A
Security Rating: A
```

### Frontend

Os quality gates recomendados:
```
Coverage: >= 70%
Duplications: <= 3%
Maintainability Rating: A ou B
Reliability Rating: A
Security Rating: A
```

## ✅ Verificar Configuração

Após configurar tudo:

1. Faça um commit no repositório
2. Verifique o workflow no GitHub Actions
3. Aguarde a conclusão dos jobs de teste
4. Os jobs SonarCloud serão executados automaticamente
5. Acesse o dashboard do SonarCloud para ver os resultados

## 🐛 Troubleshooting

### Erro: "SONAR_TOKEN not found"

**Solução**: Verifique se o secret `SONAR_TOKEN` foi criado corretamente no GitHub.

### Erro: "Project not found"

**Solução**: Verifique se os Project Keys estão corretos:
- Backend: `MedicWarehouse_MW.Code`
- Frontend: `MedicWarehouse_MW.Code_Frontend`

### Erro: "Organization not found"

**Solução**: Certifique-se de que a organização "medicwarehouse" existe no SonarCloud e que você tem acesso.

### Erro de Cobertura: "Coverage report not found"

**Solução Backend**: Verifique se os testes estão gerando relatórios no formato OpenCover em `**/TestResults/**/coverage.opencover.xml`

**Solução Frontend**: Verifique se os testes estão gerando o arquivo `coverage/lcov.info`

## 📚 Recursos

- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [SonarScanner for .NET](https://docs.sonarcloud.io/advanced-setup/ci-based-analysis/sonarscanner-for-net/)
- [SonarCloud GitHub Action](https://github.com/SonarSource/sonarcloud-github-action)

## 🎯 Próximos Passos

Após a configuração:

1. ✅ Configure quality gates personalizados
2. ✅ Configure notificações por email
3. ✅ Adicione badges do SonarCloud ao README
4. ✅ Configure pull request decoration
5. ✅ Revise e corrija issues encontrados
