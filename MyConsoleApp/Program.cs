using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Shouldly;
using System.Diagnostics;
using System.Net;

namespace MyConsoleApp;

public class Program
{
    private static bool _benchmark;
    private static bool _clear;
    private static bool _color;
    private static bool _noHeaders;

    static void Main(string[] args)
    {
        if (args.Length == 0 || !Uri.TryCreate(args[0], UriKind.Absolute, out Uri? uri))
        {
            uri = new Uri("https://localhost:444/healthcheck");
        }

        _benchmark = args.Contains("--benchmark");
        _clear = args.Contains("--clear");
        _color = args.Contains("--color");
        _noHeaders = args.Contains("--noheaders");

        var background = Console.BackgroundColor;
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.BackgroundColor = background;
            if (_clear)
            {
                Console.Clear();
            }
        };

        if (_benchmark)
        {
            // TODO: Resolve .sln reference error during benchmark run.
            var config = ManualConfig
                .Create(DefaultConfig.Instance)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                .AddJob(Job.Dry.WithIterationCount(1).WithWarmupCount(1));

            Console.WriteLine("Running Benchmark");
            var summary = BenchmarkRunner.Run<MyBenchmark>(config);
        }

        Stopwatch stopwatch = new();

        while (true)
        {
            HttpClientHandler handler = new()
            {
                // NOTE: Needed for localhost and lab environments with untrusted, self-signed certificates.
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using HttpClient client = new(handler);

            stopwatch.Start();

            try
            {
                var response = client.GetAsync(uri).Result;
                response.StatusCode.ShouldBe(HttpStatusCode.OK);

                Refresh("OK", stopwatch.ElapsedMilliseconds, uri, ConsoleColor.DarkGreen);

                foreach (var header in response.Headers)
                {
                    if (!_noHeaders)
                    {
                        Console.WriteLine($"{header.Key} = {header.Value.FirstOrDefault()}");
                    }
                }
            }
            catch (Exception e)
            {
                Refresh(e.Message, stopwatch.ElapsedMilliseconds, uri, ConsoleColor.DarkRed);
            }
            finally
            {
                stopwatch.Stop();
                stopwatch.Reset();
                client.Dispose();
            }

            Thread.Sleep(1000);
        }
    }

    private static void Refresh(string message, long duration, Uri uri, ConsoleColor color)
    {
        if (_color)
        {
            Console.BackgroundColor = color;
        }
        if (_clear)
        {
            Console.Clear();
        }
        Console.WriteLine($"{DateTime.UtcNow} - {uri} - {message} - {duration} ms");
    }
}

public class MyBenchmark
{
    private readonly bool input;

    public MyBenchmark() { input = false; }

    [Benchmark]
    public void MyBenchmarkMethod() => Console.WriteLine($"Benchmarking {input}");
}
