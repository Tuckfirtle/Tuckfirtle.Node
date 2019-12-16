// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Node.Network
{
    internal interface IListener
    {
        event Action<List<ConsoleMessage>> Logger;

        string ListenerType { get; }

        IPAddress ListenerIpAddress { get; }

        int ListenerPort { get; }

        void Start();

        void Stop();
    }
}