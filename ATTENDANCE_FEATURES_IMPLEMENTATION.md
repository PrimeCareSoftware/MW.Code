# Implementação de Funcionalidades na Página de Consulta (Attendance)

## Resumo das Alterações

Este documento descreve as novas funcionalidades implementadas na página de consulta/atendimento do sistema MedicWarehouse, conforme solicitado.

## Funcionalidades Implementadas

### 1. Gestão de Procedimentos durante o Atendimento

**Localização:** Página de Atendimento (`/frontend/medicwarehouse-app/src/app/pages/attendance/`)

#### Recursos:
- **Adicionar Procedimentos**: Médicos e dentistas podem adicionar procedimentos realizados durante a consulta
- **Seleção de Procedimentos**: Lista de procedimentos disponíveis cadastrados na clínica
- **Preço Customizado**: Opção de definir preço diferente do padrão para casos especiais
- **Observações**: Campo para adicionar notas específicas sobre o procedimento
- **Visualização de Custos**: Exibição do total dos procedimentos realizados

#### Backend:
- **Entidade**: `AppointmentProcedure` (já existente)
- **Controlador**: `ProceduresController`
- **Endpoints**:
  - `POST /api/procedures/appointments/{appointmentId}/procedures` - Adicionar procedimento
  - `GET /api/procedures/appointments/{appointmentId}/procedures` - Listar procedimentos
  - `GET /api/procedures/appointments/{appointmentId}/billing-summary` - Resumo de cobrança

#### Frontend:
- **Models**: `/frontend/medicwarehouse-app/src/app/models/procedure.model.ts`
- **Service**: `/frontend/medicwarehouse-app/src/app/services/procedure.ts`
- **Componente**: Integrado em `attendance.ts` e `attendance.html`

### 2. Solicitação de Exames

**Localização:** Página de Atendimento (`/frontend/medicwarehouse-app/src/app/pages/attendance/`)

#### Recursos:
- **Criar Pedidos de Exame**: Solicitação de exames laboratoriais, imagens, cardíacos, etc.
- **Tipos de Exame**: 
  - Laboratorial
  - Imagem (Raio-X, Tomografia, etc)
  - Cardíaco (ECG, Ecocardiograma, etc)
  - Endoscopia
  - Biópsia
  - Ultrassom
  - Outros
- **Níveis de Urgência**:
  - Rotina
  - Urgente
  - Emergência
- **Gestão de Status**:
  - Pendente
  - Agendado
  - Em andamento
  - Concluído
  - Cancelado

#### Backend:
- **Entidade**: `ExamRequest` (novo)
  - Localização: `/src/MedicSoft.Domain/Entities/ExamRequest.cs`
- **DTOs**: `/src/MedicSoft.Application/DTOs/ExamRequestDto.cs`
- **Repository**: 
  - Interface: `/src/MedicSoft.Domain/Interfaces/IExamRequestRepository.cs`
  - Implementação: `/src/MedicSoft.Repository/Repositories/ExamRequestRepository.cs`
- **Controlador**: `/src/MedicSoft.Api/Controllers/ExamRequestsController.cs`
- **Endpoints**:
  - `POST /api/exam-requests` - Criar pedido de exame
  - `PUT /api/exam-requests/{id}` - Atualizar pedido
  - `POST /api/exam-requests/{id}/complete` - Marcar como concluído
  - `POST /api/exam-requests/{id}/cancel` - Cancelar pedido
  - `GET /api/exam-requests/{id}` - Obter por ID
  - `GET /api/exam-requests/appointment/{appointmentId}` - Listar por consulta
  - `GET /api/exam-requests/patient/{patientId}` - Listar por paciente
  - `GET /api/exam-requests/pending` - Listar pendentes
  - `GET /api/exam-requests/urgent` - Listar urgentes

#### Frontend:
- **Models**: `/frontend/medicwarehouse-app/src/app/models/exam-request.model.ts`
- **Service**: `/frontend/medicwarehouse-app/src/app/services/exam-request.ts`
- **Componente**: Integrado em `attendance.ts` e `attendance.html`

## Arquitetura e Padrões

### Backend (C# / .NET 8)

#### Domain Layer
- **Entities**: Entidades de domínio com lógica de negócio encapsulada
- **Value Objects**: Objetos imutáveis para valores complexos
- **Repositories**: Interfaces para acesso a dados

#### Application Layer
- **DTOs**: Data Transfer Objects para comunicação entre camadas
- **Services**: Lógica de aplicação e orquestração
- **Commands/Queries**: Padrão CQRS com MediatR (preparado, mas não implementado completamente)

#### Infrastructure Layer
- **Repository**: Implementações concretas usando Entity Framework Core
- **Configurations**: Configurações de entidades para o EF Core

