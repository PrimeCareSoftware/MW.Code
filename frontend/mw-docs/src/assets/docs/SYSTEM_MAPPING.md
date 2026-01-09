# Mapeamento Completo do Sistema PrimeCare Software

## 📊 Visão Geral da Arquitetura

O PrimeCare Software é um sistema de gestão para clínicas médicas desenvolvido em .NET 8.0 com arquitetura em camadas (Clean Architecture).

### Estrutura de Projetos

```
PrimeCare Software.sln
├── src/
│   ├── MedicSoft.Domain         - Entidades, interfaces e lógica de domínio
│   ├── MedicSoft.Application    - Serviços de aplicação e casos de uso
│   ├── MedicSoft.Repository     - Implementação de repositórios e DbContext
│   ├── MedicSoft.CrossCutting   - Serviços transversais (segurança, identity)
│   ├── MedicSoft.Api            - Controllers e endpoints da API
│   └── MedicSoft.WhatsAppAgent  - Agente de integração WhatsApp
├── tests/
│   └── MedicSoft.Test          - Testes unitários
└── frontend/                    - Aplicações frontend
    ├── medicwarehouse-app       - Aplicação principal
    ├── mw-system-admin          - Painel administrativo
    ├── mw-site                  - Site institucional
    └── mw-docs                  - Documentação
```

---

## 🗄️ Modelo de Dados Completo

### Entidades Principais (19 Entidades)

#### 1. **Gestão de Usuários e Autenticação**

##### Owner (Proprietário)
- Proprietários de clínicas no sistema
- Campos: Username, Email, PasswordHash, FullName, Phone, IsSystemOwner
- Relacionamentos: Pode ter múltiplas clínicas

##### User (Usuário)
- Usuários do sistema (admin, médicos, recepcionistas)
- Campos: Username, Email, PasswordHash, FullName, Phone, Role, CRM, Specialty
- Roles: SystemAdmin, Owner, Doctor, Receptionist, Nurse
- Relacionamentos: Vinculado a um tenant/clínica

##### PasswordResetToken
- Tokens para recuperação de senha
- Campos: Email, Token, ExpiresAt, IsUsed
- Tempo de expiração: Configurável

---

#### 2. **Gestão de Clínicas**

##### Clinic (Clínica)
- Dados principais da clínica
- Campos: LegalName, TradeName, Cnpj, Phone, Email, Address, OpeningTime, ClosingTime, DefaultAppointmentDuration
- Relacionamentos: Possui usuários, pacientes, agendamentos

##### ClinicSubscription (Assinatura da Clínica)
- Controle de assinaturas e pagamentos
- Campos: ClinicId, SubscriptionPlanId, StartDate, EndDate, TrialEndDate, Status, CurrentPrice
- Status: Trial, Active, Suspended, Cancelled, Overdue
- Funcionalidades: Freeze, upgrade/downgrade de planos

##### SubscriptionPlan (Plano de Assinatura)
- Planos disponíveis para clínicas
- Campos: Name, Description, MonthlyPrice, TrialDays, MaxUsers, MaxPatients
- Features: HasReports, HasWhatsAppIntegration, HasSMSNotifications, HasTissExport
- Tipos: Trial, Basic, Standard, Premium, Enterprise

##### ModuleConfiguration (Configuração de Módulos)
- Controle de módulos habilitados por clínica
- Campos: ClinicId, ModuleName, IsEnabled, Configuration
- Módulos: PatientManagement, AppointmentScheduling, MedicalRecords, Prescriptions, FinancialManagement, Reports, WhatsAppIntegration, SMSNotifications, TissExport, InventoryManagement, UserManagement

---

#### 3. **Gestão de Pacientes**

##### Patient (Paciente)
- Dados dos pacientes
- Campos: Name, Document (CPF), DateOfBirth, Gender, Email, Phone, Address, MedicalHistory, Allergies
- Funcionalidades: Responsável (para crianças), histórico médico
- Relacionamentos: Vinculado a clínicas, tem agendamentos, prontuários

