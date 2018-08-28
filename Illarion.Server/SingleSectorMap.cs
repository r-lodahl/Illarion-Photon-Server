using System;
using System.Collections.Generic;
using System.Numerics;
using Illarion.Server.Events;

namespace Illarion.Server
{
    internal class SingleSectorMap : IMap
    {
        #region Channels

        private readonly Dictionary<MapEventChannelType, IEventChannel> _eventChannels =
            new Dictionary<MapEventChannelType, IEventChannel>();

        IEventChannel IMap.GetEventChannel(MapEventChannelType channelType)
        {
            if(!Enum.IsDefined(typeof(MapEventChannelType), channelType)) throw new ArgumentOutOfRangeException(nameof(channelType), channelType, "Illegal event channel requested!");
            if (_eventChannels.TryGetValue(channelType, out IEventChannel channel)) return channel;
            channel = new EventChannel(channelType);
            _eventChannels.Add(channelType, channel);
            return channel;
        }

        IEventChannel IMap.GetChatChannel(MapChatChannelType channelType)
        {
            switch (channelType)
            {
                case MapChatChannelType.Global:
                    return ( (IMap) this ).GetEventChannel(MapEventChannelType.GlobalChat);
                case MapChatChannelType.Speaking:
                    return ( (IMap) this ).GetEventChannel(MapEventChannelType.TalkingChat);
                case MapChatChannelType.Yelling:
                    return ( (IMap) this ).GetEventChannel(MapEventChannelType.YellingChat);
                case MapChatChannelType.Whispering:
                    return ( (IMap) this ).GetEventChannel(MapEventChannelType.WhisperingChat);
                default:
                    throw new ArgumentOutOfRangeException(nameof(channelType), channelType, "Illegal chat channel requested!");
            }
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