# MedicWarehouse - Sistema de Gestão para Consultórios Médicos

[![CI - Test Frontend e Backend](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml/badge.svg)](https://github.com/MedicWarehouse/MW.Code/actions/workflows/ci.yml)

Uma solução **DDD** multitenant completa para gestão de consultórios médicos (SaaS) construída com **Angular**, **.NET 8** e **PostgreSQL**.

## 🏗️ Arquitetura

O projeto segue os princípios do Domain-Driven Design (DDD) com arquitetura em camadas:

- **MedicSoft.Domain**: Entidades, Value Objects, Domain Services e Events
- **MedicSoft.Application**: CQRS com Commands/Queries, DTOs e Application Services  
- **MedicSoft.Repository**: Implementação do repositório com Entity Framework Core
- **MedicSoft.Api**: API RESTful com autenticação JWT e Swagger
- **MedicSoft.CrossCutting**: Serviços transversais (logging, segurança, etc.)
- **MedicSoft.Test**: Testes unitários e de integração

## 🚀 Funcionalidades

### 🏥 Gestão Clínica
- ✅ **Multitenant**: Isolamento de dados por consultório
- ✅ **Vínculo Multi-Clínica**: Paciente pode estar vinculado a múltiplas clínicas (N:N)
- ✅ **Busca Inteligente**: Busca de pacientes por CPF, Nome ou Telefone
- ✅ **Reutilização de Cadastro**: Sistema detecta cadastro prévio e vincula à nova clínica
- ✅ **Privacidade de Prontuários**: Cada clínica acessa apenas seus próprios prontuários
- ✅ **Templates**: Templates reutilizáveis para prontuários e prescrições médicas

### 💊 Medicamentos e Prescrições
- ✅ **Cadastro de Medicamentos**: Base completa com classificação ANVISA
- ✅ **Autocomplete**: Busca inteligente de medicamentos ao prescrever
- ✅ **Itens de Prescrição**: Vínculo de medicamentos com dosagem, frequência e duração
- ✅ **Medicamentos Controlados**: Identificação de substâncias controladas (Portaria 344/98)
- ✅ **Categorias**: Analgésico, Antibiótico, Anti-inflamatório, etc.

### 📅 Agendamentos e Atendimento
- ✅ **CRUD de Pacientes**: Cadastro completo com validações
- ✅ **Agendamento de Consultas**: Sistema completo de agendamentos
- ✅ **Agenda Diária**: Visualização da agenda com slots disponíveis
- ✅ **Visualização em Calendário**: Navegação mensal com indicadores
- ✅ **Atendimento ao Paciente**: Tela completa de atendimento com prontuário
- ✅ **Timer de Consulta**: Cronômetro automático para controle do tempo
- ✅ **Prontuário Médico**: Registro de diagnóstico, prescrição e observações
- ✅ **Histórico do Paciente**: Timeline de consultas anteriores
- ✅ **Prescrição Médica**: Área de texto com impressão otimizada
- ✅ **Encaixes**: Permite agendamentos de emergência

### 💳 Assinaturas e Cobrança
- ✅ **Período de Teste**: 15 dias gratuitos para novas clínicas
- ✅ **Planos Flexíveis**: Trial, Basic, Standard, Premium, Enterprise
- ✅ **Gestão de Assinaturas**: Ativação, suspensão, cancelamento
- ✅ **Controle de Pagamentos**: Registro de pagamentos e renovações
- ✅ **Status de Assinatura**: Trial, Active, Suspended, PaymentOverdue, Cancelled

### 📲 Notificações
- ✅ **SMS**: Integração preparada para envio de SMS
- ✅ **WhatsApp**: Interface para WhatsApp Business API
- ✅ **Lembretes Automáticos**: Confirmação de agendamento 24h antes
- ✅ **Retry Logic**: Até 3 tentativas para notificações falhadas
- ✅ **Múltiplos Canais**: SMS, WhatsApp, Email, Push

### 🏥 Procedimentos e Serviços
- ✅ **Cadastro de Procedimentos**: Nome, código, categoria, preço, duração
- ✅ **Gestão de Materiais**: Controle de estoque com entrada e saída
- ✅ **Vínculo Procedimento-Consulta**: Registro completo por atendimento
- ✅ **Controle de Estoque**: Alerta de estoque mínimo
- ✅ **Categorias**: Consulta, Exame, Cirurgia, Terapia, Vacinação, etc.

### 🔐 Segurança e Administração
- ✅ **Autenticação JWT**: API segura com tokens JWT
- ✅ **Painel do Dono da Clínica**: Gestão completa de usuários e configurações
- ✅ **Painel do Sistema**: Administração master para dono do sistema
- ✅ **Gestão de Permissões**: Controle granular de acesso
- ✅ **Auditoria**: Log completo de operações

### 📊 Relatórios e Integrações
- ✅ **Swagger**: Documentação interativa da API
- ✅ **Docker**: Containerização completa
- 🚧 **TISS Export**: Integração com padrão TISS (em planejamento)
- 🚧 **Relatórios BI**: Dashboards e analytics (em planejamento)

## 🔧 Tecnologias

- **Backend**: .NET 8, Entity Framework Core, SQL Server
- **Frontend**: Angular 18, TypeScript, SCSS
- **Banco de Dados**: SQL Server 2022 (via Docker)
- **Containerização**: Docker e Docker Compose
- **Autenticação**: JWT Bearer Tokens

## 🏃‍♂️ Como Executar

> 📖 **Para um guia completo e detalhado passo a passo**, consulte o arquivo [GUIA_EXECUCAO.md](GUIA_EXECUCAO.md)

### Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento)
- Node.js 18+ (para desenvolvimento)

### Executar com Docker

```bash
# Clone o repositório
git clone https://github.com/MedicWarehouse/MW.Code.git
cd MW.Code

# Execute com Docker Compose
docker-compose up -d

# A API estará disponível em: http://localhost:5000
# O Frontend estará disponível em: http://localhost:4200
# Swagger UI estará disponível em: http://localhost:5000
```

### Executar para Desenvolvimento

#### Backend (.NET API)

```bash
# Restaurar dependências
dotnet restore

# Executar a API
cd src/MedicSoft.Api
dotnet run

# A API estará disponível em: https://localhost:7100
```

#### Frontend (Angular)

```bash
# Navegar para o frontend
cd frontend/medicwarehouse-app

# Instalar dependências
npm install

# Executar em modo de desenvolvimento
ng serve

# O frontend estará disponível em: http://localhost:4200
```

#### Banco de Dados (SQL Server)

```bash
# Executar apenas o SQL Server
docker run -d \
  --name medicwarehouse-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=MedicW@rehouse2024!" \
  -e "MSSQL_PID=Developer" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest

# Criar o banco de dados
docker exec -it medicwarehouse-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "MedicW@rehouse2024!" \
  -Q "CREATE DATABASE MedicWarehouse;"
```

## 📖 Documentação da API

Após executar a aplicação, acesse a documentação interativa do Swagger:

- **Swagger UI**: http://localhost:5000

### Autenticação

Para testar a API, primeiro obtenha um token JWT:

```bash
POST /api/auth/login
{
  "username": "admin",
  "password": "admin123",
  "tenantId": "default-tenant"
}
```

Use o token retornado no header `Authorization: Bearer {token}` nas demais requisições.

### Endpoints Principais

- **Autenticação**:
  - `POST /api/auth/login` - Login e obtenção do token
  - `GET /api/auth/me` - Informações do usuário atual

- **Pacientes**:
  - `GET /api/patients` - Listar pacientes
  - `GET /api/patients/{id}` - Obter paciente por ID
  - `GET /api/patients/search?searchTerm={termo}` - Buscar por CPF, Nome ou Telefone
  - `GET /api/patients/by-document/{cpf}` - Buscar por CPF em todas as clínicas
  - `POST /api/patients` - Criar novo paciente
  - `PUT /api/patients/{id}` - Atualizar paciente
  - `DELETE /api/patients/{id}` - Excluir paciente
  - `POST /api/patients/{patientId}/link-clinic/{clinicId}` - Vincular paciente à clínica

- **Agendamentos**:
  - `POST /api/appointments` - Criar agendamento
  - `GET /api/appointments/{id}` - Obter agendamento por ID
  - `PUT /api/appointments/{id}/cancel` - Cancelar agendamento
  - `GET /api/appointments/agenda` - Agenda diária
  - `GET /api/appointments/available-slots` - Horários disponíveis

- **Prontuários Médicos**:
  - `POST /api/medical-records` - Criar prontuário
  - `PUT /api/medical-records/{id}` - Atualizar prontuário
  - `POST /api/medical-records/{id}/complete` - Finalizar atendimento
  - `GET /api/medical-records/appointment/{appointmentId}` - Buscar por agendamento
  - `GET /api/medical-records/patient/{patientId}` - Histórico do paciente

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 🗃️ Estrutura do Banco de Dados

### Tabelas Principais

- **Patients**: Dados dos pacientes
- **Clinics**: Informações dos consultórios
- **PatientClinicLinks**: Vínculos N:N entre pacientes e clínicas
- **Appointments**: Agendamentos de consultas
- **MedicalRecords**: Prontuários médicos e histórico de atendimentos (isolados por clínica)
- **MedicalRecordTemplates**: Templates reutilizáveis para prontuários
- **PrescriptionTemplates**: Templates reutilizáveis para prescrições

### Multitenancy

O sistema utiliza **multitenancy** por coluna `TenantId`, garantindo isolamento de dados entre diferentes consultórios.

**Importante**: 
- Pacientes podem estar vinculados a múltiplas clínicas (N:N)
- Dados cadastrais são compartilhados entre clínicas vinculadas
- Prontuários médicos são **isolados por clínica** - cada clínica vê apenas seus próprios registros
- Sistema detecta cadastro prévio por CPF e reutiliza dados, criando novo vínculo

Para mais detalhes sobre as regras de negócio, consulte [BUSINESS_RULES.md](BUSINESS_RULES.md)

## 🔐 Segurança

- **JWT Authentication**: Autenticação baseada em tokens
- **Tenant Isolation**: Isolamento automático de dados por tenant
- **CORS Configuration**: Configuração de CORS para frontend
- **Input Validation**: Validação de dados de entrada
- **SQL Injection Protection**: Entity Framework Core com parâmetros

## 🚀 Deploy

### Usando Docker

```bash
# Build das imagens
docker-compose build

# Deploy em produção
docker-compose -f docker-compose.yml up -d
```

### Configuração de Produção

Atualize as seguintes configurações para produção:

- `appsettings.Production.json`: String de conexão e chave JWT
- `docker-compose.yml`: Variáveis de ambiente de produção
- Nginx: Configuração SSL/TLS

## 🔄 CI/CD

O projeto utiliza **GitHub Actions** para integração e entrega contínuas. O workflow executa automaticamente:

- ✅ **Testes Backend**: Executa todos os 305 testes unitários do .NET
- ✅ **Testes Frontend**: Executa testes do Angular com Karma/Jasmine
- ✅ **Build Verification**: Verifica se o build está funcional
- ✅ **Code Coverage**: Gera relatórios de cobertura de código

O workflow é executado automaticamente em:
- Push para as branches `main` e `develop`
- Pull Requests para as branches `main` e `develop`
- Execução manual via GitHub Actions

Para mais detalhes, consulte: [CI_CD_DOCUMENTATION.md](CI_CD_DOCUMENTATION.md)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Contato

- **Projeto**: MedicWarehouse
- **Email**: contato@medicwarehouse.com
- **GitHub**: [https://github.com/MedicWarehouse/MW.Code](https://github.com/MedicWarehouse/MW.Code)