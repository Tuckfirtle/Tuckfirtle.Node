// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Config
{
    internal abstract class Config : IConfig, IDisposable
    {
        public NetworkType NetworkType { get; set; } = NetworkType.Testnet;

        public bool UniversalPlugAndPlay { get; set; } = true;

        public string P2PListenerIp { get; set; } = IPAddress.Any.ToString();

        public int P2PListenerPort { get; set; } = CoreConfiguration.P2PDefaultPort;

        public string RPCListenerIp { get; set; } = IPAddress.Any.ToString();

        public int RPCListenerPort { get; set; } = CoreConfiguration.RPCDefaultPort;

        public string ConfigFilePath { get; }

        protected Config(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public abstract void LoadConfig();

        public abstract void SaveConfig();

        public void Dispose()
        {
            SaveConfig();
        }
    }
}