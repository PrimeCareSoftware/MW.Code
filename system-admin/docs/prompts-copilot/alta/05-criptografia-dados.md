# 🔐 Prompt: Criptografia de Dados Médicos

## 📊 Status
- **Prioridade**: 🔥🔥 ALTA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1-2 meses | 1 dev
- **Prazo**: Q1/2025

## 🎯 Contexto

Implementar criptografia completa de dados médicos sensíveis em repouso (at rest) no banco de dados, garantindo segurança crítica e conformidade com LGPD e CFM. Proteger informações pessoais de saúde (PHI - Protected Health Information) contra vazamentos e acessos não autorizados.

## 🔍 Justificativa

### Requisitos Legais
- **LGPD Art. 46**: Dados sensíveis de saúde requerem proteção especial
- **CFM 1.638/2002**: Segurança de prontuários eletrônicos
- **ISO 27001**: Criptografia de dados sensíveis
- **HIPAA Compliance** (referência internacional): Encryption at rest obrigatória

### Riscos de Não Implementar
- ⚠️ Vazamento de dados médicos sensíveis
- ⚠️ Exposição de informações pessoais (CPF, RG, CNS)
- ⚠️ Multas LGPD (até 2% do faturamento, máx R$ 50 milhões)
- ⚠️ Perda de confiança de pacientes
- ⚠️ Responsabilização criminal (dados de saúde)
- ⚠️ Não conformidade com auditorias

## 📋 Dados a Criptografar

### 1. Dados Médicos (Prioridade CRÍTICA)
- ✅ **Prontuários médicos completos**
  - Anamnese (subjetiva e objetiva)
  - Diagnósticos e hipóteses
  - Evolução clínica
  - Prescrições e planos terapêuticos
- ✅ **Prescrições médicas**
  - Medicamentos prescritos
  - Dosagens e posologias
  - Orientações de uso
- ✅ **Exames e laudos**
  - Resultados de exames laboratoriais
  - Laudos de imagem
  - Biópsias e anatomopatológicos
- ✅ **Dados de saúde mental**
  - Diagnósticos psiquiátricos
  - Anotações de psicoterapia
  - Tratamentos e medicações controladas

### 2. Documentos Pessoais (Prioridade ALTA)
- ✅ **CPF** (Cadastro de Pessoa Física)
- ✅ **RG** (Registro Geral)
- ✅ **CNS** (Cartão Nacional de Saúde)
- ✅ **Passaporte**
- ✅ **Carteira de motorista**

### 3. Dados Financeiros (Prioridade ALTA)
- ✅ **Números de cartão de crédito** (se armazenados - evitar se possível)
- ✅ **Dados bancários** (conta, agência)
- ✅ **Informações de pagamento**

### 4. Dados de Contato Sensíveis (Prioridade MÉDIA)
- ⚠️ Email (opcional - não obrigatório)
- ⚠️ Telefone (opcional - não obrigatório)
- ✅ Endereço completo (se requerido por regulamentação)

## 🏗️ Arquitetura de Criptografia

### Estratégia: Criptografia em Nível de Aplicação

**Escolha Recomendada: AES-256-GCM**
- Algoritmo: AES (Advanced Encryption Standard)
- Tamanho de chave: 256 bits
- Modo: GCM (Galois/Counter Mode)
- Autenticação: AEAD (Authenticated Encryption with Associated Data)

### Implementação .NET 8

```csharp
// Service de Criptografia
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    byte[] Encrypt(byte[] data);
    byte[] Decrypt(byte[] encryptedData);
}

public class AesGcmEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly ILogger<AesGcmEncryptionService> _logger;
    
    public AesGcmEncryptionService(IConfiguration configuration, ILogger<AesGcmEncryptionService> logger)
    {
        // NUNCA hardcode a chave!
        // Buscar do Azure Key Vault ou variável de ambiente
        var keyBase64 = configuration["Encryption:Key"];
        _key = Convert.FromBase64String(keyBase64);
        _logger = logger;
    }
    
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;
        
        try
        {
            using var aesGcm = new AesGcm(_key);
            
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
            var ciphertext = new byte[plainBytes.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes
            
            RandomNumberGenerator.Fill(nonce);
            
            aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);
            
            // Formato: nonce + tag + ciphertext (todos em Base64)
            var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);
            
            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criptografar dados");
            throw new EncryptionException("Falha na criptografia", ex);
        }
    }
    
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;
        
        try
        {
            using var aesGcm = new AesGcm(_key);
            
            var encryptedBytes = Convert.FromBase64String(cipherText);
            
            var nonceSize = AesGcm.NonceByteSizes.MaxSize;
            var tagSize = AesGcm.TagByteSizes.MaxSize;
            
            var nonce = new byte[nonceSize];
            var tag = new byte[tagSize];
            var ciphertext = new byte[encryptedBytes.Length - nonceSize - tagSize];
            
            Buffer.BlockCopy(encryptedBytes, 0, nonce, 0, nonceSize);
            Buffer.BlockCopy(encryptedBytes, nonceSize, tag, 0, tagSize);
            Buffer.BlockCopy(encryptedBytes, nonceSize + tagSize, ciphertext, 0, ciphertext.Length);
            
            var plainBytes = new byte[ciphertext.Length];
            aesGcm.Decrypt(nonce, ciphertext, tag, plainBytes);
            
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao descriptografar dados");
            throw new EncryptionException("Falha na descriptografia", ex);
        }
    }
}
```

