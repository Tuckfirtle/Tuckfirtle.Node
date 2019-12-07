// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.Net.Sockets;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config;
using Tuckfirtle.Node.Network.Client;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class P2PListener : BaseListener
    {
        public override string ListenerType { get; } = "P2P";

        public override IPAddress ListenerIpAddress { get; }

        public override int ListenerPort { get; }

        private P2PClientCollection P2PClientCollection { get; }

        public P2PListener(IConsoleLogger consoleLogger, ITaskAwaiter taskAwaiter, IConfig config, P2PClientCollection p2pClientCollection) : base(consoleLogger, taskAwaiter)
        {
            P2PClientCollection = p2pClientCollection;
            ListenerIpAddress = IPAddress.Parse(config.P2PListenerIp);
            ListenerPort = config.P2PListenerPort;

            Initialize();
        }

        protected override void AcceptTcpClient(TcpClient tcpClient)
        {
            var peer = new P2PClient(tcpClient);
            ConsoleLogger.LogMessage($"[{ListenerType} IN {peer.PublicIpAddress}] Incoming connection.", ConsoleColor.Cyan);

            if (!P2PClientCollection.TryAddPeer(peer))
            {
                peer.Dispose();
                ConsoleLogger.LogMessage($"[{ListenerType} IN {peer.PublicIpAddress}] Connection dropped.", ConsoleColor.Cyan);
            }
            else
                ConsoleLogger.LogMessage($"[{ListenerType} IN {peer.PublicIpAddress}] Connected.", ConsoleColor.Cyan);
        }
    }
}