#### API Layer
- **Controllers**: Endpoints REST seguindo convenções RESTful
- **Dependency Injection**: Registro de serviços no `Program.cs`

### Frontend (Angular 18)

#### Estrutura
- **Models**: Interfaces TypeScript para tipagem forte
- **Services**: Serviços injetáveis para comunicação com API
- **Components**: Componentes standalone com signals do Angular
- **Reactive Forms**: Formulários reativos para validação

## Configuração e Uso

### Configuração do Backend

1. **Adicionar Migration** (necessário para criar tabela ExamRequests):
```bash
cd src/MedicSoft.Api
dotnet ef migrations add AddExamRequestEntity
dotnet ef database update
```

2. **Configurar Connection String** em `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MedicWarehouse;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

3. **Executar API**:
```bash
cd src/MedicSoft.Api
dotnet run
```

### Configuração do Frontend

1. **Instalar Dependências**:
```bash
cd frontend/medicwarehouse-app
npm install
```

2. **Executar Aplicação**:
```bash
ng serve
```

3. **Acessar**: http://localhost:4200

## Fluxo de Uso

### Adicionando Procedimentos

1. Acesse uma consulta em andamento
2. Na seção "Procedimentos Realizados", clique em "+ Adicionar Procedimento"
3. Selecione o procedimento da lista
4. (Opcional) Defina um preço customizado
5. (Opcional) Adicione observações
6. Clique em "Adicionar"
7. O procedimento aparecerá na lista com o valor total atualizado

### Solicitando Exames

1. Na página de atendimento, vá até "Pedidos de Exames"
2. Clique em "+ Solicitar Exame"
3. Preencha:
   - Tipo de exame (ex: Laboratorial, Imagem)
   - Nome do exame (ex: "Hemograma Completo")
   - Descrição/Justificativa
   - Urgência (Rotina, Urgente, Emergência)
   - Observações adicionais (opcional)
4. Clique em "Adicionar Pedido"
5. O exame aparecerá na lista com badges de tipo e urgência

### Finalizando Atendimento

1. Preencha o diagnóstico
2. Adicione prescrição médica
3. Adicione procedimentos realizados
4. Solicite exames necessários
5. Adicione observações gerais
6. Clique em "Finalizar Atendimento"

## Características Opcionais/Condicionais

Conforme solicitado, todas as funcionalidades são **opcionais e condicionais**:

- ✅ Não é obrigatório adicionar procedimentos
- ✅ Não é obrigatório solicitar exames
- ✅ Médico/dentista decide o que usar durante o atendimento
- ✅ Interface adaptativa mostra/esconde formulários conforme necessário
- ✅ Sistema mantém histórico completo para consultas futuras

## Validações e Regras de Negócio

### Procedimentos
- Procedimento deve existir e estar ativo
- Preço customizado não pode ser negativo
- Apenas profissionais autorizados podem adicionar procedimentos

### Exames
- Nome e descrição são obrigatórios
- Paciente e consulta devem existir
- Status segue fluxo: Pendente → Agendado → Em Andamento → Concluído
- Exames concluídos não podem ser cancelados
- Exames urgentes e emergenciais são destacados na interface

## Segurança

- ✅ Multi-tenancy: Todas as entidades isoladas por TenantId
- ✅ Autenticação JWT requerida
- ✅ Validação de permissões por role
- ✅ Validação de entrada em todos os endpoints
- ✅ Proteção contra SQL Injection (Entity Framework)

## Melhorias Futuras Sugeridas

1. **Notificações**:
   - Notificar paciente quando exame for agendado
   - Alertar sobre exames urgentes pendentes

2. **Integração com Laboratórios**:
   - API para enviar pedidos diretamente
   - Receber resultados automaticamente

3. **Templates de Exames**:
   - Grupos de exames pré-definidos
   - Exames frequentes por especialidade

4. **Relatórios**:
   - Dashboard de exames pendentes
   - Relatório de procedimentos realizados
   - Análise de custos por paciente/período

5. **Impressão**:
   - Pedidos de exame formatados para impressão
   - QR Code para rastreamento

## Testes

### Backend
```bash
cd tests/MedicSoft.Test
dotnet test
```

### Frontend
```bash
cd frontend/medicwarehouse-app
ng test
```

## Suporte

Para dúvidas ou problemas:
1. Consulte a documentação da API em: http://localhost:5000/swagger
2. Verifique os logs da aplicação
3. Entre em contato com a equipe de desenvolvimento

## Conclusão

A implementação adiciona funcionalidades essenciais para o fluxo de atendimento médico/odontológico, mantendo a flexibilidade e opcionalidade conforme solicitado. Todas as funcionalidades são condicionais e o médico/dentista tem liberdade para usar ou não durante a consulta.
