# 📚 CFM 1.638/2002 - Versionamento e Auditoria de Prontuário

**Prioridade:** 🔥🔥🔥 P0 - CRÍTICA  
**Obrigatoriedade:** Legal (Conselho Federal de Medicina)  
**Status Atual:** ✅ 100% completo (Janeiro 2026)  
**Esforço:** 1.5 meses | 1 desenvolvedor  
**Custo Estimado:** R$ 22.500  
**Prazo:** Q1 2026 (Fevereiro-Março) - **CONCLUÍDO**

## 📋 Contexto

A Resolução CFM 1.638/2002 estabelece requisitos para prontuários eletrônicos, incluindo:

1. **Versionamento completo** - Nunca deletar versões anteriores
2. **Imutabilidade** - Após fechamento, não permitir edições (apenas reabrir com justificativa)
3. **Auditoria de acessos** - Registrar todos os acessos ao prontuário
4. **Assinatura digital** - Preparação da infraestrutura (implementação em outra tarefa)

### Situação Atual
- ✅ Prontuário médico básico funcional
- ✅ CRUD de prontuários existente
- ✅ **Versionamento completo** - Event sourcing implementado (Janeiro 2026)
- ✅ **Imutabilidade funcional** - Prontuários fechados não podem ser editados
- ✅ **Auditoria detalhada** - Logs completos de acesso com IP, User-Agent, timestamp

### Por que é Crítico
- **Legal:** Obrigatório por CFM 1.638/2002
- **Auditoria:** Necessário para processos jurídicos e éticos
- **Confiabilidade:** Garante integridade dos dados médicos
- **Multas:** CFM pode aplicar sanções por não conformidade

## 🎯 Objetivos da Tarefa

Implementar um sistema completo de versionamento, imutabilidade e auditoria para prontuários médicos, em conformidade com CFM 1.638/2002.

## 📝 Tarefas Detalhadas

### 1. Versionamento com Event Sourcing (2 semanas)

#### 1.1 Modelagem de Dados
```csharp
// Criar entidade MedicalRecordVersion
public class MedicalRecordVersion
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public int Version { get; set; }
    public string ChangeType { get; set; } // Created, Updated, Closed, Reopened
    public DateTime ChangedAt { get; set; }
    public int ChangedByUserId { get; set; }
    public string ChangeReason { get; set; } // Obrigatório para reaberturas
    public string SnapshotJson { get; set; } // JSON completo do estado
    public string ChangesSummary { get; set; } // Resumo das mudanças
    
    // Relacionamentos
    public MedicalRecord MedicalRecord { get; set; }
    public User ChangedBy { get; set; }
}

// Adicionar campos ao MedicalRecord existente
public class MedicalRecord
{
    // ... campos existentes ...
    
    public int CurrentVersion { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? ClosedByUserId { get; set; }
    public DateTime? ReopenedAt { get; set; }
    public int? ReopenedByUserId { get; set; }
    public string ReopenReason { get; set; }
    
    // Navegação
    public ICollection<MedicalRecordVersion> Versions { get; set; }
}
```

#### 1.2 Migration
```csharp
// Criar tabela MedicalRecordVersions
// Adicionar campos novos em MedicalRecords
// Criar índices: (MedicalRecordId, Version), (ChangedAt)

// IMPORTANTE: Migração de dados existentes
// - Criar versão 1 para todos os prontuários existentes
// - Snapshot atual como estado inicial
```

#### 1.3 Repositório e Serviço
```csharp
public interface IMedicalRecordVersionService
{
    Task<MedicalRecordVersion> CreateVersionAsync(int medicalRecordId, string changeType, int userId, string reason = null);
    Task<List<MedicalRecordVersion>> GetVersionHistoryAsync(int medicalRecordId);
    Task<MedicalRecordVersion> GetVersionAsync(int medicalRecordId, int version);
    Task<MedicalRecord> RestoreVersionAsync(int medicalRecordId, int version, int userId, string reason);
    Task<string> GenerateChangesSummaryAsync(MedicalRecord oldState, MedicalRecord newState);
}

// Implementar lógica de:
// - Snapshot automático ao salvar
// - Comparação de estados (diff)
// - Restauração de versão específica
```

