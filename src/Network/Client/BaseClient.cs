// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Task;

namespace Tuckfirtle.Node.Network.Client
{
    internal abstract class BaseClient : IDisposable
    {
        private TcpClient TcpClient { get; }

        private StreamReader TcpClientReader { get; }

        private StreamWriter TcpClientWriter { get; }

        private Task ReadPacketFromNetworkTask { get; }

        protected BaseClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            TcpClientReader = new StreamReader(tcpClient.GetStream());
            TcpClientWriter = new StreamWriter(tcpClient.GetStream());

            ReadPacketFromNetworkTask = Task.Factory.StartNew<Task, (StreamReader, Action<string>)>(async state =>
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