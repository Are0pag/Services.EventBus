using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Services.EventBus
{
    static public class EventBus<TBaseModuleType> 
        where TBaseModuleType : class
    {
       static internal readonly Dictionary<Type, SubscribersList<TBaseModuleType>> Subscribers = new();

        static public void Subscribe(TBaseModuleType subscriber) {
            var subscriberTypes = TypeExposer<TBaseModuleType>.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                if (!Subscribers.ContainsKey(t)) 
                    Subscribers[t] = new SubscribersList<TBaseModuleType>();
                
                Subscribers[t].Add(subscriber);
            }
        }

        static public void Unsubscribe(TBaseModuleType subscriber) {
            var subscriberTypes = TypeExposer<TBaseModuleType>.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                if (Subscribers.TryGetValue(t, out var subscriber1))
                    subscriber1.Remove(subscriber);
            }
        }

        static public void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
            where TSubscriber : class, TBaseModuleType 
        {
            Subscribers.TryGetValue(typeof(TSubscriber), out var subscribers);
            if (subscribers == null) 
                return;

            var executingTypes = subscribers.List;
            
            ExecuteEventCallback(action, subscribers, executingTypes);
        }

        static internal void ExecuteEventCallback<TSubscriber>(Action<TSubscriber> action, SubscribersList<TBaseModuleType> subscribers, List<TBaseModuleType> executingTypes) 
            where TSubscriber : class, TBaseModuleType 
        {
            subscribers.Executing = true;
            foreach (var subscriber in executingTypes) {
                try {
                    action.Invoke(subscriber as TSubscriber);
                }
                catch (Exception e) {
#if UNITY_EDITOR
                    Debug.Log($"{e.Message}\nStackTrace: {e.StackTrace}");
#endif
                }
            }

            subscribers.Executing = false;
            subscribers.Cleanup();
        }
    }
}