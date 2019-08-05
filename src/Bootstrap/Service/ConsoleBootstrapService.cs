// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Bootstrap.Service
{
    internal sealed class ConsoleBootstrapService : IServiceExecutor
    {
        private IConsoleLogger ConsoleLogger { get; }

        private IConfig Config { get; }

        private ITaskAwaiter TaskAwaiter { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public ConsoleBootstrapService(IConsoleLogger consoleLogger, IConfig config, ITaskAwaiter taskAwaiter, CancellationTokenSource cancellationTokenSource)
        {
            ConsoleLogger = consoleLogger;
            Config = config;
            TaskAwaiter = taskAwaiter;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void Execute()
        {
            TaskAwaiter.EnqueueTask((ConsoleLogger, Config, CancellationTokenSource), async (token, state) =>
            {
                var consoleLogger = state.ConsoleLogger;
                var config = state.Config;
                var cancellationTokenSource = state.CancellationTokenSource;

                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

                consoleLogger.LogMessage(new ConsoleMessageBuilder()
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
                    .Write($"{CoreSettings.CoinFullName} Node/{version} ", ConsoleColor.Cyan, false)
                    .WriteLine(frameworkVersion, false)
                    .Write(" * ", ConsoleColor.Green, false)
                    .Write("COMMANDS".PadRight(13), false)
                    .Write("Type ", false)
                    .Write("help ", ConsoleColor.Magenta, false)
                    .WriteLine("to get a list of commands.", false)
                    .WriteLine("", false)
                    .Build());

                Console.CancelKeyPress += (sender, args) =>
                {
                    args.Cancel = true;
                    cancellationTokenSource.Cancel();
                };

                while (!token.IsCancellationRequested)
                {
                    var command = await Console.In.ReadLineAsync().ConfigureAwait(false);

                    if (command == null)
                        continue;

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
            });
        }
    }
}