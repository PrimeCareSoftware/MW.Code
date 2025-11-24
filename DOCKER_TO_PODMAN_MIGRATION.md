# 🔄 Guia de Migração: Docker para Podman

## 📋 Visão Geral

Este guia orienta a migração do Docker para o Podman no MedicWarehouse. O Podman é uma alternativa **100% gratuita e open-source** ao Docker, ideal para uso em produção sem custos de licenciamento.

## 🎯 Por que migrar para Podman?

### Problemas com Docker em Produção
- 💰 **Licenciamento**: Docker Desktop requer licença paga para empresas com mais de 250 funcionários ou $10M+ de receita
- 💸 **Custos**: Taxas mensais para uso comercial
- 🔒 **Restrições**: Limitações de uso em ambientes corporativos

### Benefícios do Podman
- ✅ **100% Gratuito**: Licença Apache 2.0, sem custos para uso comercial
- ✅ **Daemonless**: Mais seguro (não requer daemon root rodando)
- ✅ **Compatível com Docker**: Usa mesmos comandos e arquivos (OCI Standard)
- ✅ **Rootless**: Pode rodar sem privilégios root
- ✅ **Production-ready**: Usado por Red Hat, IBM, Fedora e outras grandes empresas
- ✅ **Kubernetes-native**: Melhor integração com Kubernetes
- ✅ **Mais Leve**: Menor overhead de recursos

## 🚀 Instalação do Podman

### Linux (Ubuntu/Debian)
```bash
# Atualizar repositórios
sudo apt update

# Instalar Podman e Podman Compose
sudo apt install -y podman podman-compose

# Verificar instalação
podman --version
podman-compose --version
```

### Linux (Fedora/RHEL/CentOS)
```bash
# Instalar Podman e Podman Compose
sudo dnf install -y podman podman-compose

# Verificar instalação
podman --version
podman-compose --version
```

### macOS
```bash
# Instalar via Homebrew
brew install podman podman-compose

# Inicializar máquina virtual do Podman
podman machine init
podman machine start

# Verificar instalação
podman --version
podman-compose --version
```

### Windows
```bash
# Opção 1: Instalador oficial
# Download: https://github.com/containers/podman/releases

# Opção 2: Via WSL2 (recomendado)
# 1. Instale WSL2
# 2. Instale Ubuntu no WSL2
# 3. Siga as instruções do Linux Ubuntu acima
```

## 📝 Mudanças Necessárias

### 1. Comandos Básicos

A maioria dos comandos Docker funciona substituindo `docker` por `podman`:

| Docker | Podman |
|--------|--------|
| `docker run` | `podman run` |
| `docker ps` | `podman ps` |
| `docker images` | `podman images` |
| `docker build` | `podman build` |
| `docker-compose up` | `podman-compose up` |
| `docker-compose down` | `podman-compose down` |

### 2. Arquivos de Composição

**Antes (Docker):**
```bash
docker-compose up -d
docker-compose -f docker-compose.production.yml up -d
```

**Depois (Podman):**
```bash
podman-compose up -d
podman-compose -f podman-compose.production.yml up -d
```

### 3. Arquivos no Repositório

Os seguintes arquivos foram atualizados:

| Arquivo Original | Novo Arquivo | Status |
|-----------------|--------------|--------|
| `docker-compose.yml` | `podman-compose.yml` | ✅ Criado |
| `docker-compose.production.yml` | `podman-compose.production.yml` | ✅ Criado |
| `DOCKER_POSTGRES_SETUP.md` | `PODMAN_POSTGRES_SETUP.md` | ✅ Criado |

**Nota:** Os arquivos originais do Docker foram mantidos para compatibilidade, mas agora usamos os arquivos `podman-compose.*` como padrão.

## 🔄 Passo a Passo de Migração

### Para Ambiente de Desenvolvimento Local

#### 1. Parar containers Docker existentes
```bash
# Parar e remover containers Docker
docker-compose down -v

# Verificar que não há containers rodando
docker ps -a
```

