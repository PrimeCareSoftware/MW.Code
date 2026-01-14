# 🧪 Guia de Testes - Portal do Paciente

> **Status:** ✅ Completo  
> **Última Atualização:** Janeiro 2026  
> **Code Coverage:** 98.66%

## 📋 Visão Geral

Este documento descreve a infraestrutura de testes do Portal do Paciente, incluindo testes unitários, de integração e end-to-end.

## 🎯 Métricas de Qualidade

### Coverage Summary
```
Statements   : 98.66% ( 74/75 )
Branches     : 92.85% ( 13/14 )
Functions    : 100% ( 33/33 )
Lines        : 98.64% ( 73/74 )
```

### Testes Executados
- **Total de Testes:** 52
- **Status:** 100% passando ✅
- **Tempo de Execução:** ~0.2 segundos

## 🧪 Tipos de Testes

### 1. Testes Unitários (Frontend)

#### Localização
```
frontend/patient-portal/src/app/
├── app.spec.ts                              # 1 teste
└── services/
    ├── auth.service.spec.ts                 # 18 testes
    ├── appointment.service.spec.ts          # 12 testes
    ├── document.service.spec.ts             # 12 testes
    └── profile.service.spec.ts              # 9 testes
```

#### Tecnologias
- **Framework:** Jasmine 5.1
- **Runner:** Karma 6.4
- **Browser:** Chrome Headless
- **Utilities:** Angular Testing Module, HttpClientTestingModule

### 2. Testes de Integração (Backend)

#### Localização
```
patient-portal-api/PatientPortal.Tests/
├── Integration/
│   ├── AuthControllerIntegrationTests.cs   # 7 testes
│   └── CustomWebApplicationFactory.cs
└── Domain/
    ├── PatientUserTests.cs                  # 7 testes
    └── RefreshTokenTests.cs                 # 5 testes
```

#### Tecnologias
- **Framework:** xUnit
- **Mocking:** Moq
- **Test Server:** ASP.NET Core TestServer

### 3. Testes E2E (End-to-End)

#### Localização
```
frontend/patient-portal/e2e/
├── auth.spec.ts                             # 7 testes
├── dashboard.spec.ts                        # 6 testes
├── appointments.spec.ts                     # 5 testes
├── documents.spec.ts                        # 6 testes
└── profile.spec.ts                          # 6 testes
```

#### Tecnologias
- **Framework:** Playwright
- **Browsers:** Chromium, Firefox, WebKit
- **Dispositivos:** Desktop + Mobile

## 📝 Guia de Testes por Serviço

### AuthService (18 testes)

**Cobertura:**
- ✅ Login com email
- ✅ Login com CPF
- ✅ Registro de novos usuários
- ✅ Refresh token
- ✅ Logout
- ✅ Change password
- ✅ Tratamento de erros (401, 404, 500)
- ✅ Gerenciamento de tokens no localStorage
- ✅ Observable currentUser$

**Exemplo de Teste:**
```typescript
it('should login successfully with email', (done) => {
  const loginRequest: LoginRequest = {
    emailOrCPF: 'test@example.com',
    password: 'Password123!'
  };

  service.login(loginRequest).subscribe(response => {
    expect(response).toEqual(mockLoginResponse);
    expect(service.isAuthenticated()).toBe(true);
    done();
  });

  const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
  expect(req.request.method).toBe('POST');
  req.flush(mockLoginResponse);
});
```

### AppointmentService (12 testes)

**Cobertura:**
- ✅ Listar agendamentos (com paginação)
- ✅ Agendamentos futuros
- ✅ Buscar agendamento por ID
- ✅ Filtrar por status
- ✅ Contagem de agendamentos
- ✅ Tratamento de erros (404, 500, network)

**Exemplo de Teste:**
```typescript
it('should retrieve upcoming appointments', (done) => {
  service.getUpcomingAppointments(5).subscribe(appointments => {
    expect(appointments.length).toBe(1);
    done();
  });

  const req = httpMock.expectOne('http://localhost:5000/api/appointments/upcoming?take=5');
  req.flush([mockAppointment]);
});
```

### DocumentService (12 testes)

**Cobertura:**
- ✅ Listar documentos (com paginação)
- ✅ Documentos recentes
- ✅ Buscar documento por ID
- ✅ Filtrar por tipo
- ✅ Contagem de documentos
- ✅ Download de documentos (Blob)
- ✅ Tratamento de erros

**Exemplo de Teste:**
```typescript
it('should download document as blob', (done) => {
  const mockBlob = new Blob(['PDF content'], { type: 'application/pdf' });

  service.downloadDocument('1').subscribe(blob => {
    expect(blob.type).toBe('application/pdf');
    done();
  });

  const req = httpMock.expectOne('http://localhost:5000/api/documents/1/download');
  expect(req.request.responseType).toBe('blob');
  req.flush(mockBlob);
});
```

### ProfileService (9 testes)

**Cobertura:**
- ✅ Obter perfil do usuário
- ✅ Atualizar nome completo
- ✅ Atualizar telefone
- ✅ Atualizar ambos simultaneamente
- ✅ Tratamento de erros (401, 404, 400, 500)

**Exemplo de Teste:**
```typescript
it('should update profile successfully', (done) => {
  const updateRequest = {
    fullName: 'John Doe Santos',
    phoneNumber: '+5511999887766'
  };

  service.updateProfile(updateRequest).subscribe(response => {
    expect(response.message).toBe('Profile updated successfully');
    done();
  });

  const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
  expect(req.request.method).toBe('PUT');
  req.flush({ message: 'Profile updated successfully' });
});
```

