using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Illarion.Server.Chat;

namespace Illarion.Server.Event
{
    internal class ChatMessageEventUpdate : IChatMessageEventUpdate
    {
        public Guid CharacterId { get; internal set; }
        public MapChatChannelType ChatType { get; internal set; }
        public string Message { get; internal set; }
        public Vector3 Origin { get; internal set; }
    }
}
