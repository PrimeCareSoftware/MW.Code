# Guia de Migração: Sistema de Chamados (Tickets)

## Visão Geral

Este guia documenta a migração do sistema de chamados (tickets) do microserviço **SystemAdmin** para a API principal **MedicSoft.Api**.

## Motivação da Migração

A migração foi realizada para:
- Consolidar funcionalidades relacionadas em um único serviço
- Simplificar a arquitetura eliminando um microserviço específico
- Facilitar a manutenção e evolução do sistema
- Reduzir a complexidade operacional e de deployment

## O Que Foi Migrado

### Entidades de Domínio
- `Ticket` - Representa um chamado de suporte
- `TicketComment` - Comentários em chamados
- `TicketAttachment` - Anexos de imagens
- `TicketHistory` - Histórico de mudanças de status

### Funcionalidades
- ✅ Criação de chamados por usuários
- ✅ Visualização e edição de chamados
- ✅ Sistema de comentários
- ✅ Upload de anexos (imagens)
- ✅ Atribuição de tickets para System Owners
- ✅ Rastreamento de status e histórico
- ✅ Estatísticas e métricas
- ✅ Filtros e buscas

## Estrutura do Código Migrado

### Domain Layer (`src/MedicSoft.Domain`)
```
Entities/
├── Ticket.cs                  # Entidade principal do chamado
├── TicketComment.cs           # Comentários
├── TicketAttachment.cs        # Anexos
└── TicketHistory.cs           # Histórico de status

Interfaces/
└── ITicketRepository.cs       # Interface do repositório
```

### Repository Layer (`src/MedicSoft.Repository`)
```
Repositories/
└── TicketRepository.cs        # Implementação do repositório

Configurations/
├── TicketConfiguration.cs
├── TicketCommentConfiguration.cs
├── TicketAttachmentConfiguration.cs
└── TicketHistoryConfiguration.cs

Migrations/PostgreSQL/
└── 20251221154116_AddTicketSystem.cs  # Migration EF Core
```

### Application Layer (`src/MedicSoft.Application`)
```
DTOs/
└── TicketDtos.cs             # DTOs para requests/responses

Services/
├── ITicketService.cs         # Interface do serviço
└── TicketService.cs          # Implementação do serviço
```

### API Layer (`src/MedicSoft.Api`)
```
Controllers/
└── TicketsController.cs      # Endpoints REST
```

## Banco de Dados

### Tabelas Criadas

#### Tickets
Tabela principal que armazena os chamados.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | uuid | Identificador único |
| Title | varchar(200) | Título do chamado |
| Description | text | Descrição detalhada |
| Type | integer | Tipo (0-6) |
| Status | integer | Status (0-5) |
| Priority | integer | Prioridade (0-3) |
| UserId | uuid | ID do usuário criador |
| UserName | varchar(200) | Nome do usuário |
| UserEmail | varchar(250) | Email do usuário |
| ClinicId | uuid | ID da clínica (opcional) |
| ClinicName | varchar(200) | Nome da clínica |
| AssignedToId | uuid | ID do responsável |
| AssignedToName | varchar(200) | Nome do responsável |
| TenantId | varchar(100) | Tenant ID |
| CreatedAt | timestamp | Data de criação |
| UpdatedAt | timestamp | Data de atualização |
| LastStatusChangeAt | timestamp | Última mudança de status |

**Índices:**
- `IX_Tickets_TenantId`
- `IX_Tickets_TenantId_UserId`
- `IX_Tickets_TenantId_ClinicId`
- `IX_Tickets_Status_TenantId`

#### TicketComments
Armazena comentários nos chamados.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | uuid | Identificador único |
| TicketId | uuid | ID do chamado |
| Comment | text | Texto do comentário |
| AuthorId | uuid | ID do autor |
| AuthorName | varchar(200) | Nome do autor |
| IsInternal | boolean | Comentário interno? |
| IsSystemOwner | boolean | Autor é System Owner? |
| TenantId | varchar(100) | Tenant ID |
| CreatedAt | timestamp | Data de criação |

