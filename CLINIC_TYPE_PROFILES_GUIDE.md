# Guia de Perfis Multi-Profissionais por Tipo de Clínica

## Visão Geral

Este documento descreve as implementações realizadas para suportar diferentes tipos de clínicas e perfis profissionais no sistema MedicWarehouse/Omni Care.

## Problema Resolvido

Anteriormente, o sistema criava apenas perfis médicos (Médico, Recepção, Financeiro, Proprietário) independentemente do tipo de clínica registrada. Agora, o sistema cria perfis apropriados baseados no tipo de clínica escolhida durante o registro.

## Tipos de Clínica Suportados

O sistema suporta os seguintes tipos de clínica (enum `ClinicType`):

1. **Medical (0)** - Clínica médica geral
2. **Dental (1)** - Clínica odontológica  
3. **Nutritionist (2)** - Clínica de nutrição
4. **Psychology (3)** - Clínica de psicologia/terapia
5. **PhysicalTherapy (4)** - Clínica de fisioterapia
6. **Veterinary (5)** - Clínica veterinária
7. **Other (99)** - Outras especialidades

## Perfis de Acesso Criados

### Perfis Comuns (Todas as Clínicas)

Todos os tipos de clínica recebem estes perfis básicos:

#### 1. Proprietário
- **Nome:** "Proprietário"
- **Descrição:** "Acesso total à clínica - pode gerenciar tudo"
- **Permissões:** Acesso completo a todas as funcionalidades

#### 2. Recepção/Secretaria
- **Nome:** "Recepção/Secretaria"
- **Descrição:** "Acesso de recepção - agendamentos, pacientes e pagamentos"
- **Permissões:**
  - Pacientes (visualizar, criar, editar)
  - Agendamentos (completo)
  - Prontuários médicos (apenas visualizar)
  - Procedimentos (apenas visualizar)
  - Pagamentos (visualizar e gerenciar)
  - Notificações
  - Fila de espera

#### 3. Financeiro
- **Nome:** "Financeiro"
- **Descrição:** "Acesso financeiro - pagamentos, despesas e relatórios"
- **Permissões:**
  - Pacientes (apenas visualizar para fins de cobrança)
  - Agendamentos (apenas visualizar)
  - Procedimentos (apenas visualizar)
  - Pagamentos e faturas (completo)
  - Despesas (completo)
  - Relatórios financeiros
  - Notificações

### Perfis Específicos por Tipo de Clínica

#### Clínica Médica (Medical)
**Perfil: Médico** - Atendimento médico completo com prontuários e prescrições

#### Clínica Odontológica (Dental)
**Perfil: Dentista** - Atendimento odontológico com odontograma e procedimentos dentários

#### Clínica de Nutrição (Nutritionist)
**Perfil: Nutricionista** - Atendimento nutricional com planos alimentares e avaliação antropométrica

#### Clínica de Psicologia (Psychology)
**Perfil: Psicólogo** - Atendimento psicológico com anotações de sessão e avaliação terapêutica

#### Clínica de Fisioterapia (PhysicalTherapy)
**Perfil: Fisioterapeuta** - Atendimento fisioterapêutico com avaliação de movimento e exercícios

#### Clínica Veterinária (Veterinary)
**Perfil: Veterinário** - Atendimento veterinário com prontuário animal e procedimentos veterinários

## Implementação Técnica

### Backend

#### 1. Entidade AccessProfile (Domain)
Novos métodos estáticos para criar perfis específicos:
- `CreateDefaultDentistProfile()`
- `CreateDefaultNutritionistProfile()`
- `CreateDefaultPsychologistProfile()`
- `CreateDefaultPhysicalTherapistProfile()`
- `CreateDefaultVeterinarianProfile()`

Arquivo: `/src/MedicSoft.Domain/Entities/AccessProfile.cs`

#### 2. AccessProfileService (Application)
Novo método para criar perfis baseados no tipo de clínica:
```csharp
Task<IEnumerable<AccessProfileDto>> CreateDefaultProfilesForClinicTypeAsync(
    Guid clinicId, 
    string tenantId, 
    ClinicType clinicType)
```

Arquivo: `/src/MedicSoft.Application/Services/AccessProfileService.cs`

#### 3. RegistrationService (Application)
Atualizado para criar perfis específicos durante o registro:
- Método privado `GetDefaultProfilesForClinicType()` determina quais perfis criar
- Integração com o fluxo de registro de clínicas

Arquivo: `/src/MedicSoft.Application/Services/RegistrationService.cs`

#### 4. AccessProfilesController (API)
Novo endpoint para clínicas existentes criarem perfis baseados no tipo:
```
POST /api/accessprofiles/create-defaults-by-type
```

Arquivo: `/src/MedicSoft.Api/Controllers/AccessProfilesController.cs`

## Uso

### Para Novas Clínicas

Durante o registro, o sistema automaticamente:
1. Detecta o tipo de clínica escolhido
2. Cria os perfis comuns (Proprietário, Recepção, Financeiro)
3. Cria o perfil profissional específico (Médico, Dentista, Nutricionista, etc.)

### Para Clínicas Existentes

Proprietários de clínicas podem criar os perfis específicos para seu tipo de clínica:

**Endpoint API:**
```
POST /api/accessprofiles/create-defaults-by-type
Authorization: Bearer {token}
```

### Atribuir Perfil a um Usuário

**Endpoint API:**
```
POST /api/accessprofiles/assign
Content-Type: application/json
Authorization: Bearer {token}

{
  "userId": "user-guid",
  "profileId": "profile-guid"
}
```

## Segurança

### Controle de Acesso

- Apenas proprietários (ClinicOwner) e administradores do sistema (SystemAdmin) podem:
  - Criar perfis de acesso
  - Editar perfis personalizados (perfis padrão são read-only)
  - Excluir perfis não utilizados
  - Atribuir perfis a usuários

- Perfis padrão (`IsDefault = true`) não podem ser:
  - Modificados
  - Excluídos
  - Desativados

## Conclusão

A implementação de perfis específicos por tipo de clínica é o primeiro passo para tornar o sistema verdadeiramente multi-profissional. Com esta base, cada especialidade pode ter sua experiência de atendimento otimizada para suas necessidades específicas.

O sistema agora:
- ✅ Cria perfis apropriados durante o registro baseado no tipo de clínica
- ✅ Permite que clínicas existentes criem perfis específicos para seu tipo
- ✅ Suporta 6 tipos diferentes de clínicas profissionais
- ✅ Mantém controle de acesso granular por perfil
- ✅ Permite atribuição flexível de perfis a usuários

**Próximo passo:** Implementar as telas de atendimento especializadas para cada tipo de clínica no frontend.
