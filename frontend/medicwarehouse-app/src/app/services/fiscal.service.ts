import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

/**
 * Status da apuração de impostos
 */
export enum StatusApuracao {
  EmAberto = 1,
  Apurado = 2,
  Pago = 3,
  Parcelado = 4,
  Atrasado = 5
}

/**
 * Regime tributário
 */
export enum RegimeTributarioEnum {
  SimplesNacional = 1,
  LucroPresumido = 2,
  LucroReal = 3,
  MEI = 4
}

/**
 * Anexo do Simples Nacional
 */
export enum AnexoSimplesNacional {
  AnexoIII = 3,
  AnexoV = 5
}

/**
 * Apuração mensal de impostos
 */
export interface ApuracaoImpostos {
  id: string;
  clinicaId: string;
  mes: number;
  ano: number;
  dataApuracao: Date;
  faturamentoBruto: number;
  deducoes: number;
  faturamentoLiquido: number;
  totalPIS: number;
  totalCOFINS: number;
  totalIR: number;
  totalCSLL: number;
  totalISS: number;
  totalINSS: number;
  receitaBruta12Meses?: number;
  aliquotaEfetiva?: number;
  valorDAS?: number;
  status: StatusApuracao;
  dataPagamento?: Date;
  comprovantesPagamento?: string;
}

/**
 * Configuração fiscal da clínica
 */
export interface ConfiguracaoFiscal {
  id: string;
  clinicaId: string;
  regime: RegimeTributarioEnum;
  vigenciaInicio: Date;
  vigenciaFim?: Date;
  optanteSimplesNacional: boolean;
  anexoSimples?: AnexoSimplesNacional;
  fatorR?: number;
  aliquotaISS: number;
  aliquotaPIS: number;
  aliquotaCOFINS: number;
  aliquotaIR: number;
  aliquotaCSLL: number;
  retemINSS: boolean;
  aliquotaINSS: number;
  codigoServico: string;
  cnae: string;
  inscricaoMunicipal: string;
  issRetido: boolean;
}

/**
 * Request para registrar pagamento
 */
export interface PagamentoRequest {
  dataPagamento: Date;
  comprovante: string;
}

/**
 * Serviço para gestão fiscal e impostos
 */
@Injectable({
  providedIn: 'root'
})
export class FiscalService {
  private readonly apiUrl = `${environment.apiUrl}/Fiscal`;

  constructor(private http: HttpClient) {}

  /**
   * Obtém apuração mensal de impostos
   */
  getApuracaoMensal(mes: number, ano: number): Observable<ApuracaoImpostos> {
    return this.http.get<ApuracaoImpostos>(`${this.apiUrl}/apuracao/${mes}/${ano}`);
  }

  /**
   * Gera nova apuração mensal de impostos
   */
  gerarApuracao(mes: number, ano: number): Observable<ApuracaoImpostos> {
    return this.http.post<ApuracaoImpostos>(`${this.apiUrl}/apuracao/${mes}/${ano}`, {});
  }

  /**
   * Obtém configuração fiscal da clínica
   */
  getConfiguracao(): Observable<ConfiguracaoFiscal> {
    return this.http.get<ConfiguracaoFiscal>(`${this.apiUrl}/configuracao`);
  }

  /**
   * Obtém evolução mensal de impostos
   */
  getEvolucaoMensal(meses: number = 12): Observable<ApuracaoImpostos[]> {
    const params = new HttpParams().set('meses', meses.toString());
    return this.http.get<ApuracaoImpostos[]>(`${this.apiUrl}/evolucao-mensal`, { params });
  }

  /**
   * Obtém DRE do período
   */
  getDRE(mes: number, ano: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/dre/${mes}/${ano}`);
  }

  /**
   * Atualiza status da apuração
   */
  atualizarStatus(apuracaoId: string, status: StatusApuracao): Observable<ApuracaoImpostos> {
    return this.http.put<ApuracaoImpostos>(
      `${this.apiUrl}/apuracao/${apuracaoId}/status`,
      status
    );
  }

  /**
   * Registra pagamento de apuração
   */
  registrarPagamento(apuracaoId: string, request: PagamentoRequest): Observable<ApuracaoImpostos> {
    return this.http.post<ApuracaoImpostos>(
      `${this.apiUrl}/apuracao/${apuracaoId}/pagamento`,
      request
    );
  }

  /**
   * Calcula carga tributária efetiva
   */
  calcularCargaTributaria(apuracao: ApuracaoImpostos): number {
    if (apuracao.faturamentoBruto === 0) return 0;
    
    const totalImpostos = 
      apuracao.totalPIS + 
      apuracao.totalCOFINS + 
      apuracao.totalIR + 
      apuracao.totalCSLL + 
      apuracao.totalISS + 
      apuracao.totalINSS;
    
    return (totalImpostos / apuracao.faturamentoBruto) * 100;
  }

  /**
   * Obtém nome do status
   */
  getStatusNome(status: StatusApuracao): string {
    switch (status) {
      case StatusApuracao.EmAberto:
        return 'Em Aberto';
      case StatusApuracao.Apurado:
        return 'Apurado';
      case StatusApuracao.Pago:
        return 'Pago';
      case StatusApuracao.Parcelado:
        return 'Parcelado';
      case StatusApuracao.Atrasado:
        return 'Atrasado';
      default:
        return 'Desconhecido';
    }
  }

  /**
   * Obtém nome do regime tributário
   */
  getRegimeNome(regime: RegimeTributarioEnum): string {
    switch (regime) {
      case RegimeTributarioEnum.SimplesNacional:
        return 'Simples Nacional';
      case RegimeTributarioEnum.LucroPresumido:
        return 'Lucro Presumido';
      case RegimeTributarioEnum.LucroReal:
        return 'Lucro Real';
      case RegimeTributarioEnum.MEI:
        return 'MEI';
      default:
        return 'Não definido';
    }
  }

  /**
   * Obtém nome do anexo
   */
  getAnexoNome(anexo?: AnexoSimplesNacional): string {
    if (!anexo) return '-';
    return anexo === AnexoSimplesNacional.AnexoIII ? 'Anexo III' : 'Anexo V';
  }
}
