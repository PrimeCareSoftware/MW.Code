# Plano de Desenvolvimento: Autenticação de Dois Fatores (2FA)

## Status da Implementação (Janeiro 2026)

### ✅ Fase 1: 2FA por Email - Portal do Paciente (CONCLUÍDA)

**Data de Conclusão**: 29/01/2026

#### Componentes Implementados:
1. **Entidade TwoFactorToken** - Completa com campos necessários e índices otimizados
2. **TwoFactorTokenRepository** - Todos os métodos implementados
3. **TwoFactorService** - Implementação completa com:
   - EnableTwoFactorAsync() ✅
   - DisableTwoFactorAsync() ✅
   - GenerateAndSendCodeAsync() ✅
   - VerifyCodeAsync() ✅
   - ResendCodeAsync() ✅
   - IsTwoFactorEnabledAsync() ✅

4. **Endpoints da API** - Todos funcionais:
   - POST /api/auth/enable-2fa ✅
   - POST /api/auth/disable-2fa ✅
   - GET /api/auth/2fa-status ✅
   - POST /api/auth/verify-2fa ✅
   - POST /api/auth/resend-2fa-code ✅

5. **Integração com Login** - Completa:
   - AuthService integrado com ITwoFactorService ✅
   - LoginAsync verifica se 2FA está habilitado ✅
   - Retorna TwoFactorRequiredDto quando necessário ✅
   - CompleteLoginAfter2FAAsync implementado ✅

6. **Testes** - Suite completa de testes de integração:
   - TwoFactorAuthTests.cs com 14 cenários de teste ✅
   - Testes de habilitação/desabilitação ✅
   - Testes de verificação de código ✅
   - Testes de reenvio de código ✅
   - Testes de autenticação e autorização ✅
   - Testes de rate limiting ✅

7. **Segurança**:
   - Códigos gerados com CSPRNG e rejection sampling ✅
   - Rate limiting: 3 códigos/hora ✅
   - Máximo 5 tentativas de verificação ✅
   - Códigos válidos por 5 minutos ✅
   - TempToken codifica userId:tokenId para prevenir ataques ✅
   - HTML encoding em templates de email ✅

8. **Documentação**:
   - API_2FA_DOCUMENTATION.md - Documentação completa de endpoints ✅
   - GUIA_USUARIO_2FA.md - Guia do usuário ✅
   - SECURITY_SUMMARY_2FA.md - Análise de segurança ✅
   - PLANO_2FA_DESENVOLVIMENTO.md - Plano de desenvolvimento ✅

#### Status Final:
- **Implementação**: 100% completa
- **Testes**: 100% completa
- **Documentação**: 100% completa
- **Pronto para produção**: ✅ Sim

---

## Análise do Sistema Atual

### Fluxos de Autenticação Identificados

1. **Portal do Paciente (PatientPortal)**
   - Localização: `/patient-portal-api`
   - Endpoint: `PatientPortal.Api/Controllers/AuthController.cs`
   - Serviço: `PatientPortal.Application/Services/AuthService.cs`
   - Entidades:
     - `PatientUser` - já possui campo `TwoFactorEnabled` e `TwoFactorSecret`
     - `EmailVerificationToken` - para verificação de email
     - `PasswordResetToken` - para recuperação de senha
   - Fluxos existentes:
     - Login (email ou CPF)
     - Registro
     - Verificação de email
     - Recuperação de senha
     - Bloqueio de conta após 5 tentativas falhas

2. **Sistema Principal (MedicSoft.Api)**
   - Localização: `/src/MedicSoft.Api`
   - Endpoint: `MedicSoft.Api/Controllers/AuthController.cs`
   - Serviço: `MedicSoft.Application/Services/IAuthService`
   - Entidades:
     - `User` - usuários do sistema (médicos, secretárias, etc.)
     - `TwoFactorAuth` - entidade já existente para 2FA
     - `LoginAttempt` - rastreamento de tentativas de login
     - `AccountLockout` - bloqueio de contas
   - Serviço 2FA: `TwoFactorAuthService` - já implementado para TOTP
   - Fluxos existentes:
     - Login de usuários regulares
     - Login de proprietários (owners)
     - Suporte a múltiplas clínicas
     - Gerenciamento de sessões

### Infraestrutura Existente

