using System.Collections.Generic;
using Illarion.Net.Common.Events.Player;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Illarion.Server.Photon.Rpc
{
    internal sealed class UpdateAllLocationEvent : DataContract
    {
        [DataMember(Name = nameof(LocationList), Code = (byte) UpdateAllLocationEventParameterCode.Locations)]
        internal IList<EventData> LocationList { get; set; }
    }
}
