# 🎥 Teleatendimento para Profissionais Autônomos
## Especificações Técnicas e Modelo de Negócio

> **Data:** 26 de Janeiro de 2026  
> **Versão:** 1.0  
> **Status:** Especificação Técnica  
> **Objetivo:** Detalhar implementação de teleatendimento para profissionais sem consultório físico

---

## 📋 Sumário Executivo

Este documento especifica as adaptações necessárias no sistema de telemedicina do Omni Care para atender profissionais autônomos que trabalham **100% online**, sem consultório físico ou com consultório compartilhado. Este segmento representa aproximadamente **35-40%** dos profissionais de saúde no Brasil (principalmente psicólogos, nutricionistas e coaches).

### Contexto
- **Profissionais Autônomos:** ~150.000 no Brasil (35% do total)
- **Sem CNPJ:** ~70.000 (trabalham apenas com CPF)
- **Sem Consultório Fixo:** ~90.000 (consultórios compartilhados ou home office)
- **Demanda por Teleatendimento:** ~120.000 (80% dos autônomos)

---

## 🎯 Personas e Casos de Uso

### Persona 1: Psicólogo Autônomo 100% Online

**Perfil:**
- Nome: Julia, 28 anos, CRP 06/123456
- Formação: Psicologia, especialização em TCC
- Situação: Recém-formada, trabalha de casa
- Documento: Apenas CPF (não tem CNPJ)
- Estrutura: Sem consultório físico
- Atendimentos: 100% online (15-20 sessões/semana)

**Necessidades:**
1. **Sala Virtual Permanente**
   - Link fixo para suas sessões (ex: omnicare.com.br/julia.silva)
   - Personalização (logo, cores, mensagem de boas-vindas)
   - Disponível 24/7

2. **Agenda Online Pública**
   - Pacientes agendam direto sem intermediários
   - Sincronização com Google Calendar
   - Bloqueio automático de horários

3. **Sala de Espera Virtual**
   - Paciente entra 5 min antes
   - Música ambiente relaxante
   - Timer mostrando tempo de espera

4. **Videochamada de Alta Qualidade**
   - HD 1080p (quando possível)
   - Ajuste automático de qualidade
   - Modo "retrato" (desfoque de fundo)
   - Gravação opcional (com consentimento)

5. **Prontuário Durante Sessão**
   - Anotações em tempo real
   - Atalhos de teclado
   - Não visível para paciente

6. **Pagamento Online**
   - PIX integrado
   - Cartão de crédito (split automático)
   - Recibos automáticos

7. **Lembretes Automáticos**
   - WhatsApp 24h antes
   - Email 1h antes
   - SMS 30min antes (opcional)

**Fluxo Típico:**
```
1. Paciente agenda online → 2. Recebe confirmação + link
3. 24h antes: lembrete WhatsApp → 4. 1h antes: lembrete email
5. Paciente entra na sala de espera → 6. Psicóloga é notificada
7. Psicóloga inicia chamada → 8. Sessão de 50min
9. Psicóloga anota em prontuário → 10. Paciente paga online
11. Recibo enviado automaticamente → 12. Agendamento da próxima sessão
```

---

### Persona 2: Nutricionista Híbrida

**Perfil:**
- Nome: Pedro, 35 anos, CRN 3/45678
- Situação: Atende em consultório compartilhado (2 dias/semana) + online (3 dias/semana)
- Documento: CNPJ (MEI)
- Estrutura: Consultório compartilhado (paga por uso)
- Atendimentos: 60% online, 40% presencial

**Necessidades:**
1. **Modo Híbrido**
   - Agenda marca se atendimento é presencial ou online
   - Link de videochamada enviado apenas para online
   - Endereço do consultório enviado para presencial

2. **Flexibilidade de Local**
   - Pode atender de qualquer lugar (casa, consultório, viagem)
   - Notificação de qual dispositivo está usando

3. **Compartilhamento de Tela**
   - Mostrar planilhas nutricionais
   - Planos alimentares
   - Gráficos de evolução

