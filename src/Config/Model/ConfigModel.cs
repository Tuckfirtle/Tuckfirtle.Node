// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Config.Model
{
    internal sealed class ConfigModel : IConfig
    {
        public NetworkType NetworkType { get; set; } = NetworkType.Testnet;

        public ushort P2PPort { get; set; } = CoreSettings.P2PDefaultPort;

        public ushort RPCPort { get; set; } = CoreSettings.RPCDefaultPort;
    }
}