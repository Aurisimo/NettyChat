using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;

namespace NettyChat.Common
{
    public class LoggingHelper
    {
        public static void SetConsoleLogger()
        {
            InternalLoggerFactory.DefaultFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
        }
    }
}
