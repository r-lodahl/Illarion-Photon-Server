using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Illarion.Server
{
  internal class SingleSectorMap : IMap
  {
    private readonly ITimedEventChannel _locationEventChannel = new TimedEventChannel(MapEventChannelType.Location, 100);

    IChatChannel IMap.GetChatChannel(MapChatChannelType channelType) => null;
    IEventChannel IMap.GetEventChannel(MapEventChannelType channelType) => null;
    ITimedEventChannel IMap.GetTimedEventChannel(MapEventChannelType channelType) => _locationEventChannel; //TODO: Support other channel types, too //TODO: Generate channels on first request


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

      void Dispose(bool disposing)
      {
        if (!_disposed)
        {
          if (disposing)
          {
          }

          _disposed = true;
        }
      }
      
      ~Subscription() {
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
