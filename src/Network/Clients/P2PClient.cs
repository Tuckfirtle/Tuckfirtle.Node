// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Tasks;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network.P2P;
using Tuckfirtle.Core.Network.P2P.Packets;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network.Clients
{
    internal class P2PClient : Client
    {
        private readonly IConfig _config;

        private PacketNetworkStream _packetNetworkStream;

        private Task _pingNetworkTask;

        private DateTimeOffset _lastValidPacketTimeOffset = DateTimeOffset.Now;

        public byte[] SharedSecretKey { get; set; }

        public P2PClient(IConfig config, TcpClient tcpClient = null) : base(tcpClient)
        {
            _config = config;

            if (TcpClient.Connected)
                StartReadingPacketsFromNetwork();
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

            _packetNetworkStream = new PacketNetworkStream(NetworkStream, _config.NetworkType);

            _pingNetworkTask = TaskState.Run(this, async client =>
            {
                while (!client.CancellationToken.IsCancellationRequested)
                {
                    if (DateTimeOffset.Now < client._lastValidPacketTimeOffset.AddMilliseconds(CoreConfiguration.P2PKeepAliveDuration))
                        await Task.Delay(1000, client.CancellationToken).ConfigureAwait(false);

                    var packet = await PacketUtility.SerializePacketAsync(new PingPacket(), client._config.NetworkType, client.SharedSecretKey).ConfigureAwait(false);
                    await client.WritePacketToNetworkAsync(packet).ConfigureAwait(false);

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