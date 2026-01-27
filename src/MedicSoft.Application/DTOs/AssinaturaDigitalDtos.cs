using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for CertificadoDigital entity.
    /// </summary>
    public class CertificadoDigitalDto
    {
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public string MedicoNome { get; set; } = null!;
        public string Tipo { get; set; } = null!; // A1 or A3
        public string NumeroCertificado { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string IssuerName { get; set; } = null!;
        public DateTime DataEmissao { get; set; }
        public DateTime DataExpiracao { get; set; }
        public bool Valido { get; set; }
        public int TotalAssinaturas { get; set; }
        public int DiasParaExpiracao { get; set; }
    }

    /// <summary>
    /// DTO for AssinaturaDigital entity.
    /// </summary>
    public class AssinaturaDigitalDto
    {
        public Guid Id { get; set; }
        public Guid DocumentoId { get; set; }
        public string TipoDocumento { get; set; } = null!;
        public Guid MedicoId { get; set; }
        public string MedicoNome { get; set; } = null!;
        public string MedicoCRM { get; set; } = null!;
        public DateTime DataHoraAssinatura { get; set; }
        public string HashDocumento { get; set; } = null!;
        public bool TemTimestamp { get; set; }
        public DateTime? DataTimestamp { get; set; }
        public bool Valida { get; set; }
        public DateTime? DataUltimaValidacao { get; set; }
        public string CertificadoSubject { get; set; } = null!;
        public DateTime CertificadoExpiracao { get; set; }
    }

    /// <summary>
    /// Information about an X.509 certificate.
    /// </summary>
    public class CertificateInfo
    {
        public string Subject { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Thumbprint { get; set; } = null!;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsValid { get; set; }
    }

    /// <summary>
    /// Result of a signature operation.
    /// </summary>
    public class ResultadoAssinatura
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public Guid? AssinaturaId { get; set; }
        public AssinaturaDigitalDto? Assinatura { get; set; }
    }

    /// <summary>
    /// Result of a signature validation.
    /// </summary>
    public class ResultadoValidacao
    {
        public bool Valida { get; set; }
        public string? Motivo { get; set; }
        public DateTime? DataAssinatura { get; set; }
        public string? Assinante { get; set; }
        public string? CRM { get; set; }
        public string? Certificado { get; set; }
    }

    /// <summary>
    /// Timestamp response from TSA.
    /// </summary>
    public class TimestampResponse
    {
        public DateTime Data { get; set; }
        public byte[] Bytes { get; set; } = null!;
        public bool Valido { get; set; }
    }
}
