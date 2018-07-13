using System;
using System.Numerics;
using Illarion.Server.Events;

namespace Illarion.Server.Events
{
    internal class ChatMessageEventUpdate : IChatMessageEventUpdate
    {
        public Guid CharacterId { get; internal set; }
        public MapChatChannelType ChatType { get; internal set; }
        public string Message { get; internal set; }
        public Vector3 Origin { get; internal set; }
    }
}
