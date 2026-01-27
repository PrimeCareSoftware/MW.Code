// BI Analytics DTOs - matching backend models exactly

export interface DashboardClinico {
  periodo: PeriodoDto;
  totalConsultas: number;
  taxaOcupacao: number;
  tempoMedioConsulta: number;
  taxaNoShow: number;
  consultasPorEspecialidade: ConsultasPorEspecialidadeDto[];
  consultasPorDiaSemana: ConsultasPorDiaDto[];
  consultasPorMedico: ConsultasPorMedicoDto[];
  consultasPorHorario: ConsultasPorHorarioDto[];
  diagnosticosMaisFrequentes: DiagnosticoFrequenciaDto[];
  pacientesNovos: number;
  pacientesRetorno: number;
  tendenciaConsultas: TendenciaMensalDto[];
}

export interface DashboardFinanceiro {
  periodo: PeriodoDto;
  receitaTotal: number;
  receitaRecebida: number;
  receitaPendente: number;
  receitaAtrasada: number;
  despesaTotal: number;
  despesaPaga: number;
  despesaPendente: number;
  lucroBruto: number;
  margemLucro: number;
  receitaPorConvenio: ReceitaPorConvenioDto[];
  receitaPorMedico: ReceitaPorMedicoDto[];
  receitaPorFormaPagamento: ReceitaPorFormaPagamentoDto[];
  despesaPorCategoria: DespesaPorCategoriaDto[];
  ticketMedio: number;
  projecaoMesAtual: number;
  fluxoCaixaDiario: FluxoCaixaDiarioDto[];
}

export interface PeriodoDto {
  inicio: string;
  fim: string;
}

export interface ConsultasPorEspecialidadeDto {
  especialidade: string;
  total: number;
  percentual: number;
}

export interface ConsultasPorDiaDto {
  diaSemana: string;
  total: number;
  mediaPorDia: number;
}

export interface ConsultasPorMedicoDto {
  nomeMedico: string;
  crm: string;
  total: number;
  taxaOcupacao: number;
}

export interface ConsultasPorHorarioDto {
  hora: number;
  total: number;
}

export interface DiagnosticoFrequenciaDto {
  codigoCid: string;
  descricao: string;
  frequencia: number;
  percentual: number;
}

export interface TendenciaMensalDto {
  mes: string;
  agendadas: number;
  realizadas: number;
  canceladas: number;
  noShow: number;
}

export interface ReceitaPorConvenioDto {
  nomeConvenio: string;
  total: number;
  quantidadeConsultas: number;
  ticketMedio: number;
}

export interface ReceitaPorMedicoDto {
  nomeMedico: string;
  total: number;
  quantidadeConsultas: number;
}

export interface ReceitaPorFormaPagamentoDto {
  formaPagamento: string;
  total: number;
  percentual: number;
}

export interface DespesaPorCategoriaDto {
  categoria: string;
  total: number;
  percentual: number;
}

export interface FluxoCaixaDiarioDto {
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
