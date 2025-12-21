# Sistema de Perfis de Acesso e Permissões

## Visão Geral

O MedicWarehouse implementa um sistema completo de perfis de acesso e permissões granulares, permitindo que proprietários de clínicas gerenciem de forma flexível quais funcionalidades cada usuário pode acessar.

## Arquitetura

### Modelo de Permissões

O sistema segue o padrão **RBAC (Role-Based Access Control)** com perfis personalizáveis:

- **Perfis (AccessProfile)**: Coleções nomeadas de permissões
- **Permissões (ProfilePermission)**: Ações granulares em recursos
- **Usuários**: Associados a um perfil que define suas permissões

### Formato de Permissões

As permissões seguem o padrão `recurso.ação`:

```
patients.view          → Visualizar pacientes
patients.create        → Criar pacientes
appointments.edit      → Editar agendamentos
medical-records.view   → Visualizar prontuários
```

## Perfis Padrão

Ao registrar uma nova clínica, o sistema cria automaticamente 4 perfis padrão:

### 1. Proprietário (Owner)
**Acesso total** à clínica - pode gerenciar tudo

**Permissões:**
- Gestão da clínica e configurações
- Gerenciamento de usuários e perfis
- Todos os recursos de pacientes, agendamentos e atendimentos
- Acesso financeiro completo (pagamentos, notas, despesas, relatórios)
- Procedimentos, medicações, exames
- Notificações e fila de espera

### 2. Médico (Medical)
**Acesso médico** - atendimento, prontuários e prescrições

**Permissões:**
- Visualizar, criar e editar pacientes
- Visualizar, criar e editar agendamentos
- Acesso completo a prontuários médicos
- Realizar atendimentos
- Visualizar procedimentos
- Criar prescrições e solicitar exames
- Visualizar notificações e fila de espera

### 3. Recepção/Secretaria (Reception)
**Acesso de recepção** - agendamentos, pacientes e pagamentos

**Permissões:**
- Gerenciar pacientes (criar, editar)
- Gerenciar agendamentos (criar, editar, deletar)
- Visualizar prontuários (somente leitura)
- Visualizar procedimentos
- Gerenciar pagamentos
- Gerenciar notificações e fila de espera

### 4. Financeiro (Financial)
**Acesso financeiro** - pagamentos, despesas e relatórios

**Permissões:**
- Visualizar pacientes e agendamentos
- Visualizar procedimentos
- Gerenciar pagamentos e notas fiscais
- Gerenciar despesas
- Visualizar relatórios financeiros
- Visualizar notificações

## Categorias de Permissões

### Gestão da Clínica
- `clinic.view` - Visualizar configurações da clínica
- `clinic.manage` - Gerenciar configurações da clínica

### Usuários
- `users.view` - Visualizar usuários
- `users.create` - Criar usuários
- `users.edit` - Editar usuários
- `users.delete` - Excluir usuários

### Perfis de Acesso
- `profiles.view` - Visualizar perfis de acesso
- `profiles.create` - Criar perfis de acesso
- `profiles.edit` - Editar perfis de acesso
- `profiles.delete` - Excluir perfis de acesso

### Pacientes
- `patients.view` - Visualizar pacientes
- `patients.create` - Cadastrar pacientes
- `patients.edit` - Editar pacientes
- `patients.delete` - Excluir pacientes

### Agendamentos
- `appointments.view` - Visualizar agendamentos
- `appointments.create` - Criar agendamentos
- `appointments.edit` - Editar agendamentos
- `appointments.delete` - Excluir agendamentos

### Prontuários
- `medical-records.view` - Visualizar prontuários
- `medical-records.create` - Criar prontuários
- `medical-records.edit` - Editar prontuários

### Atendimento
- `attendance.view` - Visualizar atendimentos
- `attendance.perform` - Realizar atendimentos

### Procedimentos
- `procedures.view` - Visualizar procedimentos
- `procedures.create` - Criar procedimentos
- `procedures.edit` - Editar procedimentos
- `procedures.delete` - Excluir procedimentos

### Financeiro - Pagamentos
- `payments.view` - Visualizar pagamentos
- `payments.manage` - Gerenciar pagamentos