4. **Envio de Arquivos Durante Consulta**
   - PDF com dieta
   - Receitas
   - Lista de compras

5. **Fotos de Progressão**
   - Paciente envia fotos (antes/durante/depois)
   - Galeria organizada por data
   - Comparação lado a lado

**Fluxo Típico Online:**
```
1. Paciente agenda "Consulta Online de Retorno"
2. Sistema envia link 1h antes
3. Nutricionista entra de casa (notebook)
4. Consulta com compartilhamento de plano alimentar
5. Envia PDF durante a chamada
6. Agenda próxima consulta (presencial)
```

---

### Persona 3: Fisioterapeuta Domiciliar + Online

**Perfil:**
- Nome: Carlos, 42 anos, CREFITO 2/34567
- Situação: Atende em domicílio (idosos) + online (pós-consultas)
- Documento: CNPJ (ME)
- Estrutura: Sem consultório, atende na casa do paciente
- Atendimentos: 70% domiciliar, 30% online (acompanhamento)

**Necessidades:**
1. **Atendimento Domiciliar no Sistema**
   - Tipo de atendimento: "Domiciliar"
   - Endereço do paciente registrado
   - Tempo de deslocamento calculado
   - Rota no Google Maps

2. **Videochamadas de Acompanhamento**
   - Sessões curtas (15-20min)
   - Paciente mostra exercícios
   - Fisioterapeuta corrige em tempo real

3. **Gravação para Análise**
   - Paciente se filma fazendo exercícios
   - Fisioterapeuta analisa depois
   - Feedback por vídeo assíncrono

**Fluxo Típico:**
```
1. Primeira consulta: domiciliar (presencial)
2. Sistema registra endereço do paciente
3. Após 1 semana: consulta online de acompanhamento (15min)
4. Paciente mostra exercícios pela câmera
5. Fisioterapeuta corrige postura
6. Próximo atendimento: domiciliar (daqui a 2 semanas)
```

---

## 🏗️ Arquitetura Técnica

### Componentes Principais

#### 1. Sala Virtual Permanente

**Conceito:**
Cada profissional tem uma "sala virtual" fixa, como se fosse um consultório físico no mundo digital.

**URL Personalizada:**
```
https://omnicare.com.br/sala/{username}
OU
https://{subdomain}.omnicare.com.br
```

**Exemplos:**
- `https://omnicare.com.br/sala/dra.julia.psicologa`
- `https://juliasilva.omnicare.com.br`

**Funcionalidades:**
- ✅ Sempre disponível (24/7)
- ✅ Sala de espera virtual
- ✅ Branding personalizado (logo, cores)
- ✅ Mensagem de boas-vindas customizável
- ✅ Música ambiente (opcional)
- ✅ Vídeo de apresentação do profissional
- ✅ Informações de contato (email, WhatsApp)

**Implementação Backend:**

```csharp
// Domain Entity
public class VirtualRoom : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Slug { get; private set; } // URL-friendly name
    public string? Subdomain { get; private set; } // Optional custom subdomain
    
    // Branding
    public string? LogoUrl { get; private set; }
    public string? PrimaryColor { get; private set; }
    public string? SecondaryColor { get; private set; }
    public string? WelcomeMessage { get; private set; }
    
    // Waiting Room
    public bool EnableWaitingRoom { get; private set; } = true;
    public bool EnableBackgroundMusic { get; private set; } = true;
    public string? MusicUrl { get; private set; } // URL to background music
    
    // Settings
    public bool IsActive { get; private set; } = true;
    public int MaxWaitingTimeMinutes { get; private set; } = 15;
    
    // Navigation
    public User User { get; private set; }
    
    public VirtualRoom(Guid userId, string slug, string tenantId) : base(tenantId)
    {
        UserId = userId;
        Slug = NormalizeSlug(slug);
        EnableWaitingRoom = true;
        IsActive = true;
    }
    
    public void UpdateBranding(string? logoUrl, string? primaryColor, 
        string? secondaryColor, string? welcomeMessage)
    {
        LogoUrl = logoUrl;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        WelcomeMessage = welcomeMessage;
        UpdateTimestamp();
    }
    
    public void SetSubdomain(string subdomain)
    {
        // Validate subdomain availability
        Subdomain = subdomain.ToLowerInvariant();
        UpdateTimestamp();
    }
    
    private static string NormalizeSlug(string slug)
    {
        // Remove special characters, convert to lowercase, replace spaces with hyphens
        return slug.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "-");
    }
}
```

