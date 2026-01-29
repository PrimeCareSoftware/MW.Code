# Guia do Usuário: Autenticação de Dois Fatores (2FA)

## O que é Autenticação de Dois Fatores?

A Autenticação de Dois Fatores (2FA) é uma camada adicional de segurança para sua conta. Além da senha, você precisará fornecer um código temporário enviado para seu e-mail sempre que fizer login.

## Por que usar 2FA?

- **Maior Segurança**: Protege sua conta mesmo se alguém descobrir sua senha
- **Conformidade**: Ajuda a cumprir requisitos de segurança da LGPD e regulamentações médicas
- **Tranquilidade**: Você será notificado por e-mail sempre que alguém tentar acessar sua conta

## Como Funciona

### Fluxo de Login com 2FA Habilitado

1. **Acesse o Portal do Paciente**
   - Entre com seu e-mail/CPF e senha normalmente

2. **Receba o Código**
   - Você receberá um e-mail com um código de 6 dígitos
   - O código é válido por 5 minutos

3. **Digite o Código**
   - Insira o código na tela de verificação
   - Clique em "Verificar"

4. **Acesso Liberado**
   - Você será direcionado para sua página inicial

## Como Habilitar a Autenticação de Dois Fatores

### Via Portal Web

1. **Faça Login** no Portal do Paciente
2. Vá para **Meu Perfil** → **Segurança**
3. Clique em **Habilitar Autenticação de Dois Fatores**
4. Confirme sua decisão
5. Você receberá um e-mail de confirmação

### Via API (para desenvolvedores)

```http
POST /api/auth/enable-2fa
Authorization: Bearer {seu-token-jwt}
```

**Resposta de Sucesso (200):**
```json
{
  "message": "Autenticação de dois fatores habilitada com sucesso"
}
```

## Como Desabilitar a Autenticação de Dois Fatores

### Via Portal Web

1. **Faça Login** no Portal do Paciente (você precisará do código 2FA)
2. Vá para **Meu Perfil** → **Segurança**
3. Clique em **Desabilitar Autenticação de Dois Fatores**
4. Confirme sua decisão inserindo sua senha
5. Você receberá um e-mail de confirmação

### Via API (para desenvolvedores)

```http
POST /api/auth/disable-2fa
Authorization: Bearer {seu-token-jwt}
```

**Resposta de Sucesso (200):**
```json
{
  "message": "Autenticação de dois fatores desabilitada com sucesso"
}
```

## Fluxo de Login com 2FA - Detalhado

### Passo 1: Login Inicial

```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrCPF": "paciente@exemplo.com",
  "password": "SuaSenhaSegura123"
}
```

**Se 2FA estiver DESABILITADO:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "expiresAt": "2026-01-30T14:00:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "paciente@exemplo.com",
    "fullName": "João Silva",
    "twoFactorEnabled": false
  }
}
```

**Se 2FA estiver HABILITADO:**
```json
{
  "requiresTwoFactor": true,
  "tempToken": "dGVtcF90b2tlbl8xMjM=",
  "message": "Código de verificação enviado para seu e-mail"
}
```

### Passo 2: Verificação do Código 2FA

```http
POST /api/auth/verify-2fa
Content-Type: application/json

{
  "tempToken": "dGVtcF90b2tlbl8xMjM=",
  "code": "123456"
}
```

**Resposta de Sucesso (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123...",
  "expiresAt": "2026-01-30T14:00:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "paciente@exemplo.com",
    "fullName": "João Silva",
    "twoFactorEnabled": true
  }
}
```

## Reenviar Código

Se você não recebeu o código ou ele expirou:

```http
POST /api/auth/resend-2fa-code
Content-Type: application/json

{
  "tempToken": "dGVtcF90b2tlbl8xMjM="
}
```

**Limitação:** Você pode solicitar no máximo 3 códigos por hora.

## Verificar Status do 2FA

Para verificar se o 2FA está habilitado:

```http
GET /api/auth/2fa-status
Authorization: Bearer {seu-token-jwt}
```

**Resposta:**
```json
{
  "isEnabled": true
}
```

## Segurança do Código 2FA

### Características do Código

- **6 dígitos numéricos** (000000 - 999999)
- **Válido por 5 minutos**
- **Uso único** (após verificado, não pode ser reutilizado)
- **Máximo 5 tentativas de verificação**

### Proteções Implementadas

1. **Rate Limiting:**
   - Máximo 3 códigos por hora por usuário
   - Máximo 5 tentativas de verificação por código

2. **Expiração Automática:**
   - Códigos expiram após 5 minutos
   - Sistema limpa códigos antigos automaticamente

3. **Log de Auditoria:**
   - Todas as tentativas de 2FA são registradas
   - Inclui endereço IP e timestamp

4. **Notificações:**
   - E-mail quando 2FA é habilitado/desabilitado
   - E-mail de alerta para atividades suspeitas

## E-mails que Você Receberá

### 1. Código de Verificação

**Assunto:** Código de Verificação - Portal do Paciente

```
Olá João Silva,

Você solicitou um código de verificação para acessar o Portal do Paciente PrimeCare.

Seu código de verificação é:

123456

⚠️ Importante: Este código expira em 5 minutos e só pode ser usado uma vez.

Se você não solicitou este código, ignore este e-mail ou entre em contato conosco.

Atenciosamente,
Equipe PrimeCare
```

### 2. 2FA Habilitado

**Assunto:** Alteração de Segurança - Autenticação de Dois Fatores

