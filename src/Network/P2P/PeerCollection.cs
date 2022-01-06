// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;

namespace Tuckfirtle.Node.Network.P2P
{
    internal sealed class PeerCollection
    {
        private readonly List<Peer> _peers = new List<Peer>();
        private readonly object _peerLock = new object();

        public void AddNewPeer(Peer peer)
        {
            lock (_peerLock)
            {
                _peers.Add(peer);
            }
        }

        public Peer[] GetAllPeers()
        {
            lock (_peerLock)
            {
                return _peers.ToArray();
            }
        }
    }
}