##### PatientClinicLink
- Relacionamento N-N entre pacientes e clínicas
- Campos: PatientId, ClinicId
- Permite que um paciente seja atendido em múltiplas clínicas

##### HealthInsurancePlan (Plano de Saúde)
- Convênios médicos dos pacientes
- Campos: PatientId, InsuranceName, PlanNumber, PlanType, ValidFrom, ValidUntil, HolderName
- Validação: Verifica validade do plano

---

#### 4. **Agendamentos e Consultas**

##### Appointment (Agendamento)
- Agendamentos de consultas
- Campos: PatientId, ClinicId, Date, Time, Duration, Type, Status, Notes
- Tipos: Regular, FollowUp, Emergency, Exam
- Status: Pending, Confirmed, CheckedIn, InProgress, Completed, Cancelled, NoShow
- Workflow: Schedule → Confirm → CheckIn → CheckOut → Complete

##### AppointmentProcedure
- Procedimentos realizados em um agendamento
- Campos: AppointmentId, ProcedureId, PatientId, Price, PerformedAt
- Relacionamentos: Liga agendamentos aos procedimentos realizados

---

#### 5. **Procedimentos e Serviços**

##### Procedure (Procedimento)
- Serviços oferecidos pela clínica
- Campos: Name, Code, Description, Category, Price, Duration, RequiresMaterials
- Categorias: Consultation, Exam, Surgery, Therapy, Vaccination, FollowUp, Emergency, Procedure, Other
- Relacionamentos: Usado em agendamentos, pode ter materiais associados

##### Material (Material Médico)
- Materiais e insumos médicos
- Campos: Name, Code, Description, Unit, UnitPrice, StockQuantity, MinimumStock
- Funcionalidades: Controle de estoque, alertas de estoque baixo
- Relacionamentos: Vinculado a procedimentos

##### ProcedureMaterial
- Relacionamento entre procedimentos e materiais
- Campos: ProcedureId, MaterialId, Quantity
- Define quais materiais são usados em cada procedimento

---

#### 6. **Prontuários e Prescrições**

##### MedicalRecord (Prontuário Médico)
- Prontuários das consultas
- Campos: AppointmentId, PatientId, ChiefComplaint, PhysicalExam, Anamnesis, Diagnosis, Treatment, Notes
- Status: InProgress, Completed
- Relacionamentos: Vinculado a agendamento, pode ter prescrições

##### MedicalRecordTemplate (Template de Prontuário)
- Templates reutilizáveis para prontuários
- Campos: Name, Description, TemplateContent, Category
- Categorias: Clínica geral, Cardiologia, Pediatria, etc.

##### Medication (Medicamento)
- Catálogo de medicamentos
- Campos: Name, Dosage, Form, Category, RequiresPrescription, ActivePrinciple, Manufacturer
- Categorias: Antibiotic, Analgesic, AntiInflammatory, Antihypertensive, Antidiabetic, Antihistamine, Antacid, Vitamin, Other

##### PrescriptionItem (Item de Prescrição)
- Medicamentos prescritos no prontuário
- Campos: MedicalRecordId, MedicationId, Dosage, Instructions, Duration, Quantity
- Relacionamentos: Liga prontuários a medicamentos

##### PrescriptionTemplate (Template de Prescrição)
- Templates reutilizáveis para prescrições
- Campos: Name, Description, TemplateContent, Category
- Categorias: Antibióticos, Anti-hipertensivos, Analgésicos, Diabetes, etc.

---

#### 7. **Exames**

##### ExamRequest (Solicitação de Exame)
- Pedidos de exames médicos
- Campos: AppointmentId, PatientId, ExamType, ExamName, Description, Urgency, Status, Results
- Tipos: Laboratory, Imaging, Cardiac, Endoscopy, Biopsy, Ultrasound, Other
- Urgência: Routine, Urgent, Emergency
- Status: Pending, Scheduled, InProgress, Completed, Cancelled

---

#### 8. **Gestão Financeira**

