using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.PublicClinics;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.PublicClinics
{
    /// <summary>
    /// Handler para busca pública de clínicas.
    /// Retorna apenas informações públicas, sem dados sensíveis (LGPD compliant).
    /// </summary>
    public class SearchPublicClinicsQueryHandler : IRequestHandler<SearchPublicClinicsQuery, SearchClinicsResultDto>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;

        public SearchPublicClinicsQueryHandler(
            IClinicRepository clinicRepository,
            IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<SearchClinicsResultDto> Handle(SearchPublicClinicsQuery request, CancellationToken cancellationToken)
        {
            // Busca todas as clínicas ativas usando o método do repositório
            var clinics = await _clinicRepository.SearchPublicClinicsAsync(
                request.Name,
                request.City,
                request.State,
                request.ClinicType,
                request.PageNumber,
                request.PageSize
            );

            var totalCount = await _clinicRepository.CountPublicClinicsAsync(
                request.Name,
                request.City,
                request.State,
                request.ClinicType
            );

            // Mapeia para DTO público (apenas dados essenciais)
            var publicClinics = clinics.Select(c => new PublicClinicDto
            {
                Id = c.Id,
                Name = c.Name,
                TradeName = c.TradeName,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                City = ExtractCity(c.Address),
                State = ExtractState(c.Address),
                OpeningTime = c.OpeningTime,
                ClosingTime = c.ClosingTime,
                AppointmentDurationMinutes = c.AppointmentDurationMinutes,
                IsAcceptingNewPatients = c.IsActive && c.AllowEmergencySlots,
                ClinicType = c.ClinicType.ToString(),
                WhatsAppNumber = c.WhatsAppNumber
            }).ToList();

            return new SearchClinicsResultDto
            {
                Clinics = publicClinics,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        /// <summary>
        /// Extrai a cidade do endereço completo.
        /// Formato esperado: "Rua, Número, Bairro, Cidade - Estado, CEP"
        /// </summary>
        private string ExtractCity(string address)
        {
            try
            {
                // Procura por padrão "Cidade - Estado"
                var parts = address.Split(',');
                if (parts.Length >= 3)
                {
                    var cityStatePart = parts[^2].Trim(); // Penúltima parte
                    var cityParts = cityStatePart.Split('-');
                    if (cityParts.Length >= 1)
                    {
                        return cityParts[0].Trim();
                    }
                }
            }
            catch
            {
                // Em caso de erro, retorna vazio
            }
            return string.Empty;
        }

        /// <summary>
        /// Extrai o estado do endereço completo.
        /// Formato esperado: "Rua, Número, Bairro, Cidade - Estado, CEP"
        /// </summary>
        private string ExtractState(string address)
        {
            try
            {
                // Procura por padrão "Cidade - Estado"
                var parts = address.Split(',');
                if (parts.Length >= 3)
                {
                    var cityStatePart = parts[^2].Trim(); // Penúltima parte
                    var cityParts = cityStatePart.Split('-');
                    if (cityParts.Length >= 2)
                    {
                        return cityParts[1].Trim();
                    }
                }
            }
            catch
            {
                // Em caso de erro, retorna vazio
            }
            return string.Empty;
        }
    }
}
