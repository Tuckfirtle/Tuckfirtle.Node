// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Network
{
    internal abstract class Listener
    {
        public event Action<List<ConsoleMessage>> Logger;

        public abstract string ListenerType { get; }

        public IPAddress ListenerIpAddress { get; }

        public int ListenerPort { get; }

        protected TcpListener TcpListener { get; }

        protected Listener(IPAddress ipAddress, int port, bool natTraversal = false)
        {
            TcpListener = new TcpListener(ipAddress, port);
            TcpListener.AllowNatTraversal(natTraversal);

            ListenerIpAddress = ipAddress;
            ListenerPort = port;
        }

        public void Start()
        {
            BeforeStart();
            TcpListener.Start();
            AfterStart();
        }

        public void Stop()
        {
            BeforeStop();
            TcpListener.Stop();
            AfterStop();
        }

        protected virtual void BeforeStart()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Initializing {ListenerType} Listener.", ConsoleColor.Cyan).Build());
        }

        protected virtual void AfterStart()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Started listening for {ListenerType} connection.", ConsoleColor.Green).Build());
        }

        protected virtual void BeforeStop()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Stopping listening for {ListenerType} connection.", ConsoleColor.Cyan).Build());
        }

        protected virtual void AfterStop()
        {
            OnLogger(new ConsoleMessageBuilder().WriteLine($"Stopped listening for {ListenerType} connection.", ConsoleColor.Red).Build());
        }

        protected virtual void OnLogger(List<ConsoleMessage> consoleMessages)
        {
            Logger?.Invoke(consoleMessages);
        }
    }
}