**Implementação Frontend (Angular):**

```typescript
// virtual-room.component.ts
@Component({
  selector: 'app-virtual-room',
  templateUrl: './virtual-room.component.html',
  styleUrls: ['./virtual-room.component.scss']
})
export class VirtualRoomComponent implements OnInit {
  room: VirtualRoom;
  isWaiting: boolean = true;
  waitingTimeMinutes: number = 0;
  
  constructor(
    private route: ActivatedRoute,
    private virtualRoomService: VirtualRoomService,
    private audioService: AudioService
  ) {}
  
  ngOnInit() {
    const slug = this.route.snapshot.params['slug'];
    this.loadRoom(slug);
    this.startWaitingTimer();
    
    // Play background music if enabled
    if (this.room.enableBackgroundMusic && this.room.musicUrl) {
      this.audioService.play(this.room.musicUrl, { loop: true, volume: 0.3 });
    }
  }
  
  async loadRoom(slug: string) {
    this.room = await this.virtualRoomService.getBySlug(slug);
    this.applyBranding();
  }
  
  applyBranding() {
    if (this.room.primaryColor) {
      document.documentElement.style.setProperty('--primary-color', this.room.primaryColor);
    }
    if (this.room.secondaryColor) {
      document.documentElement.style.setProperty('--secondary-color', this.room.secondaryColor);
    }
  }
  
  startWaitingTimer() {
    setInterval(() => {
      this.waitingTimeMinutes++;
      if (this.waitingTimeMinutes >= this.room.maxWaitingTimeMinutes) {
        this.showTimeoutWarning();
      }
    }, 60000); // Every minute
  }
}
```

---

#### 2. Videochamada Aprimorada

**Requisitos Técnicos:**

| Requisito | Especificação |
|-----------|--------------|
| **Resolução** | HD 1080p (adaptável) |
| **Frame Rate** | 30 FPS |
| **Codec Vídeo** | VP8/VP9 ou H.264 |
| **Codec Áudio** | Opus (48kHz) |
| **Bitrate Vídeo** | 500 kbps - 2.5 Mbps (adaptável) |
| **Bitrate Áudio** | 32-128 kbps |
| **Latência** | < 200ms |
| **Perda de Pacotes** | Tolerância até 5% |

**Funcionalidades Avançadas:**

1. **Ajuste Automático de Qualidade**
```typescript
class AdaptiveQualityManager {
  private currentQuality: VideoQuality = 'high';
  private bandwidthMonitor: BandwidthMonitor;
  
  constructor(private peerConnection: RTCPeerConnection) {
    this.bandwidthMonitor = new BandwidthMonitor(peerConnection);
    this.startMonitoring();
  }
  
  private startMonitoring() {
    setInterval(() => {
      const bandwidth = this.bandwidthMonitor.getCurrentBandwidth();
      const packetLoss = this.bandwidthMonitor.getPacketLoss();
      
      if (bandwidth < 500 || packetLoss > 5) {
        this.downgradeQuality();
      } else if (bandwidth > 2000 && packetLoss < 1) {
        this.upgradeQuality();
      }
    }, 5000); // Check every 5 seconds
  }
  
  private downgradeQuality() {
    const qualities: VideoQuality[] = ['high', 'medium', 'low'];
    const currentIndex = qualities.indexOf(this.currentQuality);
    if (currentIndex < qualities.length - 1) {
      this.currentQuality = qualities[currentIndex + 1];
      this.applyQuality(this.currentQuality);
    }
  }
  
  private upgradeQuality() {
    // Similar logic for upgrading
  }
  
  private applyQuality(quality: VideoQuality) {
    const constraints = this.getConstraintsForQuality(quality);
    this.peerConnection.getSenders()[0].setParameters({
      encodings: [constraints]
    });
  }
  
  private getConstraintsForQuality(quality: VideoQuality) {
    switch(quality) {
      case 'high':
        return { maxBitrate: 2500000, maxFramerate: 30 };
      case 'medium':
        return { maxBitrate: 1000000, maxFramerate: 24 };
      case 'low':
        return { maxBitrate: 500000, maxFramerate: 15 };
    }
  }
}
```

