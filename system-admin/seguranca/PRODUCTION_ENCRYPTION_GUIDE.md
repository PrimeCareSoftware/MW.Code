# 🔐 Guia de Produção - Criptografia de Dados Médicos

## 📋 Visão Geral

Este guia contém todas as informações necessárias para implantar e gerenciar a criptografia de dados médicos em ambiente de produção, garantindo conformidade com a LGPD e segurança máxima.

## 🎯 Objetivo

Fornecer um guia passo a passo para:
- Configurar Azure Key Vault para gestão segura de chaves
- Implantar a solução de criptografia em produção
- Gerenciar chaves e rotação
- Realizar disaster recovery
- Monitorar e auditar o sistema de criptografia

## 📦 Pré-requisitos

### Ambiente Azure
- ✅ Subscription Azure ativa
- ✅ Resource Group criado
- ✅ Permissões de administrador no Azure
- ✅ Azure CLI instalado

### Aplicação
- ✅ Sistema PrimeCare implantado
- ✅ Backup completo do banco de dados
- ✅ Todos os testes passando
- ✅ Janela de manutenção agendada

## 🔧 Parte 1: Configuração do Azure Key Vault

### 1.1 Criar Resource Group (se não existir)

```bash
# Login no Azure
az login

# Criar Resource Group
az group create \
  --name primecare-prod-rg \
  --location brazilsouth \
  --tags Environment=Production Application=PrimeCare
```

### 1.2 Criar Azure Key Vault

```bash
# Criar Key Vault com proteção avançada
az keyvault create \
  --name primecare-prod-kv \
  --resource-group primecare-prod-rg \
  --location brazilsouth \
  --enable-soft-delete true \
  --soft-delete-retention-days 90 \
  --enable-purge-protection true \
  --enable-rbac-authorization false \
  --sku premium \
  --tags Environment=Production Application=PrimeCare Purpose=MedicalDataEncryption

# Verificar criação
az keyvault show \
  --name primecare-prod-kv \
  --resource-group primecare-prod-rg
```

**Explicação dos parâmetros:**
- `--enable-soft-delete`: Proteção contra exclusão acidental (90 dias de recuperação)
- `--enable-purge-protection`: Impede exclusão permanente durante o período de retenção
- `--sku premium`: SKU Premium com HSM backing para chaves mais seguras
- `--enable-rbac-authorization false`: Usa políticas de acesso tradicionais (mais simples)

### 1.3 Criar Chave de Criptografia

```bash
# Opção 1: Chave protegida por software (mais barato)
az keyvault key create \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key \
  --protection software \
  --kty RSA \
  --size 2048 \
  --ops encrypt decrypt wrapKey unwrapKey

# Opção 2: Chave protegida por HSM (mais seguro, recomendado para produção)
az keyvault key create \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key \
  --protection hsm \
  --kty RSA-HSM \
  --size 2048 \
  --ops encrypt decrypt wrapKey unwrapKey

# Verificar chave
az keyvault key show \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key
```

### 1.4 Configurar Rotação Automática de Chaves

```bash
# Criar política de rotação (365 dias)
az keyvault key rotation-policy update \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key \
  --value '{
    "lifetimeActions": [
      {
        "trigger": {
          "timeAfterCreate": "P365D"
        },
        "action": {
          "type": "Rotate"
        }
      },
      {
        "trigger": {
          "timeBeforeExpiry": "P30D"
        },
        "action": {
          "type": "Notify"
        }
      }
    ],
    "attributes": {
      "expiryTime": "P730D"
    }
  }'

# Verificar política de rotação
az keyvault key rotation-policy show \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key
```

## 🔐 Parte 2: Configuração de Managed Identity

### 2.1 Habilitar Managed Identity no App Service

```bash
# Obter nome do App Service
APP_SERVICE_NAME="primecare-prod-api"

# Habilitar System-Assigned Managed Identity
az webapp identity assign \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg

# Obter Principal ID da Managed Identity
PRINCIPAL_ID=$(az webapp identity show \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg \
  --query principalId \
  --output tsv)

echo "Managed Identity Principal ID: $PRINCIPAL_ID"
```

### 2.2 Conceder Permissões ao Managed Identity

