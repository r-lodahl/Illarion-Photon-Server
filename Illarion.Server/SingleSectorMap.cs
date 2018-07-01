using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Illarion.Server
{
  internal class SingleSectorMap : IMap
  {
    private ImmutableList<IMapSubscriber> _subscriberList;

    IChatChannel IMap.GetChatChannel(MapChatChannelType channelType) => null;

    IMapSubscription IMap.Subscribe(IMapSubscriber subscriber)
    {
      if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));

      _subscriberList.Add(subscriber);

      return new Subscription(this, subscriber);
    }

    public SingleSectorMap() => _subscriberList = ImmutableList<IMapSubscriber>.Empty;

    void UpdateSubscribers()
    {

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
