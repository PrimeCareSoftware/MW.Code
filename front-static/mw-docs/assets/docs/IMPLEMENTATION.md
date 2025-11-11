# Implementação da Funcionalidade de Atendimento ao Paciente

## Resumo

Esta implementação adiciona uma tela completa de atendimento ao paciente com prontuário médico, timer de consulta, área de prescrição de medicamentos com opção de impressão, e visualização de calendário na agenda de consultas.

## Funcionalidades Implementadas

### 1. Backend (API .NET 8)

#### Entidades e Modelos
- **MedicalRecord Entity**: Nova entidade para armazenar prontuários médicos
  - Campos: Diagnóstico, Prescrição, Observações, Duração da consulta
  - Relacionamentos: Patient, Appointment
  - Rastreamento de tempo de início e fim da consulta

#### Controllers
- **MedicalRecordsController**: Novo controller com endpoints:
  - `POST /api/medical-records` - Criar prontuário
  - `PUT /api/medical-records/{id}` - Atualizar prontuário
  - `POST /api/medical-records/{id}/complete` - Finalizar atendimento
  - `GET /api/medical-records/appointment/{appointmentId}` - Buscar por agendamento
  - `GET /api/medical-records/patient/{patientId}` - Histórico do paciente

- **AppointmentsController**: Adicionado endpoint:
  - `GET /api/appointments/{id}` - Buscar agendamento por ID

#### Arquitetura CQRS
- Commands: CreateMedicalRecord, UpdateMedicalRecord, CompleteMedicalRecord
- Queries: GetMedicalRecordByAppointment, GetPatientMedicalRecords, GetAppointmentById
- Handlers correspondentes para cada comando e consulta

#### Repository Pattern
- **IMedicalRecordRepository**: Interface do repositório
- **MedicalRecordRepository**: Implementação com Entity Framework Core
- Configuração de relacionamentos e índices no banco de dados

### 2. Frontend (Angular 18)

#### Página de Atendimento (`/appointments/:appointmentId/attendance`)

**Componentes Principais:**

1. **Timer de Consulta**
   - Contador de tempo em tempo real (HH:MM:SS)
   - Inicia automaticamente ao criar o prontuário
   - Mantém contagem mesmo se a página for recarregada

2. **Informações do Paciente**
   - Nome, idade, CPF, telefone
   - Destaque para alergias (se houver)
   - Dados carregados automaticamente

3. **Histórico de Consultas**
   - Lista de consultas anteriores do paciente
   - Data, diagnóstico e duração de cada consulta
   - Scroll automático para consultas extensas

4. **Formulário de Prontuário**
   - Campo de Diagnóstico (textarea)
   - Campo de Prescrição Médica (textarea com fonte monoespaçada)
   - Campo de Observações (textarea)
   - Botão "Salvar Prontuário" - Salva sem finalizar
   - Botão "Finalizar Atendimento" - Completa e fecha a consulta

5. **Funcionalidade de Impressão**
   - Botão "Imprimir Receita" no campo de prescrição
   - Layout otimizado para impressão
   - Inclui nome do paciente e data da consulta

#### Lista de Agendamentos Atualizada

**Novas Funcionalidades:**

1. **Visualização em Calendário**
   - Toggle entre visualização em lista e calendário
   - Navegação entre meses (anterior/próximo)
   - Destaque para o dia atual
   - Indicador visual de dias com agendamentos
   - Clique no dia para selecionar e ver agendamentos

2. **Botões de Atendimento**
   - "Iniciar Atendimento" - Para agendamentos Scheduled/Confirmed
   - "Continuar Atendimento" - Para agendamentos InProgress
   - "Atendimento Concluído" - Badge para agendamentos Completed
   - Botão de cancelar desabilitado para consultas finalizadas

#### Services
- **MedicalRecordService**: Serviço para comunicação com API de prontuários
  - CRUD completo de prontuários
  - Busca por agendamento
  - Histórico do paciente

## Fluxo de Uso

