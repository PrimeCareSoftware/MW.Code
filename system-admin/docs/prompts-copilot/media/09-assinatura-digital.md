# 🔐 Prompt: Assinatura Digital ICP-Brasil

## 📊 Status
- **Prioridade**: 🔥 MÉDIA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 2-3 meses | 2 devs
- **Prazo**: Q3/2026

## 🎯 Contexto

Implementar suporte completo a certificados digitais ICP-Brasil (A1/A3) para assinatura de documentos médicos, garantindo validade jurídica conforme exigido pelo CFM e legislação brasileira. Esta funcionalidade é essencial para conformidade com CFM 1.638/2002, CFM 1.643/2002 e Medida Provisória 2.200-2/2001.

## 📋 Justificativa

### Regulamentação
- **CFM 1.638/2002**: Exige assinatura digital em prontuários eletrônicos
- **CFM 1.643/2002**: Obrigatório para receitas médicas digitais
- **MP 2.200-2/2001**: Define ICP-Brasil como padrão nacional
- **Lei 14.063/2020**: Assinaturas eletrônicas em serviços públicos e privados

### Benefícios
- ✅ Validade jurídica plena dos documentos
- ✅ Eliminação de papel (sustentabilidade)
- ✅ Rastreabilidade e não-repúdio
- ✅ Conformidade regulatória total
- ✅ Diferencial competitivo premium
- ✅ Redução de custos operacionais

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// src/Domain/Entities/DigitalSignature.cs
public class DigitalSignature : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public Guid DocumentId { get; set; }
    public DocumentType DocumentType { get; set; }
    public Guid SignerId { get; set; }  // DoctorId ou UserId
    public string SignerName { get; set; }
    public string SignerCpf { get; set; }
    public string SignerCrm { get; set; }  // Se médico
    
    // Certificado Digital
    public CertificateType CertificateType { get; set; }  // A1, A3
    public string CertificateSubject { get; set; }
    public string CertificateIssuer { get; set; }
    public string CertificateSerialNumber { get; set; }
    public DateTime CertificateValidFrom { get; set; }
    public DateTime CertificateValidTo { get; set; }
    public string CertificateThumbprint { get; set; }
    
    // Assinatura
    public string SignatureValue { get; set; }  // Base64
    public string SignatureAlgorithm { get; set; }  // SHA256withRSA
    public DateTime SignedAt { get; set; }
    public string TimestampUrl { get; set; }
    public string TimestampValue { get; set; }  // RFC 3161
    
    // Hash do Documento
    public string DocumentHash { get; set; }  // SHA-256
    public string HashAlgorithm { get; set; }
    
    // Metadados
    public string IpAddress { get; set; }
    public string Location { get; set; }
    public string Reason { get; set; }
    public bool IsValid { get; set; }
    public DateTime? ValidationDate { get; set; }
    public string ValidationResult { get; set; }
}

public enum DocumentType
{
    MedicalRecord,       // Prontuário
    Prescription,        // Receita
    MedicalCertificate,  // Atestado
    MedicalReport,       // Laudo
    Consent,             // Consentimento
    Invoice,             // Nota Fiscal
    Contract             // Contrato
}

public enum CertificateType
{
    A1,  // Software (arquivo PFX)
    A3   // Hardware (token/smartcard)
}

// Value Objects
public class CertificateInfo : ValueObject
{
    public string Subject { get; private set; }
    public string Issuer { get; private set; }
    public string SerialNumber { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public string Thumbprint { get; private set; }
    public CertificateType Type { get; private set; }
    
    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return now >= ValidFrom && now <= ValidTo;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Subject;
        yield return Issuer;
        yield return SerialNumber;
        yield return Thumbprint;
    }
}
```

### Camada de Aplicação (Application Layer)

```csharp
// src/Application/Services/IDigitalSignatureService.cs
public interface IDigitalSignatureService
{
    // Certificados
    Task<List<CertificateInfo>> GetAvailableCertificatesAsync();
    Task<CertificateInfo> GetCertificateByThumbprintAsync(string thumbprint);
    Task<bool> ValidateCertificateAsync(string thumbprint);
    
    // Assinatura
    Task<DigitalSignature> SignDocumentAsync(
        Guid documentId,
        DocumentType documentType,
        string certificateThumbprint,
        string pin = null);  // PIN do token A3
    
