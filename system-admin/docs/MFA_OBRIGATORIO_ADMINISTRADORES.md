# MFA Obrigatório para Administradores - Documentação Completa

> **Implementado em:** 30 de Janeiro de 2026  
> **Status:** ✅ Completo  
> **Categoria:** Segurança e Compliance (Categoria 2.3)

---

## 📋 Visão Geral

Este documento descreve a implementação de **Multi-Factor Authentication (MFA) obrigatório** para usuários com funções administrativas no Omni Care Software, conforme especificado no documento `IMPLEMENTACOES_PARA_100_PORCENTO.md`.

### Objetivos

1. **Segurança:** Proteger contas administrativas com autenticação de dois fatores
2. **Compliance:** Atender melhores práticas de segurança (NIST, ISO 27001)
3. **Flexibilidade:** Fornecer período de carência para configuração
4. **Auditoria:** Rastreamento completo de conformidade MFA

---

## 🎯 Funcionalidades Implementadas

### 1. Política de MFA por Função

MFA é **obrigatório** para as seguintes funções:
- ✅ **SystemAdmin** - Administradores do sistema
- ✅ **ClinicOwner** - Proprietários de clínicas

MFA é **opcional** para:
- Doctor, Dentist, Nurse, Receptionist, Secretary

### 2. Período de Carência

- **Duração padrão:** 7 dias (configurável)
- **Início:** No primeiro login do usuário
- **Durante o período:** Usuário pode acessar o sistema normalmente
- **Após expiração:** Acesso bloqueado até configurar MFA

### 3. Enforcement Middleware

O `MfaEnforcementMiddleware` realiza:
- ✅ Verificação automática de MFA em cada requisição
- ✅ Permite acesso durante período de carência
- ✅ Bloqueia acesso após carência expirada
- ✅ Exceções para rotas de login e configuração MFA

### 4. APIs de Gerenciamento

Novos endpoints em `/api/mfa`:

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/mfa/status` | Verificar status MFA do usuário |
| POST | `/api/mfa/setup` | Iniciar configuração MFA |
| POST | `/api/mfa/verify` | Verificar código MFA |
| POST | `/api/mfa/regenerate-backup-codes` | Regenerar códigos de backup |
| POST | `/api/mfa/disable` | Desabilitar MFA (requer verificação) |

### 5. Relatórios de Compliance

Novos endpoints em `/api/system-admin`:

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/system-admin/mfa-compliance` | Estatísticas de conformidade MFA |
| GET | `/api/system-admin/users-without-mfa` | Listar usuários sem MFA |

---

## 🔧 Configuração

### appsettings.json

```json
{
  "MfaPolicy": {
    "EnforcementEnabled": true,
    "RequiredForRoles": ["SystemAdmin", "ClinicOwner"],
    "GracePeriodDays": 7,
    "AllowBypass": false
  }
}
```

**Parâmetros:**

- `EnforcementEnabled` (bool): Ativa/desativa enforcement de MFA
- `RequiredForRoles` (string[]): Funções que requerem MFA
- `GracePeriodDays` (int): Dias de carência para configuração
- `AllowBypass` (bool): Permite bypass (apenas desenvolvimento)

---

## 📚 Guia do Usuário - Configurar MFA

### Passo 1: Primeiro Login

Após o primeiro login, se sua função requer MFA:

1. **Resposta do login incluirá:**
```json
{
  "token": "...",
  "mfaEnabled": false,
  "requiresMfaSetup": true,
  "mfaGracePeriodEndsAt": "2026-02-06T10:00:00Z"
}
```

2. **Você verá uma notificação:**
> "⚠️ MFA é obrigatório para sua função. Configure até 06/02/2026"

### Passo 2: Verificar Status

**Endpoint:** `GET /api/mfa/status`

**Resposta:**
```json
{
  "isEnabled": false,
  "requiredByPolicy": true,
  "isInGracePeriod": true,
  "gracePeriodEndsAt": "2026-02-06T10:00:00Z",
  "mustSetupNow": false
}
```

