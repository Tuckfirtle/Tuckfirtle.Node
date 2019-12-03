// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Net;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Config.Model
{
    internal sealed class ConfigModel : IConfigModel
    {
        public NetworkType NetworkType { get; set; } = NetworkType.Testnet;

        public bool UniversalPlugAndPlay { get; set; } = true;

        public string P2PListenerIp { get; set; } = IPAddress.Any.ToString();

        public int P2PListenerPort { get; set; } = CoreConfiguration.P2PDefaultPort;

        public string RPCListenerIp { get; set; } = IPAddress.Any.ToString();

        public int RPCListenerPort { get; set; } = CoreConfiguration.RPCDefaultPort;
    }
}