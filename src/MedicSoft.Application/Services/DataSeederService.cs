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
        private readonly INotificationRoutineRepository _notificationRoutineRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IClinicSubscriptionRepository _clinicSubscriptionRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExamRequestRepository _examRequestRepository;
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
            INotificationRoutineRepository notificationRoutineRepository,
            IPasswordHasher passwordHasher,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IClinicSubscriptionRepository clinicSubscriptionRepository,
            IOwnerRepository ownerRepository,
            IExpenseRepository expenseRepository,
            IExamRequestRepository examRequestRepository)
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
            _notificationRoutineRepository = notificationRoutineRepository;
            _passwordHasher = passwordHasher;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _clinicSubscriptionRepository = clinicSubscriptionRepository;
            _ownerRepository = ownerRepository;
            _expenseRepository = expenseRepository;
            _examRequestRepository = examRequestRepository;
        }

        public async Task SeedDemoDataAsync()
        {
            // Execute all seeding operations in a transaction to ensure data consistency
            await _clinicRepository.ExecuteInTransactionAsync(async () =>
            {
                // Check if demo data already exists
                var existingClinics = await _clinicRepository.GetAllAsync(_demoTenantId);
                if (existingClinics.Any())
                {
                    throw new InvalidOperationException("Demo data already exists for this tenant");
                }

                // 0. Create Subscription Plans (system-wide)
            var subscriptionPlans = CreateDemoSubscriptionPlans();
            foreach (var plan in subscriptionPlans)
            {
                await _subscriptionPlanRepository.AddAsync(plan);
            }

            // 1. Create Demo Clinic
            var clinic = CreateDemoClinic();
            await _clinicRepository.AddAsync(clinic);

            // 1.1. Create Clinic Subscription
            var clinicSubscription = CreateClinicSubscription(clinic.Id, subscriptionPlans[2].Id); // Standard plan
            await _clinicSubscriptionRepository.AddAsync(clinicSubscription);

            // 1.2. Create Demo Owner for the clinic
            var owner = CreateDemoOwner();
            await _ownerRepository.AddAsync(owner);

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

            // 15. Create Notification Routines
            var notificationRoutines = CreateDemoNotificationRoutines(clinic.Id);
            foreach (var routine in notificationRoutines)
            {
                await _notificationRoutineRepository.AddAsync(routine);
            }

            // 16. Create Expenses
            var expenses = CreateDemoExpenses(clinic.Id);
            foreach (var expense in expenses)
            {
                await _expenseRepository.AddAsync(expense);
            }

            // 17. Create Exam Requests
            var examRequests = CreateDemoExamRequests(appointments, patients);
            foreach (var examRequest in examRequests)
            {
                await _examRequestRepository.AddAsync(examRequest);
            }
            });
        }

        public async Task ClearDatabaseAsync()
        {
            // Execute all deletion operations in a transaction to ensure data consistency
            await _clinicRepository.ExecuteInTransactionAsync(async () =>
            {
                // Delete data in the correct order to respect foreign key constraints
                // Delete child entities first, then parent entities
                
                // 1. Delete PrescriptionItems (depends on MedicalRecords and Medications)
            var prescriptionItems = await _prescriptionItemRepository.GetAllAsync(_demoTenantId);
            foreach (var item in prescriptionItems)
            {
                await _prescriptionItemRepository.DeleteAsync(item.Id, _demoTenantId);
            }

            // 2. Delete ExamRequests (depends on Appointments and Patients)
            var examRequests = await _examRequestRepository.GetAllAsync(_demoTenantId);
            foreach (var examRequest in examRequests)
            {
                await _examRequestRepository.DeleteAsync(examRequest.Id, _demoTenantId);
            }

            // 3. Delete Notifications (depends on Patients and Appointments)
            var notifications = await _notificationRepository.GetAllAsync(_demoTenantId);
            foreach (var notification in notifications)
            {
                await _notificationRepository.DeleteAsync(notification.Id, _demoTenantId);
            }

            // 4. Delete NotificationRoutines
            var notificationRoutines = await _notificationRoutineRepository.GetAllAsync(_demoTenantId);
            foreach (var routine in notificationRoutines)
            {
                await _notificationRoutineRepository.DeleteAsync(routine.Id, _demoTenantId);
            }

            // 5. Delete MedicalRecords (depends on Appointments and Patients)
            var medicalRecords = await _medicalRecordRepository.GetAllAsync(_demoTenantId);
            foreach (var record in medicalRecords)
            {
                await _medicalRecordRepository.DeleteAsync(record.Id, _demoTenantId);
            }

            // 6. Delete Payments (depends on Appointments)
            var payments = await _paymentRepository.GetAllAsync(_demoTenantId);
            foreach (var payment in payments)
            {
                await _paymentRepository.DeleteAsync(payment.Id, _demoTenantId);
            }

            // 7. Delete AppointmentProcedures (depends on Appointments and Procedures)
            var appointmentProcedures = await _appointmentProcedureRepository.GetAllAsync(_demoTenantId);
            foreach (var ap in appointmentProcedures)
            {
                await _appointmentProcedureRepository.DeleteAsync(ap.Id, _demoTenantId);
            }

            // 8. Delete Appointments
            var appointments = await _appointmentRepository.GetAllAsync(_demoTenantId);
            foreach (var appointment in appointments)
            {
                await _appointmentRepository.DeleteAsync(appointment.Id, _demoTenantId);
            }

            // 9. Delete PatientClinicLinks
            var patientLinks = await _patientClinicLinkRepository.GetAllAsync(_demoTenantId);
            foreach (var link in patientLinks)
            {
                await _patientClinicLinkRepository.DeleteAsync(link.Id, _demoTenantId);
            }

            // 10. Delete Patients
            var patients = await _patientRepository.GetAllAsync(_demoTenantId);
            foreach (var patient in patients)
            {
                await _patientRepository.DeleteAsync(patient.Id, _demoTenantId);
            }

            // 11. Delete PrescriptionTemplates
            var prescriptionTemplates = await _prescriptionTemplateRepository.GetAllAsync(_demoTenantId);
            foreach (var template in prescriptionTemplates)
            {
                await _prescriptionTemplateRepository.DeleteAsync(template.Id, _demoTenantId);
            }

            // 12. Delete MedicalRecordTemplates
            var medicalRecordTemplates = await _medicalRecordTemplateRepository.GetAllAsync(_demoTenantId);
            foreach (var template in medicalRecordTemplates)
            {
                await _medicalRecordTemplateRepository.DeleteAsync(template.Id, _demoTenantId);
            }

            // 13. Delete Medications
            var medications = await _medicationRepository.GetAllAsync(_demoTenantId);
            foreach (var medication in medications)
            {
                await _medicationRepository.DeleteAsync(medication.Id, _demoTenantId);
            }

            // 14. Delete Procedures
            var procedures = await _procedureRepository.GetAllAsync(_demoTenantId);
            foreach (var procedure in procedures)
            {
                await _procedureRepository.DeleteAsync(procedure.Id, _demoTenantId);
            }

            // 15. Delete Expenses
            var expenses = await _expenseRepository.GetAllAsync(_demoTenantId);
            foreach (var expense in expenses)
            {
                await _expenseRepository.DeleteAsync(expense.Id, _demoTenantId);
            }

            // 16. Delete Clinics
            var clinics = await _clinicRepository.GetAllAsync(_demoTenantId);
            foreach (var clinic in clinics)
            {
                await _clinicRepository.DeleteAsync(clinic.Id, _demoTenantId);
            }

            // 17. Delete SubscriptionPlans (system-wide, delete the demo plans)
            var subscriptionPlans = await _subscriptionPlanRepository.GetAllAsync("system");
            foreach (var plan in subscriptionPlans)
            {
                await _subscriptionPlanRepository.DeleteAsync(plan.Id, "system");
            }

                // Note: Users, Owners, and ClinicSubscriptions don't have standard GetAllAsync/DeleteAsync methods
                // in their repository interfaces. These entities may cascade delete when their parent
                // entities (Clinics) are deleted, depending on the database foreign key configuration.
                // If manual deletion is needed, it can be implemented by extending the repository interfaces.
            });
        }

        private Clinic CreateDemoClinic()
        {
            return new Clinic(
                "Clínica Demo MedicWarehouse",
                "Clínica Demo",
                "12.345.678/0001-95",
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

            // Past appointments (completed) - using allowHistoricalData flag
            var pastAppointment1 = new Appointment(
                patients[0].Id, // Carlos
                clinicId,
                today.AddDays(-7),
                new TimeSpan(9, 0, 0),
                30,
                AppointmentType.Regular,
                _demoTenantId,
                "Consulta de rotina",
                allowHistoricalData: true
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
                "Consulta cardiológica",
                allowHistoricalData: true
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
            var today = DateTime.Today;

            // First completed appointment - General consultation (7 days ago at 9:00)
            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[0].Id,
                procedures[0].Id, // Consulta Geral
                patients[0].Id,
                150.00m,
                today.AddDays(-7).Add(new TimeSpan(9, 0, 0)),
                _demoTenantId,
                "Consulta realizada"
            ));

            // Second completed appointment - Cardiology + ECG (5 days ago at 10:00)
            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[1].Id,
                procedures[1].Id, // Consulta Cardiológica
                patients[1].Id,
                250.00m,
                today.AddDays(-5).Add(new TimeSpan(10, 0, 0)),
                _demoTenantId,
                "Consulta cardiológica"
            ));

            appointmentProcedures.Add(new AppointmentProcedure(
                appointments[1].Id,
                procedures[3].Id, // Eletrocardiograma
                patients[1].Id,
                120.00m,
                today.AddDays(-5).Add(new TimeSpan(10, 30, 0)),
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
            var today = DateTime.Today;

            // Medical record for first completed appointment (Carlos) - 7 days ago at 9:00
            var record1 = new MedicalRecord(
                appointments[0].Id,
                patients[0].Id,
                _demoTenantId,
                today.AddDays(-7).Add(new TimeSpan(9, 0, 0)),
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

            // Medical record for second completed appointment (Ana) - 5 days ago at 10:00
            var record2 = new MedicalRecord(
                appointments[1].Id,
                patients[1].Id,
                _demoTenantId,
                today.AddDays(-5).Add(new TimeSpan(10, 0, 0)),
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

        private List<SubscriptionPlan> CreateDemoSubscriptionPlans()
        {
            return new List<SubscriptionPlan>
            {
                new SubscriptionPlan(
                    "Trial Gratuito",
                    "Plano de teste gratuito por 30 dias com funcionalidades básicas",
                    0.00m,
                    30,
                    3,
                    50,
                    PlanType.Trial,
                    "system",
                    hasReports: false,
                    hasWhatsAppIntegration: false,
                    hasSMSNotifications: false,
                    hasTissExport: false
                ),
                new SubscriptionPlan(
                    "Básico",
                    "Plano básico para pequenas clínicas com funcionalidades essenciais",
                    99.90m,
                    15,
                    5,
                    100,
                    PlanType.Basic,
                    "system",
                    hasReports: true,
                    hasWhatsAppIntegration: false,
                    hasSMSNotifications: true,
                    hasTissExport: false
                ),
                new SubscriptionPlan(
                    "Standard",
                    "Plano completo para clínicas médicas com todas as funcionalidades",
                    199.90m,
                    15,
                    15,
                    500,
                    PlanType.Standard,
                    "system",
                    hasReports: true,
                    hasWhatsAppIntegration: true,
                    hasSMSNotifications: true,
                    hasTissExport: true
                ),
                new SubscriptionPlan(
                    "Premium",
                    "Plano premium para grandes clínicas e hospitais",
                    399.90m,
                    15,
                    50,
                    2000,
                    PlanType.Premium,
                    "system",
                    hasReports: true,
                    hasWhatsAppIntegration: true,
                    hasSMSNotifications: true,
                    hasTissExport: true
                ),
                new SubscriptionPlan(
                    "Enterprise",
                    "Plano corporativo para redes de clínicas e hospitais com suporte dedicado",
                    999.90m,
                    30,
                    200,
                    10000,
                    PlanType.Enterprise,
                    "system",
                    hasReports: true,
                    hasWhatsAppIntegration: true,
                    hasSMSNotifications: true,
                    hasTissExport: true
                )
            };
        }

        private ClinicSubscription CreateClinicSubscription(Guid clinicId, Guid subscriptionPlanId)
        {
            var startDate = DateTime.UtcNow.AddDays(-15); // Started 15 days ago
            
            var subscription = new ClinicSubscription(
                clinicId,
                subscriptionPlanId,
                startDate,
                15, // trial days
                199.90m, // Standard plan price
                _demoTenantId
            );
            
            // Activate the subscription
            subscription.Activate();
            
            return subscription;
        }

        private Owner CreateDemoOwner()
        {
            var ownerPassword = _passwordHasher.HashPassword("Owner@123");
            return new Owner(
                "owner.demo",
                "owner@clinicademo.com.br",
                ownerPassword,
                "Dr. Roberto Almeida",
                "+55 11 98765-4320",
                _demoTenantId
            );
        }

        private List<NotificationRoutine> CreateDemoNotificationRoutines(Guid clinicId)
        {
            return new List<NotificationRoutine>
            {
                new NotificationRoutine(
                    "Lembrete de Consulta - 24h Antes",
                    "Rotina automática para enviar lembrete de consulta 24 horas antes via WhatsApp",
                    NotificationChannel.WhatsApp,
                    NotificationType.AppointmentReminder,
                    "Olá {{patient_name}}! Lembrete: Você tem consulta agendada amanhã às {{appointment_time}}. Clínica {{clinic_name}}. Favor chegar 15 minutos antes.",
                    RoutineScheduleType.BeforeAppointment,
                    "{\"hours_before\": 24}",
                    RoutineScope.Clinic,
                    _demoTenantId,
                    maxRetries: 3
                ),
                new NotificationRoutine(
                    "Lembrete de Consulta - 2h Antes (SMS)",
                    "Rotina para enviar lembrete via SMS 2 horas antes da consulta",
                    NotificationChannel.SMS,
                    NotificationType.AppointmentReminder,
                    "Lembrete: Consulta hoje às {{appointment_time}}. {{clinic_name}}. Chegar 15min antes.",
                    RoutineScheduleType.BeforeAppointment,
                    "{\"hours_before\": 2}",
                    RoutineScope.Clinic,
                    _demoTenantId,
                    maxRetries: 2
                ),
                new NotificationRoutine(
                    "Confirmação de Agendamento",
                    "Envio imediato de confirmação após agendamento de consulta",
                    NotificationChannel.Email,
                    NotificationType.AppointmentConfirmation,
                    "Olá {{patient_name}},\n\nSua consulta foi agendada com sucesso!\n\nData: {{appointment_date}}\nHorário: {{appointment_time}}\nMédico: {{doctor_name}}\n\nClínica {{clinic_name}}\n{{clinic_address}}\n{{clinic_phone}}",
                    RoutineScheduleType.Custom,
                    "{\"trigger\": \"on_appointment_created\"}",
                    RoutineScope.Clinic,
                    _demoTenantId,
                    maxRetries: 3
                ),
                new NotificationRoutine(
                    "Aniversário do Paciente",
                    "Envio de mensagem de aniversário aos pacientes",
                    NotificationChannel.WhatsApp,
                    NotificationType.General,
                    "Parabéns {{patient_name}}! 🎉🎂 A equipe da {{clinic_name}} deseja um feliz aniversário! Que este novo ano seja repleto de saúde e felicidade!",
                    RoutineScheduleType.Daily,
                    "{\"time\": \"09:00\"}",
                    RoutineScope.Clinic,
                    _demoTenantId,
                    maxRetries: 1
                ),
                new NotificationRoutine(
                    "Pesquisa de Satisfação",
                    "Envio de pesquisa de satisfação 24 horas após a consulta",
                    NotificationChannel.Email,
                    NotificationType.General,
                    "Olá {{patient_name}},\n\nGostaríamos de saber como foi sua experiência na consulta de {{appointment_date}}.\n\nPor favor, avalie nosso atendimento: [link_pesquisa]\n\nSua opinião é muito importante para nós!\n\nAtenciosamente,\n{{clinic_name}}",
                    RoutineScheduleType.AfterAppointment,
                    "{\"hours_after\": 24}",
                    RoutineScope.Clinic,
                    _demoTenantId,
                    maxRetries: 2
                )
            };
        }

        private List<Expense> CreateDemoExpenses(Guid clinicId)
        {
            var today = DateTime.Today;
            var expenses = new List<Expense>();

            // Aluguel - Pago
            var rent = new Expense(
                clinicId,
                "Aluguel do consultório - Mês atual",
                ExpenseCategory.Rent,
                3500.00m,
                today.AddDays(-25),
                _demoTenantId,
                "Imobiliária Santos",
                "12.345.678/0001-95",
                "Aluguel mensal do espaço da clínica"
            );
            rent.MarkAsPaid(PaymentMethod.BankTransfer, "TRF-2024-001");
            expenses.Add(rent);

            // Conta de luz - Pago
            var electricity = new Expense(
                clinicId,
                "Conta de energia elétrica",
                ExpenseCategory.Utilities,
                450.00m,
                today.AddDays(-20),
                _demoTenantId,
                "Companhia de Energia",
                null,
                "Consumo referente ao mês anterior"
            );
            electricity.MarkAsPaid(PaymentMethod.Pix, "PIX-2024-015");
            expenses.Add(electricity);

            // Internet e telefone - Pago
            var internet = new Expense(
                clinicId,
                "Internet fibra ótica 200MB + telefone",
                ExpenseCategory.Utilities,
                199.90m,
                today.AddDays(-18),
                _demoTenantId,
                "Provedor de Internet",
                "98.765.432/0001-98",
                "Plano empresarial"
            );
            internet.MarkAsPaid(PaymentMethod.CreditCard, "CC-2024-045");
            expenses.Add(internet);

            // Material de limpeza - Pago
            var cleaning = new Expense(
                clinicId,
                "Material de limpeza e higienização",
                ExpenseCategory.Supplies,
                350.00m,
                today.AddDays(-15),
                _demoTenantId,
                "Distribuidora de Produtos",
                "11.222.333/0001-81",
                "Álcool, desinfetantes, papel toalha, etc."
            );
            cleaning.MarkAsPaid(PaymentMethod.Cash, null);
            expenses.Add(cleaning);

            // Software de gestão - Pendente
            var software = new Expense(
                clinicId,
                "Assinatura sistema de gestão - MedicWarehouse",
                ExpenseCategory.Software,
                199.90m,
                today.AddDays(5),
                _demoTenantId,
                "MedicWarehouse Ltda",
                "22.333.444/0001-81",
                "Plano Standard mensal"
            );
            expenses.Add(software);

            // Material médico - Pendente
            var medical = new Expense(
                clinicId,
                "Material médico descartável",
                ExpenseCategory.Supplies,
                890.00m,
                today.AddDays(10),
                _demoTenantId,
                "Distribuidora Médica",
                "33.444.555/0001-81",
                "Luvas, seringas, gazes, curativos"
            );
            expenses.Add(medical);

            // Manutenção equipamentos - Vencido
            var maintenance = new Expense(
                clinicId,
                "Manutenção preventiva ar condicionado",
                ExpenseCategory.Maintenance,
                280.00m,
                today.AddDays(-5),
                _demoTenantId,
                "Climatização e Refrigeração",
                "44.555.666/0001-81",
                "Limpeza e manutenção preventiva"
            );
            maintenance.CheckOverdue();
            expenses.Add(maintenance);

            // Contador - Pendente
            var accounting = new Expense(
                clinicId,
                "Serviços contábeis mensais",
                ExpenseCategory.ProfessionalServices,
                650.00m,
                today.AddDays(15),
                _demoTenantId,
                "Contabilidade Empresarial",
                "55.666.777/0001-81",
                "Honorários contábeis do mês"
            );
            expenses.Add(accounting);

            // Marketing - Pago
            var marketing = new Expense(
                clinicId,
                "Publicidade em redes sociais",
                ExpenseCategory.Marketing,
                500.00m,
                today.AddDays(-10),
                _demoTenantId,
                "Agência Digital",
                "66.777.888/0001-81",
                "Gestão de mídias sociais e anúncios"
            );
            marketing.MarkAsPaid(PaymentMethod.Pix, "PIX-2024-028");
            expenses.Add(marketing);

            // Treinamento - Cancelado
            var training = new Expense(
                clinicId,
                "Curso de atualização médica",
                ExpenseCategory.Training,
                1200.00m,
                today.AddDays(20),
                _demoTenantId,
                "Centro de Educação Médica",
                "77.888.999/0001-81",
                "Curso cancelado devido a conflito de agenda"
            );
            training.Cancel("Conflito de agenda - curso remarcado para próximo mês");
            expenses.Add(training);

            return expenses;
        }

        private List<ExamRequest> CreateDemoExamRequests(List<Appointment> appointments, List<Patient> patients)
        {
            var examRequests = new List<ExamRequest>();
            var today = DateTime.Today;

            // Exam request for first completed appointment (Carlos - Hypertension) - 7 days ago
            var exam1 = new ExamRequest(
                appointments[0].Id,
                patients[0].Id,
                ExamType.Laboratory,
                "Hemograma completo, Glicemia em jejum, Perfil lipídico",
                "Exames de rotina para controle de hipertensão",
                ExamUrgency.Routine,
                _demoTenantId,
                "Em jejum de 12 horas",
                requestedDate: today.AddDays(-7).Add(new TimeSpan(9, 0, 0))
            );
            exam1.Complete("Hemograma: Normal\nGlicemia: 95 mg/dL\nColesterol total: 180 mg/dL\nHDL: 55 mg/dL\nLDL: 110 mg/dL\nTriglicerídeos: 120 mg/dL");
            examRequests.Add(exam1);

            // Exam request for second completed appointment (Ana - Diabetes and Arrhythmia) - 5 days ago
            var exam2 = new ExamRequest(
                appointments[1].Id,
                patients[1].Id,
                ExamType.Laboratory,
                "Hemograma, Glicemia em jejum, Hemoglobina glicada (HbA1c)",
                "Avaliação e controle glicêmico",
                ExamUrgency.Urgent,
                _demoTenantId,
                "Jejum de 12 horas para exames laboratoriais",
                requestedDate: today.AddDays(-5).Add(new TimeSpan(10, 0, 0))
            );
            exam2.Complete("Hemograma: Normal\nGlicemia: 145 mg/dL (elevada)\nHbA1c: 7.2% (controle inadequado)");
            examRequests.Add(exam2);

            // ECG for Ana - 5 days ago
            var exam2b = new ExamRequest(
                appointments[1].Id,
                patients[1].Id,
                ExamType.Cardiac,
                "Eletrocardiograma (ECG)",
                "Investigação de palpitações",
                ExamUrgency.Urgent,
                _demoTenantId,
                "Trazer lista de medicamentos em uso",
                requestedDate: today.AddDays(-5).Add(new TimeSpan(10, 15, 0))
            );
            exam2b.Complete("ECG: Ritmo sinusal, sem alterações agudas");
            examRequests.Add(exam2b);

            // Exam request for today's appointment (Pedro) - Pending
            var exam3 = new ExamRequest(
                appointments[2].Id,
                patients[2].Id,
                ExamType.Imaging,
                "Raio-X de tórax PA e perfil",
                "Investigação de tosse persistente",
                ExamUrgency.Routine,
                _demoTenantId,
                "Levar exames anteriores se disponível"
            );
            examRequests.Add(exam3);

            // Additional exam request - Scheduled for 5 days in the future
            var exam4 = new ExamRequest(
                appointments[1].Id,
                patients[1].Id,
                ExamType.Cardiac,
                "Ecocardiograma com doppler",
                "Avaliação detalhada da função cardíaca devido a queixas de palpitação",
                ExamUrgency.Urgent,
                _demoTenantId,
                "Trazer ECG anterior",
                requestedDate: today.AddDays(-5).Add(new TimeSpan(10, 30, 0))
            );
            exam4.Schedule(DateTime.UtcNow.AddDays(5));
            examRequests.Add(exam4);

            // Ultrasound request - Pending (from 7 days ago)
            var exam5 = new ExamRequest(
                appointments[0].Id,
                patients[0].Id,
                ExamType.Ultrasound,
                "Ultrassom de abdômen total",
                "Avaliação de rotina",
                ExamUrgency.Routine,
                _demoTenantId,
                notes: null,
                requestedDate: today.AddDays(-7).Add(new TimeSpan(9, 15, 0))
            );
            examRequests.Add(exam5);

            return examRequests;
        }
    }
}
