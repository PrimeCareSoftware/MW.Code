# TISS/TUSS Unit Testing Guide

## Overview
This document provides comprehensive patterns and examples for creating unit tests for the TISS/TUSS functionality in the MedicSoft system.

## Test Structure

### 1. Domain Entity Tests ✅ COMPLETED
Location: `tests/MedicSoft.Test/Entities/`

All domain entity tests have been completed with 212 passing tests covering:
- HealthInsuranceOperatorTests.cs (19 tests)
- PatientHealthInsuranceTests.cs (33 tests)
- AuthorizationRequestTests.cs (35 tests)
- TissBatchTests.cs (30 tests)
- TissGuideTests.cs (32 tests)
- TissGuideProcedureTests.cs (30 tests)
- TussProcedureTests.cs (27 tests)
- HealthInsurancePlanTests.cs (Updated with 5 new TISS-related tests)

#### Test Patterns for Entity Tests

```csharp
using FluentAssertions;
using Xunit;

[Fact]
public void Constructor_WithValidData_CreatesEntity()
{
    // Arrange
    var data = "test data";
    
    // Act
    var entity = new Entity(data, tenantId);
    
    // Assert
    entity.Should().NotBeNull();
    entity.PropertyName.Should().Be(data);
}

[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void Constructor_WithInvalidData_ThrowsArgumentException(string? invalidData)
{
    // Act
    var act = () => new Entity(invalidData!, tenantId);
    
    // Assert
    act.Should().Throw<ArgumentException>()
        .WithMessage("*expected message*");
}

[Fact]
public void BusinessMethod_WithValidState_UpdatesEntity()
{
    // Arrange
    var entity = CreateValidEntity();
    
    // Act
    entity.SomeBusinessMethod();
    
    // Assert
    entity.State.Should().Be(ExpectedState);
    entity.UpdatedAt.Should().NotBeNull();
}
```

### 2. Service Tests (PARTIAL)
Location: `tests/MedicSoft.Test/Services/`

#### Pattern for Service Tests

```csharp
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMapper;
using Xunit;

public class ServiceTests
{
    private const string TenantId = "test-tenant";
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Service _service;

    public ServiceTests()
    {
        _repositoryMock = new Mock<IRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new Service(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesEntity()
    {
        // Arrange
        var dto = new CreateDto { /* properties */ };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TenantId))
            .ReturnsAsync((Entity?)null);
        
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Entity>()))
            .Returns(Task.CompletedTask);
        
        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.FromResult(1));

        var expectedDto = new EntityDto { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<EntityDto>(It.IsAny<Entity>()))
            .Returns(expectedDto);

        // Act
        var result = await _service.CreateAsync(dto, TenantId);

        // Assert
        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Entity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentEntity_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateDto();
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id, TenantId))
            .ReturnsAsync((Entity?)null);

        // Act
        var act = async () => await _service.UpdateAsync(id, dto, TenantId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }
}
```

#### Services Requiring Tests

1. **HealthInsuranceOperatorServiceTests.cs** (IN PROGRESS)
   - CreateAsync
   - UpdateAsync
   - ConfigureIntegrationAsync
   - ConfigureTissAsync
   - GetAllAsync
   - GetByIdAsync
   - GetByRegisterNumberAsync
   - SearchByNameAsync
   - ActivateAsync/DeactivateAsync
   - DeleteAsync

2. **PatientHealthInsuranceServiceTests.cs**
   - CreateAsync
   - UpdateAsync
   - UpdateCardInfoAsync
   - UpdateValidityPeriodAsync
   - UpdateHolderInfoAsync
   - GetByPatientIdAsync
   - GetActiveByPatientIdAsync
   - ValidateAsync
   - ActivateAsync/DeactivateAsync

3. **AuthorizationRequestServiceTests.cs**
   - CreateAsync
   - ApproveAsync
   - DenyAsync
   - CancelAsync
   - GetByIdAsync
   - GetByPatientIdAsync
   - GetPendingRequestsAsync
   - CheckExpirationAsync

4. **TissGuideServiceTests.cs**
   - CreateAsync
   - AddProcedureAsync
   - RemoveProcedureAsync
   - MarkAsSentAsync
   - ApproveAsync
   - RejectAsync
   - GetByBatchIdAsync

5. **TissBatchServiceTests.cs**
   - CreateAsync
   - AddGuideAsync
   - RemoveGuideAsync
   - MarkAsReadyToSendAsync
   - GenerateXmlAsync
   - SubmitAsync
   - ProcessResponseAsync
   - GetByOperatorIdAsync

6. **TussProcedureServiceTests.cs** (IN PROGRESS)
   - CreateAsync
   - UpdateAsync
   - GetAllAsync
   - GetByIdAsync
   - GetByCodeAsync
   - ActivateAsync/DeactivateAsync

7. **TissXmlGeneratorServiceTests.cs**
   - GenerateBatchXmlAsync
   - ValidateXmlAsync
   - ParseResponseXmlAsync

### 3. Controller Tests
Location: `tests/MedicSoft.Test/Api/Controllers/`

#### Pattern for Controller Tests

