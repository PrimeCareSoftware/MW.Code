# Resumo Visual dos Fluxos - PrimeCare Software

## 🎯 Visão Geral do Sistema

Este documento apresenta um resumo visual rápido dos principais fluxos do sistema PrimeCare Software.

Para documentação completa, consulte: [SCREENS_DOCUMENTATION.md](../SCREENS_DOCUMENTATION.md)

---

## 🗺️ Mapa de Navegação Completo

```mermaid
graph TB
    %% Autenticação
    LOGIN[🔐 Login]
    REGISTER[📝 Cadastro]
    
    %% Dashboard Central
    DASH[🏠 Dashboard]
    
    %% Módulo Pacientes
    PAT_LIST[👥 Lista Pacientes]
    PAT_FORM[📋 Form. Paciente]
    
    %% Módulo Agendamentos
    APPT_LIST[📅 Lista Agendamentos]
    APPT_FORM[📝 Form. Agendamento]
    ATTEND[⚕️ Atendimento]
    
    %% Conexões de Autenticação
    LOGIN -->|Login bem-sucedido| DASH
    LOGIN -.->|Não tem conta| REGISTER
    REGISTER -.->|Já tem conta| LOGIN
    
    %% Conexões Dashboard
    DASH -->|Gerenciar Pacientes| PAT_LIST
    DASH -->|Gerenciar Agenda| APPT_LIST
    DASH -->|+ Novo Paciente| PAT_FORM
    DASH -->|+ Novo Agendamento| APPT_FORM
    
    %% Módulo Pacientes
    PAT_LIST -->|+ Novo| PAT_FORM
    PAT_LIST -->|Editar| PAT_FORM
    PAT_FORM -->|Salvar| PAT_LIST
    PAT_FORM -->|Cancelar| PAT_LIST
    
    %% Módulo Agendamentos
    APPT_LIST -->|+ Novo| APPT_FORM
    APPT_LIST -->|Iniciar Atendimento| ATTEND
    APPT_FORM -->|Criar| APPT_LIST
    APPT_FORM -->|Cancelar| APPT_LIST
    ATTEND -->|Finalizar| APPT_LIST
    ATTEND -->|Voltar| APPT_LIST
    
    %% Todas as telas voltam para Dashboard via navbar
    PAT_LIST -.->|Navbar| DASH
    PAT_FORM -.->|Navbar| DASH
    APPT_LIST -.->|Navbar| DASH
    APPT_FORM -.->|Navbar| DASH
    ATTEND -.->|Navbar| DASH
    
    %% Estilos
    classDef auth fill:#e1f5ff,stroke:#0277bd,stroke-width:2px
    classDef main fill:#fff4e1,stroke:#f57c00,stroke-width:2px
    classDef patient fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    classDef appt fill:#e8f5e9,stroke:#388e3c,stroke-width:2px
    
    class LOGIN,REGISTER auth
    class DASH main
    class PAT_LIST,PAT_FORM patient
    class APPT_LIST,APPT_FORM,ATTEND appt
```

---

## 📊 Ciclo de Vida do Atendimento

```mermaid
stateDiagram-v2
    [*] --> Agendado: Criar agendamento
    
    Agendado --> Confirmado: Paciente confirma
    Agendado --> Cancelado: ❌ Cancelar
    
    Confirmado --> EmAtendimento: ▶️ Iniciar
    Confirmado --> Cancelado: ❌ Cancelar
    
    EmAtendimento --> Concluído: ✅ Finalizar
    
    Concluído --> [*]
    Cancelado --> [*]
    
    note right of Agendado
        Status inicial
        Aguardando data
    end note
    
    note right of EmAtendimento
        Timer ativo
        Prontuário aberto
        Histórico visível
    end note
    
    note right of Concluído
        Prontuário salvo
        Timeline atualizada
        Prescrição gerada
    end note
```

---

## 🔄 Fluxo 1: Primeiro Atendimento (Paciente Novo)

```mermaid
sequenceDiagram
    actor 👨‍⚕️ as Médico
    participant 🏠 as Dashboard
    participant 👥 as Pacientes
    participant 📋 as Cadastro
    participant 📅 as Agenda
    participant 🗓️ as Novo Agend.
    participant ⚕️ as Atendimento

    👨‍⚕️->>🏠: 1. Login no sistema
    🏠->>📋: 2. + Novo Paciente
    👨‍⚕️->>📋: 3. Preenche dados<br/>(Nome, CPF, contato...)
    📋->>👥: 4. Salvar ✓<br/>Paciente criado
    
    👥->>🗓️: 5. + Novo Agendamento
    👨‍⚕️->>🗓️: 6. Seleciona paciente<br/>Define data/hora
    🗓️->>📅: 7. Criar ✓<br/>Agendamento criado
    
    Note over 📅: Dia do atendimento
    
    📅->>⚕️: 8. Iniciar Atendimento ▶️
    
    Note over ⚕️: Histórico vazio<br/>(primeira consulta)
    
    👨‍⚕️->>⚕️: 9. Preenche prontuário:<br/>• Diagnóstico<br/>• Prescrição<br/>• Observações
    ⚕️->>📅: 10. Finalizar ✅<br/>Consulta concluída
    
    Note over 📅: Timeline atualizada<br/>Prescrição disponível
```

