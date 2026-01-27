// BI Analytics DTOs - matching backend models

export interface DashboardClinico {
  periodo: PeriodoInfo;
  totalConsultas: number;
  taxaOcupacao: number;
  tempoMedioConsulta: number;
  taxaNoShow: number;
  consultasPorEspecialidade: ConsultaPorEspecialidade[];
  consultasPorDiaSemana: ConsultaPorDiaSemana[];
  consultasPorMedico: ConsultaPorMedico[];
  diagnosticosMaisFrequentes: DiagnosticoFrequencia[];
  pacientesNovos: number;
  pacientesRetorno: number;
  tendenciaConsultas: TendenciaMensal[];
}

export interface DashboardFinanceiro {
  periodo: PeriodoInfo;
  receitaTotal: number;
  receitaRecebida: number;
  receitaPendente: number;
  receitaAtrasada: number;
  despesaTotal: number;
  despesaPaga: number;
  despesaPendente: number;
  lucroBruto: number;
  margemLucro: number;
  receitaPorConvenio: ReceitaPorConvenio[];
  receitaPorFormaPagamento: ReceitaPorFormaPagamento[];
  despesaPorCategoria: DespesaPorCategoria[];
  ticketMedio: number;
  projecaoMesAtual: number;
  fluxoCaixaDiario: FluxoCaixaDiario[];
}

export interface PeriodoInfo {
  inicio: string;
  fim: string;
}

export interface ConsultaPorEspecialidade {
  especialidadeId: string;
  especialidade: string;
  total: number;
  percentual: number;
}

export interface ConsultaPorDiaSemana {
  diaSemana: number;
  diaSemanaTexto: string;
  total: number;
  percentual: number;
}

export interface ConsultaPorMedico {
  medicoId: string;
  medicoNome: string;
  total: number;
  percentual: number;
}

export interface DiagnosticoFrequencia {
  codigoCid: string;
  descricao: string;
  frequencia: number;
  percentual: number;
}

export interface TendenciaMensal {
  mes: string;
  ano: number;
  mesNumero: number;
  agendadas: number;
  realizadas: number;
  canceladas: number;
  noShow: number;
}

export interface ReceitaPorConvenio {
  convenioId: string;
  convenioNome: string;
  valor: number;
  quantidade: number;
  percentual: number;
}

export interface ReceitaPorFormaPagamento {
  formaPagamento: string;
  formaPagamentoTexto: string;
  valor: number;
  quantidade: number;
  percentual: number;
}

export interface DespesaPorCategoria {
  categoria: string;
  valor: number;
  quantidade: number;
  percentual: number;
}

export interface FluxoCaixaDiario {
  data: string;
  entradas: number;
  saidas: number;
  saldo: number;
}

export interface ProjecaoReceita {
  mes: string;
  projecao: number;
  dataCalculo: string;
}

export interface MedicoOption {
  id: string;
  nome: string;
  crm: string;
  especialidade?: string;
}
