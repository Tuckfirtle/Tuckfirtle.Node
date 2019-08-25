// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config.Model;
using Tuckfirtle.Node.Network.Nat;

namespace Tuckfirtle.Node.Network.Listener
{
    internal abstract class BaseListener : IListener, IDisposable
    {
        protected abstract string ListenerType { get; }

        private TcpListener TcpListener { get; set; }

        private List<TcpClient> TcpClients { get; } = new List<TcpClient>();

        public void InitializeListener(IConfigModel configModel, INatDeviceUtility natDeviceUtility)
        {
            TcpListener = new TcpListener(IPAddress.Any, configModel.P2PListenerPort);
            TcpListener.AllowNatTraversal(true);

            natDeviceUtility.AddPortMapping(RequiredPortMapping(configModel));
        }

        public void StartListener(ITaskAwaiter taskAwaiter, IConsoleLogger consoleLogger)
        {
            taskAwaiter.EnqueueTask((TcpListener, consoleLogger), async (cancellationToken, state) =>
            {
                var listener = state.TcpListener;
                
                state.consoleLogger.LogMessage($"Started listening for {ListenerType} connections.", ConsoleColor.Green);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        var tcpClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                        TcpClients.Add(tcpClient);
                        AcceptTcpClient(tcpClient);
                    }

                    try
                    {
                        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            });
        }

        protected abstract int RequiredPortMapping(IConfigModel configModel);

        protected abstract void AcceptTcpClient(TcpClient tcpClient);

        public void Dispose()
        {
            TcpListener?.Stop();

            foreach (var tcpClient in TcpClients)
                tcpClient.Dispose();
        }
    }
}