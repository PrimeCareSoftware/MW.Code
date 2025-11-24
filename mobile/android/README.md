# MedicWarehouse Android App

Aplicativo Android nativo para o sistema MedicWarehouse, desenvolvido com Kotlin, Jetpack Compose e integrando com a API RESTful do backend.

## 📱 Características

- **Jetpack Compose**: UI moderna e declarativa
- **Kotlin**: 100% Kotlin com coroutines
- **MVVM + Clean Architecture**: Arquitetura escalável e testável
- **Hilt**: Injeção de dependências
- **Retrofit**: Cliente HTTP type-safe
- **Material Design 3**: Design system moderno do Google
- **DataStore**: Armazenamento seguro de preferências
- **JWT Authentication**: Autenticação segura com tokens

## 🎨 Funcionalidades

### Autenticação
- ✅ Login de usuários (médicos, secretárias, etc.)
- ✅ Login de proprietários de clínicas
- ✅ Logout seguro
- ✅ Persistência de token JWT com DataStore

### Dashboard
- ✅ Estatísticas em tempo real
- ✅ Consultas do dia
- ✅ Total de pacientes
- ✅ Consultas pendentes e concluídas
- ✅ Ações rápidas

### Próximas Funcionalidades
- 🚧 Listagem e busca de pacientes
- 🚧 Listagem e filtros de agendamentos
- 🚧 Criação/edição de pacientes
- 🚧 Criação/edição de agendamentos
- 🚧 Visualização de prontuários

## 🛠️ Requisitos

- **Android Studio**: Hedgehog (2023.1.1) ou superior
- **JDK**: 17 ou superior
- **Android SDK**: API 34
- **Min SDK**: API 24 (Android 7.0)
- **Gradle**: 8.2+

## 🚀 Como Executar

### 1. Abrir o Projeto no Android Studio

```bash
cd mobile/android
# Abra o Android Studio e selecione "Open an Existing Project"
# Navegue até a pasta mobile/android
```

### 2. Configurar a API Base URL

O app está configurado para usar URLs diferentes em debug e release:

**Debug** (desenvolvimento local):
```kotlin
// Em app/build.gradle.kts
buildConfigField("String", "API_BASE_URL", "\"http://10.0.2.2:5000/api\"")
```

**Release** (produção):
```kotlin
buildConfigField("String", "API_BASE_URL", "\"https://api.medicwarehouse.com/api\"")
```

> **Nota**: `10.0.2.2` é o IP especial do emulador Android que aponta para o `localhost` da máquina host.

Para dispositivo físico, use o IP da sua máquina na rede local:
```kotlin
buildConfigField("String", "API_BASE_URL", "\"http://192.168.1.100:5000/api\"")
```

### 3. Sincronizar Gradle

No Android Studio:
- Clique em "Sync Project with Gradle Files" ou
- File → Sync Project with Gradle Files

### 4. Executar o App

- Selecione um emulador ou conecte um dispositivo físico
- Clique em "Run" (▶️) ou Shift+F10

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
app/src/main/
├── kotlin/com/medicwarehouse/app/
│   ├── MedicWarehouseApp.kt          # Application class com Hilt
│   ├── MainActivity.kt                # Activity principal
│   ├── data/                          # Camada de dados
│   │   ├── Models.kt                  # Data classes
│   │   └── Repository.kt              # Repository pattern
│   ├── network/                       # Camada de rede
│   │   ├── ApiService.kt              # Retrofit interface
│   │   ├── AuthInterceptor.kt         # Interceptor de autenticação
│   │   ├── TokenManager.kt            # Gerenciador de tokens
│   │   └── NetworkModule.kt           # Módulo Hilt para DI
│   ├── ui/                            # Camada de UI
│   │   ├── theme/                     # Material Design theme
│   │   │   ├── Theme.kt
│   │   │   └── Type.kt
│   │   ├── navigation/                # Navegação
│   │   │   └── NavGraph.kt
│   │   └── screens/                   # Telas Compose
│   │       ├── LoginScreen.kt
│   │       └── DashboardScreen.kt
│   └── viewmodel/                     # ViewModels
│       ├── AuthViewModel.kt
│       └── DashboardViewModel.kt
├── res/                               # Recursos Android
│   ├── values/
│   │   ├── strings.xml
│   │   └── themes.xml
│   └── xml/
│       ├── backup_rules.xml
│       └── data_extraction_rules.xml
└── AndroidManifest.xml
```

## 🏗️ Arquitetura

O app segue a arquitetura recomendada pelo Google:

- **UI Layer (Jetpack Compose)**: Screens e componentes UI
- **ViewModel Layer**: Lógica de apresentação e estados
- **Repository Layer**: Abstração de fonte de dados
- **Network Layer**: API calls com Retrofit
- **Data Layer**: Modelos e DTOs

### Fluxo de Dados

```
UI (Compose) → ViewModel → Repository → API Service → Backend API
                ↓
             StateFlow
                ↓
          UI (recompose)
