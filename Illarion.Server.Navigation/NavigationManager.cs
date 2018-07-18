using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Illarion.Server.Navigation
{
    internal class NavigationManager : INavigationManager
    {
        private readonly ImmutableDictionary<IWorld, INavigator> _loadedNavigators =
            ImmutableDictionary<IWorld, INavigator>.Empty;

        public NavigationManager(IServiceProvider provider)
        {
            IWorldManager worldManager = provider.GetRequiredService<IWorldManager>();

            for (var i = 0; i < worldManager.WorldCount; i++)
            {
                IWorld world = worldManager.GetWorld(i);
                _loadedNavigators.Add(world, new Navigator(worldManager.GetWorld(i), $"Map\\{i}_navmesh.obj"));
            }
        }

        public INavigator GetNavigator(IWorld world)
        {
            if (_loadedNavigators.TryGetValue(world, out INavigator navigator)) return navigator;
            throw new ArgumentOutOfRangeException(nameof(world), world, "There is no navigator available for the selected world.");
        }
    }
}