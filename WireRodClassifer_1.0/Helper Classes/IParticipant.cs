using System;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public interface IParticipant
    {
        void ReceiveMessage(Message message);

        void AddMediator(IMediator mediator);
       //event EventHandler<SendMessageEventArgs> SendMessage; 
    }
}