```
Olá João Silva,

A autenticação de dois fatores foi habilitada para sua conta.

Se você não realizou esta ação, entre em contato conosco imediatamente.

Data: 29/01/2026 10:30 (Horário de Brasília)

Atenciosamente,
Equipe PrimeCare
```

### 3. 2FA Desabilitado

**Assunto:** Alteração de Segurança - Autenticação de Dois Fatores

```
Olá João Silva,

A autenticação de dois fatores foi desabilitada para sua conta.

Se você não realizou esta ação, entre em contato conosco imediatamente.

Data: 29/01/2026 11:45 (Horário de Brasília)

Atenciosamente,
Equipe PrimeCare
```

## Problemas Comuns e Soluções

### Não Recebi o Código

**Possíveis Causas:**
1. O e-mail pode estar na caixa de spam
2. Atraso na entrega do e-mail
3. E-mail incorreto cadastrado

**Soluções:**
1. Verifique sua caixa de spam
2. Aguarde alguns minutos
3. Clique em "Reenviar código"
4. Verifique se o e-mail cadastrado está correto em "Meu Perfil"

### Código Expirado

**Mensagem:** "Código inválido ou expirado"

**Solução:**
- Clique em "Reenviar código" para receber um novo código
- Códigos são válidos por apenas 5 minutos

### Muitas Tentativas Erradas

**Mensagem:** "Número máximo de tentativas excedido"

**Solução:**
- Solicite um novo código clicando em "Reenviar código"
- Após 3 códigos solicitados, aguarde 1 hora antes de tentar novamente

### Perdeu Acesso ao E-mail

**Se você não tem mais acesso ao e-mail cadastrado:**

1. **Via Recuperação de Senha:**
   - Clique em "Esqueci minha senha"
   - O link para redefinir a senha também permite desabilitar o 2FA

2. **Via Suporte:**
   - Entre em contato com nossa equipe de suporte
   - Será necessário verificar sua identidade
   - Documentos podem ser solicitados

## Melhores Práticas

### ✅ Recomendações

1. **Use um E-mail Seguro:**
   - Prefira provedores confiáveis (Gmail, Outlook, etc.)
   - Habilite 2FA no seu provedor de e-mail também

2. **Mantenha seu E-mail Atualizado:**
   - Verifique regularmente se o e-mail cadastrado está correto
   - Atualize imediatamente se mudar de e-mail

3. **Não Compartilhe Códigos:**
   - Nunca compartilhe códigos 2FA com ninguém
   - Nossa equipe nunca solicitará seu código

4. **Monitore Notificações:**
   - Fique atento aos e-mails de alteração de segurança
   - Reporte atividades suspeitas imediatamente

### ⚠️ Cuidados

1. **Não Use E-mails Compartilhados:**
   - Evite e-mails de trabalho compartilhados
   - Não use e-mails de familiares

2. **Cuidado com Phishing:**
   - Nunca clique em links suspeitos
   - Verifique o remetente dos e-mails
   - Acesse o portal apenas pelo endereço oficial

3. **Dispositivos Seguros:**
   - Use dispositivos confiáveis para acessar o portal
   - Mantenha antivírus atualizado
   - Evite redes WiFi públicas

## Suporte

### Contato

- **E-mail:** suporte@primecaresoftware.com
- **Telefone:** (XX) XXXX-XXXX
- **Horário:** Segunda a Sexta, 8h às 18h

### Informações Úteis ao Contatar o Suporte

Tenha em mãos:
- Seu nome completo
- CPF
- E-mail cadastrado
- Descrição detalhada do problema
- Mensagens de erro (se houver)
- Data e hora da tentativa de login

## Perguntas Frequentes

### O 2FA é obrigatório?

Não, o 2FA é opcional mas **altamente recomendado** para proteger sua conta e seus dados médicos sensíveis.

### Posso usar 2FA em múltiplos dispositivos?

Sim! O 2FA é vinculado ao seu e-mail, não ao dispositivo. Você pode fazer login de qualquer dispositivo.

### O 2FA funciona offline?

Não, você precisa de conexão à internet para receber o código por e-mail.

### Quanto tempo leva para receber o código?

Normalmente, o código chega em alguns segundos. Em casos raros, pode levar até 2 minutos.

### Posso escolher outro método de 2FA?

**Em breve!** Estamos trabalhando para adicionar:
- SMS
- WhatsApp
- Aplicativos autenticadores (Google Authenticator, Microsoft Authenticator)

### O que acontece se meu e-mail for hackeado?

Se seu e-mail for comprometido:
1. Altere a senha do e-mail imediatamente
2. Entre em contato com nosso suporte
3. Redefina sua senha do portal
4. Considere cadastrar um novo e-mail

## Changelog

### Versão 1.0 - Janeiro 2026
- ✨ Lançamento inicial do 2FA por e-mail
- 📧 Códigos de 6 dígitos com validade de 5 minutos
- 🔒 Rate limiting e proteção contra brute force
- 📱 Notificações de alterações de segurança
- 📊 Log completo de auditoria

### Próximas Versões (Roadmap)

#### Versão 1.1 - Fevereiro 2026
- 📱 2FA por SMS
- 💬 2FA por WhatsApp
- 🔑 Códigos de recuperação (backup codes)

#### Versão 1.2 - Março 2026
- 📲 Suporte a aplicativos autenticadores (TOTP)
- 🌍 Autenticação baseada em localização
- 📊 Dashboard de atividades de segurança

---

© 2026 PrimeCare Software. Todos os direitos reservados.
