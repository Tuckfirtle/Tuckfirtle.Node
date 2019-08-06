// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Config.Json
{
    internal sealed class JsonConfigService : IServiceExecutor
    {
        private JsonConfig JsonConfig { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public JsonConfigService(JsonConfig jsonConfig, IConsoleLogger consoleLogger, CancellationTokenSource cancellationTokenSource)
        {
            JsonConfig = jsonConfig;
            ConsoleLogger = consoleLogger;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void Execute()
        {
            var jsonConfig = JsonConfig;

            if (!jsonConfig.IsConfigFileExist())
            {
                jsonConfig.SaveConfig();

                ConsoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine("Thank you for running a local node!", false)
                    .WriteLine("We are now building a configuration file for this node...", false)
                    .WriteLine($"Generated configuration file is at: \"{jsonConfig.ConfigFilePath}\"", false)
                    .WriteLine("Please ensure that the configuration is correct and run this application to start the node.", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                System.Console.ReadLine();
                CancellationTokenSource.Cancel();
            }
            else
                jsonConfig.LoadConfig();
        }
    }
}