### Financeiro - Notas Fiscais
- `invoices.view` - Visualizar notas fiscais
- `invoices.manage` - Gerenciar notas fiscais

### Financeiro - Despesas
- `expenses.view` - Visualizar despesas
- `expenses.create` - Criar despesas
- `expenses.edit` - Editar despesas
- `expenses.delete` - Excluir despesas

### Relatórios
- `reports.financial` - Visualizar relatórios financeiros
- `reports.operational` - Visualizar relatórios operacionais

### Medicamentos e Prescrições
- `medications.view` - Visualizar medicamentos
- `prescriptions.create` - Criar prescrições

### Exames
- `exams.view` - Visualizar exames
- `exams.request` - Solicitar exames

### Notificações
- `notifications.view` - Visualizar notificações
- `notifications.manage` - Gerenciar notificações

### Fila de Espera
- `waiting-queue.view` - Visualizar fila de espera
- `waiting-queue.manage` - Gerenciar fila de espera

## API Endpoints

### Listar Perfis
```http
GET /api/access-profiles
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Resposta:**
```json
[
  {
    "id": "uuid",
    "name": "Médico",
    "description": "Acesso médico - atendimento, prontuários e prescrições",
    "isDefault": true,
    "isActive": true,
    "clinicId": "uuid",
    "clinicName": "Clínica ABC",
    "createdAt": "2025-01-01T00:00:00Z",
    "updatedAt": null,
    "permissions": [
      "patients.view",
      "patients.create",
      "appointments.view",
      "medical-records.create"
    ],
    "userCount": 3
  }
]
```

### Obter Perfil por ID
```http
GET /api/access-profiles/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

### Criar Perfil
```http
POST /api/access-profiles
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "name": "Enfermeiro",
  "description": "Acesso de enfermagem",
  "permissions": [
    "patients.view",
    "appointments.view",
    "medical-records.view",
    "medical-records.edit"
  ]
}
```

**Nota:** O `clinicId` é extraído automaticamente do token JWT.

### Atualizar Perfil
```http
PUT /api/access-profiles/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "name": "Enfermeiro Atualizado",
  "description": "Descrição atualizada",
  "permissions": [
    "patients.view",
    "medical-records.edit"
  ]
}
```

**Importante:** Não é possível modificar perfis padrão.

### Excluir Perfil
```http
DELETE /api/access-profiles/{id}
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Validações:**
- Não é possível excluir perfis padrão
- Não é possível excluir perfis em uso por usuários

### Listar Todas as Permissões
```http
GET /api/access-profiles/permissions
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Resposta:**
```json
[
  {
    "category": "Pacientes",
    "permissions": [
      {
        "key": "patients.view",
        "description": "Visualizar pacientes"
      },
      {
        "key": "patients.create",
        "description": "Cadastrar pacientes"
      }
    ]
  }
]
```

### Atribuir Perfil a Usuário
```http
POST /api/access-profiles/assign
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "userId": "uuid",
  "profileId": "uuid"
}
```

### Criar Perfis Padrão
```http
POST /api/access-profiles/create-defaults
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
```

**Nota:** Este endpoint é chamado automaticamente durante o registro da clínica.

## Verificação de Permissões

### No Backend (C#)

#### Método no Domain Entity User:
```csharp
public bool HasPermissionKey(string permissionKey)
{
    // Se usuário tem um perfil, usa permissões do perfil
    if (Profile != null && Profile.IsActive)
    {
        return Profile.HasPermission(permissionKey);
    }
    
    // Fallback para permissões baseadas em role (compatibilidade)
    return HasLegacyPermission(permissionKey);
}
```

#### Uso em Controllers:
```csharp
[HttpPost]
public async Task<ActionResult> SomeAction()
{
    var user = await GetCurrentUser();
    
    if (!user.HasPermissionKey("patients.create"))
    {
        return Forbid();
    }
    
    // Lógica da ação...
}
```

### No Frontend (Angular)

