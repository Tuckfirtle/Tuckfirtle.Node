// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;

namespace Tuckfirtle.Node.Config.Json
{
    internal sealed class JsonConfigServiceInstaller : IServiceInstaller
    {
        private string ConfigFilePath { get; }

        public JsonConfigServiceInstaller(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public void InstallService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton(new JsonConfig(ConfigFilePath));
            serviceCollection.AddInterfacesAsSingleton<ConfigServiceExecutor>();
        }
    }
}