namespace Illarion.Server
{
    public interface IPlayerFactory
    {
      ICharacterCallback DefaultCharacterCallback(ICharacter character);
    }
}
