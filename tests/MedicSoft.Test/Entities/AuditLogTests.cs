using System;
using System.Collections.Generic;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class AuditLogTests
    {
        [Fact]
        public void Constructor_ShouldCreateAuditLogSuccessfully()
        {
            // Arrange
            var userId = "user123";
            var userName = "John Doe";
            var userEmail = "john@example.com";
            var action = AuditAction.CREATE;
            var actionDescription = "Created a new record";
            var entityType = "Patient";
            var entityId = Guid.NewGuid().ToString();
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var requestPath = "/api/patients";
            var httpMethod = "POST";
            var result = OperationResult.SUCCESS;
            var dataCategory = DataCategory.SENSITIVE;
            var purpose = LgpdPurpose.HEALTHCARE;
            var severity = AuditSeverity.INFO;
            var tenantId = "tenant123";

            // Act
            var auditLog = new AuditLog(
                userId,
                userName,
                userEmail,
                action,
                actionDescription,
                entityType,
                entityId,
                ipAddress,
                userAgent,
                requestPath,
                httpMethod,
                result,
                dataCategory,
                purpose,
                severity,
                tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, auditLog.Id);
            Assert.Equal(userId, auditLog.UserId);
            Assert.Equal(userName, auditLog.UserName);
            Assert.Equal(userEmail, auditLog.UserEmail);
            Assert.Equal(action, auditLog.Action);
            Assert.Equal(actionDescription, auditLog.ActionDescription);
            Assert.Equal(entityType, auditLog.EntityType);
            Assert.Equal(entityId, auditLog.EntityId);
            Assert.Equal(ipAddress, auditLog.IpAddress);
            Assert.Equal(userAgent, auditLog.UserAgent);
            Assert.Equal(requestPath, auditLog.RequestPath);
            Assert.Equal(httpMethod, auditLog.HttpMethod);
            Assert.Equal(result, auditLog.Result);
            Assert.Equal(dataCategory, auditLog.DataCategory);
            Assert.Equal(purpose, auditLog.Purpose);
            Assert.Equal(severity, auditLog.Severity);
            Assert.Equal(tenantId, auditLog.TenantId);
            Assert.True(auditLog.Timestamp <= DateTime.UtcNow);
        }

        [Fact]
        public void SetOldValues_ShouldUpdateOldValues()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var oldValues = "{\"name\": \"Old Name\"}";

            // Act
            auditLog.SetOldValues(oldValues);

            // Assert
            Assert.Equal(oldValues, auditLog.OldValues);
        }

        [Fact]
        public void SetNewValues_ShouldUpdateNewValues()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var newValues = "{\"name\": \"New Name\"}";

            // Act
            auditLog.SetNewValues(newValues);

            // Assert
            Assert.Equal(newValues, auditLog.NewValues);
        }

        [Fact]
        public void SetChangedFields_ShouldUpdateChangedFields()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var changedFields = new List<string> { "name", "email" };

            // Act
            auditLog.SetChangedFields(changedFields);

            // Assert
            Assert.Equal(changedFields, auditLog.ChangedFields);
        }

        [Fact]
        public void SetFailureReason_ShouldUpdateFailureReason()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var failureReason = "Authentication failed";

            // Act
            auditLog.SetFailureReason(failureReason);

            // Assert
            Assert.Equal(failureReason, auditLog.FailureReason);
        }

        [Fact]
        public void SetStatusCode_ShouldUpdateStatusCode()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var statusCode = 403;

            // Act
            auditLog.SetStatusCode(statusCode);

            // Assert
            Assert.Equal(statusCode, auditLog.StatusCode);
        }

        [Fact]
        public void SetEntityDisplayName_ShouldUpdateEntityDisplayName()
        {
            // Arrange
            var auditLog = CreateSampleAuditLog();
            var displayName = "John Doe - Patient #123";

            // Act
            auditLog.SetEntityDisplayName(displayName);

            // Assert
            Assert.Equal(displayName, auditLog.EntityDisplayName);
        }

        [Theory]
        [InlineData(null)]
        public void Constructor_WithNullRequiredField_ShouldThrowArgumentNullException(string? nullValue)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AuditLog(
                nullValue!,  // userId
                "userName",
                "email",
                AuditAction.CREATE,
                "action",
                "entityType",
                "entityId",
                "ip",
                "userAgent",
                "path",
                "method",
                OperationResult.SUCCESS,
                DataCategory.PERSONAL,
                LgpdPurpose.HEALTHCARE,
                AuditSeverity.INFO,
                "tenant"
            ));
        }

        private AuditLog CreateSampleAuditLog()
        {
            return new AuditLog(
                "user123",
                "John Doe",
                "john@example.com",
                AuditAction.READ,
                "Read patient data",
                "Patient",
                Guid.NewGuid().ToString(),
                "192.168.1.1",
                "Mozilla/5.0",
                "/api/patients/123",
                "GET",
                OperationResult.SUCCESS,
                DataCategory.SENSITIVE,
                LgpdPurpose.HEALTHCARE,
                AuditSeverity.INFO,
                "tenant123"
            );
        }
    }
}