**Índices:**
- `IX_TicketComments_TicketId`
- `IX_TicketComments_TenantId`

#### TicketAttachments
Armazena anexos (imagens) dos chamados.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | uuid | Identificador único |
| TicketId | uuid | ID do chamado |
| FileName | varchar(255) | Nome do arquivo |
| FileUrl | varchar(500) | URL do arquivo |
| ContentType | varchar(100) | Tipo MIME |
| FileSize | bigint | Tamanho em bytes |
| UploadedAt | timestamp | Data de upload |
| TenantId | varchar(100) | Tenant ID |
| CreatedAt | timestamp | Data de criação |

**Índices:**
- `IX_TicketAttachments_TicketId`
- `IX_TicketAttachments_TenantId`

#### TicketHistory
Armazena histórico de mudanças de status.

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | uuid | Identificador único |
| TicketId | uuid | ID do chamado |
| OldStatus | integer | Status anterior |
| NewStatus | integer | Novo status |
| ChangedById | uuid | ID de quem alterou |
| ChangedByName | varchar(200) | Nome de quem alterou |
| Comment | text | Comentário da mudança |
| ChangedAt | timestamp | Data da mudança |
| TenantId | varchar(100) | Tenant ID |
| CreatedAt | timestamp | Data de criação |

**Índices:**
- `IX_TicketHistory_TicketId`
- `IX_TicketHistory_TenantId`

## Como Executar a Migração

### Opção 1: EF Core Migration (Recomendado)

```bash
cd src/MedicSoft.Repository
dotnet ef database update --startup-project ../MedicSoft.Api/MedicSoft.Api.csproj
```

Esta opção:
- ✅ Aplica todas as migrations pendentes automaticamente
- ✅ Mantém o histórico de migrations
- ✅ É reversível com `dotnet ef database update PreviousMigration`

### Opção 2: Script SQL Direto

```bash
# Usando o script fornecido
./scripts/run-ticket-migration.sh "Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword"

# Ou aplicar manualmente
psql -h localhost -U postgres -d medicsoft -f scripts/migrations/20251221_add_ticket_system.sql
```

Esta opção:
- ✅ Útil para ambientes onde EF Core não está disponível
- ✅ Pode ser executada diretamente no banco
- ⚠️ Não mantém histórico no `__EFMigrationsHistory`

## Validação Pós-Migração

Execute as seguintes validações após a migração:

### 1. Verificar Tabelas Criadas

```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_name IN ('Tickets', 'TicketComments', 'TicketAttachments', 'TicketHistory');
```

Resultado esperado: 4 tabelas

### 2. Verificar Índices

```sql
SELECT tablename, indexname 
FROM pg_indexes 
WHERE tablename LIKE 'Ticket%' 
ORDER BY tablename, indexname;
```

Resultado esperado: ~10 índices

### 3. Testar API Endpoints

