# 📋 Prompt 14: Sistema de Fila de Espera Avançado

**Prioridade:** 🔥 P2 - Médio  
**Complexidade:** ⚡⚡ Média  
**Tempo Estimado:** 2-3 meses | 2 desenvolvedores  
**Custo:** R$ 90.000  
**Pré-requisitos:** Sistema de agendamentos funcionando

---

## 🎯 Objetivo

Implementar sistema completo de gestão de fila de espera com totem de autoatendimento, painel de TV em tempo real, sistema de priorização, e notificações automáticas aos pacientes.

---

## 📊 Contexto do Sistema

### Problema Atual
- Fila manual com fichas de papel
- Pacientes não sabem tempo de espera
- Sem sistema de priorização automático
- Recepção sobrecarregada chamando pacientes
- Sem rastreabilidade de tempo de atendimento

### Solução Proposta
- Totem touchscreen de autoatendimento
- Geração digital de senhas
- Painel de TV com atualização real-time (SignalR)
- Priorização automática (idosos, gestantes, deficientes)
- Estimativa inteligente de tempo de espera
- Notificações SMS/App quando próximo da vez
- Analytics de tempo de espera por especialidade

---

## 🏗️ Arquitetura da Solução

### 1. Backend - Gestão de Fila (4 semanas)

#### 1.1 Entidades do Domínio
```csharp
// src/MedicSoft.Core/Entities/Queue/FilaEspera.cs
public class FilaEspera
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public Clinica Clinica { get; set; }
    
    public string Nome { get; set; } // "Fila Geral", "Fila Pediatria", etc
    public TipoFila Tipo { get; set; }
    public bool Ativa { get; set; } = true;
    
    // Configurações
    public int TempoMedioAtendimento { get; set; } // minutos
    public bool UsaPrioridade { get; set; } = true;
    public bool UsaAgendamento { get; set; } = true;
    
    public List<SenhaFila> Senhas { get; set; }
}

public enum TipoFila
{
    Geral = 1,
    PorEspecialidade = 2,
    PorMedico = 3,
    Triagem = 4
}

public class SenhaFila
{
    public Guid Id { get; set; }
    public Guid FilaId { get; set; }
    public FilaEspera Fila { get; set; }
    
    // Dados do paciente
    public Guid? PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    public string NomePaciente { get; set; }
    public string CpfPaciente { get; set; }
    public string TelefonePaciente { get; set; }
    
    // Dados da senha
    public string NumeroSenha { get; set; } // "A001", "P042", etc
    public DateTime DataHoraEntrada { get; set; }
    public DateTime? DataHoraChamada { get; set; }
    public DateTime? DataHoraAtendimento { get; set; }
    public DateTime? DataHoraSaida { get; set; }
    
    // Prioridade
    public PrioridadeAtendimento Prioridade { get; set; }
    public string MotivoPrioridade { get; set; }
    
    // Status
    public StatusSenha Status { get; set; }
    public int TentativasChamada { get; set; } = 0;
    
    // Atendimento
    public Guid? MedicoId { get; set; }
    public Medico Medico { get; set; }
    public Guid? EspecialidadeId { get; set; }
    public Especialidade Especialidade { get; set; }
    public Guid? ConsultorioId { get; set; }
    public string NumeroConsultorio { get; set; }
    
    // Agendamento vinculado
    public Guid? AgendamentoId { get; set; }
    public Agendamento Agendamento { get; set; }
    
    // Métricas
    public int TempoEsperaMinutos { get; set; }
    public int TempoAtendimentoMinutos { get; set; }
}

public enum PrioridadeAtendimento
{
    Normal = 0,
    Idoso = 1,          // +60 anos
    Gestante = 2,
    Deficiente = 3,
    Crianca = 4,        // < 2 anos
    Urgencia = 5
}

public enum StatusSenha
{
    Aguardando = 1,
    Chamando = 2,
    EmAtendimento = 3,
    Atendido = 4,
    NaoCompareceu = 5,
    Cancelado = 6
}
```

