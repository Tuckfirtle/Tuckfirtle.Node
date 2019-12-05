// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using System.Net.Sockets;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class RPCListener : BaseListener
    {
        public override string ListenerType { get; } = "RPC";

        public override IPAddress ListenerIpAddress { get; }

        public override int ListenerPort { get; }

        public RPCListener(IConfig config, IConsoleLogger consoleLogger, ITaskAwaiter taskAwaiter) : base(config, consoleLogger, taskAwaiter)
        {
            ListenerIpAddress = IPAddress.Parse(config.RPCListenerIp);
            ListenerPort = config.RPCListenerPort;

            Initialize();
        }

        protected override void AcceptTcpClient(TcpClient tcpClient)
        {
            // TODO: Whatever you need to handle RPC connections.
            tcpClient.Dispose();
        }
    }
}