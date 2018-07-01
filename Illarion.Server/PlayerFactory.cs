using System;
using System.Numerics;

namespace Illarion.Server
{
    internal class PlayerFactory : IPlayerFactory
    {
      public ICharacterCallback DefaultCharacterCallback(ICharacter character) => new DefaultCharacterCallback();
    }
}
