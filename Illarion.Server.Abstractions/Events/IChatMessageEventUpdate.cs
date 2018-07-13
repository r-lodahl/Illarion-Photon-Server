using System.Numerics;

namespace Illarion.Server.Events
{
    public interface IChatMessageEventUpdate : IEventUpdate
    {
        MapChatChannelType ChatType { get; }
        string Message { get; }
        Vector3 Origin { get; }
    }
}