```bash
# Dar permissões de acesso às chaves
az keyvault set-policy \
  --name primecare-prod-kv \
  --object-id $PRINCIPAL_ID \
  --key-permissions get list encrypt decrypt wrapKey unwrapKey \
  --secret-permissions get list

# Verificar permissões
az keyvault show \
  --name primecare-prod-kv \
  --query "properties.accessPolicies[?objectId=='$PRINCIPAL_ID']"
```

## ⚙️ Parte 3: Configuração da Aplicação

### 3.1 Configurar appsettings.json para Produção

Não adicione secrets no appsettings.json! Use App Configuration ou Environment Variables:

```json
{
  "Azure": {
    "KeyVault": {
      "VaultUri": "https://primecare-prod-kv.vault.azure.net/",
      "KeyName": "medical-data-encryption-key",
      "UseManagedIdentity": true,
      "CacheExpirationMinutes": 60
    }
  },
  "Encryption": {
    "Enabled": true,
    "PerformanceBudgetMs": 10,
    "EnableAuditLogging": true
  }
}
```

### 3.2 Configurar via Azure App Configuration

```bash
# Criar App Configuration (se não existir)
az appconfig create \
  --name primecare-prod-config \
  --resource-group primecare-prod-rg \
  --location brazilsouth \
  --sku Standard

# Adicionar configurações
az appconfig kv set \
  --name primecare-prod-config \
  --key "Azure:KeyVault:VaultUri" \
  --value "https://primecare-prod-kv.vault.azure.net/" \
  --yes

az appconfig kv set \
  --name primecare-prod-config \
  --key "Azure:KeyVault:KeyName" \
  --value "medical-data-encryption-key" \
  --yes

# Conectar App Service ao App Configuration
az webapp config appsettings set \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg \
  --settings "AzureAppConfiguration__Endpoint=https://primecare-prod-config.azconfig.io"
```

### 3.3 Configurar via Environment Variables (Alternativa)

```bash
# Configurar variáveis de ambiente no App Service
az webapp config appsettings set \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg \
  --settings \
    "Azure__KeyVault__VaultUri=https://primecare-prod-kv.vault.azure.net/" \
    "Azure__KeyVault__KeyName=medical-data-encryption-key" \
    "Azure__KeyVault__UseManagedIdentity=true" \
    "Encryption__Enabled=true"
```

## 📊 Parte 4: Migração de Dados Existentes

### 4.1 Backup Completo

```bash
# CRÍTICO: Fazer backup COMPLETO antes de qualquer migração
# PostgreSQL
pg_dump -h HOST -U USER -d DATABASE -F c -f backup_pre_encryption_$(date +%Y%m%d_%H%M%S).dump

# SQL Server
sqlcmd -S SERVER -d DATABASE -Q "BACKUP DATABASE [DATABASE] TO DISK='C:\Backups\pre_encryption_$(date +%Y%m%d_%H%M%S).bak'"
```

### 4.2 Executar Migração

```bash
# 1. Colocar aplicação em modo de manutenção
az webapp stop --name $APP_SERVICE_NAME --resource-group primecare-prod-rg

# 2. Executar script de migração de dados
# (O script deve ser executado via console do App Service ou localmente com VPN)
dotnet run --project tools/EncryptExistingData/EncryptExistingData.csproj \
  --connection-string "PROD_CONNECTION_STRING" \
  --batch-size 1000 \
  --dry-run false

# 3. Verificar logs de migração
tail -f /home/LogFiles/migration.log

# 4. Validar dados criptografados
dotnet run --project tools/ValidateEncryption/ValidateEncryption.csproj

# 5. Reativar aplicação
az webapp start --name $APP_SERVICE_NAME --resource-group primecare-prod-rg
```

### 4.3 Script de Migração (C#)

Criar projeto console `tools/EncryptExistingData/`:

