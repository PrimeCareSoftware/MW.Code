# Guia de Desenvolvimento - Autenticação e Usuários Iniciais

## 📋 Problema Resolvido

Este documento descreve como resolver o problema de "chicken and egg" durante o desenvolvimento: você precisa de autenticação para acessar os endpoints, mas não consegue criar o primeiro usuário sem autenticação.

## 🎯 Solução para MVP/Desenvolvimento

Para facilitar o desenvolvimento e testes, foram implementados **endpoints especiais** que funcionam APENAS em ambiente de desenvolvimento e permitem criar usuários iniciais sem necessidade de autenticação.

## ⚠️ IMPORTANTE

**Estes endpoints são APENAS para desenvolvimento/MVP e devem ser desabilitados em produção!**

Os endpoints estão protegidos e só funcionam quando:
- O ambiente é `Development` OU
- A configuração `Development:EnableDevEndpoints` está definida como `true`

## 🚀 Guia Rápido de Uso

### Passo 1: Verificar se os Endpoints Estão Disponíveis

```bash
GET http://localhost:5000/api/dev/info
```

**Resposta esperada:**
```json
{
  "environment": "Development",
  "isDevelopment": true,
  "devEndpointsEnabled": true,
  "availableEndpoints": [
    "POST /api/dev/create-system-owner - Create a system owner without authentication",
    "GET /api/dev/info - Get development environment information",
    "POST /api/data-seeder/seed-system-owner - Create default system owner (admin/Admin@123)",
    "POST /api/registration - Create a clinic with owner (use this for clinic registration)"
  ],
  "note": {
    "message": "These endpoints are for DEVELOPMENT/MVP only and should be disabled in production",
    "recommendation": "For creating clinics and users, use the standard /api/registration endpoint or /api/data-seeder/seed-demo for test data"
  }
}
```

### Passo 2: Criar um System Owner (Administrador do Sistema)

#### Opção A: Usando o Endpoint de Seed (Rápido)

Cria um system owner padrão com credenciais pré-definidas:

```bash
POST http://localhost:5000/api/data-seeder/seed-system-owner
```

**Resposta:**
```json
{
  "message": "System owner created successfully",
  "owner": {
    "username": "admin",
    "email": "admin@medicwarehouse.com",
    "password": "Admin@123",
    "isSystemOwner": true,
    "tenantId": "system"
  },
  "loginInfo": {
    "endpoint": "POST /api/auth/owner-login",
    "body": {
      "username": "admin",
      "password": "Admin@123",
      "tenantId": "system"
    }
  },
  "note": "Use these credentials to login and manage the system. Change the password after first login!"
}
```

**Credenciais Criadas:**
- **Username:** `admin`
- **Password:** `Admin@123`
- **TenantId:** `system`

#### Opção B: Criando um System Owner Personalizado

Se você quiser criar um system owner com suas próprias credenciais:

```bash
POST http://localhost:5000/api/dev/create-system-owner
Content-Type: application/json

{
  "username": "myadmin",
  "password": "MySecurePassword@123",
  "email": "myadmin@example.com",
  "fullName": "My Administrator",
  "phone": "+5511987654321"
}
```

**Resposta:**
```json
{
  "message": "System owner created successfully",
  "owner": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "username": "myadmin",
    "email": "myadmin@example.com",
    "fullName": "My Administrator",
    "isSystemOwner": true,
    "tenantId": "system"
  },
  "loginInstructions": {
    "endpoint": "/api/auth/owner-login",
    "method": "POST",
    "body": {
      "username": "myadmin",
      "password": "<your-password>",
      "tenantId": "system"
    }
  }
}
```

### Passo 3: Fazer Login com o System Owner

```bash
POST http://localhost:5000/api/auth/owner-login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "system"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "tenantId": "system",
  "role": "Owner",
  "clinicId": null,
  "isSystemOwner": true,
  "expiresAt": "2025-10-19T01:38:25Z"
}
```

### Passo 4: Usar o Token nas Requisições