```

## 📦 Dependências Principais

```kotlin
// Jetpack Compose
implementation("androidx.compose.ui:ui")
implementation("androidx.compose.material3:material3")

// Navigation
implementation("androidx.navigation:navigation-compose")

// Networking
implementation("com.squareup.retrofit2:retrofit")
implementation("com.squareup.retrofit2:converter-gson")

// Dependency Injection
implementation("com.google.dagger:hilt-android")

// DataStore
implementation("androidx.datastore:datastore-preferences")

// Coroutines
implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android")
```

## 🔐 Segurança

- **JWT Tokens**: Armazenados de forma segura no DataStore
- **HTTPS**: Configurado para produção
- **ProGuard**: Regras de ofuscação configuradas
- **Backup Rules**: Tokens excluídos de backup automático
- **Certificate Pinning**: Pronto para implementação

## 🎨 Design System

O app segue o Material Design 3 com:

- **Color Scheme**: 
  - Primary: Indigo (`#6366F1`)
  - Secondary: Purple (`#8B5CF6`)
  - Tertiary: Pink (`#EC4899`)
  - Error: Red (`#EF4444`)
- **Typography**: Material Design 3 type scale
- **Components**: Material 3 components (Cards, Buttons, etc.)
- **Dark Theme**: Suporte completo

## 🧪 Testes

Para executar os testes:

```bash
# Testes unitários
./gradlew test

# Testes instrumentados (requer emulador/dispositivo)
./gradlew connectedAndroidTest
```

## 📱 Compatibilidade

- **Min SDK**: Android 7.0 (API 24)
- **Target SDK**: Android 14 (API 34)
- **Arquiteturas**: ARM, ARM64, x86, x86_64

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

**Emulador Android**:
- Use `10.0.2.2` em vez de `localhost`
- Verifique se a API está rodando
- Certifique-se de que `usesCleartextTraffic="true"` está no AndroidManifest

**Dispositivo Físico**:
- Use o IP da sua máquina na rede local (ex: `192.168.1.100`)
- Certifique-se de que o dispositivo e a máquina estão na mesma rede
- Verifique o firewall da máquina

### Erro de Build

Se encontrar erros ao fazer sync do Gradle:
1. File → Invalidate Caches → Invalidate and Restart
2. Delete a pasta `.gradle` no projeto
3. Execute: `./gradlew clean build`

### Erro de certificado SSL

Para desenvolvimento local, `usesCleartextTraffic` está habilitado. Em produção, sempre use HTTPS.

## 🚀 Build de Release

Para gerar um APK de release:

```bash
./gradlew assembleRelease
```

O APK estará em: `app/build/outputs/apk/release/`

Para gerar um App Bundle (recomendado para Play Store):

```bash
./gradlew bundleRelease
```

O bundle estará em: `app/build/outputs/bundle/release/`

## 📋 Próximos Passos

- [ ] Implementar telas de pacientes e agendamentos
- [ ] Adicionar pull-to-refresh
- [ ] Implementar paginação infinita
- [ ] Adicionar notificações push (Firebase Cloud Messaging)
- [ ] Implementar modo offline com Room Database
- [ ] Adicionar testes unitários e instrumentados
- [ ] Implementar deep linking
- [ ] Adicionar analytics (Firebase Analytics)
- [ ] Implementar crash reporting (Firebase Crashlytics)
- [ ] Adicionar suporte multi-idioma (PT-BR, EN, ES)
- [ ] Implementar biometria para login
- [ ] Adicionar widget para agenda

## 📄 Licença

Este projeto está sob a mesma licença do projeto principal MedicWarehouse.

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação principal do projeto ou abra uma issue no GitHub.
