# Arquitetura do Portal do Paciente

## 📐 Visão Geral da Arquitetura

O Portal do Paciente segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, garantindo:

- Separação clara de responsabilidades
- Testabilidade
- Manutenibilidade
- Escalabilidade
- Independência de frameworks

## 🏗️ Camadas da Aplicação

### 1. Domain Layer (Núcleo)

A camada de domínio contém a lógica de negócio e regras da aplicação. É **independente** de qualquer tecnologia externa.

#### Entidades

**PatientUser**
- Representa um usuário paciente no portal
- Contém informações de autenticação e perfil
- Implementa regras de bloqueio de conta

```csharp
public class PatientUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    // ... outros campos
}
```

**RefreshToken**
- Gerencia tokens de atualização JWT
- Implementa lógica de rotação e revogação

```csharp
public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive => !IsRevoked && !IsExpired;
    // ... outros campos
}
```

**AppointmentView** e **DocumentView**
- Views somente leitura para consultas otimizadas
- Não permitem modificações diretas

#### Interfaces de Repositório

Define contratos para acesso a dados sem implementação concreta:

```csharp
public interface IPatientUserRepository
{
    Task<PatientUser?> GetByIdAsync(Guid id);
    Task<PatientUser?> GetByEmailAsync(string email);
    Task<PatientUser> CreateAsync(PatientUser patientUser);
    // ... outros métodos
}
```

### 2. Application Layer (Casos de Uso)

Orquestra a lógica de negócio e coordena o fluxo de dados entre camadas.

#### DTOs (Data Transfer Objects)

Objetos imutáveis para transferência de dados entre camadas:

**LoginRequestDto**
```csharp
public class LoginRequestDto
{
    [Required]
    public string EmailOrCPF { get; set; }
    
    [Required]
    public string Password { get; set; }
}
```

**LoginResponseDto**
```csharp
public class LoginResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public PatientUserDto User { get; set; }
}
```

#### Serviços

**AuthService**
- Autenticação de pacientes
- Registro de novos usuários
- Gerenciamento de tokens JWT
- Hashing de senhas com PBKDF2

```csharp
public class AuthService : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        // 1. Validar credenciais
        // 2. Verificar bloqueio de conta
        // 3. Verificar senha
        // 4. Gerar tokens JWT
        // 5. Atualizar último login
    }
}
```

**IAppointmentService** e **IDocumentService**
- Interfaces para serviços de agendamentos e documentos (a serem implementados)

### 3. Infrastructure Layer (Implementação)

Implementa os detalhes técnicos de acesso a dados, serviços externos, etc.

#### Entity Framework Core

**PatientPortalDbContext**
```csharp
public class PatientPortalDbContext : DbContext
{
    public DbSet<PatientUser> PatientUsers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações de entidades
        modelBuilder.Entity<PatientUser>()
            .HasIndex(p => p.Email)
            .IsUnique();
            
        modelBuilder.Entity<PatientUser>()
            .HasIndex(p => p.CPF)
            .IsUnique();
    }
}
```

#### Repositórios

Implementações concretas das interfaces de domínio:

```csharp
public class PatientUserRepository : IPatientUserRepository
{
    private readonly PatientPortalDbContext _context;
    
    public async Task<PatientUser?> GetByEmailAsync(string email)
    {
        return await _context.PatientUsers
            .FirstOrDefaultAsync(p => p.Email == email.ToLower());
    }
}
```

### 4. API Layer (Apresentação)

Controllers REST que expõem endpoints HTTP.

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var response = await _authService.LoginAsync(request, ipAddress);
        return Ok(response);
    }
}
```

## 🔐 Fluxo de Autenticação

```
┌─────────────┐         ┌─────────────┐         ┌──────────────────┐
│   Cliente   │         │  API/Auth   │         │   AuthService    │
│  (Angular)  │         │  Controller │         │   (Application)  │
└──────┬──────┘         └──────┬──────┘         └────────┬─────────┘
       │                       │                          │
       │ POST /api/auth/login  │                          │
       │ { email, password }   │                          │
       ├──────────────────────>│                          │
       │                       │ LoginAsync(dto, ip)      │
       │                       ├─────────────────────────>│
       │                       │                          │
       │                       │                ┌─────────▼─────────┐
       │                       │                │ 1. Validar usuário│
       │                       │                │ 2. Verificar senha│
       │                       │                │ 3. Gerar JWT      │
       │                       │                │ 4. Criar refresh  │
       │                       │                └─────────┬─────────┘
       │                       │<─────────────────────────┤
       │                       │  LoginResponseDto        │
       │<──────────────────────┤                          │
       │ { accessToken,        │                          │
       │   refreshToken,       │                          │
       │   user }              │                          │
       │                       │                          │
       │                       │                          │
       │ Armazenar tokens      │                          │
       │ no localStorage       │                          │
       │                       │                          │