##### Payment (Pagamento)
- Pagamentos de consultas e procedimentos
- Campos: Amount, PaymentMethod, Status, AppointmentId, InvoiceId, TransactionId
- Métodos: Cash, CreditCard, DebitCard, Pix, BankTransfer, HealthInsurance
- Status: Pending, Paid, Failed, Refunded, Cancelled

##### Invoice (Fatura)
- Faturas e notas fiscais
- Campos: InvoiceNumber, IssueDate, DueDate, TotalAmount, Status, PaymentId
- Status: Draft, Issued, Paid, Overdue, Cancelled
- Relacionamentos: Pode ter múltiplos pagamentos

##### Expense (Despesa)
- Despesas e contas a pagar da clínica
- Campos: ClinicId, Description, Category, Amount, DueDate, PaidDate, Status, SupplierName
- Categorias: Rent, Utilities, Supplies, Equipment, Maintenance, Marketing, Software, Salary, Taxes, Insurance, ProfessionalServices, Transportation, Training, Other
- Status: Pending, Paid, Overdue, Cancelled

---

#### 9. **Notificações**

##### Notification (Notificação)
- Notificações enviadas aos pacientes
- Campos: PatientId, Type, Channel, Recipient, Message, Status, SentAt, DeliveredAt
- Canais: SMS, Email, WhatsApp, Push
- Tipos: AppointmentReminder, AppointmentConfirmation, AppointmentCancellation, PaymentReminder, General
- Status: Pending, Sent, Delivered, Failed, Read

##### NotificationRoutine (Rotina de Notificação)
- Configuração de envios automáticos
- Campos: Name, Description, Channel, Type, MessageTemplate, ScheduleType, ScheduleConfiguration, Scope
- Tipos de Agendamento: Daily, Weekly, Monthly, Custom, BeforeAppointment, AfterAppointment
- Escopo: Clinic, System
- Funcionalidades: Templates com variáveis, retry automático

---

## 🔐 Sistema de Autenticação e Autorização

### Multi-Tenancy
- Isolamento por Tenant ID
- Cada clínica é um tenant isolado
- Global query filters no EF Core

### Autenticação
- JWT (JSON Web Tokens)
- Refresh tokens (não implementado ainda)
- Password hashing com BCrypt

### Roles e Permissões
1. **System Owner**: Acesso total ao sistema
2. **Owner**: Dono da clínica, acesso administrativo
3. **SystemAdmin**: Administrador da clínica
4. **Doctor**: Acesso médico completo
5. **Receptionist**: Agendamentos e cadastros
6. **Nurse**: Acesso limitado a enfermagem

---

## 🔌 APIs Disponíveis

### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/owner-login` - Login de proprietário
- `POST /api/auth/register` - Registro de nova clínica
- `POST /api/auth/password-reset-request` - Solicitar reset de senha
- `POST /api/auth/password-reset` - Resetar senha

### Pacientes
- `GET /api/patients` - Listar pacientes
- `GET /api/patients/{id}` - Detalhes do paciente
- `POST /api/patients` - Criar paciente
- `PUT /api/patients/{id}` - Atualizar paciente
- `DELETE /api/patients/{id}` - Deletar paciente
- `GET /api/patients/{id}/appointments` - Agendamentos do paciente
- `GET /api/patients/{id}/medical-records` - Prontuários do paciente

### Agendamentos
- `GET /api/appointments` - Listar agendamentos
- `GET /api/appointments/{id}` - Detalhes do agendamento
- `POST /api/appointments` - Criar agendamento
- `PUT /api/appointments/{id}` - Atualizar agendamento
- `DELETE /api/appointments/{id}` - Cancelar agendamento
- `PUT /api/appointments/{id}/confirm` - Confirmar agendamento
- `PUT /api/appointments/{id}/check-in` - Check-in
- `PUT /api/appointments/{id}/check-out` - Check-out

