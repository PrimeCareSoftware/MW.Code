# Guia de Integração da API de Telemedicina no Frontend

## Visão Geral

Este documento descreve como usar a integração da API de Telemedicina (videochamadas) do MedicSoft no frontend do MedicWarehouse.

## Configuração

### 1. Variáveis de Ambiente

As URLs da API de telemedicina já estão configuradas nos arquivos de ambiente:

**Development** (`environment.ts`):
```typescript
microservices: {
  // ... outros serviços
  telemedicine: 'http://localhost:5084/api'
}
```

**Production** (`environment.prod.ts`):
```typescript
microservices: {
  // ... outros serviços
  telemedicine: 'https://telemedicine.medicwarehouse.com/api'
}
```

### 2. Habilitar Modo Microservices

Para usar a API de telemedicina, configure `useMicroservices: true` no arquivo de ambiente apropriado.

## Serviços Disponíveis

### TelemedicineService

O serviço `TelemedicineService` fornece métodos para gerenciar sessões de videochamada.

#### Importação

```typescript
import { TelemedicineService } from './services/telemedicine.service';
import { 
  CreateSessionRequest, 
  JoinSessionRequest, 
  ParticipantRole 
} from './models/telemedicine.model';
```

#### Métodos Principais

##### 1. Criar Sessão

Cria uma nova sessão de telemedicina vinculada a um agendamento.

```typescript
constructor(private telemedicineService: TelemedicineService) {}

createTelemedicineSession(appointmentId: string, clinicId: string, providerId: string, patientId: string) {
  const request: CreateSessionRequest = {
    appointmentId: appointmentId,
    clinicId: clinicId,
    providerId: providerId,
    patientId: patientId
  };

  this.telemedicineService.createSession(request).subscribe({
    next: (session) => {
      console.log('Sessão criada:', session);
      // session.id contém o ID da sessão
      // session.roomUrl contém a URL da sala
    },
    error: (error) => console.error('Erro ao criar sessão:', error)
  });
}
```

##### 2. Entrar na Sessão

Gera credenciais de acesso para um participante entrar na videochamada.

```typescript
joinVideoCall(sessionId: string, userId: string, userName: string, isProvider: boolean) {
  const request: JoinSessionRequest = {
    userId: userId,
    userName: userName,
    role: isProvider ? ParticipantRole.Provider : ParticipantRole.Patient
  };

  this.telemedicineService.joinSession(sessionId, request).subscribe({
    next: (joinInfo) => {
      // joinInfo.roomUrl - URL da sala Daily.co
      // joinInfo.accessToken - Token de acesso temporário
      // joinInfo.expiresAt - Data de expiração do token
      
      // Aqui você pode abrir a sala de videochamada
      window.open(joinInfo.roomUrl, '_blank');
    },
    error: (error) => console.error('Erro ao entrar na sessão:', error)
  });
}
```

##### 3. Iniciar Sessão

Marca a sessão como "Em Progresso".

```typescript
startVideoSession(sessionId: string) {
  this.telemedicineService.startSession(sessionId).subscribe({
    next: (session) => {
      console.log('Sessão iniciada:', session);
    },
    error: (error) => console.error('Erro ao iniciar sessão:', error)
  });
}
```

##### 4. Completar Sessão

Finaliza a sessão, opcionalmente adicionando notas.

```typescript
completeVideoSession(sessionId: string, notes: string) {
  this.telemedicineService.completeSession(sessionId, { notes }).subscribe({
    next: (session) => {
      console.log('Sessão completada:', session);
      // session.durationMinutes contém a duração total
      // session.recordingUrl contém a URL de gravação (se disponível)
    },
    error: (error) => console.error('Erro ao completar sessão:', error)
  });
}
```

##### 5. Cancelar Sessão

Cancela uma sessão agendada.

```typescript
cancelVideoSession(sessionId: string, reason: string) {
  this.telemedicineService.cancelSession(sessionId, reason).subscribe({
    next: (session) => {
      console.log('Sessão cancelada:', session);
    },
    error: (error) => console.error('Erro ao cancelar sessão:', error)
  });
}
```

##### 6. Buscar Sessão

Obtém informações de uma sessão específica.

```typescript
// Por ID da sessão
getSession(sessionId: string) {
  this.telemedicineService.getSessionById(sessionId).subscribe({
    next: (session) => console.log('Sessão:', session),
    error: (error) => console.error('Erro:', error)
  });
}

// Por ID do agendamento
getSessionByAppointment(appointmentId: string) {
  this.telemedicineService.getSessionByAppointmentId(appointmentId).subscribe({
    next: (session) => console.log('Sessão:', session),
    error: (error) => console.error('Erro:', error)
  });
}
```

##### 7. Listar Sessões

Obtém lista de sessões por clínica, médico ou paciente.

