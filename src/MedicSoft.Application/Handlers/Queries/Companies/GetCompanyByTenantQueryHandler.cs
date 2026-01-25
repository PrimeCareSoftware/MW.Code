using System.Linq;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Companies;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Companies
{
    public class GetCompanyByTenantQueryHandler : IRequestHandler<GetCompanyByTenantQuery, CompanyDto?>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public GetCompanyByTenantQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto?> Handle(GetCompanyByTenantQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllAsync(request.TenantId);
            var company = companies.FirstOrDefault();
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }
    }
}
