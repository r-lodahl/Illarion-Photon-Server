using System;
using System.Numerics;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Illarion.Server.Events;
using Moq;
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

        [Fact]
        public void TestIMapSubscription()
        {
            var world = new Mock<IWorld>();
            var map = new Mock<IMap>();
            var subscription = new Mock<IMapSubscription>();

            world.SetupGet(x => x.Map).Returns(map.Object);
            map.Setup(x => x.Subscribe(It.IsAny<IMapSubscriber>())).Returns(subscription.Object);

            Assert.Equal(subscription.Object, ( (IMapSubscriber) new Character(Guid.Empty, world.Object) ).Subscription);
        }

        [Theory, AutoData]
        public void TestChat(MapChatChannelType chatType, string text)
        {
            var raised = false;

            var channel = new Mock<IEventChannel>(MockBehavior.Loose);
            var map = new Mock<IMap>(MockBehavior.Loose);
            var world = new Mock<IWorld>(MockBehavior.Loose);
            world.SetupGet(x => x.Map).Returns(map.Object);
            map.Setup(x => x.GetChatChannel(chatType)).Returns(channel.Object);
            channel.Setup(x => x.PostEvent(It.IsAny<ChatMessageEventUpdate>())).Callback(() => { raised = true; });
            
            var character = new Character(Guid.Empty, world.Object);
            ((ICharacterController)character).Chat(chatType, text);

            Assert.True(raised);
        }
    }
}
