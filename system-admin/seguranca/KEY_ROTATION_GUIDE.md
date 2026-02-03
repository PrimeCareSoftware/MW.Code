# 🔄 Guia de Rotação de Chaves de Criptografia

## 📋 Visão Geral

A rotação de chaves de criptografia é uma prática de segurança essencial que reduz o risco de comprometimento de chaves e garante conformidade com políticas de segurança e regulamentações como LGPD.

## 🎯 Objetivos da Rotação de Chaves

1. **Reduzir janela de exposição**: Limitar a quantidade de dados criptografados com uma única chave
2. **Compliance**: Atender requisitos regulatórios (LGPD, ISO 27001)
3. **Mitigar comprometimento**: Reduzir impacto caso uma chave seja comprometida
4. **Best practice**: Seguir padrões de segurança da indústria

## 📅 Política de Rotação

### Frequência Recomendada

| Tipo de Chave | Frequência | Justificativa |
|---------------|------------|---------------|
| **KEK** (Key Encryption Key) | 12 meses | Chave master no Azure Key Vault |
| **DEK** (Data Encryption Key) | 90 dias | Chave usada diretamente para dados |
| **Emergency Rotation** | Imediato | Em caso de suspeita de comprometimento |

### Nossa Configuração Atual

- **KEK no Azure Key Vault**: Rotação automática a cada 365 dias
- **DEK em cache**: Rotação a cada 60 minutos (via cache expiration)

## 🔧 Tipos de Rotação

### 1. Rotação Automática (Recomendado)

O Azure Key Vault gerencia automaticamente a rotação da KEK.

**Vantagens:**
- ✅ Sem downtime
- ✅ Sem intervenção manual
- ✅ Auditoria automática
- ✅ Rollback fácil

**Como funciona:**
1. Azure Key Vault cria nova versão da chave
2. Aplicação continua usando chave antiga para descriptografar dados antigos
3. Novos dados são criptografados com chave nova
4. Chaves antigas permanecem disponíveis para descriptografia

### 2. Rotação Manual

Necessária quando há suspeita de comprometimento ou mudança de ambiente.

**Quando usar:**
- 🚨 Chave possivelmente comprometida
- 🔄 Migração de ambiente (dev → prod)
- 📋 Auditoria de segurança recomendou
- 🆕 Upgrade de algoritmo de criptografia

## 📝 Procedimento de Rotação Automática

### Passo 1: Verificar Configuração Atual

```bash
# Verificar política de rotação
az keyvault key rotation-policy show \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key

# Verificar versões da chave
az keyvault key list-versions \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key
```

### Passo 2: Confirmar Rotação Automática Habilitada

```bash
# Atualizar política se necessário
az keyvault key rotation-policy update \
  --vault-name omnicare-prod-kv \
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
```

### Passo 3: Monitorar Rotação

```bash
# Configurar alerta para notificação de rotação
az monitor activity-log alert create \
  --name key-rotation-alert \
  --resource-group omnicare-prod-rg \
  --condition category=Administrative and operationName=Microsoft.KeyVault/vaults/keys/rotate/action \
  --action omnicare-security-alerts
```

## 🔄 Procedimento de Rotação Manual de Emergência

### Cenário: Chave Possivelmente Comprometida

#### Fase 1: Análise e Preparação (0-2 horas)

```bash
# 1. Verificar logs de acesso suspeito
az monitor activity-log list \
  --resource-group omnicare-prod-rg \
  --start-time $(date -u -d '7 days ago' +"%Y-%m-%dT%H:%M:%SZ") \
  --end-time $(date -u +"%Y-%m-%dT%H:%M:%SZ") \
  --query "[?contains(operationName.value, 'KeyVault/vaults/keys')]" \
  --output table

# 2. Identificar IPs suspeitos
az monitor activity-log list \
  --resource-group omnicare-prod-rg \
  --query "[?contains(operationName.value, 'KeyVault') && claims.ipaddr != 'YOUR_ALLOWED_IPS'].{Time:eventTimestamp, IP:claims.ipaddr, Operation:operationName.localizedValue}" \
  --output table

# 3. Backup da chave atual (CRÍTICO)
az keyvault key backup \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key \
  --file medical-data-encryption-key_emergency_backup_$(date +%Y%m%d_%H%M%S).backup
```

