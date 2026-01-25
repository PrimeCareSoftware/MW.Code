export interface Company {
  id: string;
  name: string;
  tradeName: string;
  document: string;
  documentType: DocumentType;
  phone: string;
  email: string;
  isActive: boolean;
  subdomain?: string;
  createdAt: string;
  updatedAt?: string;
}

export enum DocumentType {
  CPF = 0,
  CNPJ = 1
}

export interface CreateCompany {
  name: string;
  tradeName: string;
  document: string;
  documentType: DocumentType;
  phone: string;
  email: string;
}

export interface UpdateCompany {
  name: string;
  tradeName: string;
  phone: string;
  email: string;
}

export const DocumentTypeLabels: Record<DocumentType, string> = {
  [DocumentType.CPF]: 'CPF',
  [DocumentType.CNPJ]: 'CNPJ'
};
