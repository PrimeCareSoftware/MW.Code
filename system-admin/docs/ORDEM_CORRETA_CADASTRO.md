# Ordem Correta de Cadastro - Referência Rápida

## ⚠️ IMPORTANTE: Autenticação Sempre Obrigatória

A partir desta versão, a autenticação JWT é **SEMPRE OBRIGATÓRIA**. A funcionalidade de desabilitar autenticação foi removida por questões de segurança.

---

## 🎯 Sequência Correta de Cadastro

### 1️⃣ Planos de Assinatura (Já existem no sistema)
- **Não precisa criar** - Os planos já estão configurados no banco de dados
- Trial, Basic, Standard, Premium, Enterprise

### 2️⃣ Registro de Clínica (Endpoint Público)
**Endpoint**: `POST /api/registration`
- ✅ Cria a clínica
- ✅ Cria o primeiro usuário (ClinicOwner)
- ✅ Cria a assinatura
- ✅ Gera o TenantId

**Importante**: Guarde o `clinicId` retornado - ele será seu `tenantId`!

### 3️⃣ Login (Obter Token JWT)
**Endpoint**: `POST /api/auth/login`
```json
{
  "username": "seu-username",
  "password": "sua-senha",
  "tenantId": "clinicId-do-passo-2"
}
```

**Importante**: Guarde o token JWT retornado!

### 4️⃣ Adicionar Header de Autenticação
Em **TODAS** as próximas requisições, adicione:
```
Authorization: Bearer {seu-token-jwt}
X-Tenant-Id: {seu-clinicId}
```

### 5️⃣ Cadastrar Usuários Adicionais
**Endpoint**: `POST /api/users` (Requer autenticação)
- Médicos, dentistas, recepcionistas, etc.

### 6️⃣ Cadastrar Pacientes
**Endpoint**: `POST /api/patients` (Requer autenticação)

### 7️⃣ Cadastrar Procedimentos
**Endpoint**: `POST /api/procedures` (Requer autenticação)

### 8️⃣ Criar Agendamentos
**Endpoint**: `POST /api/appointments` (Requer autenticação)

---

## 🔑 Requisitos de Senha

Todas as senhas devem ter:
- ✅ Mínimo 8 caracteres
- ✅ Pelo menos uma letra minúscula (a-z)
- ✅ Pelo menos uma letra maiúscula (A-Z)
- ✅ Pelo menos um dígito (0-9)
- ✅ Pelo menos um caractere especial (!@#$%^&*...)

**Exemplo de senha válida**: `MedicWare2024!@#`

---

## 🚨 Erros Comuns

### "401 Unauthorized"
- Verifique se adicionou o header `Authorization: Bearer {token}`
- Verifique se o token não expirou (validade de 60 minutos)
- Faça login novamente se necessário

### "CNPJ already registered"
- Use outro CNPJ ou faça login com a clínica existente

### "Username already taken"
- Escolha outro username ou faça login com o existente

### "Password validation failed"
- Verifique se a senha atende todos os requisitos listados acima

### "Invalid credentials"
- Verifique username, senha e tenantId
- Certifique-se de que o usuário está ativo

---

## 📖 Documentação Completa

Para guia detalhado com exemplos Swagger e Postman:
👉 [SYSTEM_SETUP_GUIDE.md](./SYSTEM_SETUP_GUIDE.md)

---

## ✅ Checklist de Verificação

Antes de começar a usar o sistema, certifique-se de que:

- [ ] Backend está rodando (dotnet run no projeto Api)
- [ ] Banco de dados está criado e acessível
- [ ] JWT SecretKey está configurada (mínimo 32 caracteres)
- [ ] Você completou o registro de clínica (Passo 2)
- [ ] Você fez login e obteve o token JWT (Passo 3)
- [ ] Você está adicionando os headers de autenticação em todas as requisições (Passo 4)

---

## 💡 Dica Importante

**Para Swagger**: Após fazer login, clique no botão "Authorize" 🔒 no topo da página e cole o token no formato:
```
Bearer {seu-token-aqui}
```

**Para Postman**: Configure as variáveis de ambiente para automatizar o processo:
- `token`: Seu token JWT
- `tenant_id`: Seu clinicId
- `base_url`: URL da API
