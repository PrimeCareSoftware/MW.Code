# 📝 Assinatura Digital ICP-Brasil - Documentação Técnica

## 📖 Visão Geral

Sistema completo de assinatura digital compatível com ICP-Brasil para documentos médicos, garantindo validade jurídica e conformidade com CFM 1.821/2007 e CFM 1.638/2002.

## 🎯 Conformidade Legal

- **CFM 1.821/2007:** Prontuários eletrônicos com assinatura digital ICP-Brasil
- **CFM 1.638/2002:** Receitas médicas digitais com assinatura
- **MP 2.200-2/2001:** ICP-Brasil para validade jurídica
- **RFC 3161:** Timestamp Authority Protocol (carimbo de tempo)
- **PKCS#7:** Formato de assinatura digital (SignedCms)

## 🏗️ Arquitetura

### Camadas

```
┌─────────────────────────────────────────────┐
│          Controllers / API Layer            │
├─────────────────────────────────────────────┤
│        Application Services Layer           │
│  - AssinaturaDigitalService                 │
│  - CertificateManager                       │
│  - TimestampService                         │
├─────────────────────────────────────────────┤
│            Domain Layer                     │
│  - CertificadoDigital                       │
│  - AssinaturaDigital                        │
├─────────────────────────────────────────────┤
│         Repository Layer                    │
│  - CertificadoDigitalRepository             │
│  - AssinaturaDigitalRepository              │
├─────────────────────────────────────────────┤
│          Database (PostgreSQL)              │
└─────────────────────────────────────────────┘
```

## 📦 Entidades de Domínio

### CertificadoDigital

Representa um certificado digital ICP-Brasil (A1 ou A3).

**Propriedades:**
- `Id` (Guid): Identificador único
- `MedicoId` (Guid): ID do médico proprietário
- `Tipo` (TipoCertificado): A1 (software) ou A3 (token/smartcard)
- `NumeroCertificado` (string): Número serial do certificado
- `SubjectName` (string): Subject do certificado (CN)
- `IssuerName` (string): Emissor do certificado
- `Thumbprint` (string): Impressão digital única
- `CertificadoA1Criptografado` (byte[]?): Certificado A1 criptografado
- `ChavePrivadaA1Criptografada` (byte[]?): Chave privada A1 criptografada
- `DataEmissao` (DateTime): Data de emissão
- `DataExpiracao` (DateTime): Data de expiração
- `Valido` (bool): Status de validade
- `TotalAssinaturas` (int): Contador de assinaturas realizadas

**Métodos:**
- `IncrementarAssinaturas()`: Incrementa contador de assinaturas
- `Invalidar()`: Marca certificado como inválido
- `Revalidar()`: Revalida certificado (se não expirado)
- `IsExpirado()`: Verifica se está expirado
- `DiasParaExpiracao()`: Calcula dias restantes

### AssinaturaDigital

Representa uma assinatura digital em um documento médico.

**Propriedades:**
- `Id` (Guid): Identificador único
- `DocumentoId` (Guid): ID do documento assinado
- `TipoDocumento` (TipoDocumento): Tipo (Prontuário, Receita, Atestado, etc.)
- `MedicoId` (Guid): ID do médico que assinou
- `CertificadoId` (Guid): ID do certificado utilizado
- `DataHoraAssinatura` (DateTime): Momento da assinatura
- `AssinaturaDigitalBytes` (byte[]): Assinatura PKCS#7
- `HashDocumento` (string): Hash SHA-256 do documento
- `TemTimestamp` (bool): Indica se tem carimbo de tempo
- `DataTimestamp` (DateTime?): Data do timestamp
- `TimestampBytes` (byte[]?): Bytes do timestamp RFC 3161
- `Valida` (bool): Status de validação
- `DataUltimaValidacao` (DateTime?): Última validação
- `LocalAssinatura` (string): Local onde foi assinado
- `IpAssinatura` (string): IP de origem