#### 1.2 Serviço de Gestão de Fila
```csharp
// src/MedicSoft.Api/Services/Queue/FilaService.cs
public class FilaService
{
    private readonly IHubContext<FilaHub> _hubContext;
    private readonly ISmsService _smsService;
    
    // Gerar nova senha
    public async Task<SenhaFila> GerarSenhaAsync(GerarSenhaRequest request)
    {
        var fila = await _filaRepository.GetByIdAsync(request.FilaId);
        
        // Determina prioridade
        var prioridade = DeterminarPrioridade(
            request.DataNascimento,
            request.IsGestante,
            request.IsDeficiente);
        
        // Gera número da senha
        var numeroSenha = await GerarNumeroSenhaAsync(fila.Id, prioridade);
        
        var senha = new SenhaFila
        {
            FilaId = fila.Id,
            PacienteId = request.PacienteId,
            NomePaciente = request.NomePaciente,
            CpfPaciente = request.Cpf,
            TelefonePaciente = request.Telefone,
            NumeroSenha = numeroSenha,
            DataHoraEntrada = DateTime.Now,
            Prioridade = prioridade,
            MotivoPrioridade = ObterMotivoPrioridade(prioridade),
            Status = StatusSenha.Aguardando,
            EspecialidadeId = request.EspecialidadeId,
            AgendamentoId = request.AgendamentoId
        };
        
        await _senhaRepository.AddAsync(senha);
        
        // Notifica painel em tempo real
        await _hubContext.Clients.Group($"fila_{fila.Id}")
            .SendAsync("NovaSenha", senha);
        
        // Calcula estimativa de espera
        var estimativa = await CalcularTempoEsperaAsync(senha);
        
        // Envia SMS com estimativa
        await _smsService.SendSmsAsync(
            senha.TelefonePaciente,
            $"Sua senha: {senha.NumeroSenha}. " +
            $"Tempo estimado: {estimativa} min. " +
            $"Posição na fila: {await ObterPosicaoNaFilaAsync(senha.Id)}");
        
        return senha;
    }
    
    private PrioridadeAtendimento DeterminarPrioridade(
        DateTime dataNascimento,
        bool isGestante,
        bool isDeficiente)
    {
        var idade = DateTime.Now.Year - dataNascimento.Year;
        
        if (isDeficiente) return PrioridadeAtendimento.Deficiente;
        if (isGestante) return PrioridadeAtendimento.Gestante;
        if (idade >= 60) return PrioridadeAtendimento.Idoso;
        if (idade < 2) return PrioridadeAtendimento.Crianca;
        
        return PrioridadeAtendimento.Normal;
    }
    
    // Chamar próxima senha
    public async Task<SenhaFila> ChamarProximaSenhaAsync(Guid filaId, Guid medicoId)
    {
        // Busca próxima senha respeitando prioridade
        var proximaSenha = await _senhaRepository
            .GetProximaSenhaAsync(filaId, medicoId);
        
        if (proximaSenha == null)
            throw new BusinessException("Não há senhas na fila");
        
        proximaSenha.Status = StatusSenha.Chamando;
        proximaSenha.DataHoraChamada = DateTime.Now;
        proximaSenha.MedicoId = medicoId;
        proximaSenha.TentativasChamada++;
        
        await _senhaRepository.UpdateAsync(proximaSenha);
        
        // Atualiza painel em tempo real
        await _hubContext.Clients.Group($"fila_{filaId}")
            .SendAsync("ChamarSenha", new
            {
                Senha = proximaSenha.NumeroSenha,
                Paciente = proximaSenha.NomePaciente,
                Consultorio = proximaSenha.NumeroConsultorio
            });
        
        // Notifica paciente via SMS
        if (!string.IsNullOrEmpty(proximaSenha.TelefonePaciente))
        {
            await _smsService.SendSmsAsync(
                proximaSenha.TelefonePaciente,
                $"🔔 Sua senha {proximaSenha.NumeroSenha} foi chamada! " +
                $"Dirija-se ao consultório {proximaSenha.NumeroConsultorio}");
        }
        
        // Notifica próximos 3 da fila (alerta preventivo)
        await NotificarProximosDaFilaAsync(filaId, 3);
        
        return proximaSenha;
    }
    
    // Estimar tempo de espera
    public async Task<int> CalcularTempoEsperaAsync(SenhaFila senha)
    {
        // Conta senhas à frente com prioridade igual ou maior
        var senhasAFrente = await _senhaRepository
            .GetSenhasAFrenteAsync(senha.Id);
        
        // Tempo médio de atendimento da especialidade
        var tempoMedio = await _analyticsService
            .GetTempoMedioAtendimentoAsync(senha.EspecialidadeId);
        
        // Considera prioridade (senhas prioritárias "furar" fila)
        var fatorPrioridade = senha.Prioridade == PrioridadeAtendimento.Normal 
            ? 1.3 : 1.0;
        
        return (int)(senhasAFrente * tempoMedio * fatorPrioridade);
    }
}
```

