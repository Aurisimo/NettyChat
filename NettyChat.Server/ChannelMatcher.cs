using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;

namespace NettyChat.Server
{
    class ExcludeChannelMatcher : IChannelMatcher
    {
        private readonly IChannelId _id;

        public ExcludeChannelMatcher(IChannelId idOfExcludedChannel)
        {
            _id = idOfExcludedChannel;
        }
        
        public bool Matches(IChannel channel)
        {
            return _id != channel.Id;
        }
    }
}
