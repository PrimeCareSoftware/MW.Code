/**
 * Representa um certificado digital ICP-Brasil
 */
export interface CertificadoDigital {
  id: string;
  medicoId: string;
  medicoNome?: string;
  tipo: TipoCertificado;
  numeroCertificado: string;
  subjectName: string;
  issuerName: string;
  thumbprint?: string;
  dataEmissao: Date | string;
  dataExpiracao: Date | string;
  valido: boolean;
  totalAssinaturas: number;
  diasParaExpiracao?: number;
}

/**
 * Tipo de certificado digital
 */
export enum TipoCertificado {
  A1 = 'A1',  // Armazenado em software (1 ano validade)
  A3 = 'A3'   // Armazenado em token/smartcard (3-5 anos validade)
}

/**
 * Informação de certificado A3 disponível
 */
export interface CertificateInfo {
  subject: string;
  issuer: string;
  thumbprint: string;
  validFrom: Date | string;
  validTo: Date | string;
  isValid: boolean;
}

/**
 * Request para importar certificado A1
 */
export interface ImportarCertificadoA1Request {
  arquivo: File;
  senha: string;
}

/**
 * Request para registrar certificado A3
 */
export interface RegistrarCertificadoA3Request {
  thumbprint: string;
}
