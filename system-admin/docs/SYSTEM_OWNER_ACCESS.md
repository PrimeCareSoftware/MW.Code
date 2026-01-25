# Documentação de Acesso e Cadastro Inicial - System Owner

## 📋 Visão Geral

Este documento descreve como configurar, acessar e utilizar o novo sistema de administração MW System Admin, separado do aplicativo principal PrimeCare Software.

## 🏗️ Arquitetura da Separação

O PrimeCare Software agora é composto por **dois aplicativos Angular independentes**:

### 1. **PrimeCare Software App** (`medicwarehouse-app`)
- **Usuários**: Proprietários de clínicas, médicos, secretárias, etc.
- **Funcionalidades**: 
  - Dashboard da clínica
  - Gestão de pacientes
  - Agendamentos
  - Atendimentos
  - Prontuários
- **URL**: `http://localhost:4200` (desenvolvimento)
- **Login**: `/api/auth/login` (com tenantId da clínica)

### 2. **MW System Admin** (`mw-system-admin`) 🆕
- **Usuários**: System Owners (administradores do sistema)
- **Funcionalidades**:
  - Dashboard global com métricas de todas as clínicas
  - Gestão de clínicas (criar, ativar, desativar)
  - Controle de assinaturas
  - Override manual
  - Gestão de system owners
- **URL**: `http://localhost:4201` (desenvolvimento, porta diferente)
- **Login**: `/api/auth/owner-login` (sem tenantId)

## 🚀 Configuração Inicial

### Passo 1: Instalar Dependências

```bash
# MW System Admin
cd frontend/mw-system-admin
npm install

# PrimeCare Software App (se necessário)
cd ../medicwarehouse-app
npm install
```

### Passo 2: Configurar Portas

Para rodar ambos os projetos simultaneamente, configure portas diferentes:

**mw-system-admin/angular.json**:
```json
"serve": {
  "options": {
    "port": 4201
  }
}
```

**medicwarehouse-app/angular.json**:
```json
"serve": {
  "options": {
    "port": 4200
  }
}
```

### Passo 3: Executar os Projetos

```bash
# Terminal 1 - PrimeCare Software App
cd frontend/medicwarehouse-app
npm start
# Disponível em http://localhost:4200

# Terminal 2 - MW System Admin
cd frontend/mw-system-admin
npm start
# Disponível em http://localhost:4201
```

## 👤 Cadastro do Primeiro System Owner

### Opção 1: Via Backend Direto (Recomendado)

Se você ainda não tem nenhum System Owner, pode criar o primeiro diretamente no banco de dados ou via migration/seeder:

```bash
# No projeto backend
cd src/MedicSoft.Api
dotnet run --seed-system-owner
```

Ou criar manualmente no banco de dados:

```sql
INSERT INTO Owners (
    Id,
    Username,
    Email,
    PasswordHash,
    FullName,
    Phone,
    IsActive,
    TenantId,
    ClinicId,
    CreatedAt
) VALUES (
    NEWID(),
    'admin',
    'admin@medicwarehouse.com',
    -- Hash BCrypt da senha "Admin123!"
    '$2a$12$...',
    'Administrador do Sistema',
    '+5511999999999',
    1,
    'system',
    NULL,
    GETUTCDATE()
);
```

### Opção 2: Via API (Se já tiver um System Owner)

Se você já tem acesso a um System Owner, pode criar outros via API:

```bash
POST /api/system-admin/system-owners
Authorization: Bearer <seu-token-jwt>
Content-Type: application/json

{
  "username": "novoowner",
  "email": "owner@medicwarehouse.com",
  "password": "SenhaSegura123!",
  "fullName": "Nome do Owner",
  "phone": "+5511999999999"
}
```

## 🔐 Como Fazer Login

### 1. Acesse o MW System Admin

```
http://localhost:4201
```

### 2. Credenciais

```
Usuário: admin
Senha: [a senha que você configurou]
```

### 3. Fluxo de Autenticação

1. O sistema envia as credenciais para `/api/auth/owner-login`
2. O backend valida e verifica se é um System Owner
3. Retorna um JWT token com `isSystemOwner: true`
4. O frontend valida e permite acesso apenas se `isSystemOwner === true`

## 📊 Funcionalidades Disponíveis

### Dashboard

Após o login, você verá o dashboard com:

- **Métricas Gerais**:
  - Total de clínicas (ativas/inativas)
  - Total de usuários no sistema
  - Total de pacientes cadastrados
  - MRR (Monthly Recurring Revenue)

- **Distribuições**:
  - Assinaturas por status (Active, Trial, Expired, etc.)
  - Assinaturas por plano (Basic, Standard, Premium, etc.)

- **Ações Rápidas**:
  - Gerenciar todas as clínicas
  - Ver clínicas ativas
  - Ver clínicas inativas
  - Gerenciar usuários system owner

### Gestão de Clínicas

#### Listar Clínicas

```
Navegue para: Clínicas → Listar
```

**Funcionalidades**:
- Visualizar todas as clínicas cadastradas
- Filtrar por status (todas/ativas/inativas)
- Paginação (20 clínicas por página)
- Ver detalhes de cada clínica
- Ativar ou desativar clínicas

#### Criar Nova Clínica

```
Navegue para: Clínicas → Nova Clínica
```

**Campos obrigatórios**:
- Nome da clínica
- CNPJ
- Email
- Telefone
- Endereço
- Dados do proprietário (username, senha, nome completo)
- Plano de assinatura

#### Ativar/Desativar Clínica

```
Na lista de clínicas → Botão 🚫 (desativar) ou ✅ (ativar)
```

