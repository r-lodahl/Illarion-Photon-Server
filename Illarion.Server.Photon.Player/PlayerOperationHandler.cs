using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Illarion.Net.Common;
using Illarion.Net.Common.Operations.Player;
using Illarion.Server.Events;
using Illarion.Server.Photon.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;

namespace Illarion.Server.Photon
{
    public sealed class PlayerOperationHandler : PlayerPeerOperationHandler, IPlayerOperationHandler
    {
        private readonly IServiceProvider _services;

        private readonly IWorldManager _worldManager;

        private long _locationUpdateTimestamp;

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
                    OnLoadingReady(peer, operationRequest);
                    break;
                case PlayerOperationCode.LogoutPlayer:
                    OnLeaveMap(peer);
                    break;
                case PlayerOperationCode.SendMessage:
                    OnSendMessage(peer, operationRequest);
                    break;
                case PlayerOperationCode.UpdateAllLocation:
                    break;
                case PlayerOperationCode.UpdateAppearance:
                    break;
                case PlayerOperationCode.UpdateLocation:
                    OnUpdateLocation(peer, operationRequest);
                    break;
            }

            return InvalidOperation(operationRequest);
        }

        private static void OnLeaveMap(PlayerPeerBase peer) => peer.UpdateCallback.UnregisterAll();

        private static void OnSendMessage(PlayerPeerBase peer, OperationRequest operationRequest)
        {
            var channelType = (MapChatChannelType)operationRequest.Parameters[(byte) SendMessageOperationRequestParameterCode.ChatType];

            peer.CharacterController.Chat(
                channelType,
                (string)operationRequest.Parameters[(byte)SendMessageOperationRequestParameterCode.Message]
            );
        }

        private void OnLoadingReady(PlayerPeerBase peer, OperationRequest operationRequest)
        {
            peer.CharacterController = _worldManager.GetWorld(0).CreateNewCharacter(
                peer.Character.CharacterId,
                x => _services.GetRequiredService<IPlayerFactory>().DefaultCharacterCallback(x)
            );

            if (peer.UpdateCallback == null)
            {
                peer.UpdateCallback = new UpdateCallback();
            }

            // Register to events on the current map chunk

            _worldManager.GetWorld(0).Map.GetEventChannel(MapEventChannelType.TalkingChat).EventReceived +=
                (sender, opRequest) => AtTalkingChat(sender, opRequest, peer);

            _worldManager.GetWorld(0).Map.GetEventChannel(MapEventChannelType.WhisperingChat).EventReceived +=
                (sender, opRequest) => AtWhisperingChat(sender, opRequest, peer);

            _worldManager.GetWorld(0).Map.GetEventChannel(MapEventChannelType.GlobalChat).EventReceived +=
                (sender, opRequest) => AtGlobalChat(sender, opRequest, peer);

            _worldManager.GetWorld(0).Map.GetEventChannel(MapEventChannelType.YellingChat).EventReceived +=
                (sender, opRequest) => AtYellingChat(sender, opRequest, peer);

            peer.UpdateCallback.RegisterUpdater<ILocationEventUpdate>(
                _worldManager.GetWorld(0).Map.GetEventChannel(MapEventChannelType.Location),
                peer,
                AtUpdateAllLocations
            );
        }

        private void OnUpdateLocation(PlayerPeerBase peer, OperationRequest operationRequest)
        {
            var currentTimestamp = Stopwatch.GetTimestamp();
            var deltaTime = (_locationUpdateTimestamp - currentTimestamp) / (double) Stopwatch.Frequency * 1000;
            _locationUpdateTimestamp = currentTimestamp;

            if (peer.CharacterController.UpdateMovement(
                (Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.Location],
                (Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.Velocity],
                (Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.LookAtDirection],
                (float) deltaTime
            ))
            {
                peer.Character.Location =(Vector3) operationRequest.Parameters[(byte) UpdateLocationOperationRequestParameterCode.Location];
            }
        }
        
        #endregion

        #region Events

        private bool IsPeerUsable(PlayerPeerBase peer) =>
            peer != null && peer.Connected && peer.CurrentOperationHandler == this;

        private void AtTalkingChat(object sender, IEventUpdate update, PlayerPeerBase peer) => AtChat(update, peer, 10f);
        private void AtWhisperingChat(object sender, IEventUpdate update, PlayerPeerBase peer) => AtChat(update, peer, 2f);
        private void AtYellingChat(object sender, IEventUpdate update, PlayerPeerBase peer) => AtChat(update, peer, 20f);
        private void AtGlobalChat(object sender, IEventUpdate update, PlayerPeerBase peer) => AtChat(update, peer, 1000f);

        private void AtChat(IEventUpdate update, PlayerPeerBase peer, float chatDistance)
        {
            if (!IsPeerUsable(peer)) throw new InvalidOperationException("Peer used for event messaging is out of service");

            if (update is IChatMessageEventUpdate chatUpdate && Vector3.DistanceSquared(peer.Character.Location, chatUpdate.Origin) <= chatDistance)
            {
                var responseData = new ChatMessageEvent
                {
                    CharacterId = chatUpdate.CharacterId,
                    ChatType = (byte) chatUpdate.ChatType,
                    Message = chatUpdate.Message
                };

                peer.SendEvent(new EventData((byte)PlayerEventCode.Chat)
                {
                    Parameters = responseData.ToDictionary()
                }, new SendParameters());
            }
            else
            {
                throw new ArgumentException($"Expected {typeof(IChatMessageEventUpdate)} but got {update.GetType()} as update.");
            }
        }

        private void AtUpdateAllLocations(PlayerPeerBase peer, List<ILocationEventUpdate> updates)
        {
            if (!IsPeerUsable(peer))
                throw new InvalidOperationException("Peer used for event messaging is out of service");

            var responseData = new UpdateAllLocationEvent {LocationList = new List<EventData>()};

            foreach (ILocationEventUpdate update in updates)
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