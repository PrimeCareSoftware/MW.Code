# Guia de Administração de Clínicas

## Visão Geral

A área de administração de clínicas é uma funcionalidade exclusiva para proprietários (owners) de clínicas, permitindo o gerenciamento completo de usuários, visualização de detalhes da assinatura e administração do plano contratado.

## Recursos Principais

### 1. Gerenciamento de Usuários
- Criar novos usuários
- Editar informações de usuários existentes
- Alterar senhas de usuários
- Modificar perfis/roles de usuários
- Ativar/Desativar usuários
- Visualizar lista completa de usuários da clínica

### 2. Gerenciamento de Assinatura
- Visualizar detalhes do plano contratado
- Verificar limites de usuários e pacientes
- Monitorar uso atual vs. limites do plano
- Ver funcionalidades incluídas no plano
- Solicitar cancelamento de assinatura

### 3. Múltiplas Clínicas
- Listar todas as clínicas associadas ao proprietário
- Alternar entre clínicas (via troca de token JWT)

## Acesso à Área de Administração

### Pré-requisitos
- Usuário autenticado no sistema
- Role de **Owner** (Proprietário)
- Pelo menos uma clínica vinculada ao usuário

### Rotas Frontend
Todas as rotas estão protegidas pelos guards `authGuard` e `ownerGuard`:

- `/clinic-admin/info` - Informações da clínica
- `/clinic-admin/users` - Gerenciamento de usuários
- `/clinic-admin/subscription` - Detalhes da assinatura
- `/clinic-admin/customization` - Personalização da clínica

## API Endpoints

### Base URL
```
/api/ClinicAdmin
```

### Endpoints de Gerenciamento de Usuários

#### 1. Listar Usuários
```http
GET /api/ClinicAdmin/users
```

**Response:**
```json
[
  {
    "id": "uuid",
    "username": "joao.silva",
    "name": "João Silva",
    "email": "joao@clinica.com",
    "role": "Doctor",
    "isActive": true,
    "createdAt": "2024-01-15T10:30:00Z"
  }
]
```

#### 2. Criar Usuário
```http
POST /api/ClinicAdmin/users
```

**Request Body:**
```json
{
  "username": "maria.santos",
  "email": "maria@clinica.com",
  "password": "SenhaSegura123!",
  "name": "Maria Santos",
  "phone": "(11) 98765-4321",
  "role": "Nurse"
}
```

**Response:**
```json
{
  "id": "uuid",
  "username": "maria.santos",
  "name": "Maria Santos",
  "email": "maria@clinica.com",
  "role": "Nurse",
  "isActive": true,
  "createdAt": "2024-01-15T14:20:00Z"
}
```

**Validações:**
- Verifica limite de usuários do plano
- Valida força da senha (mínimo 8 caracteres)
- Verifica se username já existe
- Requer assinatura ativa

#### 3. Atualizar Usuário
```http
PUT /api/ClinicAdmin/users/{id}
```

**Request Body:**
```json
{
  "email": "novo.email@clinica.com",
  "name": "Maria Santos Silva",
  "phone": "(11) 99999-8888",
  "isActive": true
}
```

#### 4. Alterar Senha de Usuário
```http
PUT /api/ClinicAdmin/users/{id}/password
```

**Request Body:**
```json
{
  "newPassword": "NovaSenhaSegura123!"
}
```

**Validações:**
- Senha deve ter no mínimo 8 caracteres
- Deve conter letras, números e caracteres especiais (recomendado)

#### 5. Alterar Perfil de Usuário
```http
PUT /api/ClinicAdmin/users/{id}/role
```

**Request Body:**
```json
{
  "newRole": "Admin"
}
```

**Roles Disponíveis:**
- `Doctor` - Médico
- `Nurse` - Enfermeiro
- `Receptionist` - Recepcionista
- `Admin` - Administrador
- `Owner` - Proprietário

#### 6. Desativar Usuário
```http
POST /api/ClinicAdmin/users/{id}/deactivate
```

**Efeito:** O usuário não poderá mais fazer login no sistema

#### 7. Ativar Usuário
```http
POST /api/ClinicAdmin/users/{id}/activate
```

**Efeito:** Restaura o acesso do usuário ao sistema

### Endpoints de Assinatura

#### 1. Detalhes da Assinatura
```http
GET /api/ClinicAdmin/subscription/details
```

