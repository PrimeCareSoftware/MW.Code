# 💊 Prescrições Digitais - Finalização (20% Restante)

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (CFM 1.643/2002 + ANVISA 344/98)  
**Status Atual:** 80% completo (Janeiro 2026)  
**Esforço Restante:** 2 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 30.000  
**Prazo:** Q1-Q2 2026 (Fevereiro-Abril)

## 📋 Contexto

### ✅ O que já foi implementado (80%)

**Backend - 100% Completo:**
- ✅ Entidades criadas: `DigitalPrescription`, `DigitalPrescriptionItem`, `SNGPCReport`, `PrescriptionSequenceControl`
- ✅ 5 tipos de receita implementados: Simples, Controladas A/B/C1, Antimicrobiana
- ✅ Validações ANVISA por tipo e substância
- ✅ Controle sequencial de numeração para controladas
- ✅ Sistema SNGPC para reporting mensal
- ✅ QR Code para verificação de autenticidade
- ✅ API RESTful completa com 15+ endpoints

**Frontend - 100% Completo:**
- ✅ 4 componentes production-ready (~2.236 linhas):
  - `DigitalPrescriptionFormComponent` (~950 linhas)
  - `DigitalPrescriptionViewComponent` (~700 linhas)
  - `PrescriptionTypeSelectorComponent` (~210 linhas)
  - `SNGPCDashboardComponent` (~376 linhas)
- ✅ Seleção visual de tipo de receita com compliance info
- ✅ Autocomplete de medicamentos integrado
- ✅ Alertas para medicamentos controlados
- ✅ Preview antes de finalizar
- ✅ Layout otimizado para impressão

**Documentação - 100% Completa:**
- ✅ `docs/DIGITAL_PRESCRIPTIONS.md`
- ✅ `docs/IMPLEMENTACAO_PENDENTE_CFM_PRESCRICOES.md`

### ⏳ O que falta (20%)

1. **PDF de Receita Profissional com Templates** (40% do trabalho restante)
   - Criar templates PDF para cada tipo de receita
   - Layout profissional seguindo normas ANVISA/CFM
   - Incluir QR Code, marcas d'água, identificação médica
   - Suporte a impressão A4 e meia-folha

2. **Assinatura Digital ICP-Brasil** (35% do trabalho restante)
   - Integração com certificados ICP-Brasil A1/A3
   - Assinatura digital do PDF
   - Validação de certificados
   - Timestamp confiável

3. **Geração Completa XML ANVISA v2.1** (15% do trabalho restante)
   - Exportação XML conforme schema ANVISA v2.1
   - Validação contra XSD oficial
   - Preparação para envio ao SNGPC

4. **Testes com Farmácias Reais** (10% do trabalho restante)
   - Validar aceitação em redes de farmácias
   - Testes de leitura de QR Code
   - Feedback e ajustes finais

## 🎯 Objetivos da Tarefa

Completar a implementação de prescrições digitais com PDFs profissionais, assinatura digital ICP-Brasil e compatibilidade total com farmácias, garantindo conformidade com CFM 1.643/2002 e ANVISA 344/98.

## 📝 Tarefas Detalhadas

### 1. Templates PDF para Receitas (3 semanas)

#### 1.1 Biblioteca de Geração de PDF
```csharp
// Adicionar pacote NuGet
// Install-Package iTextSharp ou QuestPDF (recomendado - mais moderno)

public interface IPrescriptionPdfService
{
    Task<byte[]> GeneratePdfAsync(int prescriptionId, PrescriptionPdfOptions options);
    Task<byte[]> GenerateSimplePrescriptionPdfAsync(DigitalPrescription prescription);
    Task<byte[]> GenerateControlledPrescriptionPdfAsync(DigitalPrescription prescription);
    Task<byte[]> GenerateAntimicrobialPrescriptionPdfAsync(DigitalPrescription prescription);
}

// Opções de geração
public class PrescriptionPdfOptions
{
    public bool IncludeHeader { get; set; } = true;
    public bool IncludeFooter { get; set; } = true;
    public bool IncludeQRCode { get; set; } = true;
    public bool IncludeWatermark { get; set; } = true;
    public PaperSize PaperSize { get; set; } = PaperSize.A4;
    public bool IncludePatientCopy { get; set; } = true; // Gerar 2 vias
}
```

