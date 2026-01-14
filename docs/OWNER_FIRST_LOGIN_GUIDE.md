# Guia de Primeiro Acesso do Proprietário

Este documento explica o fluxo completo desde o registro da clínica até o primeiro acesso do proprietário e criação de usuários adicionais.

## 📋 Resumo do Fluxo

1. **Registro da Clínica** (no site)
2. **Primeiro Login do Proprietário** (na aplicação principal)
3. **Configuração Inicial**
4. **Criação de Usuários e Perfis**

---

## 1️⃣ Registro da Clínica

### Passo a Passo

1. Acesse o site principal em `http://localhost:5000` (desenvolvimento)
2. Clique em "Cadastre-se" ou "Preços"
3. Preencha o formulário de registro:
   - **Passo 1**: Dados da Clínica (nome, CNPJ, telefone, email)
   - **Passo 2**: Endereço
   - **Passo 3**: Dados do Proprietário (nome, CPF, telefone, email)
   - **Passo 4**: Credenciais de Acesso (usuário e senha)
   - **Passo 5**: Escolha do Plano
   - **Passo 6**: Confirmação e aceitação de termos

4. Após o registro bem-sucedido, você será redirecionado para a página de confirmação com:
   - **Tenant ID** (identificador único da sua clínica)
   - **Nome de usuário**
   - **Nome da clínica**

### ⚠️ Importante

- **GUARDE** seu Tenant ID e nome de usuário em local seguro
- Você precisará dessas informações para fazer login
- O Tenant ID é único e identifica sua clínica no sistema

---

## 2️⃣ Primeiro Login do Proprietário

### Acesso à Aplicação

1. Acesse a aplicação principal em `http://localhost:4200` (desenvolvimento)
2. Você verá a tela de login

### Como Fazer Login como Proprietário

**MUITO IMPORTANTE:** Proprietários devem marcar a opção **"Login como Proprietário"** na tela de login.

#### Opção 1: Login com Subdomínio (Recomendado)

Se sua clínica tiver um subdomínio configurado (ex: `minhaclinica.primecare.com`):

1. Acesse através do subdomínio
2. Digite seu **usuário**
3. Digite sua **senha**
4. ✅ **MARQUE** a caixa "Login como Proprietário"
5. Clique em "Entrar"

#### Opção 2: Login com Tenant ID

Se não estiver usando subdomínio:

1. Digite seu **usuário**
2. Digite sua **senha**
3. Digite seu **Tenant ID**
4. ✅ **MARQUE** a caixa "Login como Proprietário"
5. Clique em "Entrar"

### 🚨 Problema Comum: Erro de Login

**Sintoma:** "Usuário ou senha incorretos" mesmo com credenciais corretas.

**Causa:** Você não marcou a opção "Login como Proprietário".

**Solução:** 
1. Certifique-se de que a caixa "Login como Proprietário" está marcada
2. Tente fazer login novamente

---

## 3️⃣ Entendendo os Tipos de Login

### Dois Tipos de Autenticação

O sistema possui dois tipos distintos de usuários:

#### 👑 Proprietário (Owner)
- Criado automaticamente durante o registro da clínica
- Tem acesso completo ao sistema
- Responsável por gerenciar todos os usuários
- Pode criar e configurar perfis de acesso
- Usa o endpoint `/auth/owner-login`
- **DEVE** marcar "Login como Proprietário" ao fazer login

#### 👤 Usuário Regular (User)
- Criado pelo proprietário após o primeiro acesso
- Pode ser: médico, dentista, enfermeiro, recepcionista, secretária
- Tem permissões baseadas em seu perfil de acesso
- Usa o endpoint `/auth/login`
- **NÃO** deve marcar "Login como Proprietário"

---

## 4️⃣ Responsabilidades do Proprietário

### Após o Primeiro Login

Como proprietário, você é responsável por:

1. **Gerenciar Usuários**
   - Criar contas para médicos, secretárias, recepcionistas, etc.
   - Ativar/desativar usuários
   - Editar informações de usuários

2. **Configurar Perfis de Acesso**
   - O sistema cria perfis padrão automaticamente:
     - Perfil de Proprietário (acesso completo)
     - Perfil Médico
     - Perfil de Recepção
     - Perfil Financeiro
   - Você pode criar perfis personalizados conforme necessário

3. **Configurar a Clínica**
   - Horários de atendimento
   - Especialidades
   - Procedimentos
   - Personalização visual

---

## 5️⃣ Criando Usuários Adicionais

### Como Criar um Novo Usuário

1. Faça login como proprietário
2. Acesse o menu **"Usuários"** ou **"Configurações > Usuários"**
3. Clique em **"Novo Usuário"** ou **"Adicionar Usuário"**
4. Preencha os dados:
   - Nome completo
   - Email
   - Telefone
   - Nome de usuário
   - Senha inicial
   - Função (médico, secretária, etc.)
   - Perfil de acesso (opcional - se não especificar, usa permissões baseadas na função)
   - CRM/CRO (se aplicável)
   - Especialidade (se aplicável)

5. Clique em **"Salvar"** ou **"Criar"**

### Informações para o Novo Usuário

Após criar um usuário, compartilhe com ele:

- **Nome de usuário**
- **Senha inicial** (ele deve alterá-la no primeiro acesso)
- **Tenant ID** (o mesmo da clínica)
- **Instruções:** NÃO marcar "Login como Proprietário" ao fazer login

---

## 6️⃣ Permissões do Proprietário

### O Que o Proprietário Pode Fazer

Como proprietário, você tem **acesso completo** a todos os recursos do sistema:

