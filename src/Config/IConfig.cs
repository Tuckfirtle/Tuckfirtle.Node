// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using Serilog.Events;
using Tuckfirtle.Core.Network;

namespace Tuckfirtle.Node.Config
{
    internal interface IConfig
    {
        LogEventLevel MinimumLogEventLevel { get; }

        NetworkType NetworkType { get; }

        bool UniversalPlugAndPlay { get; }

        string P2PListenerIp { get; }

        int P2PListenerPort { get; }

        int P2PMaxConnectionLimit { get; }

        int P2PPingLimit { get; }

        List<string> P2PIpBlacklist { get; }

        string RPCListenerIp { get; }

        int RPCListenerPort { get; }

        string ConfigFilePath { get; }

        void LoadConfig();

        void SaveConfig();
    }
}