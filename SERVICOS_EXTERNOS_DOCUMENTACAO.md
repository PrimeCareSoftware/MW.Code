# 📋 Documentação de Serviços Externos - Omni Care Software

**Data:** Fevereiro 2026  
**Versão:** 1.0  
**Status do MVP:** 95% Completo

## 📊 Índice

1. [Resumo Executivo](#resumo-executivo)
2. [Serviços Essenciais (Obrigatórios para MVP)](#serviços-essenciais-obrigatórios-para-mvp)
3. [Serviços de Comunicação](#serviços-de-comunicação)
4. [Serviços de Pagamento](#serviços-de-pagamento)
5. [Serviços de Compliance e Regulatório](#serviços-de-compliance-e-regulatório)
6. [Serviços de Marketing e CRM](#serviços-de-marketing-e-crm)
7. [Serviços de Monitoramento e Logs](#serviços-de-monitoramento-e-logs)
8. [Serviços Opcionais (Futuro)](#serviços-opcionais-futuro)
9. [Cronograma de Contratação](#cronograma-de-contratação)
10. [Estimativa de Custos](#estimativa-de-custos)

---

## 📌 Resumo Executivo

Este documento apresenta **todos os serviços externos** que o Omni Care Software utiliza ou utilizará. A documentação está organizada por **prioridade de contratação** baseada no lançamento do MVP e no retorno financeiro esperado.

### Status Atual dos Serviços:
- ✅ **Implementados**: 8 serviços
- ⏳ **Configurados mas não contratados**: 6 serviços
- 🔜 **Planejados para o futuro**: 4 serviços

---

## 🎯 Serviços Essenciais (Obrigatórios para MVP)

### 1. PostgreSQL (Banco de Dados)
**Status:** ✅ Implementado  
**Prioridade:** 🔴 CRÍTICA - Fase 0

**Descrição:**  
Banco de dados relacional principal do sistema. Armazena todos os dados da aplicação incluindo clínicas, pacientes, consultas, agendamentos, etc.

**Fornecedor Recomendado:**
- **Auto-hospedado no VPS**: Incluído no custo do servidor
- **Alternativa Cloud**: Amazon RDS PostgreSQL ou Azure Database for PostgreSQL

**Configuração Atual:**
```
Host: localhost / postgres (Docker)
Port: 5432
Database: primecare
```

**Recursos Necessários:**
- 2-4 GB RAM (inicialmente)
- 20-50 GB de armazenamento SSD
- Backups automáticos diários

**Custo Mensal Estimado:**
- **Auto-hospedado**: R$ 0 (incluído no VPS)
- **RDS (AWS)**: ~R$ 300-600/mês
- **Azure**: ~R$ 400-700/mês

**Justificativa:**  
Sem banco de dados, o sistema não funciona. É o alicerce de toda a aplicação.

---

### 2. Servidor de Hospedagem (VPS/Cloud)
**Status:** ✅ Implementado (Docker/Podman ready)  
**Prioridade:** 🔴 CRÍTICA - Fase 0

**Descrição:**  
Servidor Linux para hospedar a aplicação .NET, PostgreSQL, frontend Angular e demais serviços.

**Fornecedores Recomendados:**
1. **Hostinger VPS** (Recomendado para MVP)
   - VPS 2GB RAM: R$ 59/mês
   - VPS 4GB RAM: R$ 89/mês
   - VPS 8GB RAM: R$ 169/mês

2. **Amazon AWS EC2**
   - t3.small (2GB): ~R$ 150-200/mês
   - t3.medium (4GB): ~R$ 250-350/mês

3. **DigitalOcean**
   - Basic 2GB: $12/mês (~R$ 60)
   - Basic 4GB: $24/mês (~R$ 120)

**Configuração Mínima:**
- 2 vCPU
- 4GB RAM
- 80GB SSD
- 2TB de transferência/mês

**Custo Mensal Estimado:**
- **Hostinger VPS 4GB**: R$ 89/mês (RECOMENDADO)
- **AWS**: R$ 250-350/mês
- **DigitalOcean**: R$ 120/mês

**Justificativa:**  
Necessário para hospedar toda a aplicação. Hostinger oferece melhor custo-benefício para startups.

---

### 3. Domínio e SSL
**Status:** ⏳ A contratar  
**Prioridade:** 🔴 CRÍTICA - Fase 0

**Descrição:**  
Domínio personalizado e certificado SSL para acesso seguro ao sistema.

**Fornecedores Recomendados:**
- **Registro.br** (domínios .com.br)
- **GoDaddy** / **Hostinger** (domínios internacionais)
- **Let's Encrypt** (SSL gratuito)

**Necessidades:**
- 1 domínio principal: `omnicare.com.br` ou similar
- Certificado SSL (Let's Encrypt gratuito ou comercial)
- Configuração de subdomínios: `api.omnicare.com.br`, `admin.omnicare.com.br`

**Custo Mensal Estimado:**
- **Domínio .com.br**: R$ 40/ano (~R$ 3,33/mês)
- **SSL Let's Encrypt**: R$ 0 (gratuito)
- **SSL Comercial (opcional)**: R$ 20-50/mês

**Justificativa:**  
Essencial para identidade profissional e segurança (HTTPS obrigatório para dados médicos).

---

## 📧 Serviços de Comunicação

### 4. Email (SMTP)
**Status:** ✅ Implementado (Hostinger configurado)  
**Prioridade:** 🟡 ALTA - Fase 1

**Descrição:**  
Envio de emails transacionais (confirmações, lembretes, recuperação de senha, 2FA).

**Fornecedores Configurados:**

#### 4.1 Hostinger SMTP (Atual)
**Status:** ✅ Configurado
```json
"Email": {
  "SmtpServer": "smtp.hostinger.com",
  "SmtpPort": 465,
  "UseSsl": true,
  "Username": "omni@primecaretech.com.br"
}
```

**Limitações:**
- ~500-1000 emails/dia (depende do plano)
- Pode ter limitações de reputação inicial

**Custo:** Incluído no plano de hospedagem Hostinger

#### 4.2 SendGrid (Alternativa)
**Status:** ⏳ Integrado mas não contratado
**Pacote Gratuito:** 100 emails/dia  
**Pacote Essentials:** $19,95/mês (~R$ 100) - 50.000 emails/mês

**Vantagens:**
- Alta entregabilidade
- Analytics de emails
- Templates profissionais

**Código implementado em:**
- `src/MedicSoft.Api/Services/CRM/SendGridEmailService.cs`
- Package: `SendGrid 9.29.3`

**Custo Mensal Estimado:**
- **Hostinger SMTP**: R$ 0 (incluído)
- **SendGrid Free**: R$ 0 (100 emails/dia)
- **SendGrid Essentials**: R$ 100/mês

**Recomendação MVP:**  
Iniciar com **Hostinger SMTP** (já incluso). Migrar para **SendGrid** quando ultrapassar 500 emails/dia ou precisar de melhor entregabilidade.

---

### 5. SMS (Twilio)
**Status:** ⏳ Integrado mas não contratado  
**Prioridade:** 🟢 MÉDIA - Fase 2

**Descrição:**  
Envio de SMS para lembretes de consultas, notificações urgentes e 2FA via SMS.

**Fornecedor Configurado:**
- **Twilio**: Líder mundial em comunicação programável

**Configuração:**
```json
"Messaging": {
  "Sms": {
    "AccountSid": "",
    "AuthToken": "",
    "FromPhoneNumber": "",
    "Enabled": false
  }
}
```

**Código implementado em:**
- `src/MedicSoft.Api/Services/CRM/TwilioSmsService.cs`
- Package: `Twilio 7.8.0`

**Custos no Brasil:**
- **SMS Básico**: ~R$ 0,30 por SMS
- **SMS com short code**: ~R$ 0,15 por SMS (requer contrato)

**Custo Mensal Estimado:**
- 100 SMS/mês: R$ 30
- 500 SMS/mês: R$ 150
- 1.000 SMS/mês: R$ 300

**Recomendação MVP:**  
⏸️ **Não contratar inicialmente**. O sistema usa email para lembretes. Adicionar SMS apenas quando clientes solicitarem e pagarem por isso como add-on.

---

### 6. WhatsApp Business API (Meta/Facebook)
**Status:** ⏳ Integrado mas não contratado  
**Prioridade:** 🟢 MÉDIA - Fase 2-3

**Descrição:**  
Envio de mensagens via WhatsApp para lembretes e comunicação com pacientes.

**Fornecedor:**
- **Meta (Facebook) WhatsApp Business API**

**Configuração:**
```json
"Messaging": {
  "WhatsApp": {
    "ApiUrl": "https://graph.facebook.com/v18.0",
    "AccessToken": "",
    "PhoneNumberId": "",
    "Enabled": false
  }
}
```

**Código implementado em:**
- `src/MedicSoft.Api/Services/CRM/WhatsAppBusinessService.cs`

**Requisitos:**
- Conta WhatsApp Business verificada
- Aprovação da Meta
- Business Manager do Facebook

**Custos no Brasil:**
- **Conversas de Marketing**: R$ 0,50-1,00 por conversa
- **Conversas de Serviço**: R$ 0,30-0,60 por conversa
- **Primeiras 1000 conversas/mês**: GRATUITAS

**Custo Mensal Estimado:**
- 0-1000 conversas: R$ 0 (gratuito)
- 1000-5000 conversas: R$ 300-600
- 5000-10000 conversas: R$ 600-1200

**Recomendação MVP:**  
⏸️ **Não contratar inicialmente**. WhatsApp é muito popular no Brasil, mas requer aprovação complexa da Meta. Adicionar quando houver demanda clara dos clientes e base de usuários estabelecida.

---

## 💳 Serviços de Pagamento

### 7. Mercado Pago (Gateway de Pagamento)
**Status:** ⏳ Integrado mas não contratado  
**Prioridade:** 🟡 ALTA - Fase 1

**Descrição:**  
Gateway de pagamento para processar assinaturas SaaS, pagamentos de consultas e gestão financeira das clínicas.

**Fornecedor Configurado:**
- **Mercado Pago**: Maior processador de pagamentos da América Latina

**Configuração:**
```json
"PaymentGateway": {
  "Provider": "MercadoPago",
  "Enabled": true,
  "MercadoPago": {
    "AccessToken": "",
    "PublicKey": "",
    "WebhookSecret": "",
    "ApiUrl": "https://api.mercadopago.com",
    "EnableCreditCardPayments": true,
    "EnablePixPayments": true,
    "EnableBankSlipPayments": false
  }
}
```

**Código implementado em:**
- `src/MedicSoft.Application/Services/MercadoPagoPaymentGatewayService.cs`

**Métodos de Pagamento Suportados:**
- ✅ Cartão de Crédito (até 12x)
- ✅ PIX (instantâneo)
- ✅ Boleto Bancário
- ✅ Débito Online

**Taxas:**
- **PIX**: 0,99% por transação
- **Boleto**: R$ 3,49 por boleto
- **Cartão de Crédito**: 
  - À vista: 3,99% + R$ 0,40
  - Parcelado: 5,19% + R$ 0,40
- **Antecipação**: Disponível com taxas variáveis

**Custo Mensal Estimado:**
- Sem mensalidade fixa
- Custo por transação apenas
- Exemplo: R$ 10.000 em vendas via PIX = R$ 99 de taxas

**Recomendação MVP:**  
✅ **CONTRATAR NA FASE 1** - Essencial para monetização do SaaS. Mercado Pago é confiável, sem mensalidade e amplamente usado no Brasil.

**Alternativas Consideradas:**
- **Stripe**: Melhor para mercado internacional, mas complexo no Brasil
- **PagSeguro**: Similar ao Mercado Pago, mas com interface menos moderna
- **Asaas**: Focado em SaaS, mas menor market share

---

## 🏥 Serviços de Compliance e Regulatório

### 8. ANVISA SNGPC (Medicamentos Controlados)
**Status:** ✅ Integrado  
**Prioridade:** 🟡 ALTA - Fase 1 (para clínicas que prescrevem controlados)

**Descrição:**  
Sistema Nacional de Gerenciamento de Produtos Controlados da ANVISA. Obrigatório para clínicas que prescrevem medicamentos controlados.

**API:**
- **Base URL**: `https://sngpc.anvisa.gov.br/api`
- **Autenticação**: API Key (fornecida pela ANVISA após cadastro)

**Configuração:**
```json
"Anvisa": {
  "Sngpc": {
    "BaseUrl": "https://sngpc.anvisa.gov.br/api",
    "ApiKey": "",
    "EnableValidation": true
  }
}
```

**Código implementado em:**
- `src/MedicSoft.Application/Services/AnvisaSngpcClient.cs`
- `src/MedicSoft.Application/Services/SngpcTransmissionService.cs`
- `src/MedicSoft.Application/Services/SNGPCXmlGeneratorService.cs`

**Requisitos:**
- Cadastro na ANVISA
- Certificado Digital válido
- Responsável técnico com CRF ativo

**Custo:**
- **Cadastro ANVISA**: GRATUITO
- **Certificado Digital A1**: R$ 150-250/ano
- **Certificado Digital A3**: R$ 250-400/ano (cartão/token)

**Custo Mensal Estimado:**
- R$ 12-35/mês (certificado digital anualizado)

**Recomendação MVP:**  
⏸️ **Não contratar inicialmente**. Apenas necessário quando clínicas que prescrevem medicamentos controlados entrarem no sistema. Oferecer como add-on pago.

---

### 9. TISS (Padrão ANS)
**Status:** ✅ Implementado  
**Prioridade:** 🟡 ALTA - Fase 1 (para clínicas que atendem convênios)

**Descrição:**  
Padrão TISS (Troca de Informações na Saúde Suplementar) da ANS para comunicação com operadoras de saúde.

**API/Protocolo:**
- **WebService SOAP** das operadoras (ex: Unimed, Bradesco Saúde)
- Cada operadora tem seu próprio endpoint
- Validação contra XSD schemas da ANS

**Código implementado em:**
- `src/MedicSoft.Application/Services/TissXmlGeneratorService.cs`
- `src/MedicSoft.Application/Services/TissXmlValidatorService.cs`
- `src/MedicSoft.Application/Services/TissWebServiceClient.cs`
- `src/MedicSoft.Application/Services/TissBatchService.cs`

**Requisitos:**
- Registro ANS da clínica
- Contratos com operadoras de saúde
- Certificado Digital A1 ou A3

**Custo:**
- **Certificado Digital A1**: R$ 150-250/ano
- **Homologação com operadoras**: Geralmente gratuito
- **Suporte especializado** (opcional): R$ 200-500/mês

**Custo Mensal Estimado:**
- R$ 12-20/mês (certificado digital)
- R$ 0 (APIs das operadoras são gratuitas)

**Recomendação MVP:**  
⏸️ **Não contratar inicialmente**. O código está pronto. Ativar quando clínicas que atendem convênios entrarem. Pode ser add-on premium.

---

## 📊 Serviços de Marketing e CRM

### 10. Salesforce (CRM)
**Status:** ⏳ Integrado mas não contratado  
**Prioridade:** 🔵 BAIXA - Fase 3-4

**Descrição:**  
Integração com Salesforce para gerenciamento avançado de leads e pipeline de vendas.

**Configuração:**
```json
"Salesforce": {
  "Enabled": false,
  "InstanceUrl": "https://your-instance.salesforce.com",
  "ClientId": "",
  "ClientSecret": "",
  "ApiVersion": "v57.0"
}
```

**Código implementado em:**
- `src/MedicSoft.Api/Services/CRM/*`

**Planos Salesforce:**
- **Essentials**: $25/usuário/mês
- **Professional**: $75/usuário/mês
- **Enterprise**: $150/usuário/mês

**Custo Mensal Estimado:**
- 2 usuários (vendas): $150/mês (~R$ 750)
- 5 usuários: $375/mês (~R$ 1.875)

**Recomendação MVP:**  
⏸️ **NÃO CONTRATAR**. O sistema tem CRM interno robusto implementado. Salesforce só faz sentido quando a operação de vendas escalar significativamente (50+ clínicas ativas).

**Alternativas Mais Baratas:**
- **HubSpot Free**: Gratuito até 1M contatos
- **Pipedrive**: R$ 45-90/usuário/mês
- **RD Station CRM**: R$ 39/usuário/mês

---

### 11. Google Analytics 4 (Analytics)
**Status:** ⏳ Configurável (não requer contratação)  
**Prioridade:** 🟢 MÉDIA - Fase 1

**Descrição:**  
Analytics para site institucional e funil de conversão do SaaS.

**Configuração:**
- Código de tracking no frontend
- Eventos customizados de conversão
- Funil de onboarding

**Código:**
- `frontend/medicwarehouse-app/src/app/services/analytics/website-analytics.service.ts`

**Custo:**
- **Google Analytics 4**: GRATUITO
- **Google Analytics 360** (enterprise): $150.000/ano (não necessário)

**Custo Mensal Estimado:**
- R$ 0 (gratuito)

**Recomendação MVP:**  
✅ **ATIVAR IMEDIATAMENTE** - Essencial para entender comportamento dos usuários e otimizar conversão. É gratuito.

---

## 📈 Serviços de Monitoramento e Logs

### 12. Seq (Log Aggregation)
**Status:** ✅ Configurado (pode ser auto-hospedado)  
**Prioridade:** 🟢 MÉDIA - Fase 1

**Descrição:**  
Agregação e visualização centralizada de logs da aplicação para debugging e monitoramento.

**Configuração:**
```json
"Serilog": {
  "WriteTo": [
    {
      "Name": "Seq",
      "Args": {
        "serverUrl": "http://localhost:5341",
        "apiKey": ""
      }
    }
  ]
}
```

**Opções:**

#### 12.1 Seq Self-Hosted (Recomendado para MVP)
- **Licença Developer**: GRATUITA (até 1GB/dia)
- **Requisitos**: Rodar container Docker no mesmo VPS
- **Recursos**: 0.5GB RAM adicional

#### 12.2 Seq Cloud
- **Free tier**: GRATUITO (até 10GB/mês)
- **Standard**: $135/mês (50GB/mês)
- **Pro**: $450/mês (200GB/mês)

**Custo Mensal Estimado:**
- **Self-hosted**: R$ 0 (dentro do VPS)
- **Cloud Free**: R$ 0

**Recomendação MVP:**  
✅ **USAR SEQ SELF-HOSTED GRATUITO** - Rodar no mesmo VPS que a aplicação. Essencial para diagnosticar problemas em produção.

**Alternativas:**
- **ELK Stack** (Elasticsearch, Logstash, Kibana): Gratuito mas complexo
- **Grafana Loki**: Gratuito e leve
- **AWS CloudWatch**: Pago, integrado com AWS

---

### 13. Redis (Cache Distribuído)
**Status:** ⏳ Configurado (pode ser auto-hospedado)  
**Prioridade:** 🟡 ALTA - Fase 1

**Descrição:**  
Cache em memória para melhorar performance de consultas frequentes e reduzir carga no banco de dados.

**Configuração:**
```json
"CacheSettings": {
  "EnableDistributedCache": true,
  "CacheProvider": "Redis",
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "Omni Care:"
  }
}
```

**Opções:**

#### 13.1 Redis Self-Hosted (Recomendado)
- Container Docker no mesmo VPS
- **Memória recomendada**: 256MB-1GB
- **Licença**: Open Source (gratuito)

#### 13.2 Redis Cloud
- **Free tier**: 30MB (muito limitado)
- **Paid**: $5/mês (1GB) - $10/mês (5GB)

**Custo Mensal Estimado:**
- **Self-hosted**: R$ 0 (dentro do VPS)
- **Redis Cloud 1GB**: R$ 25/mês

**Recomendação MVP:**  
✅ **USAR REDIS SELF-HOSTED** - Rodar container no mesmo VPS. Melhora drasticamente a performance.

---

### 14. Hangfire (Background Jobs)
**Status:** ✅ Implementado  
**Prioridade:** ✅ Já incluído (library .NET)

**Descrição:**  
Processamento de jobs em background (envio de emails em massa, relatórios, notificações agendadas).

**Package:**
- `Hangfire.AspNetCore 1.8.14`
- `Hangfire.PostgreSql 1.20.9`

**Configuração:**
- Usa o mesmo PostgreSQL como storage
- Dashboard em `/hangfire`
- Jobs recorrentes configurados

**Custo:**
- **GRATUITO** (biblioteca open source)

**Recomendação MVP:**  
✅ **JÁ ESTÁ ATIVO** - Não requer contratação. Essencial para processamento assíncrono.

---

## 🔮 Serviços Opcionais (Futuro)

### 15. AWS S3 / Azure Blob Storage (Armazenamento de Arquivos)
**Status:** 🔜 Planejado  
**Prioridade:** 🔵 BAIXA - Fase 3+

**Descrição:**  
Armazenamento escalável de arquivos (imagens, PDFs, documentos médicos, backup de prontuários).

**Uso Atual:**
- Arquivos salvos no filesystem do servidor (funciona para MVP)

**Quando Contratar:**
- Quando atingir 50+ clínicas
- Quando precisar de CDN para performance
- Para backups redundantes geográficos

**Custo Estimado:**
- **AWS S3**: $0,023/GB (~R$ 0,12/GB)
- 100GB: ~R$ 12/mês
- 500GB: ~R$ 60/mês

**Recomendação:**  
⏸️ **NÃO CONTRATAR NO MVP** - Filesystem local é suficiente. Migrar para cloud storage quando escalar.

---

### 16. Twilio Video / Agora.io (Telemedicina)
**Status:** 🔜 Planejado  
**Prioridade:** 🔵 BAIXA - Fase 4+

**Descrição:**  
Videochamadas para telemedicina (já temos microserviço de telemedicina implementado, mas sem vídeo real).

**Fornecedores:**
- **Twilio Video**: Focado em healthcare
- **Agora.io**: Melhor performance, menor latência
- **Jitsi**: Open source (gratuito mas menos confiável)

**Custos (Twilio):**
- **Pay-as-you-go**: $0,0015/minuto/participante
- 1000 minutos/mês: ~$1,50/mês (2 participantes)
- 10.000 minutos/mês: ~$15/mês

**Custo Mensal Estimado:**
- R$ 50-500/mês dependendo do uso

**Recomendação:**  
⏸️ **NÃO CONTRATAR NO MVP** - Telemedicina ainda é nicho no Brasil. Adicionar quando houver demanda real.

---

### 17. Elastic APM (Performance Monitoring)
**Status:** 🔜 Planejado  
**Prioridade:** 🔵 BAIXA - Fase 3+

**Descrição:**  
Monitoramento avançado de performance (APM - Application Performance Monitoring).

**Alternativas:**
- **Elastic APM**: Open source
- **New Relic**: $99-349/mês
- **Datadog**: $15/host/mês
- **Application Insights** (Azure): Pay-as-you-go

**Recomendação:**  
⏸️ **NÃO CONTRATAR NO MVP** - Logs via Seq são suficientes inicialmente. Adicionar APM quando tiver problemas de performance complexos.

---

### 18. Mailchimp / SendinBlue (Email Marketing)
**Status:** 🔜 Planejado  
**Prioridade:** 🔵 BAIXA - Fase 2-3

**Descrição:**  
Plataforma de email marketing para newsletters, campanhas promocionais e nurturing de leads.

**Uso Atual:**
- Sistema envia emails transacionais
- Não tem automação de marketing ainda

**Fornecedores:**
- **SendinBlue** (Brevo): Gratuito até 300 emails/dia
- **Mailchimp**: Gratuito até 500 contatos
- **RD Station**: R$ 59/mês (até 5.000 contatos)

**Custo Mensal Estimado:**
- R$ 0-200/mês

**Recomendação:**  
⏸️ **NÃO CONTRATAR NO MVP** - Foco em produto primeiro. Marketing avançado vem depois.

---

## 📅 Cronograma de Contratação

### Fase 0 - Pré-Lançamento (Antes do MVP)
**Prazo:** Imediato  
**Investimento:** R$ 92-152/mês

| Serviço | Prioridade | Status | Custo Mensal |
|---------|-----------|--------|--------------|
| VPS Hostinger 4GB | 🔴 Crítica | ⏳ Contratar | R$ 89 |
| Domínio .com.br | 🔴 Crítica | ⏳ Contratar | R$ 3,33 |
| SSL Let's Encrypt | 🔴 Crítica | ⏳ Configurar | R$ 0 |
| PostgreSQL | 🔴 Crítica | ✅ Incluído | R$ 0 |
| Redis Self-hosted | 🔴 Crítica | ⏳ Configurar | R$ 0 |
| Seq Self-hosted | 🔴 Crítica | ⏳ Configurar | R$ 0 |

**Total Fase 0:** R$ 92,33/mês

---

### Fase 1 - Lançamento MVP (Primeiros 3 meses)
**Prazo:** Mês 1-3  
**Investimento adicional:** R$ 0-100/mês

| Serviço | Prioridade | Status | Custo Mensal |
|---------|-----------|--------|--------------|
| Email Hostinger SMTP | 🟡 Alta | ✅ Incluído | R$ 0 |
| Google Analytics 4 | 🟡 Alta | ⏳ Configurar | R$ 0 |
| Mercado Pago | 🟡 Alta | ⏳ Contratar | ~1% de vendas |

**Total Fase 1:** R$ 92/mês + taxas de transação

**Justificativa:**  
Foco em colocar o produto no ar com custo mínimo. Mercado Pago é pay-per-use, então só paga quando vender.

---

### Fase 2 - Crescimento Inicial (Meses 4-6)
**Prazo:** Após 5-10 clínicas pagas  
**Investimento adicional:** R$ 150-300/mês

| Serviço | Prioridade | Quando Contratar |
|---------|-----------|------------------|
| SendGrid Essentials | 🟡 Alta | Quando ultrapassar 500 emails/dia |
| Twilio SMS | 🟢 Média | Quando clientes solicitarem |
| WhatsApp Business | 🟢 Média | Quando houver demanda clara |
| Upgrade VPS para 8GB | 🟡 Alta | Com 10-15 clínicas simultâneas |

**Total Fase 2:** R$ 250-450/mês (dependendo do uso)

---

### Fase 3 - Expansão (Meses 7-12)
**Prazo:** Após 20+ clínicas pagas  
**Investimento adicional:** R$ 300-600/mês

| Serviço | Prioridade | Quando Contratar |
|---------|-----------|------------------|
| AWS S3 | 🔵 Baixa | Para redundância e backups |
| Certificado Digital SNGPC | 🟡 Alta | Quando clínicas precisarem |
| Email Marketing | 🔵 Baixa | Para growth e retenção |
| CDN (Cloudflare) | 🟢 Média | Para melhor performance |

**Total Fase 3:** R$ 550-1050/mês

---

### Fase 4 - Maturidade (Mês 12+)
**Prazo:** Após 50+ clínicas pagas  
**Investimento:** Depende de escala

| Serviço | Prioridade | Quando Contratar |
|---------|-----------|------------------|
| APM (New Relic/Datadog) | 🔵 Baixa | Para otimização avançada |
| Twilio Video | 🔵 Baixa | Se houver demanda de telemedicina |
| Salesforce | 🔵 Baixa | Se vendas escalarem muito |
| Infraestrutura escalável | 🟡 Alta | Migrar para Kubernetes |

---

## 💰 Estimativa de Custos

### Resumo por Fase

| Fase | Período | Clínicas | Receita Est. | Custo Serviços | % da Receita |
|------|---------|----------|--------------|----------------|--------------|
| **Fase 0** | Pré-lançamento | 0 | R$ 0 | R$ 92 | N/A |
| **Fase 1** | Meses 1-3 | 1-5 | R$ 500-2.500 | R$ 92-120 | 4-18% |
| **Fase 2** | Meses 4-6 | 5-15 | R$ 2.500-7.500 | R$ 250-450 | 3-18% |
| **Fase 3** | Meses 7-12 | 15-30 | R$ 7.500-15.000 | R$ 550-1.050 | 4-14% |
| **Fase 4** | Mês 12+ | 30-100 | R$ 15.000-50.000 | R$ 1.000-3.000 | 2-20% |

*Assumindo ticket médio de R$ 500/clínica/mês*

---

### Breakdown de Custos Mensais

#### MVP (Primeiros 6 meses)
```
VPS Hostinger 4GB            R$ 89,00
Domínio .com.br              R$ 3,33
PostgreSQL                   R$ 0,00 (incluído)
Redis                        R$ 0,00 (self-hosted)
Seq                          R$ 0,00 (self-hosted)
Email (Hostinger SMTP)       R$ 0,00 (incluído)
Google Analytics             R$ 0,00 (gratuito)
Mercado Pago                 ~1% vendas
                             ─────────
TOTAL FIXO:                  R$ 92,33/mês
TOTAL VARIÁVEL:              1% das vendas
```

#### Crescimento (Meses 7-12)
```
VPS Upgrade 8GB              R$ 169,00
Domínio                      R$ 3,33
SendGrid                     R$ 100,00 (se necessário)
SMS Twilio                   R$ 0-200 (se ativado)
WhatsApp                     R$ 0-300 (se ativado)
AWS S3                       R$ 12-60 (se necessário)
                             ─────────
TOTAL ESTIMADO:              R$ 284-832/mês
```

#### Escala (12+ meses, 50+ clínicas)
```
VPS ou Cloud (escalado)     R$ 500-1.500
Serviços de Comunicação      R$ 300-800
Storage Cloud                R$ 100-300
Monitoramento APM            R$ 500-1.500 (opcional)
Outros serviços              R$ 200-500
                             ─────────
TOTAL ESTIMADO:              R$ 1.600-4.600/mês
```

---

## 🎯 Recomendações Estratégicas

### Para o MVP (Fase 0-1)

✅ **CONTRATAR IMEDIATAMENTE:**
1. **VPS Hostinger 4GB** (R$ 89/mês) - Essencial para hospedar tudo
2. **Domínio .com.br** (R$ 3,33/mês) - Identidade profissional
3. **Mercado Pago** (sem mensalidade) - Monetização

✅ **CONFIGURAR (GRATUITO):**
1. **PostgreSQL** (self-hosted) - Banco de dados
2. **Redis** (self-hosted) - Cache para performance
3. **Seq** (self-hosted) - Logs e debugging
4. **SSL Let's Encrypt** - Segurança HTTPS
5. **Google Analytics** - Métricas de uso

**Investimento Total MVP: R$ 92/mês + 1% das vendas**

---

### Próximos Passos por Prioridade

#### URGENTE (Antes do lançamento):
1. ☑️ Contratar VPS Hostinger 4GB
2. ☑️ Registrar domínio .com.br
3. ☑️ Configurar SSL gratuito
4. ☑️ Criar conta Mercado Pago
5. ☑️ Configurar Google Analytics

#### IMPORTANTE (Primeiros 3 meses):
1. ☐ Monitorar uso de email (migrar para SendGrid se necessário)
2. ☐ Coletar feedback sobre necessidade de SMS/WhatsApp
3. ☐ Monitorar performance do VPS (upgrade se necessário)
4. ☐ Implementar backups automatizados

#### FUTURO (Após tração):
1. ☐ Avaliar necessidade de SMS/WhatsApp
2. ☐ Considerar CDN para performance global
3. ☐ Avaliar necessidade de armazenamento cloud (S3)
4. ☐ Planejar escalabilidade de infraestrutura

---

## 📊 Análise de ROI

### Cenário Base (50 clínicas em 12 meses)

**Receita:**
- 50 clínicas × R$ 500/mês = R$ 25.000/mês
- Receita anual = R$ 300.000

**Custos de Serviços Externos:**
- Ano 1 (média): ~R$ 300/mês = R$ 3.600/ano
- % da Receita: 1,2%

**Margem:**
- Custo de infraestrutura é **extremamente baixo**
- Outros custos (pessoal, suporte, desenvolvimento) são muito maiores
- Serviços externos **NÃO** são o gargalo financeiro

---

## 🔐 Considerações de Segurança

### Serviços que Requerem Certificado Digital:
1. **ANVISA SNGPC** - Certificado A1 ou A3
2. **TISS (Operadoras)** - Certificado A1 ou A3
3. **NFSe** - Certificado A1 ou A3

**Custo Certificado Digital:**
- A1 (software): R$ 150-250/ano
- A3 (token/cartão): R$ 250-400/ano

**Recomendação:**  
Comprar 1 certificado A1 quando a primeira clínica precisar de SNGPC ou faturamento de convênios.

---

## 📞 Contatos e Suporte

### Para Contratação:

**Hostinger:**
- Site: https://www.hostinger.com.br
- Suporte: 24/7 via chat

**Mercado Pago:**
- Site: https://www.mercadopago.com.br/developers
- Docs: https://www.mercadopago.com.br/developers/pt/docs

**Twilio (futuro):**
- Site: https://www.twilio.com
- Representante Brasil: twilio.com/pt-br

**Meta WhatsApp Business (futuro):**
- Site: https://business.whatsapp.com
- Docs: https://developers.facebook.com/docs/whatsapp

---

## 📝 Notas Finais

### Filosofia de Contratação:
> **"Comece pequeno, escale quando necessário"**

1. **MVP com Custos Mínimos**: R$ 92/mês é extremamente viável
2. **Pay-as-you-grow**: Mercado Pago, SMS, WhatsApp só cobram pelo uso
3. **Self-hosting Inteligente**: PostgreSQL, Redis, Seq no mesmo VPS = R$ 0 extra
4. **Migração Gradual**: Mover para serviços pagos quando houver ROI claro

### Mantenha Documentado:
- ✅ Senhas e credenciais em gerenciador seguro (1Password, Bitwarden)
- ✅ Documentar todas as integrações configuradas
- ✅ Manter backup de configurações
- ✅ Revisar custos mensalmente

### Revisão Recomendada:
- **Mensal**: Revisar custos vs. receita
- **Trimestral**: Avaliar necessidade de novos serviços
- **Semestral**: Analisar oportunidades de otimização

---

## ✅ Checklist de Implementação

### Pré-MVP:
- [ ] Contratar VPS Hostinger 4GB (R$ 89/mês)
- [ ] Registrar domínio .com.br (R$ 40/ano)
- [ ] Configurar DNS apontando para VPS
- [ ] Instalar Docker/Podman no VPS
- [ ] Deploy do PostgreSQL via Docker
- [ ] Deploy do Redis via Docker
- [ ] Deploy do Seq via Docker
- [ ] Deploy da API .NET
- [ ] Deploy dos frontends Angular
- [ ] Configurar SSL Let's Encrypt
- [ ] Criar conta Mercado Pago
- [ ] Integrar Mercado Pago no sistema
- [ ] Configurar Google Analytics no site
- [ ] Testar envio de emails via Hostinger SMTP
- [ ] Configurar backups automatizados

### Pós-MVP:
- [ ] Monitorar uso de recursos (CPU, RAM, Disco)
- [ ] Coletar feedback de clientes sobre comunicação
- [ ] Avaliar necessidade de SMS/WhatsApp
- [ ] Considerar upgrade de VPS (se necessário)
- [ ] Implementar monitoramento de uptime
- [ ] Configurar alertas de erro

---

**Documento criado em:** Fevereiro 2026  
**Última atualização:** Fevereiro 2026  
**Versão:** 1.0  
**Autor:** Análise Técnica do Sistema Omni Care Software  
**Status:** ✅ Completo e pronto para uso

---

## 📚 Documentação Relacionada

- [Plano de Lançamento MVP](PLANO_LANCAMENTO_MVP_SAAS.md)
- [Plano Financeiro Mensal](PLANO_FINANCEIRO_MENSAL.md)
- [Guia de Implementação](MVP_IMPLEMENTATION_GUIDE.md)
- [Documentação de Deploy](DEPLOY_HOSTINGER_RESUMO_EXECUTIVO.md)
- [Guia de Infraestrutura](GUIA_IMPLEMENTACAO_INFRAESTRUTURA.md)
