using System;
using System.Collections.Generic;
using System.Numerics;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Illarion.Server.Events;
using Xunit;

namespace Illarion.Server
{
    public class SingleSectorMapTest
    {
        [Fact]
        public void TestMapSubscription()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var map = (IMap) fixture.Create<SingleSectorMap>();

            IMapSubscriber subscriber = fixture.Create<IMapSubscriber>();
            IMapSubscription subscription = map.Subscribe(subscriber);
            Assert.IsAssignableFrom<IMapSubscription>(subscription);

            var updatedPosition = fixture.Create<Vector3>();
            subscription.UpdatePosition(updatedPosition);
            //Assert.Equal(updatedPosition, subscriber.Location); TODO!

            subscription.Dispose();

            // Test Invalid
            Assert.ThrowsAny<Exception>(() => map.Subscribe(null));
        }

        [Theory, MemberData(nameof(ValidEventChannelData))]
        public void TestValidEventChannel(MapEventChannelType eventChannelType)
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var map = (IMap)fixture.Create<SingleSectorMap>();

            // Valid Test
            IEventChannel channel = map.GetEventChannel(eventChannelType);
            Assert.NotNull(channel);

            IEventChannel channel2 = map.GetEventChannel(eventChannelType);
            Assert.Same(channel2, channel);
        }

        public static IEnumerable<object[]> ValidEventChannelData()
        {
            Array values = Enum.GetValues(typeof(MapEventChannelType));
            foreach (var channelType in values)
            {
                yield return new[] { channelType };
            }
        }

        [Theory, MemberData(nameof(ValidChatChannelData))]
        public void TestValidChatChannel(MapChatChannelType eventChannelType)
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var map = (IMap)fixture.Create<SingleSectorMap>();

            // Valid Test
            IEventChannel channel = map.GetChatChannel(eventChannelType);
            Assert.NotNull(channel);

            IEventChannel channel2 = map.GetChatChannel(eventChannelType);
            Assert.Same(channel2, channel);
        }

        public static IEnumerable<object[]> ValidChatChannelData()
        {
            Array values = Enum.GetValues(typeof(MapChatChannelType));
            foreach (var channelType in values)
            {
                yield return new[] { channelType };
            }
        }

        [Fact]
        public void TestInvalidEventChannel()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var map = (IMap)fixture.Create<SingleSectorMap>();

            Assert.ThrowsAny<Exception>(() => map.GetEventChannel((MapEventChannelType)(-1)));
        }

        [Fact]
        public void TestInvalidChatChannel()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var map = (IMap)fixture.Create<SingleSectorMap>();

            Assert.ThrowsAny<Exception>(() => map.GetChatChannel((MapChatChannelType)(-1)));
        }
    }
}
