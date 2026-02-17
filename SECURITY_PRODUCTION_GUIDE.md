# 🔐 Guia de Segurança para Dados Sensíveis - Produção

> **🏥 Dados Médicos Requerem Máxima Segurança**  
> Este guia fornece práticas essenciais de segurança para lidar com dados sensíveis em produção.

## ⚖️ Conformidade Legal

### 📋 Leis e Regulamentações Aplicáveis

- **🇧🇷 LGPD** (Lei Geral de Proteção de Dados - Lei 13.709/2018)
  - Dados pessoais sensíveis (saúde) requerem proteção especial
  - Necessidade de consentimento explícito
  - Direito ao esquecimento e portabilidade
  - Penalidades de até 2% do faturamento (máx R$ 50 milhões)

- **🏥 CFM** (Conselho Federal de Medicina)
  - Resolução CFM 1.821/2007 (Prontuário Eletrônico)
  - Resolução CFM 2.314/2022 (Telemedicina)
  - Guarda obrigatória por 20 anos

- **🔒 ISO 27001** (opcional, mas recomendado)
  - Gestão de Segurança da Informação
  - Certificação internacional

---

## 🛡️ Checklist de Segurança Essencial

### ✅ Camada 1: Infraestrutura

- [ ] **Firewall Ativo** (UFW)
  - Apenas portas 22, 80, 443 expostas
  - Portas internas (5000, 5084, 5432) NÃO expostas

- [ ] **SSH Seguro**
  - Desabilitar login root via SSH
  - Usar chaves SSH ao invés de senha
  - Trocar porta padrão (22) se possível
  - Usar Fail2Ban contra ataques de força bruta

- [ ] **Sistema Atualizado**
  - Patches de segurança aplicados mensalmente
  - Atualizações automáticas configuradas

- [ ] **Swap Configurado**
  - Evita OOM (Out of Memory) que pode causar perda de dados

### ✅ Camada 2: Banco de Dados

- [ ] **Criptografia em Repouso**
  - Disco criptografado (LUKS no Linux)
  - Backup criptografado (GPG)

- [ ] **Acesso Restrito**
  - PostgreSQL aceita conexões APENAS de localhost
  - Usuário com privilégios mínimos necessários
  - Senha forte (20+ caracteres)

- [ ] **Auditoria de Acesso**
  - Log de todas as queries (apenas em desenvolvimento)
  - Log de todas as conexões
  - Revisão mensal dos logs

- [ ] **Backup Seguro**
  - Backup diário automatizado
  - Backup armazenado fora do servidor (S3, Backblaze)
  - Backup criptografado
  - Teste de restore mensal

### ✅ Camada 3: Aplicação

- [ ] **HTTPS Obrigatório**
  - SSL/TLS 1.2+ apenas
  - Certificado válido e renovado automaticamente
  - HSTS habilitado
  - Redirect HTTP → HTTPS

- [ ] **Autenticação Forte**
  - JWT com expiração curta (60 minutos)
  - Refresh token para renovação
  - 2FA habilitado (quando disponível)
  - Senhas com hash bcrypt (custo 12+)

- [ ] **Autorização Granular**
  - RBAC (Role-Based Access Control)
  - Princípio do menor privilégio
  - Validação de permissões em cada endpoint

- [ ] **Proteção contra Ataques**
  - Rate Limiting (máx 100 req/min por IP)
  - CORS restrito a domínios autorizados
  - SQL Injection (usar Entity Framework/ORM)
  - XSS (sanitização de inputs)
  - CSRF tokens

- [ ] **Logs de Auditoria**
  - Registrar TODAS as ações em dados sensíveis:
    - Quem acessou
    - Quando acessou
    - O que foi acessado/modificado
    - De qual IP
  - Retenção de logs por 6-12 meses
  - Logs imutáveis (append-only)

### ✅ Camada 4: Dados Sensíveis

- [ ] **Criptografia de Campos Sensíveis**
  - CPF, RG, dados bancários criptografados no banco
  - Prontuários médicos criptografados
  - Usar AES-256 ou superior

- [ ] **Minimização de Dados**
  - Coletar apenas dados necessários
  - Não armazenar dados desnecessários
  - Anonimizar dados quando possível

