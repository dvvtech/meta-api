using MetaApi.Core.Domain.UserTryOnLimit;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Services;
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
                          .ReturnsAsync(UserTryOnLimit.Create(accountId: 0,
                                                              maxAttempts: 0,
                                                              attemptsUsed: 0,
                                                              lastResetTime: new DateTime(2023, 10, 9, 12, 0, 0),
                                                              resetPeriod: TimeSpan.FromHours(24)));                          

            var service = new TryOnLimitService(mockRepository.Object, mockClock.Object);

            // Act
            var result = await service.GetTimeUntilLimitResetAsync(1);

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }
    }
}
