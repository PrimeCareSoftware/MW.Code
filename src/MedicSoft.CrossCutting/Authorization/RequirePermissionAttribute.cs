using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using System.Security.Claims;

namespace MedicSoft.CrossCutting.Authorization
{
    /// <summary>
    /// Authorization attribute that checks if the user has the required permission
    /// based on their role. This prevents unauthorized access to sensitive operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly Permission _requiredPermission;

        public RequirePermissionAttribute(Permission requiredPermission)
        {
            _requiredPermission = requiredPermission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                return;
            }

            // Get user's role from claims
            var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, out var userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if user has the required permission
            var permissions = GetRolePermissions(userRole);
            if (!permissions.Contains(_requiredPermission))
            {
                context.Result = new ForbidResult();
                return;
            }

            await Task.CompletedTask;
        }

        private static Permission[] GetRolePermissions(UserRole role)
        {
            return role switch
            {
                UserRole.SystemAdmin => new[]
                {
                    Permission.ViewAllClinics,
                    Permission.ManageSubscriptions,
                    Permission.ViewSystemAnalytics,
                    Permission.ManagePlans,
                    Permission.CrossTenantAccess,
                    Permission.ManageUsers,
                    Permission.ManageClinic,
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords,
                    Permission.ViewReports,
                    Permission.ManagePayments
                },
                UserRole.ClinicOwner => new[]
                {
                    Permission.ManageUsers,
                    Permission.ManageClinic,
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords,
                    Permission.ViewReports,
                    Permission.ManagePayments,
                    Permission.ManageSubscription
                },
                UserRole.Doctor => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                UserRole.Dentist => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                UserRole.Nurse => new[]
                {
                    Permission.ViewPatients,
                    Permission.ViewAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords // Limited editing
                },
                UserRole.Receptionist => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments
                },
                UserRole.Secretary => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ManagePayments
                    // Note: Secretary does NOT have ManageMedicalRecords permission
                    // This prevents editing of prescriptions and medical records
                },
                _ => Array.Empty<Permission>()
            };
        }
    }
}
