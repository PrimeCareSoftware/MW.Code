# Refatoração do Sistema de Email 2FA

## Resumo das Alterações

Este documento descreve a refatoração do sistema de autenticação de dois fatores (2FA) para usar envio de email direto via SMTP ao invés de APIs externas como SendGrid.

## Motivação

A refatoração foi realizada para:

1. **Eliminar Dependências Externas**: Remover dependência de APIs pagas como SendGrid
2. **Maior Controle**: Ter controle total sobre o envio de emails
3. **Uso de Infraestrutura Própria**: Utilizar diretamente o servidor SMTP da hospedagem (ex: Hostinger)
4. **Redução de Custos**: Não haver custos adicionais de serviços de terceiros
5. **Simplicidade**: Simplificar a configuração e manutenção

## Mudanças Implementadas

### 1. Novo Serviço SMTP (`SmtpEmailService`)

**Localização**: `/src/MedicSoft.Application/Services/Email/SmtpEmailService.cs`

Um novo serviço de email foi criado que implementa a interface `IEmailService` usando SMTP diretamente:

```csharp
public class SmtpEmailService : IEmailService
{
    // Envia emails usando System.Net.Mail.SmtpClient
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Implementação usando SMTP
    }
}
```

**Características**:
- ✅ Usa `System.Net.Mail.SmtpClient` nativo do .NET
- ✅ Suporta TLS/SSL
- ✅ Configuração via `appsettings.json` ou variáveis de ambiente
- ✅ Logging completo de erros e sucessos
- ✅ Timeout configurável
- ✅ Validação de parâmetros

### 2. Configuração de Email Atualizada

**Localização**: `/src/MedicSoft.Api/appsettings.json`

A seção de configuração foi atualizada:

**Antes (usando SendGrid)**:
```json
{
  "Messaging": {
    "Email": {
      "ApiKey": "",
      "FromEmail": "no-reply@primecaretech.com.br",
      "FromName": "PrimeCare Software",
      "UseSandbox": true,
      "Enabled": false
    }
  }
}
```

**Depois (usando SMTP)**:
```json
{
  "Email": {
    "SmtpServer": "smtp.hostinger.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "",
    "Password": "",
    "From": "noreply@primecare.com.br",
    "FromName": "PrimeCare Software",
    "Enabled": false,
    "TimeoutSeconds": 30
  }
}
```

### 3. Registro de Serviço Atualizado

**Localização**: `/src/MedicSoft.Api/Program.cs`

**Antes**:
```csharp
var useRealEmailService = builder.Configuration.GetValue<bool>("Messaging:Email:Enabled");
if (useRealEmailService)
{
    builder.Services.AddScoped<IEmailService, SendGridEmailService>();
}
```

**Depois**:
```csharp
// Configure SMTP email settings
builder.Services.Configure<SmtpEmailSettings>(
    builder.Configuration.GetSection(SmtpEmailSettings.SectionName));

var useRealEmailService = builder.Configuration.GetValue<bool>("Email:Enabled");
if (useRealEmailService)
{
    builder.Services.AddScoped<IEmailService, SmtpEmailService>();
}
```

### 4. Documentação de Configuração

**Novo Arquivo**: `/HOSTINGER_EMAIL_CONFIG_GUIDE.md`

Um guia completo foi criado explicando:
- Como obter credenciais SMTP da Hostinger
- Passo a passo de configuração
- Exemplos de configuração
- Solução de problemas comuns
- Melhores práticas
- Configuração de SPF, DKIM e DMARC

### 5. Variáveis de Ambiente Atualizadas

**Localização**: `/.env.example`

Novas variáveis de ambiente foram adicionadas:

```bash
EMAIL_SMTP_SERVER=smtp.hostinger.com
EMAIL_SMTP_PORT=587
EMAIL_USE_SSL=true
EMAIL_USERNAME=noreply@seudominio.com.br
EMAIL_PASSWORD=
EMAIL_FROM=noreply@seudominio.com.br
EMAIL_FROM_NAME=PrimeCare Software
EMAIL_ENABLED=true
EMAIL_TIMEOUT_SECONDS=30
```

## Compatibilidade

### APIs Afetadas

1. **MedicSoft.Api** (API Principal)
   - ✅ Atualizada para usar `SmtpEmailService`
   - ✅ Configuração via `Email` section
   - ✅ Backward compatible (stub service quando desabilitado)

2. **PatientPortal.Api** (Portal do Paciente)
   - ℹ️ Já usa SMTP via `NotificationService`
   - ✅ Nenhuma alteração necessária
   - ✅ Continua funcionando com configuração existente

### Serviços que Usam Email

Os seguintes serviços continuam funcionando normalmente:

- ✅ **2FA (Autenticação de Dois Fatores)**
  - `TwoFactorAuthService` (API Principal)
  - `TwoFactorService` (Portal do Paciente)
  
- ✅ **Notificações do CRM**
  - Marketing automation
  - Campaigns
  - Patient journey
  
- ✅ **Recuperação de Senha**
  - Password reset emails
  
- ✅ **Notificações do Sistema**
  - System notifications
  - Alerts
  
- ✅ **Relatórios por Email**
  - Report delivery

## Migração

### Para Ambientes Existentes

