# Clinic Registration Refactoring - Progress Summary

## Objetivo
Reformular o processo de cadastro de clínicas de um modelo 1:1 (um proprietário, uma clínica) para um modelo 1:N (um proprietário/empresa pode ter múltiplas clínicas).

## Mudanças Arquiteturais

### Conceito de Tenant
**Antes:** TenantId = Clinic ID (cada clínica era um tenant isolado)
**Depois:** TenantId = Company ID (empresa é o tenant, clínicas pertencem à empresa)

## Progresso Realizado

### ✅ Fase 1: Modelo de Domínio e Repositórios (COMPLETO)

#### Novas Entidades Criadas:
1. **Company** (`src/MedicSoft.Domain/Entities/Company.cs`)
   - Representa uma empresa/CPF que pode ter uma ou mais clínicas
   - Campos: Name, TradeName, Document, DocumentType, Phone, Email, Subdomain
   - O Company agora é a entidade "tenant" do sistema

2. **UserClinicLink** (`src/MedicSoft.Domain/Entities/UserClinicLink.cs`)
   - Relacionamento N:N entre usuários e clínicas
   - Permite que um usuário trabalhe em múltiplas clínicas da mesma empresa
   - Campos: UserId, ClinicId, IsPreferredClinic (clínica padrão), IsActive

#### Entidades Atualizadas:
1. **Clinic** - Adicionado `CompanyId` para referenciar a empresa proprietária
2. **User** - Adicionado:
   - Collection `ClinicLinks` para múltiplas clínicas
   - `CurrentClinicId` para armazenar a clínica onde o usuário está trabalhando
   - Métodos: `SetCurrentClinic()`, `GetPreferredClinicId()`, `HasAccessToClinic()`

#### Repositórios Criados:
1. **ICompanyRepository** / **CompanyRepository**
   - `GetByDocumentAsync()` - Busca empresa por CPF/CNPJ
   - `GetBySubdomainAsync()` - Busca empresa por subdomínio
   - `IsSubdomainUniqueAsync()` - Valida unicidade de subdomínio
   - `GetCompanyClinicsAsync()` - Lista clínicas da empresa

2. **IUserClinicLinkRepository** / **UserClinicLinkRepository**
   - `GetByUserIdAsync()` - Lista clínicas de um usuário
   - `GetUserClinicsAsync()` - Retorna clínicas ativas do usuário
   - `UserHasAccessToClinicAsync()` - Verifica acesso do usuário à clínica

#### Configurações EF Core:
- `CompanyConfiguration.cs` - Configuração da entidade Company
- `UserClinicLinkConfiguration.cs` - Configuração do relacionamento usuário-clínica
- Atualização de `ClinicConfiguration.cs` com relacionamento para Company
- Atualização de `UserConfiguration.cs` com CurrentClinic
- DbContext atualizado com `Companies` e `UserClinicLinks`

## Próximos Passos Necessários

### ✅ Fase 2: Migração de Banco de Dados (COMPLETO)

#### Migration Criada: `20260123150022_AddCompanyAndMultiClinicSupport.cs`

1. ✅ Tabelas criadas:
   - `Companies` - Entidade de empresa/tenant
   - `UserClinicLinks` - Relacionamento N:N entre Users e Clinics
   
2. ✅ Colunas adicionadas:
   - `CompanyId` em `Clinics` (foreign key para Companies)
   - `CurrentClinicId` em `Users` (foreign key para Clinics)

3. ✅ Script de migração de dados incluído:
   - Cria Company para cada Clinic existente (agrupado por Document)
   - Migra dados de Clinic.Document para Company.Document
   - Vincula Clinics às Companies criadas
   - Cria UserClinicLink para cada User.ClinicId existente
   - Define User.CurrentClinicId para usuários existentes

4. ✅ Índices criados:
   - Índice único em Companies.Document
   - Índice único em Companies.Subdomain (filtered)
   - Índices de performance em UserClinicLinks
   - Foreign keys com ReferentialAction.Restrict

5. ✅ Documentação criada:
   - `PHASE2_MIGRATION_GUIDE.md` - Guia completo da migração
   - `scripts/phase2_migration_validation.sql` - Scripts de validação

