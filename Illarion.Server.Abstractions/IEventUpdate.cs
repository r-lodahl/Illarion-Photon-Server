using System;

namespace Illarion.Server
{
    /// <summary>
    /// Contains update data for one specific character
    /// </summary>
    public interface IEventUpdate
    {
        Guid CharacterId { get; }
    }
}
