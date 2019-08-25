// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Network.Nat
{
    internal interface INatDeviceUtility
    {
        void AddPortMapping(int port);

        Task OpenPortsAsync(IConsoleLogger consoleLogger);
    }
}