✅ Gerenciar usuários (criar, editar, ativar, desativar)  
✅ Configurar perfis de acesso  
✅ Gerenciar configurações da clínica  
✅ Visualizar e gerenciar pacientes  
✅ Visualizar e gerenciar agendamentos  
✅ Visualizar prontuários médicos  
✅ Gerenciar financeiro (pagamentos, despesas, notas fiscais)  
✅ Visualizar relatórios (financeiros e operacionais)  
✅ Gerenciar procedimentos  
✅ Configurar notificações  
✅ Gerenciar fila de espera  
✅ Gerenciar assinatura da clínica  

### Implementação Técnica

O sistema automaticamente concede todas as permissões aos proprietários através do atributo `RequirePermissionKeyAttribute` no backend. Quando um proprietário faz login:

1. O token JWT inclui `role: "Owner"`
2. Todas as validações de permissão verificam se o usuário é um Owner
3. Owners bypassam as verificações de permissões granulares
4. Owners têm acesso irrestrito a todas as funcionalidades

---

## 7️⃣ Troubleshooting

### Problema: "Usuário ou senha incorretos"

**Verificações:**
1. ✅ Você está usando as credenciais corretas?
2. ✅ Você marcou "Login como Proprietário"?
3. ✅ Você está usando o Tenant ID correto (se não estiver usando subdomínio)?
4. ✅ Sua senha está correta? (senhas são case-sensitive)

### Problema: "Acesso negado" ao tentar criar usuários

**Verificações:**
1. ✅ Você fez login como proprietário?
2. ✅ Você marcou "Login como Proprietário" na tela de login?
3. ✅ Seu token JWT está válido? (tente fazer logout e login novamente)

### Problema: Não consigo ver o menu de usuários e perfis

**Causa:** Você provavelmente fez login como usuário regular em vez de proprietário.

**Solução:**
1. Faça logout
2. Faça login novamente
3. **Marque** a opção "Login como Proprietário"
4. Após o login bem-sucedido, você verá o menu **"Administração"** na barra de navegação
5. Clique em "Administração" para acessar:
   - **Usuários**: Criar e gerenciar usuários da clínica
   - **Perfis de Acesso**: Criar e gerenciar perfis personalizados
   - **Informações da Clínica**: Editar dados da clínica
   - **Personalização**: Customizar aparência (logo, cores)
   - **Assinatura**: Gerenciar plano e pagamentos

---

## 8️⃣ Fluxo Técnico (Para Desenvolvedores)

### Registro da Clínica

```
POST /api/registration
{
  "clinicName": "...",
  "clinicCNPJ": "...",
  "username": "...",
  "password": "...",
  ...
}
```

**Backend:**
1. `RegistrationService.RegisterClinicWithOwnerAsync()`
2. Cria Clinic, Owner, ClinicSubscription, AccessProfiles
3. Retorna TenantID, subdomain, ownerUsername

### Login do Proprietário

```
POST /auth/owner-login
{
  "username": "...",
  "password": "...",
  "tenantId": "..."
}
```

**Backend:**
1. `AuthService.AuthenticateOwnerAsync()` busca no `OwnerRepository`
2. Valida senha usando `PasswordHasher`
3. Cria sessão usando `AuthService.RecordOwnerLoginAsync()`
4. Gera JWT token com `role: "Owner"`
5. Retorna token + informações do owner

### Autorização

**RequirePermissionKeyAttribute:**
```csharp
// Verifica role do token
if (roleClaim == "Owner")
{
    // Busca no OwnerRepository
    var owner = await ownerRepository.GetByIdAsync(userId, tenantId);
    
    // Owners têm todas as permissões - retorna sucesso
    return;
}

// Para users regulares, verifica permissões específicas
var user = await userRepository.GetByIdAsync(userId, tenantId);
if (!user.HasPermissionKey(requiredPermissionKey))
{
    return Forbidden;
}
```

---

## 9️⃣ Arquitetura do Sistema

### Separação Owner vs User

```
┌─────────────────┐         ┌──────────────────┐
│   Owners Table  │         │   Users Table    │
├─────────────────┤         ├──────────────────┤
│ - Id            │         │ - Id             │
│ - Username      │         │ - Username       │
│ - PasswordHash  │         │ - PasswordHash   │
│ - Email         │         │ - Email          │
│ - ClinicId      │         │ - ClinicId       │
│ - TenantId      │         │ - TenantId       │
│ - IsActive      │         │ - Role           │
│                 │         │ - ProfileId      │
│                 │         │ - IsActive       │
└─────────────────┘         └──────────────────┘
        │                            │
        │                            │
        v                            v
┌─────────────────┐         ┌──────────────────┐
│  owner-login    │         │     login        │
│   endpoint      │         │   endpoint       │
└─────────────────┘         └──────────────────┘
```

### Por que Dois Tipos?

1. **Segurança**: Separação clara entre administradores (owners) e usuários operacionais
2. **Permissões**: Owners têm acesso completo, users têm permissões granulares
3. **Auditoria**: Facilita rastrear ações de administração vs operação
4. **Escalabilidade**: Permite múltiplos owners por clínica (futuramente)

---

## 🔗 Links Relacionados

- [Documentação de Autenticação](./AUTH_DOCUMENTATION.md)
- [Guia de Perfis de Acesso](./ACCESS_PROFILES_GUIDE.md)
- [API de Usuários](./API_USERS.md)

---

## 📞 Suporte

Se você continuar tendo problemas com o primeiro login:

1. Verifique os logs do backend para mensagens de erro detalhadas
2. Verifique se o Owner foi criado corretamente no banco de dados
3. Verifique se o Owner está ativo (`IsActive = true`)
4. Entre em contato com o suporte técnico

---

**Última atualização:** 2025-01-14  
**Versão:** 1.0
