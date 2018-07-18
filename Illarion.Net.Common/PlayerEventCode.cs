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

      /// <summary>
      /// This event should be send to the player if a player writes a message nearby.
      /// </summary>
      Chat,

      /// <summary>
      /// This event should be send to the player if another character changes some part of his equipment altering his appearance.
      /// </summary>
      UpdateAppearance,

      /// <summary>
      /// This event should be send to the player once after loading to inform him about his complete equipment.
      /// </summary>
      UpdateAllEquipmentItems,

      /// <summary>
      /// This event should be send to the player once after loading to inform him about his complete inventory.
      /// </summary>
      UpdateAllInventoryItems,

      /// <summary>
      /// This event should be send to the player to add, move or remove an item in the inventory.
      /// </summary>
      ChangeInventoryItem,

      /// <summary>
      /// This event should be send to the player to add, move or remove an item from the equipment.
      /// </summary>
      ChangeEquipmentItem,

      /// <summary>
      /// This event should be send to the player if the attributes of his character have changed.
      /// </summary>
      UpdateAttribute,

      /// <summary>
      /// This event should be send to the player if his stats have changed.
      /// </summary>
      UpdateStat,

      /// <summary>
      /// This event should be send to the player if he has to change the map (or requested to do via UseMapObject).
      /// </summary>
      ChangeMap,

      /// <summary>
      /// This event should be send to the player if a specific effect (sound, graphic, ...) should be shown.
      /// </summary>
      ShowEffect,

      /// <summary>
      /// This event should be send to the player if he ís prompted to give input.
      /// </summary>
      OpenInputWindow,

      /// <summary>
      /// This event should be send to the player if a specific message should be displayed.
      /// </summary>
      OpenMessageWindow,

      /// <summary>
      /// This event should be send to the player if he interacts with a merchant.
      /// </summary>
      OpenMerchantWindow,

      /// <summary>
      /// This event should be send to the player if he has used a map object.
      /// </summary>
      OpenCraftingWindow,

      /// <summary>
      /// This event should be send to the player if he is prompted to make a choice.
      /// </summary>
      OpenSelectionWindow,

      /// <summary>
      /// This event should be send to the player after login to inform him about the current time.
      /// </summary>
      UpdateTime,

      /// <summary>
      /// This event should be send to the player if another player introduces himself nearby.
      /// </summary>
      IntroduceCharacter,

      /// <summary>
      /// This event should be send to the player to inform him about his magical affiliation.
      /// </summary>
      UpdateMagicFlag,

      /// <summary>
      /// This event should be send to the player to inform him about the current weather.
      /// </summary>
      UpdateWeather,

      /// <summary>
      /// This event should be send to the player on map loading to inform him about all available quests.
      /// </summary>
      UpdateAllQuests,

      /// <summary>
      /// This event should be send to the player to update the status of a specific quest for him.
      /// </summary>
      UpdateQuest
    }
}
