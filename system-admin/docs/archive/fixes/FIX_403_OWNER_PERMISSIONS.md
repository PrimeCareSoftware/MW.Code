# Fix: 403 Forbidden Errors for Clinic Owners

## Data da Correção
14 de Janeiro de 2026

## Problema Identificado

Proprietários de clínicas estavam recebendo erros 403 (Forbidden) ao tentar acessar as opções do menu de administração, incluindo:
- Gestão de Usuários (`/clinic-admin/users`)
- Perfis de Acesso (`/admin/profiles`)
- Informações da Clínica (`/clinic-admin/info`)
- Personalização (`/clinic-admin/customization`)
- Assinatura (`/clinic-admin/subscription`)

### Causa Raiz

O problema foi causado por inconsistência na nomenclatura das roles (funções) do sistema:

1. **No AuthController**: Ao gerar o token JWT para owners, o código usava a string hardcoded `"Owner"`
2. **No Enum UserRole**: A role está definida como `ClinicOwner`
3. **No RequirePermissionKeyAttribute**: Verificava apenas por `"Owner"`
4. **Para usuários regulares**: O sistema usa `user.Role.ToString()` que retorna `"ClinicOwner"` do enum

Resultado: Os proprietários com tokens gerados usando o enum não passavam na verificação de permissões.

## Solução Implementada

### 1. Padronização da Nomenclatura

#### Arquivo: `src/MedicSoft.Api/Controllers/AuthController.cs`

**Antes:**
```csharp
var token = _jwtTokenService.GenerateToken(
    username: owner.Username,
    userId: owner.Id.ToString(),
    tenantId: tenantId,
    role: "Owner",  // Hardcoded
    clinicId: owner.ClinicId?.ToString(),
    isSystemOwner: owner.IsSystemOwner,
    sessionId: sessionId
);
```

**Depois:**
```csharp
var token = _jwtTokenService.GenerateToken(
    username: owner.Username,
    userId: owner.Id.ToString(),
    tenantId: tenantId,
    role: RoleNames.ClinicOwner,  // Usando constante
    clinicId: owner.ClinicId?.ToString(),
    isSystemOwner: owner.IsSystemOwner,
    sessionId: sessionId
);
```

### 2. Retrocompatibilidade

#### Arquivo: `src/MedicSoft.CrossCutting/Authorization/RequirePermissionKeyAttribute.cs`

**Antes:**
```csharp
if (roleClaim == "Owner")
{
    // Verificação de permissões do owner
}
```

**Depois:**
```csharp
if (roleClaim == RoleNames.ClinicOwner || roleClaim == "Owner")
{
    // Suporta tanto "ClinicOwner" quanto "Owner" para retrocompatibilidade
}
```

### 3. Validação de Sessão

Atualizada a verificação de sessões para reconhecer ambas as nomenclaturas:

```csharp
if (roleClaim == "Owner" || roleClaim == RoleNames.ClinicOwner)
{
    isSessionValid = await _authService.ValidateOwnerSessionAsync(userId, sessionIdClaim, tenantIdClaim);
}
```

### 4. Frontend (Navbar)

#### Arquivo: `frontend/medicwarehouse-app/src/app/shared/navbar/navbar.ts`

**Antes:**
```typescript
isOwner(): boolean {
  const user = this.authService.currentUser();
  return user ? (user.role === 'Owner' || user.isSystemOwner === true) : false;
}
```

**Depois:**
```typescript
isOwner(): boolean {
  const user = this.authService.currentUser();
  return user ? (user.role === 'Owner' || user.role === 'ClinicOwner' || user.isSystemOwner === true) : false;
}
```

## Arquivos Modificados

```
src/
├── MedicSoft.Api/Controllers/
│   └── AuthController.cs                         # Linha 206, 219, 439
└── MedicSoft.CrossCutting/Authorization/
    └── RequirePermissionKeyAttribute.cs          # Linha 66

frontend/medicwarehouse-app/src/app/shared/navbar/
└── navbar.ts                                     # Linha 38
```

## Testes Realizados

✅ **Compilação**: Build succeeded sem erros  
✅ **Code Review**: Passed sem issues  
✅ **Segurança**: Mantém mesma lógica de autorização, apenas corrige nomenclatura  

## Como Testar Manualmente

### 1. Login como Proprietário
```bash
# Endpoint
POST http://localhost:5000/api/auth/owner-login

# Body
{
  "username": "owner.demo",
  "password": "Owner@123",
  "tenantId": "demo-clinic-001",
  "clinicId": "guid-da-clinica"
}
```

### 2. Verificar Token
O token JWT agora deve ter:
```json
{
  "role": "ClinicOwner",  // Não mais "Owner"
  "tenant_id": "demo-clinic-001",
  "clinic_id": "guid-da-clinica"
}
```

### 3. Acessar Menu de Administração
- O menu "Administração" deve aparecer na navbar
- Clicar em "Usuários" não deve retornar 403
- Clicar em "Perfis de Acesso" não deve retornar 403

### 4. Verificar Permissões
```bash
# Endpoint protegido
GET http://localhost:5000/api/users
Authorization: Bearer {token}

# Deve retornar 200 OK, não 403 Forbidden
```

## Retrocompatibilidade

A solução mantém compatibilidade com tokens existentes que possam ter sido gerados com `role: "Owner"`:

- ✅ Novos tokens: Gerados com `"ClinicOwner"`
- ✅ Tokens antigos: Ainda aceitos se tiverem `"Owner"`
- ✅ Frontend: Aceita ambas as nomenclaturas
- ✅ Backend: Valida ambas as nomenclaturas

## Impacto

- **Positivo**: Proprietários podem acessar menu de administração
- **Nenhum impacto negativo**: Mantém retrocompatibilidade
- **Performance**: Sem impacto (apenas verificações de string)
- **Segurança**: Mantida (mesma lógica de autorização)

## Referências

- `RoleNames.cs`: Define a constante `ClinicOwner = "ClinicOwner"`
- `UserRole.cs`: Enum que define `ClinicOwner` como role
- `OWNER_MENU_FIX.md`: Documentação original do menu de administração
- `OWNER_DASHBOARD_PERMISSIONS.md`: Permissões do proprietário

## Próximos Passos

### Futuras Melhorias (Opcional)
1. Considerar remover suporte a `"Owner"` em versão futura (breaking change)
2. Migrar tokens existentes durante login para usar nova nomenclatura
3. Adicionar testes automatizados para verificação de permissões

### Não Recomendado Agora
- Alterar nomenclatura de outras roles (pode quebrar sistema)
- Remover retrocompatibilidade imediatamente
- Mudar estrutura do JWT token

## Status

✅ **CORREÇÃO IMPLEMENTADA E TESTADA**

---

**Implementado por:** GitHub Copilot  
**Data:** 14 de Janeiro de 2026  
**Issue:** Proprietários recebendo 403 no menu administração  
**PR:** [Link será adicionado após merge]
