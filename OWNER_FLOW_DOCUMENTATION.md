# Fluxo de Proprietários (Owners) - MedicWarehouse

## Visão Geral

O sistema MedicWarehouse agora possui um fluxo separado para gerenciamento de **Proprietários (Owners)** de clínicas, distinto do fluxo de **Usuários (Users)**. Esta separação permite um melhor controle e gerenciamento das permissões e responsabilidades dentro do sistema.

## Diferença entre Owner e User

### Owner (Proprietário)
- **Função**: Proprietário e administrador principal da clínica
- **Permissões**: Controle total sobre a clínica, incluindo gerenciamento de usuários, assinatura e configurações
- **Entidade**: Entidade separada `Owner` no banco de dados
- **Criação**: Criado durante o registro da clínica ou por SystemAdmin
- **Obrigatório**: Uma clínica sempre tem um Owner associado

### User (Usuário)
- **Função**: Profissionais e colaboradores da clínica (médicos, dentistas, enfermeiros, recepcionistas, secretárias)
- **Permissões**: Permissões baseadas em roles (Doctor, Dentist, Nurse, Receptionist, Secretary, ClinicOwner)
- **Entidade**: Entidade `User` no banco de dados
- **Criação**: Criado pelo Owner ou por outro usuário com permissões adequadas
- **Opcional**: Uma clínica pode ter múltiplos usuários

## APIs Disponíveis

### 1. Registro de Clínica com Owner
**Endpoint**: `POST /api/registration`

Registra uma nova clínica e cria automaticamente o Owner associado.

**Request Body**:
```json
{
  "clinicName": "Clínica Exemplo",
  "clinicCNPJ": "12345678000195",
  "clinicPhone": "(11) 98765-4321",
  "clinicEmail": "contato@clinica.com",
  "street": "Rua Exemplo",
  "number": "123",
  "complement": "Sala 4",
  "neighborhood": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567",
  "ownerName": "João da Silva",
  "ownerCPF": "12345678901",
  "ownerPhone": "(11) 99999-8888",
  "ownerEmail": "joao@clinica.com",
  "username": "joao.silva",
  "password": "Senha@Forte123",
  "planId": "guid-do-plano",
  "acceptTerms": true,
  "useTrial": true
}
```

**Response**:
```json
{
  "success": true,
  "message": "Registration successful! Welcome to MedicWarehouse. You can now login with your credentials.",
  "clinicId": "guid-da-clinica",
  "userId": "guid-do-owner"
}
```

### 2. Listar Todos os Owners (SystemAdmin)
**Endpoint**: `GET /api/owners`

**Autenticação**: Requer role `SystemAdmin`

**Response**:
```json
[
  {
    "id": "guid-do-owner",
    "username": "joao.silva",
    "email": "joao@clinica.com",
    "fullName": "João da Silva",
    "phone": "(11) 99999-8888",
    "clinicId": "guid-da-clinica",
    "isActive": true,
    "lastLoginAt": "2024-01-15T10:30:00Z",
    "professionalId": "CRM 12345",
    "specialty": "Cardiologia"
  }
]
```

### 3. Buscar Owner por ID
**Endpoint**: `GET /api/owners/{id}`

**Response**: Mesmo formato do item da lista acima.

### 4. Buscar Owner por Clínica
**Endpoint**: `GET /api/owners/by-clinic/{clinicId}`

**Response**: Retorna o Owner da clínica especificada.

### 5. Criar Novo Owner (SystemAdmin)
**Endpoint**: `POST /api/owners`

**Autenticação**: Requer role `SystemAdmin`

**Request Body**:
```json
{
  "username": "maria.santos",
  "email": "maria@clinica.com",
  "password": "Senha@Forte123",
  "fullName": "Maria Santos",
  "phone": "(11) 98888-7777",
  "clinicId": "guid-da-clinica",
  "professionalId": "CRM 67890",
  "specialty": "Pediatria"
}
```

**Response**: Retorna o Owner criado.

### 6. Atualizar Perfil do Owner
**Endpoint**: `PUT /api/owners/{id}`

**Request Body**:
```json
{
  "email": "novo.email@clinica.com",
  "fullName": "João da Silva Junior",
  "phone": "(11) 99999-7777",
  "professionalId": "CRM 12345",
  "specialty": "Cardiologia"
}
```

### 7. Ativar Owner
**Endpoint**: `POST /api/owners/{id}/activate`

**Autenticação**: Requer role `SystemAdmin`

### 8. Desativar Owner
**Endpoint**: `POST /api/owners/{id}/deactivate`

**Autenticação**: Requer role `SystemAdmin`

### 9. Login
**Endpoint**: `POST /api/auth/login`

O login funciona tanto para Owners quanto para Users. O sistema tenta autenticar primeiro como User, depois como Owner.

**Request Body**:
```json
{
  "username": "joao.silva",
  "password": "Senha@Forte123",
  "tenantId": "tenant-id"
}
```

**Response**:
```json
{
  "token": "jwt-token",
  "username": "joao.silva",
  "tenantId": "tenant-id",
  "role": "ClinicOwner",
  "userId": "guid-do-owner",
  "clinicId": "guid-da-clinica",
  "expiresAt": "2024-01-15T11:30:00Z"
}
```

## Fluxos de Uso

### Fluxo 1: Registro de Nova Clínica
1. Usuário acessa a tela de registro
2. Preenche os dados da clínica e do owner
3. Sistema cria a clínica
4. Sistema cria o owner associado à clínica
5. Sistema cria a assinatura da clínica
6. Owner pode fazer login e começar a usar o sistema

