// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.IO;
using Serilog;
using Serilog.Core;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.Formatter;

namespace Tuckfirtle.Node.Logger
{
    internal class Logger
    {
        private const string OutputTemplate = "{Message}{NewLine}{Exception}";

        public LoggingLevelSwitch LoggingLevelSwitch { get; }

        private ILogger _logger;

        public Logger(string logsDirectory)
        {
            LoggingLevelSwitch = new LoggingLevelSwitch();

            _logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LoggingLevelSwitch)
                .WriteTo.CustomConsole(new OutputTemplateTextFormatter(OutputTemplate))
                .WriteTo.Async(configuration => configuration.File(new OutputTemplateTextFormatter(OutputTemplate), Path.Combine(logsDirectory, "log.log"), rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null, retainedFileCountLimit: null), blockWhenFull: true)
                .CreateLogger();
        }
    }
}