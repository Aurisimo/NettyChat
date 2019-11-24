using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Codecs.Base64;
using System.Net;
using DotNetty.Codecs;
using NettyChat.Common;
using DotNetty.Handlers.Logging;
using System.Text;

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

                        pipeline.AddLast(new LoggingHandler(LogLevel.DEBUG));

                        pipeline.AddLast(new DelimiterBasedFrameDecoder(80,
                            //Delimiters.NullDelimiter()
                            Delimiters.LineDelimiter()
                            ));

                        pipeline.AddLast(new Base64Decoder());
                        pipeline.AddLast(new Base64Encoder());

                        pipeline.AddLast(new StringEncoder());
                        pipeline.AddLast(new StringDecoder());
                        pipeline.AddLast(new ChatClientChannelHandler());
                    }));

                var channel = await bootstrap.ConnectAsync(IPAddress.Parse("127.0.0.1"), 50010);

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input == "quit") break;
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
