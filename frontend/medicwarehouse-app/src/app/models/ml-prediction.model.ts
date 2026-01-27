// ML Prediction DTOs - matching backend models

export interface PrevisaoConsultas {
  periodo: string;
  totalPrevisto: number;
  previsoes: PrevisaoDia[];
}

export interface PrevisaoDia {
  data: string;
  consultasPrevistas: number;
  confiancaPrevisao: number;
}

export interface PrevisaoDataEspecifica {
  data: string;
  consultasPrevistas: number;
}

export interface DadosNoShow {
  idadePaciente: number;
  diasAteConsulta: number;
  horaDia: number;
  historicoNoShow: number;
  tempoDesdeUltimaConsulta: number;
  isConvenio: number;
  temLembrete: number;
}

export interface RiscoNoShow {
  risco: number;
  acoesRecomendadas: string[];
}

export interface AgendamentoRisco {
  agendamentoId: string;
  pacienteNome: string;
  dataHora: string;
  riscoNoShow: number;
  acoesRecomendadas: string[];
}
