# MedicWarehouse iOS App

Aplicativo iOS nativo para o sistema MedicWarehouse, desenvolvido com SwiftUI e integrando com a API RESTful do backend.

## 📱 Características

- **SwiftUI**: Interface moderna e nativa para iOS 17+
- **MVVM Architecture**: Arquitetura limpa e testável
- **API Integration**: Consumo completo da API MedicWarehouse
- **Async/Await**: Código assíncrono moderno e seguro
- **JWT Authentication**: Autenticação segura com tokens
- **Responsive Design**: Suporte para iPhone e iPad

## 🎨 Funcionalidades

### Autenticação
- ✅ Login de usuários (médicos, secretárias, etc.)
- ✅ Login de proprietários de clínicas
- ✅ Logout seguro
- ✅ Persistência de token JWT

### Dashboard
- ✅ Estatísticas em tempo real
- ✅ Consultas do dia
- ✅ Total de pacientes
- ✅ Consultas pendentes e concluídas
- ✅ Ações rápidas

### Pacientes
- ✅ Listagem de pacientes
- ✅ Busca por nome, CPF ou telefone
- ✅ Visualização de detalhes
- ✅ Pull to refresh

### Agendamentos
- ✅ Listagem de agendamentos
- ✅ Filtros por status (Todos, Agendados, Em Andamento, Concluídos)
- ✅ Visualização de detalhes
- ✅ Pull to refresh

### Perfil
- ✅ Informações do usuário
- ✅ Role e tenant
- ✅ Logout

## 🛠️ Requisitos

- **macOS**: Monterey (12.0) ou superior
- **Xcode**: 15.0 ou superior
- **iOS**: 17.0 ou superior (deployment target)
- **Swift**: 5.9 ou superior

## 🚀 Como Executar

### 1. Abrir o Projeto no Xcode

```bash
cd mobile/ios
open MedicWarehouse.xcodeproj
```

### 2. Configurar a API Base URL

Edite o arquivo `MedicWarehouse/Services/NetworkManager.swift` e configure a URL da API:

```swift
// Para desenvolvimento local
private let baseURL = "http://localhost:5000/api"

// Para produção
private let baseURL = "https://api.medicwarehouse.com/api"

// Para simulador iOS com API local (use o IP da máquina)
private let baseURL = "http://192.168.1.100:5000/api"
```

### 3. Selecionar o Simulador ou Dispositivo

No Xcode:
- Selecione um simulador (ex: iPhone 15 Pro) ou conecte um dispositivo físico
- Clique em "Run" (⌘R) ou no botão de play

### 4. Build e Executar

O Xcode fará o build automaticamente e instalará o app no simulador/dispositivo.

## 📝 Credenciais de Teste

Use as mesmas credenciais do sistema web:

```
Usuário: admin
Senha: Admin@123
Tenant ID: demo-clinic-001
```

Ou crie dados de teste usando o endpoint:
```bash
POST http://localhost:5000/api/data-seeder/seed-demo
```

## 📂 Estrutura do Projeto

```
MedicWarehouse/
├── MedicWarehouseApp.swift      # Entry point
├── ContentView.swift             # Root view
├── Info.plist                    # App configuration
├── Assets.xcassets/              # Images and colors
├── Views/                        # SwiftUI views
│   ├── LoginView.swift
│   ├── DashboardView.swift
│   ├── PatientsView.swift
│   └── AppointmentsView.swift
├── ViewModels/                   # View models (MVVM)
│   └── AuthViewModel.swift
├── Services/                     # API and network layer
│   ├── APIService.swift
│   └── NetworkManager.swift
└── Models/                       # Data models
    └── Models.swift
```

## 🔐 Segurança

- **JWT Tokens**: Armazenados de forma segura no UserDefaults
- **HTTPS**: Suporte completo para comunicação segura
- **Token Refresh**: Implementado no NetworkManager
- **NSAppTransportSecurity**: Configurado no Info.plist para desenvolvimento

## 🎨 Design System

O app segue as guidelines de design da Apple com:

- **SF Symbols**: Ícones nativos do sistema
- **Native Components**: Componentes nativos do SwiftUI
- **Color Palette**: 
  - Primary: Blue (`#007AFF`)
  - Secondary: Purple (`#5856D6`)
  - Success: Green (`#34C759`)
  - Warning: Orange (`#FF9500`)
  - Error: Red (`#FF3B30`)

## 🧪 Testes

Para adicionar testes:

```bash
# No Xcode, pressione ⌘U para executar os testes
# Ou use o menu: Product > Test
```

## 📱 Compatibilidade

- **iPhone**: iOS 17.0+
- **iPad**: iPadOS 17.0+
- **Orientações**: Portrait, Landscape Left, Landscape Right

## 🔄 API Integration

O app consome os seguintes endpoints da API:

### Authentication
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/owner-login` - Login de proprietário

### Patients
- `GET /api/patients` - Listar pacientes
- `GET /api/patients/{id}` - Buscar paciente por ID
- `GET /api/patients/search?searchTerm={term}` - Buscar pacientes

### Appointments
- `GET /api/appointments` - Listar agendamentos
- `GET /api/appointments/{id}` - Buscar agendamento por ID
- `GET /api/appointments/agenda` - Agenda do dia

## 🐛 Troubleshooting

### Erro de conexão com a API

Se você está usando o simulador iOS e não consegue conectar à API local:

1. Use o IP da sua máquina em vez de `localhost`
2. Verifique se o firewall não está bloqueando a conexão
3. Certifique-se de que a API está rodando

### Erro de certificado SSL

Para desenvolvimento local, o `NSAppTransportSecurity` está configurado para permitir conexões inseguras. Em produção, certifique-se de usar HTTPS.

## 🚀 Próximos Passos

- [ ] Implementar criação/edição de pacientes
- [ ] Implementar criação/edição de agendamentos
- [ ] Adicionar visualização de prontuários
- [ ] Implementar notificações push
- [ ] Adicionar suporte offline
- [ ] Implementar sincronização de dados
- [ ] Adicionar testes unitários e UI
- [ ] Implementar Dark Mode completo
- [ ] Adicionar localização (PT-BR, EN, ES)

## 📄 Licença

Este projeto está sob a mesma licença do projeto principal MedicWarehouse.

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação principal do projeto ou abra uma issue no GitHub.
