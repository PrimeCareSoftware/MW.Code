# 🧪 Guia de Testes - Sistema de Configuração de Módulos

> **Documento:** Guia Prático de Testes  
> **Versão:** 1.0  
> **Data:** 29 de Janeiro de 2026

---

## 📋 Índice

1. [Introdução](#introdução)
2. [Configuração do Ambiente](#configuração-do-ambiente)
3. [Executando os Testes](#executando-os-testes)
4. [Estrutura de Testes](#estrutura-de-testes)
5. [Escrevendo Novos Testes](#escrevendo-novos-testes)
6. [Debugging de Testes](#debugging-de-testes)
7. [CI/CD](#cicd)
8. [Boas Práticas](#boas-práticas)

---

## 📚 Introdução

Este guia fornece instruções completas para executar, entender e criar testes para o Sistema de Configuração de Módulos.

### O que está Coberto

- ✅ 74 testes automatizados
- ✅ Testes unitários (services e controllers)
- ✅ Testes de integração
- ✅ Testes de segurança
- ✅ Mocking e isolamento
- ✅ Cobertura de código

---

## 🛠️ Configuração do Ambiente

### Pré-requisitos

```bash
# .NET SDK 8.0 ou superior
dotnet --version
# Saída esperada: 8.0.x

# Git
git --version
```

### Instalação

```bash
# 1. Clonar o repositório
git clone https://github.com/PrimeCareSoftware/MW.Code.git
cd MW.Code

# 2. Restaurar dependências
dotnet restore

# 3. Verificar instalação
dotnet test --list-tests | grep ModuleConfig
```

### Ferramentas Opcionais

#### ReportGenerator (para relatórios HTML)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

#### Coverage Gutters (VS Code Extension)

Instale a extensão "Coverage Gutters" para visualização inline de cobertura.

---

## 🚀 Executando os Testes

### Comandos Básicos

#### Todos os Testes de Módulos

```bash
dotnet test --filter "FullyQualifiedName~ModuleConfig"
```

#### Testes por Arquivo

```bash
# Service tests
dotnet test --filter "FullyQualifiedName~ModuleConfigurationServiceTests"

# Controller tests
dotnet test --filter "FullyQualifiedName~ModuleConfigControllerTests"

# Security tests
dotnet test --filter "FullyQualifiedName~ModulePermissionsTests"

# Integration tests
dotnet test --filter "FullyQualifiedName~ModuleConfigIntegrationTests"
```

#### Teste Específico

```bash
dotnet test --filter "FullyQualifiedName~EnableModule_WithValidPlan"
```

### Opções Úteis

#### Modo Verbose

```bash
dotnet test --filter "FullyQualifiedName~ModuleConfig" --verbosity detailed
```

#### Sem Build

```bash
# Se já compilou recentemente
dotnet test --no-build --filter "FullyQualifiedName~ModuleConfig"
```

#### Modo Watch (re-executar ao salvar)

```bash
dotnet watch test --filter "FullyQualifiedName~ModuleConfig"
```

### Cobertura de Código

#### Gerar Cobertura

```bash
dotnet test \
  --filter "FullyQualifiedName~ModuleConfig" \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

#### Gerar Relatório HTML

```bash
# 1. Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# 2. Gerar relatório
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# 3. Abrir no navegador
open coveragereport/index.html  # macOS
xdg-open coveragereport/index.html  # Linux
start coveragereport/index.html  # Windows
```

---

## 📁 Estrutura de Testes

### Localização dos Arquivos

```
tests/
└── MedicSoft.Test/
    ├── Services/
    │   └── ModuleConfigurationServiceTests.cs   (26 testes)
    ├── Controllers/
    │   └── ModuleConfigControllerTests.cs       (20 testes)
    ├── Security/
    │   └── ModulePermissionsTests.cs            (18 testes)
    └── Integration/
        └── ModuleConfigIntegrationTests.cs      (10 testes)
```

### Anatomia de um Teste

```csharp
public class ModuleConfigurationServiceTests
{
    // 1. Dependências mockadas
    private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepo;
    private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepo;
    private readonly MedicSoftDbContext _context;
    private readonly ModuleConfigurationService _service;

    // 2. Setup no construtor
    public ModuleConfigurationServiceTests()
    {
        // Criar mocks
        _mockSubscriptionRepo = new Mock<IClinicSubscriptionRepository>();
        _mockPlanRepo = new Mock<ISubscriptionPlanRepository>();
        
        // Configurar database in-memory
        var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new MedicSoftDbContext(options);
        
        // Criar serviço
        _service = new ModuleConfigurationService(
            _context,
            _mockSubscriptionRepo.Object,
            _mockPlanRepo.Object,
            Mock.Of<ILogger<ModuleConfigurationService>>()
        );
    }

    // 3. Teste usando padrão AAA
    [Fact]
    public async Task EnableModule_WithValidPlan_ShouldEnableModule()
    {
        // Arrange - Preparar dados
        var clinicId = Guid.NewGuid();
        var moduleName = SystemModules.Reports;
        
        var clinic = new Clinic("Test Clinic", "test-tenant");
        await _context.Clinics.AddAsync(clinic);
        await _context.SaveChangesAsync();
        
        var plan = new SubscriptionPlan(
            "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
            SubscriptionPlanType.Standard, "test-tenant",
            hasReports: true
        );
        
        var subscription = new ClinicSubscription(clinic.Id, plan.Id, "test-tenant");
        
        _mockSubscriptionRepo
            .Setup(r => r.GetByClinicIdAsync(clinic.Id, It.IsAny<string>()))
            .ReturnsAsync(subscription);
            
        _mockPlanRepo
            .Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<string>()))
            .ReturnsAsync(plan);

        // Act - Executar ação
        await _service.EnableModuleAsync(clinic.Id, moduleName, "test-user");

        // Assert - Verificar resultado
        var config = await _context.ModuleConfigurations
            .FirstOrDefaultAsync(mc => mc.ClinicId == clinic.Id && mc.ModuleName == moduleName);
            
        config.Should().NotBeNull();
        config!.IsEnabled.Should().BeTrue();
    }

    // 4. Cleanup (se necessário)
    public void Dispose()
    {
        _context?.Dispose();
    }
}
```

---

## ✏️ Escrevendo Novos Testes

### Passo 1: Criar Arquivo de Teste

```bash
# Escolha a pasta apropriada
cd tests/MedicSoft.Test/Services  # ou Controllers, Security, Integration
```

### Passo 2: Estrutura Básica

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Test.Services
{
    public class MinhaClasseTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly MinhaClasse _service;

        public MinhaClasseTests()
        {
            // Setup
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new MedicSoftDbContext(options);
            
            _service = new MinhaClasse(_context);
        }

        [Fact]
        public async Task MeuMetodo_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            
            // Act
            
            // Assert
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
```

### Passo 3: Adicionar Testes

#### Teste de Sucesso

```csharp
[Fact]
public async Task EnableModule_WithValidData_ShouldSucceed()
{
    // Arrange
    var clinicId = Guid.NewGuid();
    
    // Act
    await _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user");
    
    // Assert
    var config = await _context.ModuleConfigurations
        .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId);
    config.Should().NotBeNull();
}
```

#### Teste de Erro

```csharp
[Fact]
public async Task EnableModule_WithInvalidPlan_ShouldThrowException()
{
    // Arrange
    var clinicId = Guid.NewGuid();
    
    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user")
    );
}
```

#### Teste Parametrizado

```csharp
[Theory]
[InlineData(SystemModules.PatientManagement, true)]
[InlineData(SystemModules.Reports, false)]
[InlineData(SystemModules.TissExport, false)]
public async Task IsCore_ShouldReturnCorrectValue(string moduleName, bool expectedIsCore)
{
    // Act
    var moduleInfo = SystemModules.GetModuleInfo(moduleName);
    
    // Assert
    moduleInfo.IsCore.Should().Be(expectedIsCore);
}
```

### Passo 4: Mocking

#### Mock de Repositório

```csharp
var mockRepo = new Mock<IModuleConfigurationRepository>();

mockRepo
    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
    .ReturnsAsync(new ModuleConfiguration(/* params */));

mockRepo
    .Setup(r => r.AddAsync(It.IsAny<ModuleConfiguration>()))
    .Returns(Task.CompletedTask);
```

#### Verificar Chamadas

```csharp
// Verificar que foi chamado exatamente uma vez
mockRepo.Verify(r => r.AddAsync(It.IsAny<ModuleConfiguration>()), Times.Once);

// Verificar que nunca foi chamado
mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);

// Verificar parâmetros específicos
mockRepo.Verify(r => r.AddAsync(
    It.Is<ModuleConfiguration>(mc => mc.ModuleName == SystemModules.Reports)
), Times.Once);
```

---

## 🐛 Debugging de Testes

### Visual Studio Code

1. Abra o arquivo de teste
2. Clique no ícone de "debug" ao lado do teste
3. Ou use `F5` para debugar o teste atual

### Visual Studio

1. Abra o Test Explorer (Test > Test Explorer)
2. Clique com botão direito no teste
3. Selecione "Debug"

### Linha de Comando

```bash
# Executar com verbose para ver mais detalhes
dotnet test --filter "FullyQualifiedName~MeuTeste" --verbosity detailed

# Usar logger de console
dotnet test --filter "FullyQualifiedName~MeuTeste" --logger "console;verbosity=detailed"
```

### Dicas de Debugging

#### 1. Adicionar Logging

```csharp
var mockLogger = new Mock<ILogger<ModuleConfigurationService>>();
var service = new ModuleConfigurationService(/* ... */, mockLogger.Object);

// Os logs aparecerão na saída do teste
```

#### 2. Inspecionar Database In-Memory

```csharp
[Fact]
public async Task MeuTeste()
{
    // ...
    
    // Inspecionar dados salvos
    var allConfigs = await _context.ModuleConfigurations.ToListAsync();
    Console.WriteLine($"Found {allConfigs.Count} configurations");
    
    // ...
}
```

#### 3. Breakpoints Condicionais

```csharp
[Theory]
[InlineData("Module1")]
[InlineData("Module2")]
[InlineData("Module3")]
public async Task MeuTeste(string moduleName)
{
    // Breakpoint só para "Module2"
    if (moduleName == "Module2")
    {
        System.Diagnostics.Debugger.Break();
    }
    
    // ...
}
```

---

## 🔄 CI/CD

### GitHub Actions

Os testes são executados automaticamente via GitHub Actions.

**Workflow:** `.github/workflows/module-config-tests.yml`

#### Quando é Executado

- ✅ Push para `main` ou `develop`
- ✅ Pull Requests para essas branches
- ✅ Alterações em `src/**` ou `tests/**`

#### Visualizar Resultados

1. Vá para o repositório no GitHub
2. Clique em "Actions"
3. Selecione o workflow "Module Config Tests"
4. Visualize os resultados

#### Verificar Cobertura

Os relatórios de cobertura são enviados para Codecov automaticamente.

### Executar Localmente (Simular CI)

```bash
# Simular o que o CI fará
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --filter "FullyQualifiedName~ModuleConfig"
```

---

## 📝 Boas Práticas

### 1. Nomenclatura

```csharp
// ✅ BOM - Descreve o que o teste faz
[Fact]
public async Task EnableModule_WithValidPlan_ShouldEnableModule()

// ❌ EVITAR - Nome genérico
[Fact]
public async Task Test1()
```

### 2. Um Assert por Conceito

```csharp
// ✅ BOM - Testa um conceito específico
[Fact]
public async Task EnableModule_ShouldSetIsEnabledToTrue()
{
    await _service.EnableModuleAsync(clinicId, module, user);
    var config = await GetConfig(clinicId, module);
    config.IsEnabled.Should().BeTrue();
}

// ✅ BOM - Outro teste para outro conceito
[Fact]
public async Task EnableModule_ShouldCreateAuditLog()
{
    await _service.EnableModuleAsync(clinicId, module, user);
    var history = await GetHistory(clinicId, module);
    history.Should().NotBeNull();
}
```

### 3. Arrange-Act-Assert

Sempre separe claramente as três seções:

```csharp
[Fact]
public async Task MeuTeste()
{
    // Arrange - Preparação
    var data = PrepareTestData();
    
    // Act - Ação
    var result = await _service.DoSomething(data);
    
    // Assert - Verificação
    result.Should().NotBeNull();
}
```

### 4. Testes Independentes

```csharp
// ✅ BOM - Cada teste cria seus próprios dados
[Fact]
public async Task Test1()
{
    var clinic = await CreateTestClinic("Clinic1");
    // ...
}

[Fact]
public async Task Test2()
{
    var clinic = await CreateTestClinic("Clinic2");
    // ...
}

// ❌ EVITAR - Testes dependentes
private Clinic _sharedClinic;

[Fact]
public async Task Test1()
{
    _sharedClinic = await CreateTestClinic();
}

[Fact]
public async Task Test2()
{
    // Depende de Test1 ter executado
    Assert.NotNull(_sharedClinic);
}
```

### 5. Cleanup Apropriado

```csharp
public class MeusTests : IDisposable
{
    private readonly MedicSoftDbContext _context;
    
    public MeusTests()
    {
        // Setup
        _context = CreateContext();
    }
    
    public void Dispose()
    {
        // Cleanup
        _context?.Dispose();
    }
}
```

### 6. Mensagens de Erro Claras

```csharp
// ✅ BOM - Mensagem descritiva
config.IsEnabled.Should().BeTrue("o módulo deveria estar habilitado após EnableModuleAsync");

// ✅ BOM - FluentAssertions já fornece boas mensagens
config.Should().NotBeNull();
```

### 7. Testar Comportamento, Não Implementação

```csharp
// ✅ BOM - Testa o comportamento público
var result = await _service.EnableModule(id);
result.IsEnabled.Should().BeTrue();

// ❌ EVITAR - Testa detalhes de implementação
_service._internalField.Should().Be(expectedValue);
```

---

## 📊 Métricas e Cobertura

### Objetivos de Cobertura

- **Mínimo:** 70%
- **Objetivo:** 80%
- **Ideal:** 90%+

### O que Deve Ser Testado

✅ **Prioridade Alta:**
- Lógica de negócio
- Validações
- Segurança e permissões
- Fluxos críticos

⚠️ **Prioridade Média:**
- DTOs e mapeamentos
- Helpers e utilitários
- Configurações

⏸️ **Baixa Prioridade:**
- Getters/setters simples
- Constantes
- Propriedades auto-implementadas

### Visualizar Cobertura

```bash
# Gerar cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ver resumo
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:"TextSummary"
  
cat coveragereport/Summary.txt
```

---

## 🆘 Troubleshooting

### Problema: Testes Falhando Aleatoriamente

**Causa:** Testes não são independentes ou há condições de corrida.

**Solução:**
- Garantir que cada teste cria seus próprios dados
- Usar banco in-memory com nome único (`Guid.NewGuid().ToString()`)
- Evitar variáveis estáticas compartilhadas

### Problema: Testes Lentos

**Causa:** Muitas operações de I/O ou configurações pesadas.

**Solução:**
- Usar in-memory database
- Mockar dependências externas
- Evitar `Thread.Sleep()` - usar `Task.Delay()` se necessário

### Problema: "Cannot access disposed object"

**Causa:** Context foi disposto antes do teste terminar.

**Solução:**
```csharp
// Garantir que o contexto vive até o final do teste
public class MeusTests : IDisposable
{
    private readonly MedicSoftDbContext _context;
    
    public void Dispose()
    {
        _context?.Dispose();  // Apenas no final
    }
}
```

### Problema: Mocks Não Funcionam

**Causa:** Setup incorreto do mock.

**Solução:**
```csharp
// ✅ Usar It.IsAny<>() para aceitar qualquer valor
mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
    .ReturnsAsync(expectedValue);

// ✅ Ou especificar valores exatos
mockRepo.Setup(r => r.GetByIdAsync(specificGuid, "tenant"))
    .ReturnsAsync(expectedValue);
```

---

## 📚 Recursos Adicionais

### Documentação

- [xUnit.net](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions](https://fluentassertions.com/)
- [EF Core Testing](https://docs.microsoft.com/en-us/ef/core/testing/)

### Arquivos Relacionados

- [IMPLEMENTACAO_FASE4_TESTES.md](./IMPLEMENTACAO_FASE4_TESTES.md) - Resumo da implementação
- [MODULE_CONFIG_TESTS_SUMMARY.md](../../MODULE_CONFIG_TESTS_SUMMARY.md) - Detalhes técnicos
- [04-PROMPT-TESTES.md](./04-PROMPT-TESTES.md) - Especificação original

---

## 📞 Suporte

### Problemas com Testes?

1. Verifique a [seção de Troubleshooting](#troubleshooting)
2. Consulte a documentação oficial
3. Abra uma issue no GitHub

### Contribuindo

Ao adicionar novos testes:
1. Siga as boas práticas deste guia
2. Execute todos os testes antes de commitar
3. Atualize a documentação se necessário

---

> **Última Atualização:** 29 de Janeiro de 2026  
> **Versão:** 1.0  
> **Autor:** GitHub Copilot Agent
