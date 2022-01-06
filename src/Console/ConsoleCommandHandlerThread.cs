//// Copyright (C) 2020, The Tuckfirtle Developers
//// 
//// Please see the included LICENSE file for more information.

//using System;
//using System.Threading;
//using Microsoft.Extensions.Hosting;
//using TheDialgaTeam.Core.Thread;

//namespace Tuckfirtle.Node.Console
//{
//    internal class ConsoleCommandHandlerThread : PollingThreadWithObjectState
//    {
//        private readonly Logger _logger;
//        private readonly IHostApplicationLifetime _hostApplicationLifetime;

//        public ConsoleCommandHandlerThread(Logger logger, IHostApplicationLifetime hostApplicationLifetime, CancellationToken cancellationToken) : base(cancellationToken)
//        {
//            _logger = logger;
//            _hostApplicationLifetime = hostApplicationLifetime;
//        }

//        public override void Start()
//        {
//            Thread.IsBackground = true;
//            Thread.Name = nameof(ConsoleCommandHandlerThread);
//            Thread.Priority = ThreadPriority.Normal;

//            base.Start();
//        }

//        protected override void Execute(CancellationToken cancellationToken)
//        {
//            var command = System.Console.ReadLine();

//            if (command == null)
//            {
//                Cancel();
//                return;
//            }

//            command = command.Trim();

//            var logger = _logger;

//            if (command.Equals("Help", StringComparison.OrdinalIgnoreCase))
//            {
//                logger.LogInformation("Help Menu:", false);
//                logger.LogInformation("Exit - Gracefully stop the application and flush all unsaved data into database.", false);
//            }
//            else if (command.Equals("Exit", StringComparison.OrdinalIgnoreCase))
//            {
//                _hostApplicationLifetime.StopApplication();
//            }
//            else
//            {
//                logger.LogInformation("\u001b[31;1mInvalid command.\u001b[0m", false);
//            }
//        }
//    }
//}