```csharp
// Program.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MedicSoft.Repository.Context;
using MedicSoft.CrossCutting.Security;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        var connectionString = config["connection-string"];
        var batchSize = int.Parse(config["batch-size"] ?? "1000");
        var dryRun = bool.Parse(config["dry-run"] ?? "true");

        Console.WriteLine($"Starting data migration...");
        Console.WriteLine($"Batch size: {batchSize}");
        Console.WriteLine($"Dry run: {dryRun}");

        // Configurar DbContext
        var optionsBuilder = new DbContextOptionsBuilder<MedicSoftDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Criar serviço de criptografia
        var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
        var encryptionService = new DataEncryptionService(encryptionKey!);

        using var context = new MedicSoftDbContext(optionsBuilder.Options, null, encryptionService);

        // Migrar pacientes
        await MigratePatientsAsync(context, encryptionService, batchSize, dryRun);

        // Migrar prontuários
        await MigrateMedicalRecordsAsync(context, encryptionService, batchSize, dryRun);

        // Migrar prescrições
        await MigratePrescriptionsAsync(context, encryptionService, batchSize, dryRun);

        Console.WriteLine("Migration completed successfully!");
    }

    static async Task MigratePatientsAsync(
        MedicSoftDbContext context,
        DataEncryptionService encryptionService,
        int batchSize,
        bool dryRun)
    {
        Console.WriteLine("Migrating patients...");
        var total = await context.Patients.CountAsync();
        var processed = 0;

        while (processed < total)
        {
            var patients = await context.Patients
                .OrderBy(p => p.Id)
                .Skip(processed)
                .Take(batchSize)
                .ToListAsync();

            foreach (var patient in patients)
            {
                // Verificar se já está criptografado
                if (!string.IsNullOrEmpty(patient.MedicalHistory) && !IsEncrypted(patient.MedicalHistory))
                {
                    Console.WriteLine($"Encrypting MedicalHistory for patient {patient.Id}");
                    if (!dryRun)
                    {
                        patient.MedicalHistory = encryptionService.Encrypt(patient.MedicalHistory);
                    }
                }

                if (!string.IsNullOrEmpty(patient.Allergies) && !IsEncrypted(patient.Allergies))
                {
                    Console.WriteLine($"Encrypting Allergies for patient {patient.Id}");
                    if (!dryRun)
                    {
                        patient.Allergies = encryptionService.Encrypt(patient.Allergies);
                    }
                }
            }

            if (!dryRun)
            {
                await context.SaveChangesAsync();
            }

            processed += patients.Count;
            Console.WriteLine($"Progress: {processed}/{total} ({processed * 100 / total}%)");
        }
    }

    static bool IsEncrypted(string value)
    {
        try
        {
            // Dados criptografados são Base64 válidos
            Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Implementar MigrateMedicalRecordsAsync e MigratePrescriptionsAsync de forma similar
}
```

## 🔍 Parte 5: Monitoramento e Auditoria

### 5.1 Configurar Application Insights

```bash
# Criar Application Insights
az monitor app-insights component create \
  --app primecare-prod-insights \
  --location brazilsouth \
  --resource-group primecare-prod-rg \
  --application-type web

# Obter Instrumentation Key
INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app primecare-prod-insights \
  --resource-group primecare-prod-rg \
  --query instrumentationKey \
  --output tsv)

# Configurar no App Service
az webapp config appsettings set \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg \
  --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$INSTRUMENTATION_KEY"
```

### 5.2 Configurar Alertas de Key Vault

```bash
# Criar Action Group para notificações
az monitor action-group create \
  --name primecare-security-alerts \
  --resource-group primecare-prod-rg \
  --short-name SecAlerts \
  --email-receiver name=SecurityTeam email=security@primecare.com

# Alerta para tentativas de acesso negadas
az monitor metrics alert create \
  --name kv-access-denied \
  --resource-group primecare-prod-rg \
  --scopes /subscriptions/SUBSCRIPTION_ID/resourceGroups/primecare-prod-rg/providers/Microsoft.KeyVault/vaults/primecare-prod-kv \
  --condition "count ServiceApiResult where ResultType == 'Unauthorized' > 5" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action primecare-security-alerts

# Alerta para uso excessivo de chaves (possível vazamento)
az monitor metrics alert create \
  --name kv-excessive-usage \
  --resource-group primecare-prod-rg \
  --scopes /subscriptions/SUBSCRIPTION_ID/resourceGroups/primecare-prod-rg/providers/Microsoft.KeyVault/vaults/primecare-prod-kv \
  --condition "count ServiceApiHit > 10000" \
  --window-size 1h \
  --evaluation-frequency 5m \
  --action primecare-security-alerts
```

### 5.3 Habilitar Diagnostic Logs do Key Vault

