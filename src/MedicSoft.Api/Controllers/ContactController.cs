using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Registration;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for handling contact form submissions
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Submit a contact form message
        /// </summary>
        /// <param name="request">Contact form data</param>
        /// <returns>Success or error message</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactResponseDto>> SendContactMessage([FromBody] ContactRequestDto request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Phone) ||
                    string.IsNullOrWhiteSpace(request.Subject) ||
                    string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new ContactResponseDto
                    {
                        Success = false,
                        Message = "All fields are required"
                    });
                }

                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new ContactResponseDto
                    {
                        Success = false,
                        Message = "Invalid email format"
                    });
                }

                // Log the contact message
                _logger.LogInformation(
                    "Contact form submission - Name: {Name}, Email: {Email}, Subject: {Subject}",
                    request.Name,
                    request.Email,
                    request.Subject
                );

                // In a real implementation, you would:
                // 1. Save to database
                // 2. Send email notification to support team
                // 3. Send confirmation email to user
                // 4. Integrate with CRM system

                // For now, we'll just log and return success
                await Task.CompletedTask;

                return Ok(new ContactResponseDto
                {
                    Success = true,
                    Message = "Thank you for contacting us! We will get back to you soon."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission");
                
                return BadRequest(new ContactResponseDto
                {
                    Success = false,
                    Message = "Failed to send message. Please try again later."
                });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
