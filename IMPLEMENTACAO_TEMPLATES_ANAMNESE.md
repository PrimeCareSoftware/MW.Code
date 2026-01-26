# Implementação: CRUD de Templates de Anamnese

## Resumo

Este documento descreve a implementação completa do sistema de gerenciamento de templates de anamnese, incluindo backend, frontend e configurações de menu e permissões.

## O que foi implementado

### 1. Backend (C# / .NET)

#### Permissões
- **Arquivo**: `src/MedicSoft.Domain/Common/PermissionKeys.cs`
- **Adicionado**: `MedicalRecordsDelete = "medical-records.delete"`
- Esta permissão é automaticamente incluída no sistema de perfis de acesso

#### Queries e Handlers
- **GetAllTemplatesQuery**: Lista todos os templates ativos sem filtro de especialidade
  - Arquivo: `src/MedicSoft.Application/Queries/Anamnesis/GetAllTemplatesQuery.cs`
  - Handler: `src/MedicSoft.Application/Handlers/Queries/Anamnesis/GetAllTemplatesQueryHandler.cs`

#### Commands e Handlers
- **DeleteAnamnesisTemplateCommand**: Deleta um template de anamnese
  - Arquivo: `src/MedicSoft.Application/Commands/Anamnesis/DeleteAnamnesisTemplateCommand.cs`
  - Handler: `src/MedicSoft.Application/Handlers/Commands/Anamnesis/DeleteAnamnesisTemplateCommandHandler.cs`

#### Endpoints da API
- **GET /api/anamnesis/templates/all**: Lista todos os templates
  - Permissão requerida: `MedicalRecordsView`
- **DELETE /api/anamnesis/templates/{id}**: Deleta um template
  - Permissão requerida: `MedicalRecordsDelete`

### 2. Frontend (Angular)

#### Serviço
- **Arquivo**: `frontend/medicwarehouse-app/src/app/services/anamnesis.service.ts`
- **Métodos adicionados**:
  - `getAllTemplates()`: Busca todos os templates
  - `deleteTemplate(templateId)`: Deleta um template

#### Componentes

##### Template Management List
- **Localização**: `frontend/medicwarehouse-app/src/app/pages/anamnesis/template-management/`
- **Arquivos**:
  - `template-management.ts`: Lógica do componente
  - `template-management.html`: Template HTML
  - `template-management.scss`: Estilos
- **Funcionalidades**:
  - Lista todos os templates de anamnese
  - Busca por nome, especialidade ou descrição
  - Botões para editar e excluir templates
  - Exibe status (Ativo/Inativo) e se é template padrão
  - Mostra número de seções de cada template

##### Template Form
- **Localização**: `frontend/medicwarehouse-app/src/app/pages/anamnesis/template-form/`
- **Arquivos**:
  - `template-form.ts`: Lógica do componente
  - `template-form.html`: Template HTML
  - `template-form.scss`: Estilos
- **Funcionalidades**:
  - Criar novo template ou editar existente
  - Campos básicos: Nome, Especialidade, Descrição, Template Padrão
  - Gerenciamento de seções com perguntas
  - Tipos de perguntas: Text, Number, YesNo, SingleChoice, MultipleChoice, Date, Scale
  - Validação de campos obrigatórios
  - Interface intuitiva para adicionar/remover seções e perguntas

#### Rotas
- **Arquivo**: `frontend/medicwarehouse-app/src/app/app.routes.ts`
- **Rotas adicionadas**:
  - `/anamnesis/templates/manage`: Lista de templates (gerenciamento)
  - `/anamnesis/templates/new`: Criar novo template
  - `/anamnesis/templates/edit/:id`: Editar template existente

#### Menu
- **Arquivo**: `frontend/medicwarehouse-app/src/app/shared/navbar/navbar.html`
- **Itens adicionados**:
  - "Templates de Anamnese" - Link para seletor de templates (uso)
  - "Gerenciar Templates" - Link para gerenciamento CRUD

### 3. Configuração de Perfis

As permissões são automaticamente carregadas do método `PermissionKeys.GetAllPermissionsByCategory()`, que agora inclui:
- `medical-records.view`: Visualizar prontuários
- `medical-records.create`: Criar prontuários
- `medical-records.edit`: Editar prontuários
- `medical-records.delete`: **NOVO** - Excluir prontuários (incluindo templates de anamnese)

