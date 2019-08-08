// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Threading.Tasks;
using Open.Nat;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Network.Nat
{
    internal sealed class NatDeviceUtility : INatDeviceUtility, IDisposable
    {
        private NatDiscoverer NatDiscoverer { get; } = new NatDiscoverer();

        public async Task OpenPortAsync(ushort port)
        {
            var natDevice = await NatDiscoverer.DiscoverDeviceAsync().ConfigureAwait(false);
            await natDevice.CreatePortMapAsync(new Mapping(Protocol.Tcp, port, port, $"{CoreSettings.CoinFullName} port.")).ConfigureAwait(false);
        }

        public void Dispose()
        {
            NatDiscoverer.ReleaseAll();
        }
    }
}