### Prontuários
- `GET /api/medical-records` - Listar prontuários
- `GET /api/medical-records/{id}` - Detalhes do prontuário
- `POST /api/medical-records` - Criar prontuário
- `PUT /api/medical-records/{id}` - Atualizar prontuário
- `PUT /api/medical-records/{id}/complete` - Completar prontuário

### Despesas
- `GET /api/expenses` - Listar despesas
- `GET /api/expenses/{id}` - Detalhes da despesa
- `POST /api/expenses` - Criar despesa
- `PUT /api/expenses/{id}` - Atualizar despesa
- `PUT /api/expenses/{id}/pay` - Marcar como paga
- `PUT /api/expenses/{id}/cancel` - Cancelar despesa

### Exames
- `GET /api/exam-requests` - Listar solicitações
- `GET /api/exam-requests/{id}` - Detalhes da solicitação
- `POST /api/exam-requests` - Criar solicitação
- `PUT /api/exam-requests/{id}` - Atualizar solicitação
- `PUT /api/exam-requests/{id}/schedule` - Agendar exame
- `PUT /api/exam-requests/{id}/complete` - Completar exame

### Seeders
- `GET /api/data-seeder/demo-info` - Informações dos dados demo
- `POST /api/data-seeder/seed-demo` - Popular banco com dados demo
- `POST /api/data-seeder/seed-system-owner` - Criar owner do sistema

---

## 🏗️ Padrões de Arquitetura

### Clean Architecture
```
API (Controllers)
    ↓
Application (Services, DTOs)
    ↓
Domain (Entities, Interfaces)
    ↓
Repository (Data Access)
```

### Padrões Utilizados
- **Repository Pattern**: Abstração de acesso a dados
- **Service Layer**: Lógica de negócio
- **Domain Events**: Eventos de domínio
- **Value Objects**: CPF, Email, Phone, Address, etc.
- **Factory Pattern**: DbContextFactory
- **Dependency Injection**: Injeção de dependências nativa do .NET

### Bibliotecas Principais
- **Entity Framework Core**: ORM
- **AutoMapper**: Mapeamento de objetos
- **MediatR**: Mediator pattern
- **FluentValidation**: Validação (potencial)
- **Swagger/OpenAPI**: Documentação da API
- **BCrypt**: Hash de senhas

---

## 📊 Banco de Dados

### SQL Server
- Migrations do Entity Framework Core
- Query filters para multi-tenancy
- Índices otimizados
- Relacionamentos configurados via Fluent API

### Principais Tabelas
1. Owners
2. Users
3. Clinics
4. SubscriptionPlans
5. ClinicSubscriptions
6. Patients
7. PatientClinicLinks
8. Appointments
9. Procedures
10. AppointmentProcedures
11. MedicalRecords
12. Medications
13. PrescriptionItems
14. ExamRequests
15. Payments
16. Invoices
17. Expenses
18. Notifications
19. NotificationRoutines

---

## 🔄 Fluxos Principais

### Fluxo de Agendamento
1. Recepcionista cria agendamento
2. Sistema envia notificação de confirmação
3. Sistema envia lembrete 24h antes (rotina automática)
4. Paciente faz check-in
5. Médico atende e cria prontuário
6. Sistema registra procedimentos realizados
7. Paciente faz check-out
8. Sistema gera cobrança
9. Pagamento é registrado

### Fluxo de Prescrição
1. Médico abre prontuário durante consulta
2. Seleciona template de prescrição (opcional)
3. Adiciona medicamentos
4. Define dosagem e instruções
5. Completa prontuário
6. Sistema vincula prescrição ao prontuário
7. Prescrição disponível para impressão

### Fluxo de Exames
1. Médico solicita exame durante consulta
2. Exame é registrado como pendente
3. Recepcionista agenda o exame
4. Sistema notifica paciente
5. Exame é realizado
6. Resultados são inseridos
7. Médico tem acesso aos resultados

---

## 🧪 Dados de Teste (Seeders)