## Verificações Realizadas

### Menu
✅ Todos os screens principais estão disponíveis no menu
✅ Novos itens de menu para templates de anamnese foram adicionados

### Permissões
✅ A permissão `MedicalRecordsDelete` foi adicionada
✅ Ela aparece automaticamente na tela de configuração de perfis na categoria "Prontuários"

### Compilação
✅ Backend compila com sucesso (apenas warnings pré-existentes)
✅ Frontend compila com sucesso (TypeScript sem erros)

## Como Usar

### Acessar Gerenciamento de Templates
1. Faça login no sistema
2. No menu lateral, clique em "Gerenciar Templates"
3. Você verá a lista de todos os templates de anamnese

### Criar Novo Template
1. Na tela de gerenciamento, clique em "Novo Template"
2. Preencha as informações básicas (Nome, Especialidade, Descrição)
3. Adicione seções clicando em "Adicionar Seção"
4. Para cada seção, adicione perguntas clicando em "Adicionar Pergunta"
5. Configure cada pergunta (texto, tipo, obrigatória, opções, etc.)
6. Clique em "Criar Template" para salvar

### Editar Template
1. Na lista de templates, clique no botão de editar (ícone de lápis)
2. Faça as alterações necessárias
3. Clique em "Atualizar Template" para salvar

### Excluir Template
1. Na lista de templates, clique no botão de excluir (ícone de lixeira)
2. Confirme a exclusão
3. O template será removido

### Configurar Permissões
1. Acesse "Perfis de Acesso" no menu (apenas para proprietários)
2. Crie ou edite um perfil
3. Na categoria "Prontuários", você verá:
   - Visualizar prontuários
   - Criar prontuários
   - Editar prontuários
   - **Excluir prontuários** (nova permissão)
4. Selecione as permissões desejadas e salve

## Arquivos Modificados/Criados

### Backend
- `src/MedicSoft.Domain/Common/PermissionKeys.cs` (modificado)
- `src/MedicSoft.Api/Controllers/AnamnesisController.cs` (modificado)
- `src/MedicSoft.Application/Queries/Anamnesis/GetAllTemplatesQuery.cs` (novo)
- `src/MedicSoft.Application/Handlers/Queries/Anamnesis/GetAllTemplatesQueryHandler.cs` (novo)
- `src/MedicSoft.Application/Commands/Anamnesis/DeleteAnamnesisTemplateCommand.cs` (novo)
- `src/MedicSoft.Application/Handlers/Commands/Anamnesis/DeleteAnamnesisTemplateCommandHandler.cs` (novo)

### Frontend
- `frontend/medicwarehouse-app/src/app/services/anamnesis.service.ts` (modificado)
- `frontend/medicwarehouse-app/src/app/app.routes.ts` (modificado)
- `frontend/medicwarehouse-app/src/app/shared/navbar/navbar.html` (modificado)
- `frontend/medicwarehouse-app/src/app/pages/anamnesis/template-management/` (novo diretório)
  - `template-management.ts`
  - `template-management.html`
  - `template-management.scss`
- `frontend/medicwarehouse-app/src/app/pages/anamnesis/template-form/` (novo diretório)
  - `template-form.ts`
  - `template-form.html`
  - `template-form.scss`

## Notas Técnicas

1. **Soft Delete**: O método `DeleteAsync` no repositório implementa soft delete, então os templates não são fisicamente removidos do banco de dados, apenas marcados como inativos.

2. **Tenant Isolation**: Todas as operações respeitam o contexto de tenant, garantindo que uma clínica só pode ver/gerenciar seus próprios templates.

3. **Permissões**: As operações de CRUD seguem o padrão de permissões do sistema:
   - View: Listar e visualizar templates
   - Create/Edit: Criar e editar templates
   - Delete: Excluir templates

4. **Validações**: O formulário de criação/edição possui validações client-side e server-side para garantir a integridade dos dados.

5. **UI Responsiva**: Todos os componentes foram desenvolvidos com design responsivo, funcionando bem em desktop e mobile.

## Próximos Passos (Opcional)

- Adicionar pré-visualização de template antes de aplicar
- Implementar duplicação de templates
- Adicionar histórico de versões de templates
- Implementar importação/exportação de templates
- Adicionar templates pré-definidos para especialidades comuns
