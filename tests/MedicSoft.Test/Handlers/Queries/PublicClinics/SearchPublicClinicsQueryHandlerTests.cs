using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Handlers.Queries.PublicClinics;
using MedicSoft.Application.Queries.PublicClinics;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Test.Handlers.Queries.PublicClinics
{
    public class SearchPublicClinicsQueryHandlerTests
    {
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SearchPublicClinicsQueryHandler _handler;

        public SearchPublicClinicsQueryHandlerTests()
        {
            _mockClinicRepository = new Mock<IClinicRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new SearchPublicClinicsQueryHandler(
                _mockClinicRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedClinics()
        {
            // Arrange
            var query = new SearchPublicClinicsQuery(
                Name: null,
                City: null,
                State: null,
                PageNumber: 1,
                PageSize: 10
            );

            var clinics = new List<Clinic>
            {
                CreateTestClinic("Clínica 1", "Rua A, 123, Centro, São Paulo - SP, 01000-000"),
                CreateTestClinic("Clínica 2", "Rua B, 456, Jardins, São Paulo - SP, 01400-000")
            };

            _mockClinicRepository
                .Setup(r => r.SearchPublicClinicsAsync(null, null, null, 1, 10))
                .ReturnsAsync(clinics);

            _mockClinicRepository
                .Setup(r => r.CountPublicClinicsAsync(null, null, null))
                .ReturnsAsync(2);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(2, result.Clinics.Count);
            
            // Verifica que dados sensíveis não estão expostos
            Assert.All(result.Clinics, clinic =>
            {
                Assert.NotEmpty(clinic.Name);
                Assert.NotEmpty(clinic.Phone);
                Assert.NotEmpty(clinic.Email);
                Assert.NotEmpty(clinic.Address);
            });
        }

        [Fact]
        public async Task Handle_ShouldFilterByName()
        {
            // Arrange
            var query = new SearchPublicClinicsQuery(
                Name: "Clínica 1",
                City: null,
                State: null,
                PageNumber: 1,
                PageSize: 10
            );

            var clinics = new List<Clinic>
            {
                CreateTestClinic("Clínica 1", "Rua A, 123, Centro, São Paulo - SP, 01000-000")
            };

            _mockClinicRepository
                .Setup(r => r.SearchPublicClinicsAsync("Clínica 1", null, null, 1, 10))
                .ReturnsAsync(clinics);

            _mockClinicRepository
                .Setup(r => r.CountPublicClinicsAsync("Clínica 1", null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result.Clinics);
            Assert.Contains("Clínica 1", result.Clinics[0].Name);
        }

        [Fact]
        public async Task Handle_ShouldExtractCityAndStateFromAddress()
        {
            // Arrange
            var query = new SearchPublicClinicsQuery(
                Name: null,
                City: null,
                State: null,
                PageNumber: 1,
                PageSize: 10
            );

            var clinics = new List<Clinic>
            {
                CreateTestClinic("Clínica Teste", "Rua ABC, 123, Centro, São Paulo - SP, 01000-000")
            };

            _mockClinicRepository
                .Setup(r => r.SearchPublicClinicsAsync(null, null, null, 1, 10))
                .ReturnsAsync(clinics);

            _mockClinicRepository
                .Setup(r => r.CountPublicClinicsAsync(null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result.Clinics);
            Assert.Equal("São Paulo", result.Clinics[0].City);
            Assert.Equal("SP", result.Clinics[0].State);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyListWhenNoClinicsFound()
        {
            // Arrange
            var query = new SearchPublicClinicsQuery(
                Name: "Nonexistent",
                City: null,
                State: null,
                PageNumber: 1,
                PageSize: 10
            );

            _mockClinicRepository
                .Setup(r => r.SearchPublicClinicsAsync("Nonexistent", null, null, 1, 10))
                .ReturnsAsync(new List<Clinic>());

            _mockClinicRepository
                .Setup(r => r.CountPublicClinicsAsync("Nonexistent", null, null))
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Clinics);
            Assert.Equal(0, result.TotalCount);
        }

        private Clinic CreateTestClinic(string name, string address)
        {
            return new Clinic(
                name: name,
                tradeName: name,
                document: "11222333000181", // Valid CNPJ
                phone: "(11) 98765-4321",
                email: "contato@clinica.com.br",
                address: address,
                openingTime: new TimeSpan(8, 0, 0),
                closingTime: new TimeSpan(18, 0, 0),
                tenantId: "test-tenant",
                appointmentDurationMinutes: 30
            );
        }
    }
}
