# 🔐 Criptografia de Dados Médicos Sensíveis

**Prioridade:** 🔥🔥 P1 - ALTA  
**Obrigatoriedade:** Best Practice + LGPD Compliance  
**Status Atual:** 0% completo  
**Esforço:** 1-2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 22.500  
**Prazo:** Q1 2026 (Janeiro-Março)

## 📋 Contexto

Dados de saúde são considerados **ultra-sensíveis** pela LGPD (Art. 11 e 13), exigindo medidas técnicas e administrativas para garantir sua proteção. A criptografia em repouso (at-rest encryption) é uma das principais salvaguardas contra vazamento de dados.

### Por que é Prioridade Alta?

1. **LGPD Art. 46:** Controlador deve adotar medidas técnicas adequadas
2. **Dados Ultra-Sensíveis:** Prontuários, diagnósticos, prescrições
3. **Risco de Vazamento:** Um vazamento pode custar milhões em multas e danos
4. **Compliance e Certificações:** ISO 27001, HIPAA-like brasileiro
5. **Confiança do Cliente:** Hospitais exigem criptografia forte

### Situação Atual

- ❌ **Dados armazenados em texto claro** no banco de dados
- ❌ Sem criptografia em campos sensíveis
- ❌ Sem gestão de chaves de criptografia
- ❌ Sem rotação automática de chaves
- ✅ HTTPS habilitado (criptografia em trânsito)
- ✅ Banco de dados com autenticação forte

### Riscos de Não Implementar

- Vazamento de dados médicos sensíveis
- Multas LGPD de até **R$ 50 milhões**
- Processos judiciais de pacientes
- Perda total de confiança e reputação
- Impossibilidade de obter certificações ISO

## 🎯 Objetivos da Tarefa

Implementar criptografia AES-256-GCM em nível de aplicação para todos os dados sensíveis, com gestão segura de chaves usando Azure Key Vault ou AWS KMS, rotação automática de chaves, e impacto mínimo na performance (<10%).

## 📝 Tarefas Detalhadas

### 1. Escolha de Estratégia de Criptografia (1 semana)

#### 1.1 Opções Avaliadas

**Opção A: TDE (Transparent Data Encryption)**
- ✅ Fácil de configurar
- ✅ Zero mudanças no código
- ❌ Criptografia em nível de arquivo (menos granular)
- ❌ Todas as chaves no mesmo lugar
- ❌ Mais difícil auditar acessos

**Opção B: Application-Level Encryption (RECOMENDADO)**
- ✅ Controle granular (campo por campo)
- ✅ Chaves gerenciadas externamente
- ✅ Auditoria detalhada
- ✅ Compliance mais fácil de demonstrar
- ❌ Requer mudanças no código
- ❌ Mais complexo de implementar

**Opção C: Híbrida (TDE + Application)**
- ✅ Máxima segurança
- ❌ Complexidade e custo altos
- ⚠️ Overkill para maioria dos casos

**Decisão:** **Application-Level Encryption + Azure Key Vault/AWS KMS**

#### 1.2 Gestão de Chaves

```
Opções:
1. Azure Key Vault (Microsoft Azure) - RECOMENDADO
   - Integração nativa com .NET
   - Rotação automática de chaves
   - Auditoria integrada
   - HSM-backed (Hardware Security Module)
   - Custo: ~$0.03 por 10.000 operações

2. AWS KMS (Amazon Web Services)
   - Similar ao Key Vault
   - Integração com AWS ecosystem
   - Custo similar

3. HashiCorp Vault (Open Source/Enterprise)
   - Mais controle
   - Pode ser self-hosted
   - Mais complexo de gerenciar
```

**Decisão:** **Azure Key Vault** (melhor custo-benefício e suporte .NET)

### 2. Setup de Key Management (1 semana)

#### 2.1 Configurar Azure Key Vault

