using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Illarion.Server.Navigation
{
    internal class NavigationManager : INavigationManager
    {
        private readonly ImmutableDictionary<int, INavigator> _loadedNavigators =
            ImmutableDictionary<int, INavigator>.Empty;

        public NavigationManager(IServiceProvider provider)
        {
            IWorldManager worldManager = provider.GetRequiredService<IWorldManager>();

            for (var i = 0; i < worldManager.WorldCount; i++)
            {
                _loadedNavigators.Add(i, new Navigator(worldManager.GetWorld(i), $"Map\\{i}_navmesh.obj"));
            }
        }

        public INavigator GetNavigator(int worldIndex)
        {
            if (_loadedNavigators.TryGetValue(worldIndex, out INavigator navigator)) return navigator;
            throw new ArgumentOutOfRangeException(nameof(worldIndex), worldIndex, "There is no navigator available for the selected world.");
        }
    }
}