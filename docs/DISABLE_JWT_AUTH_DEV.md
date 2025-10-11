# Desabilitando Autenticação JWT no Desenvolvimento

## Visão Geral

Este sistema agora suporta desabilitar a autenticação JWT em ambientes de desenvolvimento para facilitar testes e desenvolvimento local. Quando desabilitada, todas as requisições serão automaticamente autenticadas com um usuário de desenvolvimento simulado.

## Como Usar

### Desenvolvimento

Para desabilitar a autenticação JWT no ambiente de desenvolvimento, configure o flag no arquivo `appsettings.Development.json`:

```json
{
  "Authentication": {
    "DisableAuthentication": true
  }
}
```

Quando o aplicativo iniciar com esta configuração, você verá a seguinte mensagem no console:

```
⚠️  WARNING: Authentication is DISABLED for development purposes
```

### Produção

A autenticação **sempre deve estar habilitada** em produção. O arquivo `appsettings.Production.json` já está configurado corretamente:

```json
{
  "Authentication": {
    "DisableAuthentication": false
  }
}
```

## O Que Acontece Quando a Autenticação É Desabilitada

Quando `DisableAuthentication` está definido como `true`:

1. **Autenticação Automática**: Todas as requisições são automaticamente autenticadas com um usuário simulado
2. **Claims Padrão**: O sistema cria claims padrão para desenvolvimento:
   - `Name`: "dev-user"
   - `tenant_id`: "default-tenant"
   - `user_id`: "dev-user-id"
3. **Swagger Simplificado**: A interface do Swagger não exige tokens JWT e mostra um aviso de que a autenticação está desabilitada
4. **Sem JWT Necessário**: Todas as rotas protegidas com `[Authorize]` são acessíveis sem token

## Configurações por Ambiente

### appsettings.json (Base)
```json
{
  "Authentication": {
    "DisableAuthentication": false
  }
}
```

### appsettings.Development.json
```json
{
  "Authentication": {
    "DisableAuthentication": true
  }
}
```

### appsettings.Production.json
```json
{
  "Authentication": {
    "DisableAuthentication": false
  }
}
```

## Segurança

⚠️ **AVISO IMPORTANTE**: 

- **NUNCA** desabilite a autenticação em ambiente de produção
- Esta funcionalidade é **APENAS** para desenvolvimento e testes locais
- O sistema mostrá um aviso claro no console quando a autenticação estiver desabilitada
- O Swagger também indicará claramente quando a autenticação estiver desabilitada

## Testando

### Com Autenticação Desabilitada

```bash
# Requisição direta sem token
curl http://localhost:5000/api/patients
```

### Com Autenticação Habilitada

```bash
# 1. Obter token JWT
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"SecureP@ssw0rd!","tenantId":"default-tenant"}'

# 2. Usar o token nas requisições
curl http://localhost:5000/api/patients \
  -H "Authorization: Bearer {seu-token-aqui}"
```

## Implementação Técnica

A funcionalidade utiliza um `DevBypassAuthenticationHandler` customizado localizado em:
`src/MedicSoft.Api/Authentication/DevBypassAuthenticationHandler.cs`

Este handler:
- Intercepta todas as requisições de autenticação
- Cria automaticamente um `ClaimsPrincipal` com claims de desenvolvimento
- Retorna sucesso para todas as tentativas de autenticação
- É registrado apenas quando `DisableAuthentication` é `true`

## Benefícios

1. **Desenvolvimento Mais Rápido**: Não precisa obter tokens JWT repetidamente
2. **Testes Simplificados**: Facilita testes de integração e E2E
3. **Swagger Mais Fácil**: Interface Swagger mais simples durante desenvolvimento
4. **Seguro por Padrão**: Autenticação habilitada por padrão em todos os ambientes
5. **Configurável**: Pode ser ativado/desativado através de configuração

## Troubleshooting

### Autenticação não está desabilitando

1. Verifique se está executando com o perfil correto:
   ```bash
   dotnet run --project src/MedicSoft.Api --launch-profile Development
   ```

2. Confirme a configuração no `appsettings.Development.json`

3. Verifique o console para a mensagem de aviso

### Ainda recebendo 401 Unauthorized

1. Verifique se a aplicação reiniciou após alterar a configuração
2. Confirme que o ambiente está correto (Development)
3. Verifique os logs do console para erros
