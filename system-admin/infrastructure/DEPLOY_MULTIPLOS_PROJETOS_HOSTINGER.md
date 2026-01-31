# 🚀 Deploy de Múltiplos Projetos na Hostinger

## 📋 Visão Geral

Este documento serve como referência rápida para fazer deploy de **múltiplas APIs e aplicações Angular** na Hostinger.

**Para o guia completo e detalhado**, veja: [DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md](../../DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md) na raiz do repositório.

---

## 🎯 O que este guia cobre?

### Aplicações
- ✅ **MedicSoft.Api** - API Principal (.NET 8)
- ✅ **PatientPortal.Api** - API Portal do Paciente (.NET 8)
- ✅ **medicwarehouse-app** - Frontend Principal (Angular)
- ✅ **mw-system-admin** - Sistema de Administração (Angular)
- ✅ **patient-portal** - Portal do Paciente (Angular)
- ✅ **mw-docs** - Documentação (Angular)

### Infraestrutura
- ✅ **PostgreSQL 16** - Banco de dados principal
- ✅ **Nginx** - Servidor web e proxy reverso
- ✅ **Let's Encrypt** - Certificados SSL gratuitos
- ✅ **Systemd** - Gerenciamento de serviços

---

## 💰 Planos Recomendados da Hostinger

| Necessidade | Plano | RAM | CPU | Preço/mês* |
|-------------|-------|-----|-----|------------|
| **Desenvolvimento/Testes** | KVM 1 | 4GB | 2 vCPU | R$ 19,99 |
| **Produção (RECOMENDADO)** | KVM 2 | 8GB | 4 vCPU | R$ 39,99 |
| **Alta Disponibilidade** | KVM 3 | 12GB | 6 vCPU | R$ 59,99 |

*Preços aproximados para contrato anual

### Por que KVM 2 é o Recomendado?

Para rodar **2 APIs .NET + PostgreSQL + 4 frontends Angular**, o KVM 2 oferece:
- ✅ RAM suficiente (8GB) para todas as aplicações
- ✅ CPU adequada (4 vCPU) para processamento
- ✅ Margem de segurança para picos de acesso
- ✅ Melhor custo-benefício (R$ 39,99/mês)
- ✅ Suporta 10-30 usuários simultâneos

---

## 🏗️ Arquitetura da Solução

```
┌─────────────────────────────────────────────────┐
│              Internet (HTTPS)                    │
└────────────────┬────────────────────────────────┘
                 │
        ┌────────▼────────┐
        │  Nginx (80/443) │  ← Proxy Reverso + SSL
        └────────┬────────┘
                 │
    ┏━━━━━━━━━━━┻━━━━━━━━━━━┓
    ┃                        ┃
┌───▼───┐               ┌───▼───┐
│ APIs  │               │Frontend│
│ .NET  │               │Angular │
├───────┤               ├────────┤
│:5000  │               │Static  │
│:5001  │               │Files   │
└───┬───┘               └────────┘
    │
    │ ┌────────────────┐
    └─►   PostgreSQL   │
      │     :5432      │
      └────────────────┘
```

### Estrutura de Domínios

```
meuprimecare.com.br               → medicwarehouse-app
api.meuprimecare.com.br           → MedicSoft.Api
admin.meuprimecare.com.br         → mw-system-admin
paciente.meuprimecare.com.br      → patient-portal
api-paciente.meuprimecare.com.br  → PatientPortal.Api
docs.meuprimecare.com.br          → mw-docs
```

---

## 📦 Componentes Necessários

### Software a Instalar

1. **Sistema Operacional**: Ubuntu 22.04 LTS
2. **.NET 8 SDK**: Para rodar as APIs
3. **PostgreSQL 16**: Banco de dados
4. **Node.js 18**: Para build do Angular
5. **Nginx**: Servidor web
6. **Certbot**: Para SSL gratuito

### Portas Utilizadas

| Serviço | Porta | Exposição |
|---------|-------|-----------|
| SSH | 22 | Externa |
| HTTP | 80 | Externa (redirect HTTPS) |
| HTTPS | 443 | Externa |
| MedicSoft.Api | 5000 | Interna (via Nginx) |
| PatientPortal.Api | 5001 | Interna (via Nginx) |
| PostgreSQL | 5432 | Interna (localhost) |

