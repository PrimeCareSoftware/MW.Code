# Documentação: Data Seed do Portal do Paciente

## Visão Geral

Esta funcionalidade permite criar dados de demonstração (seed) para o Portal do Paciente, facilitando o desenvolvimento, testes e demonstrações do sistema.

## Componentes Implementados

### 1. PatientPortalSeederService

**Localização:** `PatientPortal.Infrastructure/Services/PatientPortalSeederService.cs`

**Responsabilidades:**
- Criar usuários do portal a partir de pacientes existentes no banco principal
- Gerar hashes de senha seguros usando PBKDF2
- Buscar dados de pacientes do banco principal via SQL
- Limpar dados do portal quando necessário

**Métodos Principais:**

#### SeedDemoDataAsync()
Cria usuários do portal do paciente baseados nos pacientes do banco principal.

```csharp
public async Task SeedDemoDataAsync()
```

**Processo:**
1. Verifica se já existem usuários no portal
2. Busca pacientes da clínica demo (demo-clinic-001) no banco principal
3. Cria um `PatientUser` para cada paciente encontrado
4. Define senha padrão "Patient@123" com hash seguro
5. Confirma email automaticamente para facilitar testes
6. Salva todos os usuários no banco de dados

**Validações:**
- Impede criação de dados duplicados
- Verifica existência de pacientes no banco principal
- Exige que a clínica demo exista com pacientes

#### GetPatientUsersAsync()
Retorna lista de todos os usuários do portal.

```csharp
public async Task<List<PatientUser>> GetPatientUsersAsync()
```

#### ClearDatabaseAsync()
Remove todos os dados do portal do paciente.

```csharp
public async Task ClearDatabaseAsync()
```

**Ordem de exclusão:**
1. TwoFactorTokens
2. PasswordResetTokens
3. EmailVerificationTokens
4. RefreshTokens
5. PatientUsers

### 2. DataSeederController

**Localização:** `PatientPortal.Api/Controllers/DataSeederController.cs`

**Endpoints:**

#### POST /api/data-seeder/seed-demo

Cria dados de demonstração para o portal do paciente.

**Requisitos:**
- Ambiente de desenvolvimento OU `Development:EnableDevEndpoints = true`
- Banco principal deve estar populado com dados demo
- Clínica demo-clinic-001 deve existir com pacientes

**Resposta de Sucesso (200):**
```json
{
  "message": "Demo data seeded successfully for Patient Portal",
  "tenantId": "demo-clinic-001",
  "credentials": {
    "note": "Use these credentials to login to the patient portal",
    "password": "Patient@123",
    "loginEndpoint": "POST /api/auth/login",
    "users": "All patients from demo clinic can login..."
  },
  "summary": {
    "patientUsers": "Created from existing patients...",
    "emailConfirmed": true,
    "twoFactorEnabled": false
  },
  "nextSteps": [...]
}
```

**Erros Possíveis:**
- **400 Bad Request:** Dados já existem ou pré-requisitos não atendidos
- **403 Forbidden:** Endpoint não disponível em produção
- **500 Internal Server Error:** Erro durante o processo

#### GET /api/data-seeder/demo-info

Retorna informações sobre os dados demo existentes.

**Resposta (200):**
```json
{
  "tenantId": "demo-clinic-001",
  "totalUsers": 6,
  "loginCredentials": {
    "password": "Patient@123",
    "note": "Use any patient email or CPF with this password",
    "endpoint": "POST /api/auth/login"
  },
  "patients": [
    {
      "email": "paciente@exemplo.com",
      "cpf": "12345678900",
      "fullName": "João Silva",
      "emailConfirmed": true,
      "twoFactorEnabled": false
    }
  ],
  "availableEndpoints": [...]
}
```

#### DELETE /api/data-seeder/clear-database

Remove todos os dados do portal do paciente.

**Requisitos:**
- Ambiente de desenvolvimento OU `Development:EnableDevEndpoints = true`

**Resposta de Sucesso (200):**
```json
{
  "message": "Patient Portal database cleared successfully",
  "deletedTables": [
    "TwoFactorTokens",
    "PasswordResetTokens",
    "EmailVerificationTokens",
    "RefreshTokens",
    "PatientUsers"
  ],
  "note": "All patient portal data has been removed..."
}
```

## Configuração

### Registro no Program.cs

O serviço é registrado no container de DI:

```csharp
builder.Services.AddScoped<PatientPortalSeederService>();
```

### Proteção em Produção

Todos os endpoints são protegidos por verificação de ambiente:

```csharp
var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);

if (!_environment.IsDevelopment() && !devModeEnabled)
{
    return StatusCode(StatusCodes.Status403Forbidden, new
    {
        error = "This endpoint is only available in Development environment..."
    });
}
```

## Fluxo de Uso

### 1. Preparação
```bash
# 1. Iniciar banco de dados PostgreSQL
docker-compose up -d postgres

# 2. Popular banco principal com dados demo
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

### 2. Criar Dados do Portal
```bash
# Criar usuários do portal
curl -X POST http://localhost:5001/api/data-seeder/seed-demo
```

### 3. Verificar Dados
```bash
# Ver informações dos usuários criados
curl -X GET http://localhost:5001/api/data-seeder/demo-info
```

### 4. Testar Login
```bash
# Login com email
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrCPF": "paciente@exemplo.com",
    "password": "Patient@123"
  }'

# Ou login com CPF
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrCPF": "12345678900",
    "password": "Patient@123"
  }'
