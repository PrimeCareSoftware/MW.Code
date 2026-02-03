# Guia Completo dos Aplicativos Móveis Omni Care Software

Este documento fornece uma visão geral completa dos aplicativos móveis nativos do Omni Care Software para iOS e Android.

## 📱 Visão Geral

O Omni Care Software agora oferece aplicativos móveis nativos que complementam perfeitamente o sistema web, permitindo que médicos, secretárias e proprietários de clínicas gerenciem suas operações em qualquer lugar.

### Plataformas Disponíveis

| Plataforma | Tecnologia | Versão Mínima | Status |
|------------|-----------|---------------|--------|
| **iOS** | Swift 5.9 + SwiftUI | iOS 17.0+ | ✅ Beta |
| **Android** | Kotlin + Jetpack Compose | Android 7.0+ (API 24) | ✅ Beta |

## 🎯 Objetivos dos Apps Mobile

1. **Mobilidade**: Acesso ao sistema em qualquer lugar
2. **Performance**: Apps nativos para melhor experiência
3. **Offline-first**: Planejado para próximas versões
4. **Notificações**: Push notifications para lembretes e alertas
5. **UX Nativa**: Seguir guidelines de cada plataforma

## 🏗️ Arquitetura Técnica

### Arquitetura Geral

```
┌─────────────────────────────────────────────────────┐
│                  Backend API (.NET 8)                │
│              https://api.medicwarehouse.com          │
└─────────────────┬───────────────────────────────────┘
                  │
                  │ REST API (JWT Auth)
                  │
        ┌─────────┴──────────┐
        │                    │
┌───────▼────────┐  ┌────────▼────────┐
│   iOS App      │  │  Android App    │
│   (SwiftUI)    │  │  (Jetpack       │
│                │  │   Compose)      │
└────────────────┘  └─────────────────┘
```

### iOS - MVVM Architecture

```
┌────────────────────────────────────────────┐
│              SwiftUI Views                 │
│  (LoginView, DashboardView, etc.)         │
└──────────────┬─────────────────────────────┘
               │ @Published / @StateObject
               ▼
┌────────────────────────────────────────────┐
│            ViewModels                      │
│  (AuthViewModel, DashboardViewModel)       │
└──────────────┬─────────────────────────────┘
               │ async/await
               ▼
┌────────────────────────────────────────────┐
│          APIService Layer                  │
│   (APIService, NetworkManager)             │
└──────────────┬─────────────────────────────┘
               │ URLSession
               ▼
┌────────────────────────────────────────────┐
│          Backend API                       │
└────────────────────────────────────────────┘
```

### Android - Clean Architecture + MVVM

```
┌────────────────────────────────────────────┐
│          Jetpack Compose UI                │
│  (LoginScreen, DashboardScreen, etc.)      │
└──────────────┬─────────────────────────────┘
               │ StateFlow / collectAsState
               ▼
┌────────────────────────────────────────────┐
│            ViewModels (Hilt)               │
│  (AuthViewModel, DashboardViewModel)       │
└──────────────┬─────────────────────────────┘
               │ Coroutines
               ▼
┌────────────────────────────────────────────┐
│            Repository Layer                │
│     (Single source of truth)               │
└──────────────┬─────────────────────────────┘
               │ Retrofit
               ▼
┌────────────────────────────────────────────┐
│          API Service                       │
│   (Retrofit + OkHttp + Gson)              │
└──────────────┬─────────────────────────────┘
               │ HTTP/REST
               ▼
┌────────────────────────────────────────────┐
│          Backend API                       │
└────────────────────────────────────────────┘
```

## 🔐 Autenticação e Segurança

### Fluxo de Autenticação

1. **Usuário insere credenciais** (username, password, tenantId)
2. **App faz POST** para `/api/auth/login` ou `/api/auth/owner-login`
3. **Backend retorna JWT token** + informações do usuário
4. **Token é armazenado** de forma segura:
   - iOS: UserDefaults (Keychain em produção)
   - Android: DataStore com encriptação
5. **Token é incluído** em todas as requisições subsequentes no header `Authorization: Bearer <token>`
6. **Token expira** após 60 minutos (configurável)