#### 1.2 Template para Receita Simples
```csharp
public class SimplePrescriptionPdfGenerator
{
    public byte[] Generate(DigitalPrescription prescription)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                // Cabeçalho
                page.Header().Element(ComposeHeader);
                
                // Conteúdo
                page.Content().Element(content =>
                {
                    content.PaddingVertical(1, Unit.Centimetre);
                    
                    // Dados do Paciente
                    content.Column(column =>
                    {
                        column.Item().Text($"Paciente: {prescription.Patient.FullName}")
                            .FontSize(12).Bold();
                        column.Item().Text($"Data de Nascimento: {prescription.Patient.BirthDate:dd/MM/yyyy}")
                            .FontSize(10);
                        column.Item().Text($"Data: {prescription.PrescriptionDate:dd/MM/yyyy}")
                            .FontSize(10);
                        
                        column.Item().PaddingTop(1, Unit.Centimetre);
                        
                        // Medicamentos
                        column.Item().Text("Medicamentos Prescritos:")
                            .FontSize(12).Bold().Underline();
                        
                        foreach (var item in prescription.Items)
                        {
                            column.Item().PaddingTop(0.5f, Unit.Centimetre);
                            column.Item().Text($"{item.MedicationName}")
                                .FontSize(11).Bold();
                            column.Item().Text($"{item.Dosage} - {item.Quantity} {item.Unit}")
                                .FontSize(10);
                            column.Item().Text($"Uso: {item.Instructions}")
                                .FontSize(10).Italic();
                        }
                    });
                });
                
                // Rodapé
                page.Footer().Element(ComposeFooter);
            });
        });
        
        return document.GeneratePdf();
    }
    
    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            // Logo da clínica (se houver)
            row.RelativeItem(3).Column(column =>
            {
                column.Item().Text(clinic.Name).FontSize(16).Bold();
                column.Item().Text(clinic.Address).FontSize(9);
                column.Item().Text($"Tel: {clinic.Phone}").FontSize(9);
            });
            
            // QR Code
            row.RelativeItem(1).AlignRight().Image(GenerateQRCode());
        });
        
        container.PaddingTop(0.5f, Unit.Centimetre)
            .BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }
    
    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(column =>
        {
            column.Item().PaddingTop(1, Unit.Centimetre)
                .BorderTop(1).BorderColor(Colors.Grey.Lighten2);
            
            column.Item().PaddingTop(0.5f, Unit.Centimetre)
                .Text("Assinatura do Médico")
                .FontSize(10);
            
            column.Item().PaddingTop(1.5f, Unit.Centimetre)
                .LineHorizontal(1).LineColor(Colors.Black);
            
            column.Item().PaddingTop(0.3f, Unit.Centimetre)
                .Text($"Dr(a). {doctor.FullName}")
                .FontSize(10).Bold();
            
            column.Item().Text($"CRM: {doctor.CRM} - {doctor.CRMState}")
                .FontSize(9);
            
            column.Item().PaddingTop(0.5f, Unit.Centimetre)
                .Text($"Documento: {prescription.DocumentNumber}")
                .FontSize(8).Italic();
        });
    }
}
```

#### 1.3 Template para Receita Controlada (Receita Branca Especial)
```csharp
public class ControlledPrescriptionPdfGenerator
{
    public byte[] Generate(DigitalPrescription prescription)
    {
        // Receita controlada deve seguir ANVISA 344/98
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                // Marca d'água obrigatória
                page.Background()
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("RECEITA CONTROLADA")
                    .FontSize(50)
                    .FontColor(Colors.Grey.Lighten4)
                    .Bold();
                
                page.Content().Element(content =>
                {
                    content.Column(column =>
                    {
                        // Notificação de Receita (número sequencial)
                        column.Item().Text($"NOTIFICAÇÃO DE RECEITA: {prescription.SequenceNumber}")
                            .FontSize(14).Bold().FontColor(Colors.Red.Darken2);
                        
                        column.Item().Text($"TIPO: {GetControlType(prescription.ControlType)}")
                            .FontSize(12).Bold();
                        
                        // Identificação do Emitente
                        column.Item().PaddingTop(1, Unit.Centimetre);
                        column.Item().Text("IDENTIFICAÇÃO DO EMITENTE")
                            .FontSize(12).Bold().Underline();
                        
                        column.Item().Text($"Nome: {doctor.FullName}");
                        column.Item().Text($"CRM: {doctor.CRM}/{doctor.CRMState}");
                        column.Item().Text($"Endereço: {clinic.Address}");
                        column.Item().Text($"Telefone: {clinic.Phone}");
                        
                        // Identificação do Paciente
                        column.Item().PaddingTop(1, Unit.Centimetre);
                        column.Item().Text("IDENTIFICAÇÃO DO PACIENTE")
                            .FontSize(12).Bold().Underline();
                        
                        column.Item().Text($"Nome: {patient.FullName}");
                        column.Item().Text($"CPF: {patient.CPF}");
                        column.Item().Text($"Endereço: {patient.Address}");
                        column.Item().Text($"Telefone: {patient.Phone}");
                        
                        // Medicamento (apenas 1 por receita controlada)
                        column.Item().PaddingTop(1, Unit.Centimetre);
                        column.Item().Text("MEDICAMENTO PRESCRITO")
                            .FontSize(12).Bold().Underline();
                        
                        var medication = prescription.Items.First();
                        column.Item().Text($"{medication.MedicationName}")
                            .FontSize(14).Bold();
                        column.Item().Text($"Quantidade: {medication.Quantity} {medication.Unit}");
                        column.Item().Text($"Posologia: {medication.Instructions}");
                        
                        // Data e Validade
                        column.Item().PaddingTop(1, Unit.Centimetre);
                        column.Item().Text($"Data de Emissão: {prescription.PrescriptionDate:dd/MM/yyyy}")
                            .FontSize(11);
                        column.Item().Text($"Validade: {GetControlledPrescriptionValidity(prescription.ControlType)} dias")
                            .FontSize(11).Bold().FontColor(Colors.Red.Medium);
                    });
                });
                
                page.Footer().Element(ComposeControlledFooter);
            });
        });
        
        return document.GeneratePdf();
    }
    
    private string GetControlType(ControlType type)
    {
        return type switch
        {
            ControlType.A1 => "LISTA A1 (Entorpecentes)",
            ControlType.A2 => "LISTA A2 (Entorpecentes)",
            ControlType.A3 => "LISTA A3 (Psicotrópicos)",
            ControlType.B1 => "LISTA B1 (Psicotrópicos)",
            ControlType.B2 => "LISTA B2 (Psicotrópicos Anorexígenos)",
            ControlType.C1 => "LISTA C1 (Outras substâncias sujeitas a controle)",
            _ => "CONTROLADO"
        };
    }
    
    private int GetControlledPrescriptionValidity(ControlType type)
    {
        // Conforme ANVISA 344/98
        return type switch
        {
            ControlType.A1 or ControlType.A2 or ControlType.A3 => 30,
            ControlType.B1 or ControlType.B2 => 30,
            ControlType.C1 => 30,
            _ => 30
        };
    }
}
```

