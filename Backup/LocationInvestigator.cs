using System;

namespace Illarion.Server.Navigation
{
    internal class LocationInvestigator : ILocationInvestigator
    {
        private IServiceProvider ServiceProvider { get; }

        internal LocationInvestigator(IServiceProvider provider) => ServiceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
    }
}
