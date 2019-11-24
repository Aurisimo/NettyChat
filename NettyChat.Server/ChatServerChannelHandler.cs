using System;
using DotNetty.Transport.Channels;

namespace NettyChat.Server
{
    public class ChatServerChannelHandler : SimpleChannelInboundHandler<string>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, string msg)
        {
            Console.WriteLine(msg);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);

            Console.WriteLine($"New client (Id: {context.Channel.Id}, remote address: {context.Channel.RemoteAddress}) connected");
        }
    }
}
