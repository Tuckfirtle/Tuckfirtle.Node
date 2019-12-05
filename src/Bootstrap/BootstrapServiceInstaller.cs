// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;

namespace Tuckfirtle.Node.Bootstrap
{
    internal sealed class BootstrapServiceInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton<BootstrapServiceExecutor>();
        }
    }
}