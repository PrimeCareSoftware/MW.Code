# 05 - Configuração Docker/Podman (Opcional)

> **Objetivo:** Executar o sistema completo usando containers  
> **Tempo estimado:** 15-20 minutos  
> **Pré-requisitos:** Docker ou Podman instalado

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Escolher Docker ou Podman](#escolher-docker-ou-podman)
3. [Configuração com Docker](#configuração-com-docker)
4. [Configuração com Podman](#configuração-com-podman)
5. [Serviços Disponíveis](#serviços-disponíveis)
6. [Comandos Úteis](#comandos-úteis)
7. [Troubleshooting](#troubleshooting)

## 🐳 Visão Geral

O PrimeCare Software pode ser executado completamente em containers, facilitando:
- ✅ Setup rápido (1 comando)
- ✅ Ambiente isolado
- ✅ Consistência entre ambientes
- ✅ Fácil deploy
- ✅ Não afeta sua máquina local

### Arquitetura em Containers

```
┌─────────────────────────────────────────────┐
│           Docker/Podman Network              │
├─────────────────────────────────────────────┤
│                                              │
│  ┌──────────────┐  ┌──────────────┐         │
│  │   Frontend   │  │   Backend    │         │
│  │  (Angular)   │  │   (.NET 8)   │         │
│  │  Port: 4200  │  │  Port: 5000  │         │
│  └──────────────┘  └──────────────┘         │
│         │                   │                │
│         │                   │                │
│  ┌──────────────┐  ┌──────────────┐         │
│  │ PostgreSQL   │  │    Redis     │         │
│  │  Port: 5432  │  │  Port: 6379  │         │
│  └──────────────┘  └──────────────┘         │
│                                              │
│  ┌──────────────┐  ┌──────────────┐         │
│  │   Seq Logs   │  │   Swagger    │         │
│  │  Port: 5341  │  │    (API)     │         │
│  └──────────────┘  └──────────────┘         │
│                                              │
└─────────────────────────────────────────────┘
```

## 🆚 Escolher Docker ou Podman

### Docker
- ✅ Mais popular e com mais documentação
- ✅ Docker Desktop inclui interface gráfica
- ❌ Requer licença paga para uso comercial (empresas 250+)
- ❌ Requer daemon rodando (mais recursos)

### Podman (Recomendado)
- ✅ 100% gratuito e open-source
- ✅ Não requer daemon (mais leve)
- ✅ Compatível com Docker (comandos similares)
- ✅ Mais seguro (rootless por padrão)
- ⚠️ Menos documentação disponível

**Nossa recomendação:** Use Podman para economia e segurança.

## 🐳 Configuração com Docker

### 1. Instalar Docker

#### Windows/macOS
Baixe e instale Docker Desktop:
- https://www.docker.com/products/docker-desktop

#### Linux (Ubuntu/Debian)
```bash
# Instalar Docker
sudo apt update
sudo apt install -y docker.io docker-compose

# Adicionar seu usuário ao grupo docker
sudo usermod -aG docker $USER

# Relogar ou executar:
newgrp docker
```

### 2. Verificar Instalação

```bash
docker --version
# Esperado: Docker version 20.x+

docker-compose --version
# Esperado: docker-compose version 1.29.x+
```

### 3. Executar com Docker

```bash
cd MW.Code

# Executar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### 4. Configuração Avançada

#### docker-compose.yml (já incluído no projeto)

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: medicsoft_dev
      POSTGRES_USER: medicsoft_user
      POSTGRES_PASSWORD: medicsoft_pass
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U medicsoft_user"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  backend:
    build:
      context: .
      dockerfile: src/MedicSoft.Api/Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=medicsoft_dev;Username=medicsoft_user;Password=medicsoft_pass
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_started

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - backend

volumes:
  postgres_data:
  redis_data:
```

### 5. Build Personalizado

```bash
# Build apenas backend
docker-compose build backend

# Build apenas frontend
docker-compose build frontend

# Build tudo
docker-compose build
```

## 🦭 Configuração com Podman

### 1. Instalar Podman

#### macOS
```bash
brew install podman
podman machine init
podman machine start
```

#### Linux (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install -y podman podman-compose
```

#### Linux (Fedora)
```bash
sudo dnf install -y podman podman-compose
```

#### Windows
Baixe e instale Podman Desktop:
- https://podman-desktop.io/

### 2. Verificar Instalação

```bash
podman --version
# Esperado: podman version 4.x+

podman-compose --version
# Esperado: podman-compose version 1.x+
```

### 3. Executar com Podman

```bash
cd MW.Code

# Executar todos os serviços
podman-compose up -d

# Ver logs
podman-compose logs -f

# Parar serviços
podman-compose down
```

### 4. Migrar de Docker para Podman

O PrimeCare já inclui arquivos específicos para Podman:

```bash
# Usar configuração Podman
podman-compose -f podman-compose.yml up -d

# Ou usar a versão de produção
podman-compose -f podman-compose.production.yml up -d
```

## 🔧 Serviços Disponíveis

Após executar `docker-compose up -d` ou `podman-compose up -d`:

| Serviço | URL | Descrição |
|---------|-----|-----------|
| Frontend | http://localhost:4200 | Aplicação Angular |
| Backend API | http://localhost:5000 | API .NET 8 |
| Swagger UI | http://localhost:5000/swagger | Documentação API |
| PostgreSQL | localhost:5432 | Banco de dados |
| Redis | localhost:6379 | Cache e sessões |
| Seq Logs | http://localhost:5341 | Visualizador de logs |
| System Admin | http://localhost:3000 | Admin SPA |
| Patient Portal | http://localhost:5100 | Portal do Paciente |

### Credenciais Padrão

**PostgreSQL:**
- Host: localhost
- Port: 5432
- Database: medicsoft_dev
- Username: medicsoft_user
- Password: medicsoft_pass

**Admin do Sistema:**
- Email: admin@demo.com
- Senha: Admin@123

## 📝 Comandos Úteis

### Docker/Podman Compose

```bash
# Ver status dos containers
docker-compose ps
# ou
podman-compose ps

# Ver logs de um serviço específico
docker-compose logs backend
docker-compose logs -f frontend

# Reiniciar um serviço
docker-compose restart backend

# Parar um serviço específico
docker-compose stop frontend

# Remover tudo (incluindo volumes)
docker-compose down -v

# Executar comando dentro do container
docker-compose exec backend bash
docker-compose exec postgres psql -U medicsoft_user -d medicsoft_dev
```

### Docker/Podman (comandos diretos)

```bash
# Listar containers rodando
docker ps
# ou
podman ps

# Listar todas as imagens
docker images
podman images

# Ver logs de um container
docker logs <container_id>
podman logs <container_id>

# Executar comando dentro do container
docker exec -it <container_id> bash
podman exec -it <container_id> bash

# Parar todos os containers
docker stop $(docker ps -q)
podman stop $(podman ps -q)

# Remover containers parados
docker container prune
podman container prune

# Remover imagens não usadas
docker image prune -a
podman image prune -a
```

### Gerenciamento de Volumes

```bash
# Listar volumes
docker volume ls
podman volume ls

# Inspecionar volume
docker volume inspect postgres_data
podman volume inspect postgres_data

# Remover volume
docker volume rm postgres_data
podman volume rm postgres_data

# Backup de volume
docker run --rm -v postgres_data:/data -v $(pwd):/backup alpine tar czf /backup/postgres_backup.tar.gz /data
```

## 🔍 Verificação

### 1. Verificar Containers Rodando

```bash
docker-compose ps
# ou
podman-compose ps

# Todos os serviços devem estar "Up"
```

### 2. Verificar Saúde dos Serviços

```bash
# Health check do PostgreSQL
docker-compose exec postgres pg_isready

# Health check da API
curl http://localhost:5000/health

# Resposta esperada:
# {"status":"Healthy"}
```

### 3. Verificar Logs

```bash
# Ver logs de todos os serviços
docker-compose logs

# Ver últimas 50 linhas
docker-compose logs --tail=50

# Seguir logs em tempo real
docker-compose logs -f
```

### Checklist de Verificação

- [ ] Docker ou Podman instalado
- [ ] Docker Compose ou Podman Compose instalado
- [ ] Containers iniciados com sucesso
- [ ] Frontend acessível em http://localhost:4200
- [ ] Backend acessível em http://localhost:5000
- [ ] Swagger acessível em http://localhost:5000/swagger
- [ ] PostgreSQL acessível na porta 5432
- [ ] Sem erros críticos nos logs

## 🚨 Troubleshooting

### Problema: Porta já está em uso

```bash
# Encontrar processo usando a porta
# macOS/Linux
lsof -i :5432
kill -9 <PID>

# Windows
netstat -ano | findstr :5432
taskkill /PID <PID> /F

# Ou mudar a porta no docker-compose.yml
ports:
  - "5433:5432"  # Porta 5433 no host
```

### Problema: Container não inicia

```bash
# Ver logs detalhados
docker-compose logs backend

# Verificar se há erro de build
docker-compose build --no-cache backend

# Remover e recriar
docker-compose down
docker-compose up -d
```

### Problema: "No space left on device"

```bash
# Limpar tudo que não está em uso
docker system prune -a --volumes

# Ou com Podman
podman system prune -a --volumes
```

### Problema: Backend não conecta ao PostgreSQL

**Solução:** Verifique a string de conexão. Use o nome do serviço como host:

```yaml
ConnectionStrings__DefaultConnection: "Host=postgres;Database=medicsoft_dev;..."
```

Não use `localhost` dentro dos containers!

### Problema: Containers muito lentos (Windows/macOS)

**Solução:** Configure mais recursos no Docker Desktop:
- Settings > Resources
- Aumente CPU e Memória
- Recomendado: 4 CPUs, 8 GB RAM

### Problema: "permission denied" ao executar docker-compose

```bash
# Linux - adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
newgrp docker

# Ou executar com sudo (não recomendado)
sudo docker-compose up -d
```

## 🎯 Ambientes Diferentes

### Desenvolvimento (Development)

```bash
docker-compose -f docker-compose.yml up -d
```

### Produção (Production)

```bash
docker-compose -f docker-compose.production.yml up -d
```

### Microserviços (Completo)

```bash
docker-compose -f docker-compose.microservices.yml up -d
```

### Observabilidade (ELK Stack)

```bash
docker-compose -f docker-compose.elk.yml up -d
```

## 📚 Documentação Adicional

- [Docker to Podman Migration](../../system-admin/infrastructure/DOCKER_TO_PODMAN_MIGRATION.md)
- [Deploy Hostinger Guide](../../DEPLOY_HOSTINGER_MULTIPLOS_PROJETOS.md)
- [Docker Official Docs](https://docs.docker.com/)
- [Podman Official Docs](https://docs.podman.io/)

## ⏭️ Próximos Passos

Agora que o sistema está rodando em containers:

1. ✅ Containers configurados e rodando
2. ✅ Sistema totalmente funcional
3. ➡️ Vá para os [cenários de testes](../CenariosTestesQA/) para começar os testes de QA

**Sistema pronto para uso! 🎉**

Acesse http://localhost:4200 e faça login com:
- **Email:** admin@demo.com
- **Senha:** Admin@123

---

**Dúvidas?** Verifique os logs com `docker-compose logs -f` ou `podman-compose logs -f`