#### Como Aplicar a Migration:
```bash
cd src/MedicSoft.Repository
dotnet ef database update --context MedicSoftDbContext
```

### ✅ Fase 3: Serviços Backend (COMPLETO)

1. ✅ **RegistrationService** - Refatorado para:
   - Criar Company em vez de usar Clinic como tenant
   - Usar Company.Subdomain como TenantId
   - Criar primeiro Clinic vinculado à Company
   - Manter compatibilidade com dados existentes

2. ✅ **AuthenticationService** - Atualizado para:
   - Retornar lista de clínicas disponíveis para o usuário
   - Definir CurrentClinicId na primeira autenticação
   - Validar acesso do usuário à clínica selecionada
   - LoginResponse inclui AvailableClinics e CurrentClinicId

3. ✅ **ClinicSelectionService** (novo) - Criado para:
   - Trocar clínica ativa do usuário
   - Validar permissões de acesso
   - Atualizar User.CurrentClinicId
   - Retornar lista de clínicas do usuário

4. ✅ **API Endpoints** - Criados:
   - `GET /api/users/clinics` - Lista clínicas disponíveis para o usuário
   - `GET /api/users/current-clinic` - Retorna clínica atual
   - `POST /api/users/select-clinic/{clinicId}` - Seleciona clínica ativa

5. ✅ **DTOs e Response Models**:
   - UserClinicDto - Representa clínica acessível ao usuário
   - SwitchClinicRequest/Response - Troca de clínica
   - LoginResponse atualizado com lista de clínicas

6. ⚠️ **PatientService** - Queries existentes mantidas:
   - Filtros por TenantId (Company) funcionam
   - Filtros por ClinicId já existem e funcionam
   - Nenhuma mudança necessária para funcionamento básico

7. ⚠️ **AppointmentService** - Queries existentes mantidas:
   - Filtros por clínica já existem
   - CurrentClinicId pode ser usado pelos controladores
   - Nenhuma mudança necessária para funcionamento básico

### ✅ Fase 4: API Endpoints (COMPLETO)
1. ✅ **Registration Endpoints**:
   - `POST /api/registration` - Atualizado para criar Company + Clinic
   - Mantém compatibilidade com campos legados (ClinicCNPJ)
   - Implementado em RegistrationController

2. ✅ **Clinic Selection Endpoints**:
   - `GET /api/users/clinics` - Lista clínicas disponíveis para o usuário
   - `POST /api/users/select-clinic/{clinicId}` - Seleciona clínica ativa
   - `GET /api/users/current-clinic` - Retorna clínica atual
   - Implementados em UsersController com ClinicSelectionService

3. ✅ **User Management Endpoints**:
   - `POST /api/users/{userId}/clinics` - Vincula usuário a clínica
   - `DELETE /api/users/{userId}/clinics/{clinicId}` - Remove vínculo
   - `PUT /api/users/{userId}/preferred-clinic/{clinicId}` - Define clínica preferencial
   - Implementados em UsersController com UserService
   - Requer permissão users.edit (ClinicOwner/Admin)
   - Incluem validações de segurança e tratamento de erros

### 🎨 Fase 5: Frontend - Site (Cadastro)
1. Atualizar formulário de registro:
   - Manter campos de "Empresa" (já existe suporte a CPF/CNPJ)
   - Clarificar que o cadastro é da empresa, não apenas da clínica
   - Adicionar campo "Nome da primeira clínica" (opcional, pode usar nome da empresa)

2. Atualizar textos e labels:
   - "Dados da Empresa" em vez de "Dados da Clínica"
   - Explicar que mais clínicas podem ser adicionadas depois

### 🖥️ Fase 6: Frontend - Sistema
1. **Topbar/Navbar**:
   - Adicionar seletor de clínica (dropdown)
   - Mostrar apenas se usuário tem acesso a múltiplas clínicas
   - Ícone de localização para indicar "onde você está"
   - Atualizar ao trocar de clínica

2. **Lista de Pacientes**:
   - Filtrar por agendamentos da clínica selecionada
   - Adicionar toggle "Ver todos os pacientes da empresa" (se permitido)

3. **Agenda/Schedule**:
   - Mostrar agenda da clínica selecionada
   - Adicionar visualização multi-clínica (com permissão)
   - Indicador visual de qual clínica cada agendamento pertence

