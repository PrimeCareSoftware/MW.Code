# 🧾 Prompt: Emissão NF-e / NFS-e

## 📊 Status
- **Prioridade**: 🔥🔥🔥 CRÍTICA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 3 meses | 2 devs
- **Prazo**: Q2/2025

## 🎯 Contexto

Implementar sistema completo de emissão de Notas Fiscais Eletrônicas (NF-e/NFS-e) conforme legislação brasileira. Este é um recurso **OBRIGATÓRIO POR LEI** para empresas que prestam serviços ou vendem produtos no Brasil.

## ⚖️ Legislação Aplicável

- **Receita Federal**: Obrigatoriedade de emissão de NF-e
- **SEFAZ Municipal/Estadual**: Padrões de NFS-e variam por município
- **NFSe Nacional**: Novo padrão unificado em implantação

## 📋 Tipos de Nota Fiscal

### 1. NFS-e - Nota Fiscal de Serviços Eletrônica
**Uso**: Serviços médicos, consultas, exames  
**Emissor**: SEFAZ Municipal  
**Prioridade**: CRÍTICA

### 2. NF-e - Nota Fiscal Eletrônica
**Uso**: Venda de produtos (medicamentos, insumos)  
**Emissor**: SEFAZ Estadual  
**Prioridade**: MÉDIA (apenas se clínica vende produtos)

### 3. NFC-e - Nota Fiscal ao Consumidor Eletrônica
**Uso**: Venda direta ao consumidor final  
**Emissor**: SEFAZ Estadual  
**Prioridade**: BAIXA (alternativa à NF-e)

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// Entidades
public class Invoice : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public InvoiceType Type { get; set; }  // NFSe, NFe, NFCe
    public string Number { get; set; }
    public string Series { get; set; }
    public DateTime IssueDate { get; set; }
    
    // Prestador (Clínica)
    public string ProviderCnpj { get; set; }
    public string ProviderName { get; set; }
    public string ProviderMunicipalRegistration { get; set; }
    
    // Tomador (Paciente/Cliente)
    public string ClientCpfCnpj { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }
    
    // Serviço
    public string ServiceDescription { get; set; }
    public string ServiceCode { get; set; }  // CNAE
    public decimal ServiceAmount { get; set; }
    
    // Impostos
    public decimal IssRate { get; set; }  // Alíquota ISS
    public decimal IssAmount { get; set; }
    public decimal IrAmount { get; set; }  // IR Retido
    public decimal PisAmount { get; set; }
    public decimal CofinsAmount { get; set; }
    public decimal CsllAmount { get; set; }
    public decimal InssAmount { get; set; }
    
    // Status
    public InvoiceStatus Status { get; set; }
    public string AuthorizationCode { get; set; }
    public string AccessKey { get; set; }  // Chave de acesso
    public string XmlContent { get; set; }
    public string PdfUrl { get; set; }
    
    // Cancelamento
    public DateTime? CancellationDate { get; set; }
    public string CancellationReason { get; set; }
    
    // Substituição
    public Guid? ReplacedInvoiceId { get; set; }
    public Guid? ReplacementInvoiceId { get; set; }
}

public enum InvoiceType
{
    NFSe,    // Serviços
    NFe,     // Produtos
    NFCe,    // Consumidor
    RPS      // Recibo Provisório de Serviços
}

public enum InvoiceStatus
{
    Draft,              // Rascunho
    Pending,            // Aguardando processamento
    PendingAuthorization, // Enviada, aguardando SEFAZ
    Authorized,         // Autorizada pela SEFAZ
    Cancelled,          // Cancelada
    Denied,             // Denegada pela SEFAZ
    Error               // Erro no processamento
}

public class InvoiceConfiguration : Entity
{
    public string TenantId { get; set; }
    public string Cnpj { get; set; }
    public string MunicipalRegistration { get; set; }
    public string StateRegistration { get; set; }
    
    // Certificado Digital
    public byte[] DigitalCertificate { get; set; }  // A1
    public string CertificatePassword { get; set; }
    
    // Configurações Municipais
    public string CityCode { get; set; }
    public string ServiceCode { get; set; }
    public decimal IssRate { get; set; }
    