#### Usando o Service:
```typescript
export class SomeComponent implements OnInit {
  constructor(private authService: Auth) {}
  
  ngOnInit() {
    // Verificar permissão específica
    if (this.hasPermission('patients.create')) {
      // Mostrar botão de criar paciente
    }
  }
  
  hasPermission(key: string): boolean {
    const user = this.authService.currentUser();
    return user?.profile?.permissions.includes(key) ?? false;
  }
}
```

#### Diretiva Estrutural (TODO):
```html
<button *hasPermission="'patients.create'" (click)="createPatient()">
  Novo Paciente
</button>
```

## Interface do Usuário

### Tela de Listagem de Perfis

**Acesso:** Menu Admin > Perfis de Acesso ou `/admin/profiles`

**Funcionalidades:**
- Visualizar todos os perfis da clínica
- Indicador visual para perfis padrão
- Contador de permissões e usuários
- Botões de ação (Editar/Excluir)
- Botão para criar novo perfil

### Tela de Criação/Edição de Perfil

**Acesso:** `/admin/profiles/new` ou `/admin/profiles/edit/{id}`

**Campos:**
- **Nome do Perfil:** Campo de texto obrigatório
- **Descrição:** Textarea obrigatório
- **Permissões:** Checkboxes agrupados por categoria

**Funcionalidades:**
- Seleção individual de permissões
- Botões "Selecionar todos" e "Limpar" por categoria
- Validação de campos obrigatórios
- Validação de pelo menos uma permissão

## Segurança

### Autorizações

1. **Somente Proprietários** podem acessar as funcionalidades de perfis:
   - O controller verifica se o usuário tem role `ClinicOwner` ou `SystemAdmin`
   
2. **Isolamento por Tenant:**
   - Todos os endpoints verificam o `TenantId` do token
   - Perfis são sempre filtrados pelo tenant da clínica

3. **Proteção de Perfis Padrão:**
   - Perfis padrão não podem ser editados ou excluídos
   - Flag `IsDefault` previne modificações acidentais

4. **Validação de Uso:**
   - Sistema verifica se perfil está em uso antes de permitir exclusão
   - Evita inconsistências de dados

### Boas Práticas

1. **Princípio do Menor Privilégio:**
   - Crie perfis com apenas as permissões necessárias
   - Revise permissões periodicamente

2. **Separação de Responsabilidades:**
   - Use perfis diferentes para funções diferentes
   - Evite dar acesso desnecessário

3. **Auditoria:**
   - Monitore atribuições de perfis
   - Revise usuários com perfis administrativos

## Integração com Registro de Clínica

Ao registrar uma nova clínica, o sistema automaticamente:

1. Cria a clínica e o proprietário
2. Cria 4 perfis padrão (Owner, Medical, Reception, Financial)
3. Associa o proprietário ao perfil "Proprietário"

**Código em RegistrationService:**
```csharp
// Criar perfis padrão para a clínica
var defaultProfiles = new[]
{
    AccessProfile.CreateDefaultOwnerProfile(tenantId, clinic.Id),
    AccessProfile.CreateDefaultMedicalProfile(tenantId, clinic.Id),
    AccessProfile.CreateDefaultReceptionProfile(tenantId, clinic.Id),
    AccessProfile.CreateDefaultFinancialProfile(tenantId, clinic.Id)
};

foreach (var profile in defaultProfiles)
{
    await _accessProfileRepository.AddAsync(profile);
}
```

## Migração de Dados

### Estrutura do Banco de Dados

**Tabela: AccessProfiles**
```sql
CREATE TABLE "AccessProfiles" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(100) NOT NULL,
    "Description" varchar(500) NOT NULL,
    "IsDefault" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "ClinicId" uuid,
    "TenantId" varchar(100) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    FOREIGN KEY ("ClinicId") REFERENCES "Clinics"("Id")
);

CREATE INDEX "IX_AccessProfiles_TenantId_ClinicId_Name" 
    ON "AccessProfiles" ("TenantId", "ClinicId", "Name");
```

**Tabela: ProfilePermissions**
```sql
CREATE TABLE "ProfilePermissions" (
    "Id" uuid PRIMARY KEY,
    "ProfileId" uuid NOT NULL,
    "PermissionKey" varchar(100) NOT NULL,
    "IsActive" boolean NOT NULL,
    "TenantId" varchar(100) NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    FOREIGN KEY ("ProfileId") REFERENCES "AccessProfiles"("Id") ON DELETE CASCADE,
    UNIQUE ("ProfileId", "PermissionKey")
);
```

