# 🏪 Prompt: Marketplace Público

## 📊 Status
- **Prioridade**: BAIXA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 3-4 meses | 2 devs
- **Prazo**: 2027+

## 🎯 Contexto

Plataforma pública onde pacientes podem descobrir clínicas, ver médicos e especialidades, verificar disponibilidade em tempo real e agendar consultas online sem cadastro prévio, com páginas otimizadas para SEO.

## 📋 Justificativa

### Benefícios
- ✅ Aquisição de novos pacientes
- ✅ Visibilidade no Google
- ✅ Redução de fricção
- ✅ Competição com Doctoralia
- ✅ Canal de marketing gratuito

## 🏗️ Arquitetura

```csharp
// Public Clinic Profile
public class PublicClinicProfile : Entity
{
    public Guid ClinicId { get; set; }
    public string PublicUrl { get; set; }  // /clinica/nome-da-clinica
    public bool IsPublic { get; set; }
    public string Description { get; set; }
    public List<string> Photos { get; set; }
    public List<PublicDoctorInfo> Doctors { get; set; }
    public List<string> Specialties { get; set; }
    public Address Address { get; set; }
    public ContactInfo Contact { get; set; }
    public WorkingHours Hours { get; set; }
    public List<Review> Reviews { get; set; }
    public double AverageRating { get; set; }
}

// Public Booking
public class PublicBooking : Entity
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string PatientName { get; set; }
    public string PatientEmail { get; set; }
    public string PatientPhone { get; set; }
    public string PatientCpf { get; set; }
    public BookingStatus Status { get; set; }
    public bool IsNewPatient { get; set; }
}
```

## 🎨 Frontend

```typescript
// Public clinic page
@Component({
  selector: 'app-public-clinic-page',
  template: `
    <div class="clinic-header">
      <h1>{{ clinic.name }}</h1>
      <div class="rating">
        <mat-icon>star</mat-icon>
        {{ clinic.averageRating }} ({{ clinic.reviews.length }} avaliações)
      </div>
    </div>
    
    <div class="doctors-list">
      <mat-card *ngFor="let doctor of clinic.doctors">
        <img [src]="doctor.photoUrl" alt="{{ doctor.name }}">
        <h3>{{ doctor.name }}</h3>
        <p>{{ doctor.specialty }}</p>
        <button mat-raised-button color="primary" (click)="bookAppointment(doctor)">
          Agendar Consulta
        </button>
      </mat-card>
    </div>
    
    <div class="calendar">
      <app-public-availability-calendar [doctorId]="selectedDoctor?.id">
      </app-public-availability-calendar>
    </div>
  `
})
export class PublicClinicPageComponent {}
```

## ✅ Checklist

### Backend
- [ ] Public API endpoints
- [ ] SEO-friendly URLs
- [ ] Booking without login
- [ ] Email confirmations

### Frontend
- [ ] Public landing pages
- [ ] Search functionality
- [ ] Calendar with availability
- [ ] Quick booking form
- [ ] Reviews system

### SEO
- [ ] Meta tags
- [ ] Open Graph
- [ ] Structured data (Schema.org)
- [ ] Sitemap
- [ ] Google Business integration

## 💰 Investimento

- **Esforço**: 3-4 meses | 2 devs
- **Custo**: R$ 135-180k

## 🎯 Critérios de Aceitação

- [ ] Páginas públicas funcionando
- [ ] SEO otimizado
- [ ] Agendamento sem login funciona
- [ ] Google indexa páginas
- [ ] Sistema de avaliações operacional
