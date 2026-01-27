using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Services.CRM
{
    public class ChurnPredictionServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<ChurnPredictionService>> _mockLogger;
        private readonly IChurnPredictionService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public ChurnPredictionServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<ChurnPredictionService>>();
            _service = new ChurnPredictionService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldPredictChurnForExistingPatient()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patient.Id, result.PatientId);
            Assert.True(result.ChurnProbability >= 0 && result.ChurnProbability <= 1);
            Assert.NotNull(result.RiskLevel);
            Assert.NotNull(result.Factors);
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldThrowException_WhenPatientNotFound()
        {
            // Arrange
            var nonExistentPatientId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.PredictChurnAsync(nonExistentPatientId, _testTenantId));
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldReturnCriticalRisk_ForInactivePatientWithComplaints()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add complaints to trigger high risk
            for (int i = 0; i < 5; i++)
            {
                var complaint = new Complaint(
                    $"CMP-TEST-{i}",
                    patient.Id,
                    $"Subject {i}",
                    $"Complaint {i}",
                    ComplaintCategory.Service,
                    _testTenantId);
                _context.Complaints.Add(complaint);
            }

            // Add no-shows
            for (int i = 0; i < 3; i++)
            {
                var appointment = CreateTestAppointment(patient.Id, AppointmentStatus.NoShow);
                _context.Appointments.Add(appointment);
            }

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ChurnProbability > 0.5); // Should be at least high risk
            Assert.NotEmpty(result.Factors);
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldReturnLowRisk_ForActivePatientWithHighSatisfaction()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add recent completed appointment
            var appointment = CreateTestAppointment(patient.Id, AppointmentStatus.Completed);
            _context.Appointments.Add(appointment);

            // Add high satisfaction survey
            var survey = new SurveyResponse(patient.Id, Guid.NewGuid(), _testTenantId);
            survey.Complete(npsScore: 10);
            _context.SurveyResponses.Add(survey);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ChurnProbability < 0.5); // Should be low to medium risk
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldIdentifyInactivityFactor()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add appointment from more than 90 days ago
            var oldAppointment = CreateTestAppointment(patient.Id, AppointmentStatus.Completed, daysFromNow: -120);
            _context.Appointments.Add(oldAppointment);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Factors);
            Assert.Single(result.Factors, f => f.Name == "Inatividade");
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldIdentifyLowSatisfactionFactor()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add low satisfaction survey
            var survey = new SurveyResponse(patient.Id, Guid.NewGuid(), _testTenantId);
            survey.Complete(npsScore: 4); // Low score
            _context.SurveyResponses.Add(survey);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Factors);
            Assert.Single(result.Factors, f => f.Name == "Baixa Satisfação");
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldIdentifyComplaintsFactor()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add complaints
            for (int i = 0; i < 3; i++)
            {
                var complaint = new Complaint(
                    $"CMP-TEST-{i}",
                    patient.Id,
                    $"Subject {i}",
                    $"Complaint {i}",
                    ComplaintCategory.Service,
                    _testTenantId);
                _context.Complaints.Add(complaint);
            }

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Factors);
            Assert.Single(result.Factors, f => f.Name == "Reclamações");
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldIdentifyNoShowsFactor()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Add no-shows
            for (int i = 0; i < 2; i++)
            {
                var appointment = CreateTestAppointment(patient.Id, AppointmentStatus.NoShow);
                _context.Appointments.Add(appointment);
            }

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Factors);
            Assert.Single(result.Factors, f => f.Name == "No-Shows");
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldSavePredictionToDatabase()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var initialCount = _context.ChurnPredictions.Count();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            var finalCount = _context.ChurnPredictions.Count();
            Assert.Equal(initialCount + 1, finalCount);

            var savedPrediction = await _context.ChurnPredictions
                .FirstOrDefaultAsync(p => p.PatientId == patient.Id && p.TenantId == _testTenantId);
            Assert.NotNull(savedPrediction);
            Assert.Equal(result.ChurnProbability, savedPrediction.ChurnProbability);
            Assert.Equal(result.RiskLevel, savedPrediction.RiskLevel);
        }

        [Fact]
        public async Task GetHighRiskPatientsAsync_ShouldReturnOnlyHighAndCriticalRiskPatients()
        {
            // Arrange
            var highRiskPatient = CreateTestPatient();
            var lowRiskPatient = CreateTestPatient();

            _context.Patients.AddRange(highRiskPatient, lowRiskPatient);
            await _context.SaveChangesAsync();

            // Create predictions with different risk levels
            var highRiskPrediction = new ChurnPrediction(highRiskPatient.Id, _testTenantId);
            highRiskPrediction.SetFeatures(200, 1, 150m, 3.0, 5, 3, 2);
            highRiskPrediction.SetPrediction(0.8, ChurnRiskLevel.High);

            var lowRiskPrediction = new ChurnPrediction(lowRiskPatient.Id, _testTenantId);
            lowRiskPrediction.SetFeatures(10, 20, 3000m, 9.0, 0, 0, 0);
            lowRiskPrediction.SetPrediction(0.2, ChurnRiskLevel.Low);

            _context.ChurnPredictions.AddRange(highRiskPrediction, lowRiskPrediction);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetHighRiskPatientsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Single(resultList);
            Assert.Equal(highRiskPatient.Id, resultList[0].PatientId);
            Assert.True(resultList[0].RiskLevel >= ChurnRiskLevel.High);
        }

        [Fact]
        public async Task GetHighRiskPatientsAsync_ShouldReturnEmpty_WhenNoHighRiskPatients()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var prediction = new ChurnPrediction(patient.Id, _testTenantId);
            prediction.SetFeatures(10, 20, 3000m, 9.0, 0, 0, 0);
            prediction.SetPrediction(0.1, ChurnRiskLevel.Low);
            _context.ChurnPredictions.Add(prediction);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetHighRiskPatientsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetHighRiskPatientsAsync_ShouldOrderByChurnProbabilityDescending()
        {
            // Arrange
            var patient1 = CreateTestPatient();
            var patient2 = CreateTestPatient();

            _context.Patients.AddRange(patient1, patient2);
            await _context.SaveChangesAsync();

            var prediction1 = new ChurnPrediction(patient1.Id, _testTenantId);
            prediction1.SetFeatures(100, 1, 150m, 4.0, 3, 2, 1);
            prediction1.SetPrediction(0.9, ChurnRiskLevel.Critical);

            var prediction2 = new ChurnPrediction(patient2.Id, _testTenantId);
            prediction2.SetFeatures(80, 2, 300m, 5.0, 2, 1, 0);
            prediction2.SetPrediction(0.6, ChurnRiskLevel.High);

            _context.ChurnPredictions.AddRange(prediction1, prediction2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetHighRiskPatientsAsync(_testTenantId);

            // Assert
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.True(resultList[0].ChurnProbability >= resultList[1].ChurnProbability);
        }

        [Fact]
        public async Task GetChurnFactorsAsync_ShouldReturnFactorsForPatient()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Create prediction with risk factors
            var prediction = new ChurnPrediction(patient.Id, _testTenantId);
            prediction.SetFeatures(150, 2, 300m, 5.0, 2, 1, 0);
            prediction.SetPrediction(0.6, ChurnRiskLevel.High);
            prediction.AddRiskFactor("Inatividade");
            prediction.AddRiskFactor("Baixa Satisfação");
            _context.ChurnPredictions.Add(prediction);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetChurnFactorsAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.NotEmpty(resultList.SelectMany(f => f.Description));
        }

        [Fact]
        public async Task GetChurnFactorsAsync_ShouldReturnEmpty_WhenNoPredictionExists()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            // Act
            var results = await _service.GetChurnFactorsAsync(patientId, _testTenantId);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetChurnFactorsAsync_ShouldReturnLatestPredictionFactors()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Create first prediction
            var prediction1 = new ChurnPrediction(patient.Id, _testTenantId);
            prediction1.SetFeatures(50, 10, 1500m, 8.0, 0, 0, 0);
            prediction1.SetPrediction(0.2, ChurnRiskLevel.Low);
            prediction1.AddRiskFactor("Old Factor");
            _context.ChurnPredictions.Add(prediction1);
            await _context.SaveChangesAsync();

            // Create second prediction (newer)
            var prediction2 = new ChurnPrediction(patient.Id, _testTenantId);
            prediction2.SetFeatures(150, 2, 300m, 5.0, 3, 2, 1);
            prediction2.SetPrediction(0.7, ChurnRiskLevel.High);
            prediction2.AddRiskFactor("New Factor 1");
            prediction2.AddRiskFactor("New Factor 2");
            _context.ChurnPredictions.Add(prediction2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetChurnFactorsAsync(patient.Id, _testTenantId);

            // Assert
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, f => Assert.NotNull(f.Description));
        }

        [Fact]
        public async Task RecalculateAllPredictionsAsync_ShouldPredictChurnForAllPatients()
        {
            // Arrange
            var patient1 = CreateTestPatient();
            var patient2 = CreateTestPatient();
            var patient3 = CreateTestPatient();

            _context.Patients.AddRange(patient1, patient2, patient3);
            await _context.SaveChangesAsync();

            var initialCount = _context.ChurnPredictions.Count();

            // Act
            await _service.RecalculateAllPredictionsAsync(_testTenantId);

            // Assert
            var finalCount = _context.ChurnPredictions.Count();
            Assert.Equal(initialCount + 3, finalCount);

            // Verify predictions exist for all patients
            var predictions = await _context.ChurnPredictions
                .Where(p => p.TenantId == _testTenantId)
                .ToListAsync();
            Assert.Equal(3, predictions.Count);
        }

        [Fact]
        public async Task RecalculateAllPredictionsAsync_ShouldHandleFailures_ContinueProcessing()
        {
            // Arrange
            var validPatient = CreateTestPatient();
            _context.Patients.Add(validPatient);
            await _context.SaveChangesAsync();

            // Act - Should not throw even if there are issues
            await _service.RecalculateAllPredictionsAsync(_testTenantId);

            // Assert - Should have processed at least the valid patient
            var predictions = await _context.ChurnPredictions
                .Where(p => p.TenantId == _testTenantId)
                .ToListAsync();
            Assert.NotEmpty(predictions);
        }

        [Fact]
        public async Task RecalculateAllPredictionsAsync_ShouldProcessOnlyCurrentTenant()
        {
            // Arrange
            var tenantAPatient = CreateTestPatient();
            var tenantBPatient = CreateTestPatient(tenantId: "other-tenant");

            _context.Patients.AddRange(tenantAPatient, tenantBPatient);
            await _context.SaveChangesAsync();

            // Act
            await _service.RecalculateAllPredictionsAsync(_testTenantId);

            // Assert
            var predictions = await _context.ChurnPredictions
                .Where(p => p.TenantId == _testTenantId)
                .ToListAsync();
            Assert.Single(predictions);
            Assert.Equal(tenantAPatient.Id, predictions[0].PatientId);
        }

        [Fact]
        public async Task PredictChurnAsync_ShouldGenerateRecommendedActions_ForHighRiskPatient()
        {
            // Arrange
            var patient = CreateTestPatient();
            _context.Patients.Add(patient);

            // Create high-risk scenario with complaints
            var complaint = new Complaint(
                "CMP-TEST-001",
                patient.Id,
                "Test subject",
                "Test complaint",
                ComplaintCategory.Service,
                _testTenantId);
            _context.Complaints.Add(complaint);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PredictChurnAsync(patient.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.RecommendedActions);
            // For a patient with complaints, should recommend actions like "Revisar e resolver reclamações pendentes"
            Assert.Contains(result.RecommendedActions, a => a.Contains("reclamação") || a.Contains("Reclamação") || a.Contains("contato"));
        }

        // Helper methods
        private Patient CreateTestPatient(string tenantId = null)
        {
            tenantId ??= _testTenantId;
            var patient = new Patient(
                "John Doe",
                "12345678900",
                new DateTime(1990, 1, 1),
                "M",
                new Email("john@test.com"),
                new Phone("+55", "11999999999"),
                new Address("Street", "123", "Neighborhood", "City", "State", "12345-678", "Brasil", "Apt 1"),
                tenantId);
            return patient;
        }

        private Appointment CreateTestAppointment(Guid patientId, AppointmentStatus status, int daysFromNow = 1, string tenantId = null)
        {
            tenantId ??= _testTenantId;
            var appointment = new Appointment(
                patientId,
                Guid.NewGuid(), // clinicId
                DateTime.UtcNow.AddDays(daysFromNow),
                TimeSpan.FromHours(10),
                60,
                AppointmentType.Consultation,
                tenantId,
                allowHistoricalData: true);
            
            // Set status by calling appropriate methods
            if (status == AppointmentStatus.NoShow)
            {
                appointment.Confirm();
                appointment.MarkAsNoShow();
            }
            else if (status == AppointmentStatus.Completed)
            {
                appointment.Confirm();
                appointment.CheckIn();
                appointment.CheckOut();
            }
            
            return appointment;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
