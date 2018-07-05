using System;
using System.Numerics;

namespace Illarion.Server
{
  internal sealed class Character : ICharacter, IMapSubscriber
  {
    internal IWorld World { get; }

    public Guid CharacterId { get; }

    public Vector3 Location { get; set; }

    public Vector3 Velocity { get; set; }

    public Vector3 FacingDirection { get; set; }

    public ICharacterCallback Callback { get; internal set; }
    IMapSubscription IMapSubscriber.Subscription { get; set; }

    internal Character(Guid characterId, IWorld world)
    {
      CharacterId = characterId;
      World = world ?? throw new ArgumentNullException(nameof(world));
      ((IMapSubscriber)this).Subscription = world.Map.Subscribe(this);
    } 

    bool ICharacterController.UpdateMovement(Vector3 location, Vector3 velocity, Vector3 facing)
    {
      Location = location;
      Velocity = velocity;
      FacingDirection = facing;

      World.Map.GetEventChannel(MapEventChannelType.Location).PostEvent(new LocationEventUpdate {
        CharacterId = CharacterId;
        Location = location,
        Velocity = velocity,
        FacingDirection = facing
      });

      return true;
    }

    void ICharacterController.Chat(IChatChannel channel, string text)
    {
      throw new NotImplementedException();
    }
  }
}
