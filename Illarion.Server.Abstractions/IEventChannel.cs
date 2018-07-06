using System;

namespace Illarion.Server
{
    public interface IEventChannel
    {
        event EventHandler<IEventUpdate> EventReceived;
        void PostEvent(IEventUpdate update);
    }
}
