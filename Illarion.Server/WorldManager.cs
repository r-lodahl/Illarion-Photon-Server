using System;
using System.Collections.Immutable;

namespace Illarion.Server
{
  internal class WorldManager : IWorldManager
  {
    private readonly ImmutableDictionary<int, World> _worlds;
        
    public int WorldCount => _worlds.Count;

    internal WorldManager(IMapFactory mapFactory, INavigationManager navigationManager)
    {
        if (mapFactory == null) throw new ArgumentNullException(nameof(mapFactory));
        if (navigationManager == null) throw new ArgumentNullException(nameof(navigationManager));
        _worlds = ImmutableDictionary.Create<int, World>();

        // TODO: For the moment we only got a single "world" with the index 0. That needs to change in future versions.
        _worlds = _worlds.Add(0, new World(mapFactory.CreateMap(), navigationManager.GetNavigator(0)));
    }

    IWorld IWorldManager.GetWorld(int index)
    {
      if (_worlds.TryGetValue(index, out World world))
      {
        return world;
      }
      throw new ArgumentOutOfRangeException(nameof(index), index, "There is no world available for the selected index.");
    }
  }
}
