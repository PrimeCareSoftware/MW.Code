using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            // Busca todas as clínicas ativas (sem filtro de tenant, pois é público)
            var query = _clinicRepository.GetAllQueryable()
                .Where(c => c.IsActive);

            // Filtros opcionais
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var searchTerm = request.Name.ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) || 
                    c.TradeName.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var citySearch = request.City.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(citySearch));
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                var stateSearch = request.State.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(stateSearch));
            }

            // Conta total para paginação
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplica paginação
            var clinics = await query
                .OrderBy(c => c.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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
                IsAcceptingNewPatients = c.IsActive && c.AllowEmergencySlots
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