Agora você pode usar o token obtido para acessar todos os endpoints protegidos:

```bash
GET http://localhost:5000/api/system-admin/clinics
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🏥 Criando Clínicas e Usuários

### Opção 1: Criar uma Clínica Completa com Owner

Use o endpoint de registro que já existe (não requer autenticação):

```bash
POST http://localhost:5000/api/registration
Content-Type: application/json

{
  "clinicName": "Minha Clínica",
  "cnpj": "12.345.678/0001-95",
  "tradeName": "Clínica Exemplo",
  "phone": "+5511999999999",
  "email": "contato@minhaclinca.com",
  "address": {
    "street": "Rua Principal",
    "number": "100",
    "complement": "Sala 201",
    "neighborhood": "Centro",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01000-000",
    "country": "Brasil"
  },
  "ownerUsername": "owner",
  "ownerEmail": "owner@minhaclinca.com",
  "ownerPassword": "Owner@123",
  "ownerFullName": "João da Silva",
  "ownerPhone": "+5511987654321"
}
```

### Opção 2: Criar Dados de Demonstração Completos

Para testar o sistema com dados completos (pacientes, agendamentos, etc.):

```bash
POST http://localhost:5000/api/data-seeder/seed-demo
```

Isso criará:
- ✅ 1 Clínica Demo (`demo-clinic-001`)
- ✅ 3 Usuários (Admin, Médico, Recepcionista)
- ✅ 6 Pacientes
- ✅ 8 Procedimentos
- ✅ 5 Agendamentos
- ✅ 2 Pagamentos
- ✅ E muito mais...

**Credenciais criadas:**
- `admin` / `Admin@123` (SystemAdmin)
- `dr.silva` / `Doctor@123` (Doctor)
- `recep.maria` / `Recep@123` (Receptionist)

## 📖 Fluxo Completo de Desenvolvimento

### Cenário 1: Apenas Testar a API

```bash
# 1. Criar system owner padrão
POST http://localhost:5000/api/data-seeder/seed-system-owner

# 2. Fazer login
POST http://localhost:5000/api/auth/owner-login
{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "system"
}

# 3. Usar o token para acessar endpoints protegidos
GET http://localhost:5000/api/system-admin/clinics
Authorization: Bearer <seu-token>
```

### Cenário 2: Testar com Dados Completos

```bash
# 1. Criar system owner
POST http://localhost:5000/api/data-seeder/seed-system-owner

# 2. Criar dados de demonstração
POST http://localhost:5000/api/data-seeder/seed-demo

# 3. Fazer login como médico
POST http://localhost:5000/api/auth/login
{
  "username": "dr.silva",
  "password": "Doctor@123",
  "tenantId": "demo-clinic-001"
}

# 4. Acessar pacientes, agendamentos, etc.
GET http://localhost:5000/api/patients
Authorization: Bearer <seu-token>
```

### Cenário 3: Criar sua Própria Clínica

```bash
# 1. Criar system owner (opcional, se quiser gerenciar clínicas)
POST http://localhost:5000/api/data-seeder/seed-system-owner

# 2. Criar clínica com owner
POST http://localhost:5000/api/registration
{
  "clinicName": "Minha Clínica",
  "cnpj": "12.345.678/0001-95",
  // ... demais campos
}

# 3. Fazer login como owner da clínica
POST http://localhost:5000/api/auth/owner-login
{
  "username": "owner",
  "password": "Owner@123",
  "tenantId": "<tenantId-retornado-no-passo-2>"
}