```csharp
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Moq;
using Xunit;

public class ControllerTests
{
    private const string TenantId = "test-tenant";
    private readonly Mock<IService> _serviceMock;
    private readonly Controller _controller;

    public ControllerTests()
    {
        _serviceMock = new Mock<IService>();
        _controller = new Controller(_serviceMock.Object);
        
        // Setup HttpContext with TenantId claim
        var claims = new List<Claim>
        {
            new Claim("TenantId", TenantId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var dto = new CreateDto();
        var expectedDto = new EntityDto { Id = Guid.NewGuid() };
        
        _serviceMock.Setup(s => s.CreateAsync(dto, TenantId))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().Be(expectedDto);
    }

    [Fact]
    public async Task GetById_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new EntityDto { Id = id };
        
        _serviceMock.Setup(s => s.GetByIdAsync(id, TenantId))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(dto);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _serviceMock.Setup(s => s.GetByIdAsync(id, TenantId))
            .ReturnsAsync((EntityDto?)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
```

#### Controllers Requiring Tests

1. **HealthInsuranceOperatorsControllerTests.cs**
   - GET /api/healthinsuranceoperators
   - GET /api/healthinsuranceoperators/{id}
   - POST /api/healthinsuranceoperators
   - PUT /api/healthinsuranceoperators/{id}
   - DELETE /api/healthinsuranceoperators/{id}
   - POST /api/healthinsuranceoperators/{id}/configure-integration
   - POST /api/healthinsuranceoperators/{id}/configure-tiss
   - POST /api/healthinsuranceoperators/{id}/activate
   - POST /api/healthinsuranceoperators/{id}/deactivate

2. **PatientHealthInsuranceControllerTests.cs**
3. **AuthorizationRequestsControllerTests.cs**
4. **TissGuidesControllerTests.cs**
5. **TissBatchesControllerTests.cs**
6. **TussProceduresControllerTests.cs**

## Common Testing Patterns

### Testing Multi-Tenancy
```csharp
[Fact]
public async Task GetAll_FiltersBy TenantId()
{
    // Arrange
    var tenantData = new List<Entity> { /* tenant data */ };
    
    _repositoryMock.Setup(r => r.GetAllAsync(TenantId, It.IsAny<bool>()))
        .ReturnsAsync(tenantData);

    // Act
    var result = await _service.GetAllAsync(TenantId);

    // Assert
    result.Should().HaveCount(tenantData.Count);
    _repositoryMock.Verify(r => r.GetAllAsync(TenantId, It.IsAny<bool>()), Times.Once);
}
```

### Testing State Transitions
```csharp
[Fact]
public void MarkAsSent_WithDraftStatus_ChangesStatusToSent()
{
    // Arrange
    var guide = CreateGuide();
    guide.AddProcedure(CreateProcedure());

    // Act
    guide.MarkAsSent();

    // Assert
    guide.Status.Should().Be(GuideStatus.Sent);
}

[Fact]
public void MarkAsSent_WithInvalidStatus_ThrowsInvalidOperationException()
{
    // Arrange
    var guide = CreateGuide();
    guide.AddProcedure(CreateProcedure());
    guide.MarkAsSent();

    // Act
    var act = () => guide.MarkAsSent();

    // Assert
    act.Should().Throw<InvalidOperationException>();
}
```

### Testing Business Logic
```csharp
[Fact]
public void IsValid_WithActiveAndValidDates_ReturnsTrue()
{
    // Arrange
    var validFrom = DateTime.UtcNow.AddDays(-30);
    var validUntil = DateTime.UtcNow.AddDays(30);
    var entity = new Entity(validFrom, validUntil, TenantId);

    // Act
    var isValid = entity.IsValid();

    // Assert
    isValid.Should().BeTrue();
}
```

## Best Practices

1. **Use FluentAssertions** for readable assertions
2. **Use Theory and InlineData** for parameterized tests
3. **Mock only interfaces**, not concrete classes
4. **One Assert per Test** when possible
5. **Follow AAA Pattern**: Arrange, Act, Assert
6. **Use descriptive test names**: `MethodName_StateUnderTest_ExpectedBehavior`
7. **Test edge cases**: null, empty, invalid data
8. **Test multi-tenancy**: always verify TenantId filtering
9. **Test business rules**: validation, state transitions, calculations

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter "FullyQualifiedName~TissBatchTests"

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run TISS/TUSS related tests
dotnet test --filter "FullyQualifiedName~Tiss|FullyQualifiedName~Tuss|FullyQualifiedName~HealthInsurance|FullyQualifiedName~Authorization"
```

## Coverage Goals

- **Domain Entities**: >90% coverage ✅ ACHIEVED
- **Services**: >80% coverage (IN PROGRESS)
- **Controllers**: >75% coverage (PENDING)

## Notes

- Entity tests completed: 212 tests passing
- Service tests started: 2 files with patterns
- Controller tests: Patterns documented, implementation pending

## Next Steps

1. Complete remaining service tests following the patterns provided
2. Implement controller tests using the documented patterns
3. Run coverage analysis to verify >80% overall coverage
4. Fix any failing tests
5. Add integration tests for critical workflows
