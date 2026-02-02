# Guia de Implementação da Infraestrutura - Kinghost + Twilio

## Data de Criação
02 de Fevereiro de 2026

## Objetivo
Este guia fornece instruções práticas para implementar e configurar a infraestrutura descrita no PLANO_FINANCEIRO_MENSAL.md, incluindo Kinghost KVM 2, Twilio e preparação para Certificado Digital ICP-Brasil.

---

## 1. Configuração Kinghost KVM 2

### 1.1 Informações do Plano Contratado

**Plano**: Kinghost VPS KVM 2  
**Custo**: R$ 149,90/mês  
**Link**: https://king.host/hospedagem-vps

**Especificações**:
- 2 vCPUs dedicados
- 4 GB RAM
- 60 GB SSD NVMe
- Tráfego ilimitado
- IP dedicado
- Painel de controle (cPanel ou similar)
- Backup diário automático incluído
- SSL Let's Encrypt gratuito

### 1.2 Configuração Inicial

#### Passo 1: Acesso ao Servidor
```bash
# Acessar via SSH
ssh root@seu-ip-kinghost

# Atualizar sistema
apt update && apt upgrade -y

# Instalar pacotes essenciais
apt install -y git curl wget vim nginx postgresql-14
```

#### Passo 2: Configurar PostgreSQL
```bash
# Configurar PostgreSQL para aceitar conexões locais
sudo -u postgres psql

# Dentro do psql:
CREATE DATABASE medicsoft_production;
CREATE USER medicsoft WITH PASSWORD 'sua_senha_segura';
GRANT ALL PRIVILEGES ON DATABASE medicsoft_production TO medicsoft;
\q

# Configurar limites de conexão
sudo nano /etc/postgresql/14/main/postgresql.conf
# Ajustar: max_connections = 100
# Ajustar: shared_buffers = 1GB

sudo systemctl restart postgresql
```

#### Passo 3: Instalar .NET Runtime
```bash
# Instalar .NET 8.0
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Adicionar ao PATH
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc
source ~/.bashrc
```

#### Passo 4: Configurar Nginx como Reverse Proxy
```bash
# Criar configuração do site
sudo nano /etc/nginx/sites-available/medicsoft

# Adicionar configuração inicial (HTTP - será atualizado para HTTPS pelo Certbot):
server {
    listen 80;
    server_name seu-dominio.com.br;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Ativar site
sudo ln -s /etc/nginx/sites-available/medicsoft /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### Passo 5: Configurar SSL com Let's Encrypt
```bash
# Instalar Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obter certificado SSL
sudo certbot --nginx -d seu-dominio.com.br

# Renovação automática já está configurada
```

### 1.3 Deploy da Aplicação

```bash
# Criar diretório da aplicação
mkdir -p /var/www/medicsoft
cd /var/www/medicsoft

# Clonar repositório (ou fazer upload via FTP)
git clone https://github.com/PrimeCareSoftware/MW.Code.git .

# Build da aplicação
dotnet publish -c Release -o ./publish

# Configurar como serviço systemd
sudo nano /etc/systemd/system/medicsoft.service
```

Conteúdo do arquivo service:
```ini
[Unit]
Description=MedicSoft Web Application
After=network.target

[Service]
WorkingDirectory=/var/www/medicsoft/publish
ExecStart=/root/.dotnet/dotnet /var/www/medicsoft/publish/MedicWarehouse.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=medicsoft
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

```bash
# Iniciar serviço
sudo systemctl enable medicsoft
sudo systemctl start medicsoft
sudo systemctl status medicsoft
```

### 1.4 Monitoramento de Recursos

#### Script de Monitoramento
```bash
# Criar script de monitoramento
nano /usr/local/bin/monitor-resources.sh
```

