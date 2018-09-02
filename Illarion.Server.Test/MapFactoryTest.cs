using xunit;

namespace Illarion.Server
{
    public class MapFactoryTest
    {
        [Fact]
        public void CreateSingleSectorMapTest()
        {
            var mapFactory = new MapFactory();
            var map = mapFactory.CreateMap();

            Assert.NotNull(map);
            Assert.IsType(typeof(SingleSectorMap), map);
        }
    }
}