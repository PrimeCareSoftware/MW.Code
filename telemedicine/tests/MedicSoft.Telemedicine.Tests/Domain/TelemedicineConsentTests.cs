using MedicSoft.Telemedicine.Domain.Entities;
using Xunit;

namespace MedicSoft.Telemedicine.Tests.Domain;

/// <summary>
/// Unit tests for TelemedicineConsent entity
/// Tests CFM 2.314/2022 compliance requirements
/// </summary>
public class TelemedicineConsentTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesConsent()
    {
        // Arrange
        var tenantId = "tenant-1";
        var patientId = Guid.NewGuid();
        var consentText = "I consent to telemedicine";
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var acceptsRecording = true;
        var acceptsDataSharing = true;

        // Act
        var consent = new TelemedicineConsent(
            tenantId,
            patientId,
            consentText,
            ipAddress,
            userAgent,
            acceptsRecording,
            acceptsDataSharing);

        // Assert
        Assert.NotEqual(Guid.Empty, consent.Id);
        Assert.Equal(tenantId, consent.TenantId);
        Assert.Equal(patientId, consent.PatientId);
        Assert.Equal(consentText, consent.ConsentText);
        Assert.Equal(ipAddress, consent.IpAddress);
        Assert.Equal(userAgent, consent.UserAgent);
        Assert.True(consent.AcceptsRecording);
        Assert.True(consent.AcceptsDataSharing);
        Assert.True(consent.IsActive);
        Assert.Null(consent.RevokedAt);
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ThrowsArgumentException()
    {
        // Arrange
        var tenantId = "";
        var patientId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new TelemedicineConsent(
                tenantId,
                patientId,
                "consent text",
                "192.168.1.1",
                "user agent",
                true,
                true));
    }

    [Fact]
    public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
    {
        // Arrange
        var tenantId = "tenant-1";
        var patientId = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new TelemedicineConsent(
                tenantId,
                patientId,
                "consent text",
                "192.168.1.1",
                "user agent",
                true,
                true));
    }

    [Fact]
    public void Constructor_WithEmptyConsentText_ThrowsArgumentException()
    {
        // Arrange
        var tenantId = "tenant-1";
        var patientId = Guid.NewGuid();
        var consentText = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new TelemedicineConsent(
                tenantId,
                patientId,
                consentText,
                "192.168.1.1",
                "user agent",
                true,
                true));
    }

    [Fact]
    public void Revoke_WithActiveConsent_RevokesConsent()
    {
        // Arrange
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);
        var reason = "Patient requested revocation";

        // Act
        consent.Revoke(reason);

        // Assert
        Assert.False(consent.IsActive);
        Assert.NotNull(consent.RevokedAt);
        Assert.Equal(reason, consent.RevocationReason);
    }

    [Fact]
    public void Revoke_WithAlreadyRevokedConsent_ThrowsInvalidOperationException()
    {
        // Arrange
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);
        consent.Revoke("First reason");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            consent.Revoke("Second reason"));
    }

    [Fact]
    public void Revoke_WithEmptyReason_ThrowsArgumentException()
    {
        // Arrange
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            consent.Revoke(""));
    }

    [Fact]
    public void IsValidForSession_WithActiveConsent_ReturnsTrue()
    {
        // Arrange
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);

        // Act
        var isValid = consent.IsValidForSession();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidForSession_WithRevokedConsent_ReturnsFalse()
    {
        // Arrange
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);
        consent.Revoke("Patient requested");

        // Act
        var isValid = consent.IsValidForSession();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Constructor_WithAppointmentId_CreatesConsentLinkedToAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        // Act
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true,
            appointmentId);

        // Assert
        Assert.Equal(appointmentId, consent.AppointmentId);
    }

    [Fact]
    public void Constructor_WithDigitalSignature_StoresSignature()
    {
        // Arrange
        var digitalSignature = "SIGNATURE_HASH_123456";

        // Act
        var consent = new TelemedicineConsent(
            "tenant-1",
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true,
            null,
            digitalSignature);

        // Assert
        Assert.Equal(digitalSignature, consent.DigitalSignature);
    }
}
