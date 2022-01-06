// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network;
using Tuckfirtle.Core.Network.P2P;
using Tuckfirtle.Core.Network.P2P.Header;
using Tuckfirtle.Core.Network.P2P.Packets;
using Tuckfirtle.OpenQuantumSafe;

namespace Tuckfirtle.Node.Network.P2P
{
    internal sealed class OutgoingPeer : Peer
    {
        public OutgoingPeer(NetworkType networkType, CancellationToken cancellationToken = default) : base(new TcpClient(), networkType, cancellationToken)
        {
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            TcpClient.Connect(ipAddress, port);
        }

        protected override void HandleHandshakeRequest()
        {
            // For outgoing connection, peer will send the ciphertext to generate shared secret.
            var networkStream = NetworkStream;

            using var kem = new KeyEncapsulationMechanism(CoreConfiguration.P2PPacketKeyEncapsulationMechanismType switch
            {
                KeyEncapsulationMechanismType.NtruHps4096821 => "NTRU-HPS-4096-821"
            });

            kem.GenerateKeypair(out var publicKey, out var secretKey);

            var handshakePacket = new HandshakePacket
            {
                KeyEncapsulationMechanismType = CoreConfiguration.P2PPacketKeyEncapsulationMechanismType,
                HandshakeType = HandshakeType.PublicKey,
                HandshakeData = ByteString.CopyFrom(publicKey)
            };

            var packetToSend = PacketUtility.SerializePacket(handshakePacket, NetworkType);
            packetToSend.WriteDelimitedTo(networkStream);

            var packet = Packet.Parser.ParseDelimitedFrom(networkStream);
            if (packet.PacketType != PacketType.Handshake) throw new Exception("Invalid packet type.");

            KeepAliveDuration = packet.PacketKeepAliveDuration;

            var nextHandshakePacket = PacketUtility.DeserializePacketData(packet, HandshakePacket.Parser);
            if (nextHandshakePacket.HandshakeType == HandshakeType.PublicKey) throw new Exception("Invalid handshake type.");

            kem.Decapsulation(out SharedSecretBytes, nextHandshakePacket.HandshakeData.ToByteArray(), secretKey);
        }
    }
}