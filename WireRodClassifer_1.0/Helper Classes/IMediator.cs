using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public interface IMediator
    {
        void AddParticipant(IParticipant participant);

        void BroadcastMessage(Message message);

        //event EventHandler<BroadcastEventArgs> BroadcastEvent;
    }

    public enum Message
    {
        FechaCaptura
    };
}
