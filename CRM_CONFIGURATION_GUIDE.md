# ⚙️ Guia de Configuração - Sistema CRM Avançado

**Versão:** 1.0  
**Data:** Janeiro 2026  
**Sistema:** MedicSoft - PrimeCare

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Pré-requisitos](#pré-requisitos)
3. [Configuração de Email (SendGrid)](#configuração-de-email-sendgrid)
4. [Configuração de SMS (Twilio)](#configuração-de-sms-twilio)
5. [Configuração de WhatsApp Business API](#configuração-de-whatsapp-business-api)
6. [Azure Cognitive Services](#azure-cognitive-services)
7. [Variáveis de Ambiente](#variáveis-de-ambiente)
8. [Testes de Integração](#testes-de-integração)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

Este guia detalha a configuração de todas as integrações externas necessárias para o funcionamento completo do Sistema CRM Avançado.

### Serviços de Terceiros

| Serviço | Propósito | Custo Estimado | Obrigatório |
|---------|-----------|----------------|-------------|
| SendGrid | Envio de emails | ~R$ 300/mês | ✅ Sim |
| Twilio | Envio de SMS | ~R$ 700/mês | ⚠️ Recomendado |
| WhatsApp Business API | Mensagens WhatsApp | ~R$ 800/mês | ⚠️ Recomendado |
| Azure Cognitive Services | IA e Análise de Sentimento | ~R$ 500/mês | ⚠️ Recomendado |

> **✅ Atualização (28/01/2026):** As integrações de Email (SendGrid), SMS (Twilio) e WhatsApp Business API estão **totalmente implementadas** e prontas para uso em produção. Configure as credenciais no `appsettings.json` e ative com `"Enabled": true`. Veja o guia completo em [`/CRM_MESSAGING_INTEGRATIONS.md`](/CRM_MESSAGING_INTEGRATIONS.md).

---

## 🔧 Pré-requisitos

### Software

- .NET 8.0 SDK ou superior
- PostgreSQL 14+
- Acesso à internet para APIs externas
- Certificados SSL/TLS válidos

### Contas de Serviço

Crie contas nos seguintes serviços:

1. **SendGrid:** https://sendgrid.com/
2. **Twilio:** https://www.twilio.com/
3. **Meta Business (WhatsApp):** https://business.whatsapp.com/
4. **Microsoft Azure:** https://portal.azure.com/

### Permissões

- Administrador do sistema MedicSoft
- Acesso ao servidor de aplicação
- Permissão para modificar variáveis de ambiente
- Acesso ao repositório de código (para desenvolvimento)

---

## 📧 Configuração de Email (SendGrid)

### 1. Criar Conta SendGrid

1. Acesse https://sendgrid.com/ e crie uma conta
2. Escolha o plano adequado:
   - **Free:** 100 emails/dia (desenvolvimento)
   - **Essentials:** R$ 300/mês - 50.000 emails/mês (recomendado)
   - **Pro:** R$ 1.500/mês - 100.000 emails/mês

### 2. Gerar API Key

1. No dashboard SendGrid, vá para **Settings > API Keys**
2. Clique em **Create API Key**
3. Preencha:
   - **Name:** MedicSoft-CRM-Production
   - **Permissions:** Full Access (ou Restricted com permissões de Mail Send)
4. Copie a chave gerada (aparece apenas uma vez!)

### 3. Configurar Domínio

#### Autenticação de Domínio (SPF/DKIM)

1. Vá para **Settings > Sender Authentication**
2. Clique em **Authenticate Your Domain**
3. Siga as instruções para adicionar registros DNS:

```dns
# Exemplo de registros DNS
Type: CNAME
Host: s1._domainkey.seudominio.com.br
Value: s1.domainkey.u12345.wl123.sendgrid.net

Type: CNAME
Host: s2._domainkey.seudominio.com.br
Value: s2.domainkey.u12345.wl123.sendgrid.net

Type: TXT
Host: seudominio.com.br
Value: v=spf1 include:sendgrid.net ~all
```

4. Aguarde propagação DNS (até 48h)
5. Verifique autenticação no dashboard

### 4. Criar Sender Identity

1. Vá para **Settings > Sender Authentication > Single Sender Verification**
2. Adicione:
   - **From Name:** PrimeCare Saúde
   - **From Email:** noreply@seudominio.com.br
   - **Reply To:** contato@seudominio.com.br
   - **Company Address:** Endereço completo da clínica
3. Verifique o email de confirmação

### 5. Templates Dinâmicos (Opcional)

1. Vá para **Email API > Dynamic Templates**
2. Crie templates com design personalizado
3. Use Handlebars para variáveis: `{{nome}}`, `{{data}}`
4. Copie o Template ID para usar na aplicação

### 6. Configurar no MedicSoft

#### Opção A: Variáveis de Ambiente

Edite `.env` ou configure no servidor:

```env
# SendGrid Configuration
EMAIL_SERVICE_PROVIDER=SendGrid
SENDGRID_API_KEY=SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
SENDGRID_FROM_EMAIL=noreply@seudominio.com.br
SENDGRID_FROM_NAME=PrimeCare Saúde
SENDGRID_REPLY_TO=contato@seudominio.com.br
```

#### Opção B: appsettings.json

```json
{
  "EmailSettings": {
    "Provider": "SendGrid",
    "SendGrid": {
      "ApiKey": "SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
      "FromEmail": "noreply@seudominio.com.br",
      "FromName": "PrimeCare Saúde",
      "ReplyTo": "contato@seudominio.com.br",
      "EnableClickTracking": true,
      "EnableOpenTracking": true
    }
  }
}
```

### 7. Implementação no Código

Substitua `StubEmailService` por `SendGridEmailService`:

```csharp
// src/MedicSoft.Api/Services/CRM/SendGridEmailService.cs
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        ISendGridClient client,
        IConfiguration configuration,
        ILogger<SendGridEmailService> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        Dictionary<string, string>? variables = null)
    {
        try
        {
            var from = new EmailAddress(
                _configuration["EmailSettings:SendGrid:FromEmail"],
                _configuration["EmailSettings:SendGrid:FromName"]);
            
            var toAddress = new EmailAddress(to);
            
            var msg = MailHelper.CreateSingleEmail(
                from,
                toAddress,
                subject,
                plainTextContent: null,
                htmlContent: ReplaceVariables(body, variables));
            
            var response = await _client.SendEmailAsync(msg);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To}", to);
                return true;
            }
            else
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email. Status: {Status}, Error: {Error}",
                    response.StatusCode, errorBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending email to {To}", to);
            return false;
        }
    }

    private string ReplaceVariables(string template, Dictionary<string, string>? variables)
    {
        if (variables == null) return template;
        
        foreach (var kvp in variables)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }
        return template;
    }
}
```

Registre no `Program.cs`:

```csharp
// Program.cs
builder.Services.AddSendGrid(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new SendGridClient(config["EmailSettings:SendGrid:ApiKey"]);
});

builder.Services.AddScoped<IEmailService, SendGridEmailService>();
```

### 8. Monitoramento

- **Dashboard:** https://app.sendgrid.com/
- **Activity Feed:** Veja todos os emails enviados
- **Suppressions:** Gerencie bounces e unsubscribes
- **Analytics:** Taxa de abertura, cliques, bounces

---

## 📱 Configuração de SMS (Twilio)

### 1. Criar Conta Twilio

1. Acesse https://www.twilio.com/ e crie uma conta
2. Escolha o plano adequado:
   - **Trial:** Gratuito com limitações (desenvolvimento)
   - **Pay as you go:** R$ 0,15 por SMS (recomendado)

### 2. Obter Credenciais

1. No dashboard Twilio, copie:
   - **Account SID:** AC...
   - **Auth Token:** (clique para revelar)

### 3. Comprar Número de Telefone

1. Vá para **Phone Numbers > Buy a Number**
2. Selecione:
   - **Country:** Brazil (+55)
   - **Capabilities:** SMS
3. Escolha um número e compre
4. Copie o número comprado (formato: +5511999999999)

### 4. Configurar Messaging Service (Opcional mas Recomendado)

1. Vá para **Messaging > Services**
2. Crie um novo serviço: "MedicSoft-CRM"
3. Adicione o número comprado ao serviço
4. Configure:
   - **Use Case:** Notifications
   - **Fallback to Long Code:** Enabled
5. Copie o **Messaging Service SID**

### 5. Configurar no MedicSoft

```env
# Twilio Configuration
SMS_SERVICE_PROVIDER=Twilio
TWILIO_ACCOUNT_SID=ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_AUTH_TOKEN=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_PHONE_NUMBER=+5511999999999
TWILIO_MESSAGING_SERVICE_SID=MGxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (opcional)
```

### 6. Implementação no Código

```csharp
// src/MedicSoft.Api/Services/CRM/TwilioSmsService.cs
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioSmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var accountSid = _configuration["SmsSettings:Twilio:AccountSid"];
        var authToken = _configuration["SmsSettings:Twilio:AuthToken"];
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var from = new PhoneNumber(_configuration["SmsSettings:Twilio:PhoneNumber"]);
            var to = new PhoneNumber(NormalizePhoneNumber(phoneNumber));

            var messageOptions = new CreateMessageOptions(to)
            {
                From = from,
                Body = message
            };

            // Use Messaging Service se configurado
            var messagingServiceSid = _configuration["SmsSettings:Twilio:MessagingServiceSid"];
            if (!string.IsNullOrEmpty(messagingServiceSid))
            {
                messageOptions.MessagingServiceSid = messagingServiceSid;
                messageOptions.From = null; // Remove From quando usa Messaging Service
            }

            var result = await MessageResource.CreateAsync(messageOptions);

            if (result.ErrorCode.HasValue)
            {
                _logger.LogError("Failed to send SMS. Error: {Error}", result.ErrorMessage);
                return false;
            }

            _logger.LogInformation("SMS sent successfully. SID: {Sid}", result.Sid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    private string NormalizePhoneNumber(string phoneNumber)
    {
        // Remove caracteres não numéricos
        var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // Adiciona +55 se necessário
        if (!cleaned.StartsWith("55"))
            cleaned = "55" + cleaned;
            
        return "+" + cleaned;
    }
}
```

Registre no `Program.cs`:

```csharp
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
```

### 7. Monitoramento

- **Dashboard:** https://console.twilio.com/
- **Message Logs:** Veja status de todos os SMS
- **Usage:** Monitore consumo e custos
- **Alerts:** Configure alertas de budget

---

## 💬 Configuração de WhatsApp Business API

### 1. Requisitos

- Facebook Business Manager configurado
- Número de telefone dedicado (não pode usar WhatsApp pessoal)
- Verificação de identidade comercial

### 2. Criar Conta WhatsApp Business

1. Acesse https://business.whatsapp.com/
2. Crie ou vincule à sua conta Business Manager
3. Adicione um número de telefone
4. Verifique o número (SMS ou chamada)

### 3. Configurar via Twilio (Recomendado para Integração Fácil)

Twilio oferece integração simplificada com WhatsApp Business API.

1. No console Twilio, vá para **Messaging > Try it out > Send a WhatsApp message**
2. Siga o wizard de configuração:
   - Conecte sua conta WhatsApp Business
   - Configure templates de mensagens
   - Obtenha aprovação do WhatsApp

### 4. Templates de Mensagem

WhatsApp exige que mensagens proativas (iniciadas pela empresa) usem templates pré-aprovados.

#### Criar Template

1. Vá para **Messaging > Content Editor**
2. Crie um template:

```
Nome: consulta_confirmacao
Categoria: APPOINTMENT_UPDATE
Idioma: pt_BR

Mensagem:
Olá {{1}},

Sua consulta com Dr(a). {{2}} está confirmada para {{3}}.

Local: {{4}}

Para cancelar ou remarcar, responda esta mensagem.

Atenciosamente,
{{5}}
```

3. Envie para aprovação do WhatsApp (1-3 dias)

### 5. Configurar no MedicSoft

```env
# WhatsApp Configuration
WHATSAPP_SERVICE_PROVIDER=TwilioWhatsApp
TWILIO_WHATSAPP_NUMBER=whatsapp:+14155238886  # Twilio Sandbox ou seu número
WHATSAPP_API_VERSION=v17.0
```

### 6. Implementação no Código

```csharp
// src/MedicSoft.Api/Services/CRM/TwilioWhatsAppService.cs
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwilioWhatsAppService> _logger;

    public TwilioWhatsAppService(IConfiguration configuration, ILogger<TwilioWhatsAppService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var accountSid = _configuration["SmsSettings:Twilio:AccountSid"];
        var authToken = _configuration["SmsSettings:Twilio:AuthToken"];
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task<bool> SendWhatsAppAsync(
        string phoneNumber,
        string message,
        string? templateName = null,
        Dictionary<string, string>? templateVariables = null)
    {
        try
        {
            var from = new PhoneNumber(_configuration["WhatsAppSettings:TwilioWhatsAppNumber"]);
            var to = new PhoneNumber("whatsapp:" + NormalizePhoneNumber(phoneNumber));

            var messageOptions = new CreateMessageOptions(to)
            {
                From = from
            };

            // Se usar template aprovado
            if (!string.IsNullOrEmpty(templateName) && templateVariables != null)
            {
                messageOptions.Body = ReplaceTemplateVariables(message, templateVariables);
            }
            else
            {
                messageOptions.Body = message;
            }

            var result = await MessageResource.CreateAsync(messageOptions);

            if (result.ErrorCode.HasValue)
            {
                _logger.LogError("Failed to send WhatsApp. Error: {Error}", result.ErrorMessage);
                return false;
            }

            _logger.LogInformation("WhatsApp sent successfully. SID: {Sid}", result.Sid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    private string NormalizePhoneNumber(string phoneNumber)
    {
        var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (!cleaned.StartsWith("55"))
            cleaned = "55" + cleaned;
        return "+" + cleaned;
    }

    private string ReplaceTemplateVariables(string template, Dictionary<string, string> variables)
    {
        var result = template;
        for (int i = 1; i <= variables.Count; i++)
        {
            if (variables.TryGetValue(i.ToString(), out var value))
            {
                result = result.Replace($"{{{{{i}}}}}", value);
            }
        }
        return result;
    }
}
```

Registre no `Program.cs`:

```csharp
builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();
```

### 7. Webhooks para Respostas

Configure um endpoint para receber respostas:

```csharp
// src/MedicSoft.Api/Controllers/WebhooksController.cs
[ApiController]
[Route("api/webhooks")]
public class WhatsAppWebhookController : ControllerBase
{
    [HttpPost("whatsapp/incoming")]
    public async Task<IActionResult> IncomingWhatsApp([FromForm] TwilioWebhookRequest request)
    {
        // Processar mensagem recebida
        _logger.LogInformation("WhatsApp received from {From}: {Body}", 
            request.From, request.Body);
        
        // Lógica de processamento...
        
        return Ok();
    }
}
```

Configure a URL no Twilio:
- **URL:** https://seudominio.com.br/api/webhooks/whatsapp/incoming
- **Method:** POST

---

## 🧠 Azure Cognitive Services

### 1. Criar Recurso no Azure

1. Acesse https://portal.azure.com/
2. Crie um novo recurso **Text Analytics**
3. Preencha:
   - **Subscription:** Sua assinatura
   - **Resource Group:** MedicSoft-CRM
   - **Region:** Brazil South (recomendado para menor latência)
   - **Name:** medicsoft-textanalytics
   - **Pricing Tier:** S0 (Standard) ou F0 (Free para testes)

### 2. Obter Credenciais

1. Após criar, acesse o recurso
2. Vá para **Keys and Endpoint**
3. Copie:
   - **Key 1** (ou Key 2)
   - **Endpoint** (ex: https://medicsoft-textanalytics.cognitiveservices.azure.com/)

### 3. Configurar no MedicSoft

```env
# Azure Cognitive Services Configuration
SENTIMENT_ANALYSIS_PROVIDER=AzureCognitiveServices
AZURE_COGNITIVE_SERVICES_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AZURE_COGNITIVE_SERVICES_ENDPOINT=https://medicsoft-textanalytics.cognitiveservices.azure.com/
AZURE_COGNITIVE_SERVICES_REGION=brazilsouth
```

### 4. Implementação no Código

```csharp
// src/MedicSoft.Api/Services/CRM/AzureSentimentAnalysisService.cs
using Azure;
using Azure.AI.TextAnalytics;

public class AzureSentimentAnalysisService : ISentimentAnalysisService
{
    private readonly TextAnalyticsClient _client;
    private readonly MedicSoftDbContext _context;
    private readonly ILogger<AzureSentimentAnalysisService> _logger;

    public AzureSentimentAnalysisService(
        IConfiguration configuration,
        MedicSoftDbContext context,
        ILogger<AzureSentimentAnalysisService> logger)
    {
        var endpoint = new Uri(configuration["AzureCognitiveServices:Endpoint"]);
        var credential = new AzureKeyCredential(configuration["AzureCognitiveServices:Key"]);
        _client = new TextAnalyticsClient(endpoint, credential);
        _context = context;
        _logger = logger;
    }

    public async Task<SentimentAnalysisResultDto> AnalyzeTextAsync(string text, string tenantId)
    {
        try
        {
            var response = await _client.AnalyzeSentimentAsync(text, language: "pt");
            var sentiment = response.Value;

            var sentimentType = sentiment.Sentiment switch
            {
                TextSentiment.Positive => SentimentType.Positive,
                TextSentiment.Negative => SentimentType.Negative,
                TextSentiment.Neutral => SentimentType.Neutral,
                _ => SentimentType.Neutral
            };

            // Extração de key phrases
            var keyPhrasesResponse = await _client.ExtractKeyPhrasesAsync(text, language: "pt");
            var topics = keyPhrasesResponse.Value.ToList();

            // Salvar no banco
            var analysis = new SentimentAnalysis
            {
                Text = text,
                Sentiment = sentimentType,
                ConfidenceScore = (double)sentiment.ConfidenceScores.Positive,
                Topics = topics,
                TenantId = tenantId,
                AnalyzedAt = DateTime.UtcNow
            };

            _context.SentimentAnalyses.Add(analysis);
            await _context.SaveChangesAsync();

            return new SentimentAnalysisResultDto
            {
                Sentiment = sentimentType,
                PositiveScore = (double)sentiment.ConfidenceScores.Positive,
                NegativeScore = (double)sentiment.ConfidenceScores.Negative,
                NeutralScore = (double)sentiment.ConfidenceScores.Neutral,
                Topics = topics,
                ConfidenceScore = (double)sentiment.ConfidenceScores.Positive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment with Azure");
            throw;
        }
    }
}
```

Registre no `Program.cs`:

```csharp
builder.Services.AddScoped<ISentimentAnalysisService, AzureSentimentAnalysisService>();
```

### 5. Recursos Disponíveis

- **Sentiment Analysis:** Identifica sentimento (positivo/negativo/neutro)
- **Key Phrase Extraction:** Extrai tópicos principais
- **Entity Recognition:** Identifica entidades (pessoas, locais, organizações)
- **Language Detection:** Detecta idioma automaticamente

### 6. Limites e Custos

**Tier F0 (Free):**
- 5.000 transações/mês
- Gratuito

**Tier S0 (Standard):**
- R$ 0,25 por 1.000 transações (0-500K)
- R$ 0,20 por 1.000 transações (500K-2.5M)
- R$ 0,15 por 1.000 transações (2.5M+)

---

## 🔐 Variáveis de Ambiente

### Arquivo .env Completo

```env
# Database
DATABASE_CONNECTION_STRING=Host=localhost;Database=medicsoft;Username=medicsoft;Password=xxx

# SendGrid
EMAIL_SERVICE_PROVIDER=SendGrid
SENDGRID_API_KEY=SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
SENDGRID_FROM_EMAIL=noreply@seudominio.com.br
SENDGRID_FROM_NAME=PrimeCare Saúde
SENDGRID_REPLY_TO=contato@seudominio.com.br

# Twilio SMS
SMS_SERVICE_PROVIDER=Twilio
TWILIO_ACCOUNT_SID=ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_AUTH_TOKEN=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_PHONE_NUMBER=+5511999999999
TWILIO_MESSAGING_SERVICE_SID=MGxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

# WhatsApp (via Twilio)
WHATSAPP_SERVICE_PROVIDER=TwilioWhatsApp
TWILIO_WHATSAPP_NUMBER=whatsapp:+5511999999999

# Azure Cognitive Services
SENTIMENT_ANALYSIS_PROVIDER=AzureCognitiveServices
AZURE_COGNITIVE_SERVICES_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AZURE_COGNITIVE_SERVICES_ENDPOINT=https://medicsoft-textanalytics.cognitiveservices.azure.com/
AZURE_COGNITIVE_SERVICES_REGION=brazilsouth

# Hangfire (Jobs)
HANGFIRE_CONNECTION_STRING=Host=localhost;Database=medicsoft;Username=medicsoft;Password=xxx
```

### Docker Secrets (Produção)

Para ambientes containerizados, use secrets:

```yaml
# docker-compose.yml
version: '3.8'
services:
  api:
    image: medicsoft/api:latest
    secrets:
      - sendgrid_api_key
      - twilio_auth_token
      - azure_cognitive_key
    environment:
      SENDGRID_API_KEY_FILE: /run/secrets/sendgrid_api_key
      TWILIO_AUTH_TOKEN_FILE: /run/secrets/twilio_auth_token
      AZURE_COGNITIVE_SERVICES_KEY_FILE: /run/secrets/azure_cognitive_key

secrets:
  sendgrid_api_key:
    external: true
  twilio_auth_token:
    external: true
  azure_cognitive_key:
    external: true
```

---

## 🧪 Testes de Integração

### 1. Teste de Email

```bash
curl -X POST https://seudominio.com.br/api/crm/test/email \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "to": "teste@exemplo.com",
    "subject": "Teste de Integração",
    "body": "Este é um teste de envio de email."
  }'
```

### 2. Teste de SMS

```bash
curl -X POST https://seudominio.com.br/api/crm/test/sms \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "phoneNumber": "+5511999999999",
    "message": "Teste de SMS do MedicSoft"
  }'
```

### 3. Teste de WhatsApp

```bash
curl -X POST https://seudominio.com.br/api/crm/test/whatsapp \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "phoneNumber": "+5511999999999",
    "message": "Teste de WhatsApp do MedicSoft"
  }'
```

### 4. Teste de Análise de Sentimento

```bash
curl -X POST https://seudominio.com.br/api/crm/test/sentiment \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "text": "O atendimento foi excelente e o médico muito atencioso!"
  }'
```

---

## 🔍 Troubleshooting

### Erros Comuns de Email

#### "Unauthorized"
- **Causa:** API Key inválida ou expirada
- **Solução:** Gere nova API Key no SendGrid

#### "Invalid From Address"
- **Causa:** Email não autenticado
- **Solução:** Complete autenticação de domínio

#### "Blocked"
- **Causa:** Email em lista de supressão
- **Solução:** Remova da suppression list no SendGrid

### Erros Comuns de SMS

#### "21211: Invalid 'To' Phone Number"
- **Causa:** Formato de telefone incorreto
- **Solução:** Use formato +55DDNNNNNNNNN

#### "21608: Unverified Number"
- **Causa:** Conta trial tentando enviar para número não verificado
- **Solução:** Upgrade para conta paga ou verifique o número

#### "20003: Authentication Error"
- **Causa:** Account SID ou Auth Token incorretos
- **Solução:** Verifique credenciais no console Twilio

### Erros Comuns de WhatsApp

#### "63016: Template not found"
- **Causa:** Template não aprovado ou nome incorreto
- **Solução:** Verifique status de aprovação

#### "63007: From number not enabled for WhatsApp"
- **Causa:** Número não configurado para WhatsApp
- **Solução:** Complete configuração WhatsApp Business no Twilio

### Erros Azure Cognitive Services

#### "401: Access Denied"
- **Causa:** Key inválida ou expirada
- **Solução:** Regenere key no Azure Portal

#### "429: Rate Limit Exceeded"
- **Causa:** Excedeu limite de requisições
- **Solução:** Aguarde ou upgrade do tier

---

## 📊 Monitoramento

### Métricas Recomendadas

- **Email Delivery Rate:** > 95%
- **Email Open Rate:** > 20%
- **SMS Delivery Rate:** > 98%
- **WhatsApp Delivery Rate:** > 95%
- **Sentiment Analysis Latency:** < 2s

### Logs

Verifique logs da aplicação:

```bash
# Logs de email
grep "Email sent" /var/log/medicsoft/application.log

# Logs de SMS
grep "SMS sent" /var/log/medicsoft/application.log

# Logs de erros
grep "ERROR" /var/log/medicsoft/application.log | grep "CRM"
```

### Alertas

Configure alertas para:
- Taxa de erro > 5%
- Latência > 5s
- Falhas consecutivas > 10

---

## 📞 Suporte

### Contatos de Suporte dos Fornecedores

- **SendGrid:** https://support.sendgrid.com/
- **Twilio:** https://support.twilio.com/
- **Meta/WhatsApp:** https://business.whatsapp.com/support
- **Azure:** https://azure.microsoft.com/support/

### Documentação Oficial

- **SendGrid API:** https://docs.sendgrid.com/
- **Twilio API:** https://www.twilio.com/docs/
- **WhatsApp Business API:** https://developers.facebook.com/docs/whatsapp/
- **Azure Text Analytics:** https://learn.microsoft.com/azure/cognitive-services/text-analytics/

---

**Última Atualização:** Janeiro 2026  
**Versão do Documento:** 1.0  
**Sistema:** MedicSoft CRM Advanced
