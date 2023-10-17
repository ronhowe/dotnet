/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;

namespace TestProject1
{
    [TestClass]
    public class TestBase
    {
        internal readonly string _asterisk = new('*', 80);
        internal readonly string _enter = new('>', 80);
        internal readonly string _exit = new('<', 80);
        internal readonly string _outputTemplate = "[{Level:u3}] [{SourceContext}] {Message}{NewLine}";
        internal readonly string _sourceContext = nameof(TestBase);
        // this is an example with more robust data included in a differently formatted log entry
        //internal readonly string _outputTemplate = "[{Timestamp:HH:mm:ss.fff zzz}] [{Level:u3}] [{MachineName}] [{SourceContext}] {Message}{NewLine}{Exception}";

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

            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            Log.ForContext("SourceContext", _sourceContext).Debug("Initializing Test");
            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        }
    }
}