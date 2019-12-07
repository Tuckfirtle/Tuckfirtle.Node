// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Network.Client
{
    internal enum ConnectionStatus
    {
        /// <summary>
        /// Connection is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connection is disconnecting.
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Connection is connecting.
        /// </summary>
        Connecting,

        /// <summary>
        /// Connection is connected.
        /// </summary>
        Connected
    }
}