## 🚀 Como Executar os Testes

### Testes Unitários (Frontend)

#### Executar todos os testes
```bash
cd frontend/patient-portal
npm test
```

#### Executar com coverage
```bash
npm test -- --code-coverage
```

#### Executar em modo headless (CI)
```bash
npm test -- --browsers=ChromeHeadless --watch=false
```

#### Executar testes específicos
```bash
npm test -- --include='**/auth.service.spec.ts'
```

### Testes de Integração (Backend)

```bash
cd patient-portal-api
dotnet test --verbosity normal
```

#### Com coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Testes E2E

```bash
cd frontend/patient-portal
npm run e2e
```

#### Com UI interativa
```bash
npm run e2e:ui
```

#### Browser específico
```bash
npm run e2e -- --project chromium
```

## 📊 Relatórios de Coverage

### Localização dos Relatórios
```
frontend/patient-portal/coverage/
├── lcov-report/
│   └── index.html          # Relatório HTML detalhado
├── lcov.info               # Formato LCOV
└── coverage-summary.json   # JSON summary
```

### Visualizar Relatório
```bash
cd frontend/patient-portal/coverage/lcov-report
open index.html  # macOS
xdg-open index.html  # Linux
start index.html  # Windows
```

## 🎨 Boas Práticas de Testes

### 1. Estrutura AAA (Arrange, Act, Assert)
```typescript
it('should do something', (done) => {
  // Arrange
  const mockData = { /* ... */ };
  
  // Act
  service.doSomething(mockData).subscribe(result => {
    // Assert
    expect(result).toEqual(expectedResult);
    done();
  });
  
  const req = httpMock.expectOne(url);
  req.flush(mockResponse);
});
```

### 2. Usar Mock Data Consistente
```typescript
const mockUser: User = {
  id: '123e4567-e89b-12d3-a456-426614174000',
  email: 'test@example.com',
  // ... outros campos
};
```

### 3. Testar Casos de Erro
```typescript
it('should handle 404 errors', (done) => {
  service.getById('999').subscribe({
    next: () => fail('should have failed'),
    error: (error) => {
      expect(error.status).toBe(404);
      done();
    }
  });

  const req = httpMock.expectOne(url);
  req.flush({ message: 'Not found' }, { status: 404, statusText: 'Not Found' });
});
```

### 4. Cleanup de Recursos
```typescript
afterEach(() => {
  httpMock.verify();
  localStorage.clear();
});
```

### 5. Testes Assíncronos
```typescript
it('should handle async operations', async () => {
  const result = await firstValueFrom(service.getData());
  expect(result).toBeDefined();
});
```

## 🔍 Debugging de Testes

### Chrome DevTools
```bash
npm test -- --browsers=Chrome
# Abre o browser e permite debugar com DevTools
```

### Logs de Debug
```typescript
it('should debug something', () => {
  console.log('Debug info:', mockData);
  // ... teste
});
```

### Executar Teste Único
```typescript
fit('should run only this test', () => {
  // Este teste será executado sozinho
});
```

### Pular Teste Temporariamente
```typescript
xit('should skip this test', () => {
  // Este teste será pulado
});
```

## 🐛 Troubleshooting

### Problema: Testes falham com "Cannot find module"
**Solução:**
```bash
npm install
```

### Problema: Browser não inicia
**Solução:**
```bash
# Instalar Chrome Headless
npm install --save-dev puppeteer
```

### Problema: Timeout nos testes
**Solução:**
```typescript
// karma.conf.js
browserNoActivityTimeout: 60000,
captureTimeout: 210000
```

### Problema: Coverage baixo em alguns arquivos
**Solução:**
```bash
# Verificar quais linhas não foram cobertas
npm test -- --code-coverage
# Abrir relatório HTML e identificar linhas vermelhas
```

## 📚 Recursos Adicionais

### Documentação Oficial
- [Angular Testing Guide](https://angular.dev/guide/testing)
- [Jasmine Documentation](https://jasmine.github.io/)
- [Karma Documentation](https://karma-runner.github.io/)
- [Playwright Documentation](https://playwright.dev/)

### Artigos Recomendados
- [Testing Best Practices](https://testingjavascript.com/)
- [Angular Testing Patterns](https://blog.angular.io/testing-patterns-for-angular-applications-f3b6a39c6c4)

## 📝 Checklist de Novos Testes

Ao adicionar novos testes, certifique-se de:

- [ ] Seguir a estrutura AAA
- [ ] Testar casos de sucesso
- [ ] Testar casos de erro (401, 404, 500, network)
- [ ] Limpar recursos no afterEach
- [ ] Usar mock data consistente
- [ ] Adicionar descrições claras (it/describe)
- [ ] Verificar coverage (>70%)
- [ ] Executar todos os testes antes de commitar

## 🎯 Metas de Qualidade

### Atuais (Janeiro 2026)
- ✅ Code Coverage: 98.66%
- ✅ Testes Passando: 100%
- ✅ Tempo de Execução: < 1s

### Metas Futuras
- [ ] Code Coverage: > 90% (já alcançado!)
- [ ] Testes E2E: 100% cobertura de fluxos críticos
- [ ] Performance: Tempo de execução < 500ms
- [ ] Testes de Mutação: > 80%

---

**Documento Criado Por:** GitHub Copilot  
**Data:** Janeiro 2026  
**Versão:** 1.0  

**Este documento é atualizado conforme novos testes são adicionados ao projeto.**
