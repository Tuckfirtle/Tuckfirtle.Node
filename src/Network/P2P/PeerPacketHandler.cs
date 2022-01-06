// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Tuckfirtle.Core.Network.P2P;

namespace Tuckfirtle.Node.Network.P2P
{
    public class PeerPacketHandler
    {
        private readonly PacketNetworkStream _packetNetworkStream;

        public PeerPacketHandler(PacketNetworkStream packetNetworkStream)
        {
            _packetNetworkStream = packetNetworkStream;
        }
    }
}