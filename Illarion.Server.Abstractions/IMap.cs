using Illarion.Server.Chat;

namespace Illarion.Server
{
    public interface IMap
    {
        /// <summary>
        /// Chat channels are special channels used for communication of characters. Events posted here will not be sent to all characters on the map.
        /// </summary>
        /// <param name="channelType">The chat type to be used.</param>
        /// <returns>The channel for the specified channel type.</returns>
        IChatChannel GetChatChannel(MapChatChannelType channelType);

        /// <summary>
        /// Timed Event Channels allow to inform other character about actions of one character.
        /// The events should be communicated as packages with a fixed time interval.
        /// </summary>
        /// <param name="channelType">The event type of the channel.</param>
        /// <returns>The channel for the specified event type.</returns>
        ITimedEventChannel GetTimedEventChannel(MapEventChannelType channelType);

        /// <summary>
        /// Event Channels allow to inform other characters about actions of one character.
        /// The event is supposed to be communicated directly.
        /// </summary>
        /// <param name="channelType">The event type of the channel.</param>
        /// <returns>The channel for events of the specified type.</returns>
        IEventChannel GetEventChannel(MapEventChannelType channelType);

        /// <summary>
        /// Returns a Subscription to events triggered by the map itself.
        /// </summary>
        /// <param name="subscriber">The object to be subscribed.</param>
        /// <returns>The subscription of the subscriber.</returns>
        IMapSubscription Subscribe(IMapSubscriber subscriber);
    }
}