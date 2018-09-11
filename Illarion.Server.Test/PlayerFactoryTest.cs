using AutoFixture;
using AutoFixture.AutoMoq;
using Xunit;

namespace Illarion.Server
{
    public class PlayerFactoryTest
    {
        [Fact]
        public void DefaultCharacterCreationTest()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var playerFactory = new PlayerFactory();

            ICharacterCallback characterCallback = playerFactory.DefaultCharacterCallback(fixture.Create<ICharacter>());

            Assert.NotNull(characterCallback);
            Assert.IsType<DefaultCharacterCallback>(characterCallback);
        }
    }
}