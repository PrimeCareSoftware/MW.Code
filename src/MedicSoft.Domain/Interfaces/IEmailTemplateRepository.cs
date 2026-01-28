using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Domain.Interfaces
{
    public interface IEmailTemplateRepository : IRepository<EmailTemplate>
    {
        /// <summary>
        /// Gets an email template by its name.
        /// </summary>
        Task<EmailTemplate?> GetByNameAsync(string name, string tenantId);

        /// <summary>
        /// Gets all active email templates.
        /// </summary>
        Task<IEnumerable<EmailTemplate>> GetAllActiveAsync(string tenantId);

        /// <summary>
        /// Checks if a template with the given name exists.
        /// </summary>
        Task<bool> NameExistsAsync(string name, string tenantId);
    }
}
