// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Config.Model
{
    internal interface IConfig
    {
        NetworkType NetworkType { get; }

        bool UniversalPlugAndPlay { get; }

        string P2PListenerIp { get; }

        ushort P2PListenerPort { get; }

        string RPCListenerIp { get; }

        ushort RPCListenerPort { get; }
    }
}