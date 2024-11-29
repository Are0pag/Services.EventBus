using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Services.EventBus
{
    static public class EventBus<TBaseModuleType> 
        where TBaseModuleType : class
    {
       static private readonly Dictionary<Type, SubscribersList<TBaseModuleType>> _subscribers = new();

        static public void Subscribe(TBaseModuleType subscriber) {
            var subscriberTypes = TypeExposer<TBaseModuleType>.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                if (!_subscribers.ContainsKey(t)) 
                    _subscribers[t] = new SubscribersList<TBaseModuleType>();
                
                _subscribers[t].Add(subscriber);
            }
        }

        static public void Unsubscribe(TBaseModuleType subscriber) {
            var subscriberTypes = TypeExposer<TBaseModuleType>.GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes) {
                if (_subscribers.TryGetValue(t, out var subscriber1))
                    subscriber1.Remove(subscriber);
            }
        }

        static public void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
            where TSubscriber : class, TBaseModuleType 
        {
            var subscribers = _subscribers[typeof(TSubscriber)];

            subscribers.Executing = true;
            foreach (var subscriber in subscribers.List) {
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