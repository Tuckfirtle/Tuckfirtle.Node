// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Serilog.Events;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network;

namespace Tuckfirtle.Node.Config
{
    internal abstract class Config : IConfig, IDisposable
    {
        public LogEventLevel MinimumLogEventLevel { get; set; } = LogEventLevel.Information;

        public NetworkType NetworkType { get; set; } = NetworkType.Testnet;

        public bool UniversalPlugAndPlay { get; set; } = true;

        public string P2PListenerIp { get; set; } = IPAddress.Any.ToString();

        public int P2PListenerPort { get; set; } = CoreConfiguration.P2PDefaultPort;

        public int P2PMaxConnectionLimit { get; set; } = int.MaxValue;

        public int P2PPingLimit { get; set; } = 1000;

        public List<string> P2PIpBlacklist { get; set; } = new List<string>();

        public string RPCListenerIp { get; set; } = IPAddress.Any.ToString();

        public int RPCListenerPort { get; set; } = CoreConfiguration.RPCDefaultPort;

        [JsonIgnore]
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