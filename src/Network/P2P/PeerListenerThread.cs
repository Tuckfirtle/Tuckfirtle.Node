// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TheDialgaTeam.Core.Thread;

namespace Tuckfirtle.Node.Network.P2P
{
    internal sealed class PeerListenerThread : PollingThreadWithObjectState
    {
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentBag<Peer> _peers;

        public PeerListenerThread(TcpListener tcpListener, ConcurrentBag<Peer> peers, CancellationToken cancellationToken) : base(cancellationToken)
        {
            _tcpListener = tcpListener;
            _peers = peers;
        }

        protected override void Execute(CancellationToken cancellationToken)
        {
        }
    }
}