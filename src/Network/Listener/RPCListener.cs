// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net.Sockets;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class RPCListener : BaseListener
    {
        protected override string ListenerType { get; } = "RPC";

        protected override int RequiredPortMapping(IConfigModel configModel)
        {
            return configModel.RPCListenerPort;
        }

        protected override void AcceptTcpClient(TcpClient tcpClient)
        {
            // TODO: Whatever you need to handle RPC connections.
            tcpClient.Dispose();
        }
    }
}