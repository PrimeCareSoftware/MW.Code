# MW System Admin - Painel de Administração do Sistema

Sistema de administração dedicado para System Owners do MedicWarehouse gerenciarem todas as clínicas e usuários do sistema.

## 📋 Sobre

O MW System Admin é um aplicativo Angular standalone separado do aplicativo principal do MedicWarehouse, projetado especificamente para proprietários do sistema (System Owners) gerenciarem:

- 🏥 **Clínicas**: Visualizar, criar, ativar/desativar e gerenciar todas as clínicas
- 👥 **Usuários System Owner**: Adicionar e gerenciar outros administradores do sistema
- 💰 **Financeiro**: Monitorar MRR, assinaturas e métricas financeiras
- 📊 **Analytics**: Dashboard com métricas globais do sistema
- ⚙️ **Configurações**: Gerenciar assinaturas, planos e override manual

## 🚀 Como Executar

### Pré-requisitos

- Node.js 18+ instalado
- NPM ou Yarn
- Backend API MedicWarehouse rodando (padrão: http://localhost:5000)

### Instalação

```bash
# Navegar para o diretório do projeto
cd frontend/mw-system-admin

# Instalar dependências
npm install
```

### Executar em Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm start

# O aplicativo estará disponível em http://localhost:4200
```

### Build para Produção

```bash
# Gerar build de produção
npm run build

# Os arquivos serão gerados em dist/mw-system-admin
```

## 🔐 Login

Para acessar o sistema, você precisa ter um usuário System Owner cadastrado no backend com `tenantId = "system"` e `IsSystemOwner = true`.

## �� Funcionalidades

- Dashboard com métricas globais
- Gestão completa de clínicas
- Controle de assinaturas e planos
- Override manual para casos especiais
- Gestão de System Owners (em desenvolvimento)

## 📖 Documentação

Veja a [documentação completa](../../SYSTEM_OWNER_ACCESS.md) para mais detalhes.
