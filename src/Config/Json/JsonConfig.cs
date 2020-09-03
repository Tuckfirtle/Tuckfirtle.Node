// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.IO;
using Newtonsoft.Json;

namespace Tuckfirtle.Node.Config.Json
{
    internal class JsonConfig : Config
    {
        private readonly JsonSerializer _jsonSerializer;

        public JsonConfig(string configFilePath) : base(configFilePath)
        {
            _jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        public override void LoadConfig()
        {
            using var streamReader = new StreamReader(new FileStream(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            _jsonSerializer.Populate(new JsonTextReader(streamReader), this);
        }

        public override void SaveConfig()
        {
            using var streamWriter = new StreamWriter(new FileStream(ConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
            _jsonSerializer.Serialize(streamWriter, this);
        }
    }
}