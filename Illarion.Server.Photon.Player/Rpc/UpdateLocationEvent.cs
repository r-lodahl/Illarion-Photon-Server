using System;
using System.Numerics;
using Illarion.Net.Common.Events.Player;
using Photon.SocketServer.Rpc;

namespace Illarion.Server.Photon.Rpc
{
    internal sealed class UpdateLocationEvent : DataContract
    {
        [DataMember(Name = nameof(CharacterId), Code = (byte)UpdateLocationEventParameterCode.CharacterId)]
        internal Guid CharacterId { get; set; }

        [DataMember(Name = nameof(Location), Code = (byte)UpdateLocationEventParameterCode.Location)]
        internal Vector3 Location { get; set; }

        [DataMember(Name = nameof(Facing), Code = (byte)UpdateLocationEventParameterCode.LookAtDirection)]
        internal Vector3 Facing { get; set; }

        [DataMember(Name = nameof(Velocity), Code = (byte)UpdateLocationEventParameterCode.Velocity)]
        internal Vector3 Velocity { get; set; }
    }
}
