// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using Open.Nat;
using TheDialgaTeam.Core.DependencyInjection;
using Tuckfirtle.Node.Network.Clients;
using Tuckfirtle.Node.Network.Listeners;

namespace Tuckfirtle.Node.Network
{
    internal class NetworkServiceInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAndSelfAsSingleton<NatDiscoverer>();
            serviceCollection.AddSingleton<P2PClientCollection>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<P2PListener>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<NetworkServiceExecutor>();
        }
    }
}