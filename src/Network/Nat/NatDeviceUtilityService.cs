// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config.Model;

namespace Tuckfirtle.Node.Network.Nat
{
    internal sealed class NatDeviceUtilityService : IServiceExecutor
    {
        private ITaskAwaiter TaskAwaiter { get; }

        private IConfigModel ConfigModel { get; }

        private INatDeviceUtility NatDeviceUtility { get; }

        private IConsoleLogger ConsoleLogger { get; }

        public NatDeviceUtilityService(ITaskAwaiter taskAwaiter, IConfigModel configModel, INatDeviceUtility natDeviceUtility, IConsoleLogger consoleLogger)
        {
            TaskAwaiter = taskAwaiter;
            ConfigModel = configModel;
            NatDeviceUtility = natDeviceUtility;
            ConsoleLogger = consoleLogger;
        }

        public void Execute()
        {
            if (!ConfigModel.UniversalPlugAndPlay)
                return;

            TaskAwaiter.EnqueueTask((ConsoleLogger, NatDeviceUtility), async (cancellationToken, state) =>
            {
                var consoleLogger = state.ConsoleLogger;

                try
                {
                    consoleLogger.LogMessage("Locating NAT devices...");
                    await state.NatDeviceUtility.DiscoverDeviceAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    consoleLogger.LogMessage("No NAT device found. Please ensure that you have a NAT device that support universal plug and play and is enabled.", ConsoleColor.Red);
                }
            }).Wait();
        }
    }
}