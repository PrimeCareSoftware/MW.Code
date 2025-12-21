# Sistema de Chamados / Ticketing System

## Visão Geral

O Sistema de Chamados do MedicWarehouse permite que usuários abram tickets para reportar bugs, solicitar funcionalidades, obter suporte técnico e gerenciar questões financeiras. System owners podem gerenciar todos os chamados através de um quadro Kanban interativo.

## Funcionalidades Principais

### Para Usuários (MedicWarehouse App)

#### 1. Abertura de Chamados
- **Botão Flutuante (FAB)**: Disponível em todas as telas do sistema
- **Badge de Notificação**: Exibe número de atualizações não lidas
- **Modal de Criação**:
  - Campo de título (obrigatório)
  - Seletor de tipo de chamado
  - Seletor de prioridade
  - Descrição detalhada (obrigatório)
  - Upload de imagens (arrastar, selecionar ou colar Ctrl+V)
  
#### 2. Tipos de Chamado
- **Reporte de Bug**: Para reportar problemas no sistema
- **Solicitação de Funcionalidade**: Para solicitar novas funcionalidades
- **Ajuste no Sistema**: Para solicitar ajustes em funcionalidades existentes
- **Questão Financeira**: Para questões relacionadas a pagamentos e assinaturas
- **Suporte Técnico**: Para problemas técnicos gerais
- **Suporte ao Usuário**: Para dúvidas sobre uso do sistema
- **Outro**: Para outros tipos de solicitações

#### 3. Prioridades
- **Baixa**: Questões não urgentes
- **Média**: Prioridade padrão
- **Alta**: Questões importantes que afetam o uso
- **Crítica**: Problemas graves que impedem o uso do sistema

#### 4. Página de Chamados
- Visualização de todos os chamados do usuário
- Filtro por status
- Busca por título
- Cards com informações resumidas
- Click para ver detalhes completos

#### 5. Detalhes do Chamado
- Visualização completa das informações
- Histórico de comentários
- Anexos de imagens
- Informações de status e atribuição
- Possibilidade de adicionar comentários

### Para System Owners (MW System Admin)

#### 1. Dashboard de Estatísticas
- Total de chamados
- Chamados concluídos
- Chamados em andamento
- Chamados com impedimento
- Métricas por tipo e prioridade
- Tempo médio de resolução

#### 2. Quadro Kanban
- **6 Colunas de Status**:
  - Aberto
  - Em Análise
  - Em Atendimento
  - Com Impedimento
  - Concluído
  - Cancelado
- **Drag & Drop**: Arraste cards entre colunas para alterar status
- **Filtros**: Por tipo, clínica, busca textual
- **View Toggle**: Alternar entre Kanban e Lista

#### 3. Gerenciamento de Chamados
- Visualizar todos os chamados do sistema
- Atribuir chamados a owners
- Atualizar status com comentário
- Adicionar comentários (visíveis para o usuário)
- Comentários internos (apenas entre owners)
- Filtrar por clínica, tipo, status

#### 4. Detalhes Expandidos
- Informações completas do usuário
- Clínica de origem
- Histórico completo de mudanças
- Thread de comentários
- Anexos com visualização

## Arquitetura Técnica

### Backend (Microservice SystemAdmin)

#### Entidades
```csharp
- TicketEntity: Ticket principal
- TicketCommentEntity: Comentários no ticket
- TicketAttachmentEntity: Anexos de imagem
- TicketHistoryEntity: Histórico de mudanças de status
```

