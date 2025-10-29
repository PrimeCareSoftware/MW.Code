# 🎥 Frontend - Componente de Telemedicina

Exemplo de integração do frontend Angular com o microserviço de telemedicina.

## 📦 Instalação

```bash
cd frontend/medicwarehouse-app
npm install @daily-co/daily-js --save
```

## 🧩 Componente TelemedicineComponent

### 1. Criar o componente

```bash
ng generate component components/telemedicine
```

### 2. telemedicine.component.ts

```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import DailyIframe, { DailyCall } from '@daily-co/daily-js';
import { TelemedicineService } from '../../services/telemedicine.service';

@Component({
  selector: 'app-telemedicine',
  templateUrl: './telemedicine.component.html',
  styleUrls: ['./telemedicine.component.css']
})
export class TelemedicineComponent implements OnInit, OnDestroy {
  private callFrame: DailyCall | null = null;
  sessionId: string = '';
  isLoading = true;
  isInCall = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private telemedicineService: TelemedicineService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.sessionId = params['sessionId'];
      this.initializeCall();
    });
  }

  ngOnDestroy(): void {
    this.leaveCall();
  }

  async initializeCall(): Promise<void> {
    try {
      this.isLoading = true;
      this.error = null;

      // Get join info from backend
      const joinInfo = await this.telemedicineService.joinSession(
        this.sessionId,
        this.getUserId(),
        this.getUserName(),
        this.getUserRole()
      );

      // Create Daily iframe
      this.callFrame = DailyIframe.createFrame({
        iframeStyle: {
          position: 'absolute',
          top: '0',
          left: '0',
          width: '100%',
          height: '100%',
          border: 'none'
        },
        showLeaveButton: true,
        showFullscreenButton: true
      });

      // Set up event listeners
      this.callFrame
        .on('joined-meeting', () => {
          console.log('Joined meeting');
          this.isInCall = true;
          this.isLoading = false;
        })
        .on('left-meeting', () => {
          console.log('Left meeting');
          this.isInCall = false;
          this.handleCallEnded();
        })
        .on('error', (error) => {
          console.error('Daily error:', error);
          this.error = 'Erro ao conectar com a videochamada';
          this.isLoading = false;
        });

      // Join the call
      await this.callFrame.join({
        url: joinInfo.roomUrl,
        token: joinInfo.accessToken
      });

    } catch (error) {
      console.error('Error initializing call:', error);
      this.error = 'Falha ao iniciar a consulta. Tente novamente.';
      this.isLoading = false;
    }
  }

  async leaveCall(): Promise<void> {
    if (this.callFrame) {
      await this.callFrame.leave();
      this.callFrame.destroy();
      this.callFrame = null;
    }
  }

  async handleCallEnded(): Promise<void> {
    // Complete session on backend
    try {
      await this.telemedicineService.completeSession(this.sessionId, 'Consulta finalizada');
    } catch (error) {
      console.error('Error completing session:', error);
    }
    
    // Navigate back
    // this.router.navigate(['/appointments']);
  }

  private getUserId(): string {
    // Get from AuthService
    return localStorage.getItem('userId') || '';
  }

  private getUserName(): string {
    // Get from AuthService
    return localStorage.getItem('userName') || 'Usuário';
  }

  private getUserRole(): 'provider' | 'patient' {
    // Determine role based on user type
    const userType = localStorage.getItem('userType');
    return userType === 'doctor' || userType === 'dentist' ? 'provider' : 'patient';
  }
}
```

### 3. telemedicine.component.html

