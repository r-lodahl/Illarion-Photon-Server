using System;
using Illarion.Net.Common.Events.Player;
using Photon.SocketServer.Rpc;

namespace Illarion.Server.Photon.Rpc
{
    internal sealed class ChatMessageEvent : DataContract
    {
        [DataMember(Name = nameof(CharacterId), Code = (byte)ChatMessageEventParameterCode.CharacterId)]
        internal Guid CharacterId { get; set; }

        [DataMember(Name = nameof(Message), Code = (byte)ChatMessageEventParameterCode.Message)]
        internal string Message { get; set; }

        [DataMember(Name = nameof(ChatType), Code = (byte)ChatMessageEventParameterCode.ChatType)]
        internal byte ChatType { get; set; }
    }
}