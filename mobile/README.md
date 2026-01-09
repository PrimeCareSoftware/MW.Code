# PrimeCare Software Mobile Apps

Aplicativos móveis nativos para o sistema PrimeCare Software, disponíveis para **iOS** (iPhone/iPad) e **Android**.

## 📱 Plataformas Suportadas

### iOS (Swift/SwiftUI)
- **Localização**: `mobile/ios/`
- **Versão mínima**: iOS 17.0+
- **Tecnologias**: Swift 5.9, SwiftUI, Combine
- **[Documentação completa →](ios/README.md)**

### Android (Kotlin/Jetpack Compose)
- **Localização**: `mobile/android/`
- **Versão mínima**: Android 7.0 (API 24)
- **Tecnologias**: Kotlin, Jetpack Compose, Coroutines, Hilt
- **[Documentação completa →](android/README.md)**

## 🎯 Características Comuns

Ambos os aplicativos compartilham as mesmas funcionalidades core:

### ✅ Autenticação
- Login de usuários (médicos, secretárias, enfermeiros)
- Login de proprietários de clínicas
- Logout seguro
- Persistência de sessão com JWT tokens

### ✅ Dashboard
- Visão geral em tempo real
- Consultas agendadas para hoje
- Total de pacientes cadastrados
- Consultas pendentes e concluídas
- Ações rápidas

### ✅ Pacientes
- Listagem com paginação
- Busca por nome, CPF ou telefone
- Visualização de detalhes
- Pull to refresh
- (Criação/edição em desenvolvimento)

### ✅ Agendamentos
- Listagem com paginação
- Filtros por status (Agendados, Em Andamento, Concluídos)
- Visualização de detalhes
- Pull to refresh
- (Criação/edição em desenvolvimento)

## 🏗️ Arquitetura

Ambos os apps seguem padrões de arquitetura modernos e recomendados:

### iOS (MVVM + Combine)
```
View (SwiftUI) → ViewModel → APIService → NetworkManager → Backend API
                    ↓
              @Published
                    ↓
            View (recompose)
```

### Android (MVVM + Clean Architecture)
```
UI (Compose) → ViewModel → Repository → ApiService → Backend API
                ↓
             StateFlow
                ↓
          UI (recompose)
```

## 🔐 Segurança

Ambos os apps implementam as melhores práticas de segurança mobile:

- **JWT Authentication**: Tokens armazenados de forma segura
- **HTTPS**: Comunicação criptografada (produção)
- **Certificate Pinning**: Pronto para implementação
- **Secure Storage**: 
  - iOS: Keychain (UserDefaults para desenvolvimento)
  - Android: DataStore com encriptação
- **No hardcoded secrets**: Configurações via environment/build config
- **Biometric Authentication**: Planejado para próximas versões

## 🎨 Design

Ambos os apps seguem as diretrizes de design de suas respectivas plataformas:

### iOS - Human Interface Guidelines
- Native SF Symbols icons
- SwiftUI native components
- iOS-style navigation
- Dynamic Type support
- Dark Mode support

### Android - Material Design 3
- Material icons
- Material 3 components
- Android-style navigation
- Adaptive layouts
- Dark Theme support

### Color Palette (Comum)
- **Primary**: Indigo/Blue (`#6366F1`)
- **Secondary**: Purple (`#8B5CF6`)
- **Tertiary**: Pink (`#EC4899`)
- **Success**: Green
- **Warning**: Orange
- **Error**: Red

## 🚀 Quick Start

### iOS
```bash
cd mobile/ios
open PrimeCare Software.xcodeproj
# Configure API URL in NetworkManager.swift
# Build and Run (⌘R)
```

### Android
```bash
cd mobile/android
# Abra no Android Studio
# Configure API URL em build.gradle.kts
# Sync Gradle e Run (▶️)
```

## 📡 API Integration

Ambos os apps se conectam à mesma API backend:

**Development**:
- iOS (Simulator): `http://localhost:5000/api`
- iOS (Device): `http://<YOUR_IP>:5000/api`
- Android (Emulator): `http://10.0.2.2:5000/api`
- Android (Device): `http://<YOUR_IP>:5000/api`

**Production**:
- Ambos: `https://api.medicwarehouse.com/api`

### Endpoints Consumidos

