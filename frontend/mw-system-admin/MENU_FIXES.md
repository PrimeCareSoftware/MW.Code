# Correções do Menu da Aplicação - mw-system-admin

Data: 19 de janeiro de 2026

## Problema Identificado

O arquivo `navbar.html` continha **itens de menu que não existiam nas rotas da aplicação** (`app.routes.ts`), causando:
1. Exibição de itens que não deveriam estar visíveis
2. Cliques em itens de menu que não funcionavam (rotas não definidas)
3. Confusão entre responsabilidades (menu para diferentes aplicações mesclado)

## Contexto

Esta é uma aplicação de **Administração de Sistema (System Admin)** separada, responsável apenas por:
- Gerenciar clínicas
- Gerenciar planos de assinatura
- Gerenciar proprietários de clínicas
- Gerenciar subdomínios
- Gerenciar tickets de suporte
- Visualizar métricas de vendas
- Dashboard do sistema

## Itens Removidos do Menu

Os seguintes itens foram removidos porque **não possuem rotas definidas em `app.routes.ts`**:

### Seção Clínica (não deveria estar aqui)
- ❌ Pacientes (`/patients`)
- ❌ Agendamentos (`/appointments`)
- ❌ Fila de Espera (`/waiting-queue`)
- ❌ Relatórios (`/analytics`)

### Seção Financeiro (não deveria estar aqui)
- ❌ Fluxo de Caixa (`/financial/cash-flow`)
- ❌ Contas a Receber (`/financial/receivables`)
- ❌ Contas a Pagar (`/financial/payables`)
- ❌ Fornecedores (`/financial/suppliers`)
- ❌ Fechamentos (`/financial/closures`)

### Seção Compliance (não deveria estar aqui)
- ❌ SNGPC - ANVISA (`/sngpc/dashboard`)

### Seção Administração - Para Proprietários de Clínica (não deveria estar aqui)
- ❌ Dashboard do Sistema com prefixo `/system-admin/` (redundante)
- ❌ Gerenciar Clínicas com prefixo `/system-admin/` (já existe em cima sem prefixo)
- ❌ Planos de Assinatura com prefixo `/system-admin/` (já existe em cima sem prefixo)
- ❌ Proprietários de Clínicas com prefixo `/system-admin/` (já existe em cima sem prefixo)
- ❌ Subdomínios com prefixo `/system-admin/` (já existe em cima sem prefixo)
- ❌ Tickets do Sistema com prefixo `/system-admin/` (já existe em cima sem prefixo)
- ❌ Métricas de Vendas com prefixo `/system-admin/` (já existe em cima sem prefixo)

### Seção Proprietários de Clínica (não deveria estar aqui)
- ❌ Usuários (`/clinic-admin/users`)
- ❌ Perfis de Acesso (`/admin/profiles`)
- ❌ Informações da Clínica (`/clinic-admin/info`)
- ❌ Personalização (`/clinic-admin/customization`)
- ❌ Assinatura (`/clinic-admin/subscription`)

## Itens Mantidos no Menu

Os seguintes itens foram mantidos porque **possuem rotas definidas em `app.routes.ts`**:

### Dashboard
- ✅ Dashboard (`/dashboard`)

### Gerenciamento de Sistema
- ✅ Clínicas (`/clinics`)
- ✅ Planos de Assinatura (`/plans`)
- ✅ Proprietários de Clínicas (`/clinic-owners`)
- ✅ Subdomínios (`/subdomains`)
- ✅ Tickets de Suporte (`/tickets`)
- ✅ Métricas de Vendas (`/sales-metrics`)

## Mudanças Realizadas

### 1. Arquivo: `src/app/shared/navbar/navbar.html`
- Removidos todos os itens de menu que não possuem rotas
- Simplificada a estrutura do menu
- Removida a condicional `@if (isSystemAdmin())` redundante
- Removida completamente a seção `@if (isOwner())` que era para perfis diferentes
- Mantida apenas uma seção: "Gerenciamento de Sistema"

### 2. Arquivo: `src/app/shared/navbar/navbar.ts`
- Removida a propriedade `adminDropdownOpen` (não era usada)
- Removido o método `toggleAdminDropdown()` (não era chamado)
- Removido o método `isOwner()` (não é necessário nesta aplicação)
- Simplificada a lógica do `onDocumentClick()` para apenas lidar com `user-dropdown`
- Removida a chamada `this.adminDropdownOpen = false` do método `logout()`

## Resultado

✅ **Menu limpo e funcional**
- Todos os itens visíveis no menu agora possuem rotas definidas
- Cliques em itens de menu funcionam corretamente
- Menu reflete apenas a responsabilidade desta aplicação (System Admin)
- Interface mais clara e menos confusa para o usuário

## Rotas Disponíveis (Conforme `app.routes.ts`)

```
/login                    - Login (sem autenticação)
/403                      - Página de erro 403
/                         - Redireciona para /dashboard
/dashboard                - Dashboard do sistema (requer autenticação)
/clinics                  - Lista de clínicas (requer autenticação)
/clinics/create           - Criar nova clínica (requer autenticação)
/clinics/:id              - Detalhe da clínica (requer autenticação)
/plans                    - Planos de assinatura (requer autenticação)
/clinic-owners            - Proprietários de clínicas (requer autenticação)
/subdomains               - Subdomínios (requer autenticação)
/tickets                  - Tickets de suporte (requer autenticação)
/sales-metrics            - Métricas de vendas (requer autenticação)
/**                       - Qualquer rota desconhecida redireciona para /dashboard
```

## Notas Importantes

1. **Separação de Responsabilidades**: Se no futuro for necessário adicionar funcionalidades de clínica, financeiro ou outros, elas devem ser implementadas em **aplicações separadas**, não nesta aplicação System Admin.

2. **Autenticação**: Todas as rotas (exceto `/login` e `/403`) são protegidas por `systemAdminGuard`, que verifica se o usuário possui a flag `isSystemOwner`.

3. **Possível Refatoração Futura**: O código atual que removia `isOwner()` sugere que no passado houve planos de misturar funcionalidades de System Admin com funcionalidades de Clinic Owner no mesmo navbar. Isso foi eliminado nesta correção.

## Build Status

✅ Build successful - Nenhum erro de compilação após as mudanças