**Métodos:**
- `AtualizarValidacao(bool valida)`: Atualiza status de validação
- `Invalidar()`: Marca assinatura como inválida

### Enums

```csharp
public enum TipoCertificado
{
    A1 = 1,  // Armazenado em software (1 ano validade)
    A3 = 3   // Armazenado em token/smartcard (3-5 anos validade)
}

public enum TipoDocumento
{
    Prontuario = 1,
    Receita = 2,
    Atestado = 3,
    Laudo = 4,
    Prescricao = 5,
    Encaminhamento = 6
}
```

## 🔧 Serviços

### CertificateManager

Gerencia certificados digitais ICP-Brasil.

**Métodos principais:**

#### ImportarCertificadoA1Async
Importa um certificado A1 de um arquivo PFX.

```csharp
Task<CertificadoDigital> ImportarCertificadoA1Async(
    Guid medicoId, 
    string tenantId, 
    byte[] pfxBytes, 
    string senha);
```

**Fluxo:**
1. Valida arquivo PFX e senha
2. Verifica se é ICP-Brasil
3. Valida data de expiração
4. Invalida certificado anterior (se existir)
5. Criptografa certificado e chave privada (AES-256-GCM)
6. Salva no banco de dados

#### ListarCertificadosA3Disponiveis
Lista certificados A3 disponíveis no Windows Certificate Store.

```csharp
Task<List<CertificateInfo>> ListarCertificadosA3Disponiveis();
```

**Retorna:** Lista de certificados ICP-Brasil válidos com chave privada.

#### RegistrarCertificadoA3Async
Registra um certificado A3 pelo thumbprint.

```csharp
Task<CertificadoDigital> RegistrarCertificadoA3Async(
    Guid medicoId, 
    string tenantId, 
    string thumbprint);
```

**Nota:** A3 não armazena bytes criptografados, apenas metadados.

#### CarregarCertificadoAsync
Carrega um certificado para uso em assinatura.

```csharp
Task<X509Certificate2> CarregarCertificadoAsync(
    CertificadoDigital certificado, 
    string? senha = null);
```

**Comportamento:**
- **A1:** Descriptografa e carrega do banco
- **A3:** Busca no Windows Certificate Store (requer token conectado)

### TimestampService

Gerencia carimbos de tempo (timestamps) de Autoridades Certificadoras ICP-Brasil.

**TSAs suportadas:**
- `timestamp.iti.gov.br` (ITI - Instituto Nacional de Tecnologia da Informação)
- `tsa.certisign.com.br` (Certisign)
- `validcertificadora.com.br` (Valid)

#### ObterTimestampAsync
Obtém um carimbo de tempo RFC 3161.

```csharp
Task<TimestampResponse> ObterTimestampAsync(string hash);
```

**Fluxo:**
1. Converte hash para bytes
2. Cria requisição RFC 3161 (ASN.1 DER encoding)
3. Envia para TSAs (com fallback automático)
4. Retorna timestamp com data e bytes

**Formato da requisição:** 
```
SEQUENCE {
  version INTEGER(1),
  messageImprint SEQUENCE {
    hashAlgorithm OID(SHA-256),
    hashedMessage OCTET STRING
  },
  certReq BOOLEAN(true),
  nonce INTEGER(random)
}
```

#### ValidarTimestampAsync
Valida a autenticidade de um timestamp.

```csharp
Task<bool> ValidarTimestampAsync(byte[] timestampBytes);
```

**Validações:**
- Estrutura ASN.1 válida
- Assinatura da TSA
- Certificado da TSA válido

### AssinaturaDigitalService

Serviço principal para assinatura e validação de documentos.

#### AssinarDocumentoAsync
Assina um documento digitalmente.

```csharp
Task<ResultadoAssinatura> AssinarDocumentoAsync(
    Guid documentoId,
    TipoDocumento tipoDocumento,
    Guid medicoId,
    byte[] documentoBytes,
    string? senhaCertificado = null);
```

