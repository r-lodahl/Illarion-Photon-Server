using System;
using System.Collections.Generic;
using System.Timers;
using Illarion.Server.Events;

namespace Illarion.Server.Photon
{
    public sealed class UpdateCallback
    {
        private readonly List<IDisposable> _updaters;

        public UpdateCallback() => _updaters = new List<IDisposable>();

        public void RegisterUpdater<T>(IEventChannel channel, PlayerPeerBase peer, Action<PlayerPeerBase, List<T>> sendUpdatesAction)
            where T : IEventUpdate => _updaters.Add(new Updater<T>(channel, peer, sendUpdatesAction, 100f));

        public void UnregisterAll()
        {
            foreach (IDisposable updater in _updaters)
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
            private readonly IEventChannel _channel;

            public Updater(IEventChannel channel, PlayerPeerBase peer, Action<PlayerPeerBase, List<T>> sendUpdatesAction, float initialUpdateFrequency)
            {
                channel.EventReceived += OnEventUpdateReceived;
                _sendUpdatesAction = sendUpdatesAction;
                _updates = new List<T>();
                _peer = peer;
                _channel = channel;

                _timer = new Timer
                {
                    Interval = initialUpdateFrequency
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

            //TODO: Change update frequency if needed
            public void OnUpdate(object sender, ElapsedEventArgs args) => _sendUpdatesAction(_peer, _updates);

            public void Dispose()
            {
                _channel.EventReceived -= OnEventUpdateReceived;
                _timer.Stop();
                _timer.Close();
                _updates.Clear();
            }
        }
    }
}