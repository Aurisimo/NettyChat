using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NettyChat.Common.Logging;
using System;
using System.Threading.Tasks;

namespace NettyChat.Server
{
    class Program
    {
        private static async Task StartServerAsync()
        {
            LoggingHelper.SetConsoleLogger();

            var parentGroup = new MultithreadEventLoopGroup();
            var childGroup = new MultithreadEventLoopGroup();

            try
            {
                var server = new ServerBootstrap();
                server
                    .Group(parentGroup, childGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new LoggingHandler(LogLevel.INFO))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;

                        pipeline.AddLast(new Common.Codecs.Base64Encoder());
                        pipeline.AddLast(new Common.Codecs.Base64Decoder());

                        pipeline.AddLast(new Common.Codecs.StringEncoder());
                        pipeline.AddLast(new Common.Codecs.StringDecoder());
 
                        pipeline.AddLast(new ChatServerChannelHandler());
                    }));

                var channel = await server.BindAsync(50010);

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input.Equals("quit", StringComparison.InvariantCultureIgnoreCase)) break;
                }

                await channel.CloseAsync();
            }
            finally
            {
                Task.WaitAll(parentGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    childGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
        
        static void Main(string[] args)
        {
            try
            {
                StartServerAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server crashed:\n{e.Message}");
            }
        }
    }
}
