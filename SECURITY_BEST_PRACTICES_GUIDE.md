# 🔐 Security Best Practices Guide - PrimeCare System Admin

**Version:** 1.0  
**Last Updated:** Janeiro 2026  
**Status:** ✅ Fase 6 Completa

---

## 📋 Sumário

1. [Autenticação e MFA](#autenticação-e-mfa)
2. [Autorização e Permissões](#autorização-e-permissões)
3. [Audit Logging](#audit-logging)
4. [LGPD Compliance](#lgpd-compliance)
5. [Segurança de Dados](#segurança-de-dados)
6. [Incident Response](#incident-response)

---

## 🔑 Autenticação e MFA

### Multi-Factor Authentication (MFA)

O sistema suporta múltiplos métodos de autenticação de dois fatores:

#### 1. TOTP (Time-Based One-Time Password)

**Aplicativos suportados:**
- Google Authenticator
- Microsoft Authenticator
- Authy
- 1Password

**Configuração:**
1. Acesse Configurações → Segurança
2. Clique em "Habilitar MFA"
3. Escaneie o QR Code com seu aplicativo autenticador
4. Digite o código de 6 dígitos para confirmar
5. Salve os códigos de backup em local seguro

**Exemplo de uso:**
```typescript
// Frontend - Habilitar MFA
const setup = await mfaService.setupTotp();
console.log('Secret Key:', setup.secretKey);
console.log('QR Code:', setup.qrCode);
console.log('Backup Codes:', setup.backupCodes);
```

```csharp
// Backend - Verificar código TOTP
var isValid = await _mfaService.VerifyTotp(userId, code, tenantId);
if (isValid) {
    // Login autorizado
}
```

#### 2. SMS

**Configuração:**
1. Acesse Configurações → Segurança
2. Adicione um número de telefone válido
3. Clique em "Enviar código de teste"
4. Digite o código recebido

**Nota:** SMS deve ser usado como método secundário devido a vulnerabilidades conhecidas (SIM swapping).

#### 3. Códigos de Backup

**Importante:**
- Gere 10 códigos de backup durante a configuração
- Armazene em local seguro (gerenciador de senhas)
- Cada código pode ser usado apenas uma vez
- Regenere códigos após usar metade deles

### Detecção de Login Suspeito

O sistema detecta automaticamente logins suspeitos baseado em:

1. **Novo endereço IP**
2. **Nova localização geográfica**
3. **Novo dispositivo/navegador**
4. **Viagem impossível** (mudança de país em menos de 1 hora)

**Ações automáticas quando detectado:**
- Exige verificação MFA adicional
- Envia notificação para o usuário
- Registra no audit log
- Alerta administradores se configurado

**Exemplo:**
```csharp
var loginAttempt = new LoginAttemptDto
{
    IpAddress = request.IpAddress,
    UserAgent = request.UserAgent,
    Country = await GetCountryFromIp(request.IpAddress)
};

var isSuspicious = await _anomalyDetection.IsLoginSuspicious(userId, loginAttempt, tenantId);

if (isSuspicious || user.MfaEnabled)
{
    // Exigir MFA
    return RequireMfaResponse(user);
}
```

### Políticas de Senha

**Requisitos mínimos:**
- 8+ caracteres
- Letras maiúsculas e minúsculas
- Números
- Caracteres especiais
- Não pode ser senha comum (123456, password, etc.)

**Recomendações:**
- Use 12+ caracteres
- Use um gerenciador de senhas
- Não reutilize senhas
- Ative MFA sempre que possível

---

## 👮 Autorização e Permissões

### Sistema de Permissões Granular

O sistema usa permissões granulares no formato `resource.action`:

#### Recursos Disponíveis
- `clinic` - Clínicas
- `users` - Usuários
- `profiles` - Perfis de acesso
- `patients` - Pacientes
- `appointments` - Consultas
- `medical-records` - Prontuários
- `procedures` - Procedimentos
- `payments` - Pagamentos
- `reports` - Relatórios
- `data` - Dados (LGPD)

#### Ações Disponíveis
- `view` - Visualizar
- `create` - Criar
- `edit` - Editar
- `delete` - Excluir
- `export` - Exportar
- `manage` - Gerenciar (todas as ações)

#### Exemplos de Permissões

```
clinic.view          - Visualizar clínica
clinic.manage        - Gerenciar clínica
users.create         - Criar usuários
users.edit           - Editar usuários
patients.view        - Visualizar pacientes
patients.manage      - Gerenciar pacientes
data.export          - Exportar dados (LGPD)
data.delete          - Anonimizar dados (LGPD)
```

### Uso em Controllers

```csharp
using MedicSoft.Application.Authorization;

[RequirePermission("clinic.manage")]
[HttpPost]
public async Task<ActionResult<ClinicDto>> CreateClinic(CreateClinicDto dto)
{
    // Apenas usuários com permissão clinic.manage podem criar clínicas
    var clinic = await _service.CreateAsync(dto);
    return Ok(clinic);
}

[RequirePermission("data.export")]
[HttpGet("clinics/{id}/export")]
public async Task<IActionResult> ExportClinicData(Guid id)
{
    // Apenas usuários com permissão data.export podem exportar
    var data = await _gdprService.ExportClinicDataAsync(id, TenantId);
    return File(data, "application/json", $"clinic-{id}-data.json");
}
```

### Roles Pré-Definidos

#### SystemAdmin
```
Permissões: TODAS
Descrição: Acesso completo ao sistema
```

#### ClinicOwner
```
Permissões:
- clinic.manage
- users.manage
- patients.manage
- appointments.manage
- medical-records.manage
- payments.manage
- reports.view
```

#### Doctor/Dentist
```
Permissões:
- patients.view
- patients.manage
- appointments.view
- appointments.manage
- medical-records.view
- medical-records.manage
```

#### Nurse
```
Permissões:
- patients.view
- appointments.view
- medical-records.view
- medical-records.manage
```

#### Receptionist
```
Permissões:
- patients.view
- patients.manage
- appointments.view
- appointments.manage
```

---

## 📝 Audit Logging

### O que é Registrado

**100% das ações são registradas:**
- Autenticação (login, logout, falhas)
- Acesso a dados sensíveis
- Modificações de dados
- Exportação de dados
- Exclusão/anonimização de dados
- Mudanças de permissões
- Configurações de segurança

### Estrutura do Audit Log

```csharp
public class AuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public AuditAction Action { get; set; }
    public string ActionDescription { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string EntityDisplayName { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string OldValues { get; set; }  // JSON
    public string NewValues { get; set; }  // JSON
    public List<string> ChangedFields { get; set; }
    public AuditSeverity Severity { get; set; }  // INFO, WARNING, CRITICAL
}
```

### Níveis de Severidade

- **INFO** - Operações normais (leitura, criação)
- **WARNING** - Operações sensíveis (falha de login, acesso negado)
- **CRITICAL** - Operações críticas (exclusão, anonimização, mudança de permissões)

### Retenção de Dados

- **Logs INFO:** 1 ano
- **Logs WARNING:** 2 anos
- **Logs CRITICAL:** 5 anos (ou mais por requisitos legais)

### Consultas Úteis

```csharp
// Obter atividade de um usuário
var logs = await _auditService.GetUserActivityAsync(
    userId, 
    startDate: DateTime.UtcNow.AddDays(-30),
    endDate: DateTime.UtcNow,
    tenantId
);

// Obter histórico de uma entidade
var history = await _auditService.GetEntityHistoryAsync(
    "Patient",
    patientId,
    tenantId
);

// Obter eventos de segurança
var securityEvents = await _auditService.GetSecurityEventsAsync(
    startDate: DateTime.UtcNow.AddDays(-7),
    endDate: DateTime.UtcNow,
    tenantId
);
```

### Alertas Automáticos

Ações críticas geram alertas automáticos:

```csharp
if (log.Severity == AuditSeverity.CRITICAL)
{
    await _alertingService.SendAlert(new AlertDto
    {
        Title = "Ação Crítica Executada",
        Message = $"{log.UserName} executou: {log.Action} em {log.EntityType}",
        Severity = "high",
        Recipients = GetSecurityTeam()
    });
}
```

---

## 🛡️ LGPD Compliance

### Direitos dos Titulares

#### 1. Direito de Acesso (Art. 18, I e II)

**Implementação:**
```csharp
[RequirePermission("data.export")]
[HttpGet("users/{id}/export-data")]
public async Task<IActionResult> ExportUserData(string id)
{
    var data = await _gdprService.ExportUserDataAsync(id, TenantId);
    return File(data, "application/json", $"user-{id}-data.json");
}
```

**Dados exportados:**
- Informações pessoais
- Histórico de atividades
- Dados de saúde (se aplicável)
- Audit logs relacionados

#### 2. Direito de Exclusão/Anonimização (Art. 18, VI)

**Implementação:**
```csharp
[RequirePermission("data.delete")]
[HttpPost("users/{id}/anonymize")]
public async Task<IActionResult> AnonymizeUserData(string id)
{
    await _gdprService.AnonymizeUserDataAsync(id, TenantId, CurrentUserId);
    return Ok(new { message = "Dados anonimizados com sucesso" });
}
```

**Processo de anonimização:**
1. Validar solicitação
2. Backup dos dados originais (audit)
3. Substituir dados pessoais por valores genéricos
4. Manter relações estruturais
5. Registrar ação no audit log
6. Notificar partes interessadas

#### 3. Relatório LGPD

```csharp
[HttpGet("users/{id}/lgpd-report")]
public async Task<ActionResult<AuditReport>> GetLgpdReport(string id)
{
    var report = await _auditService.GenerateLgpdReportAsync(id, TenantId);
    return Ok(report);
}
```

**Conteúdo do relatório:**
- Total de acessos aos dados
- Modificações realizadas
- Exportações/downloads
- Compartilhamentos
- Atividade recente (50 últimas ações)

### Base Legal para Tratamento

Todas as operações devem especificar a base legal (Art. 7):

```csharp
Purpose.HEALTHCARE           // Tutela da saúde
Purpose.LEGAL_OBLIGATION     // Cumprimento de obrigação legal
Purpose.LEGITIMATE_INTEREST  // Interesse legítimo
Purpose.CONSENT              // Consentimento do titular
```

### Categorias de Dados

```csharp
DataCategory.PERSONAL        // Dados pessoais comuns
DataCategory.SENSITIVE       // Dados sensíveis (saúde)
DataCategory.FINANCIAL       // Dados financeiros
DataCategory.CLINICAL        // Dados clínicos
```

---

## 🔒 Segurança de Dados

### Criptografia

#### 1. Em Repouso (At Rest)
- Banco de dados criptografado (TDE)
- Arquivos sensíveis criptografados (AES-256)
- Backups criptografados

#### 2. Em Trânsito (In Transit)
- HTTPS/TLS 1.3
- Certificados SSL válidos
- HSTS habilitado

#### 3. Campos Sensíveis
```csharp
// MFA Secret Keys são criptografados
var encryptedSecret = _encryption.Encrypt(secretKey);
user.MfaSecretKey = encryptedSecret;

// Backup codes são hasheados
var hashedCode = _passwordHasher.HashPassword(code);
```

### Backups

**Frequência:**
- Completo: Diário (00:00 UTC)
- Incremental: A cada 6 horas
- Transacional: Contínuo

**Retenção:**
- Diários: 30 dias
- Semanais: 12 semanas
- Mensais: 12 meses

**Teste de Restore:**
- Mensal em ambiente de homologação

---

## 🚨 Incident Response

### Plano de Resposta a Incidentes

#### 1. Identificação
- Monitoramento contínuo de logs
- Alertas automáticos configurados
- Análise de anomalias

#### 2. Contenção
- Suspender contas comprometidas
- Revogar sessões ativas
- Isolar sistemas afetados

#### 3. Erradicação
- Remover acessos não autorizados
- Corrigir vulnerabilidades
- Atualizar credenciais

#### 4. Recuperação
- Restore de backups se necessário
- Verificar integridade dos dados
- Monitoramento intensivo

#### 5. Lições Aprendidas
- Documentar incidente
- Atualizar procedimentos
- Treinar equipe

### Contatos de Emergência

```
Security Team: security@primecare.com
Emergency Hotline: +55 (11) XXXX-XXXX
LGPD DPO: dpo@primecare.com
```

### Notificação de Vazamento (LGPD Art. 48)

**Prazo:** Comunicar ANPD em prazo razoável (recomendado: 72h)

**Informações obrigatórias:**
1. Natureza dos dados
2. Titulares afetados
3. Medidas tomadas
4. Riscos relacionados
5. Motivos da demora (se aplicável)

---

## 📚 Referências

- [LGPD - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CIS Controls](https://www.cisecurity.org/controls)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

---

**Última revisão:** Janeiro 2026  
**Próxima revisão:** Julho 2026
