// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;
using Tuckfirtle.Node.Network.Listener;
using Tuckfirtle.Node.Network.Nat;

namespace Tuckfirtle.Node.Network
{
    internal sealed class NetworkFactoryInstaller : IFactoryInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton<NatDeviceUtility>();
            serviceCollection.AddInterfacesAsSingleton<P2PListener>();
            serviceCollection.AddInterfacesAsSingleton<RPCListener>();
            serviceCollection.AddInterfacesAsSingleton<NetworkService>();
        }
    }
}