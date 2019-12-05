// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Console
{
    internal sealed class ConsoleServiceExecutor : IServiceExecutor
    {
        private IConsoleLogger ConsoleLogger { get; }

        private IConfig Config { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public ConsoleServiceExecutor(IConsoleLogger consoleLogger, IConfig config, CancellationTokenSource cancellationTokenSource)
        {
            ConsoleLogger = consoleLogger;
            Config = config;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            taskAwaiter.EnqueueTask((ConsoleLogger, Config, CancellationTokenSource), async (token, state) =>
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
                    .Write($"{CoreConfiguration.CoinFullName} Node/{version} ", ConsoleColor.Cyan, false)
                    .WriteLine(frameworkVersion, false)
                    .Write(" * ", ConsoleColor.Green, false)
                    .Write("COMMANDS".PadRight(13), false)
                    .Write("Type ", false)
                    .Write("help ", ConsoleColor.Magenta, false)
                    .WriteLine("to get a list of commands.", false)
                    .WriteLine("", false)
                    .Build());

                System.Console.CancelKeyPress += (sender, args) => { args.Cancel = true; };

                while (!token.IsCancellationRequested)
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
            });
        }
    }
}