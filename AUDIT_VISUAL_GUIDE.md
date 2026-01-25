# Guia Visual do Sistema de Logs de Auditoria

## Acesso ao Sistema

### Menu de Navegação

No menu lateral do system-admin, uma nova seção foi adicionada:

```
📊 Dashboard
━━━━━━━━━━━━━━━━━━━━━━━━━
Gerenciamento de Sistema
━━━━━━━━━━━━━━━━━━━━━━━━━
🏥 Clínicas
📋 Planos de Assinatura
👥 Proprietários de Clínicas
🌐 Subdomínios
📝 Tickets de Suporte
📈 Métricas de Vendas
━━━━━━━━━━━━━━━━━━━━━━━━━
Catálogos e Dados
━━━━━━━━━━━━━━━━━━━━━━━━━
💊 Medicações
📋 Catálogo de Exames
━━━━━━━━━━━━━━━━━━━━━━━━━
Monitoramento e Segurança    ← NOVA SEÇÃO
━━━━━━━━━━━━━━━━━━━━━━━━━
📄 Logs de Auditoria          ← NOVA PÁGINA
```

## Estrutura da Página

### 1. Cabeçalho

```
┌────────────────────────────────────────────────────────────────┐
│ 📄 Logs de Auditoria                    [Exportar CSV] [JSON] │
│ Visualize e acompanhe todas as atividades do sistema          │
└────────────────────────────────────────────────────────────────┘
```

### 2. Filtros de Pesquisa (Expansível/Recolhível)

```
┌────────────────────────────────────────────────────────────────┐
│ 🔍 Filtros de Pesquisa                                      [▼] │
├────────────────────────────────────────────────────────────────┤
│ [Data Inicial] [Data Final] [ID do Usuário]                   │
│ [Tipo Entidade] [ID Entidade] [Ação ▼]                        │
│ [Resultado ▼] [Severidade ▼]                                  │
│                                    [Limpar] [Aplicar Filtros]  │
└────────────────────────────────────────────────────────────────┘
```

### 3. Resumo de Resultados

```
┌────────────────────────────────────────────────────────────────┐
│ Total de registros: 1,234        Página 1 de 25               │
└────────────────────────────────────────────────────────────────┘
```

### 4. Tabela de Logs

```
┌──────────────┬─────────────┬────────┬──────────┬──────────┬──────────┬──────────┬────────┐
│ Data/Hora    │ Usuário     │ Ação   │ Entidade │ Resultado│Severidade│    IP    │ Ações  │
├──────────────┼─────────────┼────────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ 25/01/2026   │ João Silva  │ 👁️ READ│ Patient  │ SUCCESS  │ INFO     │ 10.0.0.1 │  [👁️]  │
│ 04:45:23     │ joao@...    │        │ John Doe │  (verde) │ (azul)   │          │        │
├──────────────┼─────────────┼────────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ 25/01/2026   │ Maria Silva │ ✏️ UPD │ User     │ SUCCESS  │ INFO     │ 10.0.0.2 │  [👁️]  │
│ 04:44:12     │ maria@...   │  ATE   │ ID: 123  │  (verde) │ (azul)   │          │        │
├──────────────┼─────────────┼────────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ 25/01/2026   │ Admin User  │ ❌     │ Login    │  FAILED  │  ERROR   │ 10.0.0.3 │  [👁️]  │
│ 04:43:01     │ admin@...   │  LOGIN │ Attempt  │(vermelho)│(vermelho)│          │        │
└──────────────┴─────────────┴────────┴──────────┴──────────┴──────────┴──────────┴────────┘
```

### 5. Paginação

```
┌────────────────────────────────────────────────────────────────┐
│ [◀ Anterior]  [1] 2 3 ... 24 [25]  [Próxima ▶]               │
└────────────────────────────────────────────────────────────────┘
```

## Modal de Detalhes

Ao clicar no ícone 👁️, abre-se um modal com informações completas:

