import { Patient } from '../models/patient.model';

export const MOCK_PATIENTS: Patient[] = [
  {
    id: '1',
    name: 'João Silva',
    document: '123.456.789-00',
    dateOfBirth: '1985-05-15',
    gender: 'M',
    email: 'joao.silva@email.com',
    phone: '+55 11 98765-4321',
    address: {
      street: 'Rua das Flores',
      number: '123',
      complement: 'Apto 45',
      neighborhood: 'Centro',
      city: 'São Paulo',
      state: 'SP',
      zipCode: '01234-567',
      country: 'Brasil'
    },
    medicalHistory: 'Hipertensão controlada',
    allergies: 'Penicilina',
    isActive: true,
    age: 38,
    isChild: false,
    createdAt: '2024-01-15T10:00:00Z'
  },
  {
    id: '2',
    name: 'Maria Santos',
    document: '987.654.321-00',
    dateOfBirth: '1990-08-22',
    gender: 'F',
    email: 'maria.santos@email.com',
    phone: '+55 11 91234-5678',
    address: {
      street: 'Avenida Paulista',
      number: '456',
      neighborhood: 'Bela Vista',
      city: 'São Paulo',
      state: 'SP',
      zipCode: '01310-100',
      country: 'Brasil'
    },
    medicalHistory: 'Diabetes tipo 2',
    allergies: 'Nenhuma',
    isActive: true,
    age: 33,
    isChild: false,
    createdAt: '2024-02-10T14:30:00Z'
  },
  {
    id: '3',
    name: 'Pedro Oliveira',
    document: '456.789.123-00',
    dateOfBirth: '2015-03-10',
    gender: 'M',
    email: 'contato.oliveira@email.com',
    phone: '+55 11 99876-5432',
    address: {
      street: 'Rua Augusta',
      number: '789',
      neighborhood: 'Consolação',
      city: 'São Paulo',
      state: 'SP',
      zipCode: '01305-100',
      country: 'Brasil'
    },
    medicalHistory: 'Saudável',
    allergies: 'Lactose',
    isActive: true,
    age: 8,
    isChild: true,
    guardianId: '2',
    guardianName: 'Maria Santos',
    createdAt: '2024-03-05T09:15:00Z'
  }
];
