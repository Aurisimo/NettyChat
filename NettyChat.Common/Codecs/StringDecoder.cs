using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Text;

namespace NettyChat.Common.Codecs
{
    public class StringDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            var decoded = Decode(message);
            output.Add(decoded);
        }

        protected internal string Decode(IByteBuffer message)
        {
            return message.ToString(Encoding.UTF8);
        }
    }
}
