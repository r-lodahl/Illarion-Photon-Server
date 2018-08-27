using System;
using System.Collections.Generic;
using System.Numerics;
using AutoFixture.Xunit2;
using Xunit;

namespace Illarion.Server
{
    public sealed class CharacterTest
    {
        [Theory]
        [MemberData(nameof(BasicPropertyTestData))]
        [AutoMoqData]
        public void TestBasicProperties(Vector3 location, Vector3 facing, Vector3 velocity, Guid characterId,
            IWorld world)
        {
            var character = new Character(characterId, world)
            {
                FacingDirection = facing,
                Location = location,
                Velocity = velocity
            };

            Assert.Equal(characterId, character.CharacterId);
            Assert.Equal(facing, character.FacingDirection);
            Assert.Equal(velocity, character.Velocity);
            Assert.Equal(location, character.Location);
            Assert.Equal(world, character.World);
        }

        public static IEnumerable<object[]> BasicPropertyTestData()
        {
            yield return new object[]
            {
                new Vector3(float.NaN, float.NaN, float.NaN), new Vector3(float.NaN, float.NaN, float.NaN),
                new Vector3(float.NaN, float.NaN, float.NaN), Guid.Empty, null
            };
        }
    }
}