```bash
# Criar Resource Group
az group create --name omnicare-rg --location brazilsouth

# Criar Key Vault
az keyvault create \
  --name omnicare-keyvault \
  --resource-group omnicare-rg \
  --location brazilsouth \
  --enable-soft-delete true \
  --enable-purge-protection true

# Criar chave de criptografia
az keyvault key create \
  --vault-name omnicare-keyvault \
  --name medical-data-encryption-key \
  --protection software \
  --size 256 \
  --kty RSA

# Configurar rotação automática (365 dias)
az keyvault key rotation-policy update \
  --vault-name omnicare-keyvault \
  --name medical-data-encryption-key \
  --value '{
    "lifetimeActions": [{
      "trigger": { "timeAfterCreate": "P365D" },
      "action": { "type": "Rotate" }
    }],
    "attributes": { "expiryTime": "P730D" }
  }'

# Dar permissões à aplicação
az keyvault set-policy \
  --name omnicare-keyvault \
  --spn <APP_CLIENT_ID> \
  --key-permissions get unwrapKey wrapKey \
  --secret-permissions get list
```

#### 2.2 Configurar Managed Identity

```bash
# Habilitar Managed Identity para App Service
az webapp identity assign \
  --name omnicare-api \
  --resource-group omnicare-rg

# Obter Principal ID
PRINCIPAL_ID=$(az webapp identity show \
  --name omnicare-api \
  --resource-group omnicare-rg \
  --query principalId -o tsv)

# Dar permissões ao Managed Identity
az keyvault set-policy \
  --name omnicare-keyvault \
  --object-id $PRINCIPAL_ID \
  --key-permissions get unwrapKey wrapKey \
  --secret-permissions get list
```

### 3. Serviço de Criptografia (2 semanas)

#### 3.1 Interface e Implementação

```csharp
// src/MedicSoft.Core/Services/Encryption/IEncryptionService.cs
namespace MedicSoft.Core.Services.Encryption
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Criptografa texto usando AES-256-GCM
        /// </summary>
        string Encrypt(string plainText);
        
        /// <summary>
        /// Descriptografa texto criptografado
        /// </summary>
        string Decrypt(string cipherText);
        
        /// <summary>
        /// Criptografa bytes (para documentos, imagens)
        /// </summary>
        byte[] EncryptBytes(byte[] data);
        
        /// <summary>
        /// Descriptografa bytes
        /// </summary>
        byte[] DecryptBytes(byte[] encryptedData);
        
        /// <summary>
        /// Gera hash para indexação de campos criptografados
        /// </summary>
        string Hash(string value);
    }
}
```