### Integração com Entity Framework

```csharp
// Value Converter para criptografia automática
public class EncryptedStringConverter : ValueConverter<string, string>
{
    public EncryptedStringConverter(IEncryptionService encryptionService) 
        : base(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v))
    {
    }
}

// Configuração no DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();
    var converter = new EncryptedStringConverter(encryptionService);
    
    // Aplicar criptografia em campos específicos
    modelBuilder.Entity<Patient>()
        .Property(p => p.Cpf)
        .HasConversion(converter);
    
    modelBuilder.Entity<MedicalRecord>()
        .Property(m => m.Notes)
        .HasConversion(converter);
    
    modelBuilder.Entity<Prescription>()
        .Property(p => p.Medications)
        .HasConversion(converter);
}
```

## 🔑 Gerenciamento de Chaves

### ✅ O QUE FAZER

#### 1. Azure Key Vault (RECOMENDADO)

```csharp
// Configuração no Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Uso no EncryptionService
public AesGcmEncryptionService(IConfiguration configuration)
{
    // Chave vem diretamente do Key Vault
    var keyBase64 = configuration["EncryptionKey"];
    _key = Convert.FromBase64String(keyBase64);
}
```

**Benefícios:**
- Chaves nunca tocam o código ou disco
- Rotação automática de chaves
- Auditoria de acesso
- HSM-backed (Hardware Security Module)
- Integração nativa com Azure

#### 2. AWS KMS (Key Management Service)

```csharp
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

public class AwsKmsEncryptionService : IEncryptionService
{
    private readonly IAmazonKeyManagementService _kmsClient;
    private readonly string _keyId;
    
    public async Task<string> Encrypt(string plainText)
    {
        var request = new EncryptRequest
        {
            KeyId = _keyId,
            Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plainText))
        };
        
        var response = await _kmsClient.EncryptAsync(request);
        return Convert.ToBase64String(response.CiphertextBlob.ToArray());
    }
}
```

#### 3. HashiCorp Vault

```csharp
using VaultSharp;

public class VaultEncryptionService : IEncryptionService
{
    private readonly IVaultClient _vaultClient;
    
    public async Task<string> GetEncryptionKey()
    {
        var secret = await _vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretAsync("encryption-key", mountPoint: "secret");
        
        return secret.Data.Data["key"].ToString();
    }
}
```

#### 4. Variáveis de Ambiente (Mínimo Aceitável)

```bash
# Desenvolvimento (docker-compose.yml)
ENCRYPTION_KEY=base64EncodedKeyHere...

# Produção (Kubernetes Secret)
kubectl create secret generic encryption-key \
  --from-literal=key='base64EncodedKeyHere...'
```

### ❌ O QUE NÃO FAZER

```csharp
// ❌ NUNCA hardcode chaves!
private const string KEY = "minhachavesecreta123"; // PÉSSIMO!

// ❌ NUNCA commit chaves no código
public string GetKey() => "AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcY"; // PÉSSIMO!

// ❌ NUNCA em appsettings.json (produção)
{
  "Encryption": {
    "Key": "base64key..." // OK apenas em desenvolvimento local
  }
}

// ❌ NUNCA reutilize nonce/IV
var nonce = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }; // PÉSSIMO!
```

## 🔄 Rotação de Chaves

### Estratégia de Rotação