    // Numeração
    public int CurrentInvoiceNumber { get; set; }
    public string InvoiceSeries { get; set; }
    
    // Gateway
    public InvoiceGateway Gateway { get; set; }
    public string GatewayApiKey { get; set; }
}

public enum InvoiceGateway
{
    FocusNFe,
    ENotas,
    NFeCidades,
    Direct  // Integração direta com SEFAZ
}
```

### Camada de Aplicação (Application Layer)

```csharp
// Service Interface
public interface IInvoiceService
{
    Task<Invoice> CreateInvoice(CreateInvoiceCommand command);
    Task<Invoice> IssueInvoice(Guid invoiceId);
    Task<Invoice> CancelInvoice(Guid invoiceId, string reason);
    Task<Invoice> ReplaceInvoice(Guid invoiceId, string reason);
    Task<byte[]> GetInvoicePdf(Guid invoiceId);
    Task<string> GetInvoiceXml(Guid invoiceId);
    Task<List<Invoice>> GetInvoicesByPeriod(DateTime startDate, DateTime endDate);
    Task SendInvoiceByEmail(Guid invoiceId, string email);
}

// DTOs
public record CreateInvoiceCommand(
    Guid? AppointmentId,
    Guid? PaymentId,
    string ClientCpfCnpj,
    string ClientName,
    string ClientEmail,
    string ServiceDescription,
    decimal ServiceAmount,
    bool AutoIssue = false  // Emitir automaticamente
);

public record IssueInvoiceCommand(
    Guid InvoiceId
);

public record CancelInvoiceCommand(
    Guid InvoiceId,
    string Reason
);

// Response
public record InvoiceResponse(
    Guid Id,
    string Number,
    DateTime IssueDate,
    string ClientName,
    decimal Amount,
    InvoiceStatus Status,
    string PdfUrl,
    string AccessKey
);
```

### Camada de Infraestrutura (Infrastructure Layer)

```csharp
// Gateway Integration (Focus NFe Example)
public class FocusNFeGateway : IInvoiceGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public async Task<InvoiceResult> IssueInvoice(Invoice invoice)
    {
        var nfseRequest = new
        {
            data_emissao = invoice.IssueDate.ToString("yyyy-MM-dd"),
            prestador = new
            {
                cnpj = invoice.ProviderCnpj,
                inscricao_municipal = invoice.ProviderMunicipalRegistration
            },
            tomador = new
            {
                cpf_cnpj = invoice.ClientCpfCnpj,
                razao_social = invoice.ClientName,
                email = invoice.ClientEmail
            },
            servico = new
            {
                discriminacao = invoice.ServiceDescription,
                codigo_servico = invoice.ServiceCode,
                valor_servicos = invoice.ServiceAmount,
                iss_retido = false,
                aliquota_iss = invoice.IssRate
            },
            impostos = new
            {
                iss = invoice.IssAmount,
                ir = invoice.IrAmount,
                pis = invoice.PisAmount,
                cofins = invoice.CofinsAmount,
                csll = invoice.CsllAmount,
                inss = invoice.InssAmount
            }
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            $"/v2/nfse?ref={invoice.Id}", 
            nfseRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvoiceException($"Erro ao emitir NFS-e: {error}");
        }
        
        var result = await response.Content.ReadFromJsonAsync<FocusNFeResponse>();
        
        return new InvoiceResult
        {
            Success = true,
            Number = result.Numero,
            AccessKey = result.CodigoVerificacao,
            AuthorizationCode = result.CodigoAutorizacao,
            XmlContent = result.CaminhoXmlNotaFiscal,
            PdfUrl = result.CaminhoDanfe
        };
    }
    
    public async Task<bool> CancelInvoice(string accessKey, string reason)
    {
        var cancelRequest = new
        {
            justificativa = reason
        };
        
        var response = await _httpClient.DeleteAsync(
            $"/v2/nfse/{accessKey}", 
            cancelRequest);
        
        return response.IsSuccessStatusCode;
    }
    
    public async Task<byte[]> DownloadPdf(string pdfUrl)
    {
        var response = await _httpClient.GetAsync(pdfUrl);
        return await response.Content.ReadAsByteArrayAsync();
    }
}

