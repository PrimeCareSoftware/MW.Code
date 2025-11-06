using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class WaitingQueueConfigurationTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesConfiguration()
        {
            // Arrange & Act
            var config = new WaitingQueueConfiguration(
                _clinicId,
                _tenantId,
                QueueDisplayMode.InternalOnly,
                showEstimatedWaitTime: true,
                showPatientNames: true,
                showPriority: false,
                autoRefreshSeconds: 30,
                enableSoundNotifications: true,
                showPosition: true
            );

            // Assert
            Assert.NotEqual(Guid.Empty, config.Id);
            Assert.Equal(_clinicId, config.ClinicId);
            Assert.Equal(QueueDisplayMode.InternalOnly, config.DisplayMode);
            Assert.True(config.ShowEstimatedWaitTime);
            Assert.True(config.ShowPatientNames);
            Assert.False(config.ShowPriority);
            Assert.Equal(30, config.AutoRefreshSeconds);
            Assert.True(config.EnableSoundNotifications);
            Assert.True(config.ShowPosition);
        }

        [Fact]
        public void Constructor_WithDefaultValues_CreatesConfiguration()
        {
            // Arrange & Act
            var config = new WaitingQueueConfiguration(_clinicId, _tenantId);

            // Assert
            Assert.Equal(QueueDisplayMode.InternalOnly, config.DisplayMode);
            Assert.True(config.ShowEstimatedWaitTime);
            Assert.True(config.ShowPatientNames);
            Assert.False(config.ShowPriority);
            Assert.Equal(30, config.AutoRefreshSeconds);
            Assert.True(config.EnableSoundNotifications);
            Assert.True(config.ShowPosition);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new WaitingQueueConfiguration(Guid.Empty, _tenantId));

            Assert.Contains("clínica", exception.Message.ToLower());
        }

        [Fact]
        public void Constructor_WithTooShortAutoRefresh_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new WaitingQueueConfiguration(_clinicId, _tenantId, autoRefreshSeconds: 3));

            Assert.Contains("5 segundos", exception.Message.ToLower());
        }

        [Fact]
        public void UpdateDisplayMode_UpdatesSuccessfully()
        {
            // Arrange
            var config = CreateValidConfiguration();

            // Act
            config.UpdateDisplayMode(QueueDisplayMode.PublicDisplay);

            // Assert
            Assert.Equal(QueueDisplayMode.PublicDisplay, config.DisplayMode);
        }

        [Fact]
        public void UpdateDisplaySettings_WithAllParameters_UpdatesAll()
        {
            // Arrange
            var config = CreateValidConfiguration();

            // Act
            config.UpdateDisplaySettings(
                showEstimatedWaitTime: false,
                showPatientNames: false,
                showPriority: true,
                showPosition: false
            );

            // Assert
            Assert.False(config.ShowEstimatedWaitTime);
            Assert.False(config.ShowPatientNames);
            Assert.True(config.ShowPriority);
            Assert.False(config.ShowPosition);
        }

        [Fact]
        public void UpdateDisplaySettings_WithPartialParameters_UpdatesOnlyProvided()
        {
            // Arrange
            var config = CreateValidConfiguration();
            var originalShowNames = config.ShowPatientNames;

            // Act
            config.UpdateDisplaySettings(showEstimatedWaitTime: false);

            // Assert
            Assert.False(config.ShowEstimatedWaitTime);
            Assert.Equal(originalShowNames, config.ShowPatientNames); // Unchanged
        }

        [Fact]
        public void UpdateAutoRefresh_WithValidSeconds_UpdatesSuccessfully()
        {
            // Arrange
            var config = CreateValidConfiguration();

            // Act
            config.UpdateAutoRefresh(60);

            // Assert
            Assert.Equal(60, config.AutoRefreshSeconds);
        }

        [Fact]
        public void UpdateAutoRefresh_WithTooShortInterval_ThrowsArgumentException()
        {
            // Arrange
            var config = CreateValidConfiguration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                config.UpdateAutoRefresh(3));

            Assert.Contains("5 segundos", exception.Message.ToLower());
        }

        [Fact]
        public void ToggleSoundNotifications_TogglesValue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            var original = config.EnableSoundNotifications;

            // Act
            config.ToggleSoundNotifications();

            // Assert
            Assert.Equal(!original, config.EnableSoundNotifications);

            // Toggle again
            config.ToggleSoundNotifications();
            Assert.Equal(original, config.EnableSoundNotifications);
        }

        [Fact]
        public void IsPublicDisplayEnabled_WhenPublicDisplay_ReturnsTrue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.PublicDisplay);

            // Act & Assert
            Assert.True(config.IsPublicDisplayEnabled());
        }

        [Fact]
        public void IsPublicDisplayEnabled_WhenBoth_ReturnsTrue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.Both);

            // Act & Assert
            Assert.True(config.IsPublicDisplayEnabled());
        }

        [Fact]
        public void IsPublicDisplayEnabled_WhenInternalOnly_ReturnsFalse()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.InternalOnly);

            // Act & Assert
            Assert.False(config.IsPublicDisplayEnabled());
        }

        [Fact]
        public void IsInternalDisplayEnabled_WhenInternalOnly_ReturnsTrue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.InternalOnly);

            // Act & Assert
            Assert.True(config.IsInternalDisplayEnabled());
        }

        [Fact]
        public void IsInternalDisplayEnabled_WhenBoth_ReturnsTrue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.Both);

            // Act & Assert
            Assert.True(config.IsInternalDisplayEnabled());
        }

        [Fact]
        public void IsInternalDisplayEnabled_WhenPublicDisplay_ReturnsFalse()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.UpdateDisplayMode(QueueDisplayMode.PublicDisplay);

            // Act & Assert
            Assert.False(config.IsInternalDisplayEnabled());
        }

        [Fact]
        public void DisplayModes_AllCombinations_WorkCorrectly()
        {
            // Arrange
            var config = CreateValidConfiguration();

            // Test InternalOnly
            config.UpdateDisplayMode(QueueDisplayMode.InternalOnly);
            Assert.True(config.IsInternalDisplayEnabled());
            Assert.False(config.IsPublicDisplayEnabled());

            // Test PublicDisplay
            config.UpdateDisplayMode(QueueDisplayMode.PublicDisplay);
            Assert.False(config.IsInternalDisplayEnabled());
            Assert.True(config.IsPublicDisplayEnabled());

            // Test Both
            config.UpdateDisplayMode(QueueDisplayMode.Both);
            Assert.True(config.IsInternalDisplayEnabled());
            Assert.True(config.IsPublicDisplayEnabled());
        }

        private WaitingQueueConfiguration CreateValidConfiguration()
        {
            return new WaitingQueueConfiguration(_clinicId, _tenantId);
        }
    }
}
