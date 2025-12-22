using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an access profile (role) with a collection of permissions.
    /// Clinic owners can create custom profiles or use default ones.
    /// </summary>
    public class AccessProfile : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsDefault { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? ClinicId { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }
        private readonly List<ProfilePermission> _permissions = new();
        public IReadOnlyCollection<ProfilePermission> Permissions => _permissions.AsReadOnly();

        private AccessProfile()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
        }

        public AccessProfile(string name, string description, string tenantId, 
            Guid? clinicId = null, bool isDefault = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Profile description cannot be empty", nameof(description));

            Name = name.Trim();
            Description = description.Trim();
            ClinicId = clinicId;
            IsDefault = isDefault;
            IsActive = true;
        }

        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Profile description cannot be empty", nameof(description));

            Name = name.Trim();
            Description = description.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (IsDefault)
                throw new InvalidOperationException("Cannot deactivate default profiles");

            IsActive = false;
            UpdateTimestamp();
        }

        public void AddPermission(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                throw new ArgumentException("Permission key cannot be empty", nameof(permissionKey));

            if (_permissions.Any(p => p.PermissionKey == permissionKey))
                return; // Permission already exists

            var permission = new ProfilePermission(Id, permissionKey, TenantId);
            _permissions.Add(permission);
            UpdateTimestamp();
        }

        public void RemovePermission(string permissionKey)
        {
            var permission = _permissions.FirstOrDefault(p => p.PermissionKey == permissionKey);
            if (permission != null)
            {
                _permissions.Remove(permission);
                UpdateTimestamp();
            }
        }

        public void SetPermissions(IEnumerable<string> permissionKeys)
        {
            _permissions.Clear();
            foreach (var key in permissionKeys.Distinct())
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _permissions.Add(new ProfilePermission(Id, key, TenantId));
                }
            }
            UpdateTimestamp();
        }

        public bool HasPermission(string permissionKey)
        {
            return _permissions.Any(p => p.PermissionKey == permissionKey && p.IsActive);
        }

        public IEnumerable<string> GetPermissionKeys()
        {
            return _permissions.Where(p => p.IsActive).Select(p => p.PermissionKey);
        }

        /// <summary>
        /// Creates default profile templates with standard permissions
        /// </summary>
        public static AccessProfile CreateDefaultOwnerProfile(string tenantId, Guid clinicId)
        {
            var profile = new AccessProfile(
                "Proprietário",
                "Acesso total à clínica - pode gerenciar tudo",
                tenantId,
                clinicId,
                isDefault: true
            );

            // Owner has all permissions
            var ownerPermissions = new[]
            {
                // Clinic management
                "clinic.view", "clinic.manage",
                // Users and profiles
                "users.view", "users.create", "users.edit", "users.delete",
                "profiles.view", "profiles.create", "profiles.edit", "profiles.delete",
                // Patients
                "patients.view", "patients.create", "patients.edit", "patients.delete",
                // Appointments
                "appointments.view", "appointments.create", "appointments.edit", "appointments.delete",
                // Medical records
                "medical-records.view", "medical-records.create", "medical-records.edit",
                // Attendance
                "attendance.view", "attendance.perform",
                // Procedures
                "procedures.view", "procedures.create", "procedures.edit", "procedures.delete",
                // Financial
                "payments.view", "payments.manage", "invoices.view", "invoices.manage",
                "expenses.view", "expenses.create", "expenses.edit", "expenses.delete",
                "reports.financial", "reports.operational",
                // Medications and prescriptions
                "medications.view", "prescriptions.create",
                // Exams
                "exams.view", "exams.request",
                // Notifications
                "notifications.view", "notifications.manage",
                // Waiting queue
                "waiting-queue.view", "waiting-queue.manage"
            };

            profile.SetPermissions(ownerPermissions);
            return profile;
        }

        public static AccessProfile CreateDefaultMedicalProfile(string tenantId, Guid clinicId)
        {
            var profile = new AccessProfile(
                "Médico",
                "Acesso médico - atendimento, prontuários e prescrições",
                tenantId,
                clinicId,
                isDefault: true
            );

            var medicalPermissions = new[]
            {
                // Patients (view and basic management)
                "patients.view", "patients.create", "patients.edit",
                // Appointments
                "appointments.view", "appointments.create", "appointments.edit",
                // Medical records (full access)
                "medical-records.view", "medical-records.create", "medical-records.edit",
                // Attendance
                "attendance.view", "attendance.perform",
                // Procedures (view only)
                "procedures.view",
                // Medications and prescriptions
                "medications.view", "prescriptions.create",
                // Exams
                "exams.view", "exams.request",
                // Notifications
                "notifications.view",
                // Waiting queue
                "waiting-queue.view"
            };

            profile.SetPermissions(medicalPermissions);
            return profile;
        }

        public static AccessProfile CreateDefaultReceptionProfile(string tenantId, Guid clinicId)
        {
            var profile = new AccessProfile(
                "Recepção/Secretaria",
                "Acesso de recepção - agendamentos, pacientes e pagamentos",
                tenantId,
                clinicId,
                isDefault: true
            );

            var receptionPermissions = new[]
            {
                // Patients
                "patients.view", "patients.create", "patients.edit",
                // Appointments (full management)
                "appointments.view", "appointments.create", "appointments.edit", "appointments.delete",
                // Medical records (view only)
                "medical-records.view",
                // Procedures (view only)
                "procedures.view",
                // Financial (basic)
                "payments.view", "payments.manage",
                // Notifications
                "notifications.view", "notifications.manage",
                // Waiting queue
                "waiting-queue.view", "waiting-queue.manage"
            };

            profile.SetPermissions(receptionPermissions);
            return profile;
        }

        public static AccessProfile CreateDefaultFinancialProfile(string tenantId, Guid clinicId)
        {
            var profile = new AccessProfile(
                "Financeiro",
                "Acesso financeiro - pagamentos, despesas e relatórios",
                tenantId,
                clinicId,
                isDefault: true
            );

            var financialPermissions = new[]
            {
                // Patients (view only for billing)
                "patients.view",
                // Appointments (view only)
                "appointments.view",
                // Procedures (view only)
                "procedures.view",
                // Financial (full access)
                "payments.view", "payments.manage",
                "invoices.view", "invoices.manage",
                "expenses.view", "expenses.create", "expenses.edit", "expenses.delete",
                "reports.financial",
                // Notifications
                "notifications.view"
            };

            profile.SetPermissions(financialPermissions);
            return profile;
        }
    }
}
