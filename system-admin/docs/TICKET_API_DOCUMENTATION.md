# Sistema de Chamados (Tickets) - Documentação da API

## Visão Geral

O sistema de chamados (tickets) foi migrado do microserviço SystemAdmin para a API principal MedicSoft.Api. Este sistema permite que usuários criem tickets de suporte para relatar bugs, solicitar recursos e obter assistência técnica.

## Endpoints da API

Base URL: `/api/tickets`

### 1. Criar Chamado
**POST** `/api/tickets`

Cria um novo chamado de suporte.

**Headers:**
- `Authorization: Bearer {token}` (obrigatório)
- `Content-Type: application/json`

**Request Body:**
```json
{
  "title": "Título do chamado",
  "description": "Descrição detalhada do problema ou solicitação",
  "type": 0,
  "priority": 1
}
```

**Tipos de Chamado (Type):**
- `0` - BugReport (Relatório de Bug)
- `1` - FeatureRequest (Solicitação de Recurso)
- `2` - SystemAdjustment (Ajuste no Sistema)
- `3` - FinancialIssue (Problema Financeiro)
- `4` - TechnicalSupport (Suporte Técnico)
- `5` - UserSupport (Suporte ao Usuário)
- `6` - Other (Outro)

**Prioridades (Priority):**
- `0` - Low (Baixa)
- `1` - Medium (Média)
- `2` - High (Alta)
- `3` - Critical (Crítica)

**Response:** `200 OK`
```json
{
  "message": "Chamado criado com sucesso",
  "ticketId": "guid"
}
```

### 2. Obter Chamado por ID
**GET** `/api/tickets/{id}`

Retorna os detalhes completos de um chamado específico.

**Response:** `200 OK`
```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "type": 0,
  "status": 0,
  "priority": 1,
  "userId": "guid",
  "userName": "string",
  "userEmail": "string",
  "clinicId": "guid",
  "clinicName": "string",
  "assignedToId": "guid",
  "assignedToName": "string",
  "comments": [],
  "attachments": [],
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "lastStatusChangeAt": "datetime"
}
```

### 3. Listar Meus Chamados
**GET** `/api/tickets/my-tickets`

Retorna todos os chamados do usuário atual.

**Response:** `200 OK`
```json
[
  {
    "id": "guid",
    "title": "string",
    "type": 0,
    "status": 0,
    "priority": 1,
    "userName": "string",
    "clinicName": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
  }
]
```

### 4. Listar Chamados por Clínica
**GET** `/api/tickets/clinic/{clinicId}`

Retorna chamados de uma clínica específica.

**Permissões:** System Owner ou usuários da clínica

### 5. Listar Todos os Chamados (Admin)
**GET** `/api/tickets`

**Permissões:** Apenas System Owners

**Query Parameters:**
- `status` (opcional): Filtrar por status (0-5)
- `type` (opcional): Filtrar por tipo (0-6)
- `clinicId` (opcional): Filtrar por clínica
- `tenantId` (opcional): Filtrar por tenant

### 6. Atualizar Chamado
**PUT** `/api/tickets/{id}`

Atualiza os detalhes de um chamado.

**Permissões:** Proprietário do chamado ou System Owner

**Request Body:**
```json
{
  "title": "string",
  "description": "string",
  "type": 0,
  "priority": 1
}
```

### 7. Atualizar Status do Chamado
**PUT** `/api/tickets/{id}/status`

Atualiza o status de um chamado.

**Request Body:**
```json
{
  "status": 2,
  "comment": "Comentário opcional sobre a mudança de status"
}
```

**Status Disponíveis:**
- `0` - Open (Aberto)
- `1` - InAnalysis (Em Análise)
- `2` - InProgress (Em Progresso)
- `3` - Blocked (Bloqueado)
- `4` - Completed (Concluído)
- `5` - Cancelled (Cancelado)

### 8. Atribuir Chamado
**PUT** `/api/tickets/{id}/assign`

Atribui um chamado a um System Owner.

**Permissões:** Apenas System Owners

**Request Body:**
```json
{
  "assignedToId": "guid"
}
```

### 9. Adicionar Comentário
**POST** `/api/tickets/{id}/comments`

Adiciona um comentário ao chamado.

**Request Body:**
```json
{
  "comment": "Texto do comentário",
  "isInternal": false
}
```

**Nota:** Comentários internos (`isInternal: true`) são visíveis apenas para System Owners.

### 10. Adicionar Anexo
**POST** `/api/tickets/{id}/attachments`

Adiciona um anexo de imagem ao chamado.

**Request Body:**
```json
{
  "fileName": "screenshot.png",
  "base64Data": "base64_encoded_image_data",
  "contentType": "image/png"
}
```