### 2. Imutabilidade após Fechamento (1 semana)

#### 2.1 Backend - Validação de Imutabilidade
```csharp
public class MedicalRecordService
{
    public async Task<Result> UpdateMedicalRecordAsync(UpdateMedicalRecordDto dto, int userId)
    {
        var record = await _repository.GetByIdAsync(dto.Id);
        
        // Validar se está fechado
        if (record.IsClosed)
        {
            return Result.Failure("Prontuário fechado não pode ser editado. Use 'Reabrir' para fazer alterações.");
        }
        
        // Criar versão antes de atualizar
        await _versionService.CreateVersionAsync(record.Id, "Updated", userId);
        
        // Atualizar e incrementar versão
        record.CurrentVersion++;
        await _repository.UpdateAsync(record);
        
        return Result.Success();
    }
    
    public async Task<Result> CloseMedicalRecordAsync(int recordId, int userId)
    {
        var record = await _repository.GetByIdAsync(recordId);
        
        // Validar CFM 1.821 completude
        var isComplete = await _cfm1821ValidationService.IsMedicalRecordReadyForClosure(recordId);
        if (!isComplete)
        {
            return Result.Failure("Prontuário incompleto. Complete todos os campos CFM 1.821 antes de fechar.");
        }
        
        // Fechar e criar versão
        record.IsClosed = true;
        record.ClosedAt = DateTime.UtcNow;
        record.ClosedByUserId = userId;
        
        await _versionService.CreateVersionAsync(recordId, "Closed", userId);
        await _repository.UpdateAsync(record);
        
        return Result.Success();
    }
    
    public async Task<Result> ReopenMedicalRecordAsync(int recordId, int userId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure("Justificativa obrigatória para reabertura de prontuário.");
        }
        
        var record = await _repository.GetByIdAsync(recordId);
        
        if (!record.IsClosed)
        {
            return Result.Failure("Prontuário já está aberto.");
        }
        
        // Reabrir com justificativa
        record.IsClosed = false;
        record.ReopenedAt = DateTime.UtcNow;
        record.ReopenedByUserId = userId;
        record.ReopenReason = reason;
        record.CurrentVersion++;
        
        await _versionService.CreateVersionAsync(recordId, "Reopened", userId, reason);
        await _repository.UpdateAsync(record);
        
        return Result.Success();
    }
}
```

#### 2.2 Endpoints API
```csharp
[HttpPost("{id}/close")]
public async Task<IActionResult> CloseMedicalRecord(int id)
{
    var userId = GetCurrentUserId();
    var result = await _service.CloseMedicalRecordAsync(id, userId);
    return result.IsSuccess ? Ok() : BadRequest(result.Error);
}

[HttpPost("{id}/reopen")]
public async Task<IActionResult> ReopenMedicalRecord(int id, [FromBody] ReopenRequestDto dto)
{
    var userId = GetCurrentUserId();
    var result = await _service.ReopenMedicalRecordAsync(id, userId, dto.Reason);
    return result.IsSuccess ? Ok() : BadRequest(result.Error);
}

[HttpGet("{id}/versions")]
public async Task<IActionResult> GetVersionHistory(int id)
{
    var versions = await _versionService.GetVersionHistoryAsync(id);
    return Ok(versions);
}

[HttpGet("{id}/versions/{version}")]
public async Task<IActionResult> GetVersion(int id, int version)
{
    var versionData = await _versionService.GetVersionAsync(id, version);
    return Ok(versionData);
}
```

### 3. Auditoria de Acessos (2 semanas)

