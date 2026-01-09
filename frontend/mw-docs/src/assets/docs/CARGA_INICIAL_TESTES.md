# Carga Inicial para Testes - PrimeCare Software

## 📋 Visão Geral

Este documento descreve a carga inicial de dados implementada para testar **todos os pontos do sistema** PrimeCare Software. Os dados de teste são abrangentes e cobrem todas as principais entidades e funcionalidades do sistema.

## 🎯 Objetivo

Gerar dados de demonstração completos e interconectados que permitam testar:
- ✅ Gestão de clínicas e usuários
- ✅ Cadastro e vínculo de pacientes
- ✅ Agendamentos com diferentes estados
- ✅ Procedimentos e serviços
- ✅ Prontuários médicos e prescrições
- ✅ Medicamentos e itens de prescrição
- ✅ Templates de prescrição e prontuário
- ✅ Pagamentos e faturamento
- ✅ Notificações multicanal (SMS, WhatsApp, Email)

## 📦 Dados Gerados

### 1. Clínica Demo
- **Nome**: Clínica Demo PrimeCare Software
- **TenantId**: `demo-clinic-001`
- **CNPJ**: 12.345.678/0001-95
- **Horário**: 08:00 - 18:00
- **Duração de consulta**: 30 minutos

### 2. Usuários (3)

| Username | Senha | Role | Email | Detalhes |
|----------|-------|------|-------|----------|
| `admin` | `Admin@123` | SystemAdmin | admin@clinicademo.com.br | Administrador do sistema |
| `dr.silva` | `Doctor@123` | Doctor | joao.silva@clinicademo.com.br | CRM-123456, Clínico Geral |
| `recep.maria` | `Recep@123` | Receptionist | maria.santos@clinicademo.com.br | Recepcionista |

### 3. Pacientes (6)

1. **Carlos Alberto Santos** - Hipertensão arterial controlada, Alergia: Penicilina
2. **Ana Maria Oliveira** - Diabetes tipo 2
3. **Pedro Henrique Costa** - Sem condições especiais
4. **Juliana Martins Silva** - Responsável legal (mãe)
5. **Lucas Martins Silva** - Criança (filho de Juliana), Asma leve
6. **Sofia Martins Silva** - Criança (filha de Juliana), Alergia: Lactose

### 4. Procedimentos (8)

| Código | Nome | Categoria | Preço | Duração |
|--------|------|-----------|-------|---------|
| CONS-001 | Consulta Médica Geral | Consultation | R$ 150,00 | 30 min |
| CONS-002 | Consulta Cardiológica | Consultation | R$ 250,00 | 45 min |
| EXAM-001 | Exame de Sangue Completo | Exam | R$ 80,00 | 15 min |
| EXAM-002 | Eletrocardiograma | Exam | R$ 120,00 | 20 min |
| VAC-001 | Vacina Influenza | Vaccination | R$ 50,00 | 10 min |
| THER-001 | Fisioterapia Sessão | Therapy | R$ 100,00 | 60 min |
| SURG-001 | Sutura Pequeno Porte | Surgery | R$ 200,00 | 30 min |
| RET-001 | Retorno Consulta | FollowUp | R$ 80,00 | 20 min |

### 5. Agendamentos (5)

- **2 consultas passadas** (concluídas com check-out)
  - Carlos - 7 dias atrás - Consulta de rotina
  - Ana - 5 dias atrás - Consulta cardiológica
- **1 consulta hoje** (confirmada)
  - Pedro - Hoje às 14:00 - Consulta médica
- **2 consultas futuras** (agendadas)
  - Lucas - Em 3 dias - Consulta pediátrica
  - Sofia - Em 3 dias - Consulta pediátrica

### 6. Pagamentos (2)

1. **Pagamento 1**: R$ 150,00 - Dinheiro - PAGO
   - Referente à consulta de Carlos
2. **Pagamento 2**: R$ 370,00 - Cartão de Crédito - PAGO
   - Referente à consulta cardiológica + ECG de Ana

### 7. Medicamentos (8)

1. **Amoxicilina 500mg** - Antibiótico (Cápsula)
2. **Dipirona Sódica 500mg** - Analgésico (Comprimido)
3. **Ibuprofeno 600mg** - Anti-inflamatório (Comprimido)
4. **Losartana Potássica 50mg** - Anti-hipertensivo (Comprimido)
5. **Omeprazol 20mg** - Antiácido (Cápsula)
6. **Loratadina 10mg** - Anti-histamínico (Comprimido)
7. **Metformina 850mg** - Antidiabético (Comprimido)
8. **Vitamina D3 7000 UI** - Vitamina (Cápsula)

### 8. Prontuários Médicos (2)

#### Prontuário 1 - Carlos
- **Diagnóstico**: Hipertensão arterial sistêmica (CID I10)
- **Prescrição**: Losartana 50mg + Dieta + Exercícios
- **Status**: Finalizado
- **Observações**: PA 120/80 mmHg, bom controle

#### Prontuário 2 - Ana
- **Diagnóstico**: Diabetes tipo 2 (CID E11) + Arritmia cardíaca (CID I49.9)
- **Prescrição**: Metformina 850mg + Omeprazol 20mg + Dieta
- **Status**: Finalizado
- **Observações**: Glicemia 145 mg/dL, ECG normal

### 9. Itens de Prescrição (3)

1. **Carlos**: Losartana Potássica 50mg - 30 dias
2. **Ana**: Metformina 850mg - 30 dias (60 comprimidos)
3. **Ana**: Omeprazol 20mg - 30 dias (30 cápsulas)

### 10. Templates de Prescrição (4)

