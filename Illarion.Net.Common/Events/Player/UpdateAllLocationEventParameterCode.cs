namespace Illarion.Net.Common.Events.Player
{
    public enum UpdateAllLocationEventParameterCode : byte
    {
        /// <summary>
        /// The list of all locations. Encoded as <see cref="PlayerEventCode.UpdateLocation"/> responses.
        /// </summary>
        Locations = 0
    }
}
