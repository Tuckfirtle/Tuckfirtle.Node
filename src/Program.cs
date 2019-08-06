// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.DependencyInjection.Factory;
using Tuckfirtle.Node.Bootstrap;
using Tuckfirtle.Node.Config.Json;
using Tuckfirtle.Node.Console;
using Tuckfirtle.Node.Network.Listener;
using Tuckfirtle.Node.Network.Listener.P2P;

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

            DependencyManager.InstallFactory(new ConsoleStreamWriteToFileQueuedTaskLoggerFactoryInstaller(Path.Combine(logsDirectory, $"{DateTime.Now:yyyy-MM-dd}.log")));
            DependencyManager.InstallFactory(new BootstrapFactoryInstaller());
            DependencyManager.InstallFactory(new JsonConfigFactoryInstaller(Path.Combine(Environment.CurrentDirectory, "Config.json")));
            DependencyManager.InstallFactory(new ConsoleFactoryInstaller());
            DependencyManager.InstallFactory(new ListenerFactoryInstaller());
            DependencyManager.InstallFactory(new P2PListenerFactoryInstaller());

            DependencyManager.BuildAndExecute((provider, exception) =>
            {
                var consoleLogger = provider.GetRequiredService<IConsoleLogger>() ?? new ConsoleStreamLogger(System.Console.Error);

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

                    if (message.Length > 1)
                    {
                        consoleLogger.LogMessage(message);
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