**Formatos Aceitos:**
- image/jpeg, image/jpg
- image/png
- image/gif
- image/webp

**Tamanho Máximo:** 5 MB

### 11. Obter Contagem de Atualizações Não Lidas
**GET** `/api/tickets/unread-count`

Retorna o número de chamados com atualizações não lidas.

**Response:** `200 OK`
```json
{
  "count": 0
}
```

### 12. Marcar Chamado como Lido
**POST** `/api/tickets/{id}/mark-read`

Marca um chamado como lido pelo usuário atual.

### 13. Obter Estatísticas
**GET** `/api/tickets/statistics`

Retorna estatísticas agregadas sobre os chamados.

**Permissões:** Apenas System Owners

**Query Parameters:**
- `clinicId` (opcional): Filtrar por clínica
- `tenantId` (opcional): Filtrar por tenant

**Response:** `200 OK`
```json
{
  "totalTickets": 100,
  "openTickets": 20,
  "inAnalysisTickets": 15,
  "inProgressTickets": 30,
  "blockedTickets": 5,
  "completedTickets": 25,
  "cancelledTickets": 5,
  "ticketsByType": {
    "BugReport": 30,
    "FeatureRequest": 25,
    "TechnicalSupport": 45
  },
  "ticketsByPriority": {
    "Low": 20,
    "Medium": 50,
    "High": 25,
    "Critical": 5
  },
  "ticketsByClinic": {
    "Clínica A": 50,
    "Clínica B": 30
  },
  "averageResolutionTimeHours": 48.5
}
```

## Autenticação

Todos os endpoints requerem autenticação JWT. O token deve ser incluído no header `Authorization`:

```
Authorization: Bearer {jwt_token}
```

## Permissões

- **Usuários Regulares:** Podem criar, visualizar e comentar em seus próprios chamados
- **System Owners:** Acesso completo a todos os chamados, incluindo:
  - Visualizar todos os chamados
  - Atribuir chamados
  - Ver comentários internos
  - Acessar estatísticas

## Modelos de Dados

### Ticket
```typescript
interface Ticket {
  id: string;
  title: string;
  description: string;
  type: TicketType;
  status: TicketStatus;
  priority: TicketPriority;
  userId: string;
  userName: string;
  userEmail: string;
  clinicId?: string;
  clinicName?: string;
  tenantId: string;
  assignedToId?: string;
  assignedToName?: string;
  comments: TicketComment[];
  attachments: TicketAttachment[];
  createdAt: Date;
  updatedAt: Date;
  lastStatusChangeAt?: Date;
}
```

### TicketComment
```typescript
interface TicketComment {
  id: string;
  ticketId: string;
  comment: string;
  authorName: string;
  isInternal: boolean;
  isSystemOwner: boolean;
  createdAt: Date;
}
```

### TicketAttachment
```typescript
interface TicketAttachment {
  id: string;
  ticketId: string;
  fileName: string;
  fileUrl: string;
  contentType: string;
  fileSize: number;
  uploadedAt: Date;
}
```

## Migração de Dados

Se você está migrando dados existentes do microserviço SystemAdmin, use o script de migração:

```bash
# Via EF Core (recomendado)
cd src/MedicSoft.Repository
dotnet ef database update --startup-project ../MedicSoft.Api

# Via script SQL direto
./scripts/run-ticket-migration.sh "Host=localhost;Database=medicsoft;Username=postgres;Password=yourpassword"
```

## Exemplos de Uso

### Criar um Bug Report
```bash
curl -X POST https://api.medicwarehouse.com/api/tickets \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Erro ao salvar paciente",
    "description": "Ao tentar salvar um novo paciente, aparece erro 500",
    "type": 0,
    "priority": 2
  }'
```

### Listar Meus Chamados
```bash
curl -X GET https://api.medicwarehouse.com/api/tickets/my-tickets \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Adicionar Comentário
```bash
curl -X POST https://api.medicwarehouse.com/api/tickets/{ticketId}/comments \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "comment": "Consegui reproduzir o erro. Anexando screenshot.",
    "isInternal": false
  }'
```

## Troubleshooting

### Erro 401 Unauthorized
- Verifique se o token JWT está sendo enviado corretamente no header
- Certifique-se de que o token não está expirado

### Erro 403 Forbidden
- Verifique se o usuário tem permissões adequadas para a operação
- Algumas operações requerem role "SystemOwner"

### Erro 404 Not Found
- Verifique se o ticketId está correto
- Usuários só podem acessar seus próprios tickets (exceto System Owners)

## Suporte

Para problemas ou dúvidas sobre a API de tickets, crie um ticket através do próprio sistema ou entre em contato com a equipe de suporte técnico.
