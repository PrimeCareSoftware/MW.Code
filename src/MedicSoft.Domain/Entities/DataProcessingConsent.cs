using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Entidade para registro de consentimento de tratamento de dados
    /// Compliance com LGPD (Lei 13.709/2018) - Artigos 7 e 8
    /// </summary>
    public class DataProcessingConsent : BaseEntity
    {
        public string UserId { get; private set; }  // Titular dos dados
        public DateTime ConsentDate { get; private set; }
        public DateTime? RevokedDate { get; private set; }
        public bool IsRevoked { get; private set; }
        
        public LgpdPurpose Purpose { get; private set; }
        public string PurposeDescription { get; private set; }
        
        public List<DataCategory> DataCategories { get; private set; }
        public string ConsentText { get; private set; }
        
        // Evidência
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string ConsentMethod { get; private set; }  // WEB, MOBILE, PAPER

        // Construtor privado para EF Core
        private DataProcessingConsent()
        {
            UserId = null!;
            PurposeDescription = null!;
            DataCategories = null!;
            ConsentText = null!;
            IpAddress = null!;
            UserAgent = null!;
            ConsentMethod = null!;
        }

        public DataProcessingConsent(
            string userId,
            LgpdPurpose purpose,
            string purposeDescription,
            List<DataCategory> dataCategories,
            string consentText,
            string ipAddress,
            string userAgent,
            string consentMethod,
            string tenantId) : base(tenantId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ConsentDate = DateTime.UtcNow;
            IsRevoked = false;
            Purpose = purpose;
            PurposeDescription = purposeDescription ?? throw new ArgumentNullException(nameof(purposeDescription));
            DataCategories = dataCategories ?? throw new ArgumentNullException(nameof(dataCategories));
            ConsentText = consentText ?? throw new ArgumentNullException(nameof(consentText));
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
            ConsentMethod = consentMethod ?? throw new ArgumentNullException(nameof(consentMethod));
        }

        public void Revoke()
        {
            if (IsRevoked)
            {
                throw new InvalidOperationException("Consentimento já foi revogado anteriormente.");
            }

            IsRevoked = true;
            RevokedDate = DateTime.UtcNow;
        }
    }
}
