// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Config
{
    internal abstract class Config : IConfig, IConfigModel, IDisposable
    {
        public NetworkType NetworkType => ConfigModel.NetworkType;

        public bool UniversalPlugAndPlay => ConfigModel.UniversalPlugAndPlay;

        public string P2PListenerIp => ConfigModel.P2PListenerIp;

        public int P2PListenerPort => ConfigModel.P2PListenerPort;

        public string RPCListenerIp => ConfigModel.RPCListenerIp;

        public int RPCListenerPort => ConfigModel.RPCListenerPort;

        public string ConfigFilePath { get; }

        public bool IsConfigFileExist => File.Exists(ConfigFilePath);

        protected ConfigModel ConfigModel { get; set; } = new ConfigModel();

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