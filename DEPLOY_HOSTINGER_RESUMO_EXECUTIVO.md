# 📋 Resumo Executivo - Deploy na Hostinger

## 🎯 Objetivo

Este é um resumo executivo do guia completo de deploy para **múltiplos projetos** (APIs + Angular + Banco de Dados) na Hostinger.

**📖 Guia Completo:** [DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md](./DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)

---

## 🚀 O que será implantado?

### Backend (.NET 8)
1. **MedicSoft.Api** - API Principal (porta 5000)
2. **PatientPortal.Api** - API Portal do Paciente (porta 5001)

### Frontend (Angular)
1. **medicwarehouse-app** - Aplicação principal
2. **mw-system-admin** - Sistema de administração
3. **patient-portal** - Portal do paciente
4. **mw-docs** - Documentação (opcional)

### Banco de Dados
1. **PostgreSQL 16** com 2 bancos de dados:
   - `omnicare_db` (API Principal)
   - `patient_portal_db` (Portal do Paciente)

---

## 💰 Qual plano da Hostinger escolher?

### Recomendação: **Plano KVM 2** (R$ 39,99/mês)

| Plano | RAM | CPU | Disco | Preço/mês* | Para quem? |
|-------|-----|-----|-------|------------|------------|
| **KVM 1** | 4GB | 2 vCPU | 50GB | R$ 19,99 | Testes/desenvolvimento |
| **KVM 2** ⭐ | 8GB | 4 vCPU | 100GB | R$ 39,99 | **Produção (IDEAL)** |
| **KVM 3** | 12GB | 6 vCPU | 150GB | R$ 59,99 | Alta disponibilidade |
| **KVM 4** | 16GB | 8 vCPU | 200GB | R$ 79,99 | Alto desempenho |

*Valores aproximados para contrato anual

### Por que KVM 2?

✅ **RAM suficiente** (8GB) para 2 APIs + PostgreSQL + 4 frontends  
✅ **CPU adequada** (4 vCPU) para processar requisições  
✅ **Margem de segurança** para picos de acesso  
✅ **Melhor custo-benefício** - R$ 39,99/mês  
✅ **Capacidade**: 10-30 usuários simultâneos  
✅ **Escalável**: Pode crescer até 20-30 clínicas

---

## 🏗️ Arquitetura da Solução

```
                    Internet (HTTPS)
                           |
                    Nginx (Porta 80/443)
                    [SSL + Proxy Reverso]
                           |
        ┏━━━━━━━━━━━━━━━━━━┻━━━━━━━━━━━━━━━━━━┓
        ┃                                      ┃
  APIs (.NET 8)                        Frontends (Angular)
  ┣━ MedicSoft.Api (5000)               ┣━ medicwarehouse-app
  ┗━ PatientPortal.Api (5001)           ┣━ mw-system-admin
        |                               ┣━ patient-portal
        |                               ┗━ mw-docs
        ↓
  PostgreSQL 16 (5432)
  ┣━ omnicare_db
  ┗━ patient_portal_db
```

---

## 🌐 Estrutura de Domínios

Com 1 domínio (ex: `meuomnicare.com.br`), você criará **7 subdomínios**:

| Subdomínio | Aplicação | Tipo |
|------------|-----------|------|
| `meuomnicare.com.br` | medicwarehouse-app | Frontend Principal |
| `api.meuomnicare.com.br` | MedicSoft.Api | API Principal |
| `admin.meuomnicare.com.br` | mw-system-admin | Frontend Admin |
| `paciente.meuomnicare.com.br` | patient-portal | Frontend Portal |
| `api-paciente.meuomnicare.com.br` | PatientPortal.Api | API Portal |
| `docs.meuomnicare.com.br` | mw-docs | Frontend Docs |

**Todos com SSL/HTTPS gratuito via Let's Encrypt!**

---

## ⏱️ Tempo e Custo Estimados

### Tempo de Implementação
- **Primeira vez**: 3-4 horas
- **Com experiência**: 1-2 horas
- **Atualização futura**: 15-30 minutos

