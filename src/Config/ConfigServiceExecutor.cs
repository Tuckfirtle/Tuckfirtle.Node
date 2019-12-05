// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.IO;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Config
{
    internal sealed class ConfigServiceExecutor : IServiceExecutor
    {
        private IConfig Config { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public ConfigServiceExecutor(IConfig config, IConsoleLogger consoleLogger, CancellationTokenSource cancellationTokenSource)
        {
            Config = config;
            ConsoleLogger = consoleLogger;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            var config = Config;

            if (!File.Exists(config.ConfigFilePath))
            {
                config.SaveConfig();

                ConsoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine("Thank you for running a local node!", false)
                    .WriteLine("We are now building a configuration file for this node...", false)
                    .WriteLine($"Generated configuration file is at: \"{config.ConfigFilePath}\"", false)
                    .WriteLine("Please ensure that the configuration is correct and run this application to start the node.", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                System.Console.ReadLine();
                CancellationTokenSource.Cancel();
            }
            else
                config.LoadConfig();
        }
    }
}