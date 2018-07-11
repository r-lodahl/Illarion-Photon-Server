namespace Illarion.Server
{
    public interface IChatMessageEventUpdate : IEventUpdate
    {
        MapChatChannelType ChatType { get; }
        string Message { get; }
    }
}
