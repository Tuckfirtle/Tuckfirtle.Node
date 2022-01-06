// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using Tuckfirtle.Core.Network;
using Tuckfirtle.Core.Network.P2P;
using Tuckfirtle.Core.Network.P2P.Header;
using Tuckfirtle.Core.Network.P2P.Packets;
using Tuckfirtle.OpenQuantumSafe;

namespace Tuckfirtle.Node.Network.P2P
{
    internal sealed class IncomingPeer : Peer
    {
        public IncomingPeer(TcpClient tcpClient, NetworkType networkType, CancellationToken cancellationToken = default) : base(tcpClient, networkType, cancellationToken)
        {
        }

        protected override void HandleHandshakeRequest()
        {
            // For incoming connection, the peer would send the public key to generate a shared secret.
            var networkStream = NetworkStream;

            var packet = Packet.Parser.ParseDelimitedFrom(networkStream);
            if (packet.PacketType != PacketType.Handshake) throw new Exception("Invalid packet type.");

            KeepAliveDuration = packet.PacketKeepAliveDuration;

            var handshakePacket = PacketUtility.DeserializePacketData(packet, HandshakePacket.Parser);
            if (handshakePacket.HandshakeType == HandshakeType.CipherText) throw new Exception("Invalid handshake type.");

            using var kem = new KeyEncapsulationMechanism(handshakePacket.KeyEncapsulationMechanismType switch
            {
                KeyEncapsulationMechanismType.NtruHps4096821 => "NTRU-HPS-4096-821",
                var _ => throw new ArgumentOutOfRangeException()
            });

            kem.Encapsulation(out var ciphertext, out SharedSecretBytes, handshakePacket.HandshakeData.ToByteArray());

            var handshakePacketToSend = PacketUtility.SerializePacket(new HandshakePacket
            {
                KeyEncapsulationMechanismType = handshakePacket.KeyEncapsulationMechanismType,
                HandshakeType = HandshakeType.CipherText,
                HandshakeData = ByteString.CopyFrom(ciphertext)
            }, NetworkType);

            handshakePacketToSend.WriteDelimitedTo(networkStream);
        }
    }
}