using System;
using System.Collections.Generic;
using System.Text;

namespace Illarion.Server
{
  public interface IMap
  {
    IChatChannel GetChatChannel(MapChatChannelType channelType);

    IEventChannel GetEventChannel(MapEventChannelType channelType);

    IMapSubscription Subscribe(IMapSubscriber subscriber);
  }
}
