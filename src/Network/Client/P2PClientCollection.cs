//// Copyright (C) 2019, The Tuckfirtle Developers
//// 
//// Please see the included LICENSE file for more information.

//using System.Collections.Generic;
//using System.Net.NetworkInformation;
//using TheDialgaTeam.Core.Logger;
//using Tuckfirtle.Node.Config;

//namespace Tuckfirtle.Node.Network.Client
//{
//    internal sealed class P2PClientCollection
//    {
//        public List<P2PClient> PeersConnected { get; }

//        private IConsoleLogger ConsoleLogger { get; }

//        private IConfig Config { get; }

//        public P2PClientCollection(IConsoleLogger consoleLogger, IConfig config)
//        {
//            ConsoleLogger = consoleLogger;
//            Config = config;
//            PeersConnected = new List<P2PClient>();
//        }

//        public bool TryAddPeer(P2PClient peer)
//        {
//            if (PeersConnected.Count + 1 > Config.P2PMaxConnectionLimit)
//                return false;

//            if (Config.P2PIpBlacklist.Contains(peer.PublicIpAddress.Address.ToString()))
//                return false;

//            try
//            {
//                using var ping = new Ping();
//                var pingResponse = ping.Send(peer.PublicIpAddress.Address, Config.P2PPingLimit + 1000);

//                if (pingResponse?.RoundtripTime > Config.P2PPingLimit)
//                    return false;
//            }
//            catch (PingException)
//            {
//            }

//            PeersConnected.Add(peer);
//            return true;
//        }

//        public void BroadcastPacketToAllPeers(string packet)
//        {
//            // TODO: Reserve for broadcasting to all connected peers.
//        }
//    }
//}