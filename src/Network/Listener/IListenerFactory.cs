// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Node.Network.Listener
{
    internal interface IListenerFactory
    {
        Listener CreateP2PListener();

        Listener CreateRPCListener();
    }
}