#### Fase 2: Revogar Acesso (2-4 horas)

```bash
# 1. Revogar todas as access policies (exceto admin)
# Listar policies atuais
az keyvault show \
  --name omnicare-prod-kv \
  --query "properties.accessPolicies[].objectId" \
  --output tsv > current_policies.txt

# 2. Remover policies suspeitas
while read -r object_id; do
    if [ "$object_id" != "$ADMIN_PRINCIPAL_ID" ]; then
        echo "Revoking access for: $object_id"
        az keyvault delete-policy \
          --name omnicare-prod-kv \
          --object-id $object_id
    fi
done < current_policies.txt

# 3. Criar nova Managed Identity
az identity create \
  --name omnicare-prod-api-new-identity \
  --resource-group omnicare-prod-rg

NEW_PRINCIPAL_ID=$(az identity show \
  --name omnicare-prod-api-new-identity \
  --resource-group omnicare-prod-rg \
  --query principalId \
  --output tsv)
```

#### Fase 3: Criar Nova Chave (4-6 horas)

```bash
# 1. Desabilitar chave comprometida (NÃO deletar - precisamos descriptografar dados antigos)
az keyvault key set-attributes \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key \
  --enabled false

# 2. Criar nova chave com nome diferente
az keyvault key create \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key-v2 \
  --protection hsm \
  --kty RSA-HSM \
  --size 4096 \
  --ops encrypt decrypt wrapKey unwrapKey

# 3. Conceder permissões à nova Managed Identity
az keyvault set-policy \
  --name omnicare-prod-kv \
  --object-id $NEW_PRINCIPAL_ID \
  --key-permissions get list encrypt decrypt wrapKey unwrapKey \
  --secret-permissions get list
```

#### Fase 4: Atualizar Aplicação (6-8 horas)

```bash
# 1. Parar aplicação
az webapp stop --name omnicare-prod-api --resource-group omnicare-prod-rg

# 2. Atualizar Managed Identity
az webapp identity assign \
  --name omnicare-prod-api \
  --resource-group omnicare-prod-rg \
  --identities /subscriptions/SUBSCRIPTION_ID/resourcegroups/omnicare-prod-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/omnicare-prod-api-new-identity

# 3. Atualizar configuração
az webapp config appsettings set \
  --name omnicare-prod-api \
  --resource-group omnicare-prod-rg \
  --settings \
    "Azure__KeyVault__KeyName=medical-data-encryption-key-v2" \
    "Azure__KeyVault__OldKeyName=medical-data-encryption-key"

# 4. Reiniciar aplicação
az webapp start --name omnicare-prod-api --resource-group omnicare-prod-rg
```

#### Fase 5: Re-criptografar Dados (8-48 horas, depende do volume)

```bash
# Executar ferramenta de re-criptografia
# Esta ferramenta descriptografa com chave antiga e re-criptografa com chave nova

dotnet run --project tools/ReEncryptData/ReEncryptData.csproj \
  --old-key-name medical-data-encryption-key \
  --new-key-name medical-data-encryption-key-v2 \
  --batch-size 1000 \
  --parallel-threads 4 \
  --connection-string "PROD_CONNECTION_STRING"
```

#### Fase 6: Verificação e Limpeza (48-72 horas)

```bash
# 1. Verificar que todos os dados foram re-criptografados
dotnet run --project tools/ValidateEncryption/ValidateEncryption.csproj \
  --verify-key medical-data-encryption-key-v2

# 2. Após confirmação (30 dias), deletar chave antiga
az keyvault key delete \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key

# 3. Purge após período de soft-delete (90 dias)
az keyvault key purge \
  --vault-name omnicare-prod-kv \
  --name medical-data-encryption-key
```

