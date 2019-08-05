// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Config.Model
{
    internal interface IConfig
    {
        NetworkType NetworkType { get; }

        ushort P2PPort { get; }

        ushort RPCPort { get; }
    }
}