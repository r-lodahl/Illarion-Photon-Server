using System;

namespace Illarion.Server.Events
{
    /// <summary>
    /// Contains update data for one specific character
    /// </summary>
    public interface IEventUpdate
    {
        Guid CharacterId { get; }
    }
}
