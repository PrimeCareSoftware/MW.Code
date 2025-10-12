using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Data seeder for creating test data for a demo clinic
    /// </summary>
    public class DataSeederService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientClinicLinkRepository _patientClinicLinkRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMedicationRepository _medicationRepository;
        private readonly IPrescriptionItemRepository _prescriptionItemRepository;
        private readonly IPrescriptionTemplateRepository _prescriptionTemplateRepository;
        private readonly IMedicalRecordTemplateRepository _medicalRecordTemplateRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly string _demoTenantId = "demo-clinic-001";

        public DataSeederService(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IProcedureRepository procedureRepository,
            IPatientRepository patientRepository,
            IPatientClinicLinkRepository patientClinicLinkRepository,
            IAppointmentRepository appointmentRepository,
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IPaymentRepository paymentRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IMedicationRepository medicationRepository,
            IPrescriptionItemRepository prescriptionItemRepository,
            IPrescriptionTemplateRepository prescriptionTemplateRepository,
            IMedicalRecordTemplateRepository medicalRecordTemplateRepository,
            INotificationRepository notificationRepository,
            IPasswordHasher passwordHasher)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _procedureRepository = procedureRepository;
            _patientRepository = patientRepository;
            _patientClinicLinkRepository = patientClinicLinkRepository;
            _appointmentRepository = appointmentRepository;
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _paymentRepository = paymentRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _medicationRepository = medicationRepository;
            _prescriptionItemRepository = prescriptionItemRepository;
            _prescriptionTemplateRepository = prescriptionTemplateRepository;
            _medicalRecordTemplateRepository = medicalRecordTemplateRepository;
            _notificationRepository = notificationRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedDemoDataAsync()
        {
            // Check if demo data already exists
            var existingClinics = await _clinicRepository.GetAllAsync(_demoTenantId);
            if (existingClinics.Any())
            {
                throw new InvalidOperationException("Demo data already exists for this tenant");
            }

            // 1. Create Demo Clinic
            var clinic = CreateDemoClinic();
            await _clinicRepository.AddAsync(clinic);

            // 2. Create Users (Admin, Doctor, Receptionist)
            var users = CreateDemoUsers();
            foreach (var user in users)
            {
                await _userRepository.AddAsync(user);
            }

            // 3. Create Procedures/Services
            var procedures = CreateDemoProcedures();
            foreach (var procedure in procedures)
            {
                await _procedureRepository.AddAsync(procedure);
            }

            // 4. Create Patients
            var patients = CreateDemoPatients(clinic.Id);
            foreach (var patient in patients)
            {
                await _patientRepository.AddAsync(patient);
            }

            // 5. Link patients to clinic
            var patientLinks = CreatePatientClinicLinks(patients, clinic.Id);
            foreach (var link in patientLinks)
            {
                await _patientClinicLinkRepository.AddAsync(link);
            }

            // 6. Create Appointments
            var appointments = CreateDemoAppointments(patients, clinic.Id);
            foreach (var appointment in appointments)
            {
                await _appointmentRepository.AddAsync(appointment);
            }

            // 7. Create Appointment Procedures
            var appointmentProcedures = CreateAppointmentProcedures(appointments, procedures, patients);
            foreach (var ap in appointmentProcedures)
            {
                await _appointmentProcedureRepository.AddAsync(ap);
            }

            // 8. Create Payments for some appointments
            var payments = CreateDemoPayments(appointments);
            foreach (var payment in payments)
            {
                await _paymentRepository.AddAsync(payment);
            }

            // 9. Create Medications
            var medications = CreateDemoMedications();
            foreach (var medication in medications)
            {
                await _medicationRepository.AddAsync(medication);
            }

            // 10. Create Medical Records for completed appointments
            var medicalRecords = CreateDemoMedicalRecords(appointments, patients);
            foreach (var record in medicalRecords)
            {
                await _medicalRecordRepository.AddAsync(record);
            }

            // 11. Create Prescription Items
            var prescriptionItems = CreateDemoPrescriptionItems(medicalRecords, medications, patients);
            foreach (var item in prescriptionItems)
            {
                await _prescriptionItemRepository.AddAsync(item);
            }

            // 12. Create Prescription Templates
            var prescriptionTemplates = CreateDemoPrescriptionTemplates();
            foreach (var template in prescriptionTemplates)
            {
                await _prescriptionTemplateRepository.AddAsync(template);
            }

            // 13. Create Medical Record Templates
            var medicalRecordTemplates = CreateDemoMedicalRecordTemplates();
            foreach (var template in medicalRecordTemplates)
            {
                await _medicalRecordTemplateRepository.AddAsync(template);
            }

            // 14. Create Notifications for appointments
            var notifications = CreateDemoNotifications(appointments, patients);
            foreach (var notification in notifications)
            {
                await _notificationRepository.AddAsync(notification);
            }
        }

        private Clinic CreateDemoClinic()
        {
            return new Clinic(
                "Clínica Demo MedicWarehouse",
                "Clínica Demo",
                "12.345.678/0001-90",
                "+55 11 98765-4321",
                "contato@clinicademo.com.br",
                "Avenida Paulista, 1000 - Bela Vista, São Paulo - SP, 01310-100",
                new TimeSpan(8, 0, 0),   // 08:00
                new TimeSpan(18, 0, 0),  // 18:00
                _demoTenantId,
                30
            );
        }

        private List<User> CreateDemoUsers()
        {
            var users = new List<User>();

            // Admin User
            var adminPassword = _passwordHasher.HashPassword("Admin@123");
            users.Add(new User(
                "admin",
                "admin@clinicademo.com.br",
                adminPassword,
                "Administrador Sistema",
                "+55 11 98765-4321",
                UserRole.SystemAdmin,
                _demoTenantId
            ));

            // Doctor
            var doctorPassword = _passwordHasher.HashPassword("Doctor@123");
            users.Add(new User(
                "dr.silva",
                "joao.silva@clinicademo.com.br",
                doctorPassword,
                "Dr. João Silva",
                "+55 11 98765-4322",
                UserRole.Doctor,
                _demoTenantId,
                null,
                "CRM-123456",
                "Clínico Geral"
            ));

            // Receptionist
            var receptionistPassword = _passwordHasher.HashPassword("Recep@123");
            users.Add(new User(
                "recep.maria",
                "maria.santos@clinicademo.com.br",
                receptionistPassword,
                "Maria Santos",
                "+55 11 98765-4323",
                UserRole.Receptionist,
                _demoTenantId
            ));

            return users;
        }

        private List<Procedure> CreateDemoProcedures()
        {
            return new List<Procedure>
            {
                new Procedure(
                    "Consulta Médica Geral",
                    "CONS-001",
                    "Consulta médica de rotina com clínico geral",
                    ProcedureCategory.Consultation,
                    150.00m,
                    30,
                    _demoTenantId,
                    false
                ),
                new Procedure(
                    "Consulta Cardiológica",
                    "CONS-002",
                    "Consulta com especialista em cardiologia",
                    ProcedureCategory.Consultation,
                    250.00m,
                    45,
                    _demoTenantId,
                    false
                ),
                new Procedure(
                    "Exame de Sangue Completo",
                    "EXAM-001",
                    "Hemograma completo com análise detalhada",
                    ProcedureCategory.Exam,
                    80.00m,
                    15,
                    _demoTenantId,
                    true
                ),
                new Procedure(
                    "Eletrocardiograma",
                    "EXAM-002",
                    "Exame de eletrocardiograma (ECG)",
                    ProcedureCategory.Exam,
                    120.00m,
                    20,
                    _demoTenantId,
                    true
                ),
                new Procedure(
                    "Vacina Influenza",
                    "VAC-001",
                    "Vacinação contra gripe (Influenza)",
                    ProcedureCategory.Vaccination,
                    50.00m,
                    10,
                    _demoTenantId,
                    true
                ),
                new Procedure(
                    "Fisioterapia Sessão",
                    "THER-001",
                    "Sessão de fisioterapia",
                    ProcedureCategory.Therapy,
                    100.00m,
                    60,
                    _demoTenantId,
                    false
                ),
                new Procedure(
                    "Sutura Pequeno Porte",
                    "SURG-001",
                    "Sutura de pequeno porte para ferimentos",
                    ProcedureCategory.Surgery,
                    200.00m,
                    30,
                    _demoTenantId,
                    true
                ),
                new Procedure(
                    "Retorno Consulta",
                    "RET-001",
                    "Consulta de retorno para acompanhamento",
                    ProcedureCategory.FollowUp,
                    80.00m,
                    20,
                    _demoTenantId,
                    false
                )
            };
        }

        private List<Patient> CreateDemoPatients(Guid clinicId)
        {
            var patients = new List<Patient>();

            // Adult patients
            patients.Add(new Patient(
                "Carlos Alberto Santos",
                "123.456.789-00",
                new DateTime(1980, 5, 15),
                "Masculino",
                new Email("carlos.santos@email.com"),
                new Phone("+55", "11987654321"),
                new Address("Rua das Flores", "100", "Centro", "São Paulo", "SP", "01000-000", "Brasil"),
                _demoTenantId,
                "Hipertensão arterial controlada",
                "Penicilina"
            ));

            patients.Add(new Patient(
                "Ana Maria Oliveira",
                "234.567.890-11",
                new DateTime(1975, 8, 20),
                "Feminino",
                new Email("ana.oliveira@email.com"),
                new Phone("+55", "11987654322"),
                new Address("Avenida Paulista", "200", "Bela Vista", "São Paulo", "SP", "01310-100", "Brasil"),
                _demoTenantId,
                "Diabetes tipo 2",
                "Nenhuma"
            ));

            patients.Add(new Patient(
                "Pedro Henrique Costa",
                "345.678.901-22",
                new DateTime(1990, 3, 10),
                "Masculino",
                new Email("pedro.costa@email.com"),
                new Phone("+55", "11987654323"),
                new Address("Rua Augusta", "300", "Consolação", "São Paulo", "SP", "01305-000", "Brasil"),
                _demoTenantId,
                null,
                null
            ));

            // Guardian (mother)
            var guardian = new Patient(
                "Juliana Martins Silva",
                "456.789.012-33",
                new DateTime(1985, 12, 25),
                "Feminino",
                new Email("juliana.martins@email.com"),
                new Phone("+55", "11987654324"),
                new Address("Rua Oscar Freire", "400", "Jardins", "São Paulo", "SP", "01426-001", "Brasil"),
                _demoTenantId,
                null,
                null
            );
            patients.Add(guardian);

            // Children linked to guardian
            var child1 = new Patient(
                "Lucas Martins Silva",
                "567.890.123-44",
                new DateTime(2015, 6, 10),
                "Masculino",
                new Email("lucas.martins@email.com"),
                new Phone("+55", "11987654325"),
                new Address("Rua Oscar Freire", "400", "Jardins", "São Paulo", "SP", "01426-001", "Brasil"),
                _demoTenantId,
                "Asma leve",
                "Nenhuma"
            );
            child1.SetGuardian(guardian.Id);
            patients.Add(child1);

            var child2 = new Patient(
                "Sofia Martins Silva",
                "678.901.234-55",
                new DateTime(2017, 9, 15),
                "Feminino",
                new Email("sofia.martins@email.com"),
                new Phone("+55", "11987654326"),
                new Address("Rua Oscar Freire", "400", "Jardins", "São Paulo", "SP", "01426-001", "Brasil"),
                _demoTenantId,
                null,
                "Lactose"
            );
            child2.SetGuardian(guardian.Id);
            patients.Add(child2);

            return patients;
        }

        private List<PatientClinicLink> CreatePatientClinicLinks(List<Patient> patients, Guid clinicId)
        {
            return patients.Select(p => new PatientClinicLink(p.Id, clinicId, _demoTenantId)).ToList();
        }

        private List<Appointment> CreateDemoAppointments(List<Patient> patients, Guid clinicId)
        {
            var appointments = new List<Appointment>();
            var today = DateTime.Today;

            // Past appointments (completed)
            var pastAppointment1 = new Appointment(
                patients[0].Id, // Carlos
                clinicId,
                today.AddDays(-7),
                new TimeSpan(9, 0, 0),
                30,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta de rotina"
            );
            pastAppointment1.Confirm();
            pastAppointment1.CheckIn();
            pastAppointment1.CheckOut();
            appointments.Add(pastAppointment1);

            var pastAppointment2 = new Appointment(
                patients[1].Id, // Ana
                clinicId,
                today.AddDays(-5),
                new TimeSpan(10, 0, 0),
                45,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta cardiológica"
            );
            pastAppointment2.Confirm();
            pastAppointment2.CheckIn();
            pastAppointment2.CheckOut();
            appointments.Add(pastAppointment2);

            // Today's appointments
            var todayAppointment = new Appointment(
                patients[2].Id, // Pedro
                clinicId,
                today,
                new TimeSpan(14, 0, 0),
                30,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta médica"
            );
            todayAppointment.Confirm();
            appointments.Add(todayAppointment);

            // Future appointments
            appointments.Add(new Appointment(
                patients[4].Id, // Lucas (child)
                clinicId,
                today.AddDays(3),
                new TimeSpan(15, 0, 0),
                20,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta pediátrica"
            ));

            appointments.Add(new Appointment(
                patients[5].Id, // Sofia (child)
                clinicId,
                today.AddDays(3),
                new TimeSpan(15, 30, 0),
                20,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta pediátrica"
            ));

            return appointments;
        }

        private List<AppointmentProcedure> CreateAppointmentProcedures(
            List<Appointment> appointments,
            List<Procedure> procedures,
            List<Patient> patients)
        {
            var appointmentProcedures = new List<AppointmentProcedure>();

            // First completed appointment - General consultation
            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[0].Id,
                procedures[0].Id, // Consulta Geral
                patients[0].Id,
                150.00m,
                DateTime.UtcNow.AddDays(-7),
                _demoTenantId,
                "Consulta realizada"
            ));

            // Second completed appointment - Cardiology + ECG
            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[1].Id,
                procedures[1].Id, // Consulta Cardiológica
                patients[1].Id,
                250.00m,
                DateTime.UtcNow.AddDays(-5),
                _demoTenantId,
                "Consulta cardiológica"
            ));

            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[1].Id,
                procedures[3].Id, // Eletrocardiograma
                patients[1].Id,
                120.00m,
                DateTime.UtcNow.AddDays(-5).AddMinutes(30),
                _demoTenantId,
                "ECG realizado"
            ));

            return appointmentProcedures;
        }

        private List<Payment> CreateDemoPayments(List<Appointment> appointments)
        {
            var payments = new List<Payment>();

            // Payment for first appointment (paid with cash)
            var payment1 = new Payment(
                150.00m,
                PaymentMethod.Cash,
                _demoTenantId,
                appointmentId: appointments[0].Id
            );
            payment1.MarkAsPaid("CASH-" + DateTime.UtcNow.Ticks);
            payments.Add(payment1);

            // Payment for second appointment (paid with credit card)
            var payment2 = new Payment(
                370.00m, // 250 + 120
                PaymentMethod.CreditCard,
                _demoTenantId,
                appointmentId: appointments[1].Id
            );
            payment2.SetCardDetails("4321");
            payment2.MarkAsPaid("CC-" + DateTime.UtcNow.Ticks);
            payments.Add(payment2);

            return payments;
        }

        private List<Medication> CreateDemoMedications()
        {
            return new List<Medication>
            {
                new Medication(
                    "Amoxicilina",
                    "500mg",
                    "Cápsula",
                    MedicationCategory.Antibiotic,
                    true,
                    _demoTenantId,
                    "Amoxicillin",
                    "EMS",
                    "Amoxicilina tri-hidratada",
                    "500mg",
                    "Oral",
                    false,
                    "123456789",
                    null,
                    "Antibiótico de amplo espectro"
                ),
                new Medication(
                    "Dipirona Sódica",
                    "500mg",
                    "Comprimido",
                    MedicationCategory.Analgesic,
                    false,
                    _demoTenantId,
                    "Dipyrone",
                    "Novartis",
                    "Dipirona sódica mono-hidratada",
                    "500mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Analgésico e antitérmico"
                ),
                new Medication(
                    "Ibuprofeno",
                    "600mg",
                    "Comprimido",
                    MedicationCategory.AntiInflammatory,
                    false,
                    _demoTenantId,
                    "Ibuprofen",
                    "Pfizer",
                    "Ibuprofeno",
                    "600mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Anti-inflamatório não esteroidal"
                ),
                new Medication(
                    "Losartana Potássica",
                    "50mg",
                    "Comprimido",
                    MedicationCategory.Antihypertensive,
                    true,
                    _demoTenantId,
                    "Losartan",
                    "Merck",
                    "Losartana potássica",
                    "50mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Anti-hipertensivo"
                ),
                new Medication(
                    "Omeprazol",
                    "20mg",
                    "Cápsula",
                    MedicationCategory.Antacid,
                    true,
                    _demoTenantId,
                    "Omeprazole",
                    "AstraZeneca",
                    "Omeprazol",
                    "20mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Inibidor da bomba de prótons"
                ),
                new Medication(
                    "Loratadina",
                    "10mg",
                    "Comprimido",
                    MedicationCategory.Antihistamine,
                    false,
                    _demoTenantId,
                    "Loratadine",
                    "Schering-Plough",
                    "Loratadina",
                    "10mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Anti-histamínico de 2ª geração"
                ),
                new Medication(
                    "Metformina",
                    "850mg",
                    "Comprimido",
                    MedicationCategory.Antidiabetic,
                    true,
                    _demoTenantId,
                    "Metformin",
                    "Bristol-Myers Squibb",
                    "Cloridrato de metformina",
                    "850mg",
                    "Oral",
                    false,
                    null,
                    null,
                    "Antidiabético oral"
                ),
                new Medication(
                    "Vitamina D3",
                    "7000 UI",
                    "Cápsula",
                    MedicationCategory.Vitamin,
                    false,
                    _demoTenantId,
                    "Cholecalciferol",
                    "Sanofi",
                    "Colecalciferol",
                    "7000 UI",
                    "Oral",
                    false,
                    null,
                    null,
                    "Suplemento vitamínico"
                )
            };
        }

        private List<MedicalRecord> CreateDemoMedicalRecords(List<Appointment> appointments, List<Patient> patients)
        {
            var medicalRecords = new List<MedicalRecord>();

            // Medical record for first completed appointment (Carlos)
            var record1 = new MedicalRecord(
                appointments[0].Id,
                patients[0].Id,
                _demoTenantId,
                DateTime.UtcNow.AddDays(-7).AddHours(9),
                "Hipertensão arterial controlada. Paciente apresenta bom estado geral.",
                "Manter medicação atual. Orientado sobre dieta e exercícios.",
                "Paciente relata controle adequado da pressão arterial. PA: 120/80 mmHg"
            );
            record1.CompleteConsultation(
                "Hipertensão arterial sistêmica (CID I10)",
                "Losartana Potássica 50mg - 1 comprimido ao dia\nDieta hipossódica\nExercícios físicos leves",
                "Retorno em 3 meses para reavaliação"
            );
            medicalRecords.Add(record1);

            // Medical record for second completed appointment (Ana)
            var record2 = new MedicalRecord(
                appointments[1].Id,
                patients[1].Id,
                _demoTenantId,
                DateTime.UtcNow.AddDays(-5).AddHours(10),
                "Diabetes tipo 2. Queixa de palpitações ocasionais.",
                "Solicitado ECG. Ajuste de medicação para controle glicêmico.",
                "Paciente relata episódios de palpitação. Glicemia: 145 mg/dL. ECG normal."
            );
            record2.CompleteConsultation(
                "Diabetes mellitus tipo 2 (CID E11) + Arritmia cardíaca não especificada (CID I49.9)",
                "Metformina 850mg - 2x ao dia\nOmeprazol 20mg - 1x ao dia em jejum\nDieta para diabéticos",
                "Retorno em 1 mês com exames de glicemia e HbA1c"
            );
            medicalRecords.Add(record2);

            return medicalRecords;
        }

        private List<PrescriptionItem> CreateDemoPrescriptionItems(
            List<MedicalRecord> medicalRecords,
            List<Medication> medications,
            List<Patient> patients)
        {
            var items = new List<PrescriptionItem>();

            // Prescription items for first medical record (Carlos - Hypertension)
            items.Add(new PrescriptionItem(
                medicalRecords[0].Id,
                medications[3].Id, // Losartana
                "50mg",
                "1 comprimido ao dia pela manhã em jejum",
                30,
                30,
                _demoTenantId,
                "Tomar pela manhã em jejum"
            ));

            // Prescription items for second medical record (Ana - Diabetes)
            items.Add(new PrescriptionItem(
                medicalRecords[1].Id,
                medications[6].Id, // Metformina
                "850mg",
                "1 comprimido 2x ao dia",
                30,
                60,
                _demoTenantId,
                "Tomar junto com as refeições (almoço e jantar)"
            ));

            items.Add(new PrescriptionItem(
                medicalRecords[1].Id,
                medications[4].Id, // Omeprazol
                "20mg",
                "1 cápsula ao dia",
                30,
                30,
                _demoTenantId,
                "Tomar em jejum, 30 minutos antes do café da manhã"
            ));

            return items;
        }

        private List<PrescriptionTemplate> CreateDemoPrescriptionTemplates()
        {
            return new List<PrescriptionTemplate>
            {
                new PrescriptionTemplate(
                    "Receita Antibiótico Amoxicilina",
                    "Template padrão para prescrição de Amoxicilina",
                    @"Medicamento: {{medication_name}}
Dosagem: {{dosage}}
Posologia: {{frequency}}
Via de administração: {{route}}
Duração do tratamento: {{duration}}

Orientações:
- Tomar conforme orientação médica
- Não interromper o tratamento mesmo se houver melhora dos sintomas
- Em caso de efeitos adversos, procurar atendimento médico",
                    "Antibióticos",
                    _demoTenantId
                ),
                new PrescriptionTemplate(
                    "Receita Anti-hipertensivo",
                    "Template para medicamentos de controle de pressão arterial",
                    @"Medicamento: {{medication_name}}
Dosagem: {{dosage}}
Posologia: {{frequency}}

Orientações importantes:
- Manter uso contínuo conforme prescrição
- Monitorar pressão arterial regularmente
- Dieta hipossódica
- Atividade física regular
- Retorno conforme agendado",
                    "Cardiologia",
                    _demoTenantId
                ),
                new PrescriptionTemplate(
                    "Receita Analgésico Simples",
                    "Template para prescrição de analgésicos de venda livre",
                    @"Medicamento: {{medication_name}}
Dosagem: {{dosage}}
Posologia: Tomar {{frequency}}
Duração: {{duration}}

Observações:
- Tomar preferencialmente após as refeições
- Não ultrapassar a dose máxima diária
- Se persistirem os sintomas, procurar atendimento médico",
                    "Analgésicos",
                    _demoTenantId
                ),
                new PrescriptionTemplate(
                    "Receita Diabetes",
                    "Template para controle de diabetes",
                    @"Medicamento: {{medication_name}}
Dosagem: {{dosage}}
Posologia: {{frequency}}

Plano terapêutico:
- Dieta para diabéticos (acompanhamento nutricional)
- Exercícios físicos regulares
- Monitoramento de glicemia conforme orientado
- Controle de peso
- Hidratação adequada

Retorno: {{return_date}} para reavaliação e ajuste de dose se necessário",
                    "Endocrinologia",
                    _demoTenantId
                )
            };
        }

        private List<MedicalRecordTemplate> CreateDemoMedicalRecordTemplates()
        {
            return new List<MedicalRecordTemplate>
            {
                new MedicalRecordTemplate(
                    "Consulta Clínica Geral",
                    "Template para consultas de clínica geral",
                    @"IDENTIFICAÇÃO DO PACIENTE:
Nome: {{patient_name}}
Data de Nascimento: {{patient_dob}}
CPF: {{patient_cpf}}

ANAMNESE:
Queixa Principal: {{chief_complaint}}
História da Doença Atual: {{hda}}
História Patológica Pregressa: {{hpp}}
Medicações em uso: {{current_medications}}
Alergias: {{allergies}}

EXAME FÍSICO:
Estado Geral: {{general_state}}
PA: {{blood_pressure}} | FC: {{heart_rate}} | FR: {{respiratory_rate}} | Tax: {{temperature}}
Peso: {{weight}} kg | Altura: {{height}} cm | IMC: {{bmi}}

HIPÓTESE DIAGNÓSTICA:
{{diagnosis}}

CONDUTA:
{{treatment_plan}}

OBSERVAÇÕES:
{{notes}}",
                    "Clínica Geral",
                    _demoTenantId
                ),
                new MedicalRecordTemplate(
                    "Consulta Cardiológica",
                    "Template para consultas de cardiologia",
                    @"CONSULTA CARDIOLÓGICA

PACIENTE: {{patient_name}}
DATA: {{date}}

MOTIVO DA CONSULTA:
{{reason}}

HISTÓRIA CARDIOVASCULAR:
{{cardiovascular_history}}

FATORES DE RISCO:
- HAS: {{has_hypertension}}
- DM: {{has_diabetes}}
- Tabagismo: {{smoking}}
- Dislipidemia: {{dyslipidemia}}
- História Familiar: {{family_history}}

EXAME FÍSICO:
PA: {{blood_pressure}}
FC: {{heart_rate}}
Ausculta Cardíaca: {{heart_auscultation}}
Ausculta Pulmonar: {{lung_auscultation}}
Edema: {{edema}}

EXAMES COMPLEMENTARES:
ECG: {{ecg_result}}
Eco: {{echo_result}}

DIAGNÓSTICO:
{{diagnosis}}

CONDUTA:
{{treatment}}

RETORNO: {{return_date}}",
                    "Cardiologia",
                    _demoTenantId
                ),
                new MedicalRecordTemplate(
                    "Consulta Pediátrica",
                    "Template para consultas pediátricas",
                    @"CONSULTA PEDIÁTRICA

IDENTIFICAÇÃO:
Nome: {{patient_name}}
Idade: {{patient_age}}
Responsável: {{guardian_name}}

QUEIXA:
{{chief_complaint}}

DESENVOLVIMENTO:
Peso: {{weight}} kg (P{{weight_percentile}})
Altura: {{height}} cm (P{{height_percentile}})
IMC: {{bmi}}

ALIMENTAÇÃO:
{{feeding_info}}

DESENVOLVIMENTO NEUROPSICOMOTOR:
{{development}}

VACINAÇÃO:
{{vaccination_status}}

EXAME FÍSICO:
{{physical_exam}}

DIAGNÓSTICO:
{{diagnosis}}

ORIENTAÇÕES:
{{guidance}}

PRESCRIÇÃO:
{{prescription}}

RETORNO: {{return_date}}",
                    "Pediatria",
                    _demoTenantId
                )
            };
        }

        private List<Notification> CreateDemoNotifications(List<Appointment> appointments, List<Patient> patients)
        {
            var notifications = new List<Notification>();

            // Notification for past appointment 1 (sent and delivered)
            var notif1 = new Notification(
                patients[0].Id,
                NotificationType.AppointmentReminder,
                NotificationChannel.SMS,
                "+5511987654321",
                "Lembrete: Você tem consulta agendada para amanhã às 09:00. Clínica Demo MedicWarehouse.",
                _demoTenantId,
                appointments[0].Id
            );
            notif1.MarkAsSent();
            notif1.MarkAsDelivered();
            notifications.Add(notif1);

            // Notification for past appointment 2 (sent, delivered and read)
            var notif2 = new Notification(
                patients[1].Id,
                NotificationType.AppointmentReminder,
                NotificationChannel.WhatsApp,
                "+5511987654322",
                "Olá Ana! Lembrete de consulta cardiológica amanhã às 10:00. Clínica Demo MedicWarehouse.",
                _demoTenantId,
                appointments[1].Id
            );
            notif2.MarkAsSent();
            notif2.MarkAsDelivered();
            notif2.MarkAsRead();
            notifications.Add(notif2);

            // Notification for today's appointment (sent)
            var notif3 = new Notification(
                patients[2].Id,
                NotificationType.AppointmentConfirmation,
                NotificationChannel.SMS,
                "+5511987654323",
                "Consulta confirmada para hoje às 14:00. Por favor, chegue 15 minutos antes. Clínica Demo.",
                _demoTenantId,
                appointments[2].Id
            );
            notif3.MarkAsSent();
            notifications.Add(notif3);

            // Notification for future appointment (pending)
            var notif4 = new Notification(
                patients[4].Id,
                NotificationType.AppointmentReminder,
                NotificationChannel.WhatsApp,
                "+5511987654325",
                "Olá! Lucas tem consulta pediátrica agendada para " + DateTime.Today.AddDays(3).ToString("dd/MM/yyyy") + " às 15:00.",
                _demoTenantId,
                appointments[3].Id
            );
            notifications.Add(notif4);

            // Payment reminder notification
            var notif5 = new Notification(
                patients[0].Id,
                NotificationType.PaymentReminder,
                NotificationChannel.Email,
                "carlos.santos@email.com",
                "Lembrete: Pagamento da consulta realizada em " + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + " foi confirmado. Obrigado!",
                _demoTenantId
            );
            notif5.MarkAsSent();
            notif5.MarkAsDelivered();
            notifications.Add(notif5);

            return notifications;
        }
    }
}
