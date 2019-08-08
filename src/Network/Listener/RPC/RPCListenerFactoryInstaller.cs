// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;

namespace Tuckfirtle.Node.Network.Listener.RPC
{
    internal sealed class RPCListenerFactoryInstaller : IFactoryInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton<RPCListenerService>();
        }
    }
}