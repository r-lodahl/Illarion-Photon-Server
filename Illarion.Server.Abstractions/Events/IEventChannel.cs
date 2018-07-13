using System;

namespace Illarion.Server.Events
{
    public interface IEventChannel
    {
        event EventHandler<IEventUpdate> EventReceived;
        void PostEvent(IEventUpdate update);
    }
}
