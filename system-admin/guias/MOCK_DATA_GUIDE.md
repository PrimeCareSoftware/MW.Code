# Guia de Dados Mockados (Mock Data)

## Visão Geral

Este guia documenta a funcionalidade de dados mockados implementada nos aplicativos frontend do Omni Care Software. Esta funcionalidade permite que os aplicativos sejam executados sem a necessidade de um backend ativo, facilitando o desenvolvimento, testes e demonstrações.

## Benefícios

- 🚀 **Desenvolvimento Independente**: Desenvolvedores frontend podem trabalhar sem depender do backend
- 🧪 **Testes**: Facilita testes de UI sem configurar toda a infraestrutura
- 📊 **Demonstrações**: Permite demonstrações do sistema sem servidor
- 🎓 **Aprendizado**: Desenvolvedores podem explorar o sistema sem riscos

## Como Habilitar

### Opção 1: Através das Variáveis de Ambiente

#### Omni Care Software App

Edite o arquivo `/frontend/medicwarehouse-app/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  useMockData: true, // ← Altere para true
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  },
  tenant: {
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger']
  }
};
```

#### MW System Admin

Edite o arquivo `/frontend/mw-system-admin/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  useMockData: true, // ← Altere para true
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
```

### Opção 2: Futuro - Configuração via API

Em versões futuras, será possível habilitar/desabilitar dados mockados através de uma chamada à API, permitindo controle dinâmico sem recompilar o aplicativo.

## Estrutura dos Arquivos

### Omni Care Software App

```
frontend/medicwarehouse-app/src/app/
├── mocks/
│   ├── patient.mock.ts           # Dados de pacientes
│   ├── appointment.mock.ts       # Dados de agendamentos
│   ├── auth.mock.ts              # Dados de autenticação
│   ├── procedure.mock.ts         # Dados de procedimentos
│   ├── exam-request.mock.ts      # Dados de solicitações de exames
│   ├── medical-record.mock.ts    # Dados de prontuários
│   └── waiting-queue.mock.ts     # Dados da fila de espera
└── interceptors/
    └── mock-data.interceptor.ts  # Interceptor HTTP para mock
```

### MW System Admin

```
frontend/mw-system-admin/src/app/
├── mocks/
│   ├── auth.mock.ts              # Dados de autenticação
│   └── system-admin.mock.ts      # Dados administrativos
└── interceptors/
    └── mock-data.interceptor.ts  # Interceptor HTTP para mock
```

## Dados Mockados Disponíveis

### Omni Care Software App

#### Pacientes (`patient.mock.ts`)
- 3 pacientes de exemplo
- Incluindo adultos e criança com tutor
- Dados completos: nome, documento, endereço, histórico médico, alergias

#### Agendamentos (`appointment.mock.ts`)
- 3 agendamentos em diferentes status
- Agenda diária mockada
- Horários disponíveis mockados

#### Autenticação (`auth.mock.ts`)
- Token JWT mockado
- Informações de usuário mockado
- Válido por 24 horas

#### Procedimentos (`procedure.mock.ts`)
- 4 procedimentos de diferentes categorias
- Procedimentos vinculados a agendamentos
- Resumo de faturamento

#### Solicitações de Exames (`exam-request.mock.ts`)
- 3 solicitações de exames em diferentes status
- Exames pendentes e urgentes
- Diferentes tipos: laboratorial, imagem, cardíaco

#### Prontuários Médicos (`medical-record.mock.ts`)
- 3 prontuários de exemplo
- Diagnósticos, prescrições e notas
- Histórico de consultas

#### Fila de Espera (`waiting-queue.mock.ts`)
- 3 entradas na fila
- Diferentes prioridades e status
- Configuração da fila
- Exibição pública

### MW System Admin

#### Clínicas (`system-admin.mock.ts`)
- 3 clínicas de exemplo
- Diferentes status: ativa, trial, suspensa
- Análises e métricas do sistema
- Dados paginados

#### System Owners
- 2 administradores do sistema
- Informações completas de acesso

#### Autenticação (`auth.mock.ts`)
- Token JWT mockado para administrador
- Informações de usuário com privilégios de system owner

## Funcionamento Técnico

### Interceptor HTTP

O interceptor HTTP (`mock-data.interceptor.ts`) captura todas as requisições HTTP quando `useMockData` está habilitado:

