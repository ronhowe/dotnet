namespace ClassLibrary1;

public class MockException : Exception
{
    public const string KeyName = "MockExceptionEnabled";

    public MockException(string message)
        : base($"{message}")
    {
    }
}
