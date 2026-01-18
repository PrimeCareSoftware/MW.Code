using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class HealthInsuranceOperatorTests
    {
        private const string TenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesOperator()
        {
            // Arrange
            var tradeName = "Unimed São Paulo";
            var companyName = "Cooperativa Médica";
            var registerNumber = "123456";
            var document = "12345678000100";

            // Act
            var op = new HealthInsuranceOperator(tradeName, companyName, registerNumber, document, TenantId);

            // Assert
            op.Id.Should().NotBeEmpty();
            op.TradeName.Should().Be(tradeName);
            op.CompanyName.Should().Be(companyName);
            op.RegisterNumber.Should().Be(registerNumber);
            op.Document.Should().Be(document);
            op.TenantId.Should().Be(TenantId);
            op.IsActive.Should().BeTrue();
            op.IntegrationType.Should().Be(OperatorIntegrationType.Manual);
            op.SupportsTissXml.Should().BeFalse();
            op.RequiresPriorAuthorization.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithOptionalData_CreatesOperator()
        {
            // Arrange
            var phone = "11999999999";
            var email = "contato@unimed.com.br";
            var contactPerson = "João Silva";

            // Act
            var op = new HealthInsuranceOperator("Unimed", "Cooperativa", "123", "12345678000100", TenantId, phone, email, contactPerson);

            // Assert
            op.Phone.Should().Be(phone);
            op.Email.Should().Be(email);
            op.ContactPerson.Should().Be(contactPerson);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidTradeName_ThrowsArgumentException(string? tradeName)
        {
            // Act
            var act = () => new HealthInsuranceOperator(tradeName!, "Company", "123", "12345678000100", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Trade name*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCompanyName_ThrowsArgumentException(string? companyName)
        {
            // Act
            var act = () => new HealthInsuranceOperator("Trade", companyName!, "123", "12345678000100", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Company name*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidRegisterNumber_ThrowsArgumentException(string? registerNumber)
        {
            // Act
            var act = () => new HealthInsuranceOperator("Trade", "Company", registerNumber!, "12345678000100", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Register number*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidDocument_ThrowsArgumentException(string? document)
        {
            // Act
            var act = () => new HealthInsuranceOperator("Trade", "Company", "123", document!, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document*");
        }

        [Fact]
        public void UpdateBasicInfo_WithValidData_UpdatesOperator()
        {
            // Arrange
            var op = CreateValidOperator();
            var newTradeName = "Amil São Paulo";
            var newCompanyName = "Amil Assistência Médica";
            var newPhone = "11888888888";
            var newEmail = "novo@amil.com.br";
            var newContact = "Maria Santos";

            // Act
            op.UpdateBasicInfo(newTradeName, newCompanyName, newPhone, newEmail, newContact);

            // Assert
            op.TradeName.Should().Be(newTradeName);
            op.CompanyName.Should().Be(newCompanyName);
            op.Phone.Should().Be(newPhone);
            op.Email.Should().Be(newEmail);
            op.ContactPerson.Should().Be(newContact);
            op.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateBasicInfo_WithInvalidTradeName_ThrowsArgumentException()
        {
            // Arrange
            var op = CreateValidOperator();

            // Act
            var act = () => op.UpdateBasicInfo("", "Company", null, null, null);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ConfigureIntegration_WithValidData_ConfiguresOperator()
        {
            // Arrange
            var op = CreateValidOperator();
            var websiteUrl = "https://portal.unimed.com.br";
            var apiEndpoint = "https://api.unimed.com.br";
            var apiKey = "secret-key-123";

            // Act
            op.ConfigureIntegration(OperatorIntegrationType.RestApi, websiteUrl, apiEndpoint, apiKey, true);

            // Assert
            op.IntegrationType.Should().Be(OperatorIntegrationType.RestApi);
            op.WebsiteUrl.Should().Be(websiteUrl);
            op.ApiEndpoint.Should().Be(apiEndpoint);
            op.ApiKey.Should().Be(apiKey);
            op.RequiresPriorAuthorization.Should().BeTrue();
            op.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureTiss_WithValidData_ConfiguresTissSettings()
        {
            // Arrange
            var op = CreateValidOperator();
            var tissVersion = "4.02.00";
            var email = "tiss@unimed.com.br";

            // Act
            op.ConfigureTiss(tissVersion, true, email);

            // Assert
            op.TissVersion.Should().Be(tissVersion);
            op.SupportsTissXml.Should().BeTrue();
            op.BatchSubmissionEmail.Should().Be(email);
            op.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ConfigureTiss_WithInvalidVersion_ThrowsArgumentException(string? tissVersion)
        {
            // Arrange
            var op = CreateValidOperator();

            // Act
            var act = () => op.ConfigureTiss(tissVersion!, true);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*TISS version*");
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var op = CreateValidOperator();
            op.Deactivate();

            // Act
            op.Activate();

            // Assert
            op.IsActive.Should().BeTrue();
            op.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var op = CreateValidOperator();

            // Act
            op.Deactivate();

            // Assert
            op.IsActive.Should().BeFalse();
            op.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_TrimsWhitespace_FromAllStringProperties()
        {
            // Arrange & Act
            var op = new HealthInsuranceOperator(
                "  Trade  ",
                "  Company  ",
                "  123  ",
                "  12345678000100  ",
                TenantId,
                "  11999999999  ",
                "  email@test.com  ",
                "  João  "
            );

            // Assert
            op.TradeName.Should().Be("Trade");
            op.CompanyName.Should().Be("Company");
            op.RegisterNumber.Should().Be("123");
            op.Document.Should().Be("12345678000100");
            op.Phone.Should().Be("11999999999");
            op.Email.Should().Be("email@test.com");
            op.ContactPerson.Should().Be("João");
        }

        private HealthInsuranceOperator CreateValidOperator()
        {
            return new HealthInsuranceOperator(
                "Unimed São Paulo",
                "Cooperativa Médica",
                "123456",
                "12345678000100",
                TenantId
            );
        }
    }
}