**Fluxo:**
1. Busca certificado ativo do médico
2. Valida certificado (expiração, revogação)
3. Calcula hash SHA-256 do documento
4. Carrega certificado X.509
5. Assina com PKCS#7 (SignedCms)
6. Obtém timestamp da TSA
7. Registra assinatura no banco
8. Incrementa contador de assinaturas

**Algoritmos:**
- **Hash:** SHA-256
- **Assinatura:** RSA com PKCS#7
- **Timestamp:** RFC 3161

#### ValidarAssinaturaAsync
Valida uma assinatura existente.

```csharp
Task<ResultadoValidacao> ValidarAssinaturaAsync(Guid assinaturaId);
```

**Validações realizadas:**
1. **Hash do documento:**
   - Recalcula hash do documento
   - Compara com hash armazenado
   - Detecta alterações pós-assinatura

2. **Assinatura PKCS#7:**
   - Decodifica SignedCms
   - Valida assinatura com certificado
   - Verifica cadeia de certificados

3. **Certificado:**
   - Valida período de validade
   - Verifica se estava válido no momento da assinatura
   - Valida cadeia até raiz ICP-Brasil

4. **Timestamp:**
   - Valida estrutura RFC 3161
   - Verifica assinatura da TSA
   - Confirma data do carimbo

**Retorna:** `ResultadoValidacao` com status e detalhes.

#### ObterAssinaturasPorDocumentoAsync
Retorna todas as assinaturas de um documento.

```csharp
Task<List<AssinaturaDigitalDto>> ObterAssinaturasPorDocumentoAsync(
    Guid documentoId, 
    TipoDocumento tipoDocumento);
```

## 🔐 Segurança

### Criptografia de Certificados A1

Certificados A1 são armazenados criptografados usando **AES-256-GCM**.

**DataEncryptionService:**
- Algoritmo: AES-256-GCM
- Tamanho da chave: 256 bits
- Nonce: 96 bits (gerado aleatoriamente)
- Tag de autenticação: 128 bits

```csharp
byte[] EncryptBytes(byte[] plainBytes);
byte[] DecryptBytes(byte[] encryptedBytes);
```

### Validação ICP-Brasil

Certificados são validados contra Autoridades Certificadoras reconhecidas:
- AC Certisign
- AC Serasa
- AC Soluti
- Autoridade Certificadora Raiz Brasileira
- AC VALID
- AC SERPROPR

## 📊 Banco de Dados

### Tabela: CertificadosDigitais

```sql
CREATE TABLE "CertificadosDigitais" (
    "Id" uuid PRIMARY KEY,
    "MedicoId" uuid NOT NULL,
    "Tipo" integer NOT NULL,
    "NumeroCertificado" varchar(100) NOT NULL,
    "SubjectName" varchar(500) NOT NULL,
    "IssuerName" varchar(500) NOT NULL,
    "Thumbprint" varchar(100) NOT NULL UNIQUE,
    "CertificadoA1Criptografado" bytea,
    "ChavePrivadaA1Criptografada" bytea,
    "DataEmissao" timestamp NOT NULL,
    "DataExpiracao" timestamp NOT NULL,
    "Valido" boolean NOT NULL,
    "DataCadastro" timestamp NOT NULL,
    "TotalAssinaturas" integer NOT NULL DEFAULT 0,
    "TenantId" varchar(50) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp NOT NULL,
    FOREIGN KEY ("MedicoId") REFERENCES "Users"("Id")
);

CREATE INDEX "IX_CertificadosDigitais_MedicoId" ON "CertificadosDigitais"("MedicoId");
CREATE INDEX "IX_CertificadosDigitais_TenantId" ON "CertificadosDigitais"("TenantId");
```

### Tabela: AssinaturasDigitais