Conteúdo:
```bash
#!/bin/bash
# Monitor de recursos Kinghost KVM 2

echo "=== Monitor de Recursos - $(date) ===="
echo ""

# CPU
echo "CPU Usage:"
top -bn1 | grep "Cpu(s)" | sed "s/.*, *\([0-9.]*\)%* id.*/\1/" | awk '{print 100 - $1"%"}'

# RAM
echo ""
echo "Memory Usage:"
free -h | grep Mem | awk '{print "Used: " $3 " / Total: " $2 " (" $3/$2*100 "%)"}'

# Disco
echo ""
echo "Disk Usage:"
df -h / | grep / | awk '{print "Used: " $3 " / Total: " $2 " (" $5 ")"}'

# PostgreSQL
echo ""
echo "PostgreSQL Connections:"
sudo -u postgres psql -c "SELECT count(*) as active_connections FROM pg_stat_activity WHERE state = 'active';"

# Nginx
echo ""
echo "Nginx Status:"
systemctl status nginx | grep Active
```

```bash
chmod +x /usr/local/bin/monitor-resources.sh

# Executar manualmente
/usr/local/bin/monitor-resources.sh

# Ou configurar cron para executar a cada hora
crontab -e
# Adicionar: 0 * * * * /usr/local/bin/monitor-resources.sh >> /var/log/resource-monitor.log
```

### 1.5 Quando Fazer Upgrade

**Indicadores para upgrade KVM 2 → KVM 4**:
- ✅ CPU usage constante > 80%
- ✅ RAM usage constante > 90%
- ✅ Mais de 150 clientes ativos
- ✅ Response time da API > 2 segundos
- ✅ PostgreSQL atingindo max_connections

**Processo de Upgrade**:
1. Abrir ticket com suporte Kinghost
2. Solicitar upgrade para KVM 4 (R$ 299,90/mês)
3. Upgrade é feito sem downtime (live migration)
4. Verificar recursos após upgrade

---

## 2. Configuração Twilio

### 2.1 Criar Conta Twilio

1. Acessar: https://www.twilio.com/try-twilio
2. Criar conta gratuita (trial)
3. Verificar telefone e email
4. Upgrade para conta pay-as-you-go (sem commitment)

### 2.2 Obter Credenciais API

```bash
# No Console Twilio:
# 1. Account SID: Encontrado no dashboard
# 2. Auth Token: Clicar em "Show" no dashboard
# 3. API Key: Settings > API Keys > Create API Key
```

### 2.3 Configurar no Projeto

**Atualizar appsettings.Production.json**:
```json
{
  "Twilio": {
    "AccountSid": "seu_account_sid",
    "AuthToken": "seu_auth_token",
    "ApiKeySid": "sua_api_key",
    "ApiKeySecret": "sua_api_key_secret"
  },
  "VideoCall": {
    "Provider": "Twilio",
    "TwilioRoomType": "peer-to-peer"
  }
}
```

**Implementação no código** (já existe em `telemedicine/`):
```csharp
// Configuração já implementada em:
// telemedicine/src/MedicSoft.Telemedicine.Infrastructure/ExternalServices/TwilioVideoService.cs

// Uso:
var videoService = serviceProvider.GetRequiredService<IVideoCallService>();
var room = await videoService.CreateRoomAsync("consulta-123", expirationHours: 2);
```

### 2.4 Monitorar Custos Twilio

#### Script de Monitoramento de Custos
```bash
# Instalar Twilio CLI
npm install -g twilio-cli

# Login
twilio login

# Verificar uso do mês atual
twilio api:usage:records:list --category=video-group-minutes

# Criar alerta de custo
# No console Twilio: Monitor > Alerts > Create Alert
# Configurar: Alert when usage exceeds R$ 100/month
```

#### Dashboard de Custos
```python
#!/usr/bin/env python3
# Script Python para dashboard de custos Twilio
# Salvar como: /usr/local/bin/twilio-cost-monitor.py
# Instalar dependências: pip install twilio

from twilio.rest import Client
import os
from datetime import datetime

# Configurações (ajustar conforme necessário)
USD_TO_BRL_RATE = 5.0  # Taxa de conversão USD para BRL
TWILIO_COST_PER_MINUTE = 0.004  # Custo por minuto em USD

account_sid = os.environ['TWILIO_ACCOUNT_SID']
auth_token = os.environ['TWILIO_AUTH_TOKEN']
client = Client(account_sid, auth_token)

# Buscar uso do mês
records = client.usage.records.list(
    category='video-group-minutes',
    start_date=datetime.now().replace(day=1),
    end_date=datetime.now()
)

total_minutes = sum([float(r.usage) for r in records])
cost_usd = total_minutes * TWILIO_COST_PER_MINUTE
cost_brl = cost_usd * USD_TO_BRL_RATE

print(f"Total de minutos usados: {total_minutes:.2f}")
print(f"Custo estimado: USD ${cost_usd:.2f} / BRL R$ {cost_brl:.2f}")

# Alerta se custo > R$ 200
if cost_brl > 200:
    print("⚠️ ALERTA: Custo Twilio acima do esperado!")
    # Enviar email de alerta
```

