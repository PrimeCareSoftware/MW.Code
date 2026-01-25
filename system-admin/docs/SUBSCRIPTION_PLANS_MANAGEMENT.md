# Gerenciamento de Planos de Assinatura

## Visão Geral

Os planos de assinatura exibidos no site são agora carregados dinamicamente do banco de dados através da API. Isso permite que você modifique os planos sem precisar alterar o código do frontend.

## Como Funciona

1. **API Endpoint**: `/api/registration/plans` (público, sem autenticação)
2. **Fonte de Dados**: Tabela `SubscriptionPlans` no banco de dados
3. **Filtro**: Retorna apenas planos ativos (`IsActive = true`)

## Estrutura dos Planos

Cada plano possui as seguintes propriedades:

- **Id**: Identificador único (GUID)
- **Name**: Nome do plano (ex: "Básico", "Premium")
- **Description**: Descrição do plano
- **MonthlyPrice**: Preço mensal em decimal
- **TrialDays**: Dias de período de teste gratuito
- **MaxUsers**: Número máximo de usuários
- **MaxPatients**: Número máximo de pacientes
- **HasReports**: Indica se tem relatórios
- **HasWhatsAppIntegration**: Indica se tem integração WhatsApp
- **HasSMSNotifications**: Indica se tem notificações SMS
- **HasTissExport**: Indica se tem exportação TISS
- **IsActive**: Indica se o plano está ativo
- **Type**: Tipo do plano (0=Trial, 1=Basic, 2=Standard, 3=Premium, 4=Enterprise)

## Gerando Features Automaticamente

A lista de features (recursos) é gerada automaticamente pela API com base nas propriedades do plano:

### Features Base (todos os planos)
- Informação sobre limite de usuários
- Informação sobre limite de pacientes
- "Agenda de consultas"
- "Cadastro de pacientes"
- "Prontuário médico digital"

### Features Adicionais por Tipo

**Trial (Type = 0):**
- "Suporte por email"

**Basic (Type = 1):**
- "Relatórios básicos" (se HasReports = true)
- "Lembretes de consulta" (se HasSMSNotifications = true)
- "Suporte por email"

**Standard (Type = 2):**
- "Relatórios gerenciais" (se HasReports = true)
- "Integração WhatsApp" (se HasWhatsAppIntegration = true)
- "Lembretes de consulta" (se HasSMSNotifications = true)
- "Suporte prioritário"

**Premium (Type = 3):**
- "Relatórios gerenciais" (se HasReports = true)
- "Integração WhatsApp" (se HasWhatsAppIntegration = true)
- "Notificações por SMS" (se HasSMSNotifications = true)
- "Exportação TISS" (se HasTissExport = true)
- "Dashboard avançado"
- "API de integração"
- "Suporte 24/7"

**Enterprise (Type = 4):**
- "Todos os recursos Premium" (se HasReports = true)
- "Desenvolvimento de funcionalidades específicas"
- "Treinamento personalizado"
- "Gerente de conta dedicado"
- "SLA garantido"

## Como Modificar Planos

### Via SQL (Direto no Banco de Dados)

```sql
-- Exemplo: Atualizar preço do plano Standard
UPDATE SubscriptionPlans
SET MonthlyPrice = 249.90,
    UpdatedAt = GETDATE()
WHERE Name = 'Standard' AND TenantId = 'system';

-- Exemplo: Ativar/Desativar um plano
UPDATE SubscriptionPlans
SET IsActive = 0,  -- 0 para desativar, 1 para ativar
    UpdatedAt = GETDATE()
WHERE Name = 'Premium' AND TenantId = 'system';

-- Exemplo: Adicionar novo recurso (modificando propriedades)
UPDATE SubscriptionPlans
SET HasWhatsAppIntegration = 1,
    UpdatedAt = GETDATE()
WHERE Name = 'Básico' AND TenantId = 'system';

-- Exemplo: Inserir novo plano
INSERT INTO SubscriptionPlans (
    Id, Name, Description, MonthlyPrice, TrialDays, MaxUsers, MaxPatients,
    HasReports, HasWhatsAppIntegration, HasSMSNotifications, HasTissExport,
    IsActive, Type, TenantId, CreatedAt
)
VALUES (
    NEWID(),
    'Plano Especial',
    'Plano especial com funcionalidades customizadas',
    179.90,
    15,
    4,
    500,
    1, -- HasReports
    1, -- HasWhatsAppIntegration
    0, -- HasSMSNotifications
    0, -- HasTissExport
    1, -- IsActive
    2, -- Type (Standard)
    'system',
    GETDATE()
);
```

### Via Seeder (Para Ambiente de Desenvolvimento)

Os planos são automaticamente criados quando você executa o seeder de dados de demonstração:

```bash
# Via API endpoint (em desenvolvimento)
POST /api/dev/seed-data
```

Ou você pode modificar o arquivo `DataSeederService.cs` no método `CreateDemoSubscriptionPlans()` para personalizar os planos padrão.

## Testando Alterações

Após modificar os planos no banco de dados:

1. Recarregue a página de preços no site
2. Os novos valores devem aparecer automaticamente
3. Não é necessário rebuild ou redeploy do frontend

## Importante

- **TenantId**: Os planos do site devem ter `TenantId = 'system'`
- **IsActive**: Apenas planos com `IsActive = true` aparecem no site
- **Type**: Defina o tipo correto para garantir que as features sejam geradas apropriadamente
- **Recomendação**: O plano com `Type = 2` (Standard) é marcado como recomendado no site

## Fallback

Se a API não estiver disponível ou ocorrer um erro, o frontend usa automaticamente os planos hardcoded como fallback, garantindo que o site continue funcionando.

## Exemplos de Casos de Uso

### Promoção Temporária
```sql
-- Reduzir preço temporariamente
UPDATE SubscriptionPlans
SET MonthlyPrice = MonthlyPrice * 0.8,  -- 20% de desconto
    Description = Description + ' - PROMOÇÃO LIMITADA',
    UpdatedAt = GETDATE()
WHERE Type IN (1, 2, 3) AND TenantId = 'system';
```

### Lançar Novo Plano
```sql
-- Criar plano intermediário entre Basic e Standard
INSERT INTO SubscriptionPlans (...)
VALUES (...);
```

### Desativar Plano Temporariamente
```sql
-- Desativar plano para manutenção
UPDATE SubscriptionPlans
SET IsActive = 0,
    UpdatedAt = GETDATE()
WHERE Name = 'Premium' AND TenantId = 'system';
```

## Monitoramento

Para verificar os planos atualmente ativos:

```sql
SELECT 
    Name, 
    MonthlyPrice, 
    Type, 
    MaxUsers, 
    MaxPatients,
    IsActive
FROM SubscriptionPlans
WHERE TenantId = 'system'
ORDER BY MonthlyPrice;
```
