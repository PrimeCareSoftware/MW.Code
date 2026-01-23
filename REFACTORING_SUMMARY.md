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

### 🔄 Fase 2: Migração de Banco de Dados
1. Criar migration para adicionar tabelas:
   - `Companies`
   - `UserClinicLinks`
   - Coluna `CompanyId` em `Clinics`
   - Coluna `CurrentClinicId` em `Users`

2. Script de migração de dados:
   - Criar Company para cada Clinic existente
   - Migrar dados de Clinic.Document para Company.Document
   - Vincular Clinics às Companies criadas
   - Criar UserClinicLink para cada User.ClinicId existente

### 📝 Fase 3: Serviços Backend
1. **RegistrationService** - Refatorar para:
   - Criar Company em vez de usar Clinic como tenant
   - Usar Company.Subdomain como TenantId
   - Criar primeiro Clinic vinculado à Company
   - Criar UserClinicLink para o primeiro usuário

2. **AuthenticationService** - Atualizar para:
   - Retornar lista de clínicas disponíveis para o usuário
   - Armazenar clínica selecionada no token/sessão
   - Validar acesso do usuário à clínica selecionada

3. **ClinicSelectionService** (novo) - Criar para:
   - Trocar clínica ativa do usuário
   - Validar permissões de acesso
   - Atualizar User.CurrentClinicId

4. **PatientService** - Refatorar queries:
   - Filtrar por TenantId (Company) em vez de ClinicId
   - Adicionar filtro opcional por ClinicId quando necessário

5. **AppointmentService** - Refatorar para:
   - Filtrar agendamentos pela clínica atual do usuário
   - Permitir visualização de outras clínicas se configurado no perfil

### 🌐 Fase 4: API Endpoints
1. **Registration Endpoints**:
   - `POST /api/registration` - Atualizar para criar Company + Clinic
   - Manter compatibilidade com campos legados (ClinicCNPJ)

2. **Clinic Selection Endpoints** (novos):
   - `GET /api/user/clinics` - Lista clínicas disponíveis para o usuário
   - `POST /api/user/select-clinic/{clinicId}` - Seleciona clínica ativa
   - `GET /api/user/current-clinic` - Retorna clínica atual

3. **User Management Endpoints**:
   - `POST /api/users/{userId}/clinics` - Vincula usuário a clínica
   - `DELETE /api/users/{userId}/clinics/{clinicId}` - Remove vínculo
   - `PUT /api/users/{userId}/preferred-clinic/{clinicId}` - Define clínica preferencial

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
- Fase 2 (Migration): 4-6 horas
- Fase 3 (Backend Services): 8-12 horas
- Fase 4 (API): 4-6 horas
- Fase 5 (Frontend Site): 2-4 horas
- Fase 6 (Frontend Sistema): 12-16 horas
- Fase 7 (Testes): 8-12 horas

**Total estimado: 38-56 horas**

## Status Atual
✅ Modelo de domínio completo
✅ Repositórios implementados
✅ Configurações EF Core
✅ Build sem erros

**Próximo passo recomendado:** Criar a migration de banco de dados e script de migração de dados.