```sql
CREATE TABLE "AssinaturasDigitais" (
    "Id" uuid PRIMARY KEY,
    "DocumentoId" uuid NOT NULL,
    "TipoDocumento" integer NOT NULL,
    "MedicoId" uuid NOT NULL,
    "CertificadoId" uuid NOT NULL,
    "DataHoraAssinatura" timestamp NOT NULL,
    "AssinaturaDigitalBytes" bytea NOT NULL,
    "HashDocumento" varchar(100) NOT NULL,
    "TemTimestamp" boolean NOT NULL,
    "DataTimestamp" timestamp,
    "TimestampBytes" bytea,
    "Valida" boolean NOT NULL,
    "DataUltimaValidacao" timestamp,
    "LocalAssinatura" varchar(200) NOT NULL,
    "IpAssinatura" varchar(50) NOT NULL,
    "TenantId" varchar(50) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp NOT NULL,
    FOREIGN KEY ("MedicoId") REFERENCES "Users"("Id"),
    FOREIGN KEY ("CertificadoId") REFERENCES "CertificadosDigitais"("Id")
);

CREATE INDEX "IX_AssinaturasDigitais_DocumentoId" ON "AssinaturasDigitais"("DocumentoId");
CREATE INDEX "IX_AssinaturasDigitais_MedicoId" ON "AssinaturasDigitais"("MedicoId");
CREATE INDEX "IX_AssinaturasDigitais_CertificadoId" ON "AssinaturasDigitais"("CertificadoId");
CREATE INDEX "IX_AssinaturasDigitais_DocumentoId_TipoDocumento" ON "AssinaturasDigitais"("DocumentoId", "TipoDocumento");
CREATE INDEX "IX_AssinaturasDigitais_TenantId" ON "AssinaturasDigitais"("TenantId");
```

## 🚀 Uso

### 1. Importar Certificado A1

```csharp
var certificateManager = services.GetService<ICertificateManager>();
var pfxBytes = File.ReadAllBytes("certificado.pfx");
var senha = "senha_do_certificado";

var certificado = await certificateManager.ImportarCertificadoA1Async(
    medicoId: medicoGuid,
    tenantId: "tenant-123",
    pfxBytes: pfxBytes,
    senha: senha
);
```

### 2. Registrar Certificado A3

```csharp
// Listar certificados disponíveis
var certificadosDisponiveis = await certificateManager.ListarCertificadosA3Disponiveis();

// Registrar um certificado
var certificado = await certificateManager.RegistrarCertificadoA3Async(
    medicoId: medicoGuid,
    tenantId: "tenant-123",
    thumbprint: certificadosDisponiveis[0].Thumbprint
);
```

### 3. Assinar Documento

```csharp
var assinaturaService = services.GetService<IAssinaturaDigitalService>();
var documentoBytes = GerarPdfDocumento(prontuario);

var resultado = await assinaturaService.AssinarDocumentoAsync(
    documentoId: prontuarioId,
    tipoDocumento: TipoDocumento.Prontuario,
    medicoId: medicoGuid,
    documentoBytes: documentoBytes,
    senhaCertificado: "senha_a1" // Opcional, apenas para A1
);

if (resultado.Sucesso)
{
    Console.WriteLine($"Documento assinado! ID: {resultado.AssinaturaId}");
}
```

### 4. Validar Assinatura

```csharp
var resultado = await assinaturaService.ValidarAssinaturaAsync(assinaturaId);

if (resultado.Valida)
{
    Console.WriteLine($"Assinatura válida!");
    Console.WriteLine($"Assinado por: {resultado.Assinante} ({resultado.CRM})");
    Console.WriteLine($"Data: {resultado.DataAssinatura}");
}
else
{
    Console.WriteLine($"Assinatura inválida: {resultado.Motivo}");
}
```

## ⚠️ Considerações de Produção

