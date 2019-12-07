// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network.Listener
{
    internal abstract class BaseListener : IListener, IDisposable
    {
        public abstract string ListenerType { get; }

        public abstract IPAddress ListenerIpAddress { get; }

        public abstract int ListenerPort { get; }

        private IConfig Config { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private ITaskAwaiter TaskAwaiter { get; }

        private TcpListener TcpListener { get; set; }

        protected BaseListener(IConfig config, IConsoleLogger consoleLogger, ITaskAwaiter taskAwaiter)
        {
            Config = config;
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
                                AcceptTcpClient(tcpClient);
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