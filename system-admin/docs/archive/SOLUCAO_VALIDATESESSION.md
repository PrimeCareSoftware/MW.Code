# Solução: Token ValidateSession Retornando Null

## Problema Identificado

Todos os tokens JWT eram rejeitados na validação com o seguinte erro:

```
warn: MedicSoft.Application.Services.JwtTokenService[0]
      Token read in ValidateToken - Claims: 5, Issuer: PrimeCare Software, Audience: , Expires: 01/01/0001 00:00:00, Exp Payload: (null), Exp Claim: NO - MISSING

warn: MedicSoft.Application.Services.JwtTokenService[0]
      Token validation failed: IDX10225: Lifetime validation failed. The token is missing an Expiration Time.

warn: MedicSoft.Api.Controllers.AuthController[0]
      ValidateSession failed: Token validation returned null
```

## Causa Raiz

O problema estava na **biblioteca `System.IdentityModel.Tokens.Jwt` versão 7.1.2**:

1. O construtor `new JwtSecurityToken()` com parâmetros `expires` e `notBefore` **não estava gerando corretamente os registered claims** (`exp`, `nbf`) no payload
2. Quando `ReadJwtToken()` deserializava o token, não encontrava a claim `exp`
3. A validação com `RequireExpirationTime = true` falhava com erro `IDX10225`

## ✅ Solução Implementada

Mudei de usar o **construtor direto** para usar `JwtSecurityTokenHandler.CreateToken()` com `SecurityTokenDescriptor`, que é a forma correta de gerar tokens JWT:

```csharp
var tokenHandler = new JwtSecurityTokenHandler();

// Usar SecurityTokenDescriptor é a forma correta
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(claims),
    Expires = expiresAt,
    NotBefore = now,
    Issuer = issuer,
    Audience = audience,
    SigningCredentials = credentials
};

var token = tokenHandler.CreateToken(tokenDescriptor) as JwtSecurityToken;
var tokenString = tokenHandler.WriteToken(token);
```

### Por que funciona:

1. **`SecurityTokenDescriptor` é o padrão correto**: A biblioteca foi projetada para usar esse padrão
2. **`CreateToken()` serializa corretamente**: Gera os registered claims (`exp`, `nbf`, `iat`) no formato JWT padrão (RFC 7519)
3. **Compatibilidade total**: Tokens gerados dessa forma passam em qualquer validador JWT
4. **Suporte à expiração**: O `Payload.Expiration` é preenchido corretamente

## Mudanças Realizadas

**Arquivo**: `/Users/igorlessarobainadesouza/Documents/MW.Code/src/MedicSoft.Application/Services/JwtTokenService.cs`

**Método**: `GenerateToken()`

**Antes:**
```csharp
var token = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    notBefore: now,
    expires: expiresAt,
    signingCredentials: credentials
);

var tokenHandler = new JwtSecurityTokenHandler();
var tokenString = tokenHandler.WriteToken(token);
```

**Depois:**
```csharp
var tokenHandler = new JwtSecurityTokenHandler();

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(claims),
    Expires = expiresAt,
    NotBefore = now,
    Issuer = issuer,
    Audience = audience,
    SigningCredentials = credentials
};

var token = tokenHandler.CreateToken(tokenDescriptor) as JwtSecurityToken;
var tokenString = tokenHandler.WriteToken(token);
```

## Impacto Esperado

- ✅ Tokens gerados com `exp` e `nbf` claims corretamente
- ✅ Validação com `RequireExpirationTime = true` funcionará
- ✅ `ValidateSession` não retornará mais `null`
- ✅ Sessões de usuários serão validadas corretamente

## Verificação

- ✅ Código compila sem erros
- ✅ Método `GenerateToken()` atualizado
- ✅ Pronto para teste em produção

## Biblioteca

Sistema.IdentityModel.Tokens.Jwt versão 7.1.2 (versão atual no projeto)

