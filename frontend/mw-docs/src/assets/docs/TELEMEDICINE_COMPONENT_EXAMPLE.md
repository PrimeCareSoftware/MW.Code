# Exemplo de Componente de Telemedicina

## Componente de Botão para Videochamada

Este é um exemplo de como criar um componente Angular que integra a funcionalidade de videochamada usando o `TelemedicineService`.

### Arquivo: `telemedicine-button.component.ts`

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TelemedicineService } from '../../services/telemedicine.service';
import { ParticipantRole } from '../../models/telemedicine.model';

/**
 * Example component showing how to integrate telemedicine video calls
 * This is a reference implementation that can be adapted to your needs
 */
@Component({
  selector: 'app-telemedicine-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      *ngIf="appointmentId"
      (click)="startVideoCall()"
      [disabled]="isLoading"
      class="video-call-btn">
      <span *ngIf="!isLoading">📹 Iniciar Videochamada</span>
      <span *ngIf="isLoading">⏳ Conectando...</span>
    </button>

    <div *ngIf="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>
  `,
  styles: [`
    .video-call-btn {
      padding: 10px 20px;
      background-color: #4CAF50;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 16px;
    }

    .video-call-btn:hover {
      background-color: #45a049;
    }

    .video-call-btn:disabled {
      background-color: #cccccc;
      cursor: not-allowed;
    }

    .error-message {
      color: red;
      margin-top: 10px;
    }
  `]
})
export class TelemedicineButtonComponent {
  @Input() appointmentId!: string;
  @Input() clinicId!: string;
  @Input() providerId!: string;
  @Input() patientId!: string;
  @Input() currentUserId!: string;
  @Input() currentUserName!: string;
  @Input() isProvider: boolean = false;

  isLoading = false;
  errorMessage = '';

  constructor(private telemedicineService: TelemedicineService) {}

  /**
   * Starts a video call for the appointment
   * 
   * Steps:
   * 1. Check if session exists for this appointment
   * 2. If not, create a new session
   * 3. Start the session (mark as in progress)
   * 4. Join the session to get access credentials
   * 5. Open the video call in a new window
   */
  async startVideoCall() {
    this.isLoading = true;
    this.errorMessage = '';

    try {
      // Step 1: Check if session already exists
      let session;
      try {
        session = await this.telemedicineService
          .getSessionByAppointmentId(this.appointmentId)
          .toPromise();
      } catch (notFoundError) {
        // Session doesn't exist, will create one
        session = null;
      }

      // Step 2: Create session if it doesn't exist
      if (!session) {
        session = await this.telemedicineService.createSession({
          appointmentId: this.appointmentId,
          clinicId: this.clinicId,
          providerId: this.providerId,
          patientId: this.patientId
        }).toPromise();
        console.log('Sessão de telemedicina criada:', session);
      }

      // Step 3: Start the session if it's scheduled
      if (session && session.status === 'Scheduled') {
        session = await this.telemedicineService
          .startSession(session.id)
          .toPromise();
        console.log('Sessão iniciada:', session);
      }

      // Step 4: Join the session
      if (session) {
        const joinInfo = await this.telemedicineService.joinSession(
          session.id,
          {
            userId: this.currentUserId,
            userName: this.currentUserName,
            role: this.isProvider ? ParticipantRole.Provider : ParticipantRole.Patient
          }
        ).toPromise();

        // Step 5: Open video call
        // Option 1: Open in new window
        window.open(joinInfo!.roomUrl, '_blank');

        // Option 2: Navigate to a dedicated telemedicine page
        // this.router.navigate(['/telemedicine', session.id]);

        console.log('Credenciais de acesso:', joinInfo);
      }
    } catch (error) {
      console.error('Erro ao iniciar videochamada:', error);
      this.errorMessage = 'Erro ao iniciar videochamada. Tente novamente.';
    } finally {
      this.isLoading = false;
    }
  }
}
```

## Como Usar o Componente

### 1. Em um template HTML

```html
<app-telemedicine-button
  [appointmentId]="appointment.id"
  [clinicId]="appointment.clinicId"
  [providerId]="appointment.providerId"
  [patientId]="appointment.patientId"
  [currentUserId]="currentUser.id"
  [currentUserName]="currentUser.name"
  [isProvider]="currentUser.role === 'doctor'">
</app-telemedicine-button>
```

### 2. Importar no módulo ou componente pai

```typescript
import { TelemedicineButtonComponent } from './components/telemedicine-button.component';

@Component({
  // ...
  imports: [
    CommonModule,
    TelemedicineButtonComponent, // Para componentes standalone
    // ...
  ]
})
```

## Fluxo de Uso

1. **Usuário clica no botão** "Iniciar Videochamada"
2. **Sistema verifica** se já existe uma sessão para o agendamento
3. **Se não existir**, cria uma nova sessão de telemedicina
4. **Inicia a sessão** (marca como "Em Progresso")
5. **Gera credenciais** de acesso para o usuário atual
6. **Abre a videochamada** em uma nova janela do navegador

## Customizações Possíveis

### Abrir em Modal

Em vez de abrir em nova janela, você pode abrir em um modal:

```typescript
// Usar Daily.co iframe em modal
import DailyIframe from '@daily-co/daily-js';

const callFrame = DailyIframe.createFrame({
  iframeStyle: {
    width: '100%',
    height: '600px',
  }
});

await callFrame.join({
  url: joinInfo.roomUrl,
  token: joinInfo.accessToken
});
```

### Adicionar Callback ao Finalizar

```typescript
async endVideoCall(sessionId: string) {
  await this.telemedicineService.completeSession(sessionId, {
    notes: 'Consulta realizada com sucesso'
  }).toPromise();
  
  // Redirecionar ou atualizar UI
  this.router.navigate(['/appointments']);
}
```

### Exibir Status da Sessão

```typescript
async checkSessionStatus(appointmentId: string) {
  const session = await this.telemedicineService
    .getSessionByAppointmentId(appointmentId)
    .toPromise();
  
  switch (session?.status) {
    case 'Scheduled':
      return 'Agendada';
    case 'InProgress':
      return 'Em andamento';
    case 'Completed':
      return 'Concluída';
    case 'Cancelled':
      return 'Cancelada';
    default:
      return 'Sem sessão';
  }
}
```

## Considerações de Segurança

1. **Autenticação**: Certifique-se de que o usuário está autenticado antes de permitir acesso
2. **Autorização**: Verifique se o usuário tem permissão para acessar aquele agendamento
3. **Tenant ID**: O serviço automaticamente adiciona o X-Tenant-Id do localStorage
4. **Tokens temporários**: Os tokens de acesso à sala expiram em 120 minutos

## Testes

Exemplo de teste unitário para o componente:

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TelemedicineButtonComponent } from './telemedicine-button.component';
import { TelemedicineService } from '../../services/telemedicine.service';
import { of, throwError } from 'rxjs';

describe('TelemedicineButtonComponent', () => {
  let component: TelemedicineButtonComponent;
  let fixture: ComponentFixture<TelemedicineButtonComponent>;
  let telemedicineService: jasmine.SpyObj<TelemedicineService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('TelemedicineService', [
      'createSession',
      'joinSession',
      'startSession',
      'getSessionByAppointmentId'
    ]);

    await TestBed.configureTestingModule({
      imports: [TelemedicineButtonComponent],
      providers: [
        { provide: TelemedicineService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TelemedicineButtonComponent);
    component = fixture.componentInstance;
    telemedicineService = TestBed.inject(TelemedicineService) as jasmine.SpyObj<TelemedicineService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start video call successfully', async () => {
    const mockSession = { id: '123', status: 'Scheduled' };
    const mockJoinInfo = { roomUrl: 'https://daily.co/room', accessToken: 'token' };

    telemedicineService.getSessionByAppointmentId.and.returnValue(throwError(() => new Error('Not found')));
    telemedicineService.createSession.and.returnValue(of(mockSession as any));
    telemedicineService.startSession.and.returnValue(of({ ...mockSession, status: 'InProgress' } as any));
    telemedicineService.joinSession.and.returnValue(of(mockJoinInfo as any));

    spyOn(window, 'open');

    await component.startVideoCall();

    expect(window.open).toHaveBeenCalledWith(mockJoinInfo.roomUrl, '_blank');
  });
});
```

## Recursos Adicionais

- [Documentação da API de Telemedicina](/docs/FRONTEND_TELEMEDICINE_INTEGRATION.md)
- [Daily.co JavaScript SDK](https://docs.daily.co/reference/daily-js)
- [Angular HttpClient Guide](https://angular.io/guide/http)
