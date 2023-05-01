namespace ClassLibrary1;

public class MockException : Exception
{
    public MockException(string configKey)
        : base($"{configKey}")
    {
    }
}
