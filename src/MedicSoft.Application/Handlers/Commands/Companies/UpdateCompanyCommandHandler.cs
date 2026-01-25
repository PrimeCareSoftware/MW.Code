using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Companies;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Companies
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.CompanyId, request.TenantId);
            if (company == null)
            {
                throw new InvalidOperationException("Company not found");
            }

            company.UpdateInfo(
                request.Company.Name,
                request.Company.TradeName,
                request.Company.Phone,
                request.Company.Email
            );

            if (request.Company.Subdomain != company.Subdomain)
            {
                company.SetSubdomain(request.Company.Subdomain);
            }

            await _companyRepository.UpdateAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }
    }
}
