# ✅ Checklist Completo de Testes - Omni Care Software

> **Objetivo:** Documento para verificar todos os métodos, APIs, front-end e fluxos do sistema Omni Care Software

## 📋 Índice

1. [Preparação do Ambiente](#preparação-do-ambiente)
2. [Testes de API - Backend](#testes-de-api---backend)
3. [Testes de Frontend - Omni Care Software App](#testes-de-frontend---medicwarehouse-app)
4. [Testes de Frontend - System Admin](#testes-de-frontend---system-admin)
5. [Testes de Integração](#testes-de-integração)
6. [Testes de Segurança](#testes-de-segurança)
7. [Testes de Performance](#testes-de-performance)

---

## 🔧 Preparação do Ambiente

### Pré-requisitos
- [ ] Podman (ou Docker) instalado e rodando
- [ ] .NET 8 SDK instalado
- [ ] Node.js 18+ instalado
- [ ] PostgreSQL rodando (via Podman/Docker)
- [ ] Migrations aplicadas
- [ ] Dados demo populados

### Verificação Inicial
```bash
# 1. PostgreSQL rodando (Podman)
podman-compose ps | grep postgres

# Ou com Docker:
# docker-compose ps | grep postgres

# 2. API buildando
dotnet build src/MedicSoft.Api/MedicSoft.Api.csproj

# 3. API rodando
curl http://localhost:5000/health

# 4. Dados demo criados
curl http://localhost:5000/api/data-seeder/demo-info
```

---

## 🔌 Testes de API - Backend

### 1. Autenticação (`/api/auth`)

#### Login de Usuários
- [ ] `POST /api/auth/login` - Login com credenciais válidas
  ```json
  {
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }
  ```
- [ ] `POST /api/auth/login` - Login com credenciais inválidas (deve falhar)
- [ ] `POST /api/auth/login` - Login sem tenant (deve falhar)

#### Login de Proprietários
- [ ] `POST /api/auth/owner-login` - Login de owner válido
  ```json
  {
    "username": "owner.demo",
    "password": "Owner@123",
    "tenantId": "demo-clinic-001"
  }
  ```
- [ ] `POST /api/auth/owner-login` - Login de system owner
  ```json
  {
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "system"
  }
  ```

#### Validação de Token
- [ ] `POST /api/auth/validate` - Token válido
- [ ] `POST /api/auth/validate` - Token expirado (deve falhar)
- [ ] `POST /api/auth/validate` - Token inválido (deve falhar)

### 2. Registro (`/api/registration`)

- [ ] `POST /api/registration` - Registrar nova clínica
- [ ] `GET /api/registration/check-cnpj/{cnpj}` - Verificar CNPJ disponível
- [ ] `GET /api/registration/check-username/{username}` - Verificar username disponível

### 3. Pacientes (`/api/patients`)

#### CRUD Básico
- [ ] `GET /api/patients` - Listar todos os pacientes
- [ ] `GET /api/patients/{id}` - Obter paciente específico
- [ ] `POST /api/patients` - Criar novo paciente adulto
- [ ] `POST /api/patients` - Criar novo paciente criança (com guardianId)
- [ ] `PUT /api/patients/{id}` - Atualizar paciente
- [ ] `DELETE /api/patients/{id}` - Deletar paciente

#### Busca e Filtros
- [ ] `GET /api/patients/search?searchTerm=Carlos` - Buscar por nome
- [ ] `GET /api/patients/search?searchTerm=123.456.789-00` - Buscar por CPF
- [ ] `GET /api/patients/search?searchTerm=(11)99999-9999` - Buscar por telefone
- [ ] `GET /api/patients/by-document/{cpf}` - Buscar por CPF em todas clínicas

#### Relacionamentos
- [ ] `POST /api/patients/{patientId}/link-clinic/{clinicId}` - Vincular paciente à clínica
- [ ] `POST /api/patients/{childId}/link-guardian/{guardianId}` - Vincular criança a responsável
- [ ] `GET /api/patients/{guardianId}/children` - Listar filhos de um responsável

### 4. Agendamentos (`/api/appointments`)

- [ ] `GET /api/appointments` - Listar agendamentos
- [ ] `GET /api/appointments/{id}` - Obter agendamento específico
- [ ] `POST /api/appointments` - Criar novo agendamento
- [ ] `PUT /api/appointments/{id}` - Atualizar agendamento
- [ ] `PUT /api/appointments/{id}/cancel` - Cancelar agendamento
- [ ] `PUT /api/appointments/{id}/confirm` - Confirmar agendamento
- [ ] `GET /api/appointments/agenda` - Ver agenda do dia
- [ ] `GET /api/appointments/available-slots` - Ver horários disponíveis

### 5. Prontuários Médicos (`/api/medical-records`)

- [ ] `GET /api/medical-records` - Listar prontuários
- [ ] `GET /api/medical-records/{id}` - Obter prontuário específico
- [ ] `POST /api/medical-records` - Criar prontuário
- [ ] `PUT /api/medical-records/{id}` - Atualizar prontuário
- [ ] `POST /api/medical-records/{id}/complete` - Finalizar atendimento
- [ ] `GET /api/medical-records/appointment/{appointmentId}` - Prontuário por agendamento
- [ ] `GET /api/medical-records/patient/{patientId}` - Histórico do paciente

### 6. Procedimentos (`/api/procedures`)

- [ ] `GET /api/procedures` - Listar procedimentos
- [ ] `GET /api/procedures/{id}` - Obter procedimento específico
- [ ] `POST /api/procedures` - Criar procedimento
- [ ] `PUT /api/procedures/{id}` - Atualizar procedimento
- [ ] `DELETE /api/procedures/{id}` - Desativar procedimento
- [ ] `POST /api/procedures/appointments/{appointmentId}/procedures` - Adicionar procedimento ao atendimento
- [ ] `GET /api/procedures/appointments/{appointmentId}/procedures` - Listar procedimentos do atendimento
- [ ] `GET /api/procedures/appointments/{appointmentId}/billing-summary` - Resumo de cobrança

### 7. Pagamentos (`/api/payments`)

- [ ] `GET /api/payments` - Listar pagamentos
- [ ] `GET /api/payments/{id}` - Obter pagamento específico
- [ ] `POST /api/payments` - Criar pagamento
- [ ] `PUT /api/payments/{id}/process` - Processar pagamento
- [ ] `PUT /api/payments/{id}/refund` - Reembolsar pagamento
- [ ] `PUT /api/payments/{id}/cancel` - Cancelar pagamento

### 8. Despesas (`/api/expenses`)

- [ ] `GET /api/expenses` - Listar despesas
- [ ] `GET /api/expenses?status=Pending` - Filtrar por status
- [ ] `GET /api/expenses?category=Rent` - Filtrar por categoria
- [ ] `GET /api/expenses/{id}` - Obter despesa específica
- [ ] `POST /api/expenses` - Criar despesa
- [ ] `PUT /api/expenses/{id}` - Atualizar despesa
- [ ] `PUT /api/expenses/{id}/pay` - Marcar como paga
- [ ] `PUT /api/expenses/{id}/cancel` - Cancelar despesa
- [ ] `DELETE /api/expenses/{id}` - Deletar despesa

### 9. Relatórios (`/api/reports`)

- [ ] `GET /api/reports/financial-summary` - Resumo financeiro completo
- [ ] `GET /api/reports/revenue` - Relatório de receita
- [ ] `GET /api/reports/appointments` - Relatório de agendamentos
- [ ] `GET /api/reports/patients` - Relatório de pacientes
- [ ] `GET /api/reports/accounts-receivable` - Contas a receber
- [ ] `GET /api/reports/accounts-payable` - Contas a pagar

### 10. Notificações (`/api/notifications`)

- [ ] `GET /api/notifications` - Listar notificações
- [ ] `GET /api/notifications/{id}` - Obter notificação específica
- [ ] `POST /api/notifications` - Criar notificação
- [ ] `PUT /api/notifications/{id}/mark-sent` - Marcar como enviada
- [ ] `PUT /api/notifications/{id}/mark-delivered` - Marcar como entregue
- [ ] `PUT /api/notifications/{id}/mark-read` - Marcar como lida

### 11. Medicamentos (`/api/medications`)

- [ ] `GET /api/medications` - Listar medicamentos
- [ ] `GET /api/medications/search?term=Amoxicilina` - Buscar medicamento
- [ ] `POST /api/medications` - Criar medicamento
- [ ] `PUT /api/medications/{id}` - Atualizar medicamento

### 12. Data Seeder (`/api/data-seeder`)

- [ ] `GET /api/data-seeder/demo-info` - Informações dos dados demo
- [ ] `POST /api/data-seeder/seed-demo` - Popular dados demo
- [ ] `POST /api/data-seeder/seed-system-owner` - Criar system owner
- [ ] `DELETE /api/data-seeder/clear-database` - Limpar banco de dados

---

## 🖥️ Testes de Frontend - Omni Care Software App

### Autenticação

- [ ] **Login**
  - [ ] Login com credenciais válidas
  - [ ] Login com credenciais inválidas (mensagem de erro)
  - [ ] Login sem preencher campos (validação)
  - [ ] Logout funcional
  - [ ] Token JWT armazenado corretamente
  - [ ] Redirecionamento após login

- [ ] **Proteção de Rotas**
  - [ ] Rotas protegidas não acessíveis sem login
  - [ ] Redirecionamento para login quando não autenticado
  - [ ] Acesso correto após autenticação

### Dashboard

- [ ] **Visão Geral**
  - [ ] Cards de estatísticas carregam
  - [ ] Números estão corretos (pacientes, agendamentos, receita)
  - [ ] Gráficos renderizam corretamente
  - [ ] Dados em tempo real

### Pacientes

- [ ] **Listagem**
  - [ ] Tabela de pacientes carrega
  - [ ] Paginação funciona
  - [ ] Ordenação por colunas
  - [ ] Busca por nome, CPF, telefone
  - [ ] Filtros funcionam

- [ ] **Cadastro**
  - [ ] Formulário de novo paciente abre
  - [ ] Validações de campos obrigatórios
  - [ ] Validação de CPF
  - [ ] Validação de email
  - [ ] Validação de telefone
  - [ ] Salvar paciente adulto
  - [ ] Salvar paciente criança (com responsável)

- [ ] **Edição**
  - [ ] Abrir formulário de edição
  - [ ] Campos pré-preenchidos
  - [ ] Salvar alterações
  - [ ] Cancelar edição

- [ ] **Visualização**
  - [ ] Ver detalhes do paciente
  - [ ] Histórico de consultas
  - [ ] Prontuários anteriores
  - [ ] Dependentes (se for responsável)

### Agendamentos

- [ ] **Agenda**
  - [ ] Visualização em calendário
  - [ ] Visualização em lista
  - [ ] Filtro por data
  - [ ] Filtro por médico
  - [ ] Filtro por status

- [ ] **Criar Agendamento**
  - [ ] Formulário abre
  - [ ] Seleção de paciente
  - [ ] Seleção de médico
  - [ ] Seleção de data/hora
  - [ ] Horários disponíveis mostrados
  - [ ] Confirmar agendamento

- [ ] **Gerenciar Agendamento**
  - [ ] Confirmar agendamento
  - [ ] Cancelar agendamento
  - [ ] Remarcar agendamento
  - [ ] Iniciar atendimento

### Atendimento Médico

- [ ] **Prontuário**
  - [ ] Abrir tela de atendimento
  - [ ] Timer de consulta inicia
  - [ ] Campos de anamnese
  - [ ] Campos de exame físico
  - [ ] Campos de diagnóstico (CID-10)
  - [ ] Histórico do paciente visível

- [ ] **Prescrição**
  - [ ] Adicionar medicamento
  - [ ] Autocomplete de medicamentos
  - [ ] Dosagem e frequência
  - [ ] Múltiplos medicamentos
  - [ ] Templates de prescrição

- [ ] **Solicitação de Exames**
  - [ ] Adicionar exames
  - [ ] Selecionar tipo de exame
  - [ ] Observações do exame

- [ ] **Procedimentos**
  - [ ] Adicionar procedimentos realizados
  - [ ] Valores calculados automaticamente

- [ ] **Finalizar Atendimento**
  - [ ] Salvar prontuário
  - [ ] Imprimir prescrição
  - [ ] Imprimir atestado
  - [ ] Resumo de cobrança

### Financeiro

- [ ] **Dashboard Financeiro**
  - [ ] Receitas do período
  - [ ] Despesas do período
  - [ ] Lucro líquido
  - [ ] Gráficos de evolução

- [ ] **Contas a Receber**
  - [ ] Listagem de pagamentos pendentes
  - [ ] Processar pagamento
  - [ ] Filtros por status

- [ ] **Contas a Pagar**
  - [ ] Listagem de despesas
  - [ ] Criar nova despesa
  - [ ] Marcar como paga
  - [ ] Filtros por categoria e status

- [ ] **Relatórios**
  - [ ] Relatório de receitas
  - [ ] Relatório de despesas
  - [ ] Relatório de lucro
  - [ ] Exportar relatórios

### Configurações

- [ ] **Perfil**
  - [ ] Visualizar dados do usuário
  - [ ] Editar perfil
  - [ ] Alterar senha

- [ ] **Clínica**
  - [ ] Dados da clínica
  - [ ] Editar informações
  - [ ] Logo da clínica

- [ ] **Usuários**
  - [ ] Listar usuários da clínica
  - [ ] Adicionar novo usuário
  - [ ] Editar usuário
  - [ ] Desativar usuário
  - [ ] Gerenciar permissões

---

## 🔐 Testes de Frontend - System Admin

### Dashboard do Sistema

- [ ] **Métricas Globais**
  - [ ] Total de clínicas ativas
  - [ ] MRR (Monthly Recurring Revenue)
  - [ ] Churn rate
  - [ ] Gráficos de crescimento

### Gestão de Clínicas

- [ ] **Listagem**
  - [ ] Todas as clínicas listadas
  - [ ] Paginação
  - [ ] Busca por nome/CNPJ
  - [ ] Filtros por status

- [ ] **Detalhes da Clínica**
  - [ ] Informações completas
  - [ ] Assinatura atual
  - [ ] Histórico de pagamentos
  - [ ] Usuários da clínica

- [ ] **Ações**
  - [ ] Ativar clínica
  - [ ] Desativar clínica
  - [ ] Alterar plano
  - [ ] Override manual de assinatura

### Gestão de Planos

- [ ] **Planos de Assinatura**
  - [ ] Listar planos
  - [ ] Criar novo plano
  - [ ] Editar plano
  - [ ] Desativar plano

### Gestão de System Owners

- [ ] **Administradores**
  - [ ] Listar system owners
  - [ ] Criar novo admin
  - [ ] Editar admin
  - [ ] Desativar admin

---

## 🔗 Testes de Integração

### Fluxo Completo - Primeiro Atendimento

1. [ ] Login no sistema
2. [ ] Cadastrar novo paciente
3. [ ] Criar agendamento para o paciente
4. [ ] Confirmar agendamento
5. [ ] Iniciar atendimento
6. [ ] Preencher prontuário
7. [ ] Adicionar prescrição
8. [ ] Adicionar procedimentos
9. [ ] Finalizar atendimento
10. [ ] Processar pagamento
11. [ ] Verificar histórico do paciente

### Fluxo Completo - Paciente Recorrente

1. [ ] Login no sistema
2. [ ] Buscar paciente existente
3. [ ] Ver histórico completo
4. [ ] Criar novo agendamento
5. [ ] Atendimento com histórico visível
6. [ ] Comparar com consultas anteriores

### Fluxo Completo - Gestão Financeira

1. [ ] Registrar despesas do mês
2. [ ] Processar pagamentos de consultas
3. [ ] Gerar relatório financeiro
4. [ ] Analisar lucro do período
5. [ ] Exportar dados

### Fluxo Completo - Multi-tenancy

1. [ ] Login em clínica A
2. [ ] Cadastrar paciente
3. [ ] Logout
4. [ ] Login em clínica B
5. [ ] Tentar acessar paciente da clínica A (deve falhar)
6. [ ] Cadastrar mesmo paciente (CPF) na clínica B
7. [ ] Verificar isolamento de prontuários

---

## 🔒 Testes de Segurança

### Autenticação

- [ ] **JWT Token**
  - [ ] Token gerado corretamente
  - [ ] Token expira após 60 minutos
  - [ ] Token inválido rejeitado
  - [ ] Token expirado rejeitado

- [ ] **Senha**
  - [ ] Senha hashada com BCrypt
  - [ ] Validação de força de senha
  - [ ] Não retornar hash de senha em APIs

- [ ] **Rate Limiting**
  - [ ] Limite de requisições por minuto
  - [ ] Bloqueio temporário após exceder
  - [ ] Resposta 429 Too Many Requests

### Autorização

- [ ] **Roles e Permissões**
  - [ ] SystemAdmin acessa área de sistema
  - [ ] Doctor acessa prontuários
  - [ ] Receptionist não acessa prontuários
  - [ ] Owner gerencia usuários

- [ ] **Tenant Isolation**
  - [ ] Usuário de clínica A não acessa dados da clínica B
  - [ ] Header X-Tenant-Id obrigatório
  - [ ] Validação de tenant em todas as APIs

### Proteções

- [ ] **CORS**
  - [ ] Apenas origens permitidas
  - [ ] Headers corretos configurados

- [ ] **Security Headers**
  - [ ] X-Content-Type-Options
  - [ ] X-Frame-Options
  - [ ] Content-Security-Policy

- [ ] **Input Validation**
  - [ ] Proteção contra XSS
  - [ ] Proteção contra SQL Injection
  - [ ] Sanitização de inputs

---

## ⚡ Testes de Performance

### Backend

- [ ] **Tempo de Resposta**
  - [ ] Login < 500ms
  - [ ] Listagem de pacientes < 1s
  - [ ] Criação de prontuário < 1s

- [ ] **Carga**
  - [ ] 100 requisições simultâneas
  - [ ] Sem memory leaks

### Frontend

- [ ] **Carregamento**
  - [ ] Primeira página < 3s
  - [ ] Páginas subsequentes < 1s

- [ ] **Responsividade**
  - [ ] Desktop (1920x1080)
  - [ ] Tablet (768x1024)
  - [ ] Mobile (375x667)

---

## 📊 Resumo de Cobertura

### APIs Testadas
- [ ] Autenticação (100%)
- [ ] Registro (100%)
- [ ] Pacientes (100%)
- [ ] Agendamentos (100%)
- [ ] Prontuários (100%)
- [ ] Procedimentos (100%)
- [ ] Pagamentos (100%)
- [ ] Despesas (100%)
- [ ] Relatórios (100%)
- [ ] Notificações (100%)
- [ ] Medicamentos (100%)
- [ ] Data Seeder (100%)

### Frontend Testado
- [ ] Autenticação e Proteção (100%)
- [ ] Dashboard (100%)
- [ ] Pacientes (100%)
- [ ] Agendamentos (100%)
- [ ] Atendimento Médico (100%)
- [ ] Financeiro (100%)
- [ ] Configurações (100%)

### Segurança Testada
- [ ] JWT e Autenticação (100%)
- [ ] Autorização e Roles (100%)
- [ ] Tenant Isolation (100%)
- [ ] Rate Limiting (100%)
- [ ] Security Headers (100%)

---

## ✅ Critérios de Aceitação

Para considerar o sistema pronto para produção:

1. **APIs:** 100% dos endpoints principais testados e funcionais
2. **Frontend:** Todas as telas principais carregam e funcionam
3. **Segurança:** Autenticação, autorização e isolamento funcionando
4. **Performance:** Tempos de resposta dentro dos limites
5. **Documentação:** Guias atualizados e completos
6. **Data Seeding:** Dados demo completos para testes

---

**Última Atualização:** Novembro 2024  
**Responsável:** Time de Desenvolvimento  
**Status:** Em Execução
