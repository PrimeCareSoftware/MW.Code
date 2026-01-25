# 🌱 Guia Completo da API de Seed - Dados de Exemplo

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Pré-requisitos](#pré-requisitos)
3. [Endpoints Disponíveis](#endpoints-disponíveis)
4. [Dados Criados pelo Seed](#dados-criados-pelo-seed)
5. [Fluxo Completo de Uso](#fluxo-completo-de-uso)
6. [Credenciais de Acesso](#credenciais-de-acesso)
7. [Cenários de Teste](#cenários-de-teste)
8. [Troubleshooting](#troubleshooting)

## 🎯 Visão Geral

A API de Seed permite popular o banco de dados com dados de exemplo completos e realistas para desenvolvimento e testes. O sistema cria automaticamente:

- ✅ **8 Perfis de formulário de consulta padrão do sistema** (Médico, Psicólogo, Nutricionista, Fisioterapeuta, Dentista, Enfermeiro, Terapeuta Ocupacional, Fonoaudiólogo) 🆕
- ✅ **5 Planos de assinatura** (Trial, Básico, Standard, Premium, Enterprise)
- ✅ **1 Clínica Demo** completa com configurações
- ✅ **1 Assinatura ativa** (Plano Standard)
- ✅ **1 Proprietário** da clínica (Owner)
- ✅ **3 Usuários** com diferentes perfis (Admin, Médico, Recepcionista)
- ✅ **6 Pacientes com nome da mãe (CFM 1.821)** incluindo 2 crianças com responsável
- ✅ **8 Procedimentos** diversos (consultas, exames, vacinas, etc.)
- ✅ **5 Agendamentos** (passados, hoje e futuros)
- ✅ **3 Procedimentos vinculados** a agendamentos
- ✅ **2 Pagamentos** processados
- ✅ **8 Medicamentos** cadastrados
- ✅ **2 Prontuários médicos** completos
- ✅ **3 Itens de prescrição** vinculados aos prontuários
- ✅ **2 Prescrições digitais assinadas (CFM 1.643/2002 e ANVISA 344/1998)** 🆕
- ✅ **4 Templates de prescrição** (antibióticos, anti-hipertensivos, analgésicos, diabetes)
- ✅ **3 Templates de prontuário** (clínica geral, cardiologia, pediatria)
- ✅ **5 Notificações** em diversos estados (pendente, enviado, entregue, lido)
- ✅ **5 Rotinas de notificação** automatizadas
- ✅ **10 Despesas** (pagas, pendentes, vencidas e canceladas)
- ✅ **5 Solicitações de exames** (laboratoriais, imagem, cardiológicos)
- ✅ **3 Planos de saúde ativos** para pacientes 🆕
- ✅ **3 Notas fiscais** (2 pagas, 1 pendente) 🆕

## 📋 Pré-requisitos

Antes de usar a API de Seed, certifique-se que:

1. ✅ O banco de dados PostgreSQL está rodando
2. ✅ A aplicação ASP.NET Core está rodando
3. ✅ As migrations foram executadas com sucesso
4. ✅ Você tem acesso ao Postman ou outro cliente HTTP

### Verificar se o sistema está rodando

```bash
# Verificar se o PostgreSQL está rodando (Podman/Docker)
podman ps

# Verificar se a API está respondendo
curl http://localhost:5000/health
```

## 🔧 Endpoints Disponíveis

### 1. GET /api/data-seeder/demo-info

**Descrição**: Retorna informações sobre os dados de exemplo sem criar nada no banco.

**Uso**: Para consultar quais dados serão criados antes de executar o seed.

**Exemplo de Requisição**:
```bash
curl -X GET http://localhost:5000/api/data-seeder/demo-info
```

**Resposta**:
```json
{
  "tenantId": "demo-clinic-001",
  "clinic": {
    "name": "Clínica Demo PrimeCare Software",
    "tradeName": "Clínica Demo"
  },
  "users": [
    {
      "username": "owner.demo",
      "role": "Owner",
      "email": "owner@clinicademo.com.br"
    },
    {
      "username": "admin",
      "role": "SystemAdmin",
      "email": "admin@clinicademo.com.br"
    },
    {
      "username": "dr.silva",
      "role": "Doctor",
      "email": "joao.silva@clinicademo.com.br",
      "crm": "CRM-123456",
      "specialty": "Clínico Geral"
    },
    {
      "username": "recep.maria",
      "role": "Receptionist",
      "email": "maria.santos@clinicademo.com.br"
    }
  ],
  "dataSeeded": {
    "subscriptionPlans": 5,
    "clinic": 1,
    "clinicSubscription": 1,
    "owner": 1,
    "users": 3,
    "patients": 6,
    "procedures": 8,
    "appointments": 5,
    "payments": 2,
    "medications": 8,
    "medicalRecords": 2,
    "prescriptionItems": 3,
    "prescriptionTemplates": 4,
    "medicalRecordTemplates": 3,
    "notifications": 5,
    "notificationRoutines": 5,
    "expenses": 10,
    "examRequests": 5
  }
}
```

### 2. POST /api/data-seeder/seed-demo

**Descrição**: Cria todos os dados de exemplo no banco de dados.

**⚠️ IMPORTANTE**: Este endpoint só pode ser executado UMA vez. Se você tentar executar novamente, receberá um erro informando que os dados já existem.

**Exemplo de Requisição**:
```bash
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

**Resposta de Sucesso**:
```json
{
  "message": "Demo data seeded successfully",
  "tenantId": "demo-clinic-001",
  "credentials": {
    "owner": {
      "username": "owner.demo",
      "password": "Owner@123",
      "role": "Owner"
    },
    "users": [
      {
        "username": "admin",
        "password": "Admin@123",
        "role": "SystemAdmin"
      },
      {
        "username": "dr.silva",
        "password": "Doctor@123",
        "role": "Doctor"
      },
      {
        "username": "recep.maria",
        "password": "Recep@123",
        "role": "Receptionist"
      }
    ]
  },
  "summary": {
    "subscriptionPlans": 5,
    "clinic": 1,
    "clinicSubscription": 1,
    "owner": 1,
    "users": 3,
    "patients": 6,
    "procedures": 8,
    "appointments": 5,
    "payments": 2,
    "medications": 8,
    "medicalRecords": 2,
    "prescriptionItems": 3,
    "prescriptionTemplates": 4,
    "medicalRecordTemplates": 3,
    "notifications": 5,
    "notificationRoutines": 5,
    "expenses": 10,
    "examRequests": 5,
    "digitalPrescriptions": 2,
    "healthInsurancePlans": 3,
    "invoices": 3
  },
  "note": "Use these credentials to login and test the system. Complete database seeded with realistic demo data including CFM/ANVISA compliant digital prescriptions."
}
```

**Resposta de Erro** (se dados já existem):
```json
{
  "error": "Demo data already exists for this tenant"
}
```

### 3. POST /api/data-seeder/seed-system-owner

**Descrição**: Cria um proprietário do sistema (System Owner) para gerenciar a plataforma.

**⚠️ ATENÇÃO**: Este endpoint só está disponível em ambiente de desenvolvimento ou quando `Development:EnableDevEndpoints` está configurado como `true`.

**Exemplo de Requisição**:
```bash
curl -X POST http://localhost:5000/api/data-seeder/seed-system-owner
```

**Resposta**:
```json
{
  "message": "System owner created successfully",
  "owner": {
    "username": "admin",
    "email": "admin@medicwarehouse.com",
    "password": "Admin@123",
    "isSystemOwner": true,
    "tenantId": "system"
  },
  "loginInfo": {
    "endpoint": "POST /api/auth/owner-login",
    "body": {
      "username": "admin",
      "password": "Admin@123",
      "tenantId": "system"
    }
  },
  "note": "Use these credentials to login and manage the system. Change the password after first login!"
}
```

### 4. DELETE /api/data-seeder/clear-database

**Descrição**: Remove TODOS os dados de exemplo do banco de dados.

**⚠️ PERIGO**: Este endpoint deleta TODOS os dados. Use com extremo cuidado!

**⚠️ ATENÇÃO**: Só está disponível em ambiente de desenvolvimento ou quando `Development:EnableDevEndpoints` está configurado como `true`.

**Exemplo de Requisição**:
```bash
curl -X DELETE http://localhost:5000/api/data-seeder/clear-database
```

**Resposta**:
```json
{
  "message": "Database cleared successfully",
  "deletedTables": [
    "PrescriptionItems",
    "ExamRequests",
    "Notifications",
    "NotificationRoutines",
    "MedicalRecords",
    "Payments",
    "AppointmentProcedures",
    "Appointments",
    "PatientClinicLinks",
    "Patients",
    "PrescriptionTemplates",
    "MedicalRecordTemplates",
    "Medications",
    "ExamCatalogs",
    "Procedures",
    "Expenses",
    "Users",
    "OwnerClinicLinks",
    "ClinicSubscriptions",
    "Owners",
    "Clinics",
    "SubscriptionPlans"
  ],
  "note": "All demo data has been removed from the database. You can now re-seed the database using POST /api/data-seeder/seed-demo"
}
```

## 📊 Dados Criados pelo Seed

### Planos de Assinatura

| Plano | Preço (R$) | Usuários | Agendamentos/mês | Pacientes | Funcionalidades |
|-------|-----------|----------|------------------|-----------|-----------------|
| **Trial** | R$ 0,00 | 3 | 30 dias | 50 | Teste gratuito básico |
| **Básico** | R$ 99,90 | 5 | 15 dias | 100 | Relatórios + SMS |
| **Standard** | R$ 199,90 | 15 | 15 dias | 500 | WhatsApp + TISS + Relatórios |
| **Premium** | R$ 399,90 | 50 | 15 dias | 2000 | Todas as funcionalidades |
| **Enterprise** | R$ 999,90 | 200 | 30 dias | 10000 | Suporte dedicado |

### Clínica Demo

```
Nome: Clínica Demo PrimeCare Software
Nome Fantasia: Clínica Demo
CNPJ: 12.345.678/0001-95
Telefone: +55 11 98765-4321
Email: contato@clinicademo.com.br
Endereço: Avenida Paulista, 1000 - Bela Vista, São Paulo - SP
Horário: 08:00 - 18:00
TenantID: demo-clinic-001
Plano: Standard (ativo)
```

### Usuários Criados

| Username | Senha | Perfil | Email | Especialidade |
|----------|-------|--------|-------|---------------|
| **owner.demo** | Owner@123 | Owner | owner@clinicademo.com.br | - |
| **admin** | Admin@123 | SystemAdmin | admin@clinicademo.com.br | - |
| **dr.silva** | Doctor@123 | Doctor | joao.silva@clinicademo.com.br | Clínico Geral |
| **recep.maria** | Recep@123 | Receptionist | maria.santos@clinicademo.com.br | - |

### Pacientes Cadastrados

1. **Carlos Alberto Santos** (45 anos, masculino)
   - CPF: 529.982.247-25
   - Condições: Hipertensão arterial
   - Alergias: Penicilina

2. **Ana Maria Oliveira** (48 anos, feminino)
   - CPF: 318.649.712-40
   - Condições: Diabetes tipo 2

3. **Pedro Henrique Costa** (33 anos, masculino)
   - CPF: 123.891.234-65
   - Sem condições especiais

4. **Juliana Martins Silva** (38 anos, feminino) - Responsável
   - CPF: 456.782.345-10

5. **Lucas Martins Silva** (8 anos, masculino) - Filho de Juliana
   - CPF: 789.673.456-74
   - Condições: Asma leve

6. **Sofia Martins Silva** (6 anos, feminino) - Filha de Juliana
   - CPF: 912.564.567-64
   - Alergias: Lactose

### Procedimentos Cadastrados

1. **Consulta Médica Geral** - R$ 150,00 (30 min)
2. **Consulta Cardiológica** - R$ 250,00 (45 min)
3. **Exame de Sangue Completo** - R$ 80,00 (15 min)
4. **Eletrocardiograma** - R$ 120,00 (20 min)
5. **Vacina Influenza** - R$ 50,00 (10 min)
6. **Fisioterapia Sessão** - R$ 100,00 (60 min)
7. **Sutura Pequeno Porte** - R$ 200,00 (30 min)
8. **Retorno Consulta** - R$ 80,00 (20 min)

### Agendamentos Criados

| Data | Hora | Paciente | Status | Tipo |
|------|------|----------|--------|------|
| Há 7 dias | 09:00 | Carlos | Completo | Consulta Geral |
| Há 5 dias | 10:00 | Ana | Completo | Cardiologia + ECG |
| Hoje | 14:00 | Pedro | Confirmado | Consulta Geral |
| Daqui 3 dias | 15:00 | Lucas | Agendado | Pediatria |
| Daqui 3 dias | 15:30 | Sofia | Agendado | Pediatria |

### Medicamentos no Catálogo

O sistema inclui um catálogo completo com mais de 100 medicamentos organizados por categoria:

- **Analgésicos**: Dipirona, Paracetamol, Tramadol, Codeína + Paracetamol, Morfina
- **Anti-inflamatórios**: Ibuprofeno, Naproxeno, Nimesulida, Diclofenaco, Cetoprofeno, Meloxicam, Piroxicam
- **Antibióticos**: Amoxicilina, Azitromicina, Ciprofloxacino, Cefalexina, Ceftriaxona
- **Anti-hipertensivos**: Losartana, Enalapril, Captopril, Anlodipino, Atenolol
- E muitos mais...

### Prontuários Médicos

2 prontuários completos foram criados para consultas finalizadas:

1. **Carlos Alberto Santos** (Consulta há 7 dias)
   - Queixa: Consulta de rotina para controle de hipertensão
   - Diagnóstico: Hipertensão arterial controlada
   - Prescrição: Losartana 50mg + orientações

2. **Ana Maria Oliveira** (Consulta há 5 dias)
   - Queixa: Palpitações e controle de diabetes
   - Diagnóstico: Diabetes tipo 2 + Arritmia cardíaca
   - Exames: ECG realizado, HbA1c solicitado
   - Prescrição: Ajuste de medicação

### Templates Disponíveis

**Templates de Prescrição**:
1. Receita para Antibióticos
2. Receita para Anti-hipertensivos
3. Receita para Analgésicos
4. Receita para Diabetes

**Templates de Prontuário**:
1. Consulta Clínica Geral
2. Consulta Cardiológica
3. Consulta Pediátrica

### Notificações

5 notificações foram criadas demonstrando diferentes estados:

1. **SMS enviado e entregue** - Lembrete de consulta (Carlos)
2. **WhatsApp enviado, entregue e lido** - Lembrete de consulta (Ana)
3. **SMS enviado** - Confirmação de consulta hoje (Pedro)
4. **WhatsApp pendente** - Lembrete de consulta futura (Lucas)
5. **Email enviado e entregue** - Confirmação de pagamento (Carlos)

### Rotinas de Notificação

5 rotinas automatizadas configuradas:

1. **Lembrete 24h antes** - WhatsApp
2. **Lembrete 2h antes** - SMS
3. **Confirmação de agendamento** - Email
4. **Aniversário do paciente** - WhatsApp
5. **Pesquisa de satisfação** - Email (24h após consulta)

### Despesas

10 despesas com diferentes status:

| Descrição | Categoria | Valor | Vencimento | Status |
|-----------|-----------|-------|------------|--------|
| Aluguel | Rent | R$ 3.500,00 | Há 25 dias | ✅ Pago |
| Energia | Utilities | R$ 450,00 | Há 20 dias | ✅ Pago |
| Internet | Utilities | R$ 199,90 | Há 18 dias | ✅ Pago |
| Material Limpeza | Supplies | R$ 350,00 | Há 15 dias | ✅ Pago |
| Software PrimeCare | Software | R$ 199,90 | Daqui 5 dias | ⏳ Pendente |
| Material Médico | Supplies | R$ 890,00 | Daqui 10 dias | ⏳ Pendente |
| Manutenção AC | Maintenance | R$ 280,00 | Há 5 dias | ⚠️ Vencida |
| Contador | ProfessionalServices | R$ 650,00 | Daqui 15 dias | ⏳ Pendente |
| Marketing | Marketing | R$ 500,00 | Há 10 dias | ✅ Pago |
| Treinamento | Training | R$ 1.200,00 | Daqui 20 dias | ❌ Cancelada |

### Solicitações de Exames

5 solicitações de exames em diferentes estados:

1. **Hemograma + Glicemia + Perfil Lipídico** (Carlos, há 7 dias) - ✅ Completo
2. **Hemograma + HbA1c** (Ana, há 5 dias) - ✅ Completo
3. **Eletrocardiograma** (Ana, há 5 dias) - ✅ Completo
4. **Raio-X de Tórax** (Pedro, hoje) - ⏳ Pendente
5. **Ecocardiograma** (Ana, há 5 dias) - 📅 Agendado para daqui 5 dias

## 🚀 Fluxo Completo de Uso

### Opção 1: Usando o Postman (Recomendado)

1. **Importe a coleção do Postman**
   - Abra o Postman
   - File > Import
   - Selecione o arquivo `PrimeCare-Postman-Collection.json`

2. **Popule os dados de exemplo**
   - Na pasta "Data Seeder", execute: `Seed Demo Data`
   - Guarde as credenciais retornadas

3. **Faça login**
   - Na pasta "Auth", execute: `Login`
   - Use: username: `dr.silva`, password: `Doctor@123`, tenantId: `demo-clinic-001`
   - Copie o token retornado

4. **Configure o token**
   - Nas variáveis da coleção, cole o token em `bearer_token`

5. **Teste os endpoints**
   - Agora você pode testar qualquer endpoint da API
   - Todos já estão configurados para usar o token automaticamente

### Opção 2: Usando cURL

1. **Popule os dados**
```bash
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

2. **Faça login**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "dr.silva",
    "password": "Doctor@123",
    "tenantId": "demo-clinic-001"
  }'
```

3. **Salve o token da resposta**
```json
{
  "token": "eyJhbGc...",
  "expiresAt": "2024-01-15T12:00:00Z"
}
```

4. **Use o token nas requisições**
```bash
curl -X GET http://localhost:5000/api/patients \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "X-Tenant-ID: demo-clinic-001"
```

### Opção 3: Script Automatizado

Crie um arquivo `setup-demo.sh`:

```bash
#!/bin/bash

echo "🌱 Populando dados de exemplo..."
curl -X POST http://localhost:5000/api/data-seeder/seed-demo

echo ""
echo "🔐 Fazendo login..."
RESPONSE=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "dr.silva",
    "password": "Doctor@123",
    "tenantId": "demo-clinic-001"
  }')

TOKEN=$(echo $RESPONSE | jq -r '.token')

echo ""
echo "✅ Token obtido com sucesso!"
echo "📋 Token: $TOKEN"
echo ""
echo "🎉 Sistema pronto para uso!"
echo ""
echo "Exemplo de uso:"
echo "curl -X GET http://localhost:5000/api/patients \\"
echo "  -H \"Authorization: Bearer $TOKEN\" \\"
echo "  -H \"X-Tenant-ID: demo-clinic-001\""
```

Torne executável e execute:
```bash
chmod +x setup-demo.sh
./setup-demo.sh
```

## 🔑 Credenciais de Acesso

### Para Testes de Proprietário (Owner)
```
Username: owner.demo
Password: Owner@123
TenantID: demo-clinic-001
Permissões: Acesso total à clínica
```

### Para Testes de Administrador
```
Username: admin
Password: Admin@123
TenantID: demo-clinic-001
Permissões: Administração completa do sistema
```

### Para Testes de Médico
```
Username: dr.silva
Password: Doctor@123
TenantID: demo-clinic-001
CRM: CRM-123456
Especialidade: Clínico Geral
Permissões: Consultas, prontuários, prescrições
```

### Para Testes de Recepcionista
```
Username: recep.maria
Password: Recep@123
TenantID: demo-clinic-001
Permissões: Agendamentos, pacientes, pagamentos
```

## 🧪 Cenários de Teste

### Cenário 1: Gestão de Pacientes

1. Login como `recep.maria`
2. Listar todos os pacientes: `GET /api/patients`
3. Buscar paciente por CPF: `GET /api/patients/document/529.982.247-25`
4. Ver detalhes do paciente: `GET /api/patients/{id}`
5. Criar novo paciente: `POST /api/patients`
6. Atualizar paciente: `PUT /api/patients/{id}`

### Cenário 2: Agendamentos

1. Login como `recep.maria`
2. Ver agenda do dia: `GET /api/appointments/daily-agenda?date=2024-01-15`
3. Ver horários disponíveis: `GET /api/appointments/available-slots?date=2024-01-15`
4. Criar agendamento: `POST /api/appointments`
5. Confirmar agendamento: `PUT /api/appointments/{id}/confirm`
6. Cancelar agendamento: `PUT /api/appointments/{id}/cancel`

### Cenário 3: Atendimento Médico

1. Login como `dr.silva`
2. Ver agendamentos do dia: `GET /api/appointments/daily-agenda`
3. Fazer check-in do paciente: `PUT /api/appointments/{id}/checkin`
4. Criar prontuário: `POST /api/medical-records`
5. Adicionar prescrição: `POST /api/medical-records/{id}/prescriptions`
6. Solicitar exames: `POST /api/exam-requests`
7. Fazer check-out: `PUT /api/appointments/{id}/checkout`
8. Completar prontuário: `PUT /api/medical-records/{id}/complete`

### Cenário 4: Gestão Financeira

1. Login como `owner.demo`
2. Ver resumo financeiro: `GET /api/reports/financial-summary`
3. Ver contas a receber: `GET /api/reports/accounts-receivable`
4. Ver contas a pagar: `GET /api/reports/accounts-payable`
5. Listar despesas: `GET /api/expenses`
6. Pagar despesa: `PUT /api/expenses/{id}/pay`
7. Ver relatório de receitas: `GET /api/reports/revenue`

### Cenário 5: Relatórios

1. Login como `owner.demo` ou `admin`
2. Relatório de agendamentos: `GET /api/reports/appointments?startDate=2024-01-01&endDate=2024-01-31`
3. Relatório de pacientes: `GET /api/reports/patients?startDate=2024-01-01&endDate=2024-01-31`
4. Relatório financeiro: `GET /api/reports/financial-summary?startDate=2024-01-01&endDate=2024-01-31`

## ❌ Troubleshooting

### Erro: "Demo data already exists for this tenant"

**Problema**: Você já executou o seed anteriormente.

**Soluções**:

1. **Limpar e recriar** (desenvolvimento):
```bash
# Limpar dados
curl -X DELETE http://localhost:5000/api/data-seeder/clear-database

# Recriar dados
curl -X POST http://localhost:5000/api/data-seeder/seed-demo
```

2. **Usar os dados existentes**: Simplesmente faça login com as credenciais listadas acima.

### Erro: "This endpoint is only available in Development environment"

**Problema**: O endpoint de clear-database está protegido.

**Solução**: Configure o ambiente como Development ou adicione no `appsettings.json`:

```json
{
  "Development": {
    "EnableDevEndpoints": true
  }
}
```

### Erro: "Connection to database failed"

**Problema**: O PostgreSQL não está rodando ou não é acessível.

**Soluções**:

1. **Verificar se o PostgreSQL está rodando**:
```bash
podman ps | grep postgres
```

2. **Iniciar o PostgreSQL**:
```bash
podman-compose up -d postgres
```

3. **Verificar a connection string** no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicsoft;Username=postgres;Password=postgres"
  }
}
```

### Erro 401 Unauthorized

**Problema**: Token inválido ou expirado.

**Solução**: Faça login novamente e atualize o token:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "dr.silva",
    "password": "Doctor@123",
    "tenantId": "demo-clinic-001"
  }'
```

### Erro 403 Forbidden

**Problema**: O usuário não tem permissão para acessar o recurso.

**Solução**: Use um usuário com o perfil adequado:
- **Owner** ou **Admin** para operações administrativas
- **Doctor** para prontuários e prescrições
- **Receptionist** para agendamentos e pagamentos

## 📚 Próximos Passos

Após popular os dados de exemplo:

1. 📖 **[Guia de Desenvolvimento de Autenticação](GUIA_DESENVOLVIMENTO_AUTH.md)** - Entenda como funciona a autenticação
2. 📮 **[Guia do Postman](POSTMAN_QUICK_GUIDE.md)** - Aprenda a usar a collection do Postman
3. 🚀 **[Guia de Início Rápido](GUIA_INICIO_RAPIDO_LOCAL.md)** - Configure o ambiente completo
4. 🎯 **[Checklist de Testes](CHECKLIST_TESTES_COMPLETO.md)** - Teste todas as funcionalidades

## 🤝 Suporte

Problemas ou dúvidas? 

- 📧 Email: support@primecaresoftware.com
- 🐛 Issues: [GitHub Issues](https://github.com/PrimeCareSoftware/MW.Code/issues)
- 📖 Documentação: [Wiki do Projeto](https://github.com/PrimeCareSoftware/MW.Code/wiki)

---

**Última atualização**: Janeiro 2024
**Versão do sistema**: 1.0.0
