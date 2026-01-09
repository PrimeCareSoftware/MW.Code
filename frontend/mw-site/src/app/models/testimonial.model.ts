export interface Testimonial {
  id: string;
  name: string;
  clinicName: string;
  role: string;
  photo?: string;
  rating: number;
  comment: string;
  date: Date;
}

export const SAMPLE_TESTIMONIALS: Testimonial[] = [
  {
    id: '1',
    name: 'Dr. João Silva',
    clinicName: 'Clínica Saúde Total',
    role: 'Médico Cardiologista',
    rating: 5,
    comment: 'O PrimeCare Software transformou a gestão do meu consultório. A interface é intuitiva e o sistema de agendamentos é excelente!',
    date: new Date('2025-09-15')
  },
  {
    id: '2',
    name: 'Dra. Maria Santos',
    clinicName: 'Clínica São Lucas',
    role: 'Médica Pediatra',
    rating: 5,
    comment: 'Desde que comecei a usar, consegui organizar melhor minha agenda e os prontuários ficaram muito mais acessíveis. Recomendo!',
    date: new Date('2025-09-20')
  },
  {
    id: '3',
    name: 'Dr. Carlos Oliveira',
    clinicName: 'Centro Médico Vida',
    role: 'Médico Ortopedista',
    rating: 5,
    comment: 'A integração com WhatsApp e os lembretes automáticos reduziram muito as faltas. Excelente investimento!',
    date: new Date('2025-10-01')
  }
];