1. **Receita Antibiótico Amoxicilina** - Categoria: Antibióticos
2. **Receita Anti-hipertensivo** - Categoria: Cardiologia
3. **Receita Analgésico Simples** - Categoria: Analgésicos
4. **Receita Diabetes** - Categoria: Endocrinologia

### 11. Templates de Prontuário (3)

1. **Consulta Clínica Geral** - Anamnese completa com exame físico
2. **Consulta Cardiológica** - Avaliação cardiovascular detalhada
3. **Consulta Pediátrica** - Acompanhamento de desenvolvimento infantil

### 12. Notificações (5)

1. **SMS para Carlos** - Lembrete de consulta - Entregue
2. **WhatsApp para Ana** - Lembrete de consulta - Lido
3. **SMS para Pedro** - Confirmação de consulta hoje - Enviado
4. **WhatsApp para Lucas** - Lembrete de consulta futura - Pendente
5. **Email para Carlos** - Confirmação de pagamento - Entregue

## 🚀 Como Usar

### 1. Verificar Informações Disponíveis

```bash
GET http://localhost:5000/api/data-seeder/demo-info
```

**Resposta**: Retorna um resumo de todos os dados que serão gerados.

### 2. Gerar Dados de Teste

```bash
POST http://localhost:5000/api/data-seeder/seed-demo
```

**Resposta**:
```json
{
  "message": "Demo data seeded successfully",
  "tenantId": "demo-clinic-001",
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
  ],
  "note": "Use these credentials to login and test the system"
}
```

### 3. Fazer Login

```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "dr.silva",
  "password": "Doctor@123"
}
```

**Resposta**: Retorna um token JWT para usar nas próximas requisições.

### 4. Testar Endpoints

Com o token JWT obtido no login, você pode testar todos os endpoints:

```bash
# Listar pacientes
GET http://localhost:5000/api/patients
Authorization: Bearer {token}

# Listar agendamentos
GET http://localhost:5000/api/appointments
Authorization: Bearer {token}

# Listar procedimentos
GET http://localhost:5000/api/procedures
Authorization: Bearer {token}

# Listar medicamentos
GET http://localhost:5000/api/medications
Authorization: Bearer {token}

# Listar prontuários
GET http://localhost:5000/api/medical-records
Authorization: Bearer {token}

# Listar notificações
GET http://localhost:5000/api/notifications
Authorization: Bearer {token}
```

## 🧪 Cenários de Teste Cobertos

### ✅ Gestão de Usuários
- [x] Login com diferentes roles (Admin, Doctor, Receptionist)
- [x] Controle de acesso por perfil
- [x] Médicos com CRM e especialidade

### ✅ Gestão de Pacientes
- [x] Pacientes adultos
- [x] Crianças com responsável legal
- [x] Vínculo de pacientes à clínica
- [x] Histórico médico e alergias

### ✅ Agendamentos
- [x] Agendamentos passados (concluídos)
- [x] Agendamentos atuais (hoje)
- [x] Agendamentos futuros
- [x] Estados: Agendado, Confirmado, Check-in, Check-out

### ✅ Procedimentos e Serviços
- [x] Diferentes categorias (Consulta, Exame, Vacina, etc.)
- [x] Preços e durações variadas
- [x] Vínculo de procedimentos a atendimentos

### ✅ Prontuários Médicos
- [x] Prontuários completos com diagnósticos
- [x] Prescrições médicas detalhadas
- [x] Observações clínicas
- [x] Vínculo com consultas

### ✅ Medicamentos e Prescrições
- [x] Cadastro de medicamentos comuns
- [x] Informações farmacêuticas completas
- [x] Itens de prescrição vinculados
- [x] Dosagens e instruções de uso

### ✅ Templates
- [x] Templates de prescrição personalizáveis
- [x] Templates de prontuário por especialidade
- [x] Placeholders para dados dinâmicos

### ✅ Pagamentos
- [x] Diferentes métodos de pagamento
- [x] Estados de pagamento (Pendente, Pago)
- [x] Vínculo com consultas

### ✅ Notificações
- [x] SMS, WhatsApp e Email
- [x] Estados: Pendente, Enviado, Entregue, Lido
- [x] Lembretes de consulta
- [x] Confirmações de pagamento

## ⚠️ Observações Importantes

1. **Dados de Demonstração**: Estes dados são **apenas para testes** e não devem ser usados em produção.

2. **Tenant Isolado**: Todos os dados são criados no tenant `demo-clinic-001` para garantir isolamento.

3. **Execução Única**: O endpoint `seed-demo` verifica se já existem dados para o tenant e retorna erro se houver duplicação.

4. **Limpar Dados**: Para gerar novos dados, você precisa limpar o banco de dados ou usar outro tenant.

5. **Senhas**: As senhas dos usuários de teste seguem a política de segurança do sistema (mínimo 8 caracteres, maiúscula, minúscula, número e caractere especial).

## 🔐 Segurança

- Todas as senhas são hashadas usando BCrypt
- Tokens JWT com expiração configurável
- Autenticação obrigatória para endpoints sensíveis
- Multi-tenant isolation garantido

## 📚 Documentação Adicional

Para mais informações sobre o sistema, consulte:
- [README.md](../README.md) - Documentação geral do projeto
- [IMPLEMENTACAO_FECHAMENTO_CONSULTA.md](IMPLEMENTACAO_FECHAMENTO_CONSULTA.md) - Detalhes sobre fechamento de consulta
- [SECURITY_GUIDE.md](SECURITY_GUIDE.md) - Guia de segurança completo
- [API_QUICK_GUIDE.md](API_QUICK_GUIDE.md) - Guia rápido da API

## 🎉 Conclusão

A carga inicial implementada fornece uma base completa e realista para testar todos os aspectos do sistema PrimeCare Software, desde o cadastro básico até fluxos complexos de atendimento médico com prescrições e notificações.
