using System;
using DotNetty.Transport.Channels;

namespace NettyChat.Client
{
    public class ChatClientChannelHandler : SimpleChannelInboundHandler<string>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