4. **Gestão de Usuários**:
   - Adicionar seção "Clínicas de Acesso"
   - Checkboxes para selecionar clínicas que o usuário pode acessar
   - Opção de definir clínica preferencial

5. **Gestão de Clínicas** (novo módulo):
   - Listar todas as clínicas da empresa
   - Adicionar nova clínica
   - Editar clínica existente
   - Desativar clínica

### 🧪 Fase 7: Testes
1. Testes unitários para novas entidades
2. Testes de integração para repositories
3. Testes E2E para:
   - Fluxo de registro criando Company + Clinic
   - Usuário trocando entre clínicas
   - Acesso a pacientes através de clínicas diferentes
   - Permissões de visualização entre clínicas

## Considerações Importantes

### Compatibilidade com Dados Existentes
- A migração deve preservar todos os dados existentes
- Clinics existentes devem funcionar normalmente
- User.ClinicId deve ser mantido para backward compatibility
- PatientClinicLink continua funcionando

### Segurança
- Validar sempre que usuário tem acesso à clínica que está tentando acessar
- TenantId (Company) deve ser validado em todas as queries
- CurrentClinicId deve ser validado contra UserClinicLinks

### Performance
- Índices adicionados para UserClinicLinks
- Queries devem usar Include() apropriadamente
- Cache de clínicas disponíveis do usuário

## Comandos para Continuar

### Criar Migration
```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Repository
dotnet ef migrations add AddCompanyAndMultiClinicSupport --context MedicSoftDbContext --output-dir Migrations/PostgreSQL
```

### Build e Test
```bash
cd /home/runner/work/MW.Code/MW.Code
dotnet build
dotnet test
```

## Estimativa de Esforço Restante
- ~~Fase 2 (Migration): 4-6 horas~~ ✅ COMPLETO
- ~~Fase 3 (Backend Services): 8-12 horas~~ ✅ COMPLETO
- ~~Fase 4 (API Endpoints): 2-4 horas~~ ✅ COMPLETO
- Fase 5 (Frontend Site): 2-4 horas
- Fase 6 (Frontend Sistema): 12-16 horas
- Fase 7 (Testes): 8-12 horas

**Total estimado restante: 22-32 horas**

## Status Atual
✅ Fase 1: Modelo de domínio completo
✅ Fase 1: Repositórios implementados
✅ Fase 1: Configurações EF Core
✅ Fase 2: Migration de banco de dados criada
✅ Fase 2: Scripts de migração de dados incluídos
✅ Fase 2: Documentação completa
✅ Fase 3: RegistrationService refatorado
✅ Fase 3: ClinicSelectionService implementado
✅ Fase 3: AuthService atualizado
✅ Fase 3: API Endpoints principais criados
✅ Fase 3: DTOs implementados
✅ Fase 3: Dependency Injection configurado
✅ Fase 4: Endpoints de gestão de usuário-clínica implementados
✅ Fase 4: Testes unitários para novos métodos
✅ Fase 4: Code review e otimizações
✅ Build sem erros (API project)

**Próximo passo recomendado:** 
1. ~~Aplicar a migration em ambiente de desenvolvimento/teste~~ ✅ COMPLETO (Fase 2)
2. ~~Validar migração de dados com scripts em `scripts/phase2_migration_validation.sql`~~ ✅ COMPLETO (Fase 2)
3. ~~Iniciar Fase 3: Refatorar serviços backend~~ ✅ COMPLETO
4. ~~Testar manualmente o fluxo de registro e seleção de clínicas~~ ✅ COMPLETO (Fase 3)
5. ~~Iniciar Fase 4: Endpoints adicionais~~ ✅ COMPLETO
6. Iniciar Fase 5: Frontend - Atualizar site de registro
7. Iniciar Fase 6: Frontend - Implementar seletor de clínicas no sistema
4. Testar manualmente o fluxo de registro e seleção de clínicas
5. Iniciar Fase 4: Endpoints adicionais (opcional, endpoints principais já criados)
6. Iniciar Fase 5: Frontend - Atualizar site de registro
7. Iniciar Fase 6: Frontend - Implementar seletor de clínicas no sistema
