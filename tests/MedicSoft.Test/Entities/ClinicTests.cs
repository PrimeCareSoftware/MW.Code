using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class ClinicTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesClinic()
        {
            // Arrange
            var name = "Medical Clinic";
            var tradeName = "MC Health";
            var document = "11222333000181"; // Valid CNPJ
            var phone = "+55 11 3333-4444";
            var email = "clinic@example.com";
            var address = "Main Street, 123";
            var openingTime = new TimeSpan(8, 0, 0);
            var closingTime = new TimeSpan(18, 0, 0);

            // Act
            var clinic = new Clinic(name, tradeName, document, phone, email, address, 
                openingTime, closingTime, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, clinic.Id);
            Assert.Equal(name, clinic.Name);
            Assert.Equal(tradeName, clinic.TradeName);
            Assert.Equal(document, clinic.Document);
            Assert.Equal(phone, clinic.Phone);
            Assert.Equal(email, clinic.Email);
            Assert.Equal(address, clinic.Address);
            Assert.Equal(openingTime, clinic.OpeningTime);
            Assert.Equal(closingTime, clinic.ClosingTime);
            Assert.Equal(30, clinic.AppointmentDurationMinutes);
            Assert.True(clinic.AllowEmergencySlots);
            Assert.True(clinic.IsActive);
        }

        [Fact]
        public void Constructor_WithCustomAppointmentDuration_CreatesClinic()
        {
            // Arrange
            var appointmentDuration = 45;

            // Act
            var clinic = CreateValidClinic(appointmentDuration);

            // Assert
            Assert.Equal(appointmentDuration, clinic.AppointmentDurationMinutes);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyName_ThrowsArgumentException(string name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic(name, "Trade Name", "11222333000181", "+55 11 3333-4444", 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyTradeName_ThrowsArgumentException(string tradeName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", tradeName, "11222333000181", "+55 11 3333-4444", 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Trade name cannot be empty (Parameter 'tradeName')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyDocument_ThrowsArgumentException(string document)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", document, "+55 11 3333-4444", 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Document cannot be empty (Parameter 'document')", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidCnpj_ThrowsArgumentException()
        {
            // Arrange
            var invalidCnpj = "11222333000182"; // Invalid CNPJ check digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", invalidCnpj, "+55 11 3333-4444", 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Invalid CNPJ format (Parameter 'document')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyPhone_ThrowsArgumentException(string phone)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", "11222333000181", phone, 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Phone cannot be empty (Parameter 'phone')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyEmail_ThrowsArgumentException(string email)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", "11222333000181", "+55 11 3333-4444", 
                    email, "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Email cannot be empty (Parameter 'email')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmptyAddress_ThrowsArgumentException(string address)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", "11222333000181", "+55 11 3333-4444", 
                    "clinic@example.com", address, new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId));

            Assert.Equal("Address cannot be empty (Parameter 'address')", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidAppointmentDuration_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", "11222333000181", "+55 11 3333-4444", 
                    "clinic@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId, 0));

            Assert.Equal("Appointment duration must be positive (Parameter 'appointmentDurationMinutes')", exception.Message);
        }

        [Fact]
        public void Constructor_WithOpeningTimeAfterClosingTime_ThrowsArgumentException()
        {
            // Arrange
            var openingTime = new TimeSpan(18, 0, 0);
            var closingTime = new TimeSpan(8, 0, 0);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Name", "Trade Name", "11222333000181", "+55 11 3333-4444", 
                    "clinic@example.com", "Address", openingTime, closingTime, _tenantId));

            Assert.Equal("Opening time must be before closing time", exception.Message);
        }

        [Fact]
        public void UpdateInfo_WithValidData_UpdatesClinic()
        {
            // Arrange
            var clinic = CreateValidClinic();
            var newName = "New Clinic Name";
            var newTradeName = "New Trade Name";
            var newPhone = "+55 11 5555-6666";
            var newEmail = "newclinic@example.com";
            var newAddress = "New Address, 456";

            // Act
            clinic.UpdateInfo(newName, newTradeName, newPhone, newEmail, newAddress);

            // Assert
            Assert.Equal(newName, clinic.Name);
            Assert.Equal(newTradeName, clinic.TradeName);
            Assert.Equal(newPhone, clinic.Phone);
            Assert.Equal(newEmail, clinic.Email);
            Assert.Equal(newAddress, clinic.Address);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void UpdateScheduleSettings_WithValidData_UpdatesSettings()
        {
            // Arrange
            var clinic = CreateValidClinic();
            var newOpeningTime = new TimeSpan(7, 0, 0);
            var newClosingTime = new TimeSpan(19, 0, 0);
            var newDuration = 45;
            var allowEmergency = false;

            // Act
            clinic.UpdateScheduleSettings(newOpeningTime, newClosingTime, newDuration, allowEmergency);

            // Assert
            Assert.Equal(newOpeningTime, clinic.OpeningTime);
            Assert.Equal(newClosingTime, clinic.ClosingTime);
            Assert.Equal(newDuration, clinic.AppointmentDurationMinutes);
            Assert.Equal(allowEmergency, clinic.AllowEmergencySlots);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void UpdateScheduleSettings_WithInvalidTimes_ThrowsArgumentException()
        {
            // Arrange
            var clinic = CreateValidClinic();
            var openingTime = new TimeSpan(18, 0, 0);
            var closingTime = new TimeSpan(8, 0, 0);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                clinic.UpdateScheduleSettings(openingTime, closingTime, 30, true));

            Assert.Equal("Opening time must be before closing time", exception.Message);
        }

        [Fact]
        public void UpdateScheduleSettings_WithInvalidDuration_ThrowsArgumentException()
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                clinic.UpdateScheduleSettings(new TimeSpan(8, 0, 0), new TimeSpan(18, 0, 0), -10, true));

            Assert.Equal("Appointment duration must be positive (Parameter 'appointmentDurationMinutes')", exception.Message);
        }

        [Fact]
        public void Deactivate_SetsClinicToInactive()
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act
            clinic.Deactivate();

            // Assert
            Assert.False(clinic.IsActive);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsClinicToActive()
        {
            // Arrange
            var clinic = CreateValidClinic();
            clinic.Deactivate();

            // Act
            clinic.Activate();

            // Assert
            Assert.True(clinic.IsActive);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Theory]
        [InlineData(8, 0, true)]   // 8:00 AM - within hours
        [InlineData(12, 0, true)]  // 12:00 PM - within hours
        [InlineData(18, 0, true)]  // 6:00 PM - closing time
        [InlineData(7, 59, false)] // 7:59 AM - before opening
        [InlineData(18, 1, false)] // 6:01 PM - after closing
        public void IsWithinWorkingHours_ReturnsCorrectValue(int hours, int minutes, bool expectedResult)
        {
            // Arrange
            var clinic = CreateValidClinic();
            var time = new TimeSpan(hours, minutes, 0);

            // Act
            var result = clinic.IsWithinWorkingHours(time);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EnablePublicDisplay_SetsShowOnPublicSiteToTrue()
        {
            // Arrange
            var clinic = CreateValidClinic();
            Assert.False(clinic.ShowOnPublicSite); // Default is false

            // Act
            clinic.EnablePublicDisplay();

            // Assert
            Assert.True(clinic.ShowOnPublicSite);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void DisablePublicDisplay_SetsShowOnPublicSiteToFalse()
        {
            // Arrange
            var clinic = CreateValidClinic();
            clinic.EnablePublicDisplay();
            Assert.True(clinic.ShowOnPublicSite);

            // Act
            clinic.DisablePublicDisplay();

            // Assert
            Assert.False(clinic.ShowOnPublicSite);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void UpdatePublicSiteSettings_WithValidData_UpdatesSettings()
        {
            // Arrange
            var clinic = CreateValidClinic();
            var whatsApp = "+5511999999999";

            // Act
            clinic.UpdatePublicSiteSettings(true, Domain.Enums.ClinicType.Dental, whatsApp);

            // Assert
            Assert.True(clinic.ShowOnPublicSite);
            Assert.Equal(Domain.Enums.ClinicType.Dental, clinic.ClinicType);
            Assert.Equal(whatsApp, clinic.WhatsAppNumber);
            Assert.NotNull(clinic.UpdatedAt);
        }

        [Fact]
        public void UpdatePublicSiteSettings_WithoutWhatsApp_UpdatesSettings()
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act
            clinic.UpdatePublicSiteSettings(true, Domain.Enums.ClinicType.Medical, null);

            // Assert
            Assert.True(clinic.ShowOnPublicSite);
            Assert.Equal(Domain.Enums.ClinicType.Medical, clinic.ClinicType);
            Assert.Null(clinic.WhatsAppNumber);
        }

        [Theory]
        [InlineData("11999999999")]  // Missing country code
        [InlineData("+55119")]        // Too short
        [InlineData("+551199999999999999")]  // Too long
        public void UpdatePublicSiteSettings_WithInvalidWhatsApp_ThrowsArgumentException(string whatsApp)
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                clinic.UpdatePublicSiteSettings(true, Domain.Enums.ClinicType.Medical, whatsApp));

            Assert.Contains("WhatsApp", exception.Message);
        }

        private Clinic CreateValidClinic(int appointmentDuration = 30)
        {
            return new Clinic(
                "Medical Clinic",
                "MC Health",
                "11222333000181", // Valid CNPJ
                "+55 11 3333-4444",
                "clinic@example.com",
                "Main Street, 123",
                new TimeSpan(8, 0, 0),
                new TimeSpan(18, 0, 0),
                _tenantId,
                appointmentDuration
            );
        }

        [Fact]
        public void Constructor_SetsDefaultEnableOnlineAppointmentSchedulingToTrue()
        {
            // Arrange & Act
            var clinic = CreateValidClinic();

            // Assert
            Assert.True(clinic.EnableOnlineAppointmentScheduling);
        }

        [Fact]
        public void EnableOnlineScheduling_EnablesOnlineAppointmentScheduling()
        {
            // Arrange
            var clinic = CreateValidClinic();
            clinic.DisableOnlineScheduling();

            // Act
            clinic.EnableOnlineScheduling();

            // Assert
            Assert.True(clinic.EnableOnlineAppointmentScheduling);
        }

        [Fact]
        public void DisableOnlineScheduling_DisablesOnlineAppointmentScheduling()
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act
            clinic.DisableOnlineScheduling();

            // Assert
            Assert.False(clinic.EnableOnlineAppointmentScheduling);
        }

        [Fact]
        public void UpdateOnlineSchedulingSetting_WithTrue_EnablesOnlineAppointmentScheduling()
        {
            // Arrange
            var clinic = CreateValidClinic();
            clinic.DisableOnlineScheduling();

            // Act
            clinic.UpdateOnlineSchedulingSetting(true);

            // Assert
            Assert.True(clinic.EnableOnlineAppointmentScheduling);
        }

        [Fact]
        public void UpdateOnlineSchedulingSetting_WithFalse_DisablesOnlineAppointmentScheduling()
        {
            // Arrange
            var clinic = CreateValidClinic();

            // Act
            clinic.UpdateOnlineSchedulingSetting(false);

            // Assert
            Assert.False(clinic.EnableOnlineAppointmentScheduling);
        }
    }
}
