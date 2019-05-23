using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public class ConcreteMediator : IMediator
    {
        List<IParticipant> participants = new List<IParticipant>();

        public void AddParticipant(IParticipant participant)
        {
            participants.Add(participant);
        }

        public void BroadcastMessage(Message message)
        {
            foreach (var participant in participants)
            {
                participant.ReceiveMessage(message);
            }
        }

        //public event EventHandler<BroadcastEventArgs> BroadcastEvent;

        //delegate void BroadcastDelegate(Message message);
        //BroadcastDelegate broadcastDelegate;

        //public ConcreteMediator()
        //{
        //    broadcastDelegate = new BroadcastDelegate(BroadcastMessage);
        //}

        ///// <summary>
        ///// Adiciona as classes que participam da troca de mensagens
        ///// </summary>
        ///// <param name="participant"></param>
        //public void AddParticipant(IParticipant participant)
        //{
        //    participants.Add(participant);
        //    subscribe(participant);
        //}

        //public void BroadcastMessage(Message message)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Se inscreve nos eventos de mensagem das classes participantes
        ///// </summary>
        ///// <param name="participant"></param>
        //private void subscribe(IParticipant participant)
        //{
        //    EventInfo eventInfo = participant.GetType().GetEvent("SendMessage");
        //    Type handlerType = eventInfo.EventHandlerType;
        //    var eventParams = handlerType.GetMethod("Invoke").GetParameters();
        //    //MethodInfo invokeMethod = handlerType.GetMethod("Invoke");
        //    MethodInfo mi = this.GetType().GetMethod("BroadcastMessage", BindingFlags.Public | BindingFlags.Instance);

        //    Delegate del = Delegate.CreateDelegate(eventInfo.EventHandlerType, mi);
        //    eventInfo.AddEventHandler(participant, del);

        //string eventName = "SendMessage";
        //var eventInfo = participant.GetType().GetEvent(eventName);
        //eventInfo.AddEventHandler(participant, EventProxy.Create<Message>(eventInfo, this.BroadcastMessage));
        //}

    }

    //static class EventProxy
    //{
    //    //void delegates with no parameters
    //    static public Delegate Create(EventInfo evt, Action d)
    //    {
    //        var handlerType = evt.EventHandlerType;
    //        var eventParams = handlerType.GetMethod("Invoke").GetParameters();

    //        //lambda: (object x0, EventArgs x1) => d()
    //        var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x"));
    //        var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"));
    //        var lambda = Expression.Lambda(body, parameters.ToArray());
    //        return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
    //    }

    //    //void delegate with one parameter
    //    static public Delegate Create<T>(EventInfo evt, Action<T> d)
    //    {
    //        var handlerType = evt.EventHandlerType;
    //        var eventParams = handlerType.GetMethod("Invoke").GetParameters();

    //        //lambda: (object x0, ExampleEventArgs x1) => d(x1.IntArg)
    //        var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x")).ToArray();
    //        var arg = getArgExpression(parameters[1], typeof(T));
    //        var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"), arg);
    //        var lambda = Expression.Lambda(body, parameters);
    //        return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
    //    }

    //    //returns an expression that represents an argument to be passed to the delegate
    //    static Expression getArgExpression(ParameterExpression eventArgs, Type handlerArgType)
    //    {
    //        if (eventArgs.Type == typeof(SendMessageEventHandler) && handlerArgType == typeof(int))
    //        {
    //            //"x1.IntArg"
    //            var memberInfo = eventArgs.Type.GetMember("IntArg")[0];
    //            return Expression.MakeMemberAccess(eventArgs, memberInfo);
    //        }

    //        throw new NotSupportedException(eventArgs + "->" + handlerArgType);
    //    }
    //}

}
