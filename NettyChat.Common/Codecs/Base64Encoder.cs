using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;

namespace NettyChat.Common.Codecs
{
    public class Base64Encoder : MessageToMessageEncoder<IByteBuffer>
    {
        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            var encoded = Encode(context, message); 
            output.Add(encoded);
        }

        protected internal IByteBuffer Encode(IChannelHandlerContext context, IByteBuffer input)
        {
            var bytes = new byte[input.ReadableBytes];
            input.ReadBytes(bytes);

            var base64String = Convert.ToBase64String(bytes);
            return  ByteBufferUtil.EncodeString(context.Allocator, base64String, Encoding.ASCII);
        }
    }
}