```bash
# Criar um ticket de teste
curl -X POST http://localhost:5000/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Ticket de teste",
    "description": "Teste pós-migração",
    "type": 4,
    "priority": 1
  }'

# Listar tickets
curl -X GET http://localhost:5000/api/tickets/my-tickets \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 4. Verificar Logs da Aplicação

Inicie a aplicação e verifique se não há erros relacionados ao Ticket:

```bash
cd src/MedicSoft.Api
dotnet run
```

Procure por:
- ✅ "Database migrations applied successfully"
- ✅ Nenhum erro relacionado a `Ticket` ou `TicketRepository`
- ✅ Swagger UI carrega endpoints `/api/tickets/*`

## Rollback (Se Necessário)

### Rollback via EF Core

```bash
cd src/MedicSoft.Repository
dotnet ef database update AddSessionTables --startup-project ../MedicSoft.Api/MedicSoft.Api.csproj
```

Isso irá desfazer a migration `AddTicketSystem`.

### Rollback via SQL

```sql
-- Remover tabelas na ordem correta (devido às foreign keys)
DROP TABLE IF EXISTS "TicketHistory" CASCADE;
DROP TABLE IF EXISTS "TicketAttachments" CASCADE;
DROP TABLE IF EXISTS "TicketComments" CASCADE;
DROP TABLE IF EXISTS "Tickets" CASCADE;
```

## Migração de Dados Existentes

Se você tinha dados no microserviço SystemAdmin e precisa migrá-los:

### 1. Exportar Dados do SystemAdmin

```bash
# Conectar ao banco do SystemAdmin
psql -h systemadmin-host -U postgres -d systemadmin_db

# Exportar dados
\copy (SELECT * FROM "Tickets") TO '/tmp/tickets.csv' CSV HEADER;
\copy (SELECT * FROM "TicketComments") TO '/tmp/ticket_comments.csv' CSV HEADER;
\copy (SELECT * FROM "TicketAttachments") TO '/tmp/ticket_attachments.csv' CSV HEADER;
\copy (SELECT * FROM "TicketHistory") TO '/tmp/ticket_history.csv' CSV HEADER;
```

### 2. Importar para MedicSoft.Api

```bash
# Conectar ao banco principal
psql -h localhost -U postgres -d medicsoft

# Importar dados
\copy "Tickets" FROM '/tmp/tickets.csv' CSV HEADER;
\copy "TicketComments" FROM '/tmp/ticket_comments.csv' CSV HEADER;
\copy "TicketAttachments" FROM '/tmp/ticket_attachments.csv' CSV HEADER;
\copy "TicketHistory" FROM '/tmp/ticket_history.csv' CSV HEADER;
```

### 3. Validar Integridade

```sql
-- Verificar contagens
SELECT COUNT(*) FROM "Tickets";
SELECT COUNT(*) FROM "TicketComments";
SELECT COUNT(*) FROM "TicketAttachments";
SELECT COUNT(*) FROM "TicketHistory";

-- Verificar foreign keys
SELECT COUNT(*) 
FROM "TicketComments" c 
LEFT JOIN "Tickets" t ON c."TicketId" = t."Id" 
WHERE t."Id" IS NULL;
-- Deve retornar 0
```

## Atualizações no Frontend

Se você está usando o frontend Angular, atualize os serviços para apontar para a nova API:

```typescript
// Antes (microserviço)
const SYSTEMADMIN_API = 'http://localhost:5006/api';

// Depois (API principal)
const API_BASE = 'http://localhost:5000/api';

// Endpoints permanecem os mesmos
GET  /api/tickets/my-tickets
POST /api/tickets
GET  /api/tickets/{id}
// etc.
```

## Troubleshooting

### Erro: "Tickets table already exists"

A migration já foi aplicada. Você pode:
1. Verificar se a tabela está correta
2. Ou fazer rollback e reaplicar

### Erro: "Cannot add foreign key constraint"

Verifique se a tabela `Tickets` foi criada antes das tabelas dependentes.

### Erro: 401 Unauthorized nos endpoints

Verifique:
- Token JWT está sendo enviado no header `Authorization`
- Token não está expirado
- Usuário tem permissões adequadas

### Performance lenta nas consultas

Verifique se os índices foram criados:

```sql
SELECT indexname FROM pg_indexes WHERE tablename = 'Tickets';
```

## Próximos Passos

Após a migração bem-sucedida:

1. ✅ Teste todos os endpoints da API
2. ✅ Atualize o frontend para usar a nova API
3. ✅ Execute testes de integração
4. ✅ Monitore logs para erros
5. ✅ Considere desativar o microserviço SystemAdmin antigo (após período de transição)

## Suporte

Para problemas ou dúvidas sobre a migração:
- Consulte a [documentação da API](TICKET_API_DOCUMENTATION.md)
- Verifique os logs da aplicação
- Crie um ticket através do próprio sistema migrado! 😊
