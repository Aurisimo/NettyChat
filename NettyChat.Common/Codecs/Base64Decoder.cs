using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;

namespace NettyChat.Common.Codecs
{
    public class Base64Decoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            var decoded = Decode(context, message);
            output.Add(decoded);
        }

        protected IByteBuffer Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            var base64String = input.ToString(Encoding.ASCII);
            var stringBytes = Convert.FromBase64String(base64String);

            var buffer = context.Allocator.Buffer();
            buffer.WriteBytes(stringBytes);
            return buffer;
        }
    }
}
