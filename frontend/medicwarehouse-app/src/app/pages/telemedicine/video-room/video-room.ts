import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TelemedicineService } from '../../../services/telemedicine.service';
import { TelemedicineSession, ParticipantRole } from '../../../models/telemedicine.model';
import { Auth } from '../../../services/auth';

declare var DailyIframe: any;

@Component({
  selector: 'app-video-room',
  imports: [CommonModule],
  templateUrl: './video-room.html',
  styleUrl: './video-room.scss'
})
export class VideoRoom implements OnInit, OnDestroy {
  session = signal<TelemedicineSession | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string>('');
  sessionId: string = '';
  callFrame: any = null;
  isAudioMuted = signal<boolean>(false);
  isVideoOff = signal<boolean>(false);
  sessionStartTime: Date | null = null;
  elapsedTime = signal<string>('00:00');
  timerInterval: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private telemedicineService: TelemedicineService,
    private auth: Auth
  ) {}

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('id') || '';
    
    if (!this.sessionId) {
      this.errorMessage.set('ID da sessão inválido');
      this.isLoading.set(false);
      return;
    }
    
    this.loadSessionAndJoin();
  }

  ngOnDestroy(): void {
    this.leaveCall();
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  loadSessionAndJoin(): void {
    this.telemedicineService.getSessionById(this.sessionId).subscribe({
      next: (session) => {
        this.session.set(session);
        this.joinSession();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar sessão');
        this.isLoading.set(false);
        console.error('Error loading session:', error);
      }
    });
  }

  joinSession(): void {
    const user = this.auth.currentUser();
    if (!user) {
      this.errorMessage.set('Usuário não autenticado');
      this.isLoading.set(false);
      return;
    }

    const joinRequest = {
      userId: user.username,
      userName: user.username,
      role: ParticipantRole.Provider // TODO: Determine role based on user type
    };

    this.telemedicineService.joinSession(this.sessionId, joinRequest).subscribe({
      next: (response) => {
        this.initializeVideoCall(response.roomUrl, response.accessToken);
        this.startTimer();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao entrar na sessão');
        this.isLoading.set(false);
        console.error('Error joining session:', error);
      }
    });
  }

  initializeVideoCall(roomUrl: string, token: string): void {
    try {
      // Check if Daily.co script is loaded
      if (typeof DailyIframe === 'undefined') {
        this.loadDailyScript().then(() => {
          this.createCallFrame(roomUrl, token);
        });
      } else {
        this.createCallFrame(roomUrl, token);
      }
    } catch (error) {
      this.errorMessage.set('Erro ao inicializar videochamada');
      this.isLoading.set(false);
      console.error('Error initializing video call:', error);
    }
  }

  loadDailyScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.src = 'https://unpkg.com/@daily-co/daily-js';
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('Failed to load Daily.co script'));
      document.head.appendChild(script);
    });
  }

  createCallFrame(roomUrl: string, token: string): void {
    const container = document.getElementById('video-container');
    
    if (!container) {
      this.errorMessage.set('Contêiner de vídeo não encontrado');
      this.isLoading.set(false);
      return;
    }

    this.callFrame = DailyIframe.createFrame(container, {
      showLeaveButton: false,
      iframeStyle: {
        width: '100%',
        height: '100%',
        border: '0',
        borderRadius: '8px'
      }
    });

    this.callFrame.join({ url: roomUrl, token: token })
      .then(() => {
        this.isLoading.set(false);
        this.setupEventListeners();
      })
      .catch((error: any) => {
        this.errorMessage.set('Erro ao entrar na sala');
        this.isLoading.set(false);
        console.error('Error joining call:', error);
      });
  }

  setupEventListeners(): void {
    if (!this.callFrame) return;

    this.callFrame.on('left-meeting', () => {
      this.handleEndSession();
    });

    this.callFrame.on('error', (error: any) => {
      console.error('Call error:', error);
    });
  }

  toggleAudio(): void {
    if (!this.callFrame) return;
    
    const muted = !this.isAudioMuted();
    this.callFrame.setLocalAudio(!muted);
    this.isAudioMuted.set(muted);
  }

  toggleVideo(): void {
    if (!this.callFrame) return;
    
    const videoOff = !this.isVideoOff();
    this.callFrame.setLocalVideo(!videoOff);
    this.isVideoOff.set(videoOff);
  }

  leaveCall(): void {
    if (this.callFrame) {
      this.callFrame.leave();
      this.callFrame.destroy();
      this.callFrame = null;
    }
  }

  endSession(): void {
    if (confirm('Tem certeza que deseja encerrar a sessão?')) {
      this.leaveCall();
      
      this.telemedicineService.completeSession(this.sessionId).subscribe({
        next: () => {
          this.router.navigate(['/telemedicine']);
        },
        error: (error) => {
          console.error('Error completing session:', error);
          this.router.navigate(['/telemedicine']);
        }
      });
    }
  }

  handleEndSession(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    this.router.navigate(['/telemedicine']);
  }

  startTimer(): void {
    this.sessionStartTime = new Date();
    
    this.timerInterval = setInterval(() => {
      if (this.sessionStartTime) {
        const elapsed = Math.floor((Date.now() - this.sessionStartTime.getTime()) / 1000);
        const minutes = Math.floor(elapsed / 60);
        const seconds = elapsed % 60;
        this.elapsedTime.set(
          `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
        );
      }
    }, 1000);
  }
}
