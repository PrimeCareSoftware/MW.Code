# 🧪 PROMPT: Testes - Sistema de Configuração de Módulos

> **Fase:** 4 de 5  
> **Duração Estimada:** 1-2 semanas  
> **Desenvolvedores:** 1  
> **Prioridade:** 🔥🔥 ALTA  
> **Dependências:** 01, 02, 03-PROMPT (concluídos)

---

## 📋 Contexto

Esta fase cobre a implementação de **testes automatizados** completos para o sistema de configuração de módulos.

**Tipos de Testes:**
1. **Testes Unitários** (Backend e Services)
2. **Testes de Integração** (API e Banco de Dados)
3. **Testes E2E** (Frontend)
4. **Testes de Segurança** (Permissões e Validações)

**Objetivo:** Cobertura > 80%

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais

1. Criar testes unitários para serviços e controllers
2. Implementar testes de integração da API
3. Criar testes E2E do frontend
4. Validar segurança e permissões
5. Configurar CI/CD para executar testes

---

## 📝 Tarefas Detalhadas

### 1. Testes Unitários - Backend (3-4 dias)

#### 1.1. Testes do ModuleConfigurationService

**Criar:** `/tests/MedicSoft.Test/Services/ModuleConfigurationServiceTests.cs`

```csharp
using Xunit;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Test.Services
{
    public class ModuleConfigurationServiceTests
    {
        private readonly Mock<IModuleConfigurationRepository> _mockRepository;
        private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepository;
        private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepository;
        private readonly ModuleConfigurationService _service;

        public ModuleConfigurationServiceTests()
        {
            _mockRepository = new Mock<IModuleConfigurationRepository>();
            _mockPlanRepository = new Mock<ISubscriptionPlanRepository>();
            _mockSubscriptionRepository = new Mock<IClinicSubscriptionRepository>();
            
            _service = new ModuleConfigurationService(
                _mockRepository.Object,
                _mockPlanRepository.Object,
                _mockSubscriptionRepository.Object,
                Mock.Of<MedicSoftDbContext>(),
                Mock.Of<ILogger<ModuleConfigurationService>>()
            );
        }

        [Fact]
        public async Task EnableModule_WithValidPlan_ShouldEnableModule()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var moduleName = SystemModules.Reports;
            var userId = "test-user";
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, "tenant-1",
                hasReports: true
            );
            
            var subscription = new ClinicSubscription(clinicId, plan.Id, "tenant-1");
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, It.IsAny<string>()))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<string>()))
                .ReturnsAsync(plan);

            // Act
            await _service.EnableModuleAsync(clinicId, moduleName, userId);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ModuleConfiguration>()), Times.Once);
        }

        [Fact]
        public async Task EnableModule_WithoutValidPlan_ShouldThrowException()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var moduleName = SystemModules.Reports;
            var userId = "test-user";
            
            var plan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, "tenant-1",
                hasReports: false // Reports não disponível no plano Basic
            );
            
            var subscription = new ClinicSubscription(clinicId, plan.Id, "tenant-1");
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, It.IsAny<string>()))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<string>()))
                .ReturnsAsync(plan);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EnableModuleAsync(clinicId, moduleName, userId)
            );
        }

        [Fact]
        public async Task ValidateModuleConfig_WithCoreModule_ShouldReturnInvalid()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var moduleName = SystemModules.PatientManagement; // Core module

            // Act
            var result = await _service.ValidateModuleConfigAsync(clinicId, moduleName);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Core modules cannot be disabled", result.ErrorMessage);
        }

        [Theory]
        [InlineData(SystemModules.PatientManagement, true)]
        [InlineData(SystemModules.Reports, true)]
        [InlineData(SystemModules.WhatsAppIntegration, true)]
        public async Task HasRequiredModules_ShouldValidateCorrectly(
            string moduleName, 
            bool expectedResult)
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            
            // Mock required modules enabled
            _mockRepository
                .Setup(r => r.GetByClinicAndModuleAsync(clinicId, It.IsAny<string>()))
                .ReturnsAsync((Guid cId, string mName) =>
                {
                    return new ModuleConfiguration(cId, mName, "tenant-1", true);
                });

            // Act
            var result = await _service.HasRequiredModulesAsync(clinicId, moduleName);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
```

#### 1.2. Testes do Controller

**Criar:** `/tests/MedicSoft.Test/Controllers/ModuleConfigControllerTests.cs`

```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.Services;

namespace MedicSoft.Test.Controllers
{
    public class ModuleConfigControllerTests
    {
        private readonly Mock<IModuleConfigurationService> _mockService;
        private readonly ModuleConfigController _controller;

        public ModuleConfigControllerTests()
        {
            _mockService = new Mock<IModuleConfigurationService>();
            _controller = new ModuleConfigController(
                Mock.Of<ITenantContext>(),
                Mock.Of<MedicSoftDbContext>(),
                Mock.Of<IClinicSubscriptionRepository>(),
                Mock.Of<ISubscriptionPlanRepository>()
            );
        }

        [Fact]
        public async Task GetModules_ShouldReturnOk()
        {
            // Arrange
            var expectedModules = new List<ModuleDto>
            {
                new ModuleDto { ModuleName = "PatientManagement", IsEnabled = true }
            };

            // Act
            var result = await _controller.GetModules();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task EnableModule_WithValidModule_ShouldReturnOk()
        {
            // Arrange
            var moduleName = SystemModules.Reports;

            // Act
            var result = await _controller.EnableModule(moduleName);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EnableModule_WithInvalidModule_ShouldReturnBadRequest()
        {
            // Arrange
            var moduleName = "InvalidModule";

            // Act
            var result = await _controller.EnableModule(moduleName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
```

