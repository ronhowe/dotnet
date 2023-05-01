namespace ClassLibrary1;

public class ChaosException : Exception
{
    public const string KeyName = "ChaosExceptionEnabled";

    public ChaosException(string message)
        : base($"{message}")
    {
    }
}