---

## 🚀 Processo de Deploy (Resumo)

### 1. Configuração Inicial
```bash
# Atualizar sistema
apt update && apt upgrade -y

# Configurar firewall
ufw allow 22/tcp && ufw allow 80/tcp && ufw allow 443/tcp
ufw enable

# Criar swap
fallocate -l 2G /swapfile
chmod 600 /swapfile
mkswap /swapfile && swapon /swapfile
```

### 2. Instalar Componentes
```bash
# .NET 8
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt update && apt install -y dotnet-sdk-8.0

# PostgreSQL 16
sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -
apt update && apt install -y postgresql-16

# Node.js 18
curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
apt install -y nodejs

# Nginx
apt install -y nginx

# Certbot
apt install -y certbot python3-certbot-nginx
```

### 3. Configurar Banco de Dados
```sql
-- Conectar ao PostgreSQL
sudo -u postgres psql

-- Criar usuário e bancos
CREATE USER primecare_user WITH PASSWORD 'SenhaForte123!';
CREATE DATABASE primecare_db;
CREATE DATABASE patient_portal_db;
GRANT ALL PRIVILEGES ON DATABASE primecare_db TO primecare_user;
GRANT ALL PRIVILEGES ON DATABASE patient_portal_db TO primecare_user;
```

### 4. Deploy das APIs
```bash
# Clonar repositório
git clone https://github.com/PrimeCareSoftware/MW.Code.git

# API Principal
cd MW.Code/src/MedicSoft.Api
dotnet publish -c Release -o /var/www/primecare/api

# Patient Portal API
cd ../../patient-portal-api/PatientPortal.Api
dotnet publish -c Release -o /var/www/primecare/patient-portal-api

# Criar serviços systemd
# Ver guia completo para configuração dos services
```

### 5. Deploy dos Frontends
```bash
# Medicwarehouse App
cd frontend/medicwarehouse-app
npm install && ng build --configuration production
cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/medicwarehouse-app/

# System Admin
cd ../mw-system-admin
npm install && ng build --configuration production
cp -r dist/mw-system-admin/browser/* /var/www/primecare/frontend/mw-system-admin/

# Patient Portal
cd ../patient-portal
npm install && ng build --configuration production
cp -r dist/patient-portal/browser/* /var/www/primecare/frontend/patient-portal/

# Docs
cd ../mw-docs
npm install && ng build --configuration production
cp -r dist/mw-docs/browser/* /var/www/primecare/frontend/mw-docs/
```

### 6. Configurar Nginx
```bash
# Configurar sites
# Ver guia completo para configuração completa do Nginx

# Testar configuração
nginx -t

# Reiniciar
systemctl restart nginx
```

### 7. Configurar SSL
```bash
# Obter certificados para todos os domínios
certbot --nginx -d meuprimecare.com.br -d www.meuprimecare.com.br
certbot --nginx -d api.meuprimecare.com.br
certbot --nginx -d api-paciente.meuprimecare.com.br
certbot --nginx -d admin.meuprimecare.com.br
certbot --nginx -d paciente.meuprimecare.com.br
certbot --nginx -d docs.meuprimecare.com.br
```

---

## 📊 Estimativa de Recursos por Aplicação

| Aplicação | RAM | CPU | Disco |
|-----------|-----|-----|-------|
| MedicSoft.Api | 512MB-1GB | 1 vCPU | 500MB |
| PatientPortal.Api | 512MB-1GB | 1 vCPU | 500MB |
| PostgreSQL | 1-2GB | 1 vCPU | 5-10GB |
| Nginx | 64MB | 0.5 vCPU | 100MB |
| Frontends (4x) | 0MB* | 0* | 2GB |
| Sistema + Swap | 1-2GB | - | 10GB |
| **TOTAL** | **4-7GB** | **3-4 vCPU** | **18-23GB** |

*Arquivos estáticos não consomem RAM/CPU

---

## 🔧 Manutenção

### Scripts Úteis

```bash
# Status do sistema
/usr/local/bin/primecare-status.sh

# Backup manual
/usr/local/bin/primecare-backup.sh

# Atualização
/usr/local/bin/primecare-update.sh
```