### Passo 3: Iniciar Configuração

**Endpoint:** `POST /api/mfa/setup`

**Resposta:**
```json
{
  "secretKey": "JBSWY3DPEHPK3PXP",
  "qrCodeUrl": "otpauth://totp/Omni Care:user@email.com?secret=JBSWY3DPEHPK3PXP&issuer=Omni Care",
  "backupCodes": [
    "12345678",
    "87654321",
    "..."
  ]
}
```

**Ações:**
1. Escaneie o QR Code com um app autenticador (Google Authenticator, Authy, Microsoft Authenticator)
2. **IMPORTANTE:** Salve os códigos de backup em local seguro
3. Após escanear, verifique com um código para confirmar

### Passo 4: Verificar Código

**Endpoint:** `POST /api/mfa/verify`

**Requisição:**
```json
{
  "code": "123456",
  "isBackupCode": false
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Verification successful"
}
```

### ✅ MFA Configurado!

Após verificação bem-sucedida:
- O período de carência é removido
- MFA está ativo
- Em próximos logins, será solicitado código MFA

---

## 🔐 Login com MFA Habilitado

### Fluxo de Login

1. **Login normal:**
```json
POST /api/auth/login
{
  "username": "admin",
  "password": "senha123"
}
```

2. **Resposta inclui status MFA:**
```json
{
  "token": "...",
  "mfaEnabled": true,
  "requiresMfaSetup": false,
  "mfaGracePeriodEndsAt": null
}
```

3. **Em requisições subsequentes:**
   - Se MFA está habilitado, o sistema valida automaticamente
   - Códigos de backup podem ser usados em caso de emergência

---

## 🚨 Códigos de Backup

### Quando Usar

Use códigos de backup quando:
- Perdeu acesso ao app autenticador
- Trocou de celular
- App autenticador não está funcionando

### Como Usar

**Endpoint:** `POST /api/mfa/verify`

**Requisição:**
```json
{
  "code": "12345678",
  "isBackupCode": true
}
```

⚠️ **IMPORTANTE:** Cada código de backup pode ser usado apenas **uma vez**.

### Regenerar Códigos

Se você usou muitos códigos de backup, regenere:

**Endpoint:** `POST /api/mfa/regenerate-backup-codes`

**Resposta:**
```json
{
  "backupCodes": [
    "98765432",
    "23456789",
    "..."
  ]
}
```

⚠️ **Códigos antigos são invalidados!** Salve os novos códigos.

---

## 👨‍💼 Guia do Administrador

### Verificar Conformidade MFA

**Endpoint:** `GET /api/system-admin/mfa-compliance`

**Resposta:**
```json
{
  "totalAdministrators": 25,
  "withMfaEnabled": 20,
  "withoutMfaEnabled": 5,
  "inGracePeriod": 3,
  "gracePeriodExpired": 2,
  "compliancePercentage": 80.0
}
```

**Interpretação:**
- 80% dos administradores têm MFA habilitado ✅
- 3 estão no período de carência ⚠️
- 2 têm carência expirada (bloqueados) 🔴

### Listar Usuários sem MFA

**Endpoint:** `GET /api/system-admin/users-without-mfa`

**Resposta:**
```json
[
  {
    "userId": "...",
    "username": "admin1",
    "email": "admin1@clinic.com",
    "fullName": "João Silva",
    "role": "SystemAdmin",
    "mfaEnabled": false,
    "isInGracePeriod": true,
    "gracePeriodEndsAt": "2026-02-06T10:00:00Z",
    "gracePeriodExpired": false,
    "firstLoginAt": "2026-01-30T10:00:00Z",
    "lastLoginAt": "2026-01-30T15:00:00Z",
    "clinicName": "Clínica Exemplo"
  }
]
```

**Filtrar apenas carência expirada:**
```
GET /api/system-admin/users-without-mfa?graceExpiredOnly=true
```

### Ações Administrativas

1. **Identificar usuários com carência expirada**
2. **Contatar usuários no período de carência**
3. **Monitorar taxa de conformidade**
4. **Garantir 100% de conformidade antes do prazo regulatório**

