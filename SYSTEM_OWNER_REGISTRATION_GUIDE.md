# Guia de Registro do System Owner (Administrador do Sistema)

## Visão Geral

Este documento descreve como criar a conta de administrador do sistema (System Owner) quando o MedicWarehouse for implantado em produção pela primeira vez.

## ⚠️ IMPORTANTE - Segurança

O endpoint de registro do System Owner foi projetado para ser usado **APENAS UMA VEZ** durante a configuração inicial do sistema em produção. Após criar o administrador, você deve:

1. Remover ou rotacionar o token de registro
2. Nunca compartilhar o token de registro
3. Guardar o token em local seguro (gerenciador de senhas)

## Pré-requisitos

1. Sistema implantado em produção
2. Acesso às variáveis de ambiente do servidor
3. Token de registro gerado (veja abaixo)

## Passo 1: Gerar Token de Registro

Use o comando abaixo para gerar um token seguro de 32+ caracteres:

```bash
openssl rand -base64 32
```

Exemplo de saída:
```
Xp8kL3mN9qR4sT5uV6wX7yZ8aB1cD2eF3gH4iJ5kL6mN==
```

## Passo 2: Configurar Variável de Ambiente

No servidor de produção, adicione a variável de ambiente:

```bash
export SYSTEM_OWNER_REGISTRATION_TOKEN="Xp8kL3mN9qR4sT5uV6wX7yZ8aB1cD2eF3gH4iJ5kL6mN=="
```

Ou adicione no arquivo `.env`:

```env
SYSTEM_OWNER_REGISTRATION_TOKEN=Xp8kL3mN9qR4sT5uV6wX7yZ8aB1cD2eF3gH4iJ5kL6mN==
```

## Passo 3: Fazer Requisição de Registro

Envie uma requisição POST para o endpoint de registro:

### Usando cURL

```bash
curl -X POST https://api.seu-dominio.com/api/auth/register-system-owner \
  -H "Content-Type: application/json" \
  -d '{
    "registrationToken": "Xp8kL3mN9qR4sT5uV6wX7yZ8aB1cD2eF3gH4iJ5kL6mN==",
    "username": "admin",
    "password": "SuaSenhaForte@123",
    "email": "admin@sua-empresa.com",
    "fullName": "Administrador do Sistema",
    "phone": "+55 11 98765-4321",
    "tenantId": "medicwarehouse"
  }'
```

### Usando JavaScript/Fetch

```javascript
const response = await fetch('https://api.seu-dominio.com/api/auth/register-system-owner', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    registrationToken: 'Xp8kL3mN9qR4sT5uV6wX7yZ8aB1cD2eF3gH4iJ5kL6mN==',
    username: 'admin',
    password: 'SuaSenhaForte@123',
    email: 'admin@sua-empresa.com',
    fullName: 'Administrador do Sistema',
    phone: '+55 11 98765-4321',
    tenantId: 'medicwarehouse'
  })
});

const result = await response.json();
console.log(result);
```

### Resposta de Sucesso

```json
{
  "message": "Administrador do sistema criado com sucesso. Por favor, faça login.",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "admin"
}
```

## Passo 4: Fazer Login

Após criar o administrador, faça login usando o endpoint de owner login:

```bash
curl -X POST https://api.seu-dominio.com/api/auth/owner-login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "SuaSenhaForte@123",
    "tenantId": "medicwarehouse"
  }'
```

Você receberá um token JWT com role `SystemAdmin`:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "tenantId": "medicwarehouse",
  "role": "SystemAdmin",
  "isSystemOwner": true,
  "expiresAt": "2026-02-04T18:00:00Z"
}
```

## Passo 5: Remover Token de Registro (CRÍTICO)

**IMPORTANTE:** Após criar o administrador com sucesso, você DEVE remover ou rotacionar o token de registro:

### Opção 1: Remover a variável de ambiente

```bash
unset SYSTEM_OWNER_REGISTRATION_TOKEN
```

Ou remova do arquivo `.env`:

```env
# SYSTEM_OWNER_REGISTRATION_TOKEN=  # REMOVIDO - já usado
```

### Opção 2: Rotacionar o token (recomendado)

Gere um novo token e substitua:

```bash
openssl rand -base64 32
# Atualize a variável de ambiente com o novo token
```

## Mensagens de Erro Comuns

### Token Inválido

```json
{
  "message": "Token de registro inválido."
}
```

**Solução:** Verifique se o token no corpo da requisição corresponde exatamente ao configurado na variável de ambiente.

### System Owner Já Existe

```json
{
  "message": "Um administrador do sistema já existe. Use o endpoint de gerenciamento de owners para criar owners adicionais."
}
```

**Solução:** Já existe um System Owner cadastrado. Use o endpoint regular `/api/owners` (autenticado como SystemAdmin) para criar owners de clínicas.

### Token Não Configurado

```json
{
  "message": "System owner registration is not configured. Contact support."
}
```

**Solução:** A variável de ambiente `SYSTEM_OWNER_REGISTRATION_TOKEN` não está configurada no servidor.

## Criar Owners de Clínicas (Após Login)

Após fazer login como System Owner, você pode criar owners de clínicas usando:

```bash
curl -X POST https://api.seu-dominio.com/api/owners \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_JWT" \
  -d '{
    "username": "clinica1_owner",
    "password": "SenhaClinica@123",
    "email": "contato@clinica1.com",
    "fullName": "Dr. João Silva",
    "phone": "+55 11 91234-5678",
    "clinicId": "uuid-da-clinica-aqui"
  }'
```

**Nota:** Owners de clínicas TÊM um `clinicId` definido. Apenas System Owners (você) têm `clinicId = null`.

## Segurança

### Diferenças entre System Owner e Clinic Owner

| Característica | System Owner | Clinic Owner |
|----------------|--------------|--------------|
| ClinicId | `null` | UUID válido |
| Role JWT | `SystemAdmin` | `ClinicOwner` |
| Acesso | Todo o sistema | Apenas sua clínica |
| Criar Owners | ✅ Sim | ❌ Não |
| Ver todas clínicas | ✅ Sim | ❌ Não |

### Proteções Implementadas

1. **Token de Registro Obrigatório:** O endpoint requer um token secreto configurado no servidor
2. **Apenas Uma Vez:** O endpoint verifica se já existe um System Owner e bloqueia novas criações
3. **Autorização Dupla:** Endpoints de gerenciamento usam `[Authorize(Roles = "SystemAdmin")]` + `[RequireSystemOwner]`
4. **Audit Logging:** Todas as operações de criação/modificação de owners são registradas
5. **JWT Claims:** O token inclui claim `is_system_owner = true` que é verificado em cada requisição

## Suporte

Se você encontrar problemas durante o registro, verifique:

1. Logs do servidor para erros detalhados
2. Variável de ambiente configurada corretamente
3. Formato correto do JSON na requisição
4. Conexão com o banco de dados funcionando

Para mais informações, consulte a documentação técnica em `AUTHENTICATION_ARCHITECTURE.txt`.
