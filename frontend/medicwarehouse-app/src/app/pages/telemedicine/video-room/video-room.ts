import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TelemedicineService } from '../../../services/telemedicine.service';
import { JoinSessionResponse, ParticipantRole, TelemedicineSession } from '../../../models/telemedicine.model';
import { Auth } from '../../../services/auth';

declare global {
  interface Window {
    Twilio?: any;
  }
}

@Component({
  selector: 'app-video-room',
  imports: [CommonModule],
  templateUrl: './video-room.html',
  styleUrl: './video-room.scss'
})
export class VideoRoom implements OnInit, OnDestroy {
  session = signal<TelemedicineSession | null>(null);
  joinInfo = signal<JoinSessionResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string>('');
  sessionId = '';
  room: any = null;
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
      role: ParticipantRole.Provider
    };

    this.telemedicineService.joinSession(this.sessionId, joinRequest).subscribe({
      next: (response) => {
        this.joinInfo.set(response);
        this.initializeVideoCall(response);
        this.startTimer();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao entrar na sessão');
        this.isLoading.set(false);
        console.error('Error joining session:', error);
      }
    });
  }

  initializeVideoCall(response: JoinSessionResponse): void {
    try {
      if (response.provider?.toLowerCase() !== 'twilio') {
        this.errorMessage.set('Provedor de videochamada não suportado para esta sprint.');
        this.isLoading.set(false);
        return;
      }

      if (!window.Twilio) {
        this.loadTwilioScript().then(() => this.connectTwilioRoom(response));
      } else {
        this.connectTwilioRoom(response);
      }
    } catch (error) {
      this.errorMessage.set('Erro ao inicializar videochamada');
      this.isLoading.set(false);
      console.error('Error initializing video call:', error);
    }
  }

  loadTwilioScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      const existing = document.querySelector('script[data-provider="twilio-video"]');
      if (existing) {
        resolve();
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://sdk.twilio.com/js/video/releases/2.30.0/twilio-video.min.js';
      script.async = true;
      script.dataset['provider'] = 'twilio-video';
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('Failed to load Twilio Video script'));
      document.head.appendChild(script);
    });
  }

  connectTwilioRoom(response: JoinSessionResponse): void {
    const container = document.getElementById('video-container');
    if (!container) {
      this.errorMessage.set('Contêiner de vídeo não encontrado');
      this.isLoading.set(false);
      return;
    }

    window.Twilio.Video.connect(response.accessToken, {
      name: response.roomName,
      audio: true,
      video: true
    })
      .then((room: any) => {
        this.room = room;
        this.attachParticipants(container, room);
        this.setupTwilioEvents(room, container);
        this.isLoading.set(false);
      })
      .catch((error: any) => {
        this.errorMessage.set('Erro ao entrar na sala');
        this.isLoading.set(false);
        console.error('Error joining Twilio room:', error);
      });
  }

  attachParticipants(container: HTMLElement, room: any): void {
    container.innerHTML = '';

    room.localParticipant.videoTracks.forEach((publication: any) => {
      if (publication.track) {
        container.appendChild(publication.track.attach());
      }
    });

    room.participants.forEach((participant: any) => {
      this.attachParticipantTracks(participant, container);
    });
  }

  attachParticipantTracks(participant: any, container: HTMLElement): void {
    participant.tracks.forEach((publication: any) => {
      if (publication.track) {
        container.appendChild(publication.track.attach());
      }
    });

    participant.on('trackSubscribed', (track: any) => {
      container.appendChild(track.attach());
    });
  }

  setupTwilioEvents(room: any, container: HTMLElement): void {
    room.on('participantConnected', (participant: any) => {
      this.attachParticipantTracks(participant, container);
    });

    room.on('participantDisconnected', (participant: any) => {
      participant.tracks.forEach((publication: any) => {
        if (publication.track) {
          publication.track.detach().forEach((el: HTMLElement) => el.remove());
        }
      });
    });

    room.on('disconnected', () => {
      this.handleEndSession();
    });
  }

  toggleAudio(): void {
    if (!this.room) return;
    const muted = !this.isAudioMuted();
    this.room.localParticipant.audioTracks.forEach((publication: any) => {
      if (muted) {
        publication.track.disable();
      } else {
        publication.track.enable();
      }
    });
    this.isAudioMuted.set(muted);
  }

  toggleVideo(): void {
    if (!this.room) return;
    const off = !this.isVideoOff();
    this.room.localParticipant.videoTracks.forEach((publication: any) => {
      if (off) {
        publication.track.disable();
      } else {
        publication.track.enable();
      }
    });
    this.isVideoOff.set(off);
  }

  leaveCall(): void {
    if (this.room) {
      this.room.disconnect();
      this.room = null;
    }
  }

  endSession(): void {
    if (confirm('Tem certeza que deseja encerrar a sessão?')) {
      this.leaveCall();

      this.telemedicineService.completeSession(this.sessionId).subscribe({
        next: () => this.router.navigate(['/telemedicine']),
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
        this.elapsedTime.set(`${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`);
      }
    }, 1000);
  }
}
