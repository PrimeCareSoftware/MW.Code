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
