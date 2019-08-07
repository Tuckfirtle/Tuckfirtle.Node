// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Config
{
    internal interface IConfig
    {
        string ConfigFilePath { get; }

        bool IsConfigFileExist { get; }

        void LoadConfig();

        void SaveConfig();
    }
}