#### 1.3 SignalR Hub - Tempo Real
```csharp
// src/MedicSoft.Api/Hubs/FilaHub.cs
public class FilaHub : Hub
{
    public async Task JoinFila(Guid filaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"fila_{filaId}");
    }
    
    public async Task LeaveFila(Guid filaId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"fila_{filaId}");
    }
    
    // Médico confirma atendimento iniciado
    public async Task IniciarAtendimento(Guid senhaId)
    {
        var senha = await _senhaRepository.GetByIdAsync(senhaId);
        senha.Status = StatusSenha.EmAtendimento;
        senha.DataHoraAtendimento = DateTime.Now;
        senha.TempoEsperaMinutos = 
            (int)(senha.DataHoraAtendimento.Value - senha.DataHoraEntrada).TotalMinutes;
        
        await _senhaRepository.UpdateAsync(senha);
        
        // Notifica painel
        await Clients.Group($"fila_{senha.FilaId}")
            .SendAsync("SenhaEmAtendimento", senhaId);
    }
    
    // Médico finaliza atendimento
    public async Task FinalizarAtendimento(Guid senhaId)
    {
        var senha = await _senhaRepository.GetByIdAsync(senhaId);
        senha.Status = StatusSenha.Atendido;
        senha.DataHoraSaida = DateTime.Now;
        senha.TempoAtendimentoMinutos = 
            (int)(senha.DataHoraSaida.Value - senha.DataHoraAtendimento.Value).TotalMinutes;
        
        await _senhaRepository.UpdateAsync(senha);
        
        // Registra métrica para analytics
        await _analyticsService.RegistrarAtendimentoAsync(senha);
    }
}
```

---

### 2. Totem de Autoatendimento (3 semanas)

#### 2.1 Interface Angular para Totem
```typescript
// frontend/src/app/features/totem/totem-home/totem-home.component.ts
@Component({
  selector: 'app-totem-home',
  template: `
    <div class="totem-container">
      <div class="header">
        <img src="assets/logo.png" alt="Logo">
        <h1>Bem-vindo(a) à Clínica</h1>
      </div>
      
      <div class="main-menu">
        <button class="menu-button" (click)="checkIn()">
          <mat-icon>person_add</mat-icon>
          <span>Fazer Check-in</span>
          <small>Tenho consulta agendada</small>
        </button>
        
        <button class="menu-button" (click)="gerarSenha()">
          <mat-icon>confirmation_number</mat-icon>
          <span>Retirar Senha</span>
          <small>Atendimento por ordem de chegada</small>
        </button>
        
        <button class="menu-button" (click)="consultarSenha()">
          <mat-icon>search</mat-icon>
          <span>Consultar Minha Senha</span>
          <small>Ver posição na fila</small>
        </button>
      </div>
      
      <div class="footer">
        <p>Toque na tela para começar</p>
      </div>
    </div>
  `,
  styles: [`
    .totem-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      font-size: 1.5rem;
    }
    
    .menu-button {
      width: 400px;
      height: 200px;
      margin: 20px;
      font-size: 2rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      border: none;
      border-radius: 20px;
      background: white;
      color: #667eea;
      cursor: pointer;
      transition: transform 0.2s;
    }
    
    .menu-button:hover {
      transform: scale(1.05);
    }
  `]
})
export class TotemHomeComponent {
  checkIn() {
    this.router.navigate(['/totem/check-in']);
  }
  
  gerarSenha() {
    this.router.navigate(['/totem/gerar-senha']);
  }
  
  consultarSenha() {
    this.router.navigate(['/totem/consultar']);
  }
}
```