- [ ] **Anonimização em Logs**
  - NUNCA logar senhas (nem hasheadas)
  - NUNCA logar CPF, RG completos
  - Mascarar dados sensíveis em logs (ex: CPF: ***.***.123-45)

- [ ] **Retenção de Dados**
  - Política clara de retenção (20 anos para prontuários)
  - Exclusão segura de dados expirados
  - Soft-delete para auditoria

---

## 🔐 Configurações de Segurança Detalhadas

### 1. Configurar SSH com Chave Pública

```bash
# No seu computador local
ssh-keygen -t ed25519 -C "seu-email@exemplo.com"

# Copiar chave pública para o servidor
ssh-copy-id -i ~/.ssh/id_ed25519.pub root@IP_DO_SEU_VPS

# No servidor, desabilitar login por senha
nano /etc/ssh/sshd_config
```

Altere:
```
PasswordAuthentication no
PermitRootLogin no
PubkeyAuthentication yes
```

```bash
# Reiniciar SSH
systemctl restart sshd
```

### 2. Configurar Fail2Ban

```bash
# Instalar
apt install -y fail2ban

# Configurar
nano /etc/fail2ban/jail.local
```

Adicione:

```ini
[DEFAULT]
bantime = 3600
findtime = 600
maxretry = 3
destemail = seu-email@exemplo.com
sendername = Fail2Ban PrimeCare
action = %(action_mwl)s

[sshd]
enabled = true
port = 22
logpath = /var/log/auth.log
maxretry = 3

[nginx-http-auth]
enabled = true
filter = nginx-http-auth
port = http,https
logpath = /var/log/nginx/error.log

[nginx-noscript]
enabled = true
port = http,https
filter = nginx-noscript
logpath = /var/log/nginx/access.log
maxretry = 6

[nginx-badbots]
enabled = true
port = http,https
filter = nginx-badbots
logpath = /var/log/nginx/access.log
maxretry = 2

[nginx-noproxy]
enabled = true
port = http,https
filter = nginx-noproxy
logpath = /var/log/nginx/access.log
maxretry = 2
```

```bash
# Iniciar e habilitar
systemctl enable fail2ban
systemctl start fail2ban

# Verificar status
fail2ban-client status
```

### 3. Criptografar Banco de Dados

```bash
# Instalar LUKS (já vem no Ubuntu)
apt install -y cryptsetup

# Criar volume criptografado (CUIDADO: VAI APAGAR DADOS)
# Faça isso ANTES de iniciar o uso em produção
cryptsetup luksFormat /dev/sdX
cryptsetup luksOpen /dev/sdX encrypted_volume
mkfs.ext4 /dev/mapper/encrypted_volume

# Montar
mkdir -p /var/lib/postgresql/data-encrypted
mount /dev/mapper/encrypted_volume /var/lib/postgresql/data-encrypted

# Configurar PostgreSQL para usar o volume criptografado
# Editar /etc/postgresql/16/main/postgresql.conf
# data_directory = '/var/lib/postgresql/data-encrypted'
```

### 4. Configurar Backup Criptografado

```bash
# Criar script de backup criptografado
nano /var/www/primecare/scripts/backup-encrypted.sh
```

Adicione:

```bash
#!/bin/bash

BACKUP_DIR="/var/backups/primecare"
DATE=$(date +%Y%m%d_%H%M%S)
ENCRYPTION_KEY="SuaChaveDeBackup123!"  # MUDAR ISSO!

# Criar diretório
mkdir -p $BACKUP_DIR

# Backup do banco
echo "Backing up database..."
sudo -u postgres pg_dump primecare | gzip | \
  gpg --symmetric --cipher-algo AES256 --passphrase "$ENCRYPTION_KEY" \
  > $BACKUP_DIR/primecare_$DATE.sql.gz.gpg

sudo -u postgres pg_dump telemedicine | gzip | \
  gpg --symmetric --cipher-algo AES256 --passphrase "$ENCRYPTION_KEY" \
  > $BACKUP_DIR/telemedicine_$DATE.sql.gz.gpg

# Limpar backups antigos (30 dias)
find $BACKUP_DIR -name "*.gpg" -mtime +30 -delete

echo "Encrypted backup completed: $DATE"
```