---

## 🔨 Guia Técnico de Integração

### Frontend - Verificar Status MFA

```typescript
// Após login
interface LoginResponse {
  token: string;
  mfaEnabled: boolean;
  requiresMfaSetup: boolean;
  mfaGracePeriodEndsAt?: string;
}

async function handleLogin(username: string, password: string) {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password })
  });
  
  const data: LoginResponse = await response.json();
  
  if (data.requiresMfaSetup) {
    // Redirecionar para setup MFA ou mostrar banner
    if (data.mfaGracePeriodEndsAt) {
      showWarning(`Configure MFA até ${formatDate(data.mfaGracePeriodEndsAt)}`);
    }
  }
  
  return data;
}
```

### Frontend - Configurar MFA

```typescript
async function setupMfa() {
  // 1. Iniciar setup
  const setupResponse = await fetch('/api/mfa/setup', {
    method: 'POST',
    headers: { 
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json' 
    }
  });
  
  const setupData = await setupResponse.json();
  
  // 2. Mostrar QR Code
  showQRCode(setupData.qrCodeUrl);
  
  // 3. Mostrar códigos de backup
  showBackupCodes(setupData.backupCodes);
  
  // 4. Solicitar código de verificação
  const code = await promptForCode();
  
  // 5. Verificar código
  const verifyResponse = await fetch('/api/mfa/verify', {
    method: 'POST',
    headers: { 
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json' 
    },
    body: JSON.stringify({ code, isBackupCode: false })
  });
  
  if (verifyResponse.ok) {
    showSuccess('MFA configurado com sucesso!');
  }
}
```

### Tratamento de Erro 403 (MFA Requerido)

```typescript
// Interceptor de resposta HTTP
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 403) {
      const data = error.response.data;
      if (data.error === 'MFA_REQUIRED') {
        // Redirecionar para setup MFA
        router.push('/mfa-setup');
        showError(data.message);
      }
    }
    return Promise.reject(error);
  }
);
```

---

## 🛠️ Troubleshooting

### Problema: "Grace period expired, access blocked"

**Causa:** Período de carência MFA expirou  
**Solução:**
1. Contate o administrador do sistema
2. Administrador pode temporariamente estender período via banco de dados:
```sql
UPDATE users 
SET mfa_grace_period_ends_at = NOW() + INTERVAL '7 days'
WHERE id = 'user-guid';
```

### Problema: "Invalid verification code"

**Causa:** Código TOTP incorreto ou expirado  
**Soluções:**
1. Verifique relógio do celular está sincronizado
2. Aguarde próximo código (30 segundos)
3. Use um código de backup se disponível

### Problema: "Perdi acesso ao app autenticador"

**Solução:**
1. Use um código de backup
2. Configure novo autenticador
3. Se não tem backup: contate administrador para reset

### Problema: "MFA não está sendo exigido"

**Verifique configuração:**
```json
"MfaPolicy": {
  "EnforcementEnabled": true  // Deve ser true
}
```

---

## 📊 Arquitetura Técnica

### Componentes

1. **User Entity** (`User.cs`)
   - `MfaGracePeriodEndsAt`: Data de expiração da carência
   - `FirstLoginAt`: Data do primeiro login
   - `MfaRequiredByPolicy`: Propriedade computada (role-based)
   - `IsInMfaGracePeriod`: Verifica se está na carência
   - `MfaGracePeriodExpired`: Verifica se expirou

2. **MfaController** (`MfaController.cs`)
   - Gerencia todo fluxo de configuração MFA
   - Endpoints de setup, verify, disable, regenerate

3. **MfaEnforcementMiddleware** (`MfaEnforcementMiddleware.cs`)
   - Intercepta todas requisições autenticadas
   - Valida MFA para roles administrativas
   - Bloqueia acesso se necessário

4. **SystemAdminController** (estendido)
   - `/mfa-compliance`: Estatísticas
   - `/users-without-mfa`: Listagem

5. **AuthController** (estendido)
   - Login response inclui status MFA