#### 3.1 Modelagem de Auditoria
```csharp
public class MedicalRecordAccessLog
{
    public long Id { get; set; }
    public int MedicalRecordId { get; set; }
    public int UserId { get; set; }
    public string AccessType { get; set; } // View, Edit, Close, Reopen, Print, Export
    public DateTime AccessedAt { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Details { get; set; } // Informações adicionais
    
    // Navegação
    public MedicalRecord MedicalRecord { get; set; }
    public User User { get; set; }
}
```

#### 3.2 Middleware de Auditoria
```csharp
public class MedicalRecordAuditMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value;
        
        // Verificar se é acesso a prontuário
        if (path.Contains("/api/medical-records/"))
        {
            var recordId = ExtractRecordId(path);
            var userId = GetUserIdFromToken(context);
            var accessType = DetermineAccessType(context.Request.Method, path);
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            
            // Registrar acesso de forma assíncrona (não bloquear request)
            _ = Task.Run(() => LogAccessAsync(recordId, userId, accessType, ipAddress, userAgent));
        }
        
        await next(context);
    }
}

// Registrar no Startup.cs
app.UseMiddleware<MedicalRecordAuditMiddleware>();
```

#### 3.3 Serviço de Auditoria
```csharp
public interface IMedicalRecordAuditService
{
    Task LogAccessAsync(int recordId, int userId, string accessType, string ipAddress, string userAgent, string details = null);
    Task<List<MedicalRecordAccessLog>> GetAccessLogsAsync(int recordId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MedicalRecordAccessLog>> GetUserAccessLogsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MedicalRecordAccessLog>> GetSuspiciousActivityAsync(DateTime? startDate = null);
}

// Implementar detecção de atividade suspeita:
// - Múltiplos acessos em curto período
// - Acessos fora do horário normal
// - Acesso a prontuários sem vínculo de atendimento
// - IPs suspeitos
```

### 4. Frontend - Interfaces de Versionamento e Auditoria (2 semanas)

#### 4.1 Botão de Conclusão de Prontuário
```typescript
// medical-record-form.component.ts
async closeMedicalRecord() {
  // Validar CFM 1.821
  const isComplete = await this.validateCfm1821Completeness();
  
  if (!isComplete) {
    this.showError('Prontuário incompleto. Complete todos os campos obrigatórios.');
    return;
  }
  
  // Confirmar ação
  const confirmed = await this.confirmDialog.show({
    title: 'Concluir Prontuário',
    message: 'Após a conclusão, o prontuário ficará imutável. Deseja continuar?',
    confirmText: 'Sim, Concluir',
    cancelText: 'Cancelar'
  });
  
  if (confirmed) {
    await this.medicalRecordService.close(this.recordId);
    this.showSuccess('Prontuário concluído com sucesso!');
    this.record.isClosed = true;
  }
}
```

#### 4.2 Modal de Reabertura
```typescript
async reopenMedicalRecord() {
  const reason = await this.promptDialog.show({
    title: 'Reabrir Prontuário',
    message: 'Informe a justificativa para reabertura:',
    inputType: 'textarea',
    required: true,
    minLength: 20
  });
  
  if (reason) {
    await this.medicalRecordService.reopen(this.recordId, reason);
    this.showSuccess('Prontuário reaberto. Todas as alterações serão registradas.');
    this.record.isClosed = false;
  }
}
```

#### 4.3 Visualizador de Histórico de Versões
```html
<!-- medical-record-version-history.component.html -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Histórico de Versões</mat-card-title>
  </mat-card-header>
  
  <mat-card-content>
    <mat-list>
      <mat-list-item *ngFor="let version of versions">
        <mat-icon mat-list-icon>{{getVersionIcon(version.changeType)}}</mat-icon>
        <div mat-line>
          <strong>Versão {{version.version}}</strong> - {{version.changeType}}
        </div>
        <div mat-line class="text-muted">
          {{version.changedAt | date:'short'}} por {{version.changedBy.name}}
        </div>
        <div mat-line *ngIf="version.changesSummary">
          {{version.changesSummary}}
        </div>
        <button mat-icon-button (click)="viewVersion(version)">
          <mat-icon>visibility</mat-icon>
        </button>
        <button mat-icon-button (click)="compareVersions(version)" 
                *ngIf="version.version > 1">
          <mat-icon>compare</mat-icon>
        </button>
      </mat-list-item>
    </mat-list>
  </mat-card-content>
</mat-card>
```

