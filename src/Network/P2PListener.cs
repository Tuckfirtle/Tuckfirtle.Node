// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using TheDialgaTeam.Core.DependencyInjection;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network
{
    internal class P2PListener : Listener
    {
        private readonly ITaskAwaiter _taskAwaiter;

        private readonly IConfig _config;

        public override string ListenerType { get; } = "P2P";

        public P2PListener(ITaskAwaiter taskAwaiter, IConfig config) : base(IPAddress.Parse(config.P2PListenerIp), config.P2PListenerPort, config.UniversalPlugAndPlay)
        {
            _taskAwaiter = taskAwaiter;
            _config = config;
        }

        protected override void AfterStart()
        {
            base.AfterStart();

            _taskAwaiter.EnqueueTask(TcpListener, async (cancellationToken, tcpListener) =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    // TODO: Add tcpclient into a collection which handles this.
                }
            });
        }
    }
}