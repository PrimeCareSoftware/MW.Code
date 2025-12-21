using System.Collections.Generic;
using System.Linq;

namespace MedicSoft.Domain.Common
{
    /// <summary>
    /// Defines all available permissions in the system.
    /// Permissions follow the pattern: "resource.action"
    /// </summary>
    public static class PermissionKeys
    {
        // Clinic Management
        public const string ClinicView = "clinic.view";
        public const string ClinicManage = "clinic.manage";

        // User Management
        public const string UsersView = "users.view";
        public const string UsersCreate = "users.create";
        public const string UsersEdit = "users.edit";
        public const string UsersDelete = "users.delete";

        // Profile Management
        public const string ProfilesView = "profiles.view";
        public const string ProfilesCreate = "profiles.create";
        public const string ProfilesEdit = "profiles.edit";
        public const string ProfilesDelete = "profiles.delete";

        // Patient Management
        public const string PatientsView = "patients.view";
        public const string PatientsCreate = "patients.create";
        public const string PatientsEdit = "patients.edit";
        public const string PatientsDelete = "patients.delete";

        // Appointment Management
        public const string AppointmentsView = "appointments.view";
        public const string AppointmentsCreate = "appointments.create";
        public const string AppointmentsEdit = "appointments.edit";
        public const string AppointmentsDelete = "appointments.delete";

        // Medical Records
        public const string MedicalRecordsView = "medical-records.view";
        public const string MedicalRecordsCreate = "medical-records.create";
        public const string MedicalRecordsEdit = "medical-records.edit";

        // Attendance
        public const string AttendanceView = "attendance.view";
        public const string AttendancePerform = "attendance.perform";

        // Procedures
        public const string ProceduresView = "procedures.view";
        public const string ProceduresCreate = "procedures.create";
        public const string ProceduresEdit = "procedures.edit";
        public const string ProceduresDelete = "procedures.delete";

        // Financial - Payments
        public const string PaymentsView = "payments.view";
        public const string PaymentsManage = "payments.manage";

        // Financial - Invoices
        public const string InvoicesView = "invoices.view";
        public const string InvoicesManage = "invoices.manage";

        // Financial - Expenses
        public const string ExpensesView = "expenses.view";
        public const string ExpensesCreate = "expenses.create";
        public const string ExpensesEdit = "expenses.edit";
        public const string ExpensesDelete = "expenses.delete";

        // Reports
        public const string ReportsFinancial = "reports.financial";
        public const string ReportsOperational = "reports.operational";

        // Medications
        public const string MedicationsView = "medications.view";
        public const string PrescriptionsCreate = "prescriptions.create";

        // Exams
        public const string ExamsView = "exams.view";
        public const string ExamsRequest = "exams.request";

        // Notifications
        public const string NotificationsView = "notifications.view";
        public const string NotificationsManage = "notifications.manage";

        // Waiting Queue
        public const string WaitingQueueView = "waiting-queue.view";
        public const string WaitingQueueManage = "waiting-queue.manage";

