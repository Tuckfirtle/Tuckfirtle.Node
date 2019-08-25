// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config.Model;
using Tuckfirtle.Node.Network.Nat;

namespace Tuckfirtle.Node.Network.Listener
{
    internal interface IListener
    {
        void InitializeListener(IConfigModel configModel, INatDeviceUtility natDeviceUtility);

        void StartListener(ITaskAwaiter taskAwaiter, IConsoleLogger consoleLogger);
    }
}