### Fluxo 2: Owner Gerencia Usuários da Clínica
1. Owner faz login no sistema
2. Acessa a tela de gerenciamento de usuários
3. Pode criar novos usuários (médicos, enfermeiros, recepcionistas, etc.)
4. Pode ativar/desativar usuários
5. Pode alterar roles dos usuários

### Fluxo 3: SystemAdmin Gerencia Owners
1. SystemAdmin faz login no sistema
2. Acessa a tela de gerenciamento de owners
3. Pode visualizar todos os owners do sistema
4. Pode criar novos owners para clínicas
5. Pode ativar/desativar owners
6. Pode atualizar informações dos owners

## Arquitetura

### Camadas do Sistema

```
┌─────────────────────────────────────┐
│     Presentation Layer (API)        │
│  - OwnersController                 │
│  - UsersController                  │
│  - AuthController                   │
│  - RegistrationController           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│     Application Layer (Services)    │
│  - IOwnerService / OwnerService     │
│  - IUserService / UserService       │
│  - IAuthService / AuthService       │
│  - IRegistrationService             │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│       Domain Layer                  │
│  - Owner (Entity)                   │
│  - User (Entity)                    │
│  - IOwnerRepository (Interface)     │
│  - IUserRepository (Interface)      │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   Infrastructure Layer (Repository) │
│  - OwnerRepository                  │
│  - UserRepository                   │
│  - MedicSoftDbContext               │
└─────────────────────────────────────┘
```

### Princípios Implementados

1. **Separação de Responsabilidades**: Cada camada tem uma responsabilidade clara
2. **Dependency Injection**: Todas as dependências são injetadas via constructor
3. **Repository Pattern**: Abstração do acesso a dados
4. **Service Layer**: Lógica de negócio isolada dos controllers
5. **DTO Pattern**: Separação entre entidades de domínio e objetos de transferência
6. **Single Responsibility**: Cada classe/serviço tem uma única responsabilidade

## Segurança

### Autenticação
- JWT tokens são gerados após login bem-sucedido
- Tokens incluem claims de `user_id`, `tenant_id`, `clinic_id` e `role`
- Tokens expiram após 60 minutos (configurável)

### Autorização
- Endpoints protegidos por `[Authorize]` attribute
- Permissões específicas via `[RequirePermission(Permission.X)]` attribute
- Owners têm permissões de `ClinicOwner` role
- SystemAdmin pode gerenciar todos os owners

### Validações
- Senhas devem ter no mínimo 8 caracteres
- Senhas devem conter letras maiúsculas, minúsculas, números e caracteres especiais
- Username deve ser único dentro do tenant
- Email deve ser único dentro do tenant
- CNPJ deve ser único no sistema

## Banco de Dados

### Tabela Owners
```sql
CREATE TABLE Owners (
    Id uniqueidentifier PRIMARY KEY,
    Username nvarchar(100) NOT NULL,
    Email nvarchar(200) NOT NULL,
    PasswordHash nvarchar(500) NOT NULL,
    FullName nvarchar(200) NOT NULL,
    Phone nvarchar(20) NOT NULL,
    ClinicId uniqueidentifier NULL,  -- Nullable to support system owners
    IsActive bit NOT NULL,
    LastLoginAt datetime2 NULL,
    ProfessionalId nvarchar(50) NULL,
    Specialty nvarchar(100) NULL,
    TenantId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NULL,
    
    CONSTRAINT FK_Owners_Clinics FOREIGN KEY (ClinicId) REFERENCES Clinics(Id)
);

CREATE UNIQUE INDEX IX_Owners_Username ON Owners(Username);
CREATE INDEX IX_Owners_Email ON Owners(Email);
CREATE INDEX IX_Owners_ClinicId ON Owners(ClinicId);
CREATE INDEX IX_Owners_TenantId_IsActive ON Owners(TenantId, IsActive);
```

### Migração
A migração `AddOwnerEntity` foi criada e adiciona a tabela `Owners` ao banco de dados.

A migração `MakeOwnerClinicIdNullableForSystemOwners` atualiza a coluna `ClinicId` para permitir valores NULL, possibilitando a criação de proprietários de sistema (system owners).

Para aplicar as migrações:
```bash
dotnet ef database update --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api
```

## Testes

### Testes Unitários (A Implementar)
- Criar testes para `OwnerService`
- Criar testes para `AuthService` com Owner
- Criar testes para `RegistrationService` com Owner
- Criar testes para validações da entidade `Owner`

### Testes de Integração (A Implementar)
- Testar fluxo completo de registro
- Testar autenticação de Owner
- Testar CRUD de Owners
- Testar permissões de Owners

## Próximos Passos

1. **Frontend**:
   - Criar tela de registro com dados do Owner
   - Criar dashboard do Owner
   - Criar tela de gerenciamento de Owners (para SystemAdmin)

2. **Funcionalidades**:
   - Implementar recuperação de senha para Owners
   - Implementar alteração de senha para Owners
   - Implementar notificações por email para Owners

3. **Documentação**:
   - Adicionar exemplos de uso no Postman
   - Criar guia de migração de dados existentes
   - Documentar casos de uso avançados

## Suporte

Para dúvidas ou problemas:
- **Email**: contato@medicwarehouse.com
- **Documentação**: https://docs.medicwarehouse.com
- **Issues**: https://github.com/MedicWarehouse/MW.Code/issues

---

**Última atualização**: 12 de outubro de 2024
**Versão**: 1.0.0
