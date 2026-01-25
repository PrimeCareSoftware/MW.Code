# Frontend API Configuration Guide

Este documento explica como configurar corretamente as URLs da API nos aplicativos frontend para evitar erros de conexão, código 307 (redirect) ou falta de resposta da API.

## 🔧 Problema Identificado e Resolvido

### Problema Original
Os aplicativos frontend estavam configurados com URLs incorretas que causavam:
- **Código 307 (Temporary Redirect)**: Quando o frontend tentava acessar via HTTPS na porta errada
- **Sem resposta da API**: Quando a porta ou protocolo não correspondiam ao backend
- **Redirecionamento forçado**: Backend estava redirecionando HTTP → HTTPS mesmo em desenvolvimento

### Solução Aplicada
1. ✅ Corrigido URLs nos arquivos `environment.ts` de todos os frontends
2. ✅ Tornado o redirecionamento HTTPS condicional no backend (`Security:RequireHttps`)
3. ✅ Documentado a configuração correta para diferentes cenários

## 📋 Configuração por Ambiente

### 🔨 Desenvolvimento Local (com Docker)

**Backend:**
- URL: `http://localhost:5000`
- Container interno roda na porta 8080, mapeada para 5000 no host
- Sem HTTPS (não necessário em dev)

**Frontend - medicwarehouse-app:**
```typescript
// frontend/medicwarehouse-app/src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',  // ✅ Correto
  enableDebug: true,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
```

**Frontend - mw-system-admin:**
```typescript
// frontend/mw-system-admin/src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',  // ✅ Correto
  enableDebug: true,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
```

**Frontend - mw-site:**
```typescript
// frontend/mw-site/src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',  // ✅ Correto (sem /api - adicionado nos services)
  whatsappNumber: '5511999999999',
  companyEmail: 'contato@primecaresoftware.com',
  companyPhone: '(11) 99999-9999'
};
```

### 💻 Desenvolvimento Local (com dotnet run)

**Backend:**
- URL HTTPS: `https://localhost:5000` (principal)
- URL HTTP: `http://localhost:5001` (alternativa)
- Configurado em `src/MedicSoft.Api/Properties/launchSettings.json`

**Frontend:**
Mesma configuração que Docker:
```typescript
apiUrl: 'http://localhost:5000/api'  // Usa HTTP na porta 5000
```

**Nota:** Embora o backend suporte HTTPS na porta 5000, os frontends usam HTTP para evitar problemas de certificado autoassinado em desenvolvimento.

### 🚀 Produção

**Backend:**
- URL: `https://api.medicwarehouse.com`
- HTTPS obrigatório (`Security:RequireHttps: true`)
- Certificado SSL válido

**Frontend - medicwarehouse-app:**
```typescript
// frontend/medicwarehouse-app/src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com/api',  // ✅ HTTPS obrigatório
  enableDebug: false,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
```

**Frontend - mw-system-admin:**
```typescript
// frontend/mw-system-admin/src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com/api',  // ✅ HTTPS obrigatório
  enableDebug: false,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
```

**Frontend - mw-site:**
```typescript
// frontend/mw-site/src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com',  // ✅ HTTPS obrigatório
  whatsappNumber: '5511999999999',
  companyEmail: 'contato@primecaresoftware.com',
  companyPhone: '(11) 99999-9999'
};
```

## 🔐 Configuração de Segurança no Backend

O redirecionamento HTTPS agora é condicional, baseado na configuração:

**Desenvolvimento (appsettings.json):**
```json
{
  "Security": {
    "RequireHttps": false  // ✅ Sem redirecionamento forçado
  }
}
```

**Produção (appsettings.Production.json):**
```json
{
  "Security": {
    "RequireHttps": true  // ✅ HTTPS obrigatório
  }
}
```

**Implementação em Program.cs:**
```csharp
// Use HTTPS redirection only if required by configuration
var requireHttps = builder.Configuration.GetValue<bool>("Security:RequireHttps", false);
if (requireHttps)
{
    app.UseHttpsRedirection();
}
```

## 🌐 CORS - Origens Permitidas