```bash
# Dar permissão
chmod +x /var/www/primecare/scripts/backup-encrypted.sh

# Testar
/var/www/primecare/scripts/backup-encrypted.sh

# Para restaurar:
# gpg --decrypt backup.sql.gz.gpg | gunzip | psql -U primecare_user primecare
```

### 5. Configurar HSTS e Security Headers

```bash
# Editar configuração global do Nginx
nano /etc/nginx/nginx.conf
```

Adicione dentro de `http {}`:

```nginx
# Ocultar versão do Nginx
server_tokens off;

# HSTS (HTTP Strict Transport Security)
add_header Strict-Transport-Security "max-age=31536000; includeSubDomains; preload" always;

# XSS Protection
add_header X-XSS-Protection "1; mode=block" always;

# Clickjacking Protection
add_header X-Frame-Options "SAMEORIGIN" always;

# MIME Type Sniffing Protection
add_header X-Content-Type-Options "nosniff" always;

# Referrer Policy
add_header Referrer-Policy "strict-origin-when-cross-origin" always;

# Content Security Policy (ajustar conforme necessário)
add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' https://api.daily.co;" always;

# Permissions Policy (substituto do Feature-Policy)
add_header Permissions-Policy "geolocation=(), microphone=(self), camera=(self)" always;
```

```bash
# Testar e reiniciar
nginx -t
systemctl restart nginx
```

### 6. Configurar Auditoria no PostgreSQL

```bash
# Instalar extensão pgAudit
apt install -y postgresql-16-pgaudit

# Conectar ao banco
sudo -u postgres psql primecare
```

```sql
-- Habilitar extensão
CREATE EXTENSION IF NOT EXISTS pgaudit;

-- Configurar auditoria
ALTER SYSTEM SET pgaudit.log = 'ddl, role, read, write';
ALTER SYSTEM SET pgaudit.log_catalog = on;
ALTER SYSTEM SET pgaudit.log_parameter = on;
ALTER SYSTEM SET pgaudit.log_relation = on;

-- Aplicar mudanças
SELECT pg_reload_conf();
```

```bash
# Verificar logs de auditoria
tail -f /var/log/postgresql/postgresql-16-main.log
```

---

## 🔍 Monitoramento de Segurança

### 1. Monitorar Tentativas de Acesso

```bash
# Ver tentativas de login SSH falhadas
grep "Failed password" /var/log/auth.log | tail -20

# Ver IPs banidos pelo Fail2Ban
fail2ban-client status sshd

# Ver tentativas de acesso à API
grep "401\|403" /var/log/nginx/access.log | tail -20
```

### 2. Monitorar Integridade de Arquivos

```bash
# Instalar AIDE
apt install -y aide

# Inicializar banco de dados
aide --init
mv /var/lib/aide/aide.db.new /var/lib/aide/aide.db

# Verificar integridade (executar diariamente)
aide --check

# Atualizar banco de dados após mudanças legítimas
aide --update
mv /var/lib/aide/aide.db.new /var/lib/aide/aide.db
```

### 3. Escanear Vulnerabilidades

```bash
# Instalar Lynis (scanner de segurança)
apt install -y lynis

# Executar auditoria completa
lynis audit system

# Ver relatório
cat /var/log/lynis.log
```

---

## 📋 Checklist de Conformidade LGPD

### ✅ Direitos dos Titulares

- [ ] **Direito de Acesso**: Paciente pode visualizar seus dados
- [ ] **Direito de Correção**: Paciente pode corrigir dados incorretos
- [ ] **Direito de Exclusão**: Soft-delete com guarda de 20 anos
- [ ] **Direito de Portabilidade**: Exportar dados em formato estruturado
- [ ] **Direito de Oposição**: Paciente pode recusar determinados tratamentos
- [ ] **Direito de Informação**: Política de Privacidade clara

### ✅ Bases Legais

- [ ] **Consentimento**: Termo de consentimento explícito
- [ ] **Execução de Contrato**: Prestação de serviço médico
- [ ] **Obrigação Legal**: Conformidade com CFM e ANVISA
- [ ] **Proteção da Vida**: Situações de emergência

### ✅ Medidas Técnicas