// Invoice Repository
public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Invoice> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Configuration)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
    
    public async Task<int> GetNextInvoiceNumber(string tenantId)
    {
        var config = await _context.InvoiceConfigurations
            .FirstOrDefaultAsync(c => c.TenantId == tenantId);
        
        if (config == null)
            throw new InvalidOperationException("Configuração de NF não encontrada");
        
        config.CurrentInvoiceNumber++;
        await _context.SaveChangesAsync();
        
        return config.CurrentInvoiceNumber;
    }
}
```

### Camada de API (API Layer)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    
    [HttpPost]
    public async Task<IActionResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand command)
    {
        var invoice = await _invoiceService.CreateInvoice(command);
        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }
    
    [HttpPost("{id}/issue")]
    public async Task<IActionResult> IssueInvoice(Guid id)
    {
        try
        {
            var invoice = await _invoiceService.IssueInvoice(id);
            return Ok(invoice);
        }
        catch (InvoiceException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelInvoice(
        Guid id, 
        [FromBody] CancelInvoiceCommand command)
    {
        var invoice = await _invoiceService.CancelInvoice(id, command.Reason);
        return Ok(invoice);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();
        
        return Ok(invoice);
    }
    
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var pdf = await _invoiceService.GetInvoicePdf(id);
        return File(pdf, "application/pdf", $"nfse-{id}.pdf");
    }
    
    [HttpGet("{id}/xml")]
    public async Task<IActionResult> DownloadXml(Guid id)
    {
        var xml = await _invoiceService.GetInvoiceXml(id);
        return File(Encoding.UTF8.GetBytes(xml), "application/xml", $"nfse-{id}.xml");
    }
    
    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendByEmail(Guid id, [FromBody] SendEmailCommand command)
    {
        await _invoiceService.SendInvoiceByEmail(id, command.Email);
        return Ok();
    }
}
```

## 🎨 Frontend (Angular)

### Componentes Necessários

