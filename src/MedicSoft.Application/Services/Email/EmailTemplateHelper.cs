using System.Net;

namespace MedicSoft.Application.Services.EmailTemplates
{
    /// <summary>
    /// Helper class with pre-built HTML email templates for system notifications.
    /// Use these templates with IEmailService.SendEmailAsync to send well-formatted emails.
    /// </summary>
    public static class EmailTemplateHelper
    {
        /// <summary>
        /// Generates a welcome email body for newly registered clinics.
        /// Includes clinic and owner details, login instructions, and next steps.
        /// </summary>
        public static string GenerateClinicWelcomeEmail(
            string ownerName,
            string clinicName,
            string tenantId,
            string username,
            string appUrl)
        {
            var encodedOwnerName = WebUtility.HtmlEncode(ownerName);
            var encodedClinicName = WebUtility.HtmlEncode(clinicName);
            var encodedTenantId = WebUtility.HtmlEncode(tenantId);
            var encodedUsername = WebUtility.HtmlEncode(username);
            var loginUrl = $"{appUrl}/login?isOwner=true&tenantId={Uri.EscapeDataString(tenantId)}&username={Uri.EscapeDataString(username)}";

            return $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0066cc; color: white; padding: 30px 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .header p {{ margin: 8px 0 0; opacity: 0.9; font-size: 14px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #e0e0e0; }}
        .info-box {{ background-color: white; border: 1px solid #ddd; border-radius: 6px; padding: 20px; margin: 20px 0; }}
        .info-row {{ display: flex; margin-bottom: 10px; }}
        .info-label {{ font-weight: bold; color: #555; min-width: 140px; }}
        .info-value {{ color: #333; }}
        .tenant-id {{ font-family: monospace; font-size: 16px; font-weight: bold; color: #0066cc; background: #e8f0fe; padding: 4px 8px; border-radius: 4px; }}
        .warning-box {{ background-color: #fff8e1; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 0 4px 4px 0; }}
        .steps {{ margin: 20px 0; }}
        .step {{ display: flex; margin-bottom: 15px; align-items: flex-start; }}
        .step-num {{ background-color: #0066cc; color: white; width: 28px; height: 28px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 14px; flex-shrink: 0; margin-right: 12px; margin-top: 2px; }}
        .btn {{ display: inline-block; background-color: #0066cc; color: white; padding: 14px 28px; border-radius: 6px; text-decoration: none; font-weight: bold; font-size: 16px; margin: 10px 0; }}
        .footer {{ background-color: #f0f0f0; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🎉 Bem-vindo ao Omni Care Software!</h1>
            <p>Sua clínica foi cadastrada com sucesso</p>
        </div>
        <div class=""content"">
            <p>Olá, <strong>{encodedOwnerName}</strong>!</p>
            <p>Estamos felizes em informar que o cadastro da sua clínica foi realizado com sucesso. Você já pode acessar o sistema e começar a utilizar todas as funcionalidades.</p>

            <div class=""info-box"">
                <h3 style=""margin-top:0;color:#0066cc;"">📋 Dados do Cadastro</h3>
                <div class=""info-row"">
                    <span class=""info-label"">Clínica:</span>
                    <span class=""info-value"">{encodedClinicName}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Usuário:</span>
                    <span class=""info-value"">{encodedUsername}</span>
                </div>
                <div class=""info-row"">
                    <span class=""info-label"">Identificador (TenantID):</span>
                    <span class=""info-value tenant-id"">{encodedTenantId}</span>
                </div>
            </div>

            <div class=""warning-box"">
                <strong>⚠️ Importante:</strong> Guarde essas informações em local seguro. Você precisará do <strong>TenantID</strong> e do seu <strong>usuário</strong> para fazer login no sistema.
            </div>

            <h3 style=""color:#0066cc;"">🚀 Próximos Passos</h3>
            <div class=""steps"">
                <div class=""step"">
                    <div class=""step-num"">1</div>
                    <div><strong>Acesse sua conta</strong><br>Clique no botão abaixo e faça login como proprietário usando seu usuário e senha.</div>
                </div>
                <div class=""step"">
                    <div class=""step-num"">2</div>
                    <div><strong>Configure usuários e perfis</strong><br>Crie os demais usuários (médicos, secretárias) e configure seus perfis de acesso.</div>
                </div>
                <div class=""step"">
                    <div class=""step-num"">3</div>
                    <div><strong>Configure sua clínica</strong><br>Adicione horários de atendimento e personalize o sistema.</div>
                </div>
                <div class=""step"">
                    <div class=""step-num"">4</div>
                    <div><strong>Comece a atender</strong><br>Cadastre pacientes e agende consultas.</div>
                </div>
            </div>

            <div style=""text-align:center;margin:30px 0;"">
                <a href=""{loginUrl}"" class=""btn"">Fazer Login como Proprietário</a>
            </div>

            <div class=""info-box"">
                <h3 style=""margin-top:0;color:#0066cc;"">🎁 Período de Teste Gratuito</h3>
                <p style=""margin-bottom:0;"">Você tem <strong>15 dias</strong> para testar todas as funcionalidades sem custo. Não é necessário cartão de crédito.</p>
            </div>
        </div>
        <div class=""footer"">
            <p>Precisa de ajuda? Entre em contato: 📧 contato@omnicaresoftware.com</p>
            <p>© 2026 Omni Care Software. Todos os direitos reservados.</p>
            <p style=""font-size:11px;"">Este é um e-mail automático. Por favor, não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Generates a password reset email body with a verification code.
        /// </summary>
        public static string GeneratePasswordResetEmail(string userName, string verificationCode)
        {
            var encodedUserName = WebUtility.HtmlEncode(userName);
            var encodedCode = WebUtility.HtmlEncode(verificationCode);

            return $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0066cc; color: white; padding: 30px 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #e0e0e0; }}
        .code {{ font-size: 36px; font-weight: bold; color: #0066cc; text-align: center; padding: 20px; background-color: white; border-radius: 8px; margin: 20px 0; letter-spacing: 8px; border: 2px dashed #0066cc; }}
        .warning-box {{ background-color: #fff8e1; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
        .footer {{ background-color: #f0f0f0; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔐 Recuperação de Senha</h1>
        </div>
        <div class=""content"">
            <p>Olá, <strong>{encodedUserName}</strong>!</p>
            <p>Você solicitou a recuperação de senha da sua conta Omni Care. Use o código abaixo para continuar:</p>
            <div class=""code"">{encodedCode}</div>
            <p style=""text-align:center;""><strong>Este código é válido por 15 minutos.</strong></p>
            <div class=""warning-box"">
                <strong>⚠️ Atenção:</strong> Se você não solicitou esta recuperação de senha, ignore este e-mail. Sua conta permanece segura.
            </div>
        </div>
        <div class=""footer"">
            <p>© 2026 Omni Care Software. Todos os direitos reservados.</p>
            <p style=""font-size:11px;"">Este é um e-mail automático. Por favor, não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Generates a generic notification email with a title, message and optional action button.
        /// Use this for sending any custom notification email.
        /// </summary>
        public static string GenerateGenericEmail(
            string title,
            string recipientName,
            string message,
            string? actionText = null,
            string? actionUrl = null)
        {
            var encodedTitle = WebUtility.HtmlEncode(title);
            var encodedName = WebUtility.HtmlEncode(recipientName);
            var encodedMessage = WebUtility.HtmlEncode(message).Replace("\n", "<br>");
            var encodedActionText = actionText != null ? WebUtility.HtmlEncode(actionText) : null;

            // Only allow http/https URLs for the action button to prevent XSS via javascript: or data: schemes
            string? safeActionUrl = null;
            if (actionUrl != null && Uri.TryCreate(actionUrl, UriKind.Absolute, out var parsedUri)
                && (parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps))
            {
                safeActionUrl = actionUrl;
            }

            var actionHtml = (encodedActionText != null && safeActionUrl != null)
                ? $@"<div style=""text-align:center;margin:30px 0;"">
                        <a href=""{safeActionUrl}"" style=""display:inline-block;background-color:#0066cc;color:white;padding:14px 28px;border-radius:6px;text-decoration:none;font-weight:bold;font-size:16px;"">{encodedActionText}</a>
                     </div>"
                : string.Empty;

            return $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0066cc; color: white; padding: 30px 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #e0e0e0; }}
        .footer {{ background-color: #f0f0f0; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>{encodedTitle}</h1>
        </div>
        <div class=""content"">
            <p>Olá, <strong>{encodedName}</strong>!</p>
            <p>{encodedMessage}</p>
            {actionHtml}
        </div>
        <div class=""footer"">
            <p>© 2026 Omni Care Software. Todos os direitos reservados.</p>
            <p style=""font-size:11px;"">Este é um e-mail automático. Por favor, não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Generates an email confirmation email body for newly registered clinic owners.
        /// </summary>
        public static string GenerateEmailConfirmationEmail(
            string ownerName,
            string clinicName,
            string confirmationUrl)
        {
            var encodedOwnerName = WebUtility.HtmlEncode(ownerName);
            var encodedClinicName = WebUtility.HtmlEncode(clinicName);

            return $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0066cc; color: white; padding: 30px 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .header p {{ margin: 8px 0 0; opacity: 0.9; font-size: 14px; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #e0e0e0; }}
        .warning-box {{ background-color: #fff8e1; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 0 4px 4px 0; }}
        .btn {{ display: inline-block; background-color: #0066cc; color: white; padding: 14px 28px; border-radius: 6px; text-decoration: none; font-weight: bold; font-size: 16px; margin: 10px 0; }}
        .footer {{ background-color: #f0f0f0; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none; }}
    </style>
</head>
<body>
<div class=""container"">
    <div class=""header"">
        <h1>✉️ Confirme seu E-mail</h1>
        <p>Omni Care Software - Confirmação de Cadastro</p>
    </div>
    <div class=""content"">
        <p>Olá, <strong>{encodedOwnerName}</strong>!</p>
        <p>Obrigado por cadastrar a clínica <strong>{encodedClinicName}</strong> no Omni Care Software.</p>
        <p>Para ativar seu acesso e fazer login, confirme seu endereço de e-mail clicando no botão abaixo:</p>
        <div style=""text-align:center; margin: 30px 0;"">
            <a href=""{confirmationUrl}"" class=""btn"">✅ Confirmar E-mail</a>
        </div>
        <div class=""warning-box"">
            <strong>⚠️ Importante:</strong> Este link é válido por <strong>24 horas</strong>. Após esse prazo, você precisará solicitar um novo link de confirmação.
        </div>
        <p>Se você não realizou este cadastro, ignore este e-mail.</p>
        <p>Se o botão acima não funcionar, copie e cole o link abaixo no seu navegador:</p>
        <p style=""word-break:break-all; font-size:12px; color:#555;"">{confirmationUrl}</p>
    </div>
    <div class=""footer"">
        <p>© {DateTime.UtcNow.Year} Omni Care Software. Todos os direitos reservados.</p>
        <p style=""font-size:11px;"">Este é um e-mail automático. Por favor, não responda.</p>
    </div>
</div>
</body>
</html>";
        }
    }
}