#### 4.4 Visualizador de Log de Acessos
```html
<!-- medical-record-access-log.component.html -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Log de Acessos</mat-card-title>
  </mat-card-header>
  
  <mat-card-content>
    <table mat-table [dataSource]="accessLogs">
      <ng-container matColumnDef="accessedAt">
        <th mat-header-cell *matHeaderCellDef>Data/Hora</th>
        <td mat-cell *matCellDef="let log">{{log.accessedAt | date:'short'}}</td>
      </ng-container>
      
      <ng-container matColumnDef="user">
        <th mat-header-cell *matHeaderCellDef>Usuário</th>
        <td mat-cell *matCellDef="let log">{{log.user.name}}</td>
      </ng-container>
      
      <ng-container matColumnDef="accessType">
        <th mat-header-cell *matHeaderCellDef>Ação</th>
        <td mat-cell *matCellDef="let log">
          <mat-chip [class]="getAccessTypeClass(log.accessType)">
            {{log.accessType}}
          </mat-chip>
        </td>
      </ng-container>
      
      <ng-container matColumnDef="ipAddress">
        <th mat-header-cell *matHeaderCellDef>IP</th>
        <td mat-cell *matCellDef="let log">{{log.ipAddress}}</td>
      </ng-container>
      
      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
    
    <mat-paginator [pageSize]="20" [pageSizeOptions]="[10, 20, 50, 100]">
    </mat-paginator>
  </mat-card-content>
</mat-card>
```

### 5. Preparação para Assinatura Digital (1 semana)

#### 5.1 Hash SHA-256 de Versões
```csharp
public class MedicalRecordVersion
{
    // ... campos existentes ...
    
    public string ContentHash { get; set; } // SHA-256 do conteúdo
    public string PreviousVersionHash { get; set; } // Blockchain-like
}

public async Task<string> GenerateContentHashAsync(MedicalRecord record)
{
    // Serializar conteúdo de forma determinística
    var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });
    
    // Calcular SHA-256
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(json);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}
```

#### 5.2 Preparar Estrutura para Assinatura
```csharp
public class MedicalRecordSignature
{
    public int Id { get; set; }
    public int MedicalRecordVersionId { get; set; }
    public int SignedByUserId { get; set; }
    public DateTime SignedAt { get; set; }
    public string SignatureType { get; set; } // ICP-Brasil, Simple, etc.
    public string SignatureValue { get; set; } // Assinatura digital
    public string CertificateData { get; set; } // Dados do certificado
    
    // Navegação
    public MedicalRecordVersion Version { get; set; }
    public User SignedBy { get; set; }
}

// Nota: Implementação completa de assinatura digital será feita em tarefa separada
```

### 6. Testes (1 semana)

#### 6.1 Testes Unitários
```csharp
[Fact]
public async Task ClosedMedicalRecord_CannotBeEdited()
{
    // Arrange
    var record = await CreateAndCloseMedicalRecord();
    
    // Act
    var result = await _service.UpdateMedicalRecordAsync(new UpdateDto { Id = record.Id }, 1);
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains("fechado", result.Error.ToLower());
}

[Fact]
public async Task ReopenMedicalRecord_RequiresReason()
{
    // Arrange
    var record = await CreateAndCloseMedicalRecord();
    
    // Act
    var result = await _service.ReopenMedicalRecordAsync(record.Id, 1, null);
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains("justificativa", result.Error.ToLower());
}

[Fact]
public async Task AllChanges_CreateNewVersion()
{
    // Arrange
    var record = await CreateMedicalRecord();
    
    // Act
    await _service.UpdateMedicalRecordAsync(new UpdateDto { Id = record.Id }, 1);
    await _service.UpdateMedicalRecordAsync(new UpdateDto { Id = record.Id }, 1);
    
    // Assert
    var versions = await _versionService.GetVersionHistoryAsync(record.Id);
    Assert.Equal(3, versions.Count); // Criação + 2 updates
}
```

