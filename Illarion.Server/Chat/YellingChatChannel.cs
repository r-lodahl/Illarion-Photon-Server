namespace Illarion.Server.Chat
{
    internal class YellingChatChannel : ChatChannel
    {
        protected override float MaximalChatDistance() => 10f;
    }
}