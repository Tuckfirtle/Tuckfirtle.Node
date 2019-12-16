// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config;

namespace Tuckfirtle.Node.Network.Clients
{
    internal class P2PClientCollection
    {
        private readonly IConfig _config;

        private readonly IConsoleLogger _consoleLogger;

        public List<P2PClient> PeersConnected { get; } = new List<P2PClient>();

        public P2PClientCollection(IConfig config, IConsoleLogger consoleLogger)
        {
            _config = config;
            _consoleLogger = consoleLogger;
        }

        public bool TryAddNewPeer(P2PClient client)
        {
            _consoleLogger.LogMessage($"[IN {client.RemoteIpEndPoint.Address}] Incoming peer connection.", ConsoleColor.Cyan);

            if (PeersConnected.Count + 1 > _config.P2PMaxConnectionLimit)
                return false;

            if (_config.P2PIpBlacklist.Contains(client.RemoteIpEndPoint.Address.ToString()))
                return false;

            try
            {
                using var ping = new Ping();
                ping.Send(client.RemoteIpEndPoint.Address, _config.P2PPingLimit);
            }
            catch (PingException)
            {
                return false;
            }

            PeersConnected.Add(client);
            return true;
        }
    }
}