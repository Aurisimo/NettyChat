using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Logging;

namespace NettyChat.Common.Logging
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
