﻿// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;

namespace Tuckfirtle.Node.Config.Json
{
    internal sealed class JsonConfigFactoryInstaller : IFactoryInstaller
    {
        private string ConfigFilePath { get; }

        public JsonConfigFactoryInstaller(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton(new JsonConfig(ConfigFilePath));
            serviceCollection.AddInterfacesAsSingleton<ConfigService>();
        }
    }
}