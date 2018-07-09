namespace Illarion.Server
{
  public interface IMap
  {
    IChatChannel GetChatChannel(MapChatChannelType channelType);

    ITimedEventChannel GetTimedEventChannel(MapEventChannelType channelType);

    IEventChannel GetEventChannel(MapEventChannelType channelType);

    IMapSubscription Subscribe(IMapSubscriber subscriber);
  }
}