**Desenvolvimento (appsettings.json):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",  // medicwarehouse-app
      "http://localhost:4201",  // mw-system-admin
      "http://localhost:3000"   // mw-site (se usar porta 3000)
    ]
  }
}
```

**Produção (appsettings.Production.json):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://medicwarehouse.com",
      "https://www.medicwarehouse.com",
      "https://app.medicwarehouse.com"
    ]
  }
}
```

## 🧪 Como Testar a Configuração

### 1. Testar Backend API

```bash
# Com Docker
docker-compose up -d
curl http://localhost:5000/swagger

# Com dotnet run
cd src/MedicSoft.Api
dotnet run
# Abrir navegador: https://localhost:5000/swagger
```

### 2. Testar Frontend

```bash
# medicwarehouse-app
cd frontend/medicwarehouse-app
npm install
npm start
# Abrir navegador: http://localhost:4200

# mw-system-admin
cd frontend/mw-system-admin
npm install
npm start
# Abrir navegador: http://localhost:4201

# mw-site
cd frontend/mw-site
npm install
npm start
# Abrir navegador: http://localhost:4200
```

### 3. Testar Chamada API

Abra o console do navegador (F12) e execute:

```javascript
// Verificar URL configurada
console.log('API URL:', environment.apiUrl);

// Fazer chamada de teste
fetch('http://localhost:5000/api/data-seeder/demo-info')
  .then(r => r.json())
  .then(d => console.log('✅ API funcionando:', d))
  .catch(e => console.error('❌ Erro na API:', e));
```

## 🐛 Troubleshooting

### Erro 307 (Temporary Redirect)

**Causa:** Backend está redirecionando HTTP → HTTPS
**Solução:** 
- Verifique `Security:RequireHttps: false` em appsettings.json
- Use `http://localhost:5000/api` no frontend (não HTTPS)

### Erro CORS

**Causa:** Frontend não está nas origens permitidas
**Solução:** 
- Adicione a origem em `Cors:AllowedOrigins` no appsettings.json
- Ex: `"http://localhost:4200"`

### Erro "Cannot connect to API"

**Causa:** Backend não está rodando ou porta incorreta
**Solução:**
- Verifique se o backend está rodando: `docker ps` ou `dotnet run`
- Confirme a porta: `5000` para Docker, `5000` (HTTPS) ou `5001` (HTTP) para dotnet
- Verifique logs: `docker-compose logs -f api`

### Erro "net::ERR_CERT_AUTHORITY_INVALID"

**Causa:** Tentando usar HTTPS com certificado autoassinado
**Solução:**
- Use HTTP em desenvolvimento: `http://localhost:5000/api`
- Ou aceite o certificado no navegador (não recomendado)

## 📝 Nota Importante sobre mw-site

O aplicativo `mw-site` usa uma configuração ligeiramente diferente:

- **environment.ts:** `apiUrl: 'http://localhost:5000'` (sem `/api`)
- **services:** Adicionam `/api` nas chamadas: `${this.apiUrl}/api/registration`

Isso é por design e está funcionando corretamente.

## ✅ Checklist de Configuração

Antes de começar o desenvolvimento, verifique:

- [ ] Backend configurado: `Security:RequireHttps: false`
- [ ] CORS configurado: `http://localhost:4200` nas origens permitidas
- [ ] Frontend medicwarehouse-app: `apiUrl: 'http://localhost:5000/api'`
- [ ] Frontend mw-system-admin: `apiUrl: 'http://localhost:5000/api'`
- [ ] Frontend mw-site: `apiUrl: 'http://localhost:5000'`
- [ ] Backend rodando: `docker-compose up -d` ou `dotnet run`
- [ ] Teste de conectividade: Swagger acessível em `http://localhost:5000/swagger`

## 📚 Referências

- [README.md](../README.md) - Guia geral do projeto
- [GUIA_EXECUCAO.md](GUIA_EXECUCAO.md) - Como executar o projeto
- [AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md) - Guia de autenticação
- [SECURITY_GUIDE.md](SECURITY_GUIDE.md) - Guia de segurança

---

**Última atualização:** 2025-11-03
**Versão:** 1.0
