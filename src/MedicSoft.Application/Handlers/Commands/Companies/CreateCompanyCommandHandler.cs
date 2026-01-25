using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Companies;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Companies
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = new Company(
                request.Company.Name,
                request.Company.TradeName,
                request.Company.Document,
                request.Company.DocumentType,
                request.Company.Phone,
                request.Company.Email,
                request.TenantId
            );

            if (!string.IsNullOrWhiteSpace(request.Company.Subdomain))
            {
                company.SetSubdomain(request.Company.Subdomain);
            }

            var createdCompany = await _companyRepository.AddAsync(company);
            return _mapper.Map<CompanyDto>(createdCompany);
        }
    }
}
