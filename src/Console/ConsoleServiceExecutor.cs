// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using Serilog;
using Serilog.Core;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.Formatter;
using TheDialgaTeam.Core.Tasks;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Console
{
    internal class ConsoleServiceExecutor : IServiceExecutor, IDisposable
    {
        private readonly IConfig _config;

        private readonly LoggingLevelSwitch _loggingLevelSwitch;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ILogger _logger;

        private bool _isDisposed;

        public ConsoleServiceExecutor(IConfig config, LoggingLevelSwitch loggingLevelSwitch, CancellationTokenSource cancellationTokenSource)
        {
            _config = config;
            _loggingLevelSwitch = loggingLevelSwitch;
            _cancellationTokenSource = cancellationTokenSource;
            _logger = new LoggerConfiguration().WriteTo.CustomConsole(new OutputTemplateTextFormatter("{Message}{NewLine}")).CreateLogger();
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

            System.Console.Title = $"{CoreConfiguration.CoinFullName} Node v{version} ({frameworkVersion})";
            System.Console.CancelKeyPress += (sender, args) => { args.Cancel = true; };

            _logger.Information("");
            _logger.Information("████████╗██╗   ██╗ ██████╗██╗  ██╗███████╗██╗██████╗ ████████╗██╗     ███████╗");
            _logger.Information("╚══██╔══╝██║   ██║██╔════╝██║ ██╔╝██╔════╝██║██╔══██╗╚══██╔══╝██║     ██╔════╝");
            _logger.Information("   ██║   ██║   ██║██║     █████╔╝ █████╗  ██║██████╔╝   ██║   ██║     █████╗  ");
            _logger.Information("   ██║   ██║   ██║██║     ██╔═██╗ ██╔══╝  ██║██╔══██╗   ██║   ██║     ██╔══╝  ");
            _logger.Information("   ██║   ╚██████╔╝╚██████╗██║  ██╗██║     ██║██║  ██║   ██║   ███████╗███████╗");
            _logger.Information("   ╚═╝    ╚═════╝  ╚═════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚══════╝");
            _logger.Information("");
            _logger.Information($" * ABOUT        {CoreConfiguration.CoinFullName} Node/{version}");
            _logger.Information(" * COMMANDS     Type help to get a list of commands.");
            _logger.Information("");

            if (!File.Exists(_config.ConfigFilePath))
            {
                _config.SaveConfig();
            }
            else
            {
                _config.LoadConfig();
            }

            _loggingLevelSwitch.MinimumLevel = _config.MinimumLogEventLevel;

            TaskState.RunAndForget((_logger, _cancellationTokenSource), async state =>
            {
                var (logger, cancellationTokenSource) = state;

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
                        logger.Information("Available commands:");
                        logger.Information("Exit - Signal the node to close. This will properly save the block chain buffer if there is any.");
                    }
                    else if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        cancellationTokenSource.Cancel();
                    }
                    else
                    {
                        logger.Information("Invalid Command!");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            (_logger as Logger)?.Dispose();
        }
    }
}