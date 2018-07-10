using System;
using Illarion.Server;
using Illarion.Server.Navigation;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class IllarionNavigationServiceCollectionExtensions
  {
    public static IServiceCollection AddIllarionLocationService(this IServiceCollection serviceCollection)
    {
      if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

      serviceCollection.AddSingleton<INavigator>(p => new LocationInvestigator());

      return serviceCollection;
    }
  }
}