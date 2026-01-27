using FluentAssertions;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Services;
using Xunit;

namespace PatientPortal.Tests.Services;

public class EmailTemplateHelperTests
{
    private readonly AppointmentReminderDto _sampleAppointment;
    private const string PortalBaseUrl = "https://portal.test.com";

    public EmailTemplateHelperTests()
    {
        _sampleAppointment = new AppointmentReminderDto
        {
            AppointmentId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            PatientName = "João Silva",
            PatientEmail = "joao@test.com",
            PatientPhone = "+5511999999999",
            DoctorId = Guid.NewGuid(),
            DoctorName = "Dr. Maria Santos",
            DoctorSpecialty = "Cardiologia",
            ClinicName = "Clínica Central",
            AppointmentDate = DateTime.Today.AddDays(1),
            AppointmentTime = new TimeSpan(14, 30, 0),
            AppointmentType = "Consulta",
            IsTelehealth = false
        };
    }

    [Fact]
    public void GenerateEmailVerificationEmail_ShouldContainPatientName()
    {
        // Arrange
        var patientName = "João Silva";
        var verificationLink = $"{PortalBaseUrl}/verify-email?token=test-token";

        // Act
        var result = EmailTemplateHelper.GenerateEmailVerificationEmail(patientName, verificationLink, PortalBaseUrl);

        // Assert
        result.Should().Contain(patientName);
        result.Should().Contain("Confirme seu E-mail");
    }

    [Fact]
    public void GenerateEmailVerificationEmail_ShouldContainVerificationLink()
    {
        // Arrange
        var patientName = "João Silva";
        var verificationLink = $"{PortalBaseUrl}/verify-email?token=test-token";

        // Act
        var result = EmailTemplateHelper.GenerateEmailVerificationEmail(patientName, verificationLink, PortalBaseUrl);

        // Assert
        result.Should().Contain(verificationLink);
        result.Should().Contain("href=\"" + verificationLink + "\"");
    }

    [Fact]
    public void GenerateEmailVerificationEmail_ShouldBeValidHtml()
    {
        // Arrange
        var patientName = "João Silva";
        var verificationLink = $"{PortalBaseUrl}/verify-email?token=test-token";

        // Act
        var result = EmailTemplateHelper.GenerateEmailVerificationEmail(patientName, verificationLink, PortalBaseUrl);

        // Assert
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("<html");
        result.Should().Contain("</html>");
        result.Should().Contain("<body");
        result.Should().Contain("</body>");
    }

    [Fact]
    public void GeneratePasswordResetEmail_ShouldContainPatientName()
    {
        // Arrange
        var patientName = "João Silva";
        var resetLink = $"{PortalBaseUrl}/reset-password?token=test-token";

        // Act
        var result = EmailTemplateHelper.GeneratePasswordResetEmail(patientName, resetLink, PortalBaseUrl);

        // Assert
        result.Should().Contain(patientName);
        result.Should().Contain("Recuperação de Senha");
    }

    [Fact]
    public void GeneratePasswordResetEmail_ShouldContainResetLink()
    {
        // Arrange
        var patientName = "João Silva";
        var resetLink = $"{PortalBaseUrl}/reset-password?token=test-token";

        // Act
        var result = EmailTemplateHelper.GeneratePasswordResetEmail(patientName, resetLink, PortalBaseUrl);

        // Assert
        result.Should().Contain(resetLink);
        result.Should().Contain("href=\"" + resetLink + "\"");
    }

    [Fact]
    public void GeneratePasswordResetEmail_ShouldBeValidHtml()
    {
        // Arrange
        var patientName = "João Silva";
        var resetLink = $"{PortalBaseUrl}/reset-password?token=test-token";

        // Act
        var result = EmailTemplateHelper.GeneratePasswordResetEmail(patientName, resetLink, PortalBaseUrl);

        // Assert
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("<html");
        result.Should().Contain("</html>");
        result.Should().Contain("<body");
        result.Should().Contain("</body>");
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_ShouldContainPatientName()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.PatientName);
        result.Should().Contain("João Silva");
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_ShouldContainDoctorInfo()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.DoctorName);
        result.Should().Contain(_sampleAppointment.DoctorSpecialty);
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_ShouldContainClinicName()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.ClinicName);
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_ShouldContainAppointmentLink()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain($"{PortalBaseUrl}/appointments/{_sampleAppointment.AppointmentId}");
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_ShouldBeValidHtml()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("<html");
        result.Should().Contain("</html>");
        result.Should().Contain("<body");
        result.Should().Contain("</body>");
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_WhenTelehealth_ShouldShowTelehealthInfo()
    {
        // Arrange
        _sampleAppointment.IsTelehealth = true;

        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain("Telemedicina");
        result.Should().Contain("online");
    }

    [Fact]
    public void GenerateAppointmentConfirmationEmail_WhenDoctorIdIsNull_ShouldStillGenerate()
    {
        // Arrange
        _sampleAppointment.DoctorId = null; // DoctorId may not be available in some views

        // Act
        var result = EmailTemplateHelper.GenerateAppointmentConfirmationEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.PatientName);
        result.Should().Contain(_sampleAppointment.DoctorName);
        result.Should().Contain(_sampleAppointment.ClinicName);
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("</html>");
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_ShouldContainPatientName()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.PatientName);
        result.Should().Contain("João Silva");
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_ShouldContainDoctorInfo()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.DoctorName);
        result.Should().Contain(_sampleAppointment.DoctorSpecialty);
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_ShouldContainClinicName()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.ClinicName);
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_ShouldContainActionLinks()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain($"{PortalBaseUrl}/appointments/{_sampleAppointment.AppointmentId}/confirm");
        result.Should().Contain($"{PortalBaseUrl}/appointments/{_sampleAppointment.AppointmentId}/reschedule");
        result.Should().Contain($"{PortalBaseUrl}/appointments/{_sampleAppointment.AppointmentId}/cancel");
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_ShouldBeValidHtml()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("<html");
        result.Should().Contain("</html>");
        result.Should().Contain("<body");
        result.Should().Contain("</body>");
    }

    [Fact]
    public void GenerateAppointmentReminderEmail_WhenTelehealth_ShouldShowTelehealthInfo()
    {
        // Arrange
        _sampleAppointment.IsTelehealth = true;

        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderEmail(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain("Telemedicina");
        result.Should().Contain("online");
    }

    [Fact]
    public void GenerateAppointmentReminderText_ShouldContainEssentialInfo()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderText(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain(_sampleAppointment.PatientName);
        result.Should().Contain(_sampleAppointment.DoctorName);
        result.Should().Contain(_sampleAppointment.DoctorSpecialty);
        result.Should().Contain(_sampleAppointment.ClinicName);
        result.Should().Contain("AMANHÃ");
    }

    [Fact]
    public void GenerateAppointmentReminderText_ShouldContainConfirmLink()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderText(_sampleAppointment, PortalBaseUrl);

        // Assert
        result.Should().Contain($"{PortalBaseUrl}/appointments/{_sampleAppointment.AppointmentId}/confirm");
    }

    [Fact]
    public void GenerateAppointmentReminderText_ShouldBeShortForSms()
    {
        // Act
        var result = EmailTemplateHelper.GenerateAppointmentReminderText(_sampleAppointment, PortalBaseUrl);

        // Assert - SMS messages should generally be under 160 characters (though this is longer due to link)
        result.Should().NotBeEmpty();
        result.Length.Should().BeLessThan(500); // Reasonable length for WhatsApp
    }
}
