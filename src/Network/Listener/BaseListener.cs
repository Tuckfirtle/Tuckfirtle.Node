// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Tuckfirtle.Node.Network.Listener
{
    internal abstract class BaseListener : IListener, IDisposable
    {
        public abstract string ListenerType { get; }

        public abstract IPAddress ListenerIpAddress { get; }

        public abstract int ListenerPort { get; }

        protected IConsoleLogger ConsoleLogger { get; }

        private ITaskAwaiter TaskAwaiter { get; }

        private TcpListener TcpListener { get; set; }

        protected BaseListener(IConsoleLogger consoleLogger, ITaskAwaiter taskAwaiter)
        {
            ConsoleLogger = consoleLogger;
            TaskAwaiter = taskAwaiter;
        }

        public void StartListener()
        {
            TaskAwaiter.EnqueueTask((TcpListener, ConsoleLogger), async (cancellationToken, state) =>
            {
                var (listener, consoleLogger) = state;

                try
                {
                    listener.Start();
                    consoleLogger.LogMessage($"Started listening for {ListenerType} connections.", ConsoleColor.Green);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            if (listener.Pending())
                            {
                                var tcpClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                                _ = Task.Factory.StartNew<(TcpClient tcpClient, Action<TcpClient> acceptTcpClient)>(innerState => { innerState.acceptTcpClient(innerState.tcpClient); }, (tcpClient, AcceptTcpClient), cancellationToken);
                            }

                            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    }
                }
                catch (SocketException)
                {
                    consoleLogger.LogMessage($"Unable to start {ListenerType} listener.", ConsoleColor.Red);
                }
            });
        }

        protected void Initialize()
        {
            TcpListener = new TcpListener(ListenerIpAddress, ListenerPort);
            TcpListener.AllowNatTraversal(true);
        }

        protected abstract void AcceptTcpClient(TcpClient tcpClient);

        public void Dispose()
        {
            TcpListener?.Stop();
        }
    }
}