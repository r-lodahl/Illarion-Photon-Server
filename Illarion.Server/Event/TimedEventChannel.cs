namespace Illarion.Server.Event
{
    internal class TimedEventChannel : EventChannel, ITimedEventChannel
    {
        public float UpdateFrequency { get; }

        public TimedEventChannel(MapEventChannelType channelType, float initialUpdateFrequency) : base(channelType) => UpdateFrequency = initialUpdateFrequency;
    }
}