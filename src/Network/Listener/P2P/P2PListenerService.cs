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

        private IConfigModel ConfigModel { get; }

        private ITaskAwaiter TaskAwaiter { get; }

        private IConsoleLogger ConsoleLogger { get; }

        public P2PListenerService(IListenerFactory listenerFactory, IConfigModel configModel, ITaskAwaiter taskAwaiter, IConsoleLogger consoleLogger)
        {
            ListenerFactory = listenerFactory;
            ConfigModel = configModel;
            TaskAwaiter = taskAwaiter;
            ConsoleLogger = consoleLogger;
        }

        public void Execute()
        {
            TaskAwaiter.EnqueueTask((ListenerFactory, ConfigModel, ConsoleLogger), async (token, state) =>
            {
                var configModel = state.ConfigModel;
                var consoleLogger = state.ConsoleLogger;

                consoleLogger.LogMessage("Initializing P2P Listener...");

                if (configModel.UniversalPlugAndPlay)
                {
                    try
                    {
                        consoleLogger.LogMessage("Locating NAT devices...");

                        var natDiscoverer = new NatDiscoverer();
                        var natDevice = await natDiscoverer.DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource(3000)).ConfigureAwait(false);
                        await natDevice.CreatePortMapAsync(new Mapping(Protocol.Tcp, configModel.P2PListenerPort, configModel.P2PListenerPort, $"{CoreSettings.CoinFullName} P2P Listener Port.")).ConfigureAwait(false);
                        consoleLogger.LogMessage($"Successfully open port {configModel.P2PListenerPort} for this session.", ConsoleColor.Green);
                    }
                    catch (NatDeviceNotFoundException)
                    {
                        consoleLogger.LogMessage(new ConsoleMessageBuilder()
                            .WriteLine("No NAT device found. Please ensure that you have a NAT device that support universal plug and play and is enabled.", ConsoleColor.Red)
                            .WriteLine($"Automatic port mapping failed. You are required to open port {configModel.P2PListenerPort} to communicate with other peers.", ConsoleColor.Red)
                            .Build());
                    }
                    catch (MappingException)
                    {
                        consoleLogger.LogMessage($"Unable to open port {configModel.P2PListenerPort} for this session. Perhaps it is already open by another node on the same NAT device.", ConsoleColor.Red);
                    }
                }
            });
        }
    }
}