---

## 🔄 Fluxo 2: Atendimento Recorrente (Paciente Existente)

```mermaid
sequenceDiagram
    actor 👨‍⚕️ as Médico
    participant 🏠 as Dashboard
    participant 📅 as Agenda
    participant ⚕️ as Atendimento
    participant 📜 as Histórico

    👨‍⚕️->>🏠: 1. Login no sistema
    🏠->>📅: 2. Agendamentos
    
    Note over 📅: Lista com agendamento<br/>de paciente recorrente
    
    📅->>⚕️: 3. Iniciar Atendimento ▶️
    
    ⚕️->>📜: 4. Carrega histórico
    
    Note over 📜: Consultas anteriores:<br/>• 15/12/2023 - Gripe<br/>• 10/11/2023 - Check-up<br/>• 05/10/2023 - Rotina
    
    👨‍⚕️->>⚕️: 5. Revisa histórico
    Note over ⚕️: Alergias destacadas ⚠️<br/>Diagnósticos anteriores
    
    👨‍⚕️->>⚕️: 6. Preenche novo prontuário
    ⚕️->>📅: 7. Finalizar ✅
    
    Note over 📅: Histórico ampliado<br/>Nova entrada na timeline
```

---

## 🔄 Fluxo 3: Vínculo de Paciente (Outra Clínica)

```mermaid
sequenceDiagram
    actor 👨‍⚕️ as Recepcionista
    participant 🏠 as Dashboard
    participant 👥 as Lista Pacientes
    participant 🔍 as Busca
    participant 🔗 as Vínculo
    participant ✅ as Confirmação

    👨‍⚕️->>🏠: 1. Login no sistema
    🏠->>👥: 2. Pacientes
    👥->>🔍: 3. Buscar por CPF
    
    👨‍⚕️->>🔍: 4. Digita CPF:<br/>123.456.789-00
    
    Note over 🔍: Sistema busca<br/>em todas as clínicas
    
    🔍-->>🔗: 5. Paciente encontrado! ✓<br/>Cadastrado em Clínica A
    
    Note over 🔗: Dados pré-existentes:<br/>• Nome: João Silva<br/>• Idade: 35 anos<br/>• Contato: (11) 98765-4321
    
    👨‍⚕️->>🔗: 6. Confirma dados<br/>(pode atualizar se necessário)
    🔗->>✅: 7. Vincular à Clínica B ✓
    
    Note over ✅: Vínculo criado!<br/>Paciente disponível<br/>para agendamento
    
    ✅->>👥: 8. Retorna para lista<br/>Paciente vinculado
```

---

## 📋 Resumo das Telas

| # | Tela | Função Principal | Acesso |
|---|------|------------------|--------|
| 1 | **Login** | Autenticação de usuário | Entrada do sistema |
| 2 | **Cadastro** | Criar nova conta | Link no Login |
| 3 | **Dashboard** | Visão geral e navegação | Após login |
| 4 | **Lista de Pacientes** | Gerenciar pacientes | Dashboard → Pacientes |
| 5 | **Formulário de Paciente** | Criar/editar paciente | Lista ou Dashboard → + Novo |
| 6 | **Lista de Agendamentos** | Visualizar agenda | Dashboard → Agendamentos |
| 7 | **Formulário de Agendamento** | Criar agendamento | Lista ou Dashboard → + Novo |
| 8 | **Atendimento** | Realizar consulta e prontuário | Lista Agendamentos → Iniciar |

---

## 🎨 Componentes Visuais Principais

### Navbar (Presente em todas as telas autenticadas)
```
┌────────────────────────────────────────────┐
│ [🏥 PrimeCare Software] [Dashboard] [Pacientes] │
│ [Agendamentos] [Financeiro] [Sair] 👤      │
└────────────────────────────────────────────┘
```

### Card de Ação Rápida (Dashboard)
```
┌──────────────┐
│   👥 Ícone   │
│              │
│   Título     │
│              │
│  Descrição   │
│   do card    │
└──────────────┘
```

