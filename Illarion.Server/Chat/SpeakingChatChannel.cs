namespace Illarion.Server.Chat
{
    internal class SpeakingChatChannel : ChatChannel
    {
        protected override float MaximalChatDistance() => 10f;
    }
}