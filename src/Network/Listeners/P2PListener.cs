// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Tasks;
using Tuckfirtle.Node.Config;
using Tuckfirtle.Node.Network.Clients;

namespace Tuckfirtle.Node.Network.Listeners
{
    internal class P2PListener : Listener
    {
        private readonly ITaskAwaiter _taskAwaiter;

        private readonly IConfig _config;

        private readonly P2PClientCollection _clientCollection;

        public override string ListenerType { get; } = "P2P";

        public P2PListener(ITaskAwaiter taskAwaiter, IConfig config, P2PClientCollection clientCollection) : base(IPAddress.Parse(config.P2PListenerIp), config.P2PListenerPort, config.UniversalPlugAndPlay)
        {
            _taskAwaiter = taskAwaiter;
            _config = config;
            _clientCollection = clientCollection;
        }

        protected override void AfterStart()
        {
            base.AfterStart();

            _taskAwaiter.EnqueueTask(TcpListener, async (cancellationToken, tcpListener) =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    
                    TaskState.RunAndForget((tcpClient, _clientCollection, _config), state =>
                    {
                        var (stateTcpClient, clientCollection, config) = state;

                        if (!clientCollection.TryAddNewPeer(new P2PClient(config, stateTcpClient)))
                            stateTcpClient.Close();
                    }, cancellationToken);
                }
            });
        }
    }
}