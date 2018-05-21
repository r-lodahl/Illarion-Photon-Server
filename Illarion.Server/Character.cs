using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Illarion.Server
{
  internal sealed class Character : ICharacter, IMapSubscriber
  {
    internal World World { get; }

    public Vector3 Location { get; set; }

    public Vector3 Velocity { get; set; }

    public Vector3 FacingDirection { get; set; }

    public ICharacterCallback Callback { get; internal set; }
    IMapSubscription IMapSubscriber.Subscription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    internal Character(World world)
    {
      World = world ?? throw new ArgumentNullException(nameof(world));
    }

    bool ICharacterController.UpdateMovement(Vector3 location, Vector3 velocity, Vector3 facing)
    {
      Location = location;
      Velocity = velocity;
      FacingDirection = facing;

      return true;
    }

    void ICharacterController.Chat(IChatChannel channel, string text)
    {
      throw new NotImplementedException();
    }
  }
}