### Custo Mensal Total
| Item | Custo |
|------|-------|
| VPS KVM 2 (Hostinger) | R$ 39,99 |
| Domínio (.com.br) | R$ 3,33 (R$ 40/ano) |
| SSL (Let's Encrypt) | R$ 0,00 (gratuito) |
| Backup externo (opcional) | R$ 10-30 |
| **TOTAL** | **R$ 43,32 - 73,32** |

**💡 Economia vs. Cloud tradicional: 60-80%**

---

## 📊 Recursos Necessários

### Consumo por Aplicação

| Componente | RAM | CPU | Disco |
|------------|-----|-----|-------|
| MedicSoft.Api | 512MB-1GB | 1 vCPU | 500MB |
| PatientPortal.Api | 512MB-1GB | 1 vCPU | 500MB |
| PostgreSQL | 1-2GB | 1 vCPU | 5-10GB |
| Nginx | 64MB | 0.5 vCPU | 100MB |
| 4x Frontends* | 0MB | 0 vCPU | 2GB |
| Sistema + Swap | 1-2GB | - | 10GB |
| **TOTAL** | **4-7GB** | **3-4 vCPU** | **18-23GB** |

*Arquivos estáticos servidos pelo Nginx, não consomem RAM/CPU

**✅ KVM 2 tem recursos suficientes com margem de segurança!**

---

## 🔧 Componentes a Instalar no VPS

1. **Sistema Operacional**: Ubuntu 22.04 LTS (recomendado)
2. **.NET 8 SDK**: Para rodar as APIs
3. **PostgreSQL 16**: Banco de dados
4. **Node.js 18**: Para compilar os frontends Angular
5. **Nginx**: Servidor web e proxy reverso
6. **Certbot**: Para certificados SSL gratuitos

---

## 📝 Resumo do Processo

### 1️⃣ Contratar VPS na Hostinger
- Acessar: https://www.hostinger.com.br/vps
- Escolher: **KVM 2** (R$ 39,99/mês)
- Sistema: **Ubuntu 22.04 LTS**

### 2️⃣ Configurar Servidor
- Conectar via SSH
- Atualizar sistema
- Configurar firewall (UFW)
- Criar swap de 2GB
- Criar usuário de deploy

### 3️⃣ Instalar Componentes
```bash
# .NET 8 SDK
apt install dotnet-sdk-8.0

# PostgreSQL 16
apt install postgresql-16

# Node.js 18
apt install nodejs

# Nginx
apt install nginx

# Certbot
apt install certbot python3-certbot-nginx
```

### 4️⃣ Configurar Banco de Dados
```sql
-- Criar usuário
CREATE USER omnicare_user WITH PASSWORD 'SenhaForte123!';

-- Criar bancos
CREATE DATABASE omnicare_db;
CREATE DATABASE patient_portal_db;

-- Dar permissões
GRANT ALL PRIVILEGES ON DATABASE omnicare_db TO omnicare_user;
GRANT ALL PRIVILEGES ON DATABASE patient_portal_db TO omnicare_user;
```

### 5️⃣ Deploy das APIs
```bash
# Clonar repositório
git clone https://github.com/Omni CareSoftware/MW.Code.git

# Publicar API Principal
cd src/MedicSoft.Api
dotnet publish -c Release -o /var/www/primecare/api

# Publicar Patient Portal API
cd patient-portal-api/PatientPortal.Api
dotnet publish -c Release -o /var/www/primecare/patient-portal-api

# Criar serviços systemd para ambas as APIs
# Iniciar serviços
systemctl start omnicare-api
systemctl start patient-portal-api
```

### 6️⃣ Build e Deploy dos Frontends
```bash
# Para cada app Angular:
cd frontend/medicwarehouse-app
npm install
ng build --configuration production
cp -r dist/medicwarehouse-app/browser/* /var/www/primecare/frontend/medicwarehouse-app/

# Repetir para: mw-system-admin, patient-portal, mw-docs
```

### 7️⃣ Configurar Nginx
- Criar configurações para proxy reverso das APIs
- Criar configurações para servir os frontends
- Testar: `nginx -t`
- Reiniciar: `systemctl restart nginx`

### 8️⃣ Configurar DNS
No seu provedor de domínio, criar registros A:
- `@` → IP do VPS
- `www` → IP do VPS
- `api` → IP do VPS
- `api-paciente` → IP do VPS
- `admin` → IP do VPS
- `paciente` → IP do VPS
- `docs` → IP do VPS

### 9️⃣ Configurar SSL
```bash
# Obter certificados para todos os domínios
certbot --nginx -d meuomnicare.com.br
certbot --nginx -d api.meuomnicare.com.br
certbot --nginx -d api-paciente.meuomnicare.com.br
certbot --nginx -d admin.meuomnicare.com.br
certbot --nginx -d paciente.meuomnicare.com.br
certbot --nginx -d docs.meuomnicare.com.br
```

### 🔟 Configurar Backups Automáticos
- Script de backup para PostgreSQL
- Backup de configurações
- Cron job para execução diária

---

## ✅ Checklist Final

Antes de considerar completo:

**Infraestrutura:**
- [ ] VPS KVM 2 contratado
- [ ] Ubuntu 22.04 instalado
- [ ] Swap de 2GB configurado
- [ ] Firewall (UFW) ativo
- [ ] Domínio registrado

**Software:**
- [ ] .NET 8 instalado
- [ ] PostgreSQL 16 instalado e rodando
- [ ] Node.js 18 instalado
- [ ] Nginx instalado e rodando
- [ ] Certbot instalado

**Banco de Dados:**
- [ ] 2 bancos criados
- [ ] Usuário com permissões
- [ ] Migrations executadas

**APIs:**
- [ ] MedicSoft.Api publicada e rodando (porta 5000)
- [ ] PatientPortal.Api publicada e rodando (porta 5001)
- [ ] Serviços systemd configurados
- [ ] APIs acessíveis via Nginx

**Frontends:**
- [ ] 4 apps Angular compilados
- [ ] Arquivos copiados para /var/www/primecare/frontend/
- [ ] Servidos corretamente pelo Nginx

**DNS e SSL:**
- [ ] Todos os registros DNS configurados
- [ ] DNS propagado (15min - 48h)
- [ ] SSL instalado em todos os domínios
- [ ] HTTPS funcionando

**Segurança:**
- [ ] Senhas fortes configuradas
- [ ] Portas 5000/5001 não expostas externamente
- [ ] CORS configurado corretamente
- [ ] Firewall permitindo apenas portas necessárias

**Backups e Monitoramento:**
- [ ] Script de backup configurado
- [ ] Cron job de backup ativo
- [ ] Script de monitoramento criado
- [ ] Logs sendo gerados corretamente

**Testes:**
- [ ] https://meuomnicare.com.br acessível
- [ ] https://api.meuomnicare.com.br/swagger acessível
- [ ] https://api-paciente.meuomnicare.com.br/swagger acessível
- [ ] https://admin.meuomnicare.com.br acessível
- [ ] https://paciente.meuomnicare.com.br acessível
- [ ] https://docs.meuomnicare.com.br acessível
- [ ] Login funcionando
- [ ] APIs respondendo corretamente

---

## 🆘 Problemas Comuns

### API não inicia
```bash
# Ver logs
journalctl -u omnicare-api -n 50

# Verificar porta
netstat -tulpn | grep 5000
```

### Frontend mostra página em branco
```bash
# Verificar arquivos
ls -la /var/www/primecare/frontend/medicwarehouse-app/

# Ver logs do Nginx
tail -f /var/log/nginx/error.log
```

### Erro de conexão com banco
```bash
# Verificar PostgreSQL
systemctl status postgresql

# Testar conexão
sudo -u postgres psql -c "SELECT version();"
```

### CORS Error
- Verificar `Cors__AllowedOrigins` no `appsettings.Production.json`
- Reiniciar API após mudanças
- Usar HTTPS (não HTTP)

---

## 📚 Documentação Adicional

### Guias Detalhados
- **[DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md](./DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)** - Guia completo passo a passo
- **[DEPLOY_HOSTINGER_GUIA_COMPLETO.md](./system-admin/infrastructure/DEPLOY_HOSTINGER_GUIA_COMPLETO.md)** - Para iniciantes
- **[DEPLOY_HOSTINGER_INICIO_RAPIDO.md](./system-admin/infrastructure/DEPLOY_HOSTINGER_INICIO_RAPIDO.md)** - Deploy em 30 min

### Infraestrutura
- **[INFRA_DOCS_INDEX.md](./system-admin/infrastructure/INFRA_DOCS_INDEX.md)** - Índice de toda documentação
- **[MIGRACAO_POSTGRESQL.md](./system-admin/infrastructure/MIGRACAO_POSTGRESQL.md)** - Migração para PostgreSQL

---

## 💡 Dicas Finais

### Para Economizar
1. Contrate plano anual (desconto de até 75%)
2. Comece com KVM 2 (fácil fazer upgrade depois)
3. Use Let's Encrypt para SSL (gratuito)
4. Configure backups para evitar perda de dados

### Para Performance
1. Configure swap de 2GB mínimo
2. Otimize configurações do PostgreSQL
3. Use gzip no Nginx
4. Configure cache de navegador

### Para Segurança
1. Use senhas fortes
2. Mantenha sistema atualizado
3. Configure firewall corretamente
4. Não exponha portas das APIs diretamente
5. Configure CORS apenas para domínios autorizados

---

## 🎯 Próximos Passos Após Deploy

1. **Monitoramento**: Configurar alertas de erro e uso de recursos
2. **CI/CD**: Automatizar deploy com GitHub Actions
3. **Cache**: Adicionar Redis para melhorar performance
4. **CDN**: Usar Cloudflare para distribuir conteúdo
5. **Backup Externo**: Configurar backup para storage externo (S3, BackBlaze)

---

## 📞 Precisa de Ajuda?

- **Guia Completo**: [DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md](./DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)
- **Índice de Documentação**: [INFRA_DOCS_INDEX.md](./system-admin/infrastructure/INFRA_DOCS_INDEX.md)
- **README Principal**: [README.md](./README.md)

---

**Última atualização**: Janeiro 2026  
**Versão**: 1.0

🚀 **Boa sorte com seu deploy!**
