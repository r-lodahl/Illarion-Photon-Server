using System.Numerics;
using Illarion.Server.Event;

namespace Illarion.Server.Chat
{
    public interface IChatMessageEventUpdate : IEventUpdate
    {
        MapChatChannelType ChatType { get; }
        string Message { get; }
        Vector3 Origin { get; }
    }
}
