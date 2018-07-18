namespace Illarion.Net.Common
{
  /// <summary>
  /// These are the operation codes that can be used to control the game and actually play the character.
  /// </summary>
  public enum PlayerOperationCode : byte
  {
    /// <summary>
    /// This should be used by the client to report that it is done loading the current map. Upon sending it the server
    /// will spawn the character on the map and report back once it is done.
    /// </summary>
    LoadingReady = 0,

    /// <summary>
    /// Exit the game and return to the logged in account operation code. Removes the player from the current game.
    /// Once this is successful the <see cref="AccountOperationCode"/> is active.
    /// </summary>
    LogoutPlayer,

    /// <summary>
    ///   Update the location, looking direction and velocity of the character. This should be send very often.
    ///   It may be send as message since the server is not reponding with the meaningful message anyway.
    /// </summary>
    /// <seealso cref="Operations.Player.UpdateLocationReturnCode"/>
    /// <seealso cref="Operations.Player.UpdateLocationOperationRequestParameterCode"/>
    /// <seealso cref="Operations.Player.UpdateLocationOperationReponseParameterCode"/>
    UpdateLocation,

    /// <summary>
    /// This operation is actively send by the server and it contains location updates for every character presently
    /// monitored by the client receiving.
    /// It is possible to for the client to send this command in order to issue the server to send a update right now.
    /// </summary>
    /// <seealso cref="Operations.Player.UpdateAllLocationReturnCode"/>
    /// <seealso cref="Operations.Player.UpdateAllLocationsOperationReponseParameterCode"/>
    UpdateAllLocation,

    /// <summary>
    ///   <para>
    ///     This command should be send by the player in order to send a message to the server and to other players.
    ///   </para>
    ///   <para>
    ///     The server is sending this command in order to inform the player about any messages spoken including it's
    ///     own.
    ///   </para>
    /// </summary>
    SendMessage,

      /// <summary>
      /// This command should be send by the player if the description for an item is requested.
      /// This is needed, as scripts may modify single lookat-texts during runtime.
      /// </summary>
    LookAt,

      /// <summary>
      /// This command should be send by the player if he enters into attack mode with a specific target.
      /// </summary>
      FocusCharacter,

      /// <summary>
      /// This command should be used whenever the player attacks his focused target.
      /// </summary>
      AttackCharacter,

      /// <summary>
      /// This command should be send by the player if the player uses an ability.
      /// </summary>
      UseAbility,

      /// <summary>
      /// This command should be send by the player if the player uses an object on the map.
      /// </summary>
      UseMapObject,

      /// <summary>
      /// This command should be send by the player if he uses an equipped item.
      /// </summary>
      UseEquipmentItem,

      /// <summary>
      /// This command should be send by the player if he uses and inventory item.
      /// </summary>
      UseInventoryItem,

      /// <summary>
      /// This command should be send by the player if he opens his inventory.
      /// </summary>
      OpenWindow,

      /// <summary>
      /// This command should be send by the player if he moves an item.
      /// </summary>
      MoveItem,

      /// <summary>
      /// This command should be send by the player if he closes inventory or crafting window.
      /// </summary>
      CloseWindow,

      /// <summary>
      /// This command should be send by the player if he begins to craft an item.
      /// </summary>
      CraftItem,

      /// <summary>
      /// This command should be send by an admin player to view all online players.
      /// </summary>
      AdminShowPlayers,

      /// <summary>
      /// This command should be send by an admin player to broadcast to the whole world.
      /// </summary>
      AdminBroadcast,

      /// <summary>
      /// This command should be send by an admin player to set the location of a character.
      /// </summary>
      AdminSetLocation,

      /// <summary>
      /// This command should be send by an admin player to chat as another character.
      /// </summary>
      AdminChatAs
    }
}
