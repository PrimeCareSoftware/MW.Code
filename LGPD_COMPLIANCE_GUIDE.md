# 🛡️ Guia de Compliance LGPD - PrimeCare

**Lei Geral de Proteção de Dados - Lei 13.709/2018**  
**Versão:** 1.0  
**Atualizado:** Janeiro 2026

---

## 📋 Sumário

1. [Visão Geral da LGPD](#visão-geral-da-lgpd)
2. [Direitos dos Titulares](#direitos-dos-titulares)
3. [Bases Legais](#bases-legais)
4. [Categorias de Dados](#categorias-de-dados)
5. [Implementação Técnica](#implementação-técnica)
6. [Processos e Procedimentos](#processos-e-procedimentos)
7. [Gestão de Incidentes](#gestão-de-incidentes)
8. [Checklist de Compliance](#checklist-de-compliance)

---

## 📖 Visão Geral da LGPD

### O que é a LGPD?

A Lei Geral de Proteção de Dados (LGPD - Lei 13.709/2018) é a legislação brasileira que regula o tratamento de dados pessoais, incluindo em meios digitais, por pessoa natural ou pessoa jurídica de direito público ou privado.

### Princípios da LGPD

1. **Finalidade** - Propósito legítimo e específico
2. **Adequação** - Compatível com finalidade informada
3. **Necessidade** - Limitado ao mínimo necessário
4. **Livre acesso** - Consulta facilitada e gratuita
5. **Qualidade dos dados** - Exatidão, clareza, relevância
6. **Transparência** - Informações claras e acessíveis
7. **Segurança** - Medidas técnicas e administrativas
8. **Prevenção** - Medidas para evitar danos
9. **Não discriminação** - Impossibilidade de tratamento discriminatório
10. **Responsabilização** - Demonstração de conformidade

### Papéis na LGPD

**Titular:** Pessoa natural a quem se referem os dados pessoais  
**Controlador:** Quem decide sobre o tratamento (Clínicas)  
**Operador:** Quem trata dados em nome do controlador (PrimeCare)  
**Encarregado (DPO):** Canal de comunicação entre controlador, titulares e ANPD

---

## 👤 Direitos dos Titulares

### Art. 18 - Direitos Garantidos

#### 1. Confirmação e Acesso (Art. 18, I e II)

**Direito:** Saber se a empresa trata seus dados e acessá-los.

**Implementação no PrimeCare:**

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
- Informações cadastrais
- Histórico de atividades
- Prontuários médicos (se aplicável)
- Audit logs relacionados

**Prazo:** 15 dias (Art. 18, §1º)

---

#### 2. Correção (Art. 18, III)

**Direito:** Corrigir dados incompletos, inexatos ou desatualizados.

**Implementação:**
- Usuário pode editar seus próprios dados
- Solicitações via suporte são atendidas

---

#### 3. Anonimização, Bloqueio ou Eliminação (Art. 18, IV)

**Direito:** Anonimizar, bloquear ou eliminar dados desnecessários, excessivos ou tratados em desconformidade.

**Implementação no PrimeCare:**

```csharp
[RequirePermission("data.delete")]
[HttpPost("users/{id}/anonymize")]
public async Task<IActionResult> AnonymizeUserData(string id)
{
    await _gdprService.AnonymizeUserDataAsync(id, TenantId, CurrentUserId);
    return Ok(new { message = "Dados anonimizados com sucesso" });
}
```

**Processo de Anonimização:**
1. Validar solicitação
2. Verificar se há obrigações legais de retenção
3. Criar backup para audit
4. Substituir dados pessoais por valores genéricos
5. Registrar ação no audit log (severity: CRITICAL)
6. Notificar partes interessadas

**Exceções:**
- Dados necessários para cumprimento de obrigação legal (CFM 1821/2007)
- Prontuários médicos devem ser mantidos por no mínimo 20 anos
- Dados podem ser mantidos anonimizados para fins estatísticos

---

#### 4. Portabilidade (Art. 18, V)

**Direito:** Receber dados em formato estruturado e interoperável.

**Formato:** JSON (padrão REST API)

**Dados incluídos:**
- Informações pessoais
- Histórico de consultas
- Prescrições e exames
- Atividades registradas

---

#### 5. Informação sobre Compartilhamento (Art. 18, VII)

**Direito:** Saber com quem os dados foram compartilhados.

**Implementação:**
- Audit logs registram todos os acessos
- Relatório de compartilhamento disponível

```csharp
// Obter todos os usuários que acessaram dados de um paciente
var accessLogs = await _auditService.GetEntityHistoryAsync(
    "Patient",
    patientId,
    tenantId
);

var usersWhoAccessed = accessLogs
    .Select(l => l.UserName)
    .Distinct()
    .ToList();
```

---

#### 6. Revogação de Consentimento (Art. 18, IX)

**Direito:** Revogar consentimento a qualquer momento.

**Implementação:**
- Sistema de consentimentos granular
- Revogação via interface ou solicitação
- Efeito imediato

**Consequências:**
- Interrupção do tratamento baseado em consentimento
- Manutenção apenas se houver outra base legal
- Possível impossibilidade de continuar prestando serviços

---

## ⚖️ Bases Legais

### Art. 7 - Bases Legais para Tratamento

O PrimeCare utiliza as seguintes bases legais:

#### 1. Consentimento (Art. 7, I)

**Quando:** Para envio de comunicações de marketing, newsletters

**Características:**
- Livre, informado e inequívoco
- Por escrito ou meio equivalente
- Destacado das demais cláusulas
- Pode ser revogado a qualquer momento

**Implementação:**
```csharp
public class UserConsent
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public ConsentType Type { get; set; } // Marketing, Newsletter, etc
    public bool Granted { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string Purpose { get; set; }
}
```

---

#### 2. Cumprimento de Obrigação Legal (Art. 7, II)

**Quando:** Para cumprimento de obrigações legais e regulatórias

**Exemplos:**
- Manutenção de prontuários médicos (CFM 1821/2007)
- Emissão de notas fiscais (Receita Federal)
- Registros trabalhistas (CLT)

**Características:**
- Não requer consentimento
- Dados devem ser mantidos pelo tempo obrigatório
- Anonimização só após fim da obrigação

---

#### 3. Tutela da Saúde (Art. 7, VIII e Art. 11, II, f)

**Quando:** Para prestação de serviços de saúde

**Exemplos:**
- Prontuários médicos
- Prescrições
- Resultados de exames
- Histórico de consultas

**Características:**
- Aplicável a dados sensíveis de saúde
- Profissionais de saúde, serviços de saúde ou autoridades sanitárias
- Procedimentos em conformidade com resoluções do CFM/CRM

---

#### 4. Exercício Regular de Direitos (Art. 7, VI)

**Quando:** Para exercer direitos em processo judicial, administrativo ou arbitral

**Exemplos:**
- Defesa em processos judiciais
- Resposta a auditorias
- Investigações internas

---

#### 5. Legítimo Interesse (Art. 7, IX)

**Quando:** Para situações legítimas não cobertas por outras bases

**Exemplos:**
- Prevenção de fraudes
- Segurança da informação
- Melhoria de serviços (analytics anonimizados)

**Teste de Legítimo Interesse:**
1. **Finalidade legítima?** ✓
2. **Necessário?** ✓
3. **Balanceamento:** Interesse > Impacto no titular? ✓

---

## 📊 Categorias de Dados

### Dados Pessoais Comuns

Dados relacionados a pessoa identificada ou identificável (Art. 5, I)

**Exemplos no PrimeCare:**
- Nome, email, telefone
- Endereço, CPF, RG
- Data de nascimento
- Profissão

**Tratamento:** Base legal necessária

---

### Dados Pessoais Sensíveis

Dados que podem gerar discriminação (Art. 5, II)

**Exemplos no PrimeCare:**
- **Origem racial ou étnica** (autodeclaração)
- **Dados de saúde** ⭐ (principal categoria)
- **Vida sexual**
- **Biometria** (se implementado)

**Tratamento:** Base legal específica mais restritiva (Art. 11)

**Atenção especial:**
- Requer medidas de segurança reforçadas
- Acesso restrito apenas aos autorizados
- Audit log obrigatório para todos os acessos

---

### Dados de Crianças e Adolescentes

**Menores de 18 anos** (Art. 14)

**Requisitos:**
- Consentimento de um dos pais ou responsável
- Linguagem clara e acessível
- Informações sobre coleta claras

**Implementação:**
```csharp
public class Patient
{
    public DateTime BirthDate { get; set; }
    
    public bool IsMinor()
    {
        var age = DateTime.Today.Year - BirthDate.Year;
        return age < 18;
    }
    
    public Guid? GuardianId { get; set; } // Se menor
    public User? Guardian { get; set; }
}
```

---

## 💻 Implementação Técnica

### Arquitetura de Segurança

```
┌─────────────────────────────────────────┐
│          Interface do Usuário           │
│  (HTTPS/TLS 1.3 - Criptografia)        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         API Layer (ASP.NET Core)        │
│  • Autenticação (JWT + MFA)             │
│  • Autorização (Permissões Granulares)  │
│  • Rate Limiting                        │
│  • Audit Middleware                     │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Service Layer (Business Logic)    │
│  • GDPR Service                         │
│  • Audit Service                        │
│  • Anonymization Service                │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Data Layer (Repository Pattern)   │
│  • Encryption at field level            │
│  • Change tracking                      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│    Database (PostgreSQL/SQL Server)     │
│  • TDE (Transparent Data Encryption)    │
│  • Backups criptografados               │
│  • Row-level security                   │
└─────────────────────────────────────────┘
```

### Categorização Automática

```csharp
public enum DataCategory
{
    PERSONAL,        // CPF, RG, email, telefone
    SENSITIVE,       // Dados de saúde, raça
    FINANCIAL,       // Dados bancários, pagamentos
    CLINICAL,        // Prontuários, diagnósticos
    BEHAVIORAL       // Histórico de uso, preferências
}

// Uso nos audit logs
await _auditService.LogDataAccessAsync(
    userId: currentUser.Id,
    userName: currentUser.Name,
    userEmail: currentUser.Email,
    entityType: "Patient",
    entityId: patient.Id,
    entityDisplayName: patient.Name,
    ipAddress: request.IpAddress,
    userAgent: request.UserAgent,
    requestPath: request.Path,
    httpMethod: request.Method,
    tenantId: tenant.Id,
    dataCategory: DataCategory.SENSITIVE,  // ⬅️ Categorização
    purpose: LgpdPurpose.HEALTHCARE
);
```

### Minimização de Dados

**Princípio:** Coletar apenas dados necessários

**Implementação:**
- Formulários de cadastro simplificados
- Campos opcionais claramente marcados
- Revisão periódica de campos obrigatórios

```csharp
public class CreatePatientDto
{
    [Required] public string Name { get; set; } = null!;
    [Required] public DateTime BirthDate { get; set; }
    
    // Opcional - apenas se necessário
    public string? Email { get; set; }
    public string? Cpf { get; set; }
    
    // Sensível - requer justificativa
    public string? EthnicOrigin { get; set; }
    public string? HealthInsurance { get; set; }
}
```

### Anonimização vs Pseudonimização

**Anonimização:**
- Irreversível
- Dados não podem ser vinculados ao titular
- Não está mais sob LGPD

```csharp
// Após anonimização
patient.Name = $"Patient-{Guid.NewGuid()}";      // Patient-a1b2c3...
patient.Cpf = "***";
patient.Email = $"anonymized-{Guid.NewGuid()}@example.com";
```

**Pseudonimização:**
- Reversível com informação adicional
- Ainda é dado pessoal
- Continua sob LGPD

```csharp
// Pseudonimização (não implementado no PrimeCare atualmente)
var pseudonym = _crypto.Hash(patient.Cpf, secretKey);
patient.CpfHash = pseudonym;
```

---

## 📋 Processos e Procedimentos

### Fluxo de Solicitação LGPD

```
Titular → Solicitação → Canal de Atendimento → Validação → Execução → Resposta
    ↓           ↓              ↓                   ↓           ↓          ↓
  Email      Ticket        DPO/Suporte         Identidade   Sistema   15 dias
  Portal     Telefone      Registra no           CPF        API        máx.
  Presencial Formulário    Sistema              Foto       Manual
```

### 1. Solicitação de Acesso aos Dados

**Canais:**
- Email: lgpd@primecare.com
- Portal: Sistema → Segurança → Meus Dados
- Presencial: Clínica/Consultório

**Processo:**
1. Titular faz solicitação
2. Validação de identidade (CPF + foto/documento)
3. Sistema gera export JSON
4. Envio seguro ao titular (download criptografado ou email)
5. Registro no audit log

**Prazo:** 15 dias corridos

---

### 2. Solicitação de Anonimização/Exclusão

**Avaliação:**
1. Verificar se há obrigação legal de retenção
2. Verificar se há processos judiciais em andamento
3. Verificar dependências de dados

**Obrigações de Retenção:**
- Prontuários médicos: 20 anos (CFM 1821/2007)
- Notas fiscais: 5 anos (Código Tributário)
- Dados trabalhistas: 5 anos após fim do vínculo

**Processo:**
```
Solicitação → Análise → Backup → Anonimização → Verificação → Confirmação
     ↓           ↓         ↓          ↓            ↓            ↓
  Registro   Legal      Audit    API GDPR      Testes      Titular
  Ticket     DPO      Completo   Service      Quality     Notificado
```

**Prazo:** 15 dias corridos (pode ser prorrogado justificadamente)

---

### 3. Consentimento e Revogação

**Obtenção de Consentimento:**
- Checkbox específico para cada finalidade
- Linguagem clara e objetiva
- Separado de outros termos
- Registrado em banco de dados

**Revogação:**
- Simples quanto dar o consentimento
- Efeito imediato
- Registro no audit log

```typescript
// Frontend - Gestão de Consentimentos
interface ConsentManagement {
  marketing: boolean;         // Emails promocionais
  newsletter: boolean;        // Newsletter mensal
  analytics: boolean;         // Dados para melhoria do serviço
  thirdPartySharing: boolean; // Compartilhamento com parceiros
}
```

---

## 🚨 Gestão de Incidentes

### Definição de Incidente de Segurança

**Art. 48 LGPD:** Incidente que possa acarretar risco ou dano relevante aos titulares.

**Exemplos:**
- Acesso não autorizado a dados
- Vazamento de dados
- Perda de dados
- Ransomware
- Roubo de dispositivos

### Classificação de Incidentes

| Nível | Descrição | Exemplo | Notificação ANPD |
|-------|-----------|---------|------------------|
| **Baixo** | Dados não sensíveis, poucas pessoas | Email de um usuário exposto | Não obrigatória |
| **Médio** | Dados pessoais, número moderado | Lista de pacientes vazada | Recomendada |
| **Alto** | Dados sensíveis, muitas pessoas | Prontuários acessados indevidamente | **Obrigatória** |
| **Crítico** | Dados sensíveis em massa, risco iminente | Ransomware, banco de dados exposto | **Urgente** |

### Plano de Resposta a Incidentes (IRP)

#### Fase 1: Detecção e Análise (0-2h)

**Ações:**
1. Identificar o incidente
2. Classificar severidade
3. Isolar sistemas afetados
4. Acionar equipe de resposta

**Equipe de Resposta:**
- DPO (Encarregado)
- TI/Segurança
- Jurídico
- Comunicação

---

#### Fase 2: Contenção (2-8h)

**Ações:**
1. Bloquear acesso não autorizado
2. Revogar credenciais comprometidas
3. Isolar sistemas afetados
4. Preservar evidências

```csharp
// Suspender usuário comprometido
await _userService.SuspendUserAsync(compromisedUserId);

// Revogar todas as sessões
await _sessionService.RevokeAllSessionsAsync(compromisedUserId);

// Log crítico
await _auditService.LogAsync(new CreateAuditLogDto {
    Action = AuditAction.SECURITY_INCIDENT,
    Severity = AuditSeverity.CRITICAL,
    Details = "User account compromised - all access revoked"
});
```

---

#### Fase 3: Erradicação (8-24h)

**Ações:**
1. Identificar causa raiz
2. Remover ameaças
3. Corrigir vulnerabilidades
4. Atualizar sistemas

---

#### Fase 4: Recuperação (24-72h)

**Ações:**
1. Restaurar sistemas
2. Verificar integridade dos dados
3. Restabelecer operações
4. Monitoramento intensivo

---

#### Fase 5: Notificação

**Prazo ANPD:** Razoável (recomendado 72h)

**Conteúdo da Notificação:**
1. Descrição do incidente
2. Dados envolvidos
3. Titulares afetados (quantidade estimada)
4. Medidas tomadas
5. Riscos identificados
6. Medidas de mitigação
7. Motivo da demora (se aplicável)

**Template de Comunicação aos Titulares:**
```
Assunto: Notificação de Incidente de Segurança - PrimeCare

Prezado(a) [Nome],

Informamos que em [data] identificamos um incidente de segurança
que pode ter afetado seus dados pessoais.

Dados potencialmente afetados:
- [Lista de dados]

Ações tomadas:
- [Medidas de contenção]
- [Correções implementadas]

Riscos identificados:
- [Riscos para o titular]

Recomendações:
- [Alterar senha]
- [Monitorar contas]
- [Ativar MFA]

Para mais informações: lgpd@primecare.com

Atenciosamente,
Equipe PrimeCare
```

---

#### Fase 6: Lições Aprendidas

**Documentar:**
- Cronologia do incidente
- Ações tomadas
- Efetividade das medidas
- Melhorias necessárias

**Atualizar:**
- Procedimentos de segurança
- Treinamentos
- Políticas internas

---

## ✅ Checklist de Compliance

### Governança

- [x] DPO nomeado e divulgado
- [x] Política de privacidade publicada
- [x] Termo de uso atualizado
- [x] Treinamento anual da equipe
- [ ] RIPD (Relatório de Impacto) para tratamentos de alto risco
- [x] Inventário de dados atualizado

### Bases Legais

- [x] Base legal definida para cada tratamento
- [x] Sistema de consentimentos implementado
- [x] Revogação de consentimento funcional
- [x] Documentação das bases legais

### Direitos dos Titulares

- [x] Canal de atendimento LGPD
- [x] Processo de resposta em 15 dias
- [x] Export de dados implementado
- [x] Anonimização implementada
- [x] Portabilidade (JSON) implementada
- [x] Correção de dados funcional

### Segurança

- [x] Criptografia em trânsito (HTTPS/TLS)
- [x] Criptografia em repouso (TDE)
- [x] Controle de acesso (RBAC)
- [x] MFA disponível
- [x] Audit log completo
- [x] Backups criptografados
- [x] Plano de resposta a incidentes
- [x] Testes de segurança regulares

### Ciclo de Vida dos Dados

- [x] Minimização de dados
- [x] Qualidade e atualização
- [x] Retenção definida
- [x] Eliminação segura
- [x] Revisão periódica

### Transparência

- [x] Política de privacidade clara
- [x] Aviso de cookies
- [x] Informações sobre tratamento
- [x] Compartilhamento divulgado
- [x] Contato do DPO visível

### Contratos

- [ ] Cláusulas LGPD em contratos com parceiros
- [ ] Acordo de processamento de dados (DPA)
- [ ] Verificação de compliance de fornecedores
- [ ] Cláusulas de responsabilidade

---

## 📚 Referências Legais

### Legislação

- **LGPD:** [Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- **CFM 1821/2007:** Prontuário médico
- **Código de Ética Médica**
- **Marco Civil da Internet:** Lei 12.965/2014

### Órgãos Reguladores

- **ANPD:** Autoridade Nacional de Proteção de Dados
  - Site: https://www.gov.br/anpd
  - Email: anpd@economia.gov.br

- **CFM:** Conselho Federal de Medicina
  - Resolução 1821/2007 (Prontuários)

### Guias e Orientações

- [Guia de Boas Práticas LGPD - ANPD](https://www.gov.br/anpd)
- [ISO 27001/27701](https://www.iso.org)
- [GDPR (Europa)](https://gdpr.eu) - Referência internacional

---

## 📞 Contatos

### Encarregado de Dados (DPO)

**Email:** dpo@primecare.com  
**Telefone:** +55 (11) XXXX-XXXX  
**Horário:** Segunda a Sexta, 9h às 18h

### Canal de Atendimento LGPD

**Email:** lgpd@primecare.com  
**Portal:** https://primecare.com.br/lgpd  
**Resposta:** Até 15 dias corridos

---

**Última Atualização:** Janeiro 2026  
**Próxima Revisão:** Julho 2026  
**Versão:** 1.0
