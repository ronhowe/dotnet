/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

namespace ClassLibrary1;

public class MockService1Exception : Exception
{
    public MockService1Exception(string message)
        : base($"{message}")
    {
    }
}
