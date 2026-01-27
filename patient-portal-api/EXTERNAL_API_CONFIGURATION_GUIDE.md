# 🔧 Guia de Configuração de APIs Externas - Portal do Paciente

> **Status:** 95% do código está implementado. Este guia cobre a configuração final das APIs externas necessárias.  
> **Tempo Estimado:** 1-2 horas  
> **Custo:** ~R$ 100-300/mês (custos variáveis de API)

---

## 📋 Visão Geral

O Portal do Paciente utiliza duas APIs externas para o sistema de notificações automáticas:
1. **Twilio** - Envio de mensagens WhatsApp
2. **SendGrid** - Envio de emails profissionais

**Nota:** O código de integração JÁ ESTÁ implementado. Este guia cobre apenas a configuração das credenciais.

---

## 📱 1. Configuração do Twilio (WhatsApp)

### Passo 1: Criar Conta Twilio

1. Acesse [https://www.twilio.com/try-twilio](https://www.twilio.com/try-twilio)
2. Crie uma conta gratuita (trial) ou paga
3. Verifique seu número de telefone

### Passo 2: Configurar WhatsApp Sandbox (Trial) ou Número Oficial

#### Opção A: WhatsApp Sandbox (Para Testes - GRÁTIS)
1. No console Twilio, vá para **Messaging** > **Try it Out** > **Send a WhatsApp message**
2. Siga as instruções para ativar o sandbox
3. Envie a mensagem de ativação do seu WhatsApp pessoal
4. Anote o **WhatsApp Number** (formato: `whatsapp:+14155238886`)

#### Opção B: Número WhatsApp Business Oficial (Produção)
1. Solicite um número WhatsApp Business oficial via Twilio
2. Processo de aprovação pode levar 1-2 semanas
3. Custo: ~$15/mês + mensagens enviadas

### Passo 3: Obter Credenciais

No [Twilio Console](https://console.twilio.com/):
1. Vá para **Account** > **Keys & Credentials**
2. Anote os seguintes valores:
   - **Account SID** - Exemplo: `ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`
   - **Auth Token** - Exemplo: `xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`
   - **WhatsApp Number** - Exemplo: `whatsapp:+14155238886` (sandbox) ou seu número oficial

### Passo 4: Configurar no Patient Portal API

Edite o arquivo `patient-portal-api/PatientPortal.Api/appsettings.json`:

```json
{
  "TwilioSettings": {
    "AccountSid": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "AuthToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "WhatsAppFromNumber": "whatsapp:+14155238886",
    "Enabled": true
  }
}
```

Para **produção**, use `appsettings.Production.json` ou variáveis de ambiente:

```bash
# Variáveis de ambiente (recomendado para produção)
TwilioSettings__AccountSid=ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TwilioSettings__AuthToken=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TwilioSettings__WhatsAppFromNumber=whatsapp:+14155238886
TwilioSettings__Enabled=true
```

---

## 📧 2. Configuração do SendGrid (Email)

### Passo 1: Criar Conta SendGrid

1. Acesse [https://signup.sendgrid.com/](https://signup.sendgrid.com/)
2. Crie uma conta gratuita (12.000 emails/mês grátis) ou paga
3. Verifique seu email

### Passo 2: Verificar Domínio (Recomendado para Produção)

1. No [SendGrid Dashboard](https://app.sendgrid.com/):
   - Vá para **Settings** > **Sender Authentication** > **Domain Authentication**
   - Clique em **Authenticate Your Domain**
   - Siga o wizard para adicionar registros DNS (CNAME records)
   - Aguarde verificação (pode levar algumas horas)

**Por que verificar domínio?**
- ✅ Melhor deliverability (menos chance de spam)
- ✅ Remove "via sendgrid.net" do remetente
- ✅ Aumenta confiança dos pacientes

### Passo 3: Criar API Key

1. Vá para **Settings** > **API Keys**
2. Clique em **Create API Key**
3. Nome: `Patient Portal Production` (ou similar)
4. Permissões: **Full Access** (ou apenas "Mail Send" para segurança)
5. Clique em **Create & View**
6. **⚠️ ATENÇÃO CRÍTICA:** 
   - A API Key só será exibida UMA VEZ e nunca mais
   - Copie a chave IMEDIATAMENTE para um local seguro
   - Use um gerenciador de senhas (1Password, LastPass, etc.) ou variáveis de ambiente
   - NUNCA commite a chave no código ou repositório
   - Se perder a chave, terá que criar uma nova
   - Exemplo de formato: `SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

### Passo 4: Configurar Remetente

1. Vá para **Settings** > **Sender Authentication** > **Single Sender Verification**
2. Clique em **Create New Sender**
3. Preencha:
   - **From Name:** PrimeCare Software
   - **From Email:** noreply@primecare.com (ou seu domínio verificado)
   - **Reply To:** suporte@primecare.com
   - **Company Address:** Endereço da clínica
4. Verifique o email de confirmação

### Passo 5: Configurar no Patient Portal API

Edite o arquivo `patient-portal-api/PatientPortal.Api/appsettings.json`:

```json
{
  "EmailSettings": {
    "ApiKey": "SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "FromEmail": "noreply@primecare.com",
    "FromName": "PrimeCare Software",
    "Enabled": true
  }
}
```

Para **produção**, use `appsettings.Production.json` ou variáveis de ambiente:

```bash
# Variáveis de ambiente (recomendado para produção)
EmailSettings__ApiKey=SG.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
EmailSettings__FromEmail=noreply@primecare.com
EmailSettings__FromName=PrimeCare Software
EmailSettings__Enabled=true
```

---

## ⚙️ 3. Habilitar Serviço de Lembretes Automáticos

### Configurar Parâmetros do Reminder Service

Edite `appsettings.json` ou use variáveis de ambiente:

```json
{
  "AppointmentReminderSettings": {
    "Enabled": true,
    "CheckIntervalMinutes": 60,
    "AdvanceNoticeHours": 24,
    "SendWhatsApp": true,
    "SendEmail": true
  }
}
```

**Parâmetros:**
- `Enabled` - Habilita/desabilita o serviço (true para produção)
- `CheckIntervalMinutes` - Frequência de verificação (padrão: 60 min)
- `AdvanceNoticeHours` - Quantas horas antes enviar lembrete (padrão: 24h)
- `SendWhatsApp` - Habilita notificações WhatsApp
- `SendEmail` - Habilita notificações Email

---

## 🧪 4. Testar Configuração

### Teste 1: Verificar Serviço de Lembretes Iniciou

1. Inicie a aplicação:
   ```bash
   cd patient-portal-api/PatientPortal.Api
   dotnet run
   ```

2. Verifique os logs. Deve aparecer:
   ```
   Appointment Reminder Service started. Check interval: 60 minutes, Advance notice: 24 hours
   ```

### Teste 2: Envio de WhatsApp Manual

Criar endpoint de teste (opcional) ou usar o existente:

```bash
POST /api/test/send-whatsapp
{
  "toNumber": "+5511999999999",
  "message": "Teste de notificação WhatsApp - PrimeCare"
}
```

### Teste 3: Envio de Email Manual

```bash
POST /api/test/send-email
{
  "toEmail": "seuemail@example.com",
  "subject": "Teste de Notificação",
  "body": "Este é um email de teste do Patient Portal"
}
```

### Teste 4: Lembrete Automático End-to-End

1. Criar uma consulta para **exatamente 24 horas no futuro**:
   ```sql
   INSERT INTO "Appointments" (...)
   VALUES (..., NOW() + INTERVAL '24 hours', ...);
   ```

2. Aguardar o serviço executar (máx 60 min se CheckInterval = 60)
3. Verificar recebimento de WhatsApp e Email
4. Clicar no link de confirmação
5. Verificar que status mudou para "Confirmed"

---

## 📊 5. Monitoramento

### Logs do Serviço de Lembretes

Os logs incluem:
```
[INFO] Checking for appointments needing reminders...
[INFO] Found 3 appointments needing reminders
[INFO] Sending reminder for appointment {AppointmentId} to patient {PatientName}
[INFO] WhatsApp reminder sent to +5511999999999
[INFO] Email reminder sent to paciente@email.com
```

### Dashboard Twilio

- Acesse: [https://console.twilio.com/us1/monitor/logs/whatsapp](https://console.twilio.com/us1/monitor/logs/whatsapp)
- Verifique mensagens entregues, falhas, etc.

### Dashboard SendGrid

- Acesse: [https://app.sendgrid.com/statistics](https://app.sendgrid.com/statistics)
- Verifique emails enviados, abertos, cliques, bounces

---

## 💰 6. Custos Estimados

### Twilio WhatsApp

**Trial (Sandbox):**
- ✅ Gratuito
- ⚠️ Limitado a números pré-aprovados
- ⚠️ Não recomendado para produção

**Produção:**
- $0.005 por mensagem enviada (~R$ 0,025)
- Para 100 consultas/dia = R$ 75/mês
- Para 300 consultas/dia = R$ 225/mês

### SendGrid

**Free Tier:**
- ✅ 12.000 emails/mês GRÁTIS
- ✅ Suficiente para ~400 consultas/dia
- ✅ Recomendado para começar

**Essentials:**
- $19.95/mês (50.000 emails)
- Apenas se ultrapassar 12k/mês

### Total Estimado

| Cenário | Consultas/dia | WhatsApp/mês | Email/mês | Total/mês |
|---------|---------------|--------------|-----------|-----------|
| **Pequeno** | 50 | R$ 40 | Grátis | **~R$ 40** |
| **Médio** | 150 | R$ 115 | Grátis | **~R$ 115** |
| **Grande** | 300 | R$ 225 | Grátis | **~R$ 225** |

---

## ✅ Checklist de Configuração

- [ ] Conta Twilio criada e verificada
- [ ] WhatsApp Sandbox ativado (trial) OU Número oficial aprovado (prod)
- [ ] Twilio Account SID e Auth Token obtidos
- [ ] Twilio configurado em appsettings.json
- [ ] Conta SendGrid criada e verificada
- [ ] Domínio verificado no SendGrid (recomendado)
- [ ] SendGrid API Key criada
- [ ] Sender verificado no SendGrid
- [ ] SendGrid configurado em appsettings.json
- [ ] AppointmentReminderSettings habilitado (Enabled: true)
- [ ] Aplicação reiniciada com novas configurações
- [ ] Logs verificados (serviço iniciou corretamente)
- [ ] Teste de WhatsApp manual realizado
- [ ] Teste de Email manual realizado
- [ ] Teste end-to-end de lembrete automático realizado
- [ ] Monitoramento configurado (Twilio + SendGrid dashboards)

---

## 🆘 Problemas Comuns

### WhatsApp não envia

**Problema:** Mensagens não chegam  
**Soluções:**
1. Verificar se número está no formato correto: `+5511999999999` (com + e código do país)
2. Se usando Sandbox, verificar se número foi ativado (enviou "join" message)
3. Verificar Account SID e Auth Token corretos
4. Verificar logs Twilio: [console.twilio.com/monitor/logs](https://console.twilio.com/us1/monitor/logs)

### Email cai em Spam

**Problema:** Emails vão para pasta de spam  
**Soluções:**
1. Verificar domínio no SendGrid (Domain Authentication)
2. Configurar SPF, DKIM, DMARC records
3. Evitar palavras de spam no assunto ("Grátis", "Urgente", etc.)
4. Pedir pacientes adicionarem noreply@primecare.com aos contatos

### Reminder Service não executa

**Problema:** Serviço não envia lembretes  
**Soluções:**
1. Verificar `AppointmentReminderSettings.Enabled = true`
2. Verificar logs para erros de inicialização
3. Verificar se há consultas exatamente 24h no futuro
4. Ajustar `AdvanceNoticeHours` se necessário
5. Aguardar `CheckIntervalMinutes` (padrão 60 min)

---

## 📚 Documentação Adicional

- [Twilio WhatsApp API](https://www.twilio.com/docs/whatsapp)
- [SendGrid API Documentation](https://docs.sendgrid.com/)
- [NOTIFICATION_SERVICE_GUIDE.md](./NOTIFICATION_SERVICE_GUIDE.md) - Detalhes técnicos da implementação

---

**Documento Criado:** 27 de Janeiro de 2026  
**Autor:** GitHub Copilot Agent  
**Versão:** 1.0  
**Status:** ✅ Pronto para Uso