#### 6.2 Testes de Integração
- Fluxo completo: criar → editar → fechar → tentar editar (falhar) → reabrir → editar
- Verificar criação de versões em cada etapa
- Validar logs de auditoria

### 7. Deploy e Documentação (1 semana)

#### 7.1 Migração de Dados
- Criar versão inicial para todos os prontuários existentes
- Validar integridade dos dados
- Rollback plan preparado

#### 7.2 Documentação
- Atualizar guia do usuário
- Documentar fluxo de versionamento
- FAQ sobre reabertura de prontuários

## ✅ Critérios de Sucesso

### Técnicos
- [x] 100% dos prontuários versionados
- [x] Imutabilidade funcional após fechamento
- [x] Logs de auditoria funcionando
- [x] Performance: <10% overhead por versionamento
- [x] Retenção: logs mantidos por 20+ anos

### Funcionais
- [x] Médicos conseguem fechar/reabrir prontuários
- [x] Histórico de versões acessível e claro
- [x] Logs de acesso disponíveis para administradores

### Conformidade Legal (CFM 1.638/2002)
- [x] ✅ Versionamento completo implementado
- [x] ✅ Imutabilidade após fechamento
- [x] ✅ Auditoria de acessos funcional
- [x] ✅ Preparação para assinatura digital

### Melhorias Futuras (Fora do Escopo CFM 1.638)
- [ ] Alertas de atividade suspeita funcionando

## 📦 Entregáveis

1. **Código Backend**
   - `MedicalRecordVersion` entity
   - `MedicalRecordAccessLog` entity
   - `MedicalRecordVersionService`
   - `MedicalRecordAuditService`
   - Migrations
   - Middleware de auditoria

2. **Código Frontend**
   - `MedicalRecordVersionHistoryComponent`
   - `MedicalRecordAccessLogComponent`
   - Modals de fechamento/reabertura
   - Indicadores visuais

3. **Documentação**
   - Guia de versionamento para usuários
   - Documentação técnica de auditoria
   - Política de retenção de dados

4. **Testes**
   - 20+ testes unitários
   - 5+ testes de integração
   - Testes E2E do fluxo completo

## 🔗 Dependências

### Pré-requisitos
- ✅ CFM 1.821 completo (tarefa #01)
- ✅ Prontuário médico básico funcional

### Dependências Externas
- Entity Framework Core 6+
- SQL Server (suporta JSON)

### Tarefas Dependentes
- **Assinatura Digital ICP-Brasil** (usará hashes e estrutura criada aqui)
- **LGPD Auditoria** (complementará auditoria implementada aqui)

## 📚 Referências

- [Resolução CFM nº 1.638/2002](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1638)
- Event Sourcing Pattern: Martin Fowler
- LGPD Art. 37-40 (Segurança e Boas Práticas)

---

> **STATUS:** ✅ **IMPLEMENTAÇÃO COMPLETA - Janeiro 2026**  
> **Documentação Técnica:** [CFM-1638-VERSIONING-README.md](../../docs/CFM-1638-VERSIONING-README.md)  
> **Documentação de Conclusão:** [CFM-1638-IMPLEMENTATION-COMPLETE.md](../../CFM-1638-IMPLEMENTATION-COMPLETE.md)  
> **Próximo Passo:** **03-prescricoes-digitais-finalizacao.md**  
> **Última Atualização:** 24 de Janeiro de 2026
