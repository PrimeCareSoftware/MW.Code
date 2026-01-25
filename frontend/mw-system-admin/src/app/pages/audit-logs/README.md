# Sistema de Logs de Auditoria

## Visão Geral

O sistema de logs de auditoria foi implementado para permitir o monitoramento e rastreamento de todas as atividades do sistema, erros, e execução de funções. Esta funcionalidade atende aos requisitos de auditoria e conformidade com LGPD.

## Características Implementadas

### 1. Backend (Já Existente)
- **API de Auditoria**: `/api/audit`
- **Entidade AuditLog**: Armazena logs detalhados de todas as ações
- **Serviço de Auditoria**: `IAuditService` e `AuditService`
- **Enums de Auditoria**: Ações, Resultados, Severidade, Categorias LGPD

### 2. Frontend (Implementado)

#### Componentes
- **AuditLogs Component**: Página principal de visualização de logs
- **AuditService**: Serviço Angular para comunicação com a API

#### Funcionalidades

##### Filtros de Pesquisa
- **Data**: Filtro por período (data inicial e final)
- **Usuário**: Busca por ID de usuário específico
- **Tipo de Entidade**: Filtro por tipo de entidade (Patient, User, Clinic, etc.)
- **ID da Entidade**: Busca por ID específico de entidade
- **Ação**: Filtro por tipo de ação (CREATE, READ, UPDATE, DELETE, LOGIN, etc.)
- **Resultado**: Filtro por resultado (SUCCESS, FAILED, UNAUTHORIZED, PARTIAL_SUCCESS)
- **Severidade**: Filtro por nível de severidade (INFO, WARNING, ERROR, CRITICAL)

##### Visualização
- **Tabela de Logs**: Exibição organizada dos logs
  - Data/Hora formatada
  - Informações do usuário (nome e email)
  - Ação executada com ícone visual
  - Tipo de entidade afetada
  - Resultado da operação (com badge colorido)
  - Severidade (com badge colorido)
  - Endereço IP
  - Ações (botão para ver detalhes)

- **Modal de Detalhes**: Visualização completa de um log específico
  - Informações gerais (data, ação, descrição, resultado, severidade)
  - Dados do usuário
  - Entidade afetada
  - Detalhes da requisição (IP, método HTTP, caminho, status code)
  - Alterações (campos alterados, valores antigos e novos)
  - Razão de falha (quando aplicável)
  - Informações LGPD (categoria de dados, finalidade)
  - User Agent completo

##### Paginação
- Navegação entre páginas
- Exibição do total de registros
- Controle de registros por página (padrão: 50)

##### Exportação
- **CSV**: Exportação em formato CSV com as principais informações
- **JSON**: Exportação completa em formato JSON

## Como Usar

### Acesso
1. Fazer login no sistema como SystemAdmin
2. No menu lateral, acessar: **Monitoramento e Segurança > Logs de Auditoria**
3. A página carregará automaticamente os logs dos últimos 7 dias

### Pesquisa e Filtros
1. Expandir/recolher a seção de filtros clicando no cabeçalho
2. Preencher os filtros desejados
3. Clicar em "Aplicar Filtros" para executar a busca
4. Usar "Limpar Filtros" para resetar aos valores padrão

### Visualizar Detalhes
1. Na tabela de logs, clicar no ícone de olho (👁️) na coluna "Ações"
2. O modal será aberto com todos os detalhes do log
3. Clicar em "Fechar" ou fora do modal para retornar à lista

### Exportar Dados
1. Após filtrar os logs desejados
2. Clicar em "Exportar CSV" ou "Exportar JSON" no cabeçalho da página
3. O arquivo será baixado automaticamente

## Tipos de Ações Rastreadas

### CRUD
- CREATE: Criação de entidades
- READ: Leitura de dados
- UPDATE: Atualização de registros
- DELETE: Exclusão de registros

### Autenticação
- LOGIN: Login bem-sucedido
- LOGOUT: Logout do sistema
- LOGIN_FAILED: Tentativa de login falha
- PASSWORD_CHANGED: Alteração de senha
- PASSWORD_RESET_REQUESTED: Solicitação de reset de senha
- MFA_ENABLED: Autenticação multifator habilitada
- MFA_DISABLED: Autenticação multifator desabilitada