```typescript
// 1. Invoice Configuration Component
@Component({
  selector: 'app-invoice-configuration',
  template: `
    <h2>Configuração de Notas Fiscais</h2>
    <form [formGroup]="configForm" (ngSubmit)="saveConfiguration()">
      <mat-form-field>
        <input matInput placeholder="CNPJ" formControlName="cnpj" mask="00.000.000/0000-00">
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Inscrição Municipal" formControlName="municipalRegistration">
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Código da Cidade (IBGE)" formControlName="cityCode">
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Alíquota ISS (%)" formControlName="issRate" type="number">
      </mat-form-field>
      
      <mat-form-field>
        <mat-select placeholder="Gateway" formControlName="gateway">
          <mat-option value="FocusNFe">Focus NFe</mat-option>
          <mat-option value="ENotas">eNotas</mat-option>
          <mat-option value="NFeCidades">NFeáceis</mat-option>
        </mat-select>
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="API Key do Gateway" formControlName="gatewayApiKey" type="password">
      </mat-form-field>
      
      <div class="certificate-upload">
        <label>Certificado Digital A1 (.pfx)</label>
        <input type="file" (change)="onCertificateUpload($event)" accept=".pfx">
        <input matInput placeholder="Senha do Certificado" formControlName="certificatePassword" type="password">
      </div>
      
      <button mat-raised-button color="primary" type="submit">Salvar Configuração</button>
    </form>
  `
})
export class InvoiceConfigurationComponent { }

// 2. Invoice List Component
@Component({
  selector: 'app-invoice-list',
  template: `
    <h2>Notas Fiscais Emitidas</h2>
    
    <mat-form-field>
      <mat-date-range-input [rangePicker]="picker">
        <input matStartDate placeholder="Data Inicial" [(ngModel)]="startDate">
        <input matEndDate placeholder="Data Final" [(ngModel)]="endDate">
      </mat-date-range-input>
      <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
      <mat-date-range-picker #picker></mat-date-range-picker>
    </mat-form-field>
    
    <button mat-raised-button (click)="filterInvoices()">Filtrar</button>
    
    <table mat-table [dataSource]="invoices">
      <ng-container matColumnDef="number">
        <th mat-header-cell *matHeaderCellDef>Número</th>
        <td mat-cell *matCellDef="let invoice">{{ invoice.number }}</td>
      </ng-container>
      
      <ng-container matColumnDef="issueDate">
        <th mat-header-cell *matHeaderCellDef>Data</th>
        <td mat-cell *matCellDef="let invoice">{{ invoice.issueDate | date:'dd/MM/yyyy' }}</td>
      </ng-container>
      
      <ng-container matColumnDef="client">
        <th mat-header-cell *matHeaderCellDef>Cliente</th>
        <td mat-cell *matCellDef="let invoice">{{ invoice.clientName }}</td>
      </ng-container>
      
      <ng-container matColumnDef="amount">
        <th mat-header-cell *matHeaderCellDef>Valor</th>
        <td mat-cell *matCellDef="let invoice">{{ invoice.serviceAmount | currency:'BRL' }}</td>
      </ng-container>
      
      <ng-container matColumnDef="status">
        <th mat-header-cell *matHeaderCellDef>Status</th>
        <td mat-cell *matCellDef="let invoice">
          <mat-chip [color]="getStatusColor(invoice.status)">
            {{ getStatusText(invoice.status) }}
          </mat-chip>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef>Ações</th>
        <td mat-cell *matCellDef="let invoice">
          <button mat-icon-button [matMenuTriggerFor]="menu">
            <mat-icon>more_vert</mat-icon>
          </button>
          <mat-menu #menu="matMenu">
            <button mat-menu-item (click)="downloadPdf(invoice.id)">
              <mat-icon>picture_as_pdf</mat-icon>
              Download PDF
            </button>
            <button mat-menu-item (click)="downloadXml(invoice.id)">
              <mat-icon>code</mat-icon>
              Download XML
            </button>
            <button mat-menu-item (click)="sendByEmail(invoice.id)">
              <mat-icon>email</mat-icon>
              Enviar por Email
            </button>
            <button mat-menu-item (click)="cancelInvoice(invoice.id)" 
                    [disabled]="invoice.status !== 'Authorized'">
              <mat-icon>cancel</mat-icon>
              Cancelar
            </button>
          </mat-menu>
        </td>
      </ng-container>
      
      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
    
    <mat-paginator [length]="totalInvoices" [pageSize]="pageSize"></mat-paginator>
  `
})
export class InvoiceListComponent { }

// 3. Invoice Form Component
@Component({
  selector: 'app-invoice-form',
  template: `
    <h2>Emitir Nota Fiscal</h2>
    <form [formGroup]="invoiceForm" (ngSubmit)="issueInvoice()">
      <mat-form-field>
        <input matInput placeholder="CPF/CNPJ do Cliente" formControlName="clientCpfCnpj" 
               [mask]="cpfCnpjMask">
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Nome do Cliente" formControlName="clientName">
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Email do Cliente" formControlName="clientEmail" type="email">
      </mat-form-field>
      
      <mat-form-field>
        <textarea matInput placeholder="Descrição do Serviço" formControlName="serviceDescription" 
                  rows="4"></textarea>
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Valor do Serviço" formControlName="serviceAmount" 
               type="number" step="0.01">
      </mat-form-field>
      
      <div class="tax-summary">
        <h3>Resumo de Impostos</h3>
        <p>ISS ({{ issRate }}%): {{ calculateIss() | currency:'BRL' }}</p>
        <p>PIS (0.65%): {{ calculatePis() | currency:'BRL' }}</p>
        <p>COFINS (3.00%): {{ calculateCofins() | currency:'BRL' }}</p>
        <p>CSLL (1.00%): {{ calculateCsll() | currency:'BRL' }}</p>
        <p><strong>Valor Líquido: {{ calculateNet() | currency:'BRL' }}</strong></p>
      </div>
      
      <mat-checkbox formControlName="autoIssue">
        Emitir automaticamente após criar
      </mat-checkbox>
      
      <div class="form-actions">
        <button mat-raised-button type="button" (click)="saveDraft()">Salvar Rascunho</button>
        <button mat-raised-button color="primary" type="submit">Emitir NFS-e</button>
      </div>
    </form>
  `
})
export class InvoiceFormComponent { }
```

