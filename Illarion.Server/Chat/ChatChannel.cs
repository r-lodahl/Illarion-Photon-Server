using System;
using System.Collections.Generic;
using System.Numerics;

namespace Illarion.Server.Chat
{
    internal abstract class ChatChannel : IChatChannel
    {
        private readonly List<IChatSubscriber> _subscribers;

        protected ChatChannel() => _subscribers = new List<IChatSubscriber>();

        public void Subscribe(IChatSubscriber subscriber) => _subscribers.Add(subscriber);

        public void Chat(IChatSubscriber sender, IChatMessageEventUpdate message)
        {
            foreach (IChatSubscriber subscriber in _subscribers)
            {
                var distance = Vector3.Distance(sender.Location, subscriber.Location);
                if (distance <= MaximalChatDistance())
                {
                    subscriber.ReceiveMessage(message);
                }
            }
        }

        protected abstract float MaximalChatDistance();

        public static ChatChannel GetNewChatChannel(MapChatChannelType channel)
        {
            switch (channel)
            {
                case MapChatChannelType.Global:
                    return new GlobalChatChannel();
                case MapChatChannelType.Speaking:
                    return new SpeakingChatChannel();
                case MapChatChannelType.Yelling:
                    return new YellingChatChannel();
                case MapChatChannelType.Whispering:
                    return new WhisperingChatChannel();
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, "Channel type does not exist!");
            }
        }

    }
}
