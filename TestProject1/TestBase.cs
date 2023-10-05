/*******************************************************************************
https://github.com/ronhowe/dotnet
*******************************************************************************/

using ClassLibrary1;
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
        internal readonly string _outputTemplate = "[{SourceContext}] {Message}{NewLine}";
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

            Log.ForContext("SourceContext", _sourceContext).Debug(PowerOnSelfTest.DebugLoggingOn);
            Log.ForContext("SourceContext", _sourceContext).Information(PowerOnSelfTest.InformationLoggingOn);
            Log.ForContext("SourceContext", _sourceContext).Warning(PowerOnSelfTest.WarningLoggingOn);
            Log.ForContext("SourceContext", _sourceContext).Error(PowerOnSelfTest.ErrorLoggingOn);
            Log.ForContext("SourceContext", _sourceContext).Fatal(PowerOnSelfTest.FatalLoggingOn);

            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
            Log.ForContext("SourceContext", _sourceContext).Debug("Initializing Test");
            Log.ForContext("SourceContext", _sourceContext).Debug(_asterisk);
        }
    }
}