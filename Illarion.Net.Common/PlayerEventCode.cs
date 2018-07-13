namespace Illarion.Net.Common
{
    public enum PlayerEventCode : byte
  {
      /// <summary>
      ///   Update the location, looking direction and velocity of the character. This should be send very often.
      ///   It may be send as message since the server is not reponding with the meaningful message anyway.
      /// </summary>
      /// <seealso cref="Operations.Player.UpdateLocationReturnCode"/>
      /// <seealso cref="Operations.Player.UpdateLocationOperationRequestParameterCode"/>
      /// <seealso cref="Operations.Player.UpdateLocationOperationReponseParameterCode"/>
      UpdateLocation = 0,

      //TODO: If not issued by the client, this command should only send data for those character whose values have changed since last time the message was sent
      /// <summary>
      /// This operation is actively send by the server and it contains location updates for every character presently
      /// monitored by the client receiving.
      /// It is possible to for the client to send this command in order to issue the server to send a update right now.
      /// </summary>
      /// <seealso cref="Operations.Player.UpdateAllLocationReturnCode"/>
      /// <seealso cref="Operations.Player.UpdateAllLocationsOperationReponseParameterCode"/>
      UpdateAllLocations,

      Chat
    }
}
