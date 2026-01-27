/**
 * Representa uma assinatura digital em um documento
 */
export interface AssinaturaDigital {
  id: string;
  documentoId: string;
  tipoDocumento: TipoDocumento;
  tipoDocumentoNome?: string;
  medicoId: string;
  medicoNome?: string;
  medicoCRM?: string;
  certificadoId: string;
  certificadoSubject?: string;
  certificadoExpiracao?: Date | string;
  dataHoraAssinatura: Date | string;
  hashDocumento: string;
  temTimestamp: boolean;
  dataTimestamp?: Date | string;
  valida: boolean;
  dataUltimaValidacao?: Date | string;
  localAssinatura?: string;
  ipAssinatura?: string;
}

/**
 * Tipo de documento que pode ser assinado
 */
export enum TipoDocumento {
  Prontuario = 1,
  Receita = 2,
  Atestado = 3,
  Laudo = 4,
  Prescricao = 5,
  Encaminhamento = 6
}

/**
 * Mapa de nomes dos tipos de documento
 */
export const TipoDocumentoNomes: { [key: number]: string } = {
  [TipoDocumento.Prontuario]: 'Prontuário',
  [TipoDocumento.Receita]: 'Receita',
  [TipoDocumento.Atestado]: 'Atestado',
  [TipoDocumento.Laudo]: 'Laudo',
  [TipoDocumento.Prescricao]: 'Prescrição',
  [TipoDocumento.Encaminhamento]: 'Encaminhamento'
};

/**
 * Request para assinar documento
 */
export interface AssinarDocumentoRequest {
  documentoId: string;
  tipoDocumento: TipoDocumento;
  documentoBytes: string; // Base64
  senhaCertificado?: string; // Opcional para A1
}

/**
 * Resultado da operação de assinatura
 */
export interface ResultadoAssinatura {
  sucesso: boolean;
  mensagem: string;
  assinaturaId?: string;
  assinatura?: AssinaturaDigital;
}

/**
 * Resultado da validação de assinatura
 */
export interface ResultadoValidacao {
  valida: boolean;
  dataAssinatura?: Date | string;
  assinante?: string;
  crm?: string;
  certificado?: string;
  motivo?: string;
}
