# API Endpoint Guide

## Common Issues and Solutions

### 404 Not Found Errors with `/api/api/` Prefix

**Problem**: Endpoints returning 404 when accessed with doubled `/api/` prefix.

**Examples of INCORRECT URLs**:
- ❌ `http://localhost:5293/api/api/module-config/info`
- ❌ `http://localhost:5293/api/api/system-admin/modules/usage`
- ❌ `http://localhost:5293/api/api/system-admin/modules/adoption`

**Correct URLs**:
- ✅ `http://localhost:5293/api/module-config/info`
- ✅ `http://localhost:5293/api/system-admin/modules/usage`
- ✅ `http://localhost:5293/api/system-admin/modules/adoption`

**Root Cause**: Client code is incorrectly adding an extra `/api/` prefix when constructing URLs. The server-side controllers are correctly configured with their routes.

**Solution**: Update client-side URL construction to avoid adding duplicate prefixes. If using a base URL configuration, ensure it doesn't include `/api/` if the route already includes it.

---

### 400 Bad Request on Enum Values

**Problem**: Endpoints accepting enums return 400 with error: "The JSON value could not be converted to [EnumType]"

**Example**:
```json
POST /api/system-admin/clinic-management/filter
{
  "page": 1,
  "pageSize": 20,
  "healthStatus": "AtRisk"  // String value for enum
}
```

**Error**:
```json
{
  "errors": {
    "$.healthStatus": [
      "The JSON value could not be converted to System.Nullable`1[HealthStatus]"
    ]
  }
}
```

**Solution**: 
- **Server-side** (✅ Already implemented): Added `JsonStringEnumConverter` to support string-to-enum conversion
- **Client-side**: Use string values matching the enum names exactly (case-insensitive matching is enabled)

**Valid Enum Values for `HealthStatus`**:
- `"Healthy"` or `0`
- `"NeedsAttention"` or `1`
- `"AtRisk"` or `2`

---

## Module Configuration Endpoints

### Base Route: `/api/module-config`

#### Get Module Information
```http
GET /api/module-config/info
```
**Returns**: Detailed information about all available modules including display name, description, category, and requirements.

**Response Example**:
```json
[
  {
    "name": "PatientManagement",
    "displayName": "Gestão de Pacientes",
    "description": "Core patient management module",
    "category": "Core",
    "icon": "people",
    "isCore": true,
    "requiredModules": [],
    "minimumPlan": "Basic"
  }
]
```

#### Get Available Modules
```http
GET /api/module-config/available
```
**Returns**: Simple list of all module names.

---

## System Admin Module Endpoints

### Base Route: `/api/system-admin/modules`

**Authorization**: Requires `SystemAdmin` role

#### Get Module Usage Statistics
```http
GET /api/system-admin/modules/usage
```
**Returns**: Global usage statistics for all modules across all clinics.

```json
[
  {
    "moduleName": "PatientManagement",
    "clinicsUsing": 148,
    "totalClinics": 150,
    "usagePercentage": 98.67
  }
]
```

#### Get Module Adoption Rates
```http
GET /api/system-admin/modules/adoption
```
**Returns**: Adoption rates calculated as percentage of clinics using each module.

```json
[
  {
    "moduleName": "PatientManagement",
    "adoptionRate": 98.5,
    "totalClinics": 150,
    "clinicsUsing": 148
  }
]
```

#### Get Usage by Plan
```http
GET /api/system-admin/modules/usage-by-plan
```
**Returns**: Module usage broken down by subscription plan type.

---

## Clinic Management Endpoints

### Base Route: `/api/system-admin/clinic-management`

**Authorization**: Requires `SystemAdmin` role

#### Filter Clinics
```http
POST /api/system-admin/clinic-management/filter
Content-Type: application/json

