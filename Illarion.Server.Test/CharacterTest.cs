using System;
using System.Numerics;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Illarion.Server.Events;
using Xunit;

namespace Illarion.Server
{
    public sealed class CharacterTest
    {
        [Theory, AutoMoqData]
        public void TestBasicProperties(Vector3 location, Vector3 facing, Vector3 velocity, Guid characterId,
            IWorld world, ICharacterCallback callback)
        {
            var character = new Character(characterId, world)
            {
                FacingDirection = facing,
                Location = location,
                Velocity = velocity,
                Callback = callback
            };

            Assert.Equal(characterId, character.CharacterId);
            Assert.Equal(facing, character.FacingDirection);
            Assert.Equal(velocity, character.Velocity);
            Assert.Equal(location, character.Location);
            Assert.Equal(world, character.World);
            Assert.Equal(callback, character.Callback);
        }

        [Theory, AutoMoqData]
        public void TestUpdateMovement(Vector3 location, Vector3 velocity, Vector3 facing, float deltaTime)
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            IWorld world = fixture.Create<IWorld>();

            var character = new Character(Guid.Empty, world);

            var controller = (ICharacterController) character;
            var acceptMovement = location.Equals(world.Navigator.InvestigateUpdatedLocation(character.Location, location, deltaTime));

            Assert.Equal(acceptMovement, controller.UpdateMovement(location, velocity, facing, deltaTime));
        }

        [Theory, AutoMoqData]
        public void TestChat(MapChatChannelType chatType, string text)
        {
        }
    }
}
