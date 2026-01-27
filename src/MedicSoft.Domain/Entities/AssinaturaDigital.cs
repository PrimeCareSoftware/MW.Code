using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a digital signature on a medical document (ICP-Brasil compliant).
    /// CFM 1.821/2007 requires digital signatures for medical records.
    /// </summary>
    public class AssinaturaDigital : BaseEntity
    {
        public Guid DocumentoId { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; }
        
        public Guid MedicoId { get; private set; }
        public User Medico { get; private set; } = null!;
        
        public Guid CertificadoId { get; private set; }
        public CertificadoDigital Certificado { get; private set; } = null!;
        
        public DateTime DataHoraAssinatura { get; private set; }
        public byte[] AssinaturaDigitalBytes { get; private set; }
        public string HashDocumento { get; private set; } // SHA-256
        
        // Timestamping (carimbo de tempo)
        public bool TemTimestamp { get; private set; }
        public DateTime? DataTimestamp { get; private set; }
        public byte[]? TimestampBytes { get; private set; }
        
        // Validação
        public bool Valida { get; private set; }
        public DateTime? DataUltimaValidacao { get; private set; }
        
        // Metadados
        public string LocalAssinatura { get; private set; }
        public string IpAssinatura { get; private set; }

        private AssinaturaDigital()
        {
            // EF Constructor
            AssinaturaDigitalBytes = null!;
            HashDocumento = null!;
            LocalAssinatura = null!;
            IpAssinatura = null!;
        }

        public AssinaturaDigital(
            Guid documentoId,
            TipoDocumento tipoDocumento,
            Guid medicoId,
            Guid certificadoId,
            byte[] assinaturaDigitalBytes,
            string hashDocumento,
            string localAssinatura,
            string ipAssinatura,
            string tenantId,
            bool temTimestamp = false,
            DateTime? dataTimestamp = null,
            byte[]? timestampBytes = null) : base(tenantId)
        {
            if (documentoId == Guid.Empty)
                throw new ArgumentException("Document ID cannot be empty", nameof(documentoId));
            
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Medico ID cannot be empty", nameof(medicoId));
            
            if (certificadoId == Guid.Empty)
                throw new ArgumentException("Certificate ID cannot be empty", nameof(certificadoId));
            
            if (assinaturaDigitalBytes == null || assinaturaDigitalBytes.Length == 0)
                throw new ArgumentException("Digital signature bytes cannot be empty", nameof(assinaturaDigitalBytes));
            
            if (string.IsNullOrWhiteSpace(hashDocumento))
                throw new ArgumentException("Document hash is required", nameof(hashDocumento));
            
            if (string.IsNullOrWhiteSpace(localAssinatura))
                throw new ArgumentException("Signature location is required", nameof(localAssinatura));
            
            if (string.IsNullOrWhiteSpace(ipAssinatura))
                throw new ArgumentException("IP address is required", nameof(ipAssinatura));

            if (temTimestamp && (dataTimestamp == null || timestampBytes == null))
                throw new ArgumentException("Timestamp data is required when timestamp is enabled");

            DocumentoId = documentoId;
            TipoDocumento = tipoDocumento;
            MedicoId = medicoId;
            CertificadoId = certificadoId;
            AssinaturaDigitalBytes = assinaturaDigitalBytes;
            HashDocumento = hashDocumento.Trim();
            DataHoraAssinatura = DateTime.UtcNow;
            TemTimestamp = temTimestamp;
            DataTimestamp = dataTimestamp;
            TimestampBytes = timestampBytes;
            LocalAssinatura = localAssinatura.Trim();
            IpAssinatura = ipAssinatura.Trim();
            Valida = true;
        }

        public void AtualizarValidacao(bool valida)
        {
            Valida = valida;
            DataUltimaValidacao = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Invalidar()
        {
            Valida = false;
            DataUltimaValidacao = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Types of documents that can be digitally signed.
    /// </summary>
    public enum TipoDocumento
    {
        /// <summary>
        /// Medical record (prontuário médico)
        /// </summary>
        Prontuario = 1,
        
        /// <summary>
        /// Medical prescription (receita médica)
        /// </summary>
        Receita = 2,
        
        /// <summary>
        /// Medical certificate (atestado médico)
        /// </summary>
        Atestado = 3,
        
        /// <summary>
        /// Medical report (laudo médico)
        /// </summary>
        Laudo = 4,
        
        /// <summary>
        /// Medical prescription (prescrição médica)
        /// </summary>
        Prescricao = 5,
        
        /// <summary>
        /// Medical referral (encaminhamento médico)
        /// </summary>
        Encaminhamento = 6
    }
}
