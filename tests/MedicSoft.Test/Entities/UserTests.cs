using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class UserTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        [Theory]
        [InlineData(UserRole.Doctor)]
        [InlineData(UserRole.Dentist)]
        [InlineData(UserRole.Nurse)]
        [InlineData(UserRole.Psychologist)]
        public void IsProfessional_WithProfessionalRole_ReturnsTrue(UserRole role)
        {
            // Arrange
            var user = new User(
                username: "testuser",
                email: "test@example.com",
                passwordHash: "hashedpassword",
                fullName: "Test User",
                phone: "+1234567890",
                role: role,
                tenantId: _tenantId,
                clinicId: _clinicId
            );

            // Act
            var result = user.IsProfessional();

            // Assert
            Assert.True(result, $"User with role {role} should be identified as a professional");
        }

        [Theory]
        [InlineData(UserRole.SystemAdmin)]
        [InlineData(UserRole.ClinicOwner)]
        [InlineData(UserRole.Receptionist)]
        [InlineData(UserRole.Secretary)]
        public void IsProfessional_WithNonProfessionalRole_ReturnsFalse(UserRole role)
        {
            // Arrange
            var user = new User(
                username: "testuser",
                email: "test@example.com",
                passwordHash: "hashedpassword",
                fullName: "Test User",
                phone: "+1234567890",
                role: role,
                tenantId: _tenantId,
                clinicId: _clinicId
            );

            // Act
            var result = user.IsProfessional();

            // Assert
            Assert.False(result, $"User with role {role} should NOT be identified as a professional");
        }

        [Fact]
        public void IsProfessional_DoctorWithShowInAppointmentSchedulingTrue_IsCorrectlyIdentified()
        {
            // Arrange
            var user = new User(
                username: "doctor",
                email: "doctor@example.com",
                passwordHash: "hashedpassword",
                fullName: "Dr. Test",
                phone: "+1234567890",
                role: UserRole.Doctor,
                tenantId: _tenantId,
                clinicId: _clinicId,
                showInAppointmentScheduling: true
            );

            // Act
            var result = user.IsProfessional();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsProfessional_DoctorWithShowInAppointmentSchedulingFalse_StillIdentifiedAsProfessional()
        {
            // Arrange - IsProfessional checks role type, not the ShowInAppointmentScheduling flag
            var user = new User(
                username: "doctor",
                email: "doctor@example.com",
                passwordHash: "hashedpassword",
                fullName: "Dr. Test",
                phone: "+1234567890",
                role: UserRole.Doctor,
                tenantId: _tenantId,
                clinicId: _clinicId,
                showInAppointmentScheduling: false
            );

            // Act
            var result = user.IsProfessional();

            // Assert
            // IsProfessional() only checks the role type, not the visibility flag
            // The visibility flag is checked separately in the GetProfessionals() filter
            Assert.True(result);
        }
    }
}