```csharp
// src/MedicSoft.Infrastructure/Services/Encryption/AesGcmEncryptionService.cs
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Security.Cryptography;

namespace MedicSoft.Infrastructure.Services.Encryption
{
    public class AesGcmEncryptionService : IEncryptionService
    {
        private readonly CryptographyClient _cryptoClient;
        private readonly ILogger<AesGcmEncryptionService> _logger;
        private readonly byte[] _cachedKey;
        private readonly DateTime _keyExpiresAt;
        
        public AesGcmEncryptionService(
            IConfiguration configuration,
            ILogger<AesGcmEncryptionService> logger)
        {
            _logger = logger;
            
            var keyVaultUrl = configuration["Azure:KeyVault:Url"];
            var keyName = configuration["Azure:KeyVault:KeyName"];
            
            // Usar Managed Identity
            var credential = new DefaultAzureCredential();
            var keyClient = new KeyClient(new Uri(keyVaultUrl), credential);
            
            var key = keyClient.GetKey(keyName).Value;
            _cryptoClient = new CryptographyClient(key.Id, credential);
            
            // Cache de chave por 1 hora
            _cachedKey = DeriveDataEncryptionKey();
            _keyExpiresAt = DateTime.UtcNow.AddHours(1);
        }
        
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = EncryptBytes(plainBytes);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to encrypt data");
                throw new CryptographicException("Encryption failed", ex);
            }
        }
        
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
            
            try
            {
                var encryptedBytes = Convert.FromBase64String(cipherText);
                var plainBytes = DecryptBytes(encryptedBytes);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt data");
                throw new CryptographicException("Decryption failed", ex);
            }
        }
        
        public byte[] EncryptBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;
            
            // Gerar nonce aleatório (12 bytes para GCM)
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(nonce);
            
            // Gerar tag de autenticação (16 bytes)
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            
            // Criptografar
            var ciphertext = new byte[data.Length];
            
            using var aesGcm = new AesGcm(GetDataEncryptionKey());
            aesGcm.Encrypt(nonce, data, ciphertext, tag);
            
            // Formato: [nonce(12)][tag(16)][ciphertext(N)]
            var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);
            
            return result;
        }
        
        public byte[] DecryptBytes(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return encryptedData;
            
            // Extrair componentes
            var nonceSize = AesGcm.NonceByteSizes.MaxSize;
            var tagSize = AesGcm.TagByteSizes.MaxSize;
            
            var nonce = new byte[nonceSize];
            var tag = new byte[tagSize];
            var ciphertext = new byte[encryptedData.Length - nonceSize - tagSize];
            
            Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonceSize);
            Buffer.BlockCopy(encryptedData, nonceSize, tag, 0, tagSize);
            Buffer.BlockCopy(encryptedData, nonceSize + tagSize, ciphertext, 0, ciphertext.Length);
            
            // Descriptografar
            var plaintext = new byte[ciphertext.Length];
            
            using var aesGcm = new AesGcm(GetDataEncryptionKey());
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
            
            return plaintext;
        }
        
        public string Hash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            
            // SHA-256 para indexação
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashBytes);
        }
        
        private byte[] GetDataEncryptionKey()
        {
            // Verificar se chave em cache expirou
            if (DateTime.UtcNow > _keyExpiresAt)
            {
                return DeriveDataEncryptionKey();
            }
            
            return _cachedKey;
        }
        
        private byte[] DeriveDataEncryptionKey()
        {
            // Usar Key Vault para gerar DEK (Data Encryption Key)
            // Na prática, usar KEK (Key Encryption Key) do Key Vault
            // para proteger DEK gerada localmente
            
            var dek = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(dek);
            
            // Wrap DEK com KEK do Key Vault
            var wrappedKey = _cryptoClient.WrapKey(KeyWrapAlgorithm.RsaOaep, dek).EncryptedKey;
            
            // Armazenar wrapped key para recuperação futura
            // (implementação simplificada - em produção usar cache distribuído)
            
            return dek;
        }
    }
}
```

### 4. Identificar Dados Sensíveis (1 semana)

#### 4.1 Mapeamento de Dados

```markdown
## Dados que DEVEM ser criptografados:

### Alta Prioridade (Dados LGPD Categoria Especial)
1. **Prontuários Médicos**
   - Anamnese
   - Exame clínico
   - Hipóteses diagnósticas
   - Plano terapêutico
   - Evoluções

2. **Prescrições**
   - Medicamentos controlados
   - Dosagens
   - Orientações

3. **Dados Pessoais Sensíveis**
   - CPF
   - RG
   - CNS (Cartão Nacional de Saúde)
   - Data de nascimento
   - Endereço completo

4. **Informações de Saúde**
   - Alergias
   - Comorbidades
   - Histórico familiar
   - Vícios e hábitos
   - Saúde mental

5. **Resultados de Exames**
   - Laudos
   - Imagens médicas (metadata)
   - Resultados laboratoriais

### Média Prioridade
6. **Dados Financeiros**
   - Números de cartão (se armazenados)
   - Informações bancárias
   - CPF/CNPJ de pagadores

### Dados que NÃO precisam criptografia
- Nome (indexação necessária)
- Email (indexação necessária)
- Telefone (indexação necessária)
- Dados não-sensíveis de configuração
```

### 5. Implementação Backend (3 semanas)

#### 5.1 Atributo [Encrypted]