# 4. Criar usuários, cadastrar pacientes, etc.
```

## ⚙️ Configuração

### appsettings.Development.json

```json
{
  "Development": {
    "EnableDevEndpoints": true
  }
}
```

### Para Desabilitar em Produção

Em `appsettings.Production.json`, não inclua a configuração ou defina como `false`:

```json
{
  "Development": {
    "EnableDevEndpoints": false
  }
}
```

## 🔒 Segurança

### Em Desenvolvimento

- Os endpoints de desenvolvimento estão habilitados
- Facilita testes e desenvolvimento
- Permite criar usuários iniciais rapidamente

### Em Produção

- **SEMPRE desabilite os endpoints de desenvolvimento**
- Use apenas os fluxos de registro normais (`/api/registration`)
- O primeiro system owner deve ser criado manualmente no banco de dados ou via migration

### Verificação de Segurança

Os endpoints verificam automaticamente:
1. Se o ambiente é `Development`
2. Se a configuração `Development:EnableDevEndpoints` está habilitada
3. Retorna erro `403 Forbidden` se nenhuma das condições for atendida

## 📝 Endpoints Disponíveis

### Endpoints de Desenvolvimento

| Endpoint | Método | Descrição | Auth Required |
|----------|--------|-----------|---------------|
| `/api/dev/info` | GET | Info sobre endpoints de desenvolvimento | ❌ No |
| `/api/dev/create-system-owner` | POST | Criar system owner personalizado | ❌ No |
| `/api/data-seeder/seed-system-owner` | POST | Criar system owner padrão | ❌ No |
| `/api/data-seeder/seed-demo` | POST | Criar dados de demonstração completos | ❌ No |

### Endpoints Normais (Sempre Disponíveis)

| Endpoint | Método | Descrição | Auth Required |
|----------|--------|-----------|---------------|
| `/api/registration` | POST | Registrar nova clínica com owner | ❌ No |
| `/api/auth/login` | POST | Login de usuário regular | ❌ No |
| `/api/auth/owner-login` | POST | Login de owner | ❌ No |
| `/api/auth/validate` | POST | Validar token JWT | ❌ No |

## 🎯 Casos de Uso

### Caso 1: Desenvolvedor Frontend Precisa Testar

```bash
# Solução rápida: usar dados de demo
POST /api/data-seeder/seed-demo

# Login como médico
POST /api/auth/login
{
  "username": "dr.silva",
  "password": "Doctor@123",
  "tenantId": "demo-clinic-001"
}
```

### Caso 2: Testar Área de System Owner

```bash
# Criar system owner
POST /api/data-seeder/seed-system-owner

# Login como system owner
POST /api/auth/owner-login
{
  "username": "admin",
  "password": "Admin@123",
  "tenantId": "system"
}

# Acessar endpoints de administração do sistema
GET /api/system-admin/clinics
Authorization: Bearer <token>
```

### Caso 3: Resetar Dados de Teste

Para resetar e começar do zero:

1. Limpe o banco de dados (ou use outro banco de teste)
2. Execute novamente os endpoints de seed

## 📚 Documentação Relacionada

- [AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md) - Guia completo de autenticação
- [CARGA_INICIAL_TESTES.md](CARGA_INICIAL_TESTES.md) - Detalhes sobre dados de teste
- [SYSTEM_OWNER_ACCESS.md](SYSTEM_OWNER_ACCESS.md) - Acesso de system owners
- [README.md](../README.md) - Documentação geral do projeto

## ❓ Problemas Comuns

### "This endpoint is only available in Development environment"

**Solução:** Certifique-se de que:
1. O ambiente está configurado como `Development`
2. Ou a configuração `Development:EnableDevEndpoints` está como `true` em appsettings

### "System owner already exists"

**Solução:** Um system owner com esse username já foi criado. Você pode:
1. Usar as credenciais existentes para fazer login
2. Criar um system owner com outro username
3. Limpar o banco de dados

### "Cannot create clinic: CNPJ already exists"

**Solução:** Use outro CNPJ ou limpe os dados de teste.

## 🎉 Resumo

Com essas ferramentas, você pode:

✅ Criar system owners sem autenticação (apenas em dev)
✅ Criar dados de teste completos com um comando
✅ Testar todos os fluxos do sistema
✅ Não ter o problema de "preciso de autenticação para criar usuários"
✅ Desenvolver e testar rapidamente durante o MVP

**Lembre-se:** Estes endpoints são para desenvolvimento. Em produção, use os fluxos normais de registro e autenticação!