```

### 5. Limpar e Recriar (opcional)
```bash
# Limpar dados
curl -X DELETE http://localhost:5001/api/data-seeder/clear-database

# Recriar
curl -X POST http://localhost:5001/api/data-seeder/seed-demo
```

## Detalhes Técnicos

### Busca de Pacientes

A busca é feita via SQL raw para acessar o banco principal:

```sql
SELECT 
    p."Id" as "PatientId",
    p."ClinicId",
    p."CPF",
    p."Name" as "FullName",
    p."Email",
    p."Phone" as "PhoneNumber",
    p."BirthDate" as "DateOfBirth"
FROM "Patients" p
WHERE p."ClinicId"::text = 'demo-clinic-001'
AND p."Email" IS NOT NULL 
AND p."Email" != ''
AND p."CPF" IS NOT NULL 
AND p."CPF" != ''
ORDER BY p."CreatedAt" DESC
LIMIT 10
```

**Filtros aplicados:**
- Somente clínica demo-clinic-001
- Email obrigatório e não vazio
- CPF obrigatório e não vazio
- Limitado aos 10 pacientes mais recentes

### Hash de Senha

Utiliza PBKDF2 com as seguintes especificações:

- **Algoritmo:** HMACSHA256
- **Iterações:** 100.000
- **Salt:** 128 bits aleatórios
- **Hash:** 256 bits
- **Formato:** `{salt_base64}:{hash_base64}`

```csharp
private string HashPassword(string password)
{
    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
    
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 100000,
        numBytesRequested: 256 / 8));
    
    return $"{Convert.ToBase64String(salt)}:{hashed}";
}
```

### Dados do PatientUser

Cada usuário criado tem:

```csharp
{
    Id = Guid.NewGuid(),
    ClinicId = patient.ClinicId,          // Da clínica demo
    PatientId = patient.PatientId,        // Link com paciente principal
    Email = patient.Email,                // Email do paciente
    PasswordHash = HashPassword("Patient@123"),
    CPF = patient.CPF,                    // CPF do paciente
    FullName = patient.FullName,          // Nome completo
    PhoneNumber = patient.PhoneNumber,    // Telefone
    DateOfBirth = patient.DateOfBirth,    // Data de nascimento
    IsActive = true,                      // Conta ativa
    EmailConfirmed = true,                // Email já confirmado
    PhoneConfirmed = false,               // Telefone não confirmado
    TwoFactorEnabled = false,             // 2FA desabilitado
    AccessFailedCount = 0,                // Sem tentativas falhas
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
}
```

## Segurança

### Proteções Implementadas

1. **Restrição de Ambiente**
   - Endpoints bloqueados em produção por padrão
   - Requer configuração explícita para habilitar

2. **Hash de Senha**
   - PBKDF2 com 100.000 iterações
   - Salt único por usuário
   - Não armazena senha em texto claro

3. **Validação de Dados**
   - Verifica existência de dados antes de criar
   - Valida pré-requisitos
   - Tratamento de erros robusto

4. **Documentação Swagger**
   - Warnings sobre uso em produção
   - Documentação clara dos requisitos
   - Exemplos de resposta

### Considerações de Produção

**⚠️ IMPORTANTE:** Estes endpoints devem ser:

1. **Desabilitados em produção** (comportamento padrão)
2. **Removidos do build de produção** (opcional)
3. **Protegidos por autenticação adicional** se habilitados
4. **Monitorados** se acessados em ambiente não-dev

## Manutenção

### Adicionar Novo Campo ao PatientUser

1. Atualizar `PatientData` DTO
2. Atualizar query SQL em `FetchPatientsFromMainDatabaseAsync()`
3. Atualizar criação do `PatientUser` em `SeedDemoDataAsync()`
4. Atualizar resposta do endpoint `demo-info`

### Alterar Senha Padrão

Modificar em `SeedDemoDataAsync()`:

```csharp
PasswordHash = HashPassword("NovaSenha@123")
```

E atualizar documentação nos endpoints.

### Adicionar Nova Validação

Adicionar checks em `SeedDemoDataAsync()`:

```csharp
// Exemplo: validar número mínimo de pacientes
if (patients.Count < 3)
{
    throw new InvalidOperationException("Minimum 3 patients required");
}
```

## Testes

### Testes Manuais

Ver [DATA_SEEDER_TESTING_GUIDE.md](./DATA_SEEDER_TESTING_GUIDE.md)

### Testes Automatizados

Criar testes em `PatientPortal.Tests`:

```csharp
[Fact]
public async Task SeedDemoData_CreatesPatientUsers()
{
    // Arrange
    var seeder = new PatientPortalSeederService(_context);
    
    // Act
    await seeder.SeedDemoDataAsync();
    
    // Assert
    var users = await _context.PatientUsers.ToListAsync();
    Assert.NotEmpty(users);
}
```

## Referências

- [AuthService.cs](./PatientPortal.Application/Services/AuthService.cs) - Implementação de hash de senha
- [DataSeederController.cs (Main)](../src/MedicSoft.Api/Controllers/DataSeederController.cs) - Padrão seguido
- [DataSeederService.cs (Main)](../src/MedicSoft.Application/Services/DataSeederService.cs) - Referência de implementação

## Changelog

### v1.0.0 (2026-02-06)
- ✨ Implementação inicial do data seeder
- ✨ Três endpoints: seed-demo, demo-info, clear-database
- ✨ Busca automática de pacientes do banco principal
- ✨ Hash de senha com PBKDF2
- ✨ Proteção de ambiente em produção
- 📝 Documentação completa em inglês e português
