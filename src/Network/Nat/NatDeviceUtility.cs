// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Network.Nat
{
    internal sealed class NatDeviceUtility : INatDeviceUtility, IDisposable
    {
        private List<Mapping> PortMappings { get; } = new List<Mapping>();

        public NatDeviceUtility()
        {
            // This is a fail safe mechanism in case someone clicked the BIG RED X BUTTON. It should release the port in time.
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => NatDiscoverer.ReleaseAll();
        }

        public void AddPortMapping(int port)
        {
            PortMappings.Add(new Mapping(Protocol.Tcp, port, port, $"{CoreSettings.CoinFullName} port."));
        }

        public async Task OpenPortsAsync(IConsoleLogger consoleLogger)
        {
            try
            {
                consoleLogger.LogMessage("Locating NAT devices. This may take 5 seconds...");

                var natDiscoverer = new NatDiscoverer();
                var natDevices = await natDiscoverer.DiscoverDevicesAsync(PortMapper.Pmp | PortMapper.Upnp, new CancellationTokenSource(5000)).ConfigureAwait(false);

                var portMappings = PortMappings;
                var natDevicesAvailable = false;

                foreach (var natDevice in natDevices)
                {
                    if (!natDevicesAvailable)
                        natDevicesAvailable = true;

                    foreach (var portMapping in portMappings)
                    {
                        try
                        {
                            await natDevice.CreatePortMapAsync(portMapping).ConfigureAwait(false);
                            consoleLogger.LogMessage($"Successfully open port {portMapping.PublicPort} for this session. If you are on multiple NAT devices, you are required to forward {portMapping.PublicPort} for each NAT devices that are not directly connected to this device.", ConsoleColor.Green);
                        }
                        catch (MappingException)
                        {
                            consoleLogger.LogMessage($"Unable to open port {portMapping.PublicPort} for this session. Perhaps it is already open by another node on the same NAT device.", ConsoleColor.Yellow);
                        }
                    }
                }

                if (!natDevicesAvailable)
                    throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                consoleLogger.LogMessage("No NAT device found. Please ensure that you have a NAT device that support universal plug and play and is enabled.", ConsoleColor.Red);

                var portMappings = PortMappings;

                foreach (var portMapping in portMappings)
                    consoleLogger.LogMessage($"Automatic port mapping failed. You are required to open port {portMapping.PublicPort} manually to communicate with other peers.", ConsoleColor.Red);
            }
        }

        public void Dispose()
        {
            NatDiscoverer.ReleaseAll();
        }
    }
}