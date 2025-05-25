using Microsoft.Extensions.Logging;
using Moq;

namespace LearningManagementDashboard.Tests.Helpers;

internal static class LoggerMockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> mock,
                                    LogLevel level,
                                    string partialMessage,
                                    Times times)
    {
        if (mock == null)
        {
            throw new ArgumentNullException(nameof(mock));
        }

        mock.Verify(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString().Contains(partialMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}
