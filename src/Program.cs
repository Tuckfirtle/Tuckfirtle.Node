// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.DependencyInjection.Installer;
using Tuckfirtle.Node.Config.Json;
using Tuckfirtle.Node.Console;
using Tuckfirtle.Node.Network;

namespace Tuckfirtle.Node
{
    internal static class Program
    {
        private static DependencyManager DependencyManager { get; } = new DependencyManager();

        public static void Main()
        {
            var logsDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

            if (!Directory.Exists(logsDirectory))
                Directory.CreateDirectory(logsDirectory);

            DependencyManager.InstallService(new ConsoleStreamWriteToFileQueueLoggerServiceInstaller(Path.Combine(logsDirectory, $"{DateTime.Now:yyyy-MM-dd}.log")));
            DependencyManager.InstallService(new JsonConfigServiceInstaller(Path.Combine(Environment.CurrentDirectory, "Config.json")));
            DependencyManager.InstallService(new ConsoleServiceInstaller());
            DependencyManager.InstallService(new NetworkServiceInstaller());

            DependencyManager.BuildAndExecute((provider, exception) =>
            {
                var consoleLogger = new ConsoleStreamLogger(System.Console.Error);

                if (exception is AggregateException aggregateException)
                {
                    var consoleMessages = new ConsoleMessageBuilder();

                    foreach (var exInnerException in aggregateException.InnerExceptions)
                    {
                        if (exInnerException is OperationCanceledException)
                            continue;

                        consoleMessages.WriteLine(exInnerException.ToString(), ConsoleColor.Red);
                    }

                    var message = consoleMessages.WriteLine("Press Enter/Return to exit...").Build();

                    if (message.Count > 1)
                    {
                        consoleLogger.LogMessage(message);
                        provider.GetRequiredService<CancellationTokenSource>().Cancel();
                        System.Console.ReadLine();
                    }

                    ExitWithFault();
                }
                else
                {
                    consoleLogger.LogMessage(new ConsoleMessageBuilder()
                        .WriteLine(exception.ToString(), ConsoleColor.Red)
                        .WriteLine("Press Enter/Return to exit...")
                        .Build());

                    provider.GetRequiredService<CancellationTokenSource>().Cancel();
                    System.Console.ReadLine();

                    ExitWithFault();
                }
            });
        }

        private static void ExitWithFault()
        {
            DependencyManager.Dispose();
            Environment.Exit(1);
        }
    }
}