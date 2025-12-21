using System;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly ITicketService _ticketService;

        public TicketServiceTests()
        {
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _ticketService = new TicketService(_mockTicketRepository.Object, _mockOwnerRepository.Object);
        }

        [Fact]
        public async Task CreateTicketAsync_ShouldCreateTicketSuccessfully()
        {
            // Arrange
            var request = new CreateTicketRequest
            {
                Title = "Test Ticket",
                Description = "Test Description",
                Type = TicketType.BugReport,
                Priority = TicketPriority.High
            };

            var userId = Guid.NewGuid();
            var userName = "Test User";
            var userEmail = "test@example.com";
            var clinicId = Guid.NewGuid();
            var clinicName = "Test Clinic";
            var tenantId = "test-tenant";

            Ticket? capturedTicket = null;
            _mockTicketRepository.Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t)
                .ReturnsAsync((Ticket t) => t);

            // Act
            var ticketId = await _ticketService.CreateTicketAsync(
                request, userId, userName, userEmail, clinicId, clinicName, tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, ticketId);
            _mockTicketRepository.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
            
            Assert.NotNull(capturedTicket);
            Assert.Equal(request.Title, capturedTicket.Title);
            Assert.Equal(request.Description, capturedTicket.Description);
            Assert.Equal(request.Type, capturedTicket.Type);
            Assert.Equal(request.Priority, capturedTicket.Priority);
            Assert.Equal(userId, capturedTicket.UserId);
            Assert.Equal(userName, capturedTicket.UserName);
            Assert.Equal(userEmail, capturedTicket.UserEmail);
            Assert.Equal(TicketStatus.Open, capturedTicket.Status);
        }

        [Fact]
        public async Task GetTicketByIdAsync_AsOwner_ShouldReturnTicket()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Test Ticket",
                "Test Description",
                TicketType.BugReport,
                TicketPriority.Medium,
                userId,
                "Test User",
                "test@example.com",
                null,
                null,
                tenantId
            );

            _mockTicketRepository.Setup(r => r.GetTicketWithDetailsAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.GetTicketByIdAsync(ticketId, userId, false, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
            Assert.Equal(ticket.Title, result.Title);
            Assert.Equal(ticket.Description, result.Description);
        }

        [Fact]
        public async Task GetTicketByIdAsync_AsNonOwnerNonSystemOwner_ShouldReturnNull()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Test Ticket",
                "Test Description",
                TicketType.BugReport,
                TicketPriority.Medium,
                ownerId, // Different user
                "Owner",
                "owner@example.com",
                null,
                null,
                tenantId
            );

            _mockTicketRepository.Setup(r => r.GetTicketWithDetailsAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.GetTicketByIdAsync(ticketId, differentUserId, false, tenantId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTicketByIdAsync_AsSystemOwner_ShouldReturnTicketRegardlessOfOwnership()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var systemOwnerId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Test Ticket",
                "Test Description",
                TicketType.BugReport,
                TicketPriority.Medium,
                ownerId,
                "Owner",
                "owner@example.com",
                null,
                null,
                tenantId
            );

            _mockTicketRepository.Setup(r => r.GetTicketWithDetailsAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.GetTicketByIdAsync(ticketId, systemOwnerId, true, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
        }

        [Fact]
        public async Task UpdateTicketAsync_AsOwner_ShouldUpdateSuccessfully()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Original Title",
                "Original Description",
                TicketType.BugReport,
                TicketPriority.Low,
                userId,
                "Test User",
                "test@example.com",
                null,
                null,
                tenantId
            );

            var updateRequest = new UpdateTicketRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Priority = TicketPriority.High
            };

            _mockTicketRepository.Setup(r => r.GetByIdAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);
            _mockTicketRepository.Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ticketService.UpdateTicketAsync(ticketId, updateRequest, userId, false, tenantId);

            // Assert
            Assert.True(result);
            _mockTicketRepository.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTicketAsync_AsNonOwner_ShouldReturnFalse()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Original Title",
                "Original Description",
                TicketType.BugReport,
                TicketPriority.Low,
                ownerId,
                "Owner",
                "owner@example.com",
                null,
                null,
                tenantId
            );

            var updateRequest = new UpdateTicketRequest
            {
                Title = "Updated Title"
            };

            _mockTicketRepository.Setup(r => r.GetByIdAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.UpdateTicketAsync(ticketId, updateRequest, differentUserId, false, tenantId);

            // Assert
            Assert.False(result);
            _mockTicketRepository.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldUpdateStatusAndCreateHistoryEntry()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var userName = "Test User";
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Test Ticket",
                "Test Description",
                TicketType.BugReport,
                TicketPriority.Medium,
                userId,
                userName,
                "test@example.com",
                null,
                null,
                tenantId
            );

            var statusUpdateRequest = new UpdateTicketStatusRequest
            {
                Status = TicketStatus.InProgress,
                Comment = "Starting work on this issue"
            };

            _mockTicketRepository.Setup(r => r.GetByIdAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);
            _mockTicketRepository.Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ticketService.UpdateTicketStatusAsync(
                ticketId, statusUpdateRequest, userId, userName, false, tenantId);

            // Assert
            Assert.True(result);
            Assert.Equal(TicketStatus.InProgress, ticket.Status);
            _mockTicketRepository.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldAddCommentToTicket()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var userName = "Test User";
            var tenantId = "test-tenant";

            var ticket = new Ticket(
                "Test Ticket",
                "Test Description",
                TicketType.BugReport,
                TicketPriority.Medium,
                userId,
                userName,
                "test@example.com",
                null,
                null,
                tenantId
            );

            var commentRequest = new AddTicketCommentRequest
            {
                Comment = "This is a test comment",
                IsInternal = false
            };

            _mockTicketRepository.Setup(r => r.GetByIdAsync(ticketId, tenantId))
                .ReturnsAsync(ticket);
            _mockTicketRepository.Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);
            _mockTicketRepository.Setup(r => r.GetTicketCommentsAsync(ticketId, tenantId, true))
                .ReturnsAsync(new[] { new TicketComment(ticketId, commentRequest.Comment, userId, userName, false, false, tenantId) });

            // Act
            var commentId = await _ticketService.AddCommentAsync(
                ticketId, commentRequest, userId, userName, false, tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, commentId);
            _mockTicketRepository.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
        }
    }
}
