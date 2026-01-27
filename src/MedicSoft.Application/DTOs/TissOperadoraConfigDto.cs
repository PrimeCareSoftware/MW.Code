using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO representing TISS webservice configuration for an operator
    /// </summary>
    public class TissOperadoraConfigDto
    {
        public Guid Id { get; set; }
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public string WebServiceUrl { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public int TimeoutSegundos { get; set; }
        public int TentativasReenvio { get; set; }
        public bool UsaSoapHeader { get; set; }
        public bool UsaCertificadoDigital { get; set; }
        public string? CertificadoDigitalPath { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating/updating operator configuration
    /// </summary>
    public class CreateTissOperadoraConfigDto
    {
        public Guid OperatorId { get; set; }
        public string WebServiceUrl { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public string? Senha { get; set; }
        public int TimeoutSegundos { get; set; } = 120;
        public int TentativasReenvio { get; set; } = 3;
        public bool UsaSoapHeader { get; set; }
        public bool UsaCertificadoDigital { get; set; }
        public string? CertificadoDigitalPath { get; set; }
    }
}
