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
    }
}