### Autorização
- ACCESS_DENIED: Acesso negado
- PERMISSION_CHANGED: Alteração de permissões
- ROLE_CHANGED: Alteração de função/papel

### Dados
- EXPORT: Exportação de dados
- DOWNLOAD: Download de arquivos
- PRINT: Impressão de documentos

### LGPD
- DATA_ACCESS_REQUEST: Solicitação de acesso a dados
- DATA_DELETION_REQUEST: Solicitação de exclusão de dados
- DATA_PORTABILITY_REQUEST: Solicitação de portabilidade
- DATA_CORRECTION_REQUEST: Solicitação de correção de dados

## Níveis de Severidade

- **INFO**: Eventos informativos normais
- **WARNING**: Avisos que requerem atenção
- **ERROR**: Erros que afetam funcionalidades
- **CRITICAL**: Eventos críticos como violações de segurança

## Resultados de Operação

- **SUCCESS**: Operação executada com sucesso
- **FAILED**: Operação falhou
- **UNAUTHORIZED**: Acesso não autorizado
- **PARTIAL_SUCCESS**: Operação parcialmente bem-sucedida

## API Endpoints

### Query de Logs
```
POST /api/audit/query
```

Corpo da requisição:
```json
{
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-01-31T23:59:59Z",
  "userId": "user-id-optional",
  "entityType": "Patient",
  "action": "READ",
  "result": "SUCCESS",
  "severity": "INFO",
  "pageNumber": 1,
  "pageSize": 50
}
```

### Atividade de Usuário
```
GET /api/audit/user/{userId}?startDate=&endDate=
```

### Histórico de Entidade
```
GET /api/audit/entity/{entityType}/{entityId}
```

### Eventos de Segurança
```
GET /api/audit/security-events?startDate=&endDate=
```

### Relatório LGPD
```
GET /api/audit/lgpd-report/{userId}
```

## Estrutura de Arquivos

```
frontend/mw-system-admin/src/app/
├── pages/
│   └── audit-logs/
│       ├── audit-logs.ts          # Componente principal
│       ├── audit-logs.html        # Template
│       └── audit-logs.scss        # Estilos
├── services/
│   └── audit.service.ts           # Serviço de comunicação com API
└── shared/
    └── navbar/
        └── navbar.html            # Menu atualizado com link

backend/src/MedicSoft.Api/Controllers/
└── AuditController.cs              # Controlador de API

backend/src/MedicSoft.Application/Services/
├── IAuditService.cs                # Interface do serviço
└── AuditService.cs                 # Implementação do serviço

backend/src/MedicSoft.Domain/
├── Entities/
│   └── AuditLog.cs                 # Entidade principal
├── Enums/
│   └── AuditEnums.cs              # Enumerações
└── ValueObjects/
    └── AuditFilter.cs             # Filtro de pesquisa
```

## Conformidade LGPD

O sistema de auditoria está em conformidade com a LGPD (Lei 13.709/2018), especificamente:

- **Artigo 37**: Registro das operações de tratamento de dados pessoais
- **Artigo 46**: Segurança dos dados e prevenção de incidentes
- **Artigo 48**: Comunicação de incidentes de segurança

Cada log inclui:
- Categoria de dados (PUBLIC, PERSONAL, SENSITIVE, CONFIDENTIAL)
- Finalidade do tratamento (HEALTHCARE, BILLING, LEGAL_OBLIGATION, etc.)
- Registro detalhado de acesso e modificações

## Melhorias Futuras Possíveis

1. **Dashboard de Analytics**: Gráficos e visualizações de dados agregados
2. **Alertas Automáticos**: Notificações para eventos críticos
3. **Integração com Elastic Search**: Para buscas mais avançadas e rápidas
4. **Retenção de Dados**: Políticas automatizadas de arquivamento/exclusão
5. **Exportação Agendada**: Relatórios periódicos automáticos
6. **Machine Learning**: Detecção de anomalias e padrões suspeitos

## Suporte

Para questões ou problemas relacionados ao sistema de auditoria:
1. Verificar este documento primeiro
2. Consultar os logs do sistema
3. Contatar a equipe de desenvolvimento
