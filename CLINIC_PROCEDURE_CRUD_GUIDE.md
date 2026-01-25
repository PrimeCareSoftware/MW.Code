# Guia de Gerenciamento de Clínicas e Procedimentos

## Visão Geral

Este documento descreve as funcionalidades de gerenciamento de clínicas e procedimentos para proprietários no sistema PrimeCare, implementado em Janeiro de 2026.

## 📋 Índice

1. [Gerenciamento de Clínicas](#gerenciamento-de-clínicas)
2. [Gerenciamento de Procedimentos](#gerenciamento-de-procedimentos)
3. [API Endpoints](#api-endpoints)
4. [Modelos de Dados](#modelos-de-dados)
5. [Validações e Regras de Negócio](#validações-e-regras-de-negócio)

---

## Gerenciamento de Clínicas

### Funcionalidades

Proprietários podem gerenciar múltiplas clínicas através da interface unificada localizada em **Administração da Clínica > Informações da Clínica**.

#### Criar Nova Clínica

1. Acesse "Informações da Clínica"
2. Clique no botão "+ Nova Clínica"
3. Preencha o formulário com os dados:
   - **Nome**: Nome oficial da clínica (obrigatório)
   - **Nome Fantasia**: Nome comercial (obrigatório)
   - **CPF/CNPJ**: Documento da clínica (obrigatório, validado)
   - **Telefone**: Número de contato (obrigatório)
   - **Email**: Email de contato (obrigatório, validado)
   - **Endereço**: Endereço completo (obrigatório)
   - **Horário de Abertura**: Hora de início do expediente
   - **Horário de Fechamento**: Hora de término do expediente
   - **Duração da Consulta**: Tempo padrão em minutos
   - **Número de Salas**: Quantidade de salas de atendimento
4. Clique em "Criar"

**Nota**: O proprietário é automaticamente vinculado como proprietário principal (100% de participação).

#### Editar Clínica Existente

1. Localize a clínica na lista "Minhas Clínicas"
2. Clique no botão "Editar"
3. Modifique os campos necessários:
   - Nome, nome fantasia, contatos e endereço
   - Horários de funcionamento
   - Configurações de atendimento
   - Permissão de slots de emergência
   - Notificações entre médicos
4. Clique em "Atualizar"

**Nota**: O documento (CPF/CNPJ) não pode ser alterado após a criação.

#### Visualizar Clínicas

A lista de clínicas mostra:
- Nome da clínica
- Status (Ativa/Inativa)
- Nome fantasia
- Documento
- Telefone e email
- Endereço

### Limites e Restrições

#### Limites do Plano de Assinatura

Cada plano tem um limite de clínicas (`MaxClinics`):
- **Trial/Basic**: 1 clínica
- **Standard**: 3 clínicas
- **Premium**: 5 clínicas
- **Enterprise**: 10 clínicas

Ao tentar criar uma clínica além do limite, o sistema exibe:
```
"Você atingiu o limite de X clínica(s) do seu plano. 
Faça upgrade do plano para adicionar mais clínicas."
```

#### Restrições de Operação

- **Deleção não permitida**: Clínicas não podem ser deletadas, apenas desativadas
- **Documento único**: Cada CPF/CNPJ só pode ser usado uma vez no sistema
- **Validação de documento**: Sistema valida formato e dígitos verificadores

---

## Gerenciamento de Procedimentos

### Funcionalidades

O sistema permite o pré-cadastro de procedimentos para seleção durante atendimentos médicos.

#### Criar Novo Procedimento

1. Acesse "Procedimentos" no menu
2. Clique em "Novo Procedimento"
3. Preencha os campos:
   - **Nome**: Nome do procedimento (ex: "Preenchimento Labial")
   - **Código**: Código único identificador
   - **Descrição**: Descrição detalhada do procedimento
   - **Categoria**: Tipo de procedimento (consulta, exame, cirurgia, etc.)
   - **Preço**: Valor do procedimento
   - **Duração**: Tempo estimado em minutos
   - **Requer Materiais**: Checkbox se utiliza materiais
   - **Clínica**: Opcional, para procedimento específico de uma clínica
   - **Convênios Aceitos**: Lista de convênios que cobrem o procedimento
   - **Permitir em Consulta Médica**: Habilita seleção durante consultas
   - **Permitir em Atendimento Exclusivo**: Habilita para procedimentos independentes

#### Editar Procedimento

1. Localize o procedimento na lista
2. Clique em "Editar"
3. Modifique os campos necessários
4. Salve as alterações

**Nota**: O código do procedimento não pode ser alterado após criação.

#### Desativar Procedimento

1. Localize o procedimento na lista
2. Clique em "Deletar" ou "Desativar"
3. O procedimento fica inativo mas permanece no histórico

### Categorias de Procedimentos

- **Consultation**: Consulta médica
- **Exam**: Exame diagnóstico
- **Surgery**: Cirurgia
- **Therapy**: Terapia
- **Vaccination**: Vacinação
- **Diagnostic**: Diagnóstico
- **Treatment**: Tratamento
- **Emergency**: Emergência
- **Prevention**: Prevenção
- **Aesthetic**: Estética
- **FollowUp**: Retorno
- **Other**: Outros

### Uso Durante Atendimento

Durante uma consulta médica, o profissional pode:
1. Selecionar um ou mais procedimentos pré-cadastrados
2. Ajustar o preço cobrado (se necessário)
3. Adicionar observações específicas
4. Os procedimentos são automaticamente vinculados ao atendimento

---

## API Endpoints

### Clínicas

#### GET /api/owner-clinics
Retorna todas as clínicas do proprietário autenticado.

**Permissões**: `clinic.view`

**Response**:
```json
[
  {
    "id": "guid",
    "name": "Clínica XYZ",
    "tradeName": "Clínica XYZ - Especialidades",
    "document": "12.345.678/0001-90",
    "phone": "(11) 1234-5678",
    "email": "contato@clinicaxyz.com.br",
    "address": "Rua ABC, 123 - Centro",
    "openingTime": "08:00",
    "closingTime": "18:00",
    "appointmentDurationMinutes": 30,
    "isActive": true
  }
]
```

#### GET /api/owner-clinics/{id}
Retorna uma clínica específica.

**Permissões**: `clinic.view`

**Response**: Objeto ClinicDto

#### POST /api/owner-clinics
Cria uma nova clínica.

**Permissões**: `clinic.edit`

**Request Body**:
```json
{
  "name": "Nova Clínica",
  "tradeName": "Nova Clínica - Médica",
  "document": "12.345.678/0001-90",
  "phone": "(11) 1234-5678",
  "email": "contato@novaclinica.com.br",
  "address": "Rua DEF, 456 - Bairro",
  "openingTime": "08:00",
  "closingTime": "18:00",
  "appointmentDurationMinutes": 30
}
```

**Validações**:
- Verifica limite de clínicas do plano
- Valida unicidade do documento
- Valida formato CPF/CNPJ

**Response**: 201 Created com objeto ClinicDto

#### PUT /api/owner-clinics/{id}
Atualiza uma clínica existente.

**Permissões**: `clinic.edit`

**Request Body**:
```json
{
  "name": "Clínica Atualizada",
  "tradeName": "Clínica Atualizada - Médica",
  "phone": "(11) 9999-8888",
  "email": "novo@clinica.com.br",
  "address": "Novo Endereço",
  "openingTime": "07:00",
  "closingTime": "19:00",
  "appointmentDurationMinutes": 45,
  "allowEmergencySlots": true,
  "numberOfRooms": 3
}
```

**Response**: 200 OK com objeto ClinicDto atualizado

### Procedimentos

#### GET /api/procedures
Retorna todos os procedimentos.

**Query Parameters**:
- `activeOnly` (bool): Filtrar apenas ativos

**Permissões**: `procedures.view`

**Response**: Array de ProcedureDto

#### GET /api/procedures/{id}
Retorna um procedimento específico.

**Permissões**: `procedures.view`

#### POST /api/procedures
Cria um novo procedimento.

**Permissões**: `procedures.create`

**Request Body**:
```json
{
  "name": "Preenchimento Labial",
  "code": "PROC-001",
  "description": "Procedimento estético de preenchimento labial com ácido hialurônico",
  "category": 9,
  "price": 1500.00,
  "durationMinutes": 60,
  "requiresMaterials": true,
  "clinicId": "guid-optional",
  "acceptedHealthInsurances": "Unimed, Amil",
  "allowInMedicalAttendance": true,
  "allowInExclusiveProcedureAttendance": true
}
```

#### PUT /api/procedures/{id}
Atualiza um procedimento existente.

**Permissões**: `procedures.edit`

**Nota**: O campo `code` não pode ser atualizado.

#### DELETE /api/procedures/{id}
Desativa um procedimento.

**Permissões**: `procedures.delete`

**Nota**: Soft delete - o procedimento é marcado como inativo.

---

## Modelos de Dados

### SubscriptionPlan

```csharp
public class SubscriptionPlan : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public int TrialDays { get; private set; }
    public int MaxUsers { get; private set; }
    public int MaxPatients { get; private set; }
    public int MaxClinics { get; private set; }  // NOVO
    public bool HasReports { get; private set; }
    // ... outros campos
}
```

### Clinic

```csharp
public class Clinic : BaseEntity
{
    public Guid? CompanyId { get; private set; }
    public string Name { get; private set; }
    public string TradeName { get; private set; }
    public string Document { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }
    public TimeSpan OpeningTime { get; private set; }
    public TimeSpan ClosingTime { get; private set; }
    public int AppointmentDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }
    // ... outros campos
}
```

### Procedure

```csharp
public class Procedure : BaseEntity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public ProcedureCategory Category { get; private set; }
    public decimal Price { get; private set; }
    public int DurationMinutes { get; private set; }
    public bool RequiresMaterials { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? ClinicId { get; private set; }  // NOVO
    public string? AcceptedHealthInsurances { get; private set; }  // NOVO
    public bool AllowInMedicalAttendance { get; private set; }  // NOVO
    public bool AllowInExclusiveProcedureAttendance { get; private set; }  // NOVO
}
```

---

## Validações e Regras de Negócio

### Clínicas

1. **Limite de Clínicas**
   - Verificado no comando CreateClinicCommand
   - Conta clínicas ativas do proprietário
   - Compara com MaxClinics do plano
   - Lança exceção se limite atingido

2. **Documento Único**
   - CPF/CNPJ deve ser único no sistema
   - Validação de formato e dígitos verificadores
   - Não pode ser alterado após criação

3. **Vinculação Automática**
   - Ao criar clínica, cria OwnerClinicLink automaticamente
   - Proprietário definido como principal (isPrimary = true)
   - Participação de 100%

4. **Horários**
   - OpeningTime deve ser antes de ClosingTime
   - Duração da consulta deve ser > 0

### Procedimentos

1. **Código Único**
   - Cada procedimento tem código único
   - Não pode ser alterado após criação

2. **Preço e Duração**
   - Preço não pode ser negativo
   - Duração deve ser entre 5 e 480 minutos

3. **Ativação/Desativação**
   - Procedimentos são marcados como inativos (soft delete)
   - Mantidos no histórico para referência

4. **Clínica Específica**
   - Se ClinicId for fornecido, procedimento é específico daquela clínica
   - Se ClinicId for null, procedimento é compartilhado

---

## Migrações de Banco de Dados

### Migration: AddMaxClinicsToSubscriptionPlan

**Arquivo**: `20260125193339_AddMaxClinicsToSubscriptionPlan.cs`

**Alterações**:
```sql
ALTER TABLE "SubscriptionPlans" 
ADD COLUMN "MaxClinics" integer NOT NULL DEFAULT 1;
```

**Rollback**:
```sql
ALTER TABLE "SubscriptionPlans" 
DROP COLUMN "MaxClinics";
```

---

## Segurança

### Permissões Necessárias

- **clinic.view**: Visualizar clínicas
- **clinic.edit**: Criar/editar clínicas
- **procedures.view**: Visualizar procedimentos
- **procedures.create**: Criar procedimentos
- **procedures.edit**: Editar procedimentos
- **procedures.delete**: Desativar procedimentos

### Validações de Segurança

1. Verificação de ownership no backend
2. Validação de tenant em todas as operações
3. Claims JWT verificados via `owner_id`
4. Limite de recursos por plano

---

## Suporte e Contato

Para dúvidas ou sugestões sobre estas funcionalidades, entre em contato com a equipe de desenvolvimento.

**Versão do Documento**: 1.0  
**Data**: Janeiro 2026  
**Autor**: PrimeCare Development Team