---

### 2. Testes de Integração - API (2-3 dias)

#### 2.1. Setup de Testes de Integração

**Criar:** `/tests/MedicSoft.IntegrationTests/ModuleConfigIntegrationTests.cs`

```csharp
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using MedicSoft.Api;

namespace MedicSoft.IntegrationTests
{
    public class ModuleConfigIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ModuleConfigIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetModules_ShouldReturnModulesList()
        {
            // Arrange
            // Add authentication header
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");

            // Act
            var response = await _client.GetAsync("/api/module-config");

            // Assert
            response.EnsureSuccessStatusCode();
            var modules = await response.Content.ReadFromJsonAsync<List<ModuleDto>>();
            Assert.NotNull(modules);
            Assert.NotEmpty(modules);
        }

        [Fact]
        public async Task EnableModule_WithValidData_ShouldReturn200()
        {
            // Arrange
            var moduleName = "Reports";

            // Act
            var response = await _client.PostAsync($"/api/module-config/{moduleName}/enable", null);

            // Assert
            Assert.True(response.IsSuccessStatusCode || 
                       response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetModulesInfo_ShouldReturnAllModules()
        {
            // Act
            var response = await _client.GetAsync("/api/module-config/info");

            // Assert
            response.EnsureSuccessStatusCode();
            var modules = await response.Content.ReadFromJsonAsync<List<ModuleInfoDto>>();
            Assert.NotNull(modules);
            Assert.Equal(13, modules.Count); // 13 módulos definidos
        }

        [Fact]
        public async Task ValidateModule_ShouldReturnValidationResponse()
        {
            // Arrange
            var request = new { ModuleName = "Reports" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/module-config/validate", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var validation = await response.Content.ReadFromJsonAsync<ValidationResponseDto>();
            Assert.NotNull(validation);
        }
    }
}
```

---

### 3. Testes E2E - Frontend (2-3 dias)

#### 3.1. Testes com Cypress (System Admin)

**Criar:** `/frontend/mw-system-admin/cypress/e2e/modules.cy.ts`

```typescript
describe('Modules Dashboard', () => {
  beforeEach(() => {
    cy.login('systemadmin', 'password'); // Helper de login
    cy.visit('/modules');
  });

  it('should display modules dashboard', () => {
    cy.contains('Dashboard de Módulos').should('be.visible');
    cy.get('.kpi-card').should('have.length.greaterThan', 0);
  });

  it('should display module usage table', () => {
    cy.get('.modules-table').should('be.visible');
    cy.get('.modules-table tbody tr').should('have.length.greaterThan', 0);
  });

  it('should navigate to module details', () => {
    cy.get('.modules-table tbody tr').first().find('button').click();
    cy.url().should('include', '/modules/');
  });

  it('should filter modules by category', () => {
    // Implementar filtro e testar
  });
});

describe('Plan Modules Configuration', () => {
  beforeEach(() => {
    cy.login('systemadmin', 'password');
    cy.visit('/modules/plans');
  });

  it('should display plan selector', () => {
    cy.get('mat-select').should('be.visible');
  });

  it('should load modules when plan is selected', () => {
    cy.get('mat-select').click();
    cy.get('mat-option').first().click();
    cy.get('.modules-grid').should('be.visible');
  });

  it('should enable module for plan', () => {
    // Selecionar plano
    cy.get('mat-select').click();
    cy.get('mat-option').first().click();

    // Habilitar módulo
    cy.get('mat-checkbox').first().click();
    
    // Salvar
    cy.contains('Salvar Configurações').click();
    
    // Verificar mensagem de sucesso
    cy.contains('atualizados com sucesso').should('be.visible');
  });

  it('should not allow disabling core modules', () => {
    cy.get('mat-select').click();
    cy.get('mat-option').first().click();

    cy.get('mat-checkbox[disabled]').should('exist');
    cy.contains('CORE').should('be.visible');
  });
});
```

#### 3.2. Testes com Cypress (Clínica)

**Criar:** `/frontend/medicwarehouse-app/cypress/e2e/clinic-modules.cy.ts`

