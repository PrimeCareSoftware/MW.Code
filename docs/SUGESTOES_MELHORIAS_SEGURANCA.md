# 🔐 Sugestões de Melhorias de Segurança - MedicWarehouse

> **IMPORTANTE**: Este documento contém apenas **sugestões** de melhorias de segurança. Nenhuma implementação foi realizada.

## 📋 Índice
1. [Proteção contra Ataques de Força Bruta](#1-proteção-contra-ataques-de-força-bruta)
2. [Bloqueio e Lista de IPs](#2-bloqueio-e-lista-de-ips)
3. [SQL Injection - Defesas Adicionais](#3-sql-injection---defesas-adicionais)
4. [Logging e Auditoria de Segurança](#4-logging-e-auditoria-de-segurança)
5. [Testes de Penetração (Pentest)](#5-testes-de-penetração-pentest)
6. [Validação de Tokens e Sessões](#6-validação-de-tokens-e-sessões)
7. [Proteção de Dados Sensíveis](#7-proteção-de-dados-sensíveis)
8. [Segurança de Upload de Arquivos](#8-segurança-de-upload-de-arquivos)
9. [Proteção contra CSRF](#9-proteção-contra-csrf)
10. [Monitoramento e Detecção de Intrusões](#10-monitoramento-e-detecção-de-intrusões)
11. [Hardening da Infraestrutura](#11-hardening-da-infraestrutura)
12. [Conformidade com LGPD e HIPAA](#12-conformidade-com-lgpd-e-hipaa)

---

## 1. 🛡️ Proteção contra Ataques de Força Bruta

### 📊 Status Atual
- Rate limiting implementado: ✅ (100 req/min em dev, 10 req/min em produção)
- BCrypt para hashing de senhas: ✅ (Work factor 12)
- Validação de força de senha: ✅

### 🔧 Sugestões de Melhorias

#### 1.1 Sistema de Bloqueio de Conta por Tentativas Falhadas
```csharp
// Sugestão de implementação
public class LoginAttemptTracker
{
    - Contador de tentativas falhadas por usuário
    - Bloqueio temporário após X tentativas (ex: 5 tentativas)
    - Tempo de bloqueio progressivo: 5min, 15min, 1h, 24h
    - Notificação ao usuário por email quando conta for bloqueada
    - Log de todas as tentativas falhadas com IP, timestamp, user-agent
}
```

#### 1.2 CAPTCHA em Endpoints Críticos
- **Onde implementar**: `/api/auth/login`, `/api/auth/owner-login`, `/api/auth/password-recovery`
- **Tecnologias sugeridas**: 
  - reCAPTCHA v3 (Google) - Funciona em background sem interação
  - hCaptcha - Alternativa focada em privacidade
  - Cloudflare Turnstile - Sem cookies de rastreamento

#### 1.3 Multi-Factor Authentication (MFA/2FA)
**Status atual**: Sistema de 2FA implementado apenas para recuperação de senha ✅

**Sugestões de expansão**:
- Habilitar 2FA no login principal (não apenas na recuperação)
- Suporte a múltiplos métodos:
  - SMS (já implementado)
  - Email (já implementado)
  - **NOVO**: Autenticador TOTP (Google Authenticator, Microsoft Authenticator)
  - **NOVO**: Chaves de segurança U2F/FIDO2 (YubiKey)
  - **NOVO**: Códigos de backup descartáveis

#### 1.4 Análise de Comportamento de Login
```csharp
// Detectar padrões suspeitos
- Login de localização geográfica incomum
- Login em horários atípicos para o usuário
- Múltiplos logins simultâneos de IPs diferentes
- Mudança abrupta de dispositivo/navegador
- Velocidade impossível (login em países diferentes em curto período)
```

---

## 2. 🚫 Bloqueio e Lista de IPs

### 📊 Status Atual
- Não há sistema de bloqueio de IP implementado ❌
- Rate limiting usa IP como identificador parcial ✅

### 🔧 Sugestões de Melhorias

#### 2.1 Sistema de Lista Negra (Blacklist) de IPs
```csharp
public class IpBlockingMiddleware
{
    // Funcionalidades sugeridas:
    - Lista negra de IPs persistida em banco de dados
    - Bloqueio manual pelo administrador
    - Bloqueio automático baseado em comportamento
    - TTL configurável para bloqueios temporários
    - Whitelist para IPs confiáveis (escritórios, VPNs corporativas)
    - Interface administrativa para gerenciar IPs bloqueados
}
```

#### 2.2 Integração com Serviços de Inteligência de Ameaças
- **AbuseIPDB**: Verificar IPs em lista global de IPs maliciosos
- **IPQualityScore**: Análise de qualidade e reputação de IP
- **Cloudflare Zero Trust**: Proteção em nível de rede
- **MaxMind GeoIP2**: Detecção de proxies, VPNs e IPs de alto risco

#### 2.3 Bloqueio Geográfico (Geo-blocking)
```json
{
  "GeoBlocking": {
    "Enabled": true,
    "BlockedCountries": ["CN", "RU", "KP", "IR"],
    "AllowedCountries": ["BR", "US", "PT", "AR"],
    "Mode": "AllowList" // ou "BlockList"
  }
}
```

#### 2.4 Detecção de Proxy/VPN/Tor
- Bloquear ou adicionar fricção em logins de:
  - Servidores proxy anônimos
  - VPNs comerciais (opcional)
  - Exit nodes Tor
  - Data centers conhecidos (AWS, Azure, GCP quando não esperado)

#### 2.5 Rate Limiting por IP Mais Agressivo
**Configuração atual**: Global para todos os endpoints

**Sugestão**: Rate limiting diferenciado por tipo de endpoint:
```json
{
  "RateLimiting": {
    "AuthEndpoints": {
      "PermitLimit": 5,
      "WindowSeconds": 300  // 5 tentativas a cada 5 minutos
    },
    "ApiEndpoints": {
      "PermitLimit": 100,
      "WindowSeconds": 60
    },
    "PublicEndpoints": {
      "PermitLimit": 20,
      "WindowSeconds": 60
    }
  }
}
```

---

## 3. 💉 SQL Injection - Defesas Adicionais

### 📊 Status Atual
- Entity Framework Core com queries parametrizadas: ✅ Excelente!
- Nenhuma query raw SQL encontrada: ✅
- Sanitização de entrada básica: ✅
- Filtros globais de tenant (multi-tenancy): ✅

### 🔧 Sugestões de Melhorias

#### 3.1 Stored Procedures para Operações Críticas
Mesmo com EF Core, considere usar Stored Procedures para:
- Operações financeiras (pagamentos, faturas)
- Mudanças de permissões/roles
- Operações em massa (batch)
- Relatórios complexos

**Vantagens**:
- Performance otimizada
- Lógica de negócio no banco
- Superfície de ataque reduzida
- Facilita auditoria

#### 3.2 Princípio do Menor Privilégio no Banco de Dados
```sql
-- Criar usuários de banco com permissões limitadas
-- Usuário da aplicação NÃO deve ter:
- DROP TABLE/DATABASE
- ALTER SCHEMA
- CREATE/ALTER/DROP STORED PROCEDURES
- EXECUTE em procedures do sistema

-- Apenas deve ter:
- SELECT, INSERT, UPDATE, DELETE em tabelas específicas
- EXECUTE em stored procedures da aplicação
```

#### 3.3 Auditoria de Queries do EF Core
```csharp
// Logging de todas as queries geradas
builder.Services.AddDbContext<MedicSoftDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(isDevelopment); // APENAS em DEV
    options.LogTo(Console.WriteLine, LogLevel.Information); // Log de queries
});
```

#### 3.4 Validação de Input em Múltiplas Camadas
**Camadas sugeridas**:
1. **Frontend** (Angular): Validação inicial, UX
2. **Controller** (API): Data Annotations, ModelState
3. **Service Layer**: Business rules validation
4. **Domain Layer**: Value Objects, invariants
5. **Repository**: Sanitização final antes do banco

#### 3.5 Proteção contra Mass Assignment
```csharp
// Usar DTOs específicos ao invés de expor entidades diretamente
[HttpPost]
public async Task<ActionResult> UpdatePatient([FromBody] UpdatePatientDTO dto)
{
    // DTO previne que campos não autorizados sejam atualizados
    // Nunca usar: [FromBody] Patient patient
}
```

#### 3.6 Detecção de Anomalias em Queries
- Monitorar queries com duração anormal
- Alertar sobre queries que retornam quantidade incomum de registros
- Detectar padrões de varredura de dados (data scraping)

---

## 4. 📝 Logging e Auditoria de Segurança

### 📊 Status Atual
- Logging básico configurado: ✅
- Sem auditoria estruturada: ❌
- Sem trilha de auditoria para LGPD: ❌

### 🔧 Sugestões de Melhorias

#### 4.1 Sistema de Auditoria Completo
```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string Action { get; set; }  // CREATE, READ, UPDATE, DELETE, LOGIN, LOGOUT
    public string EntityType { get; set; }  // Patient, MedicalRecord, etc
    public string EntityId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string OldValues { get; set; }  // JSON dos valores antigos
    public string NewValues { get; set; }  // JSON dos valores novos
    public string Result { get; set; }  // SUCCESS, FAILED, UNAUTHORIZED
    public string FailureReason { get; set; }
}
```

#### 4.2 Eventos de Segurança a Auditar
**Autenticação**:
- ✅ Login bem-sucedido
- ✅ Tentativa de login falhada
- ⚠️ NOVO: Logout
- ⚠️ NOVO: Expiração de sessão
- ⚠️ NOVO: Token renovado
- ⚠️ NOVO: Token invalidado
- ⚠️ NOVO: MFA habilitado/desabilitado
- ⚠️ NOVO: Senha alterada

**Autorização**:
- ❌ NOVO: Acesso negado (403)
- ❌ NOVO: Tentativa de acesso a recurso de outro tenant
- ❌ NOVO: Escalação de privilégios tentada

**Dados Sensíveis**:
- ❌ NOVO: Acesso a prontuário médico
- ❌ NOVO: Modificação de dados de paciente
- ❌ NOVO: Download de relatórios
- ❌ NOVO: Exportação de dados
- ❌ NOVO: Exclusão de registros (soft delete)

**Configurações**:
- ❌ NOVO: Mudança de configuração do sistema
- ❌ NOVO: Criação/alteração de usuário
- ❌ NOVO: Mudança de permissões

#### 4.3 Centralização de Logs
**Ferramentas sugeridas**:
- **Serilog** com sinks para:
  - Elasticsearch + Kibana (ELK Stack)
  - Azure Application Insights
  - AWS CloudWatch
  - Seq (ferramenta .NET específica)
  - Splunk

#### 4.4 Alertas em Tempo Real
```csharp
// Alertas automáticos para:
- 10+ tentativas de login falhadas em 5 minutos
- Acesso a dados de outro tenant detectado
- Mudança de configuração crítica
- Exclusão em massa de registros
- Erro 500 recorrente (possível ataque)
- Tráfego anormal de um IP
- Upload de arquivo suspeito
```

#### 4.5 Retenção e Arquivamento de Logs
```json
{
  "LogRetention": {
    "SecurityLogs": "7 anos",  // Conformidade com regulamentações médicas
    "AuditTrail": "10 anos",
    "ApplicationLogs": "90 dias",
    "PerformanceLogs": "30 dias"
  }
}
```

---

## 5. 🎯 Testes de Penetração (Pentest)

### 📊 Status Atual
- Sem evidências de pentest realizados: ❌
- Testes automatizados de segurança: ❌

### 🔧 Sugestões de Melhorias

#### 5.1 Pentest Manual Profissional
**Frequência sugerida**: Semestral ou anual

**Escopo sugerido**:
- Web Application Penetration Testing (OWASP)
- API Security Testing
- Authentication & Session Management
- Authorization & Access Control
- Business Logic Testing
- Infrastructure Penetration Testing
- Social Engineering (opcional)

**Empresas/Serviços no Brasil**:
- Morphus Labs
- Clavis Segurança da Informação
- E-VAL Tecnologia
- Tempest Security Intelligence

#### 5.2 Bug Bounty Program
**Quando implementar**: Após maturidade de segurança

**Plataformas sugeridas**:
- HackerOne
- Bugcrowd
- Intigriti
- YesWeHack

**Benefícios**:
- Descoberta contínua de vulnerabilidades
- Custo-benefício (paga apenas por bugs válidos)
- Comunidade de pesquisadores

#### 5.3 Ferramentas de Pentest Automatizadas

**Web Application Scanners**:
```bash
# OWASP ZAP (Open Source)
docker run -t owasp/zap2docker-stable zap-baseline.py \
  -t https://sua-api.medicwarehouse.com

# Burp Suite Professional
# Scan manual e automatizado de vulnerabilidades

# Nikto
nikto -h https://sua-api.medicwarehouse.com
```

**API Security Testing**:
```bash
# OWASP API Security Top 10
# Ferramentas sugeridas:
- Postman Security Tests
- REST-Assured (Java)
- Katalon Studio
- SoapUI Security Testing
```

**Dependency Scanning**:
```bash
# Verificar vulnerabilidades em pacotes NuGet
dotnet list package --vulnerable --include-transitive

# OWASP Dependency-Check
dependency-check --project MedicWarehouse --scan ./src

# Snyk
snyk test
```

#### 5.4 SAST (Static Application Security Testing)
**Ferramentas sugeridas**:
- **SonarQube/SonarCloud**: ✅ Já em uso parcialmente
- **Fortify Static Code Analyzer**
- **Checkmarx**
- **Veracode**
- **Security Code Scan** (extensão Roslyn para .NET)

**Integração CI/CD**:
```yaml
# GitHub Actions
- name: Run SAST
  run: |
    dotnet tool install --global security-scan
    security-scan ./src/**/*.csproj
```

#### 5.5 DAST (Dynamic Application Security Testing)
**Ferramentas sugeridas**:
- OWASP ZAP em modo Spider + Active Scan
- Burp Suite CI/CD Integration
- Acunetix
- Netsparker

#### 5.6 IAST (Interactive Application Security Testing)
**Ferramentas sugeridas**:
- Contrast Security
- Hdiv Detection
- Seeker (Synopsys)

**Vantagem**: Analisa a aplicação em runtime, durante testes funcionais

---

## 6. 🎫 Validação de Tokens e Sessões

### 📊 Status Atual
- JWT implementado corretamente: ✅
- Validação completa de tokens: ✅
- Expiração em 60 minutos: ✅
- Zero clock skew: ✅

### 🔧 Sugestões de Melhorias

#### 6.1 Refresh Token Pattern
**Problema atual**: Quando token expira, usuário precisa fazer login novamente

**Solução sugerida**:
```csharp
public class TokenResponse
{
    public string AccessToken { get; set; }  // Curta duração (15-30 min)
    public string RefreshToken { get; set; }  // Longa duração (7-30 dias)
    public DateTime AccessTokenExpiresAt { get; set; }
}

// Endpoint para renovar
[HttpPost("refresh")]
public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
{
    // Validar refresh token
    // Gerar novo access token
    // Opcionalmente rotacionar refresh token
}
```

**Benefícios**:
- Melhor UX (usuário não é deslogado constantemente)
- Mais seguro (access token de curta duração)
- Possibilidade de revogar refresh tokens

#### 6.2 Token Revocation (Lista de Tokens Revogados)
```csharp
public class RevokedToken
{
    public string TokenId { get; set; }  // "jti" claim do JWT
    public DateTime RevokedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Reason { get; set; }  // LOGOUT, PASSWORD_CHANGED, SECURITY_BREACH
}

// Middleware para verificar revogação
public class TokenRevocationMiddleware
{
    // Verificar se token está na lista de revogados
    // Rejeitar se estiver
}
```

**Usar em casos de**:
- Logout explícito do usuário
- Mudança de senha
- Desativação de conta
- Suspeita de comprometimento

#### 6.3 Token Binding (Device Binding)
```csharp
// Associar token a um dispositivo específico
var claims = new[]
{
    // ... claims existentes
    new Claim("device_fingerprint", GenerateDeviceFingerprint(httpContext)),
    new Claim("ip_address", httpContext.Connection.RemoteIpAddress.ToString())
};

// Na validação, verificar se device_fingerprint e IP correspondem
```

**Nota**: IP pode mudar (mobile networks), usar com cautela

#### 6.4 JWT Security Best Practices Checklist
- ✅ Algoritmo seguro (HS256 ou melhor RS256)
- ✅ Chave secreta forte (>= 256 bits)
- ✅ Expiração configurada
- ✅ Validação de issuer e audience
- ⚠️ **SUGESTÃO**: Usar "jti" (JWT ID) para rastreamento e revogação
- ⚠️ **SUGESTÃO**: Incluir "nbf" (not before) para prevenir replay imediato
- ⚠️ **SUGESTÃO**: Rotação periódica da chave secreta
- ❌ **ATENÇÃO**: Nunca armazenar dados sensíveis no JWT (são decodificáveis!)

#### 6.5 Session Management
**Adicionar tracking de sessões ativas**:
```csharp
public class ActiveSession
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string DeviceInfo { get; set; }
    public string IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsActive { get; set; }
}

// Permitir usuário:
- Ver todas as sessões ativas
- Revogar sessões específicas
- "Logout de todos os dispositivos"
```

---

## 7. 🔒 Proteção de Dados Sensíveis

### 📊 Status Atual
- Senha hasheada com BCrypt: ✅
- HTTPS configurado (produção): ✅
- Dados médicos armazenados em texto claro: ⚠️

### 🔧 Sugestões de Melhorias

#### 7.1 Criptografia de Dados em Repouso
**Dados que DEVEM ser criptografados**:
- Prontuários médicos completos
- Prescrições
- Documentos (CPF, RG, CNS)
- Números de cartão de crédito (se armazenados)
- Informações de saúde mental
- Resultados de exames

**Implementação sugerida**:
```csharp
public class EncryptionService
{
    // AES-256-GCM para criptografia
    public string Encrypt(string plainText, string keyId)
    {
        // Usar chave do Azure Key Vault ou AWS KMS
        // Retornar: IV + CipherText + Tag
    }

    public string Decrypt(string cipherText, string keyId)
    {
        // Descriptografar usando chave correta
    }
}

// Na entidade
public class MedicalRecord
{
    public string EncryptedNotes { get; set; }  // Criptografado no banco
    
    [NotMapped]
    public string Notes  // Propriedade descriptografada em memória
    {
        get => _encryptionService.Decrypt(EncryptedNotes, "medical-records-key");
        set => EncryptedNotes = _encryptionService.Encrypt(value, "medical-records-key");
    }
}
```

#### 7.2 Key Management (Gerenciamento de Chaves)
**NÃO fazer**:
- ❌ Chaves hardcoded no código
- ❌ Chaves em appsettings.json (produção)
- ❌ Chaves commitadas no git

**Fazer**:
- ✅ **Azure Key Vault** (recomendado para Azure)
- ✅ **AWS KMS** (Key Management Service)
- ✅ **HashiCorp Vault**
- ✅ **Variáveis de ambiente** (mínimo aceitável)

```csharp
// Integração com Azure Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Acesso a secrets
var jwtSecret = builder.Configuration["JwtSecretKey"];
```

#### 7.3 Transparent Data Encryption (TDE)
**Nível de banco de dados**:
- SQL Server: Habilitar TDE
- Azure SQL: TDE habilitado por padrão
- PostgreSQL: Usar pgcrypto ou criptografia de disco

```sql
-- SQL Server TDE
USE master;
GO
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '<senha-forte>';
GO
CREATE CERTIFICATE TDECert WITH SUBJECT = 'TDE Certificate';
GO
USE MedicWarehouse;
GO
CREATE DATABASE ENCRYPTION KEY
WITH ALGORITHM = AES_256
ENCRYPTION BY SERVER CERTIFICATE TDECert;
GO
ALTER DATABASE MedicWarehouse SET ENCRYPTION ON;
GO
```

#### 7.4 Mascaramento de Dados (Data Masking)
**Para logs e telemetria**:
```csharp
public class DataMasker
{
    public static string MaskCPF(string cpf)
    {
        // 123.456.789-00 -> ***.***.***-00
    }
    
    public static string MaskEmail(string email)
    {
        // joao@example.com -> j***@example.com
    }
    
    public static string MaskCreditCard(string cc)
    {
        // 1234 5678 9012 3456 -> **** **** **** 3456
    }
}

// Usar em logs
_logger.LogInformation($"User {DataMasker.MaskEmail(email)} logged in");
```

#### 7.5 Data Loss Prevention (DLP)
- Impedir cópia de dados sensíveis em endpoints não seguros
- Watermark em relatórios exportados
- Limitar exportação em massa
- Alertar sobre downloads suspeitos (volume, frequência)

#### 7.6 Backup Encryption
- Todos os backups devem ser criptografados
- Testar restauração de backups regularmente
- Armazenar backups em local geograficamente separado
- Backup imutável (WORM - Write Once Read Many)

---

## 8. 📎 Segurança de Upload de Arquivos

### 📊 Status Atual
- Upload de arquivos não foi identificado no código analisado: ⚠️

### 🔧 Sugestões de Melhorias (Caso haja upload)

#### 8.1 Validação de Tipo de Arquivo
```csharp
public class FileUploadValidator
{
    private static readonly HashSet<string> AllowedExtensions = new()
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"
    };

    private static readonly HashSet<string> AllowedMimeTypes = new()
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "application/msword"
    };

    public bool IsValid(IFormFile file)
    {
        // 1. Verificar extensão
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
            return false;

        // 2. Verificar MIME type
        if (!AllowedMimeTypes.Contains(file.ContentType))
            return false;

        // 3. Verificar magic bytes (cabeçalho do arquivo)
        if (!VerifyFileSignature(file))
            return false;

        // 4. Verificar tamanho
        if (file.Length > 10 * 1024 * 1024)  // 10 MB
            return false;

        return true;
    }
}
```

#### 8.2 Scan de Antivírus
**Integração com**:
- ClamAV (open source)
- Windows Defender (via API)
- VirusTotal API
- MetaDefender

```csharp
public interface IAntivirusScanner
{
    Task<ScanResult> ScanFileAsync(Stream fileStream, string fileName);
}

// Usar antes de salvar arquivo
var scanResult = await _antivirusScanner.ScanFileAsync(file.OpenReadStream(), file.FileName);
if (!scanResult.IsClean)
{
    _logger.LogWarning($"Malicious file detected: {file.FileName}");
    return BadRequest("File contains malware");
}
```

#### 8.3 Armazenamento Seguro
**NÃO fazer**:
- ❌ Salvar na pasta wwwroot (acesso direto)
- ❌ Usar nome de arquivo original
- ❌ Armazenar no mesmo servidor da aplicação

**Fazer**:
- ✅ Usar Azure Blob Storage / AWS S3
- ✅ Gerar nome único (GUID)
- ✅ Armazenar metadados separadamente
- ✅ Usar SAS tokens / pre-signed URLs para acesso

```csharp
public class SecureFileStorage
{
    public async Task<string> UploadFileAsync(IFormFile file, string userId)
    {
        // Gerar nome único
        var fileId = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(file.FileName);
        var storageFileName = $"{fileId}{extension}";

        // Upload para blob storage
        var blobClient = _blobContainerClient.GetBlobClient(storageFileName);
        await blobClient.UploadAsync(file.OpenReadStream());

        // Salvar metadados no banco
        var fileMetadata = new FileMetadata
        {
            Id = fileId,
            OriginalFileName = file.FileName,
            StorageFileName = storageFileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            UploadedBy = userId,
            UploadedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(fileMetadata);

        return fileId;
    }

    public async Task<Stream> DownloadFileAsync(string fileId, string userId)
    {
        // Verificar permissão
        var metadata = await _repository.GetByIdAsync(fileId);
        if (!await _authService.CanAccessFile(userId, metadata))
            throw new UnauthorizedAccessException();

        // Baixar do blob storage
        var blobClient = _blobContainerClient.GetBlobClient(metadata.StorageFileName);
        return await blobClient.OpenReadAsync();
    }
}
```

#### 8.4 Proteção contra Path Traversal
```csharp
public string SanitizeFileName(string fileName)
{
    // Remover caracteres perigosos
    fileName = Path.GetFileName(fileName);  // Remove path
    fileName = fileName.Replace("..", "");
    fileName = Regex.Replace(fileName, @"[^\w\.]", "_");
    return fileName;
}
```

---

## 9. 🛡️ Proteção contra CSRF

### 📊 Status Atual
- API REST sem proteção CSRF específica: ⚠️
- JWT em Authorization header: ✅ (naturalmente protegido)

### 🔧 Sugestões de Melhorias

#### 9.1 Análise de Necessidade
**API REST com JWT**: Naturalmente protegida se:
- ✅ Token enviado em Authorization header (não em cookie)
- ✅ SameSite cookies se usar cookies
- ✅ CORS configurado corretamente

**Onde CSRF é risco**:
- Cookies de sessão (não usado atualmente)
- Formulários web tradicionais (não aplicável a SPA)

#### 9.2 Anti-CSRF Token (se necessário)
```csharp
// Apenas se adicionar formulários MVC tradicionais
[ValidateAntiForgeryToken]
public class FormController : Controller
{
    [HttpPost]
    public IActionResult Submit(FormModel model)
    {
        // Protegido automaticamente
    }
}
```

#### 9.3 SameSite Cookies
```csharp
// Se usar cookies de autenticação
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
```

#### 9.4 Custom Request Headers
**Proteção adicional**:
```typescript
// Angular - Adicionar header personalizado em todas as requests
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const cloned = req.clone({
      headers: req.headers.set('X-Requested-With', 'XMLHttpRequest')
    });
    return next.handle(cloned);
  }
}
```

```csharp
// Backend - Validar header
if (context.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
{
    return Unauthorized();
}
```

---

## 10. 👁️ Monitoramento e Detecção de Intrusões

### 📊 Status Atual
- Logging básico: ✅
- Sem sistema de alertas: ❌
- Sem SIEM: ❌

### 🔧 Sugestões de Melhorias

#### 10.1 SIEM (Security Information and Event Management)
**Soluções open source**:
- **Wazuh**: IDS/IPS, monitoramento de integridade, compliance
- **OSSEC**: Host-based intrusion detection
- **ELK Stack + Security**: Elasticsearch + Kibana com Security plugin
- **Graylog**: Gerenciamento centralizado de logs

**Soluções comerciais**:
- Splunk Enterprise Security
- IBM QRadar
- Microsoft Sentinel (Azure)
- AWS Security Hub

#### 10.2 Detecção de Anomalias com Machine Learning
```csharp
// Padrões a detectar:
- Acessos em horários incomuns
- Volume anormal de requests
- Padrão de navegação suspeito
- Acesso a dados de muitos pacientes rapidamente
- Mudança de comportamento do usuário
- Logins de localizações geograficamente impossíveis
```

**Ferramentas**:
- Azure ML Anomaly Detection
- AWS GuardDuty
- Google Cloud Anomaly Detection

#### 10.3 Honeypots e Honeytokens
```csharp
// Honeypot endpoints
[ApiExplorerSettings(IgnoreApi = true)]
[HttpGet("/api/admin/users")]  // Endpoint falso
public IActionResult FakeAdminEndpoint()
{
    _logger.LogCritical($"SECURITY ALERT: Honeypot accessed from {HttpContext.Connection.RemoteIpAddress}");
    _securityService.BlockIpAsync(HttpContext.Connection.RemoteIpAddress.ToString());
    return NotFound();
}

// Honeytoken - usuário fake no banco
var honeytokenUser = new User
{
    Username = "admin_backup",
    Email = "admin@internal.local",
    // Qualquer acesso a este usuário dispara alerta
};
```

#### 10.4 Web Application Firewall (WAF)
**Soluções cloud**:
- **Cloudflare WAF**: Proteção contra OWASP Top 10, DDoS, bot protection
- **AWS WAF**: Integrado com CloudFront, ALB, API Gateway
- **Azure WAF**: Integrado com Azure Front Door, Application Gateway
- **Google Cloud Armor**: Proteção DDoS e WAF

**Open source**:
- **ModSecurity**: WAF open source
- **NAXSI**: WAF para Nginx

**Regras a implementar**:
- OWASP Core Rule Set (CRS)
- Rate limiting avançado
- Geo-blocking
- Bot detection
- SQL Injection patterns
- XSS patterns

#### 10.5 Alertas e Notificações
```csharp
public class SecurityAlertService
{
    public async Task SendAlertAsync(SecurityAlert alert)
    {
        switch (alert.Severity)
        {
            case Severity.Critical:
                await SendToOpsTeamAsync(alert);  // PagerDuty, Opsgenie
                await SendSMSAsync(alert);
                await SendEmailAsync(alert);
                await LogToSIEMAsync(alert);
                break;
            
            case Severity.High:
                await SendToOpsTeamAsync(alert);
                await SendEmailAsync(alert);
                await LogToSIEMAsync(alert);
                break;
            
            case Severity.Medium:
                await SendEmailAsync(alert);
                await LogToSIEMAsync(alert);
                break;
            
            case Severity.Low:
                await LogToSIEMAsync(alert);
                break;
        }
    }
}
```

**Canais de notificação**:
- Email
- SMS
- Slack/Microsoft Teams
- PagerDuty
- Webhook customizado

---

## 11. 🏗️ Hardening da Infraestrutura

### 📊 Status Atual
- Docker configurado: ✅
- Produção não analisada: ⚠️

### 🔧 Sugestões de Melhorias

#### 11.1 Docker Security
```dockerfile
# Dockerfile security best practices

# 1. Use imagem base mínima
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

# 2. Não rodar como root
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# 3. Scan de vulnerabilidades
# docker scan medicwarehouse-api:latest

# 4. Multi-stage build para imagem menor
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ... build steps

FROM base AS final
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MedicSoft.Api.dll"]

# 5. Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# 6. Limitar recursos
```

```yaml
# docker-compose.yml security
services:
  api:
    image: medicwarehouse-api
    read_only: true  # Filesystem read-only
    security_opt:
      - no-new-privileges:true
    cap_drop:
      - ALL
    cap_add:
      - NET_BIND_SERVICE
    resources:
      limits:
        cpus: '2'
        memory: 2G
    networks:
      - backend
    secrets:
      - db_password
      - jwt_secret
```

#### 11.2 Kubernetes Security (se aplicável)
```yaml
# Pod Security Standards
apiVersion: v1
kind: Pod
metadata:
  name: medicwarehouse-api
spec:
  securityContext:
    runAsNonRoot: true
    runAsUser: 1000
    fsGroup: 2000
    seccompProfile:
      type: RuntimeDefault
  containers:
  - name: api
    image: medicwarehouse-api:latest
    securityContext:
      allowPrivilegeEscalation: false
      readOnlyRootFilesystem: true
      capabilities:
        drop:
          - ALL
    resources:
      limits:
        memory: "2Gi"
        cpu: "1000m"
      requests:
        memory: "1Gi"
        cpu: "500m"
```

**Ferramentas de análise**:
- kube-bench: Verifica configurações do CIS Kubernetes Benchmark
- kube-hunter: Procura vulnerabilidades em clusters
- Falco: Runtime security monitoring

#### 11.3 Network Segmentation
```
┌─────────────────────────────────────────┐
│            Internet                      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│          WAF / CDN                       │
│      (Cloudflare/AWS WAF)                │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│       Load Balancer                      │
│    (Azure LB / AWS ALB)                  │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│     DMZ (Public Subnet)                  │
│   ┌────────────────────┐                 │
│   │  API Gateway       │                 │
│   │  (Rate Limiting)   │                 │
│   └────────────────────┘                 │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│   Application Tier (Private Subnet)     │
│   ┌────────────────────┐                 │
│   │  API Servers       │                 │
│   │  (Containers)      │                 │
│   └────────────────────┘                 │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│   Data Tier (Isolated Subnet)           │
│   ┌────────────────────┐                 │
│   │  SQL Server        │                 │
│   │  (Private Only)    │                 │
│   └────────────────────┘                 │
└──────────────────────────────────────────┘
```

#### 11.4 Database Hardening
**SQL Server Security Checklist**:
- ✅ Desabilitar protocolo TCP/IP se não usado
- ✅ Habilitar SSL/TLS para conexões
- ✅ Firewall configurado (apenas IPs da aplicação)
- ✅ Autenticação Windows quando possível
- ✅ Senhas fortes para contas SQL
- ✅ Desabilitar xp_cmdshell e procedures perigosas
- ✅ Auditoria de SQL Server habilitada
- ✅ Transparent Data Encryption (TDE)
- ✅ Backup automatizado e testado
- ✅ Patch management atualizado

#### 11.5 Secrets Management
**Rotação automática de secrets**:
```csharp
// Azure Key Vault com rotação automática
public class SecretRotationService
{
    public async Task RotateSecretsAsync()
    {
        // 1. Gerar novo secret
        var newJwtSecret = GenerateSecureSecret(256);
        
        // 2. Armazenar no Key Vault com nova versão
        await _keyVaultClient.SetSecretAsync("JwtSecret", newJwtSecret);
        
        // 3. Atualizar aplicação (rolling restart)
        await _deploymentService.RollingRestartAsync();
        
        // 4. Versão antiga continua válida por período de graça
        // 5. Após período, invalida versão antiga
    }
}
```

**Frequência de rotação sugerida**:
- JWT Secret: 90 dias
- Database passwords: 180 dias
- API Keys: 30-90 dias
- Certificados SSL: Antes da expiração

#### 11.6 Principle of Least Privilege
**Azure RBAC Example**:
```json
{
  "Api-Container-Instance": {
    "permissions": [
      "Key Vault: Get secrets",
      "SQL Database: Connect",
      "Blob Storage: Read/Write",
      "Application Insights: Write telemetry"
    ],
    "not-allowed": [
      "Delete databases",
      "Manage Key Vault",
      "Create resources"
    ]
  }
}
```

---

## 12. 📋 Conformidade com LGPD e HIPAA

### 📊 Status Atual
- Sistema para área médica: ⚠️ Alta criticidade
- Multi-tenancy implementado: ✅
- Auditoria completa: ❌

### 🔧 Sugestões de Melhorias

#### 12.1 LGPD (Lei Geral de Proteção de Dados)
**Requisitos principais**:

1. **Consentimento**:
```csharp
public class ConsentManagement
{
    // Registrar consentimento do paciente
    public async Task RecordConsentAsync(PatientConsent consent)
    {
        consent.ConsentedAt = DateTime.UtcNow;
        consent.IpAddress = _httpContext.Connection.RemoteIpAddress;
        await _repository.AddAsync(consent);
    }
}

public class PatientConsent
{
    public Guid PatientId { get; set; }
    public ConsentType Type { get; set; }  // DataProcessing, Marketing, Research
    public bool IsGranted { get; set; }
    public DateTime ConsentedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string IpAddress { get; set; }
}
```

2. **Direito ao Esquecimento**:
```csharp
[HttpDelete("patients/{id}/gdpr-deletion")]
public async Task<ActionResult> RequestDeletion(Guid id)
{
    // 1. Verificar se pode deletar (regulamentações médicas)
    // 2. Anonimizar dados ao invés de deletar (manter histórico médico)
    // 3. Registrar solicitação
    await _gdprService.ProcessDeletionRequestAsync(id);
}

public class GdprService
{
    public async Task ProcessDeletionRequestAsync(Guid patientId)
    {
        // Anonimizar dados pessoais
        patient.Name = "ANONYMIZED";
        patient.Document = "***.***.***-**";
        patient.Email = $"deleted-{Guid.NewGuid()}@anonymized.local";
        patient.Phone = "***********";
        patient.IsDeleted = true;
        patient.DeletedAt = DateTime.UtcNow;
        
        // Manter dados médicos agregados para pesquisa
    }
}
```

3. **Direito de Portabilidade**:
```csharp
[HttpGet("patients/{id}/export")]
public async Task<ActionResult> ExportPatientData(Guid id)
{
    // Exportar todos os dados em formato estruturado (JSON/XML)
    var export = new PatientDataExport
    {
        PersonalData = patient,
        MedicalRecords = records,
        Appointments = appointments,
        Prescriptions = prescriptions,
        ExportedAt = DateTime.UtcNow
    };
    
    return File(JsonSerializer.Serialize(export), "application/json", "patient-data.json");
}
```

4. **Relatório de Impacto (RIPD)**:
- Documentar fluxos de dados pessoais
- Identificar riscos de processamento
- Medidas de mitigação implementadas
- Revisar anualmente

#### 12.2 HIPAA Compliance (se aplicar nos EUA)
**Requisitos principais**:

1. **Administrative Safeguards**:
- ✅ Política de segurança documentada
- ✅ Treinamento de funcionários
- ✅ Disaster recovery plan
- ✅ Incident response plan

2. **Physical Safeguards**:
- ✅ Data centers certificados (Azure/AWS compliance)
- ✅ Backup em múltiplas localizações
- ✅ Destruição segura de hardware

3. **Technical Safeguards**:
- ✅ Unique user identification (implementado)
- ✅ Automatic logoff (expiração de token)
- ⚠️ Encryption at rest (sugerido anteriormente)
- ✅ Encryption in transit (HTTPS)
- ⚠️ Audit controls (sugerido anteriormente)
- ✅ Access control (implementado)

4. **Business Associate Agreement (BAA)**:
- Necessário com provedores de cloud (Azure/AWS)
- Necessário com serviços de terceiros (Twilio, SendGrid, etc)

#### 12.3 Outras Regulamentações Médicas no Brasil

**CFM (Conselho Federal de Medicina)**:
- Resolução CFM 2.299/2021: Telemedicina
- Resolução CFM 1.821/2007: Prontuário eletrônico
- Resolução CFM 2.314/2022: Imagem do paciente

**Requisitos**:
```csharp
public class ElectronicMedicalRecord
{
    // Requisitos do CFM
    public DateTime CreatedAt { get; set; }
    public string ResponsibleDoctorCRM { get; set; }  // Médico responsável
    public bool IsDigitallySigned { get; set; }  // Assinatura digital
    public string DigitalSignature { get; set; }  // ICP-Brasil
    public bool IsLocked { get; set; }  // Prontuário não pode ser alterado após assinatura
    public DateTime? LockedAt { get; set; }
}
```

**Assinatura Digital ICP-Brasil**:
- Certificado digital A1 ou A3
- Timestamping para validade legal
- Integração com HSM (Hardware Security Module)

#### 12.4 Privacy by Design
**Princípios a seguir**:
1. Proativo, não reativo
2. Privacidade como padrão
3. Privacidade incorporada no design
4. Funcionalidade total - soma positiva
5. Segurança end-to-end
6. Visibilidade e transparência
7. Respeito pela privacidade do usuário

#### 12.5 Data Minimization
```csharp
// Coletar apenas dados necessários
public class PatientRegistration
{
    // Necessário
    public string Name { get; set; }
    public string Document { get; set; }
    public DateTime BirthDate { get; set; }
    
    // Opcional - coletar apenas se necessário
    public string? SocialSecurityNumber { get; set; }
    public string? MotherName { get; set; }
    
    // Não coletar dados irrelevantes
    // public string? FavoriteColor { get; set; }  ❌
}
```

#### 12.6 Privacy Policy e Terms of Service
**Documentos obrigatórios**:
- [ ] Política de Privacidade detalhada
- [ ] Termos de Uso do sistema
- [ ] Termo de Consentimento para tratamento de dados
- [ ] Política de Cookies (se aplicável)
- [ ] Notificação de mudanças nas políticas

**Conteúdo mínimo da Política de Privacidade**:
- Dados coletados
- Finalidade da coleta
- Base legal (LGPD)
- Compartilhamento com terceiros
- Prazo de retenção
- Direitos do titular
- Contato do DPO (Data Protection Officer)
- Medidas de segurança implementadas

---

## 📝 Resumo Executivo - Priorização

### 🔴 Prioridade CRÍTICA (Implementar imediatamente)
1. **Sistema de auditoria completo** (LGPD/CFM compliance)
2. **Criptografia de dados médicos em repouso**
3. **Bloqueio de conta por tentativas falhadas**
4. **Key Vault para gerenciamento de secrets**
5. **Backup encryption e disaster recovery**

### 🟠 Prioridade ALTA (Implementar em 1-3 meses)
1. **WAF (Web Application Firewall)**
2. **SIEM para centralização de logs**
3. **Refresh token pattern**
4. **MFA obrigatório para administradores**
5. **Pentest profissional semestral**
6. **Detecção de anomalias com ML**

### 🟡 Prioridade MÉDIA (Implementar em 3-6 meses)
1. **IP blocking e geo-blocking**
2. **Scan de antivírus em uploads**
3. **SAST/DAST no CI/CD**
4. **Honeypots e honeytokens**
5. **Session management tracking**
6. **Dependency scanning automatizado**

### 🟢 Prioridade BAIXA (Nice to have)
1. **Bug bounty program**
2. **IAST (Interactive testing)**
3. **Advanced threat protection com ML**
4. **Kubernetes security (se migrar)**
5. **Token binding por dispositivo**

---

## 🎯 Próximos Passos Recomendados

### Fase 1: Assessment (1-2 semanas)
- [ ] Contratar pentest profissional
- [ ] Executar SAST/DAST em todo codebase
- [ ] Audit de configurações atuais
- [ ] Gap analysis de compliance (LGPD/CFM)

### Fase 2: Quick Wins (1 mês)
- [ ] Implementar bloqueio de conta por tentativas
- [ ] Configurar Key Vault (Azure/AWS)
- [ ] Adicionar auditoria básica de segurança
- [ ] Configurar alertas críticos
- [ ] Documentar políticas de privacidade

### Fase 3: Hardening (2-3 meses)
- [ ] Implementar WAF
- [ ] Criptografar dados sensíveis
- [ ] Configurar SIEM
- [ ] Adicionar refresh tokens
- [ ] MFA obrigatório para admins

### Fase 4: Advanced Protection (3-6 meses)
- [ ] IP blocking e geo-blocking
- [ ] Detecção de anomalias
- [ ] Compliance audit completo
- [ ] Certificações (ISO 27001, SOC 2)
- [ ] Bug bounty program

---

## 📚 Recursos Adicionais

### Frameworks e Padrões de Segurança
- **OWASP Top 10**: https://owasp.org/www-project-top-ten/
- **OWASP API Security Top 10**: https://owasp.org/www-project-api-security/
- **CIS Benchmarks**: https://www.cisecurity.org/cis-benchmarks/
- **NIST Cybersecurity Framework**: https://www.nist.gov/cyberframework
- **ISO/IEC 27001**: Information security management

### Regulamentações
- **LGPD**: https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm
- **CFM Resoluções**: https://portal.cfm.org.br/
- **HIPAA**: https://www.hhs.gov/hipaa/

### Ferramentas Gratuitas
- **OWASP ZAP**: https://www.zaproxy.org/
- **SonarQube Community**: https://www.sonarqube.org/
- **Wazuh**: https://wazuh.com/
- **ClamAV**: https://www.clamav.net/
- **ModSecurity**: https://github.com/SpiderLabs/ModSecurity

### Treinamentos
- **OWASP Secure Coding Practices**
- **Microsoft Security Development Lifecycle (SDL)**
- **SANS Security Training**
- **Certified Information Systems Security Professional (CISSP)**

---

## 🤝 Contribuições

Este documento deve ser revisado e atualizado:
- Após cada pentest
- Quando novas vulnerabilidades forem descobertas
- Após mudanças significativas na arquitetura
- Anualmente no mínimo

**Responsável pela segurança**: [Definir DPO/CISO/Security Lead]

---

**Data de criação**: 15/10/2025  
**Última atualização**: 15/10/2025  
**Versão**: 1.0  
**Status**: Sugestões aguardando aprovação para implementação