**Response:**
```json
{
  "id": "uuid",
  "planId": "uuid",
  "planName": "Plano Premium",
  "planType": "Premium",
  "status": "Active",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2025-01-01T00:00:00Z",
  "nextBillingDate": "2024-02-01T00:00:00Z",
  "currentPrice": 299.90,
  "isTrial": false,
  "isActive": true,
  "limits": {
    "maxUsers": 15,
    "maxPatients": 1000,
    "currentUsers": 8
  },
  "features": {
    "hasReports": true,
    "hasWhatsAppIntegration": true,
    "hasSMSNotifications": true,
    "hasTissExport": true
  },
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### 2. Listar Minhas Clínicas
```http
GET /api/ClinicAdmin/my-clinics
```

**Response:**
```json
[
  {
    "clinicId": "uuid",
    "name": "Clínica São José",
    "tradeName": "Clínica SJ",
    "document": "12.345.678/0001-90",
    "subdomain": "clinica-sj",
    "tenantId": "tenant-uuid",
    "isActive": true,
    "isPrimaryOwner": true,
    "hasActiveSubscription": true,
    "subscriptionStatus": "Active"
  }
]
```

#### 3. Cancelar Assinatura
```http
PUT /api/ClinicAdmin/subscription/cancel
```

**Nota:** Solicita o cancelamento. O acesso permanece até o fim do período pago.

## Interface do Usuário

### Tela de Gerenciamento de Usuários

#### Funcionalidades
1. **Lista de Usuários**: Tabela com todos os usuários da clínica
2. **Botão "Novo Usuário"**: Abre modal para criar usuário
3. **Ações por Usuário**:
   - ✏️ Editar - Altera informações básicas
   - 🔒 Alterar Senha - Define nova senha
   - 👤 Alterar Perfil - Muda o role do usuário
   - ❌/✓ Desativar/Ativar - Controla acesso

#### Modal de Criação de Usuário
Campos obrigatórios (*):
- Nome de Usuário *
- Nome Completo *
- E-mail *
- Senha * (mínimo 8 caracteres)
- Perfil * (dropdown com roles)
- Telefone (opcional)

#### Modal de Edição de Usuário
Campos editáveis:
- Nome Completo
- E-mail
- Telefone

#### Modal de Alteração de Senha
- Nova Senha (mínimo 8 caracteres)

#### Modal de Alteração de Perfil
- Dropdown com todos os perfis disponíveis

#### Confirmação de Desativação/Ativação
- Diálogo de confirmação antes de alterar status

### Tela de Assinatura

#### Informações Exibidas

**Cabeçalho do Plano:**
- Nome do Plano
- Tipo do Plano
- Status (Ativo/Trial/Cancelado/etc)
- Valor mensal
- Badge "Trial" se aplicável

**Datas:**
- Data de Início
- Data de Término (se aplicável)
- Próxima Cobrança

**Limites do Plano:**
- Usuários: Barra de progresso visual
  - Verde: 0-74% do limite
  - Amarelo: 75-89% do limite
  - Vermelho: 90-100% do limite
- Pacientes: Limite máximo

**Funcionalidades:**
- ✓ Relatórios Avançados (enabled/disabled)
- ✓ Integração WhatsApp (enabled/disabled)
- ✓ Notificações SMS (enabled/disabled)
- ✓ Exportação TISS (enabled/disabled)

**Ações:**
- Botão "Solicitar Cancelamento" (apenas se assinatura ativa)

## Segurança e Permissões

### Autenticação
- Todos os endpoints requerem autenticação JWT
- Token deve conter informações de TenantId e UserId

### Autorização
- **Owner Guard**: Verifica se o usuário tem role Owner
- **Tenant Scope**: Todas as operações são restritas ao tenant do token JWT
- **Clinic Verification**: Valida que o usuário é owner da clínica

### Validações de Negócio

#### Criação de Usuário
1. Verifica se o owner possui clínica vinculada
2. Valida se há assinatura ativa
3. Verifica se o limite de usuários não foi atingido
4. Valida força da senha
5. Verifica se username já existe no tenant

#### Alteração de Senha
1. Valida força da senha (mínimo 8 caracteres)
2. Verifica se usuário pertence à clínica do owner

#### Alteração de Status/Role
1. Verifica se usuário pertence à clínica do owner
2. Valida o novo role fornecido

### Isolamento Multi-Tenant
- Todas as consultas incluem filtro por TenantId
- Não é possível acessar usuários/dados de outros tenants
- Owner só visualiza/gerencia usuários de suas próprias clínicas

## Fluxo de Uso Típico

### 1. Primeiro Acesso
```
1. Owner faz login no sistema
2. JWT é gerado com TenantId e UserId
3. Acessa /clinic-admin/users
4. Sistema valida:
   - Autenticação (authGuard)
   - Role Owner (ownerGuard)
   - Vínculo com clínica
5. Exibe tela de gerenciamento
```

### 2. Criar Novo Usuário
```
1. Owner clica em "Novo Usuário"
2. Preenche formulário de criação
3. Sistema valida:
   - Limite de usuários não atingido
   - Senha forte
   - Username único
