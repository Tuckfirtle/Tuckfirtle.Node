// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;

namespace Tuckfirtle.Node.Config.Json
{
    internal class JsonConfigServiceInstaller : IServiceInstaller
    {
        private readonly string _configFilePath;

        public JsonConfigServiceInstaller(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public void InstallService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAndSelfAsSingleton(new JsonConfig(_configFilePath));
        }
    }
}