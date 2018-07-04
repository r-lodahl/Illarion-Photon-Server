using System;

namespace Illarion.Server
{
    public interface IEventChannel
    {
      event EventHandler<IEventUpdate> EventReceived;
      MapEventChannelType ChannelType { get; }

      /// <summary> Recommended update frequency of this channel in ms </summary>
      float UpdateFrequency { get; }

      void PostEvent(IEventUpdate update);
    }
}