## 🔍 Ferramenta de Re-criptografia

### Criar Projeto: `tools/ReEncryptData/`

```csharp
// ReEncryptData/Program.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        var oldKeyName = config["old-key-name"];
        var newKeyName = config["new-key-name"];
        var batchSize = int.Parse(config["batch-size"] ?? "1000");
        var parallelThreads = int.Parse(config["parallel-threads"] ?? "4");
        var connectionString = config["connection-string"];

        Console.WriteLine("=== Data Re-encryption Tool ===");
        Console.WriteLine($"Old key: {oldKeyName}");
        Console.WriteLine($"New key: {newKeyName}");
        Console.WriteLine($"Batch size: {batchSize}");
        Console.WriteLine($"Parallel threads: {parallelThreads}");
        Console.WriteLine();

        // Criar serviços de criptografia para ambas as chaves
        var oldEncryptionService = CreateEncryptionService(oldKeyName);
        var newEncryptionService = CreateEncryptionService(newKeyName);

        var optionsBuilder = new DbContextOptionsBuilder<MedicSoftDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        using var context = new MedicSoftDbContext(optionsBuilder.Options, null, null);

        // Re-criptografar cada entidade
        await ReEncryptPatientsAsync(context, oldEncryptionService, newEncryptionService, batchSize);
        await ReEncryptMedicalRecordsAsync(context, oldEncryptionService, newEncryptionService, batchSize);
        await ReEncryptPrescriptionsAsync(context, oldEncryptionService, newEncryptionService, batchSize);

        Console.WriteLine("\nRe-encryption completed successfully!");
    }

    static DataEncryptionService CreateEncryptionService(string keyName)
    {
        // Aqui você implementaria a lógica para obter a chave do Azure Key Vault
        // Por simplicidade, usando variável de ambiente
        var keyValue = Environment.GetEnvironmentVariable($"ENCRYPTION_KEY_{keyName.ToUpper().Replace("-", "_")}");
        return new DataEncryptionService(keyValue!);
    }

    static async Task ReEncryptPatientsAsync(
        MedicSoftDbContext context,
        DataEncryptionService oldService,
        DataEncryptionService newService,
        int batchSize)
    {
        Console.WriteLine("Re-encrypting patients...");
        var total = await context.Patients.CountAsync();
        var processed = 0;

        while (processed < total)
        {
            var patients = await context.Patients
                .OrderBy(p => p.Id)
                .Skip(processed)
                .Take(batchSize)
                .AsNoTracking()
                .ToListAsync();

            foreach (var patient in patients)
            {
                try
                {
                    // Re-criptografar MedicalHistory
                    if (!string.IsNullOrEmpty(patient.MedicalHistory))
                    {
                        var decrypted = oldService.Decrypt(patient.MedicalHistory);
                        var reEncrypted = newService.Encrypt(decrypted);
                        
                        await context.Database.ExecuteSqlRawAsync(
                            @"UPDATE ""Patients"" SET ""MedicalHistory"" = {0} WHERE ""Id"" = {1}",
                            reEncrypted, patient.Id);
                    }

                    // Re-criptografar Allergies
                    if (!string.IsNullOrEmpty(patient.Allergies))
                    {
                        var decrypted = oldService.Decrypt(patient.Allergies);
                        var reEncrypted = newService.Encrypt(decrypted);
                        
                        await context.Database.ExecuteSqlRawAsync(
                            @"UPDATE ""Patients"" SET ""Allergies"" = {0} WHERE ""Id"" = {1}",
                            reEncrypted, patient.Id);
                    }

                    processed++;
                    
                    if (processed % 100 == 0)
                    {
                        Console.WriteLine($"Progress: {processed}/{total} ({processed * 100 / total}%)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR processing patient {patient.Id}: {ex.Message}");
                    // Continuar com próximo
                }
            }
        }

        Console.WriteLine($"Completed patients: {processed}/{total}");
    }

    // Implementar ReEncryptMedicalRecordsAsync e ReEncryptPrescriptionsAsync de forma similar
}
```

