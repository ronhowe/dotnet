/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using FluentAssertions;
using System.Net;

namespace ConsoleApp1;

public class Program
{
    static void Main(string[] args)
    {
        string host;

        if (args == null || args.Length == 0)
        {
            host = "https://localhost:444/health";
        }
        else
        {
            host = args[0];
        }

        Uri uri = new(host);

        while (true)
        {
            HttpClient client = new();

            try
            {
                client.GetAsync(uri).Result.StatusCode.Should().Be(HttpStatusCode.OK);

                Refresh("OK", uri, ConsoleColor.DarkGreen);
            }
            catch (Exception e)
            {
                Refresh(e.Message, uri, ConsoleColor.DarkRed);
            }
            finally
            {
                client.Dispose();
            }

            Thread.Sleep(1000);
        }
    }

    private static void Refresh(string message, Uri uri, ConsoleColor color)
    {
        Console.BackgroundColor = color;
        Console.Clear();
        Console.WriteLine($"{DateTime.Now} - {uri} - {message}");
    }
}