1. **Verifica a flag**: Se `environment.useMockData` for `false`, passa a requisição para o backend real
2. **Analisa a URL e método**: Identifica qual endpoint está sendo chamado
3. **Retorna dados mockados**: Retorna uma resposta HTTP simulada com os dados mockados apropriados
4. **Simula latência**: Adiciona um delay de 200-500ms para simular latência de rede

### Ordem dos Interceptors

Os interceptors são executados na ordem:
1. `mockDataInterceptor` (primeiro) - Retorna mocks se habilitado
2. `authInterceptor` (segundo) - Adiciona autenticação se a requisição passar

## Endpoints Mockados

### Omni Care Software App

#### Autenticação
- `POST /api/auth/login` - Login com credenciais
- `GET /api/auth/me` - Informações do usuário atual

#### Pacientes
- `GET /api/patients` - Lista todos os pacientes
- `GET /api/patients/:id` - Busca paciente por ID
- `POST /api/patients` - Cria novo paciente
- `PUT /api/patients/:id` - Atualiza paciente
- `DELETE /api/patients/:id` - Remove paciente
- `GET /api/patients/search` - Busca pacientes
- `GET /api/patients/:id/children` - Lista filhos de um tutor
- `POST /api/patients/:childId/link-guardian/:guardianId` - Vincula tutor

#### Agendamentos
- `GET /api/appointments/agenda` - Agenda diária
- `GET /api/appointments/available-slots` - Horários disponíveis
- `GET /api/appointments/:id` - Busca agendamento
- `POST /api/appointments` - Cria agendamento
- `PUT /api/appointments/:id` - Atualiza agendamento
- `PUT /api/appointments/:id/cancel` - Cancela agendamento

#### Procedimentos
- `GET /api/procedures` - Lista procedimentos
- `GET /api/procedures/:id` - Busca procedimento
- `POST /api/procedures` - Cria procedimento
- `PUT /api/procedures/:id` - Atualiza procedimento
- `DELETE /api/procedures/:id` - Remove procedimento
- `GET /api/procedures/appointments/:id/procedures` - Procedimentos do agendamento
- `POST /api/procedures/appointments/:id/procedures` - Adiciona procedimento
- `GET /api/procedures/appointments/:id/billing-summary` - Resumo de cobrança

#### Solicitações de Exames
- `GET /api/exam-requests/pending` - Exames pendentes
- `GET /api/exam-requests/urgent` - Exames urgentes
- `GET /api/exam-requests/appointment/:id` - Exames do agendamento
- `GET /api/exam-requests/patient/:id` - Exames do paciente
- `GET /api/exam-requests/:id` - Busca exame
- `POST /api/exam-requests` - Cria solicitação
- `PUT /api/exam-requests/:id` - Atualiza solicitação
- `POST /api/exam-requests/:id/complete` - Completa exame
- `POST /api/exam-requests/:id/cancel` - Cancela exame

#### Prontuários Médicos
- `GET /api/medical-records/appointment/:id` - Prontuário do agendamento
- `GET /api/medical-records/patient/:id` - Prontuários do paciente
- `POST /api/medical-records` - Cria prontuário
- `PUT /api/medical-records/:id` - Atualiza prontuário
- `POST /api/medical-records/:id/complete` - Completa prontuário

#### Fila de Espera
- `GET /api/waiting-queue/clinic/:id` - Fila da clínica
- `GET /api/waiting-queue/clinic/:id/summary` - Resumo da fila
- `GET /api/waiting-queue/clinic/:id/configuration` - Configuração
- `PUT /api/waiting-queue/clinic/:id/configuration` - Atualiza configuração
- `GET /api/waiting-queue/clinic/:id/public` - Exibição pública
- `POST /api/waiting-queue` - Adiciona à fila
- `POST /api/waiting-queue/:id/call` - Chama paciente
- `POST /api/waiting-queue/:id/complete` - Completa atendimento
- `DELETE /api/waiting-queue/:id/cancel` - Cancela entrada
- `PUT /api/waiting-queue/:id/triage` - Atualiza triagem

### MW System Admin

#### Autenticação
- `POST /api/auth/login` - Login de administrador
- `GET /api/auth/me` - Informações do administrador

