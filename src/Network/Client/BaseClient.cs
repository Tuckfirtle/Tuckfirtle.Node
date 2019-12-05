// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Network.Client
{
    internal abstract class BaseClient : IDisposable
    {
        private IConsoleLogger ConsoleLogger { get; }

        private TcpClient TcpClient { get; }

        private StreamReader TcpClientReader { get; }

        private StreamWriter TcpClientWriter { get; }

        private Task ReadPacketFromNetworkTask { get; }

        protected BaseClient(TcpClient tcpClient, IConsoleLogger consoleLogger)
        {
            ConsoleLogger = consoleLogger;
            TcpClient = tcpClient;
            TcpClientReader = new StreamReader(tcpClient.GetStream());
            TcpClientWriter = new StreamWriter(tcpClient.GetStream());

            ReadPacketFromNetworkTask = Task.Factory.StartNew(async state =>
            {
                if (TcpClientReader.BaseStream is NetworkStream networkStream)
                {
                    while (true)
                    {
                        try
                        {
                            if (networkStream.DataAvailable)
                            {
                                //var packet = await OnReceivePacketFromNetworkAsync(TcpClientReader).ConfigureAwait(false);
                                //_ = Task.Run(async () => await OnHandlePacketFromNetworkAsync(packet).ConfigureAwait(false));
                            }

                            await Task.Delay(1).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }, consoleLogger).Unwrap();
        }

        protected abstract Task OnHandlePacketFromNetworkAsync(string packet);

        public void Dispose()
        {
            TcpClient?.Dispose();
            TcpClientReader?.Dispose();
            TcpClientWriter?.Dispose();
        }
    }
}