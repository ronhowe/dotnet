/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

namespace ClassLibrary1;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
