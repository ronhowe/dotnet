using ClassLibrary1;
using Moq;

namespace TestProject1;

internal class MockDateTimeService : IDateTimeService
{
    private readonly bool _even;

    public MockDateTimeService(bool even)
    {
        _even = even;
    }
    public DateTime UtcNow
    {
        get
        {
            long ticks = _even ? (DateTime.UtcNow.Ticks / 2) * 2 : ((DateTime.UtcNow.Ticks / 2) * 2) + 1;
            DateTime dateTime = new(ticks);

            return dateTime;
        }
    } 

    //public static IDateTimeService CreateMockDateTimeService(bool even)
    //{
    //    long ticks = even ? (DateTime.UtcNow.Ticks / 2) * 2 : ((DateTime.UtcNow.Ticks / 2) * 2) + 1;
    //    DateTime dateTime = new(ticks);

    //    var mockDateTimeService = new Mock<IDateTimeService>();
    //    mockDateTimeService.Setup(x => x.Now).Returns(dateTime);

    //    return mockDateTimeService.Object;
    //}
}
