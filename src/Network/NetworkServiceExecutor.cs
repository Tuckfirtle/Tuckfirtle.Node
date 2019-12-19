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

namespace Tuckfirtle.Node.Network
{
    internal class NetworkServiceExecutor : IServiceExecutor, IDisposable
    {
        private readonly IEnumerable<IListener> _listeners;

        private readonly IConfig _config;

        private readonly IConsoleLogger _consoleLogger;

        private readonly NatDiscoverer _natDiscoverer;

        public NetworkServiceExecutor(IEnumerable<IListener> listeners, IConfig config, IConsoleLogger consoleLogger, NatDiscoverer natDiscoverer)
        {
            _listeners = listeners;
            _config = config;
            _consoleLogger = consoleLogger;
            _natDiscoverer = natDiscoverer;
        }

        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            taskAwaiter.EnqueueTask((_listeners, _config, _consoleLogger, _natDiscoverer), async (cancellationToken, state) =>
            {
                var (listeners, config, consoleLogger, natDiscoverer) = state;
                listeners = listeners.ToArray();

                if (config.UniversalPlugAndPlay)
                {
                    try
                    {
                        consoleLogger.LogMessage("Locating NAT devices. This may take 3 seconds...");

                        var natDevices = await natDiscoverer.DiscoverDevicesAsync(PortMapper.Pmp | PortMapper.Upnp, new CancellationTokenSource(3000)).ConfigureAwait(false);
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
                {
                    listener.Logger += ListenerOnLogger;
                    listener.Start();
                }
            });
        }

        private void ListenerOnLogger(List<ConsoleMessage> consoleMessages)
        {
            _consoleLogger.LogMessage(consoleMessages);
        }

        public void Dispose()
        {
            foreach (var listener in _listeners)
            {
                listener.Stop();
                listener.Logger -= ListenerOnLogger;
            }

            NatDiscoverer.ReleaseAll();
        }
    }
}