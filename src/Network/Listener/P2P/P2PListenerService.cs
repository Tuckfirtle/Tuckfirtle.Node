// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Linq;
using System.Threading;
using Open.Nat;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Listener.P2P
{
    internal sealed class P2PListenerService : IServiceExecutor
    {
        private IListenerFactory ListenerFactory { get; }

        private IConfig Config { get; }

        private ITaskAwaiter TaskAwaiter { get; }

        private IConsoleLogger ConsoleLogger { get; }

        public P2PListenerService(IListenerFactory listenerFactory, IConfig config, ITaskAwaiter taskAwaiter, IConsoleLogger consoleLogger)
        {
            ListenerFactory = listenerFactory;
            Config = config;
            TaskAwaiter = taskAwaiter;
            ConsoleLogger = consoleLogger;
        }

        public void Execute()
        {
            TaskAwaiter.EnqueueTask((ListenerFactory, Config, ConsoleLogger), async (token, state) =>
            {
                var config = state.Config;
                var consoleLogger = state.ConsoleLogger;

                consoleLogger.LogMessage("Initializing P2P Listener...");

                if (config.UniversalPlugAndPlay)
                {
                    try
                    {
                        consoleLogger.LogMessage("Locating NAT devices...");

                        var natDiscoverer = new NatDiscoverer();
                        var natDevice = await natDiscoverer.DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource(3000)).ConfigureAwait(false);
                        await natDevice.CreatePortMapAsync(new Mapping(Protocol.Tcp, config.P2PListenerPort, config.P2PListenerPort, $"{CoreSettings.CoinFullName} P2P Listener Port.")).ConfigureAwait(false);
                        consoleLogger.LogMessage($"Successfully open port {config.P2PListenerPort} for this session.", ConsoleColor.Green);
                    }
                    catch (NatDeviceNotFoundException)
                    {
                        consoleLogger.LogMessage(new ConsoleMessageBuilder()
                            .WriteLine("No NAT device found. Please ensure that you have a NAT device that support universal plug and play and is enabled.", ConsoleColor.Red)
                            .WriteLine($"Automatic port mapping failed. You are required to open port {config.P2PListenerPort} to communicate with other peers.", ConsoleColor.Red)
                            .Build());
                    }
                    catch (MappingException)
                    {
                        consoleLogger.LogMessage($"Unable to open port {config.P2PListenerPort} for this session. Perhaps it is already open by another node on the same NAT device.", ConsoleColor.Red);
                    }
                }
            });
        }
    }
}