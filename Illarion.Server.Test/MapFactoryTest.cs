using Xunit;

namespace Illarion.Server
{
    public class MapFactoryTest
    {
        [Fact]
        public void CreateSingleSectorMapTest()
        {
            var mapFactory = new MapFactory();
            IMap map = ((IMapFactory)mapFactory).CreateMap();

            Assert.NotNull(map);
            Assert.IsType<SingleSectorMap>(map);
        }
    }
}