```bash
# Criar Log Analytics Workspace
az monitor log-analytics workspace create \
  --resource-group primecare-prod-rg \
  --workspace-name primecare-prod-logs

# Habilitar logs de diagnóstico
az monitor diagnostic-settings create \
  --name kv-diagnostics \
  --resource /subscriptions/SUBSCRIPTION_ID/resourceGroups/primecare-prod-rg/providers/Microsoft.KeyVault/vaults/primecare-prod-kv \
  --logs '[{"category": "AuditEvent", "enabled": true}]' \
  --metrics '[{"category": "AllMetrics", "enabled": true}]' \
  --workspace /subscriptions/SUBSCRIPTION_ID/resourceGroups/primecare-prod-rg/providers/Microsoft.OperationalInsights/workspaces/primecare-prod-logs
```

## 🚨 Parte 6: Disaster Recovery

### 6.1 Backup de Chaves

```bash
# Backup da chave (guarde em local MUITO seguro)
az keyvault key backup \
  --vault-name primecare-prod-kv \
  --name medical-data-encryption-key \
  --file medical-data-encryption-key.backup

# Armazenar backup em Azure Blob Storage (criptografado)
az storage blob upload \
  --account-name primecarebackups \
  --container-name key-backups \
  --name medical-data-encryption-key_$(date +%Y%m%d).backup \
  --file medical-data-encryption-key.backup \
  --encryption-scope key-backup-scope
```

### 6.2 Procedimento de Recuperação

```bash
# Em caso de perda do Key Vault, criar novo
az keyvault create \
  --name primecare-prod-kv-recovery \
  --resource-group primecare-prod-rg \
  --location brazilsouth \
  --enable-soft-delete true \
  --enable-purge-protection true

# Restaurar chave do backup
az keyvault key restore \
  --vault-name primecare-prod-kv-recovery \
  --file medical-data-encryption-key.backup

# Atualizar configuração da aplicação para novo Key Vault
az webapp config appsettings set \
  --name $APP_SERVICE_NAME \
  --resource-group primecare-prod-rg \
  --settings "Azure__KeyVault__VaultUri=https://primecare-prod-kv-recovery.vault.azure.net/"
```

### 6.3 Teste de Disaster Recovery (Quarterly)

```bash
# Executar em ambiente de staging
# 1. Simular perda do Key Vault
# 2. Restaurar do backup
# 3. Verificar que aplicação consegue descriptografar dados
# 4. Documentar tempo de recuperação (RTO/RPO)
```

## 📋 Parte 7: Checklist de Implantação

### Pré-Implantação
- [ ] Backup completo do banco de dados realizado
- [ ] Azure Key Vault configurado e testado
- [ ] Managed Identity habilitada e com permissões
- [ ] Configurações de produção validadas
- [ ] Script de migração testado em staging
- [ ] Janela de manutenção agendada
- [ ] Equipe de plantão escalada
- [ ] Plano de rollback documentado

### Durante Implantação
- [ ] Aplicação em modo de manutenção
- [ ] Migração de dados executada com sucesso
- [ ] Validação de dados criptografados
- [ ] Testes de fumaça passando
- [ ] Performance dentro do esperado (<10% overhead)
- [ ] Logs sem erros críticos

### Pós-Implantação
- [ ] Aplicação funcionando normalmente
- [ ] Monitoramento configurado e ativo
- [ ] Alertas de segurança funcionando
- [ ] Backup de chaves realizado
- [ ] Documentação atualizada
- [ ] Treinamento da equipe de suporte
- [ ] Auditoria de segurança agendada (30 dias)

## 📞 Contatos de Emergência

### Equipe de Segurança
- **Email**: security@primecare.com
- **Telefone Plantão**: +55 (11) 99999-9999
- **Slack**: #security-incidents

### Suporte Azure
- **Portal**: https://portal.azure.com
- **Suporte**: Criar ticket no portal
- **Documentação**: https://docs.microsoft.com/azure/key-vault/

## 📚 Referências

- [Azure Key Vault Best Practices](https://docs.microsoft.com/azure/key-vault/general/best-practices)
- [LGPD Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [NIST Cryptographic Standards](https://csrc.nist.gov/publications/fips)
- [OWASP Cryptographic Storage](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

---

**Versão**: 1.0  
**Última Atualização**: Janeiro 2026  
**Responsável**: Equipe de Segurança - PrimeCare Software
