using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    public interface IDataDeletionService
    {
        /// <summary>
        /// Cria uma requisição de exclusão/anonimização de dados (direito ao esquecimento)
        /// </summary>
        Task<Guid> RequestDataDeletionAsync(
            Guid patientId,
            string patientName,
            string patientEmail,
            string reason,
            DeletionRequestType requestType,
            string ipAddress,
            string userAgent,
            bool requiresLegalApproval,
            string tenantId);

        /// <summary>
        /// Processa uma requisição de exclusão pendente
        /// </summary>
        Task ProcessDataDeletionRequestAsync(
            Guid requestId,
            string userId,
            string userName,
            string notes,
            string tenantId);

        /// <summary>
        /// Completa uma requisição de exclusão (executa a anonimização/exclusão)
        /// </summary>
        Task CompleteDataDeletionRequestAsync(Guid requestId, string tenantId);

        /// <summary>
        /// Rejeita uma requisição de exclusão
        /// </summary>
        Task RejectDataDeletionRequestAsync(Guid requestId, string reason, string tenantId);

        /// <summary>
        /// Aprova legalmente uma requisição
        /// </summary>
        Task ApproveLegalAsync(Guid requestId, string approver, string tenantId);

        /// <summary>
        /// Obtém requisições pendentes
        /// </summary>
        Task<List<DataDeletionRequest>> GetPendingRequestsAsync(string tenantId);

        /// <summary>
        /// Obtém requisições de um paciente
        /// </summary>
        Task<List<DataDeletionRequest>> GetPatientRequestsAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Anonimiza dados de um paciente (substitui informações identificáveis)
        /// </summary>
        Task AnonymizePatientDataAsync(Guid patientId, string tenantId);
    }
}