```html
<div class="telemedicine-container">
  <!-- Loading State -->
  <div *ngIf="isLoading" class="loading-overlay">
    <div class="spinner"></div>
    <p>Conectando à consulta...</p>
  </div>

  <!-- Error State -->
  <div *ngIf="error" class="error-message">
    <mat-icon>error</mat-icon>
    <h3>{{ error }}</h3>
    <button mat-raised-button color="primary" (click)="initializeCall()">
      Tentar Novamente
    </button>
  </div>

  <!-- Call Container -->
  <div id="call-container" *ngIf="isInCall && !error">
    <!-- Daily.co iframe will be inserted here -->
  </div>

  <!-- Call Controls (Optional) -->
  <div class="call-controls" *ngIf="isInCall">
    <button mat-fab color="warn" (click)="leaveCall()" matTooltip="Encerrar Consulta">
      <mat-icon>call_end</mat-icon>
    </button>
  </div>
</div>
```

### 4. telemedicine.component.css

```css
.telemedicine-container {
  position: relative;
  width: 100%;
  height: 100vh;
  background-color: #000;
}

.loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: rgba(0, 0, 0, 0.8);
  color: white;
  z-index: 1000;
}

.spinner {
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid #fff;
  border-radius: 50%;
  width: 50px;
  height: 50px;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  text-align: center;
  color: white;
  z-index: 1000;
}

.error-message mat-icon {
  font-size: 64px;
  width: 64px;
  height: 64px;
  color: #f44336;
}

#call-container {
  width: 100%;
  height: 100%;
}

.call-controls {
  position: absolute;
  bottom: 30px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 100;
}
```

## 🔧 Serviço TelemedicineService

### telemedicine.service.ts

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export interface CreateSessionRequest {
  appointmentId: string;
  clinicId: string;
  providerId: string;
  patientId: string;
}

export interface SessionResponse {
  id: string;
  appointmentId: string;
  clinicId: string;
  roomUrl: string;
  status: string;
  createdAt: string;
  durationMinutes?: number;
  recordingUrl?: string;
}

export interface JoinSessionResponse {
  roomUrl: string;
  accessToken: string;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class TelemedicineService {
  private apiUrl = `${environment.telemedicineApiUrl}/api/sessions`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const tenantId = localStorage.getItem('tenantId') || '';
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Tenant-Id': tenantId
    });
  }

  createSession(request: CreateSessionRequest): Observable<SessionResponse> {
    return this.http.post<SessionResponse>(
      this.apiUrl,
      request,
      { headers: this.getHeaders() }
    );
  }

  async joinSession(
    sessionId: string,
    userId: string,
    userName: string,
    role: 'provider' | 'patient'
  ): Promise<JoinSessionResponse> {
    const url = `${this.apiUrl}/${sessionId}/join`;
    const body = { userId, userName, role };
    
    return firstValueFrom(
      this.http.post<JoinSessionResponse>(url, body, { headers: this.getHeaders() })
    );
  }

  startSession(sessionId: string): Observable<SessionResponse> {
    const url = `${this.apiUrl}/${sessionId}/start`;
    return this.http.post<SessionResponse>(url, null, { headers: this.getHeaders() });
  }

  async completeSession(sessionId: string, notes?: string): Promise<SessionResponse> {
    const url = `${this.apiUrl}/${sessionId}/complete`;
    const body = { notes };
    
    return firstValueFrom(
      this.http.post<SessionResponse>(url, body, { headers: this.getHeaders() })
    );
  }

  cancelSession(sessionId: string, reason: string): Observable<SessionResponse> {
    const url = `${this.apiUrl}/${sessionId}/cancel`;
    return this.http.post<SessionResponse>(url, reason, { headers: this.getHeaders() });
  }

  getSessionById(sessionId: string): Observable<SessionResponse> {
    const url = `${this.apiUrl}/${sessionId}`;
    return this.http.get<SessionResponse>(url, { headers: this.getHeaders() });
  }

  getSessionByAppointmentId(appointmentId: string): Observable<SessionResponse> {
    const url = `${this.apiUrl}/appointment/${appointmentId}`;
    return this.http.get<SessionResponse>(url, { headers: this.getHeaders() });
  }

  getClinicSessions(clinicId: string, skip: number = 0, take: number = 50): Observable<SessionResponse[]> {
    const url = `${this.apiUrl}/clinic/${clinicId}?skip=${skip}&take=${take}`;
    return this.http.get<SessionResponse[]>(url, { headers: this.getHeaders() });
  }

  getProviderSessions(providerId: string, skip: number = 0, take: number = 50): Observable<SessionResponse[]> {
    const url = `${this.apiUrl}/provider/${providerId}?skip=${skip}&take=${take}`;
    return this.http.get<SessionResponse[]>(url, { headers: this.getHeaders() });
  }

  getPatientSessions(patientId: string, skip: number = 0, take: number = 50): Observable<SessionResponse[]> {
    const url = `${this.apiUrl}/patient/${patientId}?skip=${skip}&take=${take}`;
    return this.http.get<SessionResponse[]>(url, { headers: this.getHeaders() });
  }
}
```

## 🌐 Configuração de Environment

### environment.ts

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  telemedicineApiUrl: 'http://localhost:5001/api'  // Adicionar esta linha
};
```

