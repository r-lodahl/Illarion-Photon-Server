using System;
using System.Collections.Generic;
using System.Numerics;
using Illarion.Net.Common;
using Illarion.Net.Common.Operations.Player;
using Illarion.Server.Photon.Rpc;
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

        #region Operations

        protected override void OnDisconnect(PlayerPeerBase peer)
        {
        }

        protected override OperationResponse OnOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            if (peer == null) throw new ArgumentNullException(nameof(peer));
            if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

            switch ((PlayerOperationCode) operationRequest.OperationCode)
            {
                case PlayerOperationCode.LoadingReady:
                    peer.CharacterController = _worldManager.GetWorld(0).CreateNewCharacter(peer.Character.CharacterId,
                        x => _services.GetRequiredService<IPlayerFactory>().DefaultCharacterCallback(x));

                    if (peer.UpdateCallback == null) peer.UpdateCallback = new UpdateCallback();
                    peer.UpdateCallback.RegisterUpdater<ILocationEventUpdate>(_worldManager.GetWorld(0).Map
                        .GetEventChannel(MapEventChannelType.Location), peer, AtUpdateAllLocations);
                    break;
                case PlayerOperationCode.LogoutPlayer:
                    OnPlayerLeaveMap(peer);
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

        private void OnPlayerUpdateLocation(PlayerPeerBase peer, OperationRequest operationRequest) =>
            peer.CharacterController.UpdateMovement(
                (Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.Location],
                (Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.Velocity],
                (Vector3) operationRequest.Parameters[
                    (byte) UpdateLocationOperationRequestParameterCode.LookAtDirection]
            );

        #endregion

        #region Events

        private bool IsPeerUsable(PlayerPeerBase peer) =>
            peer != null && peer.Connected && peer.CurrentOperationHandler == this;

        private void AtUpdateAllLocations(PlayerPeerBase peer, List<ILocationEventUpdate> updates)
        {
            if (!IsPeerUsable(peer))
                throw new InvalidOperationException("Peer used for event messaging is out of service");

            var responseData = new UpdateAllLocationEvent {LocationList = new List<EventData>()};

            foreach (var update in updates)
            {
                responseData.LocationList.Add(GetLocationFromUpdate((byte) PlayerEventCode.UpdateLocation, update));
            }

            peer.SendEvent(new EventData((byte) PlayerEventCode.UpdateAllLocations)
            {
                Parameters = responseData.ToDictionary()
            }, new SendParameters());
        }

        private static EventData GetLocationFromUpdate(byte eventCode, ILocationEventUpdate locUpdate)
        {
            var responseData = new UpdateLocationEvent()
            {
                CharacterId = locUpdate.CharacterId,
                Location = locUpdate.Location,
                Velocity = locUpdate.Velocity,
                Facing = locUpdate.Facing
            };

            return new EventData(eventCode)
            {
                Parameters = responseData.ToDictionary()
            };
        }

        #endregion
    }
}