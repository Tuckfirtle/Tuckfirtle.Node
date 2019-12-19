// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Tasks;

namespace Tuckfirtle.Node.Network
{
    internal abstract class Client : IDisposable
    {
        public event Action<List<ConsoleMessage>> Logger;

        private Task _tcpClientReaderTask;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _isDisposed;

        public IPEndPoint RemoteIpEndPoint => TcpClient?.Client?.RemoteEndPoint as IPEndPoint;

        protected TcpClient TcpClient { get; }

        protected NetworkStream NetworkStream { get; private set; }

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected Client(TcpClient tcpClient = null)
        {
            TcpClient = tcpClient ?? new TcpClient();
        }

        public void Connect(string hostname, int port)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TcpClient));

            if (TcpClient.Connected)
                throw new InvalidOperationException("TcpClient is already connected.");

            BeforeConnect(hostname, port);
            TcpClient.Connect(hostname, port);
            StartReadingPacketsFromNetwork();
            AfterConnect(hostname, port);
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TcpClient));

            if (TcpClient.Connected)
                throw new InvalidOperationException("TcpClient is already connected.");

            BeforeConnect(hostname, port);
            await TcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);
            StartReadingPacketsFromNetwork();
            AfterConnect(hostname, port);
        }

        public void Close()
        {
            if (_isDisposed)
                return;

            BeforeClose();
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_tcpClientReaderTask);
            AfterClose();
            Dispose();
        }

        protected virtual void OnLogger(List<ConsoleMessage> consoleMessages)
        {
            Logger?.Invoke(consoleMessages);
        }

        protected virtual void BeforeConnect(string hostname, int port)
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Connecting to {hostname}:{port}.", ConsoleColor.Cyan).Build());
        }

        protected virtual void AfterConnect(string hostname, int port)
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Connected to {hostname}:{port}.", ConsoleColor.Green).Build());
        }

        protected virtual void BeforeClose()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Connection from {RemoteIpEndPoint} is closing.", ConsoleColor.Cyan).Build());
        }

        protected virtual void AfterClose()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Connection from {RemoteIpEndPoint} is closed.", ConsoleColor.Red).Build());
        }

        protected virtual void BeforeStartReadingPacketsFromNetwork()
        {
        }

        protected abstract Task<bool> ReadPacketsFromNetworkAsync(CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                TcpClient?.Dispose();
                NetworkStream?.Dispose();
                _tcpClientReaderTask?.Dispose();
                _cancellationTokenSource?.Dispose();
            }
        }

        protected void StartReadingPacketsFromNetwork()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TcpClient));

            if (!TcpClient.Connected)
                throw new InvalidOperationException("TcpClient is not connected.");

            if (_tcpClientReaderTask != null)
                return;

            NetworkStream = TcpClient.GetStream();

            BeforeStartReadingPacketsFromNetwork();

            _tcpClientReaderTask = TaskState.Run<(CancellationTokenSource, Action, Func<CancellationToken, Task<bool>>), Task>((_cancellationTokenSource, Dispose, ReadPacketsFromNetworkAsync), async state =>
            {
                var (cancellationTokenSource, dispose, readPacketsFromNetworkAsync) = state;
                var cancellationToken = cancellationTokenSource.Token;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var packetResult = await readPacketsFromNetworkAsync(cancellationToken).ConfigureAwait(false);

                    if (packetResult)
                        continue;

                    cancellationTokenSource.Cancel();
                    dispose();
                }
            }, _cancellationTokenSource.Token).Unwrap();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}