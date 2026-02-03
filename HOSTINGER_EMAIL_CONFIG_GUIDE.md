# Guia de Configuração de Email - Hostinger

Este documento fornece instruções detalhadas sobre como configurar o envio de emails via SMTP utilizando sua hospedagem Hostinger para o sistema de autenticação de dois fatores (2FA) e outras notificações.

## Índice

1. [Visão Geral](#visão-geral)
2. [Pré-requisitos](#pré-requisitos)
3. [Como Obter Dados de Email da Hostinger](#como-obter-dados-de-email-da-hostinger)
4. [Configuração no Sistema](#configuração-no-sistema)
5. [Testando a Configuração](#testando-a-configuração)
6. [Solução de Problemas](#solução-de-problemas)
7. [Melhores Práticas](#melhores-práticas)

## Visão Geral

O sistema Omni Care agora envia emails diretamente através de SMTP (Simple Mail Transfer Protocol) ao invés de usar APIs externas como SendGrid. Isso oferece:

- ✅ Maior controle sobre o envio de emails
- ✅ Sem dependência de serviços externos pagos
- ✅ Uso direto da infraestrutura de email da sua hospedagem
- ✅ Melhor privacidade dos dados

## Pré-requisitos

Antes de começar, você precisará:

1. Uma conta de hospedagem ativa na Hostinger
2. Pelo menos uma conta de email criada no painel da Hostinger
3. Acesso ao painel de controle (hPanel) da Hostinger
4. Acesso ao servidor onde a API está hospedada

## Como Obter Dados de Email da Hostinger

### Passo 1: Acessar o Painel da Hostinger

1. Acesse [hpanel.hostinger.com](https://hpanel.hostinger.com)
2. Faça login com suas credenciais
3. Selecione o plano de hospedagem desejado

### Passo 2: Criar ou Localizar Conta de Email

Se você ainda não possui uma conta de email:

1. No painel lateral, clique em **"Emails"**
2. Clique em **"Criar Conta de Email"**
3. Escolha o nome da conta (ex: `noreply`, `notificacoes`, `sistema`)
4. Escolha uma senha forte
5. Clique em **"Criar"**

**Recomendação**: Use um endereço específico para o sistema, como:
- `noreply@seudominio.com.br` - Para emails automáticos
- `notificacoes@seudominio.com.br` - Para notificações
- `2fa@seudominio.com.br` - Específico para 2FA

### Passo 3: Obter Configurações SMTP

As configurações SMTP da Hostinger são:

#### Servidor SMTP (Hostinger Padrão)
```
Servidor SMTP: smtp.hostinger.com
Porta: 587 (recomendada - com TLS/STARTTLS)
Porta alternativa: 465 (com SSL)
Criptografia: TLS/STARTTLS (porta 587) ou SSL (porta 465)
```

#### Dados de Autenticação
```
Usuário: seu-email@seudominio.com.br (email completo)
Senha: a senha que você criou para a conta de email
```

### Passo 4: Verificar Configurações no hPanel

Para confirmar as configurações:

1. No painel da Hostinger, vá em **"Emails"**
2. Clique nos **três pontinhos** ao lado da conta de email
3. Selecione **"Configurar Email"**
4. Você verá as configurações de SMTP/IMAP para sua conta

**Exemplo de Configuração Completa:**
```
Servidor SMTP: smtp.hostinger.com
Porta SMTP: 587
Segurança: STARTTLS
Usuário: noreply@seudominio.com.br
Senha: SuaSenhaForte123!
```

## Configuração no Sistema

### Para Patient Portal API

Edite o arquivo `appsettings.json` ou `appsettings.Production.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.hostinger.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "noreply@seudominio.com.br",
    "Password": "SuaSenhaForte123!",
    "From": "noreply@seudominio.com.br",
    "FromName": "Omni Care - Portal do Paciente"
  }
}
```

### Para API Principal (MedicSoft.Api)

Edite o arquivo `appsettings.json` ou `appsettings.Production.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.hostinger.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "noreply@seudominio.com.br",
    "Password": "SuaSenhaForte123!",
    "From": "noreply@seudominio.com.br",
    "FromName": "Omni Care Software",
    "Enabled": true
  }
}
```

### Usando Variáveis de Ambiente (Recomendado para Produção)

Para maior segurança, use variáveis de ambiente:

```bash
# Linux/Mac
export EMAIL_SMTP_SERVER="smtp.hostinger.com"
export EMAIL_SMTP_PORT="587"
export EMAIL_USERNAME="noreply@seudominio.com.br"
export EMAIL_PASSWORD="SuaSenhaForte123!"
export EMAIL_FROM="noreply@seudominio.com.br"
export EMAIL_FROM_NAME="Omni Care Software"

# Windows PowerShell
$env:EMAIL_SMTP_SERVER="smtp.hostinger.com"
$env:EMAIL_SMTP_PORT="587"
$env:EMAIL_USERNAME="noreply@seudominio.com.br"
$env:EMAIL_PASSWORD="SuaSenhaForte123!"
$env:EMAIL_FROM="noreply@seudominio.com.br"
$env:EMAIL_FROM_NAME="Omni Care Software"
```

Ou adicione ao arquivo `.env`:

```env
# Configurações de Email - Hostinger
EMAIL_SMTP_SERVER=smtp.hostinger.com
EMAIL_SMTP_PORT=587
EMAIL_USE_SSL=true
EMAIL_USERNAME=noreply@seudominio.com.br
EMAIL_PASSWORD=SuaSenhaForte123!
EMAIL_FROM=noreply@seudominio.com.br
EMAIL_FROM_NAME=Omni Care Software
EMAIL_ENABLED=true
```

## Testando a Configuração

### Teste Manual via Código

Você pode testar o envio de email usando um endpoint de teste ou através do console:

```csharp
// Exemplo de teste
var emailService = serviceProvider.GetRequiredService<INotificationService>();
await emailService.SendEmailAsync(
    "seu-email@exemplo.com",
    "Teste de Configuração SMTP",
    "<h1>Email de Teste</h1><p>Se você recebeu este email, a configuração está correta!</p>"
);
```

### Teste via Endpoint da API

Use o Swagger ou Postman para testar:

```bash
# Endpoint de teste (se disponível)
POST /api/test/send-email
Content-Type: application/json
Authorization: Bearer {seu-token}

{
  "to": "seu-email@exemplo.com",
  "subject": "Teste SMTP",
  "body": "<p>Teste de configuração SMTP</p>"
}
```

### Verificar Logs

Após tentar enviar um email, verifique os logs da aplicação:

```bash
# Logs da aplicação
tail -f Logs/omnicare-*.log | grep -i email
```

Você deve ver mensagens como:
```
[Information] Email sent successfully to seu-email@exemplo.com
```

## Solução de Problemas

### Problema 1: "Authentication failed"

**Causa**: Usuário ou senha incorretos

**Solução**:
1. Verifique se o usuário é o email completo: `usuario@dominio.com.br`
2. Verifique se a senha está correta
3. Tente fazer login no webmail da Hostinger para confirmar as credenciais
4. Certifique-se de que a conta de email não está bloqueada

### Problema 2: "Connection timeout" ou "Cannot connect to SMTP server"

**Causa**: Firewall ou porta bloqueada

**Solução**:
1. Verifique se a porta 587 está aberta no firewall
2. Tente usar a porta alternativa 465 com SSL
3. Verifique se o servidor tem acesso à internet
4. Teste conexão com telnet: `telnet smtp.hostinger.com 587`

### Problema 3: "Certificate validation failed"

**Causa**: Problema com certificado SSL/TLS

**Solução**:
1. Certifique-se de que `UseSsl: true` está configurado
2. Verifique se a data/hora do servidor está correta
3. Atualize os certificados raiz do sistema operacional

### Problema 4: "Sender address rejected"

**Causa**: Email de origem não autorizado

**Solução**:
1. Use o mesmo email no campo `From` e `Username`
2. Verifique se o domínio do email corresponde ao domínio da hospedagem
3. Certifique-se de que SPF/DKIM estão configurados para o domínio

### Problema 5: "Daily sending limit exceeded"

**Causa**: Limite de envio da Hostinger atingido

**Solução**:
1. Verifique os limites do seu plano de hospedagem
2. A Hostinger geralmente permite 100-500 emails/hora dependendo do plano
3. Considere implementar fila de emails para distribuir o envio
4. Entre em contato com o suporte da Hostinger para aumentar o limite

## Melhores Práticas

### 1. Segurança

- ✅ **Nunca** commite senhas no código-fonte
- ✅ Use variáveis de ambiente ou secrets management
- ✅ Use contas de email dedicadas (não use emails pessoais)
- ✅ Implemente rate limiting para evitar spam
- ✅ Use senhas fortes e únicas para contas de email

### 2. Configuração SPF, DKIM e DMARC

Para melhorar a deliverability dos emails:

1. **SPF (Sender Policy Framework)**:
   ```
   Adicione no DNS: v=spf1 include:_spf.hostinger.com ~all
   ```

2. **DKIM (DomainKeys Identified Mail)**:
   - Configure no painel da Hostinger em "Emails" > "DKIM"
   - Adicione o registro TXT fornecido no seu DNS

3. **DMARC**:
   ```
   Adicione no DNS: v=DMARC1; p=quarantine; rua=mailto:dmarc@seudominio.com.br
   ```

### 3. Monitoramento

- Configure alertas para falhas de envio
- Monitore logs regularmente
- Acompanhe taxa de entrega e bounces
- Implemente retry logic com backoff exponencial

### 4. Conteúdo dos Emails

- Use templates HTML responsivos
- Inclua versão texto plano
- Adicione link de unsubscribe (se aplicável)
- Inclua informações de contato claras
- Evite palavras que acionam filtros de spam

### 5. Limites e Performance

- Respeite os limites de envio do seu plano Hostinger
- Implemente fila de emails para alto volume
- Use conexões persistentes quando possível
- Implemente timeout apropriado (30-60 segundos)

## Informações Adicionais

### Limites Típicos da Hostinger por Plano

- **Plano Single**: ~100 emails/hora
- **Plano Premium**: ~150-200 emails/hora  
- **Plano Business**: ~300-500 emails/hora

*Nota: Os limites podem variar. Consulte sua documentação específica ou contate o suporte.*

### Portas SMTP Alternativas

Se a porta 587 não funcionar:

1. **Porta 465 com SSL**: Conexão SSL/TLS direta
2. **Porta 25**: Geralmente bloqueada por provedores, não recomendada

### Suporte

- **Documentação Hostinger**: [https://support.hostinger.com](https://support.hostinger.com)
- **Chat ao Vivo**: Disponível 24/7 no painel da Hostinger
- **Base de Conhecimento**: Busque por "SMTP" ou "configurar email"

## Conclusão

Seguindo este guia, você terá o sistema de email configurado corretamente usando sua hospedagem Hostinger. O sistema agora envia emails diretamente via SMTP, sem dependência de serviços externos.

**Próximos Passos:**

1. Configure SPF, DKIM e DMARC para melhor deliverability
2. Teste o envio de emails 2FA
3. Monitore os logs de envio
4. Configure alertas para falhas

---

**Última Atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Autor**: Omni Care Software Team
