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
        private readonly IExamCatalogRepository _examCatalogRepository;
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
            IExamRequestRepository examRequestRepository,
            IExamCatalogRepository examCatalogRepository)
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
            _examCatalogRepository = examCatalogRepository;
        }

        public async Task SeedDemoDataAsync()
        {
            // Execute all seeding operations in a transaction to ensure data consistency
            // Using AddWithoutSaveAsync to batch all operations and avoid multiple SaveChanges calls
            // which cause issues with NpgsqlRetryingExecutionStrategy
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
                    await _subscriptionPlanRepository.AddWithoutSaveAsync(plan);
                }

                // 1. Create Demo Clinic
                var clinic = CreateDemoClinic();
                await _clinicRepository.AddWithoutSaveAsync(clinic);

                // 1.1. Create Clinic Subscription
                var clinicSubscription = CreateClinicSubscription(clinic.Id, subscriptionPlans[2].Id); // Standard plan
                await _clinicSubscriptionRepository.AddWithoutSaveAsync(clinicSubscription);

                // 1.2. Create Demo Owner for the clinic
                var owner = CreateDemoOwner();
                await _ownerRepository.AddWithoutSaveAsync(owner);

                // 2. Create Users (Admin, Doctor, Receptionist)
                var users = CreateDemoUsers();
                foreach (var user in users)
                {
                    await _userRepository.AddWithoutSaveAsync(user);
                }

                // 3. Create Procedures/Services
                var procedures = CreateDemoProcedures();
                foreach (var procedure in procedures)
                {
                    await _procedureRepository.AddWithoutSaveAsync(procedure);
                }

                // 4. Create Patients
                var patients = CreateDemoPatients(clinic.Id);
                foreach (var patient in patients)
                {
                    await _patientRepository.AddWithoutSaveAsync(patient);
                }

                // 5. Link patients to clinic
                var patientLinks = CreatePatientClinicLinks(patients, clinic.Id);
                foreach (var link in patientLinks)
                {
                    await _patientClinicLinkRepository.AddWithoutSaveAsync(link);
                }

                // 6. Create Appointments
                var appointments = CreateDemoAppointments(patients, clinic.Id);
                foreach (var appointment in appointments)
                {
                    await _appointmentRepository.AddWithoutSaveAsync(appointment);
                }

                // 7. Create Appointment Procedures
                var appointmentProcedures = CreateAppointmentProcedures(appointments, procedures, patients);
                foreach (var ap in appointmentProcedures)
                {
                    await _appointmentProcedureRepository.AddWithoutSaveAsync(ap);
                }

                // 8. Create Payments for some appointments
                var payments = CreateDemoPayments(appointments);
                foreach (var payment in payments)
                {
                    await _paymentRepository.AddWithoutSaveAsync(payment);
                }

                // 9. Create Medications
                var medications = CreateDemoMedications();
                foreach (var medication in medications)
                {
                    await _medicationRepository.AddWithoutSaveAsync(medication);
                }

                // 10. Create Medical Records for completed appointments
                var medicalRecords = CreateDemoMedicalRecords(appointments, patients);
                foreach (var record in medicalRecords)
                {
                    await _medicalRecordRepository.AddWithoutSaveAsync(record);
                }

                // 11. Create Prescription Items
                var prescriptionItems = CreateDemoPrescriptionItems(medicalRecords, medications, patients);
                foreach (var item in prescriptionItems)
                {
                    await _prescriptionItemRepository.AddWithoutSaveAsync(item);
                }

                // 12. Create Prescription Templates
                var prescriptionTemplates = CreateDemoPrescriptionTemplates();
                foreach (var template in prescriptionTemplates)
                {
                    await _prescriptionTemplateRepository.AddWithoutSaveAsync(template);
                }

                // 13. Create Medical Record Templates
                var medicalRecordTemplates = CreateDemoMedicalRecordTemplates();
                foreach (var template in medicalRecordTemplates)
                {
                    await _medicalRecordTemplateRepository.AddWithoutSaveAsync(template);
                }

                // 14. Create Notifications for appointments
                var notifications = CreateDemoNotifications(appointments, patients);
                foreach (var notification in notifications)
                {
                    await _notificationRepository.AddWithoutSaveAsync(notification);
                }

                // 15. Create Notification Routines
                var notificationRoutines = CreateDemoNotificationRoutines(clinic.Id);
                foreach (var routine in notificationRoutines)
                {
                    await _notificationRoutineRepository.AddWithoutSaveAsync(routine);
                }

                // 16. Create Expenses
                var expenses = CreateDemoExpenses(clinic.Id);
                foreach (var expense in expenses)
                {
                    await _expenseRepository.AddWithoutSaveAsync(expense);
                }

                // 17. Create Exam Requests
                var examRequests = CreateDemoExamRequests(appointments, patients);
                foreach (var examRequest in examRequests)
                {
                    await _examRequestRepository.AddWithoutSaveAsync(examRequest);
                }

                // 18. Create Exam Catalog (comprehensive list for autocomplete)
                var examCatalogs = CreateDemoExamCatalogs();
                foreach (var examCatalog in examCatalogs)
                {
                    await _examCatalogRepository.AddWithoutSaveAsync(examCatalog);
                }

                // Save all changes at once at the end of the transaction
                // This is compatible with NpgsqlRetryingExecutionStrategy
                await _clinicRepository.SaveChangesAsync();
            });
        }

        public async Task ClearDatabaseAsync()
        {
            // Execute all deletion operations in a transaction to ensure data consistency
            // Using DeleteWithoutSaveAsync to batch all operations and avoid multiple SaveChanges calls
            // which cause issues with NpgsqlRetryingExecutionStrategy
            await _clinicRepository.ExecuteInTransactionAsync(async () =>
            {
                // Delete data in the correct order to respect foreign key constraints
                // Delete child entities first, then parent entities
                
                // 1. Delete PrescriptionItems (depends on MedicalRecords and Medications)
                var prescriptionItems = await _prescriptionItemRepository.GetAllAsync(_demoTenantId);
                foreach (var item in prescriptionItems)
                {
                    await _prescriptionItemRepository.DeleteWithoutSaveAsync(item.Id, _demoTenantId);
                }

                // 2. Delete ExamRequests (depends on Appointments and Patients)
                var examRequests = await _examRequestRepository.GetAllAsync(_demoTenantId);
                foreach (var examRequest in examRequests)
                {
                    await _examRequestRepository.DeleteWithoutSaveAsync(examRequest.Id, _demoTenantId);
                }

                // 3. Delete Notifications (depends on Patients and Appointments)
                var notifications = await _notificationRepository.GetAllAsync(_demoTenantId);
                foreach (var notification in notifications)
                {
                    await _notificationRepository.DeleteWithoutSaveAsync(notification.Id, _demoTenantId);
                }

                // 4. Delete NotificationRoutines
                var notificationRoutines = await _notificationRoutineRepository.GetAllAsync(_demoTenantId);
                foreach (var routine in notificationRoutines)
                {
                    await _notificationRoutineRepository.DeleteWithoutSaveAsync(routine.Id, _demoTenantId);
                }

                // 5. Delete MedicalRecords (depends on Appointments and Patients)
                var medicalRecords = await _medicalRecordRepository.GetAllAsync(_demoTenantId);
                foreach (var record in medicalRecords)
                {
                    await _medicalRecordRepository.DeleteWithoutSaveAsync(record.Id, _demoTenantId);
                }

                // 6. Delete Payments (depends on Appointments)
                var payments = await _paymentRepository.GetAllAsync(_demoTenantId);
                foreach (var payment in payments)
                {
                    await _paymentRepository.DeleteWithoutSaveAsync(payment.Id, _demoTenantId);
                }

                // 7. Delete AppointmentProcedures (depends on Appointments and Procedures)
                var appointmentProcedures = await _appointmentProcedureRepository.GetAllAsync(_demoTenantId);
                foreach (var ap in appointmentProcedures)
                {
                    await _appointmentProcedureRepository.DeleteWithoutSaveAsync(ap.Id, _demoTenantId);
                }

                // 8. Delete Appointments
                var appointments = await _appointmentRepository.GetAllAsync(_demoTenantId);
                foreach (var appointment in appointments)
                {
                    await _appointmentRepository.DeleteWithoutSaveAsync(appointment.Id, _demoTenantId);
                }

                // 9. Delete PatientClinicLinks
                var patientLinks = await _patientClinicLinkRepository.GetAllAsync(_demoTenantId);
                foreach (var link in patientLinks)
                {
                    await _patientClinicLinkRepository.DeleteWithoutSaveAsync(link.Id, _demoTenantId);
                }

                // 10. Delete Patients
                var patients = await _patientRepository.GetAllAsync(_demoTenantId);
                foreach (var patient in patients)
                {
                    await _patientRepository.DeleteWithoutSaveAsync(patient.Id, _demoTenantId);
                }

                // 11. Delete PrescriptionTemplates
                var prescriptionTemplates = await _prescriptionTemplateRepository.GetAllAsync(_demoTenantId);
                foreach (var template in prescriptionTemplates)
                {
                    await _prescriptionTemplateRepository.DeleteWithoutSaveAsync(template.Id, _demoTenantId);
                }

                // 12. Delete MedicalRecordTemplates
                var medicalRecordTemplates = await _medicalRecordTemplateRepository.GetAllAsync(_demoTenantId);
                foreach (var template in medicalRecordTemplates)
                {
                    await _medicalRecordTemplateRepository.DeleteWithoutSaveAsync(template.Id, _demoTenantId);
                }

                // 13. Delete Medications
                var medications = await _medicationRepository.GetAllAsync(_demoTenantId);
                foreach (var medication in medications)
                {
                    await _medicationRepository.DeleteWithoutSaveAsync(medication.Id, _demoTenantId);
                }

                // 13.1. Delete ExamCatalogs
                var examCatalogs = await _examCatalogRepository.GetAllAsync(_demoTenantId);
                foreach (var examCatalog in examCatalogs)
                {
                    await _examCatalogRepository.DeleteWithoutSaveAsync(examCatalog.Id, _demoTenantId);
                }

                // 14. Delete Procedures
                var procedures = await _procedureRepository.GetAllAsync(_demoTenantId);
                foreach (var procedure in procedures)
                {
                    await _procedureRepository.DeleteWithoutSaveAsync(procedure.Id, _demoTenantId);
                }

                // 15. Delete Expenses
                var expenses = await _expenseRepository.GetAllAsync(_demoTenantId);
                foreach (var expense in expenses)
                {
                    await _expenseRepository.DeleteWithoutSaveAsync(expense.Id, _demoTenantId);
                }

                // 16. Delete Clinics
                var clinics = await _clinicRepository.GetAllAsync(_demoTenantId);
                foreach (var clinic in clinics)
                {
                    await _clinicRepository.DeleteWithoutSaveAsync(clinic.Id, _demoTenantId);
                }

                // 17. Delete SubscriptionPlans (system-wide, delete the demo plans)
                var subscriptionPlans = await _subscriptionPlanRepository.GetAllAsync("system");
                foreach (var plan in subscriptionPlans)
                {
                    await _subscriptionPlanRepository.DeleteWithoutSaveAsync(plan.Id, "system");
                }

                // Save all changes at once at the end of the transaction
                // This is compatible with NpgsqlRetryingExecutionStrategy
                await _clinicRepository.SaveChangesAsync();

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
                "529.982.247-25",
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
                "318.649.712-40",
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
                "123.891.234-65",
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
                "456.782.345-10",
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
                "789.673.456-74",
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
                "912.564.567-64",
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
                // Analgésicos / Analgesics
                new Medication("Dipirona Sódica", "500mg", "Comprimido", MedicationCategory.Analgesic, false, _demoTenantId, "Dipyrone", "Novartis", "Dipirona sódica mono-hidratada", "500mg", "Oral", false, null, null, "Analgésico e antitérmico"),
                new Medication("Dipirona Sódica", "1g", "Comprimido", MedicationCategory.Analgesic, false, _demoTenantId, "Dipyrone", "EMS", "Dipirona sódica", "1g", "Oral", false, null, null, "Analgésico forte"),
                new Medication("Dipirona Gotas", "500mg/ml", "Solução Oral", MedicationCategory.Analgesic, false, _demoTenantId, "Dipyrone", "Medley", "Dipirona sódica", "500mg/ml", "Oral", false, null, null, "Gotas para dor e febre"),
                new Medication("Paracetamol", "750mg", "Comprimido", MedicationCategory.Analgesic, false, _demoTenantId, "Acetaminophen", "Medley", "Paracetamol", "750mg", "Oral", false, null, null, "Analgésico e antitérmico"),
                new Medication("Paracetamol", "500mg", "Comprimido", MedicationCategory.Analgesic, false, _demoTenantId, "Acetaminophen", "EMS", "Paracetamol", "500mg", "Oral", false, null, null, "Analgésico"),
                new Medication("Paracetamol Gotas", "200mg/ml", "Solução Oral", MedicationCategory.Analgesic, false, _demoTenantId, "Acetaminophen", "Prati-Donaduzzi", "Paracetamol", "200mg/ml", "Oral", false, null, null, "Gotas pediátricas"),
                new Medication("Tramadol", "50mg", "Cápsula", MedicationCategory.Analgesic, true, _demoTenantId, "Tramadol", "Pfizer", "Cloridrato de tramadol", "50mg", "Oral", true, null, null, "Analgésico opioide"),
                new Medication("Tramadol", "100mg", "Comprimido", MedicationCategory.Analgesic, true, _demoTenantId, "Tramadol", "EMS", "Cloridrato de tramadol", "100mg", "Oral", true, null, null, "Analgésico opioide forte"),
                new Medication("Codeína + Paracetamol", "30mg/500mg", "Comprimido", MedicationCategory.Analgesic, true, _demoTenantId, "Codeine/Paracetamol", "Sanofi", "Fosfato de codeína + Paracetamol", "30mg/500mg", "Oral", true, null, null, "Tylex - Analgésico opioide"),
                new Medication("Morfina", "10mg", "Comprimido", MedicationCategory.Analgesic, true, _demoTenantId, "Morphine", "Cristália", "Sulfato de morfina", "10mg", "Oral", true, null, null, "Analgésico opioide forte - uso controlado"),

                // Anti-inflamatórios / Anti-inflammatory
                new Medication("Ibuprofeno", "600mg", "Comprimido", MedicationCategory.AntiInflammatory, false, _demoTenantId, "Ibuprofen", "Pfizer", "Ibuprofeno", "600mg", "Oral", false, null, null, "Anti-inflamatório não esteroidal"),
                new Medication("Ibuprofeno", "400mg", "Comprimido", MedicationCategory.AntiInflammatory, false, _demoTenantId, "Ibuprofen", "EMS", "Ibuprofeno", "400mg", "Oral", false, null, null, "AINE"),
                new Medication("Ibuprofeno Gotas", "50mg/ml", "Solução Oral", MedicationCategory.AntiInflammatory, false, _demoTenantId, "Ibuprofen", "Aché", "Ibuprofeno", "50mg/ml", "Oral", false, null, null, "Alivium infantil"),
                new Medication("Naproxeno", "500mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Naproxen", "Roche", "Naproxeno sódico", "500mg", "Oral", false, null, null, "AINE de longa duração"),
                new Medication("Nimesulida", "100mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Nimesulide", "Aché", "Nimesulida", "100mg", "Oral", false, null, null, "AINE seletivo COX-2"),
                new Medication("Diclofenaco Sódico", "50mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Diclofenac", "Novartis", "Diclofenaco sódico", "50mg", "Oral", false, null, null, "Voltaren - AINE"),
                new Medication("Diclofenaco Potássico", "50mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Diclofenac", "EMS", "Diclofenaco potássico", "50mg", "Oral", false, null, null, "Cataflam - AINE de rápida absorção"),
                new Medication("Cetoprofeno", "100mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Ketoprofen", "Sanofi", "Cetoprofeno", "100mg", "Oral", false, null, null, "Profenid - AINE"),
                new Medication("Meloxicam", "15mg", "Comprimido", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Meloxicam", "Boehringer", "Meloxicam", "15mg", "Oral", false, null, null, "AINE seletivo COX-2"),
                new Medication("Piroxicam", "20mg", "Cápsula", MedicationCategory.AntiInflammatory, true, _demoTenantId, "Piroxicam", "Pfizer", "Piroxicam", "20mg", "Oral", false, null, null, "Feldene - AINE"),

                // Antibióticos / Antibiotics
                new Medication("Amoxicilina", "500mg", "Cápsula", MedicationCategory.Antibiotic, true, _demoTenantId, "Amoxicillin", "EMS", "Amoxicilina tri-hidratada", "500mg", "Oral", false, null, null, "Antibiótico de amplo espectro"),
                new Medication("Amoxicilina", "875mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Amoxicillin", "GSK", "Amoxicilina tri-hidratada", "875mg", "Oral", false, null, null, "Antibiótico de amplo espectro - dose alta"),
                new Medication("Amoxicilina + Clavulanato", "875mg/125mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Amoxicillin/Clavulanate", "GSK", "Amoxicilina + Ácido clavulânico", "875mg/125mg", "Oral", false, null, null, "Clavulin - Antibiótico com inibidor de beta-lactamase"),
                new Medication("Azitromicina", "500mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Azithromycin", "Pfizer", "Azitromicina di-hidratada", "500mg", "Oral", false, null, null, "Zitromax - Macrolídeo"),
                new Medication("Azitromicina Suspensão", "200mg/5ml", "Pó para Suspensão", MedicationCategory.Antibiotic, true, _demoTenantId, "Azithromycin", "EMS", "Azitromicina", "200mg/5ml", "Oral", false, null, null, "Suspensão pediátrica"),
                new Medication("Ciprofloxacino", "500mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Ciprofloxacin", "Bayer", "Cloridrato de ciprofloxacino", "500mg", "Oral", false, null, null, "Cipro - Fluoroquinolona"),
                new Medication("Levofloxacino", "500mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Levofloxacin", "Sanofi", "Levofloxacino hemi-hidratado", "500mg", "Oral", false, null, null, "Tavanic - Fluoroquinolona"),
                new Medication("Cefalexina", "500mg", "Cápsula", MedicationCategory.Antibiotic, true, _demoTenantId, "Cephalexin", "EMS", "Cefalexina mono-hidratada", "500mg", "Oral", false, null, null, "Cefalosporina de 1ª geração"),
                new Medication("Ceftriaxona", "1g", "Pó para Injeção", MedicationCategory.Antibiotic, true, _demoTenantId, "Ceftriaxone", "Roche", "Ceftriaxona dissódica", "1g", "Intravenosa/Intramuscular", false, null, null, "Cefalosporina de 3ª geração"),
                new Medication("Sulfametoxazol + Trimetoprima", "400mg/80mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "SMX/TMP", "Roche", "Sulfametoxazol + Trimetoprima", "400mg/80mg", "Oral", false, null, null, "Bactrim - Sulfonamida"),
                new Medication("Metronidazol", "400mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Metronidazole", "Sanofi", "Metronidazol", "400mg", "Oral", false, null, null, "Flagyl - Nitroimidazol"),
                new Medication("Clindamicina", "300mg", "Cápsula", MedicationCategory.Antibiotic, true, _demoTenantId, "Clindamycin", "Pfizer", "Cloridrato de clindamicina", "300mg", "Oral", false, null, null, "Dalacin C - Lincosamida"),
                new Medication("Doxiciclina", "100mg", "Comprimido", MedicationCategory.Antibiotic, true, _demoTenantId, "Doxycycline", "Pfizer", "Hiclato de doxiciclina", "100mg", "Oral", false, null, null, "Vibramicina - Tetraciclina"),

                // Anti-hipertensivos / Antihypertensives
                new Medication("Losartana Potássica", "50mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Losartan", "Merck", "Losartana potássica", "50mg", "Oral", false, null, null, "BRA - Bloqueador de receptor de angiotensina"),
                new Medication("Losartana Potássica", "100mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Losartan", "EMS", "Losartana potássica", "100mg", "Oral", false, null, null, "BRA - dose alta"),
                new Medication("Enalapril", "10mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Enalapril", "Merck", "Maleato de enalapril", "10mg", "Oral", false, null, null, "IECA - Inibidor da ECA"),
                new Medication("Enalapril", "20mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Enalapril", "EMS", "Maleato de enalapril", "20mg", "Oral", false, null, null, "IECA - dose alta"),
                new Medication("Captopril", "25mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Captopril", "Bristol-Myers", "Captopril", "25mg", "Oral", false, null, null, "Capoten - IECA"),
                new Medication("Anlodipino", "5mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Amlodipine", "Pfizer", "Besilato de anlodipino", "5mg", "Oral", false, null, null, "Norvasc - Bloqueador de canal de cálcio"),
                new Medication("Anlodipino", "10mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Amlodipine", "EMS", "Besilato de anlodipino", "10mg", "Oral", false, null, null, "BCC - dose alta"),
                new Medication("Atenolol", "50mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Atenolol", "AstraZeneca", "Atenolol", "50mg", "Oral", false, null, null, "Betabloqueador seletivo"),
                new Medication("Propranolol", "40mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Propranolol", "Sanofi", "Cloridrato de propranolol", "40mg", "Oral", false, null, null, "Betabloqueador não seletivo"),
                new Medication("Carvedilol", "6,25mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Carvedilol", "Roche", "Carvedilol", "6,25mg", "Oral", false, null, null, "Betabloqueador com ação vasodilatadora"),
                new Medication("Valsartana", "80mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Valsartan", "Novartis", "Valsartana", "80mg", "Oral", false, null, null, "Diovan - BRA"),
                new Medication("Hidroclorotiazida", "25mg", "Comprimido", MedicationCategory.Antihypertensive, true, _demoTenantId, "Hydrochlorothiazide", "Merck", "Hidroclorotiazida", "25mg", "Oral", false, null, null, "Diurético tiazídico"),

                // Diuréticos / Diuretics
                new Medication("Furosemida", "40mg", "Comprimido", MedicationCategory.Diuretic, true, _demoTenantId, "Furosemide", "Sanofi", "Furosemida", "40mg", "Oral", false, null, null, "Lasix - Diurético de alça"),
                new Medication("Espironolactona", "25mg", "Comprimido", MedicationCategory.Diuretic, true, _demoTenantId, "Spironolactone", "Pfizer", "Espironolactona", "25mg", "Oral", false, null, null, "Aldactone - Diurético poupador de potássio"),
                new Medication("Indapamida", "1,5mg", "Comprimido", MedicationCategory.Diuretic, true, _demoTenantId, "Indapamide", "Servier", "Indapamida", "1,5mg", "Oral", false, null, null, "Natrilix - Diurético tiazida-like"),

                // Anti-histamínicos / Antihistamines
                new Medication("Loratadina", "10mg", "Comprimido", MedicationCategory.Antihistamine, false, _demoTenantId, "Loratadine", "Schering-Plough", "Loratadina", "10mg", "Oral", false, null, null, "Claritin - Anti-histamínico de 2ª geração"),
                new Medication("Desloratadina", "5mg", "Comprimido", MedicationCategory.Antihistamine, false, _demoTenantId, "Desloratadine", "Merck", "Desloratadina", "5mg", "Oral", false, null, null, "Desalex - Anti-histamínico de 3ª geração"),
                new Medication("Cetirizina", "10mg", "Comprimido", MedicationCategory.Antihistamine, false, _demoTenantId, "Cetirizine", "UCB", "Dicloridrato de cetirizina", "10mg", "Oral", false, null, null, "Zyrtec - Anti-histamínico"),
                new Medication("Fexofenadina", "180mg", "Comprimido", MedicationCategory.Antihistamine, true, _demoTenantId, "Fexofenadine", "Sanofi", "Cloridrato de fexofenadina", "180mg", "Oral", false, null, null, "Allegra - Anti-histamínico"),
                new Medication("Hidroxizina", "25mg", "Comprimido", MedicationCategory.Antihistamine, true, _demoTenantId, "Hydroxyzine", "UCB", "Dicloridrato de hidroxizina", "25mg", "Oral", false, null, null, "Hixizine - Anti-histamínico com efeito sedativo"),
                new Medication("Prometazina", "25mg", "Comprimido", MedicationCategory.Antihistamine, true, _demoTenantId, "Promethazine", "Sanofi", "Cloridrato de prometazina", "25mg", "Oral", false, null, null, "Fenergan - Anti-histamínico sedativo"),

                // Antiácidos e protetores gástricos / Antacids
                new Medication("Omeprazol", "20mg", "Cápsula", MedicationCategory.Antacid, true, _demoTenantId, "Omeprazole", "AstraZeneca", "Omeprazol", "20mg", "Oral", false, null, null, "Losec - IBP"),
                new Medication("Omeprazol", "40mg", "Cápsula", MedicationCategory.Antacid, true, _demoTenantId, "Omeprazole", "EMS", "Omeprazol", "40mg", "Oral", false, null, null, "IBP - dose alta"),
                new Medication("Pantoprazol", "40mg", "Comprimido", MedicationCategory.Antacid, true, _demoTenantId, "Pantoprazole", "Takeda", "Pantoprazol sódico sesqui-hidratado", "40mg", "Oral", false, null, null, "Pantozol - IBP"),
                new Medication("Esomeprazol", "40mg", "Comprimido", MedicationCategory.Antacid, true, _demoTenantId, "Esomeprazole", "AstraZeneca", "Esomeprazol magnésio tri-hidratado", "40mg", "Oral", false, null, null, "Nexium - IBP"),
                new Medication("Ranitidina", "150mg", "Comprimido", MedicationCategory.Antacid, true, _demoTenantId, "Ranitidine", "GSK", "Cloridrato de ranitidina", "150mg", "Oral", false, null, null, "Antak - Antagonista H2"),
                new Medication("Hidróxido de Alumínio", "61,5mg/ml", "Suspensão Oral", MedicationCategory.Antacid, false, _demoTenantId, "Aluminum Hydroxide", "Sanofi", "Hidróxido de alumínio", "61,5mg/ml", "Oral", false, null, null, "Pepsamar - Antiácido"),

                // Antidiabéticos / Antidiabetics
                new Medication("Metformina", "850mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Metformin", "Bristol-Myers", "Cloridrato de metformina", "850mg", "Oral", false, null, null, "Glifage - Biguanida"),
                new Medication("Metformina", "500mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Metformin", "EMS", "Cloridrato de metformina", "500mg", "Oral", false, null, null, "Biguanida - dose inicial"),
                new Medication("Metformina XR", "500mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Metformin XR", "Merck", "Cloridrato de metformina", "500mg", "Oral", false, null, null, "Glifage XR - Liberação prolongada"),
                new Medication("Glibenclamida", "5mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Glibenclamide", "Sanofi", "Glibenclamida", "5mg", "Oral", false, null, null, "Daonil - Sulfonilureia"),
                new Medication("Glimepirida", "2mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Glimepiride", "Sanofi", "Glimepirida", "2mg", "Oral", false, null, null, "Amaryl - Sulfonilureia"),
                new Medication("Sitagliptina", "100mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Sitagliptin", "Merck", "Fosfato de sitagliptina", "100mg", "Oral", false, null, null, "Januvia - Inibidor de DPP-4"),
                new Medication("Empagliflozina", "25mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Empagliflozin", "Boehringer", "Empagliflozina", "25mg", "Oral", false, null, null, "Jardiance - Inibidor de SGLT2"),
                new Medication("Dapagliflozina", "10mg", "Comprimido", MedicationCategory.Antidiabetic, true, _demoTenantId, "Dapagliflozin", "AstraZeneca", "Dapagliflozina", "10mg", "Oral", false, null, null, "Forxiga - Inibidor de SGLT2"),
                new Medication("Insulina NPH", "100 UI/ml", "Suspensão Injetável", MedicationCategory.Antidiabetic, true, _demoTenantId, "Insulin NPH", "Novo Nordisk", "Insulina humana isofana", "100 UI/ml", "Subcutânea", false, null, null, "Insulina de ação intermediária"),
                new Medication("Insulina Regular", "100 UI/ml", "Solução Injetável", MedicationCategory.Antidiabetic, true, _demoTenantId, "Regular Insulin", "Eli Lilly", "Insulina humana regular", "100 UI/ml", "Subcutânea/Intravenosa", false, null, null, "Insulina de ação rápida"),

                // Ansiolíticos / Anxiolytics
                new Medication("Clonazepam", "2mg", "Comprimido", MedicationCategory.Anxiolytic, true, _demoTenantId, "Clonazepam", "Roche", "Clonazepam", "2mg", "Oral", true, null, null, "Rivotril - Benzodiazepínico"),
                new Medication("Clonazepam Gotas", "2,5mg/ml", "Solução Oral", MedicationCategory.Anxiolytic, true, _demoTenantId, "Clonazepam", "Roche", "Clonazepam", "2,5mg/ml", "Oral", true, null, null, "Rivotril Gotas"),
                new Medication("Alprazolam", "0,5mg", "Comprimido", MedicationCategory.Anxiolytic, true, _demoTenantId, "Alprazolam", "Pfizer", "Alprazolam", "0,5mg", "Oral", true, null, null, "Frontal - Benzodiazepínico"),
                new Medication("Diazepam", "10mg", "Comprimido", MedicationCategory.Anxiolytic, true, _demoTenantId, "Diazepam", "Roche", "Diazepam", "10mg", "Oral", true, null, null, "Valium - Benzodiazepínico"),
                new Medication("Bromazepam", "3mg", "Comprimido", MedicationCategory.Anxiolytic, true, _demoTenantId, "Bromazepam", "Roche", "Bromazepam", "3mg", "Oral", true, null, null, "Lexotan - Benzodiazepínico"),
                new Medication("Lorazepam", "2mg", "Comprimido", MedicationCategory.Anxiolytic, true, _demoTenantId, "Lorazepam", "Pfizer", "Lorazepam", "2mg", "Oral", true, null, null, "Lorax - Benzodiazepínico"),

                // Antidepressivos / Antidepressants
                new Medication("Sertralina", "50mg", "Comprimido", MedicationCategory.Antidepressant, true, _demoTenantId, "Sertraline", "Pfizer", "Cloridrato de sertralina", "50mg", "Oral", false, null, null, "Zoloft - ISRS"),
                new Medication("Escitalopram", "10mg", "Comprimido", MedicationCategory.Antidepressant, true, _demoTenantId, "Escitalopram", "Lundbeck", "Oxalato de escitalopram", "10mg", "Oral", false, null, null, "Lexapro - ISRS"),
                new Medication("Fluoxetina", "20mg", "Cápsula", MedicationCategory.Antidepressant, true, _demoTenantId, "Fluoxetine", "Eli Lilly", "Cloridrato de fluoxetina", "20mg", "Oral", false, null, null, "Prozac - ISRS"),
                new Medication("Paroxetina", "20mg", "Comprimido", MedicationCategory.Antidepressant, true, _demoTenantId, "Paroxetine", "GSK", "Cloridrato de paroxetina", "20mg", "Oral", false, null, null, "Paxil - ISRS"),
                new Medication("Venlafaxina", "75mg", "Cápsula", MedicationCategory.Antidepressant, true, _demoTenantId, "Venlafaxine", "Pfizer", "Cloridrato de venlafaxina", "75mg", "Oral", false, null, null, "Effexor - ISRSN"),
                new Medication("Duloxetina", "60mg", "Cápsula", MedicationCategory.Antidepressant, true, _demoTenantId, "Duloxetine", "Eli Lilly", "Cloridrato de duloxetina", "60mg", "Oral", false, null, null, "Cymbalta - ISRSN"),
                new Medication("Amitriptilina", "25mg", "Comprimido", MedicationCategory.Antidepressant, true, _demoTenantId, "Amitriptyline", "Eurofarma", "Cloridrato de amitriptilina", "25mg", "Oral", false, null, null, "Tryptanol - Antidepressivo tricíclico"),
                new Medication("Bupropiona", "150mg", "Comprimido", MedicationCategory.Antidepressant, true, _demoTenantId, "Bupropion", "GSK", "Cloridrato de bupropiona", "150mg", "Oral", false, null, null, "Wellbutrin - Inibidor de recaptação de norepinefrina-dopamina"),

                // Anticoagulantes / Anticoagulants
                new Medication("Varfarina", "5mg", "Comprimido", MedicationCategory.Anticoagulant, true, _demoTenantId, "Warfarin", "Bristol-Myers", "Varfarina sódica", "5mg", "Oral", false, null, null, "Marevan - Anticoagulante cumarínico"),
                new Medication("Rivaroxabana", "20mg", "Comprimido", MedicationCategory.Anticoagulant, true, _demoTenantId, "Rivaroxaban", "Bayer", "Rivaroxabana", "20mg", "Oral", false, null, null, "Xarelto - NOAC"),
                new Medication("Apixabana", "5mg", "Comprimido", MedicationCategory.Anticoagulant, true, _demoTenantId, "Apixaban", "Bristol-Myers", "Apixabana", "5mg", "Oral", false, null, null, "Eliquis - NOAC"),
                new Medication("AAS", "100mg", "Comprimido", MedicationCategory.Anticoagulant, false, _demoTenantId, "Aspirin", "Bayer", "Ácido acetilsalicílico", "100mg", "Oral", false, null, null, "Aspirina Prevent - Antiagregante plaquetário"),
                new Medication("Clopidogrel", "75mg", "Comprimido", MedicationCategory.Anticoagulant, true, _demoTenantId, "Clopidogrel", "Sanofi", "Bissulfato de clopidogrel", "75mg", "Oral", false, null, null, "Plavix - Antiagregante plaquetário"),
                new Medication("Enoxaparina", "40mg", "Solução Injetável", MedicationCategory.Anticoagulant, true, _demoTenantId, "Enoxaparin", "Sanofi", "Enoxaparina sódica", "40mg", "Subcutânea", false, null, null, "Clexane - Heparina de baixo peso molecular"),

                // Corticosteroides / Corticosteroids
                new Medication("Prednisona", "20mg", "Comprimido", MedicationCategory.Corticosteroid, true, _demoTenantId, "Prednisone", "Pfizer", "Prednisona", "20mg", "Oral", false, null, null, "Meticorten - Corticosteroide sistêmico"),
                new Medication("Prednisona", "5mg", "Comprimido", MedicationCategory.Corticosteroid, true, _demoTenantId, "Prednisone", "EMS", "Prednisona", "5mg", "Oral", false, null, null, "Corticosteroide - dose baixa"),
                new Medication("Dexametasona", "4mg", "Comprimido", MedicationCategory.Corticosteroid, true, _demoTenantId, "Dexamethasone", "Aché", "Dexametasona", "4mg", "Oral", false, null, null, "Decadron - Corticosteroide potente"),
                new Medication("Betametasona", "0,5mg", "Comprimido", MedicationCategory.Corticosteroid, true, _demoTenantId, "Betamethasone", "Schering-Plough", "Betametasona", "0,5mg", "Oral", false, null, null, "Celestone - Corticosteroide"),
                new Medication("Hidrocortisona Creme", "1%", "Creme", MedicationCategory.Corticosteroid, false, _demoTenantId, "Hydrocortisone", "Sanofi", "Acetato de hidrocortisona", "1%", "Tópica", false, null, null, "Corticosteroide tópico leve"),

                // Broncodilatadores / Bronchodilators
                new Medication("Salbutamol", "100mcg/dose", "Aerossol", MedicationCategory.Bronchodilator, true, _demoTenantId, "Salbutamol", "GSK", "Sulfato de salbutamol", "100mcg/dose", "Inalatória", false, null, null, "Aerolin - Beta-2 agonista de curta duração"),
                new Medication("Fenoterol", "100mcg/dose", "Aerossol", MedicationCategory.Bronchodilator, true, _demoTenantId, "Fenoterol", "Boehringer", "Bromidrato de fenoterol", "100mcg/dose", "Inalatória", false, null, null, "Berotec - Beta-2 agonista"),
                new Medication("Formoterol + Budesonida", "6mcg/200mcg", "Cápsula Inalatória", MedicationCategory.Bronchodilator, true, _demoTenantId, "Formoterol/Budesonide", "AstraZeneca", "Fumarato de formoterol + Budesonida", "6mcg/200mcg", "Inalatória", false, null, null, "Symbicort - Beta-2 agonista de longa duração + CI"),
                new Medication("Salmeterol + Fluticasona", "25mcg/250mcg", "Aerossol", MedicationCategory.Bronchodilator, true, _demoTenantId, "Salmeterol/Fluticasone", "GSK", "Xinafoato de salmeterol + Propionato de fluticasona", "25mcg/250mcg", "Inalatória", false, null, null, "Seretide - LABA + CI"),
                new Medication("Brometo de Ipratrópio", "20mcg/dose", "Aerossol", MedicationCategory.Bronchodilator, true, _demoTenantId, "Ipratropium", "Boehringer", "Brometo de ipratrópio", "20mcg/dose", "Inalatória", false, null, null, "Atrovent - Anticolinérgico"),

                // Vitaminas e Suplementos / Vitamins and Supplements
                new Medication("Vitamina D3", "7000 UI", "Cápsula", MedicationCategory.Vitamin, false, _demoTenantId, "Cholecalciferol", "Sanofi", "Colecalciferol", "7000 UI", "Oral", false, null, null, "Suplemento vitamínico"),
                new Medication("Vitamina D3", "50000 UI", "Cápsula", MedicationCategory.Vitamin, true, _demoTenantId, "Cholecalciferol", "Aché", "Colecalciferol", "50000 UI", "Oral", false, null, null, "Reposição de vitamina D"),
                new Medication("Vitamina B12", "1000mcg", "Comprimido", MedicationCategory.Vitamin, false, _demoTenantId, "Cyanocobalamin", "Merck", "Cianocobalamina", "1000mcg", "Oral", false, null, null, "Citoneurin - Vitamina B12"),
                new Medication("Complexo B", "variado", "Comprimido", MedicationCategory.Vitamin, false, _demoTenantId, "B Complex", "Pfizer", "Vitaminas do complexo B", "variado", "Oral", false, null, null, "Centrum - Polivitamínico"),
                new Medication("Ácido Fólico", "5mg", "Comprimido", MedicationCategory.Vitamin, false, _demoTenantId, "Folic Acid", "EMS", "Ácido fólico", "5mg", "Oral", false, null, null, "Suplemento para gestantes"),
                new Medication("Sulfato Ferroso", "40mg", "Comprimido", MedicationCategory.Supplement, true, _demoTenantId, "Ferrous Sulfate", "EMS", "Sulfato ferroso", "40mg", "Oral", false, null, null, "Suplemento de ferro"),
                new Medication("Cálcio + Vitamina D", "500mg/200UI", "Comprimido", MedicationCategory.Supplement, false, _demoTenantId, "Calcium/Vitamin D", "Sanofi", "Carbonato de cálcio + Colecalciferol", "500mg/200UI", "Oral", false, null, null, "Os-Cal - Suplemento ósseo"),
                new Medication("Ômega 3", "1000mg", "Cápsula", MedicationCategory.Supplement, false, _demoTenantId, "Omega-3", "Naturalis", "Óleo de peixe", "1000mg", "Oral", false, null, null, "Suplemento cardiovascular"),

                // Antifúngicos / Antifungals
                new Medication("Fluconazol", "150mg", "Cápsula", MedicationCategory.Antifungal, true, _demoTenantId, "Fluconazole", "Pfizer", "Fluconazol", "150mg", "Oral", false, null, null, "Zoltec - Antifúngico sistêmico"),
                new Medication("Itraconazol", "100mg", "Cápsula", MedicationCategory.Antifungal, true, _demoTenantId, "Itraconazole", "Janssen", "Itraconazol", "100mg", "Oral", false, null, null, "Sporanox - Antifúngico de amplo espectro"),
                new Medication("Cetoconazol", "200mg", "Comprimido", MedicationCategory.Antifungal, true, _demoTenantId, "Ketoconazole", "Janssen", "Cetoconazol", "200mg", "Oral", false, null, null, "Nizoral - Antifúngico"),
                new Medication("Nistatina Suspensão", "100000 UI/ml", "Suspensão Oral", MedicationCategory.Antifungal, true, _demoTenantId, "Nystatin", "Bristol-Myers", "Nistatina", "100000 UI/ml", "Oral", false, null, null, "Antifúngico para candidíase oral"),
                new Medication("Terbinafina", "250mg", "Comprimido", MedicationCategory.Antifungal, true, _demoTenantId, "Terbinafine", "Novartis", "Cloridrato de terbinafina", "250mg", "Oral", false, null, null, "Lamisil - Antifúngico"),

                // Antivirais / Antivirals
                new Medication("Aciclovir", "400mg", "Comprimido", MedicationCategory.Antiviral, true, _demoTenantId, "Acyclovir", "GSK", "Aciclovir", "400mg", "Oral", false, null, null, "Zovirax - Antiviral herpes"),
                new Medication("Valaciclovir", "500mg", "Comprimido", MedicationCategory.Antiviral, true, _demoTenantId, "Valacyclovir", "GSK", "Cloridrato de valaciclovir", "500mg", "Oral", false, null, null, "Valtrex - Pró-droga do aciclovir"),
                new Medication("Oseltamivir", "75mg", "Cápsula", MedicationCategory.Antiviral, true, _demoTenantId, "Oseltamivir", "Roche", "Fosfato de oseltamivir", "75mg", "Oral", false, null, null, "Tamiflu - Antiviral influenza"),

                // Antiparasitários / Antiparasitics
                new Medication("Albendazol", "400mg", "Comprimido Mastigável", MedicationCategory.Antiparasitic, true, _demoTenantId, "Albendazole", "GSK", "Albendazol", "400mg", "Oral", false, null, null, "Zentel - Antiparasitário"),
                new Medication("Ivermectina", "6mg", "Comprimido", MedicationCategory.Antiparasitic, true, _demoTenantId, "Ivermectin", "Merck", "Ivermectina", "6mg", "Oral", false, null, null, "Revectina - Antiparasitário"),
                new Medication("Mebendazol", "100mg", "Comprimido", MedicationCategory.Antiparasitic, true, _demoTenantId, "Mebendazole", "Janssen", "Mebendazol", "100mg", "Oral", false, null, null, "Pantelmin - Antiparasitário"),
                new Medication("Nitazoxanida", "500mg", "Comprimido", MedicationCategory.Antiparasitic, true, _demoTenantId, "Nitazoxanide", "Romark", "Nitazoxanida", "500mg", "Oral", false, null, null, "Annita - Antiparasitário e antiviral"),
                new Medication("Secnidazol", "1g", "Comprimido", MedicationCategory.Antiparasitic, true, _demoTenantId, "Secnidazole", "Sanofi", "Secnidazol", "1g", "Oral", false, null, null, "Antifúngico e antiprotozoário"),

                // Outros / Others
                new Medication("Domperidona", "10mg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Domperidone", "Janssen", "Domperidona", "10mg", "Oral", false, null, null, "Motilium - Antiemético"),
                new Medication("Metoclopramida", "10mg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Metoclopramide", "Sanofi", "Cloridrato de metoclopramida", "10mg", "Oral", false, null, null, "Plasil - Antiemético"),
                new Medication("Ondansetrona", "4mg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Ondansetron", "GSK", "Cloridrato de ondansetrona", "4mg", "Oral", false, null, null, "Zofran - Antiemético para quimioterapia"),
                new Medication("Simeticona", "125mg", "Comprimido", MedicationCategory.Other, false, _demoTenantId, "Simethicone", "Pfizer", "Simeticona", "125mg", "Oral", false, null, null, "Luftal - Antiflatulento"),
                new Medication("Loperamida", "2mg", "Cápsula", MedicationCategory.Other, false, _demoTenantId, "Loperamide", "Janssen", "Cloridrato de loperamida", "2mg", "Oral", false, null, null, "Imosec - Antidiarreico"),
                new Medication("Lactulose", "667mg/ml", "Xarope", MedicationCategory.Other, false, _demoTenantId, "Lactulose", "Solvay", "Lactulose", "667mg/ml", "Oral", false, null, null, "Lactulona - Laxante osmótico"),
                new Medication("Bisacodil", "5mg", "Comprimido", MedicationCategory.Other, false, _demoTenantId, "Bisacodyl", "Boehringer", "Bisacodil", "5mg", "Oral", false, null, null, "Dulcolax - Laxante estimulante"),
                new Medication("Sildenafila", "50mg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Sildenafil", "Pfizer", "Citrato de sildenafila", "50mg", "Oral", false, null, null, "Viagra - Tratamento de disfunção erétil"),
                new Medication("Finasterida", "1mg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Finasteride", "Merck", "Finasterida", "1mg", "Oral", false, null, null, "Propecia - Tratamento de calvície"),
                new Medication("Levotiroxina", "50mcg", "Comprimido", MedicationCategory.Other, true, _demoTenantId, "Levothyroxine", "Merck", "Levotiroxina sódica", "50mcg", "Oral", false, null, null, "Synthroid - Hormônio tireoidiano"),
                new Medication("Melatonina", "3mg", "Comprimido", MedicationCategory.Other, false, _demoTenantId, "Melatonin", "Medley", "Melatonina", "3mg", "Oral", false, null, null, "Regulador do sono")
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

        private List<ExamCatalog> CreateDemoExamCatalogs()
        {
            return new List<ExamCatalog>
            {
                // Exames Laboratoriais / Laboratory Tests
                new ExamCatalog("Hemograma Completo", ExamType.Laboratory, _demoTenantId, "Avaliação das células sanguíneas (glóbulos vermelhos, brancos e plaquetas)", "Hematologia", "Jejum de 4 horas", "hemograma,sangue completo,células sanguíneas", "40304361"),
                new ExamCatalog("Hemoglobina Glicada (HbA1c)", ExamType.Laboratory, _demoTenantId, "Avaliação do controle glicêmico dos últimos 3 meses", "Bioquímica", "Não necessita jejum", "hba1c,glicada,diabetes controle", "40302415"),
                new ExamCatalog("Glicemia de Jejum", ExamType.Laboratory, _demoTenantId, "Dosagem de glicose no sangue em jejum", "Bioquímica", "Jejum de 8 a 12 horas", "glicose,açúcar,diabetes", "40302040"),
                new ExamCatalog("Curva Glicêmica (TOTG)", ExamType.Laboratory, _demoTenantId, "Teste de tolerância oral à glicose", "Bioquímica", "Jejum de 8 a 12 horas", "curva de glicose,totg,diabetes gestacional", "40302180"),
                new ExamCatalog("Colesterol Total", ExamType.Laboratory, _demoTenantId, "Dosagem de colesterol total no sangue", "Bioquímica", "Jejum de 12 horas", "colesterol,lipídeos", "40301575"),
                new ExamCatalog("HDL Colesterol", ExamType.Laboratory, _demoTenantId, "Dosagem de colesterol HDL (bom colesterol)", "Bioquímica", "Jejum de 12 horas", "hdl,colesterol bom,lipídeos", "40301583"),
                new ExamCatalog("LDL Colesterol", ExamType.Laboratory, _demoTenantId, "Dosagem de colesterol LDL (mau colesterol)", "Bioquímica", "Jejum de 12 horas", "ldl,colesterol ruim,lipídeos", "40301591"),
                new ExamCatalog("Triglicerídeos", ExamType.Laboratory, _demoTenantId, "Dosagem de triglicerídeos no sangue", "Bioquímica", "Jejum de 12 horas", "triglicerides,gordura,lipídeos", "40302610"),
                new ExamCatalog("Perfil Lipídico Completo", ExamType.Laboratory, _demoTenantId, "Colesterol total, HDL, LDL e triglicerídeos", "Bioquímica", "Jejum de 12 horas", "lipidograma,colesterol completo", "40302636"),
                new ExamCatalog("Ureia", ExamType.Laboratory, _demoTenantId, "Avaliação da função renal", "Bioquímica", "Não necessita jejum", "ureia,função renal,rim", "40302652"),
                new ExamCatalog("Creatinina", ExamType.Laboratory, _demoTenantId, "Avaliação da função renal", "Bioquímica", "Não necessita jejum", "creatinina,função renal,rim", "40301613"),
                new ExamCatalog("Ácido Úrico", ExamType.Laboratory, _demoTenantId, "Dosagem de ácido úrico no sangue", "Bioquímica", "Jejum de 4 horas", "ácido úrico,gota,urato", "40301508"),
                new ExamCatalog("TGO (AST)", ExamType.Laboratory, _demoTenantId, "Enzima hepática - Aspartato aminotransferase", "Bioquímica", "Não necessita jejum", "tgo,ast,fígado,transaminase", "40302571"),
                new ExamCatalog("TGP (ALT)", ExamType.Laboratory, _demoTenantId, "Enzima hepática - Alanina aminotransferase", "Bioquímica", "Não necessita jejum", "tgp,alt,fígado,transaminase", "40302580"),
                new ExamCatalog("Gama GT", ExamType.Laboratory, _demoTenantId, "Enzima hepática - Gama glutamil transferase", "Bioquímica", "Jejum de 4 horas", "ggt,gama gt,fígado", "40302032"),
                new ExamCatalog("Fosfatase Alcalina", ExamType.Laboratory, _demoTenantId, "Enzima hepática e óssea", "Bioquímica", "Jejum de 4 horas", "fosfatase,fígado,osso", "40301966"),
                new ExamCatalog("Bilirrubinas (Total e Frações)", ExamType.Laboratory, _demoTenantId, "Bilirrubina total, direta e indireta", "Bioquímica", "Jejum de 4 horas", "bilirrubina,icterícia,fígado", "40301540"),
                new ExamCatalog("Proteínas Totais e Frações", ExamType.Laboratory, _demoTenantId, "Dosagem de albumina e globulina", "Bioquímica", "Jejum de 4 horas", "proteínas,albumina,globulina", "40302504"),
                new ExamCatalog("Sódio", ExamType.Laboratory, _demoTenantId, "Dosagem de sódio sérico", "Bioquímica", "Não necessita jejum", "sódio,na,eletrólito", "40302555"),
                new ExamCatalog("Potássio", ExamType.Laboratory, _demoTenantId, "Dosagem de potássio sérico", "Bioquímica", "Não necessita jejum", "potássio,k,eletrólito", "40302482"),
                new ExamCatalog("Cálcio", ExamType.Laboratory, _demoTenantId, "Dosagem de cálcio sérico", "Bioquímica", "Jejum de 4 horas", "cálcio,ca,osso", "40301559"),
                new ExamCatalog("Magnésio", ExamType.Laboratory, _demoTenantId, "Dosagem de magnésio sérico", "Bioquímica", "Jejum de 4 horas", "magnésio,mg,eletrólito", "40302334"),
                new ExamCatalog("Ferro Sérico", ExamType.Laboratory, _demoTenantId, "Dosagem de ferro no sangue", "Bioquímica", "Jejum de 8 horas", "ferro,anemia,ferritina", "40301958"),
                new ExamCatalog("Ferritina", ExamType.Laboratory, _demoTenantId, "Reserva de ferro no organismo", "Bioquímica", "Jejum de 4 horas", "ferritina,ferro,anemia", "40301940"),
                new ExamCatalog("Transferrina", ExamType.Laboratory, _demoTenantId, "Proteína transportadora de ferro", "Bioquímica", "Jejum de 4 horas", "transferrina,ferro,anemia", "40302601"),
                new ExamCatalog("Vitamina B12", ExamType.Laboratory, _demoTenantId, "Dosagem de vitamina B12", "Bioquímica", "Jejum de 4 horas", "b12,cobalamina,anemia", "40302679"),
                new ExamCatalog("Ácido Fólico", ExamType.Laboratory, _demoTenantId, "Dosagem de ácido fólico", "Bioquímica", "Jejum de 4 horas", "folato,ácido fólico,anemia", "40301516"),
                new ExamCatalog("Vitamina D (25-OH)", ExamType.Laboratory, _demoTenantId, "Dosagem de vitamina D", "Bioquímica", "Não necessita jejum", "vitamina d,calcifediol,25 hidroxi", "40302687"),
                new ExamCatalog("TSH", ExamType.Laboratory, _demoTenantId, "Hormônio estimulante da tireoide", "Hormônios", "Não necessita jejum", "tsh,tireoide,hormônio", "40316521"),
                new ExamCatalog("T4 Livre", ExamType.Laboratory, _demoTenantId, "Tiroxina livre", "Hormônios", "Não necessita jejum", "t4,tiroxina,tireoide", "40316440"),
                new ExamCatalog("T3", ExamType.Laboratory, _demoTenantId, "Triiodotironina", "Hormônios", "Não necessita jejum", "t3,tireoide,hormônio", "40316424"),
                new ExamCatalog("Anti-TPO", ExamType.Laboratory, _demoTenantId, "Anticorpos antitireoperoxidase", "Imunologia", "Não necessita jejum", "anti tpo,tireoide,autoimune,hashimoto", "40306623"),
                new ExamCatalog("PSA Total", ExamType.Laboratory, _demoTenantId, "Antígeno prostático específico", "Marcadores Tumorais", "Não necessita jejum, evitar atividade sexual 48h antes", "psa,próstata,câncer", "40316289"),
                new ExamCatalog("PSA Livre", ExamType.Laboratory, _demoTenantId, "PSA livre e relação PSA livre/total", "Marcadores Tumorais", "Não necessita jejum", "psa livre,próstata", "40316297"),
                new ExamCatalog("CEA", ExamType.Laboratory, _demoTenantId, "Antígeno carcinoembrionário", "Marcadores Tumorais", "Não necessita jejum", "cea,câncer,marcador tumoral", "40316050"),
                new ExamCatalog("CA 125", ExamType.Laboratory, _demoTenantId, "Marcador tumoral ovariano", "Marcadores Tumorais", "Não necessita jejum", "ca125,ovário,câncer", "40316033"),
                new ExamCatalog("CA 19-9", ExamType.Laboratory, _demoTenantId, "Marcador tumoral pancreático", "Marcadores Tumorais", "Não necessita jejum", "ca199,pâncreas,câncer", "40316017"),
                new ExamCatalog("AFP (Alfa-fetoproteína)", ExamType.Laboratory, _demoTenantId, "Marcador tumoral hepático", "Marcadores Tumorais", "Não necessita jejum", "afp,fígado,câncer,alfa fetoproteína", "40316009"),
                new ExamCatalog("Tempo de Protrombina (TP/INR)", ExamType.Laboratory, _demoTenantId, "Avaliação da coagulação sanguínea", "Coagulação", "Não necessita jejum", "tp,inr,coagulação,varfarina", "40304388"),
                new ExamCatalog("Tempo de Tromboplastina Parcial (TTPA)", ExamType.Laboratory, _demoTenantId, "Avaliação da coagulação sanguínea", "Coagulação", "Não necessita jejum", "ttpa,ptt,coagulação", "40304396"),
                new ExamCatalog("Fibrinogênio", ExamType.Laboratory, _demoTenantId, "Fator de coagulação", "Coagulação", "Não necessita jejum", "fibrinogênio,coagulação", "40304205"),
                new ExamCatalog("D-Dímero", ExamType.Laboratory, _demoTenantId, "Marcador de trombose", "Coagulação", "Não necessita jejum", "d-dímero,trombose,embolia", "40304191"),
                new ExamCatalog("VHS (Velocidade de Hemossedimentação)", ExamType.Laboratory, _demoTenantId, "Marcador de inflamação", "Hematologia", "Não necessita jejum", "vhs,inflamação,hemossedimentação", "40304418"),
                new ExamCatalog("PCR (Proteína C Reativa)", ExamType.Laboratory, _demoTenantId, "Marcador de inflamação e infecção", "Bioquímica", "Não necessita jejum", "pcr,proteína c reativa,inflamação", "40302474"),
                new ExamCatalog("PCR Ultrassensível", ExamType.Laboratory, _demoTenantId, "Marcador de risco cardiovascular", "Bioquímica", "Não necessita jejum", "pcr us,cardiovascular,inflamação", "40302466"),
                new ExamCatalog("Homocisteína", ExamType.Laboratory, _demoTenantId, "Fator de risco cardiovascular", "Bioquímica", "Jejum de 12 horas", "homocisteína,cardiovascular", "40302270"),
                new ExamCatalog("Troponina", ExamType.Laboratory, _demoTenantId, "Marcador de lesão cardíaca", "Bioquímica", "Não necessita jejum", "troponina,infarto,coração", "40302628"),
                new ExamCatalog("CK Total", ExamType.Laboratory, _demoTenantId, "Creatinofosfoquinase total", "Bioquímica", "Não necessita jejum", "ck,cpk,músculo,coração", "40301605"),
                new ExamCatalog("CK-MB", ExamType.Laboratory, _demoTenantId, "Fração cardíaca da CK", "Bioquímica", "Não necessita jejum", "ckmb,coração,infarto", "40301621"),
                new ExamCatalog("LDH", ExamType.Laboratory, _demoTenantId, "Lactato desidrogenase", "Bioquímica", "Não necessita jejum", "ldh,lactato,desidrogenase", "40302318"),
                new ExamCatalog("Amilase", ExamType.Laboratory, _demoTenantId, "Enzima pancreática", "Bioquímica", "Não necessita jejum", "amilase,pâncreas,pancreatite", "40301532"),
                new ExamCatalog("Lipase", ExamType.Laboratory, _demoTenantId, "Enzima pancreática", "Bioquímica", "Jejum de 4 horas", "lipase,pâncreas,pancreatite", "40302326"),
                new ExamCatalog("EAS (Urina Tipo I)", ExamType.Laboratory, _demoTenantId, "Exame de urina simples", "Urinálise", "Primeira urina da manhã ou 4 horas sem urinar", "eas,urina,urina tipo 1,sumário de urina", "40311074"),
                new ExamCatalog("Urocultura", ExamType.Laboratory, _demoTenantId, "Cultura de urina com antibiograma", "Microbiologia", "Primeira urina da manhã, jato médio", "urocultura,infecção urinária,bacteriúria", "40310213"),
                new ExamCatalog("Clearance de Creatinina", ExamType.Laboratory, _demoTenantId, "Taxa de filtração glomerular", "Bioquímica", "Coleta de urina de 24 horas", "clearance,creatinina,função renal", "40311040"),
                new ExamCatalog("Proteinúria de 24 Horas", ExamType.Laboratory, _demoTenantId, "Dosagem de proteínas na urina", "Bioquímica", "Coleta de urina de 24 horas", "proteinúria,proteínas na urina", "40311139"),
                new ExamCatalog("Microalbuminúria", ExamType.Laboratory, _demoTenantId, "Detecção precoce de lesão renal", "Bioquímica", "Primeira urina da manhã", "microalbuminúria,nefropatia diabética", "40311112"),
                new ExamCatalog("Beta-HCG Quantitativo", ExamType.Laboratory, _demoTenantId, "Teste de gravidez quantitativo", "Hormônios", "Não necessita jejum", "bhcg,gravidez,hcg", "40316122"),
                new ExamCatalog("Espermogrma", ExamType.Laboratory, _demoTenantId, "Análise do sêmen", "Fertilidade", "Abstinência sexual de 3 a 5 dias", "espermograma,sêmen,fertilidade,espermatozoides", "40311023"),
                new ExamCatalog("Parasitológico de Fezes", ExamType.Laboratory, _demoTenantId, "Pesquisa de parasitas nas fezes", "Parasitologia", "Coletar em dias alternados (3 amostras)", "epf,parasitas,vermes,fezes", "40308251"),
                new ExamCatalog("Sangue Oculto nas Fezes", ExamType.Laboratory, _demoTenantId, "Pesquisa de sangue nas fezes", "Bioquímica", "Evitar carne vermelha 3 dias antes", "sangue oculto,hemorragia,câncer colorretal", "40308219"),
                new ExamCatalog("Coprocultura", ExamType.Laboratory, _demoTenantId, "Cultura de fezes", "Microbiologia", "Coletar em frasco estéril", "coprocultura,fezes,bactérias,diarreia", "40310060"),
                new ExamCatalog("Hemocultura", ExamType.Laboratory, _demoTenantId, "Cultura de sangue", "Microbiologia", "Coleta antes do início de antibióticos", "hemocultura,sepse,bacteremia", "40310108"),
                new ExamCatalog("Cultura de Secreção", ExamType.Laboratory, _demoTenantId, "Cultura de material biológico", "Microbiologia", "Coletar antes do início de antibióticos", "cultura,secreção,pus,infecção", "40310060"),
                new ExamCatalog("Sorologias para Hepatite B (HBsAg, Anti-HBs, Anti-HBc)", ExamType.Laboratory, _demoTenantId, "Painel de hepatite B", "Imunologia", "Não necessita jejum", "hepatite b,hbsag,anti hbs,anti hbc", "40307603"),
                new ExamCatalog("Anti-HCV", ExamType.Laboratory, _demoTenantId, "Sorologia para hepatite C", "Imunologia", "Não necessita jejum", "hepatite c,anti hcv,sorologia", "40307573"),
                new ExamCatalog("Anti-HIV", ExamType.Laboratory, _demoTenantId, "Sorologia para HIV", "Imunologia", "Não necessita jejum", "hiv,aids,anti hiv,sorologia", "40307620"),
                new ExamCatalog("VDRL", ExamType.Laboratory, _demoTenantId, "Sorologia para sífilis", "Imunologia", "Não necessita jejum", "vdrl,sífilis,sorologia", "40307760"),
                new ExamCatalog("FTA-ABS", ExamType.Laboratory, _demoTenantId, "Sorologia confirmatória para sífilis", "Imunologia", "Não necessita jejum", "fta-abs,sífilis,confirmatório", "40307484"),
                new ExamCatalog("Toxoplasmose (IgG e IgM)", ExamType.Laboratory, _demoTenantId, "Sorologia para toxoplasmose", "Imunologia", "Não necessita jejum", "toxoplasmose,igg,igm,pré-natal", "40307735"),
                new ExamCatalog("Rubéola (IgG e IgM)", ExamType.Laboratory, _demoTenantId, "Sorologia para rubéola", "Imunologia", "Não necessita jejum", "rubéola,igg,igm,pré-natal", "40307654"),
                new ExamCatalog("Citomegalovírus (IgG e IgM)", ExamType.Laboratory, _demoTenantId, "Sorologia para citomegalovírus", "Imunologia", "Não necessita jejum", "cmv,citomegalovírus,pré-natal", "40307352"),
                new ExamCatalog("Fator Reumatoide", ExamType.Laboratory, _demoTenantId, "Pesquisa de fator reumatoide", "Imunologia", "Não necessita jejum", "fr,fator reumatoide,artrite", "40306500"),
                new ExamCatalog("FAN (Fator Antinúcleo)", ExamType.Laboratory, _demoTenantId, "Pesquisa de anticorpos antinucleares", "Imunologia", "Não necessita jejum", "fan,lúpus,autoimune", "40306496"),
                new ExamCatalog("Anti-DNA", ExamType.Laboratory, _demoTenantId, "Anticorpos anti-DNA nativo", "Imunologia", "Não necessita jejum", "anti dna,lúpus,autoimune", "40306402"),
                new ExamCatalog("Complemento C3 e C4", ExamType.Laboratory, _demoTenantId, "Dosagem de frações do complemento", "Imunologia", "Não necessita jejum", "c3,c4,complemento,autoimune", "40306461"),
                new ExamCatalog("Cortisol", ExamType.Laboratory, _demoTenantId, "Dosagem de cortisol sérico", "Hormônios", "Jejum de 4 horas, coleta até 9h da manhã", "cortisol,adrenal,estresse", "40316092"),
                new ExamCatalog("ACTH", ExamType.Laboratory, _demoTenantId, "Hormônio adrenocorticotrófico", "Hormônios", "Jejum de 4 horas, coleta até 9h da manhã", "acth,adrenal,hipófise", "40316009"),
                new ExamCatalog("Prolactina", ExamType.Laboratory, _demoTenantId, "Dosagem de prolactina", "Hormônios", "Repouso de 30 min antes da coleta", "prolactina,hipófise,galactorreia", "40316262"),
                new ExamCatalog("LH (Hormônio Luteinizante)", ExamType.Laboratory, _demoTenantId, "Hormônio luteinizante", "Hormônios", "Não necessita jejum", "lh,ovulação,fertilidade", "40316157"),
                new ExamCatalog("FSH (Hormônio Folículo-Estimulante)", ExamType.Laboratory, _demoTenantId, "Hormônio folículo-estimulante", "Hormônios", "Não necessita jejum", "fsh,fertilidade,menopausa", "40316114"),
                new ExamCatalog("Estradiol", ExamType.Laboratory, _demoTenantId, "Dosagem de estradiol", "Hormônios", "Não necessita jejum", "estradiol,estrogênio,fertilidade", "40316106"),
                new ExamCatalog("Progesterona", ExamType.Laboratory, _demoTenantId, "Dosagem de progesterona", "Hormônios", "Não necessita jejum", "progesterona,fertilidade,gravidez", "40316254"),
                new ExamCatalog("Testosterona Total", ExamType.Laboratory, _demoTenantId, "Dosagem de testosterona total", "Hormônios", "Não necessita jejum, coleta até 10h", "testosterona,homem,hipogonadismo", "40316505"),
                new ExamCatalog("Testosterona Livre", ExamType.Laboratory, _demoTenantId, "Dosagem de testosterona livre", "Hormônios", "Não necessita jejum", "testosterona livre,biodisponível", "40316513"),
                new ExamCatalog("PTH (Paratormônio)", ExamType.Laboratory, _demoTenantId, "Hormônio da paratireoide", "Hormônios", "Não necessita jejum", "pth,paratireoide,cálcio", "40316270"),
                new ExamCatalog("GH (Hormônio do Crescimento)", ExamType.Laboratory, _demoTenantId, "Hormônio do crescimento", "Hormônios", "Jejum de 10 horas, repouso", "gh,somatotropina,crescimento", "40316130"),
                new ExamCatalog("IGF-1", ExamType.Laboratory, _demoTenantId, "Fator de crescimento semelhante à insulina", "Hormônios", "Jejum de 4 horas", "igf1,somatomedina c,crescimento", "40316149"),
                new ExamCatalog("Insulina Basal", ExamType.Laboratory, _demoTenantId, "Dosagem de insulina em jejum", "Hormônios", "Jejum de 8 horas", "insulina,resistência insulínica", "40302296"),
                new ExamCatalog("Peptídeo C", ExamType.Laboratory, _demoTenantId, "Marcador de produção de insulina", "Hormônios", "Jejum de 8 horas", "peptídeo c,insulina,diabetes", "40316238"),
                new ExamCatalog("HOMA-IR", ExamType.Laboratory, _demoTenantId, "Índice de resistência à insulina", "Bioquímica", "Jejum de 8 horas", "homa,resistência insulínica,síndrome metabólica", "40302288"),

                // Exames de Imagem / Imaging Tests
                new ExamCatalog("Raio-X de Tórax PA e Perfil", ExamType.Imaging, _demoTenantId, "Radiografia de tórax em duas incidências", "Radiologia", "Não necessita preparo", "rx tórax,radiografia,pulmão", "41001010"),
                new ExamCatalog("Raio-X de Coluna Lombar", ExamType.Imaging, _demoTenantId, "Radiografia da coluna lombar", "Radiologia", "Não necessita preparo", "rx coluna,lombar,vértebra", "41001052"),
                new ExamCatalog("Raio-X de Coluna Cervical", ExamType.Imaging, _demoTenantId, "Radiografia da coluna cervical", "Radiologia", "Não necessita preparo", "rx coluna,cervical,pescoço", "41001044"),
                new ExamCatalog("Raio-X de Seios da Face", ExamType.Imaging, _demoTenantId, "Radiografia dos seios paranasais", "Radiologia", "Não necessita preparo", "rx seios da face,sinusite", "41001028"),
                new ExamCatalog("Raio-X de Abdome", ExamType.Imaging, _demoTenantId, "Radiografia simples de abdome", "Radiologia", "Não necessita preparo", "rx abdome,barriga", "41001087"),
                new ExamCatalog("Raio-X de Mão e Punho", ExamType.Imaging, _demoTenantId, "Radiografia de mão e punho", "Radiologia", "Não necessita preparo", "rx mão,punho,idade óssea", "41001109"),
                new ExamCatalog("Raio-X de Joelho", ExamType.Imaging, _demoTenantId, "Radiografia de joelho", "Radiologia", "Não necessita preparo", "rx joelho,articulação", "41001125"),
                new ExamCatalog("Raio-X de Quadril", ExamType.Imaging, _demoTenantId, "Radiografia de quadril", "Radiologia", "Não necessita preparo", "rx quadril,bacia", "41001141"),
                new ExamCatalog("Raio-X de Tornozelo", ExamType.Imaging, _demoTenantId, "Radiografia de tornozelo", "Radiologia", "Não necessita preparo", "rx tornozelo,pé", "41001150"),
                new ExamCatalog("Tomografia de Crânio", ExamType.Imaging, _demoTenantId, "Tomografia computadorizada de crânio", "Tomografia", "Não necessita preparo (sem contraste)", "tc crânio,cabeça,cérebro", "41001257"),
                new ExamCatalog("Tomografia de Tórax", ExamType.Imaging, _demoTenantId, "Tomografia computadorizada de tórax", "Tomografia", "Jejum de 4 horas se com contraste", "tc tórax,pulmão", "41001265"),
                new ExamCatalog("Tomografia de Abdome Total", ExamType.Imaging, _demoTenantId, "TC de abdome superior e inferior", "Tomografia", "Jejum de 6 horas", "tc abdome,abdominal", "41001273"),
                new ExamCatalog("Tomografia de Coluna", ExamType.Imaging, _demoTenantId, "TC de coluna vertebral", "Tomografia", "Não necessita preparo", "tc coluna,vértebra,hérnia", "41001281"),
                new ExamCatalog("Angiotomografia de Coronárias", ExamType.Imaging, _demoTenantId, "TC das artérias coronárias com contraste", "Tomografia", "Jejum de 4 horas, FC < 65 bpm", "angiotomografia,coronárias,coração", "41001290"),
                new ExamCatalog("Ressonância Magnética de Crânio", ExamType.Imaging, _demoTenantId, "RM de encéfalo", "Ressonância", "Não portar objetos metálicos", "rm crânio,cérebro,encéfalo", "41001320"),
                new ExamCatalog("Ressonância Magnética de Coluna", ExamType.Imaging, _demoTenantId, "RM de coluna vertebral", "Ressonância", "Não portar objetos metálicos", "rm coluna,hérnia de disco", "41001338"),
                new ExamCatalog("Ressonância Magnética de Joelho", ExamType.Imaging, _demoTenantId, "RM de joelho", "Ressonância", "Não portar objetos metálicos", "rm joelho,menisco,ligamento", "41001346"),
                new ExamCatalog("Ressonância Magnética de Ombro", ExamType.Imaging, _demoTenantId, "RM de ombro", "Ressonância", "Não portar objetos metálicos", "rm ombro,manguito rotador", "41001354"),
                new ExamCatalog("Ressonância Magnética de Pelve", ExamType.Imaging, _demoTenantId, "RM de pelve/bacia", "Ressonância", "Bexiga moderadamente cheia", "rm pelve,próstata,útero", "41001362"),
                new ExamCatalog("Ressonância Magnética de Abdome", ExamType.Imaging, _demoTenantId, "RM de abdome", "Ressonância", "Jejum de 6 horas", "rm abdome,fígado", "41001370"),
                new ExamCatalog("Ressonância Magnética de Mama", ExamType.Imaging, _demoTenantId, "RM de mamas", "Ressonância", "Realizar entre 7º e 14º dia do ciclo", "rm mama,mamografia", "41001389"),
                new ExamCatalog("Mamografia", ExamType.Imaging, _demoTenantId, "Mamografia bilateral", "Radiologia", "Não usar desodorante/talco no dia", "mamografia,mama,rastreamento", "41401093"),
                new ExamCatalog("Densitometria Óssea", ExamType.Imaging, _demoTenantId, "Avaliação da densidade mineral óssea", "Densitometria", "Não necessita preparo", "densitometria,osteoporose,osso", "41301084"),

                // Ultrassonografia / Ultrasound
                new ExamCatalog("Ultrassom de Abdome Total", ExamType.Ultrasound, _demoTenantId, "USG de abdome superior e inferior", "Ultrassonografia", "Jejum de 6 horas, bexiga cheia", "usg abdome,ultrassom abdominal", "41501055"),
                new ExamCatalog("Ultrassom de Abdome Superior", ExamType.Ultrasound, _demoTenantId, "USG de fígado, vesícula, vias biliares, pâncreas, baço", "Ultrassonografia", "Jejum de 6 horas", "usg abdome superior,fígado,vesícula", "41501047"),
                new ExamCatalog("Ultrassom de Tireoide", ExamType.Ultrasound, _demoTenantId, "USG de tireoide", "Ultrassonografia", "Não necessita preparo", "usg tireoide,nódulo tireoidiano", "41501128"),
                new ExamCatalog("Ultrassom de Mama", ExamType.Ultrasound, _demoTenantId, "USG mamária bilateral", "Ultrassonografia", "Não necessita preparo", "usg mama,nódulo mamário", "41501098"),
                new ExamCatalog("Ultrassom Pélvico", ExamType.Ultrasound, _demoTenantId, "USG pélvico via abdominal", "Ultrassonografia", "Bexiga cheia (ingerir 4-6 copos de água)", "usg pélvico,útero,ovário", "41501110"),
                new ExamCatalog("Ultrassom Transvaginal", ExamType.Ultrasound, _demoTenantId, "USG pélvico via transvaginal", "Ultrassonografia", "Bexiga vazia", "usg transvaginal,útero,ovário", "41501136"),
                new ExamCatalog("Ultrassom de Próstata", ExamType.Ultrasound, _demoTenantId, "USG de próstata via abdominal", "Ultrassonografia", "Bexiga cheia", "usg próstata,próstata", "41501144"),
                new ExamCatalog("Ultrassom de Próstata Transretal", ExamType.Ultrasound, _demoTenantId, "USG de próstata via transretal", "Ultrassonografia", "Fleet enema 2 horas antes", "usg transretal,próstata,biópsia", "41501152"),
                new ExamCatalog("Ultrassom de Rins e Vias Urinárias", ExamType.Ultrasound, _demoTenantId, "USG de rins, ureteres e bexiga", "Ultrassonografia", "Bexiga moderadamente cheia", "usg rins,vias urinárias,cálculo renal", "41501063"),
                new ExamCatalog("Ultrassom Obstétrico", ExamType.Ultrasound, _demoTenantId, "USG obstétrico com biometria fetal", "Ultrassonografia", "Bexiga moderadamente cheia (1º trimestre)", "usg obstétrico,gravidez,feto", "41501179"),
                new ExamCatalog("Ultrassom Morfológico", ExamType.Ultrasound, _demoTenantId, "USG morfológico fetal", "Ultrassonografia", "Não necessita preparo", "morfológico,anomalias fetais,gravidez", "41501187"),
                new ExamCatalog("Ultrassom com Doppler Obstétrico", ExamType.Ultrasound, _demoTenantId, "Doppler de artérias uterinas e umbilical", "Ultrassonografia", "Não necessita preparo", "doppler obstétrico,fluxo fetal", "41501195"),
                new ExamCatalog("Ultrassom Doppler de Carótidas e Vertebrais", ExamType.Ultrasound, _demoTenantId, "Doppler de artérias carótidas e vertebrais", "Ultrassonografia", "Não necessita preparo", "doppler carótidas,avc,estenose", "41501217"),
                new ExamCatalog("Ultrassom Doppler Venoso de MMII", ExamType.Ultrasound, _demoTenantId, "Doppler venoso de membros inferiores", "Ultrassonografia", "Não necessita preparo", "doppler venoso,trombose,varizes", "41501225"),
                new ExamCatalog("Ultrassom Doppler Arterial de MMII", ExamType.Ultrasound, _demoTenantId, "Doppler arterial de membros inferiores", "Ultrassonografia", "Não necessita preparo", "doppler arterial,claudicação", "41501233"),
                new ExamCatalog("Ultrassom de Bolsa Escrotal", ExamType.Ultrasound, _demoTenantId, "USG de testículos e epidídimo", "Ultrassonografia", "Não necessita preparo", "usg escrotal,testículo,varicocele", "41501160"),
                new ExamCatalog("Ultrassom de Partes Moles", ExamType.Ultrasound, _demoTenantId, "USG de partes moles superficiais", "Ultrassonografia", "Não necessita preparo", "usg partes moles,lipoma,cisto", "41501071"),
                new ExamCatalog("Ultrassom de Articulação", ExamType.Ultrasound, _demoTenantId, "USG musculoesquelético", "Ultrassonografia", "Não necessita preparo", "usg articulação,tendão,músculo", "41501241"),

                // Exames Cardíacos / Cardiac Tests
                new ExamCatalog("Eletrocardiograma (ECG)", ExamType.Cardiac, _demoTenantId, "ECG de repouso de 12 derivações", "Cardiologia", "Não necessita preparo", "ecg,eletrocardiograma,coração", "40101010"),
                new ExamCatalog("Ecocardiograma Transtorácico", ExamType.Cardiac, _demoTenantId, "Ecocardiograma com Doppler", "Cardiologia", "Não necessita preparo", "eco,ecocardiograma,coração", "40101150"),
                new ExamCatalog("Ecocardiograma Transesofágico", ExamType.Cardiac, _demoTenantId, "Eco via transesofágica", "Cardiologia", "Jejum de 6 horas", "eco transesofágico,átrio,válvula", "40101168"),
                new ExamCatalog("Ecocardiograma de Estresse", ExamType.Cardiac, _demoTenantId, "Eco com estresse farmacológico ou físico", "Cardiologia", "Suspender betabloqueador 24-48h antes", "eco estresse,isquemia", "40101176"),
                new ExamCatalog("Teste Ergométrico", ExamType.Cardiac, _demoTenantId, "Teste de esforço em esteira", "Cardiologia", "Roupas confortáveis, suspender betabloqueador", "teste ergométrico,esteira,esforço", "40101028"),
                new ExamCatalog("Holter 24 Horas", ExamType.Cardiac, _demoTenantId, "Monitorização contínua do ECG por 24h", "Cardiologia", "Banho antes da instalação", "holter,arritmia,monitorização", "40101036"),
                new ExamCatalog("MAPA 24 Horas", ExamType.Cardiac, _demoTenantId, "Monitorização ambulatorial da pressão arterial", "Cardiologia", "Não molhar o aparelho", "mapa,pressão arterial,hipertensão", "40101044"),
                new ExamCatalog("Cintilografia Miocárdica", ExamType.Cardiac, _demoTenantId, "Cintilografia de perfusão miocárdica", "Medicina Nuclear", "Jejum de 4 horas, suspender cafeína 24h", "cintilografia,miocárdio,isquemia", "40701018"),
                new ExamCatalog("Cateterismo Cardíaco", ExamType.Cardiac, _demoTenantId, "Coronariografia", "Cardiologia Intervencionista", "Jejum de 8 horas", "cateterismo,coronariografia,stent", "40702014"),

                // Endoscopia / Endoscopy
                new ExamCatalog("Endoscopia Digestiva Alta", ExamType.Endoscopy, _demoTenantId, "Exame do esôfago, estômago e duodeno", "Endoscopia", "Jejum de 8 horas", "endoscopia,esofago,estomago,duodeno", "40201016"),
                new ExamCatalog("Colonoscopia", ExamType.Endoscopy, _demoTenantId, "Exame do intestino grosso", "Endoscopia", "Preparo intestinal (laxante), dieta sem resíduos 2 dias antes", "colonoscopia,colon,intestino,polipos", "40201059"),
                new ExamCatalog("Retossigmoidoscopia", ExamType.Endoscopy, _demoTenantId, "Exame do reto e sigmóide", "Endoscopia", "Fleet enema 2 horas antes", "retossigmoidoscopia,reto,sigmóide", "40201067"),
                new ExamCatalog("Broncoscopia", ExamType.Endoscopy, _demoTenantId, "Exame das vias aéreas", "Pneumologia", "Jejum de 8 horas", "broncoscopia,brônquios,pulmão", "40201083"),
                new ExamCatalog("Cistoscopia", ExamType.Endoscopy, _demoTenantId, "Exame da bexiga", "Urologia", "Não necessita preparo especial", "cistoscopia,bexiga,uretra", "40201091"),
                new ExamCatalog("Laringoscopia", ExamType.Endoscopy, _demoTenantId, "Exame da laringe", "Otorrinolaringologia", "Jejum de 2 horas", "laringoscopia,laringe,cordas vocais", "40201105"),
                new ExamCatalog("Nasofibroscopia", ExamType.Endoscopy, _demoTenantId, "Exame das fossas nasais e faringe", "Otorrinolaringologia", "Não necessita preparo", "nasofibroscopia,nariz,faringe", "40201113"),
                new ExamCatalog("Colangiopancreatografia Retrógrada Endoscópica (CPRE)", ExamType.Endoscopy, _demoTenantId, "Exame e tratamento das vias biliares", "Endoscopia", "Jejum de 8 horas", "cpre,vias biliares,cálculo", "40201121"),
                new ExamCatalog("Ecoendoscopia (Endossonografia)", ExamType.Endoscopy, _demoTenantId, "Ultrassom endoscópico", "Endoscopia", "Jejum de 8 horas", "ecoendoscopia,pâncreas,estadiamento", "40201130"),

                // Biópsia / Biopsy
                new ExamCatalog("Biópsia de Pele", ExamType.Biopsy, _demoTenantId, "Biópsia cutânea", "Patologia", "Não necessita preparo especial", "biópsia pele,punch,lesão cutânea", "40501012"),
                new ExamCatalog("Biópsia de Mama (Core Biopsy)", ExamType.Biopsy, _demoTenantId, "Biópsia mamária guiada por ultrassom", "Patologia", "Suspender anticoagulantes se possível", "biópsia mama,core,nódulo", "40501020"),
                new ExamCatalog("Biópsia de Tireoide (PAAF)", ExamType.Biopsy, _demoTenantId, "Punção aspirativa por agulha fina de tireoide", "Patologia", "Não necessita preparo", "paaf tireoide,nódulo tireoidiano", "40501039"),
                new ExamCatalog("Biópsia de Próstata", ExamType.Biopsy, _demoTenantId, "Biópsia prostática transretal guiada por ultrassom", "Patologia", "Antibiótico profilático, fleet enema", "biópsia próstata,câncer próstata", "40501047"),
                new ExamCatalog("Biópsia de Linfonodo", ExamType.Biopsy, _demoTenantId, "Biópsia de gânglio linfático", "Patologia", "Não necessita preparo especial", "biópsia linfonodo,gânglio,linfoma", "40501055"),
                new ExamCatalog("Biópsia de Medula Óssea", ExamType.Biopsy, _demoTenantId, "Mielograma e biópsia de medula", "Hematologia", "Hemograma e coagulograma prévios", "mielograma,medula óssea,leucemia", "40501063"),
                new ExamCatalog("Biópsia Renal", ExamType.Biopsy, _demoTenantId, "Biópsia renal percutânea", "Patologia", "Coagulograma normal, suspender anticoagulantes", "biópsia renal,rim,glomerulonefrite", "40501071"),
                new ExamCatalog("Biópsia Hepática", ExamType.Biopsy, _demoTenantId, "Biópsia hepática percutânea", "Patologia", "Coagulograma normal, jejum de 6 horas", "biópsia fígado,hepática,cirrose", "40501080"),
                new ExamCatalog("Biópsia Endometrial", ExamType.Biopsy, _demoTenantId, "Biópsia do endométrio", "Patologia", "Não estar menstruada", "biópsia endométrio,útero,sangramento", "40501098"),
                new ExamCatalog("Biópsia Gástrica", ExamType.Biopsy, _demoTenantId, "Biópsia durante endoscopia digestiva", "Patologia", "Preparo de endoscopia", "biópsia gástrica,h pylori,úlcera", "40501101"),
                new ExamCatalog("Biópsia Colônica", ExamType.Biopsy, _demoTenantId, "Biópsia durante colonoscopia", "Patologia", "Preparo de colonoscopia", "biópsia colon,pólipo,câncer", "40501110"),

                // Outros Exames / Other Tests
                new ExamCatalog("Espirometria", ExamType.Other, _demoTenantId, "Prova de função pulmonar", "Pneumologia", "Não fumar 4-6 horas antes, não usar broncodilatador", "espirometria,asma,dpoc,função pulmonar", "40801012"),
                new ExamCatalog("Polissonografia", ExamType.Other, _demoTenantId, "Estudo do sono", "Neurologia/Pneumologia", "Evitar cafeína e álcool no dia", "polissonografia,apneia,sono", "40801039"),
                new ExamCatalog("Eletroencefalograma (EEG)", ExamType.Other, _demoTenantId, "Registro da atividade elétrica cerebral", "Neurologia", "Cabelos limpos, sem gel, pode ter privação de sono se solicitado", "eeg,eletroencefalograma,convulsão,epilepsia", "40801055"),
                new ExamCatalog("Eletroneuromiografia (ENMG)", ExamType.Other, _demoTenantId, "Avaliação de nervos e músculos periféricos", "Neurologia", "Evitar cremes/loções no dia", "enmg,eletromiografia,neuropatia,síndrome do túnel do carpo", "40801063"),
                new ExamCatalog("Potencial Evocado", ExamType.Other, _demoTenantId, "Avaliação das vias sensitivas", "Neurologia", "Cabelos limpos", "potencial evocado,esclerose múltipla", "40801071"),
                new ExamCatalog("Audiometria", ExamType.Other, _demoTenantId, "Avaliação da audição", "Otorrinolaringologia", "Repouso auditivo de 14 horas", "audiometria,surdez,audição", "40801080"),
                new ExamCatalog("Impedanciometria", ExamType.Other, _demoTenantId, "Avaliação do ouvido médio", "Otorrinolaringologia", "Não necessita preparo", "impedanciometria,otite,tímpano", "40801098"),
                new ExamCatalog("Videonistagmografia", ExamType.Other, _demoTenantId, "Avaliação do equilíbrio", "Otorrinolaringologia", "Jejum de 4 horas, evitar medicamentos sedativos", "videonistagmografia,labirintite,tontura,vertigem", "40801101"),
                new ExamCatalog("Campimetria Visual", ExamType.Other, _demoTenantId, "Avaliação do campo visual", "Oftalmologia", "Levar óculos se usar", "campimetria,campo visual,glaucoma", "40501128"),
                new ExamCatalog("Tonometria", ExamType.Other, _demoTenantId, "Medida da pressão intraocular", "Oftalmologia", "Não necessita preparo", "tonometria,pressão ocular,glaucoma", "40501136"),
                new ExamCatalog("Mapeamento de Retina", ExamType.Other, _demoTenantId, "Exame de fundo de olho com dilatação", "Oftalmologia", "Não dirigir após o exame (dilatação)", "fundo de olho,retina,retinopatia", "40501144"),
                new ExamCatalog("OCT (Tomografia de Coerência Óptica)", ExamType.Other, _demoTenantId, "Tomografia da retina e nervo óptico", "Oftalmologia", "Não necessita preparo", "oct,retina,mácula,glaucoma", "40501152"),
                new ExamCatalog("Colposcopia", ExamType.Other, _demoTenantId, "Exame do colo uterino com lente de aumento", "Ginecologia", "Não ter relação sexual 2 dias antes, não usar creme vaginal", "colposcopia,colo do útero,hpv,papanicolau", "40201148"),
                new ExamCatalog("Vulvoscopia", ExamType.Other, _demoTenantId, "Exame da vulva com lente de aumento", "Ginecologia", "Não usar cremes vaginais", "vulvoscopia,vulva,hpv", "40201156"),
                new ExamCatalog("Histeroscopia Diagnóstica", ExamType.Other, _demoTenantId, "Exame do interior do útero", "Ginecologia", "Realizar após menstruação", "histeroscopia,útero,mioma,pólipo", "40201164"),
                new ExamCatalog("Papanicolau (Citologia Oncótica)", ExamType.Other, _demoTenantId, "Exame preventivo do colo uterino", "Ginecologia", "Não ter relação sexual 2 dias antes, fora do período menstrual", "papanicolau,preventivo,câncer de colo", "40601013"),
                new ExamCatalog("Coleta de Swab Vaginal/Uretral", ExamType.Other, _demoTenantId, "Coleta para pesquisa de infecções", "Ginecologia/Urologia", "Não urinar 2 horas antes, sem ducha vaginal", "swab,dst,corrimento,uretrite", "40601021"),
                new ExamCatalog("Urodinâmica", ExamType.Other, _demoTenantId, "Estudo urodinâmico completo", "Urologia", "Não urinar 2 horas antes do exame", "urodinâmica,incontinência,bexiga", "40801110"),
                new ExamCatalog("Urofluxometria", ExamType.Other, _demoTenantId, "Medida do fluxo urinário", "Urologia", "Bexiga confortavelmente cheia", "urofluxometria,jato urinário,próstata", "40801128"),
                new ExamCatalog("Bioimpedância", ExamType.Other, _demoTenantId, "Avaliação da composição corporal", "Nutrição", "Jejum de 4 horas, não fazer exercício no dia", "bioimpedância,gordura corporal,massa magra", "40901012"),
                new ExamCatalog("Calorimetria Indireta", ExamType.Other, _demoTenantId, "Avaliação do gasto energético basal", "Nutrição", "Jejum de 12 horas, repouso antes do exame", "calorimetria,metabolismo,gasto energético", "40901020"),
                new ExamCatalog("Teste Alérgico (Prick Test)", ExamType.Other, _demoTenantId, "Teste cutâneo para alergias", "Alergologia", "Suspender anti-histamínicos 7 dias antes", "prick test,alergia,ige,rinite", "40901039"),
                new ExamCatalog("Teste de Provocação Oral", ExamType.Other, _demoTenantId, "Teste para alergia alimentar", "Alergologia", "Suspender anti-histamínicos, jejum", "provocação oral,alergia alimentar", "40901047"),
                new ExamCatalog("Teste do Pezinho Ampliado", ExamType.Other, _demoTenantId, "Triagem neonatal ampliada", "Pediatria", "Coletar entre 3º e 5º dia de vida", "teste do pezinho,triagem neonatal,fenilcetonúria", "40601030"),
                new ExamCatalog("PET-CT", ExamType.Other, _demoTenantId, "Tomografia por emissão de pósitrons", "Medicina Nuclear", "Jejum de 6 horas, evitar exercício 24h antes", "pet ct,pet scan,câncer,metástase,estadiamento", "40703014"),
                new ExamCatalog("Cintilografia de Tireoide", ExamType.Other, _demoTenantId, "Avaliação funcional da tireoide", "Medicina Nuclear", "Jejum de 4 horas", "cintilografia tireoide,nódulo,hipertireoidismo", "40703022"),
                new ExamCatalog("Cintilografia Óssea", ExamType.Other, _demoTenantId, "Avaliação de metástases ósseas", "Medicina Nuclear", "Boa hidratação", "cintilografia óssea,metástase,câncer", "40703030"),
                new ExamCatalog("Cintilografia Renal (DMSA)", ExamType.Other, _demoTenantId, "Avaliação da função renal", "Medicina Nuclear", "Boa hidratação", "dmsa,cintilografia renal,rim,cicatriz", "40703049")
            };
        }
    }
}
