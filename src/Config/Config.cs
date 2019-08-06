// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Config
{
    internal abstract class Config : IConfig, IDisposable
    {
        public NetworkType NetworkType => ConfigModel.NetworkType;

        public bool UniversalPlugAndPlay => ConfigModel.UniversalPlugAndPlay;

        public string P2PListenerIp => ConfigModel.P2PListenerIp;

        public ushort P2PListenerPort => ConfigModel.P2PListenerPort;

        public string RPCListenerIp => ConfigModel.RPCListenerIp;

        public ushort RPCListenerPort => ConfigModel.RPCListenerPort;

        public string ConfigFilePath { get; }

        protected ConfigModel ConfigModel { get; set; } = new ConfigModel();

        protected Config(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public bool IsConfigFileExist()
        {
            return File.Exists(ConfigFilePath);
        }

        public abstract void LoadConfig();

        public abstract void SaveConfig();

        public void Dispose()
        {
            SaveConfig();
        }
    }
}