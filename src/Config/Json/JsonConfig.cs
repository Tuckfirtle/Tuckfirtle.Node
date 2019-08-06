// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.IO;
using Newtonsoft.Json;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Config.Json
{
    internal sealed class JsonConfig : Config
    {
        public JsonConfig(string configFilePath) : base(configFilePath)
        {
        }

        public override void LoadConfig()
        {
            using (var streamReader = new StreamReader(new FileStream(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer();
                ConfigModel = jsonSerializer.Deserialize<ConfigModel>(new JsonTextReader(streamReader));
            }
        }

        public override void SaveConfig()
        {
            using (var streamWriter = new StreamWriter(new FileStream(ConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
                jsonSerializer.Serialize(streamWriter, ConfigModel);
            }
        }
    }
}