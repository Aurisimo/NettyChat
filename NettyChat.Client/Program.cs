using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NettyChat.Common.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NettyChat.Client
{
    class Program
    {
        private static async Task StartClientAsync()
        {
            LoggingHelper.SetConsoleLogger();

            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;

                        pipeline.AddLast(new LoggingHandler((LogLevel.INFO)));
                        
                        pipeline.AddLast(new Common.Codecs.Base64Encoder());
                        pipeline.AddLast(new Common.Codecs.Base64Decoder());

                        pipeline.AddLast(new Common.Codecs.StringEncoder());
                        pipeline.AddLast(new Common.Codecs.StringDecoder());

                        pipeline.AddLast(new ChatClientChannelHandler());
                    }));

                var channel = await bootstrap.ConnectAsync(IPAddress.Parse("127.0.0.1"), 50010);

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input.Equals("quit", StringComparison.InvariantCultureIgnoreCase)) break;
                    else if (!string.IsNullOrEmpty(input))
                    {
                        await channel.WriteAndFlushAsync(input + "\r\n");
                    }
                }

                await channel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        static void Main(string[] args)
        {
            try
            {
                StartClientAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client crashed:\n{e.Message}");
            }
        }
    }
}
