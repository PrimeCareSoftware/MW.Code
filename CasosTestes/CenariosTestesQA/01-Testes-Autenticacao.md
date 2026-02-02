# 01 - Cenários de Testes de Autenticação

> **Módulo:** Autenticação e Autorização  
> **Tempo estimado:** 30-40 minutos  
> **Pré-requisitos:** Sistema configurado e rodando

## 📋 Índice

1. [Objetivo dos Testes](#objetivo-dos-testes)
2. [Preparação](#preparação)
3. [Casos de Teste](#casos-de-teste)
4. [Critérios de Aceite](#critérios-de-aceite)
5. [Bugs Conhecidos](#bugs-conhecidos)

## 🎯 Objetivo dos Testes

Validar que o sistema de autenticação funciona corretamente, incluindo:
- ✅ Login com email e senha
- ✅ Autenticação de dois fatores (2FA)
- ✅ Recuperação de senha
- ✅ Controle de acesso por roles
- ✅ Sessões e tokens JWT
- ✅ Logout
- ✅ Proteção contra brute force

## 🔧 Preparação

### Dados de Teste

Certifique-se que os seguintes usuários existem no banco (criados pelo seed):

| Email | Senha | Role | 2FA Habilitado |
|-------|-------|------|----------------|
| admin@demo.com | Admin@123 | SystemAdmin | Não |
| doctor@demo.com | Doctor@123 | Doctor | Não |
| secretary@demo.com | Secretary@123 | Secretary | Não |

### URLs de Teste

- **Frontend:** http://localhost:4200
- **Backend API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger

## 📝 Casos de Teste

### CT-AUTH-001: Login Bem-Sucedido com Admin

**Objetivo:** Verificar que um usuário admin consegue fazer login

**Pré-condições:**
- Sistema rodando
- Usuário admin@demo.com existe no banco

**Passos:**
1. Acesse http://localhost:4200
2. Na tela de login, digite:
   - Email: `admin@demo.com`
   - Senha: `Admin@123`
3. Clique em "Entrar"

**Resultado Esperado:**
- ✅ Usuário é redirecionado para o dashboard
- ✅ Nome do usuário aparece no header (Admin)
- ✅ Menu lateral mostra todas as opções de admin
- ✅ Token JWT é armazenado no localStorage

**Validações Adicionais:**
```javascript
// Abrir DevTools (F12) > Console
localStorage.getItem('primecare_token') !== null
// Deve retornar: true
```

**Prioridade:** 🔴 Crítica

---

### CT-AUTH-002: Login com Credenciais Inválidas

**Objetivo:** Verificar que login falha com credenciais incorretas

**Pré-condições:**
- Sistema rodando

**Passos:**
1. Acesse http://localhost:4200
2. Digite:
   - Email: `admin@demo.com`
   - Senha: `SenhaErrada123`
3. Clique em "Entrar"

**Resultado Esperado:**
- ✅ Mensagem de erro é exibida: "Email ou senha incorretos"
- ✅ Usuário permanece na tela de login
- ✅ Campo de senha é limpo
- ✅ Não há token no localStorage

**Prioridade:** 🔴 Crítica

---

### CT-AUTH-003: Login com Email Inválido

**Objetivo:** Validação de formato de email

**Passos:**
1. Acesse http://localhost:4200
2. Digite:
   - Email: `emailinvalido`
   - Senha: `Admin@123`
3. Clique em "Entrar"

**Resultado Esperado:**
- ✅ Mensagem de validação: "Email inválido"
- ✅ Botão "Entrar" pode estar desabilitado
- ✅ Requisição não é enviada ao backend

**Prioridade:** 🟡 Média

---

### CT-AUTH-004: Login com Campos Vazios

**Objetivo:** Verificar validação de campos obrigatórios

**Passos:**
1. Acesse http://localhost:4200
2. Deixe email e senha vazios
3. Tente clicar em "Entrar"

**Resultado Esperado:**
- ✅ Mensagens de validação aparecem:
  - "Email é obrigatório"
  - "Senha é obrigatória"
- ✅ Botão "Entrar" está desabilitado

**Prioridade:** 🟡 Média

---

### CT-AUTH-005: Proteção contra Brute Force

**Objetivo:** Verificar que conta é bloqueada após múltiplas tentativas

**Passos:**
1. Acesse http://localhost:4200
2. Tente fazer login 5 vezes seguidas com senha errada:
   - Email: `admin@demo.com`
   - Senha: `SenhaErrada`

**Resultado Esperado:**
- ✅ Após 5 tentativas, conta é temporariamente bloqueada
- ✅ Mensagem: "Conta bloqueada. Tente novamente em 15 minutos"
- ✅ Login com senha correta também falha durante o bloqueio
- ✅ Após 15 minutos, login volta a funcionar

**Validação Backend:**
```bash
# Ver logs de tentativas
docker-compose logs backend | grep "Login attempt"
```

**Prioridade:** 🔴 Crítica (Segurança)

---

### CT-AUTH-006: Recuperação de Senha

**Objetivo:** Testar fluxo de recuperação de senha

**Passos:**
1. Na tela de login, clique em "Esqueci minha senha"
2. Digite email: `doctor@demo.com`
3. Clique em "Enviar email de recuperação"
4. Verifique o email (Mailtrap ou console do backend)
5. Clique no link de recuperação
6. Digite nova senha: `NovaSenh@123`
7. Confirme a senha
8. Clique em "Redefinir senha"

**Resultado Esperado:**
- ✅ Mensagem de confirmação: "Email enviado com sucesso"
- ✅ Email recebido com link válido
- ✅ Link expira em 1 hora
- ✅ Senha é alterada com sucesso
- ✅ Login funciona com nova senha
- ✅ Login com senha antiga falha

**Prioridade:** 🔴 Crítica

---

### CT-AUTH-007: Ativar 2FA (Autenticação de Dois Fatores)

**Objetivo:** Configurar 2FA para um usuário

**Passos:**
1. Faça login com `admin@demo.com`
2. Vá para "Meu Perfil" > "Segurança"
3. Clique em "Ativar Autenticação de Dois Fatores"
4. Use um app autenticador (Google Authenticator, Authy) para escanear o QR Code
5. Digite o código de 6 dígitos exibido no app
6. Clique em "Confirmar"

**Resultado Esperado:**
- ✅ QR Code é exibido
- ✅ Código de recuperação é gerado e exibido (salvar!)
- ✅ 2FA é ativado com sucesso
- ✅ Mensagem de confirmação aparece
- ✅ Badge "2FA Ativo" aparece no perfil

**Prioridade:** 🟡 Média

---

### CT-AUTH-008: Login com 2FA Ativo

**Objetivo:** Testar login quando 2FA está habilitado

**Pré-condições:**
- 2FA ativado para `admin@demo.com` (CT-AUTH-007)

**Passos:**
1. Faça logout
2. Faça login novamente com `admin@demo.com` / `Admin@123`
3. Tela de 2FA é exibida
4. Abra o app autenticador
5. Digite o código de 6 dígitos
6. Clique em "Verificar"

**Resultado Esperado:**
- ✅ Após senha correta, tela de 2FA aparece
- ✅ Código de 6 dígitos é aceito
- ✅ Redirecionado para dashboard
- ✅ Login completo com 2FA

**Prioridade:** 🟡 Média

---

### CT-AUTH-009: Login com 2FA - Código Inválido

**Objetivo:** Verificar que código 2FA incorreto não permite login

**Pré-condições:**
- 2FA ativado para `admin@demo.com`

**Passos:**
1. Faça login com email e senha corretos
2. Na tela de 2FA, digite: `000000`
3. Clique em "Verificar"

**Resultado Esperado:**
- ✅ Mensagem de erro: "Código inválido"
- ✅ Permanece na tela de 2FA
- ✅ Permite nova tentativa

**Prioridade:** 🟡 Média

---

### CT-AUTH-010: Controle de Acesso por Role - Admin

**Objetivo:** Verificar que admin tem acesso completo

**Pré-condições:**
- Logado como `admin@demo.com`

**Passos:**
1. Verifique o menu lateral

**Resultado Esperado:**
- ✅ Visualiza todas as opções:
  - Dashboard
  - Pacientes
  - Médicos
  - Agendamentos
  - Prontuários
  - Prescrições
  - Relatórios/Analytics
  - CRM
  - Configurações
  - Usuários
  - LGPD

**Prioridade:** 🔴 Crítica

---

### CT-AUTH-011: Controle de Acesso por Role - Doctor

**Objetivo:** Verificar que médico tem acesso limitado

**Pré-condições:**
- Logado como `doctor@demo.com`

**Passos:**
1. Verifique o menu lateral

**Resultado Esperado:**
- ✅ Visualiza apenas:
  - Dashboard
  - Agenda (seus agendamentos)
  - Pacientes (seus pacientes)
  - Prontuários (que ele criou)
  - Prescrições
- ❌ NÃO visualiza:
  - Configurações do sistema
  - Gestão de usuários
  - Relatórios completos
  - CRM

**Prioridade:** 🔴 Crítica (Segurança)

---

### CT-AUTH-012: Controle de Acesso por Role - Secretary

**Objetivo:** Verificar que secretária tem acesso intermediário

**Pré-condições:**
- Logado como `secretary@demo.com`

**Passos:**
1. Verifique o menu lateral

**Resultado Esperado:**
- ✅ Visualiza:
  - Dashboard
  - Pacientes (CRUD completo)
  - Agendamentos (CRUD completo)
  - Fila de espera
- ❌ NÃO visualiza:
  - Prontuários médicos
  - Prescrições
  - Relatórios financeiros
  - Configurações

**Prioridade:** 🔴 Crítica (Segurança)

---

### CT-AUTH-013: Expiração de Token JWT

**Objetivo:** Verificar que token expira após tempo configurado

**Pré-condições:**
- Token configurado para expirar em 60 minutos (padrão)

**Passos:**
1. Faça login com qualquer usuário
2. Aguarde 65 minutos (ou altere JWT_EXPIRATION_MINUTES para 1 minuto para teste rápido)
3. Tente acessar qualquer página

**Resultado Esperado:**
- ✅ Após expiração, usuário é deslogado automaticamente
- ✅ Redirecionado para tela de login
- ✅ Mensagem: "Sessão expirada. Faça login novamente"

**Prioridade:** 🟡 Média

---

### CT-AUTH-014: Logout

**Objetivo:** Verificar que logout funciona corretamente

**Pré-condições:**
- Usuário logado

**Passos:**
1. Clique no avatar/nome do usuário no header
2. Clique em "Sair"

**Resultado Esperado:**
- ✅ Usuário é redirecionado para tela de login
- ✅ Token é removido do localStorage
- ✅ Tentativa de acessar páginas protegidas redireciona para login

**Validação:**
```javascript
// DevTools > Console
localStorage.getItem('primecare_token')
// Deve retornar: null
```

**Prioridade:** 🔴 Crítica

---

### CT-AUTH-015: Tentativa de Acesso Direto a URL Protegida

**Objetivo:** Verificar que URLs protegidas não são acessíveis sem login

**Pré-condições:**
- Usuário NÃO logado

**Passos:**
1. Abra o navegador em modo anônimo
2. Tente acessar diretamente: `http://localhost:4200/dashboard`

**Resultado Esperado:**
- ✅ Redirecionado automaticamente para `/login`
- ✅ URL de destino é preservada: `/login?returnUrl=/dashboard`
- ✅ Após login, redireciona para `/dashboard`

**Prioridade:** 🔴 Crítica (Segurança)

---

### CT-AUTH-016: Refresh Token

**Objetivo:** Verificar renovação automática do token

**Pré-condições:**
- Token JWT com 60 minutos de validade

**Passos:**
1. Faça login
2. Use o sistema normalmente
3. Após 50 minutos, faça uma requisição à API

**Resultado Esperado:**
- ✅ Token é renovado automaticamente antes de expirar
- ✅ Usuário não precisa fazer login novamente
- ✅ Experiência contínua sem interrupções

**Validação:**
```javascript
// DevTools > Network > Headers
// Verificar header Authorization: Bearer <novo_token>
```

**Prioridade:** 🟡 Média

---

## ✅ Critérios de Aceite

### Funcionalidade Básica
- [ ] Login com credenciais válidas funciona
- [ ] Login com credenciais inválidas falha apropriadamente
- [ ] Validações de campos funcionam
- [ ] Logout funciona corretamente

### Segurança
- [ ] Proteção contra brute force ativa
- [ ] Tokens JWT funcionam corretamente
- [ ] 2FA funciona quando habilitado
- [ ] Controle de acesso por role funciona
- [ ] URLs protegidas não são acessíveis sem autenticação

### Recuperação de Senha
- [ ] Email de recuperação é enviado
- [ ] Link de recuperação funciona
- [ ] Nova senha pode ser definida
- [ ] Senha antiga não funciona mais

### UX/UI
- [ ] Mensagens de erro são claras
- [ ] Loading states são exibidos
- [ ] Validações são em tempo real
- [ ] Interface é responsiva

## 🐛 Bugs Conhecidos

Nenhum bug conhecido no módulo de autenticação no momento.

## 📊 Relatório de Testes

Após completar todos os testes, preencha:

| CT | Descrição | Status | Observações |
|----|-----------|--------|-------------|
| CT-AUTH-001 | Login Admin | ⬜ Não testado | |
| CT-AUTH-002 | Credenciais Inválidas | ⬜ Não testado | |
| CT-AUTH-003 | Email Inválido | ⬜ Não testado | |
| ... | ... | ... | ... |

**Status:**
- ✅ Passou
- ❌ Falhou
- ⚠️ Passou com ressalvas
- ⬜ Não testado

## 📚 Documentação Relacionada

- [API 2FA Documentation](../../API_2FA_DOCUMENTATION.md)
- [Authentication Architecture](../../AUTHENTICATION_ARCHITECTURE.txt)
- [Guia do Usuário 2FA](../../GUIA_USUARIO_2FA.md)
- [MFA Setup User Guide](../../MFA_SETUP_USER_GUIDE.md)

## ⏭️ Próximos Passos

Após completar os testes de autenticação:
1. ✅ Todos os casos de teste executados
2. ➡️ Vá para [02-Testes-Agendamento.md](02-Testes-Agendamento.md)

---

**Encontrou um bug?** Documente com screenshots e passos para reproduzir, depois abra uma issue no GitHub.
