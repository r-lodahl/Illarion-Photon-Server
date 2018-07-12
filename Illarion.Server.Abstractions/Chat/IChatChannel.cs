namespace Illarion.Server.Chat
{
  public interface IChatChannel
  {
      void Chat(IChatSubscriber sender, IChatMessageEventUpdate message);
      void Subscribe(IChatSubscriber subscriber);
  }
}