1. **Acessar Agenda**
   - Usuário acessa `/appointments`
   - Pode alternar entre visualização de lista e calendário
   - Seleciona uma data específica

2. **Iniciar Atendimento**
   - Clica em "Iniciar Atendimento" no agendamento desejado
   - Sistema redireciona para `/appointments/{id}/attendance`
   - Timer inicia automaticamente
   - Prontuário médico é criado

3. **Durante o Atendimento**
   - Médico visualiza informações do paciente e histórico
   - Preenche diagnóstico, prescrição e observações
   - Pode salvar parcialmente durante a consulta
   - Timer continua contando

4. **Finalizar Atendimento**
   - Médico clica em "Finalizar Atendimento"
   - Sistema salva prontuário completo
   - Marca agendamento como Completed
   - Timer para
   - Redireciona para agenda

5. **Imprimir Receita**
   - Durante ou após o atendimento
   - Clica em "Imprimir Receita"
   - Sistema abre janela de impressão com layout formatado

## Estrutura de Arquivos

### Backend
```
src/MedicSoft.Domain/
  └── Entities/
      └── MedicalRecord.cs
  └── Interfaces/
      └── IMedicalRecordRepository.cs

src/MedicSoft.Application/
  └── Commands/MedicalRecords/
      ├── CreateMedicalRecordCommand.cs
      ├── UpdateMedicalRecordCommand.cs
      └── CompleteMedicalRecordCommand.cs
  └── Queries/MedicalRecords/
      ├── GetMedicalRecordByAppointmentQuery.cs
      └── GetPatientMedicalRecordsQuery.cs
  └── Handlers/
      ├── Commands/MedicalRecords/
      └── Queries/MedicalRecords/
  └── DTOs/
      └── MedicalRecordDto.cs
  └── Services/
      └── MedicalRecordService.cs

src/MedicSoft.Repository/
  └── Repositories/
      └── MedicalRecordRepository.cs
  └── Configurations/
      └── MedicalRecordConfiguration.cs

src/MedicSoft.Api/
  └── Controllers/
      └── MedicalRecordsController.cs
```

### Frontend
```
frontend/medicwarehouse-app/src/app/
  └── pages/
      └── attendance/
          ├── attendance.ts
          ├── attendance.html
          └── attendance.scss
      └── appointments/
          └── appointment-list/
              ├── appointment-list.ts (atualizado)
              ├── appointment-list.html (atualizado)
              └── appointment-list.scss (atualizado)
  └── models/
      └── medical-record.model.ts
  └── services/
      └── medical-record.ts
  └── app.routes.ts (atualizado)
```

## Tecnologias Utilizadas

### Backend
- .NET 8
- Entity Framework Core
- PostgreSQL
- MediatR (CQRS)
- AutoMapper
- Swagger/OpenAPI

### Frontend
- Angular 18
- TypeScript
- SCSS
- RxJS
- Standalone Components

## Próximos Passos

1. Adicionar validações mais robustas nos formulários
2. Implementar busca de medicamentos para prescrição
3. Adicionar assinatura digital para receitas
4. Exportar prontuário em PDF
5. Implementar notificações em tempo real
6. Adicionar suporte a anexos (exames, imagens)
7. Criar relatórios de atendimentos
8. Implementar backup automático de prontuários

## Notas de Desenvolvimento

- O sistema usa multitenancy por `TenantId` para isolamento de dados
- Todas as operações são auditadas com `CreatedAt` e `UpdatedAt`
- O timer usa `interval` do RxJS com limpeza adequada no `OnDestroy`
- A impressão usa CSS media queries para layout otimizado
- O calendário é gerado dinamicamente sem bibliotecas externas
- Backend segue princípios DDD e Clean Architecture

## Testes

Para testar a aplicação localmente:

```bash
# Backend
cd src/MedicSoft.Api
dotnet run

# Frontend
cd frontend/medicwarehouse-app
npm install
npm start

# Com Docker
docker-compose up --build
```

## Suporte

Para questões ou problemas, criar issue no repositório GitHub.
