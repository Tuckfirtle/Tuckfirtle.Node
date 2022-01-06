// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tuckfirtle.Core.Utility;

namespace Tuckfirtle.Node.Network.P2P
{
    internal sealed class PeerListener : IDisposable
    {
        private readonly Configuration _configuration;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TcpListener _tcpListener;

        private bool _isActive;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _acceptTcpClientTask;

        private readonly List<Peer> _peers = new List<Peer>();

        public PeerListener(Configuration configuration, IHostApplicationLifetime hostApplicationLifetime)
        {
            _configuration = configuration;
            _hostApplicationLifetime = hostApplicationLifetime;

            _tcpListener = new TcpListener(IPAddress.Parse(configuration.P2PListenerAddress), configuration.P2PPort);
            _tcpListener.AllowNatTraversal(true);
        }

        public async Task StartAsync()
        {
            if (_isActive) return;
            _isActive = true;

            _cancellationTokenSource = new CancellationTokenSource();

            _tcpListener.Start();
            _acceptTcpClientTask = await Task.Factory.StartNew(WaitForTcpClientTask, _hostApplicationLifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task StopAsync()
        {
            if (!_isActive) return;
            _isActive = false;

            _tcpListener.Stop();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            if (_acceptTcpClientTask != null) await _acceptTcpClientTask;
        }

        private async Task WaitForTcpClientTask()
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, _cancellationTokenSource!.Token);
            var cancellationToken = cancellationTokenSource.Token;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                while (!_tcpListener.Pending())
                {
                    await Task.Delay(1, cancellationToken);
                }

                var acceptTcpClientTask = _tcpListener.AcceptTcpClientAsync();
                var tcpClient = await TaskUtility.WaitUntilCancellation(acceptTcpClientTask, cancellationToken);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _acceptTcpClientTask?.Dispose();
        }
    }
}