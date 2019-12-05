// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net.Sockets;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Network.Client
{
    internal sealed class P2PClient : BaseClient
    {
        private Task ReadingTask { get; }

        private Task WritingTask { get; }

        public P2PClient(TcpClient tcpClient, IConsoleLogger consoleLogger) : base(tcpClient, consoleLogger)
        {
        }

        protected override async Task OnHandlePacketFromNetworkAsync(string packet)
        {
        }
    }
}