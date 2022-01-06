// Copyright (C) 2020, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Tuckfirtle.Node
{
    internal class Logger
    {
        public const string DateTimeTemplate = "\u001b[30;1m{DateTimeOffset:yyyy-MM-dd HH:mm:ss}\u001b[0m";

        private readonly ILogger<Logger> _logger;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;

        public LogEventLevel MinimumLevel
        {
            get => _loggingLevelSwitch.MinimumLevel;
            set => _loggingLevelSwitch.MinimumLevel = value;
        }

        public Logger(ILogger<Logger> logger, LoggingLevelSwitch loggingLevelSwitch)
        {
            _logger = logger;
            _loggingLevelSwitch = loggingLevelSwitch;
        }

        public void LogInformation(string message, bool includeDateTime, params object[] args)
        {
            if (includeDateTime)
            {
                var newArgs = new object[args.Length + 1];
                newArgs[0] = DateTimeOffset.Now;
                Array.Copy(args, 0, newArgs, 1, args.Length);

                _logger.LogInformation($"{DateTimeTemplate} {message}", newArgs);
            }
            else
            {
                _logger.LogInformation(message, args);
            }
        }
    }
}