### Segurança Implementada

- ✅ **JWT Tokens**: Autenticação stateless
- ✅ **HTTPS**: Comunicação criptografada (produção)
- ✅ **Token Refresh**: Refresh automático planejado
- ✅ **Secure Storage**: Tokens armazenados de forma segura
- 🚧 **Certificate Pinning**: Planejado
- 🚧 **Biometric Auth**: Face ID/Touch ID planejado

## 📊 Funcionalidades Implementadas

### ✅ Autenticação
- Login de usuários regulares (médicos, secretárias)
- Login de proprietários de clínicas
- Logout com limpeza de sessão
- Persistência de sessão entre aberturas do app

### ✅ Dashboard
- Estatísticas em tempo real:
  - Consultas agendadas para hoje
  - Total de pacientes cadastrados
  - Consultas pendentes
  - Consultas concluídas hoje
- Ações rápidas para navegação
- Pull to refresh

### ✅ Pacientes (iOS Completo)
- Listagem com paginação
- Busca por nome, CPF ou telefone
- Visualização de detalhes
- Pull to refresh
- Avatar com inicial do nome

### ✅ Agendamentos (iOS Completo)
- Listagem com paginação
- Filtros por status:
  - Todos
  - Agendados
  - Em Andamento
  - Concluídos
- Visualização de detalhes
- Pull to refresh
- Status visual com cores

### ✅ Perfil
- Informações do usuário logado
- Role e tenant
- Logout

## 🚧 Funcionalidades em Desenvolvimento

### Próxima Sprint
- [ ] Completar telas de pacientes no Android
- [ ] Completar telas de agendamentos no Android
- [ ] Criar/editar pacientes
- [ ] Criar/editar agendamentos

### Roadmap 2024 Q4
- [ ] Visualização de prontuários médicos
- [ ] Prescrições médicas
- [ ] Upload de documentos/fotos
- [ ] Notificações push (Firebase/APNs)
- [ ] Modo offline com cache local

### Roadmap 2025 Q1
- [ ] Telemedicina integrada
- [ ] Chat em tempo real
- [ ] Biometria para login
- [ ] Assinatura digital
- [ ] Widgets (iOS/Android)

## 🔧 Configuração de Desenvolvimento

### Pré-requisitos

**Para iOS:**
- macOS Monterey (12.0) ou superior
- Xcode 15.0 ou superior
- Simulador iOS ou dispositivo físico

**Para Android:**
- Android Studio Hedgehog (2023.1.1) ou superior
- JDK 17 ou superior
- Emulador Android ou dispositivo físico

### Setup do Backend

Antes de executar os apps, certifique-se de que a API está rodando:

```bash
# Na raiz do projeto
cd src/MedicSoft.Api
dotnet run

# A API estará disponível em:
# https://localhost:7107 (desenvolvimento)
# http://localhost:5000 (container)
```

### Setup iOS

```bash
cd mobile/ios
open Omni Care Software.xcodeproj

# No Xcode:
# 1. Selecione um simulador ou dispositivo
# 2. Pressione ⌘R para build e executar
```

**Configurar API URL** (em `NetworkManager.swift`):
```swift
// Simulador iOS
private let baseURL = "http://localhost:5000/api"

// Dispositivo físico (use o IP da sua máquina)
private let baseURL = "http://192.168.1.100:5000/api"
```

### Setup Android

```bash
cd mobile/android
# Abra no Android Studio

# No Android Studio:
# 1. Sync Gradle (se necessário)
# 2. Selecione um emulador ou dispositivo
# 3. Clique em Run (▶️)
```

**Configurar API URL** (em `app/build.gradle.kts`):
```kotlin
// Emulador Android
buildConfigField("String", "API_BASE_URL", "\"http://10.0.2.2:5000/api\"")

// Dispositivo físico (use o IP da sua máquina)
buildConfigField("String", "API_BASE_URL", "\"http://192.168.1.100:5000/api\"")
```

## 🧪 Testes

### Credenciais de Teste

```
Username: admin
Password: Admin@123
Tenant ID: demo-clinic-001
```

Ou crie dados de demonstração:
```bash
POST http://localhost:5000/api/data-seeder/seed-demo
```

