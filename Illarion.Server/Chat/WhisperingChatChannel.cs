namespace Illarion.Server.Chat
{
    internal class WhisperingChatChannel : ChatChannel
    {
        protected override float MaximalChatDistance() => 10f;
    }
}