#### 1.4 Template para Receita Antimicrobiana (Receita Branca Comum)
```csharp
public class AntimicrobialPrescriptionPdfGenerator
{
    public byte[] Generate(DigitalPrescription prescription)
    {
        // Receita antimicrobiana conforme RDC 20/2011
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                // Marca d'água
                page.Background()
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("USO SOB ORIENTAÇÃO MÉDICA")
                    .FontSize(40)
                    .FontColor(Colors.Grey.Lighten4)
                    .Bold();
                
                page.Content().Element(content =>
                {
                    content.Column(column =>
                    {
                        // Título
                        column.Item().AlignCenter()
                            .Text("RECEITA DE ANTIMICROBIANO")
                            .FontSize(14).Bold();
                        
                        column.Item().AlignCenter()
                            .Text("RDC 20/2011 ANVISA")
                            .FontSize(10).Italic();
                        
                        // Dados do médico e paciente
                        ComposeStandardPrescriptionBody(column, prescription);
                        
                        // Avisos obrigatórios
                        column.Item().PaddingTop(1, Unit.Centimetre)
                            .Background(Colors.Yellow.Lighten4)
                            .Padding(10)
                            .Column(warnings =>
                            {
                                warnings.Item().Text("ATENÇÃO:")
                                    .FontSize(11).Bold();
                                warnings.Item().Text("• Validade: 30 dias a partir da data de emissão")
                                    .FontSize(9);
                                warnings.Item().Text("• Retenção da 2ª via pela farmácia é obrigatória")
                                    .FontSize(9);
                                warnings.Item().Text("• Não compartilhe este medicamento")
                                    .FontSize(9);
                            });
                    });
                });
                
                page.Footer().Element(ComposeFooter);
            });
        });
        
        return document.GeneratePdf();
    }
}
```

#### 1.5 Serviço de Geração de PDF
```csharp
public class PrescriptionPdfService : IPrescriptionPdfService
{
    private readonly IDigitalPrescriptionRepository _repository;
    private readonly SimplePrescriptionPdfGenerator _simpleGenerator;
    private readonly ControlledPrescriptionPdfGenerator _controlledGenerator;
    private readonly AntimicrobialPrescriptionPdfGenerator _antimicrobialGenerator;
    
    public async Task<byte[]> GeneratePdfAsync(int prescriptionId, PrescriptionPdfOptions options)
    {
        var prescription = await _repository.GetByIdWithDetailsAsync(prescriptionId);
        
        if (prescription == null)
            throw new NotFoundException($"Prescrição {prescriptionId} não encontrada");
        
        return prescription.PrescriptionType switch
        {
            PrescriptionType.Simple => _simpleGenerator.Generate(prescription),
            PrescriptionType.Controlled => _controlledGenerator.Generate(prescription),
            PrescriptionType.Antimicrobial => _antimicrobialGenerator.Generate(prescription),
            _ => throw new InvalidOperationException($"Tipo de prescrição não suportado: {prescription.PrescriptionType}")
        };
    }
}
```

#### 1.6 Endpoint de Download
```csharp
[HttpGet("{id}/pdf")]
[ProducesResponseType(typeof(FileContentResult), 200)]
public async Task<IActionResult> DownloadPdf(int id, [FromQuery] PrescriptionPdfOptions options)
{
    var pdf = await _pdfService.GeneratePdfAsync(id, options);
    var prescription = await _repository.GetByIdAsync(id);
    
    var fileName = $"receita_{prescription.DocumentNumber}_{DateTime.Now:yyyyMMdd}.pdf";
    
    return File(pdf, "application/pdf", fileName);
}

[HttpGet("{id}/pdf/preview")]
public async Task<IActionResult> PreviewPdf(int id)
{
    var pdf = await _pdfService.GeneratePdfAsync(id, new PrescriptionPdfOptions());
    return File(pdf, "application/pdf");
}
```

### 2. Assinatura Digital ICP-Brasil (3 semanas)

