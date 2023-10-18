/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

namespace ClassLibrary1;

public class MockService1Exception : Exception
{
    public MockService1Exception(string message)
        : base($"{message}")
    {
    }
}
