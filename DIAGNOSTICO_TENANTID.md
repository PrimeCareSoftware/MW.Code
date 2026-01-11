# Diagnóstico de Problemas com TenantId e Cadastro de Pacientes

## Problema Relatado
- Pacientes sendo cadastrados com `tenantId = "default-tenant"` ao invés do tenant correto
- Pacientes cadastrados pela API demo-seed (com `tenantId = "demo-clinic-001"`) não estão sendo consultados
- Inconsistência entre criação e consulta de registros

## Análise e Achados

### 1. **Problema Principal: BaseController.GetTenantId() [CORRIGIDO]**

**Local**: `MedicSoft.Api/Controllers/BaseController.cs` (linhas 20-25)

**Problema**:
- O método retornava `"default-tenant"` como fallback quando nenhum header `X-Tenant-Id` estava presente
- Isso causava que pacientes criados com um `TenantId` diferente (como `"demo-clinic-001"`) não aparecessem nas consultas

**Solução Implementada**:
```csharp
protected string GetTenantId()
{
    // 1. Tenta extrair do JWT claims (tenant_id)
    var tenantClaim = User?.FindFirst("tenant_id");
    if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
    {
        return tenantClaim.Value;
    }

    // 2. Tenta obter do header X-Tenant-Id (definido pelo middleware)
    var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
    if (!string.IsNullOrEmpty(tenantId))
    {
        return tenantId;
    }

    // 3. Tenta obter do HttpContext.Items (definido pelo middleware)
    if (HttpContext.Items.TryGetValue("TenantId", out var tenantIdObj) && tenantIdObj is string contextTenantId)
    {
        return contextTenantId;
    }

    // Fallback apenas se realmente indisponível
    return "default-tenant";
}
```

**Fluxo Correto**:
1. JWT token gerado no login inclui claim `"tenant_id"` com o tenant correto
2. Middleware `TenantResolutionMiddleware` resolve tenant pelo subdomain e define header `X-Tenant-Id`
3. Controllers agora têm múltiplas fontes para obter o TenantId correto

---

### 2. **Problema Secundário: GetAllAsync sem filtro IsActive [CORRIGIDO]**

**Local**: `MedicSoft.Repository/Repositories/PatientRepository.cs`

**Problema**:
- `PatientRepository.GetAllAsync()` não estava filtrando por `IsActive` enquanto `GetByClinicIdAsync()` filtrava
- Inconsistência entre métodos

**Solução Implementada**:
```csharp
public override async Task<IEnumerable<Patient>> GetAllAsync(string tenantId)
{
    return await _dbSet
        .Where(p => p.TenantId == tenantId && p.IsActive)
        .OrderBy(p => p.Name)
        .ToListAsync();
}
```

---

## Fluxo de Funcionamento Correto

### Cenário 1: Login de Usuário (via seed demo)
1. **Seed**: Usuários criados com `TenantId = "demo-clinic-001"`
   - Usuários: `admin`, `dr.silva`, `recep.maria`

2. **Login**: Endpoint `POST /api/auth/login`
   - Autentica usuário
   - `AuthController` chama `_authService.AuthenticateUserAsync(username, password, tenantId)`
   - JWT token gerado com claims incluindo `"tenant_id": "demo-clinic-001"`

3. **Requisição Subsequente**:
   - Token JWT enviado em header `Authorization: Bearer <token>`
   - `BaseController.GetTenantId()` extrai `tenant_id` do JWT
   - Retorna `"demo-clinic-001"`
   - Pacientes consultados são filtrados por `TenantId = "demo-clinic-001"`

### Cenário 2: Acesso via Subdomain
1. **Requisição**: `https://demo-clinic-001.medicsoft.com/api/patients`
2. **Middleware**: `TenantResolutionMiddleware` resolve subdomain para TenantId
3. **Header**: Define `X-Tenant-Id: demo-clinic-001`
4. **Controller**: `GetTenantId()` obtém do header

### Cenário 3: Cadastro Manual de Paciente
1. **POST** `api/patients` com JWT token válido
2. `PatientsController.Create()` chama `GetTenantId()` → obtém do JWT
3. `PatientService.CreatePatientAsync(dto, tenantId)`
4. Paciente criado com `TenantId` correto

