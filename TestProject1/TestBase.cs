/*******************************************************************************
https://github.com/ronhowe
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;

namespace TestProject1
{
    [TestClass]
    public class TestBase
    {
        internal readonly string _outputTemplate = "[{Timestamp:yyyy-MM-dd @ HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] [{MachineName}] {Message}{NewLine}{Exception}";
        internal readonly string _sourceContext = nameof(TestBase);

        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate: _outputTemplate)
                .CreateLogger();

            Log.ForContext("SourceContext", _sourceContext).Debug("Initializing Test");
        }
    }
}