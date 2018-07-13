namespace Illarion.Net.Common.Events.Player
{
    public enum ChatMessageEventParameterCode
    {
        /// <summary>
        /// The character ID of the character this update relates to.
        /// Encoded as <see cref="System.Guid"/>
        /// </summary>
        CharacterId = 0,

        /// <summary>
        /// The message sent to the chat.
        /// Encoded as <see cref="string"/>.
        /// </summary>
        Message,

        /// <summary>
        /// The type of chat used for communication.
        /// Encoded as <see cref="byte"/>.
        /// </summary>
        ChatType
    }
}