```

## 🔄 Fluxo de Refresh Token

```
┌─────────────┐         ┌─────────────┐         ┌──────────────────┐
│   Cliente   │         │  API/Auth   │         │   AuthService    │
└──────┬──────┘         └──────┬──────┘         └────────┬─────────┘
       │                       │                          │
       │ Access token expirado │                          │
       │                       │                          │
       │ POST /api/auth/       │                          │
       │ refresh-token         │                          │
       │ { refreshToken }      │                          │
       ├──────────────────────>│                          │
       │                       │ RefreshTokenAsync()      │
       │                       ├─────────────────────────>│
       │                       │                          │
       │                       │                ┌─────────▼─────────┐
       │                       │                │ 1. Validar token  │
       │                       │                │ 2. Revogar antigo │
       │                       │                │ 3. Gerar novos    │
       │                       │                └─────────┬─────────┘
       │                       │<─────────────────────────┤
       │<──────────────────────┤  Novos tokens            │
       │                       │                          │
```

## 🗄️ Modelo de Dados

### Diagrama ER

```
┌─────────────────────────────────┐
│        PatientUser              │
├─────────────────────────────────┤
│ Id (PK)                    GUID │
│ ClinicId                   GUID │
│ PatientId                  GUID │
│ Email                    VARCHAR│
│ PasswordHash             VARCHAR│
│ CPF                      VARCHAR│
│ FullName                 VARCHAR│
│ PhoneNumber              VARCHAR│
│ DateOfBirth               DATE  │
│ IsActive                  BOOL  │
│ EmailConfirmed            BOOL  │
│ PhoneConfirmed            BOOL  │
│ TwoFactorSecret          VARCHAR│
│ TwoFactorEnabled          BOOL  │
│ AccessFailedCount          INT  │
│ LockoutEnd           TIMESTAMP  │
│ CreatedAt            TIMESTAMP  │
│ UpdatedAt            TIMESTAMP  │
│ LastLoginAt          TIMESTAMP  │
└─────────────────────────────────┘
                │
                │ 1:N
                │
                ▼
┌─────────────────────────────────┐
│        RefreshToken             │
├─────────────────────────────────┤
│ Id (PK)                    GUID │
│ PatientUserId (FK)         GUID │
│ Token                    VARCHAR│
│ ExpiresAt            TIMESTAMP  │
│ CreatedAt            TIMESTAMP  │
│ CreatedByIp              VARCHAR│
│ RevokedAt            TIMESTAMP  │
│ RevokedByIp              VARCHAR│
│ ReplacedByToken          VARCHAR│
│ ReasonRevoked            VARCHAR│
└─────────────────────────────────┘
```

## 🔒 Segurança

### Password Hashing

Utiliza **PBKDF2** (Password-Based Key Derivation Function 2):

- Algoritmo: HMACSHA256
- Iterações: 100.000
- Salt: 128 bits (único por senha)
- Output: 256 bits

```csharp
string HashPassword(string password)
{
    byte[] salt = RandomNumberGenerator.GetBytes(16);
    
    string hashed = Convert.ToBase64String(
        KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32
        )
    );
    
    return $"{Convert.ToBase64String(salt)}:{hashed}";
}
```

### JWT Tokens

**Access Token (curta duração)**
- Validade: 15 minutos
- Contém: userId, email, fullName
- Usado em todas as requisições autenticadas

**Refresh Token (longa duração)**
- Validade: 7 dias
- Armazenado no banco de dados
- Permite renovação do access token
- Rotação automática a cada uso
- Revogação individual

### Account Lockout

- 5 tentativas falhadas → bloqueio de 15 minutos
- Reset do contador após login bem-sucedido
- Logs de todas as tentativas

### Rate Limiting

- 100 requisições por minuto por IP
- Proteção contra brute force
- Configurável por endpoint

## 🧪 Testes

### Estrutura de Testes

```
PatientPortal.Tests/
├── Domain/
│   ├── Entities/
│   │   └── PatientUserTests.cs
│   └── ValueObjects/
├── Application/
│   ├── Services/
│   │   └── AuthServiceTests.cs
│   └── DTOs/
├── Infrastructure/
│   └── Repositories/
│       └── PatientUserRepositoryTests.cs
└── Api/
    └── Controllers/
        └── AuthControllerTests.cs
```

### Exemplo de Teste Unitário

```csharp
public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var mockRepo = new Mock<IPatientUserRepository>();
        var mockTokenService = new Mock<ITokenService>();
        var service = new AuthService(mockRepo.Object, ...);
        
        var request = new LoginRequestDto
        {
            EmailOrCPF = "test@example.com",
            Password = "ValidPass123!"
        };
        
        // Act
        var result = await service.LoginAsync(request, "127.0.0.1");
        
        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }
}
```

## 🚀 Performance

### Otimizações

1. **Índices de Banco de Dados**
   - Email (unique)
   - CPF (unique)
   - PatientUserId em RefreshToken

2. **Caching**
   - Redis para sessões (futuro)
   - Memory cache para dados frequentes

3. **Query Optimization**
   - Projeções específicas (Select)
   - Paginação em listas
   - Async/await para I/O

## 📚 Referências

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design - Eric Evans](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

**Versão:** 1.0.0  
**Autor:** PrimeCare Software Team  
**Data:** Janeiro 2026
