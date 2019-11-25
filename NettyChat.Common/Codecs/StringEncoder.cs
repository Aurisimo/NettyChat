using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Text;

namespace NettyChat.Common.Codecs
{
    public class StringEncoder : MessageToMessageEncoder<string>
    {
        protected override void Encode(IChannelHandlerContext context, string message, List<object> output)
        {
            output.Add(ByteBufferUtil.EncodeString(context.Allocator, message, Encoding.UTF8));
        }
    }
}
