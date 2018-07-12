namespace Illarion.Net.Common.Operations.Player
{
    public enum SendMessageOperationRequestParameterCode : byte
    {
        /// <summary>
        /// The chat type used for the message send by the character.
        /// Encoded as <see cref="byte"/>.
        /// </summary>
        ChatType = 0,

        /// <summary>
        /// The message send by the character.
        /// Encoded as <see cref="string"/>.
        /// </summary>
        Message
    }
}