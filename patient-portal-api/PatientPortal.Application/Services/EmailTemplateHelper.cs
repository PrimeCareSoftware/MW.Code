using System.Text;
using PatientPortal.Application.DTOs.Appointments;

namespace PatientPortal.Application.Services;

/// <summary>
/// Helper class for generating email templates
/// </summary>
public static class EmailTemplateHelper
{
    /// <summary>
    /// Generates an HTML email for email verification
    /// </summary>
    public static string GenerateEmailVerificationEmail(string patientName, string verificationLink, string portalBaseUrl)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"pt-BR\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Confirme seu E-mail</title>");
        sb.AppendLine(GetEmailStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <div class=\"header\">");
        sb.AppendLine("            <h1>✉️ Confirme seu E-mail</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"content\">");
        sb.AppendLine($"            <p>Olá <strong>{patientName}</strong>,</p>");
        sb.AppendLine("            <p>Bem-vindo ao Portal do Paciente PrimeCare! Para concluir seu cadastro, precisamos confirmar seu endereço de e-mail.</p>");
        sb.AppendLine("            <div class=\"buttons\">");
        sb.AppendLine($"                <a href=\"{verificationLink}\" class=\"btn btn-primary\">✅ Confirmar E-mail</a>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"alert\">");
        sb.AppendLine("                <strong>⚠️ Importante:</strong> Este link de verificação expira em 24 horas.");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <p>Se você não criou uma conta no Portal do Paciente PrimeCare, por favor ignore este e-mail.</p>");
        sb.AppendLine("            <p>Atenciosamente,<br><strong>Equipe PrimeCare</strong></p>");
        sb.AppendLine("        </div>");
        sb.AppendLine(GetEmailFooter());
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates an HTML email for password reset
    /// </summary>
    public static string GeneratePasswordResetEmail(string patientName, string resetLink, string portalBaseUrl)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"pt-BR\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Recuperação de Senha</title>");
        sb.AppendLine(GetEmailStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <div class=\"header\">");
        sb.AppendLine("            <h1>🔒 Recuperação de Senha</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"content\">");
        sb.AppendLine($"            <p>Olá <strong>{patientName}</strong>,</p>");
        sb.AppendLine("            <p>Recebemos uma solicitação para redefinir a senha da sua conta no Portal do Paciente PrimeCare.</p>");
        sb.AppendLine("            <div class=\"buttons\">");
        sb.AppendLine($"                <a href=\"{resetLink}\" class=\"btn btn-primary\">🔑 Redefinir Senha</a>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"alert\">");
        sb.AppendLine("                <strong>⚠️ Importante:</strong> Este link de recuperação expira em 1 hora.");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <p>Se você não solicitou a recuperação de senha, por favor ignore este e-mail. Sua senha permanecerá inalterada.</p>");
        sb.AppendLine("            <p>Atenciosamente,<br><strong>Equipe PrimeCare</strong></p>");
        sb.AppendLine("        </div>");
        sb.AppendLine(GetEmailFooter());
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates an HTML email for appointment confirmation
    /// </summary>
    public static string GenerateAppointmentConfirmationEmail(AppointmentReminderDto appointment, string portalBaseUrl)
    {
        var appointmentDateTime = appointment.AppointmentDate.Add(appointment.AppointmentTime);
        var appointmentUrl = $"{portalBaseUrl}/appointments/{appointment.AppointmentId}";
        
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"pt-BR\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Confirmação de Agendamento</title>");
        sb.AppendLine(GetEmailStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <div class=\"header\">");
        sb.AppendLine("            <h1>✅ Consulta Agendada com Sucesso!</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"content\">");
        sb.AppendLine($"            <p>Olá <strong>{appointment.PatientName}</strong>,</p>");
        sb.AppendLine("            <p>Sua consulta foi agendada com sucesso! Veja os detalhes abaixo:</p>");
        
        sb.AppendLine("            <div class=\"appointment-details\">");
        sb.AppendLine("                <h3 style=\"margin-top: 0; color: #1e40af;\">Detalhes da Consulta</h3>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">📅 Data:</span> {appointmentDateTime:dddd, dd/MM/yyyy}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🕐 Horário:</span> {appointmentDateTime:HH:mm}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">👨‍⚕️ Médico(a):</span> {appointment.DoctorName}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🏥 Especialidade:</span> {appointment.DoctorSpecialty}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🏢 Local:</span> {appointment.ClinicName}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">📋 Tipo:</span> {appointment.AppointmentType}</div>");
        
        if (appointment.IsTelehealth)
        {
            sb.AppendLine("                <div class=\"alert\">");
            sb.AppendLine("                    <strong>💻 Consulta por Telemedicina</strong><br>");
            sb.AppendLine("                    Esta é uma consulta online. O link de acesso será disponibilizado próximo ao horário da consulta.");
            sb.AppendLine("                </div>");
        }
        
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class=\"buttons\">");
        sb.AppendLine($"                <a href=\"{appointmentUrl}\" class=\"btn btn-primary\">📋 Ver Detalhes</a>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class=\"alert\">");
        sb.AppendLine("                <strong>⚠️ Importante:</strong> Você receberá um lembrete 24 horas antes da consulta.");
        if (!appointment.IsTelehealth)
        {
            sb.AppendLine(" Recomendamos chegar com 15 minutos de antecedência.");
        }
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p>Em caso de dúvidas, entre em contato conosco.</p>");
        sb.AppendLine("            <p>Atenciosamente,<br><strong>Equipe PrimeCare</strong></p>");
        sb.AppendLine("        </div>");
        sb.AppendLine(GetEmailFooter());
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates common email styles
    /// </summary>
    private static string GetEmailStyles()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f4f4f4; margin: 0; padding: 0; }");
        sb.AppendLine("        .container { max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
        sb.AppendLine("        .header { background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }");
        sb.AppendLine("        .content { padding: 30px; }");
        sb.AppendLine("        .appointment-details { background-color: #f8fafc; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #2563eb; }");
        sb.AppendLine("        .detail-row { margin: 10px 0; }");
        sb.AppendLine("        .label { font-weight: bold; color: #1e40af; }");
        sb.AppendLine("        .buttons { text-align: center; margin: 30px 0; }");
        sb.AppendLine("        .btn { display: inline-block; padding: 12px 30px; margin: 5px; text-decoration: none; border-radius: 6px; font-weight: bold; }");
        sb.AppendLine("        .btn-primary { background-color: #2563eb; color: white; }");
        sb.AppendLine("        .btn-secondary { background-color: #64748b; color: white; }");
        sb.AppendLine("        .btn-danger { background-color: #dc2626; color: white; }");
        sb.AppendLine("        .footer { background-color: #f8fafc; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; font-size: 12px; color: #64748b; }");
        sb.AppendLine("        .alert { background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0; border-radius: 4px; }");
        sb.AppendLine("    </style>");
        return sb.ToString();
    }

    /// <summary>
    /// Generates common email footer
    /// </summary>
    /// <remarks>
    /// Note: Uses DateTime.Now which may have timezone implications in production.
    /// Consider injecting a time service if precise timezone control is needed.
    /// </remarks>
    private static string GetEmailFooter()
    {
        var sb = new StringBuilder();
        sb.AppendLine("        <div class=\"footer\">");
        sb.AppendLine("            <p>Este é um e-mail automático. Por favor, não responda.</p>");
        sb.AppendLine($"            <p>© {DateTime.Now.Year} PrimeCare Software. Todos os direitos reservados.</p>");
        sb.AppendLine("        </div>");
        return sb.ToString();
    }

    /// <summary>
    /// Generates an HTML email body for appointment reminder
    /// </summary>
    public static string GenerateAppointmentReminderEmail(AppointmentReminderDto appointment, string portalBaseUrl)
    {
        var appointmentDateTime = appointment.AppointmentDate.Add(appointment.AppointmentTime);
        var confirmUrl = $"{portalBaseUrl}/appointments/{appointment.AppointmentId}/confirm";
        var rescheduleUrl = $"{portalBaseUrl}/appointments/{appointment.AppointmentId}/reschedule";
        var cancelUrl = $"{portalBaseUrl}/appointments/{appointment.AppointmentId}/cancel";

        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"pt-BR\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Lembrete de Consulta</title>");
        sb.AppendLine(GetEmailStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <div class=\"header\">");
        sb.AppendLine("            <h1>🔔 Lembrete de Consulta</h1>");
        sb.AppendLine("        </div>");
        sb.AppendLine("        <div class=\"content\">");
        sb.AppendLine($"            <p>Olá <strong>{appointment.PatientName}</strong>,</p>");
        sb.AppendLine("            <p>Este é um lembrete de que você tem uma consulta agendada para <strong>amanhã</strong>.</p>");
        
        sb.AppendLine("            <div class=\"appointment-details\">");
        sb.AppendLine("                <h3 style=\"margin-top: 0; color: #1e40af;\">Detalhes da Consulta</h3>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">📅 Data:</span> {appointmentDateTime:dddd, dd/MM/yyyy}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🕐 Horário:</span> {appointmentDateTime:HH:mm}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">👨‍⚕️ Médico(a):</span> {appointment.DoctorName}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🏥 Especialidade:</span> {appointment.DoctorSpecialty}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">🏢 Local:</span> {appointment.ClinicName}</div>");
        sb.AppendLine($"                <div class=\"detail-row\"><span class=\"label\">📋 Tipo:</span> {appointment.AppointmentType}</div>");
        
        if (appointment.IsTelehealth)
        {
            sb.AppendLine("                <div class=\"alert\">");
            sb.AppendLine("                    <strong>💻 Consulta por Telemedicina</strong><br>");
            sb.AppendLine("                    Esta é uma consulta online. O link de acesso será disponibilizado próximo ao horário da consulta.");
            sb.AppendLine("                </div>");
        }
        
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class=\"buttons\">");
        sb.AppendLine($"                <a href=\"{confirmUrl}\" class=\"btn btn-primary\">✅ Confirmar Presença</a>");
        sb.AppendLine($"                <a href=\"{rescheduleUrl}\" class=\"btn btn-secondary\">📅 Reagendar</a>");
        sb.AppendLine($"                <a href=\"{cancelUrl}\" class=\"btn btn-danger\">❌ Cancelar</a>");
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <div class=\"alert\">");
        sb.AppendLine("                <strong>⚠️ Importante:</strong> Recomendamos chegar com 15 minutos de antecedência.");
        if (!appointment.IsTelehealth)
        {
            sb.AppendLine(" Não se esqueça de trazer seus documentos e carteirinha do convênio (se houver).");
        }
        sb.AppendLine("            </div>");
        
        sb.AppendLine("            <p>Em caso de dúvidas, entre em contato conosco.</p>");
        sb.AppendLine("            <p>Atenciosamente,<br><strong>Equipe PrimeCare</strong></p>");
        sb.AppendLine("        </div>");
        sb.AppendLine(GetEmailFooter());
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates a plain text version for SMS/WhatsApp
    /// </summary>
    public static string GenerateAppointmentReminderText(AppointmentReminderDto appointment, string portalBaseUrl)
    {
        var appointmentDateTime = appointment.AppointmentDate.Add(appointment.AppointmentTime);
        var confirmUrl = $"{portalBaseUrl}/appointments/{appointment.AppointmentId}/confirm";
        
        return $"🔔 Olá {appointment.PatientName}!\n\n" +
               $"Lembrete: Você tem consulta AMANHÃ às {appointmentDateTime:HH:mm}\n" +
               $"👨‍⚕️ {appointment.DoctorName} - {appointment.DoctorSpecialty}\n" +
               $"🏢 {appointment.ClinicName}\n\n" +
               $"Confirme sua presença: {confirmUrl}\n\n" +
               $"Equipe PrimeCare";
    }
}
