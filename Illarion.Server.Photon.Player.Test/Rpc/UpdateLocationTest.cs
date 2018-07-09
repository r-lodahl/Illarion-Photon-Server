using System;
using System.Collections.Generic;
using Illarion.Net.Common;
//using Photon.SocketServer;
using Xunit;
using System.Numerics;
using Illarion.Net.Common.Events.Player;

namespace Illarion.Server.Photon.Rpc
{
    public sealed class UpdateLocationTest
    {
        [Trait("Category", "Networking")]
        [Theory]
        [MemberData(nameof(TestData))]
        public static void EncodeOperation(Guid characterId, Vector3 location, Vector3 facing, Vector3 velocity)
        {
            var data = new Dictionary<byte, object>() {
                //{ (byte)UpdateLocationOperationRequestParameterCode.CharacterId, characterId },
                { (byte)UpdateLocationEventParameterCode.Location, location },
                { (byte)UpdateLocationEventParameterCode.LookAtDirection, facing },
                { (byte)UpdateLocationEventParameterCode.Velocity, velocity }
            };

          //  var operationRequest = new OperationRequest((byte)PlayerOperationCode.UpdateLocation, data);
           // var operation = new UpdateLocationOperation(Protocol.GpBinaryV17, operationRequest);

          //  Assert.NotNull(operation.OperationRequest);
          //  Assert.Equal((byte)PlayerOperationCode.UpdateLocation, operation.OperationRequest.OperationCode);
          //  Assert.Equal(location, operation.Location);
          //  Assert.Equal(velocity, operation.Velocity);
          //  Assert.Equal(facing, operation.LookAtDirection);
        }

        public static IEnumerable<object[]> TestData()
        {
            unchecked
            {
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), Vector3.One, Vector3.One, Vector3.One };
                yield return new object[] { Guid.Empty, Vector3.One, Vector3.One, Vector3.One };
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), Vector3.One, Vector3.One };
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), Vector3.One, new Vector3(float.MinValue, float.MaxValue, float.MinValue), Vector3.One };
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), Vector3.One, Vector3.One, new Vector3(float.NaN, float.NaN, float.NaN) };
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), Vector3.One, Vector3.One, new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity) };
                yield return new object[] { new Guid(0xddd8f46, 0xa72, 0x4b4a, new byte[] { 0x9d, 0x4a, 0x2d, 0xeb, 0xb7, 0x17, 0x16, 0x1 }), Vector3.One, new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity), Vector3.One };
            }
        }
    }
}
