// Enums
export enum TipoFila {
  Geral = 1,
  PorEspecialidade = 2,
  PorMedico = 3,
  Triagem = 4
}

export enum PrioridadeAtendimento {
  Normal = 0,
  Idoso = 1,
  Gestante = 2,
  Deficiente = 3,
  Crianca = 4,
  Urgencia = 5
}

export enum StatusSenha {
  Aguardando = 1,
  Chamando = 2,
  EmAtendimento = 3,
  Atendido = 4,
  NaoCompareceu = 5,
  Cancelado = 6
}

// Interfaces
export interface FilaEspera {
  id: string;
  clinicaId: string;
  nome: string;
  tipo: TipoFila;
  ativa: boolean;
  tempoMedioAtendimento: number;
  usaPrioridade: boolean;
  usaAgendamento: boolean;
}

export interface SenhaFila {
  id: string;
  filaId: string;
  pacienteId?: string;
  nomePaciente: string;
  cpfPaciente: string;
  telefonePaciente: string;
  numeroSenha: string;
  dataHoraEntrada: Date;
  dataHoraChamada?: Date;
  dataHoraAtendimento?: Date;
  dataHoraSaida?: Date;
  prioridade: PrioridadeAtendimento;
  motivoPrioridade?: string;
  status: StatusSenha;
  tentativasChamada: number;
  medicoId?: string;
  especialidadeId?: string;
  consultorioId?: string;
  numeroConsultorio?: string;
  agendamentoId?: string;
  tempoEsperaMinutos: number;
  tempoAtendimentoMinutos: number;
}

export interface GerarSenhaRequest {
  filaId: string;
  pacienteId?: string;
  nomePaciente: string;
  cpf: string;
  telefone: string;
  dataNascimento: Date;
  especialidadeId?: string;
  agendamentoId?: string;
  isGestante: boolean;
  isDeficiente: boolean;
}

export interface ChamarSenhaRequest {
  filaId: string;
  medicoId: string;
  numeroConsultorio: string;
}

export interface FilaSummary {
  filaId: string;
  totalAguardando: number;
  totalEmAtendimento: number;
  tempoMedioEspera: number;
  proximaSenha?: string;
}

export interface ConsultarSenhaResponse {
  senha: SenhaFila;
  posicaoNaFila: number;
  tempoEstimadoMinutos: number;
  senhasAFrente: number;
}

export interface FilaMetrics {
  data: Date;
  totalAtendimentos: number;
  tempoMedioEspera: number;
  tempoMedioAtendimento: number;
  taxaNaoComparecimento: number;
  picoAtendimento?: string;
  atendimentosPorPrioridade: { prioridade: PrioridadeAtendimento; total: number }[];
}

// SignalR Event Models
export interface ChamarSenhaEvent {
  senha: string;
  paciente: string;
  consultorio: string;
  senhaId: string;
}

export interface NovaSenhaEvent {
  senhaId: string;
  numeroSenha: string;
  prioridade: PrioridadeAtendimento;
}