#### Clínicas
- `GET /api/system-admin/clinics` - Lista clínicas (paginado)
- `GET /api/system-admin/clinics/:id` - Detalhes da clínica
- `POST /api/system-admin/clinics` - Cria clínica
- `POST /api/system-admin/clinics/:id/toggle-status` - Ativa/desativa clínica
- `PUT /api/system-admin/clinics/:id/subscription` - Atualiza assinatura
- `POST /api/system-admin/clinics/:id/subscription/manual-override/enable` - Habilita override
- `POST /api/system-admin/clinics/:id/subscription/manual-override/disable` - Desabilita override

#### Analytics
- `GET /api/system-admin/analytics` - Métricas do sistema

#### System Owners
- `GET /api/system-admin/system-owners` - Lista administradores
- `POST /api/system-admin/system-owners` - Cria administrador
- `POST /api/system-admin/system-owners/:id/toggle-status` - Ativa/desativa

## Customização

### Adicionando Novos Dados Mockados

1. **Crie o arquivo de mock** em `src/app/mocks/`:
```typescript
// my-feature.mock.ts
export const MOCK_MY_DATA = [
  { id: '1', name: 'Exemplo 1' },
  { id: '2', name: 'Exemplo 2' }
];
```

2. **Importe no interceptor**:
```typescript
import { MOCK_MY_DATA } from '../mocks/my-feature.mock';
```

3. **Adicione a lógica de interceptação**:
```typescript
if (url.includes('/my-endpoint') && method === 'GET') {
  return of(new HttpResponse({ status: 200, body: MOCK_MY_DATA }))
    .pipe(delay(mockDelay));
}
```

### Modificando Dados Existentes

Edite os arquivos em `src/app/mocks/` para alterar os dados retornados. Por exemplo, para adicionar mais pacientes:

```typescript
// patient.mock.ts
export const MOCK_PATIENTS: Patient[] = [
  // ... pacientes existentes
  {
    id: '4',
    name: 'Novo Paciente',
    // ... outros campos
  }
];
```

## Limitações

- ❌ **Persistência**: Dados criados/modificados não persistem após refresh da página
- ❌ **Validações**: Validações de negócio do backend não são executadas
- ❌ **Relacionamentos**: Relacionamentos complexos são simplificados
- ❌ **Paginação Real**: A paginação é simulada, não real
- ⚠️ **Sincronização**: Múltiplas abas não compartilham estado

## Desenvolvimento Futuro

### Planejado
- [ ] Configuração via API para habilitar/desabilitar mocks dinamicamente
- [ ] Persistência de dados mockados no localStorage
- [ ] Interface admin para gerenciar dados mockados
- [ ] Geração automática de dados mockados a partir de schemas
- [ ] Mock de uploads de arquivos
- [ ] Simulação de erros e cenários de falha

## Troubleshooting

### Problema: Dados mockados não aparecem

**Solução**: Verifique se:
1. `useMockData` está `true` no arquivo environment correto
2. O aplicativo foi recompilado após a mudança
3. O cache do navegador foi limpo

### Problema: Console mostra "No mock handler"

**Solução**: Isso significa que um endpoint não tem mock implementado. Você pode:
1. Adicionar um mock para esse endpoint
2. Desabilitar mocks temporariamente
3. Ignorar se não for crítico para seu caso de uso

### Problema: Erros 404 em produção

**Solução**: Certifique-se de que `useMockData` está `false` em `environment.prod.ts`

## Exemplos de Uso

### Login Mockado

```typescript
// Qualquer credencial funcionará quando mocks estão habilitados
this.authService.login({
  username: 'admin',
  password: 'qualquer-senha',
  tenantId: 'clinic1'
}).subscribe(response => {
  console.log('Token mockado:', response.token);
});
```

### Listagem de Pacientes

```typescript
// Retorna os 3 pacientes mockados
this.patientService.getAll().subscribe(patients => {
  console.log('Pacientes mockados:', patients);
});
```

## Contribuindo

Para adicionar suporte a novos mocks:
1. Crie os dados mockados seguindo a estrutura dos models
2. Adicione os mocks ao interceptor
3. Teste todos os métodos HTTP (GET, POST, PUT, DELETE)
4. Atualize esta documentação

## Referências

- [Angular HTTP Interceptors](https://angular.io/guide/http-interceptors)
- [RxJS delay operator](https://rxjs.dev/api/operators/delay)
- [Guia de Desenvolvimento](GUIA_DESENVOLVIMENTO_AUTH.md)
