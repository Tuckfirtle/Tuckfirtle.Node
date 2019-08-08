﻿// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Threading.Tasks;
using Open.Nat;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.DependencyInjection.TaskAwaiter;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Node.Config.Model;
using Tuckfirtle.Node.Network.Nat;

namespace Tuckfirtle.Node.Network.Listener.P2P
{
    internal sealed class P2PListenerService : IServiceExecutor, IDisposable
    {
        private ITaskAwaiter TaskAwaiter { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private IConfigModel ConfigModel { get; }

        private INatDeviceUtility NatDeviceUtility { get; }

        private IListenerFactory ListenerFactory { get; }

        private Listener Listener { get; set; }

        public P2PListenerService(ITaskAwaiter taskAwaiter, IConsoleLogger consoleLogger, IConfigModel configModel, INatDeviceUtility natDeviceUtility, IListenerFactory listenerFactory)
        {
            TaskAwaiter = taskAwaiter;
            ConsoleLogger = consoleLogger;
            ConfigModel = configModel;
            NatDeviceUtility = natDeviceUtility;
            ListenerFactory = listenerFactory;
        }

        public void Execute()
        {
            TaskAwaiter.EnqueueTask((ListenerFactory, ConfigModel, ConsoleLogger, NatDeviceUtility), async (cancellationToken, state) =>
            {
                var configModel = state.ConfigModel;
                var consoleLogger = state.ConsoleLogger;

                consoleLogger.LogMessage("Initializing P2P Listener...");

                if (configModel.UniversalPlugAndPlay)
                {
                    try
                    {
                        await state.NatDeviceUtility.OpenPortAsync(configModel.P2PListenerPort).ConfigureAwait(false);
                        consoleLogger.LogMessage($"Successfully open port {configModel.P2PListenerPort} for this session. If you are on multiple NAT devices, this may not work.", ConsoleColor.Green);
                    }
                    catch (NatDeviceNotFoundException)
                    {
                        consoleLogger.LogMessage($"Automatic port mapping failed. You are required to open port {configModel.P2PListenerPort} manually to communicate with other peers.", ConsoleColor.Red);
                    }
                    catch (MappingException)
                    {
                        consoleLogger.LogMessage($"Unable to open port {configModel.P2PListenerPort} for this session. Perhaps it is already open by another node on the same NAT device.", ConsoleColor.Yellow);
                    }
                }

                Listener = state.ListenerFactory.CreateP2PListener();
                Listener.StartListener();

                var listener = Listener;

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (listener.IsPendingNewClient)
                    {
                        var newClient = await listener.AcceptNewClientAsync().ConfigureAwait(false);
                        // TODO: What to do with the new client?
                        newClient.Dispose();
                    }

                    try
                    {
                        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            });
        }

        public void Dispose()
        {
            Listener?.Dispose();
        }
    }
}