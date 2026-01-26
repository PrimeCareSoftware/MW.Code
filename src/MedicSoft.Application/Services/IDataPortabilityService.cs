using System;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    public interface IDataPortabilityService
    {
        /// <summary>
        /// Exporta todos os dados de um paciente em formato JSON
        /// </summary>
        Task<string> ExportPatientDataAsJsonAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Exporta todos os dados de um paciente em formato XML
        /// </summary>
        Task<string> ExportPatientDataAsXmlAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Gera PDF com todos os dados de um paciente
        /// </summary>
        Task<byte[]> ExportPatientDataAsPdfAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Cria um pacote completo de dados do paciente (ZIP com múltiplos formatos)
        /// </summary>
        Task<byte[]> CreatePatientDataPackageAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Registra solicitação de portabilidade de dados
        /// </summary>
        Task LogPortabilityRequestAsync(
            Guid patientId,
            string format,
            string ipAddress,
            string userAgent,
            string tenantId);
    }
}
