# 📋 Cadastro de Paciente - Guia de Configuração e Testes

## 📌 Visão Geral

Este guia fornece instruções completas para configurar e testar o módulo de Cadastro de Paciente do Omni Care Software, incluindo todos os fluxos e cenários possíveis.

## 🔧 Pré-requisitos

- Sistema iniciado (API + Frontend)
- Usuário com perfil Owner, Medic ou Secretary logado
- Banco de dados configurado com migrations aplicadas

## 📖 Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Cenários de Teste - Cadastro Básico](#cenários-de-teste---cadastro-básico)
3. [Cenários de Teste - Validações](#cenários-de-teste---validações)
4. [Cenários de Teste - Edição](#cenários-de-teste---edição)
5. [Cenários de Teste - Busca e Filtros](#cenários-de-teste---busca-e-filtros)
6. [Cenários de Teste - Integração](#cenários-de-teste---integração)
7. [API Testing](#api-testing)
8. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuração Inicial

### 1. Verificar Permissões de Acesso

```bash
# Verificar roles configuradas no sistema
curl -X GET "http://localhost:5000/api/auth/roles" \
  -H "Authorization: Bearer {seu_token}"
```

**Perfis com acesso ao Cadastro de Paciente:**
- ✅ Owner (acesso total)
- ✅ Medic (acesso total)
- ✅ Secretary (acesso total)
- ✅ Nurse (apenas leitura)
- ❌ SystemAdmin (sem acesso a dados de pacientes)

### 2. Acessar o Módulo

1. Faça login no sistema
2. No menu lateral, clique em **"Pacientes"**
3. Você será direcionado para `/patients`

### 3. Verificar Configurações Regionais

O sistema suporta formatação brasileira:
- **CPF**: XXX.XXX.XXX-XX
- **CEP**: XXXXX-XXX
- **Telefone**: (XX) XXXXX-XXXX
- **Data**: DD/MM/YYYY

---

## 🧪 Cenários de Teste - Cadastro Básico

### Cenário 1.1: Cadastro Completo de Paciente

**Objetivo:** Validar cadastro de paciente com todos os campos preenchidos

**Passos:**
1. Clique no botão **"+ Novo Paciente"**
2. Preencha os dados pessoais:
   - **Nome Completo:** Maria Silva Santos
   - **CPF:** 123.456.789-00
   - **RG:** 12.345.678-9
   - **Data de Nascimento:** 15/05/1985
   - **Sexo:** Feminino
   - **Estado Civil:** Casada
   - **Profissão:** Enfermeira

3. Preencha os dados de contato:
   - **Email:** maria.silva@email.com
   - **Telefone:** (11) 98765-4321
   - **Celular:** (11) 91234-5678

4. Preencha o endereço:
   - **CEP:** 01310-100 (deve preencher automaticamente)
   - **Logradouro:** Av. Paulista
   - **Número:** 1000
   - **Complemento:** Apto 101
   - **Bairro:** Bela Vista
   - **Cidade:** São Paulo
   - **Estado:** SP

5. Preencha informações médicas:
   - **Tipo Sanguíneo:** A+
   - **Alergias:** Dipirona, Penicilina
   - **Condições Pré-existentes:** Hipertensão
   - **Medicamentos em Uso:** Losartana 50mg
   - **Observações:** Paciente gestante - 12 semanas

6. Clique em **"Salvar"**

**Resultado Esperado:**
- ✅ Mensagem de sucesso exibida
- ✅ Paciente aparece na listagem
- ✅ Dados salvos corretamente no banco
- ✅ ID único gerado para o paciente

**Validação via API:**
```bash
curl -X GET "http://localhost:5000/api/patients" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

### Cenário 1.2: Cadastro Mínimo (Apenas Campos Obrigatórios)

**Objetivo:** Validar cadastro com campos mínimos necessários

**Passos:**
1. Clique no botão **"+ Novo Paciente"**
2. Preencha apenas os campos obrigatórios:
   - **Nome Completo:** João Santos
   - **CPF:** 987.654.321-00
   - **Data de Nascimento:** 20/03/1990
   - **Sexo:** Masculino
   - **Telefone:** (11) 98765-1234

3. Clique em **"Salvar"**

**Resultado Esperado:**
- ✅ Cadastro aceito mesmo sem todos os campos
- ✅ Campos opcionais ficam vazios no banco
- ✅ Possível editar depois para completar

---

### Cenário 1.3: Cadastro com Convênio

**Objetivo:** Cadastrar paciente vinculado a um convênio médico

**Pré-requisito:** Pelo menos um convênio cadastrado no sistema

**Passos:**
1. Clique no botão **"+ Novo Paciente"**
2. Preencha os dados básicos do paciente
3. Na seção **"Convênio Médico"**:
   - **Possui Convênio?:** Sim
   - **Convênio:** Unimed
   - **Número da Carteirinha:** 123456789012345
   - **Validade:** 31/12/2026
   - **Plano:** Enfermaria
   - **Tipo de Acomodação:** Apartamento

4. Clique em **"Salvar"**

**Resultado Esperado:**
- ✅ Paciente salvo com vínculo ao convênio
- ✅ Carteirinha registrada
- ✅ Convênio aparece no perfil do paciente

---

## 🧪 Cenários de Teste - Validações

### Cenário 2.1: Validação de CPF

**Objetivo:** Verificar validação de CPF inválido e duplicado

**Teste A - CPF Inválido:**
1. Tente cadastrar com CPF: 111.111.111-11
2. **Resultado Esperado:** ❌ Erro "CPF inválido"

**Teste B - CPF Duplicado:**
1. Cadastre um paciente com CPF: 123.456.789-00
2. Tente cadastrar outro paciente com mesmo CPF
3. **Resultado Esperado:** ❌ Erro "CPF já cadastrado"

**Teste C - CPF Válido:**
1. Use um gerador de CPF válido
2. Cadastre o paciente
3. **Resultado Esperado:** ✅ Cadastro aceito

---

### Cenário 2.2: Validação de Email

**Objetivo:** Verificar validação de formato de email

**Teste A - Email Inválido:**
1. Tente cadastrar com email: "maria.email.com"
2. **Resultado Esperado:** ❌ Erro "Email inválido"

**Teste B - Email Válido:**
1. Use email: "maria@email.com"
2. **Resultado Esperado:** ✅ Aceito

---

### Cenário 2.3: Validação de Data de Nascimento

**Objetivo:** Verificar validações de idade

**Teste A - Data Futura:**
1. Tente cadastrar com data: 01/01/2030
2. **Resultado Esperado:** ❌ Erro "Data de nascimento não pode ser futura"

**Teste B - Idade Superior a 150 anos:**
1. Tente cadastrar com data: 01/01/1800
2. **Resultado Esperado:** ❌ Erro "Data de nascimento inválida"

**Teste C - Menor de Idade:**
1. Cadastre com data: 01/01/2020 (criança)
2. **Resultado Esperado:** ✅ Aceito, campo "Responsável" deve ser preenchido

---

### Cenário 2.4: Validação de CEP

**Objetivo:** Verificar busca automática de endereço

**Teste A - CEP Válido:**
1. Digite CEP: 01310-100
2. **Resultado Esperado:** 
   - ✅ Campos preenchidos automaticamente
   - Logradouro: Av. Paulista
   - Bairro: Bela Vista
   - Cidade: São Paulo
   - Estado: SP

**Teste B - CEP Inválido:**
1. Digite CEP: 99999-999
2. **Resultado Esperado:** 
   - ❌ Erro "CEP não encontrado"
   - Campos ficam habilitados para preenchimento manual

---

## 🧪 Cenários de Teste - Edição

### Cenário 3.1: Editar Dados Pessoais

**Objetivo:** Validar edição de informações do paciente

**Passos:**
1. Na listagem de pacientes, clique no ícone de **"Editar"** (✏️)
2. Altere o **Telefone** para (11) 91111-1111
3. Altere o **Email** para novo.email@email.com
4. Clique em **"Salvar"**

**Resultado Esperado:**
- ✅ Alterações salvas com sucesso
- ✅ Histórico de alterações registrado
- ✅ Dados atualizados na listagem

---

### Cenário 3.2: Adicionar Informações Médicas Posteriormente

**Objetivo:** Completar dados médicos de paciente já cadastrado

**Passos:**
1. Edite um paciente que não tinha informações médicas
2. Adicione:
   - **Tipo Sanguíneo:** O+
   - **Alergias:** Lactose
   - **Condições:** Diabetes Tipo 2
3. Salve as alterações

**Resultado Esperado:**
- ✅ Informações adicionadas com sucesso
- ✅ Aparecem no histórico médico do paciente

---

### Cenário 3.3: Desativar Paciente

**Objetivo:** Inativar um paciente (soft delete)

**Passos:**
1. Na listagem, clique no menu de ações (⋮)
2. Selecione **"Desativar Paciente"**
3. Confirme a ação

**Resultado Esperado:**
- ✅ Paciente marcado como inativo
- ✅ Não aparece na listagem padrão
- ✅ Pode ser reativado posteriormente
- ✅ Histórico preservado

---

## 🧪 Cenários de Teste - Busca e Filtros

### Cenário 4.1: Busca por Nome

**Objetivo:** Encontrar paciente pelo nome

**Passos:**
1. No campo de busca, digite: "Maria"
2. Pressione Enter

**Resultado Esperado:**
- ✅ Lista filtrada mostrando todos os pacientes com "Maria" no nome
- ✅ Busca case-insensitive
- ✅ Busca em nome completo

---

### Cenário 4.2: Busca por CPF

**Objetivo:** Encontrar paciente pelo CPF

**Passos:**
1. No campo de busca, digite: "123.456.789-00"
2. Pressione Enter

**Resultado Esperado:**
- ✅ Paciente específico exibido
- ✅ Busca aceita com ou sem formatação

---

### Cenário 4.3: Filtro por Sexo

**Objetivo:** Filtrar pacientes por sexo

**Passos:**
1. Clique no filtro **"Sexo"**
2. Selecione **"Feminino"**

**Resultado Esperado:**
- ✅ Apenas pacientes do sexo feminino exibidos

---

### Cenário 4.4: Filtro por Convênio

**Objetivo:** Filtrar pacientes por convênio

**Passos:**
1. Clique no filtro **"Convênio"**
2. Selecione **"Unimed"**

**Resultado Esperado:**
- ✅ Apenas pacientes com Unimed exibidos

---

### Cenário 4.5: Filtro por Faixa Etária

**Objetivo:** Filtrar por idade

**Passos:**
1. Clique no filtro **"Idade"**
2. Configure: Mínima 18, Máxima 65

**Resultado Esperado:**
- ✅ Apenas adultos na faixa especificada

---

## 🧪 Cenários de Teste - Integração

### Cenário 5.1: Vincular Paciente a Agendamento

**Objetivo:** Criar agendamento para paciente cadastrado

**Passos:**
1. Cadastre um paciente
2. Vá para o módulo **"Agendamentos"**
3. Crie novo agendamento
4. Busque o paciente cadastrado
5. Complete o agendamento

**Resultado Esperado:**
- ✅ Paciente aparece na busca
- ✅ Dados preenchidos automaticamente
- ✅ Agendamento criado com sucesso

---

### Cenário 5.2: Acessar Histórico de Consultas

**Objetivo:** Visualizar consultas anteriores do paciente

**Passos:**
1. Abra o perfil de um paciente
2. Clique na aba **"Histórico de Consultas"**

**Resultado Esperado:**
- ✅ Lista de consultas anteriores
- ✅ Datas, médicos e diagnósticos
- ✅ Possibilidade de visualizar detalhes

---

### Cenário 5.3: Vincular Paciente a Convênio Existente

**Objetivo:** Associar paciente particular a um convênio

**Pré-requisito:** Paciente cadastrado sem convênio

**Passos:**
1. Edite o paciente
2. Na seção **"Convênio"**, altere para **"Sim"**
3. Preencha dados do convênio
4. Salve

**Resultado Esperado:**
- ✅ Convênio associado ao paciente
- ✅ Futuras consultas podem usar o convênio

---

## 🔌 API Testing

### Endpoint: Criar Paciente

```bash
curl -X POST "http://localhost:5000/api/patients" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "fullName": "Maria Silva Santos",
    "cpf": "12345678900",
    "rg": "123456789",
    "birthDate": "1985-05-15",
    "gender": "Female",
    "maritalStatus": "Married",
    "occupation": "Enfermeira",
    "email": "maria.silva@email.com",
    "phone": "11987654321",
    "cellPhone": "11912345678",
    "address": {
      "zipCode": "01310100",
      "street": "Av. Paulista",
      "number": "1000",
      "complement": "Apto 101",
      "neighborhood": "Bela Vista",
      "city": "São Paulo",
      "state": "SP"
    },
    "medicalInfo": {
      "bloodType": "APositive",
      "allergies": "Dipirona, Penicilina",
      "preExistingConditions": "Hipertensão",
      "currentMedications": "Losartana 50mg",
      "observations": "Paciente gestante - 12 semanas"
    }
  }'
```

**Resposta Esperada (201 Created):**
```json
{
  "id": "uuid-gerado",
  "fullName": "Maria Silva Santos",
  "cpf": "12345678900",
  "birthDate": "1985-05-15",
  "createdAt": "2026-01-22T00:00:00Z"
}
```

---

### Endpoint: Listar Pacientes

```bash
curl -X GET "http://localhost:5000/api/patients?page=1&pageSize=10" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

**Resposta Esperada (200 OK):**
```json
{
  "items": [...],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

---

### Endpoint: Buscar Paciente por ID

```bash
curl -X GET "http://localhost:5000/api/patients/{patient_id}" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

### Endpoint: Atualizar Paciente

```bash
curl -X PUT "http://localhost:5000/api/patients/{patient_id}" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}" \
  -d '{
    "phone": "11911111111",
    "email": "novo.email@email.com"
  }'
```

---

### Endpoint: Desativar Paciente

```bash
curl -X DELETE "http://localhost:5000/api/patients/{patient_id}" \
  -H "Authorization: Bearer {seu_token}" \
  -H "TenantId: {seu_tenant_id}"
```

---

## 🐛 Troubleshooting

### Problema 1: CEP não preenche automaticamente

**Causa:** API ViaCEP pode estar indisponível

**Solução:**
1. Verifique conexão com internet
2. Preencha manualmente os campos
3. Teste com outro CEP

---

### Problema 2: CPF aceito mas inválido

**Causa:** Validação de dígito verificador

**Solução:**
1. Use gerador de CPF válido
2. Verifique algoritmo de validação no frontend

---

### Problema 3: Paciente não aparece na listagem

**Causa:** Filtro de TenantId

**Solução:**
1. Verifique se TenantId está correto no header
2. Confirme que paciente pertence à mesma clínica
3. Verifique se paciente não está inativo

---

### Problema 4: Erro 403 ao tentar cadastrar

**Causa:** Permissões insuficientes

**Solução:**
1. Verifique seu perfil de usuário
2. Apenas Owner, Medic e Secretary podem cadastrar
3. Reautentique se necessário

---

### Problema 5: Foto do paciente não carrega

**Causa:** Limite de tamanho ou formato inválido

**Solução:**
1. Use imagens até 5MB
2. Formatos aceitos: JPG, PNG, WEBP
3. Redimensione se necessário

---

## ✅ Checklist de Validação Final

Use este checklist para validar que todos os cenários foram testados:

- [ ] Cadastro completo de paciente
- [ ] Cadastro mínimo (campos obrigatórios)
- [ ] Cadastro com convênio
- [ ] Validação de CPF (válido, inválido, duplicado)
- [ ] Validação de email
- [ ] Validação de data de nascimento
- [ ] Validação de CEP e preenchimento automático
- [ ] Edição de dados pessoais
- [ ] Adição de informações médicas
- [ ] Desativação de paciente
- [ ] Busca por nome
- [ ] Busca por CPF
- [ ] Filtro por sexo
- [ ] Filtro por convênio
- [ ] Filtro por faixa etária
- [ ] Integração com agendamentos
- [ ] Visualização de histórico
- [ ] Testes de API (CRUD completo)

---

## 📚 Documentação Relacionada

- [Guia de Testes Completo](../GUIA_TESTES_PASSO_A_PASSO.md)
- [Checklist de Testes](../CHECKLIST_TESTES_COMPLETO.md)
- [Fluxo Completo do Sistema](../FLUXO_COMPLETO_SISTEMA.md)
- [Ordem Correta de Cadastro](../ORDEM_CORRETA_CADASTRO.md)
