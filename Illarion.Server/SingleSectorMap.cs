using System;
using System.Collections.Generic;
using System.Numerics;
using Illarion.Server.Chat;
using Illarion.Server.Event;

namespace Illarion.Server
{
    internal class SingleSectorMap : IMap
    {
        #region Channels

        private readonly Dictionary<MapEventChannelType, ITimedEventChannel> _timedEventChannels =
            new Dictionary<MapEventChannelType, ITimedEventChannel>();

        private readonly Dictionary<MapEventChannelType, IEventChannel> _eventChannels =
            new Dictionary<MapEventChannelType, IEventChannel>();

        private readonly Dictionary<MapChatChannelType, IChatChannel> _chatChannels =
            new Dictionary<MapChatChannelType, IChatChannel>(4);

        IChatChannel IMap.GetChatChannel(MapChatChannelType channelType)
        {
            if (_chatChannels.TryGetValue(channelType, out IChatChannel channel)) return channel;
            channel = ChatChannel.GetNewChatChannel(channelType);
            _chatChannels.Add(channelType, channel);
            return channel;
        }

        IEventChannel IMap.GetEventChannel(MapEventChannelType channelType)
        {
            if (_eventChannels.TryGetValue(channelType, out IEventChannel channel)) return channel;
            channel = new EventChannel(channelType);
            _eventChannels.Add(channelType, channel);
            return channel;
        }

        ITimedEventChannel IMap.GetTimedEventChannel(MapEventChannelType channelType)
        {
            if (_timedEventChannels.TryGetValue(channelType, out ITimedEventChannel channel)) return channel;
            channel = new TimedEventChannel(channelType, 100f);
            _timedEventChannels.Add(channelType, channel);
            return channel;
        }

        #endregion

        IMapSubscription IMap.Subscribe(IMapSubscriber subscriber)
        {
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));

            return new Subscription(this, subscriber);
        }

        private sealed class Subscription : IMapSubscription
        {
            private SingleSectorMap Map { get; }
            private IMapSubscriber Subscriber { get; }

            internal Subscription(SingleSectorMap map, IMapSubscriber subscriber)
            {
                Map = map ?? throw new ArgumentNullException(nameof(map));
                Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            }

            void IMapSubscription.UpdatePosition(Vector3 position)
            {
                //TODO: Update character position and inform the client
            }

            #region IDisposable Support

            private bool _disposed = false;

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                    }

                    _disposed = true;
                }
            }

            ~Subscription()
            {
                Dispose(false);
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }
    }
}