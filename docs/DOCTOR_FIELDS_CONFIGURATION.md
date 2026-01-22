# Configuração de Campos para Médicos

Este guia explica como configurar os campos obrigatórios para cadastro de médicos no sistema.

## Visão Geral

O sistema PrimeCare permite que o proprietário da clínica configure se os campos específicos de médicos (CRM e Especialidade) são obrigatórios ou opcionais ao criar/editar usuários com perfil "Médico".

## Campos Disponíveis

### CRM / Registro Profissional
- Campo para o número do Conselho Regional de Medicina (CRM) ou outro registro profissional
- Exemplo: `CRM-123456`
- Pode ser configurado como obrigatório ou opcional

### Especialidade
- Campo para a especialidade médica do profissional
- Exemplo: `Cardiologia`, `Clínico Geral`, `Pediatria`
- Pode ser configurado como obrigatório ou opcional

## Como Configurar

### Opção 1: Via API (Backend)

#### Consultar Configuração Atual

```http
GET /api/ClinicAdmin/doctor-fields-config
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "professionalIdRequired": false,
  "specialtyRequired": false
}
```

#### Atualizar Configuração

```http
PUT /api/ClinicAdmin/doctor-fields-config
Authorization: Bearer {token}
Content-Type: application/json

{
  "professionalIdRequired": true,
  "specialtyRequired": true
}
```

**Resposta:** `204 No Content`

### Opção 2: Via Interface (Frontend)

1. Acesse o sistema como **Proprietário da Clínica**
2. Navegue até **Configurações da Clínica**
3. Na seção **Gestão de Usuários**, encontre as configurações de campos para médicos
4. Marque/desmarque as opções conforme desejado:
   - ☑️ **CRM Obrigatório**: Todos os médicos devem ter CRM cadastrado
   - ☑️ **Especialidade Obrigatória**: Todos os médicos devem ter especialidade cadastrada
5. Clique em **Salvar Configurações**

## Comportamento no Cadastro de Usuários

### Quando os Campos são Opcionais (Padrão)

Ao criar um usuário com perfil "Médico":
- Os campos CRM e Especialidade aparecem no formulário
- Não há validação obrigatória
- O usuário pode ser salvo sem preencher esses campos

### Quando os Campos são Obrigatórios

Ao criar um usuário com perfil "Médico":
- Os campos CRM e Especialidade aparecem no formulário com asterisco (*)
- A validação obrigatória é aplicada
- O formulário não pode ser salvo sem preencher os campos obrigatórios
- Mensagens de erro são exibidas se tentar salvar sem preencher

**Mensagens de erro:**
- "Professional ID (CRM) is required for doctors in this clinic"
- "Specialty is required for doctors in this clinic"

### Para Outros Perfis

Os campos CRM e Especialidade **não** aparecem para perfis que não sejam "Médico" (Enfermeiro, Recepcionista, Admin, etc).

## Visualização na Lista de Usuários

Na lista de usuários, é exibida uma coluna "CRM/Especialidade" que mostra:
- **Para médicos:** O CRM e especialidade cadastrados
- **Para outros perfis:** Um traço (-) indicando que não se aplica

Exemplo:
```
| Nome          | Perfil   | CRM/Especialidade              |
|---------------|----------|--------------------------------|
| Dr. João      | Médico   | CRM: CRM-123456               |
|               |          | Cardiologia                    |
| Maria Silva   | Enfermeiro| -                             |
```

## Casos de Uso

### Caso 1: Clínica Pequena (Configuração Opcional)
Uma clínica pequena com poucos médicos pode preferir deixar os campos opcionais para facilitar o cadastro rápido e atualizar as informações posteriormente.

**Configuração:**
- `professionalIdRequired: false`
- `specialtyRequired: false`

### Caso 2: Clínica Grande ou Hospital (Configuração Obrigatória)
Uma clínica grande ou hospital que precisa garantir que todos os médicos tenham CRM e especialidade devidamente registrados para conformidade legal e auditoria.

**Configuração:**
- `professionalIdRequired: true`
- `specialtyRequired: true`

### Caso 3: Configuração Mista
Uma clínica que exige CRM mas deixa a especialidade opcional (por exemplo, para médicos em treinamento).

**Configuração:**
- `professionalIdRequired: true`
- `specialtyRequired: false`

## Segurança e Permissões

### Quem Pode Configurar

Apenas usuários com as seguintes permissões podem configurar os campos:
- **Proprietário da Clínica** (ClinicOwner)
- **Administrador do Sistema** (SystemAdmin)

Permissão requerida: `clinic.manage`

### Quem Pode Criar Médicos

Usuários com a permissão `users.create` podem criar usuários médicos, mas devem respeitar a configuração de campos obrigatórios definida pelo proprietário.

## Armazenamento

A configuração é armazenada na tabela `ModuleConfigurations` com:
- **ModuleName:** `DoctorFieldsConfig`
- **Configuration:** JSON com `{ professionalIdRequired, specialtyRequired }`
- **IsEnabled:** `true` (sempre habilitado)

## Comportamento Padrão

Se nenhuma configuração foi definida:
- `professionalIdRequired: false` (opcional)
- `specialtyRequired: false` (opcional)

Isso permite que novas clínicas comecem com a configuração mais flexível e ajustem conforme necessário.

## Troubleshooting

### Erro: "Professional ID (CRM) is required for doctors in this clinic"

**Causa:** A clínica está configurada para exigir CRM obrigatório e você tentou criar um médico sem preencher o campo.

**Solução:** Preencha o campo CRM ou peça ao proprietário para alterar a configuração.

### Erro: "Specialty is required for doctors in this clinic"

**Causa:** A clínica está configurada para exigir especialidade obrigatória e você tentou criar um médico sem preencher o campo.

**Solução:** Preencha o campo Especialidade ou peça ao proprietário para alterar a configuração.

### Os campos não aparecem no formulário

**Causa:** O perfil selecionado não é "Médico".

**Solução:** Selecione o perfil "Médico" no dropdown para que os campos apareçam.

## API Reference

### GET /api/ClinicAdmin/doctor-fields-config

Retorna a configuração atual dos campos de médico para a clínica.

**Autenticação:** Bearer Token  
**Permissão:** `clinic.view`

**Response 200 OK:**
```json
{
  "professionalIdRequired": boolean,
  "specialtyRequired": boolean
}
```

### PUT /api/ClinicAdmin/doctor-fields-config

Atualiza a configuração dos campos de médico para a clínica.

**Autenticação:** Bearer Token  
**Permissão:** `clinic.manage`

**Request Body:**
```json
{
  "professionalIdRequired": boolean,
  "specialtyRequired": boolean
}
```

**Response 204 No Content:** Configuração atualizada com sucesso

**Response 401 Unauthorized:** Token inválido ou não fornecido

**Response 403 Forbidden:** Usuário sem permissão para alterar configurações

**Response 500 Internal Server Error:** Erro ao processar a solicitação

## Conclusão

A funcionalidade de configuração de campos para médicos oferece flexibilidade para que cada clínica defina suas próprias regras de cadastro, adaptando-se às necessidades específicas de cada organização.