```
┌──────────────────────────────────────────────────────────────────┐
│ Detalhes do Log de Auditoria                              [✕]   │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│ ━━ Informações Gerais ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ Data/Hora:     25/01/2026 04:45:23                             │
│ Ação:          READ                                             │
│ Descrição:     Visualização de dados do paciente               │
│ Resultado:     SUCCESS (verde)                                  │
│ Severidade:    INFO (azul)                                      │
│                                                                  │
│ ━━ Usuário ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ Nome:          João Silva                                       │
│ Email:         joao.silva@exemplo.com                          │
│                                                                  │
│ ━━ Entidade Afetada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ Tipo:          Patient                                          │
│ ID:            abc-123-def-456                                 │
│ Nome:          John Doe                                        │
│                                                                  │
│ ━━ Requisição ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ IP:            10.0.0.1                                        │
│ Método HTTP:   GET                                             │
│ Caminho:       /api/patients/abc-123-def-456                  │
│ Status Code:   200                                             │
│                                                                  │
│ ━━ LGPD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ Categoria:     SENSITIVE                                        │
│ Finalidade:    HEALTHCARE                                       │
│                                                                  │
│ ━━ User Agent ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│ Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0    │
│                                                                  │
│                                           [Fechar]              │
└──────────────────────────────────────────────────────────────────┘
```

## Indicadores Visuais

### Badges de Resultado

```
✓ SUCCESS       → Verde claro com texto verde escuro
✕ FAILED        → Vermelho claro com texto vermelho escuro
⊘ UNAUTHORIZED  → Amarelo claro com texto amarelo escuro
◐ PARTIAL_SUCCESS → Azul claro com texto azul escuro
```

### Badges de Severidade

```
ℹ INFO      → Azul claro com texto azul escuro
⚠ WARNING   → Amarelo claro com texto amarelo escuro
⚠ ERROR     → Vermelho claro com texto vermelho escuro
⚠ CRITICAL  → Vermelho escuro com texto branco
```

### Ícones de Ação

```
➕ CREATE           📥 EXPORT
👁️ READ             🖨️ PRINT
✏️ UPDATE           ⚠️ ACCESS_DENIED
🗑️ DELETE           🔐 PERMISSION_CHANGED
🔓 LOGIN            📝 [outros]
🔒 LOGOUT
❌ LOGIN_FAILED
```

## Exemplos de Uso

### Exemplo 1: Buscar Erros Críticos

```
1. Abrir Filtros
2. Severidade: "CRITICAL"
3. Data Inicial: [hoje menos 7 dias]
4. Data Final: [hoje]
5. Clicar "Aplicar Filtros"
6. Ver lista de erros críticos
7. Clicar em 👁️ para ver detalhes
8. Exportar CSV para análise
```

### Exemplo 2: Rastrear Atividade de Usuário

```
1. Abrir Filtros
2. ID do Usuário: "user-123-abc"
3. Data Inicial: [data específica]
4. Data Final: [data específica]
5. Clicar "Aplicar Filtros"
6. Ver todas as ações do usuário
7. Analisar padrão de atividades
```

### Exemplo 3: Auditar Modificações em Paciente

```
1. Abrir Filtros
2. Tipo de Entidade: "Patient"
3. ID da Entidade: "patient-456-xyz"
4. Ação: "UPDATE"
5. Clicar "Aplicar Filtros"
6. Ver todas as modificações
7. No modal, ver campos alterados e diff
```

## Características Responsivas

### Desktop (> 1024px)
- Menu lateral sempre visível
- Tabela completa com todas as colunas
- Filtros em grid de 3 colunas

### Tablet (768px - 1024px)
- Menu lateral recolhível
- Tabela com scroll horizontal
- Filtros em grid de 2 colunas

### Mobile (< 768px)
- Menu lateral como overlay
- Tabela com scroll horizontal (largura mínima 1000px)
- Filtros em coluna única
- Botões de exportação em largura completa
- Modal de detalhes em tela cheia

## Temas

O sistema suporta tanto tema claro quanto escuro:

### Tema Claro
- Fundo: Branco (#FFFFFF)
- Texto: Cinza escuro (#1a1a1a)
- Cards: Branco com sombra sutil
- Badges: Cores pastel

### Tema Escuro
- Fundo: Cinza muito escuro (#0a0a0a)
- Texto: Cinza claro (#e5e5e5)
- Cards: Cinza escuro com sombra
- Badges: Cores mais saturadas

## Performance

- **Lazy Loading**: Componente carrega apenas quando acessado
- **Paginação**: Apenas 50 registros carregados por vez
- **Filtros Server-Side**: Filtragem feita no backend
- **Build Size**: ~105 KB do chunk de audit-logs

## Segurança

- ✅ Autenticação obrigatória (SystemAdmin)
- ✅ Proteção contra CSV injection
- ✅ Sanitização de inputs
- ✅ Nomes de arquivo seguros
- ✅ CodeQL scan sem alertas
- ✅ Tratamento de null/undefined

---

**Última Atualização**: 25 de Janeiro de 2026
**Versão**: 1.0.0
