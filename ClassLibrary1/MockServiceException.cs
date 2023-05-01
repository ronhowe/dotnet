namespace ClassLibrary1;

public class MockServiceException : Exception
{
    public MockServiceException(string message)
        : base($"{message}")
    {
    }
}
