// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Task;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Console
{
    internal sealed class ConsoleServiceExecutor : IServiceExecutor
    {
        private IConsoleLogger ConsoleLogger { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        private IConfig Config { get; }

        public ConsoleServiceExecutor(IConsoleLogger consoleLogger, CancellationTokenSource cancellationTokenSource, IConfig config)
        {
            ConsoleLogger = consoleLogger;
            CancellationTokenSource = cancellationTokenSource;
            Config = config;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

            System.Console.Title = $"{CoreConfiguration.CoinFullName} Node v{version} ({frameworkVersion})";
            System.Console.CancelKeyPress += (sender, args) => { args.Cancel = true; };

            ConsoleLogger.LogMessage(new ConsoleMessageBuilder()
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

            if (!File.Exists(Config.ConfigFilePath))
            {
                Config.SaveConfig();

                ConsoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine("Thank you for running a public node! This will help the chain a lot.", false)
                    .WriteLine("We are now building a configuration file for this node...", false)
                    .WriteLine($"Generated configuration file is at: \"{Config.ConfigFilePath}\"", false)
                    .WriteLine("Please ensure that the configuration is correct and run this application again to start the node.", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                System.Console.ReadLine();
                CancellationTokenSource.Cancel();
            }
            else
            {
                Config.LoadConfig();
            }

            Task.Factory.StartNew(async innerState =>
            {
                var (consoleLogger, cancellationTokenSource) = innerState;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var command = await System.Console.In.ReadLineAsync().ConfigureAwait(false);

                    if (command == null)
                    {
                        cancellationTokenSource.Cancel();
                        continue;
                    }

                    if (command.Trim().Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        consoleLogger.LogMessage(new ConsoleMessageBuilder()
                            .WriteLine("Available commands:", false)
                            .WriteLine("Exit - Signal the node to close. This will properly save the blockchain buffer if there is any.", false)
                            .Build());
                    }
                    else if (command.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                        cancellationTokenSource.Cancel();
                    else
                        consoleLogger.LogMessage("Invalid command!", ConsoleColor.Red, false);
                }
            }, (ConsoleLogger, CancellationTokenSource), CancellationTokenSource.Token);
        }
    }
}