## 📊 Monitoramento de Rotação

### Métricas para Acompanhar

1. **Duração da rotação**: Tempo total do processo
2. **Registros processados**: Número de registros re-criptografados
3. **Taxa de erro**: Erros durante re-criptografia
4. **Performance**: Impacto na aplicação durante rotação

### Dashboard no Application Insights

```kusto
// Query: Monitorar operações de criptografia
requests
| where name contains "Encrypt" or name contains "Decrypt"
| summarize 
    count(), 
    avg(duration), 
    percentiles(duration, 50, 95, 99)
  by bin(timestamp, 5m)
| render timechart

// Query: Detectar falhas de criptografia
exceptions
| where message contains "Cryptographic"
| summarize count() by bin(timestamp, 1h), outerMessage
| render timechart
```

## ✅ Checklist de Rotação

### Antes da Rotação
- [ ] Backup completo do banco de dados
- [ ] Backup da chave atual do Key Vault
- [ ] Verificar política de rotação configurada
- [ ] Testar em ambiente de staging
- [ ] Janela de manutenção agendada (se manual)
- [ ] Equipe de plantão escalada

### Durante a Rotação
- [ ] Monitorar logs de aplicação
- [ ] Verificar métricas de performance
- [ ] Acompanhar progresso da re-criptografia
- [ ] Validar que novos dados usam chave nova

### Após a Rotação
- [ ] Verificar que descriptografia funciona corretamente
- [ ] Validar integridade dos dados
- [ ] Documentar versão da chave ativa
- [ ] Atualizar runbook se necessário
- [ ] Agendar próxima rotação

## 🚨 Troubleshooting

### Problema: Descriptografia falhando após rotação

**Sintoma**: `CryptographicException` ao ler dados antigos

**Causa**: Aplicação tentando usar apenas chave nova para descriptografar dados criptografados com chave antiga

**Solução**:
```csharp
// Manter referência para chaves antigas
public class MultiKeyEncryptionService : IDataEncryptionService
{
    private readonly List<IDataEncryptionService> _decryptionServices;
    private readonly IDataEncryptionService _encryptionService;

    public string? Decrypt(string? cipherText)
    {
        // Tentar descriptografar com cada chave até funcionar
        foreach (var service in _decryptionServices)
        {
            try
            {
                return service.Decrypt(cipherText);
            }
            catch (CryptographicException)
            {
                // Tentar próxima chave
                continue;
            }
        }
        
        throw new CryptographicException("Failed to decrypt with any available key");
    }

    public string? Encrypt(string? plainText)
    {
        // Sempre usar chave mais recente para criptografia
        return _encryptionService.Encrypt(plainText);
    }
}
```

### Problema: Performance degradada durante re-criptografia

**Solução**: Ajustar parâmetros do script
```bash
# Reduzir batch size
--batch-size 500

# Reduzir threads paralelas
--parallel-threads 2

# Executar fora do horário de pico
--schedule "02:00-06:00"
```

## 📞 Contatos de Emergência

### Durante Rotação
- **Equipe de Segurança**: security@omnicare.com
- **DevOps**: devops@omnicare.com
- **Plantão**: +55 (11) 99999-9999

### Suporte Azure Key Vault
- **Portal**: https://portal.azure.com
- **Documentação**: https://docs.microsoft.com/azure/key-vault/
- **Suporte**: Abrir ticket no portal Azure

## 📚 Referências

- [Azure Key Vault Key Rotation](https://docs.microsoft.com/azure/key-vault/keys/how-to-configure-key-rotation)
- [NIST Key Management Guidelines](https://csrc.nist.gov/publications/detail/sp/800-57-part-1/rev-5/final)
- [OWASP Cryptographic Storage](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

---

**Versão**: 1.0  
**Última Atualização**: Janeiro 2026  
**Responsável**: Equipe de Segurança - Omni Care Software
