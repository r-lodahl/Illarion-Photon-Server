using System;
using System.Numerics;
using Illarion.Server.Events;

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

    bool ICharacterController.UpdateMovement(Vector3 location, Vector3 velocity, Vector3 facing, float deltaTime)
    {
      Vector3 updatedLocation = World.Navigator.InvestigateUpdatedLocation(Location, location, deltaTime);
      if (!updatedLocation.Equals(location))
      {
        //TODO: decide: (a) if position changed send instant correction to client (b) changed position sent per normal event channel, put investigation in network
      }

      Location = updatedLocation;
      Velocity = velocity;
      FacingDirection = facing;

      World.Map.GetEventChannel(MapEventChannelType.Location).PostEvent(new LocationEventUpdate {
        CharacterId = CharacterId,
        Location = updatedLocation,
        Velocity = velocity,
        Facing = facing
      });

      return true;
    }

      void ICharacterController.Chat(MapChatChannelType channelType, string text)
      {
          var update = new ChatMessageEventUpdate
          {
              CharacterId = CharacterId,
              ChatType = channelType,
              Message = text,
              Origin = Location
          };
          World.Map.GetChatChannel(channelType).PostEvent(update);
        }
    }
}