#### 2.1 Integração com Certificados Digitais
```csharp
// Install-Package iTextSharp.LGPLv2.Core (para assinatura)
// Install-Package System.Security.Cryptography.Pkcs

public interface IDigitalSignatureService
{
    Task<byte[]> SignPdfAsync(byte[] pdfContent, X509Certificate2 certificate);
    Task<bool> ValidateSignatureAsync(byte[] signedPdf);
    Task<SignatureInfo> GetSignatureInfoAsync(byte[] signedPdf);
}

public class SignatureInfo
{
    public string SignerName { get; set; }
    public string CPF { get; set; }
    public DateTime SignedAt { get; set; }
    public bool IsValid { get; set; }
    public string CertificateIssuer { get; set; }
    public DateTime CertificateValidFrom { get; set; }
    public DateTime CertificateValidTo { get; set; }
}

public class DigitalSignatureService : IDigitalSignatureService
{
    private readonly ILogger<DigitalSignatureService> _logger;
    
    public async Task<byte[]> SignPdfAsync(byte[] pdfContent, X509Certificate2 certificate)
    {
        if (certificate == null)
            throw new ArgumentNullException(nameof(certificate));
        
        if (!certificate.HasPrivateKey)
            throw new InvalidOperationException("Certificado não possui chave privada");
        
        // Verificar validade do certificado
        if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
            throw new InvalidOperationException("Certificado inválido ou expirado");
        
        using var reader = new PdfReader(pdfContent);
        using var outputStream = new MemoryStream();
        using var stamper = PdfStamper.CreateSignature(reader, outputStream, '\0');
        
        // Configurar aparência da assinatura
        var appearance = stamper.SignatureAppearance;
        appearance.Reason = "Assinatura digital de prescrição médica";
        appearance.Location = "Brasil";
        appearance.SignDate = DateTime.Now;
        
        // Posição da assinatura (canto inferior direito)
        appearance.SetVisibleSignature(
            new iTextSharp.text.Rectangle(400, 100, 550, 150),
            1, // primeira página
            "Signature"
        );
        
        // Criar assinatura
        var privateKey = certificate.GetRSAPrivateKey();
        var signatureCreator = new PrivateKeySignature(privateKey, "SHA-256");
        
        // Aplicar assinatura
        MakeSignature.SignDetached(
            appearance,
            signatureCreator,
            new[] { certificate },
            null, null, null, 0,
            CryptoStandard.CMS
        );
        
        _logger.LogInformation($"PDF assinado com sucesso. Certificado: {certificate.Subject}");
        
        return outputStream.ToArray();
    }
    
    public async Task<bool> ValidateSignatureAsync(byte[] signedPdf)
    {
        using var reader = new PdfReader(signedPdf);
        var af = reader.AcroFields;
        var names = af.GetSignatureNames();
        
        if (names.Count == 0)
            return false;
        
        foreach (var name in names)
        {
            var pk = af.VerifySignature(name);
            
            if (!pk.Verify())
                return false;
            
            // Verificar revogação do certificado (se necessário)
            // TODO: Implementar verificação OCSP/CRL
        }
        
        return true;
    }
    
    public async Task<SignatureInfo> GetSignatureInfoAsync(byte[] signedPdf)
    {
        using var reader = new PdfReader(signedPdf);
        var af = reader.AcroFields;
        var names = af.GetSignatureNames();
        
        if (names.Count == 0)
            return null;
        
        var name = names[0]; // Primeira assinatura
        var pk = af.VerifySignature(name);
        
        var cert = pk.SigningCertificate;
        
        return new SignatureInfo
        {
            SignerName = cert.SubjectDN.ToString(),
            CPF = ExtractCPFFromCertificate(cert),
            SignedAt = pk.SignDate,
            IsValid = pk.Verify(),
            CertificateIssuer = cert.IssuerDN.ToString(),
            CertificateValidFrom = cert.NotBefore,
            CertificateValidTo = cert.NotAfter
        };
    }
    
    private string ExtractCPFFromCertificate(X509Certificate cert)
    {
        // Extrair CPF do campo Subject Alternative Name ou OID específico
        // Certificados ICP-Brasil incluem CPF em campo específico
        var subject = cert.SubjectDN.ToString();
        var cpfMatch = System.Text.RegularExpressions.Regex.Match(subject, @"CPF=(\d{11})");
        return cpfMatch.Success ? cpfMatch.Groups[1].Value : null;
    }
}
```

#### 2.2 Gerenciamento de Certificados
```csharp
public interface ICertificateManagementService
{
    Task<List<CertificateInfo>> GetAvailableCertificatesAsync();
    Task<X509Certificate2> GetCertificateAsync(string thumbprint);
    Task<bool> UploadCertificateAsync(byte[] certificateData, string password);
    Task<bool> RemoveCertificateAsync(string thumbprint);
}

public class CertificateInfo
{
    public string Thumbprint { get; set; }
    public string SubjectName { get; set; }
    public string CPF { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsValid => DateTime.Now >= ValidFrom && DateTime.Now <= ValidTo;
    public string Issuer { get; set; }
}

public class CertificateManagementService : ICertificateManagementService
{
    private readonly IConfiguration _configuration;
    
    public async Task<List<CertificateInfo>> GetAvailableCertificatesAsync()
    {
        var certificates = new List<CertificateInfo>();
        
        // Buscar no Windows Certificate Store (A3) ou armazenamento local (A1)
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        
        foreach (var cert in store.Certificates)
        {
            // Filtrar apenas certificados ICP-Brasil com uso de assinatura digital
            if (IsICPBrasilCertificate(cert) && cert.HasPrivateKey)
            {
                certificates.Add(new CertificateInfo
                {
                    Thumbprint = cert.Thumbprint,
                    SubjectName = cert.Subject,
                    CPF = ExtractCPFFromCertificate(cert),
                    ValidFrom = cert.NotBefore,
                    ValidTo = cert.NotAfter,
                    Issuer = cert.Issuer
                });
            }
        }
        
        return certificates;
    }
    
    private bool IsICPBrasilCertificate(X509Certificate2 cert)
    {
        // Verificar se é certificado ICP-Brasil
        var issuer = cert.Issuer.ToLower();
        return issuer.Contains("icp-brasil") || 
               issuer.Contains("ac ") || 
               issuer.Contains("autoridade certificadora");
    }
}
```

