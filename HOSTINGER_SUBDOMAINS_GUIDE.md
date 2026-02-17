# 🌐 Guia Rápido: Criar Subdomínios na Hostinger

> **📋 Referência Rápida**  
> Como criar subdomínios no painel da Hostinger (hPanel) para seu sistema PrimeCare

## ❓ Onde Criar Subdomínios na Hostinger?

### 🔍 O Problema Comum

Muitos usuários relatam **não encontrar a opção de criar subdomínios** no painel da Hostinger. Isso acontece porque:

1. A opção está "escondida" dentro das configurações de domínio
2. Você precisa ter um domínio já registrado ou apontado para a Hostinger
3. Para VPS, você gerencia subdomínios via **DNS**, não via cPanel

---

## 📍 Para VPS Hostinger (Nosso Caso)

### ⚠️ Importante: VPS ≠ Hospedagem Compartilhada

No **VPS da Hostinger**, você **NÃO** tem um painel cPanel tradicional. Você gerencia subdomínios através de:

1. **Registros DNS** no painel da Hostinger
2. **Configuração do Nginx** no próprio servidor VPS

---

## 🎯 Passo a Passo: Criar Subdomínios no VPS

### Etapa 1: Acessar Configurações de DNS

1. Entre em: **https://hpanel.hostinger.com**
2. Faça login com suas credenciais
3. No menu lateral esquerdo, clique em **"Domínios"**
4. Clique no seu domínio (ex: `suaclinica.com.br`)
5. Na página do domínio, procure por **"DNS / Name Servers"**
6. Clique em **"DNS / Name Servers"**
7. Role a página até encontrar **"Registros DNS"** ou **"DNS Records"**

### Etapa 2: Criar Registros A para Subdomínios

Para cada subdomínio que você precisa, crie um **Registro A**:

#### Exemplo de Registros Necessários:

```
┌─────────────┬────────┬─────────────────┬──────┐
│ Nome/Host   │ Tipo   │ Aponta para     │ TTL  │
├─────────────┼────────┼─────────────────┼──────┤
│ @           │ A      │ IP_DO_SEU_VPS   │ 3600 │
│ www         │ A      │ IP_DO_SEU_VPS   │ 3600 │
│ api         │ A      │ IP_DO_SEU_VPS   │ 3600 │
│ tele        │ A      │ IP_DO_SEU_VPS   │ 3600 │
│ admin       │ A      │ IP_DO_SEU_VPS   │ 3600 │
└─────────────┴────────┴─────────────────┴──────┘
```

#### Como Adicionar Cada Registro:

1. Clique no botão **"Adicionar Registro"** ou **"Add Record"**
2. Selecione **"Tipo: A"**
3. No campo **"Nome"** ou **"Host"**, digite o subdomínio:
   - Para domínio raiz: digite `@`
   - Para www: digite `www`
   - Para api: digite `api`
   - Para telemedicina: digite `tele`
   - Para admin: digite `admin`
4. No campo **"Aponta para"** ou **"Points to"** ou **"Value"**, digite o **IP do seu VPS**
5. No campo **"TTL"**, deixe **3600** (1 hora)
6. Clique em **"Adicionar"** ou **"Add"** ou **"Salvar"**

### Etapa 3: Aguardar Propagação do DNS

- **Tempo mínimo**: 15 minutos
- **Tempo máximo**: 48 horas
- **Tempo médio**: 2-6 horas

#### Como Verificar a Propagação:

```bash
# No seu computador, abra o terminal e execute:

# Para Windows (PowerShell ou CMD)
nslookup api.suaclinica.com.br
nslookup tele.suaclinica.com.br
nslookup admin.suaclinica.com.br

# Para Linux/Mac
dig api.suaclinica.com.br
dig tele.suaclinica.com.br
dig admin.suaclinica.com.br
```

**Resultado esperado**: O comando deve retornar o IP do seu VPS.

#### Alternativa Online:

Use o site **https://dnschecker.org** para verificar a propagação mundial:
1. Acesse https://dnschecker.org
2. Digite seu subdomínio (ex: `api.suaclinica.com.br`)
3. Selecione o tipo **A**
4. Clique em **Search**
5. Veja se o IP aparece corretamente em vários locais do mundo

---

## 🖥️ Configurar Nginx no VPS

Depois que o DNS estiver propagado, você precisa configurar o **Nginx** no VPS para responder aos subdomínios.

### Exemplo de Configuração para API Principal:

```bash
# Conectar ao VPS
ssh root@IP_DO_SEU_VPS

# Criar configuração do Nginx
nano /etc/nginx/sites-available/api.suaclinica.com.br
```

Adicione:

```nginx
server {
    listen 80;
    server_name api.suaclinica.com.br;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Habilitar o site
ln -s /etc/nginx/sites-available/api.suaclinica.com.br /etc/nginx/sites-enabled/

# Testar configuração
nginx -t

# Se OK, reiniciar Nginx
systemctl restart nginx
```

