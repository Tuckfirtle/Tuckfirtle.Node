// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Database;
using Tuckfirtle.Node.Network.P2P;

namespace Tuckfirtle.Node
{
    internal sealed class ProgramHostedService : IHostedService, IDisposable
    {
        private readonly Logger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly SqliteDatabaseFactory _sqliteDatabaseFactory;
        private readonly PeerListener _peerListener;

        //private ConsoleCommandHandlerThread? _consoleCommandHandlerThread;

        public ProgramHostedService(Logger logger, IHostApplicationLifetime hostApplicationLifetime, SqliteDatabaseFactory sqliteDatabaseFactory, PeerListener peerListener)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _sqliteDatabaseFactory = sqliteDatabaseFactory;
            _peerListener = peerListener;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

            Console.Title = $"{CoreConfiguration.CoinFullName} Node v{version} ({frameworkVersion})";

            var logger = _logger;

            logger.LogInformation("", false);
            logger.LogInformation("████████╗██╗   ██╗ ██████╗██╗  ██╗███████╗██╗██████╗ ████████╗██╗     ███████╗", false);
            logger.LogInformation("╚══██╔══╝██║   ██║██╔════╝██║ ██╔╝██╔════╝██║██╔══██╗╚══██╔══╝██║     ██╔════╝", false);
            logger.LogInformation("   ██║   ██║   ██║██║     █████╔╝ █████╗  ██║██████╔╝   ██║   ██║     █████╗  ", false);
            logger.LogInformation("   ██║   ██║   ██║██║     ██╔═██╗ ██╔══╝  ██║██╔══██╗   ██║   ██║     ██╔══╝  ", false);
            logger.LogInformation("   ██║   ╚██████╔╝╚██████╗██║  ██╗██║     ██║██║  ██║   ██║   ███████╗███████╗", false);
            logger.LogInformation("   ╚═╝    ╚═════╝  ╚═════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚══════╝", false);
            logger.LogInformation("", false);
            logger.LogInformation(" \u001b[32;1m*\u001b[0m ABOUT        \u001b[36;1m{CoinFullName:l} Node/{version:l}\u001b[0m {frameworkVersion:l}", false, CoreConfiguration.CoinFullName, version!, frameworkVersion!);
            logger.LogInformation(" \u001b[32;1m*\u001b[0m COMMANDS     Type \u001b[35;1mhelp\u001b[0m to get a list of commands.", false);
            logger.LogInformation("", false);
            logger.LogInformation("Creating/Updating database...", true);

            using (var sqliteDatabaseContext = _sqliteDatabaseFactory.GetSqliteDatabaseContext(false))
            {
                await sqliteDatabaseContext.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }

            logger.LogInformation("Update complete.", true);

            //_consoleCommandHandlerThread = new ConsoleCommandHandlerThread(logger, _hostApplicationLifetime, cancellationToken);
            //_consoleCommandHandlerThread.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //_consoleCommandHandlerThread?.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_consoleCommandHandlerThread?.Dispose();
        }
    }
}