```csharp
public class KeyRotationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly ApplicationDbContext _context;
    
    public async Task RotateKeys()
    {
        // 1. Gerar nova chave
        var newKey = GenerateNewKey();
        
        // 2. Re-criptografar todos os dados sensíveis
        var patients = await _context.Patients.ToListAsync();
        
        foreach (var patient in patients)
        {
            // Descriptografar com chave antiga
            var decryptedCpf = _encryptionService.Decrypt(patient.Cpf);
            
            // Re-criptografar com chave nova
            patient.Cpf = _encryptionService.Encrypt(decryptedCpf, newKey);
        }
        
        await _context.SaveChangesAsync();
        
        // 3. Atualizar Key Vault
        await UpdateKeyInVault(newKey);
        
        // 4. Arquivar chave antiga (para recuperação de backups antigos)
        await ArchiveOldKey();
    }
}
```

### Cronograma de Rotação

- **Chave de Criptografia de Dados**: 365 dias (anualmente)
- **JWT Secret**: 90 dias (trimestralmente)
- **Database Passwords**: 180 dias (semestralmente)
- **API Keys Externas**: 30-90 dias
- **Certificados SSL**: Antes da expiração (geralmente 365 dias)

## 🧪 Testes

### Testes Unitários

```csharp
public class EncryptionServiceTests
{
    [Fact]
    public void ShouldEncryptAndDecryptString()
    {
        // Arrange
        var service = CreateEncryptionService();
        var plainText = "CPF: 123.456.789-00";
        
        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);
        
        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }
    
    [Fact]
    public void ShouldGenerateDifferentCipherForSamePlainText()
    {
        // Test that nonce is random (different each time)
        var service = CreateEncryptionService();
        var plainText = "Teste";
        
        var encrypted1 = service.Encrypt(plainText);
        var encrypted2 = service.Encrypt(plainText);
        
        Assert.NotEqual(encrypted1, encrypted2); // Diferentes devido a nonce aleatório
    }
    
    [Fact]
    public void ShouldHandleNullAndEmptyStrings()
    {
        var service = CreateEncryptionService();
        
        Assert.Null(service.Encrypt(null));
        Assert.Empty(service.Encrypt(string.Empty));
    }
}
```

### Testes de Performance

```csharp
[Fact]
public async Task ShouldEncrypt1000RecordsInLessThan5Seconds()
{
    var service = CreateEncryptionService();
    var stopwatch = Stopwatch.StartNew();
    
    for (int i = 0; i < 1000; i++)
    {
        var encrypted = service.Encrypt($"Test record {i}");
    }
    
    stopwatch.Stop();
    Assert.True(stopwatch.ElapsedMilliseconds < 5000);
}
```

## 📚 Referências

- [PENDING_TASKS.md - Seção Criptografia](../../PENDING_TASKS.md#6-criptografia-de-dados-médicos)
- [SUGESTOES_MELHORIAS_SEGURANCA.md](../../SUGESTOES_MELHORIAS_SEGURANCA.md)
- [LGPD - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/L13709.htm)
- [NIST SP 800-175B - Guideline for Using Cryptographic Standards](https://csrc.nist.gov/publications/detail/sp/800-175b/rev-1/final)
- [OWASP Cryptographic Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

## 💰 Investimento

- **Desenvolvimento**: 1-2 meses, 1 dev
- **Custo**: R$ 45-90k
- **Azure Key Vault**: ~R$ 150/mês
- **AWS KMS**: ~R$ 100/mês
- **ROI Esperado**: Conformidade LGPD, redução de risco de multas

## ✅ Critérios de Aceitação

1. ✅ Prontuários médicos são criptografados em repouso
2. ✅ Prescrições são criptografadas
3. ✅ Documentos pessoais (CPF, RG, CNS) são criptografados
4. ✅ Dados de saúde mental são criptografados
5. ✅ Chaves são armazenadas em Key Vault (não no código)
6. ✅ Criptografia usa AES-256-GCM
7. ✅ Nonce é gerado aleatoriamente para cada operação
8. ✅ Sistema suporta rotação de chaves
9. ✅ Performance não degrada significativamente (< 100ms adicional)
10. ✅ Testes automatizados cobrem criptografia (≥ 80%)
11. ✅ Documentação de gerenciamento de chaves está completa
12. ✅ Auditoria de acesso às chaves está implementada

---

**Última Atualização**: Janeiro 2026  
**Status**: Pronto para desenvolvimento  
**Próximo Passo**: Configurar Azure Key Vault e implementar EncryptionService