Repita o processo para cada subdomínio (tele, admin, etc.).

---

## 🔒 Configurar SSL (HTTPS)

Depois que os subdomínios estiverem funcionando, instale certificados SSL:

```bash
# Instalar Certbot (se ainda não tiver)
apt install -y certbot python3-certbot-nginx

# Obter certificado para cada subdomínio
certbot --nginx -d api.suaclinica.com.br
certbot --nginx -d tele.suaclinica.com.br
certbot --nginx -d admin.suaclinica.com.br
certbot --nginx -d suaclinica.com.br -d www.suaclinica.com.br
```

Para cada comando:
1. Digite seu email
2. Aceite os termos (Y)
3. Escolha opção **2** (redirect para HTTPS)

---

## 📋 Checklist de Verificação

### ✅ DNS Configurado

- [ ] Registro A para domínio raiz (`@`) criado
- [ ] Registro A para `www` criado
- [ ] Registro A para `api` criado
- [ ] Registro A para `tele` criado
- [ ] Registro A para `admin` criado
- [ ] Todos os registros apontam para o IP correto do VPS
- [ ] DNS propagado (verificado com nslookup ou dnschecker.org)

### ✅ Nginx Configurado

- [ ] Arquivo de configuração criado para cada subdomínio
- [ ] Symlink criado em sites-enabled
- [ ] Nginx testado sem erros (`nginx -t`)
- [ ] Nginx reiniciado

### ✅ SSL Instalado

- [ ] Certbot instalado
- [ ] Certificado SSL obtido para todos os subdomínios
- [ ] HTTPS funcionando (cadeado verde no navegador)
- [ ] Renovação automática testada (`certbot renew --dry-run`)

### ✅ Testes Finais

- [ ] https://suaclinica.com.br acessível
- [ ] https://api.suaclinica.com.br/swagger acessível
- [ ] https://tele.suaclinica.com.br/swagger acessível
- [ ] https://admin.suaclinica.com.br acessível

---

## 🆘 Problemas Comuns

### Problema 1: "Não encontro a opção de criar subdomínios"

**Solução**: No VPS, você não cria subdomínios no painel da Hostinger. Você cria **Registros DNS tipo A** que apontam para o IP do seu VPS.

### Problema 2: "O subdomínio não carrega"

**Causas possíveis**:
1. DNS ainda não propagou (aguarde até 48h)
2. Nginx não está configurado corretamente
3. Firewall bloqueando portas 80/443

**Como verificar**:
```bash
# Verificar se Nginx está rodando
systemctl status nginx

# Verificar se porta 80 está aberta
netstat -tulpn | grep :80

# Verificar firewall
ufw status
```

### Problema 3: "Erro de SSL/HTTPS"

**Causas possíveis**:
1. Certificado não foi instalado
2. DNS não propagou antes de tentar instalar SSL
3. Porta 443 bloqueada no firewall

**Solução**:
```bash
# Abrir porta 443
ufw allow 443/tcp

# Tentar novamente
certbot --nginx -d api.suaclinica.com.br
```

### Problema 4: "Registro DNS não salva"

**Causas possíveis**:
1. Domínio não está apontando para os nameservers da Hostinger
2. Você está tentando criar o registro no lugar errado

**Solução**:
1. Verifique se o domínio usa nameservers da Hostinger:
   - `ns1.dns-parking.com`
   - `ns2.dns-parking.com`
2. Se não, você precisa criar os registros no provedor atual do domínio (Registro.br, GoDaddy, etc.)

---

## 📚 Documentação Relacionada

- **[PRODUCAO_HOSTINGER_GUIDE.md](PRODUCAO_HOSTINGER_GUIDE.md)** - Guia completo de deploy em produção
- **[DEPLOY_HOSTINGER_GUIA_COMPLETO.md](system-admin/infrastructure/DEPLOY_HOSTINGER_GUIA_COMPLETO.md)** - Para iniciantes
- **[DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md](DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)** - Deploy de múltiplos projetos

---

## 🎯 Resumo Rápido

### Para criar subdomínios no VPS Hostinger:

1. **DNS**: Crie Registros A no painel hPanel (Domínios → Seu Domínio → DNS)
2. **Aguarde**: Propagação do DNS (15min - 48h)
3. **Nginx**: Configure proxy reverso no VPS
4. **SSL**: Instale certificados com Certbot
5. **Teste**: Acesse os subdomínios no navegador

### Importante:

- ❌ VPS NÃO tem cPanel com opção "Subdomínios"
- ✅ VPS usa DNS + Nginx para gerenciar subdomínios
- 🔒 Sempre use HTTPS em produção

---

**Última atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Autor**: PrimeCare Software Team

🌐 **Bons subdomínios!**