## 📋 Checklist de Implementação

### Backend

- [ ] Criar entidades de domínio (Invoice, InvoiceConfiguration)
- [ ] Implementar repositórios
- [ ] Criar serviços de aplicação
- [ ] Integrar com gateway (Focus NFe recomendado)
- [ ] Implementar cálculo de impostos
- [ ] Criar controllers REST
- [ ] Adicionar migrations
- [ ] Implementar emissão automática após pagamento
- [ ] Sistema de armazenamento de XML/PDF (5 anos)
- [ ] Implementar envio por email
- [ ] Implementar testes unitários
- [ ] Implementar testes de integração

### Frontend

- [ ] Criar componente de configuração
- [ ] Implementar listagem de notas
- [ ] Criar formulário de emissão
- [ ] Implementar filtros e busca
- [ ] Adicionar download de PDF/XML
- [ ] Criar visualizador de nota
- [ ] Implementar cancelamento
- [ ] Adicionar dashboard fiscal
- [ ] Relatório de livro de serviços

### Integrações

- [ ] Escolher gateway (Focus NFe, eNotas, ou direto)
- [ ] Configurar certificado digital A1/A3
- [ ] Integrar com cada município (se direto)
- [ ] Testar em ambiente de homologação
- [ ] Validar em produção

### Compliance e Documentação

- [ ] Manual de configuração
- [ ] Guia de emissão
- [ ] Processo de cancelamento
- [ ] Política de retenção (5 anos XML/PDF)
- [ ] Auditoria de emissões

## 🧪 Testes

### Testes Unitários
```csharp
public class InvoiceServiceTests
{
    [Fact]
    public async Task ShouldCreateInvoice()
    {
        // Test invoice creation
    }
    
    [Fact]
    public async Task ShouldCalculateTaxesCorrectly()
    {
        // Test ISS, PIS, COFINS calculation
    }
    
    [Fact]
    public async Task ShouldCancelInvoice()
    {
        // Test cancellation
    }
}
```

### Testes de Integração
- [ ] Testar emissão em homologação
- [ ] Testar cancelamento
- [ ] Testar download de PDF
- [ ] Testar envio por email
- [ ] Validar XML contra schema

## 📚 Referências

- [PENDING_TASKS.md - Seção NF-e/NFS-e](../../PENDING_TASKS.md#41-emissão-de-notas-fiscais-eletrônicas-nf-e--nfs-e)
- [Focus NFe Documentation](https://focusnfe.com.br/doc/)
- [eNotas Documentation](https://enotas.com.br/desenvolvedores/)
- [Portal da Nota Fiscal de Serviço Eletrônica](http://www.nfse.gov.br/)
- [Receita Federal - NF-e](http://www.nfe.fazenda.gov.br/)

## 💰 Investimento

- **Desenvolvimento**: 3 meses, 2 devs
- **Custo**: R$ 135k
- **Gateway**: R$ 50-200/mês (Focus NFe, eNotas)
- **Certificado Digital A1**: R$ 150-300/ano
- **ROI Esperado**: Compliance legal obrigatório

## ✅ Critérios de Aceitação

1. ✅ Sistema permite configuração de dados fiscais
2. ✅ Certificado digital pode ser uploaded
3. ✅ Notas podem ser criadas manualmente
4. ✅ Notas são emitidas automaticamente após pagamento
5. ✅ Impostos são calculados corretamente
6. ✅ PDF e XML são armazenados por 5 anos
7. ✅ Notas podem ser canceladas
8. ✅ Paciente recebe nota por email automaticamente
9. ✅ Relatório fiscal (livro de serviços) disponível
10. ✅ Integração com SEFAZ funciona corretamente

---

**Última Atualização**: Janeiro 2026
**Status**: Pronto para desenvolvimento
**Próximo Passo**: Escolher gateway e iniciar implementação backend