#### 2.2 Fluxo de Geração de Senha
```typescript
// frontend/src/app/features/totem/gerar-senha/gerar-senha.component.ts
export class GerarSenhaComponent implements OnInit {
  form: FormGroup;
  
  ngOnInit() {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      cpf: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      telefone: ['', Validators.required],
      dataNascimento: ['', Validators.required],
      especialidadeId: [null],
      isGestante: [false],
      isDeficiente: [false]
    });
  }
  
  async gerarSenha() {
    if (!this.form.valid) return;
    
    this.loading = true;
    
    try {
      const senha = await this.filaService.gerarSenha({
        ...this.form.value,
        filaId: this.filaAtual.id
      });
      
      // Mostra senha gerada
      this.mostrarSenhaGerada(senha);
      
      // Imprime comprovante (se tiver impressora térmica)
      await this.imprimirComprovante(senha);
      
      // Volta ao início após 10 segundos
      setTimeout(() => {
        this.router.navigate(['/totem']);
      }, 10000);
      
    } catch (error) {
      this.snackBar.open('Erro ao gerar senha', 'OK', { duration: 3000 });
    } finally {
      this.loading = false;
    }
  }
  
  mostrarSenhaGerada(senha: SenhaFila) {
    const dialogRef = this.dialog.open(SenhaGeradaDialogComponent, {
      width: '800px',
      disableClose: true,
      data: senha
    });
  }
}
```

---

### 3. Painel de TV (3 semanas)

