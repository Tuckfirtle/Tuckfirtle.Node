// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Threading.Tasks;

namespace Tuckfirtle.Node.Network.Nat
{
    public interface INatDeviceUtility
    {
        Task OpenPortAsync(ushort port);
    }
}