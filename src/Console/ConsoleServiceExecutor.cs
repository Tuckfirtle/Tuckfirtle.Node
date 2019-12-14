// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Tasks;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Console
{
    internal class ConsoleServiceExecutor : IServiceExecutor
    {
        private readonly IConfig _config;

        private readonly IConsoleLogger _consoleLogger;

        private readonly CancellationTokenSource _cancellationTokenSource;

        public ConsoleServiceExecutor(IConfig config, IConsoleLogger consoleLogger, CancellationTokenSource cancellationTokenSource)
        {
            _config = config;
            _consoleLogger = consoleLogger;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

            System.Console.Title = $"{CoreConfiguration.CoinFullName} Node v{version} ({frameworkVersion})";
            System.Console.CancelKeyPress += (sender, args) => { args.Cancel = true; };

            _consoleLogger.LogMessage(new ConsoleMessageBuilder()
                .WriteLine("", false)
                .WriteLine("████████╗██╗   ██╗ ██████╗██╗  ██╗███████╗██╗██████╗ ████████╗██╗     ███████╗", false)
                .WriteLine("╚══██╔══╝██║   ██║██╔════╝██║ ██╔╝██╔════╝██║██╔══██╗╚══██╔══╝██║     ██╔════╝", false)
                .WriteLine("   ██║   ██║   ██║██║     █████╔╝ █████╗  ██║██████╔╝   ██║   ██║     █████╗  ", false)
                .WriteLine("   ██║   ██║   ██║██║     ██╔═██╗ ██╔══╝  ██║██╔══██╗   ██║   ██║     ██╔══╝  ", false)
                .WriteLine("   ██║   ╚██████╔╝╚██████╗██║  ██╗██║     ██║██║  ██║   ██║   ███████╗███████╗", false)
                .WriteLine("   ╚═╝    ╚═════╝  ╚═════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚══════╝", false)
                .WriteLine("", false)
                .Write(" * ", ConsoleColor.Green, false)
                .Write("ABOUT".PadRight(13), false)
                .Write($"{CoreConfiguration.CoinFullName} Node/{version} ", ConsoleColor.Cyan, false)
                .WriteLine(frameworkVersion, false)
                .Write(" * ", ConsoleColor.Green, false)
                .Write("COMMANDS".PadRight(13), false)
                .Write("Type ", false)
                .Write("help ", ConsoleColor.Magenta, false)
                .WriteLine("to get a list of commands.", false)
                .WriteLine("", false)
                .Build());

            if (!File.Exists(_config.ConfigFilePath))
            {
                _config.SaveConfig();

                _consoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine("Thank you for running a public node! This will help the chain a lot.", false)
                    .WriteLine("We are now building a configuration file for this node...", false)
                    .WriteLine($"Generated configuration file is at: \"{_config.ConfigFilePath}\"", false)
                    .WriteLine("Please ensure that the configuration is correct and run this application again to start the node.", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                System.Console.ReadLine();
                _cancellationTokenSource.Cancel();
                return;
            }

            _config.LoadConfig();

            TaskState.RunAndForget((_consoleLogger, _cancellationTokenSource), async state =>
            {
                var (consoleLogger, cancellationTokenSource) = state;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var command = await System.Console.In.ReadLineAsync().ConfigureAwait(false);

                    if (command == null)
                    {
                        cancellationTokenSource.Cancel();
                        continue;
                    }

                    command = command.Trim();

                    if (command.Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        consoleLogger.LogMessage(new ConsoleMessageBuilder()
                            .WriteLine("Available commands:", false)
                            .WriteLine("Exit - Signal the node to close. This will properly save the blockchain buffer if there is any.", false)
                            .Build());
                    }
                    else if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        cancellationTokenSource.Cancel();
                    else
                        consoleLogger.LogMessage("Invalid command!", ConsoleColor.Red, false);
                }
            }, _cancellationTokenSource.Token);
        }
    }
}