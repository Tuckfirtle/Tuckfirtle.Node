// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net.Sockets;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class P2PListener : BaseListener
    {
        protected override string ListenerType { get; } = "P2P";

        protected override int RequiredPortMapping(IConfigModel configModel)
        {
            return configModel.P2PListenerPort;
        }

        protected override void AcceptTcpClient(TcpClient tcpClient)
        {
            // TODO: Whatever you need to handle P2P connections.
            tcpClient.Dispose();
        }
    }
}