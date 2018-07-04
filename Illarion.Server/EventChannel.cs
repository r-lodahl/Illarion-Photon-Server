using System;

namespace Illarion.Server
{
    internal class EventChannel : IEventChannel
    {
      public MapEventChannelType ChannelType { get; }
      public float UpdateFrequency { get; }
      public event EventHandler<IEventUpdate> EventReceived;

      protected virtual void OnEventReceived(IEventUpdate update) => EventReceived?.Invoke(this, update);

      public void PostEvent(IEventUpdate eventUpdate) => EventReceived?.Invoke(this, eventUpdate);

      public EventChannel(MapEventChannelType channelType, float initialUpdateFrequency)
      {
        ChannelType = channelType;
        UpdateFrequency = initialUpdateFrequency;
      }
    }
}
