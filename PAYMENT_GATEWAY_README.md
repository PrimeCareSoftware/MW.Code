# Gateway de Pagamento - Mercado Pago

## Visão Geral

Este documento descreve a implementação do gateway de pagamento Mercado Pago no sistema PrimeCare, incluindo configuração, uso e feature flags.

**Data de Criação:** 02 de Fevereiro de 2026  
**Status:** Implementado - Aguardando Credenciais

---

## 1. Arquitetura

### 1.1 Componentes Implementados

```
MedicSoft.Application/
├── Configurations/
│   └── PaymentGatewaySettings.cs          # Configurações do gateway
├── Interfaces/
│   └── IPaymentGatewayService.cs          # Interface do serviço
└── Services/
    └── MercadoPagoPaymentGatewayService.cs # Implementação Mercado Pago
```

### 1.2 Feature Flag

O sistema inclui uma feature flag `CreditCardPayments` na entidade `BusinessConfiguration` que permite habilitar/desabilitar pagamentos por cartão de crédito por clínica.

**Localização:** `MedicSoft.Domain/Entities/BusinessConfiguration.cs`

```csharp
public bool CreditCardPayments { get; private set; }
```

---

## 2. Configuração

### 2.1 Configuração no appsettings.json

Adicione as seguintes configurações ao `appsettings.json`:

```json
{
  "PaymentGateway": {
    "Provider": "MercadoPago",
    "Enabled": true,
    "MercadoPago": {
      "AccessToken": "",
      "PublicKey": "",
      "WebhookSecret": "",
      "ApiUrl": "https://api.mercadopago.com",
      "Enabled": false,
      "NotificationUrl": "https://seu-dominio.com/api/webhooks/mercadopago"
    },
    "EnableCreditCardPayments": true,
    "EnablePixPayments": true,
    "EnableBankSlipPayments": false,
    "TimeoutSeconds": 30
  }
}
```

### 2.2 Credenciais Necessárias

Para ativar a integração com Mercado Pago, você precisa:

1. **AccessToken**: Token de acesso da sua conta Mercado Pago
2. **PublicKey**: Chave pública para operações do lado do cliente
3. **WebhookSecret**: Segredo para validar notificações webhook
4. **NotificationUrl**: URL pública para receber notificações de pagamento

#### Como Obter as Credenciais

