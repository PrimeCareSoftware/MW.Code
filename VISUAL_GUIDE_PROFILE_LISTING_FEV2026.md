# Visual Guide: Profile Listing - What You Should See

## 📋 Scenario 1: Single Clinic in Tenant

### Clinic Configuration
- **Tenant**: "Clinica Exemplo"
- **Clinic**: "Clinica Médica Central"
- **Type**: Medical

### Expected Profiles in List
```
╔══════════════════════════════════════════════════════════════╗
║                    PERFIS DISPONÍVEIS                         ║
╠══════════════════════════════════════════════════════════════╣
║                                                               ║
║  📋 PERFIS PADRÃO (4)                                         ║
║  ├─ Proprietário [Padrão] ✓                                  ║
║  ├─ Médico [Padrão] ✓                                        ║
║  ├─ Recepção/Secretaria [Padrão] ✓                          ║
║  └─ Financeiro [Padrão] ✓                                    ║
║                                                               ║
║  ✏️ PERFIS CUSTOMIZADOS (0)                                   ║
║  └─ (Nenhum perfil customizado criado)                       ║
║                                                               ║
║  TOTAL: 4 perfis                                             ║
║                                                               ║
╚══════════════════════════════════════════════════════════════╝
```

**⚠️ NOTA**: Com apenas uma clínica no tenant, você só verá os perfis dessa clínica.

---

## 📋 Scenario 2: Multiple Clinics in Same Tenant (IDEAL)

### Tenant Configuration
- **Tenant**: "Grupo Saúde Total"
- **Clinic 1**: "Clinica Médica Centro" (Medical)
- **Clinic 2**: "Clinica Odontológica Sul" (Dental)
- **Clinic 3**: "Clinica de Nutrição Norte" (Nutritionist)
- **Current User**: Owner of Clinic 1 (Medical)

### Expected Profiles in List
```
╔══════════════════════════════════════════════════════════════════╗
║                      PERFIS DISPONÍVEIS                           ║
╠══════════════════════════════════════════════════════════════════╣
║                                                                   ║
║  📋 PERFIS PADRÃO - DA MINHA CLÍNICA (4)                          ║
║  ├─ Proprietário [Padrão] [Clínica Médica Centro] ✓             ║
║  ├─ Médico [Padrão] [Clínica Médica Centro] ✓                   ║
║  ├─ Recepção/Secretaria [Padrão] [Clínica Médica Centro] ✓      ║
║  └─ Financeiro [Padrão] [Clínica Médica Centro] ✓               ║
║                                                                   ║
║  📋 PERFIS PADRÃO - DE OUTRAS CLÍNICAS NO TENANT (6)              ║
║  ├─ Proprietário [Padrão] [Clínica Odontológica Sul] ✓          ║
║  ├─ Dentista [Padrão] [Clínica Odontológica Sul] ✓ ⭐          ║
║  ├─ Recepção/Secretaria [Padrão] [Clínica Odontológica Sul] ✓   ║
║  ├─ Financeiro [Padrão] [Clínica Odontológica Sul] ✓            ║
║  ├─ Nutricionista [Padrão] [Clínica de Nutrição Norte] ✓ ⭐     ║
║  └─ ... outros perfis de outras clínicas                         ║
║                                                                   ║
║  ✏️ PERFIS CUSTOMIZADOS - DA MINHA CLÍNICA (1)                   ║
║  └─ Médico Plantonista [Custom] [Minha Clínica] ✓              ║
║                                                                   ║
║  TOTAL: 11+ perfis                                               ║
║  (4 da minha clínica + 6+ de outras clínicas + 1 customizado)    ║
║                                                                   ║
╚══════════════════════════════════════════════════════════════════╝
```

**✅ CORRETO**: Com múltiplas clínicas, você vê TODOS os perfis padrão!

---

## 📋 Scenario 3: All Profile Types Available

### Full Tenant with All Clinic Types
When multiple clinics of different types exist in the tenant:

