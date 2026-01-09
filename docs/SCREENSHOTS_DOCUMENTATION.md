# 📸 Documentação Visual - Screenshots das Telas

> **Objetivo:** Documentação visual completa com capturas de tela de todas as aplicações do PrimeCare Software.

> **Última Atualização:** Novembro 2025

---

## 🏥 PrimeCare Software App (Frontend Principal)

### 1. Tela de Login

![Tela de Login](https://github.com/user-attachments/assets/7676fba5-7abb-4d49-b5a7-137d5f216878)

**Descrição:** Tela de autenticação do sistema com campos para usuário, senha e Tenant ID.

**Funcionalidades:**
- Login de usuários (médicos, secretárias, enfermeiros)
- Login de proprietários de clínicas
- Suporte a subdomínio para auto-preenchimento do Tenant
- Link para cadastro de nova conta

---

### 2. Tela de Cadastro

![Tela de Cadastro](https://github.com/user-attachments/assets/d7afd118-6b8f-4903-a4f6-484debd17b31)

**Descrição:** Formulário de registro para novas clínicas e proprietários.

**Funcionalidades:**
- Cadastro de usuário, e-mail e senha
- Definição do Tenant ID
- Validação de campos obrigatórios
- Link para login existente

---

### 3. Dashboard Principal

![Dashboard](https://github.com/user-attachments/assets/08890c4d-2199-4f4d-a757-0f5a6a725f5a)

**Descrição:** Visão geral do sistema com cards de acesso rápido e ações principais.

**Funcionalidades:**
- Navegação principal (Dashboard, Pacientes, Agendamentos)
- Cards de módulos (Pacientes, Agendamentos, Financeiro, Prontuários)
- Ações rápidas (Novo Paciente, Novo Agendamento)
- Identificação do usuário logado
- Módulos "Em breve" (Financeiro, Prontuários)

---

### 4. Lista de Pacientes

![Lista de Pacientes](https://github.com/user-attachments/assets/48b1395f-588b-4121-b5be-fe717702edee)

**Descrição:** Gerenciamento completo de pacientes cadastrados.

**Funcionalidades:**
- Listagem de todos os pacientes
- Busca e filtros
- Botão para novo paciente
- Estado vazio com call-to-action
- Tratamento de erros de conexão

---

### 5. Formulário de Paciente

![Formulário de Paciente](https://github.com/user-attachments/assets/597e0b4a-861a-48a0-92aa-305979cae39a)

**Descrição:** Cadastro completo de novo paciente com todas as informações necessárias.

**Seções:**
- **Dados Pessoais:** Nome, CPF, Data de Nascimento, Gênero
- **Contato:** E-mail, Telefone
- **Endereço:** CEP, Rua, Número, Complemento, Bairro, Cidade, Estado
- **Informações Médicas:** Histórico Médico, Alergias

**Funcionalidades:**
- Validação de campos obrigatórios
- Botões de Cancelar e Salvar
- Navegação de volta para lista

---

### 6. Lista de Agendamentos

**Descrição:** Agenda diária de consultas com seleção de data.

**Funcionalidades:**
- Seleção de data
- Visualização em lista ou calendário
- Botão para novo agendamento
- Status de carregamento
- Tratamento de erros

---

### 7. Formulário de Agendamento

**Descrição:** Criação de novo agendamento de consulta.

**Campos:**
- **Paciente:** Seleção do paciente cadastrado
- **Data e Horário:** Definição da consulta
- **Duração:** Tempo em minutos (padrão: 30)
- **Tipo:** Regular, Emergência, Retorno, Consulta
- **Observações:** Campo livre para anotações

---

## 📱 Aplicativos Mobile

### iOS App (Swift/SwiftUI)

> Screenshots dos aplicativos mobile serão adicionados conforme desenvolvimento.

**Telas Implementadas:**
- Login
- Dashboard
- Lista de Pacientes
- Lista de Agendamentos
- Perfil do Usuário

### Android App (Kotlin/Compose)

> Screenshots dos aplicativos mobile serão adicionados conforme desenvolvimento.

**Telas Implementadas:**
- Login
- Dashboard

---

## 🔧 MW System Admin

> Screenshots do painel administrativo serão adicionados conforme desenvolvimento.

**Telas Planejadas:**
- Login de System Owner
- Dashboard Global
- Gestão de Clínicas
- Gestão de Planos
- Relatórios

---

## 🌐 MW Site (Landing Page)

> Screenshots do site institucional serão adicionados conforme desenvolvimento.

**Telas Planejadas:**
- Home / Landing Page
- Funcionalidades
- Planos e Preços
- Contato

---

## 📚 MW Docs (Documentação)

> Screenshots da central de documentação serão adicionados conforme desenvolvimento.

**Telas Planejadas:**
- Home com listagem de documentos
- Visualizador de documentos
- Busca

---

## 🎨 Design System

### Paleta de Cores

| Cor | Hex | Uso |
|-----|-----|-----|
| Primary (Indigo) | `#6366F1` | Botões principais, links, destaques |
| Secondary (Purple) | `#8B5CF6` | Elementos secundários |
| Background | `#F8FAFC` | Fundo das páginas |
| Card Background | `#FFFFFF` | Cards e containers |
| Text Primary | `#1E293B` | Textos principais |
| Text Secondary | `#64748B` | Textos secundários |
| Success | `#22C55E` | Sucesso, confirmações |
| Warning | `#F59E0B` | Alertas, avisos |
| Error | `#EF4444` | Erros, exclusões |

### Tipografia

- **Títulos:** Inter, bold
- **Corpo:** Inter, regular
- **Monospace:** Fira Code (código)

### Componentes

- Botões com cantos arredondados
- Cards com sombra suave
- Inputs com bordas claras
- Navegação horizontal no topo
- Gradiente no fundo da tela de login

---

## 🔗 Documentação Relacionada

- [APPS_PENDING_TASKS.md](APPS_PENDING_TASKS.md) - Pendências de desenvolvimento dos apps
- [PENDING_TASKS.md](PENDING_TASKS.md) - Pendências gerais do sistema
- [TELAS_COM_FLUXO.md](TELAS_COM_FLUXO.md) - Documentação de fluxos com mockups ASCII
- [MOBILE_APPS_GUIDE.md](MOBILE_APPS_GUIDE.md) - Guia dos aplicativos mobile

---

**Documento Elaborado Por:** GitHub Copilot  
**Data:** Novembro 2025  
**Versão:** 1.0
