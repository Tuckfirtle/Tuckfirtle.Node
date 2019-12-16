// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
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
        private static readonly DependencyManager DependencyManager = new DependencyManager();

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
                var fileLogger = new ConsoleStreamLogger(new StreamWriter(new FileStream(Path.Combine(logsDirectory, $"{DateTime.Now:yyyy-MM-dd}.error"), FileMode.Append, FileAccess.Write, FileShare.Read)));

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
                        fileLogger.LogMessage(message);
                        System.Console.ReadLine();
                    }
                }
                else
                {
                    var consoleMessage = new ConsoleMessageBuilder()
                        .WriteLine(exception.ToString(), ConsoleColor.Red)
                        .WriteLine("Press Enter/Return to exit...")
                        .Build();

                    consoleLogger.LogMessage(consoleMessage);
                    fileLogger.LogMessage(consoleMessage);
                    System.Console.ReadLine();
                }

                consoleLogger.Dispose();
                fileLogger.Dispose();

                ExitWithFault();
            });
        }

        private static void ExitWithFault()
        {
            DependencyManager.Dispose();
            Environment.Exit(1);
        }
    }
}