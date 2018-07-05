using System.Numerics;

namespace Illarion.Server
{
    public interface ILocationEventUpdate : IEventUpdate
    {
        Vector3 Location { get; }
        Vector3 Facing { get; }
        Vector3 Velocity { get; }
    }
}