```csharp
// src/MedicSoft.Core/Attributes/EncryptedAttribute.cs
namespace MedicSoft.Core.Attributes
{
    /// <summary>
    /// Marca propriedade para criptografia automática
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute
    {
        public bool Searchable { get; set; }
    }
}
```

#### 5.2 Entidades com Criptografia

```csharp
// src/MedicSoft.Core/Entities/Patient.cs
public class Patient : BaseEntity
{
    // Dados não-criptografados (indexação)
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    
    // Dados criptografados
    [Encrypted(Searchable = true)]
    public string CPF { get; set; }
    
    // Hash para busca (gerado automaticamente)
    public string CPFHash { get; set; }
    
    [Encrypted]
    public string RG { get; set; }
    
    [Encrypted]
    public string CNS { get; set; }
    
    [Encrypted]
    public string Address { get; set; }
    
    [Encrypted]
    public string MedicalHistory { get; set; }
    
    [Encrypted]
    public string Allergies { get; set; }
}
```

```csharp
// src/MedicSoft.Core/Entities/MedicalRecord.cs
public class MedicalRecord : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    
    // Tudo criptografado
    [Encrypted]
    public string ChiefComplaint { get; set; }
    
    [Encrypted]
    public string Anamnesis { get; set; }
    
    [Encrypted]
    public string PhysicalExamination { get; set; }
    
    [Encrypted]
    public string Diagnosis { get; set; }
    
    [Encrypted]
    public string TreatmentPlan { get; set; }
    
    [Encrypted]
    public string Notes { get; set; }
}
```

#### 5.3 Entity Framework Interceptor

```csharp
// src/MedicSoft.Infrastructure/Data/Interceptors/EncryptionInterceptor.cs
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MedicSoft.Infrastructure.Data.Interceptors
{
    public class EncryptionInterceptor : SaveChangesInterceptor
    {
        private readonly IEncryptionService _encryptionService;
        
        public EncryptionInterceptor(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }
        
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ProcessEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ProcessEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        
        private void ProcessEntities(DbContext context)
        {
            if (context == null) return;
            
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    EncryptEntity(entry.Entity);
                }
                else if (entry.State == EntityState.Unchanged || entry.State == EntityState.Modified)
                {
                    DecryptEntity(entry.Entity);
                }
            }
        }
        
        private void EncryptEntity(object entity)
        {
            var properties = entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null);
            
            foreach (var property in properties)
            {
                var value = property.GetValue(entity) as string;
                if (!string.IsNullOrEmpty(value) && !IsEncrypted(value))
                {
                    var encrypted = _encryptionService.Encrypt(value);
                    property.SetValue(entity, encrypted);
                    
                    // Se searchable, criar hash
                    var attr = property.GetCustomAttribute<EncryptedAttribute>();
                    if (attr.Searchable)
                    {
                        var hashProperty = entity.GetType().GetProperty($"{property.Name}Hash");
                        if (hashProperty != null)
                        {
                            var hash = _encryptionService.Hash(value);
                            hashProperty.SetValue(entity, hash);
                        }
                    }
                }
            }
        }
        
        private void DecryptEntity(object entity)
        {
            var properties = entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null);
            
            foreach (var property in properties)
            {
                var value = property.GetValue(entity) as string;
                if (!string.IsNullOrEmpty(value) && IsEncrypted(value))
                {
                    var decrypted = _encryptionService.Decrypt(value);
                    property.SetValue(entity, decrypted);
                }
            }
        }
        
        private bool IsEncrypted(string value)
        {
            // Verificar se já está criptografado (base64 válido)
            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

#### 5.4 Busca em Campos Criptografados

```csharp
// Exemplo de busca por CPF criptografado
public async Task<Patient> FindByCPFAsync(string cpf)
{
    // Gerar hash do CPF para busca
    var cpfHash = _encryptionService.Hash(cpf);
    
    // Buscar por hash
    var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.CPFHash == cpfHash);
    
    return patient;
}
```

### 6. Migração de Dados Existentes (2 semanas)

#### 6.1 Script de Migração

```csharp
// src/MedicSoft.Infrastructure/Migrations/Scripts/EncryptExistingData.cs
public class EncryptExistingDataMigration
{
    private readonly ApplicationDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<EncryptExistingDataMigration> _logger;
    
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting encryption of existing data...");
        