### Testando Funcionalidades

1. **Login**: Teste com usuário regular e proprietário
2. **Dashboard**: Verifique se as estatísticas carregam corretamente
3. **Pacientes**: Teste busca, paginação e detalhes
4. **Agendamentos**: Teste filtros e visualização
5. **Logout**: Confirme que a sessão é limpa

## 📱 Design System

### Paleta de Cores

Ambos os apps utilizam a mesma paleta:

```
Primary:   #6366F1 (Indigo)
Secondary: #8B5CF6 (Purple)
Tertiary:  #EC4899 (Pink)
Success:   #34C759 (Green)
Warning:   #FF9500 (Orange)
Error:     #EF4444 (Red)
```

### Tipografia

**iOS**: San Francisco (sistema padrão)
**Android**: Roboto (sistema padrão)

### Componentes

Ambos os apps utilizam componentes nativos de suas plataformas:
- **iOS**: SF Symbols, SwiftUI native components
- **Android**: Material Icons, Material 3 components

## 🚀 Deploy e Distribuição

### iOS - TestFlight & App Store

1. **Configurar assinatura** no Xcode
2. **Archive**: Product → Archive
3. **Upload para TestFlight**: Distribute App → TestFlight
4. **Beta Testing**: Convide testadores
5. **Submit para App Store**: Após aprovação dos testes

### Android - Play Store

1. **Gerar App Bundle**:
   ```bash
   ./gradlew bundleRelease
   ```

2. **Assinar o bundle** com sua chave de produção

3. **Upload para Play Console**: 
   - Internal Testing → Alpha → Beta → Production

4. **Beta Testing**: Use tracks do Play Console

## 📊 Métricas e Analytics

### Planejado para implementação:

- **Firebase Analytics**: Rastreamento de eventos
- **Firebase Crashlytics**: Relatórios de crashes
- **Custom Events**:
  - Login success/failure
  - Screen views
  - API call timing
  - Feature usage

## 🐛 Troubleshooting Comum

### Problema: Não consigo conectar à API

**Solução**:
- Verifique se a API está rodando
- Confirme o IP correto para dispositivo físico
- iOS Simulator: use `localhost`
- Android Emulator: use `10.0.2.2`
- Dispositivos físicos: use IP da máquina na rede local
- Verifique firewall da máquina host

### Problema: Erro de certificado SSL

**Solução**:
- Para desenvolvimento, HTTP está permitido
- Verifique configurações no Info.plist (iOS) ou AndroidManifest.xml (Android)
- Em produção, sempre use HTTPS

### Problema: Token expira muito rápido

**Solução**:
- Token JWT expira em 60 minutos por padrão
- Implementar refresh token (planejado)
- Por enquanto, faça login novamente

## 📚 Recursos Adicionais

### Documentação
- [README iOS](mobile/ios/README.md)
- [README Android](mobile/android/README.md)
- [README Mobile Geral](mobile/README.md)

### API
- [Documentação da API](../README.md)
- [Swagger UI](http://localhost:5000/swagger)
- [Postman Collection](../Omni Care Software-Postman-Collection.json)

### Desenvolvimento
- [Guia de Desenvolvimento Auth](GUIA_DESENVOLVIMENTO_AUTH.md)
- [Autenticação](AUTHENTICATION_GUIDE.md)
- [Guia Multiplataforma](GUIA_MULTIPLATAFORMA.md)

## 🤝 Contribuindo

Interessado em contribuir para os apps móveis?

1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit suas mudanças: `git commit -m 'Adiciona nova funcionalidade'`
4. Push para a branch: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

### Áreas que precisam de contribuições:
- 📱 Testes unitários e de UI
- 🎨 Melhorias de UX/UI
- 🌍 Internacionalização (i18n)
- 📚 Documentação e tutoriais
- 🐛 Bug fixes e melhorias de performance

## 📞 Suporte

- **GitHub Issues**: Para bugs e feature requests
- **Email**: contato@omnicaresoftware.com
- **Documentação**: Consulte os READMEs específicos

---

**Desenvolvido com ❤️ para levar mobilidade à gestão médica**