    Task<DigitalSignature> SignPdfAsync(
        byte[] pdfContent,
        string certificateThumbprint,
        SignatureOptions options);
    
    Task<DigitalSignature> SignXmlAsync(
        string xmlContent,
        string certificateThumbprint);
    
    // Validação
    Task<bool> ValidateSignatureAsync(Guid signatureId);
    Task<SignatureValidationResult> ValidateDocumentSignatureAsync(
        byte[] documentContent,
        string signatureValue);
    
    // Timestamp
    Task<string> AddTimestampAsync(byte[] documentHash);
    
    // Múltiplas Assinaturas
    Task<List<DigitalSignature>> GetDocumentSignaturesAsync(
        Guid documentId,
        DocumentType documentType);
}

// DTOs
public class SignatureOptions
{
    public string Location { get; set; }
    public string Reason { get; set; }
    public string ContactInfo { get; set; }
    public bool Visible { get; set; }
    public SignaturePosition Position { get; set; }
    public bool AddTimestamp { get; set; }
}

public class SignaturePosition
{
    public int Page { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

public class SignatureValidationResult
{
    public bool IsValid { get; set; }
    public bool CertificateIsValid { get; set; }
    public bool SignatureIntegrityValid { get; set; }
    public bool TimestampValid { get; set; }
    public List<string> ValidationErrors { get; set; }
    public CertificateInfo CertificateInfo { get; set; }
    public DateTime? SignedAt { get; set; }
}

// src/Application/Services/DigitalSignatureService.cs
public class DigitalSignatureService : IDigitalSignatureService
{
    private readonly IDigitalSignatureRepository _repository;
    private readonly ICertificateProvider _certificateProvider;
    private readonly ITimestampService _timestampService;
    private readonly ILogger<DigitalSignatureService> _logger;
    
    public async Task<DigitalSignature> SignPdfAsync(
        byte[] pdfContent,
        string certificateThumbprint,
        SignatureOptions options)
    {
        var certificate = await _certificateProvider
            .GetCertificateAsync(certificateThumbprint);
        
        if (!await _certificateProvider.ValidateCertificateAsync(certificate))
            throw new InvalidOperationException("Certificado inválido ou expirado");
        
        // Calcular hash do documento
        var documentHash = ComputeSha256Hash(pdfContent);
        
        // Assinar com RSA
        using var rsa = certificate.GetRSAPrivateKey();
        var signatureBytes = rsa.SignData(
            pdfContent,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        
        var signatureValue = Convert.ToBase64String(signatureBytes);
        
        // Adicionar timestamp
        string timestampValue = null;
        if (options.AddTimestamp)
        {
            timestampValue = await _timestampService
                .AddTimestampAsync(signatureBytes);
        }
        
        var signature = new DigitalSignature
        {
            DocumentHash = documentHash,
            HashAlgorithm = "SHA-256",
            SignatureValue = signatureValue,
            SignatureAlgorithm = "SHA256withRSA",
            SignedAt = DateTime.UtcNow,
            CertificateThumbprint = certificate.Thumbprint,
            CertificateSubject = certificate.Subject,
            CertificateIssuer = certificate.Issuer,
            CertificateSerialNumber = certificate.SerialNumber,
            TimestampValue = timestampValue,
            IsValid = true,
            Location = options.Location,
            Reason = options.Reason
        };
        
        await _repository.AddAsync(signature);
        
        return signature;
    }
    
    private string ComputeSha256Hash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }
}
```

### Camada de Infraestrutura (Infrastructure Layer)

```csharp
// src/Infrastructure/DigitalSignature/ICertificateProvider.cs
public interface ICertificateProvider
{
    Task<List<X509Certificate2>> GetAvailableCertificatesAsync();
    Task<X509Certificate2> GetCertificateAsync(string thumbprint);
    Task<bool> ValidateCertificateAsync(X509Certificate2 certificate);
    Task<bool> IsCertificateRevokedAsync(X509Certificate2 certificate);
}

// src/Infrastructure/DigitalSignature/WindowsCertificateProvider.cs
public class WindowsCertificateProvider : ICertificateProvider
{
    public async Task<List<X509Certificate2>> GetAvailableCertificatesAsync()
    {
        var certificates = new List<X509Certificate2>();
        
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        
        foreach (var cert in store.Certificates)
        {
            // Filtrar certificados ICP-Brasil com chave privada
            if (cert.HasPrivateKey && IsIcpBrasilCertificate(cert))
            {
                certificates.Add(cert);
            }
        }
        
        return await Task.FromResult(certificates);
    }
    
    private bool IsIcpBrasilCertificate(X509Certificate2 cert)
    {
        // Verificar se o emissor é uma AC ICP-Brasil
        var icpBrasilIssuers = new[]
        {
            "AC Serasa RFB",
            "AC Certisign RFB",
            "AC SOLUTI",
            "AC Safeweb RFB",
            "AC VALID RFB"
        };
        
        return icpBrasilIssuers.Any(issuer => 
            cert.Issuer.Contains(issuer, StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task<bool> ValidateCertificateAsync(X509Certificate2 certificate)
    {
        var chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
        
        var isValid = chain.Build(certificate);
        
        // Verificar expiração
        var now = DateTime.Now;
        isValid &= certificate.NotBefore <= now && certificate.NotAfter >= now;
        
        return await Task.FromResult(isValid);
    }
}

// src/Infrastructure/DigitalSignature/ITimestampService.cs
public interface ITimestampService
{
    Task<string> AddTimestampAsync(byte[] data);
    Task<bool> ValidateTimestampAsync(string timestamp);
}

// src/Infrastructure/DigitalSignature/Rfc3161TimestampService.cs
public class Rfc3161TimestampService : ITimestampService
{
    private readonly string _timestampUrl;
    private readonly HttpClient _httpClient;
    
    public Rfc3161TimestampService(string timestampUrl = "http://timestamp.iti.gov.br")
    {
        _timestampUrl = timestampUrl;
        _httpClient = new HttpClient();
    }
    
    public async Task<string> AddTimestampAsync(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        
        // Criar requisição RFC 3161
        var request = CreateTimestampRequest(hash);
        
        var content = new ByteArrayContent(request);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/timestamp-query");
        
        var response = await _httpClient.PostAsync(_timestampUrl, content);
        var timestampResponse = await response.Content.ReadAsByteArrayAsync();
        
        return Convert.ToBase64String(timestampResponse);
    }
    
    private byte[] CreateTimestampRequest(byte[] hash)
    {
        // Implementar RFC 3161 Timestamp Request
        // Usar biblioteca como Bouncy Castle para simplificar
        throw new NotImplementedException("Use Bouncy Castle for RFC 3161");
    }
}

// src/Infrastructure/DigitalSignature/PdfSignatureService.cs
public class PdfSignatureService
{
    public byte[] SignPdfDocument(
        byte[] pdfContent,
        X509Certificate2 certificate,
        SignatureOptions options)
    {
        // Usar iTextSharp ou similar para assinar PDF
        using var reader = new PdfReader(pdfContent);
        using var output = new MemoryStream();
        using var stamper = PdfStamper.CreateSignature(reader, output, '\0');
        
        var appearance = stamper.SignatureAppearance;
        appearance.Reason = options.Reason;
        appearance.Location = options.Location;
        appearance.SetVisibleSignature(
            new Rectangle(options.Position.X, options.Position.Y, 
                         options.Position.Width, options.Position.Height),
            options.Position.Page,
            "Signature");
        
        var pk = new PrivateKeySignature(certificate, "SHA-256");
        MakeSignature.SignDetached(
            appearance,
            pk,
            new[] { certificate },
            null,
            null,
            null,
            0,
            CryptoStandard.CMS);
        
        return output.ToArray();
    }
}
```

### Camada de API (API Layer)

```csharp
// src/API/Controllers/DigitalSignaturesController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DigitalSignaturesController : ControllerBase
{
    private readonly IDigitalSignatureService _signatureService;
    
    [HttpGet("certificates")]
    public async Task<ActionResult<List<CertificateInfo>>> GetCertificates()
    {
        var certificates = await _signatureService.GetAvailableCertificatesAsync();
        return Ok(certificates);
    }
    
    [HttpPost("sign-document")]
    public async Task<ActionResult<DigitalSignature>> SignDocument(
        [FromBody] SignDocumentRequest request)
    {
        var signature = await _signatureService.SignDocumentAsync(
            request.DocumentId,
            request.DocumentType,
            request.CertificateThumbprint,
            request.Pin);
        
        return Ok(signature);
    }
    
    [HttpPost("sign-pdf")]
    public async Task<ActionResult<byte[]>> SignPdf(
        [FromForm] IFormFile file,
        [FromForm] string certificateThumbprint,
        [FromForm] SignatureOptions options)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var pdfContent = ms.ToArray();
        
        var signature = await _signatureService.SignPdfAsync(
            pdfContent,
            certificateThumbprint,
            options);
        
        // Retornar PDF assinado
        return File(pdfContent, "application/pdf", "signed.pdf");
    }
    
    [HttpPost("validate/{signatureId}")]
    public async Task<ActionResult<bool>> ValidateSignature(Guid signatureId)
    {
        var isValid = await _signatureService.ValidateSignatureAsync(signatureId);
        return Ok(isValid);
    }
    
    [HttpGet("document/{documentId}/signatures")]
    public async Task<ActionResult<List<DigitalSignature>>> GetDocumentSignatures(
        Guid documentId,
        [FromQuery] DocumentType documentType)
    {
        var signatures = await _signatureService.GetDocumentSignaturesAsync(
            documentId,
            documentType);
        
        return Ok(signatures);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes

```typescript
// src/app/features/digital-signature/certificate-selector/certificate-selector.component.ts
@Component({
  selector: 'app-certificate-selector',
  template: `
    <mat-form-field appearance="outline" class="w-full">
      <mat-label>Certificado Digital</mat-label>
      <mat-select [(ngModel)]="selectedCertificate" 
                  (selectionChange)="onCertificateChange()">
        <mat-option *ngFor="let cert of certificates" [value]="cert">
          <div class="certificate-option">
            <div class="cert-name">{{ cert.subject }}</div>
            <div class="cert-details">
              <span class="cert-type" [class.cert-a3]="cert.type === 'A3'">
                {{ cert.type }}
              </span>
              <span class="cert-validity">
                Válido até: {{ cert.validTo | date:'dd/MM/yyyy' }}
              </span>
            </div>
          </div>
        </mat-option>
      </mat-select>
      <mat-icon matPrefix>verified_user</mat-icon>
    </mat-form-field>
    
    <div *ngIf="selectedCertificate" class="certificate-info mt-3">
      <mat-card>
        <mat-card-content>
          <h4>Informações do Certificado</h4>
          <dl>
            <dt>Titular:</dt>
            <dd>{{ selectedCertificate.subject }}</dd>
            
            <dt>Emissor:</dt>
            <dd>{{ selectedCertificate.issuer }}</dd>
            
            <dt>Tipo:</dt>
            <dd>{{ selectedCertificate.type }}</dd>
            
            <dt>Validade:</dt>
            <dd>{{ selectedCertificate.validFrom | date:'dd/MM/yyyy' }} até 
                {{ selectedCertificate.validTo | date:'dd/MM/yyyy' }}</dd>
            
            <dt>Status:</dt>
            <dd>
              <mat-chip [color]="selectedCertificate.isValid ? 'primary' : 'warn'">
                {{ selectedCertificate.isValid ? 'Válido' : 'Expirado' }}
              </mat-chip>
            </dd>
          </dl>
        </mat-card-content>
      </mat-card>
    </div>
  `
})
export class CertificateSelectorComponent implements OnInit {
  @Output() certificateSelected = new EventEmitter<CertificateInfo>();
  
  certificates: CertificateInfo[] = [];
  selectedCertificate: CertificateInfo | null = null;
  
  constructor(private signatureService: DigitalSignatureService) {}
  
  async ngOnInit() {
    this.certificates = await this.signatureService.getAvailableCertificates();
  }
  
  onCertificateChange() {
    this.certificateSelected.emit(this.selectedCertificate!);
  }
}

// src/app/features/digital-signature/sign-document-dialog/sign-document-dialog.component.ts
@Component({
  selector: 'app-sign-document-dialog',
  template: `
    <h2 mat-dialog-title>Assinar Documento Digitalmente</h2>
    
    <mat-dialog-content>
      <form [formGroup]="signForm">
        <app-certificate-selector 
          (certificateSelected)="onCertificateSelected($event)">
        </app-certificate-selector>
        
        <mat-form-field appearance="outline" class="w-full mt-3">
          <mat-label>Motivo da Assinatura</mat-label>
          <input matInput formControlName="reason" 
                 placeholder="Ex: Aprovação de prontuário">
        </mat-form-field>
        
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Localização</mat-label>
          <input matInput formControlName="location" 
                 placeholder="Ex: São Paulo, SP">
        </mat-form-field>
        
        <mat-checkbox formControlName="addTimestamp">
          Adicionar carimbo de tempo
        </mat-checkbox>
        
        <mat-form-field *ngIf="requiresPin" appearance="outline" class="w-full mt-3">
          <mat-label>PIN do Token</mat-label>
          <input matInput type="password" formControlName="pin" 
                 placeholder="Digite o PIN do token A3">
        </mat-form-field>
      </form>
    </mat-dialog-content>
    
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancelar</button>
      <button mat-raised-button color="primary" 
              [disabled]="!signForm.valid || loading"
              (click)="onSign()">
        <mat-icon *ngIf="!loading">draw</mat-icon>
        <mat-spinner *ngIf="loading" diameter="20"></mat-spinner>
        Assinar
      </button>
    </mat-dialog-actions>
  `
})
export class SignDocumentDialogComponent {
  @Inject(MAT_DIALOG_DATA) public data: { documentId: string; documentType: string };
  
  signForm: FormGroup;
  requiresPin = false;
  loading = false;
  
  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<SignDocumentDialogComponent>,
    private signatureService: DigitalSignatureService,
    private snackBar: MatSnackBar
  ) {
    this.signForm = this.fb.group({
      reason: ['', Validators.required],
      location: [''],
      addTimestamp: [true],
      pin: ['']
    });
  }
  
  onCertificateSelected(certificate: CertificateInfo) {
    this.requiresPin = certificate.type === 'A3';
    
    if (this.requiresPin) {
      this.signForm.get('pin')?.setValidators([Validators.required]);
    } else {
      this.signForm.get('pin')?.clearValidators();
    }
    this.signForm.get('pin')?.updateValueAndValidity();
  }
  
  async onSign() {
    if (!this.signForm.valid) return;
    
    this.loading = true;
    try {
      const result = await this.signatureService.signDocument({
        documentId: this.data.documentId,
        documentType: this.data.documentType,
        certificateThumbprint: this.selectedCertificate.thumbprint,
        reason: this.signForm.value.reason,
        location: this.signForm.value.location,
        addTimestamp: this.signForm.value.addTimestamp,
        pin: this.signForm.value.pin
      });
      
      this.snackBar.open('Documento assinado com sucesso!', 'OK', { duration: 3000 });
      this.dialogRef.close(result);
    } catch (error) {
      this.snackBar.open('Erro ao assinar documento: ' + error.message, 'OK', { duration: 5000 });
    } finally {
      this.loading = false;
    }
  }
  
  onCancel() {
    this.dialogRef.close();
  }
}

// src/app/features/digital-signature/signature-validation/signature-validation.component.ts
@Component({
  selector: 'app-signature-validation',
  template: `
    <mat-card *ngIf="validationResult">
      <mat-card-header>
        <mat-card-title>
          <mat-icon [color]="validationResult.isValid ? 'primary' : 'warn'">
            {{ validationResult.isValid ? 'verified' : 'error' }}
          </mat-icon>
          Validação de Assinatura Digital
        </mat-card-title>
      </mat-card-header>
      
      <mat-card-content>
        <div class="validation-status" 
             [class.valid]="validationResult.isValid"
             [class.invalid]="!validationResult.isValid">
          <h3>{{ validationResult.isValid ? 'Assinatura Válida' : 'Assinatura Inválida' }}</h3>
        </div>
        
        <mat-list>
          <mat-list-item>
            <mat-icon mat-list-icon [color]="validationResult.certificateIsValid ? 'primary' : 'warn'">
              {{ validationResult.certificateIsValid ? 'check_circle' : 'cancel' }}
            </mat-icon>
            <div mat-line>Certificado</div>
            <div mat-line>{{ validationResult.certificateIsValid ? 'Válido' : 'Inválido ou Expirado' }}</div>
          </mat-list-item>
          
          <mat-list-item>
            <mat-icon mat-list-icon [color]="validationResult.signatureIntegrityValid ? 'primary' : 'warn'">
              {{ validationResult.signatureIntegrityValid ? 'check_circle' : 'cancel' }}
            </mat-icon>
            <div mat-line>Integridade</div>
            <div mat-line>{{ validationResult.signatureIntegrityValid ? 'Documento Íntegro' : 'Documento Modificado' }}</div>
          </mat-list-item>
          
          <mat-list-item *ngIf="validationResult.timestampValid !== null">
            <mat-icon mat-list-icon [color]="validationResult.timestampValid ? 'primary' : 'warn'">
              {{ validationResult.timestampValid ? 'check_circle' : 'cancel' }}
            </mat-icon>
            <div mat-line>Carimbo de Tempo</div>
            <div mat-line>{{ validationResult.timestampValid ? 'Válido' : 'Inválido' }}</div>
          </mat-list-item>
        </mat-list>
        
        <div class="certificate-details mt-4" *ngIf="validationResult.certificateInfo">
          <h4>Informações do Certificado</h4>
          <dl>
            <dt>Titular:</dt>
            <dd>{{ validationResult.certificateInfo.subject }}</dd>
            
            <dt>Emissor:</dt>
            <dd>{{ validationResult.certificateInfo.issuer }}</dd>
            
            <dt>Data da Assinatura:</dt>
            <dd>{{ validationResult.signedAt | date:'dd/MM/yyyy HH:mm:ss' }}</dd>
          </dl>
        </div>
        
        <mat-expansion-panel *ngIf="validationResult.validationErrors?.length">
          <mat-expansion-panel-header>
            <mat-panel-title>
              Erros de Validação ({{ validationResult.validationErrors.length }})
            </mat-panel-title>
          </mat-expansion-panel-header>
          
          <ul>
            <li *ngFor="let error of validationResult.validationErrors">
              {{ error }}
            </li>
          </ul>
        </mat-expansion-panel>
      </mat-card-content>
    </mat-card>
  `
})
export class SignatureValidationComponent implements OnInit {
  @Input() signatureId!: string;
  
  validationResult: SignatureValidationResult | null = null;
  
  constructor(private signatureService: DigitalSignatureService) {}
  
  async ngOnInit() {
    this.validationResult = await this.signatureService.validateSignature(this.signatureId);
  }
}
```

### Service

```typescript
// src/app/core/services/digital-signature.service.ts
@Injectable({ providedIn: 'root' })
export class DigitalSignatureService {
  private apiUrl = '/api/digital-signatures';
  
  constructor(private http: HttpClient) {}
  
  getAvailableCertificates(): Promise<CertificateInfo[]> {
    return firstValueFrom(
      this.http.get<CertificateInfo[]>(`${this.apiUrl}/certificates`)
    );
  }
  
  signDocument(request: SignDocumentRequest): Promise<DigitalSignature> {
    return firstValueFrom(
      this.http.post<DigitalSignature>(`${this.apiUrl}/sign-document`, request)
    );
  }
  
  validateSignature(signatureId: string): Promise<SignatureValidationResult> {
    return firstValueFrom(
      this.http.post<SignatureValidationResult>(
        `${this.apiUrl}/validate/${signatureId}`,
        {}
      )
    );
  }
  
  getDocumentSignatures(documentId: string, documentType: string): Promise<DigitalSignature[]> {
    return firstValueFrom(
      this.http.get<DigitalSignature[]>(
        `${this.apiUrl}/document/${documentId}/signatures`,
        { params: { documentType } }
      )
    );
  }
}
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar entidades de domínio (DigitalSignature, CertificateInfo)
- [ ] Implementar repositórios e persistência
- [ ] Criar serviços de aplicação (DigitalSignatureService)
- [ ] Implementar provider de certificados (Windows Store)
- [ ] Implementar timestamp service (RFC 3161)
- [ ] Criar service de assinatura PDF (iTextSharp)
- [ ] Criar service de assinatura XML
- [ ] Implementar validação de certificados (cadeia, revogação)
- [ ] Criar controllers REST API
- [ ] Adicionar migrations
- [ ] Configurar injeção de dependências
- [ ] Implementar logging de auditoria

### Frontend
- [ ] Criar CertificateSelectorComponent
- [ ] Criar SignDocumentDialogComponent
- [ ] Criar SignatureValidationComponent
- [ ] Criar SignatureListComponent
- [ ] Implementar DigitalSignatureService
- [ ] Adicionar rotas
- [ ] Integrar com prontuários
- [ ] Integrar com receitas digitais
- [ ] Integrar com atestados
- [ ] Adicionar botão "Assinar" nos documentos
- [ ] Criar visualizador de assinaturas

### Integrações
- [ ] Integração com HSM (A3)
- [ ] Integração com ITI (timestamp)
- [ ] Validação de LCR (Lista de Certificados Revogados)
- [ ] Validação OCSP (Online Certificate Status Protocol)

### Testes
- [ ] Testes unitários (entidades)
- [ ] Testes de serviços
- [ ] Testes de validação de certificados
- [ ] Testes de assinatura PDF
- [ ] Testes de assinatura XML
- [ ] Testes de timestamp
- [ ] Testes de integração

### Documentação
- [ ] Guia de instalação de certificados
- [ ] Guia de uso para médicos
- [ ] Documentação técnica da API
- [ ] Troubleshooting guide

## 🧪 Testes

```csharp
public class DigitalSignatureServiceTests
{
    [Fact]
    public async Task SignDocument_WithValidCertificate_ShouldSucceed()
    {
        // Arrange
        var service = CreateService();
        var documentId = Guid.NewGuid();
        var thumbprint = "ABC123...";
        
        // Act
        var signature = await service.SignDocumentAsync(
            documentId,
            DocumentType.MedicalRecord,
            thumbprint);
        
        // Assert
        Assert.NotNull(signature);
        Assert.True(signature.IsValid);
        Assert.NotEmpty(signature.SignatureValue);
    }
    
    [Fact]
    public async Task ValidateSignature_WithValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateService();
        var signatureId = Guid.NewGuid();
        
        // Act
        var isValid = await service.ValidateSignatureAsync(signatureId);
        
        // Assert
        Assert.True(isValid);
    }
}
```

## 📚 Referências

### Regulamentação
- [Resolução CFM 1.638/2002](http://www.portalmedico.org.br/resolucoes/cfm/2002/1638_2002.htm) - Prontuário Eletrônico
- [Resolução CFM 1.643/2002](http://www.portalmedico.org.br/resolucoes/cfm/2002/1643_2002.htm) - Receita Médica Digital
- [MP 2.200-2/2001](http://www.planalto.gov.br/ccivil_03/mpv/antigas_2001/2200-2.htm) - ICP-Brasil
- [Lei 14.063/2020](http://www.planalto.gov.br/ccivil_03/_ato2019-2022/2020/lei/L14063.htm) - Assinaturas Eletrônicas

### Certificadoras ICP-Brasil
- [Serasa Experian](https://certificadodigital.serasaexperian.com.br/)
- [Certisign](https://www.certisign.com.br/)
- [Safeweb](https://www.safeweb.com.br/)
- [Soluti (Docusign)](https://www.soluti.com.br/)
- [Valid Certificadora](https://www.validcertificadora.com.br/)

### Bibliotecas
- [iTextSharp](https://github.com/itext/itextsharp) - Assinatura PDF
- [Bouncy Castle](https://www.bouncycastle.org/) - Criptografia
- [System.Security.Cryptography](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography)

### Padrões
- [RFC 3161](https://www.ietf.org/rfc/rfc3161.txt) - Time-Stamp Protocol (TSP)
- [RFC 5280](https://www.ietf.org/rfc/rfc5280.txt) - X.509 Certificates
- [PKCS #7](https://www.rfc-editor.org/rfc/rfc2315) - Cryptographic Message Syntax

## 💰 Investimento

### Desenvolvimento
- **Esforço**: 2-3 meses | 2 devs
- **Custo**: R$ 90-135k

### Infraestrutura
- Servidor de timestamp: Gratuito (ITI)
- Certificados A1: R$ 150-300/ano por médico
- Tokens A3: R$ 180-350/unidade
- HSM (opcional): R$ 15-50k (uma vez)

### ROI Esperado
- Eliminação de papel: R$ 2-5k/mês
- Redução de tempo: 30-40%
- Premium pricing: +R$ 50-100/mês por clínica
- Compliance obrigatório: Evita multas e processos

## 🎯 Critérios de Aceitação

- [ ] Médico consegue selecionar certificado instalado no computador
- [ ] Sistema detecta certificados A1 e A3
- [ ] Assinatura de prontuário funciona
- [ ] Assinatura de receita funciona
- [ ] PDF assinado tem assinatura visível e válida
- [ ] Validação de assinatura funciona
- [ ] Timestamp é adicionado corretamente
- [ ] Sistema valida certificado (expiração, revogação)
- [ ] Múltiplas assinaturas no mesmo documento
- [ ] Auditoria completa de assinaturas
- [ ] Interface intuitiva para médicos
- [ ] Suporte a token A3 com PIN
- [ ] Documentação completa para usuários