```typescript
describe('Clinic Modules Management', () => {
  beforeEach(() => {
    cy.login('clinicadmin', 'password');
    cy.visit('/clinic-admin/modules');
  });

  it('should display available modules', () => {
    cy.contains('Módulos do Sistema').should('be.visible');
    cy.get('.module-card').should('have.length.greaterThan', 0);
  });

  it('should enable a module', () => {
    // Encontrar módulo desabilitado
    cy.get('.module-card')
      .contains('Desabilitado')
      .parents('.module-card')
      .find('mat-slide-toggle')
      .click();

    // Verificar mensagem de sucesso
    cy.contains('habilitado com sucesso').should('be.visible');
    
    // Verificar que o status mudou
    cy.contains('Habilitado').should('be.visible');
  });

  it('should disable a module', () => {
    cy.get('.module-card')
      .contains('Habilitado')
      .parents('.module-card')
      .find('mat-slide-toggle')
      .click();

    cy.contains('desabilitado').should('be.visible');
  });

  it('should not allow enabling module not in plan', () => {
    cy.get('.module-card')
      .contains('UPGRADE NECESSÁRIO')
      .parents('.module-card')
      .find('mat-slide-toggle')
      .should('not.exist');
  });

  it('should open configuration dialog', () => {
    cy.get('.module-card')
      .contains('Habilitado')
      .parents('.module-card')
      .contains('Configurar')
      .click();

    cy.get('mat-dialog-container').should('be.visible');
    cy.contains('Configurar').should('be.visible');
  });

  it('should save module configuration', () => {
    // Abrir dialog
    cy.get('.module-card')
      .contains('Habilitado')
      .parents('.module-card')
      .contains('Configurar')
      .click();

    // Editar configuração
    cy.get('textarea').clear().type('{"test": "value"}');
    
    // Salvar
    cy.contains('Salvar').click();
    
    // Verificar sucesso
    cy.contains('salva com sucesso').should('be.visible');
  });
});
```

---

### 4. Testes de Segurança (1-2 dias)

#### 4.1. Testes de Permissões

**Criar:** `/tests/MedicSoft.Test/Security/ModulePermissionsTests.cs`

```csharp
using Xunit;

namespace MedicSoft.Test.Security
{
    public class ModulePermissionsTests
    {
        [Fact]
        public async Task SystemAdmin_CanAccessGlobalEndpoints()
        {
            // Test that system admin can access /api/system-admin/modules
        }

        [Fact]
        public async Task RegularUser_CannotAccessGlobalEndpoints()
        {
            // Test that regular users get 403 Forbidden
        }

        [Fact]
        public async Task Clinic_CanOnlyAccessOwnModules()
        {
            // Test that clinic can only see/edit their own modules
        }

        [Fact]
        public async Task CoreModules_CannotBeDisabled()
        {
            // Test that core modules throw error when trying to disable
        }

        [Fact]
        public async Task PlanRestrictions_AreEnforced()
        {
            // Test that modules outside plan cannot be enabled
        }
    }
}
```

---

### 5. Configurar CI/CD (1 dia)

#### 5.1. GitHub Actions Workflow

**Criar:** `/.github/workflows/module-config-tests.yml`

```yaml
name: Module Config Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  backend-tests:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Upload coverage
      uses: codecov/codecov-action@v3

  frontend-tests:
    runs-on: ubuntu-latest
    
    strategy:
      matrix:
        app: [mw-system-admin, medicwarehouse-app]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node
      uses: actions/setup-node@v3
      with:
        node-version: '20'
    
    - name: Install dependencies
      working-directory: ./frontend/${{ matrix.app }}
      run: npm ci
    
    - name: Run tests
      working-directory: ./frontend/${{ matrix.app }}
      run: npm test -- --watch=false --browsers=ChromeHeadless
    
    - name: Run E2E tests
      working-directory: ./frontend/${{ matrix.app }}
      run: npm run e2e
```

---

## ✅ Critérios de Sucesso

### Cobertura de Código
- ✅ Backend: > 80% de cobertura
- ✅ Frontend: > 70% de cobertura
- ✅ Todos os casos críticos testados

### Qualidade dos Testes
- ✅ Testes independentes e isolados
- ✅ Testes rápidos (< 5 min total)
- ✅ Testes estáveis (sem flakiness)
- ✅ Nomenclatura clara

### CI/CD
- ✅ Pipeline executando em cada PR
- ✅ Relatório de cobertura gerado
- ✅ Testes passando antes do merge

---

## 📊 Checklist de Testes

### Backend
- [ ] Testes unitários de services
- [ ] Testes unitários de controllers
- [ ] Testes de integração da API
- [ ] Testes de validações
- [ ] Testes de permissões
- [ ] Testes de auditoria

### Frontend System Admin
- [ ] Testes unitários de services
- [ ] Testes unitários de components
- [ ] Testes E2E de dashboard
- [ ] Testes E2E de configuração de planos
- [ ] Testes de responsividade

### Frontend Clínica
- [ ] Testes unitários de services
- [ ] Testes unitários de components
- [ ] Testes E2E de listagem
- [ ] Testes E2E de habilitar/desabilitar
- [ ] Testes E2E de configuração
- [ ] Testes de validação de plano

---

## ⏭️ Próximos Passos

Após completar este prompt:
1. Executar todos os testes localmente
2. Verificar cobertura de código
3. Corrigir testes que falharem
4. Configurar CI/CD
5. Prosseguir para **05-PROMPT-DOCUMENTACAO.md**

---

> **Status:** 📝 Pronto para desenvolvimento  
> **Última Atualização:** 29 de Janeiro de 2026