4. Cria usuário no banco
5. Retorna usuário criado
6. Atualiza lista de usuários
```

### 3. Verificar Limites do Plano
```
1. Owner acessa /clinic-admin/subscription
2. Sistema busca:
   - Detalhes da assinatura
   - Plano contratado
   - Uso atual
3. Exibe:
   - Limites e uso
   - Funcionalidades
   - Status do plano
4. Barra de progresso indica proximidade do limite
```

### 4. Múltiplas Clínicas
```
1. Owner com múltiplas clínicas
2. Acessa /clinic-admin/my-clinics
3. Sistema lista todas as clínicas
4. Owner seleciona clínica
5. Frontend solicita novo token JWT para a clínica selecionada
6. Todas as operações subsequentes usam novo contexto
```

## Tratamento de Erros

### Erros Comuns

#### 401 Unauthorized
```json
{
  "message": "Usuário não autenticado"
}
```
**Solução:** Fazer login novamente

#### 403 Forbidden
```json
{
  "message": "Acesso negado. Apenas proprietários podem acessar esta área."
}
```
**Solução:** Usuário não é owner da clínica

#### 400 Bad Request - Limite Atingido
```json
{
  "message": "User limit reached. Current plan allows 10 users. Please upgrade your plan."
}
```
**Solução:** Fazer upgrade do plano ou remover usuários inativos

#### 400 Bad Request - Senha Fraca
```json
{
  "message": "Password must be at least 8 characters long and contain uppercase, lowercase, numbers and special characters"
}
```
**Solução:** Usar senha mais forte

#### 404 Not Found
```json
{
  "message": "User not found"
}
```
**Solução:** Usuário não existe ou não pertence à clínica

#### 500 Internal Server Error
```json
{
  "message": "An error occurred while processing your request"
}
```
**Solução:** Verificar logs do servidor, contatar suporte

## Testes

### Testes Unitários Backend
Arquivo: `tests/MedicSoft.Test/Services/UserServiceTests.cs`

Cenários testados:
- Criação de usuário com sucesso
- Falha ao criar usuário com username existente
- Alteração de senha com sucesso
- Falha ao alterar senha de usuário inexistente
- Ativação/Desativação de usuário
- Alteração de role

### Testes de Integração
Recomendações de cenários:
1. Fluxo completo de CRUD de usuário
2. Validação de limites de plano
3. Multi-tenant isolation
4. Permissões de owner
5. Alteração entre clínicas

## Melhores Práticas

### Para Desenvolvedores

1. **Sempre validar limites**: Antes de criar usuário, verificar limite do plano
2. **Usar tenant scope**: Todas as queries devem filtrar por TenantId
3. **Logging**: Registrar todas as operações administrativas
4. **Senha segura**: Validar força da senha antes de salvar
5. **Transações**: Usar transações para operações críticas

### Para Usuários (Owners)

1. **Senhas Fortes**: Usar senhas com no mínimo 8 caracteres, letras, números e símbolos
2. **Roles Apropriados**: Atribuir roles adequados para cada usuário
3. **Desativar ao invés de Deletar**: Manter histórico de usuários desativando-os
4. **Monitorar Limites**: Acompanhar uso vs. limites do plano
5. **Revisar Usuários**: Periodicamente revisar lista de usuários ativos

## Roadmap e Melhorias Futuras

### Planejado
- [ ] Histórico de alterações de usuários (audit log)
- [ ] Exportação de lista de usuários (CSV/Excel)
- [ ] Filtros e busca avançada na lista de usuários
- [ ] Convite de usuários por e-mail
- [ ] Configuração de permissões granulares
- [ ] Dashboard com métricas de uso
- [ ] Notificações quando próximo do limite
- [ ] Self-service de upgrade de plano
- [ ] Gestão de múltiplas clínicas melhorada

### Em Consideração
- [ ] Autenticação de dois fatores (2FA)
- [ ] Single Sign-On (SSO)
- [ ] Integração com diretório ativo (AD)
- [ ] Roles personalizados
- [ ] Workflow de aprovação de criação de usuários

## Suporte e Contato

Para dúvidas, problemas ou sugestões relacionadas à área de administração de clínicas:

- **Documentação Técnica**: Ver arquivos em `/docs`
- **Issues**: Abrir issue no GitHub
- **Suporte**: Contatar equipe de desenvolvimento

## Referências

- [Documentação de Autenticação](./AUTHENTICATION_GUIDE.md)
- [Guia de Permissões](./QUICK_REFERENCE_PERMISSIONS.md)
- [Documentação de Multi-Clinic](./MULTI_CLINIC_OWNERSHIP_GUIDE.md)
- [Planos de Assinatura](./SUBSCRIPTION_PLANS_MANAGEMENT.md)

---

**Última Atualização:** Janeiro 2026  
**Versão:** 1.0
