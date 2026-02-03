# Gerenciamento de Proprietários (Owners) - Guia Completo

## Índice
- [Visão Geral](#visão-geral)
- [Diferença entre Owner e User](#diferença-entre-owner-e-user)
- [Fluxo de Proprietários](#fluxo-de-proprietários)
- [Conceder Permissões de Owner](#conceder-permissões-de-owner)
- [Dashboard e Permissões](#dashboard-e-permissões)
- [Override Manual de Assinatura](#override-manual-de-assinatura)

---

## Visão Geral

O sistema Omni Care Software possui um fluxo separado para gerenciamento de **Proprietários (Owners)** de clínicas, distinto do fluxo de **Usuários (Users)**. Esta separação permite um melhor controle e gerenciamento das permissões e responsabilidades dentro do sistema.

---

## Diferença entre Owner e User

### Owner (Proprietário)
- **Função**: Proprietário e administrador principal da clínica
- **Permissões**: Controle total sobre a clínica, incluindo gerenciamento de usuários, assinatura e configurações
- **Entidade**: Entidade separada `Owner` no banco de dados
- **Criação**: Criado durante o registro da clínica ou por SystemAdmin
- **Obrigatório**: Uma clínica sempre tem um Owner associado

### User (Usuário)
- **Função**: Profissionais e colaboradores da clínica (médicos, dentistas, enfermeiros, recepcionistas, secretárias)
- **Permissões**: Permissões baseadas em roles (Doctor, Dentist, Nurse, Receptionist, Secretary, ClinicOwner)
- **Entidade**: Entidade `User` no banco de dados
- **Criação**: Criado pelo Owner ou por outro usuário com permissões adequadas
- **Opcional**: Uma clínica pode ter múltiplos usuários

---

## Fluxo de Proprietários

### APIs Disponíveis

#### 1. Registro de Clínica com Owner
**Endpoint**: `POST /api/registration`

Registra uma nova clínica e cria automaticamente o Owner associado.

**Request Body**:
```json
{
  "clinicName": "Clínica Exemplo",
  "clinicCNPJ": "12345678000195",
  "clinicPhone": "(11) 98765-4321",
  "clinicEmail": "contato@clinica.com",
  "street": "Rua Exemplo",
  "number": "123",
  "complement": "Sala 4",
  "neighborhood": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "zipCode": "01234-567",
  "ownerName": "João da Silva",
  "ownerCPF": "12345678901",
  "ownerPhone": "(11) 99999-8888",
  "ownerEmail": "joao@clinica.com",
  "username": "joao.silva",
  "password": "Senha@Forte123",
  "planId": "guid-do-plano",
  "subdomain": "clinica-exemplo"
}
```

#### 2. Login de Owner
**Endpoint**: `POST /api/owners/login`

Autentica um Owner e retorna o token JWT.

**Request Body**:
```json
{
  "username": "joao.silva",
  "password": "Senha@Forte123"
}
```

#### 3. Perfil do Owner
**Endpoint**: `GET /api/owners/me`

Retorna informações do Owner autenticado.

#### 4. Gerenciamento de Usuários pelo Owner
**Endpoint**: `GET /api/owners/users`

Lista todos os usuários da clínica do Owner autenticado.

---

## Conceder Permissões de Owner

A permissão de owner pode ser concedida a qualquer usuário pelo proprietário atual da clínica. Isso permite que múltiplas pessoas gerenciem a clínica com privilégios administrativos completos.

### Casos de Uso
- Clínicas com múltiplos sócios/proprietários
- Delegação de responsabilidades administrativas
- Acesso administrativo temporário para pessoal específico

### Método 1: Criar Novo Usuário com Role de Owner

1. **Login como Owner**
   - Acesse a aplicação
   - Use as credenciais de owner
   - ✅ Marque "Login como Proprietário"

2. **Navegue para Gerenciamento de Usuários**
   - Clique em "Administração" na barra de navegação
   - Selecione "Usuários"

3. **Criar Novo Usuário**
   - Clique em "Novo Usuário" ou "Adicionar Usuário"
   - Preencha as informações obrigatórias:
     - Nome Completo
     - Email
     - Telefone
     - Username
     - Senha
     - **Role: Selecione "Owner"** ⭐

4. **Salvar**
   - O novo usuário terá privilégios completos de owner
   - Compartilhe as credenciais de forma segura
   - Informe-os para usar "Login como Proprietário" ao fazer login

### Método 2: Alterar Role de Usuário Existente para Owner

1. **Login como Owner**
   - Acesse com credenciais de owner

2. **Navegue para Gerenciamento de Usuários**
   - Clique em "Administração" → "Usuários"

3. **Selecione o Usuário para Promover**
   - Encontre o usuário na lista
   - Clique em "Editar" ou nos detalhes do usuário

4. **Alterar Role**
   - Localize o campo "Role"
   - Altere para "Owner"
   - Salvar as alterações

### Método 3: Via API

**Endpoint**: `PUT /api/owners/users/{userId}/role`

```json
{
  "role": "ClinicOwner"
}
```

---

## Dashboard e Permissões

### Acesso ao Dashboard

Owners têm acesso a um dashboard dedicado com:
- Visão geral da clínica
- Estatísticas de atendimentos
- Gestão financeira
- Controle de usuários
- Configurações da clínica

### Permissões Granulares

O sistema permite controle granular de permissões para cada Owner:

1. **Gerenciamento de Usuários**
   - Criar, editar e desativar usuários
   - Atribuir roles e permissões
   - Visualizar histórico de acesso

2. **Gestão Financeira**
   - Visualizar relatórios financeiros
   - Gerenciar planos e assinaturas
   - Controlar pagamentos

3. **Configurações da Clínica**
   - Alterar dados cadastrais
   - Configurar especialidades e procedimentos
   - Gerenciar integração com convênios

4. **Controle de Acesso**
   - Configurar horários de funcionamento
   - Gerenciar agendas
   - Controlar acesso de usuários

---

## Override Manual de Assinatura

### Visão Geral

Permite ao SystemAdmin manter uma clínica ativa mesmo que:
- O pagamento da mensalidade esteja em atraso
- A clínica não tenha sido cadastrada pelo site
- A clínica esteja em período de teste

### Casos de Uso
- Oferecer acesso gratuito para amigos médicos
- Manter acesso para clínicas parceiras
- Facilitar testes e demonstrações

### API Endpoints

#### Ativar Override Manual

```http
POST /api/system-admin/clinics/{clinicId}/subscription/manual-override/enable
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Acesso gratuito para amigo médico"
}
```

**Resposta de Sucesso**:
```json
{
  "message": "Override manual ativado com sucesso",
  "reason": "Acesso gratuito para amigo médico",
  "setBy": "admin@medicwarehouse.com",
  "setAt": "2025-10-12T03:15:00Z"
}
```

#### Desativar Override Manual

```http
POST /api/system-admin/clinics/{clinicId}/subscription/manual-override/disable
Authorization: Bearer {token}
```

### Controle de Ambientes

O sistema diferencia ambientes (Dev/Staging/Production) para:
- Dados de teste vs. dados reais
- Segurança adicional em produção
- Facilitar debugging em desenvolvimento

---

## Boas Práticas

### Segurança
- ✅ Use senhas fortes para todas as contas de Owner
- ✅ Ative autenticação de dois fatores (2FA) quando disponível
- ✅ Revise regularmente os usuários com permissões de Owner
- ✅ Documente mudanças de permissões

### Gestão
- ✅ Mantenha pelo menos 2 owners por clínica (redundância)
- ✅ Use o sistema de override manual apenas quando necessário
- ✅ Documente o motivo do override manual
- ✅ Revise permissões periodicamente

### Auditoria
- ✅ Monitore logs de acesso de Owners
- ✅ Revise ações administrativas regularmente
- ✅ Mantenha histórico de mudanças de permissões

---

## Troubleshooting

### Owner não consegue fazer login
1. Verifique se o checkbox "Login como Proprietário" está marcado
2. Confirme que a clínica está ativa
3. Verifique se a assinatura está válida ou se há override manual

### Owner não vê opções administrativas
1. Confirme que o usuário tem role "ClinicOwner"
2. Verifique se está logado como Owner (não como User)
3. Limpe o cache do navegador

### Erro ao criar novo Owner
1. Verifique se o email/username já existe
2. Confirme que a senha atende aos requisitos de segurança
3. Verifique se o usuário logado tem permissões adequadas

---

## Documentos Relacionados

- [QUICK_REFERENCE_PERMISSIONS.md](./QUICK_REFERENCE_PERMISSIONS.md) - Referência rápida de permissões
- [ACCESS_PROFILES_DOCUMENTATION.md](./ACCESS_PROFILES_DOCUMENTATION.md) - Perfis de acesso
- [SYSTEM_OWNER_ACCESS.md](./SYSTEM_OWNER_ACCESS.md) - Acesso de system admin

---

**Última Atualização**: Fevereiro 2026  
**Consolidado de**: OWNER_FLOW_DOCUMENTATION.md, GRANTING_OWNER_PERMISSIONS.md, OWNER_DASHBOARD_PERMISSIONS.md