#### 3.1 Interface de Painel em Tempo Real
```typescript
// frontend/src/app/features/painel/painel-tv/painel-tv.component.ts
@Component({
  selector: 'app-painel-tv',
  template: `
    <div class="painel-container">
      <!-- Chamada atual destacada -->
      <div class="chamada-atual" *ngIf="chamadaAtual">
        <div class="senha-grande">{{ chamadaAtual.numeroSenha }}</div>
        <div class="paciente">{{ chamadaAtual.nomePaciente }}</div>
        <div class="consultorio">
          <mat-icon>room</mat-icon>
          Consultório {{ chamadaAtual.numeroConsultorio }}
        </div>
      </div>
      
      <!-- Últimas chamadas -->
      <div class="ultimas-chamadas">
        <h3>Últimas Chamadas</h3>
        <div class="chamada" *ngFor="let chamada of ultimasChamadas">
          <span class="senha">{{ chamada.numeroSenha }}</span>
          <span class="consultorio">Consultório {{ chamada.numeroConsultorio }}</span>
        </div>
      </div>
      
      <!-- Fila de espera -->
      <div class="fila-espera">
        <h3>Aguardando Atendimento ({{ senhasAguardando.length }})</h3>
        <div class="senhas-list">
          <span *ngFor="let senha of senhasAguardando" class="senha-chip">
            {{ senha.numeroSenha }}
            <mat-icon *ngIf="senha.prioridade > 0" class="priority">star</mat-icon>
          </span>
        </div>
      </div>
      
      <!-- Informações -->
      <div class="info-rodape">
        <div>{{ dataHoraAtual | date:'dd/MM/yyyy HH:mm' }}</div>
        <div>Tempo médio de espera: {{ tempoMedioEspera }} min</div>
      </div>
    </div>
  `,
  styles: [`
    .painel-container {
      height: 100vh;
      background: #1a1a2e;
      color: white;
      display: flex;
      flex-direction: column;
      padding: 40px;
    }
    
    .chamada-atual {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 30px;
      padding: 60px;
      text-align: center;
      margin-bottom: 40px;
      animation: pulse 2s infinite;
    }
    
    .senha-grande {
      font-size: 180px;
      font-weight: bold;
      margin-bottom: 20px;
      text-shadow: 0 0 30px rgba(255,255,255,0.5);
    }
    
    @keyframes pulse {
      0%, 100% { transform: scale(1); }
      50% { transform: scale(1.02); }
    }
  `]
})
export class PainelTvComponent implements OnInit, OnDestroy {
  private hubConnection: signalR.HubConnection;
  
  chamadaAtual: any;
  ultimasChamadas: any[] = [];
  senhasAguardando: any[] = [];
  
  ngOnInit() {
    this.iniciarConexaoSignalR();
    this.carregarFilaAtual();
    
    // Atualiza hora a cada segundo
    setInterval(() => {
      this.dataHoraAtual = new Date();
    }, 1000);
  }
  
  iniciarConexaoSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://api.clinica.com/hubs/fila')
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.on('ChamarSenha', (senha) => {
      this.chamadaAtual = senha;
      this.reproduzirSom();
      this.reproduzirVoz(senha);
      
      // Move para últimas chamadas após 30 segundos
      setTimeout(() => {
        this.ultimasChamadas.unshift(senha);
        if (this.ultimasChamadas.length > 5) {
          this.ultimasChamadas.pop();
        }
        this.chamadaAtual = null;
      }, 30000);
    });
    
    this.hubConnection.on('NovaSenha', (senha) => {
      this.senhasAguardando.push(senha);
    });
    
    this.hubConnection.on('SenhaEmAtendimento', (senhaId) => {
      this.senhasAguardando = this.senhasAguardando
        .filter(s => s.id !== senhaId);
    });
    
    this.hubConnection.start();
  }
  
  reproduzirVoz(senha: any) {
    // Text-to-Speech
    const utterance = new SpeechSynthesisUtterance(
      `Senha ${senha.numeroSenha}, ${senha.nomePaciente}, ` +
      `comparecer ao consultório ${senha.numeroConsultorio}`
    );
    utterance.lang = 'pt-BR';
    utterance.rate = 0.9;
    speechSynthesis.speak(utterance);
  }
  
  reproduzirSom() {
    const audio = new Audio('assets/sounds/chamada.mp3');
    audio.play();
  }
}
```

---

### 4. Notificações e Analytics (2 semanas)

#### 4.1 Serviço de Notificações
```csharp
// src/MedicSoft.Api/Services/Queue/FilaNotificationService.cs
public class FilaNotificationService
{
    // Notifica quando estiver próximo (3 senhas antes)
    public async Task NotificarProximosDaFilaAsync(Guid filaId, int quantidade)
    {
        var proximas = await _senhaRepository
            .GetProximasSenhasAsync(filaId, quantidade);
        
        foreach (var senha in proximas)
        {
            if (!string.IsNullOrEmpty(senha.TelefonePaciente))
            {
                var posicao = await _filaService.ObterPosicaoNaFilaAsync(senha.Id);
                var tempoEstimado = await _filaService.CalcularTempoEsperaAsync(senha);
                
                await _smsService.SendSmsAsync(
                    senha.TelefonePaciente,
                    $"⏰ Você está próximo! Posição: {posicao}. " +
                    $"Tempo estimado: ~{tempoEstimado} min. " +
                    $"Senha: {senha.NumeroSenha}");
            }
        }
    }
    
    // Alerta de senha não compareceu
    public async Task AlertarNaoComparecimentoAsync(Guid senhaId)
    {
        var senha = await _senhaRepository.GetByIdAsync(senhaId);
        
        if (senha.TentativasChamada >= 3)
        {
            senha.Status = StatusSenha.NaoCompareceu;
            await _senhaRepository.UpdateAsync(senha);
            
            // Notifica paciente
            await _smsService.SendSmsAsync(
                senha.TelefonePaciente,
                $"Sua senha {senha.NumeroSenha} foi chamada 3x e você não compareceu. " +
                $"Por favor, retire nova senha na recepção.");
        }
    }
}
```

