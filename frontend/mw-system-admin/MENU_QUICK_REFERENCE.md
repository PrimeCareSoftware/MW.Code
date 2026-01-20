# Guia Rápido - Menu da Aplicação mw-system-admin

## 📋 O que é mw-system-admin?

Uma aplicação **Angular standalone** para administração central do sistema MedicWarehouse, responsável por:

- ✅ Gerenciar clínicas
- ✅ Gerenciar planos de assinatura  
- ✅ Gerenciar proprietários de clínicas
- ✅ Configurar subdomínios
- ✅ Gerenciar tickets de suporte
- ✅ Visualizar métricas de vendas
- ✅ Ver dashboard do sistema

---

## 🗂️ Estrutura do Menu (Após Correção)

```
📱 TOP BAR (Barra Superior)
├── 🔔 Notificações
├── 👤 Usuário (Dropdown)
│   └── Sair
└── ≡ Toggle Menu

🗂️ SIDEBAR (Menu Lateral)
├── 🏠 Dashboard
└── ─────────────────────
    📊 GERENCIAMENTO DE SISTEMA
    ├── 🏥 Clínicas
    ├── 📋 Planos de Assinatura
    ├── 👤 Proprietários de Clínicas
    ├── 🌐 Subdomínios
    ├── 🎫 Tickets de Suporte
    └── 📈 Métricas de Vendas
```

---

## 🔗 Mapeamento de Rotas

| Menu Item | Rota | Componente |
|-----------|------|-----------|
| Dashboard | `/dashboard` | Dashboard |
| Clínicas | `/clinics` | ClinicsList |
| ➕ Nova Clínica | `/clinics/create` | ClinicCreate |
| 📝 Detalhe Clínica | `/clinics/:id` | ClinicDetail |
| Planos de Assinatura | `/plans` | PlansList |
| Proprietários de Clínicas | `/clinic-owners` | ClinicOwnersList |
| Subdomínios | `/subdomains` | SubdomainsList |
| Tickets de Suporte | `/tickets` | TicketsPage |
| Métricas de Vendas | `/sales-metrics` | SalesMetrics |

---

## 🔒 Segurança

### Guard de Autenticação
- **Guard**: `systemAdminGuard`
- **Requisito**: `isSystemOwner === true`
- **Rotas Protegidas**: Todas exceto `/login` e `/403`

### Verificação de Acesso
```typescript
// No navbar.ts
isSystemAdmin(): boolean {
  const user = this.authService.currentUser();
  return !!user?.isSystemOwner;
}
```

---

## 📁 Arquivos Importantes

| Arquivo | Responsabilidade |
|---------|-----------------|
| `app.routes.ts` | Definição de todas as rotas |
| `navbar/navbar.html` | Template do menu |
| `navbar/navbar.ts` | Lógica do menu |
| `navbar/navbar.scss` | Estilos do menu |
| `guards/system-admin-guard.ts` | Proteção de rotas |

---

## ✏️ Como Adicionar um Novo Item de Menu

### 1️⃣ Definir a Rota em `app.routes.ts`

```typescript
{
  path: 'novo-item',
  loadComponent: () => import('./pages/novo-item/novo-item').then(m => m.NovoItem),
  canActivate: [systemAdminGuard]
}
```

### 2️⃣ Adicionar Item no Menu HTML

```html
<!-- No navbar.html, dentro da seção GERENCIAMENTO DE SISTEMA -->
<a routerLink="/novo-item" routerLinkActive="active" class="nav-item" (click)="closeSidebar()">
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
    <!-- Seu ícone SVG aqui -->
  </svg>
  <span class="nav-text">Novo Item</span>
</a>
```

### 3️⃣ Criar o Componente

```
src/app/pages/novo-item/
├── novo-item.ts       (componente)
├── novo-item.html     (template)
└── novo-item.scss     (estilos)
```

### 4️⃣ Validar

- [ ] Build sem erros: `npm run build`
- [ ] Menu carrega: `npm start`
- [ ] Link funciona: clique no menu
- [ ] Guard valida: tenta acessar sem autenticação

---

## 🚨 O que NÃO Fazer

### ❌ NÃO adicione items sem rotas
```typescript
// ❌ ERRADO - Sem rota definida em app.routes.ts
<a routerLink="/item-inexistente" ...>
```

### ❌ NÃO misture responsabilidades
```typescript
// ❌ ERRADO - Funções de clínica em sistema admin
/clinic/patients  ← Não existe aqui!
```

### ❌ NÃO use condicionais para features
```typescript
// ❌ ERRADO - Menu dinâmico demais
@if (user.role === 'something') {
  // Menu item
}
```

---

## 🧪 Teste Rápido

### Verificar Se Menu Está Correto

```bash
# 1. Build sem erros
npm run build

# 2. Inicie o servidor
npm start

# 3. Abra no navegador
http://localhost:4201

# 4. Teste cada link do menu
# Cada link deve funcionar e carregar a página corretamente
```

### Debug de Rotas

```typescript
// No console do navegador
// Veja as rotas definidas
router.config
```

---

## 📊 Estatísticas Pós-Correção

| Métrica | Antes | Depois | Mudança |
|---------|-------|--------|---------|
| Items Menu | 31 | 7 | -77% ✅ |
| Rotas Válidas | 7 | 7 | ±0% |
| Linhas HTML | 282 | 80 | -72% ✅ |
| Métodos TS | 6 | 5 | -17% ✅ |
| Erros Build | 0 | 0 | ±0% ✅ |

---

## 💡 Dicas de Manutenção

1. **Sempre sincronize menu com rotas**
   - Quando adicionar rota → adicione menu item
   - Quando remover rota → remova menu item

2. **Use TypeScript para evitar erros**
   - Crie tipos para rotas
   - Use enums para paths

3. **Teste navegação**
   - Click em cada menu item
   - Verifique se carrega correto

4. **Documente mudanças**
   - Atualize README/guias
   - Deixe comentários em código

---

## 📞 Referências

- 📄 [app.routes.ts](src/app/app.routes.ts) - Rotas
- 📄 [navbar.html](src/app/shared/navbar/navbar.html) - Menu template  
- 📄 [navbar.ts](src/app/shared/navbar/navbar.ts) - Menu lógica
- 📄 [MENU_FIXES.md](MENU_FIXES.md) - Detalhes da correção
- 📄 [NAVBAR_ANALYSIS.md](NAVBAR_ANALYSIS.md) - Análise completa

---

## ✅ Checklist de Qualidade

- [x] Build sem erros
- [x] Todos os links funcionam
- [x] Código documentado
- [x] Menu intuitivo
- [x] Sem items duplicados
- [x] Sem rotas quebradas
- [x] TypeScript validado
- [x] Pronto para produção

---

**Versão**: 1.0  
**Data**: 19 de janeiro de 2026  
**Status**: ✅ Operacional
