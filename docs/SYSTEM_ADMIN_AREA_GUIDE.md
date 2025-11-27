# Área Administrativa do System Owner - Guia de Uso

## Visão Geral

A área administrativa do System Owner permite que proprietários do sistema (como Igor) gerenciem todas as clínicas cadastradas no MedicWarehouse, visualizem métricas globais do sistema e controlem assinaturas.

## Acesso

### Requisitos
- Usuário deve estar autenticado com credenciais de System Owner
- O `tenantId` do usuário deve ser `"system"`
- Após login, o link "⚙️ Administração" aparecerá na barra de navegação

### URL de Acesso
```
https://seu-dominio.com/system-admin
```

## Funcionalidades

### 1. Dashboard Principal (`/system-admin`)

O dashboard apresenta uma visão geral do sistema com as seguintes métricas:

#### Métricas Principais
- **Total de Clínicas**: Número total de clínicas cadastradas
  - Clínicas Ativas
  - Clínicas Inativas
- **Total de Usuários**: Todos os usuários do sistema
  - Usuários Ativos
  - Usuários Inativos
- **Total de Pacientes**: Pacientes cadastrados em todas as clínicas
- **Receita Mensal Recorrente (MRR)**: Soma do valor mensal de todas as assinaturas ativas

#### Ações Rápidas
- **Gerenciar Clínicas**: Acessa a lista completa de clínicas
- **Ver Clínicas Ativas**: Filtra apenas clínicas ativas
- **Ver Clínicas Inativas**: Filtra apenas clínicas inativas

#### Gráficos e Distribuições
- **Assinaturas por Status**: Distribuição de assinaturas por status (Active, Trial, Expired, etc.)
- **Assinaturas por Plano**: Distribuição de clínicas por plano contratado

### 2. Listagem de Clínicas (`/system-admin/clinics`)

Apresenta todas as clínicas cadastradas no sistema com:

#### Informações Exibidas
- Nome da clínica
- CNPJ
- Email de contato
- Telefone
- Plano contratado
- Status da assinatura
- Status da clínica (Ativa/Inativa)
- Data de criação

#### Funcionalidades
- **Filtro por Status**: 
  - Todas
  - Apenas Ativas
  - Apenas Inativas
- **Paginação**: 20 clínicas por página
- **Ações por Clínica**:
  - 👁️ Ver Detalhes
  - ✅/🚫 Ativar/Desativar Clínica

#### Navegação
- Botão "Voltar ao Dashboard" retorna ao dashboard principal

### 3. Detalhes da Clínica (`/system-admin/clinics/{id}`)

Exibe informações completas de uma clínica específica:

#### Informações Gerais
- Nome e nome fantasia
- CNPJ
- Email
- Telefone
- Endereço completo
- Tenant ID
- Data de criação

#### Informações de Assinatura
- Plano contratado
- Valor mensal do plano
- Status atual da assinatura
- Data da próxima cobrança
- Data de término do período de teste (se aplicável)

#### Estatísticas de Usuários
- Total de usuários cadastrados
- Usuários ativos
- Usuários inativos

#### Ações Disponíveis
1. **Ativar/Desativar Clínica**
   - Alterna o status da clínica entre ativa e inativa
   - Confirmação necessária antes da ação

2. **Ativar Override Manual**
   - Permite manter a clínica ativa mesmo com assinatura vencida
   - Útil para casos especiais (amigos, testes, demonstrações)
   - Requer informar o motivo do override

3. **Desativar Override Manual**
   - Remove o override manual
   - Retorna a clínica às regras normais de assinatura

## API Endpoints Utilizados

### Backend (C# .NET)

```csharp
// Listar todas as clínicas (paginado)
GET /api/system-admin/clinics?status={active|inactive}&page={num}&pageSize={num}

// Obter detalhes de uma clínica
GET /api/system-admin/clinics/{id}

// Ativar/Desativar clínica
POST /api/system-admin/clinics/{id}/toggle-status

// Atualizar assinatura
PUT /api/system-admin/clinics/{id}/subscription

// Analytics do sistema
GET /api/system-admin/analytics

// Ativar override manual
POST /api/system-admin/clinics/{id}/subscription/manual-override/enable

// Desativar override manual
POST /api/system-admin/clinics/{id}/subscription/manual-override/disable
```

