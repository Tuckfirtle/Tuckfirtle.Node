// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
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

        private List<TcpClient> TcpClients { get; } = new List<TcpClient>();

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
                var listener = state.TcpListener;
                var consoleLogger = state.ConsoleLogger;

                try
                {
                    listener.Start();
                    consoleLogger.LogMessage($"Started listening for {ListenerType} connections.", ConsoleColor.Green);

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

        private async Task<IPAddress> GetPublicIpAddressAsync()
        {
            try
            {
                using var httpClient = new HttpClient();
                
                var request = await httpClient.GetAsync("https://api.ipify.org", new CancellationTokenSource(10000).Token).ConfigureAwait(false);
                request.EnsureSuccessStatusCode();

                var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                return IPAddress.TryParse(response, out var result) ? result : IPAddress.None;
            }
            catch (Exception)
            {
                return IPAddress.None;
            }
        }

        public void Dispose()
        {
            TcpListener?.Stop();

            foreach (var tcpClient in TcpClients)
                tcpClient.Dispose();
        }
    }
}