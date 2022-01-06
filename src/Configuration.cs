// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Tuckfirtle.Core;
using Tuckfirtle.Core.Network;

namespace Tuckfirtle.Node
{
    internal sealed class Configuration
    {
        public NetworkType NetworkType { get; }

        public bool UseUniversalPlugAndPlay { get; }

        public string P2PListenerAddress { get; }

        public int P2PPort { get; }

        public int P2PMaxConnectionLimit { get; }

        public int P2PMaxPingLimit { get; }

        public string[] P2PBlackListAddresses { get; }

        public string RPCListenerAddress { get; }

        public int RPCPort { get; }

        public string DatabaseLocation { get; }

        public Configuration(IConfiguration configuration)
        {
            NetworkType = configuration.GetValue("Node:NetworkType", NetworkType.Mainnet);

            UseUniversalPlugAndPlay = configuration.GetValue("Node:UseUniversalPlugAndPlay", true);

            P2PListenerAddress = configuration.GetValue("Node:P2P:ListenerAddress", IPAddress.Any.ToString());
            P2PPort = configuration.GetValue("Node:P2P:Port", CoreConfiguration.P2PDefaultPort);
            P2PMaxConnectionLimit = configuration.GetValue("Node:P2P:MaxConnectionLimit", -1);
            P2PMaxPingLimit = configuration.GetValue("Node:P2P:MaxPingLimit", 1000);

            var p2pBlackListAddresses = new List<string>();
            var p2pBlackListAddressesConfigurationSection = configuration.GetSection("Node:P2P:BlackListAddresses");

            foreach (var configurationSection in p2pBlackListAddressesConfigurationSection.GetChildren())
            {
                p2pBlackListAddresses.Add(configurationSection.Value);
            }

            P2PBlackListAddresses = p2pBlackListAddresses.ToArray();

            RPCListenerAddress = configuration.GetValue("Node:RPC:ListenerAddress", IPAddress.Any.ToString());
            RPCPort = configuration.GetValue("Node:RPC:Port", 15081);

            var defaultDatabaseLocation = Path.Combine(Environment.CurrentDirectory, "data");
            DatabaseLocation = configuration.GetValue("Node:Database:Location", defaultDatabaseLocation);
        }
    }
}