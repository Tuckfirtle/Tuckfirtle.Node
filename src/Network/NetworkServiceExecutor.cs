// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;
using Tuckfirtle.Node.Network.Listener;

namespace Tuckfirtle.Node.Network
{
    internal sealed class NetworkServiceExecutor : IServiceExecutor
    {
        private IEnumerable<IListener> Listeners { get; }

        private IConfig Config { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private NatDiscoverer NatDiscoverer { get; }

        public NetworkServiceExecutor(IEnumerable<IListener> listeners, IConfig config, IConsoleLogger consoleLogger, NatDiscoverer natDiscoverer)
        {
            Listeners = listeners;
            Config = config;
            ConsoleLogger = consoleLogger;
            NatDiscoverer = natDiscoverer;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            taskAwaiter.EnqueueTask((Listeners, Config, ConsoleLogger, NatDiscoverer), async (cancellationToken, state) =>
            {
                var listeners = state.Listeners.ToArray();
                var config = state.Config;
                var consoleLogger = state.ConsoleLogger;
                var natDiscoverer = state.NatDiscoverer;

                // This is a fail safe mechanism in case someone clicked the BIG RED X BUTTON. It should release the port in time.
                AppDomain.CurrentDomain.ProcessExit += (sender, args) => NatDiscoverer.ReleaseAll();

                if (config.UniversalPlugAndPlay)
                {
                    try
                    {
                        consoleLogger.LogMessage("Locating NAT devices. This may take 5 seconds...");

                        var natDevices = await natDiscoverer.DiscoverDevicesAsync(PortMapper.Pmp | PortMapper.Upnp, new CancellationTokenSource(5000)).ConfigureAwait(false);
                        var natDevicesAvailable = false;

                        foreach (var natDevice in natDevices)
                        {
                            if (!natDevicesAvailable)
                                natDevicesAvailable = true;

                            foreach (var listener in listeners)
                            {
                                try
                                {
                                    await natDevice.CreatePortMapAsync(new Mapping(Protocol.Tcp, listener.ListenerPort, listener.ListenerPort, $"{CoreConfiguration.CoinFullName} {listener.ListenerType} port.")).ConfigureAwait(false);
                                    consoleLogger.LogMessage($"Successfully open port {listener.ListenerPort} for this session. If you are on multiple NAT devices, you are required to forward {listener.ListenerPort} for each NAT devices that are not directly connected to this device.", ConsoleColor.Green);
                                }
                                catch (MappingException)
                                {
                                    consoleLogger.LogMessage($"Unable to open port {listener.ListenerPort} for this session. Perhaps it is already open by another node on the same NAT device.", ConsoleColor.Yellow);
                                }
                            }
                        }

                        if (!natDevicesAvailable)
                            throw new TaskCanceledException();
                    }
                    catch (TaskCanceledException)
                    {
                        consoleLogger.LogMessage("No NAT device found. Please ensure that you have a NAT device that support universal plug and play and is enabled.", ConsoleColor.Red);

                        foreach (var listener in listeners)
                            consoleLogger.LogMessage($"Automatic port mapping failed. You are required to open port {listener.ListenerPort} manually to communicate with other peers.", ConsoleColor.Red);
                    }
                }

                foreach (var listener in listeners)
                    listener.StartListener();
            });
        }
    }
}