### environment.production.ts

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com',
  telemedicineApiUrl: 'https://telemedicine.medicwarehouse.com'  // Adicionar esta linha
};
```

## 🎯 Uso nos Appointments

### appointments.component.html

Adicionar botão de telemedicina:

```html
<button 
  mat-raised-button 
  color="primary"
  (click)="startTelemedicine(appointment.id)"
  [disabled]="!appointment.isTelemedicine">
  <mat-icon>videocam</mat-icon>
  Iniciar Consulta Online
</button>
```

### appointments.component.ts

```typescript
async startTelemedicine(appointmentId: string): Promise<void> {
  try {
    // Check if session exists
    let session = await firstValueFrom(
      this.telemedicineService.getSessionByAppointmentId(appointmentId)
    );

    // Create session if not exists
    if (!session) {
      const request: CreateSessionRequest = {
        appointmentId: appointmentId,
        clinicId: this.clinicId,
        providerId: this.providerId,
        patientId: this.patientId
      };
      session = await firstValueFrom(
        this.telemedicineService.createSession(request)
      );
    }

    // Start session
    await firstValueFrom(
      this.telemedicineService.startSession(session.id)
    );

    // Navigate to telemedicine component
    this.router.navigate(['/telemedicine', session.id]);

  } catch (error) {
    console.error('Error starting telemedicine:', error);
    this.snackBar.open('Erro ao iniciar consulta online', 'Fechar', { duration: 3000 });
  }
}
```

## 📱 Roteamento

### app-routing.module.ts

```typescript
const routes: Routes = [
  // ... outras rotas
  {
    path: 'telemedicine/:sessionId',
    component: TelemedicineComponent,
    canActivate: [AuthGuard]
  }
];
```

## ✅ Checklist de Implementação

- [ ] Instalar `@daily-co/daily-js`
- [ ] Criar `TelemedicineComponent`
- [ ] Criar `TelemedicineService`
- [ ] Configurar `environment.ts`
- [ ] Adicionar rota para telemedicina
- [ ] Adicionar botão nos appointments
- [ ] Testar fluxo completo
- [ ] Adicionar tratamento de erros
- [ ] Implementar notificações
- [ ] Documentar para usuários finais

## 🎨 Melhorias Futuras

- [ ] Preview de vídeo antes de entrar
- [ ] Chat durante a consulta
- [ ] Compartilhamento de tela
- [ ] Gravação local opcional
- [ ] Verificação de câmera/microfone
- [ ] Qualidade de conexão indicator
- [ ] Modo espera (waiting room)
- [ ] Notificações de entrada/saída

## 📚 Recursos

- [Daily.co React SDK](https://docs.daily.co/reference/daily-react)
- [Daily.co JavaScript SDK](https://docs.daily.co/reference/daily-js)
- [Angular HTTP Client](https://angular.io/guide/http)

---

**Nota**: Este é um exemplo básico. Ajuste conforme necessário para seu caso de uso específico.
