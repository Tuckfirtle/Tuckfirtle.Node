// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Reflection;
using System.Runtime.Versioning;
using TheDialgaTeam.Core.DependencyInjection;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Bootstrap
{
    internal sealed class BootstrapServiceExecutor : IServiceExecutor
    {
        public void ExecuteService(ITaskAwaiter taskAwaiter)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            System.Console.Title = $"{CoreConfiguration.CoinFullName} Node v{version} ({frameworkVersion})";
        }
    }
}