2. **Modo Retrato (Desfoque de Fundo)**
```typescript
class BackgroundBlurFilter {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private bodyPixModel: any; // TensorFlow.js BodyPix model
  
  async initialize() {
    // Load TensorFlow.js BodyPix model
    this.bodyPixModel = await bodyPix.load({
      architecture: 'MobileNetV1',
      outputStride: 16,
      multiplier: 0.75,
      quantBytes: 2
    });
  }
  
  async applyBlur(videoElement: HTMLVideoElement): Promise<MediaStream> {
    const segmentation = await this.bodyPixModel.segmentPerson(videoElement);
    
    // Draw blurred background + sharp foreground
    const backgroundBlurAmount = 15;
    const edgeBlurAmount = 3;
    const flipHorizontal = false;
    
    bodyPix.drawBokehEffect(
      this.canvas,
      videoElement,
      segmentation,
      backgroundBlurAmount,
      edgeBlurAmount,
      flipHorizontal
    );
    
    return this.canvas.captureStream(30);
  }
}
```

3. **Gravação de Sessão (Opcional)**

```typescript
class SessionRecorder {
  private mediaRecorder: MediaRecorder;
  private recordedChunks: Blob[] = [];
  private patientConsent: boolean = false;
  
  async startRecording(stream: MediaStream, patientConsent: boolean) {
    if (!patientConsent) {
      throw new Error('Patient consent required for recording');
    }
    
    this.patientConsent = true;
    this.recordedChunks = [];
    
    this.mediaRecorder = new MediaRecorder(stream, {
      mimeType: 'video/webm;codecs=vp9',
      videoBitsPerSecond: 2500000
    });
    
    this.mediaRecorder.ondataavailable = (event) => {
      if (event.data.size > 0) {
        this.recordedChunks.push(event.data);
      }
    };
    
    this.mediaRecorder.start(1000); // Capture every 1 second
  }
  
  async stopRecording(): Promise<Blob> {
    return new Promise((resolve) => {
      this.mediaRecorder.onstop = () => {
        const blob = new Blob(this.recordedChunks, { type: 'video/webm' });
        resolve(blob);
      };
      this.mediaRecorder.stop();
    });
  }
  
  async uploadRecording(blob: Blob, appointmentId: string) {
    // Encrypt recording before upload
    const encryptedBlob = await this.encryptBlob(blob);
    
    // Upload to secure storage (Azure Blob Storage with encryption)
    await this.storageService.upload(encryptedBlob, {
      container: 'session-recordings',
      metadata: {
        appointmentId,
        recordedAt: new Date().toISOString(),
        patientConsent: true,
        expiresAt: this.getExpirationDate() // Auto-delete after 5 years (LGPD)
      }
    });
  }
  
  private async encryptBlob(blob: Blob): Promise<Blob> {
    // AES-256-GCM encryption
    const key = await this.getEncryptionKey();
    const arrayBuffer = await blob.arrayBuffer();
    const iv = window.crypto.getRandomValues(new Uint8Array(12));
    
    const encryptedData = await window.crypto.subtle.encrypt(
      { name: 'AES-GCM', iv },
      key,
      arrayBuffer
    );
    
    return new Blob([iv, encryptedData]);
  }
}
```

---

#### 3. Chat Durante Videochamada

**Casos de Uso:**
- Profissional envia links (exercícios, artigos)
- Paciente faz perguntas por escrito (se tímido)
- Compartilhamento de código (dietas, protocolos)

**Implementação:**

