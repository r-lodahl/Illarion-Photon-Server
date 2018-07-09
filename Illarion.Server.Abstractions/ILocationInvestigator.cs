using System.Numerics;

namespace Illarion.Server
{
    public interface ILocationInvestigator
    {
        Vector3 InvestigateUpdatedLocation(Vector3 location, Vector3 updatedLocation, float deltaTime);
    }
}
