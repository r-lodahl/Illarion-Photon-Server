using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Illarion.Server
{
  public interface IWorldManager
  {
    int WorldCount { get; }

    IWorld GetWorld(int index);
  }
}