### Item de Timeline (Atendimento)
```
┌─────────────────────────────┐
│ 📅 15/01/2024 14:30         │
│ Consulta Regular (30 min)   │
│ Diagnóstico: Hipertensão    │
│ Prescrição: Losartana 50mg  │
└─────────────────────────────┘
```

### Card de Agendamento (Lista)
```
┌──────────────────────────────────────┐
│ 08:00 │ João Silva                  │
│       │ Consulta - 30min            │
│       │ Status: Agendado            │
│       │ [Iniciar] [Cancelar]        │
└──────────────────────────────────────┘
```

---

## 🚦 Indicadores de Status

- 🟢 **Verde**: Concluído, Ativo, Sucesso
- 🔵 **Azul**: Em Progresso, Informação
- 🟡 **Amarelo**: Aguardando, Aviso
- 🔴 **Vermelho**: Cancelado, Erro
- ⚫ **Cinza**: Inativo, Desabilitado

---

## 📱 Ações Disponíveis por Tela

### Dashboard
- ✅ Acessar módulo de Pacientes
- ✅ Acessar módulo de Agendamentos
- ✅ Criar novo paciente (ação rápida)
- ✅ Criar novo agendamento (ação rápida)
- 🚧 Acessar Financeiro (em desenvolvimento)
- 🚧 Acessar Prontuários (em desenvolvimento)

### Lista de Pacientes
- ✅ Criar novo paciente
- ✅ Editar paciente existente
- ✅ Excluir paciente (com confirmação)
- ✅ Buscar/filtrar pacientes
- ✅ Ver detalhes do paciente

### Formulário de Paciente
- ✅ Preencher dados pessoais
- ✅ Preencher dados de contato
- ✅ Preencher endereço completo
- ✅ Registrar informações médicas
- ✅ Salvar paciente
- ⚠️ Campos imutáveis em edição: CPF, Data Nascimento, Gênero

### Lista de Agendamentos
- ✅ Alternar entre visualização Lista/Calendário
- ✅ Selecionar data para visualizar agenda
- ✅ Criar novo agendamento
- ✅ Iniciar atendimento
- ✅ Continuar atendimento em andamento
- ✅ Cancelar agendamento
- ✅ Navegar entre meses (modo calendário)

### Formulário de Agendamento
- ✅ Selecionar paciente
- ✅ Definir data e horário
- ✅ Configurar duração
- ✅ Escolher tipo de consulta
- ✅ Adicionar observações
- ✅ Criar agendamento

### Atendimento
- ✅ Visualizar informações do paciente
- ✅ Ver histórico de consultas
- ✅ Monitorar tempo de consulta (timer)
- ✅ Preencher diagnóstico
- ✅ Escrever prescrição
- ✅ Adicionar observações
- ✅ Solicitar exames
- ✅ Agendar retorno
- ✅ Salvar rascunho
- ✅ Finalizar consulta
- ✅ Imprimir prescrição

---

## 🔐 Validações Importantes

### Paciente
- ✅ CPF único no tenant (não pode haver duplicatas na mesma clínica)
- ✅ CPF pode existir globalmente (paciente em múltiplas clínicas)
- ✅ E-mail único no tenant
- ✅ Campos obrigatórios: Nome, CPF, Data Nascimento, Gênero, Email, Telefone
- ⚠️ Campos imutáveis após criação: CPF, Data Nascimento, Gênero

### Agendamento
- ✅ Paciente deve estar vinculado à clínica
- ✅ Data não pode ser no passado
- ✅ Duração mínima: 15 minutos
- ✅ Todos os campos obrigatórios: Paciente, Data, Hora, Duração, Tipo

### Prontuário (Atendimento)
- ✅ Diagnóstico obrigatório para finalizar
- ✅ Timer inicia automaticamente
- ✅ Histórico isolado por clínica (multi-tenancy)
- ⚠️ Alergias destacadas para segurança

---

## 📚 Documentação Completa

Para informações detalhadas sobre cada tela, incluindo:
- Mockups completos em ASCII
- Descrições extensivas de funcionalidades
- Regras de negócio específicas
- Exemplos de uso
- Cenários de teste

Consulte: **[SCREENS_DOCUMENTATION.md](../SCREENS_DOCUMENTATION.md)**

---

## 🔗 Links Relacionados

- [README.md](../README.md) - Visão geral do projeto
- [BUSINESS_RULES.md](../BUSINESS_RULES.md) - Regras de negócio
- [TECHNICAL_IMPLEMENTATION.md](../TECHNICAL_IMPLEMENTATION.md) - Detalhes técnicos
- [SCREENS_DOCUMENTATION.md](../SCREENS_DOCUMENTATION.md) - Documentação completa de telas

---

**Última atualização**: Janeiro 2025  
**Versão**: 1.0  
**Equipe**: PrimeCare Software