- [ ] **Pseudonimização**: Separar dados identificadores de dados sensíveis
- [ ] **Criptografia**: AES-256 para dados em repouso e TLS 1.2+ para dados em trânsito
- [ ] **Controle de Acesso**: RBAC com princípio do menor privilégio
- [ ] **Registro de Operações**: Log de todas as operações com dados pessoais

### ✅ Medidas Organizacionais

- [ ] **Encarregado de Dados (DPO)**: Pessoa responsável designada
- [ ] **Política de Privacidade**: Publicada e atualizada
- [ ] **Treinamento**: Equipe treinada em LGPD
- [ ] **Contratos**: Acordos com terceiros (Daily.co, etc.)
- [ ] **Relatório de Impacto**: RIPD para tratamentos de alto risco

### ✅ Resposta a Incidentes

- [ ] **Plano de Resposta**: Procedimentos documentados
- [ ] **Notificação**: Processo para notificar ANPD e titulares em até 72h
- [ ] **Investigação**: Equipe treinada para investigar incidentes
- [ ] **Remediação**: Plano de contenção e correção

---

## 🚨 Plano de Resposta a Incidentes

### Etapa 1: Detecção (0-1h)

1. Identificar o incidente
2. Classificar a severidade (baixa, média, alta, crítica)
3. Isolar sistemas afetados se necessário

### Etapa 2: Contenção (1-4h)

1. Bloquear acesso não autorizado
2. Preservar evidências
3. Notificar equipe de resposta

### Etapa 3: Análise (4-24h)

1. Investigar causa raiz
2. Determinar extensão do vazamento
3. Identificar dados afetados

### Etapa 4: Notificação (24-72h)

1. Notificar ANPD se necessário
2. Notificar titulares afetados
3. Publicar comunicado se aplicável

### Etapa 5: Remediação (72h+)

1. Corrigir vulnerabilidade
2. Atualizar sistemas
3. Reforçar controles
4. Atualizar documentação

### Etapa 6: Revisão (30 dias)

1. Revisar resposta
2. Identificar lições aprendidas
3. Atualizar plano de resposta
4. Treinar equipe

---

## 📚 Recursos Adicionais

### Documentação

- [LGPD - Lei 13.709/2018](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [CFM Resolução 1.821/2007](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821)
- [CFM Resolução 2.314/2022](https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2022/2314)

### Ferramentas de Segurança

- **Fail2Ban**: https://www.fail2ban.org/
- **Lynis**: https://cisofy.com/lynis/
- **AIDE**: https://aide.github.io/
- **Certbot**: https://certbot.eff.org/
- **OWASP**: https://owasp.org/

### Testes de Segurança Online

- **SSL Labs**: https://www.ssllabs.com/ssltest/
- **Security Headers**: https://securityheaders.com/
- **Mozilla Observatory**: https://observatory.mozilla.org/

---

## ⚠️ Avisos Importantes

### 🚨 NUNCA FAÇA ISSO:

- ❌ Commitar senhas ou chaves no Git
- ❌ Usar senhas fracas ou padrão
- ❌ Expor portas de banco de dados
- ❌ Desabilitar SSL/HTTPS
- ❌ Logar senhas ou tokens
- ❌ Enviar dados sensíveis sem criptografia
- ❌ Ignorar atualizações de segurança
- ❌ Usar software pirata ou não licenciado
- ❌ Compartilhar credenciais entre usuários
- ❌ Acessar produção de redes públicas sem VPN

### ✅ SEMPRE FAÇA ISSO:

- ✅ Use senhas únicas e fortes (20+ caracteres)
- ✅ Ative 2FA onde disponível
- ✅ Mantenha backups criptografados e testados
- ✅ Monitore logs diariamente
- ✅ Atualize sistema mensalmente
- ✅ Treine equipe em segurança
- ✅ Teste plano de recuperação de desastres
- ✅ Documente tudo
- ✅ Revise acessos trimestralmente
- ✅ Contrate auditoria externa anualmente

---

**Última atualização**: Fevereiro 2026  
**Versão**: 1.0  
**Autor**: PrimeCare Software Team

🔐 **A segurança dos dados dos seus pacientes é sua responsabilidade!**