#### Endpoints da API
```
POST   /api/tickets                    - Criar chamado
GET    /api/tickets/{id}               - Obter detalhes
GET    /api/tickets/my-tickets         - Chamados do usuário
GET    /api/tickets/clinic/{clinicId}  - Chamados da clínica
GET    /api/tickets                    - Todos (system owner)
PUT    /api/tickets/{id}               - Atualizar ticket
PUT    /api/tickets/{id}/status        - Atualizar status
PUT    /api/tickets/{id}/assign        - Atribuir owner
POST   /api/tickets/{id}/comments      - Adicionar comentário
POST   /api/tickets/{id}/attachments   - Upload de anexo
GET    /api/tickets/unread-count       - Contador de não lidos
POST   /api/tickets/{id}/mark-read     - Marcar como lido
GET    /api/tickets/statistics         - Estatísticas
```

#### Autenticação e Permissões
- Usuários regulares: Apenas seus próprios tickets
- System Owners: Todos os tickets do sistema
- Isolamento por TenantId e ClinicId
- JWT token compartilhado entre microservices

### Frontend

#### MedicWarehouse App
```typescript
Componentes:
- TicketFab: Botão flutuante com modal de criação
- Tickets: Página de listagem e detalhes
- TicketService: Comunicação com API

Rotas:
- /tickets - Listagem de chamados do usuário
```

#### MW System Admin
```typescript
Componentes:
- TicketsPage: Kanban board e gerenciamento
- TicketService: Comunicação com API

Rotas:
- /tickets - Quadro Kanban e gerenciamento
```

#### Modelos TypeScript
```typescript
- TicketStatus enum
- TicketType enum
- TicketPriority enum
- Ticket interface
- TicketSummary interface
- CreateTicketRequest
- UpdateTicketStatusRequest
- AddTicketCommentRequest
- TicketStatistics
```

## Fluxo de Trabalho Típico

### 1. Usuário Abre Chamado
1. Clica no botão flutuante (FAB)
2. Preenche título e descrição
3. Seleciona tipo e prioridade
4. Adiciona imagens (opcional)
5. Submete o chamado

### 2. System Owner Gerencia Chamado
1. Visualiza novo chamado no Kanban (coluna "Aberto")
2. Arrasta para "Em Análise" ou clica para ver detalhes
3. Adiciona comentário com análise
4. Atribui a um owner específico (opcional)
5. Move para "Em Atendimento"
6. Trabalha na resolução
7. Move para "Concluído" com comentário final

### 3. Usuário Acompanha Progresso
1. Vê badge de notificação no FAB
2. Acessa página de chamados
3. Clica no chamado atualizado
4. Lê comentários do suporte
5. Adiciona feedback se necessário

## Upload de Imagens

### Métodos Suportados
1. **Upload de Arquivo**: Click no botão "Selecionar Imagens"
2. **Drag & Drop**: Arrastar imagens para o textarea (futuro)
3. **Ctrl+V / Cmd+V**: Colar imagens da área de transferência

