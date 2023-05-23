using Microsoft.Extensions.Logging;
using Moq;

internal static class TestHelpers
{
    //help - https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
    public static Mock<ILogger<T>> VerifyDebugWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage)
    {
        //todo - refactor and remove pragma
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        //todo - refactor and remove pragma
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        return logger;
    }
}