{
  "searchTerm": "Clínica",
  "isActive": true,
  "tags": ["vip", "enterprise"],
  "healthStatus": "AtRisk",        // Enum: Healthy, NeedsAttention, AtRisk
  "subscriptionStatus": "Active",
  "createdAfter": "2024-01-01",
  "createdBefore": "2024-12-31",
  "page": 1,
  "pageSize": 20,
  "sortBy": "name",                // Options: name, createdAt, lastActivity
  "sortDescending": false
}
```

**Response**:
```json
{
  "data": [...],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

**Available HealthStatus Values**:
- `"Healthy"` (0): Clinic is performing well
- `"NeedsAttention"` (1): Some metrics need improvement
- `"AtRisk"` (2): Critical issues requiring immediate attention

**Notes**:
- All filter parameters are optional
- Property names are case-insensitive (`healthStatus`, `HealthStatus`, and `HEALTHSTATUS` all work)
- Enum values can be provided as strings or numeric values

#### Get Clinic by Segment
```http
GET /api/system-admin/clinic-management/segment/{segment}
```

**Valid Segments**:
- `new`: Clinics created in the last 30 days
- `trial`: Clinics on trial subscriptions
- `at-risk`: Clinics with health status = AtRisk
- `needs-attention`: Clinics with health status = NeedsAttention
- `healthy`: Clinics with health status = Healthy
- `inactive`: Inactive clinics

---

## Best Practices

### 1. URL Construction
```javascript
// ✅ Good: Use a base URL without /api suffix
const baseUrl = "http://localhost:5293";
const endpoint = "/api/module-config/info";
const fullUrl = `${baseUrl}${endpoint}`;

// ❌ Bad: Double /api prefix
const baseUrl = "http://localhost:5293/api";
const endpoint = "/api/module-config/info";  // Already includes /api!
const fullUrl = `${baseUrl}${endpoint}`;     // Results in /api/api/
```

### 2. Enum Handling
```javascript
// ✅ Good: Use string enum values
const filter = {
  healthStatus: "AtRisk",  // String representation
  page: 1,
  pageSize: 20
};

// ✅ Also valid: Use numeric enum values
const filter = {
  healthStatus: 2,  // 2 = AtRisk
  page: 1,
  pageSize: 20
};

// ❌ Bad: Incorrect enum value with separator
const filter = {
  healthStatus: "at-risk",  // Should be "AtRisk" (no dash separator)
  page: 1,
  pageSize: 20
};

// ✅ Also valid: Case variations work
const filter = {
  healthStatus: "atrisk",  // Case-insensitive matching
  page: 1,
  pageSize: 20
};
```

### 3. Case Sensitivity
JSON property names are case-insensitive, so all of these work:
```json
{"healthStatus": "AtRisk"}
{"HealthStatus": "AtRisk"}
{"HEALTHSTATUS": "AtRisk"}
```

**Enum values** are also case-insensitive with the current configuration:
- ✅ `"AtRisk"`
- ✅ `"atrisk"` (case-insensitive matching enabled)
- ✅ `"ATRISK"`
- ❌ `"AT_RISK"` (underscore not part of enum name)

---

## Troubleshooting

### Issue: "The filters field is required"
**Cause**: Model binding failed, usually due to enum conversion errors.
**Solution**: Check that enum values are spelled correctly and match the enum definition.

### Issue: 404 Not Found
**Cause**: URL includes double `/api/` prefix or incorrect route.
**Solution**: 
1. Verify the URL doesn't have `/api/api/`
2. Check the controller's `[Route]` attribute in the source code
3. Use Swagger UI to see the correct endpoint paths

### Issue: 401 Unauthorized
**Cause**: Missing or invalid JWT token, or insufficient permissions.
**Solution**: 
1. Ensure JWT token is included in the `Authorization` header
2. Verify the token hasn't expired
3. Check that the user has the required role (e.g., `SystemAdmin`)

---

## Testing Endpoints

### Using cURL

```bash
# Get module info (no auth required)
curl http://localhost:5293/api/module-config/info

# Get module usage (requires SystemAdmin role)
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  http://localhost:5293/api/system-admin/modules/usage

# Filter clinics
curl -X POST \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"healthStatus":"AtRisk","page":1,"pageSize":20}' \
  http://localhost:5293/api/system-admin/clinic-management/filter
```

### Using Swagger UI

Navigate to: `http://localhost:5293/swagger`

All endpoints are documented with:
- Required/optional parameters
- Request/response schemas
- Example values
- Authorization requirements

---

## Migration Guide

If you have existing code calling endpoints with `/api/api/` prefix:

### JavaScript/TypeScript
```javascript
// Before (WRONG)
const response = await fetch('http://localhost:5293/api/api/module-config/info');

// After (CORRECT)
const response = await fetch('http://localhost:5293/api/module-config/info');
```

### C# HttpClient
```csharp
// Before (WRONG)
var response = await httpClient.GetAsync("/api/api/module-config/info");

// After (CORRECT)
var response = await httpClient.GetAsync("/api/module-config/info");
```

### Angular HttpClient
```typescript
// Before (WRONG)
this.http.get(`${environment.apiUrl}/api/module-config/info`)

// After (CORRECT) - if environment.apiUrl already includes /api
this.http.get(`${environment.apiUrl}/module-config/info`)

// After (CORRECT) - if environment.apiUrl is just the base URL
this.http.get(`${environment.apiUrl}/api/module-config/info`)
```

---

## Additional Resources

- [ASP.NET Core Routing](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing)
- [System.Text.Json Enum Conversion](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties#enums-as-strings)
- Project Swagger Documentation: `http://localhost:5293/swagger`

---

## Clinic Admin Endpoints (Usuários, Permissões e Campos Profissionais)

### Base Route: `/api/ClinicAdmin`

**Authorization**: usuário autenticado com permissões de clínica (owner/admin conforme política de autorização).

### Usuários da clínica

#### Listar usuários
```http
GET /api/ClinicAdmin/users
```

#### Criar usuário
```http
POST /api/ClinicAdmin/users
Content-Type: application/json

{
  "username": "dr.joao",
  "password": "SenhaForte123!",
  "name": "Dr. João Silva",
  "email": "joao@clinicax.com",
  "role": "Dentista",
  "phone": "(11) 99999-0000",
  "professionalId": "CRO-12345",
  "specialty": "Ortodontia",
  "showInAppointmentScheduling": true
}
```

**Observações**:
- `role` aceita aliases em português e inglês para perfis clínicos e administrativos.
- Para profissionais com atendimento habilitado (`showInAppointmentScheduling = true`), o backend pode exigir `professionalId` conforme configuração da clínica.

#### Atualizar usuário
```http
PUT /api/ClinicAdmin/users/{id}
Content-Type: application/json

{
  "name": "Dra. Maria Souza",
  "email": "maria@clinicax.com",
  "professionalId": "CRM-54321",
  "specialty": "Cardiologia",
  "showInAppointmentScheduling": true,
  "isActive": true
}
```

#### Alterar senha
```http
PUT /api/ClinicAdmin/users/{id}/password
Content-Type: application/json

{
  "newPassword": "SenhaNova123!"
}
```

#### Alterar perfil/função
```http
PUT /api/ClinicAdmin/users/{id}/role
Content-Type: application/json

{
  "newRole": "Recepcionista"
}
```

### Configuração dinâmica de campos profissionais

#### Obter configuração
```http
GET /api/ClinicAdmin/doctor-fields-config
```

**Response Example**:
```json
{
  "professionalIdRequired": true,
  "specialtyRequired": false
}
```

#### Atualizar configuração
```http
PUT /api/ClinicAdmin/doctor-fields-config
Content-Type: application/json

{
  "professionalIdRequired": true,
  "specialtyRequired": true
}
```

Esses endpoints suportam o comportamento dinâmico do frontend no cadastro/edição de profissionais de saúde.
