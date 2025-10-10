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
  document: string;
  dateOfBirth: string;
  gender: string;
  email: string;
  phone: string;
  address: Address;
  medicalHistory?: string;
  allergies?: string;
  isActive: boolean;
  age: number;
  isChild: boolean;
  guardianId?: string;
  guardianName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePatient {
  name: string;
  document: string;
  dateOfBirth: string;
  gender: string;
  email: string;
  phoneCountryCode: string;
  phoneNumber: string;
  address: Address;
  medicalHistory?: string;
  allergies?: string;
  guardianId?: string;
}

export interface UpdatePatient {
  name: string;
  email: string;
  phoneCountryCode: string;
  phoneNumber: string;
  address: Address;
  medicalHistory?: string;
  allergies?: string;
}
