﻿// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Config
{
    internal interface IConfig
    {
        NetworkType NetworkType { get; }

        bool UniversalPlugAndPlay { get; }

        string P2PListenerIp { get; }

        int P2PListenerPort { get; }

        string RPCListenerIp { get; }

        int RPCListenerPort { get; }

        string ConfigFilePath { get; }

        void LoadConfig();

        void SaveConfig();
    }
}