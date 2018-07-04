using System.Numerics;

namespace Illarion.Server
{
    internal class MovementEventUpdate : IEventUpdate
    {
      public Vector3 Location { get; set; }

      public Vector3 Velocity { get; set; }

      public Vector3 FacingDirection { get; set; }
    }
}
