// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.Formatter;
using Tuckfirtle.Node.Database;
using Tuckfirtle.Node.Network.P2P;

namespace Tuckfirtle.Node
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            await CreateHostBuilder(args).RunConsoleAsync().ConfigureAwait(false);
#else
            try
            {
                await CreateHostBuilder(args).RunConsoleAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var errorLogDirectory = Path.Combine(Environment.CurrentDirectory, "errors");

                if (!Directory.Exists(errorLogDirectory))
                {
                    Directory.CreateDirectory(errorLogDirectory);
                }

                await using var fileStream = new FileStream(Path.Combine(errorLogDirectory, $"{DateTimeOffset.Now:yyyyMMddHHmmss}.log"), FileMode.Append, FileAccess.Write, FileShare.Read);
                await using var writer = new StreamWriter(fileStream);

                await writer.WriteLineAsync(ex.ToString()).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
#endif
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, serviceCollection) =>
                {
                    serviceCollection.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromMinutes(5); });

                    serviceCollection.AddSingleton<Configuration>();

                    serviceCollection.AddDbContext<SqliteDatabaseContext>((serviceProvider, builder) =>
                    {
                        var configuration = serviceProvider.GetRequiredService<Configuration>();
                        var defaultLocation = Path.Combine(Environment.CurrentDirectory, "data");
                        var dataDirectory = configuration.DatabaseLocation;

                        if (string.IsNullOrWhiteSpace(dataDirectory))
                        {
                            dataDirectory = defaultLocation;
                        }

                        if (!Directory.Exists(dataDirectory))
                        {
                            Directory.CreateDirectory(dataDirectory);
                        }

                        builder.UseSqlite($"Data Source={Path.Combine(dataDirectory, "data.db")}");
                    });

                    serviceCollection.AddSingleton<SqliteDatabaseFactory>();

                    serviceCollection.AddSingleton<LoggingLevelSwitch>();
                    serviceCollection.AddSingleton<Logger>();

                    serviceCollection.AddSingleton<PeerListener>();

                    serviceCollection.AddHostedService<ProgramHostedService>();
                })
                .UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
                {
                    const string outputTemplate = "{Message}{NewLine}{Exception}";

                    var logsDirectory = Path.Combine(hostBuilderContext.HostingEnvironment.ContentRootPath, "logs");

                    if (!Directory.Exists(logsDirectory))
                    {
                        Directory.CreateDirectory(logsDirectory);
                    }

                    loggerConfiguration
                        .ReadFrom.Configuration(hostBuilderContext.Configuration)
                        .MinimumLevel.ControlledBy(serviceProvider.GetRequiredService<LoggingLevelSwitch>())
                        .WriteTo.AnsiConsole(new OutputTemplateTextFormatter(outputTemplate))
                        .WriteTo.Async(configuration => configuration.File(Path.Combine(logsDirectory, "log.log"), outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null, retainedFileCountLimit: null), blockWhenFull: true);
                });
        }
    }
}