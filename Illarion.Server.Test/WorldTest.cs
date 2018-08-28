using System;
using System.Numerics;
using AutoFixture;
using AutoFixture.AutoMoq;
using Illarion.Server.Events;
using Xunit;

namespace Illarion.Server
{
    public class WorldTest
    {
        [Fact]
        public void TestBasicProperties()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
          
            IMap map = fixture.Create<IMap>();
            INavigator nav = fixture.Create<INavigator>();
            var world = new World(map, nav);

            Assert.Equal(map, world.Map);
            Assert.Equal(nav, world.Navigator);
        }

        [Fact]
        public void TestCharacter()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var world = (IWorld) new World(fixture.Create<IMap>(), fixture.Create<INavigator>());

            // Test valid character
            ICharacter character = world.CreateNewCharacter(Guid.Empty, x => x.Callback);
            Assert.Equal(Guid.Empty, character.CharacterId);

            world.AddCharacter(character); // Assert no exception
            Assert.ThrowsAny<Exception>(() => world.AddCharacter(character));

            Assert.True(world.RemoveCharacter(character));
            Assert.False(world.RemoveCharacter(character));

            // Test invalid character
            ICharacter faultyCharacter = new FaultyCharacter();
            Assert.ThrowsAny<Exception>(() => world.AddCharacter(faultyCharacter));
            Assert.ThrowsAny<Exception>(() => world.RemoveCharacter(faultyCharacter));

            // Test null
            Assert.ThrowsAny<Exception>(() => world.AddCharacter(null));
            Assert.ThrowsAny<Exception>(() => world.RemoveCharacter(null));
        }

        /// <summary>
        /// This character is not part of any world/character implementation and should not be accepted by any world implementation
        /// </summary>
        private class FaultyCharacter : ICharacter
        {
            public bool UpdateMovement(Vector3 location, Vector3 velocity, Vector3 facing, float deltaTime) => throw new NotImplementedException();
            public void Chat(MapChatChannelType channelType, string text) => throw new NotImplementedException();

            public ICharacterCallback Callback { get; }
            public Guid CharacterId { get; }
            public Vector3 Location { get; }
            public Vector3 Velocity { get; }
            public Vector3 FacingDirection { get; }
        }
    }
}
