using MetaApi.Core.Time;
using MetaApi.Services;
using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Repositories;
using Moq;

namespace MetaApi.UnitTests
{
    public class TryOnLimitServiceTests
    {
        [Fact]
        public async Task GetTimeUntilLimitResetAsync_ReturnsZero_WhenResetTimeHasPassed()
        {
            // Arrange
            var mockClock = new Mock<ISystemTime>();
            mockClock.Setup(c => c.UtcNow).Returns(new DateTime(2023, 10, 10, 12, 0, 0));

            var mockRepository = new Mock<ITryOnLimitRepository>();
            mockRepository.Setup(r => r.GetLimit(It.IsAny<int>()))
                          .ReturnsAsync(new UserTryOnLimitEntity
                          {
                              LastResetTime = new DateTime(2023, 10, 9, 12, 0, 0),
                              ResetPeriod = TimeSpan.FromHours(24)
                          });

            var service = new TryOnLimitService(mockRepository.Object, mockClock.Object);

            // Act
            var result = await service.GetTimeUntilLimitResetAsync(1);

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }
    }
}