### Frontend (Angular)

```typescript
// Serviço: SystemAdminService
import { SystemAdminService } from './services/system-admin';

// Obter analytics
systemAdminService.getAnalytics().subscribe(data => {
  console.log(data);
});

// Listar clínicas
systemAdminService.getClinics('active', 1, 20).subscribe(clinics => {
  console.log(clinics);
});

// Alternar status da clínica
systemAdminService.toggleClinicStatus(clinicId).subscribe(response => {
  console.log(response.message);
});
```

## Modelos de Dados

### ClinicSummary
```typescript
interface ClinicSummary {
  id: string;
  name: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
  tenantId: string;
  createdAt: string;
  subscriptionStatus: string;
  planName: string;
  nextBillingDate?: string;
}
```

### ClinicDetail
```typescript
interface ClinicDetail extends ClinicSummary {
  planPrice: number;
  trialEndsAt?: string;
  totalUsers: number;
  activeUsers: number;
}
```

### SystemAnalytics
```typescript
interface SystemAnalytics {
  totalClinics: number;
  activeClinics: number;
  inactiveClinics: number;
  totalUsers: number;
  activeUsers: number;
  totalPatients: number;
  monthlyRecurringRevenue: number;
  subscriptionsByStatus: { [key: string]: number };
  subscriptionsByPlan: { [key: string]: number };
}
```

## Segurança

### Controle de Acesso

1. **Autenticação Obrigatória**
   - Todas as rotas estão protegidas pelo `authGuard`
   - Usuário deve estar logado

2. **Verificação de Tenant**
   - O link da área administrativa só aparece se `tenantId === 'system'`
   - Implementado no componente `Navbar`

3. **Backend Authorization**
   - O backend deve verificar se o usuário tem role `SystemAdmin` ou `SystemOwner`
   - Implementar middleware de autorização nas rotas da API

### Recomendações de Segurança

```csharp
// No backend, adicionar verificação de role
[Authorize(Roles = "SystemAdmin,SystemOwner")]
[HttpGet("clinics")]
public async Task<ActionResult<IEnumerable<ClinicSummaryDto>>> GetAllClinics()
{
    // ...
}
```

## Casos de Uso

### Caso 1: Desativar Clínica com Pagamento Atrasado

1. Acessar `/system-admin/clinics`
2. Filtrar por "Todas" ou pesquisar a clínica
3. Identificar clínica com assinatura "Expired"
4. Clicar no botão 🚫 para desativar
5. Confirmar a ação

### Caso 2: Liberar Acesso para Amigo (Override Manual)

1. Acessar `/system-admin/clinics`
2. Clicar em 👁️ para ver detalhes da clínica do amigo
3. Clicar em "🔓 Ativar Override Manual"
4. Informar o motivo: "Acesso cortesia para Dr. João - amigo pessoal"
5. Confirmar
6. A clínica permanecerá ativa independente do status da assinatura

### Caso 3: Monitorar Crescimento do Sistema

1. Acessar `/system-admin`
2. Verificar métricas:
   - Total de clínicas cresceu 10% no último mês
   - MRR atual: R$ 45.000,00
   - Taxa de churn: 2 clínicas inativas de 50 totais = 4%

### Caso 4: Identificar Clínicas em Período de Teste

1. Acessar `/system-admin`
2. Ver distribuição "Assinaturas por Status"
3. Identificar quantas clínicas estão em "Trial"
4. Clicar em "Gerenciar Clínicas"
5. Filtrar e revisar cada clínica em trial antes do vencimento

## Componentes Técnicos

### Estrutura de Arquivos

