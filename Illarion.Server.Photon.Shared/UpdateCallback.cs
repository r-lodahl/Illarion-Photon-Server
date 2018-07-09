using System;
using System.Collections.Generic;
using System.Timers;

namespace Illarion.Server.Photon
{
    public sealed class UpdateCallback
    {
        private readonly List<IDisposable> _updaters;

        public UpdateCallback() => _updaters = new List<IDisposable>();

        public void RegisterUpdater<T>(ITimedEventChannel channel, PlayerPeerBase peer, Action<PlayerPeerBase, List<T>> sendUpdatesAction)
            where T : ITimedEventChannel => _updaters.Add(new Updater<T>(channel, peer, sendUpdatesAction));

        public void UnregisterAll()
        {
            foreach (var updater in _updaters)
            {
                updater.Dispose();
            }

            _updaters.Clear();
        }

        internal sealed class Updater<T> : IDisposable where T : IEventUpdate
        {
            private readonly Timer _timer;
            private readonly List<T> _updates;
            private readonly Action<PlayerPeerBase, List<T>> _sendUpdatesAction;
            private readonly PlayerPeerBase _peer;
            private readonly ITimedEventChannel _channel;

            public Updater(ITimedEventChannel channel, PlayerPeerBase peer, Action<PlayerPeerBase, List<T>> sendUpdatesAction)
            {
                channel.EventReceived += OnEventUpdateReceived;
                _sendUpdatesAction = sendUpdatesAction;
                _updates = new List<T>();
                _peer = peer;
                _channel = channel;

                _timer = new Timer
                {
                    Interval = channel.UpdateFrequency
                };
                _timer.Elapsed += OnUpdate;
                _timer.Start();
            }

            public void OnEventUpdateReceived(object sender, IEventUpdate update)
            {
                if (update is T tUpdate)
                {
                    _updates.Add(tUpdate);
                }
                else
                {
                    throw new ArgumentException($"Expected {typeof(T)} but got {update.GetType()} as update.");
                }
            }

            public void OnUpdate(object sender, ElapsedEventArgs args)
            {
                _sendUpdatesAction(_peer, _updates);
                _timer.Interval = _channel.UpdateFrequency;
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