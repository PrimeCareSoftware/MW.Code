# Guia de Gerenciamento de Sessões

## Visão Geral

Este guia documenta o sistema de gerenciamento de sessões implementado no PrimeCare Software, que garante que **apenas uma sessão ativa por usuário** seja permitida por vez.

## Funcionalidade

### Objetivo
Garantir que apenas um dispositivo/navegador possa estar autenticado com as mesmas credenciais ao mesmo tempo. Quando um usuário faz login em um novo dispositivo, **todas as sessões anteriores são automaticamente invalidadas**.

### Características
- **Sessão única por usuário**: Apenas uma sessão ativa por credencial
- **Invalidação automática**: Ao fazer login, todas as sessões anteriores são encerradas
- **Validação periódica**: O frontend valida a sessão a cada 30 segundos
- **Notificação ao usuário**: Mensagem clara quando a sessão é invalidada
- **Logout automático**: Redirecionamento automático para tela de login

## Arquitetura

### Backend

#### Entidades
- `User` e `Owner` agora possuem a propriedade `CurrentSessionId`
- Cada login gera um novo UUID como identificador de sessão
- O `CurrentSessionId` é armazenado no banco de dados

#### Endpoint de Validação
```
POST /api/auth/validate-session
Content-Type: application/json

{
  "token": "jwt-token-aqui"
}

Response:
{
  "isValid": true/false,
  "message": "Mensagem descritiva"
}
```

#### Fluxo de Login
1. Usuário fornece credenciais
2. Sistema valida credenciais
3. **Sistema invalida TODAS as sessões anteriores do usuário**
4. Gera novo `SessionId` (UUID)
5. Armazena novo `SessionId` na tabela UserSessions/OwnerSessions
6. Atualiza campo `CurrentSessionId` na entidade User/Owner (compatibilidade)
7. Inclui `SessionId` no token JWT
8. Retorna token ao cliente

#### Fluxo de Validação
1. Cliente envia token JWT
2. Sistema valida token JWT
3. Extrai `SessionId` do token
4. Compara com `CurrentSessionId` no banco de dados
5. Retorna se a sessão é válida

### Frontend

#### Validação Periódica
- Intervalo: 30 segundos
- Inicia automaticamente após login bem-sucedido
- Para ao fazer logout

#### Tratamento de Sessão Inválida
1. Sessão detectada como inválida
2. Para validação periódica
3. Limpa tokens do localStorage
4. Redireciona para página de login
5. Exibe mensagem: "Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador."

## Implementação

### Migrations
Uma migration foi criada para adicionar a coluna `CurrentSessionId`:
```
dotnet ef migrations add AddSessionManagement --context MedicSoftDbContext --output-dir Migrations/PostgreSQL
```

### JWT Claims
O token JWT agora inclui:
- `session_id`: Identificador único da sessão

### Configuração
Não são necessárias configurações adicionais. O sistema funciona automaticamente após a aplicação das migrations.

## Segurança

### Considerações
- O `SessionId` é um UUID v4, garantindo aleatoriedade e unicidade
- A validação é feita no servidor, não podendo ser burlada no cliente
- Em caso de erro de rede, o sistema não faz logout automático (apenas em caso de sessão explicitamente inválida)
- O token JWT continua com sua expiração normal (60 minutos)

### Possíveis Melhorias Futuras
1. Permitir múltiplas sessões simultâneas com limite configurável
2. Dashboard para visualizar sessões ativas
3. Capacidade de revogar sessões individuais
4. Notificação por email quando nova sessão é iniciada
5. Histórico de logins e dispositivos

## Testes

### Testes Unitários
- `RecordLogin_UpdatesLastLoginAt`: Verifica atualização de login com sessionId
- `RecordLogin_ThrowsException_WhenSessionIdIsEmpty`: Valida que sessionId não pode ser vazio
- `IsSessionValid_ReturnsTrueForMatchingSession`: Verifica validação de sessão correta
- `IsSessionValid_ReturnsFalseForDifferentSession`: Verifica rejeição de sessão incorreta
- `RecordLogin_InvalidatesPreviousSession`: Verifica que novo login invalida sessão anterior

### Teste Manual
1. Faça login em um navegador
2. Copie o token JWT
3. Faça login novamente com as mesmas credenciais em outro navegador
4. **O primeiro navegador é desconectado imediatamente** (não precisa esperar ~30 segundos)
5. Verifique a mensagem exibida na tela de login no primeiro navegador: "Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador."

## Troubleshooting

### Problema: Usuário é desconectado constantemente
**Solução**: Verifique se há múltiplos tabs/janelas abertas. Cada tab faz validação independente, mas compartilham o mesmo sessionId do localStorage.

### Problema: Validação não está funcionando
**Solução**: 
1. Verifique se a migration foi aplicada
2. Confirme que o endpoint `/api/auth/validate-session` está respondendo
3. Verifique logs do navegador (Console) e servidor

### Problema: Mensagem não aparece na tela de login
**Solução**: A mensagem é passada via router state. Certifique-se de que o componente de login está capturando corretamente o state da navegação.

## Suporte

Para questões ou problemas relacionados ao gerenciamento de sessões, entre em contato com a equipe de desenvolvimento ou abra uma issue no repositório.