```typescript
interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  senderType: 'professional' | 'patient';
  message: string;
  timestamp: Date;
  type: 'text' | 'link' | 'file';
  fileUrl?: string;
}

class VideoChatService {
  private messages: ChatMessage[] = [];
  private messageSubject = new Subject<ChatMessage>();
  public messages$ = this.messageSubject.asObservable();
  
  sendMessage(content: string, type: 'text' | 'link' = 'text') {
    const message: ChatMessage = {
      id: uuidv4(),
      senderId: this.currentUserId,
      senderName: this.currentUserName,
      senderType: this.currentUserType,
      message: content,
      timestamp: new Date(),
      type
    };
    
    this.messages.push(message);
    this.messageSubject.next(message);
    
    // Send via WebSocket to other participant
    this.websocketService.send('chat-message', message);
  }
  
  async sendFile(file: File) {
    // Upload file to temporary storage
    const url = await this.uploadFile(file);
    
    const message: ChatMessage = {
      id: uuidv4(),
      senderId: this.currentUserId,
      senderName: this.currentUserName,
      senderType: this.currentUserType,
      message: file.name,
      timestamp: new Date(),
      type: 'file',
      fileUrl: url
    };
    
    this.messages.push(message);
    this.messageSubject.next(message);
    this.websocketService.send('chat-message', message);
  }
}
```

---

#### 4. Compartilhamento de Tela

**Casos de Uso:**
- Nutricionista mostra plano alimentar
- Psicólogo mostra exercícios terapêuticos
- Fisioterapeuta mostra vídeos de exercícios

**Implementação:**

```typescript
class ScreenShareService {
  private screenStream: MediaStream | null = null;
  
  async startScreenShare(): Promise<MediaStream> {
    try {
      this.screenStream = await navigator.mediaDevices.getDisplayMedia({
        video: {
          cursor: 'always',
          displaySurface: 'monitor'
        },
        audio: false
      });
      
      // Detect when user stops sharing via browser button
      this.screenStream.getVideoTracks()[0].addEventListener('ended', () => {
        this.stopScreenShare();
      });
      
      return this.screenStream;
    } catch (error) {
      console.error('Error starting screen share:', error);
      throw new Error('Failed to start screen sharing');
    }
  }
  
  stopScreenShare() {
    if (this.screenStream) {
      this.screenStream.getTracks().forEach(track => track.stop());
      this.screenStream = null;
    }
  }
  
  async switchToScreenShare(peerConnection: RTCPeerConnection) {
    const screenStream = await this.startScreenShare();
    const screenTrack = screenStream.getVideoTracks()[0];
    
    // Replace camera video with screen share
    const sender = peerConnection.getSenders().find(s => s.track?.kind === 'video');
    if (sender) {
      await sender.replaceTrack(screenTrack);
    }
  }
  
  async switchBackToCamera(peerConnection: RTCPeerConnection, cameraStream: MediaStream) {
    this.stopScreenShare();
    
    const cameraTrack = cameraStream.getVideoTracks()[0];
    const sender = peerConnection.getSenders().find(s => s.track?.kind === 'video');
    if (sender) {
      await sender.replaceTrack(cameraTrack);
    }
  }
}
```

---

## 💰 Modelo de Negócio para Profissionais Autônomos

### Planos Específicos

| Plano | Preço | Recursos | Público-Alvo |
|-------|-------|----------|--------------|
| **Solo Online** | R$ 69/mês | 1 profissional, teleatendimento ilimitado, agenda pública | Profissionais 100% online |
| **Solo Híbrido** | R$ 89/mês | 1 profissional, teleatendimento + presencial, 2 locais | Profissionais em consultório compartilhado |
| **Duo Online** | R$ 119/mês | 2 profissionais, teleatendimento ilimitado | Duplas de profissionais |

### Comparação com Concorrentes