```
Authentication:
  POST /api/auth/login
  POST /api/auth/owner-login

Dashboard:
  GET /api/appointments/agenda
  GET /api/patients

Patients:
  GET /api/patients
  GET /api/patients/{id}
  GET /api/patients/search

Appointments:
  GET /api/appointments
  GET /api/appointments/{id}
  GET /api/appointments/agenda
```

## 🧪 Testes

### iOS
```bash
# No Xcode, pressione ⌘U
# Ou: Product → Test
```

### Android
```bash
# Testes unitários
./gradlew test

# Testes instrumentados
./gradlew connectedAndroidTest
```

## 📦 Build para Produção

### iOS (App Store)
1. Configure código de assinatura no Xcode
2. Archive: Product → Archive
3. Distribute App → App Store Connect

### Android (Play Store)
```bash
# App Bundle (recomendado)
./gradlew bundleRelease

# APK
./gradlew assembleRelease
```

## 🔄 Sincronização de Dados

Os apps são **stateless** e sempre buscam dados frescos do servidor:
- Sem banco de dados local (por enquanto)
- Pull to refresh em todas as listas
- Loading states apropriados
- Error handling robusto

**Futuro**: Implementação de cache local e modo offline.

## 📊 Status do Desenvolvimento

| Feature | iOS | Android | Notas |
|---------|-----|---------|-------|
| Autenticação | ✅ | ✅ | JWT completo |
| Dashboard | ✅ | ✅ | Stats em tempo real |
| Listagem Pacientes | ✅ | 🚧 | iOS completo |
| Listagem Agendamentos | ✅ | 🚧 | iOS completo |
| Criar Paciente | 🚧 | 🚧 | Planejado |
| Criar Agendamento | 🚧 | 🚧 | Planejado |
| Prontuários | 🚧 | 🚧 | Planejado |
| Notificações Push | 🚧 | 🚧 | Planejado |
| Modo Offline | 🚧 | 🚧 | Planejado |
| Biometria | 🚧 | 🚧 | Planejado |

## 🐛 Troubleshooting

### Não consigo conectar à API

**iOS Simulator + Mac**:
- Use `http://localhost:5000/api`

**iOS Device + Mac**:
- Use o IP da sua máquina: `http://192.168.1.XXX:5000/api`
- Certifique-se de estar na mesma rede Wi-Fi

**Android Emulator + PC**:
- Use `http://10.0.2.2:5000/api` (IP especial do emulador)

**Android Device + PC**:
- Use o IP da sua máquina: `http://192.168.1.XXX:5000/api`
- Certifique-se de estar na mesma rede Wi-Fi
- Verifique o firewall do PC

### Erro de certificado SSL

Para desenvolvimento, ambos os apps aceitam HTTP (não seguro).
Em produção, sempre use HTTPS.

## 📝 Credenciais de Teste

```
Usuário: admin
Senha: Admin@123
Tenant ID: demo-clinic-001
```

Ou use o endpoint de seeder para criar dados de teste:
```bash
POST http://localhost:5000/api/data-seeder/seed-demo
```

## 🚀 Próximas Features

### Curto Prazo
- [ ] Finalizar telas de pacientes e agendamentos no Android
- [ ] Implementar criação/edição de pacientes
- [ ] Implementar criação/edição de agendamentos
- [ ] Adicionar visualização de prontuários

### Médio Prazo
- [ ] Notificações push (Firebase/APNs)
- [ ] Modo offline com cache local
- [ ] Biometria para login (Face ID/Touch ID/Fingerprint)
- [ ] Upload de fotos/documentos
- [ ] Assinatura digital de prescrições

### Longo Prazo
- [ ] Telemedicina integrada nos apps
- [ ] Chat em tempo real
- [ ] Widgets (iOS/Android)
- [ ] Apple Watch app
- [ ] Wear OS app
- [ ] Suporte multi-idioma completo

## 📄 Licença

Este projeto está sob a mesma licença do projeto principal PrimeCare Software.

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📞 Suporte

- **Documentação iOS**: [ios/README.md](ios/README.md)
- **Documentação Android**: [android/README.md](android/README.md)
- **Issues**: GitHub Issues
- **Email**: contato@medicwarehouse.com

---

**Desenvolvido com ❤️ para a comunidade médica brasileira**
