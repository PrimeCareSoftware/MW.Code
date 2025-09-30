# MedicWarehouse - Sistema de Gestão para Consultórios Médicos

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

- ✅ **Multitenant**: Isolamento de dados por consultório
- ✅ **CRUD de Pacientes**: Cadastro completo com validações
- ✅ **Agendamento de Consultas**: Sistema completo de agendamentos
- ✅ **Agenda Diária**: Visualização da agenda com slots disponíveis
- ✅ **Encaixes**: Permite agendamentos de emergência
- ✅ **Autenticação JWT**: API segura com tokens JWT
- ✅ **Swagger**: Documentação interativa da API
- ✅ **Docker**: Containerização completa

## 🔧 Tecnologias

- **Backend**: .NET 8, Entity Framework Core, PostgreSQL
- **Frontend**: Angular 18, TypeScript, SCSS
- **Banco de Dados**: PostgreSQL 15
- **Containerização**: Docker e Docker Compose
- **Autenticação**: JWT Bearer Tokens

## 🏃‍♂️ Como Executar

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

#### Banco de Dados (PostgreSQL)

```bash
# Executar apenas o PostgreSQL
docker run -d \
  --name medicwarehouse-postgres \
  -e POSTGRES_DB=medicwarehouse \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -p 5432:5432 \
  postgres:15-alpine
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
  - `POST /api/patients` - Criar novo paciente
  - `PUT /api/patients/{id}` - Atualizar paciente
  - `DELETE /api/patients/{id}` - Excluir paciente

- **Agendamentos**:
  - `POST /api/appointments` - Criar agendamento
  - `PUT /api/appointments/{id}/cancel` - Cancelar agendamento
  - `GET /api/appointments/agenda` - Agenda diária
  - `GET /api/appointments/available-slots` - Horários disponíveis

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
- **Appointments**: Agendamentos de consultas

### Multitenancy

O sistema utiliza **multitenancy** por coluna `TenantId`, garantindo isolamento de dados entre diferentes consultórios.

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