```
frontend/medicwarehouse-app/src/app/
├── models/
│   └── system-admin.model.ts          # Interfaces TypeScript
├── services/
│   └── system-admin.ts                # Serviço HTTP
├── pages/
│   └── system-admin/
│       ├── system-admin-dashboard.ts  # Dashboard principal
│       ├── clinic-list.ts             # Lista de clínicas
│       └── clinic-detail.ts           # Detalhes da clínica
└── shared/
    └── navbar/
        ├── navbar.ts                  # Navbar com link admin
        ├── navbar.html
        └── navbar.scss
```

### Rotas Configuradas

```typescript
// app.routes.ts
{ 
  path: 'system-admin', 
  loadComponent: () => import('./pages/system-admin/system-admin-dashboard')
    .then(m => m.SystemAdminDashboard),
  canActivate: [authGuard]
},
{ 
  path: 'system-admin/clinics', 
  loadComponent: () => import('./pages/system-admin/clinic-list')
    .then(m => m.ClinicList),
  canActivate: [authGuard]
},
{ 
  path: 'system-admin/clinics/:id', 
  loadComponent: () => import('./pages/system-admin/clinic-detail')
    .then(m => m.ClinicDetailComponent),
  canActivate: [authGuard]
}
```

## Estilização

### Design System

- **Cores Principais**:
  - Primária: `#667eea` (roxo/azul)
  - Sucesso: `#10b981` (verde)
  - Erro: `#ef4444` (vermelho)
  - Aviso: `#f59e0b` (amarelo)

- **Tipografia**:
  - Headers: `font-weight: 600-700`
  - Body: `font-size: 14-16px`

- **Espaçamento**:
  - Cards: `padding: 24px`
  - Gaps: `16-24px`

- **Efeitos**:
  - Hover em cards: `transform: translateY(-4px)`
  - Box shadows: `0 2px 8px rgba(0, 0, 0, 0.1)`
  - Border radius: `8-12px`

## Performance

### Otimizações Implementadas

1. **Lazy Loading**: Componentes carregados sob demanda
2. **Paginação**: 20 itens por página para reduzir carga inicial
3. **Signals**: Reatividade eficiente do Angular
4. **Standalone Components**: Menor bundle size

### Métricas de Build

```
Initial chunk files   | Raw size | Estimated transfer
chunk-MSDP6UNI.js     | 268.35 kB | 73.17 kB
main-GSVCU57G.js      | 2.30 kB   | 866 bytes

Lazy chunks:
system-admin-dashboard | 8.94 kB  | 2.44 kB
clinic-list           | 9.51 kB  | 2.87 kB
clinic-detail         | 10.83 kB | 3.00 kB
```

## Troubleshooting

### Problema: Link de Administração Não Aparece

**Solução**:
1. Verificar se usuário está autenticado
2. Verificar se `tenantId === 'system'` no localStorage
3. Limpar cache do navegador e fazer login novamente

### Problema: Erro 403 Forbidden ao Acessar APIs

**Solução**:
1. Verificar se token JWT está válido
2. Verificar se usuário tem role `SystemAdmin` ou `SystemOwner`
3. Verificar configuração de autorização no backend

### Problema: Dados Não Carregam

**Solução**:
1. Abrir DevTools e verificar console de erros
2. Verificar se API está acessível (Network tab)
3. Verificar se `environment.apiUrl` está corretamente configurado

## Futuras Melhorias

### Fase 2
- [ ] Exportar relatórios em PDF/Excel
- [ ] Gráficos interativos com bibliotecas como Chart.js
- [ ] Notificações push para events importantes
- [ ] Logs de auditoria de ações administrativas

### Fase 3
- [ ] Dashboard customizável (drag & drop widgets)
- [ ] Filtros avançados e pesquisa global
- [ ] Comparação temporal de métricas
- [ ] Previsões e tendências com IA

## Contato e Suporte

Para dúvidas ou problemas relacionados à área administrativa:
- **Email**: suporte@medicwarehouse.com
- **Documentação Técnica**: [README.md](../README.md)
- **Issue Tracker**: GitHub Issues

---

**Última Atualização**: 14 de Outubro de 2024  
**Versão**: 1.0.0  
**Autor**: GitHub Copilot para MedicWarehouse
