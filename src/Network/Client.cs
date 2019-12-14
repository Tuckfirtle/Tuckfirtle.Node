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

        private readonly TcpClient _tcpClient;

        private NetworkStream _tcpNetworkStream;

        private Task _tcpClientReaderTask;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _isDisposed;

        public IPEndPoint RemoteIpEndPoint => _tcpClient?.Client?.RemoteEndPoint as IPEndPoint;

        protected Client(TcpClient tcpClient = null)
        {
            _tcpClient = tcpClient ?? new TcpClient();

            if (_tcpClient.Connected)
                StartReadingPacketsFromNetwork();
        }

        public void Connect(string hostname, int port)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(_tcpClient));

            if (_tcpClient.Connected)
                throw new InvalidOperationException("TcpClient is already connected.");

            BeforeConnect(hostname, port);
            _tcpClient.Connect(hostname, port);
            StartReadingPacketsFromNetwork();
            AfterConnect(hostname, port);
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(_tcpClient));

            if (_tcpClient.Connected)
                throw new InvalidOperationException("TcpClient is already connected.");

            BeforeConnect(hostname, port);
            await _tcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);
            StartReadingPacketsFromNetwork();
            AfterConnect(hostname, port);
        }

        public void Close()
        {
            BeforeClose();
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_tcpClientReaderTask);
            Dispose();
            AfterClose();
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

        protected virtual void SetNetworkStreamTimeout(NetworkStream networkStream)
        {
        }

        protected virtual void BeforeStartReadingPacketsFromNetwork(NetworkStream networkStream)
        {
        }

        protected abstract Task<bool> ReadPacketsFromNetworkAsync(NetworkStream networkStream);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                _tcpClient?.Dispose();
                _tcpNetworkStream?.Dispose();
                _tcpClientReaderTask?.Dispose();
                _cancellationTokenSource?.Dispose();
            }
        }

        private void StartReadingPacketsFromNetwork()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(_tcpClient));

            if (!_tcpClient.Connected)
                throw new InvalidOperationException("TcpClient is not connected.");

            if (_tcpClientReaderTask != null)
                return;

            _tcpNetworkStream = _tcpClient.GetStream();

            if (_tcpNetworkStream.CanTimeout)
                SetNetworkStreamTimeout(_tcpNetworkStream);

            BeforeStartReadingPacketsFromNetwork(_tcpNetworkStream);

            _tcpClientReaderTask = TaskState.Run<(NetworkStream, CancellationTokenSource, Action, Func<NetworkStream, Task<bool>>), Task>((_tcpNetworkStream, _cancellationTokenSource, Dispose, ReadPacketsFromNetworkAsync), async state =>
            {
                var (networkStream, cancellationTokenSource, dispose, readPacketsFromNetworkAsync) = state;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var packetResult = await readPacketsFromNetworkAsync(networkStream).ConfigureAwait(false);

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