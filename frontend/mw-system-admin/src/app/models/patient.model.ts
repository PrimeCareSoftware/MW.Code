export interface Address {
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface Patient {
  id: string;
  name: string;
  document?: string;
  cpf?: string;
  dateOfBirth: string;
  gender?: string;
  email: string;
  phone: string;
  address: Address | string;
  medicalHistory?: string;
  allergies?: string;
  isActive?: boolean;
  age?: number;
  isChild?: boolean;
  guardianId?: string;
  guardianName?: string;
  createdAt?: string;
}