**Impacto**:
- Clínica inativa: Usuários não conseguem fazer login
- Clínica ativa: Funcionamento normal

#### Override Manual

```
Detalhes da Clínica → Ativar Override Manual
```

**Quando usar**:
- Liberar acesso para amigos/parceiros
- Período de teste especial
- Casos excepcionais

**Como funciona**:
- Clínica permanece ativa independente do status da assinatura
- Requer justificativa (motivo)
- Pode ser removido a qualquer momento

### Gestão de Assinaturas

#### Ver Status da Assinatura

```
Detalhes da Clínica → Seção "Assinatura"
```

**Informações disponíveis**:
- Plano atual
- Valor mensal
- Status (Active, Trial, Expired, etc.)
- Próxima data de cobrança
- Se está em período de teste

#### Alterar Plano

```
Detalhes da Clínica → Atualizar Assinatura
```

**Opções**:
- Mudar para outro plano
- Alterar status manualmente
- Ajustar data de próxima cobrança

## 🛠️ Casos de Uso Comuns

### Caso 1: Nova Clínica Cadastrada

1. Acesse MW System Admin
2. Navegue para "Clínicas"
3. Clique em "Nova Clínica"
4. Preencha os dados da clínica e do proprietário
5. Selecione o plano de assinatura
6. Confirme o cadastro
7. A clínica estará ativa e pronta para uso

### Caso 2: Clínica Inadimplente

1. Verifique no dashboard quais clínicas têm status "PaymentOverdue"
2. Entre em contato com o proprietário da clínica
3. Se não houver pagamento:
   - Opção A: Desativar a clínica temporariamente
   - Opção B: Aguardar suspensão automática
4. Quando o pagamento for confirmado:
   - Reativar a clínica
   - Status volta automaticamente para "Active"

### Caso 3: Liberar Acesso Cortesia

1. Acesse a clínica desejada
2. Clique em "Ativar Override Manual"
3. Informe o motivo (ex: "Cortesia para parceiro estratégico")
4. Confirme
5. A clínica terá acesso liberado independente do status de pagamento

### Caso 4: Adicionar Novo System Owner

1. Navegue para "Usuários" (em desenvolvimento)
2. Clique em "Novo System Owner"
3. Preencha os dados:
   - Username
   - Email
   - Senha
   - Nome completo
   - Telefone
4. Confirme o cadastro
5. O novo owner pode fazer login no MW System Admin

## 🔒 Segurança e Permissões

### Níveis de Acesso

| Tipo de Usuário | Acesso MW App | Acesso System Admin |
|-----------------|---------------|---------------------|
| System Owner    | ❌ Não        | ✅ Sim              |
| Clinic Owner    | ✅ Sim        | ❌ Não              |
| Doctor          | ✅ Sim        | ❌ Não              |
| Secretary       | ✅ Sim        | ❌ Não              |

### Autenticação

- **System Owner**: 
  - Endpoint: `/api/auth/owner-login`
  - Não requer `tenantId`
  - Retorna token com `isSystemOwner: true`
  
- **Outros usuários**:
  - Endpoint: `/api/auth/login`
  - Requer `tenantId` da clínica
  - Retorna token com `isSystemOwner: false`

## 📱 Responsividade

Ambos os sistemas são totalmente responsivos:

- **Desktop**: Layout completo com sidebar
- **Tablet**: Layout adaptado
- **Mobile**: Menu hamburger e cards empilhados

## 🐛 Troubleshooting

### Problema: Não consigo fazer login no System Admin

**Soluções**:
1. Verifique se o usuário tem `ClinicId = NULL` no banco
2. Verifique se o `TenantId = "system"`
3. Confirme que a senha está correta
4. Verifique se o backend está rodando
5. Inspecione o console do navegador para erros

### Problema: Erro "Acesso negado" após login

**Causa**: Usuário não é System Owner

**Solução**: Verificar no banco se `IsSystemOwner = true` ou `ClinicId IS NULL`

### Problema: API retorna 401 Unauthorized

**Causas possíveis**:
1. Token expirado
2. Token inválido
3. Usuário não é System Owner

**Solução**: Fazer logout e login novamente

### Problema: Não vejo a clínica na lista

**Verificações**:
1. Clínica existe no banco de dados?
2. Filtro está correto (todas/ativas/inativas)?
3. Verifique a paginação

## 📞 Suporte

Para problemas ou dúvidas:

- **Email**: suporte@primecaresoftware.com
- **GitHub Issues**: https://github.com/PrimeCare Software/MW.Code/issues
- **Documentação**: Veja os arquivos `.md` no repositório

## 🔄 Atualizações Futuras

### Funcionalidades Planejadas

- [ ] Criar nova clínica via interface
- [ ] Editar dados de clínicas existentes
- [ ] Gestão completa de System Owners
- [ ] Área financeira com relatórios detalhados
- [ ] Gráficos de MRR histórico
- [ ] Exportação de relatórios (PDF, Excel)
- [ ] Notificações automáticas
- [ ] Logs de auditoria

## 📚 Documentos Relacionados

- [README Principal](../README.md)
- [README MW System Admin](frontend/mw-system-admin/README.md)
- [Arquitetura do Sistema](BEFORE_AND_AFTER_ARCHITECTURE.md)
- [Guia de Autenticação](AUTHENTICATION_GUIDE.md)
- [Implementação System Owner](RESUMO_IMPLEMENTACAO_SYSTEM_OWNER.md)

---

**Última atualização**: Outubro 2024
**Versão**: 1.0.0