#### 2.3 Endpoints de Assinatura
```csharp
[HttpPost("{id}/sign")]
public async Task<IActionResult> SignPrescription(int id, [FromBody] SignPrescriptionRequest request)
{
    var prescription = await _repository.GetByIdAsync(id);
    
    if (prescription == null)
        return NotFound();
    
    // Gerar PDF
    var pdfContent = await _pdfService.GeneratePdfAsync(id, new PrescriptionPdfOptions());
    
    // Buscar certificado
    var certificate = await _certificateService.GetCertificateAsync(request.CertificateThumbprint);
    
    if (certificate == null)
        return BadRequest("Certificado não encontrado");
    
    // Assinar
    var signedPdf = await _signatureService.SignPdfAsync(pdfContent, certificate);
    
    // Armazenar PDF assinado
    var fileName = $"prescription_{prescription.Id}_signed.pdf";
    await _fileStorageService.SaveAsync(fileName, signedPdf);
    
    // Atualizar prescrição
    prescription.IsSigned = true;
    prescription.SignedAt = DateTime.UtcNow;
    prescription.SignedByUserId = GetCurrentUserId();
    prescription.SignedPdfPath = fileName;
    
    await _repository.UpdateAsync(prescription);
    
    return Ok(new { signed = true, path = fileName });
}

[HttpGet("{id}/signed-pdf")]
public async Task<IActionResult> DownloadSignedPdf(int id)
{
    var prescription = await _repository.GetByIdAsync(id);
    
    if (prescription == null || !prescription.IsSigned)
        return NotFound();
    
    var pdfContent = await _fileStorageService.GetAsync(prescription.SignedPdfPath);
    
    return File(pdfContent, "application/pdf", $"receita_assinada_{prescription.DocumentNumber}.pdf");
}

[HttpGet("{id}/verify-signature")]
public async Task<IActionResult> VerifySignature(int id)
{
    var prescription = await _repository.GetByIdAsync(id);
    
    if (prescription == null || !prescription.IsSigned)
        return NotFound();
    
    var pdfContent = await _fileStorageService.GetAsync(prescription.SignedPdfPath);
    var isValid = await _signatureService.ValidateSignatureAsync(pdfContent);
    var signatureInfo = await _signatureService.GetSignatureInfoAsync(pdfContent);
    
    return Ok(new
    {
        isValid,
        signatureInfo
    });
}
```

### 3. Geração XML ANVISA v2.1 (2 semanas)

#### 3.1 Modelo XML ANVISA
```csharp
// Modelo para exportação SNGPC
[XmlRoot("prescricao")]
public class AnvisaPrescriptionXml
{
    [XmlElement("identificacao")]
    public IdentificationXml Identification { get; set; }
    
    [XmlElement("profissional")]
    public ProfessionalXml Professional { get; set; }
    
    [XmlElement("paciente")]
    public PatientXml Patient { get; set; }
    
    [XmlArray("medicamentos")]
    [XmlArrayItem("medicamento")]
    public List<MedicationXml> Medications { get; set; }
}

public class IdentificationXml
{
    [XmlElement("numeroNotificacao")]
    public string NotificationNumber { get; set; }
    
    [XmlElement("tipoReceita")]
    public string PrescriptionType { get; set; }
    
    [XmlElement("dataEmissao")]
    public DateTime IssueDate { get; set; }
    
    [XmlElement("unidadeEmissora")]
    public string IssuingUnit { get; set; }
}

public class ProfessionalXml
{
    [XmlElement("nome")]
    public string Name { get; set; }
    
    [XmlElement("cpf")]
    public string CPF { get; set; }
    
    [XmlElement("crm")]
    public string CRM { get; set; }
    
    [XmlElement("uf")]
    public string State { get; set; }
}

public class PatientXml
{
    [XmlElement("nome")]
    public string Name { get; set; }
    
    [XmlElement("cpf")]
    public string CPF { get; set; }
    
    [XmlElement("dataNascimento")]
    public DateTime BirthDate { get; set; }
    
    [XmlElement("endereco")]
    public AddressXml Address { get; set; }
}

public class MedicationXml
{
    [XmlElement("nomeComercial")]
    public string CommercialName { get; set; }
    
    [XmlElement("principioAtivo")]
    public string ActiveIngredient { get; set; }
    
    [XmlElement("concentracao")]
    public string Concentration { get; set; }
    
    [XmlElement("quantidade")]
    public decimal Quantity { get; set; }
    
    [XmlElement("unidade")]
    public string Unit { get; set; }
    
    [XmlElement("posologia")]
    public string Dosage { get; set; }
}
```

