using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a digital certificate (ICP-Brasil) for signing medical documents.
    /// Supports both A1 (software) and A3 (token/smartcard) certificates.
    /// </summary>
    public class CertificadoDigital : BaseEntity
    {
        public Guid MedicoId { get; private set; }
        public User Medico { get; private set; } = null!;
        
        public TipoCertificado Tipo { get; private set; }
        public string NumeroCertificado { get; private set; }
        public string SubjectName { get; private set; } // CN do certificado
        public string IssuerName { get; private set; }
        public string Thumbprint { get; private set; }
        
        // A1: armazenado criptografado
        public byte[]? CertificadoA1Criptografado { get; private set; }
        public byte[]? ChavePrivadaA1Criptografada { get; private set; }
        
        // Validade
        public DateTime DataEmissao { get; private set; }
        public DateTime DataExpiracao { get; private set; }
        public bool Valido { get; private set; }
        
        // Auditoria
        public DateTime DataCadastro { get; private set; }
        public int TotalAssinaturas { get; private set; }

        private CertificadoDigital()
        {
            // EF Constructor
            NumeroCertificado = null!;
            SubjectName = null!;
            IssuerName = null!;
            Thumbprint = null!;
        }

        public CertificadoDigital(
            Guid medicoId,
            TipoCertificado tipo,
            string numeroCertificado,
            string subjectName,
            string issuerName,
            string thumbprint,
            DateTime dataEmissao,
            DateTime dataExpiracao,
            string tenantId,
            byte[]? certificadoA1Criptografado = null,
            byte[]? chavePrivadaA1Criptografada = null) : base(tenantId)
        {
            if (medicoId == Guid.Empty)
                throw new ArgumentException("Medico ID cannot be empty", nameof(medicoId));
            
            if (string.IsNullOrWhiteSpace(numeroCertificado))
                throw new ArgumentException("Certificate number is required", nameof(numeroCertificado));
            
            if (string.IsNullOrWhiteSpace(subjectName))
                throw new ArgumentException("Subject name is required", nameof(subjectName));
            
            if (string.IsNullOrWhiteSpace(issuerName))
                throw new ArgumentException("Issuer name is required", nameof(issuerName));
            
            if (string.IsNullOrWhiteSpace(thumbprint))
                throw new ArgumentException("Thumbprint is required", nameof(thumbprint));
            
            if (dataExpiracao <= dataEmissao)
                throw new ArgumentException("Expiration date must be after issue date");

            // A1 certificates require encrypted data
            if (tipo == TipoCertificado.A1 && 
                (certificadoA1Criptografado == null || chavePrivadaA1Criptografada == null))
                throw new ArgumentException("A1 certificate requires encrypted certificate and private key data");

            MedicoId = medicoId;
            Tipo = tipo;
            NumeroCertificado = numeroCertificado.Trim();
            SubjectName = subjectName.Trim();
            IssuerName = issuerName.Trim();
            Thumbprint = thumbprint.Trim();
            DataEmissao = dataEmissao;
            DataExpiracao = dataExpiracao;
            CertificadoA1Criptografado = certificadoA1Criptografado;
            ChavePrivadaA1Criptografada = chavePrivadaA1Criptografada;
            Valido = true;
            DataCadastro = DateTime.UtcNow;
            TotalAssinaturas = 0;
        }

        public void IncrementarAssinaturas()
        {
            TotalAssinaturas++;
            UpdateTimestamp();
        }

        public void Invalidar()
        {
            Valido = false;
            UpdateTimestamp();
        }

        public void Revalidar()
        {
            if (DateTime.UtcNow > DataExpiracao)
                throw new InvalidOperationException("Cannot revalidate an expired certificate");
            
            Valido = true;
            UpdateTimestamp();
        }

        public bool IsExpirado()
        {
            return DateTime.UtcNow > DataExpiracao;
        }

        public int DiasParaExpiracao()
        {
            if (IsExpirado())
                return 0;
            
            return (int)(DataExpiracao - DateTime.UtcNow).TotalDays;
        }
    }

    /// <summary>
    /// Types of ICP-Brasil digital certificates.
    /// </summary>
    public enum TipoCertificado
    {
        /// <summary>
        /// A1 certificate - Stored in software (1 year validity)
        /// </summary>
        A1 = 1,
        
        /// <summary>
        /// A3 certificate - Stored in token/smartcard (3-5 years validity)
        /// </summary>
        A3 = 3
    }
}
