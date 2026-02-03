# Guia de Integração Frontend-Backend

Este documento descreve como o frontend Angular se integra com a API backend do Portal do Paciente.

## 🔗 Configuração da API

### Base URL

A URL base da API é configurada em `src/environments/`:

**Desenvolvimento** (`environment.ts`):
```typescript
apiUrl: 'http://localhost:5000/api'
```

**Produção** (`environment.prod.ts`):
```typescript
apiUrl: '/api'
```

## 🔐 Autenticação

### Fluxo de Autenticação

1. **Login**
   - Endpoint: `POST /api/auth/login`
   - Body: `{ emailOrCPF: string, password: string }`
   - Response: `{ accessToken, refreshToken, expiresAt, user }`

2. **Registro**
   - Endpoint: `POST /api/auth/register`
   - Body: `{ fullName, email, cpf, phoneNumber, dateOfBirth, password, confirmPassword }`
   - Response: `{ accessToken, refreshToken, expiresAt, user }`

3. **Refresh Token**
   - Endpoint: `POST /api/auth/refresh`
   - Body: `{ refreshToken: string }`
   - Response: `{ accessToken, refreshToken, expiresAt, user }`

4. **Logout**
   - Endpoint: `POST /api/auth/logout`
   - Header: `Authorization: Bearer {token}`
   - Body: `{ refreshToken: string }`

5. **Alterar Senha**
   - Endpoint: `POST /api/auth/change-password`
   - Header: `Authorization: Bearer {token}`
   - Body: `{ currentPassword: string, newPassword: string }`

### Interceptor JWT

O `AuthInterceptor` automaticamente:
- Adiciona o header `Authorization: Bearer {token}` em todas as requisições
- Intercepta erros 401 e tenta renovar o token
- Redireciona para login se a renovação falhar

```typescript
// Configurado em app-module.ts
provideHttpClient(withInterceptors([authInterceptor]))
```

### Auth Guard

O `authGuard` protege rotas autenticadas:

```typescript
{
  path: 'dashboard',
  loadComponent: () => import('./pages/dashboard/dashboard.component'),
  canActivate: [authGuard]  // Requer autenticação
}
```

## 📅 Endpoints de Consultas

### Listar Minhas Consultas
```
GET /api/appointments?skip=0&take=50
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": "guid",
    "doctorName": "string",
    "doctorSpecialty": "string",
    "clinicName": "string",
    "appointmentDate": "2026-01-15T00:00:00Z",
    "startTime": "10:00:00",
    "endTime": "11:00:00",
    "status": "Scheduled",
    "appointmentType": "Consulta",
    "isTelehealth": false,
    "telehealthLink": null,
    "notes": "string",
    "canReschedule": true,
    "canCancel": true
  }
]
```

### Consultas Próximas
```
GET /api/appointments/upcoming?take=10
Authorization: Bearer {token}
```

### Consulta por ID
```
GET /api/appointments/{id}
Authorization: Bearer {token}
```

### Consultas por Status
```
GET /api/appointments/status/{status}?skip=0&take=50
Authorization: Bearer {token}
```

Status válidos: `Scheduled`, `Confirmed`, `InProgress`, `Completed`, `Cancelled`, `NoShow`

### Contagem de Consultas
```
GET /api/appointments/count
Authorization: Bearer {token}
```

## 📄 Endpoints de Documentos

### Listar Meus Documentos
```
GET /api/documents?skip=0&take=50
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": "guid",
    "title": "string",
    "documentType": "Prescription",
    "description": "string",
    "doctorName": "string",
    "issuedDate": "2026-01-10T00:00:00Z",
    "fileUrl": "string",
    "fileName": "document.pdf",
    "fileSizeFormatted": "250 KB",
    "isAvailable": true
  }
]
```

### Documentos Recentes
```
GET /api/documents/recent?take=5
Authorization: Bearer {token}
```

### Documento por ID
```
GET /api/documents/{id}
Authorization: Bearer {token}
```

### Documentos por Tipo
```
GET /api/documents/type/{type}?skip=0&take=50
Authorization: Bearer {token}
```

Tipos válidos: `Prescription`, `MedicalCertificate`, `LabReport`, `ImagingReport`, `MedicalReport`, `Other`

### Contagem de Documentos
```
GET /api/documents/count
Authorization: Bearer {token}
```

### Download de Documento
```
GET /api/documents/{id}/download
Authorization: Bearer {token}
Response: Binary (application/pdf)
```

## 🔄 Tratamento de Erros

### Códigos de Status

- **200 OK** - Requisição bem-sucedida
- **400 Bad Request** - Dados inválidos
- **401 Unauthorized** - Token inválido ou expirado
- **403 Forbidden** - Sem permissão
- **404 Not Found** - Recurso não encontrado
- **500 Internal Server Error** - Erro no servidor

### Exemplo de Tratamento

```typescript
this.authService.login(request).subscribe({
  next: (response) => {
    // Sucesso
    this.router.navigate(['/dashboard']);
  },
  error: (error) => {
    if (error.status === 401) {
      this.errorMessage = 'Credenciais inválidas';
    } else if (error.status === 500) {
      this.errorMessage = 'Erro no servidor';
    } else {
      this.errorMessage = error.error?.message || 'Erro desconhecido';
    }
  }
});
```

## 🧪 Testando a Integração

### Usando o Frontend

1. Inicie o backend:
```bash
cd patient-portal-api
dotnet run --project PatientPortal.Api
```

2. Inicie o frontend:
```bash
cd frontend/patient-portal
npm start
```

3. Acesse `http://localhost:4200`

### Usando Postman

Importe a collection do Omni Care Software:
- Arquivo: `Omni Care Software-Postman-Collection.json`
- Configure a variável `{{baseUrl}}` para `http://localhost:5000`

### CORS

O backend deve estar configurado para aceitar requisições do frontend:

```csharp
// Em Program.cs ou Startup.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

## 📝 Modelos TypeScript

Os modelos TypeScript no frontend correspondem aos DTOs do backend:

### Auth Models (`src/app/models/auth.model.ts`)
- `User` → `PatientUserDto`
- `LoginRequest` → `LoginRequestDto`
- `LoginResponse` → `LoginResponseDto`
- `RegisterRequest` → `RegisterRequestDto`

### Appointment Model (`src/app/models/appointment.model.ts`)
- `Appointment` → `AppointmentDto`

### Document Model (`src/app/models/document.model.ts`)
- `Document` → `DocumentDto`

## 🔍 Debug

### Ver requisições HTTP

Abra o Console do navegador (F12) → Network tab para ver todas as requisições HTTP.

### Ver tokens

```typescript
// No console do navegador
localStorage.getItem('access_token')
localStorage.getItem('refresh_token')
localStorage.getItem('current_user')
```

### Limpar cache

```typescript
// No console do navegador
localStorage.clear()
```

## 🚀 Próximos Passos

- [ ] Implementar PWA para funcionar offline
- [ ] Adicionar testes de integração E2E
- [ ] Implementar cache de dados
- [ ] Adicionar indicadores de loading
- [ ] Melhorar tratamento de erros
- [ ] Adicionar notificações push