### Formato de Dados
- Imagens convertidas para Base64
- Enviadas no formato: `{ fileName, base64Data, contentType }`
- Armazenamento: `/uploads/tickets/{ticketId}/{guid}_{filename}`
- Tipos aceitos: image/*

### Implementação Futura
- Upload para cloud storage (AWS S3, Azure Blob)
- Compressão de imagens
- Thumbnails
- Visualização inline de imagens

## Estados do Kanban

| Status | Cor | Descrição |
|--------|-----|-----------|
| Aberto | Azul (#3b82f6) | Ticket recém-criado |
| Em Análise | Amarelo (#f59e0b) | Sendo analisado pelo suporte |
| Em Atendimento | Roxo (#8b5cf6) | Em processo de resolução |
| Com Impedimento | Vermelho (#ef4444) | Bloqueado aguardando algo |
| Concluído | Verde (#10b981) | Resolvido com sucesso |
| Cancelado | Cinza (#6b7280) | Cancelado sem resolução |

## Notificações

### Contador de Não Lidos
- Badge no botão flutuante
- Atualizado ao criar ticket
- Atualizado ao receber comentário
- Resetado ao visualizar ticket

### Implementação Futura
- Notificações em tempo real (WebSockets)
- Email notifications
- Push notifications mobile
- Notificações no sistema

## Multi-tenancy e Segurança

### Isolamento de Dados
- Tickets filtrados por `TenantId`
- Usuários veem apenas seus tickets
- System owners veem todos os tickets
- Clínicas isoladas por `ClinicId`

### Validações
- JWT token obrigatório em todos endpoints
- Verificação de permissões no backend
- Claims: `tenant_id`, `clinic_id`, `user_id`, `role`
- System owner identificado por claim especial

## Métricas e Analytics

### Estatísticas Disponíveis
- Total de tickets
- Tickets por status (6 categorias)
- Tickets por tipo
- Tickets por prioridade
- Tickets por clínica
- Tempo médio de resolução (em horas)

### Uso
```typescript
ticketService.getStatistics(clinicId?, tenantId?)
```

## Migrações de Banco de Dados

### Tabelas Criadas
```sql
- Tickets: Ticket principal
- TicketComments: Comentários
- TicketAttachments: Anexos
- TicketHistory: Histórico de mudanças
```

### Executar Migrações
```bash
cd microservices/systemadmin
dotnet ef migrations add AddTicketingSystem
dotnet ef database update
```

## Testes Recomendados

### Backend
- [ ] Criar ticket como usuário
- [ ] Obter tickets por usuário
- [ ] Obter tickets por clínica
- [ ] System owner visualizar todos
- [ ] Atualizar status
- [ ] Adicionar comentário
- [ ] Upload de anexo
- [ ] Validar isolamento multi-tenant
- [ ] Verificar permissões

### Frontend - User App
- [ ] Abrir modal de criação
- [ ] Preencher formulário
- [ ] Upload de imagem
- [ ] Paste de imagem (Ctrl+V)
- [ ] Submeter ticket
- [ ] Visualizar lista de tickets
- [ ] Filtrar por status
- [ ] Ver detalhes do ticket
- [ ] Adicionar comentário
- [ ] Badge de notificação

### Frontend - System Admin
- [ ] Visualizar Kanban board
- [ ] Drag & drop entre colunas
- [ ] Filtrar tickets
- [ ] Buscar tickets
- [ ] Alternar para view de lista
- [ ] Ver detalhes do ticket
- [ ] Atualizar status
- [ ] Adicionar comentário
- [ ] Atribuir owner
- [ ] Ver estatísticas

## Melhorias Futuras

### Curto Prazo
- [ ] Adicionar migrações de banco
- [ ] Implementar upload para cloud storage
- [ ] Adicionar validação de tamanho de arquivo
- [ ] Compressão de imagens
- [ ] Thumbnails de imagens
- [ ] SLA tracking

### Médio Prazo
- [ ] Notificações em tempo real (WebSockets)
- [ ] Email notifications
- [ ] Templates de resposta
- [ ] Macros para respostas rápidas
- [ ] Busca avançada (ElasticSearch)
- [ ] Export de relatórios (PDF/Excel)

### Longo Prazo
- [ ] Integração com WhatsApp
- [ ] Chatbot para triagem
- [ ] Base de conhecimento (KB)
- [ ] Portal self-service
- [ ] Analytics avançados
- [ ] Machine learning para categorização

## Contribuindo

### Adicionando Novo Tipo de Ticket
1. Adicionar ao enum `TicketType` no backend
2. Adicionar ao enum `TicketType` no frontend
3. Atualizar função `getTicketTypeLabel()`
4. Atualizar lista `ticketTypes` nos componentes

### Adicionando Novo Status
1. Adicionar ao enum `TicketStatus` no backend
2. Adicionar ao enum `TicketStatus` no frontend
3. Atualizar funções helper (label, color, badge)
4. Adicionar coluna no Kanban (system admin)

## Suporte

Para dúvidas ou problemas:
- Abra um ticket no sistema 😉
- Ou contate o suporte técnico

## License

© 2024 MedicWarehouse. All rights reserved.