```
╔════════════════════════════════════════════════════════════════╗
║              TODOS OS PERFIS PADRÃO DISPONÍVEIS                 ║
╠════════════════════════════════════════════════════════════════╣
║                                                                 ║
║  👔 GESTÃO                                                      ║
║  ├─ Proprietário [Padrão]                                      ║
║  ├─ Recepção/Secretaria [Padrão]                               ║
║  └─ Financeiro [Padrão]                                        ║
║                                                                 ║
║  🏥 PROFISSIONAIS DE SAÚDE                                      ║
║  ├─ Médico [Padrão]                                            ║
║  ├─ Dentista [Padrão]                                          ║
║  ├─ Nutricionista [Padrão]                                     ║
║  ├─ Psicólogo [Padrão]                                         ║
║  ├─ Fisioterapeuta [Padrão]                                    ║
║  └─ Veterinário [Padrão]                                       ║
║                                                                 ║
║  ✏️ PERFIS CUSTOMIZADOS                                         ║
║  └─ [Seus perfis personalizados aqui]                          ║
║                                                                 ║
║  TOTAL: 9 perfis padrão + customizados                         ║
║                                                                 ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 🖥️ Tela de Cadastro de Usuário

### Dropdown de Seleção de Perfil

#### ❌ ANTES (Incorreto)
```
┌─────────────────────────────────┐
│ Selecionar Perfil:         [▼]  │
├─────────────────────────────────┤
│ Proprietário                     │
│ Médico                           │ ← Apenas perfis da clínica
│ Recepção/Secretaria              │
│ Financeiro                       │
└─────────────────────────────────┘
(4 opções apenas)
```

#### ✅ DEPOIS (Correto - Multi-Tenant)
```
┌─────────────────────────────────┐
│ Selecionar Perfil:         [▼]  │
├─────────────────────────────────┤
│ ═══ PERFIS DISPONÍVEIS ═══       │
│                                  │
│ Proprietário (Padrão)            │
│ Médico (Padrão)                  │
│ Dentista (Padrão) ⭐             │ ← De outra clínica!
│ Nutricionista (Padrão) ⭐        │ ← De outra clínica!
│ Psicólogo (Padrão) ⭐            │ ← De outra clínica!
│ Fisioterapeuta (Padrão) ⭐       │ ← De outra clínica!
│ Veterinário (Padrão) ⭐          │ ← De outra clínica!
│ Recepção/Secretaria (Padrão)     │
│ Financeiro (Padrão)              │
│                                  │
│ ─── Perfis Customizados ───      │
│ Médico Plantonista               │
│                                  │
└─────────────────────────────────┘
(9+ opções - TODOS os perfis!)

ℹ️ Mostrando todos os perfis disponíveis (9 perfis)
```

---

## 🖥️ Tela de Perfis de Acesso

### Lista de Perfis

```
╔═════════════════════════════════════════════════════════════════════════╗
║                         PERFIS DE ACESSO                                 ║
╠═════════════════════════════════════════════════════════════════════════╣
║                                                                          ║
║  Nome                     │ Tipo      │ Usuários │ Ações                ║
║  ────────────────────────│───────────│──────────│───────────────        ║
║  Proprietário            │ [Padrão]  │    1     │ 👁️ Ver               ║
║  Médico                  │ [Padrão]  │    5     │ 👁️ Ver               ║
║  Dentista               │ [Padrão]  │    0     │ 👁️ Ver ⭐           ║
║  Nutricionista          │ [Padrão]  │    0     │ 👁️ Ver ⭐           ║
║  Psicólogo              │ [Padrão]  │    0     │ 👁️ Ver ⭐           ║
║  Fisioterapeuta         │ [Padrão]  │    0     │ 👁️ Ver ⭐           ║
║  Veterinário            │ [Padrão]  │    0     │ 👁️ Ver ⭐           ║
║  Recepção/Secretaria    │ [Padrão]  │    3     │ 👁️ Ver               ║
║  Financeiro             │ [Padrão]  │    1     │ 👁️ Ver               ║
║  Médico Plantonista     │ [Custom]  │    2     │ ✏️ ✏️ 🗑️             ║
║                                                                          ║
║  Total: 10 perfis (9 padrão + 1 customizado)                            ║
║                                                                          ║
╚═════════════════════════════════════════════════════════════════════════╝

⭐ = Perfis de outras clínicas que você pode usar
```

---

## 🔍 Como Verificar no Console do Navegador

Abra o Console (F12) e procure por:

### ✅ Sucesso - Carregamento Correto
```
✅ Successfully loaded 9 access profiles
📋 Available profiles for selection: 9 (9 default, 0 custom)
```

### ❌ Problema - Poucos Perfis
```
✅ Successfully loaded 4 access profiles
📋 Available profiles for selection: 4 (4 default, 0 custom)
⚠️ Esperado: 9+ perfis, mas recebeu apenas 4
```

---

## 🎯 Casos de Uso

### Caso 1: Clínica Médica Contrata Nutricionista
**Antes**: ❌ Não tinha perfil Nutricionista disponível  
**Agora**: ✅ Pode selecionar perfil "Nutricionista (Padrão)"

### Caso 2: Clínica Odontológica Adiciona Psicólogo  
**Antes**: ❌ Precisava criar perfil manualmente  
**Agora**: ✅ Pode selecionar perfil "Psicólogo (Padrão)"

### Caso 3: Clínica Multi-Especialidade
**Antes**: ❌ Limitada aos perfis do tipo principal  
**Agora**: ✅ Pode usar TODOS os perfis profissionais

---

## ⚠️ Se Você Não Vê Todos os Perfis

### Possíveis Causas

1. **Apenas Uma Clínica no Tenant**
   - Solução: Criar mais clínicas ou executar seed de dados

2. **Perfis Padrão Não Criados**
   - Solução: Chamar `POST /api/accessprofiles/create-defaults-by-type`

3. **Problema de Permissão**
   - Solução: Verificar se você é Owner da clínica

4. **Erro no Frontend**
   - Solução: Verificar console (F12) para mensagens de erro

---

## 📊 Comparação Rápida

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Perfis Visíveis** | 4-5 | 9-12+ |
| **Multi-Especialidade** | ❌ Limitado | ✅ Total |
| **Expansão** | ❌ Manual | ✅ Automática |
| **Flexibilidade** | ❌ Restrita | ✅ Completa |

---

**Implementado**: Fevereiro 2026  
**Status**: ✅ Funcionando Corretamente
