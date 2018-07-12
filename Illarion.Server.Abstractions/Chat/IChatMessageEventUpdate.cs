namespace Illarion.Server.Chat
{
    public interface IChatMessageEventUpdate : IEventUpdate
    {
        MapChatChannelType ChatType { get; }
        string Message { get; }
    }
}
