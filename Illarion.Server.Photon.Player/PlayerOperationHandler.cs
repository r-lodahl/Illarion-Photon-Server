using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Illarion.Net.Common;
using Illarion.Net.Common.Operations.Player;
using Illarion.Server.Persistence.Server;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;

namespace Illarion.Server.Photon
{
  public sealed class PlayerOperationHandler : PlayerPeerOperationHandler, IPlayerOperationHandler
  {
    private readonly IServiceProvider _services;

    private readonly IWorldManager _worldManager;

    public PlayerOperationHandler(IServiceProvider services) : base(services)
    {
      _services = services ?? throw new ArgumentNullException(nameof(services));
      _worldManager = _services.GetRequiredService<IWorldManager>();
    }
    

    protected override void OnDisconnect(PlayerPeerBase peer)
    {
    }

    protected override OperationResponse OnOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
    {
      if (peer == null) throw new ArgumentNullException(nameof(peer));
      if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

      switch ((PlayerOperationCode)operationRequest.OperationCode)
      {
        case PlayerOperationCode.LoadingReady:
          peer.CharacterController = _worldManager.GetWorld(0).CreateNewCharacter(x => _services.GetRequiredService<IPlayerFactory>().DefaultCharacterCallback(x));

          if (peer.UpdateCallback == null) peer.UpdateCallback = new UpdateCallback();
          peer.UpdateCallback.RegisterUpdater(_worldManager.GetWorld(0).Map
            .GetEventChannel(MapEventChannelType.Movement), peer);
          break;
        case PlayerOperationCode.LogoutPlayer:
          break;
        case PlayerOperationCode.SendMessage:
          break;
        case PlayerOperationCode.UpdateAllLocations:
          break;
        case PlayerOperationCode.UpdateAppearance:
          break;
        case PlayerOperationCode.UpdateLocation:
          OnPlayerUpdateLocation(peer, operationRequest);
          break;
      }

      return InvalidOperation(operationRequest);
    }

    private void OnPlayerLeaveMap(PlayerPeerBase peer) => peer.UpdateCallback.UnregisterAll();

    private void OnPlayerUpdateLocation(PlayerPeerBase peer, OperationRequest operationRequest) => peer.CharacterController.UpdateMovement(
        (Vector3)operationRequest.Parameters[(byte)UpdateLocationOperationRequestParameterCode.Location],
        (Vector3)operationRequest.Parameters[(byte)UpdateLocationOperationRequestParameterCode.Velocity],
        (Vector3)operationRequest.Parameters[(byte)UpdateLocationOperationRequestParameterCode.LookAtDirection]
    );
  }
}
