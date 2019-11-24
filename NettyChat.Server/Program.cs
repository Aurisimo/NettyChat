using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Logging;
using DotNetty.Codecs.Base64;
using DotNetty.Codecs;
using NettyChat.Common;

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
                    .Handler(new LoggingHandler(LogLevel.DEBUG))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new DelimiterBasedFrameDecoder(80, 
                            //Delimiters.NullDelimiter()
                            Delimiters.LineDelimiter()
                            ));

                        pipeline.AddLast(new Base64Decoder());
                        pipeline.AddLast(new Base64Encoder());
                        
                        pipeline.AddLast(new StringEncoder());
                        pipeline.AddLast(new StringDecoder());
                        pipeline.AddLast(new ChatServerChannelHandler());
                    }));

                var channel = await server.BindAsync(50010);

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input == "quit") break;
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
