// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net.Sockets;
using System.Threading.Tasks;
using Tuckfirtle.Core;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network
{
    internal class P2PClient : Client
    {
        private readonly IConfig _config;

        public P2PClient(IConfig config)
        {
            _config = config;
        }

        protected override void SetNetworkStreamTimeout(NetworkStream networkStream)
        {
            base.SetNetworkStreamTimeout(networkStream);

            networkStream.ReadTimeout = _config.P2PPingLimit + CoreConfiguration.P2PPingPacketDuration;
            networkStream.WriteTimeout = _config.P2PPingLimit;
        }

        protected override async Task<bool> ReadPacketsFromNetworkAsync(NetworkStream networkStream)
        {
            return true;
        }
    }
}