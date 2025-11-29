using System;
using System.Linq;

/*
 * Base: Type - interface event type, SubscribersList<TBaseModuleType> - all instances, implements event type
 * Purpose: save Type and separate SubscribersList<TBaseModuleType> on invokerType and all instances, implements event type
 */

namespace Scripts.Services.EventBus
{
    static public class InvokerEventRaiser<TBaseModuleType> 
        where TBaseModuleType : class
    {
        static public void RaiseEvent<TSubscriber>(Action<TSubscriber> action, Type invokerType)
            where TSubscriber : class, TBaseModuleType 
        {
            EventBus<TBaseModuleType>.Subscribers.TryGetValue(typeof(TSubscriber), out var subscribers);
            if (subscribers == null) 
                return;

            var executingTypes = subscribers.List.Where(s => s.GetType().GetInterfaces().Contains(invokerType)).ToList();

            EventBus<TBaseModuleType>.ExecuteEventCallback<TSubscriber>(action, subscribers, executingTypes);
        }
    }
}