        /// <summary>
        /// Gets all available permissions grouped by category
        /// </summary>
        public static Dictionary<string, List<PermissionInfo>> GetAllPermissionsByCategory()
        {
            return new Dictionary<string, List<PermissionInfo>>
            {
                {
                    "Gestão da Clínica", new List<PermissionInfo>
                    {
                        new(ClinicView, "Visualizar configurações da clínica"),
                        new(ClinicManage, "Gerenciar configurações da clínica")
                    }
                },
                {
                    "Usuários", new List<PermissionInfo>
                    {
                        new(UsersView, "Visualizar usuários"),
                        new(UsersCreate, "Criar usuários"),
                        new(UsersEdit, "Editar usuários"),
                        new(UsersDelete, "Excluir usuários")
                    }
                },
                {
                    "Perfis de Acesso", new List<PermissionInfo>
                    {
                        new(ProfilesView, "Visualizar perfis de acesso"),
                        new(ProfilesCreate, "Criar perfis de acesso"),
                        new(ProfilesEdit, "Editar perfis de acesso"),
                        new(ProfilesDelete, "Excluir perfis de acesso")
                    }
                },
                {
                    "Pacientes", new List<PermissionInfo>
                    {
                        new(PatientsView, "Visualizar pacientes"),
                        new(PatientsCreate, "Cadastrar pacientes"),
                        new(PatientsEdit, "Editar pacientes"),
                        new(PatientsDelete, "Excluir pacientes")
                    }
                },
                {
                    "Agendamentos", new List<PermissionInfo>
                    {
                        new(AppointmentsView, "Visualizar agendamentos"),
                        new(AppointmentsCreate, "Criar agendamentos"),
                        new(AppointmentsEdit, "Editar agendamentos"),
                        new(AppointmentsDelete, "Excluir agendamentos")
                    }
                },
                {
                    "Prontuários", new List<PermissionInfo>
                    {
                        new(MedicalRecordsView, "Visualizar prontuários"),
                        new(MedicalRecordsCreate, "Criar prontuários"),
                        new(MedicalRecordsEdit, "Editar prontuários")
                    }
                },
                {
                    "Atendimento", new List<PermissionInfo>
                    {
                        new(AttendanceView, "Visualizar atendimentos"),
                        new(AttendancePerform, "Realizar atendimentos")
                    }
                },
                {
                    "Procedimentos", new List<PermissionInfo>
                    {
                        new(ProceduresView, "Visualizar procedimentos"),
                        new(ProceduresCreate, "Criar procedimentos"),
                        new(ProceduresEdit, "Editar procedimentos"),
                        new(ProceduresDelete, "Excluir procedimentos")
                    }
                },
                {
                    "Financeiro - Pagamentos", new List<PermissionInfo>
                    {
                        new(PaymentsView, "Visualizar pagamentos"),
                        new(PaymentsManage, "Gerenciar pagamentos")
                    }
                },
                {
                    "Financeiro - Notas Fiscais", new List<PermissionInfo>
                    {
                        new(InvoicesView, "Visualizar notas fiscais"),
                        new(InvoicesManage, "Gerenciar notas fiscais")
                    }
                },
                {
                    "Financeiro - Despesas", new List<PermissionInfo>
                    {
                        new(ExpensesView, "Visualizar despesas"),
                        new(ExpensesCreate, "Criar despesas"),
                        new(ExpensesEdit, "Editar despesas"),
                        new(ExpensesDelete, "Excluir despesas")
                    }
                },
                {
                    "Relatórios", new List<PermissionInfo>
                    {
                        new(ReportsFinancial, "Visualizar relatórios financeiros"),
                        new(ReportsOperational, "Visualizar relatórios operacionais")
                    }
                },
                {
                    "Medicamentos e Prescrições", new List<PermissionInfo>
                    {
                        new(MedicationsView, "Visualizar medicamentos"),
                        new(PrescriptionsCreate, "Criar prescrições")
                    }
                },
                {
                    "Exames", new List<PermissionInfo>
                    {
                        new(ExamsView, "Visualizar exames"),
                        new(ExamsRequest, "Solicitar exames")
                    }
                },
                {
                    "Notificações", new List<PermissionInfo>
                    {
                        new(NotificationsView, "Visualizar notificações"),
                        new(NotificationsManage, "Gerenciar notificações")
                    }
                },
                {
                    "Fila de Espera", new List<PermissionInfo>
                    {
                        new(WaitingQueueView, "Visualizar fila de espera"),
                        new(WaitingQueueManage, "Gerenciar fila de espera")
                    }
                }
            };
        }

        /// <summary>
        /// Gets all permission keys as a flat list
        /// </summary>
        public static List<string> GetAllPermissionKeys()
        {
            return GetAllPermissionsByCategory()
                .SelectMany(c => c.Value.Select(p => p.Key))
                .ToList();
        }
    }

    public record PermissionInfo(string Key, string Description);
}
