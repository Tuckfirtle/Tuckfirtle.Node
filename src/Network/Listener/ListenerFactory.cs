// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class ListenerFactory : IListenerFactory
    {
        private IConfigModel ConfigModel { get; }

        public ListenerFactory(IConfigModel configModel)
        {
            ConfigModel = configModel;
        }

        public Listener CreateP2PListener()
        {
            var configModel = ConfigModel;
            return new Listener(IPAddress.Parse(configModel.P2PListenerIp), configModel.P2PListenerPort);
        }

        public Listener CreateRPCListener()
        {
            var configModel = ConfigModel;
            return new Listener(IPAddress.Parse(configModel.RPCListenerIp), configModel.RPCListenerPort);
        }
    }
}