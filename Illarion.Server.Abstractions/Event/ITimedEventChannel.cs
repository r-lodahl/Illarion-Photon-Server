namespace Illarion.Server
{
    public interface ITimedEventChannel : IEventChannel
    {
        /// <summary> Recommended update frequency of this channel in ms </summary>
        float UpdateFrequency { get; }
    }
}
