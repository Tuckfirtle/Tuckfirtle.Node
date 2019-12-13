// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Tasks;

namespace Tuckfirtle.Node.Network.Client
{
    internal abstract class BaseClient : IDisposable
    {
        public IPEndPoint PublicIpAddress => TcpClient?.Client.RemoteEndPoint as IPEndPoint;

        private TcpClient TcpClient { get; }

        private StreamReader TcpClientReader { get; }

        private StreamWriter TcpClientWriter { get; }

        private Task ReadPacketFromNetworkTask { get; }

        private SemaphoreSlim SemaphoreSlim { get; }

        protected BaseClient()
        {
            SemaphoreSlim = new SemaphoreSlim(1, 1);
        }

        protected BaseClient(TcpClient tcpClient) : this()
        {
            TcpClient = tcpClient;
            TcpClientReader = new StreamReader(tcpClient.GetStream());
            TcpClientWriter = new StreamWriter(tcpClient.GetStream());

            ReadPacketFromNetworkTask = Task.Factory.StartNew<(StreamReader, Action<string>), Task>(async state =>
            {
                var (tcpClientReader, onHandlePacketFromNetwork) = state;

                if (tcpClientReader?.BaseStream is NetworkStream networkStream)
                {
                    try
                    {
                        while (true)
                        {
                            if (networkStream.DataAvailable)
                                onHandlePacketFromNetwork(tcpClientReader.ReadLine());

                            await Task.Delay(1).ConfigureAwait(false);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }, (TcpClientReader, OnHandlePacketFromNetwork)).Unwrap();
        }

        public virtual void SendPacketToNetworkAsync(string packet)
        {
        }

        protected abstract void OnHandlePacketFromNetwork(string packet);

        public void Dispose()
        {
            TcpClient?.Dispose();
            TcpClientReader?.Dispose();
            TcpClientWriter?.Dispose();
            ReadPacketFromNetworkTask?.Dispose();
        }
    }
}