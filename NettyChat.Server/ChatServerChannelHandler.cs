using System;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;

namespace NettyChat.Server
{
    public class ChatServerChannelHandler : SimpleChannelInboundHandler<string>
    {
        private static volatile IChannelGroup _group;
        
        protected override void ChannelRead0(IChannelHandlerContext context, string msg)
        {
            context.WriteAndFlushAsync($"[you] {msg}\n");
            _group.WriteAndFlushAsync($"[{context.Channel.RemoteAddress}] {msg}", new ExcludeChannelMatcher(context.Channel.Id));
            
            if (msg.Equals("quit", StringComparison.InvariantCultureIgnoreCase)) context.Channel.CloseAsync();
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            if (_group == null)
            {
                lock (this)
                {
                    if (_group == null)
                    {
                        _group = new DefaultChannelGroup(context.Executor);
                    }
                }
            }

            _group.Add(context.Channel);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            context.CloseAsync();
        }
    }
}
