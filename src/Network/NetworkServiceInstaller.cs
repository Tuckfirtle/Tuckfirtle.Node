// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using Open.Nat;
using TheDialgaTeam.Core.DependencyInjection;
using Tuckfirtle.Node.Network.Client;
using Tuckfirtle.Node.Network.Listener;

namespace Tuckfirtle.Node.Network
{
    internal sealed class NetworkServiceInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAndSelfAsSingleton<NatDiscoverer>();
            serviceCollection.AddSingleton<P2PClientCollection>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<P2PListener>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<RPCListener>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<NetworkServiceExecutor>();
        }
    }
}