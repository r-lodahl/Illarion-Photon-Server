using System;
using System.Numerics;

namespace Illarion.Server
{
    internal class LocationEventUpdate : ILocationEventUpdate
    {
      public Guid CharacterId { get; set; }

      public Vector3 Location { get; set; }

      public Vector3 Velocity { get; set; }

      public Vector3 Facing { get; set; }
    }
}