### Fluxo de Dados

```
Login → AuthController
  ↓
Verifica role → Se admin
  ↓
Verifica MFA → TwoFactorAuthService
  ↓
Retorna status MFA na resposta
  ↓
Requisições subsequentes → MfaEnforcementMiddleware
  ↓
Se admin sem MFA e carência expirada → 403 Forbidden
```

---

## 🧪 Testes

### Cenários de Teste

1. **Teste: Usuário admin novo (primeiro login)**
   - Login → Grace period iniciado (7 dias)
   - Requisições funcionam normalmente
   - Status MFA indica `requiresMfaSetup: true`

2. **Teste: Usuário admin configura MFA**
   - POST `/api/mfa/setup` → QR Code gerado
   - POST `/api/mfa/verify` → Verificação OK
   - Grace period removido
   - Próximas requisições requerem MFA válido

3. **Teste: Usuário admin carência expirada**
   - Grace period expirado
   - Qualquer requisição → 403 Forbidden
   - Mensagem: "MFA_REQUIRED"

4. **Teste: Usuário não-admin (Doctor)**
   - MFA não requerido
   - Middleware permite acesso sem MFA

5. **Teste: Compliance reporting**
   - GET `/api/system-admin/mfa-compliance` → Estatísticas corretas
   - GET `/api/system-admin/users-without-mfa` → Lista correta

### Scripts de Teste

```bash
# 1. Login como admin
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"senha123"}'

# 2. Verificar status MFA
curl -X GET http://localhost:5000/api/mfa/status \
  -H "Authorization: Bearer {token}"

# 3. Setup MFA
curl -X POST http://localhost:5000/api/mfa/setup \
  -H "Authorization: Bearer {token}"

# 4. Verificar compliance
curl -X GET http://localhost:5000/api/system-admin/mfa-compliance \
  -H "Authorization: Bearer {admin-token}"
```

---

## 📜 Compliance e Regulamentações

### NIST Special Publication 800-63B

✅ **Atende:** Recomendação de MFA para funções administrativas  
✅ **Atende:** Códigos de backup para recuperação  
✅ **Atende:** TOTP (Time-based One-Time Password)

### ISO/IEC 27001

✅ **A.9.4.2:** Acesso privilegiado requer autenticação forte  
✅ **A.12.4.3:** Logs de acesso administrativo (via auditoria)

### LGPD (Lei Geral de Proteção de Dados)

✅ **Art. 46:** Medidas de segurança para proteção de dados  
✅ **Art. 49:** Sistemas e aplicativos devem ser desenvolvidos com segurança

---

## 🚀 Próximos Passos (Melhorias Futuras)

### Fase 1 (Curto Prazo)
- [ ] Notificações por email sobre grace period
- [ ] Dashboard frontend para configuração MFA
- [ ] Histórico de logins com MFA

### Fase 2 (Médio Prazo)
- [ ] Suporte a WebAuthn/FIDO2
- [ ] Biometria (fingerprint, face ID)
- [ ] SMS como método alternativo

### Fase 3 (Longo Prazo)
- [ ] Análise de risco por login (geolocation, device)
- [ ] MFA adaptativo (baseado em contexto)
- [ ] Integração com SSO (SAML, OAuth)

---

## 📞 Suporte

### Dúvidas Técnicas
- Email: dev@omnicare.com.br
- Documentação: `/docs/api/mfa`

### Problemas de Acesso
- Email: suporte@omnicare.com.br
- Telefone: (11) 1234-5678

---

## 📝 Changelog

### v1.0 - 30/01/2026
- ✅ Implementação inicial de MFA obrigatório
- ✅ Middleware de enforcement
- ✅ APIs de gerenciamento
- ✅ Relatórios de compliance
- ✅ Documentação completa
- ✅ Migration de banco de dados
- ✅ Período de carência configurável

---

**Documentação criada em:** 30 de Janeiro de 2026  
**Última atualização:** 30 de Janeiro de 2026  
**Versão:** 1.0  
**Autor:** Omni Care Development Team