1. Acesse [Mercado Pago Developers](https://www.mercadopago.com.br/developers)
2. Faça login com sua conta
3. Vá para "Suas integrações" > "Criar aplicação"
4. Escolha o nome da aplicação
5. Copie o `AccessToken` e `PublicKey` da seção de credenciais
6. Configure o webhook em "Webhooks" > "Configurar notificações"

---

## 3. Uso da Feature Flag

### 3.1 Habilitar/Desabilitar Pagamentos por Cartão

A feature flag `CreditCardPayments` pode ser gerenciada por clínica:

#### Via API

**Habilitar:**
```bash
PUT /api/businessconfiguration/{clinicId}/feature
{
  "featureName": "CreditCardPayments",
  "enabled": true
}
```

**Desabilitar:**
```bash
PUT /api/businessconfiguration/{clinicId}/feature
{
  "featureName": "CreditCardPayments",
  "enabled": false
}
```

**Verificar Status:**
```bash
GET /api/businessconfiguration/{clinicId}
```

#### Via Código

```csharp
// Verificar se pagamentos por cartão estão habilitados
var config = await _businessConfigurationRepository.GetByClinicIdAsync(clinicId);
bool canProcessCard = config.IsFeatureEnabled("CreditCardPayments");

// Habilitar pagamentos por cartão
config.EnableFeature("CreditCardPayments");
await _businessConfigurationRepository.UpdateAsync(config);

// Desabilitar pagamentos por cartão
config.DisableFeature("CreditCardPayments");
await _businessConfigurationRepository.UpdateAsync(config);
```

### 3.2 Valor Padrão

Por padrão, a feature `CreditCardPayments` é habilitada (`true`) para todas as novas clínicas. Isso pode ser alterado no método `SetDefaultFeatures()` da classe `BusinessConfiguration`.

---

## 4. Interface do Serviço

### 4.1 IPaymentGatewayService

O serviço de gateway de pagamento fornece os seguintes métodos:

```csharp
public interface IPaymentGatewayService
{
    // Criar pagamento de assinatura
    Task<PaymentGatewayResult> CreateSubscriptionPaymentAsync(
        string customerId,
        string customerEmail,
        decimal amount,
        string planName,
        string tenantId,
        CancellationToken cancellationToken = default);

    // Criar pagamento de consulta
    Task<PaymentGatewayResult> CreateAppointmentPaymentAsync(
        string customerId,
        string customerEmail,
        decimal amount,
        string description,
        string tenantId,
        CancellationToken cancellationToken = default);

    // Processar notificação webhook
    Task<bool> ProcessWebhookNotificationAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default);

    // Obter status do pagamento
    Task<PaymentGatewayStatus> GetPaymentStatusAsync(
        string transactionId,
        CancellationToken cancellationToken = default);

    // Reembolsar pagamento
    Task<PaymentGatewayResult> RefundPaymentAsync(
        string transactionId,
        decimal? amount,
        string reason,
        CancellationToken cancellationToken = default);

    // Verificar se está configurado
    bool IsConfigured();
}
```

### 4.2 Exemplo de Uso

```csharp
public class SubscriptionController : ControllerBase
{
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly IBusinessConfigurationRepository _configRepository;

    public SubscriptionController(
        IPaymentGatewayService paymentGateway,
        IBusinessConfigurationRepository configRepository)
    {
        _paymentGateway = paymentGateway;
        _configRepository = configRepository;
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        // Verificar se pagamentos por cartão estão habilitados
        var config = await _configRepository.GetByClinicIdAsync(request.ClinicId);
        if (!config.IsFeatureEnabled("CreditCardPayments"))
        {
            return BadRequest("Pagamentos por cartão não estão habilitados para esta clínica");
        }

        // Verificar se o gateway está configurado
        if (!_paymentGateway.IsConfigured())
        {
            return ServiceUnavailable("Gateway de pagamento não configurado");
        }

        // Criar pagamento
        var result = await _paymentGateway.CreateSubscriptionPaymentAsync(
            request.CustomerId,
            request.Email,
            request.Amount,
            request.PlanName,
            request.TenantId);

        if (result.Success)
        {
            return Ok(new { 
                paymentUrl = result.PaymentUrl,
                transactionId = result.TransactionId
            });
        }

        return BadRequest(result.ErrorMessage);
    }
}
```

---

## 5. Migração de Banco de Dados

### 5.1 Aplicar Migração

A migração `20260202172100_AddCreditCardPaymentsFeatureFlag` adiciona a coluna `CreditCardPayments` à tabela `BusinessConfigurations`.

**Aplicar via Entity Framework:**
```bash
cd src/MedicSoft.Api
dotnet ef database update
```

**Aplicar manualmente (PostgreSQL):**
```sql
ALTER TABLE "BusinessConfigurations"
ADD COLUMN "CreditCardPayments" boolean NOT NULL DEFAULT true;
```

---

## 6. Webhooks

### 6.1 Endpoint de Webhook

O sistema deve expor um endpoint para receber notificações do Mercado Pago:

```
POST /api/webhooks/mercadopago
```

### 6.2 Configuração no Mercado Pago

1. Acesse o [Dashboard do Mercado Pago](https://www.mercadopago.com.br/developers/panel)
2. Vá para "Suas integrações" > Sua aplicação > "Webhooks"
3. Configure o URL: `https://seu-dominio.com/api/webhooks/mercadopago`
4. Selecione os eventos:
   - `payment` (pagamento criado/atualizado)
   - `subscription` (assinatura criada/atualizada)

---

## 7. Estados de Pagamento

### 7.1 Mapeamento de Status

| Mercado Pago | PrimeCare Gateway | Descrição |
|--------------|-------------------|-----------|
| `pending` | `Pending` | Aguardando pagamento |
| `in_process` | `Processing` | Pagamento em processamento |
| `approved` | `Approved` | Pagamento aprovado |
| `rejected` | `Rejected` | Pagamento rejeitado |
| `cancelled` | `Cancelled` | Pagamento cancelado |
| `refunded` | `Refunded` | Pagamento reembolsado |
| `charged_back` | `ChargedBack` | Estorno realizado |

---

## 8. Segurança

### 8.1 Validação de Webhooks

**IMPORTANTE:** Sempre valide a assinatura dos webhooks recebidos para garantir que vieram do Mercado Pago:

```csharp
public bool ValidateWebhookSignature(string payload, string signature, string secret)
{
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    var computedSignature = Convert.ToBase64String(hash);
    return signature == computedSignature;
}
```

### 8.2 Armazenamento de Credenciais

- **NUNCA** commite credenciais no código fonte
- Use variáveis de ambiente ou Azure Key Vault em produção
- Use User Secrets em desenvolvimento

```bash
# Configurar User Secrets
dotnet user-secrets set "PaymentGateway:MercadoPago:AccessToken" "YOUR_TOKEN"
dotnet user-secrets set "PaymentGateway:MercadoPago:PublicKey" "YOUR_PUBLIC_KEY"
```

---

## 9. Testes

### 9.1 Credenciais de Teste

O Mercado Pago fornece credenciais de teste para desenvolvimento:

1. Acesse o [Dashboard](https://www.mercadopago.com.br/developers/panel)
2. Alterne para "Modo de teste" no canto superior direito
3. Use as credenciais de teste para validar a integração

### 9.2 Cartões de Teste

Use estes cartões para testar diferentes cenários:

| Cartão | Resultado |
|--------|-----------|
| `5031 4332 1540 6351` | Aprovado |
| `5031 7557 3453 0604` | Rejeitado |
| `4235 6477 2802 5682` | Aprovado (Visa) |

---

## 10. Próximos Passos

### 10.1 Pendências de Implementação

Após configurar as credenciais do Mercado Pago, os seguintes métodos precisam ser implementados:

1. ✅ Configuração básica
2. ✅ Interface do serviço
3. ✅ Feature flag
4. ⏳ Criar preferência de pagamento (Mercado Pago SDK)
5. ⏳ Processar webhooks
6. ⏳ Consultar status de pagamento
7. ⏳ Processar reembolsos
8. ⏳ Testes de integração

### 10.2 Dependências Necessárias

Adicione o SDK do Mercado Pago ao projeto:

```bash
cd src/MedicSoft.Application
dotnet add package MercadoPagoCore
```

---

## 11. Logs e Monitoramento

### 11.1 Logs Importantes

O serviço de pagamento gera logs para:

- Criação de pagamentos
- Processamento de webhooks
- Erros e exceções
- Status de configuração

### 11.2 Verificar Configuração

Para verificar se o gateway está configurado:

```csharp
var isConfigured = _paymentGateway.IsConfigured();
if (!isConfigured)
{
    _logger.LogWarning("Payment gateway is not configured");
}
```

---

## 12. Referências

- [Documentação Mercado Pago](https://www.mercadopago.com.br/developers/pt)
- [SDK .NET Mercado Pago](https://github.com/mercadopago/sdk-dotnet)
- [Guia de Webhooks](https://www.mercadopago.com.br/developers/pt/docs/your-integrations/notifications/webhooks)
- [API Reference](https://www.mercadopago.com.br/developers/pt/reference)

---

## 13. Suporte

Para questões sobre a implementação:

- **Técnicas**: Time de desenvolvimento PrimeCare
- **Mercado Pago**: [Suporte Mercado Pago](https://www.mercadopago.com.br/developers/pt/support)
- **Documentação**: Este arquivo e `GUIA_IMPLEMENTACAO_PLANO_FINANCEIRO.md`

---

**Última Atualização:** 02/02/2026  
**Versão:** 1.0  
**Status:** Implementado - Aguardando Credenciais
