using System.Numerics;

namespace Illarion.Server.Chat
{
    public interface IChatSubscriber
    {
        Vector3 Location { get; }
        void ReceiveMessage(IChatMessageEventUpdate chatUpdate);
    }
}