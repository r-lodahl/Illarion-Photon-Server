namespace Illarion.Server.Chat
{
    internal class GlobalChatChannel : ChatChannel
    {
        protected override float MaximalChatDistance() => 10f;
    }
}