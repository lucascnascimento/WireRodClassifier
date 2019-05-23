using System;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public class BroadcastEventArgs : EventArgs
    {
        private Message _broadcastMessage;

        public Message BroadcastMessage { get => _broadcastMessage; private set => _broadcastMessage = value; }

        public BroadcastEventArgs(Message broadcastMessage)
        {
            BroadcastMessage = broadcastMessage;
        }
    }
}