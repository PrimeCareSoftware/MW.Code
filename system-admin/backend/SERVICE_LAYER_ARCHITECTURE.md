# Arquitetura de Serviços - Omni Care Software

## Visão Geral

Este documento descreve a nova arquitetura em camadas do Omni Care Software, onde todas as APIs utilizam a camada de Application Services em vez de acessar diretamente os repositórios.

## Estrutura em Camadas

### 1. Presentation Layer (API Controllers)
- **Responsabilidade**: Receber requisições HTTP e retornar respostas
- **Não pode**: Acessar repositórios diretamente
- **Pode**: Chamar serviços da camada Application

**Controllers Refatorados**:
- ✅ `OwnersController` - Usa `IOwnerService`
- ✅ `UsersController` - Usa `IUserService`
- ✅ `AuthController` - Usa `IAuthService`
- ✅ `RegistrationController` - Usa `IRegistrationService`
- ✅ `PatientsController` - Usa `IPatientService`
- ✅ `AppointmentsController` - Usa `IAppointmentService`
- ✅ `MedicalRecordsController` - Usa `IMedicalRecordService`

### 2. Application Layer (Services)
- **Responsabilidade**: Implementar lógica de negócio e orquestrar operações
- **Pode**: Chamar repositórios, outros serviços e validar regras de negócio
- **Padrão**: Interface + Implementação

**Serviços Implementados**:
- `IOwnerService` / `OwnerService` - Gerenciamento de proprietários
- `IUserService` / `UserService` - Gerenciamento de usuários
- `IAuthService` / `AuthService` - Autenticação
- `IRegistrationService` / `RegistrationService` - Registro de clínicas
- `IPatientService` / `PatientService` - Gerenciamento de pacientes
- `IAppointmentService` / `AppointmentService` - Gerenciamento de agendamentos
- `IMedicalRecordService` / `MedicalRecordService` - Gerenciamento de prontuários

### 3. Domain Layer
- **Responsabilidade**: Definir entidades, interfaces de repositório e lógica de domínio
- **Contém**: Entidades, Enums, Interfaces de Repositório, Serviços de Domínio

**Interfaces de Repositório**:
- `IOwnerRepository`
- `IUserRepository`
- `IPatientRepository`
- `IAppointmentRepository`
- `IMedicalRecordRepository`
- `IClinicRepository`
- Etc.

### 4. Infrastructure Layer (Repository)
- **Responsabilidade**: Implementar acesso a dados
- **Contém**: Repositórios concretos, DbContext, Migrations

## Benefícios da Arquitetura

### 1. Separação de Responsabilidades
Cada camada tem uma responsabilidade clara e bem definida.

### 2. Testabilidade
Serviços podem ser testados independentemente dos controllers e repositórios.

### 3. Reutilização
Lógica de negócio nos serviços pode ser reutilizada em diferentes controllers.

### 4. Manutenibilidade
Mudanças na lógica de negócio são feitas em um único lugar (serviço).

### 5. Isolamento
Controllers não conhecem detalhes de implementação de acesso a dados.

## Exemplo de Fluxo

### Criação de Usuário

```
┌────────────────┐
│  HTTP Request  │
└────────┬───────┘
         │
         ▼
┌────────────────────────────────────┐
│  UsersController.CreateUser()     │
│  - Valida request                 │
│  - Chama _userService             │
└────────┬───────────────────────────┘
         │
         ▼
┌────────────────────────────────────┐
│  UserService.CreateUserAsync()    │
│  - Valida regras de negócio       │
│  - Hash de senha                  │
│  - Chama _userRepository          │
└────────┬───────────────────────────┘
         │
         ▼
┌────────────────────────────────────┐
│  UserRepository.AddAsync()        │
│  - Persiste no banco              │
│  - Retorna entidade criada        │
└────────┬───────────────────────────┘
         │
         ▼
┌────────────────┐
│  HTTP Response │
└────────────────┘
```

## Registro de Serviços

Todos os serviços são registrados no `Program.cs`:

```csharp
// Application Services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
// ... outros repositórios
```

## Padrões Implementados

### 1. Repository Pattern
Abstração do acesso a dados através de interfaces.

### 2. Service Layer Pattern
Lógica de negócio encapsulada em serviços.

### 3. Dependency Injection
Todas as dependências são injetadas via constructor.

### 4. DTO Pattern
Separação entre entidades de domínio e objetos de transferência.

### 5. CQRS (Command Query Responsibility Segregation)
Separação entre operações de leitura e escrita (parcialmente implementado com MediatR).

## Diretrizes de Desenvolvimento

### Para Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
public class ExemploController : BaseController
{
    private readonly IExemploService _exemploService;

    public ExemploController(
        IExemploService exemploService,
        ITenantContext tenantContext) 
        : base(tenantContext)
    {
        _exemploService = exemploService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateRequest request)
    {
        // Validações simples
        if (string.IsNullOrEmpty(request.Nome))
            return BadRequest("Nome é obrigatório");

        // Chamar serviço
        var result = await _exemploService.CreateAsync(request, GetTenantId());
        
        return Ok(result);
    }
}
```

### Para Services
```csharp
public interface IExemploService
{
    Task<ExemploDto> CreateAsync(CreateRequest request, string tenantId);
    Task<ExemploDto?> GetByIdAsync(Guid id, string tenantId);
}

public class ExemploService : IExemploService
{
    private readonly IExemploRepository _repository;

    public ExemploService(IExemploRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExemploDto> CreateAsync(CreateRequest request, string tenantId)
    {
        // Validações de negócio
        if (await _repository.ExistsByNameAsync(request.Nome, tenantId))
            throw new InvalidOperationException("Nome já existe");

        // Criar entidade
        var entity = new Exemplo(request.Nome, tenantId);
        
        // Persistir
        await _repository.AddAsync(entity);
        
        // Retornar DTO
        return MapToDto(entity);
    }
}
```

### Para Repositories
```csharp
public interface IExemploRepository
{
    Task<Exemplo?> GetByIdAsync(Guid id, string tenantId);
    Task<bool> ExistsByNameAsync(string nome, string tenantId);
    Task AddAsync(Exemplo entity);
    Task UpdateAsync(Exemplo entity);
}

public class ExemploRepository : IExemploRepository
{
    private readonly MedicSoftDbContext _context;

    public ExemploRepository(MedicSoftDbContext context)
    {
        _context = context;
    }

    public async Task<Exemplo?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.Exemplos
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
    }

    public async Task AddAsync(Exemplo entity)
    {
        await _context.Exemplos.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
```

## Migração de Código Existente

### Antes (Controller com Repositório Direto)
```csharp
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateRequest request)
{
    var entity = new Exemplo(request.Nome, GetTenantId());
    await _exemploRepository.AddAsync(entity);
    return Ok(entity);
}
```

### Depois (Controller com Serviço)
```csharp
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateRequest request)
{
    var result = await _exemploService.CreateAsync(request, GetTenantId());
    return Ok(result);
}
```

## Conclusão

A nova arquitetura em camadas proporciona:
- ✅ Melhor organização do código
- ✅ Maior testabilidade
- ✅ Facilidade de manutenção
- ✅ Reutilização de código
- ✅ Separação clara de responsabilidades
- ✅ Preparação para crescimento futuro

---

**Data**: 12 de outubro de 2024
**Versão**: 1.0.0
