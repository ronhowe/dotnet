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
        public readonly string _asterisk = new('*', 80);
        public readonly string _enter = new('>', 80);
        public readonly string _exit = new('<', 80);
        public readonly string _outputTemplate = "[{SourceContext}] {Message}{NewLine}";
        public readonly string _sourceContext = nameof(TestBase);

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

            Log.ForContext("SourceContext", _sourceContext).Debug("Power-On Self-Test (1 of 5) => Logging Debug OK");
            Log.ForContext("SourceContext", _sourceContext).Information("Power-On Self-Test (2 of 5) => Logging Information OK");
            Log.ForContext("SourceContext", _sourceContext).Warning("Power-On Self-Test (3 of 5) => Logging Warning OK");
            Log.ForContext("SourceContext", _sourceContext).Error("Power-On Self-Test (4 of 5) => Logging Error OK");
            Log.ForContext("SourceContext", _sourceContext).Fatal("Power-On Self-Test (5 of 5) => Logging Fatal OK");

            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            Log.ForContext("SourceContext", _sourceContext).Debug("Initializing Test");
            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        }
    }
}