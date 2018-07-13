using Illarion.Server.Event;

namespace Illarion.Server
{
    public interface IMap
    {
        /// <summary>
        /// Event Channels allow to inform other characters about actions of one character.
        /// The event is supposed to be communicated directly.
        /// </summary>
        /// <param name="channelType">The event type of the channel.</param>
        /// <returns>The channel for events of the specified type.</returns>
        IEventChannel GetEventChannel(MapEventChannelType channelType);

        /// <summary>
        /// Retrieves a chat channel from the IMap.
        /// Use this for chat instead of directly calling <see cref="GetEventChannel"/>.
        /// </summary>
        /// <param name="channelType">The type of the chat channel.</param>
        /// <returns>The correct event channel type.</returns>
        IEventChannel GetChatChannel(MapChatChannelType channelType);

        /// <summary>
        /// Returns a Subscription to events triggered by the map itself.
        /// </summary>
        /// <param name="subscriber">The object to be subscribed.</param>
        /// <returns>The subscription of the subscriber.</returns>
        IMapSubscription Subscribe(IMapSubscriber subscriber);
    }
}