### 2.5 Alternativas ao Twilio

#### Opção 1: Daily.co (Fixo, mais previsível)
```bash
# Custo: US$ 99/mês para até 1000 minutos
# Vantagem: Previsibilidade de custo
# Implementação similar ao Twilio

# Configuração:
{
  "DailyCoVideo": {
    "ApiKey": "sua-api-key-daily-co",
    "BaseUrl": "https://api.daily.co/v1"
  }
}
```

#### Opção 2: Jitsi Meet (Self-hosted, zero custo variável)
```bash
# Instalar Jitsi Meet no próprio servidor
# Custo: Apenas infraestrutura (já coberta pelo KVM 2)
# Vantagem: Custo zero de licença
# Desvantagem: Consome mais recursos do servidor

# Instalação rápida:
wget -qO - https://download.jitsi.org/jitsi-key.gpg.key | sudo apt-key add -
echo 'deb https://download.jitsi.org stable/' | sudo tee /etc/apt/sources.list.d/jitsi-stable.list
sudo apt update
sudo apt install -y jitsi-meet
```

---

## 3. Preparação para Certificado Digital ICP-Brasil

### 3.1 Quando Implementar

**Critérios para implementação**:
- ✅ 5-10 clientes solicitaram ativamente
- ✅ Receita mensal > R$ 10.000
- ✅ Mês 6+ de operação
- ✅ Base de clientes médicos estabelecida

### 3.2 Escolha do Certificado

**Opções recomendadas**:

| Certificado | Uso | Custo Anual | Quando Usar |
|-------------|-----|-------------|-------------|
| e-CPF A1 | Pessoa física, software | R$ 150 | Profissionais autônomos |
| e-CPF A3 | Pessoa física, token/cartão | R$ 200-500 | Médicos independentes |
| e-CNPJ A1 | Pessoa jurídica, software | R$ 250 | Clínicas pequenas |
| **e-CNPJ A3** | **Pessoa jurídica, token** | **R$ 500** | **Recomendado para plataforma** |

**Recomendação**: e-CNPJ A3 (3 anos) para a plataforma central

### 3.3 Certificadoras Homologadas

**Certificadoras ICP-Brasil recomendadas**:
1. **Serasa Experian** - https://certificadodigital.serasaexperian.com.br
2. **Certisign** - https://www.certisign.com.br
3. **Valid** - https://www.validcertificadora.com.br
4. **SafeWeb** - https://www.safeweb.com.br

**Processo de compra**:
1. Escolher certificadora
2. Selecionar tipo (e-CNPJ A3 recomendado)
3. Validação presencial obrigatória
4. Receber token/cartão
5. Instalar certificado

### 3.4 Integração Técnica

**Módulo já implementado**:
```bash
# Módulo de assinatura digital já existe em:
src/MedicSoft.Domain/Entities/DigitalSignature.cs
src/MedicSoft.Application/Services/DigitalSignatureService.cs
```

**Configuração**:
```json
{
  "DigitalSignature": {
    "Enabled": true,
    "Provider": "ICP-Brasil",
    "CertificatePath": "/var/certificates/",
    "RequireTimestamp": true,
    "TimestampServer": "http://timestamp.iti.gov.br"
  }
}
```

**Bibliotecas necessárias**:
```bash
# Instalar pacotes .NET
dotnet add package Lacuna.Pki
dotnet add package Lacuna.RestPki.Client
```

**Exemplo de uso**:
```csharp
// Assinar documento PDF
var signer = new DigitalSignatureService(certificatePath);
var signedPdf = await signer.SignPdfAsync(
    originalPdfPath,
    certificate,
    "Dr. João Silva - CRM 12345"
);
```

