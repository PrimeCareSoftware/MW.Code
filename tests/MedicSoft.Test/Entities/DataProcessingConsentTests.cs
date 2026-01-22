using System;
using System.Collections.Generic;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class DataProcessingConsentTests
    {
        [Fact]
        public void Constructor_ShouldCreateConsentSuccessfully()
        {
            // Arrange
            var userId = "user123";
            var purpose = LgpdPurpose.HEALTHCARE;
            var purposeDescription = "Medical treatment";
            var dataCategories = new List<DataCategory> { DataCategory.SENSITIVE, DataCategory.PERSONAL };
            var consentText = "I consent to the processing of my data for healthcare purposes.";
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var consentMethod = "WEB";
            var tenantId = "tenant123";

            // Act
            var consent = new DataProcessingConsent(
                userId,
                purpose,
                purposeDescription,
                dataCategories,
                consentText,
                ipAddress,
                userAgent,
                consentMethod,
                tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, consent.Id);
            Assert.Equal(userId, consent.UserId);
            Assert.True(consent.ConsentDate <= DateTime.UtcNow);
            Assert.Null(consent.RevokedDate);
            Assert.False(consent.IsRevoked);
            Assert.Equal(purpose, consent.Purpose);
            Assert.Equal(purposeDescription, consent.PurposeDescription);
            Assert.Equal(dataCategories, consent.DataCategories);
            Assert.Equal(consentText, consent.ConsentText);
            Assert.Equal(ipAddress, consent.IpAddress);
            Assert.Equal(userAgent, consent.UserAgent);
            Assert.Equal(consentMethod, consent.ConsentMethod);
            Assert.Equal(tenantId, consent.TenantId);
        }

        [Fact]
        public void Revoke_ShouldRevokeConsentSuccessfully()
        {
            // Arrange
            var consent = CreateSampleConsent();

            // Act
            consent.Revoke();

            // Assert
            Assert.True(consent.IsRevoked);
            Assert.NotNull(consent.RevokedDate);
            Assert.True(consent.RevokedDate <= DateTime.UtcNow);
        }

        [Fact]
        public void Revoke_WhenAlreadyRevoked_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var consent = CreateSampleConsent();
            consent.Revoke();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => consent.Revoke());
            Assert.Contains("já foi revogado", exception.Message.ToLower());
        }

        [Fact]
        public void Constructor_WithMultipleDataCategories_ShouldStoreAllCategories()
        {
            // Arrange
            var dataCategories = new List<DataCategory>
            {
                DataCategory.SENSITIVE,
                DataCategory.PERSONAL,
                DataCategory.CONFIDENTIAL
            };

            // Act
            var consent = CreateSampleConsent(dataCategories);

            // Assert
            Assert.Equal(3, consent.DataCategories.Count);
            Assert.Contains(DataCategory.SENSITIVE, consent.DataCategories);
            Assert.Contains(DataCategory.PERSONAL, consent.DataCategories);
            Assert.Contains(DataCategory.CONFIDENTIAL, consent.DataCategories);
        }

        [Theory]
        [InlineData(null)]
        public void Constructor_WithNullUserId_ShouldThrowArgumentNullException(string? nullValue)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DataProcessingConsent(
                nullValue!,
                LgpdPurpose.HEALTHCARE,
                "purpose",
                new List<DataCategory> { DataCategory.PERSONAL },
                "consent text",
                "192.168.1.1",
                "Mozilla/5.0",
                "WEB",
                "tenant"
            ));
        }

        [Fact]
        public void Constructor_WithNullDataCategories_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DataProcessingConsent(
                "user123",
                LgpdPurpose.HEALTHCARE,
                "purpose",
                null!,
                "consent text",
                "192.168.1.1",
                "Mozilla/5.0",
                "WEB",
                "tenant"
            ));
        }

        private DataProcessingConsent CreateSampleConsent(List<DataCategory>? dataCategories = null)
        {
            return new DataProcessingConsent(
                "user123",
                LgpdPurpose.HEALTHCARE,
                "Medical treatment and health monitoring",
                dataCategories ?? new List<DataCategory> { DataCategory.SENSITIVE, DataCategory.PERSONAL },
                "I consent to the processing of my health data for medical purposes.",
                "192.168.1.1",
                "Mozilla/5.0",
                "WEB",
                "tenant123"
            );
        }
    }
}