**Atualização na Tabela Users:**
```sql
ALTER TABLE "Users" ADD COLUMN "ProfileId" uuid;
ALTER TABLE "Users" ADD CONSTRAINT "FK_Users_AccessProfiles_ProfileId" 
    FOREIGN KEY ("ProfileId") REFERENCES "AccessProfiles"("Id");
CREATE INDEX "IX_Users_ProfileId" ON "Users" ("ProfileId");
```

### Executar Migration

```bash
# Aplicar migration ao banco de dados
cd src/MedicSoft.Repository
dotnet ef database update --context MedicSoftDbContext
```

## Casos de Uso

### Caso 1: Criar Perfil para Enfermeiro

1. Proprietário acessa `/admin/profiles`
2. Clica em "Novo Perfil"
3. Preenche:
   - Nome: "Enfermeiro"
   - Descrição: "Acesso de enfermagem para triagem e procedimentos"
4. Seleciona permissões:
   - Pacientes: view, create, edit
   - Agendamentos: view
   - Prontuários: view, edit
   - Procedimentos: view
5. Salva o perfil

### Caso 2: Atribuir Perfil a Novo Usuário

1. Proprietário cria novo usuário
2. Seleciona perfil "Enfermeiro" durante a criação
3. Sistema associa o perfil ao usuário
4. Usuário tem acesso apenas às funcionalidades permitidas

### Caso 3: Modificar Permissões de um Perfil

1. Proprietário acessa lista de perfis
2. Clica em "Editar" no perfil desejado
3. Adiciona ou remove permissões
4. Salva as alterações
5. Todos os usuários com este perfil têm suas permissões atualizadas automaticamente

## Comparação com Ferramentas de Mercado

### Baseado em:

1. **iClinic** - Sistema brasileiro de gestão de clínicas
2. **Doctoralia** - Plataforma de agendamento e gestão
3. **MedPlus** - Software médico nacional

### Implementação Similar:

✅ Perfis personalizáveis por clínica  
✅ Permissões granulares por tela e ação  
✅ Perfis padrão pré-configurados  
✅ Interface visual para gestão  
✅ Atribuição simples de perfis a usuários  
✅ Proteção de perfis críticos  
✅ Auditoria de acesso  

## Troubleshooting

### Erro: "Profile not found"
- Verifique se o `TenantId` está correto
- Confirme que o perfil pertence à clínica do usuário

### Erro: "Cannot delete default profiles"
- Perfis padrão não podem ser excluídos
- Crie um novo perfil personalizado se necessário

### Erro: "Cannot delete profile that is assigned to users"
- Reatribua os usuários a outro perfil antes de excluir
- Ou desative o perfil em vez de excluí-lo

### Usuário não vê funcionalidades esperadas
- Verifique se o perfil está ativo
- Confirme que as permissões corretas estão associadas ao perfil
- Limpe o cache do navegador e faça novo login

## Roadmap Futuro

### Curto Prazo
- [ ] Incluir informações do perfil no JWT
- [ ] Diretiva Angular para ocultar elementos sem permissão
- [ ] Auditoria de alterações em perfis
- [ ] Histórico de atribuições de perfis

### Médio Prazo
- [ ] Permissões temporárias com data de expiração
- [ ] Delegação de permissões
- [ ] Perfis hierárquicos (perfis que herdam de outros)
- [ ] Relatórios de uso de permissões

### Longo Prazo
- [ ] Permissões baseadas em contexto (horário, localização)
- [ ] Aprovação multi-nível para ações sensíveis
- [ ] Integração com Single Sign-On (SSO)
- [ ] Machine Learning para sugestão de permissões

## Conclusão

O sistema de perfis de acesso e permissões do MedicWarehouse oferece controle granular e flexível sobre o que cada usuário pode fazer na plataforma, seguindo as melhores práticas de segurança e experiência do usuário das principais ferramentas do mercado.