#### 4.2 Analytics de Fila
```csharp
// src/MedicSoft.Api/Services/Queue/FilaAnalyticsService.cs
public class FilaAnalyticsService
{
    public async Task<FilaMetrics> GetMetricasDoDiaAsync(DateTime data)
    {
        var senhas = await _senhaRepository
            .GetByDataAsync(data)
            .Where(s => s.Status == StatusSenha.Atendido)
            .ToListAsync();
        
        return new FilaMetrics
        {
            Data = data,
            TotalAtendimentos = senhas.Count,
            TempoMedioEspera = senhas.Average(s => s.TempoEsperaMinutos),
            TempoMedioAtendimento = senhas.Average(s => s.TempoAtendimentoMinutos),
            TaxaNaoComparecimento = await CalcularTaxaNaoComparecimentoAsync(data),
            PicoAtendimento = await GetHorarioPicoAsync(data),
            AtendimentosPorPrioridade = senhas
                .GroupBy(s => s.Prioridade)
                .Select(g => new { Prioridade = g.Key, Total = g.Count() })
                .ToList()
        };
    }
}
```

---

## 📝 Tarefas de Implementação

### Sprint 1: Backend Core (Semanas 1-4)
- [ ] Criar entidades `FilaEspera` e `SenhaFila`
- [ ] Implementar `FilaService` completo
- [ ] Sistema de priorização
- [ ] Cálculo de tempo de espera
- [ ] SignalR Hub configurado
- [ ] Testes unitários

### Sprint 2: Totem (Semanas 5-7)
- [ ] Interface Angular para totem
- [ ] Tela de geração de senha
- [ ] Validação de CPF e dados
- [ ] Impressão térmica (opcional)
- [ ] Testes de usabilidade

### Sprint 3: Painel de TV (Semanas 8-10)
- [ ] Interface full-screen do painel
- [ ] Integração SignalR tempo real
- [ ] Animações e efeitos visuais
- [ ] Text-to-Speech para chamadas
- [ ] Sons de notificação

### Sprint 4: Notificações e Analytics (Semanas 11-12)
- [ ] Sistema de notificações SMS
- [ ] Alertas preventivos
- [ ] Dashboard de analytics
- [ ] Relatórios de performance
- [ ] Otimizações

---

## 🧪 Testes

### Testes de Carga
- Simular 100+ senhas simultâneas
- Testar latência do SignalR
- Validar priorização em alta carga

### Testes de Usabilidade
- Idosos conseguem usar o totem?
- Texto legível à distância no painel?
- Tempo de resposta aceitável?

---

## 📊 Métricas de Sucesso

- ✅ 90%+ dos pacientes usam totem (sem ir à recepção)
- ✅ Redução de 60% em tempo de espera na recepção
- ✅ Tempo médio de geração de senha < 45 segundos
- ✅ Latência painel TV < 2 segundos
- ✅ Taxa de não comparecimento < 5%

---

## 💰 ROI Esperado

**Investimento:** R$ 90.000  
**Hardware Adicional:** R$ 15.000 (totem + TV + mini-PC)  
**Economia Anual:**
- Redução de 1 recepcionista: R$ 36.000/ano
- Melhor aproveitamento de agenda: R$ 40.000/ano
- Redução de no-show: R$ 30.000/ano

**Total Economia:** R$ 106.000/ano  
**Payback:** ~12 meses
