using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.Entities
{
    public class PatientTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesPatient()
        {
            // Arrange
            var name = "John Doe";
            var document = "11144477735"; // Valid CPF
            var dateOfBirth = DateTime.Now.AddYears(-30);
            var gender = "Male";
            var email = new Email("john@example.com");
            var phone = new Phone("+55", "11999999999");
            var address = CreateValidAddress();

            // Act
            var patient = new Patient(name, document, dateOfBirth, gender, email, phone, address, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, patient.Id);
            Assert.Equal(name, patient.Name);
            Assert.Equal(document, patient.Document);
            Assert.Equal(dateOfBirth, patient.DateOfBirth);
            Assert.Equal(gender, patient.Gender);
            Assert.Equal(email, patient.Email);
            Assert.Equal(phone, patient.Phone);
            Assert.Equal(address, patient.Address);
            Assert.True(patient.IsActive);
        }

        [Fact]
        public void Constructor_WithMedicalHistoryAndAllergies_CreatesPatient()
        {
            // Arrange
            var medicalHistory = "Diabetes Type 2";
            var allergies = "Penicillin";

            // Act
            var patient = CreateValidPatient(medicalHistory, allergies);

            // Assert
            Assert.Equal(medicalHistory, patient.MedicalHistory);
            Assert.Equal(allergies, patient.Allergies);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyName_ThrowsArgumentException(string name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient(name, "PASSPORT123", DateTime.Now.AddYears(-30), "Male",
                    new Email("test@example.com"), new Phone("+55", "11999999999"),
                    CreateValidAddress(), _tenantId));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyDocument_ThrowsArgumentException(string document)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient("John Doe", document, DateTime.Now.AddYears(-30), "Male",
                    new Email("test@example.com"), new Phone("+55", "11999999999"),
                    CreateValidAddress(), _tenantId));

            Assert.Equal("Document cannot be empty (Parameter 'document')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyGender_ThrowsArgumentException(string gender)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient("John Doe", "PASSPORT123", DateTime.Now.AddYears(-30), gender,
                    new Email("test@example.com"), new Phone("+55", "11999999999"),
                    CreateValidAddress(), _tenantId));

            Assert.Equal("Gender cannot be empty (Parameter 'gender')", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidCpf_ThrowsArgumentException()
        {
            // Arrange
            var invalidCpf = "12345678901"; // Invalid CPF check digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient("John Doe", invalidCpf, DateTime.Now.AddYears(-30), "Male",
                    new Email("test@example.com"), new Phone("+55", "11999999999"),
                    CreateValidAddress(), _tenantId));

            Assert.Equal("Invalid CPF format (Parameter 'document')", exception.Message);
        }

        [Fact]
        public void Constructor_WithFutureDateOfBirth_ThrowsArgumentException()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Patient("John Doe", "PASSPORT123", futureDate, "Male",
                    new Email("test@example.com"), new Phone("+55", "11999999999"),
                    CreateValidAddress(), _tenantId));

            Assert.Equal("Date of birth must be in the past (Parameter 'dateOfBirth')", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullEmail_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Patient("John Doe", "PASSPORT123", DateTime.Now.AddYears(-30), "Male",
                    null!, new Phone("+55", "11999999999"), CreateValidAddress(), _tenantId));

            Assert.Equal("email", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullPhone_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Patient("John Doe", "PASSPORT123", DateTime.Now.AddYears(-30), "Male",
                    new Email("test@example.com"), null!, CreateValidAddress(), _tenantId));

            Assert.Equal("phone", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullAddress_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Patient("John Doe", "PASSPORT123", DateTime.Now.AddYears(-30), "Male",
                    new Email("test@example.com"), new Phone("+55", "11999999999"), null!, _tenantId));

            Assert.Equal("address", exception.ParamName);
        }

        [Fact]
        public void UpdatePersonalInfo_WithValidData_UpdatesPatient()
        {
            // Arrange
            var patient = CreateValidPatient();
            var newName = "Jane Doe";
            var newEmail = new Email("jane@example.com");
            var newPhone = new Phone("+55", "11888888888");
            var newAddress = new Address("New Street", "456", "Uptown", "Rio de Janeiro", "RJ", "20000-000", "Brazil");

            // Act
            patient.UpdatePersonalInfo(newName, newEmail, newPhone, newAddress);

            // Assert
            Assert.Equal(newName, patient.Name);
            Assert.Equal(newEmail, patient.Email);
            Assert.Equal(newPhone, patient.Phone);
            Assert.Equal(newAddress, patient.Address);
            Assert.NotNull(patient.UpdatedAt);
        }

        [Fact]
        public void UpdateMedicalInfo_UpdatesMedicalHistoryAndAllergies()
        {
            // Arrange
            var patient = CreateValidPatient();
            var newHistory = "Updated medical history";
            var newAllergies = "Updated allergies";

            // Act
            patient.UpdateMedicalInfo(newHistory, newAllergies);

            // Assert
            Assert.Equal(newHistory, patient.MedicalHistory);
            Assert.Equal(newAllergies, patient.Allergies);
            Assert.NotNull(patient.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsPatientToInactive()
        {
            // Arrange
            var patient = CreateValidPatient();

            // Act
            patient.Deactivate();

            // Assert
            Assert.False(patient.IsActive);
            Assert.NotNull(patient.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsPatientToActive()
        {
            // Arrange
            var patient = CreateValidPatient();
            patient.Deactivate();

            // Act
            patient.Activate();

            // Assert
            Assert.True(patient.IsActive);
            Assert.NotNull(patient.UpdatedAt);
        }

        [Fact]
        public void GetAge_ReturnsCorrectAge()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-30).AddDays(-1);
            var patient = CreateValidPatient(dateOfBirth: dateOfBirth);

            // Act
            var age = patient.GetAge();

            // Assert
            Assert.Equal(30, age);
        }

        [Fact]
        public void GetAge_BeforeBirthday_ReturnsCorrectAge()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-30).AddDays(1);
            var patient = CreateValidPatient(dateOfBirth: dateOfBirth);

            // Act
            var age = patient.GetAge();

            // Assert
            Assert.Equal(29, age);
        }

        [Fact]
        public void AddHealthInsurancePlan_WithValidPlan_AddsPlan()
        {
            // Arrange
            var patient = CreateValidPatient();
            var patientId = patient.Id;
            var plan = new HealthInsurancePlan(patientId, "Unimed", "123456", DateTime.UtcNow, _tenantId);

            // Use reflection to set PatientId for testing
            var idProperty = typeof(HealthInsurancePlan).GetProperty("PatientId");
            idProperty?.SetValue(plan, patientId);

            // Act
            patient.AddHealthInsurancePlan(plan);

            // Assert
            Assert.Single(patient.HealthInsurancePlans);
        }

        [Fact]
        public void AddHealthInsurancePlan_WithNullPlan_ThrowsArgumentNullException()
        {
            // Arrange
            var patient = CreateValidPatient();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => patient.AddHealthInsurancePlan(null!));
            Assert.Equal("plan", exception.ParamName);
        }

        [Fact]
        public void RemoveHealthInsurancePlan_RemovesPlan()
        {
            // Arrange
            var patient = CreateValidPatient();
            var patientId = patient.Id;
            var plan = new HealthInsurancePlan(patientId, "Unimed", "123456", DateTime.UtcNow, _tenantId);
            
            var idProperty = typeof(HealthInsurancePlan).GetProperty("PatientId");
            idProperty?.SetValue(plan, patientId);
            
            patient.AddHealthInsurancePlan(plan);

            // Act
            patient.RemoveHealthInsurancePlan(plan.Id);

            // Assert
            Assert.Empty(patient.HealthInsurancePlans);
        }

        [Fact]
        public void GetActiveHealthInsurancePlans_ReturnsOnlyValidPlans()
        {
            // Arrange
            var patient = CreateValidPatient();
            var patientId = patient.Id;
            var activePlan = new HealthInsurancePlan(patientId, "Unimed", "111", DateTime.UtcNow.AddDays(-1), _tenantId, validUntil: DateTime.UtcNow.AddDays(30));
            var inactivePlan = new HealthInsurancePlan(patientId, "Amil", "222", DateTime.UtcNow, _tenantId);
            inactivePlan.Deactivate();

            var idProperty = typeof(HealthInsurancePlan).GetProperty("PatientId");
            idProperty?.SetValue(activePlan, patientId);
            idProperty?.SetValue(inactivePlan, patientId);

            patient.AddHealthInsurancePlan(activePlan);
            patient.AddHealthInsurancePlan(inactivePlan);

            // Act
            var activePlans = patient.GetActiveHealthInsurancePlans();

            // Assert
            Assert.Single(activePlans);
        }

        private Patient CreateValidPatient(string? medicalHistory = null, string? allergies = null, DateTime? dateOfBirth = null)
        {
            return new Patient(
                "Test Patient",
                "11144477735", // Valid CPF
                dateOfBirth ?? DateTime.Now.AddYears(-30),
                "Male",
                new Email("test@example.com"),
                new Phone("+55", "11999999999"),
                CreateValidAddress(),
                _tenantId,
                medicalHistory,
                allergies
            );
        }

        private Address CreateValidAddress()
        {
            return new Address("Main St", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil");
        }
    }
}