### 1. Certificados A3 - Windows Certificate Store
- Requer que o token esteja conectado
- Funciona apenas em Windows (usar PKCS#11 para Linux)
- Pode requerer PIN do token

### 2. TSA (Timestamp Authority)
- URLs das TSAs são hard-coded
- Considerar tornar configurável via `appsettings.json`
- Implementar cache de timestamps
- Adicionar retry com backoff exponencial

### 3. ASN.1 Encoding
- Implementação simplificada de RFC 3161
- Para produção robusta, considerar:
  - Bouncy Castle Library
  - LibreSSL/OpenSSL bindings

### 4. Performance
- Assinaturas são operações custosas (criptografia RSA)
- Considerar fila assíncrona para assinaturas em lote
- Cache de certificados carregados

### 5. Revogação de Certificados
- Implementar verificação de LCR (Lista de Certificados Revogados)
- Integrar com OCSP (Online Certificate Status Protocol)

### 6. Validação de Integridade de Documentos

⚠️ **IMPORTANTE:** A validação atual verifica a estrutura PKCS#7, certificado e timestamp, mas **não valida a integridade do documento** recalculando o hash.

**Implementação necessária para produção:**

```csharp
public async Task<ResultadoValidacao> ValidarAssinaturaCompletoAsync(Guid assinaturaId)
{
    var assinatura = await _assinaturaRepository.GetAssinaturaComRelacoesAsync(assinaturaId);
    
    // 1. Recuperar documento original do storage
    byte[] documentoBytes = await _documentStorageService
        .GetDocumentoBytesAsync(assinatura.DocumentoId, assinatura.TipoDocumento);
    
    // 2. Recalcular hash SHA-256
    string hashAtual = CalcularHashSHA256(documentoBytes);
    
    // 3. Comparar com hash armazenado
    if (hashAtual != assinatura.HashDocumento)
    {
        return new ResultadoValidacao
        {
            Valida = false,
            Motivo = "Documento foi modificado após assinatura. Violação de integridade."
        };
    }
    
    // 4. Continuar com validação PKCS#7, certificado e timestamp...
}
```

**Requisitos:**
- Serviço de armazenamento de documentos (IDocumentStorageService)
- Recuperação de bytes originais do documento
- Integração com módulos de prontuário, receitas, atestados, etc.

**Por que não está implementado:**
- Requer integração com sistema de armazenamento de documentos
- Cada tipo de documento (Prontuário, Receita, Atestado) tem estrutura diferente
- Precisa de geração de PDF consistente e reproduzível
- Fora do escopo da implementação inicial do serviço de assinatura

**Recomendação:** Implementar esta validação antes de usar em produção.

## 📚 Referências

- [CFM 1.821/2007](http://www.portalmedico.org.br/resolucoes/cfm/2007/1821_2007.htm) - Prontuários eletrônicos
- [CFM 1.638/2002](http://www.portalmedico.org.br/resolucoes/cfm/2002/1638_2002.htm) - Receitas médicas
- [MP 2.200-2/2001](http://www.planalto.gov.br/ccivil_03/mpv/antigas_2001/2200-2.htm) - ICP-Brasil
- [RFC 3161](https://www.ietf.org/rfc/rfc3161.txt) - Time-Stamp Protocol
- [PKCS#7](https://datatracker.ietf.org/doc/html/rfc2315) - Cryptographic Message Syntax
- [ICP-Brasil](https://www.gov.br/iti/pt-br/assuntos/icp-brasil) - Infraestrutura de Chaves Públicas Brasileira

## 🔍 Troubleshooting

### Erro: "Certificado ou senha inválidos"
- Verifique se o arquivo PFX está correto
- Confirme a senha do certificado
- Teste abrindo o certificado no Windows

### Erro: "Token A3 não está conectado"
- Conecte o token USB
- Instale drivers do token
- Verifique se o certificado aparece no Windows Certificate Store

### Erro: "Não foi possível obter timestamp"
- Verifique conectividade com internet
- TSAs podem estar temporariamente indisponíveis
- Sistema tenta 3 TSAs automaticamente

### Erro: "Certificado expirado"
- Renovar certificado junto à Autoridade Certificadora
- Certificados A1: validade de 1 ano
- Certificados A3: validade de 3-5 anos

## 📞 Suporte

Para dúvidas ou problemas:
1. Consulte a documentação do ICP-Brasil
2. Verifique logs da aplicação
3. Entre em contato com suporte técnico