#### 2. Instalar Podman
Siga as instruções de instalação acima para seu sistema operacional.

#### 3. Migrar volumes de dados (opcional)
Se você tem dados importantes no Docker, pode exportá-los:

```bash
# Backup do banco de dados PostgreSQL
docker-compose up postgres -d
docker-compose exec postgres pg_dump -U postgres medicwarehouse > backup_pre_migration.sql
docker-compose down

# Restaurar no Podman (após iniciar)
podman-compose up postgres -d
podman-compose exec postgres psql -U postgres medicwarehouse < backup_pre_migration.sql
```

#### 4. Iniciar com Podman
```bash
# Navegar para o diretório do projeto
cd /caminho/para/MW.Code

# Iniciar serviços com Podman
podman-compose up -d

# Verificar status
podman-compose ps

# Ver logs
podman-compose logs -f
```

#### 5. Aplicar migrations
```bash
# Aplicar migrations no banco
podman-compose exec api dotnet ef database update --context MedicSoftDbContext

# Ou localmente
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository
```

#### 6. Popular dados de teste
```bash
# Via API
curl -X POST http://localhost:5000/api/data-seeder/seed-demo

# Verificar que está funcionando
curl http://localhost:5000/health
```

### Para Ambiente de Produção (VPS)

#### 1. Backup completo
```bash
# Backup do banco de dados
docker exec medicwarehouse-postgres pg_dump -U medicwarehouse medicwarehouse > backup_production.sql

# Backup de volumes
docker run --rm -v mwcode_postgres_data:/data -v $(pwd):/backup ubuntu tar czf /backup/postgres_data_backup.tar.gz /data
```

#### 2. Instalar Podman no servidor
```bash
# Em servidor Ubuntu/Debian
ssh usuario@seu-servidor
sudo apt update
sudo apt install -y podman podman-compose
```

#### 3. Transferir arquivos
```bash
# Do seu ambiente local, copiar arquivos necessários
scp podman-compose.production.yml usuario@seu-servidor:/opt/medicwarehouse/
scp .env usuario@seu-servidor:/opt/medicwarehouse/
scp backup_production.sql usuario@seu-servidor:/opt/medicwarehouse/
```

#### 4. Parar Docker e iniciar Podman
```bash
# No servidor
cd /opt/medicwarehouse

# Parar Docker
docker-compose -f docker-compose.production.yml down

# Iniciar Podman
podman-compose -f podman-compose.production.yml up -d

# Restaurar backup
podman-compose exec postgres psql -U medicwarehouse medicwarehouse < backup_production.sql

# Verificar saúde
podman-compose ps
podman-compose logs -f
```

#### 5. Configurar systemd (opcional)
Para iniciar automaticamente com o sistema:

```bash
# Gerar serviço systemd
cd /opt/medicwarehouse
podman-compose -f podman-compose.production.yml systemd

# Habilitar serviço
sudo systemctl enable podman-compose@medicwarehouse
sudo systemctl start podman-compose@medicwarehouse
```

## 🔍 Verificação Pós-Migração

### Checklist de Validação

