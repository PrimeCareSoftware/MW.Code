# Portal do Paciente - Frontend

Este é o frontend do Portal do Paciente, construído com Angular 20.

## 📋 Funcionalidades Implementadas

### Páginas
- **Login** - Autenticação de pacientes via email ou CPF
- **Registro** - Cadastro de novos pacientes
- **Dashboard** - Visão geral com estatísticas e acessos rápidos
- **Consultas** - Listagem e visualização de agendamentos
- **Documentos** - Visualização e download de documentos médicos
- **Perfil** - Gerenciamento de dados pessoais e alteração de senha

### Serviços
- **AuthService** - Gerenciamento de autenticação (login, registro, logout, refresh token)
- **AppointmentService** - Integração com API de agendamentos
- **DocumentService** - Integração com API de documentos

### Funcionalidades Técnicas
- **Auth Guard** - Proteção de rotas autenticadas
- **HTTP Interceptor** - Injeção automática de tokens JWT
- **Lazy Loading** - Carregamento otimizado de rotas
- **Material Design** - Interface moderna e responsiva

## 🚀 Como Executar

### Pré-requisitos
- Node.js 18+
- npm 9+

### Instalação

```bash
# Instalar dependências
npm install
```

### Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm start

# O app estará disponível em http://localhost:4200/
```

### Build de Produção

```bash
# Build para produção
npm run build

# Os arquivos estarão em dist/patient-portal/
```

### Testes

```bash
# Executar testes unitários
npm test
```

## 🔧 Configuração

### Ambiente de Desenvolvimento
Edite `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // URL da API backend
};
```

### Ambiente de Produção
Edite `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: '/api'  // URL relativa para produção
};
```

## 📁 Estrutura do Projeto

```
src/
├── app/
│   ├── pages/                 # Páginas da aplicação
│   │   ├── auth/              # Login, registro
│   │   ├── dashboard/         # Dashboard
│   │   ├── appointments/      # Gestão de consultas
│   │   ├── documents/         # Visualização de documentos
│   │   └── profile/           # Perfil do usuário
│   ├── services/              # Serviços Angular
│   │   ├── auth.service.ts
│   │   ├── appointment.service.ts
│   │   └── document.service.ts
│   ├── guards/                # Guards de autenticação
│   │   └── auth.guard.ts
│   ├── interceptors/          # HTTP interceptors
│   │   └── auth.interceptor.ts
│   └── models/                # Modelos TypeScript
│       ├── auth.model.ts
│       ├── appointment.model.ts
│       └── document.model.ts
└── environments/              # Configurações de ambiente
```

## 🎨 Design System

O projeto utiliza Angular Material com customizações:
- Paleta de cores: Gradiente roxo/azul (#667eea → #764ba2)
- Typography: Inter font family
- Mobile-first design
- Componentes reutilizáveis

## 🔐 Segurança

- Tokens JWT armazenados em localStorage
- Refresh tokens automáticos via interceptor
- Auth guard protegendo rotas
- HTTPS obrigatório em produção

## 📚 Documentação Adicional

Para mais informações sobre o projeto completo:
- [PATIENT_PORTAL_GUIDE.md](../../docs/PATIENT_PORTAL_GUIDE.md) - Guia geral
- [README.md](../../patient-portal-api/README.md) - Documentação do backend
- [ARCHITECTURE.md](../../patient-portal-api/ARCHITECTURE.md) - Arquitetura técnica

---

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 20.3.13.
