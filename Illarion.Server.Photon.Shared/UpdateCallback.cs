using System;
using System.Collections.Generic;
using System.Timers;

namespace Illarion.Server.Photon
{
    public sealed class UpdateCallback
    {
      private readonly List<Updater> _updaters;

      public UpdateCallback() => _updaters = new List<Updater>();

      public void RegisterUpdater(IEventChannel channel, PlayerPeerBase peer) => _updaters.Add(new Updater(channel, peer));

      public void UnregisterAll()
      {
        foreach (var updater in _updaters)
        {
          updater.Dispose();
        }
        _updaters.Clear();
      }

      internal sealed class Updater : IDisposable
      {
        private readonly PlayerPeerBase _peer; //TODO: Try to rem the peer here
        private readonly Timer _timer;
        private readonly List<IEventUpdate> _updates;

        public Updater(IEventChannel channel, PlayerPeerBase peer)
        {
          channel.EventReceived += OnEventUpdateReceived;
          _peer = peer;
          _updates = new List<IEventUpdate>();

          _timer = new Timer
          {
            Interval = channel.UpdateFrequency
          };
          _timer.Elapsed += OnUpdate;
          _timer.Start();
        }

        public void OnEventUpdateReceived(object sender, IEventUpdate update) => _updates.Add(update);

        public void OnUpdate(object sender, ElapsedEventArgs args)
        {
          //TODO: implement
          foreach (var update in _updates)
          {
            // Pack the updates together
          }
          _updates.Clear();

          // peer.SendMessage(...)
        }

        public void Dispose()
        {
          _timer.Stop();
          _timer.Close();
          _updates.Clear();
        }
      }
    }
}
