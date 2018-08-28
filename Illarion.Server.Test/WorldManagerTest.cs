using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Xunit;

namespace Illarion.Server
{
    public class WorldManagerTest
    {
        [Fact]
        public void TestWorldManager()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var worldManager = (IWorldManager) new WorldManager(fixture.Create<IMapFactory>(), fixture.Create<INavigationManager>());

            // Test Valid
            Assert.Equal(1, worldManager.WorldCount);
            Assert.IsType<World>(worldManager.GetWorld(0));

            // Test Invalid: Faulty world index ]0[
            Assert.ThrowsAny<Exception>(() => worldManager.GetWorld(-1));

            // Test null
            Assert.ThrowsAny<Exception>(() => new WorldManager(fixture.Create<IMapFactory>(), null));
            Assert.ThrowsAny<Exception>(() => new WorldManager(null, fixture.Create<INavigationManager>()));
        }

    }
}