| Feature | Omni Care Solo | Zenklub | Doctoralia |
|---------|----------------|---------|------------|
| **Preço** | R$ 69/mês | R$ 89/mês + 20% comissão | R$ 149/mês |
| **Comissão** | ❌ Nenhuma | ✅ 10-30% | ❌ Nenhuma |
| **Teleatendimento** | ✅ Ilimitado | ✅ Ilimitado | ❌ Não incluído |
| **Sala Virtual Própria** | ✅ Sim | ❌ Não (usa plataforma) | ❌ Não |
| **Agenda Pública** | ✅ Sim | ✅ Sim (só na plataforma) | ✅ Sim |
| **Marketplace** | ⚠️ Opcional | ✅ Obrigatório | ✅ Principal negócio |
| **Personalização** | ✅ Total (logo, cores) | ❌ Limitada | ❌ Padronizado |
| **Independência** | ✅ Total | ❌ Dependente | ⚠️ Parcial |

---

## 🔐 Segurança e Compliance

### LGPD e Dados Sensíveis

1. **Criptografia End-to-End**
   - Vídeo/áudio criptografados durante transmissão (DTLS-SRTP)
   - Gravações criptografadas em repouso (AES-256-GCM)
   - Chaves gerenciadas via Azure Key Vault

2. **Consentimento para Gravação**
   - Termo de consentimento específico
   - Aceite obrigatório antes de iniciar gravação
   - Indicador visual durante gravação (ponto vermelho)
   - Paciente pode revogar consentimento a qualquer momento

3. **Retenção e Exclusão**
   - Gravações mantidas por até 5 anos (conforme LGPD)
   - Exclusão automática após prazo
   - Exportação facilitada (direito à portabilidade)

4. **Auditoria**
   - Log de todos os acessos a gravações
   - Notificação ao paciente quando gravação é acessada
   - Relatório trimestral de acessos

---

## 📈 Métricas de Sucesso

### KPIs Técnicos

| Métrica | Meta | Medição |
|---------|------|---------|
| **Qualidade de Vídeo** | > 90% em HD | Telemetria em tempo real |
| **Latência** | < 200ms | Ping durante chamadas |
| **Taxa de Conexão** | > 98% | Sucesso vs. falhas |
| **Uptime** | > 99.9% | Monitoramento 24/7 |
| **Tempo de Carregamento** | < 3s | Métricas frontend |

### KPIs de Negócio

| Métrica | Meta Ano 1 | Medição |
|---------|------------|---------|
| **Profissionais 100% Online** | 1.000 | Cadastros com "sem consultório" |
| **Sessões Online/Mês** | 15.000 | Total de videochamadas |
| **NPS** | > 60 | Pesquisa pós-sessão |
| **Churn Mensal** | < 5% | Cancelamentos/mês |
| **Uso de Gravação** | 40% | % de sessões gravadas |

---

## 🚀 Roadmap de Implementação

### Fase 1: MVP (2 meses - Q1 2026)
**Investimento:** R$ 50.000

- ✅ Sala virtual permanente com URL personalizada
- ✅ Videochamada HD com ajuste automático
- ✅ Sala de espera virtual básica
- ✅ Chat durante chamada
- ✅ Compartilhamento de tela

### Fase 2: Aprimoramentos (2 meses - Q2 2026)
**Investimento:** R$ 30.000

- ✅ Modo retrato (desfoque de fundo)
- ✅ Gravação de sessão com consentimento
- ✅ Transcrição automática (IA)
- ✅ Personalização visual avançada (branding)

### Fase 3: Recursos Avançados (2 meses - Q3 2026)
**Investimento:** R$ 20.000

- ✅ Música ambiente na sala de espera
- ✅ Vídeo de apresentação do profissional
- ✅ Modo "somente áudio" (economia de banda)
- ✅ Quadro branco virtual (desenho colaborativo)

---

## 📞 Contato

**Equipe de Produto Omni Care**  
**Email:** produto@omnicare.com.br  
**Documentos Relacionados:**
- [PLANO_ADAPTACAO_MULTI_NEGOCIOS.md](./PLANO_ADAPTACAO_MULTI_NEGOCIOS.md)
- [ANALISE_MERCADO_SAAS_SAUDE.md](./ANALISE_MERCADO_SAAS_SAUDE.md)

---

> **Versão:** 1.0  
> **Data:** 26 de Janeiro de 2026  
> **Status:** Especificação Técnica Completa
