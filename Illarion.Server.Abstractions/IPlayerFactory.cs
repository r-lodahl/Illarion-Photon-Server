using System;
using System.Collections.Generic;
using System.Text;

namespace Illarion.Server
{
    public interface IPlayerFactory
    {
      ICharacterCallback DefaultCharacterCallback(ICharacter character);
    }
}