```typescript
// Sessões da clínica
listClinicSessions(clinicId: string) {
  this.telemedicineService.getClinicSessions(clinicId, 0, 50).subscribe({
    next: (sessions) => console.log('Sessões da clínica:', sessions),
    error: (error) => console.error('Erro:', error)
  });
}

// Sessões do médico
listProviderSessions(providerId: string) {
  this.telemedicineService.getProviderSessions(providerId, 0, 50).subscribe({
    next: (sessions) => console.log('Sessões do médico:', sessions),
    error: (error) => console.error('Erro:', error)
  });
}

// Sessões do paciente
listPatientSessions(patientId: string) {
  this.telemedicineService.getPatientSessions(patientId, 0, 50).subscribe({
    next: (sessions) => console.log('Sessões do paciente:', sessions),
    error: (error) => console.error('Erro:', error)
  });
}
```

## Modelos de Dados

### SessionStatus

Estados possíveis de uma sessão:

```typescript
enum SessionStatus {
  Scheduled = 'Scheduled',      // Agendada
  InProgress = 'InProgress',    // Em progresso
  Completed = 'Completed',      // Completada
  Cancelled = 'Cancelled',      // Cancelada
  Failed = 'Failed'             // Falhou
}
```

### TelemedicineSession

```typescript
interface TelemedicineSession {
  id: string;
  tenantId: string;
  appointmentId: string;
  clinicId: string;
  providerId: string;
  patientId: string;
  roomId: string;
  roomUrl: string;
  status: SessionStatus;
  durationMinutes?: number;
  recordingUrl?: string;
  sessionNotes?: string;
  createdAt: string;
  updatedAt?: string;
  startedAt?: string;
  completedAt?: string;
}
```

## Fluxo de Uso Típico

### Para Agendamentos com Telemedicina

1. **Ao agendar consulta de telemedicina:**
   ```typescript
   // Criar agendamento normal primeiro
   // Depois criar sessão de telemedicina
   this.telemedicineService.createSession({
     appointmentId: appointment.id,
     clinicId: clinicId,
     providerId: providerId,
     patientId: patientId
   }).subscribe(session => {
     console.log('Sessão criada para agendamento');
   });
   ```

2. **Quando paciente/médico entrar na consulta:**
   ```typescript
   // Iniciar sessão
   this.telemedicineService.startSession(sessionId).subscribe();
   
   // Obter credenciais de acesso
   this.telemedicineService.joinSession(sessionId, {
     userId: currentUserId,
     userName: currentUserName,
     role: userRole
   }).subscribe(joinInfo => {
     // Abrir videochamada com joinInfo.roomUrl e joinInfo.accessToken
     // Ou usar a biblioteca @daily-co/daily-js para embedding
   });
   ```

3. **Ao finalizar consulta:**
   ```typescript
   this.telemedicineService.completeSession(sessionId, {
     notes: 'Consulta realizada com sucesso'
   }).subscribe(session => {
     console.log('Duração:', session.durationMinutes, 'minutos');
     if (session.recordingUrl) {
       console.log('Gravação disponível em:', session.recordingUrl);
     }
   });
   ```

## Integração com Daily.co (Opcional)

Para uma integração mais avançada, você pode usar a biblioteca Daily.co:

```bash
npm install @daily-co/daily-js
```

Exemplo de uso:

```typescript
import DailyIframe from '@daily-co/daily-js';

// Criar frame de vídeo
const callFrame = DailyIframe.createFrame({
  iframeStyle: {
    width: '100%',
    height: '600px',
  }
});

// Entrar na chamada
this.telemedicineService.joinSession(sessionId, joinRequest).subscribe(joinInfo => {
  callFrame.join({
    url: joinInfo.roomUrl,
    token: joinInfo.accessToken
  });
});
```

## Executar com Docker

Para executar o microserviço de telemedicina com Docker:

```bash
# Configurar variável de ambiente
export DAILYCO_API_KEY=sua-chave-api-daily-co

# Executar com docker-compose
docker-compose -f docker-compose.microservices.yml up telemedicine-api
```

## Segurança

- Todas as requisições requerem autenticação JWT
- O header `X-Tenant-Id` é obrigatório para isolamento multi-tenant
- Tokens de acesso às salas são temporários (120 minutos)
- A API valida permissões antes de permitir acesso às sessões

## Troubleshooting

### Erro: "Cannot find module telemedicine.service"

Verifique se o serviço está sendo importado corretamente:

```typescript
import { TelemedicineService } from './services/telemedicine.service';
```

### Erro: "X-Tenant-Id header is missing"

O tenant ID é obtido do localStorage. Certifique-se de que o usuário está autenticado:

```typescript
localStorage.getItem('tenantId')
```

### Erro: "API endpoint not found"

Verifique se `useMicroservices: true` está configurado no ambiente e se a API de telemedicina está rodando na porta correta (5084).

## Referências

- [README da API de Telemedicina](/telemedicine/README.md)
- [Documentação Daily.co](https://docs.daily.co)
- [Guia de Integração Frontend](/telemedicine/FRONTEND_INTEGRATION.md)