#### 3.2 Serviço de Exportação XML
```csharp
public interface IAnvisaXmlService
{
    Task<string> GenerateXmlAsync(int prescriptionId);
    Task<bool> ValidateXmlAsync(string xmlContent);
    Task<byte[]> ExportSngpcBatchAsync(DateTime startDate, DateTime endDate);
}

public class AnvisaXmlService : IAnvisaXmlService
{
    public async Task<string> GenerateXmlAsync(int prescriptionId)
    {
        var prescription = await _repository.GetByIdWithDetailsAsync(prescriptionId);
        
        var xmlModel = new AnvisaPrescriptionXml
        {
            Identification = new IdentificationXml
            {
                NotificationNumber = prescription.SequenceNumber,
                PrescriptionType = MapPrescriptionType(prescription.PrescriptionType),
                IssueDate = prescription.PrescriptionDate,
                IssuingUnit = prescription.Clinic.Name
            },
            Professional = new ProfessionalXml
            {
                Name = prescription.Doctor.FullName,
                CPF = prescription.Doctor.CPF,
                CRM = prescription.Doctor.CRM,
                State = prescription.Doctor.CRMState
            },
            Patient = new PatientXml
            {
                Name = prescription.Patient.FullName,
                CPF = prescription.Patient.CPF,
                BirthDate = prescription.Patient.BirthDate,
                Address = MapAddress(prescription.Patient.Address)
            },
            Medications = prescription.Items.Select(MapMedication).ToList()
        };
        
        // Serializar para XML
        var serializer = new XmlSerializer(typeof(AnvisaPrescriptionXml));
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = Encoding.UTF8
        });
        
        serializer.Serialize(xmlWriter, xmlModel);
        return stringWriter.ToString();
    }
    
    public async Task<bool> ValidateXmlAsync(string xmlContent)
    {
        // Validar contra XSD da ANVISA
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, "path/to/anvisa_schema_v2.1.xsd");
        
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet
        };
        
        var isValid = true;
        settings.ValidationEventHandler += (sender, args) =>
        {
            isValid = false;
            _logger.LogError($"Erro de validação XML: {args.Message}");
        };
        
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader, settings);
        
        while (xmlReader.Read()) { }
        
        return isValid;
    }
    
    public async Task<byte[]> ExportSngpcBatchAsync(DateTime startDate, DateTime endDate)
    {
        // Exportar lote mensal para SNGPC
        var prescriptions = await _repository.GetControlledPrescriptionsAsync(startDate, endDate);
        
        var batch = new SngpcBatchXml
        {
            BatchNumber = GenerateBatchNumber(),
            GeneratedAt = DateTime.Now,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            Prescriptions = new List<AnvisaPrescriptionXml>()
        };
        
        foreach (var prescription in prescriptions)
        {
            var xml = await GenerateXmlAsync(prescription.Id);
            // Adicionar ao lote
        }
        
        // Serializar e compactar (ZIP)
        return await CompressBatchAsync(batch);
    }
}
```

#### 3.3 Endpoints de Exportação
```csharp
[HttpGet("{id}/xml")]
[Produces("application/xml")]
public async Task<IActionResult> ExportXml(int id)
{
    var xml = await _anvisaXmlService.GenerateXmlAsync(id);
    return Content(xml, "application/xml");
}

[HttpGet("sngpc/export")]
public async Task<IActionResult> ExportSngpcBatch([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
{
    var batch = await _anvisaXmlService.ExportSngpcBatchAsync(startDate, endDate);
    var fileName = $"sngpc_batch_{startDate:yyyyMM}_{endDate:yyyyMM}.zip";
    return File(batch, "application/zip", fileName);
}

[HttpPost("xml/validate")]
public async Task<IActionResult> ValidateXml([FromBody] string xmlContent)
{
    var isValid = await _anvisaXmlService.ValidateXmlAsync(xmlContent);
    return Ok(new { valid = isValid });
}
```

### 4. Frontend - Download e Assinatura (1 semana)

#### 4.1 Componente de Download de PDF
```typescript
// prescription-download.component.ts
export class PrescriptionDownloadComponent {
  @Input() prescriptionId: number;
  
  isGenerating = false;
  isAvailable = false;
  
  async downloadPdf() {
    this.isGenerating = true;
    try {
      const response = await this.prescriptionService.downloadPdf(this.prescriptionId);
      const blob = new Blob([response], { type: 'application/pdf' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `receita_${this.prescriptionId}.pdf`;
      link.click();
      window.URL.revokeObjectURL(url);
    } finally {
      this.isGenerating = false;
    }
  }
  
  async previewPdf() {
    const response = await this.prescriptionService.previewPdf(this.prescriptionId);
    const blob = new Blob([response], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    window.open(url, '_blank');
  }
}
```

