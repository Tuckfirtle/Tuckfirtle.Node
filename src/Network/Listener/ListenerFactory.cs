// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class ListenerFactory : IListenerFactory
    {
        private IConfig Config { get; }

        public ListenerFactory(IConfig config)
        {
            Config = config;
        }

        public Listener CreateP2PListener()
        {
            var config = Config;
            return new Listener(IPAddress.Parse(config.P2PListenerIp), config.P2PListenerPort);
        }

        public Listener CreateRPCListener()
        {
            var config = Config;
            return new Listener(IPAddress.Parse(config.RPCListenerIp), config.RPCListenerPort);
        }
    }
}