using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class PatientClinicLinkTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesLink()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            // Act
            var link = new PatientClinicLink(patientId, clinicId, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, link.Id);
            Assert.Equal(patientId, link.PatientId);
            Assert.Equal(clinicId, link.ClinicId);
            Assert.True(link.IsActive);
            Assert.Equal(_tenantId, link.TenantId);
            Assert.NotEqual(default(DateTime), link.LinkedAt);
            Assert.NotEqual(default(DateTime), link.CreatedAt);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Arrange
            var clinicId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                new PatientClinicLink(Guid.Empty, clinicId, _tenantId));
            
            Assert.Equal("Patient ID cannot be empty (Parameter 'patientId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                new PatientClinicLink(patientId, Guid.Empty, _tenantId));
            
            Assert.Equal("Clinic ID cannot be empty (Parameter 'clinicId')", exception.Message);
        }

        [Fact]
        public void Deactivate_SetsLinkToInactive()
        {
            // Arrange
            var link = CreateValidLink();
            Assert.True(link.IsActive);

            // Act
            link.Deactivate();

            // Assert
            Assert.False(link.IsActive);
            Assert.NotNull(link.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsLinkToActive()
        {
            // Arrange
            var link = CreateValidLink();
            link.Deactivate();
            Assert.False(link.IsActive);

            // Act
            link.Activate();

            // Assert
            Assert.True(link.IsActive);
            Assert.NotNull(link.UpdatedAt);
        }

        [Fact]
        public void Constructor_SetsLinkedAtToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var link = CreateValidLink();
            var afterCreation = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.True(link.LinkedAt >= beforeCreation);
            Assert.True(link.LinkedAt <= afterCreation);
        }

        [Fact]
        public void Constructor_NewLinkIsActiveByDefault()
        {
            // Act
            var link = CreateValidLink();

            // Assert
            Assert.True(link.IsActive);
        }

        [Fact]
        public void MultipleDeactivateCalls_DoesNotThrowException()
        {
            // Arrange
            var link = CreateValidLink();

            // Act & Assert
            link.Deactivate();
            Assert.False(link.IsActive);
            
            link.Deactivate();
            Assert.False(link.IsActive);
        }

        [Fact]
        public void MultipleActivateCalls_DoesNotThrowException()
        {
            // Arrange
            var link = CreateValidLink();

            // Act & Assert
            link.Activate();
            Assert.True(link.IsActive);
            
            link.Activate();
            Assert.True(link.IsActive);
        }

        private PatientClinicLink CreateValidLink()
        {
            return new PatientClinicLink(Guid.NewGuid(), Guid.NewGuid(), _tenantId);
        }
    }
}
