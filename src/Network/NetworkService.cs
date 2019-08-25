// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Linq;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config.Model;
using Tuckfirtle.Node.Network.Listener;
using Tuckfirtle.Node.Network.Nat;

namespace Tuckfirtle.Node.Network
{
    internal sealed class NetworkService : IServiceExecutor
    {
        private ITaskAwaiter TaskAwaiter { get; }

        private IEnumerable<IListener> Listeners { get; }

        private IConfigModel ConfigModel { get; }

        private INatDeviceUtility NatDeviceUtility { get; }

        private IConsoleLogger ConsoleLogger { get; }

        public NetworkService(ITaskAwaiter taskAwaiter, IEnumerable<IListener> listeners, IConfigModel configModel, INatDeviceUtility natDeviceUtility, IConsoleLogger consoleLogger)
        {
            TaskAwaiter = taskAwaiter;
            Listeners = listeners;
            ConfigModel = configModel;
            NatDeviceUtility = natDeviceUtility;
            ConsoleLogger = consoleLogger;
        }

        public void Execute()
        {
            TaskAwaiter.EnqueueTask((Listeners, ConfigModel, NatDeviceUtility, ConsoleLogger, TaskAwaiter), async (cancellationToken, state) =>
            {
                var listeners = state.Listeners.ToArray();
                var configModel = state.ConfigModel;
                var natDeviceUtility = state.NatDeviceUtility;
                var consoleLogger = state.ConsoleLogger;
                var taskAwaiter = state.TaskAwaiter;

                foreach (var listener in listeners)
                    listener.InitializeListener(configModel, natDeviceUtility);

                if (configModel.UniversalPlugAndPlay)
                    await natDeviceUtility.OpenPortsAsync(consoleLogger).ConfigureAwait(false);

                foreach (var listener in listeners)
                    listener.StartListener(taskAwaiter, consoleLogger);
            });
        }
    }
}