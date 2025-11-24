# 🚀 Guia de Início Rápido - Executar MedicWarehouse Localmente

> **Objetivo:** Colocar o sistema MedicWarehouse rodando em seu PC para testes completos em menos de 10 minutos!

> 🌍 **NOVO!** Está usando **macOS** ou **Windows**? Use nossos scripts automatizados!
> - **macOS**: Execute `./setup-macos.sh`
> - **Windows**: Execute `.\setup-windows.ps1` (PowerShell como Administrador)
> - **[Guia Completo Multiplataforma](GUIA_MULTIPLATAFORMA.md)**: Instruções detalhadas para cada plataforma

## ⚡ Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- ✅ **Podman** (recomendado - livre e open-source) **ou Docker** (para PostgreSQL)
  - **Podman (Linux)**: `sudo apt install podman podman-compose` ou `sudo dnf install podman podman-compose`
  - **Podman (macOS)**: `brew install podman podman-compose` ou use `./setup-macos.sh`
  - **Podman (Windows)**: [Podman Desktop](https://podman-desktop.io/) ou use `.\setup-windows.ps1`
  - **Docker (alternativa)**: [Download Docker Desktop](https://www.docker.com/products/docker-desktop)
- ✅ **.NET 8 SDK** (para API)
  - [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
  - Ou use scripts: `./setup-macos.sh` (macOS) ou `.\setup-windows.ps1` (Windows)
- ✅ **Node.js 18+** (para frontend)
  - [Download Node.js](https://nodejs.org/)
  - Ou use scripts: `./setup-macos.sh` (macOS) ou `.\setup-windows.ps1` (Windows)
- ✅ **Git** (já deve estar instalado)

## 📋 Passo a Passo

### 1️⃣ Clone o Repositório (se ainda não clonou)

```bash
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code
```

### 2️⃣ Iniciar o Banco de Dados PostgreSQL

**Com Podman (recomendado):**
```bash
# Iniciar apenas o PostgreSQL via Podman
podman-compose up postgres -d

# Verificar se está rodando
podman-compose ps
```

**Com Docker (alternativa):**
```bash
# Iniciar apenas o PostgreSQL via Docker
docker-compose up postgres -d

# Verificar se está rodando
docker-compose ps
```

**Aguarde ~10 segundos** para o PostgreSQL inicializar completamente.

### 3️⃣ Aplicar Migrations do Banco de Dados

```bash
# Navegar para a API
cd src/MedicSoft.Api

# Aplicar migrations
dotnet ef database update --context MedicSoftDbContext --project ../MedicSoft.Repository

# Voltar para raiz
cd ../..
```

### 4️⃣ Iniciar a API (Backend)

```bash
# Restaurar pacotes (primeira vez)
dotnet restore

# Executar a API
cd src/MedicSoft.Api
dotnet run
```

A API estará disponível em:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000/swagger

**✅ Deixe este terminal aberto!**

### 5️⃣ Popular o Banco com Dados Demo

Abra um **novo terminal** e execute:

```bash
# Popular dados de demonstração
curl -X POST http://localhost:5000/api/data-seeder/seed-demo

# OU use o Swagger:
# 1. Abra http://localhost:5000/swagger
# 2. Encontre POST /api/data-seeder/seed-demo
# 3. Clique em "Try it out" -> "Execute"
```

**Isso criará:**
- ✅ 1 clínica demo
- ✅ 4 usuários (owner, admin, médico, recepcionista)
- ✅ 6 pacientes (incluindo 2 crianças)
- ✅ 8 procedimentos
- ✅ 5 agendamentos
- ✅ Prontuários, prescrições, pagamentos, etc.

### 6️⃣ Iniciar o Frontend (Aplicativo Principal)

Abra um **novo terminal**:

```bash
# Navegar para o frontend
cd frontend/medicwarehouse-app

# Instalar dependências (primeira vez)
npm install

# Iniciar aplicativo
npm start
```

O frontend estará disponível em: **http://localhost:4200**

### 7️⃣ (Opcional) Iniciar o System Admin

Abra um **novo terminal**:

```bash
# Navegar para o system admin
cd frontend/mw-system-admin

# Instalar dependências (primeira vez)
npm install

# Iniciar aplicativo
npm start
```

O system admin estará disponível em: **http://localhost:4201**

## 🔐 Credenciais de Acesso

Use estas credenciais para fazer login:

### Proprietário da Clínica (Owner)
- **Username:** `owner.demo`
- **Password:** `Owner@123`
- **Tenant ID:** `demo-clinic-001`
- **Endpoint:** `POST /api/auth/owner-login`

### Administrador do Sistema
- **Username:** `admin`
- **Password:** `Admin@123`
- **Tenant ID:** `demo-clinic-001`
- **Endpoint:** `POST /api/auth/login`

### Médico
- **Username:** `dr.silva`
- **Password:** `Doctor@123`
- **Tenant ID:** `demo-clinic-001`
- **Endpoint:** `POST /api/auth/login`

### Recepcionista
- **Username:** `recep.maria`
- **Password:** `Recep@123`
- **Tenant ID:** `demo-clinic-001`
- **Endpoint:** `POST /api/auth/login`

## 🧪 Testar a API

### Via Swagger UI (Recomendado)

1. Abra http://localhost:5000/swagger
2. Teste o endpoint de login:
   - Endpoint: `POST /api/auth/login`
   - Body:
     ```json
     {
       "username": "admin",
       "password": "Admin@123",
       "tenantId": "demo-clinic-001"
     }
     ```
3. Copie o `token` da resposta
4. Clique no botão "Authorize" no topo
5. Cole o token no formato: `Bearer SEU_TOKEN_AQUI`
6. Teste outros endpoints!

### Via cURL

```bash
# 1. Fazer login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }'

# 2. Copie o token da resposta e use em outras chamadas
TOKEN="seu-token-aqui"

# 3. Listar pacientes
curl -X GET http://localhost:5000/api/patients \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: demo-clinic-001"

# 4. Listar agendamentos
curl -X GET http://localhost:5000/api/appointments \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: demo-clinic-001"
```

### Via Postman

1. Importe a coleção: `MedicWarehouse-Postman-Collection.json`
2. Configure as variáveis:
   - `base_url`: `http://localhost:5000`
   - `tenant_id`: `demo-clinic-001`
3. Execute o request de login
4. O token será salvo automaticamente
5. Teste outros endpoints!

## 📊 Verificar os Dados Demo

### Informações sobre os dados criados

```bash
curl -X GET http://localhost:5000/api/data-seeder/demo-info
```

### Dados incluídos:

- **Clínica:** Clínica Demo MedicWarehouse (TenantId: `demo-clinic-001`)
- **Pacientes:** Carlos, Ana Maria, Pedro, Juliana, Lucas (criança), Sofia (criança)
- **Procedimentos:** Consulta Geral, Cardiologia, Exames, Vacinas, etc.
- **Agendamentos:** 5 agendamentos (passados, hoje e futuros)
- **Prontuários:** 2 prontuários completos com prescrições
- **Medicamentos:** 8 medicamentos diversos
- **Notificações:** 5 notificações em diferentes estados
- **Despesas:** 10 despesas (pagas, pendentes, vencidas)
- **Exames:** 5 solicitações de exames

## 🧹 Limpar e Reiniciar Dados

Se quiser recomeçar do zero:

```bash
# Limpar todos os dados
curl -X DELETE http://localhost:5000/api/data-seeder/clear-database

# Popular novamente
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

## ❌ Troubleshooting

### Erro: "Demo data already exists"
**Solução:** Os dados já foram criados. Use o endpoint de clear acima ou delete manualmente.

### Erro: "Connection refused" ao iniciar API
**Solução:** Verifique se o PostgreSQL está rodando: `docker compose ps`

### Erro: "Database does not exist"
**Solução:** Execute as migrations: `dotnet ef database update`

### Erro no frontend: "Cannot connect to API"
**Solução:** 
1. Verifique se a API está rodando em http://localhost:5000
2. Verifique o arquivo `environment.ts` do frontend
3. Certifique-se que CORS está habilitado para localhost:4200

### Porta 5432 já está em uso
**Solução:** Você já tem PostgreSQL rodando localmente. Opções:
1. Pare o PostgreSQL local: `sudo systemctl stop postgresql` (Linux)
2. Mude a porta no docker-compose.yml: `"5433:5432"`
3. Atualize a connection string na API

### Frontend não carrega após npm start
**Solução:**
1. Limpe cache: `rm -rf node_modules package-lock.json`
2. Reinstale: `npm install`
3. Tente: `npm start -- --host 0.0.0.0`

## 🎯 Fluxos de Teste Recomendados

### 1. Fluxo de Login e Dashboard
1. Abra http://localhost:4200
2. Faça login com `admin` / `Admin@123` / `demo-clinic-001`
3. Explore o dashboard

### 2. Fluxo de Agendamento
1. Navegue para "Agenda"
2. Veja os agendamentos existentes
3. Crie um novo agendamento
4. Confirme ou cancele um agendamento

### 3. Fluxo de Atendimento
1. Veja um agendamento para hoje
2. Clique em "Iniciar Atendimento"
3. Preencha o prontuário médico
4. Adicione prescrições
5. Finalize o atendimento

### 4. Fluxo de Pacientes
1. Navegue para "Pacientes"
2. Busque pacientes por nome, CPF ou telefone
3. Veja o histórico de um paciente
4. Crie um novo paciente

### 5. Fluxo Financeiro
1. Navegue para "Financeiro"
2. Veja receitas e despesas
3. Analise relatórios
4. Gerencie pagamentos

## 📚 Documentação Adicional

- **Autenticação:** [AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md)
- **API Completa:** [README.md](README.md)
- **Seeders:** [SEEDER_GUIDE.md](SEEDER_GUIDE.md)
- **Postman:** [POSTMAN_IMPORT_GUIDE.md](POSTMAN_IMPORT_GUIDE.md)
- **Pendências:** [PENDING_TASKS.md](PENDING_TASKS.md)

## 🆘 Precisa de Ajuda?

1. Verifique a documentação no diretório raiz
2. Veja os logs da API no terminal
3. Use o Swagger para testar endpoints
4. Consulte o SEEDER_GUIDE.md para detalhes dos dados

## ✅ Checklist de Verificação

Antes de começar os testes, certifique-se de que:

- [ ] PostgreSQL está rodando (porta 5432)
- [ ] Migrations foram aplicadas
- [ ] API está rodando (http://localhost:5000/swagger abre)
- [ ] Dados demo foram populados (sem erro)
- [ ] Frontend carrega (http://localhost:4200 abre)
- [ ] Você consegue fazer login
- [ ] Token JWT está sendo gerado

**Pronto! Agora você tem o MedicWarehouse rodando localmente com dados completos para teste! 🎉**

---

**Última Atualização:** Novembro 2024  
**Versão:** 1.0