- [ ] Podman instalado e funcionando (`podman --version`)
- [ ] Podman Compose instalado (`podman-compose --version`)
- [ ] Containers rodando (`podman-compose ps`)
- [ ] PostgreSQL acessível (`podman-compose exec postgres psql -U postgres -d medicwarehouse -c "SELECT 1"`)
- [ ] API respondendo (`curl http://localhost:5000/health`)
- [ ] Frontend acessível (http://localhost:4200)
- [ ] System Admin acessível (http://localhost:4201)
- [ ] Swagger documentação acessível (http://localhost:5000/swagger)
- [ ] Dados preservados/restaurados
- [ ] Logs sem erros críticos (`podman-compose logs`)

### Comandos de Diagnóstico

```bash
# Verificar containers
podman-compose ps

# Verificar logs
podman-compose logs -f api
podman-compose logs -f postgres

# Verificar recursos
podman stats

# Verificar volumes
podman volume ls

# Verificar redes
podman network ls

# Entrar em um container
podman-compose exec api /bin/bash
podman-compose exec postgres psql -U postgres medicwarehouse
```

## ❓ Troubleshooting

### Problema: "podman-compose: command not found"

**Solução:**
```bash
# Instalar podman-compose via pip
pip3 install podman-compose

# Ou via package manager
sudo apt install podman-compose  # Ubuntu/Debian
sudo dnf install podman-compose  # Fedora/RHEL
```

### Problema: "permission denied" ao rodar Podman

**Solução 1 - Adicionar usuário ao grupo:**
```bash
# Adicionar usuário ao grupo podman (se existir)
sudo usermod -aG podman $USER

# Fazer logout e login novamente
```

**Solução 2 - Rodar rootless:**
```bash
# Configurar subuid e subgid
echo "$USER:100000:65536" | sudo tee -a /etc/subuid
echo "$USER:100000:65536" | sudo tee -a /etc/subgid

# Reiniciar sessão
podman system migrate
```

### Problema: Porta já em uso

**Solução:**
```bash
# Verificar o que está usando a porta
sudo lsof -i :5432
sudo lsof -i :5000
sudo lsof -i :4200

# Parar Docker se ainda estiver rodando
docker-compose down

# Parar PostgreSQL local
sudo systemctl stop postgresql
```

### Problema: Container não consegue resolver DNS

**Solução:**
```bash
# Editar /etc/containers/containers.conf
sudo nano /etc/containers/containers.conf

# Adicionar:
[network]
dns_servers = ["8.8.8.8", "8.8.4.4"]

# Reiniciar containers
podman-compose down
podman-compose up -d
```

### Problema: Volumes não persistem dados

**Solução:**
```bash
# Listar volumes
podman volume ls

# Inspecionar volume
podman volume inspect mwcode_postgres_data

# Recriar volume se necessário
podman volume rm mwcode_postgres_data
podman volume create mwcode_postgres_data
```

## 📚 Recursos Adicionais

### Documentação Oficial
- [Podman Official Documentation](https://docs.podman.io/)
- [Podman Compose GitHub](https://github.com/containers/podman-compose)
- [Podman vs Docker Comparison](https://docs.podman.io/en/latest/Introduction.html)

### Guias do MedicWarehouse
- [PODMAN_POSTGRES_SETUP.md](PODMAN_POSTGRES_SETUP.md) - Setup completo do PostgreSQL com Podman
- [QUICK_START_PRODUCTION.md](QUICK_START_PRODUCTION.md) - Deploy em produção
- [README.md](README.md) - Documentação principal do projeto

### Comandos Úteis Podman

```bash
# Ver ajuda
podman --help
podman-compose --help

# Ver info do sistema
podman info

# Limpar recursos não usados
podman system prune -a

# Ver uso de disco
podman system df

# Exportar container
podman save -o image.tar postgres:16-alpine

# Importar container
podman load -i image.tar

# Logs de sistema
journalctl -xe | grep podman
```

## ✅ Checklist Final

Após completar a migração:

- [ ] Remover Docker Desktop (opcional)
- [ ] Atualizar documentação do projeto
- [ ] Atualizar scripts de CI/CD se necessário
- [ ] Treinar equipe nos novos comandos
- [ ] Atualizar README.md com instruções do Podman
- [ ] Configurar backups regulares
- [ ] Monitorar performance
- [ ] Documentar quaisquer problemas encontrados

## 🎉 Próximos Passos

1. ✅ Migração concluída com sucesso
2. 📖 Revisar [PODMAN_POSTGRES_SETUP.md](PODMAN_POSTGRES_SETUP.md)
3. 🚀 Conferir [QUICK_START_PRODUCTION.md](QUICK_START_PRODUCTION.md) para deploy
4. 💾 Configurar backups automáticos
5. 📊 Monitorar logs e performance

---

**Versão**: 1.0  
**Data**: Novembro 2024  
**Autor**: GitHub Copilot  
**Status**: ✅ Migração Docker → Podman completa
