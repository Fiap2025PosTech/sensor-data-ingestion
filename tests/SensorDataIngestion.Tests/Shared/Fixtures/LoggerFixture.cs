using Microsoft.Extensions.Logging;

namespace SensorDataIngestion.Tests.Shared.Fixtures;

/// <summary>
/// Base fixture for tests with mock Logger
/// </summary>
public static class LoggerFixture
{
    public static Mock<ILogger<T>> CreateLoggerMock<T>()
    {
        return new Mock<ILogger<T>>();
    }

    public static void VerifyLogCalled<T>(Mock<ILogger<T>> loggerMock, LogLevel level)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