        // Desabilitar tracking para performance
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
        
        await EncryptPatientsAsync(cancellationToken);
        await EncryptMedicalRecordsAsync(cancellationToken);
        await EncryptPrescriptionsAsync(cancellationToken);
        
        _logger.LogInformation("Encryption of existing data completed.");
    }
    
    private async Task EncryptPatientsAsync(CancellationToken cancellationToken)
    {
        var batchSize = 1000;
        var skip = 0;
        
        while (true)
        {
            var patients = await _context.Patients
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
            
            if (!patients.Any()) break;
            
            foreach (var patient in patients)
            {
                // Criptografar campos sensíveis
                if (!string.IsNullOrEmpty(patient.CPF) && !IsEncrypted(patient.CPF))
                {
                    patient.CPF = _encryptionService.Encrypt(patient.CPF);
                    patient.CPFHash = _encryptionService.Hash(patient.CPF);
                }
                
                if (!string.IsNullOrEmpty(patient.RG) && !IsEncrypted(patient.RG))
                {
                    patient.RG = _encryptionService.Encrypt(patient.RG);
                }
                
                // Outros campos...
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            
            skip += batchSize;
            _logger.LogInformation($"Encrypted {skip} patients...");
        }
    }
    
    private bool IsEncrypted(string value)
    {
        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

```bash
# Executar migração de dados
dotnet run --project src/MedicSoft.Cli encrypt-existing-data
```

### 7. Performance e Otimização (1 semana)

#### 7.1 Cache de Chaves

```csharp
// src/MedicSoft.Infrastructure/Services/Encryption/CachedEncryptionService.cs
public class CachedEncryptionService : IEncryptionService
{
    private readonly IEncryptionService _inner;
    private readonly IMemoryCache _cache;
    
    public CachedEncryptionService(
        IEncryptionService inner,
        IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }
    
    public string Encrypt(string plainText)
    {
        // Cache não aplicável para criptografia (cada resultado deve ser único)
        return _inner.Encrypt(plainText);
    }
    
    public string Decrypt(string cipherText)
    {
        // Cache de descriptografia por 5 minutos
        var cacheKey = $"decrypt:{cipherText}";
        
        if (_cache.TryGetValue(cacheKey, out string cachedValue))
        {
            return cachedValue;
        }
        
        var decrypted = _inner.Decrypt(cipherText);
        
        _cache.Set(cacheKey, decrypted, TimeSpan.FromMinutes(5));
        
        return decrypted;
    }
}
```

#### 7.2 Benchmark

```csharp
// tests/MedicSoft.Benchmarks/EncryptionBenchmarks.cs
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class EncryptionBenchmarks
{
    private IEncryptionService _encryptionService;
    private string _sampleText;
    private string _encryptedText;
    
    [GlobalSetup]
    public void Setup()
    {
        _encryptionService = new AesGcmEncryptionService(/*...*/);
        _sampleText = "Paciente apresenta quadro de hipertensão arterial sistêmica";
        _encryptedText = _encryptionService.Encrypt(_sampleText);
    }
    
    [Benchmark]
    public string Encrypt_SmallText()
    {
        return _encryptionService.Encrypt(_sampleText);
    }
    
    [Benchmark]
    public string Decrypt_SmallText()
    {
        return _encryptionService.Decrypt(_encryptedText);
    }
    
    [Benchmark]
    public string Hash_SmallText()
    {
        return _encryptionService.Hash(_sampleText);
    }
}
```

```bash
# Executar benchmark
dotnet run -c Release --project tests/MedicSoft.Benchmarks

# Meta: < 1ms para encrypt/decrypt de texto pequeno
```

### 8. Testes (1 semana)

#### 8.1 Testes Unitários

```csharp
// tests/MedicSoft.Tests/Services/EncryptionServiceTests.cs
public class EncryptionServiceTests
{
    [Fact]
    public void Encrypt_Decrypt_Should_ReturnOriginalValue()
    {
        // Arrange
        var service = CreateEncryptionService();
        var originalText = "Dados sensíveis do paciente";
        
        // Act
        var encrypted = service.Encrypt(originalText);
        var decrypted = service.Decrypt(encrypted);
        
        // Assert
        Assert.NotEqual(originalText, encrypted);
        Assert.Equal(originalText, decrypted);
    }
    
    [Fact]
    public void Encrypt_SameValue_Should_ProduceDifferentCiphertext()
    {
        // Arrange
        var service = CreateEncryptionService();
        var text = "Test";
        
        // Act
        var encrypted1 = service.Encrypt(text);
        var encrypted2 = service.Encrypt(text);
        
        // Assert (nonce aleatório)
        Assert.NotEqual(encrypted1, encrypted2);
    }
    
    [Fact]
    public void Hash_SameValue_Should_ProduceSameHash()
    {
        // Arrange
        var service = CreateEncryptionService();
        var text = "12345678900";
        
        // Act
        var hash1 = service.Hash(text);
        var hash2 = service.Hash(text);
        
        // Assert
        Assert.Equal(hash1, hash2);
    }
    
    [Fact]
    public void EncryptBytes_Should_EncryptBinaryData()
    {
        // Arrange
        var service = CreateEncryptionService();
        var data = Encoding.UTF8.GetBytes("Binary data");
        
        // Act
        var encrypted = service.EncryptBytes(data);
        var decrypted = service.DecryptBytes(encrypted);
        
        // Assert
        Assert.Equal(data, decrypted);
    }
}
```

#### 8.2 Testes de Integração

```csharp
// tests/MedicSoft.IntegrationTests/EncryptionIntegrationTests.cs
public class EncryptionIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SavePatient_Should_EncryptSensitiveFields()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder => { });
        var client = factory.CreateClient();
        
        var patient = new CreatePatientDto
        {
            Name = "João Silva",
            CPF = "12345678900",
            RG = "1234567",
            MedicalHistory = "Hipertensão"
        };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/patients", patient);
        response.EnsureSuccessStatusCode();
        
        var createdPatient = await response.Content.ReadFromJsonAsync<Patient>();
        
        // Assert
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Verificar no banco que está criptografado
        var dbPatient = await context.Patients.FindAsync(createdPatient.Id);
        Assert.NotEqual(patient.CPF, dbPatient.CPF);
        Assert.True(IsBase64(dbPatient.CPF)); // Criptografado
    }
}
```

### 9. Documentação e Configuração (1 semana)

#### 9.1 Configuração appsettings.json

```json
{
  "Azure": {
    "KeyVault": {
      "Url": "https://omnicare-keyvault.vault.azure.net/",
      "KeyName": "medical-data-encryption-key",
      "UseManagedIdentity": true
    }
  },
  "Encryption": {
    "EnableEncryption": true,
    "CacheExpirationMinutes": 60,
    "KeyRotationDays": 365
  }
}
```

#### 9.2 Startup Configuration

```csharp
// src/MedicSoft.Api/Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Encryption
    services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();
    services.Decorate<IEncryptionService, CachedEncryptionService>();
    
    // DbContext com interceptor
    services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        
        var encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();
        options.AddInterceptors(new EncryptionInterceptor(encryptionService));
    });
}
```

## ✅ Critérios de Sucesso

### Técnicos
- [ ] 100% dos dados sensíveis criptografados (conforme mapeamento)
- [ ] Chaves gerenciadas no Azure Key Vault (NUNCA no código ou banco)
- [ ] Rotação automática de chaves a cada 365 dias
- [ ] Impacto de performance < 10% (benchmark necessário)
- [ ] Cache de chaves funcionando (reduz latência)

### Funcionais
- [ ] Busca por CPF continua funcionando (usando hash)
- [ ] Exportação de dados descriptografa corretamente
- [ ] APIs retornam dados descriptografados
- [ ] Migração de dados existentes completa (0% texto claro)

### Segurança
- [ ] AES-256-GCM implementado corretamente
- [ ] Nonce aleatório em cada criptografia (não reutilizar)
- [ ] Tag de autenticação verificada na descriptografia
- [ ] Chaves nunca logadas ou expostas
- [ ] Pentest aprovado

## 📦 Entregáveis

1. **Backend**
   - IEncryptionService com implementação AES-256-GCM
   - Azure Key Vault integration
   - Entity Framework Interceptor
   - Atributo [Encrypted]
   - Script de migração de dados existentes

2. **Infraestrutura**
   - Azure Key Vault configurado
   - Managed Identity habilitada
   - Rotação automática de chaves
   - Políticas de acesso

3. **Documentação**
   - Guia de criptografia
   - Procedimentos de rotação de chaves
   - Disaster recovery (perda de chaves)
   - Documentação de compliance

4. **Testes**
   - Testes unitários de EncryptionService
   - Testes de integração com EF
   - Benchmarks de performance

## 🔗 Dependências

### Pré-requisitos
- ✅ Entity Framework configurado
- ✅ Azure/AWS account (para Key Vault/KMS)
- ❌ Task #08 (Auditoria) - recomendado mas não obrigatório

### Dependências Externas
- Azure Key Vault ou AWS KMS
- Managed Identity configurada

### Tarefas Dependentes
- Task #08 (Auditoria) - pode auditar operações de criptografia

## 🧪 Testes

### Testes de Performance
```bash
# Benchmark de performance
dotnet run -c Release --project tests/MedicSoft.Benchmarks

# Meta: 
# - Encrypt: < 1ms
# - Decrypt: < 1ms
# - Hash: < 0.5ms
```

### Testes de Segurança
1. Verificar que dados no banco estão criptografados (não texto claro)
2. Testar rotação de chaves
3. Simular perda de acesso ao Key Vault (disaster recovery)
4. Validar que chaves nunca são logadas

## 📊 Métricas de Sucesso

- **Performance:** < 10% de overhead
- **Cobertura:** 100% de dados sensíveis criptografados
- **Segurança:** AES-256-GCM implementado corretamente
- **Compliance:** Aprovação de auditor de segurança

## 🚨 Riscos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Performance degradada | Média | Alto | Cache, benchmark, otimização |
| Perda de chaves | Baixa | Crítico | Backup de Key Vault, disaster recovery |
| Custo de Key Vault alto | Baixa | Médio | Monitorar uso, otimizar chamadas |
| Migração de dados falha | Média | Alto | Backup completo antes, execução em etapas |
| Busca não funciona | Média | Alto | Usar hashes para campos searchable |

## 📚 Referências

### Legal
- [LGPD Art. 46 - Medidas de Segurança](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- ISO 27001 - Information Security Management

### Técnico
- [Azure Key Vault Best Practices](https://docs.microsoft.com/azure/key-vault/general/best-practices)
- [AES-GCM Encryption](https://en.wikipedia.org/wiki/Galois/Counter_Mode)
- [OWASP Cryptographic Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

### Código
- `src/MedicSoft.Core/Services/Encryption/` - Serviços de criptografia
- `src/MedicSoft.Infrastructure/Data/Interceptors/` - EF Interceptor
- `src/MedicSoft.Core/Attributes/EncryptedAttribute.cs` - Atributo customizado

---

> **IMPORTANTE:** Esta task implementa **criptografia at-rest** para dados sensíveis (LGPD compliance)  
> **Próximos Passos:** Após aprovação, iniciar setup Azure Key Vault  
> **Última Atualização:** 23 de Janeiro de 2026