### 3.5 Add-on Premium de Certificado Digital

**Modelo de negócio**:
- Cobrar R$ 39/mês do cliente
- Custo do certificado: R$ 13,89/mês (amortizado)
- Margem: R$ 25,11/mês (64%)

**Incluir no add-on**:
- ✅ Certificado e-CNPJ A3 (renovação coberta)
- ✅ Assinatura ilimitada de documentos
- ✅ Armazenamento seguro
- ✅ Conformidade CFM/prescrição digital
- ✅ Suporte prioritário

---

## 4. Checklist de Implementação

### Fase 1: Setup Inicial (Semana 1)
- [ ] Contratar Kinghost KVM 2
- [ ] Configurar servidor (SSH, firewall, updates)
- [ ] Instalar PostgreSQL, Nginx, .NET
- [ ] Configurar SSL com Let's Encrypt
- [ ] Deploy da aplicação
- [ ] Configurar backup automático
- [ ] Documentar credenciais (armazenar em 1Password/Bitwarden)

### Fase 2: Telemedicina (Semana 2)
- [ ] Criar conta Twilio (trial inicialmente)
- [ ] Obter API credentials
- [ ] Configurar no appsettings.json
- [ ] Testar criação de sala de vídeo
- [ ] Configurar alertas de custo
- [ ] Documentar processo de upgrade para Daily.co se necessário

### Fase 3: Monitoramento (Semana 3)
- [ ] Implementar script de monitoramento de recursos
- [ ] Configurar alertas de CPU/RAM/Disco
- [ ] Implementar dashboard de custos Twilio
- [ ] Configurar logs centralizados
- [ ] Testar processo de backup/restore
- [ ] Documentar runbooks operacionais

### Fase 4: ICP-Brasil (Mês 6+)
- [ ] Avaliar demanda de clientes
- [ ] Escolher certificadora
- [ ] Comprar certificado e-CNPJ A3
- [ ] Instalar e testar módulo de assinatura
- [ ] Criar add-on premium (R$ 39/mês)
- [ ] Atualizar planos e sistema de billing
- [ ] Treinar equipe de suporte

---

## 5. Custos Resumidos

| Item | Setup | Mensal | Anual |
|------|-------|--------|-------|
| Kinghost KVM 2 | R$ 149,90 | R$ 149,90 | R$ 1.798,80 |
| Twilio (estimado fase inicial) | R$ 0 | R$ 27,00 | R$ 324,00 |
| Domínio .com.br | R$ 40,00 | R$ 3,33 | R$ 40,00 |
| SSL Let's Encrypt | R$ 0 | R$ 0 | R$ 0 |
| Certificado ICP (opcional) | R$ 500 | R$ 13,89 | R$ 166,67 |
| **Total Inicial** | **R$ 189,90** | **R$ 180,23** | **R$ 2.162,80** |
| **Total com ICP** | **R$ 689,90** | **R$ 194,12** | **R$ 2.329,47** |

**Observação**: Valores Twilio podem variar de R$ 27 a R$ 270/mês conforme uso real.

---

## 6. Contatos e Suporte

### Kinghost
- **Site**: https://king.host
- **Suporte**: suporte@kinghost.com.br
- **WhatsApp**: (51) 3062-8000
- **Painel**: https://painel.kinghost.net

### Twilio
- **Site**: https://www.twilio.com
- **Suporte**: https://support.twilio.com
- **Console**: https://console.twilio.com
- **Docs**: https://www.twilio.com/docs/video

### ICP-Brasil
- **Portal**: https://www.gov.br/iti/pt-br/assuntos/icp-brasil
- **Telefone**: 0800 970 2050
- **Certificadoras**: Ver lista em https://www.gov.br/iti/pt-br/assuntos/certificadoras

---

## 7. Próximos Passos

1. ✅ Seguir checklist de implementação
2. ✅ Testar cada componente individualmente
3. ✅ Realizar testes de carga
4. ✅ Documentar procedimentos operacionais
5. ✅ Treinar equipe
6. ✅ Lançar em produção
7. ✅ Monitorar métricas diariamente na primeira semana

---

**Documento criado em**: 02/02/2026  
**Próxima revisão**: Após implementação (registrar datas e ajustes)

