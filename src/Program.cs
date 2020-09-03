// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TheDialgaTeam.Core.DependencyInjection;
using Tuckfirtle.Node.Config;
using Tuckfirtle.Node.Config.Json;
using Tuckfirtle.Node.Console;

namespace Tuckfirtle.Node
{
    internal static class Program
    {
        private static readonly DependencyManager DependencyManager = new DependencyManager();

        public static void Main()
        {
            DependencyManager.InstallService(serviceCollection =>
            {
                serviceCollection.AddSingleton<IConfig>(factory => new JsonConfig(Path.Combine(Environment.CurrentDirectory, "config.json")));
                serviceCollection.AddSingleton<LoggingLevelSwitch>();
                serviceCollection.AddSingleton<ILogger>(factory =>
                {
                    const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level} {Message}{NewLine}{Exception}";

                    var logsDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

                    if (!Directory.Exists(logsDirectory))
                    {
                        Directory.CreateDirectory(logsDirectory);
                    }

                    return new LoggerConfiguration()
                        .MinimumLevel.ControlledBy(factory.GetRequiredService<LoggingLevelSwitch>())
                        .WriteTo.Console(outputTemplate: outputTemplate)
                        .WriteTo.Async(configuration => configuration.File(Path.Combine(logsDirectory, "log.log"), outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null, retainedFileCountLimit: null), blockWhenFull: true)
                        .CreateLogger();
                });
                serviceCollection.AddSingleton<IServiceExecutor, ConsoleServiceExecutor>();
            });

            DependencyManager.BuildAndExecute((provider, exception) =>
            {
                const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level} {Message:lj}{NewLine}{Exception}";

                var logsDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                var logger = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .WriteTo.File(Path.Combine(logsDirectory, "error.log"), LogEventLevel.Fatal, outputTemplate)
                    .CreateLogger();

                if (exception is AggregateException aggregateException)
                {
                    foreach (var exInnerException in aggregateException.InnerExceptions)
                    {
                        if (exInnerException is OperationCanceledException)
                        {
                            continue;
                        }

                        logger.Fatal(exInnerException, "Unexpected error occured!");
                    }

                    logger.Information("Press Any key to exit...");
                    System.Console.Read();
                }
                else if (!(exception is OperationCanceledException))
                {
                    logger.Fatal(exception, "Unexpected error occured!");
                    logger.Information("Press Any key to exit...");
                    System.Console.Read();
                }
            });
        }
    }
}