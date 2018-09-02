using xunit;

namespace Illarion.Server
{
    public class PlayerFactoryTest
    {
        [Fact]
        public void DefaultCharacterCreationTest()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            var playerFactory = new playerFactory();

            var characterCallback = playerFactory.DefaultCharacterCallback(fixture.Create<ICharacter>());

            Assert.NotNull(characterCallback);
            Assert.IsType(typeof(DefaultCharacterCallback), characterCallback);
        }
    }
}