#### 4.2 Componente de Assinatura Digital
```typescript
// prescription-signature.component.ts
export class PrescriptionSignatureComponent implements OnInit {
  @Input() prescriptionId: number;
  
  certificates: CertificateInfo[] = [];
  selectedCertificate: string;
  isSigning = false;
  
  async ngOnInit() {
    await this.loadCertificates();
  }
  
  async loadCertificates() {
    this.certificates = await this.certificateService.getAvailableCertificates();
  }
  
  async signPrescription() {
    if (!this.selectedCertificate) {
      this.toastr.warning('Selecione um certificado digital');
      return;
    }
    
    this.isSigning = true;
    try {
      await this.prescriptionService.sign(this.prescriptionId, this.selectedCertificate);
      this.toastr.success('Prescrição assinada digitalmente com sucesso!');
      this.onSigned.emit();
    } catch (error) {
      this.toastr.error('Erro ao assinar prescrição: ' + error.message);
    } finally {
      this.isSigning = false;
    }
  }
}
```

```html
<!-- prescription-signature.component.html -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Assinatura Digital</mat-card-title>
  </mat-card-header>
  
  <mat-card-content>
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Selecione o Certificado Digital</mat-label>
      <mat-select [(ngModel)]="selectedCertificate">
        <mat-option *ngFor="let cert of certificates" [value]="cert.thumbprint">
          <div class="certificate-option">
            <strong>{{cert.subjectName}}</strong>
            <small>CPF: {{cert.cpf}}</small>
            <small>Válido até: {{cert.validTo | date:'dd/MM/yyyy'}}</small>
            <mat-chip *ngIf="!cert.isValid" color="warn">Expirado</mat-chip>
          </div>
        </mat-option>
      </mat-select>
    </mat-form-field>
    
    <div class="actions">
      <button mat-raised-button color="primary" 
              (click)="signPrescription()"
              [disabled]="!selectedCertificate || isSigning">
        <mat-icon>verified_user</mat-icon>
        Assinar Digitalmente
      </button>
    </div>
  </mat-card-content>
</mat-card>
```

### 5. Testes com Farmácias (2 semanas)

#### 5.1 Preparação de Testes
1. **Contatar Redes de Farmácias:**
   - Drogaria São Paulo
   - Raia/Drogasil
   - Pague Menos
   - Farmácias independentes locais

2. **Preparar Kit de Teste:**
   - 10 receitas de exemplo (variados tipos)
   - PDFs assinados e não-assinados
   - QR Codes funcionais
   - Documentação de validação

3. **Protocolo de Teste:**
   - Farmacêutico valida prescrição via QR Code
   - Sistema da farmácia lê dados da prescrição
   - Verificação de assinatura digital
   - Registro de controlados no SNGPC

#### 5.2 Checklist de Validação
- [ ] QR Code escaneável em diferentes apps
- [ ] Informações completas no QR Code
- [ ] PDF legível e profissional
- [ ] Assinatura digital válida
- [ ] Dados compatíveis com sistema da farmácia
- [ ] Receitas controladas aceitas
- [ ] Conformidade ANVISA 344/98

#### 5.3 Feedback e Ajustes
- Coletar feedback de farmacêuticos
- Identificar problemas de leitura/validação
- Ajustar formato de QR Code se necessário
- Melhorar layout de PDF conforme sugestões
- Documentar melhores práticas

## ✅ Critérios de Sucesso

### Técnicos
- [ ] PDFs profissionais para todos os tipos de receita
- [ ] Assinatura digital ICP-Brasil funcional
- [ ] Validação de assinaturas operacional
- [ ] XML ANVISA v2.1 validado contra XSD
- [ ] Performance: geração de PDF < 3 segundos
- [ ] Armazenamento seguro de PDFs assinados

### Funcionais
- [ ] Médicos conseguem gerar e assinar receitas facilmente
- [ ] PDFs são legíveis e profissionais
- [ ] QR Codes funcionam em farmácias
- [ ] Certificados A1/A3 suportados
- [ ] Exportação SNGPC automática

### Conformidade Legal
- [ ] ✅ CFM 1.643/2002 compliance total
- [ ] ✅ ANVISA 344/98 compliance para controlados
- [ ] ✅ RDC 20/2011 compliance para antimicrobianos
- [ ] ✅ ICP-Brasil certificação válida
- [ ] ✅ SNGPC reporting funcional

### Aceitação de Mercado
- [ ] Testes com pelo menos 3 redes de farmácias
- [ ] Taxa de aceitação: >90%
- [ ] Feedback positivo de farmacêuticos
- [ ] Zero problemas críticos reportados

## 📦 Entregáveis

1. **Código Backend**
   - `PrescriptionPdfService` com generators
   - `DigitalSignatureService`
   - `CertificateManagementService`
   - `AnvisaXmlService`
   - Migrations (se necessário)

2. **Código Frontend**
   - `PrescriptionDownloadComponent`
   - `PrescriptionSignatureComponent`
   - Telas de preview e download
   - Interface de seleção de certificados