#### Email
- **PatientPortal**: `NotificationService` implementado com SMTP
  - Configuração via `EmailSettings`
  - Suporte para HTML
  - Já usado para verificação de email e recuperação de senha
  
- **MedicSoft**: `SendGridEmailService` para CRM
  - Integração com SendGrid
  - Templates de email

#### 2FA Atual
- Entidade `TwoFactorAuth` já existe em `MedicSoft.Domain`
- Enum `TwoFactorMethod`: None, TOTP, SMS, Email
- `TwoFactorAuthService` implementado para TOTP (Google Authenticator)
- Sistema de backup codes já implementado
- Criptografia de chaves secretas

#### SMS/WhatsApp
- Interface `INotificationService` já prevê métodos para SMS e WhatsApp
- Implementação atual é apenas placeholder
- Preparado para integração futura com Twilio/AWS SNS

## Estratégia de Implementação

### Fase 1: 2FA por Email - Portal do Paciente ✅ (Prioritário)

Esta é a implementação mais crítica pois afeta diretamente os pacientes.

#### 1.1. Criar Entidade TwoFactorToken

```csharp
// PatientPortal.Domain/Entities/TwoFactorToken.cs
public class TwoFactorToken
{
    public Guid Id { get; set; }
    public Guid PatientUserId { get; set; }
    public string Code { get; set; } // Código de 6 dígitos
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; } // Válido por 5 minutos
    public bool IsUsed { get; set; }
    public string Purpose { get; set; } // "Login" ou "Verification"
    public string IpAddress { get; set; }
    
    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;
}
```

#### 1.2. Criar Repositório

```csharp
// PatientPortal.Domain/Interfaces/ITwoFactorTokenRepository.cs
public interface ITwoFactorTokenRepository
{
    Task<TwoFactorToken> CreateAsync(TwoFactorToken token);
    Task<TwoFactorToken?> GetByCodeAsync(string code, Guid patientUserId);
    Task UpdateAsync(TwoFactorToken token);
    Task DeleteExpiredTokensAsync();
}
```

#### 1.3. Criar Serviço de 2FA

```csharp
// PatientPortal.Application/Interfaces/ITwoFactorService.cs
public interface ITwoFactorService
{
    Task<bool> EnableTwoFactorAsync(Guid patientUserId);
    Task<bool> DisableTwoFactorAsync(Guid patientUserId);
    Task<string> GenerateAndSendCodeAsync(Guid patientUserId, string purpose);
    Task<bool> VerifyCodeAsync(Guid patientUserId, string code);
    Task<bool> IsTwoFactorEnabledAsync(Guid patientUserId);
}
```

#### 1.4. Atualizar Fluxo de Login

**Fluxo Atual:**
```
1. POST /api/auth/login (email/cpf + senha)
2. Verificar credenciais
3. Gerar JWT tokens
4. Retornar tokens
```

**Novo Fluxo com 2FA:**
```
1. POST /api/auth/login (email/cpf + senha)
2. Verificar credenciais
3. SE 2FA habilitado:
   a. Gerar código de 6 dígitos
   b. Enviar código por email
   c. Retornar: { requiresTwoFactor: true, tempToken: "..." }
4. SENÃO:
   a. Gerar JWT tokens
   b. Retornar tokens

5. POST /api/auth/verify-2fa (tempToken + código)
6. Verificar código
7. Gerar JWT tokens
8. Retornar tokens
```

#### 1.5. Novos Endpoints

```csharp
// AuthController.cs

[HttpPost("enable-2fa")]
[Authorize]
public async Task<IActionResult> EnableTwoFactor()

[HttpPost("disable-2fa")]
[Authorize]
public async Task<IActionResult> DisableTwoFactor()

[HttpPost("verify-2fa")]
[AllowAnonymous]
public async Task<ActionResult<LoginResponseDto>> VerifyTwoFactor([FromBody] VerifyTwoFactorDto request)

[HttpPost("resend-2fa-code")]
[AllowAnonymous]
public async Task<IActionResult> ResendTwoFactorCode([FromBody] ResendTwoFactorCodeDto request)
```

#### 1.6. Templates de Email

```html
<!-- Template: Código 2FA para Login -->
Título: Código de Verificação - Portal do Paciente

Olá {NomePaciente},

Seu código de verificação para login no Portal do Paciente é:

{CodigoVerificacao}

Este código expira em 5 minutos.

Se você não solicitou este código, ignore este email ou entre em contato conosco.
```

