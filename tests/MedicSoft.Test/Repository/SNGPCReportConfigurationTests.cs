using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Repository
{
    /// <summary>
    /// Tests for SNGPCReport entity configuration in Entity Framework
    /// </summary>
    public class SNGPCReportConfigurationTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly string _testTenantId = "test-tenant";

        public SNGPCReportConfigurationTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"SNGPCReportTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
        }

        [Fact]
        public void DbContext_CanBeCreatedWithSNGPCReportConfiguration()
        {
            // Act - The context is created in the constructor
            // This will throw if the configuration is invalid
            var model = _context.Model;
            
            // Assert - Verify the entity is configured
            var entityType = model.FindEntityType(typeof(SNGPCReport));
            Assert.NotNull(entityType);
            
            // Verify the PrescriptionIds property is configured
            var property = entityType.FindProperty(nameof(SNGPCReport.PrescriptionIds));
            Assert.NotNull(property);
        }

        [Fact]
        public void SNGPCReport_CanBeAddedToContext()
        {
            // Arrange
            var report = new SNGPCReport(1, 2026, _testTenantId);
            var prescriptionId = Guid.NewGuid();
            
            // Act - Add a prescription to trigger the collection
            report.AddPrescription(prescriptionId);
            
            _context.Set<SNGPCReport>().Add(report);
            var result = _context.SaveChanges();
            
            // Assert
            Assert.Equal(1, result);
            Assert.Contains(prescriptionId, report.PrescriptionIds);
        }

        [Fact]
        public void SNGPCReport_PrescriptionIds_CanBeRetrievedFromDatabase()
        {
            // Arrange
            var report = new SNGPCReport(2, 2026, _testTenantId);
            var prescriptionId1 = Guid.NewGuid();
            var prescriptionId2 = Guid.NewGuid();
            
            report.AddPrescription(prescriptionId1);
            report.AddPrescription(prescriptionId2);
            
            _context.Set<SNGPCReport>().Add(report);
            _context.SaveChanges();
            
            // Clear the context to force a fresh read from database
            _context.ChangeTracker.Clear();
            
            // Act
            var retrievedReport = _context.Set<SNGPCReport>()
                .FirstOrDefault(r => r.Id == report.Id);
            
            // Assert
            Assert.NotNull(retrievedReport);
            Assert.Equal(2, retrievedReport.PrescriptionIds.Count);
            Assert.Contains(prescriptionId1, retrievedReport.PrescriptionIds);
            Assert.Contains(prescriptionId2, retrievedReport.PrescriptionIds);
        }

        [Fact]
        public void SNGPCReport_PrescriptionIds_ValueComparerWorks()
        {
            // Arrange
            var report = new SNGPCReport(3, 2026, _testTenantId);
            var prescriptionId1 = Guid.NewGuid();
            
            report.AddPrescription(prescriptionId1);
            _context.Set<SNGPCReport>().Add(report);
            _context.SaveChanges();
            
            // Act - Modify the collection (add another prescription)
            var prescriptionId2 = Guid.NewGuid();
            report.AddPrescription(prescriptionId2);
            
            // The change tracking should detect this change
            var entry = _context.Entry(report);
            var isModified = entry.State == EntityState.Modified;
            
            // Save changes
            var result = _context.SaveChanges();
            
            // Assert - Changes should be detected and saved
            Assert.True(result > 0 || isModified, "Changes to PrescriptionIds collection should be detected");
            
            // Verify the data persisted
            _context.ChangeTracker.Clear();
            var retrievedReport = _context.Set<SNGPCReport>().Find(report.Id);
            Assert.NotNull(retrievedReport);
            Assert.Equal(2, retrievedReport.PrescriptionIds.Count);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