1. **Atualizar Configuração**:
   
   Remova a seção antiga `Messaging:Email` e adicione a nova seção `Email`:

   ```json
   {
     "Email": {
       "SmtpServer": "smtp.hostinger.com",
       "SmtpPort": 587,
       "UseSsl": true,
       "Username": "seu-email@dominio.com.br",
       "Password": "sua-senha",
       "From": "seu-email@dominio.com.br",
       "FromName": "Nome da Empresa",
       "Enabled": true,
       "TimeoutSeconds": 30
     }
   }
   ```

2. **Configurar Variáveis de Ambiente** (Recomendado):

   ```bash
   export EMAIL_SMTP_SERVER="smtp.hostinger.com"
   export EMAIL_SMTP_PORT="587"
   export EMAIL_USE_SSL="true"
   export EMAIL_USERNAME="seu-email@dominio.com.br"
   export EMAIL_PASSWORD="sua-senha"
   export EMAIL_FROM="seu-email@dominio.com.br"
   export EMAIL_FROM_NAME="Nome da Empresa"
   export EMAIL_ENABLED="true"
   ```

3. **Reiniciar Aplicação**:
   ```bash
   # Parar aplicação
   systemctl stop primecare-api
   
   # Iniciar com novas configurações
   systemctl start primecare-api
   ```

4. **Testar Envio de Email**:
   - Tente fazer login e solicitar código 2FA
   - Verifique os logs para confirmar envio bem-sucedido
   - Confirme recebimento do email

## Testando a Implementação

### 1. Teste Local

```bash
# Configurar no appsettings.Development.json
{
  "Email": {
    "SmtpServer": "smtp.hostinger.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "teste@dominio.com.br",
    "Password": "senha-teste",
    "From": "teste@dominio.com.br",
    "FromName": "Teste Local",
    "Enabled": true
  }
}

# Executar API
dotnet run --project src/MedicSoft.Api

# Verificar logs
tail -f Logs/primecare-*.log
```

### 2. Teste de 2FA

1. Registrar/fazer login no sistema
2. Habilitar 2FA nas configurações do perfil
3. Fazer logout e login novamente
4. Verificar se recebe o código 2FA por email
5. Inserir código e confirmar login

### 3. Verificar Logs

Procure por mensagens de sucesso:
```
[Information] Sending email to user@example.com with subject: Código de Verificação 2FA
[Information] Email sent successfully to user@example.com
```

Ou mensagens de erro:
```
[Error] SMTP error sending email to user@example.com. Status: ...
```

## Solução de Problemas

### Erro: "SMTP server not configured"

**Causa**: Falta configuração do servidor SMTP

**Solução**: Adicione a seção `Email` no appsettings.json com todas as propriedades necessárias.

### Erro: "Authentication failed"

**Causa**: Credenciais SMTP incorretas

**Solução**: 
1. Verifique se o `Username` é o email completo
2. Confirme a senha no painel da Hostinger
3. Tente fazer login no webmail para validar credenciais

### Erro: "Connection timeout"

**Causa**: Porta bloqueada ou servidor inacessível

**Solução**:
1. Verifique firewall (porta 587 deve estar aberta)
2. Tente porta alternativa 465 com SSL
3. Teste conectividade: `telnet smtp.hostinger.com 587`

### Email não está sendo recebido

**Possíveis Causas**:
1. Email indo para spam
2. Configuração SPF/DKIM incorreta
3. Email bloqueado pelo destinatário

**Soluções**:
1. Verifique pasta de spam
2. Configure SPF, DKIM e DMARC (veja guia da Hostinger)
3. Verifique logs do servidor SMTP
4. Teste com diferentes provedores de email

## Rollback

Se necessário reverter para SendGrid:

1. **Restaurar Código Anterior**:
   ```bash
   git revert <commit-hash>
   ```

2. **Ou Manualmente**:
   - Alterar `Program.cs` para usar `SendGridEmailService`
   - Restaurar seção `Messaging:Email` no appsettings.json
   - Adicionar API Key do SendGrid

3. **Reinstalar Pacote SendGrid** (se removido):
   ```bash
   dotnet add package SendGrid
   ```

## Benefícios da Nova Implementação

✅ **Sem Custos Adicionais**: Usa infraestrutura existente da hospedagem  
✅ **Maior Privacidade**: Dados não passam por serviços terceiros  
✅ **Controle Total**: Configuração e troubleshooting direto  
✅ **Simplicidade**: Menos dependências e configurações  
✅ **Confiabilidade**: Usa protocolo SMTP padrão e amplamente suportado  
✅ **Flexibilidade**: Fácil trocar de provedor SMTP  

## Próximos Passos Recomendados

1. ✅ Configurar SPF, DKIM e DMARC para melhor deliverability
2. ✅ Implementar fila de emails para alto volume
3. ✅ Adicionar monitoramento de taxa de entrega
4. ✅ Criar templates HTML responsivos para emails
5. ✅ Implementar retry logic com backoff exponencial
6. ✅ Configurar alertas para falhas de envio

## Referências

- [Guia de Configuração Hostinger](./HOSTINGER_EMAIL_CONFIG_GUIDE.md)
- [Documentação API 2FA](./API_2FA_DOCUMENTATION.md)
- [System.Net.Mail.SmtpClient Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient)
- [RFC 5321 - SMTP Protocol](https://datatracker.ietf.org/doc/html/rfc5321)

---

**Data da Refatoração**: Fevereiro 2026  
**Versão**: 1.0  
**Status**: ✅ Concluído
