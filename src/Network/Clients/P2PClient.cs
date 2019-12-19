// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using TheDialgaTeam.Core.Tasks;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network;
using Tuckfirtle.Core.Network.P2P;
using Tuckfirtle.Core.Network.P2P.Packets;

namespace Tuckfirtle.Node.Network.Clients
{
    internal class P2PClient : Client
    {
        private readonly NetworkType _networkType;

        private PacketNetworkStream _packetNetworkStream;

        private Task _pingNetworkTask;

        private DateTimeOffset _lastValidPacketTimeOffset = DateTimeOffset.Now;

        public byte[] SharedSecretKey { get; set; }

        public P2PClient(NetworkType networkType, TcpClient tcpClient = null) : base(tcpClient)
        {
            _networkType = networkType;

            if (TcpClient.Connected)
                StartReadingPacketsFromNetwork();
        }

        public async Task WritePacketToNetworkAsync(IMessage message)
        {
            try
            {
                var packet = await PacketUtility.SerializePacketAsync(message, _networkType, SharedSecretKey).ConfigureAwait(false);
                await _packetNetworkStream.WritePacketAsync(packet, CancellationToken).ConfigureAwait(false);
            }
            catch (Exception )
            {
                Close();
            }
        }

        public async Task WritePacketToNetworkAsync(Packet packet)
        {
            try
            {
                await _packetNetworkStream.WritePacketAsync(packet, CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                Close();
            }
        }

        protected override void BeforeStartReadingPacketsFromNetwork()
        {
            base.BeforeStartReadingPacketsFromNetwork();

            TcpClient.ReceiveBufferSize = CoreConfiguration.P2PReceiveBufferSize;
            TcpClient.SendBufferSize = CoreConfiguration.P2PSendBufferSize;

            _packetNetworkStream = new PacketNetworkStream(NetworkStream, _networkType);

            _pingNetworkTask = TaskState.Run(this, async client =>
            {
                var cancellationToken = client.CancellationToken;

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (DateTimeOffset.Now < client._lastValidPacketTimeOffset.AddMilliseconds(CoreConfiguration.P2PKeepAliveDuration))
                        await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

                    await client.WritePacketToNetworkAsync(new PingPacket()).ConfigureAwait(false);

                    client._lastValidPacketTimeOffset = DateTimeOffset.Now;
                }
            });
        }

        protected override async Task<bool> ReadPacketsFromNetworkAsync(CancellationToken cancellationToken)
        {
            try
            {
                var packet = await _packetNetworkStream.ReadPacketAsync(cancellationToken).ConfigureAwait(false);

                if (packet == null)
                    return false;

                _lastValidPacketTimeOffset = DateTimeOffset.Now;

                TaskState.RunAndForget((this, packet), state =>
                {
                    // Handle Packet here.
                }, cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Task.WaitAll(_pingNetworkTask);
                _pingNetworkTask.Dispose();
                _packetNetworkStream?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}