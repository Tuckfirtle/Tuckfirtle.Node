// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tuckfirtle.Node.Network.Listener
{
    internal sealed class Listener : IDisposable
    {
        public bool IsPendingNewClient => TcpListener?.Pending() ?? false;

        private TcpListener TcpListener { get; }

        public Listener(IPAddress ipAddress, ushort port)
        {
            TcpListener = new TcpListener(ipAddress, port);
        }

        public void StartListener()
        {
            TcpListener.AllowNatTraversal(true);
            TcpListener.Start();
        }

        public void StopListener()
        {
            TcpListener.Stop();
        }

        public async Task<TcpClient> AcceptNewClientAsync()
        {
            return await TcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            StopListener();
        }
    }
}