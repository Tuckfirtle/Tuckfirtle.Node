// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using TheDialgaTeam.Core.Thread;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network;
using Tuckfirtle.Core.Network.P2P;
using Tuckfirtle.Core.Network.P2P.Packets;

namespace Tuckfirtle.Node.Network.P2P
{
    internal abstract class Peer
    {
        private sealed class ReadPacketThread : ThreadWithObjectState
        {
            private readonly Peer _peer;
            private readonly CancellationToken _cancellationToken;

            public ReadPacketThread(Peer peer, CancellationToken cancellationToken)
            {
                _peer = peer;
                _cancellationToken = cancellationToken;
            }

            protected override void Execute()
            {
                var peer = _peer;
                var networkStream = peer.NetworkStream;
                var cancellationToken = _cancellationToken;

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var packet = Packet.Parser.ParseDelimitedFrom(networkStream);
                        Interlocked.Exchange(ref peer.KeepAliveDuration, packet.PacketKeepAliveDuration);

                        // TODO: Handle this packet.
                    }
                    catch (InvalidProtocolBufferException)
                    {
                        // Packet is invalid but doesn't mean the stream has ended.
                        // Ignoring this packet and try again.
                    }
                    catch (IOException)
                    {
                        // Either the network has closed or read timeout has been triggered due to no ping response.
                        // TODO: Disconnect peer.
                    }
                }
            }
        }

        private sealed class WritePacketThread : ThreadWithObjectState
        {
            private readonly Peer _peer;
            private readonly CancellationToken _cancellationToken;

            public WritePacketThread(Peer peer, CancellationToken cancellationToken)
            {
                _peer = peer;
                _cancellationToken = cancellationToken;
            }

            protected override void Execute()
            {
                var peer = _peer;
                var networkStream = peer.NetworkStream;
                var networkType = peer.NetworkType;
                var sharedSecretBytes = peer.SharedSecretBytes;
                var packetsToSend = peer._packetsToSend;
                var cancellationToken = _cancellationToken;

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        using var pingTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        pingTimeout.CancelAfter(Thread.VolatileRead(ref peer.KeepAliveDuration));

                        var packetToSend = packetsToSend.Take(pingTimeout.Token);
                        packetToSend.WriteDelimitedTo(_peer.NetworkStream);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == cancellationToken) return;

                        // Timeout caused by ping duration.
                        var packet = PacketUtility.SerializePacket(new PingPacket(), networkType, sharedSecretBytes);
                        packet.WriteDelimitedTo(networkStream);
                    }
                    catch (IOException)
                    {
                        // Either the network has closed or write timeout has been triggered due to no response from the client.
                        // TODO: Disconnect peer.
                    }
                }
            }
        }

        public event Action<LogLevel, string, object[]>? Log;

        protected readonly TcpClient TcpClient;
        protected NetworkStream? NetworkStream;

        protected readonly NetworkType NetworkType;
        protected int KeepAliveDuration;
        protected byte[] SharedSecretBytes = new byte[32];

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly BlockingCollection<Packet> _packetsToSend = new BlockingCollection<Packet>();

        private bool _isActive;

        private ReadPacketThread? _readPacketThread;
        private WritePacketThread? _writePacketThread;

        protected Peer(TcpClient tcpClient, NetworkType networkType, CancellationToken cancellationToken)
        {
            TcpClient = tcpClient;
            NetworkType = networkType;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        public void Start()
        {
            if (_isActive) return;
            _isActive = true;

            var tcpClient = TcpClient;
            tcpClient.ReceiveTimeout = CoreConfiguration.P2PKeepAliveDuration * 2;
            tcpClient.SendTimeout = CoreConfiguration.P2PKeepAliveDuration * 2;
            tcpClient.ReceiveBufferSize = CoreConfiguration.P2PReceiveBufferSize;
            tcpClient.SendBufferSize = CoreConfiguration.P2PSendBufferSize;

            if (!tcpClient.Connected) throw new Exception("Peer is not connected.");

            NetworkStream = tcpClient.GetStream();

            try
            {
                HandleHandshakeRequest();

                var cancellationToken = _cancellationTokenSource.Token;

                _readPacketThread = new ReadPacketThread(this, cancellationToken);
                _readPacketThread.Start();

                _writePacketThread = new WritePacketThread(this, cancellationToken);
                _writePacketThread.Start();
            }
            catch (Exception)
            {
                // Something went wrong with the process, probably due to network error or packet issues.
                // TODO: Disconnect peer.
            }
        }

        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;
        }

        public async Task<bool> PeerCheckAsync(Configuration configuration)
        {
            if (!(TcpClient.Client.RemoteEndPoint is IPEndPoint ipEndPoint)) return false;
            if (configuration.P2PBlackListAddresses.Contains(ipEndPoint.ToString())) return false;

            using var ping = new Ping();
            var maxPingLimit = configuration.P2PMaxPingLimit;

            try
            {
                var pingReply = await ping.SendPingAsync(ipEndPoint.Address, maxPingLimit).ConfigureAwait(false);
                return pingReply.RoundtripTime <= maxPingLimit;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public void SendPacket(Packet packet)
        {
            _packetsToSend.Add(packet);
        }

        protected abstract void HandleHandshakeRequest();

        protected void LogMessage(LogLevel logLevel, string message, params object[] args)
        {
            var newArgs = new object[args.Length + 1];
            newArgs[0] = DateTimeOffset.Now;
            Array.Copy(args, 0, newArgs, 1, args.Length);

            Log?.Invoke(logLevel, $"{Logger.DateTimeTemplate} {message}", newArgs);
        }
    }
}