#### 1.7. Segurança

- Código de 6 dígitos numérico (000000-999999)
- Validade de 5 minutos
- Rate limiting: máximo 3 códigos por hora por usuário
- Rate limiting: máximo 5 tentativas de verificação por código
- Log de todas as tentativas de 2FA
- Invalidar código após uso
- Limpar códigos expirados periodicamente (job em background)

### Fase 2: 2FA por Email - Sistema Principal (MedicSoft)

Expandir o serviço `TwoFactorAuthService` existente para suportar email.

#### 2.1. Estender TwoFactorAuthService

```csharp
// Adicionar ao ITwoFactorAuthService
Task<string> EnableEmailAsync(string userId, string email, string ipAddress, string tenantId);
Task<bool> VerifyEmailCodeAsync(string userId, string code, string tenantId);
Task SendEmailCodeAsync(string userId, string tenantId);
```

#### 2.2. Reutilizar Infraestrutura

- Usar a mesma entidade `TwoFactorAuth` existente
- Adicionar método `Email` ao enum `TwoFactorMethod` (já existe!)
- Criar tabela `TwoFactorTokens` para códigos temporários

#### 2.3. Integração com Email

- Usar `SendGridEmailService` do CRM para envios
- Templates profissionais para médicos e administradores

### Fase 3: Preparação para SMS/WhatsApp

#### 3.1. Interface Unificada de Notificações

```csharp
// MedicSoft.Application/Services/IUnifiedNotificationService.cs
public interface IUnifiedNotificationService
{
    Task SendTwoFactorCodeAsync(string recipient, string code, NotificationChannel channel);
}

public enum NotificationChannel
{
    Email,
    SMS,
    WhatsApp
}
```

#### 3.2. Provedores de SMS

**Opção 1: Twilio** (Recomendado)
- Suporte para SMS e WhatsApp
- API bem documentada
- Preço competitivo
- Configuração:
  ```json
  "Twilio": {
    "AccountSid": "...",
    "AuthToken": "...",
    "PhoneNumber": "+55...",
    "WhatsAppNumber": "whatsapp:+55..."
  }
  ```

**Opção 2: AWS SNS**
- Integrado com ecossistema AWS
- Bom para volumes altos
- Configuração:
  ```json
  "AWS": {
    "AccessKeyId": "...",
    "SecretAccessKey": "...",
    "Region": "us-east-1",
    "SnsTopicArn": "..."
  }
  ```

**Opção 3: Vonage (Nexmo)**
- API simples
- Boa cobertura no Brasil

#### 3.3. Implementação

```csharp
// MedicSoft.Infrastructure/Services/TwilioSmsService.cs
public class TwilioSmsService : ISmsService
{
    public async Task SendAsync(string phoneNumber, string message)
    {
        var client = new TwilioRestClient(accountSid, authToken);
        await MessageResource.CreateAsync(
            to: new PhoneNumber(phoneNumber),
            from: new PhoneNumber(fromNumber),
            body: message
        );
    }
}

// MedicSoft.Infrastructure/Services/TwilioWhatsAppService.cs
public class TwilioWhatsAppService : IWhatsAppService
{
    public async Task SendAsync(string phoneNumber, string message)
    {
        var client = new TwilioRestClient(accountSid, authToken);
        await MessageResource.CreateAsync(
            to: new PhoneNumber($"whatsapp:{phoneNumber}"),
            from: new PhoneNumber($"whatsapp:{whatsappNumber}"),
            body: message
        );
    }
}
```

#### 3.4. Configuração por Usuário

```csharp
// Adicionar ao PatientUser e User
public TwoFactorMethod PreferredTwoFactorMethod { get; set; }
```

Interface do usuário permitirá escolher:
- Email (padrão)
- SMS (requer número verificado)
- WhatsApp (requer número verificado)
- TOTP/Authenticator App (mais seguro)

### Fase 4: Recuperação de Senha com 2FA

#### 4.1. Fluxo Atualizado

Quando o usuário tem 2FA ativado:

**Opção 1: Desabilitar 2FA temporariamente via email**
```
1. Solicitar recuperação de senha
2. Receber link por email
3. Link permite resetar senha E desabilitar 2FA
4. Usuário pode reativar 2FA após login
```

**Opção 2: Usar códigos de backup**
```
1. Tentar login
2. Clicar em "Perdeu acesso ao 2FA?"
3. Inserir um dos códigos de backup
4. Permitir login e configurar novo 2FA
```