Os seeders criam um ambiente completo de teste com:
- 5 planos de assinatura
- 1 clínica demo ativa
- 1 owner + 3 usuários
- 6 pacientes (incluindo casos especiais)
- 8 procedimentos médicos
- 5 agendamentos (passado, presente, futuro)
- 2 prontuários completos
- 8 medicamentos no catálogo
- 5 rotinas de notificação
- 10 despesas variadas
- 5 solicitações de exames

**Veja:** [SEEDER_GUIDE.md](./SEEDER_GUIDE.md) para detalhes completos

---

## 📈 Integrações

### WhatsApp
- MedicSoft.WhatsAppAgent
- Envio de notificações
- Configurável por clínica

### SMS
- Interface ISmSNotificationService
- Provider configurável

### Email
- SMTP configurável
- Templates de email

---

## 🔒 Segurança

### Implementações
- ✅ Autenticação JWT
- ✅ Hash de senhas com BCrypt
- ✅ Rate limiting
- ✅ CORS configurável
- ✅ HTTPS obrigatório em produção
- ✅ Security headers
- ✅ Proteção CSRF (token-based)
- ✅ Multi-tenancy com isolamento

### Melhorias Sugeridas
- ⚠️ Implementar refresh tokens
- ⚠️ 2FA (Two-factor authentication)
- ⚠️ Audit logs
- ⚠️ IP whitelisting para admin

---

## 📚 Documentação Disponível

1. **README.md** - Visão geral do projeto
2. **AUTHENTICATION_GUIDE.md** - Guia de autenticação
3. **SEEDER_GUIDE.md** - Guia completo dos seeders
4. **SEEDER_QUICK_REFERENCE.md** - Referência rápida dos seeders
5. **SYSTEM_MAPPING.md** (este arquivo) - Mapeamento completo
6. **API_CONTROLLERS_REPOSITORY_ACCESS_ANALYSIS.md** - Análise de controllers
7. **Postman Collection** - Collection completa para testes

---

## 🚀 Como Começar

1. **Clone o repositório**
2. **Configure o banco de dados** (connection string)
3. **Execute migrations**: `dotnet ef database update`
4. **Popular dados demo**: `POST /api/data-seeder/seed-demo`
5. **Fazer login** com credenciais do seeder
6. **Explorar as APIs** via Swagger em `/swagger`

---

## 🎯 Casos de Uso Cobertos

### Módulo de Pacientes
- ✅ Cadastro completo de pacientes
- ✅ Histórico médico
- ✅ Alergias e condições
- ✅ Responsável para crianças
- ✅ Múltiplas clínicas por paciente

### Módulo de Agendamentos
- ✅ Agendamento de consultas
- ✅ Tipos variados (regular, retorno, emergência)
- ✅ Controle de status
- ✅ Check-in/Check-out
- ✅ Cancelamento

### Módulo Médico
- ✅ Prontuários eletrônicos
- ✅ Prescrições médicas
- ✅ Solicitação de exames
- ✅ Templates reutilizáveis
- ✅ Histórico completo

### Módulo Financeiro
- ✅ Controle de pagamentos
- ✅ Múltiplos métodos de pagamento
- ✅ Faturas e notas fiscais
- ✅ Contas a pagar
- ✅ Controle de despesas

### Módulo de Comunicação
- ✅ Notificações multi-canal
- ✅ Rotinas automatizadas
- ✅ Templates personalizáveis
- ✅ Rastreamento de envio

### Módulo Administrativo
- ✅ Gestão de usuários
- ✅ Controle de permissões
- ✅ Assinaturas e planos
- ✅ Configuração de módulos
- ✅ Multi-tenancy

---

## 📊 Estatísticas do Sistema

- **19 Entidades principais**
- **50+ Endpoints de API**
- **6 Módulos funcionais**
- **5 Níveis de permissão**
- **4 Canais de notificação**
- **14 Categorias de despesas**
- **9 Categorias de procedimentos**
- **8 Categorias de medicamentos**
- **719 Testes unitários** (703 passando)

---

Este mapeamento fornece uma visão completa e detalhada de todo o sistema PrimeCare Software, suas entidades, relacionamentos, fluxos e funcionalidades.