---

## Componentes Verificados (Funcionando Corretamente)

✅ **JwtTokenService**: Inclui corretamente o claim `"tenant_id"` (linha 72)
- `new Claim("tenant_id", tenantId)`

✅ **DataSeederService**: Usa hardcoded `_demoTenantId = "demo-clinic-001"` consistentemente
- Pacientes criados com TenantId correto
- Usuários criados com TenantId correto

✅ **AuthController**: Passa `tenantId` corretamente ao gerar token (linha 100)
- `_jwtTokenService.GenerateToken(..., tenantId: tenantId, ...)`

✅ **CreatePatientCommandHandler**: Usa `request.TenantId` obtido do controller
- Mapping passa TenantId via `opt.Items["TenantId"] = request.TenantId`

✅ **PatientRepository**: Queries filtram corretamente por TenantId e IsActive
- `GetByClinicIdAsync()` já filtrava IsActive
- Agora `GetAllAsync()` também filtra

✅ **TenantResolutionMiddleware**: Resolve corretamente via subdomain
- Define `X-Tenant-Id` header quando encontra clinic

---

## Checklist de Verificação

Para garantir que o fluxo está funcionando:

- [ ] Executar seed: `POST /api/data-seeder/seed-demo`
- [ ] Verificar pacientes criados com TenantId `"demo-clinic-001"` no banco
- [ ] Login com `admin@clinicademo.com.br` / `Admin@123`
- [ ] Copiar JWT token retornado
- [ ] Chamar `GET /api/patients` com header `Authorization: Bearer <token>`
- [ ] Verificar que pacientes retornados têm TenantId `"demo-clinic-001"`
- [ ] Cadastrar novo paciente via `POST /api/patients`
- [ ] Verificar que novo paciente foi criado com TenantId correto

---

## Informações para Debug

### Logs Importantes
Procure nos logs por:
- `"Extracted tenant_id from JWT claims"`
- `"Extracted subdomain from host"`
- `"Resolved subdomain to tenantId"`
- `"User login attempt for username"`

### Queries SQL para Verificação
```sql
-- Ver pacientes por tenant
SELECT id, name, tenantid, isactive FROM patients WHERE tenantid = 'demo-clinic-001';

-- Ver usuários por tenant
SELECT id, username, tenantid FROM users WHERE tenantid = 'demo-clinic-001';

-- Verificar clínicas
SELECT id, tenantid, name FROM clinics WHERE tenantid = 'demo-clinic-001';
```

---

## Resumo das Correções

| Arquivo | Linha | Problema | Solução |
|---------|-------|----------|---------|
| `BaseController.cs` | 20-25 | Fallback para `"default-tenant"` sem verificar JWT | Priorizar JWT claims, depois headers, depois contexto |
| `PatientRepository.cs` | + | Falta filtro `IsActive` em `GetAllAsync()` | Sobrescrever `GetAllAsync()` com filtro |

---

## Recomendações Adicionais

1. **Ambiente de Desenvolvimento**:
   - Sempre usar header `X-Tenant-Id` em requisições de API ou autenticar primeiro
   - Ou usar subdomain correto ao acessar (ex: `localhost:demo-clinic-001/api/...` não funciona, precisa de DNS real)

2. **Frontend**:
   - Armazenar JWT token após login
   - Incluir token em todas as requisições subsequentes
   - Nunca fazer requisições sem autenticação para endpoints autenticados

3. **Testes**:
   - Criar suite de testes para verificar isolamento de TenantId
   - Testar que usuário de um tenant não pode ver dados de outro

4. **Monitoramento**:
   - Adicionar logs em `BaseController.GetTenantId()` para rastrear qual fonte está sendo usada
   - Alertar se fallback para `"default-tenant"` está sendo usado em produção

---

## Dúvidas e Próximos Passos

Se os pacientes ainda não aparecerem após essas correções:
1. Verificar se JWT token está sendo enviado corretamente
2. Verificar logs do servidor para ver qual TenantId está sendo usado
3. Executar queries SQL acima para confirmar que dados estão no banco com TenantId correto
4. Verificar se há middleware adicionais que podem estar alterando o TenantId
