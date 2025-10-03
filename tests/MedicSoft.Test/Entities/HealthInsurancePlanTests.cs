using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.Entities
{
    public class HealthInsurancePlanTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _patientId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesHealthInsurancePlan()
        {
            // Arrange
            var insuranceName = "Unimed";
            var planNumber = "123456789";
            var validFrom = DateTime.UtcNow;

            // Act
            var plan = new HealthInsurancePlan(
                _patientId,
                insuranceName,
                planNumber,
                validFrom,
                _tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, plan.Id);
            Assert.Equal(_patientId, plan.PatientId);
            Assert.Equal(insuranceName, plan.InsuranceName);
            Assert.Equal(planNumber, plan.PlanNumber);
            Assert.Equal(validFrom, plan.ValidFrom);
            Assert.True(plan.IsActive);
            Assert.Null(plan.ValidUntil);
            Assert.Null(plan.PlanType);
            Assert.Null(plan.HolderName);
        }

        [Fact]
        public void Constructor_WithAllOptionalData_CreatesHealthInsurancePlan()
        {
            // Arrange
            var insuranceName = "Amil";
            var planNumber = "987654321";
            var planType = "Gold";
            var holderName = "John Doe";
            var validFrom = DateTime.UtcNow;
            var validUntil = DateTime.UtcNow.AddYears(1);

            // Act
            var plan = new HealthInsurancePlan(
                _patientId,
                insuranceName,
                planNumber,
                validFrom,
                _tenantId,
                planType,
                validUntil,
                holderName
            );

            // Assert
            Assert.Equal(planType, plan.PlanType);
            Assert.Equal(validUntil, plan.ValidUntil);
            Assert.Equal(holderName, plan.HolderName);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new HealthInsurancePlan(
                    Guid.Empty,
                    "Unimed",
                    "123456789",
                    DateTime.UtcNow,
                    _tenantId
                )
            );

            Assert.Equal("Patient ID cannot be empty (Parameter 'patientId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyInsuranceName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new HealthInsurancePlan(
                    _patientId,
                    "",
                    "123456789",
                    DateTime.UtcNow,
                    _tenantId
                )
            );

            Assert.Equal("Insurance name cannot be empty (Parameter 'insuranceName')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyPlanNumber_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new HealthInsurancePlan(
                    _patientId,
                    "Unimed",
                    "",
                    DateTime.UtcNow,
                    _tenantId
                )
            );

            Assert.Equal("Plan number cannot be empty (Parameter 'planNumber')", exception.Message);
        }

        [Fact]
        public void Constructor_WithValidUntilBeforeValidFrom_ThrowsArgumentException()
        {
            // Arrange
            var validFrom = DateTime.UtcNow;
            var validUntil = validFrom.AddDays(-1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new HealthInsurancePlan(
                    _patientId,
                    "Unimed",
                    "123456789",
                    validFrom,
                    _tenantId,
                    validUntil: validUntil
                )
            );

            Assert.Equal("Valid until date must be after valid from date (Parameter 'validUntil')", exception.Message);
        }

        [Fact]
        public void UpdatePlanInfo_WithValidData_UpdatesPlan()
        {
            // Arrange
            var plan = CreateValidPlan();
            var newInsuranceName = "Bradesco Saúde";
            var newPlanNumber = "999999999";
            var newPlanType = "Premium";
            var newHolderName = "Jane Doe";

            // Act
            plan.UpdatePlanInfo(newInsuranceName, newPlanNumber, newPlanType, newHolderName);

            // Assert
            Assert.Equal(newInsuranceName, plan.InsuranceName);
            Assert.Equal(newPlanNumber, plan.PlanNumber);
            Assert.Equal(newPlanType, plan.PlanType);
            Assert.Equal(newHolderName, plan.HolderName);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void UpdateValidityPeriod_WithValidDates_UpdatesPeriod()
        {
            // Arrange
            var plan = CreateValidPlan();
            var newValidFrom = DateTime.UtcNow.AddDays(30);
            var newValidUntil = newValidFrom.AddYears(1);

            // Act
            plan.UpdateValidityPeriod(newValidFrom, newValidUntil);

            // Assert
            Assert.Equal(newValidFrom, plan.ValidFrom);
            Assert.Equal(newValidUntil, plan.ValidUntil);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsPlanToInactive()
        {
            // Arrange
            var plan = CreateValidPlan();

            // Act
            plan.Deactivate();

            // Assert
            Assert.False(plan.IsActive);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsPlanToActive()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.Deactivate();

            // Act
            plan.Activate();

            // Assert
            Assert.True(plan.IsActive);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void IsValid_WithActiveAndValidDates_ReturnsTrue()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(-30);
            var validUntil = DateTime.UtcNow.AddDays(30);
            var plan = new HealthInsurancePlan(
                _patientId,
                "Unimed",
                "123456789",
                validFrom,
                _tenantId,
                validUntil: validUntil
            );

            // Act
            var isValid = plan.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_WithInactivePlan_ReturnsFalse()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.Deactivate();

            // Act
            var isValid = plan.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_WithExpiredPlan_ReturnsFalse()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(-60);
            var validUntil = DateTime.UtcNow.AddDays(-30);
            var plan = new HealthInsurancePlan(
                _patientId,
                "Unimed",
                "123456789",
                validFrom,
                _tenantId,
                validUntil: validUntil
            );

            // Act
            var isValid = plan.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_WithFuturePlan_ReturnsFalse()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(30);
            var plan = new HealthInsurancePlan(
                _patientId,
                "Unimed",
                "123456789",
                validFrom,
                _tenantId
            );

            // Act
            var isValid = plan.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Patient_CanHaveMultipleHealthInsurancePlans()
        {
            // Arrange
            var patient = CreateValidPatient();
            var plan1 = new HealthInsurancePlan(_patientId, "Unimed", "111111111", DateTime.UtcNow, _tenantId);
            var plan2 = new HealthInsurancePlan(_patientId, "Amil", "222222222", DateTime.UtcNow, _tenantId);

            // Act
            patient.AddHealthInsurancePlan(plan1);
            patient.AddHealthInsurancePlan(plan2);

            // Assert
            Assert.Equal(2, patient.HealthInsurancePlans.Count);
        }

        [Fact]
        public void Patient_GetActiveHealthInsurancePlans_ReturnsOnlyValidPlans()
        {
            // Arrange
            var patient = CreateValidPatient();
            var activePlan = new HealthInsurancePlan(_patientId, "Unimed", "111111111", DateTime.UtcNow.AddDays(-1), _tenantId, validUntil: DateTime.UtcNow.AddDays(30));
            var inactivePlan = new HealthInsurancePlan(_patientId, "Amil", "222222222", DateTime.UtcNow, _tenantId);
            inactivePlan.Deactivate();

            patient.AddHealthInsurancePlan(activePlan);
            patient.AddHealthInsurancePlan(inactivePlan);

            // Act
            var activePlans = patient.GetActiveHealthInsurancePlans();

            // Assert
            Assert.Single(activePlans);
        }

        [Fact]
        public void Patient_CanRemoveHealthInsurancePlan()
        {
            // Arrange
            var patient = CreateValidPatient();
            var plan = new HealthInsurancePlan(_patientId, "Unimed", "111111111", DateTime.UtcNow, _tenantId);
            patient.AddHealthInsurancePlan(plan);

            // Act
            patient.RemoveHealthInsurancePlan(plan.Id);

            // Assert
            Assert.Empty(patient.HealthInsurancePlans);
        }

        private HealthInsurancePlan CreateValidPlan()
        {
            return new HealthInsurancePlan(
                _patientId,
                "Unimed",
                "123456789",
                DateTime.UtcNow,
                _tenantId
            );
        }

        private Patient CreateValidPatient()
        {
            var email = new Email("test@example.com");
            var phone = new Phone("+55", "11999999999");
            var address = new Address("Main St", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil");
            
            var patient = new Patient(
                "Test Patient",
                "12345678900",
                DateTime.Now.AddYears(-30),
                "Male",
                email,
                phone,
                address,
                _tenantId
            );

            // Use reflection to set the Id to match our _patientId for testing
            var idProperty = typeof(Patient).BaseType?.GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(patient, _patientId);
            }

            return patient;
        }
    }
}
