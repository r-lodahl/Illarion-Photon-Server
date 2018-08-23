using System.Numerics;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Xunit;

namespace Illarion.Server
{
    public sealed class CharacterTest
    {
        [Theory, AutoData]
        public void TestLocationWriteable(Vector3 location)
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
            Character character = fixture.Create<Character>();

            character.Location = location;
            Assert.Equal(location, character.Location);
        }
            
    }
}