#### 4.2. Códigos de Backup

Sistema já implementado no `TwoFactorAuthService`, precisa ser exposto no PatientPortal:

```csharp
[HttpGet("backup-codes")]
[Authorize]
public async Task<ActionResult<BackupCodesDto>> GetBackupCodes()

[HttpPost("regenerate-backup-codes")]
[Authorize]
public async Task<ActionResult<BackupCodesDto>> RegenerateBackupCodes()

[HttpPost("login-with-backup")]
[AllowAnonymous]
public async Task<ActionResult<LoginResponseDto>> LoginWithBackupCode([FromBody] BackupCodeLoginDto request)
```

## Cronograma Estimado

### Sprint 1 (Semana 1-2): 2FA Email - PatientPortal
- [x] Análise completa dos fluxos
- [ ] Criar entidade TwoFactorToken + repositório
- [ ] Implementar TwoFactorService
- [ ] Atualizar AuthService para suportar 2FA
- [ ] Criar novos endpoints
- [ ] Templates de email
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Documentação API

### Sprint 2 (Semana 3): 2FA Email - MedicSoft.Api
- [ ] Estender TwoFactorAuthService
- [ ] Criar repositório para tokens
- [ ] Integrar com SendGridEmailService
- [ ] Atualizar AuthController
- [ ] Templates de email profissionais
- [ ] Testes
- [ ] Documentação

### Sprint 3 (Semana 4): Recuperação e Backup Codes
- [ ] Implementar backup codes no PatientPortal
- [ ] Atualizar fluxo de recuperação de senha
- [ ] Interface para gerenciar 2FA
- [ ] Testes de todos os cenários
- [ ] Guia do usuário

### Sprint 4 (Semana 5): Preparação SMS/WhatsApp
- [ ] Documentar arquitetura
- [ ] Criar interfaces e stubs
- [ ] Implementação de exemplo com Twilio
- [ ] Guia de configuração
- [ ] Documentação de custos

## Considerações de Segurança

### Implementadas
- ✅ Códigos com entropia adequada (6 dígitos = 1 milhão de possibilidades)
- ✅ Validade curta (5 minutos)
- ✅ Rate limiting para prevenção de brute force
- ✅ Log de todas as tentativas
- ✅ Invalidação após uso
- ✅ Hash de códigos de backup
- ✅ Criptografia de chaves TOTP

### A Implementar
- [ ] Notificação ao usuário quando 2FA é habilitado/desabilitado
- [ ] Alerta de login de localização nova (mesmo com 2FA)
- [ ] Dashboard de atividades de segurança
- [ ] Auditoria completa de eventos 2FA

## Métricas de Sucesso

1. **Adoção**: % de usuários que habilitam 2FA
2. **Segurança**: Redução em contas comprometidas
3. **Usabilidade**: Taxa de conclusão do login com 2FA
4. **Performance**: Tempo de entrega dos códigos por email
5. **Suporte**: Número de tickets relacionados a 2FA

## Custos Estimados (SMS/WhatsApp)

### Twilio (Brasil)
- SMS: R$ 0,25 - R$ 0,35 por mensagem
- WhatsApp: R$ 0,15 - R$ 0,25 por mensagem
- Estimativa para 10.000 usuários/mês (2 códigos/mês): R$ 5.000 - R$ 7.000

### AWS SNS (Brasil)
- SMS: R$ 0,20 - R$ 0,30 por mensagem
- Volumes altos podem ter desconto

### Recomendação
- Começar com **Email** (custo zero)
- Oferecer **SMS/WhatsApp como premium** ou para usuários críticos
- Incentivar uso de **Authenticator App** (custo zero, mais seguro)

## Próximos Passos

1. Revisar e aprovar este plano
2. Criar issues no GitHub para cada sprint
3. Configurar ambiente de desenvolvimento
4. Implementar Sprint 1 (2FA Email - PatientPortal)
5. Testes com usuários beta
6. Rollout gradual

## Referências

- [RFC 6238 - TOTP](https://tools.ietf.org/html/rfc6238)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [NIST Digital Identity Guidelines](https://pages.nist.gov/800-63-3/sp800-63b.html)
- [Twilio API Documentation](https://www.twilio.com/docs)
- [WhatsApp Business API](https://developers.facebook.com/docs/whatsapp)
