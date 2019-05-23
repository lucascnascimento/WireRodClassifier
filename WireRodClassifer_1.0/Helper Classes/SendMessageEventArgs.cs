namespace WireRodClassifer_1._0.Helper_Classes
{
    public class SendMessageEventArgs
    {
        private Message _messageToBeSent;

        public Message MessageToBeSent { get => _messageToBeSent; private set => _messageToBeSent = value; }

        public SendMessageEventArgs( Message messageToBeSent)
        {
            MessageToBeSent = messageToBeSent;
        }
    }
}