3. **Templates PDF**
   - Template receita simples
   - Template receita controlada
   - Template receita antimicrobiana
   - Assets (logos, marcas d'água)

4. **Documentação**
   - Guia de assinatura digital
   - Manual para farmácias
   - FAQ técnico
   - Relatório de testes com farmácias

5. **Schemas e Validação**
   - XSD ANVISA v2.1
   - Scripts de validação
   - Exemplos de XML válidos

## 🔗 Dependências

### Pré-requisitos (✅ Completos)
- ✅ Sistema de prescrições digitais 80% completo
- ✅ Backend e frontend funcionais
- ✅ SNGPC dashboard implementado

### Dependências Externas
- Certificado digital ICP-Brasil (médico deve possuir)
- Acesso a farmácias para testes
- XSD schemas ANVISA v2.1

### Bibliotecas Necessárias
- QuestPDF ou iTextSharp (geração PDF)
- System.Security.Cryptography (assinatura digital)
- System.Xml (validação XSD)

### Tarefas Dependentes
- **Telemedicina** - Prescrições digitais usadas em consultas online
- **Prontuário SOAP** - Integração com plano terapêutico

## 🧪 Testes

### Testes Unitários
```csharp
[Fact]
public async Task GeneratePdf_SimplePrescrip tion_ShouldCreateValidPdf()
{
    // Arrange
    var prescription = CreateSimplePrescription();
    
    // Act
    var pdf = await _pdfService.GeneratePdfAsync(prescription.Id, new PrescriptionPdfOptions());
    
    // Assert
    Assert.NotNull(pdf);
    Assert.True(pdf.Length > 0);
    Assert.True(IsValidPdf(pdf));
}

[Fact]
public async Task SignPdf_WithValidCertificate_ShouldSignSuccessfully()
{
    // Arrange
    var pdf = await GenerateSamplePdf();
    var certificate = GetTestCertificate();
    
    // Act
    var signedPdf = await _signatureService.SignPdfAsync(pdf, certificate);
    
    // Assert
    Assert.NotNull(signedPdf);
    var isValid = await _signatureService.ValidateSignatureAsync(signedPdf);
    Assert.True(isValid);
}

[Fact]
public async Task GenerateXml_ControlledPrescription_ShouldValidateAgainstXsd()
{
    // Arrange
    var prescription = CreateControlledPrescription();
    
    // Act
    var xml = await _anvisaXmlService.GenerateXmlAsync(prescription.Id);
    
    // Assert
    var isValid = await _anvisaXmlService.ValidateXmlAsync(xml);
    Assert.True(isValid);
}
```

### Testes de Integração
- Fluxo completo: criar prescrição → gerar PDF → assinar → validar
- Exportação SNGPC com múltiplas prescrições
- Download de PDF assinado

### Testes E2E
- Médico cria receita controlada → assina digitalmente → farmácia valida QR Code
- Exportação mensal SNGPC
- Verificação de certificado expirado

### Testes Manuais
- Impressão de PDFs em diferentes impressoras
- QR Code em diferentes leitores/apps
- Validação em farmácias reais

## 📊 Métricas de Acompanhamento

### Durante Desenvolvimento
- Cobertura de testes: >70%
- Tempo de geração de PDF: <3s
- Taxa de validação XML: 100%
- Bugs encontrados vs resolvidos

### Pós-Deploy
- Taxa de uso de prescrições digitais: meta >60%
- Taxa de aceitação em farmácias: meta >90%
- Tempo médio de assinatura: meta <30s
- Taxa de erro na geração: meta <1%
- Satisfação de médicos: meta >8/10
- Satisfação de farmacêuticos: meta >7/10

## 🚨 Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Farmácias não aceitam formato | Média | Alto | Testes antecipados, seguir padrões ANVISA |
| Problemas com certificados A3 | Média | Médio | Suporte amplo, documentação clara |
| Performance ruim na geração | Baixa | Médio | Otimização, cache de templates |
| XML não validado pela ANVISA | Baixa | Alto | Validação rigorosa contra XSD oficial |
| QR Code não legível | Baixa | Alto | Testes em múltiplos dispositivos |

## 📚 Referências

### Documentação Interna
- [DIGITAL_PRESCRIPTIONS.md](../../docs/DIGITAL_PRESCRIPTIONS.md)
- [IMPLEMENTACAO_PENDENTE_CFM_PRESCRICOES.md](../../docs/IMPLEMENTACAO_PENDENTE_CFM_PRESCRICOES.md)

### Regulamentações
- [Resolução CFM nº 1.643/2002](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1643) - Prescrições Médicas
- [Portaria ANVISA 344/98](https://bvsms.saude.gov.br/bvs/saudelegis/svs/1998/prt0344_12_05_1998_rep.html) - Medicamentos Controlados
- [RDC ANVISA 20/2011](http://antigo.anvisa.gov.br/documents/10181/2718376/RDC_20_2011_COMP.pdf) - Antimicrobianos
- [ICP-Brasil](https://www.gov.br/iti/pt-br/assuntos/icp-brasil) - Certificação Digital
- [SNGPC](https://www.gov.br/anvisa/pt-br/assuntos/fiscalizacao-e-monitoramento/sngpc) - Sistema Nacional

### Bibliotecas e Ferramentas
- [QuestPDF](https://www.questpdf.com/) - Geração de PDF moderna
- [iTextSharp](https://github.com/itext/itextsharp) - Assinatura digital
- [XSD.exe](https://docs.microsoft.com/dotnet/standard/serialization/xml-schema-definition-tool-xsd-exe) - Validação XML

### Código Existente
- `src/MedicSoft.Api/Controllers/DigitalPrescriptionsController.cs`
- `frontend/src/app/prescriptions/` - Componentes existentes
- `docs/schemas/anvisa_v2.1.xsd` - Schema ANVISA

---

> **Próximo Passo:** Após concluir esta tarefa, seguir para **04-sngpc-integracao.md**  
> **Última Atualização:** 23 de Janeiro de 2026