### Ver Logs

```bash
# Logs das APIs
journalctl -u primecare-api -f
journalctl -u patient-portal-api -f

# Logs do Nginx
tail -f /var/log/nginx/primecare-frontend-access.log
tail -f /var/log/nginx/primecare-frontend-error.log
```

### Verificar Status dos Serviços

```bash
systemctl status primecare-api
systemctl status patient-portal-api
systemctl status postgresql
systemctl status nginx
```

---

## 📚 Documentação Relacionada

### Guias de Deploy
- **[Guia Completo de Deploy](../../DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)** - Documentação detalhada
- **[Guia para Iniciantes](./DEPLOY_HOSTINGER_GUIA_COMPLETO.md)** - Passo a passo para iniciantes
- **[Guia de Início Rápido](./DEPLOY_HOSTINGER_INICIO_RAPIDO.md)** - Deploy em 30 minutos

### Infraestrutura
- **[Infraestrutura de Baixo Custo](./INFRA_PRODUCAO_BAIXO_CUSTO.md)** - Estratégias de economia
- **[Migração PostgreSQL](./MIGRACAO_POSTGRESQL.md)** - Detalhes do banco de dados
- **[Setup de Monitoramento](./SETUP_MONITORING.md)** - Configurar monitoramento

### Configuração
- **[Domínios e Subdomínios](./SUBDOMAIN_DOMAIN_CONFIGURATION.md)** - Configuração de DNS
- **[CI/CD](./CI_CD_DOCUMENTATION.md)** - Deploy automático

---

## 💡 Dicas Importantes

### Segurança
- ✅ Use senhas fortes para PostgreSQL
- ✅ Mantenha as portas 5000 e 5001 fechadas externamente
- ✅ Configure firewall (UFW) corretamente
- ✅ Mantenha certificados SSL atualizados
- ✅ Configure CORS apenas para domínios autorizados

### Performance
- ✅ Configure swap (mínimo 2GB)
- ✅ Otimize configurações do PostgreSQL
- ✅ Use gzip no Nginx para arquivos estáticos
- ✅ Configure cache de navegador
- ✅ Monitore uso de recursos

### Backups
- ✅ Configure backups diários automáticos
- ✅ Teste restauração dos backups
- ✅ Mantenha backups em local externo
- ✅ Retenha backups por pelo menos 7 dias

---

## 🆘 Problemas Comuns

### API não inicia
```bash
# Verificar logs
journalctl -u primecare-api -n 50

# Verificar porta
netstat -tulpn | grep 5000
```

### Frontend mostra página em branco
```bash
# Verificar arquivos
ls -la /var/www/primecare/frontend/medicwarehouse-app/

# Ver logs
tail -f /var/log/nginx/primecare-frontend-error.log
```

### Erro de conexão com banco
```bash
# Verificar PostgreSQL
systemctl status postgresql

# Testar conexão
sudo -u postgres psql -c "SELECT version();"
```

---

## ✅ Checklist de Deploy

- [ ] VPS contratado (KVM 2 recomendado)
- [ ] Sistema operacional instalado (Ubuntu 22.04)
- [ ] DNS configurado para todos os subdomínios
- [ ] Firewall configurado (UFW)
- [ ] Swap configurado (2GB mínimo)
- [ ] .NET 8 instalado
- [ ] PostgreSQL 16 instalado e configurado
- [ ] Node.js 18 instalado
- [ ] Nginx instalado
- [ ] 2 bancos de dados criados
- [ ] 2 APIs publicadas e rodando
- [ ] 4 frontends compilados e servidos
- [ ] SSL configurado em todos os domínios
- [ ] Backups automáticos configurados
- [ ] Monitoramento configurado
- [ ] Todas as URLs testadas

---

## 📞 Suporte

Para suporte adicional, consulte:
- **[Guia Completo](../../DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)** - Documentação detalhada
- **[Problemas Comuns](../../docs/COMMON_ISSUES.md)** - Soluções para erros comuns
- **[README Principal](../../README.md)** - Documentação geral do projeto

---

**Última atualização**: Janeiro 2026  
**Versão**: 1.0
