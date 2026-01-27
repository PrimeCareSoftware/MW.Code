using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Configuração de webservice e integração TISS por operadora
    /// </summary>
    public class TissOperadoraConfig : BaseEntity
    {
        public Guid OperatorId { get; private set; }
        public HealthInsuranceOperator? Operator { get; private set; }
        
        // Webservice configuration
        public string WebServiceUrl { get; private set; }
        public string? Usuario { get; private set; }
        public string? SenhaEncriptada { get; private set; }
        public string? CertificadoDigitalPath { get; private set; } // A1/A3
        
        // Configurações específicas
        public int TimeoutSegundos { get; private set; } = 120;
        public int TentativasReenvio { get; private set; } = 3;
        public bool UsaSoapHeader { get; private set; }
        public bool UsaCertificadoDigital { get; private set; }
        public bool IsActive { get; private set; } = true;
        
        // Mapeamento de códigos específicos (JSON serializado)
        public string? MapeamentoTabelasJson { get; private set; }
        
        private TissOperadoraConfig() 
        { 
            // EF Constructor
            WebServiceUrl = null!;
        }

        public TissOperadoraConfig(
            Guid operatorId,
            string webServiceUrl,
            string tenantId,
            string? usuario = null,
            string? senhaEncriptada = null) : base(tenantId)
        {
            if (operatorId == Guid.Empty)
                throw new ArgumentException("Operator ID cannot be empty", nameof(operatorId));
            
            if (string.IsNullOrWhiteSpace(webServiceUrl))
                throw new ArgumentException("WebService URL cannot be empty", nameof(webServiceUrl));

            OperatorId = operatorId;
            WebServiceUrl = webServiceUrl.Trim();
            Usuario = usuario?.Trim();
            SenhaEncriptada = senhaEncriptada?.Trim();
        }

        public void UpdateCredentials(string? usuario, string? senhaEncriptada)
        {
            Usuario = usuario?.Trim();
            SenhaEncriptada = senhaEncriptada?.Trim();
            UpdateTimestamp();
        }

        public void ConfigureCertificadoDigital(bool usaCertificado, string? certificadoPath = null)
        {
            UsaCertificadoDigital = usaCertificado;
            CertificadoDigitalPath = certificadoPath?.Trim();
            UpdateTimestamp();
        }

        public void ConfigureRetryPolicy(int timeoutSegundos, int tentativasReenvio)
        {
            if (timeoutSegundos <= 0)
                throw new ArgumentException("Timeout must be positive", nameof(timeoutSegundos));
            
            if (tentativasReenvio < 0)
                throw new ArgumentException("Retry attempts cannot be negative", nameof(tentativasReenvio));

            TimeoutSegundos = timeoutSegundos;
            TentativasReenvio = tentativasReenvio;
            UpdateTimestamp();
        }

        public void SetUsaSoapHeader(bool usaSoapHeader)
        {
            UsaSoapHeader = usaSoapHeader;
            UpdateTimestamp();
        }

        public void SetMapeamentoTabelas(string? mapeamentoJson)
        {
            MapeamentoTabelasJson = mapeamentoJson?.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
